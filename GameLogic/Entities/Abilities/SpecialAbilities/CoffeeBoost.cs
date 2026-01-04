// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.CoffeeBoost
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class CoffeeBoost : SpecialAbility
{
  private const float TTL = 4f;
  private static readonly int DISPLAY_NAME = "#specab_drink".GetHashCodeCustom();
  private Damage mHealingA;
  private Damage mHealingB;

  public CoffeeBoost(Magicka.Animations iAnimation)
    : base(Magicka.Animations.special0, CoffeeBoost.DISPLAY_NAME)
  {
    this.mHealingA = new Damage(AttackProperties.Damage, Elements.Life, -100f, 1f);
    this.mHealingB = new Damage(AttackProperties.Status, Elements.Life, -100f, 2f);
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("CoffeeBoost have to be cast by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    DamageResult damageResult = iOwner.Damage(this.mHealingA, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation) | iOwner.Damage(this.mHealingB, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
    Haste instance = Haste.GetInstance();
    instance.CustomTTL = 4f;
    return instance.Execute(iOwner, iPlayState, false);
  }
}
