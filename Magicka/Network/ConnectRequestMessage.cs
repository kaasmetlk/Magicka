using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x0200020B RID: 523
	internal struct ConnectRequestMessage : ISendable
	{
		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x060010F5 RID: 4341 RVA: 0x00069E96 File Offset: 0x00068096
		public PacketType PacketType
		{
			get
			{
				return (PacketType)129;
			}
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x00069E9D File Offset: 0x0006809D
		public void Write(BinaryWriter iWriter)
		{
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x00069E9F File Offset: 0x0006809F
		public void Read(BinaryReader iReader)
		{
		}

		// Token: 0x04000FB3 RID: 4019
		private byte mDummy;
	}
}
