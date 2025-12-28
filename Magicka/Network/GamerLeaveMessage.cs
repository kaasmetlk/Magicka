using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x0200021A RID: 538
	internal struct GamerLeaveMessage : ISendable
	{
		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x0600111E RID: 4382 RVA: 0x0006A6A5 File Offset: 0x000688A5
		public PacketType PacketType
		{
			get
			{
				return PacketType.GamerLeave;
			}
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x0006A6A9 File Offset: 0x000688A9
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Id);
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x0006A6B7 File Offset: 0x000688B7
		public void Read(BinaryReader iReader)
		{
			this.Id = iReader.ReadByte();
		}

		// Token: 0x04000FEA RID: 4074
		public byte Id;
	}
}
