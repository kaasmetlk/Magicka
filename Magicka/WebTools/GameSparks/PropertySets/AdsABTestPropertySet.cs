using System;

namespace Magicka.WebTools.GameSparks.PropertySets
{
	// Token: 0x020000D8 RID: 216
	internal class AdsABTestPropertySet : GameSparksPropertySet
	{
		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x0002679B File Offset: 0x0002499B
		public override string Name
		{
			get
			{
				return "ABTestAdLevel";
			}
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x000267A4 File Offset: 0x000249A4
		protected override void SetDefaults()
		{
			GameSparksPropertySet.Property value = new GameSparksPropertySet.Property("Ads", GameSparksPropertySet.PropertyType.BOOL, true);
			this.mDefaultProperties.Add("Ads", value);
		}

		// Token: 0x04000549 RID: 1353
		private const string AD_PROPERTY_NAME = "Ads";

		// Token: 0x0400054A RID: 1354
		private const string SET_NAME = "ABTestAdLevel";
	}
}
