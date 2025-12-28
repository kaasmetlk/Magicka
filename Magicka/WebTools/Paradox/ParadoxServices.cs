using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.WebTools.Paradox.Telemetry;
using PopsApi;
using SteamWrapper;

namespace Magicka.WebTools.Paradox
{
	// Token: 0x02000047 RID: 71
	public class ParadoxServices : Singleton<ParadoxServices>
	{
		// Token: 0x060002BF RID: 703 RVA: 0x00011C58 File Offset: 0x0000FE58
		private void OnDefineTelemetryEvents()
		{
			this.RegisterDefinition("unhandled_exception", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"error_message",
				EventParameter.Type.String,
				"os_version",
				EventParameter.Type.String,
				"system_mem",
				EventParameter.Type.UInt64,
				"gfx_device",
				EventParameter.Type.String,
				"gfx_mem",
				EventParameter.Type.UInt,
				"gfx_driver",
				EventParameter.Type.String,
				"cpu_type",
				EventParameter.Type.String,
				"logical_processors",
				EventParameter.Type.UInt
			});
			this.RegisterDefinition("hardware_report", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"os_version",
				EventParameter.Type.String,
				"system_mem",
				EventParameter.Type.UInt64,
				"gfx_device",
				EventParameter.Type.String,
				"gfx_mem",
				EventParameter.Type.UInt,
				"gfx_driver",
				EventParameter.Type.String,
				"cpu_type",
				EventParameter.Type.String,
				"logical_processors",
				EventParameter.Type.UInt
			});
			this.RegisterDefinition("dlc", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"active",
				EventParameter.Type.Int,
				"dlc_name",
				EventParameter.Type.String,
				"dlc_steam_id",
				EventParameter.Type.UInt
			});
			this.RegisterDefinition("dlc_ad_clicked", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"ad_id",
				EventParameter.Type.String,
				"dlc_name",
				EventParameter.Type.String,
				"dlc_steam_id",
				EventParameter.Type.UInt
			});
			this.RegisterDefinition("gameplay_started", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"mode",
				EventParameter.Type.String,
				"level",
				EventParameter.Type.String,
				"number_of_players",
				EventParameter.Type.Int,
				"online_status",
				EventParameter.Type.NetworkStateEnum
			});
			this.RegisterDefinition("ingame_menu_clicked", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"mode",
				EventParameter.Type.String,
				"level",
				EventParameter.Type.String,
				"number_of_players",
				EventParameter.Type.Int,
				"online_status",
				EventParameter.Type.NetworkStateEnum,
				"button",
				EventParameter.Type.String
			});
			this.RegisterDefinition("tutorial_action", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"action",
				EventParameter.Type.TutorialActionEnum,
				"tutorial_name",
				EventParameter.Type.String,
				"tutorial_step",
				EventParameter.Type.String,
				"time_spent",
				EventParameter.Type.Int
			});
			this.RegisterDefinition("collect_spellbook", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"spellbook",
				EventParameter.Type.MagickTypeEnum
			});
			this.RegisterDefinition("controller_setup", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"controller_one",
				EventParameter.Type.ControllerTypeEnum,
				"controller_two",
				EventParameter.Type.ControllerTypeEnum,
				"controller_three",
				EventParameter.Type.ControllerTypeEnum,
				"controller_four",
				EventParameter.Type.ControllerTypeEnum
			});
			this.RegisterDefinition("spell_cast", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"mode",
				EventParameter.Type.String,
				"level",
				EventParameter.Type.String,
				"spell",
				EventParameter.Type.String
			});
			this.RegisterDefinition("player_death", new object[]
			{
				"variant",
				EventParameter.Type.String,
				"player_segment",
				EventParameter.Type.PlayerSegment,
				"mode",
				EventParameter.Type.String,
				"level",
				EventParameter.Type.String,
				"cause_of_death_category",
				EventParameter.Type.PlayerDeath,
				"cause_of_death_specific",
				EventParameter.Type.String
			});
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x0001223D File Offset: 0x0001043D
		public bool IsInitialized
		{
			get
			{
				return this.mParadoxOPS != null;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x0001224B File Offset: 0x0001044B
		public static bool HasInstanceAndInitialized
		{
			get
			{
				return Singleton<ParadoxServices>.HasInstance & Singleton<ParadoxServices>.Instance.IsInitialized;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x0001225D File Offset: 0x0001045D
		public bool IsSandbox
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00012260 File Offset: 0x00010460
		public void Initialize()
		{
			ParadoxUtils.EnsureParadoxFolder();
			string universeId = SteamUser.GetSteamID().AsUInt64.ToString();
			this.mParadoxOPS = new PopsApiWrapper("red_wizard", Application.ProductVersion, string.Empty, string.Empty, "steam", universeId, this.IsSandbox, true, false, new PopsApiWrapper.LogCallback(this.DebugWriteLine), 3, false, false);
			this.mParadoxOPS.SetRootPath(".");
			AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;
			this.OnDefineTelemetryEvents();
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x000122F0 File Offset: 0x000104F0
		public void Update()
		{
			this.mParadoxOPS.Update();
			lock (this.mLocalLock)
			{
				int i = 0;
				while (i < this.mRunningAsyncOperations.Count)
				{
					IPendingAsyncOperation pendingAsyncOperation = this.mRunningAsyncOperations[i];
					if (pendingAsyncOperation.EndIfComplete())
					{
						this.mRunningAsyncOperations.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00012368 File Offset: 0x00010568
		private void DebugWriteLine(string iMessage)
		{
			Logger.LogDebug(Logger.Source.ParadoxOPS, iMessage);
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00012372 File Offset: 0x00010572
		public void Dispose()
		{
			if (this.mParadoxOPS != null)
			{
				this.mParadoxOPS.Dispose();
				this.mParadoxOPS = null;
			}
			AppDomain.CurrentDomain.UnhandledException -= this.OnUnhandledException;
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x000123A4 File Offset: 0x000105A4
		public void AccountAddCredentials(string iEmail, string iPassword, string iEmailTemplate, string iLandingUrl, bool iMarketingPermission, ParadoxServices.AccountAddCredentialsDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountAddCredentials(iEmail, iPassword, iEmailTemplate, iLandingUrl, iMarketingPermission);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountAddCredentials", "EndAccountAddCredentials", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x00012414 File Offset: 0x00010614
		public void AccountAddEmail(string iEmail, string iEmailTemplate, string iLandingUrl, bool iMarketingPermission, ParadoxServices.AccountAddEmailDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountAddEmail(iEmail, iEmailTemplate, iLandingUrl, iMarketingPermission);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountAddEmail", "EndAccountAddEmail", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x00012480 File Offset: 0x00010680
		public void AccountConnectAccountSteam(string iAppId, string iAuthTicket, ParadoxServices.AccountConnectAccountSteamDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountConnectAccountSteam(iAppId, iAuthTicket);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountConnectAccountSteam", "EndAccountConnectAccountSteam", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002CA RID: 714 RVA: 0x000124E8 File Offset: 0x000106E8
		public void AccountConnectAccountTwitch(string iAccessToken, string iClientId, ParadoxServices.AccountConnectAccountTwitchDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountConnectAccountTwitch(iAccessToken, iClientId);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountConnectAccountTwitch", "EndAccountConnectAccountTwitch", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00012550 File Offset: 0x00010750
		public void AccountConnections(ParadoxServices.AccountConnectionsDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountConnections();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.Connection>>(this.mParadoxOPS, "AccountConnections", "EndAccountConnections", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x000125B4 File Offset: 0x000107B4
		public void AccountCreate(string iEmail, string iPassword, string iLanguageCode, string iCountryCode, DateTime? iDateOfBirth, string iSource, string iReferer, string iSourceService, string iCampaign, string iMedium, string iChannel, string iFirstName, string iLastName, string iAddressLine1, string iAddressLine2, string iCity, string iState, string iZipCode, string iPhone, string iEmailTemplate, string iLandingUrl, string iRefererAccount, ParadoxServices.AccountCreateDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountCreate(iEmail, iPassword, iLanguageCode, iCountryCode, iDateOfBirth, iSource, iReferer, iSourceService, iCampaign, iMedium, iChannel, iFirstName, iLastName, iAddressLine1, iAddressLine2, iCity, iState, iZipCode, iPhone, iEmailTemplate, iLandingUrl, iRefererAccount);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountCreate", "EndAccountCreate", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00012644 File Offset: 0x00010844
		public void AccountCreateShadow(string iDeviceId, string iIdType, string iSourceService, ParadoxServices.AccountCreateShadowDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountCreateShadow(iDeviceId, iIdType, iSourceService);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountCreateShadow", "EndAccountCreateShadow", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002CE RID: 718 RVA: 0x000126B0 File Offset: 0x000108B0
		public void AccountDisconnectAccountSteam(ParadoxServices.AccountDisconnectAccountSteamDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountDisconnectAccountSteam();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountDisconnectAccountSteam", "EndAccountDisconnectAccountSteam", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00012714 File Offset: 0x00010914
		public void AccountDisconnectAccountSteamTicket(string iAppId, string iAuthTicket, ParadoxServices.AccountDisconnectAccountSteamTicketDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountDisconnectAccountSteamTicket(iAppId, iAuthTicket);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountDisconnectAccountSteamTicket", "EndAccountDisconnectAccountSteamTicket", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0001277C File Offset: 0x0001097C
		public void AccountDisconnectAccountTwitch(string iAccessToken, string iClientId, ParadoxServices.AccountDisconnectAccountTwitchDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountDisconnectAccountTwitch(iAccessToken, iClientId);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountDisconnectAccountTwitch", "EndAccountDisconnectAccountTwitch", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x000127E4 File Offset: 0x000109E4
		public void AccountGetDetails(ParadoxServices.AccountGetDetailsDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountGetDetails();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.AccountGetDetailsResult>(this.mParadoxOPS, "AccountGetDetails", "EndAccountGetDetails", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00012848 File Offset: 0x00010A48
		public void AccountGetMergeStatus(string iMergeTaskId, ParadoxServices.AccountGetMergeDetailsDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountGetMergeStatus(iMergeTaskId);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.AccountGetMergeStatusResult>(this.mParadoxOPS, "AccountGetMergeDetails", "EndAccountGetMergeStatus", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x000128B0 File Offset: 0x00010AB0
		public void AccountIsOnline(ParadoxServices.AccountIsOnlineDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountIsOnline();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountIsOnline", "EndAccountIsOnline", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00012914 File Offset: 0x00010B14
		public void AccountLogin(string iUserName, string iPassword, ParadoxServices.AccountLoginDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountLogin(iUserName, iPassword);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountLogin", "EndAccountLogin", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0001297C File Offset: 0x00010B7C
		public void AccountLoginShadow(string iDeviceId, string iIdType, ParadoxServices.AccountLoginShadowDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountLoginShadow(iDeviceId, iIdType);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountLoginShadow", "EndAccountLoginShadow", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x000129E4 File Offset: 0x00010BE4
		public void AccountLoginSteamTicket(string iAppId, string iAuthTicket, ParadoxServices.AccountLoginSteamTicketDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountLoginSteamTicket(iAppId, iAuthTicket);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountLoginSteamTicket", "EndAccountLoginSteamTicket", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00012A4C File Offset: 0x00010C4C
		public void AccountLoginTwitch(string iAccessToken, string iClientId, ParadoxServices.AccountLoginTwitchDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountLoginTwitch(iAccessToken, iClientId);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountLoginTwitch", "EndAccountLoginTwitch", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00012AB4 File Offset: 0x00010CB4
		public void AccountLoginWithAuthToken(string iAuthToken, ParadoxServices.AccountLoginWithAuthTokenDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountLoginWithAuthToken(iAuthToken);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountLoginWithAuthToken", "EndAccountLoginWithAuthToken", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00012B1C File Offset: 0x00010D1C
		public void AccountMergeShadow(string iDeviceId, string iIdType, ParadoxServices.AccountMergeShadowDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountMergeShadow(iDeviceId, iIdType);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<string>(this.mParadoxOPS, "AccountMergeShadow", "EndAccountMergeShadow", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00012B84 File Offset: 0x00010D84
		public void AccountUpdateDetails(string iFirstName, string iLastName, string iAddressLine1, string iAddressLine2, string iCity, string iState, string iZipCode, string iCountry, string iPhone, string iLanguage, string iDateOfBirth, ParadoxServices.AccountUpdateDetailsDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountUpdateDetails(iFirstName, iLastName, iAddressLine1, iAddressLine2, iCity, iState, iZipCode, iCountry, iPhone, iLanguage, iDateOfBirth);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountUpdateDetails", "EndAccountUpdateDetails", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00012C00 File Offset: 0x00010E00
		public void AccountResetPassword(string iEmail, string iTemplateName, ParadoxServices.AccountResetPasswordDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAccountResetPassword(iEmail, iTemplateName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AccountResetPassword", "EndAccountResetPassword", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00012C68 File Offset: 0x00010E68
		public void AchievementsGet(ICollection<string> iEntries, ParadoxServices.AchievementsGetDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAchievementsGet(iEntries);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AchievementsGetResultEntry>>(this.mParadoxOPS, "AchievementsGet", "EndAchievementsGet", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00012CD0 File Offset: 0x00010ED0
		public void AchievementsList(ParadoxServices.AchievementsListDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAchievementsList();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AchievementsListResultEntry>>(this.mParadoxOPS, "AchievementsList", "EndAchievementsList", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00012D34 File Offset: 0x00010F34
		public void AchievementsSend(ICollection<PopsApiWrapper.AchievementsSendEntry> iEntries, ParadoxServices.AchievementsSendDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAchievementsSend(iEntries);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AchievementsSend", "EndAchievementsSend", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00012D9C File Offset: 0x00010F9C
		public void AchievementsStoreToDisk(ParadoxServices.AchievementsStoreToDiskDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAchievementsStoreToDisk();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AchievementsStoreToDisk", "EndAchievementsStoreToDisk", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00012E00 File Offset: 0x00011000
		public void AppStatsDecrease(ICollection<PopsApiWrapper.AppStatsEntry> iEntries, bool useAuthentication, ParadoxServices.AppStatsDecreaseDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAppStatsDecrease(iEntries, useAuthentication);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AppStatsChangeResult>>(this.mParadoxOPS, "AppStatsDecrease", "EndAppStatsDecrease", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00012E68 File Offset: 0x00011068
		public void AppStatsGet(ICollection<string> iEntries, bool useAuthentication, ParadoxServices.AppStatsGetDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAppStatsGet(iEntries, useAuthentication);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AppStatsGetResult>>(this.mParadoxOPS, "AppStatsGet", "EndAppStatsGet", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00012ED0 File Offset: 0x000110D0
		public void AppStatsIncrease(ICollection<PopsApiWrapper.AppStatsEntry> iEntries, bool useAuthentication, ParadoxServices.AppStatsIncreaseDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAppStatsIncrease(iEntries, useAuthentication);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.AppStatsChangeResult>>(this.mParadoxOPS, "AppStatsIncrease", "EndAppStatsIncrease", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00012F38 File Offset: 0x00011138
		public void AuthTokenInvalidate(ParadoxServices.AuthTokenInvalidateDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginAuthTokenInvalidate();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "AuthTokenInvalidate", "EndAuthTokenInvalidate", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00012F9C File Offset: 0x0001119C
		public void FetchLegalDocument(string iLocalization, string iPlatform, string iDocument, string iVersion, ParadoxServices.FetchLegalDocumentDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginFetchLegalDocument(iLocalization, iPlatform, iDocument, iVersion);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.LegalDocument>(this.mParadoxOPS, "FetchLegalDocument", "EndFetchLegalDocument", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00013008 File Offset: 0x00011208
		public void FileStorageDelete(string iRemotePath, ParadoxServices.FileStorageDeleteDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginFileStorageDelete(iRemotePath);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "FileStorageDelete", "EndFileStorageDelete", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00013070 File Offset: 0x00011270
		public void FileStorageDownload(string iRemotePath, string iLocalPath, string iIfNoneMatch, ParadoxServices.FileStorageDownloadDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginFileStorageDownload(iRemotePath, iLocalPath, iIfNoneMatch);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.FileInfo>(this.mParadoxOPS, "FileStorageDownload", "EndFileStorageDownload", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x000130DC File Offset: 0x000112DC
		public void FileStorageList(ParadoxServices.FileStorageListDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginFileStorageList();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<List<PopsApiWrapper.FileListItem>>(this.mParadoxOPS, "FileStorageList", "EndFileStorageList", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00013140 File Offset: 0x00011340
		public void FileStorageUpload(string iLocalPath, string iRemotePath, ParadoxServices.FileStorageUploadDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginFileStorageUpload(iLocalPath, iRemotePath);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.FileInfo>(this.mParadoxOPS, "FileStorageUpload", "EndFileStorageUpload", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x000131A8 File Offset: 0x000113A8
		public void GameProfileCreate(string iProfileName, ParadoxServices.GameProfileCreateDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginGameProfileCreate(iProfileName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "GameProfileCreate", "EndGameProfileCreate", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00013210 File Offset: 0x00011410
		public void GameProfileDelete(ParadoxServices.GameProfileDeleteDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginGameProfileDelete();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "GameProfileDelete", "EndGameProfileDelete", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00013274 File Offset: 0x00011474
		public void GameProfileRetrieve(ParadoxServices.GameProfileRetrieveDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginGameProfileRetrieve();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.GameProfile>(this.mParadoxOPS, "GameProfileRetrieve", "EndGameProfileRetrieve", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002EC RID: 748 RVA: 0x000132D8 File Offset: 0x000114D8
		public void GameProfileUpdate(string iProfileName, ParadoxServices.GameProfileUpdateDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginGameProfileUpdate(iProfileName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "GameProfileUpdate", "EndGameProfileUpdate", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00013340 File Offset: 0x00011540
		public void GetAppChildren(string iAppName, ParadoxServices.GetAppChildrenDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginGetAppChildren(iAppName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.App>>(this.mParadoxOPS, "GetAppChildren", "EndGetAppChildren", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002EE RID: 750 RVA: 0x000133A8 File Offset: 0x000115A8
		public void GetOpenFriendRequests(ParadoxServices.GetOpenFriendRequestsDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginGetOpenFriendRequests();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.FriendRequest>>(this.mParadoxOPS, "GetOpenFriendRequests", "EndGetOpenFriendRequests", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0001340C File Offset: 0x0001160C
		public void IndexingCreate(string iIndexName, string iDoctype, ICollection<PopsApiWrapper.IndexingKeyValuePair> iMetaKeyVals, ICollection<PopsApiWrapper.IndexingKeyValuePair> iBodyKeyVals, ParadoxServices.IndexingCreateDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginIndexingCreate(iIndexName, iDoctype, iMetaKeyVals, iBodyKeyVals);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.IndexingResult>(this.mParadoxOPS, "IndexingCreate", "EndIndexingCreate", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00013478 File Offset: 0x00011678
		public void IndexingDelete(string iIndexName, string iDocumentId, string iDoctype, ParadoxServices.IndexingDeleteDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginIndexingDelete(iIndexName, iDocumentId, iDoctype);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "IndexingDelete", "EndIndexingDelete", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x000134E4 File Offset: 0x000116E4
		public void IndexingUpdate(string iIndexName, string iDocumentId, string iDoctype, ICollection<PopsApiWrapper.IndexingUpdateEntry> iUpdateEntries, ParadoxServices.IndexingUpdateDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginIndexingUpdate(iIndexName, iDocumentId, iDoctype, iUpdateEntries);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.IndexingResult>(this.mParadoxOPS, "IndexingUpdate", "EndIndexingUpdate", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00013550 File Offset: 0x00011750
		public void InventoryDownloadsList(ParadoxServices.InventoryDownloadsListDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginInventoryDownloadsList();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.DownloadsEntry>>(this.mParadoxOPS, "InventoryDownloadsList", "EndInventoryDowloadsList", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x000135B4 File Offset: 0x000117B4
		public void InventoryGetDownloadUrl(string iSkuid, string iLabel, ParadoxServices.InventoryGetDownloadUrlDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.InventoryGetDownloadUrlAsync(iSkuid, iLabel);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<Uri>(this.mParadoxOPS, "InventoryGetDownloadUrl", "EndInventoryDownloadUrl", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0001361C File Offset: 0x0001181C
		public void InventoryList(PopsApiWrapper.InventoryCategory iCategory, ParadoxServices.InventoryListDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginInventoryList(iCategory);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.InventoryItems>(this.mParadoxOPS, "InventoryList", "EndInventoryList", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00013684 File Offset: 0x00011884
		public void InventoryOwnedProducts(ParadoxServices.InventoryOwnedProductsDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginInventoryOwnedProducts();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.OwnedProduct>>(this.mParadoxOPS, "InventoryOwnedProducts", "EndInventoryOwnedProducts", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x000136E8 File Offset: 0x000118E8
		public void KVStorageDelete(string iKey, ParadoxServices.KVStorageDeleteDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginKVStorageDelete(iKey);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "KVStorageDelete", "EndKVStorageDelete", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00013750 File Offset: 0x00011950
		public void KVStorageRead(string iPath, ParadoxServices.KVStorageReadDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginKVStorageRead(iPath);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.StorageReadEntry>>(this.mParadoxOPS, "KVStorageRead", "EndKVStorageRead", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x000137B8 File Offset: 0x000119B8
		public void KVStorageWrite(ICollection<PopsApiWrapper.StorageEntry> iEntries, ParadoxServices.KVStorageWriteDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginKVStorageWrite(iEntries);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "KVStorageWrite", "EndKVStorageWrite", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00013820 File Offset: 0x00011A20
		public void NewsletterSubscribe(string iNewsletterName, ParadoxServices.NewsletterSubscribeDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginNewsletterSubscribe(iNewsletterName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "NewsletterSubscribe", "EndNewsletterSubscribe", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00013888 File Offset: 0x00011A88
		public void NewsletterUnsubscribeAll(ParadoxServices.NewsletterUnsubscribeAllDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginNewsletterUnsubscribeAll();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "NewsletterUnsubscribeAll", "EndNewsletterUnsubscribeAll", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002FB RID: 763 RVA: 0x000138EC File Offset: 0x00011AEC
		public void RequestFeed(ParadoxServices.RequestFeedDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginRequestFeed();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<List<FeedEntry>>(this.mParadoxOPS, "RequestFeed", "EndRequestFeed", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00013950 File Offset: 0x00011B50
		public void SocialAddFriend(string iFriendId, ParadoxServices.SocialAddFriendDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialAddFriend(iFriendId);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "SocialAddFriend", "EndSocialAddFriend", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x000139B8 File Offset: 0x00011BB8
		public void SocialConnect(ParadoxServices.SocialConnectDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialConnect();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "SocialConnect", "EndSocialConnect", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00013A1C File Offset: 0x00011C1C
		public void SocialGetFriendsList(ParadoxServices.SocialGetFriendsListDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialGetFriendsList();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<ICollection<PopsApiWrapper.FriendsListEntry>>(this.mParadoxOPS, "SocialGetFriendsList", "EndSocialGetFriendsList", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00013A80 File Offset: 0x00011C80
		public void SocialJoinRoomConnect(string iRoomName, string iNickName, ParadoxServices.SocialJoinRoomConnectDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialJoinRoomConnect(iRoomName, iNickName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<string>(this.mParadoxOPS, "SocialJoinRoomConnect", "EndSocialJoinRoom", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00013AE8 File Offset: 0x00011CE8
		public void SocialLeaveRoomConnect(string iRoomName, ParadoxServices.SocialLeaveRoomConnectDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialLeaveRoomConnect(iRoomName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<string>(this.mParadoxOPS, "SocialLeaveRoomConnect", "EndSocialLeaveRoom", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00013B50 File Offset: 0x00011D50
		public void SocialProfileCreate(string iProfileName, ParadoxServices.SocialProfileCreateDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialProfileCreate(iProfileName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "SocialProfileCreate", "EndSocialProfileCreate", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00013BB8 File Offset: 0x00011DB8
		public void SocialProfileQuery(string iProfileName, ParadoxServices.SocialProfileQueryDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialProfileQuery(iProfileName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.GameProfile>(this.mParadoxOPS, "SocialProfileQuery", "EndSocialProfileQuery", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00013C20 File Offset: 0x00011E20
		public void SocialProfileRetrieve(ParadoxServices.SocialProfileRetrieveDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialProfileRetrieve();
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<PopsApiWrapper.GameProfile>(this.mParadoxOPS, "SocialProfileRetrieve", "EndSocialProfileRetrieve", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00013C84 File Offset: 0x00011E84
		public void SocialProfileUpdate(string iProfileName, ParadoxServices.SocialProfileUpdateDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialProfileUpdate(iProfileName);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "SocialProfileUpdate", "EndSocialProfileUpdate", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00013CEC File Offset: 0x00011EEC
		public void SocialRemoveFriend(string iFriendId, ParadoxServices.SocialRemoveFriendDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialRemoveFriend(iFriendId);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "SocialRemoveFriend", "EndSocialRemoveFriend", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00013D54 File Offset: 0x00011F54
		public void SocialRemoveOutgoingFriendRequest(string iFriendId, ParadoxServices.SocialRemoveOutgoingFriendRequestDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialRemoveOutgoingFriendRequest(iFriendId);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "SocialRemoveOutgoingFriendRequest", "EndSocialRemoveOutgoingFriendRequest", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00013DBC File Offset: 0x00011FBC
		public void SocialRespondToFriendRequest(string iFriendId, bool iConfirm, ParadoxServices.SocialRespondToFriendRequestDelegate iCallback, ParadoxServices.HandledExceptionDelegate iErrorCallback)
		{
			IAsyncResult iAsyncResult = this.mParadoxOPS.BeginSocialRespondToFriendRequest(iFriendId, iConfirm);
			IPendingAsyncOperation item = new ParadoxServices.PendingAsyncOperation<bool>(this.mParadoxOPS, "SocialRespondToFriendRequest", "EndSocialRespondToFriendRequest", iAsyncResult, iCallback, iErrorCallback);
			lock (this.mLocalLock)
			{
				this.mRunningAsyncOperations.Add(item);
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00013E24 File Offset: 0x00012024
		public string RetrieveAuthToken()
		{
			string result = string.Empty;
			try
			{
				result = this.mParadoxOPS.AuthTokenRetrieve();
			}
			catch (Exception)
			{
			}
			return result;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00013E5C File Offset: 0x0001205C
		public string RetrieveAccountGuid()
		{
			string result = string.Empty;
			try
			{
				result = this.mParadoxOPS.AccountGuidRetrieve();
			}
			catch (Exception)
			{
			}
			return result;
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00013E94 File Offset: 0x00012094
		public void RegisterDefinition(string iEventName, params object[] iParameters)
		{
			this.mEventDefinitions.Add(iEventName, new EventDefinition(iParameters));
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00013EA8 File Offset: 0x000120A8
		public void RegisterValidator(IEventValidator iValidator)
		{
			if (iValidator == null)
			{
				throw new NullReferenceException("Cannot pass a null validator.");
			}
			this.mValidators.Add(iValidator);
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00013EC4 File Offset: 0x000120C4
		public bool ValidateEvent(string iEventName, object[] iValues)
		{
			for (int i = 0; i < this.mValidators.Count; i++)
			{
				if (!this.mValidators[i].Validate(iEventName, iValues))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00013EFF File Offset: 0x000120FF
		public EventDefinition GetDefinition(string iEventName)
		{
			if (!this.mEventDefinitions.ContainsKey(iEventName))
			{
				throw new Exception(string.Format("A telemetry definition with name {0} doesn't exists in the definition library.", iEventName));
			}
			return this.mEventDefinitions[iEventName];
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00013F2C File Offset: 0x0001212C
		public void TelemetryEvent(string iEventName, params object[] iParameters)
		{
			if (string.IsNullOrEmpty(iEventName))
			{
				throw new Exception("Cannot send a telemetry event without name.");
			}
			EventDefinition definition = this.GetDefinition(iEventName);
			if (!definition.IsValid(iParameters))
			{
				throw new Exception(string.Format("Parameters mismatch for event {0}.", iEventName));
			}
			if (this.ValidateEvent(iEventName, iParameters))
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				for (int i = 0; i < definition.Count; i++)
				{
					string parameterName = definition.GetParameterName(i);
					string value = EventParameter.ToString(iParameters[i], definition.GetParameterType(i));
					KeyValuePair<string, string> item = new KeyValuePair<string, string>(parameterName, value);
					list.Add(item);
				}
				this.mParadoxOPS.SendTelemetry(iEventName, list);
				return;
			}
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00013FC8 File Offset: 0x000121C8
		public void TelemetryEvent(EventData[] iEvents)
		{
			if (iEvents == null)
			{
				throw new NullReferenceException("Telemetry events cannot be a null array.");
			}
			List<PopsApiWrapper.TelemetryEntry> list = new List<PopsApiWrapper.TelemetryEntry>();
			foreach (EventData eventData in iEvents)
			{
				if (string.IsNullOrEmpty(eventData.Name))
				{
					throw new Exception("Cannot send a telemetry event without name.");
				}
				EventDefinition definition = this.GetDefinition(eventData.Name);
				if (!definition.IsValid(eventData.Parameters))
				{
					throw new Exception(string.Format("Parameters mismatch for event {0}.", eventData.Name));
				}
				if (this.ValidateEvent(eventData.Name, eventData.Parameters))
				{
					List<KeyValuePair<string, string>> list2 = new List<KeyValuePair<string, string>>();
					for (int j = 0; j < definition.Count; j++)
					{
						string parameterName = definition.GetParameterName(j);
						string value = EventParameter.ToString(eventData.GetParameter(j), definition.GetParameterType(j));
						KeyValuePair<string, string> item = new KeyValuePair<string, string>(parameterName, value);
						list2.Add(item);
					}
					list.Add(new PopsApiWrapper.TelemetryEntry
					{
						eventName = eventData.Name,
						keyValuePairs = list2
					});
					Logger.LogVerbose(Logger.Source.ParadoxServices, string.Format("Telemetry event {0} have been dispatched.", eventData.Name));
				}
				else
				{
					Logger.LogWarning(Logger.Source.ParadoxServices, string.Format("Telemetry event {0} failed the validation process and will be ignored.", eventData.Name));
				}
			}
			if (list.Count > 0)
			{
				this.mParadoxOPS.SendTelemetryMulti(list);
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00014124 File Offset: 0x00012324
		public void OnUnhandledException(object iSender, UnhandledExceptionEventArgs iArgs)
		{
			Exception ex = (Exception)iArgs.ExceptionObject;
			StackTrace stackTrace = new StackTrace(ex, true);
			StackFrame frame = stackTrace.GetFrame(0);
			string iErrorMessage = string.Concat(new string[]
			{
				ex.Message,
				" \nin ",
				Path.GetFileName(frame.GetFileName()),
				"(",
				frame.GetFileLineNumber().ToString(),
				")"
			});
			this.TelemetryEvent("unhandled_exception", TelemetryUtils.GetUnhandledExceptionParameters("", iErrorMessage));
			if (iArgs.IsTerminating)
			{
				Thread.Sleep(2000);
			}
		}

		// Token: 0x0400024E RID: 590
		private const string VARIANT_PARAM_NAME = "variant";

		// Token: 0x0400024F RID: 591
		private const string ERROR_MESSAGE_PARAM_NAME = "error_message";

		// Token: 0x04000250 RID: 592
		private const string PLAYER_SEGMENT_PARAM_NAME = "player_segment";

		// Token: 0x04000251 RID: 593
		private const string OS_VERSION_PARAM_NAME = "os_version";

		// Token: 0x04000252 RID: 594
		private const string SYSTEM_MEM_PARAM_NAME = "system_mem";

		// Token: 0x04000253 RID: 595
		private const string GFX_DEVICE_PARAM_NAME = "gfx_device";

		// Token: 0x04000254 RID: 596
		private const string GFX_MEM_PARAM_NAME = "gfx_mem";

		// Token: 0x04000255 RID: 597
		private const string GFX_DRIVER_PARAM_NAME = "gfx_driver";

		// Token: 0x04000256 RID: 598
		private const string CPU_TYPE_PARAM_NAME = "cpu_type";

		// Token: 0x04000257 RID: 599
		private const string LOGICAL_PROCESSORS_PARAM_NAME = "logical_processors";

		// Token: 0x04000258 RID: 600
		private const string MODE_PARAM_NAME = "mode";

		// Token: 0x04000259 RID: 601
		private const string LEVEL_PARAM_NAME = "level";

		// Token: 0x0400025A RID: 602
		private const string NO_OF_PLAYERS_PARAM_NAME = "number_of_players";

		// Token: 0x0400025B RID: 603
		private const string ONLINE_STATUS_PARAM_NAME = "online_status";

		// Token: 0x0400025C RID: 604
		private const string ACTIVE_PARAM_NAME = "active";

		// Token: 0x0400025D RID: 605
		private const string AD_ID_PARAM_NAME = "ad_id";

		// Token: 0x0400025E RID: 606
		private const string DLC_ID_PARAM_NAME = "dlc_name";

		// Token: 0x0400025F RID: 607
		private const string DLC_STEAM_ID_PARAM_NAME = "dlc_steam_id";

		// Token: 0x04000260 RID: 608
		private const string BUTTON_PARAM_NAME = "button";

		// Token: 0x04000261 RID: 609
		private const string ACTION_PARAM_NAME = "action";

		// Token: 0x04000262 RID: 610
		private const string TUTORIAL_NAME_PARAM_NAME = "tutorial_name";

		// Token: 0x04000263 RID: 611
		private const string TUTORIAL_STEP_PARAM_NAME = "tutorial_step";

		// Token: 0x04000264 RID: 612
		private const string TIME_SPENT_PARAM_NAME = "time_spent";

		// Token: 0x04000265 RID: 613
		private const string SPELLBOOK_PARAM_NAME = "spellbook";

		// Token: 0x04000266 RID: 614
		private const string CONTROLLER_ONE_PARAM_NAME = "controller_one";

		// Token: 0x04000267 RID: 615
		private const string CONTROLLER_TWO_PARAM_NAME = "controller_two";

		// Token: 0x04000268 RID: 616
		private const string CONTROLLER_THREE_PARAM_NAME = "controller_three";

		// Token: 0x04000269 RID: 617
		private const string CONTROLLER_FOUR_PARAM_NAME = "controller_four";

		// Token: 0x0400026A RID: 618
		private const string SPELL_PARAM_NAME = "spell";

		// Token: 0x0400026B RID: 619
		private const string COD_CATEGORY_PARAM_NAME = "cause_of_death_category";

		// Token: 0x0400026C RID: 620
		private const string COD_SPECIFIC_PARAM_NAME = "cause_of_death_specific";

		// Token: 0x0400026D RID: 621
		private const string GAME_NAME = "red_wizard";

		// Token: 0x0400026E RID: 622
		private const string GAME_UNIVERSE = "steam";

		// Token: 0x0400026F RID: 623
		private const int RETRY_ATTEMPT = 3;

		// Token: 0x04000270 RID: 624
		private const string EXCEPTION_TELEMETRY_DEFINITION_NOT_FOUND = "A telemetry definition with name {0} doesn't exists in the definition library.";

		// Token: 0x04000271 RID: 625
		private const string EXCEPTION_TELEMETRY_UNNAMED_EVENT = "Cannot send a telemetry event without name.";

		// Token: 0x04000272 RID: 626
		private const string EXCEPTION_TELEMETRY_PARAMETERS_MISMATCH = "Parameters mismatch for event {0}.";

		// Token: 0x04000273 RID: 627
		private const string EXCEPTION_TELEMETRY_NULL_EVENT_ARRAY = "Telemetry events cannot be a null array.";

		// Token: 0x04000274 RID: 628
		private const bool UNHANDLED_EXCEPTION_FILE_INFO = true;

		// Token: 0x04000275 RID: 629
		private PopsApiWrapper mParadoxOPS;

		// Token: 0x04000276 RID: 630
		private List<IPendingAsyncOperation> mRunningAsyncOperations = new List<IPendingAsyncOperation>();

		// Token: 0x04000277 RID: 631
		private object mLocalLock = new object();

		// Token: 0x04000278 RID: 632
		private readonly Dictionary<string, EventDefinition> mEventDefinitions = new Dictionary<string, EventDefinition>();

		// Token: 0x04000279 RID: 633
		private readonly List<IEventValidator> mValidators = new List<IEventValidator>();

		// Token: 0x02000048 RID: 72
		// (Invoke) Token: 0x06000313 RID: 787
		public delegate void AccountAddCredentialsDelegate(bool iSuccess);

		// Token: 0x02000049 RID: 73
		// (Invoke) Token: 0x06000317 RID: 791
		public delegate void AccountAddEmailDelegate(bool iSuccess);

		// Token: 0x0200004A RID: 74
		// (Invoke) Token: 0x0600031B RID: 795
		public delegate void AccountConnectAccountSteamDelegate(bool iSuccess);

		// Token: 0x0200004B RID: 75
		// (Invoke) Token: 0x0600031F RID: 799
		public delegate void AccountConnectAccountTwitchDelegate(bool iSuccess);

		// Token: 0x0200004C RID: 76
		// (Invoke) Token: 0x06000323 RID: 803
		public delegate void AccountConnectionsDelegate(ICollection<PopsApiWrapper.Connection> iConnections);

		// Token: 0x0200004D RID: 77
		// (Invoke) Token: 0x06000327 RID: 807
		public delegate void AccountCreateDelegate(bool iSuccess);

		// Token: 0x0200004E RID: 78
		// (Invoke) Token: 0x0600032B RID: 811
		public delegate void AccountCreateShadowDelegate(bool iSuccess);

		// Token: 0x0200004F RID: 79
		// (Invoke) Token: 0x0600032F RID: 815
		public delegate void AccountDisconnectAccountSteamDelegate(bool iSuccess);

		// Token: 0x02000050 RID: 80
		// (Invoke) Token: 0x06000333 RID: 819
		public delegate void AccountDisconnectAccountSteamTicketDelegate(bool iSuccess);

		// Token: 0x02000051 RID: 81
		// (Invoke) Token: 0x06000337 RID: 823
		public delegate void AccountDisconnectAccountTwitchDelegate(bool iSuccess);

		// Token: 0x02000052 RID: 82
		// (Invoke) Token: 0x0600033B RID: 827
		public delegate void AccountGetDetailsDelegate(PopsApiWrapper.AccountGetDetailsResult iDetails);

		// Token: 0x02000053 RID: 83
		// (Invoke) Token: 0x0600033F RID: 831
		public delegate void AccountGetMergeDetailsDelegate(PopsApiWrapper.AccountGetMergeStatusResult iDetails);

		// Token: 0x02000054 RID: 84
		// (Invoke) Token: 0x06000343 RID: 835
		public delegate void AccountIsOnlineDelegate(bool iSuccess);

		// Token: 0x02000055 RID: 85
		// (Invoke) Token: 0x06000347 RID: 839
		public delegate void AccountLoginDelegate(bool iSuccess);

		// Token: 0x02000056 RID: 86
		// (Invoke) Token: 0x0600034B RID: 843
		public delegate void AccountLoginShadowDelegate(bool iSuccess);

		// Token: 0x02000057 RID: 87
		// (Invoke) Token: 0x0600034F RID: 847
		public delegate void AccountLoginSteamTicketDelegate(bool iSuccess);

		// Token: 0x02000058 RID: 88
		// (Invoke) Token: 0x06000353 RID: 851
		public delegate void AccountLoginTwitchDelegate(bool iSuccess);

		// Token: 0x02000059 RID: 89
		// (Invoke) Token: 0x06000357 RID: 855
		public delegate void AccountLoginWithAuthTokenDelegate(bool iSuccess);

		// Token: 0x0200005A RID: 90
		// (Invoke) Token: 0x0600035B RID: 859
		public delegate void AccountMergeShadowDelegate(string iMergeTaskID);

		// Token: 0x0200005B RID: 91
		// (Invoke) Token: 0x0600035F RID: 863
		public delegate void AccountUpdateDetailsDelegate(bool iSuccess);

		// Token: 0x0200005C RID: 92
		// (Invoke) Token: 0x06000363 RID: 867
		public delegate void AccountResetPasswordDelegate(bool iSuccess);

		// Token: 0x0200005D RID: 93
		// (Invoke) Token: 0x06000367 RID: 871
		public delegate void AchievementsGetDelegate(ICollection<PopsApiWrapper.AchievementsGetResultEntry> iAchievements);

		// Token: 0x0200005E RID: 94
		// (Invoke) Token: 0x0600036B RID: 875
		public delegate void AchievementsListDelegate(ICollection<PopsApiWrapper.AchievementsListResultEntry> iAchievements);

		// Token: 0x0200005F RID: 95
		// (Invoke) Token: 0x0600036F RID: 879
		public delegate void AchievementsSendDelegate(bool iSuccess);

		// Token: 0x02000060 RID: 96
		// (Invoke) Token: 0x06000373 RID: 883
		public delegate void AchievementsStoreToDiskDelegate(bool iSuccess);

		// Token: 0x02000061 RID: 97
		// (Invoke) Token: 0x06000377 RID: 887
		public delegate void AppStatsDecreaseDelegate(ICollection<PopsApiWrapper.AppStatsChangeResult> iResult);

		// Token: 0x02000062 RID: 98
		// (Invoke) Token: 0x0600037B RID: 891
		public delegate void AppStatsGetDelegate(ICollection<PopsApiWrapper.AppStatsGetResult> iResult);

		// Token: 0x02000063 RID: 99
		// (Invoke) Token: 0x0600037F RID: 895
		public delegate void AppStatsIncreaseDelegate(ICollection<PopsApiWrapper.AppStatsChangeResult> iResult);

		// Token: 0x02000064 RID: 100
		// (Invoke) Token: 0x06000383 RID: 899
		public delegate void AuthTokenInvalidateDelegate(bool iSuccess);

		// Token: 0x02000065 RID: 101
		// (Invoke) Token: 0x06000387 RID: 903
		public delegate void FetchLegalDocumentDelegate(PopsApiWrapper.LegalDocument iDocument);

		// Token: 0x02000066 RID: 102
		// (Invoke) Token: 0x0600038B RID: 907
		public delegate void FileStorageDeleteDelegate(bool iSuccess);

		// Token: 0x02000067 RID: 103
		// (Invoke) Token: 0x0600038F RID: 911
		public delegate void FileStorageDownloadDelegate(PopsApiWrapper.FileInfo iFileInfo);

		// Token: 0x02000068 RID: 104
		// (Invoke) Token: 0x06000393 RID: 915
		public delegate void FileStorageListDelegate(List<PopsApiWrapper.FileListItem> iFiles);

		// Token: 0x02000069 RID: 105
		// (Invoke) Token: 0x06000397 RID: 919
		public delegate void FileStorageUploadDelegate(PopsApiWrapper.FileInfo iFile);

		// Token: 0x0200006A RID: 106
		// (Invoke) Token: 0x0600039B RID: 923
		public delegate void GameProfileCreateDelegate(bool iSuccess);

		// Token: 0x0200006B RID: 107
		// (Invoke) Token: 0x0600039F RID: 927
		public delegate void GameProfileDeleteDelegate(bool iSuccess);

		// Token: 0x0200006C RID: 108
		// (Invoke) Token: 0x060003A3 RID: 931
		public delegate void GameProfileRetrieveDelegate(PopsApiWrapper.GameProfile iProfile);

		// Token: 0x0200006D RID: 109
		// (Invoke) Token: 0x060003A7 RID: 935
		public delegate void GameProfileUpdateDelegate(bool iSuccess);

		// Token: 0x0200006E RID: 110
		// (Invoke) Token: 0x060003AB RID: 939
		public delegate void GetAppChildrenDelegate(ICollection<PopsApiWrapper.App> iChildren);

		// Token: 0x0200006F RID: 111
		// (Invoke) Token: 0x060003AF RID: 943
		public delegate void GetOpenFriendRequestsDelegate(ICollection<PopsApiWrapper.FriendRequest> iRequests);

		// Token: 0x02000070 RID: 112
		// (Invoke) Token: 0x060003B3 RID: 947
		public delegate void IndexingCreateDelegate(PopsApiWrapper.IndexingResult iResult);

		// Token: 0x02000071 RID: 113
		// (Invoke) Token: 0x060003B7 RID: 951
		public delegate void IndexingDeleteDelegate(bool iSuccess);

		// Token: 0x02000072 RID: 114
		// (Invoke) Token: 0x060003BB RID: 955
		public delegate void IndexingUpdateDelegate(PopsApiWrapper.IndexingResult iResult);

		// Token: 0x02000073 RID: 115
		// (Invoke) Token: 0x060003BF RID: 959
		public delegate void InventoryDownloadsListDelegate(ICollection<PopsApiWrapper.DownloadsEntry> iEntries);

		// Token: 0x02000074 RID: 116
		// (Invoke) Token: 0x060003C3 RID: 963
		public delegate void InventoryGetDownloadUrlDelegate(Uri iUri);

		// Token: 0x02000075 RID: 117
		// (Invoke) Token: 0x060003C7 RID: 967
		public delegate void InventoryListDelegate(PopsApiWrapper.InventoryItems iItems);

		// Token: 0x02000076 RID: 118
		// (Invoke) Token: 0x060003CB RID: 971
		public delegate void InventoryOwnedProductsDelegate(ICollection<PopsApiWrapper.OwnedProduct> iProducts);

		// Token: 0x02000077 RID: 119
		// (Invoke) Token: 0x060003CF RID: 975
		public delegate void KVStorageDeleteDelegate(bool iSuccess);

		// Token: 0x02000078 RID: 120
		// (Invoke) Token: 0x060003D3 RID: 979
		public delegate void KVStorageReadDelegate(ICollection<PopsApiWrapper.StorageReadEntry> iEntries);

		// Token: 0x02000079 RID: 121
		// (Invoke) Token: 0x060003D7 RID: 983
		public delegate void KVStorageWriteDelegate(bool iSuccess);

		// Token: 0x0200007A RID: 122
		// (Invoke) Token: 0x060003DB RID: 987
		public delegate void NewsletterSubscribeDelegate(bool iSuccess);

		// Token: 0x0200007B RID: 123
		// (Invoke) Token: 0x060003DF RID: 991
		public delegate void NewsletterUnsubscribeAllDelegate(bool iSuccess);

		// Token: 0x0200007C RID: 124
		// (Invoke) Token: 0x060003E3 RID: 995
		public delegate void RequestFeedDelegate(List<FeedEntry> iEntries);

		// Token: 0x0200007D RID: 125
		// (Invoke) Token: 0x060003E7 RID: 999
		public delegate void SocialAddFriendDelegate(bool iSuccess);

		// Token: 0x0200007E RID: 126
		// (Invoke) Token: 0x060003EB RID: 1003
		public delegate void SocialConnectDelegate(bool iSuccess);

		// Token: 0x0200007F RID: 127
		// (Invoke) Token: 0x060003EF RID: 1007
		public delegate void SocialGetFriendsListDelegate(ICollection<PopsApiWrapper.FriendsListEntry> iFriendList);

		// Token: 0x02000080 RID: 128
		// (Invoke) Token: 0x060003F3 RID: 1011
		public delegate void SocialJoinRoomConnectDelegate(string iName);

		// Token: 0x02000081 RID: 129
		// (Invoke) Token: 0x060003F7 RID: 1015
		public delegate void SocialLeaveRoomConnectDelegate(string iName);

		// Token: 0x02000082 RID: 130
		// (Invoke) Token: 0x060003FB RID: 1019
		public delegate void SocialProfileCreateDelegate(bool iSuccess);

		// Token: 0x02000083 RID: 131
		// (Invoke) Token: 0x060003FF RID: 1023
		public delegate void SocialProfileQueryDelegate(PopsApiWrapper.GameProfile iProfile);

		// Token: 0x02000084 RID: 132
		// (Invoke) Token: 0x06000403 RID: 1027
		public delegate void SocialProfileRetrieveDelegate(PopsApiWrapper.GameProfile iProfile);

		// Token: 0x02000085 RID: 133
		// (Invoke) Token: 0x06000407 RID: 1031
		public delegate void SocialProfileUpdateDelegate(bool iSuccess);

		// Token: 0x02000086 RID: 134
		// (Invoke) Token: 0x0600040B RID: 1035
		public delegate void SocialRemoveFriendDelegate(bool iSuccess);

		// Token: 0x02000087 RID: 135
		// (Invoke) Token: 0x0600040F RID: 1039
		public delegate void SocialRemoveOutgoingFriendRequestDelegate(bool iSuccess);

		// Token: 0x02000088 RID: 136
		// (Invoke) Token: 0x06000413 RID: 1043
		public delegate void SocialRespondToFriendRequestDelegate(bool iSuccess);

		// Token: 0x02000089 RID: 137
		// (Invoke) Token: 0x06000417 RID: 1047
		public delegate void PopsInitializedDelegate();

		// Token: 0x0200008A RID: 138
		// (Invoke) Token: 0x0600041B RID: 1051
		public delegate void HandledExceptionDelegate(string iReason);

		// Token: 0x0200008C RID: 140
		public class PendingAsyncOperation<ReturnType> : IPendingAsyncOperation
		{
			// Token: 0x0600041F RID: 1055 RVA: 0x00014200 File Offset: 0x00012400
			public PendingAsyncOperation(PopsApiWrapper iParadoxOPS, string iInitialMethodName, string iEndMethodName, IAsyncResult iAsyncResult, Delegate iCallbackDelegate, ParadoxServices.HandledExceptionDelegate iErrorCallback)
			{
				if (iParadoxOPS == null)
				{
					throw new NullReferenceException("PopsApiWrapper object cannot be null in a PendingAsyncOperation.");
				}
				this.mParadoxOPS = iParadoxOPS;
				if (string.IsNullOrEmpty(iInitialMethodName))
				{
					throw new Exception("The initial method name cannot be null or empty in PendingASyncOperation.");
				}
				this.mInitialMethodName = iInitialMethodName;
				if (string.IsNullOrEmpty(iEndMethodName))
				{
					throw new Exception("The end method name cannot be null or empty in PendingASyncOperation.");
				}
				this.mEndMethodName = iEndMethodName;
				if (iAsyncResult == null)
				{
					throw new NullReferenceException("IAsyncResult object cannot be null in a PendingAsyncOperation.");
				}
				this.mAsyncResult = iAsyncResult;
				this.mNormalCallbackDelegate = iCallbackDelegate;
				this.mErrorCallback = iErrorCallback;
			}

			// Token: 0x06000420 RID: 1056 RVA: 0x0001429C File Offset: 0x0001249C
			public bool EndIfComplete()
			{
				bool result = false;
				if (this.mAsyncResult.IsCompleted)
				{
					Type type = this.mParadoxOPS.GetType();
					MethodInfo method = type.GetMethod(this.mEndMethodName);
					if (method == null)
					{
						throw new Exception(string.Format("No method with the name {0} exists in the PopsApiWrapper", this.mEndMethodName));
					}
					try
					{
						Logger.LogDebug(Logger.Source.ParadoxServices, "PendingAsyncOperation Calling for method " + method.Name);
						ReturnType returnType = (ReturnType)((object)method.Invoke(this.mParadoxOPS, new object[]
						{
							this.mAsyncResult
						}));
						if (this.mNormalCallbackDelegate != null)
						{
							this.mNormalCallbackDelegate.DynamicInvoke(new object[]
							{
								returnType
							});
						}
					}
					catch (Exception ex)
					{
						Exception ex2 = ex;
						string iReason = string.Empty;
						while (ex2 != null)
						{
							Logger.LogVerbose(Logger.Source.ParadoxServices, ex2.Message);
							iReason = ex2.Message;
							ex2 = ex2.InnerException;
						}
						if (this.mErrorCallback != null)
						{
							this.mErrorCallback(iReason);
						}
					}
					finally
					{
						result = true;
					}
				}
				return result;
			}

			// Token: 0x0400027A RID: 634
			private const string EXCEPTION_NULL_OPS = "PopsApiWrapper object cannot be null in a PendingAsyncOperation.";

			// Token: 0x0400027B RID: 635
			private const string EXCEPTION_EMPTY_INITIAL_METHOD_NAME = "The initial method name cannot be null or empty in PendingASyncOperation.";

			// Token: 0x0400027C RID: 636
			private const string EXCEPTION_EMPTY_END_METHOD_NAME = "The end method name cannot be null or empty in PendingASyncOperation.";

			// Token: 0x0400027D RID: 637
			private const string EXCEPTION_NULL_ASYNC_RESULT = "IAsyncResult object cannot be null in a PendingAsyncOperation.";

			// Token: 0x0400027E RID: 638
			private const string EXCEPTION_METHOD_NOT_FOUND = "No method with the name {0} exists in the PopsApiWrapper";

			// Token: 0x0400027F RID: 639
			private readonly PopsApiWrapper mParadoxOPS;

			// Token: 0x04000280 RID: 640
			private readonly string mInitialMethodName = string.Empty;

			// Token: 0x04000281 RID: 641
			private readonly string mEndMethodName = string.Empty;

			// Token: 0x04000282 RID: 642
			private readonly IAsyncResult mAsyncResult;

			// Token: 0x04000283 RID: 643
			private readonly Delegate mNormalCallbackDelegate;

			// Token: 0x04000284 RID: 644
			private readonly ParadoxServices.HandledExceptionDelegate mErrorCallback;
		}
	}
}
