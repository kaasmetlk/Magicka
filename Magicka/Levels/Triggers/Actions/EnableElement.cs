using System;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200041B RID: 1051
	internal class EnableElement : Action
	{
		// Token: 0x06002089 RID: 8329 RVA: 0x000E6D5E File Offset: 0x000E4F5E
		public EnableElement(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600208A RID: 8330 RVA: 0x000E6D68 File Offset: 0x000E4F68
		protected override void Execute()
		{
			TutorialManager.Instance.EnableElement(this.mElement);
		}

		// Token: 0x0600208B RID: 8331 RVA: 0x000E6D7A File Offset: 0x000E4F7A
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x0600208C RID: 8332 RVA: 0x000E6D82 File Offset: 0x000E4F82
		// (set) Token: 0x0600208D RID: 8333 RVA: 0x000E6D8A File Offset: 0x000E4F8A
		public Elements Element
		{
			get
			{
				return this.mElement;
			}
			set
			{
				this.mElement = value;
			}
		}

		// Token: 0x04002308 RID: 8968
		private Elements mElement;
	}
}
