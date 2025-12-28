using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x020005B1 RID: 1457
	internal class SpecialAbilityAbility : Ability
	{
		// Token: 0x06002B94 RID: 11156 RVA: 0x0015864C File Offset: 0x0015684C
		public SpecialAbilityAbility(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMaxRange = iInput.ReadSingle();
			this.mMinRange = iInput.ReadSingle();
			this.mAngle = iInput.ReadSingle();
			this.mWeapon = iInput.ReadInt32();
		}

		// Token: 0x06002B95 RID: 11157 RVA: 0x00158688 File Offset: 0x00156888
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			return FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
		}

		// Token: 0x06002B96 RID: 11158 RVA: 0x001586AE File Offset: 0x001568AE
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06002B97 RID: 11159 RVA: 0x001586B0 File Offset: 0x001568B0
		public override bool InternalExecute(Agent iAgent)
		{
			if (this.mAnimationKeys.Length <= 0)
			{
				iAgent.Owner.Equipment[this.mWeapon].Item.ExecuteSpecialAbility();
			}
			else
			{
				iAgent.Owner.Attack(this.mAnimationKeys[MagickaMath.Random.Next(this.mAnimationKeys.Length)], false);
			}
			return true;
		}

		// Token: 0x06002B98 RID: 11160 RVA: 0x0015870C File Offset: 0x0015690C
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mMaxRange;
		}

		// Token: 0x06002B99 RID: 11161 RVA: 0x00158714 File Offset: 0x00156914
		public override float GetMinRange(Agent iAgent)
		{
			return this.mMinRange;
		}

		// Token: 0x06002B9A RID: 11162 RVA: 0x0015871C File Offset: 0x0015691C
		public override float GetArc(Agent iAgent)
		{
			return this.mAngle;
		}

		// Token: 0x06002B9B RID: 11163 RVA: 0x00158724 File Offset: 0x00156924
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x06002B9C RID: 11164 RVA: 0x00158728 File Offset: 0x00156928
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

		// Token: 0x06002B9D RID: 11165 RVA: 0x00158779 File Offset: 0x00156979
		public override bool IsUseful(Agent iAgent)
		{
			return iAgent.Owner.Equipment[this.mWeapon].Item.SpecialAbilityReady;
		}

		// Token: 0x06002B9E RID: 11166 RVA: 0x00158797 File Offset: 0x00156997
		public override bool IsUseful(ref ExpressionArguments iArgs)
		{
			return iArgs.AI.Owner.Equipment[this.mWeapon].Item.SpecialAbilityReady;
		}

		// Token: 0x04002F4F RID: 12111
		private float mMaxRange;

		// Token: 0x04002F50 RID: 12112
		private float mMinRange;

		// Token: 0x04002F51 RID: 12113
		private float mAngle;

		// Token: 0x04002F52 RID: 12114
		private int mWeapon;
	}
}
