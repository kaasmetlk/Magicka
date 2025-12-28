using System;
using Magicka.Network;

namespace Magicka.WebTools.Paradox.Telemetry.TypeValidators
{
	// Token: 0x0200031D RID: 797
	public class NetworkStateTypeValidator : BaseTypeValidator<NetworkState>
	{
		// Token: 0x0600187C RID: 6268 RVA: 0x000A27A0 File Offset: 0x000A09A0
		protected override string ToString(NetworkState iValue)
		{
			string result = "invalid";
			switch (iValue)
			{
			case NetworkState.Offline:
				result = "offline";
				break;
			case NetworkState.Server:
				result = "host";
				break;
			case NetworkState.Client:
				result = "client";
				break;
			}
			return result;
		}

		// Token: 0x04001A2C RID: 6700
		private const string TELEMETRY_OFFLINE = "offline";

		// Token: 0x04001A2D RID: 6701
		private const string TELEMETRY_HOST = "host";

		// Token: 0x04001A2E RID: 6702
		private const string TELEMETRY_CLIENT = "client";

		// Token: 0x04001A2F RID: 6703
		private const string TELEMETRY_INVALID = "invalid";
	}
}
