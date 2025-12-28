using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x0200009B RID: 155
	internal struct SpawnPlayerMessage : ISendable
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x00017EF7 File Offset: 0x000160F7
		public PacketType PacketType
		{
			get
			{
				return PacketType.SpawnPlayer;
			}
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x00017EFC File Offset: 0x000160FC
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.Id);
			iWriter.Write(this.MagickRevive);
			iWriter.Write(this.Position.X);
			iWriter.Write(this.Position.Y);
			iWriter.Write(this.Position.Z);
			iWriter.Write(new HalfSingle(this.Direction.X).PackedValue);
			iWriter.Write(new HalfSingle(this.Direction.Z).PackedValue);
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x00017F9C File Offset: 0x0001619C
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
			this.Id = iReader.ReadByte();
			this.MagickRevive = iReader.ReadBoolean();
			this.Position.X = iReader.ReadSingle();
			this.Position.Y = iReader.ReadSingle();
			this.Position.Z = iReader.ReadSingle();
			HalfSingle halfSingle = default(HalfSingle);
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Direction.X = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Direction.Z = halfSingle.ToSingle();
		}

		// Token: 0x0400030C RID: 780
		public ushort Handle;

		// Token: 0x0400030D RID: 781
		public byte Id;

		// Token: 0x0400030E RID: 782
		public bool MagickRevive;

		// Token: 0x0400030F RID: 783
		public Vector3 Position;

		// Token: 0x04000310 RID: 784
		public Vector3 Direction;
	}
}
