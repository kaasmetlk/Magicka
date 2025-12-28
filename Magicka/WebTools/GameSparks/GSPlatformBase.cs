using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GameSparks;
using GameSparks.Core;
using Magicka.CoreFramework;

namespace Magicka.WebTools.GameSparks
{
	// Token: 0x02000202 RID: 514
	[Serializable]
	public abstract class GSPlatformBase : IGSPlatform
	{
		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x060010C3 RID: 4291 RVA: 0x00069785 File Offset: 0x00067985
		// (set) Token: 0x060010C4 RID: 4292 RVA: 0x0006978D File Offset: 0x0006798D
		public GameSparksServices Services
		{
			get
			{
				return this.mServices;
			}
			set
			{
				if (value == null)
				{
					throw new NullReferenceException("Cannot pass a null as GameSparksServices");
				}
				this.mServices = value;
			}
		}

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x060010C5 RID: 4293 RVA: 0x000697A4 File Offset: 0x000679A4
		// (set) Token: 0x060010C6 RID: 4294 RVA: 0x000697AC File Offset: 0x000679AC
		public string AuthToken
		{
			get
			{
				return this.mAuthToken;
			}
			set
			{
				this.mAuthToken = value;
				this.OnSavePlatform();
			}
		}

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x060010C7 RID: 4295 RVA: 0x000697BB File Offset: 0x000679BB
		// (set) Token: 0x060010C8 RID: 4296 RVA: 0x000697C3 File Offset: 0x000679C3
		public string UserId
		{
			get
			{
				return this.mUserId;
			}
			set
			{
				this.mUserId = value;
				this.OnSavePlatform();
			}
		}

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x060010C9 RID: 4297 RVA: 0x000697D2 File Offset: 0x000679D2
		public bool ExtraDebug
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x060010CA RID: 4298 RVA: 0x000697D5 File Offset: 0x000679D5
		// (set) Token: 0x060010CB RID: 4299 RVA: 0x000697DD File Offset: 0x000679DD
		public int RequestTimeoutSeconds
		{
			get
			{
				return this.mRequestTimeOut;
			}
			set
			{
				this.mRequestTimeOut = value;
				this.OnSavePlatform();
			}
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x000697EC File Offset: 0x000679EC
		public void DebugMsg(string iMessage)
		{
			Logger.LogDebug(Logger.Source.GameSparks, string.Format("[{0} {1}] {2}", "Live", "RedWizardLive", iMessage));
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x00069809 File Offset: 0x00067A09
		public string MakeHmac(string iStringToHmac, string iSecret)
		{
			return GameSparksUtil.MakeHmac(iStringToHmac, iSecret);
		}

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x060010CE RID: 4302 RVA: 0x00069812 File Offset: 0x00067A12
		public bool PreviewBuild
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x060010CF RID: 4303 RVA: 0x00069815 File Offset: 0x00067A15
		public string GameSparksSecret
		{
			get
			{
				return "lKGWRqlC7x9qUuatUjV34RmflqGbTp3F";
			}
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x060010D0 RID: 4304 RVA: 0x0006981C File Offset: 0x00067A1C
		public string ServiceUrl
		{
			get
			{
				string text = "G347409i9Fs2";
				if ("lKGWRqlC7x9qUuatUjV34RmflqGbTp3F".Contains(":"))
				{
					text = "lKGWRqlC7x9qUuatUjV34RmflqGbTp3F".Substring(0, "lKGWRqlC7x9qUuatUjV34RmflqGbTp3F".IndexOf(":")) + "/" + text;
				}
				return string.Format("wss://service.gamesparks.net/ws/{0}", text);
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x060010D1 RID: 4305
		public abstract string PersistentDataPath { get; }

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x060010D2 RID: 4306
		public abstract string DeviceId { get; }

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x060010D3 RID: 4307
		public abstract string DeviceOS { get; }

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x060010D4 RID: 4308
		public abstract string DeviceType { get; }

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x060010D5 RID: 4309
		public abstract string Platform { get; }

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x060010D6 RID: 4310
		public abstract string SDK { get; }

		// Token: 0x060010D7 RID: 4311 RVA: 0x00069871 File Offset: 0x00067A71
		public void ExecuteOnMainThread(Action iAction)
		{
			if (this.mServices == null)
			{
				throw new NullReferenceException("Cannot execute on main thread without services.");
			}
			this.mServices.ExecuteInMainThread(iAction);
		}

		// Token: 0x060010D8 RID: 4312
		public abstract IGameSparksWebSocket GetSocket(string iUrl, Action<string> iMessageReceived, Action iClosed, Action iOpened, Action<string> iError);

		// Token: 0x060010D9 RID: 4313
		public abstract IGameSparksTimer GetTimer();

		// Token: 0x060010DA RID: 4314
		protected abstract void OnSavePlatform();

		// Token: 0x060010DB RID: 4315 RVA: 0x00069894 File Offset: 0x00067A94
		public static void SavePlatform<T>(string iFilename, T platform) where T : GSPlatformBase
		{
			string directoryName = Path.GetDirectoryName(iFilename);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			FileStream fileStream = new FileStream(iFilename, FileMode.OpenOrCreate, FileAccess.Write);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(fileStream, platform);
			fileStream.Close();
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x000698DC File Offset: 0x00067ADC
		public static T LoadPlatform<T>(string iFilename) where T : GSPlatformBase, new()
		{
			if (File.Exists(iFilename))
			{
				FileStream fileStream = new FileStream(iFilename, FileMode.Open, FileAccess.Read);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				T result = (T)((object)binaryFormatter.Deserialize(fileStream));
				fileStream.Close();
				return result;
			}
			return Activator.CreateInstance<T>();
		}

		// Token: 0x04000F61 RID: 3937
		private const string SERVICE_URL_BASE = "wss://service.gamesparks.net/ws/{0}";

		// Token: 0x04000F62 RID: 3938
		private const string GAMESPARK_SERVER = "Live";

		// Token: 0x04000F63 RID: 3939
		private const string GAMESPARKS_API_KEY = "G347409i9Fs2";

		// Token: 0x04000F64 RID: 3940
		private const string GAMESPARKS_SECRET = "lKGWRqlC7x9qUuatUjV34RmflqGbTp3F";

		// Token: 0x04000F65 RID: 3941
		private const string GAMESPARK_CONFIG = "RedWizardLive";

		// Token: 0x04000F66 RID: 3942
		private const int DEFAULT_TIME_OUT_DURATION = 10;

		// Token: 0x04000F67 RID: 3943
		private const bool GIVE_EXTRA_DEBUG = true;

		// Token: 0x04000F68 RID: 3944
		private string mAuthToken = string.Empty;

		// Token: 0x04000F69 RID: 3945
		private string mUserId = string.Empty;

		// Token: 0x04000F6A RID: 3946
		private int mRequestTimeOut = 10;

		// Token: 0x04000F6B RID: 3947
		[NonSerialized]
		private GameSparksServices mServices;
	}
}
