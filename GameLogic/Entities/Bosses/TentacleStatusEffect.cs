// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.TentacleStatusEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead.ParticleEffects;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public struct TentacleStatusEffect : IEquatable<StatusEffect>
{
  private static readonly double ONEOVERLN2 = 1.0 / Math.Log(2.0);
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
  private VisualEffect[] mEmitterA;
  private float mDamageRest;
  private float mMagnitude;
  private int mParticleMagntiude;
  private float mDPS;
  private float mParticleTimer;
  private float mRadius;
  private float mLength;
  private float mWetTimer;
  private StatusEffects mDamageType;

  public TentacleStatusEffect(
    StatusEffects iStatus,
    float iDPS,
    float iMagnitude,
    float iLength,
    float iRadius)
    : this(iStatus, iDPS, iMagnitude, iLength, iRadius, 0.0f)
  {
  }

  public TentacleStatusEffect(
    StatusEffects iStatus,
    float iDPS,
    float iMagnitude,
    float iLength,
    float iRadius,
    float iWetTimer)
  {
    this.mWetTimer = iWetTimer;
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
    this.mEmitterA = new VisualEffect[10];
    for (int index = 0; index < this.mEmitterA.Length; ++index)
      this.mEmitterA[index] = new VisualEffect();
    this.mMagnitude = iMagnitude;
    this.mParticleMagntiude = (int) Math.Ceiling((double) this.mMagnitude);
    switch (iStatus)
    {
      case StatusEffects.Wet:
        if ((double) this.mWetTimer > 0.0)
        {
          for (int index = 0; index < this.mEmitterA.Length; ++index)
          {
            this.mEmitterA[index] = EffectManager.Instance.GetEffect(TentacleStatusEffect.EFFECTS_STATUS[TentacleStatusEffect.StatusIndex(iStatus)]);
            this.mEmitterA[index].Start(ref iTransform);
          }
          break;
        }
        break;
      case StatusEffects.Frozen:
        for (int index = 0; index < this.mEmitterA.Length; ++index)
        {
          this.mEmitterA[index] = EffectManager.Instance.GetEffect(TentacleStatusEffect.EFFECTS_STATUS[TentacleStatusEffect.StatusIndex(iStatus)]);
          this.mEmitterA[index].Start(ref iTransform);
        }
        break;
      case StatusEffects.Steamed:
        for (int index = 0; index < this.mEmitterA.Length; ++index)
        {
          this.mEmitterA[index] = EffectManager.Instance.GetEffect(Defines.STATUS_DRYING_EFFECT_HASH);
          this.mMagnitude = 1f;
          this.mEmitterA[index].Start(ref iTransform);
        }
        break;
    }
    this.mParticleTimer = 0.0f;
    this.mDamageRest = 0.0f;
  }

  public static int StatusIndex(StatusEffects iStatus)
  {
    return (int) (Math.Log((double) iStatus) * TentacleStatusEffect.ONEOVERLN2 + 0.5);
  }

  public static StatusEffects StatusFromIndex(int iIndex)
  {
    return (StatusEffects) (int) (Math.Pow(2.0, (double) iIndex) + 0.5);
  }

  public void Update(float iDeltaTime, IStatusEffected iOwner, CollisionSkin iSkin)
  {
    if ((double) this.mMagnitude <= 0.0)
      return;
    float num1 = 1f;
    this.mParticleTimer -= iDeltaTime;
    Matrix matrix = new Matrix();
    switch (this.mDamageType)
    {
      case StatusEffects.Wet:
        if ((double) this.mWetTimer < 0.0)
        {
          num1 = 0.0f;
        }
        else
        {
          num1 = 1f;
          this.mWetTimer -= iDeltaTime;
        }
        this.mMagnitude = 1f;
        break;
      case StatusEffects.Frozen:
        this.mMagnitude = Math.Min(this.mMagnitude, 3f);
        this.mMagnitude -= iDeltaTime * 0.25f;
        break;
      case StatusEffects.Cold:
        num1 = (float) Math.Sqrt((double) this.mMagnitude * 0.20000000298023224) * 0.2f;
        if (iOwner is Character character)
        {
          float slowdown = this.GetSlowdown();
          character.CharacterBody.SpeedMultiplier = Math.Min(character.CharacterBody.SpeedMultiplier, slowdown);
        }
        this.mMagnitude -= iDeltaTime * 0.1f;
        this.mMagnitude = Math.Min(this.mMagnitude, 3f);
        int num2 = (int) Math.Ceiling((double) this.mMagnitude);
        if (num2 > 0 && (this.mParticleMagntiude != num2 || !this.mEmitterA[0].IsActive))
        {
          this.mParticleMagntiude = num2;
          Matrix iTransform = new Matrix();
          iTransform.M11 = this.mRadius;
          iTransform.M22 = 1f + this.mRadius;
          iTransform.M33 = this.mRadius;
          iTransform.M44 = this.mLength;
          iTransform.M41 = float.MaxValue;
          iTransform.M42 = float.MaxValue;
          iTransform.M43 = float.MaxValue;
          int index1 = Math.Min(Math.Max(this.mParticleMagntiude - 1, 0), TentacleStatusEffect.EFFECT_COLD.Length - 1);
          for (int index2 = 0; index2 < this.mEmitterA.Length; ++index2)
          {
            this.mEmitterA[index2].Stop();
            this.mEmitterA[index2] = EffectManager.Instance.GetEffect(TentacleStatusEffect.EFFECT_COLD[index1]);
            this.mEmitterA[index2].Start(ref iTransform);
          }
          break;
        }
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
      Elements iElement = Elements.Earth;
      iOwner.Damage((float) mDamageRest, iElement);
    }
    float iDeltaTime1 = iDeltaTime * num1;
    if ((double) iDeltaTime1 <= 0.0)
      return;
    for (int prim = 0; prim < this.mEmitterA.Length; ++prim)
    {
      Vector3 translation = iSkin.GetPrimitiveNewWorld(prim).TransformMatrix.Translation;
      this.mEmitterA[prim].Transform.M41 = translation.X;
      this.mEmitterA[prim].Transform.M42 = translation.Y;
      this.mEmitterA[prim].Transform.M43 = translation.Z;
      this.mEmitterA[prim].Update(iDeltaTime1);
    }
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

  public bool Equals(StatusEffect obj) => obj.DamageType == this.mDamageType;

  public override bool Equals(object obj)
  {
    return obj is StatusEffect statusEffect ? this.Equals(statusEffect) : base.Equals(obj);
  }

  public override string ToString()
  {
    return $"Effect: {this.mDamageType.ToString()} Magnitude: {(object) this.mMagnitude}";
  }

  public override int GetHashCode() => this.mDamageType.GetHashCode();

  public static TentacleStatusEffect operator +(TentacleStatusEffect A, TentacleStatusEffect B)
  {
    if (A.Dead | A.mDamageType == StatusEffects.None)
      return B;
    if (B.Dead | B.DamageType == StatusEffects.None)
      return A;
    if (A.mDamageType != B.DamageType)
      throw new Exception($"Cannot add status effects. Both effects must be of the same type. {A.ToString()} and {B.ToString()}");
    TentacleStatusEffect tentacleStatusEffect = new TentacleStatusEffect();
    tentacleStatusEffect.mParticleTimer = Math.Max(A.mParticleTimer, B.mParticleTimer);
    tentacleStatusEffect.mDamageRest = A.mDamageRest + B.mDamageRest;
    tentacleStatusEffect.mEmitterA = A.mEmitterA;
    tentacleStatusEffect.mMagnitude = A.DamageType != StatusEffects.Healing ? A.mMagnitude + B.mMagnitude : Math.Max(A.mMagnitude, B.mMagnitude);
    tentacleStatusEffect.mDamageType = A.mDamageType;
    tentacleStatusEffect.mParticleMagntiude = Math.Max(A.mParticleMagntiude, B.mParticleMagntiude);
    tentacleStatusEffect.mLength = A.mLength;
    tentacleStatusEffect.mRadius = A.mRadius;
    tentacleStatusEffect.mWetTimer = Math.Max(A.mWetTimer, B.mWetTimer);
    float num = 1f / tentacleStatusEffect.mMagnitude;
    tentacleStatusEffect.mDPS = (float) ((double) A.mMagnitude * (double) num * (double) A.mDPS + (double) B.Magnitude * (double) num * (double) B.DPS);
    return tentacleStatusEffect;
  }

  public static bool operator ==(TentacleStatusEffect A, StatusEffect B)
  {
    return A.mDamageType == B.DamageType;
  }

  public static bool operator !=(TentacleStatusEffect A, StatusEffect B)
  {
    return A.mDamageType != B.DamageType;
  }

  public void Stop() => this.mMagnitude = 0.0f;
}
