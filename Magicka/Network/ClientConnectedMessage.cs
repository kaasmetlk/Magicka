using System;
using System.IO;
using SteamWrapper;

namespace Magicka.Network
{
	// Token: 0x02000211 RID: 529
	internal struct ClientConnectedMessage : ISendable
	{
		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001104 RID: 4356 RVA: 0x0006A121 File Offset: 0x00068321
		public PacketType PacketType
		{
			get
			{
				return PacketType.ClientConnected;
			}
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0006A124 File Offset: 0x00068324
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.ID.AsUInt64);
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x0006A137 File Offset: 0x00068337
		public void Read(BinaryReader iReader)
		{
			this.ID = new SteamID(iReader.ReadUInt64());
		}

		// Token: 0x04000FC8 RID: 4040
		public SteamID ID;
	}
}
