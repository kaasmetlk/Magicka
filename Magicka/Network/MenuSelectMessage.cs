using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Magicka.Network
{
	// Token: 0x0200021E RID: 542
	[StructLayout(LayoutKind.Explicit)]
	internal struct MenuSelectMessage : ISendable
	{
		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x0600112A RID: 4394 RVA: 0x0006A740 File Offset: 0x00068940
		public PacketType PacketType
		{
			get
			{
				return PacketType.MenuSelection;
			}
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x0006A744 File Offset: 0x00068944
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Option);
			iWriter.Write((byte)this.IntendedMenu);
			iWriter.Write(this.Param0I);
			iWriter.Write(this.Param1I);
			iWriter.Write(this.Param2I);
			iWriter.Write(this.Param3I);
			iWriter.Write(this.Param4I);
			iWriter.Write(this.Param5I);
			iWriter.Write(this.Param6I);
			iWriter.Write(this.Param7I);
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x0006A7CC File Offset: 0x000689CC
		public void Read(BinaryReader iReader)
		{
			this.Option = iReader.ReadInt32();
			this.IntendedMenu = (MenuSelectMessage.MenuType)iReader.ReadByte();
			this.Param0I = iReader.ReadInt32();
			this.Param1I = iReader.ReadInt32();
			this.Param2I = iReader.ReadInt32();
			this.Param3I = iReader.ReadInt32();
			this.Param4I = iReader.ReadInt32();
			this.Param5I = iReader.ReadInt32();
			this.Param6I = iReader.ReadInt32();
			this.Param7I = iReader.ReadInt32();
		}

		// Token: 0x04000FEF RID: 4079
		[FieldOffset(0)]
		public int Option;

		// Token: 0x04000FF0 RID: 4080
		[FieldOffset(4)]
		public int Param0I;

		// Token: 0x04000FF1 RID: 4081
		[FieldOffset(4)]
		public float Param0F;

		// Token: 0x04000FF2 RID: 4082
		[FieldOffset(8)]
		public int Param1I;

		// Token: 0x04000FF3 RID: 4083
		[FieldOffset(8)]
		public float Param1F;

		// Token: 0x04000FF4 RID: 4084
		[FieldOffset(12)]
		public int Param2I;

		// Token: 0x04000FF5 RID: 4085
		[FieldOffset(12)]
		public float Param2F;

		// Token: 0x04000FF6 RID: 4086
		[FieldOffset(16)]
		public int Param3I;

		// Token: 0x04000FF7 RID: 4087
		[FieldOffset(16)]
		public float Param3F;

		// Token: 0x04000FF8 RID: 4088
		[FieldOffset(20)]
		public int Param4I;

		// Token: 0x04000FF9 RID: 4089
		[FieldOffset(20)]
		public float Param4F;

		// Token: 0x04000FFA RID: 4090
		[FieldOffset(24)]
		public int Param5I;

		// Token: 0x04000FFB RID: 4091
		[FieldOffset(24)]
		public float Param5F;

		// Token: 0x04000FFC RID: 4092
		[FieldOffset(28)]
		public int Param6I;

		// Token: 0x04000FFD RID: 4093
		[FieldOffset(28)]
		public float Param6F;

		// Token: 0x04000FFE RID: 4094
		[FieldOffset(32)]
		public int Param7I;

		// Token: 0x04000FFF RID: 4095
		[FieldOffset(32)]
		public float Param7F;

		// Token: 0x04001000 RID: 4096
		[FieldOffset(36)]
		public MenuSelectMessage.MenuType IntendedMenu;

		// Token: 0x0200021F RID: 543
		public enum MenuType : byte
		{
			// Token: 0x04001002 RID: 4098
			CharacterSelect,
			// Token: 0x04001003 RID: 4099
			Statistics
		}
	}
}
