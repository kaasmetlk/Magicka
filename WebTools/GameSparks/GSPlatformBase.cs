// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.GameSparks.GSPlatformBase
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using GameSparks;
using GameSparks.Core;
using Magicka.CoreFramework;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#nullable disable
namespace Magicka.WebTools.GameSparks;

[Serializable]
public abstract class GSPlatformBase : IGSPlatform
{
  private const string SERVICE_URL_BASE = "wss://service.gamesparks.net/ws/{0}";
  private const string GAMESPARK_SERVER = "Live";
  private const string GAMESPARKS_API_KEY = "G347409i9Fs2";
  private const string GAMESPARKS_SECRET = "lKGWRqlC7x9qUuatUjV34RmflqGbTp3F";
  private const string GAMESPARK_CONFIG = "RedWizardLive";
  private const int DEFAULT_TIME_OUT_DURATION = 10;
  private const bool GIVE_EXTRA_DEBUG = true;
  private string mAuthToken = string.Empty;
  private string mUserId = string.Empty;
  private int mRequestTimeOut = 10;
  [NonSerialized]
  private GameSparksServices mServices;

  public GameSparksServices Services
  {
    get => this.mServices;
    set
    {
      this.mServices = value != null ? value : throw new NullReferenceException("Cannot pass a null as GameSparksServices");
    }
  }

  public string AuthToken
  {
    get => this.mAuthToken;
    set
    {
      this.mAuthToken = value;
      this.OnSavePlatform();
    }
  }

  public string UserId
  {
    get => this.mUserId;
    set
    {
      this.mUserId = value;
      this.OnSavePlatform();
    }
  }

  public bool ExtraDebug => true;

  public int RequestTimeoutSeconds
  {
    get => this.mRequestTimeOut;
    set
    {
      this.mRequestTimeOut = value;
      this.OnSavePlatform();
    }
  }

  public void DebugMsg(string iMessage)
  {
    Logger.LogDebug(Logger.Source.GameSparks, $"[{"Live"} {"RedWizardLive"}] {iMessage}");
  }

  public string MakeHmac(string iStringToHmac, string iSecret)
  {
    return GameSparksUtil.MakeHmac(iStringToHmac, iSecret);
  }

  public bool PreviewBuild => false;

  public string GameSparksSecret => "lKGWRqlC7x9qUuatUjV34RmflqGbTp3F";

  public string ServiceUrl
  {
    get
    {
      string str = "G347409i9Fs2";
      if ("lKGWRqlC7x9qUuatUjV34RmflqGbTp3F".Contains(":"))
        str = $"{"lKGWRqlC7x9qUuatUjV34RmflqGbTp3F".Substring(0, "lKGWRqlC7x9qUuatUjV34RmflqGbTp3F".IndexOf(":"))}/{str}";
      return $"wss://service.gamesparks.net/ws/{str}";
    }
  }

  public abstract string PersistentDataPath { get; }

  public abstract string DeviceId { get; }

  public abstract string DeviceOS { get; }

  public abstract string DeviceType { get; }

  public abstract string Platform { get; }

  public abstract string SDK { get; }

  public void ExecuteOnMainThread(Action iAction)
  {
    if (this.mServices == null)
      throw new NullReferenceException("Cannot execute on main thread without services.");
    this.mServices.ExecuteInMainThread(iAction);
  }

  public abstract IGameSparksWebSocket GetSocket(
    string iUrl,
    Action<string> iMessageReceived,
    Action iClosed,
    Action iOpened,
    Action<string> iError);

  public abstract IGameSparksTimer GetTimer();

  protected abstract void OnSavePlatform();

  public static void SavePlatform<T>(string iFilename, T platform) where T : GSPlatformBase
  {
    string directoryName = Path.GetDirectoryName(iFilename);
    if (!Directory.Exists(directoryName))
      Directory.CreateDirectory(directoryName);
    FileStream serializationStream = new FileStream(iFilename, FileMode.OpenOrCreate, FileAccess.Write);
    new BinaryFormatter().Serialize((Stream) serializationStream, (object) platform);
    serializationStream.Close();
  }

  public static T LoadPlatform<T>(string iFilename) where T : GSPlatformBase, new()
  {
    if (!File.Exists(iFilename))
      return new T();
    FileStream serializationStream = new FileStream(iFilename, FileMode.Open, FileAccess.Read);
    T obj = (T) new BinaryFormatter().Deserialize((Stream) serializationStream);
    serializationStream.Close();
    return obj;
  }
}
