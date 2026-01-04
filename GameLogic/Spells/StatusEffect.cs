// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.StatusEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead.ParticleEffects;
using System;

#nullable disable
namespace Magicka.GameLogic.Spells;

public struct StatusEffect : IEquatable<StatusEffect>
{
  private static readonly double ONEOVERLN2 = 1.0 / Math.Log(2.0);
  private static readonly int POISON_VOMIT = "statuseffect_poison".GetHashCodeCustom();
  public static readonly int[] EFFECTS_STATUS = new int[8]
  {
    0,
    "statuseffect_wet".GetHashCodeCustom(),
    "statuseffect_frozen".GetHashCodeCustom(),
    0,
    "statuseffect_burning_arcane".GetHashCodeCustom(),
    "statuseffect_healing".GetHashCodeCustom(),
    "statuseffect_wet".GetHashCodeCustom(),
    "drying_steam".GetHashCodeCustom()
  };
  public static readonly int[] EFFECT_BURNING = new int[3]
  {
    "statuseffect_burning".GetHashCodeCustom(),
    "statuseffect_burning_2".GetHashCodeCustom(),
    "statuseffect_burning_3".GetHashCodeCustom()
  };
  public static readonly int[] EFFECT_COLD = new int[3]
  {
    "statuseffect_cold".GetHashCodeCustom(),
    "statuseffect_cold_2".GetHashCodeCustom(),
    "statuseffect_cold_3".GetHashCodeCustom()
  };
  public static readonly int[] BLEED_EFFECT = new int[6]
  {
    "gore_bleed_regular".GetHashCodeCustom(),
    "gore_bleed_green".GetHashCodeCustom(),
    "gore_bleed_black".GetHashCodeCustom(),
    "gore_bleed_wood".GetHashCodeCustom(),
    "gore_bleed_insect".GetHashCodeCustom(),
    "".GetHashCode()
  };
  private VisualEffect mEmitterA;
  private VisualEffectReference mVomitEffect;
  private float mDamageRest;
  private float mMagnitude;
  private int mParticleMagntiude;
  private float mDPS;
  private float mParticleTimer;
  private float mRadius;
  private float mLength;
  private StatusEffects mDamageType;

