// Decompiled with JetBrains decompiler
// Type: Magicka.Network.SpawnPlayerMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct SpawnPlayerMessage : ISendable
{
  public ushort Handle;
  public byte Id;
  public bool MagickRevive;
  public Vector3 Position;
  public Vector3 Direction;

  public PacketType PacketType => PacketType.SpawnPlayer;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Handle);
    iWriter.Write(this.Id);
    iWriter.Write(this.MagickRevive);
    iWriter.Write(this.Position.X);
    iWriter.Write(this.Position.Y);
    iWriter.Write(this.Position.Z);
    iWriter.Write(new HalfSingle(this.Direction.X).PackedValue);
    iWriter.Write(new HalfSingle(this.Direction.Z).PackedValue);
  }

  public void Read(BinaryReader iReader)
  {
    this.Handle = iReader.ReadUInt16();
    this.Id = iReader.ReadByte();
    this.MagickRevive = iReader.ReadBoolean();
    this.Position.X = iReader.ReadSingle();
    this.Position.Y = iReader.ReadSingle();
    this.Position.Z = iReader.ReadSingle();
    HalfSingle halfSingle = new HalfSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.Direction.X = halfSingle.ToSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.Direction.Z = halfSingle.ToSingle();
  }
}
