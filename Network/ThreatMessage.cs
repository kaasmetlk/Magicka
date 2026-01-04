// Decompiled with JetBrains decompiler
// Type: Magicka.Network.ThreatMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct ThreatMessage : ISendable
{
  public bool Threat;

  public PacketType PacketType => PacketType.Threat;

  public void Write(BinaryWriter iWriter) => iWriter.Write(this.Threat);

  public void Read(BinaryReader iReader) => this.Threat = iReader.ReadBoolean();
}
