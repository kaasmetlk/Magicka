using System;
using System.Collections.Generic;
using System.Reflection;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using Magicka.CoreFramework;
using Magicka.Misc;

namespace Magicka.WebTools.GameSparks
{
	// Token: 0x020003D6 RID: 982
	public class GameSparksServices : Singleton<GameSparksServices>
	{
		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x06001E22 RID: 7714 RVA: 0x000D407D File Offset: 0x000D227D
		public static bool Available
		{
			get
			{
				return GS.Available;
			}
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x06001E23 RID: 7715 RVA: 0x000D4084 File Offset: 0x000D2284
		public static bool Authenticated
		{
			get
			{
				return GS.Authenticated;
			}
		}

		// Token: 0x06001E24 RID: 7716 RVA: 0x000D408B File Offset: 0x000D228B
		public GameSparksServices()
		{
			GS.GameSparksAvailable = new Action<bool>(this.GameSparksAvailableCallback);
		}

		// Token: 0x06001E25 RID: 7717 RVA: 0x000D40C8 File Offset: 0x000D22C8
		public void Initialize<T>() where T : GSPlatformBase
		{
			MethodInfo method = typeof(T).GetMethod("Create");
			if (method == null)
			{
				throw new Exception(string.Format("The class {0} must have a \"Create\" static method implemented.", typeof(T).Name));
			}
			if (method.IsStatic)
			{
				if (method.ReturnType == typeof(T))
				{
					try
					{
						T t = (T)((object)method.Invoke(null, null));
						t.Services = this;
						GS.Initialise(t);
						return;
					}
					catch (TargetInvocationException ex)
					{
						for (Exception ex2 = ex; ex2 != null; ex2 = ex2.InnerException)
						{
						}
						return;
					}
				}
				throw new Exception(string.Format("The Create method in class {0} must return an object of type {1}.", typeof(T).Name, typeof(T).Name));
			}
			throw new Exception(string.Format("The Create method in class {0} must be static.", typeof(T).Name));
		}

		// Token: 0x06001E26 RID: 7718 RVA: 0x000D41C0 File Offset: 0x000D23C0
		public void Update()
		{
			lock (this.mActionQueueLock)
			{
				while (this.mPendingActions.Count > 0)
				{
					Action action = this.mPendingActions.Dequeue();
					action.Invoke();
				}
			}
		}

		// Token: 0x06001E27 RID: 7719 RVA: 0x000D4218 File Offset: 0x000D2418
		private void GameSparksAvailableCallback(bool iAvailable)
		{
			if (iAvailable)
			{
				Logger.LogDebug(Logger.Source.GameSparksServices, "GameSparks became available.");
				Singleton<GameSparksProperties>.Instance.RetrievePropertySetsFromGameSparks();
			}
			else
			{
				Logger.LogDebug(Logger.Source.GameSparksServices, "GameSparks became unavailable.");
			}
			if (GameSparksServices.AvailabilityChanged != null)
			{
				GameSparksServices.AvailabilityChanged(iAvailable);
			}
		}

		// Token: 0x06001E28 RID: 7720 RVA: 0x000D4251 File Offset: 0x000D2451
		public void Dispose()
		{
			GS.ShutDown();
		}

		// Token: 0x06001E29 RID: 7721 RVA: 0x000D4258 File Offset: 0x000D2458
		public void ExecuteInMainThread(Action iAction)
		{
			lock (this.mActionQueueLock)
			{
				this.mPendingActions.Enqueue(iAction);
			}
		}

		// Token: 0x06001E2A RID: 7722 RVA: 0x000D4298 File Offset: 0x000D2498
		public void SendAuthenticationRequest(string iGuid, string iAuthToken, GameSparksServices.AuthenticationDelegate iCallback)
		{
			if (this.mIsAuthenticating)
			{
				Logger.LogError(Logger.Source.GameSparksServices, "Authentication already in progress ! Request ignored.");
				return;
			}
			if (string.IsNullOrEmpty(iGuid))
			{
				Logger.LogError(Logger.Source.GameSparksServices, "Cannot authenticate, guid is missing.");
				return;
			}
			if (string.IsNullOrEmpty(iAuthToken))
			{
				Logger.LogError(Logger.Source.GameSparksServices, "Cannot authenticate, auth token is missing.");
				return;
			}
			this.mLastErrorString = string.Empty;
			GSRequestData scriptData = new GSRequestData(new Dictionary<string, object>
			{
				{
					"sessionToken",
					iAuthToken
				}
			});
			new AuthenticationRequest().SetUserName(iGuid).SetPassword(string.Empty).SetScriptData(scriptData).Send(new Action<AuthenticationResponse>(this.AuthenticateGameSparksAccountCallback));
			this.mIsAuthenticating = true;
			this.mAuthenticationCallback = iCallback;
		}

		// Token: 0x06001E2B RID: 7723 RVA: 0x000D4340 File Offset: 0x000D2540
		public void SendRegistrationRequest(string iGuid, string iAuthToken, GameSparksServices.RegistrationDelegate iCallback)
		{
			if (this.mIsRegistering)
			{
				Logger.LogError(Logger.Source.GameSparksServices, "Registration already in progress ! Request ignored.");
				return;
			}
			if (string.IsNullOrEmpty(iGuid))
			{
				Logger.LogError(Logger.Source.GameSparksServices, "Cannot register, guid is missing.");
				return;
			}
			if (string.IsNullOrEmpty(iAuthToken))
			{
				Logger.LogError(Logger.Source.GameSparksServices, "Cannot register, auth token is missing.");
				return;
			}
			this.mLastErrorString = string.Empty;
			GSRequestData scriptData = new GSRequestData(new Dictionary<string, object>
			{
				{
					"sessionToken",
					iAuthToken
				}
			});
			new RegistrationRequest().SetUserName(iGuid).SetPassword(string.Empty).SetDisplayName(iGuid).SetScriptData(scriptData).Send(new Action<RegistrationResponse>(this.RegisterGameSparksAccountDelegate));
			this.mIsRegistering = true;
			this.mRegistrationCallback = iCallback;
		}

		// Token: 0x06001E2C RID: 7724 RVA: 0x000D43ED File Offset: 0x000D25ED
		public void LogOut()
		{
			if (GS.Authenticated)
			{
				GS.Reset();
			}
		}

		// Token: 0x06001E2D RID: 7725 RVA: 0x000D43FC File Offset: 0x000D25FC
		private void AuthenticateGameSparksAccountCallback(AuthenticationResponse iResponse)
		{
			GameSparksServices.AuthenticationResult iResult = GameSparksServices.AuthenticationResult.Unknown;
			if (iResponse.HasErrors)
			{
				Logger.LogWarning(Logger.Source.GameSparksServices, "Authentication response have errors :");
				IDictionary<string, object> baseData = iResponse.Errors.BaseData;
				if (baseData.ContainsKey("DETAILS"))
				{
					string text = baseData["DETAILS"].ToString();
					if (text.Equals("UNRECOGNISED"))
					{
						iResult = GameSparksServices.AuthenticationResult.GSUnrecognised;
					}
					else if (text.Equals("LOCKED"))
					{
						iResult = GameSparksServices.AuthenticationResult.GSLocked;
					}
					Logger.LogError(Logger.Source.GameSparksServices, string.Format("GameSparks error code : [{0}] -> [{1}].", "DETAILS", text));
					this.mLastErrorString = text;
				}
				else if (baseData.ContainsKey("token-auth-error"))
				{
					GSData gsdata = baseData["token-auth-error"] as GSData;
					string @string = gsdata.GetString("pops-errorCode");
					if (@string.Equals("not-found"))
					{
						iResult = GameSparksServices.AuthenticationResult.POPSNotFound;
					}
					else if (@string.Equals("not-authorized"))
					{
						iResult = GameSparksServices.AuthenticationResult.POPSNotAuthorized;
					}
					Logger.LogError(Logger.Source.GameSparksServices, string.Format("Pops error code : [{0}] -> [{1}].", "token-auth-error", @string));
					this.mLastErrorString = @string;
				}
				else
				{
					foreach (KeyValuePair<string, object> keyValuePair in baseData)
					{
						Logger.LogError(Logger.Source.GameSparksServices, string.Format("Unknown GameSparks error : [{0}] -> [{1}].", keyValuePair.Key, keyValuePair.Value.ToString()));
					}
					this.mLastErrorString = "unknown";
				}
			}
			else
			{
				Logger.LogDebug(Logger.Source.GameSparksServices, "Authentication response succeeded.");
				iResult = GameSparksServices.AuthenticationResult.Success;
				this.mLastErrorString = string.Empty;
				Singleton<GameSparksProperties>.Instance.RetrievePropertySetsFromGameSparks();
				Singleton<GameSparksServices>.Instance.LogEvent("VariantRequest");
			}
			this.mIsAuthenticating = false;
			if (this.mAuthenticationCallback != null)
			{
				this.mAuthenticationCallback(iResult);
				this.mAuthenticationCallback = null;
			}
		}

		// Token: 0x06001E2E RID: 7726 RVA: 0x000D45BC File Offset: 0x000D27BC
		public void RequestPropertySet(string iPropertySetName, Action<GetPropertySetResponse> iCallback)
		{
			new GetPropertySetRequest().SetPropertySetShortCode(iPropertySetName).Send(iCallback);
		}

		// Token: 0x06001E2F RID: 7727 RVA: 0x000D45CF File Offset: 0x000D27CF
		public void LogEvent(string iEventKey)
		{
			new LogEventRequest().SetEventKey(iEventKey).Send(new Action<LogEventResponse>(this.LogGameSparksEventCallback));
		}

		// Token: 0x06001E30 RID: 7728 RVA: 0x000D45F0 File Offset: 0x000D27F0
		private void LogGameSparksEventCallback(LogEventResponse iResponse)
		{
			if (iResponse.HasErrors)
			{
				Logger.LogError(Logger.Source.GameSparksServices, "Log Event response has errors :");
				using (IEnumerator<KeyValuePair<string, object>> enumerator = iResponse.Errors.BaseData.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, object> keyValuePair = enumerator.Current;
						Logger.LogError(Logger.Source.GameSparksServices, keyValuePair.Key);
					}
					return;
				}
			}
			Logger.LogDebug(Logger.Source.GameSparksServices, "Log Event response succeeded.");
			Singleton<GameSparksEventHandler>.Instance.HandleResponse(iResponse.ScriptData);
			this.mLastErrorString = string.Empty;
		}

