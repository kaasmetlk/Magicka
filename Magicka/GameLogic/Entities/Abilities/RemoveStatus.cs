using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x02000461 RID: 1121
	public class RemoveStatus : Ability
	{
		// Token: 0x06002239 RID: 8761 RVA: 0x000F5192 File Offset: 0x000F3392
		public RemoveStatus(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
		}

		// Token: 0x0600223A RID: 8762 RVA: 0x000F519C File Offset: 0x000F339C
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			throw new NotImplementedException("Remove status must define a desirability expression!");
		}

		// Token: 0x0600223B RID: 8763 RVA: 0x000F51A8 File Offset: 0x000F33A8
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x0600223C RID: 8764 RVA: 0x000F51AA File Offset: 0x000F33AA
		public override bool InternalExecute(Agent iAgent)
		{
			iAgent.Owner.Attack(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], false);
			return true;
		}

		// Token: 0x0600223D RID: 8765 RVA: 0x000F51D2 File Offset: 0x000F33D2
		public override float GetMaxRange(Agent iAgent)
		{
			return float.MaxValue;
		}

		// Token: 0x0600223E RID: 8766 RVA: 0x000F51D9 File Offset: 0x000F33D9
		public override float GetMinRange(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x0600223F RID: 8767 RVA: 0x000F51E0 File Offset: 0x000F33E0
		public override float GetArc(Agent iAgent)
		{
			return 3.1415927f;
		}

		// Token: 0x06002240 RID: 8768 RVA: 0x000F51E7 File Offset: 0x000F33E7
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x06002241 RID: 8769 RVA: 0x000F51EC File Offset: 0x000F33EC
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			return iAgent.NPC.Body.Orientation.Forward;
		}

		// Token: 0x06002242 RID: 8770 RVA: 0x000F5214 File Offset: 0x000F3414
		public override bool IsUseful(Agent iAgent)
		{
			for (int i = 0; i < this.mAnimationKeys.Length; i++)
			{
				if (this.mAnimationKeys[i] == iAgent.Owner.CurrentAnimation)
				{
					return false;
				}
			}
			return base.IsUseful(iAgent);
		}

		// Token: 0x06002243 RID: 8771 RVA: 0x000F5254 File Offset: 0x000F3454
		public override bool IsUseful(ref ExpressionArguments iArgs)
		{
			for (int i = 0; i < this.mAnimationKeys.Length; i++)
			{
				if (this.mAnimationKeys[i] == iArgs.AI.Owner.CurrentAnimation)
				{
					return false;
				}
			}
			return base.IsUseful(ref iArgs);
		}
	}
}
