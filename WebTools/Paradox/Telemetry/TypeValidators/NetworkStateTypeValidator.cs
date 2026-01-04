// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.TypeValidators.NetworkStateTypeValidator
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Network;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry.TypeValidators;

public class NetworkStateTypeValidator : BaseTypeValidator<NetworkState>
{
  private const string TELEMETRY_OFFLINE = "offline";
  private const string TELEMETRY_HOST = "host";
  private const string TELEMETRY_CLIENT = "client";
  private const string TELEMETRY_INVALID = "invalid";

  protected override string ToString(NetworkState iValue)
  {
    string str = "invalid";
    switch (iValue)
    {
      case NetworkState.Offline:
        str = "offline";
        break;
      case NetworkState.Server:
        str = "host";
        break;
      case NetworkState.Client:
        str = "client";
        break;
    }
    return str;
  }
}
