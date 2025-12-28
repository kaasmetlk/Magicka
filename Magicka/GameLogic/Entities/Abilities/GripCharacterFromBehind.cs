using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x02000363 RID: 867
	public class GripCharacterFromBehind : Ability
	{
		// Token: 0x06001A72 RID: 6770 RVA: 0x000B3BDA File Offset: 0x000B1DDA
		public GripCharacterFromBehind(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMaxRange = iInput.ReadSingle();
			this.mMinRange = iInput.ReadSingle();
			this.mAngle = iInput.ReadSingle();
			this.mMaxWeight = iInput.ReadSingle();
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x000B3C14 File Offset: 0x000B1E14
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			Character character = iArgs.Target as Character;
			if ((iArgs.AI.Owner.IsGripping | !(character is Avatar)) || character.IsGripped)
			{
				return float.MinValue;
			}
			float num = FuzzyMath.FuzzyAngle(ref iArgs.DeltaNormalized, ref iArgs.TargetDir);
			num *= num;
			num *= 2f;
			num += FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
			return num * 0.33333334f;
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x000B3C9B File Offset: 0x000B1E9B
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x000B3CA0 File Offset: 0x000B1EA0
		public override bool InternalExecute(Agent iAgent)
		{
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = iAgent.CurrentTarget.Position;
			Vector3 iDelta;
			Vector3.Subtract(ref position2, ref position, out iDelta);
			float num = iDelta.Length();
			Vector3.Divide(ref iDelta, num, out iDelta);
			num = MathHelper.Clamp(num * 1.2f, this.mMinRange, this.mMaxRange);
			Vector3.Multiply(ref iDelta, num, out iDelta);
			iAgent.Owner.Jump(iDelta, 0.7853982f);
			return true;
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x000B3D17 File Offset: 0x000B1F17
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mMaxRange;
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x000B3D1F File Offset: 0x000B1F1F
		public override float GetMinRange(Agent iAgent)
		{
			return this.mMinRange;
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x000B3D27 File Offset: 0x000B1F27
		public override float GetArc(Agent iAgent)
		{
			return this.mAngle;
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x000B3D2F File Offset: 0x000B1F2F
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x000B3D34 File Offset: 0x000B1F34
		public override bool InRange(Agent iAgent)
		{
			Vector3 position = iAgent.Owner.Position;
			Vector3 vector = iAgent.CurrentTarget.Position;
			Vector3.Subtract(ref vector, ref position, out position);
			vector = (iAgent.CurrentTarget as Character).Direction;
			position.Normalize();
			float num;
			Vector3.Dot(ref vector, ref position, out num);
			return num > 0.5f && base.InRange(iAgent);
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x000B3D98 File Offset: 0x000B1F98
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

		// Token: 0x06001A7C RID: 6780 RVA: 0x000B3DE9 File Offset: 0x000B1FE9
		public override bool IsUseful(Agent iAgent)
		{
			return iAgent.CurrentTarget is Character && !iAgent.Owner.IsGripping;
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x000B3E08 File Offset: 0x000B2008
		public override bool ChooseAttackAngle(Agent iAgent, out Vector3 oDirection)
		{
			float scaleFactor = (this.mMaxRange + this.mMinRange) * -0.5f;
			Vector3 direction = (iAgent.CurrentTarget as Character).Direction;
			Vector3.Multiply(ref direction, scaleFactor, out direction);
			oDirection = direction;
			return true;
		}

		// Token: 0x04001CCB RID: 7371
		private float mMaxRange;

		// Token: 0x04001CCC RID: 7372
		private float mMinRange;

		// Token: 0x04001CCD RID: 7373
		private float mAngle;

		// Token: 0x04001CCE RID: 7374
		private float mMaxWeight;
	}
}
