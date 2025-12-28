using System;
using System.IO;
using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x020000A4 RID: 164
	internal struct SpawnMineRequestMessage : ISendable
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060004A9 RID: 1193 RVA: 0x0001A63E File Offset: 0x0001883E
		public PacketType PacketType
		{
			get
			{
				return (PacketType)169;
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x0001A648 File Offset: 0x00018848
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

		// Token: 0x060004AB RID: 1195 RVA: 0x0001AA6C File Offset: 0x00018C6C
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

		// Token: 0x04000360 RID: 864
		public ushort OwnerHandle;

		// Token: 0x04000361 RID: 865
		public ushort AnimationHandle;

		// Token: 0x04000362 RID: 866
		public Vector3 Position;

		// Token: 0x04000363 RID: 867
		public Vector3 Direction;

		// Token: 0x04000364 RID: 868
		public float Scale;

		// Token: 0x04000365 RID: 869
		public float Range;

		// Token: 0x04000366 RID: 870
		public Vector3 NextDir;

		// Token: 0x04000367 RID: 871
		public Quaternion NextRotation;

		// Token: 0x04000368 RID: 872
		public float Distance;

		// Token: 0x04000369 RID: 873
		public Spell Spell;

		// Token: 0x0400036A RID: 874
		public DamageCollection5 Damage;
	}
}
