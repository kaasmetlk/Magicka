// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Votal
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Votal : SpecialAbility, ITargetAbility
{
  private static Votal mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static Votal Instance
  {
    get
    {
      if (Votal.mSingelton == null)
      {
        lock (Votal.mSingeltonLock)
        {
          if (Votal.mSingelton == null)
            Votal.mSingelton = new Votal();
        }
      }
      return Votal.mSingelton;
    }
  }

  public Votal(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_votal".GetHashCodeCustom())
  {
  }

  private Votal()
    : base(Magicka.Animations.cast_magick_self, "#magick_votal".GetHashCodeCustom())
  {
  }

  public bool Execute(ISpellCaster iOwner, Entity iTarget, PlayState iPlayState)
  {
    if (!(iTarget is Character))
      return false;
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      Damage iDamage = new Damage();
      iDamage.Amount = 50f;
      iDamage.AttackProperty = AttackProperties.Status;
      iDamage.Element = Elements.None;
      iDamage.Magnitude = 1f;
      switch (SpecialAbility.RANDOM.Next(7))
      {
        case 0:
          iDamage.Element = Elements.Poison;
          break;
        case 1:
          iDamage.Element = Elements.Water;
          break;
        case 2:
          iDamage.AttackProperty = AttackProperties.Knockdown;
          break;
        case 3:
          int num1 = (int) (iTarget as Character).Damage(new Damage()
          {
            Amount = 1f,
            AttackProperty = AttackProperties.Status,
            Element = Elements.Water,
            Magnitude = 1f
          }, iOwner as Entity, 0.0, iOwner.Position);
          iDamage.Element = Elements.Cold;
          break;
        case 4:
          iDamage.Element = Elements.Cold;
          break;
        case 5:
          iDamage.Amount = 30f;
          iDamage.AttackProperty = AttackProperties.Bleed;
          break;
        case 6:
          iDamage.Element = Elements.Fire;
          break;
      }
      int num2 = (int) (iTarget as Character).Damage(iDamage, iOwner as Entity, 0.0, iOwner.Position);
    }
    return true;
  }
}
