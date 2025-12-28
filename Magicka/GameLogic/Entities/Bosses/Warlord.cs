using System;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x0200016C RID: 364
	public class Warlord : IBoss
	{
		// Token: 0x06000B0F RID: 2831 RVA: 0x00042FEB File Offset: 0x000411EB
		public Warlord(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Warlord");
			this.mWarlord = new WarlordCharacter(iPlayState);
			this.mRandom = new Random();
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x00043027 File Offset: 0x00041227
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x00043030 File Offset: 0x00041230
		public void Initialize(ref Matrix iOrientation)
		{
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_warlord".GetHashCodeCustom());
			Vector3 vector = iOrientation.Translation;
			iOrientation.Translation = default(Vector3);
			Segment seg = default(Segment);
			seg.Origin = vector + Vector3.Up;
			seg.Delta.Y = seg.Delta.Y - 6f;
			float num;
			Vector3 vector2;
			Vector3 vector3;
			if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out num, out vector2, out vector3, seg))
			{
				vector = vector2;
			}
			vector.Y += this.mWarlord.Capsule.Radius + this.mWarlord.Capsule.Length * 0.5f + 0.1f;
			this.mWarlord.Initialize(cachedTemplate, vector, Warlord.KHAN_ID, float.MaxValue);
			this.mWarlord.CharacterBody.MoveTo(vector, iOrientation);
			this.mWarlord.CharacterBody.DesiredDirection = iOrientation.Forward;
			this.mWarlord.CannotDieWithoutExplicitKill = true;
			this.mPlayState.EntityManager.AddEntity(this.mWarlord);
			this.mCurrentState = Warlord.IntroState.Instance;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x00043174 File Offset: 0x00041374
		public void DeInitialize()
		{
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x00043178 File Offset: 0x00041378
		public unsafe void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (this.mCurrentState is Warlord.IntroState && iFightStarted && NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Warlord.ChangeStateMessage changeStateMessage;
					changeStateMessage.NewState = Warlord.State.Battle;
					BossFight.Instance.SendMessage<Warlord.ChangeStateMessage>(this, 0, (void*)(&changeStateMessage), true);
				}
				this.ChangeState(Warlord.BattleState.Instance);
			}
			if (this.mWarlord.HitPoints <= 0f && this.mCurrentState != Warlord.DeathState.Instance && NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Warlord.ChangeStateMessage changeStateMessage2;
					changeStateMessage2.NewState = Warlord.State.Death;
					BossFight.Instance.SendMessage<Warlord.ChangeStateMessage>(this, 0, (void*)(&changeStateMessage2), true);
				}
				this.ChangeState(Warlord.DeathState.Instance);
			}
			this.mCurrentState.OnUpdate(iDeltaTime, this);
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x0004323B File Offset: 0x0004143B
		public void ChangeState(IBossState<Warlord> iState)
		{
			this.mCurrentState.OnExit(this);
			this.mCurrentState = iState;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x0004325C File Offset: 0x0004145C
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			throw new NotImplementedException();
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06000B16 RID: 2838 RVA: 0x00043263 File Offset: 0x00041463
		public bool Dead
		{
			get
			{
				return this.mWarlord.Dead;
			}
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000B17 RID: 2839 RVA: 0x00043270 File Offset: 0x00041470
		public float MaxHitPoints
		{
			get
			{
				return this.mWarlord.MaxHitPoints;
			}
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000B18 RID: 2840 RVA: 0x0004327D File Offset: 0x0004147D
		public float HitPoints
		{
			get
			{
				return this.mWarlord.HitPoints;
			}
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0004328A File Offset: 0x0004148A
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x00043291 File Offset: 0x00041491
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x00043298 File Offset: 0x00041498
		public void SetSlow(int iIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x0004329F File Offset: 0x0004149F
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x000432A6 File Offset: 0x000414A6
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x000432AD File Offset: 0x000414AD
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x000432B4 File Offset: 0x000414B4
		public StatusEffect[] GetStatusEffects()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x000432BB File Offset: 0x000414BB
		public void ScriptMessage(BossMessages iMessage)
		{
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x000432C0 File Offset: 0x000414C0
		public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			if (iMsg.Type == 0)
			{
				Warlord.ChangeStateMessage changeStateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
				switch (changeStateMessage.NewState)
				{
				case Warlord.State.Intro:
					this.ChangeState(Warlord.IntroState.Instance);
					return;
				case Warlord.State.Battle:
					this.ChangeState(Warlord.BattleState.Instance);
					return;
				case Warlord.State.Death:
					this.ChangeState(Warlord.DeathState.Instance);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x0004331C File Offset: 0x0004151C
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x00043323 File Offset: 0x00041523
		public BossEnum GetBossType()
		{
			return BossEnum.Khan;
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06000B24 RID: 2852 RVA: 0x00043326 File Offset: 0x00041526
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x00043329 File Offset: 0x00041529
		public float ResistanceAgainst(Elements iElement)
		{
			if (this.mWarlord != null)
			{
				return this.mWarlord.ResistanceAgainst(iElement);
			}
			return 1f;
		}

		// Token: 0x04000A20 RID: 2592
		private const float TAUNT_COOLDOWN = 9f;

		// Token: 0x04000A21 RID: 2593
		private static readonly int DIALOG_DEATH_TALK = "warlorddeathtalk".GetHashCodeCustom();

		// Token: 0x04000A22 RID: 2594
		private static readonly int KHAN_ID = "#boss_n06".GetHashCodeCustom();

		// Token: 0x04000A23 RID: 2595
		private static readonly int[] DIALOG_TAUNTS = new int[]
		{
			"warlordtaunt1".GetHashCodeCustom(),
			"warlordtaunt2".GetHashCodeCustom(),
			"warlordtaunt3".GetHashCodeCustom()
		};

		// Token: 0x04000A24 RID: 2596
		private float mTauntTimer;

		// Token: 0x04000A25 RID: 2597
		private Random mRandom;

		// Token: 0x04000A26 RID: 2598
		private WarlordCharacter mWarlord;

		// Token: 0x04000A27 RID: 2599
		private PlayState mPlayState;

		// Token: 0x04000A28 RID: 2600
		private IBossState<Warlord> mCurrentState;

		// Token: 0x0200016D RID: 365
		private enum MessageType : ushort
		{
			// Token: 0x04000A2A RID: 2602
			ChangeState
		}

		// Token: 0x0200016E RID: 366
		internal struct ChangeStateMessage
		{
			// Token: 0x04000A2B RID: 2603
			public const ushort TYPE = 0;

			// Token: 0x04000A2C RID: 2604
			public Warlord.State NewState;
		}

		// Token: 0x0200016F RID: 367
		public enum State : byte
		{
			// Token: 0x04000A2E RID: 2606
			Intro,
			// Token: 0x04000A2F RID: 2607
			Battle,
			// Token: 0x04000A30 RID: 2608
			Death
		}

		// Token: 0x02000170 RID: 368
		public class IntroState : IBossState<Warlord>
		{
			// Token: 0x170002A1 RID: 673
			// (get) Token: 0x06000B27 RID: 2855 RVA: 0x000433A8 File Offset: 0x000415A8
			public static Warlord.IntroState Instance
			{
				get
				{
					if (Warlord.IntroState.mSingelton == null)
					{
						lock (Warlord.IntroState.mSingeltonLock)
						{
							if (Warlord.IntroState.mSingelton == null)
							{
								Warlord.IntroState.mSingelton = new Warlord.IntroState();
							}
						}
					}
					return Warlord.IntroState.mSingelton;
				}
			}

			// Token: 0x06000B28 RID: 2856 RVA: 0x000433FC File Offset: 0x000415FC
			private IntroState()
			{
			}

			// Token: 0x06000B29 RID: 2857 RVA: 0x00043404 File Offset: 0x00041604
			public void OnEnter(Warlord iOwner)
			{
			}

			// Token: 0x06000B2A RID: 2858 RVA: 0x00043406 File Offset: 0x00041606
			public void OnUpdate(float iDeltaTime, Warlord iOwner)
			{
			}

			// Token: 0x06000B2B RID: 2859 RVA: 0x00043408 File Offset: 0x00041608
			public void OnExit(Warlord iOwner)
			{
			}

			// Token: 0x04000A31 RID: 2609
			private static Warlord.IntroState mSingelton;

			// Token: 0x04000A32 RID: 2610
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000171 RID: 369
		public class BattleState : IBossState<Warlord>
		{
			// Token: 0x170002A2 RID: 674
			// (get) Token: 0x06000B2D RID: 2861 RVA: 0x00043418 File Offset: 0x00041618
			public static Warlord.BattleState Instance
			{
				get
				{
					if (Warlord.BattleState.mSingelton == null)
					{
						lock (Warlord.BattleState.mSingeltonLock)
						{
							if (Warlord.BattleState.mSingelton == null)
							{
								Warlord.BattleState.mSingelton = new Warlord.BattleState();
							}
						}
					}
					return Warlord.BattleState.mSingelton;
				}
			}

			// Token: 0x06000B2E RID: 2862 RVA: 0x0004346C File Offset: 0x0004166C
			private BattleState()
			{
			}

			// Token: 0x06000B2F RID: 2863 RVA: 0x00043474 File Offset: 0x00041674
			public void OnEnter(Warlord iOwner)
			{
				iOwner.mWarlord.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
			}

			// Token: 0x06000B30 RID: 2864 RVA: 0x00043490 File Offset: 0x00041690
			public void OnUpdate(float iDeltaTime, Warlord iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client && NetworkManager.Instance.State == NetworkState.Server)
				{
					iOwner.mTauntTimer -= iDeltaTime;
					if (iOwner.mTauntTimer <= 0f)
					{
						iOwner.mTauntTimer += 9f;
						DialogManager.Instance.StartDialog(Warlord.DIALOG_TAUNTS[iOwner.mRandom.Next(Warlord.DIALOG_TAUNTS.Length)], iOwner.mWarlord, null);
					}
				}
			}

			// Token: 0x06000B31 RID: 2865 RVA: 0x0004350D File Offset: 0x0004170D
			public void OnExit(Warlord iOwner)
			{
			}

			// Token: 0x04000A33 RID: 2611
			private static Warlord.BattleState mSingelton;

			// Token: 0x04000A34 RID: 2612
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000172 RID: 370
		public class DeathState : IBossState<Warlord>
		{
			// Token: 0x170002A3 RID: 675
			// (get) Token: 0x06000B33 RID: 2867 RVA: 0x00043520 File Offset: 0x00041720
			public static Warlord.DeathState Instance
			{
				get
				{
					if (Warlord.DeathState.mSingelton == null)
					{
						lock (Warlord.DeathState.mSingeltonLock)
						{
							if (Warlord.DeathState.mSingelton == null)
							{
								Warlord.DeathState.mSingelton = new Warlord.DeathState();
							}
						}
					}
					return Warlord.DeathState.mSingelton;
				}
			}

			// Token: 0x06000B34 RID: 2868 RVA: 0x00043574 File Offset: 0x00041774
			private DeathState()
			{
			}

			// Token: 0x06000B35 RID: 2869 RVA: 0x0004357C File Offset: 0x0004177C
			public void OnEnter(Warlord iOwner)
			{
				iOwner.mWarlord.SpecialIdleAnimation = Animations.idlelong_agg2;
			}

			// Token: 0x06000B36 RID: 2870 RVA: 0x0004358A File Offset: 0x0004178A
			public void OnUpdate(float iDeltaTime, Warlord iOwner)
			{
			}

			// Token: 0x06000B37 RID: 2871 RVA: 0x0004358C File Offset: 0x0004178C
			public void OnExit(Warlord iOwner)
			{
			}

			// Token: 0x04000A35 RID: 2613
			private static Warlord.DeathState mSingelton;

			// Token: 0x04000A36 RID: 2614
			private static volatile object mSingeltonLock = new object();
		}
	}
}
