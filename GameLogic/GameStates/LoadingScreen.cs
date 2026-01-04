// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.LoadingScreen
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

#nullable disable
namespace Magicka.GameLogic.GameStates;

internal sealed class LoadingScreen
{
  private const int HEALTHBARSIDESIZE = 96 /*0x60*/;
  private const int HEALTHOFFSET = 16 /*0x10*/;
  private const int HEALTHBARSIZE = 640;
  private const int HEALTHBARHALFSIZE = 320;
  private GraphicsDevice mDevice;
  private ContentManager mContent;
  private GUIBasicEffect mEffect;
  private VertexBuffer mVertexBuffer;
  private VertexDeclaration VertexDeclaration;
  private float mProgress;
  private float mAlpha;
  private Vector3 mHealthColor = new Vector3(1f, 0.0f, 0.0f);
  private Texture2D mHudTexture;
  private Texture2D mBackgroundTexture;
  private Texture2D mImage;
  private bool mShowProgress;
  private bool mWaiting;
  private bool mManagedMode;
  private bool alphaBlend_Saved;
  private Blend destBlend_Saved;
  private Blend sourceBlend_Saved;
  private bool depthBuff_Saved;
  private bool stencil_Saved;
  private RenderTarget renderTarget_Saved;
  private DepthStencilBuffer depthStencilBuffer_Saved;
  private Text mLoadingText;
  private Text mTipText;
  private Vector2 mScreenSize;

  public LoadingScreen(bool iShowProgress, string iTipText)
    : this(iShowProgress, iTipText, false)
  {
  }

  public LoadingScreen(bool iShowProgress, string iTipText, bool managedMode)
  {
    if (this.mDevice == null)
      this.mDevice = Magicka.Game.Instance.GraphicsDevice;
    if (this.mContent == null)
      this.mContent = new ContentManager(Magicka.Game.Instance.Content.ServiceProvider, Magicka.Game.Instance.Content.RootDirectory);
    this.mHudTexture = this.mContent.Load<Texture2D>("UI/HUD/hud");
    this.CreateVertices((float) this.mHudTexture.Width, (float) this.mHudTexture.Height);
    this.Initialize(iShowProgress, iTipText, managedMode);
  }

