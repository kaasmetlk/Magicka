using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000289 RID: 649
	public class SetDialog : Action
	{
		// Token: 0x06001327 RID: 4903 RVA: 0x000762BF File Offset: 0x000744BF
		public SetDialog(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x000762CC File Offset: 0x000744CC
		protected override void Execute()
		{
			Character character = Entity.GetByID(this.mIDHash) as Character;
			if (character != null)
			{
				character.Dialog = this.mDialog.GetHashCodeCustom();
			}
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x000762FE File Offset: 0x000744FE
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x0600132A RID: 4906 RVA: 0x00076306 File Offset: 0x00074506
		// (set) Token: 0x0600132B RID: 4907 RVA: 0x0007630E File Offset: 0x0007450E
		public string Dialog
		{
			get
			{
				return this.mDialog;
			}
			set
			{
				this.mDialog = value;
				this.mDialogHash = this.mDialog.GetHashCodeCustom();
			}
		}

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x0600132C RID: 4908 RVA: 0x00076328 File Offset: 0x00074528
		// (set) Token: 0x0600132D RID: 4909 RVA: 0x00076330 File Offset: 0x00074530
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

		// Token: 0x040014DA RID: 5338
		private string mDialog;

		// Token: 0x040014DB RID: 5339
		private string mID;

		// Token: 0x040014DC RID: 5340
		private int mDialogHash;

		// Token: 0x040014DD RID: 5341
		private int mIDHash;
	}
}
