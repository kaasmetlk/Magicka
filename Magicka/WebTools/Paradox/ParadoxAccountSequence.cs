using System;
using Magicka.CoreFramework;

namespace Magicka.WebTools.Paradox
{
	// Token: 0x0200008D RID: 141
	public abstract class ParadoxAccountSequence
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000421 RID: 1057 RVA: 0x000143C0 File Offset: 0x000125C0
		public bool Completed
		{
			get
			{
				return this.mCurrentPhase == ParadoxAccountSequence.SequencePhase.Complete;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000422 RID: 1058 RVA: 0x000143CC File Offset: 0x000125CC
		protected ParadoxAccount Account
		{
			get
			{
				return this.mAccount;
			}
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x000143D4 File Offset: 0x000125D4
		private ParadoxAccountSequence()
		{
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x000143E7 File Offset: 0x000125E7
		public ParadoxAccountSequence(ParadoxAccount iAccount, ParadoxAccountSequence.ExecutionDoneDelegate iCallback)
		{
			this.mAccount = iAccount;
			this.mCallback = iCallback;
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00014408 File Offset: 0x00012608
		public void Execute()
		{
			if (this.mCurrentPhase == ParadoxAccountSequence.SequencePhase.Enter)
			{
				this.OnExecute();
			}
		}

		// Token: 0x06000426 RID: 1062
		protected abstract void OnExecute();

		// Token: 0x06000427 RID: 1063 RVA: 0x00014418 File Offset: 0x00012618
		protected void ExitSuccess()
		{
			this.ChangePhase(ParadoxAccountSequence.SequencePhase.Complete);
			if (this.mCallback != null)
			{
				this.mCallback(true, ParadoxAccount.ErrorCode.None);
			}
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00014437 File Offset: 0x00012637
		protected void ExitFailure(ParadoxAccount.ErrorCode iErrorCode)
		{
			this.ChangePhase(ParadoxAccountSequence.SequencePhase.Complete);
			if (this.mCallback != null)
			{
				this.mCallback(false, iErrorCode);
			}
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x00014456 File Offset: 0x00012656
		protected bool CheckPhase(params ParadoxAccountSequence.SequencePhase[] iExpected)
		{
			return true;
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00014459 File Offset: 0x00012659
		protected void ChangePhase(ParadoxAccountSequence.SequencePhase iPhase)
		{
			Logger.LogDebug(Logger.Source.ParadoxAccountSequence, string.Format("Changed phase from [{0}] to [{1}]", this.mCurrentPhase.ToString(), iPhase.ToString()));
			this.mCurrentPhase = iPhase;
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00014490 File Offset: 0x00012690
		private void DisplayPhaseError(ParadoxAccountSequence.SequencePhase iCurrent, params ParadoxAccountSequence.SequencePhase[] iExpected)
		{
			string[] array = new string[iExpected.Length];
			for (int i = 0; i < iExpected.Length; i++)
			{
				array[i] = iExpected[i].ToString();
			}
			string arg = string.Join(", ", array);
			Logger.LogError(Logger.Source.ParadoxAccountSequence, string.Format(this.EXCEPTION_WRONG_PHASE, arg, iCurrent.ToString()));
		}

		// Token: 0x04000285 RID: 645
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountSequence;

		// Token: 0x04000286 RID: 646
		private string EXCEPTION_WRONG_PHASE = "Wrong startup phase. Expected : [{0}]. Current : [{1}].";

		// Token: 0x04000287 RID: 647
		private readonly ParadoxAccount mAccount;

		// Token: 0x04000288 RID: 648
		private readonly ParadoxAccountSequence.ExecutionDoneDelegate mCallback;

		// Token: 0x04000289 RID: 649
		private ParadoxAccountSequence.SequencePhase mCurrentPhase;

		// Token: 0x0200008E RID: 142
		// (Invoke) Token: 0x0600042D RID: 1069
		public delegate void ExecutionDoneDelegate(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode);

		// Token: 0x0200008F RID: 143
		protected enum SequencePhase
		{
			// Token: 0x0400028B RID: 651
			Enter,
			// Token: 0x0400028C RID: 652
			AccountCreate,
			// Token: 0x0400028D RID: 653
			AccountLogin,
			// Token: 0x0400028E RID: 654
			AccountLoginWithAuthToken,
			// Token: 0x0400028F RID: 655
			AccountLoginWithSteamTicket,
			// Token: 0x04000290 RID: 656
			AccountCreateShadow,
			// Token: 0x04000291 RID: 657
			AccountLoginShadow,
			// Token: 0x04000292 RID: 658
			AccountAddCredentials,
			// Token: 0x04000293 RID: 659
			AccountUpdateDetails,
			// Token: 0x04000294 RID: 660
			AccountGetDetails,
			// Token: 0x04000295 RID: 661
			AccountMergeShadow,
			// Token: 0x04000296 RID: 662
			AccountGetMergeStatus,
			// Token: 0x04000297 RID: 663
			AuthTokenInvalidate,
			// Token: 0x04000298 RID: 664
			AccountConnectAccountSteam,
			// Token: 0x04000299 RID: 665
			AccountDisconnectAccountSteam,
			// Token: 0x0400029A RID: 666
			AccountConnections,
			// Token: 0x0400029B RID: 667
			GameSparksRegister,
			// Token: 0x0400029C RID: 668
			GameSparksAuthenticate,
			// Token: 0x0400029D RID: 669
			Complete
		}

		// Token: 0x02000090 RID: 144
		private enum ErrorCode
		{
			// Token: 0x0400029F RID: 671
			Default
		}
	}
}
