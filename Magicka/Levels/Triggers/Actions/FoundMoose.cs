using System;
using Magicka.GameLogic;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200028B RID: 651
	public class FoundMoose : Action
	{
		// Token: 0x06001331 RID: 4913 RVA: 0x00076368 File Offset: 0x00074568
		public FoundMoose(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x00076372 File Offset: 0x00074572
		protected override void Execute()
		{
			Profile.Instance.FoundAMoose(base.GameScene.PlayState, this.mID);
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x0007638F File Offset: 0x0007458F
		public override void QuickExecute()
		{
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06001334 RID: 4916 RVA: 0x00076391 File Offset: 0x00074591
		// (set) Token: 0x06001335 RID: 4917 RVA: 0x00076399 File Offset: 0x00074599
		public string ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
			}
		}

		// Token: 0x040014DE RID: 5342
		private string mID;
	}
}
