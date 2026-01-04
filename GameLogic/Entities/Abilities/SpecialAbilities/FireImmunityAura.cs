// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.FireImmunityAura
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class FireImmunityAura(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_fire_im"))
{
  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    AuraStorage iAura = new AuraStorage(new AuraBuff(new BuffStorage(new BuffResistance(new Resistance()
    {
      ResistanceAgainst = Elements.Fire,
      Multiplier = 0.0f
    }), VisualCategory.Defensive, Spell.FIRECOLOR)), AuraTarget.Friendly, AuraType.Buff, 0, 10f, 10f, VisualCategory.Defensive, Spell.FIRECOLOR, (int[]) null, Factions.NONE);
    (iOwner as Character).AddAura(ref iAura, false);
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;
}
