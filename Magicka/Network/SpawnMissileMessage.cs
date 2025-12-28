using System;
using System.IO;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x0200009C RID: 156
	internal struct SpawnMissileMessage : ISendable
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x00018046 File Offset: 0x00016246
		public PacketType PacketType
		{
			get
			{
				return PacketType.SpawnMissile;
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x0001804C File Offset: 0x0001624C
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.Type);
			iWriter.Write(this.Handle);
			iWriter.Write(this.Item);
			iWriter.Write(this.Owner);
			iWriter.Write(this.Target);
			iWriter.Write(this.Position.X);
			iWriter.Write(this.Position.Y);
			iWriter.Write(this.Position.Z);
			iWriter.Write(new HalfSingle(this.Velocity.X).PackedValue);
			iWriter.Write(new HalfSingle(this.Velocity.Y).PackedValue);
			iWriter.Write(new HalfSingle(this.Velocity.Z).PackedValue);
			iWriter.Write(new HalfSingle(this.Homing).PackedValue);
			switch (this.Type)
			{
			case SpawnMissileMessage.MissileType.Spell:
				iWriter.Write((ushort)this.Spell.Element);
				for (int i = 0; i < 11; i++)
				{
					Elements elements = Defines.ElementFromIndex(i);
					if ((elements & this.Spell.Element) == elements)
					{
						iWriter.Write(new HalfSingle(this.Spell[elements]).PackedValue);
					}
				}
				iWriter.Write(new HalfSingle(this.Splash).PackedValue);
				return;
			case SpawnMissileMessage.MissileType.Item:
			case SpawnMissileMessage.MissileType.HolyGrenade:
			case SpawnMissileMessage.MissileType.Grenade:
			case SpawnMissileMessage.MissileType.FireFlask:
			case SpawnMissileMessage.MissileType.JudgementMissile:
			case SpawnMissileMessage.MissileType.GreaseLump:
			case SpawnMissileMessage.MissileType.PotionFlask:
				return;
			case SpawnMissileMessage.MissileType.ProppMagick:
				iWriter.Write(new HalfSingle(this.AngularVelocity.X).PackedValue);
				iWriter.Write(new HalfSingle(this.AngularVelocity.Y).PackedValue);
				iWriter.Write(new HalfSingle(this.AngularVelocity.Z).PackedValue);
				iWriter.Write(new HalfSingle(this.Lever.X).PackedValue);
				iWriter.Write(new HalfSingle(this.Lever.Y).PackedValue);
				iWriter.Write(new HalfSingle(this.Lever.Z).PackedValue);
				return;
			}
			throw new Exception("Invalid missile type!");
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x000182B4 File Offset: 0x000164B4
		public void Read(BinaryReader iReader)
		{
			this.Type = (SpawnMissileMessage.MissileType)iReader.ReadByte();
			this.Handle = iReader.ReadUInt16();
			this.Item = iReader.ReadUInt16();
			this.Owner = iReader.ReadUInt16();
			this.Target = iReader.ReadUInt16();
			this.Position.X = iReader.ReadSingle();
			this.Position.Y = iReader.ReadSingle();
			this.Position.Z = iReader.ReadSingle();
			HalfSingle halfSingle = default(HalfSingle);
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Velocity.X = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Velocity.Y = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Velocity.Z = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Homing = halfSingle.ToSingle();
			switch (this.Type)
			{
			case SpawnMissileMessage.MissileType.Spell:
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
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Splash = halfSingle.ToSingle();
				return;
			case SpawnMissileMessage.MissileType.Item:
			case SpawnMissileMessage.MissileType.HolyGrenade:
			case SpawnMissileMessage.MissileType.Grenade:
			case SpawnMissileMessage.MissileType.FireFlask:
			case SpawnMissileMessage.MissileType.JudgementMissile:
			case SpawnMissileMessage.MissileType.GreaseLump:
			case SpawnMissileMessage.MissileType.PotionFlask:
				return;
			case SpawnMissileMessage.MissileType.ProppMagick:
				halfSingle = default(HalfSingle);
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.AngularVelocity.X = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.AngularVelocity.Y = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.AngularVelocity.Z = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Lever.X = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Lever.Y = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Lever.Z = halfSingle.ToSingle();
				return;
			}
			throw new Exception("Invalid missile type!");
		}

		// Token: 0x04000311 RID: 785
		public SpawnMissileMessage.MissileType Type;

		// Token: 0x04000312 RID: 786
		public ushort Handle;

		// Token: 0x04000313 RID: 787
		public ushort Item;

		// Token: 0x04000314 RID: 788
		public ushort Owner;

		// Token: 0x04000315 RID: 789
		public ushort Target;

		// Token: 0x04000316 RID: 790
		public Vector3 Position;

		// Token: 0x04000317 RID: 791
		public Vector3 Velocity;

		// Token: 0x04000318 RID: 792
		public Spell Spell;

		// Token: 0x04000319 RID: 793
		public float Splash;

		// Token: 0x0400031A RID: 794
		public float Homing;

		// Token: 0x0400031B RID: 795
		public Vector3 AngularVelocity;

		// Token: 0x0400031C RID: 796
		public Vector3 Lever;

		// Token: 0x0200009D RID: 157
		public enum MissileType : byte
		{
			// Token: 0x0400031E RID: 798
			Invalid,
			// Token: 0x0400031F RID: 799
			Spell,
			// Token: 0x04000320 RID: 800
			Item,
			// Token: 0x04000321 RID: 801
			HolyGrenade,
			// Token: 0x04000322 RID: 802
			Grenade,
			// Token: 0x04000323 RID: 803
			CthulhuMissile,
			// Token: 0x04000324 RID: 804
			FireFlask,
			// Token: 0x04000325 RID: 805
			ProppMagick,
			// Token: 0x04000326 RID: 806
			JudgementMissile,
			// Token: 0x04000327 RID: 807
			GreaseLump,
			// Token: 0x04000328 RID: 808
			PotionFlask
		}
	}
}
