using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000228 RID: 552
	internal struct EntityRemoveMessage : ISendable
	{
		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x0600113F RID: 4415 RVA: 0x0006B2BB File Offset: 0x000694BB
		public PacketType PacketType
		{
			get
			{
				return PacketType.EntityRemove;
			}
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x0006B2BF File Offset: 0x000694BF
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x0006B2CD File Offset: 0x000694CD
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
		}

		// Token: 0x04001027 RID: 4135
		public ushort Handle;
	}
}
