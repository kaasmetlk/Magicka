using System;

namespace Magicka.WebTools.Paradox.Telemetry.TypeValidators
{
	// Token: 0x0200027D RID: 637
	public class PlayerSegmentTypeValidator : BaseTypeValidator<string>
	{
		// Token: 0x060012D2 RID: 4818 RVA: 0x00074DBC File Offset: 0x00072FBC
		protected override bool OnMatchType(string iValue)
		{
			return PlayerSegment.IsSegmentString(iValue);
		}
	}
}
