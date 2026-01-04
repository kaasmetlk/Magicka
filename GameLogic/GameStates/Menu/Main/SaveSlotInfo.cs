// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SaveSlotInfo
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Network;
using Magicka.Storage;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal struct SaveSlotInfo : ISendable
{
  private bool mValid;
  private bool mLooped;
  private ulong mUnlockedMagicks;
  private Dictionary<string, PlayerSaveData> mPlayers;
  private SaveData.tip[] mShownTips;

  public SaveSlotInfo(SaveData iSaveData)
  {
    this.mValid = true;
    this.mLooped = iSaveData.Looped;
    this.mUnlockedMagicks = iSaveData.UnlockedMagicks;
    this.mPlayers = iSaveData.Players;
    this.mShownTips = iSaveData.ShownTips;
  }

  public SaveSlotInfo(
    bool iLooped,
    ulong iUnlockedMagicks,
    Dictionary<string, PlayerSaveData> iPlayers,
    SaveData.tip[] iShownTips)
  {
    this.mValid = true;
    this.mLooped = iLooped;
    this.mUnlockedMagicks = iUnlockedMagicks;
    this.mPlayers = iPlayers;
    this.mShownTips = iShownTips;
  }

  public bool IsValid => this.mValid;

  public bool Looped => this.mLooped;

  public Dictionary<string, PlayerSaveData> Players => this.mPlayers;

  public ulong UnlockedMagicks => this.mUnlockedMagicks;

  public SaveData.tip[] ShownTips => this.mShownTips;

  public PacketType PacketType => PacketType.SaveData;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Looped);
    iWriter.Write(this.UnlockedMagicks);
    iWriter.Write(this.Players.Count);
    foreach (KeyValuePair<string, PlayerSaveData> mPlayer in this.mPlayers)
    {
      iWriter.Write(mPlayer.Key);
      mPlayer.Value.Write(iWriter);
    }
    iWriter.Write(this.ShownTips.Length);
    for (int index = 0; index < this.ShownTips.Length; ++index)
    {
      iWriter.Write(this.ShownTips[index].tipName);
      iWriter.Write(this.ShownTips[index].count);
    }
  }

  public void Read(BinaryReader iReader)
  {
    this.mValid = true;
    this.mLooped = iReader.ReadBoolean();
    this.mUnlockedMagicks = iReader.ReadUInt64();
    int capacity = iReader.ReadInt32();
    this.mPlayers = new Dictionary<string, PlayerSaveData>(capacity);
    for (int index = 0; index < capacity; ++index)
      this.mPlayers.Add(iReader.ReadString(), PlayerSaveData.Read(iReader));
    int num = iReader.ReadInt32();
    this.mShownTips = new SaveData.tip[11];
    for (int index = 0; index < num; ++index)
    {
      SaveData.tip tip;
      tip.tipName = iReader.ReadString();
      tip.timeStamp = double.MinValue;
      tip.count = iReader.ReadInt32();
      this.mShownTips[index] = tip;
    }
  }
}
