using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000210 RID: 528
	internal struct LobbyInfoMessage : ISendable
	{
		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001101 RID: 4353 RVA: 0x0006A102 File Offset: 0x00068302
		public PacketType PacketType
		{
			get
			{
				return PacketType.LobbyInfo;
			}
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x0006A105 File Offset: 0x00068305
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.SteamID);
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x0006A113 File Offset: 0x00068313
		public void Read(BinaryReader iReader)
		{
			this.SteamID = iReader.ReadUInt64();
		}

		// Token: 0x04000FC7 RID: 4039
		public ulong SteamID;
	}
}
