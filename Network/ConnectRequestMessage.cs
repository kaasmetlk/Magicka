// Decompiled with JetBrains decompiler
// Type: Magicka.Network.ConnectRequestMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct ConnectRequestMessage : ISendable
{
  private byte mDummy;

  public PacketType PacketType => PacketType.Connect | PacketType.Request;

  public void Write(BinaryWriter iWriter)
  {
  }

  public void Read(BinaryReader iReader)
  {
  }
}
