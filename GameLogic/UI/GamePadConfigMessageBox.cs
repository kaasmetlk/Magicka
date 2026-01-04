// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.GamePadConfigMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main.Options;
using Magicka.Graphics;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class GamePadConfigMessageBox : MessageBox
{
  private static GamePadConfigMessageBox sSingelton;
  private static volatile object sSingeltonLock = new object();
  private DirectInputController mGamePad;
  private int mFunction;
  private ControllerFunction[][] mFunctions = new ControllerFunction[20][]
  {
    new ControllerFunction[1]{ ControllerFunction.Move_Up },
    new ControllerFunction[1]{ ControllerFunction.Move_Down },
    new ControllerFunction[1]{ ControllerFunction.Move_Left },
    new ControllerFunction[1],
    new ControllerFunction[1]{ ControllerFunction.Spell_Up },
    new ControllerFunction[1]{ ControllerFunction.Spell_Down },
    new ControllerFunction[1]{ ControllerFunction.Spell_Left },
    new ControllerFunction[1]{ ControllerFunction.Spell_Right },
    new ControllerFunction[1]{ ControllerFunction.Spell_Wheel },
    new ControllerFunction[2]
    {
      ControllerFunction.Menu_Select,
      ControllerFunction.Attack
    },
    new ControllerFunction[3]
    {
      ControllerFunction.Menu_Back,
      ControllerFunction.Boost,
      ControllerFunction.Cast_Magick
    },
    new ControllerFunction[1]{ ControllerFunction.Interact },
    new ControllerFunction[2]
    {
      ControllerFunction.Special,
      ControllerFunction.Cast_Self
    },
    new ControllerFunction[1]{ ControllerFunction.Block },
    new ControllerFunction[1]{ ControllerFunction.Cast_Force },
    new ControllerFunction[1]{ ControllerFunction.Cast_Area },
    new ControllerFunction[1]{ ControllerFunction.Magick_Next },
    new ControllerFunction[1]{ ControllerFunction.Magick_Prev },
    new ControllerFunction[1]{ ControllerFunction.Pause },
    new ControllerFunction[1]{ ControllerFunction.Inventory }
  };
  private Texture2D mTexture;
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private MenuTextItem mBack;
  private MenuTextItem mCancel;
  private Text mAction;

  public static GamePadConfigMessageBox Instance
  {
    get
    {
      if (GamePadConfigMessageBox.sSingelton == null)
      {
        lock (GamePadConfigMessageBox.sSingeltonLock)
        {
          if (GamePadConfigMessageBox.sSingelton == null)
            GamePadConfigMessageBox.sSingelton = new GamePadConfigMessageBox();
        }
      }
      return GamePadConfigMessageBox.sSingelton;
    }
  }

  private GamePadConfigMessageBox()
    : base("Configure GamePad (NonLoc)")
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
      this.mTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/GamePadConfig");
    this.mSize = new Vector2(640f, 512f);
    Vector4[] vector4Array = new Vector4[72];
    Vector2 iUVScale = new Vector2();
    iUVScale.X = 1f / (float) this.mTexture.Width;
    iUVScale.Y = 1f / (float) this.mTexture.Height;
    int iIndex = 0;
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(0, 0, 640, 448), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(0, 448, 64 /*0x40*/, 64 /*0x40*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(64 /*0x40*/, 448, 64 /*0x40*/, 64 /*0x40*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(384, 448, 64 /*0x40*/, 64 /*0x40*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(448, 448, 64 /*0x40*/, 64 /*0x40*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(128 /*0x80*/, 448, 128 /*0x80*/, 64 /*0x40*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(256 /*0x0100*/, 448, 128 /*0x80*/, 64 /*0x40*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(640, 0, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(640, 128 /*0x80*/, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(768 /*0x0300*/, 0, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(896, 0, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(768 /*0x0300*/, 128 /*0x80*/, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(896, 128 /*0x80*/, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(640, 256 /*0x0100*/, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(768 /*0x0300*/, 256 /*0x0100*/, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(896, 256 /*0x0100*/, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(768 /*0x0300*/, 384, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    GamePadConfigMessageBox.MakeQuad(vector4Array, ref iIndex, new Rectangle(896, 384, 128 /*0x80*/, 128 /*0x80*/), ref iUVScale);
    lock (graphicsDevice)
    {
      this.mVertices = new VertexBuffer(graphicsDevice, 16 /*0x10*/ * vector4Array.Length, BufferUsage.WriteOnly);
      this.mVertices.SetData<Vector4>(vector4Array);
      this.mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
    }
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.mAction = new Text(128 /*0x80*/, font, TextAlign.Center, false);
    this.mBack = new MenuTextItem(SubMenu.LOC_BACK, new Vector2(), font, TextAlign.Left);
    this.mBack.ColorDisabled = Defines.DIALOGUE_COLOR_DEFAULT * 0.5f;
    this.mBack.Color = Defines.DIALOGUE_COLOR_DEFAULT;
    this.mBack.ColorSelected = Vector4.One;
    this.mCancel = new MenuTextItem(SubMenu.LOC_CANCEL, new Vector2(), font, TextAlign.Right);
    this.mCancel.ColorDisabled = Defines.DIALOGUE_COLOR_DEFAULT * 0.5f;
    this.mCancel.Color = Defines.DIALOGUE_COLOR_DEFAULT;
    this.mCancel.ColorSelected = Vector4.One;
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mBack.LanguageChanged();
    this.mCancel.LanguageChanged();
  }

  private static void MakeQuad(
    Vector4[] iVertices,
    ref int iIndex,
    Rectangle iRect,
    ref Vector2 iUVScale)
  {
    iVertices[iIndex].X = (float) -(iRect.Width / 2);
    iVertices[iIndex].Y = (float) -(iRect.Height / 2);
    iVertices[iIndex].Z = (float) iRect.X * iUVScale.X;
    iVertices[iIndex].W = (float) iRect.Y * iUVScale.Y;
    ++iIndex;
    iVertices[iIndex].X = (float) (iRect.Width / 2);
    iVertices[iIndex].Y = (float) -(iRect.Height / 2);
    iVertices[iIndex].Z = (float) (iRect.X + iRect.Width) * iUVScale.X;
    iVertices[iIndex].W = (float) iRect.Y * iUVScale.Y;
    ++iIndex;
    iVertices[iIndex].X = (float) (iRect.Width / 2);
    iVertices[iIndex].Y = (float) (iRect.Height / 2);
    iVertices[iIndex].Z = (float) (iRect.X + iRect.Width) * iUVScale.X;
    iVertices[iIndex].W = (float) (iRect.Y + iRect.Height) * iUVScale.Y;
    ++iIndex;
    iVertices[iIndex].X = (float) -(iRect.Width / 2);
    iVertices[iIndex].Y = (float) (iRect.Height / 2);
    iVertices[iIndex].Z = (float) iRect.X * iUVScale.X;
    iVertices[iIndex].W = (float) (iRect.Y + iRect.Height) * iUVScale.Y;
    ++iIndex;
  }

  public DirectInputController GamePad
  {
    get => this.mGamePad;
    set => this.mGamePad = value;
  }

  public override void OnTextInput(char iChar)
  {
  }

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    this.mBack.Selected = this.mBack.InsideBounds((float) iNewState.X, (float) iNewState.Y);
    this.mCancel.Selected = this.mCancel.InsideBounds((float) iNewState.X, (float) iNewState.Y);
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (this.mCancel.InsideBounds((float) iNewState.X, (float) iNewState.Y) && iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
    {
      this.mGamePad.Configured = true;
      this.Kill();
    }
    else
    {
      if (!(iNewState.LeftButton == ButtonState.Released & this.mFunction > 0 & this.mBack.InsideBounds((float) iNewState.X, (float) iNewState.Y)))
        return;
      --this.mFunction;
      this.UpdateAction();
    }
  }

  public override void OnSelect(Controller iSender)
  {
  }

  public override void Show()
  {
    this.mFunction = 0;
    this.UpdateAction();
    this.mGamePad.OnChange += new Action<DirectInputController.Binding>(this.OnGamePadChange);
    base.Show();
  }

  private void UpdateAction()
  {
    ControllerFunction[] mFunction = this.mFunctions[this.mFunction];
    string iText = $"({this.mFunction + 1}/{this.mFunctions.Length})\n{mFunction[0].ToString()}";
    for (int index = 1; index < mFunction.Length; ++index)
      iText = $"{iText}/{mFunction[index].ToString()}";
    this.mAction.SetText(iText);
  }

  private void OnGamePadChange(DirectInputController.Binding iBinding)
  {
    if (this.mFunction < this.mFunctions.Length)
    {
      foreach (int index in this.mFunctions[this.mFunction])
        this.mGamePad.Bindings[index] = iBinding;
    }
    ++this.mFunction;
    if (this.mFunction >= this.mFunctions.Length)
    {
      this.mGamePad.Configured = true;
      SaveManager.Instance.SaveSettings();
      this.Kill();
      if (SubMenuOptionsGamepad.Instance.Controller != this.mGamePad)
        return;
      SubMenuOptionsGamepad.Instance.Controller = (Controller) this.mGamePad;
    }
    else
    {
      this.UpdateAction();
      this.mGamePad.OnChange += new Action<DirectInputController.Binding>(this.OnGamePadChange);
    }
  }

  public override void Draw(float iDeltaTime)
  {
    base.Draw(iDeltaTime);
    float num = this.mAlpha * this.mAlpha;
    this.mBack.Enabled = this.mFunction > 0;
    MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16 /*0x10*/);
    MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    Point screenSize = RenderManager.Instance.ScreenSize;
    Vector2 vector2 = new Vector2();
    vector2.X = (float) screenSize.X;
    vector2.Y = (float) screenSize.Y;
    Matrix matrix = new Matrix();
    matrix.M11 = matrix.M22 = 1f;
    matrix.M44 = 1f;
    matrix.M41 = vector2.X * 0.5f;
    matrix.M42 = vector2.Y * 0.5f;
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.Texture = (Texture) this.mTexture;
    MessageBox.sGUIBasicEffect.TextureEnabled = true;
    MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
    Vector4 vector4 = new Vector4();
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = num;
    MessageBox.sGUIBasicEffect.Color = vector4;
    MessageBox.sGUIBasicEffect.CommitChanges();
    MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 + 32.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 + 64.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 18)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 16 /*0x10*/, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 12, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 - 32.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 + 64.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 19)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 16 /*0x10*/, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 12, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 + 112.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 + 48.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 9)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 + 160.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 - 0.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 10)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 + 64.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 - 0.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 11)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 + 112.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 - 48.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 12)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 - 112.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 - 112.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == -1)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 24, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 - 96.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 - 160.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 15)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 24, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 + 112.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 - 112.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 13)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 24, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 + 96.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 - 160.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 14)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 24, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 - 118.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 - 0.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 17)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
    else if (this.mFunction == 16 /*0x10*/)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 60, 2);
    else if (this.mFunction == -1)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 64 /*0x40*/, 2);
    else if (this.mFunction == -1)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 68, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 52, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 - 64.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 + 142.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 0)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 36, 2);
    else if (this.mFunction == 1)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 40, 2);
    else if (this.mFunction == 2)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 44, 2);
    else if (this.mFunction == 3)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 48 /*0x30*/, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 28, 2);
    matrix.M41 = (float) ((double) vector2.X * 0.5 + 64.0);
    matrix.M42 = (float) ((double) vector2.Y * 0.5 + 142.0);
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.CommitChanges();
    if (this.mFunction == 4)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 36, 2);
    else if (this.mFunction == 5)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 40, 2);
    else if (this.mFunction == 6)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 44, 2);
    else if (this.mFunction == 7)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 48 /*0x30*/, 2);
    else if (this.mFunction == 8)
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 32 /*0x20*/, 2);
    else
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 28, 2);
    Vector4 color = MenuItem.COLOR;
    color.W *= num;
    MessageBox.sGUIBasicEffect.Color = color;
    this.mAction.Draw(MessageBox.sGUIBasicEffect, vector2.X * 0.5f, (float) ((double) vector2.Y * 0.5 + (210.0 - 0.5 * (double) this.mAction.Font.LineHeight)));
    Vector4 colorDisabled = MenuItem.COLOR_DISABLED;
    colorDisabled.W *= num;
    Vector4 colorSelected = MenuItem.COLOR_SELECTED;
    colorSelected.W *= num;
    this.mBack.Color = color;
    this.mBack.ColorSelected = colorSelected;
    this.mBack.ColorDisabled = colorDisabled;
    this.mBack.Position = new Vector2((float) ((double) vector2.X * 0.5 - 180.0), (float) ((double) vector2.Y * 0.5 + 220.0));
    this.mBack.Draw(MessageBox.sGUIBasicEffect, 1f);
    this.mCancel.Color = color;
    this.mCancel.ColorSelected = colorSelected;
    this.mCancel.ColorDisabled = colorDisabled;
    this.mCancel.Position = new Vector2((float) ((double) vector2.X * 0.5 + 180.0), (float) ((double) vector2.Y * 0.5 + 220.0));
    this.mCancel.Draw(MessageBox.sGUIBasicEffect, 1f);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }
}
