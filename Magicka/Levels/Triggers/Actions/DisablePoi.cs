using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200059D RID: 1437
	public class DisablePoi : Action
	{
		// Token: 0x06002AE0 RID: 10976 RVA: 0x001518C4 File Offset: 0x0014FAC4
		public DisablePoi(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002AE1 RID: 10977 RVA: 0x001518CE File Offset: 0x0014FACE
		protected override void Execute()
		{
			(base.GameScene.Triggers[this.mIDHash] as Interactable).Enabled = false;
		}

		// Token: 0x06002AE2 RID: 10978 RVA: 0x001518F1 File Offset: 0x0014FAF1
		public override void QuickExecute()
		{
			(base.GameScene.Triggers[this.mIDHash] as Interactable).Enabled = false;
		}

		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x06002AE3 RID: 10979 RVA: 0x00151914 File Offset: 0x0014FB14
		// (set) Token: 0x06002AE4 RID: 10980 RVA: 0x0015191C File Offset: 0x0014FB1C
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

		// Token: 0x04002E26 RID: 11814
		private string mID;

		// Token: 0x04002E27 RID: 11815
		private int mIDHash;
	}
}
