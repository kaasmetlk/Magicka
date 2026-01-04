// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.SpawnPoint
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels;

public struct SpawnPoint
{
  public int Scene;
  public unsafe fixed int Locations[4];
  public bool SpawnPlayers;

  public unsafe SpawnPoint(string iScene, string iLocation, bool iSpawnPlayers)
  {
    this.Scene = iScene.GetHashCodeCustom();
    fixed (int* numPtr = this.Locations)
    {
      for (int index = 0; index < 4; ++index)
        numPtr[index] = (iLocation + (object) index).GetHashCodeCustom();
    }
    this.SpawnPlayers = iSpawnPlayers;
  }
}
