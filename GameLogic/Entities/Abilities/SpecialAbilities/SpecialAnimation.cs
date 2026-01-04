// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAnimation
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class SpecialAnimation : SpecialAbility
{
  private const float TTL = 4f;

  public SpecialAnimation(Magicka.Animations iAnimation, int iDisplayName)
    : base(iAnimation, iDisplayName)
  {
  }

  public SpecialAnimation(Magicka.Animations iAnimation)
    : base(iAnimation, "#item_specab_bash".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("This action must be cast by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState) => true;
}
