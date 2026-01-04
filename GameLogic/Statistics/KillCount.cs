// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Statistics.KillCount
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Statistics;

public struct KillCount
{
  public float TTL;
  public int Count;

  public KillCount(int iCount, float iTTL)
  {
    this.Count = iCount;
    this.TTL = iTTL;
  }

  public KillCount(int iCount)
  {
    this.Count = iCount;
    this.TTL = 60f;
  }

  public KillCount(float iTTL)
  {
    this.Count = 1;
    this.TTL = iTTL;
  }
}
