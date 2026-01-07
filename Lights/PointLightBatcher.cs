// Decompiled with JetBrains decompiler
// Type: PolygonHead.Lights.PointLightBatcher
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;

#nullable disable
namespace PolygonHead.Lights;

public sealed class PointLightBatcher : Light
{
  public const int MAX_LIGHTS = 128 /*0x80*/;
  private static PointLightBatcher sSingelton = (PointLightBatcher) null;
  private static volatile object sSingeltonLock = new object();
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private IndexBuffer mIndices;
  private Random mRandomizer = new Random();
  private int[] mUniqueIDs;
  private BatchedPointLightEffect.LightProperties[] mLights;
  private PointLightBatcher.LightLogic[] mLightLogic;
  private int mLastActiveLight;
  private IntHeap mFreeLights;
  private DynamicVertexBuffer[] mLightBuffers;
  private int[] mBufferedLastActiveLight;
  private int mRenderThreadLastActiveLight;

  public static PointLightBatcher Instance
  {
    get
    {
      if (PointLightBatcher.sSingelton == null)
      {
        lock (PointLightBatcher.sSingeltonLock)
        {
          if (PointLightBatcher.sSingelton == null)
            PointLightBatcher.sSingelton = new PointLightBatcher();
        }
      }
      return PointLightBatcher.sSingelton;
    }
  }

  private PointLightBatcher()
  {
    this.mUniqueIDs = new int[128 /*0x80*/];
    this.mLights = new BatchedPointLightEffect.LightProperties[128 /*0x80*/];
    this.mLightLogic = new PointLightBatcher.LightLogic[128 /*0x80*/];
    this.mLightBuffers = new DynamicVertexBuffer[3];
    this.mBufferedLastActiveLight = new int[3];
    this.mFreeLights = new IntHeap(128 /*0x80*/);
  }

