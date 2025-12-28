using System;
using System.Collections.Generic;
using System.Xml;
using Magicka.AI;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020001AD RID: 429
	public class ActivateDispenser : Action
	{
		// Token: 0x06000CD1 RID: 3281 RVA: 0x0004B8B8 File Offset: 0x00049AB8
		public ActivateDispenser(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
			this.mNode = iNode;
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x0004B8CC File Offset: 0x00049ACC
		public override void Initialize()
		{
			base.Initialize();
			LevelModel levelModel = base.GameScene.LevelModel;
			List<AIEvent> list = new List<AIEvent>();
			foreach (object obj in this.mNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlComment))
				{
					list.Add(new AIEvent(levelModel, xmlNode));
				}
			}
			this.mEvents = list.ToArray();
			this.mWanderSpeed = 1f;
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x0004B96C File Offset: 0x00049B6C
		protected override void Execute()
		{
			if (this.mIsSpecific)
			{
				Dispenser dispenser = Entity.GetByID(this.mIDHash) as Dispenser;
				if (dispenser != null)
				{
					dispenser.Activate();
					return;
				}
			}
			else
			{
				TriggerArea triggerArea = base.GameScene.GetTriggerArea(this.mAreaHash);
				for (int i = 0; i < triggerArea.PresentEntities.Count; i++)
				{
					Dispenser dispenser2 = Entity.GetByID(this.mIDHash) as Dispenser;
					if (dispenser2 != null && dispenser2.mType == this.mType)
					{
						dispenser2.Activate();
					}
				}
			}
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x0004B9EC File Offset: 0x00049BEC
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06000CD5 RID: 3285 RVA: 0x0004B9F4 File Offset: 0x00049BF4
		// (set) Token: 0x06000CD6 RID: 3286 RVA: 0x0004B9FC File Offset: 0x00049BFC
		public Order Order
		{
			get
			{
				return this.mOrder;
			}
			set
			{
				this.mOrder = value;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06000CD7 RID: 3287 RVA: 0x0004BA05 File Offset: 0x00049C05
		// (set) Token: 0x06000CD8 RID: 3288 RVA: 0x0004BA0D File Offset: 0x00049C0D
		public ReactTo ReactTo
		{
			get
			{
				return this.mReactTo;
			}
			set
			{
				this.mReactTo = value;
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06000CD9 RID: 3289 RVA: 0x0004BA16 File Offset: 0x00049C16
		// (set) Token: 0x06000CDA RID: 3290 RVA: 0x0004BA1E File Offset: 0x00049C1E
		public string ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				if (!string.IsNullOrEmpty(this.mID))
				{
					this.mIsSpecific = true;
					this.mIDHash = this.mID.GetHashCodeCustom();
					return;
				}
				this.mIsSpecific = false;
				this.mIDHash = 0;
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06000CDB RID: 3291 RVA: 0x0004BA5B File Offset: 0x00049C5B
		// (set) Token: 0x06000CDC RID: 3292 RVA: 0x0004BA63 File Offset: 0x00049C63
		public Dispensers Type
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06000CDD RID: 3293 RVA: 0x0004BA6C File Offset: 0x00049C6C
		// (set) Token: 0x06000CDE RID: 3294 RVA: 0x0004BA74 File Offset: 0x00049C74
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

		// Token: 0x04000BA6 RID: 2982
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x04000BA7 RID: 2983
		private XmlNode mNode;

		// Token: 0x04000BA8 RID: 2984
		protected Order mOrder;

		// Token: 0x04000BA9 RID: 2985
		protected ReactTo mReactTo;

		// Token: 0x04000BAA RID: 2986
		protected string mID;

		// Token: 0x04000BAB RID: 2987
		protected int mIDHash;

		// Token: 0x04000BAC RID: 2988
		protected Dispensers mType;

		// Token: 0x04000BAD RID: 2989
		protected string mArea;

		// Token: 0x04000BAE RID: 2990
		protected int mAreaHash;

		// Token: 0x04000BAF RID: 2991
		protected string mTargetArea;

		// Token: 0x04000BB0 RID: 2992
		protected int mTargetAreaHash;

		// Token: 0x04000BB1 RID: 2993
		protected float mWanderSpeed;

		// Token: 0x04000BB2 RID: 2994
		protected bool mIsSpecific;

		// Token: 0x04000BB3 RID: 2995
		internal AIEvent[] mEvents;
	}
}
