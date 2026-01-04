// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.StaffShield
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class StaffShield(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_ss"))
{
  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    Spell iSpell = new Spell();
    iSpell.ShieldMagnitude = 1f;
    iSpell.Element = Elements.Shield;
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      if (NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Action = ActionType.SelfShield,
          Handle = iOwner.Handle,
          Param0I = 16777216 /*0x01000000*/ | (int) (ushort) iSpell.Element
        });
      iOwner.AddSelfShield(iSpell);
    }
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;
}
