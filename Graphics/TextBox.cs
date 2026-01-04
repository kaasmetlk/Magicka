// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.TextBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.UI;
using Magicka.Graphics.Effects;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.Graphics;

public class TextBox
{
  public const float BORDERSIZE = 32f;
  public const int INDENTATION = 20;
  public static readonly ushort[] INDICES = new ushort[54]
  {
    (ushort) 0,
    (ushort) 1,
    (ushort) 4,
    (ushort) 1,
    (ushort) 5,
    (ushort) 4,
    (ushort) 1,
    (ushort) 2,
    (ushort) 5,
    (ushort) 2,
    (ushort) 6,
    (ushort) 5,
    (ushort) 2,
    (ushort) 3,
    (ushort) 6,
    (ushort) 3,
    (ushort) 7,
    (ushort) 6,
    (ushort) 4,
    (ushort) 5,
    (ushort) 8,
    (ushort) 5,
    (ushort) 9,
    (ushort) 8,
    (ushort) 5,
    (ushort) 6,
    (ushort) 9,
    (ushort) 6,
    (ushort) 10,
    (ushort) 9,
    (ushort) 6,
    (ushort) 7,
    (ushort) 10,
    (ushort) 7,
    (ushort) 11,
    (ushort) 10,
    (ushort) 8,
    (ushort) 9,
    (ushort) 12,
    (ushort) 9,
    (ushort) 13,
    (ushort) 12,
    (ushort) 9,
    (ushort) 10,
    (ushort) 13,
    (ushort) 10,
    (ushort) 14,
    (ushort) 13,
    (ushort) 10,
    (ushort) 11,
    (ushort) 14,
    (ushort) 11,
    (ushort) 15,
    (ushort) 14
  };
  public static readonly VertexPositionNormalTexture[] VERTICES = new VertexPositionNormalTexture[16 /*0x10*/]
  {
    new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(-1f, -1f, 0.0f), new Vector2(0.0f, 0.0f)),
    new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0.0f, -1f, 0.0f), new Vector2(0.25f, 0.0f)),
    new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector3(0.0f, -1f, 0.0f), new Vector2(0.75f, 0.0f)),
    new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector3(1f, -1f, 0.0f), new Vector2(1f, 0.0f)),
    new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(-1f, 0.0f, 0.0f), new Vector2(0.0f, 0.25f)),
    new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector2(0.25f, 0.25f)),
    new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector2(0.75f, 0.25f)),
    new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector3(1f, 0.0f, 0.0f), new Vector2(1f, 0.25f)),
    new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(-1f, 0.0f, 0.0f), new Vector2(0.0f, 0.75f)),
    new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector2(0.25f, 0.75f)),
    new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector2(0.75f, 0.75f)),
    new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.0f), new Vector3(1f, 0.0f, 0.0f), new Vector2(1f, 0.75f)),
    new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(-1f, 1f, 0.0f), new Vector2(0.0f, 1f)),
    new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(0.0f, 1f, 0.0f), new Vector2(0.25f, 1f)),
    new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.0f), new Vector3(0.0f, 1f, 0.0f), new Vector2(0.75f, 1f)),
    new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.0f), new Vector3(1f, 1f, 0.0f), new Vector2(1f, 1f))
  };
  protected string mString;
  protected Rectangle mBox;
  protected PolygonHead.Text mOwnerName;
  protected bool mShowName;
  protected TypingText mText;
  protected TextBoxEffect mTextBoxEffect;
  protected GUIBasicEffect mGUIBasicEffect;
  protected static IndexBuffer sIndexBuffer;
  protected static VertexBuffer sVertexBuffer;
  protected static VertexDeclaration sVertexDeclaration;
  protected TextBox.RenderData[] mRenderData;
  protected Scene mScene;
  protected Entity mOwner;
  protected Vector3 mWorldPos;
  protected bool mIsScreenPos;
  protected bool mForceScreen;
  private float mScale;
  private bool mGrow;
  private float mTTL;
  private bool mAutomaticAdvance;

  public TextBox()
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
    this.mText = new TypingText(1024 /*0x0400*/, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Left, false, 40f);
    this.mText.DefaultColor = Defines.DIALOGUE_COLOR_DEFAULT;
    this.mOwnerName = new PolygonHead.Text(64 /*0x40*/, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, false);
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
    this.mGUIBasicEffect.Color = Vector4.One;
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new TextBox.RenderData()
      {
        mTextBoxEffect = this.mTextBoxEffect,
        mGUIBasicEffect = this.mGUIBasicEffect,
        VertexBuffer = TextBox.sVertexBuffer,
        VertexDeclaration = TextBox.sVertexDeclaration,
        IndexBuffer = TextBox.sIndexBuffer
      };
    this.mBox = new Rectangle();
  }

  public static void GetRenderBuffers(
    out VertexBuffer oVertexBuffer,
    out VertexDeclaration oVertexDeclaration,
    out IndexBuffer oIndexBuffer,
    out int oVertexStride)
  {
    oVertexBuffer = TextBox.sVertexBuffer;
    oVertexDeclaration = TextBox.sVertexDeclaration;
    oIndexBuffer = TextBox.sIndexBuffer;
    oVertexStride = VertexPositionNormalTexture.SizeInBytes;
  }

  public Entity Owner => this.mOwner;

  public virtual void Initialize(
    Scene iScene,
    MagickaFont iFont,
    string iText,
    Vector2 iMinSize,
    bool iForceOnScreen,
    Entity iOwner,
    float iTTL)
  {
    if (iScene == null)
      throw new ArgumentNullException(nameof (iScene));
    this.mOwner = iOwner;
    this.mIsScreenPos = false;
    if (iOwner != null)
      this.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, iOwner.UniqueID, iTTL);
    else
      this.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, 0, iTTL);
  }

  public virtual void Initialize(
    Scene iScene,
    MagickaFont iFont,
    string iText,
    Vector2 iMinSize,
    Vector3 iWorldPosition,
    bool iForceOnScreen,
    int iName,
    float iTTL)
  {
    if (iScene == null)
      throw new ArgumentNullException(nameof (iScene));
    this.mOwner = (Entity) null;
    this.mWorldPos = iWorldPosition;
    this.mIsScreenPos = false;
    this.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, iName, iTTL);
  }

  public virtual void Initialize(
    Scene iScene,
    MagickaFont iFont,
    string iText,
    Vector2 iMinSize,
    Vector2 iScreenPosition,
    bool iForceOnScreen,
    int iName,
    float iTTL)
  {
    if (iScene == null)
      throw new ArgumentNullException(nameof (iScene));
    this.mOwner = (Entity) null;
    this.mWorldPos.X = iScreenPosition.X;
    this.mWorldPos.Y = iScreenPosition.Y;
    this.mWorldPos.Z = 0.0f;
    this.mIsScreenPos = true;
    this.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, iName, iTTL);
  }

  protected virtual void Initialize(
    Scene iScene,
    MagickaFont iFont,
    string iText,
    Vector2 iMinSize,
    bool iForceOnScreen,
    int iName,
    float iTTL)
  {
    if (!this.mGrow)
      this.mScale = 0.0f;
    this.mGrow = true;
    this.mForceScreen = iForceOnScreen;
    this.mTTL = iTTL;
    this.mAutomaticAdvance = (double) iTTL > 0.0;
    this.mString = iText;
    if (iText != null)
    {
      this.mString = LanguageManager.Instance.ParseReferences(this.mString);
      this.mText.SetText(this.mString);
    }
    this.mScene = iScene;
    this.mTextBoxEffect.Texture = (Texture) Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/Dialog_Say");
    string oString;
    this.mShowName = LanguageManager.Instance.TryGetString(iName, out oString);
    if (this.mShowName)
    {
      this.mOwnerName.SetText(oString);
      Vector2 vector2 = this.mOwnerName.Font.MeasureText(this.mOwnerName.Characters, true);
      iMinSize.X = Math.Max(iMinSize.X + 20f, vector2.X);
      iMinSize.Y += vector2.Y;
    }
    if (string.IsNullOrEmpty(this.mString))
      return;
    Vector2 vector2_1 = Vector2.Max(this.mText.Font.MeasureText(this.mText.Characters, true), iMinSize);
    this.mBox.Width = (int) vector2_1.X;
    this.mBox.Height = (int) vector2_1.Y;
  }

  public virtual void Hide() => this.mGrow = false;

  public virtual void Update(
    float iDeltaTime,
    DataChannel iDataChannel,
    ref Matrix iViewProjection)
  {
    if (this.mScene == null || (double) this.mScale <= 1.4012984643248171E-45 && !this.mGrow)
      return;
    TextBox.RenderData iObject = this.mRenderData[(int) iDataChannel];
    this.mScale = !this.mGrow ? Math.Max(this.mScale - iDeltaTime * 8f, 0.0f) : Math.Min(this.mScale + iDeltaTime * 4f, 1f);
    if (this.mOwner != null)
    {
      iObject.mWorldPosition = this.mOwner.Position;
      if (this.mOwner is Magicka.GameLogic.Entities.Character)
      {
        Magicka.GameLogic.Entities.Character mOwner = this.mOwner as Magicka.GameLogic.Entities.Character;
        iObject.mWorldPosition.Y += mOwner.Capsule.Length * 0.5f + mOwner.Capsule.Radius;
      }
    }
    else
      iObject.mWorldPosition = this.mWorldPos;
    if (this.mText.IsFinished)
      this.mTTL -= iDeltaTime;
    if (this.mAutomaticAdvance && (double) this.mTTL < 0.0)
      DialogManager.Instance.Advance(this);
    iObject.mIsScreenPos = this.mIsScreenPos;
    iObject.mForceOnScreen = this.mForceScreen;
    iObject.mScale = this.mScale;
    iObject.mText = this.mText;
    iObject.mOwnerName = this.mOwnerName;
    iObject.mShowName = this.mShowName;
    iObject.mSize.X = (float) this.mBox.Width;
    iObject.mSize.Y = (float) this.mBox.Height;
    this.mScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public void FinishAnimation(bool iRemoveTTLAdvance)
  {
    if (iRemoveTTLAdvance)
      this.mAutomaticAdvance = false;
    this.mScale = this.mGrow ? 1f : 0.0f;
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mText.Finish();
  }

  public string Text => this.mString;

  public Point Position => this.mBox.Location;

  public bool Animating
  {
    get
    {
      return this.mGrow && (double) this.mScale < 0.99000000953674316 || !this.mGrow && (double) this.mScale > 1.4012984643248171E-45 || !this.mText.IsFinished;
    }
  }

  public bool Visible => this.mGrow || (double) this.mScale > 1.4012984643248171E-45;

  public int Width => this.mBox.Width;

  public int Height => this.mBox.Height;

  public bool AutomaticAdvance => this.mAutomaticAdvance;

  protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
  {
    public TextBoxEffect mTextBoxEffect;
    public GUIBasicEffect mGUIBasicEffect;
    protected float mAlpha;
    protected VertexBuffer mVertexBuffer;
    protected VertexDeclaration mVertexDeclaration;
    protected IndexBuffer mIndexBuffer;
    protected int mZIndex;
    public Vector3 mWorldPosition;
    public bool mIsScreenPos;
    public bool mForceOnScreen;
    protected Vector2 mPosition;
    public TypingText mText;
    public bool mShowName;
    public PolygonHead.Text mOwnerName;
    public Vector2 mSize;
    public float mScale;

    public IndexBuffer IndexBuffer
    {
      get => this.mIndexBuffer;
      set => this.mIndexBuffer = value;
    }

    public VertexBuffer VertexBuffer
    {
      get => this.mVertexBuffer;
      set => this.mVertexBuffer = value;
    }

    public VertexDeclaration VertexDeclaration
    {
      get => this.mVertexDeclaration;
      set => this.mVertexDeclaration = value;
    }

    public virtual void Draw(float iDeltaTime)
    {
      this.mTextBoxEffect.Color = Vector4.One;
      this.mTextBoxEffect.Position = this.mPosition;
      this.mTextBoxEffect.Size = this.mSize;
      this.mTextBoxEffect.Scale = this.mScale;
      this.mTextBoxEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
      this.mTextBoxEffect.GraphicsDevice.Indices = this.mIndexBuffer;
      this.mTextBoxEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mTextBoxEffect.Begin();
      this.mTextBoxEffect.CurrentTechnique.Passes[0].Begin();
      this.mTextBoxEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 18);
      this.mTextBoxEffect.CurrentTechnique.Passes[0].End();
      this.mTextBoxEffect.End();
      this.mGUIBasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
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

    public int ZIndex => 201;

    public virtual void PreRenderUpdate(
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
      if (this.mIsScreenPos)
      {
        this.mPosition.X = (float) Math.Floor((double) vector2_1.X * 0.5 * (1.0 + (double) this.mWorldPosition.X));
        this.mPosition.Y = (float) Math.Floor((double) vector2_1.Y * 0.5 * (1.0 + (double) this.mWorldPosition.Y));
      }
      else
      {
        Vector4 result;
        Vector4.Transform(ref this.mWorldPosition, ref iViewProjectionMatrix, out result);
        this.mPosition.X = (float) ((double) result.X / (double) result.W * 0.5 + 0.5) * vector2_1.X;
        this.mPosition.X -= (float) (((double) this.mSize.X * 0.5 + 64.0) / 3.0) * this.mScale;
        this.mPosition.Y = (float) ((double) result.Y / (double) result.W * -0.5 + 0.5) * vector2_1.Y;
        this.mPosition.Y -= (float) ((double) this.mSize.Y * 0.5 + 64.0) * this.mScale;
      }
      if (!this.mForceOnScreen)
        return;
      Vector2 vector2_2 = new Vector2();
      vector2_2.X = (float) Math.Floor((double) vector2_1.X * 0.05000000074505806);
      vector2_2.Y = (float) Math.Floor((double) vector2_1.Y * 0.05000000074505806);
      this.mPosition.X = Math.Max(this.mPosition.X, (float) (((double) this.mSize.X + 32.0) * 0.5) + vector2_2.X);
      this.mPosition.Y = Math.Max(this.mPosition.Y, (float) (((double) this.mSize.Y + 32.0) * 0.5) + vector2_2.Y);
      this.mPosition.X = Math.Min(this.mPosition.X, vector2_1.X - (float) (((double) this.mSize.X + 32.0) * 0.5) - vector2_2.X);
      this.mPosition.Y = Math.Min(this.mPosition.Y, vector2_1.Y - (float) (((double) this.mSize.Y + 32.0) * 0.5) - vector2_2.Y);
    }
  }
}
