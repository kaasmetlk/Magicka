using System;
using Magicka.GameLogic;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020001AB RID: 427
	public class FoundSecretArea : Action
	{
		// Token: 0x06000CC9 RID: 3273 RVA: 0x0004B846 File Offset: 0x00049A46
		public FoundSecretArea(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x0004B850 File Offset: 0x00049A50
		protected override void Execute()
		{
			Profile.Instance.FoundSecretArea(base.GameScene.PlayState, this.mID);
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x0004B86D File Offset: 0x00049A6D
		public override void QuickExecute()
		{
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06000CCC RID: 3276 RVA: 0x0004B86F File Offset: 0x00049A6F
		// (set) Token: 0x06000CCD RID: 3277 RVA: 0x0004B877 File Offset: 0x00049A77
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

		// Token: 0x04000BA5 RID: 2981
		private string mID;
	}
}
