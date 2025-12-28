using System;
using System.IO;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x0200009E RID: 158
	internal struct SpawnShieldRequestMessage : ISendable
	{
		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000497 RID: 1175 RVA: 0x00018532 File Offset: 0x00016732
		public PacketType PacketType
		{
			get
			{
				return (PacketType)166;
			}
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0001853C File Offset: 0x0001673C
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.OwnerHandle);
			iWriter.Write(this.Position.X);
			iWriter.Write(this.Position.Y);
			iWriter.Write(this.Position.Z);
			iWriter.Write(new Normalized101010(this.Direction).PackedValue);
			iWriter.Write(new HalfSingle(this.Radius).PackedValue);
			iWriter.Write(new HalfSingle(this.HitPoints).PackedValue);
			iWriter.Write((byte)this.ShieldType);
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x000185E0 File Offset: 0x000167E0
		public void Read(BinaryReader iReader)
		{
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

		// Token: 0x04000329 RID: 809
		public ushort OwnerHandle;

		// Token: 0x0400032A RID: 810
		public Vector3 Position;

		// Token: 0x0400032B RID: 811
		public Vector3 Direction;

		// Token: 0x0400032C RID: 812
		public float Radius;

		// Token: 0x0400032D RID: 813
		public float HitPoints;

		// Token: 0x0400032E RID: 814
		public ShieldType ShieldType;
	}
}
