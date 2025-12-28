using System;
using Magicka.AI;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.MoveAbilities
{
	// Token: 0x020005EE RID: 1518
	internal class Jump : MoveAbility
	{
		// Token: 0x06002DF9 RID: 11769 RVA: 0x0017526A File Offset: 0x0017346A
		public Jump(ContentReader iInput) : base(iInput)
		{
			this.mAngle = 0f;
			this.mForceMultiplier = 0f;
			this.mMinRange = iInput.ReadSingle();
			this.mMaxRange = iInput.ReadSingle();
		}

		// Token: 0x06002DFA RID: 11770 RVA: 0x001752A1 File Offset: 0x001734A1
		public override void Execute(Agent iAgent)
		{
			iAgent.Owner.NextAttackAnimation = this.mAnimationKeys[MagickaMath.Random.Next(this.mAnimationKeys.Length)];
		}

		// Token: 0x06002DFB RID: 11771 RVA: 0x001752C7 File Offset: 0x001734C7
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06002DFC RID: 11772 RVA: 0x001752C9 File Offset: 0x001734C9
		public override float GetFuzzyWeight(Agent iAgent)
		{
			return base.GetFuzzyWeight(iAgent);
		}

		// Token: 0x06002DFD RID: 11773 RVA: 0x001752D2 File Offset: 0x001734D2
		public override float GetMaxRange()
		{
			return this.mMaxRange;
		}

		// Token: 0x06002DFE RID: 11774 RVA: 0x001752DA File Offset: 0x001734DA
		public override float GetMinRange()
		{
			return this.mMinRange;
		}

		// Token: 0x06002DFF RID: 11775 RVA: 0x001752E2 File Offset: 0x001734E2
		public override float GetAngle()
		{
			return this.mAngle;
		}

		// Token: 0x06002E00 RID: 11776 RVA: 0x001752EA File Offset: 0x001734EA
		public override float GetForceMultiplier()
		{
			return this.mForceMultiplier;
		}

		// Token: 0x040031BC RID: 12732
		private float mMaxRange;

		// Token: 0x040031BD RID: 12733
		private float mMinRange;

		// Token: 0x040031BE RID: 12734
		private float mAngle;

		// Token: 0x040031BF RID: 12735
		private float mForceMultiplier;
	}
}
