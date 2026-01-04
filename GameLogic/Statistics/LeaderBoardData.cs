// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Statistics.LeaderBoardData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.GameLogic.Statistics;

public struct LeaderBoardData
{
  public const int MAX_LOCAL_LEADERBOARDS = 8;
  public static LeaderBoardData.ComparerDataBeforeScore DataBeforeScoreComparer = new LeaderBoardData.ComparerDataBeforeScore();
  public static LeaderBoardData.ComparerScoreBeforeData ScoreBeforeDataComparer = new LeaderBoardData.ComparerScoreBeforeData();
  public int Score;
  public string Name;
  public int Data1;

  public static void ReadData(BinaryReader iReader, LeaderBoardData[] iStorage)
  {
    for (int index = 0; index < 8; ++index)
      iStorage[index].Read(iReader);
  }

  public void Read(BinaryReader iReader)
  {
    this.Score = iReader.ReadInt32();
    this.Name = iReader.ReadString();
    this.Data1 = iReader.ReadInt32();
  }

  public void Writer(BinaryWriter iWriter)
  {
    iWriter.Write(this.Score);
    iWriter.Write(this.Name);
    iWriter.Write(this.Data1);
  }

  public LeaderBoardData(int iScore, string iGamerTag, byte iWaves)
  {
    this.Score = iScore;
    this.Name = iGamerTag;
    this.Data1 = (int) iWaves;
  }

  public LeaderBoardData(int iScore, string iGamerTag, int iTime)
  {
    this.Score = iScore;
    this.Name = iGamerTag;
    this.Data1 = iTime;
  }

  public class ComparerDataBeforeScore : IComparer<LeaderBoardData>
  {
    public int Compare(LeaderBoardData x, LeaderBoardData y)
    {
      return x.Data1 == y.Data1 ? y.Score - x.Score : y.Data1 - x.Data1;
    }
  }

  public class ComparerScoreBeforeData : IComparer<LeaderBoardData>
  {
    public int Compare(LeaderBoardData x, LeaderBoardData y)
    {
      if (x.Score != y.Score)
        return y.Score - x.Score;
      FloatIntConverter floatIntConverter1 = new FloatIntConverter(x.Data1);
      FloatIntConverter floatIntConverter2 = new FloatIntConverter(y.Data1);
      if ((double) floatIntConverter1.Float > (double) floatIntConverter2.Float)
        return -1;
      return (double) floatIntConverter1.Float < (double) floatIntConverter2.Float ? 1 : 0;
    }
  }
}
