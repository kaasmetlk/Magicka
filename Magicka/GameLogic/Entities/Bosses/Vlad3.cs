using System;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020003C7 RID: 967
	public class Vlad3 : IBoss
	{
		// Token: 0x06001D9B RID: 7579 RVA: 0x000D1903 File Offset: 0x000CFB03
		public Vlad3(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mRandom = new Random();
			iPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Vlad_Spirit");
			this.mVlad = new VladCharacter(iPlayState);
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x000D193A File Offset: 0x000CFB3A
		public void Initialize(ref Matrix iOrientation)
		{
			this.Initialize(ref iOrientation, 0);
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x000D1944 File Offset: 0x000CFB44
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.mDead = false;
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_vlad_spirit".GetHashCodeCustom());
			Vector3 vector = iOrientation.Translation;
			iOrientation.Translation = default(Vector3);
			Segment seg = default(Segment);
			seg.Origin = vector + Vector3.Up;
			seg.Delta.Y = seg.Delta.Y - 8f;
			float num;
			Vector3 vector2;
			Vector3 vector3;
			if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out num, out vector2, out vector3, seg))
			{
				vector = vector2;
			}
			this.mVlad.Initialize(cachedTemplate, vector, iUniqueID);
			this.mVlad.CannotDieWithoutExplicitKill = true;
			vector.Y += this.mVlad.Capsule.Radius + this.mVlad.Capsule.Length * 0.5f + 0.1f;
			this.mVlad.CharacterBody.MoveTo(vector, iOrientation);
			this.mVlad.CharacterBody.DesiredDirection = iOrientation.Forward;
			this.mPlayState.EntityManager.AddEntity(this.mVlad);
			this.mCurrentState = Vlad3.IntroState.Instance;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x000D1A86 File Offset: 0x000CFC86
		protected void ChangeState(IBossState<Vlad3> iState)
		{
			this.mCurrentState.OnExit(this);
			this.mCurrentState = iState;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x000D1AA7 File Offset: 0x000CFCA7
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (this.mCurrentState is Vlad3.IntroState && iFightStarted)
			{
				this.ChangeState(Vlad3.BattleState.Instance);
			}
			this.mCurrentState.OnUpdate(iDeltaTime, this);
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x000D1AD4 File Offset: 0x000CFCD4
		public void DeInitialize()
		{
			this.mVlad.CannotDieWithoutExplicitKill = false;
			AudioManager.Instance.PlayCue(Banks.Spells, Vlad3.DEATH_SOUND);
			Vector3 position = this.mVlad.Position;
			Vector3 right = Vector3.Right;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Vlad3.DEATH_EFFECT, ref position, ref right, out visualEffectReference);
			this.mVlad.Kill();
			this.mVlad.Terminate(true, false);
		}

		// Token: 0x06001DA1 RID: 7585 RVA: 0x000D1B40 File Offset: 0x000CFD40
		public void ScriptMessage(BossMessages iMessage)
		{
			if (iMessage != BossMessages.VladMortal)
			{
				return;
			}
			if (this.mVlad != null)
			{
				float hitPoints = this.mVlad.HitPoints;
			}
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x000D1B6E File Offset: 0x000CFD6E
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x000D1B75 File Offset: 0x000CFD75
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001DA4 RID: 7588 RVA: 0x000D1B7C File Offset: 0x000CFD7C
		public void SetSlow(int iIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x000D1B83 File Offset: 0x000CFD83
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x000D1B8A File Offset: 0x000CFD8A
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x000D1B91 File Offset: 0x000CFD91
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x000D1B98 File Offset: 0x000CFD98
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			throw new NotImplementedException();
		}

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06001DA9 RID: 7593 RVA: 0x000D1B9F File Offset: 0x000CFD9F
		public bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x06001DAA RID: 7594 RVA: 0x000D1BA7 File Offset: 0x000CFDA7
		public float MaxHitPoints
		{
			get
			{
				return this.mVlad.MaxHitPoints;
			}
		}

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x06001DAB RID: 7595 RVA: 0x000D1BB4 File Offset: 0x000CFDB4
		public float HitPoints
		{
			get
			{
				return this.mVlad.HitPoints;
			}
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x000D1BC1 File Offset: 0x000CFDC1
		public StatusEffect[] GetStatusEffects()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x000D1BC8 File Offset: 0x000CFDC8
		public void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x000D1BCA File Offset: 0x000CFDCA
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x000D1BD1 File Offset: 0x000CFDD1
		public BossEnum GetBossType()
		{
			return BossEnum.Vlad3;
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06001DB0 RID: 7600 RVA: 0x000D1BD5 File Offset: 0x000CFDD5
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001DB1 RID: 7601 RVA: 0x000D1BD8 File Offset: 0x000CFDD8
		public float ResistanceAgainst(Elements iElement)
		{
			if (this.mVlad != null)
			{
				return this.mVlad.ResistanceAgainst(iElement);
			}
			return 1f;
		}

		// Token: 0x04002040 RID: 8256
		private Random mRandom;

		// Token: 0x04002041 RID: 8257
		private static readonly int DEATH_SOUND = "spell_arcane_area".GetHashCodeCustom();

		// Token: 0x04002042 RID: 8258
		private static readonly int DEATH_EFFECT = "special_fairy_crash".GetHashCodeCustom();

		// Token: 0x04002043 RID: 8259
		private bool mDead;

		// Token: 0x04002044 RID: 8260
		private VladCharacter mVlad;

		// Token: 0x04002045 RID: 8261
		private PlayState mPlayState;

		// Token: 0x04002046 RID: 8262
		private IBossState<Vlad3> mCurrentState;

		// Token: 0x020003C8 RID: 968
		protected class IntroState : IBossState<Vlad3>
		{
			// Token: 0x17000752 RID: 1874
			// (get) Token: 0x06001DB3 RID: 7603 RVA: 0x000D1C14 File Offset: 0x000CFE14
			public static Vlad3.IntroState Instance
			{
				get
				{
					if (Vlad3.IntroState.mSingelton == null)
					{
						lock (Vlad3.IntroState.mSingeltonLock)
						{
							if (Vlad3.IntroState.mSingelton == null)
							{
								Vlad3.IntroState.mSingelton = new Vlad3.IntroState();
							}
						}
					}
					return Vlad3.IntroState.mSingelton;
				}
			}

			// Token: 0x06001DB4 RID: 7604 RVA: 0x000D1C68 File Offset: 0x000CFE68
			private IntroState()
			{
			}

			// Token: 0x06001DB5 RID: 7605 RVA: 0x000D1C70 File Offset: 0x000CFE70
			public void OnEnter(Vlad3 iOwner)
			{
				iOwner.mVlad.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
				iOwner.mVlad.Equipment[0].Item.Visible = true;
				iOwner.mVlad.Equipment[1].Item.Visible = false;
			}

			// Token: 0x06001DB6 RID: 7606 RVA: 0x000D1CC4 File Offset: 0x000CFEC4
			public void OnUpdate(float iDeltaTime, Vlad3 iOwner)
			{
			}

			// Token: 0x06001DB7 RID: 7607 RVA: 0x000D1CC6 File Offset: 0x000CFEC6
			public void OnExit(Vlad3 iOwner)
			{
			}

			// Token: 0x04002047 RID: 8263
			private static Vlad3.IntroState mSingelton;

			// Token: 0x04002048 RID: 8264
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020003C9 RID: 969
		protected class BattleState : IBossState<Vlad3>
		{
			// Token: 0x17000753 RID: 1875
			// (get) Token: 0x06001DB9 RID: 7609 RVA: 0x000D1CD8 File Offset: 0x000CFED8
			public static Vlad3.BattleState Instance
			{
				get
				{
					if (Vlad3.BattleState.mSingelton == null)
					{
						lock (Vlad3.BattleState.mSingeltonLock)
						{
							if (Vlad3.BattleState.mSingelton == null)
							{
								Vlad3.BattleState.mSingelton = new Vlad3.BattleState();
							}
						}
					}
					return Vlad3.BattleState.mSingelton;
				}
			}

			// Token: 0x06001DBA RID: 7610 RVA: 0x000D1D2C File Offset: 0x000CFF2C
			private BattleState()
			{
			}

			// Token: 0x06001DBB RID: 7611 RVA: 0x000D1D34 File Offset: 0x000CFF34
			public void OnEnter(Vlad3 iOwner)
			{
				iOwner.mVlad.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				this.mIdleTimer = 5f + (float)iOwner.mRandom.NextDouble() * 5f;
			}

			// Token: 0x06001DBC RID: 7612 RVA: 0x000D1D6C File Offset: 0x000CFF6C
			public void OnUpdate(float iDeltaTime, Vlad3 iOwner)
			{
				this.mIdleTimer -= iDeltaTime;
				if (iOwner.mVlad.HitPoints <= 0f)
				{
					iOwner.DeInitialize();
					iOwner.ChangeState(Vlad3.OutroState.Instance);
				}
				if (iOwner.mVlad.Position.Y < -5f)
				{
					iOwner.mVlad.Magick = Teleport.Instance;
					iOwner.mVlad.Magick.Execute(iOwner.mVlad, iOwner.mPlayState);
				}
				if (iOwner.mVlad.HasStatus(StatusEffects.Frozen))
				{
					iOwner.mVlad.StopStatusEffects(StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold);
				}
			}

			// Token: 0x06001DBD RID: 7613 RVA: 0x000D1E09 File Offset: 0x000D0009
			public void OnExit(Vlad3 iOwner)
			{
			}

			// Token: 0x04002049 RID: 8265
			private const float TAUNT_TIME = 5f;

			// Token: 0x0400204A RID: 8266
			private static Vlad3.BattleState mSingelton;

			// Token: 0x0400204B RID: 8267
			private static volatile object mSingeltonLock = new object();

			// Token: 0x0400204C RID: 8268
			private float mIdleTimer;
		}

		// Token: 0x020003CA RID: 970
		protected class OutroState : IBossState<Vlad3>
		{
			// Token: 0x17000754 RID: 1876
			// (get) Token: 0x06001DBF RID: 7615 RVA: 0x000D1E1C File Offset: 0x000D001C
			public static Vlad3.OutroState Instance
			{
				get
				{
					if (Vlad3.OutroState.mSingelton == null)
					{
						lock (Vlad3.OutroState.mSingeltonLock)
						{
							if (Vlad3.OutroState.mSingelton == null)
							{
								Vlad3.OutroState.mSingelton = new Vlad3.OutroState();
							}
						}
					}
					return Vlad3.OutroState.mSingelton;
				}
			}

			// Token: 0x06001DC0 RID: 7616 RVA: 0x000D1E70 File Offset: 0x000D0070
			private OutroState()
			{
			}

			// Token: 0x06001DC1 RID: 7617 RVA: 0x000D1E78 File Offset: 0x000D0078
			public void OnEnter(Vlad3 iOwner)
			{
			}

			// Token: 0x06001DC2 RID: 7618 RVA: 0x000D1E7A File Offset: 0x000D007A
			public void OnUpdate(float iDeltaTime, Vlad3 iOwner)
			{
			}

			// Token: 0x06001DC3 RID: 7619 RVA: 0x000D1E7C File Offset: 0x000D007C
			public void OnExit(Vlad3 iOwner)
			{
			}

			// Token: 0x0400204D RID: 8269
			private static Vlad3.OutroState mSingelton;

			// Token: 0x0400204E RID: 8270
			private static volatile object mSingeltonLock = new object();
		}
	}
}
