using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000412 RID: 1042
	internal struct TriggerRequestMessage : ISendable
	{
		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x06002050 RID: 8272 RVA: 0x000E43F6 File Offset: 0x000E25F6
		public PacketType PacketType
		{
			get
			{
				return (PacketType)151;
			}
		}

		// Token: 0x06002051 RID: 8273 RVA: 0x000E43FD File Offset: 0x000E25FD
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.Scene);
			iWriter.Write(this.Id);
		}

		// Token: 0x06002052 RID: 8274 RVA: 0x000E4423 File Offset: 0x000E2623
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
			this.Scene = iReader.ReadInt32();
			this.Id = iReader.ReadInt32();
		}

		// Token: 0x040022B4 RID: 8884
		public ushort Handle;

		// Token: 0x040022B5 RID: 8885
		public int Scene;

		// Token: 0x040022B6 RID: 8886
		public int Id;
	}
}
