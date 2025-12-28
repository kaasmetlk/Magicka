using System;
using System.Xml;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020002CB RID: 715
	internal class Cutscene : Action
	{
		// Token: 0x060015C4 RID: 5572 RVA: 0x0008AF24 File Offset: 0x00089124
		public Cutscene(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x0008AF35 File Offset: 0x00089135
		protected override void Execute()
		{
			this.mScene.PlayState.BeginCutscene(this.mOnSkipID, this.mSkipBarMove, this.mKillDialogs);
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x0008AF59 File Offset: 0x00089159
		public override void QuickExecute()
		{
			this.mScene.PlayState.BeginCutscene(this.mOnSkipID, true, this.mKillDialogs);
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x060015C7 RID: 5575 RVA: 0x0008AF78 File Offset: 0x00089178
		// (set) Token: 0x060015C8 RID: 5576 RVA: 0x0008AF80 File Offset: 0x00089180
		public bool SkipBarMove
		{
			get
			{
				return this.mSkipBarMove;
			}
			set
			{
				this.mSkipBarMove = value;
			}
		}

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x060015C9 RID: 5577 RVA: 0x0008AF89 File Offset: 0x00089189
		// (set) Token: 0x060015CA RID: 5578 RVA: 0x0008AF91 File Offset: 0x00089191
		public bool KillDialogs
		{
			get
			{
				return this.mKillDialogs;
			}
			set
			{
				this.mKillDialogs = value;
			}
		}

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x060015CB RID: 5579 RVA: 0x0008AF9A File Offset: 0x0008919A
		// (set) Token: 0x060015CC RID: 5580 RVA: 0x0008AFA2 File Offset: 0x000891A2
		public string OnSkip
		{
			get
			{
				return this.mOnSkip;
			}
			set
			{
				this.mOnSkip = value;
				this.mOnSkipID = value.GetHashCodeCustom();
			}
		}

		// Token: 0x04001713 RID: 5907
		private string mOnSkip;

		// Token: 0x04001714 RID: 5908
		private int mOnSkipID;

		// Token: 0x04001715 RID: 5909
		private bool mSkipBarMove;

		// Token: 0x04001716 RID: 5910
		private bool mKillDialogs = true;
	}
}
