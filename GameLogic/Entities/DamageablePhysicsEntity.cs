// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.DamageablePhysicsEntity
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Statistics;
using Magicka.Graphics.Lights;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Lights;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal class DamageablePhysicsEntity(PlayState iPlayState) : 
  PhysicsEntity(iPlayState),
  IStatusEffected,
  IDamageable
{
  protected float mMaxHitPoints;
  protected float mHitPoints;
  private List<GibReference> mGibs = new List<GibReference>();
  private List<PhysAnimationControl> mAnimations = new List<PhysAnimationControl>(10);
  private float mOldHitPoints;
  private float mOldPercentage;
  private Resistance[] mResistances;
  private bool mCanHasStatus;
  private float mVolume;
  private int mOnDeath;
  private int mOnDamage;
  protected DynamicLight mStatusEffectLight;
  protected StatusEffects mCurrentStatusEffects;
  protected StatusEffect[] mStatusEffects = new StatusEffect[9];
  protected float mRestingHealthTimer;
  protected float mLastHitpoints;
  protected static readonly Random sRandom = new Random();

  public override void Initialize(
    PhysicsEntityTemplate iTemplate,
    Matrix iStartTransform,
    int iUniqueID)
  {
    base.Initialize(iTemplate, iStartTransform, iUniqueID);
    this.mMaxHitPoints = this.mOldHitPoints = this.mHitPoints = (float) iTemplate.MaxHitpoints;
    this.mOldPercentage = 1f;
    this.mGibs.Clear();
    for (int index = 0; index < iTemplate.Gibs.Length; ++index)
      this.mGibs.Add(iTemplate.Gibs[index]);
    for (int index = 0; index < this.mStatusEffects.Length; ++index)
      this.mStatusEffects[index] = new StatusEffect();
    this.mResistances = iTemplate.Resistances;
    this.mCanHasStatus = iTemplate.CanHaveStatus;
    Vector3 sides = iTemplate.Box.Sides;
    this.mVolume = sides.X * sides.Y * sides.Z;
  }

  public float ResistanceAgainst(Elements iElement)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      Elements elements = Defines.ElementFromIndex(iIndex);
      if ((iElement & elements) != Elements.None)
      {
        float multiplier = this.mResistances[iIndex].Multiplier;
        float modifier = this.mResistances[iIndex].Modifier;
        if (this.HasStatus(StatusEffects.Frozen) && (iElement & Elements.Earth) != Elements.None)
          modifier -= 350f;
        if (this.HasStatus(StatusEffects.Greased) && (iElement & Elements.Fire) != Elements.None)
          multiplier *= 2f;
        num1 += modifier;
        num2 += multiplier;
      }
    }
    return 1f - MathHelper.Clamp(num1 / 300f + num2, -1f, 1f);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mAnimations.Count > 0 && (double) Math.Abs(this.mOldHitPoints - this.mHitPoints) > 1.4012984643248171E-45)
    {
      float num1 = (float) (1.0 - (double) this.mHitPoints / (double) this.mMaxHitPoints);
      foreach (PhysAnimationControl mAnimation in this.mAnimations)
      {
        float num2 = mAnimation.Start + (mAnimation.End - mAnimation.Start) * this.mOldPercentage;
        float num3 = mAnimation.Start + (mAnimation.End - mAnimation.Start) * num1;
        if ((double) num2 > (double) num3)
          mAnimation.AnimatedLevelPart.Play(mAnimation.Children, num3, num2, -mAnimation.Speed, false, false);
        else
          mAnimation.AnimatedLevelPart.Play(mAnimation.Children, num2, num3, mAnimation.Speed, false, false);
      }
      this.mOldHitPoints = this.mHitPoints;
      this.mOldPercentage = num1;
    }
    this.UpdateStatusEffects(iDeltaTime);
    if ((double) this.mLastHitpoints != (double) this.mHitPoints || this.mCurrentStatusEffects != StatusEffects.None)
      this.mRestingHealthTimer = 1f;
    else
      this.mRestingHealthTimer -= iDeltaTime;
    base.Update(iDataChannel, iDeltaTime);
  }

  private void UpdateStatusEffects(float iDeltaTime)
  {
    StatusEffects statusEffects = StatusEffects.None;
    if (this.Dead)
    {
      for (int index = 0; index < this.mStatusEffects.Length; ++index)
      {
        switch (this.mStatusEffects[index].DamageType)
        {
          case StatusEffects.Burning:
          case StatusEffects.Wet:
          case StatusEffects.Burning | StatusEffects.Wet:
          case StatusEffects.Frozen:
            this.mStatusEffects[index].Stop();
            this.mStatusEffects[index] = new StatusEffect();
            continue;
          default:
            goto case StatusEffects.Burning;
        }
      }
    }
    else
    {
      for (int index = 0; index < this.mStatusEffects.Length; ++index)
      {
        this.mStatusEffects[index].Update(iDeltaTime, (IStatusEffected) this);
        if (this.mStatusEffects[index].Dead)
        {
          switch (this.mStatusEffects[index].DamageType)
          {
            case StatusEffects.Burning:
            case StatusEffects.Wet:
            case StatusEffects.Burning | StatusEffects.Wet:
            case StatusEffects.Frozen:
              this.mStatusEffects[index].Stop();
              this.mStatusEffects[index] = new StatusEffect();
              continue;
            default:
              goto case StatusEffects.Burning;
          }
        }
        else
          statusEffects |= this.mStatusEffects[index].DamageType;
      }
    }
    if (this.mStatusEffectLight != null)
      this.mStatusEffectLight.Position = this.Position;
    if (this.HasStatus(StatusEffects.Burning))
    {
      if (this.mStatusEffectLight == null)
      {
        this.mStatusEffectLight = DynamicLight.GetCachedLight();
        this.mStatusEffectLight.Initialize(Vector3.Zero, new Vector3(1f, 0.4f, 0.0f), 2.5f, 5f, 1f, 0.5f);
        this.mStatusEffectLight.VariationType = LightVariationType.Candle;
        this.mStatusEffectLight.VariationSpeed = 4f;
        this.mStatusEffectLight.VariationAmount = 0.2f;
        this.mStatusEffectLight.Enable();
      }
      this.mStatusEffectLight.Intensity = 3f + (float) Math.Sqrt((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Magnitude);
    }
    else if (this.mStatusEffectLight != null)
    {
      this.mStatusEffectLight.Stop(false);
      this.mStatusEffectLight = (DynamicLight) null;
    }
    this.mCurrentStatusEffects = statusEffects;
  }

  public override void Deinitialize()
  {
    base.Deinitialize();
    if (this.mStatusEffectLight == null)
      return;
    this.mStatusEffectLight.Disable();
    this.mStatusEffectLight = (DynamicLight) null;
  }

  public int OnDeath
  {
    get => this.mOnDeath;
    set => this.mOnDeath = value;
  }

  public int OnDamage
  {
    get => this.mOnDamage;
    set => this.mOnDamage = value;
  }

  public bool HasStatus(StatusEffects iStatus) => (this.mCurrentStatusEffects & iStatus) == iStatus;

  public StatusEffect[] GetStatusEffects() => this.mStatusEffects;

  public float StatusMagnitude(StatusEffects iStatus)
  {
    int index = StatusEffect.StatusIndex(iStatus);
    return !this.mStatusEffects[index].Dead ? this.mStatusEffects[index].Magnitude : 0.0f;
  }

  public override bool Dead => (double) this.mHitPoints <= 0.0;

  public void SetSlow()
  {
  }

  public void Damage(float iDamage, Elements iElement)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.mHitPoints -= iDamage;
    if ((double) this.mHitPoints > 0.0)
      return;
    this.SpawnGibs();
  }

  public float Volume => this.mVolume;

  public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    Vector3 vector3_1 = new Vector3();
    Box primitiveNewWorld = this.mCollision.GetPrimitiveNewWorld(0) as Box;
    float pfLParam;
    float num = Distance.SegmentBoxDistanceSq(out pfLParam, out vector3_1.X, out vector3_1.Y, out vector3_1.Z, iSeg, primitiveNewWorld);
    Vector3 vector3_2 = vector3_1 + primitiveNewWorld.Position;
    iSeg.GetPoint(pfLParam, out oPosition);
    return (double) num <= (double) iSegmentRadius * (double) iSegmentRadius;
  }

  public virtual DamageResult InternalDamage(
    DamageCollection5 iDamages,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult damageResult1 = this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    DamageResult damageResult2 = this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    DamageResult damageResult3 = this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    DamageResult damageResult4 = this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    DamageResult damageResult5 = this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    return (damageResult1 | damageResult2 | damageResult3 | damageResult4 | damageResult5) & ~(damageResult1 & damageResult2 & damageResult3 & damageResult4 & damageResult5 ^ DamageResult.Deflected);
  }

  public virtual DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult iResult = DamageResult.None;
    if (this.Dead)
      return iResult;
    if ((iDamage.AttackProperty & AttackProperties.Knockback) == AttackProperties.Knockback)
    {
      Vector3 vector3 = iAttackPosition;
      Vector3 result = this.Position;
      Vector3.Subtract(ref result, ref vector3, out result);
      result.Y = 0.0f;
      if ((double) result.LengthSquared() > 1.4012984643248171E-45)
      {
        result.Normalize();
        Vector3 iVelocity = this.CalcImpulseVelocity(result, 0.6980619f, iDamage.Amount, iDamage.Magnitude);
        if ((double) iVelocity.LengthSquared() > 9.9999999747524271E-07)
        {
          if (Defines.FeatureKnockback(iFeatures))
            this.AddImpulseVelocity(ref iVelocity);
          iResult |= DamageResult.Knockedback;
        }
      }
    }
    else if ((iDamage.AttackProperty & AttackProperties.Pushed) == AttackProperties.Pushed)
    {
      Vector3 vector3 = iAttackPosition;
      Vector3 result = this.Position;
      Vector3.Subtract(ref result, ref vector3, out result);
      result.Y = 0.0f;
      result.Normalize();
      Vector3 iVelocity = this.CalcImpulseVelocity(result, 0.17453292f, iDamage.Amount, iDamage.Magnitude);
      if ((double) iVelocity.LengthSquared() > 9.9999999747524271E-07)
      {
        if (Defines.FeatureKnockback(iFeatures))
          this.AddImpulseVelocity(ref iVelocity);
        iResult |= DamageResult.Pushed;
      }
    }
    iDamage.ApplyResistancesInclusive(this.mResistances, (Resistance[]) null, (IList<BuffStorage>) null, this.mCurrentStatusEffects);
    if ((iDamage.AttackProperty & AttackProperties.Status) == AttackProperties.Status)
    {
      if ((iDamage.Element & Elements.Fire) == Elements.Fire)
        iResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Burning, iDamage.Amount, iDamage.Magnitude, 0.0f, this.Radius));
      if ((iDamage.Element & Elements.Cold) == Elements.Cold)
        iResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Cold, iDamage.Amount, iDamage.Magnitude, 0.0f, this.Radius));
      if ((iDamage.Element & Elements.Water) == Elements.Water)
        iResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, iDamage.Amount, iDamage.Magnitude, 0.0f, this.Radius));
      if ((iDamage.Element & Elements.Poison) == Elements.Poison)
        iResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Poisoned, iDamage.Amount, iDamage.Magnitude, 0.0f, this.Radius));
    }
    if ((iDamage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage)
    {
      if (iDamage.Element == Elements.Lightning && this.HasStatus(StatusEffects.Wet))
        iDamage.Amount *= 3f;
      if ((iDamage.Element & Elements.Life) == Elements.Life)
      {
        this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude -= iDamage.Magnitude;
        if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude <= 0.0)
          this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
      }
      if ((iDamage.Element & Elements.PhysicalElements) != Elements.None && this.HasStatus(StatusEffects.Frozen))
      {
        iDamage.Amount = Math.Max(iDamage.Amount - 200f, 0.0f);
        iDamage.Magnitude = Math.Max(1f, iDamage.Magnitude);
        iDamage.Amount *= 3f;
      }
      iDamage.Amount = (float) (int) ((double) iDamage.Amount * (double) iDamage.Magnitude);
      if (Defines.FeatureDamage(iFeatures))
        this.mHitPoints -= iDamage.Amount;
      if ((iDamage.AttackProperty & AttackProperties.Piercing) != (AttackProperties) 0 && (double) iDamage.Magnitude > 0.0 && (double) iDamage.Amount > 0.0)
        iResult |= DamageResult.Pierced;
      if ((double) iDamage.Amount > 0.0)
        iResult |= DamageResult.Damaged;
      if ((double) iDamage.Amount == 0.0)
        iResult |= DamageResult.Deflected;
      if ((double) iDamage.Amount < 0.0)
        iResult |= DamageResult.Healed;
    }
    if ((double) this.mHitPoints > (double) this.mMaxHitPoints)
      this.mHitPoints = this.mMaxHitPoints;
    if ((double) iDamage.Amount == 0.0)
      iResult |= DamageResult.Deflected;
    if ((double) this.mHitPoints <= 0.0)
    {
      iResult |= DamageResult.Killed;
      this.SpawnGibs();
    }
    if ((iResult & DamageResult.Damaged) != DamageResult.None && this.mOnDamage != 0 && NetworkManager.Instance.State != NetworkState.Client)
      this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDamage, (Character) null, false);
    StatisticsManager.Instance.AddDamageEvent(this.mPlayState, iAttacker as IDamageable, (IDamageable) this, iTimeStamp, iDamage, iResult);
    return iResult;
  }

  public virtual DamageResult AddStatusEffect(StatusEffect iStatusEffect)
  {
    DamageResult damageResult = DamageResult.None;
    if (!iStatusEffect.Dead)
    {
      bool flag = false;
      switch (iStatusEffect.DamageType)
      {
        case StatusEffects.Burning:
          if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Dead)
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)] = new StatusEffect();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = new StatusEffect();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude -= iStatusEffect.Magnitude;
            if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude <= 0.0)
            {
              this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Stop();
              this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = new StatusEffect();
            }
            flag = true;
            break;
          }
          break;
        case StatusEffects.Wet:
          if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead)
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)] = new StatusEffect();
            flag = true;
            break;
          }
          break;
        case StatusEffects.Cold:
          float magnitude = iStatusEffect.Magnitude;
          if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead)
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)] = new StatusEffect();
            flag = true;
          }
          float num = magnitude * (this.mResistances[Defines.ElementIndex(Elements.Cold)].Multiplier * (float) (0.5 + 0.5 * (500.0 / (double) this.Body.Mass)));
          if (this.HasStatus(StatusEffects.Wet))
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = new StatusEffect(StatusEffects.Frozen, 0.0f, 0.5f, 0.0f, this.mRadius);
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = new StatusEffect();
          }
          if (this.HasStatus(StatusEffects.Frozen))
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude += num;
            num = 0.0f;
          }
          iStatusEffect.Magnitude = num;
          break;
        case StatusEffects.Poisoned:
          iStatusEffect.Magnitude *= this.mResistances[Defines.ElementIndex(Elements.Poison)].Multiplier;
          break;
      }
      if (!flag)
      {
        int index = StatusEffect.StatusIndex(iStatusEffect.DamageType);
        if (iStatusEffect.DamageType == StatusEffects.Burning)
          this.HasStatus(StatusEffects.Burning);
        if (iStatusEffect.DamageType == StatusEffects.Wet)
          this.HasStatus(StatusEffects.Wet);
        if (iStatusEffect.DamageType == StatusEffects.Cold)
          this.HasStatus(StatusEffects.Cold);
        if (iStatusEffect.DamageType == StatusEffects.Frozen)
          this.HasStatus(StatusEffects.Frozen);
        this.mStatusEffects[index] = this.mStatusEffects[index] + iStatusEffect;
        damageResult |= DamageResult.Statusadded;
      }
      else
        damageResult |= DamageResult.Statusremoved;
    }
    return damageResult;
  }

  public override void Kill()
  {
    this.mHitPoints = 0.0f;
    this.SpawnGibs();
    if (this.mStatusEffectLight == null)
      return;
    this.mStatusEffectLight.Stop(false);
  }

  public void OverKill()
  {
    this.mHitPoints = 0.0f;
    this.SpawnGibs();
    if (this.mStatusEffectLight == null)
      return;
    this.mStatusEffectLight.Stop(false);
  }

  public float HitPoints => this.mHitPoints;

  public float MaxHitPoints => this.mMaxHitPoints;

  protected bool RestingHealth => (double) this.mRestingHealthTimer < 0.0;

  public void SpawnGibs()
  {
    if (this.mOnDeath != 0 && NetworkManager.Instance.State != NetworkState.Client)
      this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDeath, (Character) null, false);
    if (this.mConditions != null)
      this.mConditions.ExecuteAll((Entity) this, (Entity) null, ref new EventCondition()
      {
        EventConditionType = EventConditionType.Death
      }, out DamageResult _);
    if (this.mGibs.Count <= 0)
      return;
    Vector3 position = this.Position;
    for (int index = 0; index < this.mGibs.Count; ++index)
    {
      Gib fromCache = Gib.GetFromCache();
      if (fromCache != null)
      {
        float scaleFactor = fromCache.Mass * 1.25f;
        float num1 = (double) this.Radius >= 0.20000000298023224 ? this.Radius : 0.2f;
        float num2 = 6.28318548f * (float) DamageablePhysicsEntity.sRandom.NextDouble();
        float num3 = (float) Math.Acos(2.0 * DamageablePhysicsEntity.sRandom.NextDouble() - 1.0);
        Vector3 result1 = new Vector3(num1 * (float) Math.Cos((double) num2) * (float) Math.Sin((double) num3), 3f * num1 * (float) Math.Sin((double) num2) * (float) Math.Sin((double) num3), num1 * (float) Math.Cos((double) num3));
        Vector3.Normalize(ref result1, out result1);
        Vector3 result2;
        Vector3.Multiply(ref result1, scaleFactor, out result2);
        result2.Y = Math.Abs(result2.Y);
        float iTTL = (float) (10.0 + DamageablePhysicsEntity.sRandom.NextDouble() * 10.0);
        fromCache.Initialize(this.mGibs[index].mModel, this.mGibs[index].mMass, this.mGibs[index].mScale, position, result2, iTTL, (Entity) this, BloodType.none, this.mGibTrailEffect, this.HasStatus(StatusEffects.Frozen));
        Vector3 result3 = new Vector3();
        Vector3.Negate(ref result1, out result3);
        Vector3.Multiply(ref result3, 5f, out result3);
        fromCache.Body.AngularVelocity = result3;
        this.mPlayState.EntityManager.AddEntity((Entity) fromCache);
      }
      else
        break;
    }
    this.mGibs.Clear();
  }

  protected override unsafe void INetworkUpdate(ref EntityUpdateMessage iMsg)
  {
    base.INetworkUpdate(ref iMsg);
    if ((iMsg.Features & EntityFeatures.Damageable) != EntityFeatures.None)
      this.mHitPoints = iMsg.HitPoints;
    if ((iMsg.Features & EntityFeatures.StatusEffected) == EntityFeatures.None)
      return;
    this.mCurrentStatusEffects = iMsg.StatusEffects;
    fixed (float* numPtr1 = iMsg.StatusEffectMagnitude)
      fixed (float* numPtr2 = iMsg.StatusEffectDPS)
      {
        for (int iIndex = 0; iIndex < 9; ++iIndex)
        {
          StatusEffects iStatus = StatusEffect.StatusFromIndex(iIndex);
          if ((double) this.StatusMagnitude(iStatus) > 0.0)
          {
            if ((double) numPtr1[iIndex] > 0.0)
            {
              this.mStatusEffects[iIndex].Magnitude = numPtr1[iIndex];
              this.mStatusEffects[iIndex].DPS = numPtr2[iIndex];
            }
            else
            {
              this.mStatusEffects[iIndex].Stop();
              this.mStatusEffects[iIndex] = new StatusEffect();
            }
          }
          else if ((double) numPtr1[iIndex] > 0.0)
          {
            int num = (int) this.AddStatusEffect(new StatusEffect(iStatus, numPtr2[iIndex], numPtr1[iIndex], 0.0f, this.Radius));
          }
        }
      }
  }

  protected override unsafe void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    base.IGetNetworkUpdate(out oMsg, iPrediction);
    if (this.RestingHealth)
      return;
    oMsg.Features |= EntityFeatures.Damageable;
    oMsg.HitPoints = this.mHitPoints;
    this.mLastHitpoints = this.mHitPoints;
    oMsg.Features |= EntityFeatures.StatusEffected;
    oMsg.StatusEffects = this.mCurrentStatusEffects;
    fixed (float* numPtr1 = oMsg.StatusEffectMagnitude)
      fixed (float* numPtr2 = oMsg.StatusEffectDPS)
      {
        for (int index = 0; index < 9; ++index)
        {
          numPtr1[index] = this.mStatusEffects[index].Magnitude;
          numPtr2[index] = this.mStatusEffects[index].DPS;
        }
      }
  }

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
  }

  internal void AddAnimation(PhysAnimationControl physAnimCont)
  {
    this.mAnimations.Add(physAnimCont);
  }
}
