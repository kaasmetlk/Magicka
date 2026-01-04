// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Death
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Death : IBoss
{
  private const float NETWORK_UPDATE_PERIOD = 0.0333333351f;
  private const float MAXHITPOINTS = 10000f;
  private const float MOVESPEED = 5f;
  private const float HITTOLERANCE = 0.0f;
  private const int MAX_NUMBER_OF_DEATHS = 16 /*0x10*/;
  private const float MIN_ATTACK_ANIMATION_SPEED = 0.65f;
  private const float MAX_ATTACK_ANIMATION_SPEED = 2f;
  private const float MAX_MOVE_DISTANCE = 10f;
  private const float MIN_MOVE_DISTANCE = 5f;
  private const float SCYTHE_ATTACK_START = 0.40625f;
  private const float SCYTHE_ATTACK_END = 0.46875f;
  private const float SCALE = 1.2f;
  private float mLastNetworkUpdate;
  protected float mNetworkUpdateTimer;
  private Death.IdleState mIdleState;
  private Death.SitIntroState mSitIntroState;
  private Death.RiseIntroState mRiseIntroState;
  private Death.PrepState mPrepState;
  private Death.AttackState mAttackState;
  private Death.MoveState mMoveState;
  private Death.HitState mHitState;
  private Death.DefeatState mDefeatState;
  private Death.DefeatPreSitState mDefeatPreSitState;
  private Death.WarpTimeState mWarpTimeState;
  private Death.RaiseDeadState mRaiseDeadState;
  private Death.MultiDeathState mMultiDeathState;
  private Death.PreMultiDeathState mPreMultiDeathState;
  private IBossState<Death> mPreviousState;
  private Death.States mNextState;
  private Death.AppearState mAppearState;
  private TriggerArea mAnyArea;
  private TriggerArea mCheckboxDeathArea;
  private float mWarpTimeTTL;
  private float mAlphaFadeSteps;
  private float mAppearDuration;
  private Matrix[] mEmptyMatrix;
  private List<NonPlayerCharacter> mSkeletonList;
  private static readonly int MAX_SKELETONS = 6;
  private static readonly int HIT_EFFECT = "death_hit".GetHashCodeCustom();
  private static readonly int ETHEREAL_SPAWN_EFFECT = "death_ethereal_spawn".GetHashCodeCustom();
  private static readonly int ETHEREAL_DESPAWN_EFFECT = "death_ethereal_despawn".GetHashCodeCustom();
  private static readonly int DEFEATED_EFFECT = "death_defeated".GetHashCodeCustom();
  private static readonly int CORPOREAL_SPAWN_EFFECT = "death_corporeal_spawn".GetHashCodeCustom();
  private static readonly int CORPOREAL_DESPAWN_EFFECT = "death_corporeal_despawn".GetHashCodeCustom();
  private static readonly int STUNNED_EFFECT = "death_stunned".GetHashCodeCustom();
  private static readonly int SCYTHE_HIT_SOUND = "wep_death_scythe".GetHashCodeCustom();
  private static readonly int SCYTHE_SWING_SOUND = "wep_death_scythe_swing".GetHashCodeCustom();
  private static readonly int DEATH_SOUND = "boss_death_death".GetHashCodeCustom();
  private static readonly int SKELETON1 = "skeleton_darksoul_arcane".GetHashCodeCustom();
  private static readonly int SKELETON2 = "skeleton_darksoul_frost".GetHashCodeCustom();
  private static readonly int SKELETON3 = "skeleton_darksoul_lightning".GetHashCodeCustom();
  private static readonly int SKELETON4 = "skeleton_darksoul_poison".GetHashCodeCustom();
  private static readonly float TIME_SLOW_DOWN = 0.5f;
  private float mHitPoints;
  private int mDamageNotifyerNumber;
  private bool mDead;
  private bool mDefeated;
  private bool mDraw;
  private bool mUpdate;
  private int mControllerSkeletonIndex;
  private static Random sRandom = new Random();
  private bool[] mIsVisibleMultiDeath = new bool[16 /*0x10*/];
  private int mIsRealDeath;
  private int mCurrentMaxDeaths;
  private Vector3 mMultiDeathOffset;
  private Matrix mMultiDeathOrientation;
  private BossDamageZone mBody;
  private BossCollisionZone mScytheBody;
  private PlayState mPlayState;
  private AudioEmitter mAudioEmitter;
  private Cue mSwingCue;
  private float mAfterImageTimer;
  private float mAfterImageIntensity;
  private float mDeathFlux;
  private Death.AfterImageRenderData[] mAfterImageRenderData;
  private Death.RenderData[] mRenderData;
  private Death.ItemRenderData[] mScytheRenderData;
  private AnimationController mController;
  private AnimationController mCloneController;
  private AnimationClip[] mClips;
  private SkinnedModel mModel;
  private Magicka.GameLogic.Entities.Character mTarget;
  private int mHandIndex;
  private Matrix mHandBindPose;
  private float mSpeed;
  private Vector3 mMovement;
  private Matrix mStartOrientation;
  private Matrix mOrientation;
  private bool mIsHit;
  private IBossState<Death> mCurrentState;
  private DataChannel mCurrentChannel;
  private BoundingSphere mBoundingSphere;
  private float mDamageFlashTimer;
  private Player[] mPlayers;
  public static readonly int SOUND_HASH = "misc_flash".GetHashCodeCustom();

  protected unsafe void RaiseDead(int iAmount, Death iOwner)
  {
    if (NetworkManager.Instance.State == NetworkState.Client || iAmount <= 0)
      return;
    for (int index = 0; index < iAmount; ++index)
    {
      CharacterTemplate cachedTemplate;
      switch (Death.sRandom.Next(4))
      {
        case 0:
          cachedTemplate = CharacterTemplate.GetCachedTemplate(Death.SKELETON1);
          break;
        case 1:
          cachedTemplate = CharacterTemplate.GetCachedTemplate(Death.SKELETON2);
          break;
        case 2:
          cachedTemplate = CharacterTemplate.GetCachedTemplate(Death.SKELETON3);
          break;
        default:
          cachedTemplate = CharacterTemplate.GetCachedTemplate(Death.SKELETON4);
          break;
      }
      NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
      Matrix mOrientation = this.mOrientation;
      StaticList<Entity> entities = this.mPlayState.EntityManager.Entities;
      int num = 0;
      float val2;
      Vector3 randomLocation;
      do
      {
        val2 = float.MaxValue;
        randomLocation = iOwner.mCheckboxDeathArea.GetRandomLocation();
        for (int iIndex = 0; iIndex < entities.Count; ++iIndex)
        {
          Vector3 position = entities[iIndex].Position;
          float result;
          Vector3.DistanceSquared(ref randomLocation, ref position, out result);
          val2 = Math.Min(result, val2);
        }
        ++num;
      }
      while ((double) val2 < 3.0 && num < 10);
      if (num >= 10)
        break;
      instance.Initialize(cachedTemplate, randomLocation, 0, 0.5f);
      instance.IsSummoned = true;
      instance.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, (AIEvent[]) null);
      instance.CharacterBody.Orientation = Matrix.Identity;
      instance.CharacterBody.DesiredDirection = mOrientation.Forward;
      instance.SpawnAnimation = Magicka.Animations.spawn;
      this.mPlayState.EntityManager.AddEntity((Entity) instance);
      this.mSkeletonList.Add(instance);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        Death.SpawnMessage spawnMessage;
        spawnMessage.Handle = instance.Handle;
        spawnMessage.Position = instance.Position;
        spawnMessage.Direction = mOrientation.Forward;
        spawnMessage.TypeID = cachedTemplate.ID;
        BossFight.Instance.SendMessage<Death.SpawnMessage>((IBoss) this, (ushort) 3, (void*) &spawnMessage, true);
      }
    }
  }

  public Death(PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mAudioEmitter = new AudioEmitter();
    Model model;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mModel = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Death/Death");
      model = iPlayState.Content.Load<Model>("Models/Bosses/Death/Death_Scythe");
      iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_arcane");
      iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_frost");
      iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_lightning");
      iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_poison");
    }
    Matrix rotationY = Matrix.CreateRotationY(3.14159274f);
    foreach (SkinnedModelBone skeletonBone in (ReadOnlyCollection<SkinnedModelBone>) this.mModel.SkeletonBones)
    {
      if (skeletonBone.Name.Equals("RightAttach", StringComparison.OrdinalIgnoreCase))
      {
        this.mHandIndex = (int) skeletonBone.Index;
        this.mHandBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mHandBindPose, ref rotationY, out this.mHandBindPose);
        Matrix.Invert(ref this.mHandBindPose, out this.mHandBindPose);
      }
    }
    this.mEmptyMatrix = new Matrix[80 /*0x50*/];
    this.mController = new AnimationController();
    this.mController.Skeleton = this.mModel.SkeletonBones;
    this.mCloneController = new AnimationController();
    this.mCloneController.Skeleton = this.mModel.SkeletonBones;
    this.mClips = new AnimationClip[12];
    this.mClips[0] = this.mModel.AnimationClips["appear"];
    this.mClips[1] = this.mModel.AnimationClips["attack_scythe_fall"];
    this.mClips[2] = this.mModel.AnimationClips["attack_scythe_rise"];
    this.mClips[3] = this.mModel.AnimationClips["defeated"];
    this.mClips[4] = this.mModel.AnimationClips["hit"];
    this.mClips[5] = this.mModel.AnimationClips["idle"];
    this.mClips[6] = this.mModel.AnimationClips["move_glide"];
    this.mClips[7] = this.mModel.AnimationClips["sit"];
    this.mClips[8] = this.mModel.AnimationClips["sit_talk"];
    this.mClips[9] = this.mModel.AnimationClips["summon_scythe"];
    this.mClips[10] = this.mModel.AnimationClips["summon_skeleton"];
    this.mClips[11] = this.mModel.AnimationClips["timewarp"];
    SkinnedModelDeferredBasicMaterial oMaterial;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out oMaterial);
    this.mBoundingSphere = this.mModel.Model.Meshes[0].BoundingSphere;
    this.mRenderData = new Death.RenderData[3];
    this.mScytheRenderData = new Death.ItemRenderData[3];
    this.mAfterImageRenderData = new Death.AfterImageRenderData[3];
    Matrix[][] iSkeleton = new Matrix[5][];
    for (int index = 0; index < iSkeleton.Length; ++index)
      iSkeleton[index] = new Matrix[80 /*0x50*/];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new Death.RenderData();
      this.mRenderData[index].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
      this.mRenderData[index].mMaterial = oMaterial;
      this.mScytheRenderData[index] = new Death.ItemRenderData();
      this.mScytheRenderData[index].SetMesh(model.Meshes[0], model.Meshes[0].MeshParts[0], 4, 0, 5);
      this.mAfterImageRenderData[index] = new Death.AfterImageRenderData(iSkeleton);
      this.mAfterImageRenderData[index].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
      this.mAfterImageRenderData[index].mMaterial = oMaterial;
    }
    this.mBody = new BossDamageZone(iPlayState, (IBoss) this, 0, 0.75f, (Primitive) new Capsule(Vector3.Zero, Matrix.CreateRotationX(-1.57079637f), 0.6f, 1.2f));
    this.mBody.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnBodyCollision);
    VertexElement[] vertexElements;
    lock (Magicka.Game.Instance.GraphicsDevice)
      vertexElements = model.Meshes[0].MeshParts[0].VertexDeclaration.GetVertexElements();
    int offsetInBytes = -1;
    for (int index = 0; index < vertexElements.Length; ++index)
    {
      if (vertexElements[index].VertexElementUsage == VertexElementUsage.Position)
      {
        offsetInBytes = (int) vertexElements[index].Offset;
        break;
      }
    }
    if (offsetInBytes < 0)
      throw new Exception("No positions found");
    Vector3[] vector3Array = new Vector3[model.Meshes[0].MeshParts[0].NumVertices];
    model.Meshes[0].VertexBuffer.GetData<Vector3>(offsetInBytes, vector3Array, model.Meshes[0].MeshParts[0].StartIndex, vector3Array.Length, model.Meshes[0].MeshParts[0].VertexStride);
    BoundingBox fromPoints = BoundingBox.CreateFromPoints((IEnumerable<Vector3>) vector3Array);
    Vector3 result;
    Vector3.Subtract(ref fromPoints.Max, ref fromPoints.Min, out result);
    this.mScytheBody = new BossCollisionZone(iPlayState, (IBoss) this, new Primitive[1]
    {
      (Primitive) new Box(new Vector3(fromPoints.Min.X, fromPoints.Max.Y * 0.5f, fromPoints.Min.Z), Matrix.Identity, new Vector3(result.X, fromPoints.Max.Y * 0.5f, result.Z))
    });
    this.mScytheBody.Body.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnScytheCollision);
    this.mSitIntroState = new Death.SitIntroState();
    this.mRiseIntroState = new Death.RiseIntroState();
    this.mPrepState = new Death.PrepState();
    this.mAttackState = new Death.AttackState();
    this.mMoveState = new Death.MoveState();
    this.mHitState = new Death.HitState();
    this.mIdleState = new Death.IdleState();
    this.mDefeatState = new Death.DefeatState();
    this.mDefeatPreSitState = new Death.DefeatPreSitState();
    this.mRaiseDeadState = new Death.RaiseDeadState();
    this.mWarpTimeState = new Death.WarpTimeState();
    this.mMultiDeathState = new Death.MultiDeathState();
    this.mPreMultiDeathState = new Death.PreMultiDeathState();
    this.mAppearState = new Death.AppearState();
    this.mSkeletonList = new List<NonPlayerCharacter>();
  }

  protected bool OnBodyCollision(
    CollisionSkin iSkin0,
    int iPrim0,
    CollisionSkin iSkin1,
    int iPrim1)
  {
    return false;
  }

  protected bool OnScytheCollision(
    CollisionSkin iSkin0,
    int iPrim0,
    CollisionSkin iSkin1,
    int iPrim1)
  {
    if (iSkin1.Owner != null && this.mCurrentState is Death.AttackState | this.mCurrentState is Death.MultiDeathState)
    {
      if (iSkin1.Owner.Tag is Magicka.GameLogic.Entities.Character && (double) (iSkin1.Owner.Tag as Magicka.GameLogic.Entities.Character).HitPoints > 0.0)
      {
        if (!this.mIsHit && this.mController.AnimationClip == this.mClips[1] && !(iSkin1.Owner.Tag as Magicka.GameLogic.Entities.Character).IsImmortal)
        {
          Vector3 position = (iSkin1.Owner.Tag as Magicka.GameLogic.Entities.Character).Position;
          DamageNotifyer.Instance.AddNumber((iSkin1.Owner.Tag as Magicka.GameLogic.Entities.Character).HitPoints, ref position, 1f, false);
          (iSkin1.Owner.Tag as Magicka.GameLogic.Entities.Character).Damage((iSkin1.Owner.Tag as Magicka.GameLogic.Entities.Character).HitPoints, Elements.Arcane);
        }
        return true;
      }
      if (iSkin1.Owner.Tag is Shield && !(iSkin1.Owner.Tag as Shield).Dead)
        (iSkin1.Owner.Tag as Shield).Kill();
    }
    return false;
  }

  public void Initialize(ref Matrix iOrientation, int iUniqueID)
  {
    this.Initialize(ref iOrientation);
  }

  public void Initialize(ref Matrix iOrientation)
  {
    this.mOrientation = iOrientation;
    this.mStartOrientation = iOrientation;
    this.mHitPoints = 10000f;
    this.mDead = false;
    this.mDefeated = false;
    this.mBody.IsEthereal = false;
    this.mCurrentState = (IBossState<Death>) this.mSitIntroState;
    this.mCurrentState.OnEnter(this);
    this.mBody.Initialize();
    this.mBody.Body.CollisionSkin.NonCollidables.Add(this.mScytheBody.Body.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Entity) this.mBody);
    this.mScytheBody.Initialize();
    this.mScytheBody.Body.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    this.mPlayState.EntityManager.AddEntity((Entity) this.mScytheBody);
    this.mPlayers = Magicka.Game.Instance.Players;
    this.mIsHit = false;
    this.mDraw = true;
    this.mUpdate = true;
    this.mSpeed = (float) (5.0 * (2.0 - (double) this.mHitPoints / 10000.0));
    this.mControllerSkeletonIndex = 0;
    this.mWarpTimeTTL = -99f;
    this.mAnyArea = this.mPlayState.Level.CurrentScene.GetTriggerArea(TriggerArea.ANYID);
    this.mCheckboxDeathArea = this.mPlayState.Level.CurrentScene.GetTriggerArea("checkbox_death".GetHashCodeCustom());
    this.mIsVisibleMultiDeath[0] = true;
    for (int index = 1; index < this.mIsVisibleMultiDeath.Length; ++index)
      this.mIsVisibleMultiDeath[index] = false;
    this.mDamageFlashTimer = 0.0f;
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
    this.mDamageFlashTimer = Math.Max(this.mDamageFlashTimer - iDeltaTime, 0.0f);
    this.mCurrentChannel = iDataChannel;
    iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
    if (iFightStarted && this.mCurrentState is Death.SitIntroState && !this.mDead)
      this.ChangeState(Death.States.RiseIntro);
    if (this.mCurrentState == this.mAppearState || this.mCurrentState == this.mPreMultiDeathState)
      this.mAlphaFadeSteps += iDeltaTime / this.mAppearDuration;
    this.mRenderData[(int) iDataChannel].Flash = this.mDamageFlashTimer * 10f;
    this.mRenderData[(int) iDataChannel].mMaterial.Alpha = Math.Min(this.mAlphaFadeSteps, 1f);
    if ((double) this.mHitPoints <= 1.0 && !this.mDefeated)
    {
      this.mDefeated = true;
      this.KillSkeletons();
      Vector3 translation = this.mOrientation.Translation;
      Vector3 forward = this.mOrientation.Forward;
      EffectManager.Instance.StartEffect(Death.HIT_EFFECT, ref translation, ref forward, out VisualEffectReference _);
      AudioManager.Instance.PlayCue(Banks.Characters, Death.DEATH_SOUND, this.mAudioEmitter);
      this.ChangeState(Death.States.DefeatPreSit);
    }
    this.mIsVisibleMultiDeath.CopyTo((Array) this.mRenderData[(int) iDataChannel].mDeathsIsVisible, 0);
    this.mIsVisibleMultiDeath.CopyTo((Array) this.mScytheRenderData[(int) iDataChannel].mItemsIsVisible, 0);
    if (this.mCurrentState == this.mSitIntroState || this.mCurrentState == this.mDefeatState)
      this.mScytheRenderData[(int) iDataChannel].mItemsIsVisible[0] = false;
    this.mAudioEmitter.Position = this.mOrientation.Translation;
    this.mAudioEmitter.Forward = this.mOrientation.Forward;
    this.mAudioEmitter.Up = this.mOrientation.Up;
    if (!this.mDead && !this.mDefeated)
    {
      bool flag = false;
      for (int index = 0; index < this.mPlayers.Length; ++index)
      {
        if (this.mPlayers[index].Playing && this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
          flag = true;
      }
      if (!flag && this.mCurrentState != this.mIdleState && this.mController.HasFinished && !this.mController.CrossFadeEnabled)
        this.ChangeState(Death.States.Idle);
      else if (this.mIsHit && this.mCurrentState != this.mHitState)
        this.ChangeState(Death.States.Hit);
    }
    this.mCurrentState.OnUpdate(iDeltaTime, this);
    Vector3 iPosition = this.mOrientation.Translation;
    iPosition.X += this.mMovement.X * this.mSpeed * iDeltaTime;
    iPosition.Z += this.mMovement.Z * this.mSpeed * iDeltaTime;
    Segment iSeg = new Segment();
    iSeg.Origin = iPosition;
    ++iSeg.Origin.Y;
    iSeg.Delta.Y -= 4f;
    Vector3 oPos;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
    {
      if (this.mCurrentState is Death.SitIntroState || this.mCurrentState is Death.DefeatState)
        ++oPos.Y;
      this.mOrientation.Translation = oPos;
    }
    if (this.mUpdate)
    {
      Matrix mOrientation1 = this.mOrientation;
      if (this.mCurrentState != this.mSitIntroState && this.mCurrentState != this.mDefeatState)
        this.mDeathFlux += iDeltaTime;
      else
        this.mDeathFlux = 0.0f;
      Vector3 translation = mOrientation1.Translation;
      translation.Y += (float) Math.Cos((double) this.mDeathFlux) * 0.2f;
      mOrientation1.Translation = translation;
      MagickaMath.UniformMatrixScale(ref mOrientation1, 1.2f);
      this.mController.Update(iDeltaTime, ref mOrientation1, true);
      this.mController.SkinnedBoneTransforms.CopyTo((Array) this.mRenderData[(int) iDataChannel].mBones[this.mControllerSkeletonIndex], 0);
      iPosition.Y += (this.mBody.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length * 0.5f;
      iPosition.Y += (oPos.Y - iPosition.Y) * iDeltaTime;
      Matrix mOrientation2 = this.mOrientation with
      {
        Translation = new Vector3()
      };
      this.mBody.SetOrientation(ref iPosition, ref mOrientation2);
      Matrix result = this.mHandBindPose;
      Matrix.Multiply(ref result, ref this.mController.SkinnedBoneTransforms[this.mHandIndex], out result);
      this.mScytheRenderData[(int) iDataChannel].WorldOrientation[this.mControllerSkeletonIndex] = result;
      iPosition = result.Translation;
      result.Translation = new Vector3();
      this.mScytheBody.SetOrientation(ref iPosition, ref result);
    }
    else
    {
      iPosition = new Vector3(0.0f, 50f, 0.0f);
      Matrix identity = Matrix.Identity;
      this.mBody.SetOrientation(ref iPosition, ref identity);
      this.mScytheBody.SetOrientation(ref iPosition, ref identity);
    }
    if (!this.mDraw)
      return;
    this.mBoundingSphere.Center = this.mOrientation.Translation;
    this.mScytheRenderData[(int) iDataChannel].mBoundingSphere = this.mBoundingSphere;
    this.mRenderData[(int) iDataChannel].mBoundingSphere = this.mBoundingSphere;
    this.mRenderData[(int) iDataChannel].RenderAdditive = this.mBody.IsEthereal;
    if (this.mBody.IsEthereal)
    {
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) this.mRenderData[(int) iDataChannel]);
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mScytheRenderData[(int) iDataChannel]);
    }
    else
    {
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mScytheRenderData[(int) iDataChannel]);
    }
    float num = (this.mMovement with { Y = 0.0f }).LengthSquared();
    if ((double) num <= 0.10000000149011612)
      return;
    Death.AfterImageRenderData iObject = this.mAfterImageRenderData[(int) iDataChannel];
    int count = this.mModel.SkeletonBones.Count;
    this.mAfterImageTimer -= iDeltaTime;
    this.mAfterImageIntensity -= iDeltaTime;
    while ((double) this.mAfterImageTimer <= 0.0)
    {
      this.mAfterImageTimer += 0.05f;
      if ((double) num > 1.0 / 1000.0)
      {
        while ((double) this.mAfterImageIntensity <= 0.0)
          this.mAfterImageIntensity += 0.05f;
      }
      for (int index = iObject.mSkeleton.Length - 1; index > 0; --index)
        Array.Copy((Array) iObject.mSkeleton[index - 1], (Array) iObject.mSkeleton[index], count);
      Array.Copy((Array) this.mController.SkinnedBoneTransforms, (Array) iObject.mSkeleton[0], count);
    }
    iObject.mIntensity = this.mAfterImageIntensity * 20f;
    iObject.mBoundingSphere = this.mRenderData[(int) iDataChannel].mBoundingSphere;
    iObject.RenderAdditive = this.mBody.IsEthereal;
    if (this.mBody.IsEthereal)
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
    else
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
  }

  protected unsafe void ChangeState(Death.States iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.mCurrentState.OnExit(this);
    this.mPreviousState = this.mCurrentState;
    this.mCurrentState = this.GetState(iState);
    this.mCurrentState.OnEnter(this);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    Death.ChangeStateMessage changeStateMessage;
    changeStateMessage.NewState = iState;
    BossFight.Instance.SendMessage<Death.ChangeStateMessage>((IBoss) this, (ushort) 1, (void*) &changeStateMessage, true);
  }

  private IBossState<Death> GetState(Death.States iState)
  {
    switch (iState)
    {
      case Death.States.Idle:
        return (IBossState<Death>) this.mIdleState;
      case Death.States.SitIntro:
        return (IBossState<Death>) this.mSitIntroState;
      case Death.States.RiseIntro:
        return (IBossState<Death>) this.mRiseIntroState;
      case Death.States.Prep:
        return (IBossState<Death>) this.mPrepState;
      case Death.States.Attack:
        return (IBossState<Death>) this.mAttackState;
      case Death.States.Move:
        return (IBossState<Death>) this.mMoveState;
      case Death.States.Hit:
        return (IBossState<Death>) this.mHitState;
      case Death.States.Defeat:
        return (IBossState<Death>) this.mDefeatState;
      case Death.States.DefeatPreSit:
        return (IBossState<Death>) this.mDefeatPreSitState;
      case Death.States.WarpTime:
        return (IBossState<Death>) this.mWarpTimeState;
      case Death.States.RaiseDead:
        return (IBossState<Death>) this.mRaiseDeadState;
      case Death.States.MultiDeath:
        return (IBossState<Death>) this.mMultiDeathState;
      case Death.States.PreMultiDeath:
        return (IBossState<Death>) this.mPreMultiDeathState;
      case Death.States.Appear:
        return (IBossState<Death>) this.mAppearState;
      default:
        return (IBossState<Death>) null;
    }
  }

  public void DeInitialize()
  {
  }

  private unsafe void SelectTarget()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    int num1 = 4;
    int num2 = Death.sRandom.Next(Magicka.Game.Instance.PlayerCount);
    for (int index = 0; index < num1; ++index)
    {
      if (this.mPlayers[(index + num2) % num1].Playing)
      {
        Player mPlayer = this.mPlayers[(index + num2) % num1];
        if (mPlayer.Avatar != null && !mPlayer.Avatar.Dead)
        {
          this.mTarget = (Magicka.GameLogic.Entities.Character) mPlayer.Avatar;
          if (NetworkManager.Instance.State != NetworkState.Server)
            break;
          Death.ChangeTargetMessage changeTargetMessage;
          changeTargetMessage.Target = this.mTarget.Handle;
          BossFight.Instance.SendMessage<Death.ChangeTargetMessage>((IBoss) this, (ushort) 2, (void*) &changeTargetMessage, true);
          break;
        }
      }
    }
  }

  protected void Turn(ref Vector3 iNewDirection, float iTurnSpeed, float iDeltaTime)
  {
    Vector3 up = Vector3.Up;
    Vector3 result1;
    Vector3.Cross(ref iNewDirection, ref up, out result1);
    Matrix identity = Matrix.Identity with
    {
      Forward = iNewDirection,
      Up = up,
      Right = result1
    };
    Quaternion result2;
    Quaternion.CreateFromRotationMatrix(ref this.mOrientation, out result2);
    Quaternion result3;
    Quaternion.CreateFromRotationMatrix(ref identity, out result3);
    Quaternion.Lerp(ref result2, ref result3, MathHelper.Clamp(iDeltaTime * iTurnSpeed, 0.0f, 1f), out result3);
    Matrix result4;
    Matrix.CreateFromQuaternion(ref result3, out result4);
    result4.Translation = this.mOrientation.Translation;
    this.mOrientation = result4;
  }

  protected Vector3 Movement
  {
    get => this.mMovement;
    set
    {
      value.Y = 0.0f;
      this.mMovement = value;
      float d = this.mMovement.LengthSquared();
      if ((double) d <= 1.4012984643248171E-45)
        return;
      float num1 = (float) Math.Sqrt((double) d);
      float num2 = 1f / num1;
      if ((double) num1 <= 1.0)
        return;
      this.mMovement.X = value.X * num2;
      this.mMovement.Y = value.Y * num2;
      this.mMovement.Z = value.Z * num2;
    }
  }

  public DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if (!this.mDraw || this.mCurrentState is Death.MultiDeathState && this.mMultiDeathState.mIsFakeAttack || !this.mDraw || this.mBody.IsEthereal || (iDamage.AttackProperty & AttackProperties.Damage) == (AttackProperties) 0 || (iDamage.Element & Elements.Life) != Elements.Life)
      return DamageResult.Deflected;
    if ((double) Math.Abs(iDamage.Amount) >= 0.0)
    {
      if (!this.mIsHit)
      {
        if (this.mSwingCue != null)
          this.mSwingCue.Stop(AudioStopOptions.AsAuthored);
        AudioManager.Instance.PlayCue(Banks.Weapons, Death.SCYTHE_HIT_SOUND, this.mAudioEmitter);
        this.mIsHit = true;
        Vector3 translation = this.mOrientation.Translation;
        Vector3 right = Vector3.Right;
        EffectManager.Instance.StartEffect(Death.HIT_EFFECT, ref translation, ref right, out VisualEffectReference _);
        translation.Y += 2.2f;
        this.mDamageNotifyerNumber = DamageNotifyer.Instance.AddNumber(Math.Abs(iDamage.Amount), ref translation, 1f, false);
      }
      this.mHitPoints -= Math.Abs(iDamage.Amount);
      this.mDamageFlashTimer = 0.1f;
      if (this.mDamageNotifyerNumber >= 0)
        DamageNotifyer.Instance.AddToNumber(this.mDamageNotifyerNumber, Math.Abs(iDamage.Amount));
    }
    if ((double) this.mHitPoints < 1.0)
      this.mHitPoints = 1f;
    return DamageResult.Deflected;
  }

  public void Damage(int iPartIndex, float iDamage, Elements iElement)
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

  public bool Dead => this.mDead;

  public float MaxHitPoints => 10000f;

  public float HitPoints => this.mHitPoints;

  public void SetSlow(int iIndex)
  {
  }

  public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
  {
    oPosition = new Vector3();
  }

  public bool HasStatus(int iIndex, StatusEffects iStatus) => false;

  public float StatusMagnitude(int iIndex, StatusEffects iStatus) => 0.0f;

  public StatusEffect[] GetStatusEffects() => (StatusEffect[]) null;

  public unsafe void KillSkeletons()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      Death.ClearMessage clearMessage;
      BossFight.Instance.SendMessage<Death.ClearMessage>((IBoss) this, (ushort) 4, (void*) &clearMessage, true);
    }
    foreach (Magicka.GameLogic.Entities.Character mSkeleton in this.mSkeletonList)
      mSkeleton.OverKill();
    this.mSkeletonList.Clear();
  }

  public bool IsEthereal
  {
    get => this.mBody.IsEthereal;
    set => this.mBody.IsEthereal = value;
  }

  public void Corporealize()
  {
    for (int index = 0; index < this.mIsVisibleMultiDeath.Length; ++index)
    {
      if (index != this.mIsRealDeath)
        this.mIsVisibleMultiDeath[index] = false;
    }
  }

  public void ScriptMessage(BossMessages iMessage)
  {
  }

  private unsafe void NetworkUpdate()
  {
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer))
      return;
    Death.UpdateMessage updateMessage = new Death.UpdateMessage();
    updateMessage.Animation = (byte) 0;
    while ((int) updateMessage.Animation < this.mClips.Length && this.mController.AnimationClip != this.mClips[(int) updateMessage.Animation])
      ++updateMessage.Animation;
    updateMessage.AnimationTime = this.mController.Time;
    updateMessage.Hitpoints = this.mHitPoints;
    updateMessage.Orientation = this.mOrientation;
    for (int index = 0; index < networkServer.Connections; ++index)
    {
      float num = networkServer.GetLatency(index) * 0.5f;
      updateMessage.AnimationTime += num;
      BossFight.Instance.SendMessage<Death.UpdateMessage>((IBoss) this, (ushort) 0, (void*) &updateMessage, false, index);
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
        Death.UpdateMessage updateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &updateMessage);
        if (this.mController.AnimationClip != this.mClips[(int) updateMessage.Animation])
          this.mController.StartClip(this.mClips[(int) updateMessage.Animation], false);
        this.mController.Time = updateMessage.AnimationTime;
        this.mHitPoints = updateMessage.Hitpoints;
        this.mOrientation = updateMessage.Orientation;
        break;
      case 1:
        Death.ChangeStateMessage changeStateMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeStateMessage);
        IBossState<Death> state = this.GetState(changeStateMessage.NewState);
        if (state == null)
          break;
        this.mCurrentState.OnExit(this);
        this.mPreviousState = this.mCurrentState;
        this.mCurrentState = state;
        this.mCurrentState.OnEnter(this);
        break;
      case 2:
        Death.ChangeTargetMessage changeTargetMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &changeTargetMessage);
        if (changeTargetMessage.Target == ushort.MaxValue)
        {
          this.mTarget = (Magicka.GameLogic.Entities.Character) null;
          break;
        }
        this.mTarget = Entity.GetFromHandle((int) changeTargetMessage.Target) as Magicka.GameLogic.Entities.Character;
        break;
      case 3:
        Death.SpawnMessage spawnMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &spawnMessage);
        CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(spawnMessage.TypeID);
        NonPlayerCharacter fromHandle = Entity.GetFromHandle((int) spawnMessage.Handle) as NonPlayerCharacter;
        fromHandle.Initialize(cachedTemplate, spawnMessage.Position, 0, 0.5f);
        fromHandle.IsSummoned = true;
        fromHandle.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, -1, 0, (AIEvent[]) null);
        fromHandle.CharacterBody.Orientation = Matrix.Identity;
        fromHandle.CharacterBody.DesiredDirection = spawnMessage.Direction;
        fromHandle.SpawnAnimation = Magicka.Animations.spawn;
        this.mPlayState.EntityManager.AddEntity((Entity) fromHandle);
        this.mSkeletonList.Add(fromHandle);
        break;
      case 4:
        this.mSkeletonList.Clear();
        break;
      case 5:
        Death.MultiDeathMessage multiDeathMessage;
        BossUpdateMessage.ConvertTo(ref iMsg, (void*) &multiDeathMessage);
        this.mIsRealDeath = (int) multiDeathMessage.RealDeath;
        break;
    }
  }

  public void NetworkInitialize(ref BossInitializeMessage iMsg)
  {
    throw new NotImplementedException();
  }

  public BossEnum GetBossType() => BossEnum.Death;

  public bool NetworkInitialized => true;

  public float ResistanceAgainst(Elements iElement) => 1f;

  public class ItemRenderData : 
    RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>,
    IRenderableAdditiveObject
  {
    public Matrix[] WorldOrientation;
    public bool[] mItemsIsVisible;

    public ItemRenderData()
    {
      this.WorldOrientation = new Matrix[16 /*0x10*/];
      this.mItemsIsVisible = new bool[16 /*0x10*/];
      for (int index = 0; index < 16 /*0x10*/; ++index)
        this.mItemsIsVisible[index] = false;
    }

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      for (int index = 0; index < this.mItemsIsVisible.Length; ++index)
      {
        iEffect1.World = this.WorldOrientation[index];
        this.mMaterial.WorldTransform = this.WorldOrientation[index];
        iEffect1.CommitChanges();
        if (this.mItemsIsVisible[index])
          iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      }
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignOpacityToEffect(iEffect1);
      for (int index = 0; index < this.mItemsIsVisible.Length; ++index)
      {
        iEffect1.World = this.WorldOrientation[index];
        this.mMaterial.WorldTransform = this.WorldOrientation[index];
        iEffect1.CommitChanges();
        if (this.mItemsIsVisible[index])
          iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      }
    }
  }

  public class RenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>,
    IRenderableAdditiveObject
  {
    public bool RenderAdditive;
    public float Flash;
    public Matrix[][] mBones;
    public bool[] mDeathsIsVisible;

    public RenderData()
    {
      this.mBones = new Matrix[16 /*0x10*/][];
      for (int index = 0; index < this.mBones.Length; ++index)
        this.mBones[index] = new Matrix[80 /*0x50*/];
      this.mDeathsIsVisible = new bool[16 /*0x10*/];
      for (int index = 0; index < this.mDeathsIsVisible.Length; ++index)
        this.mDeathsIsVisible[index] = false;
    }

    public override int Technique => this.RenderAdditive ? 2 : base.Technique;

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.ProjectionMapEnabled = false;
      for (int index = 0; index < this.mDeathsIsVisible.Length; ++index)
      {
        iEffect1.Bones = this.mBones[index];
        iEffect1.CommitChanges();
        if (this.mDeathsIsVisible[index])
        {
          iEffect1.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
          iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
        }
      }
      iEffect1.OverrideColor = Vector4.Zero;
      iEffect1.Colorize = new Vector4();
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignOpacityToEffect(iEffect1);
      for (int index = 0; index < this.mDeathsIsVisible.Length; ++index)
      {
        iEffect1.Bones = this.mBones[index];
        iEffect1.CommitChanges();
        if (this.mDeathsIsVisible[index])
          iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      }
    }
  }

  protected class AfterImageRenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>,
    IRenderableAdditiveObject
  {
    protected static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);
    public float mIntensity;
    public Matrix[][] mSkeleton;
    public bool RenderAdditive;

    public AfterImageRenderData(Matrix[][] iSkeleton) => this.mSkeleton = iSkeleton;

    public override int Technique => this.RenderAdditive ? 2 : base.Technique;

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.ProjectionMapEnabled = false;
      iEffect1.Colorize = new Vector4(Death.AfterImageRenderData.ColdColor, 1f);
      float num1 = 0.333f;
      float num2 = (float) (0.33300000429153442 / ((double) this.mSkeleton.Length + 1.0));
      float num3 = num1 + this.mIntensity * num2;
      for (int index = 0; index < this.mSkeleton.Length; ++index)
      {
        if ((double) num3 != 0.0)
        {
          iEffect1.Alpha = num3;
          iEffect1.Bones = this.mSkeleton[index];
          iEffect1.CommitChanges();
          iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
          num3 -= num2;
        }
      }
      iEffect1.Colorize = new Vector4();
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
    }
  }

  public class SitIntroState : IBossState<Death>
  {
    public void OnEnter(Death iOwner)
    {
      iOwner.mSpeed = 0.0f;
      iOwner.mOrientation = iOwner.mStartOrientation;
      iOwner.mDraw = true;
      iOwner.mController.StartClip(iOwner.mClips[8], true);
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
    }

    public void OnExit(Death iOwner) => iOwner.mPreviousState = iOwner.mCurrentState;
  }

  public class RiseIntroState : IBossState<Death>
  {
    private float mIdleTimer;

    public void OnEnter(Death iOwner)
    {
      iOwner.mDraw = true;
      iOwner.mController.CrossFade(iOwner.mClips[9], 0.15f, false);
      this.mIdleTimer = 0.15f;
      iOwner.IsEthereal = true;
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      iOwner.Movement = iOwner.mOrientation.Forward;
      iOwner.mSpeed = 1f;
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled)
        this.mIdleTimer -= iDeltaTime;
      if ((double) this.mIdleTimer > 0.0)
        return;
      iOwner.ChangeState(Death.States.Prep);
    }

    public void OnExit(Death iOwner)
    {
      iOwner.mSpeed = (float) (5.0 * (2.0 - (double) iOwner.mHitPoints / 10000.0));
      iOwner.mPreviousState = iOwner.mCurrentState;
    }
  }

  public class IdleState : IBossState<Death>
  {
    public void OnEnter(Death iOwner)
    {
      iOwner.mDraw = true;
      iOwner.mController.CrossFade(iOwner.mClips[5], 0.2f, true);
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      for (int index = 0; index < iOwner.mPlayers.Length; ++index)
      {
        if (iOwner.mPlayers[index].Playing && iOwner.mPlayers[index].Avatar != null && !iOwner.mPlayers[index].Avatar.Dead)
        {
          iOwner.ChangeState(Death.States.Prep);
          break;
        }
      }
    }

    public void OnExit(Death iOwner)
    {
    }
  }

  public class PrepState : IBossState<Death>
  {
    private float mIdleTimer;

    public void OnEnter(Death iOwner)
    {
      iOwner.mDraw = false;
      iOwner.mBody.IsEthereal = true;
      iOwner.mController.StartClip(iOwner.mClips[6], true);
      float num = iOwner.mHitPoints / 10000f;
      this.mIdleTimer = (float) (1.0 + 3.0 * (0.5 + Death.sRandom.NextDouble() * 0.5) * (double) num);
      Vector3 translation = iOwner.mOrientation.Translation;
      Vector3 forward = iOwner.mOrientation.Forward;
      EffectManager.Instance.StartEffect(Death.ETHEREAL_DESPAWN_EFFECT, ref translation, ref forward, out VisualEffectReference _);
      iOwner.SelectTarget();
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      this.mIdleTimer -= iDeltaTime;
      if ((double) this.mIdleTimer > 0.0)
        return;
      float iDifficulty = iOwner.mHitPoints / 10000f;
      if (iOwner.mTarget != null)
      {
        bool flag = false;
        while (!flag)
        {
          int num = Death.sRandom.Next(10);
          if (num < 3 & iOwner.mPreviousState != iOwner.mRaiseDeadState)
          {
            this.PositionDeath(iOwner, 1f);
            iOwner.mNextState = Death.States.RaiseDead;
            iOwner.ChangeState(Death.States.Appear);
            flag = true;
          }
          else if (num < 6 & (double) iOwner.mHitPoints / 10000.0 < 0.800000011920929 & iOwner.mPreviousState != iOwner.mMultiDeathState)
          {
            this.PositionDeath(iOwner, iDifficulty);
            iOwner.ChangeState(Death.States.PreMultiDeath);
            flag = true;
          }
          else if (num < 9 & (double) iOwner.mHitPoints / 10000.0 < 0.5 & iOwner.mPreviousState != iOwner.mWarpTimeState & (double) iOwner.mWarpTimeTTL <= -5.0)
          {
            this.PositionDeath(iOwner, 1f);
            iOwner.mNextState = Death.States.WarpTime;
            iOwner.ChangeState(Death.States.Appear);
            flag = true;
          }
          else
          {
            this.PositionDeath(iOwner, iDifficulty);
            iOwner.mNextState = Death.States.Move;
            iOwner.ChangeState(Death.States.Appear);
            flag = true;
          }
        }
      }
      else
        this.mIdleTimer = (float) (1.0 + 3.0 * (0.5 + Death.sRandom.NextDouble() * 0.5) * (double) iDifficulty);
    }

    public void OnExit(Death iOwner)
    {
    }

    internal void PositionDeath(Death iOwner, float iDifficulty)
    {
      Vector3 position = iOwner.mTarget.Position;
      Vector3 direction = iOwner.mTarget.Direction;
      Vector3 result1;
      Vector3.Negate(ref direction, out result1);
      float scaleFactor = MathHelper.Lerp(5f, 10f, iDifficulty);
      Vector3.Multiply(ref result1, scaleFactor, out result1);
      Vector3 result2;
      Vector3.Add(ref position, ref result1, out result2);
      Vector3 oPoint;
      if ((double) iOwner.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result2, out oPoint, MovementProperties.Default) < 7.0)
        result2 = oPoint;
      Vector3 result3;
      Vector3.Subtract(ref position, ref result2, out result3);
      result3.Y = 0.0f;
      result3.Normalize();
      Vector3 up = Vector3.Up;
      Vector3 result4;
      Vector3.Cross(ref result3, ref up, out result4);
      iOwner.mOrientation.Forward = result3;
      iOwner.mOrientation.Up = up;
      iOwner.mOrientation.Right = result4;
      iOwner.mOrientation.Translation = result2;
    }
  }

  public class MoveState : IBossState<Death>
  {
    public void OnEnter(Death iOwner)
    {
      iOwner.mBody.IsEthereal = true;
      iOwner.mDraw = true;
      iOwner.mController.CrossFade(iOwner.mClips[6], 0.2f, true);
      iOwner.mController.Update(1f / 1000f, ref iOwner.mOrientation, true);
      int count = iOwner.mController.Skeleton.Count;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        Death.AfterImageRenderData afterImageRenderData = iOwner.mAfterImageRenderData[index1];
        for (int index2 = 0; index2 < afterImageRenderData.mSkeleton.Length; ++index2)
          Array.Copy((Array) iOwner.mController.SkinnedBoneTransforms, (Array) afterImageRenderData.mSkeleton[index2], count);
      }
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        return;
      if (iOwner.mTarget == null)
      {
        iOwner.ChangeState(Death.States.Prep);
      }
      else
      {
        if ((double) iOwner.mSpeed <= 0.10000000149011612)
          iOwner.mSpeed = (float) (5.0 * (2.0 - (double) iOwner.mHitPoints / 10000.0));
        Vector3 position = iOwner.mTarget.Position;
        Vector3 translation = iOwner.mOrientation.Translation;
        Vector3 result;
        Vector3.Subtract(ref position, ref translation, out result);
        result.Y = 0.0f;
        float num = result.Length();
        result.Normalize();
        iOwner.Movement = result;
        iOwner.Turn(ref result, 8f, iDeltaTime);
        if ((double) num >= 2.5)
          return;
        iOwner.ChangeState(Death.States.Attack);
      }
    }

    public void OnExit(Death iOwner)
    {
      iOwner.mBody.IsEthereal = false;
      iOwner.Movement = Vector3.Zero;
    }
  }

  public class AttackState : IBossState<Death>
  {
    private bool mScytheLifted;

    public void OnEnter(Death iOwner)
    {
      this.mScytheLifted = false;
      iOwner.mController.CrossFade(iOwner.mClips[2], 0.2f, false);
      iOwner.mSwingCue = AudioManager.Instance.PlayCue(Banks.Weapons, Death.SCYTHE_SWING_SOUND, iOwner.mAudioEmitter);
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      if ((double) iOwner.mSpeed <= 0.10000000149011612)
        iOwner.mSpeed = 1f;
      if (iOwner.mTarget == null)
      {
        iOwner.ChangeState(Death.States.Prep);
      }
      else
      {
        if (NetworkManager.Instance.State != NetworkState.Client)
        {
          Vector3 position = iOwner.mTarget.Position;
          Vector3 translation = iOwner.mOrientation.Translation;
          Vector3 result;
          Vector3.Subtract(ref position, ref translation, out result);
          result.Y = 0.0f;
          float num = result.Length();
          result.Normalize();
          if ((double) iOwner.mTarget.Radius + (double) iOwner.mBody.Radius < (double) num)
          {
            float iTurnSpeed = (float) ((double) num / 2.5 * 5.0 * (2.0 - (double) iOwner.mHitPoints / 10000.0));
            iOwner.mSpeed = iTurnSpeed;
            iOwner.Movement = result;
            if ((double) iTurnSpeed < 8.0)
              iTurnSpeed = 8f;
            iOwner.Turn(ref result, iTurnSpeed, iDeltaTime);
          }
          else
            iOwner.mSpeed = 0.1f;
        }
        if (!iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
          return;
        if (!this.mScytheLifted)
        {
          iOwner.mController.CrossFade(iOwner.mClips[1], 0.2f, false);
          this.mScytheLifted = true;
        }
        else
          iOwner.ChangeState(Death.States.Prep);
      }
    }

    public void OnExit(Death iOwner)
    {
      iOwner.mSpeed = (float) (5.0 * (2.0 - (double) iOwner.mHitPoints / 10000.0));
      iOwner.Movement = Vector3.Zero;
      iOwner.mController.Speed = 1f;
      iOwner.mPreviousState = iOwner.mCurrentState;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        Death.AfterImageRenderData afterImageRenderData = iOwner.mAfterImageRenderData[index1];
        for (int index2 = 0; index2 < afterImageRenderData.mSkeleton.Length; ++index2)
          iOwner.mEmptyMatrix.CopyTo((Array) afterImageRenderData.mSkeleton[index2], 0);
      }
    }
  }

  public class HitState : IBossState<Death>
  {
    private VisualEffectReference e;

    public void OnEnter(Death iOwner)
    {
      iOwner.mController.CrossFade(iOwner.mClips[4], 0.2f, false);
      Vector3 translation = iOwner.mOrientation.Translation;
      Vector3 forward = iOwner.mOrientation.Forward;
      EffectManager.Instance.StartEffect(Death.STUNNED_EFFECT, ref translation, ref forward, out this.e);
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      if (!iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
        return;
      iOwner.ChangeState(Death.States.Prep);
    }

    public void OnExit(Death iOwner)
    {
      EffectManager.Instance.Stop(ref this.e);
      iOwner.mDraw = false;
      iOwner.mIsHit = false;
      iOwner.mDamageNotifyerNumber = -1;
      if ((double) iOwner.mHitPoints >= 1.0)
        return;
      iOwner.mHitPoints = 1f;
    }
  }

  public class DefeatState : IBossState<Death>
  {
    public void OnEnter(Death iOwner)
    {
      iOwner.IsEthereal = false;
      iOwner.mDraw = true;
      iOwner.mOrientation = iOwner.mStartOrientation;
      iOwner.mController.CrossFade(iOwner.mClips[8], 0.2f, false);
      iOwner.mHitPoints = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
    }

    public void OnExit(Death iOwner)
    {
    }
  }

  public class DefeatPreSitState : IBossState<Death>
  {
    private VisualEffectReference e;

    public void OnEnter(Death iOwner)
    {
      iOwner.IsEthereal = false;
      iOwner.mDraw = true;
      iOwner.mController.CrossFade(iOwner.mClips[3], 0.2f, false);
      Vector3 translation = iOwner.mOrientation.Translation;
      Vector3 forward = iOwner.mOrientation.Forward;
      EffectManager.Instance.StartEffect(Death.DEFEATED_EFFECT, ref translation, ref forward, out this.e);
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      if (!iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
        return;
      iOwner.ChangeState(Death.States.Defeat);
    }

    public void OnExit(Death iOwner) => EffectManager.Instance.Stop(ref this.e);
  }

  public class PreMultiDeathSweepingCircleState : IBossState<Death>
  {
    private float mSpawnTimer;
    private int mCount;

    public void OnEnter(Death iOwner)
    {
      iOwner.mBody.IsEthereal = true;
      iOwner.mDraw = true;
      iOwner.KillSkeletons();
      for (int iPlayerIndex = 0; iPlayerIndex < 4; ++iPlayerIndex)
      {
        if ((iOwner.mTarget as Avatar).Player.ID != iPlayerIndex)
        {
          if (iOwner.mPlayers[iPlayerIndex].Playing)
            iOwner.mPlayers[iPlayerIndex].Avatar.IsEthereal = true;
          ControlManager.Instance.LockPlayerInput(iPlayerIndex);
        }
      }
      iOwner.mCloneController.StartClip(iOwner.mClips[0], false);
      iOwner.mUpdate = false;
      iOwner.Movement = Vector3.Zero;
      Vector3 translation = iOwner.mOrientation.Translation;
      Vector3 position = iOwner.mTarget.Position;
      Vector3 result;
      Vector3.Subtract(ref position, ref translation, out result);
      result.Y = 0.0f;
      float z = result.Length();
      iOwner.mMultiDeathOrientation = iOwner.mOrientation;
      iOwner.mMultiDeathOffset = new Vector3(0.0f, 0.0f, z);
      this.mSpawnTimer = 0.0f;
      iOwner.mCurrentMaxDeaths = (double) iOwner.mHitPoints / (double) iOwner.MaxHitPoints <= 0.66666668653488159 ? ((double) iOwner.mHitPoints / (double) iOwner.MaxHitPoints <= 0.3333333432674408 ? 16 /*0x10*/ : 12) : 8;
      this.mCount = 1;
      iOwner.mIsVisibleMultiDeath[0] = true;
      AudioManager.Instance.PlayCue(Banks.Misc, Death.SOUND_HASH);
      Flash.Instance.Execute(iOwner.mPlayState.Scene, 0.2f);
      iOwner.mPlayState.Camera.Magnification = 1.2f;
      iOwner.mPlayState.Camera.AttachPlayers(iOwner.mTarget);
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      if (iOwner.mTarget == null | iOwner.mTarget.Dead)
      {
        iOwner.ChangeState(Death.States.Prep);
      }
      else
      {
        this.mSpawnTimer += iDeltaTime;
        bool flag = false;
        if ((double) this.mSpawnTimer > 1.0)
          iOwner.ChangeState(Death.States.MultiDeath);
        if ((double) this.mSpawnTimer / 1.0 >= (double) this.mCount / (double) iOwner.mCurrentMaxDeaths & this.mCount < iOwner.mCurrentMaxDeaths)
        {
          flag = true;
          iOwner.mIsVisibleMultiDeath[this.mCount] = true;
          ++this.mCount;
        }
        Matrix identity = Matrix.Identity;
        iOwner.mCloneController.Update(iDeltaTime, ref identity, true);
        Vector3 result1 = iOwner.mMultiDeathOffset;
        Matrix result2 = iOwner.mOrientation;
        Vector3.TransformNormal(ref result1, ref result2, out result1);
        float num = Math.Min(this.mSpawnTimer / 1f, 1f);
        Matrix result3;
        Matrix.CreateRotationY((float) (1.0 / (double) iOwner.mCurrentMaxDeaths * 6.2831854820251465) * num, out result3);
        for (int index1 = 0; index1 < iOwner.mCurrentMaxDeaths; ++index1)
        {
          Matrix result4 = iOwner.mHandBindPose;
          result2.Translation = new Vector3();
          Matrix.Multiply(ref result3, ref result2, out result2);
          Vector3.TransformNormal(ref result1, ref result3, out result1);
          result2.Translation = result1 + iOwner.mTarget.Position;
          if (flag)
          {
            Vector3 translation = iOwner.mMultiDeathOrientation.Translation;
            Vector3 right = Vector3.Right;
            EffectManager.Instance.StartEffect(Death.ETHEREAL_SPAWN_EFFECT, ref translation, ref right, out VisualEffectReference _);
            flag = false;
          }
          for (int index2 = 0; index2 < iOwner.mCloneController.Skeleton.Count; ++index2)
            Matrix.Multiply(ref iOwner.mCloneController.SkinnedBoneTransforms[index2], ref result2, out iOwner.mRenderData[(int) iOwner.mCurrentChannel].mBones[index1][index2]);
          Matrix.Multiply(ref result4, ref iOwner.mRenderData[(int) iOwner.mCurrentChannel].mBones[index1][iOwner.mHandIndex], out result4);
          iOwner.mScytheRenderData[(int) iOwner.mCurrentChannel].WorldOrientation[index1] = result4;
        }
      }
    }

    public void OnExit(Death iOwner)
    {
    }
  }

  public class PreMultiDeathState : IBossState<Death>
  {
    private float mSpawnTimer;

    public void OnEnter(Death iOwner)
    {
      iOwner.mBody.IsEthereal = true;
      iOwner.mDraw = true;
      iOwner.SelectTarget();
      iOwner.KillSkeletons();
      iOwner.mPlayState.TimeModifier = Death.TIME_SLOW_DOWN;
      iOwner.mTarget.TimeWarpModifier = 1f / Death.TIME_SLOW_DOWN;
      for (int iPlayerIndex = 0; iPlayerIndex < 4; ++iPlayerIndex)
      {
        if ((iOwner.mTarget as Avatar).Player.ID != iPlayerIndex)
        {
          if (iOwner.mPlayers[iPlayerIndex].Playing)
            iOwner.mPlayers[iPlayerIndex].Avatar.IsEthereal = true;
          ControlManager.Instance.LockPlayerInput(iPlayerIndex);
        }
      }
      iOwner.mCloneController.StartClip(iOwner.mClips[0], false);
      iOwner.mUpdate = false;
      iOwner.Movement = Vector3.Zero;
      Vector3 translation = iOwner.mOrientation.Translation;
      Vector3 position = iOwner.mTarget.Position;
      Vector3 result;
      Vector3.Subtract(ref position, ref translation, out result);
      result.Y = 0.0f;
      float z = result.Length();
      iOwner.mMultiDeathOrientation = iOwner.mOrientation;
      iOwner.mMultiDeathOffset = new Vector3(0.0f, 0.0f, z);
      this.mSpawnTimer = 0.0f;
      iOwner.mCurrentMaxDeaths = (double) iOwner.mHitPoints / (double) iOwner.MaxHitPoints <= 0.66666668653488159 ? ((double) iOwner.mHitPoints / (double) iOwner.MaxHitPoints <= 0.3333333432674408 ? 16 /*0x10*/ : 12) : 8;
      for (int index = 0; index < iOwner.mCurrentMaxDeaths; ++index)
        iOwner.mIsVisibleMultiDeath[index] = true;
      AudioManager.Instance.PlayCue(Banks.Misc, Death.SOUND_HASH);
      Flash.Instance.Execute(iOwner.mPlayState.Scene, 0.2f);
      iOwner.mPlayState.Camera.Time = 0.0f;
      iOwner.mPlayState.Camera.Magnification = 1.2f;
      iOwner.mPlayState.Camera.AttachPlayers(iOwner.mTarget);
      this.mSpawnTimer = 1f;
      iOwner.mAlphaFadeSteps = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      int num1 = iOwner.mBody.IsEthereal ? 1 : 0;
      if (iOwner.mTarget == null | iOwner.mTarget.Dead)
      {
        iOwner.ChangeState(Death.States.Prep);
      }
      else
      {
        if (iOwner.mCloneController.HasFinished && !iOwner.mCloneController.CrossFadeEnabled)
          iOwner.ChangeState(Death.States.MultiDeath);
        iOwner.mTarget.CharacterBody.SpeedMultiplier *= 1f / Death.TIME_SLOW_DOWN;
        Matrix identity = Matrix.Identity;
        MagickaMath.UniformMatrixScale(ref identity, 1.2f);
        iOwner.mCloneController.Update(iDeltaTime, ref identity, true);
        Vector3 result1 = iOwner.mMultiDeathOffset;
        Matrix result2 = iOwner.mOrientation;
        Vector3.TransformNormal(ref result1, ref result2, out result1);
        float num2 = Math.Min(this.mSpawnTimer / 1f, 1f);
        Matrix result3;
        Matrix.CreateRotationY((float) (1.0 / (double) iOwner.mCurrentMaxDeaths * 6.2831854820251465) * num2, out result3);
        for (int index1 = 0; index1 < iOwner.mCurrentMaxDeaths; ++index1)
        {
          Matrix result4 = iOwner.mHandBindPose;
          result2.Translation = new Vector3();
          Matrix.Multiply(ref result3, ref result2, out result2);
          Vector3.TransformNormal(ref result1, ref result3, out result1);
          result2.Translation = result1 + iOwner.mTarget.Position;
          for (int index2 = 0; index2 < iOwner.mCloneController.Skeleton.Count; ++index2)
            Matrix.Multiply(ref iOwner.mCloneController.SkinnedBoneTransforms[index2], ref result2, out iOwner.mRenderData[(int) iOwner.mCurrentChannel].mBones[index1][index2]);
          Matrix.Multiply(ref result4, ref iOwner.mRenderData[(int) iOwner.mCurrentChannel].mBones[index1][iOwner.mHandIndex], out result4);
          iOwner.mScytheRenderData[(int) iOwner.mCurrentChannel].WorldOrientation[index1] = result4;
        }
      }
    }

    public void OnExit(Death iOwner)
    {
    }
  }

  public class MultiDeathState : IBossState<Death>
  {
    private float mAttackTime;
    private int mAttacker;
    private bool mIsAttacking;
    private bool mIsWithinReach;
    private bool mScytheRaised;
    private int mNrOfAttacks;
    public bool mIsFakeAttack;

    public unsafe void OnEnter(Death iOwner)
    {
      iOwner.mCloneController.CrossFade(iOwner.mClips[6], 0.2f, true);
      iOwner.Movement = Vector3.Zero;
      if (NetworkManager.Instance.State != NetworkState.Client)
      {
        iOwner.mIsRealDeath = Death.sRandom.Next(iOwner.mCurrentMaxDeaths);
        if (NetworkManager.Instance.State == NetworkState.Server)
          BossFight.Instance.SendMessage<Death.MultiDeathMessage>((IBoss) iOwner, (ushort) 5, (void*) &new Death.MultiDeathMessage()
          {
            RealDeath = (byte) iOwner.mIsRealDeath
          }, true);
      }
      this.mNrOfAttacks = 0;
      this.mAttacker = -1;
      this.mAttackTime = 0.0f;
      this.mIsAttacking = false;
      this.mIsWithinReach = false;
      iOwner.mUpdate = false;
      this.mScytheRaised = false;
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      if (this.mNrOfAttacks >= iOwner.mCurrentMaxDeaths | iOwner.mTarget == null | iOwner.mTarget.Dead)
      {
        iOwner.ChangeState(Death.States.Prep);
      }
      else
      {
        if ((double) iOwner.mSpeed <= 0.10000000149011612)
          iOwner.mSpeed = (float) (5.0 * (2.0 - (double) iOwner.mHitPoints / 10000.0));
        this.mAttackTime += iDeltaTime;
        iOwner.mTarget.CharacterBody.SpeedMultiplier *= 1f / Death.TIME_SLOW_DOWN;
        if (!this.mIsAttacking && !this.mIsWithinReach)
        {
          this.mIsAttacking = true;
          while (this.mAttacker == -1 || !iOwner.mIsVisibleMultiDeath[this.mAttacker])
            this.mAttacker = Death.sRandom.Next(iOwner.mCurrentMaxDeaths);
          this.mIsFakeAttack = this.mAttacker != iOwner.mIsRealDeath;
          iOwner.mController.StartClip(iOwner.mCloneController.AnimationClip, true);
          iOwner.mController.Time = iOwner.mCloneController.Time;
          iOwner.mCloneController.LocalBonePoses.CopyTo((Array) iOwner.mController.LocalBonePoses, 0);
          iOwner.mController.CrossFade(iOwner.mClips[6], 0.2f, true);
          Vector3 result1 = iOwner.mMultiDeathOffset;
          Matrix result2 = iOwner.mMultiDeathOrientation;
          Vector3.TransformNormal(ref result1, ref result2, out result1);
          Matrix result3;
          Matrix.CreateRotationY((float) ((double) (this.mAttacker + 1) * (1.0 / (double) iOwner.mCurrentMaxDeaths) * 6.2831854820251465), out result3);
          result2.Translation = new Vector3();
          Matrix.Multiply(ref result3, ref result2, out result2);
          Vector3.TransformNormal(ref result1, ref result3, out result1);
          result2.Translation = result1 + iOwner.mTarget.Position;
          iOwner.mOrientation = result2;
          iOwner.mControllerSkeletonIndex = this.mAttacker;
          iOwner.mUpdate = true;
          for (int index1 = 0; index1 < 3; ++index1)
          {
            Death.AfterImageRenderData afterImageRenderData = iOwner.mAfterImageRenderData[index1];
            for (int index2 = 0; index2 < afterImageRenderData.mSkeleton.Length; ++index2)
              iOwner.mRenderData[(int) iOwner.mCurrentChannel].mBones[this.mAttacker].CopyTo((Array) afterImageRenderData.mSkeleton[index2], 0);
          }
        }
        Matrix identity = Matrix.Identity;
        MagickaMath.UniformMatrixScale(ref identity, 1.2f);
        iOwner.mCloneController.Update(iDeltaTime, ref identity, true);
        Vector3 result4 = iOwner.mMultiDeathOffset;
        Matrix result5 = iOwner.mMultiDeathOrientation;
        Vector3.TransformNormal(ref result4, ref result5, out result4);
        Matrix result6;
        Matrix.CreateRotationY((float) (1.0 / (double) iOwner.mCurrentMaxDeaths * 6.2831854820251465), out result6);
        for (int index3 = 0; index3 < iOwner.mCurrentMaxDeaths; ++index3)
        {
          Matrix result7 = iOwner.mHandBindPose;
          result5.Translation = new Vector3();
          Matrix.Multiply(ref result6, ref result5, out result5);
          Vector3.TransformNormal(ref result4, ref result6, out result4);
          result5.Translation = result4 + iOwner.mTarget.Position;
          if (this.mAttacker != index3)
          {
            for (int index4 = 0; index4 < iOwner.mCloneController.Skeleton.Count; ++index4)
              Matrix.Multiply(ref iOwner.mCloneController.SkinnedBoneTransforms[index4], ref result5, out iOwner.mRenderData[(int) iOwner.mCurrentChannel].mBones[index3][index4]);
            Matrix.Multiply(ref result7, ref iOwner.mRenderData[(int) iOwner.mCurrentChannel].mBones[index3][iOwner.mHandIndex], out result7);
            iOwner.mScytheRenderData[(int) iOwner.mCurrentChannel].WorldOrientation[index3] = result7;
          }
          else if (this.mIsWithinReach)
          {
            if (!this.mIsFakeAttack)
            {
              iOwner.mBody.IsEthereal = false;
              for (int index5 = 0; index5 < 4; ++index5)
              {
                if (iOwner.mPlayers[index5].Playing)
                  iOwner.mPlayers[index5].Avatar.IsEthereal = false;
              }
              for (int index6 = 0; index6 < iOwner.mCurrentMaxDeaths; ++index6)
              {
                if (index6 != this.mAttacker)
                  iOwner.mIsVisibleMultiDeath[index6] = false;
              }
            }
            Vector3 position = iOwner.mTarget.Position;
            Vector3 translation1 = iOwner.mOrientation.Translation;
            Vector3 result8;
            Vector3.Subtract(ref position, ref translation1, out result8);
            result8.Y = 0.0f;
            float num = result8.Length();
            result8.Normalize();
            if ((double) iOwner.mTarget.Radius + (double) iOwner.mBody.Radius < (double) num)
            {
              float iTurnSpeed = (float) ((double) num / 2.5 * 5.0 * (2.0 - (double) iOwner.mHitPoints / 10000.0));
              iOwner.mSpeed = iTurnSpeed;
              iOwner.Movement = result8;
              if ((double) iTurnSpeed < 8.0)
                iTurnSpeed = 8f;
              iOwner.Turn(ref result8, iTurnSpeed, iDeltaTime);
            }
            else
              iOwner.mSpeed = 0.1f;
            if (iOwner.mController.HasFinished && this.mIsFakeAttack)
            {
              Vector3 translation2 = iOwner.mOrientation.Translation;
              Vector3 right = Vector3.Right;
              EffectManager.Instance.StartEffect(Death.ETHEREAL_SPAWN_EFFECT, ref translation2, ref right, out VisualEffectReference _);
              iOwner.mIsVisibleMultiDeath[this.mAttacker] = false;
              this.mIsAttacking = false;
              this.mIsWithinReach = false;
              this.mAttackTime = 0.0f;
              iOwner.mUpdate = false;
              iOwner.Movement = Vector3.Zero;
              iOwner.mSpeed = (float) (5.0 * (2.0 - (double) iOwner.mHitPoints / 10000.0));
              ++this.mNrOfAttacks;
            }
            if (iOwner.mController.HasFinished && !iOwner.mController.CrossFadeEnabled && !this.mIsFakeAttack)
            {
              if (!this.mScytheRaised)
              {
                iOwner.mController.CrossFade(iOwner.mClips[1], 0.2f, false);
                this.mScytheRaised = true;
              }
              else
                iOwner.ChangeState(Death.States.Prep);
            }
          }
          if (this.mIsAttacking)
          {
            Vector3 position = iOwner.mTarget.Position;
            Vector3 translation = iOwner.mOrientation.Translation;
            Vector3 result9;
            Vector3.Subtract(ref position, ref translation, out result9);
            result9.Y = 0.0f;
            float num = result9.Length();
            result9.Normalize();
            iOwner.Movement = result9;
            iOwner.Turn(ref result9, 8f, iDeltaTime);
            if ((double) num < 2.5)
            {
              this.mIsAttacking = false;
              this.mIsWithinReach = true;
              iOwner.mController.CrossFade(iOwner.mClips[2], 0.2f, false);
              iOwner.Movement = Vector3.Zero;
            }
          }
        }
      }
    }

    public void OnExit(Death iOwner)
    {
      iOwner.Movement = Vector3.Zero;
      iOwner.mDraw = true;
      iOwner.mIsVisibleMultiDeath[0] = true;
      iOwner.mPlayState.TimeModifier = 1f;
      iOwner.mTarget.CharacterBody.SpeedMultiplier = 1f;
      ControlManager.Instance.UnlockPlayerInput();
      iOwner.mPlayState.Camera.Release(0.5f, false);
      for (int index = 0; index < 4; ++index)
      {
        if (iOwner.mPlayers[index].Playing)
          iOwner.mPlayers[index].Avatar.IsEthereal = false;
      }
      for (int index = 1; index < iOwner.mIsVisibleMultiDeath.Length; ++index)
      {
        int num = iOwner.mIsVisibleMultiDeath[index] ? 1 : 0;
        iOwner.mIsVisibleMultiDeath[index] = false;
      }
      iOwner.mController.Speed = 1f;
      iOwner.mUpdate = true;
      iOwner.mPreviousState = iOwner.mCurrentState;
      iOwner.mControllerSkeletonIndex = 0;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        Death.AfterImageRenderData afterImageRenderData = iOwner.mAfterImageRenderData[index1];
        for (int index2 = 0; index2 < afterImageRenderData.mSkeleton.Length; ++index2)
          iOwner.mEmptyMatrix.CopyTo((Array) afterImageRenderData.mSkeleton[index2], 0);
      }
      iOwner.mIsRealDeath = 0;
    }
  }

  public class WarpTimeState : IBossState<Death>
  {
    public void OnEnter(Death iOwner)
    {
      iOwner.mDraw = true;
      iOwner.mController.CrossFade(iOwner.mClips[11], 0.2f, false);
      iOwner.Movement = Vector3.Zero;
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      if (NetworkManager.Instance.State == NetworkState.Client || !iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
        return;
      new Magick() { MagickType = MagickType.TimeWarp }.Effect.Execute(iOwner.mOrientation.Translation, iOwner.mPlayState);
      iOwner.ChangeState(Death.States.Prep);
    }

    public void OnExit(Death iOwner)
    {
      iOwner.mController.Speed = 1f;
      iOwner.mPreviousState = iOwner.mCurrentState;
    }
  }

  public class AppearState : IBossState<Death>
  {
    public void OnEnter(Death iOwner)
    {
      iOwner.mBody.IsEthereal = true;
      iOwner.mDraw = true;
      iOwner.mController.StartClip(iOwner.mClips[0], false);
      iOwner.Movement = Vector3.Zero;
      iOwner.mAppearDuration = iOwner.mClips[0].Duration;
      iOwner.mAlphaFadeSteps = 0.0f;
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      if (!iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
        return;
      iOwner.ChangeState(iOwner.mNextState);
    }

    public void OnExit(Death iOwner) => iOwner.mBody.IsEthereal = false;
  }

  public class RaiseDeadState : IBossState<Death>
  {
    private float time;

    public void OnEnter(Death iOwner)
    {
      iOwner.mDraw = true;
      iOwner.mController.Speed = MathHelper.Lerp(2f, 0.65f, (float) ((double) iOwner.mHitPoints / 10000.0 - 1.0));
      iOwner.mController.CrossFade(iOwner.mClips[10], 0.2f, false);
      iOwner.Movement = Vector3.Zero;
      this.time = 0.2f;
    }

    public void OnUpdate(float iDeltaTime, Death iOwner)
    {
      int num1 = Death.MAX_SKELETONS - iOwner.mAnyArea.GetCount(Death.SKELETON1) - iOwner.mAnyArea.GetCount(Death.SKELETON2) - iOwner.mAnyArea.GetCount(Death.SKELETON3) - iOwner.mAnyArea.GetCount(Death.SKELETON4);
      this.time -= iDeltaTime;
      if ((double) this.time <= 0.0)
      {
        if (num1 > 0)
        {
          iOwner.RaiseDead(1, iOwner);
          int num2 = num1 + 1;
        }
        this.time = 0.2f;
      }
      if (!iOwner.mController.HasFinished || iOwner.mController.CrossFadeEnabled)
        return;
      iOwner.ChangeState(Death.States.Prep);
    }

    public void OnExit(Death iOwner)
    {
      iOwner.mController.Speed = 1f;
      iOwner.mPreviousState = iOwner.mCurrentState;
    }
  }

  private enum MessageType : ushort
  {
    Update,
    ChangeState,
    ChangeTarget,
    Spawn,
    ClearSkeletons,
    MultiDeath,
    Ethereal,
  }

  internal struct UpdateMessage
  {
    public const ushort TYPE = 0;
    public Matrix Orientation;
    public byte Animation;
    public float AnimationTime;
    public float Hitpoints;
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct ClearMessage
  {
    public const ushort TYPE = 4;
  }

  internal struct SpawnMessage
  {
    public const ushort TYPE = 3;
    public ushort Handle;
    public int TypeID;
    public Vector3 Position;
    public Vector3 Direction;
  }

  internal struct ChangeStateMessage
  {
    public const ushort TYPE = 1;
    public Death.States NewState;
  }

  internal struct ChangeTargetMessage
  {
    public const ushort TYPE = 2;
    public ushort Target;
  }

  internal struct MultiDeathMessage
  {
    public const ushort TYPE = 5;
    public byte RealDeath;
  }

  public enum States
  {
    Idle,
    SitIntro,
    RiseIntro,
    Prep,
    Attack,
    Move,
    Hit,
    Defeat,
    DefeatPreSit,
    WarpTime,
    RaiseDead,
    MultiDeath,
    PreMultiDeath,
    Appear,
  }

  private enum Animations
  {
    appear,
    attack_scythe_fall,
    attack_scythe_rise,
    defeated,
    hit,
    idle,
    move_glide,
    sit,
    sit_talk,
    summon_scythe,
    summon_skeleton,
    timewarp,
    NrOfAnimations,
  }
}
