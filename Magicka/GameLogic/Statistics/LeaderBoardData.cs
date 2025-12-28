using System;
using System.Collections.Generic;
using System.IO;

namespace Magicka.GameLogic.Statistics
{
	// Token: 0x020000CE RID: 206
	public struct LeaderBoardData
	{
		// Token: 0x06000644 RID: 1604 RVA: 0x000253F0 File Offset: 0x000235F0
		public static void ReadData(BinaryReader iReader, LeaderBoardData[] iStorage)
		{
			for (int i = 0; i < 8; i++)
			{
				iStorage[i].Read(iReader);
			}
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00025416 File Offset: 0x00023616
		public void Read(BinaryReader iReader)
		{
			this.Score = iReader.ReadInt32();
			this.Name = iReader.ReadString();
			this.Data1 = iReader.ReadInt32();
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x0002543C File Offset: 0x0002363C
		public void Writer(BinaryWriter iWriter)
		{
			iWriter.Write(this.Score);
			iWriter.Write(this.Name);
			iWriter.Write(this.Data1);
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x00025462 File Offset: 0x00023662
		public LeaderBoardData(int iScore, string iGamerTag, byte iWaves)
		{
			this.Score = iScore;
			this.Name = iGamerTag;
			this.Data1 = (int)iWaves;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00025479 File Offset: 0x00023679
		public LeaderBoardData(int iScore, string iGamerTag, int iTime)
		{
			this.Score = iScore;
			this.Name = iGamerTag;
			this.Data1 = iTime;
		}

		// Token: 0x04000502 RID: 1282
		public const int MAX_LOCAL_LEADERBOARDS = 8;

		// Token: 0x04000503 RID: 1283
		public static LeaderBoardData.ComparerDataBeforeScore DataBeforeScoreComparer = new LeaderBoardData.ComparerDataBeforeScore();

		// Token: 0x04000504 RID: 1284
		public static LeaderBoardData.ComparerScoreBeforeData ScoreBeforeDataComparer = new LeaderBoardData.ComparerScoreBeforeData();

		// Token: 0x04000505 RID: 1285
		public int Score;

		// Token: 0x04000506 RID: 1286
		public string Name;

		// Token: 0x04000507 RID: 1287
		public int Data1;

		// Token: 0x020000CF RID: 207
		public class ComparerDataBeforeScore : IComparer<LeaderBoardData>
		{
			// Token: 0x0600064A RID: 1610 RVA: 0x000254A6 File Offset: 0x000236A6
			public int Compare(LeaderBoardData x, LeaderBoardData y)
			{
				if (x.Data1 == y.Data1)
				{
					return y.Score - x.Score;
				}
				return y.Data1 - x.Data1;
			}
		}

		// Token: 0x020000D0 RID: 208
		public class ComparerScoreBeforeData : IComparer<LeaderBoardData>
		{
			// Token: 0x0600064C RID: 1612 RVA: 0x000254E0 File Offset: 0x000236E0
			public int Compare(LeaderBoardData x, LeaderBoardData y)
			{
				if (x.Score != y.Score)
				{
					return y.Score - x.Score;
				}
				FloatIntConverter floatIntConverter = new FloatIntConverter(x.Data1);
				FloatIntConverter floatIntConverter2 = new FloatIntConverter(y.Data1);
				if (floatIntConverter.Float > floatIntConverter2.Float)
				{
					return -1;
				}
				if (floatIntConverter.Float < floatIntConverter2.Float)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
