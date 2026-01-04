// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Jormungandr
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Jormungandr : BossStatusEffected, IBoss
{
  private const float NETWORK_UPDATE_PERIOD = 0.0333333351f;
  private const float MAXHITPOINTS = 10000f;
  private const float COLLISIONDAMAGE = 750f;
  private const float HITTOLERANCE = 250f;
  private static float MAX_WARNINGTIME = 2.5f;
  private static float MIN_WARNINGTIME = 0.75f;
  private static float WARNINGTIME;
  private static readonly int[] JORMUNGANDREFFECTS = new int[6]
  {
    "jormungandr_dirt_splash".GetHashCodeCustom(),
    "jormungandr_dirt_tremor".GetHashCodeCustom(),
    "jormungandr_dirt_splash_tremor".GetHashCodeCustom(),
    "jormungandr_spit_spray".GetHashCodeCustom(),
    "jormungandr_spit_splash".GetHashCodeCustom(),
    "jormungandr_spit_projectile".GetHashCodeCustom()
  };
  private static readonly int BLOOD_EFFECT = Gib.GORE_SPLASH_EFFECTS[0];
  private static readonly float TAIL_WHIP = 0.887096763f;
  private static readonly int[] JORMUNGANDRSOUNDS = new int[15]
  {
    "boss_jormungandr_death".GetHashCodeCustom(),
    "boss_jormungandr_wakeup".GetHashCodeCustom(),
    "boss_jormungandr_dive".GetHashCodeCustom(),
    "boss_jormungandr_unburrow".GetHashCodeCustom(),
    "boss_jormungandr_leap".GetHashCodeCustom(),
    "boss_jormungandr_rise".GetHashCodeCustom(),
    "boss_jormungandr_spit".GetHashCodeCustom(),
    "boss_jormungandr_bite".GetHashCodeCustom(),
    "boss_jormungandr_pain".GetHashCodeCustom(),
    "boss_jormungandr_prespit".GetHashCodeCustom(),
    "boss_jormungandr_prebite".GetHashCodeCustom(),
    "boss_jormungandr_leapdive".GetHashCodeCustom(),
    "boss_jormungandr_hitobject".GetHashCodeCustom(),
    "boss_jormungandr_whip".GetHashCodeCustom(),
    "boss_jormungandr_prowl".GetHashCodeCustom()
  };
  protected long mLastNetworkUpdate;
  protected float mNetworkUpdateTimer;
  protected VisualEffectReference mEarthSplashReference;
  protected VisualEffectReference mEarthTremorReference;
  protected Cue mProwlCue;
  protected static Random mRandom = new Random();
  protected AnimationClip[] mAnimationClips;
  protected AnimationController mAnimationController;
  protected SkinnedModel mModel;
  protected IBossState<Jormungandr> mGlobalState;
  protected IBossState<Jormungandr> mCurrentState;
  protected IBossState<Jormungandr> mLastState;
  protected Jormungandr.RenderData[] mRenderData;
  private bool mInitialized;
  private PlayState mPlayState;
  protected ConditionCollection mSpitConditions;
  protected Jormungandr.DamageState mDamageState;
  protected bool mMidLeap;
  protected bool mDraw;
  protected bool mUpdateAnimation;
  protected bool mIsHit;
  protected bool mDirtSpawned;
  protected bool mNoEarthCollisionEffect;
  protected Quaternion mSavedRotation;
  protected Vector3 mSavedPosition;
  protected float mIdleTimer;
  protected int mAttackCount;
  protected Vector3 mPosition;
  protected Vector3 mDirection = Vector3.Right;
  protected Matrix mOrientation;
  protected int mHeadJointIndex;
  protected Matrix mHeadBindPose;
  protected int mNeckJointIndex;
  protected Matrix mNeckBindPose;
  protected int[] mSpineIndices;
  protected Matrix[] mSpineBindPose;
  protected JormungandrCollisionZone mSpine;
  protected BossDamageZone mDamageZone;
  protected Magicka.GameLogic.Entities.Character mTarget;
  protected DamageCollection5 mLeapDamage;
  protected DamageCollection5 mEmergeDamage;
  protected DamageCollection5 mBiteDamage;
  protected DamageCollection5 mSpitDamage;
  protected HitList mHitlist = new HitList(8);
  protected HitList mSpineHitlist = new HitList(8);
  protected AudioEmitter mAudioEmitter;
  private bool mDead;
  protected float mDamageFlashTimer;
  protected Jormungandr.Animations mDeathAnimation;
  protected VisualEffectReference mFrontTremor;
  protected VisualEffectReference mBackTremor;
  private SkinnedModelDeferredAdvancedMaterial mMaterial;
  private TextureCube mIceCubeMap;
  private TextureCube mIceCubeNormalMap;

  public Jormungandr(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mGlobalState = (IBossState<Jormungandr>) Jormungandr.GlobalState.Instance;
    this.mCurrentState = (IBossState<Jormungandr>) Jormungandr.IntroState.Instance;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mModel = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Jormungandr/Jormungandr");
      this.mIceCubeMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube");
      this.mIceCubeNormalMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
    }
    this.mAnimationController = new AnimationController();
    this.mAnimationController.Skeleton = this.mModel.SkeletonBones;
    this.mAnimationClips = new AnimationClip[13];
    this.mAnimationClips[1] = this.mModel.AnimationClips["intro"];
    this.mAnimationClips[2] = this.mModel.AnimationClips["dive_attack"];
    this.mAnimationClips[3] = this.mModel.AnimationClips["dive_recoil"];
    this.mAnimationClips[4] = this.mModel.AnimationClips["emerge"];
    this.mAnimationClips[5] = this.mModel.AnimationClips["submerge"];
    this.mAnimationClips[6] = this.mModel.AnimationClips["idle"];
    this.mAnimationClips[7] = this.mModel.AnimationClips["spit"];
    this.mAnimationClips[8] = this.mModel.AnimationClips["venom"];
    this.mAnimationClips[9] = this.mModel.AnimationClips["attack"];
    this.mAnimationClips[10] = this.mModel.AnimationClips["hit"];
    this.mAnimationClips[11] = this.mModel.AnimationClips["die"];
    this.mAnimationClips[12] = this.mModel.AnimationClips["die_above"];
    this.mSpineIndices = new int[26];
    this.mSpineBindPose = new Matrix[26];
    Matrix result1;
    Matrix.CreateRotationY(3.14159274f, out result1);
    for (int index1 = 0; index1 < this.mModel.SkeletonBones.Count; ++index1)
    {
      SkinnedModelBone skeletonBone = this.mModel.SkeletonBones[index1];
      Matrix result2;
      if (skeletonBone.Name.Equals("HeadBase", StringComparison.OrdinalIgnoreCase))
      {
        this.mHeadJointIndex = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mHeadBindPose);
      }
      for (int index2 = 11; index2 > 0; --index2)
      {
        string str = $"back{index2:00}";
        if (skeletonBone.Name.Equals(str, StringComparison.OrdinalIgnoreCase))
        {
          this.mSpineIndices[11 - index2] = (int) skeletonBone.Index;
          result2 = skeletonBone.InverseBindPoseTransform;
          Matrix.Multiply(ref result2, ref result1, out result2);
          Matrix.Invert(ref result2, out this.mSpineBindPose[11 - index2]);
        }
      }
      if (skeletonBone.Name.Equals("center", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndices[11] = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mSpineBindPose[11]);
      }
      for (int index3 = 1; index3 <= 12; ++index3)
      {
        string str = $"front{index3:00}";
        if (skeletonBone.Name.Equals(str, StringComparison.OrdinalIgnoreCase))
        {
          this.mSpineIndices[index3 + 11] = (int) skeletonBone.Index;
          result2 = skeletonBone.InverseBindPoseTransform;
          Matrix.Multiply(ref result2, ref result1, out result2);
          Matrix.Invert(ref result2, out this.mSpineBindPose[index3 + 11]);
        }
      }
      if (skeletonBone.Name.Equals("NeckBase", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndices[24] = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mSpineBindPose[24]);
      }
      if (skeletonBone.Name.Equals("NeckMid", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndices[25] = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mSpineBindPose[25]);
      }
      if (skeletonBone.Name.Equals("NeckEnd", StringComparison.OrdinalIgnoreCase))
      {
        this.mNeckJointIndex = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mNeckBindPose);
      }
    }
    Primitive[] primitiveArray = new Primitive[25];
    for (int index = 0; index < primitiveArray.Length; ++index)
    {
      float radius = System.Math.Min((float) (0.33300000429153442 + (double) index * 0.20000000298023224), 1f);
      float length = Vector3.Distance(this.mSpineBindPose[index].Translation, this.mSpineBindPose[index + 1].Translation);
      primitiveArray[index] = (Primitive) new Capsule(new Vector3(), Matrix.Identity, radius, length);
    }
    this.mSpine = new JormungandrCollisionZone(iPlayState, (IBoss) this, primitiveArray);
    this.mSpine.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnSpineCollision);
    this.mDamageZone = new BossDamageZone(iPlayState, (IBoss) this, 1, 1.1f);
    this.mDamageZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
    ModelMesh mesh = this.mModel.Model.Meshes[0];
    this.mRenderData = new Jormungandr.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      Jormungandr.RenderData renderData = new Jormungandr.RenderData();
      this.mRenderData[index] = renderData;
      renderData.SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
      renderData.mMaterial = this.mMaterial;
    }
    this.mLeapDamage = new DamageCollection5();
    this.mLeapDamage.A.Amount = 150f;
    this.mLeapDamage.A.Magnitude = 1f;
    this.mLeapDamage.A.AttackProperty = AttackProperties.Damage;
    this.mLeapDamage.A.Element = Elements.Earth;
    this.mLeapDamage.B.Amount = 30f;
    this.mLeapDamage.B.Magnitude = 1f;
    this.mLeapDamage.B.AttackProperty = AttackProperties.Status;
    this.mLeapDamage.C.Element = Elements.Poison;
    this.mLeapDamage.C.Amount = 70f;
    this.mLeapDamage.C.Magnitude = 2f;
    this.mLeapDamage.C.AttackProperty = AttackProperties.Pushed;
    this.mLeapDamage.C.Element = Elements.Earth;
    this.mEmergeDamage = new DamageCollection5();
    this.mEmergeDamage.A.Amount = 150f;
    this.mEmergeDamage.A.Magnitude = 2f;
    this.mEmergeDamage.A.AttackProperty = AttackProperties.Knockback;
    this.mEmergeDamage.A.Element = Elements.Earth;
    this.mEmergeDamage.B.Amount = 300f;
    this.mEmergeDamage.B.Magnitude = 2f;
    this.mEmergeDamage.B.AttackProperty = AttackProperties.Damage;
    this.mEmergeDamage.B.Element = Elements.Earth;
    this.mBiteDamage = new DamageCollection5();
    this.mBiteDamage.A.Amount = 325f;
    this.mBiteDamage.A.Magnitude = 1f;
    this.mBiteDamage.A.AttackProperty = AttackProperties.Damage;
    this.mBiteDamage.A.Element = Elements.Earth;
    this.mBiteDamage.B.Amount = 30f;
    this.mBiteDamage.B.Magnitude = 2f;
    this.mBiteDamage.B.AttackProperty = AttackProperties.Status;
    this.mBiteDamage.B.Element = Elements.Poison;
    this.mBiteDamage.C.Amount = 70f;
    this.mBiteDamage.C.Magnitude = 2f;
    this.mBiteDamage.C.AttackProperty = AttackProperties.Knockback;
    this.mBiteDamage.C.Element = Elements.Earth;
    this.mSpitDamage = new DamageCollection5();
    this.mSpitDamage.A.Amount = 75f;
    this.mSpitDamage.A.Magnitude = 4f;
    this.mSpitDamage.A.AttackProperty = AttackProperties.Status;
    this.mSpitDamage.A.Element = Elements.Poison;
    this.mSpitDamage.B.Amount = 70f;
    this.mSpitDamage.B.Magnitude = 1f;
    this.mSpitDamage.B.AttackProperty = AttackProperties.Pushed;
    this.mSpitDamage.B.Element = Elements.Earth;
    this.mSpitConditions = new ConditionCollection();
    this.mSpitConditions[0].Condition.EventConditionType = EventConditionType.Default;
    this.mSpitConditions[0].Condition.Repeat = true;
    this.mSpitConditions[0].Add(new EventStorage(new PlayEffectEvent(Jormungandr.JORMUNGANDREFFECTS[5], true)));
    this.mSpitConditions[0].Add(new EventStorage(new PlaySoundEvent(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[6])));
    this.mSpitConditions[1].Condition.EventConditionType = EventConditionType.Hit;
    this.mSpitConditions[1].Add(new EventStorage(new PlayEffectEvent(Jormungandr.JORMUNGANDREFFECTS[4])));
    this.mSpitConditions[1].Add(new EventStorage(new PlaySoundEvent(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[6])));
    this.mSpitConditions[1].Add(new EventStorage(new DamageEvent(this.mSpitDamage.A)));
    this.mSpitConditions[1].Add(new EventStorage(new RemoveEvent()));
    this.mSpitConditions[2].Condition.EventConditionType = EventConditionType.Collision;
    this.mSpitConditions[2].Add(new EventStorage(new PlaySoundEvent(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[6])));
    this.mSpitConditions[2].Add(new EventStorage(new PlayEffectEvent(Jormungandr.JORMUNGANDREFFECTS[4])));
    this.mSpitConditions[2].Add(new EventStorage(new SplashEvent(this.mSpitDamage.A, 2f)));
    this.mSpitConditions[2].Add(new EventStorage(new SplashEvent(this.mSpitDamage.B, 2f)));
    this.mSpitConditions[2].Add(new EventStorage(new RemoveEvent()));
    this.mDirtSpawned = false;
    this.mNoEarthCollisionEffect = false;
    this.mResistances = new Resistance[11];
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements elements = Defines.ElementFromIndex(iIndex);
      this.mResistances[iIndex].ResistanceAgainst = elements;
      if (elements == Elements.Ice | elements == Elements.Earth)
      {
        this.mResistances[iIndex].Multiplier = 0.75f;
        this.mResistances[iIndex].Modifier = 0.0f;
      }
      else
      {
        this.mResistances[iIndex].Multiplier = 1f;
        this.mResistances[iIndex].Modifier = 0.0f;
      }
    }
    this.mAudioEmitter = new AudioEmitter();
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mMaxHitPoints = 10000f;
    this.mHitPoints = 10000f;
    this.mCurrentState = (IBossState<Jormungandr>) Jormungandr.SleepIntroState.Instance;
    this.mCurrentState.OnEnter(this);
    this.mOrientation = iOrientation;
    this.mPosition = iOrientation.Translation;
    this.mDirection = iOrientation.Forward;
    this.mDamageZone.Initialize();
    this.mDamageZone.Body.CollisionSkin.NonCollidables.Add(this.mSpine.Body.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mDamageZone);
    this.mSpine.Initialize();
    this.mSpine.Body.CollisionSkin.NonCollidables.Add(this.mDamageZone.Body.CollisionSkin);
    this.mSpine.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mSpine);
    for (int index = 0; index < this.mStatusEffects.Length; ++index)
    {
      this.mStatusEffects[index].Stop();
      this.mStatusEffects[index] = new StatusEffect();
    }
    this.mInitialized = true;
    this.mDead = false;
    this.mDamageFlashTimer = 0.0f;
    this.mDeathAnimation = Jormungandr.Animations.Die_Above;
    EffectManager.Instance.Stop(ref this.mEarthTremorReference);
    EffectManager.Instance.Stop(ref this.mEarthSplashReference);
  }

  public bool OnSpineCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner == null)
      return false;
    if (this.Dead || !(iSkin1.Owner.Tag is IDamageable tag) || tag is MissileEntity || this.mSpineHitlist.ContainsKey(tag.Handle))
      return true;
    this.mSpineHitlist.Add(tag.Handle, 0.25f);
    Segment seg = new Segment();
    seg.Origin = iSkin0.Owner.Position;
    seg.Delta = iSkin1.Owner.Position;
    Vector3.Subtract(ref seg.Delta, ref seg.Origin, out seg.Delta);
    Vector3 pos;
    iSkin1.SegmentIntersect(out float _, out pos, out Vector3 _, seg);
    switch (tag)
    {
      case Magicka.GameLogic.Entities.Character _:
        Magicka.GameLogic.Damage iDamage1 = new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, 60f, 1f);
        int num1 = (int) tag.Damage(iDamage1, (Magicka.GameLogic.Entities.Entity) this.mDamageZone, 0.0, pos);
        break;
      case Shield _:
      case Barrier _:
        Magicka.GameLogic.Damage iDamage2 = new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, 750f, 1f);
        int num2 = (int) tag.Damage(iDamage2, (Magicka.GameLogic.Entities.Entity) this.mDamageZone, 0.0, pos);
        break;
    }
    return true;
  }

  public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1 == this.mPlayState.Level.CurrentScene.CollisionSkin && !this.mNoEarthCollisionEffect && !this.mDirtSpawned)
    {
      if (this.mCurrentState is Jormungandr.BiteState)
      {
        Vector3 right = Vector3.Right;
        Vector3 oPosition;
        if (this.FrontBodyCollision(out oPosition) && !EffectManager.Instance.IsActive(ref this.mEarthSplashReference))
          EffectManager.Instance.StartEffect(Jormungandr.JORMUNGANDREFFECTS[0], ref oPosition, ref right, out this.mEarthSplashReference);
        return false;
      }
      if (this.mAnimationController.AnimationClip == this.mAnimationClips[2])
      {
        if (this.mMidLeap)
          AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[11]);
        else
          AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[4]);
      }
      EffectManager.Instance.Stop(ref this.mEarthTremorReference);
      this.StartEffect(out this.mEarthSplashReference, Jormungandr.JORMUNGANDREFFECTS[2]);
      this.mDirtSpawned = true;
      this.mPlayState.Camera.CameraShake(1f, 0.7f);
    }
    if (iSkin1.Owner == null || this.mDamageState == Jormungandr.DamageState.None || !(iSkin1.Owner.Tag is IDamageable tag))
      return false;
    if (this.mHitlist.ContainsKey(tag.Handle))
      return true;
    this.mHitlist.Add(tag.Handle, 0.25f);
    DamageCollection5 iDamage1;
    switch (this.mDamageState)
    {
      case Jormungandr.DamageState.Leap:
        iDamage1 = this.mLeapDamage;
        break;
      case Jormungandr.DamageState.Emerge:
        iDamage1 = this.mEmergeDamage;
        break;
      case Jormungandr.DamageState.Bite:
        iDamage1 = this.mBiteDamage;
        break;
      default:
        return false;
    }
    Segment seg = new Segment();
    seg.Origin = iSkin0.Owner.Position;
    seg.Delta = iSkin1.Owner.Position;
    Vector3.Subtract(ref seg.Delta, ref seg.Origin, out seg.Delta);
    Vector3 pos;
    iSkin1.SegmentIntersect(out float _, out pos, out Vector3 _, seg);
    switch (tag)
    {
      case Shield _:
      case Barrier _:
        if (tag is Barrier && !(tag as Barrier).Solid)
          return false;
        AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[12]);
        if (this.mAnimationController.AnimationClip != this.mAnimationClips[5] || this.mAnimationController.AnimationClip != this.mAnimationClips[4])
        {
          Magicka.GameLogic.Damage iDamage2 = new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth, (float) (int) ((double) tag.MaxHitPoints * 0.75), 1f);
          int num = (int) tag.Damage(iDamage2, (Magicka.GameLogic.Entities.Entity) this.mDamageZone, 0.0, pos);
          this.mIsHit = true;
          this.mHitPoints -= 750f;
        }
        else
          tag.Kill();
        return false;
      default:
        int num1 = (int) tag.Damage(iDamage1, (Magicka.GameLogic.Entities.Entity) this.mDamageZone, 0.0, pos);
        if (tag.Dead)
          tag.OverKill();
        return false;
    }
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (!this.mInitialized)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mNetworkUpdateTimer -= iDeltaTime;
      if ((double) this.mNetworkUpdateTimer <= 0.0)
      {
        this.mNetworkUpdateTimer = 0.0333333351f;
        this.NetworkUpdate();
      }
    }
    if (this.mCurrentState is Jormungandr.SleepIntroState && iFightStarted)
      this.ChangeState((IBossState<Jormungandr>) Jormungandr.IntroState.Instance);
    Jormungandr.WARNINGTIME = MathHelper.Lerp(Jormungandr.MIN_WARNINGTIME, Jormungandr.MAX_WARNINGTIME, this.mHitPoints / 10000f);
    this.mGlobalState.OnUpdate(iDeltaTime, this);
    this.mCurrentState.OnUpdate(iDeltaTime, this);
    Matrix orientation = this.GetOrientation();
    this.mAudioEmitter.Position = orientation.Translation;
    this.mAudioEmitter.Forward = orientation.Forward;
    this.mAudioEmitter.Up = orientation.Up;
    this.mAnimationController.Speed = 1f;
    if (this.HasStatus(StatusEffects.Frozen))
    {
      this.mRenderData[(int) iDataChannel].mMaterial.Bloat = 0.1f;
      this.mRenderData[(int) iDataChannel].mMaterial.EmissiveAmount = 3f;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularBias = 0.8f;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularPower = 20f;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapRotation = Matrix.Identity;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMap = this.mIceCubeMap;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeNormalMap = this.mIceCubeNormalMap;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapColor = Vector4.One;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapEnabled = true;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeNormalMapEnabled = true;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapColor.W = 1f - (float) System.Math.Pow(0.20000000298023224, (double) this.StatusMagnitude(StatusEffects.Frozen));
      this.mAnimationController.Speed = 0.0f;
    }
    else
    {
      if (this.HasStatus(StatusEffects.Cold))
        this.mAnimationController.Speed *= 0.5f;
      this.mRenderData[(int) iDataChannel].mMaterial.Bloat = 0.0f;
      this.mRenderData[(int) iDataChannel].mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularBias = this.mMaterial.SpecularBias;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularPower = this.mMaterial.SpecularPower;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapEnabled = false;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeNormalMapEnabled = false;
    }
    if (this.Dead)
    {
      this.mIdleTimer += iDeltaTime;
      if ((double) this.mIdleTimer > 1.0)
      {
        this.mSpineHitlist.Clear();
        this.mHitlist.Clear();
      }
    }
    else
    {
      this.mSpineHitlist.Update(iDeltaTime);
      this.mHitlist.Update(iDeltaTime);
      Jormungandr.WARNINGTIME = (float) (1.0 * ((double) this.mHitPoints / 10000.0) + 0.40000000596046448);
    }
    Vector3 result1;
    if (this.mUpdateAnimation)
    {
      this.mAnimationController.Update(iDeltaTime, ref this.mOrientation, true);
      this.mSpine.Body.Position = new Vector3();
      Transform iTransform = new Transform();
      iTransform.Position = this.mSpineBindPose[0].Translation;
      Vector3.Transform(ref iTransform.Position, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndices[0]], out iTransform.Position);
      for (int prim = 0; prim < 25; ++prim)
      {
        Vector3 result2 = this.mSpineBindPose[prim + 1].Translation;
        Vector3.Transform(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndices[prim + 1]], out result2);
        Vector3 result3;
        Vector3.Subtract(ref iTransform.Position, ref result2, out result3);
        result3.Normalize();
        Jormungandr.CreateVertebraeOrientation(ref result3, out iTransform.Orientation);
        iTransform.Position = result2;
        this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(prim).SetTransform(ref iTransform);
      }
      Transform identity = Transform.Identity;
      this.mSpine.Body.CollisionSkin.SetTransform(ref identity, ref identity);
      this.mSpine.Body.CollisionSkin.UpdateWorldBoundingBox();
      this.mSpine.Body.Transform = identity;
      result1 = this.mHeadBindPose.Translation;
      Vector3.Transform(ref result1, ref this.mAnimationController.SkinnedBoneTransforms[this.mHeadJointIndex], out result1);
      Vector3 result4 = this.mOrientation.Forward;
      Vector3.Multiply(ref result4, 0.6f, out result4);
      Vector3.Add(ref result1, ref result4, out result1);
    }
    else
    {
      result1 = new Vector3(0.0f, -100f, 0.0f);
      this.mSpine.Body.Position = result1;
    }
    this.mDamageZone.SetPosition(ref result1);
    if (this.mCurrentState is Jormungandr.UndergroundState | this.mCurrentState is Jormungandr.SleepIntroState)
    {
      EffectManager.Instance.Stop(ref this.mBackTremor);
      EffectManager.Instance.Stop(ref this.mFrontTremor);
    }
    else
    {
      Vector3 right = Vector3.Right;
      Vector3 oPosition;
      if (this.BackBodyCollision(out oPosition))
      {
        if (!EffectManager.Instance.UpdatePositionDirection(ref this.mBackTremor, ref oPosition, ref right))
          EffectManager.Instance.StartEffect(Jormungandr.JORMUNGANDREFFECTS[1], ref oPosition, ref right, out this.mBackTremor);
      }
      else if (EffectManager.Instance.IsActive(ref this.mBackTremor))
        EffectManager.Instance.Stop(ref this.mBackTremor);
      if (this.FrontBodyCollision(out oPosition) && !(this.mCurrentState is Jormungandr.IntroState) || this.mCurrentState is Jormungandr.IntroState && !this.mAnimationController.CrossFadeEnabled && (double) this.mAnimationController.Time > 2.0)
      {
        if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFrontTremor, ref oPosition, ref right))
          EffectManager.Instance.StartEffect(Jormungandr.JORMUNGANDREFFECTS[1], ref oPosition, ref right, out this.mFrontTremor);
      }
      else if (EffectManager.Instance.IsActive(ref this.mFrontTremor))
        EffectManager.Instance.Stop(ref this.mFrontTremor);
    }
    this.UpdateDamage(iDeltaTime);
    this.UpdateStatusEffects(iDeltaTime);
    this.mDamageFlashTimer = System.Math.Max(this.mDamageFlashTimer - iDeltaTime, 0.0f);
    if (!this.mDraw)
      return;
    Jormungandr.RenderData iObject = this.mRenderData[(int) iDataChannel];
    this.mAnimationController.SkinnedBoneTransforms.CopyTo((Array) iObject.mSkeleton, 0);
    iObject.Damage = (float) (1.0 - (double) this.mHitPoints / 10000.0);
    iObject.mBoundingSphere.Center = this.mPosition;
    iObject.mBoundingSphere.Radius = 20f;
    iObject.Flash = this.mDamageFlashTimer * 10f;
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
  }

  protected bool FindGround(ref Vector3 iPosition, out Vector3 iDirection)
  {
    Segment iSeg = new Segment();
    iSeg.Origin = iPosition;
    iSeg.Origin.Y += 10f;
    iSeg.Delta.Y = -30f;
    return this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out iPosition, out iDirection, iSeg);
  }

  protected void StartEffect(out VisualEffectReference iRef, int iHash)
  {
    Vector3 iPosition = this.mDamageZone.Body.Position;
    if ((double) iPosition.Y < -10.0)
      iPosition = this.mOrientation.Translation;
    if (this.FindGround(ref iPosition, out Vector3 _))
    {
      Vector3 right = Vector3.Right;
      EffectManager.Instance.StartEffect(iHash, ref iPosition, ref right, out iRef);
    }
    else
      iRef = new VisualEffectReference();
  }

  protected void Turn(ref Vector3 iNewDirection, float iTurnSpeed, float iDeltaTime)
  {
    if (this.HasStatus(StatusEffects.Frozen))
      return;
    if (this.HasStatus(StatusEffects.Cold))
      iTurnSpeed *= 0.5f;
    Matrix identity = Matrix.Identity;
    Vector3 up = Vector3.Up;
    Vector3 result1;
    Vector3.Cross(ref iNewDirection, ref up, out result1);
    identity.Forward = iNewDirection;
    identity.Up = up;
    identity.Right = result1;
    Quaternion result2;
    Quaternion.CreateFromRotationMatrix(ref this.mOrientation, out result2);
    Quaternion result3;
    Quaternion.CreateFromRotationMatrix(ref identity, out result3);
    Quaternion.Lerp(ref result2, ref result3, MathHelper.Clamp(iDeltaTime * iTurnSpeed, 0.0f, 1f), out result3);
    Vector3 translation = this.mOrientation.Translation;
    Matrix.CreateFromQuaternion(ref result3, out this.mOrientation);
    this.mOrientation.Translation = translation;
    this.mDirection = this.mOrientation.Forward;
  }

  protected unsafe void SelectTarget(Jormungandr.TargettingType iFlags)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Player[] players = Magicka.Game.Instance.Players;
    Vector3 mPosition = this.mPosition;
    switch (iFlags)
    {
      case Jormungandr.TargettingType.Distance:
        float num1 = float.MaxValue;
        for (int index = 0; index < players.Length; ++index)
        {
          Player player = players[index];
          if (player.Avatar != null && !player.Avatar.Dead)
          {
            Vector3 position = player.Avatar.Position;
            float result = 0.0f;
            Vector3.DistanceSquared(ref mPosition, ref position, out result);
            if ((double) result < (double) num1)
            {
              this.mTarget = (Magicka.GameLogic.Entities.Character) player.Avatar;
              num1 = result;
            }
          }
        }
        break;
      case Jormungandr.TargettingType.Angle:
        float num2 = float.MaxValue;
        for (int index = 0; index < players.Length; ++index)
        {
          Player player = players[index];
          if (player.Avatar != null && !player.Avatar.Dead)
          {
            Vector3 forward = this.mOrientation.Forward;
            Vector3 position = player.Avatar.Position;
            Vector3 result;
            Vector3.Subtract(ref position, ref this.mPosition, out result);
            result.Normalize();
            float num3 = MagickaMath.Angle(ref forward, ref result);
            if ((double) num3 < (double) num2)
            {
              this.mTarget = (Magicka.GameLogic.Entities.Character) player.Avatar;
              num2 = num3;
            }
          }
        }
        break;
      case Jormungandr.TargettingType.PlayerStrain:
        int num4 = 0;
        for (int index = 0; index < players.Length; ++index)
        {
          Player player = players[index];
          if (player.Avatar != null && !player.Avatar.Dead)
          {
            int count = player.Avatar.SpellQueue.Count;
            if (count > num4)
            {
              this.mTarget = (Magicka.GameLogic.Entities.Character) player.Avatar;
              num4 = count;
            }
          }
        }
        break;
      default:
        int num5 = Jormungandr.mRandom.Next(4);
        for (int index = 0; index < players.Length; ++index)
        {
          Player player = players[(index + num5) % 4];
          if (player.Avatar != null && !player.Avatar.Dead)
          {
            this.mTarget = (Magicka.GameLogic.Entities.Character) player.Avatar;
            break;
          }
        }
        break;
    }
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    Jormungandr.ChangeTargetMessage changeTargetMessage;
    changeTargetMessage.Target = this.mTarget == null ? ushort.MaxValue : this.mTarget.Handle;
    BossFight.Instance.SendMessage<Jormungandr.ChangeTargetMessage>((IBoss) this, (ushort) 2, (void*) &changeTargetMessage, true);
  }

  protected void ChangeState(IBossState<Jormungandr> iState)
  {
    if (this.mCurrentState == iState)
      return;
    this.mLastState = this.mCurrentState;
    this.mCurrentState.OnExit(this);
    this.mCurrentState = iState;
    this.mCurrentState.OnEnter(this);
  }

  protected static void CreateTransform(
    ref Vector3 iTranslation,
    ref Vector3 iDirection,
    out Matrix oOrientation)
  {
    Vector3 result1 = new Vector3();
    result1.Y = 1f;
    Vector3 result2;
    Vector3.Cross(ref iDirection, ref result1, out result2);
    result2.Normalize();
    Vector3.Cross(ref result2, ref iDirection, out result1);
    oOrientation = new Matrix();
    oOrientation.Forward = iDirection;
    oOrientation.Up = result1;
    oOrientation.Right = result2;
    oOrientation.Translation = iTranslation;
    oOrientation.M44 = 1f;
  }

  protected static void CreateVertebraeOrientation(ref Vector3 iForward, out Matrix oOrientation)
  {
    Vector3 result1 = new Vector3();
    if ((double) iForward.X < 9.9999999747524271E-07 && (double) iForward.Z < 9.9999999747524271E-07)
      result1.X = 1f;
    else
      result1.Y = 1f;
    Vector3 result2;
    Vector3.Cross(ref iForward, ref result1, out result2);
    result2.Normalize();
    Vector3.Cross(ref result2, ref iForward, out result1);
    oOrientation = new Matrix();
    oOrientation.Forward = iForward;
    oOrientation.Up = result1;
    oOrientation.Right = result2;
    oOrientation.M44 = 1f;
  }

  protected void TransformBones(int iStartIndex, ref Matrix iTransform)
  {
    Matrix.Multiply(ref this.mAnimationController.SkinnedBoneTransforms[iStartIndex], ref iTransform, out this.mAnimationController.SkinnedBoneTransforms[iStartIndex]);
    SkinnedModelBoneCollection children = this.mModel.SkeletonBones[iStartIndex].Children;
    for (int index = 0; index < children.Count; ++index)
      this.TransformBones((int) children[index].Index, ref iTransform);
  }

  private Matrix GetOrientation()
  {
    return this.mDamageZone.Body.Orientation with
    {
      Translation = this.mDamageZone.Body.Position
    };
  }

  public void DeInitialize()
  {
  }

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Magicka.GameLogic.Entities.Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if ((double) iDamage.Amount == 0.0 && (double) iDamage.Magnitude == 0.0 || !this.mDraw | this.Dead | iPartIndex != 1 || this.mAnimationController.AnimationClip == this.mAnimationClips[5])
      return DamageResult.None;
    float mHitPoints = this.mHitPoints;
    DamageResult damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
    if (((damageResult & DamageResult.Hit) == DamageResult.Hit | (damageResult & DamageResult.Damaged) == DamageResult.Damaged | (damageResult & DamageResult.Killed) == DamageResult.Killed) & (damageResult & DamageResult.Healed) == DamageResult.None)
    {
      this.mDamageFlashTimer = 0.1f;
      if ((double) mHitPoints - (double) this.mHitPoints >= 250.0)
        this.mIsHit = true;
    }
    return damageResult;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
    this.Damage(iDamage, iElement);
  }

  public void SetSlow(int iIndex) => throw new NotImplementedException();

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return false;
  }

  public float MaxHitPoints => 10000f;

  public float HitPoints => this.mHitPoints;

  private bool BackBodyCollision(out Vector3 oPosition)
  {
    Segment iSeg = new Segment();
    oPosition = new Vector3();
    iSeg.Origin = (this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(11) as Capsule).Position;
    for (int index = 0; index < 11; ++index)
    {
      Vector3 position = (this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(10 - index) as Capsule).Position;
      Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
      if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPosition, out Vector3 _, iSeg))
        return true;
      iSeg.Origin = position;
    }
    return false;
  }

  private bool FrontBodyCollision(out Vector3 oPosition)
  {
    Segment iSeg = new Segment();
    oPosition = new Vector3();
    iSeg.Origin = (this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(11) as Capsule).Position;
    for (int index = 0; index < 11; ++index)
    {
      Vector3 position = (this.mSpine.Body.CollisionSkin.GetPrimitiveLocal(12 + index) as Capsule).Position;
      Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
      if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPosition, out Vector3 _, iSeg))
        return true;
      iSeg.Origin = position;
    }
    return false;
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus)
  {
    return (this.mCurrentStatusEffects & iStatus) == iStatus;
  }

  public float StatusMagnitude(int iIndex, StatusEffects iStatus)
  {
    int index = StatusEffect.StatusIndex(iStatus);
    return !this.mStatusEffects[index].Dead ? this.mStatusEffects[index].Magnitude : 0.0f;
  }

  public void ScriptMessage(BossMessages iMessage)
  {
  }

  private unsafe void NetworkUpdate()
  {
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer))
      return;
    Jormungandr.UpdateMessage updateMessage = new Jormungandr.UpdateMessage();
    updateMessage.Animation = (byte) 0;
    while ((int) updateMessage.Animation < this.mAnimationClips.Length && this.mAnimationController.AnimationClip != this.mAnimationClips[(int) updateMessage.Animation])
      ++updateMessage.Animation;
    updateMessage.AnimationTime = this.mAnimationController.Time;
    updateMessage.Hitpoints = this.mHitPoints;
    updateMessage.Position = this.mPosition;
    updateMessage.Direction = this.mDirection;
    updateMessage.Orientation = this.mOrientation;
    for (int index = 0; index < networkServer.Connections; ++index)
    {
      float num = networkServer.GetLatency(index) * 0.5f;
      updateMessage.AnimationTime += num;
      BossFight.Instance.SendMessage<Jormungandr.UpdateMessage>((IBoss) this, (ushort) 0, (void*) &updateMessage, false, index);
    }
  }

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    if (iMsg.Type == (ushort) 0)
    {
      if (iMsg.TimeStamp < this.mLastNetworkUpdate)
        return;
      this.mLastNetworkUpdate = iMsg.TimeStamp;
      Jormungandr.UpdateMessage updateMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &updateMessage);
      if (this.mAnimationController.AnimationClip != this.mAnimationClips[(int) updateMessage.Animation])
        this.mAnimationController.StartClip(this.mAnimationClips[(int) updateMessage.Animation], false);
      this.mAnimationController.Time = updateMessage.AnimationTime;
      this.mHitPoints = updateMessage.Hitpoints;
      this.mPosition = updateMessage.Position;
      this.mDirection = updateMessage.Direction;
      this.mOrientation = updateMessage.Orientation;
    }
    else if (iMsg.Type == (ushort) 3)
    {
      Jormungandr.SpitMessage spitMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &spitMessage);
      MissileEntity fromHandle = Magicka.GameLogic.Entities.Entity.GetFromHandle((int) spitMessage.Handle) as MissileEntity;
      fromHandle.Initialize((Magicka.GameLogic.Entities.Entity) this.mDamageZone, 0.25f, ref spitMessage.Position, ref spitMessage.Velocity, (Model) null, this.mSpitConditions, false);
      this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) fromHandle);
    }
    else if (iMsg.Type == (ushort) 2)
    {
      Jormungandr.ChangeTargetMessage changeTargetMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeTargetMessage);
      if (changeTargetMessage.Target == ushort.MaxValue)
        this.mTarget = (Magicka.GameLogic.Entities.Character) null;
      else
        this.mTarget = Magicka.GameLogic.Entities.Entity.GetFromHandle((int) changeTargetMessage.Target) as Magicka.GameLogic.Entities.Character;
    }
    else
    {
      if (iMsg.Type != (ushort) 1)
        return;
      Jormungandr.ChangeStateMessage changeStateMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
      if (changeStateMessage.Animation != Jormungandr.Animations.Invalid)
      {
        if ((double) changeStateMessage.AnimationBlendTime > 1.4012984643248171E-45)
          this.mAnimationController.CrossFade(this.mAnimationClips[(int) changeStateMessage.Animation], changeStateMessage.AnimationBlendTime, changeStateMessage.AnimationLooped);
        else
          this.mAnimationController.StartClip(this.mAnimationClips[(int) changeStateMessage.Animation], changeStateMessage.AnimationLooped);
      }
      switch (changeStateMessage.NewState)
      {
        case Jormungandr.State.Sleep:
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.SleepIntroState.Instance);
          break;
        case Jormungandr.State.Intro:
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.IntroState.Instance);
          break;
        case Jormungandr.State.Underground:
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.UndergroundState.Instance);
          break;
        case Jormungandr.State.Leap:
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.LeapState.Instance);
          break;
        case Jormungandr.State.Risen:
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.RisenState.Instance);
          break;
        case Jormungandr.State.Spit:
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.SpitState.Instance);
          break;
        case Jormungandr.State.Bite:
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.BiteState.Instance);
          break;
        case Jormungandr.State.HitRisen:
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.HitRisenState.Instance);
          break;
        case Jormungandr.State.HitLeap:
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.HitLeapState.Instance);
          break;
        case Jormungandr.State.Dead:
          this.mDeathAnimation = !(this.mCurrentState is Jormungandr.UndergroundState) ? Jormungandr.Animations.Die_Above : Jormungandr.Animations.Die_Below;
          this.ChangeState((IBossState<Jormungandr>) Jormungandr.DeadState.Instance);
          break;
      }
    }
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  protected override BossDamageZone Entity => this.mDamageZone;

  protected override float Radius => 1.5f;

  protected override float Length => 1f;

  protected override int BloodEffect => Jormungandr.BLOOD_EFFECT;

  protected override Vector3 NotifierTextPostion
  {
    get
    {
      Vector3 position = this.mDamageZone.Position;
      position.Y += 1.5f;
      return position;
    }
  }

  public override bool Dead => this.mDead;

  public BossEnum GetBossType() => BossEnum.Jormungandr;

  public bool NetworkInitialized => true;

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

  protected class RenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
  {
    public float Damage;
    public float Flash;
    public Matrix[] mSkeleton;

    public RenderData() => this.mSkeleton = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
      modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
      modelDeferredEffect.Bones = this.mSkeleton;
      modelDeferredEffect.Damage = this.Damage;
      base.Draw(iEffect, iViewFrustum);
      modelDeferredEffect.OverrideColor = Vector4.Zero;
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.mSkeleton;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }

  public enum State : byte
  {
    Sleep,
    Intro,
    Underground,
    Leap,
    Risen,
    Spit,
    Bite,
    HitRisen,
    HitLeap,
    Dead,
  }

  protected class GlobalState : IBossState<Jormungandr>
  {
    private static Jormungandr.GlobalState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Jormungandr.GlobalState Instance
    {
      get
      {
        if (Jormungandr.GlobalState.mSingelton == null)
        {
          lock (Jormungandr.GlobalState.mSingeltonLock)
          {
            if (Jormungandr.GlobalState.mSingelton == null)
              Jormungandr.GlobalState.mSingelton = new Jormungandr.GlobalState();
          }
        }
        return Jormungandr.GlobalState.mSingelton;
      }
    }

    private GlobalState()
    {
      Jormungandr.IntroState instance1 = Jormungandr.IntroState.Instance;
      Jormungandr.UndergroundState instance2 = Jormungandr.UndergroundState.Instance;
      Jormungandr.LeapState instance3 = Jormungandr.LeapState.Instance;
      Jormungandr.RisenState instance4 = Jormungandr.RisenState.Instance;
      Jormungandr.SpitState instance5 = Jormungandr.SpitState.Instance;
      Jormungandr.BiteState instance6 = Jormungandr.BiteState.Instance;
      Jormungandr.HitRisenState instance7 = Jormungandr.HitRisenState.Instance;
      Jormungandr.HitLeapState instance8 = Jormungandr.HitLeapState.Instance;
      Jormungandr.DeadState instance9 = Jormungandr.DeadState.Instance;
    }

    public void OnEnter(Jormungandr iOwner) => throw new NotImplementedException();

    public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      if (iOwner.mCurrentState is Jormungandr.DeadState || !iOwner.mIsHit)
        return;
      if (iOwner.mCurrentState is Jormungandr.LeapState)
      {
        if (NetworkManager.Instance.State == NetworkState.Client)
          return;
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Jormungandr.ChangeStateMessage changeStateMessage;
          changeStateMessage.NewState = Jormungandr.State.HitLeap;
          changeStateMessage.Animation = Jormungandr.Animations.Invalid;
          BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
        }
        iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.HitLeapState.Instance);
      }
      else if (iOwner.mCurrentState is Jormungandr.RisenState || iOwner.mCurrentState is Jormungandr.BiteState)
      {
        if (NetworkManager.Instance.State == NetworkState.Client)
          return;
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Jormungandr.ChangeStateMessage changeStateMessage;
          changeStateMessage.NewState = Jormungandr.State.HitRisen;
          changeStateMessage.Animation = Jormungandr.Animations.Invalid;
          BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
        }
        iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.HitRisenState.Instance);
      }
      else
        iOwner.mIsHit = false;
    }

    public void OnExit(Jormungandr iOwner) => throw new NotImplementedException();
  }

  protected class SleepIntroState : IBossState<Jormungandr>
  {
    private static Jormungandr.SleepIntroState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Jormungandr.SleepIntroState Instance
    {
      get
      {
        if (Jormungandr.SleepIntroState.mSingelton == null)
        {
          lock (Jormungandr.SleepIntroState.mSingeltonLock)
          {
            if (Jormungandr.SleepIntroState.mSingelton == null)
              Jormungandr.SleepIntroState.mSingelton = new Jormungandr.SleepIntroState();
          }
        }
        return Jormungandr.SleepIntroState.mSingelton;
      }
    }

    private SleepIntroState()
    {
    }

    public void OnEnter(Jormungandr iOwner)
    {
      iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[1], false);
      Matrix mOrientation = iOwner.mOrientation with
      {
        Translation = iOwner.mPosition
      };
      iOwner.mAnimationController.Update(0.0f, ref mOrientation, true);
      iOwner.mAnimationController.Stop();
      iOwner.mDraw = true;
      iOwner.mUpdateAnimation = true;
      iOwner.mNoEarthCollisionEffect = true;
    }

    public void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
    }

    public void OnExit(Jormungandr iOwner)
    {
    }
  }

  protected class IntroState : IBossState<Jormungandr>
  {
    private static Jormungandr.IntroState mSingelton;
    private static volatile object mSingeltonLock = new object();
    private bool mPlayedCue;

    public static Jormungandr.IntroState Instance
    {
      get
      {
        if (Jormungandr.IntroState.mSingelton == null)
        {
          lock (Jormungandr.IntroState.mSingeltonLock)
          {
            if (Jormungandr.IntroState.mSingelton == null)
              Jormungandr.IntroState.mSingelton = new Jormungandr.IntroState();
          }
        }
        return Jormungandr.IntroState.mSingelton;
      }
    }

    private IntroState()
    {
    }

    public void OnEnter(Jormungandr iOwner)
    {
      iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[1], false);
      AudioManager.Instance.PlayCue(Banks.Characters, "boss_jormungandr_wakeup".GetHashCodeCustom(), iOwner.mDamageZone.AudioEmitter);
      iOwner.mDraw = true;
      iOwner.mUpdateAnimation = true;
      iOwner.mNoEarthCollisionEffect = true;
      this.mPlayedCue = false;
    }

    public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.HasFinished)
      {
        if (NetworkManager.Instance.State == NetworkState.Client)
          return;
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Jormungandr.ChangeStateMessage changeStateMessage;
          changeStateMessage.NewState = Jormungandr.State.Underground;
          changeStateMessage.Animation = Jormungandr.Animations.Invalid;
          BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
        }
        iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.UndergroundState.Instance);
      }
      else
      {
        if ((double) iOwner.mAnimationController.Time <= 2.2000000476837158 || this.mPlayedCue)
          return;
        AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[2], iOwner.mDamageZone.AudioEmitter);
        this.mPlayedCue = true;
      }
    }

    public void OnExit(Jormungandr iOwner) => iOwner.mIdleTimer = 0.0f;
  }

  protected class UndergroundState : IBossState<Jormungandr>
  {
    private static Jormungandr.UndergroundState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Jormungandr.UndergroundState Instance
    {
      get
      {
        if (Jormungandr.UndergroundState.mSingelton == null)
        {
          lock (Jormungandr.UndergroundState.mSingeltonLock)
          {
            if (Jormungandr.UndergroundState.mSingelton == null)
              Jormungandr.UndergroundState.mSingelton = new Jormungandr.UndergroundState();
          }
        }
        return Jormungandr.UndergroundState.mSingelton;
      }
    }

    private UndergroundState()
    {
    }

    public void OnEnter(Jormungandr iOwner)
    {
      EffectManager.Instance.Stop(ref iOwner.mEarthSplashReference);
      EffectManager.Instance.Stop(ref iOwner.mEarthTremorReference);
      iOwner.mNoEarthCollisionEffect = false;
      iOwner.mDirtSpawned = false;
      iOwner.mDraw = false;
      iOwner.mUpdateAnimation = false;
      iOwner.mIsHit = false;
      iOwner.mIdleTimer = 0.0f;
      iOwner.mAttackCount = 0;
      iOwner.mSpineHitlist.Clear();
      iOwner.mHitlist.Clear();
      for (int index = 0; index < iOwner.mStatusEffects.Length; ++index)
      {
        iOwner.mStatusEffects[index].Stop();
        iOwner.mStatusEffects[index] = new StatusEffect();
      }
    }

    public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      if ((double) iOwner.HitPoints <= 0.0)
      {
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Jormungandr.ChangeStateMessage changeStateMessage;
          changeStateMessage.NewState = Jormungandr.State.Dead;
          changeStateMessage.Animation = Jormungandr.Animations.Invalid;
          BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
        }
        iOwner.mDeathAnimation = Jormungandr.Animations.Die_Below;
        iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.DeadState.Instance);
      }
      else
      {
        iOwner.mIdleTimer += iDeltaTime;
        if ((double) iOwner.mIdleTimer <= (double) Jormungandr.WARNINGTIME)
          return;
        iOwner.SelectTarget(Jormungandr.TargettingType.Random);
        float iMagnitude = (float) (0.5 + MagickaMath.Random.NextDouble() * 0.5);
        iOwner.mPlayState.Camera.CameraShake(iOwner.mDamageZone.Position, iMagnitude, Jormungandr.WARNINGTIME);
        float num1 = (float) Jormungandr.mRandom.NextDouble();
        float num2 = (float) (0.40000000596046448 * (double) iOwner.mHitPoints / 10000.0);
        iOwner.mAttackCount = Jormungandr.mRandom.Next(2) + 2;
        if ((double) num1 <= 0.34999999403953552 + (double) num2)
        {
          iOwner.mPosition = iOwner.mTarget.Position;
          --iOwner.mPosition.Y;
          iOwner.mOrientation = Matrix.CreateRotationY(2.5f);
          iOwner.mOrientation.Translation = iOwner.mPosition;
          iOwner.mHitlist.Clear();
          Vector3 mPosition = iOwner.mPosition;
          iOwner.mDamageZone.SetPosition(ref mPosition);
          if (NetworkManager.Instance.State == NetworkState.Server)
          {
            Jormungandr.ChangeStateMessage changeStateMessage;
            changeStateMessage.NewState = Jormungandr.State.Risen;
            changeStateMessage.Animation = Jormungandr.Animations.Rise;
            changeStateMessage.AnimationBlendTime = 0.0f;
            changeStateMessage.AnimationLooped = false;
            BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
          }
          iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[4], false);
          iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.RisenState.Instance);
        }
        else
        {
          bool flag1 = true;
          Vector3 result1 = new Vector3();
          Vector3 position = iOwner.mTarget.Position;
          Vector3 vector3 = new Vector3();
          double num3 = Jormungandr.mRandom.NextDouble() * System.Math.PI * 2.0;
          for (int index1 = 0; index1 < 6; ++index1)
          {
            vector3.X = (float) System.Math.Cos(num3);
            vector3.Z = (float) System.Math.Sin(num3);
            num3 += System.Math.PI / 3.0;
            Vector3 result2 = vector3;
            Vector3.Negate(ref vector3, out result2);
            Vector3 result3;
            Vector3.Multiply(ref vector3, 14f, out result3);
            Vector3.Add(ref position, ref result3, out result1);
            Vector3 oPoint = new Vector3();
            double nearestPosition = (double) iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result1, out oPoint, MovementProperties.Default);
            Vector3 result4;
            Vector3.Subtract(ref oPoint, ref result1, out result4);
            Vector3 result5 = new Vector3();
            Vector3.Subtract(ref position, ref oPoint, out result5);
            result5.Y = 0.0f;
            result5.Normalize();
            bool flag2 = false;
            Segment seg = new Segment();
            seg.Origin = oPoint;
            seg.Delta.Y = 10f;
            for (int index2 = 0; index2 < iOwner.mPlayState.Level.CurrentScene.Liquids.Length; ++index2)
            {
              if (iOwner.mPlayState.Level.CurrentScene.Liquids[index2].CollisionSkin.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, seg))
                flag2 = true;
            }
            if ((double) result4.Length() <= 2.0 && !flag2)
            {
              flag1 = false;
              result1 = oPoint;
              vector3 = result5;
            }
          }
          if (flag1)
            return;
          iOwner.mPosition = result1;
          Segment seg1 = new Segment();
          seg1.Delta.Y = -10f;
          Vector3.Negate(ref vector3, out iOwner.mDirection);
          Vector3 result6;
          Vector3.Multiply(ref vector3, 14f, out result6);
          Vector3.Add(ref position, ref result6, out seg1.Origin);
          iOwner.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out float _, out iOwner.mPosition, out Vector3 _, seg1);
          Jormungandr.CreateTransform(ref iOwner.mPosition, ref iOwner.mDirection, out iOwner.mOrientation);
          iOwner.mOrientation.M41 -= vector3.X * 8f;
          iOwner.mOrientation.M42 -= 9.5f;
          iOwner.mOrientation.M43 -= vector3.Z * 8f;
          iOwner.mDamageZone.SetPosition(ref result1);
          if (NetworkManager.Instance.State == NetworkState.Server)
          {
            Jormungandr.ChangeStateMessage changeStateMessage;
            changeStateMessage.NewState = Jormungandr.State.Leap;
            changeStateMessage.Animation = Jormungandr.Animations.Invalid;
            BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
          }
          iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.LeapState.Instance);
        }
      }
    }

    public void OnExit(Jormungandr iOwner)
    {
    }
  }

  protected class LeapState : IBossState<Jormungandr>
  {
    private static Jormungandr.LeapState mSingelton;
    private static volatile object mSingeltonLock = new object();
    protected bool mTargetLock;
    protected bool mHasGrowled;
    protected bool mHasWhipped;
    protected bool mHasWhipDamaged;
    private DamageCollection5 mWhipDamage;

    public static Jormungandr.LeapState Instance
    {
      get
      {
        if (Jormungandr.LeapState.mSingelton == null)
        {
          lock (Jormungandr.LeapState.mSingeltonLock)
          {
            if (Jormungandr.LeapState.mSingelton == null)
              Jormungandr.LeapState.mSingelton = new Jormungandr.LeapState();
          }
        }
        return Jormungandr.LeapState.mSingelton;
      }
    }

    private LeapState()
    {
      this.mWhipDamage = new DamageCollection5();
      this.mWhipDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, 200f, 4f));
      this.mWhipDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth, 300f, 1f));
    }

    public void OnEnter(Jormungandr iOwner)
    {
      Matrix result;
      Matrix.CreateRotationX(MathHelper.ToRadians(125f), out result);
      Matrix.Multiply(ref result, ref iOwner.mOrientation, out iOwner.mOrientation);
      iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[2], false);
      iOwner.mUpdateAnimation = false;
      iOwner.mDraw = false;
      iOwner.mDamageState = Jormungandr.DamageState.Leap;
      iOwner.mIdleTimer = 0.0f;
      iOwner.mHitlist.Clear();
      iOwner.mDirtSpawned = false;
      iOwner.mNoEarthCollisionEffect = false;
      iOwner.mMidLeap = false;
      this.mHasGrowled = false;
      this.mHasWhipped = false;
      this.mHasWhipDamaged = false;
      this.mTargetLock = false;
    }

    public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      iOwner.mIdleTimer += iDeltaTime;
      if (!iOwner.mAnimationController.CrossFadeEnabled && (double) num1 > (double) Jormungandr.TAIL_WHIP && !this.mHasWhipDamaged)
      {
        Matrix result;
        Matrix.Multiply(ref iOwner.mSpineBindPose[1], ref iOwner.mAnimationController.SkinnedBoneTransforms[iOwner.mSpineIndices[1]], out result);
        Vector3 translation = result.Translation;
        int num2 = (int) Helper.CircleDamage(iOwner.mPlayState, (Magicka.GameLogic.Entities.Entity) iOwner.mDamageZone, iOwner.mPlayState.PlayTime, (Magicka.GameLogic.Entities.Entity) iOwner.mSpine, ref translation, 3f, ref this.mWhipDamage);
        this.mHasWhipDamaged = true;
      }
      if (!this.mTargetLock)
      {
        Vector3 position = iOwner.mTarget.Position;
        Vector3.Subtract(ref position, ref iOwner.mPosition, out iOwner.mDirection);
        iOwner.mDirection.Normalize();
        Jormungandr.CreateTransform(ref iOwner.mPosition, ref iOwner.mDirection, out iOwner.mOrientation);
        Matrix result;
        Matrix.CreateRotationX(MathHelper.ToRadians(125f), out result);
        Matrix.Multiply(ref result, ref iOwner.mOrientation, out iOwner.mOrientation);
        iOwner.mOrientation.M41 += iOwner.mDirection.X * 8f;
        iOwner.mOrientation.M42 -= 9.5f;
        iOwner.mOrientation.M43 += iOwner.mDirection.Z * 8f;
        this.mTargetLock = true;
        AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[3]);
      }
      if ((double) iOwner.mIdleTimer <= (double) Jormungandr.WARNINGTIME)
        return;
      if ((double) iOwner.mIdleTimer > (double) Jormungandr.WARNINGTIME + 0.89999997615814209 && !iOwner.mMidLeap)
      {
        iOwner.mMidLeap = true;
        iOwner.mDirtSpawned = false;
        EffectManager.Instance.Stop(ref iOwner.mEarthSplashReference);
      }
      else if ((double) iOwner.mIdleTimer > (double) Jormungandr.WARNINGTIME + 0.60000002384185791 && !this.mHasGrowled)
      {
        AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[7]);
        this.mHasGrowled = true;
      }
      else if ((double) iOwner.mIdleTimer > (double) Jormungandr.WARNINGTIME + 1.8999999761581421 && !this.mHasWhipped)
      {
        AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[13]);
        this.mHasWhipped = true;
      }
      EffectManager.Instance.Stop(ref iOwner.mEarthTremorReference);
      iOwner.mUpdateAnimation = true;
      iOwner.mDraw = true;
      Matrix result1;
      Matrix.CreateRotationX(MathHelper.ToRadians(-(275f / iOwner.mAnimationController.AnimationClip.Duration * iDeltaTime)), out result1);
      Matrix.Multiply(ref result1, ref iOwner.mOrientation, out iOwner.mOrientation);
      iOwner.mPosition = iOwner.mDamageZone.Position;
      if (!iOwner.mAnimationController.HasFinished || NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Jormungandr.ChangeStateMessage changeStateMessage;
        changeStateMessage.NewState = Jormungandr.State.Underground;
        changeStateMessage.Animation = Jormungandr.Animations.Invalid;
        BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
      }
      iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.UndergroundState.Instance);
    }

    public void OnExit(Jormungandr iOwner)
    {
      iOwner.mDamageState = Jormungandr.DamageState.None;
      iOwner.mPosition = iOwner.mDamageZone.Position;
      EffectManager.Instance.Stop(ref iOwner.mEarthSplashReference);
      iOwner.mMidLeap = false;
      this.mTargetLock = false;
      iOwner.mAnimationController.Speed = 1f;
    }
  }

  protected class RisenState : IBossState<Jormungandr>
  {
    private static Jormungandr.RisenState mSingelton;
    private static volatile object mSingeltonLock = new object();
    private VisualEffectReference mDeathEffect;
    private int mDeathEffectHash = "barrier_earth_death".GetHashCodeCustom();
    private bool mBarriersCleared;
    private bool mRoarPlayed;

    public static Jormungandr.RisenState Instance
    {
      get
      {
        if (Jormungandr.RisenState.mSingelton == null)
        {
          lock (Jormungandr.RisenState.mSingeltonLock)
          {
            if (Jormungandr.RisenState.mSingelton == null)
              Jormungandr.RisenState.mSingelton = new Jormungandr.RisenState();
          }
        }
        return Jormungandr.RisenState.mSingelton;
      }
    }

    private RisenState()
    {
    }

    public void OnEnter(Jormungandr iOwner)
    {
      iOwner.mIdleTimer = 0.0f;
      iOwner.mNoEarthCollisionEffect = false;
      iOwner.mDirtSpawned = false;
      this.mBarriersCleared = false;
      this.mRoarPlayed = false;
      iOwner.mSpineHitlist.Clear();
      if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[4])
      {
        iOwner.StartEffect(out iOwner.mEarthTremorReference, Jormungandr.JORMUNGANDREFFECTS[1]);
        AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[3]);
      }
      iOwner.SelectTarget(Jormungandr.TargettingType.Angle);
    }

    public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      iOwner.mIdleTimer += iDeltaTime;
      if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[5])
      {
        if (!this.mBarriersCleared)
        {
          List<Magicka.GameLogic.Entities.Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(iOwner.mPosition, 3f, false);
          for (int index = 0; index < entities.Count; ++index)
          {
            if (entities[index] is Shield | entities[index] is Barrier)
            {
              IDamageable damageable = entities[index] as IDamageable;
              Vector3 position = entities[index].Position;
              Vector3 right = Vector3.Right;
              EffectManager.Instance.StartEffect(this.mDeathEffectHash, ref position, ref right, out this.mDeathEffect);
              damageable.Kill();
            }
          }
          iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
          this.mBarriersCleared = true;
        }
        if (!iOwner.mDirtSpawned)
        {
          AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[2]);
          iOwner.mDirtSpawned = true;
          iOwner.mNoEarthCollisionEffect = true;
          float iMagnitude = (float) (0.5 + MagickaMath.Random.NextDouble() * 0.5);
          iOwner.mPlayState.Camera.CameraShake(iMagnitude, 1f);
        }
        if (!iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled || NetworkManager.Instance.State == NetworkState.Client)
          return;
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Jormungandr.ChangeStateMessage changeStateMessage;
          changeStateMessage.NewState = Jormungandr.State.Underground;
          changeStateMessage.Animation = Jormungandr.Animations.Invalid;
          BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
        }
        iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.UndergroundState.Instance);
      }
      else if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[4])
      {
        if ((double) iOwner.mAnimationController.Time / (double) iOwner.mAnimationController.AnimationClip.Duration >= 0.125 && !this.mRoarPlayed)
        {
          this.mRoarPlayed = true;
          AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[5]);
        }
        if (iOwner.mAnimationController.HasFinished)
        {
          if (NetworkManager.Instance.State != NetworkState.Client)
          {
            if (NetworkManager.Instance.State == NetworkState.Server)
            {
              Jormungandr.ChangeStateMessage changeStateMessage;
              changeStateMessage.NewState = Jormungandr.State.Risen;
              changeStateMessage.Animation = Jormungandr.Animations.Idle;
              changeStateMessage.AnimationBlendTime = 0.5f;
              changeStateMessage.AnimationLooped = true;
              BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
            }
            iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.5f, true);
          }
          iOwner.mIdleTimer = 0.0f;
        }
        if ((double) iOwner.mIdleTimer <= (double) Jormungandr.WARNINGTIME && !iOwner.mDirtSpawned)
          return;
        iOwner.mDraw = true;
        iOwner.mUpdateAnimation = true;
        iOwner.mDamageState = Jormungandr.DamageState.Emerge;
        if (this.mBarriersCleared)
          return;
        List<Magicka.GameLogic.Entities.Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(iOwner.mPosition, 2f, false);
        for (int index = 0; index < entities.Count; ++index)
        {
          if (entities[index] is Shield | entities[index] is Barrier)
          {
            IDamageable damageable = entities[index] as IDamageable;
            Vector3 position = entities[index].Position;
            Vector3 right = Vector3.Right;
            EffectManager.Instance.StartEffect(this.mDeathEffectHash, ref position, ref right, out this.mDeathEffect);
            damageable.Kill();
          }
        }
        iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
        this.mBarriersCleared = true;
      }
      else if (NetworkManager.Instance.State != NetworkState.Client && (double) iOwner.mHitPoints <= 0.0)
      {
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Jormungandr.ChangeStateMessage changeStateMessage;
          changeStateMessage.NewState = Jormungandr.State.Dead;
          changeStateMessage.Animation = Jormungandr.Animations.Invalid;
          BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
        }
        iOwner.mDeathAnimation = Jormungandr.Animations.Die_Above;
        iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.DeadState.Instance);
      }
      else if (!iOwner.mAnimationController.CrossFadeEnabled && iOwner.mAttackCount <= 0)
      {
        if (NetworkManager.Instance.State == NetworkState.Client)
          return;
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Jormungandr.ChangeStateMessage changeStateMessage;
          changeStateMessage.NewState = Jormungandr.State.Risen;
          changeStateMessage.Animation = Jormungandr.Animations.Submerge;
          changeStateMessage.AnimationBlendTime = 0.5f;
          changeStateMessage.AnimationLooped = false;
          BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
        }
        iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[5], 0.5f, false);
      }
      else
      {
        Vector3 position1;
        Vector3 result1;
        if (iOwner.mTarget != null)
        {
          Vector3 position2 = iOwner.mDamageZone.Position;
          position1 = iOwner.mTarget.Position;
          position2.Y = position1.Y = 0.0f;
          Vector3.Subtract(ref position1, ref position2, out result1);
          float num = result1.Length();
          if ((double) num > 1.4012984643248171E-45)
            Vector3.Divide(ref result1, num, out result1);
          else
            result1 = Vector3.Backward;
          iOwner.Turn(ref result1, 2f, iDeltaTime);
        }
        if ((double) iOwner.mIdleTimer <= (double) Jormungandr.WARNINGTIME + 0.800000011920929 || iOwner.mTarget == null)
          return;
        position1 = iOwner.mTarget.Position;
        Vector3.Subtract(ref position1, ref iOwner.mPosition, out result1);
        float num1 = result1.Length();
        result1.Y = 0.0f;
        bool flag = false;
        Vector3 result2 = iOwner.mPosition;
        Vector3 result3 = iOwner.mOrientation.Forward;
        Vector3.Multiply(ref result3, 3f, out result3);
        Vector3.Add(ref result2, ref result3, out result2);
        List<Magicka.GameLogic.Entities.Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(result2, 2f, false);
        for (int index = 0; index < entities.Count; ++index)
        {
          if (entities[index] is Barrier && (entities[index] as Barrier).Solid)
          {
            flag = true;
            break;
          }
        }
        iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
        if (((double) num1 > 15.0 || flag) && iOwner.mLastState != Jormungandr.SpitState.Instance)
        {
          if (NetworkManager.Instance.State == NetworkState.Client)
            return;
          if (NetworkManager.Instance.State == NetworkState.Server)
          {
            Jormungandr.ChangeStateMessage changeStateMessage;
            changeStateMessage.NewState = Jormungandr.State.Spit;
            changeStateMessage.Animation = Jormungandr.Animations.Invalid;
            BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
          }
          iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.SpitState.Instance);
        }
        else
        {
          if (NetworkManager.Instance.State == NetworkState.Client)
            return;
          if (NetworkManager.Instance.State == NetworkState.Server)
          {
            Jormungandr.ChangeStateMessage changeStateMessage;
            changeStateMessage.NewState = Jormungandr.State.Bite;
            changeStateMessage.Animation = Jormungandr.Animations.Invalid;
            BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
          }
          iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.BiteState.Instance);
        }
      }
    }

    public void OnExit(Jormungandr iOwner) => iOwner.mDamageState = Jormungandr.DamageState.None;
  }

  protected class SpitState : IBossState<Jormungandr>
  {
    private static Jormungandr.SpitState mSingelton;
    private static volatile object mSingeltonLock = new object();
    private float mSpitTimer;
    private int mNumProjectiles;
    private bool mHasSprayed;
    private VisualEffectReference mSprayRef;

    public static Jormungandr.SpitState Instance
    {
      get
      {
        if (Jormungandr.SpitState.mSingelton == null)
        {
          lock (Jormungandr.SpitState.mSingeltonLock)
          {
            if (Jormungandr.SpitState.mSingelton == null)
              Jormungandr.SpitState.mSingelton = new Jormungandr.SpitState();
          }
        }
        return Jormungandr.SpitState.mSingelton;
      }
    }

    private SpitState()
    {
    }

    public void OnEnter(Jormungandr iOwner)
    {
      iOwner.mAnimationController.Speed = 1.333f;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[8], 0.2f, false);
      iOwner.mDamageState = Jormungandr.DamageState.None;
      AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[9]);
      iOwner.SelectTarget(Jormungandr.TargettingType.Random);
      this.mNumProjectiles = 0;
      this.mSpitTimer = 0.0f;
      this.mHasSprayed = false;
    }

    public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      if (iOwner.mTarget != null)
      {
        float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        Vector3 position1 = iOwner.mTarget.Position;
        Vector3 position2 = iOwner.mDamageZone.Position;
        Vector3 vector3 = position1;
        position2.Y = vector3.Y = 0.0f;
        Vector3 position3 = iOwner.mTarget.Position;
        Vector3 result1;
        Vector3.Subtract(ref vector3, ref position2, out result1);
        float num2 = result1.Length();
        if ((double) num2 > 1.4012984643248171E-45)
          Vector3.Divide(ref result1, num2, out result1);
        else
          result1 = iOwner.mOrientation.Forward;
        iOwner.Turn(ref result1, 1f, iDeltaTime);
        if (!iOwner.mAnimationController.CrossFadeEnabled)
        {
          this.mSpitTimer -= iDeltaTime;
          Vector3 result2 = iOwner.mDamageZone.Position;
          if ((double) num1 >= 0.375 & !this.mHasSprayed)
          {
            EffectManager.Instance.StartEffect(Jormungandr.JORMUNGANDREFFECTS[3], ref result2, ref iOwner.mDirection, out this.mSprayRef);
            AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[6]);
            this.mHasSprayed = true;
          }
          else
            EffectManager.Instance.UpdatePositionDirection(ref this.mSprayRef, ref result2, ref iOwner.mDirection);
          if ((double) num1 >= 0.37999999523162842 & (double) this.mSpitTimer <= 0.0 & this.mNumProjectiles < 4)
          {
            ++this.mNumProjectiles;
            this.mSpitTimer += 0.5f;
            if (NetworkManager.Instance.State != NetworkState.Client)
            {
              Vector3.Multiply(ref result1, 0.5f, out result1);
              Vector3.Add(ref result2, ref result1, out result2);
              MissileEntity instance = MissileEntity.GetInstance(iOwner.mPlayState);
              Vector3 gravity = PhysicsSystem.CurrentPhysicsSystem.Gravity;
              float num3 = (float) System.Math.Sqrt((double) ((result2.Y - position3.Y) / (gravity.Y * -0.5f)));
              Vector3 result3 = new Vector3();
              Vector3.Subtract(ref position3, ref result2, out result3);
              Vector3 result4;
              Vector3.Multiply(ref gravity, num3 * num3, out result4);
              Vector3.Divide(ref result4, 2f, out result4);
              Vector3.Subtract(ref result3, ref result4, out result3);
              Vector3.Divide(ref result3, num3, out result3);
              instance.Initialize((Magicka.GameLogic.Entities.Entity) iOwner.mDamageZone, 0.25f, ref result2, ref result3, (Model) null, iOwner.mSpitConditions, false);
              iOwner.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) instance);
              if (NetworkManager.Instance.State == NetworkState.Server)
              {
                Jormungandr.SpitMessage spitMessage;
                spitMessage.Handle = instance.Handle;
                spitMessage.Position = result2;
                spitMessage.Velocity = result3;
                BossFight.Instance.SendMessage<Jormungandr.SpitMessage>((IBoss) iOwner, (ushort) 3, (void*) &spitMessage, true);
              }
            }
          }
        }
      }
      if (!iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled || NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Jormungandr.ChangeStateMessage changeStateMessage;
        changeStateMessage.NewState = Jormungandr.State.Risen;
        changeStateMessage.Animation = Jormungandr.Animations.Idle;
        changeStateMessage.AnimationBlendTime = 0.5f;
        changeStateMessage.AnimationLooped = true;
        BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
      }
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.5f, true);
      iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.RisenState.Instance);
    }

    public void OnExit(Jormungandr iOwner)
    {
      iOwner.SelectTarget(Jormungandr.TargettingType.Random);
      iOwner.mAnimationController.Speed = 1f;
    }
  }

  protected class BiteState : IBossState<Jormungandr>
  {
    private static Jormungandr.BiteState mSingelton;
    private static volatile object mSingeltonLock = new object();
    private bool mHasPlayedSound;

    public static Jormungandr.BiteState Instance
    {
      get
      {
        if (Jormungandr.BiteState.mSingelton == null)
        {
          lock (Jormungandr.BiteState.mSingeltonLock)
          {
            if (Jormungandr.BiteState.mSingelton == null)
              Jormungandr.BiteState.mSingelton = new Jormungandr.BiteState();
          }
        }
        return Jormungandr.BiteState.mSingelton;
      }
    }

    private BiteState()
    {
    }

    public void OnEnter(Jormungandr iOwner)
    {
      this.mHasPlayedSound = false;
      --iOwner.mAttackCount;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[9], 0.15f, false);
      iOwner.mDamageState = Jormungandr.DamageState.Bite;
      iOwner.mHitlist.Clear();
      AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[10]);
      if (iOwner.mTarget != null)
        return;
      iOwner.SelectTarget(Jormungandr.TargettingType.PlayerStrain);
    }

    public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      if (iOwner.mTarget != null)
      {
        Vector3 position = iOwner.mTarget.Position;
        Vector3 result;
        Vector3.Subtract(ref position, ref iOwner.mPosition, out result);
        float num = result.Length();
        result.Y = 0.0f;
        if ((double) num > 1.4012984643248171E-45)
          Vector3.Divide(ref result, num, out result);
        else
          result = Vector3.Backward;
        iOwner.Turn(ref result, 1.5f, iDeltaTime);
      }
      if (!iOwner.mAnimationController.CrossFadeEnabled && (double) iOwner.mAnimationController.Time / (double) iOwner.mAnimationClips[9].Duration >= 0.40000000596046448 && !this.mHasPlayedSound)
      {
        float num = (float) (Jormungandr.mRandom.NextDouble() * 0.60000002384185791 * (1.0 - (double) iOwner.mHitPoints / (double) iOwner.MaxHitPoints));
        iOwner.mAnimationController.Speed = 1f + num;
        AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[7]);
        this.mHasPlayedSound = true;
      }
      if (!iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled || NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Jormungandr.ChangeStateMessage changeStateMessage;
        changeStateMessage.NewState = Jormungandr.State.Risen;
        changeStateMessage.Animation = Jormungandr.Animations.Idle;
        changeStateMessage.AnimationBlendTime = 0.5f;
        changeStateMessage.AnimationLooped = true;
        BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
      }
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.5f, true);
      iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.RisenState.Instance);
    }

    public void OnExit(Jormungandr iOwner)
    {
      iOwner.mDamageState = Jormungandr.DamageState.None;
      iOwner.mAnimationController.Speed = 1f;
    }
  }

  protected class HitRisenState : IBossState<Jormungandr>
  {
    private static Jormungandr.HitRisenState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Jormungandr.HitRisenState Instance
    {
      get
      {
        if (Jormungandr.HitRisenState.mSingelton == null)
        {
          lock (Jormungandr.HitRisenState.mSingeltonLock)
          {
            if (Jormungandr.HitRisenState.mSingelton == null)
              Jormungandr.HitRisenState.mSingelton = new Jormungandr.HitRisenState();
          }
        }
        return Jormungandr.HitRisenState.mSingelton;
      }
    }

    private HitRisenState()
    {
    }

    public void OnEnter(Jormungandr iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[10], 0.5f, false);
      if ((double) iOwner.mAnimationController.Time > 1.1000000238418579)
        iOwner.mAnimationController.Stop();
      AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[8]);
    }

    public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      if (!iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled || NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Jormungandr.ChangeStateMessage changeStateMessage;
        changeStateMessage.NewState = Jormungandr.State.Risen;
        changeStateMessage.AnimationBlendTime = 0.6f;
        changeStateMessage.Animation = Jormungandr.Animations.Submerge;
        changeStateMessage.AnimationLooped = false;
        BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
      }
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[5], 0.6f, false);
      iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.RisenState.Instance);
    }

    public void OnExit(Jormungandr iOwner) => iOwner.mIsHit = false;
  }

  protected class HitLeapState : IBossState<Jormungandr>
  {
    private static Jormungandr.HitLeapState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Jormungandr.HitLeapState Instance
    {
      get
      {
        if (Jormungandr.HitLeapState.mSingelton == null)
        {
          lock (Jormungandr.HitLeapState.mSingeltonLock)
          {
            if (Jormungandr.HitLeapState.mSingelton == null)
              Jormungandr.HitLeapState.mSingelton = new Jormungandr.HitLeapState();
          }
        }
        return Jormungandr.HitLeapState.mSingelton;
      }
    }

    private HitLeapState()
    {
    }

    public void OnEnter(Jormungandr iOwner)
    {
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[3], 0.2f, false);
      AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[8]);
    }

    public unsafe void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled || (double) iOwner.mAnimationController.Time / (double) iOwner.mAnimationClips[3].Duration <= 0.66600000858306885)
        return;
      iOwner.mIdleTimer -= iDeltaTime;
      Matrix result;
      Matrix.CreateRotationX(MathHelper.ToRadians(275f / iOwner.mAnimationController.AnimationClip.Duration * iDeltaTime), out result);
      Matrix.Multiply(ref result, ref iOwner.mOrientation, out iOwner.mOrientation);
      if ((double) iOwner.mIdleTimer > (double) Jormungandr.WARNINGTIME || NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Jormungandr.ChangeStateMessage changeStateMessage;
        changeStateMessage.NewState = Jormungandr.State.Underground;
        changeStateMessage.Animation = Jormungandr.Animations.Invalid;
        BossFight.Instance.SendMessage<Jormungandr.ChangeStateMessage>((IBoss) iOwner, (ushort) 1, (void*) &changeStateMessage, true);
      }
      iOwner.ChangeState((IBossState<Jormungandr>) Jormungandr.UndergroundState.Instance);
    }

    public void OnExit(Jormungandr iOwner)
    {
      iOwner.mIsHit = false;
      iOwner.mAnimationController.Speed = 1f;
    }
  }

  protected class DeadState : IBossState<Jormungandr>
  {
    private static Jormungandr.DeadState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Jormungandr.DeadState Instance
    {
      get
      {
        if (Jormungandr.DeadState.mSingelton == null)
        {
          lock (Jormungandr.DeadState.mSingeltonLock)
          {
            if (Jormungandr.DeadState.mSingelton == null)
              Jormungandr.DeadState.mSingelton = new Jormungandr.DeadState();
          }
        }
        return Jormungandr.DeadState.mSingelton;
      }
    }

    private DeadState()
    {
    }

    public void OnEnter(Jormungandr iOwner)
    {
      iOwner.mDraw = true;
      iOwner.mUpdateAnimation = true;
      iOwner.mOrientation.Decompose(out Vector3 _, out iOwner.mSavedRotation, out iOwner.mSavedPosition);
      iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[(int) iOwner.mDeathAnimation], false);
      iOwner.mDamageState = Jormungandr.DamageState.None;
      AudioManager.Instance.PlayCue(Banks.Characters, Jormungandr.JORMUNGANDRSOUNDS[0]);
      EffectManager.Instance.Stop(ref iOwner.mEarthSplashReference);
      EffectManager.Instance.Stop(ref iOwner.mEarthTremorReference);
      iOwner.mIdleTimer = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Jormungandr iOwner)
    {
      Matrix oOrientation;
      Jormungandr.CreateTransform(ref iOwner.mPosition, ref iOwner.mDirection, out oOrientation);
      if (!iOwner.mAnimationController.CrossFadeEnabled)
      {
        iOwner.mOrientation = oOrientation;
      }
      else
      {
        Quaternion quaternion;
        Vector3 vector3;
        oOrientation.Decompose(out Vector3 _, out quaternion, out vector3);
        float amount = iOwner.mAnimationController.CrossFadeTime / iOwner.mAnimationController.CrossFadeDuration;
        Quaternion.Lerp(ref iOwner.mSavedRotation, ref quaternion, amount, out quaternion);
        Vector3.Lerp(ref iOwner.mSavedPosition, ref vector3, amount, out vector3);
        Matrix.CreateFromQuaternion(ref quaternion, out iOwner.mOrientation);
        iOwner.mOrientation.Translation = vector3;
      }
      if (!iOwner.mAnimationController.HasFinished || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      iOwner.mDead = true;
    }

    public void OnExit(Jormungandr iOwner)
    {
    }
  }

  private enum MessageType : ushort
  {
    Update,
    ChangeState,
    ChangeTarget,
    Spit,
  }

  internal struct UpdateMessage
  {
    public const ushort TYPE = 0;
    public Vector3 Position;
    public Vector3 Direction;
    public Matrix Orientation;
    public byte Animation;
    public float AnimationTime;
    public float Hitpoints;
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 1;
    public float AnimationBlendTime;
    public bool AnimationLooped;
    public Jormungandr.State NewState;
    public Jormungandr.Animations Animation;
  }

  internal struct ChangeTargetMessage
  {
    public const ushort TYPE = 2;
    public ushort Target;
  }

  internal struct SpitMessage
  {
    public const ushort TYPE = 3;
    public ushort Handle;
    public Vector3 Velocity;
    public Vector3 Position;
  }

  public enum Animations : byte
  {
    Invalid,
    Intro,
    Leap,
    Leap_Hit,
    Rise,
    Submerge,
    Idle,
    Spit,
    Venom,
    Bite,
    Risen_Hit,
    Die_Below,
    Die_Above,
    NrOfAnimations,
  }

  public enum DamageState
  {
    None,
    Leap,
    Emerge,
    Bite,
  }

  public enum TargettingType
  {
    Random,
    Distance,
    Angle,
    PlayerStrain,
  }

  public enum Effects
  {
    Splash,
    Tremor,
    SplashTremor,
    SpitSpray,
    SpitSplash,
    SpitProjectile,
  }

  private enum Sounds
  {
    Death,
    WakeUp,
    Dive,
    Unburrow,
    Leap,
    Rise,
    Spit,
    Bite,
    Pain,
    PreSpit,
    PreBite,
    LeapDive,
    HitObject,
    Whip,
    Prowl,
  }
}
