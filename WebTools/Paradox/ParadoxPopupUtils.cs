// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.ParadoxPopupUtils
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.UI.Popup;
using Magicka.Misc;

#nullable disable
namespace Magicka.WebTools.Paradox;

public static class ParadoxPopupUtils
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxPopupUtils;
  private static readonly int LOC_POPUP_WARNING = "#popup_warning".GetHashCodeCustom();
  private static readonly int LOC_CREATION_FAILED = "#login_fail_01".GetHashCodeCustom();
  private static readonly int LOC_LOGIN_FAILED = "#login_fail_02".GetHashCodeCustom();
  private static readonly int LOC_ACCOUNT_EXISTS = "#login_fail_03".GetHashCodeCustom();
  private static readonly int LOC_INVALID_EMAIL = "#login_fail_04".GetHashCodeCustom();
  private static readonly int LOC_BAD_PASSWORD_LENGTH = "#login_fail_05".GetHashCodeCustom();
  private static readonly int LOC_NOT_AUTHORISED = "#login_fail_06".GetHashCodeCustom();
  private static readonly int LOC_UNKNOWN_ERROR = "#login_fail_07".GetHashCodeCustom();
  private static MenuMessagePopup sPopup = (MenuMessagePopup) null;
  private static object sPopupLock = new object();

  static ParadoxPopupUtils()
  {
    ParadoxPopupUtils.sPopup = new MenuMessagePopup();
    ParadoxPopupUtils.sPopup.Alignment = PopupAlign.Middle | PopupAlign.Center;
  }

  public static void ShowErrorPopup(ParadoxAccount.ErrorCode iErrorCode)
  {
    switch (iErrorCode)
    {
      case ParadoxAccount.ErrorCode.Startup_FailedLoginWithSteamOrAuthToken:
      case ParadoxAccount.ErrorCode.AccountCreate_CreatedButNotLoggedIn:
      case ParadoxAccount.ErrorCode.Login_AccountLoginFailedNotAuthorized:
      case ParadoxAccount.ErrorCode.Login_AccountLoginFailedUnknown:
      case ParadoxAccount.ErrorCode.Login_GameSparksAuthenticationFailed:
        ParadoxPopupUtils.ShowErrorPopupWithExtra(ParadoxPopupUtils.LOC_LOGIN_FAILED, $"({(int) iErrorCode})");
        break;
      case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidEmail:
      case ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedInvalidEmail:
        ParadoxPopupUtils.ShowErrorPopup(ParadoxPopupUtils.LOC_INVALID_EMAIL);
        break;
      case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedBadPasswordLength:
      case ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedBadPasswordLength:
        ParadoxPopupUtils.ShowErrorPopup(ParadoxPopupUtils.LOC_BAD_PASSWORD_LENGTH);
        break;
      case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedAlreadyExists:
      case ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedAlreadyExists:
        ParadoxPopupUtils.ShowErrorPopup(ParadoxPopupUtils.LOC_ACCOUNT_EXISTS);
        break;
      case ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedUnknown:
      case ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedUnknown:
      case ParadoxAccount.ErrorCode.AccountCreate_GameSparksAuthenticationFailed:
      case ParadoxAccount.ErrorCode.AccountCreate_GameSparksRegistrationFailed:
      case ParadoxAccount.ErrorCode.AccountCreate_GameSparksUnknownFailure:
        ParadoxPopupUtils.ShowErrorPopupWithExtra(ParadoxPopupUtils.LOC_CREATION_FAILED, $"({(int) iErrorCode})");
        break;
      case ParadoxAccount.ErrorCode.Logout_AuthTokenInvalidationFailed:
      case ParadoxAccount.ErrorCode.SteamLink_LinkAlreadyExistWithAnotherAccount:
      case ParadoxAccount.ErrorCode.SteamLink_LinkFailed:
      case ParadoxAccount.ErrorCode.SteamUnlink_UnlinkFailed:
        ParadoxPopupUtils.ShowErrorPopupWithExtra(ParadoxPopupUtils.LOC_UNKNOWN_ERROR, $"({(int) iErrorCode})");
        break;
      default:
        Logger.LogWarning(Logger.Source.ParadoxPopupUtils, "Error code not supported, no popup to display.");
        break;
    }
  }

  private static void ShowErrorPopup(int iLocMessage)
  {
    lock (ParadoxPopupUtils.sPopupLock)
    {
      if (ParadoxPopupUtils.sPopup == null)
        return;
      ParadoxPopupUtils.sPopup.SetTitle(ParadoxPopupUtils.LOC_POPUP_WARNING, MenuItem.COLOR);
      ParadoxPopupUtils.sPopup.SetMessage(iLocMessage, MenuItem.COLOR);
      Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) ParadoxPopupUtils.sPopup);
    }
  }

  private static void ShowErrorPopup(string iLocTitle, string iLocMessage)
  {
    lock (ParadoxPopupUtils.sPopupLock)
    {
      if (ParadoxPopupUtils.sPopup == null)
        return;
      ParadoxPopupUtils.sPopup.SetTitle(iLocTitle, MenuItem.COLOR);
      ParadoxPopupUtils.sPopup.SetMessage(iLocMessage, MenuItem.COLOR);
      Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) ParadoxPopupUtils.sPopup);
    }
  }

  private static void ShowErrorPopupWithExtra(int iLocMessage, string iExtra)
  {
    lock (ParadoxPopupUtils.sPopupLock)
    {
      if (ParadoxPopupUtils.sPopup == null)
        return;
      ParadoxPopupUtils.sPopup.SetTitle(ParadoxPopupUtils.LOC_POPUP_WARNING, MenuItem.COLOR);
      ParadoxPopupUtils.sPopup.SetMessage(iLocMessage, MenuItem.COLOR);
      ParadoxPopupUtils.sPopup.SetExtra(iExtra, MenuItem.COLOR);
      Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) ParadoxPopupUtils.sPopup);
    }
  }
}
