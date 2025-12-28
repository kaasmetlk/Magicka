using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x02000368 RID: 872
	public class DamageGrip : Ability
	{
		// Token: 0x06001A97 RID: 6807 RVA: 0x000B4FA2 File Offset: 0x000B31A2
		public DamageGrip(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
		}

		// Token: 0x06001A98 RID: 6808 RVA: 0x000B4FAC File Offset: 0x000B31AC
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			if (!iArgs.AI.Owner.IsGripping)
			{
				return float.MinValue;
			}
			return (-2f * iArgs.AI.Owner.GripDamageAccumulation / iArgs.AI.Owner.HitTolerance + 1f) * 2f;
		}

		// Token: 0x06001A99 RID: 6809 RVA: 0x000B5004 File Offset: 0x000B3204
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x000B5006 File Offset: 0x000B3206
		public override bool InternalExecute(Agent iAgent)
		{
			iAgent.Owner.DamageGripped(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)]);
			return true;
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x000B502D File Offset: 0x000B322D
		public override float GetMaxRange(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x06001A9C RID: 6812 RVA: 0x000B5034 File Offset: 0x000B3234
		public override float GetMinRange(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x000B503B File Offset: 0x000B323B
		public override float GetArc(Agent iAgent)
		{
			return 3.1415927f;
		}

		// Token: 0x06001A9E RID: 6814 RVA: 0x000B5042 File Offset: 0x000B3242
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x000B5045 File Offset: 0x000B3245
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			return Vector3.Backward;
		}

		// Token: 0x06001AA0 RID: 6816 RVA: 0x000B504C File Offset: 0x000B324C
		public override bool InRange(Agent iAgent)
		{
			return true;
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x000B504F File Offset: 0x000B324F
		public override bool FacingTarget(Agent iAgent)
		{
			return true;
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x000B5052 File Offset: 0x000B3252
		public override bool IsUseful(Agent iAgent)
		{
			return iAgent.Owner.IsGripping && iAgent.Owner.GripDamageAccumulation < iAgent.Owner.HitTolerance / 2f;
		}
	}
}
