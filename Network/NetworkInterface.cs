// Decompiled with JetBrains decompiler
// Type: Magicka.Network.NetworkInterface
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using SteamWrapper;

#nullable disable
namespace Magicka.Network;

internal abstract class NetworkInterface
{
  protected GameType mGameType;

  public GameType GameType => this.mGameType;

  public abstract void Dispose();

  public abstract int Connections { get; }

  public abstract bool IsVACSecure { get; }

  public abstract float GetLatency(int iConnection);

  public abstract int GetLatencyMS(int iConnection);

  public abstract SteamID ServerID { get; }

  public abstract float GetLatency(SteamID iConnection);

  public abstract int GetLatencyMS(SteamID iConnection);

  public virtual void SendMessage<T>(ref T iMessage) where T : ISendable
  {
    this.SendMessage<T>(ref iMessage, P2PSend.ReliableWithBuffering);
  }

  public abstract void SendMessage<T>(ref T iMessage, P2PSend sendType) where T : ISendable;

  public virtual void SendMessage<T>(ref T iMessage, int iPeer) where T : ISendable
  {
    this.SendMessage<T>(ref iMessage, iPeer, P2PSend.ReliableWithBuffering);
  }

  public abstract void SendMessage<T>(ref T iMessage, int iPeer, P2PSend sendType) where T : ISendable;

  public virtual void SendMessage<T>(ref T iMessage, SteamID iPeer) where T : ISendable
  {
    this.SendMessage<T>(ref iMessage, iPeer, P2PSend.ReliableWithBuffering);
  }

  public abstract void SendMessage<T>(ref T iMessage, SteamID iPeer, P2PSend sendType) where T : ISendable;

  public abstract void SendUdpMessage<T>(ref T iMessage) where T : ISendable;

  public abstract void SendUdpMessage<T>(ref T iMessage, int iPeer) where T : ISendable;

  public abstract void SendUdpMessage<T>(ref T iMessage, SteamID iPeer) where T : ISendable;

  public abstract unsafe void SendRaw(PacketType iType, void* iPtr, int iLength);

  public abstract unsafe void SendRaw(PacketType iType, void* iPtr, int iLength, int iPeer);

  public abstract unsafe void SendRaw(PacketType iType, void* iPtr, int iLength, SteamID iPeer);

  public abstract void Sync();

  public abstract void Update();

  public abstract void CloseConnection();

  public abstract void FlushMessageBuffers();
}
