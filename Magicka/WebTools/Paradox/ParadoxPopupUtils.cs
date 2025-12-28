using System;
using Magicka.CoreFramework;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.UI.Popup;
using Magicka.Misc;

namespace Magicka.WebTools.Paradox
{
	// Token: 0x020004ED RID: 1261
	public static class ParadoxPopupUtils
	{
		// Token: 0x06002551 RID: 9553 RVA: 0x0010F8EC File Offset: 0x0010DAEC
		static ParadoxPopupUtils()
		{
			ParadoxPopupUtils.sPopup = new MenuMessagePopup();
			ParadoxPopupUtils.sPopup.Alignment = (PopupAlign.Middle | PopupAlign.Center);
		}

		// Token: 0x06002552 RID: 9554 RVA: 0x0010F998 File Offset: 0x0010DB98
		public static void ShowErrorPopup(ParadoxAccount.ErrorCode iErrorCode)
		{
			if (iErrorCode > ParadoxAccount.ErrorCode.Login_AccountLoginFailedUnknown)
			{
				if (iErrorCode <= ParadoxAccount.ErrorCode.Logout_AuthTokenInvalidationFailed)
				{
					if (iErrorCode == ParadoxAccount.ErrorCode.Login_GameSparksAuthenticationFailed)
					{
						goto IL_FE;
					}
					if (iErrorCode != ParadoxAccount.ErrorCode.Logout_AuthTokenInvalidationFailed)
					{
						goto IL_134;
					}
				}
				else
				{
					switch (iErrorCode)
					{
					case ParadoxAccount.ErrorCode.SteamLink_LinkAlreadyExistWithAnotherAccount:
					case ParadoxAccount.ErrorCode.SteamLink_LinkFailed:
						break;
					default:
						if (iErrorCode != ParadoxAccount.ErrorCode.SteamUnlink_UnlinkFailed)
						{
							goto IL_134;
						}
						break;
					}
				}
				ParadoxPopupUtils.ShowErrorPopupWithExtra(ParadoxPopupUtils.LOC_UNKNOWN_ERROR, string.Format("({0})", (int)iErrorCode));
				return;
			}
			if (iErrorCode != ParadoxAccount.ErrorCode.Startup_FailedLoginWithSteamOrAuthToken)
			{
				switch (iErrorCode)
				{
				case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidEmail:
				case ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedInvalidEmail:
					ParadoxPopupUtils.ShowErrorPopup(ParadoxPopupUtils.LOC_INVALID_EMAIL);
					return;
				case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedBadPasswordLength:
				case ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedBadPasswordLength:
					ParadoxPopupUtils.ShowErrorPopup(ParadoxPopupUtils.LOC_BAD_PASSWORD_LENGTH);
					return;
				case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedAlreadyExists:
				case ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedAlreadyExists:
					ParadoxPopupUtils.ShowErrorPopup(ParadoxPopupUtils.LOC_ACCOUNT_EXISTS);
					return;
				case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidLanguage:
				case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidCountry:
				case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidDoB:
				case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedUnder13:
				case ParadoxAccount.ErrorCode.AccountCreate_DetailsUpdateFailedInvalidLanguage:
				case ParadoxAccount.ErrorCode.AccountCreate_DetailsUpdateFailedInvalidCountry:
				case ParadoxAccount.ErrorCode.AccountCreate_DetailsUpdateFailedInvalidDoB:
				case ParadoxAccount.ErrorCode.AccountCreate_DetailsUpdateFailedUnknown:
					goto IL_134;
				case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedUnknown:
				case ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedUnknown:
				case ParadoxAccount.ErrorCode.AccountCreate_GameSparksAuthenticationFailed:
				case ParadoxAccount.ErrorCode.AccountCreate_GameSparksRegistrationFailed:
				case ParadoxAccount.ErrorCode.AccountCreate_GameSparksUnknownFailure:
					ParadoxPopupUtils.ShowErrorPopupWithExtra(ParadoxPopupUtils.LOC_CREATION_FAILED, string.Format("({0})", (int)iErrorCode));
					return;
				case ParadoxAccount.ErrorCode.AccountCreate_CreatedButNotLoggedIn:
					break;
				default:
					switch (iErrorCode)
					{
					case ParadoxAccount.ErrorCode.Login_AccountLoginFailedNotAuthorized:
					case ParadoxAccount.ErrorCode.Login_AccountLoginFailedUnknown:
						break;
					default:
						goto IL_134;
					}
					break;
				}
			}
			IL_FE:
			ParadoxPopupUtils.ShowErrorPopupWithExtra(ParadoxPopupUtils.LOC_LOGIN_FAILED, string.Format("({0})", (int)iErrorCode));
			return;
			IL_134:
			Logger.LogWarning(Logger.Source.ParadoxPopupUtils, "Error code not supported, no popup to display.");
		}

