// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SummonDeath
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
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
using System.Collections.ObjectModel;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class SummonDeath : SpecialAbility, IAbilityEffect
{
  private static SummonDeath mSingelton;
  private static volatile object mSingeltonLock = new object();
  private SummonDeath.MagickDeath mDeath;
  private PlayState mPlayState;
  private ISpellCaster mOwner;
  private static readonly int SOUND_HASH = "magick_summon_death".GetHashCodeCustom();
  private static readonly int SOUND_SLOWDOWN_HASH = "magick_timewarp".GetHashCodeCustom();
  private static Cue sCue;

  public static SummonDeath Instance
  {
    get
    {
      if (SummonDeath.mSingelton == null)
      {
        lock (SummonDeath.mSingeltonLock)
        {
          if (SummonDeath.mSingelton == null)
            SummonDeath.mSingelton = new SummonDeath();
        }
      }
      return SummonDeath.mSingelton;
    }
  }

  private SummonDeath()
    : base(Magicka.Animations.cast_magick_direct, "#magick_sdeath".GetHashCodeCustom())
  {
  }

  public void Initialize(PlayState iPlayState)
  {
    this.mDeath = new SummonDeath.MagickDeath(iPlayState);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    if (this.IsDead)
    {
      this.mOwner = iOwner;
      this.mPlayState = iPlayState;
      return this.Execute(this.mOwner.Position);
    }
    AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
    return false;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    if (this.IsDead)
    {
      this.mPlayState = iPlayState;
      this.mOwner = (ISpellCaster) null;
      return this.Execute(iPosition);
    }
    AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
    return false;
  }

  private bool Execute(Vector3 iPosition)
  {
    AudioManager.Instance.PlayCue(Banks.Spells, SummonDeath.SOUND_HASH);
    SummonDeath.sCue = AudioManager.Instance.GetCue(Banks.Spells, SummonDeath.SOUND_SLOWDOWN_HASH);
    SummonDeath.sCue.Play();
    Vector3 result1 = this.mPlayState.Camera.Position;
    Vector3 result2 = MagickCamera.CAMERAOFFSET;
    Vector3.Negate(ref result2, out result2);
    Vector3.Add(ref result1, ref result2, out result1);
    Magicka.GameLogic.Entities.Character iTarget = (Magicka.GameLogic.Entities.Character) null;
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(result1, 40f, false);
    float num1 = 1f;
    int num2 = SpecialAbility.RANDOM.Next(entities.Count);
    for (int index1 = 0; index1 < entities.Count; ++index1)
    {
      int index2 = (index1 + num2) % entities.Count;
      if (entities[index2] is Magicka.GameLogic.Entities.Character character && !character.Dead && (character.Faction & Factions.UNDEAD) == Factions.NONE)
      {
        float num3 = character.HitPoints / character.MaxHitPoints;
        if ((double) num3 > 0.0 && (double) num3 <= (double) num1)
        {
          num1 = num3;
          iTarget = character;
        }
      }
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    if (iTarget == null)
    {
      if (this.mOwner is Magicka.GameLogic.Entities.Character)
      {
        iTarget = this.mOwner as Magicka.GameLogic.Entities.Character;
      }
      else
      {
        AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
        return false;
      }
    }
    Vector3 result3 = iTarget.Position;
    Vector3 vector3 = result3;
    int num4 = 0;
    float result4;
    do
    {
      float oSin;
      float oCos;
      MathApproximation.FastSinCos((float) (SpecialAbility.RANDOM.NextDouble() - 0.5) * 2f * 3.14159274f, out oSin, out oCos);
      Vector3 result5 = new Vector3(oCos, 0.0f, oSin);
      Vector3.Multiply(ref result5, 7f, out result5);
      Vector3.Add(ref result3, ref result5, out result3);
      Vector3 oPoint;
      double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result3, out oPoint, MovementProperties.Default);
      result3 = oPoint;
      Vector3.DistanceSquared(ref result3, ref vector3, out result4);
    }
    while ((double) result4 < 4.0 && num4 < 10);
    if (num4 > 9)
      return false;
    Vector3 up = Vector3.Up;
    Vector3 cameraPosition = result3;
    Vector3 position = iTarget.Position;
    cameraPosition.Y = position.Y = 0.0f;
    Matrix result6;
    Matrix.CreateLookAt(ref cameraPosition, ref position, ref up, out result6);
    result6.Translation = result3;
    this.mDeath.Initialize(ref result6, this.mPlayState, iTarget);
    this.mPlayState.EntityManager.AddEntity((Entity) this.mDeath);
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => this.mDeath.Dead;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    RenderManager.Instance.Saturation = 0.5f;
  }

  public void OnRemove()
  {
    RenderManager.Instance.Saturation = 1f;
    if (SummonDeath.sCue != null)
      SummonDeath.sCue.Stop(AudioStopOptions.AsAuthored);
    this.mDeath.Kill();
  }

  public class MagickDeath : Entity, IDamageable
  {
    private const float CAPSULE_RADIUS = 0.6f;
    private const float CAPSULE_LENGTH = 1.2f;
    private const float SCYTHE_ATTACK_START = 0.40625f;
    private const float SCYTHE_ATTACK_END = 0.46875f;
    private const float SCALE = 1.2f;
    private const float HEIGHT_OFFSET = 1.2f;
    private static readonly int SPAWN_EFFECT = "death_ethereal_spawn".GetHashCodeCustom();
    private static readonly int DESPAWN_EFFECT = "death_ethereal_despawn".GetHashCodeCustom();
    private static readonly int SCYTHE_HIT_SOUND = "wep_death_scythe".GetHashCodeCustom();
    private static readonly int SCYTHE_SWING_SOUND = "wep_death_scythe_swing".GetHashCodeCustom();
    private static readonly int DEATH_PURSUIT_SOUND = "boss_death_proximity".GetHashCodeCustom();
    private static readonly int DEATH_SOUND = "boss_death_death".GetHashCodeCustom();
    private Cue mPursuitCue;
    private Cue mScytheCue;
    private Cue mDeathCue;
    private HitList mHitlist = new HitList(32 /*0x20*/);
    private SummonDeath.MagickDeath.DeferredRenderData[] mRenderData;
    private SummonDeath.MagickDeath.ItemRenderData[] mScytheRenderData;
    private SummonDeath.MagickDeath.AfterImageRenderData[] mAfterImageRenderData;
    private float mAfterImageTimer;
    private float mAfterImageIntensity;
    private AnimationController mController;
    private AnimationClip[] mClips;
    private SummonDeath.MagickDeath.Animations mCurrentAnimation;
    private Vector3 mLastScythePosition;
    private SkinnedModel mModel;
    private Random mRandom;
    private Magicka.GameLogic.Entities.Character mTarget;
    private bool mIsHit;
    private float mOscilationTimer;
    private float mAlphaTimer;
    private float mSpawnDelayTimer;
    private float mRangeSqr;
    private float mTimeTargetModifier;
    private float mSpeed;
    private Matrix mOrientation;
    private Vector3 mMovement;
    private BoundingSphere mBoundingSphere;
    private BindJoint mHandJoint;
    private bool mIsEthereal = true;
    private static readonly Matrix sScale = Matrix.CreateScale(1.2f);
    private static readonly Matrix sInvScale = Matrix.Invert(SummonDeath.MagickDeath.sScale);

    public MagickDeath(PlayState iPlayState)
      : base(iPlayState)
    {
      this.mRandom = new Random();
      this.mPlayState = iPlayState;
      this.mDead = true;
      Model model;
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        this.mModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Bosses/Death/Death");
        model = Magicka.Game.Instance.Content.Load<Model>("Models/Bosses/Death/death_Scythe");
      }
      Matrix rotationY = Matrix.CreateRotationY(3.14159274f);
      foreach (SkinnedModelBone skeletonBone in (ReadOnlyCollection<SkinnedModelBone>) this.mModel.SkeletonBones)
      {
        if (skeletonBone.Name.Equals("RightAttach", StringComparison.OrdinalIgnoreCase))
        {
          this.mHandJoint.mIndex = (int) skeletonBone.Index;
          this.mHandJoint.mBindPose = skeletonBone.InverseBindPoseTransform;
          Matrix.Multiply(ref this.mHandJoint.mBindPose, ref rotationY, out this.mHandJoint.mBindPose);
          Matrix.Invert(ref this.mHandJoint.mBindPose, out this.mHandJoint.mBindPose);
        }
      }
      this.mController = new AnimationController();
      this.mController.Skeleton = this.mModel.SkeletonBones;
      this.mClips = new AnimationClip[4];
      this.mClips[0] = this.mModel.AnimationClips["move_glide"];
      this.mClips[1] = this.mModel.AnimationClips["attack_scythe_rise"];
      this.mClips[2] = this.mModel.AnimationClips["attack_scythe_fall"];
      this.mClips[3] = this.mModel.AnimationClips["hit"];
      SkinnedModelDeferredBasicMaterial oMaterial;
      Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out oMaterial);
      this.mBoundingSphere = this.mModel.Model.Meshes[0].BoundingSphere;
      this.mRenderData = new SummonDeath.MagickDeath.DeferredRenderData[3];
      this.mScytheRenderData = new SummonDeath.MagickDeath.ItemRenderData[3];
      this.mAfterImageRenderData = new SummonDeath.MagickDeath.AfterImageRenderData[3];
      Matrix[][] iSkeleton = new Matrix[5][];
      for (int index = 0; index < iSkeleton.Length; ++index)
        iSkeleton[index] = new Matrix[80 /*0x50*/];
      for (int index = 0; index < 3; ++index)
      {
        this.mRenderData[index] = new SummonDeath.MagickDeath.DeferredRenderData();
        this.mRenderData[index].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
        this.mRenderData[index].mMaterial = oMaterial;
        this.mScytheRenderData[index] = new SummonDeath.MagickDeath.ItemRenderData();
        this.mScytheRenderData[index].SetMesh(model.Meshes[0], model.Meshes[0].MeshParts[0], 4, 0, 5);
        this.mAfterImageRenderData[index] = new SummonDeath.MagickDeath.AfterImageRenderData(iSkeleton);
      }
      this.mBody = new Body();
      this.mCollision = new CollisionSkin(this.mBody);
      this.mCollision.AddPrimitive((Primitive) new Capsule(Vector3.Zero, Matrix.CreateRotationX(-1.57079637f), 0.6f, 1.2f), 1, new MaterialProperties(0.0f, 0.0f, 0.0f));
      this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
      this.mBody.CollisionSkin = this.mCollision;
      this.mBody.SetBodyInvInertia(0.0f, 0.0f, 0.0f);
      this.mBody.Mass = 500f;
      this.mBody.Immovable = false;
      this.mBody.Tag = (object) this;
    }

    public bool OnCollision(CollisionSkin skin0, int prim0, CollisionSkin skin1, int prim1)
    {
      return false;
    }

    public void Initialize(ref Matrix iOrientation, PlayState iPlayState, Magicka.GameLogic.Entities.Character iTarget)
    {
      this.mPlayState = iPlayState;
      this.mTarget = iTarget;
      Vector3 position = this.mTarget.Position;
      Vector3 pos = iOrientation.Translation;
      iOrientation.Translation = new Vector3();
      Vector3 result;
      Vector3.Subtract(ref position, ref pos, out result);
      result.Normalize();
      Segment iSeg = new Segment();
      iSeg.Origin = pos + Vector3.Up;
      iSeg.Delta.Y -= 6f;
      Vector3 oPos;
      if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
        pos = oPos;
      this.mOrientation = iOrientation;
      this.mOrientation.Translation = pos;
      this.Turn(ref result, 1f, 1f);
      pos.Y += 1.30000007f;
      this.mOrientation.Translation = pos;
      this.mBody.MoveTo(pos, iOrientation);
      this.mDead = false;
      this.mIsHit = false;
      this.mSpeed = 8f;
      this.Initialize();
      Matrix[] matrixArray = new Matrix[5];
      for (int index1 = 0; index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < this.mAfterImageRenderData[index1].mSkeleton.Length; ++index2)
          matrixArray.CopyTo((Array) this.mAfterImageRenderData[index1].mSkeleton[index2], 0);
      }
      this.mHitlist.Clear();
      this.mRangeSqr = (float) (0.39600002765655518 + (double) this.mTarget.Radius * (double) this.mTarget.Radius);
      this.mRangeSqr *= 1.2f;
      this.mController.StartClip(this.mClips[0], true);
      this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Move;
      this.mTimeTargetModifier = 0.25f;
      this.mPursuitCue = AudioManager.Instance.PlayCue(Banks.Characters, SummonDeath.MagickDeath.DEATH_PURSUIT_SOUND, this.mAudioEmitter);
      this.mScytheCue = (Cue) null;
      this.mDeathCue = (Cue) null;
      EffectManager.Instance.StartEffect(SummonDeath.MagickDeath.SPAWN_EFFECT, ref this.mOrientation, out VisualEffectReference _);
      this.mSpawnDelayTimer = (float) (1.0 / (double) this.mTimeTargetModifier * 0.5);
      this.mMovement = Vector3.Zero;
      this.mAlphaTimer = 0.0f;
      this.mOscilationTimer = 0.0f;
      this.mIsEthereal = true;
    }

    public float ResistanceAgainst(Elements iElement)
    {
      return 1f - MathHelper.Clamp((float) (0.0 / 300.0) + 0.0f, -1f, 1f);
    }

    public override void Update(DataChannel iDataChannel, float iDeltaTime)
    {
      float timeModifier = this.mPlayState.TimeModifier;
      this.mPlayState.TimeModifier = timeModifier + (float) (((double) this.mTimeTargetModifier - (double) timeModifier) * ((double) iDeltaTime / (double) this.mTimeTargetModifier));
      iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
      base.Update(iDataChannel, iDeltaTime);
      this.mHitlist.Update(iDeltaTime);
      Vector3 position1 = this.Position;
      Vector3 translation1 = this.mOrientation.Translation;
      translation1.X += this.mMovement.X * this.mSpeed * iDeltaTime;
      translation1.Z += this.mMovement.Z * this.mSpeed * iDeltaTime;
      Segment iSeg = new Segment();
      iSeg.Origin = translation1;
      ++iSeg.Origin.Y;
      iSeg.Delta.Y -= 3f;
      Vector3 oPos;
      if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
        oPos.Y += 1.2f;
      translation1.Y += (oPos.Y - translation1.Y) * iDeltaTime;
      this.mBody.Velocity = Vector3.Zero;
      this.mBody.AngularVelocity = Vector3.Zero;
      this.mBody.MoveTo(translation1, this.mOrientation);
      this.mOrientation.Translation = translation1;
      Matrix sScale = SummonDeath.MagickDeath.sScale;
      Matrix result1;
      Matrix.Multiply(ref sScale, ref this.mOrientation, out result1);
      translation1.Y -= 1.2f;
      this.mOscilationTimer += iDeltaTime;
      translation1.Y += (float) Math.Cos((double) this.mOscilationTimer) * 0.2f;
      result1.Translation = translation1;
      this.mBoundingSphere.Center = translation1;
      this.mController.Update(iDeltaTime, ref result1, true);
      this.mController.SkinnedBoneTransforms.CopyTo((Array) this.mRenderData[(int) iDataChannel].mBones, 0);
      this.mRenderData[(int) iDataChannel].mBoundingSphere = this.mBoundingSphere;
      this.mRenderData[(int) iDataChannel].RenderAdditive = this.IsEthereal;
      if (!this.IsEthereal)
        this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
      else
        this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) this.mRenderData[(int) iDataChannel]);
      Matrix result2 = this.mHandJoint.mBindPose;
      Matrix.Multiply(ref result2, ref this.mController.SkinnedBoneTransforms[this.mHandJoint.mIndex], out result2);
      Vector3 result3 = result2.Translation;
      Vector3 up = result2.Up;
      Vector3.Add(ref up, ref result3, out result3);
      Segment seg = new Segment();
      seg.Origin = result3;
      Vector3.Subtract(ref seg.Origin, ref this.mLastScythePosition, out seg.Delta);
      this.mLastScythePosition = seg.Origin;
      Vector3 translation2 = result2.Translation;
      Matrix sInvScale = SummonDeath.MagickDeath.sInvScale;
      Matrix.Multiply(ref result2, ref sInvScale, out result2);
      result2.Translation = translation2;
      this.mScytheRenderData[(int) iDataChannel].mBoundingSphere = this.mBoundingSphere;
      this.mScytheRenderData[(int) iDataChannel].WorldOrientation = result2;
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mScytheRenderData[(int) iDataChannel]);
      if (this.mDead || this.mTarget == null || this.mTarget.Dead)
      {
        this.mDead = true;
      }
      else
      {
        if (this.mCurrentAnimation == SummonDeath.MagickDeath.Animations.Move)
        {
          this.mAlphaTimer += iDeltaTime;
          this.mRenderData[(int) iDataChannel].mMaterial.Alpha = Math.Min(this.mAlphaTimer, 1f);
          Vector3 position2 = this.mTarget.Position;
          Vector3 result4;
          Vector3.Subtract(ref position2, ref translation1, out result4);
          result4.Y = 0.0f;
          float num = result4.LengthSquared();
          result4.Normalize();
          this.Turn(ref result4, 8f, iDeltaTime);
          this.mSpawnDelayTimer -= iDeltaTime;
          if ((double) this.mSpawnDelayTimer <= 0.0)
            this.Movement = result4;
          if ((double) num <= (double) this.mRangeSqr)
          {
            this.mSpeed = 0.0f;
            this.mController.CrossFade(this.mClips[1], 0.33f, false);
            this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Scythe_Rise;
            this.IsEthereal = false;
            AudioManager.Instance.PlayCue(Banks.Weapons, SummonDeath.MagickDeath.SCYTHE_SWING_SOUND, this.mAudioEmitter);
          }
        }
        else if (this.mCurrentAnimation == SummonDeath.MagickDeath.Animations.Scythe_Rise)
        {
          this.mAlphaTimer += iDeltaTime;
          this.mRenderData[(int) iDataChannel].mMaterial.Alpha = Math.Min(this.mAlphaTimer, 1f);
          if (this.mController.HasFinished && !this.mController.CrossFadeEnabled)
          {
            this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Scythe_Fall;
            this.mController.CrossFade(this.mClips[2], 0.1f, false);
          }
        }
        else if (this.mCurrentAnimation == SummonDeath.MagickDeath.Animations.Scythe_Fall)
        {
          this.mAlphaTimer += iDeltaTime;
          this.mRenderData[(int) iDataChannel].mMaterial.Alpha = Math.Min(this.mAlphaTimer, 1f);
          Vector3 position3 = this.mTarget.Position;
          Vector3 result5;
          Vector3.Subtract(ref position3, ref translation1, out result5);
          result5.Y = 0.0f;
          float num1 = result5.LengthSquared();
          result5.Normalize();
          float scaleFactor = Math.Max(0.0f, Math.Min(1f, num1 - this.mRangeSqr * 1.1f));
          Vector3.Multiply(ref result5, scaleFactor, out result5);
          this.Movement = result5;
          this.Turn(ref result5, scaleFactor * 4f, iDeltaTime);
          float num2 = this.mController.Time / this.mController.AnimationClip.Duration;
          if (!this.mController.CrossFadeEnabled && (double) num2 >= 0.10000000149011612 && (double) num2 <= 0.800000011920929)
          {
            if (this.mScytheCue == null)
              this.mScytheCue = AudioManager.Instance.PlayCue(Banks.Weapons, SummonDeath.MagickDeath.SCYTHE_SWING_SOUND, this.mAudioEmitter);
            List<Entity> entities = this.mPlayState.EntityManager.GetEntities(seg.Origin, 3f, false, true);
            entities.Remove((Entity) this);
            for (int index = 0; index < entities.Count; ++index)
            {
              if (entities[index] is IStatusEffected statusEffected && !this.mHitlist.ContainsKey(statusEffected.Handle) && entities[index].Body.CollisionSkin.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, seg))
              {
                AudioManager.Instance.PlayCue(Banks.Weapons, SummonDeath.MagickDeath.SCYTHE_HIT_SOUND, this.mAudioEmitter);
                statusEffected.Damage(statusEffected.HitPoints, Elements.Arcane);
                this.mHitlist.Add(statusEffected.Handle, 1.5f);
              }
            }
            this.mPlayState.EntityManager.ReturnEntityList(entities);
          }
          if (this.mIsHit)
          {
            this.mController.CrossFade(this.mClips[3], 0.33f, false);
            this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Disappear;
            this.mAlphaTimer = 1f;
          }
          else if (!this.mController.CrossFadeEnabled && this.mController.HasFinished)
          {
            this.mController.CrossFade(this.mClips[3], 0.33f, false);
            this.mCurrentAnimation = SummonDeath.MagickDeath.Animations.Disappear;
            this.mAlphaTimer = 1f;
          }
        }
        else if (this.mCurrentAnimation == SummonDeath.MagickDeath.Animations.Disappear)
        {
          this.mAlphaTimer -= iDeltaTime;
          this.mRenderData[(int) iDataChannel].mMaterial.Alpha = Math.Max(this.mAlphaTimer, 0.0f);
          if (this.mDeathCue == null)
          {
            this.mDeathCue = AudioManager.Instance.PlayCue(Banks.Characters, SummonDeath.MagickDeath.DEATH_SOUND, this.AudioEmitter);
            EffectManager.Instance.StartEffect(SummonDeath.MagickDeath.DESPAWN_EFFECT, ref this.mOrientation, out VisualEffectReference _);
          }
          this.mTimeTargetModifier = 1f;
          if (!this.mController.CrossFadeEnabled && this.mController.HasFinished)
            this.mDead = true;
        }
        if ((double) this.mMovement.LengthSquared() <= 0.5)
          return;
        SummonDeath.MagickDeath.AfterImageRenderData iObject = this.mAfterImageRenderData[(int) iDataChannel];
        if (iObject.MeshDirty)
        {
          ModelMesh mesh = this.mModel.Model.Meshes[0];
          ModelMeshPart meshPart = mesh.MeshParts[0];
          iObject.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, SkinnedModelBasicEffect.TYPEHASH);
        }
        int count = this.mModel.SkeletonBones.Count;
        this.mAfterImageTimer -= iDeltaTime;
        this.mAfterImageIntensity -= iDeltaTime;
        while ((double) this.mAfterImageTimer <= 0.0)
        {
          this.mAfterImageTimer += 0.0375f;
          if ((double) this.mSpeed > 0.0)
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
        if (!this.IsEthereal)
          this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
        else
          this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
      }
    }

    public override void Deinitialize()
    {
      if (this.mPursuitCue != null && !this.mPursuitCue.IsStopping)
        this.mPursuitCue.Stop(AudioStopOptions.AsAuthored);
      this.mPlayState.TimeModifier = 1f;
      base.Deinitialize();
      this.mDead = true;
    }

    public override bool Dead => this.mDead;

    public override bool Removable => this.mDead;

    public override void Kill() => this.Deinitialize();

    public float HitPoints => 1f;

    public float MaxHitPoints => 1f;

    internal override float GetDanger() => 1000f;

    public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
    {
      return this.mBody.CollisionSkin.SegmentIntersect(out float _, out oPosition, out Vector3 _, iSeg);
    }

    public DamageResult InternalDamage(
      DamageCollection5 iDamages,
      Entity iAttacker,
      double iTimeStamp,
      Vector3 iAttackPosition,
      Defines.DamageFeatures iFeatures)
    {
      return this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    }

    public DamageResult InternalDamage(
      Damage iDamage,
      Entity iAttacker,
      double iTimeStamp,
      Vector3 iAttackPosition,
      Defines.DamageFeatures iFeatures)
    {
      if ((iDamage.Element & Elements.Life) != Elements.Life || this.IsEthereal)
        return DamageResult.None;
      if (Defines.FeatureDamage(iFeatures))
        this.mIsHit = true;
      return DamageResult.Hit;
    }

    public void Electrocute(IDamageable iTarget, float iMultiplyer)
    {
    }

    public void OverKill() => this.Kill();

    protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
    {
      oMsg = new EntityUpdateMessage();
      oMsg.Features |= EntityFeatures.Position;
      oMsg.Position = this.Position;
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

    public bool IsEthereal
    {
      get => this.mIsEthereal;
      set => this.mIsEthereal = value;
    }

    public class ItemRenderData : 
      RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>,
      IRenderableAdditiveObject
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

    public class DeferredRenderData : 
      RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>,
      IRenderableAdditiveObject
    {
      public bool RenderAdditive;
      public Matrix[] mBones;

      public DeferredRenderData() => this.mBones = new Matrix[80 /*0x50*/];

      public override int Technique => this.RenderAdditive ? 2 : base.Technique;

      public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
      {
        (iEffect as SkinnedModelDeferredEffect).Bones = this.mBones;
        base.Draw(iEffect, iViewFrustum);
      }

      public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
      {
        (iEffect as SkinnedModelDeferredEffect).Bones = this.mBones;
        base.DrawShadow(iEffect, iViewFrustum);
      }
    }

    protected class AfterImageRenderData : IRenderableObject, IRenderableAdditiveObject
    {
      protected static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);
      public BoundingSphere mBoundingSphere;
      protected int mEffect;
      protected VertexDeclaration mVertexDeclaration;
      protected int mBaseVertex;
      protected int mNumVertices;
      protected int mPrimitiveCount;
      protected int mStartIndex;
      protected int mStreamOffset;
      protected int mVertexStride;
      protected VertexBuffer mVertexBuffer;
      protected IndexBuffer mIndexBuffer;
      public float mIntensity;
      public Vector3 Color;
      public Matrix[][] mSkeleton;
      private SkinnedModelMaterial mMaterial;
      protected int mVerticesHash;
      protected bool mMeshDirty = true;

      public AfterImageRenderData(Matrix[][] iSkeleton) => this.mSkeleton = iSkeleton;

      public bool MeshDirty => this.mMeshDirty;

      public int Effect => this.mEffect;

      public int DepthTechnique => 3;

      public int Technique => 1;

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
        SkinnedModelBasicEffect iEffect1 = iEffect as SkinnedModelBasicEffect;
        this.mMaterial.AssignToEffect(iEffect1);
        iEffect1.Colorize = new Vector4(SummonDeath.MagickDeath.AfterImageRenderData.ColdColor, 1f);
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

      public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
      {
      }

      public void SetMeshDirty() => this.mMeshDirty = true;

      public void SetMesh(
        VertexBuffer iVertices,
        IndexBuffer iIndices,
        ModelMeshPart iMeshPart,
        int iEffectHash)
      {
        this.mMeshDirty = false;
        SkinnedModelMaterial.CreateFromEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mMaterial);
        this.mVertexBuffer = iVertices;
        this.mVerticesHash = iVertices.GetHashCode();
        this.mIndexBuffer = iIndices;
        this.mEffect = iEffectHash;
        this.mVertexDeclaration = iMeshPart.VertexDeclaration;
        this.mBaseVertex = iMeshPart.BaseVertex;
        this.mNumVertices = iMeshPart.NumVertices;
        this.mPrimitiveCount = iMeshPart.PrimitiveCount;
        this.mStartIndex = iMeshPart.StartIndex;
        this.mStreamOffset = iMeshPart.StreamOffset;
        this.mVertexStride = iMeshPart.VertexStride;
        for (int index1 = 0; index1 < this.mSkeleton.Length; ++index1)
        {
          Matrix[] matrixArray = this.mSkeleton[index1];
          for (int index2 = 0; index2 < matrixArray.Length; ++index2)
            matrixArray[index2].M11 = matrixArray[index2].M22 = matrixArray[index2].M33 = matrixArray[index2].M44 = float.NaN;
        }
      }
    }

    public enum Animations
    {
      Move,
      Scythe_Rise,
      Scythe_Fall,
      Disappear,
      NrOfAnimations,
    }
  }
}
