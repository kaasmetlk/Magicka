// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.ParticleSystem
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;

#nullable disable
namespace PolygonHead.ParticleEffects;

public sealed class ParticleSystem
{
  public const int MAX_PARTICLES = 32768 /*0x8000*/;
  private static ParticleSystem sSingelton = (ParticleSystem) null;
  private static volatile object sSingeltonLock = new object();
  private static readonly float sParticleMultiplyerBase = (float) Math.Log(0.98);
  private static float sParticleMultiplyer;
  private static float sParticleInvMultiplyer;
  private ParticleEffect mEffect;
  private GraphicsDevice mDevice;
  private VertexBuffer mVertices;
  private DynamicVertexBuffer[] mParticleBuffers;
  private int[] mBufferedLastActiveParticle;
  private VertexDeclaration mVertexDeclaration;
  private IndexBuffer mIndices;
  private ParticleSystem.ParticleLogic[] mParticleLogic;
  private ParticleSystem.ParticleGraphics[] mParticleGraphics;
  private IntHeap mFreeParticles;
  private int mLastActiveParticle = -1;

  public static ParticleSystem Instance
  {
    get
    {
      if (ParticleSystem.sSingelton == null)
      {
        lock (ParticleSystem.sSingeltonLock)
        {
          if (ParticleSystem.sSingelton == null)
            ParticleSystem.sSingelton = new ParticleSystem();
        }
      }
      return ParticleSystem.sSingelton;
    }
  }

  private ParticleSystem()
  {
    this.mFreeParticles = new IntHeap(32768 /*0x8000*/);
    this.mParticleLogic = new ParticleSystem.ParticleLogic[32768 /*0x8000*/];
    this.mParticleGraphics = new ParticleSystem.ParticleGraphics[32768 /*0x8000*/];
    this.mBufferedLastActiveParticle = new int[3];
    this.mParticleBuffers = new DynamicVertexBuffer[3];
    ParticleSystem.SetSpawnModifier(1f);
  }

