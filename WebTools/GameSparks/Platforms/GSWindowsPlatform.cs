// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.GameSparks.Platforms.GSWindowsPlatform
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using GameSparks;
using GameSparks.Core;
using System;
using System.IO;

#nullable disable
namespace Magicka.WebTools.GameSparks.Platforms;

[Serializable]
public class GSWindowsPlatform : GSPlatformBase
{
  private const string SAVE_FOLDER = "./gs";
  private const string SAVE_FILENAME = "platform.dat";
  private const string PLATFORM_KEY = "Windows";
  private const string PLATFORM_ID = "W8";
  private const string DEVICE_TYPE = "PC";
  private const string PLATFORM_SDK = "XNA";

  public static GSWindowsPlatform Create()
  {
    return GSPlatformBase.LoadPlatform<GSWindowsPlatform>(GSWindowsPlatform.SavePath);
  }

  private static string SavePath => Path.Combine("./gs", "platform.dat");

  public override string PersistentDataPath => "./gs";

  protected override void OnSavePlatform()
  {
    GSPlatformBase.SavePlatform<GSWindowsPlatform>(GSWindowsPlatform.SavePath, this);
  }

  public override string DeviceId => HardwareInfoManager.DeviceUniqueId;

  public override string DeviceOS => "W8";

  public override string DeviceType => "PC";

  public override string SDK => "XNA";

  public override string Platform => "Windows";

  public override IGameSparksWebSocket GetSocket(
    string iUrl,
    Action<string> iMessageReceived,
    Action iClosed,
    Action iOpened,
    Action<string> iError)
  {
    GameSparksWebSocket socket = new GameSparksWebSocket();
    socket.Initialize(iUrl, iMessageReceived, iClosed, iOpened, iError);
    return (IGameSparksWebSocket) socket;
  }

  public override IGameSparksTimer GetTimer() => (IGameSparksTimer) new GameSparksTimer();
}
