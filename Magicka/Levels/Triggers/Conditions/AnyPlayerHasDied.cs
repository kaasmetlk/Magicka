using System;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020000B9 RID: 185
	internal class AnyPlayerHasDied : Condition
	{
		// Token: 0x06000563 RID: 1379 RVA: 0x00020186 File Offset: 0x0001E386
		public AnyPlayerHasDied(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0002018F File Offset: 0x0001E38F
		protected override bool InternalMet(Character iSender)
		{
			return PlayState.RecentPlayState.DiedInLevel;
		}
	}
}
