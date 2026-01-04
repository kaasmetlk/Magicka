// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.PlayerSegmentManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry;

public class PlayerSegmentManager : Singleton<PlayerSegmentManager>
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.PlayerSegmentManager;
  private const string EXCEPTION_DATA_NOT_INITIALIZED = "Player segment data have not been initialized !";
  private const string SAVE_FILENAME = "segment.dat";
  private PlayerSegmentManager.SegmentData mData;

  public PlayerSegment CurrentSegment
  {
    get
    {
      if (this.mData == null)
        throw new Exception("Player segment data have not been initialized !");
      return new PlayerSegment(new PlayerSegment.Section[5]
      {
        this.mData.mHasPassedTutorial,
        this.mData.mHasWonAgainstAI,
        this.mData.mHasWonAgainstHuman,
        this.mData.mHasSpentRealMoney,
        this.mData.mHasSpentHardCurrency
      });
    }
  }

  private string DataPath => Path.Combine(ParadoxSettings.PARADOX_CACHE_PATH, "segment.dat");

  public PlayerSegmentManager() => this.Load();

  public void NotifyTutorialEnded()
  {
    this.mData.mHasPassedTutorial = PlayerSegment.Section.True;
    this.Save();
  }

  public void NotifyWonAgainstAI()
  {
    this.mData.mHasWonAgainstAI = PlayerSegment.Section.True;
    this.Save();
  }

  public void NotifyWonAgainstHuman()
  {
    this.mData.mHasWonAgainstHuman = PlayerSegment.Section.True;
    this.Save();
  }

  public void NotifySpentRealMoney()
  {
  }

  public void NotifySpentHardCurrency()
  {
  }

  private void Load()
  {
    if (File.Exists(this.DataPath))
    {
      FileStream serializationStream = File.Open(this.DataPath, FileMode.Open);
      this.mData = (PlayerSegmentManager.SegmentData) new BinaryFormatter().Deserialize((Stream) serializationStream);
      serializationStream.Close();
    }
    else
      this.mData = new PlayerSegmentManager.SegmentData();
  }

  private void Save()
  {
    if (this.mData != null)
    {
      ParadoxUtils.EnsureParadoxFolder();
      FileStream serializationStream = File.Open(this.DataPath, FileMode.Create);
      new BinaryFormatter().Serialize((Stream) serializationStream, (object) this.mData);
      serializationStream.Close();
    }
    else
      Logger.LogWarning(Logger.Source.PlayerSegmentManager, "There is no data to save.");
  }

  [Serializable]
  private class SegmentData
  {
    public PlayerSegment.Section mHasPassedTutorial;
    public PlayerSegment.Section mHasWonAgainstAI;
    public PlayerSegment.Section mHasWonAgainstHuman;
    public PlayerSegment.Section mHasSpentRealMoney = PlayerSegment.Section.NotApplicable;
    public PlayerSegment.Section mHasSpentHardCurrency = PlayerSegment.Section.NotApplicable;
  }
}
