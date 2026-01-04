// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.ParadoxServices
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.WebTools.Paradox.Telemetry;
using PopsApi;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace Magicka.WebTools.Paradox;

public class ParadoxServices : Singleton<ParadoxServices>
{
  private const string VARIANT_PARAM_NAME = "variant";
  private const string ERROR_MESSAGE_PARAM_NAME = "error_message";
  private const string PLAYER_SEGMENT_PARAM_NAME = "player_segment";
  private const string OS_VERSION_PARAM_NAME = "os_version";
  private const string SYSTEM_MEM_PARAM_NAME = "system_mem";
  private const string GFX_DEVICE_PARAM_NAME = "gfx_device";
  private const string GFX_MEM_PARAM_NAME = "gfx_mem";
  private const string GFX_DRIVER_PARAM_NAME = "gfx_driver";
  private const string CPU_TYPE_PARAM_NAME = "cpu_type";
  private const string LOGICAL_PROCESSORS_PARAM_NAME = "logical_processors";
  private const string MODE_PARAM_NAME = "mode";
  private const string LEVEL_PARAM_NAME = "level";
  private const string NO_OF_PLAYERS_PARAM_NAME = "number_of_players";
  private const string ONLINE_STATUS_PARAM_NAME = "online_status";
  private const string ACTIVE_PARAM_NAME = "active";
  private const string AD_ID_PARAM_NAME = "ad_id";
  private const string DLC_ID_PARAM_NAME = "dlc_name";
  private const string DLC_STEAM_ID_PARAM_NAME = "dlc_steam_id";
  private const string BUTTON_PARAM_NAME = "button";
  private const string ACTION_PARAM_NAME = "action";
  private const string TUTORIAL_NAME_PARAM_NAME = "tutorial_name";
  private const string TUTORIAL_STEP_PARAM_NAME = "tutorial_step";
  private const string TIME_SPENT_PARAM_NAME = "time_spent";
  private const string SPELLBOOK_PARAM_NAME = "spellbook";
  private const string CONTROLLER_ONE_PARAM_NAME = "controller_one";
  private const string CONTROLLER_TWO_PARAM_NAME = "controller_two";
  private const string CONTROLLER_THREE_PARAM_NAME = "controller_three";
  private const string CONTROLLER_FOUR_PARAM_NAME = "controller_four";
  private const string SPELL_PARAM_NAME = "spell";
  private const string COD_CATEGORY_PARAM_NAME = "cause_of_death_category";
  private const string COD_SPECIFIC_PARAM_NAME = "cause_of_death_specific";
  private const string GAME_NAME = "red_wizard";
  private const string GAME_UNIVERSE = "steam";
  private const int RETRY_ATTEMPT = 3;
  private const string EXCEPTION_TELEMETRY_DEFINITION_NOT_FOUND = "A telemetry definition with name {0} doesn't exists in the definition library.";
  private const string EXCEPTION_TELEMETRY_UNNAMED_EVENT = "Cannot send a telemetry event without name.";
  private const string EXCEPTION_TELEMETRY_PARAMETERS_MISMATCH = "Parameters mismatch for event {0}.";
  private const string EXCEPTION_TELEMETRY_NULL_EVENT_ARRAY = "Telemetry events cannot be a null array.";
  private const bool UNHANDLED_EXCEPTION_FILE_INFO = true;
  private PopsApiWrapper mParadoxOPS;
  private List<IPendingAsyncOperation> mRunningAsyncOperations = new List<IPendingAsyncOperation>();
  private object mLocalLock = new object();
  private readonly Dictionary<string, EventDefinition> mEventDefinitions = new Dictionary<string, EventDefinition>();
  private readonly List<IEventValidator> mValidators = new List<IEventValidator>();

