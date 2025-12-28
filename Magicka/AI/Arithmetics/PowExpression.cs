using System;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x02000469 RID: 1129
	internal class PowExpression : Expression
	{
		// Token: 0x06002255 RID: 8789 RVA: 0x000F59F0 File Offset: 0x000F3BF0
		public PowExpression(Expression iChildA, Expression iChildB)
		{
			this.mChildA = iChildA;
			this.mChildB = iChildB;
		}

		// Token: 0x06002256 RID: 8790 RVA: 0x000F5A06 File Offset: 0x000F3C06
		public override float GetValue(ref ExpressionArguments iArgs)
		{
			return (float)Math.Pow((double)this.mChildA.GetValue(ref iArgs), (double)this.mChildB.GetValue(ref iArgs));
		}

		// Token: 0x04002567 RID: 9575
		public Expression mChildA;

		// Token: 0x04002568 RID: 9576
		public Expression mChildB;
	}
}
