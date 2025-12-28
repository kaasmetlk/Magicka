using System;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000144 RID: 324
	internal class DisableElement : Action
	{
		// Token: 0x06000925 RID: 2341 RVA: 0x00039839 File Offset: 0x00037A39
		public DisableElement(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x00039843 File Offset: 0x00037A43
		protected override void Execute()
		{
			TutorialManager.Instance.DisableElement(this.mElement);
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x00039855 File Offset: 0x00037A55
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000928 RID: 2344 RVA: 0x0003985D File Offset: 0x00037A5D
		// (set) Token: 0x06000929 RID: 2345 RVA: 0x00039865 File Offset: 0x00037A65
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

		// Token: 0x0400087A RID: 2170
		private Elements mElement;
	}
}
