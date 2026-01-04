// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.PlayersHaveStatusEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class PlayersHaveStatusEffect : Condition
{
  private StatusEffects mStatusEffects;
  private Player[] mPlayers;

  public PlayersHaveStatusEffect(GameScene iScene)
    : base(iScene)
  {
    this.mPlayers = Game.Instance.Players;
  }

  protected override bool InternalMet(Character iSender)
  {
    for (int index = 0; index < this.mPlayers.Length; ++index)
    {
      if (this.mPlayers[index].Playing && this.mPlayers[index].Avatar != null)
      {
        for (int iIndex = 0; iIndex < 9; ++iIndex)
        {
          StatusEffects iStatus = Magicka.GameLogic.Spells.StatusEffect.StatusFromIndex(iIndex);
          if (this.mPlayers[index].Avatar.HasStatus(iStatus))
            return true;
        }
      }
    }
    return false;
  }

  public StatusEffects StatusEffect
  {
    get => this.mStatusEffects;
    set => this.mStatusEffects = value;
  }
}
