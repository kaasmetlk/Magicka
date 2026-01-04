// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.Tentacle
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
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

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class Tentacle : Entity, IStatusEffected, IDamageable
{
  private const float NETWORK_UPDATE_PERIOD = 0.05f;
  internal const int NR_OF_SPINE_PARTS = 11;
  public const int MAX_RANGE_FOR_GOOD_SPOT = 18;
  public const int CRUSH_RADIUS_RELATIVE_HEAD = 2;
  private static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);
  private float mLastNetworkUpdate;
  protected float mNetworkUpdateTimer;
  private static readonly float[] SQUEEZE_DAMAGE_TIME = new float[2]
  {
    0.4f,
    0.7f
  };
  private static readonly float[] CRUSH_DAMAGE_TIME = new float[2]
  {
    0.208333328f,
    0.5f
  };
  private static readonly float[] GRAB_TIME = new float[2]
  {
    0.178571433f,
    0.321428567f
  };
  private Tentacle.States mPreviousState;
  private Tentacle.States mCurrentState;
  private Tentacle.RenderData[] mRenderData;
  private Matrix mTransform;
  private AnimationClip[] mAnimations;
  private AnimationController mAnimationController;
  private IDamageable mGrabbed;
  private Magicka.GameLogic.Damage mSqueezeDamage;
  private Magicka.GameLogic.Damage mCrushDamage;
  private Magicka.GameLogic.Damage mSwipeDamage;
  private HitList mHitList;
  private int mSpawnPoint;
  private int mGrabAttachIndex;
  private Matrix mGrabAttachBindPose;
  private int[] mSpineIndex = new int[11];
  private Matrix[] mSpineBindPose = new Matrix[11];
  private Matrix mGrabTransform;
  private Matrix mTransformedTopJoint;
  private Cthulhu mOwner;
  private byte mID;
  private Avatar mCrushTarget;
  private Vector3 mOriginalForwardDirection;
  private bool mSearchForTarget;
  private int mSecondsWhileOutOfRange;
  private float mCheckEnemiesInRangeTimer;
  private bool mCthulhuKilled;
  private float mHitFlashTimer;
  private float mHitPoints;
  private float mMaxHitPoints;
  private float mWetnessCounter;
  private float mHPOnLastEmerge;
  private float mTimeUntilSubmerge;
  private float mTimeSinceLastEmerge;
  private float mTentacleLength;
  private VisualEffectReference mIdleEffectRef;
  private int mIdleEffect = "cthulhu_tentacle_water_surface".GetHashCodeCustom();
  private StatusEffects mCurrentStatusEffects;
  private TentacleStatusEffect[] mStatusEffects = new TentacleStatusEffect[9];
  private Resistance[] mResistances = new Resistance[11];
  private int mLastDamageIndex;
  private float mLastDamageAmount;
  private Elements mLastDamageElement;
  private float mTimeSinceLastDamage;
  private float mTimeSinceLastStatusDamage;
  private float mDryTimer;
  private Tentacle.TentacleState[] mStates = new Tentacle.TentacleState[11]
  {
    null,
    (Tentacle.TentacleState) new Tentacle.IdleState(),
    (Tentacle.TentacleState) new Tentacle.EmergeState(),
    (Tentacle.TentacleState) new Tentacle.SubmergeState(),
    (Tentacle.TentacleState) new Tentacle.SwipeGrabState(),
    (Tentacle.TentacleState) new Tentacle.GrabSuccessState(),
    (Tentacle.TentacleState) new Tentacle.SqueezeState(),
    (Tentacle.TentacleState) new Tentacle.ReleaseState(),
    (Tentacle.TentacleState) new Tentacle.CrushState(),
    (Tentacle.TentacleState) new Tentacle.AimState(),
    (Tentacle.TentacleState) new Tentacle.DieState()
  };
  private static readonly int SOUND_CRUSH = "cthulhu_tentacle_crush".GetHashCodeCustom();
  private static readonly int SOUND_EMERGE = "cthulhu_tentacle_emerge".GetHashCodeCustom();
  private static readonly int SOUND_GRAB_SUCCESS = "cthulhu_tentacle_grab_success".GetHashCodeCustom();
  private static readonly int SOUND_RELEASE = "cthulhu_tentacle_release".GetHashCodeCustom();
  private static readonly int SOUND_SQUEEZE = "cthulhu_tentacle_squeeze".GetHashCodeCustom();
  private static readonly int SOUND_SUBMERGE = "cthulhu_tentacle_submerge".GetHashCodeCustom();
  private static readonly int SOUND_SWIPE_GRAB = "cthulhu_tentacle_swipe_grab".GetHashCodeCustom();

  private float WaterYPos => this.mOwner != null ? this.mOwner.WaterYpos : -2f;

  public Tentacle(Cthulhu iOwner, byte iID, int iDamageZoneIndex, PlayState iPlayState)
    : base(iPlayState)
  {
    this.mOwner = iOwner;
    this.mID = iID;
    this.mPlayState = iPlayState;
    if (!(RenderManager.Instance.GetEffect(SkinnedModelDeferredNormalMappedEffect.TYPEHASH) is SkinnedModelDeferredNormalMappedEffect))
    {
      SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool = RenderManager.Instance.GlobalDummyEffect.EffectPool;
      RenderManager.Instance.RegisterEffect((Effect) new SkinnedModelDeferredNormalMappedEffect(Magicka.Game.Instance.GraphicsDevice, SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool));
    }
    SkinnedModel skinnedModel1 = (SkinnedModel) null;
    SkinnedModel skinnedModel2 = (SkinnedModel) null;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      skinnedModel1 = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Cthulhu/Tentacle_mesh");
      skinnedModel2 = iPlayState.Content.Load<SkinnedModel>("Models/Bosses/Cthulhu/Tentacle_animations");
    }
    this.mAnimationController = new AnimationController();
    this.mAnimationController.Skeleton = skinnedModel2.SkeletonBones;
    this.mAnimations = new AnimationClip[9];
    this.mAnimations[0] = skinnedModel2.AnimationClips["idle"];
    this.mAnimations[6] = skinnedModel2.AnimationClips["grip"];
    this.mAnimations[7] = skinnedModel2.AnimationClips["grip_success"];
    this.mAnimations[1] = skinnedModel2.AnimationClips["squeeze"];
    this.mAnimations[2] = skinnedModel2.AnimationClips["crush"];
    this.mAnimations[8] = skinnedModel2.AnimationClips["let_go"];
    this.mAnimations[5] = skinnedModel2.AnimationClips["die"];
    this.mAnimations[3] = skinnedModel2.AnimationClips["emerge"];
    this.mAnimations[4] = skinnedModel2.AnimationClips["submerge0"];
    this.mRenderData = new Tentacle.RenderData[3];
    for (int index = 0; index < this.mRenderData.Length; ++index)
    {
      this.mRenderData[index] = new Tentacle.RenderData(skinnedModel2.SkeletonBones.Count, skinnedModel1.Model.Meshes[0], skinnedModel1.Model.Meshes[0].MeshParts[0]);
      this.mRenderData[index].BoundingSphere.Radius = skinnedModel1.Model.Meshes[0].BoundingSphere.Radius;
    }
    Matrix result1;
    Matrix.CreateRotationY(3.14159274f, out result1);
    for (int index = 0; index < skinnedModel2.SkeletonBones.Count; ++index)
    {
      SkinnedModelBone skeletonBone = skinnedModel2.SkeletonBones[index];
      if (skeletonBone.Name.Equals("joint24", StringComparison.OrdinalIgnoreCase))
      {
        this.mGrabAttachIndex = (int) skeletonBone.Index;
        this.mGrabAttachBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mGrabAttachBindPose, ref result1, out this.mGrabAttachBindPose);
        Matrix.Invert(ref this.mGrabAttachBindPose, out this.mGrabAttachBindPose);
      }
      else if (skeletonBone.Name.Equals("joint12", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[0] = (int) skeletonBone.Index;
        this.mSpineBindPose[0] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[0], ref result1, out this.mSpineBindPose[0]);
        Matrix.Invert(ref this.mSpineBindPose[0], out this.mSpineBindPose[0]);
      }
      else if (skeletonBone.Name.Equals("joint2", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[1] = (int) skeletonBone.Index;
        this.mSpineBindPose[1] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[1], ref result1, out this.mSpineBindPose[1]);
        Matrix.Invert(ref this.mSpineBindPose[1], out this.mSpineBindPose[1]);
      }
      else if (skeletonBone.Name.Equals("joint3", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[2] = (int) skeletonBone.Index;
        this.mSpineBindPose[2] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[2], ref result1, out this.mSpineBindPose[2]);
        Matrix.Invert(ref this.mSpineBindPose[2], out this.mSpineBindPose[2]);
      }
      else if (skeletonBone.Name.Equals("joint4", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[3] = (int) skeletonBone.Index;
        this.mSpineBindPose[3] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[3], ref result1, out this.mSpineBindPose[3]);
        Matrix.Invert(ref this.mSpineBindPose[3], out this.mSpineBindPose[3]);
      }
      else if (skeletonBone.Name.Equals("joint5", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[4] = (int) skeletonBone.Index;
        this.mSpineBindPose[4] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[4], ref result1, out this.mSpineBindPose[4]);
        Matrix.Invert(ref this.mSpineBindPose[4], out this.mSpineBindPose[4]);
      }
      else if (skeletonBone.Name.Equals("joint6", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[5] = (int) skeletonBone.Index;
        this.mSpineBindPose[5] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[5], ref result1, out this.mSpineBindPose[5]);
        Matrix.Invert(ref this.mSpineBindPose[5], out this.mSpineBindPose[5]);
      }
      else if (skeletonBone.Name.Equals("joint7", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[6] = (int) skeletonBone.Index;
        this.mSpineBindPose[6] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[6], ref result1, out this.mSpineBindPose[6]);
        Matrix.Invert(ref this.mSpineBindPose[6], out this.mSpineBindPose[6]);
      }
      else if (skeletonBone.Name.Equals("joint8", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[7] = (int) skeletonBone.Index;
        this.mSpineBindPose[7] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[7], ref result1, out this.mSpineBindPose[7]);
        Matrix.Invert(ref this.mSpineBindPose[7], out this.mSpineBindPose[7]);
      }
      else if (skeletonBone.Name.Equals("joint9", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[8] = (int) skeletonBone.Index;
        this.mSpineBindPose[8] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[8], ref result1, out this.mSpineBindPose[8]);
        Matrix.Invert(ref this.mSpineBindPose[8], out this.mSpineBindPose[8]);
      }
      else if (skeletonBone.Name.Equals("joint10", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[9] = (int) skeletonBone.Index;
        this.mSpineBindPose[9] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[9], ref result1, out this.mSpineBindPose[9]);
        Matrix.Invert(ref this.mSpineBindPose[9], out this.mSpineBindPose[9]);
      }
      else if (skeletonBone.Name.Equals("joint11", StringComparison.OrdinalIgnoreCase))
      {
        this.mSpineIndex[10] = (int) skeletonBone.Index;
        this.mSpineBindPose[10] = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mSpineBindPose[10], ref result1, out this.mSpineBindPose[10]);
        Matrix.Invert(ref this.mSpineBindPose[10], out this.mSpineBindPose[10]);
      }
    }
    this.mRadius = 0.8f;
    Primitive[] primitiveArray = new Primitive[this.mSpineBindPose.Length - 1];
    for (int index = 0; index < this.mSpineBindPose.Length - 1; ++index)
    {
      Vector3 translation1 = this.mSpineBindPose[index].Translation;
      Vector3 translation2 = this.mSpineBindPose[index + 1].Translation;
      float result2;
      Vector3.Distance(ref translation1, ref translation2, out result2);
      primitiveArray[index] = (Primitive) new Capsule(Vector3.Zero, Matrix.Identity, this.mRadius, result2);
    }
    this.mTentacleLength = Vector3.Distance(this.mSpineBindPose[0].Translation, this.mSpineBindPose[this.mSpineBindPose.Length - 1].Translation);
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    for (int index = 0; index < primitiveArray.Length; ++index)
      this.mCollision.AddPrimitive(primitiveArray[index], 1, new MaterialProperties(0.0f, 0.0f, 0.0f));
    this.mCollision.ApplyLocalTransform(Transform.Identity);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.Immovable = true;
    this.mBody.CollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mBody.Tag = (object) this;
    this.mBody.MoveTo(new Vector3(), Matrix.Identity);
    this.mMaxHitPoints = 800f;
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements elements = Spell.ElementFromIndex(iIndex);
      this.mResistances[iIndex].Modifier = 0.0f;
      this.mResistances[iIndex].Multiplier = 1f;
      this.mResistances[iIndex].ResistanceAgainst = elements;
      switch (elements)
      {
        case Elements.Water:
          this.mResistances[iIndex].Multiplier = 0.0f;
          break;
        case Elements.Poison:
          this.mResistances[iIndex].Multiplier = 0.0f;
          break;
      }
    }
    this.mHitList = new HitList(8);
    this.mSqueezeDamage = new Magicka.GameLogic.Damage(AttackProperties.Damage | AttackProperties.Bleed, Elements.Earth, 150f, 1f);
    this.mCrushDamage = new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth, 500f, 1f);
    this.mSwipeDamage = new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth, 150f, 1f);
    Vector3 pos = new Vector3(0.0f, (float) ((int) this.mID * 1000), 0.0f);
    Matrix identity = Matrix.Identity;
    this.mBody.MoveTo(ref pos, ref identity);
  }

  private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    return iSkin1.Owner == null || !(iSkin1.Owner.Tag is Magicka.GameLogic.Entities.Character);
  }

  private Vector3 NotifierTextPostion
  {
    get
    {
      Vector3 translation = this.mTransform.Translation;
      translation.Y += 3f;
      return translation;
    }
  }

  internal int ID => (int) this.mID;

  private void StartClip(Tentacle.Animations anim, bool loop)
  {
    this.mAnimationController.StartClip(this.mAnimations[(int) anim], loop);
  }

  private void StopClip() => this.mAnimationController.Stop();

  private void CrossFade(Tentacle.Animations anim, float time, bool loop)
  {
    this.mAnimationController.CrossFade(this.mAnimations[(int) anim], time, loop);
  }

  private bool IsPlaying(Tentacle.Animations anim)
  {
    return this.mAnimationController.AnimationClip == this.mAnimations[(int) anim];
  }

  private bool AnimationHasFinished
  {
    get => !this.mAnimationController.CrossFadeEnabled && this.mAnimationController.HasFinished;
  }

  public new void Initialize()
  {
    base.Initialize();
    this.mDead = true;
  }

  internal void Start(Matrix iTransform)
  {
    this.mTransform = iTransform;
    this.mOriginalForwardDirection = this.mTransform.Forward;
    this.mHitList.Clear();
    this.mHitPoints = this.mMaxHitPoints;
    this.mDead = false;
    if (this.mCurrentState == Tentacle.States.None)
    {
      this.mPreviousState = Tentacle.States.Idle;
      this.mCurrentState = Tentacle.States.Emerge;
      this.mStates[(int) this.mCurrentState].OnEnter(this);
    }
    else
      this.ChangeState(Tentacle.States.Emerge);
  }

  internal unsafe void ChangeState(Tentacle.States iState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (this.mCurrentState != Tentacle.States.None)
      this.mStates[(int) this.mCurrentState].OnExit(this);
    this.mPreviousState = this.mCurrentState;
    this.mCurrentState = iState;
    this.mStates[(int) this.mCurrentState].OnEnter(this);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    Cthulhu.TentacleChangeStateMessage changeStateMessage;
    changeStateMessage.NewState = iState;
    changeStateMessage.TentacleIndex = this.mID;
    BossFight.Instance.SendMessage<Cthulhu.TentacleChangeStateMessage>((IBoss) this.mOwner, (ushort) 12, (void*) &changeStateMessage, true);
  }

  internal void GrabDamageable(IDamageable iDamageable) => this.mGrabbed = iDamageable;

  internal void SetAimTarget(Avatar iAvatar)
  {
    this.mCrushTarget = iAvatar;
    if (this.mCrushTarget != null)
      return;
    this.mSearchForTarget = false;
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
    base.Update(iDataChannel, iDeltaTime);
    if ((double) this.mHitPoints <= 0.0 && !this.mCthulhuKilled && NetworkManager.Instance.State != NetworkState.Client && this.mCurrentState != Tentacle.States.Submerge && this.mCurrentState != Tentacle.States.Release)
    {
      if (this.mGrabbed == null)
        this.ChangeState(Tentacle.States.Submerge);
      else
        this.ChangeState(Tentacle.States.Release);
    }
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mNetworkUpdateTimer -= iDeltaTime;
      if ((double) this.mNetworkUpdateTimer <= 0.0)
      {
        this.mNetworkUpdateTimer = 0.05f;
        this.NetworkUpdate();
      }
    }
    if (this.mCurrentState == Tentacle.States.None)
      return;
    this.UpdateDamage(iDeltaTime);
    this.UpdateStatusEffects(iDeltaTime);
    this.mTimeUntilSubmerge -= iDeltaTime;
    this.mTimeSinceLastEmerge += iDeltaTime;
    this.mHitList.Update(iDeltaTime);
    this.mStates[(int) this.mCurrentState].OnUpdate(this, iDeltaTime);
    Matrix result1;
    Matrix.CreateScale(1f, out result1);
    Matrix.Multiply(ref this.mTransform, ref result1, out result1);
    result1.Translation = this.mTransform.Translation;
    this.mAnimationController.Update(this.mOwner.StageSpeedModifier * this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown() * iDeltaTime, ref result1, true);
    Matrix identity1 = Matrix.Identity;
    Matrix.Multiply(ref this.mGrabAttachBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mGrabAttachIndex], out this.mGrabTransform);
    Matrix.Multiply(ref this.mSpineBindPose[10], ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[10]], out this.mTransformedTopJoint);
    Vector3 translation1 = this.mTransform.Translation;
    Vector3 zero = Vector3.Zero;
    Vector3 up = Vector3.Up;
    for (int prim = 0; prim < 10; ++prim)
    {
      Vector3 result2 = this.mSpineBindPose[prim].Translation;
      Vector3.Transform(ref result2, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[prim]], out result2);
      Vector3 result3 = this.mSpineBindPose[prim + 1].Translation;
      Vector3.Transform(ref result3, ref this.mAnimationController.SkinnedBoneTransforms[this.mSpineIndex[prim + 1]], out result3);
      Vector3 result4;
      Vector3.Subtract(ref result3, ref result2, out result4);
      result4.Normalize();
      Transform iTransform;
      if ((double) Vector3.Cross(result4, up).LengthSquared() <= 9.9999999747524271E-07)
      {
        Vector3 right = Vector3.Right;
        Matrix.CreateWorld(ref zero, ref result4, ref right, out iTransform.Orientation);
      }
      else
        Matrix.CreateWorld(ref zero, ref result4, ref up, out iTransform.Orientation);
      Vector3.Subtract(ref result3, ref translation1, out iTransform.Position);
      this.mBody.CollisionSkin.GetPrimitiveLocal(prim).SetTransform(ref iTransform);
    }
    this.mBody.MoveTo(ref translation1, ref identity1);
    if (this.mGrabbed != null)
    {
      if (this.mGrabbed.Dead)
      {
        this.mGrabbed.OverKill();
        this.mGrabbed = (IDamageable) null;
      }
      else
      {
        Vector3 translation2 = this.mGrabTransform.Translation;
        Vector3 position = this.mGrabbed.Position;
        Vector3.DistanceSquared(ref translation2, ref position, out float _);
        Matrix identity2 = Matrix.Identity with
        {
          Translation = new Vector3()
        };
        this.mGrabbed.Body.MoveTo(ref translation2, ref identity2);
      }
    }
    Tentacle.RenderData renderData = this.mRenderData[(int) iDataChannel];
    float num = System.Math.Min(this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude * 10f, 1f);
    renderData.Colorize.X = Tentacle.ColdColor.X;
    renderData.Colorize.Y = Tentacle.ColdColor.Y;
    renderData.Colorize.Z = Tentacle.ColdColor.Z;
    renderData.Colorize.W = num;
    this.mHitFlashTimer = System.Math.Max(this.mHitFlashTimer - iDeltaTime * 5f, 0.0f);
    renderData.BoundingSphere.Center = this.mTransform.Translation;
    renderData.Damage = (float) (1.0 - (double) this.mOwner.HitPoints / (double) this.mOwner.MaxHitPoints);
    renderData.Flash = this.mHitFlashTimer;
    this.ApplyWetSpecularEffect(renderData, iDeltaTime);
    Vector3 vector3 = new Vector3(0.75f, 0.8f, 1f);
    Vector3 diffuseColor = renderData.Material.DiffuseColor;
    Vector3.Multiply(ref renderData.Material.DiffuseColor, ref vector3, out renderData.Material.DiffuseColor);
    Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, (Array) renderData.Skeleton, renderData.Skeleton.Length);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) renderData);
    renderData.Material.DiffuseColor = diffuseColor;
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
    this.CheckIfCharactersAreClose(iDeltaTime);
    if (!EffectManager.Instance.IsActive(ref this.mIdleEffectRef))
      return;
    Matrix translation3 = Matrix.CreateTranslation(this.mTransform.Translation with
    {
      Y = this.mOwner.WaterYpos
    });
    EffectManager.Instance.UpdateOrientation(ref this.mIdleEffectRef, ref translation3);
  }

  internal void TurnOnIdleEffect()
  {
    Matrix translation = Matrix.CreateTranslation(this.mTransform.Translation with
    {
      Y = this.mOwner.WaterYpos
    });
    EffectManager.Instance.StartEffect(this.mIdleEffect, ref translation, out this.mIdleEffectRef);
  }

  internal void KillIdleEffect() => EffectManager.Instance.Stop(ref this.mIdleEffectRef);

  private float GetColdSlowdown()
  {
    return this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown();
  }

  private void ApplyWetSpecularEffect(Tentacle.RenderData iRenderData, float iDeltaTime)
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

  private void CheckIfCharactersAreClose(float iDeltaTime)
  {
    this.mCheckEnemiesInRangeTimer -= iDeltaTime;
    if ((double) this.mCheckEnemiesInRangeTimer >= 0.0)
      return;
    if (this.CheckIfCharactersAreWithinRadius(this.mTentacleLength))
      this.mSecondsWhileOutOfRange = 0;
    else
      ++this.mSecondsWhileOutOfRange;
    this.mCheckEnemiesInRangeTimer = 1f;
  }

  private bool CheckIfCharactersAreWithinRadius(float iRadius)
  {
    EntityManager entityManager = this.mPlayState.EntityManager;
    Vector3 translation = this.mTransform.Translation;
    List<Entity> entities = entityManager.GetEntities(translation, iRadius, true);
    bool flag = false;
    for (int index = 0; index < entities.Count && !flag; ++index)
      flag = entities[index] is Avatar avatar && !avatar.IsEthereal && !avatar.IsInvisibile;
    entityManager.ReturnEntityList(entities);
    return flag;
  }

  internal void NetworkUpdate(ref Cthulhu.TentacleUpdateMessage iMsg, float iTimeStamp)
  {
    if ((double) iTimeStamp < (double) this.mLastNetworkUpdate)
      return;
    this.mLastNetworkUpdate = iTimeStamp;
    if (this.mAnimationController.AnimationClip != this.mAnimations[(int) iMsg.Animation])
      this.mAnimationController.StartClip(this.mAnimations[(int) iMsg.Animation], false);
    this.mAnimationController.Time = iMsg.AnimationTime.ToSingle();
  }

  internal void NetworkChangeState(ref Cthulhu.TentacleChangeStateMessage iMsg)
  {
    if (iMsg.NewState == Tentacle.States.NR_OF_STATES)
      return;
    this.mStates[(int) this.mCurrentState].OnExit(this);
    this.mCurrentState = iMsg.NewState;
    this.mStates[(int) this.mCurrentState].OnEnter(this);
  }

  internal void NetworkSpawnPoint(ref Cthulhu.SpawnPointMessage iMsg)
  {
    if (this.mSpawnPoint != -1)
      this.mSpawnPoint = iMsg.SpawnPoint;
    this.Start(this.mOwner.ChangeSpawnPoint(this.mSpawnPoint, (int) this.Handle));
  }

  private unsafe void NetworkUpdate()
  {
    NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
    Cthulhu.TentacleUpdateMessage tentacleUpdateMessage = new Cthulhu.TentacleUpdateMessage();
    tentacleUpdateMessage.Animation = (byte) 0;
    while ((int) tentacleUpdateMessage.Animation < this.mAnimations.Length && this.mAnimationController.AnimationClip != this.mAnimations[(int) tentacleUpdateMessage.Animation])
      ++tentacleUpdateMessage.Animation;
    tentacleUpdateMessage.TentacleIndex = this.mID;
    for (int index = 0; index < networkServer.Connections; ++index)
    {
      float num = networkServer.GetLatency(index) * 0.5f;
      BossFight.Instance.SendMessage<Cthulhu.TentacleUpdateMessage>((IBoss) this.mOwner, (ushort) 11, (void*) &(tentacleUpdateMessage with
      {
        AnimationTime = new HalfSingle(this.mAnimationController.Time + num)
      }), false, index);
    }
  }

  internal void KillTentacle()
  {
    if (this.mDead)
      return;
    this.mCthulhuKilled = true;
    this.mDead = true;
    this.mHitPoints = 0.0f;
    this.ChangeState(Tentacle.States.Die);
  }

  public override bool Dead => this.mDead;

  public override bool Removable => false;

  public override void Kill() => this.mHitPoints = 0.0f;

  internal override bool SendsNetworkUpdate(NetworkState iState) => false;

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
  }

  public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    return this.mCollision.SegmentIntersect(out float _, out oPosition, out Vector3 _, iSeg);
  }

  public override bool ArcIntersect(
    out Vector3 oPosition,
    Vector3 iOrigin,
    Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference)
  {
    oPosition = new Vector3();
    for (int prim = 0; prim < this.mCollision.NumPrimitives; ++prim)
    {
      Capsule primitiveNewWorld = this.mCollision.GetPrimitiveNewWorld(prim) as Capsule;
      Vector3 result1 = Vector3.Subtract(primitiveNewWorld.Position, primitiveNewWorld.GetEnd());
      Vector3.Multiply(ref result1, 0.5f, out result1);
      Vector3 vector3_1 = Vector3.Add(primitiveNewWorld.Position, result1);
      if ((double) System.Math.Abs(vector3_1.Y - iOrigin.Y) <= (double) iHeightDifference)
      {
        iOrigin.Y = 0.0f;
        iDirection.Y = 0.0f;
        Vector3 vector3_2 = vector3_1 with { Y = 0.0f };
        Vector3 result2;
        Vector3.Subtract(ref iOrigin, ref vector3_2, out result2);
        if ((double) result2.LengthSquared() > 9.9999999747524271E-07)
        {
          float num1 = result2.Length();
          float length = primitiveNewWorld.Length;
          if ((double) num1 - (double) length <= (double) iRange)
          {
            result2.Normalize();
            float result3;
            Vector3.Dot(ref result2, ref iDirection, out result3);
            result3 = -result3;
            float num2 = (float) System.Math.Acos((double) result3);
            float num3 = -2f * num1 * num1;
            float num4 = (float) System.Math.Acos(((double) length * (double) length + (double) num3) / (double) num3);
            if ((double) num2 - (double) num4 < (double) iAngle)
            {
              Vector3.Multiply(ref result2, length, out result2);
              Vector3 vector3_3 = vector3_1;
              Vector3.Add(ref vector3_3, ref result2, out oPosition);
              return true;
            }
          }
        }
      }
    }
    return false;
  }

  internal void Revive()
  {
    if ((double) this.mHitPoints > 0.0)
      return;
    this.mHitPoints = this.mMaxHitPoints;
  }

  public float HitPoints => this.mHitPoints;

  public float MaxHitPoints => this.mMaxHitPoints;

  public DamageResult InternalDamage(
    DamageCollection5 iDamages,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return DamageResult.None | this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
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
      if ((damage.Element & Elements.Fire) == Elements.Fire && (double) this.mResistances[Spell.ElementIndex(Elements.Fire)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new TentacleStatusEffect(StatusEffects.Burning, damage.Amount, damage.Magnitude, 1f, 1f));
      if ((damage.Element & Elements.Cold) == Elements.Cold && (double) this.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new TentacleStatusEffect(StatusEffects.Cold, damage.Amount, damage.Magnitude, 1f, 1f));
      if ((damage.Element & Elements.Water) == Elements.Water && (double) this.mResistances[Spell.ElementIndex(Elements.Water)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new TentacleStatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, 1f, 1f));
      if ((damage.Element & Elements.Steam) == Elements.Steam && (double) this.mResistances[Spell.ElementIndex(Elements.Steam)].Multiplier > 1.4012984643248171E-45)
        damageResult |= this.AddStatusEffect(new TentacleStatusEffect(StatusEffects.Steamed, damage.Amount, damage.Magnitude, 1f, 1f));
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
      if ((damage.Element & Elements.PhysicalElements) != Elements.None && !this.HasStatus(StatusEffects.Frozen) && GlobalSettings.Instance.BloodAndGore == SettingOptions.On)
      {
        Vector3 iPosition = iAttackPosition;
        Vector3 right = Vector3.Right;
        EffectManager.Instance.StartEffect(Cthulhu.BLOOD_BLACK_EFFECT, ref iPosition, ref right, out VisualEffectReference _);
      }
      damage.Amount *= damage.Magnitude;
      this.mHitPoints -= damage.Amount;
      if ((double) damage.Amount > 0.0)
        this.mHitFlashTimer = 0.5f;
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
    if ((double) this.mHitPoints > (double) this.mMaxHitPoints)
      this.mHitPoints = this.mMaxHitPoints;
    if ((double) damage.Amount == 0.0)
      damageResult |= DamageResult.Deflected;
    if ((double) this.mHitPoints <= 0.0)
      damageResult |= DamageResult.Killed;
    return damageResult;
  }

  public void Damage(float iDamage, Elements iElement)
  {
    if ((iElement & Elements.Fire) == Elements.Fire && this.HasStatus(StatusEffects.Greased))
      iDamage *= 2f;
    this.mHitPoints -= iDamage;
    if ((double) this.mHitPoints <= (double) this.mMaxHitPoints)
      return;
    this.mHitPoints = this.mMaxHitPoints;
  }

  public void OverKill() => this.Kill();

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
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

  internal void ClearAllStatusEffects()
  {
    for (int index = 0; index < this.mStatusEffects.Length; ++index)
    {
      this.mStatusEffects[index].Stop();
      this.mStatusEffects[index] = new TentacleStatusEffect();
    }
  }

  protected void UpdateDamage(float iDeltaTime)
  {
    Vector3 notifierTextPostion = this.NotifierTextPostion;
    this.mTimeSinceLastDamage += iDeltaTime;
    this.mTimeSinceLastStatusDamage += iDeltaTime;
    if (this.mLastDamageIndex < 0)
      return;
    if ((double) this.mTimeSinceLastDamage > 0.33300000429153442 || this.Dead)
    {
      DamageNotifyer.Instance.ReleasNumber(this.mLastDamageIndex);
      this.mLastDamageIndex = -1;
    }
    else
      DamageNotifyer.Instance.UpdateNumberPosition(this.mLastDamageIndex, ref notifierTextPostion);
  }

  protected void UpdateStatusEffects(float iDeltaTime)
  {
    this.mDryTimer -= iDeltaTime;
    StatusEffects statusEffects = StatusEffects.None;
    if (this.Dead)
    {
      for (int index = 0; index < this.mStatusEffects.Length; ++index)
      {
        this.mStatusEffects[index].Stop();
        this.mStatusEffects[index] = new TentacleStatusEffect();
      }
    }
    else
    {
      for (int index = 0; index < this.mStatusEffects.Length; ++index)
      {
        this.mStatusEffects[index].Update(iDeltaTime, (IStatusEffected) this, this.Body.CollisionSkin);
        if (this.mStatusEffects[index].Dead)
        {
          this.mStatusEffects[index].Stop();
          this.mStatusEffects[index] = new TentacleStatusEffect();
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

  public DamageResult AddStatusEffect(TentacleStatusEffect iStatusEffect)
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
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)] = new TentacleStatusEffect();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = new TentacleStatusEffect();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = new TentacleStatusEffect();
            flag = true;
            break;
          }
          break;
        case StatusEffects.Wet:
          if (this.HasStatus(StatusEffects.Burning) || (double) this.mDryTimer > 0.0)
          {
            int index = StatusEffect.StatusIndex(StatusEffects.Burning);
            this.mStatusEffects[index].Stop();
            this.mStatusEffects[index] = new TentacleStatusEffect();
            flag = true;
          }
          if (this.HasStatus(StatusEffects.Greased))
          {
            int index = StatusEffect.StatusIndex(StatusEffects.Greased);
            this.mStatusEffects[index].Stop();
            this.mStatusEffects[index] = new TentacleStatusEffect();
            break;
          }
          break;
        case StatusEffects.Cold:
          float magnitude = iStatusEffect.Magnitude;
          if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead || (double) this.mDryTimer > 0.0)
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)] = new TentacleStatusEffect();
            this.mDryTimer = 0.9f;
            flag = true;
          }
          float iMagnitude = magnitude * this.mResistances[Defines.ElementIndex(Elements.Cold)].Multiplier;
          if (this.HasStatus(StatusEffects.Wet))
          {
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = new TentacleStatusEffect(StatusEffects.Frozen, 0.0f, iMagnitude, 1f, 1f);
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = new TentacleStatusEffect();
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
            this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)] = new TentacleStatusEffect();
            break;
          }
          break;
        case StatusEffects.Greased:
          if (this.HasStatus(StatusEffects.Wet))
          {
            int index = StatusEffect.StatusIndex(StatusEffects.Wet);
            this.mStatusEffects[index].Stop();
            this.mStatusEffects[index] = new TentacleStatusEffect();
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

  public StatusEffect[] GetStatusEffects() => (StatusEffect[]) null;

  public float StatusMagnitude(StatusEffects iStatus)
  {
    return this.mStatusEffects[StatusEffect.StatusIndex(iStatus)].Magnitude;
  }

  public float Volume => 1f;

  private class RenderData : IRenderableObject
  {
    public Matrix[] Skeleton;
    public SkinnedModelDeferredNormalMappedMaterial Material;
    public BoundingSphere BoundingSphere;
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

    public bool Cull(BoundingFrustum iViewFrustum) => false;

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

  private enum Animations
  {
    Idle,
    Squeeze,
    Crush,
    Emerge,
    Submerge,
    Die,
    Grip,
    Grip_Success,
    Let_Go,
    NR_OF_ANIMATIONS,
  }

  internal enum States
  {
    None,
    Idle,
    Emerge,
    Submerge,
    SwipeGrab,
    GrabSuccess,
    Squeeze,
    Release,
    Crush,
    Aim,
    Die,
    NR_OF_STATES,
  }

  private interface TentacleState
  {
    void OnEnter(Tentacle iOwner);

    void OnUpdate(Tentacle iOwner, float iDeltaTime);

    void OnExit(Tentacle iOwner);

    bool Active(Tentacle iOwner);
  }

  private class IdleState : Tentacle.TentacleState
  {
    private float mTimer;

    public void OnEnter(Tentacle iOwner)
    {
      float time = 0.6f;
      switch (iOwner.mPreviousState)
      {
        case Tentacle.States.Submerge:
          time = 1f;
          break;
        case Tentacle.States.GrabSuccess:
        case Tentacle.States.Squeeze:
          time = 2f;
          break;
        case Tentacle.States.Aim:
          time = -1f;
          break;
      }
      if ((double) time != -1.0 && !iOwner.IsPlaying(Tentacle.Animations.Idle))
        iOwner.CrossFade(Tentacle.Animations.Idle, time, true);
      this.mTimer = iOwner.mOwner.TimeBetweenAttacks;
    }

    public void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      if ((double) this.mTimer <= 0.0 && iOwner.mOwner.OkToFight)
      {
        if (iOwner.mStates[3].Active(iOwner))
          iOwner.ChangeState(Tentacle.States.Submerge);
        else if (iOwner.mStates[4].Active(iOwner))
          iOwner.ChangeState(Tentacle.States.SwipeGrab);
        else if (iOwner.mStates[9].Active(iOwner))
          iOwner.ChangeState(Tentacle.States.Aim);
      }
      this.mTimer -= iDeltaTime;
    }

    public void OnExit(Tentacle iOwner)
    {
    }

    public bool Active(Tentacle iOwner) => false;
  }

  private class EmergeState : Tentacle.TentacleState
  {
    private const float LOGIC_EXECUTE_TIME = 0.218181819f;
    private const float PFX_EXECUTE_TIME = 0.0f;
    private const float UNFREEZE_TIME = 0.09090909f;
    private bool mEffectDone;
    private bool mUnfreezeDone;
    private bool mBubblesDone;
    private VisualEffectReference mEffectRef;
    private int WaterSplashEffect = "cthulhu_tentacle_emerge_water_splash".GetHashCodeCustom();
    private VisualEffectReference mBubbleEffectRef;
    private int BubbleEffect = "cthulhu_tentacle_emerge_bubbles".GetHashCodeCustom();
    private float mBubbleTimer;

    public void OnEnter(Tentacle iOwner)
    {
      iOwner.mDead = false;
      TentacleStatusEffect iStatusEffect = new TentacleStatusEffect(StatusEffects.Wet, 0.0f, 1f, 1f, 1f, 2f);
      int num = (int) iOwner.AddStatusEffect(iStatusEffect);
      iOwner.mHPOnLastEmerge = iOwner.HitPoints;
      iOwner.mTimeSinceLastEmerge = 0.0f;
      iOwner.mTimeUntilSubmerge = (float) (30.0 + Cthulhu.RANDOM.NextDouble() * 5.0);
      Vector3 translation = iOwner.mTransform.Translation;
      Vector3 result = Vector3.Right;
      Vector3.Multiply(ref result, 2.5f, out result);
      Magicka.GameLogic.Damage iDamage = new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Fire, 100f, 4f);
      Liquid.Freeze(iOwner.mPlayState.Level.CurrentScene, ref translation, ref result, 6.28318548f, 2f, ref iDamage);
      this.mUnfreezeDone = false;
      this.mEffectDone = false;
      if (iOwner.mOwner.InitialEmerge)
      {
        this.mBubblesDone = true;
        this.mBubbleTimer = 0.0f;
        iOwner.StartClip(Tentacle.Animations.Emerge, false);
      }
      else
      {
        this.mBubblesDone = false;
        this.mBubbleTimer = 1.5f;
        iOwner.StartClip(Tentacle.Animations.Emerge, false);
        iOwner.StopClip();
      }
      AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_EMERGE, iOwner.AudioEmitter);
    }

    public void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      if ((double) this.mBubbleTimer >= 0.0)
      {
        this.mBubbleTimer -= iDeltaTime;
        if ((double) this.mBubbleTimer <= 0.0)
        {
          iOwner.StartClip(Tentacle.Animations.Emerge, false);
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
            Y = iOwner.WaterYPos
          });
          EffectManager.Instance.StartEffect(this.BubbleEffect, ref translation, out this.mBubbleEffectRef);
          this.mBubblesDone = true;
        }
      }
      else
      {
        if (iOwner.mAnimationController.CrossFadeEnabled)
          return;
        if (iOwner.AnimationHasFinished)
          iOwner.ChangeState(Tentacle.States.Idle);
        float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if (!this.mUnfreezeDone && (double) num >= 0.090909093618392944)
        {
          Vector3 translation = iOwner.mTransform.Translation;
          Vector3 result = Vector3.Right;
          Vector3.Multiply(ref result, 4f, out result);
          Magicka.GameLogic.Damage iDamage = new Magicka.GameLogic.Damage(AttackProperties.Status, Elements.Fire, 100f, 4f);
          Liquid.Freeze(iOwner.mPlayState.Level.CurrentScene, ref translation, ref result, 6.28318548f, 2f, ref iDamage);
          this.mUnfreezeDone = true;
        }
        if (this.mEffectDone || (double) num < 0.0)
          return;
        if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
        {
          Matrix translation = Matrix.CreateTranslation(iOwner.mTransform.Translation with
          {
            Y = iOwner.WaterYPos
          });
          EffectManager.Instance.StartEffect(this.WaterSplashEffect, ref translation, out this.mEffectRef);
        }
        this.mEffectDone = true;
        iOwner.TurnOnIdleEffect();
      }
    }

    public void OnExit(Tentacle iOwner)
    {
    }

    public bool Active(Tentacle iOwner) => false;
  }

  private class SubmergeState : Tentacle.TentacleState
  {
    private const float PFX_EXECUTE_TIME = 0.0f;
    private float mRegenerationTimer;
    private VisualEffectReference mEffectRef;
    private int WaterSplashEffect = "cthulhu_tentacle_emerge_water_splash".GetHashCodeCustom();
    private bool mEffectDone;

    public void OnEnter(Tentacle iOwner)
    {
      iOwner.CrossFade(Tentacle.Animations.Submerge, 0.5f, false);
      this.mRegenerationTimer = (double) iOwner.HitPoints <= 0.0 ? (float) (10 + MagickaMath.Random.Next(6)) : 0.0f;
      this.mEffectDone = false;
      AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_SUBMERGE, iOwner.AudioEmitter);
    }

    public void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (NetworkManager.Instance.State != NetworkState.Client && iOwner.AnimationHasFinished)
      {
        this.mRegenerationTimer -= iDeltaTime;
        if ((double) this.mRegenerationTimer <= 0.0)
        {
          iOwner.Revive();
          iOwner.mOwner.SpawnTentacleAtGoodPoint(iOwner);
        }
      }
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (this.mEffectDone || (double) num < 0.46511629223823547)
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

    public void OnExit(Tentacle iOwner)
    {
      iOwner.mDead = true;
      iOwner.KillIdleEffect();
    }

    public bool Active(Tentacle iOwner)
    {
      return (double) iOwner.mHPOnLastEmerge - (double) iOwner.HitPoints > 0.5 * (double) iOwner.MaxHitPoints || (double) iOwner.mTimeUntilSubmerge < 0.0 || (double) iOwner.mTimeSinceLastEmerge > 5.0 && iOwner.mSecondsWhileOutOfRange > 0;
    }
  }

  private class DieState : Tentacle.TentacleState
  {
    private const float PFX_EXECUTE_TIME = 0.0f;
    private VisualEffectReference mEffectRef;
    private int WaterSplashEffect = "cthulhu_tentacle_emerge_water_splash".GetHashCodeCustom();
    private bool mEffectDone;

    public void OnEnter(Tentacle iOwner)
    {
      iOwner.CrossFade(Tentacle.Animations.Die, 0.5f, false);
      iOwner.mGrabbed = (IDamageable) null;
      this.mEffectDone = false;
      AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_SUBMERGE, iOwner.AudioEmitter);
      iOwner.mOwner.LeaveSpawnTransform(iOwner);
    }

    public void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      if (this.mEffectDone || (double) num < 0.46511629223823547)
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
      iOwner.KillIdleEffect();
    }

    public void OnExit(Tentacle iOwner) => iOwner.mDead = true;

    public bool Active(Tentacle iOwner) => false;
  }

  private class SwipeGrabState : Tentacle.TentacleState
  {
    private const float TOP_JOINT_TO_TOPMOST_POINT = 1.5f;
    private bool mDone;
    private bool mDeflected;

    public void OnEnter(Tentacle iOwner)
    {
      iOwner.CrossFade(Tentacle.Animations.Grip, 0.2f, false);
      iOwner.mGrabbed = (IDamageable) null;
      this.mDone = false;
      this.mDeflected = false;
      AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_GRAB_SUCCESS, iOwner.AudioEmitter);
    }

    public unsafe void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      if (iOwner.mGrabbed != null)
      {
        iOwner.ChangeState(Tentacle.States.GrabSuccess);
      }
      else
      {
        if (iOwner.mAnimationController.CrossFadeEnabled)
          return;
        if (iOwner.mAnimationController.HasFinished || this.mDeflected)
          iOwner.ChangeState(Tentacle.States.Idle);
        if (this.mDone)
          return;
        float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        Segment iSeg = new Segment();
        iSeg.Origin = iOwner.mTransform.Translation;
        iSeg.Delta = iOwner.mTransformedTopJoint.Translation;
        Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
        Vector3 result;
        Vector3.Multiply(ref iSeg.Delta, 0.5f, out result);
        Vector3.Add(ref iSeg.Origin, ref result, out result);
        float num2 = iSeg.Delta.Length() + 1.5f;
        List<Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(result, num2 / 2f, false);
        BoundingSphere boundingSphere = new BoundingSphere(iOwner.mTransformedTopJoint.Translation, 1f);
        entities.Remove((Entity) iOwner);
        for (int index = 0; index < entities.Count && !this.mDone; ++index)
        {
          if (entities[index] is IDamageable damageable)
          {
            switch (damageable)
            {
              case Shield _:
                int num3 = (int) damageable.Damage(iOwner.mSwipeDamage, (Entity) null, iOwner.mPlayState.PlayTime, result);
                this.mDone = true;
                this.mDeflected = true;
                iOwner.CrossFade(Tentacle.Animations.Idle, 0.15f, true);
                continue;
              case SpellMine _:
                continue;
              case Barrier _:
                int num4 = (int) damageable.Damage(iOwner.mSwipeDamage, (Entity) null, iOwner.mPlayState.PlayTime, result);
                this.mDone = true;
                this.mDeflected = true;
                iOwner.CrossFade(Tentacle.Animations.Idle, 0.15f, true);
                continue;
              default:
                if ((double) num1 >= (double) Tentacle.GRAB_TIME[0] && (double) num1 <= (double) Tentacle.GRAB_TIME[1])
                {
                  BoundingSphere sphere = new BoundingSphere(damageable.Position, damageable.Radius);
                  if (!this.mDone && boundingSphere.Intersects(sphere) && damageable is Avatar)
                  {
                    this.mDone = true;
                    if (NetworkManager.Instance.State != NetworkState.Client)
                    {
                      iOwner.mGrabbed = entities[index] as IDamageable;
                      if (NetworkManager.Instance.State == NetworkState.Server)
                      {
                        BossFight.Instance.SendMessage<Cthulhu.TentacleGrabMessage>((IBoss) iOwner.mOwner, (ushort) 14, (void*) &new Cthulhu.TentacleGrabMessage()
                        {
                          Handle = iOwner.mGrabbed.Handle,
                          TentacleIndex = iOwner.mID
                        }, true);
                        goto label_21;
                      }
                      goto label_21;
                    }
                    goto label_21;
                  }
                  Vector3 oPosition;
                  if (!this.mDone && damageable.SegmentIntersect(out oPosition, iSeg, 0.6f) && !iOwner.mHitList.Contains(damageable))
                  {
                    Vector3 iAttackPosition = oPosition + iOwner.mTransform.Right * 1f;
                    int num5 = (int) damageable.Damage(iOwner.mSwipeDamage, (Entity) null, iOwner.mPlayState.PlayTime, iAttackPosition);
                    Magicka.GameLogic.Damage iDamage = new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, damageable.Body.Mass, 2f);
                    int num6 = (int) damageable.Damage(iDamage, (Entity) iOwner, iOwner.mPlayState.PlayTime, iAttackPosition);
                    iOwner.mHitList.Add(damageable);
                    goto label_21;
                  }
                  goto label_21;
                }
                continue;
            }
          }
        }
