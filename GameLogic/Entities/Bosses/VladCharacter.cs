// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.VladCharacter
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

internal class VladCharacter : NonPlayerCharacter
{
  private float mAfterImageTimer;
  private float mAfterImageIntensity;
  private VladCharacter.AfterImageRenderData[] mAfterImageRenderData;

  public VladCharacter(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mAfterImageRenderData = new VladCharacter.AfterImageRenderData[3];
    Matrix[][] iSkeleton = new Matrix[5][];
    for (int index = 0; index < iSkeleton.Length; ++index)
      iSkeleton[index] = new Matrix[80 /*0x50*/];
    for (int index = 0; index < 3; ++index)
      this.mAfterImageRenderData[index] = new VladCharacter.AfterImageRenderData(iSkeleton);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    this.CharacterBody.SpeedMultiplier *= 1.5f;
    if (!((double) this.CharacterBody.SpeedMultiplier > 1.0 | (double) this.mAfterImageIntensity > -1.0))
      return;
    VladCharacter.AfterImageRenderData iObject = this.mAfterImageRenderData[(int) iDataChannel];
    if (iObject.MeshDirty)
    {
      ModelMesh mesh = this.mModel.Model.Meshes[0];
      ModelMeshPart meshPart = mesh.MeshParts[0];
      iObject.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, SkinnedModelBasicEffect.TYPEHASH);
      iObject.Color = this.mMaterial.TintColor;
    }
    int count = this.mModel.SkeletonBones.Count;
    this.mAfterImageTimer -= iDeltaTime;
    this.mAfterImageIntensity -= iDeltaTime;
    while ((double) this.mAfterImageTimer <= 0.0)
    {
      this.mAfterImageTimer += 0.05f;
      if ((double) (this.mBody.Velocity with { Y = 0.0f }).LengthSquared() > 1.0 / 1000.0 & (double) this.CharacterBody.SpeedMultiplier > 1.0)
      {
        while ((double) this.mAfterImageIntensity <= 0.0)
          this.mAfterImageIntensity += 0.05f;
      }
      for (int index = iObject.mSkeleton.Length - 1; index > 0; --index)
        Array.Copy((Array) iObject.mSkeleton[index - 1], (Array) iObject.mSkeleton[index], count);
      Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, (Array) iObject.mSkeleton[0], count);
    }
    iObject.mIntensity = this.mAfterImageIntensity / 0.05f;
    iObject.mBoundingSphere = this.mRenderData[(int) iDataChannel].mBoundingSphere;
    this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
  }

  public override void OverKill()
  {
    this.mDead = false;
    this.mOverkilled = true;
    this.mCannotDieWithoutExplicitKill = true;
    this.mAnimationController.Stop();
    this.mHitPoints = -1f;
  }

  protected class AfterImageRenderData : IRenderableAdditiveObject
  {
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

    public int Technique => 1;

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
      if (iEffect.GraphicsDevice.RenderState.ColorWriteChannels == ColorWriteChannels.Alpha)
      {
        iEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
        iEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      }
      else
      {
        SkinnedModelBasicEffect iEffect1 = iEffect as SkinnedModelBasicEffect;
        iEffect1.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
        iEffect1.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
        this.mMaterial.AssignToEffect(iEffect1);
        iEffect1.Colorize = new Vector4(Magicka.GameLogic.Entities.Character.ColdColor, 1f);
        float num1 = 0.333f;
        float num2 = (float) (0.33300000429153442 / ((double) this.mSkeleton.Length + 1.0));
        float num3 = num1 + this.mIntensity * num2;
        for (int index = 0; index < this.mSkeleton.Length && (double) num3 > 0.0; ++index)
        {
          if (index != 0)
          {
            iEffect1.Alpha = num3;
            iEffect1.Bones = this.mSkeleton[index];
            iEffect1.CommitChanges();
            iEffect1.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
            num3 -= num2;
          }
        }
        iEffect1.Colorize = new Vector4();
      }
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
}
