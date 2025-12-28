using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x02000559 RID: 1369
	internal class NoCutsceneActive : Condition
	{
		// Token: 0x060028D8 RID: 10456 RVA: 0x001419A3 File Offset: 0x0013FBA3
		public NoCutsceneActive(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060028D9 RID: 10457 RVA: 0x001419AC File Offset: 0x0013FBAC
		protected override bool InternalMet(Character iSender)
		{
			return !base.Scene.PlayState.IsInCutscene;
		}
	}
}
