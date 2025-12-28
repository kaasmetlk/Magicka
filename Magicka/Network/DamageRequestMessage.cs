using System;
using System.IO;
using Magicka.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x02000223 RID: 547
	internal struct DamageRequestMessage : ISendable
	{
		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001136 RID: 4406 RVA: 0x0006A8E1 File Offset: 0x00068AE1
		public PacketType PacketType
		{
			get
			{
				return (PacketType)172;
			}
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x0006A8E8 File Offset: 0x00068AE8
		public unsafe void Write(BinaryWriter iWriter)
		{
			fixed (Damage* ptr = &this.Damage.A)
			{
				for (int i = 0; i < 5; i++)
				{
					iWriter.Write((short)ptr[i].AttackProperty);
					if (ptr[i].AttackProperty != (AttackProperties)0)
					{
						iWriter.Write((ushort)ptr[i].Element);
						iWriter.Write(ptr[i].Amount);
						iWriter.Write(new HalfSingle(ptr[i].Magnitude).PackedValue);
					}
				}
			}
			iWriter.Write(this.AttackerHandle);
			iWriter.Write(this.TimeStamp);
			iWriter.Write(this.TargetHandle);
			iWriter.Write(new HalfSingle(this.RelativeAttackPosition.X).PackedValue);
			iWriter.Write(new HalfSingle(this.RelativeAttackPosition.Y).PackedValue);
			iWriter.Write(new HalfSingle(this.RelativeAttackPosition.Z).PackedValue);
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x0006AA14 File Offset: 0x00068C14
		public unsafe void Read(BinaryReader iReader)
		{
			HalfSingle halfSingle = default(HalfSingle);
			fixed (Damage* ptr = &this.Damage.A)
			{
				for (int i = 0; i < 5; i++)
				{
					ptr[i].AttackProperty = (AttackProperties)iReader.ReadInt16();
					if (ptr[i].AttackProperty != (AttackProperties)0)
					{
						ptr[i].Element = (Elements)iReader.ReadUInt16();
						ptr[i].Amount = iReader.ReadSingle();
						halfSingle.PackedValue = iReader.ReadUInt16();
						ptr[i].Magnitude = halfSingle.ToSingle();
					}
				}
			}
			this.AttackerHandle = iReader.ReadUInt16();
			this.TimeStamp = iReader.ReadDouble();
			this.TargetHandle = iReader.ReadUInt16();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.RelativeAttackPosition.X = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.RelativeAttackPosition.Y = halfSingle.ToSingle();
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.RelativeAttackPosition.Z = halfSingle.ToSingle();
		}

		// Token: 0x0400100A RID: 4106
		public DamageCollection5 Damage;

		// Token: 0x0400100B RID: 4107
		public ushort AttackerHandle;

		// Token: 0x0400100C RID: 4108
		public ushort TargetHandle;

		// Token: 0x0400100D RID: 4109
		public Vector3 RelativeAttackPosition;

		// Token: 0x0400100E RID: 4110
		public double TimeStamp;
	}
}
