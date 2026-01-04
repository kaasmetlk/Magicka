// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.WaveEntity
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;
using XNAnimation;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class WaveEntity : Barrier
{
  private static List<WaveEntity> mWaveCache;
  private Wave mParent;
  private float mAnimationTimer;
  private static SkinnedModel[] sWaveBarrierModels;
  private static AnimationClip[] sWaveAppearClips;

  public new static void InitializeCache(int iNrOfWaves, PlayState iPlayState)
  {
    WaveEntity.mWaveCache = new List<WaveEntity>(iNrOfWaves);
    for (int index = 0; index < iNrOfWaves; ++index)
      WaveEntity.mWaveCache.Add(new WaveEntity(iPlayState));
  }

  public static WaveEntity GetFromCache(PlayState iPlayState)
  {
    if (WaveEntity.mWaveCache.Count <= 0)
      return new WaveEntity(iPlayState);
    WaveEntity fromCache = WaveEntity.mWaveCache[WaveEntity.mWaveCache.Count - 1];
    WaveEntity.mWaveCache.RemoveAt(WaveEntity.mWaveCache.Count - 1);
    return fromCache;
  }

  public static void ReturnToCache(WaveEntity iWave)
  {
    if (WaveEntity.mWaveCache.Contains(iWave))
      return;
    WaveEntity.mWaveCache.Add(iWave);
  }

  public WaveEntity(PlayState iPlayState)
    : base(iPlayState)
  {
    SkinnedModel skinnedModel;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      WaveEntity.sWaveBarrierModels = new SkinnedModel[1];
      WaveEntity.sWaveBarrierModels[0] = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/rockpillar0");
      skinnedModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/rockpillar_animation");
    }
    WaveEntity.sWaveAppearClips = new AnimationClip[skinnedModel.AnimationClips.Count];
    int num = 0;
    foreach (AnimationClip animationClip in skinnedModel.AnimationClips.Values)
      WaveEntity.sWaveAppearClips[num++] = animationClip;
  }

  public void Initialize(
    ISpellCaster iOwner,
    Vector3 iPosition,
    Vector3 iDirection,
    float iScale,
    float iRange,
    Vector3 iNextDirection,
    Quaternion iNextRotation,
    float iDistanceBetweenBarriers,
    ref Spell iSpell,
    ref DamageCollection5 iDamage,
    ref Barrier.HitListWithBarriers iHitList,
    AnimatedLevelPart iAnimation,
    ref Wave iWave)
  {
    if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
      this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
    this.mEarthAnimationController.Stop();
    EffectManager.Instance.Stop(ref this.mEffect);
    if ((this.mDrawMethod & Barrier.DrawMethod.PARTICLEWALL) == Barrier.DrawMethod.PARTICLEWALL)
      this.mDrawMethod &= ~Barrier.DrawMethod.PARTICLEWALL;
    if (this.mHitList != null)
    {
      this.mHitList.Owners.Remove((Barrier) this);
      if (this.mHitList.Owners.Count == 0)
        this.mHitList.Destroy();
    }
    this.mHitList = (Barrier.HitListWithBarriers) null;
    this.mParent = iWave;
    this.mAnimationTimer = WaveEntity.sWaveAppearClips[0].Duration;
    this.mInitilizeDamage = false;
    iAnimation?.AddEntity((Entity) this);
    this.mSpell = iSpell;
    this.mHitList = iHitList;
    this.mHitList.Owners.Add((Barrier) this);
    this.mDirection = iDirection;
    this.mEarthAnimationController.PlaybackMode = PlaybackMode.Forward;
    this.mNextBarrierTTL = 0.075f;
    Vector3.Transform(ref iNextDirection, ref iNextRotation, out this.mNextBarrierDir);
    this.mNextBarrierRotation = iNextRotation;
    this.mNextBarrierRange = iRange - iDistanceBetweenBarriers;
    this.mDistanceBetweenBarriers = iDistanceBetweenBarriers;
    this.mScale = iScale;
    this.mRuneRotation = 0.0f;
    this.mNormalizedDamage = 0.0f;
    this.mNormalizedDamageTarget = 0.0f;
    this.mOwner = iOwner;
    if (iOwner != null)
      this.mTimeStamp = iOwner.PlayState.PlayTime;
    else
      this.mTimeStamp = this.PlayState.PlayTime;
    this.mDamageTimer = 0.0f;
    this.mDrawMethod = Barrier.DrawMethod.NONE;
    this.mDamageSelf = true;
    if ((this.mSpell.Element & ~Elements.Shield & Elements.PhysicalElements) != Elements.None)
      this.mBarrierType = Barrier.BarrierType.SOLID;
    else
      this.mBarrierType = Barrier.BarrierType.ELEMENTAL;
    DamageCollection5 oDamages = new DamageCollection5();
    if (this.mBarrierType == Barrier.BarrierType.SOLID)
    {
      this.mRadius = Barrier.GetRadius(true) * this.mScale;
      if ((iSpell.Element & Elements.PhysicalElements) == Elements.PhysicalElements)
        this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.Ice_Earth_Barrier_Sound_Hash);
      else if ((iSpell.Element & Elements.Ice) == Elements.Ice)
        this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.Ice_Barrier_Sound_Hash);
      else if ((iSpell.Element & Elements.Earth) == Elements.Earth)
        this.mSoundCue = AudioManager.Instance.GetCue(Banks.Spells, Barrier.Earth_Barrier_Sound_Hash);
      this.mAddedEffect = false;
      this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out oDamages);
      if ((iSpell.Element & Elements.Earth) == Elements.Earth)
      {
        this.mSpell.CalculateDamage(SpellType.Projectile, CastType.Area, out oDamages);
        Vector3 forward = this.Body.Orientation.Forward;
        SkinnedModel earthBarrierModel = Barrier.sEarthBarrierModels[Barrier.mRandom.Next(WaveEntity.sWaveBarrierModels.Length)];
        AnimationClip[] sWaveAppearClips = WaveEntity.sWaveAppearClips;
        if ((iSpell.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Steam | Elements.Poison)) != Elements.None && (iSpell.Element & Elements.Beams) == Elements.None)
        {
          this.mAddedEffect = true;
          this.mEffectAttach = Barrier.sVulcanoAttach;
          Elements iElement1 = Elements.None;
          float num = -1f;
          for (int iIndex = 0; iIndex < 11; ++iIndex)
          {
            Elements iElement2 = Defines.ElementFromIndex(iIndex);
            if ((iElement2 & Elements.Lightning) == Elements.Lightning && (this.mSpell.Element & Elements.Lightning) == Elements.Lightning)
            {
              iElement1 = iElement2;
              num = this.mSpell[iElement1];
              iIndex = 11;
            }
            else if ((iElement2 & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam)) != Elements.None && (double) this.mSpell[iElement2] > (double) num)
            {
              iElement1 = iElement2;
              num = this.mSpell[iElement1];
            }
          }
          if ((iElement1 & Elements.Steam) == Elements.Steam)
            this.mEffectTTL = this.mSpell.SteamMagnitude * 5f;
          if ((iElement1 & Elements.Water) == Elements.Water)
            this.mEffectTTL = this.mSpell.WaterMagnitude * 5f;
          Matrix.Identity.Translation = iPosition;
        }
        ModelMesh mesh = earthBarrierModel.Model.Meshes[0];
        ModelMeshPart meshPart = mesh.MeshParts[0];
        this.mEarthAnimationController.Skeleton = earthBarrierModel.SkeletonBones;
        float num1 = MathHelper.Clamp((float) (((double) iSpell.EarthMagnitude - 1.0) * 0.33333298563957214), 0.0f, 1f);
        for (int index = 0; index < 3; ++index)
        {
          Barrier.RenderData renderData = this.mEarthRenderData[index];
          renderData.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart);
          renderData.mSkinnedModelMaterial.OverlayAlpha = num1;
        }
        this.mEarthAnimationController.StartClip(sWaveAppearClips[Barrier.mRandom.Next(sWaveAppearClips.Length)], false);
        this.mDrawMethod |= Barrier.DrawMethod.NORMAL;
      }
    }
    Matrix orientation;
    MagickaMath.MakeOrientationMatrix(ref iDirection, out orientation);
    Vector3 pos = iPosition;
    pos.Y += (float) ((double) Barrier.GetRadius(this.Solid) * (double) this.mScale + 0.05000000074505806);
    Matrix scale = Matrix.CreateScale(this.mScale);
    Matrix result;
    Matrix.Multiply(ref scale, ref orientation, out result);
    this.mBody.MoveTo(pos, result);
    this.mBody.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
    (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius = this.mRadius;
    (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Radius = this.mRadius;
    (this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Radius = this.mRadius;
    float num2 = this.Solid ? this.mRadius * 2f : this.mRadius;
    (this.mCollision.GetPrimitiveLocal(1) as Sphere).Radius = num2;
    (this.mCollision.GetPrimitiveLocal(1) as Sphere).Position = new Vector3(0.0f, -this.mRadius, 0.0f);
    (this.mCollision.GetPrimitiveNewWorld(1) as Sphere).Radius = num2;
    (this.mCollision.GetPrimitiveNewWorld(1) as Sphere).Position = new Vector3(0.0f, -this.mRadius, 0.0f);
    (this.mCollision.GetPrimitiveOldWorld(1) as Sphere).Radius = num2;
    (this.mCollision.GetPrimitiveOldWorld(1) as Sphere).Position = new Vector3(0.0f, -this.mRadius, 0.0f);
    this.mVolume = (this.mCollision.GetPrimitiveLocal(0) as Capsule).GetVolume();
    List<Entity> entities1 = this.mPlayState.EntityManager.GetEntities(iPosition, (float) ((double) iScale * (double) Barrier.GetRadius(this.Solid) * 0.800000011920929), false);
    for (int index = 0; index < entities1.Count; ++index)
    {
      Barrier barrier = entities1[index] as Barrier;
      SpellMine spellMine = entities1[index] as SpellMine;
      if (barrier != null && this.mHitList != barrier.HitList)
        barrier.Kill();
      else
        spellMine?.Detonate();
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities1);
    if (this.mInitilizeDamage)
    {
      List<Entity> entities2 = this.mPlayState.EntityManager.GetEntities(iPosition, (float) ((double) iScale * (double) Barrier.GetRadius(this.Solid) * 1.25), false);
      for (int index = 0; index < entities2.Count; ++index)
      {
        IDamageable t = entities2[index] as IDamageable;
        if (t != this && t != null && t != this.mOwner && !this.HitList.HitList.ContainsKey(t.Handle) && (!(t is Barrier) || (t as Barrier).HitList != this.HitList))
        {
          int num3 = (int) t.Damage(oDamages, iOwner as Entity, this.mTimeStamp, iPosition);
          this.mHitList.HitList.Add(t.Handle, 0.25f);
        }
      }
      this.mPlayState.EntityManager.ReturnEntityList(entities2);
    }
    this.mDamage = iDamage;
    if (this.Solid)
      this.mHitPoints = (float) ((1.0 + (double) iSpell[Elements.Earth]) * 500.0);
    else
      this.mHitPoints = iSpell.TotalMagnitude() * 100f;
    this.mArmour = (int) ((double) iSpell[Elements.Ice] * 50.0);
    this.mMaxHitPoints = this.mHitPoints;
    (this.mBody.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Orientation = Matrix.CreateRotationX(-1.57079637f);
    this.Initialize();
    if (this.mSoundCue != null)
    {
      this.mSoundCue.Apply3D(this.mPlayState.Camera.Listener, this.AudioEmitter);
      this.mSoundCue.Play();
    }
    this.mDamage.A.Element = Elements.Earth;
    this.mDamage.B.Element = Elements.Earth;
    this.mDamage.C.Element = Elements.Earth;
    this.mDamage.D.Element = Elements.Earth;
    this.mDamage.E.Element = Elements.Earth;
    this.mDamage.A.AttackProperty = AttackProperties.Knockback | AttackProperties.Damage;
    this.mDamage.B.AttackProperty = AttackProperties.Knockback;
    this.mDamage.C.AttackProperty = AttackProperties.Knockback;
    this.mDamage.D.AttackProperty = AttackProperties.Knockback;
    this.mDamage.E.AttackProperty = AttackProperties.Knockback;
    this.mDamage.A.Amount = 0.0f;
    this.mDamage.B.Amount = 0.0f;
    this.mDamage.C.Amount = 0.0f;
    this.mDamage.D.Amount = 0.0f;
    this.mDamage.E.Amount = 0.0f;
    this.mDamage.A.Magnitude = 1f;
    this.mNextBarrierTTL = 0.1f;
  }

  public override void Deinitialize()
  {
    if (this.mSoundCue != null && !this.mSoundCue.IsStopping)
      this.mSoundCue.Stop(AudioStopOptions.AsAuthored);
    this.mEarthAnimationController.Stop();
    EffectManager.Instance.Stop(ref this.mEffect);
    if (this.mHitList != null)
    {
      this.mHitList.Owners.Remove((Barrier) this);
      if (this.mHitList.Owners.Count == 0)
        this.mHitList.Destroy();
    }
    this.mHitList = (Barrier.HitListWithBarriers) null;
    Entity entity;
    if (Entity.mUniqueEntities.TryGetValue(this.mUniqueID, out entity) && entity == this)
      Entity.mUniqueEntities.Remove(this.mUniqueID);
    this.mBody.DisableBody();
    WaveEntity.ReturnToCache(this);
  }

  private new void SpawnNextBarrier()
  {
    Vector3 vector3 = this.Position + this.mNextBarrierDir;
    Segment iSeg1 = new Segment();
    iSeg1.Delta.Y = -1.5f;
    iSeg1.Origin = vector3;
    iSeg1.Origin.Y += (float) (0.75 - (0.05000000074505806 + (double) this.Radius));
    List<Shield> shields = this.mPlayState.EntityManager.Shields;
    Segment iSeg2 = new Segment();
    iSeg2.Origin = this.Position;
    Vector3.Subtract(ref iSeg1.Origin, ref iSeg2.Origin, out iSeg2.Delta);
    bool flag = false;
    for (int index = 0; index < shields.Count; ++index)
    {
      if (shields[index].SegmentIntersect(out Vector3 _, iSeg2, 1f))
        flag = true;
    }
    if (flag)
      return;
    Vector3 oPos = new Vector3();
    AnimatedLevelPart oAnimatedLevelPart;
    if (!this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, out oAnimatedLevelPart, iSeg1))
      return;
    Vector3 result = this.mBody.Orientation.Forward;
    Vector3.Transform(ref result, ref this.mNextBarrierRotation, out result);
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(oPos, 1.5f * this.mRadius, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Barrier && (entities[index] as Barrier).HitList != this.mHitList)
        entities[index].Kill();
      else if (entities[index] is SpellMine)
        (entities[index] as SpellMine).Detonate();
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    WaveEntity fromCache = WaveEntity.GetFromCache(this.mPlayState);
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      SpawnWaveMessage iMessage;
      iMessage.Handle = fromCache.Handle;
      iMessage.OwnerHandle = this.Owner.Handle;
      iMessage.AnimationHandle = oAnimatedLevelPart == null ? ushort.MaxValue : oAnimatedLevelPart.Handle;
      iMessage.Position = oPos;
      iMessage.Direction = result;
      iMessage.Scale = this.mScale;
      iMessage.Spell = this.mSpell;
      iMessage.Damage = this.mDamage;
      iMessage.HitlistHandle = this.mHitList.Handle;
      iMessage.ParentHandle = (ushort) 0;
      NetworkManager.Instance.Interface.SendMessage<SpawnWaveMessage>(ref iMessage);
    }
    fromCache.Initialize(this.Owner, oPos, result, this.mScale, this.mNextBarrierRange, this.mNextBarrierDir, this.mNextBarrierRotation, this.mDistanceBetweenBarriers, ref this.mSpell, ref this.mDamage, ref this.mHitList, oAnimatedLevelPart, ref this.mParent);
    this.mParent.AddEntity(fromCache);
    this.mPlayState.EntityManager.AddEntity((Entity) fromCache);
  }

  public Wave Parent() => this.mParent;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    Matrix orientation = this.GetOrientation();
    this.mAudioEmitter.Position = orientation.Translation;
    this.mAudioEmitter.Forward = orientation.Forward;
    this.mAudioEmitter.Up = orientation.Up;
    if (this.mSoundCue != null)
      this.mSoundCue.Apply3D(this.mPlayState.Camera.Listener, this.AudioEmitter);
    this.mEffectTTL -= iDeltaTime;
    this.mRuneRotation -= iDeltaTime * 0.25f;
    if ((double) this.mNextBarrierRange > 1.4012984643248171E-45)
    {
      this.mNextBarrierTTL -= iDeltaTime;
      if ((double) this.mNextBarrierTTL < 0.0)
      {
        this.SpawnNextBarrier();
        this.mNextBarrierRange = 0.0f;
      }
    }
    if (this.mHitList.Owners[0] == this)
      this.mHitList.HitList.Update(iDeltaTime);
    this.mAnimationTimer -= iDeltaTime;
    if ((double) this.mAnimationTimer < 0.0)
      this.mHitPoints = 0.0f;
    if ((double) this.mBody.Velocity.LengthSquared() > 9.9999999747524271E-07)
      this.mRestingMovementTimer = 1f;
    else
      this.mRestingMovementTimer -= iDeltaTime;
    this.mNormalizedDamageTarget = (float) (1.0 - (double) this.mHitPoints / (double) this.mMaxHitPoints);
    this.mNormalizedDamage = this.mNormalizedDamageTarget + (this.mNormalizedDamage - this.mNormalizedDamageTarget) * (float) Math.Pow(0.15, (double) iDeltaTime);
    if ((this.mSpell.Element & Elements.Earth) != Elements.None)
      this.mEarthAnimationController.Update(iDeltaTime, ref orientation, true);
    if (this.mAddedEffect)
    {
      if ((double) this.mEffectTTL > 0.0)
      {
        Matrix result;
        Matrix.Multiply(ref this.mEffectAttach.mBindPose, ref this.mEarthAnimationController.SkinnedBoneTransforms[this.mEffectAttach.mIndex], out result);
        EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref result);
      }
      else
      {
        EffectManager.Instance.Stop(ref this.mEffect);
        this.mAddedEffect = false;
      }
    }
    Barrier.RenderData iObject = this.mEarthRenderData[(int) iDataChannel];
    iObject.mBoundingSphere.Center = this.Position;
    iObject.mBoundingSphere.Radius = this.mRadius * 3f;
    iObject.mDamage = this.mNormalizedDamage;
    Array.Copy((Array) this.mEarthAnimationController.SkinnedBoneTransforms, (Array) iObject.mBones, this.mEarthAnimationController.Skeleton.Count);
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
  }

  protected override bool OnCollision(
    CollisionSkin iSkin0,
    int iPrim0,
    CollisionSkin iSkin1,
    int iPrim1)
  {
    if (iSkin1.Owner == null || iSkin1.Owner.Tag is Barrier)
      return false;
    if (iSkin1.Owner.Tag is IDamageable tag && !tag.Dead)
    {
      switch (tag)
      {
        case Magicka.GameLogic.Entities.Character character when character.IsEthereal:
          return this.mBarrierType == Barrier.BarrierType.SOLID & iPrim0 == 0;
        case Magicka.GameLogic.Entities.Character _ when character.CharacterBody.IsTouchingGround && !this.mParent.InHitlist(tag):
          int num = (int) tag.Damage(this.mDamage, this.Owner as Entity, this.mTimeStamp, this.Position);
          this.mParent.AddToHitlist(tag.Handle);
          if (!character.IsLevitating && !character.IsImmortal && !character.IsInAEvent && !character.IsEthereal && (double) character.CharacterBody.Mass < 3000.0)
          {
            Vector3 iImpulse = new Vector3(0.0f, 10f, 0.0f);
            character.CharacterBody.AddImpulseVelocity(ref iImpulse);
            break;
          }
          break;
      }
    }
    return this.mBarrierType == Barrier.BarrierType.SOLID & iPrim0 == 0;
  }

  public override bool Removable => this.Dead;

  public override void Kill()
  {
  }
}
