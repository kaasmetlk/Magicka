// Decompiled with JetBrains decompiler
// Type: Magicka.Network.SpawnPortalMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct SpawnPortalMessage : ISendable
{
  public Vector3 Position;
  public ushort AnimationHandle;

  public PacketType PacketType => PacketType.SpawnPortal;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Position.X);
    iWriter.Write(this.Position.Y);
    iWriter.Write(this.Position.Z);
    iWriter.Write(this.AnimationHandle);
  }

  public void Read(BinaryReader iReader)
  {
    this.Position.X = iReader.ReadSingle();
    this.Position.Y = iReader.ReadSingle();
    this.Position.Z = iReader.ReadSingle();
    this.AnimationHandle = iReader.ReadUInt16();
  }
}
