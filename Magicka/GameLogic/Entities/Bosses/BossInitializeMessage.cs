using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Magicka.Network;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000538 RID: 1336
	public struct BossInitializeMessage : ISendable
	{
		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x060027C5 RID: 10181 RVA: 0x00122C77 File Offset: 0x00120E77
		public PacketType PacketType
		{
			get
			{
				return PacketType.BossInitialize;
			}
		}

		// Token: 0x060027C6 RID: 10182 RVA: 0x00122C7C File Offset: 0x00120E7C
		public unsafe void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Type);
			iWriter.Write((byte)this.BossID);
			iWriter.Write(this.Length);
			iWriter.Write(this.TimeStamp);
			fixed (byte* ptr = &this.Data.FixedElementField)
			{
				for (int i = 0; i < (int)this.Length; i++)
				{
					iWriter.Write(ptr[i]);
				}
			}
		}

		// Token: 0x060027C7 RID: 10183 RVA: 0x00122CE4 File Offset: 0x00120EE4
		public unsafe void Read(BinaryReader iReader)
		{
			this.Type = iReader.ReadUInt16();
			this.BossID = (BossEnum)iReader.ReadByte();
			this.Length = iReader.ReadUInt16();
			this.TimeStamp = iReader.ReadInt64();
			fixed (byte* ptr = &this.Data.FixedElementField)
			{
				for (int i = 0; i < (int)this.Length; i++)
				{
					ptr[i] = iReader.ReadByte();
				}
			}
		}

		// Token: 0x060027C8 RID: 10184 RVA: 0x00122D4C File Offset: 0x00120F4C
		public unsafe static void ConvertTo(ref BossInitializeMessage iMsg, void* oValue)
		{
			fixed (byte* ptr = &iMsg.Data.FixedElementField)
			{
				for (int i = 0; i < (int)iMsg.Length; i++)
				{
					((byte*)oValue)[i] = ptr[i];
				}
			}
		}

		// Token: 0x060027C9 RID: 10185 RVA: 0x00122D84 File Offset: 0x00120F84
		public unsafe static void ConvertFrom<T>(ushort iType, void* iValue, out BossInitializeMessage oMsg) where T : struct
		{
			oMsg.Type = iType;
			oMsg.TimeStamp = DateTime.Now.Ticks;
			oMsg.Length = (ushort)Marshal.SizeOf(typeof(T));
			oMsg.BossID = (BossEnum)255;
			fixed (byte* ptr = &oMsg.Data.FixedElementField)
			{
				for (int i = 0; i < (int)oMsg.Length; i++)
				{
					ptr[i] = ((byte*)iValue)[i];
				}
			}
		}

		// Token: 0x04002B62 RID: 11106
		public const int MAX_SIZE = 1024;

		// Token: 0x04002B63 RID: 11107
		public ushort Type;

		// Token: 0x04002B64 RID: 11108
		public BossEnum BossID;

		// Token: 0x04002B65 RID: 11109
		public ushort Length;

		// Token: 0x04002B66 RID: 11110
		public long TimeStamp;

		// Token: 0x04002B67 RID: 11111
		[FixedBuffer(typeof(byte), 1024)]
		public BossInitializeMessage.<Data>e__FixedBufferb Data;

		// Token: 0x02000539 RID: 1337
		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 1024)]
		public struct <Data>e__FixedBufferb
		{
			// Token: 0x04002B68 RID: 11112
			public byte FixedElementField;
		}
	}
}
