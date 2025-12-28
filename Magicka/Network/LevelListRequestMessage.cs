using System;
using System.IO;
using Magicka.GameLogic.GameStates;

namespace Magicka.Network
{
	// Token: 0x0200021D RID: 541
	internal struct LevelListRequestMessage : ISendable
	{
		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001127 RID: 4391 RVA: 0x0006A71D File Offset: 0x0006891D
		public PacketType PacketType
		{
			get
			{
				return (PacketType)143;
			}
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x0006A724 File Offset: 0x00068924
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.GameType);
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x0006A732 File Offset: 0x00068932
		public void Read(BinaryReader iReader)
		{
			this.GameType = (GameType)iReader.ReadByte();
		}

		// Token: 0x04000FEE RID: 4078
		public GameType GameType;
	}
}
