using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using SteamWrapper;

namespace Magicka
{
	// Token: 0x020001A2 RID: 418
	internal static class Program
	{
		// Token: 0x06000C65 RID: 3173 RVA: 0x0004A4C0 File Offset: 0x000486C0
		private static int Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += Program.WriteReport;
			Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
			SHA256 sha = SHA256.Create();
			byte[] iA = new byte[]
			{
				181,
				80,
				84,
				169,
				217,
				40,
				124,
				112,
				75,
				142,
				10,
				211,
				172,
				254,
				241,
				234,
				92,
				63,
				166,
				152,
				43,
				32,
				183,
				227,
				100,
				102,
				220,
				118,
				166,
				173,
				137,
				37
			};
			Stream stream = File.OpenRead("steam_api.dll");
			byte[] iB = sha.ComputeHash(stream);
			if (!Helper.ArrayEquals(iA, iB))
			{
				MessageBox.Show("Your steam_api.dll is a different version than this game was created for. Please verify the integrity of the game cache.\nRight click on the game in the Steam Client and select \"Properties\". Go to \"Local Files\". Click \"Verify Integrity Of Game Cache\".", "Invalid version!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				stream.Close();
				return 1;
			}
			stream.Close();
			if (SteamAPI.RestartAppIfNecessary(42910U))
			{
				return 0;
			}
			if (!SteamAPI.Init())
			{
				MessageBox.Show("Steam must be running to play this game.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return 1;
			}
			if (SteamUtils.GetAppID() != 42910U)
			{
				return 1;
			}
			ulong startupLobby = 0UL;
			string startupPassword = null;
			bool fullscreen = true;
			for (int i = 0; i < args.Length; i++)
			{
				args[i] = args[i].ToLowerInvariant();
				if (args[i].Contains("-window"))
				{
					fullscreen = false;
				}
				else if (args[i].Contains("+connect_lobby"))
				{
					if (args.Length > i)
					{
						string s = args[i + 1];
						if (!ulong.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out startupLobby))
						{
							startupLobby = 0UL;
						}
					}
				}
				else if (args[i].Contains("+connect"))
				{
					if (args.Length > i)
					{
						string text = args[i + 1];
					}
				}
				else if (args[i].Contains("+password") && args.Length > i)
				{
					startupPassword = args[i + 1];
				}
			}
			GlobalSettings.Instance.Fullscreen = fullscreen;
			GlobalSettings.Instance.StartupLobby = startupLobby;
			GlobalSettings.Instance.StartupPassword = startupPassword;
			using (Game instance = Game.Instance)
			{
				instance.Run();
			}
			SteamAPI.Shutdown();
			return 0;
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x0004A694 File Offset: 0x00048894
		public static void WriteReport(object sender, UnhandledExceptionEventArgs e)
		{
			string str = DateTime.Now.ToString("yyyy.MM.dd-hh.mm.ss");
			string text = "errorReport_" + str + "_({0}).txt";
			int num = 1;
			while (File.Exists(string.Format(text, num)))
			{
				num++;
			}
			text = string.Format(text, num);
			TextWriter textWriter = File.CreateText(text);
			string value = e.ExceptionObject.ToString();
			string text2 = Thread.CurrentThread.Name;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "UnNamed";
			}
			textWriter.WriteLine("Version: {0}\tThread: {1}\n\n", Application.ProductVersion, text2);
			textWriter.WriteLine(value);
			textWriter.WriteLine("\n");
			StackTrace stackTrace = new StackTrace(true);
			for (int i = 1; i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				textWriter.WriteLine("0x{0:x4} {1}->{2}.{3}", new object[]
				{
					frame.GetILOffset(),
					frame.GetMethod().Module,
					frame.GetMethod().DeclaringType.FullName,
					frame.GetMethod().Name
				});
			}
			textWriter.Close();
			textWriter.Dispose();
			if (!Debugger.IsAttached)
			{
				Process.GetCurrentProcess().Kill();
			}
		}
	}
}
