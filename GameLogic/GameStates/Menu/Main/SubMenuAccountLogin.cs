// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuAccountLogin
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.GameLogic.UI.Popup;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.WebTools;
using Magicka.WebTools.Paradox;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuAccountLogin : SubMenu
{
  private const float BTN_WIDTH = 260f;
  private const int MAX_VISUAL_LENGTH = 40;
  private const string ELLIPSIS = "...";
  private static readonly int LOC_LOGIN_TITLE = "#acc_log_in_title".GetHashCodeCustom();
  private static readonly int LOC_LOGIN_BTN = "#acc_log_in".GetHashCodeCustom();
  private static readonly int LOC_DEFAULT_EMAIL = "#acc_default_email".GetHashCodeCustom();
  private static readonly int LOC_DEFAULT_PASSWORD = "#acc_default_password".GetHashCodeCustom();
  private static readonly Rectangle TITLE_SEPERATOR_SRC = new Rectangle(448, 976, 608, 48 /*0x30*/);
  private static readonly Rectangle TITLE_SEPERATOR_DEST = new Rectangle(208 /*0xD0*/, 220, 608, 48 /*0x30*/);
  private static readonly Rectangle MENU_IMAGERY_SRC = new Rectangle(448, 768 /*0x0300*/, 496, 208 /*0xD0*/);
  private static readonly Rectangle MENU_IMAGERY_DEST = new Rectangle(264, 700, 496, 208 /*0xD0*/);
  private static readonly Vector2 EMAIL_POSITION = Vector2.Zero;
  private static readonly Vector2 PASSWORD_POSITION = new Vector2(0.0f, 140f);
  private static readonly Vector2 LOGIN_POSITION = new Vector2(0.0f, 300f);
  private static SubMenuAccountLogin mSingleton;
  private static volatile object mLock = new object();
  private TextInputMessageBox mEmailMsgBox;
  private TextInputMessageBox mPasswordMsgBox;
  private MenuItemMultiLineText mEmailTextItem;
  private MenuItemMultiLineText mPasswordTextItem;
  private string mCachedEmail = string.Empty;
  private string mCachedPassword = string.Empty;
  private MenuTextButtonItem mLogInButton;
  private MenuItem mBackButton;
  private MenuMessagePopup mPopup;

  public static SubMenuAccountLogin Instance
  {
    get
    {
      if (SubMenuAccountLogin.mSingleton == null)
      {
        lock (SubMenuAccountLogin.mLock)
        {
          if (SubMenuAccountLogin.mSingleton == null)
            SubMenuAccountLogin.mSingleton = new SubMenuAccountLogin();
        }
      }
      return SubMenuAccountLogin.mSingleton;
    }
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuAccountLogin.LOC_LOGIN_TITLE));
    this.mEmailMsgBox.LanguageChanged();
    this.mPasswordMsgBox.LanguageChanged();
  }

  private SubMenuAccountLogin()
  {
    this.mMenuItems = new List<MenuItem>();
    this.mMenuTitle = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuAccountLogin.LOC_LOGIN_TITLE));
    this.mEmailMsgBox = new TextInputMessageBox(SubMenu.LOC_EMAIL, 128 /*0x80*/);
    this.mPasswordMsgBox = new TextInputMessageBox(SubMenu.LOC_PASSWORD, 128 /*0x80*/);
    this.mPasswordMsgBox.SecureMode = true;
    this.mEmailTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountLogin.EMAIL_POSITION, MagickaFont.MenuOption, 10f);
    this.mEmailTextItem.AddNewLine(SubMenu.LOC_EMAIL, TextAlign.Center, MenuItem.COLOR);
    this.mEmailTextItem.AddNewLine(SubMenuAccountLogin.LOC_DEFAULT_EMAIL, TextAlign.Center, MenuItem.COLOR_DISABLED);
    this.mMenuItems.Add((MenuItem) this.mEmailTextItem);
    this.mPasswordTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountLogin.PASSWORD_POSITION, MagickaFont.MenuOption, 10f);
    this.mPasswordTextItem.AddNewLine(SubMenu.LOC_PASSWORD, TextAlign.Center, MenuItem.COLOR);
    this.mPasswordTextItem.AddNewLine(SubMenuAccountLogin.LOC_DEFAULT_PASSWORD, TextAlign.Center, MenuItem.COLOR_DISABLED);
    this.mMenuItems.Add((MenuItem) this.mPasswordTextItem);
    this.mLogInButton = this.CreateMenuTextButton(SubMenuAccountLogin.LOC_LOGIN_BTN, 260f);
    this.mLogInButton.Position = this.mPosition + SubMenuAccountLogin.LOGIN_POSITION;
    this.mMenuItems.Add((MenuItem) this.mLogInButton);
    this.mBackButton = (MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, Vector2.Zero, SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE);
    this.mMenuItems.Add(this.mBackButton);
    this.mPopup = new MenuMessagePopup();
    this.mPopup.Alignment = PopupAlign.Middle | PopupAlign.Center;
  }

  public void OnEmailAddressChanged(string iName)
  {
    this.mCachedEmail = iName;
    if (this.mCachedEmail == string.Empty)
    {
      this.mEmailTextItem.SetText(1, SubMenuAccountLogin.LOC_DEFAULT_EMAIL);
    }
    else
    {
      string iLoc = this.mCachedEmail;
      if (iLoc.Length > 40)
        iLoc = iLoc.Substring(0, 40) + "...";
      this.mEmailTextItem.SetText(1, iLoc);
      if (!(this.mCachedPassword == string.Empty))
        return;
      this.mSelectedPosition = 1;
      this.mPasswordMsgBox.Show(new Action<string>(this.OnPasswordChanged));
    }
  }

  public void OnPasswordChanged(string iPassword)
  {
    this.mCachedPassword = iPassword;
    if (this.mCachedPassword == string.Empty)
    {
      this.mPasswordTextItem.SetText(1, SubMenuAccountLogin.LOC_DEFAULT_PASSWORD);
    }
    else
    {
      this.mPasswordTextItem.SetText(1, new string('*', iPassword.Length > 40 ? 40 : iPassword.Length));
      this.mSelectedPosition = 2;
    }
  }

  public void OnLoginBtnClick()
  {
    if (this.mSelectedPosition >= 0)
    {
      this.mMenuItems[this.mSelectedPosition].Selected = false;
      this.mSelectedPosition = -1;
    }
    if (!ParadoxUtils.IsValidEmail(this.mCachedEmail))
    {
      this.mPopup.SetTitle(SubMenu.LOC_POPUP_WARNING, MenuItem.COLOR);
      this.mPopup.SetMessage(SubMenu.LOC_POPUP_INVALID_EMAIL, MenuItem.COLOR);
      Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) this.mPopup);
    }
    else
    {
      this.mPopup.Clear();
      Singleton<ParadoxAccount>.Instance.LoginPlayer(this.mCachedEmail, this.mCachedPassword, new ParadoxAccountSequence.ExecutionDoneDelegate(Tome.OnLoggedIn));
    }
  }

  public override void ControllerUp(Controller iSender)
  {
    if (!this.mKeyboardSelection)
    {
      this.mSelectedPosition = 0;
      this.mKeyboardSelection = true;
    }
    else
    {
      --this.mSelectedPosition;
      if (this.mSelectedPosition >= 0)
        return;
      this.mSelectedPosition = this.mMenuItems.Count - 1;
    }
  }

  public override void ControllerDown(Controller iSender)
  {
    if (!this.mKeyboardSelection)
    {
      this.mSelectedPosition = 0;
      this.mKeyboardSelection = true;
    }
    else
    {
      ++this.mSelectedPosition;
      if (this.mSelectedPosition < this.mMenuItems.Count)
        return;
      this.mSelectedPosition = 0;
    }
  }

  public override void ControllerA(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 0:
        this.mEmailMsgBox.Show(new Action<string>(this.OnEmailAddressChanged));
        break;
      case 1:
        this.mPasswordMsgBox.Show(new Action<string>(this.OnPasswordChanged));
        break;
      case 2:
        this.OnLoginBtnClick();
        break;
      case 3:
        Tome.Instance.PopMenu();
        break;
    }
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    base.Draw(iLeftSide, iRightSide);
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.DrawGraphics(SubMenu.sPagesTexture, SubMenuAccountLogin.TITLE_SEPERATOR_SRC, SubMenuAccountLogin.TITLE_SEPERATOR_DEST);
    this.DrawGraphics(SubMenu.sPagesTexture, SubMenuAccountLogin.MENU_IMAGERY_SRC, SubMenuAccountLogin.MENU_IMAGERY_DEST);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public override void OnEnter()
  {
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, GamePadMenuHelp.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    this.mCachedEmail = string.Empty;
    this.mCachedPassword = string.Empty;
    this.mEmailTextItem.SetText(1, SubMenuAccountLogin.LOC_DEFAULT_EMAIL);
    this.mPasswordTextItem.SetText(1, SubMenuAccountLogin.LOC_DEFAULT_PASSWORD);
    this.mEmailTextItem.MarkAsDirty();
    this.mPasswordTextItem.MarkAsDirty();
  }

  public override void OnExit()
  {
  }

  internal enum MenuItemId
  {
    None = -1, // 0xFFFFFFFF
    EmailAddress = 0,
    Password = 1,
    Login = 2,
    Back = 3,
  }
}
