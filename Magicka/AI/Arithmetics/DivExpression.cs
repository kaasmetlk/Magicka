using System;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x0200046B RID: 1131
	internal class DivExpression : Expression
	{
		// Token: 0x06002259 RID: 8793 RVA: 0x000F5A59 File Offset: 0x000F3C59
		public DivExpression(Expression iChildA, Expression iChildB)
		{
			this.mChildA = iChildA;
			this.mChildB = iChildB;
		}

		// Token: 0x0600225A RID: 8794 RVA: 0x000F5A6F File Offset: 0x000F3C6F
		public override float GetValue(ref ExpressionArguments iArgs)
		{
			return this.mChildA.GetValue(ref iArgs) / this.mChildB.GetValue(ref iArgs);
		}

		// Token: 0x0400256B RID: 9579
		public Expression mChildA;

		// Token: 0x0400256C RID: 9580
		public Expression mChildB;
	}
}
