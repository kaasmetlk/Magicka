using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x02000186 RID: 390
	public class Dash : Ability
	{
		// Token: 0x06000BE3 RID: 3043 RVA: 0x00047E26 File Offset: 0x00046026
		public Dash(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMinRange = iInput.ReadSingle();
			this.mMaxRange = iInput.ReadSingle();
			this.mArc = iInput.ReadSingle();
			this.mVector = iInput.ReadVector3();
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x00047E60 File Offset: 0x00046060
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x00047E64 File Offset: 0x00046064
		public override bool InternalExecute(Agent iAgent)
		{
			if (iAgent.Owner.IsGripping)
			{
				return false;
			}
			iAgent.Owner.Dash(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], false);
			iAgent.Owner.NextDashAnimation = this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)];
			return true;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x00047ECA File Offset: 0x000460CA
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mMaxRange;
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x00047ED2 File Offset: 0x000460D2
		public override float GetMinRange(Agent iAgent)
		{
			return this.mMinRange;
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x00047EDA File Offset: 0x000460DA
		public override float GetArc(Agent iAgent)
		{
			return this.mArc;
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x00047EE2 File Offset: 0x000460E2
		public override int[] GetWeapons()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x00047EEC File Offset: 0x000460EC
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			if (iArgs.AI.Owner.IsGripping)
			{
				return float.MinValue;
			}
			return FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x00047F2C File Offset: 0x0004612C
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = iAgent.CurrentTarget.Position;
			Vector3 result;
			Vector3.Subtract(ref position2, ref position, out result);
			float num = result.Length();
			if (num > 1E-45f)
			{
				Vector3.Divide(ref result, num, out result);
				return result;
			}
			return Vector3.Forward;
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x00047F7D File Offset: 0x0004617D
		public override bool IsUseful(Agent iAgent)
		{
			return !iAgent.Owner.IsGripping && base.IsUseful(iAgent);
		}

		// Token: 0x04000AE5 RID: 2789
		private float mMaxRange;

		// Token: 0x04000AE6 RID: 2790
		private float mMinRange;

		// Token: 0x04000AE7 RID: 2791
		private float mArc;

		// Token: 0x04000AE8 RID: 2792
		private Vector3 mVector;
	}
}
