using System;
using System.IO;
using Magicka.GameLogic.GameStates;

namespace Magicka.Network
{
	// Token: 0x0200021C RID: 540
	internal struct GameRestartMessage : ISendable
	{
		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001124 RID: 4388 RVA: 0x0006A6FD File Offset: 0x000688FD
		public PacketType PacketType
		{
			get
			{
				return PacketType.GameRestart;
			}
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x0006A701 File Offset: 0x00068901
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.Type);
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x0006A70F File Offset: 0x0006890F
		public void Read(BinaryReader iReader)
		{
			this.Type = (RestartType)iReader.ReadByte();
		}

		// Token: 0x04000FED RID: 4077
		public RestartType Type;
	}
}
