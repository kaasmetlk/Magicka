using System;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x0200046A RID: 1130
	internal class MulExpression : Expression
	{
		// Token: 0x06002257 RID: 8791 RVA: 0x000F5A28 File Offset: 0x000F3C28
		public MulExpression(Expression iChildA, Expression iChildB)
		{
			this.mChildA = iChildA;
			this.mChildB = iChildB;
		}

		// Token: 0x06002258 RID: 8792 RVA: 0x000F5A3E File Offset: 0x000F3C3E
		public override float GetValue(ref ExpressionArguments iArgs)
		{
			return this.mChildA.GetValue(ref iArgs) * this.mChildB.GetValue(ref iArgs);
		}

		// Token: 0x04002569 RID: 9577
		public Expression mChildA;

		// Token: 0x0400256A RID: 9578
		public Expression mChildB;
	}
}
