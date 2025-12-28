using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Magicka.Network
{
	// Token: 0x0200013D RID: 317
	[StructLayout(LayoutKind.Explicit)]
	internal struct CharacterActionMessage : ISendable
	{
		// Token: 0x170001CA RID: 458
		// (get) Token: 0x060008F3 RID: 2291 RVA: 0x00039224 File Offset: 0x00037424
		public PacketType PacketType
		{
			get
			{
				return PacketType.CharacterAction;
			}
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x00039228 File Offset: 0x00037428
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.Action);
			iWriter.Write(this.Handle);
			iWriter.Write(this.TimeStamp);
			byte b = (byte)(((this.TargetHandle == 0) ? 0 : 1) << 1 | ((this.Param0I == 0) ? 0 : 1) << 2 | ((this.Param1I == 0) ? 0 : 1) << 3 | ((this.Param2I == 0) ? 0 : 1) << 4 | ((this.Param3I == 0) ? 0 : 1) << 5 | ((this.Param4I == 0) ? 0 : 1) << 6);
			iWriter.Write(b);
			if ((b & 2) == 2)
			{
				iWriter.Write(this.TargetHandle);
			}
			if ((b & 4) == 4)
			{
				iWriter.Write(this.Param0I);
			}
			if ((b & 8) == 8)
			{
				iWriter.Write(this.Param1I);
			}
			if ((b & 16) == 16)
			{
				iWriter.Write(this.Param2I);
			}
			if ((b & 32) == 32)
			{
				iWriter.Write(this.Param3I);
			}
			if ((b & 64) == 64)
			{
				iWriter.Write(this.Param4I);
			}
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x00039330 File Offset: 0x00037530
		public void Read(BinaryReader iReader)
		{
			this.Action = (ActionType)iReader.ReadByte();
			this.Handle = iReader.ReadUInt16();
			this.TimeStamp = iReader.ReadDouble();
			byte b = iReader.ReadByte();
			if ((b & 2) == 2)
			{
				this.TargetHandle = iReader.ReadUInt16();
			}
			if ((b & 4) == 4)
			{
				this.Param0I = iReader.ReadInt32();
			}
			if ((b & 8) == 8)
			{
				this.Param1I = iReader.ReadInt32();
			}
			if ((b & 16) == 16)
			{
				this.Param2I = iReader.ReadInt32();
			}
			if ((b & 32) == 32)
			{
				this.Param3I = iReader.ReadInt32();
			}
			if ((b & 64) == 64)
			{
				this.Param4I = iReader.ReadInt32();
			}
		}

		// Token: 0x04000857 RID: 2135
		[FieldOffset(0)]
		public ActionType Action;

		// Token: 0x04000858 RID: 2136
		[FieldOffset(4)]
		public ushort Handle;

		// Token: 0x04000859 RID: 2137
		[FieldOffset(6)]
		public ushort TargetHandle;

		// Token: 0x0400085A RID: 2138
		[FieldOffset(8)]
		public double TimeStamp;

		// Token: 0x0400085B RID: 2139
		[FieldOffset(16)]
		public int Param0I;

		// Token: 0x0400085C RID: 2140
		[FieldOffset(16)]
		public float Param0F;

		// Token: 0x0400085D RID: 2141
		[FieldOffset(20)]
		public int Param1I;

		// Token: 0x0400085E RID: 2142
		[FieldOffset(20)]
		public float Param1F;

		// Token: 0x0400085F RID: 2143
		[FieldOffset(24)]
		public int Param2I;

		// Token: 0x04000860 RID: 2144
		[FieldOffset(24)]
		public float Param2F;

		// Token: 0x04000861 RID: 2145
		[FieldOffset(28)]
		public int Param3I;

		// Token: 0x04000862 RID: 2146
		[FieldOffset(28)]
		public float Param3F;

		// Token: 0x04000863 RID: 2147
		[FieldOffset(32)]
		public int Param4I;

		// Token: 0x04000864 RID: 2148
		[FieldOffset(32)]
		public float Param4F;
	}
}
