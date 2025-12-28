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
using SteamWrapper;

namespace Magicka.GameLogic.GameStates.Menu.Main.Options
{
	// Token: 0x020000D3 RID: 211
	internal class SubMenuOptionsParadoxAccount : SubMenu
	{
		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x00025DE0 File Offset: 0x00023FE0
		public static SubMenuOptionsParadoxAccount Instance
		{
			get
			{
				if (SubMenuOptionsParadoxAccount.mSingelton == null)
				{
					lock (SubMenuOptionsParadoxAccount.mLock)
					{
						if (SubMenuOptionsParadoxAccount.mSingelton == null)
						{
							SubMenuOptionsParadoxAccount.mSingelton = new SubMenuOptionsParadoxAccount();
						}
					}
				}
				return SubMenuOptionsParadoxAccount.mSingelton;
			}
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x00025E34 File Offset: 0x00024034
		public SubMenuOptionsParadoxAccount()
		{
			this.mSelectedPosition = 2;
			this.mMenuItems = new List<MenuItem>();
			this.mMenuTitle = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_PARADOX_ACCOUNT));
			this.mSteamAccTextItem = this.AddMenuTextItem(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_STEAMID), SteamFriends.GetPersonaName()));
			this.mSteamAccTextItem.Enabled = false;
			this.mParadoxAccTextItem = this.AddMenuTextItem(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_PARADOXID), SubMenu.LOC_NONE));
			this.mParadoxAccTextItem.Enabled = false;
			this.mSteamTextItem = this.AddMenuTextItemBelowPrevious(Singleton<ParadoxAccount>.Instance.IsLinkedToSteam ? SubMenuOptionsParadoxAccount.LOC_UNLINK_STEAM : SubMenuOptionsParadoxAccount.LOC_LINK_STEAM, 60f);
			this.mLogoutTextItem = this.AddMenuTextItemBelowPrevious(Singleton<ParadoxAccount>.Instance.IsLoggedFull ? SubMenuOptionsParadoxAccount.LOC_LOGOUT : SubMenuOptionsParadoxAccount.LOC_NOT_LOGGED_IN, 0f);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, Vector2.Zero, SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
			this.mPopup = new MenuMessagePopup();
			this.mPopup.Alignment = (PopupAlign.Middle | PopupAlign.Center);
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x00025FA1 File Offset: 0x000241A1
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.OnEnter();
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x00025FB0 File Offset: 0x000241B0
		public override void ControllerUp(Controller iSender)
		{
			if (!this.mKeyboardSelection)
			{
				this.mSelectedPosition = 2;
				this.mKeyboardSelection = true;
				return;
			}
			this.mSelectedPosition--;
			if (this.mSelectedPosition < 2)
			{
				this.mSelectedPosition = this.mMenuItems.Count - 1;
			}
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x00026000 File Offset: 0x00024200
		public override void ControllerDown(Controller iSender)
		{
			if (!this.mKeyboardSelection)
			{
				this.mSelectedPosition = 2;
				this.mKeyboardSelection = true;
				return;
			}
			this.mSelectedPosition++;
			if (this.mSelectedPosition >= this.mMenuItems.Count)
			{
				this.mSelectedPosition = 2;
			}
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0002604C File Offset: 0x0002424C
		public override void ControllerA(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 2:
				Singleton<ParadoxAccount>.Instance.ToggleSteamLink(new ParadoxAccountSequence.ExecutionDoneDelegate(this.ToggleSteamLinkCallback));
				return;
			case 3:
				Singleton<ParadoxAccount>.Instance.LogOffPlayer(new ParadoxAccountSequence.ExecutionDoneDelegate(this.LogOutCallback));
				return;
			case 4:
				Tome.Instance.PopMenu();
				return;
			default:
				return;
			}
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x000260B0 File Offset: 0x000242B0
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			base.Draw(iLeftSide, iRightSide);
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			base.DrawGraphics(SubMenu.sPagesTexture, SubMenuOptionsParadoxAccount.TITLE_SEPERATOR_SRC, SubMenuOptionsParadoxAccount.TITLE_SEPERATOR_DEST);
			base.DrawGraphics(SubMenu.sPagesTexture, SubMenuOptionsParadoxAccount.MENU_IMAGERY_SRC, SubMenuOptionsParadoxAccount.MENU_IMAGERY_DEST);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x0002613B File Offset: 0x0002433B
		private void ShowErrorPopup(int iLoc)
		{
			this.mPopup.SetTitle(SubMenuOptionsParadoxAccount.LOC_WARNING, MenuItem.COLOR);
			this.mPopup.SetMessage(iLoc, MenuItem.COLOR);
			Singleton<PopupSystem>.Instance.AddPopupToQueue(this.mPopup);
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x00026173 File Offset: 0x00024373
		public void ToggleSteamLinkCallback(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode)
		{
			if (!iSuccess)
			{
				ParadoxPopupUtils.ShowErrorPopup(iErrorCode);
			}
			this.mSteamTextItem.SetText(Singleton<ParadoxAccount>.Instance.IsLinkedToSteam ? SubMenuOptionsParadoxAccount.LOC_UNLINK_STEAM : SubMenuOptionsParadoxAccount.LOC_LINK_STEAM);
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x000261A4 File Offset: 0x000243A4
		public void LogOutCallback(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode)
		{
			if (iSuccess)
			{
				this.mLogoutTextItem.Enabled = false;
				Singleton<ParadoxAccount>.Instance.Email = string.Empty;
				this.mParadoxAccTextItem.SetText(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_PARADOXID), LanguageManager.Instance.GetString(SubMenu.LOC_NONE)));
				Tome.Instance.PopMenu();
				return;
			}
			ParadoxPopupUtils.ShowErrorPopup(iErrorCode);
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x00026210 File Offset: 0x00024410
		public override void OnEnter()
		{
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, GamePadMenuHelp.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			this.mLogoutTextItem.Enabled = Singleton<ParadoxAccount>.Instance.IsLoggedFull;
			this.mSteamAccTextItem.SetText(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_STEAMID), SteamFriends.GetPersonaName()));
			this.mParadoxAccTextItem.SetText(string.Format(LanguageManager.Instance.GetString(SubMenuOptionsParadoxAccount.LOC_PARADOXID), Singleton<ParadoxAccount>.Instance.Email.Equals(string.Empty) ? LanguageManager.Instance.GetString(SubMenu.LOC_NONE) : Singleton<ParadoxAccount>.Instance.Email));
			this.mSteamTextItem.SetText(Singleton<ParadoxAccount>.Instance.IsLinkedToSteam ? SubMenuOptionsParadoxAccount.LOC_UNLINK_STEAM : SubMenuOptionsParadoxAccount.LOC_LINK_STEAM);
			LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x00026316 File Offset: 0x00024516
		public override void OnExit()
		{
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
			LanguageManager.Instance.LanguageChanged -= new Action(this.LanguageChanged);
		}

		// Token: 0x0400051D RID: 1309
		private const int TITLE_LEN = 32;

		// Token: 0x0400051E RID: 1310
		private const float TEXT_SPACING = 60f;

		// Token: 0x0400051F RID: 1311
		private static readonly int LOC_LINK_STEAM = "#menu_opt_14".GetHashCodeCustom();

		// Token: 0x04000520 RID: 1312
		private static readonly int LOC_UNLINK_STEAM = "#menu_opt_13".GetHashCodeCustom();

		// Token: 0x04000521 RID: 1313
		private static readonly int LOC_PROCESSING = "#menu_main_processing".GetHashCodeCustom();

		// Token: 0x04000522 RID: 1314
		private static readonly int LOC_NOT_LOGGED_IN = "#menu_main_notloggedin".GetHashCodeCustom();

		// Token: 0x04000523 RID: 1315
		private static readonly int LOC_LOGOUT = "#acc_log_out".GetHashCodeCustom();

		// Token: 0x04000524 RID: 1316
		private static readonly int LOC_PARADOX_ACCOUNT = "#paradox_account".GetHashCodeCustom();

		// Token: 0x04000525 RID: 1317
		private static readonly int LOC_WARNING = "#popup_warning".GetHashCodeCustom();

		// Token: 0x04000526 RID: 1318
		private static readonly int LOC_STEAMID = "#steam_id".GetHashCodeCustom();

		// Token: 0x04000527 RID: 1319
		private static readonly int LOC_PARADOXID = "#paradox_id".GetHashCodeCustom();

		// Token: 0x04000528 RID: 1320
		private static readonly Rectangle TITLE_SEPERATOR_SRC = new Rectangle(448, 976, 608, 48);

		// Token: 0x04000529 RID: 1321
		private static readonly Rectangle TITLE_SEPERATOR_DEST = new Rectangle(208, 180, 608, 48);

		// Token: 0x0400052A RID: 1322
		private static readonly Rectangle MENU_IMAGERY_SRC = new Rectangle(448, 768, 496, 208);

		// Token: 0x0400052B RID: 1323
		private static readonly Rectangle MENU_IMAGERY_DEST = new Rectangle(264, 700, 496, 208);

		// Token: 0x0400052C RID: 1324
		private static SubMenuOptionsParadoxAccount mSingelton;

		// Token: 0x0400052D RID: 1325
		private static volatile object mLock = new object();

		// Token: 0x0400052E RID: 1326
		private MenuTextItem mSteamTextItem;

		// Token: 0x0400052F RID: 1327
		private MenuTextItem mLogoutTextItem;

		// Token: 0x04000530 RID: 1328
		private MenuTextItem mSteamAccTextItem;

		// Token: 0x04000531 RID: 1329
		private MenuTextItem mParadoxAccTextItem;

		// Token: 0x04000532 RID: 1330
		private MenuMessagePopup mPopup;

		// Token: 0x020000D4 RID: 212
		internal enum MenuItemId
		{
			// Token: 0x04000534 RID: 1332
			SteamId,
			// Token: 0x04000535 RID: 1333
			ParadoxId,
			// Token: 0x04000536 RID: 1334
			LinkSteam,
			// Token: 0x04000537 RID: 1335
			Logout,
			// Token: 0x04000538 RID: 1336
			Back
		}
	}
}
