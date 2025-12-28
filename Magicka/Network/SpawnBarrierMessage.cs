using System;
using System.IO;
using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x020000A2 RID: 162
	internal struct SpawnBarrierMessage : ISendable
	{
		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060004A3 RID: 1187 RVA: 0x00019896 File Offset: 0x00017A96
		public PacketType PacketType
		{
			get
			{
				return PacketType.SpawnBarrier;
			}
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001989C File Offset: 0x00017A9C
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.OwnerHandle);
			iWriter.Write(this.HitlistHandle);
			iWriter.Write(this.AnimationHandle);
			iWriter.Write(this.Position.X);
			iWriter.Write(this.Position.Y);
			iWriter.Write(this.Position.Z);
			iWriter.Write(new Normalized101010(this.Direction).PackedValue);
			iWriter.Write(new HalfSingle(this.Scale).PackedValue);
			iWriter.Write((ushort)this.Spell.Element);
			for (int i = 0; i < 11; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				if ((elements & this.Spell.Element) == elements)
				{
					iWriter.Write(new HalfSingle(this.Spell[elements]).PackedValue);
				}
			}
			iWriter.Write((short)this.Damage.A.AttackProperty);
			if (this.Damage.A.AttackProperty != (AttackProperties)0)
			{
				iWriter.Write((ushort)this.Damage.A.Element);
				iWriter.Write(this.Damage.A.Amount);
				iWriter.Write(new HalfSingle(this.Damage.A.Magnitude).PackedValue);
			}
			iWriter.Write((short)this.Damage.B.AttackProperty);
			if (this.Damage.B.AttackProperty != (AttackProperties)0)
			{
				iWriter.Write((ushort)this.Damage.B.Element);
				iWriter.Write(this.Damage.B.Amount);
				iWriter.Write(new HalfSingle(this.Damage.B.Magnitude).PackedValue);
			}
			iWriter.Write((short)this.Damage.C.AttackProperty);
			if (this.Damage.C.AttackProperty != (AttackProperties)0)
			{
				iWriter.Write((ushort)this.Damage.C.Element);
				iWriter.Write(this.Damage.C.Amount);
				iWriter.Write(new HalfSingle(this.Damage.C.Magnitude).PackedValue);
			}
			iWriter.Write((short)this.Damage.D.AttackProperty);
			if (this.Damage.D.AttackProperty != (AttackProperties)0)
			{
				iWriter.Write((ushort)this.Damage.D.Element);
				iWriter.Write(this.Damage.D.Amount);
				iWriter.Write(new HalfSingle(this.Damage.D.Magnitude).PackedValue);
			}
			iWriter.Write((short)this.Damage.E.AttackProperty);
			if (this.Damage.E.AttackProperty != (AttackProperties)0)
			{
				iWriter.Write((ushort)this.Damage.E.Element);
				iWriter.Write(this.Damage.E.Amount);
				iWriter.Write(new HalfSingle(this.Damage.E.Magnitude).PackedValue);
			}
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x00019BF4 File Offset: 0x00017DF4
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
			this.OwnerHandle = iReader.ReadUInt16();
			this.HitlistHandle = iReader.ReadUInt16();
			this.AnimationHandle = iReader.ReadUInt16();
			this.Position.X = iReader.ReadSingle();
			this.Position.Y = iReader.ReadSingle();
			this.Position.Z = iReader.ReadSingle();
			Normalized101010 normalized = default(Normalized101010);
			normalized.PackedValue = iReader.ReadUInt32();
			this.Direction = normalized.ToVector3();
			HalfSingle halfSingle = default(HalfSingle);
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Scale = halfSingle.ToSingle();
			this.Spell.Element = (Elements)iReader.ReadUInt16();
			for (int i = 0; i < (int)this.Spell.Element; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				if ((elements & this.Spell.Element) == elements)
				{
					halfSingle.PackedValue = iReader.ReadUInt16();
					this.Spell[elements] = halfSingle.ToSingle();
				}
			}
			this.Damage.A.AttackProperty = (AttackProperties)iReader.ReadInt16();
			if (this.Damage.A.AttackProperty != (AttackProperties)0)
			{
				this.Damage.A.Element = (Elements)iReader.ReadUInt16();
				this.Damage.A.Amount = iReader.ReadSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Damage.A.Magnitude = halfSingle.ToSingle();
			}
			this.Damage.B.AttackProperty = (AttackProperties)iReader.ReadInt16();
			if (this.Damage.B.AttackProperty != (AttackProperties)0)
			{
				this.Damage.B.Element = (Elements)iReader.ReadUInt16();
				this.Damage.B.Amount = iReader.ReadSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Damage.B.Magnitude = halfSingle.ToSingle();
			}
			this.Damage.C.AttackProperty = (AttackProperties)iReader.ReadInt16();
			if (this.Damage.C.AttackProperty != (AttackProperties)0)
			{
				this.Damage.C.Element = (Elements)iReader.ReadUInt16();
				this.Damage.C.Amount = iReader.ReadSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Damage.C.Magnitude = halfSingle.ToSingle();
			}
			this.Damage.D.AttackProperty = (AttackProperties)iReader.ReadInt16();
			if (this.Damage.D.AttackProperty != (AttackProperties)0)
			{
				this.Damage.D.Element = (Elements)iReader.ReadUInt16();
				this.Damage.D.Amount = iReader.ReadSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Damage.D.Magnitude = halfSingle.ToSingle();
			}
			this.Damage.E.AttackProperty = (AttackProperties)iReader.ReadInt16();
			if (this.Damage.E.AttackProperty != (AttackProperties)0)
			{
				this.Damage.E.Element = (Elements)iReader.ReadUInt16();
				this.Damage.E.Amount = iReader.ReadSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Damage.E.Magnitude = halfSingle.ToSingle();
			}
		}

		// Token: 0x0400034D RID: 845
		public ushort Handle;

		// Token: 0x0400034E RID: 846
		public ushort OwnerHandle;

		// Token: 0x0400034F RID: 847
		public ushort HitlistHandle;

		// Token: 0x04000350 RID: 848
		public ushort AnimationHandle;

		// Token: 0x04000351 RID: 849
		public Vector3 Position;

		// Token: 0x04000352 RID: 850
		public Vector3 Direction;

		// Token: 0x04000353 RID: 851
		public float Scale;

		// Token: 0x04000354 RID: 852
		public Spell Spell;

		// Token: 0x04000355 RID: 853
		public DamageCollection5 Damage;
	}
}
