// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.SubMenu
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

internal abstract class SubMenu
{
  public const float LINE_SEPARATION = 10f;
  protected const float BACK_PADDING = 8f;
  protected const int MAX_TITLE_LEN = 32 /*0x20*/;
  private const string TAG_SPRITESHEET_NAME = "UI/Menu/tag_spritesheet";
  public static readonly int LOC_MYTHOS = "#tsar_menu_mythos".GetHashCodeCustom();
  public static readonly int LOC_ADVENTURE = "#menu_main_01".GetHashCodeCustom();
  public static readonly int LOC_TSAR = "#menu_main_tsar".GetHashCodeCustom();
  public static readonly int LOC_CHALLENGES = "#menu_main_02".GetHashCodeCustom();
  public static readonly int LOC_VERSUS = "#menu_main_03".GetHashCodeCustom();
  public static readonly int LOC_LEADERBOARDS = "#menu_main_04".GetHashCodeCustom();
  public static readonly int LOC_ONLINE_PLAY = "#menu_main_05".GetHashCodeCustom();
  public static readonly int LOC_OPTIONS = "#menu_main_06".GetHashCodeCustom();
  public static readonly int LOC_QUIT = "#menu_main_07".GetHashCodeCustom();
  public static readonly int LOC_DOWNLOADABLE_CONTENT = "#menu_main_08".GetHashCodeCustom();
  public static readonly int LOC_HOW_TO_PLAY = "#menu_main_09".GetHashCodeCustom();
  public static readonly int LOC_YES = "#add_menu_yes".GetHashCodeCustom();
  public static readonly int LOC_NO = "#add_menu_no".GetHashCodeCustom();
  public static readonly int LOC_OK = "#add_menu_ok".GetHashCodeCustom();
  public static readonly int LOC_CANCEL = "#add_menu_cancel".GetHashCodeCustom();
  public static readonly int LOC_DELETE = "#add_menu_delete".GetHashCodeCustom();
  public static readonly int LOC_ON = "#menu_opt_alt_01".GetHashCodeCustom();
  public static readonly int LOC_OFF = "#menu_opt_alt_02".GetHashCodeCustom();
  public static readonly int LOC_PLAYERS_ONLY = "#menu_opt_alt_03".GetHashCodeCustom();
  public static readonly int LOC_LOW = "#menu_opt_alt_04".GetHashCodeCustom();
  public static readonly int LOC_MEDIUM = "#menu_opt_alt_05".GetHashCodeCustom();
  public static readonly int LOC_HIGH = "#menu_opt_alt_06".GetHashCodeCustom();
  public static readonly int LOC_NONE = "#menu_opt_alt_09".GetHashCodeCustom();
  public static readonly int LOC_ENABLED = "#menu_opt_alt_10".GetHashCodeCustom();
  public static readonly int LOC_DISABLED = "#menu_opt_alt_11".GetHashCodeCustom();
  public static readonly int LOC_ANY = "#network_32".GetHashCodeCustom();
  public static readonly int LOC_PREVIOUS_PAGE = "#menu_page_p".GetHashCodeCustom();
  public static readonly int LOC_NEXT_PAGE = "#menu_page_n".GetHashCodeCustom();
  public static readonly int LOC_SELECT = "#menu_select".GetHashCodeCustom();
  public static readonly int LOC_BACK = "#menu_back".GetHashCodeCustom();
  public static readonly int LOC_CLOSE = "#menu_close".GetHashCodeCustom();
  public static readonly int LOC_OPEN = "#menu_open".GetHashCodeCustom();
  public static readonly int LOC_PAUSED = "#menu_paused".GetHashCodeCustom();
  public static readonly int LOC_RESUME = "#menu_resume".GetHashCodeCustom();
  public static readonly int LOC_SETTINGS = "#add_menu_settings".GetHashCodeCustom();
  public static readonly int LOC_START = "#add_menu_start".GetHashCodeCustom();
  public static readonly int LOC_LOGIN = "".GetHashCodeCustom();
  public static readonly int LOC_REMEMBER_ME = "".GetHashCodeCustom();
  public static readonly int LOC_CHANGE_CHAPTER = "#change_chapter".GetHashCodeCustom();
  public static readonly int LOC_ENTER_NAME = "#ADD_MENU_PROF_NAME".GetHashCodeCustom();
  public static readonly int LOC_TT_DUNG1 = "#challenge_dungeons_chapter1".GetHashCodeCustom();
  public static readonly int LOC_TT_DUNG2 = "#challenge_dungeons_chapter2".GetHashCodeCustom();
  public static readonly int LOC_TT_OSOTC = "#challenge_osotc".GetHashCodeCustom();
  public static readonly int LOC_TT_VIETNAM = "#challenge_vietnam".GetHashCodeCustom();
  public static readonly int LOC_EMAIL = "#acc_login_email".GetHashCodeCustom();
  public static readonly int LOC_PASSWORD = "#acc_login_password".GetHashCodeCustom();
  public static readonly int LOC_DATEOFBIRTH = "#acc_dob".GetHashCodeCustom();
  public static readonly int LOC_POPUP_WARNING = "#popup_warning".GetHashCodeCustom();
  public static readonly int LOC_POPUP_INVALID_EMAIL = "#popup_invalid_email".GetHashCodeCustom();
  public static readonly int LOC_POPUP_INVALID_PASSWORD = "#popup_invalid_password".GetHashCodeCustom();
  public static readonly int LOC_POPUP_INVALID_DATEOFBIRTH = "#popup_invalid_dateofbirth".GetHashCodeCustom();
  public static readonly int LOC_POPUP_ACCOUNT_EXISTS = "#popup_acc_exists".GetHashCodeCustom();
  public static readonly int LOC_POPUP_ACCOUNT_REGISTERED = "#popup_acc_registered".GetHashCodeCustom();
  public static readonly int LOC_POPUP_MISSING_DETAILS = "#popup_missing_details".GetHashCodeCustom();
  public static readonly int LOC_POPUP_LOGIN_SUCCESS = "#popup_login_success".GetHashCodeCustom();
  public static readonly int LOC_POPUP_TERMSANDCONDITIONS = "#popup_termsandconditions".GetHashCodeCustom();
  protected static Texture2D sPagesTexture;
  protected static Texture2D sStainsTexture;
  protected static Texture2D sTagTexture;
  protected static readonly Vector2 BACK_SIZE = new Vector2(320f, 96f);
  protected static readonly Vector2 BACK_UVOFFSET = new Vector2((float) ((2048.0 - (double) SubMenu.BACK_SIZE.X - 32.0) / 2048.0), 3f / 64f);
  protected static readonly Vector2 BACK_UVSCALE = new Vector2(SubMenu.BACK_SIZE.X / 2048f, SubMenu.BACK_SIZE.Y / 1024f);
  protected static readonly Vector2 BACK_TEXTPOS = new Vector2(216f, (float) ((double) SubMenu.BACK_SIZE.Y * 0.5 + 14.0));
  protected static readonly MagickaFont BACK_FONT = MagickaFont.MenuOption;
  protected static readonly TextAlign BACK_TEXT_ALIGN = TextAlign.Center;
  protected static readonly Vector2 BACK_POSITION = new Vector2(96f, (float) ((double) Tome.PAGERIGHTSHEET.Y - (double) SubMenu.BACK_SIZE.Y - 16.0));
  protected int mSelectedPosition;
  protected bool mKeyboardSelection;
  protected Vector2 mPosition;
  protected GUIBasicEffect mEffect;
  protected List<MenuItem> mMenuItems;
  protected Text mMenuTitle;
  protected float mSelectedPositionF;
  private static VertexBuffer sGenericVertexBuffer;
  private static VertexDeclaration sGenericVertexDeclaration;

