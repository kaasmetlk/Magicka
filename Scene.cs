// Decompiled with JetBrains decompiler
// Type: PolygonHead.Scene
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using PolygonHead.Lights;
using System.Collections.Generic;

#nullable disable
namespace PolygonHead;

public class Scene : 
  IComparer<IRenderableObject>,
  IComparer<IRenderableAdditiveObject>,
  IComparer<IPostEffect>,
  IComparer<IRenderableGUIObject>,
  IComparer<IProjectionObject>
{
  protected List<IPreRenderRenderer>[] mPreRenderRenderers;
  protected List<IRenderableObject>[] mRenderableObjects;
  protected List<IRenderableAdditiveObject>[] mRenderableAdditiveObjects;
  protected List<IProjectionObject>[] mProjectionObjects;
  protected List<IRenderableGUIObject>[] mRenderableGUIObjects;
  protected List<IPostEffect>[] mPostEffects;
  protected List<Light> mLightsQueue;
  protected List<Light> mLightsRemoveQueue;
  protected List<Light> mLights;
  protected Camera mCamera;
  protected GraphicsDevice mDevice;
  protected object mRenderLock = new object();

  public Camera Camera => this.mCamera;

  public GraphicsDevice GraphicsDevice => this.mDevice;

  public Scene(GraphicsDevice iDevice, int iBufferCapacity, Camera iCamera)
  {
    this.mDevice = iDevice;
    this.mPreRenderRenderers = new List<IPreRenderRenderer>[3];
    for (int index = 0; index < 3; ++index)
      this.mPreRenderRenderers[index] = new List<IPreRenderRenderer>(iBufferCapacity);
    this.mRenderableObjects = new List<IRenderableObject>[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderableObjects[index] = new List<IRenderableObject>(iBufferCapacity);
    this.mRenderableAdditiveObjects = new List<IRenderableAdditiveObject>[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderableAdditiveObjects[index] = new List<IRenderableAdditiveObject>(iBufferCapacity);
    this.mProjectionObjects = new List<IProjectionObject>[3];
    for (int index = 0; index < 3; ++index)
      this.mProjectionObjects[index] = new List<IProjectionObject>(iBufferCapacity);
    this.mRenderableGUIObjects = new List<IRenderableGUIObject>[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderableGUIObjects[index] = new List<IRenderableGUIObject>(iBufferCapacity);
    this.mPostEffects = new List<IPostEffect>[3];
    for (int index = 0; index < 3; ++index)
      this.mPostEffects[index] = new List<IPostEffect>(iBufferCapacity);
    this.mLightsQueue = new List<Light>(iBufferCapacity / 4);
    this.mLightsRemoveQueue = new List<Light>(iBufferCapacity / 4);
    this.mLights = new List<Light>(iBufferCapacity);
    this.mDevice = iDevice;
    this.mCamera = iCamera;
  }

  public void AddPreRenderRenderers(DataChannel iDataChannel, IPreRenderRenderer iObject)
  {
    if (iObject == null)
      return;
    List<IPreRenderRenderer> preRenderRenderer = this.mPreRenderRenderers[(int) iDataChannel];
    lock (preRenderRenderer)
      preRenderRenderer.Add(iObject);
  }

  public virtual void AddRenderableObject(DataChannel iDataChannel, IRenderableObject iObject)
  {
    this.AddPreRenderRenderers(iDataChannel, iObject as IPreRenderRenderer);
    List<IRenderableObject> renderableObject = this.mRenderableObjects[(int) iDataChannel];
    lock (renderableObject)
      this.BinarySearchInsert<IRenderableObject>(renderableObject, iObject, 0, renderableObject.Count, (IComparer<IRenderableObject>) this);
  }

  public virtual void AddRenderableAdditiveObject(
    DataChannel iDataChannel,
    IRenderableAdditiveObject iObject)
  {
    this.AddPreRenderRenderers(iDataChannel, iObject as IPreRenderRenderer);
    List<IRenderableAdditiveObject> renderableAdditiveObject = this.mRenderableAdditiveObjects[(int) iDataChannel];
    lock (renderableAdditiveObject)
      this.BinarySearchInsert<IRenderableAdditiveObject>(renderableAdditiveObject, iObject, 0, renderableAdditiveObject.Count, (IComparer<IRenderableAdditiveObject>) this);
  }

  public void AddProjection(DataChannel iDataChannel, IProjectionObject iObject)
  {
    this.AddPreRenderRenderers(iDataChannel, iObject as IPreRenderRenderer);
    List<IProjectionObject> projectionObject = this.mProjectionObjects[(int) iDataChannel];
    lock (projectionObject)
      this.BinarySearchInsert<IProjectionObject>(projectionObject, iObject, 0, projectionObject.Count, (IComparer<IProjectionObject>) this);
  }

  private void BinarySearchInsert<T>(
    List<T> iList,
    T iObject,
    int iMin,
    int iMax,
    IComparer<T> iComparer)
    where T : class
  {
    int num1 = (iMax - iMin) / 2 + iMin;
    if (num1 == iMin)
    {
      if (num1 < iMax && iComparer.Compare(iObject, iList[num1]) > 0)
        ++num1;
      iList.Insert(num1, iObject);
    }
    else
    {
      int num2 = iComparer.Compare(iObject, iList[num1]);
      if (num2 > 0)
        this.BinarySearchInsert<T>(iList, iObject, num1 + 1, iMax, iComparer);
      else if (num2 < 0)
        this.BinarySearchInsert<T>(iList, iObject, iMin, num1, iComparer);
      else
        iList.Insert(num1 + 1, iObject);
    }
  }

  public virtual void AddRenderableGUIObject(DataChannel iDataChannel, IRenderableGUIObject iObject)
  {
    this.AddPreRenderRenderers(iDataChannel, iObject as IPreRenderRenderer);
    List<IRenderableGUIObject> renderableGuiObject = this.mRenderableGUIObjects[(int) iDataChannel];
    lock (renderableGuiObject)
      this.BinarySearchInsert<IRenderableGUIObject>(renderableGuiObject, iObject, 0, renderableGuiObject.Count, (IComparer<IRenderableGUIObject>) this);
  }

  public virtual void AddPostEffect(DataChannel iDataChannel, IPostEffect iObject)
  {
    this.AddPreRenderRenderers(iDataChannel, iObject as IPreRenderRenderer);
    List<IPostEffect> mPostEffect = this.mPostEffects[(int) iDataChannel];
    lock (mPostEffect)
      this.BinarySearchInsert<IPostEffect>(mPostEffect, iObject, 0, mPostEffect.Count, (IComparer<IPostEffect>) this);
  }

  internal virtual void AddLight(Light iLight)
  {
    lock (this.mLightsQueue)
      this.mLightsQueue.Add(iLight);
  }

  internal virtual void RemoveLight(Light iLight)
  {
    lock (this.mLightsRemoveQueue)
      this.mLightsRemoveQueue.Add(iLight);
  }

  public virtual void UpdatePreRenderRenderers(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Matrix iViewProjectionMatrix,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
    List<IPreRenderRenderer> preRenderRenderer = this.mPreRenderRenderers[(int) iDataChannel];
    int count = preRenderRenderer.Count;
    for (int index = 0; index < count; ++index)
      preRenderRenderer[index].PreRenderUpdate(iDataChannel, iDeltaTime, ref iViewProjectionMatrix, ref iCameraPosition, ref iCameraDirection);
  }

  public virtual void DrawShadows(
    DataChannel iDataChannel,
    BoundingFrustum iViewFrustum,
    float iDeltaTime)
  {
    List<IRenderableObject> renderableObject1 = this.mRenderableObjects[(int) iDataChannel];
    this.mDevice.RenderState.DepthBufferEnable = true;
    this.mDevice.RenderState.DepthBufferWriteEnable = true;
    VertexBuffer vertexBuffer = (VertexBuffer) null;
    IndexBuffer indexBuffer = (IndexBuffer) null;
    VertexDeclaration vertexDeclaration = (VertexDeclaration) null;
    int count1 = renderableObject1.Count;
    int index1 = 0;
    while (index1 < count1)
    {
      IRenderableObject renderableObject2 = renderableObject1[index1];
      if (renderableObject2.Cull(iViewFrustum) || renderableObject2.ShadowTechnique < 0)
      {
        ++index1;
      }
      else
      {
        Effect effect = RenderManager.Instance.GetEffect(renderableObject2.Effect);
        effect.CurrentTechnique = effect.Techniques[renderableObject2.ShadowTechnique];
        EffectTechnique currentTechnique = effect.CurrentTechnique;
        GraphicsDevice graphicsDevice = effect.GraphicsDevice;
        effect.Begin();
        int count2 = currentTechnique.Passes.Count;
        int index2 = index1 + 1;
        for (int index3 = 0; index3 < count2; ++index3)
        {
          currentTechnique.Passes[index3].Begin();
          if (vertexBuffer != renderableObject2.Vertices)
          {
            vertexBuffer = renderableObject2.Vertices;
            graphicsDevice.Vertices[0].SetSource(renderableObject2.Vertices, 0, renderableObject2.VertexStride);
          }
          if (indexBuffer != renderableObject2.Indices)
          {
            indexBuffer = renderableObject2.Indices;
            graphicsDevice.Indices = indexBuffer;
          }
          if (vertexDeclaration != renderableObject2.VertexDeclaration)
          {
            vertexDeclaration = renderableObject2.VertexDeclaration;
            graphicsDevice.VertexDeclaration = vertexDeclaration;
          }
          renderableObject2.DrawShadow(effect, iViewFrustum);
          index2 = index1 + 1;
          while (index2 < count1)
          {
            IRenderableObject renderableObject3 = renderableObject1[index2];
            if (renderableObject3.Effect == renderableObject2.Effect && renderableObject3.ShadowTechnique == renderableObject2.ShadowTechnique)
            {
              if (renderableObject3.Cull(iViewFrustum))
              {
                ++index2;
              }
              else
              {
                if (vertexBuffer != renderableObject3.Vertices)
                {
                  vertexBuffer = renderableObject3.Vertices;
                  graphicsDevice.Vertices[0].SetSource(renderableObject3.Vertices, 0, renderableObject3.VertexStride);
                }
                if (indexBuffer != renderableObject3.Indices)
                {
                  indexBuffer = renderableObject3.Indices;
                  graphicsDevice.Indices = indexBuffer;
                }
                if (vertexDeclaration != renderableObject3.VertexDeclaration)
                {
                  vertexDeclaration = renderableObject3.VertexDeclaration;
                  graphicsDevice.VertexDeclaration = vertexDeclaration;
                }
                renderableObject3.DrawShadow(effect, iViewFrustum);
                ++index2;
              }
            }
            else
              break;
          }
          currentTechnique.Passes[index3].End();
        }
        effect.End();
        index1 = index2;
      }
    }
  }

  public void UpdateLights(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
    while (this.mLightsQueue.Count > 0)
    {
      Light mLights = this.mLightsQueue[0];
      if (mLights != null && !this.mLights.Contains(mLights))
      {
        bool flag = false;
        for (int index1 = this.mLights.Count - 1; index1 >= 0; --index1)
        {
          if (this.mLights[index1].Effect == mLights.Effect)
          {
            for (int index2 = index1; index2 >= 0 && this.mLights[index2].Effect == mLights.Effect; --index2)
            {
              if (this.mLights[index2].Technique == mLights.Technique)
              {
                this.mLights.Insert(index2 + 1, mLights);
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              this.mLights.Insert(index1 + 1, mLights);
              flag = true;
              break;
            }
            break;
          }
        }
        if (!flag)
          this.mLights.Add(this.mLightsQueue[0]);
      }
      this.mLightsQueue.RemoveAt(0);
    }
    while (this.mLightsRemoveQueue.Count > 0)
    {
      this.mLights.Remove(this.mLightsRemoveQueue[0]);
      this.mLightsRemoveQueue.RemoveAt(0);
    }
    if (iDataChannel == DataChannel.None)
      return;
    for (int index = 0; index < this.mLights.Count; ++index)
      this.mLights[index].Update(iDataChannel, iDeltaTime, ref iCameraPosition, ref iCameraDirection);
  }

  public virtual void DrawLightShadows(
    DataChannel iDataChannel,
    float iDeltaTime,
    BoundingFrustum iViewFrustum)
  {
    for (int index = 0; index < this.mLights.Count; ++index)
    {
      Light mLight = this.mLights[index];
      if (mLight.CastShadows && mLight.ShouldDraw(iViewFrustum))
        mLight.DrawShadows(iDataChannel, iDeltaTime, this);
    }
  }

  public virtual void DrawLights(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection,
    BoundingFrustum iViewFrustum,
    ref Matrix iViewProjectionMatrix,
    ref Matrix iInverseViewProjectionMatrix,
    Texture2D iNormalMap,
    Texture2D iDepthMap)
  {
    lock (this.mRenderLock)
    {
      this.mDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
      this.mDevice.RenderState.DepthBufferEnable = true;
      this.mDevice.RenderState.DepthBufferWriteEnable = false;
      this.mDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
      DummyEffect globalDummyEffect = RenderManager.Instance.GlobalDummyEffect;
      globalDummyEffect.CameraPosition = iCameraPosition;
      globalDummyEffect.ViewProjection = iViewProjectionMatrix;
      globalDummyEffect.InverseViewProjection = iInverseViewProjectionMatrix;
      DummyEffect localDummyEffect = RenderManager.Instance.LocalDummyEffect;
      if (localDummyEffect != null)
      {
        localDummyEffect.CameraPosition = iCameraPosition;
        localDummyEffect.ViewProjection = iViewProjectionMatrix;
        localDummyEffect.InverseViewProjection = iInverseViewProjectionMatrix;
      }
      VertexBuffer vertexBuffer = (VertexBuffer) null;
      IndexBuffer indexBuffer = (IndexBuffer) null;
      VertexDeclaration vertexDeclaration = (VertexDeclaration) null;
      int count1 = this.mLights.Count;
      int index1 = 0;
      while (index1 < count1)
      {
        Light mLight1 = this.mLights[index1];
        if (!mLight1.ShouldDraw(iViewFrustum))
        {
          ++index1;
        }
        else
        {
          Effect effect = RenderManager.Instance.GetEffect(mLight1.Effect);
          effect.CurrentTechnique = effect.Techniques[mLight1.Technique];
          EffectTechnique currentTechnique = effect.CurrentTechnique;
          GraphicsDevice graphicsDevice = effect.GraphicsDevice;
          effect.Begin();
          int count2 = currentTechnique.Passes.Count;
          int index2 = index1 + 1;
          for (int index3 = 0; index3 < count2; ++index3)
          {
            currentTechnique.Passes[index3].Begin();
            if (vertexBuffer != mLight1.VertexBuffer)
            {
              vertexBuffer = mLight1.VertexBuffer;
              graphicsDevice.Vertices[0].SetSource(mLight1.VertexBuffer, 0, mLight1.VertexStride);
            }
            if (indexBuffer != mLight1.IndexBuffer)
            {
              indexBuffer = mLight1.IndexBuffer;
              graphicsDevice.Indices = indexBuffer;
            }
            if (vertexDeclaration != mLight1.VertexDeclaration)
            {
              vertexDeclaration = mLight1.VertexDeclaration;
              graphicsDevice.VertexDeclaration = vertexDeclaration;
            }
            mLight1.Draw(effect, iDataChannel, iDeltaTime, iNormalMap, iDepthMap);
            index2 = index1 + 1;
            while (index2 < count1)
            {
              Light mLight2 = this.mLights[index2];
              if (mLight2.Effect == mLight1.Effect && mLight2.Technique == mLight1.Technique)
              {
                if (!mLight2.ShouldDraw(iViewFrustum))
                {
                  ++index2;
                }
                else
                {
                  if (vertexBuffer != mLight2.VertexBuffer)
                  {
                    vertexBuffer = mLight2.VertexBuffer;
                    graphicsDevice.Vertices[0].SetSource(mLight2.VertexBuffer, 0, mLight2.VertexStride);
                  }
                  if (indexBuffer != mLight2.IndexBuffer)
                  {
                    indexBuffer = mLight2.IndexBuffer;
                    graphicsDevice.Indices = indexBuffer;
                  }
                  if (vertexDeclaration != mLight2.VertexDeclaration)
                  {
                    vertexDeclaration = mLight2.VertexDeclaration;
                    graphicsDevice.VertexDeclaration = vertexDeclaration;
                  }
                  mLight2.Draw(effect, iDataChannel, iDeltaTime, iNormalMap, iDepthMap);
                  ++index2;
                }
              }
              else
                break;
            }
            currentTechnique.Passes[index3].End();
          }
          effect.End();
          index1 = index2;
        }
      }
    }
  }

  public virtual void DrawDepth(
    DataChannel iDataChannel,
    float iDeltaTime,
    BoundingFrustum iViewFrustum,
    ref Matrix iViewMatrix,
    ref Matrix iProjectionMatrix,
    ref Matrix iViewProjectionMatrix)
  {
    List<IRenderableObject> renderableObject1 = this.mRenderableObjects[(int) iDataChannel];
    VertexBuffer vertexBuffer = (VertexBuffer) null;
    IndexBuffer indexBuffer = (IndexBuffer) null;
    VertexDeclaration vertexDeclaration = (VertexDeclaration) null;
    this.mDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    this.mDevice.RenderState.AlphaBlendEnable = false;
    this.mDevice.RenderState.DepthBufferEnable = true;
    this.mDevice.RenderState.DepthBufferWriteEnable = true;
    int count1 = renderableObject1.Count;
    int index1 = 0;
    while (index1 < count1)
    {
      IRenderableObject renderableObject2 = renderableObject1[index1];
      if (renderableObject2.Cull(iViewFrustum) || renderableObject2.DepthTechnique < 0)
      {
        ++index1;
      }
      else
      {
        int effect1 = renderableObject2.Effect;
        Effect effect2 = RenderManager.Instance.GetEffect(effect1);
        int depthTechnique = renderableObject2.DepthTechnique;
        effect2.CurrentTechnique = effect2.Techniques[depthTechnique];
        EffectTechnique currentTechnique = effect2.CurrentTechnique;
        GraphicsDevice graphicsDevice = effect2.GraphicsDevice;
        effect2.Begin();
        int count2 = currentTechnique.Passes.Count;
        int index2 = index1 + 1;
        for (int index3 = 0; index3 < count2; ++index3)
        {
          currentTechnique.Passes[index3].Begin();
          if (vertexBuffer != renderableObject2.Vertices)
          {
            vertexBuffer = renderableObject2.Vertices;
            graphicsDevice.Vertices[0].SetSource(renderableObject2.Vertices, 0, renderableObject2.VertexStride);
          }
          if (indexBuffer != renderableObject2.Indices)
          {
            indexBuffer = renderableObject2.Indices;
            graphicsDevice.Indices = indexBuffer;
          }
          if (vertexDeclaration != renderableObject2.VertexDeclaration)
          {
            vertexDeclaration = renderableObject2.VertexDeclaration;
            graphicsDevice.VertexDeclaration = vertexDeclaration;
          }
          renderableObject2.DrawShadow(effect2, iViewFrustum);
          index2 = index1 + 1;
          while (index2 < count1)
          {
            IRenderableObject renderableObject3 = renderableObject1[index2];
            if (renderableObject3.Effect == effect1 && renderableObject3.DepthTechnique == depthTechnique)
            {
              if (renderableObject3.Cull(iViewFrustum))
              {
                ++index2;
              }
              else
              {
                if (vertexBuffer != renderableObject3.Vertices)
                {
                  vertexBuffer = renderableObject3.Vertices;
                  graphicsDevice.Vertices[0].SetSource(renderableObject3.Vertices, 0, renderableObject3.VertexStride);
                }
                if (indexBuffer != renderableObject3.Indices)
                {
                  indexBuffer = renderableObject3.Indices;
                  graphicsDevice.Indices = indexBuffer;
                }
                if (vertexDeclaration != renderableObject3.VertexDeclaration)
                {
                  vertexDeclaration = renderableObject3.VertexDeclaration;
                  graphicsDevice.VertexDeclaration = vertexDeclaration;
                }
                renderableObject3.DrawShadow(effect2, iViewFrustum);
                ++index2;
              }
            }
            else
              break;
          }
          currentTechnique.Passes[index3].End();
        }
        effect2.End();
        index1 = index2;
      }
    }
    this.mDevice.RenderState.StencilEnable = false;
  }

  public virtual void Draw(
    DataChannel iDataChannel,
    float iDeltaTime,
    BoundingFrustum iViewFrustum)
  {
    this.mDevice.RenderState.StencilEnable = true;
    this.mDevice.RenderState.ReferenceStencil = 0;
    this.mDevice.RenderState.StencilFunction = CompareFunction.Always;
    this.mDevice.RenderState.StencilPass = StencilOperation.Replace;
    this.mDevice.RenderState.CounterClockwiseStencilPass = StencilOperation.Replace;
    this.mDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    this.mDevice.RenderState.AlphaBlendEnable = false;
    this.mDevice.RenderState.DepthBufferEnable = true;
    this.mDevice.RenderState.DepthBufferWriteEnable = true;
    List<IRenderableObject> renderableObject1 = this.mRenderableObjects[(int) iDataChannel];
    VertexBuffer vertexBuffer = (VertexBuffer) null;
    IndexBuffer indexBuffer = (IndexBuffer) null;
    VertexDeclaration vertexDeclaration = (VertexDeclaration) null;
    int count1 = renderableObject1.Count;
    int index1 = 0;
    while (index1 < count1)
    {
      IRenderableObject renderableObject2 = renderableObject1[index1];
      if (renderableObject2.Cull(iViewFrustum))
      {
        ++index1;
      }
      else
      {
        int effect1 = renderableObject2.Effect;
        Effect effect2 = RenderManager.Instance.GetEffect(effect1);
        int technique = renderableObject2.Technique;
        if (technique < 0)
        {
          ++index1;
        }
        else
        {
          effect2.CurrentTechnique = effect2.Techniques[technique];
          EffectTechnique currentTechnique = effect2.CurrentTechnique;
          effect2.Begin();
          int count2 = currentTechnique.Passes.Count;
          int index2 = index1 + 1;
          for (int index3 = 0; index3 < count2; ++index3)
          {
            currentTechnique.Passes[index3].Begin();
            if (vertexBuffer != renderableObject2.Vertices)
            {
              vertexBuffer = renderableObject2.Vertices;
              this.mDevice.Vertices[0].SetSource(renderableObject2.Vertices, 0, renderableObject2.VertexStride);
            }
            if (indexBuffer != renderableObject2.Indices)
            {
              indexBuffer = renderableObject2.Indices;
              this.mDevice.Indices = indexBuffer;
            }
            if (vertexDeclaration != renderableObject2.VertexDeclaration)
            {
              vertexDeclaration = renderableObject2.VertexDeclaration;
              this.mDevice.VertexDeclaration = vertexDeclaration;
            }
            renderableObject2.Draw(effect2, iViewFrustum);
            index2 = index1 + 1;
            while (index2 < count1)
            {
              IRenderableObject renderableObject3 = renderableObject1[index2];
              if (!(renderableObject3.Effect != effect1 | renderableObject3.Technique != technique))
              {
                if (renderableObject3.Cull(iViewFrustum))
                {
                  ++index2;
                }
                else
                {
                  if (vertexBuffer != renderableObject3.Vertices)
                  {
                    vertexBuffer = renderableObject3.Vertices;
                    this.mDevice.Vertices[0].SetSource(renderableObject3.Vertices, 0, renderableObject3.VertexStride);
                  }
                  if (indexBuffer != renderableObject3.Indices)
                  {
                    indexBuffer = renderableObject3.Indices;
                    this.mDevice.Indices = indexBuffer;
                  }
                  if (vertexDeclaration != renderableObject3.VertexDeclaration)
                  {
                    vertexDeclaration = renderableObject3.VertexDeclaration;
                    this.mDevice.VertexDeclaration = vertexDeclaration;
                  }
                  renderableObject3.Draw(effect2, iViewFrustum);
                  ++index2;
                }
              }
              else
                break;
            }
            currentTechnique.Passes[index3].End();
          }
          effect2.End();
          index1 = index2;
        }
      }
    }
    this.mDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
    this.mDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.All;
    this.mDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.All;
    this.mDevice.RenderState.ColorWriteChannels3 = ColorWriteChannels.All;
    this.mDevice.RenderState.StencilEnable = false;
  }

  public virtual void DrawAdditive(
    DataChannel iDataChannel,
    float iDeltaTime,
    BoundingFrustum iViewFrustum)
  {
    this.mDevice.RenderState.StencilEnable = false;
    this.mDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    this.mDevice.RenderState.AlphaBlendEnable = true;
    this.mDevice.RenderState.DestinationBlend = Blend.One;
    this.mDevice.RenderState.SourceBlend = Blend.InverseDestinationColor;
    this.mDevice.RenderState.DepthBufferEnable = true;
    this.mDevice.RenderState.DepthBufferWriteEnable = false;
    this.mDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
    List<IRenderableAdditiveObject> renderableAdditiveObject1 = this.mRenderableAdditiveObjects[(int) iDataChannel];
    VertexBuffer vertexBuffer = (VertexBuffer) null;
    IndexBuffer indexBuffer = (IndexBuffer) null;
    VertexDeclaration vertexDeclaration = (VertexDeclaration) null;
    int count1 = renderableAdditiveObject1.Count;
    int index1 = 0;
    while (index1 < count1)
    {
      IRenderableAdditiveObject renderableAdditiveObject2 = renderableAdditiveObject1[index1];
      if (renderableAdditiveObject2.Cull(iViewFrustum))
      {
        ++index1;
      }
      else
      {
        Effect effect = RenderManager.Instance.GetEffect(renderableAdditiveObject2.Effect);
        effect.CurrentTechnique = effect.Techniques[renderableAdditiveObject2.Technique];
        EffectTechnique currentTechnique = effect.CurrentTechnique;
        GraphicsDevice graphicsDevice = effect.GraphicsDevice;
        effect.Begin();
        int count2 = currentTechnique.Passes.Count;
        int index2 = index1 + 1;
        for (int index3 = 0; index3 < count2; ++index3)
        {
          currentTechnique.Passes[index3].Begin();
          if (vertexBuffer != renderableAdditiveObject2.Vertices)
          {
            vertexBuffer = renderableAdditiveObject2.Vertices;
            graphicsDevice.Vertices[0].SetSource(renderableAdditiveObject2.Vertices, 0, renderableAdditiveObject2.VertexStride);
          }
          if (indexBuffer != renderableAdditiveObject2.Indices)
          {
            indexBuffer = renderableAdditiveObject2.Indices;
            graphicsDevice.Indices = indexBuffer;
          }
          if (vertexDeclaration != renderableAdditiveObject2.VertexDeclaration)
          {
            vertexDeclaration = renderableAdditiveObject2.VertexDeclaration;
            graphicsDevice.VertexDeclaration = vertexDeclaration;
          }
          renderableAdditiveObject2.Draw(effect, iViewFrustum);
          index2 = index1 + 1;
          while (index2 < count1)
          {
            IRenderableAdditiveObject renderableAdditiveObject3 = renderableAdditiveObject1[index2];
            if (!(renderableAdditiveObject3.Effect != renderableAdditiveObject2.Effect | renderableAdditiveObject3.Technique != renderableAdditiveObject2.Technique))
            {
              if (renderableAdditiveObject3.Cull(iViewFrustum))
              {
                ++index2;
              }
              else
              {
                if (vertexBuffer != renderableAdditiveObject3.Vertices)
                {
                  vertexBuffer = renderableAdditiveObject3.Vertices;
                  graphicsDevice.Vertices[0].SetSource(renderableAdditiveObject3.Vertices, 0, renderableAdditiveObject3.VertexStride);
                }
                if (indexBuffer != renderableAdditiveObject3.Indices)
                {
                  indexBuffer = renderableAdditiveObject3.Indices;
                  graphicsDevice.Indices = indexBuffer;
                }
                if (vertexDeclaration != renderableAdditiveObject3.VertexDeclaration)
                {
                  vertexDeclaration = renderableAdditiveObject3.VertexDeclaration;
                  graphicsDevice.VertexDeclaration = vertexDeclaration;
                }
                renderableAdditiveObject3.Draw(effect, iViewFrustum);
                ++index2;
              }
            }
            else
              break;
          }
          currentTechnique.Passes[index3].End();
        }
        effect.End();
        index1 = index2;
      }
    }
    this.mDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
    this.mDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.All;
    this.mDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.All;
    this.mDevice.RenderState.ColorWriteChannels3 = ColorWriteChannels.All;
    this.mDevice.RenderState.StencilEnable = false;
  }

  public virtual void DrawProjection(
    DataChannel iDataChannel,
    float iDeltaTime,
    BoundingFrustum iViewFrustum,
    Texture2D iDepthMap)
  {
    this.mDevice.RenderState.DepthBufferWriteEnable = false;
    this.mDevice.RenderState.StencilEnable = true;
    this.mDevice.RenderState.StencilPass = StencilOperation.Keep;
    this.mDevice.RenderState.CounterClockwiseStencilPass = StencilOperation.Keep;
    this.mDevice.RenderState.AlphaBlendEnable = true;
    this.mDevice.RenderState.SourceBlend = Blend.SourceAlpha;
    this.mDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
    this.mDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
    this.mDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
    List<IProjectionObject> projectionObject1 = this.mProjectionObjects[(int) iDataChannel];
    VertexBuffer vertexBuffer = (VertexBuffer) null;
    IndexBuffer indexBuffer = (IndexBuffer) null;
    VertexDeclaration vertexDeclaration = (VertexDeclaration) null;
    int count1 = projectionObject1.Count;
    int index1 = 0;
    while (index1 < count1)
    {
      IProjectionObject projectionObject2 = projectionObject1[index1];
      if (projectionObject2.Cull(iViewFrustum))
      {
        ++index1;
      }
      else
      {
        Effect effect = RenderManager.Instance.GetEffect(projectionObject2.Effect);
        effect.CurrentTechnique = effect.Techniques[projectionObject2.Technique];
        EffectTechnique currentTechnique = effect.CurrentTechnique;
        GraphicsDevice graphicsDevice = effect.GraphicsDevice;
        effect.Begin();
        int count2 = currentTechnique.Passes.Count;
        int index2 = index1 + 1;
        for (int index3 = 0; index3 < count2; ++index3)
        {
          currentTechnique.Passes[index3].Begin();
          if (vertexBuffer != projectionObject2.Vertices)
          {
            vertexBuffer = projectionObject2.Vertices;
            graphicsDevice.Vertices[0].SetSource(projectionObject2.Vertices, 0, projectionObject2.VertexStride);
          }
          if (indexBuffer != projectionObject2.Indices)
          {
            indexBuffer = projectionObject2.Indices;
            graphicsDevice.Indices = indexBuffer;
          }
          if (vertexDeclaration != projectionObject2.VertexDeclaration)
          {
            vertexDeclaration = projectionObject2.VertexDeclaration;
            graphicsDevice.VertexDeclaration = vertexDeclaration;
          }
          projectionObject2.Draw(effect, iDepthMap);
          index2 = index1 + 1;
          while (index2 < count1)
          {
            IProjectionObject projectionObject3 = projectionObject1[index2];
            if (!(projectionObject3.Effect != projectionObject2.Effect | projectionObject3.Technique != projectionObject2.Technique))
            {
              if (projectionObject3.Cull(iViewFrustum))
              {
                ++index2;
              }
              else
              {
                if (vertexBuffer != projectionObject3.Vertices)
                {
                  vertexBuffer = projectionObject3.Vertices;
                  graphicsDevice.Vertices[0].SetSource(projectionObject3.Vertices, 0, projectionObject3.VertexStride);
                }
                if (indexBuffer != projectionObject3.Indices)
                {
                  indexBuffer = projectionObject3.Indices;
                  graphicsDevice.Indices = indexBuffer;
                }
                if (vertexDeclaration != projectionObject3.VertexDeclaration)
                {
                  vertexDeclaration = projectionObject3.VertexDeclaration;
                  graphicsDevice.VertexDeclaration = vertexDeclaration;
                }
                projectionObject3.Draw(effect, iDepthMap);
                ++index2;
              }
            }
            else
              break;
          }
          currentTechnique.Passes[index3].End();
        }
        effect.End();
        index1 = index2;
      }
    }
    this.mDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
    this.mDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.All;
    this.mDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.All;
    this.mDevice.RenderState.ColorWriteChannels3 = ColorWriteChannels.All;
    this.mDevice.RenderState.StencilEnable = false;
  }

  public virtual void DrawPost(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Vector2 iPixelSize,
    ref Matrix iViewMatrix,
    ref Matrix iProjectionMatrix,
    Texture2D iCandidate,
    Texture2D iDepthMap,
    Texture2D iNormalMap)
  {
    List<IPostEffect> mPostEffect = this.mPostEffects[(int) iDataChannel];
    for (int index = 0; index < mPostEffect.Count; ++index)
      mPostEffect[index].Draw(iDeltaTime, ref iPixelSize, ref iViewMatrix, ref iProjectionMatrix, iCandidate, iDepthMap, iNormalMap);
  }

  public virtual void DrawGui(DataChannel iDataChannel, float iDeltaTime)
  {
    List<IRenderableGUIObject> renderableGuiObject = this.mRenderableGUIObjects[(int) iDataChannel];
    this.mDevice.RenderState.AlphaBlendEnable = true;
    this.mDevice.RenderState.SourceBlend = Blend.SourceAlpha;
    this.mDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
    this.mDevice.RenderState.AlphaTestEnable = false;
    for (int index = 0; index < renderableGuiObject.Count; ++index)
      renderableGuiObject[index].Draw(iDeltaTime);
  }

  public void ClearObjects(DataChannel iDataChannel)
  {
    this.mPreRenderRenderers[(int) iDataChannel].Clear();
    this.mRenderableObjects[(int) iDataChannel].Clear();
    this.mRenderableAdditiveObjects[(int) iDataChannel].Clear();
    this.mProjectionObjects[(int) iDataChannel].Clear();
    this.mRenderableGUIObjects[(int) iDataChannel].Clear();
    this.mPostEffects[(int) iDataChannel].Clear();
  }

  public void ClearLights()
  {
    lock (this.mRenderLock)
    {
      for (int index = 0; index < this.mLights.Count; ++index)
        this.mLights[index].Disable();
      for (int index = 0; index < this.mLightsQueue.Count; ++index)
        this.mLightsQueue[index].Disable();
      this.mLights.Clear();
      this.mLightsQueue.Clear();
      this.mLightsRemoveQueue.Clear();
    }
  }

  int IComparer<IRenderableObject>.Compare(IRenderableObject x, IRenderableObject y)
  {
    int num = x.Effect - y.Effect;
    if (num == 0)
    {
      num = x.Technique - y.Technique;
      if (num == 0)
        num = x.VerticesHashCode - y.VerticesHashCode;
    }
    return num;
  }

  int IComparer<IRenderableAdditiveObject>.Compare(
    IRenderableAdditiveObject x,
    IRenderableAdditiveObject y)
  {
    int num = x.Effect - y.Effect;
    if (num == 0)
    {
      num = x.Technique - y.Technique;
      if (num == 0)
        num = x.VerticesHashCode - y.VerticesHashCode;
    }
    return num;
  }

  int IComparer<IPostEffect>.Compare(IPostEffect x, IPostEffect y) => x.ZIndex - y.ZIndex;

  int IComparer<IRenderableGUIObject>.Compare(IRenderableGUIObject x, IRenderableGUIObject y)
  {
    return x.ZIndex - y.ZIndex;
  }

  public int Compare(IProjectionObject x, IProjectionObject y)
  {
    int num = x.Effect - y.Effect;
    if (num == 0)
    {
      num = x.Technique - y.Technique;
      if (num == 0)
        num = x.VerticesHashCode - y.VerticesHashCode;
    }
    return num;
  }
}
