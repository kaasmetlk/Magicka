// Decompiled with JetBrains decompiler
// Type: Magicka.Network.SpawnVortexMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct SpawnVortexMessage : ISendable
{
  public ushort Handle;
  public ushort OwnerHandle;
  public Vector3 Position;

  public PacketType PacketType => PacketType.SpawnVortex;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Handle);
    iWriter.Write(this.OwnerHandle);
    iWriter.Write(this.Position.X);
    iWriter.Write(this.Position.Y);
    iWriter.Write(this.Position.Z);
  }

  public void Read(BinaryReader iReader)
  {
    this.Handle = iReader.ReadUInt16();
    this.OwnerHandle = iReader.ReadUInt16();
    this.Position.X = iReader.ReadSingle();
    this.Position.Y = iReader.ReadSingle();
    this.Position.Z = iReader.ReadSingle();
  }
}
