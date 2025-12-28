using System;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.UI;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x0200013E RID: 318
	public class DialogDone : Condition
	{
		// Token: 0x060008F6 RID: 2294 RVA: 0x000393DA File Offset: 0x000375DA
		public DialogDone(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x000393EA File Offset: 0x000375EA
		protected override bool InternalMet(Character iSender)
		{
			return DialogManager.Instance.IsDialogDone(this.mID, this.mInteractIndex);
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x060008F8 RID: 2296 RVA: 0x00039402 File Offset: 0x00037602
		// (set) Token: 0x060008F9 RID: 2297 RVA: 0x0003940A File Offset: 0x0003760A
		public string Dialog
		{
			get
			{
				return this.mDialog;
			}
			set
			{
				this.mDialog = value;
				this.mID = this.mDialog.GetHashCodeCustom();
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x060008FA RID: 2298 RVA: 0x00039424 File Offset: 0x00037624
		// (set) Token: 0x060008FB RID: 2299 RVA: 0x0003942C File Offset: 0x0003762C
		public int InteractIndex
		{
			get
			{
				return this.mInteractIndex;
			}
			set
			{
				this.mInteractIndex = value;
			}
		}

		// Token: 0x04000865 RID: 2149
		private string mDialog;

		// Token: 0x04000866 RID: 2150
		private int mID;

		// Token: 0x04000867 RID: 2151
		private int mInteractIndex = -1;
	}
}
