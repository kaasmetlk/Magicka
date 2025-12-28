using System;
using Magicka.AI;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.MoveAbilities
{
	// Token: 0x020000E4 RID: 228
	internal class Run : MoveAbility
	{
		// Token: 0x06000709 RID: 1801 RVA: 0x000295BF File Offset: 0x000277BF
		public Run(ContentReader iInput) : base(iInput)
		{
			this.mMinRange = iInput.ReadSingle();
			this.mMaxRange = iInput.ReadSingle();
			this.mArc = iInput.ReadSingle();
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x000295EC File Offset: 0x000277EC
		public override void Execute(Agent iAgent)
		{
			iAgent.Owner.Attack(this.mAnimationKeys[MagickaMath.Random.Next(this.mAnimationKeys.Length)], false);
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x00029613 File Offset: 0x00027813
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x00029615 File Offset: 0x00027815
		public override float GetFuzzyWeight(Agent iAgent)
		{
			return base.GetFuzzyWeight(iAgent);
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x0002961E File Offset: 0x0002781E
		public override float GetForceMultiplier()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x00029625 File Offset: 0x00027825
		public override float GetAngle()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0002962C File Offset: 0x0002782C
		public override float GetMaxRange()
		{
			return this.mMaxRange;
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x00029634 File Offset: 0x00027834
		public override float GetMinRange()
		{
			return this.mMinRange;
		}

		// Token: 0x040005B7 RID: 1463
		private float mMaxRange;

		// Token: 0x040005B8 RID: 1464
		private float mMinRange;

		// Token: 0x040005B9 RID: 1465
		private float mArc;
	}
}
