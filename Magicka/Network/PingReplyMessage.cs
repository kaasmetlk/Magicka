using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x0200020A RID: 522
	internal struct PingReplyMessage : ISendable
	{
		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x060010F2 RID: 4338 RVA: 0x00069E77 File Offset: 0x00068077
		public PacketType PacketType
		{
			get
			{
				return PacketType.Ping;
			}
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x00069E7A File Offset: 0x0006807A
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Payload);
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x00069E88 File Offset: 0x00068088
		public void Read(BinaryReader iReader)
		{
			this.Payload = iReader.ReadInt32();
		}

		// Token: 0x04000FB2 RID: 4018
		public int Payload;
	}
}
