using System;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;

namespace Magicka.WebTools.Paradox.AccountSequence
{
	// Token: 0x02000091 RID: 145
	public class PlayerLogoutSequence : ParadoxAccountSequence
	{
		// Token: 0x06000430 RID: 1072 RVA: 0x000144EE File Offset: 0x000126EE
		public PlayerLogoutSequence(ParadoxAccount iAccount, ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : base(iAccount, iCallback)
		{
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00014503 File Offset: 0x00012703
		protected override void OnExecute()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, "Starting PlayerLogout Sequence");
			if (base.Account.IsLoggedOff)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountAlreadyLoggedOut);
				return;
			}
			this.RequestTokenInvalidate();
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00014530 File Offset: 0x00012730
		private void RequestTokenInvalidate()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, "RequestTokenInvalidate");
			ParadoxAccountSequence.SequencePhase[] iExpected = new ParadoxAccountSequence.SequencePhase[1];
			if (base.CheckPhase(iExpected))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AuthTokenInvalidate);
				Singleton<ParadoxServices>.Instance.AuthTokenInvalidate(new ParadoxServices.AuthTokenInvalidateDelegate(this.AuthTokenInvalidateCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAuthTokenInvalidateCallback));
			}
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x00014584 File Offset: 0x00012784
		private void RequestTokenInvalidateNoCallback()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, "RequestTokenInvalidateNoCallback");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.GameSparksRegister
			}))
			{
				Singleton<ParadoxServices>.Instance.AuthTokenInvalidate(null, null);
			}
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x000145C0 File Offset: 0x000127C0
		private void RequestAccountCreateShadow()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, "RequestAccountCreateShadow");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AuthTokenInvalidate
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountCreateShadow);
				this.mTemporaryUniqueId = HardwareInfoManager.GenerateUniqueSessionId();
				Singleton<ParadoxServices>.Instance.AccountCreateShadow(this.mTemporaryUniqueId, "generated", "pc_steam_red_wizard_log_out", new ParadoxServices.AccountCreateShadowDelegate(this.AccountCreateShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountCreateShadowCallback));
			}
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00014634 File Offset: 0x00012834
		private void RequestAccountLoginShadow()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, "RequestAccountLoginShadow");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountCreateShadow
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountLoginShadow);
				Singleton<ParadoxServices>.Instance.AccountLoginShadow(Singleton<ParadoxAccountSaveData>.Instance.ShadowUniqueId, "generated", new ParadoxServices.AccountLoginShadowDelegate(this.AccountLoginShadowCallback), new ParadoxServices.HandledExceptionDelegate(this.FailedAccountLoginShadowCallback));
			}
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x0001469C File Offset: 0x0001289C
		private void RequestGameSparksRegistration()
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountPlayerLogout, "RequestGameSparksRegistration");
			if (base.CheckPhase(new ParadoxAccountSequence.SequencePhase[]
			{
				ParadoxAccountSequence.SequencePhase.AccountLoginShadow
			}))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.GameSparksRegister);
				ParadoxUtils.RegisterWithGameSparks(new GameSparksAccount.OperationCompleteDelegate(this.GameSparksOperationCallback));
			}
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x000146E4 File Offset: 0x000128E4
		private void AuthTokenInvalidateCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				base.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.LoggedOff);
				base.Account.Email = string.Empty;
				base.Account.IsLinkedToSteam = false;
				Singleton<ParadoxAccountSaveData>.Instance.Reset();
				Singleton<GameSparksAccount>.Instance.LogOut();
				this.RequestAccountCreateShadow();
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.Logout_AuthTokenInvalidationFailed);
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00014742 File Offset: 0x00012942
		private void FailedAuthTokenInvalidateCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerLogout, "AuthTokenInvalidate failed : " + iReason);
			base.ExitFailure(ParadoxAccount.ErrorCode.Logout_AuthTokenInvalidationFailed);
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x00014761 File Offset: 0x00012961
		private void AccountCreateShadowCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				Singleton<ParadoxAccountSaveData>.Instance.SetShadowUniqueId(this.mTemporaryUniqueId);
				this.mTemporaryUniqueId = string.Empty;
				this.RequestAccountLoginShadow();
				return;
			}
			this.mTemporaryUniqueId = string.Empty;
			base.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountCreateShadowFailed);
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0001479E File Offset: 0x0001299E
		private void FailedAccountCreateShadowCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerLogout, "AccountCreateShadow failed : " + iReason);
			this.mTemporaryUniqueId = string.Empty;
			base.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountCreateShadowFailed);
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x000147C8 File Offset: 0x000129C8
		private void AccountLoginShadowCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				base.Account.SetAuthenticationState(ParadoxAccount.AuthenticationState.ShadowAccount);
				this.RequestGameSparksRegistration();
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountLoginShadowFailed);
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x000147EB File Offset: 0x000129EB
		private void FailedAccountLoginShadowCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountPlayerLogout, "AccountCreateShadow failed : " + iReason);
			base.ExitFailure(ParadoxAccount.ErrorCode.Logout_AccountLoginShadowFailed);
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x0001480C File Offset: 0x00012A0C
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
			base.ExitFailure(ParadoxAccount.ErrorCode.Logout_GameSparksRegistrationFailed);
		}

		// Token: 0x040002A0 RID: 672
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountPlayerLogout;

		// Token: 0x040002A1 RID: 673
		private const string SOURCE_SERVICE = "pc_steam_red_wizard_log_out";

		// Token: 0x040002A2 RID: 674
		private string mTemporaryUniqueId = string.Empty;

		// Token: 0x040002A3 RID: 675
		private ParadoxAccount.ErrorCode mGameSparksFailedCode;
	}
}
