using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x02000282 RID: 642
	public class BossHealth : Condition
	{
		// Token: 0x060012FD RID: 4861 RVA: 0x000758A2 File Offset: 0x00073AA2
		public BossHealth(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x000758AB File Offset: 0x00073AAB
		public override bool IsMet(Character iSender)
		{
			return base.Scene.PlayState.BossFight != null && base.IsMet(iSender);
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x000758C8 File Offset: 0x00073AC8
		protected override bool InternalMet(Character iSender)
		{
			float normalizedHealth = base.Scene.PlayState.BossFight.NormalizedHealth;
			switch (this.mCompareMethod)
			{
			case CompareMethod.LESS:
				return normalizedHealth < this.mValue;
			case CompareMethod.EQUAL:
				return Math.Abs(normalizedHealth - this.mValue) <= 1E-06f;
			case CompareMethod.GREATER:
				return normalizedHealth > this.mValue;
			default:
				throw new Exception("Invalid compare method!");
			}
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06001300 RID: 4864 RVA: 0x0007593E File Offset: 0x00073B3E
		// (set) Token: 0x06001301 RID: 4865 RVA: 0x00075946 File Offset: 0x00073B46
		public CompareMethod CompareMethod
		{
			get
			{
				return this.mCompareMethod;
			}
			set
			{
				this.mCompareMethod = value;
			}
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06001302 RID: 4866 RVA: 0x0007594F File Offset: 0x00073B4F
		// (set) Token: 0x06001303 RID: 4867 RVA: 0x00075957 File Offset: 0x00073B57
		public float Value
		{
			get
			{
				return this.mValue;
			}
			set
			{
				this.mValue = value;
			}
		}

		// Token: 0x040014C5 RID: 5317
		private CompareMethod mCompareMethod;

		// Token: 0x040014C6 RID: 5318
		private float mValue;
	}
}
