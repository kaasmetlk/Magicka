using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020005CD RID: 1485
	public class BossDead : Condition
	{
		// Token: 0x06002C69 RID: 11369 RVA: 0x0015DA5F File Offset: 0x0015BC5F
		public BossDead(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06002C6A RID: 11370 RVA: 0x0015DA68 File Offset: 0x0015BC68
		public override bool IsMet(Character iSender)
		{
			return base.Scene.PlayState.BossFight != null && base.IsMet(iSender);
		}

		// Token: 0x06002C6B RID: 11371 RVA: 0x0015DA85 File Offset: 0x0015BC85
		protected override bool InternalMet(Character iSender)
		{
			return base.Scene.PlayState.BossFight.Dead;
		}
	}
}
