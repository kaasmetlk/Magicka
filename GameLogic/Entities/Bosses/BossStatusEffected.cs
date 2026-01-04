// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.BossStatusEffected
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public abstract class BossStatusEffected
{
  protected float mHitPoints;
  protected float mMaxHitPoints;
  protected StatusEffects mCurrentStatusEffects;
  protected StatusEffect[] mStatusEffects = new StatusEffect[9];
  protected Resistance[] mResistances = new Resistance[11];
  protected float mFireDamageAccumulation;
  protected float mFireDamageAccumulationTimer = 0.25f;
  protected float mPoisonDamageAccumulation;
  protected float mPoisonDamageAccumulationTimer = 0.25f;
  protected float mHealingAccumulation;
  protected float mHealingAccumulationTimer = 0.25f;
  protected int mLastDamageIndex;
  protected float mLastDamageAmount;
  protected Elements mLastDamageElement;
  protected float mTimeSinceLastDamage;
  protected float mTimeSinceLastStatusDamage;
  protected float mDryTimer;
  protected VisualEffectReference mDryingEffect;

  internal Resistance[] Resistances => this.mResistances;

  protected abstract BossDamageZone Entity { get; }

  protected abstract float Radius { get; }

  protected abstract float Length { get; }

  public abstract bool Dead { get; }

  protected abstract int BloodEffect { get; }

  protected abstract Vector3 NotifierTextPostion { get; }

  protected virtual void UpdateDamage(float iDeltaTime)
  {
    Vector3 notifierTextPostion = this.NotifierTextPostion;
    this.mTimeSinceLastDamage += iDeltaTime;
    this.mTimeSinceLastStatusDamage += iDeltaTime;
    if (this.mLastDamageIndex >= 0)
    {
      if ((double) this.mTimeSinceLastDamage > 0.33300000429153442 || this.Dead)
      {
        DamageNotifyer.Instance.ReleasNumber(this.mLastDamageIndex);
        this.mLastDamageIndex = -1;
      }
      else
        DamageNotifyer.Instance.UpdateNumberPosition(this.mLastDamageIndex, ref notifierTextPostion);
    }
    this.mHealingAccumulationTimer -= iDeltaTime;
    if ((double) this.mHealingAccumulationTimer <= 0.0)
    {
      ++this.mHealingAccumulationTimer;
      if ((double) this.mHealingAccumulation != 0.0)
      {
        DamageNotifyer.Instance.AddNumber(this.mHealingAccumulation, ref notifierTextPostion, 0.7f, false);
        this.mHealingAccumulation = 0.0f;
        this.mTimeSinceLastStatusDamage = 0.0f;
      }
    }
    this.mFireDamageAccumulationTimer -= iDeltaTime;
    if ((double) this.mFireDamageAccumulationTimer <= 0.0)
    {
      ++this.mFireDamageAccumulationTimer;
      if ((double) this.mFireDamageAccumulation != 0.0)
      {
        DamageNotifyer.Instance.AddNumber(this.mFireDamageAccumulation, ref notifierTextPostion, 0.7f, false);
        this.mFireDamageAccumulation = 0.0f;
        this.mTimeSinceLastStatusDamage = 0.0f;
      }
    }
    this.mPoisonDamageAccumulationTimer -= iDeltaTime;
    if ((double) this.mPoisonDamageAccumulationTimer > 0.0)
      return;
    ++this.mPoisonDamageAccumulationTimer;
    if ((double) this.mPoisonDamageAccumulation == 0.0)
      return;
    DamageNotifyer.Instance.AddNumber(this.mPoisonDamageAccumulation, ref notifierTextPostion, 0.7f, false);
    this.mPoisonDamageAccumulation = 0.0f;
    this.mTimeSinceLastStatusDamage = 0.0f;
  }

  protected virtual void UpdateStatusEffects(float iDeltaTime)
  {
    this.mDryTimer -= iDeltaTime;
    StatusEffects statusEffects = StatusEffects.None;
    if (this.Dead)
    {
      for (int index = 0; index < this.mStatusEffects.Length; ++index)
      {
        this.mStatusEffects[index].Stop();
        this.mStatusEffects[index] = new StatusEffect();
      }
    }
    else
    {
      for (int index = 0; index < this.mStatusEffects.Length; ++index)
      {
        this.mStatusEffects[index].Update(iDeltaTime, (IStatusEffected) this.Entity);
        if (this.mStatusEffects[index].Dead)
        {
          this.mStatusEffects[index].Stop();
          this.mStatusEffects[index] = new StatusEffect();
        }
        else if (this.mStatusEffects[index].DamageType == StatusEffects.Wet)
        {
          if ((double) this.mStatusEffects[index].Magnitude >= 1.0)
            statusEffects |= this.mStatusEffects[index].DamageType;
        }
        else
          statusEffects |= this.mStatusEffects[index].DamageType;
      }
    }
    this.mCurrentStatusEffects = statusEffects;
  }

  public DamageResult AddStatusEffect(StatusEffect iStatusEffect)
  {
    DamageResult damageResult = DamageResult.None;
    if (!iStatusEffect.Dead)
    {
      bool flag = false;
      switch (iStatusEffect.DamageType)
      {
        case StatusEffects.Burning:
          if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Dead || (double) this.mDryTimer > 0.0)
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)] = new StatusEffect();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = new StatusEffect();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = new StatusEffect();
            flag = true;
            break;
          }
          break;
        case StatusEffects.Wet:
          if (this.HasStatus(StatusEffects.Burning) || (double) this.mDryTimer > 0.0)
          {
            int index = StatusEffect.StatusIndex(StatusEffects.Burning);
            this.mStatusEffects[index].Stop();
            this.mStatusEffects[index] = new StatusEffect();
            flag = true;
          }
          if (this.HasStatus(StatusEffects.Greased))
          {
            int index = StatusEffect.StatusIndex(StatusEffects.Greased);
            this.mStatusEffects[index].Stop();
            this.mStatusEffects[index] = new StatusEffect();
            break;
          }
          break;
        case StatusEffects.Cold:
          float magnitude = iStatusEffect.Magnitude;
          if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead || (double) this.mDryTimer > 0.0)
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)] = new StatusEffect();
            this.mDryTimer = 0.9f;
            flag = true;
          }
          float iMagnitude = magnitude * this.mResistances[Defines.ElementIndex(Elements.Cold)].Multiplier;
          if (this.HasStatus(StatusEffects.Wet))
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = new StatusEffect(StatusEffects.Frozen, 0.0f, iMagnitude, this.Length, this.Radius);
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = new StatusEffect();
          }
          if (this.HasStatus(StatusEffects.Frozen))
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude += iMagnitude;
            iMagnitude = 0.0f;
          }
          iStatusEffect.Magnitude = iMagnitude;
          break;
        case StatusEffects.Poisoned:
          iStatusEffect.Magnitude *= this.mResistances[Defines.ElementIndex(Elements.Poison)].Multiplier;
          break;
        case StatusEffects.Healing:
          if (this.HasStatus(StatusEffects.Poisoned))
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)] = new StatusEffect();
            break;
          }
          break;
        case StatusEffects.Greased:
          if (this.HasStatus(StatusEffects.Wet))
          {
            int index = StatusEffect.StatusIndex(StatusEffects.Wet);
            this.mStatusEffects[index].Stop();
            this.mStatusEffects[index] = new StatusEffect();
            break;
          }
          break;
      }
      if (!flag)
      {
        int index = StatusEffect.StatusIndex(iStatusEffect.DamageType);
        this.mStatusEffects[index] = this.mStatusEffects[index] + iStatusEffect;
        damageResult |= DamageResult.Statusadded;
      }
      else
        damageResult |= DamageResult.Statusremoved;
    }
    return damageResult;
  }

  public bool HasStatus(StatusEffects iStatus) => (this.mCurrentStatusEffects & iStatus) == iStatus;

  public StatusEffect[] GetStatusEffects() => this.mStatusEffects;

  public float StatusMagnitude(StatusEffects iStatus)
  {
    return this.mStatusEffects[StatusEffect.StatusIndex(iStatus)].Magnitude;
  }

  protected void Damage(float iDamage, Elements iElement)
  {
    if ((iElement & Elements.Fire) == Elements.Fire && this.HasStatus(StatusEffects.Greased))
      iDamage *= 2f;
    this.mHitPoints -= iDamage;
    switch (iElement)
    {
      case Elements.Fire:
        this.mFireDamageAccumulation += iDamage;
        break;
      case Elements.Life:
        if ((double) this.mHitPoints < (double) this.mMaxHitPoints)
        {
          this.mHealingAccumulation += iDamage;
          break;
        }
        break;
      case Elements.Poison:
        this.mPoisonDamageAccumulation += iDamage;
        break;
    }
    if ((double) this.mHitPoints <= (double) this.mMaxHitPoints)
      return;
    this.mHitPoints = this.mMaxHitPoints;
  }

  protected virtual DamageResult Damage(
    Magicka.GameLogic.Damage iDamage,
    Magicka.GameLogic.Entities.Entity iAttacker,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if (this.Dead)
      return DamageResult.Deflected;
    Magicka.GameLogic.Damage damage = iDamage;
    DamageResult damageResult = DamageResult.None;
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      Elements elements = Defines.ElementFromIndex(iIndex);
      if ((damage.Element & elements) == elements)
      {
        if (damage.Element == Elements.Earth && (double) this.mResistances[iIndex].Modifier != 0.0)
          damage.Amount = (float) (int) Math.Max(damage.Amount + this.mResistances[iIndex].Modifier, 0.0f);
        else
          damage.Amount += (float) (int) this.mResistances[iIndex].Modifier;
        num1 += this.mResistances[iIndex].Multiplier;
        ++num2;
      }
    }
    if ((double) num2 != 0.0)
      damage.Magnitude *= num1 / num2;
    if ((double) Math.Abs(damage.Magnitude) <= 1.4012984643248171E-45)
      damageResult |= DamageResult.Deflected;
    if ((damageResult & DamageResult.Deflected) == DamageResult.Deflected)
      return damageResult;
    if ((damage.AttackProperty & AttackProperties.Status) == AttackProperties.Status && (double) Math.Abs(num1) > 1.4012984643248171E-45)
    {
      if ((damage.Element & Elements.Fire) == Elements.Fire && (double) this.mResistances[Spell.ElementIndex(Elements.Fire)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Burning, damage.Amount, damage.Magnitude, this.Length, this.Radius));
      if ((damage.Element & Elements.Cold) == Elements.Cold && (double) this.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Cold, damage.Amount, damage.Magnitude, this.Length, this.Radius));
      if ((damage.Element & Elements.Water) == Elements.Water && (double) this.mResistances[Spell.ElementIndex(Elements.Water)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, this.Length, this.Radius));
      if ((damage.Element & Elements.Poison) == Elements.Poison && (double) this.mResistances[Spell.ElementIndex(Elements.Poison)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Poisoned, damage.Amount, damage.Magnitude, this.Length, this.Radius));
      if ((damage.Element & Elements.Life) == Elements.Life && (double) this.mResistances[Spell.ElementIndex(Elements.Life)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Healing, damage.Amount, damage.Magnitude, this.Length, this.Radius));
      if ((damage.Element & Elements.Steam) == Elements.Steam && (double) this.mResistances[Spell.ElementIndex(Elements.Steam)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, this.Length, this.Radius));
    }
    if ((damage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage)
    {
      if ((damage.Element & Elements.Lightning) == Elements.Lightning && this.HasStatus(StatusEffects.Wet))
        damage.Amount *= 2f;
      if ((damage.Element & Elements.Life) == Elements.Life)
      {
        this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude -= damage.Magnitude;
        if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude <= 0.0)
          this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
      }
      if ((damage.Element & Elements.PhysicalElements) != Elements.None)
      {
        if (this.HasStatus(StatusEffects.Frozen))
        {
          damage.Amount = Math.Max(damage.Amount - 200f, 0.0f);
          damage.Magnitude = Math.Max(1f, damage.Magnitude);
          damage.Amount *= 3f;
        }
        else if (GlobalSettings.Instance.BloodAndGore == SettingOptions.On)
        {
          Vector3 iPosition = iAttackPosition;
          Vector3 right = Vector3.Right;
          EffectManager.Instance.StartEffect(this.BloodEffect, ref iPosition, ref right, out VisualEffectReference _);
        }
      }
      damage.Amount *= damage.Magnitude;
      this.mHitPoints -= damage.Amount;
      if ((damage.AttackProperty & AttackProperties.Piercing) != (AttackProperties) 0 && (double) damage.Magnitude > 0.0 && (double) damage.Amount > 0.0)
        damageResult |= DamageResult.Pierced;
      if ((double) damage.Amount > 0.0)
        damageResult |= DamageResult.Damaged;
      if ((double) damage.Amount == 0.0)
        damageResult |= DamageResult.Deflected;
      if ((double) damage.Amount < 0.0)
        damageResult |= DamageResult.Healed;
      damageResult |= DamageResult.Hit;
      if ((double) damage.Amount != 0.0)
        this.mTimeSinceLastDamage = 0.0f;
      if (Defines.FeatureNotify(iFeatures))
      {
        if (this.mLastDamageIndex >= 0)
        {
          DamageNotifyer.Instance.AddToNumber(this.mLastDamageIndex, damage.Amount);
        }
        else
        {
          if (this.mLastDamageIndex >= 0)
            DamageNotifyer.Instance.ReleasNumber(this.mLastDamageIndex);
          this.mLastDamageAmount = damage.Amount;
          this.mLastDamageElement = damage.Element;
          Vector3 notifierTextPostion = this.NotifierTextPostion;
          this.mLastDamageIndex = DamageNotifyer.Instance.AddNumber(damage.Amount, ref notifierTextPostion, 0.4f, true);
        }
      }
    }
    if ((double) this.mHitPoints > (double) this.mMaxHitPoints)
      this.mHitPoints = this.mMaxHitPoints;
    if ((double) damage.Amount == 0.0)
      damageResult |= DamageResult.Deflected;
    if ((double) this.mHitPoints <= 0.0)
      damageResult |= DamageResult.Killed;
    return damageResult;
  }
}
