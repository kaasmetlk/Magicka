// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.NorthPole
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class NorthPole(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_tsal_firebolt"))
{
  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    NetworkState state = NetworkManager.Instance.State;
    if ((state == NetworkState.Client || iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
      return this.Execute(iOwner.Position, iPlayState);
    base.Execute(iOwner, iPlayState);
    Avatar avatar = iOwner as Avatar;
    Vector3 result1 = iOwner.Direction;
    Vector3 result2 = iOwner.Position;
    Vector3.Multiply(ref result1, 10f, out result1);
    Vector3.Add(ref result1, ref result2, out result2);
    Teleport.Instance.DoTeleport(iOwner, result2, result1, Teleport.TeleportType.Regular);
    avatar.ConjureCold();
    avatar.ConjureCold();
    avatar.ConjureCold();
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Handle = avatar.Handle,
        Action = ActionType.CastSpell,
        Param0I = 2
      });
    avatar.CastType = CastType.Area;
    avatar.CastSpell(true, (string) null);
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;
}
