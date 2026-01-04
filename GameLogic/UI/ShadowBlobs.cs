// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.ShadowBlobs
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

public class ShadowBlobs
{
  private static ShadowBlobs mSingelton;
  private static volatile object mSingeltonLock = new object();
  private Scene mScene;
  private Matrix[] mTransforms;
  private Matrix[] mInvTransforms;
  private Vector3[] mArgs;
  private int mNrOfBlobs;
  private VertexBuffer mVertices;
  private IndexBuffer mIndices;
  private VertexDeclaration mVertexDeclaration;
  private ShadowBlobs.RenderData[] mRenderData;

  public static ShadowBlobs Instance
  {
    get
    {
      if (ShadowBlobs.mSingelton == null)
      {
        lock (ShadowBlobs.mSingeltonLock)
        {
          if (ShadowBlobs.mSingelton == null)
            ShadowBlobs.mSingelton = new ShadowBlobs();
        }
      }
      return ShadowBlobs.mSingelton;
    }
  }

  private ShadowBlobs()
  {
    this.mTransforms = new Matrix[512 /*0x0200*/];
    this.mInvTransforms = new Matrix[512 /*0x0200*/];
    this.mArgs = new Vector3[512 /*0x0200*/];
    this.mRenderData = new ShadowBlobs.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new ShadowBlobs.RenderData();
    this.CreateVertexBuffer();
  }

  public void Initialize(Scene iScene) => this.mScene = iScene;

