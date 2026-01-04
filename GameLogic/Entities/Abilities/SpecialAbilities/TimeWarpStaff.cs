// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.TimeWarpStaff
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class TimeWarpStaff : SpecialAbility, IAbilityEffect
{
  private const float DURATION_TIME = 15f;
  private const float TIME_TARGET = 0.5f;
  private const float SATURATION_TARGET = 0.1f;
  private static TimeWarpStaff mSingelton;
  private static volatile object mSingeltonLock = new object();
  private float mTTL;
  private float mTimeMultiplierTarget;
  private float mTimeMultiplier;
  private float mSaturationTarget;
  private float mSaturation;
  private float mFadeTime = 1f;
  private static Cue sCue;
  private PlayState mPlayState;
  private ISpellCaster mOwner;
  public static readonly int SOUND_HASH = "magick_timewarp".GetHashCodeCustom();
  private static readonly int EFFECT = "timewarp_effect".GetHashCodeCustom();

  public static TimeWarpStaff Instance
  {
    get
    {
      if (TimeWarpStaff.mSingelton == null)
      {
        lock (TimeWarpStaff.mSingeltonLock)
        {
          if (TimeWarpStaff.mSingelton == null)
            TimeWarpStaff.mSingelton = new TimeWarpStaff();
        }
      }
      return TimeWarpStaff.mSingelton;
    }
  }

  public TimeWarpStaff(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_timewarp".GetHashCodeCustom())
  {
  }

  private TimeWarpStaff()
    : base(Magicka.Animations.cast_magick_self, "#magick_timewarp".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (iOwner == null)
      throw new Exception("TimeWarpStaff can not be cast without a valid owner!");
    if (SpellManager.Instance.IsEffectActive(this.GetType()) || SpellManager.Instance.IsEffectActive(typeof (TimeWarp)))
      return false;
    Vector3 translation = iOwner.CastSource.Translation;
    Vector3 direction = iOwner.Direction;
    EffectManager.Instance.StartEffect(TimeWarpStaff.EFFECT, ref translation, ref direction, out VisualEffectReference _);
    this.mTTL = 15f;
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    this.mTimeMultiplier = 1f;
    this.mSaturation = this.mPlayState.Level.CurrentScene.Saturation;
    this.mTimeMultiplierTarget = 0.5f;
    this.mSaturationTarget = 0.1f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    TimeWarpStaff.sCue = AudioManager.Instance.GetCue(Banks.Spells, TimeWarpStaff.SOUND_HASH);
    TimeWarpStaff.sCue.Play();
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if ((double) this.mTTL <= (double) this.mFadeTime)
    {
      this.mTimeMultiplierTarget = 1f;
      this.mSaturationTarget = this.mPlayState.Level.CurrentScene.Saturation;
    }
    iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
    this.mTTL -= iDeltaTime;
    this.mTimeMultiplier += (this.mTimeMultiplierTarget - this.mTimeMultiplier) * iDeltaTime;
    this.mPlayState.TimeMultiplier = this.mTimeMultiplier;
    this.mSaturation += (this.mSaturationTarget - this.mSaturation) * iDeltaTime;
    RenderManager.Instance.Saturation = this.mSaturation;
  }

  public void OnRemove()
  {
    if (TimeWarpStaff.sCue != null)
      TimeWarpStaff.sCue.Stop(AudioStopOptions.AsAuthored);
    TimeWarpStaff.sCue = (Cue) null;
    if (this.mOwner is Magicka.GameLogic.Entities.Character)
      (this.mOwner as Magicka.GameLogic.Entities.Character).TimeWarpModifier = 1f;
    this.mPlayState.TimeMultiplier = 1f;
    if (this.mOwner is Avatar)
      (this.mOwner as Avatar).ResetAfterImages();
    if (this.mPlayState.Level != null)
      RenderManager.Instance.Saturation = this.mPlayState.Level.CurrentScene.Saturation;
    else
      RenderManager.Instance.Saturation = 1f;
  }
}
