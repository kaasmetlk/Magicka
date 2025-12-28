using System;
using System.Collections.Generic;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.WebTools.GameSparks;
using Magicka.WebTools.Paradox;
using Magicka.WebTools.Paradox.AccountSequence;

namespace Magicka.WebTools
{
	// Token: 0x02000322 RID: 802
	public class ParadoxAccount : Singleton<ParadoxAccount>
	{
		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06001895 RID: 6293 RVA: 0x000A2DE8 File Offset: 0x000A0FE8
		public bool IsLoggedShadow
		{
			get
			{
				bool result;
				lock (this.mAccountLock)
				{
					result = (this.mAuthenticationState == ParadoxAccount.AuthenticationState.ShadowAccount);
				}
				return result;
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x06001896 RID: 6294 RVA: 0x000A2E28 File Offset: 0x000A1028
		public bool IsLoggedFull
		{
			get
			{
				bool result;
				lock (this.mAccountLock)
				{
					result = (this.mAuthenticationState == ParadoxAccount.AuthenticationState.FullAccount);
				}
				return result;
			}
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x06001897 RID: 6295 RVA: 0x000A2E68 File Offset: 0x000A1068
		public bool IsLoggedOff
		{
			get
			{
				bool result;
				lock (this.mAccountLock)
				{
					result = (this.mAuthenticationState == ParadoxAccount.AuthenticationState.LoggedOff);
				}
				return result;
			}
		}

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x06001898 RID: 6296 RVA: 0x000A2EA8 File Offset: 0x000A10A8
		// (set) Token: 0x06001899 RID: 6297 RVA: 0x000A2EB0 File Offset: 0x000A10B0
		public string Email
		{
			get
			{
				return this.mCurrentAccountEmail;
			}
			set
			{
				this.mCurrentAccountEmail = value;
			}
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x0600189A RID: 6298 RVA: 0x000A2EB9 File Offset: 0x000A10B9
		public bool IsBusy
		{
			get
			{
				return this.mCurrentSequence != null && !this.mCurrentSequence.Completed;
			}
		}

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x0600189B RID: 6299 RVA: 0x000A2ED3 File Offset: 0x000A10D3
		// (set) Token: 0x0600189C RID: 6300 RVA: 0x000A2EDB File Offset: 0x000A10DB
		public bool IsLinkedToSteam
		{
			get
			{
				return this.mLinkedToSteam;
			}
			set
			{
				this.mLinkedToSteam = value;
			}
		}

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x0600189D RID: 6301 RVA: 0x000A2EE4 File Offset: 0x000A10E4
		public ParadoxAccount.ErrorCode PendingErrorCode
		{
			get
			{
				ParadoxAccount.ErrorCode result;
				lock (this.mAccountLock)
				{
					result = this.mPendingErrorCode;
				}
				return result;
			}
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x000A2F4C File Offset: 0x000A114C
		public void Update()
		{
			if (this.mCurrentSequence != null && this.mCurrentSequence.Completed)
			{
				if (this.mPendingSequences.Count > 0)
				{
					this.mCurrentSequence = this.mPendingSequences.Dequeue();
					this.mCurrentSequence.Execute();
					if (ParadoxAccount.OnBecameBusy != null)
					{
						ParadoxAccount.OnBecameBusy();
					}
					Logger.LogDebug(Logger.Source.ParadoxAccount, "Starting new sequence.");
					return;
				}
				this.mCurrentSequence = null;
				if (ParadoxAccount.OnBecameIdle != null)
				{
					ParadoxAccount.OnBecameIdle();
				}
				Logger.LogDebug(Logger.Source.ParadoxAccount, "Disposed of a finished sequence.");
			}
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x000A2FD8 File Offset: 0x000A11D8
		public void SetAuthenticationState(ParadoxAccount.AuthenticationState iNewState)
		{
			lock (this.mAccountLock)
			{
				this.mAuthenticationState = iNewState;
			}
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x000A3014 File Offset: 0x000A1214
		public void SetPendingErrorCode(ParadoxAccount.ErrorCode iErrorCode)
		{
			lock (this.mAccountLock)
			{
				this.mPendingErrorCode = iErrorCode;
			}
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x000A3050 File Offset: 0x000A1250
		public ParadoxAccount.ErrorCode ConsumePendingErrorCode()
		{
			ParadoxAccount.ErrorCode result = this.mPendingErrorCode;
			this.mPendingErrorCode = ParadoxAccount.ErrorCode.None;
			return result;
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x000A306C File Offset: 0x000A126C
		public void GameStartup(ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
		{
			ParadoxAccountSequence iSequence = new GameStartupSequence(this, iCallback);
			this.PushNewSequence(iSequence);
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x000A3088 File Offset: 0x000A1288
		public void CreatePlayerAccount(string iUsername, string iPassword, string iDateOfBirth, string iCountryCode, bool iSubscribeToNewsletter, ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
		{
			ParadoxAccountSequence iSequence = new PlayerCreateAccountSequence(iUsername, iPassword, iDateOfBirth, iCountryCode, iSubscribeToNewsletter, this, iCallback);
			this.PushNewSequence(iSequence);
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x000A30AC File Offset: 0x000A12AC
		public void LoginPlayer(string iUsername, string iPassword, ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
		{
			ParadoxAccountSequence iSequence = new PlayerLoginSequence(iUsername, iPassword, this, iCallback);
			this.PushNewSequence(iSequence);
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x000A30CC File Offset: 0x000A12CC
		public void LogOffPlayer(ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
		{
			ParadoxAccountSequence iSequence = new PlayerLogoutSequence(this, iCallback);
			this.PushNewSequence(iSequence);
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x000A30E8 File Offset: 0x000A12E8
		public void LinkSteam(ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
		{
			ParadoxAccountSequence iSequence = new SteamLinkSequence(this, iCallback);
			this.PushNewSequence(iSequence);
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x000A3104 File Offset: 0x000A1304
		public void UnlinkSteam(ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
		{
			ParadoxAccountSequence iSequence = new SteamLinkSequence(this, iCallback);
			this.PushNewSequence(iSequence);
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x000A3120 File Offset: 0x000A1320
		public void ToggleSteamLink(ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
		{
			ParadoxAccountSequence iSequence;
			if (this.IsLinkedToSteam)
			{
				iSequence = new SteamUnlinkSequence(this, iCallback);
			}
			else
			{
				iSequence = new SteamLinkSequence(this, iCallback);
			}
			this.PushNewSequence(iSequence);
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x000A3150 File Offset: 0x000A1350
		public void GameSparksAvailableLogin(ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
		{
			ParadoxAccountSequence iSequence = new GameSparksAvailableSequence(this, iCallback);
			this.PushNewSequence(iSequence);
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x000A316C File Offset: 0x000A136C
		public void GameSparksAvailableCallback(bool iAvailable)
		{
			if (Singleton<ParadoxAccount>.Instance.IsLoggedOff || this.mCurrentSequence is PlayerLogoutSequence)
			{
				GameSparksServices.AvailabilityChanged = (GameSparksServices.AvailabilityChangedDelegate)Delegate.Remove(GameSparksServices.AvailabilityChanged, new GameSparksServices.AvailabilityChangedDelegate(this.GameSparksAvailableCallback));
				return;
			}
			if (iAvailable)
			{
				this.GameSparksAvailableLogin(null);
				GameSparksServices.AvailabilityChanged = (GameSparksServices.AvailabilityChangedDelegate)Delegate.Remove(GameSparksServices.AvailabilityChanged, new GameSparksServices.AvailabilityChangedDelegate(this.GameSparksAvailableCallback));
			}
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x000A31DD File Offset: 0x000A13DD
		private void PushNewSequence(ParadoxAccountSequence iSequence)
		{
			if (this.mCurrentSequence == null)
			{
				this.mCurrentSequence = iSequence;
				this.mCurrentSequence.Execute();
				if (ParadoxAccount.OnBecameBusy != null)
				{
					ParadoxAccount.OnBecameBusy();
					return;
				}
			}
			else
			{
				this.mPendingSequences.Enqueue(iSequence);
			}
		}

		// Token: 0x04001A44 RID: 6724
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccount;

		// Token: 0x04001A45 RID: 6725
		private const string UNKNOWN_ERROR = "unknown-error";

		// Token: 0x04001A46 RID: 6726
		private const string ACCOUNT_EXISTS = "account-exists";

		// Token: 0x04001A47 RID: 6727
		private const string NOT_AUTHORIZED = "not-authorized";

		// Token: 0x04001A48 RID: 6728
		public static ParadoxAccount.BecameBusyDelegate OnBecameBusy;

		// Token: 0x04001A49 RID: 6729
		public static ParadoxAccount.BecameIdleDelegate OnBecameIdle;

		// Token: 0x04001A4A RID: 6730
		private ParadoxAccount.AuthenticationState mAuthenticationState;

		// Token: 0x04001A4B RID: 6731
		private object mAccountLock = new object();

		// Token: 0x04001A4C RID: 6732
		private Queue<ParadoxAccountSequence> mPendingSequences = new Queue<ParadoxAccountSequence>();

		// Token: 0x04001A4D RID: 6733
		private ParadoxAccountSequence mCurrentSequence;

		// Token: 0x04001A4E RID: 6734
		private string mCurrentAccountEmail = string.Empty;

		// Token: 0x04001A4F RID: 6735
		private bool mLinkedToSteam;

		// Token: 0x04001A50 RID: 6736
		private ParadoxAccount.ErrorCode mPendingErrorCode;

		// Token: 0x02000323 RID: 803
		public enum ErrorCode
		{
			// Token: 0x04001A52 RID: 6738
			None,
			// Token: 0x04001A53 RID: 6739
			Startup_FailedLoginWithSteamOrAuthToken = 100,
			// Token: 0x04001A54 RID: 6740
			Startup_ShadowAccountCreationFailed,
			// Token: 0x04001A55 RID: 6741
			Startup_ShadowLoginFailed,
			// Token: 0x04001A56 RID: 6742
			Startup_GetDetailsFailed,
			// Token: 0x04001A57 RID: 6743
			Startup_AccountMergeShadowFailed,
			// Token: 0x04001A58 RID: 6744
			Startup_AccountFailedToMerge,
			// Token: 0x04001A59 RID: 6745
			Startup_AccountUnknownMergeStatus,
			// Token: 0x04001A5A RID: 6746
			Startup_AccountGetMergeStatusFailed,
			// Token: 0x04001A5B RID: 6747
			Startup_GameSparksAuthenticationFailed,
			// Token: 0x04001A5C RID: 6748
			Startup_GameSparksRegistrationFailed,
			// Token: 0x04001A5D RID: 6749
			Startup_GameSparksUnknownFailure,
			// Token: 0x04001A5E RID: 6750
			AccountCreate_AccountAlreadyLoggedIn = 200,
			// Token: 0x04001A5F RID: 6751
			AccountCreate_AccountCreationFailedInvalidEmail,
			// Token: 0x04001A60 RID: 6752
			AccountCreate_AccountCreationFailedBadPasswordLength,
			// Token: 0x04001A61 RID: 6753
			AccountCreate_AccountCreationFailedAlreadyExists,
			// Token: 0x04001A62 RID: 6754
			AccountCreate_AccountCreationFailedInvalidLanguage,
			// Token: 0x04001A63 RID: 6755
			AccountCreate_AccountCreationFailedInvalidCountry,
			// Token: 0x04001A64 RID: 6756
			AccountCreate_AccountCreationFailedInvalidDoB,
			// Token: 0x04001A65 RID: 6757
			AccountCreate_AccountCreationFailedUnder13,
			// Token: 0x04001A66 RID: 6758
			AccountCreate_AccountCreationFailedUnknown,
			// Token: 0x04001A67 RID: 6759
			AccountCreate_CreatedButNotLoggedIn,
			// Token: 0x04001A68 RID: 6760
			AccountCreate_AccountPromotionFailedBadPasswordLength,
			// Token: 0x04001A69 RID: 6761
			AccountCreate_AccountPromotionFailedInvalidEmail,
			// Token: 0x04001A6A RID: 6762
			AccountCreate_AccountPromotionFailedAlreadyExists,
			// Token: 0x04001A6B RID: 6763
			AccountCreate_AccountPromotionFailedUnknown,
			// Token: 0x04001A6C RID: 6764
			AccountCreate_DetailsUpdateFailedInvalidLanguage,
			// Token: 0x04001A6D RID: 6765
			AccountCreate_DetailsUpdateFailedInvalidCountry,
			// Token: 0x04001A6E RID: 6766
			AccountCreate_DetailsUpdateFailedInvalidDoB,
			// Token: 0x04001A6F RID: 6767
			AccountCreate_DetailsUpdateFailedUnknown,
			// Token: 0x04001A70 RID: 6768
			AccountCreate_GameSparksAuthenticationFailed,
			// Token: 0x04001A71 RID: 6769
			AccountCreate_GameSparksRegistrationFailed,
			// Token: 0x04001A72 RID: 6770
			AccountCreate_GameSparksUnknownFailure,
			// Token: 0x04001A73 RID: 6771
			Login_AccountAlreadyLoggedIn = 300,
			// Token: 0x04001A74 RID: 6772
			Login_AccountLoginFailedNotAuthorized,
			// Token: 0x04001A75 RID: 6773
			Login_AccountLoginFailedUnknown,
			// Token: 0x04001A76 RID: 6774
			Login_AccountMergeShadowFailed,
			// Token: 0x04001A77 RID: 6775
			Login_AccountGetMergeStatusFailed,
			// Token: 0x04001A78 RID: 6776
			Login_AccountFailedToMerge,
			// Token: 0x04001A79 RID: 6777
			Login_AccountUnknownMergeStatus,
			// Token: 0x04001A7A RID: 6778
			Login_AccountGetDetailsFailed,
			// Token: 0x04001A7B RID: 6779
			Login_GameSparksAuthenticationFailed,
			// Token: 0x04001A7C RID: 6780
			Logout_AccountAlreadyLoggedOut = 400,
			// Token: 0x04001A7D RID: 6781
			Logout_AuthTokenInvalidationFailed,
			// Token: 0x04001A7E RID: 6782
			Logout_AccountCreateShadowFailed,
			// Token: 0x04001A7F RID: 6783
			Logout_AccountLoginShadowFailed,
			// Token: 0x04001A80 RID: 6784
			Logout_GameSparksRegistrationFailed,
			// Token: 0x04001A81 RID: 6785
			SteamLink_NotAuthenticated = 500,
			// Token: 0x04001A82 RID: 6786
			SteamLink_AlreadyLinkedToSteam,
			// Token: 0x04001A83 RID: 6787
			SteamLink_LinkAlreadyExistWithAnotherAccount,
			// Token: 0x04001A84 RID: 6788
			SteamLink_LinkFailed,
			// Token: 0x04001A85 RID: 6789
			SteamUnlink_NotAuthenticated = 600,
			// Token: 0x04001A86 RID: 6790
			SteamUnlink_NotLinkedToSteam,
			// Token: 0x04001A87 RID: 6791
			SteamUnlink_UnlinkFailed,
			// Token: 0x04001A88 RID: 6792
			GameSparksAvailable_NotAuthenticated = 700,
			// Token: 0x04001A89 RID: 6793
			GameSparksAvailable_AccountGetMergeStatusFailed = 702,
			// Token: 0x04001A8A RID: 6794
			GameSparksAvailable_AccountFailedToMerge,
			// Token: 0x04001A8B RID: 6795
			GameSparksAvailable_AccountUnknownMergeStatus,
			// Token: 0x04001A8C RID: 6796
			GameSparksAvailable_GameSparksAuthenticationFailed
		}

		// Token: 0x02000324 RID: 804
		public enum AuthenticationState
		{
			// Token: 0x04001A8E RID: 6798
			LoggedOff,
			// Token: 0x04001A8F RID: 6799
			ShadowAccount,
			// Token: 0x04001A90 RID: 6800
			FullAccount
		}

		// Token: 0x02000325 RID: 805
		// (Invoke) Token: 0x060018AE RID: 6318
		public delegate void BecameBusyDelegate();

		// Token: 0x02000326 RID: 806
		// (Invoke) Token: 0x060018B2 RID: 6322
		public delegate void BecameIdleDelegate();
	}
}
