// Decompiled with JetBrains decompiler
// Type: Magicka.Storage.SaveData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.Storage;

public class SaveData
{
  private static byte[] sBuffer = new byte[1024 /*0x0400*/];
  private byte mLevel;
  private byte mMaxAllowedLevel;
  private bool mLooped;
  private int mTotalPlayTime;
  private int mCurrentPlayTime;
  private Dictionary<string, PlayerSaveData> mPlayers;
  private SaveData.tip[] mShownTips = new SaveData.tip[11];
  private ulong mUnlockedMagicks;
  private MemoryStream mCheckPoint;

  public SaveData()
  {
    this.mLevel = (byte) 0;
    this.mMaxAllowedLevel = (byte) 0;
    this.mPlayers = new Dictionary<string, PlayerSaveData>();
    for (int index = 0; index < this.mShownTips.Length; ++index)
      this.mShownTips[index].tipName = TutorialManager.TipsNames[index];
    this.mUnlockedMagicks = 0UL;
  }

  public MemoryStream Checkpoint
  {
    get => this.mCheckPoint;
    set => this.mCheckPoint = value;
  }

  public byte Level
  {
    get => this.mLevel;
    set
    {
      this.mLevel = value;
      this.mMaxAllowedLevel = Math.Max(this.mMaxAllowedLevel, this.mLevel);
    }
  }

  public byte MaxAllowedLevel => this.mMaxAllowedLevel;

  public bool Looped
  {
    get => this.mLooped;
    set => this.mLooped = value;
  }

  public int TotalPlayTime
  {
    get => this.mTotalPlayTime;
    set => this.mTotalPlayTime = value;
  }

  public int CurrentPlayTime
  {
    get => this.mCurrentPlayTime;
    set => this.mCurrentPlayTime = value;
  }

  public Dictionary<string, PlayerSaveData> Players => this.mPlayers;

  public SaveData.tip[] ShownTips => this.mShownTips;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.mLevel);
    iWriter.Write(this.mMaxAllowedLevel);
    iWriter.Write(this.mLooped);
    iWriter.Write(this.mTotalPlayTime);
    iWriter.Write(this.mCurrentPlayTime);
    iWriter.Write(this.mPlayers.Count);
    foreach (KeyValuePair<string, PlayerSaveData> mPlayer in this.mPlayers)
    {
      iWriter.Write(mPlayer.Key);
      mPlayer.Value.Write(iWriter);
    }
    iWriter.Write(this.mUnlockedMagicks);
    iWriter.Write(this.mShownTips.Length);
    for (int index = 0; index < this.mShownTips.Length; ++index)
    {
      iWriter.Write(this.mShownTips[index].tipName);
      iWriter.Write(this.mShownTips[index].count);
    }
    if (this.mCheckPoint != null && this.mCheckPoint.Length > 0L)
    {
      iWriter.Write((int) this.mCheckPoint.Length);
      this.mCheckPoint.WriteTo(iWriter.BaseStream);
    }
    else
      iWriter.Write(0);
  }

  public static SaveData Read(ulong iVersion, BinaryReader iReader, SaveData iExistingInstance)
  {
    SaveData iTarget = iExistingInstance;
    if (iTarget == null)
      iTarget = new SaveData();
    else
      iTarget.mPlayers.Clear();
    if (iVersion >= 281492156645376UL /*0x01000400010000*/)
      SaveData.Read1410(iReader, iTarget);
    else
      SaveData.Read1000(iReader, iTarget);
    return iTarget;
  }

  private static void Read1410(BinaryReader iReader, SaveData iTarget)
  {
    iTarget.mLevel = iReader.ReadByte();
    iTarget.mMaxAllowedLevel = iReader.ReadByte();
    iTarget.mLooped = iReader.ReadBoolean();
    iTarget.mTotalPlayTime = iReader.ReadInt32();
    iTarget.mCurrentPlayTime = iReader.ReadInt32();
    int num1 = iReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      string key = iReader.ReadString();
      PlayerSaveData playerSaveData = PlayerSaveData.Read(iReader);
      iTarget.mPlayers.Add(key, playerSaveData);
    }
    iTarget.mUnlockedMagicks = iReader.ReadUInt64();
    int num2 = iReader.ReadInt32();
    iTarget.mShownTips = new SaveData.tip[11];
    for (int index = 0; index < num2; ++index)
      iTarget.mShownTips[index] = new SaveData.tip()
      {
        tipName = iReader.ReadString(),
        timeStamp = double.NegativeInfinity,
        count = iReader.ReadInt32()
      };
    int num3 = iReader.ReadInt32();
    if (num3 > 0)
    {
      iTarget.mCheckPoint = new MemoryStream(num3);
      int count;
      for (; num3 > 0; num3 -= count)
      {
        count = iReader.Read(SaveData.sBuffer, 0, Math.Min(num3, SaveData.sBuffer.Length));
        iTarget.mCheckPoint.Write(SaveData.sBuffer, 0, count);
      }
      iTarget.mCheckPoint.Position = 0L;
    }
    else
      iTarget.mCheckPoint = (MemoryStream) null;
  }

  private static void Read1000(BinaryReader iReader, SaveData iTarget)
  {
    iTarget.mLevel = iReader.ReadByte();
    iTarget.mMaxAllowedLevel = iTarget.mLevel;
    iTarget.mLooped = iReader.ReadBoolean();
    iTarget.mTotalPlayTime = iReader.ReadInt32();
    iTarget.mCurrentPlayTime = iReader.ReadInt32();
    int num1 = iReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      string key = iReader.ReadString();
      PlayerSaveData playerSaveData = PlayerSaveData.Read(iReader);
      iTarget.mPlayers.Add(key, playerSaveData);
    }
    iTarget.mUnlockedMagicks = iReader.ReadUInt64();
    int num2 = iReader.ReadInt32();
    iTarget.mShownTips = new SaveData.tip[11];
    for (int index = 0; index < num2; ++index)
      iTarget.mShownTips[index] = new SaveData.tip()
      {
        tipName = iReader.ReadString(),
        timeStamp = double.NegativeInfinity,
        count = iReader.ReadInt32()
      };
  }

  public ulong UnlockedMagicks
  {
    get => this.mUnlockedMagicks;
    set => this.mUnlockedMagicks = value;
  }

  public struct tip
  {
    public string tipName;
    public double timeStamp;
    public int count;
  }
}
