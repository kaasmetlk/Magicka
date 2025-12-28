using System;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x0200046C RID: 1132
	internal class NotExpression : Expression
	{
		// Token: 0x0600225B RID: 8795 RVA: 0x000F5A8A File Offset: 0x000F3C8A
		public NotExpression(Expression iChild)
		{
			this.mChild = iChild;
		}

		// Token: 0x0600225C RID: 8796 RVA: 0x000F5A99 File Offset: 0x000F3C99
		public override float GetValue(ref ExpressionArguments iArgs)
		{
			return 1f - this.mChild.GetValue(ref iArgs);
		}

		// Token: 0x0400256D RID: 9581
		public Expression mChild;
	}
}