		// Token: 0x06002553 RID: 9555 RVA: 0x0010FAE8 File Offset: 0x0010DCE8
		private static void ShowErrorPopup(int iLocMessage)
		{
			lock (ParadoxPopupUtils.sPopupLock)
			{
				if (ParadoxPopupUtils.sPopup != null)
				{
					ParadoxPopupUtils.sPopup.SetTitle(ParadoxPopupUtils.LOC_POPUP_WARNING, MenuItem.COLOR);
					ParadoxPopupUtils.sPopup.SetMessage(iLocMessage, MenuItem.COLOR);
					Singleton<PopupSystem>.Instance.AddPopupToQueue(ParadoxPopupUtils.sPopup);
				}
			}
		}

		// Token: 0x06002554 RID: 9556 RVA: 0x0010FB54 File Offset: 0x0010DD54
		private static void ShowErrorPopup(string iLocTitle, string iLocMessage)
		{
			lock (ParadoxPopupUtils.sPopupLock)
			{
				if (ParadoxPopupUtils.sPopup != null)
				{
					ParadoxPopupUtils.sPopup.SetTitle(iLocTitle, MenuItem.COLOR);
					ParadoxPopupUtils.sPopup.SetMessage(iLocMessage, MenuItem.COLOR);
					Singleton<PopupSystem>.Instance.AddPopupToQueue(ParadoxPopupUtils.sPopup);
				}
			}
		}

		// Token: 0x06002555 RID: 9557 RVA: 0x0010FBBC File Offset: 0x0010DDBC
		private static void ShowErrorPopupWithExtra(int iLocMessage, string iExtra)
		{
			lock (ParadoxPopupUtils.sPopupLock)
			{
				if (ParadoxPopupUtils.sPopup != null)
				{
					ParadoxPopupUtils.sPopup.SetTitle(ParadoxPopupUtils.LOC_POPUP_WARNING, MenuItem.COLOR);
					ParadoxPopupUtils.sPopup.SetMessage(iLocMessage, MenuItem.COLOR);
					ParadoxPopupUtils.sPopup.SetExtra(iExtra, MenuItem.COLOR);
					Singleton<PopupSystem>.Instance.AddPopupToQueue(ParadoxPopupUtils.sPopup);
				}
			}
		}

		// Token: 0x040028C8 RID: 10440
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxPopupUtils;

		// Token: 0x040028C9 RID: 10441
		private static readonly int LOC_POPUP_WARNING = "#popup_warning".GetHashCodeCustom();

		// Token: 0x040028CA RID: 10442
		private static readonly int LOC_CREATION_FAILED = "#login_fail_01".GetHashCodeCustom();

		// Token: 0x040028CB RID: 10443
		private static readonly int LOC_LOGIN_FAILED = "#login_fail_02".GetHashCodeCustom();

		// Token: 0x040028CC RID: 10444
		private static readonly int LOC_ACCOUNT_EXISTS = "#login_fail_03".GetHashCodeCustom();

		// Token: 0x040028CD RID: 10445
		private static readonly int LOC_INVALID_EMAIL = "#login_fail_04".GetHashCodeCustom();

		// Token: 0x040028CE RID: 10446
		private static readonly int LOC_BAD_PASSWORD_LENGTH = "#login_fail_05".GetHashCodeCustom();

		// Token: 0x040028CF RID: 10447
		private static readonly int LOC_NOT_AUTHORISED = "#login_fail_06".GetHashCodeCustom();

		// Token: 0x040028D0 RID: 10448
		private static readonly int LOC_UNKNOWN_ERROR = "#login_fail_07".GetHashCodeCustom();

		// Token: 0x040028D1 RID: 10449
		private static MenuMessagePopup sPopup = null;

		// Token: 0x040028D2 RID: 10450
		private static object sPopupLock = new object();
	}
}
