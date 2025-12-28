using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000235 RID: 565
	internal struct ForceSyncPlayerStatusesMessage : ISendable
	{
		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06001160 RID: 4448 RVA: 0x0006B991 File Offset: 0x00069B91
		public PacketType PacketType
		{
			get
			{
				return PacketType.ForceSyncPlayersMessage;
			}
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x0006B998 File Offset: 0x00069B98
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.numPlayers);
			for (int i = 0; i < (int)this.numPlayers; i++)
			{
				this.playerUpdateMessages[i].Write(iWriter);
			}
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x0006B9D4 File Offset: 0x00069BD4
		public void Read(BinaryReader iReader)
		{
			this.numPlayers = iReader.ReadInt16();
			this.playerUpdateMessages = new EntityUpdateMessage[(int)this.numPlayers];
			for (int i = 0; i < (int)this.numPlayers; i++)
			{
				this.playerUpdateMessages[i].Read(iReader);
			}
		}

		// Token: 0x04001054 RID: 4180
		public short numPlayers;

		// Token: 0x04001055 RID: 4181
		public EntityUpdateMessage[] playerUpdateMessages;
	}
}