  private void OnDefineTelemetryEvents()
  {
    this.RegisterDefinition("unhandled_exception", (object) "variant", (object) EventParameter.Type.String, (object) "error_message", (object) EventParameter.Type.String, (object) "os_version", (object) EventParameter.Type.String, (object) "system_mem", (object) EventParameter.Type.UInt64, (object) "gfx_device", (object) EventParameter.Type.String, (object) "gfx_mem", (object) EventParameter.Type.UInt, (object) "gfx_driver", (object) EventParameter.Type.String, (object) "cpu_type", (object) EventParameter.Type.String, (object) "logical_processors", (object) EventParameter.Type.UInt);
    this.RegisterDefinition("hardware_report", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "os_version", (object) EventParameter.Type.String, (object) "system_mem", (object) EventParameter.Type.UInt64, (object) "gfx_device", (object) EventParameter.Type.String, (object) "gfx_mem", (object) EventParameter.Type.UInt, (object) "gfx_driver", (object) EventParameter.Type.String, (object) "cpu_type", (object) EventParameter.Type.String, (object) "logical_processors", (object) EventParameter.Type.UInt);
    this.RegisterDefinition("dlc", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "active", (object) EventParameter.Type.Int, (object) "dlc_name", (object) EventParameter.Type.String, (object) "dlc_steam_id", (object) EventParameter.Type.UInt);
    this.RegisterDefinition("dlc_ad_clicked", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "ad_id", (object) EventParameter.Type.String, (object) "dlc_name", (object) EventParameter.Type.String, (object) "dlc_steam_id", (object) EventParameter.Type.UInt);
    this.RegisterDefinition("gameplay_started", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "mode", (object) EventParameter.Type.String, (object) "level", (object) EventParameter.Type.String, (object) "number_of_players", (object) EventParameter.Type.Int, (object) "online_status", (object) EventParameter.Type.NetworkStateEnum);
    this.RegisterDefinition("ingame_menu_clicked", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "mode", (object) EventParameter.Type.String, (object) "level", (object) EventParameter.Type.String, (object) "number_of_players", (object) EventParameter.Type.Int, (object) "online_status", (object) EventParameter.Type.NetworkStateEnum, (object) "button", (object) EventParameter.Type.String);
    this.RegisterDefinition("tutorial_action", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "action", (object) EventParameter.Type.TutorialActionEnum, (object) "tutorial_name", (object) EventParameter.Type.String, (object) "tutorial_step", (object) EventParameter.Type.String, (object) "time_spent", (object) EventParameter.Type.Int);
    this.RegisterDefinition("collect_spellbook", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "spellbook", (object) EventParameter.Type.MagickTypeEnum);
    this.RegisterDefinition("controller_setup", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "controller_one", (object) EventParameter.Type.ControllerTypeEnum, (object) "controller_two", (object) EventParameter.Type.ControllerTypeEnum, (object) "controller_three", (object) EventParameter.Type.ControllerTypeEnum, (object) "controller_four", (object) EventParameter.Type.ControllerTypeEnum);
    this.RegisterDefinition("spell_cast", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "mode", (object) EventParameter.Type.String, (object) "level", (object) EventParameter.Type.String, (object) "spell", (object) EventParameter.Type.String);
    this.RegisterDefinition("player_death", (object) "variant", (object) EventParameter.Type.String, (object) "player_segment", (object) EventParameter.Type.PlayerSegment, (object) "mode", (object) EventParameter.Type.String, (object) "level", (object) EventParameter.Type.String, (object) "cause_of_death_category", (object) EventParameter.Type.PlayerDeath, (object) "cause_of_death_specific", (object) EventParameter.Type.String);
  }

  public bool IsInitialized => this.mParadoxOPS != null;

  public static bool HasInstanceAndInitialized
  {
    get
    {
      return Singleton<ParadoxServices>.HasInstance & Singleton<ParadoxServices>.Instance.IsInitialized;
    }
  }

  public bool IsSandbox => false;

  public void Initialize()
  {
    ParadoxUtils.EnsureParadoxFolder();
    string universeId = SteamUser.GetSteamID().AsUInt64.ToString();
    this.mParadoxOPS = new PopsApiWrapper("red_wizard", Application.ProductVersion, string.Empty, string.Empty, "steam", universeId, this.IsSandbox, runUpdateThread: false, logFunction: new PopsApiWrapper.LogCallback(this.DebugWriteLine));
    this.mParadoxOPS.SetRootPath(".");
    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
    this.OnDefineTelemetryEvents();
  }

  public void Update()
  {
    this.mParadoxOPS.Update();
    lock (this.mLocalLock)
    {
      int index = 0;
      while (index < this.mRunningAsyncOperations.Count)
      {
        if (this.mRunningAsyncOperations[index].EndIfComplete())
          this.mRunningAsyncOperations.RemoveAt(index);
        else
          ++index;
      }
    }
  }

  private void DebugWriteLine(string iMessage)
  {
    Logger.LogDebug(Logger.Source.ParadoxOPS, iMessage);
  }

  public void Dispose()
  {
    if (this.mParadoxOPS != null)
    {
      this.mParadoxOPS.Dispose();
      this.mParadoxOPS = (PopsApiWrapper) null;
    }
    AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(this.OnUnhandledException);
  }

