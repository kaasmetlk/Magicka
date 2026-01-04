// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.PlayerCount
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public class PlayerCount(GameScene iScene) : Condition(iScene)
{
  private CompareMethod mCompareMethod;
  private int mNr;

  protected override bool InternalMet(Character iSender)
  {
    int playerCount = Game.Instance.PlayerCount;
    switch (this.mCompareMethod)
    {
      case CompareMethod.LESS:
        return this.mNr > playerCount;
      case CompareMethod.EQUAL:
        return this.mNr == playerCount;
      case CompareMethod.GREATER:
        return this.mNr < playerCount;
      default:
        return false;
    }
  }

  public CompareMethod CompareMethod
  {
    get => this.mCompareMethod;
    set => this.mCompareMethod = value;
  }

  public int Nr
  {
    get => this.mNr;
    set => this.mNr = value;
  }
}