  private void CreateVertexBuffer()
  {
    ShadowBlobs.Vertex[] data1 = new ShadowBlobs.Vertex[240 /*0xF0*/];
    ushort[] data2 = new ushort[1080];
    for (int index1 = 0; index1 < 30; ++index1)
    {
      int num1 = index1 * 8;
      for (int index2 = 0; index2 < 8; ++index2)
      {
        data1[num1 + index2].Position.X = index2 % 2 < 1 ? -1f : 1f;
        data1[num1 + index2].Position.Y = index2 % 4 < 2 ? -1f : 1f;
        data1[num1 + index2].Position.Z = index2 % 8 < 4 ? -1f : 1f;
        data1[num1 + index2].Instance = (float) index1;
      }
      int num2 = index1 * 36;
      ushort[] numArray1 = data2;
      int index3 = num2;
      int num3 = index3 + 1;
      int num4 = (int) (ushort) num1;
      numArray1[index3] = (ushort) num4;
      ushort[] numArray2 = data2;
      int index4 = num3;
      int num5 = index4 + 1;
      int num6 = (int) (ushort) (num1 + 2);
      numArray2[index4] = (ushort) num6;
      ushort[] numArray3 = data2;
      int index5 = num5;
      int num7 = index5 + 1;
      int num8 = (int) (ushort) (num1 + 1);
      numArray3[index5] = (ushort) num8;
      ushort[] numArray4 = data2;
      int index6 = num7;
      int num9 = index6 + 1;
      int num10 = (int) (ushort) (num1 + 1);
      numArray4[index6] = (ushort) num10;
      ushort[] numArray5 = data2;
      int index7 = num9;
      int num11 = index7 + 1;
      int num12 = (int) (ushort) (num1 + 2);
      numArray5[index7] = (ushort) num12;
      ushort[] numArray6 = data2;
      int index8 = num11;
      int num13 = index8 + 1;
      int num14 = (int) (ushort) (num1 + 3);
      numArray6[index8] = (ushort) num14;
      ushort[] numArray7 = data2;
      int index9 = num13;
      int num15 = index9 + 1;
      int num16 = (int) (ushort) (num1 + 1);
      numArray7[index9] = (ushort) num16;
      ushort[] numArray8 = data2;
      int index10 = num15;
      int num17 = index10 + 1;
      int num18 = (int) (ushort) num1;
      numArray8[index10] = (ushort) num18;
      ushort[] numArray9 = data2;
      int index11 = num17;
      int num19 = index11 + 1;
      int num20 = (int) (ushort) (num1 + 4);
      numArray9[index11] = (ushort) num20;
      ushort[] numArray10 = data2;
      int index12 = num19;
      int num21 = index12 + 1;
      int num22 = (int) (ushort) (num1 + 4);
      numArray10[index12] = (ushort) num22;
      ushort[] numArray11 = data2;
      int index13 = num21;
      int num23 = index13 + 1;
      int num24 = (int) (ushort) (num1 + 5);
      numArray11[index13] = (ushort) num24;
      ushort[] numArray12 = data2;
      int index14 = num23;
      int num25 = index14 + 1;
      int num26 = (int) (ushort) (num1 + 1);
      numArray12[index14] = (ushort) num26;
      ushort[] numArray13 = data2;
      int index15 = num25;
      int num27 = index15 + 1;
      int num28 = (int) (ushort) num1;
      numArray13[index15] = (ushort) num28;
      ushort[] numArray14 = data2;
      int index16 = num27;
      int num29 = index16 + 1;
      int num30 = (int) (ushort) (num1 + 2);
      numArray14[index16] = (ushort) num30;
      ushort[] numArray15 = data2;
      int index17 = num29;
      int num31 = index17 + 1;
      int num32 = (int) (ushort) (num1 + 4);
      numArray15[index17] = (ushort) num32;
      ushort[] numArray16 = data2;
      int index18 = num31;
      int num33 = index18 + 1;
      int num34 = (int) (ushort) (num1 + 4);
      numArray16[index18] = (ushort) num34;
      ushort[] numArray17 = data2;
      int index19 = num33;
      int num35 = index19 + 1;
      int num36 = (int) (ushort) (num1 + 2);
      numArray17[index19] = (ushort) num36;
      ushort[] numArray18 = data2;
      int index20 = num35;
      int num37 = index20 + 1;
      int num38 = (int) (ushort) (num1 + 6);
      numArray18[index20] = (ushort) num38;
      ushort[] numArray19 = data2;
      int index21 = num37;
      int num39 = index21 + 1;
      int num40 = (int) (ushort) (num1 + 4);
      numArray19[index21] = (ushort) num40;
      ushort[] numArray20 = data2;
      int index22 = num39;
      int num41 = index22 + 1;
      int num42 = (int) (ushort) (num1 + 6);
      numArray20[index22] = (ushort) num42;
      ushort[] numArray21 = data2;
      int index23 = num41;
      int num43 = index23 + 1;
      int num44 = (int) (ushort) (num1 + 5);
      numArray21[index23] = (ushort) num44;
      ushort[] numArray22 = data2;
      int index24 = num43;
      int num45 = index24 + 1;
      int num46 = (int) (ushort) (num1 + 5);
      numArray22[index24] = (ushort) num46;
      ushort[] numArray23 = data2;
      int index25 = num45;
      int num47 = index25 + 1;
      int num48 = (int) (ushort) (num1 + 6);
      numArray23[index25] = (ushort) num48;
      ushort[] numArray24 = data2;
      int index26 = num47;
      int num49 = index26 + 1;
      int num50 = (int) (ushort) (num1 + 7);
      numArray24[index26] = (ushort) num50;
      ushort[] numArray25 = data2;
      int index27 = num49;
      int num51 = index27 + 1;
      int num52 = (int) (ushort) (num1 + 1);
      numArray25[index27] = (ushort) num52;
      ushort[] numArray26 = data2;
      int index28 = num51;
      int num53 = index28 + 1;
      int num54 = (int) (ushort) (num1 + 5);
      numArray26[index28] = (ushort) num54;
      ushort[] numArray27 = data2;
      int index29 = num53;
      int num55 = index29 + 1;
      int num56 = (int) (ushort) (num1 + 3);
      numArray27[index29] = (ushort) num56;
      ushort[] numArray28 = data2;
      int index30 = num55;
      int num57 = index30 + 1;
      int num58 = (int) (ushort) (num1 + 3);
      numArray28[index30] = (ushort) num58;
      ushort[] numArray29 = data2;
      int index31 = num57;
      int num59 = index31 + 1;
      int num60 = (int) (ushort) (num1 + 5);
      numArray29[index31] = (ushort) num60;
      ushort[] numArray30 = data2;
      int index32 = num59;
      int num61 = index32 + 1;
      int num62 = (int) (ushort) (num1 + 7);
      numArray30[index32] = (ushort) num62;
      ushort[] numArray31 = data2;
      int index33 = num61;
      int num63 = index33 + 1;
      int num64 = (int) (ushort) (num1 + 2);
      numArray31[index33] = (ushort) num64;
      ushort[] numArray32 = data2;
      int index34 = num63;
      int num65 = index34 + 1;
      int num66 = (int) (ushort) (num1 + 3);
      numArray32[index34] = (ushort) num66;
      ushort[] numArray33 = data2;
      int index35 = num65;
      int num67 = index35 + 1;
      int num68 = (int) (ushort) (num1 + 6);
      numArray33[index35] = (ushort) num68;
      ushort[] numArray34 = data2;
      int index36 = num67;
      int num69 = index36 + 1;
      int num70 = (int) (ushort) (num1 + 6);
      numArray34[index36] = (ushort) num70;
      ushort[] numArray35 = data2;
      int index37 = num69;
      int num71 = index37 + 1;
      int num72 = (int) (ushort) (num1 + 3);
      numArray35[index37] = (ushort) num72;
      ushort[] numArray36 = data2;
      int index38 = num71;
      int num73 = index38 + 1;
      int num74 = (int) (ushort) (num1 + 7);
      numArray36[index38] = (ushort) num74;
    }
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 3840 /*0x0F00*/, BufferUsage.WriteOnly);
      this.mVertices.SetData<ShadowBlobs.Vertex>(data1);
      this.mIndices = new IndexBuffer(Magicka.Game.Instance.GraphicsDevice, 2160, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
      this.mIndices.SetData<ushort>(data2);
      this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, ShadowBlobs.Vertex.VertexElements);
    }
    for (int index = 0; index < 3; ++index)
    {
      ShadowBlobs.RenderData renderData = this.mRenderData[index];
      renderData.mVertices = this.mVertices;
      renderData.mVerticesHash = this.mVertices.GetHashCode();
      renderData.mIndices = this.mIndices;
      renderData.mVertexDeclaration = this.mVertexDeclaration;
    }
  }

  public void AddShadowBlob(ref Vector3 iPosition, float iRadius, float iDeadTime)
  {
    Vector3 up = Vector3.Up;
    up.Y *= 4f;
    Vector3 unitZ = Vector3.UnitZ;
    unitZ.Z = -unitZ.Z;
    Vector3 unitX = Vector3.UnitX;
    Matrix matrix = new Matrix();
    matrix.Backward = up;
    matrix.Right = unitX;
    matrix.Up = unitZ;
    matrix.M11 *= iRadius;
    matrix.M12 *= iRadius;
    matrix.M13 *= iRadius;
    matrix.M21 *= iRadius;
    matrix.M22 *= iRadius;
    matrix.M23 *= iRadius;
    matrix.M44 = 1f;
    matrix.Translation = iPosition;
    this.mTransforms[this.mNrOfBlobs] = matrix;
    this.mArgs[this.mNrOfBlobs] = new Vector3(iDeadTime, 0.0f, 0.0f);
    Matrix.Invert(ref matrix, out this.mInvTransforms[this.mNrOfBlobs]);
    ++this.mNrOfBlobs;
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mNrOfBlobs > 0)
    {
      ShadowBlobs.RenderData iObject = this.mRenderData[(int) iDataChannel];
      Array.Copy((Array) this.mTransforms, 0, (Array) iObject.mTransforms, 0, this.mNrOfBlobs);
      Array.Copy((Array) this.mInvTransforms, 0, (Array) iObject.mInvTransforms, 0, this.mNrOfBlobs);
      Array.Copy((Array) this.mArgs, 0, (Array) iObject.mArgs, 0, this.mNrOfBlobs);
      iObject.mNrOfBlobs = this.mNrOfBlobs;
      this.mScene.AddProjection(iDataChannel, (IProjectionObject) iObject);
    }
    this.mNrOfBlobs = 0;
  }

  private struct Vertex
  {
    public const int SIZEINBYTES = 16 /*0x10*/;
    public Vector3 Position;
    public float Instance;
    public static readonly VertexElement[] VertexElements = new VertexElement[2]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.BlendIndices, (byte) 0)
    };
  }

  protected class RenderData : IProjectionObject
  {
    public VertexBuffer mVertices;
    public IndexBuffer mIndices;
    public VertexDeclaration mVertexDeclaration;
    public int mNrOfBlobs;
    public int mVerticesHash;
    public Matrix[] mTransforms = new Matrix[512 /*0x0200*/];
    public Matrix[] mInvTransforms = new Matrix[512 /*0x0200*/];
    public Vector3[] mArgs = new Vector3[512 /*0x0200*/];
    private Matrix[] mBatchTransforms = new Matrix[30];
    private Matrix[] mBatchInvTransforms = new Matrix[30];
    private Vector3[] mBatchArgs = new Vector3[30];

    public int Effect => HardwareInstancedProjectionEffect.TYPEHASH;

    public int Technique => 1;

    public VertexBuffer Vertices => this.mVertices;

    public int VerticesHashCode => this.mVerticesHash;

    public int VertexStride => 16 /*0x10*/;

    public IndexBuffer Indices => this.mIndices;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum) => false;

    public void Draw(Effect iEffect, Texture2D iDepthMap)
    {
      HardwareInstancedProjectionEffect projectionEffect = iEffect as HardwareInstancedProjectionEffect;
      projectionEffect.ShadowIntensity = 0.55f;
      projectionEffect.DepthMap = iDepthMap;
      projectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
      projectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
      projectionEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
      projectionEffect.PixelSize = new Vector2(1f / (float) iDepthMap.Width, 1f / (float) iDepthMap.Height);
      projectionEffect.Texture = (Texture2D) null;
      projectionEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.None;
      projectionEffect.GraphicsDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.None;
      projectionEffect.GraphicsDevice.RenderState.SourceBlend = Blend.Zero;
      for (int sourceIndex = 0; sourceIndex < this.mNrOfBlobs; sourceIndex += 30)
      {
        int length = Math.Min(Math.Min(this.mTransforms.Length - sourceIndex, 30), this.mNrOfBlobs - sourceIndex);
        Array.Copy((Array) this.mTransforms, sourceIndex, (Array) this.mBatchTransforms, 0, length);
        Array.Copy((Array) this.mInvTransforms, sourceIndex, (Array) this.mBatchInvTransforms, 0, length);
        Array.Copy((Array) this.mArgs, sourceIndex, (Array) this.mBatchArgs, 0, length);
        projectionEffect.WorldTransforms = this.mBatchTransforms;
        projectionEffect.InvWorldTransforms = this.mBatchInvTransforms;
        projectionEffect.Args = this.mBatchArgs;
        projectionEffect.CommitChanges();
        projectionEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, length * 8, 0, length * 12);
      }
      projectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
      projectionEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.All;
      projectionEffect.GraphicsDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.All;
      projectionEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
    }
  }
}
