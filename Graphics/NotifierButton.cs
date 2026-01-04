// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.NotifierButton
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.Graphics;

public class NotifierButton
{
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private NotifierButtonEffect mEffect;
  private GUIBasicEffect mGUIEffect;
  private Entity mOwner;
  private TextBox mDialogAttach;
  private float mAlpha;
  private float mTargetAlpha;
  private Text mText;
  private Text mKey;
  private float mWidth;
  private BitmapFont mFont;
  private BitmapFont mKeyFont;
  private NotifierButton.RenderData[] mRenderData;
  private float mIntensity = 1f;
  private float mTargetIntensity = 1f;
  private Vector2 mScreenPosition;

  public NotifierButton()
  {
    VertexPositionTexture[] data = new VertexPositionTexture[8];
    VertexPositionTexture vertexPositionTexture = new VertexPositionTexture();
    vertexPositionTexture.Position.Z = 0.0f;
    vertexPositionTexture.Position.X = 64f;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = 1f;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[0] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 64f;
    vertexPositionTexture.Position.Y = 32f;
    vertexPositionTexture.TextureCoordinate.X = 1f;
    vertexPositionTexture.TextureCoordinate.Y = 1f;
    data[1] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 48f;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = 0.75f;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[2] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 48f;
    vertexPositionTexture.Position.Y = 32f;
    vertexPositionTexture.TextureCoordinate.X = 0.75f;
    vertexPositionTexture.TextureCoordinate.Y = 1f;
    data[3] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 32f;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = 0.5f;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[4] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 32f;
    vertexPositionTexture.Position.Y = 32f;
    vertexPositionTexture.TextureCoordinate.X = 0.5f;
    vertexPositionTexture.TextureCoordinate.Y = 1f;
    data[5] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = 0.0f;
    vertexPositionTexture.TextureCoordinate.X = 0.0f;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[6] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 0.0f;
    vertexPositionTexture.Position.Y = 32f;
    vertexPositionTexture.TextureCoordinate.X = 0.0f;
    vertexPositionTexture.TextureCoordinate.Y = 1f;
    data[7] = vertexPositionTexture;
    this.mVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * 8, BufferUsage.WriteOnly);
    this.mVertices.SetData<VertexPositionTexture>(data);
    this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    this.mEffect = new NotifierButtonEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
    Magicka.Game.Instance.AddLoadTask((Action) (() => this.mEffect.Texture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/NotifierButton")));
    this.mGUIEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    this.mGUIEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
    this.mGUIEffect.TextureEnabled = true;
    this.mGUIEffect.VertexColorEnabled = false;
    this.mRenderData = new NotifierButton.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      NotifierButton.RenderData renderData = new NotifierButton.RenderData(this.mEffect, this.mGUIEffect, this.mVertices, this.mVertexDeclaration);
      this.mRenderData[index] = renderData;
    }
    this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
    this.mKeyFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
    this.mText = new Text(32 /*0x20*/, this.mFont, TextAlign.Left, true);
    this.mKey = new Text(1, this.mKeyFont, TextAlign.Center, true);
  }

  public void Show(string iText, ButtonChar iKey, Entity iOwner)
  {
    this.mText.SetText(iText);
    this.mKey.Characters[0] = (char) iKey;
    this.mKey.MarkAsDirty();
    this.mWidth = this.mFont.MeasureText(this.mText.Characters, true).X;
    this.mOwner = iOwner;
    this.mDialogAttach = (TextBox) null;
    this.mTargetAlpha = 1f;
  }

  public void Show(string iText, ButtonChar iKey, Vector2 iPosition)
  {
    this.mText.SetText(iText);
    this.mKey.Characters[0] = (char) iKey;
    this.mKey.MarkAsDirty();
    this.mWidth = this.mFont.MeasureText(this.mText.Characters, true).X;
    this.mOwner = (Entity) null;
    this.mDialogAttach = (TextBox) null;
    this.mScreenPosition = iPosition;
    this.mTargetAlpha = 1f;
  }

  public void SetButtonIntensity(float iIntensity) => this.mIntensity = iIntensity;

  public void SetButtonTargetIntensity(float iIntensity) => this.mTargetIntensity = iIntensity;

  public float ButtonIntensity => this.mIntensity;

