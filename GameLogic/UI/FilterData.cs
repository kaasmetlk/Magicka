// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.FilterData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using System.IO;

#nullable disable
namespace Magicka.GameLogic.UI;

public struct FilterData
{
  public const byte MIN_SLOTS = 1;
  public const byte MAX_SLOTS = 3;
  public byte FreeSlots;
  public Scope Scope;
  public GameType GameType;
  public short MaxLatency;
  public bool VACOnly;
  public bool FilterPlaying;
  public bool FilterPassword;

  public static void Write(BinaryWriter iWriter, FilterData iData)
  {
    iWriter.Write(iData.FreeSlots);
    iWriter.Write((byte) iData.Scope);
    iWriter.Write((byte) iData.GameType);
    iWriter.Write(iData.MaxLatency);
    iWriter.Write(iData.VACOnly);
    iWriter.Write(iData.FilterPlaying);
    iWriter.Write(iData.FilterPassword);
  }

  public static void Read1410(BinaryReader iReader, out FilterData oData)
  {
    oData.FreeSlots = iReader.ReadByte();
    oData.Scope = (Scope) iReader.ReadByte();
    oData.GameType = (GameType) iReader.ReadByte();
    oData.MaxLatency = iReader.ReadInt16();
    oData.VACOnly = iReader.ReadBoolean();
    oData.FilterPlaying = iReader.ReadBoolean();
    oData.FilterPassword = iReader.ReadBoolean();
  }

  public static void Read1400(BinaryReader iReader, out FilterData oData)
  {
    oData.FreeSlots = iReader.ReadByte();
    oData.Scope = (Scope) iReader.ReadByte();
    oData.GameType = (GameType) iReader.ReadByte();
    oData.MaxLatency = iReader.ReadInt16();
    oData.VACOnly = iReader.ReadByte() == (byte) 1;
    oData.FilterPlaying = false;
    oData.FilterPassword = false;
  }
}
