using System;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x0200046D RID: 1133
	internal class Constant : Expression
	{
		// Token: 0x0600225D RID: 8797 RVA: 0x000F5AAD File Offset: 0x000F3CAD
		public Constant(float iValue)
		{
			this.Value = iValue;
		}

		// Token: 0x0600225E RID: 8798 RVA: 0x000F5ABC File Offset: 0x000F3CBC
		public override float GetValue(ref ExpressionArguments iArgs)
		{
			return this.Value;
		}

		// Token: 0x0400256E RID: 9582
		public float Value;
	}
}
