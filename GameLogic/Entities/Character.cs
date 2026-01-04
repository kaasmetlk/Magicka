// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Character
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Localization;
using Magicka.Network;
using Magicka.Physics;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities;

public abstract class Character : Entity, ISpellCaster, IStatusEffected, IDamageable
{
  private static readonly int[] BEASTMEN_HASHES = new int[8]
  {
    "beastman_brute".GetHashCodeCustom(),
    "beastman_brute_earth".GetHashCodeCustom(),
    "beastman_brute_fire".GetHashCodeCustom(),
    "beastman_chieftain".GetHashCodeCustom(),
    "beastman_raider".GetHashCodeCustom(),
    "beastman_raider_lightning".GetHashCodeCustom(),
    "beastman_raider_water".GetHashCodeCustom(),
    "beastman_torcher".GetHashCodeCustom()
  };
  protected static readonly int STUN_EFFECT = "stunned".GetHashCodeCustom();
  protected static readonly Vector3 WetColor = new Vector3(0.75f, 0.8f, 1f);
  protected static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);
  protected static Matrix sRotateY180 = Matrix.CreateRotationY(3.14159274f);
  protected static Matrix sRotateX90 = Matrix.CreateRotationX(-1.57079637f);
  protected static Matrix sRotateSpines = Matrix.CreateFromYawPitchRoll(0.0f, 1.57079637f, 1.57079637f);
  protected static TextureCube sIceCubeMap;
  protected static TextureCube sIceCubeNormalMap;
  private static SkinnedModel sBarrierSkinModel;
  private static SkinnedModelDeferredAdvancedMaterial sEarthBarrierSkinMaterial;
  private static SkinnedModelDeferredAdvancedMaterial sIceBarrierSkinMaterial;
  private static Microsoft.Xna.Framework.Graphics.Model sAuraModel;
  protected Character.RenderData[] mRenderData;
  protected Character.NormalDistortionRenderData[] mNormalDistortionRenderData;
  protected Character.ShieldSkinRenderData[] mShieldSkinRenderData;
  protected Character.BarrierSkinRenderData[] mBarrierSkinRenderData;
  protected Character.HaloAuraRenderData[] mArmourRenderData;
  protected Character.LightningZapRenderData[] mLightningZapRenderData;
  protected Character.HighlightRenderData[] mHighlightRenderData;
  protected float mAuraPulsation;
  protected float mAuraRays1Rotation;
  protected float mAuraRays2Rotation;
  protected CharacterTemplate mTemplate;
  protected ConditionCollection mEventConditions;
  private int mDisplayName;
  protected string mName;
  protected int mType;
  protected Factions mFaction;
  protected BloodType mBlood;
  protected bool mEthereal;
  protected bool mEtherealLook;
  protected float mVolume;
  protected int mScoreValue;
  protected float mHitPoints;
  protected float mMaxHitPoints;
  protected int mNumberOfHealtBars;
  protected float mNormalizedHitPoints;
  protected float mBreakFreeTolerance;
  protected float mHitTolerance;
  protected float mTurnSpeed;
  protected float mTurnSpeedMax;
  protected float mKnockdownTolerance;
  protected Magicka.GameLogic.Entities.Resistance[] mResistances;
  protected float mHeightOffset;
  protected float mMapRotation;
  protected float mMapScale;
  protected float mZapTimer;
  protected float mBloatTimer;
  protected float mDryTimer;
  protected float mMaxPanic;
  protected float mPanic;
  protected float mWanderAngle;
  protected float mZapModifier;
  protected bool mBloatKilled;
  protected bool mFearless;
  protected bool mUncharmable;
  protected bool mNonslippery;
  protected bool mHasFairy;
  protected MovementProperties mMoveAbilities;
  protected Dictionary<byte, Magicka.Animations[]> mMoveAnimations;
  protected float mTotalRegenAccumulation;
  protected float mRegenAccumulation;
  protected int mRegenRate;
  protected float mRegenTimer;
  internal Character.SelfShield mSelfShield;
  internal bool mBubble;
  protected float mTemplateMass;
  protected float mUndieTimer;
  protected bool mAlert;
  protected bool mDashing;
  protected bool mAttacking;
  protected bool mKnockedDown;
  protected bool mIsHit;
  private bool mBlock;
  protected bool mDrowning;
  protected bool mBloating;
  protected Elements mBloatElement;
  protected Entity mBloatKiller;
  protected int mDialog;
  protected bool mAllowAttackRotate;
  protected SkinnedModel mModel;
  protected AnimationController mAnimationController;
  protected AnimationClipAction[][] mAnimationClips;
  protected AnimationAction[] mCurrentActions;
  protected List<bool> mExecutedActions = new List<bool>(8);
  protected List<bool> mDeadActions = new List<bool>(8);
  protected Magicka.Animations mNextDashAnimation;
  protected Magicka.Animations mNextAttackAnimation;
  protected Magicka.Animations mNextGripAttackAnimation;
  protected Magicka.Animations mSpawnAnimation;
  protected Magicka.Animations mSpecialIdleAnimation;
  protected Magicka.Animations mNetworkAnimation;
  protected float mNetworkAnimationBlend;
  protected WeaponClass mCurrentAnimationSet;
  protected Magicka.Animations mCurrentAnimation;
  protected Matrix mStaticTransform;
  protected int mModelID;
  protected float mLastDraw;
  protected bool mForceAnimationUpdate;
  private BindJoint mHipJoint;
  private BindJoint mMouthJoint;
  private BindJoint mLeftHandJoint;
  private BindJoint mLeftKneeJoint;
  private BindJoint mRightHandJoint;
  private BindJoint mRightKneeJoint;
  protected Character mGripper;
  protected bool mGripAttack;
  protected Grip.GripType mGripType;
  protected BindJoint mGripJoint;
  protected Character mGrippedCharacter;
  protected DamageCollection5 mCollisionDamages;
  protected Magicka.Animations mDropAnimation;
  protected float mGripDamageAccumulation;
  protected float mFireDamageAccumulation;
  protected float mFireDamageAccumulationTimer;
  protected float mPoisonDamageAccumulation;
  protected float mPoisonDamageAccumulationTimer = 0.25f;
  protected float mHealingAccumulation;
  protected float mHealingAccumulationTimer = 0.25f;
  protected float mBleedDamageAccumulation;
  protected float mBleedDamageAccumulationTimer;
  protected int mLastDamageIndex;
  protected float mLastDamageAmount;
  protected Elements mLastDamageElement;
  protected float mTimeSinceLastDamage;
  protected float mTimeSinceLastStatusDamage;
  protected float[] mBloatDamageAccumulation = new float[11];
  protected NonPlayerCharacter[] mCurrentSummons = new NonPlayerCharacter[16 /*0x10*/];
  protected int mNumCurrentSummons;
  protected int mNumCurrentUndeadSummons;
  protected int mNumCurrentFlamerSummons;
  protected Player mLastAccumulationDamager;
  protected Entity mLastAttacker;
  protected float mLastAttackerTimer;
  protected DamageResult mLastDamage;
  protected DynamicLight mStatusEffectLight;
  protected int mSourceOfSpellIndex;
  protected DynamicLight mSpellLight;
  protected StatusEffects mCurrentStatusEffects;
  protected StatusEffect[] mStatusEffects = new StatusEffect[9];
  protected Cue[] mStatusEffectCues;
  public Cue ChargeCue;
  protected float mSpellPower;
  protected Spell mSpell;
  protected CastType mCastType;
  protected float mHitByLightning;
  protected Matrix mStaffOrb;
  protected Matrix mWeaponTransform;
  protected StaticList<Spell> mSpellQueue;
  protected SpellEffect mCurrentSpell;
  public int mSummonElementCue;
  public Banks mSummonElementBank;
  protected Entanglement mEntaglement;
  protected Attachment[] mEquipment = new Attachment[8];
  protected List<GibReference> mGibs;
  protected VisualEffectReference mDryingEffect;
  protected int mOnDamageTrigger;
  protected int mOnDeathTrigger;
  protected BaseState mPreviousState;
  protected BaseState mCurrentState;
  protected SkinnedModelDeferredBasicMaterial mMaterial;
  protected float mImmortalTime;
  protected float mCollisionIgnoreTime;
  protected bool mOverkilled;
  protected bool mHighlight;
  protected float mInvisibilityTimer;
  protected bool mCannotDieWithoutExplicitKill;
  protected bool mFeared;
  protected float mFearTimer;
  protected Character mFearedBy;
  protected Vector3 mFearPosition;
  protected VisualEffectReference mFearEffect;
  protected float mMeleeBoostAmount;
  protected float mMeleeBoostTimer;
  protected float mBreakFreeStrength;
  private float mWaterDepth;
  private VisualEffectReference mWaterSplashEffect;
  protected float mShadowTimer;
  protected float mDeadTimer = 20f;
  public new bool mDead;
  protected bool mRemoveAfterDeath = true;
  protected HitList mHitList = new HitList(32 /*0x20*/);
  private float mDefaultSpecular;
  protected StatusEffects mDeathStatusEffects;
  protected VisualEffectReference[] mAttachedEffects = new VisualEffectReference[8];
  protected Matrix[] mAttachedEffectsBindPose = new Matrix[8];
  protected int[] mAttachedEffectsBoneIndex = new int[8]
  {
    -1,
    -1,
    -1,
    -1,
    -1,
    -1,
    -1,
    -1
  };
  protected Cue[] mAttachedSoundCues = new Cue[4];
  protected KeyValuePair<int, Banks>[] mAttachedSounds;
  protected DynamicLight mPointLight;
  protected Character.PointLightHolder mPointLightHolder;
  protected int mBreakFreeCounter;
  protected float mBoundingScale;
  protected Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility mSpecialAbility;
  protected static readonly Random sRandom = new Random();
  protected float mEtherealAlpha = 1f;
  protected float mEtherealAlphaTarget = 1f;
  protected float mEtherealAlphaSpeed = 1f;
  protected float mEtherealTimer;
  protected bool mEtherealTimedState;
  protected bool mTemplateIsEthereal;
  protected float mTimeWarpModifier = 1f;
  protected bool mDoNotRender;
  protected List<ActiveAura> mAuras = new List<ActiveAura>();
  protected List<BuffStorage> mBuffs = new List<BuffStorage>();
  protected float mAuraCycleTimer;
  protected Vector4 mHaloBuffColor = new Vector4();
  protected float mHaloBuffFade;
  protected List<DecalManager.DecalReference> mBuffDecals = new List<DecalManager.DecalReference>();
  protected List<VisualEffectReference> mBuffEffects = new List<VisualEffectReference>();
  protected float mBuffRotation;
  protected float mBuffCycleTimerOffensive;
  protected float mBuffCycleTimerDefensive;
  protected float mBuffCycleTimerSpecial;
  public bool mUndying;
  protected bool mJustCastInvisible;
  protected bool mCanHasStatusEffect = true;
  protected float mRestingMovementTimer = 1f;
  protected float mLastHitpoints;
  protected float mRestingHealthTimer = 1f;
  protected VisualEffectReference mLevitateEffect;
  protected float mLevitateTimer;
  protected float mLevitationFreeFallTimer;
  protected int mHypnotizeEffectID;
  protected VisualEffectReference mHypnotizeEffect;
  protected bool mHypnotized;
  protected Vector3 mHypnotizeDirection;
  protected Entity mCharmOwner;
  protected float mCharmTimer;
  protected int mCharmEffectID;
  protected VisualEffectReference mCharmEffect;
  protected bool mForceCamera;
  protected bool mForceNavMesh;
  protected bool mCollidedWithCamera;
  internal float mStunTime = 2f;
  internal float mStunTimer;
  internal VisualEffectReference mStunEffect;
  protected float mBleedRate;
  protected bool mNotedKilledEvent;
  protected float mCollisionDamageGracePeriod;
  protected float mTimeDead;

  public virtual bool HasStatus(StatusEffects iStatus)
  {
    return (this.mCurrentStatusEffects & iStatus) == iStatus;
  }

  public StatusEffect[] GetStatusEffects() => this.mStatusEffects;

  public virtual float StatusMagnitude(StatusEffects iStatus)
  {
    int index = StatusEffect.StatusIndex(iStatus);
    return !this.mStatusEffects[index].Dead ? this.mStatusEffects[index].Magnitude : 0.0f;
  }

  public virtual void Damage(float iDamage, Elements iElement)
  {
    if (this.IsImmortal)
      return;
    this.mInvisibilityTimer = 0.0f;
    if (this.mSelfShield.Active)
      this.mSelfShield.Damage(ref iDamage, iElement);
    if ((iElement & Elements.Fire) == Elements.Fire && this.HasStatus(StatusEffects.Greased))
      iDamage *= 2f;
    float mHitPoints = this.mHitPoints;
    float num = this.mHitPoints - iDamage;
    if (NetworkManager.Instance.State != NetworkState.Client)
      this.mHitPoints -= iDamage;
    DamageResult iResult;
    if ((double) num <= 0.0 & (double) mHitPoints > 0.0)
      iResult = DamageResult.Killed;
    else if ((double) iDamage >= 0.0)
    {
      iResult = DamageResult.Damaged;
      this.StopHypnotize();
    }
    else
    {
      iResult = DamageResult.Healed;
      if (this is Avatar)
        Profile.Instance.AddHealedAmount(this.PlayState, iDamage);
    }
    if (this.IsGripping && this.mLastAccumulationDamager != null && this.mLastAccumulationDamager.Avatar == this.mGrippedCharacter)
      this.mGripDamageAccumulation += iDamage;
    if (this.mLastAccumulationDamager != null && (iElement & Elements.Life) == Elements.None)
      StatisticsManager.Instance.AddDamageEvent(this.mPlayState, (IDamageable) this.mLastAccumulationDamager.Avatar, (IDamageable) this, this.PlayState.PlayTime, new Magicka.GameLogic.Damage(AttackProperties.Damage, iElement, (float) (int) iDamage, 1f), iResult);
    switch (iElement)
    {
      case Elements.Earth:
        this.mBleedDamageAccumulation += iDamage;
        break;
      case Elements.Fire:
        this.mFireDamageAccumulation += iDamage;
        break;
      case Elements.Life:
        if ((double) num < (double) this.mMaxHitPoints)
        {
          this.mHealingAccumulation += iDamage;
          break;
        }
        break;
      case Elements.Poison:
        this.mPoisonDamageAccumulation += iDamage;
        break;
    }
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if ((double) num > (double) this.mMaxHitPoints)
      this.mHitPoints = this.mMaxHitPoints;
    if (!this.IsEntangled || (iElement & Elements.Fire) != Elements.Fire)
      return;
    this.mEntaglement.DecreaseEntanglement(iDamage, iElement);
  }

  public override bool Dead => !this.mCannotDieWithoutExplicitKill & this.mDead;

  public float HitPoints
  {
    get => this.mHitPoints;
    set => this.mHitPoints = value;
  }

  public float MaxHitPoints
  {
    get => this.mMaxHitPoints;
    set => this.mMaxHitPoints = value;
  }

  public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    if (this.mEthereal)
    {
      oPosition = new Vector3();
      return false;
    }
    Segment seg0 = new Segment();
    seg0.Origin = this.Position;
    seg0.Origin.Y -= this.Capsule.Length * 0.5f;
    seg0.Delta = this.Capsule.Orientation.Backward;
    Vector3.Multiply(ref seg0.Delta, this.Capsule.Length, out seg0.Delta);
    float t0;
    float t1;
    float num1 = Distance.SegmentSegmentDistanceSq(out t0, out t1, seg0, iSeg);
    float num2 = iSegmentRadius + this.Capsule.Radius;
    float num3 = num2 * num2;
    if ((double) num1 > (double) num3)
    {
      oPosition = new Vector3();
      return false;
    }
    Vector3 point;
    seg0.GetPoint(t0, out point);
    Vector3 vector3;
    iSeg.GetPoint(t1, out vector3);
    Vector3.Subtract(ref vector3, ref point, out vector3);
    vector3.Normalize();
    Vector3.Multiply(ref vector3, this.Capsule.Radius, out vector3);
    Vector3.Add(ref point, ref vector3, out oPosition);
    return true;
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
        if (this is Avatar)
        {
          multiplier *= this.Equipment[1].Item.Resistance[iIndex].Multiplier;
          modifier += this.Equipment[1].Item.Resistance[iIndex].Modifier;
        }
        for (int index = 0; index < this.mBuffs.Count; ++index)
        {
          BuffStorage mBuff = this.mBuffs[index];
          if (mBuff.BuffType == BuffType.Resistance && (mBuff.BuffResistance.Resistance.ResistanceAgainst & elements) == elements)
          {
            multiplier *= mBuff.BuffResistance.Resistance.Multiplier;
            modifier += mBuff.BuffResistance.Resistance.Modifier;
          }
        }
        num1 += modifier;
        num2 += multiplier;
      }
    }
    return 1f - MathHelper.Clamp(num1 / 300f + num2, -1f, 1f);
  }

  public virtual DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult damageResult = DamageResult.None;
    if (this.Dead | this.IsImmortal | this.mResistances == null)
      return damageResult;
    float mHitPoints = this.mHitPoints;
    this.mInvisibilityTimer = 0.0f;
    if ((iDamage.AttackProperty & AttackProperties.Entanglement) == AttackProperties.Entanglement && !this.HasStatus(StatusEffects.Burning))
    {
      if (iAttacker == this)
        return DamageResult.None;
      this.Entangle(iDamage.Magnitude);
      return DamageResult.Hit;
    }
    if (this.IsEntangled && ((iDamage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage || (iDamage.AttackProperty & AttackProperties.Status) == AttackProperties.Status && (iDamage.Element & Elements.Fire) == Elements.Fire))
      this.mEntaglement.DecreaseEntanglement(iDamage.Magnitude, iDamage.Element);
    if (this.mSelfShield.Active)
      damageResult |= this.mSelfShield.Damage(this, ref iDamage, iAttacker, iAttackPosition, iFeatures);
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
          this.mKnockedDown = true;
          damageResult |= DamageResult.Knockedback;
        }
      }
    }
    else
    {
      if ((iDamage.AttackProperty & AttackProperties.Pushed) == AttackProperties.Pushed)
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
          damageResult |= DamageResult.Pushed;
        }
      }
      if ((iDamage.AttackProperty & AttackProperties.Knockdown) == AttackProperties.Knockdown)
        this.KnockDown(iDamage.Magnitude);
    }
    if ((iDamage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage && (iDamage.Element & (Elements.Earth | Elements.Lightning)) != Elements.None && this.BlockItem >= 0)
    {
      float oBlocked = 0.0f;
      if (this.BlockDamage(iDamage, iAttackPosition, out oBlocked))
      {
        Vector3 result = this.Body.Position;
        Vector3.Subtract(ref iAttackPosition, ref result, out result);
        EffectManager.Instance.StartEffect("melee_block".GetHashCodeCustom(), ref iAttackPosition, ref result, out VisualEffectReference _);
        if ((double) oBlocked >= (double) iDamage.Amount * (double) iDamage.Magnitude)
          damageResult |= DamageResult.Deflected;
        else
          iDamage.Amount = Math.Max(0.0f, iDamage.Amount - oBlocked);
      }
    }
    if (Defines.FeatureDamage(iFeatures) && (iDamage.AttackProperty & AttackProperties.Status) == AttackProperties.Status)
    {
      if ((iDamage.Element & Elements.Fire) == Elements.Fire)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Burning, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
      if ((iDamage.Element & Elements.Cold) == Elements.Cold)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Cold, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
      if ((iDamage.Element & Elements.Water) == Elements.Water)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
      if ((iDamage.Element & Elements.Poison) == Elements.Poison)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Poisoned, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
      if ((iDamage.Element & Elements.Life) == Elements.Life)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Healing, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
      if ((iDamage.Element & Elements.Steam) == Elements.Steam)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
    }
    iDamage.ApplyResistances(this.mResistances, this is Avatar ? this.Equipment[1].Item.Resistance : (Magicka.GameLogic.Entities.Resistance[]) null, (IList<BuffStorage>) this.mBuffs, this.mCurrentStatusEffects);
    if ((double) Math.Abs(iDamage.Magnitude) <= 1.4012984643248171E-45)
      damageResult |= DamageResult.Deflected;
    if ((damageResult & DamageResult.Deflected) == DamageResult.Deflected)
      return damageResult;
    if ((iDamage.AttackProperty & AttackProperties.Stun) == AttackProperties.Stun && (double) iDamage.Amount > 0.0)
      this.mStunTimer = this.mStunTime;
    if ((iDamage.AttackProperty & AttackProperties.Bleed) == AttackProperties.Bleed && (double) iDamage.Amount > 0.0)
      damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Bleeding, this.mBleedRate, 1f, this.Capsule.Length, this.Radius));
    if ((iDamage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage)
    {
      if ((iDamage.Element & Elements.Lightning) == Elements.Lightning)
      {
        if ((double) this.mZapTimer <= 0.0)
          this.mZapTimer = 0.1f;
        if (this.HasStatus(StatusEffects.Wet))
        {
          this.mHitByLightning = 0.1f;
          iDamage.Amount *= 2f;
        }
      }
      if ((iDamage.Element & Elements.Life) == Elements.Life)
      {
        this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude -= iDamage.Magnitude;
        if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude <= 0.0)
          this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
      }
      if ((iDamage.Element & (Elements.PhysicalElements | Elements.Arcane)) != Elements.None)
      {
        if (this.HasStatus(StatusEffects.Frozen))
          iDamage.Amount *= 3f;
        else if (this.mBlood != BloodType.none && GlobalSettings.Instance.BloodAndGore == SettingOptions.On && (double) iDamage.Amount > 0.0 && Defines.FeatureEffects(iFeatures))
        {
          Vector3 oPos = this.Position;
          Vector3 direction = this.Direction;
          EffectManager.Instance.StartEffect(Gib.GORE_SPLASH_EFFECTS[(int) this.mBlood], ref oPos, ref direction, out VisualEffectReference _);
          AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Misc, Defines.SOUNDS_GORE[(int) this.mBlood], new SpellSoundVariables()
          {
            mMagnitude = iDamage.Amount
          }, this.AudioEmitter);
          Segment iSeg = new Segment();
          Vector3 vector3 = new Vector3((float) ((MagickaMath.Random.NextDouble() - 0.5) * 2.0) * this.Radius, 0.0f, (float) ((MagickaMath.Random.NextDouble() - 0.5) * 2.0) * this.Radius);
          iSeg.Origin = this.Position;
          Vector3.Add(ref iSeg.Origin, ref vector3, out iSeg.Origin);
          iSeg.Delta = this.Position;
          Vector3.Subtract(ref iSeg.Delta, ref iAttackPosition, out iSeg.Delta);
          iSeg.Delta.Normalize();
          iSeg.Delta.Y = -2f;
          Vector3.Multiply(ref iSeg.Delta, 5f, out iSeg.Delta);
          Vector3 oNrm;
          AnimatedLevelPart oAnimatedLevelPart;
          this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out oNrm, out oAnimatedLevelPart, iSeg);
          DecalManager.Instance.AddAlphaBlendedDecal((Decal) ((int) this.mBlood * 4 + (int) Math.Min((float) ((double) iDamage.Amount * (double) iDamage.Magnitude / 400.0), 3f)), oAnimatedLevelPart, 4f, ref oPos, ref oNrm, 60f);
        }
      }
      iDamage.Amount *= iDamage.Magnitude;
      if (Defines.FeatureDamage(iFeatures) && NetworkManager.Instance.State != NetworkState.Client)
        this.mHitPoints -= iDamage.Amount;
      if (iAttacker is Avatar && !((iAttacker as Avatar).Player.Gamer is NetworkGamer))
      {
        if (Defines.FeatureDamage(iFeatures) && iDamage.Element == Elements.Life)
          Profile.Instance.AddHealedAmount(this.PlayState, iDamage.Amount);
        if ((double) iDamage.Amount > 9000.0)
          AchievementsManager.Instance.AwardAchievement(this.PlayState, "itsoverninethousand");
      }
      if (this.IsGripping & iAttacker == this.mGrippedCharacter)
      {
        this.mGripDamageAccumulation += iDamage.Amount;
        if (this.CurrentState is GripAttackState)
          this.ChangeState((BaseState) GrippingState.Instance);
      }
      if ((iDamage.AttackProperty & AttackProperties.Piercing) != (AttackProperties) 0 && (double) iDamage.Magnitude > 0.0 && (double) iDamage.Amount > 0.0)
        damageResult |= DamageResult.Pierced;
      if ((double) iDamage.Amount > 0.0)
        damageResult |= DamageResult.Damaged;
      if ((double) iDamage.Amount == 0.0)
        damageResult |= DamageResult.Deflected;
      if ((double) iDamage.Amount < 0.0)
        damageResult |= DamageResult.Healed;
      if ((double) iDamage.Amount > (double) this.mHitTolerance || this.mCurrentState is RessurectionState)
      {
        damageResult |= DamageResult.Hit;
        this.mIsHit = true;
      }
      if (Defines.FeatureNotify(iFeatures))
      {
        if ((double) iDamage.Amount != 0.0)
          this.mTimeSinceLastDamage = 0.0f;
        if (this.mLastDamageIndex >= 0)
        {
          DamageNotifyer.Instance.AddToNumber(this.mLastDamageIndex, iDamage.Amount);
        }
        else
        {
          this.mLastDamageAmount = iDamage.Amount;
          this.mLastDamageElement = iDamage.Element;
          Vector3 position = this.Position;
          position.Y += this.Capsule.Length * 0.5f + this.mRadius;
          this.mLastDamageIndex = DamageNotifyer.Instance.AddNumber(iDamage.Amount, ref position, 0.4f, true);
        }
      }
    }
    if ((double) this.mHitPoints > (double) this.mMaxHitPoints)
      this.mHitPoints = this.mMaxHitPoints;
    float iAmount = mHitPoints;
    if ((iDamage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage)
      iAmount -= iDamage.Amount;
    if ((double) iDamage.Amount == 0.0)
      damageResult |= DamageResult.Deflected;
    else if ((double) iDamage.Amount > 0.0)
      this.StopHypnotize();
    if ((double) iAmount <= 0.0)
    {
      if (this.HasStatus(StatusEffects.Frozen) || this.OverKilled(iAmount))
      {
        if (Defines.FeatureDamage(iFeatures) && (iDamage.Element & ~Elements.Arcane) != Elements.None)
          this.OverKill();
        damageResult |= DamageResult.OverKilled;
      }
      damageResult |= DamageResult.Killed;
    }
    if (iAttacker is Avatar avatar && (damageResult & DamageResult.Statusadded) == DamageResult.Statusadded & (iDamage.Element & Elements.Fire) == Elements.Fire)
      this.mLastAccumulationDamager = avatar.Player;
    DamageResult iResult = damageResult;
    if ((double) mHitPoints <= 0.0)
      iResult &= ~DamageResult.Killed;
    if ((damageResult & DamageResult.Damaged) != DamageResult.None)
    {
      if (this.mOnDamageTrigger != 0)
        this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDamageTrigger, this, false);
      StatisticsManager.Instance.AddDamageEvent(this.mPlayState, iAttacker as IDamageable, (IDamageable) this, iTimeStamp, iDamage, iResult);
    }
    if (iAttacker is Avatar && !((iAttacker as Avatar).Player.Gamer is NetworkGamer) && (damageResult & DamageResult.OverKilled) == DamageResult.OverKilled && this is NonPlayerCharacter && this is NonPlayerCharacter nonPlayerCharacter)
    {
      bool flag = false;
      for (int index = 0; index < Character.BEASTMEN_HASHES.Length; ++index)
      {
        if (nonPlayerCharacter.Type == Character.BEASTMEN_HASHES[index])
          flag = true;
      }
      if (flag)
        this.PlayState.ItsRainingBeastMen[(iAttacker as Avatar).Player.ID][(this as NonPlayerCharacter).Handle] = 5f;
    }
    if (this is Avatar)
    {
      if ((double) this.mHitPoints <= 0.0 && Magicka.Game.Instance.PlayerCount > 1)
        TutorialManager.Instance.SetTip(TutorialManager.Tips.Dying, TutorialManager.Position.Top);
      if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude > 0.0)
        TutorialManager.Instance.SetTip(TutorialManager.Tips.Poison, TutorialManager.Position.Top);
      if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Magnitude > 0.0)
        TutorialManager.Instance.SetTip(TutorialManager.Tips.Wet, TutorialManager.Position.Top);
      if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude > 0.0)
        TutorialManager.Instance.SetTip(TutorialManager.Tips.Cold, TutorialManager.Position.Top);
      if ((double) this.mHitPoints > 0.0 && (double) this.mHitPoints < (double) this.mMaxHitPoints)
        TutorialManager.Instance.SetTip(TutorialManager.Tips.Heal, TutorialManager.Position.Top);
    }
    if ((double) mHitPoints > 0.0)
      this.mLastAttacker = iAttacker;
    this.mLastAttackerTimer = 8f;
    this.mLastDamage = damageResult;
    return damageResult;
  }

  public override void Kill()
  {
    if (this.CannotDieWithoutExplicitKill)
      return;
    this.mInvisibilityTimer = 0.0f;
    if (this.HasStatus(StatusEffects.Frozen))
      this.OverKill();
    else
      this.mHitPoints = 0.0f;
    if (this.mStatusEffectLight == null)
      return;
    this.mStatusEffectLight.Stop(false);
    this.mStatusEffectLight = (DynamicLight) null;
  }

  public virtual void OverKill()
  {
    if (this.mOverkilled || this.CannotDieWithoutExplicitKill)
      return;
    this.mDead = true;
    this.mOverkilled = true;
    this.mDeathStatusEffects = this.mCurrentStatusEffects;
    this.mInvisibilityTimer = 0.0f;
    this.mHitPoints = (float) (-(double) this.mMaxHitPoints - 1000.0);
    this.mPlayState.Camera.CameraShake(this.Position, 1.5f, 0.5f);
    EffectManager.Instance.Stop(ref this.mFearEffect);
    EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
    this.StopHypnotize();
    this.EndCharm();
    this.StopLevitate();
    for (int index = 0; index < this.mCurrentActions.Length; ++index)
      this.mCurrentActions[index].Kill(this);
    if (this.mStatusEffectLight != null)
    {
      this.mStatusEffectLight.Stop(false);
      this.mStatusEffectLight = (DynamicLight) null;
    }
    this.mEventConditions.ExecuteAll((Entity) this, (Entity) null, ref new EventCondition()
    {
      EventConditionType = EventConditionType.OverKill
    });
    if (this.mOnDeathTrigger != 0)
      this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDeathTrigger, this, false);
    if (this.mLastAttacker is Avatar && !((this.mLastAttacker as Avatar).Player.Gamer is NetworkGamer))
      Profile.Instance.AddOverKills(this);
    this.mHitList.Clear();
    if (this.mDialog == 0 || !DialogManager.Instance.DialogActive(this.mDialog))
      return;
    DialogManager.Instance.End(this.mDialog);
    DialogManager.Instance.Dialogs.DialogFinished(this.mDialog);
  }

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
  }

  protected Character(PlayState iPlayState)
    : base(iPlayState)
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (Character.sBarrierSkinModel == null)
    {
      lock (graphicsDevice)
        Character.sBarrierSkinModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/combined_armor");
      Helper.SkinnedModelDeferredMaterialFromBasicEffect(Character.sBarrierSkinModel.Model.Meshes[0].Effects[0] as SkinnedModelBasicEffect, out Character.sEarthBarrierSkinMaterial);
      Helper.SkinnedModelDeferredMaterialFromBasicEffect(Character.sBarrierSkinModel.Model.Meshes[0].Effects[1] as SkinnedModelBasicEffect, out Character.sIceBarrierSkinMaterial);
    }
    if (Character.sAuraModel == null)
    {
      lock (graphicsDevice)
        Character.sAuraModel = Magicka.Game.Instance.Content.Load<Microsoft.Xna.Framework.Graphics.Model>("Models/Effects/armor_aura");
    }
    if (Character.sIceCubeMap == null)
    {
      lock (graphicsDevice)
        Character.sIceCubeMap = Magicka.Game.Instance.Content.Load<TextureCube>("EffectTextures/iceCube");
    }
    if (Character.sIceCubeNormalMap == null)
    {
      lock (graphicsDevice)
        Character.sIceCubeNormalMap = Magicka.Game.Instance.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
    }
    this.mRenderData = new Character.RenderData[3];
    this.mNormalDistortionRenderData = new Character.NormalDistortionRenderData[3];
    this.mShieldSkinRenderData = new Character.ShieldSkinRenderData[3];
    this.mHighlightRenderData = new Character.HighlightRenderData[3];
    this.mLightningZapRenderData = new Character.LightningZapRenderData[3];
    this.mBarrierSkinRenderData = new Character.BarrierSkinRenderData[3];
    this.mArmourRenderData = new Character.HaloAuraRenderData[3];
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      InvisibilityEffect iEffect = new InvisibilityEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
      for (int index = 0; index < 3; ++index)
      {
        this.mRenderData[index] = new Character.RenderData(this);
        this.mNormalDistortionRenderData[index] = new Character.NormalDistortionRenderData(iEffect);
        this.mNormalDistortionRenderData[index].mSkeleton = this.mRenderData[index].mSkeleton;
        this.mShieldSkinRenderData[index] = new Character.ShieldSkinRenderData();
        this.mShieldSkinRenderData[index].mSkeleton = this.mRenderData[index].mSkeleton;
        this.mHighlightRenderData[index] = new Character.HighlightRenderData();
        this.mHighlightRenderData[index].mSkeleton = this.mRenderData[index].mSkeleton;
        this.mLightningZapRenderData[index] = new Character.LightningZapRenderData();
        this.mLightningZapRenderData[index].mSkeleton = this.mRenderData[index].mSkeleton;
        this.mBarrierSkinRenderData[index] = new Character.BarrierSkinRenderData();
        this.mBarrierSkinRenderData[index].mSkeleton = this.mRenderData[index].mSkeleton;
        this.mBarrierSkinRenderData[index].SetMesh(Character.sBarrierSkinModel.Model.Meshes[0].VertexBuffer, Character.sBarrierSkinModel.Model.Meshes[0].IndexBuffer, Character.sBarrierSkinModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
        this.mBarrierSkinRenderData[index].SetIceMesh(Character.sBarrierSkinModel.Model.Meshes[0].MeshParts[1]);
        this.mBarrierSkinRenderData[index].mMaterial = Character.sEarthBarrierSkinMaterial;
        this.mBarrierSkinRenderData[index].mIceMaterial = Character.sIceBarrierSkinMaterial;
        this.mBarrierSkinRenderData[index].RenderEarth = false;
        this.mBarrierSkinRenderData[index].RenderIce = false;
        this.mArmourRenderData[index] = new Character.HaloAuraRenderData();
        this.mArmourRenderData[index].SetMesh(Character.sAuraModel.Meshes[0], Character.sAuraModel.Meshes[0].MeshParts[0], 0);
        this.mArmourRenderData[index].AssignParts(Character.sAuraModel.Meshes[0].MeshParts[1], Character.sAuraModel.Meshes[0].MeshParts[0], Character.sAuraModel.Meshes[0].MeshParts[2]);
      }
    }
    for (int index = 0; index < this.mEquipment.Length; ++index)
      this.mEquipment[index] = new Attachment(iPlayState, this);
    this.mBody = (Body) new CharacterBody(this);
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Capsule(new Vector3(), Character.sRotateX90, 4.5f, 1f), 1, new MaterialProperties(0.0f, 0.0f, 0.0f));
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mCollision.postCollisionCallbackFn += new PostCollisionCallbackFn(this.PostCollision);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.Immovable = false;
    this.mBody.Tag = (object) this;
    this.mAnimationController = new AnimationController();
    this.mAnimationController.AnimationLooped += new AnimationEvent(this.OnAnimationLooped);
    this.mAnimationController.CrossfadeFinished += new AnimationEvent(this.OnCrossfadeFinished);
    this.mSpellQueue = (StaticList<Spell>) new StaticEquatableList<Spell>(5);
    this.mGibs = new List<GibReference>();
    this.mEntaglement = new Entanglement(this);
    this.mStatusEffectCues = new Cue[3];
    this.mDoNotRender = false;
    this.BlockItem = -1;
  }

  public void AddBuff(ref BuffStorage iBuff)
  {
    for (int index = 0; index < this.mBuffs.Count; ++index)
    {
      BuffStorage mBuff = this.mBuffs[index];
      if ((int) mBuff.UniqueID == (int) iBuff.UniqueID)
      {
        mBuff.TTL = iBuff.TTL;
        this.mBuffs[index] = mBuff;
        return;
      }
    }
    if (this.mCurrentState is DeadState)
      return;
    if (iBuff.BuffType == BuffType.Resistance)
    {
      for (int index = 0; index < 11; ++index)
      {
        Elements elements = (Elements) (1 << index);
        if ((elements & iBuff.BuffResistance.Resistance.ResistanceAgainst) != Elements.None)
        {
          switch (elements)
          {
            case Elements.Water:
              if ((double) Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 9.9999999747524271E-07)
              {
                this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Magnitude = 0.0f;
                continue;
              }
              this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Magnitude *= iBuff.BuffResistance.Resistance.Multiplier;
              continue;
            case Elements.Cold:
              if ((double) Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 9.9999999747524271E-07)
              {
                this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude = 0.0f;
                continue;
              }
              this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude *= iBuff.BuffResistance.Resistance.Multiplier;
              continue;
            case Elements.Fire:
              if ((double) Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 9.9999999747524271E-07)
              {
                this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Magnitude = 0.0f;
                continue;
              }
              this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Magnitude *= iBuff.BuffResistance.Resistance.Multiplier;
              continue;
            case Elements.Life:
              if ((double) Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 9.9999999747524271E-07)
              {
                this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Healing)].Magnitude = 0.0f;
                continue;
              }
              this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Healing)].Magnitude *= iBuff.BuffResistance.Resistance.Multiplier;
              continue;
            case Elements.Steam:
              if ((double) Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 9.9999999747524271E-07)
              {
                this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Steamed)].Magnitude = 0.0f;
                continue;
              }
              this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Steamed)].Magnitude *= iBuff.BuffResistance.Resistance.Multiplier;
              continue;
            case Elements.Poison:
              if ((double) Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 9.9999999747524271E-07)
              {
                this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude = 0.0f;
                continue;
              }
              this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude *= iBuff.BuffResistance.Resistance.Multiplier;
              continue;
            default:
              continue;
          }
        }
      }
    }
    if (iBuff.BuffType == BuffType.ModifyHitPoints && this is NonPlayerCharacter)
      this.mHitPoints *= iBuff.BuffModifyHitPoints.HitPointsMultiplier;
    DecalManager.DecalReference oReference = new DecalManager.DecalReference();
    oReference.Index = -1;
    if (iBuff.VisualCategory != VisualCategory.None)
    {
      Vector2 iScale = new Vector2();
      iScale.X = iScale.Y = this.mRadius * 1.5f;
      Vector3 position = this.Position;
      DecalManager.Instance.AddAlphaBlendedDecal((Decal) ((byte) 26 + iBuff.VisualCategory - (byte) 1), (AnimatedLevelPart) null, ref iScale, ref position, new Vector3?(), ref new Vector3()
      {
        Y = 1f
      }, 1f, ref new Vector4()
      {
        X = iBuff.Color.X,
        Y = iBuff.Color.Y,
        Z = iBuff.Color.Z
      }, out oReference);
    }
    this.mBuffs.Add(iBuff);
    this.mBuffDecals.Add(oReference);
    VisualEffectReference oRef = new VisualEffectReference();
    if (iBuff.Effect != 0)
    {
      Vector3 position = this.Position;
      Vector3 direction = this.Direction;
      EffectManager.Instance.StartEffect(iBuff.Effect, ref position, ref direction, out oRef);
    }
    this.mBuffEffects.Add(oRef);
  }

  public void ClearAura()
  {
    for (int index = 0; index < this.mAuras.Count; ++index)
    {
      ActiveAura mAura = this.mAuras[index];
      if (mAura.SelfCasted)
      {
        mAura.Aura.TTL = 0.0f;
        this.mAuras[index] = mAura;
      }
    }
  }

  public void AddAura(ref AuraStorage iAura, bool iSelfCasted)
  {
    ActiveAura activeAura = new ActiveAura();
    if (iAura.AuraType == AuraType.Buff)
      iAura.AuraBuff.Buff.SelfCasted = iSelfCasted;
    activeAura.Aura = iAura;
    activeAura.SelfCasted = iSelfCasted;
    activeAura.StartTTL = iAura.TTL;
    VisualEffectReference oRef = new VisualEffectReference();
    if (iAura.Effect != 0)
    {
      Vector3 position = this.Position;
      Vector3 direction = this.Direction;
      EffectManager.Instance.StartEffect(iAura.Effect, ref position, ref direction, out oRef);
    }
    activeAura.mEffect = oRef;
    this.mAuras.Add(activeAura);
  }

  public BaseState ChangeState(BaseState iState)
  {
    if (iState == null || this.mCurrentState == iState)
      return this.mCurrentState;
    this.mCurrentState.OnExit(this);
    this.mPreviousState = this.mCurrentState;
    this.mCurrentState = iState;
    this.mCurrentState.OnEnter(this);
    return this.mPreviousState;
  }

  public void SetState(BaseState iState)
  {
    this.mPreviousState = (BaseState) null;
    this.mCurrentState = iState;
    this.mCurrentState.OnEnter(this);
  }

  public void SetStateIgnoreEnterExit(BaseState iState)
  {
    this.mPreviousState = (BaseState) null;
    this.mCurrentState = iState;
  }

  public BaseState CurrentState => this.mCurrentState;

  public BaseState PreviousState => this.mPreviousState;

  public virtual void Initialize(
    CharacterTemplate iTemplate,
    int iRandomOverride,
    Vector3 iPosition,
    int iUniqueID,
    float iDeadTimer)
  {
    this.Initialize(iTemplate, iRandomOverride, iPosition, iUniqueID);
    this.mDeadTimer = iDeadTimer;
    this.mDoNotRender = false;
  }

  public virtual void Initialize(
    CharacterTemplate iTemplate,
    Vector3 iPosition,
    int iUniqueID,
    float iDeadTimer)
  {
    this.Initialize(iTemplate, iPosition, iUniqueID);
    this.mDeadTimer = iDeadTimer;
    this.mDoNotRender = false;
  }

  public virtual void Initialize(CharacterTemplate iTemplate, Vector3 iPosition, int iUniqueID)
  {
    this.Initialize(iTemplate, -1, iPosition, iUniqueID);
    this.mDoNotRender = false;
  }

  public virtual void Initialize(
    CharacterTemplate iTemplate,
    int iRandomOverride,
    Vector3 iPosition,
    int iUniqueID)
  {
    this.Initialize(iUniqueID);
    this.mNotedKilledEvent = false;
    this.mTemplate = iTemplate;
    this.mShadowTimer = 0.0f;
    this.mDrowning = false;
    this.mDashing = false;
    this.mRegenRate = 0;
    this.mOnDamageTrigger = 0;
    this.mOnDeathTrigger = 0;
    this.mSpecialIdleAnimation = this.mNetworkAnimation = Magicka.Animations.None;
    this.mNetworkAnimationBlend = 0.2f;
    this.StopLevitate();
    this.mLevitationFreeFallTimer = 0.0f;
    this.mCharmTimer = 0.0f;
    this.mStunTimer = 0.0f;
    this.mCannotDieWithoutExplicitKill = false;
    this.mCollisionDamageGracePeriod = 1f;
    this.ApplyTemplate(iTemplate, ref iRandomOverride);
    this.mLastAccumulationDamager = (Player) null;
    this.mNormalizedHitPoints = 0.0f;
    this.mTimeSinceLastDamage = float.MaxValue;
    this.mTimeSinceLastStatusDamage = float.MaxValue;
    this.mLastDraw = 0.2f;
    this.mForceAnimationUpdate = false;
    this.mBoundingScale = 1.333f;
    this.mLastDraw = 0.0f;
    this.mZapTimer = 0.0f;
    this.mNormalizedHitPoints = 0.0f;
    this.mRegenAccumulation = 0.0f;
    this.mPanic = 0.0f;
    this.mRegenTimer = 1f;
    this.mBloatTimer = 0.0f;
    this.mBloating = false;
    this.mAttacking = false;
    this.mGripAttack = false;
    this.mBloatKilled = false;
    this.mOverkilled = false;
    this.mBlock = false;
    this.mIsHit = false;
    this.mKnockedDown = false;
    this.mFearTimer = 0.0f;
    this.mHypnotized = false;
    this.mForceCamera = false;
    this.mForceNavMesh = false;
    this.mDead = false;
    this.mWaterDepth = 0.0f;
    this.mDeathStatusEffects = StatusEffects.None;
    this.mCastType = CastType.None;
    for (int index = 0; index < this.mBloatDamageAccumulation.Length; ++index)
      this.mBloatDamageAccumulation[index] = 0.0f;
    this.CharacterBody.Velocity = new Vector3();
    this.CharacterBody.Movement = new Vector3();
    this.mDeadTimer = (float) (20.0 + 20.0 * MagickaMath.Random.NextDouble());
    this.RemoveSelfShield();
    this.mUndying = this.mTemplate.Undying;
    this.mUndieTimer = !this.mUndying ? float.NaN : this.mTemplate.UndieTime;
    if (this.mPlayState.Level != null && this.mPlayState.Level.CurrentScene != null)
    {
      Segment iSeg = new Segment();
      iSeg.Origin = iPosition;
      ++iSeg.Origin.Y;
      iSeg.Delta.Y = -10f;
      Vector3 oPos;
      if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, out AnimatedLevelPart _, iSeg))
      {
        iPosition = oPos;
        iPosition.Y += 0.1f;
      }
    }
    else
      iPosition.Y += (this.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length * 0.5f + (this.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Radius;
    for (int index = 0; index < this.mStatusEffects.Length; ++index)
    {
      this.mStatusEffects[index].Stop();
      this.mStatusEffects[index] = new StatusEffect();
    }
    this.mCurrentAnimation = Magicka.Animations.idle;
    this.mCurrentAnimationSet = WeaponClass.Default;
    AnimationClipAction animationClipAction = this.mAnimationClips[(int) this.mCurrentAnimationSet][(int) this.mCurrentAnimation];
    this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
    this.mAnimationController.ClearCrossfadeQueue();
    this.mAnimationController.StartClip(animationClipAction.Clip, animationClipAction.LoopAnimation);
    this.mCurrentActions = animationClipAction.Actions;
    this.mForceAnimationUpdate = false;
    for (int index = 0; index < this.mCurrentActions.Length; ++index)
    {
      this.mExecutedActions.Add(false);
      this.mDeadActions.Add(false);
      if (this.mCurrentActions[index].UsesBones)
        this.mForceAnimationUpdate = true;
    }
    this.mNextAttackAnimation = Magicka.Animations.None;
    this.mSpawnAnimation = Magicka.Animations.None;
    this.mAttacking = false;
    this.mDefaultSpecular = (this.mModel.Model.Meshes[0].Effects[0] as SkinnedModelBasicEffect).SpecularPower;
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].SetMeshDirty();
      this.mNormalDistortionRenderData[index].SetMeshDirty();
      this.mShieldSkinRenderData[index].SetMeshDirty();
      this.mHighlightRenderData[index].SetMeshDirty();
      this.mLightningZapRenderData[index].SetMeshDirty();
    }
    iPosition.Y -= this.mHeightOffset;
    this.mBody.MoveTo(iPosition, Matrix.Identity);
    this.mBody.EnableBody();
    this.mVolume = this.Capsule.GetVolume();
    this.SetState((BaseState) IdleState.Instance);
    this.mDoNotRender = false;
    for (int index = 0; index < this.mAttachedSounds.Length; ++index)
      this.mAttachedSoundCues[index] = AudioManager.Instance.PlayCue(this.mAttachedSounds[index].Value, this.mAttachedSounds[index].Key, this.mAudioEmitter);
    for (int index = 0; index < iTemplate.Auras.Count; ++index)
    {
      AuraStorage aura = iTemplate.Auras[index];
      this.AddAura(ref aura, false);
    }
    this.mTimeDead = 0.0f;
  }

  protected virtual void ApplyTemplate(CharacterTemplate iTemplate, ref int iModel)
  {
    this.mType = iTemplate.ID;
    this.mName = iTemplate.Name;
    this.mDisplayName = iTemplate.DisplayName;
    this.mTemplateIsEthereal = iTemplate.IsEthereal;
    this.mEthereal = iTemplate.IsEthereal;
    this.mEtherealLook = iTemplate.LooksEthereal;
    this.mFearless = iTemplate.IsFearless;
    this.mUncharmable = iTemplate.IsUncharmable;
    this.mNonslippery = iTemplate.IsNonslippery;
    this.mHasFairy = iTemplate.HasFairy;
    this.mFaction = iTemplate.Faction;
    this.mBlood = iTemplate.Blood;
    this.mScoreValue = iTemplate.ScoreValue;
    this.mResistances = iTemplate.Resistances;
    this.mMaxPanic = iTemplate.MaxPanic;
    this.mZapModifier = iTemplate.ZapModifier;
    this.mMaxHitPoints = iTemplate.MaxHitpoints;
    this.mNumberOfHealtBars = iTemplate.NumberOfHealthBars;
    this.mHitPoints = this.mMaxHitPoints;
    this.mHitTolerance = (float) iTemplate.HitTolerance;
    this.mKnockdownTolerance = iTemplate.KnockdownTolerance;
    this.mRadius = iTemplate.Radius;
    this.mRegenRate = iTemplate.Regeneration;
    this.mBleedRate = iTemplate.BleedRate;
    this.mStunTime = iTemplate.StunTime;
    this.mEventConditions = iTemplate.EventConditions;
    this.mCanHasStatusEffect = !this.mName.Contains("elemental");
    this.mMoveAbilities = iTemplate.MoveAbilities;
    this.mMoveAnimations = iTemplate.MoveAnimations;
    this.mBreakFreeStrength = iTemplate.BreakFreeStrength;
    this.CharacterBody.MaxVelocity = iTemplate.Speed;
    this.mTurnSpeedMax = this.mTurnSpeed = iTemplate.TurnSpeed;
    this.mSummonElementCue = iTemplate.SummonElementCue;
    this.mSummonElementBank = iTemplate.SummonElementBank;
    this.mGibs.Clear();
    for (int index = 0; index < iTemplate.Gibs.Length; ++index)
      this.mGibs.Add(iTemplate.Gibs[index]);
    this.mAnimationClips = iTemplate.AnimationClips;
    if (iModel < 0)
      iModel = MagickaMath.Random.Next(0, iTemplate.Models.Length);
    else if (iModel >= iTemplate.Models.Length)
      iModel = 0;
    this.mModelID = iModel;
    this.mModel = iTemplate.Models[iModel].Model;
    this.mMouthJoint = iTemplate.MouthJoint;
    this.mHipJoint = iTemplate.HipJoint;
    this.mLeftHandJoint = iTemplate.LeftHandJoint;
    this.mLeftKneeJoint = iTemplate.LeftKneeJoint;
    this.mRightHandJoint = iTemplate.RightHandJoint;
    this.mRightKneeJoint = iTemplate.RightKneeJoint;
    this.mStaticTransform = iTemplate.Models[iModel].Transform;
    Matrix result;
    Matrix.Invert(ref this.mStaticTransform, out result);
    Matrix.Multiply(ref result, ref this.mMouthJoint.mBindPose, out this.mMouthJoint.mBindPose);
    Matrix.Multiply(ref result, ref this.mLeftHandJoint.mBindPose, out this.mLeftHandJoint.mBindPose);
    Matrix.Multiply(ref result, ref this.mRightHandJoint.mBindPose, out this.mRightHandJoint.mBindPose);
    this.mAnimationController.Skeleton = iTemplate.Skeleton;
    for (int index = 0; index < this.mEquipment.Length; ++index)
    {
      iTemplate.Equipment[index].ReAttach(this);
      iTemplate.Equipment[index].CopyToInstance(this.mEquipment[index]);
      this.mEquipment[index].TransformBindPose(ref result);
      this.mEquipment[index].ReAttach(this);
    }
    this.mSourceOfSpellIndex = -1;
    for (int index = 0; index < this.mEquipment.Length; ++index)
    {
      if (this.mEquipment[index].Item.WeaponClass == WeaponClass.Staff)
      {
        this.mSourceOfSpellIndex = index;
        break;
      }
    }
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
    this.mMaterial.TintColor = iTemplate.Models[iModel].Tint;
    this.mResistances = iTemplate.Resistances;
    for (int index = 0; index < this.mAttachedEffectsBoneIndex.Length; ++index)
      this.mAttachedEffectsBoneIndex[index] = -1;
    for (int index = 0; index < iTemplate.AttachedEffects.Length; ++index)
      this.AttachEffect(iTemplate.AttachedEffects[index].Value, iTemplate.AttachedEffects[index].Key);
    for (int index = 0; index < iTemplate.AttachedSounds.Length; ++index)
    {
      if (index >= this.mAttachedEffects.Length)
        this.mAttachedSounds[index] = iTemplate.AttachedSounds[index];
    }
    this.mPointLightHolder = iTemplate.PointLightHolder[0];
    this.mHeightOffset = (float) (-(double) iTemplate.Radius - 0.5 * (double) iTemplate.Length);
    (this.mCollision.GetPrimitiveLocal(0) as Capsule).Length = iTemplate.Length;
    (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Length = iTemplate.Length;
    (this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Length = iTemplate.Length;
    (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius = iTemplate.Radius;
    (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Radius = iTemplate.Radius;
    (this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Radius = iTemplate.Radius;
    Vector3 position = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Position with
    {
      Y = iTemplate.Length * -0.5f
    };
    (this.mCollision.GetPrimitiveLocal(0) as Capsule).Position = position;
    this.mBody.Mass = iTemplate.Mass;
    this.mTemplateMass = this.mBody.Mass;
    bool flag = (iTemplate.MoveAbilities & MovementProperties.Fly) == MovementProperties.Fly;
    this.CharacterBody.IsFlying = flag;
    this.mBody.ApplyGravity = !flag;
    this.mAttachedSounds = iTemplate.AttachedSounds;
  }

  public int AttachEffect(int iEffect, int iBoneIndex)
  {
    for (int index = 0; index < this.mAttachedEffects.Length; ++index)
    {
      if (this.mAttachedEffectsBoneIndex[index] < 0)
      {
        Matrix result1 = this.mAnimationController.Skeleton[iBoneIndex].InverseBindPoseTransform;
        Matrix.Invert(ref result1, out result1);
        Matrix.Multiply(ref Character.sRotateY180, ref result1, out this.mAttachedEffectsBindPose[index]);
        Matrix result2;
        Matrix.Multiply(ref this.mAttachedEffectsBindPose[index], ref this.mAnimationController.SkinnedBoneTransforms[iBoneIndex], out result2);
        VisualEffectReference oRef;
        EffectManager.Instance.StartEffect(iEffect, ref result2, out oRef);
        this.mAttachedEffects[index] = oRef;
        this.mAttachedEffectsBoneIndex[index] = iBoneIndex;
        return index;
      }
    }
    return -1;
  }

  public void StopAttachedEffect(int iIndex)
  {
    if (iIndex < 0)
      return;
    this.mAttachedEffectsBoneIndex[iIndex] = -1;
  }

  public Magicka.Animations SpawnAnimation
  {
    get => this.mSpawnAnimation;
    set => this.mSpawnAnimation = value;
  }

  public Magicka.Animations SpecialIdleAnimation
  {
    get => this.mSpecialIdleAnimation;
    set => this.mSpecialIdleAnimation = value;
  }

  public Magicka.Animations NetworkAnimation
  {
    get => this.mNetworkAnimation;
    set => this.mNetworkAnimation = value;
  }

  public float NetworkAnimationBlend => this.mNetworkAnimationBlend;

  public virtual bool IsImmortal => (double) this.mImmortalTime > 0.0;

  public void SetImmortalTime(float iTime) => this.mImmortalTime = iTime;

  public float CollisionIgnoreTime
  {
    get => this.mCollisionIgnoreTime;
    set => this.mCollisionIgnoreTime = value;
  }

  public void SetInvisible(float iTime)
  {
    this.mInvisibilityTimer = iTime;
    if ((double) iTime <= 0.0)
      return;
    this.RemoveSelfShield();
  }

  public float WaterDepth
  {
    get => this.mWaterDepth;
    set => this.mWaterDepth = value;
  }

  public virtual void Hypnotize(ref Vector3 iDirection, int iEffect)
  {
    iDirection.Y = 0.0f;
    this.CharacterBody.Movement = iDirection;
    this.mHypnotizeDirection = iDirection;
    this.mHypnotized = true;
    EffectManager.Instance.Stop(ref this.mHypnotizeEffect);
    this.mHypnotizeEffectID = iEffect;
    if (iEffect == 0)
      return;
    Vector3 position = this.Position;
    Vector3 direction = this.Direction;
    EffectManager.Instance.StartEffect(iEffect, ref position, ref direction, out this.mHypnotizeEffect);
  }

  public virtual void StopHypnotize()
  {
    this.mHypnotized = false;
    if (!EffectManager.Instance.IsActive(ref this.mHypnotizeEffect))
      return;
    EffectManager.Instance.Stop(ref this.mHypnotizeEffect);
  }

  public void StopLevitate()
  {
    this.mLevitateTimer = 0.0f;
    this.CharacterBody.ApplyGravity = true;
    if (!EffectManager.Instance.IsActive(ref this.mLevitateEffect))
      return;
    EffectManager.Instance.Stop(ref this.mLevitateEffect);
  }

  public void SetLevitate(float iTTL, int iEffect)
  {
    this.mLevitateTimer = iTTL;
    if ((double) iTTL > 0.0)
    {
      if (EffectManager.Instance.IsActive(ref this.mLevitateEffect))
        EffectManager.Instance.Stop(ref this.mLevitateEffect);
      this.CharacterBody.ApplyGravity = false;
      Vector3 position = this.Position;
      Vector3 direction = this.Direction;
      EffectManager.Instance.StartEffect(iEffect, ref position, ref direction, out this.mLevitateEffect);
    }
    else
      this.StopLevitate();
  }

  public bool IsHypnotized => this.mHypnotized;

  public bool IsInvisibile => (double) this.mInvisibilityTimer > 0.0;

  public void RemoveInvisibility() => this.mInvisibilityTimer = 0.0f;

  public List<GibReference> Gibs => this.mGibs;

  public float ZapTimer => this.mZapTimer;

  public float ZapModifier => this.mZapModifier;

  public int ScoreValue => this.mScoreValue;

  public bool DoNotRender
  {
    get => this.mDoNotRender;
    set => this.mDoNotRender = value;
  }

  internal void ResetRestingTimers()
  {
    this.mRestingMovementTimer = 1f;
    this.mRestingHealthTimer = 1f;
  }

  protected bool RestingMovement => (double) this.mRestingMovementTimer < 0.0;

  protected bool RestingHealth => (double) this.mRestingHealthTimer < 0.0;

  public float BleedRate => this.mBleedRate;

  public void Stun(float time) => this.mStunTimer = time;

  public void Unstun() => this.mStunTimer = 1f / 1000f;

  public bool IsStunned => (double) this.mStunTimer > 0.0 && !this.Dead;

  public bool IsLevitating => (double) this.mLevitateTimer > 0.0 && !this.Dead;

  public bool Undying
  {
    get => this.mUndying;
    set => this.mUndying = value;
  }

  public float UndyingTimer => this.mUndieTimer;

  public bool JustCastInvisible
  {
    get => this.mJustCastInvisible;
    set => this.mJustCastInvisible = value;
  }

  public Entity LastAttacker => this.mLastAttacker;

  public Elements LastDamageElement => this.mLastDamageElement;

  internal void KillAnimationActions()
  {
    for (int index = 0; index < this.mCurrentActions.Length; ++index)
    {
      if (!this.mDeadActions[index])
        this.mCurrentActions[index].Kill(this);
    }
  }

  public override void Deinitialize()
  {
    if (this.mCurrentSpell != null)
      this.mCurrentSpell.DeInitialize((ISpellCaster) this);
    this.mHitList.Clear();
    if (this.mLastDamageIndex >= 0)
      DamageNotifyer.Instance.ReleasNumber(this.mLastDamageIndex);
    this.mLastDamageIndex = -1;
    EffectManager.Instance.Stop(ref this.mStunEffect);
    EffectManager.Instance.Stop(ref this.mFearEffect);
    this.EndCharm();
    this.StopHypnotize();
    this.StopLevitate();
    this.mCurrentSpell = (SpellEffect) null;
    if (this.ChargeCue != null)
      this.ChargeCue.Stop(AudioStopOptions.Immediate);
    this.ReleaseAttachedCharacter();
    for (int index = 0; index < this.mEquipment.Length; ++index)
    {
      if (this.mEquipment[index].Item != null && this.mEquipment[index].Item.Attached)
        this.mEquipment[index].Item.Deinitialize();
    }
    for (int index = 0; index < this.mStatusEffectCues.Length; ++index)
    {
      if (this.mStatusEffectCues[index] != null)
        this.mStatusEffectCues[index].Stop(AudioStopOptions.AsAuthored);
    }
    this.mDialog = 0;
    for (int index = 0; index < this.mAttachedSoundCues.Length; ++index)
    {
      if (this.mAttachedSoundCues[index] != null && !this.mAttachedSoundCues[index].IsStopping)
        this.mAttachedSoundCues[index].Stop(AudioStopOptions.AsAuthored);
    }
    for (int index = 0; index < this.mAttachedEffects.Length; ++index)
    {
      if (this.mAttachedEffects[index].Hash != 0)
        EffectManager.Instance.Stop(ref this.mAttachedEffects[index]);
    }
    if (this.mPointLight != null)
    {
      this.mPointLight.Disable();
      this.mPointLight = (DynamicLight) null;
      this.mPointLightHolder.Enabled = false;
    }
    if (this.mStatusEffectLight != null)
    {
      this.mStatusEffectLight.Stop(false);
      this.mStatusEffectLight = (DynamicLight) null;
    }
    this.mBloatTimer = 0.0f;
    for (int index = 0; index < this.mAuras.Count; ++index)
    {
      VisualEffectReference mEffect = this.mAuras[index].mEffect;
      EffectManager.Instance.Stop(ref mEffect);
    }
    this.mAuras.Clear();
    this.mBuffs.Clear();
    for (int index = 0; index < this.mBuffEffects.Count; ++index)
    {
      VisualEffectReference mBuffEffect = this.mBuffEffects[index];
      EffectManager.Instance.Stop(ref mBuffEffect);
    }
    this.mBuffEffects.Clear();
    this.mBuffDecals.Clear();
    for (int index = 0; index < this.mCurrentActions.Length; ++index)
    {
      if (!this.mDeadActions[index])
        this.mCurrentActions[index].Kill(this);
    }
    base.Deinitialize();
  }

  internal bool IgnoreCollisionDamage => (double) this.mCollisionDamageGracePeriod > 0.0;

  protected virtual void UpdateAnimationController(
    float iDeltaTime,
    ref Matrix iOrientation,
    bool iUpdateTransforms)
  {
    this.mAnimationController.Update(iDeltaTime, ref iOrientation, iUpdateTransforms);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mCollidedWithCamera = false;
    Matrix orientation1 = this.GetOrientation();
    this.mCollision.GetPrimitiveOldWorld(0);
    this.mHaloBuffColor = new Vector4();
    for (int index = 0; index < this.mAuras.Count; ++index)
    {
      ActiveAura mAura = this.mAuras[index];
      mAura.Execute(this, iDeltaTime);
      if ((double) mAura.Aura.TTL <= 0.0)
      {
        if ((double) this.mAuraCycleTimer > (double) index)
          --this.mAuraCycleTimer;
        VisualEffectReference mEffect = this.mAuras[index].mEffect;
        EffectManager.Instance.Stop(ref mEffect);
        this.mAuras.RemoveAt(index);
        --index;
      }
      else
      {
        float scaleFactor = MathHelper.SmoothStep(1f, 0.0f, Math.Min(Math.Abs((float) index - this.mAuraCycleTimer), Math.Abs((float) index - (this.mAuraCycleTimer - (float) this.mAuras.Count))));
        this.mHaloBuffFade = 1f;
        Vector3 result = this.mAuras[index].Aura.Color;
        Vector3.Multiply(ref result, scaleFactor, out result);
        Vector3 vector3 = new Vector3(this.mHaloBuffColor.X, this.mHaloBuffColor.Y, this.mHaloBuffColor.Z);
        Vector3.Add(ref result, ref vector3, out result);
        this.mHaloBuffColor = new Vector4(result, 1f);
        Vector3 position = this.Position;
        Vector3 direction = this.Direction;
        VisualEffectReference mEffect = this.mAuras[index].mEffect;
        EffectManager.Instance.UpdatePositionDirection(ref mEffect, ref position, ref direction);
        this.mAuras[index] = mAura;
        if (mAura.SelfCasted)
        {
          position.Y += this.HeightOffset;
          Vector2 vector2 = new Vector2(0.0f, (float) (6 * this.mNumberOfHealtBars));
          Healthbars.Instance.AddHealthBar(position, this.mAuras[index].Aura.TTL / this.mAuras[index].StartTTL, this.mRadius, 1f, float.MaxValue, false, new Vector4?(new Vector4(Spell.SHIELDCOLOR, 1f)), new Vector2?(vector2));
        }
      }
    }
    this.mAuraCycleTimer = this.mAuras.Count <= 1 ? Math.Max(this.mAuraCycleTimer - iDeltaTime, 0.0f) : (this.mAuraCycleTimer + iDeltaTime) % (float) this.mAuras.Count;
    this.mCollisionDamageGracePeriod = MathHelper.Max(this.mCollisionDamageGracePeriod - iDeltaTime, 0.0f);
    if ((double) this.mBody.Velocity.LengthSquared() > 9.9999999747524271E-07)
      this.mRestingMovementTimer = 1f;
    else
      this.mRestingMovementTimer -= iDeltaTime;
    if ((double) this.mLastHitpoints != (double) this.mHitPoints || this.mCurrentStatusEffects != StatusEffects.None)
      this.mRestingHealthTimer = 1f;
    else
      this.mRestingHealthTimer -= iDeltaTime;
    if ((double) this.mLevitateTimer > 0.0)
    {
      this.mLevitateTimer -= iDeltaTime;
      Segment seg = new Segment();
      seg.Origin = this.Position;
      seg.Origin.Y += this.HeightOffset * 0.5f;
      seg.Delta.Y = (float) ((double) this.HeightOffset * 0.5 - 0.75);
      Vector3 vector3_1 = new Vector3();
      float num;
      Vector3 vector3_2;
      Vector3 vector3_3;
      if (this.mPlayState.Level.CurrentScene.LiquidSegmentIntersect(out num, out vector3_2, out vector3_3, ref seg, false, false, false) || this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector3_2, out vector3_3, out AnimatedLevelPart _, out int _, seg))
      {
        vector3_1.Y = (float) ((1.0 - (double) num) * 0.75);
        this.mLevitationFreeFallTimer = 0.0f;
      }
      else
      {
        vector3_1.Y = (float) (-(double) iDeltaTime * (0.5 + 1.5 * (double) this.mLevitationFreeFallTimer));
        this.mLevitationFreeFallTimer = Math.Min(this.mLevitationFreeFallTimer + iDeltaTime * 0.5f, 1f);
      }
      Vector3 result = this.mBody.Velocity;
      Vector3.Add(ref result, ref vector3_1, out result);
      this.mBody.Velocity = result;
      if ((double) this.mLevitateTimer <= 0.0)
      {
        this.StopLevitate();
      }
      else
      {
        Vector3 position = this.Position;
        position.Y += this.HeightOffset;
        Vector3 direction = this.Direction;
        if (!EffectManager.Instance.IsActive(ref this.mLevitateEffect))
          EffectManager.Instance.StartEffect(Levitate.MAGICK_EFFECT, ref position, ref direction, out this.mLevitateEffect);
        else
          EffectManager.Instance.UpdatePositionDirection(ref this.mLevitateEffect, ref position, ref direction);
      }
    }
    this.mHitList.Update(iDeltaTime);
    this.mLastDraw += iDeltaTime;
    this.mUndying = this.mTemplate.Undying;
    this.mMaxHitPoints = this.mTemplate.MaxHitpoints;
    this.mBuffRotation = MathHelper.WrapAngle(this.mBuffRotation + iDeltaTime * -0.2f);
    Matrix result1;
    Matrix.CreateFromYawPitchRoll(0.0f, -1.57079637f, this.mBuffRotation, out result1);
    result1.M11 *= this.mRadius * 3f;
    result1.M12 *= this.mRadius * 3f;
    result1.M13 *= this.mRadius * 3f;
    result1.M21 *= this.mRadius * 3f;
    result1.M22 *= this.mRadius * 3f;
    result1.M23 *= this.mRadius * 3f;
    result1.M31 *= this.mRadius * 1f;
    result1.M32 *= this.mRadius * 1f;
    result1.M33 *= this.mRadius * 1f;
    Vector3 result2 = this.Position;
    result1.M41 = result2.X;
    result1.M42 = result2.Y + this.mHeightOffset;
    result1.M43 = result2.Z;
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    for (int index = 0; index < this.mBuffs.Count; ++index)
    {
      switch (this.mBuffs[index].VisualCategory)
      {
        case VisualCategory.Offensive:
          ++num1;
          break;
        case VisualCategory.Defensive:
          ++num2;
          break;
        case VisualCategory.Special:
          ++num3;
          break;
      }
    }
    int num4 = 0;
    int num5 = 0;
    int num6 = 0;
    for (int index = 0; index < this.mBuffs.Count; ++index)
    {
      DecalManager.DecalReference oReference = this.mBuffDecals[index];
      BuffStorage mBuff = this.mBuffs[index];
      float num7 = 0.0f;
      float num8 = 0.0f;
      float num9 = 0.0f;
      switch (mBuff.VisualCategory)
      {
        case VisualCategory.Offensive:
          num7 = this.mBuffCycleTimerOffensive;
          num9 = (float) num1;
          num8 = (float) num4;
          ++num4;
          break;
        case VisualCategory.Defensive:
          num7 = this.mBuffCycleTimerDefensive;
          num9 = (float) num2;
          num8 = (float) num5;
          ++num5;
          break;
        case VisualCategory.Special:
          num7 = this.mBuffCycleTimerSpecial;
          num9 = (float) num3;
          num8 = (float) num6;
          ++num6;
          break;
      }
      float num10 = MathHelper.SmoothStep(1f, 0.0f, Math.Min(Math.Abs(num8 - num7), Math.Abs(num8 - (num7 - num9))));
      if (oReference.Index < 0)
      {
        if (mBuff.VisualCategory != VisualCategory.None)
        {
          Vector2 iScale = new Vector2();
          iScale.X = iScale.Y = this.mRadius * 1.5f;
          DecalManager.Instance.AddAlphaBlendedDecal((Decal) ((byte) 26 + mBuff.VisualCategory - (byte) 1), (AnimatedLevelPart) null, ref iScale, ref result2, new Vector3?(), ref new Vector3()
          {
            Y = 1f
          }, 1f, ref new Vector4()
          {
            X = mBuff.Color.X,
            Y = mBuff.Color.Y,
            Z = mBuff.Color.Z
          }, out oReference);
        }
      }
      else if (!this.mPlayState.IsInCutscene)
        DecalManager.Instance.SetDecal(ref oReference, 1f, ref result1, num10 * 0.8f);
      else
        DecalManager.Instance.SetDecal(ref oReference, 1f, ref result1, 0.0f);
      this.mBuffDecals[index] = oReference;
      if ((double) this.mBuffs[index].TTL > 0.0)
      {
        mBuff.Execute(this, iDeltaTime);
        this.mBuffs[index] = mBuff;
        if (this.mBuffs[index].Effect != 0)
        {
          Vector3 position = this.Position;
          Vector3 direction = this.Direction;
          VisualEffectReference mBuffEffect = this.mBuffEffects[index];
          EffectManager.Instance.UpdatePositionDirection(ref mBuffEffect, ref position, ref direction);
        }
      }
      else
      {
        this.mBuffs.RemoveAt(index);
        VisualEffectReference mBuffEffect = this.mBuffEffects[index];
        EffectManager.Instance.Stop(ref mBuffEffect);
        this.mBuffEffects.RemoveAt(index);
        switch (mBuff.VisualCategory)
        {
          case VisualCategory.Offensive:
            if ((double) this.mBuffCycleTimerOffensive > (double) index)
            {
              --this.mBuffCycleTimerOffensive;
              break;
            }
            break;
          case VisualCategory.Defensive:
            if ((double) this.mBuffCycleTimerDefensive > (double) index)
            {
              --this.mBuffCycleTimerDefensive;
              break;
            }
            break;
          case VisualCategory.Special:
            if ((double) this.mBuffCycleTimerSpecial > (double) index)
            {
              --this.mBuffCycleTimerSpecial;
              break;
            }
            break;
        }
        this.mBuffDecals.RemoveAt(index);
        --index;
      }
    }
    this.mBuffCycleTimerOffensive = num1 <= 1 ? Math.Max(this.mBuffCycleTimerOffensive - iDeltaTime, 0.0f) : (this.mBuffCycleTimerOffensive + iDeltaTime) % (float) num1;
    this.mBuffCycleTimerDefensive = num2 <= 1 ? Math.Max(this.mBuffCycleTimerDefensive - iDeltaTime, 0.0f) : (this.mBuffCycleTimerDefensive + iDeltaTime) % (float) num2;
    this.mBuffCycleTimerSpecial = num3 <= 1 ? Math.Max(this.mBuffCycleTimerSpecial - iDeltaTime, 0.0f) : (this.mBuffCycleTimerSpecial + iDeltaTime) % (float) num3;
    if ((double) this.mHitPoints > (double) this.mMaxHitPoints)
      this.mHitPoints = this.mMaxHitPoints;
    for (int index = 0; index < this.mBloatDamageAccumulation.Length; ++index)
      this.mBloatDamageAccumulation[index] = Math.Max(0.0f, this.mBloatDamageAccumulation[index] - iDeltaTime * 0.75f);
    this.mImmortalTime -= iDeltaTime;
    this.mCollisionIgnoreTime -= iDeltaTime;
    this.mNormalizedHitPoints = MathHelper.Clamp(MathHelper.Lerp(this.mHitPoints / this.mMaxHitPoints, this.mNormalizedHitPoints, (float) Math.Pow(0.002, (double) iDeltaTime)), 0.0f, 1f);
    this.BlockItem = -1;
    bool iUpdateTransforms = (double) this.mLastDraw < 0.20000000298023224 | this.mForceAnimationUpdate | this.IsInvisibile;
    if (!this.Dead | (double) this.mDeadTimer > -10.0 | !this.mAnimationController.HasFinished | this.mAnimationController.CrossFadeEnabled)
    {
      this.UpdateAnimationController(iDeltaTime, ref orientation1, iUpdateTransforms);
      if (iUpdateTransforms)
      {
        for (int index = 0; index < this.mAttachedEffects.Length; ++index)
        {
          if (this.mAttachedEffectsBoneIndex[index] >= 0)
          {
            Matrix boneOrientation = this.GetBoneOrientation(this.mAttachedEffectsBoneIndex[index], ref this.mAttachedEffectsBindPose[index]);
            EffectManager.Instance.UpdateOrientation(ref this.mAttachedEffects[index], ref boneOrientation);
            if (this.mAttachedEffects[index].ID < 0)
              this.mAttachedEffectsBoneIndex[index] = -1;
          }
        }
      }
      for (int index = 0; index < this.mCurrentActions.Length; ++index)
      {
        AnimationAction mCurrentAction = this.mCurrentActions[index];
        bool mExecutedAction = this.mExecutedActions[index];
        bool mDeadAction = this.mDeadActions[index];
        mCurrentAction.Execute(this, this.mAnimationController, ref mExecutedAction, ref mDeadAction);
        this.mExecutedActions[index] = mExecutedAction;
        this.mDeadActions[index] = mDeadAction;
      }
    }
    if (!this.Dead)
    {
      if (this.mDrowning || !this.CharacterBody.IsInWater)
        EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
      else if ((double) this.WaterDepth >= (double) Defines.FOOTSTEP_WATER_OFFSET)
      {
        Matrix identity = Matrix.Identity;
        Vector3 position = this.Position;
        position.Y += this.HeightOffset + this.WaterDepth;
        identity.Translation = position;
        if (EffectManager.Instance.IsActive(ref this.mWaterSplashEffect))
          EffectManager.Instance.UpdateOrientation(ref this.mWaterSplashEffect, ref identity);
        else if (this.CharacterBody.GroundMaterial == CollisionMaterials.Lava)
          EffectManager.Instance.StartEffect(Defines.LAVA_SPLASH_EFFECT, ref identity, out this.mWaterSplashEffect);
        else
          EffectManager.Instance.StartEffect(Defines.WATER_SPLASH_EFFECT, ref identity, out this.mWaterSplashEffect);
      }
      else
        EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
    }
    if ((double) this.mEtherealTimer > 0.0)
    {
      this.mEtherealTimer -= iDeltaTime;
      if ((double) this.mEtherealTimer <= 0.0)
      {
        this.mEthereal = this.mEtherealTimedState;
        this.mEtherealAlpha = 1f;
        this.Ethereal(this.mEtherealTimedState, 1f, 1f);
      }
    }
    this.mEtherealAlpha += (this.mEtherealAlphaTarget - this.mEtherealAlpha) * this.mEtherealAlphaSpeed * iDeltaTime;
    this.mAnimationController.Speed = 1f;
    this.CharacterBody.SpeedMultiplier = 1f;
    if (this.mSelfShield.Active)
      this.mSelfShield.Update(this, iDeltaTime);
    this.mMeleeBoostTimer -= iDeltaTime;
    if ((double) this.mMeleeBoostTimer <= 0.0)
      this.mMeleeBoostAmount = 0.0f;
    this.ChangeState(this.mCurrentState.Update(this, iDeltaTime));
    if ((double) this.mZapTimer >= 0.0)
    {
      if (!float.IsNaN(this.mZapModifier))
      {
        this.CharacterBody.SpeedMultiplier = this.mZapModifier;
        this.mAnimationController.Speed *= this.mZapModifier;
      }
      this.mZapTimer -= iDeltaTime;
    }
    if (this.mGrippedCharacter != null)
    {
      if ((double) this.mHitPoints < 0.0 || this.mGrippedCharacter.mDead || this.ShouldReleaseGrip)
      {
        if (this.ShouldReleaseGrip && this.mGrippedCharacter is Avatar && !((this.mGrippedCharacter as Avatar).Player.Gamer is NetworkGamer))
          AchievementsManager.Instance.AwardAchievement(this.PlayState, "houdini");
        this.ReleaseAttachedCharacter();
      }
      else
      {
        switch (this.mGripType)
        {
          case Grip.GripType.Pickup:
            Matrix result3;
            Matrix.Multiply(ref this.mGripJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mGripJoint.mIndex], out result3);
            result2 = result3.Translation;
            result3.M41 = result3.M42 = result3.M43 = 0.0f;
            this.mGrippedCharacter.Body.MoveTo(result2, result3);
            break;
          case Grip.GripType.Hold:
            this.mGrippedCharacter.mGripper = this;
            this.mGrippedCharacter.CharacterBody.SpeedMultiplier = 0.0f;
            this.mGrippedCharacter.CharacterBody.AllowRotate = false;
            Vector3 position1 = this.mGrippedCharacter.Position;
            Vector3 position2 = this.Position;
            Vector3 result4;
            Vector3.Subtract(ref position1, ref position2, out result4);
            if ((double) result4.LengthSquared() < 9.9999999747524271E-07)
              result4 = Vector3.Forward;
            else
              result4.Normalize();
            result2 = this.mGrippedCharacter.Position;
            Vector3 result5;
            Vector3.Multiply(ref result4, -this.mRadius - this.mGrippedCharacter.Radius, out result5);
            Vector3.Add(ref result2, ref result5, out result2);
            Matrix result6;
            Matrix.CreateWorld(ref result2, ref result4, ref new Vector3()
            {
              Y = 1f
            }, out result6);
            result6.M41 = result6.M42 = result6.M43 = 0.0f;
            this.Body.MoveTo(result2, result6);
            this.mGrippedCharacter.CharacterBody.AllowRotate = false;
            this.mGrippedCharacter.CharacterBody.AllowMove = false;
            break;
        }
      }
    }
    if (this.mGripper != null)
    {
      if (this.mDead | this.mGripper.mDead)
      {
        this.mGripper.ReleaseAttachedCharacter();
        this.mGripper = (Character) null;
      }
      else
      {
        switch (this.mGripType)
        {
          case Grip.GripType.Pickup:
          case Grip.GripType.Hold:
            this.mGripper.CharacterBody.SpeedMultiplier = 0.0f;
            break;
          case Grip.GripType.Ride:
            Matrix orientation2 = this.mGripper.Body.Orientation with
            {
              Translation = this.mGripper.Body.Position
            };
            Matrix result7;
            Matrix.Multiply(ref this.mGripper.mGripJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mGripJoint.mIndex], out result7);
            Character.sRotateSpines = Matrix.CreateFromYawPitchRoll(1.57079637f, 1.57079637f, 0.0f);
            Matrix.Multiply(ref Character.sRotateSpines, ref result7, out result7);
            Vector3 result8 = result7.Up;
            Vector3.Multiply(ref result8, -this.mGripper.mHeightOffset, out result8);
            result2 = result7.Translation;
            Vector3.Add(ref result2, ref result8, out result2);
            Matrix.Lerp(ref result7, ref orientation2, 0.5f, out result7);
            result7.M41 = result7.M42 = result7.M43 = 0.0f;
            this.mGripper.Body.MoveTo(result2, result7);
            break;
        }
      }
    }
    Vector3 result9 = this.Position;
    Vector3 direction1 = this.Direction;
    if (this.IsFeared)
    {
      this.mFearTimer -= iDeltaTime;
      if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFearEffect, ref result9, ref direction1))
        EffectManager.Instance.StartEffect(Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Fear.FEARED_EFFECT, ref result9, ref direction1, out this.mFearEffect);
      if (!this.IsFeared)
      {
        this.mFearTimer = 0.0f;
        EffectManager.Instance.Stop(ref this.mFearEffect);
      }
    }
    if (this.IsStunned)
    {
      this.mStunTimer -= iDeltaTime;
      if ((double) this.mStunTimer <= 0.0)
      {
        EffectManager.Instance.Stop(ref this.mStunEffect);
      }
      else
      {
        Matrix attachOrientation = this.GetMouthAttachOrientation();
        if (!EffectManager.Instance.UpdateOrientation(ref this.mStunEffect, ref attachOrientation))
          EffectManager.Instance.StartEffect(Character.STUN_EFFECT, ref attachOrientation, out this.mStunEffect);
      }
    }
    if (this.mCurrentSpell != null)
    {
      float oTurnSpeed;
      if (!this.mCurrentSpell.CastUpdate(iDeltaTime, (ISpellCaster) this, out oTurnSpeed) && this.mCurrentSpell.CastType == CastType.Weapon && !this.mCurrentSpell.Active && !(this.mCurrentState is CastState))
        this.mCurrentSpell = (SpellEffect) null;
      this.mTurnSpeed = this.mTurnSpeedMax * oTurnSpeed;
    }
    else if (!(this.mCurrentState is ChargeState))
      this.mTurnSpeed = this.mTurnSpeedMax;
    if (this.mBloating)
    {
      this.mBloatTimer += iDeltaTime;
      if ((double) this.mBloatTimer >= 0.33300000429153442)
        this.BloatBlast();
    }
    if (this.mDead)
    {
      if (!float.IsNaN(this.mUndieTimer) && !this.mOverkilled)
      {
        this.mUndieTimer -= iDeltaTime;
        if ((double) this.mUndieTimer <= 0.0)
        {
          this.mDead = false;
          this.mHitPoints = this.mTemplate.UndieHitPoints;
        }
      }
      else
      {
        this.mDeadTimer -= iDeltaTime;
        this.mShadowTimer += iDeltaTime;
        if ((double) this.mDeadTimer < 0.0 && this.mRemoveAfterDeath)
          this.CharacterBody.Position = new Vector3(this.Position.X, this.Position.Y - iDeltaTime * 0.3f, this.Position.Z);
      }
    }
    else
    {
      for (int iIndex = 0; iIndex < this.mEquipment.Length; ++iIndex)
      {
        Item obj = this.mEquipment[iIndex].Item;
        if (obj.Attached && !obj.AnimationDetached)
        {
          Matrix attachOrientation = this.GetItemAttachOrientation(iIndex);
          obj.Transform = attachOrientation;
          obj.IsInvisible = this.IsInvisibile;
          obj.Update(iDataChannel, iDeltaTime);
          if (iIndex == this.mSourceOfSpellIndex)
            this.mStaffOrb = obj.AttachAbsoluteTransform;
          else if (iIndex == 0)
            this.mWeaponTransform = obj.AttachAbsoluteTransform;
        }
      }
      if (this.mPointLightHolder.ContainsLight && !this.mPointLightHolder.Enabled)
      {
        this.mPointLightHolder.Enabled = true;
        this.mPointLight = DynamicLight.GetCachedLight();
        this.mPointLight.AmbientColor = this.mPointLightHolder.AmbientColor;
        this.mPointLight.DiffuseColor = this.mPointLightHolder.DiffuseColor;
        this.mPointLight.Radius = this.mPointLightHolder.Radius;
        this.mPointLight.SpecularAmount = this.mPointLightHolder.SpecularAmount;
        this.mPointLight.VariationAmount = this.mPointLightHolder.VariationAmount;
        this.mPointLight.VariationSpeed = this.mPointLightHolder.VariationSpeed;
        this.mPointLight.VariationType = this.mPointLightHolder.VariationType;
        this.mPointLight.Position = this.mPointLightHolder.Joint.mBindPose.Translation;
        this.mPointLight.Speed = 1f;
        this.mPointLight.Intensity = 1f;
        this.mPointLight.Enable(this.mPlayState.Scene);
      }
      else if (this.mPointLight != null && this.mPointLight.Enabled)
      {
        Matrix result10;
        if (this.mPointLightHolder.Joint.mIndex < 0)
          result10 = Matrix.Identity;
        else
          Matrix.Multiply(ref this.mPointLightHolder.Joint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mPointLightHolder.Joint.mIndex], out result10);
        this.mPointLight.Position = result10.Translation;
      }
    }
    this.UpdateDamage(iDeltaTime);
    this.UpdateStatusEffects(iDeltaTime);
    Vector3 position3 = this.Position;
    Vector3 direction2 = this.Direction;
    EffectManager.Instance.UpdatePositionDirection(ref this.mDryingEffect, ref position3, ref direction2);
    if (this.mCurrentState is PanicState || this.CurrentState is MoveState)
    {
      if ((double) this.CharacterBody.MaxVelocity > 0.0)
      {
        float f = (this.CharacterBody.Velocity with
        {
          Y = 0.0f
        }).Length() / this.CharacterBody.MaxVelocity;
        if (!float.IsNaN(f))
          this.mAnimationController.Speed *= f;
      }
      else
        this.mAnimationController.Speed = 1f;
    }
    else if (this.HasStatus(StatusEffects.Cold))
      this.mAnimationController.Speed *= this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown();
    if (this.IsHypnotized)
    {
      this.CharacterBody.SpeedMultiplier *= 0.4f;
      EffectManager.Instance.UpdatePositionDirection(ref this.mHypnotizeEffect, ref result9, ref direction1);
    }
    if (this.IsCharmed)
    {
      this.mCharmTimer -= iDeltaTime;
      EffectManager.Instance.UpdatePositionDirection(ref this.mCharmEffect, ref result9, ref direction1);
      if ((double) this.mCharmTimer <= 0.0)
        this.EndCharm();
    }
    BoundingSphere boundingSphere = this.mModel.Model.Meshes[0].BoundingSphere with
    {
      Center = this.Position
    };
    boundingSphere.Radius *= this.mBoundingScale * this.mStaticTransform.M11;
    this.mEntaglement.Update(iDataChannel, iDeltaTime, ref boundingSphere);
    this.UpdateRenderData(iDataChannel, boundingSphere, iDeltaTime);
    this.mHeightOffset = -0.5f * this.Capsule.Length - this.Capsule.Radius;
    if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude > 0.0 || this.mBloating)
    {
      this.mAnimationController.Speed = 0.0f;
      this.CharacterBody.SpeedMultiplier = 0.0f;
      this.mTurnSpeed = 0.0f;
      this.CharacterBody.Movement = Vector3.Zero;
    }
    if (iUpdateTransforms && (double) this.mShadowTimer <= 1.1000000238418579 && !this.IsEthereal)
    {
      result9 = (Matrix.CreateRotationY(3.14159274f) * Matrix.Invert(this.mModel.SkeletonBones[1].InverseBindPoseTransform)).Translation;
      Vector3.Transform(ref result9, ref this.mAnimationController.SkinnedBoneTransforms[1], out result9);
      result9.Y += this.HeightOffset;
      ShadowBlobs.Instance.AddShadowBlob(ref result9, this.mRadius, this.mShadowTimer);
      if ((double) this.mHitPoints <= 0.0)
        this.mShadowTimer += iDeltaTime;
    }
    for (int index = 0; index < this.mStatusEffectCues.Length; ++index)
    {
      if (this.mStatusEffectCues[index] != null)
        this.mStatusEffectCues[index].Apply3D(this.mPlayState.Camera.Listener, this.mAudioEmitter);
    }
    if (this.mLastAttacker != null && this.mCurrentStatusEffects == StatusEffects.None)
    {
      if ((double) this.mLastAttackerTimer <= 0.0)
        this.mLastAttacker = (Entity) null;
      this.mLastAttackerTimer -= iDeltaTime;
    }
    base.Update(iDataChannel, iDeltaTime);
  }

  public void OnSpawnedSummon(NonPlayerCharacter npc)
  {
    while (this.mNumCurrentUndeadSummons >= 1)
      this.RemoveUndead();
    while (this.mNumCurrentFlamerSummons >= 4)
      this.RemoveFlamer();
    while (this.mNumCurrentSummons >= 16 /*0x10*/)
    {
      NonPlayerCharacter mCurrentSummon = this.mCurrentSummons[0];
      this.RemoveSummon(0);
      mCurrentSummon.OverKill();
    }
    this.mCurrentSummons[this.mNumCurrentSummons] = npc;
    ++this.mNumCurrentSummons;
    if (npc.IsUndeadSummon)
    {
      ++this.mNumCurrentUndeadSummons;
    }
    else
    {
      if (!npc.IsFlamerSummon)
        return;
      ++this.mNumCurrentFlamerSummons;
    }
  }

  private void RemoveUndead()
  {
    for (int position = 0; position < this.mCurrentSummons.Length; ++position)
    {
      NonPlayerCharacter mCurrentSummon = this.mCurrentSummons[position];
      if (mCurrentSummon != null && mCurrentSummon.IsUndeadSummon)
      {
        this.RemoveSummon(position);
        mCurrentSummon.OverKill();
        --this.mNumCurrentUndeadSummons;
        break;
      }
    }
  }

  private void RemoveFlamer()
  {
    for (int position = 0; position < this.mCurrentSummons.Length; ++position)
    {
      NonPlayerCharacter mCurrentSummon = this.mCurrentSummons[position];
      if (mCurrentSummon != null && mCurrentSummon.IsFlamerSummon)
      {
        this.RemoveSummon(position);
        mCurrentSummon.OverKill();
        --this.mNumCurrentFlamerSummons;
        break;
      }
    }
  }

  private void RemoveSummon(int position)
  {
    int index1 = Math.Min(this.mNumCurrentSummons, 15);
    for (int index2 = position; index2 < index1; ++index2)
      this.mCurrentSummons[index2] = this.mCurrentSummons[index2 + 1];
    if (index1 < 0)
      return;
    this.mCurrentSummons[index1] = (NonPlayerCharacter) null;
    --this.mNumCurrentSummons;
  }

  public void OnDespawnedSummon(NonPlayerCharacter npc)
  {
    int position = -1;
    for (int index = 0; index < this.mNumCurrentSummons; ++index)
    {
      if (this.mCurrentSummons[index] == npc)
      {
        position = index;
        break;
      }
    }
    if (position < 0)
      return;
    if (this.mCurrentSummons[position].IsUndeadSummon)
      this.RemoveUndead();
    else
      this.RemoveSummon(position);
  }

  protected void UpdateRenderData(
    DataChannel iDataChannel,
    BoundingSphere iBoundingSphere,
    float iDeltaTime)
  {
    if (this.mDoNotRender)
      return;
    int count = this.mModel.SkeletonBones.Count;
    Character.RenderData iObject1 = this.mRenderData[(int) iDataChannel];
    Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, 0, (Array) iObject1.mSkeleton, 0, count);
    iObject1.mBoundingSphere = iBoundingSphere;
    iObject1.mMaterial.CubeNormalMapEnabled = false;
    Vector3.Multiply(ref this.mMaterial.DiffuseColor, MathHelper.Clamp((float) (1.0 - (double) this.mHitByLightning * 10.0), 0.0f, 1f), out iObject1.mMaterial.DiffuseColor);
    float num1 = Math.Min(this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude * 10f, 1f);
    iObject1.mMaterial.Colorize.X = Character.ColdColor.X;
    iObject1.mMaterial.Colorize.Y = Character.ColdColor.Y;
    iObject1.mMaterial.Colorize.Z = Character.ColdColor.Z;
    iObject1.mMaterial.Colorize.W = num1;
    iObject1.mMaterial.Bloat = (float) ((double) this.mBloatTimer / 0.33300000429153442 * 0.33300000429153442);
    Vector3.Multiply(ref iObject1.mMaterial.DiffuseColor, 1f + num1, out iObject1.mMaterial.DiffuseColor);
    if (this.HasStatus(StatusEffects.Frozen))
    {
      iObject1.mMaterial.CubeMapRotation = Matrix.Identity;
      iObject1.mMaterial.Bloat = (float) (int) ((double) this.StatusMagnitude(StatusEffects.Frozen) * 10.0) * 0.02f;
      iObject1.mMaterial.EmissiveAmount = 0.5f;
      iObject1.mMaterial.SpecularBias = 1f;
      iObject1.mMaterial.SpecularPower = 10f;
      iObject1.mMaterial.CubeMapEnabled = true;
      iObject1.mMaterial.CubeNormalMapEnabled = true;
      iObject1.mMaterial.CubeMap = Character.sIceCubeMap;
      iObject1.mMaterial.CubeNormalMap = Character.sIceCubeNormalMap;
      iObject1.mMaterial.CubeMapColor.X = iObject1.mMaterial.CubeMapColor.Y = iObject1.mMaterial.CubeMapColor.Z = 1f;
      iObject1.mMaterial.CubeMapColor.W = (double) this.StatusMagnitude(StatusEffects.Frozen) > 0.0 ? 1f : 0.0f;
    }
    else if (this.HasStatus(StatusEffects.Wet) || this.mDrowning)
    {
      this.mMapRotation += iDeltaTime;
      this.mMapScale += iDeltaTime * 5f;
      iObject1.mMaterial.ProjectionMap = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/wetMap");
      iObject1.mMaterial.ProjectionMapEnabled = true;
      iObject1.mMaterial.CubeMapEnabled = false;
      Vector3 axis = this.Position + Vector3.Up + Vector3.Forward - this.Position;
      Vector3 result1 = this.Direction;
      float angle = (float) Math.Sin((double) this.mMapRotation) - 0.5f;
      Quaternion result2;
      Quaternion.CreateFromAxisAngle(ref axis, angle, out result2);
      Vector3.Transform(ref result1, ref result2, out result1);
      float scale = (float) (1.0 + Math.Sin((double) this.mMapScale) * 0.0);
      iObject1.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Up + new Vector3(0.0f, 0.0f, -1f), this.Position, result1) * Matrix.CreateScale(scale);
      iObject1.mMaterial.SpecularBias = 0.5f;
      iObject1.mMaterial.SpecularPower = this.mMaterial.SpecularPower * 2f;
      Vector3 wetColor = Character.WetColor;
      Vector3.Multiply(ref iObject1.mMaterial.DiffuseColor, ref wetColor, out iObject1.mMaterial.DiffuseColor);
    }
    else if (this.HasStatus(StatusEffects.Burning))
    {
      this.mMapRotation -= iDeltaTime;
      this.mMapScale += iDeltaTime;
      iObject1.mMaterial.ProjectionMap = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/burnMap");
      iObject1.mMaterial.ProjectionMapEnabled = true;
      iObject1.mMaterial.CubeMapEnabled = false;
      iObject1.mMaterial.ProjectionMapColor = new Vector4(1f);
      iObject1.mMaterial.ProjectionMapColor.W = this.StatusMagnitude(StatusEffects.Burning);
      iObject1.mMaterial.ProjectionMapAdditive = true;
      Vector3 axis = this.Position + Vector3.Up - this.Position;
      Vector3 result3 = this.Direction;
      Quaternion result4;
      Quaternion.CreateFromAxisAngle(ref axis, this.mMapRotation, out result4);
      Vector3.Transform(ref result3, ref result4, out result3);
      float scale = (float) (1.0 + (Math.Sin((double) this.mMapScale) - 0.25) * 0.5);
      iObject1.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Up, this.Position, result3) * Matrix.CreateScale(scale);
      iObject1.mMaterial.SpecularBias = 0.0f;
      iObject1.mMaterial.SpecularPower = this.mMaterial.SpecularPower;
    }
    else if (this.HasStatus(StatusEffects.Greased))
    {
      this.mMapRotation += iDeltaTime * 0.2f;
      this.mMapScale += iDeltaTime;
      iObject1.mMaterial.ProjectionMap = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/Greased");
      iObject1.mMaterial.ProjectionMapEnabled = true;
      iObject1.mMaterial.CubeMapEnabled = false;
      Vector3 axis = this.Position + Vector3.Down - this.Position;
      Vector3 result5 = this.Direction;
      Quaternion result6;
      Quaternion.CreateFromAxisAngle(ref axis, this.mMapRotation, out result6);
      Vector3.Transform(ref result5, ref result6, out result5);
      float scale = (float) (1.0 + Math.Sin((double) this.mMapScale) * 0.20000000298023224);
      iObject1.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Down, this.Position, result5) * Matrix.CreateScale(scale);
      iObject1.mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
      iObject1.mMaterial.SpecularBias = 0.5f;
      iObject1.mMaterial.SpecularPower = this.mMaterial.SpecularPower * 2f;
    }
    else
    {
      iObject1.mMaterial.SpecularPower = this.mMaterial.SpecularPower;
      iObject1.mMaterial.SpecularAmount = this.mMaterial.SpecularAmount;
      iObject1.mMaterial.SpecularBias = 0.0f;
      iObject1.mMaterial.ProjectionMapEnabled = false;
      iObject1.mMaterial.CubeMapEnabled = false;
      iObject1.mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
    }
    this.mHitByLightning -= iDeltaTime;
    iObject1.mMaterial.Damage = (float) (1.0 - (double) this.mHitPoints / (double) this.mMaxHitPoints);
    ModelMesh mesh = this.mModel.Model.Meshes[0];
    ModelMeshPart meshPart = mesh.MeshParts[0];
    if (iObject1.MeshDirty)
    {
      SkinnedModelDeferredEffect.Technique activeTechnique = (SkinnedModelDeferredEffect.Technique) (meshPart.Effect as SkinnedModelBasicEffect).ActiveTechnique;
      iObject1.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, ref this.mMaterial, activeTechnique);
    }
    if (this.mHighlight)
    {
      Character.HighlightRenderData iObject2 = this.mHighlightRenderData[(int) iDataChannel];
      if (iObject2.MeshDirty)
        iObject2.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart);
      iObject2.mBoundingSphere = iBoundingSphere;
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject2);
    }
    if (this.IsInvisibile)
    {
      Character.NormalDistortionRenderData iObject3 = this.mNormalDistortionRenderData[(int) iDataChannel];
      if (iObject3.MeshDirty)
        iObject3.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart);
      this.mPlayState.Scene.AddPostEffect(iDataChannel, (IPostEffect) iObject3);
    }
    else if ((double) this.mZapTimer > 0.0)
    {
      this.mLastDraw = 0.0f;
      iObject1.mMaterial.DiffuseColor = new Vector3();
      iObject1.mMaterial.SpecularAmount = 0.0f;
      Character.LightningZapRenderData iObject4 = this.mLightningZapRenderData[(int) iDataChannel];
      if (iObject4.MeshDirty)
        iObject4.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, ref this.mMaterial, this.mTemplate.SkeletonVertices, this.mTemplate.SkeletonVertexDeclaration, this.mTemplate.SkeletonVertexStride, this.mTemplate.SkeletonPrimitiveCount);
      iObject4.mBoundingSphere = iBoundingSphere;
      this.mPlayState.Scene.AddPostEffect(iDataChannel, (IPostEffect) iObject4);
    }
    else if (this.mEthereal || this.mEtherealLook)
    {
      iObject1.mTechnique = SkinnedModelDeferredEffect.Technique.Additive;
      iObject1.mMaterial.Colorize = new Vector4(Character.ColdColor, 1f);
      iObject1.mMaterial.Alpha = this.mEtherealAlpha;
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject1);
    }
    else
    {
      iObject1.mTechnique = SkinnedModelDeferredEffect.Technique.Default;
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject1);
    }
    if (this.mSelfShield.Active)
    {
      switch (this.mSelfShield.mShieldType)
      {
        case Character.SelfShieldType.Shield:
          Character.ShieldSkinRenderData iObject5 = this.mShieldSkinRenderData[(int) iDataChannel];
          iObject5.mBoundingSphere = iBoundingSphere;
          Matrix result7;
          Matrix.CreateRotationY(2.09439516f, out result7);
          Vector3 cameraUpVector = new Vector3();
          cameraUpVector.Y = 1f;
          Vector3 position = this.Position;
          Vector3 direction = this.Direction;
          Vector3 result8;
          Vector3.Add(ref position, ref direction, out result8);
          Matrix.CreateLookAt(ref position, ref result8, ref cameraUpVector, out iObject5.mProjectionMatrix0);
          Matrix.Multiply(ref iObject5.mProjectionMatrix0, ref result7, out iObject5.mProjectionMatrix1);
          Matrix.Multiply(ref iObject5.mProjectionMatrix1, ref result7, out iObject5.mProjectionMatrix2);
          iObject5.mColor.X = Spell.SHIELDCOLOR.X;
          iObject5.mColor.Y = Spell.SHIELDCOLOR.Y;
          iObject5.mColor.Z = Spell.SHIELDCOLOR.Z;
          iObject5.mColor.W = (float) (4.0 + (double) MathHelper.Max(0.333f - this.mSelfShield.mTimeSinceDamage, 0.0f) * 60.0);
          if (iObject5.MeshDirty)
            iObject5.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart);
          iObject5.mTextureOffset0 = this.mSelfShield.mNoiseOffset0;
          iObject5.mTextureOffset1 = this.mSelfShield.mNoiseOffset1;
          iObject5.mTextureOffset2 = this.mSelfShield.mNoiseOffset2;
          iObject5.mTextureScale.X = iObject5.mTextureScale.Y = 0.5f;
          this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject5);
          break;
        case Character.SelfShieldType.Earth:
        case Character.SelfShieldType.Ice:
        case Character.SelfShieldType.IcedEarth:
          Character.BarrierSkinRenderData iObject6 = this.mBarrierSkinRenderData[(int) iDataChannel];
          iObject6.mBoundingSphere = iBoundingSphere;
          iObject6.RenderIce = false;
          iObject6.RenderEarth = false;
          if ((double) this.mSelfShield.mSpell[Elements.Ice] > 0.0)
          {
            iObject6.RenderIce = true;
            iObject6.mIceMaterial.OverlayAlpha = MathHelper.Clamp((float) (((double) this.mSelfShield.mSpell.IceMagnitude - 1.0) * 0.33333298563957214), 0.0f, 1f);
          }
          if ((double) this.mSelfShield.mSpell[Elements.Earth] > 0.0)
          {
            iObject6.RenderEarth = true;
            iObject6.mMaterial.OverlayAlpha = MathHelper.Clamp((float) (((double) this.mSelfShield.mSpell.EarthMagnitude - 1.0) * 0.33333298563957214), 0.0f, 1f);
          }
          this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject6);
          break;
      }
    }
    if (this.mAuras.Count > 0 || (double) this.mHaloBuffFade > 0.0)
    {
      Character.HaloAuraRenderData iObject7 = this.mArmourRenderData[(int) iDataChannel];
      Vector3 translation = this.GetHipAttachOrientation().Translation;
      translation.Y += (float) (-(double) this.HeightOffset * 0.5);
      iObject7.Position = translation;
      iObject7.mBoundingSphere = iBoundingSphere;
      this.mAuraPulsation = MathHelper.WrapAngle(this.mAuraPulsation + iDeltaTime * 2f);
      float num2 = (float) (Math.Sin((double) this.mAuraPulsation) * 0.30000001192092896 + 0.699999988079071);
      iObject7.ColorTint.X = this.mHaloBuffColor.X;
      iObject7.ColorTint.Y = this.mHaloBuffColor.Y;
      iObject7.ColorTint.Z = this.mHaloBuffColor.Z;
      iObject7.ColorTint.W = num2 * this.mHaloBuffFade;
      this.mAuraRays1Rotation = MathHelper.WrapAngle(this.mAuraRays1Rotation + iDeltaTime * 0.666f);
      Matrix.CreateRotationZ(this.mAuraRays1Rotation, out iObject7.Ray1Transform);
      this.mAuraRays2Rotation = MathHelper.WrapAngle(this.mAuraRays2Rotation - iDeltaTime * 0.333f);
      Matrix.CreateRotationZ(this.mAuraRays2Rotation, out iObject7.Ray2Transform);
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject7);
      this.mHaloBuffFade -= iDeltaTime;
    }
    this.mHighlight = false;
  }

  protected void UpdateDamage(float iDeltaTime)
  {
    if ((double) this.mHitPoints > 0.0 && (double) this.mHitPoints <= (double) this.mMaxHitPoints && !this.HasStatus(StatusEffects.Burning) && !this.HasStatus(StatusEffects.Poisoned))
    {
      this.mTotalRegenAccumulation += iDeltaTime * (float) this.mRegenRate;
      this.mHitPoints += iDeltaTime * (float) this.mRegenRate;
      this.mRegenTimer -= iDeltaTime;
      if ((double) this.mRegenTimer <= 0.0)
      {
        this.mRegenTimer = 1f;
        this.mTotalRegenAccumulation -= (float) this.mRegenRate;
        Vector3 position = this.Position;
        position.Y += this.Capsule.Length * 0.5f + this.mRadius;
        if (this.mRegenRate != 0 && GameStateManager.Instance.CurrentState is PlayState && !(GameStateManager.Instance.CurrentState as PlayState).IsInCutscene && (double) this.mHitPoints <= (double) this.mMaxHitPoints)
          DamageNotifyer.Instance.AddNumber((float) -this.mRegenRate, ref position, 0.7f, false);
      }
      if ((double) this.mHitPoints > (double) this.mMaxHitPoints)
        this.mHitPoints = this.mMaxHitPoints;
    }
    Vector3 position1 = this.Position;
    position1.Y += this.Capsule.Length * 0.5f + this.mRadius;
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
        DamageNotifyer.Instance.UpdateNumberPosition(this.mLastDamageIndex, ref position1);
    }
    this.mHealingAccumulationTimer -= iDeltaTime;
    if ((double) this.mHealingAccumulationTimer <= 0.0)
    {
      ++this.mHealingAccumulationTimer;
      if ((double) this.mHealingAccumulation != 0.0)
      {
        DamageNotifyer.Instance.AddNumber(this.mHealingAccumulation, ref position1, 0.7f, false);
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
        Vector3 firecolor = Spell.FIRECOLOR;
        DamageNotifyer.Instance.AddNumber(this.mFireDamageAccumulation, ref position1, 0.7f, false, ref firecolor);
        this.mFireDamageAccumulation = 0.0f;
        this.mTimeSinceLastStatusDamage = 0.0f;
      }
    }
    this.mPoisonDamageAccumulationTimer -= iDeltaTime;
    if ((double) this.mPoisonDamageAccumulationTimer <= 0.0)
    {
      ++this.mPoisonDamageAccumulationTimer;
      if ((double) this.mPoisonDamageAccumulation != 0.0)
      {
        Vector3 poisoncolor = Spell.POISONCOLOR;
        DamageNotifyer.Instance.AddNumber(this.mPoisonDamageAccumulation, ref position1, 0.7f, false, ref poisoncolor);
        this.mPoisonDamageAccumulation = 0.0f;
        this.mTimeSinceLastStatusDamage = 0.0f;
      }
    }
    this.mBleedDamageAccumulationTimer -= iDeltaTime;
    if ((double) this.mBleedDamageAccumulationTimer > 0.0)
      return;
    ++this.mBleedDamageAccumulationTimer;
    if ((double) this.mBleedDamageAccumulation == 0.0)
      return;
    Vector3 iColor = new Vector3(1.25f, 0.1254902f, 0.0627451f);
    DamageNotifyer.Instance.AddNumber(this.mBleedDamageAccumulation, ref position1, 0.7f, false, ref iColor);
    this.mBleedDamageAccumulation = 0.0f;
    this.mTimeSinceLastStatusDamage = 0.0f;
  }

  protected void UpdateStatusEffects(float iDeltaTime)
  {
    this.mDryTimer -= iDeltaTime;
    StatusEffects statusEffects = StatusEffects.None;
    int index1 = StatusEffect.StatusIndex(StatusEffects.Burning);
    int index2 = StatusEffect.StatusIndex(StatusEffects.Wet);
    StatusEffect.StatusIndex(StatusEffects.Cold);
    int index3 = StatusEffect.StatusIndex(StatusEffects.Frozen);
    int index4 = StatusEffect.StatusIndex(StatusEffects.Steamed);
    if (this.Dead)
    {
      for (int index5 = 0; index5 < this.mStatusEffects.Length; ++index5)
      {
        switch (this.mStatusEffects[index5].DamageType)
        {
          case StatusEffects.Burning:
            if (this.mStatusEffectCues[index1] != null && !this.mStatusEffectCues[index1].IsStopping)
            {
              this.mStatusEffectCues[index1].Stop(AudioStopOptions.AsAuthored);
              break;
            }
            break;
          case StatusEffects.Wet:
            if (this.mStatusEffectCues[index2] != null && !this.mStatusEffectCues[index2].IsStopping)
            {
              this.mStatusEffectCues[index2].Stop(AudioStopOptions.AsAuthored);
              break;
            }
            break;
          case StatusEffects.Frozen:
            if (this.mStatusEffectCues[index3] != null && !this.mStatusEffectCues[index3].IsStopping)
            {
              this.mStatusEffectCues[index3].Stop(AudioStopOptions.AsAuthored);
              break;
            }
            break;
        }
        this.mStatusEffects[index5].Stop();
        this.mStatusEffects[index5] = new StatusEffect();
      }
    }
    else
    {
      for (int index6 = 0; index6 < this.mStatusEffects.Length; ++index6)
      {
        this.mStatusEffects[index6].Update(iDeltaTime, (IStatusEffected) this);
        if (this.mStatusEffects[index6].Dead)
        {
          switch (this.mStatusEffects[index6].DamageType)
          {
            case StatusEffects.Burning:
              if (this.mStatusEffectCues[index1] != null && !this.mStatusEffectCues[index1].IsStopping)
              {
                this.mStatusEffectCues[index1].Stop(AudioStopOptions.AsAuthored);
                break;
              }
              break;
            case StatusEffects.Wet:
              if (this.mStatusEffectCues[index2] != null && !this.mStatusEffectCues[index2].IsStopping)
              {
                this.mStatusEffectCues[index2].Stop(AudioStopOptions.AsAuthored);
                break;
              }
              break;
            case StatusEffects.Frozen:
              if (this.mStatusEffectCues[index3] != null && !this.mStatusEffectCues[index3].IsStopping)
              {
                this.mStatusEffectCues[index3].Stop(AudioStopOptions.AsAuthored);
                break;
              }
              break;
          }
          this.mStatusEffects[index6].Stop();
          this.mStatusEffects[index6] = new StatusEffect();
        }
        else
          statusEffects |= this.mStatusEffects[index6].DamageType;
      }
    }
    if (this.mStatusEffectLight != null)
      this.mStatusEffectLight.Position = this.Position;
    if (this.HasStatus(StatusEffects.Steamed))
      this.mPanic = Math.Max(this.mStatusEffects[index4].Magnitude - this.mMaxPanic, 0.0f);
    else if (this.HasStatus(StatusEffects.Burning))
    {
      this.mPanic = Math.Max(this.mStatusEffects[index1].Magnitude - this.mMaxPanic, 0.0f);
      if (this.mStatusEffectLight == null)
      {
        this.mStatusEffectLight = DynamicLight.GetCachedLight();
        this.mStatusEffectLight.Initialize(this.Position, new Vector3(1f, 0.4f, 0.0f), 1f, 5f, 1f, 0.5f);
        this.mStatusEffectLight.VariationType = LightVariationType.Candle;
        this.mStatusEffectLight.VariationSpeed = 4f;
        this.mStatusEffectLight.VariationAmount = 0.2f;
        this.mStatusEffectLight.AmbientColor = new Vector3(0.4f, 0.2f, 0.0f);
        this.mStatusEffectLight.Enable();
      }
    }
    else if (this.mStatusEffectLight != null)
    {
      this.mPanic = 0.0f;
      this.mStatusEffectLight.Stop(false);
      this.mStatusEffectLight = (DynamicLight) null;
    }
    this.mCurrentStatusEffects = statusEffects;
  }

  public virtual void BreakFree()
  {
    this.mBreakFreeCounter += (int) this.mBreakFreeStrength;
    this.DamageEntanglement(0.5f, Elements.Earth);
  }

  public void Highlight()
  {
    if (this.mDialog == 0)
      return;
    this.mHighlight = true;
  }

  private void BloatBlast()
  {
    if (this.mBloatElement != Elements.None)
    {
      float iRadius = this.mRadius + 2.5f;
      Magicka.GameLogic.Damage iDamage;
      iDamage.AttackProperty = AttackProperties.Damage;
      iDamage.Element = this.mBloatElement;
      iDamage.Amount = this.mBloatElement != Elements.Life ? Defines.SPELL_DAMAGE_ARCANE : Defines.SPELL_DAMAGE_LIFE;
      iDamage.Magnitude = (float) (1.0 + (double) this.MaxHitPoints / 5000.0);
      int num = (int) Blast.FullBlast(this.mPlayState, this.mBloatKiller, this.PlayState.PlayTime, (Entity) null, iRadius, this.Position, iDamage);
      AudioManager.Instance.PlayCue(Banks.Spells, Railgun.ARCANESTAGESOUNDSHASH[3], this.AudioEmitter);
    }
    this.mBloating = false;
    if (this.HasGibs())
    {
      this.mBloatKilled = true;
      this.OverKill();
    }
    else
      this.Kill();
  }

  public Entity CharmOwner => this.mCharmOwner;

  public virtual void Charm(Entity iCassanova, float iTTL, int iEffect)
  {
    if (this.IsUncharmable)
      return;
    this.mCharmOwner = iCassanova;
    this.mCharmTimer = iTTL;
    EffectManager.Instance.Stop(ref this.mCharmEffect);
    this.mCharmEffectID = iEffect;
    if (iEffect == 0)
      return;
    Vector3 position = this.Position;
    Vector3 direction = this.Direction;
    EffectManager.Instance.StartEffect(iEffect, ref position, ref direction, out this.mCharmEffect);
  }

  public bool IsCharmed => (double) this.mCharmTimer > 1.4012984643248171E-45;

  public virtual void EndCharm()
  {
    this.mCharmTimer = 0.0f;
    EffectManager.Instance.Stop(ref this.mCharmEffect);
  }

  public Vector3 FearPosition
  {
    get => this.mFearedBy != null ? this.mFearedBy.Position : this.mFearPosition;
  }

  public void Fear(Vector3 iPosition)
  {
    if ((this.mFaction & (Factions.DEMON | Factions.UNDEAD)) != Factions.NONE || this.IsFearless)
      return;
    this.mFearTimer = 5f;
    this.mFearedBy = (Character) null;
    this.mFearPosition = iPosition;
    if (EffectManager.Instance.IsActive(ref this.mFearEffect))
      return;
    Vector3 position = this.Position;
    Vector3 direction = this.Direction;
    EffectManager.Instance.StartEffect(Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Fear.FEARED_EFFECT, ref position, ref direction, out this.mFearEffect);
  }

  public void Fear(Character iFearedBy)
  {
    if ((this.mFaction & (Factions.DEMON | Factions.UNDEAD)) != Factions.NONE || this.IsFearless)
      return;
    this.mFearTimer = 5f;
    this.mFearedBy = iFearedBy;
    if (EffectManager.Instance.IsActive(ref this.mFearEffect))
      return;
    Vector3 position = this.Position;
    Vector3 direction = this.Direction;
    EffectManager.Instance.StartEffect(Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Fear.FEARED_EFFECT, ref position, ref direction, out this.mFearEffect);
  }

  public void RemoveFear()
  {
    this.mFearTimer = 0.0f;
    this.mFearedBy = (Character) null;
    EffectManager.Instance.Stop(ref this.mFearEffect);
  }

  public bool IsFeared => (double) this.mFearTimer > 0.0 && !this.Dead;

  public BloodType BloodType => this.mBlood;

  public abstract bool IsInAEvent { get; }

  private void PostCollision(ref CollisionInfo iInfo)
  {
    if (!(this is Avatar) && !this.mPlayState.IsInCutscene || !this.IsInAEvent || iInfo.SkinInfo.Skin0.Tag is LevelModel | iInfo.SkinInfo.Skin1.Tag is LevelModel | iInfo.SkinInfo.Skin0.Tag is Water | iInfo.SkinInfo.Skin1.Tag is Water)
      return;
    if (iInfo.SkinInfo.Skin0 == this.mCollision)
      iInfo.SkinInfo.IgnoreSkin0 = true;
    else
      iInfo.SkinInfo.IgnoreSkin1 = true;
  }

  protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner != null)
    {
      if (this.Dead || iSkin1.Owner.Tag == this.mGripper || iSkin1.Owner.Tag == this.mGrippedCharacter)
        return false;
      Character tag1 = iSkin1.Owner.Tag as Character;
      if (this.CurrentSelfShieldType == Character.SelfShieldType.Ice || this.CurrentSelfShieldType == Character.SelfShieldType.IcedEarth)
      {
        if (tag1 != null && !this.mHitList.ContainsKey(tag1.Handle))
        {
          Vector3 velocity1 = tag1.Body.Velocity;
          Vector3 velocity2 = this.Body.Velocity;
          float num1 = velocity1.LengthSquared();
          float num2 = velocity2.LengthSquared();
          if ((double) num1 >= 100.0 || (double) num2 >= 100.0)
          {
            DamageCollection5 oDamages;
            this.mSelfShield.mSpell.CalculateDamage(SpellType.Projectile, CastType.Force, out oDamages);
            int num3 = (int) tag1.Damage(oDamages, (Entity) this, 0.0, new Vector3());
            this.mHitList.Add((Entity) tag1);
          }
        }
      }
      else if (this.CurrentSelfShieldType == Character.SelfShieldType.Shield)
      {
        Shield tag2 = iSkin1.Owner.Tag as Shield;
        if (tag1 != null && tag1.CurrentSelfShieldType == Character.SelfShieldType.Shield)
        {
          this.RemoveSelfShield(Character.SelfShieldType.Shield);
          tag1.RemoveSelfShield(Character.SelfShieldType.Shield);
          Vector3 result1 = this.Position;
          Vector3 result2 = tag1.Position;
          Vector3.Subtract(ref result2, ref result1, out result2);
          result2.Normalize();
          Vector3.Multiply(ref result2, this.Radius, out result2);
          Vector3.Add(ref result1, ref result2, out result1);
          Vector3 right = Vector3.Right;
          EffectManager.Instance.StartEffect(Nullify.EFFECT, ref result1, ref right, out VisualEffectReference _);
          AudioManager.Instance.PlayCue(Banks.Spells, Invisibility.SOUNDHASH, this.AudioEmitter);
        }
        else if (tag2 != null)
        {
          this.RemoveSelfShield(Character.SelfShieldType.Shield);
          tag2.Kill(0.25f);
          Vector3 result3 = this.Position;
          Vector3 result4 = tag2.Position;
          if (tag2.ShieldType == ShieldType.WALL)
          {
            Vector3 result5 = tag2.Body.Orientation.Forward;
            Vector3.Multiply(ref result5, tag2.Radius, out result5);
            Vector3.Subtract(ref result4, ref result5, out result4);
            Vector3 result6;
            Vector3.Subtract(ref result3, ref result4, out result6);
            result5 = tag2.Body.Orientation.Forward;
            Vector3.Multiply(ref result5, 2f * tag2.Radius, out result5);
            result6.Y = result5.Y = 0.0f;
            float result7;
            Vector3.Dot(ref result6, ref result5, out result7);
            result7 /= result5.Length();
            result5 = tag2.Body.Orientation.Forward;
            Vector3.Multiply(ref result5, result7, out result5);
            Vector3.Add(ref result4, ref result5, out result3);
            result3.Y = this.Position.Y;
          }
          else
          {
            Vector3.Subtract(ref result3, ref result4, out result3);
            result3.Normalize();
            Vector3.Multiply(ref result3, tag2.Radius, out result3);
            Vector3.Add(ref result3, ref result4, out result3);
          }
          Vector3 right = Vector3.Right;
          EffectManager.Instance.StartEffect(Nullify.EFFECT, ref result3, ref right, out VisualEffectReference _);
          AudioManager.Instance.PlayCue(Banks.Spells, Invisibility.SOUNDHASH, this.AudioEmitter);
          return false;
        }
      }
    }
    if (iSkin1.Tag is MagickCamera)
      this.mCollidedWithCamera = true;
    if (iSkin1.Tag is LevelModel | iSkin1.Tag is Liquid)
    {
      if (this.mCollisionDamages.A.AttackProperty != (AttackProperties) 0)
      {
        int num = (int) this.Damage(this.mCollisionDamages, (Entity) null, this.PlayState.PlayTime, this.Position);
        this.mCollisionDamages = new DamageCollection5();
      }
      return true;
    }
    return !this.mEthereal && (double) this.mCollisionIgnoreTime <= 0.0;
  }

  public virtual MissileEntity GetMissileInstance() => MissileEntity.GetInstance(this.mPlayState);

  public override Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return (double) this.mImmortalTime > 0.0 | this.mEthereal || this.mSelfShield.Solid || (double) this.mBody.Mass > 1000.0 || this.IsEntangled | this.Dead ? new Vector3() : base.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
  }

  protected override void AddImpulseVelocity(ref Vector3 iVelocity)
  {
    this.CharacterBody.AddImpulseVelocity(ref iVelocity);
  }

  public void Jump(Vector3 iDelta, float iElevation)
  {
    if (this.IsEntangled || this.Dead)
      return;
    if (!this.CharacterBody.IsJumping && NetworkManager.Instance.State == NetworkState.Server)
    {
      CharacterActionMessage iMessage = new CharacterActionMessage()
      {
        Handle = this.Handle,
        Action = ActionType.Jump,
        Param0F = iDelta.X,
        Param1F = iDelta.Y
      };
      iMessage.Param1F = iDelta.Z;
      iMessage.TimeStamp = (double) iElevation;
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref iMessage);
    }
    float y = iDelta.Y;
    iDelta.Y = 0.0f;
    float num1 = iDelta.Length();
    Vector3.Divide(ref iDelta, num1, out iDelta);
    float num2 = iDelta.Y = (float) Math.Sin((double) iElevation);
    float num3 = (float) Math.Cos((double) iElevation);
    iDelta.X *= num3;
    iDelta.Z *= num3;
    float num4 = (float) Math.Sqrt((double) PhysicsManager.Instance.Simulator.Gravity.Y * -1.0 * (double) num1 * (double) num1 / (2.0 * ((double) num1 * (double) num2 / (double) num3 - (double) y) * (double) num3 * (double) num3));
    if (float.IsNaN(num4) || float.IsInfinity(num4))
      return;
    Vector3.Multiply(ref iDelta, num4, out iDelta);
    this.CharacterBody.AddJump(iDelta);
  }

  public Capsule Capsule => (Capsule) this.mCollision.GetPrimitiveLocal(0);

  internal CharacterBody CharacterBody => (CharacterBody) this.mBody;

  public override Vector3 Direction => this.mBody.Orientation.Forward;

  public virtual float Volume => this.mVolume;

  public float TurnSpeed
  {
    get => this.mTurnSpeed;
    set => this.mTurnSpeed = value;
  }

  public int ModelIndex
  {
    get
    {
      for (int modelIndex = 0; modelIndex < this.mTemplate.Models.Length; ++modelIndex)
      {
        if (this.mTemplate.Models[modelIndex].Model == this.mModel)
          return modelIndex;
      }
      return -1;
    }
  }

  public SkinnedModel Model => this.mModel;

  public AnimationController AnimationController => this.mAnimationController;

  public Magicka.Animations CurrentAnimation => (Magicka.Animations) ((uint) this.mCurrentAnimation % 231U);

  public bool HasAnimation(Magicka.Animations iAnimation)
  {
    for (int index = 0; index < this.mEquipment.Length; ++index)
    {
      Item obj = this.mEquipment[index].Item;
      if (obj != null && obj.WeaponClass != WeaponClass.Staff && this.mAnimationClips[(int) obj.WeaponClass] != null && this.mAnimationClips[(int) obj.WeaponClass][(int) iAnimation] != null)
        return true;
    }
    return this.mAnimationClips[0][(int) iAnimation] != null;
  }

  protected void OnAnimationLooped()
  {
    this.mExecutedActions.Clear();
    this.mDeadActions.Clear();
    for (int index = 0; index < this.mCurrentActions.Length; ++index)
    {
      this.mExecutedActions.Add(false);
      this.mDeadActions.Add(false);
    }
  }

  protected void OnCrossfadeFinished()
  {
    AnimationClipAction animationClipAction = this.mAnimationClips[(int) this.mCurrentAnimationSet][(int) this.mCurrentAnimation];
    if (this.mCurrentActions != animationClipAction.Actions)
    {
      for (int index = 0; index < this.mCurrentActions.Length; ++index)
      {
        if (!this.mDeadActions[index])
          this.mCurrentActions[index].Kill(this);
      }
    }
    this.mCurrentActions = animationClipAction.Actions;
    this.mExecutedActions.Clear();
    this.mDeadActions.Clear();
    this.mForceAnimationUpdate = false;
    for (int index = 0; index < this.mCurrentActions.Length; ++index)
    {
      this.mExecutedActions.Add(false);
      this.mDeadActions.Add(false);
      if (this.mCurrentActions[index].UsesBones)
        this.mForceAnimationUpdate = true;
    }
  }

  public void GoToAnimation(Magicka.Animations iAnimation, float iTime)
  {
    if (!(this.mCurrentAnimation != iAnimation | this.mAnimationController.HasFinished))
      return;
    this.mCurrentAnimation = iAnimation;
    this.mCurrentAnimationSet = WeaponClass.Default;
    AnimationClipAction animationClipAction1 = this.mAnimationClips[0][(int) iAnimation];
    for (int index = 0; index < this.mEquipment.Length; ++index)
    {
      Item obj = this.mEquipment[index].Item;
      if (obj != null && obj.WeaponClass != WeaponClass.Staff && this.mAnimationClips[(int) obj.WeaponClass] != null && this.mAnimationClips[(int) obj.WeaponClass][(int) iAnimation] != null)
      {
        this.mCurrentAnimationSet = obj.WeaponClass;
        animationClipAction1 = this.mAnimationClips[(int) this.mCurrentAnimationSet][(int) iAnimation];
        break;
      }
    }
    if (animationClipAction1 == null)
    {
      this.mCurrentAnimation = Magicka.Animations.idle;
      this.mCurrentAnimationSet = WeaponClass.Default;
      AnimationClipAction animationClipAction2 = this.mAnimationClips[(int) this.mCurrentAnimationSet][(int) this.mCurrentAnimation];
      this.mAnimationController.ClipSpeed = animationClipAction2.AnimationSpeed;
      float blendTime = animationClipAction2.BlendTime;
      this.mAnimationController.CrossFade(animationClipAction2.Clip, (double) blendTime > 0.0 ? blendTime : iTime, false);
    }
    else
    {
      this.mAnimationController.ClipSpeed = animationClipAction1.AnimationSpeed;
      float blendTime = animationClipAction1.BlendTime;
      this.mAnimationController.CrossFade(animationClipAction1.Clip, (double) blendTime > 0.0 ? blendTime : iTime, animationClipAction1.LoopAnimation);
    }
  }

  public void ForceAnimation(Magicka.Animations iAnimation)
  {
    this.mAnimationController.ClearCrossfadeQueue();
    this.mCurrentAnimation = iAnimation;
    this.mCurrentAnimationSet = WeaponClass.Default;
    AnimationClipAction animationClipAction = this.mAnimationClips[0][(int) iAnimation];
    for (int index = 0; index < this.mEquipment.Length; ++index)
    {
      Item obj = this.mEquipment[index].Item;
      if (obj != null && obj.WeaponClass != WeaponClass.Staff && this.mAnimationClips[(int) obj.WeaponClass] != null && this.mAnimationClips[(int) obj.WeaponClass][(int) iAnimation] != null)
      {
        this.mCurrentAnimationSet = obj.WeaponClass;
        animationClipAction = this.mAnimationClips[(int) this.mCurrentAnimationSet][(int) iAnimation];
        break;
      }
    }
    if (animationClipAction == null)
    {
      this.mCurrentAnimation = Magicka.Animations.idle;
      this.mCurrentAnimationSet = WeaponClass.Default;
    }
    this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
    this.mAnimationController.StartClip(animationClipAction.Clip, animationClipAction.LoopAnimation);
    this.OnCrossfadeFinished();
  }

  internal virtual bool Polymorphed
  {
    get => false;
    set
    {
    }
  }

  public override Matrix GetOrientation()
  {
    Matrix result = this.mBody.Orientation with
    {
      Translation = this.mBody.Position
    };
    result.M42 += this.mHeightOffset;
    Matrix.Multiply(ref this.mStaticTransform, ref result, out result);
    return result;
  }

  public Matrix GetHipAttachOrientation()
  {
    if (this.mHipJoint.mIndex < 0)
      return Matrix.Identity;
    Matrix result;
    Matrix.Multiply(ref this.mHipJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mHipJoint.mIndex], out result);
    return result;
  }

  public virtual Matrix GetRightAttachOrientation()
  {
    if (this.mRightHandJoint.mIndex < 0)
      return Matrix.Identity;
    Matrix result;
    Matrix.Multiply(ref this.mRightHandJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightHandJoint.mIndex], out result);
    return result;
  }

  public virtual Matrix GetLeftAttachOrientation()
  {
    if (this.mLeftHandJoint.mIndex < 0)
      return Matrix.Identity;
    Matrix result;
    Matrix.Multiply(ref this.mLeftHandJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftHandJoint.mIndex], out result);
    return result;
  }

  public virtual Matrix GetMouthAttachOrientation()
  {
    if (this.mMouthJoint.mIndex < 0)
      return Matrix.Identity;
    Matrix result;
    Matrix.Multiply(ref this.mMouthJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthJoint.mIndex], out result);
    return result;
  }

  public virtual Matrix GetBoneOrientation(int iBoneIndex, ref Matrix iBoneBindPose)
  {
    Matrix result;
    Matrix.Multiply(ref iBoneBindPose, ref this.mAnimationController.SkinnedBoneTransforms[iBoneIndex], out result);
    return result;
  }

  public virtual Matrix GetItemAttachOrientation(int iIndex)
  {
    Attachment attachment = this.mEquipment[iIndex];
    if (attachment.AttachIndex < 0)
      return Matrix.Identity;
    Matrix result = attachment.BindBose;
    Matrix.Multiply(ref result, ref this.mAnimationController.SkinnedBoneTransforms[attachment.AttachIndex], out result);
    return result;
  }

  public void KnockDown(float iMagnitude)
  {
    if ((double) iMagnitude < (double) this.mKnockdownTolerance)
      return;
    this.mKnockedDown = true;
    this.mBody.Velocity = this.mBody.Velocity with
    {
      X = 0.0f,
      Z = 0.0f
    };
  }

  public void KnockDown()
  {
    this.mKnockedDown = true;
    this.mBody.Velocity = this.mBody.Velocity with
    {
      X = 0.0f,
      Z = 0.0f
    };
    this.mGripDamageAccumulation = 0.0f;
  }

  public virtual bool IsAlerted
  {
    get => this.mAlert;
    set => this.mAlert = value;
  }

  public virtual bool IsBlocking
  {
    get => this.mBlock;
    set
    {
      if (this.mBlock != value & NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = this.Handle,
          Action = ActionType.Block,
          Param0I = value ? 1 : 0
        });
      this.mBlock = value;
    }
  }

  public int BlockItem { get; set; }

  public bool Chanting => this.mSpellQueue.Count > 0;

  public virtual void Die()
  {
    if (this.mDead)
      return;
    if (this.mUndying & !this.mDrowning)
    {
      this.mUndieTimer = this.mTemplate.UndieTime;
      if ((double) this.mUndieTimer < 1.4012984643248171E-45)
        this.mUndieTimer = 0.5f;
    }
    else
    {
      this.mUndieTimer = float.NaN;
      Dictionary<int, AnimatedLevelPart> animatedLevelParts = this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts;
      bool flag = false;
      foreach (AnimatedLevelPart animatedLevelPart in animatedLevelParts.Values)
      {
        if (animatedLevelPart.IsTouchingEntity(this.Handle, true))
        {
          flag = true;
          break;
        }
      }
      if (!flag && this.CharacterBody.IsTouchingGround)
        this.mBody.DisableBody();
    }
    this.StopLevitate();
    this.mHitList.Clear();
    if (!(this.mOverkilled | this.mDrowning))
      this.mEventConditions.ExecuteAll((Entity) this, (Entity) null, ref new EventCondition()
      {
        EventConditionType = EventConditionType.Death
      });
    if (this.mOnDeathTrigger != 0)
      this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDeathTrigger, this, false);
    this.StopHypnotize();
    this.mDead = true;
    for (int index = 0; index < this.mAttachedEffectsBoneIndex.Length; ++index)
      this.mAttachedEffectsBoneIndex[index] = -1;
    if (this.mSpellLight != null)
      this.mSpellLight.Stop(false);
    if (this.mStatusEffectLight != null)
    {
      this.mStatusEffectLight.Stop(false);
      this.mStatusEffectLight = (DynamicLight) null;
    }
    if (this.mCurrentSpell != null)
    {
      this.mCurrentSpell.DeInitialize((ISpellCaster) this);
      this.mCurrentSpell = (SpellEffect) null;
    }
    for (int index = 0; index < this.mAuras.Count; ++index)
    {
      VisualEffectReference mEffect = this.mAuras[index].mEffect;
      EffectManager.Instance.Stop(ref mEffect);
    }
    this.mAuras.Clear();
    this.mBuffs.Clear();
    for (int index = 0; index < this.mBuffEffects.Count; ++index)
    {
      VisualEffectReference mBuffEffect = this.mBuffEffects[index];
      EffectManager.Instance.Stop(ref mBuffEffect);
    }
    this.mBuffEffects.Clear();
    this.mBuffDecals.Clear();
    EffectManager.Instance.Stop(ref this.mStunEffect);
    EffectManager.Instance.Stop(ref this.mFearEffect);
    EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
    this.EndCharm();
    this.StopHypnotize();
    this.mDeathStatusEffects = this.mCurrentStatusEffects;
    this.mCurrentStatusEffects = StatusEffects.None;
    for (int index = 0; index < 9; ++index)
      this.mStatusEffects[index].Stop();
    for (int index = 0; index < this.mStatusEffectCues.Length; ++index)
    {
      if (this.mStatusEffectCues[index] != null)
        this.mStatusEffectCues[index].Stop(AudioStopOptions.AsAuthored);
    }
    for (int index = 0; index < this.mAttachedEffects.Length; ++index)
    {
      if (this.mAttachedEffects[index].Hash != 0)
        EffectManager.Instance.Stop(ref this.mAttachedEffects[index]);
    }
    if (this.ChargeCue != null)
      this.ChargeCue.Stop(AudioStopOptions.AsAuthored);
    this.mSpellQueue.Clear();
    this.mEntaglement.Release();
    if (this.mDialog != 0 && DialogManager.Instance.DialogActive(this.mDialog))
    {
      DialogManager.Instance.End(this.mDialog);
      DialogManager.Instance.Dialogs.DialogFinished(this.mDialog);
    }
    this.RemoveSelfShield();
    for (int index = 0; index < this.mAttachedSoundCues.Length; ++index)
    {
      if (this.mAttachedSoundCues[index] != null && !this.mAttachedSoundCues[index].IsStopping)
        this.mAttachedSoundCues[index].Stop(AudioStopOptions.AsAuthored);
    }
  }

  public void UpdateDeath(float iDeltaTime)
  {
    if (!this.mDead)
      return;
    this.mTimeDead += iDeltaTime;
    this.mEventConditions.ExecuteAll((Entity) this, (Entity) null, ref new EventCondition()
    {
      EventConditionType = EventConditionType.Death,
      Time = this.mTimeDead
    });
  }

  public void Attack(Magicka.Animations iAnimation, bool iAllowRotate)
  {
    if (NetworkManager.Instance.State != NetworkState.Offline && (!this.mAttacking || iAnimation != this.mNextAttackAnimation))
    {
      CharacterActionMessage iMessage = new CharacterActionMessage();
      iMessage.Handle = this.Handle;
      iMessage.Action = ActionType.Attack;
      iMessage.Param0I = (int) iAnimation;
      iMessage.Param1I = (int) this.mCastType;
      iMessage.Param2F = this.mSpellPower;
      if (iAllowRotate)
        iMessage.Param0I |= int.MinValue;
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref iMessage);
    }
    this.mNextAttackAnimation = iAnimation;
    this.mAllowAttackRotate = iAllowRotate;
    this.mAttacking = true;
  }

  public void SpecialAbilityAnimation(Magicka.Animations iAnimation)
  {
    this.mNextAttackAnimation = iAnimation;
    this.mAllowAttackRotate = false;
    this.mAttacking = true;
  }

  public void Dash(Magicka.Animations iAnimation, bool iAllowRotate)
  {
    if (NetworkManager.Instance.State != NetworkState.Offline && (!this.mDashing || iAnimation != this.mNextAttackAnimation))
    {
      CharacterActionMessage iMessage = new CharacterActionMessage();
      iMessage.Handle = this.Handle;
      iMessage.Action = ActionType.Dash;
      iMessage.Param0I = (int) iAnimation;
      if (iAllowRotate)
        iMessage.Param0I |= int.MinValue;
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref iMessage);
    }
    this.mNextAttackAnimation = iAnimation;
    this.mAllowAttackRotate = iAllowRotate;
    this.mDashing = true;
  }

  public void DamageGripped(Magicka.Animations iAnimation)
  {
    if (this.mGripAttack && this.mNextGripAttackAnimation != Magicka.Animations.None && !(this.mCurrentState is GripAttackState))
      return;
    if (NetworkManager.Instance.State != NetworkState.Offline && (!this.mGripAttack || iAnimation != this.mNextAttackAnimation))
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Handle = this.Handle,
        Action = ActionType.GripAttack,
        Param0I = (int) iAnimation
      });
    this.mNextGripAttackAnimation = iAnimation;
    this.mAllowAttackRotate = false;
    this.mGripAttack = true;
  }

  protected bool BlockDamage(Magicka.GameLogic.Damage iDamage, Vector3 iPosition, out float oBlocked)
  {
    float num = 0.0f;
    oBlocked = 0.0f;
    for (int index = 0; index < this.mEquipment.Length; ++index)
      num += (float) this.mEquipment[index].Item.BlockValue;
    Vector3 forward = this.Body.Orientation.Forward;
    Vector3 position = this.Body.Position;
    Vector3 result;
    Vector3.Subtract(ref iPosition, ref position, out result);
    result.Y = 0.0f;
    result.Normalize();
    forward.Y = 0.0f;
    forward.Normalize();
    if ((double) MagickaMath.Angle(ref forward, ref result) > 1.0995573997497559)
      return false;
    oBlocked = num;
    return true;
  }

  public void StopStatusEffects(StatusEffects iEffects)
  {
    for (int index = 0; index < 9; ++index)
    {
      if ((iEffects & this.mStatusEffects[index].DamageType) != StatusEffects.None)
      {
        this.mStatusEffects[index].Stop();
        if (index < this.mStatusEffectCues.Length && this.mStatusEffectCues[index] != null)
          this.mStatusEffectCues[index].Stop(AudioStopOptions.AsAuthored);
      }
    }
  }

  public virtual Spell SpellToCast => SpellManager.Instance.Combine(this.SpellQueue);

  public virtual void CombineSpell()
  {
    this.mSpell = SpellManager.Instance.Combine(this.mSpellQueue);
    this.SpellQueue.Clear();
  }

  public virtual void CastSpell(bool iFromStaff, string iJoint)
  {
    if (this.mCurrentSpell != null)
      return;
    if (this.mSpell.Element == Elements.All)
    {
      if (this.mSpecialAbility == null)
      {
        this.mSpecialAbility = new SpellMagickConverter()
        {
          Spell = this.mSpell
        }.Magick.Effect;
        this.CastType = CastType.None;
      }
      if (this.mSpecialAbility.Execute((ISpellCaster) this, this.mPlayState) && this is Avatar && (this as Avatar).Player != null && !((this as Avatar).Player.Gamer is NetworkGamer))
      {
        this.PlayState.HasUsedMagick[(this as Avatar).Player.ID] = true;
        AchievementsManager.Instance.AwardAchievement(this.PlayState, "cookingbythebook");
      }
      this.mSpell = new Spell();
      this.mSpecialAbility = (Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility) null;
    }
    else
    {
      if (this is Avatar && (this as Avatar).Player != null && !((this as Avatar).Player.Gamer is NetworkGamer))
        Profile.Instance.UsedElements(this.PlayState, (this as Avatar).Player.GamerTag, this.mSpell.Element);
      this.mSpell.Cast(iFromStaff, (ISpellCaster) this, this.mCastType);
      TelemetryUtils.SendSpellCast(this);
      if (this.IsEntangled && this.mCastType == CastType.Self)
        this.mEntaglement.DecreaseEntanglement(0.0f, this.mSpell.Element);
      this.mSpell = new Spell();
    }
  }

  public Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility Magick
  {
    get => this.mSpecialAbility;
    set => this.mSpecialAbility = value;
  }

  public DynamicLight SpellLight
  {
    get => this.mSpellLight;
    set => this.mSpellLight = value;
  }

  public int SourceOfSpell => this.mSourceOfSpellIndex;

  public StaticList<Spell> SpellQueue => this.mSpellQueue;

  public SpellEffect CurrentSpell
  {
    get => this.mCurrentSpell;
    set
    {
      this.mCurrentSpell = value;
      if (value != null)
        return;
      this.mCastType = CastType.None;
      this.mTurnSpeed = this.mTurnSpeedMax;
    }
  }

  public Spell Spell => this.mSpell;

  public virtual CastType CastType
  {
    get => this.mCastType;
    set
    {
      if (value != CastType.Weapon & value != CastType.None & value != this.mCastType)
        this.CombineSpell();
      this.mCastType = value;
    }
  }

  public abstract int Boosts { get; set; }

  public abstract float BoostCooldown { get; }

  public Matrix CastSource => this.mStaffOrb;

  public Matrix WeaponSource => this.mWeaponTransform;

  public void StopAllActions()
  {
    this.ReleaseAttachedCharacter();
    if (this.mCurrentSpell == null)
      return;
    this.mCurrentSpell.Stop((ISpellCaster) this);
  }

  public void ResetSpell() => this.mSpell = new Spell();

  public bool NotedKilledEvent
  {
    get => this.mNotedKilledEvent;
    set => this.mNotedKilledEvent = true;
  }

  public virtual void Terminate(bool iKillItems, bool iIsKillPlane, bool iNetwork)
  {
    this.mDeadTimer = -100f;
    this.mDead = true;
    this.mHitPoints = 0.0f;
    if (this.mSpellLight != null)
      this.mSpellLight.Stop(false);
    if (this.mStatusEffectLight != null)
    {
      this.mStatusEffectLight.Stop(false);
      this.mStatusEffectLight = (DynamicLight) null;
    }
    if (this.mCurrentSpell != null)
    {
      this.mCurrentSpell.DeInitialize((ISpellCaster) this);
      this.mCurrentSpell = (SpellEffect) null;
    }
    for (int index = 0; index < this.mAuras.Count; ++index)
    {
      VisualEffectReference mEffect = this.mAuras[index].mEffect;
      EffectManager.Instance.Stop(ref mEffect);
    }
    this.mAuras.Clear();
    this.mBuffs.Clear();
    for (int index = 0; index < this.mBuffEffects.Count; ++index)
    {
      VisualEffectReference mBuffEffect = this.mBuffEffects[index];
      EffectManager.Instance.Stop(ref mBuffEffect);
    }
    this.mBuffEffects.Clear();
    this.mBuffDecals.Clear();
    EffectManager.Instance.Stop(ref this.mStunEffect);
    EffectManager.Instance.Stop(ref this.mFearEffect);
    EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
    this.EndCharm();
    this.StopHypnotize();
    this.mDeathStatusEffects = this.mCurrentStatusEffects;
    this.mCurrentStatusEffects = StatusEffects.None;
    for (int index = 0; index < 9; ++index)
      this.mStatusEffects[index].Stop();
    if (iKillItems)
    {
      for (int index = 0; index < this.mEquipment.Length; ++index)
        this.mEquipment[index].Item.Kill();
    }
    if (!iIsKillPlane || !(this.mLastAttacker is Avatar) || (this.mLastAttacker as Avatar).Player.Gamer is NetworkGamer || (this.mLastDamage & DamageResult.Pushed) != DamageResult.Pushed)
      return;
    AchievementsManager.Instance.AwardAchievement(this.PlayState, "wingardiumleviosa");
  }

  public void Terminate(bool iKillItems, bool iNetwork)
  {
    this.Terminate(iKillItems, false, iNetwork);
  }

  public virtual Vector3 Color
  {
    get => this.mMaterial.TintColor;
    set => this.mMaterial.TintColor = value;
  }

  public float HeightOffset
  {
    get => this.mHeightOffset;
    set => this.mHeightOffset = value;
  }

  public Vector3 GetRandomPositionOnCollisionSkin()
  {
    Vector3 position = this.Position;
    if (this.mCollision == null)
      return position;
    Vector3 positionOnCollisionSkin = new Vector3();
    float iAngle1 = MagickaMath.SetBetween(-3.14159274f, 3.14159274f, (float) Character.sRandom.NextDouble());
    float iAngle2 = MagickaMath.SetBetween(-1.57079637f, 1.57079637f, (float) Character.sRandom.NextDouble());
    float oSin1;
    float oCos1;
    MathApproximation.FastSinCos(iAngle1, out oSin1, out oCos1);
    float oSin2;
    float oCos2;
    MathApproximation.FastSinCos(iAngle2, out oSin2, out oCos2);
    float num1 = oCos1 * oCos2;
    float num2 = oSin1 * oCos2;
    float radius = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius;
    float num3 = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius * 2f + (this.mCollision.GetPrimitiveLocal(0) as Capsule).Length;
    positionOnCollisionSkin.X = num1 * radius;
    positionOnCollisionSkin.Z = num2 * radius;
    positionOnCollisionSkin.Y = (float) ((double) oSin2 * (double) num3 * 0.5);
    positionOnCollisionSkin.X += position.X;
    positionOnCollisionSkin.Y += position.Y;
    positionOnCollisionSkin.Z += position.Z;
    return positionOnCollisionSkin;
  }

  public DamageResult AddStatusEffect(StatusEffect iStatusEffect)
  {
    if (this.IsImmortal | this.mEthereal | !this.mCanHasStatusEffect)
      return DamageResult.None;
    DamageResult damageResult = DamageResult.None;
    if (!iStatusEffect.Dead)
    {
      bool flag1 = false;
      bool flag2 = false;
      switch (iStatusEffect.DamageType)
      {
        case StatusEffects.Burning:
          if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Dead)
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Stop();
            flag1 = true;
          }
          if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Dead)
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
            if ((double) this.mDryTimer < 0.0 && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)] != null && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)].IsPlaying)
            {
              this.mDryTimer = 0.9f;
              Vector3 position = this.Position;
              Vector3 direction = this.Direction;
              EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position, ref direction, out this.mDryingEffect);
              this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop(AudioStopOptions.AsAuthored);
              AudioManager.Instance.PlayCue(Banks.Spells, Defines.STEAM_CUE, this.AudioEmitter);
            }
            flag1 = true;
          }
          else if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Dead)
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude -= iStatusEffect.Magnitude;
            Vector3 position = this.Position;
            Vector3 direction = this.Direction;
            EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position, ref direction, out this.mDryingEffect);
            if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude <= 0.0 && (double) this.mDryTimer < 0.0 && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Frozen)] != null && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Frozen)].IsPlaying)
            {
              this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Frozen)].Stop(AudioStopOptions.AsAuthored);
              this.mDryTimer = 0.9f;
              EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position, ref direction, out this.mDryingEffect);
              if (this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)] != null)
                this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop(AudioStopOptions.AsAuthored);
              AudioManager.Instance.PlayCue(Banks.Spells, Defines.STEAM_CUE, this.AudioEmitter);
            }
            flag1 = true;
          }
          if (this.HasStatus(StatusEffects.Greased))
          {
            iStatusEffect.DPS *= 4f;
            int index = StatusEffect.StatusIndex(StatusEffects.Greased);
            this.mStatusEffects[index].Stop();
            this.mStatusEffects[index] = new StatusEffect();
          }
          Elements iElement1 = Elements.Fire;
          int index1 = Spell.ElementIndex(iElement1);
          float multiplier1 = this.mResistances[index1].Multiplier;
          float modifier1 = this.mResistances[index1].Modifier;
          if (this is Avatar)
          {
            multiplier1 *= this.Equipment[1].Item.Resistance[index1].Multiplier;
            modifier1 += this.Equipment[1].Item.Resistance[index1].Modifier;
          }
          for (int index2 = 0; index2 < this.mBuffs.Count; ++index2)
          {
            BuffStorage mBuff = this.mBuffs[index2];
            if (mBuff.BuffType == BuffType.Resistance && (mBuff.BuffResistance.Resistance.ResistanceAgainst & iElement1) == iElement1)
            {
              multiplier1 *= mBuff.BuffResistance.Resistance.Multiplier;
              modifier1 += mBuff.BuffResistance.Resistance.Modifier;
            }
          }
          iStatusEffect.DPS = (iStatusEffect.DPS + modifier1) * multiplier1;
          if ((double) Math.Abs(iStatusEffect.DPS) < 1.4012984643248171E-45 || (double) Math.Abs(iStatusEffect.Magnitude) < 1.4012984643248171E-45)
            flag2 = true;
          if (this.mResistances[index1].StatusResistance)
          {
            flag2 = true;
            break;
          }
          break;
        case StatusEffects.Wet:
          if (this.HasStatus(StatusEffects.Burning) | (double) this.mDryTimer > 0.0)
          {
            int index3 = StatusEffect.StatusIndex(StatusEffects.Burning);
            if (this.mStatusEffectCues[index3] != null && this.mStatusEffectCues[index3].IsPlaying)
            {
              this.mStatusEffectCues[index3].Stop(AudioStopOptions.AsAuthored);
              this.mDryTimer = 0.9f;
              Vector3 position = this.Position;
              Vector3 direction = this.Direction;
              EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position, ref direction, out this.mDryingEffect);
              AudioManager.Instance.PlayCue(Banks.Spells, Defines.STEAM_CUE, this.AudioEmitter);
            }
            this.mStatusEffects[index3].Stop();
            this.mStatusEffects[index3] = new StatusEffect();
            flag1 = true;
          }
          if (this.HasStatus(StatusEffects.Greased))
          {
            int index4 = StatusEffect.StatusIndex(StatusEffects.Greased);
            this.mStatusEffects[index4].Stop();
            this.mStatusEffects[index4] = new StatusEffect();
          }
          Elements iElement2 = Elements.Water;
          int index5 = Spell.ElementIndex(iElement2);
          float multiplier2 = this.mResistances[index5].Multiplier;
          float modifier2 = this.mResistances[index5].Modifier;
          if (this is Avatar)
          {
            multiplier2 *= this.Equipment[1].Item.Resistance[index5].Multiplier;
            modifier2 += this.Equipment[1].Item.Resistance[index5].Modifier;
          }
          for (int index6 = 0; index6 < this.mBuffs.Count; ++index6)
          {
            BuffStorage mBuff = this.mBuffs[index6];
            if (mBuff.BuffType == BuffType.Resistance && (mBuff.BuffResistance.Resistance.ResistanceAgainst & iElement2) == iElement2)
            {
              multiplier2 *= mBuff.BuffResistance.Resistance.Multiplier;
              modifier2 += mBuff.BuffResistance.Resistance.Modifier;
            }
          }
          iStatusEffect.DPS += modifier2;
          iStatusEffect.Magnitude *= multiplier2;
          if ((double) Math.Abs(iStatusEffect.Magnitude) < 1.4012984643248171E-45)
            flag2 = true;
          if (this.mResistances[index5].StatusResistance)
          {
            flag2 = true;
            break;
          }
          break;
        case StatusEffects.Cold:
          float magnitude = iStatusEffect.Magnitude;
          if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead || (double) this.mDryTimer > 0.0)
          {
            if (this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Burning)] != null && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Burning)].IsPlaying)
            {
              this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
              this.mDryTimer = 0.9f;
              Vector3 position = this.Position;
              Vector3 direction = this.Direction;
              EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position, ref direction, out this.mDryingEffect);
              this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop(AudioStopOptions.AsAuthored);
              AudioManager.Instance.PlayCue(Banks.Spells, Defines.STEAM_CUE, this.AudioEmitter);
            }
            flag1 = true;
          }
          float iMagnitude = 1f;
          int index7 = Spell.ElementIndex(Elements.Ice);
          if (this.HasStatus(StatusEffects.Wet) && !this.mResistances[index7].StatusResistance)
          {
            if (this.mCurrentSpell != null)
              this.mCurrentSpell.Stop((ISpellCaster) this);
            int index8 = StatusEffect.StatusIndex(StatusEffects.Frozen);
            this.mStatusEffects[index8] = new StatusEffect(StatusEffects.Frozen, 0.0f, iMagnitude, this.Capsule.Length, this.Capsule.Radius);
            this.mCurrentStatusEffects |= StatusEffects.Frozen;
            if (this.mStatusEffectCues[index8] == null || this.mStatusEffectCues[index8] != null && !this.mStatusEffectCues[index8].IsPlaying)
              this.mStatusEffectCues[index8] = AudioManager.Instance.PlayCue(Banks.Spells, "spell_ice_frozen".GetHashCodeCustom(), this.AudioEmitter);
            int index9 = StatusEffect.StatusIndex(StatusEffects.Wet);
            if (this.mStatusEffectCues[index9] != null && this.mStatusEffectCues[index9].IsPlaying)
              this.mStatusEffectCues[index9].Stop(AudioStopOptions.AsAuthored);
            this.mStatusEffects[index9].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Stop();
            flag1 = true;
            break;
          }
          if (this.HasStatus(StatusEffects.Frozen))
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude = iMagnitude;
            flag1 = true;
            break;
          }
          Elements iElement3 = Elements.Cold;
          int index10 = Spell.ElementIndex(iElement3);
          float multiplier3 = this.mResistances[index10].Multiplier;
          float modifier3 = this.mResistances[index10].Modifier;
          if (this is Avatar)
          {
            multiplier3 *= this.Equipment[1].Item.Resistance[index10].Multiplier;
            modifier3 += this.Equipment[1].Item.Resistance[index10].Modifier;
          }
          for (int index11 = 0; index11 < this.mBuffs.Count; ++index11)
          {
            BuffStorage mBuff = this.mBuffs[index11];
            if (mBuff.BuffType == BuffType.Resistance && (mBuff.BuffResistance.Resistance.ResistanceAgainst & iElement3) == iElement3)
            {
              multiplier3 *= mBuff.BuffResistance.Resistance.Multiplier;
              modifier3 += mBuff.BuffResistance.Resistance.Modifier;
            }
          }
          iStatusEffect.DPS += modifier3;
          iStatusEffect.Magnitude *= multiplier3;
          if ((double) Math.Abs(iStatusEffect.Magnitude) < 1.4012984643248171E-45)
            flag2 = true;
          if (this.mResistances[index10].StatusResistance)
          {
            flag2 = true;
            break;
          }
          break;
        case StatusEffects.Poisoned:
          Elements iElement4 = Elements.Poison;
          int index12 = Spell.ElementIndex(iElement4);
          float multiplier4 = this.mResistances[index12].Multiplier;
          float modifier4 = this.mResistances[index12].Modifier;
          if (this is Avatar)
          {
            multiplier4 *= this.Equipment[1].Item.Resistance[index12].Multiplier;
            modifier4 += this.Equipment[1].Item.Resistance[index12].Modifier;
          }
          for (int index13 = 0; index13 < this.mBuffs.Count; ++index13)
          {
            BuffStorage mBuff = this.mBuffs[index13];
            if (mBuff.BuffType == BuffType.Resistance && (mBuff.BuffResistance.Resistance.ResistanceAgainst & iElement4) == iElement4)
            {
              multiplier4 *= mBuff.BuffResistance.Resistance.Multiplier;
              modifier4 += mBuff.BuffResistance.Resistance.Modifier;
            }
          }
          iStatusEffect.DPS += modifier4;
          iStatusEffect.Magnitude *= multiplier4;
          if ((double) Math.Abs(iStatusEffect.DPS) < 1.4012984643248171E-45 || (double) Math.Abs(iStatusEffect.Magnitude) < 1.4012984643248171E-45)
            flag2 = true;
          if (this.mResistances[index12].StatusResistance)
          {
            flag2 = true;
            break;
          }
          break;
        case StatusEffects.Healing:
          if (this.HasStatus(StatusEffects.Poisoned))
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)] = new StatusEffect();
          }
          if (this.HasStatus(StatusEffects.Bleeding))
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Bleeding)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Bleeding)] = new StatusEffect();
          }
          Elements iElement5 = Elements.Life;
          int index14 = Spell.ElementIndex(iElement5);
          float multiplier5 = this.mResistances[index14].Multiplier;
          float modifier5 = this.mResistances[index14].Modifier;
          if (this is Avatar)
          {
            multiplier5 *= this.Equipment[1].Item.Resistance[index14].Multiplier;
            modifier5 += this.Equipment[1].Item.Resistance[index14].Modifier;
          }
          for (int index15 = 0; index15 < this.mBuffs.Count; ++index15)
          {
            BuffStorage mBuff = this.mBuffs[index15];
            if (mBuff.BuffType == BuffType.Resistance && (mBuff.BuffResistance.Resistance.ResistanceAgainst & iElement5) == iElement5)
            {
              multiplier5 *= mBuff.BuffResistance.Resistance.Multiplier;
              modifier5 += mBuff.BuffResistance.Resistance.Modifier;
            }
          }
          iStatusEffect.DPS += modifier5;
          iStatusEffect.Magnitude *= multiplier5;
          if ((double) Math.Abs(iStatusEffect.DPS) < 1.4012984643248171E-45 || (double) Math.Abs(iStatusEffect.Magnitude) < 1.4012984643248171E-45)
            flag2 = true;
          if (this.mResistances[index14].StatusResistance)
          {
            flag2 = true;
            break;
          }
          break;
        case StatusEffects.Greased:
          if (this.HasStatus(StatusEffects.Wet))
          {
            int index16 = StatusEffect.StatusIndex(StatusEffects.Wet);
            this.mStatusEffects[index16].Stop();
            this.mStatusEffects[index16] = new StatusEffect();
          }
          Elements iElement6 = Elements.Water;
          int index17 = Spell.ElementIndex(iElement6);
          float multiplier6 = this.mResistances[index17].Multiplier;
          float modifier6 = this.mResistances[index17].Modifier;
          if (this is Avatar)
          {
            multiplier6 *= this.Equipment[1].Item.Resistance[index17].Multiplier;
            modifier6 += this.Equipment[1].Item.Resistance[index17].Modifier;
          }
          for (int index18 = 0; index18 < this.mBuffs.Count; ++index18)
          {
            BuffStorage mBuff = this.mBuffs[index18];
            if (mBuff.BuffType == BuffType.Resistance && (mBuff.BuffResistance.Resistance.ResistanceAgainst & iElement6) == iElement6)
            {
              multiplier6 *= mBuff.BuffResistance.Resistance.Multiplier;
              modifier6 += mBuff.BuffResistance.Resistance.Modifier;
            }
          }
          iStatusEffect.DPS += modifier6;
          iStatusEffect.Magnitude *= multiplier6;
          if ((double) Math.Abs(iStatusEffect.DPS) < 1.4012984643248171E-45 || (double) Math.Abs(iStatusEffect.Magnitude) < 1.4012984643248171E-45)
          {
            flag2 = true;
            break;
          }
          break;
        case StatusEffects.Steamed:
          if (this.HasStatus(StatusEffects.Cold))
          {
            int index19 = StatusEffect.StatusIndex(StatusEffects.Cold);
            this.mStatusEffects[index19].Stop();
            this.mStatusEffects[index19] = new StatusEffect();
          }
          Elements iElement7 = Elements.Steam;
          int index20 = Spell.ElementIndex(iElement7);
          float multiplier7 = this.mResistances[index20].Multiplier;
          float modifier7 = this.mResistances[index20].Modifier;
          if (this is Avatar)
          {
            multiplier7 *= this.Equipment[1].Item.Resistance[index20].Multiplier;
            modifier7 += this.Equipment[1].Item.Resistance[index20].Modifier;
          }
          for (int index21 = 0; index21 < this.mBuffs.Count; ++index21)
          {
            BuffStorage mBuff = this.mBuffs[index21];
            if (mBuff.BuffType == BuffType.Resistance && (mBuff.BuffResistance.Resistance.ResistanceAgainst & iElement7) == iElement7)
            {
              multiplier7 *= mBuff.BuffResistance.Resistance.Multiplier;
              modifier7 += mBuff.BuffResistance.Resistance.Modifier;
            }
          }
          iStatusEffect.DPS += modifier7;
          iStatusEffect.Magnitude *= multiplier7;
          if ((double) Math.Abs(iStatusEffect.DPS) < 1.4012984643248171E-45 || (double) Math.Abs(iStatusEffect.Magnitude) < 1.4012984643248171E-45)
            flag2 = true;
          if (this.mResistances[index20].StatusResistance)
          {
            flag2 = true;
            break;
          }
          break;
      }
      if (!flag1 && !flag2)
      {
        int index22 = StatusEffect.StatusIndex(iStatusEffect.DamageType);
        if (iStatusEffect.DamageType == StatusEffects.Burning && !this.HasStatus(StatusEffects.Burning))
        {
          int index23 = StatusEffect.StatusIndex(StatusEffects.Burning);
          if (this.mStatusEffectCues[index23] == null || this.mStatusEffectCues[index23] != null && !this.mStatusEffectCues[index23].IsPlaying)
            this.mStatusEffectCues[index23] = AudioManager.Instance.PlayCue(Banks.Spells, "spell_fire_onfire".GetHashCodeCustom(), this.mAudioEmitter);
        }
        if (iStatusEffect.DamageType == StatusEffects.Wet && !this.HasStatus(StatusEffects.Wet))
        {
          int index24 = StatusEffect.StatusIndex(StatusEffects.Wet);
          if (this.mStatusEffectCues[index24] == null || this.mStatusEffectCues[index24] != null && !this.mStatusEffectCues[index24].IsPlaying)
            this.mStatusEffectCues[index24] = AudioManager.Instance.PlayCue(Banks.Spells, "spell_water_drip".GetHashCodeCustom(), this.mAudioEmitter);
        }
        if (iStatusEffect.DamageType == StatusEffects.Frozen && !this.HasStatus(StatusEffects.Frozen))
        {
          int index25 = StatusEffect.StatusIndex(StatusEffects.Frozen);
          if (this.mStatusEffectCues[index25] == null || this.mStatusEffectCues[index25] != null && !this.mStatusEffectCues[index25].IsPlaying)
            this.mStatusEffectCues[index25] = AudioManager.Instance.PlayCue(Banks.Spells, "spell_ice_frozen".GetHashCodeCustom(), this.mAudioEmitter);
        }
        this.mStatusEffects[index22] = this.mStatusEffects[index22] + iStatusEffect;
        damageResult |= DamageResult.Statusadded;
      }
      else if (flag1)
        damageResult |= DamageResult.Statusremoved;
      else if (flag2)
        damageResult = damageResult;
    }
    StatusEffects statusEffects = StatusEffects.None;
    int num = 1;
    int index26 = 0;
    while (num < 9)
    {
      if (!this.mStatusEffects[index26].Dead)
        statusEffects |= (StatusEffects) num;
      num <<= 1;
      ++index26;
    }
    this.mCurrentStatusEffects = statusEffects;
    return damageResult;
  }

  public float BreakFreeStrength => this.mBreakFreeStrength;

  public int Dialog
  {
    get => this.mDialog;
    set => this.mDialog = value;
  }

  internal bool Interact(Character iCaller, Magicka.GameLogic.Controls.Controller iSender)
  {
    if (this.mDialog == 0 || this.mCurrentState is DeadState)
      return false;
    Magicka.GameLogic.UI.Interact interact = DialogManager.Instance.Dialogs[this.mDialog].Peek();
    if (interact != null && interact[interact.Current].TurnToTarget)
    {
      Vector3 position1 = this.Position;
      Vector3 position2 = iCaller.Position;
      Vector3 result;
      Vector3.Subtract(ref position2, ref position1, out result);
      this.CharacterBody.DesiredDirection = result;
    }
    DialogManager.Instance.StartDialog(this.mDialog, (Entity) this, iSender);
    return true;
  }

  public InteractType InteractText
  {
    get
    {
      return this.mDialog == 0 || this.mCurrentState is DeadState ? InteractType.None : DialogManager.Instance.GetDialogIconText(this.mDialog);
    }
  }

  public virtual void Entangle(float iMagnitude)
  {
    if (this.IsImmortal || this.HasStatus(StatusEffects.Burning))
      return;
    NetworkState state = NetworkManager.Instance.State;
    if (state == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Action = ActionType.Entangle,
        Handle = this.Handle,
        Param0I = 0,
        Param1F = iMagnitude
      });
    if (state == NetworkState.Client)
      return;
    if ((double) this.mEntaglement.Magnitude <= 1.4012984643248171E-45)
      this.mEntaglement.Initialize();
    this.mEntaglement.AddEntanglement(iMagnitude);
  }

  public void ReleaseEntanglement()
  {
    if ((double) this.mEntaglement.Magnitude <= 0.0)
      return;
    this.mEntaglement.Release();
  }

  public virtual bool IsEntangled => (double) this.mEntaglement.Magnitude > 1.4012984643248171E-45;

  public virtual void DamageEntanglement(float iAmount, Elements iElement)
  {
    if ((double) this.mEntaglement.Magnitude <= 0.0)
      return;
    this.mEntaglement.DecreaseEntanglement(iAmount, iElement);
  }

  public bool HasGibs()
  {
    return this.mGibs.Count > 0 && GlobalSettings.Instance.BloodAndGore == SettingOptions.On;
  }

  public void SpawnGibs()
  {
    Vector3 position = this.Position;
    bool iFrozen = (this.mDeathStatusEffects & StatusEffects.Frozen) == StatusEffects.Frozen;
    for (int index = 0; index < this.mGibs.Count; ++index)
    {
      Gib fromCache = Gib.GetFromCache();
      if (fromCache != null)
      {
        Vector3 positionOnCollisionSkin = this.GetRandomPositionOnCollisionSkin();
        positionOnCollisionSkin.Y += (double) fromCache.Radius >= 0.10000000149011612 ? fromCache.Radius : 0.1f;
        float scaleFactor = 5f;
        float num1 = (double) this.Radius >= 0.20000000298023224 ? this.Radius : 0.2f;
        float num2 = 6.28318548f * (float) Character.sRandom.NextDouble();
        float num3 = (float) Math.Acos(2.0 * Character.sRandom.NextDouble() - 1.0);
        Vector3 result1 = new Vector3(num1 * (float) Math.Cos((double) num2) * (float) Math.Sin((double) num3), num1 * (float) Math.Sin((double) num2) * (float) Math.Sin((double) num3), num1 * (float) Math.Cos((double) num3));
        Vector3.Normalize(ref result1, out result1);
        Vector3 result2;
        Vector3.Multiply(ref result1, scaleFactor, out result2);
        result2.Y = Math.Abs(result2.Y);
        float iTTL = (float) (10.0 + Character.sRandom.NextDouble() * 10.0);
        fromCache.Initialize(this.mGibs[index].mModel, this.mGibs[index].mMass, this.mGibs[index].mScale, positionOnCollisionSkin, result2, iTTL, (Entity) this, this.mBlood, Gib.GORE_GIB_TRAIL_EFFECTS[(int) this.mBlood], iFrozen);
        Vector3 result3 = new Vector3();
        Vector3.Negate(ref result1, out result3);
        Vector3.Multiply(ref result3, 5f, out result3);
        fromCache.Body.AngularVelocity = result3;
        this.mPlayState.EntityManager.AddEntity((Entity) fromCache);
      }
      else
        break;
    }
    Vector3 direction = this.Direction;
    if (iFrozen)
      EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref position, ref direction, out VisualEffectReference _);
    else if (this.mBlood != BloodType.none)
    {
      int num4 = 1;
      VisualEffectReference oRef;
      if ((double) this.mBody.Mass < 250.0)
      {
        num4 = 2;
        EffectManager.Instance.StartEffect(Gib.GORE_GIB_SMALL_EFFECTS[(int) this.mBlood], ref position, ref direction, out oRef);
      }
      int num5;
      if ((double) this.mBody.Mass > 1000.0)
      {
        num5 = 6;
        EffectManager.Instance.StartEffect(Gib.GORE_GIB_LARGE_EFFECTS[(int) this.mBlood], ref position, ref direction, out oRef);
      }
      else
      {
        num5 = 4;
        EffectManager.Instance.StartEffect(Gib.GORE_GIB_MEDIUM_EFFECTS[(int) this.mBlood], ref position, ref direction, out oRef);
      }
      Segment iSeg = new Segment();
      for (int index = 0; index < num5; ++index)
      {
        iSeg.Origin = this.Position + new Vector3((float) (MagickaMath.Random.NextDouble() * (double) this.Radius * 2.0), 0.0f, (float) (MagickaMath.Random.NextDouble() * (double) this.Radius * 2.0));
        iSeg.Delta.Y = -8f;
        iSeg.Delta.X = (float) ((MagickaMath.Random.NextDouble() - 0.5) * 4.0);
        iSeg.Delta.Z = (float) ((MagickaMath.Random.NextDouble() - 0.5) * 4.0);
        Vector3 oPos;
        Vector3 oNrm;
        AnimatedLevelPart oAnimatedLevelPart;
        if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out oNrm, out oAnimatedLevelPart, iSeg))
          DecalManager.Instance.AddAlphaBlendedDecal((Decal) ((int) this.mBlood * 4 + MagickaMath.Random.Next(0, 3)), oAnimatedLevelPart, 3f + (float) (MagickaMath.Random.NextDouble() * 3.0), ref oPos, ref oNrm, 60f);
      }
    }
    this.mDeadTimer = -100f;
  }

  public Factions GetOriginalFaction => this.mFaction;

  public virtual Factions Faction
  {
    get => this.mFaction;
    set => this.mFaction = value;
  }

  public void ForceDraw() => this.mBoundingScale = 40f;

  public float Panic => this.mMaxPanic;

  public void OrderPanic()
  {
    this.ChangeState((BaseState) PanicState.Instance);
    this.mPanic = 10f;
  }

  public virtual int Type => this.mType;

  public string Name => this.mName;

  public bool CanSeeInvisible => this.mTemplate.CanSeeInvisible;

  public int DisplayName => this.mDisplayName;

  public float SpellCooldown { get; set; }

  public float WanderAngle
  {
    get => this.mWanderAngle;
    set => this.mWanderAngle = value;
  }

  public Magicka.GameLogic.Entities.Resistance[] Resistance => this.mResistances;

  public float NormalizedHitPoints => this.mNormalizedHitPoints;

  public bool Dashing
  {
    get => this.mDashing;
    set => this.mDashing = value;
  }

  public bool Attacking
  {
    get => this.mAttacking;
    set => this.mAttacking = value;
  }

  public Magicka.Animations SpecialAttackAnimation => this.mSpawnAnimation;

  public Magicka.Animations NextAttackAnimation
  {
    get => this.mNextAttackAnimation;
    set => this.mNextAttackAnimation = value;
  }

  public Magicka.Animations NextGripAttackAnimation
  {
    get => this.mNextGripAttackAnimation;
    set => this.mNextGripAttackAnimation = value;
  }

  public Magicka.Animations NextDashAnimation
  {
    get => this.mNextDashAnimation;
    set => this.mNextDashAnimation = value;
  }

  public Attachment[] Equipment => this.mEquipment;

  public MovementProperties MoveAbilities => this.mMoveAbilities;

  public Dictionary<byte, Magicka.Animations[]> MoveAnimations => this.mMoveAnimations;

  public override bool Removable
  {
    get
    {
      return this.Dead && (double) this.mDeadTimer < -10.0 && this.mRemoveAfterDeath && !this.mCannotDieWithoutExplicitKill;
    }
  }

  public bool RemoveAfterDeath
  {
    set => this.mRemoveAfterDeath = value;
    get => this.mRemoveAfterDeath;
  }

  public bool BloatKilled
  {
    get => this.mBloatKilled && GlobalSettings.Instance.BloodAndGore == SettingOptions.On;
  }

  public bool Overkilled
  {
    get
    {
      return ((double) this.mHitPoints <= (double) this.mMaxHitPoints * -0.5 - 500.0 || this.mBloatKilled) && GlobalSettings.Instance.BloodAndGore == SettingOptions.On;
    }
  }

  public bool OverKilled(float iAmount)
  {
    return ((double) iAmount <= (double) this.mMaxHitPoints * -0.5 - 500.0 || this.mBloatKilled) && GlobalSettings.Instance.BloodAndGore == SettingOptions.On;
  }

  public bool AllowAttackRotate => this.mAllowAttackRotate;

  public bool IsKnockedDown
  {
    get => this.mKnockedDown;
    set => this.mKnockedDown = value;
  }

  public bool IsHit
  {
    get => this.mIsHit;
    set => this.mIsHit = value;
  }

  public bool IsPanicing => (double) this.mPanic - (double) this.mMaxPanic > 1.4012984643248171E-45;

  public bool IsStumbling => this.mGripper != null && this.mGripType == Grip.GripType.Ride;

  public abstract bool IsAggressive { get; }

  public bool IsGripping => this.mGrippedCharacter != null;

  public bool IsGripped => this.mGripper != null;

  public Character GrippedCharacter => this.mGrippedCharacter;

  public Character Gripper => this.mGripper;

  public bool ShouldReleaseGrip
  {
    get
    {
      return this.mGrippedCharacter == null || this.mGrippedCharacter.HasStatus(StatusEffects.Burning) | this.mGrippedCharacter.IsImmortal | (double) this.mGrippedCharacter.mBreakFreeCounter >= (double) this.mBreakFreeTolerance || (double) this.mGripDamageAccumulation > (double) this.mHitTolerance;
    }
  }

  public bool AttachedToGripped => this.mGripType == Grip.GripType.Ride;

  public bool GripperAttached => this.mGripType == Grip.GripType.Pickup;

  public Grip.GripType GripType
  {
    get => this.mGripType;
    set => this.mGripType = value;
  }

  public void GripCharacter(
    Character iVictim,
    Grip.GripType iType,
    int iBoneIndex,
    int iTolerance)
  {
    if (iVictim.IsGripped || iVictim.Dead || iVictim.IsImmortal || iVictim.IsEthereal || iVictim.Polymorphed || (double) iVictim.CharacterBody.Mass > 500.0 || this.CurrentState is KnockDownState || iType == Grip.GripType.Ride && iVictim.Template.AnimationClips[0][140] == null)
      return;
    NetworkState state = NetworkManager.Instance.State;
    if (state == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Action = ActionType.Grip,
        Handle = this.Handle,
        TargetHandle = iVictim.Handle,
        Param0I = (int) iType,
        Param1I = iBoneIndex,
        Param2I = iTolerance
      });
    if (state == NetworkState.Client)
      return;
    this.mGripType = iType;
    this.mBreakFreeTolerance = iTolerance > 0 ? (float) iTolerance : 3f;
    this.mGripDamageAccumulation = 0.0f;
    if (iVictim.IsGripped && iVictim.GripperAttached)
      iVictim.Gripper.ReleaseAttachedCharacter();
    if (iVictim.IsGripping)
      iVictim.ReleaseAttachedCharacter();
    iVictim.ReleaseEntanglement();
    this.mGripJoint.mIndex = iBoneIndex;
    if (iBoneIndex >= 0)
    {
      this.mGripJoint.mBindPose = (iType != Grip.GripType.Pickup ? (ReadOnlyCollection<SkinnedModelBone>) iVictim.mAnimationController.Skeleton : (ReadOnlyCollection<SkinnedModelBone>) this.mAnimationController.Skeleton)[iBoneIndex].InverseBindPoseTransform;
      Matrix.Multiply(ref this.mGripJoint.mBindPose, ref this.mStaticTransform, out this.mGripJoint.mBindPose);
      Matrix.Invert(ref this.mGripJoint.mBindPose, out this.mGripJoint.mBindPose);
      Matrix.Multiply(ref Character.sRotateY180, ref this.mGripJoint.mBindPose, out this.mGripJoint.mBindPose);
    }
    this.mGrippedCharacter = iVictim;
    iVictim.mGripper = this;
    iVictim.GripType = iType;
    if (iType == Grip.GripType.Pickup)
      iVictim.mBody.Immovable = true;
    this.mBody.Immovable = true;
    this.mBreakFreeCounter = 0;
    iVictim.mBreakFreeCounter = 0;
  }

  public void ReleaseAttachedCharacter()
  {
    this.mGripDamageAccumulation = 0.0f;
    if (this.mGrippedCharacter == null)
      return;
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Action = ActionType.Release,
        Handle = this.Handle
      });
    this.mGrippedCharacter.CharacterBody.AllowMove = this.mCurrentState != BusyState.Instance && this.mCurrentState != DeadState.Instance && this.mCurrentState != DropState.Instance && this.mCurrentState != LandState.Instance && this.mCurrentState != IdleState.Instance && this.mCurrentState != JumpState.Instance && this.mCurrentState != MoveState.Instance && this.mCurrentState != EntangledState.Instance && this.mCurrentState != GripAttackState.Instance && this.mCurrentState != RessurectionState.Instance;
    this.mGrippedCharacter.CharacterBody.AllowRotate = true;
    this.mGrippedCharacter.mBreakFreeCounter = 0;
    this.mGrippedCharacter.mGripper = (Character) null;
    this.mGrippedCharacter.mBody.Immovable = false;
    this.mBody.Immovable = false;
    Segment iSeg = new Segment(this.mGrippedCharacter.Position, new Vector3(0.0f, -6f, 0.0f));
    iSeg.Origin.Y += 3f;
    Vector3 oPos;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
    {
      oPos.Y += (float) ((double) this.mGrippedCharacter.Capsule.Radius + (double) this.mGrippedCharacter.Capsule.Length * 0.5 + 0.10000000149011612);
      Matrix orientation = this.mGrippedCharacter.Body.Orientation;
      this.mGrippedCharacter.Body.MoveTo(ref oPos, ref orientation);
    }
    this.mGrippedCharacter = (Character) null;
  }

  public bool ForceCamera
  {
    get => this.mForceCamera;
    set => this.mForceCamera = value;
  }

  public bool ForceNavMesh
  {
    get => this.mForceNavMesh;
    set => this.mForceNavMesh = value;
  }

  public bool CollidedWithCamera => this.mCollidedWithCamera;

  public int OnDamageTrigger
  {
    get => this.mOnDamageTrigger;
    set => this.mOnDamageTrigger = value;
  }

  public int OnDeathTrigger
  {
    get => this.mOnDeathTrigger;
    set => this.mOnDeathTrigger = value;
  }

  public virtual void BloatKill(Elements iElement, Entity iKiller)
  {
    if (!this.HasGibs() && (double) this.mHitPoints <= 0.0 || this.mCannotDieWithoutExplicitKill)
      return;
    this.mInvisibilityTimer = 0.0f;
    if (this.HasGibs())
    {
      AudioManager.Instance.PlayCue(Banks.Spells, Railgun.ARCANESTAGESOUNDSHASH[2], this.AudioEmitter);
      this.mBloating = true;
      this.mBloatElement = iElement;
      this.mBloatKiller = iKiller;
      this.mHitPoints = 0.0f;
    }
    else
    {
      this.mBloatElement = iElement;
      this.mBloatKiller = iKiller;
      this.BloatBlast();
    }
  }

  public bool Bloating => this.mBloating;

  public bool CannotDieWithoutExplicitKill
  {
    get => this.mCannotDieWithoutExplicitKill;
    set => this.mCannotDieWithoutExplicitKill = value;
  }

  public virtual void Drown()
  {
    this.mDrowning = true;
    if (this.IsGripped)
      this.mGripper.ReleaseAttachedCharacter();
    this.mInvisibilityTimer = 0.0f;
    this.mHitPoints = 0.0f;
    this.mDeadTimer = 10f;
    Vector3 position = this.Position;
    position.Y += this.WaterDepth;
    this.CharacterBody.MoveTo(position, this.mBody.Orientation);
    if (!this.mCannotDieWithoutExplicitKill)
      this.mBody.DisableBody();
    EffectManager.Instance.Stop(ref this.mStunEffect);
    EffectManager.Instance.Stop(ref this.mFearEffect);
    this.StopHypnotize();
    this.EndCharm();
    Vector3 direction = this.Direction;
    VisualEffectReference oRef;
    if (this.CharacterBody.GroundMaterial == CollisionMaterials.Lava)
      EffectManager.Instance.StartEffect(Defines.LAVA_DROWN_EFFECT, ref position, ref direction, out oRef);
    else
      EffectManager.Instance.StartEffect(Defines.WATER_DROWN_EFFECT, ref position, ref direction, out oRef);
  }

  public bool Drowning => this.mDrowning;

  public void SetCollisionDamage(ref DamageCollection5 iDamages)
  {
    this.mCollisionDamages = iDamages;
  }

  public Magicka.Animations DropAnimation
  {
    get => this.mDropAnimation;
    set => this.mDropAnimation = value;
  }

  public void RemoveSelfShield()
  {
    if (!this.mSelfShield.Active)
      return;
    this.mSelfShield.Remove(this);
  }

  public void RemoveSelfShield(Character.SelfShieldType iType)
  {
    if (iType != this.mSelfShield.mShieldType)
      return;
    this.mSelfShield.Remove(this);
  }

  public void HealSelfShield(float iAmount)
  {
    if (!this.mSelfShield.Active)
      return;
    int num = (int) this.mSelfShield.Damage(ref iAmount, Elements.Shield);
  }

  public Character.SelfShieldType CurrentSelfShieldType => this.mSelfShield.mShieldType;

  public bool IsSelfShielded => this.mSelfShield.Active;

  public void AddSelfShield(Spell iSpell)
  {
    for (int index = 0; index < this.mAuras.Count; ++index)
    {
      ActiveAura mAura = this.mAuras[index];
      if (mAura.SelfCasted)
      {
        mAura.Aura.TTL = 0.0f;
        this.mAuras[index] = mAura;
      }
    }
    if ((double) iSpell[Elements.Earth] == 0.0 && (double) iSpell[Elements.Ice] == 0.0)
    {
      bool flag = !(this.mSelfShield.Active & this.mSelfShield.mShieldType == Character.SelfShieldType.Shield);
      if (this.mSelfShield.Active)
        this.RemoveSelfShield();
      if (!flag)
        return;
      this.mSelfShield = new Character.SelfShield(this, iSpell);
    }
    else
      this.mSelfShield = new Character.SelfShield(this, iSpell);
  }

  public void AddBubbleShield(AuraStorage aura)
  {
    this.mBubble = true;
    this.AddAura(ref aura, true);
  }

  public void RemoveBubbleShield()
  {
    if (!this.mBubble)
      return;
    this.ClearAura();
  }

  public bool HaveBubbleShield() => this.mBubble;

  public bool IsSolidSelfShielded => this.mSelfShield.Active && this.mSelfShield.Solid;

  public void MeleeDamageBoost(float iPercentage)
  {
    this.mMeleeBoostAmount = iPercentage;
    this.mMeleeBoostTimer = 5f;
  }

  public bool MeleeBoosted => (double) this.mMeleeBoostTimer > 0.0;

  public float MeleeBoostAmount => this.mMeleeBoostAmount;

  public bool GripAttacking
  {
    get => this.mGripAttack;
    set => this.mGripAttack = value;
  }

  public float SpellPower
  {
    get => this.mSpellPower;
    set => this.mSpellPower = value;
  }

  public float GripDamageAccumulation => this.mGripDamageAccumulation;

  public float HitTolerance => this.mHitTolerance;

  public float BreakFreeTolerance
  {
    get => this.mBreakFreeTolerance;
    set => this.mBreakFreeTolerance = value;
  }

  public bool IsEthereal
  {
    get => this.mEthereal;
    set
    {
      this.mEthereal = value;
      if (!this.mEthereal)
        return;
      this.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
    }
  }

  public float EtherealAlpha
  {
    get => this.mEtherealAlpha;
    set => this.mEtherealAlpha = this.mEtherealAlphaTarget = value;
  }

  public void Ethereal(bool iEthereal, float iAlpha, float iSpeed)
  {
    this.mEthereal = iEthereal;
    if (this.mEthereal)
      this.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
    this.mEtherealAlphaTarget = iAlpha;
    this.mEtherealAlphaSpeed = iSpeed;
  }

  internal void TimedEthereal(float iTime, bool iEtherealState)
  {
    if ((double) iTime <= 0.0)
    {
      this.mEthereal = iEtherealState;
      this.mEtherealTimer = 0.0f;
    }
    else
      this.mEtherealTimer = iTime;
  }

  public float TimeWarpModifier
  {
    get => this.mTimeWarpModifier;
    set => this.mTimeWarpModifier = value;
  }

  public bool IsFearless
  {
    get => this.mFearless;
    set
    {
      this.mFearless = value;
      if (!this.mFearless)
        return;
      this.mFearTimer = 0.0f;
    }
  }

  public bool IsUncharmable
  {
    get => this.mUncharmable;
    set
    {
      this.mUncharmable = value;
      if (!this.mUncharmable)
        return;
      this.mCharmTimer = 0.0f;
    }
  }

  public bool IsNonslippery => this.mNonslippery;

  public bool HasFairy => this.mHasFairy;

  internal void ResetStaticTransform()
  {
    this.mStaticTransform = this.mTemplate.Models[this.mModelID].Transform;
  }

  internal void SetStaticTransform(float iX, float iY, float iZ)
  {
    Matrix result;
    Matrix.CreateScale(iX, iY, iZ, out result);
    Matrix.Multiply(ref result, ref this.mTemplate.Models[this.mModelID].Transform, out this.mStaticTransform);
  }

  internal void ResetCapsuleForm()
  {
    this.SetCapsuleForm(this.mTemplate.Length, this.mTemplate.Radius);
  }

  internal void SetCapsuleForm(float iLength, float iRadius)
  {
    Capsule primitiveNewWorld = this.Body.CollisionSkin.GetPrimitiveNewWorld(0) as Capsule;
    Capsule primitiveOldWorld = this.Body.CollisionSkin.GetPrimitiveOldWorld(0) as Capsule;
    Capsule primitiveLocal = this.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule;
    float length1 = primitiveLocal.Length;
    float radius1 = primitiveLocal.Radius;
    float length2 = primitiveNewWorld.Length;
    float radius2 = primitiveNewWorld.Radius;
    float length3 = primitiveOldWorld.Length;
    float radius3 = primitiveOldWorld.Radius;
    primitiveLocal.Length = iLength;
    primitiveLocal.Radius = iRadius;
    primitiveNewWorld.Length = iLength;
    primitiveNewWorld.Radius = iRadius;
    primitiveOldWorld.Length = iLength;
    primitiveOldWorld.Radius = iRadius;
    Vector3 position1 = primitiveLocal.Position with
    {
      Y = iLength * -0.5f
    };
    primitiveLocal.Position = position1;
    Vector3 position2 = primitiveNewWorld.Position;
    position2.Y -= (float) (((double) length2 - (double) iLength) * 0.5);
    position2.Y -= radius2 - iRadius;
    primitiveNewWorld.Position = position2;
    Vector3 position3 = primitiveOldWorld.Position;
    position3.Y -= (float) (((double) length3 - (double) iLength) * 0.5);
    position3.Y -= radius3 - iRadius;
    primitiveOldWorld.Position = position3;
    this.HeightOffset = (float) (-(double) iRadius - 0.5 * (double) iLength);
    Matrix orientation = this.Body.Orientation;
    Vector3 position4 = this.Position;
    position4.Y -= (float) (((double) length1 - (double) iLength) * 0.5);
    position4.Y -= radius1 - iRadius;
    this.Body.MoveTo(position4, orientation);
    this.mRadius = iRadius;
  }

  public CharacterTemplate Template => this.mTemplate;

  internal ConditionCollection EventConditions
  {
    get => this.mEventConditions;
    set => this.mEventConditions = value;
  }

  public override bool ArcIntersect(
    out Vector3 oPosition,
    Vector3 iOrigin,
    Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference)
  {
    if (!this.mEthereal)
      return base.ArcIntersect(out oPosition, iOrigin, iDirection, iRange, iAngle, iHeightDifference);
    oPosition = new Vector3();
    return false;
  }

  internal void AccumulateArcaneDamage(Elements iElement, float iMagnitude)
  {
    this.mBloatDamageAccumulation[MagickaMath.CountTrailingZeroBits((uint) iElement)] += iMagnitude;
  }

  internal virtual void NetworkAction(ref CharacterActionMessage iMsg)
  {
    switch (iMsg.Action)
    {
      case ActionType.ConjureElement:
        Spell oSpell;
        Spell.DefaultSpell((Elements) iMsg.Param0I, out oSpell);
        SpellManager.Instance.TryAddToQueue((Player) null, this, this.mSpellQueue, 5, ref oSpell);
        break;
      case ActionType.Attack:
        this.mNextAttackAnimation = (Magicka.Animations) (iMsg.Param0I & int.MaxValue);
        this.mAllowAttackRotate = ((long) iMsg.Param0I & 2147483648L /*0x80000000*/) != 0L;
        this.CastType = (CastType) iMsg.Param1I;
        this.mSpellPower = iMsg.Param2F;
        this.mAttacking = true;
        break;
      case ActionType.Block:
        this.mBlock = iMsg.Param0I != 0;
        break;
      case ActionType.BreakFree:
        if (!(this.IsEntangled | this.IsGripped))
          break;
        this.BreakFree();
        if (this.mCurrentState is CastState)
          break;
        this.GoToAnimation(Magicka.Animations.spec_entangled_attack, 0.1f);
        break;
      case ActionType.Grip:
        Character fromHandle1 = Entity.GetFromHandle((int) iMsg.TargetHandle) as Character;
        Grip.GripType param0I1 = (Grip.GripType) iMsg.Param0I;
        this.mGripType = param0I1;
        this.mBreakFreeTolerance = iMsg.Param2I > 0 ? (float) iMsg.Param2I : 3f;
        this.mGripDamageAccumulation = 0.0f;
        if (fromHandle1.IsGripped && fromHandle1.GripperAttached)
          fromHandle1.Gripper.ReleaseAttachedCharacter();
        if (fromHandle1.IsGripping)
          fromHandle1.ReleaseAttachedCharacter();
        fromHandle1.ReleaseEntanglement();
        this.mGripJoint.mIndex = iMsg.Param1I;
        if (iMsg.Param1I >= 0)
        {
          this.mGripJoint.mBindPose = (param0I1 != Grip.GripType.Pickup ? (ReadOnlyCollection<SkinnedModelBone>) fromHandle1.mAnimationController.Skeleton : (ReadOnlyCollection<SkinnedModelBone>) this.mAnimationController.Skeleton)[iMsg.Param1I].InverseBindPoseTransform;
          Matrix.Multiply(ref this.mGripJoint.mBindPose, ref this.mStaticTransform, out this.mGripJoint.mBindPose);
          Matrix.Invert(ref this.mGripJoint.mBindPose, out this.mGripJoint.mBindPose);
          Matrix.Multiply(ref Character.sRotateY180, ref this.mGripJoint.mBindPose, out this.mGripJoint.mBindPose);
        }
        this.mGrippedCharacter = fromHandle1;
        fromHandle1.mGripper = this;
        fromHandle1.GripType = param0I1;
        if (param0I1 == Grip.GripType.Pickup)
          fromHandle1.mBody.Immovable = true;
        this.mBody.Immovable = true;
        this.mBreakFreeCounter = 0;
        fromHandle1.mBreakFreeCounter = 0;
        break;
      case ActionType.GripAttack:
        this.mNextGripAttackAnimation = (Magicka.Animations) iMsg.Param0I;
        this.mAllowAttackRotate = false;
        this.mGripAttack = true;
        this.mAttacking = false;
        break;
      case ActionType.Entangle:
        if ((double) this.mEntaglement.Magnitude <= 1.4012984643248171E-45)
          this.mEntaglement.Initialize();
        this.mEntaglement.AddEntanglement(iMsg.Param1F);
        break;
      case ActionType.Release:
        if (this.IsEntangled)
        {
          this.ReleaseEntanglement();
          break;
        }
        if (this.IsGripped)
        {
          this.mGripDamageAccumulation = 0.0f;
          this.mGripper.ReleaseAttachedCharacter();
          break;
        }
        if (!this.IsGripping)
          break;
        this.mGripDamageAccumulation = 0.0f;
        this.mGrippedCharacter.CharacterBody.AllowMove = this.mCurrentState != BusyState.Instance && this.mCurrentState != DeadState.Instance && this.mCurrentState != DropState.Instance && this.mCurrentState != LandState.Instance && this.mCurrentState != IdleState.Instance && this.mCurrentState != JumpState.Instance && this.mCurrentState != MoveState.Instance && this.mCurrentState != EntangledState.Instance && this.mCurrentState != GripAttackState.Instance && this.mCurrentState != RessurectionState.Instance;
        this.mGrippedCharacter.CharacterBody.AllowRotate = true;
        this.mGrippedCharacter.mBreakFreeCounter = 0;
        this.mGrippedCharacter.mGripper = (Character) null;
        this.mGrippedCharacter.mBody.Immovable = false;
        this.mBody.Immovable = false;
        Segment iSeg = new Segment(this.mGrippedCharacter.Position, new Vector3(0.0f, -6f, 0.0f));
        iSeg.Origin.Y += 3f;
        Vector3 oPos;
        if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
        {
          oPos.Y += (float) ((double) this.mGrippedCharacter.Capsule.Radius + (double) this.mGrippedCharacter.Capsule.Length * 0.5 + 0.10000000149011612);
          Matrix orientation = this.mGrippedCharacter.Body.Orientation;
          this.mGrippedCharacter.Body.MoveTo(ref oPos, ref orientation);
        }
        this.mGrippedCharacter = (Character) null;
        break;
      case ActionType.Dash:
        this.mNextDashAnimation = (Magicka.Animations) (iMsg.Param0I & int.MaxValue);
        this.mAllowAttackRotate = ((long) iMsg.Param0I & 2147483648L /*0x80000000*/) != 0L;
        this.mDashing = true;
        break;
      case ActionType.Jump:
        if (this.IsEntangled || this.Dead)
          break;
        Vector3 result1 = new Vector3(iMsg.Param0F, iMsg.Param1F, iMsg.Param2F);
        float timeStamp = (float) iMsg.TimeStamp;
        float y = result1.Y;
        result1.Y = 0.0f;
        float num1 = result1.Length();
        Vector3.Divide(ref result1, num1, out result1);
        float num2 = result1.Y = (float) Math.Sin((double) timeStamp);
        float num3 = (float) Math.Cos((double) timeStamp);
        result1.X *= num3;
        result1.Z *= num3;
        float num4 = (float) Math.Sqrt((double) PhysicsManager.Instance.Simulator.Gravity.Y * -1.0 * (double) num1 * (double) num1 / (2.0 * ((double) num1 * (double) num2 / (double) num3 - (double) y) * (double) num3 * (double) num3));
        if (float.IsNaN(num4) || float.IsInfinity(num4))
          break;
        Vector3.Multiply(ref result1, num4, out result1);
        this.CharacterBody.AddJump(result1);
        break;
      case ActionType.Magick:
        switch ((MagickType) iMsg.Param3I)
        {
          case MagickType.Teleport:
            Vector3 iPosition = new Vector3(iMsg.Param0F, iMsg.Param1F, iMsg.Param2F);
            Vector3 result2 = this.Position;
            if (iMsg.TargetHandle != (ushort) 0)
              result2 = Entity.GetFromHandle((int) iMsg.TargetHandle).Position;
            Vector3.Subtract(ref iPosition, ref result2, out result2);
            result2.Y = 0.0f;
            result2.Normalize();
            Teleport.Instance.DoTeleport((ISpellCaster) this, iPosition, result2, (Teleport.TeleportType) iMsg.Param4I);
            return;
          case MagickType.Fear:
            if (iMsg.TargetHandle != (ushort) 0)
            {
              this.Fear(Entity.GetFromHandle((int) iMsg.TargetHandle) as Character);
              return;
            }
            this.Fear(new Vector3(iMsg.Param0F, iMsg.Param1F, iMsg.Param2F));
            return;
          case MagickType.CTD:
            Character fromHandle2 = Entity.GetFromHandle((int) iMsg.TargetHandle) as Character;
            Matrix result3;
            Matrix.CreateScale(fromHandle2.Radius, fromHandle2.Capsule.Length + fromHandle2.Radius * 2f, fromHandle2.Radius, out result3);
            result3.Translation = fromHandle2.Position;
            EffectManager.Instance.StartEffect(CTD.EFFECT, ref result3, out VisualEffectReference _);
            AudioManager.Instance.PlayCue(Banks.Additional, CTD.SOUND, fromHandle2.AudioEmitter);
            if (this.PlayState.Level.CurrentScene.RuleSet is SurvivalRuleset && fromHandle2 is NonPlayerCharacter && fromHandle2.DisplayName != 0)
              NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString(CTD.MESSAGE).Replace("#1;", LanguageManager.Instance.GetString(fromHandle2.DisplayName)));
            if (NetworkManager.Instance.State != NetworkState.Server)
              return;
            fromHandle2.Terminate(true, false);
            return;
          case MagickType.Levitate:
            Levitate.CastLevitate(this);
            return;
          default:
            throw new Exception($"{this.mName} failed to handle MagicType {(Enum) (MagickType) iMsg.Param3I} by way of networkaction!");
        }
      case ActionType.SelfShield:
        Elements param0I2 = (Elements) iMsg.Param0I;
        this.AddSelfShield(new Spell()
        {
          Element = param0I2,
          [Elements.Earth] = iMsg.Param1F,
          [Elements.Ice] = iMsg.Param2F,
          [Elements.Shield] = 1f
        });
        break;
      case ActionType.EventAnimation:
        if ((ushort) iMsg.Param2I != (ushort) 0)
          this.SpecialIdleAnimation = (Magicka.Animations) iMsg.Param2I;
        if ((ushort) iMsg.Param0I == (ushort) 0)
          break;
        if (this.mCurrentState is IdleState)
        {
          this.GoToAnimation((Magicka.Animations) iMsg.Param0I, iMsg.Param1F);
          break;
        }
        this.mNetworkAnimation = (Magicka.Animations) iMsg.Param0I;
        this.mNetworkAnimationBlend = iMsg.Param1F;
        break;
      default:
        throw new NotImplementedException();
    }
  }

  protected override unsafe void INetworkUpdate(ref EntityUpdateMessage iMsg)
  {
    base.INetworkUpdate(ref iMsg);
    if ((iMsg.Features & EntityFeatures.Damageable) != EntityFeatures.None)
      this.mHitPoints = iMsg.HitPoints;
    if ((iMsg.Features & EntityFeatures.SelfShield) != EntityFeatures.None)
    {
      this.mSelfShield.mShieldType = iMsg.SelfShieldType;
      this.mSelfShield.mSelfShieldHealth = iMsg.SelfShieldHealth;
    }
    if ((iMsg.Features & EntityFeatures.Etherealized) != EntityFeatures.None)
    {
      if (iMsg.EtherealState)
      {
        if (!this.IsEthereal)
          this.Ethereal(true, 1f, 1f);
      }
      else if (this.IsEthereal)
        this.Ethereal(false, 1f, 1f);
    }
    if ((iMsg.Features & EntityFeatures.StatusEffected) != EntityFeatures.None)
    {
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
                int index1 = StatusEffect.StatusIndex(StatusEffects.Burning);
                int index2 = StatusEffect.StatusIndex(StatusEffects.Wet);
                int index3 = StatusEffect.StatusIndex(StatusEffects.Frozen);
                switch (iStatus)
                {
                  case StatusEffects.Burning:
                    if (this.mStatusEffectCues[index1] != null && !this.mStatusEffectCues[index1].IsStopping)
                    {
                      this.mStatusEffectCues[index1].Stop(AudioStopOptions.AsAuthored);
                      break;
                    }
                    break;
                  case StatusEffects.Wet:
                    if (this.mStatusEffectCues[index2] != null && !this.mStatusEffectCues[index2].IsStopping)
                    {
                      this.mStatusEffectCues[index2].Stop(AudioStopOptions.AsAuthored);
                      break;
                    }
                    break;
                  case StatusEffects.Frozen:
                    if (this.mStatusEffectCues[index3] != null && !this.mStatusEffectCues[index3].IsStopping)
                    {
                      this.mStatusEffectCues[index3].Stop(AudioStopOptions.AsAuthored);
                      break;
                    }
                    break;
                }
                this.mStatusEffects[iIndex].Stop();
                this.mStatusEffects[iIndex] = new StatusEffect();
              }
            }
            else if ((double) numPtr1[iIndex] > 0.0 && this.mResistances != null)
            {
              int num = (int) this.AddStatusEffect(new StatusEffect(iStatus, numPtr2[iIndex], numPtr1[iIndex], this.Capsule.Length, this.Capsule.Radius));
            }
          }
        }
    }
    if ((iMsg.Features & EntityFeatures.Direction) != EntityFeatures.None)
    {
      if ((double) iMsg.GenericFloat > 1.4012984643248171E-45)
      {
        this.CharacterBody.Movement = new Vector3()
        {
          X = (float) Math.Cos((double) iMsg.Direction) * iMsg.GenericFloat,
          Z = (float) Math.Sin((double) iMsg.Direction) * iMsg.GenericFloat
        };
      }
      else
      {
        this.CharacterBody.Movement = new Vector3();
        this.CharacterBody.DesiredDirection = new Vector3()
        {
          X = (float) Math.Cos((double) iMsg.Direction),
          Z = (float) Math.Sin((double) iMsg.Direction)
        };
      }
    }
    if ((iMsg.Features & EntityFeatures.WanderAngle) == EntityFeatures.None)
      return;
    this.WanderAngle = iMsg.WanderAngle;
  }

  protected override unsafe void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    if (!this.RestingMovement)
    {
      bool isTouchingGround = this.CharacterBody.IsTouchingGround;
      Vector3 result1 = this.mBody.Velocity;
      Vector3 result2;
      if (isTouchingGround)
      {
        Vector3 vector3 = new Vector3();
        result2 = new Vector3();
      }
      else
      {
        Vector3 result3 = PhysicsManager.Instance.Simulator.Gravity;
        Vector3.Multiply(ref result3, iPrediction, out result3);
        Vector3.Multiply(ref result3, iPrediction, out result2);
        oMsg.Features |= EntityFeatures.Velocity;
        Vector3.Add(ref result1, ref result3, out oMsg.Velocity);
      }
      oMsg.Features |= EntityFeatures.Position;
      oMsg.Position = this.Position;
      Vector3.Multiply(ref result1, iPrediction, out result1);
      Vector3.Add(ref result1, ref oMsg.Position, out oMsg.Position);
      if (!isTouchingGround)
        Vector3.Add(ref result2, ref oMsg.Position, out oMsg.Position);
    }
    oMsg.Features |= EntityFeatures.Direction;
    Vector3 desiredDirection = this.CharacterBody.DesiredDirection with
    {
      Y = 0.0f
    };
    desiredDirection.Normalize();
    oMsg.Direction = (float) Math.Atan2((double) desiredDirection.Z, (double) desiredDirection.X);
    oMsg.Features |= EntityFeatures.GenericFloat;
    oMsg.GenericFloat = this.CharacterBody.Movement.Length();
    if (this.IsStumbling | this.IsPanicing)
    {
      oMsg.Features |= EntityFeatures.WanderAngle;
      oMsg.WanderAngle = this.WanderAngle;
    }
    oMsg.Features |= EntityFeatures.Etherealized;
    oMsg.EtherealState = this.IsEthereal;
    oMsg.Features |= EntityFeatures.SelfShield;
    if (this.mSelfShield.Active)
    {
      oMsg.SelfShieldHealth = this.mSelfShield.mSelfShieldHealth;
      oMsg.SelfShieldType = this.mSelfShield.mShieldType;
    }
    else
    {
      oMsg.SelfShieldHealth = 0.0f;
      oMsg.SelfShieldType = Character.SelfShieldType.None;
    }
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

  internal override float GetDanger() => (float) this.mSpellQueue.Count;

  public void GetDamageModifier(Elements iElement, out float oMultiplyer, out float oBias)
  {
    oMultiplyer = 1f;
    oBias = 0.0f;
    foreach (BuffStorage mBuff in this.mBuffs)
    {
      if (mBuff.BuffType == BuffType.BoostDamage && (mBuff.BuffBoostDamage.Damage.Element & iElement) != Elements.None)
      {
        oBias += mBuff.BuffBoostDamage.Damage.Amount;
        oMultiplyer *= mBuff.BuffBoostDamage.Damage.Magnitude;
      }
    }
  }

  public void GetSpellTTLModifier(ref float iTTL)
  {
    float num1 = 1f;
    float num2 = 0.0f;
    foreach (BuffStorage mBuff in this.mBuffs)
    {
      if (mBuff.BuffType == BuffType.ModifySpellTTL)
      {
        num1 *= mBuff.BuffModifySpellTTL.TTLMultiplier;
        num2 += mBuff.BuffModifySpellTTL.TTLModifier;
      }
    }
    iTTL *= num1;
    iTTL += num2;
  }

  public void GetSpellRangeModifier(ref float iRange)
  {
    float num1 = 1f;
    float num2 = 0.0f;
    foreach (BuffStorage mBuff in this.mBuffs)
    {
      if (mBuff.BuffType == BuffType.ModifySpellRange)
      {
        num1 *= mBuff.BuffModifySpellRange.RangeMultiplier;
        num2 += mBuff.BuffModifySpellRange.RangeModifier;
      }
    }
    iRange *= num1;
    iRange += num2;
  }

  public float GetAgroMultiplier()
  {
    float agroMultiplier = 1f;
    foreach (BuffStorage mBuff in this.mBuffs)
    {
      if (mBuff.BuffType == BuffType.ReduceAgro)
        agroMultiplier *= mBuff.BuffReduceAgro.Amount;
    }
    return agroMultiplier;
  }

  internal bool HasAura(AuraType iType)
  {
    for (int index = 0; index < this.mAuras.Count; ++index)
    {
      if (this.mAuras[index].Aura.AuraType == iType)
        return true;
    }
    return false;
  }

  internal float GetResistanceBuffMultiplier(Elements iElement)
  {
    float resistanceBuffMultiplier = 1f;
    for (int index = 0; index < this.mBuffs.Count; ++index)
    {
      if (this.mBuffs[index].BuffType == BuffType.Resistance && (this.mBuffs[index].BuffResistance.Resistance.ResistanceAgainst & iElement) != Elements.None)
        resistanceBuffMultiplier *= this.mBuffs[index].BuffResistance.Resistance.Multiplier;
    }
    return resistanceBuffMultiplier;
  }

  internal float IsResistantAgainst(Elements iElement)
  {
    float num = 1f * this.GetResistanceBuffMultiplier(iElement);
    for (int index = 0; index < this.mResistances.Length; ++index)
    {
      if ((this.mResistances[index].ResistanceAgainst & iElement) != Elements.None)
        num *= this.mResistances[index].Multiplier;
    }
    for (int index1 = 0; index1 < this.mEquipment.Length; ++index1)
    {
      if (this.mEquipment[index1].Item != null)
      {
        Magicka.GameLogic.Entities.Resistance[] resistance = this.mEquipment[index1].Item.Resistance;
        for (int index2 = 0; index2 < resistance.Length; ++index2)
        {
          if ((resistance[index2].ResistanceAgainst & iElement) != Elements.None)
            num *= resistance[index2].Multiplier;
        }
      }
    }
    return num;
  }

  internal void ScaleGraphicModel(float iScale)
  {
    this.mStaticTransform.M11 = iScale;
    this.mStaticTransform.M22 = iScale;
    this.mStaticTransform.M33 = iScale;
  }

  public bool HasPassiveAbility(Item.PassiveAbilities iAbility)
  {
    for (int index = 0; index < this.mEquipment.Length; ++index)
    {
      if (this.mEquipment[index].Item.Attached && this.mEquipment[index].Item.PassiveAbility.Ability == iAbility)
        return true;
    }
    return false;
  }

  protected class LightningZapRenderData : IPostEffect, IPreRenderRenderer
  {
    public BoundingSphere mBoundingSphere;
    public Matrix[] mSkeleton;
    protected bool mMeshDirty = true;
    protected VertexDeclaration mVertexDeclaration;
    protected VertexBuffer mVertexBuffer;
    protected IndexBuffer mIndexBuffer;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    public SkinnedModelDeferredBasicMaterial mMaterial;
    protected VertexDeclaration mSkeletonVertexDeclaration;
    protected int mSkeletonPrimitiveCount;
    protected int mSkeletonVertexStride;
    protected VertexBuffer mSkeletonVertexBuffer;
    protected static VertexDeclaration sFlashVertexDeclaration;
    protected static VertexBuffer sFlashVertexBuffer;
    protected static Texture2D sFlashTexture;
    protected Matrix mFlashTransform;

    public LightningZapRenderData()
    {
      GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
      if (Character.LightningZapRenderData.sFlashVertexBuffer == null || Character.LightningZapRenderData.sFlashVertexBuffer.IsDisposed)
      {
        VertexPositionTexture[] data = new VertexPositionTexture[4];
        data[0].Position.X = 1f;
        data[0].Position.Y = 1f;
        data[0].TextureCoordinate.X = 0.0f;
        data[0].TextureCoordinate.Y = 0.0f;
        data[1].Position.X = -1f;
        data[1].Position.Y = 1f;
        data[1].TextureCoordinate.X = 1f;
        data[1].TextureCoordinate.Y = 0.0f;
        data[2].Position.X = -1f;
        data[2].Position.Y = -1f;
        data[2].TextureCoordinate.X = 1f;
        data[2].TextureCoordinate.Y = 1f;
        data[3].Position.X = 1f;
        data[3].Position.Y = -1f;
        data[3].TextureCoordinate.X = 0.0f;
        data[3].TextureCoordinate.Y = 1f;
        lock (graphicsDevice)
        {
          Character.LightningZapRenderData.sFlashVertexBuffer = new VertexBuffer(graphicsDevice, 4 * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
          Character.LightningZapRenderData.sFlashVertexBuffer.SetData<VertexPositionTexture>(data);
        }
      }
      if (Character.LightningZapRenderData.sFlashVertexDeclaration == null || Character.LightningZapRenderData.sFlashVertexDeclaration.IsDisposed)
      {
        lock (graphicsDevice)
          Character.LightningZapRenderData.sFlashVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
      }
      if (Character.LightningZapRenderData.sFlashTexture != null && !Character.LightningZapRenderData.sFlashTexture.IsDisposed)
        return;
      lock (graphicsDevice)
        Character.LightningZapRenderData.sFlashTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/LightningHit");
    }

    public void SetMeshDirty() => this.mMeshDirty = true;

    public bool MeshDirty => this.mMeshDirty;

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      ref SkinnedModelDeferredBasicMaterial iBasicMaterial,
      VertexBuffer iSkeletonVertices,
      VertexDeclaration iSkeletonVertexDeclaration,
      int iSkeletonVertexStride,
      int iSkeletonPrimitiveCount)
    {
      this.mMeshDirty = false;
      this.mMaterial = iBasicMaterial;
      this.mVertexBuffer = iVertices;
      this.mIndexBuffer = iIndices;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
      this.mSkeletonVertexBuffer = iSkeletonVertices;
      this.mSkeletonVertexDeclaration = iSkeletonVertexDeclaration;
      this.mSkeletonVertexStride = iSkeletonVertexStride;
      this.mSkeletonPrimitiveCount = iSkeletonPrimitiveCount;
    }

    public int ZIndex => 0;

    public void Draw(
      float iDeltaTime,
      ref Vector2 iPixelSize,
      ref Matrix iViewMatrix,
      ref Matrix iProjectionMatrix,
      Texture2D iCandidate,
      Texture2D iDepthMap,
      Texture2D iNormalMap)
    {
      SkinnedModelSkeletonEffect effect1 = RenderManager.Instance.GetEffect(SkinnedModelSkeletonEffect.TYPEHASH) as SkinnedModelSkeletonEffect;
      effect1.GraphicsDevice.RenderState.StencilEnable = true;
      effect1.GraphicsDevice.RenderState.ReferenceStencil = 1;
      effect1.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Always;
      effect1.GraphicsDevice.RenderState.StencilPass = StencilOperation.Replace;
      effect1.GraphicsDevice.RenderState.AlphaBlendEnable = false;
      effect1.GraphicsDevice.RenderState.DepthBufferEnable = true;
      effect1.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
      effect1.CurrentTechnique = effect1.Techniques[0];
      effect1.DiffuseMap0 = this.mMaterial.DiffuseMap0;
      effect1.DiffuseMap0Enabled = this.mMaterial.DiffuseMap0Enabled;
      effect1.DiffuseMap1 = this.mMaterial.DiffuseMap1;
      effect1.DiffuseMap1Enabled = this.mMaterial.DiffuseMap1Enabled;
      effect1.DepthMap = iDepthMap;
      effect1.View = iViewMatrix;
      effect1.Projection = iProjectionMatrix;
      effect1.Bones = this.mSkeleton;
      effect1.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
      effect1.GraphicsDevice.Indices = this.mIndexBuffer;
      effect1.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      effect1.Begin();
      effect1.CurrentTechnique.Passes[0].Begin();
      effect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      effect1.CurrentTechnique.Passes[0].End();
      effect1.End();
      effect1.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
      effect1.GraphicsDevice.RenderState.StencilPass = StencilOperation.Keep;
      effect1.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      effect1.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
      effect1.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
      effect1.CurrentTechnique = effect1.Techniques[1];
      effect1.GraphicsDevice.Vertices[0].SetSource(this.mSkeletonVertexBuffer, 0, this.mSkeletonVertexStride);
      effect1.GraphicsDevice.VertexDeclaration = this.mSkeletonVertexDeclaration;
      effect1.Begin();
      effect1.CurrentTechnique.Passes[0].Begin();
      effect1.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip, 0, this.mSkeletonPrimitiveCount);
      effect1.CurrentTechnique.Passes[0].End();
      effect1.End();
      ArcaneEffect effect2 = RenderManager.Instance.GetEffect(ArcaneEffect.TYPEHASH) as ArcaneEffect;
      effect1.GraphicsDevice.RenderState.StencilFunction = CompareFunction.NotEqual;
      effect1.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
      effect1.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
      effect2.Alpha = 1f;
      effect2.ColorCenter = Spell.LIGHTNINGCOLOR;
      effect2.ColorEdge = Spell.LIGHTNINGCOLOR;
      effect2.Texture = Character.LightningZapRenderData.sFlashTexture;
      effect2.TextureScale = 1f;
      effect2.World = this.mFlashTransform;
      effect2.ViewProjection = iViewMatrix * iProjectionMatrix;
      effect2.CurrentTechnique = effect2.Techniques[0];
      effect2.GraphicsDevice.Vertices[0].SetSource(Character.LightningZapRenderData.sFlashVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      effect2.GraphicsDevice.VertexDeclaration = Character.LightningZapRenderData.sFlashVertexDeclaration;
      effect2.Begin();
      effect2.CurrentTechnique.Passes[0].Begin();
      effect2.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      effect2.CurrentTechnique.Passes[0].End();
      effect2.End();
      effect1.GraphicsDevice.RenderState.StencilEnable = false;
    }

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      Matrix.CreateBillboard(ref this.mBoundingSphere.Center, ref iCameraPosition, ref new Vector3()
      {
        Y = 1f
      }, new Vector3?(iCameraDirection), out this.mFlashTransform);
      this.mFlashTransform.M11 *= this.mBoundingSphere.Radius * 0.85f;
      this.mFlashTransform.M12 *= this.mBoundingSphere.Radius * 0.85f;
      this.mFlashTransform.M13 *= this.mBoundingSphere.Radius * 0.85f;
      this.mFlashTransform.M21 *= this.mBoundingSphere.Radius * 0.85f;
      this.mFlashTransform.M22 *= this.mBoundingSphere.Radius * 0.85f;
      this.mFlashTransform.M23 *= this.mBoundingSphere.Radius * 0.85f;
      this.mFlashTransform.M31 *= this.mBoundingSphere.Radius * 0.85f;
      this.mFlashTransform.M32 *= this.mBoundingSphere.Radius * 0.85f;
      this.mFlashTransform.M33 *= this.mBoundingSphere.Radius * 0.85f;
      Matrix result;
      Matrix.CreateRotationZ(MagickaMath.RandomBetween(0.0f, 6.28318548f), out result);
      Matrix.Multiply(ref result, ref this.mFlashTransform, out this.mFlashTransform);
    }
  }

  protected class ShieldSkinRenderData : IRenderableAdditiveObject
  {
    private static Texture2D sTexture;
    public BoundingSphere mBoundingSphere;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    protected int mVerticesHash;
    protected VertexBuffer mVertexBuffer;
    protected IndexBuffer mIndexBuffer;
    protected bool mMeshDirty = true;
    public Matrix[] mSkeleton;
    public Vector2 mTextureScale;
    public Vector2 mTextureOffset0;
    public Vector2 mTextureOffset1;
    public Vector2 mTextureOffset2;
    public Matrix mProjectionMatrix0;
    public Matrix mProjectionMatrix1;
    public Matrix mProjectionMatrix2;
    public Vector4 mColor;

    public ShieldSkinRenderData()
    {
      if (Character.ShieldSkinRenderData.sTexture != null && !Character.ShieldSkinRenderData.sTexture.IsDisposed)
        return;
      lock (Magicka.Game.Instance.GraphicsDevice)
        Character.ShieldSkinRenderData.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/Shield");
    }

    public bool MeshDirty => this.mMeshDirty;

    public int Effect => SkinnedShieldEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedShieldEffect skinnedShieldEffect = iEffect as SkinnedShieldEffect;
      skinnedShieldEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
      skinnedShieldEffect.ProjectionMap = Character.ShieldSkinRenderData.sTexture;
      skinnedShieldEffect.Bones = this.mSkeleton;
      skinnedShieldEffect.Color = this.mColor;
      skinnedShieldEffect.TextureScale = this.mTextureScale;
      skinnedShieldEffect.TextureOffset0 = this.mTextureOffset0;
      skinnedShieldEffect.TextureOffset1 = this.mTextureOffset1;
      skinnedShieldEffect.TextureOffset2 = this.mTextureOffset2;
      skinnedShieldEffect.ProjectionMapMatrix0 = this.mProjectionMatrix0;
      skinnedShieldEffect.ProjectionMapMatrix1 = this.mProjectionMatrix1;
      skinnedShieldEffect.ProjectionMapMatrix2 = this.mProjectionMatrix2;
      skinnedShieldEffect.Bloat = 0.075f;
      skinnedShieldEffect.CommitChanges();
      skinnedShieldEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      skinnedShieldEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    }

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
    {
      this.mMeshDirty = false;
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
    }
  }

  protected class BarrierSkinRenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
  {
    public Matrix[] mSkeleton;
    public SkinnedModelDeferredAdvancedMaterial mIceMaterial;
    public bool RenderEarth;
    public bool RenderIce;
    protected int mIceBaseVertex;
    protected int mIceNumVertices;
    protected int mIceStartIndex;
    protected int mIcePrimitiveCount;

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      iEffect1.Bones = this.mSkeleton;
      if (this.RenderEarth)
      {
        this.mMaterial.AssignToEffect(iEffect1);
        iEffect1.CommitChanges();
        iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      }
      if (!this.RenderIce)
        return;
      this.mIceMaterial.AssignToEffect(iEffect1);
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mIceBaseVertex, 0, this.mIceNumVertices, this.mIceStartIndex, this.mIcePrimitiveCount);
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      iEffect1.Bones = this.mSkeleton;
      if (this.RenderEarth)
      {
        this.mMaterial.AssignToEffect(iEffect1);
        iEffect1.CommitChanges();
        iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      }
      if (!this.RenderIce)
        return;
      this.mIceMaterial.AssignToEffect(iEffect1);
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mIceBaseVertex, 0, this.mIceNumVertices, this.mIceStartIndex, this.mIcePrimitiveCount);
    }

    public void SetIceMesh(ModelMeshPart iIceMeshPart)
    {
      this.mIceBaseVertex = iIceMeshPart.BaseVertex;
      this.mIceNumVertices = iIceMeshPart.NumVertices;
      this.mIceStartIndex = iIceMeshPart.StartIndex;
      this.mIcePrimitiveCount = iIceMeshPart.PrimitiveCount;
    }
  }

  protected class HaloAuraRenderData : 
    RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>,
    IPreRenderRenderer
  {
    public Vector3 Position;
    public Vector4 ColorTint;
    private Matrix mOrientation;
    public Matrix Ray1Transform;
    public Matrix Ray2Transform;
    private int mHaloBaseVertex;
    private int mHaloNumvertices;
    private int mHaloStartIndex;
    private int mHaloPrimitiveCount;
    private int mRays1BaseVertex;
    private int mRays1Numvertices;
    private int mRays1StartIndex;
    private int mRays1PrimitiveCount;
    private int mRays2BaseVertex;
    private int mRays2Numvertices;
    private int mRays2StartIndex;
    private int mRays2PrimitiveCount;

    public HaloAuraRenderData()
    {
      this.Ray1Transform = Matrix.Identity;
      this.Ray2Transform = Matrix.Identity;
    }

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      AdditiveEffect iEffect1 = iEffect as AdditiveEffect;
      iEffect1.GraphicsDevice.RenderState.DepthBias = 1f / 800f;
      this.mMaterial.ColorTint = this.ColorTint;
      this.mMaterial.ColorTint.W = 0.666f;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.World = this.mOrientation;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mHaloBaseVertex, 0, this.mHaloNumvertices, this.mHaloStartIndex, this.mHaloPrimitiveCount);
      iEffect1.GraphicsDevice.RenderState.DepthBias = 0.0f;
    }

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      Vector3 result1 = Vector3.Up;
      Vector3 position = this.Position;
      Vector3 result2;
      Vector3.Subtract(ref iCameraPosition, ref position, out result2);
      result2.Normalize();
      Vector3 result3;
      Vector3.Cross(ref result1, ref result2, out result3);
      result3.Normalize();
      Vector3.Cross(ref result2, ref result3, out result1);
      this.mOrientation.Forward = result2;
      this.mOrientation.Right = result3;
      this.mOrientation.Up = result1;
      this.mOrientation.M44 = 1f;
      this.mOrientation.Translation = this.Position;
    }

    public void AssignParts(ModelMeshPart iHalo, ModelMeshPart iRays1, ModelMeshPart iRays2)
    {
      this.mHaloBaseVertex = iHalo.BaseVertex;
      this.mHaloNumvertices = iHalo.NumVertices;
      this.mHaloStartIndex = iHalo.StartIndex;
      this.mHaloPrimitiveCount = iHalo.PrimitiveCount;
      this.mRays1BaseVertex = iRays1.BaseVertex;
      this.mRays1Numvertices = iRays1.NumVertices;
      this.mRays1StartIndex = iRays1.StartIndex;
      this.mRays1PrimitiveCount = iRays1.PrimitiveCount;
      this.mRays2BaseVertex = iRays2.BaseVertex;
      this.mRays2Numvertices = iRays2.NumVertices;
      this.mRays2StartIndex = iRays2.StartIndex;
      this.mRays2PrimitiveCount = iRays2.PrimitiveCount;
    }
  }

  protected class NormalDistortionRenderData : IPostEffect
  {
    protected InvisibilityEffect mEffect;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    public float mDamage;
    protected VertexBuffer mVertexBuffer;
    protected IndexBuffer mIndexBuffer;
    public Matrix[] mSkeleton;
    protected int mVerticesHash;
    public Matrix mCubeMapRotation;
    public float mWet;
    public bool mBurning;
    public bool mMuddy;
    public float mCold;
    public float mBlack;
    public float mBloated;
    protected bool mMeshDirty = true;
    public bool mIsVisible;
    protected Character mOwner;

    public NormalDistortionRenderData(InvisibilityEffect iEffect) => this.mEffect = iEffect;

    public bool MeshDirty => this.mMeshDirty;

    public int ZIndex => 1500;

    public void Draw(
      float iDeltaTime,
      ref Vector2 iPixelSize,
      ref Matrix iViewMatrix,
      ref Matrix iProjectionMatrix,
      Texture2D iCandidate,
      Texture2D iDepthMap,
      Texture2D iNormalMap)
    {
      this.mEffect.PixelSize = iPixelSize;
      this.mEffect.Distortion = -0.4f;
      this.mEffect.Bones = this.mSkeleton;
      this.mEffect.SourceTexture = iCandidate;
      this.mEffect.DepthTexture = iDepthMap;
      this.mEffect.Bloat = -0.05f;
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mEffect.GraphicsDevice.Indices = this.mIndexBuffer;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      this.mEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
    {
      this.mMeshDirty = false;
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
    }
  }

  protected class HighlightRenderData : IRenderableAdditiveObject
  {
    public BoundingSphere mBoundingSphere;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    protected VertexBuffer mVertexBuffer;
    protected IndexBuffer mIndexBuffer;
    public Matrix[] mSkeleton;
    public SkinnedModelDeferredAdvancedMaterial mMaterial;
    public SkinnedModelDeferredEffect.Technique mTechnique;
    protected int mVerticesHash;
    protected bool mMeshDirty = true;

    public int Effect => SkinnedModelDeferredEffect.TYPEHASH;

    public int Technique => (int) this.mTechnique;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public int VerticesHashCode => this.mVerticesHash;

    public int VertexStride => this.mVertexStride;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.DiffuseColor = new Vector3(1f, 1f, 1f);
      iEffect1.FresnelPower = 4f;
      iEffect1.Bones = this.mSkeleton;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      iEffect1.Colorize = new Vector4();
      iEffect1.DiffuseColor = new Vector3(1f);
    }

    public bool MeshDirty => this.mMeshDirty;

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
    {
      this.mMeshDirty = false;
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
      this.mTechnique = SkinnedModelDeferredEffect.Technique.AdditiveFresnel;
    }
  }

  protected class RenderData : IRenderableObject, IRenderableAdditiveObject
  {
    public BoundingSphere mBoundingSphere;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    protected VertexBuffer mVertexBuffer;
    protected IndexBuffer mIndexBuffer;
    public Matrix[] mSkeleton;
    public SkinnedModelDeferredAdvancedMaterial mMaterial;
    public SkinnedModelDeferredEffect.Technique mTechnique;
    protected int mVerticesHash;
    protected bool mMeshDirty = true;
    protected Character mOwner;

    public RenderData(Character iOwner)
    {
      this.mOwner = iOwner;
      this.mSkeleton = new Matrix[80 /*0x50*/];
    }

    public bool MeshDirty => this.mMeshDirty;

    public int Effect => SkinnedModelDeferredEffect.TYPEHASH;

    public int DepthTechnique => 3;

    public int Technique => (int) this.mTechnique;

    public int ShadowTechnique => 4;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.Bones = this.mSkeleton;
      if (this.mTechnique == SkinnedModelDeferredEffect.Technique.Additive)
        iEffect1.Colorize = new Vector4(Character.ColdColor, 1f);
      iEffect1.GraphicsDevice.RenderState.ReferenceStencil = 2;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      iEffect1.Bloat = 0.0f;
      iEffect1.GraphicsDevice.RenderState.ReferenceStencil = 0;
      this.mOwner.mLastDraw = 0.0f;
      iEffect1.CubeMapEnabled = false;
      iEffect1.Colorize = new Vector4();
    }

    public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      if (this.mTechnique == SkinnedModelDeferredEffect.Technique.Additive)
        return;
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignOpacityToEffect(iEffect1);
      iEffect1.Bones = this.mSkeleton;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      iEffect1.Bloat = 0.0f;
      this.mOwner.mLastDraw = 0.0f;
    }

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      ref SkinnedModelDeferredBasicMaterial iBasicMaterial,
      SkinnedModelDeferredEffect.Technique iTechnique)
    {
      this.mMeshDirty = false;
      this.mMaterial.CopyFrom(ref iBasicMaterial);
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
      this.mTechnique = iTechnique;
    }
  }

  public struct PointLightHolder
  {
    public bool Enabled;
    public bool ContainsLight;
    public string JointName;
    public BindJoint Joint;
    public float Radius;
    public Vector3 DiffuseColor;
    public Vector3 AmbientColor;
    public float SpecularAmount;
    public LightVariationType VariationType;
    public float VariationAmount;
    public float VariationSpeed;
  }

  public enum SelfShieldType : byte
  {
    None,
    Shield,
    Earth,
    Ice,
    IcedEarth,
  }

  public struct SelfShield
  {
    public static readonly int SELF_SHIELD = "spell_shield_self".GetHashCodeCustom();
    public static readonly int SELF_SHIELD_EARTH = "spell_shield_self_earth".GetHashCodeCustom();
    public static readonly int SHIELD_SHIELD_ICE = "spell_shield_self_ice".GetHashCodeCustom();
    public static readonly int PURE_SHIELD_HIT_EFFECT = "pure_self_shield_hit".GetHashCodeCustom();
    public static readonly int EARTH_SHIELD_HIT_EFFECT = "earth_self_shield_hit".GetHashCodeCustom();
    public static readonly int ICE_SHIELD_HIT_EFFECT = "ice_self_shield_hit".GetHashCodeCustom();
    internal Character.SelfShieldType mShieldType;
    internal float mMagnitude;
    private readonly float mSelfShieldMaxHealth;
    internal float mSelfShieldHealth;
    internal Spell mSpell;
    private double mTimeStamp;
    private Vector4 mHealthColor;
    public float mTimeSinceDamage;
    public Vector2 mNoiseOffset0;
    public Vector2 mNoiseOffset1;
    public Vector2 mNoiseOffset2;

    public SelfShield(Character iOwner, Spell iSpell)
    {
      this.mShieldType = (double) iSpell[Elements.Earth] <= 0.0 ? ((double) iSpell[Elements.Ice] <= 0.0 ? Character.SelfShieldType.Shield : Character.SelfShieldType.Ice) : ((double) iSpell[Elements.Ice] <= 0.0 ? Character.SelfShieldType.Earth : Character.SelfShieldType.IcedEarth);
      this.mSelfShieldMaxHealth = this.mSelfShieldHealth = 0.0f;
      this.mTimeStamp = iOwner.PlayState.PlayTime;
      this.mSpell = iSpell;
      this.mMagnitude = Math.Max(1f, iSpell[Elements.Ice] + iSpell[Elements.Earth]);
      this.mNoiseOffset0 = new Vector2();
      this.mNoiseOffset1 = new Vector2();
      this.mNoiseOffset2 = new Vector2();
      this.mTimeSinceDamage = 1f;
      switch (this.mShieldType)
      {
        case Character.SelfShieldType.Shield:
          this.mSelfShieldMaxHealth = this.mSelfShieldHealth = 500f;
          AudioManager.Instance.PlayCue(Banks.Spells, Character.SelfShield.SELF_SHIELD, iOwner.AudioEmitter);
          iOwner.mShieldSkinRenderData[0].SetMeshDirty();
          iOwner.mShieldSkinRenderData[1].SetMeshDirty();
          iOwner.mShieldSkinRenderData[2].SetMeshDirty();
          break;
        case Character.SelfShieldType.Earth:
        case Character.SelfShieldType.Ice:
        case Character.SelfShieldType.IcedEarth:
          iOwner.mBody.Mass = 1001f;
          for (int index = 0; index < 3; ++index)
          {
            iOwner.mBarrierSkinRenderData[index].RenderEarth = false;
            iOwner.mBarrierSkinRenderData[index].RenderIce = false;
          }
          if ((double) iSpell[Elements.Ice] > 0.0)
          {
            Magicka.GameLogic.Damage iDamage = new Magicka.GameLogic.Damage(AttackProperties.Damage | AttackProperties.Piercing, Elements.Earth, Defines.SPELL_DAMAGE_BARRIER_ICE, this.mMagnitude);
            Vector3 position = iOwner.Position;
            int num = (int) Helper.CircleDamage(iOwner.mPlayState, (Entity) iOwner, this.mTimeStamp, (Entity) iOwner, ref position, 3f, ref iDamage);
            AudioManager.Instance.PlayCue(Banks.Spells, Character.SelfShield.SHIELD_SHIELD_ICE, iOwner.AudioEmitter);
            this.mSelfShieldMaxHealth = this.mSelfShieldHealth = 900f;
          }
          if ((double) iSpell[Elements.Earth] > 0.0)
          {
            AudioManager.Instance.PlayCue(Banks.Spells, Character.SelfShield.SELF_SHIELD_EARTH, iOwner.AudioEmitter);
            this.mSelfShieldMaxHealth = this.mSelfShieldHealth = 1500f;
            break;
          }
          break;
        default:
          throw new InvalidOperationException("New shields must be of type Shield, Earth or Ice");
      }
      Vector3 color = iSpell.GetColor();
      this.mHealthColor.X = color.X;
      this.mHealthColor.Y = color.Y;
      this.mHealthColor.Z = color.Z;
      this.mHealthColor.W = 1f;
    }

    public void Update(Character iOwner, float iDeltaTime)
    {
      this.mTimeSinceDamage += iDeltaTime;
      if ((double) this.mSelfShieldHealth > (double) this.mSelfShieldMaxHealth)
        this.mSelfShieldHealth = this.mSelfShieldMaxHealth;
      switch (this.mShieldType)
      {
        case Character.SelfShieldType.Shield:
          float num = iDeltaTime * (float) (0.5 * ((double) this.mSelfShieldMaxHealth - (double) this.mSelfShieldHealth) / (double) this.mSelfShieldMaxHealth + 0.05000000074505806);
          this.mNoiseOffset2.Y -= num;
          this.mNoiseOffset0.Y += 0.3f * num;
          this.mNoiseOffset0.X -= 0.7f * num;
          this.mNoiseOffset1.Y += 0.7f * num;
          this.mNoiseOffset1.X += 0.4f * num;
          this.mSelfShieldHealth -= iDeltaTime * 100f;
          break;
        case Character.SelfShieldType.Earth:
        case Character.SelfShieldType.Ice:
        case Character.SelfShieldType.IcedEarth:
          iOwner.CharacterBody.SpeedMultiplier = (float) (0.89999997615814209 - ((double) this.mSpell[Elements.Earth] * 0.075000002980232239 + (double) this.mSpell[Elements.Ice] * 0.02500000037252903));
          break;
      }
      Vector3 position = iOwner.Position;
      position.Y -= iOwner.Capsule.Length * 0.5f + iOwner.Capsule.Radius;
      Healthbars.Instance.AddHealthBar(position, this.mSelfShieldHealth / this.mSelfShieldMaxHealth, iOwner.mRadius, 1f, this.mTimeSinceDamage, false, new Vector4?(this.mHealthColor), new Vector2?(new Vector2()
      {
        Y = (float) (6 * iOwner.mNumberOfHealtBars)
      }));
      if ((double) this.mSelfShieldHealth > 0.0)
        return;
      this.Remove(iOwner);
    }

    internal DamageResult Damage(ref float iDamage, Elements iElement)
    {
      this.mTimeSinceDamage = 0.0f;
      DamageResult damageResult = DamageResult.None;
      switch (this.mShieldType)
      {
        case Character.SelfShieldType.Shield:
          if ((double) iDamage >= 0.0 || (iElement & Elements.Shield) == Elements.Shield)
            this.mSelfShieldHealth -= iDamage;
          damageResult = DamageResult.Deflected;
          break;
        case Character.SelfShieldType.Earth:
        case Character.SelfShieldType.Ice:
        case Character.SelfShieldType.IcedEarth:
          if ((double) iDamage < 0.0)
          {
            iDamage = 0.0f;
            break;
          }
          if ((iElement & Elements.Fire) == Elements.Fire)
            iDamage *= 1f + this.mSpell[Elements.Ice];
          this.mSelfShieldHealth -= iDamage;
          break;
      }
      return damageResult;
    }

    private void ModifyDamage(ref Magicka.GameLogic.Damage iDamage)
    {
      if (this.mShieldType == Character.SelfShieldType.Earth)
        iDamage.Amount -= 10f;
      else
        iDamage.Amount -= 70f;
      float num = (float) (1.0 - ((double) this.mSpell[Elements.Ice] * 0.05000000074505806 + (double) this.mSpell[Elements.Earth] * 0.15000000596046448));
      iDamage.Amount *= num;
    }

    internal DamageResult Damage(
      Character iOwner,
      ref Magicka.GameLogic.Damage iDamage,
      Entity iAttacker,
      Vector3 iAttackPosition,
      Defines.DamageFeatures iFeatures)
    {
      bool flag1 = (iDamage.Element & Elements.PhysicalElements) != Elements.None;
      bool flag2 = (iDamage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage;
      bool flag3 = (iDamage.AttackProperty & AttackProperties.Status) == AttackProperties.Status;
      bool flag4 = (iDamage.AttackProperty & AttackProperties.Knockback) != (AttackProperties) 0;
      if (this.mShieldType == Character.SelfShieldType.Shield)
      {
        if (flag2 | flag3)
        {
          if (Defines.FeatureDamage(iFeatures) && (double) iDamage.Amount * (double) iDamage.Magnitude >= 0.0 && NetworkManager.Instance.State != NetworkState.Client)
            this.mSelfShieldHealth -= iDamage.Amount * iDamage.Magnitude;
          this.mTimeSinceDamage = 0.0f;
          iDamage.AttackProperty = (AttackProperties) 0;
          if (flag1)
          {
            iDamage.AttackProperty = AttackProperties.Knockback;
            iDamage.Magnitude = 0.6980619f;
          }
          if (flag2)
          {
            Vector3 iPosition = iAttackPosition;
            Vector3 result = iOwner.Position;
            Vector3.Subtract(ref iPosition, ref result, out result);
            result.X += 0.1f;
            result.Normalize();
            EffectManager.Instance.StartEffect(Character.SelfShield.PURE_SHIELD_HIT_EFFECT, ref iPosition, ref result, out VisualEffectReference _);
          }
        }
        return DamageResult.Deflected;
      }
      if (flag2)
      {
        if (flag1)
          this.ModifyDamage(ref iDamage);
        float num = iDamage.Amount * iDamage.Magnitude - this.mSelfShieldHealth;
        if (Defines.FeatureDamage(iFeatures) && (double) iDamage.Amount * (double) iDamage.Magnitude >= 0.0 && NetworkManager.Instance.State != NetworkState.Client)
          this.mSelfShieldHealth -= iDamage.Amount * iDamage.Magnitude;
        this.mTimeSinceDamage = 0.0f;
        if ((double) this.mSpell[Elements.Earth] > 0.0)
        {
          Vector3 iPosition = iAttackPosition;
          Vector3 result = iOwner.Position;
          Vector3.Subtract(ref iPosition, ref result, out result);
          result.X += 0.1f;
          result.Normalize();
          EffectManager.Instance.StartEffect(Character.SelfShield.EARTH_SHIELD_HIT_EFFECT, ref iPosition, ref result, out VisualEffectReference _);
        }
        if ((double) this.mSpell[Elements.Ice] > 0.0)
        {
          Vector3 iPosition = iAttackPosition;
          Vector3 result = iOwner.Position;
          Vector3.Subtract(ref iPosition, ref result, out result);
          result.X += 0.1f;
          result.Normalize();
          EffectManager.Instance.StartEffect(Character.SelfShield.ICE_SHIELD_HIT_EFFECT, ref iPosition, ref result, out VisualEffectReference _);
        }
        if ((double) num > 0.0)
        {
          iDamage.Amount = num;
          return DamageResult.Hit;
        }
        iDamage.Amount = 0.0f;
        iDamage.AttackProperty = (AttackProperties) 0;
        return DamageResult.Deflected;
      }
      if (flag4)
      {
        iDamage.Amount = 0.0f;
        iDamage.Magnitude = 0.0f;
      }
      return DamageResult.None;
    }

    internal void Remove(Character iOwner)
    {
      iOwner.Body.Mass = iOwner.mTemplateMass;
      this.mShieldType = Character.SelfShieldType.None;
      Matrix identity = Matrix.Identity with
      {
        Translation = iOwner.Position
      };
      VisualEffectReference oRef;
      if ((double) this.mSpell.EarthMagnitude > 0.0)
        EffectManager.Instance.StartEffect(Barrier.Earth_Barrier_Death_Effect_Hash, ref identity, out oRef);
      if ((double) this.mSpell.IceMagnitude > 0.0)
        EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref identity, out oRef);
      iOwner.ClearAura();
    }

    public bool Solid
    {
      get
      {
        return this.mShieldType == Character.SelfShieldType.Ice || this.mShieldType == Character.SelfShieldType.Earth || this.mShieldType == Character.SelfShieldType.IcedEarth;
      }
    }

    internal bool Active => this.mShieldType != Character.SelfShieldType.None;
  }

  public enum DamageAssistance : byte
  {
    None,
    Damaged,
    Killed,
  }
}