		// Token: 0x06001E31 RID: 7729 RVA: 0x000D4684 File Offset: 0x000D2884
		private void RegisterGameSparksAccountDelegate(RegistrationResponse iResponse)
		{
			GameSparksServices.RegistrationResult iResult = GameSparksServices.RegistrationResult.Unknown;
			if (iResponse.HasErrors)
			{
				Logger.LogError(Logger.Source.GameSparksServices, "Registration response have errors :");
				IDictionary<string, object> baseData = iResponse.Errors.BaseData;
				if (baseData.ContainsKey("USERNAME"))
				{
					string text = baseData["USERNAME"].ToString();
					if (text.Equals("TAKEN"))
					{
						iResult = GameSparksServices.RegistrationResult.GSTaken;
					}
					Logger.LogError(Logger.Source.GameSparksServices, string.Format("GameSparks error code : [{0}] -> [{1}].", "USERNAME", text));
					this.mLastErrorString = text;
				}
				else if (baseData.ContainsKey("token-auth-error"))
				{
					GSData gsdata = baseData["token-auth-error"] as GSData;
					string @string = gsdata.GetString("pops-errorCode");
					if (@string.Equals("not-found"))
					{
						iResult = GameSparksServices.RegistrationResult.POPSNotFound;
					}
					else if (@string.Equals("internal-error"))
					{
						iResult = GameSparksServices.RegistrationResult.POPSInternalError;
					}
					Logger.LogError(Logger.Source.GameSparksServices, string.Format("Pops error code : [{0}] -> [{1}].", "token-auth-error", @string));
					this.mLastErrorString = @string;
				}
				else
				{
					foreach (KeyValuePair<string, object> keyValuePair in baseData)
					{
						Logger.LogError(Logger.Source.GameSparksServices, string.Format("Unknown GameSparks error : [{0}] -> [{1}].", keyValuePair.Key, keyValuePair.Value.ToString()));
					}
					this.mLastErrorString = "unknown";
				}
			}
			else
			{
				Logger.LogDebug(Logger.Source.GameSparksServices, "Registration response succeeded.");
				iResult = GameSparksServices.RegistrationResult.Success;
				this.mLastErrorString = string.Empty;
			}
			this.mIsRegistering = false;
			if (this.mRegistrationCallback != null)
			{
				this.mRegistrationCallback(iResult);
				this.mRegistrationCallback = null;
			}
		}

