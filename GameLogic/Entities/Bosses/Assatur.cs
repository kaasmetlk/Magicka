// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Assatur
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Assatur : BossStatusEffected, IBossSpellCaster, IBoss
{
  private const float NETWORK_UPDATE_PERIOD = 0.0333333351f;
  private const float MISSILES_COOLDOWN = 0.25f;
  private const float MISSILE_RADIUS = 0.25f;
  private const float MISSILE_DAMAGE_AMOUNT = 100f;
  private const float MISSILE_DAMAGE_MAGNITUDE = 1f;
  private const float MAXHITPOINTS = 50000f;
  private float mLastNetworkUpdate;
  protected float mNetworkUpdateTimer;
  private static readonly int DIALOG_END = "assaturdeath".GetHashCodeCustom();
  private static readonly Vector3 DIALOG_OFFSET = new Vector3(-3f, 3f, 0.0f);
  private static readonly Vector3 MISSILE_START_VELOCITY = new Vector3(0.0f, 0.0f, -30f);
  private static readonly Random RANDOM = new Random();
  private static readonly int ASSATUR_DEFEATED_TRIGGER = "assatur_defeated".GetHashCodeCustom();
  private static readonly int ASSATUR_KILLED_TRIGGER = "assatur_killed".GetHashCodeCustom();
  private static readonly int LAZER_EFFECT = "assatur_missile_explode".GetHashCodeCustom();
  private static readonly int BLIZZARD_EFFECT = "assatur_blizzard".GetHashCodeCustom();
  private static readonly int LIFE_EFFECT = "assatur_life".GetHashCodeCustom();
  private static readonly int MISSILE_EXPLODE = "assatur_missile_explode".GetHashCodeCustom();
  private static readonly int MISSILE_TRAIL = "assatur_missile_trail".GetHashCodeCustom();
  private static readonly int NULLIFY_EFFECT = "assatur_nullify".GetHashCodeCustom();
  private static readonly int GENERIC_MAGICK = "magick_generic".GetHashCodeCustom();
  private static readonly int BLOOD_BLACK_EFFECT = "gore_splash_black".GetHashCodeCustom();
  private static readonly int BARRIER_LOCATOR = "assatur_chasm".GetHashCodeCustom();
  private static readonly int[] VORTEX_LOCATORS = new int[4]
  {
    "assatur_vortex1".GetHashCodeCustom(),
    "assatur_vortex2".GetHashCodeCustom(),
    "assatur_vortex3".GetHashCodeCustom(),
    "assatur_vortex4".GetHashCodeCustom()
  };
  private bool mDead;
  private static readonly Random sRandom = new Random();
  private static readonly Grimnir2.SpellData[] SPELLS = new Grimnir2.SpellData[3];
  private HitList mHitList;
  private VisualEffectReference mCurrentEffect;
  private ConditionCollection mMissileConditions;
  private List<MissileEntity> mMissiles;
  private Magicka.GameLogic.Entities.Character mTarget;
  private float mTargettingPower;
  private Vector3 mTargetPosition;
  private float mCastTimeCooldown;
  private SpellEffect mSpellEffect;
  private float mSpellPower;
  private int mTentaclesIndex = 44;
  private int mTentacleBones = 34;
  private static float[][] ANIMATION_TIMERS = new float[17][]
  {
    new float[0],
    new float[2]{ 0.3939394f, 0.8484849f },
    new float[2]{ 0.3939394f, 0.8484849f },
    new float[2]{ 5f / 16f, 0.625f },
    new float[1]{ 1f },
    new float[1]{ 0.625f },
    new float[2]{ 13f / 32f, 1f },
    new float[2]{ 0.166666672f, 1f },
    new float[1]{ 0.5f },
    new float[2]{ 0.0f, 1f },
    new float[1]{ 0.3488372f },
    new float[4]{ 0.25f, 0.675f, 0.45f, 0.55f },
    new float[1]{ 0.25f },
    new float[4]{ 0.1f, 0.15f, 0.9f, 0.95f },
    new float[1]{ 0.25f },
    new float[0],
    new float[0]
  };
  private BindJoint mLeftHandJoint;
  private BindJoint mRightHandJoint;
  private BindJoint mDamageJoint;
  private BindJoint mStaffJoint;
  private AnimationController mController;
  private AnimationController mTentacleController;
  private AnimationClip[] mClips;
  private Matrix mMoveLoc0;
  private Matrix mMoveLoc1;
  private Matrix mChasmOrientation;
  private Vector3[] mVortexPositions;
  private Matrix[] mStartOrientations;
  private Matrix mOrientation;
  private Matrix mTargetOrientation;
  private Matrix mSpineOrientation;
  private Assatur.AssaturState[] mStates;
  private Assatur.AssaturState mCurrentState;
  private Assatur.AssaturState mLastState;
  private Matrix mRailStaffOrientation;
  private bool mUseRailStaffOrientation;
  private Assatur.RenderData[] mRenderData;
  private Assatur.StaffRenderData[] mStaffRenderData;
  private BossSpellCasterZone mBodyZone;
  private PlayState mPlayState;
  private float mAssaturFloatTimer;
  private Shield mProtectiveShield;
  private float mShieldTimer;
  private float mDamageFlashTimer;
  private BoundingSphere mBoundingSphere;
  private Matrix mMagickEffect;
  private float mDeathScale;
  private SkinnedModelDeferredAdvancedMaterial mMaterial;
  private TextureCube mIceCubeMap;
  private TextureCube mIceCubeNormalMap;

  static Assatur()
  {
    Grimnir2.SpellData spellData = new Grimnir2.SpellData()
    {
      SPELL = new Spell()
    };
    spellData.SPELL.Element = Elements.Earth | Elements.Fire | Elements.Shield;
    spellData.SPELL.EarthMagnitude = 1f;
    spellData.SPELL.ShieldMagnitude = 1f;
    spellData.SPELL.FireMagnitude = 3f;
    spellData.CASTTYPE = Magicka.GameLogic.Spells.CastType.Weapon;
    Assatur.SPELLS[0] = spellData;
    spellData.SPELL = new Spell();
    spellData.SPELL.Element = Elements.Shield | Elements.Ice;
    spellData.SPELL.ShieldMagnitude = 1f;
    spellData.SPELL.IceMagnitude = 4f;
    spellData.CASTTYPE = Magicka.GameLogic.Spells.CastType.Force;
    spellData.SPELLPOWER = 1f;
    Assatur.SPELLS[1] = spellData;
    spellData.SPELL = new Spell();
    spellData.SPELL.Element = Elements.Shield;
    spellData.SPELL.ShieldMagnitude = 1f;
    spellData.CASTTYPE = Magicka.GameLogic.Spells.CastType.Force;
    spellData.SPELLPOWER = 1f;
    Assatur.SPELLS[2] = spellData;
  }

  public Assatur(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mHitList = new HitList(32 /*0x20*/);
    this.mMissiles = new List<MissileEntity>(8);
    SkinnedModel skinnedModel;
    Model model;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      skinnedModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/Assatur/Assatur");
      model = this.mPlayState.Content.Load<Model>("Models/Bosses/Assatur/Assatur_staff");
      this.mIceCubeMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube");
      this.mIceCubeNormalMap = this.mPlayState.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
    }
    this.mController = new AnimationController();
    this.mController.Skeleton = skinnedModel.SkeletonBones;
    this.mTentacleController = new AnimationController();
    this.mTentacleController.Skeleton = skinnedModel.SkeletonBones;
    this.mClips = new AnimationClip[18];
    this.mClips[11] = skinnedModel.AnimationClips["cast_blade"];
    this.mClips[8] = skinnedModel.AnimationClips["cast_blizzard"];
    this.mClips[10] = skinnedModel.AnimationClips["cast_ground"];
    this.mClips[15] = skinnedModel.AnimationClips["idle"];
    this.mClips[7] = skinnedModel.AnimationClips["cast_self"];
    this.mClips[6] = skinnedModel.AnimationClips["cast_emperor"];
    this.mClips[5] = skinnedModel.AnimationClips["cast_sky"];
    this.mClips[1] = skinnedModel.AnimationClips["cast_mm_left"];
    this.mClips[2] = skinnedModel.AnimationClips["cast_mm_right"];
    this.mClips[3] = skinnedModel.AnimationClips["cast_mm_sides"];
    this.mClips[0] = skinnedModel.AnimationClips["cast_mm_prepare"];
    this.mClips[14] = skinnedModel.AnimationClips["cast_nullify"];
    this.mClips[12] = skinnedModel.AnimationClips["cast_shield"];
    this.mClips[13] = skinnedModel.AnimationClips["cast_staff"];
    this.mClips[9] = skinnedModel.AnimationClips["cast_spray"];
    this.mClips[4] = skinnedModel.AnimationClips["cast_focus"];
    this.mClips[16 /*0x10*/] = skinnedModel.AnimationClips["tentacles"];
    this.mClips[17] = skinnedModel.AnimationClips["die"];
    this.mStates = new Assatur.AssaturState[17];
    this.mStates[9] = (Assatur.AssaturState) new Assatur.CastBlade();
    this.mStates[6] = (Assatur.AssaturState) new Assatur.CastBlizzard();
    this.mStates[8] = (Assatur.AssaturState) new Assatur.CastTimeWarp();
    this.mStates[5] = (Assatur.AssaturState) new Assatur.CastLife();
    this.mStates[4] = (Assatur.AssaturState) new Assatur.CastLightning();
    this.mStates[1] = (Assatur.AssaturState) new Assatur.CastMMState();
    this.mStates[3] = (Assatur.AssaturState) new Assatur.CastMeteor();
    this.mStates[12] = (Assatur.AssaturState) new Assatur.CastNullifyState();
    this.mStates[10] = (Assatur.AssaturState) new Assatur.CastShield();
    this.mStates[11] = (Assatur.AssaturState) new Assatur.CastStaffBeamState();
    this.mStates[7] = (Assatur.AssaturState) new Assatur.CastWater();
    this.mStates[2] = (Assatur.AssaturState) new Assatur.CastVortex();
    this.mStates[13] = (Assatur.AssaturState) new Assatur.CastBarrier();
    this.mStates[14] = (Assatur.AssaturState) new Assatur.IntroState();
    this.mStates[0] = (Assatur.AssaturState) new Assatur.BattleState();
    this.mStates[15] = (Assatur.AssaturState) new Assatur.DeathState();
    int iWeaponBoneIndex = 0;
    int iCastBoneIndex = 0;
    Matrix result;
    Matrix.CreateRotationY(3.14159274f, out result);
    foreach (SkinnedModelBone skeletonBone in (ReadOnlyCollection<SkinnedModelBone>) skinnedModel.SkeletonBones)
    {
      if (skeletonBone.Name.Equals("SpineLower", StringComparison.OrdinalIgnoreCase))
      {
        this.mDamageJoint.mIndex = (int) skeletonBone.Index;
        this.mDamageJoint.mBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mDamageJoint.mBindPose, ref result, out this.mDamageJoint.mBindPose);
        Matrix.Invert(ref this.mDamageJoint.mBindPose, out this.mDamageJoint.mBindPose);
      }
      else if (skeletonBone.Name.Equals("joint1", StringComparison.OrdinalIgnoreCase))
      {
        this.mStaffJoint.mIndex = (int) skeletonBone.Index;
        this.mStaffJoint.mBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Invert(ref this.mStaffJoint.mBindPose, out this.mStaffJoint.mBindPose);
        Matrix.Multiply(ref result, ref this.mStaffJoint.mBindPose, out this.mStaffJoint.mBindPose);
      }
      else if (skeletonBone.Name.Equals("RightAttach", StringComparison.OrdinalIgnoreCase))
      {
        iWeaponBoneIndex = (int) skeletonBone.Index;
        this.mRightHandJoint.mIndex = (int) skeletonBone.Index;
        this.mRightHandJoint.mBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Invert(ref this.mRightHandJoint.mBindPose, out this.mRightHandJoint.mBindPose);
        Matrix.Multiply(ref result, ref this.mRightHandJoint.mBindPose, out this.mRightHandJoint.mBindPose);
      }
      else if (skeletonBone.Name.Equals("LeftAttach", StringComparison.OrdinalIgnoreCase))
      {
        iCastBoneIndex = (int) skeletonBone.Index;
        this.mLeftHandJoint.mIndex = (int) skeletonBone.Index;
        this.mLeftHandJoint.mBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Invert(ref this.mLeftHandJoint.mBindPose, out this.mLeftHandJoint.mBindPose);
        Matrix.Multiply(ref result, ref this.mLeftHandJoint.mBindPose, out this.mLeftHandJoint.mBindPose);
      }
    }
    Magicka.GameLogic.Damage iDamage = new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Arcane, 100f, 1f);
    this.mMissileConditions = new ConditionCollection();
    this.mMissileConditions[0].Condition.EventConditionType = EventConditionType.Default;
    this.mMissileConditions[0].Condition.Repeat = true;
    this.mMissileConditions[0].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_TRAIL, true)));
    this.mMissileConditions[1].Condition.EventConditionType = EventConditionType.Hit;
    this.mMissileConditions[1].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_EXPLODE)));
    this.mMissileConditions[1].Add(new EventStorage(new RemoveEvent()));
    this.mMissileConditions[1].Add(new EventStorage(new DamageEvent(iDamage)));
    this.mMissileConditions[2].Condition.EventConditionType = EventConditionType.Collision;
    this.mMissileConditions[2].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_EXPLODE)));
    this.mMissileConditions[2].Add(new EventStorage(new RemoveEvent()));
    this.mMissileConditions[2].Add(new EventStorage(new DamageEvent(iDamage)));
    this.mMissileConditions[3].Condition.EventConditionType = EventConditionType.Timer;
    this.mMissileConditions[3].Condition.Time = 5f;
    this.mMissileConditions[3].Add(new EventStorage(new RemoveEvent()));
    this.mMissileConditions[4].Condition.EventConditionType = EventConditionType.Damaged;
    this.mMissileConditions[4].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_EXPLODE)));
    this.mMissileConditions[4].Add(new EventStorage(new RemoveEvent()));
    Capsule capsule = new Capsule(new Vector3(0.0f, -0.75f, 0.0f), Matrix.CreateRotationX(-1.57079637f), 1.5f, 5f);
    this.mBodyZone = new BossSpellCasterZone(iPlayState, (IBossSpellCaster) this, this.mController, iCastBoneIndex, iWeaponBoneIndex, 0, 1.5f, new Primitive[1]
    {
      (Primitive) capsule
    });
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(skinnedModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
    this.mBoundingSphere = skinnedModel.Model.Meshes[0].BoundingSphere;
    this.mRenderData = new Assatur.RenderData[3];
    this.mStaffRenderData = new Assatur.StaffRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new Assatur.RenderData();
      this.mRenderData[index].SetMesh(skinnedModel.Model.Meshes[0].VertexBuffer, skinnedModel.Model.Meshes[0].IndexBuffer, skinnedModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
      this.mRenderData[index].mMaterial = this.mMaterial;
      this.mStaffRenderData[index] = new Assatur.StaffRenderData();
      this.mStaffRenderData[index].SetMesh(model.Meshes[0], model.Meshes[0].MeshParts[0], 4, 0, 5);
    }
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mDead = false;
    this.mAssaturFloatTimer = 0.0f;
    this.mOrientation = iOrientation;
    this.mHitPoints = 50000f;
    for (int iIndex = 0; iIndex < this.mResistances.Length; ++iIndex)
    {
      this.mResistances[iIndex].ResistanceAgainst = Spell.ElementFromIndex(iIndex);
      this.mResistances[iIndex].Modifier = 0.0f;
      if (this.mResistances[iIndex].ResistanceAgainst == Elements.Water)
        this.mResistances[iIndex].Multiplier = 0.0f;
      else if (this.mResistances[iIndex].ResistanceAgainst == Elements.Cold)
        this.mResistances[iIndex].Multiplier = 0.05f;
      else
        this.mResistances[iIndex].Multiplier = 1f;
    }
    this.mDeathScale = 1f;
    this.mVortexPositions = new Vector3[Assatur.VORTEX_LOCATORS.Length];
    this.mStartOrientations = new Matrix[4];
    Locator oLocator;
    this.mPlayState.Level.CurrentScene.GetLocator("start0".GetHashCodeCustom(), out oLocator);
    this.mChasmOrientation = oLocator.Transform;
    this.mVortexPositions[0] = oLocator.Transform.Translation;
    this.mStartOrientations[0] = oLocator.Transform;
    this.mPlayState.Level.CurrentScene.GetLocator("start1".GetHashCodeCustom(), out oLocator);
    this.mVortexPositions[1] = oLocator.Transform.Translation;
    this.mStartOrientations[1] = oLocator.Transform;
    this.mPlayState.Level.CurrentScene.GetLocator("start2".GetHashCodeCustom(), out oLocator);
    this.mVortexPositions[2] = oLocator.Transform.Translation;
    this.mStartOrientations[2] = oLocator.Transform;
    this.mPlayState.Level.CurrentScene.GetLocator("start3".GetHashCodeCustom(), out oLocator);
    this.mVortexPositions[3] = oLocator.Transform.Translation;
    this.mStartOrientations[3] = oLocator.Transform;
    this.mPlayState.Level.CurrentScene.GetLocator("assatur_move0".GetHashCodeCustom(), out oLocator);
    this.mMoveLoc0 = oLocator.Transform;
    this.mPlayState.Level.CurrentScene.GetLocator("assatur_move1".GetHashCodeCustom(), out oLocator);
    this.mMoveLoc1 = oLocator.Transform;
    this.mBodyZone.Initialize("#boss_n15".GetHashCodeCustom());
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mBodyZone);
    this.mCurrentState = this.mStates[14];
    this.mCurrentState.OnEnter(this);
    this.mMaxHitPoints = 50000f;
    this.mHitPoints = this.MaxHitPoints;
    this.mTarget = (Magicka.GameLogic.Entities.Character) Magicka.Game.Instance.Players[0].Avatar;
    this.mTargettingPower = 1f;
    this.mTargetOrientation = this.mOrientation;
    Vector3 translation = this.mOrientation.Translation;
    translation.Y += 40f;
    this.mOrientation.Translation = translation;
    this.mLastState = this.mCurrentState;
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mNetworkUpdateTimer -= iDeltaTime;
      if ((double) this.mNetworkUpdateTimer <= 0.0)
      {
        this.mNetworkUpdateTimer = 0.0333333351f;
        this.NetworkUpdate();
      }
    }
    iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
    this.UpdateDamage(iDeltaTime);
    this.UpdateStatusEffects(iDeltaTime);
    this.mShieldTimer -= iDeltaTime;
    if ((double) this.mHitPoints <= 0.0 && !(this.mCurrentState is Assatur.DeathState))
      this.ChangeState(Assatur.States.Dead);
    Vector3 translation1 = this.mOrientation.Translation;
    translation1.Y += (float) (((double) this.mTargetOrientation.Translation.Y - (double) this.mOrientation.Translation.Y) * (double) iDeltaTime * 2.0);
    this.mOrientation.Translation = translation1;
    this.mAssaturFloatTimer += iDeltaTime;
    if (this.mTarget != null)
    {
      Vector3 position = this.mTarget.Position;
      this.mTargetPosition.X += (position.X - this.mTargetPosition.X) * iDeltaTime * this.mTargettingPower;
      this.mTargetPosition.Y += (position.Y - this.mTargetPosition.Y) * iDeltaTime * this.mTargettingPower;
      this.mTargetPosition.Z += (position.Z - this.mTargetPosition.Z) * iDeltaTime * this.mTargettingPower;
    }
    Matrix mOrientation = this.mOrientation;
    mOrientation.Translation -= new Vector3(0.0f, 5f, 0.0f);
    if (this.mSpellEffect != null)
    {
      if (this.mSpellEffect.CastType == Magicka.GameLogic.Spells.CastType.Weapon)
        this.mSpellEffect.AnimationEnd((ISpellCaster) this.mBodyZone);
      if (!this.mSpellEffect.CastUpdate(iDeltaTime, (ISpellCaster) this.mBodyZone, out float _))
      {
        this.mSpellEffect.DeInitialize((ISpellCaster) this.mBodyZone);
        this.mSpellEffect = (SpellEffect) null;
      }
    }
    if ((double) this.mShieldTimer <= 0.0 && this.mProtectiveShield != null && !this.mProtectiveShield.Dead)
    {
      this.mShieldTimer += 0.2f;
      this.mProtectiveShield.Damage(100f);
    }
    this.mMagickEffect = this.mSpineOrientation;
    this.mMagickEffect.Translation = new Vector3(this.mMagickEffect.Translation.X, this.mMagickEffect.Translation.Y + 3f, this.mMagickEffect.Translation.Z + 3f);
    this.mSpineOrientation = this.mDamageJoint.mBindPose;
    Matrix.Multiply(ref this.mSpineOrientation, ref this.mController.SkinnedBoneTransforms[this.mDamageJoint.mIndex], out this.mSpineOrientation);
    if (iFightStarted)
      this.mCurrentState.OnUpdate(iDeltaTime, this);
    this.mController.Speed = 1f;
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
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapColor.W = 1f - (float) Math.Pow(0.20000000298023224, (double) this.StatusMagnitude(StatusEffects.Frozen));
      this.mController.Speed = 0.0f;
    }
    else
    {
      if (this.HasStatus(StatusEffects.Cold))
        this.mController.Speed *= 0.2f;
      this.mRenderData[(int) iDataChannel].mMaterial.Bloat = 0.0f;
      this.mRenderData[(int) iDataChannel].mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularBias = this.mMaterial.SpecularBias;
      this.mRenderData[(int) iDataChannel].mMaterial.SpecularPower = this.mMaterial.SpecularPower;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeMapEnabled = false;
      this.mRenderData[(int) iDataChannel].mMaterial.CubeNormalMapEnabled = false;
    }
    Matrix result1;
    Matrix.Multiply(ref mOrientation, this.mDeathScale, out result1);
    result1.Translation = mOrientation.Translation;
    this.mController.PreUpdate(iDeltaTime, ref result1);
    this.mTentacleController.PreUpdate(iDeltaTime, ref result1);
    Array.Copy((Array) this.mTentacleController.LocalBonePoses, this.mTentaclesIndex, (Array) this.mController.LocalBonePoses, this.mTentaclesIndex, this.mTentacleBones);
    this.mController.UpdateAbsoluteBoneTransforms(ref result1);
    this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0.0f);
    Array.Copy((Array) this.mController.SkinnedBoneTransforms, (Array) this.mRenderData[(int) iDataChannel].mBones, this.mRenderData[(int) iDataChannel].mBones.Length);
    this.mBoundingSphere.Center = this.mOrientation.Translation;
    this.mRenderData[(int) iDataChannel].Flash = this.mDamageFlashTimer * 10f;
    this.mRenderData[(int) iDataChannel].mBoundingSphere = this.mBoundingSphere;
    this.mRenderData[(int) iDataChannel].Damage = (float) (1.0 - (double) this.mHitPoints / 50000.0);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
    Matrix result2 = this.mStaffJoint.mBindPose;
    Matrix.Multiply(ref result2, ref this.mController.SkinnedBoneTransforms[this.mStaffJoint.mIndex], out result2);
    this.mStaffRenderData[(int) iDataChannel].WorldOrientation = !this.mUseRailStaffOrientation ? result2 : this.mRailStaffOrientation;
    this.mStaffRenderData[(int) iDataChannel].mBoundingSphere = this.mBoundingSphere;
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mStaffRenderData[(int) iDataChannel]);
    this.mBodyZone.Body.MoveTo(this.mOrientation.Translation, this.mOrientation with
    {
      Translation = new Vector3()
    });
    Matrix result3 = this.mDamageJoint.mBindPose;
    Matrix.Multiply(ref result3, ref this.mController.SkinnedBoneTransforms[this.mDamageJoint.mIndex], out result3);
    Vector3 translation2 = result3.Translation;
    if (this.mProtectiveShield != null)
    {
      if (this.mProtectiveShield.Dead)
      {
        this.mProtectiveShield = (Shield) null;
      }
      else
      {
        result3 = this.mOrientation;
        Vector3 translation3 = this.mSpineOrientation.Translation;
        result3.Translation = new Vector3();
        this.mProtectiveShield.Body.MoveTo(translation3, result3);
      }
    }
    for (int index = 0; index < this.mMissiles.Count; ++index)
    {
      if (this.mMissiles[index].Dead)
        this.mMissiles.RemoveAt(index--);
    }
  }

  public Matrix StaffOrientation()
  {
    Matrix result = this.mStaffJoint.mBindPose;
    Matrix.Multiply(ref result, ref this.mController.SkinnedBoneTransforms[this.mStaffJoint.mIndex], out result);
    return result;
  }

  public unsafe void ChangeState(Assatur.States iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Assatur.ChangeStateMessage changeStateMessage;
      changeStateMessage.NewState = iState;
      BossFight.Instance.SendMessage<Assatur.ChangeStateMessage>((IBoss) this, (ushort) 1, (void*) &changeStateMessage, true);
    }
    this.mCurrentState.OnExit(this);
    this.mLastState = this.mCurrentState;
    this.mCurrentState = this.mStates[(int) iState];
    this.mCurrentState.OnEnter(this);
  }

  private unsafe void SelectTarget()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Player[] players = Magicka.Game.Instance.Players;
    int num = Assatur.sRandom.Next(4);
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[(index + num) % 4].Playing)
      {
        Player player = players[(index + num) % 4];
        if (player.Avatar != null && !player.Avatar.Dead)
        {
          this.mTarget = (Magicka.GameLogic.Entities.Character) player.Avatar;
          break;
        }
      }
    }
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    Assatur.ChangeTargetMessage changeTargetMessage;
    changeTargetMessage.Target = this.mTarget == null ? ushort.MaxValue : this.mTarget.Handle;
    BossFight.Instance.SendMessage<Assatur.ChangeTargetMessage>((IBoss) this, (ushort) 2, (void*) &changeTargetMessage, true);
  }

  public void DeInitialize()
  {
  }

  public override bool Dead => this.mDead;

  public float MaxHitPoints => 50000f;

  public float HitPoints => this.mHitPoints;

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return false;
  }

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Magicka.GameLogic.Entities.Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if (iAttacker is BossCollisionZone)
      return DamageResult.None;
    DamageResult damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
    if ((damageResult & DamageResult.Hit) == DamageResult.Hit | (damageResult & DamageResult.Damaged) == DamageResult.Damaged)
    {
      if (this.mCurrentState is Assatur.CastLife)
        this.ChangeState(Assatur.States.Battle);
      this.mDamageFlashTimer = 0.1f;
    }
    return damageResult;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
    this.Damage(iDamage, iElement);
  }

  public void ScriptMessage(BossMessages iMessage)
  {
    if (iMessage != BossMessages.KillAssatur)
      return;
    this.ChangeState(Assatur.States.Dead);
  }

  public void SetSlow(int iIndex)
  {
  }

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    oPosition = new Vector3();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => false;

  public float StatusMagnitude(int iIndex, StatusEffects iStatus) => 0.0f;

  public new StatusEffect[] GetStatusEffects() => (StatusEffect[]) null;

  public void AddSelfShield(int iIndex, Spell iSpell)
  {
  }

  public void RemoveSelfShield(int iIndex, Magicka.GameLogic.Entities.Character.SelfShieldType iType)
  {
  }

  public Magicka.GameLogic.Spells.CastType CastType(int iIndex) => Magicka.GameLogic.Spells.CastType.None;

  public float SpellPower(int iIndex) => this.mSpellPower;

  public void SpellPower(int iIndex, float iSpellPower) => this.mSpellPower = iSpellPower;

  public SpellEffect CurrentSpell(int iIndex) => this.mSpellEffect;

  public void CurrentSpell(int iIndex, SpellEffect iEffect) => this.mSpellEffect = iEffect;

  protected override BossDamageZone Entity => (BossDamageZone) this.mBodyZone;

  protected override float Radius => this.mBodyZone.Radius;

  protected override float Length => this.mBodyZone.Radius * 1.5f;

  protected override int BloodEffect => Assatur.BLOOD_BLACK_EFFECT;

  protected override Vector3 NotifierTextPostion
  {
    get
    {
      Vector3 translation = this.mSpineOrientation.Translation;
      translation.Y += this.mBodyZone.Radius * 1.2f;
      return translation;
    }
  }

  private unsafe void NetworkUpdate()
  {
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer))
      return;
    Assatur.UpdateMessage updateMessage = new Assatur.UpdateMessage();
    updateMessage.Animation = (byte) 0;
    while ((int) updateMessage.Animation < this.mClips.Length && this.mController.AnimationClip != this.mClips[(int) updateMessage.Animation])
      ++updateMessage.Animation;
    updateMessage.AnimationTime = this.mController.Time;
    updateMessage.Hitpoints = this.mHitPoints;
    for (int index = 0; index < networkServer.Connections; ++index)
    {
      float num = networkServer.GetLatency(index) * 0.5f;
      updateMessage.AnimationTime += num;
      BossFight.Instance.SendMessage<Assatur.UpdateMessage>((IBoss) this, (ushort) 0, (void*) &updateMessage, false, index);
    }
  }

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    if (iMsg.Type == (ushort) 0)
    {
      if ((double) iMsg.TimeStamp < (double) this.mLastNetworkUpdate)
        return;
      this.mLastNetworkUpdate = (float) iMsg.TimeStamp;
      Assatur.UpdateMessage updateMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &updateMessage);
      if (this.mController.AnimationClip != this.mClips[(int) updateMessage.Animation])
        this.mController.StartClip(this.mClips[(int) updateMessage.Animation], false);
      this.mController.Time = updateMessage.AnimationTime;
      this.mHitPoints = updateMessage.Hitpoints;
    }
    else if (iMsg.Type == (ushort) 5)
    {
      Assatur.CastShieldMessage castShieldMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &castShieldMessage);
      this.mProtectiveShield = Magicka.GameLogic.Entities.Entity.GetFromHandle((int) castShieldMessage.Handle) as Shield;
      this.mProtectiveShield.Initialize((ISpellCaster) this.mBodyZone, castShieldMessage.Position, castShieldMessage.Radius, castShieldMessage.Direction, castShieldMessage.ShieldType, castShieldMessage.HitPoints, Spell.SHIELDCOLOR);
      this.mProtectiveShield.HitPoints = 5000f;
      this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mProtectiveShield);
      this.mShieldTimer = 15f;
      (this.mStates[10] as Assatur.CastShield).Shielded = true;
    }
    else if (iMsg.Type == (ushort) 4)
    {
      Assatur.CastBarrierMessage castBarrierMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &castBarrierMessage);
    }
    else if (iMsg.Type == (ushort) 3)
    {
      Assatur.CastMagickMessage castMagickMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &castMagickMessage);
      new Magick() { MagickType = castMagickMessage.Magick }.Effect.Execute((ISpellCaster) this.mBodyZone, this.mPlayState);
      if (castMagickMessage.Effect == 0)
        return;
      EffectManager.Instance.StartEffect(castMagickMessage.Effect, ref castMagickMessage.Position, ref castMagickMessage.Direction, out VisualEffectReference _);
    }
    else if (iMsg.Type == (ushort) 6)
    {
      Assatur.SpawnMMMessage spawnMmMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &spawnMmMessage);
      switch (spawnMmMessage.CastDir)
      {
        case 1:
          Matrix iOrientation = this.mBodyZone.CastSource;
          (this.mStates[1] as Assatur.CastMMState).SpawnMissile(ref iOrientation, this, -0.75f);
          iOrientation = this.mBodyZone.WeaponSource;
          (this.mStates[1] as Assatur.CastMMState).SpawnMissile(ref iOrientation, this, -0.75f);
          break;
        case 2:
          Matrix castSource = this.mBodyZone.CastSource;
          (this.mStates[1] as Assatur.CastMMState).SpawnMissile(ref castSource, this, -0.25f);
          break;
        case 3:
          Matrix weaponSource = this.mBodyZone.WeaponSource;
          (this.mStates[1] as Assatur.CastMMState).SpawnMissile(ref weaponSource, this, -0.25f);
          break;
      }
    }
    else if (iMsg.Type == (ushort) 2)
    {
      Assatur.ChangeTargetMessage changeTargetMessage;
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
      Assatur.ChangeStateMessage changeStateMessage;
      BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
      if (changeStateMessage.NewState == Assatur.States.None || changeStateMessage.NewState == Assatur.States.NrOfStates)
        return;
      this.mCurrentState.OnExit(this);
      this.mLastState = this.mCurrentState;
      this.mCurrentState = this.mStates[(int) changeStateMessage.NewState];
      this.mCurrentState.OnEnter(this);
    }
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  public BossEnum GetBossType() => BossEnum.Assatur;

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

  public class StaffRenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>
  {
    public Matrix WorldOrientation;

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      this.mMaterial.WorldTransform = this.WorldOrientation;
      base.Draw(iEffect, iViewFrustum);
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      this.mMaterial.WorldTransform = this.WorldOrientation;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }

  public class RenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
  {
    public float Flash;
    public Matrix[] mBones;
    public float Damage;

    public RenderData() => this.mBones = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
      modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
      modelDeferredEffect.Bones = this.mBones;
      this.mMaterial.Damage = this.Damage;
      base.Draw(iEffect, iViewFrustum);
      modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, 0.0f);
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.mBones;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }

  public interface AssaturState : IBossState<Assatur>
  {
    float GetWeight(Assatur iOwner);
  }

  public class IntroState : Assatur.AssaturState, IBossState<Assatur>
  {
    private float mTimer;

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.StartClip(iOwner.mClips[15], true);
      iOwner.mTentacleController.StartClip(iOwner.mClips[15], true);
      this.mTimer = 3f;
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if ((double) this.mTimer <= 0.0)
        iOwner.ChangeState(Assatur.States.Battle);
      this.mTimer -= iDeltaTime;
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner) => throw new NotImplementedException();
  }

  public class BattleState : Assatur.AssaturState, IBossState<Assatur>
  {
    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[15], 0.4f, true);
      iOwner.SelectTarget();
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      EntityManager entityManager = iOwner.mPlayState.EntityManager;
      float num1 = 0.0f;
      Assatur.States iState = Assatur.States.None;
      for (int index1 = 0; index1 < 16 /*0x10*/; ++index1)
      {
        float num2 = (float) MagickaMath.Random.NextDouble();
        if (!(iOwner.mLastState == iOwner.mStates[index1] | iOwner.mStates[index1] is Assatur.DeathState | iOwner.mStates[index1] is Assatur.BattleState))
        {
          switch (index1)
          {
            case 1:
              num2 += iOwner.mProtectiveShield == null || iOwner.mProtectiveShield.Dead ? 1f : 0.5f;
              break;
            case 2:
              num2 += SpellManager.Instance.IsEffectActive(typeof (Vortex)) ? -1f : 0.5f;
              break;
            case 3:
              num2 += SpellManager.Instance.IsEffectActive(typeof (MeteorShower)) ? -1f : 0.5f;
              break;
            case 4:
              num2 += 0.5f;
              break;
            case 5:
              num2 += Math.Max(0.0f, (float) ((1.0 - (double) iOwner.HitPoints / (double) iOwner.MaxHitPoints) * 2.0));
              break;
            case 6:
              num2 += SpellManager.Instance.IsEffectActive(typeof (Blizzard)) ? -1f : 0.5f;
              break;
            case 7:
              num2 -= 10f;
              break;
            case 8:
              num2 += (double) iOwner.mPlayState.TimeModifier < 1.0 ? -1f : 0.25f;
              break;
            case 9:
              List<Magicka.GameLogic.Entities.Entity> entities1 = entityManager.GetEntities(iOwner.mOrientation.Translation, 10f, true);
              int num3 = entities1.Count - 2;
              if (num3 > 0)
                num2 += 1f / (float) num3;
              entityManager.ReturnEntityList(entities1);
              break;
            case 10:
              num2 += (float) ((1.0 - (double) iOwner.HitPoints / (double) iOwner.MaxHitPoints) * 0.75 + 0.25);
              break;
            case 11:
              List<Magicka.GameLogic.Entities.Entity> entities2 = entityManager.GetEntities(iOwner.mOrientation.Translation, 50f, true);
              List<Magicka.GameLogic.Entities.Entity> entities3 = entityManager.GetEntities(iOwner.mOrientation.Translation, 50f, false);
              num2 += (float) (1.0 - (double) entities2.Count / (double) entities3.Count);
              entityManager.ReturnEntityList(entities2);
              entityManager.ReturnEntityList(entities3);
              break;
            case 12:
              int count = entityManager.Shields.Count;
              List<Magicka.GameLogic.Entities.Entity> entities4 = entityManager.GetEntities(iOwner.mOrientation.Translation, 50f, false);
              for (int index2 = 0; index2 < entities4.Count; ++index2)
              {
                if (entities4[index2] is Magicka.GameLogic.Entities.Character character && character.mSelfShield.Active & character.mSelfShield.mShieldType == Magicka.GameLogic.Entities.Character.SelfShieldType.Shield)
                  ++count;
              }
              num2 = num2 + ((float) count - 1f) - (iOwner.mProtectiveShield == null || iOwner.mProtectiveShield.Dead ? 0.0f : 1f);
              entityManager.ReturnEntityList(entities4);
              break;
          }
          if ((double) num2 > (double) num1)
          {
            num1 = num2;
            iState = (Assatur.States) index1;
          }
        }
      }
      if (iOwner.mStates[(int) iState] == null)
        return;
      iOwner.ChangeState(iState);
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner) => throw new NotImplementedException();
  }

  public class CastNullifyState : Assatur.AssaturState, IBossState<Assatur>
  {
    private bool mCastedMagick;

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[14], 0.3f, false);
      this.mCastedMagick = false;
    }

    public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mController.CrossFadeEnabled)
        return;
      Matrix iTransform = iOwner.StaffOrientation();
      EffectManager.Instance.UpdateOrientation(ref iOwner.mCurrentEffect, ref iTransform);
      if ((double) iOwner.mController.Time > 0.5 && !this.mCastedMagick)
      {
        EffectManager.Instance.StartEffect(Assatur.NULLIFY_EFFECT, ref iTransform, out iOwner.mCurrentEffect);
        iOwner.mPlayState.Camera.CameraShake(0.3f, 1.5f);
        this.mCastedMagick = true;
        new Magick() { MagickType = MagickType.Nullify }.Effect.Execute((ISpellCaster) iOwner.mBodyZone, iOwner.mPlayState);
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Assatur.CastMagickMessage castMagickMessage;
          castMagickMessage.Magick = MagickType.Nullify;
          castMagickMessage.Effect = Assatur.NULLIFY_EFFECT;
          castMagickMessage.Position = iTransform.Translation;
          castMagickMessage.Direction = iTransform.Forward;
          BossFight.Instance.SendMessage<Assatur.CastMagickMessage>((IBoss) iOwner, (ushort) 3, (void*) &castMagickMessage, true);
        }
      }
      if (!iOwner.mController.HasFinished)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner) => EffectManager.Instance.Stop(ref iOwner.mCurrentEffect);

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastNullifyState ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class CastBlizzard : Assatur.AssaturState, IBossState<Assatur>
  {
    private bool mCastedMagick;
    private VisualEffectReference mMagickEffect;

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[8], 0.4f, false);
      this.mCastedMagick = false;
      EffectManager.Instance.StartEffect(Assatur.GENERIC_MAGICK, ref iOwner.mMagickEffect, out this.mMagickEffect);
    }

    public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      EffectManager.Instance.UpdateOrientation(ref this.mMagickEffect, ref iOwner.mMagickEffect);
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mController.CrossFadeEnabled)
        return;
      if ((double) iOwner.mController.Time > 2.0 & !this.mCastedMagick)
      {
        this.mCastedMagick = true;
        Blizzard.Instance.Execute((ISpellCaster) iOwner.mBodyZone, iOwner.mPlayState);
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Assatur.CastMagickMessage castMagickMessage;
          castMagickMessage.Magick = MagickType.Blizzard;
          BossFight.Instance.SendMessage<Assatur.CastMagickMessage>((IBoss) iOwner, (ushort) 3, (void*) &castMagickMessage, true);
        }
      }
      if (!iOwner.mController.HasFinished)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastBlizzard || SpellManager.Instance.IsEffectActive(typeof (Blizzard)) ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class CastMeteor : Assatur.AssaturState, IBossState<Assatur>
  {
    private VisualEffectReference mMagickEffect;

    public unsafe void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[5], 0.4f, false);
      EffectManager.Instance.StartEffect(Assatur.GENERIC_MAGICK, ref iOwner.mMagickEffect, out this.mMagickEffect);
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Assatur.CastMagickMessage castMagickMessage;
        castMagickMessage.Magick = MagickType.MeteorS;
        BossFight.Instance.SendMessage<Assatur.CastMagickMessage>((IBoss) iOwner, (ushort) 3, (void*) &castMagickMessage, true);
      }
      MeteorShower.Instance.Execute((ISpellCaster) iOwner.mBodyZone, iOwner.mPlayState);
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      EffectManager.Instance.UpdateOrientation(ref this.mMagickEffect, ref iOwner.mMagickEffect);
      if (!iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastMeteor || SpellManager.Instance.IsEffectActive(typeof (MeteorShower)) ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class CastVortex : Assatur.AssaturState, IBossState<Assatur>
  {
    private bool mCastedMagick;
    private VisualEffectReference mMagickEffect;

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[4], 0.4f, false);
      EffectManager.Instance.StartEffect(Assatur.GENERIC_MAGICK, ref iOwner.mMagickEffect, out this.mMagickEffect);
      this.mCastedMagick = false;
    }

    public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      EffectManager.Instance.UpdateOrientation(ref this.mMagickEffect, ref iOwner.mMagickEffect);
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mController.CrossFadeEnabled)
        return;
      if ((double) iOwner.mController.Time > 1.0 & !this.mCastedMagick)
      {
        new Magick() { MagickType = MagickType.Vortex }.Effect.Execute((ISpellCaster) iOwner.mBodyZone, iOwner.mPlayState);
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Assatur.CastMagickMessage castMagickMessage;
          castMagickMessage.Magick = MagickType.Vortex;
          BossFight.Instance.SendMessage<Assatur.CastMagickMessage>((IBoss) iOwner, (ushort) 3, (void*) &castMagickMessage, true);
        }
        this.mCastedMagick = true;
      }
      if (!iOwner.mController.HasFinished)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastVortex || SpellManager.Instance.IsEffectActive(typeof (Vortex)) ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class CastTimeWarp : Assatur.AssaturState, IBossState<Assatur>
  {
    public unsafe void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[10], 0.4f, false);
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Assatur.CastMagickMessage castMagickMessage;
        castMagickMessage.Magick = MagickType.TimeWarp;
        BossFight.Instance.SendMessage<Assatur.CastMagickMessage>((IBoss) iOwner, (ushort) 3, (void*) &castMagickMessage, true);
      }
      TimeWarp.Instance.Execute((ISpellCaster) iOwner.mBodyZone, iOwner.mPlayState);
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (!iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastTimeWarp || SpellManager.Instance.IsEffectActive(typeof (TimeWarp)) ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class CastBlade : Assatur.AssaturState, IBossState<Assatur>
  {
    private ArcaneBlade mBlade;
    private DamageCollection5 mDamage;
    private HitList mHitlist = new HitList(16 /*0x10*/);
    private Cue mCue;

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[11], 0.4f, false);
      new Spell()
      {
        Element = Elements.Arcane,
        ArcaneMagnitude = 5f
      }.CalculateDamage(SpellType.Beam, Magicka.GameLogic.Spells.CastType.Weapon, out this.mDamage);
      this.mHitlist.Clear();
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (iOwner.mController.CrossFadeEnabled)
        return;
      float num1 = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
      if ((double) num1 >= (double) Assatur.ANIMATION_TIMERS[11][0] && (double) num1 <= (double) Assatur.ANIMATION_TIMERS[11][1])
      {
        if (this.mBlade == null || this.mBlade.IsDead)
        {
          this.mBlade = ArcaneBlade.GetInstance();
          this.mBlade.Initialize(iOwner.mPlayState, (Item) null, Elements.Arcane, 18f);
          this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, "spell_arcane_ray_stage2".GetHashCodeCustom(), iOwner.mBodyZone.AudioEmitter);
        }
        iOwner.mCastTimeCooldown -= iDeltaTime;
        this.mHitlist.Update(iDeltaTime);
        Matrix result1 = iOwner.mBodyZone.CastSource;
        Matrix result2;
        Matrix.CreateRotationX(-1.57079637f, out result2);
        Matrix.Multiply(ref result2, ref result1, out result1);
        this.mBlade.Orientation = result1;
        if ((double) num1 >= (double) Assatur.ANIMATION_TIMERS[11][2] && (double) num1 <= (double) Assatur.ANIMATION_TIMERS[11][3])
        {
          Vector3 oPos = this.mBlade.Orientation.Translation;
          Vector3 up = this.mBlade.Orientation.Up;
          if (NetworkManager.Instance.State != NetworkState.Client)
          {
            EntityManager entityManager = iOwner.mPlayState.EntityManager;
            List<Magicka.GameLogic.Entities.Entity> entities = entityManager.GetEntities(oPos, this.mBlade.Range, true);
            entities.Remove((Magicka.GameLogic.Entities.Entity) iOwner.mBodyZone);
            for (int index = 0; index < entities.Count; ++index)
            {
              Vector3 oPosition;
              if (entities[index] is IDamageable t && !this.mHitlist.ContainsKey(t.Handle) && t.ArcIntersect(out oPosition, oPos, up, this.mBlade.Range, 1.41371667f, 2f))
              {
                int num2 = (int) t.Damage(this.mDamage, (Magicka.GameLogic.Entities.Entity) iOwner.mBodyZone, 0.0, oPosition);
                this.mHitlist.Add(t.Handle, 0.333f);
              }
            }
            entityManager.ReturnEntityList(entities);
          }
          if ((double) iOwner.mCastTimeCooldown <= 0.0)
          {
            Segment iSeg = new Segment(this.mBlade.Orientation.Translation, this.mBlade.Orientation.Up * this.mBlade.Range);
            if (iOwner.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
              EffectManager.Instance.StartEffect("arcane_detonation".GetHashCodeCustom(), ref oPos, ref up, out VisualEffectReference _);
            iOwner.mCastTimeCooldown += 0.005f;
          }
        }
      }
      else if ((double) num1 > (double) Assatur.ANIMATION_TIMERS[11][1] && this.mBlade != null && !this.mBlade.IsDead)
        this.mBlade.Kill();
      if (!iOwner.mController.HasFinished)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner)
    {
      if (this.mBlade != null && !this.mBlade.IsDead)
        this.mBlade.Kill();
      this.mCue.Stop(AudioStopOptions.AsAuthored);
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastBlade ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class CastLife : Assatur.AssaturState, IBossState<Assatur>
  {
    private bool mCastEffect;
    private float mHealTimer;

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[7], 0.4f, false);
      this.mCastEffect = false;
      this.mHealTimer = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (iOwner.mController.CrossFadeEnabled)
        return;
      float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
      if ((double) num >= (double) Assatur.ANIMATION_TIMERS[7][0] & (double) num <= (double) Assatur.ANIMATION_TIMERS[7][1])
      {
        if (!this.mCastEffect)
        {
          Vector3 translation = iOwner.mSpineOrientation.Translation;
          Vector3 result = Vector3.UnitZ;
          Vector3.Negate(ref result, out result);
          EffectManager.Instance.StartEffect(Assatur.LIFE_EFFECT, ref translation, ref result, out iOwner.mCurrentEffect);
          this.mCastEffect = true;
        }
        if (NetworkManager.Instance.State != NetworkState.Client && (double) this.mHealTimer <= 0.0)
        {
          iOwner.Damage(1, -1000f, Elements.Life);
          this.mHealTimer += 0.25f;
        }
      }
      this.mHealTimer -= iDeltaTime;
      if (!iOwner.mController.HasFinished)
        return;
      iOwner.mController.CrossFade(iOwner.mClips[7], 0.4f, false);
      this.mCastEffect = false;
      this.mHealTimer = 0.0f;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastLife ? 0.0f : Math.Max((float) (1.0 - (double) iOwner.mHitPoints / (double) iOwner.mMaxHitPoints) * 2f, 1f);
    }
  }

  public class CastMMState : Assatur.AssaturState, IBossState<Assatur>
  {
    private Assatur.CastMMState.Castdir mCastDir;
    private float mMissileDelay = 0.15f;
    private Cue mCue;
    private static readonly int MMSOUND = "magick_vortex".GetHashCodeCustom();

    public void OnEnter(Assatur iOwner)
    {
      this.mMissileDelay = 0.15f;
      this.mCastDir = Assatur.CastMMState.Castdir.None;
      iOwner.mController.CrossFade(iOwner.mClips[0], 0.5f, false);
    }

    public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (iOwner.mController.CrossFadeEnabled || iOwner.mTarget == null)
        return;
      if (this.mCastDir == Assatur.CastMMState.Castdir.None)
      {
        if (!iOwner.mController.HasFinished)
          return;
        Vector3 result1 = iOwner.mTarget.Position;
        Vector3 position = iOwner.mBodyZone.Position;
        Vector3.Subtract(ref result1, ref position, out result1);
        result1.Y = 0.0f;
        result1.Normalize();
        Vector3 forward = iOwner.mBodyZone.Body.Orientation.Forward with
        {
          Y = 0.0f
        };
        forward.Normalize();
        float num = MagickaMath.Angle(ref forward, ref result1);
        Vector3 result2;
        Vector3.Cross(ref forward, ref result1, out result2);
        if ((double) result2.Y <= 0.0)
          num = -num;
        if ((double) num >= 0.39269909262657166)
        {
          iOwner.mController.CrossFade(iOwner.mClips[1], 0.5f, false);
          this.mCastDir = Assatur.CastMMState.Castdir.Left;
        }
        else if ((double) num <= -0.39269909262657166)
        {
          iOwner.mController.CrossFade(iOwner.mClips[2], 0.5f, false);
          this.mCastDir = Assatur.CastMMState.Castdir.Right;
        }
        else
        {
          iOwner.mController.CrossFade(iOwner.mClips[3], 0.5f, false);
          this.mCastDir = Assatur.CastMMState.Castdir.Both;
        }
      }
      else if (iOwner.mController.HasFinished)
      {
        this.mCastDir = Assatur.CastMMState.Castdir.None;
        iOwner.ChangeState(Assatur.States.Battle);
      }
      else
      {
        if (NetworkManager.Instance.State == NetworkState.Client)
          return;
        if ((double) this.mMissileDelay <= 0.0)
        {
          float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
          if ((double) num >= (double) Assatur.ANIMATION_TIMERS[3][0] && this.mCue == null)
            this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, Assatur.CastMMState.MMSOUND, iOwner.mBodyZone.AudioEmitter);
          switch (this.mCastDir)
          {
            case Assatur.CastMMState.Castdir.Both:
              if ((double) num >= (double) Assatur.ANIMATION_TIMERS[3][0] && (double) num <= (double) Assatur.ANIMATION_TIMERS[3][1])
              {
                Matrix castSource = iOwner.mBodyZone.CastSource;
                this.SpawnMissile(ref castSource, iOwner, -0.75f);
                if (NetworkManager.Instance.State == NetworkState.Server)
                {
                  Assatur.SpawnMMMessage spawnMmMessage;
                  spawnMmMessage.CastDir = 1;
                  spawnMmMessage.Handle = iOwner.mMissiles[iOwner.mMissiles.Count - 1].Handle;
                  BossFight.Instance.SendMessage<Assatur.SpawnMMMessage>((IBoss) iOwner, (ushort) 6, (void*) &spawnMmMessage, true);
                }
                Matrix weaponSource = iOwner.mBodyZone.WeaponSource;
                this.SpawnMissile(ref weaponSource, iOwner, -0.75f);
                if (NetworkManager.Instance.State == NetworkState.Server)
                {
                  Assatur.SpawnMMMessage spawnMmMessage;
                  spawnMmMessage.CastDir = 1;
                  spawnMmMessage.Handle = iOwner.mMissiles[iOwner.mMissiles.Count - 1].Handle;
                  BossFight.Instance.SendMessage<Assatur.SpawnMMMessage>((IBoss) iOwner, (ushort) 6, (void*) &spawnMmMessage, true);
                  break;
                }
                break;
              }
              break;
            case Assatur.CastMMState.Castdir.Left:
              if ((double) num >= (double) Assatur.ANIMATION_TIMERS[1][0] && (double) num <= (double) Assatur.ANIMATION_TIMERS[1][1])
              {
                Matrix castSource = iOwner.mBodyZone.CastSource;
                this.SpawnMissile(ref castSource, iOwner, -0.25f);
                if (NetworkManager.Instance.State == NetworkState.Server)
                {
                  Assatur.SpawnMMMessage spawnMmMessage;
                  spawnMmMessage.CastDir = 1;
                  spawnMmMessage.Handle = iOwner.mMissiles[iOwner.mMissiles.Count - 1].Handle;
                  BossFight.Instance.SendMessage<Assatur.SpawnMMMessage>((IBoss) iOwner, (ushort) 6, (void*) &spawnMmMessage, true);
                  break;
                }
                break;
              }
              break;
            case Assatur.CastMMState.Castdir.Right:
              if ((double) num >= (double) Assatur.ANIMATION_TIMERS[2][0] && (double) num <= (double) Assatur.ANIMATION_TIMERS[2][1])
              {
                Matrix weaponSource = iOwner.mBodyZone.WeaponSource;
                this.SpawnMissile(ref weaponSource, iOwner, -0.25f);
                if (NetworkManager.Instance.State == NetworkState.Server)
                {
                  Assatur.SpawnMMMessage spawnMmMessage;
                  spawnMmMessage.CastDir = 1;
                  spawnMmMessage.Handle = iOwner.mMissiles[iOwner.mMissiles.Count - 1].Handle;
                  BossFight.Instance.SendMessage<Assatur.SpawnMMMessage>((IBoss) iOwner, (ushort) 6, (void*) &spawnMmMessage, true);
                  break;
                }
                break;
              }
              break;
          }
          this.mMissileDelay += 0.15f;
        }
        this.mMissileDelay -= iDeltaTime;
      }
    }

    public void OnExit(Assatur iOwner)
    {
      if (this.mCue != null && !this.mCue.IsStopping)
        this.mCue.Stop(AudioStopOptions.AsAuthored);
      this.mCue = (Cue) null;
      iOwner.mMissiles.Clear();
    }

    public void SpawnMissile(ref Matrix iOrientation, Assatur iOwner, float iTolerance)
    {
      AudioManager.Instance.PlayCue(Banks.Characters, "boss_assatur_barrage".GetHashCodeCustom(), iOwner.mBodyZone.AudioEmitter);
      Matrix matrix = iOrientation;
      Vector3 translation = matrix.Translation;
      MissileEntity instance = MissileEntity.GetInstance(iOwner.mPlayState);
      Vector3 result = matrix.Forward;
      Vector3.Multiply(ref result, 30f, out result);
      iOwner.mMissileConditions[0].Clear();
      iOwner.mMissileConditions[0].Add(new EventStorage(new PlayEffectEvent(Assatur.MISSILE_TRAIL, true)));
      instance.Initialize((Magicka.GameLogic.Entities.Entity) iOwner.mBodyZone, (Magicka.GameLogic.Entities.Entity) iOwner.mTarget, 1f, 0.25f, ref translation, ref result, (Model) null, iOwner.mMissileConditions, false);
      instance.HomingTolerance = iTolerance;
      iOwner.mMissiles.Add(instance);
      iOwner.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) instance);
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastMMState ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }

    public enum Castdir
    {
      None,
      Both,
      Left,
      Right,
    }
  }

  public class CastLightning : Assatur.AssaturState, IBossState<Assatur>
  {
    private float mLightningTimer;
    private HitList mHitlist;
    private static readonly int LIGHTNING_SOUND = "spell_lightning_spray".GetHashCodeCustom();
    private Cue mCue;

    public CastLightning() => this.mHitlist = new HitList(16 /*0x10*/);

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[6], 0.4f, false);
      this.mLightningTimer = 0.0f;
      this.mHitlist.Clear();
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (iOwner.mController.CrossFadeEnabled)
        return;
      float num1 = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
      this.mLightningTimer -= iDeltaTime;
      if (iOwner.mController.HasFinished)
      {
        iOwner.ChangeState(Assatur.States.Battle);
      }
      else
      {
        if ((double) num1 >= (double) Assatur.ANIMATION_TIMERS[6][0] && (this.mCue == null || this.mCue.IsStopped))
          this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, Assatur.CastLightning.LIGHTNING_SOUND, iOwner.mBodyZone.AudioEmitter);
        if ((double) num1 < (double) Assatur.ANIMATION_TIMERS[6][0] || (double) num1 > (double) Assatur.ANIMATION_TIMERS[6][1] || (double) this.mLightningTimer > 0.0)
          return;
        Spell spell = new Spell();
        spell.Element |= Elements.Lightning;
        spell.LightningMagnitude = 1f;
        DamageCollection5 oDamages;
        spell.CalculateDamage(SpellType.Lightning, Magicka.GameLogic.Spells.CastType.Force, out oDamages);
        float num2 = Assatur.ANIMATION_TIMERS[6][1] - Assatur.ANIMATION_TIMERS[6][0];
        oDamages.MultiplyMagnitude((float) ((double) num2 * 0.15000000596046448 * 0.5));
        this.mHitlist.Clear();
        LightningBolt lightning1 = LightningBolt.GetLightning();
        Vector3 translation1 = iOwner.mBodyZone.CastSource.Translation;
        Vector3 vector3_1 = -Vector3.UnitZ;
        lightning1.Cast((ISpellCaster) iOwner.mBodyZone, translation1, (Magicka.GameLogic.Entities.Entity) iOwner.mTarget, this.mHitlist, Spell.LIGHTNINGCOLOR, 1f, 30f, ref oDamages, iOwner.mPlayState);
        LightningBolt lightning2 = LightningBolt.GetLightning();
        Vector3 translation2 = iOwner.mBodyZone.WeaponSource.Translation;
        Vector3 vector3_2 = -Vector3.UnitZ;
        lightning2.Cast((ISpellCaster) iOwner.mBodyZone, translation2, (Magicka.GameLogic.Entities.Entity) iOwner.mTarget, this.mHitlist, Spell.LIGHTNINGCOLOR, 1f, 30f, ref oDamages, iOwner.mPlayState);
        this.mLightningTimer += 0.15f;
      }
    }

    public void OnExit(Assatur iOwner)
    {
      if (this.mCue != null && !this.mCue.IsStopping)
        this.mCue.Stop(AudioStopOptions.AsAuthored);
      this.mCue = (Cue) null;
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastLightning ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class CastShield : Assatur.AssaturState, IBossState<Assatur>
  {
    private bool mShielded;

    public bool Shielded
    {
      get => this.mShielded;
      set => this.mShielded = value;
    }

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[12], 0.4f, false);
      this.mShielded = false;
    }

    public unsafe void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mController.CrossFadeEnabled)
        return;
      if ((double) (iOwner.mController.Time / iOwner.mController.AnimationClip.Duration) >= (double) Assatur.ANIMATION_TIMERS[12][0] && !this.mShielded && (iOwner.mProtectiveShield == null || iOwner.mProtectiveShield.Dead))
      {
        iOwner.mProtectiveShield = Shield.GetFromCache(iOwner.mPlayState);
        iOwner.mProtectiveShield.Initialize((ISpellCaster) iOwner.mBodyZone, iOwner.mOrientation.Translation, 5f, iOwner.mOrientation.Forward, ShieldType.DISC, 5000f, Spell.SHIELDCOLOR);
        iOwner.mProtectiveShield.HitPoints = 5000f;
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          Assatur.CastShieldMessage castShieldMessage;
          castShieldMessage.Direction = iOwner.mBodyZone.Direction;
          castShieldMessage.Position = iOwner.mOrientation.Translation;
          castShieldMessage.Handle = iOwner.mProtectiveShield.Handle;
          castShieldMessage.Radius = iOwner.mProtectiveShield.Radius;
          castShieldMessage.HitPoints = 5000f;
          castShieldMessage.ShieldType = ShieldType.SPHERE;
          BossFight.Instance.SendMessage<Assatur.CastShieldMessage>((IBoss) iOwner, (ushort) 5, (void*) &castShieldMessage, true);
        }
        iOwner.mShieldTimer = 15f;
        this.mShielded = true;
        iOwner.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) iOwner.mProtectiveShield);
      }
      if (!iOwner.mController.HasFinished)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastNullifyState || iOwner.mProtectiveShield != null || !iOwner.mProtectiveShield.Dead ? 0.0f : (Math.Min(2f - iOwner.mTimeSinceLastDamage, 0.5f) + Math.Min((float) ((1.0 - (double) (iOwner.HitPoints / iOwner.MaxHitPoints)) * 2.0), 0.5f)) * (float) (0.75 + Assatur.RANDOM.NextDouble() * 0.25);
    }
  }

  public class CastStaffBeamState : Assatur.AssaturState, IBossState<Assatur>
  {
    private Railgun mRailGun;
    private float mTimer;
    private DamageCollection5 mDamage;
    private Spell mSpell;

    public CastStaffBeamState()
    {
      this.mSpell = new Spell();
      this.mSpell.Element = Elements.Arcane;
      this.mSpell.ArcaneMagnitude = 5f;
      this.mSpell.CalculateDamage(SpellType.Beam, Magicka.GameLogic.Spells.CastType.Force, out this.mDamage);
    }

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[13], 0.4f, false);
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (iOwner.mController.CrossFadeEnabled)
        return;
      this.mTimer -= iDeltaTime;
      float num1 = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
      float num2 = Assatur.ANIMATION_TIMERS[13][0];
      float num3 = Assatur.ANIMATION_TIMERS[13][1] - num2;
      float num4 = Assatur.ANIMATION_TIMERS[13][2];
      float num5 = Assatur.ANIMATION_TIMERS[13][3] - num4;
      float num6 = Math.Max(0.0f, Math.Min(1f, (num1 - num2) / num3)) * Math.Max(0.0f, Math.Min(1f, (num4 - num1) / num5));
      iOwner.mTargettingPower = 2f;
      Matrix result1 = iOwner.mStaffJoint.mBindPose;
      Matrix.Multiply(ref result1, ref iOwner.mController.SkinnedBoneTransforms[iOwner.mStaffJoint.mIndex], out result1);
      Quaternion result2;
      Quaternion.CreateFromRotationMatrix(ref result1, out result2);
      Vector3 result3 = result1.Translation;
      Vector3 mTargetPosition = iOwner.mTargetPosition;
      Vector3 result4;
      Vector3.Subtract(ref mTargetPosition, ref result3, out result4);
      result4.Normalize();
      Vector3 right = result1.Right;
      Vector3 result5;
      Vector3.Cross(ref result4, ref right, out result5);
      Matrix result6;
      Matrix.CreateWorld(ref result3, ref result5, ref result4, out result6);
      Quaternion result7;
      Quaternion.CreateFromRotationMatrix(ref result6, out result7);
      Quaternion.Lerp(ref result2, ref result7, num6, out result7);
      Matrix.CreateFromQuaternion(ref result7, out iOwner.mRailStaffOrientation);
      iOwner.mRailStaffOrientation.Translation = result3;
      iOwner.mUseRailStaffOrientation = true;
      if (this.mRailGun == null || this.mRailGun.IsDead)
      {
        this.mRailGun = Railgun.GetFromCache();
        this.mRailGun.Initialize((ISpellCaster) iOwner.mBodyZone, result3, result4, Spell.ARCANECOLOR, ref this.mDamage, ref this.mSpell);
      }
      Vector3.Multiply(ref result4, num6, out result4);
      Vector3.Add(ref result4, ref result3, out result3);
      Vector3.Add(ref result4, ref result3, out result3);
      this.mRailGun.Position = result3;
      this.mRailGun.Direction = result4;
      if (!iOwner.mController.HasFinished)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
      if (this.mRailGun == null || this.mRailGun.IsDead)
        return;
      this.mRailGun.Kill();
    }

    public void OnExit(Assatur iOwner)
    {
      if (this.mRailGun != null && !this.mRailGun.IsDead)
        this.mRailGun.Kill();
      if (iOwner.mSpellEffect != null)
        iOwner.mSpellEffect.DeInitialize((ISpellCaster) iOwner.mBodyZone);
      iOwner.mUseRailStaffOrientation = false;
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastStaffBeamState ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class CastWater : Assatur.AssaturState, IBossState<Assatur>
  {
    private bool mSpellCasted;

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[9], 0.4f, false);
      this.mSpellCasted = false;
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (iOwner.mController.CrossFadeEnabled)
        return;
      float num = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
      if ((double) num >= (double) Assatur.ANIMATION_TIMERS[9][0] && !this.mSpellCasted)
      {
        this.mSpellCasted = true;
        Spell spell = new Spell();
        spell.Element |= Elements.Water;
        spell.WaterMagnitude = 5f;
        spell.Cast(true, (ISpellCaster) iOwner.mBodyZone, Magicka.GameLogic.Spells.CastType.Force);
      }
      else if ((double) num >= (double) Assatur.ANIMATION_TIMERS[9][1] && iOwner.mSpellEffect != null)
      {
        iOwner.mSpellEffect.DeInitialize((ISpellCaster) iOwner.mBodyZone);
        this.mSpellCasted = false;
      }
      else if (iOwner.mSpellEffect == null)
        this.mSpellCasted = false;
      if (!iOwner.mController.HasFinished)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner)
    {
      if (iOwner.mSpellEffect == null)
        return;
      iOwner.mSpellEffect.Stop((ISpellCaster) iOwner.mBodyZone);
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastWater ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class CastBarrier : Assatur.AssaturState, IBossState<Assatur>
  {
    private bool mSpellCasted;
    public Spell mSpell;
    public DamageCollection5 mDamage;

    public CastBarrier()
    {
      this.mSpell.Element = Elements.Earth | Elements.Fire | Elements.Shield;
      this.mSpell.EarthMagnitude = 1f;
      this.mSpell.ShieldMagnitude = 1f;
      this.mSpell.FireMagnitude = 3f;
      this.mSpell.CalculateDamage(SpellType.Shield, Magicka.GameLogic.Spells.CastType.Weapon, out this.mDamage);
    }

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[9], 0.4f, false);
      this.mSpellCasted = false;
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mController.CrossFadeEnabled)
        return;
      if ((double) (iOwner.mController.Time / iOwner.mController.AnimationClip.Duration) >= (double) Assatur.ANIMATION_TIMERS[9][0] && !this.mSpellCasted)
      {
        this.mSpellCasted = true;
        Segment iSeg = new Segment(iOwner.mChasmOrientation.Translation, Vector3.Down * 3f);
        iOwner.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, iSeg);
      }
      if (!iOwner.mController.HasFinished)
        return;
      iOwner.ChangeState(Assatur.States.Battle);
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner)
    {
      return iOwner.mLastState is Assatur.CastBarrier ? 0.0f : (float) Assatur.RANDOM.NextDouble();
    }
  }

  public class DeathState : Assatur.AssaturState, IBossState<Assatur>
  {
    private bool mTriggerExecuted;
    private float mPreTimer;
    private float mStartPosition;
    private VisualEffectReference mEffect;

    public void OnEnter(Assatur iOwner)
    {
      iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Assatur.ASSATUR_DEFEATED_TRIGGER, (Magicka.GameLogic.Entities.Character) null, false);
      if (iOwner.mSpellEffect != null)
        iOwner.mSpellEffect.DeInitialize((ISpellCaster) iOwner.mBodyZone);
      iOwner.mSpellEffect = (SpellEffect) null;
      this.mStartPosition = iOwner.mOrientation.Translation.Y;
      this.mTriggerExecuted = false;
      iOwner.mController.CrossFade(iOwner.mClips[17], 0.2f, false);
      this.mPreTimer = iOwner.mClips[17].Duration * 0.2f;
      Matrix orientation = iOwner.mBodyZone.Body.Orientation with
      {
        Translation = iOwner.mBodyZone.Position
      };
      EffectManager.Instance.StartEffect("assatur_death".GetHashCodeCustom(), ref orientation, out this.mEffect);
    }

    public void OnUpdate(float iDeltaTime, Assatur iOwner)
    {
      float amount = iOwner.mController.Time / iOwner.mController.AnimationClip.Duration;
      Vector3 translation = iOwner.mOrientation.Translation with
      {
        Y = MathHelper.Lerp(this.mStartPosition, this.mStartPosition + 4f, amount)
      };
      iOwner.mTargetOrientation.Translation = translation;
      iOwner.mOrientation.Translation = translation;
      Matrix mOrientation = iOwner.mOrientation;
      mOrientation.Translation = new Vector3(mOrientation.Translation.X, mOrientation.Translation.Y - amount * 4f, mOrientation.Translation.Z);
      EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref mOrientation);
      if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled && !this.mTriggerExecuted)
      {
        this.mTriggerExecuted = true;
        iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger(Assatur.ASSATUR_KILLED_TRIGGER, (Magicka.GameLogic.Entities.Character) null, false);
      }
      iOwner.mDeathScale = (float) (1.0010000467300415 - (double) iOwner.mController.Time / (double) iOwner.mController.AnimationClip.Duration);
      this.mPreTimer -= iDeltaTime;
    }

    public void OnExit(Assatur iOwner)
    {
    }

    public float GetWeight(Assatur iOwner) => throw new NotImplementedException();
  }

  public enum MessageType : ushort
  {
    Update,
    ChangeState,
    ChangeTarget,
    CastMagick,
    CastBarrier,
    CastShield,
    SpawnMM,
  }

  internal struct UpdateMessage
  {
    public const ushort TYPE = 0;
    public byte Animation;
    public float AnimationTime;
    public float Hitpoints;
  }

  internal struct SpawnMMMessage
  {
    public const ushort TYPE = 6;
    public ushort Handle;
    public int CastDir;
  }

  internal struct CastMagickMessage
  {
    public const ushort TYPE = 3;
    public MagickType Magick;
    public Vector3 Position;
    public Vector3 Direction;
    public int Effect;
  }

  internal struct CastBarrierMessage
  {
    public const ushort TYPE = 4;
    public Vector3 Position;
    public Vector3 Direction;
    public ushort Handle;
  }

  internal struct CastShieldMessage
  {
    public const ushort TYPE = 5;
    public Vector3 Position;
    public Vector3 Direction;
    public ShieldType ShieldType;
    public float Radius;
    public float HitPoints;
    public ushort Handle;
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 1;
    public Assatur.States NewState;
  }

  internal struct ChangeTargetMessage
  {
    public const ushort TYPE = 2;
    public ushort Target;
  }

  public enum Animations
  {
    MMPrepare,
    MMLeft,
    MMRight,
    MMBoth,
    Vortex,
    MeteorShower,
    Lightning,
    Life,
    Blizzard,
    Water,
    Eruption,
    ArcaneBlade,
    Shield,
    RailBeam,
    Nullify,
    Idle,
    Tentacles,
    Defeat,
    NrOfAnimations,
  }

  public enum States
  {
    Battle,
    MagickMissiles,
    Vortex,
    MeteorShower,
    Lightning,
    Life,
    Blizzard,
    Water,
    TimeWarp,
    ArcaneBlade,
    Shield,
    StaffBeam,
    Nullify,
    Barrier,
    Intro,
    Dead,
    None,
    NrOfStates,
  }
}
