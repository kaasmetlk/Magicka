// Decompiled with JetBrains decompiler
// Type: Magicka.Network.TriggerActionMessage
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

internal struct TriggerActionMessage : ISendable
{
  public TriggerActionType ActionType;
  public ushort Handle;
  public int Template;
  public int Scene;
  public int Arg;
  public int Id;
  public Vector3 Position;
  public Vector3 Direction;
  public Quaternion Orientation;
  public int Point0;
  public int Point1;
  public int Point2;
  public int Point3;
  public int Target0;
  public int Target1;
  public int Target2;
  public int Target3;
  public bool Bool0;
  public bool Bool1;
  public bool Bool2;
  public float Time;
  public Vector3 Color;
  public double TimeStamp;
  public Spell Spell;
  public float Splash;

  public PacketType PacketType => PacketType.TriggerAction;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write((byte) this.ActionType);
    switch (this.ActionType)
    {
      case TriggerActionType.TriggerExecute:
        iWriter.Write(this.Scene);
        iWriter.Write(this.Id);
        iWriter.Write(this.Handle);
        iWriter.Write(this.Arg);
        break;
      case TriggerActionType.SpawnNPC:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Template);
        iWriter.Write(this.Id);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(new Normalized101010(this.Direction).PackedValue);
        iWriter.Write(this.Point0);
        iWriter.Write(this.Point1);
        iWriter.Write(this.Point2);
        iWriter.Write(this.Point3);
        iWriter.Write(this.Bool0);
        iWriter.Write(this.Arg);
        iWriter.Write(this.Scene);
        iWriter.Write(new HalfSingle(this.Color.X).PackedValue);
        iWriter.Write(new HalfSingle(this.Color.Y).PackedValue);
        iWriter.Write(new HalfSingle(this.Color.Z).PackedValue);
        break;
      case TriggerActionType.SpawnElemental:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Time);
        iWriter.Write(this.Id);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(new Normalized101010(this.Direction).PackedValue);
        break;
      case TriggerActionType.SpawnLuggage:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Template);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(new Normalized101010(this.Direction).PackedValue);
        iWriter.Write(this.Point0);
        break;
      case TriggerActionType.SpawnFood:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(new Normalized101010(this.Orientation.X, this.Orientation.Y, this.Orientation.Z).PackedValue);
        iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
        break;
      case TriggerActionType.SpawnItem:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Template);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(this.Time);
        iWriter.Write(new Normalized101010(this.Direction).PackedValue);
        iWriter.Write(new Normalized101010(this.Orientation.X, this.Orientation.Y, this.Orientation.Z).PackedValue);
        iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
        iWriter.Write(this.Point0);
        iWriter.Write(this.Point1);
        iWriter.Write(this.Bool0);
        iWriter.Write(this.Bool1);
        break;
      case TriggerActionType.SpawnMagick:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Template);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(this.Time);
        iWriter.Write(new Normalized101010(this.Direction).PackedValue);
        iWriter.Write(new Normalized101010(this.Orientation.X, this.Orientation.Y, this.Orientation.Z).PackedValue);
        iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
        iWriter.Write(this.Point0);
        iWriter.Write(this.Bool0);
        break;
      case TriggerActionType.ChangeScene:
        iWriter.Write(this.Scene);
        iWriter.Write(this.Point0);
        iWriter.Write(this.Point1);
        iWriter.Write(this.Point2);
        iWriter.Write(this.Point3);
        iWriter.Write(this.Bool0);
        iWriter.Write(this.Bool1);
        iWriter.Write(this.Template);
        iWriter.Write(this.Time);
        break;
      case TriggerActionType.EnterScene:
        iWriter.Write(this.Scene);
        iWriter.Write(this.Point0);
        iWriter.Write(this.Point1);
        iWriter.Write(this.Point2);
        iWriter.Write(this.Point3);
        iWriter.Write(this.Target0);
        iWriter.Write(this.Target1);
        iWriter.Write(this.Target2);
        iWriter.Write(this.Target3);
        iWriter.Write(this.Arg);
        iWriter.Write(this.Bool0);
        iWriter.Write(this.Bool1);
        iWriter.Write(this.Bool2);
        iWriter.Write(this.Template);
        iWriter.Write(this.Time);
        break;
      case TriggerActionType.ThunderBolt:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Id);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        break;
      case TriggerActionType.LightningBolt:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Id);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write((ushort) this.Spell.Element);
        for (int iIndex = 0; iIndex < 11; ++iIndex)
        {
          Elements iElement = Defines.ElementFromIndex(iIndex);
          if ((iElement & this.Spell.Element) == iElement)
            iWriter.Write(new HalfSingle(this.Spell[iElement]).PackedValue);
        }
        iWriter.Write(new HalfSingle(this.Splash).PackedValue);
        break;
      case TriggerActionType.SpawnGrease:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Arg);
        iWriter.Write(this.Id);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(new Normalized101010(this.Direction).PackedValue);
        break;
      case TriggerActionType.Nullify:
        iWriter.Write(this.Bool0);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        break;
      case TriggerActionType.SpawnTornado:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Id);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(new Normalized101010(this.Orientation.X, this.Orientation.Y, this.Orientation.Z).PackedValue);
        iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
        break;
      case TriggerActionType.NapalmStrike:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Arg);
        iWriter.Write(this.Id);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(new Normalized101010(this.Direction).PackedValue);
        iWriter.Write(this.TimeStamp);
        break;
      case TriggerActionType.EarthQuake:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        iWriter.Write(this.Time);
        break;
      case TriggerActionType.Charm:
        iWriter.Write(this.Handle);
        iWriter.Write((ushort) this.Id);
        iWriter.Write(this.Time);
        iWriter.Write(this.Arg);
        break;
      case TriggerActionType.Starfall:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Position.X);
        iWriter.Write(this.Position.Y);
        iWriter.Write(this.Position.Z);
        break;
      case TriggerActionType.OtherworldlyDischarge:
      case TriggerActionType.StarGaze:
        iWriter.Write(this.Handle);
        iWriter.Write((ushort) this.Arg);
        break;
      case TriggerActionType.OtherworldlyBoltDestroyed:
        iWriter.Write(this.Handle);
        iWriter.Write((ushort) this.Arg);
        iWriter.Write(this.Bool0);
        iWriter.Write(this.Bool1);
        break;
      case TriggerActionType.Confuse:
        iWriter.Write(this.Handle);
        iWriter.Write(this.Bool0);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  public void Read(BinaryReader iReader)
  {
    this.ActionType = (TriggerActionType) iReader.ReadByte();
    switch (this.ActionType)
    {
      case TriggerActionType.TriggerExecute:
        this.Scene = iReader.ReadInt32();
        this.Id = iReader.ReadInt32();
        this.Handle = iReader.ReadUInt16();
        this.Arg = iReader.ReadInt32();
        break;
      case TriggerActionType.SpawnNPC:
        this.Handle = iReader.ReadUInt16();
        this.Template = iReader.ReadInt32();
        this.Id = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Direction = new Normalized101010()
        {
          PackedValue = iReader.ReadUInt32()
        }.ToVector3();
        this.Point0 = iReader.ReadInt32();
        this.Point1 = iReader.ReadInt32();
        this.Point2 = iReader.ReadInt32();
        this.Point3 = iReader.ReadInt32();
        this.Bool0 = iReader.ReadBoolean();
        this.Arg = iReader.ReadInt32();
        this.Scene = iReader.ReadInt32();
        HalfSingle halfSingle1 = new HalfSingle();
        halfSingle1.PackedValue = iReader.ReadUInt16();
        this.Color.X = halfSingle1.ToSingle();
        halfSingle1.PackedValue = iReader.ReadUInt16();
        this.Color.Y = halfSingle1.ToSingle();
        halfSingle1.PackedValue = iReader.ReadUInt16();
        this.Color.Z = halfSingle1.ToSingle();
        break;
      case TriggerActionType.SpawnElemental:
        this.Handle = iReader.ReadUInt16();
        this.Time = iReader.ReadSingle();
        this.Id = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Direction = new Normalized101010()
        {
          PackedValue = iReader.ReadUInt32()
        }.ToVector3();
        break;
      case TriggerActionType.SpawnLuggage:
        this.Handle = iReader.ReadUInt16();
        this.Template = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Direction = new Normalized101010()
        {
          PackedValue = iReader.ReadUInt32()
        }.ToVector3();
        this.Point0 = iReader.ReadInt32();
        break;
      case TriggerActionType.SpawnFood:
        this.Handle = iReader.ReadUInt16();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Orientation = new Quaternion(new Normalized101010()
        {
          PackedValue = iReader.ReadUInt32()
        }.ToVector3(), new HalfSingle()
        {
          PackedValue = iReader.ReadUInt16()
        }.ToSingle());
        break;
      case TriggerActionType.SpawnItem:
        this.Handle = iReader.ReadUInt16();
        this.Template = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Time = iReader.ReadSingle();
        Normalized101010 normalized101010_1 = new Normalized101010();
        normalized101010_1.PackedValue = iReader.ReadUInt32();
        this.Direction = normalized101010_1.ToVector3();
        normalized101010_1.PackedValue = iReader.ReadUInt32();
        this.Orientation = new Quaternion(normalized101010_1.ToVector3(), new HalfSingle()
        {
          PackedValue = iReader.ReadUInt16()
        }.ToSingle());
        this.Point0 = iReader.ReadInt32();
        this.Point1 = iReader.ReadInt32();
        this.Bool0 = iReader.ReadBoolean();
        this.Bool1 = iReader.ReadBoolean();
        break;
      case TriggerActionType.SpawnMagick:
        this.Handle = iReader.ReadUInt16();
        this.Template = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Time = iReader.ReadSingle();
        Normalized101010 normalized101010_2 = new Normalized101010();
        normalized101010_2.PackedValue = iReader.ReadUInt32();
        this.Direction = normalized101010_2.ToVector3();
        normalized101010_2.PackedValue = iReader.ReadUInt32();
        this.Orientation = new Quaternion(normalized101010_2.ToVector3(), new HalfSingle()
        {
          PackedValue = iReader.ReadUInt16()
        }.ToSingle());
        this.Point0 = iReader.ReadInt32();
        this.Bool0 = iReader.ReadBoolean();
        break;
      case TriggerActionType.ChangeScene:
        this.Scene = iReader.ReadInt32();
        this.Point0 = iReader.ReadInt32();
        this.Point1 = iReader.ReadInt32();
        this.Point2 = iReader.ReadInt32();
        this.Point3 = iReader.ReadInt32();
        this.Bool0 = iReader.ReadBoolean();
        this.Bool1 = iReader.ReadBoolean();
        this.Template = iReader.ReadInt32();
        this.Time = iReader.ReadSingle();
        break;
      case TriggerActionType.EnterScene:
        this.Scene = iReader.ReadInt32();
        this.Point0 = iReader.ReadInt32();
        this.Point1 = iReader.ReadInt32();
        this.Point2 = iReader.ReadInt32();
        this.Point3 = iReader.ReadInt32();
        this.Target0 = iReader.ReadInt32();
        this.Target1 = iReader.ReadInt32();
        this.Target2 = iReader.ReadInt32();
        this.Target3 = iReader.ReadInt32();
        this.Arg = iReader.ReadInt32();
        this.Bool0 = iReader.ReadBoolean();
        this.Bool1 = iReader.ReadBoolean();
        this.Bool2 = iReader.ReadBoolean();
        this.Template = iReader.ReadInt32();
        this.Time = iReader.ReadSingle();
        break;
      case TriggerActionType.ThunderBolt:
        this.Handle = iReader.ReadUInt16();
        this.Id = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        break;
      case TriggerActionType.LightningBolt:
        HalfSingle halfSingle2 = new HalfSingle();
        this.Handle = iReader.ReadUInt16();
        this.Id = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Spell.Element = (Elements) iReader.ReadUInt16();
        for (int iIndex = 0; (Elements) iIndex < this.Spell.Element; ++iIndex)
        {
          Elements iElement = Defines.ElementFromIndex(iIndex);
          if ((iElement & this.Spell.Element) == iElement)
          {
            halfSingle2.PackedValue = iReader.ReadUInt16();
            this.Spell[iElement] = halfSingle2.ToSingle();
          }
        }
        halfSingle2.PackedValue = iReader.ReadUInt16();
        this.Splash = halfSingle2.ToSingle();
        break;
      case TriggerActionType.SpawnGrease:
        this.Handle = iReader.ReadUInt16();
        this.Arg = iReader.ReadInt32();
        this.Id = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Direction = new Normalized101010()
        {
          PackedValue = iReader.ReadUInt32()
        }.ToVector3();
        break;
      case TriggerActionType.Nullify:
        this.Bool0 = iReader.ReadBoolean();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        break;
      case TriggerActionType.SpawnTornado:
        this.Handle = iReader.ReadUInt16();
        this.Id = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Orientation = new Quaternion(new Normalized101010()
        {
          PackedValue = iReader.ReadUInt32()
        }.ToVector3(), new HalfSingle()
        {
          PackedValue = iReader.ReadUInt16()
        }.ToSingle());
        break;
      case TriggerActionType.NapalmStrike:
        this.Handle = iReader.ReadUInt16();
        this.Arg = iReader.ReadInt32();
        this.Id = iReader.ReadInt32();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Direction = new Normalized101010()
        {
          PackedValue = iReader.ReadUInt32()
        }.ToVector3();
        this.TimeStamp = iReader.ReadDouble();
        break;
      case TriggerActionType.EarthQuake:
        this.Handle = iReader.ReadUInt16();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Time = iReader.ReadSingle();
        break;
      case TriggerActionType.Charm:
        this.Handle = iReader.ReadUInt16();
        this.Id = (int) iReader.ReadUInt16();
        this.Time = iReader.ReadSingle();
        this.Arg = iReader.ReadInt32();
        break;
      case TriggerActionType.Starfall:
        this.Handle = iReader.ReadUInt16();
        this.Position.X = iReader.ReadSingle();
        this.Position.Y = iReader.ReadSingle();
        this.Position.Z = iReader.ReadSingle();
        this.Time = iReader.ReadSingle();
        break;
      case TriggerActionType.OtherworldlyDischarge:
      case TriggerActionType.StarGaze:
        this.Handle = iReader.ReadUInt16();
        this.Arg = (int) iReader.ReadUInt16();
        break;
      case TriggerActionType.OtherworldlyBoltDestroyed:
        this.Handle = iReader.ReadUInt16();
        this.Arg = (int) iReader.ReadUInt16();
        this.Bool0 = iReader.ReadBoolean();
        this.Bool1 = iReader.ReadBoolean();
        break;
      case TriggerActionType.Confuse:
        this.Handle = iReader.ReadUInt16();
        this.Bool0 = iReader.ReadBoolean();
        break;
      default:
        throw new NotImplementedException();
    }
  }
}
