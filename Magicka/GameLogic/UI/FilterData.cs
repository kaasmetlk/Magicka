using System;
using System.IO;
using Magicka.GameLogic.GameStates;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000298 RID: 664
	public struct FilterData
	{
		// Token: 0x060013B6 RID: 5046 RVA: 0x00078988 File Offset: 0x00076B88
		public static void Write(BinaryWriter iWriter, FilterData iData)
		{
			iWriter.Write(iData.FreeSlots);
			iWriter.Write((byte)iData.Scope);
			iWriter.Write((byte)iData.GameType);
			iWriter.Write(iData.MaxLatency);
			iWriter.Write(iData.VACOnly);
			iWriter.Write(iData.FilterPlaying);
			iWriter.Write(iData.FilterPassword);
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x000789F4 File Offset: 0x00076BF4
		public static void Read1410(BinaryReader iReader, out FilterData oData)
		{
			oData.FreeSlots = iReader.ReadByte();
			oData.Scope = (Scope)iReader.ReadByte();
			oData.GameType = (GameType)iReader.ReadByte();
			oData.MaxLatency = iReader.ReadInt16();
			oData.VACOnly = iReader.ReadBoolean();
			oData.FilterPlaying = iReader.ReadBoolean();
			oData.FilterPassword = iReader.ReadBoolean();
		}

		// Token: 0x060013B8 RID: 5048 RVA: 0x00078A58 File Offset: 0x00076C58
		public static void Read1400(BinaryReader iReader, out FilterData oData)
		{
			oData.FreeSlots = iReader.ReadByte();
			oData.Scope = (Scope)iReader.ReadByte();
			oData.GameType = (GameType)iReader.ReadByte();
			oData.MaxLatency = iReader.ReadInt16();
			oData.VACOnly = (iReader.ReadByte() == 1);
			oData.FilterPlaying = false;
			oData.FilterPassword = false;
		}

		// Token: 0x04001533 RID: 5427
		public const byte MIN_SLOTS = 1;

		// Token: 0x04001534 RID: 5428
		public const byte MAX_SLOTS = 3;

		// Token: 0x04001535 RID: 5429
		public byte FreeSlots;

		// Token: 0x04001536 RID: 5430
		public Scope Scope;

		// Token: 0x04001537 RID: 5431
		public GameType GameType;

		// Token: 0x04001538 RID: 5432
		public short MaxLatency;

		// Token: 0x04001539 RID: 5433
		public bool VACOnly;

		// Token: 0x0400153A RID: 5434
		public bool FilterPlaying;

		// Token: 0x0400153B RID: 5435
		public bool FilterPassword;
	}
}
