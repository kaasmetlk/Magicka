using System;
using System.IO;
using GameSparks;
using GameSparks.Core;

namespace Magicka.WebTools.GameSparks.Platforms
{
	// Token: 0x02000203 RID: 515
	[Serializable]
	public class GSWindowsPlatform : GSPlatformBase
	{
		// Token: 0x060010DE RID: 4318 RVA: 0x00069940 File Offset: 0x00067B40
		public static GSWindowsPlatform Create()
		{
			return GSPlatformBase.LoadPlatform<GSWindowsPlatform>(GSWindowsPlatform.SavePath);
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x060010DF RID: 4319 RVA: 0x0006994C File Offset: 0x00067B4C
		private static string SavePath
		{
			get
			{
				return Path.Combine("./gs", "platform.dat");
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x060010E0 RID: 4320 RVA: 0x0006995D File Offset: 0x00067B5D
		public override string PersistentDataPath
		{
			get
			{
				return "./gs";
			}
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x00069964 File Offset: 0x00067B64
		protected override void OnSavePlatform()
		{
			GSPlatformBase.SavePlatform<GSWindowsPlatform>(GSWindowsPlatform.SavePath, this);
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x060010E2 RID: 4322 RVA: 0x00069971 File Offset: 0x00067B71
		public override string DeviceId
		{
			get
			{
				return HardwareInfoManager.DeviceUniqueId;
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x060010E3 RID: 4323 RVA: 0x00069978 File Offset: 0x00067B78
		public override string DeviceOS
		{
			get
			{
				return "W8";
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x060010E4 RID: 4324 RVA: 0x0006997F File Offset: 0x00067B7F
		public override string DeviceType
		{
			get
			{
				return "PC";
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x060010E5 RID: 4325 RVA: 0x00069986 File Offset: 0x00067B86
		public override string SDK
		{
			get
			{
				return "XNA";
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x060010E6 RID: 4326 RVA: 0x0006998D File Offset: 0x00067B8D
		public override string Platform
		{
			get
			{
				return "Windows";
			}
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x00069994 File Offset: 0x00067B94
		public override IGameSparksWebSocket GetSocket(string iUrl, Action<string> iMessageReceived, Action iClosed, Action iOpened, Action<string> iError)
		{
			GameSparksWebSocket gameSparksWebSocket = new GameSparksWebSocket();
			gameSparksWebSocket.Initialize(iUrl, iMessageReceived, iClosed, iOpened, iError);
			return gameSparksWebSocket;
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x000699B5 File Offset: 0x00067BB5
		public override IGameSparksTimer GetTimer()
		{
			return new GameSparksTimer();
		}

		// Token: 0x04000F6C RID: 3948
		private const string SAVE_FOLDER = "./gs";

		// Token: 0x04000F6D RID: 3949
		private const string SAVE_FILENAME = "platform.dat";

		// Token: 0x04000F6E RID: 3950
		private const string PLATFORM_KEY = "Windows";

		// Token: 0x04000F6F RID: 3951
		private const string PLATFORM_ID = "W8";

		// Token: 0x04000F70 RID: 3952
		private const string DEVICE_TYPE = "PC";

		// Token: 0x04000F71 RID: 3953
		private const string PLATFORM_SDK = "XNA";
	}
}
