// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Zoom
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Zoom(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_tsal_magnify"))
{
  private const float TTL = 4f;
  private const float MAGNIFICATION = 2f;

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    iPlayState.Camera.SetPlayerMagnification(2f, 4f);
    return base.Execute(iOwner, iPlayState);
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    iPlayState.Camera.SetPlayerMagnification(2f, 4f);
    return base.Execute(iPosition, iPlayState);
  }
}