  public void Initialize(
    GraphicsDevice iDevice,
    Texture3D iParticleSheetA,
    Texture3D iParticleSheetB,
    Texture3D iParticleSheetC,
    Texture3D iParticleSheetD)
  {
    this.mDevice = iDevice;
    if (this.mVertices != null && !this.mVertices.IsDisposed)
      this.mVertices.Dispose();
    this.mEffect = new ParticleEffect(this.mDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
    this.mEffect.SpritesA = iParticleSheetA;
    this.mEffect.SpritesB = iParticleSheetB;
    this.mEffect.SpritesC = iParticleSheetC;
    this.mEffect.SpritesD = iParticleSheetD;
    this.Clear();
    this.CreateVertices();
  }

  public void CreateVertices()
  {
    if (this.mVertices == null || this.mVertices.IsDisposed)
    {
      this.mVertices = new VertexBuffer(this.mDevice, 48 /*0x30*/, BufferUsage.WriteOnly);
      this.mParticleBuffers[0] = new DynamicVertexBuffer(this.mDevice, 1835008 /*0x1C0000*/, BufferUsage.WriteOnly);
      this.mParticleBuffers[1] = new DynamicVertexBuffer(this.mDevice, 1835008 /*0x1C0000*/, BufferUsage.WriteOnly);
      this.mParticleBuffers[2] = new DynamicVertexBuffer(this.mDevice, 1835008 /*0x1C0000*/, BufferUsage.WriteOnly);
      this.mVertices.SetData<ParticleSystem.Vertex>(new ParticleSystem.Vertex[4]
      {
        new ParticleSystem.Vertex()
        {
          Corner = 0.0f,
          TexCoord = new Vector2(1f, 1f)
        },
        new ParticleSystem.Vertex()
        {
          Corner = 1f,
          TexCoord = new Vector2(0.0f, 1f)
        },
        new ParticleSystem.Vertex()
        {
          Corner = 2f,
          TexCoord = new Vector2(0.0f, 0.0f)
        },
        new ParticleSystem.Vertex()
        {
          Corner = 3f,
          TexCoord = new Vector2(1f, 0.0f)
        }
      });
      this.mVertexDeclaration = new VertexDeclaration(this.mDevice, ParticleSystem.Vertex.VERTEXELEMENTS);
    }
    if (this.mIndices != null && !this.mIndices.IsDisposed)
      return;
    int[] data = new int[6]{ 0, 1, 2, 0, 2, 3 };
    this.mIndices = new IndexBuffer(this.mDevice, data.Length * 4, BufferUsage.WriteOnly, IndexElementSize.ThirtyTwoBits);
    this.mIndices.SetData<int>(data);
  }

  public static void SetSpawnModifier(float iMultiplyer)
  {
    ParticleSystem.sParticleMultiplyer = iMultiplyer;
    ParticleSystem.sParticleInvMultiplyer = 1f - iMultiplyer;
  }

  public static float GetSpawnMultiplyer(float iParticles) => ParticleSystem.sParticleMultiplyer;

  public void Clear()
  {
    this.mLastActiveParticle = -1;
    this.mFreeParticles.Clear();
    for (int iValue = 0; iValue < 32768 /*0x8000*/; ++iValue)
      this.mFreeParticles.Push(iValue);
  }

  public void SpawnParticle(ref Particle iParticle)
  {
    if (this.mFreeParticles.IsEmpty)
      return;
    int val2 = this.mFreeParticles.Pop();
    ParticleSystem.ParticleGraphics particleGraphics;
    particleGraphics.Position = iParticle.Position;
    particleGraphics.Rotation = iParticle.Rotation;
    particleGraphics.Radius = iParticle.StartSize;
    particleGraphics.NormalizedLifetime = 0.0f;
    particleGraphics.Color = iParticle.Color;
    particleGraphics.Sprite = iParticle.Sprite;
    particleGraphics.AlphaBlended = iParticle.AlphaBlended ? (byte) 1 : (byte) 0;
    particleGraphics.HSVColorize = (byte) ((iParticle.HSV ? 50 : 0) + (iParticle.Colorize ? 5 : 0));
    particleGraphics.RotationAligned = iParticle.RotationAligned ? (byte) 1 : (byte) 0;
    particleGraphics.Velocity = iParticle.Velocity;
    ParticleSystem.ParticleLogic particleLogic;
    particleLogic.RotationSpeed = iParticle.RotationVelocity;
    particleLogic.Gravity = iParticle.Gravity;
    particleLogic.Drag = iParticle.Drag;
    particleLogic.TTL = iParticle.TTL;
    particleLogic.InvMaxTTL = 1f / iParticle.TTL;
    particleLogic.StartSize = iParticle.StartSize;
    particleLogic.EndSize = iParticle.EndSize;
    this.mParticleLogic[val2] = particleLogic;
    this.mParticleGraphics[val2] = particleGraphics;
    this.mLastActiveParticle = Math.Max(this.mLastActiveParticle, val2);
  }

  public void UpdateParticles(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mDevice == null)
      return;
    for (int iValue = 0; iValue <= this.mLastActiveParticle; ++iValue)
    {
      if ((double) this.mParticleLogic[iValue].TTL >= 0.0)
      {
        this.mParticleLogic[iValue].TTL -= iDeltaTime;
        float num1 = this.mParticleLogic[iValue].TTL * this.mParticleLogic[iValue].InvMaxTTL;
        this.mParticleGraphics[iValue].NormalizedLifetime = 1f - num1;
        this.mParticleGraphics[iValue].Velocity.Y += this.mParticleLogic[iValue].Gravity * iDeltaTime;
        float num2 = (float) Math.Pow((double) this.mParticleLogic[iValue].Drag, (double) iDeltaTime);
        this.mParticleGraphics[iValue].Velocity.X *= num2;
        this.mParticleGraphics[iValue].Velocity.Y *= num2;
        this.mParticleGraphics[iValue].Velocity.Z *= num2;
        Vector3 vector3;
        vector3.X = this.mParticleGraphics[iValue].Velocity.X * iDeltaTime;
        vector3.Y = this.mParticleGraphics[iValue].Velocity.Y * iDeltaTime;
        vector3.Z = this.mParticleGraphics[iValue].Velocity.Z * iDeltaTime;
        this.mParticleGraphics[iValue].Position.X += vector3.X;
        this.mParticleGraphics[iValue].Position.Y += vector3.Y;
        this.mParticleGraphics[iValue].Position.Z += vector3.Z;
        this.mParticleGraphics[iValue].Rotation += this.mParticleLogic[iValue].RotationSpeed * iDeltaTime;
        this.mParticleGraphics[iValue].Radius = (float) ((double) this.mParticleLogic[iValue].StartSize * (double) num1 + (double) this.mParticleLogic[iValue].EndSize * (1.0 - (double) num1));
        if ((double) this.mParticleLogic[iValue].TTL < 0.0)
          this.mFreeParticles.Push(iValue);
      }
      else
        this.mParticleGraphics[iValue].Radius = 0.0f;
    }
    while (this.mLastActiveParticle >= 0 && (double) this.mParticleLogic[this.mLastActiveParticle].TTL < 0.0)
      --this.mLastActiveParticle;
    this.mBufferedLastActiveParticle[(int) iDataChannel] = this.mLastActiveParticle;
    if (this.mLastActiveParticle < 0)
      return;
    this.mParticleBuffers[(int) iDataChannel].SetData<ParticleSystem.ParticleGraphics>(0, this.mParticleGraphics, 0, this.mLastActiveParticle + 1, 56, SetDataOptions.Discard);
  }

