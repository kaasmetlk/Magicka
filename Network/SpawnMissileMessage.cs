// Decompiled with JetBrains decompiler
// Type: Magicka.Network.SpawnMissileMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct SpawnMissileMessage : ISendable
{
  public SpawnMissileMessage.MissileType Type;
  public ushort Handle;
  public ushort Item;
  public ushort Owner;
  public ushort Target;
  public Vector3 Position;
  public Vector3 Velocity;
  public Spell Spell;
  public float Splash;
  public float Homing;
  public Vector3 AngularVelocity;
  public Vector3 Lever;

  public PacketType PacketType => PacketType.SpawnMissile;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write((byte) this.Type);
    iWriter.Write(this.Handle);
    iWriter.Write(this.Item);
    iWriter.Write(this.Owner);
    iWriter.Write(this.Target);
    iWriter.Write(this.Position.X);
    iWriter.Write(this.Position.Y);
    iWriter.Write(this.Position.Z);
    iWriter.Write(new HalfSingle(this.Velocity.X).PackedValue);
    iWriter.Write(new HalfSingle(this.Velocity.Y).PackedValue);
    iWriter.Write(new HalfSingle(this.Velocity.Z).PackedValue);
    iWriter.Write(new HalfSingle(this.Homing).PackedValue);
    switch (this.Type)
    {
      case SpawnMissileMessage.MissileType.Spell:
        iWriter.Write((ushort) this.Spell.Element);
        for (int iIndex = 0; iIndex < 11; ++iIndex)
        {
          Elements iElement = Defines.ElementFromIndex(iIndex);
          if ((iElement & this.Spell.Element) == iElement)
            iWriter.Write(new HalfSingle(this.Spell[iElement]).PackedValue);
        }
        iWriter.Write(new HalfSingle(this.Splash).PackedValue);
        break;
      case SpawnMissileMessage.MissileType.Item:
        break;
      case SpawnMissileMessage.MissileType.HolyGrenade:
        break;
      case SpawnMissileMessage.MissileType.Grenade:
        break;
      case SpawnMissileMessage.MissileType.FireFlask:
        break;
      case SpawnMissileMessage.MissileType.ProppMagick:
        iWriter.Write(new HalfSingle(this.AngularVelocity.X).PackedValue);
        iWriter.Write(new HalfSingle(this.AngularVelocity.Y).PackedValue);
        iWriter.Write(new HalfSingle(this.AngularVelocity.Z).PackedValue);
        iWriter.Write(new HalfSingle(this.Lever.X).PackedValue);
        iWriter.Write(new HalfSingle(this.Lever.Y).PackedValue);
        iWriter.Write(new HalfSingle(this.Lever.Z).PackedValue);
        break;
      case SpawnMissileMessage.MissileType.JudgementMissile:
        break;
      case SpawnMissileMessage.MissileType.GreaseLump:
        break;
      case SpawnMissileMessage.MissileType.PotionFlask:
        break;
      default:
        throw new Exception("Invalid missile type!");
    }
  }

  public void Read(BinaryReader iReader)
  {
    this.Type = (SpawnMissileMessage.MissileType) iReader.ReadByte();
    this.Handle = iReader.ReadUInt16();
    this.Item = iReader.ReadUInt16();
    this.Owner = iReader.ReadUInt16();
    this.Target = iReader.ReadUInt16();
    this.Position.X = iReader.ReadSingle();
    this.Position.Y = iReader.ReadSingle();
    this.Position.Z = iReader.ReadSingle();
    HalfSingle halfSingle1 = new HalfSingle();
    halfSingle1.PackedValue = iReader.ReadUInt16();
    this.Velocity.X = halfSingle1.ToSingle();
    halfSingle1.PackedValue = iReader.ReadUInt16();
    this.Velocity.Y = halfSingle1.ToSingle();
    halfSingle1.PackedValue = iReader.ReadUInt16();
    this.Velocity.Z = halfSingle1.ToSingle();
    halfSingle1.PackedValue = iReader.ReadUInt16();
    this.Homing = halfSingle1.ToSingle();
    switch (this.Type)
    {
      case SpawnMissileMessage.MissileType.Spell:
        this.Spell.Element = (Elements) iReader.ReadUInt16();
        for (int iIndex = 0; (Elements) iIndex < this.Spell.Element; ++iIndex)
        {
          Elements iElement = Defines.ElementFromIndex(iIndex);
          if ((iElement & this.Spell.Element) == iElement)
          {
            halfSingle1.PackedValue = iReader.ReadUInt16();
            this.Spell[iElement] = halfSingle1.ToSingle();
          }
        }
        halfSingle1.PackedValue = iReader.ReadUInt16();
        this.Splash = halfSingle1.ToSingle();
        break;
      case SpawnMissileMessage.MissileType.Item:
        break;
      case SpawnMissileMessage.MissileType.HolyGrenade:
        break;
      case SpawnMissileMessage.MissileType.Grenade:
        break;
      case SpawnMissileMessage.MissileType.FireFlask:
        break;
      case SpawnMissileMessage.MissileType.ProppMagick:
        HalfSingle halfSingle2 = new HalfSingle();
        halfSingle2.PackedValue = iReader.ReadUInt16();
        this.AngularVelocity.X = halfSingle2.ToSingle();
        halfSingle2.PackedValue = iReader.ReadUInt16();
        this.AngularVelocity.Y = halfSingle2.ToSingle();
        halfSingle2.PackedValue = iReader.ReadUInt16();
        this.AngularVelocity.Z = halfSingle2.ToSingle();
        halfSingle2.PackedValue = iReader.ReadUInt16();
        this.Lever.X = halfSingle2.ToSingle();
        halfSingle2.PackedValue = iReader.ReadUInt16();
        this.Lever.Y = halfSingle2.ToSingle();
        halfSingle2.PackedValue = iReader.ReadUInt16();
        this.Lever.Z = halfSingle2.ToSingle();
        break;
      case SpawnMissileMessage.MissileType.JudgementMissile:
        break;
      case SpawnMissileMessage.MissileType.GreaseLump:
        break;
      case SpawnMissileMessage.MissileType.PotionFlask:
        break;
      default:
        throw new Exception("Invalid missile type!");
    }
  }

  public enum MissileType : byte
  {
    Invalid,
    Spell,
    Item,
    HolyGrenade,
    Grenade,
    CthulhuMissile,
    FireFlask,
    ProppMagick,
    JudgementMissile,
    GreaseLump,
    PotionFlask,
  }
}
