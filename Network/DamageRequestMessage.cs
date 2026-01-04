// Decompiled with JetBrains decompiler
// Type: Magicka.Network.DamageRequestMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct DamageRequestMessage : ISendable
{
  public DamageCollection5 Damage;
  public ushort AttackerHandle;
  public ushort TargetHandle;
  public Vector3 RelativeAttackPosition;
  public double TimeStamp;

  public PacketType PacketType => PacketType.Damage | PacketType.Request;

  public unsafe void Write(BinaryWriter iWriter)
  {
    fixed (Magicka.GameLogic.Damage* damagePtr = &this.Damage.A)
    {
      for (int index = 0; index < 5; ++index)
      {
        iWriter.Write((short) damagePtr[index].AttackProperty);
        if (damagePtr[index].AttackProperty != (AttackProperties) 0)
        {
          iWriter.Write((ushort) damagePtr[index].Element);
          iWriter.Write(damagePtr[index].Amount);
          iWriter.Write(new HalfSingle(damagePtr[index].Magnitude).PackedValue);
        }
      }
    }
    iWriter.Write(this.AttackerHandle);
    iWriter.Write(this.TimeStamp);
    iWriter.Write(this.TargetHandle);
    iWriter.Write(new HalfSingle(this.RelativeAttackPosition.X).PackedValue);
    iWriter.Write(new HalfSingle(this.RelativeAttackPosition.Y).PackedValue);
    iWriter.Write(new HalfSingle(this.RelativeAttackPosition.Z).PackedValue);
  }

  public unsafe void Read(BinaryReader iReader)
  {
    HalfSingle halfSingle = new HalfSingle();
    fixed (Magicka.GameLogic.Damage* damagePtr = &this.Damage.A)
    {
      for (int index = 0; index < 5; ++index)
      {
        damagePtr[index].AttackProperty = (AttackProperties) iReader.ReadInt16();
        if (damagePtr[index].AttackProperty != (AttackProperties) 0)
        {
          damagePtr[index].Element = (Elements) iReader.ReadUInt16();
          damagePtr[index].Amount = iReader.ReadSingle();
          halfSingle.PackedValue = iReader.ReadUInt16();
          damagePtr[index].Magnitude = halfSingle.ToSingle();
        }
      }
    }
    this.AttackerHandle = iReader.ReadUInt16();
    this.TimeStamp = iReader.ReadDouble();
    this.TargetHandle = iReader.ReadUInt16();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.RelativeAttackPosition.X = halfSingle.ToSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.RelativeAttackPosition.Y = halfSingle.ToSingle();
    halfSingle.PackedValue = iReader.ReadUInt16();
    this.RelativeAttackPosition.Z = halfSingle.ToSingle();
  }
}