		// Token: 0x04002095 RID: 8341
		private const Logger.Source LOGGER_SOURCE = Logger.Source.GameSparksServices;

		// Token: 0x04002096 RID: 8342
		private const string EXCEPTION_MUST_RETURN = "The Create method in class {0} must return an object of type {1}.";

		// Token: 0x04002097 RID: 8343
		private const string EXCEPTION_NOT_STATIC = "The Create method in class {0} must be static.";

		// Token: 0x04002098 RID: 8344
		private const string EXCEPTION_MUST_IMPLEMENT_CREATE = "The class {0} must have a \"Create\" static method implemented.";

		// Token: 0x04002099 RID: 8345
		private const string GAMESPARKS_SESSION_TOKEN_KEY = "sessionToken";

		// Token: 0x0400209A RID: 8346
		private const string GAMESPARKS_DETAILS = "DETAILS";

		// Token: 0x0400209B RID: 8347
		private const string GAMESPARKS_DETAILS_ERROR_UNRECOGNISED = "UNRECOGNISED";

		// Token: 0x0400209C RID: 8348
		private const string GAMESPARKS_DETAILS_ERROR_LOCKED = "LOCKED";

		// Token: 0x0400209D RID: 8349
		private const string GAMESPARKS_USERNAME = "USERNAME";

