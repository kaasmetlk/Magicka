using System;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;

namespace Magicka.WebTools.Paradox.AccountSequence
{
	// Token: 0x02000594 RID: 1428
	public class SteamLinkSequence : ParadoxAccountSequence
	{
		// Token: 0x06002AA3 RID: 10915 RVA: 0x00150650 File Offset: 0x0014E850
		public SteamLinkSequence(ParadoxAccount iAccount, ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : base(iAccount, iCallback)
		{
		}

		// Token: 0x06002AA4 RID: 10916 RVA: 0x0015065A File Offset: 0x0014E85A
		protected override void OnExecute()
		{
			if (!base.Account.IsLoggedFull)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_NotAuthenticated);
				return;
			}
			if (base.Account.IsLinkedToSteam)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_AlreadyLinkedToSteam);
				return;
			}
			this.RequestAccountConnectAccountSteam();
		}

		// Token: 0x06002AA5 RID: 10917 RVA: 0x00150694 File Offset: 0x0014E894
		private void RequestAccountConnectAccountSteam()
		{
			ParadoxAccountSequence.SequencePhase[] iExpected = new ParadoxAccountSequence.SequencePhase[1];
			if (base.CheckPhase(iExpected))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountConnectAccountSteam);
				string iAppId = InternalSteamUtils.GetSteamAppID().ToString();
				string steamAuthToken = InternalSteamUtils.GetSteamAuthToken();
				Singleton<ParadoxServices>.Instance.AccountConnectAccountSteam(iAppId, steamAuthToken, new ParadoxServices.AccountConnectAccountSteamDelegate(this.AccountConnectAccountSteamCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountConnectAccountSteamFailedCallback));
			}
		}

		// Token: 0x06002AA6 RID: 10918 RVA: 0x001506F1 File Offset: 0x0014E8F1
		private void AccountConnectAccountSteamCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				Singleton<ParadoxAccountSaveData>.Instance.ClearAuthToken();
				base.Account.IsLinkedToSteam = true;
				base.ExitSuccess();
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_LinkFailed);
		}

		// Token: 0x06002AA7 RID: 10919 RVA: 0x0015071E File Offset: 0x0014E91E
		private void AccountConnectAccountSteamFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountSteamLink, "AccountConnectAccountSteam failed : " + iReason);
			if (iReason.Equals("existing-steam-to-account-connection"))
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_LinkAlreadyExistWithAnotherAccount);
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.SteamLink_LinkFailed);
		}

		// Token: 0x04002E00 RID: 11776
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountSteamLink;

		// Token: 0x04002E01 RID: 11777
		private const string ALREADY_LINKED = "existing-steam-to-account-connection";
	}
}
