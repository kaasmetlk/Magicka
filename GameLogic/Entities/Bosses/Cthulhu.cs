// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Cthulhu
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using XNAnimation;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Cthulhu : BossStatusEffected, IBossSpellCaster, IBoss
{
  private const float NETWORK_UPDATE_PERIOD = 0.0333333351f;
  private const int NR_OF_ARM_JOINTS = 3;
  private const int NR_OF_FINGERS = 6;
  private const int NR_OF_SPINE_PARTS = 5;
  private const int DEFLECTION_TIME = 8;
  private const int MAX_LIGHTING_RANGE_REL_BODY_CHECK = 19;
  private const int MIN_LIGHTING_RANGE_REL_BODY_CHECK = 5;
  private const int MAX_VORTEX_RANGE_REL_BODY_CHECK = 23;
  private const int MIN_VORTEX_RANGE_REL_BODY_CHECK = 2;
  private const int SUBMERGE_PUSH_RANGE = 6;
  private const int SUBMERGE_SOAK_RANGE = 6;
  private const int EMERGE_KNOCKBACK_RANGE = 6;
  private const int EMERGE_SOAK_RANGE = 10;
  private const int MAX_DIST_TO_AVATARS = 21;
  private const int MAX_RANGE_FOR_GOOD_SPOT = 18;
  private const int DEVOUR_SUCK_RADIUS = 30;
  private IBossState<Cthulhu>[] mStages = new IBossState<Cthulhu>[5]
  {
    (IBossState<Cthulhu>) new Cthulhu.IntroStage(),
    (IBossState<Cthulhu>) new Cthulhu.BattleStage(),
    (IBossState<Cthulhu>) new Cthulhu.LateBattleStage(),
    (IBossState<Cthulhu>) new Cthulhu.CriticalStage(),
    (IBossState<Cthulhu>) new Cthulhu.FinalStage()
  };
  private Cthulhu.CthulhuState[] mStates = new Cthulhu.CthulhuState[14]
  {
    (Cthulhu.CthulhuState) new Cthulhu.IdleState(),
    (Cthulhu.CthulhuState) new Cthulhu.EmergeState(),
    (Cthulhu.CthulhuState) new Cthulhu.SubmergeState(),
    (Cthulhu.CthulhuState) new Cthulhu.DevourState(),
    (Cthulhu.CthulhuState) new Cthulhu.DevourHitState(),
    (Cthulhu.CthulhuState) new Cthulhu.LightningState(),
    (Cthulhu.CthulhuState) new Cthulhu.MistState(),
    (Cthulhu.CthulhuState) new Cthulhu.CallState(),
    (Cthulhu.CthulhuState) new Cthulhu.LesserCallState(),
    (Cthulhu.CthulhuState) new Cthulhu.TimewarpState(),
    (Cthulhu.CthulhuState) new Cthulhu.HypnotizeState(),
    (Cthulhu.CthulhuState) new Cthulhu.RageState(),
    (Cthulhu.CthulhuState) new Cthulhu.OtherworldlyBoltState(),
    (Cthulhu.CthulhuState) new Cthulhu.DeathState()
  };
  private static readonly int SOUND_CALL_OF_CTHULHU = "cthulhu_call_of_cthulhu".GetHashCodeCustom();
  private static readonly int SOUND_DEATH = "cthulhu_death".GetHashCodeCustom();
  private static readonly int SOUND_DEVOUR = "cthulhu_devour".GetHashCodeCustom();
  private static readonly int SOUND_DEVOUR_SUCK = "cthulhu_devour_suck".GetHashCodeCustom();
  private static readonly int SOUND_DEVOUR_HIT = "cthulhu_devour_hit".GetHashCodeCustom();
  private static readonly int SOUND_OTHERWORLDLY_BOLT = "cthulhu_howl".GetHashCodeCustom();
  private static readonly int SOUND_HOWL = "cthulhu_howl".GetHashCodeCustom();
  private static readonly int SOUND_EMERGE = "cthulhu_emerge".GetHashCodeCustom();
  private static readonly int SOUND_HYPNOTIZE = "cthulhu_hypnotize".GetHashCodeCustom();
  private static readonly int SOUND_LESSER_CALL_OF_CTHULHU = "cthulhu_lesser_call_of_cthulhu".GetHashCodeCustom();
  private static readonly int SOUND_LIGHTNING = "spell_lightning_spray".GetHashCodeCustom();
  private static readonly int SOUND_MIST = "cthulhu_mist".GetHashCodeCustom();
  private static readonly int SOUND_SUBMERGE = "cthulhu_submerge".GetHashCodeCustom();
  private static readonly int SOUND_TIMEWARP = "magick_timewarp".GetHashCodeCustom();
  private static readonly int SOUND_RAGE = "cthulhu_rage".GetHashCodeCustom();
  private float mLastNetworkUpdate;
  protected float mNetworkUpdateTimer;
  private static readonly int[] SPAWN_LOCATORS = new int[8]
  {
    "boss_spawn0".GetHashCodeCustom(),
    "boss_spawn6".GetHashCodeCustom(),
    "boss_spawn4".GetHashCodeCustom(),
    "boss_spawn5".GetHashCodeCustom(),
    "boss_spawn2".GetHashCodeCustom(),
    "boss_spawn1".GetHashCodeCustom(),
    "boss_spawn3".GetHashCodeCustom(),
    "boss_spawn7".GetHashCodeCustom()
  };
  private static readonly int[] DEEP_ONES_SPAWN_LOCATORS = new int[8]
  {
    "spawn_deepone0".GetHashCodeCustom(),
    "spawn_deepone1".GetHashCodeCustom(),
    "spawn_deepone2".GetHashCodeCustom(),
    "spawn_deepone3".GetHashCodeCustom(),
    "spawn_deepone4".GetHashCodeCustom(),
    "spawn_deepone5".GetHashCodeCustom(),
    "spawn_deepone6".GetHashCodeCustom(),
    "spawn_deepone7".GetHashCodeCustom()
  };
  private static readonly int MIST_LOCATOR = "boss_spawn_middle".GetHashCodeCustom();
  internal static readonly int BLOOD_BLACK_EFFECT = "gore_splash_black".GetHashCodeCustom();
  private static readonly int DEFLECTION_EFFECT = "cthulhu_deflect".GetHashCodeCustom();
  private static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);
  private Cthulhu.RenderData[] mRenderData;
  private Matrix mTransform;
  private PlayState mPlayState;
  private AnimationClip[] mAnimations;
  private AnimationController mAnimationController;
  internal static Random RANDOM = new Random();
  private Cthulhu.States mCurrentState;
  private Cthulhu.States mPreviousState;
  private Cthulhu.Stages mCurrentStage;
  private float mStageSpeedModifier;
  private float mStateSpeedModifier;
  private float mTimeBetweenActions;
  private int mNumberOfTentacles;
  private float mRadius;
  private bool mDead;
  private BossSpellCasterZone mDamageZone;
  private BossCollisionZone mArmZone;
  private SpellEffect mSpellEffect;
  private int[] mRightArmIndex = new int[3];
  private Matrix[] mRightArmBindPose = new Matrix[3];
  private int[] mLeftArmIndex = new int[3];
  private Matrix[] mLeftArmBindPose = new Matrix[3];
  private int mRightHandIndex;
  private Matrix mRightHandBindPose;
  private int mLeftHandIndex;
  private Matrix mLeftHandBindPose;
  private int[] mRightFingerIndex = new int[6];
  private Matrix[] mRightFingerBindPose = new Matrix[6];
  private int[] mLeftFingerIndex = new int[6];
  private Matrix[] mLeftFingerBindPose = new Matrix[6];
  private int mHeadIndex;
  private Matrix mHeadBindPose;
  private int mMouthAttachIndex;
  private Matrix mMouthAttachBindPose;
  private int[] mSpineIndex = new int[5];
  private Matrix[] mSpineBindPose = new Matrix[5];
  private Matrix mMouthAttachOrientation;
  private Matrix mHeadOrientation;
  private Matrix mLeftHandOrientation;
  private Matrix mRightHandOrientation;
  private Matrix[] mRightFingerOrientation = new Matrix[6];
  private Matrix[] mLeftFingerOrientation = new Matrix[6];
  private CthulhuMist mMistCloud;
  private Tentacle[] mTentacles;
  private List<Tentacle> mActiveTentacles;
  private List<Tentacle> mInactiveTentacles;
  private Matrix mMistSpawnTransform;
  private Matrix[] mDeepOnesSpawnTransforms = new Matrix[8];
  private Matrix[] mSpawnTransforms = new Matrix[8];
  private int[] mOccupiedSpawnPoints;
  private float mDeflectionTimer;
  private AuraDeflect mDeflectionAura = new AuraDeflect(5f);
  private VisualEffectReference mDeflectionEffect;
  private VisualEffectReference[] mCustomColdEffectRef;
  private Player[] mPlayers = Magicka.Game.Instance.Players;
  private Magicka.GameLogic.Entities.Character mCharacterToEat;
  private float mHPOnLastEmerge;
  private float mTimeUntilSubmerge;
  private float mTimeSinceLastEmerge;
  private float mTimeSinceLastDamageTimewarp;
  private float mHitFlashTimer;
  private int mSecondsWhileOutOfRange;
  private float mCheckEnemiesInRangeTimer;
  private float mWetnessCounter;
  private int mDesiredSpawnPoint;
  private bool mActivateMist;
  private float mMistDesirability;
  private float mCheckMistDesirabilityTimer;
  private float mWaterYpos;
  private VisualEffectReference mIdleEffectRef;
  private int mIdleEffect = "cthulhu_water_surface".GetHashCodeCustom();
  private bool mInitialEmerge;
  private bool mOkToFight;
  private bool mTheKingHasFallen;
  private List<OtherworldlyBolt> mCultistMissileCache;
  private bool mStarSpawnSpawned;
  private int mPlayerHypnotizeEffect = "cthulhu_player_hypnotize".GetHashCodeCustom();

  private Cthulhu.IdleState GetIdleState => this.mStates[0] as Cthulhu.IdleState;

  private Cthulhu.EmergeState GetEmergeState => this.mStates[1] as Cthulhu.EmergeState;

  private Cthulhu.SubmergeState GetSubmergeState => this.mStates[2] as Cthulhu.SubmergeState;

  private Cthulhu.DevourState GetDevourState => this.mStates[3] as Cthulhu.DevourState;

  private Cthulhu.DevourHitState GetDevourHitState => this.mStates[4] as Cthulhu.DevourHitState;

  private Cthulhu.LightningState GetLightningState => this.mStates[5] as Cthulhu.LightningState;

  private Cthulhu.MistState GetMistState => this.mStates[6] as Cthulhu.MistState;

  private Cthulhu.CallState GetCallofCthulhuState => this.mStates[7] as Cthulhu.CallState;

  private Cthulhu.LesserCallState GetLesserCallofCthulhuState
  {
    get => this.mStates[8] as Cthulhu.LesserCallState;
  }

  private Cthulhu.TimewarpState GetTimewarpState => this.mStates[9] as Cthulhu.TimewarpState;

  private Cthulhu.HypnotizeState GetHypnotizeState => this.mStates[10] as Cthulhu.HypnotizeState;

  private Cthulhu.RageState GetRageState => this.mStates[11] as Cthulhu.RageState;

  private Cthulhu.OtherworldlyBoltState GetOtherworldlyBoltState
  {
    get => this.mStates[12] as Cthulhu.OtherworldlyBoltState;
  }

  public Cthulhu(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    if (!(RenderManager.Instance.GetEffect(SkinnedModelDeferredNormalMappedEffect.TYPEHASH) is SkinnedModelDeferredNormalMappedEffect))
    {
      SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool = RenderManager.Instance.GlobalDummyEffect.EffectPool;
      RenderManager.Instance.RegisterEffect((Effect) new SkinnedModelDeferredNormalMappedEffect(Magicka.Game.Instance.GraphicsDevice, SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool));
    }
    SkinnedModel skinnedModel1 = (SkinnedModel) null;
    SkinnedModel skinnedModel2 = (SkinnedModel) null;
    this.mOccupiedSpawnPoints = new int[Cthulhu.SPAWN_LOCATORS.Length];
    for (int index = 0; index < this.mOccupiedSpawnPoints.Length; ++index)
      this.mOccupiedSpawnPoints[index] = -1;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      skinnedModel1 = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Cthulhu/Cthulhu_mesh");
      skinnedModel2 = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Cthulhu/Cthulhu_animations");
    }
    this.mRadius = skinnedModel1.Model.Meshes[0].BoundingSphere.Radius;
    this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/deep_one");
    this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/starspawn");
    this.mAnimationController = new AnimationController();
    this.mAnimationController.Skeleton = skinnedModel2.SkeletonBones;
    this.mAnimations = new AnimationClip[16 /*0x10*/];
    this.mAnimations[0] = skinnedModel2.AnimationClips["cast_bolt_begin"];
    this.mAnimations[1] = skinnedModel2.AnimationClips["cast_bolt_mid"];
    this.mAnimations[2] = skinnedModel2.AnimationClips["cast_bolt_end"];
    this.mAnimations[3] = skinnedModel2.AnimationClips["submerge"];
    this.mAnimations[4] = skinnedModel2.AnimationClips["mists"];
    this.mAnimations[5] = skinnedModel2.AnimationClips["timewarp"];
    this.mAnimations[6] = skinnedModel2.AnimationClips["mesmerize"];
    this.mAnimations[7] = skinnedModel2.AnimationClips["madness"];
    this.mAnimations[8] = skinnedModel2.AnimationClips["emerge"];
    this.mAnimations[9] = skinnedModel2.AnimationClips["idle"];
    this.mAnimations[10] = skinnedModel2.AnimationClips["devour"];
    this.mAnimations[11] = skinnedModel2.AnimationClips["devour_hit"];
    this.mAnimations[15] = skinnedModel2.AnimationClips["rage"];
    this.mAnimations[12] = skinnedModel2.AnimationClips["cast_lightning"];
    this.mAnimations[13] = skinnedModel2.AnimationClips["call_of_cthulhu"];
    this.mAnimations[14] = skinnedModel2.AnimationClips["die"];
    this.mCustomColdEffectRef = new VisualEffectReference[5];
    this.mRenderData = new Cthulhu.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new Cthulhu.RenderData(skinnedModel2.SkeletonBones.Count, skinnedModel1.Model.Meshes[0], skinnedModel1.Model.Meshes[0].MeshParts[0]);
    int num = 0;
    Matrix result1;
    Matrix.CreateRotationY(3.14159274f, out result1);
    for (int index1 = 0; index1 < skinnedModel2.SkeletonBones.Count; ++index1)
    {
      SkinnedModelBone skeletonBone = skinnedModel2.SkeletonBones[index1];
      if (skeletonBone.Name.Equals("head", StringComparison.OrdinalIgnoreCase))
      {
        this.mHeadIndex = (int) skeletonBone.Index;
        this.mHeadBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mHeadBindPose, ref result1, out this.mHeadBindPose);
        Matrix.Invert(ref this.mHeadBindPose, out this.mHeadBindPose);
      }
      else if (skeletonBone.Name.Equals("rightthumb2", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightFingerIndex[0] = (int) skeletonBone.Index;
        Matrix result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out result2);
        this.mRightFingerBindPose[0] = result2;
      }
      else if (skeletonBone.Name.Equals("rightfinger1_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightFingerIndex[1] = (int) skeletonBone.Index;
        Matrix result3 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result3, ref result1, out result3);
        Matrix.Invert(ref result3, out result3);
        this.mRightFingerBindPose[1] = result3;
      }
      else if (skeletonBone.Name.Equals("rightfinger2_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightFingerIndex[2] = (int) skeletonBone.Index;
        Matrix result4 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result4, ref result1, out result4);
        Matrix.Invert(ref result4, out result4);
        this.mRightFingerBindPose[2] = result4;
      }
      else if (skeletonBone.Name.Equals("rightfinger3_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightFingerIndex[3] = (int) skeletonBone.Index;
        Matrix result5 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result5, ref result1, out result5);
        Matrix.Invert(ref result5, out result5);
        this.mRightFingerBindPose[3] = result5;
        this.mRightHandIndex = (int) skeletonBone.Index;
        this.mRightHandBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightHandBindPose, ref result1, out this.mRightHandBindPose);
        Matrix.Invert(ref this.mRightHandBindPose, out this.mRightHandBindPose);
      }
      else if (skeletonBone.Name.Equals("rightfinger4_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightFingerIndex[4] = (int) skeletonBone.Index;
        Matrix result6 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result6, ref result1, out result6);
        Matrix.Invert(ref result6, out result6);
        this.mRightFingerBindPose[4] = result6;
      }
      else if (skeletonBone.Name.Equals("rightfinger5_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightFingerIndex[5] = (int) skeletonBone.Index;
        Matrix result7 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result7, ref result1, out result7);
        Matrix.Invert(ref result7, out result7);
        this.mRightFingerBindPose[5] = result7;
      }
      else if (skeletonBone.Name.Equals("leftthumb2", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftFingerIndex[0] = (int) skeletonBone.Index;
        Matrix result8 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result8, ref result1, out result8);
        Matrix.Invert(ref result8, out result8);
        this.mLeftFingerBindPose[0] = result8;
      }
      else if (skeletonBone.Name.Equals("leftfinger1_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftFingerIndex[1] = (int) skeletonBone.Index;
        Matrix result9 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result9, ref result1, out result9);
        Matrix.Invert(ref result9, out result9);
        this.mLeftFingerBindPose[1] = result9;
      }
      else if (skeletonBone.Name.Equals("leftfinger2_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftFingerIndex[2] = (int) skeletonBone.Index;
        Matrix result10 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result10, ref result1, out result10);
        Matrix.Invert(ref result10, out result10);
        this.mLeftFingerBindPose[2] = result10;
      }
      else if (skeletonBone.Name.Equals("leftfinger3_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftFingerIndex[3] = (int) skeletonBone.Index;
        Matrix result11 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result11, ref result1, out result11);
        Matrix.Invert(ref result11, out result11);
        this.mLeftFingerBindPose[3] = result11;
        this.mLeftHandIndex = (int) skeletonBone.Index;
        this.mLeftHandBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mLeftHandBindPose, ref result1, out this.mLeftHandBindPose);
        Matrix.Invert(ref this.mLeftHandBindPose, out this.mLeftHandBindPose);
      }
      else if (skeletonBone.Name.Equals("leftfinger4_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftFingerIndex[4] = (int) skeletonBone.Index;
        Matrix result12 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result12, ref result1, out result12);
        Matrix.Invert(ref result12, out result12);
        this.mLeftFingerBindPose[4] = result12;
      }
      else if (skeletonBone.Name.Equals("leftfinger5_3", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftFingerIndex[5] = (int) skeletonBone.Index;
        Matrix result13 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result13, ref result1, out result13);
        Matrix.Invert(ref result13, out result13);
        this.mLeftFingerBindPose[5] = result13;
      }
      else if (skeletonBone.Name.Equals("attach", StringComparison.OrdinalIgnoreCase))
      {
        this.mMouthAttachIndex = (int) skeletonBone.Index;
        this.mMouthAttachBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mMouthAttachBindPose, ref result1, out this.mMouthAttachBindPose);
        Matrix.Invert(ref this.mMouthAttachBindPose, out this.mMouthAttachBindPose);
      }
      else if (skeletonBone.Name.Equals("leftshoulder", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftArmIndex[0] = (int) skeletonBone.Index;
        this.mLeftArmBindPose[0] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mLeftArmBindPose[0], ref result1, out this.mLeftArmBindPose[0]);
        Matrix.Invert(ref this.mLeftArmBindPose[0], out this.mLeftArmBindPose[0]);
      }
      else if (skeletonBone.Name.Equals("leftelbow", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftArmIndex[1] = (int) skeletonBone.Index;
        this.mLeftArmBindPose[1] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mLeftArmBindPose[1], ref result1, out this.mLeftArmBindPose[1]);
        Matrix.Invert(ref this.mLeftArmBindPose[1], out this.mLeftArmBindPose[1]);
      }
      else if (skeletonBone.Name.Equals("lefthand", StringComparison.OrdinalIgnoreCase))
      {
        this.mLeftArmIndex[2] = (int) skeletonBone.Index;
        this.mLeftArmBindPose[2] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mLeftArmBindPose[2], ref result1, out this.mLeftArmBindPose[2]);
        Matrix.Invert(ref this.mLeftArmBindPose[2], out this.mLeftArmBindPose[2]);
      }
      else if (skeletonBone.Name.Equals("rightshoulder", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightArmIndex[0] = (int) skeletonBone.Index;
        this.mRightArmBindPose[0] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightArmBindPose[0], ref result1, out this.mRightArmBindPose[0]);
        Matrix.Invert(ref this.mRightArmBindPose[0], out this.mRightArmBindPose[0]);
      }
      else if (skeletonBone.Name.Equals("rightelbow", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightArmIndex[1] = (int) skeletonBone.Index;
        this.mRightArmBindPose[1] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightArmBindPose[1], ref result1, out this.mRightArmBindPose[1]);
        Matrix.Invert(ref this.mRightArmBindPose[1], out this.mRightArmBindPose[1]);
      }
      else if (skeletonBone.Name.Equals("righthand", StringComparison.OrdinalIgnoreCase))
      {
        this.mRightArmIndex[2] = (int) skeletonBone.Index;
        this.mRightArmBindPose[2] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mRightArmBindPose[2], ref result1, out this.mRightArmBindPose[2]);
        Matrix.Invert(ref this.mRightArmBindPose[2], out this.mRightArmBindPose[2]);
      }
      else
      {
        for (int index2 = num; index2 < 5; ++index2)
        {
          string str = $"spine{index2 + 1}";
          if (skeletonBone.Name.Equals(str, StringComparison.OrdinalIgnoreCase))
          {
            this.mSpineIndex[index2] = (int) skeletonBone.Index;
            this.mSpineBindPose[index2] = skeletonBone.InverseBindPoseTransform;
            Matrix.Multiply(ref this.mSpineBindPose[index2], ref result1, out this.mSpineBindPose[index2]);
            Matrix.Invert(ref this.mSpineBindPose[index2], out this.mSpineBindPose[index2]);
            ++num;
            break;
          }
        }
      }
    }
    this.mDamageZone = new BossSpellCasterZone(this.mPlayState, (IBossSpellCaster) this, this.mAnimationController, this.mRightHandIndex, this.mLeftHandIndex, 0, 1.75f, new Primitive[1]
    {
      (Primitive) new Capsule(Vector3.Zero, Matrix.Identity, 1.2f, 4f)
    });
    this.mArmZone = new BossCollisionZone(this.mPlayState, (IBoss) this, new Primitive[4]
    {
      (Primitive) new Capsule(Vector3.Zero, Matrix.Identity, 0.5f, 1.6f),
      (Primitive) new Capsule(Vector3.Zero, Matrix.Identity, 0.4f, 4.7f),
      (Primitive) new Capsule(Vector3.Zero, Matrix.Identity, 0.5f, 1.6f),
      (Primitive) new Capsule(Vector3.Zero, Matrix.Identity, 0.4f, 4.7f)
    });
    this.mDamageZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mArmZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mActiveTentacles = new List<Tentacle>(4);
    this.mInactiveTentacles = new List<Tentacle>(4);
    this.mTentacles = new Tentacle[4];
    for (int iID = 0; iID < 4; ++iID)
      this.mTentacles[iID] = new Tentacle(this, (byte) iID, iID + 1, iPlayState);
    this.mMistCloud = new CthulhuMist(this, (byte) this.mTentacles.Length, this.mTentacles.Length + 1, iPlayState);
    this.mResistances = new Resistance[11];
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      this.mResistances[iIndex].Multiplier = 1f;
      this.mResistances[iIndex].Modifier = 0.0f;
      Elements elements = Spell.ElementFromIndex(iIndex);
      this.mResistances[iIndex].ResistanceAgainst = elements;
      switch (elements)
      {
        case Elements.Earth:
          this.mResistances[iIndex].Multiplier = 0.25f;
          this.mResistances[iIndex].Modifier = -500f;
          break;
        case Elements.Water:
          this.mResistances[iIndex].Multiplier = 0.0f;
          break;
        case Elements.Cold:
          this.mResistances[iIndex].Multiplier = 0.25f;
          break;
        case Elements.Poison:
          this.mResistances[iIndex].Multiplier = 0.0f;
          break;
      }
    }
    this.mMaxHitPoints = 100000f;
    this.mStateSpeedModifier = 1f;
    this.mCultistMissileCache = new List<OtherworldlyBolt>();
    for (int index = 0; index < 8; ++index)
      this.mCultistMissileCache.Add(new OtherworldlyBolt(this.mPlayState));
  }

  public float WaterYpos => this.mWaterYpos;

  public bool InitialEmerge => this.mInitialEmerge;

  internal bool OkToFight => this.mOkToFight;

  private unsafe bool OnCollision(
    CollisionSkin iSkin0,
    int iPrim0,
    CollisionSkin iSkin1,
    int iPrim1)
  {
    if (iSkin1.Owner == null)
      return false;
    if (iSkin1.Owner.Tag is Shield)
      (iSkin1.Owner.Tag as Shield).Kill();
    if (iSkin1.Owner.Tag is MissileEntity)
    {
      if (NetworkManager.Instance.State == NetworkState.Server)
        BossFight.Instance.SendMessage<Cthulhu.ActivateDeflectionMessage>((IBoss) this, (ushort) 10, (void*) &new Cthulhu.ActivateDeflectionMessage(), true);
      if (NetworkManager.Instance.State != NetworkState.Client)
        this.mDeflectionTimer = 8f;
    }
    return true;
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mHitPoints = this.mMaxHitPoints;
    this.mTransform = iOrientation;
    this.mInitialEmerge = true;
    this.mTheKingHasFallen = false;
    for (int index = 0; index < this.mSpawnTransforms.Length; ++index)
      this.mPlayState.Level.CurrentScene.GetLocator(Cthulhu.SPAWN_LOCATORS[index], out this.mSpawnTransforms[index]);
    this.mPlayState.Level.CurrentScene.GetLocator(Cthulhu.MIST_LOCATOR, out this.mMistSpawnTransform);
    for (int index = 0; index < this.mDeepOnesSpawnTransforms.Length; ++index)
      this.mPlayState.Level.CurrentScene.GetLocator(Cthulhu.DEEP_ONES_SPAWN_LOCATORS[index], out this.mDeepOnesSpawnTransforms[index]);
    this.ClearAllStatusEffects();
    this.mDead = false;
    this.mStarSpawnSpawned = false;
    this.mHitFlashTimer = 0.0f;
    this.mDamageZone.Initialize();
    this.mDamageZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mDamageZone.Body.CollisionSkin.NonCollidables.Add(this.mArmZone.Body.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mDamageZone);
    this.mArmZone.Initialize();
    this.mArmZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mArmZone.Body.CollisionSkin.NonCollidables.Add(this.mDamageZone.Body.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mArmZone);
    for (int index = 0; index < this.mOccupiedSpawnPoints.Length; ++index)
      this.mOccupiedSpawnPoints[index] = -1;
    this.OccupySpawnTransform(Cthulhu.BossSpawnPoints.NORTH, (int) this.mDamageZone.Handle);
    this.mDesiredSpawnPoint = -1;
    if (NetworkManager.Instance.State == NetworkState.Client)
      this.mDesiredSpawnPoint = 0;
    this.mActiveTentacles.Clear();
    this.mInactiveTentacles.Clear();
    for (int index = 0; index < this.mTentacles.Length; ++index)
      this.mInactiveTentacles.Add(this.mTentacles[index]);
    this.mDeflectionTimer = 0.0f;
    this.mMistCloud.Initialize();
    this.mHitPoints = this.mMaxHitPoints;
    this.mCurrentStage = Cthulhu.Stages.Intro;
    this.mStages[(int) this.mCurrentStage].OnEnter(this);
    this.mCurrentState = this.mPreviousState = Cthulhu.States.Emerge;
    this.GetEmergeState.OnEnter(this);
    this.mOkToFight = false;
    Vector3 result1 = this.mSpawnTransforms[0].Translation with
    {
      Y = -10f
    };
    Segment seg = new Segment(result1, new Vector3(0.0f, 20f, 0.0f));
    this.mWaterYpos = -2f;
    float frac;
    if (this.mPlayState.Level.CurrentScene.Liquids.Length > 0 && this.mPlayState.Level.CurrentScene.Liquids[0].SegmentIntersect(out frac, out Vector3 _, out Vector3 _, ref seg, false, false, false))
    {
      Vector3 result2 = seg.Delta;
      Vector3.Multiply(ref result2, frac, out result2);
      Vector3.Add(ref result1, ref result2, out result1);
      this.mWaterYpos = result1.Y;
    }
    for (int index = 0; index < this.mCultistMissileCache.Count; ++index)
      this.mCultistMissileCache[index].Reset();
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
    if ((double) this.mHitPoints <= 0.0 && !this.mTheKingHasFallen)
    {
      this.mTheKingHasFallen = true;
      this.ChangeState(Cthulhu.States.Death);
    }
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mNetworkUpdateTimer -= iDeltaTime;
      if ((double) this.mNetworkUpdateTimer <= 0.0)
      {
        this.mNetworkUpdateTimer = 0.0333333351f;
        this.NetworkUpdate();
      }
    }
    this.UpdateDamage(iDeltaTime);
    this.UpdateStatusEffects(iDeltaTime);
    this.mTimeUntilSubmerge -= iDeltaTime;
    this.mTimeSinceLastEmerge += iDeltaTime;
    this.mTimeSinceLastDamageTimewarp += iDeltaTime;
    this.CheckIfCharactersAreClose(iDeltaTime);
    this.CheckMistDesirability(iDeltaTime);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      if (this.mCurrentState != Cthulhu.States.Death && this.mCurrentState == Cthulhu.States.Idle)
      {
        float num = this.HitPoints / this.MaxHitPoints;
        if ((double) num > 0.949999988079071)
        {
          if (this.mCurrentStage < Cthulhu.Stages.Intro)
            this.ChangeStage(Cthulhu.Stages.Intro);
        }
        else if ((double) num >= 0.64999997615814209)
        {
          if (this.mCurrentStage < Cthulhu.Stages.Battle)
            this.ChangeStage(Cthulhu.Stages.Battle);
        }
        else if ((double) num >= 0.34999999403953552)
        {
          if (this.mCurrentStage < Cthulhu.Stages.LateBattle)
            this.ChangeStage(Cthulhu.Stages.LateBattle);
        }
        else if ((double) num > 0.15000000596046448)
        {
          if (this.mCurrentStage < Cthulhu.Stages.Critical)
            this.ChangeStage(Cthulhu.Stages.Critical);
        }
        else if ((double) num > 0.0 && this.mCurrentStage < Cthulhu.Stages.Final)
          this.ChangeStage(Cthulhu.Stages.Final);
      }
      this.mStages[(int) this.mCurrentStage].OnUpdate(iDeltaTime, this);
    }
    this.mStates[(int) this.mCurrentState].OnUpdate(iDeltaTime, this);
    Matrix parent = Matrix.CreateScale(1.25f) * this.mTransform;
    this.mAnimationController.Update(this.mStageSpeedModifier * this.mStateSpeedModifier * this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown() * iDeltaTime, ref parent, true);
    if (this.mSpellEffect != null && !this.mSpellEffect.CastUpdate(iDeltaTime, (ISpellCaster) this.mDamageZone, out float _))
    {
      this.mSpellEffect.DeInitialize((ISpellCaster) this.mDamageZone);
      this.mSpellEffect = (SpellEffect) null;
    }
    Matrix.Multiply(ref this.mMouthAttachBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthAttachIndex], out this.mMouthAttachOrientation);
    Matrix.Multiply(ref this.mHeadBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mHeadIndex], out this.mHeadOrientation);
    Matrix.Multiply(ref this.mLeftHandBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftHandIndex], out this.mLeftHandOrientation);
    Matrix.Multiply(ref this.mRightHandBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightHandIndex], out this.mRightHandOrientation);
    for (int index = 0; index < this.mRightFingerBindPose.Length; ++index)
      Matrix.Multiply(ref this.mRightFingerBindPose[index], ref this.mAnimationController.SkinnedBoneTransforms[this.mRightFingerIndex[index]], out this.mRightFingerOrientation[index]);
    for (int index = 0; index < this.mLeftFingerBindPose.Length; ++index)
      Matrix.Multiply(ref this.mLeftFingerBindPose[index], ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftFingerIndex[index]], out this.mLeftFingerOrientation[index]);
    Vector3 up = Vector3.Up;
    Vector3 zero = Vector3.Zero;
    Vector3 result1 = this.mSpineBindPose[0].Translation;
    Vector3.Transform(ref result1, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[0]], out result1);
    Vector3 result2 = this.mSpineBindPose[4].Translation;
    Vector3.Transform(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[4]], out result2);
    Vector3 result3;
    Vector3.Subtract(ref result1, ref result2, out result3);
    result3.Normalize();
    Transform iTransform = new Transform();
    Matrix.CreateWorld(ref zero, ref result3, ref up, out iTransform.Orientation);
    iTransform.Position = result1;
    this.mDamageZone.SetOrientation(ref iTransform.Position, ref iTransform.Orientation);
    this.mDamageZone.Update(iDataChannel, iDeltaTime);
    for (int prim = 0; prim < 2; ++prim)
    {
      result1 = this.mLeftArmBindPose[prim].Translation;
      Vector3.Transform(ref result1, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftArmIndex[prim]], out result1);
      result2 = this.mLeftArmBindPose[prim + 1].Translation;
      Vector3.Transform(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftArmIndex[prim + 1]], out result2);
      Vector3.Subtract(ref result1, ref result2, out result3);
      result3.Normalize();
      Matrix.CreateWorld(ref zero, ref result3, ref up, out iTransform.Orientation);
      iTransform.Position = result1;
      this.mArmZone.Body.CollisionSkin.GetPrimitiveLocal(prim).SetTransform(ref iTransform);
      this.mArmZone.Body.CollisionSkin.GetPrimitiveNewWorld(prim).SetTransform(ref iTransform);
      this.mArmZone.Body.CollisionSkin.GetPrimitiveOldWorld(prim).SetTransform(ref iTransform);
      result1 = this.mRightArmBindPose[prim].Translation;
      Vector3.Transform(ref result1, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightArmIndex[prim]], out result1);
      result2 = this.mRightArmBindPose[prim + 1].Translation;
      Vector3.Transform(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightArmIndex[prim + 1]], out result2);
      Vector3.Subtract(ref result1, ref result2, out result3);
      result3.Normalize();
      Matrix.CreateWorld(ref zero, ref result3, ref up, out iTransform.Orientation);
      iTransform.Position = result1;
      this.mArmZone.Body.CollisionSkin.GetPrimitiveLocal(2 + prim).SetTransform(ref iTransform);
      this.mArmZone.Body.CollisionSkin.GetPrimitiveNewWorld(2 + prim).SetTransform(ref iTransform);
      this.mArmZone.Body.CollisionSkin.GetPrimitiveOldWorld(2 + prim).SetTransform(ref iTransform);
    }
    this.mArmZone.Body.CollisionSkin.UpdateWorldBoundingBox();
    this.mArmZone.Update(iDataChannel, iDeltaTime);
    Cthulhu.RenderData renderData = this.mRenderData[(int) iDataChannel];
    float num1 = System.Math.Min(this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude * 10f, 1f);
    renderData.Colorize.X = Cthulhu.ColdColor.X;
    renderData.Colorize.Y = Cthulhu.ColdColor.Y;
    renderData.Colorize.Z = Cthulhu.ColdColor.Z;
    renderData.Colorize.W = num1;
    this.mHitFlashTimer = System.Math.Max(this.mHitFlashTimer - iDeltaTime * 5f, 0.0f);
    renderData.BoundingSphere.Center = this.mTransform.Translation;
    renderData.Damage = (float) (1.0 - (double) this.mHitPoints / (double) this.mMaxHitPoints);
    renderData.Flash = this.mHitFlashTimer;
    this.ApplyColdEffect(iDeltaTime);
    this.ApplyWetSpecularEffect(renderData, iDeltaTime);
    Vector3 vector3 = new Vector3(0.75f, 0.8f, 1f);
    Vector3 diffuseColor = renderData.Material.DiffuseColor;
    Vector3.Multiply(ref renderData.Material.DiffuseColor, ref vector3, out renderData.Material.DiffuseColor);
    Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, (Array) renderData.Skeleton, renderData.Skeleton.Length);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) renderData);
    renderData.Material.DiffuseColor = diffuseColor;
    if ((this.mCurrentState == Cthulhu.States.Submerge || this.mCurrentState == Cthulhu.States.Death) && EffectManager.Instance.IsActive(ref this.mDeflectionEffect))
      this.mDeflectionTimer = System.Math.Min(this.mDeflectionTimer, 0.2f);
    if ((double) this.mDeflectionTimer > 0.0)
    {
      this.mDeflectionTimer -= iDeltaTime;
      if (!EffectManager.Instance.IsActive(ref this.mDeflectionEffect))
        EffectManager.Instance.StartEffect(Cthulhu.DEFLECTION_EFFECT, ref this.mTransform, out this.mDeflectionEffect);
      EffectManager.Instance.UpdateOrientation(ref this.mDeflectionEffect, ref this.mTransform);
      double num2 = (double) this.mDeflectionAura.Execute((Magicka.GameLogic.Entities.Entity) this.mDamageZone, iDeltaTime, AuraTarget.Self, 0, 5f);
    }
    else
      EffectManager.Instance.Stop(ref this.mDeflectionEffect);
  }

  internal void TurnOnIdleEffect()
  {
    Matrix translation = Matrix.CreateTranslation(this.mTransform.Translation with
    {
      Y = this.WaterYpos
    });
    EffectManager.Instance.StartEffect(this.mIdleEffect, ref translation, out this.mIdleEffectRef);
  }

  internal void KillIdleEffect()
  {
    if (!EffectManager.Instance.IsActive(ref this.mIdleEffectRef))
      return;
    EffectManager.Instance.Stop(ref this.mIdleEffectRef);
  }

  private void ApplyColdEffect(float iDeltaTime)
  {
    if (this.HasStatus(StatusEffects.Cold))
    {
      if (!EffectManager.Instance.IsActive(ref this.mCustomColdEffectRef[0]))
      {
        EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mTransform, out this.mCustomColdEffectRef[0]);
        EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mHeadOrientation, out this.mCustomColdEffectRef[1]);
        EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mMouthAttachOrientation, out this.mCustomColdEffectRef[2]);
        EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mRightHandOrientation, out this.mCustomColdEffectRef[3]);
        EffectManager.Instance.StartEffect("cthulhu_cold".GetHashCodeCustom(), ref this.mLeftHandOrientation, out this.mCustomColdEffectRef[4]);
      }
      else
      {
        EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[0], ref this.mTransform);
        EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[1], ref this.mHeadOrientation);
        EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[2], ref this.mMouthAttachOrientation);
        EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[3], ref this.mRightHandOrientation);
        EffectManager.Instance.UpdateOrientation(ref this.mCustomColdEffectRef[4], ref this.mLeftHandOrientation);
      }
    }
    else
    {
      if (!EffectManager.Instance.IsActive(ref this.mCustomColdEffectRef[0]))
        return;
      EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[0]);
      EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[1]);
      EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[2]);
      EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[3]);
      EffectManager.Instance.Stop(ref this.mCustomColdEffectRef[4]);
    }
  }

  private void ApplyWetSpecularEffect(Cthulhu.RenderData iRenderData, float iDeltaTime)
  {
    if ((double) this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Magnitude > 0.0)
    {
      if ((double) this.mWetnessCounter < 1.0)
      {
        iRenderData.Material.SpecularAmount = (float) (8.0 - 6.0 * (1.0 - (double) this.mWetnessCounter));
        iRenderData.Material.SpecularPower = (float) (800.0 - 600.0 * (1.0 - (double) this.mWetnessCounter));
        this.mWetnessCounter += iDeltaTime;
      }
      else
      {
        this.mWetnessCounter = 1f;
        iRenderData.Material.SpecularAmount = 8f;
        iRenderData.Material.SpecularPower = 600f;
      }
    }
    else if ((double) this.mWetnessCounter > 0.0)
    {
      iRenderData.Material.SpecularAmount = (float) (8.0 - 6.0 * (1.0 - (double) this.mWetnessCounter));
      iRenderData.Material.SpecularPower = (float) (800.0 - 600.0 * (1.0 - (double) this.mWetnessCounter));
      this.mWetnessCounter -= iDeltaTime;
    }
    else
    {
      iRenderData.Material.SpecularAmount = 2f;
      iRenderData.Material.SpecularPower = 200f;
      this.mWetnessCounter = 0.0f;
    }
  }

  internal OtherworldlyBolt SpawnCultistMissile(ref Vector3 iPosition, float iSpeed)
  {
    OtherworldlyBolt otherworldlyBolt = this.mCultistMissileCache[0];
    this.mCultistMissileCache.RemoveAt(0);
    Vector3 forward = this.mTransform.Forward;
    otherworldlyBolt.Spawn(this.mPlayState, ref iPosition, ref forward, iSpeed);
    this.mCultistMissileCache.Add(otherworldlyBolt);
    return otherworldlyBolt;
  }

  internal void KillBolts()
  {
    for (int index = 0; index < this.mCultistMissileCache.Count; ++index)
      this.mCultistMissileCache[index].DestroyOnNetwork(false, false, (Magicka.GameLogic.Entities.Entity) null, true);
  }

  private void CheckIfCharactersAreClose(float iDeltaTime)
  {
    this.mCheckEnemiesInRangeTimer -= iDeltaTime;
    if ((double) this.mCheckEnemiesInRangeTimer >= 0.0)
      return;
    EntityManager entityManager = this.mPlayState.EntityManager;
    Vector3 translation = this.mTransform.Translation;
    List<Magicka.GameLogic.Entities.Entity> entities = entityManager.GetEntities(translation, 21f, true);
    bool flag = false;
    for (int index = 0; index < entities.Count && !flag; ++index)
      flag = entities[index] is Avatar avatar && !avatar.IsEthereal && !avatar.IsInvisibile;
    entityManager.ReturnEntityList(entities);
    if (flag)
      this.mSecondsWhileOutOfRange = 0;
    else
      ++this.mSecondsWhileOutOfRange;
    this.mCheckEnemiesInRangeTimer = 1f;
  }

  private void CheckMistDesirability(float iDeltaTime)
  {
    this.mCheckMistDesirabilityTimer -= iDeltaTime;
    if ((double) this.mCheckMistDesirabilityTimer > 0.0)
      return;
    EntityManager entityManager = this.mPlayState.EntityManager;
    Vector3 translation = this.mMistSpawnTransform.Translation;
    List<Magicka.GameLogic.Entities.Entity> entities = entityManager.GetEntities(translation, this.mMistCloud.Radius, true, true);
    float num1 = 2.5f;
    float num2 = 0.5f;
    float num3 = num2 / num1;
    float num4 = 0.0f;
    for (int index = 0; index < entities.Count; ++index)
    {
      Avatar avatar = entities[index] as Avatar;
      num4 += avatar == null || avatar.IsInvisibile ? 0.0f : num3;
    }
    entityManager.ReturnEntityList(entities);
    this.mMistDesirability += num4;
    this.mMistDesirability -= (float) (((double) num3 - (double) num4) / 2.0);
    this.mMistDesirability = MathHelper.Clamp(this.mMistDesirability, 0.0f, 1f);
    if ((double) this.mMistDesirability == 1.0)
      this.mActivateMist = true;
    else if ((double) this.mMistDesirability == 0.0)
      this.mActivateMist = false;
    this.mCheckMistDesirabilityTimer += num2;
  }

  private unsafe void ChangeState(Cthulhu.States iNewState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.mStates[(int) this.mCurrentState].OnExit(this);
    this.mPreviousState = this.mCurrentState;
    this.mCurrentState = iNewState;
    this.mStates[(int) this.mCurrentState].OnEnter(this);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    Cthulhu.ChangeStateMessage changeStateMessage;
    changeStateMessage.NewState = iNewState;
    BossFight.Instance.SendMessage<Cthulhu.ChangeStateMessage>((IBoss) this, (ushort) 1, (void*) &changeStateMessage, true);
  }

  private unsafe void ChangeStage(Cthulhu.Stages iNewStage)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.mStages[(int) this.mCurrentStage].OnExit(this);
    this.mCurrentStage = iNewStage;
    this.mStages[(int) this.mCurrentStage].OnEnter(this);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    Cthulhu.ChangeStageMessage changeStageMessage;
    changeStageMessage.NewStage = iNewStage;
    BossFight.Instance.SendMessage<Cthulhu.ChangeStageMessage>((IBoss) this, (ushort) 2, (void*) &changeStageMessage, true);
  }

  private int FindGoodSpot(float iRadius)
  {
    int goodSpot = -1;
    float num1 = float.MaxValue;
    for (int iSpawn = 0; iSpawn < this.mSpawnTransforms.Length; ++iSpawn)
    {
      if (!this.IsSpawnPointOccupied((Cthulhu.BossSpawnPoints) iSpawn))
      {
        Matrix mSpawnTransform = this.mSpawnTransforms[iSpawn];
        EntityManager entityManager = this.mPlayState.EntityManager;
        Vector3 translation = mSpawnTransform.Translation;
        List<Magicka.GameLogic.Entities.Entity> entities = entityManager.GetEntities(translation, iRadius, true);
        float num2 = 0.0f;
        bool flag = false;
        for (int index = 0; index < entities.Count; ++index)
        {
          if (entities[index] is Avatar avatar && !avatar.IsEthereal && !avatar.IsInvisibile)
          {
            flag = true;
            num2 += (avatar.Position - translation).LengthSquared();
          }
        }
        entityManager.ReturnEntityList(entities);
        if (flag && (double) num2 < (double) num1)
        {
          goodSpot = iSpawn;
          num1 = num2;
        }
      }
    }
    return goodSpot;
  }

  internal Matrix ChangeSpawnPoint(int iNewSpawnPoint, int iHandle)
  {
    this.LeaveSpawnTransform(iHandle);
    Cthulhu.BossSpawnPoints iSpawn = (Cthulhu.BossSpawnPoints) iNewSpawnPoint;
    Matrix spawnTransform = this.GetSpawnTransform(iSpawn);
    this.OccupySpawnTransform(iSpawn, iHandle);
    return spawnTransform;
  }

  private Matrix GetSpawnTransform(Cthulhu.BossSpawnPoints iSpawn)
  {
    return this.mSpawnTransforms[(int) iSpawn];
  }

  private void OccupySpawnTransform(Cthulhu.BossSpawnPoints iSpawn, int iHandle)
  {
    this.mOccupiedSpawnPoints[(int) iSpawn] = iHandle;
  }

  private void LeaveSpawnTransform(int iHandle)
  {
    for (int index = 0; index < this.mOccupiedSpawnPoints.Length; ++index)
    {
      if (this.mOccupiedSpawnPoints[index] == iHandle)
      {
        this.mOccupiedSpawnPoints[index] = -1;
        break;
      }
    }
  }

  internal void LeaveSpawnTransform(Tentacle iTentacle)
  {
    this.LeaveSpawnTransform((int) iTentacle.Handle);
  }

  private bool IsAtPoint(Cthulhu.BossSpawnPoints iPoint)
  {
    return this.mOccupiedSpawnPoints[(int) iPoint] == (int) this.mDamageZone.Handle;
  }

  private bool IsSpawnPointOccupied(Cthulhu.BossSpawnPoints iSpawn)
  {
    return this.mOccupiedSpawnPoints[(int) iSpawn] != -1;
  }

  internal void SpawnNewTentacleAtGoodPoint()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.SpawnTentacleAtGoodPoint((Tentacle) null);
  }

  private int FindRandomSpot()
  {
    bool flag = false;
    int iSpawn = -1;
    int num = 0;
    for (int index = 200; !flag && num < index; ++num)
    {
      iSpawn = Cthulhu.RANDOM.Next(this.mSpawnTransforms.Length);
      flag = !this.IsSpawnPointOccupied((Cthulhu.BossSpawnPoints) iSpawn);
    }
    return iSpawn;
  }

  internal unsafe bool SpawnTentacleAtGoodPoint(Tentacle iTentacle)
  {
    if (iTentacle == null)
    {
      if (this.mInactiveTentacles.Count <= 0)
        return false;
      iTentacle = this.mInactiveTentacles[0];
      this.AddTentacleToActiveList(0);
      if (NetworkManager.Instance.State == NetworkState.Server)
        BossFight.Instance.SendMessage<Cthulhu.TentacleSpawnMessage>((IBoss) this, (ushort) 13, (void*) &new Cthulhu.TentacleSpawnMessage()
        {
          TentacleIndex = (byte) iTentacle.ID
        }, true);
    }
    int iNewSpawnPoint = this.FindGoodSpot(18f);
    if (iNewSpawnPoint == -1)
    {
      int randomSpot = this.FindRandomSpot();
      if (randomSpot == -1)
        return false;
      iNewSpawnPoint = randomSpot;
    }
    Matrix iTransform = this.ChangeSpawnPoint(iNewSpawnPoint, (int) iTentacle.Handle);
    if (NetworkManager.Instance.State == NetworkState.Server)
      BossFight.Instance.SendMessage<Cthulhu.SpawnPointMessage>((IBoss) this, (ushort) 4, (void*) &new Cthulhu.SpawnPointMessage()
      {
        Index = (sbyte) iTentacle.ID,
        SpawnPoint = iNewSpawnPoint
      }, true);
    iTentacle.Start(iTransform);
    return true;
  }

  internal void ActivateMist()
  {
    if (this.mMistCloud.Active)
      return;
    this.mMistCloud.Initialize(this.mMistSpawnTransform.Translation);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mMistCloud);
  }

  protected bool GetRandomTarget(out Avatar oAvatar, ref Vector3 iSource, bool iIgnoreProtected)
  {
    oAvatar = (Avatar) null;
    bool randomTarget = false;
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < this.mPlayers.Length; ++index)
        num2 |= 1 << index;
      while (!randomTarget)
      {
        int index1 = Cthulhu.RANDOM.Next(this.mPlayers.Length);
        int num3 = 1 << index1;
        if ((num1 & num2) == num2)
          return false;
        if ((num1 & num3) != num3)
        {
          num1 |= num3;
          if (this.mPlayers[index1].Playing && this.mPlayers[index1].Avatar != null && !this.mPlayers[index1].Avatar.Dead)
          {
            Avatar avatar = this.mPlayers[index1].Avatar;
            if (!iIgnoreProtected)
            {
              List<Shield> shields = this.mPlayState.EntityManager.Shields;
              bool flag = false;
              for (int index2 = 0; index2 < shields.Count && !flag; ++index2)
              {
                Vector3 position = avatar.Position;
                Vector3.Subtract(ref position, ref iSource, out position);
                Segment iSeg = new Segment(iSource, position);
                flag = shields[index2].SegmentIntersect(out position, iSeg, 0.0f);
              }
              if (flag)
                continue;
            }
            oAvatar = avatar;
            randomTarget = true;
          }
        }
      }
    }
    return randomTarget;
  }

  internal float TimeBetweenAttacks => this.mTimeBetweenActions;

  internal float StageSpeedModifier => this.mStageSpeedModifier;

  internal float StateSpeedModifier => this.mStateSpeedModifier;

  private void StartClip(Cthulhu.Animations anim, bool loop)
  {
    this.mAnimationController.StartClip(this.mAnimations[(int) anim], loop);
  }

  private void StopClip() => this.mAnimationController.Stop();

  private void CrossFade(Cthulhu.Animations anim, float time, bool loop)
  {
    this.mAnimationController.CrossFade(this.mAnimations[(int) anim], time, loop);
  }

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Magicka.GameLogic.Entities.Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult damageResult = DamageResult.None;
    switch (iPartIndex)
    {
      case 0:
        damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
        break;
      case 5:
        damageResult = this.mMistCloud.Damage(iDamage, iAttacker, ref iAttackPosition);
        break;
    }
    return damageResult;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
    switch (iPartIndex)
    {
      case 0:
        this.Damage(iDamage, iElement);
        break;
      case 5:
        this.mMistCloud.Damage(iDamage, iElement);
        break;
    }
  }

  public void SetSlow(int iIndex)
  {
  }

  public void DeInitialize()
  {
  }

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return false;
  }

  public override bool Dead => this.mDead;

  public float MaxHitPoints => this.mMaxHitPoints;

  public float HitPoints => this.mHitPoints;

  public bool HasStatus(int iIndex, StatusEffects iStatus) => this.HasStatus(iStatus);

  public float StatusMagnitude(int iIndex, StatusEffects iStatus) => this.StatusMagnitude(iStatus);

  public void ScriptMessage(BossMessages iMessage)
  {
    if (iMessage != BossMessages.StartFight)
      return;
    this.mOkToFight = true;
  }

  private unsafe void NetworkUpdate()
  {
    NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
    Cthulhu.UpdateMessage updateMessage = new Cthulhu.UpdateMessage();
    updateMessage.Animation = (byte) 0;
    while ((int) updateMessage.Animation < this.mAnimations.Length && this.mAnimationController.AnimationClip != this.mAnimations[(int) updateMessage.Animation])
      ++updateMessage.Animation;
    updateMessage.Hitpoints = this.mHitPoints;
    for (int index = 0; index < networkServer.Connections; ++index)
    {
      float num = networkServer.GetLatency(index) * 0.5f;
      BossFight.Instance.SendMessage<Cthulhu.UpdateMessage>((IBoss) this, (ushort) 0, (void*) &(updateMessage with
      {
        AnimationTime = new HalfSingle(this.mAnimationController.Time + num)
      }), false, index);
    }
  }

  private void AddTentacleToActiveList(int iIndex)
  {
    Tentacle inactiveTentacle = this.mInactiveTentacles[iIndex];
    this.mInactiveTentacles.RemoveAt(iIndex);
    this.mActiveTentacles.Add(inactiveTentacle);
    if (this.mPlayState.EntityManager.Entities.Contains((Magicka.GameLogic.Entities.Entity) inactiveTentacle))
      return;
    inactiveTentacle.Initialize();
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) inactiveTentacle);
  }

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    switch (iMsg.Type)
    {
      case 0:
        if ((double) iMsg.TimeStamp < (double) this.mLastNetworkUpdate)
          break;
        this.mLastNetworkUpdate = (float) iMsg.TimeStamp;
        Cthulhu.UpdateMessage updateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &updateMessage);
        if (this.mAnimationController.AnimationClip != this.mAnimations[(int) updateMessage.Animation])
          this.mAnimationController.StartClip(this.mAnimations[(int) updateMessage.Animation], false);
        this.mAnimationController.Time = updateMessage.AnimationTime.ToSingle();
        this.mHitPoints = updateMessage.Hitpoints;
        break;
      case 1:
        Cthulhu.ChangeStateMessage changeStateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
        if (changeStateMessage.NewState == Cthulhu.States.NR_OF_STATES)
          break;
        this.mStates[(int) this.mCurrentState].OnExit(this);
        this.mPreviousState = this.mCurrentState;
        this.mCurrentState = changeStateMessage.NewState;
        this.mStates[(int) this.mCurrentState].OnEnter(this);
        break;
      case 2:
        Cthulhu.ChangeStageMessage changeStageMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStageMessage);
        if (changeStageMessage.NewStage == Cthulhu.Stages.NR_OF_STAGES)
          break;
        this.mStages[(int) this.mCurrentStage].OnExit(this);
        this.mCurrentStage = changeStageMessage.NewStage;
        this.mStages[(int) this.mCurrentStage].OnEnter(this);
        break;
      case 3:
        Cthulhu.ChangeTargetMessage changeTargetMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeTargetMessage);
        Magicka.GameLogic.Entities.Entity.GetFromHandle(changeTargetMessage.Handle);
        break;
      case 4:
        Cthulhu.SpawnPointMessage iMsg1;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &iMsg1);
        if (iMsg1.Index == (sbyte) -1)
        {
          if (iMsg1.SpawnPoint == -1)
            break;
          this.mDesiredSpawnPoint = iMsg1.SpawnPoint;
          this.mTransform = this.ChangeSpawnPoint(this.mDesiredSpawnPoint, (int) this.mDamageZone.Handle);
          break;
        }
        if (iMsg1.SpawnPoint == -1)
          break;
        this.mTentacles[(int) iMsg1.Index].NetworkSpawnPoint(ref iMsg1);
        break;
      case 5:
        Cthulhu.HypnotizeMessage hypnotizeMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &hypnotizeMessage);
        Magicka.GameLogic.Entities.Entity fromHandle1 = Magicka.GameLogic.Entities.Entity.GetFromHandle((int) hypnotizeMessage.Handle);
        Vector3 vector3 = hypnotizeMessage.Direction.ToVector3();
        if (!(fromHandle1 is Avatar avatar))
          break;
        avatar.Hypnotize(ref vector3, this.mPlayerHypnotizeEffect);
        break;
      case 6:
        TimeWarp.Instance.Execute((ISpellCaster) this.mDamageZone, this.mPlayState);
        break;
      case 7:
        this.ActivateMist();
        break;
      case 8:
        Cthulhu.SetCharacterToEatMessage characterToEatMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &characterToEatMessage);
        if (!(Magicka.GameLogic.Entities.Entity.GetFromHandle((int) characterToEatMessage.Handle) is Magicka.GameLogic.Entities.Character fromHandle2))
          break;
        this.mCharacterToEat = fromHandle2;
        break;
      case 9:
        Cthulhu.CharmAndConfuseMessage andConfuseMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &andConfuseMessage);
        if (!(Magicka.GameLogic.Entities.Entity.GetFromHandle((int) andConfuseMessage.Handle) is Avatar fromHandle3))
          break;
        this.mMistCloud.CharmAndConfuse(fromHandle3);
        break;
      case 10:
        this.mDeflectionTimer = 8f;
        break;
      case 11:
        Cthulhu.TentacleUpdateMessage iMsg2;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &iMsg2);
        this.mTentacles[(int) iMsg2.TentacleIndex].NetworkUpdate(ref iMsg2, (float) iMsg.TimeStamp);
        break;
      case 12:
        Cthulhu.TentacleChangeStateMessage iMsg3;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &iMsg3);
        this.mTentacles[(int) iMsg3.TentacleIndex].NetworkChangeState(ref iMsg3);
        break;
      case 13:
        Cthulhu.TentacleSpawnMessage tentacleSpawnMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &tentacleSpawnMessage);
        for (int index = 0; index < this.mInactiveTentacles.Count; ++index)
        {
          if (this.mInactiveTentacles[index].ID == (int) tentacleSpawnMessage.TentacleIndex)
          {
            this.AddTentacleToActiveList(index);
            break;
          }
        }
        break;
      case 14:
        Cthulhu.TentacleGrabMessage tentacleGrabMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &tentacleGrabMessage);
        if (!(Magicka.GameLogic.Entities.Entity.GetFromHandle((int) tentacleGrabMessage.Handle) is IDamageable fromHandle4))
          break;
        this.mTentacles[(int) tentacleGrabMessage.TentacleIndex].GrabDamageable(fromHandle4);
        break;
      case 15:
        Cthulhu.TentacleAimTargetMessage aimTargetMessage1;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &aimTargetMessage1);
        if (!(Magicka.GameLogic.Entities.Entity.GetFromHandle((int) aimTargetMessage1.Handle) is Avatar fromHandle5))
          break;
        this.mTentacles[(int) aimTargetMessage1.TentacleIndex].SetAimTarget(fromHandle5);
        break;
      case 16 /*0x10*/:
        Cthulhu.TentacleReleaseAimTargetMessage aimTargetMessage2;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &aimTargetMessage2);
        this.mTentacles[(int) aimTargetMessage2.TentacleIndex].SetAimTarget((Avatar) null);
        break;
    }
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
  }

  public bool NetworkInitialized => true;

  public BossEnum GetBossType() => BossEnum.Cthulhu;

  protected override BossDamageZone Entity => (BossDamageZone) this.mDamageZone;

  protected override float Radius => 1.2f;

  protected override float Length => 4f;

  protected override int BloodEffect => Cthulhu.BLOOD_BLACK_EFFECT;

  protected override Vector3 NotifierTextPostion
  {
    get
    {
      Vector3 translation = this.mHeadOrientation.Translation;
      translation.Y += 3f;
      return translation;
    }
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

  public void AddSelfShield(int iIndex, Spell iSpell)
  {
  }

  public void RemoveSelfShield(int iIndex, Magicka.GameLogic.Entities.Character.SelfShieldType iType)
  {
  }

  public Magicka.GameLogic.Spells.CastType CastType(int iIndex) => Magicka.GameLogic.Spells.CastType.None;

  public float SpellPower(int iIndex) => 1f;

  public void SpellPower(int iIndex, float iSpellPower)
  {
  }

  public SpellEffect CurrentSpell(int iIndex) => this.mSpellEffect;

  public void CurrentSpell(int iIndex, SpellEffect iEffect) => this.mSpellEffect = iEffect;

  internal void ClearAllStatusEffects()
  {
    for (int index = 0; index < this.mStatusEffects.Length; ++index)
    {
      this.mStatusEffects[index].Stop();
      this.mStatusEffects[index] = new StatusEffect();
    }
  }

  protected override DamageResult Damage(
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
          damage.Amount = (float) (int) System.Math.Max(damage.Amount + this.mResistances[iIndex].Modifier, 0.0f);
        else
          damage.Amount += (float) (int) this.mResistances[iIndex].Modifier;
        num1 += this.mResistances[iIndex].Multiplier;
        ++num2;
      }
    }
    if ((double) num2 != 0.0)
      damage.Magnitude *= num1 / num2;
    if ((double) System.Math.Abs(damage.Magnitude) <= 1.4012984643248171E-45)
      damageResult |= DamageResult.Deflected;
    if ((damageResult & DamageResult.Deflected) == DamageResult.Deflected)
      return damageResult;
    if ((damage.AttackProperty & AttackProperties.Status) == AttackProperties.Status && (double) System.Math.Abs(num1) > 1.4012984643248171E-45)
    {
      if ((damage.Element & Elements.Fire) == Elements.Fire && (double) this.mResistances[Spell.ElementIndex(Elements.Fire)].Multiplier > 1.4012984643248171E-45 && (this.HasStatus(StatusEffects.Wet) || this.HasStatus(StatusEffects.Cold)))
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Burning, damage.Amount, damage.Magnitude, this.Length, this.Radius));
      if ((damage.Element & Elements.Cold) == Elements.Cold && (double) this.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Cold, damage.Amount, damage.Magnitude, this.Length, this.Radius * 4f));
      if ((damage.Element & Elements.Water) == Elements.Water && (double) this.mResistances[Spell.ElementIndex(Elements.Water)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, this.Length, this.Radius));
      if ((damage.Element & Elements.Steam) == Elements.Steam && (double) this.mResistances[Spell.ElementIndex(Elements.Steam)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, this.Length, this.Radius));
    }
    if ((damage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage)
    {
      if ((damage.Element & Elements.Lightning) == Elements.Lightning && this.HasStatus(StatusEffects.Wet))
        damage.Amount *= 2f;
      if ((damage.Element & Elements.PhysicalElements) != Elements.None)
      {
        if (this.HasStatus(StatusEffects.Frozen))
        {
          damage.Amount = System.Math.Max(damage.Amount - 200f, 0.0f);
          damage.Magnitude = System.Math.Max(1f, damage.Magnitude);
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
      if ((double) damage.Amount > 0.0)
      {
        this.mHitFlashTimer = 0.5f;
        this.mTimeSinceLastDamageTimewarp = 0.0f;
      }
      if ((damage.AttackProperty & AttackProperties.Piercing) != (AttackProperties) 0 && (double) damage.Magnitude > 0.0 && (double) damage.Amount > 0.0)
        damageResult |= DamageResult.Pierced;
      if ((double) damage.Amount > 0.0)
        damageResult |= DamageResult.Damaged;
      if ((double) damage.Amount == 0.0)
        damageResult |= DamageResult.Deflected;
      if ((double) damage.Amount < 0.0)
        damageResult |= DamageResult.Healed;
      damageResult |= DamageResult.Hit;
      if (Defines.FeatureNotify(iFeatures))
      {
        if ((double) damage.Amount != 0.0)
          this.mTimeSinceLastDamage = 0.0f;
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

  protected override void UpdateStatusEffects(float iDeltaTime)
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
        if (this.mStatusEffects[index].DamageType == StatusEffects.Wet)
          this.mStatusEffects[index].StopEffect();
        else
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

  public enum Stages
  {
    Intro,
    Battle,
    LateBattle,
    Critical,
    Final,
    NR_OF_STAGES,
  }

  private class IntroStage : IBossState<Cthulhu>
  {
    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.mStageSpeedModifier = 1f;
      iOwner.mTimeBetweenActions = 1.5f;
      iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.9f;
      iOwner.mNumberOfTentacles = 2;
      for (int index = 0; index < iOwner.mNumberOfTentacles; ++index)
        iOwner.SpawnNewTentacleAtGoodPoint();
      iOwner.GetRageState.Callback = (Action<Cthulhu>) null;
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      for (int index = 0; index < iOwner.mStates.Length; ++index)
        iOwner.mStates[index].NonActiveUpdate(iOwner, iDeltaTime);
      if (iOwner.mCurrentState != Cthulhu.States.Idle || !iOwner.GetIdleState.Done)
        return;
      if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Emerge);
      else if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Submerge);
      else if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
      {
        iOwner.ChangeState(Cthulhu.States.Lightning);
      }
      else
      {
        if (!iOwner.GetDevourState.Active(iOwner, iDeltaTime))
          return;
        iOwner.ChangeState(Cthulhu.States.Devour);
      }
    }

    public void OnExit(Cthulhu iOwner)
    {
    }
  }

  private class BattleStage : IBossState<Cthulhu>
  {
    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.mStageSpeedModifier = 1f;
      iOwner.mTimeBetweenActions = 1f;
      iOwner.mNumberOfTentacles = 2;
      iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.7f;
      iOwner.GetRageState.Callback = (Action<Cthulhu>) null;
      iOwner.ChangeState(Cthulhu.States.NewStage);
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      for (int index = 0; index < iOwner.mStates.Length; ++index)
        iOwner.mStates[index].NonActiveUpdate(iOwner, iDeltaTime);
      if (iOwner.mCurrentState != Cthulhu.States.Idle || !iOwner.GetIdleState.Done)
        return;
      if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Emerge);
      else if (iOwner.GetMistState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Mist);
      else if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Submerge);
      else if (iOwner.GetLesserCallofCthulhuState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Lesser_Call_of_Cthulhu);
      else if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Lightning);
      else if (iOwner.GetHypnotizeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Hypnotize);
      else if (iOwner.GetOtherworldlyBoltState.Active(iOwner, iDeltaTime))
      {
        iOwner.ChangeState(Cthulhu.States.OtherworldlyBolt);
      }
      else
      {
        if (!iOwner.GetDevourState.Active(iOwner, iDeltaTime))
          return;
        iOwner.ChangeState(Cthulhu.States.Devour);
      }
    }

    public void OnExit(Cthulhu iOwner)
    {
    }
  }

  private class LateBattleStage : IBossState<Cthulhu>
  {
    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.mStageSpeedModifier = 1f;
      iOwner.mTimeBetweenActions = 1f;
      iOwner.mNumberOfTentacles = 4;
      iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.5f;
      iOwner.GetRageState.Callback += new Action<Cthulhu>(this.SpawnTentacles);
      iOwner.ChangeState(Cthulhu.States.NewStage);
    }

    private void SpawnTentacles(Cthulhu iOwner)
    {
      int num = iOwner.mNumberOfTentacles - iOwner.mActiveTentacles.Count;
      for (int index = 0; index < num; ++index)
        iOwner.SpawnNewTentacleAtGoodPoint();
      iOwner.GetRageState.Callback = (Action<Cthulhu>) null;
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      for (int index = 0; index < iOwner.mStates.Length; ++index)
        iOwner.mStates[index].NonActiveUpdate(iOwner, iDeltaTime);
      if (iOwner.mCurrentState != Cthulhu.States.Idle || !iOwner.GetIdleState.Done)
        return;
      if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Emerge);
      else if (iOwner.GetMistState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Mist);
      else if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Submerge);
      else if (iOwner.GetLesserCallofCthulhuState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Lesser_Call_of_Cthulhu);
      else if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Lightning);
      else if (iOwner.GetHypnotizeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Hypnotize);
      else if (iOwner.GetOtherworldlyBoltState.Active(iOwner, iDeltaTime))
      {
        iOwner.ChangeState(Cthulhu.States.OtherworldlyBolt);
      }
      else
      {
        if (!iOwner.GetDevourState.Active(iOwner, iDeltaTime))
          return;
        iOwner.ChangeState(Cthulhu.States.Devour);
      }
    }

    public void OnExit(Cthulhu iOwner)
    {
    }
  }

  private class CriticalStage : IBossState<Cthulhu>
  {
    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.mStageSpeedModifier = 1.2f;
      iOwner.mTimeBetweenActions = 0.75f;
      iOwner.mNumberOfTentacles = 4;
      iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.25f;
      iOwner.GetRageState.Callback += new Action<Cthulhu>(this.SpawnTentacles);
      iOwner.ChangeState(Cthulhu.States.NewStage);
    }

    private void SpawnTentacles(Cthulhu iOwner)
    {
      int num = iOwner.mNumberOfTentacles - iOwner.mActiveTentacles.Count;
      for (int index = 0; index < num; ++index)
        iOwner.SpawnNewTentacleAtGoodPoint();
      iOwner.GetRageState.Callback = (Action<Cthulhu>) null;
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      for (int index = 0; index < iOwner.mStates.Length; ++index)
        iOwner.mStates[index].NonActiveUpdate(iOwner, iDeltaTime);
      if (iOwner.mCurrentState != Cthulhu.States.Idle || !iOwner.GetIdleState.Done)
        return;
      if (!iOwner.mStarSpawnSpawned && iOwner.GetCallofCthulhuState.Active(iOwner, iDeltaTime))
      {
        iOwner.ChangeState(Cthulhu.States.Call_of_Cthulhu);
        iOwner.mStarSpawnSpawned = true;
      }
      else if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Emerge);
      else if (iOwner.GetTimewarpState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Timewarp);
      else if (iOwner.GetMistState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Mist);
      else if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Submerge);
      else if (iOwner.GetLesserCallofCthulhuState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Lesser_Call_of_Cthulhu);
      else if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Lightning);
      else if (iOwner.GetHypnotizeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Hypnotize);
      else if (iOwner.GetOtherworldlyBoltState.Active(iOwner, iDeltaTime))
      {
        iOwner.ChangeState(Cthulhu.States.OtherworldlyBolt);
      }
      else
      {
        if (!iOwner.GetDevourState.Active(iOwner, iDeltaTime))
          return;
        iOwner.ChangeState(Cthulhu.States.Devour);
      }
    }

    public void OnExit(Cthulhu iOwner)
    {
    }
  }

  private class FinalStage : IBossState<Cthulhu>
  {
    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.mStageSpeedModifier = 1.3f;
      iOwner.mTimeBetweenActions = 0.5f;
      iOwner.mNumberOfTentacles = 4;
      iOwner.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier = 0.25f;
      iOwner.GetRageState.Callback += new Action<Cthulhu>(this.SpawnTentacles);
      iOwner.ChangeState(Cthulhu.States.NewStage);
    }

    private void SpawnTentacles(Cthulhu iOwner)
    {
      int num = iOwner.mNumberOfTentacles - iOwner.mActiveTentacles.Count;
      for (int index = 0; index < num; ++index)
        iOwner.SpawnNewTentacleAtGoodPoint();
      iOwner.GetRageState.Callback = (Action<Cthulhu>) null;
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      for (int index = 0; index < iOwner.mStates.Length; ++index)
        iOwner.mStates[index].NonActiveUpdate(iOwner, iDeltaTime);
      if (iOwner.mCurrentState != Cthulhu.States.Idle || !iOwner.GetIdleState.Done)
        return;
      if (!iOwner.mStarSpawnSpawned && iOwner.GetCallofCthulhuState.Active(iOwner, iDeltaTime))
      {
        iOwner.ChangeState(Cthulhu.States.Call_of_Cthulhu);
        iOwner.mStarSpawnSpawned = true;
      }
      else if (iOwner.GetMistState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Mist);
      else if (iOwner.GetTimewarpState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Timewarp);
      else if (iOwner.GetLesserCallofCthulhuState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Lesser_Call_of_Cthulhu);
      else if (iOwner.GetEmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Emerge);
      else if (iOwner.GetSubmergeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Submerge);
      else if (iOwner.GetLightningState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Lightning);
      else if (iOwner.GetHypnotizeState.Active(iOwner, iDeltaTime))
        iOwner.ChangeState(Cthulhu.States.Hypnotize);
      else if (iOwner.GetOtherworldlyBoltState.Active(iOwner, iDeltaTime))
      {
        iOwner.ChangeState(Cthulhu.States.OtherworldlyBolt);
      }
      else
      {
        if (!iOwner.GetDevourState.Active(iOwner, iDeltaTime))
          return;
        iOwner.ChangeState(Cthulhu.States.Devour);
      }
    }

    public void OnExit(Cthulhu iOwner)
    {
    }
  }

  public enum States
  {
    Idle,
    Emerge,
    Submerge,
    Devour,
    DevourHit,
    Lightning,
    Mist,
    Call_of_Cthulhu,
    Lesser_Call_of_Cthulhu,
    Timewarp,
    Hypnotize,
    NewStage,
    OtherworldlyBolt,
    Death,
    NR_OF_STATES,
  }

  private interface CthulhuState : IBossState<Cthulhu>
  {
    bool Active(Cthulhu iOwner, float iDeltaTime);

    void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime);
  }

  private class IdleState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private float mTimer;
    public bool Done;

    public void OnEnter(Cthulhu iOwner)
    {
      float time = iOwner.mPreviousState != Cthulhu.States.Devour ? 0.05f : 0.4f;
      iOwner.CrossFade(Cthulhu.Animations.Idle, time, true);
      this.mTimer = iOwner.mTimeBetweenActions;
      this.Done = false;
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      this.mTimer -= iDeltaTime;
      if ((double) this.mTimer >= 0.0 || !iOwner.mOkToFight)
        return;
      this.Done = true;
    }

    public void OnExit(Cthulhu iOwner)
    {
    }

    public bool Active(Cthulhu iOwner, float iDeltaTime) => false;

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
    }
  }

  private class MistState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private const float TIME = 0.2631579f;
    private bool mCasted;
    private float mCooldown;

    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.CrossFade(Cthulhu.Animations.Mists, 0.4f, false);
      this.mCasted = false;
      AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_MIST, iOwner.mDamageZone.AudioEmitter);
    }

    public unsafe void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.ChangeState(Cthulhu.States.Idle);
      }
      else
      {
        if (this.mCasted || (double) num < 0.26315790414810181)
          return;
        iOwner.ActivateMist();
        this.mCasted = true;
        if (NetworkManager.Instance.State != NetworkState.Server)
          return;
        Cthulhu.ActivateMistMessage activateMistMessage = new Cthulhu.ActivateMistMessage();
        BossFight.Instance.SendMessage<Cthulhu.ActivateMistMessage>((IBoss) iOwner, (ushort) 7, (void*) &activateMistMessage, true);
      }
    }

    public void OnExit(Cthulhu iOwner) => this.mCooldown = 10f;

    public bool Active(Cthulhu iOwner, float iDeltaTime)
    {
      return (double) this.mCooldown <= 0.0 && iOwner.mActivateMist && !iOwner.mMistCloud.Active;
    }

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
      if ((double) this.mCooldown < 0.0)
        return;
      this.mCooldown -= iDeltaTime;
    }
  }

  private class EmergeState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private const float LOGIC_EXECUTE_TIME = 0.218181819f;
    private const float PFX_EXECUTE_TIME = 0.0f;
    private const float UNFREEZE_TIME = 0.07272727f;
    private VisualEffectReference mEffectRef;
    private int WaterSplashEffect = "cthulhu_emerge_water_splash".GetHashCodeCustom();
    private VisualEffectReference mBubbleEffectRef;
    private int BubbleEffect = "cthulhu_intro_bubbles".GetHashCodeCustom();
    private bool mLogicDone;
    private bool mEffectDone;
    private bool mUnfreezeDone;
    private bool mBubblesDone;
    private float mBubbleTimer;

    public void OnEnter(Cthulhu iOwner)
    {
      this.mEffectDone = this.mLogicDone = false;
      iOwner.mHPOnLastEmerge = iOwner.HitPoints;
      iOwner.mTimeSinceLastEmerge = 0.0f;
      iOwner.mTimeUntilSubmerge = (float) (30.0 + Cthulhu.RANDOM.NextDouble() * 5.0);
      if (iOwner.mDesiredSpawnPoint != -1)
      {
        Matrix matrix = iOwner.ChangeSpawnPoint(iOwner.mDesiredSpawnPoint, (int) iOwner.mDamageZone.Handle);
        iOwner.mTransform = matrix;
      }
      StatusEffect iStatusEffect = new StatusEffect(StatusEffects.Wet, 0.0f, 1f, 1f, 1f);
      int num = (int) iOwner.AddStatusEffect(iStatusEffect);
      this.mUnfreezeDone = false;
      if (iOwner.InitialEmerge || iOwner.mTheKingHasFallen)
      {
        this.mBubblesDone = true;
        this.mBubbleTimer = -1f;
        iOwner.StartClip(Cthulhu.Animations.Emerge, false);
      }
      else
      {
        this.mBubblesDone = false;
        this.mBubbleTimer = 1.5f;
        iOwner.StartClip(Cthulhu.Animations.Emerge, false);
        iOwner.StopClip();
      }
      AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_EMERGE, iOwner.mDamageZone.AudioEmitter);
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if ((double) this.mBubbleTimer >= 0.0)
      {
        this.mBubbleTimer -= iDeltaTime;
        if ((double) this.mBubbleTimer <= 0.0)
        {
          iOwner.StartClip(Cthulhu.Animations.Emerge, false);
          if (!EffectManager.Instance.IsActive(ref this.mBubbleEffectRef))
            return;
          EffectManager.Instance.Stop(ref this.mBubbleEffectRef);
        }
        else
        {
          if (this.mBubblesDone || EffectManager.Instance.IsActive(ref this.mBubbleEffectRef))
            return;
          Matrix translation = Matrix.CreateTranslation(iOwner.mTransform.Translation with
          {
            Y = iOwner.WaterYpos
          });
          EffectManager.Instance.StartEffect(this.BubbleEffect, ref translation, out this.mBubbleEffectRef);
          this.mBubblesDone = true;
        }
      }
      else
      {
        float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if (iOwner.mTheKingHasFallen && (double) num1 >= 0.41999998688697815)
          iOwner.ChangeState(Cthulhu.States.Death);
        else if (iOwner.mAnimationController.HasFinished)
        {
          iOwner.ChangeState(Cthulhu.States.Idle);
        }
        else
        {
          if (!this.mUnfreezeDone && (double) num1 >= 0.072727270424366)
          {
            Vector3 translation = iOwner.mTransform.Translation;
            Vector3 result = Vector3.Right;
            Vector3.Multiply(ref result, 4f, out result);
            Magicka.GameLogic.Damage iDamage = new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Fire, 100f, 4f);
            Liquid.Freeze(iOwner.mPlayState.Level.CurrentScene, ref translation, ref result, 6.28318548f, 2f, ref iDamage);
            this.mUnfreezeDone = true;
          }
          if (!this.mLogicDone && (double) num1 >= 0.21818181872367859)
          {
            Magicka.GameLogic.Entities.Entity entity = (Magicka.GameLogic.Entities.Entity) iOwner.Entity;
            Vector3 position = entity.Position;
            Magicka.GameLogic.Damage iDamage1 = new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, 100f, 4f);
            int num2 = (int) Helper.CircleDamage(iOwner.mPlayState, entity, iOwner.mPlayState.PlayTime, entity, ref position, 6f, ref iDamage1);
            Magicka.GameLogic.Damage iDamage2 = new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Water, 100f, 1f);
            int num3 = (int) Helper.CircleDamage(iOwner.mPlayState, entity, iOwner.mPlayState.PlayTime, entity, ref position, 10f, ref iDamage2);
            this.mLogicDone = true;
          }
          else
          {
            if (this.mEffectDone || (double) num1 < 0.0)
              return;
            if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
            {
              Matrix translation = Matrix.CreateTranslation(iOwner.mTransform.Translation with
              {
                Y = 0.0f
              });
              EffectManager.Instance.StartEffect(this.WaterSplashEffect, ref translation, out this.mEffectRef);
            }
            this.mEffectDone = true;
            iOwner.TurnOnIdleEffect();
          }
        }
      }
    }

    public void OnExit(Cthulhu iOwner)
    {
      iOwner.mInitialEmerge = false;
      if (!EffectManager.Instance.IsActive(ref this.mBubbleEffectRef))
        return;
      EffectManager.Instance.Stop(ref this.mBubbleEffectRef);
    }

    public bool Active(Cthulhu iOwner, float iDeltaTime) => false;

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
    }
  }

  private class SubmergeState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private const float LOGIC_EXECUTE_TIME = 0.6976744f;
    private const float PFX_EXECUTE_TIME = 0.5813953f;
    private float mTimer;
    private VisualEffectReference mEffectRef;
    private int WaterSplashEffect = "cthulhu_emerge_water_splash".GetHashCodeCustom();
    private bool mLogicDone;
    private bool mEffectDone;

    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.CrossFade(Cthulhu.Animations.Submerge, 0.4f, false);
      this.mEffectDone = this.mLogicDone = false;
      this.mTimer = 2f;
      iOwner.mDeflectionTimer = 0.0f;
      AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_SUBMERGE, iOwner.mDamageZone.AudioEmitter);
    }

    public unsafe void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (!iOwner.mAnimationController.CrossFadeEnabled && iOwner.mAnimationController.HasFinished)
      {
        if ((double) this.mTimer <= 0.0)
        {
          if (NetworkManager.Instance.State != NetworkState.Client)
          {
            int iNewSpawnPoint = iOwner.mDesiredSpawnPoint;
            iOwner.mDesiredSpawnPoint = -1;
            if (iNewSpawnPoint == -1)
              iNewSpawnPoint = iOwner.FindGoodSpot(18f);
            if (iNewSpawnPoint == -1)
              iNewSpawnPoint = iOwner.FindRandomSpot();
            if (iNewSpawnPoint != -1)
            {
              Matrix matrix = iOwner.ChangeSpawnPoint(iNewSpawnPoint, (int) iOwner.mDamageZone.Handle);
              iOwner.mTransform = matrix;
            }
            if (NetworkManager.Instance.State == NetworkState.Server)
              BossFight.Instance.SendMessage<Cthulhu.SpawnPointMessage>((IBoss) iOwner, (ushort) 4, (void*) &new Cthulhu.SpawnPointMessage()
              {
                Index = (sbyte) -1,
                SpawnPoint = iNewSpawnPoint
              }, true);
          }
          iOwner.ChangeState(Cthulhu.States.Emerge);
        }
        this.mTimer -= iDeltaTime;
      }
      else if (!this.mLogicDone && (double) num1 >= 0.69767439365386963)
      {
        Magicka.GameLogic.Entities.Entity entity = (Magicka.GameLogic.Entities.Entity) iOwner.Entity;
        Vector3 translation = iOwner.mTransform.Translation;
        Magicka.GameLogic.Damage iDamage1 = new Magicka.GameLogic.Damage(AttackProperties.Pushed, Elements.Earth, 100f, 4f);
        int num2 = (int) Helper.CircleDamage(iOwner.mPlayState, entity, iOwner.mPlayState.PlayTime, entity, ref translation, 6f, ref iDamage1);
        Magicka.GameLogic.Damage iDamage2 = new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Water, 100f, 1f);
        int num3 = (int) Helper.CircleDamage(iOwner.mPlayState, entity, iOwner.mPlayState.PlayTime, entity, ref translation, 6f, ref iDamage2);
        this.mLogicDone = true;
      }
      else
      {
        if (this.mEffectDone || (double) num1 < 0.58139532804489136)
          return;
        if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
        {
          Matrix translation = Matrix.CreateTranslation(iOwner.mTransform.Translation with
          {
            Y = 0.0f
          });
          EffectManager.Instance.StartEffect(this.WaterSplashEffect, ref translation, out this.mEffectRef);
        }
        iOwner.KillIdleEffect();
        this.mEffectDone = true;
      }
    }

    public void OnExit(Cthulhu iOwner) => iOwner.ClearAllStatusEffects();

    public bool Active(Cthulhu iOwner, float iDeltaTime)
    {
      return (double) iOwner.mHPOnLastEmerge - (double) iOwner.HitPoints > 0.10000000149011612 * (double) iOwner.MaxHitPoints || (double) iOwner.mTimeUntilSubmerge < 0.0 || (double) iOwner.mTimeSinceLastEmerge > 5.0 && iOwner.mSecondsWhileOutOfRange > 1;
    }

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
    }
  }

  private class TimewarpState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private const float TIME = 0.3939394f;
    private float mCooldown;
    private bool mCasted;
    private bool mSoundPlayed;

    public void OnEnter(Cthulhu iOwner)
    {
      AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_HOWL, iOwner.mDamageZone.AudioEmitter);
      iOwner.CrossFade(Cthulhu.Animations.Timewarp, 0.4f, false);
      this.mCasted = false;
      this.mSoundPlayed = false;
    }

    public unsafe void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.ChangeState(Cthulhu.States.Idle);
      }
      else
      {
        if (this.mCasted || (double) num < 0.39393940567970276)
          return;
        if (!this.mSoundPlayed)
        {
          AudioManager.Instance.PlayCue(Banks.Spells, Cthulhu.SOUND_TIMEWARP, iOwner.mDamageZone.AudioEmitter);
          this.mSoundPlayed = true;
        }
        TimeWarp.Instance.Execute((ISpellCaster) iOwner.mDamageZone, iOwner.mPlayState);
        this.mCasted = true;
        if (NetworkManager.Instance.State != NetworkState.Server)
          return;
        Cthulhu.TimewarpMessage timewarpMessage = new Cthulhu.TimewarpMessage();
        BossFight.Instance.SendMessage<Cthulhu.TimewarpMessage>((IBoss) iOwner, (ushort) 6, (void*) &timewarpMessage, true);
      }
    }

    public void OnExit(Cthulhu iOwner) => this.mCooldown = 20f;

    public bool Active(Cthulhu iOwner, float iDeltaTime)
    {
      return iOwner.mCurrentState != Cthulhu.States.Timewarp && (double) this.mCooldown <= 0.0 && !SpellManager.Instance.IsEffectActive(typeof (TimeWarp)) && (double) iOwner.mTimeSinceLastDamage > 2.0;
    }

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
      if (!SpellManager.Instance.IsEffectActive(typeof (TimeWarp)) || (double) this.mCooldown < 0.0)
        return;
      this.mCooldown -= iDeltaTime;
    }
  }

  private class LightningState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private const float START_TIME = 0.267441869f;
    private const float END_TIME = 0.8372093f;
    private HitList mHitList;
    private DamageCollection5 mDamageCollection;
    private float mTimer;
    private SortedList<float, int> mTimers;
    private Cue mSoundCue;
    private float mCooldown;

    public LightningState()
    {
      this.mHitList = new HitList(16 /*0x10*/);
      this.mTimers = new SortedList<float, int>();
      this.mDamageCollection = new DamageCollection5();
      this.mDamageCollection.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Lightning, 8f, 1f));
    }

    public void OnEnter(Cthulhu iOwner)
    {
      this.mTimers.Clear();
      iOwner.CrossFade(Cthulhu.Animations.Cast_Lightning, 0.4f, false);
      this.mHitList.Clear();
      for (int index = 0; index < 12; ++index)
      {
        float key;
        do
        {
          key = (float) (0.0 + (double) index / 1000.0);
        }
        while (this.mTimers.ContainsKey(key));
        this.mTimers.Add(key, index);
      }
      this.mTimer = 0.0f;
      this.mSoundCue = (Cue) null;
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      this.mHitList.Update(iDeltaTime);
      float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
        iOwner.ChangeState(Cthulhu.States.Idle);
      else if ((double) num1 >= 0.26744186878204346 && (double) num1 <= 0.83720928430557251)
      {
        if (this.mSoundCue == null)
          this.mSoundCue = AudioManager.Instance.PlayCue(Banks.Spells, Cthulhu.SOUND_LIGHTNING, iOwner.mDamageZone.AudioEmitter);
        this.mTimer -= iDeltaTime;
        while (this.mTimers.Count > 0 && (double) this.mTimers.First<KeyValuePair<float, int>>().Key + (double) this.mTimer < 0.0)
        {
          float radius = 6f;
          int index = this.mTimers.First<KeyValuePair<float, int>>().Value;
          this.mTimers.RemoveAt(0);
          LightningBolt lightning = LightningBolt.GetLightning();
          Vector3 vector3 = index < 6 ? iOwner.mLeftFingerOrientation[index].Translation + iOwner.mTransform.Forward : iOwner.mRightFingerOrientation[index - 6].Translation + iOwner.mTransform.Forward;
          float num2 = (float) (Cthulhu.RANDOM.NextDouble() / 4.0 + 0.15000000596046448);
          this.mHitList.Add((ISpellCaster) iOwner.mDamageZone);
          foreach (Magicka.GameLogic.Entities.Entity mActiveTentacle in iOwner.mActiveTentacles)
            this.mHitList.Add(mActiveTentacle);
          Magicka.GameLogic.Entities.Character target;
          if (!this.FindTarget(iOwner, radius, 0.0f, vector3, out target))
          {
            Vector3 forward = iOwner.mTransform.Forward;
            float num3 = 0.7f;
            Vector3 iDirection = forward + iOwner.mTransform.Right * num3 + iOwner.mTransform.Left * 2f * (float) Cthulhu.RANDOM.NextDouble() * num3;
            iDirection.Normalize();
            lightning.Cast((ISpellCaster) iOwner.mDamageZone, vector3, iDirection, this.mHitList, new Vector3(0.2f, 0.2f, 0.2f), 1f, radius + (float) (Cthulhu.RANDOM.NextDouble() * 6.0), ref this.mDamageCollection, new Spell?(), iOwner.mPlayState);
          }
          else
            lightning.Cast((ISpellCaster) iOwner.mDamageZone, vector3, (Magicka.GameLogic.Entities.Entity) target, this.mHitList, new Vector3(0.2f, 0.2f, 0.2f), 1f, radius + (float) (Cthulhu.RANDOM.NextDouble() * 6.0), ref this.mDamageCollection, iOwner.mPlayState);
          lightning.TTL = num2;
          float key = num2 - this.mTimer;
          while (this.mTimers.ContainsKey(key))
            key += 1E-06f;
          this.mTimers.Add(key, index);
        }
      }
      else
      {
        if ((double) num1 < 0.83720928430557251)
          return;
        if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
          this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
        this.mSoundCue = (Cue) null;
      }
    }

    private bool FindTarget(
      Cthulhu iOwner,
      float radius,
      float tooCloseDistance,
      Vector3 pos,
      out Magicka.GameLogic.Entities.Character target)
    {
      EntityManager entityManager = iOwner.mPlayState.EntityManager;
      List<Magicka.GameLogic.Entities.Entity> entities = entityManager.GetEntities(pos, radius, true);
      bool target1 = false;
      target = (Magicka.GameLogic.Entities.Character) null;
      for (int index = 0; index < entities.Count && !target1; ++index)
      {
        if (entities[index] is Avatar avatar && !avatar.IsEthereal && !this.mHitList.Contains((IDamageable) avatar))
        {
          target = (Magicka.GameLogic.Entities.Character) avatar;
          Vector3 vector1 = (avatar.Position - pos) with
          {
            Y = 0.0f
          };
          if ((double) vector1.LengthSquared() > (double) tooCloseDistance * (double) tooCloseDistance)
          {
            vector1.Normalize();
            Vector3 forward = iOwner.mTransform.Forward;
            target1 = (double) Vector3.Dot(vector1, forward) > 0.699999988079071;
          }
        }
      }
      entityManager.ReturnEntityList(entities);
      return target1;
    }

    private bool ShouldDoLightning(
      Cthulhu iOwner,
      float radius,
      float tooCloseDistance,
      Vector3 pos,
      out Magicka.GameLogic.Entities.Character target)
    {
      EntityManager entityManager = iOwner.mPlayState.EntityManager;
      List<Magicka.GameLogic.Entities.Entity> entities = entityManager.GetEntities(pos, radius, true);
      target = (Magicka.GameLogic.Entities.Character) null;
      entities.Remove((Magicka.GameLogic.Entities.Entity) iOwner.mDamageZone);
      foreach (Tentacle mActiveTentacle in iOwner.mActiveTentacles)
        entities.Remove((Magicka.GameLogic.Entities.Entity) mActiveTentacle);
      float num1 = 0.0f;
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Magicka.GameLogic.Entities.Character character && !character.IsEthereal && !this.mHitList.Contains((IDamageable) character))
        {
          Vector3 vector1 = (character.Position - pos) with
          {
            Y = 0.0f
          };
          float num2 = vector1.LengthSquared();
          bool flag = false;
          if ((double) num2 > (double) tooCloseDistance * (double) tooCloseDistance)
          {
            vector1.Normalize();
            Vector3 forward = iOwner.mTransform.Forward;
            flag = (double) Vector3.Dot(vector1, forward) > 0.699999988079071;
          }
          if (flag)
          {
            if ((character.Faction & Factions.FRIENDLY) != Factions.NONE)
              num1 += character.IsResistantAgainst(Elements.Lightning);
            else
              num1 -= character.IsResistantAgainst(Elements.Lightning);
          }
        }
      }
      entityManager.ReturnEntityList(entities);
      return (double) num1 > 0.5;
    }

    public void OnExit(Cthulhu iOwner)
    {
      if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
        this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
      this.mSoundCue = (Cue) null;
      this.mCooldown = 10f;
    }

    public bool Active(Cthulhu iOwner, float iDeltaTime)
    {
      return (double) this.mCooldown < 0.0 && this.ShouldDoLightning(iOwner, 19f, 5f, iOwner.mTransform.Translation, out Magicka.GameLogic.Entities.Character _);
    }

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
      if ((double) this.mCooldown < 0.0)
        return;
      this.mCooldown -= iDeltaTime;
    }
  }

  private class CallState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private const float LOGIC_EXECUTE_TIME = 0.4864865f;
    private const float PFX_EXECUTE_TIME = 0.0f;
    private const int MAX_NR_SPAWNS = 4;
    private static readonly int ANY = "any".GetHashCodeCustom();
    private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();
    private static readonly int DEEP_ONE_HASH = "deep_one".GetHashCodeCustom();
    private static readonly int CULTIST_HASH = "cultist".GetHashCodeCustom();
    private int[] mRandomClosedList;
    private bool mCasted;
    private int mNrSpawns;

    public CallState()
    {
      this.mRandomClosedList = new int[4];
      this.mNrSpawns = 1;
    }

    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.CrossFade(Cthulhu.Animations.Call_of_Cthulhu, 0.4f, false);
      this.mCasted = false;
      AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_CALL_OF_CTHULHU, iOwner.mDamageZone.AudioEmitter);
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.ChangeState(Cthulhu.States.Idle);
      }
      else
      {
        if (this.mCasted || (double) num < 0.48648649454116821)
          return;
        for (int index = 0; index < 4; ++index)
          this.mRandomClosedList[index] = -1;
        for (int index1 = 0; index1 < this.mNrSpawns; ++index1)
        {
          bool flag1 = false;
          int index2 = -1;
          bool flag2;
          for (; !flag1; flag1 = !flag2)
          {
            index2 = Cthulhu.RANDOM.Next(Cthulhu.DEEP_ONES_SPAWN_LOCATORS.Length);
            flag2 = false;
            for (int index3 = 0; index3 < 4 && !flag2; ++index3)
              flag2 = this.mRandomClosedList[index3] == index2;
          }
          this.mRandomClosedList[index1] = index2;
          Matrix onesSpawnTransform = iOwner.mDeepOnesSpawnTransforms[index2];
          this.mRandomClosedList[index1] = index2;
          Vector3 iPoint = onesSpawnTransform.Translation;
          Vector3 oPoint;
          double nearestPosition = (double) iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPoint, out oPoint, MovementProperties.Default);
          iPoint = oPoint;
          CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(Cthulhu.CallState.STARSPAWN_HASH);
          NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iOwner.mPlayState);
          instance.Initialize(cachedTemplate, iPoint, 0);
          instance.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, (AIEvent[]) null);
          instance.CharacterBody.Orientation = Matrix.Identity;
          instance.CharacterBody.DesiredDirection = iOwner.mTransform.Forward;
          instance.SpawnAnimation = Magicka.Animations.spawn;
          instance.ChangeState((BaseState) RessurectionState.Instance);
          iOwner.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) instance);
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
            {
              ActionType = TriggerActionType.SpawnNPC,
              Handle = instance.Handle,
              Template = instance.Type,
              Id = instance.UniqueID,
              Position = instance.Position,
              Direction = instance.CharacterBody.Direction,
              Bool0 = false,
              Point2 = (int) instance.SpawnAnimation
            });
        }
        this.mCasted = true;
      }
    }

    public void OnExit(Cthulhu iOwner)
    {
    }

    public bool Active(Cthulhu iOwner, float iDeltaTime)
    {
      TriggerArea triggerArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Cthulhu.CallState.ANY);
      return triggerArea.GetCount(Cthulhu.CallState.STARSPAWN_HASH) + triggerArea.GetCount(Cthulhu.CallState.DEEP_ONE_HASH) + triggerArea.GetCount(Cthulhu.CallState.CULTIST_HASH) == 0;
    }

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
    }
  }

  private class LesserCallState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private const float LOGIC_EXECUTE_TIME = 0.4864865f;
    private const float PFX_EXECUTE_TIME = 0.0f;
    private static readonly int ANY = "any".GetHashCodeCustom();
    private static readonly int DEEP_ONE_HASH = "deep_one".GetHashCodeCustom();
    private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();
    private float mTimer;
    private SortedList<float, int> mTimers;
    private int[] mRandomClosedList;
    private int mNrSpawns;
    private float mCooldown;

    public LesserCallState()
    {
      switch (Magicka.Game.Instance.PlayerCount)
      {
        case 1:
          this.mNrSpawns = 2;
          break;
        case 2:
          this.mNrSpawns = 4;
          break;
        case 3:
          this.mNrSpawns = 5;
          break;
        case 4:
          this.mNrSpawns = 6;
          break;
        default:
          this.mNrSpawns = 1;
          break;
      }
      this.mRandomClosedList = new int[this.mNrSpawns];
      this.mTimers = new SortedList<float, int>(this.mNrSpawns);
    }

    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.CrossFade(Cthulhu.Animations.Call_of_Cthulhu, 0.4f, false);
      AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_LESSER_CALL_OF_CTHULHU, iOwner.mDamageZone.AudioEmitter);
      for (int index = 0; index < this.mNrSpawns; ++index)
        this.mRandomClosedList[index] = -1;
      for (int index1 = 0; index1 < this.mNrSpawns; ++index1)
      {
        bool flag1 = false;
        int num = -1;
        bool flag2;
        for (; !flag1; flag1 = !flag2)
        {
          num = Cthulhu.RANDOM.Next(Cthulhu.DEEP_ONES_SPAWN_LOCATORS.Length);
          flag2 = false;
          for (int index2 = 0; index2 < this.mNrSpawns && !flag2; ++index2)
            flag2 = this.mRandomClosedList[index2] == num;
        }
        this.mRandomClosedList[index1] = num;
      }
      this.mTimers.Clear();
      for (int index = 0; index < this.mNrSpawns; ++index)
      {
        float key;
        do
        {
          key = (float) Cthulhu.RANDOM.NextDouble() + 0.15f;
        }
        while (this.mTimers.ContainsKey(key));
        this.mTimers.Add(key, index);
      }
      this.mTimer = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.ChangeState(Cthulhu.States.Idle);
      }
      else
      {
        if ((double) num < 0.48648649454116821)
          return;
        this.mTimer -= iDeltaTime;
        while (this.mTimers.Count > 0 && (double) this.mTimers.First<KeyValuePair<float, int>>().Key + (double) this.mTimer < 0.0)
        {
          int index = this.mTimers.First<KeyValuePair<float, int>>().Value;
          this.mTimers.RemoveAt(0);
          int mRandomClosed = this.mRandomClosedList[index];
          Matrix onesSpawnTransform = iOwner.mDeepOnesSpawnTransforms[mRandomClosed];
          Vector3 translation = onesSpawnTransform.Translation;
          Vector3 oPoint;
          double nearestPosition = (double) iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref translation, out oPoint, MovementProperties.Default);
          Vector3 iPosition = oPoint;
          CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(Cthulhu.LesserCallState.DEEP_ONE_HASH);
          NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iOwner.mPlayState);
          instance.Initialize(cachedTemplate, iPosition, 0);
          instance.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, (AIEvent[]) null);
          Matrix matrix = onesSpawnTransform with
          {
            Translation = Vector3.Zero
          };
          instance.CharacterBody.Orientation = matrix;
          instance.CharacterBody.DesiredDirection = iOwner.mTransform.Forward;
          instance.SpawnAnimation = Magicka.Animations.special0;
          instance.ChangeState((BaseState) RessurectionState.Instance);
          iOwner.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) instance);
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
            {
              ActionType = TriggerActionType.SpawnNPC,
              Handle = instance.Handle,
              Template = instance.Type,
              Id = instance.UniqueID,
              Position = instance.Position,
              Direction = instance.CharacterBody.Direction,
              Bool0 = false,
              Point2 = (int) instance.SpawnAnimation
            });
        }
      }
    }

    public void OnExit(Cthulhu iOwner) => this.mCooldown = 25f;

    public bool Active(Cthulhu iOwner, float iDeltaTime)
    {
      if ((double) this.mCooldown >= 0.0)
        return false;
      TriggerArea triggerArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Cthulhu.LesserCallState.ANY);
      return triggerArea.GetCount(Cthulhu.LesserCallState.DEEP_ONE_HASH) + triggerArea.GetCount(Cthulhu.LesserCallState.STARSPAWN_HASH) == 0 && Cthulhu.RANDOM.NextDouble() < 0.800000011920929;
    }

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
      if ((double) this.mCooldown < 0.0)
        return;
      this.mCooldown -= iDeltaTime;
    }
  }

  private class DevourState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private const float START_TIME = 0.08f;
    private const float END_TIME = 0.9f;
    private const float PFX_START_TIME = 0.08f;
    private VisualEffectReference mEffectRef;
    private int ParticleEffect = "cthulhu_pull".GetHashCodeCustom();
    private static readonly int STARSPAWN_HASH = "starspawn".GetHashCodeCustom();
    private float mVortexTimer;
    private bool mSoundPlayed;
    private bool mEffectStarted;
    private RadialBlur blur;
    private Cue mSoundCueSuck;

    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.CrossFade(Cthulhu.Animations.Devour, 0.4f, false);
      this.mVortexTimer = 0.0f;
      this.mSoundPlayed = false;
      this.mEffectStarted = false;
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
        iOwner.ChangeState(Cthulhu.States.Idle);
      else if ((double) num >= 0.079999998211860657 && (double) num <= 0.89999997615814209)
      {
        if (!this.mSoundPlayed)
        {
          AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_DEVOUR, iOwner.mDamageZone.AudioEmitter);
          this.mSoundCueSuck = AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_DEVOUR_SUCK, iOwner.mDamageZone.AudioEmitter);
          this.mSoundPlayed = true;
        }
        if (this.CustomVortex(iOwner, iDeltaTime))
          return;
      }
      if ((double) num >= 0.079999998211860657 && !this.mEffectStarted)
      {
        if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
        {
          Matrix attachOrientation = iOwner.mMouthAttachOrientation;
          EffectManager.Instance.StartEffect(this.ParticleEffect, ref attachOrientation, out this.mEffectRef);
        }
        this.mEffectStarted = true;
      }
      if ((double) num > 0.89999997615814209)
      {
        if (!this.mEffectStarted || !EffectManager.Instance.IsActive(ref this.mEffectRef))
          return;
        EffectManager.Instance.Stop(ref this.mEffectRef);
      }
      else
      {
        if (!this.mEffectStarted || !EffectManager.Instance.IsActive(ref this.mEffectRef))
          return;
        Matrix attachOrientation = iOwner.mMouthAttachOrientation;
        EffectManager.Instance.UpdateOrientation(ref this.mEffectRef, ref attachOrientation);
      }
    }

    private bool CustomVortex(Cthulhu iOwner, float iDeltaTime)
    {
      bool flag1 = false;
      Vector3 result1 = iOwner.mMouthAttachOrientation.Translation;
      Vector3 result2 = iOwner.mTransform.Forward;
      Vector3.Multiply(ref result2, -0.8f, out result2);
      Vector3.Add(ref result1, ref result2, out result1);
      EntityManager entityManager = iOwner.mPlayState.EntityManager;
      List<Magicka.GameLogic.Entities.Entity> entities = entityManager.GetEntities(result1, 30f, true);
      entities.Remove((Magicka.GameLogic.Entities.Entity) iOwner.mDamageZone);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Magicka.GameLogic.Entities.Character iCharacter)
        {
          if (!iCharacter.IsEthereal)
          {
            if (iCharacter.IsGripped && iCharacter.Gripper != null)
              iCharacter.Gripper.ReleaseAttachedCharacter();
            if (iCharacter.Type == Cthulhu.DevourState.STARSPAWN_HASH)
              continue;
          }
          else
            continue;
        }
        else
        {
          if (entities[index] is Shield)
          {
            Vector3 nearestPosition = (entities[index] as Shield).GetNearestPosition(result1);
            Vector3 result3;
            Vector3.Subtract(ref result1, ref nearestPosition, out result3);
            result3.Y = 0.0f;
            if ((double) result3.LengthSquared() > 9.9999999747524271E-07 && (double) result3.Length() < 1.7999999523162842)
            {
              if (this.blur != null)
                this.blur.Kill();
              iOwner.ChangeState(Cthulhu.States.Idle);
              flag1 = true;
              break;
            }
            continue;
          }
          if (!(entities[index] is MissileEntity))
            continue;
        }
        Vector3 position = entities[index].Position;
        Vector3 result4;
        Vector3.Subtract(ref result1, ref position, out result4);
        result4.Y = 0.0f;
        if ((double) result4.LengthSquared() > 9.9999999747524271E-07)
        {
          float num = result4.Length();
          bool flag2;
          if (entities[index] is MissileEntity && (double) num < 1.25)
          {
            flag2 = true;
            entities[index].Kill();
          }
          else
            flag2 = (double) num < 0.40000000596046448;
          if (flag2)
          {
            this.SetCharacterToEat(iOwner, iCharacter);
            if (this.blur != null)
              this.blur.Kill();
            iOwner.ChangeState(Cthulhu.States.DevourHit);
            flag1 = true;
            break;
          }
          float scaleFactor = (float) (7.5 * (1.0 - (double) num / 30.0));
          if ((double) scaleFactor > 0.0)
          {
            result4.Normalize();
            if ((double) Vector3.Dot(result4, iOwner.mMouthAttachOrientation.Forward) >= 0.7)
            {
              Vector3.Multiply(ref result4, scaleFactor, out result4);
              if (entities[index] is MissileEntity)
              {
                result4 *= 100f;
                entities[index].Body.Velocity += result4 * entities[index].Body.InverseMass * iDeltaTime;
              }
              else if (iCharacter != null)
                iCharacter.CharacterBody.AdditionalForce = result4;
            }
          }
        }
      }
      entityManager.ReturnEntityList(entities);
      result1 -= 1.5f * iOwner.mTransform.Forward;
      this.mVortexTimer -= iDeltaTime;
      if ((double) this.mVortexTimer <= 0.0)
      {
        RadialBlur.GetRadialBlur();
        result1.Y = 0.0f;
        Vector3 forward = iOwner.mTransform.Forward;
        float iTTL = iOwner.mAnimationController.AnimationClip.Duration * 0.82f;
        this.blur = RadialBlur.GetRadialBlur();
        this.blur.Initialize(ref result1, ref forward, 0.5235988f, 15f, iTTL, iOwner.mPlayState.Scene);
        this.mVortexTimer = iTTL;
      }
      return flag1;
    }

    private unsafe void SetCharacterToEat(Cthulhu iOwner, Magicka.GameLogic.Entities.Character iCharacter)
    {
      if (iCharacter == null || NetworkManager.Instance.State == NetworkState.Client)
        return;
      iOwner.mCharacterToEat = iCharacter;
      if (NetworkManager.Instance.State != NetworkState.Server)
        return;
      BossFight.Instance.SendMessage<Cthulhu.SetCharacterToEatMessage>((IBoss) iOwner, (ushort) 8, (void*) &new Cthulhu.SetCharacterToEatMessage()
      {
        Handle = iCharacter.Handle
      }, true);
    }

    private bool FindTarget(
      Cthulhu iOwner,
      float radius,
      float tooCloseDistance,
      Vector3 pos,
      out Magicka.GameLogic.Entities.Character target)
    {
      EntityManager entityManager = iOwner.mPlayState.EntityManager;
      List<Magicka.GameLogic.Entities.Entity> entities = entityManager.GetEntities(pos, radius, true);
      bool target1 = false;
      target = (Magicka.GameLogic.Entities.Character) null;
      for (int index = 0; index < entities.Count && !target1; ++index)
      {
        if (entities[index] is Avatar avatar && !avatar.IsEthereal)
        {
          target = (Magicka.GameLogic.Entities.Character) avatar;
          Vector3 vector1 = (avatar.Position - pos) with
          {
            Y = 0.0f
          };
          if ((double) vector1.LengthSquared() > (double) tooCloseDistance * (double) tooCloseDistance)
          {
            vector1.Normalize();
            Vector3 forward = iOwner.mTransform.Forward;
            target1 = (double) Vector3.Dot(vector1, forward) > 0.699999988079071;
          }
        }
      }
      entityManager.ReturnEntityList(entities);
      return target1;
    }

    public void OnExit(Cthulhu iOwner) => this.StopAllEffectAndSounds();

    private void StopAllEffectAndSounds()
    {
      if (EffectManager.Instance.IsActive(ref this.mEffectRef))
        EffectManager.Instance.Stop(ref this.mEffectRef);
      if (this.blur != null)
        this.blur.Kill();
      if (this.mSoundCueSuck == null)
        return;
      this.mSoundCueSuck.Stop(AudioStopOptions.AsAuthored);
    }

    public bool Active(Cthulhu iOwner, float iDeltaTime)
    {
      return this.FindTarget(iOwner, 23f, 2f, iOwner.mTransform.Translation, out Magicka.GameLogic.Entities.Character _);
    }

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
    }
  }

  private class DevourHitState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private const float PFX_START_TIME = 0.1f;
    private const float SOUND_START_TIME = 0.08f;
    private VisualEffectReference mEffectRef;
    private int ParticleEffect = "cthulhu_is_eating".GetHashCodeCustom();
    private bool mSoundPlayed;
    private bool mEffectStarted;
    private bool mCharacterTerminated;
    private float mCharacterScale;
    private Vector3 mCharacterOffset;
    private Vector3 mCharacterOffsetUp;

    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.CrossFade(Cthulhu.Animations.DevourHit, 0.2f, false);
      this.mSoundPlayed = false;
      this.mEffectStarted = false;
      this.mCharacterTerminated = iOwner.mCharacterToEat == null;
      this.mCharacterScale = 1f;
      if (iOwner.mCharacterToEat == null)
        return;
      Vector3 up = iOwner.mCharacterToEat.Capsule.Orientation.Up;
      Vector3.Multiply(ref up, iOwner.mCharacterToEat.Capsule.Length, out this.mCharacterOffsetUp);
      Vector3 position = iOwner.mCharacterToEat.Position;
      Vector3 translation = iOwner.mMouthAttachOrientation.Translation;
      Vector3.Subtract(ref position, ref translation, out this.mCharacterOffset);
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (!this.mCharacterTerminated)
      {
        this.mCharacterScale -= iDeltaTime;
        Magicka.GameLogic.Entities.Character mCharacterToEat = iOwner.mCharacterToEat;
        Vector3 result1;
        Vector3.Multiply(ref this.mCharacterOffset, (float) (((double) this.mCharacterScale - 0.66666668653488159) * 3.0), out result1);
        Vector3 result2;
        Vector3.Multiply(ref this.mCharacterOffsetUp, (float) ((1.0 - (double) this.mCharacterScale) * 2.0), out result2);
        Vector3 result3 = iOwner.mMouthAttachOrientation.Translation;
        Vector3.Add(ref result3, ref result1, out result3);
        Vector3.Add(ref result3, ref result2, out result3);
        Matrix orientation = mCharacterToEat.Body.Orientation;
        mCharacterToEat.Body.MoveTo(ref result3, ref orientation);
        mCharacterToEat.ScaleGraphicModel(this.mCharacterScale);
        if ((double) this.mCharacterScale <= 0.10000000149011612)
        {
          this.mCharacterTerminated = true;
          mCharacterToEat.Terminate(true, false);
          mCharacterToEat.ScaleGraphicModel(1f);
        }
      }
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
        iOwner.ChangeState(Cthulhu.States.Idle);
      else if ((double) num >= 0.10000000149011612 && !this.mEffectStarted)
      {
        this.mEffectStarted = true;
        Matrix attachOrientation = iOwner.mMouthAttachOrientation;
        EffectManager.Instance.StartEffect(this.ParticleEffect, ref attachOrientation, out this.mEffectRef);
      }
      else if ((double) num >= 0.079999998211860657 && !this.mSoundPlayed)
      {
        AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_DEVOUR_HIT, iOwner.mDamageZone.AudioEmitter);
        this.mSoundPlayed = true;
      }
      if (!this.mEffectStarted || !EffectManager.Instance.IsActive(ref this.mEffectRef))
        return;
      Matrix attachOrientation1 = iOwner.mMouthAttachOrientation;
      EffectManager.Instance.UpdateOrientation(ref this.mEffectRef, ref attachOrientation1);
    }

    public void OnExit(Cthulhu iOwner)
    {
      if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
        return;
      EffectManager.Instance.Stop(ref this.mEffectRef);
    }

    public bool Active(Cthulhu iOwner, float iDeltaTime) => false;

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
    }
  }

  private class HypnotizeState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private float LOGIC_EXECUTE_TIME = 0.6041667f;
    private float PFX_EXECUTE_TIME;
    private bool mCasted;
    private bool mEffectDone;
    private float mCoolDown;
    private VisualEffectReference mEffectRef;
    private int ParticleEffect = "cthulhu_hypnotize".GetHashCodeCustom();

    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.CrossFade(Cthulhu.Animations.Mesmerize, 0.4f, false);
      this.mCasted = false;
      this.mEffectDone = false;
      AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_HYPNOTIZE, iOwner.mDamageZone.AudioEmitter);
    }

    public unsafe void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.ChangeState(Cthulhu.States.Idle);
      }
      else
      {
        float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if ((double) num >= (double) this.LOGIC_EXECUTE_TIME && !this.mCasted)
        {
          Vector3 translation1 = iOwner.mTransform.Translation;
          Avatar oAvatar;
          if (iOwner.GetRandomTarget(out oAvatar, ref translation1, false))
          {
            Vector3 position = oAvatar.Position;
            Vector3 translation2 = iOwner.mMistSpawnTransform.Translation;
            Vector3 result;
            Vector3.Subtract(ref position, ref translation2, out result);
            result.Y = 0.0f;
            if ((double) result.LengthSquared() < 9.9999999747524271E-07)
            {
              translation2 = iOwner.mTransform.Translation;
              Vector3.Subtract(ref translation2, ref position, out result);
              result.Y = 0.0f;
            }
            if ((double) result.LengthSquared() < 9.9999999747524271E-07)
              return;
            result.Normalize();
            oAvatar.Hypnotize(ref result, iOwner.mPlayerHypnotizeEffect);
            if (NetworkManager.Instance.State == NetworkState.Server)
              BossFight.Instance.SendMessage<Cthulhu.HypnotizeMessage>((IBoss) iOwner, (ushort) 5, (void*) &new Cthulhu.HypnotizeMessage()
              {
                Direction = new Normalized101010(result),
                Handle = oAvatar.Handle
              }, true);
          }
          this.mCasted = true;
        }
        else if (!this.mEffectDone && (double) num >= (double) this.PFX_EXECUTE_TIME)
        {
          if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
          {
            Matrix attachOrientation = iOwner.mMouthAttachOrientation;
            EffectManager.Instance.StartEffect(this.ParticleEffect, ref attachOrientation, out this.mEffectRef);
          }
          this.mEffectDone = true;
        }
        if (!this.mEffectDone || !EffectManager.Instance.IsActive(ref this.mEffectRef))
          return;
        Matrix attachOrientation1 = iOwner.mMouthAttachOrientation;
        EffectManager.Instance.UpdateOrientation(ref this.mEffectRef, ref attachOrientation1);
      }
    }

    public void OnExit(Cthulhu iOwner)
    {
      if (EffectManager.Instance.IsActive(ref this.mEffectRef))
        EffectManager.Instance.Stop(ref this.mEffectRef);
      this.mCoolDown = 5f;
    }

    public bool Active(Cthulhu iOwner, float iDeltaTime)
    {
      if (Magicka.Game.Instance.PlayerCount <= 1 || (double) this.mCoolDown >= 0.0)
        return false;
      int num = 0;
      for (int index = 0; index < iOwner.mPlayers.Length; ++index)
      {
        if (iOwner.mPlayers[index].Playing && iOwner.mPlayers[index].Avatar != null)
        {
          if (iOwner.mPlayers[index].Avatar.IsHypnotized)
            return false;
          if (!iOwner.mPlayers[index].Avatar.Dead)
            ++num;
        }
      }
      return num >= 2;
    }

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
      if ((double) this.mCoolDown < 0.0)
        return;
      this.mCoolDown -= iDeltaTime;
    }
  }

  private class RageState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    public Action<Cthulhu> Callback;
    private float RUMBLE_TIME = 0.25f;
    private float CAST_TIME = 0.5f;
    private bool mCalled;
    private bool mRumble;

    public void OnEnter(Cthulhu iOwner)
    {
      iOwner.CrossFade(Cthulhu.Animations.Rage, 0.5f, false);
      this.mCalled = false;
      this.mRumble = false;
      AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_RAGE, iOwner.mDamageZone.AudioEmitter);
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.HasFinished)
        iOwner.ChangeState(Cthulhu.States.Idle);
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if ((double) num >= (double) this.CAST_TIME && !this.mCalled)
      {
        this.mCalled = true;
        if (this.Callback != null)
          this.Callback(iOwner);
      }
      if ((double) num < (double) this.RUMBLE_TIME || this.mRumble)
        return;
      this.mRumble = true;
      iOwner.mPlayState.Camera.CameraShake(iOwner.mTransform.Translation, 0.5f, 2.5f);
    }

    public void OnExit(Cthulhu iOwner)
    {
    }

    public bool Active(Cthulhu iOwner, float iDeltaTime) => false;

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
    }
  }

  private class OtherworldlyBoltState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private int mNrCasted;
    private int mNrToCast;
    private Cthulhu.OtherworldlyBoltState.BoltState mState;
    private float mCoolDown;
    private bool mCasted;
    private OtherworldlyBolt mCurrentBolt;
    private static readonly int ANY = "any".GetHashCodeCustom();
    private static readonly int CULTIST_HASH = "cultist".GetHashCodeCustom();
    private static readonly int DEEP_ONE_HASH = "deep_one".GetHashCodeCustom();

    public void OnEnter(Cthulhu iOwner)
    {
      this.mNrToCast = Magicka.Game.Instance.PlayerCount;
      AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_OTHERWORLDLY_BOLT, iOwner.mDamageZone.AudioEmitter);
      this.mNrCasted = 0;
      this.mCasted = false;
      iOwner.CrossFade(Cthulhu.Animations.OtherwordlyBoltStart, 0.2f, false);
      this.mState = Cthulhu.OtherworldlyBoltState.BoltState.Start;
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      switch (this.mState)
      {
        case Cthulhu.OtherworldlyBoltState.BoltState.Start:
          if (iOwner.mAnimationController.CrossFadeEnabled || !iOwner.mAnimationController.HasFinished)
            break;
          this.mState = Cthulhu.OtherworldlyBoltState.BoltState.Mid;
          iOwner.CrossFade(Cthulhu.Animations.OtherwordlyBoltMid, 0.0f, false);
          break;
        case Cthulhu.OtherworldlyBoltState.BoltState.Mid:
          if (iOwner.mAnimationController.CrossFadeEnabled)
            break;
          if (iOwner.mAnimationController.HasFinished)
          {
            if (this.mNrCasted >= this.mNrToCast)
            {
              this.mState = Cthulhu.OtherworldlyBoltState.BoltState.End;
              iOwner.CrossFade(Cthulhu.Animations.OtherwordlyBoltEnd, 0.0f, false);
              break;
            }
            this.mState = Cthulhu.OtherworldlyBoltState.BoltState.Mid;
            this.mCasted = false;
            iOwner.CrossFade(Cthulhu.Animations.OtherwordlyBoltMid, 0.0f, false);
            break;
          }
          if (this.mCasted)
            break;
          float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
          if ((double) num >= 0.0 && this.mCurrentBolt == null)
          {
            Vector3 translation1 = iOwner.mLeftHandOrientation.Translation;
            Vector3 translation2 = iOwner.mRightHandOrientation.Translation;
            Vector3 result1;
            Vector3.Subtract(ref translation2, ref translation1, out result1);
            Vector3.Multiply(ref result1, 0.5f, out result1);
            Vector3 result2;
            Vector3.Add(ref translation1, ref result1, out result2);
            this.mCurrentBolt = iOwner.SpawnCultistMissile(ref result2, 4f * iOwner.mStageSpeedModifier);
            break;
          }
          if ((double) num < 0.27000001072883606)
            break;
          ++this.mNrCasted;
          this.mCurrentBolt.GoHunt();
          this.mCurrentBolt = (OtherworldlyBolt) null;
          this.mCasted = true;
          break;
        case Cthulhu.OtherworldlyBoltState.BoltState.End:
          if (iOwner.mAnimationController.CrossFadeEnabled || !iOwner.mAnimationController.HasFinished)
            break;
          iOwner.ChangeState(Cthulhu.States.Idle);
          break;
      }
    }

    private void FindTarget(Cthulhu iOwner, Vector3 pos, out Avatar target)
    {
      EntityManager entityManager = iOwner.mPlayState.EntityManager;
      List<Magicka.GameLogic.Entities.Entity> entities = entityManager.GetEntities(pos, 25f, true);
      bool flag = false;
      target = (Avatar) null;
      float num1 = float.MaxValue;
      int index1 = -1;
      for (int index2 = 0; index2 < entities.Count && !flag; ++index2)
      {
        if (entities[index2] is Avatar avatar && !avatar.IsEthereal)
        {
          float num2 = ((avatar.Position - pos) with
          {
            Y = 0.0f
          }).LengthSquared();
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            index1 = index2;
          }
        }
      }
      target = index1 == -1 ? (Avatar) null : entities[index1] as Avatar;
      entityManager.ReturnEntityList(entities);
    }

    public void OnExit(Cthulhu iOwner) => this.mCoolDown = 5f;

    public bool Active(Cthulhu iOwner, float iDeltaTime)
    {
      if ((double) this.mCoolDown > 0.0)
        return false;
      TriggerArea triggerArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Cthulhu.OtherworldlyBoltState.ANY);
      return triggerArea.GetCount(Cthulhu.OtherworldlyBoltState.CULTIST_HASH) + triggerArea.GetCount(Cthulhu.OtherworldlyBoltState.DEEP_ONE_HASH) <= Magicka.Game.Instance.PlayerCount;
    }

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
      if ((double) this.mCoolDown < 0.0)
        return;
      this.mCoolDown -= iDeltaTime;
    }

    private enum BoltState
    {
      Start,
      Mid,
      End,
    }
  }

  private class DeathState : Cthulhu.CthulhuState, IBossState<Cthulhu>
  {
    private VisualEffectReference mEffectRef;
    private int ParticleEffect = "cthulhu_death".GetHashCodeCustom();
    private bool mEffectStarted;

    public void OnEnter(Cthulhu iOwner)
    {
      for (int index = 0; index < iOwner.mActiveTentacles.Count; ++index)
        iOwner.mActiveTentacles[index].KillTentacle();
      iOwner.KillBolts();
      if (!iOwner.IsAtPoint(Cthulhu.BossSpawnPoints.NORTH))
      {
        iOwner.mDesiredSpawnPoint = 0;
        iOwner.ChangeState(Cthulhu.States.Submerge);
      }
      else
      {
        iOwner.CrossFade(Cthulhu.Animations.Die, 0.5f, false);
        AudioManager.Instance.PlayCue(Banks.Additional, Cthulhu.SOUND_DEATH, iOwner.mDamageZone.AudioEmitter);
        this.mEffectStarted = false;
        iOwner.mPlayState.Level.CurrentScene.ExecuteTrigger("bossdead".GetHashCodeCustom(), (Magicka.GameLogic.Entities.Character) null, false);
      }
    }

    public void OnUpdate(float iDeltaTime, Cthulhu iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.KillIdleEffect();
        iOwner.mDead = true;
      }
      else if (!this.mEffectStarted && (double) num >= 0.0)
      {
        this.mEffectStarted = true;
        Matrix mTransform = iOwner.mTransform;
        EffectManager.Instance.StartEffect(this.ParticleEffect, ref mTransform, out this.mEffectRef);
      }
      if (!this.mEffectStarted || !EffectManager.Instance.IsActive(ref this.mEffectRef))
        return;
      Matrix mTransform1 = iOwner.mTransform;
      EffectManager.Instance.UpdateOrientation(ref this.mEffectRef, ref mTransform1);
    }

    public void OnExit(Cthulhu iOwner)
    {
      iOwner.KillIdleEffect();
      iOwner.mDead = true;
    }

    public bool Active(Cthulhu iOwner, float iDeltaTime) => false;

    public void NonActiveUpdate(Cthulhu iOwner, float iDeltaTime)
    {
    }
  }

  public enum MessageType : ushort
  {
    Update,
    ChangeState,
    ChangeStage,
    ChangeTarget,
    SpawnPoint,
    Hypnotize,
    Timewarp,
    ActivateMist,
    SetCharacterToEat,
    CharmAndConfuse,
    ActivateDeflection,
    TentacleUpdate,
    TentacleChangeState,
    TentacleSpawn,
    TentacleGrab,
    TentacleAimTarget,
    TentacleReleaseAimTarget,
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct ActivateDeflectionMessage
  {
    public const ushort TYPE = 10;
  }

  internal struct CharmAndConfuseMessage
  {
    public const ushort TYPE = 9;
    public ushort Handle;
  }

  internal struct SetCharacterToEatMessage
  {
    public const ushort TYPE = 8;
    public ushort Handle;
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct ActivateMistMessage
  {
    public const ushort TYPE = 7;
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct TimewarpMessage
  {
    public const ushort TYPE = 6;
  }

  internal struct HypnotizeMessage
  {
    public const ushort TYPE = 5;
    public ushort Handle;
    public Normalized101010 Direction;
  }

  internal struct TentacleGrabMessage
  {
    public const ushort TYPE = 14;
    public ushort Handle;
    public byte TentacleIndex;
  }

  internal struct TentacleAimTargetMessage
  {
    public const ushort TYPE = 15;
    public ushort Handle;
    public byte TentacleIndex;
  }

  internal struct TentacleReleaseAimTargetMessage
  {
    public const ushort TYPE = 16 /*0x10*/;
    public byte TentacleIndex;
  }

  internal struct TentacleUpdateMessage
  {
    public const ushort TYPE = 11;
    public byte Animation;
    public HalfSingle AnimationTime;
    public byte TentacleIndex;
  }

  internal struct TentacleChangeStateMessage
  {
    public const ushort TYPE = 12;
    public Tentacle.States NewState;
    public byte TentacleIndex;
  }

  internal struct TentacleSpawnMessage
  {
    public const ushort TYPE = 13;
    public byte TentacleIndex;
  }

  internal struct UpdateMessage
  {
    public const ushort TYPE = 0;
    public byte Animation;
    public HalfSingle AnimationTime;
    public float Hitpoints;
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 1;
    public Cthulhu.States NewState;
  }

  internal struct ChangeStageMessage
  {
    public const ushort TYPE = 2;
    public Cthulhu.Stages NewStage;
  }

  internal struct ChangeTargetMessage
  {
    public const ushort TYPE = 3;
    public int Handle;
  }

  internal struct SpawnPointMessage
  {
    public const ushort TYPE = 4;
    public sbyte Index;
    public int SpawnPoint;
  }

  private enum BossSpawnPoints : byte
  {
    NORTH,
    NORTHWEST,
    WEST,
    SOUTHWEST,
    SOUTH,
    SOUTHEAST,
    EAST,
    NORTHEAST,
  }

  private enum Animations
  {
    OtherwordlyBoltStart,
    OtherwordlyBoltMid,
    OtherwordlyBoltEnd,
    Submerge,
    Mists,
    Timewarp,
    Mesmerize,
    Madness,
    Emerge,
    Idle,
    Devour,
    DevourHit,
    Cast_Lightning,
    Call_of_Cthulhu,
    Die,
    Rage,
    NR_OF_ANIMATIONS,
  }

  private enum Fingers
  {
    Thumb,
    Finger1,
    Finger2,
    Finger3,
    Finger4,
    Finger5,
  }

  private class RenderData : IRenderableObject
  {
    public Matrix[] Skeleton;
    public SkinnedModelDeferredNormalMappedMaterial Material;
    private VertexBuffer mVertices;
    private int mVerticesHash;
    private VertexDeclaration mDeclaration;
    private IndexBuffer mIndices;
    private int mBaseVertex;
    private int mNumVertices;
    private int mPrimCount;
    private int mStartIndex;
    private int mVertexStride;
    public float Damage;
    public float Flash;
    public Vector4 Colorize;
    public BoundingSphere BoundingSphere;

    public RenderData(int iSkeletonLength, ModelMesh iMesh, ModelMeshPart iPart)
    {
      this.Skeleton = new Matrix[iSkeletonLength];
      this.mVertices = iMesh.VertexBuffer;
      this.mIndices = iMesh.IndexBuffer;
      this.mDeclaration = iPart.VertexDeclaration;
      this.mBaseVertex = iPart.BaseVertex;
      this.mNumVertices = iPart.NumVertices;
      this.mPrimCount = iPart.PrimitiveCount;
      this.mStartIndex = iPart.StartIndex;
      this.mVertexStride = iPart.VertexStride;
      this.BoundingSphere = iMesh.BoundingSphere;
      this.Material.FetchFromEffect(iPart.Effect as SkinnedModelDeferredNormalMappedEffect);
    }

    public int Effect => SkinnedModelDeferredNormalMappedEffect.TYPEHASH;

    public int DepthTechnique => 1;

    public int Technique => 0;

    public int ShadowTechnique => 2;

    public VertexBuffer Vertices => this.mVertices;

    public int VerticesHashCode => this.mVerticesHash;

    public int VertexStride => this.mVertexStride;

    public IndexBuffer Indices => this.mIndices;

    public VertexDeclaration VertexDeclaration => this.mDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum) => !iViewFrustum.Intersects(this.BoundingSphere);

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredNormalMappedEffect iEffect1 = iEffect as SkinnedModelDeferredNormalMappedEffect;
      this.Material.AssignToEffect(iEffect1);
      iEffect1.Bones = this.Skeleton;
      iEffect1.Damage = this.Damage;
      iEffect1.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
      iEffect1.Colorize = this.Colorize;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimCount);
      iEffect1.OverrideColor = new Vector4();
      iEffect1.Colorize = new Vector4();
      iEffect1.Damage = 0.0f;
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredNormalMappedEffect iEffect1 = iEffect as SkinnedModelDeferredNormalMappedEffect;
      this.Material.AssignOpacityToEffect(iEffect1);
      iEffect1.Bones = this.Skeleton;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimCount);
    }
  }
}
