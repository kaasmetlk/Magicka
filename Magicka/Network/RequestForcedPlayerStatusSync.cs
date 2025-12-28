using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000234 RID: 564
	internal struct RequestForcedPlayerStatusSync : ISendable
	{
		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x0600115D RID: 4445 RVA: 0x0006B971 File Offset: 0x00069B71
		public PacketType PacketType
		{
			get
			{
				return PacketType.RequestForcedPlayerStatusSync;
			}
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0006B975 File Offset: 0x00069B75
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x0006B983 File Offset: 0x00069B83
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
		}

		// Token: 0x04001053 RID: 4179
		public ushort Handle;
	}
}
