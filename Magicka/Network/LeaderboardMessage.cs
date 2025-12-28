using System;
using System.IO;
using SteamWrapper;

namespace Magicka.Network
{
	// Token: 0x0200022E RID: 558
	internal struct LeaderboardMessage : ISendable
	{
		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x0600114B RID: 4427 RVA: 0x0006B7A4 File Offset: 0x000699A4
		public PacketType PacketType
		{
			get
			{
				return PacketType.LeaderboardEntry;
			}
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x0006B7A8 File Offset: 0x000699A8
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.SteamLeaderboard);
			iWriter.Write((byte)this.ScoreMethod);
			iWriter.Write(this.Score);
			iWriter.Write(this.Data);
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x0006B7DB File Offset: 0x000699DB
		public void Read(BinaryReader iReader)
		{
			this.SteamLeaderboard = iReader.ReadUInt64();
			this.ScoreMethod = (LeaderboardUploadScoreMethod)iReader.ReadByte();
			this.Score = iReader.ReadInt32();
			this.Data = iReader.ReadInt32();
		}

		// Token: 0x04001043 RID: 4163
		public ulong SteamLeaderboard;

		// Token: 0x04001044 RID: 4164
		public LeaderboardUploadScoreMethod ScoreMethod;

		// Token: 0x04001045 RID: 4165
		public int Score;

		// Token: 0x04001046 RID: 4166
		public int Data;
	}
}
