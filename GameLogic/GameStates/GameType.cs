// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.GameType
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.GameStates;

public enum GameType : byte
{
  Campaign = 1,
  Challenge = 2,
  Versus = 4,
  Mythos = 8,
  Any = 15, // 0x0F
  StoryChallange = 22, // 0x16
}
