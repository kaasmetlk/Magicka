// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.AccountSequence.PlayerLogoutSequence
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;

#nullable disable
namespace Magicka.WebTools.Paradox.AccountSequence;

public class PlayerLogoutSequence(
  ParadoxAccount iAccount,
  ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : ParadoxAccountSequence(iAccount, iCallback)
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountPlayerLogout;
  private const string SOURCE_SERVICE = "pc_steam_red_wizard_log_out";
  private string mTemporaryUniqueId = string.Empty;
  private ParadoxAccount.ErrorCode mGameSparksFailedCode;

  protected override void OnExecute()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, "Starting PlayerLogout Sequence");
    if (this.Account.IsLoggedOff)
      this.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountAlreadyLoggedOut);
    else
      this.RequestTokenInvalidate();
  }

  private void RequestTokenInvalidate()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, nameof (RequestTokenInvalidate));
    if (!this.CheckPhase(new ParadoxAccountSequence.SequencePhase[1]))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AuthTokenInvalidate);
    Singleton<ParadoxServices>.Instance.AuthTokenInvalidate(new ParadoxServices.AuthTokenInvalidateDelegate(this.AuthTokenInvalidateCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAuthTokenInvalidateCallback));
  }

  private void RequestTokenInvalidateNoCallback()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, nameof (RequestTokenInvalidateNoCallback));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.GameSparksRegister))
      return;
    Singleton<ParadoxServices>.Instance.AuthTokenInvalidate((ParadoxServices.AuthTokenInvalidateDelegate) null, (ParadoxServices.HandledExceptionDelegate) null);
  }

  private void RequestAccountCreateShadow()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, nameof (RequestAccountCreateShadow));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AuthTokenInvalidate))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountCreateShadow);
    this.mTemporaryUniqueId = HardwareInfoManager.GenerateUniqueSessionId();
    Singleton<ParadoxServices>.Instance.AccountCreateShadow(this.mTemporaryUniqueId, "generated", "pc_steam_red_wizard_log_out", new ParadoxServices.AccountCreateShadowDelegate(this.AccountCreateShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountCreateShadowCallback));
  }

  private void RequestAccountLoginShadow()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, nameof (RequestAccountLoginShadow));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountCreateShadow))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLoginShadow);
    Singleton<ParadoxServices>.Instance.AccountLoginShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountLoginShadowDelegate(this.AccountLoginShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountLoginShadowCallback));
  }

  private void RequestGameSparksRegistration()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, nameof (RequestGameSparksRegistration));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLoginShadow))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksRegister);
    ParadoxUtils.RegisterWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
  }

  private void AuthTokenInvalidateCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      this.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.LoggedOff);
      this.Account.Email = string.Empty;
      this.Account.IsLinkedToSteam = false;
      Singleton<ParadoxAccountSaveData>.Instance.Reset();
      Singleton<GameSparksAccount>.Instance.LogOut();
      this.RequestAccountCreateShadow();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.Logout_AuthTokenInvalidationFailed);
  }

  private void FailedAuthTokenInvalidateCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerLogout, "AuthTokenInvalidate failed : " + iReason);
    this.ExitFailure(ParadoxAccount.ErrorCode.Logout_AuthTokenInvalidationFailed);
  }

  private void AccountCreateShadowCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      Singleton<ParadoxAccountSaveData>.Instance.SetShadowUniqueId(this.mTemporaryUniqueId);
      this.mTemporaryUniqueId = string.Empty;
      this.RequestAccountLoginShadow();
    }
    else
    {
      this.mTemporaryUniqueId = string.Empty;
      this.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountCreateShadowFailed);
    }
  }

  private void FailedAccountCreateShadowCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerLogout, "AccountCreateShadow failed : " + iReason);
    this.mTemporaryUniqueId = string.Empty;
    this.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountCreateShadowFailed);
  }

  private void AccountLoginShadowCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      this.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.ShadowAccount);
      this.RequestGameSparksRegistration();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountLoginShadowFailed);
  }

  private void FailedAccountLoginShadowCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerLogout, "AccountCreateShadow failed : " + iReason);
    this.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountLoginShadowFailed);
  }

  private void GameSparksOperationCallback(GameSparksAccount.Result iResult)
  {
    if (iResult == GameSparksAccount.Result.Success)
    {
      this.ExitSuccess();
    }
    else
    {
      this.RequestTokenInvalidateNoCallback();
      this.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.LoggedOff);
      this.Account.Email = string.Empty;
      this.Account.IsLinkedToSteam = false;
      Singleton<ParadoxAccountSaveData>.Instance.Reset();
      Singleton<GameSparksAccount>.Instance.LogOut();
      this.ExitFailure(ParadoxAccount.ErrorCode.Logout_GameSparksRegistrationFailed);
    }
  }
}
