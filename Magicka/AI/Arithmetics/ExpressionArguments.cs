using System;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x02000464 RID: 1124
	public struct ExpressionArguments
	{
		// Token: 0x06002247 RID: 8775 RVA: 0x000F5448 File Offset: 0x000F3648
		public static void NewExpressionArguments(Agent iAI, IDamageable iTarget, out ExpressionArguments oExpressionArguments)
		{
			oExpressionArguments.AI = iAI;
			oExpressionArguments.AIPos = iAI.Owner.Position;
			oExpressionArguments.AIDir = iAI.Owner.Direction;
			oExpressionArguments.Target = iTarget;
			oExpressionArguments.TargetPos = iTarget.Position;
			oExpressionArguments.TargetDir = iTarget.Body.Orientation.Forward;
			Vector3.Subtract(ref oExpressionArguments.TargetPos, ref oExpressionArguments.AIPos, out oExpressionArguments.Delta);
			oExpressionArguments.Distance = oExpressionArguments.Delta.Length();
			Vector3.Divide(ref oExpressionArguments.Delta, oExpressionArguments.Distance, out oExpressionArguments.DeltaNormalized);
		}

		// Token: 0x04002555 RID: 9557
		public Agent AI;

		// Token: 0x04002556 RID: 9558
		public IDamageable Target;

		// Token: 0x04002557 RID: 9559
		public Vector3 Delta;

		// Token: 0x04002558 RID: 9560
		public Vector3 DeltaNormalized;

		// Token: 0x04002559 RID: 9561
		public float Distance;

		// Token: 0x0400255A RID: 9562
		public Vector3 AIPos;

		// Token: 0x0400255B RID: 9563
		public Vector3 TargetPos;

		// Token: 0x0400255C RID: 9564
		public Vector3 AIDir;

		// Token: 0x0400255D RID: 9565
		public Vector3 TargetDir;
	}
}