  public StatusEffect(
    StatusEffects iStatus,
    float iDPS,
    float iMagnitude,
    float iLength,
    float iRadius)
  {
    this.mRadius = iRadius;
    this.mLength = iLength;
    this.mDamageType = iStatus;
    this.mDPS = iDPS;
    Matrix iTransform = new Matrix();
    iTransform.M11 = iRadius;
    iTransform.M22 = 0.5f * iLength + iRadius;
    iTransform.M33 = iRadius;
    iTransform.M44 = 1f;
    iTransform.M41 = float.MaxValue;
    iTransform.M42 = float.MaxValue;
    iTransform.M43 = float.MaxValue;
    this.mEmitterA = new VisualEffect();
    this.mMagnitude = iMagnitude;
    this.mParticleMagntiude = (int) Math.Ceiling((double) this.mMagnitude);
    switch (iStatus)
    {
      case StatusEffects.Wet:
      case StatusEffects.Frozen:
        this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.EFFECTS_STATUS[StatusEffect.StatusIndex(iStatus)]);
        this.mEmitterA.Start(ref iTransform);
        break;
      case StatusEffects.Healing:
        this.mMagnitude = (float) (0.5 + (double) iMagnitude / 2.0);
        this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.EFFECTS_STATUS[StatusEffect.StatusIndex(iStatus)]);
        this.mEmitterA.Start(ref iTransform);
        break;
      case StatusEffects.Steamed:
        this.mEmitterA = EffectManager.Instance.GetEffect(Defines.STATUS_DRYING_EFFECT_HASH);
        this.mMagnitude = 1f;
        this.mEmitterA.Start(ref iTransform);
        break;
      case StatusEffects.Bleeding:
        this.mMagnitude = 1f;
        this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.BLEED_EFFECT[0]);
        this.mEmitterA.Start(ref iTransform);
        break;
    }
    this.mVomitEffect = new VisualEffectReference();
    this.mParticleTimer = 0.0f;
    this.mDamageRest = 0.0f;
  }

  public static int StatusIndex(StatusEffects iStatus)
  {
    return (int) (Math.Log((double) iStatus) * StatusEffect.ONEOVERLN2 + 0.5);
  }

  public static StatusEffects StatusFromIndex(int iIndex)
  {
    return (StatusEffects) (int) (Math.Pow(2.0, (double) iIndex) + 0.5);
  }

  public void Update(float iDeltaTime, IStatusEffected iOwner)
  {
    this.Update(iDeltaTime, iOwner, new Vector3?());
  }

  public void Update(float iDeltaTime, IStatusEffected iOwner, Vector3? iPosition)
  {
    if ((double) this.mMagnitude <= 0.0)
      return;
    float num1 = 1f;
    Vector3 vector3 = iPosition ?? iOwner.Position;
    this.mParticleTimer -= iDeltaTime;
    Matrix matrix = new Matrix();
    int num2 = 0;
    switch (this.mDamageType)
    {
      case StatusEffects.Burning:
        num1 = this.mEmitterA.Transform.M22 * 1.2f;
        this.mMagnitude = Math.Min(this.mMagnitude, 3f);
        this.mMagnitude -= iDeltaTime * 0.2f;
        this.mDPS = Math.Min(this.mDPS + (float) ((double) iDeltaTime * (double) this.mDPS * 0.25), 500f * this.mMagnitude);
        num2 = (int) Math.Ceiling((double) this.mMagnitude);
        this.mDamageRest += this.mDPS * iDeltaTime;
        int index1 = (int) MathHelper.Clamp(this.mDPS / 100f, 0.0f, 2f);
        if (this.mParticleMagntiude != index1 || !this.mEmitterA.IsActive)
        {
          this.mParticleMagntiude = index1;
          this.mEmitterA.Stop();
          Matrix iTransform = new Matrix();
          iTransform.M11 = this.mRadius;
          iTransform.M22 = 1f + this.mRadius;
          iTransform.M33 = this.mRadius;
          iTransform.M44 = this.mLength;
          iTransform.M41 = float.MaxValue;
          iTransform.M42 = float.MaxValue;
          iTransform.M43 = float.MaxValue;
          this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.EFFECT_BURNING[index1]);
          this.mEmitterA.Start(ref iTransform);
          break;
        }
        break;
      case StatusEffects.Wet:
        num1 = 1f;
        this.mMagnitude = Math.Min(this.mMagnitude, 1f);
        break;
      case StatusEffects.Frozen:
        this.mMagnitude = Math.Min(this.mMagnitude, 3f);
        this.mMagnitude -= iDeltaTime * 0.25f;
        break;
      case StatusEffects.Cold:
        num1 = (float) Math.Sqrt((double) this.mMagnitude * 0.20000000298023224) * 0.2f;
        if (iOwner is Character character1)
        {
          float slowdown = this.GetSlowdown();
          character1.CharacterBody.SpeedMultiplier = Math.Min(character1.CharacterBody.SpeedMultiplier, slowdown);
        }
        this.mMagnitude -= iDeltaTime * 0.1f;
        this.mMagnitude = Math.Min(this.mMagnitude, 3f);
        int num3 = (int) Math.Ceiling((double) this.mMagnitude);
        if (num3 > 0 && (this.mParticleMagntiude != num3 || !this.mEmitterA.IsActive))
        {
          this.mEmitterA.Stop();
          this.mParticleMagntiude = num3;
          Matrix iTransform = new Matrix();
          iTransform.M11 = this.mRadius;
          iTransform.M22 = 1f + this.mRadius;
          iTransform.M33 = this.mRadius;
          iTransform.M44 = this.mLength;
          iTransform.M41 = float.MaxValue;
          iTransform.M42 = float.MaxValue;
          iTransform.M43 = float.MaxValue;
          int index2 = Math.Min(Math.Max(this.mParticleMagntiude - 1, 0), StatusEffect.EFFECT_COLD.Length - 1);
          this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.EFFECT_COLD[index2]);
          this.mEmitterA.Start(ref iTransform);
          break;
        }
        break;
      case StatusEffects.Poisoned:
        if ((double) this.mMagnitude > 1.0)
          this.mMagnitude = 1f;
        this.mDamageRest += iDeltaTime * this.mDPS * this.mMagnitude;
        if (iOwner is Character character2)
        {
          vector3 = character2.GetMouthAttachOrientation().Translation;
          if (!character2.PlayState.IsInCutscene)
            character2.CharacterBody.SpeedMultiplier = Math.Min(character2.CharacterBody.SpeedMultiplier, this.GetSlowdown());
          Matrix attachOrientation = character2.GetMouthAttachOrientation();
          if ((double) this.mParticleTimer < 0.0)
          {
            EffectManager.Instance.StartEffect(StatusEffect.POISON_VOMIT, ref attachOrientation, out this.mVomitEffect);
            this.mParticleTimer += MagickaMath.RandomBetween(3f, 7f) / this.mMagnitude;
          }
          EffectManager.Instance.UpdateOrientation(ref this.mVomitEffect, ref attachOrientation);
          break;
        }
        break;
      case StatusEffects.Healing:
        this.mDamageRest += iDeltaTime * this.mDPS;
        if ((double) this.mMagnitude > 3.0)
          this.mMagnitude = 3f;
        if ((double) iOwner.HitPoints >= (double) iOwner.MaxHitPoints)
          this.mMagnitude = iDeltaTime;
        this.mMagnitude -= iDeltaTime;
        num1 = 1f;
        break;
      case StatusEffects.Steamed:
        this.mMagnitude = Math.Min(1f, this.mMagnitude);
        this.mMagnitude -= iDeltaTime;
        break;
      case StatusEffects.Bleeding:
        this.mDamageRest += iDeltaTime * this.mDPS;
        break;
    }
    int mDamageRest = (int) this.mDamageRest;
    if (mDamageRest != 0)
    {
      this.mDamageRest -= (float) mDamageRest;
      Elements iElement;
      switch (this.mDamageType)
      {
        case StatusEffects.Burning:
          iElement = Elements.Fire;
          break;
        case StatusEffects.Poisoned:
          iElement = Elements.Poison;
          break;
        case StatusEffects.Healing:
          iElement = Elements.Life;
          break;
        default:
          iElement = Elements.Earth;
          break;
      }
      iOwner.Damage((float) mDamageRest, iElement);
    }
    float iDeltaTime1 = iDeltaTime * num1;
    this.mEmitterA.Transform.M41 = vector3.X;
    this.mEmitterA.Transform.M42 = vector3.Y;
    this.mEmitterA.Transform.M43 = vector3.Z;
    this.mEmitterA.Update(iDeltaTime1);
  }

  internal void StopEffect()
  {
    if (!this.mEmitterA.IsActive)
      return;
    this.mEmitterA.Stop();
  }

  internal float GetSlowdown()
  {
    if (this.mDamageType == StatusEffects.Cold)
      return (float) (Math.Pow(0.001, (double) this.mMagnitude) * 0.66699999570846558 + 0.33300000429153442);
    return this.mDamageType == StatusEffects.Poisoned ? 0.5f : 1f;
  }

  public StatusEffects DamageType => this.mDamageType;

  public float Magnitude
  {
    get => this.mMagnitude;
    set => this.mMagnitude = value;
  }

  public float DPS
  {
    get => this.mDPS;
    set => this.mDPS = value;
  }

  public bool Dead => (double) this.mMagnitude <= 0.0;

  public bool Equals(StatusEffect obj) => obj.mDamageType == this.mDamageType;

  public override bool Equals(object obj)
  {
    return obj is StatusEffect statusEffect ? this.Equals(statusEffect) : base.Equals(obj);
  }

  public override string ToString()
  {
    return $"Effect: {this.mDamageType.ToString()} Magnitude: {(object) this.mMagnitude}";
  }

  public override int GetHashCode() => this.mDamageType.GetHashCode();

  public static StatusEffect operator +(StatusEffect A, StatusEffect B)
  {
    if (A.Dead | A.mDamageType == StatusEffects.None)
      return B;
    if (B.Dead | B.mDamageType == StatusEffects.None)
      return A;
    if (A.mDamageType != B.mDamageType)
      throw new Exception($"Cannot add status effects. Both effects must be of the same type. {A.ToString()} and {B.ToString()}");
    StatusEffect statusEffect = new StatusEffect();
    statusEffect.mParticleTimer = Math.Max(A.mParticleTimer, B.mParticleTimer);
    statusEffect.mDamageRest = A.mDamageRest + B.mDamageRest;
    statusEffect.mEmitterA = A.mEmitterA;
    statusEffect.mMagnitude = A.DamageType != StatusEffects.Healing ? A.mMagnitude + B.mMagnitude : Math.Max(A.mMagnitude, B.mMagnitude);
    statusEffect.mDamageType = A.mDamageType;
    statusEffect.mParticleMagntiude = Math.Max(A.mParticleMagntiude, B.mParticleMagntiude);
    statusEffect.mLength = A.mLength;
    statusEffect.mRadius = A.mRadius;
    float num = 1f / statusEffect.mMagnitude;
    statusEffect.mDPS = statusEffect.mDamageType != StatusEffects.Burning ? (float) ((double) A.mMagnitude * (double) num * (double) A.mDPS + (double) B.mMagnitude * (double) num * (double) B.mDPS) : Math.Max(A.mDPS, B.mDPS);
    return statusEffect;
  }

  public static bool operator ==(StatusEffect A, StatusEffect B) => A.mDamageType == B.mDamageType;

  public static bool operator !=(StatusEffect A, StatusEffect B) => A.mDamageType != B.mDamageType;

  public void Stop() => this.mMagnitude = 0.0f;
}
