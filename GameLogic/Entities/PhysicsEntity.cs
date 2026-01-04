// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.PhysicsEntity
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class PhysicsEntity : Entity
{
  protected PhysicsEntity.RenderData[] mRenderData;
  protected PhysicsEntity.HighlightRenderData[] mHighlightRenderData;
  protected float mHighlighted;
  protected bool mPushable;
  protected bool mSolid;
  protected ConditionCollection mConditions;
  protected int mHitEffect;
  protected int mHitSound;
  protected int mGibTrailEffect;
  protected HitList mHitList = new HitList(32 /*0x20*/);
  protected PhysicsEntity.VisualEffectStorage[] mEffects;
  protected List<VisualEffectReference> mLiveEffects = new List<VisualEffectReference>(4);
  protected float mRestingTimer = 1f;
  protected PhysicsEntityTemplate mTemplate;
  protected bool mStatic;

  public PhysicsEntity(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mRenderData = new PhysicsEntity.RenderData[3];
    this.mHighlightRenderData = new PhysicsEntity.HighlightRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new PhysicsEntity.RenderData();
      this.mHighlightRenderData[index] = new PhysicsEntity.HighlightRenderData();
    }
  }

  protected virtual void CreateBody()
  {
    this.mBody = (Body) new PhysicsObjectBody();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Box(new Vector3(0.5f), Matrix.Identity, Vector3.One), 1, new MaterialProperties(0.0f, 1f, 1f));
    this.mBody.CollisionSkin = this.mCollision;
    Vector3 vector3 = this.SetMass(100f);
    Transform transform = new Transform();
    Vector3.Negate(ref vector3, out transform.Position);
    transform.Orientation = Matrix.Identity;
    this.mCollision.ApplyLocalTransform(transform);
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mCollision.postCollisionCallbackFn += new PostCollisionCallbackFn(this.PostCollision);
    this.mBody.Immovable = false;
    this.mBody.AllowFreezing = true;
    this.mBody.ApplyGravity = true;
    this.mBody.Tag = (object) this;
  }

  protected void PostCollision(ref CollisionInfo iInfo)
  {
    if (!this.mStatic)
      return;
    if (iInfo.SkinInfo.Skin0 == this.mCollision)
      iInfo.SkinInfo.IgnoreSkin0 = true;
    else
      iInfo.SkinInfo.IgnoreSkin1 = true;
  }

  public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner == null || !(iSkin1.Owner.Tag is Entity tag))
      return true;
    if (!this.mHitList.ContainsKey(tag.Handle))
    {
      this.mHitList.Add(tag.Handle);
      this.mConditions.ExecuteAll((Entity) this, tag, ref new EventCondition()
      {
        EventConditionType = EventConditionType.Hit | EventConditionType.Collision
      }, out DamageResult _);
    }
    return this.mSolid;
  }

  protected override void AddImpulseVelocity(ref Vector3 iVelocity)
  {
    if (this.mStatic)
      return;
    base.AddImpulseVelocity(ref iVelocity);
  }

  public virtual void Initialize(
    PhysicsEntityTemplate iTemplate,
    Matrix iStartTransform,
    int iUniqueID)
  {
    this.mTemplate = iTemplate;
    this.CreateBody();
    for (int index = 0; index < 3; ++index)
    {
      PhysicsEntity.RenderData renderData = this.mRenderData[index];
      renderData.mVertexCount = iTemplate.VertexCount;
      renderData.mVertexStride = iTemplate.VertexStride;
      renderData.mPrimitiveCount = iTemplate.PrimitiveCount;
      renderData.mVertices = iTemplate.Vertices;
      renderData.mVerticesHash = renderData.mVertices == null ? 0 : renderData.mVertices.GetHashCode();
      renderData.mVertexDeclaration = iTemplate.VertexDeclaration;
      renderData.mIndices = iTemplate.Indices;
      renderData.mEffect = RenderDeferredEffect.TYPEHASH;
      renderData.mMaterial = iTemplate.Material;
      renderData.mTechnique = iTemplate.Material.ReflectionMap == null ? (iTemplate.Material.DiffuseTexture1 == null ? RenderDeferredEffect.Technique.SingleLayer : RenderDeferredEffect.Technique.DualLayer) : (iTemplate.Material.DiffuseTexture1 == null ? RenderDeferredEffect.Technique.SingleLayerReflection : RenderDeferredEffect.Technique.DualLayerReflection);
      this.mHighlightRenderData[index].SetMesh(iTemplate.Vertices, iTemplate.Indices, iTemplate.VertexDeclaration, iTemplate.VertexCount, iTemplate.PrimitiveCount, iTemplate.VertexStride, RenderDeferredEffect.TYPEHASH, iTemplate.Material);
    }
    this.mHighlighted = -1f;
    this.mConditions = iTemplate.Conditions;
    this.mHitEffect = iTemplate.HitEffect;
    this.mHitSound = iTemplate.HitSound;
    this.mGibTrailEffect = iTemplate.GibTrailEffect;
    this.mEffects = iTemplate.Effects;
    this.mLiveEffects.Clear();
    EffectManager instance = EffectManager.Instance;
    for (int index = 0; index < this.mEffects.Length; ++index)
    {
      Matrix result;
      Matrix.Multiply(ref iStartTransform, ref this.mEffects[index].Transform, out result);
      VisualEffectReference oRef;
      instance.StartEffect(this.mEffects[index].EffectHash, ref result, out oRef);
      this.mLiveEffects.Add(oRef);
    }
    this.mPushable = iTemplate.Pushable;
    this.mBody.ApplyGravity = iTemplate.Movable;
    this.mStatic = !iTemplate.Movable;
    this.mSolid = iTemplate.Solid;
    this.CorrectWithTemplateBox();
    Vector3 translation = iStartTransform.Translation;
    iStartTransform.Translation = new Vector3();
    this.SetMass(iTemplate.Mass);
    this.mBody.MoveTo(translation, iStartTransform);
    this.mBody.EnableBody();
    this.mBody.SetInactive();
    this.Initialize(iUniqueID);
  }

  protected virtual void CorrectWithTemplateBox()
  {
    PhysicsEntityTemplate.BoxInfo box = this.mTemplate.Box;
    Matrix result;
    Matrix.CreateFromQuaternion(ref box.Orientation, out result);
    (this.mBody as PhysicsObjectBody).ReactToCharacters = this.mTemplate.Pushable;
    (this.mCollision.GetPrimitiveLocal(0) as Box).Position = box.Positon;
    (this.mCollision.GetPrimitiveNewWorld(0) as Box).Position = box.Positon;
    (this.mCollision.GetPrimitiveOldWorld(0) as Box).Position = box.Positon;
    (this.mCollision.GetPrimitiveLocal(0) as Box).Orientation = result;
    (this.mCollision.GetPrimitiveNewWorld(0) as Box).Orientation = result;
    (this.mCollision.GetPrimitiveOldWorld(0) as Box).Orientation = result;
    (this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = box.Sides;
    (this.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = box.Sides;
    (this.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = box.Sides;
    this.mRadius = box.Sides.Length() * 0.5f;
  }

  internal void OnSpawn()
  {
    if (this.mStatic)
    {
      AnimatedLevelPart oAnimatedLevelPart;
      if (this.PlayState.Level.CurrentScene.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, out oAnimatedLevelPart, new Segment()
      {
        Origin = this.mBody.Position,
        Delta = {
          Y = -4f
        }
      }) && oAnimatedLevelPart != null)
      {
        oAnimatedLevelPart.AddEntity((Entity) this);
        this.mBody.AllowFreezing = false;
      }
      else
        this.mBody.AllowFreezing = true;
    }
    else
      this.mBody.AllowFreezing = true;
  }

  public override void Deinitialize()
  {
    base.Deinitialize();
    EffectManager instance = EffectManager.Instance;
    for (int index = 0; index < this.mLiveEffects.Count; ++index)
    {
      VisualEffectReference mLiveEffect = this.mLiveEffects[index];
      instance.Stop(ref mLiveEffect);
    }
    this.mLiveEffects.Clear();
  }

  public override Matrix GetOrientation()
  {
    return this.mBody.Orientation with
    {
      Translation = this.mBody.Position
    };
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mHitList.Update(iDeltaTime);
    this.UpdateRenderData(iDataChannel);
    if ((double) this.mBody.Velocity.LengthSquared() > 9.9999999747524271E-07)
      this.mRestingTimer = 1f;
    else
      this.mRestingTimer -= iDeltaTime;
    Matrix transformMatrix = this.mBody.TransformMatrix;
    EffectManager instance = EffectManager.Instance;
    for (int index = 0; index < this.mLiveEffects.Count; ++index)
    {
      Matrix result;
      Matrix.Multiply(ref transformMatrix, ref this.mEffects[index].Transform, out result);
      VisualEffectReference oRef = this.mLiveEffects[index];
      instance.UpdateOrientation(ref oRef, ref result);
      if (oRef.ID < 0)
        instance.StartEffect(this.mEffects[index].EffectHash, ref result, out oRef);
      this.mLiveEffects[index] = oRef;
    }
    base.Update(iDataChannel, iDeltaTime);
    this.mHighlighted -= iDeltaTime;
  }

  protected virtual void UpdateRenderData(DataChannel iDataChannel)
  {
    PhysicsEntity.RenderData iObject1 = this.mRenderData[(int) iDataChannel];
    if (iObject1.mPrimitiveCount <= 0)
      return;
    iObject1.mTransform = this.GetOrientation();
    iObject1.mBoundingSphere.Center = iObject1.mTransform.Translation;
    iObject1.mBoundingSphere.Radius = this.mRadius;
    this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject1);
    if ((double) this.mHighlighted < 0.0)
      return;
    PhysicsEntity.HighlightRenderData iObject2 = this.mHighlightRenderData[(int) iDataChannel];
    iObject2.mTransform = iObject1.mTransform;
    iObject2.mBoundingSphere = iObject1.mBoundingSphere;
    this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject2);
  }

  public int HitEffect => this.mHitEffect;

  public bool Resting => (double) this.mRestingTimer < 0.0;

  public override bool Dead => this.mDead;

  public override bool Removable => this.Dead;

  public void Highlight(float iTTL) => this.mHighlighted = iTTL;

  public PhysicsObjectBody PhysicsObjectBody => this.mBody as PhysicsObjectBody;

  public override void Kill() => this.mDead = true;

  protected Vector3 GetRandomPositionOnCollisionSkin()
  {
    Vector3 position = this.Position;
    if (this.mCollision == null)
      return position;
    float num1 = (float) MagickaMath.Random.NextDouble() - 0.5f;
    float num2 = (float) MagickaMath.Random.NextDouble() - 0.5f;
    float num3 = (float) MagickaMath.Random.NextDouble() - 0.5f;
    Vector3 sideLengths = (this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths;
    position.X = (double) num1 <= 0.0 ? (float) ((double) position.X - (double) sideLengths.X * 0.25 - (double) sideLengths.X * 0.25 * (double) num1) : (float) ((double) position.X + (double) sideLengths.X * 0.25 + (double) sideLengths.X * 0.25 * (double) num1);
    position.Y = (double) num2 <= 0.0 ? (float) ((double) position.Y - (double) sideLengths.Y * 0.25 - (double) sideLengths.Y * 0.25 * (double) num2) : (float) ((double) position.Y + (double) sideLengths.Y * 0.25 + (double) sideLengths.Y * 0.25 * (double) num2);
    position.Z = (double) num3 <= 0.0 ? (float) ((double) position.Z - (double) sideLengths.Z * 0.25 - (double) sideLengths.Z * 0.25 * (double) num3) : (float) ((double) position.Z + (double) sideLengths.Z * 0.25 + (double) sideLengths.Z * 0.25 * (double) num3);
    return position;
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    if (!(!this.Resting & !this.mBody.Immovable))
      return;
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

  public struct State
  {
    private PhysicsEntity mEntity;
    private string mTemplate;
    private Vector3 mPosition;
    private Quaternion mOrientation;
    private int mUniqueID;
    private int mOnDeath;
    private int mOnDamage;

    public State(BinaryReader iReader)
    {
      this.mEntity = (PhysicsEntity) null;
      this.mTemplate = iReader.ReadString();
      this.mPosition.X = iReader.ReadSingle();
      this.mPosition.Y = iReader.ReadSingle();
      this.mPosition.Z = iReader.ReadSingle();
      this.mOrientation.X = iReader.ReadSingle();
      this.mOrientation.Y = iReader.ReadSingle();
      this.mOrientation.Z = iReader.ReadSingle();
      this.mOrientation.W = iReader.ReadSingle();
      this.mUniqueID = iReader.ReadInt32();
      this.mOnDamage = iReader.ReadInt32();
      this.mOnDeath = iReader.ReadInt32();
    }

    public State(PhysicsEntity iPhysicsEntity)
    {
      this.mEntity = iPhysicsEntity;
      this.mTemplate = iPhysicsEntity.mTemplate.Path;
      iPhysicsEntity.Body.TransformMatrix.Decompose(out Vector3 _, out this.mOrientation, out this.mPosition);
      this.mUniqueID = iPhysicsEntity.UniqueID;
      if (iPhysicsEntity is DamageablePhysicsEntity damageablePhysicsEntity)
      {
        this.mOnDamage = damageablePhysicsEntity.OnDamage;
        this.mOnDeath = damageablePhysicsEntity.OnDeath;
      }
      else
      {
        this.mOnDamage = 0;
        this.mOnDeath = 0;
      }
    }

    public PhysicsEntity ApplyTo(PlayState iPlayState)
    {
      PhysicsEntityTemplate iTemplate = iPlayState.Content.Load<PhysicsEntityTemplate>(this.mTemplate);
      if (this.mEntity == null)
      {
        if ((double) iTemplate.MaxHitpoints > 0.0)
          this.mEntity = (PhysicsEntity) new DamageablePhysicsEntity(iPlayState)
          {
            OnDamage = this.mOnDamage,
            OnDeath = this.mOnDeath
          };
        else
          this.mEntity = new PhysicsEntity(iPlayState);
      }
      Matrix result;
      Matrix.CreateFromQuaternion(ref this.mOrientation, out result);
      result.Translation = this.mPosition;
      this.mEntity.Initialize(iTemplate, result, this.mUniqueID);
      return this.mEntity;
    }

    public void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.mTemplate);
      iWriter.Write(this.mPosition.X);
      iWriter.Write(this.mPosition.Y);
      iWriter.Write(this.mPosition.Z);
      iWriter.Write(this.mOrientation.X);
      iWriter.Write(this.mOrientation.Y);
      iWriter.Write(this.mOrientation.Z);
      iWriter.Write(this.mOrientation.W);
      iWriter.Write(this.mUniqueID);
      iWriter.Write(this.mOnDamage);
      iWriter.Write(this.mOnDeath);
    }
  }

  public struct VisualEffectStorage
  {
    public Matrix Transform;
    public int EffectHash;
  }

  protected class RenderData : IRenderableObject
  {
    public RenderDeferredMaterial mMaterial;
    public int mEffect;
    public BoundingSphere mBoundingSphere;
    public Matrix mTransform;
    public int mVertexCount;
    public int mVertexStride;
    public int mPrimitiveCount;
    public VertexBuffer mVertices;
    public VertexDeclaration mVertexDeclaration;
    public IndexBuffer mIndices;
    public RenderDeferredEffect.Technique mTechnique;
    public int mVerticesHash;

    public int Effect => this.mEffect;

    public int DepthTechnique => 4;

    public int Technique => (int) this.mTechnique;

    public int ShadowTechnique => 5;

    public VertexBuffer Vertices => this.mVertices;

    public IndexBuffer Indices => this.mIndices;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public Texture2D Texture => this.mMaterial.DiffuseTexture0;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.World = this.mTransform;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mVertexCount, 0, this.mPrimitiveCount);
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignOpacityToEffect(iEffect1);
      iEffect1.World = this.mTransform;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mVertexCount, 0, this.mPrimitiveCount);
    }
  }

  protected class HighlightRenderData : IRenderableAdditiveObject
  {
    protected VertexDeclaration mVertexDeclaration;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mVertexStride;
    protected VertexBuffer mVertexBuffer;
    protected int mVerticesHash;
    protected IndexBuffer mIndexBuffer;
    protected int mEffect;
    protected RenderDeferredMaterial mMaterial;
    public BoundingSphere mBoundingSphere;
    public Matrix mTransform;
    protected bool mMeshDirty = true;

    public int Effect => this.mEffect;

    public int Technique => 6;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public int VerticesHashCode => this.mVerticesHash;

    public int VertexStride => this.mVertexStride;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.DiffuseColor0 = new Vector3(1f);
      iEffect1.FresnelPower = 2f;
      iEffect1.World = this.mTransform;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
      iEffect1.DiffuseColor0 = Vector3.One;
    }

    public bool MeshDirty => this.mMeshDirty;

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      VertexDeclaration iVertexDeclaration,
      int iNumVertices,
      int iPrimitiveCount,
      int iVertexStride,
      int iEffectHash,
      RenderDeferredMaterial iMaterial)
    {
      this.mMeshDirty = false;
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices == null ? 0 : iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mEffect = iEffectHash;
      this.mVertexDeclaration = iVertexDeclaration;
      this.mNumVertices = iNumVertices;
      this.mPrimitiveCount = iPrimitiveCount;
      this.mVertexStride = iVertexStride;
      this.mMaterial = iMaterial;
    }
  }
}
