// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.TeslaField
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class TeslaField
{
  private static List<TeslaField> sCache;
  private static readonly int EFFECT_HASH = "ground_arcane".GetHashCodeCustom();
  private static readonly int STATIC_SOUND_HASH = "spell_shield".GetHashCodeCustom();
  private static readonly int SPARK_SOUND_HASH = "spell_shield".GetHashCodeCustom();
  protected VisualEffectReference mEffect;
  protected Matrix mScaleMatrix;
  protected Character mOwner;
  protected HitList mHitList = new HitList(32 /*0x20*/);
  protected Cue mCue;
  private float mTimeAlive;
  private float mLifeTime;
  private float mLastZap;
  private float mRadius;
  private Spell mSpell;
  private DamageCollection5 mDamages;
  private PlayState mPlaystate;
  private bool mItemAbility;

  public static void InitializeCache(int iNrOfItems, PlayState iPlayState)
  {
    TeslaField.sCache = new List<TeslaField>(iNrOfItems);
    for (int index = 0; index < iNrOfItems; ++index)
      TeslaField.sCache.Add(new TeslaField(iPlayState));
  }

  public static TeslaField GetFromCache(PlayState iPlayState)
  {
    if (TeslaField.sCache.Count <= 0)
      return new TeslaField(iPlayState);
    TeslaField fromCache = TeslaField.sCache[TeslaField.sCache.Count - 1];
    TeslaField.sCache.RemoveAt(TeslaField.sCache.Count - 1);
    return fromCache;
  }

  private TeslaField(PlayState iPlayState) => this.mPlaystate = iPlayState;

  public void Initialize(Character iOwner, Spell iSpell)
  {
    this.mItemAbility = false;
    this.mOwner = iOwner;
    this.mRadius = 0.0f;
    this.mSpell = iSpell;
    this.mSpell.CalculateDamage(SpellType.Shield, CastType.None, out this.mDamages);
    this.mDamages.MultiplyMagnitude(0.25f);
    this.mTimeAlive = 0.0f;
    this.mLifeTime = 5f;
  }

  public void Kill() => this.mTimeAlive = this.mLifeTime;

  public bool ItemAbility
  {
    get => this.mItemAbility;
    set => this.mItemAbility = value;
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTimeAlive += iDeltaTime;
    this.mRadius += iDeltaTime * 5f;
    this.mRadius = Math.Min(this.mRadius, 5f);
    if (this.mOwner.PlayState.EntityManager.GetClosestIDamageable((IDamageable) this.mOwner, this.mOwner.Position, this.mRadius, true) != null && (double) this.mRadius > 0.5)
    {
      this.mLastZap = this.mTimeAlive;
      Vector3 direction = this.mOwner.Direction;
      LightningBolt lightning = LightningBolt.GetLightning();
      this.mHitList.Clear();
      if (!this.mOwner.HasStatus(StatusEffects.Wet))
        this.mHitList.Add((Entity) this.mOwner);
      lightning.Cast((ISpellCaster) this.mOwner, this.mOwner.Position, direction, this.mHitList, this.mSpell.GetColor(), this.mRadius, ref this.mDamages, new Spell?(this.mSpell), this.mOwner.PlayState);
      this.mRadius = 0.0f;
    }
    if ((double) this.mTimeAlive <= (double) this.mLifeTime)
      return;
    this.Deinitialize();
  }

  public void Deinitialize() => TeslaField.sCache.Add(this);
}
