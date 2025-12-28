using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x02000409 RID: 1033
	public class ConfuseGrip : Ability
	{
		// Token: 0x06001FDA RID: 8154 RVA: 0x000DFC3F File Offset: 0x000DDE3F
		public ConfuseGrip(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
		}

		// Token: 0x06001FDB RID: 8155 RVA: 0x000DFC49 File Offset: 0x000DDE49
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			if (!iArgs.AI.Owner.IsGripping)
			{
				return float.MinValue;
			}
			return 1f;
		}

		// Token: 0x06001FDC RID: 8156 RVA: 0x000DFC68 File Offset: 0x000DDE68
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x06001FDD RID: 8157 RVA: 0x000DFC6A File Offset: 0x000DDE6A
		public override bool InternalExecute(Agent iAgent)
		{
			iAgent.Owner.Attack(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], false);
			return true;
		}

		// Token: 0x06001FDE RID: 8158 RVA: 0x000DFC92 File Offset: 0x000DDE92
		public override float GetMaxRange(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x06001FDF RID: 8159 RVA: 0x000DFC99 File Offset: 0x000DDE99
		public override float GetMinRange(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x06001FE0 RID: 8160 RVA: 0x000DFCA0 File Offset: 0x000DDEA0
		public override float GetArc(Agent iAgent)
		{
			return 3.1415927f;
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x000DFCA7 File Offset: 0x000DDEA7
		public override int[] GetWeapons()
		{
			return null;
		}

		// Token: 0x06001FE2 RID: 8162 RVA: 0x000DFCAA File Offset: 0x000DDEAA
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			return Vector3.Forward;
		}

		// Token: 0x06001FE3 RID: 8163 RVA: 0x000DFCB1 File Offset: 0x000DDEB1
		public override bool IsUseful(Agent iAgent)
		{
			return iAgent.Owner.IsGripping;
		}

		// Token: 0x06001FE4 RID: 8164 RVA: 0x000DFCBE File Offset: 0x000DDEBE
		public override bool InRange(Agent iAgent)
		{
			return true;
		}

		// Token: 0x06001FE5 RID: 8165 RVA: 0x000DFCC1 File Offset: 0x000DDEC1
		public override bool FacingTarget(Agent iAgent)
		{
			return true;
		}
	}
}
