// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenu
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Localization;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal abstract class InGameMenu : IRenderableGUIObject
{
  protected static VertexBuffer sBackground;
  protected static VertexDeclaration sBackgroundDeclaration;
  protected static GUIBasicEffect sEffect;
  protected static Texture2D sBackgroundTexture;
  protected static float sBackgroundAlpha;
  protected static bool sBackgroundVisible;
  protected static Vector2 sLastBackgroundSize;
  protected static PlayState sPlayState;
  protected static Controller sController;
  protected static Vector2 sScreenSize;
  protected static float sScale;
  protected static Stack<InGameMenu> sMenuStack;
  private static InGameMenu sNextMenu;
  public static readonly int SOUND_MOVE = "ui_menu_scroll".GetHashCodeCustom();
  public static readonly int SOUND_INCREASE = "ui_menu_increase".GetHashCodeCustom();
  public static readonly int SOUND_DECREASE = "ui_menu_decrease".GetHashCodeCustom();
  internal static readonly int LOC_ON = "#menu_opt_alt_01".GetHashCodeCustom();
  internal static readonly int LOC_OFF = "#menu_opt_alt_02".GetHashCodeCustom();
  internal static readonly int LOC_PLAYERS_ONLY = "#menu_opt_alt_03".GetHashCodeCustom();
  internal static readonly int LOC_LOW = "#menu_opt_alt_04".GetHashCodeCustom();
  internal static readonly int LOC_MEDIUM = "#menu_opt_alt_05".GetHashCodeCustom();
  internal static readonly int LOC_HIGH = "#menu_opt_alt_06".GetHashCodeCustom();
  protected List<MenuItem> mMenuItems = new List<MenuItem>();
  protected Vector2 mBackgroundSize;
  protected float mAlpha;
  protected bool mVisible;
  protected int mSelectedItem;
  protected int mDownedSelection;

  public virtual void LanguageChanged()
  {
    if (this.mMenuItems == null)
      return;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].LanguageChanged();
  }

  protected static int GetOnOffLoc(bool iOnOff)
  {
    return iOnOff ? "#menu_opt_alt_01".GetHashCodeCustom() : "#menu_opt_alt_02".GetHashCodeCustom();
  }

  protected static int GetSettingLoc(SettingOptions iSetting)
  {
    switch (iSetting)
    {
      case SettingOptions.Off:
        return InGameMenu.LOC_OFF;
      case SettingOptions.On:
        return InGameMenu.LOC_ON;
      case SettingOptions.Players_Only:
        return InGameMenu.LOC_PLAYERS_ONLY;
      case SettingOptions.Low:
        return InGameMenu.LOC_LOW;
      case SettingOptions.Medium:
        return InGameMenu.LOC_MEDIUM;
      case SettingOptions.High:
        return InGameMenu.LOC_HIGH;
      default:
        return 0;
    }
  }

  static InGameMenu() => InGameMenu.sMenuStack = new Stack<InGameMenu>();

  public InGameMenu()
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    Point point;
    point.X = GlobalSettings.Instance.Resolution.Width;
    point.Y = GlobalSettings.Instance.Resolution.Height;
    InGameMenu.sScreenSize.X = (float) point.X;
    InGameMenu.sScreenSize.Y = (float) point.Y;
    if (InGameMenu.sBackgroundTexture == null || InGameMenu.sBackgroundTexture.IsDisposed)
    {
      lock (graphicsDevice)
        InGameMenu.sBackgroundTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/FadeBox");
    }
    if (InGameMenu.sEffect == null || InGameMenu.sEffect.IsDisposed)
    {
      lock (graphicsDevice)
        InGameMenu.sEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
    }
    if (InGameMenu.sBackground == null || InGameMenu.sBackground.IsDisposed)
    {
      Vector2[] data = new Vector2[4];
      data[0].X = 0.0f;
      data[0].Y = 0.0f;
      data[1].X = 1f;
      data[1].Y = 0.0f;
      data[2].X = 1f;
      data[2].Y = 1f;
      data[3].X = 0.0f;
      data[3].Y = 1f;
      lock (graphicsDevice)
      {
        InGameMenu.sBackground = new VertexBuffer(graphicsDevice, 32 /*0x20*/, BufferUsage.WriteOnly);
        InGameMenu.sBackground.SetData<Vector2>(data);
      }
    }
    if (InGameMenu.sBackgroundDeclaration == null || InGameMenu.sBackgroundDeclaration.IsDisposed)
    {
      lock (graphicsDevice)
        InGameMenu.sBackgroundDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[2]
        {
          new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
          new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
        });
    }
    InGameMenu.sEffect.SetScreenSize(point.X, point.Y);
    InGameMenu.sScale = InGameMenu.sScreenSize.Y / 720f;
    LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
  }

  protected void DrawGraphics(Texture2D iTexture, Rectangle iScrRect, Vector4 iDestRect)
  {
    this.DrawGraphics(iTexture, iScrRect, iDestRect, Vector4.One);
  }

  protected void DrawGraphics(
    Texture2D iTexture,
    Rectangle iScrRect,
    Vector4 iDestRect,
    Vector4 iColor)
  {
    Vector2 vector2_1 = new Vector2();
    Vector2 vector2_2 = new Vector2();
    if (iTexture != null)
    {
      vector2_1.X = (float) iScrRect.X / (float) iTexture.Width;
      vector2_1.Y = (float) iScrRect.Y / (float) iTexture.Height;
      vector2_2.X = (float) iScrRect.Width / (float) iTexture.Width;
      vector2_2.Y = (float) iScrRect.Height / (float) iTexture.Height;
      InGameMenu.sEffect.TextureOffset = vector2_1;
      InGameMenu.sEffect.TextureScale = vector2_2;
    }
    InGameMenu.sEffect.Transform = new Matrix()
    {
      M11 = iDestRect.Z,
      M22 = iDestRect.W,
      M41 = iDestRect.X,
      M42 = iDestRect.Y,
      M44 = 1f
    };
    InGameMenu.sEffect.Texture = (Texture) iTexture;
    InGameMenu.sEffect.TextureEnabled = iTexture != null;
    InGameMenu.sEffect.Color = iColor;
    InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
    InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
    InGameMenu.sEffect.CommitChanges();
    InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
  }

  public virtual void UpdatePositions()
  {
    Vector2 vector2 = new Vector2();
    vector2.X = InGameMenu.sScreenSize.X * 0.5f;
    vector2.Y = 290f * InGameMenu.sScale;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Scale = InGameMenu.sScale;
      mMenuItem.Position = vector2;
      vector2.Y += mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y;
    }
  }

  protected virtual void AddMenuTextItem(int iText, BitmapFont iFont, TextAlign iTextAlign)
  {
    MenuTextItem menuTextItem = new MenuTextItem(iText, new Vector2(), iFont, iTextAlign);
    menuTextItem.Scale = InGameMenu.sScale;
    menuTextItem.ColorDisabled = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
    menuTextItem.Color = new Vector4(1f, 1f, 1f, 1f);
    menuTextItem.ColorSelected = new Vector4(10f, 10f, 10f, 1f);
    this.mMenuItems.Add((MenuItem) menuTextItem);
  }

  protected virtual void AddMenuTextItem(string iText, BitmapFont iFont, TextAlign iTextAlign)
  {
    MenuTextItem menuTextItem = new MenuTextItem(iText, new Vector2(), iFont, iTextAlign);
    menuTextItem.Scale = InGameMenu.sScale;
    menuTextItem.ColorDisabled = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
    menuTextItem.Color = new Vector4(1f, 1f, 1f, 1f);
    menuTextItem.ColorSelected = new Vector4(10f, 10f, 10f, 1f);
    this.mMenuItems.Add((MenuItem) menuTextItem);
  }

  protected virtual void IMouseMove(Controller iSender, ref Vector2 iMousePosition)
  {
    int num = -1;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      if (this.mMenuItems[index].Enabled && this.mMenuItems[index].InsideBounds(ref iMousePosition))
      {
        num = index;
        break;
      }
    }
    if (this.mSelectedItem != num & num >= 0)
      AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
    this.mSelectedItem = num;
  }

  protected virtual void IMouseScroll(Controller iSender, ref Vector2 iMousePos, int iValue)
  {
  }

  protected virtual void IMouseDown(Controller iSender, ref Vector2 iMousePosition)
  {
    this.mDownedSelection = this.mSelectedItem;
  }

  protected virtual void IMouseUp(Controller iSender, ref Vector2 iMousePosition)
  {
    if (this.mDownedSelection != this.mSelectedItem)
      return;
    InGameMenu.ControllerSelect(iSender);
  }

  protected virtual void IControllerMove(Controller iSender, ControllerDirection iDirection)
  {
    switch (iDirection)
    {
      case ControllerDirection.Up:
        int mSelectedItem1 = this.mSelectedItem;
        do
        {
          --mSelectedItem1;
          if (mSelectedItem1 < 0)
            mSelectedItem1 += this.mMenuItems.Count;
        }
        while (!this.mMenuItems[mSelectedItem1].Enabled);
        this.mSelectedItem = mSelectedItem1;
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
        break;
      case ControllerDirection.Down:
        int mSelectedItem2 = this.mSelectedItem;
        do
        {
          ++mSelectedItem2;
          if (mSelectedItem2 >= this.mMenuItems.Count)
            mSelectedItem2 -= this.mMenuItems.Count;
        }
        while (!this.mMenuItems[mSelectedItem2].Enabled);
        this.mSelectedItem = mSelectedItem2;
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
        break;
    }
  }

  protected void SendButtonPressTelemetry()
  {
    if (InGameMenu.sMenuStack.Peek().mSelectedItem < 0)
      return;
    TelemetryUtils.SendInGameMenuButtonPressTelemetry(InGameMenu.sPlayState.GameType.ToString(), InGameMenu.sPlayState.Level.Name.Substring(InGameMenu.sPlayState.Level.Name.IndexOf('#') + 1), InGameMenu.sMenuStack.Peek().IGetHighlightedButtonName());
  }

  protected abstract string IGetHighlightedButtonName();

  protected abstract void IControllerSelect(Controller iSender);

  protected abstract void IControllerBack(Controller iSender);

  protected abstract void OnEnter();

  protected virtual void IUpdate(DataChannel iDataChannel, float iDeltaTime)
  {
    InGameMenu.sPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) this);
  }

  protected abstract void OnExit();

  public virtual void Draw(float iDeltaTime)
  {
    InGameMenu.sBackgroundAlpha = !InGameMenu.sBackgroundVisible ? Math.Max(InGameMenu.sBackgroundAlpha - iDeltaTime * 5f, 0.0f) : Math.Min(InGameMenu.sBackgroundAlpha + iDeltaTime * 5f, 1f);
    this.mAlpha = !this.mVisible ? Math.Max(this.mAlpha - iDeltaTime * 5f, 0.0f) : Math.Min(this.mAlpha + iDeltaTime * 5f, 1f);
    if ((double) this.mAlpha >= 1.0)
      InGameMenu.sLastBackgroundSize = this.mBackgroundSize;
    Vector2 result;
    Vector2.SmoothStep(ref InGameMenu.sLastBackgroundSize, ref this.mBackgroundSize, this.mAlpha, out result);
    Vector4 vector4 = new Vector4();
    vector4.W = 0.5f * InGameMenu.sBackgroundAlpha;
    Matrix matrix = new Matrix();
    matrix.M11 = InGameMenu.sScreenSize.X;
    matrix.M22 = InGameMenu.sScreenSize.Y;
    matrix.M44 = 1f;
    InGameMenu.sEffect.Color = vector4;
    InGameMenu.sEffect.Transform = matrix;
    InGameMenu.sEffect.TextureEnabled = false;
    InGameMenu.sEffect.VertexColorEnabled = false;
    InGameMenu.sEffect.Texture = (Texture) InGameMenu.sBackgroundTexture;
    InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
    InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
    InGameMenu.sEffect.Begin();
    InGameMenu.sEffect.CurrentTechnique.Passes[0].Begin();
    InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    matrix.M11 = result.X * InGameMenu.sScale;
    matrix.M22 = result.Y * InGameMenu.sScale;
    matrix.M41 = (float) (((double) InGameMenu.sScreenSize.X - (double) result.X * (double) InGameMenu.sScale) * 0.5);
    matrix.M42 = (float) ((720.0 - (double) result.Y) * 0.5) * InGameMenu.sScale;
    vector4.W = 0.666f * InGameMenu.sBackgroundAlpha;
    InGameMenu.sEffect.Color = vector4;
    InGameMenu.sEffect.Transform = matrix;
    InGameMenu.sEffect.TextureEnabled = true;
    InGameMenu.sEffect.CommitChanges();
    InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    this.IDraw(iDeltaTime, ref result);
    InGameMenu.sEffect.CurrentTechnique.Passes[0].End();
    InGameMenu.sEffect.End();
  }

  protected virtual void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
  {
    Vector4 vector4_1 = new Vector4();
    vector4_1.X = vector4_1.Y = vector4_1.Z = 1f;
    vector4_1.W = this.mAlpha;
    Vector4 vector4_2 = new Vector4();
    vector4_2.X = vector4_2.Y = vector4_2.Z = 0.0f;
    vector4_2.W = this.mAlpha;
    Vector4 vector4_3 = new Vector4();
    vector4_3.X = vector4_3.Y = vector4_3.Z = 0.4f;
    vector4_3.W = this.mAlpha;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Color = vector4_1;
      mMenuItem.ColorSelected = vector4_2;
      mMenuItem.ColorDisabled = vector4_3;
      mMenuItem.Selected = mMenuItem.Enabled & this.mSelectedItem == index;
      if (mMenuItem.Selected)
      {
        InGameMenu.sEffect.Transform = new Matrix()
        {
          M44 = 1f,
          M11 = iBackgroundSize.X * InGameMenu.sScale,
          M22 = mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y,
          M41 = mMenuItem.Position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale,
          M42 = mMenuItem.TopLeft.Y
        };
        Vector4 vector4_4 = new Vector4();
        vector4_4.X = vector4_4.Y = vector4_4.Z = 1f;
        vector4_4.W = 0.8f * this.mAlpha;
        InGameMenu.sEffect.Color = vector4_4;
        InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
        InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
        InGameMenu.sEffect.CommitChanges();
        InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      }
    }
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      if (this.mMenuItems[index].Enabled)
        this.mMenuItems[index].Draw(InGameMenu.sEffect);
    }
  }

  public int ZIndex => 1000;

  public static InGameMenu CurrentMenu
  {
    get => InGameMenu.sMenuStack.Count == 0 ? (InGameMenu) null : InGameMenu.sMenuStack.Peek();
  }

  public static void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    InGameMenu inGameMenu = InGameMenu.sMenuStack.Peek();
    inGameMenu.IUpdate(iDataChannel, iDeltaTime);
    if (inGameMenu.mVisible || (double) inGameMenu.mAlpha >= 1.4012984643248171E-45)
      return;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      inGameMenu.OnExit();
      if (InGameMenu.sNextMenu == null)
      {
        if (InGameMenu.sMenuStack.Count > 1)
          InGameMenu.sMenuStack.Pop();
      }
      else
      {
        InGameMenu.sMenuStack.Push(InGameMenu.sNextMenu);
        InGameMenu.sNextMenu = (InGameMenu) null;
      }
      InGameMenu.sMenuStack.Peek().OnEnter();
      InGameMenu.sMenuStack.Peek().mVisible = true;
      InGameMenu.sMenuStack.Peek().mAlpha = 0.0f;
    }
  }

  public static void PushMenu(InGameMenu iNewMenu)
  {
    InGameMenu.sNextMenu = iNewMenu;
    InGameMenu.sMenuStack.Peek().mVisible = false;
  }

  public static void PopMenu() => InGameMenu.sMenuStack.Peek().mVisible = false;

  public static void ControllerSelect(Controller iSender)
  {
    if (!InGameMenu.sBackgroundVisible || !InGameMenu.IsControllerAllowed(iSender) || !InGameMenu.sMenuStack.Peek().mVisible)
      return;
    InGameMenu.sMenuStack.Peek().SendButtonPressTelemetry();
    InGameMenu.sMenuStack.Peek().IControllerSelect(iSender);
  }

  public static void ControllerBack(Controller iSender)
  {
    if (!InGameMenu.sBackgroundVisible || !InGameMenu.IsControllerAllowed(iSender) || !InGameMenu.sMenuStack.Peek().mVisible)
      return;
    InGameMenu.sMenuStack.Peek().IControllerBack(iSender);
  }

  public static void MouseScroll(Controller iSender, ref Vector2 iMousePos, int iValue)
  {
    if (!InGameMenu.sBackgroundVisible || !InGameMenu.IsControllerAllowed(iSender) || !InGameMenu.sMenuStack.Peek().mVisible)
      return;
    InGameMenu.sMenuStack.Peek().IMouseScroll(iSender, ref iMousePos, iValue);
  }

  public static void MouseMove(Controller iSender, ref Vector2 iMousePos)
  {
    if (!InGameMenu.sBackgroundVisible || !InGameMenu.IsControllerAllowed(iSender) || !InGameMenu.sMenuStack.Peek().mVisible)
      return;
    InGameMenu.sMenuStack.Peek().IMouseMove(iSender, ref iMousePos);
  }

  public static void MouseDown(Controller iSender, ref Vector2 iMousePos)
  {
    if (!InGameMenu.sBackgroundVisible || !InGameMenu.IsControllerAllowed(iSender) || !InGameMenu.sMenuStack.Peek().mVisible)
      return;
    InGameMenu.sMenuStack.Peek().IMouseDown(iSender, ref iMousePos);
  }

  public static void MouseUp(Controller iSender, ref Vector2 iMousePos)
  {
    if (!InGameMenu.sBackgroundVisible || !InGameMenu.IsControllerAllowed(iSender) || !InGameMenu.sMenuStack.Peek().mVisible)
      return;
    InGameMenu.sMenuStack.Peek().IMouseUp(iSender, ref iMousePos);
  }

  public static void ControllerMove(Controller iSender, ControllerDirection iDirection)
  {
    if (!InGameMenu.sBackgroundVisible || !InGameMenu.IsControllerAllowed(iSender) || !InGameMenu.sMenuStack.Peek().mVisible)
      return;
    InGameMenu.sMenuStack.Peek().IControllerMove(iSender, iDirection);
  }

  public static void Initialize(PlayState iPlayState)
  {
    InGameMenu.sPlayState = iPlayState;
    InGameMenu.sBackgroundVisible = false;
    InGameMenu.sMenuStack.Clear();
    InGameMenu.sMenuStack.Push((InGameMenu) InGameMenuMain.Instance);
    InGameMenu.sBackgroundAlpha = 0.0f;
  }

  public static void Show(Controller iController)
  {
    InGameMenu.sController = iController;
    if (InGameMenu.sBackgroundVisible)
      return;
    Point screenSize = RenderManager.Instance.ScreenSize;
    InGameMenu.sEffect.SetScreenSize(screenSize.X, screenSize.Y);
    InGameMenu.sScreenSize.X = (float) screenSize.X;
    InGameMenu.sScreenSize.Y = (float) screenSize.Y;
    InGameMenu.sScale = (float) screenSize.Y / 720f;
    InGameMenu.UpdateAllPositions();
    while (InGameMenu.sMenuStack.Count > 1)
      InGameMenu.sMenuStack.Pop();
    InGameMenu.sLastBackgroundSize = InGameMenu.sMenuStack.Peek().mBackgroundSize;
    InGameMenu.sMenuStack.Peek().mAlpha = 0.0f;
    InGameMenu.sMenuStack.Peek().mVisible = true;
    InGameMenu.sMenuStack.Peek().OnEnter();
    InGameMenu.sBackgroundVisible = true;
  }

  public static void Hide()
  {
    InGameMenu.sBackgroundVisible = false;
    InGameMenu.sMenuStack.Peek().mVisible = false;
  }

  public static void HideInstant()
  {
    InGameMenu.sBackgroundVisible = false;
    InGameMenu.sMenuStack.Peek().mVisible = false;
    InGameMenu.sBackgroundAlpha = 0.0f;
  }

  public static bool Visible
  {
    get => InGameMenu.sBackgroundVisible | (double) InGameMenu.sBackgroundAlpha > 0.0;
  }

  public static bool IsControllerAllowed(Controller iController)
  {
    return InGameMenu.sController == null || InGameMenu.sController == iController;
  }

  protected static void UpdateAllPositions()
  {
    Point point;
    point.X = GlobalSettings.Instance.Resolution.Width;
    point.Y = GlobalSettings.Instance.Resolution.Height;
    InGameMenu.sEffect.SetScreenSize(point.X, point.Y);
    InGameMenu.sScreenSize.X = (float) point.X;
    InGameMenu.sScreenSize.Y = (float) point.Y;
    InGameMenu.sScale = (float) point.Y / 720f;
    InGameMenuMain.Instance.UpdatePositions();
    InGameMenuMagicks.Instance.UpdatePositions();
    InGameMenuOptions.Instance.UpdatePositions();
    InGameMenuOptionsGame.Instance.UpdatePositions();
    InGameMenuOptionsSound.Instance.UpdatePositions();
    InGameMenuOptionsGraphics.Instance.UpdatePositions();
    InGameMenuOptionsResolution.Instance.UpdatePositions();
  }
}
