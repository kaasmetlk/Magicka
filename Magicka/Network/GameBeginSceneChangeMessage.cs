using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000220 RID: 544
	internal struct GameBeginSceneChangeMessage : ISendable
	{
		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x0600112D RID: 4397 RVA: 0x0006A851 File Offset: 0x00068A51
		public PacketType PacketType
		{
			get
			{
				return PacketType.GameBeginSceneChange;
			}
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x0006A855 File Offset: 0x00068A55
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Scene);
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x0006A863 File Offset: 0x00068A63
		public void Read(BinaryReader iReader)
		{
			this.Scene = iReader.ReadString();
		}

		// Token: 0x04001004 RID: 4100
		public string Scene;
	}
}
