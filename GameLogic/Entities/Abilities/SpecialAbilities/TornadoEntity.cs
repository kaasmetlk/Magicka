// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.TornadoEntity
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Magicka.PathFinding;
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
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class TornadoEntity : Entity
{
  private static List<TornadoEntity> sCache;
  private static readonly float RADIUS = 2.5f;
  private static readonly float LENGTH = 6f;
  private static readonly Random RANDOM = new Random();
  private float mTTL;
  private TornadoEntity.DeferredRenderData[] mRenderData;
  private TornadoEntity.DeferredRenderData[] mDebriRenderData;
  private AnimationController mController;
  private AnimationClip mClip;
  private ISpellCaster mOwner;
  private Vector3 mHeading;
  private VisualEffectReference mDustEffect;
  private HitList mHitList;
  private List<PathNode> mPath;
  private Vector3 mTargetPosition;
  private DamageCollection5 mDamage;
  private Cue mAmbience;
  private float mAlpha;
  private double mTimeStamp;

  public static TornadoEntity GetInstance()
  {
    TornadoEntity instance;
    lock (TornadoEntity.sCache)
    {
      instance = TornadoEntity.sCache[0];
      TornadoEntity.sCache.RemoveAt(0);
      TornadoEntity.sCache.Add(instance);
    }
    return instance;
  }

  public static TornadoEntity GetSpecificInstance(ushort iHandle)
  {
    TornadoEntity fromHandle;
    lock (TornadoEntity.sCache)
    {
      fromHandle = Entity.GetFromHandle((int) iHandle) as TornadoEntity;
      TornadoEntity.sCache.Remove(fromHandle);
      TornadoEntity.sCache.Add(fromHandle);
    }
    return fromHandle;
  }

  public static void InitializeCache(int iNr)
  {
    TornadoEntity.sCache = new List<TornadoEntity>(iNr);
    for (int index = 0; index < iNr; ++index)
      TornadoEntity.sCache.Add(new TornadoEntity((PlayState) null));
  }

  public TornadoEntity(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mHitList = new HitList(32 /*0x20*/);
    SkinnedModel skinnedModel1;
    SkinnedModel skinnedModel2;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      skinnedModel1 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Magicks/Tornado");
      skinnedModel2 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Magicks/Tornado_debri");
    }
    this.mController = new AnimationController();
    this.mController.Skeleton = skinnedModel1.SkeletonBones;
    this.mClip = skinnedModel1.AnimationClips["tornado"];
    this.mPath = new List<PathNode>(8);
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Capsule(new Vector3(), Matrix.CreateRotationX(-1.57079637f), TornadoEntity.RADIUS, TornadoEntity.LENGTH), 1, new MaterialProperties());
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.Immovable = true;
    this.mBody.Tag = (object) this;
    this.mDamage = new DamageCollection5();
    this.mDamage.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, 800f, 2f));
    SkinnedModelDeferredBasicMaterial oMaterial1;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(skinnedModel1.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out oMaterial1);
    SkinnedModelDeferredBasicMaterial oMaterial2;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(skinnedModel1.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out oMaterial2);
    this.mRenderData = new TornadoEntity.DeferredRenderData[3];
    this.mDebriRenderData = new TornadoEntity.DeferredRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new TornadoEntity.DeferredRenderData();
      this.mRenderData[index].SetMesh(skinnedModel1.Model.Meshes[0].VertexBuffer, skinnedModel1.Model.Meshes[0].IndexBuffer, skinnedModel1.Model.Meshes[0].MeshParts[0], 2);
      this.mRenderData[index].mMaterial = oMaterial1;
      this.mDebriRenderData[index] = new TornadoEntity.DeferredRenderData();
      this.mDebriRenderData[index].SetMesh(skinnedModel2.Model.Meshes[0].VertexBuffer, skinnedModel2.Model.Meshes[0].IndexBuffer, skinnedModel2.Model.Meshes[0].MeshParts[0], 2);
      this.mDebriRenderData[index].mMaterial = oMaterial2;
    }
  }

  public void Initialize(PlayState iPlayState, Matrix iOrientation, ISpellCaster iOwner)
  {
    if (this.mAmbience != null && !this.mAmbience.IsStopping)
      this.mAmbience.Stop(AudioStopOptions.AsAuthored);
    EffectManager.Instance.Stop(ref this.mDustEffect);
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    this.mTimeStamp = this.mPlayState.PlayTime;
    this.mPath.Clear();
    this.mHeading = iOrientation.Forward;
    this.mController.StartClip(this.mClip, true);
    this.mController.Speed = 4f;
    this.mTTL = 15f;
    this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Tornado.AMBIENCE, this.AudioEmitter);
    this.Initialize();
    this.mBody.MoveTo(iOrientation.Translation, iOrientation);
    this.mAlpha = 0.0f;
  }

  public override void Deinitialize()
  {
    base.Deinitialize();
    if (this.mAmbience != null && !this.mAmbience.IsStopping)
      this.mAmbience.Stop(AudioStopOptions.AsAuthored);
    EffectManager.Instance.Stop(ref this.mDustEffect);
  }

  public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner != null && !(iSkin1.Owner.Tag is MissileEntity) && iSkin1.Owner.Tag is IDamageable tag && !this.mHitList.ContainsKey(tag.Handle))
    {
      Vector3 position = tag.Position;
      Vector3 result1 = this.Position;
      Vector3 result2;
      Vector3.Subtract(ref position, ref result1, out result2);
      result2.Normalize();
      Vector3.Multiply(ref result2, TornadoEntity.RADIUS, out result2);
      Vector3.Add(ref result1, ref result2, out result1);
      Matrix identity = Matrix.Identity with
      {
        Translation = result1
      };
      EffectManager.Instance.StartEffect(Tornado.HIT_EFFECT, ref identity, out VisualEffectReference _);
      int num = (int) tag.Damage(this.mDamage, this.mOwner as Entity, this.mTimeStamp, result1);
      this.mHitList.Add(tag.Handle, 1f);
    }
    return false;
  }

  public override bool Dead => (double) this.mTTL < 0.0;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    this.mAlpha = (float) ((double) Math.Min(15f - this.mTTL, 0.5f) * (double) Math.Min(this.mTTL, 1f) * 2.0);
    this.mTTL -= iDeltaTime;
    this.mHitList.Update(iDeltaTime);
    Vector3 position = this.Position;
    if (this.mPath.Count > 0)
    {
      this.mTargetPosition = this.mPath[0].Position;
      float result;
      for (Vector3.DistanceSquared(ref position, ref this.mTargetPosition, out result); this.mPath.Count > 0 && (double) result <= 1.0; Vector3.DistanceSquared(ref position, ref this.mTargetPosition, out result))
      {
        this.mTargetPosition = this.mPath[0].Position;
        this.mPath.RemoveAt(0);
      }
    }
    else
    {
      float result = float.Epsilon;
      NavMesh navMesh = this.mPlayState.Level.CurrentScene.NavMesh;
      this.mPath.Clear();
      Vector2 vector = new Vector2(this.mHeading.X, this.mHeading.Z);
      vector.Normalize();
      float num1 = MagickaMath.Angle(vector) + (float) ((TornadoEntity.RANDOM.NextDouble() - 0.5) * 1.5707963705062866);
      float num2 = (float) (5.0 + TornadoEntity.RANDOM.NextDouble() * 10.0);
      this.mTargetPosition.Y = position.Y;
      this.mTargetPosition.X = position.X + (float) Math.Cos((double) num1) * num2;
      this.mTargetPosition.Z = position.Z + (float) Math.Sin((double) num1) * num2;
      if (navMesh.FindShortestPath(ref position, ref this.mTargetPosition, this.mPath, MovementProperties.Default))
      {
        this.mPath.RemoveAt(0);
        this.mTargetPosition = this.mPath[0].Position;
        this.mPath.RemoveAt(0);
        Vector3.DistanceSquared(ref position, ref this.mTargetPosition, out result);
      }
    }
    Vector3 result1;
    Vector3.Subtract(ref this.mTargetPosition, ref position, out result1);
    float num = (float) Math.Abs(Math.Sin((double) this.mTTL) * 4.0);
    result1.Normalize();
    this.mHeading = result1;
    position.X += result1.X * num * iDeltaTime;
    position.Z += result1.Z * num * iDeltaTime;
    Segment iSeg = new Segment();
    iSeg.Origin = position;
    iSeg.Delta.Y -= 3f;
    Vector3 oPos;
    if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
      position.Y += (oPos.Y - position.Y) * iDeltaTime;
    Matrix orientation = this.mBody.Orientation with
    {
      Translation = position
    };
    MagickaMath.UniformMatrixScale(ref orientation, 0.75f);
    this.mBody.MoveTo(position, Matrix.Identity);
    if (!EffectManager.Instance.UpdateOrientation(ref this.mDustEffect, ref orientation))
      EffectManager.Instance.StartEffect(Tornado.EFFECT, ref orientation, out this.mDustEffect);
    this.mController.Update(iDeltaTime, ref orientation, true);
    TornadoEntity.DeferredRenderData deferredRenderData1 = this.mRenderData[(int) iDataChannel];
    TornadoEntity.DeferredRenderData deferredRenderData2 = this.mDebriRenderData[(int) iDataChannel];
    deferredRenderData2.mMaterial.Alpha = deferredRenderData1.mMaterial.Alpha = this.mAlpha;
    deferredRenderData2.mBoundingSphere.Center = deferredRenderData1.mBoundingSphere.Center = position;
    deferredRenderData2.mBoundingSphere.Radius = deferredRenderData1.mBoundingSphere.Radius = 2f;
    this.mController.SkinnedBoneTransforms.CopyTo((Array) deferredRenderData2.mBones, 0);
    this.mController.SkinnedBoneTransforms.CopyTo((Array) deferredRenderData1.mBones, 0);
  }

  protected override void AddImpulseVelocity(ref Vector3 iVelocity)
  {
    base.AddImpulseVelocity(ref iVelocity);
    Vector3 position = this.Position;
    float result = float.Epsilon;
    NavMesh navMesh = this.mPlayState.Level.CurrentScene.NavMesh;
    this.mPath.Clear();
    new Vector2(iVelocity.X, iVelocity.Z).Normalize();
    float num = 10f;
    iVelocity.Normalize();
    this.mTargetPosition.Y = position.Y;
    this.mTargetPosition.X = position.X + iVelocity.X * num;
    this.mTargetPosition.Z = position.Z + iVelocity.Z * num;
    if (!navMesh.FindShortestPath(ref position, ref this.mTargetPosition, this.mPath, MovementProperties.Default))
      return;
    this.mPath.RemoveAt(0);
    this.mTargetPosition = this.mPath[0].Position;
    this.mPath.RemoveAt(0);
    Vector3.DistanceSquared(ref position, ref this.mTargetPosition, out result);
  }

  public override bool Removable => this.Dead;

  public override void Kill() => this.mTTL = 1f;

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = this.Position;
  }

  internal override float GetDanger() => 10f * this.mAlpha;

  protected class DeferredRenderData : 
    RenderableAdditiveObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
  {
    public Matrix[] mBones;

    public DeferredRenderData() => this.mBones = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.mBones;
      base.Draw(iEffect, iViewFrustum);
    }
  }
}
