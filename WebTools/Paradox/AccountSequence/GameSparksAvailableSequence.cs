// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.AccountSequence.GameSparksAvailableSequence
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;
using PopsApi;
using System.Threading;

#nullable disable
namespace Magicka.WebTools.Paradox.AccountSequence;

public class GameSparksAvailableSequence(
  ParadoxAccount iAccount,
  ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : ParadoxAccountSequence(iAccount, iCallback)
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountGameSparksAvailable;
  private const string MERGE_STATUS_PENDING = "pending";
  private const string MERGE_STATUS_SUCCESS = "success";
  private const string MERGE_STATUS_FAILURE = "failure";
  private string mMergeTaskId = string.Empty;

  protected override void OnExecute()
  {
    if (this.Account.IsLoggedFull || this.Account.IsLoggedShadow)
    {
      if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
        this.RequestAccountMergeShadow();
      else
        this.RequestGameSparksAuthentication();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_NotAuthenticated);
  }

  private void RequestAccountMergeShadow()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameSparksAvailable, nameof (RequestAccountMergeShadow));
    if (!this.CheckPhase(new ParadoxAccountSequence.SequencePhase[1]))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountMergeShadow);
    Singleton<ParadoxServices>.Instance.AccountMergeShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountMergeShadowDelegate(this.AccountMergeShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountMergeShadowCallback));
  }

  private void RequestAccountGetMergeStatus()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameSparksAvailable, nameof (RequestAccountGetMergeStatus));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountMergeShadow, ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus);
    Singleton<ParadoxServices>.Instance.AccountGetMergeStatus(this.mMergeTaskId, new ParadoxServices.AccountGetMergeDetailsDelegate(this.AccountGetMergeStatusCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountGetMergeStatusCallback));
  }

  private void RequestGameSparksAuthentication()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameSparksAvailable, nameof (RequestGameSparksAuthentication));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.Enter, ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus, ParadoxAccountSequence.SequencePhase.AccountMergeShadow, ParadoxAccountSequence.SequencePhase.AccountGetDetails))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate);
    ParadoxUtils.AuthenticateWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
  }

  private void RequestTokenInvalidateNoCallback()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountGameSparksAvailable, nameof (RequestTokenInvalidateNoCallback));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate))
      return;
    Singleton<ParadoxServices>.Instance.AuthTokenInvalidate((ParadoxServices.AuthTokenInvalidateDelegate) null, (ParadoxServices.HandledExceptionDelegate) null);
  }

  private void AccountMergeShadowCallback(string iMergeTaskId)
  {
    this.mMergeTaskId = iMergeTaskId;
    this.RequestAccountGetMergeStatus();
  }

  private void FailedAccountMergeShadowCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameSparksAvailable, "AccountMergeShadow failed : " + iReason);
    this.RequestGameSparksAuthentication();
  }

  private void AccountGetMergeStatusCallback(PopsApiWrapper.AccountGetMergeStatusResult iResult)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameSparksAvailable, $"AccountGetMergeStatus received : [{iResult.Status}] {iResult.StatusMessage}");
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
      this.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_AccountFailedToMerge);
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_AccountUnknownMergeStatus);
  }

  private void FailedAccountGetMergeStatusCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountGameSparksAvailable, "AccountGetMergeStatus failed : " + iReason);
    this.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_AccountGetMergeStatusFailed);
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
      this.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_GameSparksAuthenticationFailed);
    }
  }
}
