using System;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x02000468 RID: 1128
	internal class SubExpression : Expression
	{
		// Token: 0x06002253 RID: 8787 RVA: 0x000F59BF File Offset: 0x000F3BBF
		public SubExpression(Expression iChildA, Expression iChildB)
		{
			this.mChildA = iChildA;
			this.mChildB = iChildB;
		}

		// Token: 0x06002254 RID: 8788 RVA: 0x000F59D5 File Offset: 0x000F3BD5
		public override float GetValue(ref ExpressionArguments iArgs)
		{
			return this.mChildA.GetValue(ref iArgs) - this.mChildB.GetValue(ref iArgs);
		}

		// Token: 0x04002565 RID: 9573
		public Expression mChildA;

		// Token: 0x04002566 RID: 9574
		public Expression mChildB;
	}
}