  public void Initialize(bool iShowProgress, string iTipText, bool managedMode)
  {
    this.mManagedMode = managedMode;
    this.mShowProgress = iShowProgress;
    Point screenSize = RenderManager.Instance.ScreenSize;
    if (this.mHudTexture == null)
      this.mHudTexture = this.mContent.Load<Texture2D>("UI/HUD/hud");
    if (this.mBackgroundTexture == null)
      this.mBackgroundTexture = this.mContent.Load<Texture2D>("UI/Loading/background");
    FileInfo[] files = new DirectoryInfo("content/UI/Loading/images").GetFiles("*.xnb");
    string fullName = files[MagickaMath.Random.Next(files.Length)].FullName;
    this.mImage = this.mContent.Load<Texture2D>(fullName.Substring(0, fullName.Length - 4));
    if (this.mLoadingText == null)
      this.mLoadingText = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuDefault), TextAlign.Center, false);
    if (this.mTipText == null)
      this.mTipText = new Text(512 /*0x0200*/, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, false);
    iTipText = this.mTipText.Font.Wrap(iTipText, 700, true);
    this.mTipText.SetText(iTipText);
    this.mScreenSize.X = (float) screenSize.X;
    this.mScreenSize.Y = (float) screenSize.Y;
    if (managedMode)
    {
      this.alphaBlend_Saved = this.mDevice.RenderState.AlphaBlendEnable;
      this.destBlend_Saved = this.mDevice.RenderState.DestinationBlend;
      this.sourceBlend_Saved = this.mDevice.RenderState.SourceBlend;
      this.depthBuff_Saved = this.mDevice.RenderState.DepthBufferEnable;
      this.stencil_Saved = this.mDevice.RenderState.StencilEnable;
      this.renderTarget_Saved = this.mDevice.GetRenderTarget(0);
      this.depthStencilBuffer_Saved = this.mDevice.DepthStencilBuffer;
      RenderTarget2D renderTarget2D = new RenderTarget2D(this.mDevice, (int) this.mScreenSize.X, (int) this.mScreenSize.Y, 0, SurfaceFormat.Color);
      this.mDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1f, 7);
      this.mDevice.SetRenderTarget(0, (RenderTarget2D) null);
      this.mDevice.DepthStencilBuffer = (DepthStencilBuffer) null;
    }
    this.mDevice.RenderState.DepthBufferEnable = false;
    this.mDevice.RenderState.StencilEnable = false;
    this.mDevice.RenderState.AlphaTestEnable = false;
    this.mDevice.RenderState.SeparateAlphaBlendEnabled = false;
    this.mDevice.RenderState.AlphaBlendEnable = true;
    this.mDevice.RenderState.SourceBlend = Blend.SourceAlpha;
    this.mDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
    this.mEffect = new GUIBasicEffect(this.mDevice, (EffectPool) null);
    this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mEffect.ScaleToHDR = false;
  }

  private void CreateVertices(float iTextureWidth, float iTextureHeight)
  {
    VertexPositionTexture[] data = new VertexPositionTexture[24];
    data[0].Position.X = -0.5f;
    data[0].Position.Y = -0.5f;
    data[0].TextureCoordinate.X = 0.0f;
    data[0].TextureCoordinate.Y = 0.0f;
    data[1].Position.X = 0.5f;
    data[1].Position.Y = -0.5f;
    data[1].TextureCoordinate.X = 1f;
    data[1].TextureCoordinate.Y = 0.0f;
    data[2].Position.X = 0.5f;
    data[2].Position.Y = 0.5f;
    data[2].TextureCoordinate.X = 1f;
    data[2].TextureCoordinate.Y = 1f;
    data[3].Position.X = -0.5f;
    data[3].Position.Y = 0.5f;
    data[3].TextureCoordinate.X = 0.0f;
    data[3].TextureCoordinate.Y = 1f;
    data[4].Position.X = -320f;
    data[4].Position.Y = 12f;
    data[4].TextureCoordinate.X = 0.0f / iTextureWidth;
    data[4].TextureCoordinate.Y = 72f / iTextureHeight;
    data[5].Position.X = -320f;
    data[5].Position.Y = -12f;
    data[5].TextureCoordinate.X = 0.0f / iTextureWidth;
    data[5].TextureCoordinate.Y = 48f / iTextureHeight;
    data[6].Position.X = -224f;
    data[6].Position.Y = 12f;
    data[6].TextureCoordinate.X = 96f / iTextureWidth;
    data[6].TextureCoordinate.Y = 72f / iTextureHeight;
    data[7].Position.X = -224f;
    data[7].Position.Y = -12f;
    data[7].TextureCoordinate.X = 96f / iTextureWidth;
    data[7].TextureCoordinate.Y = 48f / iTextureHeight;
    data[8].Position.X = 224f;
    data[8].Position.Y = 12f;
    data[8].TextureCoordinate.X = 160f / iTextureWidth;
    data[8].TextureCoordinate.Y = 72f / iTextureHeight;
    data[9].Position.X = 224f;
    data[9].Position.Y = -12f;
    data[9].TextureCoordinate.X = 160f / iTextureWidth;
    data[9].TextureCoordinate.Y = 48f / iTextureHeight;
    data[10].Position.X = 320f;
    data[10].Position.Y = 12f;
    data[10].TextureCoordinate.X = 256f / iTextureWidth;
    data[10].TextureCoordinate.Y = 72f / iTextureHeight;
    data[11].Position.X = 320f;
    data[11].Position.Y = -12f;
    data[11].TextureCoordinate.X = 256f / iTextureWidth;
    data[11].TextureCoordinate.Y = 48f / iTextureHeight;
    data[12].Position.X = 0.0f;
    data[12].Position.Y = 12f;
    data[12].TextureCoordinate.X = 76f / iTextureWidth;
    data[12].TextureCoordinate.Y = 96f / iTextureHeight;
    data[13].Position.X = 0.0f;
    data[13].Position.Y = -12f;
    data[13].TextureCoordinate.X = 76f / iTextureWidth;
    data[13].TextureCoordinate.Y = 72f / iTextureHeight;
    data[14].Position.X = 608f;
    data[14].Position.Y = 12f;
    data[14].TextureCoordinate.X = 84f / iTextureWidth;
    data[14].TextureCoordinate.Y = 96f / iTextureHeight;
    data[15].Position.X = 608f;
    data[15].Position.Y = -12f;
    data[15].TextureCoordinate.X = 84f / iTextureWidth;
    data[15].TextureCoordinate.Y = 72f / iTextureHeight;
    data[16 /*0x10*/].Position.X = -320f;
    data[16 /*0x10*/].Position.Y = 12f;
    data[16 /*0x10*/].TextureCoordinate.X = 0.0f / iTextureWidth;
    data[16 /*0x10*/].TextureCoordinate.Y = 48f / iTextureHeight;
    data[17].Position.X = -320f;
    data[17].Position.Y = -12f;
    data[17].TextureCoordinate.X = 0.0f / iTextureWidth;
    data[17].TextureCoordinate.Y = 24f / iTextureHeight;
    data[18].Position.X = -224f;
    data[18].Position.Y = 12f;
    data[18].TextureCoordinate.X = 96f / iTextureWidth;
    data[18].TextureCoordinate.Y = 48f / iTextureHeight;
    data[19].Position.X = -224f;
    data[19].Position.Y = -12f;
    data[19].TextureCoordinate.X = 96f / iTextureWidth;
    data[19].TextureCoordinate.Y = 24f / iTextureHeight;
    data[20].Position.X = 224f;
    data[20].Position.Y = 12f;
    data[20].TextureCoordinate.X = 160f / iTextureWidth;
    data[20].TextureCoordinate.Y = 48f / iTextureHeight;
    data[21].Position.X = 224f;
    data[21].Position.Y = -12f;
    data[21].TextureCoordinate.X = 160f / iTextureWidth;
    data[21].TextureCoordinate.Y = 24f / iTextureHeight;
    data[22].Position.X = 320f;
    data[22].Position.Y = 12f;
    data[22].TextureCoordinate.X = 256f / iTextureWidth;
    data[22].TextureCoordinate.Y = 48f / iTextureHeight;
    data[23].Position.X = 320f;
    data[23].Position.Y = -12f;
    data[23].TextureCoordinate.X = 256f / iTextureWidth;
    data[23].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertexBuffer = new VertexBuffer(this.mDevice, VertexPositionTexture.SizeInBytes * data.Length, BufferUsage.WriteOnly);
    this.mVertexBuffer.SetData<VertexPositionTexture>(data);
    this.VertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
  }

  public void DisplayWaiting()
  {
    this.mLoadingText.SetText(LanguageManager.Instance.GetString("#network_24".GetHashCodeCustom()));
    this.mWaiting = true;
  }

  public float Progress
  {
    get => this.mProgress;
    set => this.mProgress = value;
  }

  public void FadeIn(float iTime)
  {
    Stopwatch stopwatch = Stopwatch.StartNew();
    do
    {
      this.mAlpha = Math.Min((float) stopwatch.Elapsed.TotalSeconds / iTime, 1f);
      this.Draw();
    }
    while ((double) this.mAlpha < 1.0);
  }

  public void FadeOut(float iTime)
  {
    Stopwatch stopwatch = Stopwatch.StartNew();
    do
    {
      this.mAlpha = 1f - Math.Min((float) stopwatch.Elapsed.TotalSeconds / iTime, 1f);
      this.Draw();
    }
    while ((double) this.mAlpha > 0.0);
  }

  public void EndDraw()
  {
    Thread.Sleep(0);
    if (!this.mManagedMode)
      return;
    lock (this.mDevice)
    {
      this.mDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1f, 7);
      this.mDevice.RenderState.AlphaBlendEnable = this.alphaBlend_Saved;
      this.mDevice.RenderState.DestinationBlend = this.destBlend_Saved;
      this.mDevice.RenderState.SourceBlend = this.sourceBlend_Saved;
      this.mDevice.RenderState.DepthBufferEnable = this.depthBuff_Saved;
      this.mDevice.RenderState.StencilEnable = this.stencil_Saved;
      this.mDevice.SetRenderTarget(0, this.renderTarget_Saved as RenderTarget2D);
      this.mDevice.DepthStencilBuffer = this.depthStencilBuffer_Saved;
    }
    Thread.Sleep(0);
  }

  public void Draw()
  {
    this.mDevice.Clear(Color.Black);
    this.mDevice.RenderState.AlphaBlendEnable = true;
    this.mDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
    this.mDevice.RenderState.SourceBlend = Blend.SourceAlpha;
    this.mDevice.RenderState.DepthBufferEnable = false;
    this.mDevice.RenderState.StencilEnable = false;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
    this.mEffect.GraphicsDevice.VertexDeclaration = this.VertexDeclaration;
    Vector4 vector4 = new Vector4();
    vector4.X = 1f;
    vector4.Y = 1f;
    vector4.Z = 1f;
    vector4.W = this.mAlpha;
    this.mEffect.Color = vector4;
    this.mEffect.Texture = (Texture) this.mBackgroundTexture;
    Matrix matrix = new Matrix();
    matrix.M44 = 1f;
    matrix.M11 = this.mScreenSize.Y;
    matrix.M22 = this.mScreenSize.Y;
    matrix.M41 = this.mScreenSize.X * 0.5f;
    matrix.M42 = this.mScreenSize.Y * 0.5f;
    this.mEffect.Transform = matrix;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    if (this.mShowProgress)
    {
      this.mEffect.Texture = (Texture) this.mImage;
      matrix.M42 = (float) ((double) this.mScreenSize.Y * 0.5 - 100.0);
      matrix.M11 = (float) this.mImage.Width;
      matrix.M22 = (float) this.mImage.Height;
      this.mEffect.Transform = matrix;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    }
    if (this.mWaiting)
      this.mLoadingText.Draw(this.mEffect, this.mScreenSize.X * 0.5f, (float) ((double) this.mScreenSize.Y * 0.5 + 100.0));
    else if (this.mShowProgress)
    {
      this.mEffect.Texture = (Texture) this.mHudTexture;
      matrix.M11 = 1f;
      matrix.M22 = 1f;
      matrix.M41 = this.mScreenSize.X * 0.5f;
      matrix.M42 = (float) ((double) this.mScreenSize.Y * 0.5 + 112.0);
      this.mEffect.Transform = matrix;
      vector4.X = 1f;
      vector4.Y = 1f;
      vector4.Z = 1f;
      vector4.W = this.mAlpha * this.mAlpha;
      this.mEffect.Color = vector4;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 4, 6);
      matrix.M11 = this.mProgress;
      matrix.M41 = (float) ((double) this.mScreenSize.X * 0.5 - 320.0 + 16.0);
      this.mEffect.Transform = matrix;
      vector4.X = this.mHealthColor.X;
      vector4.Y = this.mHealthColor.Y;
      vector4.Z = this.mHealthColor.Z;
      this.mEffect.Color = vector4;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 12, 2);
      matrix.M11 = 1f;
      matrix.M22 = 1f;
      matrix.M41 = this.mScreenSize.X * 0.5f;
      this.mEffect.Transform = matrix;
      vector4.X = 1f;
      vector4.Y = 1f;
      vector4.Z = 1f;
      this.mEffect.Color = vector4;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 16 /*0x10*/, 6);
    }
    Vector2 vector2 = this.mTipText.Font.MeasureText(this.mTipText.Characters, true);
    try
    {
      this.mTipText.Draw(this.mEffect, this.mScreenSize.X * 0.5f, this.mScreenSize.Y - (vector2.Y + (float) this.mTipText.Font.LineHeight * 2f));
    }
    catch
    {
    }
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
    this.mDevice.Present();
  }

  public void Dispose()
  {
    this.mEffect.Dispose();
    this.mContent.Dispose();
  }
}
