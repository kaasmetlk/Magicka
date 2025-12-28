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
	// Token: 0x020003D4 RID: 980
	public class PlayerLoginSequence : ParadoxAccountSequence
	{
		// Token: 0x06001DFB RID: 7675 RVA: 0x000D3414 File Offset: 0x000D1614
		public PlayerLoginSequence(string iUsername, string iPassword, ParadoxAccount iAccount, ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : base(iAccount, iCallback)
		{
			this.mUsername = iUsername;
			this.mPassword = iPassword;
		}

		// Token: 0x06001DFC RID: 7676 RVA: 0x000D344E File Offset: 0x000D164E
		protected override void OnExecute()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, "Starting PlayerLogin Sequence");
			if (base.Account.IsLoggedFull)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountAlreadyLoggedIn);
				return;
			}
			this.RequestAccountLogin();
		}

		// Token: 0x06001DFD RID: 7677 RVA: 0x000D347C File Offset: 0x000D167C
		private void RequestAccountLogin()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, "RequestAccountLogin");
			ParadoxAccountSequence.SequencePhase[] iExpected = new ParadoxAccountSequence.SequencePhase[1];
			if (base.CheckPhase(iExpected))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLogin);
				Singleton<ParadoxServices>.Instance.AccountLogin(this.mUsername, this.mPassword, new ParadoxServices.AccountLoginDelegate(this.AccountLoginCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountLoginCallback));
			}
		}

		// Token: 0x06001DFE RID: 7678 RVA: 0x000D34DC File Offset: 0x000D16DC
		private void RequestAccountMergeShadow()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, "RequestAccountMergeShadow");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountGetDetails
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountMergeShadow);
				Singleton<ParadoxServices>.Instance.AccountMergeShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountMergeShadowDelegate(this.AccountMergeShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountMergeShadowCallback));
			}
		}

		// Token: 0x06001DFF RID: 7679 RVA: 0x000D3544 File Offset: 0x000D1744
		private void RequestAccountGetMergeStatus()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, "RequestAccountGetMergeStatus");
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

		// Token: 0x06001E00 RID: 7680 RVA: 0x000D35A8 File Offset: 0x000D17A8
		private void RequestAccountGetDetails()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, "RequestAccountGetDetails");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountConnections
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountGetDetails);
				Singleton<ParadoxServices>.Instance.AccountGetDetails(new ParadoxServices.AccountGetDetailsDelegate(this.AccountGetDetailsCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountGetDetailsFailedCallback));
			}
		}

		// Token: 0x06001E01 RID: 7681 RVA: 0x000D3600 File Offset: 0x000D1800
		private void RequestAccountConnections()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, "RequestAccountConnections");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLogin
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountConnections);
				Singleton<ParadoxServices>.Instance.AccountConnections(new ParadoxServices.AccountConnectionsDelegate(this.AccountConnectionsCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountConnectionsFailedCallback));
			}
		}

		// Token: 0x06001E02 RID: 7682 RVA: 0x000D3658 File Offset: 0x000D1858
		private void RequestGameSparksAuthentication()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, "RequestGameSparksAuthentication");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus,
				ParadoxAccountSequence.SequencePhase.AccountMergeShadow,
				ParadoxAccountSequence.SequencePhase.AccountGetDetails
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate);
				ParadoxUtils.AuthenticateWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
			}
		}

		// Token: 0x06001E03 RID: 7683 RVA: 0x000D36AC File Offset: 0x000D18AC
		private void RequestTokenInvalidateNoCallback()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogin, "RequestTokenInvalidateNoCallback");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate
			}))
			{
				Singleton<ParadoxServices>.Instance.AuthTokenInvalidate(null, null);
			}
		}

		// Token: 0x06001E04 RID: 7684 RVA: 0x000D36E6 File Offset: 0x000D18E6
		private void AccountLoginCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				base.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.FullAccount);
				Singleton<ParadoxAccountSaveData>.Instance.SetAuthToken(Singleton<ParadoxServices>.Instance.RetrieveAuthToken());
				this.RequestAccountConnections();
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountLoginFailedUnknown);
		}

		// Token: 0x06001E05 RID: 7685 RVA: 0x000D371D File Offset: 0x000D191D
		private void FailedAccountLoginCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountLogin failed : " + iReason);
			if (iReason.Equals("not-authorized"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountLoginFailedNotAuthorized);
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountLoginFailedUnknown);
		}

		// Token: 0x06001E06 RID: 7686 RVA: 0x000D3755 File Offset: 0x000D1955
		private void AccountMergeShadowCallback(string iMergeTaskId)
		{
			this.mMergeTaskId = iMergeTaskId;
			this.RequestAccountGetMergeStatus();
		}

		// Token: 0x06001E07 RID: 7687 RVA: 0x000D3764 File Offset: 0x000D1964
		private void FailedAccountMergeShadowCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountMergeShadow failed : " + iReason);
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001E08 RID: 7688 RVA: 0x000D3780 File Offset: 0x000D1980
		private void AccountGetMergeStatusCallback(PopsApiWrapper.AccountGetMergeStatusResult iResult)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountGetMergeStatus received : [" + iResult.Status + "] " + iResult.StatusMessage);
			if (iResult.Status.Equals("pending"))
			{
				Thread.Sleep(2000);
				this.RequestAccountGetMergeStatus();
				return;
			}
			if (iResult.Status.Equals("success"))
			{
				Singleton<ParadoxAccountSaveData>.Instance.ClearShadowUniqueId();
				this.RequestGameSparksAuthentication();
				return;
			}
			if (iResult.Status.Equals("failure"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountFailedToMerge);
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountUnknownMergeStatus);
		}

		// Token: 0x06001E09 RID: 7689 RVA: 0x000D3823 File Offset: 0x000D1A23
		private void FailedAccountGetMergeStatusCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountGetMergeStatus failed : " + iReason);
			base.ExitFailure(ParadoxAccount.ErrorCode.Login_AccountGetMergeStatusFailed);
		}

		// Token: 0x06001E0A RID: 7690 RVA: 0x000D3844 File Offset: 0x000D1A44
		private void AccountConnectionsCallback(ICollection<PopsApiWrapper.Connection> iConnections)
		{
			Singleton<ParadoxServices>.Instance.RetrieveAccountGuid();
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

		// Token: 0x06001E0B RID: 7691 RVA: 0x000D38C4 File Offset: 0x000D1AC4
		private void AccountConnectionsFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountConnections failed : " + iReason);
			this.RequestAccountGetDetails();
		}

		// Token: 0x06001E0C RID: 7692 RVA: 0x000D38E0 File Offset: 0x000D1AE0
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

		// Token: 0x06001E0D RID: 7693 RVA: 0x000D394F File Offset: 0x000D1B4F
		private void AccountGetDetailsFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerLogin, "AccountGetDetails failed : " + iReason);
			if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
			{
				this.RequestAccountMergeShadow();
				return;
			}
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x06001E0E RID: 7694 RVA: 0x000D397C File Offset: 0x000D1B7C
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
			base.ExitFailure(ParadoxAccount.ErrorCode.Login_GameSparksAuthenticationFailed);
		}

		// Token: 0x0400207D RID: 8317
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountPlayerLogin;

		// Token: 0x0400207E RID: 8318
		private const string MERGE_STATUS_PENDING = "pending";

		// Token: 0x0400207F RID: 8319
		private const string MERGE_STATUS_SUCCESS = "success";

		// Token: 0x04002080 RID: 8320
		private const string MERGE_STATUS_FAILURE = "failure";

		// Token: 0x04002081 RID: 8321
		private const string UNIVERSE_STEAM = "steam";

		// Token: 0x04002082 RID: 8322
		private const string NOT_AUTHORIZED = "not-authorized";

		// Token: 0x04002083 RID: 8323
		private readonly string mUsername = string.Empty;

		// Token: 0x04002084 RID: 8324
		private readonly string mPassword = string.Empty;

		// Token: 0x04002085 RID: 8325
		private string mMergeTaskId = string.Empty;
	}
}
