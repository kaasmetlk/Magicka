// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.GameSparks.GameSparksServices
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using Magicka.CoreFramework;
using Magicka.Misc;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Magicka.WebTools.GameSparks;

public class GameSparksServices : Singleton<GameSparksServices>
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.GameSparksServices;
  private const string EXCEPTION_MUST_RETURN = "The Create method in class {0} must return an object of type {1}.";
  private const string EXCEPTION_NOT_STATIC = "The Create method in class {0} must be static.";
  private const string EXCEPTION_MUST_IMPLEMENT_CREATE = "The class {0} must have a \"Create\" static method implemented.";
  private const string GAMESPARKS_SESSION_TOKEN_KEY = "sessionToken";
  private const string GAMESPARKS_DETAILS = "DETAILS";
  private const string GAMESPARKS_DETAILS_ERROR_UNRECOGNISED = "UNRECOGNISED";
  private const string GAMESPARKS_DETAILS_ERROR_LOCKED = "LOCKED";
  private const string GAMESPARKS_USERNAME = "USERNAME";
  private const string GAMESPARKS_USERNAME_ERROR_TAKEN = "TAKEN";
  private const string POPS_ERROR_TOKEN_AUTH = "token-auth-error";
  private const string POPS_ERROR_GS_MESSAGE_KEY = "gs-message";
  private const string POPS_ERROR_CODE_KEY = "pops-errorCode";
  private const string POPS_ERROR_CODE_NOT_FOUND = "not-found";
  private const string POPS_ERROR_CODE_NOT_AUTHORIZED = "not-authorized";
  private const string POPS_ERROR_CODE_INTERNAL_ERROR = "internal-error";
  private const string UNKNOWN_ERROR = "unknown";
  private const string VARIANT_REQUEST = "VariantRequest";
  public static GameSparksServices.AvailabilityChangedDelegate AvailabilityChanged;
  private Queue<Action> mPendingActions = new Queue<Action>();
  private object mActionQueueLock = new object();
  private bool mIsAuthenticating;
  private GameSparksServices.AuthenticationDelegate mAuthenticationCallback;
  private bool mIsRegistering;
  private GameSparksServices.RegistrationDelegate mRegistrationCallback;
  private string mLastErrorString = string.Empty;

  public static bool Available => GS.Available;

  public static bool Authenticated => GS.Authenticated;

  public GameSparksServices()
  {
    GS.GameSparksAvailable = new Action<bool>(this.GameSparksAvailableCallback);
  }

  public void Initialize<T>() where T : GSPlatformBase
  {
    MethodInfo method = typeof (T).GetMethod("Create");
    if (method == null)
      throw new Exception($"The class {typeof (T).Name} must have a \"Create\" static method implemented.");
    if (!method.IsStatic)
      throw new Exception($"The Create method in class {typeof (T).Name} must be static.");
    if (method.ReturnType != typeof (T))
      throw new Exception($"The Create method in class {typeof (T).Name} must return an object of type {typeof (T).Name}.");
    try
    {
      T gSPlatform = (T) method.Invoke((object) null, (object[]) null);
      gSPlatform.Services = this;
      GS.Initialise((IGSPlatform) gSPlatform);
    }
    catch (TargetInvocationException ex)
    {
      Exception exception = (Exception) ex;
      while (exception != null)
        exception = exception.InnerException;
    }
  }

  public void Update()
  {
    lock (this.mActionQueueLock)
    {
      while (this.mPendingActions.Count > 0)
        this.mPendingActions.Dequeue()();
    }
  }

  private void GameSparksAvailableCallback(bool iAvailable)
  {
    if (iAvailable)
    {
      Logger.LogDebug(Logger.Source.GameSparksServices, "GameSparks became available.");
      Singleton<GameSparksProperties>.Instance.RetrievePropertySetsFromGameSparks();
    }
    else
      Logger.LogDebug(Logger.Source.GameSparksServices, "GameSparks became unavailable.");
    if (GameSparksServices.AvailabilityChanged == null)
      return;
    GameSparksServices.AvailabilityChanged(iAvailable);
  }

  public void Dispose() => GS.ShutDown();

  public void ExecuteInMainThread(Action iAction)
  {
    lock (this.mActionQueueLock)
      this.mPendingActions.Enqueue(iAction);
  }

  public void SendAuthenticationRequest(
    string iGuid,
    string iAuthToken,
    GameSparksServices.AuthenticationDelegate iCallback)
  {
    if (this.mIsAuthenticating)
      Logger.LogError(Logger.Source.GameSparksServices, "Authentication already in progress ! Request ignored.");
    else if (string.IsNullOrEmpty(iGuid))
      Logger.LogError(Logger.Source.GameSparksServices, "Cannot authenticate, guid is missing.");
    else if (string.IsNullOrEmpty(iAuthToken))
    {
      Logger.LogError(Logger.Source.GameSparksServices, "Cannot authenticate, auth token is missing.");
    }
    else
    {
      this.mLastErrorString = string.Empty;
      GSRequestData data = new GSRequestData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "sessionToken",
          (object) iAuthToken
        }
      });
      new AuthenticationRequest().SetUserName(iGuid).SetPassword(string.Empty).SetScriptData(data).Send(new Action<AuthenticationResponse>(this.AuthenticateGameSparksAccountCallback));
      this.mIsAuthenticating = true;
      this.mAuthenticationCallback = iCallback;
    }
  }

  public void SendRegistrationRequest(
    string iGuid,
    string iAuthToken,
    GameSparksServices.RegistrationDelegate iCallback)
  {
    if (this.mIsRegistering)
      Logger.LogError(Logger.Source.GameSparksServices, "Registration already in progress ! Request ignored.");
    else if (string.IsNullOrEmpty(iGuid))
      Logger.LogError(Logger.Source.GameSparksServices, "Cannot register, guid is missing.");
    else if (string.IsNullOrEmpty(iAuthToken))
    {
      Logger.LogError(Logger.Source.GameSparksServices, "Cannot register, auth token is missing.");
    }
    else
    {
      this.mLastErrorString = string.Empty;
      GSRequestData data = new GSRequestData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "sessionToken",
          (object) iAuthToken
        }
      });
      new RegistrationRequest().SetUserName(iGuid).SetPassword(string.Empty).SetDisplayName(iGuid).SetScriptData(data).Send(new Action<RegistrationResponse>(this.RegisterGameSparksAccountDelegate));
      this.mIsRegistering = true;
      this.mRegistrationCallback = iCallback;
    }
  }

  public void LogOut()
  {
    if (!GS.Authenticated)
      return;
    GS.Reset();
  }

  private void AuthenticateGameSparksAccountCallback(AuthenticationResponse iResponse)
  {
    GameSparksServices.AuthenticationResult iResult = GameSparksServices.AuthenticationResult.Unknown;
    if (iResponse.HasErrors)
    {
      Logger.LogWarning(Logger.Source.GameSparksServices, "Authentication response have errors :");
      IDictionary<string, object> baseData = iResponse.Errors.BaseData;
      if (baseData.ContainsKey("DETAILS"))
      {
        string str = baseData["DETAILS"].ToString();
        switch (str)
        {
          case "UNRECOGNISED":
            iResult = GameSparksServices.AuthenticationResult.GSUnrecognised;
            break;
          case "LOCKED":
            iResult = GameSparksServices.AuthenticationResult.GSLocked;
            break;
        }
        Logger.LogError(Logger.Source.GameSparksServices, $"GameSparks error code : [{"DETAILS"}] -> [{str}].");
        this.mLastErrorString = str;
      }
      else if (baseData.ContainsKey("token-auth-error"))
      {
        string str = (baseData["token-auth-error"] as GSData).GetString("pops-errorCode");
        switch (str)
        {
          case "not-found":
            iResult = GameSparksServices.AuthenticationResult.POPSNotFound;
            break;
          case "not-authorized":
            iResult = GameSparksServices.AuthenticationResult.POPSNotAuthorized;
            break;
        }
        Logger.LogError(Logger.Source.GameSparksServices, $"Pops error code : [{"token-auth-error"}] -> [{str}].");
        this.mLastErrorString = str;
      }
      else
      {
        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) baseData)
          Logger.LogError(Logger.Source.GameSparksServices, $"Unknown GameSparks error : [{keyValuePair.Key}] -> [{keyValuePair.Value.ToString()}].");
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
    if (this.mAuthenticationCallback == null)
      return;
    this.mAuthenticationCallback(iResult);
    this.mAuthenticationCallback = (GameSparksServices.AuthenticationDelegate) null;
  }

  public void RequestPropertySet(string iPropertySetName, Action<GetPropertySetResponse> iCallback)
  {
    new GetPropertySetRequest().SetPropertySetShortCode(iPropertySetName).Send(iCallback);
  }

  public void LogEvent(string iEventKey)
  {
    new LogEventRequest().SetEventKey(iEventKey).Send(new Action<LogEventResponse>(this.LogGameSparksEventCallback));
  }

  private void LogGameSparksEventCallback(LogEventResponse iResponse)
  {
    if (iResponse.HasErrors)
    {
      Logger.LogError(Logger.Source.GameSparksServices, "Log Event response has errors :");
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) iResponse.Errors.BaseData)
        Logger.LogError(Logger.Source.GameSparksServices, keyValuePair.Key);
    }
    else
    {
      Logger.LogDebug(Logger.Source.GameSparksServices, "Log Event response succeeded.");
      Singleton<GameSparksEventHandler>.Instance.HandleResponse(iResponse.ScriptData);
      this.mLastErrorString = string.Empty;
    }
  }

  private void RegisterGameSparksAccountDelegate(RegistrationResponse iResponse)
  {
    GameSparksServices.RegistrationResult iResult = GameSparksServices.RegistrationResult.Unknown;
    if (iResponse.HasErrors)
    {
      Logger.LogError(Logger.Source.GameSparksServices, "Registration response have errors :");
      IDictionary<string, object> baseData = iResponse.Errors.BaseData;
      if (baseData.ContainsKey("USERNAME"))
      {
        string str = baseData["USERNAME"].ToString();
        if (str.Equals("TAKEN"))
          iResult = GameSparksServices.RegistrationResult.GSTaken;
        Logger.LogError(Logger.Source.GameSparksServices, $"GameSparks error code : [{"USERNAME"}] -> [{str}].");
        this.mLastErrorString = str;
      }
      else if (baseData.ContainsKey("token-auth-error"))
      {
        string str = (baseData["token-auth-error"] as GSData).GetString("pops-errorCode");
        switch (str)
        {
          case "not-found":
            iResult = GameSparksServices.RegistrationResult.POPSNotFound;
            break;
          case "internal-error":
            iResult = GameSparksServices.RegistrationResult.POPSInternalError;
            break;
        }
        Logger.LogError(Logger.Source.GameSparksServices, $"Pops error code : [{"token-auth-error"}] -> [{str}].");
        this.mLastErrorString = str;
      }
      else
      {
        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) baseData)
          Logger.LogError(Logger.Source.GameSparksServices, $"Unknown GameSparks error : [{keyValuePair.Key}] -> [{keyValuePair.Value.ToString()}].");
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
    if (this.mRegistrationCallback == null)
      return;
    this.mRegistrationCallback(iResult);
    this.mRegistrationCallback = (GameSparksServices.RegistrationDelegate) null;
  }

  public enum AuthenticationResult
  {
    Success,
    Unknown,
    GSUnrecognised,
    GSLocked,
    POPSNotFound,
    POPSNotAuthorized,
  }

  public enum RegistrationResult
  {
    Success,
    Unknown,
    GSTaken,
    POPSNotFound,
    POPSInternalError,
  }

  public delegate void AvailabilityChangedDelegate(bool iAvailable);

  public delegate void AuthenticationDelegate(GameSparksServices.AuthenticationResult iResult);

  public delegate void RegistrationDelegate(GameSparksServices.RegistrationResult iResult);

  public delegate void GetPropertySetDelegate(GetPropertySetResponse iResult);
}
