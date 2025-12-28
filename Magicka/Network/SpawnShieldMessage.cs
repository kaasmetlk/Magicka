using System;
using System.IO;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x0200009F RID: 159
	internal struct SpawnShieldMessage : ISendable
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600049A RID: 1178 RVA: 0x00018696 File Offset: 0x00016896
		public PacketType PacketType
		{
			get
			{
				return PacketType.SpawnShield;
			}
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0001869C File Offset: 0x0001689C
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.OwnerHandle);
			iWriter.Write(this.Position.X);
			iWriter.Write(this.Position.Y);
			iWriter.Write(this.Position.Z);
			iWriter.Write(new Normalized101010(this.Direction).PackedValue);
			iWriter.Write(new HalfSingle(this.Radius).PackedValue);
			iWriter.Write(new HalfSingle(this.HitPoints).PackedValue);
			iWriter.Write((byte)this.ShieldType);
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0001874C File Offset: 0x0001694C
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
			this.OwnerHandle = iReader.ReadUInt16();
			this.Position.X = iReader.ReadSingle();
			this.Position.Y = iReader.ReadSingle();
			this.Position.Z = iReader.ReadSingle();
			HalfSingle halfSingle = default(HalfSingle);
			Normalized101010 normalized = default(Normalized101010);
			normalized.PackedValue = iReader.ReadUInt32();
			this.Direction = normalized.ToVector3();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Radius = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.HitPoints = halfSingle.ToSingle();
			this.ShieldType = (ShieldType)iReader.ReadByte();
		}

		// Token: 0x0400032F RID: 815
		public ushort Handle;

		// Token: 0x04000330 RID: 816
		public ushort OwnerHandle;

		// Token: 0x04000331 RID: 817
		public Vector3 Position;

		// Token: 0x04000332 RID: 818
		public Vector3 Direction;

		// Token: 0x04000333 RID: 819
		public float Radius;

		// Token: 0x04000334 RID: 820
		public float HitPoints;

		// Token: 0x04000335 RID: 821
		public ShieldType ShieldType;
	}
}
