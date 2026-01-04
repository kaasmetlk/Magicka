// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Healthbars
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

public class Healthbars
{
  private static Healthbars mSingelton;
  private static volatile object mSingeltonLock = new object();
  private Healthbars.RenderData[] mRenderData;
  private GUIHardwareInstancingEffect mGUIHardwareInstancingEffect;
  private IndexBuffer mIndexBuffer;
  private VertexBuffer mVertexBuffer;
  private VertexDeclaration mVertexDeclaration;
  private static readonly Vector2 BARSIZE = new Vector2(64f, 8f);
  private Vector3[] mPositions = new Vector3[512 /*0x0200*/];
  private Vector3[] mScales = new Vector3[512 /*0x0200*/];
  private Vector2[] mOffsets = new Vector2[512 /*0x0200*/];
  private Vector4[] mColors = new Vector4[512 /*0x0200*/];
  private float[] mLength = new float[512 /*0x0200*/];
  private int mHealtbarsToDraw;
  private bool mUIEnabled = true;

  public static Healthbars Instance
  {
    get
    {
      if (Healthbars.mSingelton == null)
      {
        lock (Healthbars.mSingeltonLock)
        {
          if (Healthbars.mSingelton == null)
            Healthbars.mSingelton = new Healthbars();
        }
      }
      return Healthbars.mSingelton;
    }
  }

