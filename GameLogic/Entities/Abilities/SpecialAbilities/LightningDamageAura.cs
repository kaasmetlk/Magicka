// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.LightningDamageAura
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class LightningDamageAura(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_lig_au"))
{
  private static readonly int LDA_EFFECT = "lightning_damage_aura".GetHashCodeCustom();

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    AuraStorage iAura = new AuraStorage(new AuraBuff(new BuffStorage(new BuffDealDamage(new Damage()
    {
      Amount = 100f,
      Magnitude = 1f,
      Element = Elements.Lightning,
      AttackProperty = AttackProperties.Damage
    }), VisualCategory.None, Spell.LIGHTNINGCOLOR)), AuraTarget.AllButSelf, AuraType.Buff, LightningDamageAura.LDA_EFFECT, 5f, 10f, VisualCategory.Offensive, Spell.LIGHTNINGCOLOR, (int[]) null, Factions.NONE);
    (iOwner as Character).AddAura(ref iAura, false);
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;
}
