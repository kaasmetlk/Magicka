// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Etherealize
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class Etherealize : SpecialAbility
{
  private static Etherealize sSingelton;
  private static volatile object sSingeltonLock = new object();
  internal static readonly int SOUND_EFFECT = "chr_daemon_etherealize3".GetHashCodeCustom();
  internal static readonly int MAGICK_EFFECT = "magick_etherealize".GetHashCodeCustom();

  public static Etherealize Instance
  {
    get
    {
      if (Etherealize.sSingelton == null)
      {
        lock (Etherealize.sSingeltonLock)
        {
          if (Etherealize.sSingelton == null)
            Etherealize.sSingelton = new Etherealize();
        }
      }
      return Etherealize.sSingelton;
    }
  }

  private Etherealize()
    : base(Magicka.Animations.cast_magick_self, "#magick_etherealize".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (iOwner is Character character)
    {
      NetworkState state = NetworkManager.Instance.State;
      if (state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer)) || state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))
      {
        character.Ethereal(true, 1f, 1f);
        Vector3 position = iOwner.Position;
        Vector3 direction = iOwner.Direction;
        EffectManager.Instance.StartEffect(Etherealize.MAGICK_EFFECT, ref position, ref direction, out VisualEffectReference _);
        AudioManager.Instance.PlayCue(Banks.Characters, Etherealize.SOUND_EFFECT, iOwner.AudioEmitter);
        if (state != NetworkState.Offline)
          NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
          {
            Action = ActionType.Magick,
            Handle = iOwner.Handle,
            Param3I = 38
          });
      }
      return true;
    }
    AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
    return false;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Etherealize can only be called by a character!");
  }
}
