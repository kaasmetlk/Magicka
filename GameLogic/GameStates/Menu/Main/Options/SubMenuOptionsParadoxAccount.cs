// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.Options.SubMenuOptionsParadoxAccount
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
using SteamWrapper;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main.Options;

internal class SubMenuOptionsParadoxAccount : SubMenu
{
  private const int TITLE_LEN = 32 /*0x20*/;
  private const float TEXT_SPACING = 60f;
  private static readonly int LOC_LINK_STEAM = "#menu_opt_14".GetHashCodeCustom();
  private static readonly int LOC_UNLINK_STEAM = "#menu_opt_13".GetHashCodeCustom();
  private static readonly int LOC_PROCESSING = "#menu_main_processing".GetHashCodeCustom();
  private static readonly int LOC_NOT_LOGGED_IN = "#menu_main_notloggedin".GetHashCodeCustom();
  private static readonly int LOC_LOGOUT = "#acc_log_out".GetHashCodeCustom();
  private static readonly int LOC_PARADOX_ACCOUNT = "#paradox_account".GetHashCodeCustom();
  private static readonly int LOC_WARNING = "#popup_warning".GetHashCodeCustom();
  private static readonly int LOC_STEAMID = "#steam_id".GetHashCodeCustom();
  private static readonly int LOC_PARADOXID = "#paradox_id".GetHashCodeCustom();
  private static readonly Rectangle TITLE_SEPERATOR_SRC = new Rectangle(448, 976, 608, 48 /*0x30*/);
  private static readonly Rectangle TITLE_SEPERATOR_DEST = new Rectangle(208 /*0xD0*/, 180, 608, 48 /*0x30*/);
  private static readonly Rectangle MENU_IMAGERY_SRC = new Rectangle(448, 768 /*0x0300*/, 496, 208 /*0xD0*/);
  private static readonly Rectangle MENU_IMAGERY_DEST = new Rectangle(264, 700, 496, 208 /*0xD0*/);
  private static SubMenuOptionsParadoxAccount mSingelton;
  private static volatile object mLock = new object();
  private MenuTextItem mSteamTextItem;
  private MenuTextItem mLogoutTextItem;
  private MenuTextItem mSteamAccTextItem;
  private MenuTextItem mParadoxAccTextItem;
  private MenuMessagePopup mPopup;

  public static SubMenuOptionsParadoxAccount Instance
  {
    get
    {
      if (SubMenuOptionsParadoxAccount.mSingelton == null)
      {
        lock (SubMenuOptionsParadoxAccount.mLock)
        {
          if (SubMenuOptionsParadoxAccount.mSingelton == null)
            SubMenuOptionsParadoxAccount.mSingelton = new SubMenuOptionsParadoxAccount();
        }
      }
      return SubMenuOptionsParadoxAccount.mSingelton;
    }
  }

