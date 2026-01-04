// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.AnyPlayerHasDied
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class AnyPlayerHasDied(GameScene iScene) : Condition(iScene)
{
  protected override bool InternalMet(Character iSender) => PlayState.RecentPlayState.DiedInLevel;
}
