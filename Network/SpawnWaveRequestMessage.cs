// Decompiled with JetBrains decompiler
// Type: Magicka.Network.SpawnWaveRequestMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct SpawnWaveRequestMessage : ISendable
{
  public ushort OwnerHandle;
  public ushort AnimationHandle;
  public ushort ParentHandle;
  public Vector3 Position;
  public Vector3 Direction;
  public float Scale;
  public float Range;
  public Vector3 NextDir;
  public Quaternion NextRotation;
  public float Distance;
  public Spell Spell;
  public DamageCollection5 Damage;

  public PacketType PacketType => PacketType.SpawnWave | PacketType.Request;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.OwnerHandle);
    iWriter.Write(this.AnimationHandle);
    iWriter.Write(this.Position.X);
    iWriter.Write(this.Position.Y);
    iWriter.Write(this.Position.Z);
    iWriter.Write(new Normalized101010(this.Direction).PackedValue);
    iWriter.Write(new HalfSingle(this.Scale).PackedValue);
    iWriter.Write(new HalfSingle(this.Range).PackedValue);
    iWriter.Write(new HalfSingle(this.NextDir.X).PackedValue);
    iWriter.Write(new HalfSingle(this.NextDir.Y).PackedValue);
    iWriter.Write(new HalfSingle(this.NextDir.Z).PackedValue);
    iWriter.Write(new Normalized101010(this.NextRotation.X, this.NextRotation.Y, this.NextRotation.Z).PackedValue);
    iWriter.Write(new HalfSingle(this.NextRotation.W).PackedValue);
    iWriter.Write(new HalfSingle(this.Distance).PackedValue);
    iWriter.Write((ushort) this.Spell.Element);
    for (int iIndex = 0; iIndex < 11; ++iIndex)
    {
      Elements iElement = Defines.ElementFromIndex(iIndex);
      if ((iElement & this.Spell.Element) == iElement)
        iWriter.Write(new HalfSingle(this.Spell[iElement]).PackedValue);
    }
    iWriter.Write((short) this.Damage.A.AttackProperty);
    if (this.Damage.A.AttackProperty != (AttackProperties) 0)
    {
      iWriter.Write((ushort) this.Damage.A.Element);
      iWriter.Write(this.Damage.A.Amount);
      iWriter.Write(new HalfSingle(this.Damage.A.Magnitude).PackedValue);
    }
    iWriter.Write((short) this.Damage.B.AttackProperty);
    if (this.Damage.B.AttackProperty != (AttackProperties) 0)
    {
      iWriter.Write((ushort) this.Damage.B.Element);
      iWriter.Write(this.Damage.B.Amount);
      iWriter.Write(new HalfSingle(this.Damage.B.Magnitude).PackedValue);
    }
    iWriter.Write((short) this.Damage.C.AttackProperty);
    if (this.Damage.C.AttackProperty != (AttackProperties) 0)
    {
      iWriter.Write((ushort) this.Damage.C.Element);
      iWriter.Write(this.Damage.C.Amount);
      iWriter.Write(new HalfSingle(this.Damage.C.Magnitude).PackedValue);
    }
    iWriter.Write((short) this.Damage.D.AttackProperty);
    if (this.Damage.D.AttackProperty != (AttackProperties) 0)
    {
      iWriter.Write((ushort) this.Damage.D.Element);
      iWriter.Write(this.Damage.D.Amount);
      iWriter.Write(new HalfSingle(this.Damage.D.Magnitude).PackedValue);
    }
    iWriter.Write((short) this.Damage.E.AttackProperty);
    if (this.Damage.E.AttackProperty == (AttackProperties) 0)
      return;
    iWriter.Write((ushort) this.Damage.E.Element);
    iWriter.Write(this.Damage.E.Amount);
    iWriter.Write(new HalfSingle(this.Damage.E.Magnitude).PackedValue);
  }

  public void Read(BinaryReader iReader)
  {
    this.OwnerHandle = iReader.ReadUInt16();
    this.AnimationHandle = iReader.ReadUInt16();
    this.Position.X = iReader.ReadSingle();
    this.Position.Y = iReader.ReadSingle();
    this.Position.Z = iReader.ReadSingle();
    Normalized101010 normalized101010 = new Normalized101010();
    normalized101010.PackedValue = iReader.ReadUInt32();
    this.Direction = normalized101010.ToVector3();
    HalfSingle halfSingle = new HalfSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.Scale = halfSingle.ToSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.Range = halfSingle.ToSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.NextDir.X = halfSingle.ToSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.NextDir.Y = halfSingle.ToSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.NextDir.Z = halfSingle.ToSingle();
    normalized101010.PackedValue = iReader.ReadUInt32();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.NextRotation = new Quaternion(normalized101010.ToVector3(), halfSingle.ToSingle());
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.Distance = halfSingle.ToSingle();
    this.Spell.Element = (Elements) iReader.ReadUInt16();
    for (int iIndex = 0; (Elements) iIndex < this.Spell.Element; ++iIndex)
    {
      Elements iElement = Defines.ElementFromIndex(iIndex);
      if ((iElement & this.Spell.Element) == iElement)
      {
        halfSingle.PackedValue = iReader.ReadUInt16();
        this.Spell[iElement] = halfSingle.ToSingle();
      }
    }
    this.Damage.A.AttackProperty = (AttackProperties) iReader.ReadInt16();
    if (this.Damage.A.AttackProperty != (AttackProperties) 0)
    {
      this.Damage.A.Element = (Elements) iReader.ReadUInt16();
      this.Damage.A.Amount = iReader.ReadSingle();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Damage.A.Magnitude = halfSingle.ToSingle();
    }
    this.Damage.B.AttackProperty = (AttackProperties) iReader.ReadInt16();
    if (this.Damage.B.AttackProperty != (AttackProperties) 0)
    {
      this.Damage.B.Element = (Elements) iReader.ReadUInt16();
      this.Damage.B.Amount = iReader.ReadSingle();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Damage.B.Magnitude = halfSingle.ToSingle();
    }
    this.Damage.C.AttackProperty = (AttackProperties) iReader.ReadInt16();
    if (this.Damage.C.AttackProperty != (AttackProperties) 0)
    {
      this.Damage.C.Element = (Elements) iReader.ReadUInt16();
      this.Damage.C.Amount = iReader.ReadSingle();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Damage.C.Magnitude = halfSingle.ToSingle();
    }
    this.Damage.D.AttackProperty = (AttackProperties) iReader.ReadInt16();
    if (this.Damage.D.AttackProperty != (AttackProperties) 0)
    {
      this.Damage.D.Element = (Elements) iReader.ReadUInt16();
      this.Damage.D.Amount = iReader.ReadSingle();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Damage.D.Magnitude = halfSingle.ToSingle();
    }
    this.Damage.E.AttackProperty = (AttackProperties) iReader.ReadInt16();
    if (this.Damage.E.AttackProperty == (AttackProperties) 0)
      return;
    this.Damage.E.Element = (Elements) iReader.ReadUInt16();
    this.Damage.E.Amount = iReader.ReadSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.Damage.E.Magnitude = halfSingle.ToSingle();
  }
}
