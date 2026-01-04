// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuAccountCreate
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
using System.Globalization;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuAccountCreate : SubMenu
{
  private const int BUTTON_SPACING = 40;
  private const float BTN_MIN_WIDTH = 200f;
  private const int CHECKBOX_SIZE = 32 /*0x20*/;
  private const float CHECKBOX_PADDING = 35f;
  private const float BTN_CREATE_OFFSET = 25f;
  private const int MAX_VISUAL_LENGTH = 40;
  private const string ELLIPSIS = "...";
  private const string PARADOX_PRIVACY_URL = "https://www.paradoxplaza.com/privacy";
  private const string PARADOX_TERMS_URL = "https://www.paradoxplaza.com/terms-use";
  private static readonly int LOC_CREATEACCOUNT_TITLE = "#acc_create_acc_title".GetHashCodeCustom();
  private static readonly int LOC_BTN_CREATEACCOUNT = "#acc_create_acc".GetHashCodeCustom();
  private static readonly int LOC_PRIVACYPOLICY = "#privacy_policy".GetHashCodeCustom();
  private static readonly int LOC_TERMSANDCONDITIONS = "#terms_conditions".GetHashCodeCustom();
  private static readonly int LOC_NOTICE = "#acc_notice".GetHashCodeCustom();
  private static readonly int LOC_NEWSLETTER = "#acc_newsletter_subscribe".GetHashCodeCustom();
  private static readonly int LOC_COUNTRY = "#choose_your_country".GetHashCodeCustom();
  private static readonly int LOC_DEFAULT_EMAIL = "#acc_default_email".GetHashCodeCustom();
  private static readonly int LOC_DEFAULT_PASSWORD = "#acc_default_password".GetHashCodeCustom();
  private static readonly int LOC_DEFAULT_DOB = "#acc_default_dob".GetHashCodeCustom();
  private static readonly Rectangle TITLE_SEPERATOR_SRC = new Rectangle(448, 976, 608, 48 /*0x30*/);
  private static readonly Rectangle TITLE_SEPERATOR_DEST = new Rectangle(208 /*0xD0*/, 180, 608, 48 /*0x30*/);
  private static readonly Rectangle MENU_IMAGERY_SRC = new Rectangle(448, 768 /*0x0300*/, 496, 208 /*0xD0*/);
  private static readonly Rectangle MENU_IMAGERY_DEST = new Rectangle(264, 700, 496, 208 /*0xD0*/);
  private static readonly Vector2 POS_EMAIL = new Vector2(0.0f, -65f);
  private static readonly Vector2 POS_PASSWORD = new Vector2(0.0f, 30f);
  private static readonly Vector2 POS_DATEOFBIRTH = new Vector2(0.0f, 135f);
  private static readonly Vector2 POS_COUNTRY = new Vector2(0.0f, 240f);
  private static readonly Vector2 POS_LEGALS = new Vector2(0.0f, 350f);
  private static readonly Vector2 POS_NEWSLETTER = new Vector2(0.0f, 450f);
  private static readonly Vector2 POS_BTN_POLICY = new Vector2(0.0f, 525f);
  private static readonly Vector2 POS_BTN_TERMS = new Vector2(0.0f, 525f);
  private static SubMenuAccountCreate mSingleton = (SubMenuAccountCreate) null;
  private static volatile object mLock = new object();
  private TextInputMessageBox mEmailMsgBox;
  private TextInputMessageBox mPasswordMsgBox;
  private TextInputMessageBox mDateOfBirthMsgBox;
  private MenuItemMultiLineText mEmailTextItem;
  private MenuItemMultiLineText mPasswordTextItem;
  private MenuItemMultiLineText mDateOfBirthTextItem;
  private MenuItemMultiLineText mCountryTextItem;
  private MenuItemMultiLineText mAgreementTextItem;
  private MenuItemMultiLineText mNewsletterTextItem;
  private CheckBox mTermsCheckbox;
  private CheckBox mNewsletterCheckBox;
  private string mCachedEmail = string.Empty;
  private string mCachedPassword = string.Empty;
  private string mCachedDateOfBirth = string.Empty;
  private string mCachedCountry = string.Empty;
  private string mCachedCountryCode = string.Empty;
  private MenuMessagePopup mPopup;
  private MenuTextButtonItem mTermsButton;
  private MenuTextButtonItem mPolicyButton;
  private MenuTextButtonItem mCreateButton;
  private MenuItem mBackButton;

  public static SubMenuAccountCreate Instance
  {
    get
    {
      if (SubMenuAccountCreate.mSingleton == null)
      {
        lock (SubMenuAccountCreate.mLock)
        {
          if (SubMenuAccountCreate.mSingleton == null)
            SubMenuAccountCreate.mSingleton = new SubMenuAccountCreate();
        }
      }
      return SubMenuAccountCreate.mSingleton;
    }
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuAccountCreate.LOC_CREATEACCOUNT_TITLE));
    this.mEmailMsgBox.LanguageChanged();
    this.mPasswordMsgBox.LanguageChanged();
    this.mDateOfBirthMsgBox.LanguageChanged();
    this.mCountryTextItem.LanguageChanged();
    this.mPopup.LanguageChanged();
  }

  private SubMenuAccountCreate()
  {
    this.mMenuItems = new List<MenuItem>();
    this.mMenuTitle = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuAccountCreate.LOC_CREATEACCOUNT_TITLE));
    this.mEmailMsgBox = new TextInputMessageBox(SubMenu.LOC_EMAIL, 128 /*0x80*/);
    this.mPasswordMsgBox = new TextInputMessageBox(SubMenu.LOC_PASSWORD, 128 /*0x80*/);
    this.mPasswordMsgBox.SecureMode = true;
    this.mDateOfBirthMsgBox = new TextInputMessageBox(SubMenu.LOC_DATEOFBIRTH, 10);
    this.mDateOfBirthMsgBox.InputType = TextInputMessageBox.MessageBoxInputType.Date;
    this.mEmailTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_EMAIL, MagickaFont.MenuOption, 10f);
    this.mEmailTextItem.AddNewLine(SubMenu.LOC_EMAIL, TextAlign.Center, MenuItem.COLOR);
    this.mEmailTextItem.AddNewLine(SubMenuAccountCreate.LOC_DEFAULT_EMAIL, TextAlign.Center, MenuItem.COLOR_DISABLED);
    this.mMenuItems.Add((MenuItem) this.mEmailTextItem);
    this.mPasswordTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_PASSWORD, MagickaFont.MenuOption, 10f);
    this.mPasswordTextItem.AddNewLine(SubMenu.LOC_PASSWORD, TextAlign.Center, MenuItem.COLOR);
    this.mPasswordTextItem.AddNewLine(SubMenuAccountCreate.LOC_DEFAULT_PASSWORD, TextAlign.Center, MenuItem.COLOR_DISABLED);
    this.mMenuItems.Add((MenuItem) this.mPasswordTextItem);
    this.mDateOfBirthTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_DATEOFBIRTH, MagickaFont.MenuOption, 10f);
    this.mDateOfBirthTextItem.AddNewLine(SubMenu.LOC_DATEOFBIRTH, TextAlign.Center, MenuItem.COLOR);
    this.mDateOfBirthTextItem.AddNewLine(SubMenuAccountCreate.LOC_DEFAULT_DOB, TextAlign.Center, MenuItem.COLOR_DISABLED);
    this.mMenuItems.Add((MenuItem) this.mDateOfBirthTextItem);
    this.mCountryTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_COUNTRY, MagickaFont.MenuOption, 10f);
    this.mCountryTextItem.AddNewLine(SubMenuAccountCreate.LOC_COUNTRY, TextAlign.Center, MenuItem.COLOR);
    this.mCountryTextItem.AddNewLine(RegionInfo.CurrentRegion.DisplayName, TextAlign.Center, MenuItem.COLOR_DISABLED);
    this.mMenuItems.Add((MenuItem) this.mCountryTextItem);
    this.mAgreementTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_LEGALS, MagickaFont.MenuOption);
    this.mAgreementTextItem.AddLines(SubMenuAccountCreate.LOC_NOTICE, TextAlign.Center, MenuItem.COLOR, 800);
    this.mTermsCheckbox = new CheckBox(false);
    this.mTermsCheckbox.Position = this.mAgreementTextItem.Position + new Vector2((float) ((double) this.mAgreementTextItem.Size.X * 0.5 + 35.0), 0.0f);
    this.mTermsCheckbox.Width = this.mTermsCheckbox.Height = 32 /*0x20*/;
    this.mMenuItems.Add((MenuItem) this.mTermsCheckbox);
    this.mNewsletterTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_NEWSLETTER, MagickaFont.MenuOption);
    this.mNewsletterTextItem.AddLines(SubMenuAccountCreate.LOC_NEWSLETTER, TextAlign.Center, MenuItem.COLOR, 800);
    this.mNewsletterCheckBox = new CheckBox(false);
    this.mNewsletterCheckBox.Position = this.mNewsletterTextItem.Position + new Vector2((float) ((double) this.mNewsletterTextItem.Size.X * 0.5 + 35.0), -16f);
    this.mNewsletterCheckBox.Width = this.mTermsCheckbox.Height = 32 /*0x20*/;
    this.mMenuItems.Add((MenuItem) this.mNewsletterCheckBox);
    this.mPolicyButton = this.CreateMenuTextButton(SubMenuAccountCreate.LOC_PRIVACYPOLICY, 200f);
    this.mPolicyButton.Position = this.mPosition + SubMenuAccountCreate.POS_BTN_POLICY - new Vector2((float) ((double) this.mPolicyButton.RealWidth * 0.5 + 40.0), 0.0f);
    this.mMenuItems.Add((MenuItem) this.mPolicyButton);
    this.mTermsButton = this.CreateMenuTextButton(SubMenuAccountCreate.LOC_TERMSANDCONDITIONS, 200f);
    this.mTermsButton.Position = this.mPosition + SubMenuAccountCreate.POS_BTN_TERMS + new Vector2((float) ((double) this.mTermsButton.RealWidth * 0.5 + 40.0), 0.0f);
    this.mMenuItems.Add((MenuItem) this.mTermsButton);
    this.mCreateButton = this.CreateMenuTextButton(SubMenuAccountCreate.LOC_BTN_CREATEACCOUNT, 200f);
    this.mCreateButton.Position = new Vector2((float) ((double) this.mTermsButton.Position.X + (double) this.mTermsButton.RealWidth * 0.5 - (double) this.mCreateButton.RealWidth * 0.5), (float) ((double) Tome.PAGERIGHTSHEET.Y - (double) SubMenu.BACK_SIZE.Y + 25.0));
    this.mMenuItems.Add((MenuItem) this.mCreateButton);
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
      this.mEmailTextItem.SetText(1, SubMenuAccountCreate.LOC_DEFAULT_EMAIL);
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
      this.mPasswordTextItem.SetText(1, SubMenuAccountCreate.LOC_DEFAULT_PASSWORD);
    }
    else
    {
      this.mPasswordTextItem.SetText(1, new string('*', iPassword.Length > 40 ? 40 : iPassword.Length));
      if (!(this.mCachedDateOfBirth == string.Empty))
        return;
      this.mSelectedPosition = 2;
      this.mDateOfBirthMsgBox.Show(new Action<string>(this.OnDateOfBirthChanged));
    }
  }

  public void OnDateOfBirthChanged(string iDoB)
  {
    this.mCachedDateOfBirth = iDoB;
    if (this.mCachedDateOfBirth == string.Empty)
    {
      this.mDateOfBirthTextItem.SetText(1, SubMenuAccountCreate.LOC_DEFAULT_DOB);
    }
    else
    {
      this.mDateOfBirthTextItem.SetText(1, this.mCachedDateOfBirth);
      this.mSelectedPosition = 3;
    }
  }

  public void OnCountryChanged(string iCountry, string iCode)
  {
    this.mCachedCountry = iCountry;
    this.mCachedCountryCode = iCode;
    this.mCountryTextItem.SetText(1, iCountry);
  }

  public void OnAccountCreateBtnClick()
  {
    if (this.mSelectedPosition >= 0)
    {
      this.mMenuItems[this.mSelectedPosition].Selected = false;
      this.mSelectedPosition = -1;
    }
    if (!this.mTermsCheckbox.Checked)
    {
      this.mPopup.SetTitle(SubMenu.LOC_POPUP_WARNING, MenuItem.COLOR);
      this.mPopup.SetMessage(SubMenu.LOC_POPUP_TERMSANDCONDITIONS, MenuItem.COLOR);
      Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) this.mPopup);
    }
    else if (!ParadoxUtils.IsValidEmail(this.mCachedEmail))
    {
      this.mPopup.SetTitle(SubMenu.LOC_POPUP_WARNING, MenuItem.COLOR);
      this.mPopup.SetMessage(SubMenu.LOC_POPUP_INVALID_EMAIL, MenuItem.COLOR);
      Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) this.mPopup);
    }
    else if (!ParadoxUtils.IsValidPassword(this.mCachedPassword))
    {
      this.mPopup.SetTitle(SubMenu.LOC_POPUP_WARNING, MenuItem.COLOR);
      this.mPopup.SetMessage(SubMenu.LOC_POPUP_INVALID_PASSWORD, MenuItem.COLOR);
      Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) this.mPopup);
    }
    else if (!ParadoxUtils.IsValidDoB(this.mCachedDateOfBirth))
    {
      this.mPopup.SetTitle(SubMenu.LOC_POPUP_WARNING, MenuItem.COLOR);
      this.mPopup.SetMessage(SubMenu.LOC_POPUP_INVALID_DATEOFBIRTH, MenuItem.COLOR);
      Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) this.mPopup);
    }
    else
    {
      this.mPopup.Clear();
      Singleton<ParadoxAccount>.Instance.CreatePlayerAccount(this.mCachedEmail, this.mCachedPassword, this.mCachedDateOfBirth, this.mCachedCountryCode, this.mNewsletterCheckBox.Checked, new ParadoxAccountSequence.ExecutionDoneDelegate(Tome.OnLoggedIn));
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
        this.mDateOfBirthMsgBox.Show(new Action<string>(this.OnDateOfBirthChanged));
        break;
      case 3:
        CountryCodeMessageBox.Instance.Show(this.mCachedCountry, new Action<string, string>(this.OnCountryChanged));
        break;
      case 4:
        this.mTermsCheckbox.Toggle();
        break;
      case 5:
        this.mNewsletterCheckBox.Toggle();
        break;
      case 6:
        SteamUtils.ActivateGameOverlayToWebPage("https://www.paradoxplaza.com/privacy");
        break;
      case 7:
        SteamUtils.ActivateGameOverlayToWebPage("https://www.paradoxplaza.com/terms-use");
        break;
      case 8:
        this.OnAccountCreateBtnClick();
        break;
      case 9:
        Tome.Instance.PopMenu();
        break;
    }
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    base.Draw(iLeftSide, iRightSide);
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.mAgreementTextItem.Draw(this.mEffect);
    this.mNewsletterTextItem.Draw(this.mEffect);
    this.DrawGraphics(SubMenu.sPagesTexture, SubMenuAccountCreate.TITLE_SEPERATOR_SRC, SubMenuAccountCreate.TITLE_SEPERATOR_DEST);
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
    this.mCachedDateOfBirth = string.Empty;
    this.mCachedCountry = RegionInfo.CurrentRegion.DisplayName;
    this.mCachedCountryCode = RegionInfo.CurrentRegion.TwoLetterISORegionName;
    this.mTermsCheckbox.Checked = false;
    this.mNewsletterCheckBox.Checked = false;
    this.mEmailTextItem.SetText(1, SubMenuAccountCreate.LOC_DEFAULT_EMAIL);
    this.mPasswordTextItem.SetText(1, SubMenuAccountCreate.LOC_DEFAULT_PASSWORD);
    this.mDateOfBirthTextItem.SetText(1, SubMenuAccountCreate.LOC_DEFAULT_DOB);
    this.mCountryTextItem.SetText(1, RegionInfo.CurrentRegion.DisplayName);
    this.mEmailTextItem.MarkAsDirty();
    this.mPasswordTextItem.MarkAsDirty();
    this.mDateOfBirthTextItem.MarkAsDirty();
    this.mCountryTextItem.MarkAsDirty();
    this.mAgreementTextItem.MarkAsDirty();
  }

  public override void OnExit()
  {
  }

  internal enum MenuItemId
  {
    None = -1, // 0xFFFFFFFF
    EmailAddress = 0,
    Password = 1,
    DateOfBirth = 2,
    Country = 3,
    TermsCheckbox = 4,
    NewsletterCheckbox = 5,
    Policy = 6,
    TermsAndConditions = 7,
    CreateAccount = 8,
    Back = 9,
  }
}
