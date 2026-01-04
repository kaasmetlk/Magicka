// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.AccountSequence.PlayerCreateAccountSequence
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.Storage;
using System;
using System.Globalization;

#nullable disable
namespace Magicka.WebTools.Paradox.AccountSequence;

public class PlayerCreateAccountSequence : ParadoxAccountSequence
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountPlayerCreate;
  private const string SOURCE_SERVICE = "pc_steam_red_wizard";
  private const string INVALID_EMAIL = "invalid-email";
  private const string BAD_PASSWORD_LENGTH = "bad-password-length";
  private const string ACCOUNT_ALREADY_EXISTS = "account-exists";
  private const string INVALID_LANGUAGE_CODE = "invalid-language-code";
  private const string INVALID_COUNTRY_CODE = "invalid-country-code";
  private const string INVALID_DATE_OF_BIRTH = "invalid-ISO-8601-format";
  private const string USER_UNDER_13 = "ua";
  private readonly string mUsername = string.Empty;
  private readonly string mPassword = string.Empty;
  private readonly string mDateOfBirth = string.Empty;
  private readonly string mCountryCode = string.Empty;
  private readonly bool mSubscribeToNewsLetter;
  private bool mWasPromoted;

  public PlayerCreateAccountSequence(
    string iUsername,
    string iPassword,
    string iDateOfBirth,
    string iCountryCode,
    bool iSubscribeToNewsletter,
    ParadoxAccount iAccount,
    ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
    : base(iAccount, iCallback)
  {
    this.mUsername = iUsername;
    this.mPassword = iPassword;
    this.mDateOfBirth = iDateOfBirth;
    this.mCountryCode = iCountryCode;
    this.mSubscribeToNewsLetter = iSubscribeToNewsletter;
  }

  protected override void OnExecute()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "Starting PlayerCreateAccount Sequence");
    if (this.Account.IsLoggedFull)
      this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountAlreadyLoggedIn);
    else if (this.Account.IsLoggedShadow)
      this.RequestAccountAddCredentials();
    else
      this.RequestCreateAccount();
  }

  public void RequestCreateAccount()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, nameof (RequestCreateAccount));
    if (!this.CheckPhase(new ParadoxAccountSequence.SequencePhase[1]))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountCreate);
    Singleton<ParadoxServices>.Instance.AccountCreate(this.mUsername, this.mPassword, LanguageManager.Instance.ISO6391(LanguageManager.Instance.CurrentLanguage), this.mCountryCode, new DateTime?(DateTime.Parse(this.mDateOfBirth, (IFormatProvider) new CultureInfo("en-US"))), (string) null, (string) null, "pc_steam_red_wizard", (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, new ParadoxServices.AccountCreateDelegate(this.AccountCreateCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountCreateCallback));
  }

  private void RequestAccountLogin()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, nameof (RequestAccountLogin));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountAddCredentials, ParadoxAccountSequence.SequencePhase.AccountCreate))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLogin);
    Singleton<ParadoxServices>.Instance.AccountLogin(this.mUsername, this.mPassword, new ParadoxServices.AccountLoginDelegate(this.AccountLoginCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountLoginCallback));
  }

  private void RequestAccountAddCredentials()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, nameof (RequestAccountAddCredentials));
    if (!this.CheckPhase(new ParadoxAccountSequence.SequencePhase[1]))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountAddCredentials);
    Singleton<ParadoxServices>.Instance.AccountAddCredentials(this.mUsername, this.mPassword, (string) null, (string) null, false, new ParadoxServices.AccountAddCredentialsDelegate(this.AccountAddCredentialsCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountAddCredentialsCallback));
  }

  private void RequestAccountUpdateDetails()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, nameof (RequestAccountUpdateDetails));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLogin))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountUpdateDetails);
    Singleton<ParadoxServices>.Instance.AccountUpdateDetails((string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, this.mCountryCode, (string) null, LanguageManager.Instance.ISO6391(LanguageManager.Instance.CurrentLanguage), DateTime.Parse(this.mDateOfBirth, (IFormatProvider) new CultureInfo("en-US")).ToString("yyyy'/'MM'/'dd"), new ParadoxServices.AccountUpdateDetailsDelegate(this.AccountUpdateDetailsCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountUpdateDetailsCallback));
  }

  private void RequestSubscribeToNewsletter()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, nameof (RequestSubscribeToNewsletter));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLogin))
      return;
    Singleton<ParadoxServices>.Instance.NewsletterSubscribe("red_wizard", (ParadoxServices.NewsletterSubscribeDelegate) null, (ParadoxServices.HandledExceptionDelegate) null);
  }

  private void RequestGameSparksRegistration()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, nameof (RequestGameSparksRegistration));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountLogin))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksRegister);
    ParadoxUtils.RegisterWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
  }

  private void RequestGameSparksAuthentication()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, nameof (RequestGameSparksAuthentication));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.AccountUpdateDetails))
      return;
    this.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate);
    ParadoxUtils.AuthenticateWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
  }

  private void RequestTokenInvalidateNoCallback()
  {
    Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, nameof (RequestTokenInvalidateNoCallback));
    if (!this.CheckPhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate, ParadoxAccountSequence.SequencePhase.GameSparksRegister))
      return;
    Singleton<ParadoxServices>.Instance.AuthTokenInvalidate((ParadoxServices.AuthTokenInvalidateDelegate) null, (ParadoxServices.HandledExceptionDelegate) null);
  }

  private void AccountCreateCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      Singleton<ParadoxAccountSaveData>.Instance.SetAuthToken(Singleton<ParadoxServices>.Instance.RetrieveAuthToken());
      this.RequestAccountLogin();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedUnknown);
  }

  private void FailedAccountCreateCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerCreate, "AccountCreate failed : " + iReason);
    switch (iReason)
    {
      case "invalid-email":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidEmail);
        break;
      case "bad-password-length":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedBadPasswordLength);
        break;
      case "account-exists":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedAlreadyExists);
        break;
      case "invalid-language-code":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidLanguage);
        break;
      case "invalid-country-code":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidCountry);
        break;
      case "invalid-ISO-8601-format":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidDoB);
        break;
      case "ua":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedUnder13);
        break;
      default:
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedUnknown);
        break;
    }
  }

  private void AccountLoginCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      this.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.FullAccount);
      if (this.mSubscribeToNewsLetter)
        this.RequestSubscribeToNewsletter();
      if (this.mWasPromoted)
        this.RequestAccountUpdateDetails();
      else
        this.RequestGameSparksRegistration();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_CreatedButNotLoggedIn);
  }

  private void FailedAccountLoginCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerCreate, "AccountLogin failed : " + iReason);
    this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_CreatedButNotLoggedIn);
  }

  private void AccountAddCredentialsCallback(bool iSuccess)
  {
    if (iSuccess)
    {
      this.mWasPromoted = true;
      Singleton<ParadoxAccountSaveData>.Instance.Promote(Singleton<ParadoxServices>.Instance.RetrieveAuthToken());
      this.RequestAccountLogin();
    }
    else
      this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedUnknown);
  }

  private void FailedAccountAddCredentialsCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerCreate, "AccountAddCredentials failed : " + iReason);
    switch (iReason)
    {
      case "bad-password-length":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedBadPasswordLength);
        break;
      case "invalid-email":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedInvalidEmail);
        break;
      case "account-exists":
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedAlreadyExists);
        break;
      default:
        this.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedUnknown);
        break;
    }
  }

  private void AccountUpdateDetailsCallback(bool iSuccess)
  {
    this.RequestGameSparksAuthentication();
  }

  private void FailedAccountUpdateDetailsCallback(string iReason)
  {
    Logger.LogError(Logger.Source.ParadoxAccountPlayerCreate, "AccountUpdateDetails failed : " + iReason);
    this.RequestGameSparksAuthentication();
  }

  private void GameSparksOperationCallback(GameSparksAccount.Result iResult)
  {
    if (iResult == GameSparksAccount.Result.Success)
    {
      this.Account.Email = this.mUsername;
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
