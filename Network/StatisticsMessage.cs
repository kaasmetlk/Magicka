// Decompiled with JetBrains decompiler
// Type: Magicka.Network.StatisticsMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct StatisticsMessage : ISendable
{
  public ushort AttackerHandle;
  public ushort TargetHandle;
  public double TimeStamp;
  public DamageResult Result;
  public Damage Damage;
  public byte Multiplier;

  public PacketType PacketType => PacketType.StatisticsUpdate;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.AttackerHandle);
    iWriter.Write(this.TargetHandle);
    iWriter.Write(this.TimeStamp);
    iWriter.Write((int) this.Result);
    iWriter.Write((short) this.Damage.AttackProperty);
    iWriter.Write((ushort) this.Damage.Element);
    iWriter.Write(this.Damage.Amount);
    iWriter.Write(new HalfSingle(this.Damage.Magnitude).PackedValue);
    iWriter.Write(this.Multiplier);
  }

  public void Read(BinaryReader iReader)
  {
    this.AttackerHandle = iReader.ReadUInt16();
    this.TargetHandle = iReader.ReadUInt16();
    this.TimeStamp = (double) iReader.ReadInt64();
    this.Result = (DamageResult) iReader.ReadInt32();
    this.Damage = new Damage();
    this.Damage.AttackProperty = (AttackProperties) iReader.ReadInt16();
    this.Damage.Element = (Elements) iReader.ReadUInt16();
    this.Damage.Amount = iReader.ReadSingle();
    this.Damage.Magnitude = new HalfSingle()
    {
      PackedValue = iReader.ReadUInt16()
    }.ToSingle();
    this.Multiplier = iReader.ReadByte();
  }
}