  protected static int GetSettingLoc(bool iSetting)
  {
    return SubMenu.GetSettingLoc(iSetting ? SettingOptions.On : SettingOptions.Off);
  }

  protected static int GetSettingLoc(SettingOptions iSetting)
  {
    switch (iSetting)
    {
      case SettingOptions.Off:
        return SubMenu.LOC_OFF;
      case SettingOptions.On:
        return SubMenu.LOC_ON;
      case SettingOptions.Players_Only:
        return SubMenu.LOC_PLAYERS_ONLY;
      case SettingOptions.Low:
        return SubMenu.LOC_LOW;
      case SettingOptions.Medium:
        return SubMenu.LOC_MEDIUM;
      case SettingOptions.High:
        return SubMenu.LOC_HIGH;
      default:
        return 0;
    }
  }

  protected static string GetSettingString(bool iSetting)
  {
    return SubMenu.GetSettingString(iSetting ? SettingOptions.On : SettingOptions.Off);
  }

  protected static string GetSettingString(SettingOptions iSetting)
  {
    return LanguageManager.Instance.GetString(SubMenu.GetSettingLoc(iSetting));
  }

  public SubMenu()
  {
    this.mEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
    this.mEffect.TextureEnabled = true;
    this.mEffect.ScaleToHDR = true;
    Viewport pagerightsheet = Tome.PAGERIGHTSHEET;
    this.mEffect.SetScreenSize(pagerightsheet.Width, pagerightsheet.Height);
    this.mPosition = new Vector2((float) pagerightsheet.Width * 0.5f, (float) pagerightsheet.Height * 0.333f);
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (SubMenu.sPagesTexture == null)
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        SubMenu.sPagesTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
        SubMenu.sStainsTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/ToM/Stains");
        SubMenu.sTagTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/tag_spritesheet");
      }
    }
    if (SubMenu.sGenericVertexBuffer == null)
    {
      Vector4[] data = new Vector4[4]
      {
        new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
        new Vector4(1f, 0.0f, 1f, 0.0f),
        new Vector4(1f, 1f, 1f, 1f),
        new Vector4(0.0f, 1f, 0.0f, 1f)
      };
      SubMenu.sGenericVertexBuffer = new VertexBuffer(graphicsDevice, 4 * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      SubMenu.sGenericVertexBuffer.SetData<Vector4>(data);
      SubMenu.sGenericVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
    }
    this.mKeyboardSelection = true;
    LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
  }

  public virtual MenuTextItem AddMenuTextItem(int iText)
  {
    return this.AddMenuTextItem(LanguageManager.Instance.GetString(iText));
  }

  public virtual MenuTextItem AddMenuTextItem(string iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuItems.Count;
    MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, font, TextAlign.Center);
    this.mMenuItems.Add((MenuItem) menuTextItem);
    return menuTextItem;
  }

  public virtual MenuTextItem AddMenuTextItemBelowPrevious(int iText, float iExtraSpacing)
  {
    return this.AddMenuTextItemBelowPrevious(LanguageManager.Instance.GetString(iText), iExtraSpacing);
  }

  public virtual MenuTextItem AddMenuTextItemBelowPrevious(string iText, float iExtraSpacing)
  {
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    Vector2 iPosition = this.mPosition;
    if (this.mMenuItems.Count > 0)
      iPosition = this.mMenuItems[this.mMenuItems.Count - 1].Position;
    iPosition.Y += (float) font.LineHeight + 10f + iExtraSpacing;
    MenuTextItem menuTextItem = new MenuTextItem(iText, iPosition, font, TextAlign.Center);
    this.mMenuItems.Add((MenuItem) menuTextItem);
    return menuTextItem;
  }

  public virtual MenuTextButtonItem CreateMenuTextButton(int iLoc, float minWidth)
  {
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    return new MenuTextButtonItem(Vector2.Zero, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, iLoc, font, minWidth, TextAlign.Center);
  }

  public virtual void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.mEffect.VertexColorEnabled = false;
    this.mEffect.Color = MenuItem.COLOR;
    if (this.mMenuTitle != null)
      this.mMenuTitle.Draw(this.mEffect, 512f, 96f);
    foreach (MenuItem mMenuItem in this.mMenuItems)
      mMenuItem.Draw(this.mEffect);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public virtual void DrawNewAndOld(
    SubMenu iPreviousMenu,
    Viewport iCurrentLeftSide,
    Viewport iCurrentRightSide,
    Viewport iPreviousLeftSide,
    Viewport iPreviousRightSide)
  {
    this.Draw(iCurrentLeftSide, iCurrentRightSide);
    iPreviousMenu?.Draw(iPreviousLeftSide, iPreviousRightSide);
  }

  public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mKeyboardSelection)
    {
      for (int index = 0; index < this.mMenuItems.Count; ++index)
        this.mMenuItems[index].Selected = index == this.mSelectedPosition;
    }
    if (this.mSelectedPosition < 0 || this.mSelectedPosition >= this.mMenuItems.Count)
      return;
    this.mSelectedPositionF = MathHelper.Lerp(this.mMenuItems[this.mSelectedPosition].Position.Y, this.mSelectedPositionF, (float) Math.Pow(0.00025, (double) iDeltaTime));
  }

  public virtual void ControllerUp(Controller iSender)
  {
    if (this.mMenuItems == null)
      return;
    this.mKeyboardSelection = true;
    do
    {
      --this.mSelectedPosition;
      if (this.mSelectedPosition < 0)
        this.mSelectedPosition = this.mMenuItems.Count - 1;
    }
    while (!this.mMenuItems[this.mSelectedPosition].Enabled);
  }

  public int NumItems => this.mMenuItems != null ? this.mMenuItems.Count : 0;

  public int CurrentlySelectedPosition => this.mSelectedPosition;

  public void ForceSetCurrentSelected(bool truefalse)
  {
    if (this.mMenuItems == null || this.mMenuItems.Count == 0 || this.mSelectedPosition < 0 || this.mSelectedPosition > this.mMenuItems.Count - 1)
      return;
    do
    {
      this.mMenuItems[this.mSelectedPosition].Selected = truefalse;
    }
    while (!this.mMenuItems[this.mSelectedPosition].Enabled);
  }

  public void UnselectAll()
  {
    this.ForceSetCurrentSelected(false);
    this.mSelectedPosition = -1;
  }

  public void ForceSetAndSelectCurrent(int index)
  {
    if (this.mMenuItems == null)
      return;
    if (index < 0)
      index = 0;
    else if (index > this.mMenuItems.Count - 1)
      index = this.mMenuItems.Count - 1;
    this.mSelectedPosition = index;
    this.ForceSetCurrentSelected(true);
  }

  public virtual void ControllerDown(Controller iSender)
  {
    if (this.mMenuItems == null)
      return;
    this.mKeyboardSelection = true;
    do
    {
      ++this.mSelectedPosition;
      if (this.mSelectedPosition >= this.mMenuItems.Count)
        this.mSelectedPosition = 0;
    }
    while (!this.mMenuItems[this.mSelectedPosition].Enabled);
  }

  public virtual void LanguageChanged()
  {
    if (this.mMenuItems == null)
      return;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].LanguageChanged();
  }

  protected virtual void ControllerMouseClicked(Controller iSender)
  {
    AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_select".GetHashCodeCustom());
    this.ControllerA(iSender);
  }

  public virtual void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (this.mMenuItems == null || this.mMenuItems.Count == 0 || iState.LeftButton == ButtonState.Pressed || !Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) || !oRightPageHit)
      return;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      if (mMenuItem.Enabled && mMenuItem.InsideBounds(ref oHitPosition))
      {
        this.mSelectedPosition = index;
        if ((iState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed) && (iState.RightButton != ButtonState.Released || iOldState.RightButton != ButtonState.Pressed))
          break;
        this.ControllerMouseClicked(iSender);
        break;
      }
    }
  }

  public virtual void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (this.mMenuItems == null || this.mMenuItems.Count == 0)
      return;
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit))
    {
      if (!oRightPageHit)
        return;
      bool flag = false;
      for (int index1 = 0; index1 < this.mMenuItems.Count; ++index1)
      {
        MenuItem mMenuItem = this.mMenuItems[index1];
        if (mMenuItem.Enabled && mMenuItem.InsideBounds(ref oHitPosition))
        {
          if (this.mSelectedPosition != index1)
            AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
          this.mKeyboardSelection = false;
          this.mSelectedPosition = index1;
          for (int index2 = 0; index2 < this.mMenuItems.Count; ++index2)
            this.mMenuItems[index2].Selected = index2 == index1;
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      for (int index = 0; index < this.mMenuItems.Count; ++index)
        this.mMenuItems[index].Selected = false;
      this.mSelectedPosition = -1;
    }
    else
    {
      if (!(!this.mKeyboardSelection & this.mMenuItems != null))
        return;
      for (int index = 0; index < this.mMenuItems.Count; ++index)
        this.mMenuItems[index].Selected = false;
    }
  }

  protected void DrawGraphics(Texture2D iTexture, Rectangle iScrRect, Rectangle iDestRect)
  {
    this.DrawGraphics(iTexture, iScrRect, iDestRect, Vector4.One);
  }

  protected void DrawGraphics(
    Texture2D iTexture,
    Rectangle iScrRect,
    Rectangle iDestRect,
    Vector4 iColor)
  {
    Vector2 vector2_1 = new Vector2();
    Vector2 vector2_2 = new Vector2();
    vector2_1.X = (float) iScrRect.X / (float) iTexture.Width;
    vector2_1.Y = (float) iScrRect.Y / (float) iTexture.Height;
    vector2_2.X = (float) iScrRect.Width / (float) iTexture.Width;
    vector2_2.Y = (float) iScrRect.Height / (float) iTexture.Height;
    this.mEffect.TextureOffset = vector2_1;
    this.mEffect.TextureScale = vector2_2;
    this.mEffect.Transform = new Matrix()
    {
      M11 = (float) iDestRect.Width,
      M22 = (float) iDestRect.Height,
      M41 = (float) iDestRect.X,
      M42 = (float) iDestRect.Y,
      M44 = 1f
    };
    this.mEffect.Texture = (Texture) iTexture;
    this.mEffect.Color = iColor;
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenu.sGenericVertexBuffer, 0, 16 /*0x10*/);
    this.mEffect.GraphicsDevice.VertexDeclaration = SubMenu.sGenericVertexDeclaration;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
  }

  public virtual void ControllerEvent(
    Controller iSender,
    KeyboardState iOldState,
    KeyboardState iNewState)
  {
  }

  public virtual void ControllerRight(Controller iSender)
  {
  }

  public virtual void ControllerLeft(Controller iSender)
  {
  }

  public virtual void ControllerA(Controller iSender)
  {
  }

  public virtual void ControllerB(Controller iSender) => Tome.Instance.PopMenu();

  public virtual void ControllerX(Controller iSender)
  {
  }

  public virtual void ControllerY(Controller iSender)
  {
  }

  public virtual void OnEnter()
  {
  }

  public virtual void OnExit()
  {
  }

  internal virtual void NetworkInput(ref MenuSelectMessage iMessage)
  {
  }
}
