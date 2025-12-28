using System;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.Storage;

namespace Magicka.WebTools.Paradox.AccountSequence
{
	// Token: 0x020002BF RID: 703
	public class SteamUnlinkSequence : ParadoxAccountSequence
	{
		// Token: 0x0600152A RID: 5418 RVA: 0x000866BD File Offset: 0x000848BD
		public SteamUnlinkSequence(ParadoxAccount iAccount, ParadoxAccountSequence.ExecutionDoneDelegate iCallback) : base(iAccount, iCallback)
		{
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x000866C7 File Offset: 0x000848C7
		protected override void OnExecute()
		{
			if (!base.Account.IsLoggedFull)
			{
				base.ExitFailure(ParadoxAccount.ErrorCode.SteamUnlink_NotAuthenticated);
				return;
			}
			if (base.Account.IsLinkedToSteam)
			{
				this.RequestAccountDisconnectAccountSteam();
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.SteamUnlink_NotLinkedToSteam);
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x00086704 File Offset: 0x00084904
		private void RequestAccountDisconnectAccountSteam()
		{
			ParadoxAccountSequence.SequencePhase[] iExpected = new ParadoxAccountSequence.SequencePhase[1];
			if (base.CheckPhase(iExpected))
			{
				base.ChangePhase(ParadoxAccountSequence.SequencePhase.AccountDisconnectAccountSteam);
				Singleton<ParadoxServices>.Instance.AccountDisconnectAccountSteam(new ParadoxServices.AccountDisconnectAccountSteamDelegate(this.AccountDisconnectAccountSteamCallback), new ParadoxServices.HandledExceptionDelegate(this.AccountDisconnectAccountSteamFailedCallback));
			}
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x0008674B File Offset: 0x0008494B
		private void AccountDisconnectAccountSteamCallback(bool iSuccess)
		{
			if (iSuccess)
			{
				Singleton<ParadoxAccountSaveData>.Instance.SetAuthToken(Singleton<ParadoxServices>.Instance.RetrieveAuthToken());
				base.Account.IsLinkedToSteam = false;
				base.ExitSuccess();
				return;
			}
			base.ExitFailure(ParadoxAccount.ErrorCode.SteamUnlink_UnlinkFailed);
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x00086782 File Offset: 0x00084982
		private void AccountDisconnectAccountSteamFailedCallback(string iReason)
		{
			Logger.LogError(Logger.Source.ParadoxAccountSteamUnlink, "AccountDisconnectAccountSteam failed : " + iReason);
			base.ExitFailure(ParadoxAccount.ErrorCode.SteamUnlink_UnlinkFailed);
		}

		// Token: 0x040016D5 RID: 5845
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountSteamUnlink;
	}
}
