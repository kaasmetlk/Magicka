using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Magicka.Network
{
	// Token: 0x020000A6 RID: 166
	internal struct SpawnVortexMessage : ISendable
	{
		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060004AF RID: 1199 RVA: 0x0001B532 File Offset: 0x00019732
		public PacketType PacketType
		{
			get
			{
				return PacketType.SpawnVortex;
			}
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x0001B538 File Offset: 0x00019738
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.OwnerHandle);
			iWriter.Write(this.Position.X);
			iWriter.Write(this.Position.Y);
			iWriter.Write(this.Position.Z);
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x0001B590 File Offset: 0x00019790
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
			this.OwnerHandle = iReader.ReadUInt16();
			this.Position.X = iReader.ReadSingle();
			this.Position.Y = iReader.ReadSingle();
			this.Position.Z = iReader.ReadSingle();
		}

		// Token: 0x04000373 RID: 883
		public ushort Handle;

		// Token: 0x04000374 RID: 884
		public ushort OwnerHandle;

		// Token: 0x04000375 RID: 885
		public Vector3 Position;
	}
}