  public void Hide() => this.mTargetAlpha = 0.0f;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mAlpha = (double) this.mTargetAlpha <= 1.4012984643248171E-45 ? Math.Max(this.mAlpha - iDeltaTime * 3f, 0.0f) : Math.Min(this.mAlpha + iDeltaTime * 3f, 1f);
    if ((double) this.mAlpha <= 1.4012984643248171E-45)
      return;
    this.mIntensity = MathHelper.Lerp(this.mTargetIntensity, this.mIntensity, (float) Math.Pow(0.001, (double) iDeltaTime));
    NotifierButton.RenderData iObject = this.mRenderData[(int) iDataChannel];
    if (this.mDialogAttach != null)
    {
      Entity owner = this.mDialogAttach.Owner;
      if (owner != null)
      {
        iObject.m3DPosition = true;
        iObject.mWorldPosition = owner.Position;
        if (owner is Magicka.GameLogic.Entities.Character)
        {
          Magicka.GameLogic.Entities.Character character = owner as Magicka.GameLogic.Entities.Character;
          iObject.mWorldPosition.Y += character.Capsule.Length * 0.5f + character.Capsule.Radius;
        }
        iObject.mOffset.X = (float) (((double) this.mDialogAttach.Width + 64.0) / 3.0 - 90.0);
        iObject.mOffset.Y = -58f;
      }
      else
      {
        iObject.m3DPosition = false;
        iObject.mWorldPosition.X = (float) this.mDialogAttach.Position.X;
        iObject.mWorldPosition.Y = (float) this.mDialogAttach.Position.Y;
      }
    }
    else
    {
      if (this.mOwner != null)
      {
        iObject.mWorldPosition = this.mOwner.Position;
        iObject.m3DPosition = true;
      }
      else
      {
        iObject.m3DPosition = false;
        iObject.mWorldPosition.X = this.mScreenPosition.X;
        iObject.mWorldPosition.Y = this.mScreenPosition.Y;
      }
      iObject.mOffset.X = -42f;
      iObject.mOffset.Y = 20f;
    }
    iObject.mButtonIntensity = this.mIntensity;
    iObject.mText = this.mText;
    iObject.mAlpha = this.mAlpha;
    iObject.mWidth = this.mWidth;
    iObject.mKey = this.mKey;
    if (!KeyboardHUD.Instance.UIEnabled)
      return;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
  {
    protected NotifierButtonEffect mEffect;
    protected GUIBasicEffect mGUIEffect;
    protected VertexBuffer mVertexBuffer;
    protected VertexDeclaration mVertexDeclaration;
    public float mAlpha;
    public bool m3DPosition;
    public Vector3 mWorldPosition;
    public Vector2 mOffset;
    private Vector2 mPosition;
    public float mWidth;
    public Text mText;
    public Text mKey;
    public float mButtonIntensity;

    public RenderData(
      NotifierButtonEffect iEffect,
      GUIBasicEffect iGUIEffect,
      VertexBuffer iVertexBuffer,
      VertexDeclaration iVertexDeclaration)
    {
      this.mEffect = iEffect;
      this.mGUIEffect = iGUIEffect;
      this.mVertexBuffer = iVertexBuffer;
      this.mVertexDeclaration = iVertexDeclaration;
    }

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      Vector4 vector4 = new Vector4();
      vector4.X = vector4.Y = vector4.Z = 1f;
      vector4.W = this.mAlpha;
      this.mEffect.ScreenSize = new Vector2()
      {
        X = (float) screenSize.X,
        Y = (float) screenSize.Y
      };
      this.mGUIEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.mEffect.Color = vector4;
      this.mEffect.Position = this.mPosition;
      this.mEffect.Width = this.mWidth;
      this.mEffect.Scale = Vector2.One;
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      if (this.mKey.Characters[0] == '̰')
      {
        this.mEffect.Width = this.mWidth - 34f;
        this.mEffect.Texture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/NotifierButton_Keyboard");
      }
      else
        this.mEffect.Texture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/NotifierButton");
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 6);
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
      vector4.X = vector4.Y = vector4.Z = this.mButtonIntensity;
      this.mGUIEffect.Color = vector4;
      this.mGUIEffect.Begin();
      this.mGUIEffect.CurrentTechnique.Passes[0].Begin();
      if (this.mKey.Characters[0] == '̰')
      {
        vector4.X = vector4.Y = vector4.Z = 1f;
        this.mGUIEffect.Color = vector4;
        this.mText.Draw(this.mGUIEffect, this.mPosition.X + 8f, this.mPosition.Y + 8f);
      }
      else
      {
        this.mKey.Draw(this.mGUIEffect, this.mPosition.X + 16f, this.mPosition.Y + 3f);
        vector4.X = vector4.Y = vector4.Z = 1f;
        this.mGUIEffect.Color = vector4;
        this.mText.Draw(this.mGUIEffect, this.mPosition.X + 34f, this.mPosition.Y + 8f);
      }
      this.mGUIEffect.CurrentTechnique.Passes[0].End();
      this.mGUIEffect.End();
    }

    public int ZIndex => 100;

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      if (this.m3DPosition)
      {
        this.mPosition = MagickaMath.WorldToScreenPosition(ref this.mWorldPosition, ref iViewProjectionMatrix);
      }
      else
      {
        this.mPosition.X = this.mWorldPosition.X;
        this.mPosition.Y = this.mWorldPosition.Y;
      }
      Vector2.Add(ref this.mPosition, ref this.mOffset, out this.mPosition);
    }
  }
}