  public void Initialize(GraphicsDevice iDevice)
  {
    LightHelper.CreateSphere(iDevice, out this.mVertices, out this.mVertexDeclaration, out this.mIndices);
    VertexElement[] iVertexElements = new VertexElement[LightHelper.VERTEX_ELEMENTS.Length + BatchedPointLightEffect.LightProperties.VERTEX_ELEMENTS.Length];
    LightHelper.VERTEX_ELEMENTS.CopyTo((Array) iVertexElements, 0);
    BatchedPointLightEffect.LightProperties.VERTEX_ELEMENTS.CopyTo((Array) iVertexElements, LightHelper.VERTEX_ELEMENTS.Length);
    this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(iVertexElements);
    for (int index = 0; index < 3; ++index)
      this.mLightBuffers[index] = new DynamicVertexBuffer(iDevice, 6144, BufferUsage.WriteOnly);
    this.Clear();
    if (RenderManager.Instance.GetEffect(BatchedPointLightEffect.TYPEHASH) != null)
      return;
    RenderManager.Instance.RegisterEffect((Microsoft.Xna.Framework.Graphics.Effect) new BatchedPointLightEffect(iDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
  }

  public void SpawnLight(
    ref Vector3 iPosition,
    ref PointLightBatcher.BatchedPointLight iLight,
    out PointLightBatcher.PointLightReference oReference)
  {
    this.SpawnLight(ref iPosition, ref iLight, out oReference, LightTransitionType.None, 0.0f);
  }

  public void SpawnLight(
    ref Vector3 iPosition,
    ref PointLightBatcher.BatchedPointLight iLight,
    out PointLightBatcher.PointLightReference oReference,
    LightTransitionType iTransition,
    float iTransitionTime)
  {
    if (!this.Enabled || this.mFreeLights.IsEmpty)
    {
      oReference.ID = -1;
      oReference.Sig = 0;
    }
    else
    {
      int val2 = this.mFreeLights.Pop();
      BatchedPointLightEffect.LightProperties lightProperties;
      lightProperties.Position = iPosition;
      lightProperties.Radius = iLight.Radius;
      lightProperties.DiffuseColor = iLight.DiffuseColor;
      lightProperties.AmbientColor = iLight.AmbientColor;
      lightProperties.SpecularAmount = iLight.SpecularAmount;
      lightProperties.Intensity = 0.0f;
      PointLightBatcher.LightLogic lightLogic;
      lightLogic.Active = true;
      lightLogic.VariationType = iLight.VariationType;
      lightLogic.VariationSpeed = iLight.VariationSpeed;
      lightLogic.VariationAmount = iLight.VariationAmount;
      lightLogic.TransitionType = iTransition;
      lightLogic.TransitionTime = 0.0f;
      lightLogic.TransitionTotalTime = iTransitionTime;
      lightLogic.VariationCurrent = 0.0f;
      lightLogic.VariationTimer = 0.0f;
      this.mLights[val2] = lightProperties;
      this.mLightLogic[val2] = lightLogic;
      this.mLastActiveLight = Math.Max(this.mLastActiveLight, val2);
      oReference.ID = val2;
      oReference.Sig = this.mRandomizer.Next(2147483646) + 1;
      this.mUniqueIDs[val2] = oReference.Sig;
    }
  }

  public void DisableLight(
    ref PointLightBatcher.PointLightReference iReference)
  {
    this.DisableLight(ref iReference, LightTransitionType.None, 0.0f);
  }

  public void DisableLight(
    ref PointLightBatcher.PointLightReference iReference,
    LightTransitionType iTransition,
    float iTransitionTime)
  {
    if (!this.ValidRef(ref iReference) || !this.mLightLogic[iReference.ID].Active)
      return;
    this.mLightLogic[iReference.ID].TransitionType = iTransition;
    this.mLightLogic[iReference.ID].TransitionTime = iTransitionTime;
    this.mLightLogic[iReference.ID].TransitionTotalTime = iTransitionTime;
    this.mLightLogic[iReference.ID].Active = false;
  }

  public void SetLightPosition(
    ref Vector3 iPosition,
    ref PointLightBatcher.PointLightReference iReference)
  {
    if (!this.ValidRef(ref iReference))
      return;
    this.mLights[iReference.ID].Position = iPosition;
  }

  public void SetLight(
    ref Vector3 iPosition,
    ref PointLightBatcher.BatchedPointLight iLight,
    ref PointLightBatcher.PointLightReference iReference)
  {
    if (!this.ValidRef(ref iReference))
      return;
    this.mLights[iReference.ID].Position = iPosition;
    this.mLights[iReference.ID].Radius = iLight.Radius;
    this.mLights[iReference.ID].DiffuseColor = iLight.DiffuseColor;
    this.mLights[iReference.ID].AmbientColor = iLight.AmbientColor;
    this.mLights[iReference.ID].SpecularAmount = iLight.SpecularAmount;
    this.mLightLogic[iReference.ID].VariationType = iLight.VariationType;
    this.mLightLogic[iReference.ID].VariationSpeed = iLight.VariationSpeed;
    this.mLightLogic[iReference.ID].VariationAmount = iLight.VariationAmount;
  }

  public bool IsLightActive(
    ref PointLightBatcher.PointLightReference iReference)
  {
    return this.ValidRef(ref iReference) && this.mLightLogic[iReference.ID].IsActive;
  }

  private bool ValidRef(ref PointLightBatcher.PointLightReference iRef)
  {
    if (iRef.ID >= 0 && this.mUniqueIDs[iRef.ID] == iRef.Sig)
      return true;
    iRef.ID = -1;
    iRef.Sig = 0;
    return false;
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    for (int iValue = 0; iValue <= this.mLastActiveLight; ++iValue)
    {
      if (this.mLightLogic[iValue].IsActive)
      {
        if (this.mLightLogic[iValue].Active)
        {
          this.mLightLogic[iValue].TransitionTime += iDeltaTime;
          if ((double) this.mLightLogic[iValue].TransitionTime >= (double) this.mLightLogic[iValue].TransitionTotalTime)
            this.mLightLogic[iValue].TransitionType = LightTransitionType.None;
        }
        else
          this.mLightLogic[iValue].TransitionTime -= iDeltaTime;
        if (this.mLightLogic[iValue].IsActive)
        {
          this.mLights[iValue].Intensity = Light.GetIntensity(this.mLightLogic[iValue].TransitionType, this.mLightLogic[iValue].TransitionTime / this.mLightLogic[iValue].TransitionTotalTime);
          this.mLights[iValue].Intensity *= Light.GetIntensity(this.mLightLogic[iValue].VariationType, iDeltaTime, this.mLightLogic[iValue].VariationSpeed, this.mLightLogic[iValue].VariationAmount, ref this.mLightLogic[iValue].VariationTimer, ref this.mLightLogic[iValue].VariationCurrent);
        }
        else
        {
          this.mLights[iValue].Intensity = 0.0f;
          this.mLights[iValue].Radius = 0.0f;
          this.mUniqueIDs[iValue] = int.MinValue;
          this.mFreeLights.Push(iValue);
        }
      }
    }
    while (this.mLastActiveLight >= 0 && !this.mLightLogic[this.mLastActiveLight].IsActive)
      --this.mLastActiveLight;
    this.mBufferedLastActiveLight[(int) iDataChannel] = this.mLastActiveLight;
    if (this.mLastActiveLight < 0)
      return;
    this.mLightBuffers[(int) iDataChannel].SetData<BatchedPointLightEffect.LightProperties>(0, this.mLights, 0, this.mLastActiveLight + 1, 48 /*0x30*/, SetDataOptions.Discard);
  }

  public override Vector3 DiffuseColor
  {
    get => new Vector3();
    set
    {
    }
  }

  public override Vector3 AmbientColor
  {
    get => new Vector3();
    set
    {
    }
  }

  public override float SpecularAmount
  {
    get => 0.0f;
    set
    {
    }
  }

  public override int Effect => BatchedPointLightEffect.TYPEHASH;

  public override int Technique => 0;

  public override VertexBuffer VertexBuffer => this.mVertices;

  public override IndexBuffer IndexBuffer => this.mIndices;

  public override VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

  public override int VertexStride => 12;

  public override bool CastShadows
  {
    get => false;
    set
    {
    }
  }

  public override int ShadowMapSize
  {
    get => 0;
    set
    {
    }
  }

  protected internal override void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
    this.mRenderThreadLastActiveLight = this.mBufferedLastActiveLight[(int) iDataChannel];
  }

  public override bool ShouldDraw(BoundingFrustum iViewFrustum)
  {
    return this.mRenderThreadLastActiveLight >= 0;
  }

  public override void DrawShadows(DataChannel iDataChannel, float iDeltaTime, Scene iScene)
  {
  }

  public override void Draw(
    Microsoft.Xna.Framework.Graphics.Effect iEffect,
    DataChannel iDataChannel,
    float iDeltaTime,
    Texture2D iNormalMap,
    Texture2D iDepthMap)
  {
    BatchedPointLightEffect pointLightEffect = iEffect as BatchedPointLightEffect;
    pointLightEffect.NormalMap = iNormalMap;
    pointLightEffect.DepthMap = iDepthMap;
    pointLightEffect.HalfPixel = new Vector2()
    {
      X = 0.5f / (float) iNormalMap.Width,
      Y = 0.5f / (float) iNormalMap.Height
    };
    pointLightEffect.GraphicsDevice.Vertices[0].SetFrequencyOfIndexData(this.mRenderThreadLastActiveLight + 1);
    pointLightEffect.GraphicsDevice.Vertices[1].SetSource((VertexBuffer) this.mLightBuffers[(int) iDataChannel], 0, 48 /*0x30*/);
    pointLightEffect.GraphicsDevice.Vertices[1].SetFrequencyOfInstanceData(1);
    pointLightEffect.CommitChanges();
    pointLightEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 42, 0, 80 /*0x50*/);
    pointLightEffect.GraphicsDevice.Vertices[0].SetFrequencyOfIndexData(1);
    pointLightEffect.GraphicsDevice.Vertices[1].SetSource((VertexBuffer) null, 0, 0);
  }

  public override void DisposeShadowMap()
  {
  }

  public override void CreateShadowMap()
  {
  }

  public void Clear()
  {
    this.mLastActiveLight = -1;
    this.mFreeLights.Clear();
    for (int iValue = 0; iValue < 128 /*0x80*/; ++iValue)
    {
      this.mUniqueIDs[iValue] = int.MinValue;
      this.mFreeLights.Push(iValue);
    }
  }

  public struct BatchedPointLight
  {
    public float Radius;
    public Vector3 DiffuseColor;
    public float SpecularAmount;
    public Vector3 AmbientColor;
    public LightVariationType VariationType;
    public float VariationSpeed;
    public float VariationAmount;
  }

  public struct PointLightReference
  {
    public int ID;
    public int Sig;
  }

  private struct LightLogic
  {
    public bool Active;
    public LightVariationType VariationType;
    public float VariationSpeed;
    public float VariationAmount;
    public float VariationTimer;
    public float VariationCurrent;
    public LightTransitionType TransitionType;
    public float TransitionTime;
    public float TransitionTotalTime;

    public bool IsActive => this.Active || (double) this.TransitionTime >= 0.0;
  }
}
