// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.AccountSequence.GameStartupSequence
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;
using Magicka.WebTools.GameSparks;
using PopsApi;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Magicka.WebTools.Paradox.AccountSequence;

public class GameStartupSequence(
  ParadoxAccount iAccount,
  ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : ParadoxAccountSequence(iAccount, iCallback)
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountGameStartup;
  public const string SOURCE_SERVICE = "pc_steam_red_wizard_startup";
  private const string ACCOUNT_EXISTS = "account-exists";
  private const string NOT_AUTHORIZED = "not-authorized";
  private const string MERGE_STATUS_PENDING = "pending";
  private const string MERGE_STATUS_SUCCESS = "success";
  private const string MERGE_STATUS_FAILURE = "failure";
  private const string UNIVERSE_STEAM = "steam";
  private string mTemporaryUniqueId = string.Empty;
  private string mMergeTaskId = string.Empty;
  private bool mAuthTokenAuthenticationFailed;
  private bool mDidCreateShadowAccount;

  protected override void OnExecute()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "Starting GameStartup Sequence");
    if (Singleton<ParadoxAccountSaveData>.Instance.HasAuthToken)
      this.RequestLoginWithSavedToken();
    else
      this.RequestLoginInWithSteamTicket();
  }

  private void RequestLoginWithSavedToken()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestLoginWithSavedToken));
    if (!this.CheckPhase(new ParadoxAccountSequence.SequencePhase[1]))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLoginWithAuthToken);
    Singleton<ParadoxServices>.Instance.AccountLoginWithAuthToken(Singleton<ParadoxAccountSaveData>.Instance.AuthToken, new ParadoxServices.AccountLoginWithAuthTokenDelegate(this.AccountLoginWithSavedTokenCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountLoginWithSavedTokenFailedCallback));
  }

  private void RequestLoginInWithSteamTicket()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestLoginInWithSteamTicket));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.Enter, ParadoxAccountSequence.SequencePhase.AccountLoginWithAuthToken))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLoginWithSteamTicket);
    Singleton<ParadoxServices>.Instance.AccountLoginSteamTicket(InternalSteamUtils.GetSteamAppID().ToString(), InternalSteamUtils.GetSteamAuthToken(), new ParadoxServices.AccountLoginSteamTicketDelegate(this.AccountLoginWithSteamTicketCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountLoginWithSteamTicketFailedCallback));
  }

  private void RequestShadowLogin()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestShadowLogin));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLoginWithSteamTicket, ParadoxAccountSequence.SequencePhase.AccountCreateShadow))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLoginShadow);
    Singleton<ParadoxServices>.Instance.AccountLoginShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountLoginShadowDelegate(this.AccountLoginShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountShadowLoginFailedCallback));
  }

  private void RequestCreateShadowAccount()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestCreateShadowAccount));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLoginWithSteamTicket))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountCreateShadow);
    this.mTemporaryUniqueId = HardwareInfoManager.GenerateUniqueSessionId();
    Singleton<ParadoxServices>.Instance.AccountCreateShadow(this.mTemporaryUniqueId, "generated", "pc_steam_red_wizard_startup", new ParadoxServices.AccountCreateShadowDelegate(this.AccountCreateShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountCreateShadowFailedCallback));
  }

  private void RequestAccountGetDetails()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestAccountGetDetails));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountConnections, ParadoxAccountSequence.SequencePhase.AccountLoginWithSteamTicket))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountGetDetails);
    Singleton<ParadoxServices>.Instance.AccountGetDetails(new ParadoxServices.AccountGetDetailsDelegate(this.AccountGetDetailsCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountGetDetailsFailedCallback));
  }

  private void RequestAccountConnections()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestAccountConnections));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLoginWithAuthToken))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountConnections);
    Singleton<ParadoxServices>.Instance.AccountConnections(new ParadoxServices.AccountConnectionsDelegate(this.AccountConnectionsCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountConnectionsFailedCallback));
  }

  private void RequestAccountMergeShadow()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestAccountMergeShadow));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountGetDetails))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountMergeShadow);
    Singleton<ParadoxServices>.Instance.AccountMergeShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountMergeShadowDelegate(this.AccountMergeShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountMergeShadowCallback));
  }

  private void RequestAccountGetMergeStatus()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestAccountGetMergeStatus));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountMergeShadow, ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus);
    Singleton<ParadoxServices>.Instance.AccountGetMergeStatus(this.mMergeTaskId, new ParadoxServices.AccountGetMergeDetailsDelegate(this.AccountGetMergeStatusCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountGetMergeStatusCallback));
  }

  private void RequestGameSparksRegistration()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestGameSparksRegistration));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLoginShadow))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksRegister);
    ParadoxUtils.RegisterWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
  }

  private void RequestGameSparksAuthentication()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestGameSparksAuthentication));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLoginShadow, ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus, ParadoxAccountSequence.SequencePhase.AccountMergeShadow, ParadoxAccountSequence.SequencePhase.AccountGetDetails))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate);
    ParadoxUtils.AuthenticateWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
  }

  private void RequestTokenInvalidateNoCallback()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, nameof (RequestTokenInvalidateNoCallback));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate, ParadoxAccountSequence.SequencePhase.GameSparksRegister))
      return;
    Singleton<ParadoxServices>.Instance.AuthTokenInvalidate((ParadoxServices.AuthTokenInvalidateDelegate) null, (ParadoxServices.HandledExceptionDelegate) null);
  }

  private void AccountLoginWithSavedTokenCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      this.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.FullAccount);
      this.RequestAccountConnections();
    }
    else
    {
      Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "Failed to login normal account. Fallback to shadow account.");
      this.RequestLoginInWithSteamTicket();
    }
  }

  private void AccountLoginWithSavedTokenFailedCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountLoginWithSavedToken failed : " + iReason);
    this.mAuthTokenAuthenticationFailed = true;
    Singleton<ParadoxAccountSaveData>.Instance.ClearAuthToken();
    this.RequestLoginInWithSteamTicket();
  }

  private void AccountLoginWithSteamTicketCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      this.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.FullAccount);
      this.Account.IsLinkedToSteam = true;
      this.RequestAccountGetDetails();
    }
    else
    {
      if (this.mAuthTokenAuthenticationFailed)
        Singleton<ParadoxAccount>.Instance.SetPendingErrorCode(ParadoxAccount.ErrorCode.Startup_FailedLoginWithSteamOrAuthToken);
      if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
        this.RequestShadowLogin();
      else
        this.RequestCreateShadowAccount();
    }
  }

  private void AccountLoginWithSteamTicketFailedCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountLoginWithSteamTicket failed : " + iReason);
    if (this.mAuthTokenAuthenticationFailed)
      Singleton<ParadoxAccount>.Instance.SetPendingErrorCode(ParadoxAccount.ErrorCode.Startup_FailedLoginWithSteamOrAuthToken);
    if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
      this.RequestShadowLogin();
    else
      this.RequestCreateShadowAccount();
  }

  private void AccountCreateShadowCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      Singleton<ParadoxAccountSaveData>.Instance.SetShadowUniqueId(this.mTemporaryUniqueId);
      this.mTemporaryUniqueId = string.Empty;
      this.mDidCreateShadowAccount = true;
      this.RequestShadowLogin();
    }
    else
    {
      Singleton<ParadoxAccountSaveData>.Instance.Reset();
      this.ExitFailure(ParadoxAccount.ErrorCode.Startup_ShadowAccountCreationFailed);
    }
  }

  private void AccountCreateShadowFailedCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountCreateShadow failed : " + iReason);
    if (iReason.Equals("account-exists"))
    {
      this.RequestShadowLogin();
    }
    else
    {
      Singleton<ParadoxAccountSaveData>.Instance.Reset();
      this.ExitFailure(ParadoxAccount.ErrorCode.Startup_ShadowAccountCreationFailed);
    }
  }

  private void AccountLoginShadowCallback(bool iSuccess)
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "AccountLoginShadowCallback.");
    if (iSuccess)
    {
      this.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.ShadowAccount);
      if (this.mDidCreateShadowAccount)
        this.RequestGameSparksRegistration();
      else
        this.RequestGameSparksAuthentication();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.Startup_ShadowLoginFailed);
  }

  private void AccountShadowLoginFailedCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountShadowLogin failed : " + iReason);
    this.ExitFailure(ParadoxAccount.ErrorCode.Startup_ShadowLoginFailed);
  }

  private void AccountConnectionsCallback(
    ICollection<PopsApiWrapper.Connection> iConnections)
  {
    foreach (PopsApiWrapper.Connection iConnection in (IEnumerable<PopsApiWrapper.Connection>) iConnections)
    {
      if (iConnection.Universe.Equals("steam"))
      {
        Singleton<ParadoxAccountSaveData>.Instance.ClearAuthToken();
        this.Account.IsLinkedToSteam = true;
        break;
      }
    }
    this.RequestAccountGetDetails();
  }

  private void AccountConnectionsFailedCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountConnections failed : " + iReason);
    this.RequestAccountGetDetails();
  }

  private void AccountGetDetailsCallback(PopsApiWrapper.AccountGetDetailsResult iDetails)
  {
    this.Account.Email = iDetails.Account.Email;
    if (!GameSparksServices.Available)
    {
      GameSparksServices.AvailabilityChanged += new GameSparksServices.AvailabilityChangedDelegate(Singleton<ParadoxAccount>.Instance.GameSparksAvailableCallback);
      this.ExitSuccess();
    }
    else if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
      this.RequestAccountMergeShadow();
    else
      this.RequestGameSparksAuthentication();
  }

  private void AccountGetDetailsFailedCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountGetDetails failed : " + iReason);
    if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
      this.RequestAccountMergeShadow();
    else
      this.RequestGameSparksAuthentication();
  }

  private void AccountMergeShadowCallback(string iMergeTaskId)
  {
    this.mMergeTaskId = iMergeTaskId;
    this.RequestAccountGetMergeStatus();
  }

  private void FailedAccountMergeShadowCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountMergeShadow failed : " + iReason);
    this.RequestGameSparksAuthentication();
  }

  private void AccountGetMergeStatusCallback(PopsApiWrapper.AccountGetMergeStatusResult iResult)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameStartup, $"AccountGetMergeStatus received : [{iResult.Status}] {iResult.StatusMessage}");
    if (iResult.Status.Equals("pending"))
    {
      Thread.Sleep(2000);
      this.RequestAccountGetMergeStatus();
    }
    else
    {
      Singleton<ParadoxAccountSaveData>.Instance.ClearShadowUniqueId();
      this.RequestGameSparksAuthentication();
    }
  }

  private void FailedAccountGetMergeStatusCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountGetMergeStatus failed : " + iReason);
    Singleton<ParadoxAccountSaveData>.Instance.ClearShadowUniqueId();
    this.RequestGameSparksAuthentication();
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
      if (iResult == GameSparksAccount.Result.AuthenticationFailure)
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_GameSparksAuthenticationFailed);
      else if (iResult == GameSparksAccount.Result.RegistrationFailure)
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_GameSparksRegistrationFailed);
      else
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_GameSparksUnknownFailure);
    }
  }
}
