// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimatedPhysicsEntity
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Levels;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal class AnimatedPhysicsEntity : DamageablePhysicsEntity
{
  private static readonly Vector3 WetColor = new Vector3(0.75f, 0.8f, 1f);
  private static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);
  protected static Matrix sRotateX90 = Matrix.CreateRotationX(-1.57079637f);
  private SkinnedModel mModel;
  private AnimationController mAnimationController;
  private AnimationClipAction[][] mAnimationClips;
  private AnimationAction[] mCurrentActions;
  private WeaponClass mCurrentAnimationSet;
  private Animations mCurrentAnimation;
  private Matrix mStaticTransform;
  private SkinnedModelDeferredBasicMaterial mMaterial;
  private AnimatedPhysicsEntity.AnimatedRenderData[] mAnimatedRenderData;
  protected static TextureCube sIceCubeMap;
  protected static TextureCube sIceCubeNormalMap;
  private int mModelID;
  private float mDefaultSpecular = 0.5f;
  private float mLastDraw;
  private float mBoundingScale = 1.333f;
  private bool mIsDamageable;
  private bool mIsAffectedByGravity = true;
  private bool mForceAnimationUpdate;

  public bool IsDamageable
  {
    get => this.mIsDamageable;
    set => this.mIsDamageable = value;
  }

  public bool IsAffectedByGravity
  {
    get => this.mIsAffectedByGravity;
    set
    {
      this.mIsAffectedByGravity = value;
      if (this.mBody == null)
        return;
      this.mBody.ApplyGravity = this.mIsAffectedByGravity;
    }
  }

  public AnimatedPhysicsEntity(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mAnimationController = new AnimationController();
    this.mAnimationController.AnimationLooped += new AnimationEvent(this.OnAnimationLooped);
    this.mAnimationController.CrossfadeFinished += new AnimationEvent(this.OnCrossfadeFinished);
    this.mAnimatedRenderData = new AnimatedPhysicsEntity.AnimatedRenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mAnimatedRenderData[index] = new AnimatedPhysicsEntity.AnimatedRenderData(this);
    if (AnimatedPhysicsEntity.sIceCubeMap == null)
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
        AnimatedPhysicsEntity.sIceCubeMap = Magicka.Game.Instance.Content.Load<TextureCube>("EffectTextures/iceCube");
    }
    if (AnimatedPhysicsEntity.sIceCubeNormalMap != null)
      return;
    lock (Magicka.Game.Instance.GraphicsDevice)
      AnimatedPhysicsEntity.sIceCubeNormalMap = Magicka.Game.Instance.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
  }

  protected override void CreateBody()
  {
    this.mBody = (Body) new PhysicsObjectBody();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Box(new Vector3(-this.mRadius, 0.0f, -this.mRadius), Matrix.Identity, Vector3.One * (this.mRadius * 2f)), 1, new MaterialProperties(0.0f, 1f, 1f));
    this.mCollision.callbackFn -= new CollisionCallbackFn(((PhysicsEntity) this).OnCollision);
    this.mCollision.callbackFn += new CollisionCallbackFn(((PhysicsEntity) this).OnCollision);
    this.mCollision.postCollisionCallbackFn -= new PostCollisionCallbackFn(((PhysicsEntity) this).PostCollision);
    this.mCollision.postCollisionCallbackFn += new PostCollisionCallbackFn(((PhysicsEntity) this).PostCollision);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.Immovable = !this.mPushable;
    this.mBody.AllowFreezing = true;
    this.mBody.ApplyGravity = this.mIsAffectedByGravity;
    this.mBody.Tag = (object) this;
  }

  protected override void CorrectWithTemplateBox()
  {
  }

  public override void Initialize(
    PhysicsEntityTemplate iTemplate,
    Matrix iStartTransform,
    int iUniqueID)
  {
    this.mRadius = iTemplate.mRadius;
    if ((double) this.mRadius <= 0.0)
      this.mRadius = 1f;
    base.Initialize(iTemplate, iStartTransform, iUniqueID);
    this.mLastDraw = 0.0f;
    this.mStatic = false;
    if ((double) this.mBody.Mass > 0.0 && this.mIsAffectedByGravity)
    {
      Vector3 vector3 = this.mBody.Position;
      if (this.mPlayState.Level != null && this.mPlayState.Level.CurrentScene != null)
      {
        Segment iSeg = new Segment();
        iSeg.Origin = vector3;
        ++iSeg.Origin.Y;
        iSeg.Delta.Y = -10f;
        Vector3 oPos;
        if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, out AnimatedLevelPart _, iSeg))
        {
          vector3 = oPos;
          vector3.Y += 0.1f;
        }
      }
      else
        vector3.Y += this.mRadius * 2f;
      this.mBody.Position = vector3;
    }
    this.mAnimationClips = iTemplate.AnimationClips;
    if (iTemplate.Models != null)
    {
      this.mModelID = MagickaMath.Random.Next(0, iTemplate.Models.Length);
      this.mModel = iTemplate.Models[this.mModelID].Model;
      this.mStaticTransform = iTemplate.Models[this.mModelID].Transform;
      Matrix.Invert(ref this.mStaticTransform, out Matrix _);
      Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
      this.mMaterial.TintColor = iTemplate.Models[this.mModelID].Tint;
    }
    this.mAnimationController.Skeleton = iTemplate.Skeleton;
    this.mCurrentAnimation = Animations.idle;
    this.mCurrentAnimationSet = WeaponClass.Default;
    this.mAnimationController.ClearCrossfadeQueue();
    if (this.mModel != null)
    {
      this.mDefaultSpecular = (this.mModel.Model.Meshes[0].Effects[0] as SkinnedModelBasicEffect).SpecularPower;
      if ((double) this.mDefaultSpecular < 0.10000000149011612)
        this.mDefaultSpecular = 0.1f;
    }
    for (int index = 0; index < 3; ++index)
    {
      AnimatedPhysicsEntity.AnimatedRenderData animatedRenderData = this.mAnimatedRenderData[index];
      animatedRenderData.mNumVertices = iTemplate.VertexCount;
      animatedRenderData.mVertexStride = iTemplate.VertexStride;
      animatedRenderData.mPrimitiveCount = iTemplate.PrimitiveCount;
      animatedRenderData.mVertexBuffer = iTemplate.Vertices;
      animatedRenderData.mVerticesHash = animatedRenderData.mVertexBuffer == null ? 0 : animatedRenderData.mVertexBuffer.GetHashCode();
      animatedRenderData.mVertexDeclaration = iTemplate.SkeletonVertexDeclaration;
      animatedRenderData.mIndexBuffer = iTemplate.Indices;
      animatedRenderData.SetMeshDirty();
    }
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if (this.Dead || this.mAnimationController == null)
      return;
    this.mLastDraw += iDeltaTime;
    if (!(!this.mAnimationController.HasFinished | this.mAnimationController.CrossFadeEnabled))
      return;
    Matrix orientation = this.GetOrientation();
    bool UpdateBoneTransforms = (double) this.mLastDraw < 0.20000000298023224 || this.mForceAnimationUpdate;
    this.mAnimationController.Update(iDeltaTime, ref orientation, UpdateBoneTransforms);
  }

  internal void SetStaticTransform(float iX, float iY, float iZ)
  {
    Matrix result;
    Matrix.CreateScale(iX, iY, iZ, out result);
    Matrix.Multiply(ref result, ref this.mTemplate.Models[this.mModelID].Transform, out this.mStaticTransform);
  }

  internal void ScaleGraphicModel(float iScale)
  {
    this.mStaticTransform.M11 = iScale;
    this.mStaticTransform.M22 = iScale;
    this.mStaticTransform.M33 = iScale;
  }

  protected override void UpdateRenderData(DataChannel iDataChannel)
  {
    if (this.mModel == null || this.mModel.SkeletonBones == null || this.mModel.SkeletonBones.Count == 0)
    {
      base.UpdateRenderData(iDataChannel);
    }
    else
    {
      AnimatedPhysicsEntity.AnimatedRenderData iObject = this.mAnimatedRenderData[(int) iDataChannel];
      int count = this.mModel.SkeletonBones.Count;
      Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, 0, (Array) iObject.mSkeleton, 0, count);
      BoundingSphere boundingSphere = this.mModel.Model.Meshes[0].BoundingSphere with
      {
        Center = this.Position
      };
      boundingSphere.Radius *= this.mBoundingScale * this.mStaticTransform.M11;
      iObject.mBoundingSphere = boundingSphere;
      iObject.mMaterial.CubeNormalMapEnabled = false;
      float num = Math.Min(this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude * 10f, 1f);
      iObject.mMaterial.Colorize.X = AnimatedPhysicsEntity.ColdColor.X;
      iObject.mMaterial.Colorize.Y = AnimatedPhysicsEntity.ColdColor.Y;
      iObject.mMaterial.Colorize.Z = AnimatedPhysicsEntity.ColdColor.Z;
      iObject.mMaterial.Colorize.W = num;
      iObject.mMaterial.Bloat = 0.0f;
      Vector3.Multiply(ref iObject.mMaterial.DiffuseColor, 1f + num, out iObject.mMaterial.DiffuseColor);
      if (this.HasStatus(StatusEffects.Frozen))
      {
        iObject.mMaterial.CubeMapRotation = Matrix.Identity;
        iObject.mMaterial.Bloat = (float) (int) ((double) this.StatusMagnitude(StatusEffects.Frozen) * 10.0) * 0.02f;
        iObject.mMaterial.EmissiveAmount = 0.5f;
        iObject.mMaterial.SpecularBias = 1f;
        iObject.mMaterial.SpecularPower = 10f;
        iObject.mMaterial.CubeMapEnabled = true;
        iObject.mMaterial.CubeNormalMapEnabled = true;
        iObject.mMaterial.CubeMap = AnimatedPhysicsEntity.sIceCubeMap;
        iObject.mMaterial.CubeNormalMap = AnimatedPhysicsEntity.sIceCubeNormalMap;
        iObject.mMaterial.CubeMapColor.X = iObject.mMaterial.CubeMapColor.Y = iObject.mMaterial.CubeMapColor.Z = 1f;
        iObject.mMaterial.CubeMapColor.W = (double) this.StatusMagnitude(StatusEffects.Frozen) > 0.0 ? 1f : 0.0f;
      }
      else if (this.HasStatus(StatusEffects.Wet))
      {
        iObject.mMaterial.ProjectionMap = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/wetMap");
        iObject.mMaterial.ProjectionMapEnabled = true;
        iObject.mMaterial.CubeMapEnabled = false;
        Vector3 axis = this.Position + Vector3.Up + Vector3.Forward - this.Position;
        Vector3 result1 = this.Direction;
        float angle = 0.2f;
        Quaternion result2;
        Quaternion.CreateFromAxisAngle(ref axis, angle, out result2);
        Vector3.Transform(ref result1, ref result2, out result1);
        float scale = 1f;
        iObject.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Up + new Vector3(0.0f, 0.0f, -1f), this.Position, result1) * Matrix.CreateScale(scale);
        iObject.mMaterial.SpecularBias = 0.5f;
        iObject.mMaterial.SpecularPower = this.mMaterial.SpecularPower * 2f;
        Vector3 wetColor = AnimatedPhysicsEntity.WetColor;
        Vector3.Multiply(ref iObject.mMaterial.DiffuseColor, ref wetColor, out iObject.mMaterial.DiffuseColor);
      }
      else if (this.HasStatus(StatusEffects.Burning))
      {
        iObject.mMaterial.ProjectionMap = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/burnMap");
        iObject.mMaterial.ProjectionMapEnabled = true;
        iObject.mMaterial.CubeMapEnabled = false;
        iObject.mMaterial.ProjectionMapColor = new Vector4(1f);
        iObject.mMaterial.ProjectionMapColor.W = this.StatusMagnitude(StatusEffects.Burning);
        iObject.mMaterial.ProjectionMapAdditive = true;
        Vector3 axis = this.Position + Vector3.Up - this.Position;
        Vector3 result3 = this.Direction;
        Quaternion result4;
        Quaternion.CreateFromAxisAngle(ref axis, 0.0f, out result4);
        Vector3.Transform(ref result3, ref result4, out result3);
        float scale = 1f;
        iObject.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Up, this.Position, result3) * Matrix.CreateScale(scale);
        iObject.mMaterial.EmissiveAmount = this.StatusMagnitude(StatusEffects.Burning);
        iObject.mMaterial.SpecularBias = 0.0f;
        iObject.mMaterial.SpecularPower = this.mMaterial.SpecularPower;
      }
      else if (this.HasStatus(StatusEffects.Greased))
      {
        iObject.mMaterial.ProjectionMap = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/Greased");
        iObject.mMaterial.ProjectionMapEnabled = true;
        iObject.mMaterial.CubeMapEnabled = false;
        Vector3 axis = this.Position + Vector3.Down - this.Position;
        Vector3 result5 = this.Direction;
        Quaternion result6;
        Quaternion.CreateFromAxisAngle(ref axis, 1f, out result6);
        Vector3.Transform(ref result5, ref result6, out result5);
        float scale = 1f;
        iObject.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Down, this.Position, result5) * Matrix.CreateScale(scale);
        iObject.mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
        iObject.mMaterial.SpecularBias = 0.5f;
        iObject.mMaterial.SpecularPower = this.mMaterial.SpecularPower * 2f;
      }
      else
      {
        iObject.mMaterial.SpecularPower = this.mMaterial.SpecularPower;
        iObject.mMaterial.SpecularAmount = this.mMaterial.SpecularAmount;
        iObject.mMaterial.SpecularBias = 0.0f;
        iObject.mMaterial.ProjectionMapEnabled = false;
        iObject.mMaterial.CubeMapEnabled = false;
        iObject.mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
      }
      iObject.mMaterial.Damage = (float) (1.0 - (double) this.mHitPoints / (double) this.mMaxHitPoints);
      ModelMesh mesh = this.mModel.Model.Meshes[0];
      ModelMeshPart meshPart = mesh.MeshParts[0];
      if (iObject.MeshDirty)
      {
        SkinnedModelDeferredEffect.Technique activeTechnique = (SkinnedModelDeferredEffect.Technique) (meshPart.Effect as SkinnedModelBasicEffect).ActiveTechnique;
        iObject.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, ref this.mMaterial, activeTechnique);
      }
      iObject.mTechnique = SkinnedModelDeferredEffect.Technique.Default;
      this.mPlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
    }
  }

  public void GoToAnimation(Animations iAnimation, float iTime)
  {
    if (!(this.mCurrentAnimation != iAnimation | this.mAnimationController.HasFinished))
      return;
    this.mCurrentAnimation = iAnimation;
    this.mCurrentAnimationSet = WeaponClass.Default;
    AnimationClipAction animationClipAction = this.mAnimationClips[(int) this.mCurrentAnimationSet][(int) iAnimation];
    if (animationClipAction == null)
    {
      this.mCurrentAnimation = Animations.idle;
      this.mCurrentAnimationSet = WeaponClass.Default;
    }
    else
    {
      this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
      float blendTime = animationClipAction.BlendTime;
      this.mAnimationController.CrossFade(animationClipAction.Clip, (double) blendTime > 0.0 ? blendTime : iTime, animationClipAction.LoopAnimation);
    }
  }

  public void ForceAnimation(Animations iAnimation)
  {
    this.mAnimationController.ClearCrossfadeQueue();
    this.mCurrentAnimation = iAnimation;
    this.mCurrentAnimationSet = WeaponClass.Default;
    AnimationClipAction animationClipAction = this.mAnimationClips[(int) this.mCurrentAnimationSet][(int) iAnimation];
    if (animationClipAction == null)
    {
      this.mCurrentAnimation = Animations.idle;
      this.mCurrentAnimationSet = WeaponClass.Default;
    }
    else
    {
      this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
      this.mAnimationController.StartClip(animationClipAction.Clip, animationClipAction.LoopAnimation);
      this.OnCrossfadeFinished();
    }
  }

  protected void OnAnimationLooped()
  {
    int num = 0;
    while (num < this.mCurrentActions.Length)
      ++num;
  }

  protected void OnCrossfadeFinished()
  {
    this.mCurrentActions = this.mAnimationClips[(int) this.mCurrentAnimationSet][(int) this.mCurrentAnimation].Actions;
    this.mForceAnimationUpdate = false;
    for (int index = 0; index < this.mCurrentActions.Length; ++index)
    {
      if (this.mCurrentActions[index].UsesBones)
        this.mForceAnimationUpdate = true;
    }
  }

  public override DamageResult InternalDamage(
    DamageCollection5 iDamages,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return !this.mIsDamageable ? DamageResult.Deflected : base.InternalDamage(iDamages, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public override DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    bool flag = (((iDamage.AttackProperty & AttackProperties.Pushed) == AttackProperties.Pushed ? 1 : 0) & ((iDamage.Element & Elements.Earth) == Elements.Earth ? 1 : (true ? 1 : 0))) != 0 & this.mPushable;
    return !this.mIsDamageable && !flag ? DamageResult.Deflected : base.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public override DamageResult AddStatusEffect(StatusEffect iStatusEffect)
  {
    return !this.mIsDamageable ? DamageResult.Deflected : base.AddStatusEffect(iStatusEffect);
  }

  private class AnimatedRenderData : IRenderableObject, IRenderableAdditiveObject
  {
    public BoundingSphere mBoundingSphere;
    public VertexDeclaration mVertexDeclaration;
    public int mBaseVertex;
    public int mNumVertices;
    public int mPrimitiveCount;
    public int mStartIndex;
    public int mStreamOffset;
    public int mVertexStride;
    public VertexBuffer mVertexBuffer;
    public IndexBuffer mIndexBuffer;
    public Matrix[] mSkeleton;
    public SkinnedModelDeferredAdvancedMaterial mMaterial;
    public SkinnedModelDeferredEffect.Technique mTechnique;
    public int mVerticesHash;
    public bool mMeshDirty = true;
    private AnimatedPhysicsEntity mOwner;

    public int PrimitiveCount
    {
      get => this.mPrimitiveCount;
      private set => this.mPrimitiveCount = value;
    }

    public AnimatedRenderData(AnimatedPhysicsEntity owner)
    {
      this.mOwner = owner;
      this.mSkeleton = new Matrix[80 /*0x50*/];
    }

    public bool MeshDirty => this.mMeshDirty;

    public int Effect => SkinnedModelDeferredEffect.TYPEHASH;

    public int DepthTechnique => 3;

    public int Technique => (int) this.mTechnique;

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
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.Bones = this.mSkeleton;
      iEffect1.GraphicsDevice.RenderState.ReferenceStencil = 2;
      iEffect1.DiffuseColor = Color.White.ToVector3();
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      iEffect1.Bloat = 0.0f;
      iEffect1.GraphicsDevice.RenderState.ReferenceStencil = 0;
      iEffect1.CubeMapEnabled = false;
      this.mOwner.mLastDraw = 0.0f;
      iEffect1.Colorize = new Vector4();
    }

    public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      if (this.mTechnique == SkinnedModelDeferredEffect.Technique.Additive)
        return;
      SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
      this.mMaterial.AssignOpacityToEffect(iEffect1);
      iEffect1.Bones = this.mSkeleton;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      iEffect1.Bloat = 0.0f;
    }

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      ref SkinnedModelDeferredBasicMaterial iBasicMaterial,
      SkinnedModelDeferredEffect.Technique iTechnique)
    {
      this.mMeshDirty = false;
      this.mMaterial.CopyFrom(ref iBasicMaterial);
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
      this.mTechnique = iTechnique;
    }
  }
}
