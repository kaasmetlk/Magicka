using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000301 RID: 769
	public struct AuraBoost
	{
		// Token: 0x060017A6 RID: 6054 RVA: 0x0009BDE4 File Offset: 0x00099FE4
		public AuraBoost(float iMagnitude)
		{
			this.Magnitude = iMagnitude;
		}

		// Token: 0x060017A7 RID: 6055 RVA: 0x0009BDED File Offset: 0x00099FED
		public AuraBoost(ContentReader iInput)
		{
			this.Magnitude = iInput.ReadSingle();
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x0009BDFB File Offset: 0x00099FFB
		public float Execute(Character iOwner, AuraTarget iAuraTarget, int iEffect, float iRadius)
		{
			return 1f;
		}

		// Token: 0x04001966 RID: 6502
		public float Magnitude;
	}
}
