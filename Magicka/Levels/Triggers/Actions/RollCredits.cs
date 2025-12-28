using System;
using Magicka.GameLogic.UI;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000288 RID: 648
	internal class RollCredits : Action
	{
		// Token: 0x06001321 RID: 4897 RVA: 0x00076257 File Offset: 0x00074457
		public RollCredits(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x00076261 File Offset: 0x00074461
		public override void Initialize()
		{
			base.Initialize();
			Credits.Instance.Initialize(this.mFileName);
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x00076279 File Offset: 0x00074479
		protected override void Execute()
		{
			Credits.Instance.Start(this.mScene.PlayState.SaveSlot);
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x00076295 File Offset: 0x00074495
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06001325 RID: 4901 RVA: 0x0007629D File Offset: 0x0007449D
		// (set) Token: 0x06001326 RID: 4902 RVA: 0x000762A5 File Offset: 0x000744A5
		public string File
		{
			get
			{
				return this.mFileName;
			}
			set
			{
				this.mFileName = value;
				this.mFileHash = this.mFileName.GetHashCodeCustom();
			}
		}

		// Token: 0x040014D8 RID: 5336
		private string mFileName;

		// Token: 0x040014D9 RID: 5337
		private int mFileHash;
	}
}
