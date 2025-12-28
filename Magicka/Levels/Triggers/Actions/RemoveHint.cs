using System;
using System.Xml;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020004F9 RID: 1273
	internal class RemoveHint : Action
	{
		// Token: 0x060025AA RID: 9642 RVA: 0x001115C8 File Offset: 0x0010F7C8
		public RemoveHint(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060025AB RID: 9643 RVA: 0x001115D2 File Offset: 0x0010F7D2
		protected override void Execute()
		{
			TutorialManager.Instance.RemoveHint();
		}

		// Token: 0x060025AC RID: 9644 RVA: 0x001115DE File Offset: 0x0010F7DE
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
