// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Invisibility
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class Invisibility : SpecialAbility
{
  public const float INVISIBILITY_TIME = 15f;
  private static Invisibility mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly int SOUNDHASH = "magick_invisibility".GetHashCodeCustom();
  public static readonly int INVISIBILITY_EFFECT = "invisibility".GetHashCodeCustom();

  public static Invisibility Instance
  {
    get
    {
      if (Invisibility.mSingelton == null)
      {
        lock (Invisibility.mSingeltonLock)
        {
          if (Invisibility.mSingelton == null)
            Invisibility.mSingelton = new Invisibility();
        }
      }
      return Invisibility.mSingelton;
    }
  }

  public Invisibility(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_invisibility".GetHashCodeCustom())
  {
  }

  private Invisibility()
    : base(Magicka.Animations.cast_magick_self, "#magick_invisibility".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("Invisibility have to be cast by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (!(iOwner is Character))
      return false;
    AudioManager.Instance.PlayCue(Banks.Spells, Invisibility.SOUNDHASH, iOwner.AudioEmitter);
    (iOwner as Character).SetInvisible(15f);
    return true;
  }
}
