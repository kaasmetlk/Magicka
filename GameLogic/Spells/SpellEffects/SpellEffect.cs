// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellEffects.SpellEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells.SpellEffects;

public abstract class SpellEffect
{
  public static readonly int[] SelfCastEffectHash = new int[11]
  {
    "self_earth".GetHashCodeCustom(),
    "self_water".GetHashCodeCustom(),
    "self_cold".GetHashCodeCustom(),
    "self_fire".GetHashCodeCustom(),
    "self_lightning".GetHashCodeCustom(),
    "self_arcane".GetHashCodeCustom(),
    "self_life".GetHashCodeCustom(),
    "self_fire".GetHashCodeCustom(),
    "self_ice".GetHashCodeCustom(),
    "self_steam".GetHashCodeCustom(),
    "self_steam".GetHashCodeCustom()
  };
  protected static readonly Random RANDOM = new Random();
  protected static PlayState mPlayState;
  protected float mMaxTTL;
  protected float mMinTTL;
  protected bool mFromStaff;
  protected List<Cue> mSpellCues;
  protected Spell mSpell;
  protected CastType mCastType;
  protected double mTimeStamp;

  public static void IntializeCaches(PlayState iState, ContentManager iContent)
  {
    SpellEffect.mPlayState = iState;
    PushSpell.IntializeCache(16 /*0x10*/);
    SpraySpell.IntializeCache(16 /*0x10*/);
    ProjectileSpell.InitializeCache(16 /*0x10*/);
    RailGunSpell.InitializeCache(16 /*0x10*/);
    LightningSpell.InitializeCache(16 /*0x10*/);
    ShieldSpell.InitializeCache(16 /*0x10*/);
  }

  public virtual void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    this.Active = true;
    this.mMinTTL = 0.5f;
    this.mMaxTTL = 1.5f;
    this.mFromStaff = iFromStaff;
    this.mCastType = CastType.Area;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
  }

  public virtual void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    this.Active = true;
    this.mMinTTL = 0.5f;
    this.mMaxTTL = 1.5f;
    this.mFromStaff = iFromStaff;
    this.mCastType = CastType.Force;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
  }

  public virtual void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    this.Active = true;
    this.mMinTTL = 0.2f;
    this.mMaxTTL = 0.2f;
    this.mFromStaff = iFromStaff;
    this.mCastType = CastType.Self;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
  }

  public virtual void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    this.Active = true;
    this.mMinTTL = 0.5f;
    this.mMaxTTL = 1.5f;
    this.mFromStaff = iFromStaff;
    this.mCastType = CastType.Weapon;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
  }

  public virtual bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
  {
    oTurnSpeed = 0.75f;
    this.mMinTTL -= iDeltaTime;
    this.mMaxTTL -= iDeltaTime;
    if (this.Active && (double) this.mMinTTL <= 0.0 && iOwner is Avatar && (iOwner as Avatar).CastButton(CastType.None) && !(this is ProjectileSpell))
    {
      this.DeInitialize(iOwner);
      return false;
    }
    if (this.Active && !iOwner.Dead && (!(iOwner is Avatar) || (iOwner as Avatar).CastType != CastType.None || this is ProjectileSpell))
      return true;
    this.DeInitialize(iOwner);
    return false;
  }

  public void Stop(ISpellCaster iOwner)
  {
    this.DeInitialize(iOwner);
    this.Active = false;
  }

  public abstract void DeInitialize(ISpellCaster iOwner);

  public bool Active { get; protected set; }

  public CastType CastType => this.mCastType;

  public Spell Spell => this.mSpell;

  internal virtual void AnimationEnd(ISpellCaster iOwner)
  {
  }

  public double TimeStamp => this.mTimeStamp;

  public void PlaySound(SpellType iSpellType, CastType iCastType, ISpellCaster iOwner)
  {
    if ((double) this.mSpell.TotalMagnitude() <= 0.0)
      throw new Exception("Total magnitude = 0, ERROR!");
    if (this.mSpellCues.Count > 0)
    {
      foreach (Cue mSpellCue in this.mSpellCues)
        mSpellCue.Stop(AudioStopOptions.AsAuthored);
    }
    this.mSpellCues.Clear();
    AudioListener listener = iOwner.PlayState.Camera.Listener;
    AudioEmitter audioEmitter = iOwner.AudioEmitter;
    foreach (Cue cue in this.mSpell.PlaySound(iSpellType, iCastType))
    {
      if (cue != null)
      {
        this.mSpellCues.Add(cue);
        if (cue != null && (cue.IsPrepared || cue.IsPreparing))
        {
          cue.Apply3D(listener, audioEmitter);
          cue.Play();
        }
      }
    }
  }
}
