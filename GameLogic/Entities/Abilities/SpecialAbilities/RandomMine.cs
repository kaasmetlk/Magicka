// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.RandomMine
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

public class RandomMine : SpecialAbility
{
  private static RandomMine mSingelton;
  private static volatile object mSingeltonLock = new object();
  private PlayState mPlayState;
  private bool mDoDamage;

  public static RandomMine Instance
  {
    get
    {
      if (RandomMine.mSingelton == null)
      {
        lock (RandomMine.mSingeltonLock)
        {
          if (RandomMine.mSingelton == null)
            RandomMine.mSingelton = new RandomMine();
        }
      }
      return RandomMine.mSingelton;
    }
  }

  private RandomMine()
    : base(Magicka.Animations.cast_magick_global, "#magick_reddit_protection".GetHashCodeCustom())
  {
  }

  public RandomMine(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_reddit_protection".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mDoDamage = NetworkManager.Instance.State != NetworkState.Client;
    return this.Execute();
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (iOwner is Avatar avatar && !(avatar.Player.Gamer is NetworkGamer))
    {
      avatar.ConjureShield();
      switch (SpecialAbility.RANDOM.Next(7))
      {
        case 0:
          avatar.ConjureArcane();
          break;
        case 1:
          avatar.ConjureArcane();
          avatar.ConjureCold();
          break;
        case 2:
          avatar.ConjureArcane();
          avatar.ConjureWater();
          break;
        case 3:
          avatar.ConjureArcane();
          avatar.ConjureFire();
          break;
        case 4:
          avatar.ConjureArcane();
          avatar.ConjureLightning();
          break;
        case 5:
          avatar.ConjureArcane();
          avatar.ConjureFire();
          avatar.ConjureWater();
          break;
        case 6:
          avatar.ConjureLife();
          break;
      }
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = avatar.Handle,
          Action = ActionType.CastSpell,
          Param0I = 1
        });
      avatar.CastType = CastType.Force;
      avatar.CastSpell(true, (string) null);
    }
    return this.Execute();
  }

  private bool Execute() => true;
}
