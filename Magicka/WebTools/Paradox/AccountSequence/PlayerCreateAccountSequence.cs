using System;
using System.Globalization;
using Magicka.CoreFramework;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.Storage;

namespace Magicka.WebTools.Paradox.AccountSequence
{
	// Token: 0x020003D5 RID: 981
	public class PlayerCreateAccountSequence : ParadoxAccountSequence
	{
		// Token: 0x06001E0F RID: 7695 RVA: 0x000D39E0 File Offset: 0x000D1BE0
		public PlayerCreateAccountSequence(string iUsername, string iPassword, string iDateOfBirth, string iCountryCode, bool iSubscribeToNewsletter, ParadoxAccount iAccount, ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : base(iAccount, iCallback)
		{
			this.mUsername = iUsername;
			this.mPassword = iPassword;
			this.mDateOfBirth = iDateOfBirth;
			this.mCountryCode = iCountryCode;
			this.mSubscribeToNewsLetter = iSubscribeToNewsletter;
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x000D3A48 File Offset: 0x000D1C48
		protected override void OnExecute()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "Starting PlayerCreateAccount Sequence");
			if (base.Account.IsLoggedFull)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountAlreadyLoggedIn);
				return;
			}
			if (base.Account.IsLoggedShadow)
			{
				this.RequestAccountAddCredentials();
				return;
			}
			this.RequestCreateAccount();
		}

		// Token: 0x06001E11 RID: 7697 RVA: 0x000D3A94 File Offset: 0x000D1C94
		public void RequestCreateAccount()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "RequestCreateAccount");
			ParadoxAccountSequence.SequencePhase[] iExpected = new ParadoxAccountSequence.SequencePhase[1];
			if (base.CheckPhase(iExpected))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountCreate);
				string iLanguageCode = LanguageManager.Instance.ISO6391(LanguageManager.Instance.CurrentLanguage);
				Singleton<ParadoxServices>.Instance.AccountCreate(this.mUsername, this.mPassword, iLanguageCode, this.mCountryCode, new DateTime?(DateTime.Parse(this.mDateOfBirth, new CultureInfo("en-US"))), null, null, "pc_steam_red_wizard", null, null, null, null, null, null, null, null, null, null, null, null, null, null, new ParadoxServices.AccountCreateDelegate(this.AccountCreateCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountCreateCallback));
			}
		}

		// Token: 0x06001E12 RID: 7698 RVA: 0x000D3B40 File Offset: 0x000D1D40
		private void RequestAccountLogin()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "RequestAccountLogin");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountAddCredentials,
				ParadoxAccountSequence.SequencePhase.AccountCreate
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLogin);
				Singleton<ParadoxServices>.Instance.AccountLogin(this.mUsername, this.mPassword, new ParadoxServices.AccountLoginDelegate(this.AccountLoginCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountLoginCallback));
			}
		}

		// Token: 0x06001E13 RID: 7699 RVA: 0x000D3BA8 File Offset: 0x000D1DA8
		private void RequestAccountAddCredentials()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "RequestAccountAddCredentials");
			ParadoxAccountSequence.SequencePhase[] iExpected = new ParadoxAccountSequence.SequencePhase[1];
			if (base.CheckPhase(iExpected))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountAddCredentials);
				Singleton<ParadoxServices>.Instance.AccountAddCredentials(this.mUsername, this.mPassword, null, null, false, new ParadoxServices.AccountAddCredentialsDelegate(this.AccountAddCredentialsCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountAddCredentialsCallback));
			}
		}

		// Token: 0x06001E14 RID: 7700 RVA: 0x000D3C0C File Offset: 0x000D1E0C
		private void RequestAccountUpdateDetails()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "RequestAccountUpdateDetails");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLogin
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountUpdateDetails);
				string iLanguage = LanguageManager.Instance.ISO6391(LanguageManager.Instance.CurrentLanguage);
				Singleton<ParadoxServices>.Instance.AccountUpdateDetails(null, null, null, null, null, null, null, this.mCountryCode, null, iLanguage, DateTime.Parse(this.mDateOfBirth, new CultureInfo("en-US")).ToString("yyyy'/'MM'/'dd"), new ParadoxServices.AccountUpdateDetailsDelegate(this.AccountUpdateDetailsCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountUpdateDetailsCallback));
			}
		}

		// Token: 0x06001E15 RID: 7701 RVA: 0x000D3CA8 File Offset: 0x000D1EA8
		private void RequestSubscribeToNewsletter()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "RequestSubscribeToNewsletter");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLogin
			}))
			{
				Singleton<ParadoxServices>.Instance.NewsletterSubscribe("red_wizard", null, null);
			}
		}

		// Token: 0x06001E16 RID: 7702 RVA: 0x000D3CE8 File Offset: 0x000D1EE8
		private void RequestGameSparksRegistration()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "RequestGameSparksRegistration");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLogin
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksRegister);
				ParadoxUtils.RegisterWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
			}
		}

		// Token: 0x06001E17 RID: 7703 RVA: 0x000D3D30 File Offset: 0x000D1F30
		private void RequestGameSparksAuthentication()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "RequestGameSparksAuthentication");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountUpdateDetails
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate);
				ParadoxUtils.AuthenticateWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
			}
		}

		// Token: 0x06001E18 RID: 7704 RVA: 0x000D3D78 File Offset: 0x000D1F78
		private void RequestTokenInvalidateNoCallback()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerCreate, "RequestTokenInvalidateNoCallback");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate,
				ParadoxAccountSequence.SequencePhase.GameSparksRegister
			}))
			{
				Singleton<ParadoxServices>.Instance.AuthTokenInvalidate(null, null);
			}
		}

		// Token: 0x06001E19 RID: 7705 RVA: 0x000D3DB7 File Offset: 0x000D1FB7
		private void AccountCreateCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				Singleton<ParadoxAccountSaveData>.Instance.SetAuthToken(Singleton<ParadoxServices>.Instance.RetrieveAuthToken());
				this.RequestAccountLogin();
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedUnknown);
		}

		// Token: 0x06001E1A RID: 7706 RVA: 0x000D3DE4 File Offset: 0x000D1FE4
		private void FailedAccountCreateCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerCreate, "AccountCreate failed : " + iReason);
			if (iReason.Equals("invalid-email"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidEmail);
				return;
			}
			if (iReason.Equals("bad-password-length"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedBadPasswordLength);
				return;
			}
			if (iReason.Equals("account-exists"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedAlreadyExists);
				return;
			}
			if (iReason.Equals("invalid-language-code"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidLanguage);
				return;
			}
			if (iReason.Equals("invalid-country-code"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidCountry);
				return;
			}
			if (iReason.Equals("invalid-ISO-8601-format"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedInvalidDoB);
				return;
			}
			if (iReason.Equals("ua"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedUnder13);
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountCreationFailedUnknown);
		}

		// Token: 0x06001E1B RID: 7707 RVA: 0x000D3EBD File Offset: 0x000D20BD
		private void AccountLoginCallback(bool iSuccess)
		{
			if (!iSuccess)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_CreatedButNotLoggedIn);
				return;
			}
			base.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.FullAccount);
			if (this.mSubscribeToNewsLetter)
			{
				this.RequestSubscribeToNewsletter();
			}
			if (this.mWasPromoted)
			{
				this.RequestAccountUpdateDetails();
				return;
			}
			this.RequestGameSparksRegistration();
		}

		// Token: 0x06001E1C RID: 7708 RVA: 0x000D3EFD File Offset: 0x000D20FD
		private void FailedAccountLoginCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerCreate, "AccountLogin failed : " + iReason);
			base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_CreatedButNotLoggedIn);
		}

		// Token: 0x06001E1D RID: 7709 RVA: 0x000D3F1C File Offset: 0x000D211C
		private void AccountAddCredentialsCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				this.mWasPromoted = true;
				Singleton<ParadoxAccountSaveData>.Instance.Promote(Singleton<ParadoxServices>.Instance.RetrieveAuthToken());
				this.RequestAccountLogin();
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedUnknown);
		}

		// Token: 0x06001E1E RID: 7710 RVA: 0x000D3F50 File Offset: 0x000D2150
		private void FailedAccountAddCredentialsCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerCreate, "AccountAddCredentials failed : " + iReason);
			if (iReason.Equals("bad-password-length"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedBadPasswordLength);
				return;
			}
			if (iReason.Equals("invalid-email"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedInvalidEmail);
				return;
			}
			if (iReason.Equals("account-exists"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedAlreadyExists);
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_AccountPromotionFailedUnknown);
		}

		// Token: 0x06001E1F RID: 7711 RVA: 0x000D3FC5 File Offset: 0x000D21C5
		private void AccountUpdateDetailsCallback(bool iSuccess)
		{
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001E20 RID: 7712 RVA: 0x000D3FCD File Offset: 0x000D21CD
		private void FailedAccountUpdateDetailsCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerCreate, "AccountUpdateDetails failed : " + iReason);
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001E21 RID: 7713 RVA: 0x000D3FE8 File Offset: 0x000D21E8
		private void GameSparksOperationCallback(GameSparksAccount.Result iResult)
		{
			if (iResult == GameSparksAccount.Result.Success)
			{
				base.Account.Email = this.mUsername;
				base.ExitSuccess();
				return;
			}
			this.RequestTokenInvalidateNoCallback();
			base.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.LoggedOff);
			base.Account.Email = string.Empty;
			base.Account.IsLinkedToSteam = false;
			Singleton<ParadoxAccountSaveData>.Instance.Reset();
			Singleton<GameSparksAccount>.Instance.LogOut();
			if (iResult == GameSparksAccount.Result.AuthenticationFailure)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_GameSparksAuthenticationFailed);
				return;
			}
			if (iResult == GameSparksAccount.Result.RegistrationFailure)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_GameSparksRegistrationFailed);
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.AccountCreate_GameSparksUnknownFailure);
		}

		// Token: 0x04002086 RID: 8326
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountPlayerCreate;

		// Token: 0x04002087 RID: 8327
		private const string SOURCE_SERVICE = "pc_steam_red_wizard";

		// Token: 0x04002088 RID: 8328
		private const string INVALID_EMAIL = "invalid-email";

		// Token: 0x04002089 RID: 8329
		private const string BAD_PASSWORD_LENGTH = "bad-password-length";

		// Token: 0x0400208A RID: 8330
		private const string ACCOUNT_ALREADY_EXISTS = "account-exists";

		// Token: 0x0400208B RID: 8331
		private const string INVALID_LANGUAGE_CODE = "invalid-language-code";

		// Token: 0x0400208C RID: 8332
		private const string INVALID_COUNTRY_CODE = "invalid-country-code";

		// Token: 0x0400208D RID: 8333
		private const string INVALID_DATE_OF_BIRTH = "invalid-ISO-8601-format";

		// Token: 0x0400208E RID: 8334
		private const string USER_UNDER_13 = "ua";

		// Token: 0x0400208F RID: 8335
		private readonly string mUsername = string.Empty;

		// Token: 0x04002090 RID: 8336
		private readonly string mPassword = string.Empty;

		// Token: 0x04002091 RID: 8337
		private readonly string mDateOfBirth = string.Empty;

		// Token: 0x04002092 RID: 8338
		private readonly string mCountryCode = string.Empty;

		// Token: 0x04002093 RID: 8339
		private readonly bool mSubscribeToNewsLetter;

		// Token: 0x04002094 RID: 8340
		private bool mWasPromoted;
	}
}