label_21:
        iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
      }
    }

    public void OnExit(Tentacle iOwner)
    {
    }

    public bool Active(Tentacle iOwner)
    {
      if (Cthulhu.RANDOM.NextDouble() >= 0.5)
        return false;
      EntityManager entityManager = iOwner.mPlayState.EntityManager;
      Vector3 translation = iOwner.mTransform.Translation;
      List<Entity> entities = entityManager.GetEntities(translation, iOwner.mTentacleLength - 1.2f, true);
      bool flag = false;
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is IDamageable damageable)
        {
          if (damageable is Shield || damageable is Barrier)
          {
            flag = false;
            break;
          }
          if (!flag)
            flag = entities[index] is Avatar avatar && !avatar.IsEthereal && !avatar.IsInvisibile;
        }
      }
      entityManager.ReturnEntityList(entities);
      return flag;
    }
  }

  private class GrabSuccessState : Tentacle.TentacleState
  {
    public void OnEnter(Tentacle iOwner)
    {
      iOwner.CrossFade(Tentacle.Animations.Grip_Success, 0.1f, false);
      AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_GRAB_SUCCESS, iOwner.AudioEmitter);
    }

    public void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      if (!iOwner.AnimationHasFinished)
        return;
      iOwner.ChangeState(Tentacle.States.Squeeze);
    }

    public void OnExit(Tentacle iOwner)
    {
    }

    public bool Active(Tentacle iOwner) => false;
  }

  private class SqueezeState : Tentacle.TentacleState
  {
    private float mTimer;

    public void OnEnter(Tentacle iOwner)
    {
      iOwner.CrossFade(Tentacle.Animations.Squeeze, 0.15f, true);
      this.mTimer = 5f - iOwner.mOwner.TimeBetweenAttacks;
      AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_SQUEEZE, iOwner.AudioEmitter);
    }

    public void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      IDamageable mGrabbed = iOwner.mGrabbed;
      if (mGrabbed != null && mGrabbed is IStatusEffected && (mGrabbed as IStatusEffected).HasStatus(StatusEffects.Burning))
        this.mTimer = 0.0f;
      if ((double) this.mTimer <= 0.0)
      {
        if (mGrabbed != null)
        {
          iOwner.ChangeState(Tentacle.States.Release);
          return;
        }
        iOwner.mGrabbed = (IDamageable) null;
      }
      this.mTimer -= iDeltaTime;
      if (iOwner.mGrabbed == null || iOwner.mGrabbed.Dead)
      {
        iOwner.ChangeState(Tentacle.States.Idle);
      }
      else
      {
        if (iOwner.mAnimationController.CrossFadeEnabled || iOwner.mHitList.Contains(iOwner.mGrabbed))
          return;
        float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if ((double) num1 < (double) Tentacle.SQUEEZE_DAMAGE_TIME[0] || (double) num1 > (double) Tentacle.SQUEEZE_DAMAGE_TIME[1])
          return;
        int num2 = (int) iOwner.mGrabbed.Damage(iOwner.mSqueezeDamage, (Entity) null, iOwner.mPlayState.PlayTime, iOwner.mGrabTransform.Translation);
        iOwner.mHitList.Add(iOwner.mGrabbed);
      }
    }

    public void OnExit(Tentacle iOwner)
    {
    }

    public bool Active(Tentacle iOwner) => false;
  }

  private class ReleaseState : Tentacle.TentacleState
  {
    private const float THROW_TIME = 0.4054054f;

    public void OnEnter(Tentacle iOwner)
    {
      iOwner.CrossFade(Tentacle.Animations.Let_Go, 0.2f, false);
      AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_RELEASE, iOwner.AudioEmitter);
    }

    public void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.HasFinished)
      {
        iOwner.ChangeState(Tentacle.States.Idle);
      }
      else
      {
        float num = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
        if (iOwner.mGrabbed == null || (double) num < 0.40540540218353271)
          return;
        Vector3 result = iOwner.mTransform.Forward with
        {
          Y = 0.0f
        };
        Vector3.Multiply(ref result, 12f, out result);
        iOwner.mGrabbed.Body.Velocity = result;
        iOwner.mGrabbed = (IDamageable) null;
      }
    }

    public void OnExit(Tentacle iOwner)
    {
    }

    public bool Active(Tentacle iOwner) => false;
  }

  private class AimState : Tentacle.TentacleState
  {
    private float mRadius = 8f;

    public void OnEnter(Tentacle iOwner)
    {
      this.mRadius = iOwner.mTentacleLength;
      iOwner.mSearchForTarget = true;
      iOwner.CrossFade(Tentacle.Animations.Idle, 0.6f, true);
    }

    public unsafe void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      Vector3 translation1 = iOwner.mTransform.Translation;
      Vector3 forward1 = iOwner.mTransform.Forward;
      Vector3 zero = Vector3.Zero;
      Vector3 result1 = Vector3.Zero;
      bool flag = false;
      if (iOwner.mSearchForTarget)
      {
        Avatar avatar1 = iOwner.mCrushTarget;
        if (NetworkManager.Instance.State != NetworkState.Client)
        {
          if (avatar1 != null && (avatar1.Dead || avatar1.Drowning))
            iOwner.mCrushTarget = avatar1 = (Avatar) null;
          if (avatar1 != null && !avatar1.Dead && !avatar1.IsEthereal)
          {
            float num1 = translation1.X - avatar1.Position.X;
            float num2 = translation1.Z - avatar1.Position.Z;
            if ((double) num1 * (double) num1 + (double) num2 * (double) num2 > (double) this.mRadius * (double) this.mRadius)
              avatar1 = (Avatar) null;
          }
        }
        if (avatar1 == null)
        {
          if (NetworkManager.Instance.State != NetworkState.Client)
          {
            List<Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(translation1, this.mRadius, false, true);
            entities.Remove((Entity) iOwner);
            for (int index = 0; index < entities.Count && !flag; ++index)
            {
              flag = entities[index] is Avatar avatar2 && !avatar2.IsEthereal;
              if (flag)
              {
                iOwner.mCrushTarget = avatar2;
                Vector3 position = entities[index].Position;
                Vector3.Subtract(ref position, ref translation1, out result1);
                result1.Y = 0.0f;
                if ((double) result1.LengthSquared() <= 9.9999999747524271E-07)
                  flag = false;
                else
                  result1.Normalize();
              }
            }
            iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
            if (flag && NetworkManager.Instance.State == NetworkState.Server)
              BossFight.Instance.SendMessage<Cthulhu.TentacleAimTargetMessage>((IBoss) iOwner.mOwner, (ushort) 15, (void*) &new Cthulhu.TentacleAimTargetMessage()
              {
                Handle = iOwner.mCrushTarget.Handle,
                TentacleIndex = iOwner.mID
              }, true);
          }
        }
        else
        {
          flag = true;
          Vector3 position = avatar1.Position;
          Vector3.Subtract(ref position, ref translation1, out result1);
          result1.Y = 0.0f;
          if ((double) result1.LengthSquared() <= 9.9999999747524271E-07)
            flag = false;
          else
            result1.Normalize();
        }
      }
      if (!flag)
        result1 = iOwner.mOriginalForwardDirection;
      float oAngle;
      Tentacle.AimState.GetAngle(ref forward1, ref result1, out oAngle);
      if ((double) System.Math.Abs(oAngle) < 0.01 && flag)
        iOwner.ChangeState(Tentacle.States.Crush);
      float radians = MathHelper.ToRadians(30f);
      Quaternion result2;
      Quaternion.CreateFromYawPitchRoll(2f * ((double) oAngle >= 0.0 ? System.Math.Min(oAngle, radians) : System.Math.Max(oAngle, -radians)) * iOwner.mOwner.StageSpeedModifier * iOwner.GetColdSlowdown() * iDeltaTime, 0.0f, 0.0f, out result2);
      Vector3 forward2 = iOwner.mTransform.Forward;
      Vector3.Transform(ref forward2, ref result2, out Vector3 _);
      Vector3 translation2 = iOwner.mTransform.Translation;
      Matrix.Transform(ref iOwner.mTransform, ref result2, out iOwner.mTransform);
      iOwner.mTransform.Translation = translation2;
      if ((double) System.Math.Abs(oAngle) >= 0.01 || flag)
        return;
      iOwner.ChangeState(Tentacle.States.Idle);
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
      Tentacle.AimState.GetAngle(ref iForward, ref iTargetDirection, out oAngle);
    }

    private static void GetAngle(
      ref Vector3 iForward,
      ref Vector3 iTargetDirection,
      out float oAngle)
    {
      Vector3 result;
      Vector3.Cross(ref iTargetDirection, ref iForward, out result);
      float num = (float) System.Math.Acos((double) MathHelper.Clamp(result.Y, -1f, 1f)) - 1.57079637f;
      oAngle = num;
    }

    public void OnExit(Tentacle iOwner)
    {
    }

    public bool Active(Tentacle iOwner)
    {
      return iOwner.CheckIfCharactersAreWithinRadius(iOwner.mTentacleLength);
    }
  }

  private class CrushState : Tentacle.TentacleState
  {
    private VisualEffectReference mEffectRef;
    private int CrushDustEffect = "cthulhu_tentacle_crush_dust".GetHashCodeCustom();
    private bool mDone;
    private bool mDeflected;

    public void OnEnter(Tentacle iOwner)
    {
      iOwner.CrossFade(Tentacle.Animations.Crush, 0.15f, false);
      this.mDone = false;
      this.mDeflected = false;
    }

    public unsafe void OnUpdate(Tentacle iOwner, float iDeltaTime)
    {
      if (iOwner.mAnimationController.CrossFadeEnabled)
        return;
      if (iOwner.mAnimationController.HasFinished || this.mDeflected)
      {
        iOwner.ChangeState(Tentacle.States.Aim);
        iOwner.mCrushTarget = (Avatar) null;
        iOwner.mSearchForTarget = false;
        if (NetworkManager.Instance.State == NetworkState.Server)
          BossFight.Instance.SendMessage<Cthulhu.TentacleReleaseAimTargetMessage>((IBoss) iOwner.mOwner, (ushort) 16 /*0x10*/, (void*) &new Cthulhu.TentacleReleaseAimTargetMessage()
          {
            TentacleIndex = iOwner.mID
          }, true);
      }
      if (this.mDone)
        return;
      float num1 = iOwner.mAnimationController.Time / iOwner.mAnimationController.AnimationClip.Duration;
      Segment iSeg = new Segment();
      iSeg.Origin = iOwner.mTransform.Translation;
      iSeg.Delta = iOwner.mTransformedTopJoint.Translation;
      Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
      Vector3 result;
      Vector3.Multiply(ref iSeg.Delta, 0.5f, out result);
      Vector3.Add(ref iSeg.Origin, ref result, out result);
      float num2 = iSeg.Delta.Length();
      List<Entity> entities = iOwner.mPlayState.EntityManager.GetEntities(result, num2 / 2f, false);
      entities.Remove((Entity) iOwner);
      for (int index = 0; index < entities.Count; ++index)
      {
        Vector3 oPosition;
        if (entities[index] is IDamageable damageable && damageable.SegmentIntersect(out oPosition, iSeg, 0.6f))
        {
          switch (damageable)
          {
            case Shield _:
              this.mDone = true;
              damageable.Kill();
              this.mDeflected = true;
              iOwner.CrossFade(Tentacle.Animations.Idle, 0.15f, true);
              continue;
            case SpellMine _:
            case Barrier _:
              damageable.Kill();
              continue;
            default:
              if ((double) num1 >= 0.3541666567325592 && !iOwner.mHitList.Contains(damageable))
              {
                int num3 = (int) damageable.Damage(iOwner.mCrushDamage, (Entity) null, iOwner.mPlayState.PlayTime, oPosition);
                Magicka.GameLogic.Damage iDamage = new Magicka.GameLogic.Damage(AttackProperties.Knockback, Elements.Earth, damageable.Body.Mass, 2f);
                int num4 = (int) damageable.Damage(iDamage, (Entity) null, iOwner.mPlayState.PlayTime, oPosition);
                iOwner.mHitList.Add(damageable);
                continue;
              }
              continue;
          }
        }
      }
      if ((double) num1 >= 0.3541666567325592 && !this.mDone)
      {
        this.mDone = true;
        if (!EffectManager.Instance.IsActive(ref this.mEffectRef))
        {
          Matrix translation = Matrix.CreateTranslation(iOwner.mTransformedTopJoint.Translation);
          EffectManager.Instance.StartEffect(this.CrushDustEffect, ref translation, out this.mEffectRef);
        }
        AudioManager.Instance.PlayCue(Banks.Additional, Tentacle.SOUND_CRUSH, iOwner.AudioEmitter);
      }
      iOwner.mPlayState.EntityManager.ReturnEntityList(entities);
    }

    public void OnExit(Tentacle iOwner)
    {
    }

    public bool Active(Tentacle iOwner) => false;
  }
}
