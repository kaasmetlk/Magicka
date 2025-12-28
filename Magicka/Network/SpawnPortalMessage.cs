using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Magicka.Network
{
	// Token: 0x020000A7 RID: 167
	internal struct SpawnPortalMessage : ISendable
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x0001B5E8 File Offset: 0x000197E8
		public PacketType PacketType
		{
			get
			{
				return PacketType.SpawnPortal;
			}
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x0001B5EC File Offset: 0x000197EC
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Position.X);
			iWriter.Write(this.Position.Y);
			iWriter.Write(this.Position.Z);
			iWriter.Write(this.AnimationHandle);
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x0001B638 File Offset: 0x00019838
		public void Read(BinaryReader iReader)
		{
			this.Position.X = iReader.ReadSingle();
			this.Position.Y = iReader.ReadSingle();
			this.Position.Z = iReader.ReadSingle();
			this.AnimationHandle = iReader.ReadUInt16();
		}

		// Token: 0x04000376 RID: 886
		public Vector3 Position;

		// Token: 0x04000377 RID: 887
		public ushort AnimationHandle;
	}
}