  public SubMenuOptionsParadoxAccount()
  {
    this.mSelectedPosition = 2;
    this.mMenuItems = new List<MenuItem>();
    this.mMenuTitle = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_PARADOX_ACCOUNT));
    this.mSteamAccTextItem = this.AddMenuTextItem(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_STEAMID), (object) SteamFriends.GetPersonaName()));
    this.mSteamAccTextItem.Enabled = false;
    this.mParadoxAccTextItem = this.AddMenuTextItem(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_PARADOXID), (object) SubMenu.LOC_NONE));
    this.mParadoxAccTextItem.Enabled = false;
    this.mSteamTextItem = this.AddMenuTextItemBelowPrevious(Singleton<ParadoxAccount>.Instance.IsLinkedToSteam ? SubMenuOptionsParadoxAccount.LOC_UNLINK_STEAM : SubMenuOptionsParadoxAccount.LOC_LINK_STEAM, 60f);
    this.mLogoutTextItem = this.AddMenuTextItemBelowPrevious(Singleton<ParadoxAccount>.Instance.IsLoggedFull ? SubMenuOptionsParadoxAccount.LOC_LOGOUT : SubMenuOptionsParadoxAccount.LOC_NOT_LOGGED_IN, 0.0f);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, Vector2.Zero, SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
    this.mPopup = new MenuMessagePopup();
    this.mPopup.Alignment = PopupAlign.Middle | PopupAlign.Center;
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.OnEnter();
  }

  public override void ControllerUp(Controller iSender)
  {
    if (!this.mKeyboardSelection)
    {
      this.mSelectedPosition = 2;
      this.mKeyboardSelection = true;
    }
    else
    {
      --this.mSelectedPosition;
      if (this.mSelectedPosition >= 2)
        return;
      this.mSelectedPosition = this.mMenuItems.Count - 1;
    }
  }

  public override void ControllerDown(Controller iSender)
  {
    if (!this.mKeyboardSelection)
    {
      this.mSelectedPosition = 2;
      this.mKeyboardSelection = true;
    }
    else
    {
      ++this.mSelectedPosition;
      if (this.mSelectedPosition < this.mMenuItems.Count)
        return;
      this.mSelectedPosition = 2;
    }
  }

  public override void ControllerA(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 2:
        Singleton<ParadoxAccount>.Instance.ToggleSteamLink(new ParadoxAccountSequence.ExecutionDoneDelegate(this.ToggleSteamLinkCallback));
        break;
      case 3:
        Singleton<ParadoxAccount>.Instance.LogOffPlayer(new ParadoxAccountSequence.ExecutionDoneDelegate(this.LogOutCallback));
        break;
      case 4:
        Tome.Instance.PopMenu();
        break;
    }
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    base.Draw(iLeftSide, iRightSide);
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.DrawGraphics(SubMenu.sPagesTexture, SubMenuOptionsParadoxAccount.TITLE_SEPERATOR_SRC, SubMenuOptionsParadoxAccount.TITLE_SEPERATOR_DEST);
    this.DrawGraphics(SubMenu.sPagesTexture, SubMenuOptionsParadoxAccount.MENU_IMAGERY_SRC, SubMenuOptionsParadoxAccount.MENU_IMAGERY_DEST);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  private void ShowErrorPopup(int iLoc)
  {
    this.mPopup.SetTitle(SubMenuOptionsParadoxAccount.LOC_WARNING, MenuItem.COLOR);
    this.mPopup.SetMessage(iLoc, MenuItem.COLOR);
    Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) this.mPopup);
  }

  public void ToggleSteamLinkCallback(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode)
  {
    if (!iSuccess)
      ParadoxPopupUtils.ShowErrorPopup(iErrorCode);
    this.mSteamTextItem.SetText(Singleton<ParadoxAccount>.Instance.IsLinkedToSteam ? SubMenuOptionsParadoxAccount.LOC_UNLINK_STEAM : SubMenuOptionsParadoxAccount.LOC_LINK_STEAM);
  }

  public void LogOutCallback(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode)
  {
    if (iSuccess)
    {
      this.mLogoutTextItem.Enabled = false;
      Singleton<ParadoxAccount>.Instance.Email = string.Empty;
      this.mParadoxAccTextItem.SetText(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_PARADOXID), (object) LanguageManager.Instance.GetString(SubMenu.LOC_NONE)));
      Tome.Instance.PopMenu();
    }
    else
      ParadoxPopupUtils.ShowErrorPopup(iErrorCode);
  }

  public override void OnEnter()
  {
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, GamePadMenuHelp.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    this.mLogoutTextItem.Enabled = Singleton<ParadoxAccount>.Instance.IsLoggedFull;
    this.mSteamAccTextItem.SetText(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_STEAMID), (object) SteamFriends.GetPersonaName()));
    this.mParadoxAccTextItem.SetText(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_PARADOXID), Singleton<ParadoxAccount>.Instance.Email.Equals(string.Empty) ? (object) LanguageManager.Instance.GetString(SubMenu.LOC_NONE) : (object) Singleton<ParadoxAccount>.Instance.Email));
    this.mSteamTextItem.SetText(Singleton<ParadoxAccount>.Instance.IsLinkedToSteam ? SubMenuOptionsParadoxAccount.LOC_UNLINK_STEAM : SubMenuOptionsParadoxAccount.LOC_LINK_STEAM);
    LanguageManager.Instance.LanguageChanged += new Action(((SubMenu) this).LanguageChanged);
  }

  public override void OnExit()
  {
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
    LanguageManager.Instance.LanguageChanged -= new Action(((SubMenu) this).LanguageChanged);
  }

  internal enum MenuItemId
  {
    SteamId,
    ParadoxId,
    LinkSteam,
    Logout,
    Back,
  }
}
