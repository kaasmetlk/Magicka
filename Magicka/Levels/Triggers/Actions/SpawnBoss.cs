using System;
using System.Reflection;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000388 RID: 904
	public class SpawnBoss : Action
	{
		// Token: 0x06001B93 RID: 7059 RVA: 0x000BE181 File Offset: 0x000BC381
		public SpawnBoss(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x000BE1A0 File Offset: 0x000BC3A0
		public override void Initialize()
		{
			base.Initialize();
			if (this.mProp)
			{
				this.mBossRef = new PropBoss(base.GameScene.PlayState, this.mBoss, this.mUniqueID, this.mOnDeathId, this.mOnDamageId);
				base.GameScene.PlayState.Content.Load<PhysicsEntityTemplate>("Data/PhysicsEntities/" + this.mBoss);
			}
			else if (this.mGeneric)
			{
				this.mBossRef = new GenericBoss(base.GameScene.PlayState, this.mBoss.GetHashCodeCustom(), this.mUniqueID, this.mMeshIdx);
				base.GameScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/" + this.mBoss);
			}
			else
			{
				this.mBossRef = SpawnBoss.GetBoss(this.mBoss, base.GameScene.PlayState, this.mUniqueID);
			}
			this.mBossFight = BossFight.Instance;
		}

		// Token: 0x06001B95 RID: 7061 RVA: 0x000BE29C File Offset: 0x000BC49C
		protected override void Execute()
		{
			if (!this.mBossFight.IsSetup)
			{
				BossFight.Instance.Setup(base.GameScene.PlayState, this.mFreezeTime, this.mHealthAppearDelay, this.mHealthBarWidth);
			}
			this.mBossFight.Initialize(this.mBossRef, this.mAreaHash, this.mUniqueID);
			if (!this.mDelayed)
			{
				this.mBossFight.Start();
			}
		}

		// Token: 0x06001B96 RID: 7062 RVA: 0x000BE30D File Offset: 0x000BC50D
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x06001B97 RID: 7063 RVA: 0x000BE318 File Offset: 0x000BC518
		protected static Type GetBossType(string name)
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					Type[] interfaces = types[i].GetInterfaces();
					for (int j = 0; j < interfaces.Length; j++)
					{
						if (interfaces[j] == typeof(IBoss))
						{
							return types[i];
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06001B98 RID: 7064 RVA: 0x000BE37C File Offset: 0x000BC57C
		public static IBoss GetBoss(string iClassName, PlayState iPlayState, int iUniqueID)
		{
			Type bossType = SpawnBoss.GetBossType(iClassName);
			ConstructorInfo constructor = bossType.GetConstructor(new Type[]
			{
				typeof(PlayState)
			});
			return constructor.Invoke(new object[]
			{
				iPlayState
			}) as IBoss;
		}

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x06001B99 RID: 7065 RVA: 0x000BE3C8 File Offset: 0x000BC5C8
		// (set) Token: 0x06001B9A RID: 7066 RVA: 0x000BE3D0 File Offset: 0x000BC5D0
		public string ID
		{
			get
			{
				return this.mUniqueName;
			}
			set
			{
				this.mUniqueName = value;
				this.mUniqueID = this.mUniqueName.GetHashCodeCustom();
			}
		}

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x06001B9B RID: 7067 RVA: 0x000BE3EA File Offset: 0x000BC5EA
		// (set) Token: 0x06001B9C RID: 7068 RVA: 0x000BE3F2 File Offset: 0x000BC5F2
		public string Boss
		{
			get
			{
				return this.mBoss;
			}
			set
			{
				this.mBoss = value;
			}
		}

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x06001B9D RID: 7069 RVA: 0x000BE3FB File Offset: 0x000BC5FB
		// (set) Token: 0x06001B9E RID: 7070 RVA: 0x000BE403 File Offset: 0x000BC603
		public bool Prop
		{
			get
			{
				return this.mProp;
			}
			set
			{
				this.mProp = value;
			}
		}

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06001B9F RID: 7071 RVA: 0x000BE40C File Offset: 0x000BC60C
		// (set) Token: 0x06001BA0 RID: 7072 RVA: 0x000BE414 File Offset: 0x000BC614
		public bool Generic
		{
			get
			{
				return this.mGeneric;
			}
			set
			{
				this.mGeneric = value;
			}
		}

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06001BA1 RID: 7073 RVA: 0x000BE41D File Offset: 0x000BC61D
		// (set) Token: 0x06001BA2 RID: 7074 RVA: 0x000BE425 File Offset: 0x000BC625
		public string Template
		{
			get
			{
				return this.mCharacterTemplate;
			}
			set
			{
				this.mCharacterTemplate = value;
			}
		}

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x06001BA3 RID: 7075 RVA: 0x000BE42E File Offset: 0x000BC62E
		// (set) Token: 0x06001BA4 RID: 7076 RVA: 0x000BE436 File Offset: 0x000BC636
		public bool Delayed
		{
			get
			{
				return this.mDelayed;
			}
			set
			{
				this.mDelayed = value;
			}
		}

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x06001BA5 RID: 7077 RVA: 0x000BE43F File Offset: 0x000BC63F
		// (set) Token: 0x06001BA6 RID: 7078 RVA: 0x000BE447 File Offset: 0x000BC647
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaHash = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x06001BA7 RID: 7079 RVA: 0x000BE461 File Offset: 0x000BC661
		// (set) Token: 0x06001BA8 RID: 7080 RVA: 0x000BE469 File Offset: 0x000BC669
		public float HealthAppearDelay
		{
			get
			{
				return this.mHealthAppearDelay;
			}
			set
			{
				this.mHealthAppearDelay = value;
			}
		}

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06001BA9 RID: 7081 RVA: 0x000BE472 File Offset: 0x000BC672
		// (set) Token: 0x06001BAA RID: 7082 RVA: 0x000BE47A File Offset: 0x000BC67A
		public float FreezeTime
		{
			get
			{
				return this.mFreezeTime;
			}
			set
			{
				this.mFreezeTime = value;
			}
		}

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x06001BAB RID: 7083 RVA: 0x000BE483 File Offset: 0x000BC683
		// (set) Token: 0x06001BAC RID: 7084 RVA: 0x000BE48B File Offset: 0x000BC68B
		public float HealthBarWidth
		{
			get
			{
				return this.mHealthBarWidth;
			}
			set
			{
				this.mHealthBarWidth = value;
			}
		}

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x06001BAD RID: 7085 RVA: 0x000BE494 File Offset: 0x000BC694
		// (set) Token: 0x06001BAE RID: 7086 RVA: 0x000BE49C File Offset: 0x000BC69C
		public int MeshId
		{
			get
			{
				return this.mMeshIdx;
			}
			set
			{
				this.mMeshIdx = value;
			}
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x06001BAF RID: 7087 RVA: 0x000BE4A5 File Offset: 0x000BC6A5
		// (set) Token: 0x06001BB0 RID: 7088 RVA: 0x000BE4AD File Offset: 0x000BC6AD
		public string OnDeath
		{
			get
			{
				return this.mOnDeath;
			}
			set
			{
				this.mOnDeath = value;
				this.mOnDeathId = this.mOnDeath.GetHashCodeCustom();
			}
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x06001BB1 RID: 7089 RVA: 0x000BE4C7 File Offset: 0x000BC6C7
		// (set) Token: 0x06001BB2 RID: 7090 RVA: 0x000BE4CF File Offset: 0x000BC6CF
		public string OnDamage
		{
			get
			{
				return this.mOnDamage;
			}
			set
			{
				this.mOnDamage = value;
				this.mOnDamageId = this.mOnDamage.GetHashCodeCustom();
			}
		}

		// Token: 0x04001DFC RID: 7676
		private string mBoss;

		// Token: 0x04001DFD RID: 7677
		private bool mGeneric;

		// Token: 0x04001DFE RID: 7678
		private bool mProp;

		// Token: 0x04001DFF RID: 7679
		private string mOnDeath;

		// Token: 0x04001E00 RID: 7680
		private int mOnDeathId;

		// Token: 0x04001E01 RID: 7681
		private string mOnDamage;

		// Token: 0x04001E02 RID: 7682
		private int mOnDamageId;

		// Token: 0x04001E03 RID: 7683
		private string mCharacterTemplate;

		// Token: 0x04001E04 RID: 7684
		private string mArea;

		// Token: 0x04001E05 RID: 7685
		private int mAreaHash;

		// Token: 0x04001E06 RID: 7686
		private float mHealthAppearDelay;

		// Token: 0x04001E07 RID: 7687
		private float mFreezeTime;

		// Token: 0x04001E08 RID: 7688
		private float mHealthBarWidth = 0.8f;

		// Token: 0x04001E09 RID: 7689
		private int mUniqueID;

		// Token: 0x04001E0A RID: 7690
		private string mUniqueName;

		// Token: 0x04001E0B RID: 7691
		private IBoss mBossRef;

		// Token: 0x04001E0C RID: 7692
		private BossFight mBossFight;

		// Token: 0x04001E0D RID: 7693
		private bool mDelayed;

		// Token: 0x04001E0E RID: 7694
		private int mMeshIdx = -1;
	}
}
