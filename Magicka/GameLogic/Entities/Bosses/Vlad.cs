using System;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020002A1 RID: 673
	public class Vlad : IBoss
	{
		// Token: 0x06001433 RID: 5171 RVA: 0x0007E6BA File Offset: 0x0007C8BA
		public Vlad(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mRandom = new Random();
			iPlayState.Content.Load<CharacterTemplate>("Data/Characters/Boss_Vlad_Swamp");
			this.mVlad = new VladCharacter(iPlayState);
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x0007E6F1 File Offset: 0x0007C8F1
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x0007E6FC File Offset: 0x0007C8FC
		public void Initialize(ref Matrix iOrientation)
		{
			this.mDead = false;
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate("boss_vlad_swamp".GetHashCodeCustom());
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
			this.mCurrentState = Vlad.IntroState.Instance;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x0007E83E File Offset: 0x0007CA3E
		protected void ChangeState(IBossState<Vlad> iState)
		{
			this.mCurrentState.OnExit(this);
			this.mCurrentState = iState;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x0007E85F File Offset: 0x0007CA5F
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (this.mCurrentState is Vlad.IntroState && iFightStarted)
			{
				this.ChangeState(Vlad.BattleState.Instance);
			}
			this.mCurrentState.OnUpdate(iDeltaTime, this);
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x0007E889 File Offset: 0x0007CA89
		public void DeInitialize()
		{
			this.mVlad.CannotDieWithoutExplicitKill = false;
			this.mVlad.Kill();
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x0007E8A4 File Offset: 0x0007CAA4
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

		// Token: 0x0600143A RID: 5178 RVA: 0x0007E8E4 File Offset: 0x0007CAE4
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x0007E8EB File Offset: 0x0007CAEB
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x0007E8F2 File Offset: 0x0007CAF2
		public void SetSlow(int iIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x0007E8F9 File Offset: 0x0007CAF9
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x0007E900 File Offset: 0x0007CB00
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x0007E907 File Offset: 0x0007CB07
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x0007E90E File Offset: 0x0007CB0E
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			throw new NotImplementedException();
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06001441 RID: 5185 RVA: 0x0007E915 File Offset: 0x0007CB15
		public bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06001442 RID: 5186 RVA: 0x0007E91D File Offset: 0x0007CB1D
		public float MaxHitPoints
		{
			get
			{
				return this.mVlad.MaxHitPoints;
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06001443 RID: 5187 RVA: 0x0007E92A File Offset: 0x0007CB2A
		public float HitPoints
		{
			get
			{
				return this.mVlad.HitPoints;
			}
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x0007E937 File Offset: 0x0007CB37
		public StatusEffect[] GetStatusEffects()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x0007E93E File Offset: 0x0007CB3E
		public void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x0007E940 File Offset: 0x0007CB40
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x0007E947 File Offset: 0x0007CB47
		public BossEnum GetBossType()
		{
			return BossEnum.Vlad;
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06001448 RID: 5192 RVA: 0x0007E94A File Offset: 0x0007CB4A
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x0007E94D File Offset: 0x0007CB4D
		public float ResistanceAgainst(Elements iElement)
		{
			if (this.mVlad != null)
			{
				return this.mVlad.ResistanceAgainst(iElement);
			}
			return 1f;
		}

		// Token: 0x040015AA RID: 5546
		private static readonly int TRIGGER_INTRO = "trigger_enter".GetHashCodeCustom();

		// Token: 0x040015AB RID: 5547
		private static readonly int DIALOG_OUTRO1 = "vladoutro1".GetHashCodeCustom();

		// Token: 0x040015AC RID: 5548
		private static readonly int DIALOG_OUTRO2 = "vladoutro2".GetHashCodeCustom();

		// Token: 0x040015AD RID: 5549
		private static readonly int[] DIALOG_BATTLE = new int[]
		{
			"vladbattle1".GetHashCodeCustom(),
			"vladbattle2".GetHashCodeCustom(),
			"vladbattle3".GetHashCodeCustom(),
			"vladbattle4".GetHashCodeCustom()
		};

		// Token: 0x040015AE RID: 5550
		private Random mRandom;

		// Token: 0x040015AF RID: 5551
		private bool mDead;

		// Token: 0x040015B0 RID: 5552
		private VladCharacter mVlad;

		// Token: 0x040015B1 RID: 5553
		private PlayState mPlayState;

		// Token: 0x040015B2 RID: 5554
		private IBossState<Vlad> mCurrentState;

		// Token: 0x020002A2 RID: 674
		protected class IntroState : IBossState<Vlad>
		{
			// Token: 0x17000530 RID: 1328
			// (get) Token: 0x0600144B RID: 5195 RVA: 0x0007E9E8 File Offset: 0x0007CBE8
			public static Vlad.IntroState Instance
			{
				get
				{
					if (Vlad.IntroState.mSingelton == null)
					{
						lock (Vlad.IntroState.mSingeltonLock)
						{
							if (Vlad.IntroState.mSingelton == null)
							{
								Vlad.IntroState.mSingelton = new Vlad.IntroState();
							}
						}
					}
					return Vlad.IntroState.mSingelton;
				}
			}

			// Token: 0x0600144C RID: 5196 RVA: 0x0007EA3C File Offset: 0x0007CC3C
			private IntroState()
			{
			}

			// Token: 0x0600144D RID: 5197 RVA: 0x0007EA44 File Offset: 0x0007CC44
			public void OnEnter(Vlad iOwner)
			{
				iOwner.mVlad.AI.SetOrder(Order.Idle, ReactTo.None, Order.Idle, 0, 0, 0, null);
				iOwner.mVlad.Equipment[0].Item.Visible = false;
				iOwner.mVlad.Equipment[1].Item.Visible = false;
			}

			// Token: 0x0600144E RID: 5198 RVA: 0x0007EA98 File Offset: 0x0007CC98
			public void OnUpdate(float iDeltaTime, Vlad iOwner)
			{
			}

			// Token: 0x0600144F RID: 5199 RVA: 0x0007EA9A File Offset: 0x0007CC9A
			public void OnExit(Vlad iOwner)
			{
			}

			// Token: 0x040015B3 RID: 5555
			private static Vlad.IntroState mSingelton;

			// Token: 0x040015B4 RID: 5556
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002A3 RID: 675
		protected class BattleState : IBossState<Vlad>
		{
			// Token: 0x17000531 RID: 1329
			// (get) Token: 0x06001451 RID: 5201 RVA: 0x0007EAAC File Offset: 0x0007CCAC
			public static Vlad.BattleState Instance
			{
				get
				{
					if (Vlad.BattleState.mSingelton == null)
					{
						lock (Vlad.BattleState.mSingeltonLock)
						{
							if (Vlad.BattleState.mSingelton == null)
							{
								Vlad.BattleState.mSingelton = new Vlad.BattleState();
							}
						}
					}
					return Vlad.BattleState.mSingelton;
				}
			}

			// Token: 0x06001452 RID: 5202 RVA: 0x0007EB00 File Offset: 0x0007CD00
			private BattleState()
			{
			}

			// Token: 0x06001453 RID: 5203 RVA: 0x0007EB08 File Offset: 0x0007CD08
			public void OnEnter(Vlad iOwner)
			{
				iOwner.mVlad.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				this.mIdleTimer = 5f + (float)iOwner.mRandom.NextDouble() * 5f;
			}

			// Token: 0x06001454 RID: 5204 RVA: 0x0007EB40 File Offset: 0x0007CD40
			public void OnUpdate(float iDeltaTime, Vlad iOwner)
			{
				this.mIdleTimer -= iDeltaTime;
				if (iOwner.mVlad.HitPoints <= 0f)
				{
					iOwner.ChangeState(Vlad.OutroState.Instance);
					return;
				}
				if (this.mIdleTimer <= 0f)
				{
					DialogManager.Instance.StartDialog(Vlad.DIALOG_BATTLE[iOwner.mRandom.Next(Vlad.DIALOG_BATTLE.Length)], iOwner.mVlad, null);
					this.mIdleTimer = 5f + (float)iOwner.mRandom.NextDouble() * 5f;
				}
			}

			// Token: 0x06001455 RID: 5205 RVA: 0x0007EBCD File Offset: 0x0007CDCD
			public void OnExit(Vlad iOwner)
			{
			}

			// Token: 0x040015B5 RID: 5557
			private const float TAUNT_TIME = 5f;

			// Token: 0x040015B6 RID: 5558
			private static Vlad.BattleState mSingelton;

			// Token: 0x040015B7 RID: 5559
			private static volatile object mSingeltonLock = new object();

			// Token: 0x040015B8 RID: 5560
			private float mIdleTimer;
		}

		// Token: 0x020002A4 RID: 676
		protected class OutroState : IBossState<Vlad>
		{
			// Token: 0x17000532 RID: 1330
			// (get) Token: 0x06001457 RID: 5207 RVA: 0x0007EBE0 File Offset: 0x0007CDE0
			public static Vlad.OutroState Instance
			{
				get
				{
					if (Vlad.OutroState.mSingelton == null)
					{
						lock (Vlad.OutroState.mSingeltonLock)
						{
							if (Vlad.OutroState.mSingelton == null)
							{
								Vlad.OutroState.mSingelton = new Vlad.OutroState();
							}
						}
					}
					return Vlad.OutroState.mSingelton;
				}
			}

			// Token: 0x06001458 RID: 5208 RVA: 0x0007EC34 File Offset: 0x0007CE34
			private OutroState()
			{
			}

			// Token: 0x06001459 RID: 5209 RVA: 0x0007EC3C File Offset: 0x0007CE3C
			public void OnEnter(Vlad iOwner)
			{
			}

			// Token: 0x0600145A RID: 5210 RVA: 0x0007EC3E File Offset: 0x0007CE3E
			public void OnUpdate(float iDeltaTime, Vlad iOwner)
			{
			}

			// Token: 0x0600145B RID: 5211 RVA: 0x0007EC40 File Offset: 0x0007CE40
			public void OnExit(Vlad iOwner)
			{
			}

			// Token: 0x040015B9 RID: 5561
			private static Vlad.OutroState mSingelton;

			// Token: 0x040015BA RID: 5562
			private static volatile object mSingeltonLock = new object();
		}
	}
}
