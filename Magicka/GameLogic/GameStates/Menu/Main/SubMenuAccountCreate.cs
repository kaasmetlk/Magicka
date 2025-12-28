using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x02000430 RID: 1072
	internal class SubMenuAccountCreate : SubMenu
	{
		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x06002140 RID: 8512 RVA: 0x000ECC10 File Offset: 0x000EAE10
		public static SubMenuAccountCreate Instance
		{
			get
			{
				if (SubMenuAccountCreate.mSingleton == null)
				{
					lock (SubMenuAccountCreate.mLock)
					{
						if (SubMenuAccountCreate.mSingleton == null)
						{
							SubMenuAccountCreate.mSingleton = new SubMenuAccountCreate();
						}
					}
				}
				return SubMenuAccountCreate.mSingleton;
			}
		}

		// Token: 0x06002141 RID: 8513 RVA: 0x000ECC64 File Offset: 0x000EAE64
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

		// Token: 0x06002142 RID: 8514 RVA: 0x000ECCC8 File Offset: 0x000EAEC8
		private SubMenuAccountCreate()
		{
			this.mMenuItems = new List<MenuItem>();
			this.mMenuTitle = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuAccountCreate.LOC_CREATEACCOUNT_TITLE));
			this.mEmailMsgBox = new TextInputMessageBox(SubMenu.LOC_EMAIL, 128);
			this.mPasswordMsgBox = new TextInputMessageBox(SubMenu.LOC_PASSWORD, 128);
			this.mPasswordMsgBox.SecureMode = true;
			this.mDateOfBirthMsgBox = new TextInputMessageBox(SubMenu.LOC_DATEOFBIRTH, 10);
			this.mDateOfBirthMsgBox.InputType = TextInputMessageBox.MessageBoxInputType.Date;
			this.mEmailTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_EMAIL, MagickaFont.MenuOption, 10f);
			this.mEmailTextItem.AddNewLine(SubMenu.LOC_EMAIL, TextAlign.Center, MenuItem.COLOR);
			this.mEmailTextItem.AddNewLine(SubMenuAccountCreate.LOC_DEFAULT_EMAIL, TextAlign.Center, MenuItem.COLOR_DISABLED);
			this.mMenuItems.Add(this.mEmailTextItem);
			this.mPasswordTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_PASSWORD, MagickaFont.MenuOption, 10f);
			this.mPasswordTextItem.AddNewLine(SubMenu.LOC_PASSWORD, TextAlign.Center, MenuItem.COLOR);
			this.mPasswordTextItem.AddNewLine(SubMenuAccountCreate.LOC_DEFAULT_PASSWORD, TextAlign.Center, MenuItem.COLOR_DISABLED);
			this.mMenuItems.Add(this.mPasswordTextItem);
			this.mDateOfBirthTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_DATEOFBIRTH, MagickaFont.MenuOption, 10f);
			this.mDateOfBirthTextItem.AddNewLine(SubMenu.LOC_DATEOFBIRTH, TextAlign.Center, MenuItem.COLOR);
			this.mDateOfBirthTextItem.AddNewLine(SubMenuAccountCreate.LOC_DEFAULT_DOB, TextAlign.Center, MenuItem.COLOR_DISABLED);
			this.mMenuItems.Add(this.mDateOfBirthTextItem);
			this.mCountryTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_COUNTRY, MagickaFont.MenuOption, 10f);
			this.mCountryTextItem.AddNewLine(SubMenuAccountCreate.LOC_COUNTRY, TextAlign.Center, MenuItem.COLOR);
			this.mCountryTextItem.AddNewLine(RegionInfo.CurrentRegion.DisplayName, TextAlign.Center, MenuItem.COLOR_DISABLED);
			this.mMenuItems.Add(this.mCountryTextItem);
			this.mAgreementTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_LEGALS, MagickaFont.MenuOption);
			this.mAgreementTextItem.AddLines(SubMenuAccountCreate.LOC_NOTICE, TextAlign.Center, MenuItem.COLOR, 800);
			this.mTermsCheckbox = new CheckBox(false);
			this.mTermsCheckbox.Position = this.mAgreementTextItem.Position + new Vector2(this.mAgreementTextItem.Size.X * 0.5f + 35f, 0f);
			this.mTermsCheckbox.Width = (this.mTermsCheckbox.Height = 32);
			this.mMenuItems.Add(this.mTermsCheckbox);
			this.mNewsletterTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountCreate.POS_NEWSLETTER, MagickaFont.MenuOption);
			this.mNewsletterTextItem.AddLines(SubMenuAccountCreate.LOC_NEWSLETTER, TextAlign.Center, MenuItem.COLOR, 800);
			this.mNewsletterCheckBox = new CheckBox(false);
			this.mNewsletterCheckBox.Position = this.mNewsletterTextItem.Position + new Vector2(this.mNewsletterTextItem.Size.X * 0.5f + 35f, -16f);
			this.mNewsletterCheckBox.Width = (this.mTermsCheckbox.Height = 32);
			this.mMenuItems.Add(this.mNewsletterCheckBox);
			this.mPolicyButton = this.CreateMenuTextButton(SubMenuAccountCreate.LOC_PRIVACYPOLICY, 200f);
			this.mPolicyButton.Position = this.mPosition + SubMenuAccountCreate.POS_BTN_POLICY - new Vector2(this.mPolicyButton.RealWidth * 0.5f + 40f, 0f);
			this.mMenuItems.Add(this.mPolicyButton);
			this.mTermsButton = this.CreateMenuTextButton(SubMenuAccountCreate.LOC_TERMSANDCONDITIONS, 200f);
			this.mTermsButton.Position = this.mPosition + SubMenuAccountCreate.POS_BTN_TERMS + new Vector2(this.mTermsButton.RealWidth * 0.5f + 40f, 0f);
			this.mMenuItems.Add(this.mTermsButton);
			this.mCreateButton = this.CreateMenuTextButton(SubMenuAccountCreate.LOC_BTN_CREATEACCOUNT, 200f);
			this.mCreateButton.Position = new Vector2(this.mTermsButton.Position.X + this.mTermsButton.RealWidth * 0.5f - this.mCreateButton.RealWidth * 0.5f, (float)Tome.PAGERIGHTSHEET.Y - SubMenu.BACK_SIZE.Y + 25f);
			this.mMenuItems.Add(this.mCreateButton);
			this.mBackButton = new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, Vector2.Zero, SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE);
			this.mMenuItems.Add(this.mBackButton);
			this.mPopup = new MenuMessagePopup();
			this.mPopup.Alignment = (PopupAlign.Middle | PopupAlign.Center);
		}

		// Token: 0x06002143 RID: 8515 RVA: 0x000ED244 File Offset: 0x000EB444
		public void OnEmailAddressChanged(string iName)
		{
			this.mCachedEmail = iName;
			if (this.mCachedEmail == string.Empty)
			{
				this.mEmailTextItem.SetText(1, SubMenuAccountCreate.LOC_DEFAULT_EMAIL);
				return;
			}
			string text = this.mCachedEmail;
			if (text.Length > 40)
			{
				text = text.Substring(0, 40) + "...";
			}
			this.mEmailTextItem.SetText(1, text);
			if (this.mCachedPassword == string.Empty)
			{
				this.mSelectedPosition = 1;
				this.mPasswordMsgBox.Show(new Action<string>(this.OnPasswordChanged));
			}
		}

		// Token: 0x06002144 RID: 8516 RVA: 0x000ED2E0 File Offset: 0x000EB4E0
		public void OnPasswordChanged(string iPassword)
		{
			this.mCachedPassword = iPassword;
			if (this.mCachedPassword == string.Empty)
			{
				this.mPasswordTextItem.SetText(1, SubMenuAccountCreate.LOC_DEFAULT_PASSWORD);
				return;
			}
			this.mPasswordTextItem.SetText(1, new string('*', (iPassword.Length > 40) ? 40 : iPassword.Length));
			if (this.mCachedDateOfBirth == string.Empty)
			{
				this.mSelectedPosition = 2;
				this.mDateOfBirthMsgBox.Show(new Action<string>(this.OnDateOfBirthChanged));
			}
		}

		// Token: 0x06002145 RID: 8517 RVA: 0x000ED370 File Offset: 0x000EB570
		public void OnDateOfBirthChanged(string iDoB)
		{
			this.mCachedDateOfBirth = iDoB;
			if (this.mCachedDateOfBirth == string.Empty)
			{
				this.mDateOfBirthTextItem.SetText(1, SubMenuAccountCreate.LOC_DEFAULT_DOB);
				return;
			}
			this.mDateOfBirthTextItem.SetText(1, this.mCachedDateOfBirth);
			this.mSelectedPosition = 3;
		}

		// Token: 0x06002146 RID: 8518 RVA: 0x000ED3C1 File Offset: 0x000EB5C1
		public void OnCountryChanged(string iCountry, string iCode)
		{
			this.mCachedCountry = iCountry;
			this.mCachedCountryCode = iCode;
			this.mCountryTextItem.SetText(1, iCountry);
		}

		// Token: 0x06002147 RID: 8519 RVA: 0x000ED3E0 File Offset: 0x000EB5E0
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
				Singleton<PopupSystem>.Instance.AddPopupToQueue(this.mPopup);
				return;
			}
			if (!ParadoxUtils.IsValidEmail(this.mCachedEmail))
			{
				this.mPopup.SetTitle(SubMenu.LOC_POPUP_WARNING, MenuItem.COLOR);
				this.mPopup.SetMessage(SubMenu.LOC_POPUP_INVALID_EMAIL, MenuItem.COLOR);
				Singleton<PopupSystem>.Instance.AddPopupToQueue(this.mPopup);
				return;
			}
			if (!ParadoxUtils.IsValidPassword(this.mCachedPassword))
			{
				this.mPopup.SetTitle(SubMenu.LOC_POPUP_WARNING, MenuItem.COLOR);
				this.mPopup.SetMessage(SubMenu.LOC_POPUP_INVALID_PASSWORD, MenuItem.COLOR);
				Singleton<PopupSystem>.Instance.AddPopupToQueue(this.mPopup);
				return;
			}
			if (!ParadoxUtils.IsValidDoB(this.mCachedDateOfBirth))
			{
				this.mPopup.SetTitle(SubMenu.LOC_POPUP_WARNING, MenuItem.COLOR);
				this.mPopup.SetMessage(SubMenu.LOC_POPUP_INVALID_DATEOFBIRTH, MenuItem.COLOR);
				Singleton<PopupSystem>.Instance.AddPopupToQueue(this.mPopup);
				return;
			}
			this.mPopup.Clear();
			Singleton<ParadoxAccount>.Instance.CreatePlayerAccount(this.mCachedEmail, this.mCachedPassword, this.mCachedDateOfBirth, this.mCachedCountryCode, this.mNewsletterCheckBox.Checked, new ParadoxAccountSequence.ExecutionDoneDelegate(Tome.OnLoggedIn));
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x000ED578 File Offset: 0x000EB778
		public override void ControllerUp(Controller iSender)
		{
			if (!this.mKeyboardSelection)
			{
				this.mSelectedPosition = 0;
				this.mKeyboardSelection = true;
				return;
			}
			this.mSelectedPosition--;
			if (this.mSelectedPosition < 0)
			{
				this.mSelectedPosition = this.mMenuItems.Count - 1;
			}
		}

		// Token: 0x06002149 RID: 8521 RVA: 0x000ED5C8 File Offset: 0x000EB7C8
		public override void ControllerDown(Controller iSender)
		{
			if (!this.mKeyboardSelection)
			{
				this.mSelectedPosition = 0;
				this.mKeyboardSelection = true;
				return;
			}
			this.mSelectedPosition++;
			if (this.mSelectedPosition >= this.mMenuItems.Count)
			{
				this.mSelectedPosition = 0;
			}
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x000ED614 File Offset: 0x000EB814
		public override void ControllerA(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 0:
				this.mEmailMsgBox.Show(new Action<string>(this.OnEmailAddressChanged));
				return;
			case 1:
				this.mPasswordMsgBox.Show(new Action<string>(this.OnPasswordChanged));
				return;
			case 2:
				this.mDateOfBirthMsgBox.Show(new Action<string>(this.OnDateOfBirthChanged));
				return;
			case 3:
				CountryCodeMessageBox.Instance.Show(this.mCachedCountry, new Action<string, string>(this.OnCountryChanged));
				return;
			case 4:
				this.mTermsCheckbox.Toggle();
				return;
			case 5:
				this.mNewsletterCheckBox.Toggle();
				return;
			case 6:
				SteamUtils.ActivateGameOverlayToWebPage("https://www.paradoxplaza.com/privacy");
				return;
			case 7:
				SteamUtils.ActivateGameOverlayToWebPage("https://www.paradoxplaza.com/terms-use");
				return;
			case 8:
				this.OnAccountCreateBtnClick();
				return;
			case 9:
				Tome.Instance.PopMenu();
				return;
			default:
				return;
			}
		}

		// Token: 0x0600214B RID: 8523 RVA: 0x000ED6FC File Offset: 0x000EB8FC
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			base.Draw(iLeftSide, iRightSide);
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.mAgreementTextItem.Draw(this.mEffect);
			this.mNewsletterTextItem.Draw(this.mEffect);
			base.DrawGraphics(SubMenu.sPagesTexture, SubMenuAccountCreate.TITLE_SEPERATOR_SRC, SubMenuAccountCreate.TITLE_SEPERATOR_DEST);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x0600214C RID: 8524 RVA: 0x000ED794 File Offset: 0x000EB994
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

		// Token: 0x0600214D RID: 8525 RVA: 0x000ED8B0 File Offset: 0x000EBAB0
		public override void OnExit()
		{
		}

		// Token: 0x040023DD RID: 9181
		private const int BUTTON_SPACING = 40;

		// Token: 0x040023DE RID: 9182
		private const float BTN_MIN_WIDTH = 200f;

		// Token: 0x040023DF RID: 9183
		private const int CHECKBOX_SIZE = 32;

		// Token: 0x040023E0 RID: 9184
		private const float CHECKBOX_PADDING = 35f;

		// Token: 0x040023E1 RID: 9185
		private const float BTN_CREATE_OFFSET = 25f;

		// Token: 0x040023E2 RID: 9186
		private const int MAX_VISUAL_LENGTH = 40;

		// Token: 0x040023E3 RID: 9187
		private const string ELLIPSIS = "...";

		// Token: 0x040023E4 RID: 9188
		private const string PARADOX_PRIVACY_URL = "https://www.paradoxplaza.com/privacy";

		// Token: 0x040023E5 RID: 9189
		private const string PARADOX_TERMS_URL = "https://www.paradoxplaza.com/terms-use";

		// Token: 0x040023E6 RID: 9190
		private static readonly int LOC_CREATEACCOUNT_TITLE = "#acc_create_acc_title".GetHashCodeCustom();

		// Token: 0x040023E7 RID: 9191
		private static readonly int LOC_BTN_CREATEACCOUNT = "#acc_create_acc".GetHashCodeCustom();

		// Token: 0x040023E8 RID: 9192
		private static readonly int LOC_PRIVACYPOLICY = "#privacy_policy".GetHashCodeCustom();

		// Token: 0x040023E9 RID: 9193
		private static readonly int LOC_TERMSANDCONDITIONS = "#terms_conditions".GetHashCodeCustom();

		// Token: 0x040023EA RID: 9194
		private static readonly int LOC_NOTICE = "#acc_notice".GetHashCodeCustom();

		// Token: 0x040023EB RID: 9195
		private static readonly int LOC_NEWSLETTER = "#acc_newsletter_subscribe".GetHashCodeCustom();

		// Token: 0x040023EC RID: 9196
		private static readonly int LOC_COUNTRY = "#choose_your_country".GetHashCodeCustom();

		// Token: 0x040023ED RID: 9197
		private static readonly int LOC_DEFAULT_EMAIL = "#acc_default_email".GetHashCodeCustom();

		// Token: 0x040023EE RID: 9198
		private static readonly int LOC_DEFAULT_PASSWORD = "#acc_default_password".GetHashCodeCustom();

		// Token: 0x040023EF RID: 9199
		private static readonly int LOC_DEFAULT_DOB = "#acc_default_dob".GetHashCodeCustom();

		// Token: 0x040023F0 RID: 9200
		private static readonly Rectangle TITLE_SEPERATOR_SRC = new Rectangle(448, 976, 608, 48);

		// Token: 0x040023F1 RID: 9201
		private static readonly Rectangle TITLE_SEPERATOR_DEST = new Rectangle(208, 180, 608, 48);

		// Token: 0x040023F2 RID: 9202
		private static readonly Rectangle MENU_IMAGERY_SRC = new Rectangle(448, 768, 496, 208);

		// Token: 0x040023F3 RID: 9203
		private static readonly Rectangle MENU_IMAGERY_DEST = new Rectangle(264, 700, 496, 208);

		// Token: 0x040023F4 RID: 9204
		private static readonly Vector2 POS_EMAIL = new Vector2(0f, -65f);

		// Token: 0x040023F5 RID: 9205
		private static readonly Vector2 POS_PASSWORD = new Vector2(0f, 30f);

		// Token: 0x040023F6 RID: 9206
		private static readonly Vector2 POS_DATEOFBIRTH = new Vector2(0f, 135f);

		// Token: 0x040023F7 RID: 9207
		private static readonly Vector2 POS_COUNTRY = new Vector2(0f, 240f);

		// Token: 0x040023F8 RID: 9208
		private static readonly Vector2 POS_LEGALS = new Vector2(0f, 350f);

		// Token: 0x040023F9 RID: 9209
		private static readonly Vector2 POS_NEWSLETTER = new Vector2(0f, 450f);

		// Token: 0x040023FA RID: 9210
		private static readonly Vector2 POS_BTN_POLICY = new Vector2(0f, 525f);

		// Token: 0x040023FB RID: 9211
		private static readonly Vector2 POS_BTN_TERMS = new Vector2(0f, 525f);

		// Token: 0x040023FC RID: 9212
		private static SubMenuAccountCreate mSingleton = null;

		// Token: 0x040023FD RID: 9213
		private static volatile object mLock = new object();

		// Token: 0x040023FE RID: 9214
		private TextInputMessageBox mEmailMsgBox;

		// Token: 0x040023FF RID: 9215
		private TextInputMessageBox mPasswordMsgBox;

		// Token: 0x04002400 RID: 9216
		private TextInputMessageBox mDateOfBirthMsgBox;

		// Token: 0x04002401 RID: 9217
		private MenuItemMultiLineText mEmailTextItem;

		// Token: 0x04002402 RID: 9218
		private MenuItemMultiLineText mPasswordTextItem;

		// Token: 0x04002403 RID: 9219
		private MenuItemMultiLineText mDateOfBirthTextItem;

		// Token: 0x04002404 RID: 9220
		private MenuItemMultiLineText mCountryTextItem;

		// Token: 0x04002405 RID: 9221
		private MenuItemMultiLineText mAgreementTextItem;

		// Token: 0x04002406 RID: 9222
		private MenuItemMultiLineText mNewsletterTextItem;

		// Token: 0x04002407 RID: 9223
		private CheckBox mTermsCheckbox;

		// Token: 0x04002408 RID: 9224
		private CheckBox mNewsletterCheckBox;

		// Token: 0x04002409 RID: 9225
		private string mCachedEmail = string.Empty;

		// Token: 0x0400240A RID: 9226
		private string mCachedPassword = string.Empty;

		// Token: 0x0400240B RID: 9227
		private string mCachedDateOfBirth = string.Empty;

		// Token: 0x0400240C RID: 9228
		private string mCachedCountry = string.Empty;

		// Token: 0x0400240D RID: 9229
		private string mCachedCountryCode = string.Empty;

		// Token: 0x0400240E RID: 9230
		private MenuMessagePopup mPopup;

		// Token: 0x0400240F RID: 9231
		private MenuTextButtonItem mTermsButton;

		// Token: 0x04002410 RID: 9232
		private MenuTextButtonItem mPolicyButton;

		// Token: 0x04002411 RID: 9233
		private MenuTextButtonItem mCreateButton;

		// Token: 0x04002412 RID: 9234
		private MenuItem mBackButton;

		// Token: 0x02000431 RID: 1073
		internal enum MenuItemId
		{
			// Token: 0x04002414 RID: 9236
			None = -1,
			// Token: 0x04002415 RID: 9237
			EmailAddress,
			// Token: 0x04002416 RID: 9238
			Password,
			// Token: 0x04002417 RID: 9239
			DateOfBirth,
			// Token: 0x04002418 RID: 9240
			Country,
			// Token: 0x04002419 RID: 9241
			TermsCheckbox,
			// Token: 0x0400241A RID: 9242
			NewsletterCheckbox,
			// Token: 0x0400241B RID: 9243
			Policy,
			// Token: 0x0400241C RID: 9244
			TermsAndConditions,
			// Token: 0x0400241D RID: 9245
			CreateAccount,
			// Token: 0x0400241E RID: 9246
			Back
		}
	}
}
