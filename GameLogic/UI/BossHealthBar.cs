// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.BossHealthBar
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

public class BossHealthBar
{
  public const int HEALTHBARSIDESIZE = 96 /*0x60*/;
  public const int HEALTHOFFSET = 16 /*0x10*/;
  private GUIBasicEffect mEffect;
  private VertexBuffer mVertexBuffer;
  private VertexDeclaration mVertexDeclaration;
  private VertexPositionTexture[] mVertices = new VertexPositionTexture[20];
  private Scene mScene;
  private BossHealthBar.RenderData[] mRenderData;
  private bool mDestroy;
  private float mAlpha;
  private float mPower;
  private float mNormalizedHealth;
  private float mDisplayHealth;
  private Texture2D mTexture;

  public BossHealthBar(Scene iScene)
  {
    this.mScene = iScene;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
    {
      this.mEffect = new GUIBasicEffect(graphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
      this.mTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
      this.mEffect.Texture = (Texture) this.mTexture;
    }
    this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
    this.mEffect.TextureEnabled = true;
    this.mRenderData = new BossHealthBar.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      BossHealthBar.RenderData renderData = new BossHealthBar.RenderData();
      this.mRenderData[index] = renderData;
      renderData.mEffect = this.mEffect;
    }
    this.mVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * this.mVertices.Length, BufferUsage.WriteOnly);
    this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    this.CreateVertices(0.8f, (float) this.mTexture.Width, (float) this.mTexture.Height);
    this.mAlpha = 0.0f;
    this.mPower = 1f;
    this.mDisplayHealth = 0.0f;
    this.mNormalizedHealth = 1f;
    this.mDestroy = false;
  }

  public Scene Scene
  {
    get => this.mScene;
    set => this.mScene = value;
  }

  public void Reset()
  {
    this.mPower = 1f;
    this.mAlpha = 0.0f;
    this.mDisplayHealth = 0.0f;
    this.mNormalizedHealth = 1f;
    this.mDestroy = false;
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (iDataChannel == DataChannel.None)
      return;
    BossHealthBar.RenderData iObject = this.mRenderData[(int) iDataChannel];
    this.mAlpha = !this.mDestroy ? Math.Min(this.mAlpha + iDeltaTime * 0.5f, 1f) : Math.Max(this.mAlpha - iDeltaTime * 0.5f, 0.0f);
    if ((double) this.mAlpha > 0.99000000953674316 || this.mDestroy)
      this.mDisplayHealth += (float) (((double) this.mNormalizedHealth - (double) this.mDisplayHealth) * 10.0) * iDeltaTime;
    iObject.mAlpha = this.mAlpha;
    iObject.mNormalizedHealth = this.mDisplayHealth;
    iObject.mHealthColor = new Vector3(this.mPower, 0.0f, 0.0f);
    this.mScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public void SetWidth(float iHealthbarWidth)
  {
    this.CreateVertices(iHealthbarWidth, (float) this.mTexture.Width, (float) this.mTexture.Height);
  }

  protected void CreateVertices(float iHealthbarWidth, float iTextureWidth, float iTextureHeight)
  {
    int x = RenderManager.Instance.ScreenSize.X;
    int num1 = x / 2;
    int num2 = (int) ((double) iHealthbarWidth * (double) x);
    int num3 = num2 / 2;
    this.mVertices[0].Position.X = (float) (num1 - num3);
    this.mVertices[0].Position.Y = 32f;
    this.mVertices[0].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[0].TextureCoordinate.Y = 72f / iTextureHeight;
    this.mVertices[1].Position.X = (float) (num1 - num3);
    this.mVertices[1].Position.Y = 8f;
    this.mVertices[1].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[1].TextureCoordinate.Y = 48f / iTextureHeight;
    this.mVertices[2].Position.X = (float) (num1 - num3 + 96 /*0x60*/);
    this.mVertices[2].Position.Y = 32f;
    this.mVertices[2].TextureCoordinate.X = 96f / iTextureWidth;
    this.mVertices[2].TextureCoordinate.Y = 72f / iTextureHeight;
    this.mVertices[3].Position.X = (float) (num1 - num3 + 96 /*0x60*/);
    this.mVertices[3].Position.Y = 8f;
    this.mVertices[3].TextureCoordinate.X = 96f / iTextureWidth;
    this.mVertices[3].TextureCoordinate.Y = 48f / iTextureHeight;
    this.mVertices[4].Position.X = (float) (num1 + num3 - 96 /*0x60*/);
    this.mVertices[4].Position.Y = 32f;
    this.mVertices[4].TextureCoordinate.X = 160f / iTextureWidth;
    this.mVertices[4].TextureCoordinate.Y = 72f / iTextureHeight;
    this.mVertices[5].Position.X = (float) (num1 + num3 - 96 /*0x60*/);
    this.mVertices[5].Position.Y = 8f;
    this.mVertices[5].TextureCoordinate.X = 160f / iTextureWidth;
    this.mVertices[5].TextureCoordinate.Y = 48f / iTextureHeight;
    this.mVertices[6].Position.X = (float) (num1 + num3);
    this.mVertices[6].Position.Y = 32f;
    this.mVertices[6].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[6].TextureCoordinate.Y = 72f / iTextureHeight;
    this.mVertices[7].Position.X = (float) (num1 + num3);
    this.mVertices[7].Position.Y = 8f;
    this.mVertices[7].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[7].TextureCoordinate.Y = 48f / iTextureHeight;
    this.mVertices[8].Position.X = 0.0f;
    this.mVertices[8].Position.Y = 32f;
    this.mVertices[8].TextureCoordinate.X = 76f / iTextureWidth;
    this.mVertices[8].TextureCoordinate.Y = 96f / iTextureHeight;
    this.mVertices[9].Position.X = 0.0f;
    this.mVertices[9].Position.Y = 8f;
    this.mVertices[9].TextureCoordinate.X = 76f / iTextureWidth;
    this.mVertices[9].TextureCoordinate.Y = 72f / iTextureHeight;
    this.mVertices[10].Position.X = (float) (num2 - 32 /*0x20*/);
    this.mVertices[10].Position.Y = 32f;
    this.mVertices[10].TextureCoordinate.X = 84f / iTextureWidth;
    this.mVertices[10].TextureCoordinate.Y = 96f / iTextureHeight;
    this.mVertices[11].Position.X = (float) (num2 - 32 /*0x20*/);
    this.mVertices[11].Position.Y = 8f;
    this.mVertices[11].TextureCoordinate.X = 84f / iTextureWidth;
    this.mVertices[11].TextureCoordinate.Y = 72f / iTextureHeight;
    this.mVertices[12].Position.X = (float) (num1 - num3);
    this.mVertices[12].Position.Y = 32f;
    this.mVertices[12].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[12].TextureCoordinate.Y = 48f / iTextureHeight;
    this.mVertices[13].Position.X = (float) (num1 - num3);
    this.mVertices[13].Position.Y = 8f;
    this.mVertices[13].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[13].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[14].Position.X = (float) (num1 - num3 + 96 /*0x60*/);
    this.mVertices[14].Position.Y = 32f;
    this.mVertices[14].TextureCoordinate.X = 96f / iTextureWidth;
    this.mVertices[14].TextureCoordinate.Y = 48f / iTextureHeight;
    this.mVertices[15].Position.X = (float) (num1 - num3 + 96 /*0x60*/);
    this.mVertices[15].Position.Y = 8f;
    this.mVertices[15].TextureCoordinate.X = 96f / iTextureWidth;
    this.mVertices[15].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[16 /*0x10*/].Position.X = (float) (num1 + num3 - 96 /*0x60*/);
    this.mVertices[16 /*0x10*/].Position.Y = 32f;
    this.mVertices[16 /*0x10*/].TextureCoordinate.X = 160f / iTextureWidth;
    this.mVertices[16 /*0x10*/].TextureCoordinate.Y = 48f / iTextureHeight;
    this.mVertices[17].Position.X = (float) (num1 + num3 - 96 /*0x60*/);
    this.mVertices[17].Position.Y = 8f;
    this.mVertices[17].TextureCoordinate.X = 160f / iTextureWidth;
    this.mVertices[17].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[18].Position.X = (float) (num1 + num3);
    this.mVertices[18].Position.Y = 32f;
    this.mVertices[18].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[18].TextureCoordinate.Y = 48f / iTextureHeight;
    this.mVertices[19].Position.X = (float) (num1 + num3);
    this.mVertices[19].Position.Y = 8f;
    this.mVertices[19].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[19].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertexBuffer.SetData<VertexPositionTexture>(this.mVertices);
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].mVertices = this.mVertexBuffer;
      this.mRenderData[index].mVertexDeclaration = this.mVertexDeclaration;
      this.mRenderData[index].mHealthBarPosition = (float) (num1 - num3 + 16 /*0x10*/);
    }
  }

  public bool Destroy
  {
    get => this.mDestroy;
    set => this.mDestroy = value;
  }

  public float Alpha => this.mAlpha;

  public float Power
  {
    get => this.mPower;
    set => this.mPower = value;
  }

  public void SetNormalizedHealth(float iPercent) => this.mNormalizedHealth = iPercent;

  protected class RenderData : IRenderableGUIObject
  {
    public GUIBasicEffect mEffect;
    public VertexBuffer mVertices;
    public VertexDeclaration mVertexDeclaration;
    public float mAlpha;
    public float mNormalizedHealth;
    public Vector3 mHealthColor;
    public float mHealthBarPosition;

    public void Draw(float iDeltaTime)
    {
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, VertexPositionTexture.SizeInBytes);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
      this.mEffect.Begin();
      EffectPassCollection passes = this.mEffect.CurrentTechnique.Passes;
      for (int index = 0; index < passes.Count; ++index)
      {
        passes[index].Begin();
        this.mEffect.Transform = Matrix.Identity;
        Vector4 vector4 = new Vector4();
        vector4.X = 1f;
        vector4.Y = 1f;
        vector4.Z = 1f;
        vector4.W = this.mAlpha;
        this.mEffect.Color = vector4;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 6);
        this.mEffect.Transform = new Matrix()
        {
          M11 = this.mNormalizedHealth,
          M41 = this.mHealthBarPosition,
          M22 = 1f,
          M33 = 1f,
          M44 = 1f
        };
        vector4.X = this.mHealthColor.X;
        vector4.Y = this.mHealthColor.Y;
        vector4.Z = this.mHealthColor.Z;
        vector4.W = this.mAlpha;
        this.mEffect.Color = vector4;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 8, 2);
        this.mEffect.Transform = Matrix.Identity;
        vector4.X = 1f;
        vector4.Y = 1f;
        vector4.Z = 1f;
        vector4.W = this.mAlpha;
        this.mEffect.Color = vector4;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 12, 6);
        passes[index].End();
      }
      this.mEffect.End();
    }

    public int ZIndex => 200;
  }
}
