using System;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.WebTools.GameSparks;

namespace Magicka.WebTools
{
	// Token: 0x02000432 RID: 1074
	public class GameSparksAccount : Singleton<GameSparksAccount>
	{
		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x0600214F RID: 8527 RVA: 0x000EDA7B File Offset: 0x000EBC7B
		// (set) Token: 0x06002150 RID: 8528 RVA: 0x000EDA83 File Offset: 0x000EBC83
		public string Variant
		{
			get
			{
				return this.mVariant;
			}
			set
			{
				this.mVariant = value;
			}
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x000EDA8C File Offset: 0x000EBC8C
		public void LogOut()
		{
			this.mCurrentAuthToken = string.Empty;
			this.mCurrentGuid = string.Empty;
			Singleton<GameSparksServices>.Instance.LogOut();
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x000EDAB0 File Offset: 0x000EBCB0
		public void Authenticate(string iGuid, string iAuthToken, GameSparksAccount.OperationCompleteDelegate iCallback)
		{
			if (this.AreCredentialsValid(iGuid, iAuthToken))
			{
				lock (this.mGameSparksAvailabilityLock)
				{
					if (GameSparksServices.Available)
					{
						Logger.LogDebug(Logger.Source.GameSparksAccount, string.Format("Authentificating a GameSparks account with guid {0} and auth token {1}.", iGuid, iAuthToken));
						this.SendAuthenticationRequest(iGuid, iAuthToken, iCallback);
					}
					else
					{
						Logger.LogDebug(Logger.Source.GameSparksAccount, string.Format("Pending authentication for a GameSparks account with guid {0} and auth token {1}.", iGuid, iAuthToken));
					}
					return;
				}
			}
			if (iCallback != null)
			{
				iCallback(GameSparksAccount.Result.InvalidCredentials);
			}
		}

		// Token: 0x06002153 RID: 8531 RVA: 0x000EDB30 File Offset: 0x000EBD30
		public void Register(string iGuid, string iAuthToken, GameSparksAccount.OperationCompleteDelegate iCallback)
		{
			if (this.AreCredentialsValid(iGuid, iAuthToken))
			{
				lock (this.mGameSparksAvailabilityLock)
				{
					if (GameSparksServices.Available)
					{
						Logger.LogDebug(Logger.Source.GameSparksAccount, string.Format("Registering a GameSparks account with guid {0} and auth token {1}.", iGuid, iAuthToken));
						this.SendRegistrationRequest(iGuid, iAuthToken, iCallback);
					}
					else
					{
						Logger.LogDebug(Logger.Source.GameSparksAccount, string.Format("Pending registration for a GameSparks account with guid {0} and auth token {1}.", iGuid, iAuthToken));
					}
					return;
				}
			}
			if (iCallback != null)
			{
				iCallback(GameSparksAccount.Result.InvalidCredentials);
			}
		}

		// Token: 0x06002154 RID: 8532 RVA: 0x000EDBB0 File Offset: 0x000EBDB0
		private void SendAuthenticationRequest(string iGuid, string iAuthToken)
		{
			this.mCurrentGuid = iGuid;
			this.mCurrentAuthToken = iAuthToken;
			Singleton<GameSparksServices>.Instance.SendAuthenticationRequest(iGuid, iAuthToken, new GameSparksServices.AuthenticationDelegate(this.AuthenticationCallback));
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x000EDBD8 File Offset: 0x000EBDD8
		private void SendAuthenticationRequest(string iGuid, string iAuthToken, GameSparksAccount.OperationCompleteDelegate iCallback)
		{
			this.mCurrentGuid = iGuid;
			this.mCurrentAuthToken = iAuthToken;
			this.mOperationCallback = iCallback;
			Singleton<GameSparksServices>.Instance.SendAuthenticationRequest(iGuid, iAuthToken, new GameSparksServices.AuthenticationDelegate(this.AuthenticationCallback));
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x000EDC07 File Offset: 0x000EBE07
		private void SendRegistrationRequest(string iGuid, string iAuthToken)
		{
			this.mCurrentGuid = iGuid;
			this.mCurrentAuthToken = iAuthToken;
			Singleton<GameSparksServices>.Instance.SendRegistrationRequest(iGuid, iAuthToken, new GameSparksServices.RegistrationDelegate(this.RegistrationCallback));
		}

		// Token: 0x06002157 RID: 8535 RVA: 0x000EDC2F File Offset: 0x000EBE2F
		private void SendRegistrationRequest(string iGuid, string iAuthToken, GameSparksAccount.OperationCompleteDelegate iCallback)
		{
			this.mCurrentGuid = iGuid;
			this.mCurrentAuthToken = iAuthToken;
			this.mOperationCallback = iCallback;
			Singleton<GameSparksServices>.Instance.SendRegistrationRequest(iGuid, iAuthToken, new GameSparksServices.RegistrationDelegate(this.RegistrationCallback));
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x000EDC60 File Offset: 0x000EBE60
		private void SetPendingAuthentication(string iGuid, string iAuthToken, GameSparksAccount.OperationCompleteDelegate iCallback)
		{
			Logger.LogDebug(Logger.Source.GameSparksAccount, "Authentication request will retry once GameSparks become available.");
			if (this.mPendingOperation != GameSparksAccount.PendingOperationType.None)
			{
				GameSparksServices.AvailabilityChanged = (GameSparksServices.AvailabilityChangedDelegate)Delegate.Combine(GameSparksServices.AvailabilityChanged, new GameSparksServices.AvailabilityChangedDelegate(this.PendingOperationCallback));
			}
			this.mCurrentGuid = iGuid;
			this.mCurrentAuthToken = iAuthToken;
			this.mPendingOperation = GameSparksAccount.PendingOperationType.Authentication;
			this.mOperationCallback = iCallback;
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x000EDCBC File Offset: 0x000EBEBC
		private void SetPendingRegistration(string iGuid, string iAuthToken, GameSparksAccount.OperationCompleteDelegate iCallback)
		{
			Logger.LogDebug(Logger.Source.GameSparksAccount, "Registration request will retry once GameSparks become available.");
			if (this.mPendingOperation != GameSparksAccount.PendingOperationType.None)
			{
				GameSparksServices.AvailabilityChanged = (GameSparksServices.AvailabilityChangedDelegate)Delegate.Combine(GameSparksServices.AvailabilityChanged, new GameSparksServices.AvailabilityChangedDelegate(this.PendingOperationCallback));
			}
			this.mCurrentGuid = iGuid;
			this.mCurrentAuthToken = iAuthToken;
			this.mPendingOperation = GameSparksAccount.PendingOperationType.Registration;
			this.mOperationCallback = iCallback;
		}

		// Token: 0x0600215A RID: 8538 RVA: 0x000EDD18 File Offset: 0x000EBF18
		private void AuthenticationCallback(GameSparksServices.AuthenticationResult iResult)
		{
			switch (iResult)
			{
			case GameSparksServices.AuthenticationResult.Success:
				this.ClearCredentials();
				if (this.mOperationCallback != null)
				{
					this.mOperationCallback(GameSparksAccount.Result.Success);
				}
				Singleton<GameSparksProperties>.Instance.RetrievePropertySetsFromGameSparks();
				return;
			case GameSparksServices.AuthenticationResult.GSUnrecognised:
				if (this.AreCredentialsValid(this.mCurrentGuid, this.mCurrentAuthToken))
				{
					this.SendRegistrationRequest(this.mCurrentGuid, this.mCurrentAuthToken);
					return;
				}
				this.ClearCredentials();
				if (this.mOperationCallback != null)
				{
					this.mOperationCallback(GameSparksAccount.Result.InvalidCredentials);
					return;
				}
				return;
			}
			this.ClearCredentials();
			if (this.mOperationCallback != null)
			{
				this.mOperationCallback(GameSparksAccount.Result.AuthenticationFailure);
			}
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x000EDDBC File Offset: 0x000EBFBC
		private void RegistrationCallback(GameSparksServices.RegistrationResult iResult)
		{
			switch (iResult)
			{
			case GameSparksServices.RegistrationResult.Success:
				this.ClearCredentials();
				if (this.mOperationCallback != null)
				{
					this.mOperationCallback(GameSparksAccount.Result.Success);
					return;
				}
				return;
			case GameSparksServices.RegistrationResult.GSTaken:
				if (this.AreCredentialsValid(this.mCurrentGuid, this.mCurrentAuthToken))
				{
					this.SendAuthenticationRequest(this.mCurrentGuid, this.mCurrentAuthToken);
					return;
				}
				this.ClearCredentials();
				if (this.mOperationCallback != null)
				{
					this.mOperationCallback(GameSparksAccount.Result.InvalidCredentials);
					return;
				}
				return;
			}
			this.ClearCredentials();
			if (this.mOperationCallback != null)
			{
				this.mOperationCallback(GameSparksAccount.Result.RegistrationFailure);
			}
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x000EDE58 File Offset: 0x000EC058
		private void PendingOperationCallback(bool iAvailable)
		{
			lock (this.mGameSparksAvailabilityLock)
			{
				if (iAvailable)
				{
					GameSparksServices.AvailabilityChanged = (GameSparksServices.AvailabilityChangedDelegate)Delegate.Remove(GameSparksServices.AvailabilityChanged, new GameSparksServices.AvailabilityChangedDelegate(this.PendingOperationCallback));
					if (this.mPendingOperation == GameSparksAccount.PendingOperationType.Authentication)
					{
						this.SendAuthenticationRequest(this.mCurrentGuid, this.mCurrentAuthToken);
					}
					else if (this.mPendingOperation == GameSparksAccount.PendingOperationType.Registration)
					{
						this.SendRegistrationRequest(this.mCurrentGuid, this.mCurrentAuthToken);
					}
					this.mPendingOperation = GameSparksAccount.PendingOperationType.None;
				}
			}
		}

		// Token: 0x0600215D RID: 8541 RVA: 0x000EDEF0 File Offset: 0x000EC0F0
		private void ClearCredentials()
		{
			this.mCurrentGuid = string.Empty;
			this.mCurrentAuthToken = string.Empty;
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x000EDF08 File Offset: 0x000EC108
		private bool AreCredentialsValid(string iGuid, string iAuthToken)
		{
			return !string.IsNullOrEmpty(iGuid) && !string.IsNullOrEmpty(iAuthToken);
		}

		// Token: 0x0400241F RID: 9247
		private const Logger.Source LOGGER_SOURCE = Logger.Source.GameSparksAccount;

		// Token: 0x04002420 RID: 9248
		private GameSparksAccount.PendingOperationType mPendingOperation;

		// Token: 0x04002421 RID: 9249
		private string mCurrentAuthToken = string.Empty;

		// Token: 0x04002422 RID: 9250
		private string mCurrentGuid = string.Empty;

		// Token: 0x04002423 RID: 9251
		private string mVariant = string.Empty;

		// Token: 0x04002424 RID: 9252
		private GameSparksAccount.OperationCompleteDelegate mOperationCallback;

		// Token: 0x04002425 RID: 9253
		private object mGameSparksAvailabilityLock = new object();

		// Token: 0x02000433 RID: 1075
		public enum Result
		{
			// Token: 0x04002427 RID: 9255
			Success,
			// Token: 0x04002428 RID: 9256
			AlreadyBusy,
			// Token: 0x04002429 RID: 9257
			InvalidCredentials,
			// Token: 0x0400242A RID: 9258
			RegistrationFailure,
			// Token: 0x0400242B RID: 9259
			AuthenticationFailure
		}

		// Token: 0x02000434 RID: 1076
		// (Invoke) Token: 0x06002161 RID: 8545
		public delegate void OperationCompleteDelegate(GameSparksAccount.Result iResult);

		// Token: 0x02000435 RID: 1077
		private enum PendingOperationType
		{
			// Token: 0x0400242D RID: 9261
			None,
			// Token: 0x0400242E RID: 9262
			Registration,
			// Token: 0x0400242F RID: 9263
			Authentication
		}
	}
}
