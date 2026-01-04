// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Levitate
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Levitate : SpecialAbility
{
  private static Levitate sSingelton;
  private static volatile object sSingeltonLock = new object();
  public static readonly int MAGICK_EFFECT = "magick_levitate".GetHashCodeCustom();
  private static readonly int MAGICK_SOUND = Haste.SOUNDHASH;

  public static Levitate Instance
  {
    get
    {
      if (Levitate.sSingelton == null)
      {
        lock (Levitate.sSingeltonLock)
        {
          if (Levitate.sSingelton == null)
            Levitate.sSingelton = new Levitate();
        }
      }
      return Levitate.sSingelton;
    }
  }

  private Levitate()
    : base(Magicka.Animations.cast_magick_self, "#magick_levitate".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    NetworkState state = NetworkManager.Instance.State;
    if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
    {
      if (!(iOwner is Character iOwner1))
        return false;
      Levitate.CastLevitate(iOwner1);
      if (state != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Action = ActionType.Magick,
          Handle = iOwner.Handle,
          Param3I = 28
        });
    }
    return true;
  }

  internal static void CastLevitate(Character iOwner)
  {
    AudioManager.Instance.PlayCue(Banks.Spells, Levitate.MAGICK_SOUND, iOwner.AudioEmitter);
    iOwner.SetLevitate(30f, Levitate.MAGICK_EFFECT);
  }
}
