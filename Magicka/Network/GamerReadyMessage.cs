using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x0200021B RID: 539
	internal struct GamerReadyMessage : ISendable
	{
		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001121 RID: 4385 RVA: 0x0006A6C5 File Offset: 0x000688C5
		public PacketType PacketType
		{
			get
			{
				return PacketType.GamerReady;
			}
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x0006A6C9 File Offset: 0x000688C9
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Id);
			iWriter.Write(this.Ready);
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x0006A6E3 File Offset: 0x000688E3
		public void Read(BinaryReader iReader)
		{
			this.Id = iReader.ReadByte();
			this.Ready = iReader.ReadBoolean();
		}

		// Token: 0x04000FEB RID: 4075
		public byte Id;

		// Token: 0x04000FEC RID: 4076
		public bool Ready;
	}
}
