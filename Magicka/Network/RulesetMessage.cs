using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Magicka.Levels;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x0200022A RID: 554
	public struct RulesetMessage : ISendable
	{
		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06001145 RID: 4421 RVA: 0x0006B343 File Offset: 0x00069543
		public PacketType PacketType
		{
			get
			{
				return PacketType.RulesetUpdate;
			}
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x0006B348 File Offset: 0x00069548
		public unsafe void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.Type);
			switch (this.Type)
			{
			case Rulesets.Survival:
				iWriter.Write(new HalfSingle(this.PackedFloat).PackedValue);
				iWriter.Write(this.Float01);
				iWriter.Write(this.Float02);
				iWriter.Write(this.Byte01);
				iWriter.Write(this.Byte02);
				iWriter.Write(this.Byte03);
				return;
			case Rulesets.TimedObjective:
				iWriter.Write(this.NrOfByteItems);
				fixed (byte* ptr = &this.Byte.FixedElementField)
				{
					for (int i = 0; i < (int)this.NrOfByteItems; i++)
					{
						iWriter.Write(ptr[i]);
					}
				}
				return;
			case Rulesets.DeathMatch:
			case Rulesets.Brawl:
			case Rulesets.Pyrite:
			case Rulesets.Kreitor:
			case Rulesets.King:
				iWriter.Write(this.Byte01);
				switch (this.Byte01)
				{
				case 0:
				case 2:
					iWriter.Write(this.Float01);
					return;
				case 1:
					break;
				case 3:
					iWriter.Write(this.Byte02);
					iWriter.Write(this.Integer01);
					iWriter.Write(this.UShort01);
					return;
				case 4:
					iWriter.Write(this.Byte02);
					iWriter.Write(this.NrOfShortItems);
					fixed (short* ptr2 = &this.Scores.FixedElementField)
					{
						for (int j = 0; j < (int)this.NrOfShortItems; j++)
						{
							iWriter.Write(ptr2[j]);
						}
					}
					break;
				default:
					return;
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x0006B4C8 File Offset: 0x000696C8
		public unsafe void Read(BinaryReader iReader)
		{
			this.Type = (Rulesets)iReader.ReadByte();
			switch (this.Type)
			{
			case Rulesets.Survival:
			{
				HalfSingle halfSingle = default(HalfSingle);
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.PackedFloat = halfSingle.ToSingle();
				this.Float01 = iReader.ReadSingle();
				this.Float02 = iReader.ReadSingle();
				this.Byte01 = iReader.ReadByte();
				this.Byte02 = iReader.ReadByte();
				this.Byte03 = iReader.ReadByte();
				return;
			}
			case Rulesets.TimedObjective:
				this.NrOfByteItems = iReader.ReadByte();
				fixed (byte* ptr = &this.Byte.FixedElementField)
				{
					for (int i = 0; i < (int)this.NrOfByteItems; i++)
					{
						ptr[i] = iReader.ReadByte();
					}
				}
				return;
			case Rulesets.DeathMatch:
			case Rulesets.Brawl:
			case Rulesets.Pyrite:
			case Rulesets.Kreitor:
			case Rulesets.King:
				this.Byte01 = iReader.ReadByte();
				switch (this.Byte01)
				{
				case 0:
				case 2:
					this.Float01 = iReader.ReadSingle();
					return;
				case 1:
					break;
				case 3:
					this.Byte02 = iReader.ReadByte();
					this.Integer01 = iReader.ReadInt32();
					this.UShort01 = iReader.ReadUInt16();
					return;
				case 4:
					this.Byte02 = iReader.ReadByte();
					this.NrOfShortItems = iReader.ReadByte();
					fixed (short* ptr2 = &this.Scores.FixedElementField)
					{
						for (int j = 0; j < (int)this.NrOfShortItems; j++)
						{
							ptr2[j] = iReader.ReadInt16();
						}
					}
					break;
				default:
					return;
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x0400102C RID: 4140
		public const byte BYTE_ARRAY_MAX_SIZE = 16;

		// Token: 0x0400102D RID: 4141
		public const byte SHROT_ARRAY_MAX_SIZE = 6;

		// Token: 0x0400102E RID: 4142
		public Rulesets Type;

		// Token: 0x0400102F RID: 4143
		public float PackedFloat;

		// Token: 0x04001030 RID: 4144
		public byte Byte01;

		// Token: 0x04001031 RID: 4145
		public byte Byte02;

		// Token: 0x04001032 RID: 4146
		public byte Byte03;

		// Token: 0x04001033 RID: 4147
		public float Float01;

		// Token: 0x04001034 RID: 4148
		public float Float02;

		// Token: 0x04001035 RID: 4149
		public ushort UShort01;

		// Token: 0x04001036 RID: 4150
		public int Integer01;

		// Token: 0x04001037 RID: 4151
		public byte NrOfByteItems;

		// Token: 0x04001038 RID: 4152
		[FixedBuffer(typeof(byte), 16)]
		public RulesetMessage.<Byte>e__FixedBuffer5 Byte;

		// Token: 0x04001039 RID: 4153
		public byte NrOfShortItems;

		// Token: 0x0400103A RID: 4154
		[FixedBuffer(typeof(short), 6)]
		public RulesetMessage.<Scores>e__FixedBuffer6 Scores;

		// Token: 0x0200022B RID: 555
		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <Byte>e__FixedBuffer5
		{
			// Token: 0x0400103B RID: 4155
			public byte FixedElementField;
		}

		// Token: 0x0200022C RID: 556
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 12)]
		public struct <Scores>e__FixedBuffer6
		{
			// Token: 0x0400103C RID: 4156
			public short FixedElementField;
		}
	}
}
