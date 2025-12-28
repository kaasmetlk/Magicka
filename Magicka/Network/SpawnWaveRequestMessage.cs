using System;
using System.IO;
using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x020000A1 RID: 161
	internal struct SpawnWaveRequestMessage : ISendable
	{
		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x00019052 File Offset: 0x00017252
		public PacketType PacketType
		{
			get
			{
				return (PacketType)168;
			}
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0001905C File Offset: 0x0001725C
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.OwnerHandle);
			iWriter.Write(this.AnimationHandle);
			iWriter.Write(this.Position.X);
			iWriter.Write(this.Position.Y);
			iWriter.Write(this.Position.Z);
			iWriter.Write(new Normalized101010(this.Direction).PackedValue);
			iWriter.Write(new HalfSingle(this.Scale).PackedValue);
			iWriter.Write(new HalfSingle(this.Range).PackedValue);
			iWriter.Write(new HalfSingle(this.NextDir.X).PackedValue);
			iWriter.Write(new HalfSingle(this.NextDir.Y).PackedValue);
			iWriter.Write(new HalfSingle(this.NextDir.Z).PackedValue);
			iWriter.Write(new Normalized101010(this.NextRotation.X, this.NextRotation.Y, this.NextRotation.Z).PackedValue);
			iWriter.Write(new HalfSingle(this.NextRotation.W).PackedValue);
			iWriter.Write(new HalfSingle(this.Distance).PackedValue);
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

		// Token: 0x060004A2 RID: 1186 RVA: 0x00019480 File Offset: 0x00017680
		public void Read(BinaryReader iReader)
		{
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
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Range = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.NextDir.X = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.NextDir.Y = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.NextDir.Z = halfSingle.ToSingle();
			normalized.PackedValue = iReader.ReadUInt32();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.NextRotation = new Quaternion(normalized.ToVector3(), halfSingle.ToSingle());
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Distance = halfSingle.ToSingle();
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

		// Token: 0x04000341 RID: 833
		public ushort OwnerHandle;

		// Token: 0x04000342 RID: 834
		public ushort AnimationHandle;

		// Token: 0x04000343 RID: 835
		public ushort ParentHandle;

		// Token: 0x04000344 RID: 836
		public Vector3 Position;

		// Token: 0x04000345 RID: 837
		public Vector3 Direction;

		// Token: 0x04000346 RID: 838
		public float Scale;

		// Token: 0x04000347 RID: 839
		public float Range;

		// Token: 0x04000348 RID: 840
		public Vector3 NextDir;

		// Token: 0x04000349 RID: 841
		public Quaternion NextRotation;

		// Token: 0x0400034A RID: 842
		public float Distance;

		// Token: 0x0400034B RID: 843
		public Spell Spell;

		// Token: 0x0400034C RID: 844
		public DamageCollection5 Damage;
	}
}
