// Decompiled with JetBrains decompiler
// Type: Magicka.Network.RulesetMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Levels;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.IO;

#nullable disable
namespace Magicka.Network;

public struct RulesetMessage : ISendable
{
  public const byte BYTE_ARRAY_MAX_SIZE = 16 /*0x10*/;
  public const byte SHROT_ARRAY_MAX_SIZE = 6;
  public Rulesets Type;
  public float PackedFloat;
  public byte Byte01;
  public byte Byte02;
  public byte Byte03;
  public float Float01;
  public float Float02;
  public ushort UShort01;
  public int Integer01;
  public byte NrOfByteItems;
  public unsafe fixed byte Byte[16];
  public byte NrOfShortItems;
  public unsafe fixed short Scores[6];

  public PacketType PacketType => PacketType.RulesetUpdate;

  public unsafe void Write(BinaryWriter iWriter)
  {
    iWriter.Write((byte) this.Type);
    switch (this.Type)
    {
      case Rulesets.Survival:
        iWriter.Write(new HalfSingle(this.PackedFloat).PackedValue);
        iWriter.Write(this.Float01);
        iWriter.Write(this.Float02);
        iWriter.Write(this.Byte01);
        iWriter.Write(this.Byte02);
        iWriter.Write(this.Byte03);
        break;
      case Rulesets.TimedObjective:
        iWriter.Write(this.NrOfByteItems);
        fixed (byte* numPtr = this.Byte)
        {
          for (int index = 0; index < (int) this.NrOfByteItems; ++index)
            iWriter.Write(numPtr[index]);
        }
        break;
      case Rulesets.DeathMatch:
      case Rulesets.Brawl:
      case Rulesets.Pyrite:
      case Rulesets.Kreitor:
      case Rulesets.King:
        iWriter.Write(this.Byte01);
        switch (this.Byte01)
        {
          case 0:
          case 2:
            iWriter.Write(this.Float01);
            return;
          case 1:
            return;
          case 3:
            iWriter.Write(this.Byte02);
            iWriter.Write(this.Integer01);
            iWriter.Write(this.UShort01);
            return;
          case 4:
            iWriter.Write(this.Byte02);
            iWriter.Write(this.NrOfShortItems);
            fixed (short* numPtr = this.Scores)
            {
              for (int index = 0; index < (int) this.NrOfShortItems; ++index)
                iWriter.Write(numPtr[index]);
            }
            return;
          default:
            return;
        }
    }
  }

  public unsafe void Read(BinaryReader iReader)
  {
    this.Type = (Rulesets) iReader.ReadByte();
    switch (this.Type)
    {
      case Rulesets.Survival:
        this.PackedFloat = new HalfSingle()
        {
          PackedValue = iReader.ReadUInt16()
        }.ToSingle();
        this.Float01 = iReader.ReadSingle();
        this.Float02 = iReader.ReadSingle();
        this.Byte01 = iReader.ReadByte();
        this.Byte02 = iReader.ReadByte();
        this.Byte03 = iReader.ReadByte();
        break;
      case Rulesets.TimedObjective:
        this.NrOfByteItems = iReader.ReadByte();
        fixed (byte* numPtr = this.Byte)
        {
          for (int index = 0; index < (int) this.NrOfByteItems; ++index)
            numPtr[index] = iReader.ReadByte();
        }
        break;
      case Rulesets.DeathMatch:
      case Rulesets.Brawl:
      case Rulesets.Pyrite:
      case Rulesets.Kreitor:
      case Rulesets.King:
        this.Byte01 = iReader.ReadByte();
        switch (this.Byte01)
        {
          case 0:
          case 2:
            this.Float01 = iReader.ReadSingle();
            return;
          case 1:
            return;
          case 3:
            this.Byte02 = iReader.ReadByte();
            this.Integer01 = iReader.ReadInt32();
            this.UShort01 = iReader.ReadUInt16();
            return;
          case 4:
            this.Byte02 = iReader.ReadByte();
            this.NrOfShortItems = iReader.ReadByte();
            fixed (short* numPtr = this.Scores)
            {
              for (int index = 0; index < (int) this.NrOfShortItems; ++index)
                numPtr[index] = iReader.ReadInt16();
            }
            return;
          default:
            return;
        }
    }
  }
}
