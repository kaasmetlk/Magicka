// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.LoveAura
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class LoveAura(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_tsal_charm"))
{
  private static AuraLove AURA = new AuraLove(9f, 3f);

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (!(iOwner is Character character))
      return false;
    AuraStorage iAura = new AuraStorage(LoveAura.AURA, AuraTarget.Enemy, AuraType.Love, 0, 5f, 3f, VisualCategory.Special, Spell.FIRECOLOR, (int[]) null, Factions.NONE);
    character.AddAura(ref iAura, false);
    return base.Execute(iOwner, iPlayState);
  }
}
