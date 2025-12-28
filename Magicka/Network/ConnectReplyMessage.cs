using System;
using System.IO;
using SteamWrapper;

namespace Magicka.Network
{
	// Token: 0x0200020C RID: 524
	internal struct ConnectReplyMessage : ISendable
	{
		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x060010F8 RID: 4344 RVA: 0x00069EA1 File Offset: 0x000680A1
		public PacketType PacketType
		{
			get
			{
				return PacketType.Connect;
			}
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00069EA4 File Offset: 0x000680A4
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.ServerID.AsUInt64);
			iWriter.Write(this.VACSecure);
			iWriter.Write(this.ServerName);
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x00069ECF File Offset: 0x000680CF
		public void Read(BinaryReader iReader)
		{
			this.ServerID = new SteamID(iReader.ReadUInt64());
			this.VACSecure = iReader.ReadBoolean();
			this.ServerName = iReader.ReadString();
		}

		// Token: 0x04000FB4 RID: 4020
		public SteamID ServerID;

		// Token: 0x04000FB5 RID: 4021
		public bool VACSecure;

		// Token: 0x04000FB6 RID: 4022
		public string ServerName;
	}
}
