// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SelfConfuse
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class SelfConfuse(Magicka.Animations iAnimation) : SpecialAbility(Magicka.Animations.cast_magick_direct, SelfConfuse.DISPLAY_NAME)
{
  private static readonly int DISPLAY_NAME = "#magick_confuse".GetHashCodeCustom();

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("SelfConfuse have to be cast by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    return Confuse.GetInstance().Execute(iOwner, iOwner as Entity, iPlayState);
  }
}
