using System;
using System.IO;
using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x020000A3 RID: 163
	internal struct SpawnWaveMessage : ISendable
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060004A6 RID: 1190 RVA: 0x00019F5E File Offset: 0x0001815E
		public PacketType PacketType
		{
			get
			{
				return PacketType.SpawnWave;
			}
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00019F64 File Offset: 0x00018164
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.OwnerHandle);
			iWriter.Write(this.HitlistHandle);
			iWriter.Write(this.AnimationHandle);
			iWriter.Write(this.ParentHandle);
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

		// Token: 0x060004A8 RID: 1192 RVA: 0x0001A2C8 File Offset: 0x000184C8
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
			this.OwnerHandle = iReader.ReadUInt16();
			this.HitlistHandle = iReader.ReadUInt16();
			this.AnimationHandle = iReader.ReadUInt16();
			this.ParentHandle = iReader.ReadUInt16();
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

		// Token: 0x04000356 RID: 854
		public ushort Handle;

		// Token: 0x04000357 RID: 855
		public ushort OwnerHandle;

		// Token: 0x04000358 RID: 856
		public ushort HitlistHandle;

		// Token: 0x04000359 RID: 857
		public ushort AnimationHandle;

		// Token: 0x0400035A RID: 858
		public ushort ParentHandle;

		// Token: 0x0400035B RID: 859
		public Vector3 Position;

		// Token: 0x0400035C RID: 860
		public Vector3 Direction;

		// Token: 0x0400035D RID: 861
		public float Scale;

		// Token: 0x0400035E RID: 862
		public Spell Spell;

		// Token: 0x0400035F RID: 863
		public DamageCollection5 Damage;
	}
}
