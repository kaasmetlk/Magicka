// Decompiled with JetBrains decompiler
// Type: Magicka.Network.EntityRemoveMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct EntityRemoveMessage : ISendable
{
  public ushort Handle;

  public PacketType PacketType => PacketType.EntityRemove;

  public void Write(BinaryWriter iWriter) => iWriter.Write(this.Handle);

  public void Read(BinaryReader iReader) => this.Handle = iReader.ReadUInt16();
}
