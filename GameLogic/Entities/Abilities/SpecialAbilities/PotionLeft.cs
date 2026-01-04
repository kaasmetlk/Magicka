// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.PotionLeft
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class PotionLeft(Magicka.Animations iAnimation) : Potion(Magicka.Animations.special0)
{
  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Potion have to be cast by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    NetworkState state = NetworkManager.Instance.State;
    if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
      return false;
    MissileEntity missileInstance = iOwner.GetMissileInstance();
    Vector3 translation = iOwner.CastSource.Translation;
    if (iOwner is Character)
      translation = (iOwner as Character).GetLeftAttachOrientation().Translation;
    Vector3 direction = iOwner.Direction with { Y = 1f };
    direction.Normalize();
    Vector3 up = Vector3.Up;
    Vector3 result1;
    Vector3.Cross(ref direction, ref up, out result1);
    result1.Normalize();
    float num = (float) (5.0 + (0.5 + MagickaMath.Random.NextDouble()) * 10.0 / 2.0);
    Vector3 result2;
    Vector3.Multiply(ref result1, -num, out result2);
    Potion.SpawnFlask(ref missileInstance, iOwner, ref translation, ref result2);
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
      {
        Type = SpawnMissileMessage.MissileType.PotionFlask,
        Handle = missileInstance.Handle,
        Item = (ushort) 0,
        Owner = iOwner.Handle,
        Position = translation,
        Velocity = direction
      });
    return true;
  }
}
