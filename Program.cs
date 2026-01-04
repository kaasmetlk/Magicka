// Decompiled with JetBrains decompiler
// Type: Magicka.Program
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using SteamWrapper;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace Magicka;

internal static class Program
{
  private static int Main(string[] args)
  {
    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.WriteReport);
    Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
    SHA256 shA256 = SHA256.Create();
    byte[] iA = new byte[32 /*0x20*/]
    {
      (byte) 181,
      (byte) 80 /*0x50*/,
      (byte) 84,
      (byte) 169,
      (byte) 217,
      (byte) 40,
      (byte) 124,
      (byte) 112 /*0x70*/,
      (byte) 75,
      (byte) 142,
      (byte) 10,
      (byte) 211,
      (byte) 172,
      (byte) 254,
      (byte) 241,
      (byte) 234,
      (byte) 92,
      (byte) 63 /*0x3F*/,
      (byte) 166,
      (byte) 152,
      (byte) 43,
      (byte) 32 /*0x20*/,
      (byte) 183,
      (byte) 227,
      (byte) 100,
      (byte) 102,
      (byte) 220,
      (byte) 118,
      (byte) 166,
      (byte) 173,
      (byte) 137,
      (byte) 37
    };
    Stream inputStream = (Stream) File.OpenRead("steam_api.dll");
    byte[] hash = shA256.ComputeHash(inputStream);
    if (!Helper.ArrayEquals(iA, hash))
    {
      int num = (int) MessageBox.Show("Your steam_api.dll is a different version than this game was created for. Please verify the integrity of the game cache.\nRight click on the game in the Steam Client and select \"Properties\". Go to \"Local Files\". Click \"Verify Integrity Of Game Cache\".", "Invalid version!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      inputStream.Close();
      return 1;
    }
    inputStream.Close();
    if (SteamAPI.RestartAppIfNecessary(42910U))
      return 0;
    if (!SteamAPI.Init())
    {
      int num = (int) MessageBox.Show("Steam must be running to play this game.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      return 1;
    }
    if (SteamUtils.GetAppID() != 42910U)
      return 1;
    ulong result = 0;
    string str1 = (string) null;
    bool flag = true;
    for (int index = 0; index < args.Length; ++index)
    {
      args[index] = args[index].ToLowerInvariant();
      if (args[index].Contains("-window"))
        flag = false;
      else if (args[index].Contains("+connect_lobby"))
      {
        if (args.Length > index && !ulong.TryParse(args[index + 1], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out result))
          result = 0UL;
      }
      else if (args[index].Contains("+connect"))
      {
        if (args.Length > index)
        {
          string str2 = args[index + 1];
        }
      }
      else if (args[index].Contains("+password") && args.Length > index)
        str1 = args[index + 1];
    }
    GlobalSettings.Instance.Fullscreen = flag;
    GlobalSettings.Instance.StartupLobby = result;
    GlobalSettings.Instance.StartupPassword = str1;
    using (Game instance = Game.Instance)
      instance.Run();
    SteamAPI.Shutdown();
    return 0;
  }

  public static void WriteReport(object sender, UnhandledExceptionEventArgs e)
  {
    string format = $"errorReport_{DateTime.Now.ToString("yyyy.MM.dd-hh.mm.ss")}_({{0}}).txt";
    int num = 1;
    while (File.Exists(string.Format(format, (object) num)))
      ++num;
    TextWriter text = (TextWriter) File.CreateText(string.Format(format, (object) num));
    string str1 = e.ExceptionObject.ToString();
    string str2 = Thread.CurrentThread.Name;
    if (string.IsNullOrEmpty(str2))
      str2 = "UnNamed";
    text.WriteLine("Version: {0}\tThread: {1}\n\n", (object) Application.ProductVersion, (object) str2);
    text.WriteLine(str1);
    text.WriteLine("\n");
    StackTrace stackTrace = new StackTrace(true);
    for (int index = 1; index < stackTrace.FrameCount; ++index)
    {
      StackFrame frame = stackTrace.GetFrame(index);
      text.WriteLine("0x{0:x4} {1}->{2}.{3}", (object) frame.GetILOffset(), (object) frame.GetMethod().Module, (object) frame.GetMethod().DeclaringType.FullName, (object) frame.GetMethod().Name);
    }
    text.Close();
    text.Dispose();
    if (Debugger.IsAttached)
      return;
    Process.GetCurrentProcess().Kill();
  }
}
