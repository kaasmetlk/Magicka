using System;
using System.Collections.Generic;
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

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x02000516 RID: 1302
	internal class SubMenuAccountLogin : SubMenu
	{
		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x0600272B RID: 10027 RVA: 0x0011D488 File Offset: 0x0011B688
		public static SubMenuAccountLogin Instance
		{
			get
			{
				if (SubMenuAccountLogin.mSingleton == null)
				{
					lock (SubMenuAccountLogin.mLock)
					{
						if (SubMenuAccountLogin.mSingleton == null)
						{
							SubMenuAccountLogin.mSingleton = new SubMenuAccountLogin();
						}
					}
				}
				return SubMenuAccountLogin.mSingleton;
			}
		}

		// Token: 0x0600272C RID: 10028 RVA: 0x0011D4DC File Offset: 0x0011B6DC
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuAccountLogin.LOC_LOGIN_TITLE));
			this.mEmailMsgBox.LanguageChanged();
			this.mPasswordMsgBox.LanguageChanged();
		}

		// Token: 0x0600272D RID: 10029 RVA: 0x0011D514 File Offset: 0x0011B714
		private SubMenuAccountLogin()
		{
			this.mMenuItems = new List<MenuItem>();
			this.mMenuTitle = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuAccountLogin.LOC_LOGIN_TITLE));
			this.mEmailMsgBox = new TextInputMessageBox(SubMenu.LOC_EMAIL, 128);
			this.mPasswordMsgBox = new TextInputMessageBox(SubMenu.LOC_PASSWORD, 128);
			this.mPasswordMsgBox.SecureMode = true;
			this.mEmailTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountLogin.EMAIL_POSITION, MagickaFont.MenuOption, 10f);
			this.mEmailTextItem.AddNewLine(SubMenu.LOC_EMAIL, TextAlign.Center, MenuItem.COLOR);
			this.mEmailTextItem.AddNewLine(SubMenuAccountLogin.LOC_DEFAULT_EMAIL, TextAlign.Center, MenuItem.COLOR_DISABLED);
			this.mMenuItems.Add(this.mEmailTextItem);
			this.mPasswordTextItem = new MenuItemMultiLineText(this.mPosition + SubMenuAccountLogin.PASSWORD_POSITION, MagickaFont.MenuOption, 10f);
			this.mPasswordTextItem.AddNewLine(SubMenu.LOC_PASSWORD, TextAlign.Center, MenuItem.COLOR);
			this.mPasswordTextItem.AddNewLine(SubMenuAccountLogin.LOC_DEFAULT_PASSWORD, TextAlign.Center, MenuItem.COLOR_DISABLED);
			this.mMenuItems.Add(this.mPasswordTextItem);
			this.mLogInButton = this.CreateMenuTextButton(SubMenuAccountLogin.LOC_LOGIN_BTN, 260f);
			this.mLogInButton.Position = this.mPosition + SubMenuAccountLogin.LOGIN_POSITION;
			this.mMenuItems.Add(this.mLogInButton);
			this.mBackButton = new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, Vector2.Zero, SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE);
			this.mMenuItems.Add(this.mBackButton);
			this.mPopup = new MenuMessagePopup();
			this.mPopup.Alignment = (PopupAlign.Middle | PopupAlign.Center);
		}

		// Token: 0x0600272E RID: 10030 RVA: 0x0011D718 File Offset: 0x0011B918
		public void OnEmailAddressChanged(string iName)
		{
			this.mCachedEmail = iName;
			if (this.mCachedEmail == string.Empty)
			{
				this.mEmailTextItem.SetText(1, SubMenuAccountLogin.LOC_DEFAULT_EMAIL);
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

		// Token: 0x0600272F RID: 10031 RVA: 0x0011D7B4 File Offset: 0x0011B9B4
		public void OnPasswordChanged(string iPassword)
		{
			this.mCachedPassword = iPassword;
			if (this.mCachedPassword == string.Empty)
			{
				this.mPasswordTextItem.SetText(1, SubMenuAccountLogin.LOC_DEFAULT_PASSWORD);
				return;
			}
			this.mPasswordTextItem.SetText(1, new string('*', (iPassword.Length > 40) ? 40 : iPassword.Length));
			this.mSelectedPosition = 2;
		}

		// Token: 0x06002730 RID: 10032 RVA: 0x0011D81C File Offset: 0x0011BA1C
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
				Singleton<PopupSystem>.Instance.AddPopupToQueue(this.mPopup);
				return;
			}
			this.mPopup.Clear();
			Singleton<ParadoxAccount>.Instance.LoginPlayer(this.mCachedEmail, this.mCachedPassword, new ParadoxAccountSequence.ExecutionDoneDelegate(Tome.OnLoggedIn));
		}

		// Token: 0x06002731 RID: 10033 RVA: 0x0011D8C8 File Offset: 0x0011BAC8
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

		// Token: 0x06002732 RID: 10034 RVA: 0x0011D918 File Offset: 0x0011BB18
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

		// Token: 0x06002733 RID: 10035 RVA: 0x0011D964 File Offset: 0x0011BB64
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
				this.OnLoginBtnClick();
				return;
			case 3:
				Tome.Instance.PopMenu();
				return;
			default:
				return;
			}
		}

		// Token: 0x06002734 RID: 10036 RVA: 0x0011D9D0 File Offset: 0x0011BBD0
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			base.Draw(iLeftSide, iRightSide);
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			base.DrawGraphics(SubMenu.sPagesTexture, SubMenuAccountLogin.TITLE_SEPERATOR_SRC, SubMenuAccountLogin.TITLE_SEPERATOR_DEST);
			base.DrawGraphics(SubMenu.sPagesTexture, SubMenuAccountLogin.MENU_IMAGERY_SRC, SubMenuAccountLogin.MENU_IMAGERY_DEST);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06002735 RID: 10037 RVA: 0x0011DA5C File Offset: 0x0011BC5C
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

		// Token: 0x06002736 RID: 10038 RVA: 0x0011DAED File Offset: 0x0011BCED
		public override void OnExit()
		{
		}

		// Token: 0x04002A55 RID: 10837
		private const float BTN_WIDTH = 260f;

		// Token: 0x04002A56 RID: 10838
		private const int MAX_VISUAL_LENGTH = 40;

		// Token: 0x04002A57 RID: 10839
		private const string ELLIPSIS = "...";

		// Token: 0x04002A58 RID: 10840
		private static readonly int LOC_LOGIN_TITLE = "#acc_log_in_title".GetHashCodeCustom();

		// Token: 0x04002A59 RID: 10841
		private static readonly int LOC_LOGIN_BTN = "#acc_log_in".GetHashCodeCustom();

		// Token: 0x04002A5A RID: 10842
		private static readonly int LOC_DEFAULT_EMAIL = "#acc_default_email".GetHashCodeCustom();

		// Token: 0x04002A5B RID: 10843
		private static readonly int LOC_DEFAULT_PASSWORD = "#acc_default_password".GetHashCodeCustom();

		// Token: 0x04002A5C RID: 10844
		private static readonly Rectangle TITLE_SEPERATOR_SRC = new Rectangle(448, 976, 608, 48);

		// Token: 0x04002A5D RID: 10845
		private static readonly Rectangle TITLE_SEPERATOR_DEST = new Rectangle(208, 220, 608, 48);

		// Token: 0x04002A5E RID: 10846
		private static readonly Rectangle MENU_IMAGERY_SRC = new Rectangle(448, 768, 496, 208);

		// Token: 0x04002A5F RID: 10847
		private static readonly Rectangle MENU_IMAGERY_DEST = new Rectangle(264, 700, 496, 208);

		// Token: 0x04002A60 RID: 10848
		private static readonly Vector2 EMAIL_POSITION = Vector2.Zero;

		// Token: 0x04002A61 RID: 10849
		private static readonly Vector2 PASSWORD_POSITION = new Vector2(0f, 140f);

		// Token: 0x04002A62 RID: 10850
		private static readonly Vector2 LOGIN_POSITION = new Vector2(0f, 300f);

		// Token: 0x04002A63 RID: 10851
		private static SubMenuAccountLogin mSingleton;

		// Token: 0x04002A64 RID: 10852
		private static volatile object mLock = new object();

		// Token: 0x04002A65 RID: 10853
		private TextInputMessageBox mEmailMsgBox;

		// Token: 0x04002A66 RID: 10854
		private TextInputMessageBox mPasswordMsgBox;

		// Token: 0x04002A67 RID: 10855
		private MenuItemMultiLineText mEmailTextItem;

		// Token: 0x04002A68 RID: 10856
		private MenuItemMultiLineText mPasswordTextItem;

		// Token: 0x04002A69 RID: 10857
		private string mCachedEmail = string.Empty;

		// Token: 0x04002A6A RID: 10858
		private string mCachedPassword = string.Empty;

		// Token: 0x04002A6B RID: 10859
		private MenuTextButtonItem mLogInButton;

		// Token: 0x04002A6C RID: 10860
		private MenuItem mBackButton;

		// Token: 0x04002A6D RID: 10861
		private MenuMessagePopup mPopup;

		// Token: 0x02000517 RID: 1303
		internal enum MenuItemId
		{
			// Token: 0x04002A6F RID: 10863
			None = -1,
			// Token: 0x04002A70 RID: 10864
			EmailAddress,
			// Token: 0x04002A71 RID: 10865
			Password,
			// Token: 0x04002A72 RID: 10866
			Login,
			// Token: 0x04002A73 RID: 10867
			Back
		}
	}
}
