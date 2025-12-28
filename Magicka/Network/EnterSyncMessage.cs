using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000232 RID: 562
	internal struct EnterSyncMessage : ISendable
	{
		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06001157 RID: 4439 RVA: 0x0006B931 File Offset: 0x00069B31
		public PacketType PacketType
		{
			get
			{
				return PacketType.EnterSync;
			}
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x0006B935 File Offset: 0x00069B35
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.ID);
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x0006B943 File Offset: 0x00069B43
		public void Read(BinaryReader iReader)
		{
			this.ID = iReader.ReadUInt32();
		}

		// Token: 0x04001051 RID: 4177
		public uint ID;
	}
}
