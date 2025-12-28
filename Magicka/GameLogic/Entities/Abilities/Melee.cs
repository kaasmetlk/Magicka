using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x02000357 RID: 855
	public class Melee : Ability
	{
		// Token: 0x060019F3 RID: 6643 RVA: 0x000AD6A0 File Offset: 0x000AB8A0
		public Melee(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMinRange = iInput.ReadSingle();
			this.mMaxRange = iInput.ReadSingle();
			this.mArc = iInput.ReadSingle();
			this.mWeapons = new int[iInput.ReadInt32()];
			for (int i = 0; i < this.mWeapons.Length; i++)
			{
				this.mWeapons[i] = iInput.ReadInt32();
			}
			this.mRotate = iInput.ReadBoolean();
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x000AD718 File Offset: 0x000AB918
		public Melee(Melee iCloneSource) : base(iCloneSource)
		{
			this.mWeapons = new int[iCloneSource.mWeapons.Length];
			iCloneSource.mWeapons.CopyTo(this.mWeapons, 0);
			this.mMinRange = iCloneSource.mMinRange;
			this.mMaxRange = iCloneSource.mMaxRange;
			this.mArc = iCloneSource.mArc;
			this.mRotate = iCloneSource.mRotate;
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x000AD784 File Offset: 0x000AB984
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			if (iArgs.AI.Owner.IsGripping)
			{
				return float.MinValue;
			}
			return FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
		}

		// Token: 0x060019F6 RID: 6646 RVA: 0x000AD7C2 File Offset: 0x000AB9C2
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x000AD7C4 File Offset: 0x000AB9C4
		public override bool InternalExecute(Agent iAgent)
		{
			iAgent.Owner.Attack(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], this.mRotate);
			return true;
		}

		// Token: 0x060019F8 RID: 6648 RVA: 0x000AD7F1 File Offset: 0x000AB9F1
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mMaxRange;
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x000AD7F9 File Offset: 0x000AB9F9
		public override float GetMinRange(Agent iAgent)
		{
			return this.mMinRange;
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x000AD801 File Offset: 0x000ABA01
		public override float GetArc(Agent iAgent)
		{
			return this.mArc;
		}

		// Token: 0x060019FB RID: 6651 RVA: 0x000AD809 File Offset: 0x000ABA09
		public override int[] GetWeapons()
		{
			return this.mWeapons;
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x000AD811 File Offset: 0x000ABA11
		public bool AllowRotation()
		{
			return this.mRotate;
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x000AD819 File Offset: 0x000ABA19
		public override bool IsUseful(Agent iAgent)
		{
			return !iAgent.Owner.IsGripping;
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x000AD82C File Offset: 0x000ABA2C
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

		// Token: 0x04001C28 RID: 7208
		private int[] mWeapons;

		// Token: 0x04001C29 RID: 7209
		private float mMaxRange;

		// Token: 0x04001C2A RID: 7210
		private float mMinRange;

		// Token: 0x04001C2B RID: 7211
		private float mArc;

		// Token: 0x04001C2C RID: 7212
		private bool mRotate;
	}
}
