using System;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000387 RID: 903
	internal class SpawnDispenser : Action
	{
		// Token: 0x06001B82 RID: 7042 RVA: 0x000BDF29 File Offset: 0x000BC129
		public SpawnDispenser(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x000BDF3C File Offset: 0x000BC13C
		public override void Initialize()
		{
			base.Initialize();
			for (int i = 0; i < this.mTypeID.Length; i++)
			{
				base.GameScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/" + this.mTypeID[i]);
			}
		}

		// Token: 0x06001B84 RID: 7044 RVA: 0x000BDF8C File Offset: 0x000BC18C
		protected override void Execute()
		{
			Dispenser fromCache = Dispenser.GetFromCache(base.GameScene.PlayState);
			Matrix iTransform;
			base.GameScene.GetLocator(this.mAreaID, out iTransform);
			fromCache.Initialize(iTransform, this.mModel, this.mTypeIDHash, this.mAmount, this.mTime, this.mActive);
			base.GameScene.PlayState.EntityManager.AddEntity(fromCache);
		}

		// Token: 0x06001B85 RID: 7045 RVA: 0x000BDFF8 File Offset: 0x000BC1F8
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x000BE000 File Offset: 0x000BC200
		public float GetTotalHitPoins()
		{
			float num = 0f;
			for (int i = 0; i < this.mTypeIDHash.Length; i++)
			{
				int num2 = this.mAmount.Length;
				if (num2 >= i)
				{
					num2 = 0;
				}
				CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeIDHash[i]);
				num += cachedTemplate.MaxHitpoints * (float)this.mAmount[num2];
			}
			return num;
		}

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06001B87 RID: 7047 RVA: 0x000BE057 File Offset: 0x000BC257
		// (set) Token: 0x06001B88 RID: 7048 RVA: 0x000BE05F File Offset: 0x000BC25F
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaID = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x06001B89 RID: 7049 RVA: 0x000BE079 File Offset: 0x000BC279
		// (set) Token: 0x06001B8A RID: 7050 RVA: 0x000BE081 File Offset: 0x000BC281
		public Dispensers Model
		{
			get
			{
				return this.mModel;
			}
			set
			{
				this.mModel = value;
			}
		}

		// Token: 0x170006B8 RID: 1720
		// (set) Token: 0x06001B8B RID: 7051 RVA: 0x000BE08C File Offset: 0x000BC28C
		public string Types
		{
			set
			{
				this.mTypeID = value.Split(new char[]
				{
					','
				});
				this.mTypeIDHash = new int[this.mTypeID.Length];
				for (int i = 0; i < this.mTypeIDHash.Length; i++)
				{
					this.mTypeIDHash[i] = this.mTypeID[i].GetHashCodeCustom();
				}
			}
		}

		// Token: 0x170006B9 RID: 1721
		// (set) Token: 0x06001B8C RID: 7052 RVA: 0x000BE0F0 File Offset: 0x000BC2F0
		public string Amount
		{
			set
			{
				string[] array = value.Split(new char[]
				{
					','
				});
				this.mAmount = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.mAmount[i] = int.Parse(array[i]);
				}
			}
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x06001B8D RID: 7053 RVA: 0x000BE13D File Offset: 0x000BC33D
		// (set) Token: 0x06001B8E RID: 7054 RVA: 0x000BE145 File Offset: 0x000BC345
		public float Time
		{
			get
			{
				return this.mTime;
			}
			set
			{
				this.mTime = value;
			}
		}

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06001B8F RID: 7055 RVA: 0x000BE14E File Offset: 0x000BC34E
		// (set) Token: 0x06001B90 RID: 7056 RVA: 0x000BE156 File Offset: 0x000BC356
		public string ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				this.mIDHash = this.mID.GetHashCodeCustom();
			}
		}

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x06001B91 RID: 7057 RVA: 0x000BE170 File Offset: 0x000BC370
		// (set) Token: 0x06001B92 RID: 7058 RVA: 0x000BE178 File Offset: 0x000BC378
		public bool Active
		{
			get
			{
				return this.mActive;
			}
			set
			{
				this.mActive = value;
			}
		}

		// Token: 0x04001DF2 RID: 7666
		private string mArea;

		// Token: 0x04001DF3 RID: 7667
		private int mAreaID;

		// Token: 0x04001DF4 RID: 7668
		private Dispensers mModel;

		// Token: 0x04001DF5 RID: 7669
		private string[] mTypeID;

		// Token: 0x04001DF6 RID: 7670
		private int[] mTypeIDHash;

		// Token: 0x04001DF7 RID: 7671
		private int[] mAmount;

		// Token: 0x04001DF8 RID: 7672
		private bool mActive = true;

		// Token: 0x04001DF9 RID: 7673
		private float mTime;

		// Token: 0x04001DFA RID: 7674
		private string mID;

		// Token: 0x04001DFB RID: 7675
		private int mIDHash;
	}
}
