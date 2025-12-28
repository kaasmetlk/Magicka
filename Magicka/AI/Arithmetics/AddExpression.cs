using System;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x02000467 RID: 1127
	internal class AddExpression : Expression
	{
		// Token: 0x06002251 RID: 8785 RVA: 0x000F598E File Offset: 0x000F3B8E
		public AddExpression(Expression iChildA, Expression iChildB)
		{
			this.mChildA = iChildA;
			this.mChildB = iChildB;
		}

		// Token: 0x06002252 RID: 8786 RVA: 0x000F59A4 File Offset: 0x000F3BA4
		public override float GetValue(ref ExpressionArguments iArgs)
		{
			return this.mChildA.GetValue(ref iArgs) + this.mChildB.GetValue(ref iArgs);
		}

		// Token: 0x04002563 RID: 9571
		public Expression mChildA;

		// Token: 0x04002564 RID: 9572
		public Expression mChildB;
	}
}