  public Healthbars()
  {
    Texture2D texture2D;
    lock (Magicka.Game.Instance.GraphicsDevice)
      texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Hud/Hud");
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mGUIHardwareInstancingEffect = new GUIHardwareInstancingEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
    this.mGUIHardwareInstancingEffect.SetTechnique(GUIHardwareInstancingEffect.Technique.Healthbars);
    this.mGUIHardwareInstancingEffect.ScreenSize = screenSize;
    this.mGUIHardwareInstancingEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mGUIHardwareInstancingEffect.Texture = (Texture) texture2D;
    float num1 = 1f / (float) texture2D.Height;
    float num2 = 1f / (float) texture2D.Width;
    Vector2 vector2_1 = new Vector2(16f * num2, 73f * num1);
    Vector2 vector2_2 = new Vector2(8f * num2, 6f * num1);
    Vector2 vector2_3 = new Vector2(6f * num2, 73f * num1);
    Vector2 vector2_4 = new Vector2(32f * num2, 73f * num1);
    Vector2 vector2_5 = new Vector2(2f * num2, 6f * num1);
    Vector2 vector2_6 = new Vector2(2f * num2, 81f * num1);
    Vector2 vector2_7 = new Vector2(4f * num2, 6f * num1);
    Healthbars.Vertex[] data1 = new Healthbars.Vertex[8192 /*0x2000*/];
    int[] data2 = new int[12288 /*0x3000*/];
    Healthbars.Vertex vertex = new Healthbars.Vertex();
    for (int index1 = 0; index1 < 512 /*0x0200*/; ++index1)
    {
      int index2 = index1 * 16 /*0x10*/;
      vertex.Index = (float) (index1 % 512 /*0x0200*/);
      vertex.Layer = 1f;
      vertex.Position.X = -4f;
      vertex.Position.Y = Healthbars.BARSIZE.Y;
      vertex.TexCoord.X = vector2_3.X;
      vertex.TexCoord.Y = vector2_3.Y + vector2_5.Y;
      data1[index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = -4f;
      vertex.Position.Y = 0.0f;
      vertex.TexCoord.X = vector2_3.X;
      vertex.TexCoord.Y = vector2_3.Y;
      data1[1 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = 0.0f;
      vertex.Position.Y = 0.0f;
      vertex.TexCoord.X = vector2_3.X + vector2_5.X;
      vertex.TexCoord.Y = vector2_3.Y;
      data1[2 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = 0.0f;
      vertex.Position.Y = Healthbars.BARSIZE.Y;
      vertex.TexCoord.X = vector2_3.X + vector2_5.X;
      vertex.TexCoord.Y = vector2_3.Y + vector2_5.Y;
      data1[3 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = Healthbars.BARSIZE.X;
      vertex.Position.Y = Healthbars.BARSIZE.Y;
      vertex.TexCoord.X = vector2_4.X;
      vertex.TexCoord.Y = vector2_4.Y + vector2_5.Y;
      data1[4 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = Healthbars.BARSIZE.X;
      vertex.Position.Y = 0.0f;
      vertex.TexCoord.X = vector2_4.X;
      vertex.TexCoord.Y = vector2_4.Y;
      data1[5 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = Healthbars.BARSIZE.X + 4f;
      vertex.Position.Y = 0.0f;
      vertex.TexCoord.X = vector2_4.X + vector2_5.X;
      vertex.TexCoord.Y = vector2_4.Y;
      data1[6 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = Healthbars.BARSIZE.X + 4f;
      vertex.Position.Y = Healthbars.BARSIZE.Y;
      vertex.TexCoord.X = vector2_4.X + vector2_5.X;
      vertex.TexCoord.Y = vector2_4.Y + vector2_5.Y;
      data1[7 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = 0.0f;
      vertex.Position.Y = Healthbars.BARSIZE.Y;
      vertex.TexCoord.X = vector2_1.X;
      vertex.TexCoord.Y = vector2_1.Y + vector2_2.Y;
      data1[8 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = 0.0f;
      vertex.Position.Y = 0.0f;
      vertex.TexCoord.X = vector2_1.X;
      vertex.TexCoord.Y = vector2_1.Y;
      data1[9 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = Healthbars.BARSIZE.X;
      vertex.Position.Y = 0.0f;
      vertex.TexCoord.X = vector2_1.X + vector2_2.X;
      vertex.TexCoord.Y = vector2_1.Y;
      data1[10 + index2] = vertex;
      vertex.Layer = 1f;
      vertex.Position.X = Healthbars.BARSIZE.X;
      vertex.Position.Y = Healthbars.BARSIZE.Y;
      vertex.TexCoord.X = vector2_1.X + vector2_2.X;
      vertex.TexCoord.Y = vector2_1.Y + vector2_2.Y;
      data1[11 + index2] = vertex;
      vertex.Layer = 0.0f;
      vertex.Position.X = 0.0f;
      vertex.Position.Y = Healthbars.BARSIZE.Y;
      vertex.TexCoord.X = vector2_6.X;
      vertex.TexCoord.Y = vector2_6.Y + vector2_7.Y;
      data1[12 + index2] = vertex;
      vertex.Layer = 0.0f;
      vertex.Position.X = 0.0f;
      vertex.Position.Y = 0.0f;
      vertex.TexCoord.X = vector2_6.X;
      vertex.TexCoord.Y = vector2_6.Y;
      data1[13 + index2] = vertex;
      vertex.Layer = 0.0f;
      vertex.Position.X = Healthbars.BARSIZE.X;
      vertex.Position.Y = 0.0f;
      vertex.TexCoord.X = vector2_6.X + vector2_7.X;
      vertex.TexCoord.Y = vector2_6.Y;
      data1[14 + index2] = vertex;
      vertex.Layer = 0.0f;
      vertex.Position.X = Healthbars.BARSIZE.X;
      vertex.Position.Y = Healthbars.BARSIZE.Y;
      vertex.TexCoord.X = vector2_6.X + vector2_7.X;
      vertex.TexCoord.Y = vector2_6.Y + vector2_7.Y;
      data1[15 + index2] = vertex;
      int index3 = index1 * 24;
      data2[index3] = index2;
      data2[1 + index3] = index2 + 1;
      data2[2 + index3] = index2 + 2;
      data2[3 + index3] = index2 + 2;
      data2[4 + index3] = index2 + 3;
      data2[5 + index3] = index2;
      data2[6 + index3] = index2 + 4;
      data2[7 + index3] = index2 + 1 + 4;
      data2[8 + index3] = index2 + 2 + 4;
      data2[9 + index3] = index2 + 2 + 4;
      data2[10 + index3] = index2 + 3 + 4;
      data2[11 + index3] = index2 + 4;
      data2[12 + index3] = index2 + 8;
      data2[13 + index3] = index2 + 1 + 8;
      data2[14 + index3] = index2 + 2 + 8;
      data2[15 + index3] = index2 + 2 + 8;
      data2[16 /*0x10*/ + index3] = index2 + 3 + 8;
      data2[17 + index3] = index2 + 8;
      data2[18 + index3] = index2 + 12;
      data2[19 + index3] = index2 + 1 + 12;
      data2[20 + index3] = index2 + 2 + 12;
      data2[21 + index3] = index2 + 2 + 12;
      data2[22 + index3] = index2 + 3 + 12;
      data2[23 + index3] = index2 + 12;
    }
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 229376 /*0x038000*/, BufferUsage.WriteOnly);
      this.mVertexBuffer.SetData<Healthbars.Vertex>(data1);
      this.mIndexBuffer = new IndexBuffer(Magicka.Game.Instance.GraphicsDevice, 49152 /*0xC000*/, BufferUsage.WriteOnly, IndexElementSize.ThirtyTwoBits);
      this.mIndexBuffer.SetData<int>(data2);
      this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, Healthbars.Vertex.VertexElements);
    }
    for (int index = 0; index < 512 /*0x0200*/; ++index)
    {
      this.mPositions[index] = new Vector3(0.0f);
      this.mColors[index] = new Vector4();
      this.mScales[index] = new Vector3(1f);
      this.mLength[index] = 72f;
    }
    this.mRenderData = new Healthbars.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      Healthbars.RenderData renderData = new Healthbars.RenderData();
      this.mRenderData[index] = renderData;
      renderData.mIndexBuffer = this.mIndexBuffer;
      renderData.mVertexBuffer = this.mVertexBuffer;
      renderData.mVertexDeclaration = this.mVertexDeclaration;
      renderData.mEffect = this.mGUIHardwareInstancingEffect;
    }
  }

  public void AddHealthBar(
    Vector3 iPosition,
    float iNormalizedHitPoints,
    float iWidth,
    float iHeight,
    float iTimeSinceLastDamage,
    bool iFadeWithoutDamage,
    Vector4? iColor,
    Vector2? iOffset)
  {
    if (!iColor.HasValue)
      iColor = new Vector4?(new Vector4(1f, 0.0f, 0.0f, 1f));
    float num1 = MathHelper.Clamp(0.3f - iTimeSinceLastDamage, 0.0f, 0.3f) * 4f;
    float num2 = !iFadeWithoutDamage ? 1f : MathHelper.Clamp(MathHelper.Lerp(0.0f, 1f, 3f - iTimeSinceLastDamage), 0.0f, 1f);
    if (this.mHealtbarsToDraw >= 512 /*0x0200*/ || (double) num2 <= 0.0)
      return;
    this.mPositions[this.mHealtbarsToDraw] = iPosition;
    this.mColors[this.mHealtbarsToDraw] = iColor.Value + new Vector4(num1, num1, num1, 0.0f);
    this.mColors[this.mHealtbarsToDraw].W *= num2;
    this.mScales[this.mHealtbarsToDraw].X = MathHelper.Clamp(iNormalizedHitPoints, 0.0f, 1f);
    this.mScales[this.mHealtbarsToDraw].Y = MathHelper.Min(iWidth, 1.5f);
    this.mScales[this.mHealtbarsToDraw].Z = iHeight;
    this.mOffsets[this.mHealtbarsToDraw] = iOffset.GetValueOrDefault();
    ++this.mHealtbarsToDraw;
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    Healthbars.RenderData iObject = this.mRenderData[(int) iDataChannel];
    this.mPositions.CopyTo((Array) iObject.mWorldPositions, 0);
    this.mColors.CopyTo((Array) iObject.mColors, 0);
    this.mScales.CopyTo((Array) iObject.mScales, 0);
    this.mOffsets.CopyTo((Array) iObject.mOffsets, 0);
    iObject.mHealthbarsToDraw = this.mHealtbarsToDraw;
    this.mHealtbarsToDraw = 0;
    if (!this.mUIEnabled)
      return;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public bool UIEnabled
  {
    get => this.mUIEnabled;
    set => this.mUIEnabled = value;
  }

  protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
  {
    public GUIHardwareInstancingEffect mEffect;
    public VertexDeclaration mVertexDeclaration;
    public VertexBuffer mVertexBuffer;
    public IndexBuffer mIndexBuffer;
    public Vector3[] mWorldPositions = new Vector3[512 /*0x0200*/];
    public Vector3[] mScales = new Vector3[512 /*0x0200*/];
    public Vector4[] mColors = new Vector4[512 /*0x0200*/];
    public Vector2[] mOffsets = new Vector2[512 /*0x0200*/];
    public int mHealthbarsToDraw;
    private Vector3[] mBatchPositions = new Vector3[40];
    private Vector3[] mBatchScales = new Vector3[40];
    private Vector4[] mBatchColors = new Vector4[40];
    private Vector2[] mBatchOffsets = new Vector2[40];

    public void Draw(float iDeltaTime)
    {
      if (this.mHealthbarsToDraw <= 0)
        return;
      this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
      this.mEffect.GraphicsDevice.Indices = this.mIndexBuffer;
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 28);
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      for (int sourceIndex = 0; sourceIndex < this.mHealthbarsToDraw; sourceIndex += 40)
      {
        int length = this.mWorldPositions.Length - sourceIndex;
        if (length > 40)
          length = 40;
        if (length > this.mHealthbarsToDraw)
          length = this.mHealthbarsToDraw;
        Array.Copy((Array) this.mColors, sourceIndex, (Array) this.mBatchColors, 0, length);
        Array.Copy((Array) this.mWorldPositions, sourceIndex, (Array) this.mBatchPositions, 0, length);
        Array.Copy((Array) this.mScales, sourceIndex, (Array) this.mBatchScales, 0, length);
        Array.Copy((Array) this.mOffsets, sourceIndex, (Array) this.mBatchOffsets, 0, length);
        this.mEffect.Colors = this.mBatchColors;
        this.mEffect.WorldPositions = this.mBatchPositions;
        this.mEffect.Scales = this.mBatchScales;
        this.mEffect.TextureOffsets = this.mBatchOffsets;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, sourceIndex * 16 /*0x10*/, length * 16 /*0x10*/, sourceIndex * 24, length * 8);
      }
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }

    public int ZIndex => 98;

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      this.mEffect.ScreenSize = RenderManager.Instance.ScreenSize;
      this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
      this.mEffect.WorldToScreen = iViewProjectionMatrix;
    }
  }

  private struct Vertex
  {
    public const int SIZEINBYTES = 28;
    public Vector3 Position;
    public Vector2 TexCoord;
    public float Index;
    public float Layer;
    public static readonly VertexElement[] VertexElements = new VertexElement[4]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
      new VertexElement((short) 0, (short) 20, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 1),
      new VertexElement((short) 0, (short) 24, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 2)
    };
  }
}
