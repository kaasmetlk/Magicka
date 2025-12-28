using System;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x0200019B RID: 411
	public interface ITypeValidator
	{
		// Token: 0x06000C40 RID: 3136
		bool MatchType(object iObject);

		// Token: 0x06000C41 RID: 3137
		string GetFormattedString(object iValue);

		// Token: 0x06000C42 RID: 3138
		Type GetSystemType();
	}
}