		// Token: 0x0400209E RID: 8350
		private const string GAMESPARKS_USERNAME_ERROR_TAKEN = "TAKEN";

		// Token: 0x0400209F RID: 8351
		private const string POPS_ERROR_TOKEN_AUTH = "token-auth-error";

		// Token: 0x040020A0 RID: 8352
		private const string POPS_ERROR_GS_MESSAGE_KEY = "gs-message";

		// Token: 0x040020A1 RID: 8353
		private const string POPS_ERROR_CODE_KEY = "pops-errorCode";

		// Token: 0x040020A2 RID: 8354
		private const string POPS_ERROR_CODE_NOT_FOUND = "not-found";

		// Token: 0x040020A3 RID: 8355
		private const string POPS_ERROR_CODE_NOT_AUTHORIZED = "not-authorized";

		// Token: 0x040020A4 RID: 8356
		private const string POPS_ERROR_CODE_INTERNAL_ERROR = "internal-error";

		// Token: 0x040020A5 RID: 8357
		private const string UNKNOWN_ERROR = "unknown";

		// Token: 0x040020A6 RID: 8358
		private const string VARIANT_REQUEST = "VariantRequest";

		// Token: 0x040020A7 RID: 8359
		public static GameSparksServices.AvailabilityChangedDelegate AvailabilityChanged;

		// Token: 0x040020A8 RID: 8360
		private Queue<Action> mPendingActions = new Queue<Action>();

		// Token: 0x040020A9 RID: 8361
		private object mActionQueueLock = new object();

		// Token: 0x040020AA RID: 8362
		private bool mIsAuthenticating;

		// Token: 0x040020AB RID: 8363
		private GameSparksServices.AuthenticationDelegate mAuthenticationCallback;

		// Token: 0x040020AC RID: 8364
		private bool mIsRegistering;

		// Token: 0x040020AD RID: 8365
		private GameSparksServices.RegistrationDelegate mRegistrationCallback;

		// Token: 0x040020AE RID: 8366
		private string mLastErrorString = string.Empty;

		// Token: 0x020003D7 RID: 983
		public enum AuthenticationResult
		{
			// Token: 0x040020B0 RID: 8368
			Success,
			// Token: 0x040020B1 RID: 8369
			Unknown,
			// Token: 0x040020B2 RID: 8370
			GSUnrecognised,
			// Token: 0x040020B3 RID: 8371
			GSLocked,
			// Token: 0x040020B4 RID: 8372
			POPSNotFound,
			// Token: 0x040020B5 RID: 8373
			POPSNotAuthorized
		}

		// Token: 0x020003D8 RID: 984
		public enum RegistrationResult
		{
			// Token: 0x040020B7 RID: 8375
			Success,
			// Token: 0x040020B8 RID: 8376
			Unknown,
			// Token: 0x040020B9 RID: 8377
			GSTaken,
			// Token: 0x040020BA RID: 8378
			POPSNotFound,
			// Token: 0x040020BB RID: 8379
			POPSInternalError
		}

		// Token: 0x020003D9 RID: 985
		// (Invoke) Token: 0x06001E33 RID: 7731
		public delegate void AvailabilityChangedDelegate(bool iAvailable);

		// Token: 0x020003DA RID: 986
		// (Invoke) Token: 0x06001E37 RID: 7735
		public delegate void AuthenticationDelegate(GameSparksServices.AuthenticationResult iResult);

		// Token: 0x020003DB RID: 987
		// (Invoke) Token: 0x06001E3B RID: 7739
		public delegate void RegistrationDelegate(GameSparksServices.RegistrationResult iResult);

		// Token: 0x020003DC RID: 988
		// (Invoke) Token: 0x06001E3F RID: 7743
		public delegate void GetPropertySetDelegate(GetPropertySetResponse iResult);
	}
}
