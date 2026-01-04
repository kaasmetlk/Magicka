// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.BossUpdateMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Network;
using System;
using System.IO;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public struct BossUpdateMessage : ISendable
{
  public const int MAX_SIZE = 1024 /*0x0400*/;
  public BossEnum BossID;
  public ushort Type;
  public ushort Length;
  public long TimeStamp;
  public unsafe fixed byte Data[1024];

  public PacketType PacketType => PacketType.BossUpdate;

  public unsafe void Write(BinaryWriter iWriter)
  {
    iWriter.Write((byte) this.BossID);
    iWriter.Write(this.Type);
    iWriter.Write(this.Length);
    iWriter.Write(this.TimeStamp);
    fixed (byte* numPtr = this.Data)
    {
      for (int index = 0; index < (int) this.Length; ++index)
        iWriter.Write(numPtr[index]);
    }
  }

  public unsafe void Read(BinaryReader iReader)
  {
    this.BossID = (BossEnum) iReader.ReadByte();
    this.Type = iReader.ReadUInt16();
    this.Length = iReader.ReadUInt16();
    this.TimeStamp = iReader.ReadInt64();
    fixed (byte* numPtr = this.Data)
    {
      for (int index = 0; index < (int) this.Length; ++index)
        numPtr[index] = iReader.ReadByte();
    }
  }

  public static unsafe void ConvertTo(ref BossUpdateMessage iMsg, void* oValue)
  {
    byte* numPtr1 = (byte*) oValue;
    fixed (byte* numPtr2 = iMsg.Data)
    {
      for (int index = 0; index < (int) iMsg.Length; ++index)
        numPtr1[index] = numPtr2[index];
    }
  }

  public static unsafe void ConvertFrom<T>(ushort iType, void* iValue, out BossUpdateMessage oMsg) where T : struct
  {
    byte* numPtr1 = (byte*) iValue;
    oMsg.Type = iType;
    oMsg.TimeStamp = DateTime.Now.Ticks;
    oMsg.Length = (ushort) Marshal.SizeOf(typeof (T));
    oMsg.BossID = ~BossEnum.Generic;
    fixed (byte* numPtr2 = oMsg.Data)
    {
      for (int index = 0; index < (int) oMsg.Length; ++index)
        numPtr2[index] = numPtr1[index];
    }
  }
}
