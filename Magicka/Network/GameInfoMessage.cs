using System;
using System.IO;
using Magicka.GameLogic.GameStates;
using Magicka.Levels.Campaign;

namespace Magicka.Network
{
	// Token: 0x02000215 RID: 533
	internal struct GameInfoMessage : ISendable
	{
		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x0600110D RID: 4365 RVA: 0x0006A170 File Offset: 0x00068370
		public PacketType PacketType
		{
			get
			{
				return PacketType.GameInfo;
			}
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x0006A174 File Offset: 0x00068374
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.GameName);
			iWriter.Write(this.NrOfPlayers);
			iWriter.Write((byte)this.GameType);
			int num = this.Level;
			if (num == -1)
			{
				num = 0;
			}
			GameType gameType = this.GameType;
			byte[] combinedHash;
			switch (gameType)
			{
			case GameType.Campaign:
				combinedHash = LevelManager.Instance.VanillaCampaign[num].GetCombinedHash();
				goto IL_CA;
			case GameType.Challenge:
				combinedHash = LevelManager.Instance.Challenges[num].GetCombinedHash();
				goto IL_CA;
			case (GameType)3:
				break;
			case GameType.Versus:
				combinedHash = LevelManager.Instance.Versus[num].GetCombinedHash();
				goto IL_CA;
			default:
				if (gameType == GameType.Mythos)
				{
					combinedHash = LevelManager.Instance.MythosCampaign[num].GetCombinedHash();
					goto IL_CA;
				}
				if (gameType == GameType.StoryChallange)
				{
					combinedHash = LevelManager.Instance.StoryChallanges[num].GetCombinedHash();
					goto IL_CA;
				}
				break;
			}
			throw new Exception("Invalid GameType!");
			IL_CA:
			iWriter.Write(combinedHash.Length);
			iWriter.Write(combinedHash);
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x0006A25C File Offset: 0x0006845C
		public void Read(BinaryReader iReader)
		{
			this.GameName = iReader.ReadString();
			this.NrOfPlayers = iReader.ReadByte();
			this.GameType = (GameType)iReader.ReadByte();
			byte[] iLevelHashSum = iReader.ReadBytes(iReader.ReadInt32());
			LevelManager.Instance.GetLevel(this.GameType, iLevelHashSum, out this.Level);
		}

		// Token: 0x04000FD0 RID: 4048
		public int Latency;

		// Token: 0x04000FD1 RID: 4049
		public string GameName;

		// Token: 0x04000FD2 RID: 4050
		public byte NrOfPlayers;

		// Token: 0x04000FD3 RID: 4051
		public GameType GameType;

		// Token: 0x04000FD4 RID: 4052
		public int Level;
	}
}
