// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.GameSparksAccount
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.WebTools.GameSparks;

#nullable disable
namespace Magicka.WebTools;

public class GameSparksAccount : Singleton<GameSparksAccount>
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.GameSparksAccount;
  private GameSparksAccount.PendingOperationType mPendingOperation;
  private string mCurrentAuthToken = string.Empty;
  private string mCurrentGuid = string.Empty;
  private string mVariant = string.Empty;
  private GameSparksAccount.OperationCompleteDelegate mOperationCallback;
  private object mGameSparksAvailabilityLock = new object();

  public string Variant
  {
    get => this.mVariant;
    set => this.mVariant = value;
  }

  public void LogOut()
  {
    this.mCurrentAuthToken = string.Empty;
    this.mCurrentGuid = string.Empty;
    Singleton<GameSparksServices>.Instance.LogOut();
  }

  public void Authenticate(
    string iGuid,
    string iAuthToken,
    GameSparksAccount.OperationCompleteDelegate iCallback)
  {
    if (this.AreCredentialsValid(iGuid, iAuthToken))
    {
      lock (this.mGameSparksAvailabilityLock)
      {
        if (GameSparksServices.Available)
        {
          Logger.LogDebug(Logger.Source.GameSparksAccount, $"Authentificating a GameSparks account with guid {iGuid} and auth token {iAuthToken}.");
          this.SendAuthenticationRequest(iGuid, iAuthToken, iCallback);
        }
        else
          Logger.LogDebug(Logger.Source.GameSparksAccount, $"Pending authentication for a GameSparks account with guid {iGuid} and auth token {iAuthToken}.");
      }
    }
    else
    {
      if (iCallback == null)
        return;
      iCallback(GameSparksAccount.Result.InvalidCredentials);
    }
  }

  public void Register(
    string iGuid,
    string iAuthToken,
    GameSparksAccount.OperationCompleteDelegate iCallback)
  {
    if (this.AreCredentialsValid(iGuid, iAuthToken))
    {
      lock (this.mGameSparksAvailabilityLock)
      {
        if (GameSparksServices.Available)
        {
          Logger.LogDebug(Logger.Source.GameSparksAccount, $"Registering a GameSparks account with guid {iGuid} and auth token {iAuthToken}.");
          this.SendRegistrationRequest(iGuid, iAuthToken, iCallback);
        }
        else
          Logger.LogDebug(Logger.Source.GameSparksAccount, $"Pending registration for a GameSparks account with guid {iGuid} and auth token {iAuthToken}.");
      }
    }
    else
    {
      if (iCallback == null)
        return;
      iCallback(GameSparksAccount.Result.InvalidCredentials);
    }
  }

  private void SendAuthenticationRequest(string iGuid, string iAuthToken)
  {
    this.mCurrentGuid = iGuid;
    this.mCurrentAuthToken = iAuthToken;
    Singleton<GameSparksServices>.Instance.SendAuthenticationRequest(iGuid, iAuthToken, new GameSparksServices.AuthenticationDelegate(this.AuthenticationCallback));
  }

  private void SendAuthenticationRequest(
    string iGuid,
    string iAuthToken,
    GameSparksAccount.OperationCompleteDelegate iCallback)
  {
    this.mCurrentGuid = iGuid;
    this.mCurrentAuthToken = iAuthToken;
    this.mOperationCallback = iCallback;
    Singleton<GameSparksServices>.Instance.SendAuthenticationRequest(iGuid, iAuthToken, new GameSparksServices.AuthenticationDelegate(this.AuthenticationCallback));
  }

  private void SendRegistrationRequest(string iGuid, string iAuthToken)
  {
    this.mCurrentGuid = iGuid;
    this.mCurrentAuthToken = iAuthToken;
    Singleton<GameSparksServices>.Instance.SendRegistrationRequest(iGuid, iAuthToken, new GameSparksServices.RegistrationDelegate(this.RegistrationCallback));
  }

  private void SendRegistrationRequest(
    string iGuid,
    string iAuthToken,
    GameSparksAccount.OperationCompleteDelegate iCallback)
  {
    this.mCurrentGuid = iGuid;
    this.mCurrentAuthToken = iAuthToken;
    this.mOperationCallback = iCallback;
    Singleton<GameSparksServices>.Instance.SendRegistrationRequest(iGuid, iAuthToken, new GameSparksServices.RegistrationDelegate(this.RegistrationCallback));
  }

  private void SetPendingAuthentication(
    string iGuid,
    string iAuthToken,
    GameSparksAccount.OperationCompleteDelegate iCallback)
  {
    Logger.LogDebug(Logger.Source.GameSparksAccount, "Authentication request will retry once GameSparks become available.");
    if (this.mPendingOperation != GameSparksAccount.PendingOperationType.None)
      GameSparksServices.AvailabilityChanged += new GameSparksServices.AvailabilityChangedDelegate(this.PendingOperationCallback);
    this.mCurrentGuid = iGuid;
    this.mCurrentAuthToken = iAuthToken;
    this.mPendingOperation = GameSparksAccount.PendingOperationType.Authentication;
    this.mOperationCallback = iCallback;
  }

  private void SetPendingRegistration(
    string iGuid,
    string iAuthToken,
    GameSparksAccount.OperationCompleteDelegate iCallback)
  {
    Logger.LogDebug(Logger.Source.GameSparksAccount, "Registration request will retry once GameSparks become available.");
    if (this.mPendingOperation != GameSparksAccount.PendingOperationType.None)
      GameSparksServices.AvailabilityChanged += new GameSparksServices.AvailabilityChangedDelegate(this.PendingOperationCallback);
    this.mCurrentGuid = iGuid;
    this.mCurrentAuthToken = iAuthToken;
    this.mPendingOperation = GameSparksAccount.PendingOperationType.Registration;
    this.mOperationCallback = iCallback;
  }

  private void AuthenticationCallback(GameSparksServices.AuthenticationResult iResult)
  {
    switch (iResult)
    {
      case GameSparksServices.AuthenticationResult.Success:
        this.ClearCredentials();
        if (this.mOperationCallback != null)
          this.mOperationCallback(GameSparksAccount.Result.Success);
        Singleton<GameSparksProperties>.Instance.RetrievePropertySetsFromGameSparks();
        break;
      case GameSparksServices.AuthenticationResult.GSUnrecognised:
        if (this.AreCredentialsValid(this.mCurrentGuid, this.mCurrentAuthToken))
        {
          this.SendRegistrationRequest(this.mCurrentGuid, this.mCurrentAuthToken);
          break;
        }
        this.ClearCredentials();
        if (this.mOperationCallback == null)
          break;
        this.mOperationCallback(GameSparksAccount.Result.InvalidCredentials);
        break;
      default:
        this.ClearCredentials();
        if (this.mOperationCallback == null)
          break;
        this.mOperationCallback(GameSparksAccount.Result.AuthenticationFailure);
        break;
    }
  }

  private void RegistrationCallback(GameSparksServices.RegistrationResult iResult)
  {
    switch (iResult)
    {
      case GameSparksServices.RegistrationResult.Success:
        this.ClearCredentials();
        if (this.mOperationCallback == null)
          break;
        this.mOperationCallback(GameSparksAccount.Result.Success);
        break;
      case GameSparksServices.RegistrationResult.GSTaken:
        if (this.AreCredentialsValid(this.mCurrentGuid, this.mCurrentAuthToken))
        {
          this.SendAuthenticationRequest(this.mCurrentGuid, this.mCurrentAuthToken);
          break;
        }
        this.ClearCredentials();
        if (this.mOperationCallback == null)
          break;
        this.mOperationCallback(GameSparksAccount.Result.InvalidCredentials);
        break;
      default:
        this.ClearCredentials();
        if (this.mOperationCallback == null)
          break;
        this.mOperationCallback(GameSparksAccount.Result.RegistrationFailure);
        break;
    }
  }

  private void PendingOperationCallback(bool iAvailable)
  {
    lock (this.mGameSparksAvailabilityLock)
    {
      if (!iAvailable)
        return;
      GameSparksServices.AvailabilityChanged -= new GameSparksServices.AvailabilityChangedDelegate(this.PendingOperationCallback);
      if (this.mPendingOperation == GameSparksAccount.PendingOperationType.Authentication)
        this.SendAuthenticationRequest(this.mCurrentGuid, this.mCurrentAuthToken);
      else if (this.mPendingOperation == GameSparksAccount.PendingOperationType.Registration)
        this.SendRegistrationRequest(this.mCurrentGuid, this.mCurrentAuthToken);
      this.mPendingOperation = GameSparksAccount.PendingOperationType.None;
    }
  }

  private void ClearCredentials()
  {
    this.mCurrentGuid = string.Empty;
    this.mCurrentAuthToken = string.Empty;
  }

  private bool AreCredentialsValid(string iGuid, string iAuthToken)
  {
    return !string.IsNullOrEmpty(iGuid) && !string.IsNullOrEmpty(iAuthToken);
  }

  public enum Result
  {
    Success,
    AlreadyBusy,
    InvalidCredentials,
    RegistrationFailure,
    AuthenticationFailure,
  }

  public delegate void OperationCompleteDelegate(GameSparksAccount.Result iResult);

  private enum PendingOperationType
  {
    None,
    Registration,
    Authentication,
  }
}
