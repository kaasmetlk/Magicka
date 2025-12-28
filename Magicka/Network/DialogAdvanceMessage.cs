using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x0200022F RID: 559
	internal struct DialogAdvanceMessage : ISendable
	{
		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x0600114E RID: 4430 RVA: 0x0006B80D File Offset: 0x00069A0D
		public PacketType PacketType
		{
			get
			{
				return PacketType.DialogAdvance;
			}
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x0006B811 File Offset: 0x00069A11
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Interact);
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x0006B81F File Offset: 0x00069A1F
		public void Read(BinaryReader iReader)
		{
			this.Interact = iReader.ReadInt32();
		}

		// Token: 0x04001047 RID: 4167
		public int Interact;
	}
}
