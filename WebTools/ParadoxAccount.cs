// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.ParadoxAccount
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.WebTools.GameSparks;
using Magicka.WebTools.Paradox;
using Magicka.WebTools.Paradox.AccountSequence;
using System.Collections.Generic;

#nullable disable
namespace Magicka.WebTools;

public class ParadoxAccount : Singleton<ParadoxAccount>
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccount;
  private const string UNKNOWN_ERROR = "unknown-error";
  private const string ACCOUNT_EXISTS = "account-exists";
  private const string NOT_AUTHORIZED = "not-authorized";
  public static ParadoxAccount.BecameBusyDelegate OnBecameBusy;
  public static ParadoxAccount.BecameIdleDelegate OnBecameIdle;
  private ParadoxAccount.AuthenticationState mAuthenticationState;
  private object mAccountLock = new object();
  private Queue<ParadoxAccountSequence> mPendingSequences = new Queue<ParadoxAccountSequence>();
  private ParadoxAccountSequence mCurrentSequence;
  private string mCurrentAccountEmail = string.Empty;
  private bool mLinkedToSteam;
  private ParadoxAccount.ErrorCode mPendingErrorCode;

  public bool IsLoggedShadow
  {
    get
    {
      lock (this.mAccountLock)
        return this.mAuthenticationState == ParadoxAccount.AuthenticationState.ShadowAccount;
    }
  }

  public bool IsLoggedFull
  {
    get
    {
      lock (this.mAccountLock)
        return this.mAuthenticationState == ParadoxAccount.AuthenticationState.FullAccount;
    }
  }

  public bool IsLoggedOff
  {
    get
    {
      lock (this.mAccountLock)
        return this.mAuthenticationState == ParadoxAccount.AuthenticationState.LoggedOff;
    }
  }

  public string Email
  {
    get => this.mCurrentAccountEmail;
    set => this.mCurrentAccountEmail = value;
  }

  public bool IsBusy => this.mCurrentSequence != null && !this.mCurrentSequence.Completed;

  public bool IsLinkedToSteam
  {
    get => this.mLinkedToSteam;
    set => this.mLinkedToSteam = value;
  }

  public ParadoxAccount.ErrorCode PendingErrorCode
  {
    get
    {
      lock (this.mAccountLock)
        return this.mPendingErrorCode;
    }
  }

  public void Update()
  {
    if (this.mCurrentSequence == null || !this.mCurrentSequence.Completed)
      return;
    if (this.mPendingSequences.Count > 0)
    {
      this.mCurrentSequence = this.mPendingSequences.Dequeue();
      this.mCurrentSequence.Execute();
      if (ParadoxAccount.OnBecameBusy != null)
        ParadoxAccount.OnBecameBusy();
      Logger.LogDebug(Logger.Source.ParadoxAccount, "Starting new sequence.");
    }
    else
    {
      this.mCurrentSequence = (ParadoxAccountSequence) null;
      if (ParadoxAccount.OnBecameIdle != null)
        ParadoxAccount.OnBecameIdle();
      Logger.LogDebug(Logger.Source.ParadoxAccount, "Disposed of a finished sequence.");
    }
  }

  public void SetAuthenticationState(ParadoxAccount.AuthenticationState iNewState)
  {
    lock (this.mAccountLock)
      this.mAuthenticationState = iNewState;
  }

  public void SetPendingErrorCode(ParadoxAccount.ErrorCode iErrorCode)
  {
    lock (this.mAccountLock)
      this.mPendingErrorCode = iErrorCode;
  }

  public ParadoxAccount.ErrorCode ConsumePendingErrorCode()
  {
    ParadoxAccount.ErrorCode pendingErrorCode = this.mPendingErrorCode;
    this.mPendingErrorCode = ParadoxAccount.ErrorCode.None;
    return pendingErrorCode;
  }

  public void GameStartup(
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
  {
    this.PushNewSequence((ParadoxAccountSequence) new GameStartupSequence(this, iCallback));
  }

  public void CreatePlayerAccount(
    string iUsername,
    string iPassword,
    string iDateOfBirth,
    string iCountryCode,
    bool iSubscribeToNewsletter,
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
  {
    this.PushNewSequence((ParadoxAccountSequence) new PlayerCreateAccountSequence(iUsername, iPassword, iDateOfBirth, iCountryCode, iSubscribeToNewsletter, this, iCallback));
  }

  public void LoginPlayer(
    string iUsername,
    string iPassword,
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
  {
    this.PushNewSequence((ParadoxAccountSequence) new PlayerLoginSequence(iUsername, iPassword, this, iCallback));
  }

  public void LogOffPlayer(
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
  {
    this.PushNewSequence((ParadoxAccountSequence) new PlayerLogoutSequence(this, iCallback));
  }

  public void LinkSteam(
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
  {
    this.PushNewSequence((ParadoxAccountSequence) new SteamLinkSequence(this, iCallback));
  }

  public void UnlinkSteam(
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
  {
    this.PushNewSequence((ParadoxAccountSequence) new SteamLinkSequence(this, iCallback));
  }

  public void ToggleSteamLink(
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
  {
    this.PushNewSequence(!this.IsLinkedToSteam ? (ParadoxAccountSequence) new SteamLinkSequence(this, iCallback) : (ParadoxAccountSequence) new SteamUnlinkSequence(this, iCallback));
  }

  public void GameSparksAvailableLogin(
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
  {
    this.PushNewSequence((ParadoxAccountSequence) new GameSparksAvailableSequence(this, iCallback));
  }

  public void GameSparksAvailableCallback(bool iAvailable)
  {
    if (Singleton<ParadoxAccount>.Instance.IsLoggedOff || this.mCurrentSequence is PlayerLogoutSequence)
    {
      GameSparksServices.AvailabilityChanged -= new GameSparksServices.AvailabilityChangedDelegate(this.GameSparksAvailableCallback);
    }
    else
    {
      if (!iAvailable)
        return;
      this.GameSparksAvailableLogin((ParadoxAccountSequence.ExecutionDoneDelegate) null);
      GameSparksServices.AvailabilityChanged -= new GameSparksServices.AvailabilityChangedDelegate(this.GameSparksAvailableCallback);
    }
  }

  private void PushNewSequence(ParadoxAccountSequence iSequence)
  {
    if (this.mCurrentSequence == null)
    {
      this.mCurrentSequence = iSequence;
      this.mCurrentSequence.Execute();
      if (ParadoxAccount.OnBecameBusy == null)
        return;
      ParadoxAccount.OnBecameBusy();
    }
    else
      this.mPendingSequences.Enqueue(iSequence);
  }

  public enum ErrorCode
  {
    None = 0,
    Startup_FailedLoginWithSteamOrAuthToken = 100, // 0x00000064
    Startup_ShadowAccountCreationFailed = 101, // 0x00000065
    Startup_ShadowLoginFailed = 102, // 0x00000066
    Startup_GetDetailsFailed = 103, // 0x00000067
    Startup_AccountMergeShadowFailed = 104, // 0x00000068
    Startup_AccountFailedToMerge = 105, // 0x00000069
    Startup_AccountUnknownMergeStatus = 106, // 0x0000006A
    Startup_AccountGetMergeStatusFailed = 107, // 0x0000006B
    Startup_GameSparksAuthenticationFailed = 108, // 0x0000006C
    Startup_GameSparksRegistrationFailed = 109, // 0x0000006D
    Startup_GameSparksUnknownFailure = 110, // 0x0000006E
    AccountCreate_AccountAlreadyLoggedIn = 200, // 0x000000C8
    AccountCreate_AccountCreationFailedInvalidEmail = 201, // 0x000000C9
    AccountCreate_AccountCreationFailedBadPasswordLength = 202, // 0x000000CA
    AccountCreate_AccountCreationFailedAlreadyExists = 203, // 0x000000CB
    AccountCreate_AccountCreationFailedInvalidLanguage = 204, // 0x000000CC
    AccountCreate_AccountCreationFailedInvalidCountry = 205, // 0x000000CD
    AccountCreate_AccountCreationFailedInvalidDoB = 206, // 0x000000CE
    AccountCreate_AccountCreationFailedUnder13 = 207, // 0x000000CF
    AccountCreate_AccountCreationFailedUnknown = 208, // 0x000000D0
    AccountCreate_CreatedButNotLoggedIn = 209, // 0x000000D1
    AccountCreate_AccountPromotionFailedBadPasswordLength = 210, // 0x000000D2
    AccountCreate_AccountPromotionFailedInvalidEmail = 211, // 0x000000D3
    AccountCreate_AccountPromotionFailedAlreadyExists = 212, // 0x000000D4
    AccountCreate_AccountPromotionFailedUnknown = 213, // 0x000000D5
    AccountCreate_DetailsUpdateFailedInvalidLanguage = 214, // 0x000000D6
    AccountCreate_DetailsUpdateFailedInvalidCountry = 215, // 0x000000D7
    AccountCreate_DetailsUpdateFailedInvalidDoB = 216, // 0x000000D8
    AccountCreate_DetailsUpdateFailedUnknown = 217, // 0x000000D9
    AccountCreate_GameSparksAuthenticationFailed = 218, // 0x000000DA
    AccountCreate_GameSparksRegistrationFailed = 219, // 0x000000DB
    AccountCreate_GameSparksUnknownFailure = 220, // 0x000000DC
    Login_AccountAlreadyLoggedIn = 300, // 0x0000012C
    Login_AccountLoginFailedNotAuthorized = 301, // 0x0000012D
    Login_AccountLoginFailedUnknown = 302, // 0x0000012E
    Login_AccountMergeShadowFailed = 303, // 0x0000012F
    Login_AccountGetMergeStatusFailed = 304, // 0x00000130
    Login_AccountFailedToMerge = 305, // 0x00000131
    Login_AccountUnknownMergeStatus = 306, // 0x00000132
    Login_AccountGetDetailsFailed = 307, // 0x00000133
    Login_GameSparksAuthenticationFailed = 308, // 0x00000134
    Logout_AccountAlreadyLoggedOut = 400, // 0x00000190
    Logout_AuthTokenInvalidationFailed = 401, // 0x00000191
    Logout_AccountCreateShadowFailed = 402, // 0x00000192
    Logout_AccountLoginShadowFailed = 403, // 0x00000193
    Logout_GameSparksRegistrationFailed = 404, // 0x00000194
    SteamLink_NotAuthenticated = 500, // 0x000001F4
    SteamLink_AlreadyLinkedToSteam = 501, // 0x000001F5
    SteamLink_LinkAlreadyExistWithAnotherAccount = 502, // 0x000001F6
    SteamLink_LinkFailed = 503, // 0x000001F7
    SteamUnlink_NotAuthenticated = 600, // 0x00000258
    SteamUnlink_NotLinkedToSteam = 601, // 0x00000259
    SteamUnlink_UnlinkFailed = 602, // 0x0000025A
    GameSparksAvailable_NotAuthenticated = 700, // 0x000002BC
    GameSparksAvailable_AccountGetMergeStatusFailed = 702, // 0x000002BE
    GameSparksAvailable_AccountFailedToMerge = 703, // 0x000002BF
    GameSparksAvailable_AccountUnknownMergeStatus = 704, // 0x000002C0
    GameSparksAvailable_GameSparksAuthenticationFailed = 705, // 0x000002C1
  }

  public enum AuthenticationState
  {
    LoggedOff,
    ShadowAccount,
    FullAccount,
  }

  public delegate void BecameBusyDelegate();

  public delegate void BecameIdleDelegate();
}
