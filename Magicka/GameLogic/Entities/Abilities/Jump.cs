using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x0200031A RID: 794
	public class Jump : Ability
	{
		// Token: 0x06001861 RID: 6241 RVA: 0x000A18C0 File Offset: 0x0009FAC0
		public Jump(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMaxRange = iInput.ReadSingle();
			this.mMinRange = iInput.ReadSingle();
			this.mAngle = iInput.ReadSingle();
			this.mElevation = iInput.ReadSingle();
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x000A18FC File Offset: 0x0009FAFC
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			Character character = iArgs.Target as Character;
			if (!(character is Avatar))
			{
				return float.MinValue;
			}
			return float.MaxValue;
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x000A1928 File Offset: 0x0009FB28
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06001864 RID: 6244 RVA: 0x000A192C File Offset: 0x0009FB2C
		public override bool InternalExecute(Agent iAgent)
		{
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = iAgent.CurrentTarget.Position;
			Vector3 velocity = iAgent.CurrentTarget.Body.Velocity;
			if (velocity.LengthSquared() > 1E-45f)
			{
				Vector3.Multiply(ref velocity, 0.5f, out velocity);
				Vector3.Add(ref position2, ref velocity, out position2);
			}
			Vector3 iDelta;
			Vector3.Subtract(ref position2, ref position, out iDelta);
			float num = iDelta.Length();
			if (num < 1E-45f)
			{
				return false;
			}
			Vector3.Divide(ref iDelta, num, out iDelta);
			num = MathHelper.Clamp(num * 1.1f, this.mMinRange, this.mMaxRange);
			Vector3.Multiply(ref iDelta, num, out iDelta);
			iAgent.Owner.Jump(iDelta, this.mElevation);
			return true;
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x000A19EC File Offset: 0x0009FBEC
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mMaxRange;
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x000A19F4 File Offset: 0x0009FBF4
		public override float GetMinRange(Agent iAgent)
		{
			return this.mMinRange;
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x000A19FC File Offset: 0x0009FBFC
		public override float GetArc(Agent iAgent)
		{
			return this.mAngle;
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x000A1A04 File Offset: 0x0009FC04
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x000A1A08 File Offset: 0x0009FC08
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

		// Token: 0x0600186A RID: 6250 RVA: 0x000A1A5C File Offset: 0x0009FC5C
		public override bool InRange(Agent iAgent)
		{
			if (iAgent.CurrentTarget == null)
			{
				return false;
			}
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = iAgent.CurrentTarget.Position;
			float num;
			Vector3.DistanceSquared(ref position, ref position2, out num);
			return num >= this.mMinRange * this.mMinRange & num <= this.mMaxRange * this.mMaxRange;
		}

		// Token: 0x04001A24 RID: 6692
		private float mMaxRange;

		// Token: 0x04001A25 RID: 6693
		private float mMinRange;

		// Token: 0x04001A26 RID: 6694
		private float mAngle;

		// Token: 0x04001A27 RID: 6695
		private float mElevation;
	}
}
