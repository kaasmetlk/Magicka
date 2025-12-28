using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x020004D8 RID: 1240
	public class ZombieGrip : Ability
	{
		// Token: 0x060024E1 RID: 9441 RVA: 0x0010A6C4 File Offset: 0x001088C4
		public ZombieGrip(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mMaxRange = iInput.ReadSingle();
			this.mMinRange = iInput.ReadSingle();
			this.mAngle = iInput.ReadSingle();
			this.mMaxWeight = iInput.ReadSingle();
			this.mDropAnimation = (Animations)Enum.Parse(typeof(Animations), iInput.ReadString(), true);
		}

		// Token: 0x060024E2 RID: 9442 RVA: 0x0010A72C File Offset: 0x0010892C
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			Character character = iArgs.Target as Character;
			if (character == null | character.IsGripped | iArgs.Target.Body.Mass > this.mMaxWeight | iArgs.AI.Owner.IsGripping)
			{
				return float.MinValue;
			}
			return float.MaxValue;
		}

		// Token: 0x060024E3 RID: 9443 RVA: 0x0010A787 File Offset: 0x00108987
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x060024E4 RID: 9444 RVA: 0x0010A78C File Offset: 0x0010898C
		public override bool InternalExecute(Agent iAgent)
		{
			if (iAgent.Owner.CurrentAnimation == this.mAnimationKeys[0])
			{
				if (iAgent.Owner.IsGripping)
				{
					iAgent.Owner.Attack(this.mAnimationKeys[1], false);
				}
			}
			else
			{
				iAgent.Owner.DropAnimation = this.mDropAnimation;
				iAgent.Owner.Attack(this.mAnimationKeys[1], false);
			}
			return true;
		}

		// Token: 0x060024E5 RID: 9445 RVA: 0x0010A7F7 File Offset: 0x001089F7
		public override float GetMaxRange(Agent iAgent)
		{
			return this.mMaxRange;
		}

		// Token: 0x060024E6 RID: 9446 RVA: 0x0010A7FF File Offset: 0x001089FF
		public override float GetMinRange(Agent iAgent)
		{
			return this.mMinRange;
		}

		// Token: 0x060024E7 RID: 9447 RVA: 0x0010A807 File Offset: 0x00108A07
		public override float GetArc(Agent iAgent)
		{
			return this.mAngle;
		}

		// Token: 0x060024E8 RID: 9448 RVA: 0x0010A80F File Offset: 0x00108A0F
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x060024E9 RID: 9449 RVA: 0x0010A814 File Offset: 0x00108A14
		public override bool IsUseful(ref ExpressionArguments iArgs)
		{
			return (!iArgs.AI.Owner.IsGripping || iArgs.AI.Owner.CurrentAnimation == this.mAnimationKeys[0]) && iArgs.Target.Body.Mass <= this.mMaxWeight;
		}

		// Token: 0x060024EA RID: 9450 RVA: 0x0010A86C File Offset: 0x00108A6C
		public override bool IsUseful(Agent iAgent)
		{
			return (!iAgent.Owner.IsGripping || iAgent.Owner.CurrentAnimation == this.mAnimationKeys[0]) && iAgent.CurrentTarget.Body.Mass <= this.mMaxWeight;
		}

		// Token: 0x060024EB RID: 9451 RVA: 0x0010A8B8 File Offset: 0x00108AB8
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

		// Token: 0x04002842 RID: 10306
		private float mMinRange;

		// Token: 0x04002843 RID: 10307
		private float mMaxRange;

		// Token: 0x04002844 RID: 10308
		private float mAngle;

		// Token: 0x04002845 RID: 10309
		private float mMaxWeight;

		// Token: 0x04002846 RID: 10310
		private Animations mDropAnimation;
	}
}
