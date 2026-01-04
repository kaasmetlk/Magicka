// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.AccountSequence.PlayerLoginSequence
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

public class PlayerLoginSequence : ParadoxAccountSequence
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountPlayerLogin;
  private const string MERGE_STATUS_PENDING = "pending";
  private const string MERGE_STATUS_SUCCESS = "success";
  private const string MERGE_STATUS_FAILURE = "failure";
  private const string UNIVERSE_STEAM = "steam";
  private const string NOT_AUTHORIZED = "not-authorized";
  private readonly string mUsername = string.Empty;
  private readonly string mPassword = string.Empty;
  private string mMergeTaskId = string.Empty;

  public PlayerLoginSequence(
    string iUsername,
    string iPassword,
    ParadoxAccount iAccount,
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
    : base(iAccount, iCallback)
  {
    this.mUsername = iUsername;
    this.mPassword = iPassword;
  }

  protected override void OnExecute()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, "Starting PlayerLogin Sequence");
    if (this.Account.IsLoggedFull)
      this.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountAlreadyLoggedIn);
    else
      this.RequestAccountLogin();
  }

  private void RequestAccountLogin()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, nameof (RequestAccountLogin));
    if (!this.CheckPhase(new ParadoxAccountSequence.SequencePhase[1]))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLogin);
    Singleton<ParadoxServices>.Instance.AccountLogin(this.mUsername, this.mPassword, new ParadoxServices.AccountLoginDelegate(this.AccountLoginCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountLoginCallback));
  }

  private void RequestAccountMergeShadow()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, nameof (RequestAccountMergeShadow));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountGetDetails))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountMergeShadow);
    Singleton<ParadoxServices>.Instance.AccountMergeShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountMergeShadowDelegate(this.AccountMergeShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountMergeShadowCallback));
  }

  private void RequestAccountGetMergeStatus()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, nameof (RequestAccountGetMergeStatus));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountMergeShadow, ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus);
    Singleton<ParadoxServices>.Instance.AccountGetMergeStatus(this.mMergeTaskId, new ParadoxServices.AccountGetMergeDetailsDelegate(this.AccountGetMergeStatusCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountGetMergeStatusCallback));
  }

  private void RequestAccountGetDetails()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, nameof (RequestAccountGetDetails));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountConnections))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountGetDetails);
    Singleton<ParadoxServices>.Instance.AccountGetDetails(new ParadoxServices.AccountGetDetailsDelegate(this.AccountGetDetailsCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountGetDetailsFailedCallback));
  }

  private void RequestAccountConnections()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, nameof (RequestAccountConnections));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLogin))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountConnections);
    Singleton<ParadoxServices>.Instance.AccountConnections(new ParadoxServices.AccountConnectionsDelegate(this.AccountConnectionsCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountConnectionsFailedCallback));
  }

  private void RequestGameSparksAuthentication()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, nameof (RequestGameSparksAuthentication));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus, ParadoxAccountSequence.SequencePhase.AccountMergeShadow, ParadoxAccountSequence.SequencePhase.AccountGetDetails))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate);
    ParadoxUtils.AuthenticateWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
  }

  private void RequestTokenInvalidateNoCallback()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, nameof (RequestTokenInvalidateNoCallback));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate))
      return;
    Singleton<ParadoxServices>.Instance.AuthTokenInvalidate((ParadoxServices.AuthTokenInvalidateDelegate) null, (ParadoxServices.HandledExceptionDelegate) null);
  }

  private void AccountLoginCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      this.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.FullAccount);
      Singleton<ParadoxAccountSaveData>.Instance.SetAuthToken(Singleton<ParadoxServices>.Instance.RetrieveAuthToken());
      this.RequestAccountConnections();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountLoginFailedUnknown);
  }

  private void FailedAccountLoginCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountLogin failed : " + iReason);
    if (iReason.Equals("not-authorized"))
      this.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountLoginFailedNotAuthorized);
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountLoginFailedUnknown);
  }

  private void AccountMergeShadowCallback(string iMergeTaskId)
  {
    this.mMergeTaskId = iMergeTaskId;
    this.RequestAccountGetMergeStatus();
  }

  private void FailedAccountMergeShadowCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountMergeShadow failed : " + iReason);
    this.RequestGameSparksAuthentication();
  }

  private void AccountGetMergeStatusCallback(PopsApiWrapper.AccountGetMergeStatusResult iResult)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, $"AccountGetMergeStatus received : [{iResult.Status}] {iResult.StatusMessage}");
    if (iResult.Status.Equals("pending"))
    {
      Thread.Sleep(2000);
      this.RequestAccountGetMergeStatus();
    }
    else if (iResult.Status.Equals("success"))
    {
      Singleton<ParadoxAccountSaveData>.Instance.ClearShadowUniqueId();
      this.RequestGameSparksAuthentication();
    }
    else if (iResult.Status.Equals("failure"))
      this.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountFailedToMerge);
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountUnknownMergeStatus);
  }

  private void FailedAccountGetMergeStatusCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountGetMergeStatus failed : " + iReason);
    this.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountGetMergeStatusFailed);
  }

  private void AccountConnectionsCallback(
    ICollection<PopsApiWrapper.Connection> iConnections)
  {
    Singleton<ParadoxServices>.Instance.RetrieveAccountGuid();
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
    Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountConnections failed : " + iReason);
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
    Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountGetDetails failed : " + iReason);
    if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
      this.RequestAccountMergeShadow();
    else
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
      this.ExitFailure(ParadoxAccount.ErrorCode.Login_GameSparksAuthenticationFailed);
    }
  }
}
