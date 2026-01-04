// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.RandomTeleport
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class RandomTeleport(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_eteleport"))
{
  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("RandomTeleport must be called by an entity!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return true;
    base.Execute(iOwner, iPlayState);
    Vector3 position = iOwner.Position;
    float num1 = (float) Math.Pow(SpecialAbility.RANDOM.NextDouble(), 0.25);
    float num2 = (float) SpecialAbility.RANDOM.NextDouble() * 6.28318548f;
    float num3 = num1 * (float) Math.Cos((double) num2);
    float num4 = num1 * (float) Math.Sin((double) num2);
    position.X += 20f * num3;
    position.Z += 20f * num4;
    Vector3 result = iOwner.Position;
    Vector3.Subtract(ref position, ref result, out result);
    result.Y = 0.0f;
    result.Normalize();
    return Teleport.Instance.DoTeleport(iOwner, position, result, Teleport.TeleportType.Regular);
  }
}
