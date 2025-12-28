using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000221 RID: 545
	internal struct GameEndLoadMessage : ISendable
	{
		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001130 RID: 4400 RVA: 0x0006A871 File Offset: 0x00068A71
		public PacketType PacketType
		{
			get
			{
				return PacketType.GameEndLoad;
			}
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x0006A875 File Offset: 0x00068A75
		public void Write(BinaryWriter iWriter)
		{
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x0006A877 File Offset: 0x00068A77
		public void Read(BinaryReader iReader)
		{
		}

		// Token: 0x04001005 RID: 4101
		private byte mPlaceHolder;
	}
}
