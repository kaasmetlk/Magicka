using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000212 RID: 530
	internal struct ConnectionClosedMessage : ISendable
	{
		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06001107 RID: 4359 RVA: 0x0006A14A File Offset: 0x0006834A
		public PacketType PacketType
		{
			get
			{
				return PacketType.ConnectionClosed;
			}
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x0006A14D File Offset: 0x0006834D
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.Reason);
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0006A15B File Offset: 0x0006835B
		public void Read(BinaryReader iReader)
		{
			this.Reason = (ConnectionClosedMessage.CReason)iReader.ReadByte();
		}

		// Token: 0x04000FC9 RID: 4041
		public ConnectionClosedMessage.CReason Reason;

		// Token: 0x02000213 RID: 531
		public enum CReason : byte
		{
			// Token: 0x04000FCB RID: 4043
			Kicked,
			// Token: 0x04000FCC RID: 4044
			Left,
			// Token: 0x04000FCD RID: 4045
			LostConnection,
			// Token: 0x04000FCE RID: 4046
			Unknown
		}
	}
}
