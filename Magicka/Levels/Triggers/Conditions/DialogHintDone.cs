using System;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x02000627 RID: 1575
	internal class DialogHintDone : Condition
	{
		// Token: 0x06002F48 RID: 12104 RVA: 0x00180A05 File Offset: 0x0017EC05
		public DialogHintDone(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06002F49 RID: 12105 RVA: 0x00180A0E File Offset: 0x0017EC0E
		protected override bool InternalMet(Character iSender)
		{
			return TutorialManager.Instance.IsDialogHintDone();
		}
	}
}
