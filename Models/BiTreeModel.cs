// Decompiled with JetBrains decompiler
// Type: PolygonHead.Models.BiTreeModel
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;

#nullable disable
namespace PolygonHead.Models;

public class BiTreeModel : IDisposable
{
  private BiTreeRootNode[] mBiTrees;
  private BiTreeModel.RenderData[][] mRenderData;
  private bool mSwayEnabled;

  private BiTreeModel()
  {
  }

  public static BiTreeModel Read(ContentReader iInput)
  {
    BiTreeModel biTreeModel = new BiTreeModel();
    GraphicsDevice graphicsDevice = (iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice;
    int length = iInput.ReadInt32();
    biTreeModel.mBiTrees = new BiTreeRootNode[length];
    for (int index = 0; index < length; ++index)
      biTreeModel.mBiTrees[index] = new BiTreeRootNode(iInput, graphicsDevice);
    biTreeModel.mRenderData = new BiTreeModel.RenderData[3][];
    for (int index1 = 0; index1 < 3; ++index1)
    {
      biTreeModel.mRenderData[index1] = new BiTreeModel.RenderData[biTreeModel.mBiTrees.Length];
      for (int index2 = 0; index2 < biTreeModel.mBiTrees.Length; ++index2)
        biTreeModel.mRenderData[index1][index2] = new BiTreeModel.RenderData(biTreeModel.mBiTrees[index2]);
    }
    return biTreeModel;
  }

  public BiTreeNode[] Trees => (BiTreeNode[]) this.mBiTrees;

  public void AddToScene(DataChannel iDataChannel, Scene iScene)
  {
    if (iDataChannel == DataChannel.None)
      return;
    BiTreeModel.RenderData[] renderDataArray = this.mRenderData[(int) iDataChannel];
    for (int index = 0; index < renderDataArray.Length; ++index)
    {
      renderDataArray[index].SwayEnabled = this.mSwayEnabled;
      if (renderDataArray[index].mIsAdditive)
        iScene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) renderDataArray[index]);
      else
        iScene.AddRenderableObject(iDataChannel, (IRenderableObject) renderDataArray[index]);
    }
  }

  public bool SwayEnabled
  {
    get => this.mSwayEnabled;
    set => this.mSwayEnabled = value;
  }

  public void Dispose()
  {
    for (int index = 0; index < this.mBiTrees.Length; ++index)
    {
      if (!this.mBiTrees[index].Effect.IsDisposed)
        this.mBiTrees[index].Effect.Dispose();
      this.mBiTrees[index].VertexBuffer.Dispose();
      this.mBiTrees[index].IndexBuffer.Dispose();
      this.mBiTrees[index].VertexDeclaration.Dispose();
    }
  }

  protected class RenderData : IRenderableObject, IRenderableAdditiveObject
  {
    public bool mIsAdditive;
    public bool SwayEnabled;
    private BiTreeRootNode mTree;
    private int mVerticesHash;
    private int mEffect;
    private int mTechnique;
    private int mShadowTechnique;
    private int mDepthTechnique;
    public AdditiveMaterial mAdditiveMaterial;
    public RenderDeferredMaterial mRenderDeferredMaterial;
    private BoundingBox mBoundingBox;

    public RenderData(BiTreeRootNode iTree)
    {
      this.mTree = iTree;
      this.mBoundingBox = this.mTree.BoundingBox;
      this.mEffect = this.mTree.Effect.GetType().GetHashCode();
      this.mVerticesHash = this.mTree.VertexBuffer.GetHashCode();
      if (this.mTree.Effect is AdditiveEffect effect1)
      {
        this.mIsAdditive = true;
        this.mTechnique = 0;
        this.mShadowTechnique = -1;
        this.mDepthTechnique = -1;
        this.mAdditiveMaterial.FetchFromEffect(effect1);
      }
      else
      {
        this.mIsAdditive = false;
        this.mDepthTechnique = 4;
        this.mShadowTechnique = 5;
        if (this.mTree.Effect is RenderDeferredEffect effect)
        {
          this.mTechnique = effect.ReflectionMap == null ? (effect.DiffuseTexture1 == null ? 0 : 1) : (effect.DiffuseTexture1 == null ? 2 : 3);
          this.mRenderDeferredMaterial.FetchFromEffect(effect);
        }
        if (!iTree.Visible)
        {
          this.mTechnique = -1;
          this.mDepthTechnique = -1;
        }
        if (iTree.CastShadows)
          return;
        this.mShadowTechnique = -1;
      }
    }

    public int Effect => this.mEffect;

    public int DepthTechnique => this.mDepthTechnique;

    public int Technique => this.mTechnique;

    public int ShadowTechnique => this.mShadowTechnique;

    public VertexBuffer Vertices => this.mTree.VertexBuffer;

    public IndexBuffer Indices => this.mTree.IndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mTree.VertexDeclaration;

    public int VertexStride => this.mTree.VertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingBox.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    void IRenderableObject.Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      iEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
      this.mRenderDeferredMaterial.AssignToEffect(iEffect1);
      bool flag = this.SwayEnabled && (double) this.mTree.Sway > 1.4012984643248171E-45 | (double) this.mTree.EntityInfluence > 1.4012984643248171E-45;
      if (flag)
      {
        iEffect1.Sway = this.mTree.Sway;
        iEffect1.EntityInfluence = this.mTree.EntityInfluence;
        iEffect1.GroundLevel = this.mTree.GroundLevel;
      }
      iEffect1.SwayEnabled = flag;
      iEffect.CommitChanges();
      int num = (int) this.mTree.Draw(iViewFrustum);
      iEffect1.SwayEnabled = false;
      iEffect.GraphicsDevice.RenderState.ReferenceStencil = 0;
    }

    void IRenderableAdditiveObject.Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      this.mAdditiveMaterial.AssignToEffect(iEffect as AdditiveEffect);
      iEffect.CommitChanges();
      int num = (int) this.mTree.Draw(iViewFrustum);
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredEffect iEffect1 = iEffect as RenderDeferredEffect;
      this.mRenderDeferredMaterial.WorldTransform = Matrix.Identity;
      this.mRenderDeferredMaterial.AssignOpacityToEffect(iEffect1);
      bool flag = this.SwayEnabled && (double) this.mTree.Sway > 1.4012984643248171E-45 | (double) this.mTree.EntityInfluence > 1.4012984643248171E-45;
      if (flag)
      {
        iEffect1.Sway = this.mTree.Sway;
        iEffect1.EntityInfluence = this.mTree.EntityInfluence;
        iEffect1.GroundLevel = this.mTree.GroundLevel;
      }
      iEffect1.SwayEnabled = flag;
      iEffect.CommitChanges();
      int num = (int) this.mTree.Draw(iViewFrustum);
      if (iEffect1 == null)
        return;
      iEffect1.SwayEnabled = false;
    }
  }
}
