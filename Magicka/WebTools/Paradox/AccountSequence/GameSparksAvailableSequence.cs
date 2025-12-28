using System;
using System.Threading;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;
using PopsApi;

namespace Magicka.WebTools.Paradox.AccountSequence
{
	// Token: 0x02000327 RID: 807
	public class GameSparksAvailableSequence : ParadoxAccountSequence
	{
		// Token: 0x060018B5 RID: 6325 RVA: 0x000A3217 File Offset: 0x000A1417
		public GameSparksAvailableSequence(ParadoxAccount iAccount, ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : base(iAccount, iCallback)
		{
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x000A322C File Offset: 0x000A142C
		protected override void OnExecute()
		{
			if (!base.Account.IsLoggedFull && !base.Account.IsLoggedShadow)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_NotAuthenticated);
				return;
			}
			if (Singleton<ParadoxAccountSaveData>.Instance.HasShadowUniqueId)
			{
				this.RequestAccountMergeShadow();
				return;
			}
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x000A3278 File Offset: 0x000A1478
		private void RequestAccountMergeShadow()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameSparksAvailable, "RequestAccountMergeShadow");
			ParadoxAccountSequence.SequencePhase[] iExpected = new ParadoxAccountSequence.SequencePhase[1];
			if (base.CheckPhase(iExpected))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountMergeShadow);
				Singleton<ParadoxServices>.Instance.AccountMergeShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountMergeShadowDelegate(this.AccountMergeShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountMergeShadowCallback));
			}
		}

		// Token: 0x060018B8 RID: 6328 RVA: 0x000A32DC File Offset: 0x000A14DC
		private void RequestAccountGetMergeStatus()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameSparksAvailable, "RequestAccountGetMergeStatus");
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

		// Token: 0x060018B9 RID: 6329 RVA: 0x000A3340 File Offset: 0x000A1540
		private void RequestGameSparksAuthentication()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameSparksAvailable, "RequestGameSparksAuthentication");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.Enter,
				ParadoxAccountSequence.SequencePhase.AccountGetMergeStatus,
				ParadoxAccountSequence.SequencePhase.AccountMergeShadow,
				ParadoxAccountSequence.SequencePhase.AccountGetDetails
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate);
				ParadoxUtils.AuthenticateWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
			}
		}

		// Token: 0x060018BA RID: 6330 RVA: 0x000A3394 File Offset: 0x000A1594
		private void RequestTokenInvalidateNoCallback()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountGameSparksAvailable, "RequestTokenInvalidateNoCallback");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.GameSparksAuthenticate
			}))
			{
				Singleton<ParadoxServices>.Instance.AuthTokenInvalidate(null, null);
			}
		}

		// Token: 0x060018BB RID: 6331 RVA: 0x000A33CE File Offset: 0x000A15CE
		private void AccountMergeShadowCallback(string iMergeTaskId)
		{
			this.mMergeTaskId = iMergeTaskId;
			this.RequestAccountGetMergeStatus();
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x000A33DD File Offset: 0x000A15DD
		private void FailedAccountMergeShadowCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameSparksAvailable, "AccountMergeShadow failed : " + iReason);
			this.RequestGameSparksAuthentication();
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x000A33F8 File Offset: 0x000A15F8
		private void AccountGetMergeStatusCallback(PopsApiWrapper.AccountGetMergeStatusResult iResult)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameSparksAvailable, "AccountGetMergeStatus received : [" + iResult.Status + "] " + iResult.StatusMessage);
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
				base.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_AccountFailedToMerge);
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_AccountUnknownMergeStatus);
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x000A349B File Offset: 0x000A169B
		private void FailedAccountGetMergeStatusCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountGameSparksAvailable, "AccountGetMergeStatus failed : " + iReason);
			base.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_AccountGetMergeStatusFailed);
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x000A34BC File Offset: 0x000A16BC
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
			base.ExitFailure(ParadoxAccount.ErrorCode.GameSparksAvailable_GameSparksAuthenticationFailed);
		}

		// Token: 0x04001A91 RID: 6801
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountGameSparksAvailable;

		// Token: 0x04001A92 RID: 6802
		private const string MERGE_STATUS_PENDING = "pending";

		// Token: 0x04001A93 RID: 6803
		private const string MERGE_STATUS_SUCCESS = "success";

		// Token: 0x04001A94 RID: 6804
		private const string MERGE_STATUS_FAILURE = "failure";

		// Token: 0x04001A95 RID: 6805
		private string mMergeTaskId = string.Empty;
	}
}
