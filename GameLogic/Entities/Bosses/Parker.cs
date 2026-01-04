// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Parker
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Parker : BossStatusEffected, IBoss
{
  private const float NETWORK_UPDATE_PERIOD = 0.06666667f;
  private const float MAXHITPOINTS = 55000f;
  private const float HIT_COOLDOWN = 8f;
  private static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);
  private float mLastNetworkUpdate;
  protected float mNetworkUpdateTimer;
  private Parker.IParkerState[] mStates;
  private Parker.States mCurrentState;
  private Parker.States mPreviousState;
  private IBossState<Parker>[] mStageStates;
  private Parker.Stages mCurrentStage;
  private static readonly float[] WEBBING_TIME = new float[2]
  {
    0.2888889f,
    0.8f
  };
  private static readonly float[] BITE_TIME = new float[2]
  {
    0.5483871f,
    0.9354839f
  };
  private static readonly float[] ATTACK_FRONT_TIME = new float[2]
  {
    0.2f,
    0.6666667f
  };
  private static readonly float[] ATTACK_LEFT_RIGHT_TIME = new float[2]
  {
    0.28f,
    0.56f
  };
  private static readonly float[] SPAWN_TIME = new float[2]
  {
    0.289473683f,
    0.7894737f
  };
  private static readonly float[] MINION_TIME = new float[2]
  {
    0.289473683f,
    0.7894737f
  };
  private static readonly int SPAWN_LOCATOR = "parker_spawn".GetHashCodeCustom();
  private static readonly int BATTLE_LOCATOR = "parker_battle".GetHashCodeCustom();
  private static readonly int DEFEND_LOCATOR = "parker_defensive".GetHashCodeCustom();
  private static Random RANDOM = new Random();
  private AnimationClip[] mAnimationClips;
  private AnimationController mAnimationController;
  private Parker.RenderData[] mHeadRenderData;
  private Parker.RenderData[] mBodyRenderData;
  private Parker.RenderData[] mAssRenderData;
  private bool mDead;
  private float mMovementSpeed = 8f;
  private Matrix mMouthOrientation;
  private Vector3 mAbdomPosition = Vector3.Zero;
  private Vector3 mMovement = Vector3.Zero;
  private Vector3 mDesiredDirection = Vector3.Forward;
  private BossDamageZone mMouthZone;
  private BossDamageZone mAbdomenZone;
  private BossCollisionZone mLimbZone;
  private Vector3 mPlayerTargetPosition;
  private Vector3 mTargetRotateDirection;
  private Parker.States mRotationTransitionState;
  private Matrix mTransform;
  private Matrix mDefendTransform;
  private Matrix mBattleTransform;
  private Matrix mSpawnTransform;
  private Matrix mPivotTransform;
  private float mPivotDistance;
  private Vector3 mPivotTargetDirection;
  private PlayState mPlayState;
  private bool mPlayersDead;
  private HitList mHitList;
  private bool mIsHit;
  private float mHitTimer;
  private Avatar mAggroTarget;
  private float mHeadHitFlashTimer;
  private float mAssHitFlashTimer;
  private float mStageSpeedMod;
  private float mTimeBetweenActions;
  private float mStateSpeedMod;
  private float mAnimationSpeedMod;
  private float mMovementSpeedMod;
  private bool mTargetWebbed;
  private Avatar mTarget;
  private Player[] mPlayers;
  private Resistance[] mBodyResistances;
  private Resistance[] mHeadResistances;
  private Matrix[] mFootTransform = new Matrix[8];
  private int mMouthJointIndex;
  private Matrix mMouthBindPose;
  private int mAbdomJointIndex;
  private Matrix mAbdomBindPose;
  private Matrix mBackAbdomBindPose;
  private int mBackAbdomJointIndex;
  private int[] mFootJointIndex = new int[8];
  private Matrix[] mFootBindPose = new Matrix[8];
  private int[] mKneeJointIndex = new int[8];
  private Matrix[] mKneeBindPose = new Matrix[8];
  private int[] mBodyJointIndex = new int[8];
  private Matrix[] mBodyBindPose = new Matrix[8];
  private int[] mLeftIndices = new int[16 /*0x10*/];
  private int[] mRightIndices = new int[16 /*0x10*/];
  private static readonly int FOOTSTEP_EFFECT = "footstep_mud".GetHashCodeCustom();
  private static readonly int FOOTSTEP_SOUND = "parker_footstep".GetHashCodeCustom();
  private static readonly int INTRO_SOUND = "parker_intro".GetHashCodeCustom();
  private static readonly int ENTER_CAVE_SOUND = "parker_enter_cave".GetHashCodeCustom();
  private static readonly int EXIT_CAVE_SOUND = "parker_exit_cave".GetHashCodeCustom();
  private static readonly int CALL_MINION_SOUND = "parker_call_minion".GetHashCodeCustom();
  private static readonly int SPAWN_SOUND = "parker_spawn".GetHashCodeCustom();
  private static readonly int BITE_SOUND = "parker_bite".GetHashCodeCustom();
  private static readonly int SWIPE_SIDES_SOUND = "parker_swipe_sides".GetHashCodeCustom();
  private static readonly int SWIPE_FORWARD_SOUND = "parker_swipe_forward".GetHashCodeCustom();
  private static readonly int DEATH_SOUND = "parker_death".GetHashCodeCustom();
  private static readonly int DEFEND_SOUND = "parker_defend".GetHashCodeCustom();
  private static readonly float[] FOOTSTEP_TIMES = new float[4]
  {
    0.0f,
    7f / 32f,
    15f / 32f,
    23f / 32f
  };
  private int mCurrentFootstepIndex;

  public Parker(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mHitList = new HitList(16 /*0x10*/);
    this.mStates = new Parker.IParkerState[18];
    this.mStates[0] = (Parker.IParkerState) new Parker.IntroState();
    this.mStates[3] = (Parker.IParkerState) new Parker.EnterCaveState();
    this.mStates[2] = (Parker.IParkerState) new Parker.LeaveCaveState();
    this.mStates[12] = (Parker.IParkerState) new Parker.DefendState();
    this.mStates[1] = (Parker.IParkerState) new Parker.BattleState();
    this.mStates[10] = (Parker.IParkerState) new Parker.SpawnState();
    this.mStates[4] = (Parker.IParkerState) new Parker.SwipeState();
    this.mStates[8] = (Parker.IParkerState) new Parker.StunState();
    this.mStates[11] = (Parker.IParkerState) new Parker.CallMinionState();
    this.mStates[5] = (Parker.IParkerState) new Parker.BiteState();
    this.mStates[6] = (Parker.IParkerState) new Parker.RushState();
    this.mStates[7] = (Parker.IParkerState) new Parker.BackupState();
    this.mStates[9] = (Parker.IParkerState) new Parker.WebState();
    this.mStates[16 /*0x10*/] = (Parker.IParkerState) new Parker.CaveState();
    this.mStates[17] = (Parker.IParkerState) new Parker.DeadState();
    this.mStates[13] = (Parker.IParkerState) new Parker.RotateState();
    this.mStates[15] = (Parker.IParkerState) new Parker.MoveToCaveState();
    this.mStates[14] = (Parker.IParkerState) new Parker.MoveToBattleState();
    this.mStageStates = new IBossState<Parker>[4];
    this.mStageStates[0] = (IBossState<Parker>) new Parker.IntroStage();
    this.mStageStates[1] = (IBossState<Parker>) new Parker.BattleStage();
    this.mStageStates[2] = (IBossState<Parker>) new Parker.CriticalStage();
    this.mStageStates[3] = (IBossState<Parker>) new Parker.FinalStage();
    this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/spider_baby");
    this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/spider_forest");
    this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/spider_poison");
    SkinnedModel skinnedModel1;
    SkinnedModel skinnedModel2;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      skinnedModel1 = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Parker/Parker");
      skinnedModel2 = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Parker/Parker_animation");
    }
    this.mPlayersDead = false;
    this.mAnimationController = new AnimationController();
    this.mAnimationController.AnimationLooped += new XNAnimation.Controllers.AnimationEvent(this.AnimationLooped);
    this.mAnimationController.CrossfadeFinished += new XNAnimation.Controllers.AnimationEvent(this.AnimationLooped);
    this.mAnimationController.Skeleton = skinnedModel2.SkeletonBones;
    this.mAnimationClips = new AnimationClip[23];
    this.mAnimationClips[0] = skinnedModel2.AnimationClips["walk_backward"];
    this.mAnimationClips[1] = skinnedModel2.AnimationClips["walk_forward"];
    this.mAnimationClips[2] = skinnedModel2.AnimationClips["walk_left"];
    this.mAnimationClips[3] = skinnedModel2.AnimationClips["walk_right"];
    this.mAnimationClips[4] = skinnedModel2.AnimationClips["idle"];
    this.mAnimationClips[5] = skinnedModel2.AnimationClips["rotate_left"];
    this.mAnimationClips[6] = skinnedModel2.AnimationClips["rotate_right"];
    this.mAnimationClips[7] = skinnedModel2.AnimationClips["attack_left"];
    this.mAnimationClips[8] = skinnedModel2.AnimationClips["attack_right"];
    this.mAnimationClips[9] = skinnedModel2.AnimationClips["attack_front"];
    this.mAnimationClips[10] = skinnedModel2.AnimationClips["bite"];
    this.mAnimationClips[11] = skinnedModel2.AnimationClips["web"];
    this.mAnimationClips[12] = skinnedModel2.AnimationClips["spawn"];
    this.mAnimationClips[13] = skinnedModel2.AnimationClips["defend"];
    this.mAnimationClips[14] = skinnedModel2.AnimationClips["stun"];
    this.mAnimationClips[15] = skinnedModel2.AnimationClips["cave_enter"];
    this.mAnimationClips[16 /*0x10*/] = skinnedModel2.AnimationClips["cave_help"];
    this.mAnimationClips[17] = skinnedModel2.AnimationClips["cave_idle"];
    this.mAnimationClips[18] = skinnedModel2.AnimationClips["cave_taunt"];
    this.mAnimationClips[19] = skinnedModel2.AnimationClips["cave_exit"];
    this.mAnimationClips[20] = skinnedModel2.AnimationClips["intro"];
    this.mAnimationClips[21] = skinnedModel2.AnimationClips["taunt"];
    this.mAnimationClips[22] = skinnedModel2.AnimationClips["die"];
    int index1 = 0;
    int index2 = 0;
    int index3 = 0;
    Matrix result1;
    Matrix.CreateRotationY(3.14159274f, out result1);
    Matrix matrix = new Matrix();
    for (int index4 = 0; index4 < skinnedModel1.SkeletonBones.Count; ++index4)
    {
      SkinnedModelBone skeletonBone = skinnedModel2.SkeletonBones[index4];
      for (int index5 = 0; index5 < 16 /*0x10*/; ++index5)
      {
        string str1 = $"LeftLeg{index5 / 4}";
        switch (index5 % 4)
        {
          case 0:
            str1 += "0";
            break;
          case 1:
            str1 += "1";
            break;
          case 2:
            str1 += "3";
            break;
          case 3:
            str1 += "5";
            break;
        }
        if (skeletonBone.Name.Equals(str1, StringComparison.OrdinalIgnoreCase))
        {
          this.mLeftIndices[index5] = (int) skeletonBone.Index;
        }
        else
        {
          string str2 = $"RightLeg{index5 / 4}";
          switch (index5 % 4)
          {
            case 0:
              str2 += "0";
              break;
            case 1:
              str2 += "1";
              break;
            case 2:
              str2 += "3";
              break;
            case 3:
              str2 += "5";
              break;
          }
          if (skeletonBone.Name.Equals(str2, StringComparison.OrdinalIgnoreCase))
            this.mRightIndices[index5] = (int) skeletonBone.Index;
        }
      }
      Matrix result2;
      if (skeletonBone.Name.Equals("Mouth", StringComparison.OrdinalIgnoreCase))
      {
        this.mMouthJointIndex = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mMouthBindPose);
      }
      else if (skeletonBone.Name.Equals("Spine1", StringComparison.OrdinalIgnoreCase))
      {
        this.mAbdomJointIndex = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mAbdomBindPose);
      }
      else if (skeletonBone.Name.Equals("LeftLeg05", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("LeftLeg15", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("LeftLeg25", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("LeftLeg35", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg05", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg15", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg25", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg35", StringComparison.OrdinalIgnoreCase))
      {
        this.mFootJointIndex[index1] = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mFootBindPose[index1]);
        ++index1;
      }
      else if (skeletonBone.Name.Equals("LeftLeg01", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("LeftLeg11", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("LeftLeg21", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("LeftLeg31", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg01", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg11", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg21", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg31", StringComparison.OrdinalIgnoreCase))
      {
        this.mKneeJointIndex[index2] = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mKneeBindPose[index2]);
        ++index2;
      }
      else if (skeletonBone.Name.Equals("LeftLeg00", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("LeftLeg10", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("LeftLeg20", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("LeftLeg30", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg00", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg10", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg20", StringComparison.OrdinalIgnoreCase) || skeletonBone.Name.Equals("RightLeg30", StringComparison.OrdinalIgnoreCase))
      {
        this.mBodyJointIndex[index3] = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mBodyBindPose[index3]);
        ++index3;
      }
      else if (skeletonBone.Name.Equals("Ass", StringComparison.OrdinalIgnoreCase))
      {
        this.mBackAbdomJointIndex = (int) skeletonBone.Index;
        result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out this.mBackAbdomBindPose);
      }
    }
    Primitive[] primitiveArray = new Primitive[16 /*0x10*/];
    for (int index6 = 0; index6 < 8; ++index6)
    {
      float length = Vector3.Distance(this.mKneeBindPose[index6].Translation, this.mFootBindPose[index6].Translation);
      primitiveArray[index6] = (Primitive) new Capsule(new Vector3(), Matrix.Identity, 0.55f, length);
    }
    for (int index7 = 0; index7 < 8; ++index7)
    {
      float length = Vector3.Distance(this.mBodyBindPose[index7].Translation, this.mKneeBindPose[index7].Translation);
      primitiveArray[8 + index7] = (Primitive) new Capsule(new Vector3(), Matrix.Identity, 0.8f, length);
    }
    this.mLimbZone = new BossCollisionZone(iPlayState, (IBoss) this, primitiveArray);
    this.mLimbZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mAbdomenZone = new BossDamageZone(iPlayState, (IBoss) this, 0, 2.5f, (Primitive) new Sphere(Vector3.Up, 2.5f));
    this.mAbdomenZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mMouthZone = new BossDamageZone(iPlayState, (IBoss) this, 1, 0.9f, (Primitive) new Sphere(Vector3.Zero, 0.8f));
    this.mMouthZone.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnMouthCollision);
    this.mHeadRenderData = new Parker.RenderData[3];
    this.mBodyRenderData = new Parker.RenderData[3];
    this.mAssRenderData = new Parker.RenderData[3];
    for (int index8 = 0; index8 < 3; ++index8)
    {
      this.mHeadRenderData[index8] = new Parker.RenderData(skinnedModel1.Model.Meshes[0]);
      this.mHeadRenderData[index8].SetMesh(skinnedModel1.Model.Meshes[0].VertexBuffer, skinnedModel1.Model.Meshes[0].IndexBuffer, skinnedModel1.Model.Meshes[0].MeshParts[0], 0, 3, 4);
      this.mHeadRenderData[index8].mBoundingSphere.Radius = 15f;
      this.mBodyRenderData[index8] = new Parker.RenderData(skinnedModel1.Model.Meshes[1]);
      this.mBodyRenderData[index8].SetMesh(skinnedModel1.Model.Meshes[1].VertexBuffer, skinnedModel1.Model.Meshes[1].IndexBuffer, skinnedModel1.Model.Meshes[1].MeshParts[0], 0, 3, 4);
      this.mBodyRenderData[index8].mBoundingSphere.Radius = 15f;
      this.mAssRenderData[index8] = new Parker.RenderData(skinnedModel1.Model.Meshes[2]);
      this.mAssRenderData[index8].SetMesh(skinnedModel1.Model.Meshes[2].VertexBuffer, skinnedModel1.Model.Meshes[2].IndexBuffer, skinnedModel1.Model.Meshes[2].MeshParts[0], 0, 3, 4);
      this.mAssRenderData[index8].mBoundingSphere.Radius = 15f;
    }
    this.mBodyResistances = new Resistance[this.mResistances.Length];
    for (int iIndex = 0; iIndex < this.mBodyResistances.Length; ++iIndex)
    {
      this.mBodyResistances[iIndex].ResistanceAgainst = Defines.ElementFromIndex(iIndex);
      this.mBodyResistances[iIndex].Modifier = 0.0f;
      switch (this.mBodyResistances[iIndex].ResistanceAgainst)
      {
        case Elements.Earth:
        case Elements.Arcane:
          this.mBodyResistances[iIndex].Multiplier = 1.25f;
          break;
        case Elements.Cold:
          this.mBodyResistances[iIndex].Multiplier = 0.5f;
          break;
        case Elements.Fire:
          this.mBodyResistances[iIndex].Multiplier = 1.5f;
          break;
        case Elements.Poison:
          this.mBodyResistances[iIndex].Multiplier = 0.0f;
          break;
        default:
          this.mBodyResistances[iIndex].Multiplier = 1f;
          break;
      }
    }
    this.mHeadResistances = new Resistance[this.mResistances.Length];
    for (int iIndex = 0; iIndex < this.mHeadResistances.Length; ++iIndex)
    {
      this.mHeadResistances[iIndex].ResistanceAgainst = Defines.ElementFromIndex(iIndex);
      this.mHeadResistances[iIndex].Modifier = 0.0f;
      switch (this.mHeadResistances[iIndex].ResistanceAgainst)
      {
        case Elements.Earth:
          this.mHeadResistances[iIndex].Multiplier = 0.5f;
          this.mHeadResistances[iIndex].Modifier = 40f;
          break;
        case Elements.Water:
        case Elements.Lightning:
        case Elements.Arcane:
        case Elements.Life:
        case Elements.Shield:
        case Elements.Ice:
        case Elements.Steam:
          this.mHeadResistances[iIndex].Multiplier = 1f;
          break;
        case Elements.Cold:
          this.mHeadResistances[iIndex].Multiplier = 0.5f;
          break;
        case Elements.Fire:
          this.mHeadResistances[iIndex].Multiplier = 1.5f;
          break;
        case Elements.Poison:
          this.mHeadResistances[iIndex].Multiplier = 0.0f;
          break;
      }
    }
  }

  private void AnimationLooped() => this.mCurrentFootstepIndex = -1;

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mDead = false;
    this.mMaxHitPoints = 55000f;
    this.mHitPoints = 55000f;
    this.mTransform = iOrientation;
    this.mPlayState.Level.CurrentScene.GetLocator(Parker.BATTLE_LOCATOR, out this.mBattleTransform);
    this.mPlayState.Level.CurrentScene.GetLocator(Parker.SPAWN_LOCATOR, out this.mSpawnTransform);
    this.mPlayState.Level.CurrentScene.GetLocator(Parker.DEFEND_LOCATOR, out this.mDefendTransform);
    this.mPivotDistance = (this.mDefendTransform.Translation - this.mBattleTransform.Translation).Length();
    this.ResetBattleTransform();
    this.mTransform = this.BattleStateTransform;
    this.mMovement = new Vector3();
    this.mIsHit = false;
    this.mHitTimer = 0.0f;
    this.mPlayersDead = false;
    this.mMouthZone.Initialize();
    this.mMouthZone.Body.CollisionSkin.NonCollidables.Add(this.mAbdomenZone.Body.CollisionSkin);
    this.mMouthZone.Body.CollisionSkin.NonCollidables.Add(this.mLimbZone.Body.CollisionSkin);
    this.mMouthZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mMouthZone);
    this.mAbdomenZone.Initialize();
    this.mAbdomenZone.Body.CollisionSkin.NonCollidables.Add(this.mLimbZone.Body.CollisionSkin);
    this.mAbdomenZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mAbdomenZone);
    this.mLimbZone.Initialize();
    this.mLimbZone.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) this.mLimbZone);
    Matrix mTransform = this.mTransform;
    Vector3 translation = mTransform.Translation;
    mTransform.Translation = new Vector3();
    this.mAbdomenZone.Body.MoveTo(ref translation, ref mTransform);
    this.mAbdomenZone.Update(DataChannel.None, 0.0f);
    this.mPlayers = Magicka.Game.Instance.Players;
    this.mHitList.Clear();
    this.mStageSpeedMod = 1f;
    this.mStateSpeedMod = 1f;
    this.mAnimationSpeedMod = 1f;
    this.mMovementSpeedMod = 1f;
    this.mCurrentStage = Parker.Stages.Intro;
    this.mStageStates[(int) this.mCurrentStage].OnEnter(this);
    this.mCurrentState = Parker.States.Intro;
    this.mStates[(int) this.mCurrentState].OnEnter(this);
    for (int index = 0; index < this.mStatusEffects.Length; ++index)
    {
      this.mStatusEffects[index].Stop();
      this.mStatusEffects[index] = new StatusEffect();
    }
    this.mCurrentStatusEffects = StatusEffects.None;
    this.mAnimationController.PlaybackMode = PlaybackMode.Forward;
    this.mAnimationController.Speed = this.mStageSpeedMod;
    this.mTargetWebbed = false;
    this.mCurrentFootstepIndex = -1;
  }

  protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (this.mDead || iSkin1.Owner == null || iSkin0.Owner.Tag is BossCollisionZone)
      return false;
    if (iSkin1.Owner.Tag is IDamageable)
    {
      if (iSkin1.Owner.Tag is SprayEntity)
        return false;
      IDamageable tag = iSkin1.Owner.Tag as IDamageable;
      if (!this.mHitList.Contains(tag))
      {
        if (tag is Magicka.GameLogic.Entities.Character)
        {
          if (this.mCurrentState == Parker.States.MoveToCave || this.mCurrentState == Parker.States.Backup || this.mCurrentState == Parker.States.Rotate || this.mCurrentState == Parker.States.EnterCave)
          {
            DamageCollection5 iDamage = new DamageCollection5();
            iDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth, 100f, 1f));
            iDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, tag.Body.Mass, 1f));
            int num = (int) tag.Damage(iDamage, (Magicka.GameLogic.Entities.Entity) this.mAbdomenZone, this.mPlayState.PlayTime, this.mAbdomPosition);
          }
        }
        else
        {
          DamageCollection5 iDamage = new DamageCollection5();
          iDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth, 100f, 1f));
          int num = (int) tag.Damage(iDamage, (Magicka.GameLogic.Entities.Entity) this.mAbdomenZone, this.mPlayState.PlayTime, this.mAbdomPosition);
        }
        this.mHitList.Add(tag.Handle, 1f);
      }
      return true;
    }
    return this.mCurrentState != Parker.States.Bite && this.mCurrentState != Parker.States.Swipe;
  }

  protected bool OnMouthCollision(
    CollisionSkin iSkin0,
    int iPrim0,
    CollisionSkin iSkin1,
    int iPrim1)
  {
    if (this.mDead || iSkin1.Owner == null)
      return false;
    if (iSkin1.Owner.Tag is IDamageable && !this.mAnimationController.CrossFadeEnabled)
    {
      if (this.mCurrentState == Parker.States.Bite)
      {
        float num1 = this.mAnimationController.Time / this.mAnimationController.AnimationClip.Duration;
        if ((double) num1 > 0.33300000429153442 && (iSkin1.Owner.Tag is Shield || iSkin1.Owner.Tag is Barrier))
        {
          this.mAnimationController.Time = this.mAnimationController.AnimationClip.Duration - 0.3f;
          (iSkin1.Owner.Tag as IDamageable).Kill();
          return false;
        }
        if ((double) num1 >= (double) Parker.BITE_TIME[0] && (double) num1 <= (double) Parker.BITE_TIME[1])
        {
          IDamageable tag = iSkin1.Owner.Tag as IDamageable;
          if (!this.mHitList.Contains(tag))
          {
            DamageCollection5 iDamage = new DamageCollection5();
            iDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth, 2500f, 1f));
            iDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Poison, 100f, 1f));
            int num2 = (int) tag.Damage(iDamage, (Magicka.GameLogic.Entities.Entity) this.mMouthZone, this.mPlayState.PlayTime, this.mTransform.Translation);
            this.mHitList.Add(tag.Handle);
          }
        }
      }
      else if (this.mCurrentState == Parker.States.Swipe && this.GetSwipState.mSwipeType == Parker.SwipeState.Directions.Front)
      {
        if (iSkin1.Owner.Tag is Shield | iSkin1.Owner.Tag is Barrier)
        {
          (iSkin1.Owner.Tag as IDamageable).Kill();
          return false;
        }
        IDamageable tag = iSkin1.Owner.Tag as IDamageable;
        if (!this.mHitList.Contains(tag))
        {
          DamageCollection5 iDamage = new DamageCollection5();
          iDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, tag.Body.Mass, 3f));
          iDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Poison, 100f, 1f));
          int num = (int) tag.Damage(iDamage, (Magicka.GameLogic.Entities.Entity) this.mMouthZone, this.mPlayState.PlayTime, this.mTransform.Translation);
          this.mHitList.Add(tag.Handle);
        }
      }
    }
    return this.mCurrentState != Parker.States.Bite;
  }

  public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
  {
    bool flag1 = NetworkManager.Instance.State == NetworkState.Client;
    if ((double) this.mHitPoints <= 0.0 && !flag1 && this.mCurrentState != Parker.States.Dead)
      this.ChangeState(Parker.States.Dead);
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mNetworkUpdateTimer -= iDeltaTime;
      if ((double) this.mNetworkUpdateTimer <= 0.0)
      {
        this.mNetworkUpdateTimer = 0.06666667f;
        this.NetworkUpdate();
      }
    }
    this.UpdateDamage(iDeltaTime);
    this.UpdateStatusEffects(iDeltaTime);
    if ((double) this.mHitTimer > 0.0)
      this.mIsHit = false;
    this.mHitTimer -= iDeltaTime;
    this.mHitList.Update(iDeltaTime);
    bool flag2 = true;
    for (int index = 0; index < this.mPlayers.Length; ++index)
    {
      if (this.mPlayers[index].Playing && this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
        flag2 = false;
    }
    this.mPlayersDead = flag2;
    if (this.mTarget != null)
    {
      this.mPlayerTargetPosition = this.mTarget.Position;
      this.mTargetWebbed = this.mTarget.IsEntangled;
    }
    if (!flag1 && this.mCurrentState != Parker.States.Dead)
    {
      float num = this.HitPoints / 55000f;
      if ((double) num > 0.949999988079071)
      {
        if (this.mCurrentStage != Parker.Stages.Intro)
          this.ChangeStage(Parker.Stages.Intro);
      }
      else if ((double) num >= 0.5)
      {
        if (this.mCurrentStage != Parker.Stages.Battle)
          this.ChangeStage(Parker.Stages.Battle);
      }
      else if ((double) num > 0.15000000596046448)
      {
        if (this.mCurrentStage != Parker.Stages.Critical)
          this.ChangeStage(Parker.Stages.Critical);
      }
      else if ((double) num > 0.0 && this.mCurrentStage != Parker.Stages.Final)
        this.ChangeStage(Parker.Stages.Final);
      this.mStageStates[(int) this.mCurrentStage].OnUpdate(iDeltaTime, this);
    }
    this.mStates[(int) this.mCurrentState].OnUpdate(iDeltaTime, this);
    float num1 = 1f;
    if (this.HasStatus(StatusEffects.Cold))
      num1 = this.GetColdSpeed();
    Vector3 vector3 = this.mTransform.Translation;
    if (flag1)
    {
      vector3.X += this.mMovement.X * this.mMovementSpeed * iDeltaTime;
      vector3.Z += this.mMovement.Z * this.mMovementSpeed * iDeltaTime;
    }
    else
    {
      vector3.X += this.mMovement.X * this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * iDeltaTime * this.mStateSpeedMod * num1;
      vector3.Z += this.mMovement.Z * this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * iDeltaTime * this.mStateSpeedMod * num1;
    }
    Segment seg = new Segment();
    seg.Origin = vector3;
    ++seg.Origin.Y;
    seg.Delta.Y -= 3f;
    Vector3 pos;
    if (this.mPlayState.Level.CurrentScene.CollisionSkin.SegmentIntersect(out float _, out pos, out Vector3 _, seg))
      vector3 = pos;
    vector3.Y += 0.2f;
    this.mTransform.Translation = vector3;
    if (this.mCurrentState == Parker.States.Dead)
      this.mAnimationController.Speed = 1f;
    else if (!flag1)
      this.mAnimationController.Speed = this.mStateSpeedMod * this.mStageSpeedMod * this.mAnimationSpeedMod * num1;
    this.mAnimationController.Update(iDeltaTime, ref this.mTransform, true);
    Transform iTransform = new Transform();
    Vector3 up = Vector3.Up;
    Vector3 zero = Vector3.Zero;
    for (int prim = 0; prim < 8; ++prim)
    {
      Vector3 result1 = this.mKneeBindPose[prim].Translation;
      Vector3.Transform(ref result1, ref this.mAnimationController.SkinnedBoneTransforms[this.mKneeJointIndex[prim]], out result1);
      Vector3 result2 = this.mFootBindPose[prim].Translation;
      Vector3.Transform(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mFootJointIndex[prim]], out result2);
      Vector3 result3 = this.mBodyBindPose[prim].Translation;
      Vector3.Transform(ref result3, ref this.mAnimationController.SkinnedBoneTransforms[this.mBodyJointIndex[prim]], out result3);
      Vector3 result4;
      Vector3.Subtract(ref result1, ref result2, out result4);
      result4.Normalize();
      Matrix.CreateWorld(ref zero, ref result4, ref up, out iTransform.Orientation);
      iTransform.Position = result1;
      this.mFootTransform[prim] = iTransform.Orientation;
      this.mFootTransform[prim].Translation = result2;
      this.mLimbZone.Body.CollisionSkin.GetPrimitiveLocal(prim).SetTransform(ref iTransform);
      this.mLimbZone.Body.CollisionSkin.GetPrimitiveNewWorld(prim).SetTransform(ref iTransform);
      this.mLimbZone.Body.CollisionSkin.GetPrimitiveOldWorld(prim).SetTransform(ref iTransform);
      Vector3.Subtract(ref result3, ref result1, out result4);
      result4.Normalize();
      Matrix.CreateWorld(ref zero, ref result4, ref up, out iTransform.Orientation);
      iTransform.Position = result3;
      this.mLimbZone.Body.CollisionSkin.GetPrimitiveLocal(8 + prim).SetTransform(ref iTransform);
      this.mLimbZone.Body.CollisionSkin.GetPrimitiveNewWorld(8 + prim).SetTransform(ref iTransform);
      this.mLimbZone.Body.CollisionSkin.GetPrimitiveOldWorld(8 + prim).SetTransform(ref iTransform);
    }
    this.mLimbZone.Body.CollisionSkin.UpdateWorldBoundingBox();
    Vector3 result5 = this.mAbdomBindPose.Translation;
    Vector3.Transform(ref result5, ref this.mAnimationController.SkinnedBoneTransforms[this.mAbdomJointIndex], out result5);
    Vector3 result6 = this.mBackAbdomBindPose.Translation;
    Vector3.Transform(ref result6, ref this.mAnimationController.SkinnedBoneTransforms[this.mBackAbdomJointIndex], out result6);
    float y = result5.Y;
    Vector3.Lerp(ref result5, ref result6, -1f, out result5);
    result5.Y = y;
    this.mAbdomPosition = result5;
    this.mAbdomenZone.SetPosition(ref result5);
    this.mMouthOrientation = this.mMouthBindPose;
    Matrix.Multiply(ref this.mMouthOrientation, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthJointIndex], out this.mMouthOrientation);
    result5 = this.mMouthOrientation.Translation;
    this.mMouthZone.SetPosition(ref result5);
    if (this.mAnimationController.AnimationClip == this.mAnimationClips[1] || this.mAnimationController.AnimationClip == this.mAnimationClips[0] || this.mAnimationController.AnimationClip == this.mAnimationClips[2] || this.mAnimationController.AnimationClip == this.mAnimationClips[3] || this.mAnimationController.CrossFadeAnimationClip == this.mAnimationClips[1] || this.mAnimationController.CrossFadeAnimationClip == this.mAnimationClips[0] || this.mAnimationController.CrossFadeAnimationClip == this.mAnimationClips[2] || this.mAnimationController.CrossFadeAnimationClip == this.mAnimationClips[3])
    {
      float num2 = this.mAnimationController.Time / this.mAnimationController.AnimationClip.Duration;
      for (int index = Parker.FOOTSTEP_TIMES.Length - 1; index >= 0; --index)
      {
        if ((double) num2 >= (double) Parker.FOOTSTEP_TIMES[index] && this.mCurrentFootstepIndex < index)
        {
          this.mCurrentFootstepIndex = index;
          AudioManager.Instance.PlayCue(Banks.Additional, Parker.FOOTSTEP_SOUND, this.mAbdomenZone.AudioEmitter);
        }
      }
    }
    Parker.RenderData iObject1 = this.mHeadRenderData[(int) iDataChannel];
    Parker.RenderData iObject2 = this.mBodyRenderData[(int) iDataChannel];
    Parker.RenderData iObject3 = this.mAssRenderData[(int) iDataChannel];
    float num3 = System.Math.Min(this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude * 10f, 1f);
    iObject1.mMaterial.Colorize.X = iObject2.mMaterial.Colorize.X = iObject3.mMaterial.Colorize.X = Parker.ColdColor.X;
    iObject1.mMaterial.Colorize.Y = iObject2.mMaterial.Colorize.Y = iObject3.mMaterial.Colorize.Y = Parker.ColdColor.Y;
    iObject1.mMaterial.Colorize.Z = iObject2.mMaterial.Colorize.Z = iObject3.mMaterial.Colorize.Z = Parker.ColdColor.Z;
    iObject1.mMaterial.Colorize.W = iObject2.mMaterial.Colorize.W = iObject3.mMaterial.Colorize.W = num3;
    this.mHeadHitFlashTimer = System.Math.Max(this.mHeadHitFlashTimer - iDeltaTime * 10f, 0.0f);
    this.mAssHitFlashTimer = System.Math.Max(this.mAssHitFlashTimer - iDeltaTime * 10f, 0.0f);
    iObject1.mBoundingSphere.Center = this.mTransform.Translation;
    iObject2.mBoundingSphere.Center = this.mTransform.Translation;
    iObject3.mBoundingSphere.Center = this.mTransform.Translation;
    iObject1.mDamage = (float) (1.0 - (double) this.mHitPoints / 55000.0);
    iObject2.mDamage = (float) (1.0 - (double) this.mHitPoints / 55000.0);
    iObject3.mDamage = (float) (1.0 - (double) this.mHitPoints / 55000.0);
    iObject1.mFlash = this.mHeadHitFlashTimer;
    iObject3.mFlash = this.mAssHitFlashTimer;
    Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, (Array) iObject1.mSkeleton, iObject1.mSkeleton.Length);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject1);
    Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, (Array) iObject2.mSkeleton, iObject2.mSkeleton.Length);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject2);
    Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, (Array) iObject3.mSkeleton, iObject3.mSkeleton.Length);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject3);
  }

  protected void ResetBattleTransform()
  {
    this.mPivotTransform = this.mSpawnTransform;
    this.mPivotTransform.Translation = this.mDefendTransform.Translation;
  }

  protected Matrix BattleStateTransform
  {
    get
    {
      Matrix mPivotTransform = this.mPivotTransform;
      Vector3 result1 = this.mPivotTransform.Translation;
      Vector3 result2 = this.mPivotTransform.Forward;
      Vector3.Multiply(ref result2, this.mPivotDistance, out result2);
      Vector3.Add(ref result2, ref result1, out result1);
      mPivotTransform.Translation = result1;
      return mPivotTransform;
    }
  }

  protected float GetColdSpeed()
  {
    return System.Math.Max(this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown(), 0.666f);
  }

  protected float ColdMagnitude
  {
    get => this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude;
  }

  protected float BurningMagnitude
  {
    get => this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Magnitude;
  }

  protected float BurningDPS
  {
    get => this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].DPS;
  }

  public void DeInitialize()
  {
  }

  private Parker.SwipeState GetSwipState => this.mStates[4] as Parker.SwipeState;

  private Parker.BiteState GetBiteState => this.mStates[5] as Parker.BiteState;

  private Parker.RushState GetRushState => this.mStates[6] as Parker.RushState;

  private Parker.WebState GetWebState => this.mStates[9] as Parker.WebState;

  private Parker.SpawnState GetSpawnState => this.mStates[10] as Parker.SpawnState;

  private Parker.CallMinionState GetCallMinionState => this.mStates[11] as Parker.CallMinionState;

  private Parker.CaveState GetCaveState => this.mStates[16 /*0x10*/] as Parker.CaveState;

  private Parker.DefendState GetDefendState => this.mStates[12] as Parker.DefendState;

  protected Vector3 Movement
  {
    get => this.mMovement;
    set
    {
      value.Y = 0.0f;
      this.mMovement = value;
      float d = this.mMovement.LengthSquared();
      if ((double) d <= 9.9999999747524271E-07)
        return;
      float num1 = (float) System.Math.Sqrt((double) d);
      float num2 = 1f / num1;
      this.mDesiredDirection.X = value.X * num2;
      this.mDesiredDirection.Y = value.Y * num2;
      this.mDesiredDirection.Z = value.Z * num2;
      if ((double) num1 <= 1.0)
        return;
      this.mMovement.X = value.X * num2;
      this.mMovement.Y = value.Y * num2;
      this.mMovement.Z = value.Z * num2;
    }
  }

  protected unsafe void ChangeState(Parker.States iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Parker.ChangeStateMessage changeStateMessage;
      changeStateMessage.NewState = iState;
      BossFight.Instance.SendMessage<Parker.ChangeStateMessage>((IBoss) this, (ushort) 2, (void*) &changeStateMessage, true);
    }
    this.mStates[(int) this.mCurrentState].OnExit(this);
    this.mPreviousState = this.mCurrentState;
    this.mCurrentState = iState;
    this.mStates[(int) this.mCurrentState].OnEnter(this);
  }

  protected unsafe void ChangeStage(Parker.Stages iStage)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.mStageStates[(int) this.mCurrentStage].OnExit(this);
    this.mCurrentStage = iStage;
    this.mStageStates[(int) this.mCurrentStage].OnEnter(this);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    Parker.ChangeStageMessage changeStageMessage;
    changeStageMessage.NewStage = iStage;
    BossFight.Instance.SendMessage<Parker.ChangeStageMessage>((IBoss) this, (ushort) 1, (void*) &changeStageMessage, true);
  }

  protected bool GetRandomTarget(out Avatar oAvatar)
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
        int index = Cthulhu.RANDOM.Next(this.mPlayers.Length);
        int num3 = 1 << index;
        if ((num1 & num2) == num2)
          return false;
        if ((num1 & num3) != num3)
        {
          num1 |= num3;
          if (this.mPlayers[index].Playing && this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
          {
            oAvatar = this.mPlayers[index].Avatar;
            randomTarget = true;
          }
        }
      }
    }
    return randomTarget;
  }

  public override bool Dead => this.mDead;

  public float MaxHitPoints => this.mMaxHitPoints;

  public float HitPoints => this.mHitPoints;

  internal unsafe void SetTarget(Avatar iAvatar)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
      BossFight.Instance.SendMessage<Parker.ChangeTargetMessage>((IBoss) this, (ushort) 3, (void*) &new Parker.ChangeTargetMessage()
      {
        Handle = (int) iAvatar.Handle
      }, true);
    if (this.mTarget != null && (this.mTarget.IsEntangled || this.mTarget.HasStatus(StatusEffects.Poisoned)))
      return;
    this.mTarget = iAvatar;
  }

  public bool AddImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return false;
  }

  public void SetSlow(int iIndex) => throw new NotImplementedException();

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    oPosition = new Vector3();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => this.HasStatus(iStatus);

  public float StatusMagnitude(int iIndex, StatusEffects iStatus) => this.StatusMagnitude(iStatus);

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Magicka.GameLogic.Entities.Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult damageResult = DamageResult.None;
    if (this.mCurrentState != Parker.States.Defend && this.mCurrentState != Parker.States.CallMinions && this.mCurrentState != Parker.States.Cave)
    {
      switch (iPartIndex)
      {
        case 0:
          this.mResistances = this.mBodyResistances;
          break;
        case 1:
          this.mResistances = this.mHeadResistances;
          break;
      }
      damageResult = this.Damage(iDamage, iAttacker, iAttackPosition, iFeatures);
    }
    return damageResult;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
  {
    if (iPartIndex == 0)
    {
      this.mResistances = this.mBodyResistances;
      this.Damage(iDamage, iElement);
    }
    else
    {
      if (iPartIndex != 1)
        throw new Exception($"Parker damaged on index:{(object) iPartIndex}!");
      if (this.mCurrentState == Parker.States.Defend)
        return;
      this.mResistances = this.mHeadResistances;
      this.Damage(iDamage, iElement);
    }
  }

  protected override int BloodEffect => Gib.GORE_GIB_MEDIUM_EFFECTS[4];

  protected override BossDamageZone Entity => this.mAbdomenZone;

  protected override float Radius
  {
    get => (this.mAbdomenZone.Body.CollisionSkin.GetPrimitiveLocal(0) as Sphere).Radius;
  }

  protected override float Length
  {
    get => (this.mAbdomenZone.Body.CollisionSkin.GetPrimitiveLocal(0) as Sphere).Radius;
  }

  protected override Vector3 NotifierTextPostion
  {
    get
    {
      Vector3 translation = this.mMouthOrientation.Translation;
      translation.Y += 3f;
      return translation;
    }
  }

  public void ScriptMessage(BossMessages iMessage)
  {
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

  private unsafe void NetworkUpdate()
  {
    NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
    Parker.UpdateMessage updateMessage1 = new Parker.UpdateMessage();
    updateMessage1.Animation = (byte) 0;
    while ((int) updateMessage1.Animation < this.mAnimationClips.Length && this.mAnimationController.AnimationClip != this.mAnimationClips[(int) updateMessage1.Animation])
      ++updateMessage1.Animation;
    updateMessage1.Hitpoints = (ushort) System.Math.Max(System.Math.Min(this.mHitPoints, (float) ushort.MaxValue), 0.0f);
    updateMessage1.AnimationTime = new HalfSingle(this.mAnimationController.Time);
    updateMessage1.AnimationSpeed = new HalfSingle(this.mAnimationController.Speed);
    float num1 = 1f;
    if (this.HasStatus(StatusEffects.Cold))
      num1 = this.GetColdSpeed();
    updateMessage1.Speed = new HalfSingle(this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * this.mStateSpeedMod * num1);
    updateMessage1.PositionUpdate = this.mCurrentState == Parker.States.Rush || this.mCurrentState == Parker.States.Backup || this.mCurrentState == Parker.States.MoveToBattle || this.mCurrentState == Parker.States.MoveToCave;
    updateMessage1.Position = this.mTransform.Translation;
    for (int index = 0; index < networkServer.Connections; ++index)
    {
      float num2 = networkServer.GetLatency(index) * 0.5f;
      Parker.UpdateMessage updateMessage2 = updateMessage1 with
      {
        AnimationTime = new HalfSingle(this.mAnimationController.Time + num2)
      };
      updateMessage2.Position.X += this.mMovement.X * this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * num2 * this.mStateSpeedMod * num1;
      updateMessage2.Position.Z += this.mMovement.Z * this.mMovementSpeed * this.mStageSpeedMod * this.mMovementSpeedMod * num2 * this.mStateSpeedMod * num1;
      BossFight.Instance.SendMessage<Parker.UpdateMessage>((IBoss) this, (ushort) 0, (void*) &updateMessage2, false, index);
    }
  }

  public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
  {
    switch (iMsg.Type)
    {
      case 0:
        if ((double) iMsg.TimeStamp < (double) this.mLastNetworkUpdate)
          break;
        this.mLastNetworkUpdate = (float) iMsg.TimeStamp;
        Parker.UpdateMessage updateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &updateMessage);
        if (this.mAnimationController.AnimationClip != this.mAnimationClips[(int) updateMessage.Animation])
          this.mAnimationController.StartClip(this.mAnimationClips[(int) updateMessage.Animation], false);
        this.mAnimationController.Time = updateMessage.AnimationTime.ToSingle();
        this.mHitPoints = (float) updateMessage.Hitpoints;
        this.mAnimationController.Speed = updateMessage.AnimationSpeed.ToSingle();
        this.mMovementSpeed = updateMessage.Speed.ToSingle();
        if (!updateMessage.PositionUpdate)
          break;
        this.mTransform.Translation = updateMessage.Position;
        break;
      case 1:
        Parker.ChangeStageMessage changeStageMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStageMessage);
        if (changeStageMessage.NewStage == Parker.Stages.NrOfStages || changeStageMessage.NewStage == this.mCurrentStage)
          break;
        this.mStageStates[(int) this.mCurrentStage].OnExit(this);
        this.mCurrentStage = changeStageMessage.NewStage;
        this.mStageStates[(int) this.mCurrentStage].OnEnter(this);
        break;
      case 2:
        Parker.ChangeStateMessage changeStateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
        this.mStates[(int) this.mCurrentState].OnExit(this);
        this.mPreviousState = this.mCurrentState;
        this.mCurrentState = changeStateMessage.NewState;
        this.mStates[(int) this.mCurrentState].OnEnter(this);
        break;
      case 3:
        Parker.ChangeTargetMessage changeTargetMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeTargetMessage);
        this.mTarget = Magicka.GameLogic.Entities.Entity.GetFromHandle(changeTargetMessage.Handle) as Avatar;
        break;
      case 4:
        Parker.WebMessage webMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &webMessage);
        SprayEntity specificInstance = SprayEntity.GetSpecificInstance(webMessage.Handle);
        specificInstance.Initialize(webMessage.MouthIsOwner ? (Magicka.GameLogic.Entities.Entity) this.mMouthZone : (Magicka.GameLogic.Entities.Entity) null, webMessage.Position, webMessage.Direction.ToVector3(), webMessage.Velocity.ToSingle());
        AudioManager.Instance.PlayCue(Banks.Characters, Parker.WebState.WEB_SOUND, this.mMouthZone.AudioEmitter);
        if (webMessage.Parent != (ushort) 0)
          SprayEntity.GetSpecificInstance(webMessage.Parent).Child = specificInstance;
        this.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) specificInstance);
        break;
      case 5:
        Parker.RotationTransitionMessage transitionMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &transitionMessage);
        this.mRotationTransitionState = transitionMessage.State;
        this.mTargetRotateDirection = transitionMessage.Direction.ToVector3();
        break;
    }
  }

  public BossEnum GetBossType() => BossEnum.Parker;

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  public bool NetworkInitialized => true;

  private static bool CheckBehind(Parker iOwner)
  {
    Vector3 translation = iOwner.mTransform.Translation;
    Vector3 forward = iOwner.mTransform.Forward;
    for (int index = 0; index < iOwner.mPlayers.Length; ++index)
    {
      if (iOwner.mPlayers[index].Playing && iOwner.mPlayers[index].Avatar != null && !iOwner.mPlayers[index].Avatar.Dead)
      {
        Vector3 position = iOwner.mPlayers[index].Avatar.Position;
        Vector3 result1;
        Vector3.Subtract(ref position, ref translation, out result1);
        result1.Y = 0.0f;
        result1.Normalize();
        float result2;
        Vector3.Dot(ref result1, ref forward, out result2);
        if ((double) result2 < -0.85000002384185791)
          return true;
      }
    }
    return false;
  }

  private static float GetDistanceSquared(Parker iOwner, Magicka.GameLogic.Entities.Character iTarget)
  {
    Vector2 vector2_1 = new Vector2(iOwner.mTransform.Translation.X, iOwner.mTransform.Translation.Z);
    Vector2 vector2_2 = new Vector2(iTarget.Position.X, iTarget.Position.Z);
    float result;
    Vector2.DistanceSquared(ref vector2_1, ref vector2_2, out result);
    return result;
  }

  private static void GetAngle(
    ref Vector3 iPosition,
    ref Vector3 iForward,
    ref Vector3 iTarget,
    out float oAngle)
  {
    Vector3 result;
    Vector3.Subtract(ref iTarget, ref iPosition, out result);
    result.Y = 0.0f;
    result.Normalize();
    Parker.GetAngle(ref iForward, ref result, out oAngle);
  }

  private static void GetAngle(
    ref Vector3 iForward,
    ref Vector3 iTargetDirection,
    out float oAngle)
  {
    Vector3 result;
    Vector3.Cross(ref iTargetDirection, ref iForward, out result);
    float num = (float) System.Math.Acos((double) result.Y) - 1.57079637f;
    oAngle = num;
  }

  private static void GetConstrainedAngle(
    ref Vector3 iPosition,
    ref Vector3 iForward,
    ref Vector3 iTarget,
    out float oAngle,
    float iMinAngle,
    float iMaxAngle)
  {
    Vector3 result;
    Vector3.Subtract(ref iTarget, ref iPosition, out result);
    result.Y = 0.0f;
    result.Normalize();
    Parker.GetConstrainedAngle(ref iForward, ref result, out oAngle, iMinAngle, iMaxAngle);
  }

  private static void GetConstrainedAngle(
    ref Vector3 iForward,
    ref Vector3 iTargetDirection,
    out float oAngle,
    float iMinAngle,
    float iMaxAngle)
  {
    Vector3 iNormal = new Vector3(0.0f, 0.0f, 1f);
    MagickaMath.ConstrainVector(ref iTargetDirection, ref iNormal, iMinAngle, iMaxAngle);
    Parker.GetAngle(ref iForward, ref iTargetDirection, out oAngle);
  }

  private static unsafe void TransitionToCave(Parker iOwner)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Vector3 translation1 = iOwner.mTransform.Translation;
    Vector3 translation2 = iOwner.mSpawnTransform.Translation;
    Vector3.Subtract(ref translation1, ref translation2, out iOwner.mTargetRotateDirection);
    iOwner.mTargetRotateDirection.Y = 0.0f;
    iOwner.mTargetRotateDirection.Normalize();
    iOwner.mRotationTransitionState = Parker.States.MoveToCave;
    if (NetworkManager.Instance.State == NetworkState.Server)
      BossFight.Instance.SendMessage<Parker.RotationTransitionMessage>((IBoss) iOwner, (ushort) 5, (void*) &new Parker.RotationTransitionMessage()
      {
        Direction = new Normalized101010(iOwner.mTargetRotateDirection),
        State = iOwner.mRotationTransitionState
      }, true);
    iOwner.ChangeState(Parker.States.Rotate);
  }

  protected class IntroStage : IBossState<Parker>
  {
    public void OnEnter(Parker iOwner)
    {
      iOwner.mStageSpeedMod = 1f;
      iOwner.mTimeBetweenActions = 3f;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mCurrentState == Parker.States.Intro || iOwner.mCurrentState == Parker.States.Rotate || iOwner.mCurrentState == Parker.States.MoveToCave || iOwner.mCurrentState == Parker.States.MoveToBattle || iOwner.mCurrentState == Parker.States.EnterCave || iOwner.mCurrentState == Parker.States.LeaveCave)
        return;
      if (iOwner.mPlayersDead)
      {
        if (iOwner.mCurrentState == Parker.States.Cave)
          return;
        Parker.TransitionToCave(iOwner);
      }
      else if ((iOwner.mCurrentState == Parker.States.Battle || iOwner.mCurrentState == Parker.States.Swipe || iOwner.mCurrentState == Parker.States.Defend) && Parker.CheckBehind(iOwner))
        Parker.TransitionToCave(iOwner);
      else if ((double) iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.CallMinions);
      else if ((double) iOwner.GetSwipState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Swipe);
      else if ((double) iOwner.GetDefendState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Defend);
      else if ((double) iOwner.GetWebState.GetWeight(iOwner, iDeltaTime) > 0.0)
      {
        iOwner.ChangeState(Parker.States.Web);
      }
      else
      {
        if ((double) iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) <= 0.0)
          return;
        iOwner.ChangeState(Parker.States.Rush);
      }
    }

    public void OnExit(Parker iOwner)
    {
    }
  }

  protected class BattleStage : IBossState<Parker>
  {
    public void OnEnter(Parker iOwner)
    {
      iOwner.mStageSpeedMod = 1f;
      iOwner.mTimeBetweenActions = 2f;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mCurrentState == Parker.States.Rotate || iOwner.mCurrentState == Parker.States.CallMinions || iOwner.mCurrentState == Parker.States.MoveToCave || iOwner.mCurrentState == Parker.States.MoveToBattle || iOwner.mCurrentState == Parker.States.EnterCave || iOwner.mCurrentState == Parker.States.LeaveCave)
        return;
      if (iOwner.mPlayersDead)
      {
        if (iOwner.mCurrentState == Parker.States.Cave)
          return;
        Parker.TransitionToCave(iOwner);
      }
      else if ((iOwner.mCurrentState == Parker.States.Battle || iOwner.mCurrentState == Parker.States.Swipe || iOwner.mCurrentState == Parker.States.Defend) && Parker.CheckBehind(iOwner))
        Parker.TransitionToCave(iOwner);
      else if (iOwner.HasStatus(StatusEffects.Burning) && (double) iOwner.BurningDPS > 250.0)
        Parker.TransitionToCave(iOwner);
      else if ((double) iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.CallMinions);
      else if ((double) iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Rush);
      else if ((double) iOwner.GetSwipState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Swipe);
      else if ((double) iOwner.GetWebState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Web);
      else if ((double) iOwner.GetSpawnState.GetWeight(iOwner, iDeltaTime) > 0.0)
      {
        iOwner.ChangeState(Parker.States.Spawn);
      }
      else
      {
        if ((double) iOwner.GetDefendState.GetWeight(iOwner, iDeltaTime) <= 0.0)
          return;
        iOwner.ChangeState(Parker.States.Defend);
      }
    }

    public void OnExit(Parker iOwner)
    {
    }
  }

  protected class CriticalStage : IBossState<Parker>
  {
    public void OnEnter(Parker iOwner)
    {
      iOwner.mStageSpeedMod = 1.5f;
      iOwner.mTimeBetweenActions = 1f;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mCurrentState == Parker.States.Rotate || iOwner.mCurrentState == Parker.States.CallMinions || iOwner.mCurrentState == Parker.States.MoveToCave || iOwner.mCurrentState == Parker.States.MoveToBattle || iOwner.mCurrentState == Parker.States.EnterCave || iOwner.mCurrentState == Parker.States.LeaveCave)
        return;
      if (iOwner.mPlayersDead)
      {
        if (iOwner.mCurrentState == Parker.States.Cave)
          return;
        Parker.TransitionToCave(iOwner);
      }
      else if ((iOwner.mCurrentState == Parker.States.Battle || iOwner.mCurrentState == Parker.States.Swipe || iOwner.mCurrentState == Parker.States.Defend) && Parker.CheckBehind(iOwner))
        Parker.TransitionToCave(iOwner);
      else if (iOwner.HasStatus(StatusEffects.Burning) && (double) iOwner.BurningDPS > 250.0)
        Parker.TransitionToCave(iOwner);
      else if ((double) iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.CallMinions);
      else if ((double) iOwner.GetSwipState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Swipe);
      else if ((double) iOwner.GetSpawnState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Spawn);
      else if ((double) iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Rush);
      else if ((double) iOwner.GetWebState.GetWeight(iOwner, iDeltaTime) > 0.0)
      {
        iOwner.ChangeState(Parker.States.Web);
      }
      else
      {
        if ((double) iOwner.GetDefendState.GetWeight(iOwner, iDeltaTime) <= 0.0)
          return;
        iOwner.ChangeState(Parker.States.Defend);
      }
    }

    public void OnExit(Parker iOwner)
    {
    }
  }

  protected class FinalStage : IBossState<Parker>
  {
    public void OnEnter(Parker iOwner)
    {
      iOwner.mStageSpeedMod = 2f;
      iOwner.mTimeBetweenActions = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mCurrentState == Parker.States.Rotate || iOwner.mCurrentState == Parker.States.CallMinions || iOwner.mCurrentState == Parker.States.MoveToCave || iOwner.mCurrentState == Parker.States.MoveToBattle || iOwner.mCurrentState == Parker.States.EnterCave || iOwner.mCurrentState == Parker.States.LeaveCave)
        return;
      if (iOwner.mPlayersDead)
      {
        if (iOwner.mCurrentState == Parker.States.Cave)
          return;
        Parker.TransitionToCave(iOwner);
      }
      else if ((iOwner.mCurrentState == Parker.States.Battle || iOwner.mCurrentState == Parker.States.Swipe || iOwner.mCurrentState == Parker.States.Defend) && Parker.CheckBehind(iOwner))
        Parker.TransitionToCave(iOwner);
      else if (iOwner.mIsHit && iOwner.mCurrentState == Parker.States.Battle)
        Parker.TransitionToCave(iOwner);
      else if ((double) iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.CallMinions);
      else if ((double) iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Rush);
      else if ((double) iOwner.GetSwipState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Swipe);
      else if ((double) iOwner.GetWebState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.Web);
      else if ((double) iOwner.GetSpawnState.GetWeight(iOwner, iDeltaTime) > 0.0)
      {
        iOwner.ChangeState(Parker.States.Spawn);
      }
      else
      {
        if ((double) iOwner.GetDefendState.GetWeight(iOwner, iDeltaTime) <= 0.0)
          return;
        iOwner.ChangeState(Parker.States.Defend);
      }
    }

    public void OnExit(Parker iOwner)
    {
    }
  }

  protected class RenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
  {
    public float mFlash;
    public float mDamage;
    public Matrix[] mSkeleton;

    public RenderData(ModelMesh iMesh)
    {
      this.mSkeleton = new Matrix[80 /*0x50*/];
      Helper.SkinnedModelDeferredMaterialFromBasicEffect(iMesh.MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
    }

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      this.mMaterial.Damage = this.mDamage;
      SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
      modelDeferredEffect.Bones = this.mSkeleton;
      modelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.mFlash);
      base.Draw(iEffect, iViewFrustum);
      modelDeferredEffect.OverrideColor = new Vector4();
      modelDeferredEffect.Colorize = new Vector4();
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.mSkeleton;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }

  public enum MessageType : ushort
  {
    Update,
    ChangeStage,
    ChangeState,
    ChangeTarget,
    Web,
    RotationTransition,
  }

  internal struct UpdateMessage
  {
    public const ushort TYPE = 0;
    public byte Animation;
    public ushort Hitpoints;
    public HalfSingle AnimationSpeed;
    public HalfSingle AnimationTime;
    public HalfSingle Speed;
    public bool PositionUpdate;
    public Vector3 Position;
  }

  internal struct ChangeStageMessage
  {
    public const ushort TYPE = 1;
    public Parker.Stages NewStage;
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 2;
    public Parker.States NewState;
  }

  internal struct ChangeTargetMessage
  {
    public const ushort TYPE = 3;
    public int Handle;
  }

  internal struct WebMessage
  {
    public const ushort TYPE = 4;
    public ushort Handle;
    public ushort Parent;
    public Normalized101010 Direction;
    public Vector3 Position;
    public bool MouthIsOwner;
    public HalfSingle Velocity;
  }

  internal struct RotationTransitionMessage
  {
    public const ushort TYPE = 5;
    public Parker.States State;
    public Normalized101010 Direction;
  }

  public enum States
  {
    Intro,
    Battle,
    LeaveCave,
    EnterCave,
    Swipe,
    Bite,
    Rush,
    Backup,
    Stun,
    Web,
    Spawn,
    CallMinions,
    Defend,
    Rotate,
    MoveToBattle,
    MoveToCave,
    Cave,
    Dead,
    NrOfStates,
  }

  public enum Stages
  {
    Intro,
    Battle,
    Critical,
    Final,
    NrOfStages,
  }

  public enum Animations
  {
    walk_backward,
    walk_forward,
    walk_left,
    walk_right,
    idle,
    rotate_left,
    rotate_right,
    attack_left,
    attack_right,
    attack_front,
    bite,
    web,
    spawn,
    defend,
    stun,
    cave_enter,
    cave_help,
    cave_idle,
    cave_taunt,
    cave_exit,
    intro,
    taunt,
    die,
    NrOfAnimations,
  }

  private interface IParkerState : IBossState<Parker>
  {
    float GetWeight(Parker iOwner, float iDeltaTime);
  }

  protected class IntroState : Parker.IParkerState, IBossState<Parker>
  {
    public void OnEnter(Parker iOwner)
    {
      AudioManager.Instance.PlayCue(Banks.Additional, Parker.INTRO_SOUND, iOwner.mAbdomenZone.AudioEmitter);
      iOwner.mAnimationController.StartClip(iOwner.mAnimationClips[20], false);
      iOwner.mStateSpeedMod = 1f;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[20])
      {
        if (!iOwner.mAnimationController.HasFinished)
          return;
        iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[4], 0.4f, false);
      }
      else
        iOwner.ChangeState(Parker.States.Battle);
    }

    public void OnExit(Parker iOwner)
    {
    }

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class BattleState : Parker.IParkerState, IBossState<Parker>
  {
    public const float ANGLE = 0.7f;
    public const float THRESHOLD_ANGLE = 0.01f;
    private Parker.Animations mCurrentAnimation;

    public void OnEnter(Parker iOwner)
    {
      this.mCurrentAnimation = iOwner.mAnimationController.AnimationClip != iOwner.mAnimationClips[2] ? (iOwner.mAnimationController.AnimationClip != iOwner.mAnimationClips[3] ? (iOwner.mAnimationController.AnimationClip != iOwner.mAnimationClips[4] ? Parker.Animations.intro : Parker.Animations.idle) : Parker.Animations.walk_right) : Parker.Animations.walk_left;
      iOwner.mStateSpeedMod = 1f;
      iOwner.mAnimationSpeedMod = 1f;
      if (iOwner.mTarget != null && iOwner.mTarget.Dead)
        iOwner.mTarget = (Avatar) null;
      Avatar oAvatar1;
      if (iOwner.mTarget == null && iOwner.GetRandomTarget(out oAvatar1))
        iOwner.SetTarget(oAvatar1);
      if ((double) iOwner.GetRushState.GetWeight(iOwner, 0.0f) > 0.0)
        iOwner.ChangeState(Parker.States.Rush);
      else if ((double) iOwner.GetWebState.GetWeight(iOwner, 0.0f) > 0.0)
      {
        iOwner.ChangeState(Parker.States.Web);
      }
      else
      {
        Avatar oAvatar2;
        if (!iOwner.GetRandomTarget(out oAvatar2) || iOwner.mTarget == null || iOwner.mTargetWebbed || iOwner.mTarget.HasStatus(StatusEffects.Poisoned))
          return;
        iOwner.SetTarget(oAvatar2);
      }
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      iOwner.mStateSpeedMod = iOwner.mTarget == null || !iOwner.mTargetWebbed && !iOwner.mTarget.HasStatus(StatusEffects.Poisoned) ? 1f : 1.5f;
      Vector3 translation1 = iOwner.mPivotTransform.Translation;
      Vector3 forward1 = iOwner.mPivotTransform.Forward;
      Avatar oAvatar;
      if ((iOwner.mTarget == null || iOwner.mTarget.Dead) && iOwner.GetRandomTarget(out oAvatar) && oAvatar != null)
        iOwner.SetTarget(oAvatar);
      Vector3 playerTargetPosition = iOwner.mPlayerTargetPosition;
      Vector3 result1;
      Vector3.Subtract(ref playerTargetPosition, ref translation1, out result1);
      result1.Y = 0.0f;
      result1.Normalize();
      float oAngle;
      Parker.GetConstrainedAngle(ref forward1, ref result1, out oAngle, -0.7f, 0.7f);
      iOwner.mPivotTargetDirection = result1;
      if ((double) System.Math.Abs(oAngle) > 0.004999999888241291 && !iOwner.mAnimationController.CrossFadeEnabled)
      {
        float radians = MathHelper.ToRadians(iOwner.mMovementSpeed);
        Quaternion result2;
        Quaternion.CreateFromYawPitchRoll(2f * ((double) oAngle >= 0.0 ? System.Math.Min(oAngle, radians) : System.Math.Max(oAngle, -radians)) * iOwner.mStateSpeedMod * iOwner.mStageSpeedMod * iOwner.GetColdSpeed() * iDeltaTime, 0.0f, 0.0f, out result2);
        Vector3 forward2 = iOwner.mPivotTransform.Forward;
        Vector3.Transform(ref forward2, ref result2, out Vector3 _);
        Vector3 translation2 = iOwner.mPivotTransform.Translation;
        Matrix.Transform(ref iOwner.mPivotTransform, ref result2, out iOwner.mPivotTransform);
        iOwner.mPivotTransform.Translation = translation2;
      }
      iOwner.mTransform = iOwner.BattleStateTransform;
      float val1 = (float) (0.25 + (double) System.Math.Abs(oAngle) * 5.0);
      iOwner.mAnimationSpeedMod = System.Math.Min(System.Math.Max(val1, 0.0f), 1f);
      if ((double) oAngle > 0.0099999997764825821)
      {
        if (this.mCurrentAnimation == Parker.Animations.walk_left)
          return;
        iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[2], 0.2f, true);
        this.mCurrentAnimation = Parker.Animations.walk_left;
      }
      else if ((double) oAngle < -0.0099999997764825821)
      {
        if (this.mCurrentAnimation == Parker.Animations.walk_right)
          return;
        iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[3], 0.2f, true);
        this.mCurrentAnimation = Parker.Animations.walk_right;
      }
      else
      {
        iOwner.mAnimationSpeedMod = 1f;
        if (this.mCurrentAnimation == Parker.Animations.idle)
          return;
        iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[4], 0.2f, true);
        this.mCurrentAnimation = Parker.Animations.idle;
      }
    }

    public void OnExit(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1f;
      iOwner.mAnimationSpeedMod = 1f;
    }

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class DefendState : Parker.IParkerState, IBossState<Parker>
  {
    private const float DEFEND_SOUND_TIME = 0.15384616f;
    private bool mPlayedSound;

    public void OnEnter(Parker iOwner)
    {
      this.mPlayedSound = false;
      iOwner.mStateSpeedMod = 1.5f;
      iOwner.Movement = Vector3.Zero;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[13], 0.15f, false);
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.HasFinished)
      {
        if (iOwner.mPreviousState == Parker.States.Spawn)
          iOwner.ChangeState(Parker.States.Battle);
        else
          iOwner.ChangeState(iOwner.mPreviousState);
      }
      else
      {
        if (this.mPlayedSound)
          return;
        this.mPlayedSound = true;
        AudioManager.Instance.PlayCue(Banks.Additional, Parker.DEFEND_SOUND, iOwner.mMouthZone.AudioEmitter);
      }
    }

    public void OnExit(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1f;
      iOwner.mIsHit = false;
      iOwner.mHitTimer = 8f;
    }

    public float GetWeight(Parker iOwner, float iDeltaTime)
    {
      if (iOwner.mCurrentState == Parker.States.Defend || iOwner.mCurrentState == Parker.States.Spawn || iOwner.mCurrentState == Parker.States.CallMinions)
        return 0.0f;
      if (iOwner.mCurrentState == Parker.States.Battle && iOwner.mIsHit)
      {
        if (iOwner.mCurrentStage == Parker.Stages.Intro || iOwner.mCurrentStage != Parker.Stages.Intro && !iOwner.mTargetWebbed)
          return 1f;
        iOwner.mIsHit = false;
        return 0.0f;
      }
      iOwner.mIsHit = false;
      return 0.0f;
    }
  }

  protected class StunState : Parker.IParkerState, IBossState<Parker>
  {
    public void OnEnter(Parker iOwner)
    {
      iOwner.Movement = new Vector3();
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[14], 0.25f, false);
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled || !iOwner.mAnimationController.HasFinished)
        return;
      iOwner.ChangeState(iOwner.mPreviousState);
    }

    public void OnExit(Parker iOwner) => iOwner.mStateSpeedMod = 1f;

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class SwipeState : Parker.IParkerState, IBossState<Parker>
  {
    internal bool mPlayedSound;
    internal Parker.SwipeState.Directions mSwipeType = Parker.SwipeState.Directions.Front;
    private DamageCollection5 mSwipeDamage;
    private float mWeightTimer;

    public SwipeState()
    {
      this.mSwipeDamage = new DamageCollection5();
      this.mSwipeDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, 100f, 3f));
      this.mSwipeDamage.AddDamage(new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth, 50f, 1f));
    }

    public void OnEnter(Parker iOwner)
    {
      iOwner.Movement = Vector3.Zero;
      switch (this.mSwipeType)
      {
        case Parker.SwipeState.Directions.Left:
          iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[7], 0.25f, false);
          break;
        case Parker.SwipeState.Directions.Right:
          iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[8], 0.25f, false);
          break;
        case Parker.SwipeState.Directions.Front:
          iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[9], 0.25f, false);
          break;
      }
      this.mPlayedSound = false;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.ChangeState(iOwner.mPreviousState);
      }
      else
      {
        float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if (this.mSwipeType == Parker.SwipeState.Directions.Front)
        {
          if (this.mPlayedSound || (double) num1 < 0.61290323734283447)
            return;
          AudioManager.Instance.PlayCue(Banks.Additional, Parker.BITE_SOUND, iOwner.mAbdomenZone.AudioEmitter);
          this.mPlayedSound = true;
        }
        else
        {
          if ((double) num1 < (double) Parker.ATTACK_LEFT_RIGHT_TIME[0] || (double) num1 > (double) Parker.ATTACK_LEFT_RIGHT_TIME[1])
            return;
          Segment segment = new Segment();
          Vector3 result;
          float num2;
          if (this.mSwipeType == Parker.SwipeState.Directions.Left)
          {
            Vector3 translation1 = iOwner.mFootTransform[2].Translation;
            Vector3 translation2 = iOwner.mFootTransform[1].Translation;
            Vector3.Subtract(ref translation2, ref translation1, out segment.Delta);
            segment.Origin = translation1;
            Vector3.Multiply(ref segment.Delta, 0.5f, out result);
            num2 = result.Length();
            Vector3.Add(ref result, ref translation1, out result);
          }
          else
          {
            Vector3 translation3 = iOwner.mFootTransform[6].Translation;
            Vector3 translation4 = iOwner.mFootTransform[5].Translation;
            Vector3.Subtract(ref translation4, ref translation3, out segment.Delta);
            segment.Origin = translation3;
            Vector3.Multiply(ref segment.Delta, 0.5f, out result);
            num2 = result.Length();
            Vector3.Add(ref result, ref translation3, out result);
          }
          List<Magicka.GameLogic.Entities.Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(result, num2 * 1.25f, true, false);
          entities.Remove((Magicka.GameLogic.Entities.Entity) iOwner.mAbdomenZone);
          for (int index = 0; index < entities.Count; ++index)
          {
            if (entities[index] is IDamageable damageable && !iOwner.mHitList.Contains(damageable))
            {
              this.mSwipeDamage.A.Amount = damageable.Body.Mass;
              int num3 = (int) damageable.Damage(this.mSwipeDamage, (Magicka.GameLogic.Entities.Entity) iOwner.mAbdomenZone, iOwner.mPlayState.PlayTime, iOwner.mTransform.Translation);
              iOwner.mHitList.Add(damageable);
              if (!this.mPlayedSound)
              {
                AudioManager.Instance.PlayCue(Banks.Additional, Parker.SWIPE_SIDES_SOUND, iOwner.mAbdomenZone.AudioEmitter);
                this.mPlayedSound = true;
              }
            }
          }
          iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
        }
      }
    }

    public void OnExit(Parker iOwner) => iOwner.mStateSpeedMod = 1f;

    public float GetWeight(Parker iOwner, float iDeltaTime)
    {
      if (iOwner.mCurrentState == Parker.States.Swipe || iOwner.mCurrentState != Parker.States.Battle)
        return 0.0f;
      this.mWeightTimer -= iDeltaTime;
      if ((double) this.mWeightTimer > 0.0)
        return 0.0f;
      this.mWeightTimer = 0.5f;
      Vector3 translation = iOwner.mTransform.Translation;
      Vector3 forward = iOwner.mTransform.Forward;
      for (int index = 0; index < iOwner.mPlayers.Length; ++index)
      {
        if (iOwner.mPlayers[index].Playing && iOwner.mPlayers[index].Avatar != null && !iOwner.mPlayers[index].Avatar.Dead && (double) Parker.GetDistanceSquared(iOwner, (Magicka.GameLogic.Entities.Character) iOwner.mPlayers[index].Avatar) < 49.0)
        {
          Vector3 position = iOwner.mPlayers[index].Avatar.Position;
          float oAngle;
          Parker.GetAngle(ref translation, ref forward, ref position, out oAngle);
          if ((double) System.Math.Abs(oAngle) <= 2.5132741928100586)
          {
            iOwner.SetTarget(iOwner.mPlayers[index].Avatar);
            if ((double) System.Math.Abs(oAngle) <= 0.47123894095420837)
            {
              this.mSwipeType = Parker.SwipeState.Directions.Front;
              return 1f;
            }
            if ((double) oAngle > -0.47123894095420837)
            {
              this.mSwipeType = Parker.SwipeState.Directions.Left;
              return 1f;
            }
            if ((double) oAngle < 0.47123894095420837)
            {
              this.mSwipeType = Parker.SwipeState.Directions.Right;
              return 1f;
            }
          }
        }
      }
      return 0.0f;
    }

    public enum Directions
    {
      Left,
      Right,
      Front,
    }
  }

  protected class RushState : Parker.IParkerState, IBossState<Parker>
  {
    private Vector3 mTargetPosition;
    private float mBlendTime;

    public void OnEnter(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1f;
      iOwner.Movement = Vector3.Zero;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[1], 0.5f, true);
      if (iOwner.mTarget != null)
      {
        this.mTargetPosition = iOwner.mTarget.Position;
      }
      else
      {
        this.mTargetPosition = iOwner.mTransform.Translation;
        Vector3 result = iOwner.mTransform.Forward;
        Vector3.Multiply(ref result, 13f, out result);
        Vector3.Add(ref this.mTargetPosition, ref result, out this.mTargetPosition);
      }
      this.mBlendTime = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      Vector3 mTargetPosition = this.mTargetPosition;
      Vector3 translation = iOwner.mMouthOrientation.Translation with
      {
        Y = mTargetPosition.Y = 0.0f
      };
      iOwner.mMovementSpeedMod = iOwner.mAnimationController.CrossFadeEnabled ? 0.0f : (iOwner.mTarget == null || !iOwner.mTarget.HasStatus(StatusEffects.Poisoned) ? 1f : 1.5f);
      this.mBlendTime -= iDeltaTime;
      if (MagickaMath.IsApproximately(ref translation, ref mTargetPosition, 1f))
      {
        iOwner.Movement = Vector3.Zero;
        iOwner.ChangeState(Parker.States.Bite);
      }
      else
      {
        Vector3 result;
        Vector3.Subtract(ref mTargetPosition, ref translation, out result);
        float val1 = result.LengthSquared();
        result.Normalize();
        iOwner.mStateSpeedMod = (float) (0.5 + (double) System.Math.Min(val1, 1f) * 0.5);
        iOwner.Movement = result;
      }
    }

    public void OnExit(Parker iOwner)
    {
      iOwner.mMovementSpeedMod = 1f;
      iOwner.mStateSpeedMod = 1f;
      iOwner.Movement = Vector3.Zero;
    }

    public float GetWeight(Parker iOwner, float iDeltaTime)
    {
      if (iOwner.mTarget == null || iOwner.mCurrentState != Parker.States.Battle)
        return 0.0f;
      Vector3 translation = iOwner.mTransform.Translation;
      Vector3 forward = iOwner.mTransform.Forward;
      Vector3 position = iOwner.mTarget.Position;
      if ((iOwner.mCurrentStage == Parker.Stages.Intro || iOwner.mCurrentStage == Parker.Stages.Battle) && iOwner.mTargetWebbed)
      {
        float oAngle;
        Parker.GetAngle(ref translation, ref forward, ref position, out oAngle);
        if ((double) System.Math.Abs(oAngle) > 0.062831856310367584)
          return 0.0f;
      }
      else if (iOwner.mCurrentStage == Parker.Stages.Final)
      {
        float oAngle;
        Parker.GetAngle(ref translation, ref forward, ref position, out oAngle);
        if ((double) System.Math.Abs(oAngle) > 0.047123890370130539)
          return 0.0f;
      }
      else
      {
        if (!iOwner.mTarget.HasStatus(StatusEffects.Poisoned) && !iOwner.mTargetWebbed)
          return 0.0f;
        float oAngle;
        Parker.GetAngle(ref translation, ref forward, ref position, out oAngle);
        if ((double) System.Math.Abs(oAngle) > 0.047123890370130539)
          return 0.0f;
      }
      Segment iSeg = new Segment();
      iSeg.Origin = translation;
      Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
      List<Shield> shields = iOwner.mPlayState.EntityManager.Shields;
      for (int index = 0; index < shields.Count; ++index)
      {
        if (shields[index].SegmentIntersect(out Vector3 _, iSeg, 0.0f))
          return 0.0f;
      }
      return 1f;
    }
  }

  protected class BackupState : Parker.IParkerState, IBossState<Parker>
  {
    private Vector3 mTargetPosition;

    public void OnEnter(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1f;
      this.mTargetPosition = iOwner.BattleStateTransform.Translation;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[0], 0.2f, true);
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      Vector3 mTargetPosition = this.mTargetPosition;
      Vector3 translation = iOwner.mTransform.Translation with
      {
        Y = mTargetPosition.Y = 0.0f
      };
      if (MagickaMath.IsApproximately(ref translation, ref mTargetPosition, 0.01f))
      {
        iOwner.Movement = Vector3.Zero;
        if (NetworkManager.Instance.State == NetworkState.Client)
          return;
        iOwner.ChangeState(Parker.States.Battle);
      }
      else
      {
        Vector3 result;
        Vector3.Subtract(ref mTargetPosition, ref translation, out result);
        float val1 = result.LengthSquared();
        result.Normalize();
        iOwner.mStateSpeedMod = (float) (0.5 + (double) System.Math.Min(val1, 1f) * 0.5);
        iOwner.Movement = result;
      }
    }

    public void OnExit(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1f;
      iOwner.Movement = Vector3.Zero;
    }

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class BiteState : Parker.IParkerState, IBossState<Parker>
  {
    internal const float BITE_SOUND_TIME = 0.612903237f;
    private bool mPlayedSound;

    public void OnEnter(Parker iOwner)
    {
      this.mPlayedSound = false;
      iOwner.mStateSpeedMod = 1.5f;
      iOwner.Movement = Vector3.Zero;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[10], 0.25f, false);
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (iOwner.mAnimationController.HasFinished)
      {
        if ((double) iOwner.GetRushState.GetWeight(iOwner, iDeltaTime) > 0.0)
          iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[10], 0.25f, false);
        else
          iOwner.ChangeState(Parker.States.Backup);
      }
      else
      {
        if (this.mPlayedSound || (double) num < 0.61290323734283447)
          return;
        AudioManager.Instance.PlayCue(Banks.Additional, Parker.BITE_SOUND, iOwner.mAbdomenZone.AudioEmitter);
        this.mPlayedSound = true;
      }
    }

    public void OnExit(Parker iOwner) => iOwner.mStateSpeedMod = 1f;

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class WebState : Parker.IParkerState, IBossState<Parker>
  {
    public const int SEGMENTS = 10;
    private const float SEGMENTS_DIVISOR = 0.1f;
    internal static readonly int WEB_SOUND = "chr_spider_web".GetHashCodeCustom();
    internal static readonly int PRE_SOUND = "chr_spider_web_pre".GetHashCodeCustom();
    private static readonly float WEB_TIME = Parker.WEBBING_TIME[1] - Parker.WEBBING_TIME[0];
    private SprayEntity mEntity;
    private float mTimer;
    private float mSprayTime;
    private int mCount;
    private float mWeightTimer;

    public void OnEnter(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1.75f;
      iOwner.Movement = Vector3.Zero;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[11], 0.75f, false);
      this.mEntity = (SprayEntity) null;
      this.mTimer = 0.0f;
      this.mCount = 0;
      this.mSprayTime = (float) ((double) iOwner.mAnimationClips[11].Duration * (double) Parker.WebState.WEB_TIME * 1.0 / ((double) iOwner.mStateSpeedMod * (double) iOwner.mStageSpeedMod * (double) iOwner.mAnimationSpeedMod));
      AudioManager.Instance.PlayCue(Banks.Characters, Parker.WebState.PRE_SOUND, iOwner.mMouthZone.AudioEmitter);
    }

    public unsafe void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      this.mTimer -= iDeltaTime;
      if (iOwner.mAnimationController.HasFinished)
      {
        if ((double) iOwner.GetRushState.GetWeight(iOwner, 0.0f) > 0.0)
          iOwner.ChangeState(Parker.States.Rush);
        else
          iOwner.ChangeState(Parker.States.Battle);
      }
      else
      {
        if (!((double) this.mTimer <= 0.0 & this.mCount < 10 & (double) num1 >= (double) Parker.WEBBING_TIME[0] & (double) num1 <= (double) Parker.WEBBING_TIME[1]))
          return;
        Vector3 translation1 = iOwner.mMouthOrientation.Translation;
        Vector3 playerTargetPosition = iOwner.mPlayerTargetPosition;
        Vector3 result;
        Vector3.Subtract(ref playerTargetPosition, ref translation1, out result);
        float num2 = result.Length();
        result = iOwner.mMouthOrientation.Backward with
        {
          Y = 0.0f
        };
        result.Normalize();
        Vector3 forward = iOwner.mTransform.Forward;
        MagickaMath.ConstrainVector(ref result, ref forward, -0.65f, 0.65f);
        SprayEntity instance = SprayEntity.GetInstance();
        Vector3 translation2 = iOwner.mMouthOrientation.Translation;
        ++this.mCount;
        instance.Initialize(this.mCount == 10 ? (Magicka.GameLogic.Entities.Entity) null : (Magicka.GameLogic.Entities.Entity) iOwner.mMouthZone, translation2, result, num2 * 3f);
        if (this.mEntity != null)
          this.mEntity.Child = instance;
        AudioManager.Instance.PlayCue(Banks.Characters, Parker.WebState.WEB_SOUND, iOwner.mMouthZone.AudioEmitter);
        if (NetworkManager.Instance.State == NetworkState.Server)
          BossFight.Instance.SendMessage<Parker.WebMessage>((IBoss) iOwner, (ushort) 4, (void*) &new Parker.WebMessage()
          {
            Handle = instance.Handle,
            Parent = (this.mEntity == null ? (ushort) 0 : this.mEntity.Handle),
            Position = translation2,
            Direction = new Normalized101010(result),
            Velocity = new HalfSingle(num2 * 3f),
            MouthIsOwner = (this.mCount != 10)
          }, true);
        this.mEntity = instance;
        iOwner.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) instance);
        this.mTimer = 0.1f * this.mSprayTime;
      }
    }

    public void OnExit(Parker iOwner)
    {
      this.mWeightTimer = iOwner.mTimeBetweenActions;
      iOwner.mStateSpeedMod = 1f;
    }

    public float GetWeight(Parker iOwner, float iDeltaTime)
    {
      if (iOwner.mCurrentState == Parker.States.Web)
        return 0.0f;
      this.mWeightTimer -= iDeltaTime;
      if ((double) this.mWeightTimer > 0.0 || iOwner.mCurrentState != Parker.States.Battle || iOwner.mTarget == null || iOwner.mTarget.Dead || iOwner.mTarget.IsEntangled)
        return 0.0f;
      Vector3 translation = iOwner.mTransform.Translation;
      Vector3 forward = iOwner.mTransform.Forward;
      Vector3 position = iOwner.mTarget.Position;
      float oAngle;
      Parker.GetAngle(ref translation, ref forward, ref position, out oAngle);
      return (double) System.Math.Abs(oAngle) < 0.15707963705062866 ? 1f : 0.0f;
    }
  }

  protected class SpawnState : Parker.IParkerState, IBossState<Parker>
  {
    public const int COUNT = 6;
    private static readonly float SPAWNING_TIME = Parker.SPAWN_TIME[1] - Parker.SPAWN_TIME[0];
    private static readonly int SPIDERLING_HASH = "spider_baby".GetHashCode();
    private float mTimer;
    private float mSpawnTime;
    private int mCount;
    private bool mPlayedSound;
    private float mWeightTimer;
    private static readonly int ANY = "any".GetHashCodeCustom();

    public void OnEnter(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1f;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[12], 0.25f, false);
      this.mTimer = 0.0f;
      this.mCount = 0;
      this.mSpawnTime = (float) ((double) iOwner.mAnimationClips[11].Duration * (double) Parker.SpawnState.SPAWNING_TIME * 1.0 / (6.0 * (double) iOwner.mStageSpeedMod * (double) iOwner.mStateSpeedMod * (double) iOwner.GetColdSpeed()));
      iOwner.Movement = Vector3.Zero;
      this.mPlayedSound = false;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.ChangeState(Parker.States.Battle);
      }
      else
      {
        if (!this.mPlayedSound)
        {
          AudioManager.Instance.PlayCue(Banks.Additional, Parker.SPAWN_SOUND, iOwner.mAbdomenZone.AudioEmitter);
          this.mPlayedSound = true;
        }
        float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        this.mTimer -= iDeltaTime;
        if (!((double) num1 >= (double) Parker.SPAWN_TIME[0] & (double) num1 <= (double) Parker.SPAWN_TIME[1] & (double) this.mTimer <= 0.0 & this.mCount < 6))
          return;
        Vector3 mAbdomPosition = iOwner.mAbdomPosition;
        mAbdomPosition.Z -= 4f;
        float num2 = (float) Parker.RANDOM.NextDouble() * 2f;
        float num3 = (float) Parker.RANDOM.NextDouble() * 6.28318548f;
        float num4 = num2 * (float) System.Math.Cos((double) num3);
        float num5 = num2 * (float) System.Math.Sin((double) num3);
        mAbdomPosition.X += num4;
        mAbdomPosition.Z += num5;
        CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(Parker.SpawnState.SPIDERLING_HASH);
        NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iOwner.mPlayState);
        instance.Initialize(cachedTemplate, mAbdomPosition, 0, 2f);
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
            Point1 = 170,
            Point2 = 170
          });
        ++this.mCount;
      }
    }

    public void OnExit(Parker iOwner) => this.mWeightTimer = 20f;

    public float GetWeight(Parker iOwner, float iDeltaTime)
    {
      if (iOwner.mCurrentState == Parker.States.Spawn)
        return 0.0f;
      this.mWeightTimer -= iDeltaTime;
      if ((double) this.mWeightTimer > 0.0 || iOwner.mCurrentState != Parker.States.Battle || iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Parker.SpawnState.ANY).GetCount(Parker.SpawnState.SPIDERLING_HASH) >= 6)
        return 0.0f;
      for (int index = 0; index < iOwner.mPlayers.Length; ++index)
      {
        if (iOwner.mPlayers[index].Playing && iOwner.mPlayers[index].Avatar != null && !iOwner.mPlayers[index].Avatar.Dead && (double) Parker.GetDistanceSquared(iOwner, (Magicka.GameLogic.Entities.Character) iOwner.mPlayers[index].Avatar) < 64.0)
          return 0.0f;
      }
      this.mWeightTimer = 20f;
      return 1f;
    }
  }

  protected class RotateState : Parker.IParkerState, IBossState<Parker>
  {
    private const float THRESHOLD = 0.001f;

    public void OnEnter(Parker iOwner)
    {
      Vector3 forward = iOwner.mTransform.Forward;
      Vector3 targetRotateDirection = iOwner.mTargetRotateDirection;
      float oAngle;
      Parker.GetAngle(ref forward, ref targetRotateDirection, out oAngle);
      if ((double) System.Math.Abs(oAngle) < 0.05000000074505806)
        iOwner.ChangeState(iOwner.mRotationTransitionState);
      else if ((double) oAngle > 0.0)
        iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[5], 0.25f, true);
      else
        iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[6], 0.25f, true);
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      iOwner.Movement = new Vector3();
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      Vector3 forward = iOwner.mTransform.Forward;
      Vector3 targetRotateDirection = iOwner.mTargetRotateDirection;
      float oAngle;
      Parker.GetAngle(ref forward, ref targetRotateDirection, out oAngle);
      if ((double) System.Math.Abs(oAngle) < 0.05000000074505806)
        iOwner.ChangeState(iOwner.mRotationTransitionState);
      float radians = MathHelper.ToRadians(60f * iOwner.mStageSpeedMod * iOwner.mStateSpeedMod);
      Quaternion result;
      Quaternion.CreateFromYawPitchRoll(((double) oAngle >= 0.0 ? radians : -radians) * iDeltaTime, 0.0f, 0.0f, out result);
      Vector3.Transform(ref forward, ref result, out Vector3 _);
      Vector3 translation = iOwner.mTransform.Translation;
      Matrix.Transform(ref iOwner.mTransform, ref result, out iOwner.mTransform);
      iOwner.mTransform.Translation = translation;
    }

    public void OnExit(Parker iOwner) => iOwner.mStateSpeedMod = 1f;

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class MoveToCaveState : Parker.IParkerState, IBossState<Parker>
  {
    public void OnEnter(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1f;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[0], 0.25f, true);
    }

    public unsafe void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      Vector3 translation1 = iOwner.mTransform.Translation;
      Vector3 translation2 = iOwner.mSpawnTransform.Translation;
      translation1.Y = translation2.Y = 0.0f;
      iOwner.mMovementSpeedMod = iOwner.mAnimationController.CrossFadeEnabled ? 0.0f : 1f;
      if (!MagickaMath.IsApproximately(ref translation1, ref translation2, 0.01f))
      {
        Vector3 result;
        Vector3.Subtract(ref translation2, ref translation1, out result);
        float num = System.Math.Min(System.Math.Max(result.Length(), 0.25f), 1f);
        iOwner.mStateSpeedMod = num;
        result.Normalize();
        iOwner.Movement = result;
      }
      else
      {
        if (NetworkManager.Instance.State == NetworkState.Client)
          return;
        iOwner.mTargetRotateDirection = iOwner.mSpawnTransform.Forward;
        iOwner.mRotationTransitionState = Parker.States.EnterCave;
        if (NetworkManager.Instance.State == NetworkState.Server)
          BossFight.Instance.SendMessage<Parker.RotationTransitionMessage>((IBoss) iOwner, (ushort) 5, (void*) &new Parker.RotationTransitionMessage()
          {
            Direction = new Normalized101010(iOwner.mTargetRotateDirection),
            State = iOwner.mRotationTransitionState
          }, true);
        iOwner.ChangeState(Parker.States.Rotate);
      }
    }

    public void OnExit(Parker iOwner)
    {
      iOwner.mMovementSpeedMod = 1f;
      iOwner.mAnimationSpeedMod = 1f;
      iOwner.mStateSpeedMod = 1f;
      iOwner.Movement = Vector3.Zero;
    }

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class LeaveCaveState : Parker.IParkerState, IBossState<Parker>
  {
    private bool mPlaySound;

    public void OnEnter(Parker iOwner)
    {
      this.mPlaySound = false;
      iOwner.mStateSpeedMod = 1f;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[19], 0.5f, false);
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (!this.mPlaySound)
      {
        AudioManager.Instance.PlayCue(Banks.Additional, Parker.EXIT_CAVE_SOUND, iOwner.mAbdomenZone.AudioEmitter);
        this.mPlaySound = true;
      }
      else
      {
        if (!iOwner.mAnimationController.HasFinished)
          return;
        iOwner.ChangeState(Parker.States.MoveToBattle);
      }
    }

    public void OnExit(Parker iOwner)
    {
    }

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class MoveToBattleState : Parker.IParkerState, IBossState<Parker>
  {
    public void OnEnter(Parker iOwner)
    {
      iOwner.ResetBattleTransform();
      iOwner.mStateSpeedMod = 1f;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[1], 0.2f, true);
      iOwner.mMovementSpeedMod = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
      {
        iOwner.mStateSpeedMod = 1f;
        iOwner.Movement = Vector3.Zero;
      }
      else if (iOwner.mAnimationController.AnimationClip == iOwner.mAnimationClips[1])
      {
        Vector3 translation1 = iOwner.mTransform.Translation;
        Vector3 translation2 = iOwner.mBattleTransform.Translation;
        translation1.Y = 0.0f;
        translation2.Y = 0.0f;
        iOwner.mMovementSpeedMod = iOwner.mAnimationController.CrossFadeEnabled ? 0.0f : 1f;
        if (!MagickaMath.IsApproximately(ref translation1, ref translation2, 0.01f))
        {
          Vector3 result;
          Vector3.Subtract(ref translation2, ref translation1, out result);
          float num = System.Math.Min(System.Math.Max(result.Length(), 0.25f), 1f);
          iOwner.mStateSpeedMod = num;
          result.Normalize();
          iOwner.Movement = result;
        }
        else
          iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[4], 0.4f, false);
      }
      else
        iOwner.ChangeState(Parker.States.Battle);
    }

    public void OnExit(Parker iOwner)
    {
      iOwner.mMovementSpeedMod = 1f;
      iOwner.mAnimationSpeedMod = 1f;
      iOwner.mStateSpeedMod = 1f;
      iOwner.Movement = Vector3.Zero;
    }

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class EnterCaveState : Parker.IParkerState, IBossState<Parker>
  {
    private bool mPlayedSound;

    public void OnEnter(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1f;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[15], 0.5f, false);
      this.mPlayedSound = false;
      for (int index = 0; index < iOwner.GetStatusEffects().Length; ++index)
        iOwner.GetStatusEffects()[index].Stop();
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (!this.mPlayedSound)
      {
        AudioManager.Instance.PlayCue(Banks.Additional, Parker.ENTER_CAVE_SOUND, iOwner.mAbdomenZone.AudioEmitter);
        this.mPlayedSound = true;
      }
      else
      {
        if (!iOwner.mAnimationController.HasFinished)
          return;
        iOwner.ChangeState(Parker.States.Cave);
      }
    }

    public void OnExit(Parker iOwner) => iOwner.mStateSpeedMod = 1f;

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class CaveState : Parker.IParkerState, IBossState<Parker>
  {
    private float mModifier;
    private float mTimer;

    public float Timer => this.mTimer;

    public void OnEnter(Parker iOwner)
    {
      iOwner.mStateSpeedMod = 1f;
      iOwner.Movement = Vector3.Zero;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[17], 0.25f, true);
      this.mModifier = (float) (1.0 + (3.0 - (double) iOwner.mTimeBetweenActions));
      this.mTimer = this.mModifier;
      for (int index = 0; index < iOwner.GetStatusEffects().Length; ++index)
        iOwner.GetStatusEffects()[index].Stop();
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (!iOwner.mPlayersDead)
        this.mTimer -= iDeltaTime;
      iOwner.Damage(iDeltaTime * -425f * this.mModifier, Elements.Life);
      if ((double) this.mTimer > 0.0)
        return;
      if ((double) iOwner.GetCallMinionState.GetWeight(iOwner, iDeltaTime) > 0.0)
        iOwner.ChangeState(Parker.States.CallMinions);
      else
        iOwner.ChangeState(Parker.States.LeaveCave);
    }

    public void OnExit(Parker iOwner)
    {
    }

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }

  protected class CallMinionState : Parker.IParkerState, IBossState<Parker>
  {
    private static readonly float CALL_TIME = Parker.MINION_TIME[1] - Parker.MINION_TIME[0];
    private static readonly int FOREST_HASH = "spider_forest".GetHashCodeCustom();
    private static readonly int POISON_HASH = "spider_poison".GetHashCodeCustom();
    private float mTimer;
    private float mSpawnTime;
    private int mCount;
    private int mNrOfSpawns;
    private static readonly int[] SPAWNS = new int[4]
    {
      "boss_start0".GetHashCodeCustom(),
      "boss_start1".GetHashCodeCustom(),
      "boss_start2".GetHashCodeCustom(),
      "boss_start3".GetHashCodeCustom()
    };
    private bool mPlayedSound;
    private float mWeightTimer;
    private static readonly int ANY = "any".GetHashCodeCustom();

    public int Count
    {
      get
      {
        if (this.mNrOfSpawns == 0)
          this.mNrOfSpawns = 3 + (Magicka.Game.Instance.PlayerCount - 1);
        return this.mNrOfSpawns;
      }
    }

    public void OnEnter(Parker iOwner)
    {
      iOwner.Movement = new Vector3();
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[16 /*0x10*/], 0.25f, false);
      this.mSpawnTime = (float) ((double) iOwner.mAnimationClips[15].Duration * (double) Parker.CallMinionState.CALL_TIME * 1.0 / ((double) this.mNrOfSpawns * (double) iOwner.mStageSpeedMod * (double) iOwner.mStateSpeedMod * (double) iOwner.GetColdSpeed()));
      this.mCount = 0;
      this.mTimer = 0.0f;
      this.mPlayedSound = false;
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.ChangeState(Parker.States.LeaveCave);
      }
      else
      {
        if (!this.mPlayedSound)
        {
          AudioManager.Instance.PlayCue(Banks.Additional, Parker.CALL_MINION_SOUND, iOwner.mAbdomenZone.AudioEmitter);
          this.mPlayedSound = true;
        }
        float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        this.mTimer -= iDeltaTime;
        if (!((double) num1 >= (double) Parker.SPAWN_TIME[0] & (double) num1 <= (double) Parker.SPAWN_TIME[1] & (double) this.mTimer <= 0.0 & this.mCount < this.mNrOfSpawns))
          return;
        NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iOwner.mPlayState);
        Vector3 translation = iOwner.mTransform.Translation;
        Avatar oAvatar;
        Vector3 iPoint;
        if (iOwner.GetRandomTarget(out oAvatar))
        {
          iPoint = oAvatar.Position;
        }
        else
        {
          Matrix oLocator;
          iOwner.mPlayState.Level.CurrentScene.GetLocator(Parker.CallMinionState.SPAWNS[Parker.RANDOM.Next(Parker.CallMinionState.SPAWNS.Length)], out oLocator);
          iPoint = oLocator.Translation;
        }
        float num2 = (float) Parker.RANDOM.NextDouble() * 6.28318548f;
        float num3 = (float) System.Math.Sqrt(Parker.RANDOM.NextDouble());
        CharacterTemplate cachedTemplate;
        if (this.mCount < 2)
        {
          cachedTemplate = CharacterTemplate.GetCachedTemplate(Parker.CallMinionState.POISON_HASH);
          float num4 = (float) (4.0 + (double) num3 * 3.0);
          float num5 = num4 * (float) System.Math.Cos((double) num2);
          float num6 = num4 * (float) System.Math.Sin((double) num2);
          iPoint.X += num5;
          iPoint.Z += num6;
        }
        else
        {
          cachedTemplate = CharacterTemplate.GetCachedTemplate(Parker.CallMinionState.FOREST_HASH);
          float num7 = (float) (2.0 + (double) num3 * 2.0);
          float num8 = num7 * (float) System.Math.Cos((double) num2);
          float num9 = num7 * (float) System.Math.Sin((double) num2);
          iPoint.X += num8;
          iPoint.Z += num9;
        }
        Vector3 oPoint;
        double nearestPosition = (double) iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPoint, out oPoint, MovementProperties.Default);
        iPoint = oPoint;
        instance.Initialize(cachedTemplate, iPoint, 0, 5f);
        instance.AI.SetOrder(Order.Idle, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, (AIEvent[]) null);
        instance.CharacterBody.Orientation = Matrix.Identity;
        instance.CharacterBody.DesiredDirection = iOwner.mTransform.Forward;
        instance.SpawnAnimation = Magicka.Animations.spawn;
        instance.ChangeState((BaseState) RessurectionState.Instance);
        iOwner.mPlayState.EntityManager.AddEntity((Magicka.GameLogic.Entities.Entity) instance);
        this.mTimer += this.mSpawnTime;
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
            Point2 = 170
          });
        ++this.mCount;
      }
    }

    public void OnExit(Parker iOwner) => this.mWeightTimer = 0.0f;

    public float GetWeight(Parker iOwner, float iDeltaTime)
    {
      if (iOwner.mCurrentState != Parker.States.Cave || iOwner.mCurrentState == Parker.States.CallMinions || iOwner.mPreviousState == Parker.States.CallMinions || iOwner.mPlayersDead || (double) iOwner.GetCaveState.Timer < 0.5)
        return 0.0f;
      this.mWeightTimer -= iDeltaTime;
      if ((double) this.mWeightTimer <= 0.0)
      {
        TriggerArea triggerArea = iOwner.mPlayState.Level.CurrentScene.GetTriggerArea(Parker.CallMinionState.ANY);
        int count1 = triggerArea.GetCount(Parker.CallMinionState.FOREST_HASH);
        int count2 = triggerArea.GetCount(Parker.CallMinionState.POISON_HASH);
        this.mWeightTimer = 0.25f;
        if ((double) (count1 + count2) < (double) this.Count * 1.5)
          return 1f;
      }
      return 0.0f;
    }
  }

  protected class DeadState : Parker.IParkerState, IBossState<Parker>
  {
    private bool mPlayedSound;

    public void OnEnter(Parker iOwner)
    {
      this.mPlayedSound = false;
      iOwner.Movement = Vector3.Zero;
      iOwner.mAnimationController.CrossFade(iOwner.mAnimationClips[22], 0.125f, false);
    }

    public void OnUpdate(float iDeltaTime, Parker iOwner)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (!this.mPlayedSound)
      {
        AudioManager.Instance.PlayCue(Banks.Additional, Parker.DEATH_SOUND, iOwner.mAbdomenZone.AudioEmitter);
        this.mPlayedSound = true;
      }
      if (!iOwner.mAnimationController.HasFinished)
        return;
      iOwner.mDead = true;
    }

    public void OnExit(Parker iOwner)
    {
    }

    public float GetWeight(Parker iOwner, float iDeltaTime) => 0.0f;
  }
}
