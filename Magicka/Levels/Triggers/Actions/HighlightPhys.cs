using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000243 RID: 579
	public class HighlightPhys : Action
	{
		// Token: 0x060011EA RID: 4586 RVA: 0x0006CD14 File Offset: 0x0006AF14
		public HighlightPhys(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x0006CD20 File Offset: 0x0006AF20
		protected override void Execute()
		{
			PhysicsEntity physicsEntity = Entity.GetByID(this.mIDHash) as PhysicsEntity;
			if (physicsEntity != null)
			{
				physicsEntity.Highlight(this.mTime);
			}
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x0006CD4D File Offset: 0x0006AF4D
		public override void QuickExecute()
		{
		}

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x060011ED RID: 4589 RVA: 0x0006CD4F File Offset: 0x0006AF4F
		// (set) Token: 0x060011EE RID: 4590 RVA: 0x0006CD57 File Offset: 0x0006AF57
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

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x060011EF RID: 4591 RVA: 0x0006CD71 File Offset: 0x0006AF71
		// (set) Token: 0x060011F0 RID: 4592 RVA: 0x0006CD79 File Offset: 0x0006AF79
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

		// Token: 0x040010A9 RID: 4265
		private string mID;

		// Token: 0x040010AA RID: 4266
		private int mIDHash;

		// Token: 0x040010AB RID: 4267
		private float mTime;
	}
}
