// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.FrogTongue
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;
using XNAnimation;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal class FrogTongue : Entity, IDamageable
{
  private const float SHOOT_VELOCITY = 60f;
  private const float DRAG_VELOCITY = 30f;
  private const float PULL_VELOCITY = 80f;
  private static List<FrogTongue> sCache;
  private static readonly int HIT_GROUND_SPLASH_EFFECT = "generic_dust".GetHashCodeCustom();
  private static readonly int HIT_GREEN_SPLASH_EFFECT = Gib.GORE_GIB_MEDIUM_EFFECTS[1];
  private Character mAttachedCharacter;
  private Character mOwner;
  private Matrix[] mSkeleton;
  private FrogTongue.RenderData[] mRenderData;
  private bool mPullingBack;
  private float mMaxLengthSquared;
  private Matrix mStartBindPose;
  private Matrix mEndBindPose;
  private Matrix mOrientation;
  private Vector3 mStartPoint;
  private Vector3 mEndPoint;
  private float mVelocity;

  public static FrogTongue GetInstance(PlayState iPlayState)
  {
    if (FrogTongue.sCache.Count <= 0)
      return new FrogTongue(iPlayState);
    FrogTongue instance = FrogTongue.sCache[FrogTongue.sCache.Count - 1];
    FrogTongue.sCache.RemoveAt(FrogTongue.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    FrogTongue.sCache = new List<FrogTongue>(iNr);
    for (int index = 0; index < iNr; ++index)
      FrogTongue.sCache.Add(new FrogTongue(iPlayState));
  }

  public FrogTongue(PlayState iPlayState)
    : base(iPlayState)
  {
    SkinnedModel skinnedModel;
    lock (Magicka.Game.Instance.GraphicsDevice)
      skinnedModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Missiles/Tongue");
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Sphere(Vector3.Zero, 0.5f), 1, new MaterialProperties(0.333f, 0.8f, 0.8f));
    this.mBody.CollisionSkin = this.mCollision;
    Vector3 vector3 = this.SetMass(4f);
    Transform transform = new Transform();
    Vector3.Negate(ref vector3, out transform.Position);
    transform.Orientation = Matrix.Identity;
    this.mCollision.ApplyLocalTransform(transform);
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mBody.Tag = (object) this;
    this.mBody.Immovable = false;
    this.mBody.AllowFreezing = false;
    this.mOwner = (Character) null;
    Matrix.CreateRotationY(3.14159274f, out Matrix _);
    for (int index = 0; index < skinnedModel.SkeletonBones.Count; ++index)
    {
      SkinnedModelBone skeletonBone = skinnedModel.SkeletonBones[index];
      if (skeletonBone.Name.Equals("joint1", StringComparison.OrdinalIgnoreCase))
        this.mStartBindPose = skeletonBone.InverseBindPoseTransform;
      else if (skeletonBone.Name.Equals("joint2", StringComparison.OrdinalIgnoreCase))
        this.mEndBindPose = skeletonBone.InverseBindPoseTransform;
    }
    this.mDead = true;
    ModelMesh mesh = skinnedModel.Model.Meshes[0];
    ModelMeshPart meshPart = mesh.MeshParts[0];
    this.mSkeleton = new Matrix[skinnedModel.SkeletonBones.Count];
    this.mRenderData = new FrogTongue.RenderData[3];
    for (int index = 0; index < this.mRenderData.Length; ++index)
    {
      this.mRenderData[index] = new FrogTongue.RenderData();
      this.mRenderData[index].mSkeleton = new Matrix[skinnedModel.SkeletonBones.Count];
      this.mRenderData[index].SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, SkinnedModelBasicEffect.TYPEHASH);
    }
  }

  protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (this.Dead && iSkin1.Owner != null)
      return false;
    if (iSkin1.Owner is CharacterBody)
    {
      if (this.mAttachedCharacter != null)
        return false;
      if (iSkin1.Owner.Tag is Character && iSkin1.Owner.Tag as Character != this.mOwner)
      {
        this.mPullingBack = true;
        this.mAttachedCharacter = iSkin1.Owner.Tag as Character;
        this.mVelocity = 30f;
      }
    }
    else if (iSkin1.Tag is LevelModel)
    {
      this.mPullingBack = true;
      this.mVelocity = 80f;
      Vector3 position = this.mBody.Position;
      Vector3 forward = this.mBody.Orientation.Forward;
      EffectManager.Instance.StartEffect(FrogTongue.HIT_GROUND_SPLASH_EFFECT, ref position, ref forward, out VisualEffectReference _);
    }
    else if (iSkin1.Owner is IBoss)
      return false;
    return true;
  }

  public void Initialize(Character iOwner, Vector3 iDirection, float iMaxLengthSquared)
  {
    this.Initialize();
    this.mPullingBack = false;
    this.mOwner = iOwner;
    this.mAttachedCharacter = (Character) null;
    this.mMaxLengthSquared = iMaxLengthSquared;
    Vector3 up = Vector3.Up;
    Vector3 translation = iOwner.GetMouthAttachOrientation().Translation;
    iDirection.Y *= -0.5f;
    Matrix.CreateWorld(ref translation, ref iDirection, ref up, out this.mOrientation);
    this.mBody.MoveTo(translation, Matrix.Identity);
    Vector3.Multiply(ref iDirection, 60f, out iDirection);
    this.mBody.Velocity = iDirection;
    this.mBody.EnableBody();
    this.mAudioEmitter.Position = translation;
    this.mAudioEmitter.Forward = Vector3.Forward;
    this.mAudioEmitter.Up = Vector3.Up;
    this.mSkeleton[0] = Matrix.Identity;
    this.mSkeleton[1] = Matrix.Identity;
    this.mPlayState.EntityManager.AddEntity((Entity) this);
  }

  public float ResistanceAgainst(Elements iElement) => 0.0f;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mStartPoint = this.mOwner.GetMouthAttachOrientation().Translation;
    this.mEndPoint = this.mBody.Position;
    Matrix result1 = this.mOrientation with
    {
      Translation = this.mStartPoint
    };
    Matrix.Multiply(ref this.mStartBindPose, ref result1, out result1);
    this.mSkeleton[0] = result1;
    Matrix result2 = this.mOrientation with
    {
      Translation = this.mEndPoint
    };
    Matrix.Multiply(ref this.mEndBindPose, ref result2, out result2);
    this.mSkeleton[1] = result2;
    this.mBody.AngularVelocity = new Vector3();
    if (this.mPullingBack)
    {
      Vector3 result3;
      Vector3.Subtract(ref this.mStartPoint, ref this.mEndPoint, out result3);
      float num = result3.LengthSquared();
      if (this.mAttachedCharacter != null && (double) (System.Math.Abs(result3.X) + System.Math.Abs(result3.Z)) > 2.0)
        result3.Y = 0.0f;
      result3.Normalize();
      Vector3.Multiply(ref result3, this.mVelocity, out result3);
      if ((double) num > 1.0)
        this.mBody.Velocity = result3;
      else
        this.mDead = true;
      if (!this.mDead && this.mAttachedCharacter != null)
      {
        if (this.mAttachedCharacter.Dead || this.mAttachedCharacter.IsGripped || this.mAttachedCharacter.HasStatus(StatusEffects.Burning | StatusEffects.Greased))
        {
          this.mAttachedCharacter = (Character) null;
        }
        else
        {
          Vector3 position = this.mBody.Position;
          position.Y -= (this.mBody.CollisionSkin.GetPrimitiveLocal(0) as Sphere).Radius;
          position.Y += -this.mAttachedCharacter.HeightOffset;
          this.mAttachedCharacter.Body.MoveTo(position, this.mAttachedCharacter.Body.Orientation);
        }
      }
    }
    else
    {
      float result4;
      Vector3.DistanceSquared(ref this.mStartPoint, ref this.mEndPoint, out result4);
      if ((double) result4 >= (double) this.mMaxLengthSquared)
      {
        this.mPullingBack = true;
        this.mVelocity = this.mAttachedCharacter == null ? 80f : 30f;
      }
    }
    Array.Copy((Array) this.mSkeleton, (Array) this.mRenderData[(int) iDataChannel].mSkeleton, this.mSkeleton.Length);
    GameStateManager.Instance.CurrentState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
    base.Update(iDataChannel, iDeltaTime);
  }

  public Character AttachedCharacter => this.mAttachedCharacter;

  public bool PullingBack => this.mPullingBack;

  public Vector3 Start => this.mStartPoint;

  public Vector3 End => this.mEndPoint;

  public override void Deinitialize()
  {
    base.Deinitialize();
    this.mOwner = (Character) null;
    this.mDead = true;
  }

  public override bool Dead => this.mDead;

  public override bool Removable => this.mDead;

  public override void Kill() => this.mDead = true;

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    Transform transform = this.mBody.Transform;
    TransformRate transformRate = this.mBody.TransformRate;
    transform.ApplyTransformRate(ref transformRate, iPrediction);
    oMsg = new EntityUpdateMessage();
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = transform.Position;
    oMsg.Features |= EntityFeatures.Orientation;
    Quaternion.CreateFromRotationMatrix(ref transform.Orientation, out oMsg.Orientation);
    oMsg.Features |= EntityFeatures.Velocity;
    oMsg.Velocity = this.mBody.Velocity;
  }

  public float HitPoints => 1f;

  public float MaxHitPoints => 1f;

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
    return DamageResult.None | this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public DamageResult InternalDamage(
    Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if ((iDamage.AttackProperty & AttackProperties.Damage) == AttackProperties.Damage && (double) iDamage.Amount > 0.0)
    {
      this.mPullingBack = true;
      this.mVelocity = 80f;
      this.mAttachedCharacter = (Character) null;
    }
    return DamageResult.None;
  }

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
  }

  public void OverKill() => throw new NotImplementedException();

  protected class RenderData : IRenderableObject
  {
    protected int mEffect;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    protected VertexBuffer mVertexBuffer;
    protected int mVerticesHash;
    protected IndexBuffer mIndexBuffer;
    public Matrix[] mSkeleton;
    public SkinnedModelMaterial mMaterial;
    protected bool mMeshDirty = true;

    public bool MeshDirty => this.mMeshDirty;

    public int Effect => this.mEffect;

    public int DepthTechnique => 3;

    public int Technique
    {
      get
      {
        switch (this.mMaterial.Technique)
        {
          case SkinnedModelBasicEffect.Technique.Additive:
            return 2;
          default:
            return 0;
        }
      }
    }

    public int ShadowTechnique => 4;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public Texture2D Texture => this.mMaterial.DiffuseMap0;

    public bool Cull(BoundingFrustum iViewFrustum) => false;

    public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelBasicEffect iEffect1 = iEffect as SkinnedModelBasicEffect;
      iEffect1.Damage = 0.0f;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.OverlayColor = new Vector4();
      iEffect1.SpecularBias = 0.0f;
      iEffect1.OverlayMapEnabled = false;
      iEffect1.OverlayNormalMapEnabled = false;
      iEffect1.Bones = this.mSkeleton;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelBasicEffect iEffect1 = iEffect as SkinnedModelBasicEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.Bones = this.mSkeleton;
      iEffect1.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
      iEffect1.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      iEffect1.GraphicsDevice.Indices = this.mIndexBuffer;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      int iEffectHash)
    {
      this.mMeshDirty = false;
      lock (iMeshPart.Effect.GraphicsDevice)
        SkinnedModelMaterial.CreateFromEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mMaterial);
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mEffect = iEffectHash;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
    }
  }
}
