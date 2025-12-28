using System;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x0200019D RID: 413
	public interface IEventValidator
	{
		// Token: 0x06000C4A RID: 3146
		bool Validate(string iEventName, object[] iValues);
	}
}
