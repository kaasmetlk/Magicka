// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Campaign.CampaignNode
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Campaign;

internal class CampaignNode : LevelNode
{
  internal Magicka.Levels.SpawnPoint? SpawnPoint;
  internal Cutscene Cutscene;

  public CampaignNode(string iFileName, Magicka.Levels.SpawnPoint? iSpawnPoint)
    : base("", iFileName)
  {
    this.SpawnPoint = iSpawnPoint;
  }
}
