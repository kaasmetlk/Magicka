// Decompiled with JetBrains decompiler
// Type: Magicka.Network.SpawnShieldRequestMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct SpawnShieldRequestMessage : ISendable
{
  public ushort OwnerHandle;
  public Vector3 Position;
  public Vector3 Direction;
  public float Radius;
  public float HitPoints;
  public ShieldType ShieldType;

  public PacketType PacketType => PacketType.SpawnShield | PacketType.Request;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.OwnerHandle);
    iWriter.Write(this.Position.X);
    iWriter.Write(this.Position.Y);
    iWriter.Write(this.Position.Z);
    iWriter.Write(new Normalized101010(this.Direction).PackedValue);
    iWriter.Write(new HalfSingle(this.Radius).PackedValue);
    iWriter.Write(new HalfSingle(this.HitPoints).PackedValue);
    iWriter.Write((byte) this.ShieldType);
  }

  public void Read(BinaryReader iReader)
  {
    this.OwnerHandle = iReader.ReadUInt16();
    this.Position.X = iReader.ReadSingle();
    this.Position.Y = iReader.ReadSingle();
    this.Position.Z = iReader.ReadSingle();
    HalfSingle halfSingle = new HalfSingle();
    this.Direction = new Normalized101010()
    {
      PackedValue = iReader.ReadUInt32()
    }.ToVector3();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.Radius = halfSingle.ToSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.HitPoints = halfSingle.ToSingle();
    this.ShieldType = (ShieldType) iReader.ReadByte();
  }
}
