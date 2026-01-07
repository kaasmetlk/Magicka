// Decompiled with JetBrains decompiler
// Type: PolygonHead.RenderableAdditiveObject`2
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace PolygonHead;

public class RenderableAdditiveObject<T, M> : IRenderableAdditiveObject
  where T : Microsoft.Xna.Framework.Graphics.Effect
  where M : IMaterial<T>
{
  private int mEffect = typeof (T).GetHashCode();
  public M mMaterial;
  public BoundingSphere mBoundingSphere;
  protected VertexBuffer mVertexBuffer;
  protected int mVertexBufferHash;
  protected IndexBuffer mIndexBuffer;
  protected VertexDeclaration mVertexDeclaration;
  protected int mBaseVertex;
  protected int mNumVertices;
  protected int mStartIndex;
  protected int mPrimitiveCount;
  protected int mVertexStride;
  protected int mTechnique;

  public virtual void SetMesh(
    VertexBuffer iVertices,
    IndexBuffer iIndices,
    ModelMeshPart iMeshPart,
    int iTechnique)
  {
    this.mVertexBuffer = iVertices;
    this.mVertexBufferHash = iVertices.GetHashCode();
    this.mIndexBuffer = iIndices;
    this.mVertexDeclaration = iMeshPart.VertexDeclaration;
    this.mBaseVertex = iMeshPart.BaseVertex;
    this.mNumVertices = iMeshPart.NumVertices;
    this.mPrimitiveCount = iMeshPart.PrimitiveCount;
    this.mStartIndex = iMeshPart.StartIndex;
    this.mVertexStride = iMeshPart.VertexStride;
    this.mTechnique = iTechnique;
  }

  public virtual void SetMesh(ModelMesh iMesh, ModelMeshPart iPart, int iTechnique)
  {
    if (!(iPart.Effect is T effect))
      throw new Exception("Invalid effect!");
    if (RenderManager.Instance.GetEffect(this.mEffect) == null)
      RenderManager.Instance.RegisterEffect((Microsoft.Xna.Framework.Graphics.Effect) effect);
    this.mMaterial.FetchFromEffect(effect);
    this.mVertexBuffer = iMesh.VertexBuffer;
    this.mVertexBufferHash = this.mVertexBuffer.GetHashCode();
    this.mIndexBuffer = iMesh.IndexBuffer;
    this.mVertexDeclaration = iPart.VertexDeclaration;
    this.mBaseVertex = iPart.BaseVertex;
    this.mNumVertices = iPart.NumVertices;
    this.mStartIndex = iPart.StartIndex;
    this.mPrimitiveCount = iPart.PrimitiveCount;
    this.mVertexStride = iPart.VertexStride;
    this.mTechnique = iTechnique;
  }

  public virtual int Effect => this.mEffect;

  public virtual int Technique => this.mTechnique;

  public virtual VertexBuffer Vertices => this.mVertexBuffer;

  public virtual int VerticesHashCode => this.mVertexBufferHash;

  public virtual int VertexStride => this.mVertexStride;

  public virtual IndexBuffer Indices => this.mIndexBuffer;

  public virtual VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

  public virtual bool Cull(BoundingFrustum iViewFrustum) => false;

  public virtual void Draw(Microsoft.Xna.Framework.Graphics.Effect iEffect, BoundingFrustum iViewFrustum)
  {
    T iEffect1 = iEffect as T;
    this.mMaterial.AssignToEffect(iEffect1);
    iEffect1.CommitChanges();
    iEffect1.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
  }
}
