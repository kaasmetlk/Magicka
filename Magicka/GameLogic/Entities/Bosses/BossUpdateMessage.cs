using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Magicka.Network;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000536 RID: 1334
	public struct BossUpdateMessage : ISendable
	{
		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x060027C0 RID: 10176 RVA: 0x00122AF7 File Offset: 0x00120CF7
		public PacketType PacketType
		{
			get
			{
				return PacketType.BossUpdate;
			}
		}

		// Token: 0x060027C1 RID: 10177 RVA: 0x00122AFC File Offset: 0x00120CFC
		public unsafe void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.BossID);
			iWriter.Write(this.Type);
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

		// Token: 0x060027C2 RID: 10178 RVA: 0x00122B64 File Offset: 0x00120D64
		public unsafe void Read(BinaryReader iReader)
		{
			this.BossID = (BossEnum)iReader.ReadByte();
			this.Type = iReader.ReadUInt16();
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

		// Token: 0x060027C3 RID: 10179 RVA: 0x00122BCC File Offset: 0x00120DCC
		public unsafe static void ConvertTo(ref BossUpdateMessage iMsg, void* oValue)
		{
			fixed (byte* ptr = &iMsg.Data.FixedElementField)
			{
				for (int i = 0; i < (int)iMsg.Length; i++)
				{
					((byte*)oValue)[i] = ptr[i];
				}
			}
		}

		// Token: 0x060027C4 RID: 10180 RVA: 0x00122C04 File Offset: 0x00120E04
		public unsafe static void ConvertFrom<T>(ushort iType, void* iValue, out BossUpdateMessage oMsg) where T : struct
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

		// Token: 0x04002B5B RID: 11099
		public const int MAX_SIZE = 1024;

		// Token: 0x04002B5C RID: 11100
		public BossEnum BossID;

		// Token: 0x04002B5D RID: 11101
		public ushort Type;

		// Token: 0x04002B5E RID: 11102
		public ushort Length;

		// Token: 0x04002B5F RID: 11103
		public long TimeStamp;

		// Token: 0x04002B60 RID: 11104
		[FixedBuffer(typeof(byte), 1024)]
		public BossUpdateMessage.<Data>e__FixedBuffera Data;

		// Token: 0x02000537 RID: 1335
		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 1024)]
		public struct <Data>e__FixedBuffera
		{
			// Token: 0x04002B61 RID: 11105
			public byte FixedElementField;
		}
	}
}
