using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000233 RID: 563
	internal struct LeaveSyncMessage : ISendable
	{
		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x0600115A RID: 4442 RVA: 0x0006B951 File Offset: 0x00069B51
		public PacketType PacketType
		{
			get
			{
				return PacketType.LeaveSync;
			}
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x0006B955 File Offset: 0x00069B55
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.ID);
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x0006B963 File Offset: 0x00069B63
		public void Read(BinaryReader iReader)
		{
			this.ID = iReader.ReadUInt32();
		}

		// Token: 0x04001052 RID: 4178
		public uint ID;
	}
}
