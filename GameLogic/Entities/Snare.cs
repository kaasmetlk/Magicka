// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Snare
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal class Snare : Entity
{
  private static List<Snare> sCache;
  private Snare.RenderData[] mRenderData;
  private SkinnedModel mModel;
  private AnimationClip mClip;
  private AnimationController mController;
  private int mStartJoint;
  private Matrix mStartBindPose;
  private int mEndJoint;
  private Matrix mEndBindPose;
  private Entity mSnared;
  private Character mOwner;

  public static Snare GetFromCache(PlayState iPlayState)
  {
    if (Snare.sCache.Count <= 0)
      return new Snare(iPlayState);
    Snare fromCache = Snare.sCache[Snare.sCache.Count - 1];
    Snare.sCache.RemoveAt(Snare.sCache.Count - 1);
    return fromCache;
  }

  public static void ReturnToCache(Snare iEnsnare) => Snare.sCache.Add(iEnsnare);

  public static void InitializeCache(int iNrOfEnsnares, PlayState iPlayState)
  {
    Snare.sCache = new List<Snare>(iNrOfEnsnares);
    for (int index = 0; index < iNrOfEnsnares; ++index)
      Snare.sCache.Add(new Snare(iPlayState));
  }

  public Snare(PlayState iPlaystate)
    : base(iPlaystate)
  {
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/Frog_Snare");
    this.mClip = this.mModel.AnimationClips[nameof (Snare)];
    this.mController = new AnimationController();
    this.mController.Skeleton = this.mModel.SkeletonBones;
    Matrix result;
    Matrix.CreateRotationY(3.14159274f, out result);
    foreach (SkinnedModelBone skeletonBone in (ReadOnlyCollection<SkinnedModelBone>) this.mModel.SkeletonBones)
    {
      if (skeletonBone.Index == (ushort) 1)
      {
        this.mStartJoint = (int) skeletonBone.Index;
        this.mStartBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mStartBindPose, ref result, out this.mStartBindPose);
        Matrix.Invert(ref this.mStartBindPose, out this.mStartBindPose);
      }
      else if (skeletonBone.Index == (ushort) 2)
      {
        this.mEndJoint = (int) skeletonBone.Index;
        this.mEndBindPose = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref this.mEndBindPose, ref result, out this.mEndBindPose);
        Matrix.Invert(ref this.mEndBindPose, out this.mEndBindPose);
      }
    }
    this.mBody = new Body();
    this.mCollision = new CollisionSkin(this.mBody);
    this.mCollision.AddPrimitive((Primitive) new Capsule(Vector3.Zero, Matrix.Identity, 0.2f, 2f), 1, new MaterialProperties(0.2f, 0.8f, 0.8f));
    this.mCollision.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mBody.CollisionSkin = this.mCollision;
    this.mBody.Immovable = true;
    this.mBody.Tag = (object) this;
    this.mRenderData = new Snare.RenderData[3];
    this.mRenderData[0] = new Snare.RenderData();
    this.mRenderData[1] = new Snare.RenderData();
    this.mRenderData[2] = new Snare.RenderData();
  }

  public void Initialize(Character iOwner)
  {
    this.mSnared = (Entity) null;
    this.mOwner = iOwner;
    this.mController.PlaybackMode = PlaybackMode.Forward;
    this.mController.StartClip(this.mClip, false);
  }

  public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    if (iSkin1.Owner != null && iSkin1.Owner.Tag != this.mOwner)
    {
      Entity tag = iSkin1.Owner.Tag as Entity;
      if (this.mSnared != null)
      {
        this.mSnared = tag;
        this.mController.PlaybackMode = PlaybackMode.Backward;
        return true;
      }
    }
    return false;
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    Matrix attachOrientation = this.mOwner.GetMouthAttachOrientation();
    Vector3 position = this.mOwner.Position;
    this.mBody.MoveTo(position, attachOrientation);
    Matrix parent = attachOrientation with
    {
      Translation = position
    };
    this.mController.PlaybackMode = PlaybackMode.Forward;
    this.mController.Update(iDeltaTime, ref parent, true);
    Matrix result1;
    Matrix.Multiply(ref this.mStartBindPose, ref this.mController.SkinnedBoneTransforms[this.mStartJoint], out result1);
    Matrix result2;
    Matrix.Multiply(ref this.mEndBindPose, ref this.mController.SkinnedBoneTransforms[this.mEndJoint], out result2);
    Array.Copy((Array) this.mController.SkinnedBoneTransforms, 0, (Array) this.mRenderData[(int) iDataChannel].mSkeleton, 0, 2);
    this.mOwner.PlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) this.mRenderData[(int) iDataChannel]);
    Vector3 translation1 = result1.Translation;
    Vector3 translation2 = result2.Translation;
    float result3;
    Vector3.Distance(ref translation2, ref translation1, out result3);
    if ((double) result3 < 1.4012984643248171E-45)
    {
      this.mDead = true;
      (this.mBody.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Orientation = attachOrientation;
      (this.mBody.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length = result3;
    }
    if (this.mSnared == null)
      return;
    Matrix orientation = this.mSnared.Body.Orientation;
    this.mSnared.Body.MoveTo(translation2, orientation);
  }

  public override bool Dead => this.mDead;

  public override bool Removable => this.mDead;

  public override void Kill()
  {
    this.mDead = true;
    this.mSnared = (Entity) null;
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = this.Position;
  }

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
    protected IndexBuffer mIndexBuffer;
    public Matrix[] mSkeleton;
    private SkinnedModelMaterial mSkinnedModelMaterial;
    public int mVerticesHash;

    public RenderData() => this.mSkeleton = new Matrix[80 /*0x50*/];

    public int Effect => this.mEffect;

    public int DepthTechnique => 3;

    public int Technique => 0;

    public int ShadowTechnique => 4;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public Texture2D Texture => this.mSkinnedModelMaterial.DiffuseMap0;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return new BoundingSphere().Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelBasicEffect iEffect1 = iEffect as SkinnedModelBasicEffect;
      this.mSkinnedModelMaterial.AssignToEffect(iEffect1);
      iEffect1.Bones = this.mSkeleton;
      iEffect1.OverlayColor = new Vector4();
      iEffect1.SpecularBias = 0.0f;
      iEffect1.OverlayMapEnabled = false;
      iEffect1.OverlayNormalMapEnabled = false;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      SkinnedModelBasicEffect iEffect1 = iEffect as SkinnedModelBasicEffect;
      this.mSkinnedModelMaterial.AssignToEffect(iEffect1);
      iEffect1.Bones = this.mSkeleton;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
    }

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      int iEffectHash)
    {
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
      SkinnedModelMaterial.CreateFromEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mSkinnedModelMaterial);
    }
  }
}
