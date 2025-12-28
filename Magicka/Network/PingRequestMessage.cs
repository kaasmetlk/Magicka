using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000209 RID: 521
	internal struct PingRequestMessage : ISendable
	{
		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x060010EF RID: 4335 RVA: 0x00069E54 File Offset: 0x00068054
		public PacketType PacketType
		{
			get
			{
				return PacketType.Request;
			}
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x00069E5B File Offset: 0x0006805B
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Payload);
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x00069E69 File Offset: 0x00068069
		public void Read(BinaryReader iReader)
		{
			this.Payload = iReader.ReadInt32();
		}

		// Token: 0x04000FB1 RID: 4017
		public int Payload;
	}
}
