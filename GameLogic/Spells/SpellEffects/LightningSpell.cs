// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellEffects.LightningSpell
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells.SpellEffects;

internal class LightningSpell : SpellEffect
{
  private static List<LightningSpell> mCache;
  private bool mAllAround;
  private float mTTL;
  private float mTimeBetweenCasts;
  private int mLightningsToCast;
  private float mRange;
  private float mScale;
  private HitList mHitList;
  private DamageCollection5 mDamages;
  private new double mTimeStamp;

  public static void InitializeCache(int iSize)
  {
    LightningSpell.mCache = new List<LightningSpell>(iSize);
    for (int index = 0; index < iSize; ++index)
      LightningSpell.mCache.Add(new LightningSpell());
  }

  public static SpellEffect GetFromCache()
  {
    LightningSpell fromCache;
    try
    {
      fromCache = LightningSpell.mCache[LightningSpell.mCache.Count - 1];
      LightningSpell.mCache.Remove(fromCache);
      SpellEffect.mPlayState.SpellEffects.Add((SpellEffect) fromCache);
    }
    catch
    {
      fromCache = new LightningSpell();
      SpellEffect.mPlayState.SpellEffects.Add((SpellEffect) fromCache);
    }
    return (SpellEffect) fromCache;
  }

  public static void ReturnToCache(LightningSpell iEffect)
  {
    foreach (Cue mSpellCue in iEffect.mSpellCues)
    {
      if (!mSpellCue.IsStopping || !mSpellCue.IsStopped)
        mSpellCue.Stop(AudioStopOptions.AsAuthored);
    }
    iEffect.mSpellCues.Clear();
    iEffect.mHitList.Clear();
    LightningSpell.mCache.Add(iEffect);
  }

  public LightningSpell()
  {
    this.mHitList = new HitList(256 /*0x0100*/);
    this.mSpellCues = new List<Cue>(8);
  }

  public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastArea(iSpell, iOwner, iFromStaff);
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mFromStaff = false;
    this.mSpell = iSpell;
    this.mAllAround = true;
    this.mTTL = 1f;
    this.mRange = (float) (2.0 + (double) this.mSpell[Elements.Lightning] / 5.0 * 4.0 * 0.5);
    if (iOwner is Character)
      (iOwner as Character).GetSpellRangeModifier(ref this.mRange);
    this.mLightningsToCast = 32 /*0x20*/;
    this.mSpell.CalculateDamage(SpellType.Lightning, CastType.Area, out this.mDamages);
    this.mDamages.MultiplyMagnitude(1f / 16f);
    this.mTimeBetweenCasts = this.mTTL / (float) this.mLightningsToCast;
    this.mTTL = 0.0f;
    this.mScale = (float) (0.800000011920929 + (double) this.mSpell.TotalMagnitude() / 5.0 * 0.40000000596046448);
    this.PlaySound(SpellType.Lightning, CastType.Area, iOwner);
  }

  public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastForce(iSpell, iOwner, iFromStaff);
    if (iOwner is Character && !((iOwner as Character).CurrentState is PanicCastState) && this.mFromStaff)
      this.mFromStaff = false;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mSpell = iSpell;
    this.mTTL = 1f;
    this.mAllAround = false;
    this.mRange = (float) (4.0 + (double) this.mSpell[Elements.Lightning] / 5.0 * 8.0 * 0.5);
    if (iOwner is Character)
      (iOwner as Character).GetSpellRangeModifier(ref this.mRange);
    this.mLightningsToCast = 8;
    this.mSpell.CalculateDamage(SpellType.Lightning, CastType.Force, out this.mDamages);
    this.mDamages.MultiplyMagnitude(1f / (float) this.mLightningsToCast);
    this.mTimeBetweenCasts = this.mTTL / (float) this.mLightningsToCast;
    this.mTTL = 0.0f;
    this.mScale = (float) (0.800000011920929 + (double) this.mSpell.TotalMagnitude() / 5.0 * 0.40000000596046448);
    this.PlaySound(SpellType.Lightning, CastType.Force, iOwner);
  }

  public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    base.CastSelf(iSpell, iOwner, iFromStaff);
    this.mSpell = iSpell;
    this.mTimeStamp = iOwner.PlayState.PlayTime;
    this.mTTL = 0.0f;
    Vector3 position = iOwner.Position;
    Vector3 direction = iOwner.Direction;
    EffectManager.Instance.StartEffect(SpellEffect.SelfCastEffectHash[Defines.ElementIndex(iSpell.Element)], ref position, ref direction, out VisualEffectReference _);
    this.mSpell.CalculateDamage(SpellType.Lightning, CastType.Self, out this.mDamages);
    this.mLightningsToCast = 1;
    int num = (int) iOwner.Damage(this.mDamages, iOwner as Entity, this.mTimeStamp, iOwner.Position);
  }

  public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
  {
    this.CastForce(iSpell, iOwner, iFromStaff);
    this.mCastType = CastType.Weapon;
  }

  public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
  {
    oTurnSpeed = 0.1f;
    this.mTTL -= iDeltaTime;
    if ((double) this.mTTL <= 0.0 && this.mLightningsToCast > 0)
    {
      if (this.CastType != CastType.Self)
      {
        this.mHitList.Clear();
        this.mHitList.Add(iOwner);
        this.mTTL = this.mTimeBetweenCasts;
        --this.mLightningsToCast;
        Vector3 result1 = !this.mFromStaff || !(iOwner is Character) ? iOwner.Direction : (this.CastType != CastType.Weapon ? iOwner.CastSource.Forward : iOwner.WeaponSource.Forward);
        if (iOwner.HasStatus(StatusEffects.Wet) && !iOwner.HasPassiveAbility(Item.PassiveAbilities.WetLightning))
        {
          this.mRange = 2f;
          this.mAllAround = true;
        }
        Vector3 iSource;
        if (this.mAllAround)
        {
          Quaternion result2;
          Quaternion.CreateFromYawPitchRoll((float) ((double) (this.mLightningsToCast % 8) * 3.1415927410125732 / 4.0), 0.0f, 0.0f, out result2);
          Vector3.Transform(ref result1, ref result2, out result1);
          iSource = iOwner.CastSource.Translation;
        }
        else
          iSource = this.CastType != CastType.Weapon ? iOwner.CastSource.Translation : iOwner.WeaponSource.Translation;
        LightningBolt lightning = LightningBolt.GetLightning();
        DamageCollection5 mDamages = this.mDamages;
        lightning.Cast(iOwner, iSource, result1, this.mHitList, this.mSpell.GetColor(), this.mRange, ref mDamages, new Spell?(this.mSpell), SpellEffect.mPlayState);
      }
    }
    else if (this.Active && this.mLightningsToCast <= 0)
    {
      this.DeInitialize(iOwner);
      return false;
    }
    return base.CastUpdate(iDeltaTime, iOwner, out float _);
  }

  public override void DeInitialize(ISpellCaster iOwner)
  {
    if (!this.Active)
      return;
    foreach (Cue mSpellCue in this.mSpellCues)
    {
      if (!mSpellCue.IsStopping || !mSpellCue.IsStopped)
        mSpellCue.Stop(AudioStopOptions.AsAuthored);
    }
    this.mSpellCues.Clear();
    this.Active = false;
    LightningSpell.ReturnToCache(this);
  }
}
