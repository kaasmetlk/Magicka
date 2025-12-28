using System;
using System.IO;
using Magicka.GameLogic;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x0200022D RID: 557
	internal struct StatisticsMessage : ISendable
	{
		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06001148 RID: 4424 RVA: 0x0006B653 File Offset: 0x00069853
		public PacketType PacketType
		{
			get
			{
				return PacketType.StatisticsUpdate;
			}
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x0006B658 File Offset: 0x00069858
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.AttackerHandle);
			iWriter.Write(this.TargetHandle);
			iWriter.Write(this.TimeStamp);
			iWriter.Write((int)this.Result);
			iWriter.Write((short)this.Damage.AttackProperty);
			iWriter.Write((ushort)this.Damage.Element);
			iWriter.Write(this.Damage.Amount);
			iWriter.Write(new HalfSingle(this.Damage.Magnitude).PackedValue);
			iWriter.Write(this.Multiplier);
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x0006B6F4 File Offset: 0x000698F4
		public void Read(BinaryReader iReader)
		{
			this.AttackerHandle = iReader.ReadUInt16();
			this.TargetHandle = iReader.ReadUInt16();
			this.TimeStamp = (double)iReader.ReadInt64();
			this.Result = (DamageResult)iReader.ReadInt32();
			this.Damage = default(Damage);
			this.Damage.AttackProperty = (AttackProperties)iReader.ReadInt16();
			this.Damage.Element = (Elements)iReader.ReadUInt16();
			this.Damage.Amount = iReader.ReadSingle();
			HalfSingle halfSingle = default(HalfSingle);
			halfSingle.PackedValue = iReader.ReadUInt16();
			this.Damage.Magnitude = halfSingle.ToSingle();
			this.Multiplier = iReader.ReadByte();
		}

		// Token: 0x0400103D RID: 4157
		public ushort AttackerHandle;

		// Token: 0x0400103E RID: 4158
		public ushort TargetHandle;

		// Token: 0x0400103F RID: 4159
		public double TimeStamp;

		// Token: 0x04001040 RID: 4160
		public DamageResult Result;

		// Token: 0x04001041 RID: 4161
		public Damage Damage;

		// Token: 0x04001042 RID: 4162
		public byte Multiplier;
	}
}