  public void AccountAddCredentials(
    string iEmail,
    string iPassword,
    string iEmailTemplate,
    string iLandingUrl,
    bool iMarketingPermission,
    ParadoxServices.AccountAddCredentialsDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountAddCredentials), "EndAccountAddCredentials", this.mParadoxOPS.BeginAccountAddCredentials(iEmail, iPassword, iEmailTemplate, iLandingUrl, iMarketingPermission), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountAddEmail(
    string iEmail,
    string iEmailTemplate,
    string iLandingUrl,
    bool iMarketingPermission,
    ParadoxServices.AccountAddEmailDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountAddEmail), "EndAccountAddEmail", this.mParadoxOPS.BeginAccountAddEmail(iEmail, iEmailTemplate, iLandingUrl, iMarketingPermission), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountConnectAccountSteam(
    string iAppId,
    string iAuthTicket,
    ParadoxServices.AccountConnectAccountSteamDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountConnectAccountSteam), "EndAccountConnectAccountSteam", this.mParadoxOPS.BeginAccountConnectAccountSteam(iAppId, iAuthTicket), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountConnectAccountTwitch(
    string iAccessToken,
    string iClientId,
    ParadoxServices.AccountConnectAccountTwitchDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountConnectAccountTwitch), "EndAccountConnectAccountTwitch", this.mParadoxOPS.BeginAccountConnectAccountTwitch(iAccessToken, iClientId), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountConnections(
    ParadoxServices.AccountConnectionsDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.Connection>>(this.mParadoxOPS, nameof (AccountConnections), "EndAccountConnections", this.mParadoxOPS.BeginAccountConnections(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountCreate(
    string iEmail,
    string iPassword,
    string iLanguageCode,
    string iCountryCode,
    DateTime? iDateOfBirth,
    string iSource,
    string iReferer,
    string iSourceService,
    string iCampaign,
    string iMedium,
    string iChannel,
    string iFirstName,
    string iLastName,
    string iAddressLine1,
    string iAddressLine2,
    string iCity,
    string iState,
    string iZipCode,
    string iPhone,
    string iEmailTemplate,
    string iLandingUrl,
    string iRefererAccount,
    ParadoxServices.AccountCreateDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountCreate), "EndAccountCreate", this.mParadoxOPS.BeginAccountCreate(iEmail, iPassword, iLanguageCode, iCountryCode, iDateOfBirth, iSource, iReferer, iSourceService, iCampaign, iMedium, iChannel, iFirstName, iLastName, iAddressLine1, iAddressLine2, iCity, iState, iZipCode, iPhone, iEmailTemplate, iLandingUrl, iRefererAccount), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountCreateShadow(
    string iDeviceId,
    string iIdType,
    string iSourceService,
    ParadoxServices.AccountCreateShadowDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountCreateShadow), "EndAccountCreateShadow", this.mParadoxOPS.BeginAccountCreateShadow(iDeviceId, iIdType, iSourceService), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountDisconnectAccountSteam(
    ParadoxServices.AccountDisconnectAccountSteamDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountDisconnectAccountSteam), "EndAccountDisconnectAccountSteam", this.mParadoxOPS.BeginAccountDisconnectAccountSteam(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountDisconnectAccountSteamTicket(
    string iAppId,
    string iAuthTicket,
    ParadoxServices.AccountDisconnectAccountSteamTicketDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountDisconnectAccountSteamTicket), "EndAccountDisconnectAccountSteamTicket", this.mParadoxOPS.BeginAccountDisconnectAccountSteamTicket(iAppId, iAuthTicket), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountDisconnectAccountTwitch(
    string iAccessToken,
    string iClientId,
    ParadoxServices.AccountDisconnectAccountTwitchDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountDisconnectAccountTwitch), "EndAccountDisconnectAccountTwitch", this.mParadoxOPS.BeginAccountDisconnectAccountTwitch(iAccessToken, iClientId), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountGetDetails(
    ParadoxServices.AccountGetDetailsDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.AccountGetDetailsResult>(this.mParadoxOPS, nameof (AccountGetDetails), "EndAccountGetDetails", this.mParadoxOPS.BeginAccountGetDetails(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountGetMergeStatus(
    string iMergeTaskId,
    ParadoxServices.AccountGetMergeDetailsDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.AccountGetMergeStatusResult>(this.mParadoxOPS, "AccountGetMergeDetails", "EndAccountGetMergeStatus", this.mParadoxOPS.BeginAccountGetMergeStatus(iMergeTaskId), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountIsOnline(
    ParadoxServices.AccountIsOnlineDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountIsOnline), "EndAccountIsOnline", this.mParadoxOPS.BeginAccountIsOnline(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountLogin(
    string iUserName,
    string iPassword,
    ParadoxServices.AccountLoginDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountLogin), "EndAccountLogin", this.mParadoxOPS.BeginAccountLogin(iUserName, iPassword), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountLoginShadow(
    string iDeviceId,
    string iIdType,
    ParadoxServices.AccountLoginShadowDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountLoginShadow), "EndAccountLoginShadow", this.mParadoxOPS.BeginAccountLoginShadow(iDeviceId, iIdType), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountLoginSteamTicket(
    string iAppId,
    string iAuthTicket,
    ParadoxServices.AccountLoginSteamTicketDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountLoginSteamTicket), "EndAccountLoginSteamTicket", this.mParadoxOPS.BeginAccountLoginSteamTicket(iAppId, iAuthTicket), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountLoginTwitch(
    string iAccessToken,
    string iClientId,
    ParadoxServices.AccountLoginTwitchDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountLoginTwitch), "EndAccountLoginTwitch", this.mParadoxOPS.BeginAccountLoginTwitch(iAccessToken, iClientId), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountLoginWithAuthToken(
    string iAuthToken,
    ParadoxServices.AccountLoginWithAuthTokenDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountLoginWithAuthToken), "EndAccountLoginWithAuthToken", this.mParadoxOPS.BeginAccountLoginWithAuthToken(iAuthToken), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountMergeShadow(
    string iDeviceId,
    string iIdType,
    ParadoxServices.AccountMergeShadowDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<string>(this.mParadoxOPS, nameof (AccountMergeShadow), "EndAccountMergeShadow", this.mParadoxOPS.BeginAccountMergeShadow(iDeviceId, iIdType), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountUpdateDetails(
    string iFirstName,
    string iLastName,
    string iAddressLine1,
    string iAddressLine2,
    string iCity,
    string iState,
    string iZipCode,
    string iCountry,
    string iPhone,
    string iLanguage,
    string iDateOfBirth,
    ParadoxServices.AccountUpdateDetailsDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountUpdateDetails), "EndAccountUpdateDetails", this.mParadoxOPS.BeginAccountUpdateDetails(iFirstName, iLastName, iAddressLine1, iAddressLine2, iCity, iState, iZipCode, iCountry, iPhone, iLanguage, iDateOfBirth), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AccountResetPassword(
    string iEmail,
    string iTemplateName,
    ParadoxServices.AccountResetPasswordDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AccountResetPassword), "EndAccountResetPassword", this.mParadoxOPS.BeginAccountResetPassword(iEmail, iTemplateName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AchievementsGet(
    ICollection<string> iEntries,
    ParadoxServices.AchievementsGetDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AchievementsGetResultEntry>>(this.mParadoxOPS, nameof (AchievementsGet), "EndAchievementsGet", this.mParadoxOPS.BeginAchievementsGet(iEntries), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AchievementsList(
    ParadoxServices.AchievementsListDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AchievementsListResultEntry>>(this.mParadoxOPS, nameof (AchievementsList), "EndAchievementsList", this.mParadoxOPS.BeginAchievementsList(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AchievementsSend(
    ICollection<PopsApiWrapper.AchievementsSendEntry> iEntries,
    ParadoxServices.AchievementsSendDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AchievementsSend), "EndAchievementsSend", this.mParadoxOPS.BeginAchievementsSend(iEntries), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AchievementsStoreToDisk(
    ParadoxServices.AchievementsStoreToDiskDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AchievementsStoreToDisk), "EndAchievementsStoreToDisk", this.mParadoxOPS.BeginAchievementsStoreToDisk(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AppStatsDecrease(
    ICollection<PopsApiWrapper.AppStatsEntry> iEntries,
    bool useAuthentication,
    ParadoxServices.AppStatsDecreaseDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AppStatsChangeResult>>(this.mParadoxOPS, nameof (AppStatsDecrease), "EndAppStatsDecrease", this.mParadoxOPS.BeginAppStatsDecrease(iEntries, useAuthentication), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AppStatsGet(
    ICollection<string> iEntries,
    bool useAuthentication,
    ParadoxServices.AppStatsGetDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AppStatsGetResult>>(this.mParadoxOPS, nameof (AppStatsGet), "EndAppStatsGet", this.mParadoxOPS.BeginAppStatsGet(iEntries, useAuthentication), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AppStatsIncrease(
    ICollection<PopsApiWrapper.AppStatsEntry> iEntries,
    bool useAuthentication,
    ParadoxServices.AppStatsIncreaseDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AppStatsChangeResult>>(this.mParadoxOPS, nameof (AppStatsIncrease), "EndAppStatsIncrease", this.mParadoxOPS.BeginAppStatsIncrease(iEntries, useAuthentication), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void AuthTokenInvalidate(
    ParadoxServices.AuthTokenInvalidateDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (AuthTokenInvalidate), "EndAuthTokenInvalidate", this.mParadoxOPS.BeginAuthTokenInvalidate(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void FetchLegalDocument(
    string iLocalization,
    string iPlatform,
    string iDocument,
    string iVersion,
    ParadoxServices.FetchLegalDocumentDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.LegalDocument>(this.mParadoxOPS, nameof (FetchLegalDocument), "EndFetchLegalDocument", this.mParadoxOPS.BeginFetchLegalDocument(iLocalization, iPlatform, iDocument, iVersion), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void FileStorageDelete(
    string iRemotePath,
    ParadoxServices.FileStorageDeleteDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (FileStorageDelete), "EndFileStorageDelete", this.mParadoxOPS.BeginFileStorageDelete(iRemotePath), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void FileStorageDownload(
    string iRemotePath,
    string iLocalPath,
    string iIfNoneMatch,
    ParadoxServices.FileStorageDownloadDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.FileInfo>(this.mParadoxOPS, nameof (FileStorageDownload), "EndFileStorageDownload", this.mParadoxOPS.BeginFileStorageDownload(iRemotePath, iLocalPath, iIfNoneMatch), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void FileStorageList(
    ParadoxServices.FileStorageListDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<List<PopsApiWrapper.FileListItem>>(this.mParadoxOPS, nameof (FileStorageList), "EndFileStorageList", this.mParadoxOPS.BeginFileStorageList(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void FileStorageUpload(
    string iLocalPath,
    string iRemotePath,
    ParadoxServices.FileStorageUploadDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.FileInfo>(this.mParadoxOPS, nameof (FileStorageUpload), "EndFileStorageUpload", this.mParadoxOPS.BeginFileStorageUpload(iLocalPath, iRemotePath), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void GameProfileCreate(
    string iProfileName,
    ParadoxServices.GameProfileCreateDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (GameProfileCreate), "EndGameProfileCreate", this.mParadoxOPS.BeginGameProfileCreate(iProfileName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void GameProfileDelete(
    ParadoxServices.GameProfileDeleteDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (GameProfileDelete), "EndGameProfileDelete", this.mParadoxOPS.BeginGameProfileDelete(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void GameProfileRetrieve(
    ParadoxServices.GameProfileRetrieveDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.GameProfile>(this.mParadoxOPS, nameof (GameProfileRetrieve), "EndGameProfileRetrieve", this.mParadoxOPS.BeginGameProfileRetrieve(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void GameProfileUpdate(
    string iProfileName,
    ParadoxServices.GameProfileUpdateDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (GameProfileUpdate), "EndGameProfileUpdate", this.mParadoxOPS.BeginGameProfileUpdate(iProfileName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void GetAppChildren(
    string iAppName,
    ParadoxServices.GetAppChildrenDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.App>>(this.mParadoxOPS, nameof (GetAppChildren), "EndGetAppChildren", this.mParadoxOPS.BeginGetAppChildren(iAppName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void GetOpenFriendRequests(
    ParadoxServices.GetOpenFriendRequestsDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.FriendRequest>>(this.mParadoxOPS, nameof (GetOpenFriendRequests), "EndGetOpenFriendRequests", this.mParadoxOPS.BeginGetOpenFriendRequests(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void IndexingCreate(
    string iIndexName,
    string iDoctype,
    ICollection<PopsApiWrapper.IndexingKeyValuePair> iMetaKeyVals,
    ICollection<PopsApiWrapper.IndexingKeyValuePair> iBodyKeyVals,
    ParadoxServices.IndexingCreateDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.IndexingResult>(this.mParadoxOPS, nameof (IndexingCreate), "EndIndexingCreate", this.mParadoxOPS.BeginIndexingCreate(iIndexName, iDoctype, iMetaKeyVals, iBodyKeyVals), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void IndexingDelete(
    string iIndexName,
    string iDocumentId,
    string iDoctype,
    ParadoxServices.IndexingDeleteDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (IndexingDelete), "EndIndexingDelete", this.mParadoxOPS.BeginIndexingDelete(iIndexName, iDocumentId, iDoctype), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void IndexingUpdate(
    string iIndexName,
    string iDocumentId,
    string iDoctype,
    ICollection<PopsApiWrapper.IndexingUpdateEntry> iUpdateEntries,
    ParadoxServices.IndexingUpdateDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.IndexingResult>(this.mParadoxOPS, nameof (IndexingUpdate), "EndIndexingUpdate", this.mParadoxOPS.BeginIndexingUpdate(iIndexName, iDocumentId, iDoctype, iUpdateEntries), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void InventoryDownloadsList(
    ParadoxServices.InventoryDownloadsListDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.DownloadsEntry>>(this.mParadoxOPS, nameof (InventoryDownloadsList), "EndInventoryDowloadsList", this.mParadoxOPS.BeginInventoryDownloadsList(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void InventoryGetDownloadUrl(
    string iSkuid,
    string iLabel,
    ParadoxServices.InventoryGetDownloadUrlDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<Uri>(this.mParadoxOPS, nameof (InventoryGetDownloadUrl), "EndInventoryDownloadUrl", this.mParadoxOPS.InventoryGetDownloadUrlAsync(iSkuid, iLabel), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void InventoryList(
    PopsApiWrapper.InventoryCategory iCategory,
    ParadoxServices.InventoryListDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.InventoryItems>(this.mParadoxOPS, nameof (InventoryList), "EndInventoryList", this.mParadoxOPS.BeginInventoryList(iCategory), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void InventoryOwnedProducts(
    ParadoxServices.InventoryOwnedProductsDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.OwnedProduct>>(this.mParadoxOPS, nameof (InventoryOwnedProducts), "EndInventoryOwnedProducts", this.mParadoxOPS.BeginInventoryOwnedProducts(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void KVStorageDelete(
    string iKey,
    ParadoxServices.KVStorageDeleteDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (KVStorageDelete), "EndKVStorageDelete", this.mParadoxOPS.BeginKVStorageDelete(iKey), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void KVStorageRead(
    string iPath,
    ParadoxServices.KVStorageReadDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.StorageReadEntry>>(this.mParadoxOPS, nameof (KVStorageRead), "EndKVStorageRead", this.mParadoxOPS.BeginKVStorageRead(iPath), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void KVStorageWrite(
    ICollection<PopsApiWrapper.StorageEntry> iEntries,
    ParadoxServices.KVStorageWriteDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (KVStorageWrite), "EndKVStorageWrite", this.mParadoxOPS.BeginKVStorageWrite(iEntries), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void NewsletterSubscribe(
    string iNewsletterName,
    ParadoxServices.NewsletterSubscribeDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (NewsletterSubscribe), "EndNewsletterSubscribe", this.mParadoxOPS.BeginNewsletterSubscribe(iNewsletterName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void NewsletterUnsubscribeAll(
    ParadoxServices.NewsletterUnsubscribeAllDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (NewsletterUnsubscribeAll), "EndNewsletterUnsubscribeAll", this.mParadoxOPS.BeginNewsletterUnsubscribeAll(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void RequestFeed(
    ParadoxServices.RequestFeedDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<List<FeedEntry>>(this.mParadoxOPS, nameof (RequestFeed), "EndRequestFeed", this.mParadoxOPS.BeginRequestFeed(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialAddFriend(
    string iFriendId,
    ParadoxServices.SocialAddFriendDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (SocialAddFriend), "EndSocialAddFriend", this.mParadoxOPS.BeginSocialAddFriend(iFriendId), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialConnect(
    ParadoxServices.SocialConnectDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (SocialConnect), "EndSocialConnect", this.mParadoxOPS.BeginSocialConnect(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialGetFriendsList(
    ParadoxServices.SocialGetFriendsListDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.FriendsListEntry>>(this.mParadoxOPS, nameof (SocialGetFriendsList), "EndSocialGetFriendsList", this.mParadoxOPS.BeginSocialGetFriendsList(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialJoinRoomConnect(
    string iRoomName,
    string iNickName,
    ParadoxServices.SocialJoinRoomConnectDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<string>(this.mParadoxOPS, nameof (SocialJoinRoomConnect), "EndSocialJoinRoom", this.mParadoxOPS.BeginSocialJoinRoomConnect(iRoomName, iNickName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialLeaveRoomConnect(
    string iRoomName,
    ParadoxServices.SocialLeaveRoomConnectDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<string>(this.mParadoxOPS, nameof (SocialLeaveRoomConnect), "EndSocialLeaveRoom", this.mParadoxOPS.BeginSocialLeaveRoomConnect(iRoomName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialProfileCreate(
    string iProfileName,
    ParadoxServices.SocialProfileCreateDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (SocialProfileCreate), "EndSocialProfileCreate", this.mParadoxOPS.BeginSocialProfileCreate(iProfileName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialProfileQuery(
    string iProfileName,
    ParadoxServices.SocialProfileQueryDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.GameProfile>(this.mParadoxOPS, nameof (SocialProfileQuery), "EndSocialProfileQuery", this.mParadoxOPS.BeginSocialProfileQuery(iProfileName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialProfileRetrieve(
    ParadoxServices.SocialProfileRetrieveDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.GameProfile>(this.mParadoxOPS, nameof (SocialProfileRetrieve), "EndSocialProfileRetrieve", this.mParadoxOPS.BeginSocialProfileRetrieve(), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialProfileUpdate(
    string iProfileName,
    ParadoxServices.SocialProfileUpdateDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (SocialProfileUpdate), "EndSocialProfileUpdate", this.mParadoxOPS.BeginSocialProfileUpdate(iProfileName), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialRemoveFriend(
    string iFriendId,
    ParadoxServices.SocialRemoveFriendDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (SocialRemoveFriend), "EndSocialRemoveFriend", this.mParadoxOPS.BeginSocialRemoveFriend(iFriendId), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialRemoveOutgoingFriendRequest(
    string iFriendId,
    ParadoxServices.SocialRemoveOutgoingFriendRequestDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (SocialRemoveOutgoingFriendRequest), "EndSocialRemoveOutgoingFriendRequest", this.mParadoxOPS.BeginSocialRemoveOutgoingFriendRequest(iFriendId), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public void SocialRespondToFriendRequest(
    string iFriendId,
    bool iConfirm,
    ParadoxServices.SocialRespondToFriendRequestDelegate iCallback,
    ParadoxServices.HandledExceptionDelegate iErrorCallback)
  {
    IPendingAsyncOperation pendingAsyncOperation = (IPendingAsyncOperation) new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, nameof (SocialRespondToFriendRequest), "EndSocialRespondToFriendRequest", this.mParadoxOPS.BeginSocialRespondToFriendRequest(iFriendId, iConfirm), (Delegate) iCallback, iErrorCallback);
    lock (this.mLocalLock)
      this.mRunningAsyncOperations.Add(pendingAsyncOperation);
  }

  public string RetrieveAuthToken()
  {
    string str = string.Empty;
    try
    {
      str = this.mParadoxOPS.AuthTokenRetrieve();
    }
    catch (Exception ex)
    {
    }
    return str;
  }

  public string RetrieveAccountGuid()
  {
    string str = string.Empty;
    try
    {
      str = this.mParadoxOPS.AccountGuidRetrieve();
    }
    catch (Exception ex)
    {
    }
    return str;
  }

  public void RegisterDefinition(string iEventName, params object[] iParameters)
  {
    this.mEventDefinitions.Add(iEventName, new EventDefinition(iParameters));
  }

  public void RegisterValidator(IEventValidator iValidator)
  {
    if (iValidator == null)
      throw new NullReferenceException("Cannot pass a null validator.");
    this.mValidators.Add(iValidator);
  }

  public bool ValidateEvent(string iEventName, object[] iValues)
  {
    for (int index = 0; index < this.mValidators.Count; ++index)
    {
      if (!this.mValidators[index].Validate(iEventName, iValues))
        return false;
    }
    return true;
  }

  public EventDefinition GetDefinition(string iEventName)
  {
    return this.mEventDefinitions.ContainsKey(iEventName) ? this.mEventDefinitions[iEventName] : throw new Exception($"A telemetry definition with name {iEventName} doesn't exists in the definition library.");
  }

  public void TelemetryEvent(string iEventName, params object[] iParameters)
  {
    EventDefinition eventDefinition = !string.IsNullOrEmpty(iEventName) ? this.GetDefinition(iEventName) : throw new Exception("Cannot send a telemetry event without name.");
    if (!eventDefinition.IsValid(iParameters))
      throw new Exception($"Parameters mismatch for event {iEventName}.");
    if (!this.ValidateEvent(iEventName, iParameters))
      return;
    List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
    for (int iParameterIndex = 0; iParameterIndex < eventDefinition.Count; ++iParameterIndex)
    {
      KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(eventDefinition.GetParameterName(iParameterIndex), EventParameter.ToString(iParameters[iParameterIndex], eventDefinition.GetParameterType(iParameterIndex)));
      keyValuePairs.Add(keyValuePair);
    }
    this.mParadoxOPS.SendTelemetry(iEventName, keyValuePairs);
  }

  public void TelemetryEvent(EventData[] iEvents)
  {
    if (iEvents == null)
      throw new NullReferenceException("Telemetry events cannot be a null array.");
    List<PopsApiWrapper.TelemetryEntry> telemetryEntryList = new List<PopsApiWrapper.TelemetryEntry>();
    for (int index1 = 0; index1 < iEvents.Length; ++index1)
    {
      EventData iEvent = iEvents[index1];
      EventDefinition eventDefinition = !string.IsNullOrEmpty(iEvent.Name) ? this.GetDefinition(iEvent.Name) : throw new Exception("Cannot send a telemetry event without name.");
      if (!eventDefinition.IsValid(iEvent.Parameters))
        throw new Exception($"Parameters mismatch for event {iEvent.Name}.");
      if (this.ValidateEvent(iEvent.Name, iEvent.Parameters))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        for (int index2 = 0; index2 < eventDefinition.Count; ++index2)
        {
          KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(eventDefinition.GetParameterName(index2), EventParameter.ToString(iEvent.GetParameter(index2), eventDefinition.GetParameterType(index2)));
          keyValuePairList.Add(keyValuePair);
        }
        telemetryEntryList.Add(new PopsApiWrapper.TelemetryEntry()
        {
          eventName = iEvent.Name,
          keyValuePairs = (ICollection<KeyValuePair<string, string>>) keyValuePairList
        });
        Logger.LogVerbose(Logger.Source.ParadoxServices, $"Telemetry event {iEvent.Name} have been dispatched.");
      }
      else
        Logger.LogWarning(Logger.Source.ParadoxServices, $"Telemetry event {iEvent.Name} failed the validation process and will be ignored.");
    }
    if (telemetryEntryList.Count <= 0)
      return;
    this.mParadoxOPS.SendTelemetryMulti((ICollection<PopsApiWrapper.TelemetryEntry>) telemetryEntryList);
  }

  public void OnUnhandledException(object iSender, UnhandledExceptionEventArgs iArgs)
  {
    Exception exceptionObject = (Exception) iArgs.ExceptionObject;
    StackFrame frame = new StackTrace(exceptionObject, true).GetFrame(0);
    this.TelemetryEvent("unhandled_exception", TelemetryUtils.GetUnhandledExceptionParameters("", $"{exceptionObject.Message} \nin {Path.GetFileName(frame.GetFileName())}({frame.GetFileLineNumber().ToString()})"));
    if (!iArgs.IsTerminating)
      return;
    Thread.Sleep(2000);
  }

  public delegate void AccountAddCredentialsDelegate(bool iSuccess);

  public delegate void AccountAddEmailDelegate(bool iSuccess);

  public delegate void AccountConnectAccountSteamDelegate(bool iSuccess);

  public delegate void AccountConnectAccountTwitchDelegate(bool iSuccess);

  public delegate void AccountConnectionsDelegate(
    ICollection<PopsApiWrapper.Connection> iConnections);

  public delegate void AccountCreateDelegate(bool iSuccess);

  public delegate void AccountCreateShadowDelegate(bool iSuccess);

  public delegate void AccountDisconnectAccountSteamDelegate(bool iSuccess);

  public delegate void AccountDisconnectAccountSteamTicketDelegate(bool iSuccess);

  public delegate void AccountDisconnectAccountTwitchDelegate(bool iSuccess);

  public delegate void AccountGetDetailsDelegate(PopsApiWrapper.AccountGetDetailsResult iDetails);

  public delegate void AccountGetMergeDetailsDelegate(
    PopsApiWrapper.AccountGetMergeStatusResult iDetails);

  public delegate void AccountIsOnlineDelegate(bool iSuccess);

  public delegate void AccountLoginDelegate(bool iSuccess);

  public delegate void AccountLoginShadowDelegate(bool iSuccess);

  public delegate void AccountLoginSteamTicketDelegate(bool iSuccess);

  public delegate void AccountLoginTwitchDelegate(bool iSuccess);

  public delegate void AccountLoginWithAuthTokenDelegate(bool iSuccess);

  public delegate void AccountMergeShadowDelegate(string iMergeTaskID);

  public delegate void AccountUpdateDetailsDelegate(bool iSuccess);

  public delegate void AccountResetPasswordDelegate(bool iSuccess);

  public delegate void AchievementsGetDelegate(
    ICollection<PopsApiWrapper.AchievementsGetResultEntry> iAchievements);

  public delegate void AchievementsListDelegate(
    ICollection<PopsApiWrapper.AchievementsListResultEntry> iAchievements);

  public delegate void AchievementsSendDelegate(bool iSuccess);

  public delegate void AchievementsStoreToDiskDelegate(bool iSuccess);

  public delegate void AppStatsDecreaseDelegate(
    ICollection<PopsApiWrapper.AppStatsChangeResult> iResult);

  public delegate void AppStatsGetDelegate(
    ICollection<PopsApiWrapper.AppStatsGetResult> iResult);

  public delegate void AppStatsIncreaseDelegate(
    ICollection<PopsApiWrapper.AppStatsChangeResult> iResult);

  public delegate void AuthTokenInvalidateDelegate(bool iSuccess);

  public delegate void FetchLegalDocumentDelegate(PopsApiWrapper.LegalDocument iDocument);

  public delegate void FileStorageDeleteDelegate(bool iSuccess);

  public delegate void FileStorageDownloadDelegate(PopsApiWrapper.FileInfo iFileInfo);

  public delegate void FileStorageListDelegate(List<PopsApiWrapper.FileListItem> iFiles);

  public delegate void FileStorageUploadDelegate(PopsApiWrapper.FileInfo iFile);

  public delegate void GameProfileCreateDelegate(bool iSuccess);

  public delegate void GameProfileDeleteDelegate(bool iSuccess);

  public delegate void GameProfileRetrieveDelegate(PopsApiWrapper.GameProfile iProfile);

  public delegate void GameProfileUpdateDelegate(bool iSuccess);

  public delegate void GetAppChildrenDelegate(ICollection<PopsApiWrapper.App> iChildren);

  public delegate void GetOpenFriendRequestsDelegate(
    ICollection<PopsApiWrapper.FriendRequest> iRequests);

  public delegate void IndexingCreateDelegate(PopsApiWrapper.IndexingResult iResult);

  public delegate void IndexingDeleteDelegate(bool iSuccess);

  public delegate void IndexingUpdateDelegate(PopsApiWrapper.IndexingResult iResult);

  public delegate void InventoryDownloadsListDelegate(
    ICollection<PopsApiWrapper.DownloadsEntry> iEntries);

  public delegate void InventoryGetDownloadUrlDelegate(Uri iUri);

  public delegate void InventoryListDelegate(PopsApiWrapper.InventoryItems iItems);

  public delegate void InventoryOwnedProductsDelegate(
    ICollection<PopsApiWrapper.OwnedProduct> iProducts);

  public delegate void KVStorageDeleteDelegate(bool iSuccess);

  public delegate void KVStorageReadDelegate(
    ICollection<PopsApiWrapper.StorageReadEntry> iEntries);

  public delegate void KVStorageWriteDelegate(bool iSuccess);

  public delegate void NewsletterSubscribeDelegate(bool iSuccess);

  public delegate void NewsletterUnsubscribeAllDelegate(bool iSuccess);

  public delegate void RequestFeedDelegate(List<FeedEntry> iEntries);

  public delegate void SocialAddFriendDelegate(bool iSuccess);

  public delegate void SocialConnectDelegate(bool iSuccess);

  public delegate void SocialGetFriendsListDelegate(
    ICollection<PopsApiWrapper.FriendsListEntry> iFriendList);

  public delegate void SocialJoinRoomConnectDelegate(string iName);

  public delegate void SocialLeaveRoomConnectDelegate(string iName);

  public delegate void SocialProfileCreateDelegate(bool iSuccess);

  public delegate void SocialProfileQueryDelegate(PopsApiWrapper.GameProfile iProfile);

  public delegate void SocialProfileRetrieveDelegate(PopsApiWrapper.GameProfile iProfile);

  public delegate void SocialProfileUpdateDelegate(bool iSuccess);

  public delegate void SocialRemoveFriendDelegate(bool iSuccess);

  public delegate void SocialRemoveOutgoingFriendRequestDelegate(bool iSuccess);

  public delegate void SocialRespondToFriendRequestDelegate(bool iSuccess);

  public delegate void PopsInitializedDelegate();

  public delegate void HandledExceptionDelegate(string iReason);

  public class PendingAsyncOperation<ReturnType> : IPendingAsyncOperation
  {
    private const string EXCEPTION_NULL_OPS = "PopsApiWrapper object cannot be null in a PendingAsyncOperation.";
    private const string EXCEPTION_EMPTY_INITIAL_METHOD_NAME = "The initial method name cannot be null or empty in PendingASyncOperation.";
    private const string EXCEPTION_EMPTY_END_METHOD_NAME = "The end method name cannot be null or empty in PendingASyncOperation.";
    private const string EXCEPTION_NULL_ASYNC_RESULT = "IAsyncResult object cannot be null in a PendingAsyncOperation.";
    private const string EXCEPTION_METHOD_NOT_FOUND = "No method with the name {0} exists in the PopsApiWrapper";
    private readonly PopsApiWrapper mParadoxOPS;
    private readonly string mInitialMethodName = string.Empty;
    private readonly string mEndMethodName = string.Empty;
    private readonly IAsyncResult mAsyncResult;
    private readonly Delegate mNormalCallbackDelegate;
    private readonly ParadoxServices.HandledExceptionDelegate mErrorCallback;

    public PendingAsyncOperation(
      PopsApiWrapper iParadoxOPS,
      string iInitialMethodName,
      string iEndMethodName,
      IAsyncResult iAsyncResult,
      Delegate iCallbackDelegate,
      ParadoxServices.HandledExceptionDelegate iErrorCallback)
    {
      this.mParadoxOPS = iParadoxOPS != null ? iParadoxOPS : throw new NullReferenceException("PopsApiWrapper object cannot be null in a PendingAsyncOperation.");
      this.mInitialMethodName = !string.IsNullOrEmpty(iInitialMethodName) ? iInitialMethodName : throw new Exception("The initial method name cannot be null or empty in PendingASyncOperation.");
      this.mEndMethodName = !string.IsNullOrEmpty(iEndMethodName) ? iEndMethodName : throw new Exception("The end method name cannot be null or empty in PendingASyncOperation.");
      this.mAsyncResult = iAsyncResult != null ? iAsyncResult : throw new NullReferenceException("IAsyncResult object cannot be null in a PendingAsyncOperation.");
      this.mNormalCallbackDelegate = iCallbackDelegate;
      this.mErrorCallback = iErrorCallback;
    }

    public bool EndIfComplete()
    {
      bool flag = false;
      if (this.mAsyncResult.IsCompleted)
      {
        MethodInfo method = this.mParadoxOPS.GetType().GetMethod(this.mEndMethodName);
        if (method == null)
          throw new Exception($"No method with the name {this.mEndMethodName} exists in the PopsApiWrapper");
        try
        {
          Logger.LogDebug(Logger.Source.ParadoxServices, "PendingAsyncOperation Calling for method " + method.Name);
          ReturnType returnType = (ReturnType) method.Invoke((object) this.mParadoxOPS, new object[1]
          {
            (object) this.mAsyncResult
          });
          if ((object) this.mNormalCallbackDelegate != null)
            this.mNormalCallbackDelegate.DynamicInvoke((object) returnType);
        }
        catch (Exception ex)
        {
          Exception exception = ex;
          string iReason = string.Empty;
          for (; exception != null; exception = exception.InnerException)
          {
            Logger.LogVerbose(Logger.Source.ParadoxServices, exception.Message);
            iReason = exception.Message;
          }
          if (this.mErrorCallback != null)
            this.mErrorCallback(iReason);
        }
        finally
        {
          flag = true;
        }
      }
      return flag;
    }
  }
}
