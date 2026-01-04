// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.MessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

internal abstract class MessageBox : IRenderableGUIObject, IDisposable
{
  protected const float BORDERSIZE = 32f;
  protected const float INDENTATION = 20f;
  protected const float VERTICAL_PADDING = 16f;
  protected static VertexBuffer sVertexBuffer;
  protected static VertexDeclaration sVertexDeclaration;
  protected static Texture2D sTexture;
  protected static GUIBasicEffect sGUIBasicEffect;
  protected Text mMessage;
  protected Vector2 mCenter;
  protected BitmapFont mFont;
  protected float mMessageHeight;
  protected bool mDead;
  protected float mAlpha;
  protected Vector2 mSize;

  public MessageBox(string iMessage)
  {
    this.mDead = true;
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mCenter.X = (float) screenSize.X * 0.5f;
    this.mCenter.Y = (float) screenSize.Y * 0.5f;
    if (MessageBox.sVertexBuffer == null)
    {
      GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
      lock (graphicsDevice)
        MessageBox.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
      Vector2 vector2 = new Vector2();
      vector2.X = 1f / (float) MessageBox.sTexture.Width;
      vector2.Y = 1f / (float) MessageBox.sTexture.Height;
      Vector4[] data = new Vector4[4];
      data[0].X = -224f;
      data[0].Y = -280f;
      data[0].Z = 0.0f * vector2.X;
      data[0].W = 464f * vector2.Y;
      data[1].X = 224f;
      data[1].Y = -280f;
      data[1].Z = 448f * vector2.X;
      data[1].W = 464f * vector2.Y;
      data[2].X = 224f;
      data[2].Y = 280f;
      data[2].Z = 448f * vector2.X;
      data[2].W = 1024f * vector2.Y;
      data[3].X = -224f;
      data[3].Y = 280f;
      data[3].Z = 0.0f * vector2.X;
      data[3].W = 1024f * vector2.Y;
      lock (graphicsDevice)
      {
        MessageBox.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * 4 * 4, BufferUsage.None);
        MessageBox.sVertexBuffer.SetData<Vector4>(data);
        MessageBox.sVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, new VertexElement[2]
        {
          new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
          new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
        });
      }
      MessageBox.sGUIBasicEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
      MessageBox.sGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
      MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
      MessageBox.sGUIBasicEffect.TextureEnabled = true;
      MessageBox.sGUIBasicEffect.Color = Vector4.One;
    }
    this.mSize = new Vector2(448f, 560f);
    this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
    string iText = this.mFont.Wrap(iMessage, (int) ((double) this.mSize.X * 0.89999997615814209), true);
    this.mMessage = new Text(512 /*0x0200*/, this.mFont, TextAlign.Center, false);
    this.mMessage.SetText(iText);
    this.mMessage.DefaultColor = MenuItem.COLOR;
    this.mMessageHeight = this.mFont.MeasureText(iText, true).Y;
    LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
  }

  public MessageBox(int iMessage)
    : this(LanguageManager.Instance.GetString(iMessage))
  {
  }

  public virtual void LanguageChanged()
  {
  }

  public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mCenter.X = (float) screenSize.X * 0.5f;
    this.mCenter.Y = (float) screenSize.Y * 0.5f;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) this);
  }

  public abstract void OnTextInput(char iChar);

  public abstract void OnMove(Controller iSender, ControllerDirection iDirection);

  public abstract void OnMouseMove(MouseState iNewState, MouseState iOldState);

  public abstract void OnMouseClick(MouseState iNewState, MouseState iOldState);

  public abstract void OnSelect(Controller iSender);

  public virtual void ControllerEsc(Controller iSender) => this.Kill();

  public virtual void Show()
  {
    this.mAlpha = 0.0f;
    this.mDead = false;
    DialogManager.Instance.AddMessageBox(this);
  }

  public virtual void Kill() => this.mDead = true;

  public bool Dead => this.mDead & (double) this.mAlpha < 1.4012984643248171E-45;

  public virtual void Draw(float iDeltaTime)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mCenter.X = (float) screenSize.X * 0.5f;
    this.mCenter.Y = (float) screenSize.Y * 0.5f;
    MessageBox.sGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mAlpha = !this.mDead ? Math.Min(this.mAlpha + iDeltaTime * 4f, 1f) : Math.Max(this.mAlpha - iDeltaTime * 4f, 0.0f);
    Matrix matrix = new Matrix();
    matrix.M11 = matrix.M22 = matrix.M33 = 1f;
    matrix.M44 = 1f;
    matrix.M41 = this.mCenter.X;
    matrix.M42 = this.mCenter.Y;
    MessageBox.sGUIBasicEffect.Transform = matrix;
    Vector4 vector4 = new Vector4();
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4;
    MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
    MessageBox.sGUIBasicEffect.Texture = (Texture) MessageBox.sTexture;
    MessageBox.sGUIBasicEffect.TextureEnabled = true;
    MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(MessageBox.sVertexBuffer, 0, 16 /*0x10*/);
    MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = MessageBox.sVertexDeclaration;
    MessageBox.sGUIBasicEffect.Begin();
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
    MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
  }

  public virtual int ZIndex => 2000;

  public virtual void Dispose()
  {
    DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
    LanguageManager.Instance.LanguageChanged -= new Action(this.LanguageChanged);
    this.mMessage.Dispose();
  }
}
