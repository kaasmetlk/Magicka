using System;
using System.Collections.Generic;
using System.Threading;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;
using Magicka.WebTools.GameSparks;
using PopsApi;

namespace Magicka.WebTools.Paradox.AccountSequence
{
	// Token: 0x02000369 RID: 873
	public class GameStartupSequence : ParadoxAccountSequence
	{
		// Token: 0x06001AA3 RID: 6819 RVA: 0x000B5081 File Offset: 0x000B3281
		public GameStartupSequence(ParadoxAccount iAccount, ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : base(iAccount, iCallback)
		{
		}

		// Token: 0x06001AA4 RID: 6820 RVA: 0x000B50A4 File Offset: 0x000B32A4
		protected override void OnExecute()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "Starting GameStartup Sequence");
			ParadoxAccountSaveData instance = Singleton<ParadoxAccountSaveData>.Instance;
			if (instance.HasAuthToken)
			{
				this.RequestLoginWithSavedToken();
				return;
			}
			this.RequestLoginInWithSteamTicket();
		}

		// Token: 0x06001AA5 RID: 6821 RVA: 0x000B50D8 File Offset: 0x000B32D8
		private void RequestLoginWithSavedToken()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestLoginWithSavedToken");
			ParadoxAccountSequence.SequencePhase[] iExpected = new ParadoxAccountSequence.SequencePhase[1];
			if (base.CheckPhase(iExpected))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLoginWithAuthToken);
				Singleton<ParadoxServices>.Instance.AccountLoginWithAuthToken(Singleton<ParadoxAccountSaveData>.Instance.AuthToken, new ParadoxServices.AccountLoginWithAuthTokenDelegate(this.AccountLoginWithSavedTokenCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountLoginWithSavedTokenFailedCallback));
			}
		}

		// Token: 0x06001AA6 RID: 6822 RVA: 0x000B5134 File Offset: 0x000B3334
		private void RequestLoginInWithSteamTicket()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestLoginInWithSteamTicket");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.Enter,
				ParadoxAccountSequence.SequencePhase.AccountLoginWithAuthToken
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLoginWithSteamTicket);
				string iAppId = InternalSteamUtils.GetSteamAppID().ToString();
				string steamAuthToken = InternalSteamUtils.GetSteamAuthToken();
				Singleton<ParadoxServices>.Instance.AccountLoginSteamTicket(iAppId, steamAuthToken, new ParadoxServices.AccountLoginSteamTicketDelegate(this.AccountLoginWithSteamTicketCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountLoginWithSteamTicketFailedCallback));
			}
		}

		// Token: 0x06001AA7 RID: 6823 RVA: 0x000B51A0 File Offset: 0x000B33A0
		private void RequestShadowLogin()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestShadowLogin");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLoginWithSteamTicket,
				ParadoxAccountSequence.SequencePhase.AccountCreateShadow
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLoginShadow);
				Singleton<ParadoxServices>.Instance.AccountLoginShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountLoginShadowDelegate(this.AccountLoginShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountShadowLoginFailedCallback));
			}
		}

		// Token: 0x06001AA8 RID: 6824 RVA: 0x000B5208 File Offset: 0x000B3408
		private void RequestCreateShadowAccount()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestCreateShadowAccount");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLoginWithSteamTicket
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountCreateShadow);
				this.mTemporaryUniqueId = HardwareInfoManager.GenerateUniqueSessionId();
				Singleton<ParadoxServices>.Instance.AccountCreateShadow(this.mTemporaryUniqueId, "generated", "pc_steam_red_wizard_startup", new ParadoxServices.AccountCreateShadowDelegate(this.AccountCreateShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountCreateShadowFailedCallback));
			}
		}

		// Token: 0x06001AA9 RID: 6825 RVA: 0x000B5278 File Offset: 0x000B3478
		private void RequestAccountGetDetails()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestAccountGetDetails");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountConnections,
				ParadoxAccountSequence.SequencePhase.AccountLoginWithSteamTicket
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountGetDetails);
				Singleton<ParadoxServices>.Instance.AccountGetDetails(new ParadoxServices.AccountGetDetailsDelegate(this.AccountGetDetailsCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountGetDetailsFailedCallback));
			}
		}

		// Token: 0x06001AAA RID: 6826 RVA: 0x000B52D4 File Offset: 0x000B34D4
		private void RequestAccountConnections()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestAccountConnections");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLoginWithAuthToken
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountConnections);
				Singleton<ParadoxServices>.Instance.AccountConnections(new ParadoxServices.AccountConnectionsDelegate(this.AccountConnectionsCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountConnectionsFailedCallback));
			}
		}

		// Token: 0x06001AAB RID: 6827 RVA: 0x000B532C File Offset: 0x000B352C
		private void RequestAccountMergeShadow()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestAccountMergeShadow");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountGetDetails
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountMergeShadow);
				Singleton<ParadoxServices>.Instance.AccountMergeShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountMergeShadowDelegate(this.AccountMergeShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountMergeShadowCallback));
			}
		}

		// Token: 0x06001AAC RID: 6828 RVA: 0x000B5394 File Offset: 0x000B3594
		private void RequestAccountGetMergeStatus()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestAccountGetMergeStatus");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountMergeShadow,
				ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus);
				Singleton<ParadoxServices>.Instance.AccountGetMergeStatus(this.mMergeTaskId, new ParadoxServices.AccountGetMergeDetailsDelegate(this.AccountGetMergeStatusCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountGetMergeStatusCallback));
			}
		}

		// Token: 0x06001AAD RID: 6829 RVA: 0x000B53F8 File Offset: 0x000B35F8
		private void RequestGameSparksRegistration()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestGameSparksRegistration");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLoginShadow
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksRegister);
				ParadoxUtils.RegisterWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
			}
		}

		// Token: 0x06001AAE RID: 6830 RVA: 0x000B5440 File Offset: 0x000B3640
		private void RequestGameSparksAuthentication()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestGameSparksAuthentication");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLoginShadow,
				ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus,
				ParadoxAccountSequence.SequencePhase.AccountMergeShadow,
				ParadoxAccountSequence.SequencePhase.AccountGetDetails
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate);
				ParadoxUtils.AuthenticateWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
			}
		}

		// Token: 0x06001AAF RID: 6831 RVA: 0x000B5494 File Offset: 0x000B3694
		private void RequestTokenInvalidateNoCallback()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "RequestTokenInvalidateNoCallback");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate,
				ParadoxAccountSequence.SequencePhase.GameSparksRegister
			}))
			{
				Singleton<ParadoxServices>.Instance.AuthTokenInvalidate(null, null);
			}
		}

		// Token: 0x06001AB0 RID: 6832 RVA: 0x000B54D2 File Offset: 0x000B36D2
		private void AccountLoginWithSavedTokenCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				base.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.FullAccount);
				this.RequestAccountConnections();
				return;
			}
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "Failed to login normal account. Fallback to shadow account.");
			this.RequestLoginInWithSteamTicket();
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x000B54FB File Offset: 0x000B36FB
		private void AccountLoginWithSavedTokenFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountLoginWithSavedToken failed : " + iReason);
			this.mAuthTokenAuthenticationFailed = true;
			Singleton<ParadoxAccountSaveData>.Instance.ClearAuthToken();
			this.RequestLoginInWithSteamTicket();
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x000B5528 File Offset: 0x000B3728
		private void AccountLoginWithSteamTicketCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				base.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.FullAccount);
				base.Account.IsLinkedToSteam = true;
				this.RequestAccountGetDetails();
				return;
			}
			if (this.mAuthTokenAuthenticationFailed)
			{
				Singleton<ParadoxAccount>.Instance.SetPendingErrorCode(ParadoxAccount.ErrorCode.Startup_FailedLoginWithSteamOrAuthToken);
			}
			if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
			{
				this.RequestShadowLogin();
				return;
			}
			this.RequestCreateShadowAccount();
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x000B5584 File Offset: 0x000B3784
		private void AccountLoginWithSteamTicketFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountLoginWithSteamTicket failed : " + iReason);
			if (this.mAuthTokenAuthenticationFailed)
			{
				Singleton<ParadoxAccount>.Instance.SetPendingErrorCode(ParadoxAccount.ErrorCode.Startup_FailedLoginWithSteamOrAuthToken);
			}
			if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
			{
				this.RequestShadowLogin();
				return;
			}
			this.RequestCreateShadowAccount();
		}

		// Token: 0x06001AB4 RID: 6836 RVA: 0x000B55C4 File Offset: 0x000B37C4
		private void AccountCreateShadowCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				Singleton<ParadoxAccountSaveData>.Instance.SetShadowUniqueId(this.mTemporaryUniqueId);
				this.mTemporaryUniqueId = string.Empty;
				this.mDidCreateShadowAccount = true;
				this.RequestShadowLogin();
				return;
			}
			Singleton<ParadoxAccountSaveData>.Instance.Reset();
			base.ExitFailure(ParadoxAccount.ErrorCode.Startup_ShadowAccountCreationFailed);
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x000B5604 File Offset: 0x000B3804
		private void AccountCreateShadowFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountCreateShadow failed : " + iReason);
			if (iReason.Equals("account-exists"))
			{
				this.RequestShadowLogin();
				return;
			}
			Singleton<ParadoxAccountSaveData>.Instance.Reset();
			base.ExitFailure(ParadoxAccount.ErrorCode.Startup_ShadowAccountCreationFailed);
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x000B563D File Offset: 0x000B383D
		private void AccountLoginShadowCallback(bool iSuccess)
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameStartup, "AccountLoginShadowCallback.");
			if (!iSuccess)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.Startup_ShadowLoginFailed);
				return;
			}
			base.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.ShadowAccount);
			if (this.mDidCreateShadowAccount)
			{
				this.RequestGameSparksRegistration();
				return;
			}
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001AB7 RID: 6839 RVA: 0x000B5677 File Offset: 0x000B3877
		private void AccountShadowLoginFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountShadowLogin failed : " + iReason);
			base.ExitFailure(ParadoxAccount.ErrorCode.Startup_ShadowLoginFailed);
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x000B5694 File Offset: 0x000B3894
		private void AccountConnectionsCallback(ICollection<PopsApiWrapper.Connection> iConnections)
		{
			foreach (PopsApiWrapper.Connection connection in iConnections)
			{
				if (connection.Universe.Equals("steam"))
				{
					Singleton<ParadoxAccountSaveData>.Instance.ClearAuthToken();
					base.Account.IsLinkedToSteam = true;
					break;
				}
			}
			this.RequestAccountGetDetails();
		}

		// Token: 0x06001AB9 RID: 6841 RVA: 0x000B5708 File Offset: 0x000B3908
		private void AccountConnectionsFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountConnections failed : " + iReason);
			this.RequestAccountGetDetails();
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x000B5724 File Offset: 0x000B3924
		private void AccountGetDetailsCallback(PopsApiWrapper.AccountGetDetailsResult iDetails)
		{
			base.Account.Email = iDetails.Account.Email;
			if (!GameSparksServices.Available)
			{
				GameSparksServices.AvailabilityChanged = (GameSparksServices.AvailabilityChangedDelegate)Delegate.Combine(GameSparksServices.AvailabilityChanged, new GameSparksServices.AvailabilityChangedDelegate(Singleton<ParadoxAccount>.Instance.GameSparksAvailableCallback));
				base.ExitSuccess();
				return;
			}
			if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
			{
				this.RequestAccountMergeShadow();
				return;
			}
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x000B5793 File Offset: 0x000B3993
		private void AccountGetDetailsFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountGetDetails failed : " + iReason);
			if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
			{
				this.RequestAccountMergeShadow();
				return;
			}
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001ABC RID: 6844 RVA: 0x000B57BF File Offset: 0x000B39BF
		private void AccountMergeShadowCallback(string iMergeTaskId)
		{
			this.mMergeTaskId = iMergeTaskId;
			this.RequestAccountGetMergeStatus();
		}

		// Token: 0x06001ABD RID: 6845 RVA: 0x000B57CE File Offset: 0x000B39CE
		private void FailedAccountMergeShadowCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountMergeShadow failed : " + iReason);
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001ABE RID: 6846 RVA: 0x000B57E8 File Offset: 0x000B39E8
		private void AccountGetMergeStatusCallback(PopsApiWrapper.AccountGetMergeStatusResult iResult)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountGetMergeStatus received : [" + iResult.Status + "] " + iResult.StatusMessage);
			if (iResult.Status.Equals("pending"))
			{
				Thread.Sleep(2000);
				this.RequestAccountGetMergeStatus();
				return;
			}
			Singleton<ParadoxAccountSaveData>.Instance.ClearShadowUniqueId();
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x000B584C File Offset: 0x000B3A4C
		private void FailedAccountGetMergeStatusCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameStartup, "AccountGetMergeStatus failed : " + iReason);
			Singleton<ParadoxAccountSaveData>.Instance.ClearShadowUniqueId();
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x000B5870 File Offset: 0x000B3A70
		private void GameSparksOperationCallback(GameSparksAccount.Result iResult)
		{
			if (iResult == GameSparksAccount.Result.Success)
			{
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

		// Token: 0x04001CE7 RID: 7399
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountGameStartup;

		// Token: 0x04001CE8 RID: 7400
		public const string SOURCE_SERVICE = "pc_steam_red_wizard_startup";

		// Token: 0x04001CE9 RID: 7401
		private const string ACCOUNT_EXISTS = "account-exists";

		// Token: 0x04001CEA RID: 7402
		private const string NOT_AUTHORIZED = "not-authorized";

		// Token: 0x04001CEB RID: 7403
		private const string MERGE_STATUS_PENDING = "pending";

		// Token: 0x04001CEC RID: 7404
		private const string MERGE_STATUS_SUCCESS = "success";

		// Token: 0x04001CED RID: 7405
		private const string MERGE_STATUS_FAILURE = "failure";

		// Token: 0x04001CEE RID: 7406
		private const string UNIVERSE_STEAM = "steam";

		// Token: 0x04001CEF RID: 7407
		private string mTemporaryUniqueId = string.Empty;

		// Token: 0x04001CF0 RID: 7408
		private string mMergeTaskId = string.Empty;

		// Token: 0x04001CF1 RID: 7409
		private bool mAuthTokenAuthenticationFailed;

		// Token: 0x04001CF2 RID: 7410
		private bool mDidCreateShadowAccount;
	}
}
