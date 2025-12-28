using System;

namespace Magicka.Network
{
	// Token: 0x02000479 RID: 1145
	internal enum ConnectionStatus
	{
		// Token: 0x040025E6 RID: 9702
		NotConnected,
		// Token: 0x040025E7 RID: 9703
		Connecting,
		// Token: 0x040025E8 RID: 9704
		Authenticating,
		// Token: 0x040025E9 RID: 9705
		Connected,
		// Token: 0x040025EA RID: 9706
		Failed_GameFull,
		// Token: 0x040025EB RID: 9707
		Failed_Authentication,
		// Token: 0x040025EC RID: 9708
		Failed_GamePlaying,
		// Token: 0x040025ED RID: 9709
		Failed_Version,
		// Token: 0x040025EE RID: 9710
		Failed_Password,
		// Token: 0x040025EF RID: 9711
		Failed_Unknown,
		// Token: 0x040025F0 RID: 9712
		Failed_Timeout
	}
}
