// Decompiled with JetBrains decompiler
// Type: PolygonHead.Lights.ParticleLightBatcher
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;

#nullable disable
namespace PolygonHead.Lights;

public sealed class ParticleLightBatcher : Light
{
  public const int MAX_LIGHTS = 256 /*0x0100*/;
  private static ParticleLightBatcher sSingelton = (ParticleLightBatcher) null;
  private static volatile object sSingeltonLock = new object();
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private IndexBuffer mIndices;
  private BatchedPointLightEffect.LightProperties[] mLights;
  private ParticleLightBatcher.LightLogic[] mLightLogic;
  private int mLastActiveLight;
  private IntHeap mFreeLights;
  private DynamicVertexBuffer[] mLightBuffers;
  private int[] mBufferedLastActiveLight;
  private int mRenderThreadLastActiveLight;

  public static ParticleLightBatcher Instance
  {
    get
    {
      if (ParticleLightBatcher.sSingelton == null)
      {
        lock (ParticleLightBatcher.sSingeltonLock)
        {
          if (ParticleLightBatcher.sSingelton == null)
            ParticleLightBatcher.sSingelton = new ParticleLightBatcher();
        }
      }
      return ParticleLightBatcher.sSingelton;
    }
  }

  private ParticleLightBatcher()
  {
    this.mLights = new BatchedPointLightEffect.LightProperties[256 /*0x0100*/];
    this.mLightLogic = new ParticleLightBatcher.LightLogic[256 /*0x0100*/];
    this.mLightBuffers = new DynamicVertexBuffer[3];
    this.mBufferedLastActiveLight = new int[3];
    this.mFreeLights = new IntHeap(256 /*0x0100*/);
  }

  public void Initialize(GraphicsDevice iDevice)
  {
    LightHelper.CreateSphere(iDevice, out this.mVertices, out this.mVertexDeclaration, out this.mIndices);
    VertexElement[] iVertexElements = new VertexElement[LightHelper.VERTEX_ELEMENTS.Length + BatchedPointLightEffect.LightProperties.VERTEX_ELEMENTS.Length];
    LightHelper.VERTEX_ELEMENTS.CopyTo((Array) iVertexElements, 0);
    BatchedPointLightEffect.LightProperties.VERTEX_ELEMENTS.CopyTo((Array) iVertexElements, LightHelper.VERTEX_ELEMENTS.Length);
    this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(iVertexElements);
    for (int index = 0; index < 3; ++index)
      this.mLightBuffers[index] = new DynamicVertexBuffer(iDevice, 12288 /*0x3000*/, BufferUsage.WriteOnly);
    this.Clear();
    if (RenderManager.Instance.GetEffect(BatchedPointLightEffect.TYPEHASH) != null)
      return;
    RenderManager.Instance.RegisterEffect((Microsoft.Xna.Framework.Graphics.Effect) new BatchedPointLightEffect(iDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
  }

  public void SpawnLight(ref ParticleLightBatcher.ParticleLight iLight)
  {
    if (!this.Enabled || this.mFreeLights.IsEmpty)
      return;
    int val2 = this.mFreeLights.Pop();
    BatchedPointLightEffect.LightProperties lightProperties;
    lightProperties.Position = iLight.Position;
    lightProperties.Radius = iLight.RadiusStart;
    lightProperties.DiffuseColor = iLight.DiffuseColor;
    lightProperties.AmbientColor = iLight.AmbientColor;
    lightProperties.SpecularAmount = iLight.SpecularAmount;
    lightProperties.Intensity = 0.0f;
    ParticleLightBatcher.LightLogic lightLogic;
    lightLogic.RadiusStart = iLight.RadiusStart;
    lightLogic.RadiusEnd = iLight.RadiusEnd;
    lightLogic.Velocity = iLight.Velocity;
    lightLogic.Gravity = iLight.Gravity;
    lightLogic.Drag = iLight.Drag;
    lightLogic.TTL = iLight.TTL;
    lightLogic.InvMaxTTL = 1f / iLight.TTL;
    this.mLights[val2] = lightProperties;
    this.mLightLogic[val2] = lightLogic;
    this.mLastActiveLight = Math.Max(this.mLastActiveLight, val2);
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    for (int iValue = 0; iValue <= this.mLastActiveLight; ++iValue)
    {
      if ((double) this.mLightLogic[iValue].TTL >= 0.0)
      {
        this.mLightLogic[iValue].TTL -= iDeltaTime;
        float num1 = this.mLightLogic[iValue].TTL * this.mLightLogic[iValue].InvMaxTTL;
        this.mLightLogic[iValue].Velocity.Y += this.mLightLogic[iValue].Gravity * iDeltaTime;
        float num2 = (float) Math.Pow((double) this.mLightLogic[iValue].Drag, (double) iDeltaTime);
        this.mLightLogic[iValue].Velocity.X *= num2;
        this.mLightLogic[iValue].Velocity.Y *= num2;
        this.mLightLogic[iValue].Velocity.Z *= num2;
        Vector3 vector3;
        vector3.X = this.mLightLogic[iValue].Velocity.X * iDeltaTime;
        vector3.Y = this.mLightLogic[iValue].Velocity.Y * iDeltaTime;
        vector3.Z = this.mLightLogic[iValue].Velocity.Z * iDeltaTime;
        this.mLights[iValue].Position.X += vector3.X;
        this.mLights[iValue].Position.Y += vector3.Y;
        this.mLights[iValue].Position.Z += vector3.Z;
        this.mLights[iValue].Radius = (float) ((double) this.mLightLogic[iValue].RadiusStart * (double) num1 + (double) this.mLightLogic[iValue].RadiusEnd * (1.0 - (double) num1));
        float num3 = (float) ((double) num1 * (1.0 - (double) num1) * 8.0);
        this.mLights[iValue].Intensity = MathHelper.Clamp(num3, 0.0f, 1f);
        if ((double) this.mLightLogic[iValue].TTL < 0.0)
          this.mFreeLights.Push(iValue);
      }
      else
        this.mLights[iValue].Radius = 0.0f;
    }
    while (this.mLastActiveLight >= 0 && (double) this.mLightLogic[this.mLastActiveLight].TTL < 0.0)
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
    for (int iValue = 0; iValue < 256 /*0x0100*/; ++iValue)
      this.mFreeLights.Push(iValue);
  }

  public struct ParticleLight
  {
    public Vector3 Position;
    public float RadiusStart;
    public float RadiusEnd;
    public Vector3 DiffuseColor;
    public float SpecularAmount;
    public Vector3 AmbientColor;
    public Vector3 Velocity;
    public float Gravity;
    public float Drag;
    public float TTL;
  }

  private struct LightLogic
  {
    public float RadiusStart;
    public float RadiusEnd;
    public Vector3 Velocity;
    public float Gravity;
    public float Drag;
    public float TTL;
    public float InvMaxTTL;
  }
}
