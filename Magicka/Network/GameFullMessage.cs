using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000214 RID: 532
	internal struct GameFullMessage : ISendable
	{
		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x0600110A RID: 4362 RVA: 0x0006A169 File Offset: 0x00068369
		public PacketType PacketType
		{
			get
			{
				return PacketType.GameFull;
			}
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x0006A16C File Offset: 0x0006836C
		public void Write(BinaryWriter iWriter)
		{
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0006A16E File Offset: 0x0006836E
		public void Read(BinaryReader iReader)
		{
		}

		// Token: 0x04000FCF RID: 4047
		private byte mDummy;
	}
}
