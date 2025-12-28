using System;
using System.IO;
using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x020000A5 RID: 165
	internal struct SpawnMineMessage : ISendable
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060004AC RID: 1196 RVA: 0x0001AE82 File Offset: 0x00019082
		public PacketType PacketType
		{
			get
			{
				return PacketType.SpawnMine;
			}
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x0001AE88 File Offset: 0x00019088
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.OwnerHandle);
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

		// Token: 0x060004AE RID: 1198 RVA: 0x0001B1D4 File Offset: 0x000193D4
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
			this.OwnerHandle = iReader.ReadUInt16();
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

		// Token: 0x0400036B RID: 875
		public ushort Handle;

		// Token: 0x0400036C RID: 876
		public ushort OwnerHandle;

		// Token: 0x0400036D RID: 877
		public ushort AnimationHandle;

		// Token: 0x0400036E RID: 878
		public Vector3 Position;

		// Token: 0x0400036F RID: 879
		public Vector3 Direction;

		// Token: 0x04000370 RID: 880
		public float Scale;

		// Token: 0x04000371 RID: 881
		public Spell Spell;

		// Token: 0x04000372 RID: 882
		public DamageCollection5 Damage;
	}
}
