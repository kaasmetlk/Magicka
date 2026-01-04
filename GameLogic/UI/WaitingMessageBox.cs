// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.WaitingMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class WaitingMessageBox : MessageBox
{
  private const int NR_OF_SQR = 7;
  private const float SQR_SIZE = 32f;
  private const float SQR_PADD = 6f;
  public static readonly int LOC_WAITING_FOR_OTHER_PLAYERS = "#network_24".GetHashCodeCustom();
  private static WaitingMessageBox sSingelton;
  private static volatile object sSingeltonLock = new object();
  private new static VertexBuffer sVertexBuffer;
  private new static VertexDeclaration sVertexDeclaration;
  private float[] mAlphas = new float[7];
  private int mMessageText;
  private Text mDots;
  private Vector2 mDotPosition;
  private float mTimer;
  private int mDotCount;
  public Action OnAbort;

  public static WaitingMessageBox Instance
  {
    get
    {
      if (WaitingMessageBox.sSingelton == null)
      {
        lock (WaitingMessageBox.sSingeltonLock)
        {
          if (WaitingMessageBox.sSingelton == null)
            WaitingMessageBox.sSingelton = new WaitingMessageBox();
        }
      }
      return WaitingMessageBox.sSingelton;
    }
  }

  static WaitingMessageBox()
  {
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      WaitingMessageBox.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Defines.QUAD_COL_VERTS_C.Length * VertexPositionColor.SizeInBytes, BufferUsage.WriteOnly);
      WaitingMessageBox.sVertexBuffer.SetData<VertexPositionColor>(Defines.QUAD_COL_VERTS_C);
      WaitingMessageBox.sVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionColor.VertexElements);
    }
  }

  private WaitingMessageBox()
    : base("")
  {
    this.mDots = new Text(10, this.mMessage.Font, TextAlign.Left, false);
    this.mMessage.DefaultColor = this.mDots.DefaultColor;
  }

  private WaitingMessageBox(int iMessage)
    : base(iMessage)
  {
  }

  private WaitingMessageBox(string iMessage)
    : base(iMessage)
  {
  }

  public override void Draw(float iDeltaTime)
  {
    if (this.mDead)
      this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 4f, 0.0f);
    else
      this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 4f, 1f);
    Point screenSize = RenderManager.Instance.ScreenSize;
    MessageBox.sGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
    MessageBox.sGUIBasicEffect.Begin();
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
    MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(WaitingMessageBox.sVertexBuffer, 0, VertexPositionColor.SizeInBytes);
    MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = WaitingMessageBox.sVertexDeclaration;
    MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
    MessageBox.sGUIBasicEffect.TextureEnabled = false;
    Matrix identity = Matrix.Identity with
    {
      M11 = (float) screenSize.X,
      M22 = (float) screenSize.Y,
      M41 = (float) screenSize.X * 0.5f,
      M42 = (float) screenSize.Y * 0.5f
    };
    MessageBox.sGUIBasicEffect.Transform = identity;
    MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha * 0.5f);
    MessageBox.sGUIBasicEffect.CommitChanges();
    MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
    this.mMessage.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X, this.mCenter.Y);
    this.mTimer += iDeltaTime * 2.5f;
    if ((double) this.mTimer > 1.0)
    {
      this.mTimer = 0.0f;
      ++this.mDotCount;
      if (this.mDotCount > 4)
      {
        this.mDots.Clear();
        this.mDotCount = 0;
      }
      else
        this.mDots.Append(".");
    }
    this.mDots.Draw(MessageBox.sGUIBasicEffect, this.mDotPosition.X, this.mDotPosition.Y);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }

  public void Show(int iMessage)
  {
    this.mMessageText = iMessage;
    this.mMessage.SetText(LanguageManager.Instance.GetString(iMessage));
    this.mDotPosition = this.mCenter;
    this.mDotPosition.X += this.mMessage.Font.MeasureText(this.mMessage.Characters, true).X * 0.5f;
    if (!this.mDead)
      return;
    base.Show();
  }

  public override void Show() => this.Show(WaitingMessageBox.LOC_WAITING_FOR_OTHER_PLAYERS);

  public override void Kill()
  {
    this.OnAbort = (Action) null;
    base.Kill();
  }

  public override void OnTextInput(char iChar)
  {
  }

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
  }

  public override void OnSelect(Controller iSender)
  {
  }

  public override void ControllerEsc(Controller iSender)
  {
    if (this.OnAbort != null)
      this.OnAbort();
    this.Kill();
  }

  public override void LanguageChanged()
  {
    if (this.mMessageText == 0)
      return;
    this.mMessage.SetText(LanguageManager.Instance.GetString(this.mMessageText));
    this.mDotPosition = this.mCenter;
    this.mDotPosition.X += this.mMessage.Font.MeasureText(this.mMessage.Characters, true).X * 0.5f;
  }
}
