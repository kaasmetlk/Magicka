using System;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.UI;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020002C7 RID: 711
	internal class DialogActive : Condition
	{
		// Token: 0x060015A3 RID: 5539 RVA: 0x0008A8DB File Offset: 0x00088ADB
		public DialogActive(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x0008A8E4 File Offset: 0x00088AE4
		protected override bool InternalMet(Character iSender)
		{
			if (string.IsNullOrEmpty(this.mDialog) || this.mDialog.Equals("any", StringComparison.InvariantCultureIgnoreCase))
			{
				return DialogManager.Instance.MessageBoxActive;
			}
			return DialogManager.Instance.DialogActive(this.mID);
		}

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x060015A5 RID: 5541 RVA: 0x0008A921 File Offset: 0x00088B21
		// (set) Token: 0x060015A6 RID: 5542 RVA: 0x0008A929 File Offset: 0x00088B29
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

		// Token: 0x04001700 RID: 5888
		private string mDialog;

		// Token: 0x04001701 RID: 5889
		private int mID;
	}
}
