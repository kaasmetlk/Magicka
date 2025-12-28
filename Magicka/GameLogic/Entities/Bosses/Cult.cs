using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.AI.AgentStates;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020004C5 RID: 1221
	public class Cult : IBoss
	{
		// Token: 0x0600248D RID: 9357 RVA: 0x00107B24 File Offset: 0x00105D24
		public unsafe Cult(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mTemplates = new CharacterTemplate[5];
			lock (Game.Instance.GraphicsDevice)
			{
				this.mTemplates[0] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Cold");
				this.mTemplates[1] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Fire");
				this.mTemplates[2] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Lightning");
				this.mTemplates[3] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Nature");
				this.mTemplates[4] = this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Cult_Leader");
			}
			this.mWarlocks = new List<NonPlayerCharacter>(26);
			this.mSpectators = new List<NonPlayerCharacter>(26);
			this.mFightingGoblins = new List<NonPlayerCharacter>(26);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				for (int i = 0; i < 26; i++)
				{
					NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
					this.mWarlocks.Add(instance);
					this.mSpectators.Add(instance);
				}
				this.mLeaderWarlock = NonPlayerCharacter.GetInstance(this.mPlayState);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Cult.InitializeMessage initializeMessage = default(Cult.InitializeMessage);
					initializeMessage.LeaderHandle = this.mLeaderWarlock.Handle;
					for (int j = 0; j < 26; j++)
					{
						(&initializeMessage.Handles.FixedElementField)[j] = this.mWarlocks[j].Handle;
					}
					BossFight.Instance.SendInitializeMessage<Cult.InitializeMessage>(this, 4, (void*)(&initializeMessage));
				}
			}
			this.mIntroState = new Cult.IntroState();
			this.mBattleState = new Cult.BattleState();
			this.mReviveState = new Cult.ReviveState();
			this.mSpectatorState = new Cult.SpectatorState();
			this.mFinalState = new Cult.FinalState();
			this.mDeadState = new Cult.DeadState();
			this.mTeleportEmitter = new AudioEmitter();
			this.mNetworkInitialized = (NetworkManager.Instance.State != NetworkState.Client);
		}

		// Token: 0x0600248E RID: 9358 RVA: 0x00107D4C File Offset: 0x00105F4C
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x0600248F RID: 9359 RVA: 0x00107D58 File Offset: 0x00105F58
		public void Initialize(ref Matrix iOrientation)
		{
			this.mWarlockDirection = new Vector3[5][];
			this.mWarlockPosition = new Vector3[5][];
			for (int i = 0; i < this.mWarlockPosition.Length; i++)
			{
				this.mWarlockPosition[i] = new Vector3[2];
				this.mWarlockDirection[i] = new Vector3[2];
				Matrix matrix;
				this.mPlayState.Level.CurrentScene.GetLocator(Cult.WARLOCK_POSITIONS[i, 0], out matrix);
				this.mWarlockPosition[i][0] = matrix.Translation;
				this.mWarlockDirection[i][0] = matrix.Forward;
			}
			this.mLeaderReviveMoveEvent = new AIEvent[1];
			this.mLeaderReviveMoveEvent[0].EventType = AIEventType.Move;
			this.mLeaderReviveMoveEvent[0].MoveEvent.Direction = this.mWarlockDirection[0][0];
			this.mLeaderReviveMoveEvent[0].MoveEvent.Waypoint = this.mWarlockPosition[0][0];
			this.mLeaderReviveMoveEvent[0].MoveEvent.FixedDirection = true;
			this.mLeaderReviveMoveEvent[0].MoveEvent.Speed = 2f;
			this.mLeaderReviveMoveEvent[0].MoveEvent.Delay = 0.1f;
			this.mLeaderReviveAnimation = new AIEvent[1];
			this.mLeaderReviveAnimation[0].EventType = AIEventType.Animation;
			this.mLeaderReviveAnimation[0].AnimationEvent.Animation = Animations.cast_spell0;
			this.mLeaderReviveAnimation[0].AnimationEvent.BlendTime = 0.25f;
			this.mLeaderReviveAnimation[0].AnimationEvent.Delay = 0f;
			this.mFightingGoblins.Clear();
			this.mSpectators.Clear();
			for (int j = 0; j < 26; j++)
			{
				string iString = string.Format("spectator{0}", j);
				int hashCodeCustom = iString.GetHashCodeCustom();
				Matrix orientation;
				this.mPlayState.Level.CurrentScene.GetLocator(hashCodeCustom, out orientation);
				Vector3 translation = orientation.Translation;
				orientation.Translation = default(Vector3);
				if (j == 0)
				{
					this.mWarlocks[j].Initialize(this.mTemplates[j % 4], translation, "bill".GetHashCodeCustom(), 2f);
				}
				else if (j == 1)
				{
					this.mWarlocks[j].Initialize(this.mTemplates[j % 4], translation, "bull".GetHashCodeCustom(), 2f);
				}
				else
				{
					this.mWarlocks[j].Initialize(this.mTemplates[j % 4], translation, 0, 2f);
				}
				this.mWarlocks[j].Body.MoveTo(translation, orientation);
				switch (j % 4)
				{
				case 0:
					this.mWarlocks[j].Color = Spell.COLDCOLOR;
					break;
				case 1:
					this.mWarlocks[j].Color = Spell.FIRECOLOR;
					break;
				case 2:
					this.mWarlocks[j].Color = Spell.LIGHTNINGCOLOR;
					break;
				case 3:
					this.mWarlocks[j].Color = Spell.LIFECOLOR;
					break;
				}
				this.mWarlocks[j].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
				this.mPlayState.EntityManager.AddEntity(this.mWarlocks[j]);
				this.mSpectators.Add(this.mWarlocks[j]);
				this.mTotalMaxHitPoints += this.mWarlocks[j].MaxHitPoints;
			}
			AIEvent[] array = new AIEvent[1];
			array[0].EventType = AIEventType.Animation;
			array[0].AnimationEvent.Animation = Animations.idlelong_agg;
			array[0].AnimationEvent.BlendTime = 0.25f;
			array[0].AnimationEvent.Delay = 0f;
			this.mLeaderWarlock.Initialize(this.mTemplates[4], Vector3.Zero, "leader".GetHashCodeCustom(), float.MaxValue);
			this.mLeaderWarlock.Body.MoveTo(this.mWarlockPosition[0][0], Matrix.CreateWorld(Vector3.Zero, this.mWarlockDirection[0][0], Vector3.Up));
			this.mLeaderWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, array);
			this.mLeaderWarlock.Color = Vector3.Zero;
			this.mPlayState.EntityManager.AddEntity(this.mLeaderWarlock);
			this.mSpectators[0].Body.MoveTo(this.mWarlockPosition[1][0], Matrix.CreateWorld(Vector3.Zero, this.mWarlockDirection[1][0], Vector3.Up));
			this.mFightingGoblins.Add(this.mSpectators[0]);
			this.mSpectators.Remove(this.mSpectators[0]);
			this.mSpectators[0].Body.MoveTo(this.mWarlockPosition[4][0], Matrix.CreateWorld(Vector3.Zero, this.mWarlockDirection[4][0], Vector3.Up));
			this.mFightingGoblins.Add(this.mSpectators[0]);
			this.mSpectators.Remove(this.mSpectators[0]);
			this.mTotalMaxHitPoints += this.mLeaderWarlock.MaxHitPoints;
			this.mCurrentState = this.mIntroState;
			this.mCurrentState.OnEnter(this);
			this.mPlayers = Game.Instance.Players;
			this.mReviveeTemplateType = new int[5];
			this.mBattleState.mSpectatorShow = false;
			this.mDead = false;
			this.mSpectatorState.Reset();
		}

		// Token: 0x06002490 RID: 9360 RVA: 0x00108390 File Offset: 0x00106590
		private IBossState<Cult> GetState(Cult.States iState)
		{
			switch (iState)
			{
			case Cult.States.Intro:
				return this.mIntroState;
			case Cult.States.Battle:
				return this.mBattleState;
			case Cult.States.Revive:
				return this.mReviveState;
			case Cult.States.Spectator:
				return this.mSpectatorState;
			case Cult.States.Final:
				return this.mFinalState;
			case Cult.States.Dead:
				return this.mDeadState;
			default:
				return null;
			}
		}

		// Token: 0x06002491 RID: 9361 RVA: 0x001083EC File Offset: 0x001065EC
		protected unsafe void ChangeState(Cult.States iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Cult.ChangeStateMessage changeStateMessage;
					changeStateMessage.State = iState;
					BossFight.Instance.SendMessage<Cult.ChangeStateMessage>(this, 2, (void*)(&changeStateMessage), true);
				}
				this.mCurrentState.OnExit(this);
				this.mCurrentState = this.GetState(iState);
				this.mCurrentState.OnEnter(this);
			}
		}

		// Token: 0x06002492 RID: 9362 RVA: 0x00108450 File Offset: 0x00106650
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (this.mCurrentState is Cult.IntroState && iFightStarted)
			{
				this.ChangeState(Cult.States.Battle);
			}
			this.mTotalHitPoints = 0f;
			if (this.mCurrentState is Cult.FinalState)
			{
				for (int i = 0; i < this.mFightingGoblins.Count; i++)
				{
					this.mTotalHitPoints += Math.Max(this.mFightingGoblins[i].HitPoints, 0f);
				}
			}
			else
			{
				for (int j = 0; j < this.mWarlocks.Count; j++)
				{
					this.mTotalHitPoints += Math.Max(this.mWarlocks[j].HitPoints, 0f);
				}
				for (int k = 0; k < this.mFightingGoblins.Count; k++)
				{
					if (this.mFightingGoblins[k] == null || this.mFightingGoblins[k].Dead)
					{
						this.mFightingGoblins.RemoveAt(k--);
					}
				}
			}
			this.mTotalHitPoints += Math.Max(this.mLeaderWarlock.HitPoints, 0f);
			if (this.mLeaderWarlock.HitPoints <= 0f && !(this.mCurrentState is Cult.DeadState))
			{
				this.ChangeState(Cult.States.Dead);
			}
			this.mCurrentState.OnUpdate(iDeltaTime, this);
		}

		// Token: 0x06002493 RID: 9363 RVA: 0x001085AC File Offset: 0x001067AC
		public unsafe void TeleportWarlock(NonPlayerCharacter iWarlock, int iLocator)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				Matrix matrix;
				this.mPlayState.Level.CurrentScene.GetLocator(iLocator, out matrix);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Cult.TeleportMessage teleportMessage;
					teleportMessage.Handle = iWarlock.Handle;
					teleportMessage.Position = matrix.Translation;
					teleportMessage.Direction = matrix.Forward;
					BossFight.Instance.SendMessage<Cult.ChangeStateMessage>(this, 2, (void*)(&teleportMessage), true);
				}
				this.TeleportWarlock(iWarlock, matrix.Translation, matrix.Forward);
			}
		}

		// Token: 0x06002494 RID: 9364 RVA: 0x0010863C File Offset: 0x0010683C
		public void TeleportWarlock(NonPlayerCharacter iWarlock, Vector3 iPosition, Vector3 iDirection)
		{
			Vector3 vector = iWarlock.Position;
			Matrix orientation = iWarlock.Body.Orientation;
			Vector3 right = Vector3.Right;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref vector, ref right, out visualEffectReference);
			this.mTeleportEmitter.Position = vector;
			this.mTeleportEmitter.Up = Vector3.Up;
			this.mTeleportEmitter.Forward = orientation.Forward;
			AudioManager.Instance.PlayCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN, this.mTeleportEmitter);
			Vector3 up = Vector3.Up;
			Vector3 zero = Vector3.Zero;
			Matrix.CreateWorld(ref zero, ref iDirection, ref up, out orientation);
			vector = iPosition;
			Segment iSeg = default(Segment);
			iSeg.Origin = vector;
			iSeg.Delta.Y = iSeg.Delta.Y - 5f;
			float num;
			Vector3 vector2;
			Vector3 vector3;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, iSeg))
			{
				vector = vector2;
			}
			vector.Y += iWarlock.Capsule.Radius + iWarlock.Capsule.Length * 0.5f + 0.1f;
			iWarlock.CharacterBody.MoveTo(vector, orientation);
			EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref vector, ref right, out visualEffectReference);
			AudioManager.Instance.PlayCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION, iWarlock.AudioEmitter);
		}

		// Token: 0x06002495 RID: 9365 RVA: 0x00108791 File Offset: 0x00106991
		public void DeInitialize()
		{
		}

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x06002496 RID: 9366 RVA: 0x00108793 File Offset: 0x00106993
		public bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x06002497 RID: 9367 RVA: 0x0010879B File Offset: 0x0010699B
		public float MaxHitPoints
		{
			get
			{
				return this.mTotalMaxHitPoints;
			}
		}

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x06002498 RID: 9368 RVA: 0x001087A3 File Offset: 0x001069A3
		public float HitPoints
		{
			get
			{
				return this.mTotalHitPoints;
			}
		}

		// Token: 0x06002499 RID: 9369 RVA: 0x001087AB File Offset: 0x001069AB
		public void ScriptMessage(BossMessages iMessage)
		{
		}

		// Token: 0x0600249A RID: 9370 RVA: 0x001087AD File Offset: 0x001069AD
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600249B RID: 9371 RVA: 0x001087B4 File Offset: 0x001069B4
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600249C RID: 9372 RVA: 0x001087BB File Offset: 0x001069BB
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600249D RID: 9373 RVA: 0x001087C2 File Offset: 0x001069C2
		public void SetSlow(int iIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600249E RID: 9374 RVA: 0x001087C9 File Offset: 0x001069C9
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600249F RID: 9375 RVA: 0x001087D0 File Offset: 0x001069D0
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060024A0 RID: 9376 RVA: 0x001087D7 File Offset: 0x001069D7
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060024A1 RID: 9377 RVA: 0x001087DE File Offset: 0x001069DE
		public StatusEffect[] GetStatusEffects()
		{
			return null;
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x001087E4 File Offset: 0x001069E4
		public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			switch (iMsg.Type)
			{
			case 1:
			{
				Cult.TeleportMessage teleportMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&teleportMessage));
				NonPlayerCharacter nonPlayerCharacter = Entity.GetFromHandle((int)teleportMessage.Handle) as NonPlayerCharacter;
				Vector3 position = nonPlayerCharacter.Position;
				Matrix orientation = nonPlayerCharacter.Body.Orientation;
				Vector3 right = Vector3.Right;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref position, ref right, out visualEffectReference);
				this.mTeleportEmitter.Position = position;
				this.mTeleportEmitter.Up = Vector3.Up;
				this.mTeleportEmitter.Forward = orientation.Forward;
				AudioManager.Instance.PlayCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN, this.mTeleportEmitter);
				Vector3 up = Vector3.Up;
				Vector3 zero = Vector3.Zero;
				Matrix.CreateWorld(ref zero, ref teleportMessage.Direction, ref up, out orientation);
				position = teleportMessage.Position;
				nonPlayerCharacter.CharacterBody.MoveTo(position, orientation);
				EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref position, ref right, out visualEffectReference);
				AudioManager.Instance.PlayCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION, nonPlayerCharacter.AudioEmitter);
				return;
			}
			case 2:
			{
				Cult.ChangeStateMessage changeStateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
				this.mCurrentState.OnExit(this);
				this.mCurrentState = this.GetState(changeStateMessage.State);
				this.mCurrentState.OnEnter(this);
				return;
			}
			case 3:
			{
				Cult.ReviveMessage reviveMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&reviveMessage));
				int index = reviveMessage.Index;
				Vector3 position2 = reviveMessage.Position;
				Vector3 direction = reviveMessage.Direction;
				this.mWarlocks[index].Initialize(this.mTemplates[reviveMessage.Index % 4], position2, 0, float.MaxValue);
				this.mWarlocks[index].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
				this.mWarlocks[index].CharacterBody.DesiredDirection = direction;
				switch (index % 4)
				{
				case 0:
					this.mWarlocks[index].Color = Spell.COLDCOLOR;
					break;
				case 1:
					this.mWarlocks[index].Color = Spell.FIRECOLOR;
					break;
				case 2:
					this.mWarlocks[index].Color = Spell.LIGHTNINGCOLOR;
					break;
				case 3:
					this.mWarlocks[index].Color = Spell.LIFECOLOR;
					break;
				}
				VisualEffectReference visualEffectReference2;
				EffectManager.Instance.StartEffect(Revive.EFFECT, ref position2, ref direction, out visualEffectReference2);
				AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH, this.mWarlocks[index].AudioEmitter);
				if (!this.mPlayState.EntityManager.Entities.Contains(this.mWarlocks[index]))
				{
					this.mPlayState.EntityManager.AddEntity(this.mWarlocks[index]);
				}
				this.mFightingGoblins.Add(this.mWarlocks[index]);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x060024A3 RID: 9379 RVA: 0x00108ACC File Offset: 0x00106CCC
		public unsafe void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			if (iMsg.Type == 4)
			{
				Cult.InitializeMessage initializeMessage;
				BossInitializeMessage.ConvertTo(ref iMsg, (void*)(&initializeMessage));
				this.mLeaderWarlock = (Entity.GetFromHandle((int)initializeMessage.LeaderHandle) as NonPlayerCharacter);
				for (int i = 0; i < 26; i++)
				{
					this.mWarlocks.Add(Entity.GetFromHandle((int)(&initializeMessage.Handles.FixedElementField)[i]) as NonPlayerCharacter);
					this.mSpectators.Add(Entity.GetFromHandle((int)(&initializeMessage.Handles.FixedElementField)[i]) as NonPlayerCharacter);
				}
				this.mNetworkInitialized = true;
			}
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x00108B69 File Offset: 0x00106D69
		public BossEnum GetBossType()
		{
			return BossEnum.Cult;
		}

		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x060024A5 RID: 9381 RVA: 0x00108B6C File Offset: 0x00106D6C
		public bool NetworkInitialized
		{
			get
			{
				return this.mNetworkInitialized;
			}
		}

		// Token: 0x060024A6 RID: 9382 RVA: 0x00108B74 File Offset: 0x00106D74
		public float ResistanceAgainst(Elements iElement)
		{
			return 1f;
		}

		// Token: 0x060024A7 RID: 9383 RVA: 0x00108B7C File Offset: 0x00106D7C
		// Note: this type is marked as 'beforefieldinit'.
		static Cult()
		{
			int[,] array = new int[5, 2];
			array[0, 0] = "spawn_warlock5".GetHashCodeCustom();
			array[0, 1] = "spectate_warlock5".GetHashCodeCustom();
			array[1, 0] = "spawn_warlock1".GetHashCodeCustom();
			array[1, 1] = "spectate_warlock1".GetHashCodeCustom();
			array[2, 0] = "spawn_warlock2".GetHashCodeCustom();
			array[2, 1] = "spectate_warlock2".GetHashCodeCustom();
			array[3, 0] = "spawn_warlock3".GetHashCodeCustom();
			array[3, 1] = "spectate_warlock3".GetHashCodeCustom();
			array[4, 0] = "spawn_warlock4".GetHashCodeCustom();
			array[4, 1] = "spectate_warlock4".GetHashCodeCustom();
			Cult.WARLOCK_POSITIONS = array;
		}

		// Token: 0x040027D7 RID: 10199
		private const int SPAWN = 0;

		// Token: 0x040027D8 RID: 10200
		private const int SPECTATE = 1;

		// Token: 0x040027D9 RID: 10201
		private const int SPECTATOR_COUNT = 26;

		// Token: 0x040027DA RID: 10202
		private static readonly int DIALOG_INTRO = "cult_intro".GetHashCodeCustom();

		// Token: 0x040027DB RID: 10203
		private static readonly int DIALOG_REVIVE = "cult_domyself".GetHashCodeCustom();

		// Token: 0x040027DC RID: 10204
		private static readonly int DIALOG_TAUNT1 = "cult_taunt1".GetHashCodeCustom();

		// Token: 0x040027DD RID: 10205
		private static readonly int DIALOG_TAUNT2 = "cult_taunt2".GetHashCodeCustom();

		// Token: 0x040027DE RID: 10206
		private static readonly int DIALOG_DEATH = "cult_death".GetHashCodeCustom();

		// Token: 0x040027DF RID: 10207
		private static readonly int DEFEATED_TRIGGER = "boss_defeated".GetHashCodeCustom();

		// Token: 0x040027E0 RID: 10208
		private static readonly Random sRandom = new Random();

		// Token: 0x040027E1 RID: 10209
		private static readonly int[,] WARLOCK_POSITIONS;

		// Token: 0x040027E2 RID: 10210
		private Vector3[][] mWarlockPosition;

		// Token: 0x040027E3 RID: 10211
		private Vector3[][] mWarlockDirection;

		// Token: 0x040027E4 RID: 10212
		private float mTotalMaxHitPoints;

		// Token: 0x040027E5 RID: 10213
		private float mTotalHitPoints;

		// Token: 0x040027E6 RID: 10214
		private bool mDead;

		// Token: 0x040027E7 RID: 10215
		private Cult.IntroState mIntroState;

		// Token: 0x040027E8 RID: 10216
		private Cult.BattleState mBattleState;

		// Token: 0x040027E9 RID: 10217
		private Cult.ReviveState mReviveState;

		// Token: 0x040027EA RID: 10218
		private Cult.SpectatorState mSpectatorState;

		// Token: 0x040027EB RID: 10219
		private Cult.FinalState mFinalState;

		// Token: 0x040027EC RID: 10220
		private Cult.DeadState mDeadState;

		// Token: 0x040027ED RID: 10221
		private AIEvent[] mLeaderReviveMoveEvent;

		// Token: 0x040027EE RID: 10222
		private AIEvent[] mLeaderReviveAnimation;

		// Token: 0x040027EF RID: 10223
		private NonPlayerCharacter mLeaderWarlock;

		// Token: 0x040027F0 RID: 10224
		private List<NonPlayerCharacter> mWarlocks;

		// Token: 0x040027F1 RID: 10225
		private List<NonPlayerCharacter> mSpectators;

		// Token: 0x040027F2 RID: 10226
		private List<NonPlayerCharacter> mFightingGoblins;

		// Token: 0x040027F3 RID: 10227
		private Player[] mPlayers;

		// Token: 0x040027F4 RID: 10228
		private PlayState mPlayState;

		// Token: 0x040027F5 RID: 10229
		private IBossState<Cult> mCurrentState;

		// Token: 0x040027F6 RID: 10230
		private CharacterTemplate[] mTemplates;

		// Token: 0x040027F7 RID: 10231
		private AudioEmitter mTeleportEmitter;

		// Token: 0x040027F8 RID: 10232
		private int[] mReviveeTemplateType;

		// Token: 0x040027F9 RID: 10233
		private bool mNetworkInitialized;

		// Token: 0x020004C6 RID: 1222
		public class IntroState : IBossState<Cult>
		{
			// Token: 0x060024A8 RID: 9384 RVA: 0x00108CAF File Offset: 0x00106EAF
			public void OnEnter(Cult iOwner)
			{
			}

			// Token: 0x060024A9 RID: 9385 RVA: 0x00108CB1 File Offset: 0x00106EB1
			public void OnUpdate(float iDeltaTime, Cult iOwner)
			{
			}

			// Token: 0x060024AA RID: 9386 RVA: 0x00108CB3 File Offset: 0x00106EB3
			public void OnExit(Cult iOwner)
			{
			}
		}

		// Token: 0x020004C7 RID: 1223
		public class BattleState : IBossState<Cult>
		{
			// Token: 0x060024AC RID: 9388 RVA: 0x00108CBD File Offset: 0x00106EBD
			public void OnEnter(Cult iOwner)
			{
				this.mSpawnArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea("trigger_spawn_area".GetHashCodeCustom());
				this.mPauseTimer = 0f;
			}

			// Token: 0x060024AD RID: 9389 RVA: 0x00108CF0 File Offset: 0x00106EF0
			public void OnUpdate(float iDeltaTime, Cult iOwner)
			{
				bool flag = true;
				for (int i = 0; i < iOwner.mFightingGoblins.Count; i++)
				{
					if (!iOwner.mFightingGoblins[i].Dead)
					{
						flag = false;
					}
				}
				if (flag)
				{
					if (this.mSpectatorShow)
					{
						if (this.mPauseTimer <= 0f)
						{
							this.mPauseTimer = 2.5f;
							return;
						}
						this.mPauseTimer -= iDeltaTime;
						if (this.mPauseTimer <= 1.25f && this.mPauseTimer > 0f)
						{
							if (!DialogManager.Instance.DialogActive(Cult.DIALOG_TAUNT1) && !DialogManager.Instance.DialogActive(Cult.DIALOG_TAUNT2))
							{
								DialogManager.Instance.StartDialog((Cult.sRandom.Next(2) == 0) ? Cult.DIALOG_TAUNT1 : Cult.DIALOG_TAUNT2, iOwner.mLeaderWarlock, null);
								return;
							}
						}
						else if (this.mPauseTimer <= 0f)
						{
							if (iOwner.mSpectators.Count > 0)
							{
								int num = Math.Min(iOwner.mSpectators.Count, 4);
								int j = 0;
								int num2 = 0;
								while (j < num)
								{
									Matrix matrix;
									iOwner.mPlayState.Level.CurrentScene.GetLocator(Cult.WARLOCK_POSITIONS[1 + num2, 0], out matrix);
									iOwner.TeleportWarlock(iOwner.mSpectators[j], matrix.Translation, matrix.Forward);
									iOwner.mSpectators[j].AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
									iOwner.mFightingGoblins.Add(iOwner.mSpectators[j]);
									iOwner.mSpectators.Remove(iOwner.mSpectators[j]);
									j--;
									num--;
									j++;
									num2++;
								}
								return;
							}
							iOwner.ChangeState(Cult.States.Revive);
							return;
						}
					}
					else
					{
						iOwner.ChangeState(Cult.States.Spectator);
					}
				}
			}

			// Token: 0x060024AE RID: 9390 RVA: 0x00108EC7 File Offset: 0x001070C7
			public void OnExit(Cult iOwner)
			{
			}

			// Token: 0x040027FA RID: 10234
			private const float PAUSE_TIME = 2.5f;

			// Token: 0x040027FB RID: 10235
			private float mPauseTimer;

			// Token: 0x040027FC RID: 10236
			public bool mSpectatorShow;

			// Token: 0x040027FD RID: 10237
			private TriggerArea mSpawnArea;
		}

		// Token: 0x020004C8 RID: 1224
		public class SpectatorState : IBossState<Cult>
		{
			// Token: 0x060024B0 RID: 9392 RVA: 0x00108ED4 File Offset: 0x001070D4
			public void OnEnter(Cult iOwner)
			{
				this.mSpawnArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea("area_temp".GetHashCodeCustom());
				AIEvent[] array = new AIEvent[1];
				array[0].EventType = AIEventType.Animation;
				array[0].AnimationEvent.Animation = Animations.spec_alert3;
				array[0].AnimationEvent.Delay = 0.1f;
				array[0].AnimationEvent.BlendTime = 0.3f;
				iOwner.mPlayState.Camera.LockInput = true;
				this.mTimer = 0f;
				this.mPan = false;
				this.mChangedState = false;
			}

			// Token: 0x060024B1 RID: 9393 RVA: 0x00108F88 File Offset: 0x00107188
			public void OnUpdate(float iDeltaTime, Cult iOwner)
			{
				this.mTimer += iDeltaTime;
				if (this.mPan)
				{
					if (this.mTimer > 2.75f)
					{
						if (this.mTeleported && !this.mChangedState)
						{
							iOwner.mPlayState.Camera.Release(2f);
							iOwner.ChangeState(Cult.States.Battle);
							this.mChangedState = true;
							for (int i = 0; i < 4; i++)
							{
								iOwner.mFightingGoblins.Add(iOwner.mSpectators[8]);
								iOwner.mSpectators.Remove(iOwner.mSpectators[8]);
							}
							for (int j = 0; j < iOwner.mFightingGoblins.Count; j++)
							{
								iOwner.mFightingGoblins[j].AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
							}
							return;
						}
						if (this.mRelease)
						{
							for (int k = 8; k < 12; k++)
							{
								Matrix matrix;
								iOwner.mPlayState.Level.CurrentScene.GetLocator(Cult.WARLOCK_POSITIONS[1 + k % 4, 0], out matrix);
								iOwner.TeleportWarlock(iOwner.mSpectators[k], matrix.Translation, matrix.Forward);
							}
							this.mTeleported = true;
							this.mTimer -= 0.75f;
							return;
						}
						if (this.mBackPan)
						{
							iOwner.mPlayState.Camera.MoveTo(iOwner.mWarlockPosition[0][0], 1.75f);
							this.mRelease = true;
							this.mTimer -= 2.25f;
							return;
						}
						Vector3 right = Vector3.Right;
						for (int l = 8; l < 12; l++)
						{
							Vector3 randomLocation = this.mSpawnArea.GetRandomLocation();
							iOwner.TeleportWarlock(iOwner.mSpectators[l], randomLocation, right);
							iOwner.mSpectators[l].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
						}
						this.mBackPan = true;
						this.mTimer -= 1f;
						return;
					}
				}
				else if (this.mTimer > 1.5f)
				{
					iOwner.mPlayState.Camera.MoveTo(iOwner.mSpectators[8].Position, 2f);
					this.mTimer = 0f;
					this.mPan = true;
				}
			}

			// Token: 0x060024B2 RID: 9394 RVA: 0x001091DF File Offset: 0x001073DF
			public void OnExit(Cult iOwner)
			{
				iOwner.mBattleState.mSpectatorShow = true;
				iOwner.mPlayState.Camera.LockInput = false;
			}

			// Token: 0x060024B3 RID: 9395 RVA: 0x001091FE File Offset: 0x001073FE
			public void Reset()
			{
				this.mTeleported = false;
			}

			// Token: 0x040027FE RID: 10238
			private float mTimer;

			// Token: 0x040027FF RID: 10239
			private bool mRelease;

			// Token: 0x04002800 RID: 10240
			private bool mPan;

			// Token: 0x04002801 RID: 10241
			private bool mBackPan;

			// Token: 0x04002802 RID: 10242
			private bool mTeleported;

			// Token: 0x04002803 RID: 10243
			private TriggerArea mSpawnArea;

			// Token: 0x04002804 RID: 10244
			private bool mChangedState;
		}

		// Token: 0x020004C9 RID: 1225
		public class ReviveState : IBossState<Cult>
		{
			// Token: 0x060024B5 RID: 9397 RVA: 0x00109210 File Offset: 0x00107410
			public void OnEnter(Cult iOwner)
			{
				iOwner.TeleportWarlock(iOwner.mLeaderWarlock, iOwner.mLeaderReviveMoveEvent[0].MoveEvent.Waypoint, iOwner.mLeaderReviveMoveEvent[0].MoveEvent.Direction);
				iOwner.mLeaderWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, iOwner.mLeaderReviveMoveEvent);
				iOwner.mFightingGoblins.Clear();
				iOwner.mLeaderWarlock.CannotDieWithoutExplicitKill = true;
				ControlManager.Instance.LimitInput(this);
				iOwner.mPlayState.Camera.Follow(iOwner.mLeaderWarlock);
			}

			// Token: 0x060024B6 RID: 9398 RVA: 0x001092AC File Offset: 0x001074AC
			public unsafe void OnUpdate(float iDeltaTime, Cult iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client && (iOwner.mLeaderWarlock.AI.Events == null || iOwner.mLeaderWarlock.AI.CurrentEvent >= iOwner.mLeaderWarlock.AI.Events.Length))
				{
					if (DialogManager.Instance.IsDialogDone(Cult.DIALOG_REVIVE, -1))
					{
						for (int i = 0; i < 5; i++)
						{
							int num = Cult.sRandom.Next(iOwner.mWarlocks.Count);
							while (!iOwner.mWarlocks[num].Dead)
							{
								num = Cult.sRandom.Next(iOwner.mWarlocks.Count);
							}
							Vector3 position = iOwner.mWarlocks[num].Position;
							Vector3 position2 = iOwner.mLeaderWarlock.Position;
							Vector3 desiredDirection;
							Vector3.Subtract(ref position2, ref position, out desiredDirection);
							iOwner.mReviveeTemplateType[i] = num % 4;
							iOwner.mWarlocks[num].Initialize(iOwner.mTemplates[num % 4], position, 0, float.MaxValue);
							iOwner.mWarlocks[num].AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
							iOwner.mWarlocks[num].CharacterBody.DesiredDirection = desiredDirection;
							switch (num % 4)
							{
							case 0:
								iOwner.mWarlocks[num].Color = Spell.COLDCOLOR;
								break;
							case 1:
								iOwner.mWarlocks[num].Color = Spell.FIRECOLOR;
								break;
							case 2:
								iOwner.mWarlocks[num].Color = Spell.LIGHTNINGCOLOR;
								break;
							case 3:
								iOwner.mWarlocks[num].Color = Spell.LIFECOLOR;
								break;
							}
							desiredDirection.Normalize();
							VisualEffectReference visualEffectReference;
							EffectManager.Instance.StartEffect(Revive.EFFECT, ref position, ref desiredDirection, out visualEffectReference);
							AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH, iOwner.mWarlocks[num].AudioEmitter);
							if (!iOwner.mPlayState.EntityManager.Entities.Contains(iOwner.mWarlocks[num]))
							{
								iOwner.mPlayState.EntityManager.AddEntity(iOwner.mWarlocks[num]);
							}
							iOwner.mFightingGoblins.Add(iOwner.mWarlocks[num]);
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Cult.ReviveMessage reviveMessage;
								reviveMessage.Index = num;
								reviveMessage.Position = iOwner.mWarlocks[num].Position;
								reviveMessage.Direction = iOwner.mWarlocks[num].Direction;
								BossFight.Instance.SendMessage<Cult.ReviveMessage>(iOwner, 3, (void*)(&reviveMessage), true);
							}
						}
						ControlManager.Instance.UnlimitInput(this);
						iOwner.mPlayState.Camera.Release(1f);
						iOwner.ChangeState(Cult.States.Final);
						return;
					}
					if (!DialogManager.Instance.DialogActive(Cult.DIALOG_REVIVE))
					{
						iOwner.mLeaderWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
						DialogManager.Instance.StartDialog(Cult.DIALOG_REVIVE, iOwner.mLeaderWarlock, null);
					}
				}
			}

			// Token: 0x060024B7 RID: 9399 RVA: 0x001095C2 File Offset: 0x001077C2
			public void OnExit(Cult iOwner)
			{
				ControlManager.Instance.UnlimitInput(this);
				iOwner.mPlayState.Camera.Release(1f);
			}
		}

		// Token: 0x020004CA RID: 1226
		public class FinalState : IBossState<Cult>
		{
			// Token: 0x060024B9 RID: 9401 RVA: 0x001095EC File Offset: 0x001077EC
			public void OnEnter(Cult iOwner)
			{
				iOwner.mTotalMaxHitPoints = 0f;
				iOwner.mLeaderWarlock.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				iOwner.mTotalMaxHitPoints += iOwner.mLeaderWarlock.MaxHitPoints;
				for (int i = 0; i < iOwner.mFightingGoblins.Count; i++)
				{
					iOwner.mTotalMaxHitPoints += iOwner.mFightingGoblins[i].MaxHitPoints;
					iOwner.mFightingGoblins[i].AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				}
				this.mReviveIndex = -1;
			}

			// Token: 0x060024BA RID: 9402 RVA: 0x0010968C File Offset: 0x0010788C
			public void OnUpdate(float iDeltaTime, Cult iOwner)
			{
				this.mReviveCoolDown -= iDeltaTime;
				if (this.mReviveCoolDown < 0f)
				{
					if (this.mReviveIndex < 0)
					{
						int num = Cult.sRandom.Next(iOwner.mFightingGoblins.Count);
						for (int i = num; i < num + iOwner.mFightingGoblins.Count; i++)
						{
							if (iOwner.mFightingGoblins != null && iOwner.mFightingGoblins[i % iOwner.mFightingGoblins.Count].Dead)
							{
								this.mReviveIndex = i % iOwner.mFightingGoblins.Count;
								return;
							}
						}
						return;
					}
					if (iOwner.mLeaderWarlock.AI.Events == null)
					{
						while (iOwner.mLeaderWarlock.AI.CurrentState != AIStateIdle.Instance)
						{
							if (iOwner.mLeaderWarlock.AI.CurrentState is AIStateAttack)
							{
								iOwner.mLeaderWarlock.AI.ReleaseTarget();
							}
							iOwner.mLeaderWarlock.AI.PopState();
						}
						iOwner.mLeaderWarlock.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, iOwner.mLeaderReviveAnimation);
						return;
					}
					if (iOwner.mLeaderWarlock.AI.CurrentEvent >= iOwner.mLeaderWarlock.AI.Events.Length)
					{
						Vector3 iPosition = iOwner.mWarlockPosition[this.mReviveIndex][0];
						Vector3 desiredDirection = iOwner.mWarlockDirection[this.mReviveIndex][0];
						iOwner.mFightingGoblins[this.mReviveIndex].Initialize(iOwner.mTemplates[iOwner.mReviveeTemplateType[this.mReviveIndex]], iPosition, 0, 2f);
						iOwner.mFightingGoblins[this.mReviveIndex].AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
						iOwner.mFightingGoblins[this.mReviveIndex].CharacterBody.DesiredDirection = desiredDirection;
						switch (this.mReviveIndex % 4)
						{
						case 0:
							iOwner.mFightingGoblins[this.mReviveIndex].Color = Spell.COLDCOLOR;
							break;
						case 1:
							iOwner.mFightingGoblins[this.mReviveIndex].Color = Spell.FIRECOLOR;
							break;
						case 2:
							iOwner.mFightingGoblins[this.mReviveIndex].Color = Spell.LIGHTNINGCOLOR;
							break;
						case 3:
							iOwner.mFightingGoblins[this.mReviveIndex].Color = Spell.LIFECOLOR;
							break;
						}
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(Revive.EFFECT, ref iPosition, ref desiredDirection, out visualEffectReference);
						AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUND_REVIVE_HASH, iOwner.mFightingGoblins[this.mReviveIndex].AudioEmitter);
						if (!iOwner.mPlayState.EntityManager.Entities.Contains(iOwner.mFightingGoblins[this.mReviveIndex]))
						{
							iOwner.mPlayState.EntityManager.AddEntity(iOwner.mFightingGoblins[this.mReviveIndex]);
						}
						this.mReviveIndex = -1;
						this.mReviveCoolDown = 5f;
						iOwner.mLeaderWarlock.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
					}
				}
			}

			// Token: 0x060024BB RID: 9403 RVA: 0x001099B6 File Offset: 0x00107BB6
			public void OnExit(Cult iOwner)
			{
			}

			// Token: 0x04002805 RID: 10245
			private const float REVIVE_COOLDOWN = 5f;

			// Token: 0x04002806 RID: 10246
			private float mReviveCoolDown;

			// Token: 0x04002807 RID: 10247
			private int mReviveIndex;
		}

		// Token: 0x020004CB RID: 1227
		public class DeadState : IBossState<Cult>
		{
			// Token: 0x060024BD RID: 9405 RVA: 0x001099C0 File Offset: 0x00107BC0
			public void OnEnter(Cult iOwner)
			{
				DialogManager.Instance.StartDialog(Cult.DIALOG_DEATH, iOwner.mLeaderWarlock, null);
				this.mTriggerExecuted = false;
			}

			// Token: 0x060024BE RID: 9406 RVA: 0x001099E0 File Offset: 0x00107BE0
			public void OnUpdate(float iDeltaTime, Cult iOwner)
			{
				if (!this.mTriggerExecuted && DialogManager.Instance.IsDialogDone(Cult.DIALOG_DEATH, -1))
				{
					iOwner.mLeaderWarlock.CannotDieWithoutExplicitKill = false;
					iOwner.mLeaderWarlock.Kill();
					this.mTriggerExecuted = true;
					iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Cult.DEFEATED_TRIGGER, null, false);
				}
			}

			// Token: 0x060024BF RID: 9407 RVA: 0x00109A41 File Offset: 0x00107C41
			public void OnExit(Cult iOwner)
			{
			}

			// Token: 0x04002808 RID: 10248
			private bool mTriggerExecuted;
		}

		// Token: 0x020004CC RID: 1228
		public enum MessageType : ushort
		{
			// Token: 0x0400280A RID: 10250
			Update,
			// Token: 0x0400280B RID: 10251
			Teleport,
			// Token: 0x0400280C RID: 10252
			ChangeState,
			// Token: 0x0400280D RID: 10253
			Spawn,
			// Token: 0x0400280E RID: 10254
			Initialize
		}

		// Token: 0x020004CD RID: 1229
		internal struct UpdateMessage
		{
			// Token: 0x0400280F RID: 10255
			public const ushort TYPE = 0;
		}

		// Token: 0x020004CE RID: 1230
		internal struct ReviveMessage
		{
			// Token: 0x04002810 RID: 10256
			public const ushort TYPE = 3;

			// Token: 0x04002811 RID: 10257
			public int Index;

			// Token: 0x04002812 RID: 10258
			public Vector3 Position;

			// Token: 0x04002813 RID: 10259
			public Vector3 Direction;
		}

		// Token: 0x020004CF RID: 1231
		internal struct TeleportMessage
		{
			// Token: 0x04002814 RID: 10260
			public const ushort TYPE = 1;

			// Token: 0x04002815 RID: 10261
			public ushort Handle;

			// Token: 0x04002816 RID: 10262
			public Vector3 Position;

			// Token: 0x04002817 RID: 10263
			public Vector3 Direction;
		}

		// Token: 0x020004D0 RID: 1232
		internal struct ChangeStateMessage
		{
			// Token: 0x04002818 RID: 10264
			public const ushort TYPE = 2;

			// Token: 0x04002819 RID: 10265
			public Cult.States State;
		}

		// Token: 0x020004D1 RID: 1233
		internal struct InitializeMessage
		{
			// Token: 0x0400281A RID: 10266
			public const ushort TYPE = 4;

			// Token: 0x0400281B RID: 10267
			[FixedBuffer(typeof(ushort), 26)]
			public Cult.InitializeMessage.<Handles>e__FixedBuffer9 Handles;

			// Token: 0x0400281C RID: 10268
			public ushort LeaderHandle;

			// Token: 0x020004D2 RID: 1234
			[UnsafeValueType]
			[CompilerGenerated]
			[StructLayout(LayoutKind.Sequential, Size = 52)]
			public struct <Handles>e__FixedBuffer9
			{
				// Token: 0x0400281D RID: 10269
				public ushort FixedElementField;
			}
		}

		// Token: 0x020004D3 RID: 1235
		public enum States
		{
			// Token: 0x0400281F RID: 10271
			Intro,
			// Token: 0x04002820 RID: 10272
			Battle,
			// Token: 0x04002821 RID: 10273
			Revive,
			// Token: 0x04002822 RID: 10274
			Spectator,
			// Token: 0x04002823 RID: 10275
			Final,
			// Token: 0x04002824 RID: 10276
			Dead
		}
	}
}
