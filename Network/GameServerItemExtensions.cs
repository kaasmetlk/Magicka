// Decompiled with JetBrains decompiler
// Type: Magicka.Network.GameServerItemExtensions
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using SteamWrapper;

#nullable disable
namespace Magicka.Network;

internal static class GameServerItemExtensions
{
  public static bool Playing(this GameServerItem self)
  {
    int num = self.GameTags.IndexOf('P');
    return num >= 0 && self.GameTags[num + 1] == '1';
  }

  public static Magicka.GameLogic.GameStates.GameType GameType(this GameServerItem self)
  {
    int num = self.GameTags.IndexOf('T');
    return num >= 0 ? (Magicka.GameLogic.GameStates.GameType) ((uint) self.GameTags[num + 1] - 48U /*0x30*/) : (Magicka.GameLogic.GameStates.GameType) 0;
  }
}