  public void Draw(
    DataChannel iDataChannel,
    ref Matrix iViewMatrix,
    ref Matrix iProjectionMatrix,
    Texture2D iDepthMap)
  {
    int num = this.mBufferedLastActiveParticle[(int) iDataChannel];
    if (num < 0)
      return;
    this.mEffect.DestinationDimensions = new Vector2()
    {
      X = (float) iDepthMap.Width,
      Y = (float) iDepthMap.Height
    };
    this.mEffect.View = iViewMatrix;
    this.mEffect.Projection = iProjectionMatrix;
    this.mDevice.Vertices[0].SetSource(this.mVertices, 0, 12);
    this.mDevice.Vertices[0].SetFrequencyOfIndexData(num + 1);
    this.mDevice.Vertices[1].SetSource((VertexBuffer) this.mParticleBuffers[(int) iDataChannel], 0, 56);
    this.mDevice.Vertices[1].SetFrequencyOfInstanceData(1);
    this.mDevice.VertexDeclaration = this.mVertexDeclaration;
    this.mDevice.Indices = this.mIndices;
    this.mEffect.DepthTexture = iDepthMap;
    this.mDevice.RenderState.AlphaBlendEnable = true;
    this.mDevice.RenderState.DestinationBlend = Blend.SourceAlpha;
    this.mDevice.RenderState.SourceBlend = Blend.One;
    this.mDevice.RenderState.DepthBufferEnable = false;
    this.mDevice.RenderState.DepthBufferWriteEnable = false;
    this.mDevice.RenderState.AlphaTestEnable = false;
    this.mEffect.Begin();
    foreach (EffectPass pass in this.mEffect.CurrentTechnique.Passes)
    {
      pass.Begin();
      this.mDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
      pass.End();
    }
    this.mEffect.End();
    this.mDevice.Vertices[1].SetSource((VertexBuffer) null, 0, 0);
  }

  private struct Vertex
  {
    public const int VERTEX_DATA_SIZE = 12;
    public const int PARTICLE_DATA_SIZE = 56;
    public float Corner;
    public Vector2 TexCoord;
    public static readonly VertexElement[] VERTEXELEMENTS = new VertexElement[7]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 4, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
      new VertexElement((short) 1, (short) 0, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 1),
      new VertexElement((short) 1, (short) 16 /*0x10*/, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Normal, (byte) 1),
      new VertexElement((short) 1, (short) 24, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 1),
      new VertexElement((short) 1, (short) 40, VertexElementFormat.Byte4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 1),
      new VertexElement((short) 1, (short) 44, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, (byte) 0)
    };
  }

  private struct ParticleLogic
  {
    public float RotationSpeed;
    public float Gravity;
    public float Drag;
    public float TTL;
    public float InvMaxTTL;
    public float StartSize;
    public float EndSize;
  }

  private struct ParticleGraphics
  {
    public Vector3 Position;
    public float Rotation;
    public float Radius;
    public float NormalizedLifetime;
    public Vector4 Color;
    public byte Sprite;
    public byte AlphaBlended;
    public byte HSVColorize;
    public byte RotationAligned;
    public Vector3 Velocity;
  }
}
