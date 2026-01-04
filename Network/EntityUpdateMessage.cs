// Decompiled with JetBrains decompiler
// Type: Magicka.Network.EntityUpdateMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.IO;

#nullable disable
namespace Magicka.Network;

public struct EntityUpdateMessage : ISendable
{
  public ushort UDPStamp;
  public ushort Handle;
  public EntityFeatures Features;
  public Vector3 Position;
  public float Direction;
  public Vector3 Velocity;
  public Quaternion Orientation;
  public float HitPoints;
  public bool GenericBool;
  public int GenericInt;
  public ushort GenericUShort;
  public float GenericFloat;
  public float WanderAngle;
  public bool EtherealState;
  public Character.SelfShieldType SelfShieldType;
  public float SelfShieldHealth;
  public StatusEffects StatusEffects;
  public unsafe fixed float StatusEffectMagnitude[9];
  public unsafe fixed float StatusEffectDPS[9];

  public PacketType PacketType => PacketType.EntityUpdate;

  public unsafe void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Handle);
    iWriter.Write(this.UDPStamp);
    iWriter.Write((ushort) this.Features);
    if ((this.Features & EntityFeatures.Position) != EntityFeatures.None)
    {
      iWriter.Write(this.Position.X);
      iWriter.Write(this.Position.Y);
      iWriter.Write(this.Position.Z);
    }
    if ((this.Features & EntityFeatures.Direction) != EntityFeatures.None)
    {
      float num = (MathHelper.WrapAngle(this.Direction) / 6.28318548f + 0.5f) * (float) ushort.MaxValue;
      iWriter.Write((ushort) num);
    }
    if ((this.Features & EntityFeatures.Velocity) != EntityFeatures.None)
    {
      iWriter.Write(new HalfSingle(this.Velocity.X).PackedValue);
      iWriter.Write(new HalfSingle(this.Velocity.Y).PackedValue);
      iWriter.Write(new HalfSingle(this.Velocity.Z).PackedValue);
    }
    if ((this.Features & EntityFeatures.Orientation) != EntityFeatures.None)
    {
      iWriter.Write(new HalfSingle(this.Orientation.X).PackedValue);
      iWriter.Write(new HalfSingle(this.Orientation.Y).PackedValue);
      iWriter.Write(new HalfSingle(this.Orientation.Z).PackedValue);
      iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
    }
    if ((this.Features & EntityFeatures.Character) != EntityFeatures.None)
      throw new NotImplementedException();
    if ((this.Features & EntityFeatures.Damageable) != EntityFeatures.None)
      iWriter.Write(this.HitPoints);
    if ((this.Features & EntityFeatures.StatusEffected) != EntityFeatures.None)
    {
      iWriter.Write((short) this.StatusEffects);
      fixed (float* numPtr1 = this.StatusEffectMagnitude)
        fixed (float* numPtr2 = this.StatusEffectDPS)
        {
          for (int iIndex = 0; iIndex < 9; ++iIndex)
          {
            StatusEffects statusEffects = StatusEffect.StatusFromIndex(iIndex);
            if ((this.StatusEffects & statusEffects) == statusEffects)
            {
              iWriter.Write(new HalfSingle(numPtr1[iIndex]).PackedValue);
              iWriter.Write(new HalfSingle(numPtr2[iIndex]).PackedValue);
            }
          }
        }
    }
    if ((this.Features & EntityFeatures.GenericBool) != EntityFeatures.None)
      iWriter.Write(this.GenericBool);
    if ((this.Features & EntityFeatures.GenericInt) != EntityFeatures.None)
      iWriter.Write(this.GenericInt);
    if ((this.Features & EntityFeatures.GenericFloat) != EntityFeatures.None)
      iWriter.Write(new HalfSingle(this.GenericFloat).PackedValue);
    if ((this.Features & EntityFeatures.WanderAngle) != EntityFeatures.None)
    {
      float num = (MathHelper.WrapAngle(this.WanderAngle) / 6.28318548f + 0.5f) * 256f;
      iWriter.Write((byte) num);
    }
    if ((this.Features & EntityFeatures.SelfShield) != EntityFeatures.None)
    {
      iWriter.Write((byte) this.SelfShieldType);
      iWriter.Write(new HalfSingle(this.SelfShieldHealth).PackedValue);
    }
    if ((this.Features & EntityFeatures.Etherealized) != EntityFeatures.None)
      iWriter.Write(this.EtherealState);
    if ((this.Features & EntityFeatures.GenericUShort) == EntityFeatures.None)
      return;
    iWriter.Write(this.GenericUShort);
  }

  public unsafe void Read(BinaryReader iReader)
  {
    HalfSingle halfSingle = new HalfSingle();
    this.Handle = iReader.ReadUInt16();
    this.UDPStamp = iReader.ReadUInt16();
    this.Features = (EntityFeatures) iReader.ReadUInt16();
    if ((this.Features & EntityFeatures.Position) != EntityFeatures.None)
    {
      this.Position.X = iReader.ReadSingle();
      this.Position.Y = iReader.ReadSingle();
      this.Position.Z = iReader.ReadSingle();
    }
    if ((this.Features & EntityFeatures.Direction) != EntityFeatures.None)
      this.Direction = ((float) iReader.ReadUInt16() / (float) ushort.MaxValue - 0.5f) * 6.28318548f;
    if ((this.Features & EntityFeatures.Velocity) != EntityFeatures.None)
    {
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Velocity.X = halfSingle.ToSingle();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Velocity.Y = halfSingle.ToSingle();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Velocity.Z = halfSingle.ToSingle();
    }
    if ((this.Features & EntityFeatures.Orientation) != EntityFeatures.None)
    {
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Orientation.X = halfSingle.ToSingle();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Orientation.Y = halfSingle.ToSingle();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Orientation.Z = halfSingle.ToSingle();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.Orientation.W = halfSingle.ToSingle();
    }
    if ((this.Features & EntityFeatures.Character) != EntityFeatures.None)
      throw new NotImplementedException();
    if ((this.Features & EntityFeatures.Damageable) != EntityFeatures.None)
      this.HitPoints = iReader.ReadSingle();
    if ((this.Features & EntityFeatures.StatusEffected) != EntityFeatures.None)
    {
      this.StatusEffects = (StatusEffects) iReader.ReadInt16();
      fixed (float* numPtr1 = this.StatusEffectMagnitude)
        fixed (float* numPtr2 = this.StatusEffectDPS)
        {
          for (int iIndex = 0; iIndex < 9; ++iIndex)
          {
            StatusEffects statusEffects = StatusEffect.StatusFromIndex(iIndex);
            if ((this.StatusEffects & statusEffects) == statusEffects)
            {
              halfSingle.PackedValue = iReader.ReadUInt16();
              numPtr1[iIndex] = halfSingle.ToSingle();
              halfSingle.PackedValue = iReader.ReadUInt16();
              numPtr2[iIndex] = halfSingle.ToSingle();
            }
            else
            {
              numPtr1[iIndex] = 0.0f;
              numPtr2[iIndex] = 0.0f;
            }
          }
        }
    }
    if ((this.Features & EntityFeatures.GenericBool) != EntityFeatures.None)
      this.GenericBool = iReader.ReadBoolean();
    if ((this.Features & EntityFeatures.GenericInt) != EntityFeatures.None)
      this.GenericInt = iReader.ReadInt32();
    if ((this.Features & EntityFeatures.GenericFloat) != EntityFeatures.None)
    {
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.GenericFloat = halfSingle.ToSingle();
    }
    if ((this.Features & EntityFeatures.WanderAngle) != EntityFeatures.None)
      this.WanderAngle = ((float) iReader.ReadByte() / 256f - 0.5f) * 6.28318548f;
    if ((this.Features & EntityFeatures.SelfShield) != EntityFeatures.None)
    {
      this.SelfShieldType = (Character.SelfShieldType) iReader.ReadByte();
      halfSingle.PackedValue = iReader.ReadUInt16();
      this.SelfShieldHealth = halfSingle.ToSingle();
    }
    if ((this.Features & EntityFeatures.Etherealized) != EntityFeatures.None)
      this.EtherealState = iReader.ReadBoolean();
    if ((this.Features & EntityFeatures.GenericUShort) == EntityFeatures.None)
      return;
    this.GenericUShort = iReader.ReadUInt16();
  }
}
