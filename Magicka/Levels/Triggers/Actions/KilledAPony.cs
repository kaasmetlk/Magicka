using System;
using Magicka.GameLogic;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000331 RID: 817
	public class KilledAPony : Action
	{
		// Token: 0x060018F9 RID: 6393 RVA: 0x000A41BB File Offset: 0x000A23BB
		public KilledAPony(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060018FA RID: 6394 RVA: 0x000A41C5 File Offset: 0x000A23C5
		protected override void Execute()
		{
			Profile.Instance.KilledAPony(base.GameScene.PlayState, this.mID);
		}

		// Token: 0x060018FB RID: 6395 RVA: 0x000A41E2 File Offset: 0x000A23E2
		public override void QuickExecute()
		{
		}

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x060018FC RID: 6396 RVA: 0x000A41E4 File Offset: 0x000A23E4
		// (set) Token: 0x060018FD RID: 6397 RVA: 0x000A41EC File Offset: 0x000A23EC
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

		// Token: 0x04001AC4 RID: 6852
		private string mID;
	}
}
