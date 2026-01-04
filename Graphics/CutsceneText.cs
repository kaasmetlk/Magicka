// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.CutsceneText
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.Graphics;

public class CutsceneText : TextBox
{
  public CutsceneText()
  {
    if (TextBox.sVertexBuffer == null)
    {
      TextBox.sIndexBuffer = new IndexBuffer(Magicka.Game.Instance.GraphicsDevice, 108, BufferUsage.None, IndexElementSize.SixteenBits);
      TextBox.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 16 /*0x10*/ * VertexPositionNormalTexture.SizeInBytes, BufferUsage.None);
      TextBox.sVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        TextBox.sIndexBuffer.SetData<ushort>(TextBox.INDICES);
        TextBox.sVertexBuffer.SetData<VertexPositionNormalTexture>(TextBox.VERTICES);
      }
    }
    this.mText = new TypingText(1024 /*0x0400*/, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, false, 40f);
    this.mText.DefaultColor = Defines.DIALOGUE_COLOR_DEFAULT;
    this.mOwnerName = new PolygonHead.Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, false);
    this.mOwnerName.DefaultColor = Defines.DIALOGUE_COLOR_DEFAULT;
    this.mRenderData = new TextBox.RenderData[3];
    Point screenSize = RenderManager.Instance.ScreenSize;
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mTextBoxEffect = new TextBoxEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
    this.mTextBoxEffect.BorderSize = 32f;
    this.mTextBoxEffect.ScreenSize = new Vector2((float) screenSize.X, (float) screenSize.Y);
    this.mGUIBasicEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mGUIBasicEffect.VertexColorEnabled = true;
    this.mGUIBasicEffect.TextureEnabled = true;
    Texture2D iTexture;
    lock (Magicka.Game.Instance.GraphicsDevice)
      iTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/Dialog_say");
    this.mGUIBasicEffect.Color = Vector4.One;
    for (int index = 0; index < 3; ++index)
    {
      TextBox.RenderData renderData = (TextBox.RenderData) new CutsceneText.CutSceneRenderData(iTexture);
      renderData.mTextBoxEffect = this.mTextBoxEffect;
      renderData.mGUIBasicEffect = this.mGUIBasicEffect;
      renderData.VertexBuffer = TextBox.sVertexBuffer;
      renderData.VertexDeclaration = TextBox.sVertexDeclaration;
      renderData.IndexBuffer = TextBox.sIndexBuffer;
      this.mRenderData[index] = renderData;
    }
    this.mBox = new Rectangle();
  }

  protected override void Initialize(
    Scene iScene,
    MagickaFont iFont,
    string iText,
    Vector2 iMinSize,
    bool iForceOnScreen,
    int iName,
    float iTTL)
  {
    base.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, iName, iTTL);
    this.mBox.Width = 640;
  }

  protected class CutSceneRenderData : TextBox.RenderData
  {
    private Texture2D mTexture;

    public CutSceneRenderData(Texture2D iTexture) => this.mTexture = iTexture;

    public override void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      this.mTextBoxEffect.Color = new Vector4(1f, 1f, 1f, 1f);
      this.mTextBoxEffect.Position = this.mPosition;
      this.mTextBoxEffect.Size = this.mSize;
      this.mTextBoxEffect.Scale = this.mScale;
      this.mTextBoxEffect.Texture = (Texture) this.mTexture;
      this.mTextBoxEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
      this.mTextBoxEffect.GraphicsDevice.Indices = this.mIndexBuffer;
      this.mTextBoxEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mTextBoxEffect.Begin();
      this.mTextBoxEffect.CurrentTechnique.Passes[0].Begin();
      this.mTextBoxEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 18);
      this.mTextBoxEffect.CurrentTechnique.Passes[0].End();
      this.mTextBoxEffect.End();
      this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.mGUIBasicEffect.Begin();
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
      if (this.mShowName)
      {
        this.mOwnerName.Draw(this.mGUIBasicEffect, this.mPosition.X - this.mSize.X * 0.5f * this.mScale, this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale, this.mScale);
        this.mText.Draw(this.mGUIBasicEffect, this.mPosition.X - (float) ((double) this.mSize.X * 0.5 - 20.0) * this.mScale, this.mPosition.Y - (this.mSize.Y * 0.5f - (float) this.mOwnerName.Font.LineHeight) * this.mScale, this.mScale);
      }
      else
        this.mText.Draw(this.mGUIBasicEffect, this.mPosition.X - this.mSize.X * 0.5f * this.mScale, this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale, this.mScale);
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
      this.mGUIBasicEffect.End();
    }

    public override void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      if ((double) this.mScale > 0.99000000953674316)
        this.mText.Update(iDeltaTime);
      Point screenSize = RenderManager.Instance.ScreenSize;
      Vector2 vector2_1 = new Vector2();
      vector2_1.X = (float) screenSize.X;
      vector2_1.Y = (float) screenSize.Y;
      this.mTextBoxEffect.ScreenSize = vector2_1;
      Vector2 vector2_2 = new Vector2();
      vector2_2.Y = (float) Math.Floor((double) vector2_1.Y * (double) PlayState.CUTSCENE_BLACKBAR_SIZE);
      this.mPosition = new Vector2((float) screenSize.X * 0.5f, (float) screenSize.Y * 0.8f);
      this.mPosition.X = Math.Max(this.mPosition.X, (float) (((double) this.mSize.X + 32.0) * 0.5) + vector2_2.X);
      this.mPosition.Y = Math.Max(this.mPosition.Y, (float) (((double) this.mSize.Y + 32.0) * 0.5) + vector2_2.Y);
      this.mPosition.X = Math.Min(this.mPosition.X, vector2_1.X - (float) (((double) this.mSize.X + 32.0) * 0.5) - vector2_2.X);
      this.mPosition.Y = Math.Min(this.mPosition.Y, vector2_1.Y - (float) (((double) this.mSize.Y + 32.0) * 0.5) - vector2_2.Y);
    }
  }
}
