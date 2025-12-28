using System;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020003FF RID: 1023
	public class Vlad2 : IBoss
	{
		// Token: 0x06001F80 RID: 8064 RVA: 0x000DDD9E File Offset: 0x000DBF9E
		public Vlad2(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mRandom = new Random();
			iPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Vlad_Diplomat");
			this.mVlad = new VladCharacter(iPlayState);
		}

		// Token: 0x06001F81 RID: 8065 RVA: 0x000DDDD5 File Offset: 0x000DBFD5
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06001F82 RID: 8066 RVA: 0x000DDDE0 File Offset: 0x000DBFE0
		public void Initialize(ref Matrix iOrientation)
		{
			this.mDead = false;
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_vlad_diplomat".GetHashCodeCustom());
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
			this.mVlad.Initialize(cachedTemplate, vector, 0);
			this.mVlad.CannotDieWithoutExplicitKill = true;
			vector.Y += this.mVlad.Capsule.Radius + this.mVlad.Capsule.Length * 0.5f + 0.1f;
			this.mVlad.CharacterBody.MoveTo(vector, iOrientation);
			this.mVlad.CharacterBody.DesiredDirection = iOrientation.Forward;
			this.mPlayState.EntityManager.AddEntity(this.mVlad);
			this.mCurrentState = Vlad2.IntroState.Instance;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x06001F83 RID: 8067 RVA: 0x000DDF22 File Offset: 0x000DC122
		protected void ChangeState(IBossState<Vlad2> iState)
		{
			this.mCurrentState.OnExit(this);
			this.mCurrentState = iState;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x06001F84 RID: 8068 RVA: 0x000DDF43 File Offset: 0x000DC143
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (this.mCurrentState is Vlad2.IntroState && iFightStarted)
			{
				this.ChangeState(Vlad2.BattleState.Instance);
			}
			this.mCurrentState.OnUpdate(iDeltaTime, this);
		}

		// Token: 0x06001F85 RID: 8069 RVA: 0x000DDF6D File Offset: 0x000DC16D
		public void DeInitialize()
		{
			this.mVlad.CannotDieWithoutExplicitKill = false;
			this.mVlad.Kill();
			this.mVlad.Terminate(true, false);
		}

		// Token: 0x06001F86 RID: 8070 RVA: 0x000DDF94 File Offset: 0x000DC194
		public void ScriptMessage(BossMessages iMessage)
		{
			if (iMessage != BossMessages.VladMortal)
			{
				return;
			}
			if (this.mVlad != null)
			{
				this.mVlad.CannotDieWithoutExplicitKill = false;
				this.mVlad.Kill();
				this.mVlad.Terminate(true, false);
			}
		}

		// Token: 0x06001F87 RID: 8071 RVA: 0x000DDFD4 File Offset: 0x000DC1D4
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001F88 RID: 8072 RVA: 0x000DDFDB File Offset: 0x000DC1DB
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001F89 RID: 8073 RVA: 0x000DDFE2 File Offset: 0x000DC1E2
		public void SetSlow(int iIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001F8A RID: 8074 RVA: 0x000DDFE9 File Offset: 0x000DC1E9
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001F8B RID: 8075 RVA: 0x000DDFF0 File Offset: 0x000DC1F0
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x000DDFF7 File Offset: 0x000DC1F7
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x000DDFFE File Offset: 0x000DC1FE
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			throw new NotImplementedException();
		}

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x06001F8E RID: 8078 RVA: 0x000DE005 File Offset: 0x000DC205
		public bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x06001F8F RID: 8079 RVA: 0x000DE00D File Offset: 0x000DC20D
		public float MaxHitPoints
		{
			get
			{
				return this.mVlad.MaxHitPoints;
			}
		}

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x06001F90 RID: 8080 RVA: 0x000DE01A File Offset: 0x000DC21A
		public float HitPoints
		{
			get
			{
				return this.mVlad.HitPoints;
			}
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x000DE027 File Offset: 0x000DC227
		public StatusEffect[] GetStatusEffects()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x000DE02E File Offset: 0x000DC22E
		public void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x000DE030 File Offset: 0x000DC230
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x000DE037 File Offset: 0x000DC237
		public BossEnum GetBossType()
		{
			return BossEnum.Vlad2;
		}

		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x06001F95 RID: 8085 RVA: 0x000DE03B File Offset: 0x000DC23B
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001F96 RID: 8086 RVA: 0x000DE03E File Offset: 0x000DC23E
		public float ResistanceAgainst(Elements iElement)
		{
			if (this.mVlad != null)
			{
				return this.mVlad.ResistanceAgainst(iElement);
			}
			return 1f;
		}

		// Token: 0x040021EB RID: 8683
		private Random mRandom;

		// Token: 0x040021EC RID: 8684
		private bool mDead;

		// Token: 0x040021ED RID: 8685
		private VladCharacter mVlad;

		// Token: 0x040021EE RID: 8686
		private PlayState mPlayState;

		// Token: 0x040021EF RID: 8687
		private IBossState<Vlad2> mCurrentState;

		// Token: 0x02000400 RID: 1024
		protected class IntroState : IBossState<Vlad2>
		{
			// Token: 0x170007BF RID: 1983
			// (get) Token: 0x06001F97 RID: 8087 RVA: 0x000DE05C File Offset: 0x000DC25C
			public static Vlad2.IntroState Instance
			{
				get
				{
					if (Vlad2.IntroState.mSingelton == null)
					{
						lock (Vlad2.IntroState.mSingeltonLock)
						{
							if (Vlad2.IntroState.mSingelton == null)
							{
								Vlad2.IntroState.mSingelton = new Vlad2.IntroState();
							}
						}
					}
					return Vlad2.IntroState.mSingelton;
				}
			}

			// Token: 0x06001F98 RID: 8088 RVA: 0x000DE0B0 File Offset: 0x000DC2B0
			private IntroState()
			{
			}

			// Token: 0x06001F99 RID: 8089 RVA: 0x000DE0B8 File Offset: 0x000DC2B8
			public void OnEnter(Vlad2 iOwner)
			{
				iOwner.mVlad.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
				iOwner.mVlad.Equipment[0].Item.Visible = true;
				iOwner.mVlad.Equipment[1].Item.Visible = false;
			}

			// Token: 0x06001F9A RID: 8090 RVA: 0x000DE10C File Offset: 0x000DC30C
			public void OnUpdate(float iDeltaTime, Vlad2 iOwner)
			{
			}

			// Token: 0x06001F9B RID: 8091 RVA: 0x000DE10E File Offset: 0x000DC30E
			public void OnExit(Vlad2 iOwner)
			{
			}

			// Token: 0x040021F0 RID: 8688
			private static Vlad2.IntroState mSingelton;

			// Token: 0x040021F1 RID: 8689
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x02000401 RID: 1025
		protected class BattleState : IBossState<Vlad2>
		{
			// Token: 0x170007C0 RID: 1984
			// (get) Token: 0x06001F9D RID: 8093 RVA: 0x000DE120 File Offset: 0x000DC320
			public static Vlad2.BattleState Instance
			{
				get
				{
					if (Vlad2.BattleState.mSingelton == null)
					{
						lock (Vlad2.BattleState.mSingeltonLock)
						{
							if (Vlad2.BattleState.mSingelton == null)
							{
								Vlad2.BattleState.mSingelton = new Vlad2.BattleState();
							}
						}
					}
					return Vlad2.BattleState.mSingelton;
				}
			}

			// Token: 0x06001F9E RID: 8094 RVA: 0x000DE174 File Offset: 0x000DC374
			private BattleState()
			{
			}

			// Token: 0x06001F9F RID: 8095 RVA: 0x000DE17C File Offset: 0x000DC37C
			public void OnEnter(Vlad2 iOwner)
			{
				iOwner.mVlad.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				this.mIdleTimer = 5f + (float)iOwner.mRandom.NextDouble() * 5f;
			}

			// Token: 0x06001FA0 RID: 8096 RVA: 0x000DE1B4 File Offset: 0x000DC3B4
			public void OnUpdate(float iDeltaTime, Vlad2 iOwner)
			{
				this.mIdleTimer -= iDeltaTime;
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

			// Token: 0x06001FA1 RID: 8097 RVA: 0x000DE22E File Offset: 0x000DC42E
			public void OnExit(Vlad2 iOwner)
			{
			}

			// Token: 0x040021F2 RID: 8690
			private const float TAUNT_TIME = 5f;

			// Token: 0x040021F3 RID: 8691
			private static Vlad2.BattleState mSingelton;

			// Token: 0x040021F4 RID: 8692
			private static volatile object mSingeltonLock = new object();

			// Token: 0x040021F5 RID: 8693
			private float mIdleTimer;
		}

		// Token: 0x02000402 RID: 1026
		protected class OutroState : IBossState<Vlad2>
		{
			// Token: 0x170007C1 RID: 1985
			// (get) Token: 0x06001FA3 RID: 8099 RVA: 0x000DE240 File Offset: 0x000DC440
			public static Vlad2.OutroState Instance
			{
				get
				{
					if (Vlad2.OutroState.mSingelton == null)
					{
						lock (Vlad2.OutroState.mSingeltonLock)
						{
							if (Vlad2.OutroState.mSingelton == null)
							{
								Vlad2.OutroState.mSingelton = new Vlad2.OutroState();
							}
						}
					}
					return Vlad2.OutroState.mSingelton;
				}
			}

			// Token: 0x06001FA4 RID: 8100 RVA: 0x000DE294 File Offset: 0x000DC494
			private OutroState()
			{
			}

			// Token: 0x06001FA5 RID: 8101 RVA: 0x000DE29C File Offset: 0x000DC49C
			public void OnEnter(Vlad2 iOwner)
			{
			}

			// Token: 0x06001FA6 RID: 8102 RVA: 0x000DE29E File Offset: 0x000DC49E
			public void OnUpdate(float iDeltaTime, Vlad2 iOwner)
			{
			}

			// Token: 0x06001FA7 RID: 8103 RVA: 0x000DE2A0 File Offset: 0x000DC4A0
			public void OnExit(Vlad2 iOwner)
			{
			}

			// Token: 0x040021F6 RID: 8694
			private static Vlad2.OutroState mSingelton;

			// Token: 0x040021F7 RID: 8695
			private static volatile object mSingeltonLock = new object();
		}
	}
}
