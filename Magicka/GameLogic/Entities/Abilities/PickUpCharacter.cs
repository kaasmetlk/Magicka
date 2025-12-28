using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x02000362 RID: 866
	public class PickUpCharacter : Ability
	{
		// Token: 0x06001A65 RID: 6757 RVA: 0x000B3924 File Offset: 0x000B1B24
		public PickUpCharacter(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMaxRange = iInput.ReadSingle();
			this.mMinRange = iInput.ReadSingle();
			this.mAngle = iInput.ReadSingle();
			this.mMaxWeight = iInput.ReadSingle();
			this.mDropAnimation = (Animations)Enum.Parse(typeof(Animations), iInput.ReadString(), true);
		}

		// Token: 0x06001A66 RID: 6758 RVA: 0x000B398C File Offset: 0x000B1B8C
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			Character character = iArgs.Target as Character;
			if (character == null | iArgs.Target.Body.Mass > this.mMaxWeight | iArgs.AI.Owner.IsGripping)
			{
				return float.MinValue;
			}
			float num = FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
			return num + (1f - character.CharacterBody.Movement.Length());
		}

		// Token: 0x06001A67 RID: 6759 RVA: 0x000B3A10 File Offset: 0x000B1C10
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06001A68 RID: 6760 RVA: 0x000B3A14 File Offset: 0x000B1C14
		public override bool InternalExecute(Agent iAgent)
		{
			iAgent.Owner.DropAnimation = this.mDropAnimation;
			iAgent.Owner.Attack(this.mAnimationKeys[0], false);
			if (this.mAnimationKeys.Length > 1)
			{
				iAgent.Owner.DamageGripped(this.mAnimationKeys[1]);
			}
			return true;
		}

		// Token: 0x06001A69 RID: 6761 RVA: 0x000B3A65 File Offset: 0x000B1C65
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mMaxRange;
		}

		// Token: 0x06001A6A RID: 6762 RVA: 0x000B3A6D File Offset: 0x000B1C6D
		public override float GetMinRange(Agent iAgent)
		{
			return this.mMinRange;
		}

		// Token: 0x06001A6B RID: 6763 RVA: 0x000B3A75 File Offset: 0x000B1C75
		public override float GetArc(Agent iAgent)
		{
			return this.mAngle;
		}

		// Token: 0x06001A6C RID: 6764 RVA: 0x000B3A7D File Offset: 0x000B1C7D
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x06001A6D RID: 6765 RVA: 0x000B3A80 File Offset: 0x000B1C80
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			Vector3 position = iAgent.CurrentTarget.Position;
			Vector3 position2 = iAgent.Owner.Position;
			Vector3 result;
			Vector3.Subtract(ref position, ref position2, out result);
			result.Y = 0f;
			float num = result.Length();
			if (num < 1E-06f)
			{
				return Vector3.Forward;
			}
			Vector3.Divide(ref result, num, out result);
			return result;
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x000B3AE0 File Offset: 0x000B1CE0
		public override bool IsUseful(ref ExpressionArguments iArgs)
		{
			bool result = true;
			if (iArgs.AI.Owner.IsGripping)
			{
				result = false;
			}
			else if (!(iArgs.Target is Character))
			{
				result = false;
			}
			else if (iArgs.Target.Body.Mass > this.mMaxWeight)
			{
				result = false;
			}
			else if ((iArgs.Target as Character).IsSolidSelfShielded)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x000B3B48 File Offset: 0x000B1D48
		public override bool IsUseful(Agent iAgent)
		{
			bool result = true;
			if (iAgent.Owner.IsGripping)
			{
				result = false;
			}
			else if (!(iAgent.CurrentTarget is Character))
			{
				result = false;
			}
			else if (iAgent.CurrentTarget.Body.Mass > this.mMaxWeight)
			{
				result = false;
			}
			else if ((iAgent.CurrentTarget as Character).IsSolidSelfShielded)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06001A70 RID: 6768 RVA: 0x000B3BAA File Offset: 0x000B1DAA
		public override bool InRange(Agent iAgent)
		{
			return iAgent.Owner.IsGripping || base.InRange(iAgent);
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x000B3BC2 File Offset: 0x000B1DC2
		public override bool FacingTarget(Agent iAgent)
		{
			return iAgent.Owner.IsGripping || base.FacingTarget(iAgent);
		}

		// Token: 0x04001CC6 RID: 7366
		private float mMaxRange;

		// Token: 0x04001CC7 RID: 7367
		private float mMinRange;

		// Token: 0x04001CC8 RID: 7368
		private float mAngle;

		// Token: 0x04001CC9 RID: 7369
		private float mMaxWeight;

		// Token: 0x04001CCA RID: 7370
		private Animations mDropAnimation;
	}
}
