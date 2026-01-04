// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SmokeBomb
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class SmokeBomb(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#item_specab_smoke"))
{
  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("SmokeBomb must be called by an entity!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return true;
    base.Execute(iOwner, iPlayState);
    Vector3 result1 = iOwner.Position;
    Matrix rotationY = Matrix.CreateRotationY((float) ((SpecialAbility.RANDOM.NextDouble() - 0.5) * 0.78539818525314331));
    Vector3 result2 = iOwner.Direction;
    Vector3 result3;
    Vector3.Negate(ref result2, out result3);
    Vector3.Transform(ref result3, ref rotationY, out result3);
    Vector3.Negate(ref result3, out result2);
    Vector3.Multiply(ref result3, 10f, out result3);
    Vector3.Add(ref result3, ref result1, out result1);
    return Teleport.Instance.DoTeleport(iOwner, result1, result2, Teleport.TeleportType.SmokeBomb);
  }
}
