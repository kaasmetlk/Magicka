// Decompiled with JetBrains decompiler
// Type: Magicka.Network.CharacterActionMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.Network;

[StructLayout(LayoutKind.Explicit)]
internal struct CharacterActionMessage : ISendable
{
  [FieldOffset(0)]
  public ActionType Action;
  [FieldOffset(4)]
  public ushort Handle;
  [FieldOffset(6)]
  public ushort TargetHandle;
  [FieldOffset(8)]
  public double TimeStamp;
  [FieldOffset(16 /*0x10*/)]
  public int Param0I;
  [FieldOffset(16 /*0x10*/)]
  public float Param0F;
  [FieldOffset(20)]
  public int Param1I;
  [FieldOffset(20)]
  public float Param1F;
  [FieldOffset(24)]
  public int Param2I;
  [FieldOffset(24)]
  public float Param2F;
  [FieldOffset(28)]
  public int Param3I;
  [FieldOffset(28)]
  public float Param3F;
  [FieldOffset(32 /*0x20*/)]
  public int Param4I;
  [FieldOffset(32 /*0x20*/)]
  public float Param4F;

  public PacketType PacketType => PacketType.CharacterAction;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write((byte) this.Action);
    iWriter.Write(this.Handle);
    iWriter.Write(this.TimeStamp);
    byte num = (byte) ((this.TargetHandle == (ushort) 0 ? 0 : 1) << 1 | (this.Param0I == 0 ? 0 : 1) << 2 | (this.Param1I == 0 ? 0 : 1) << 3 | (this.Param2I == 0 ? 0 : 1) << 4 | (this.Param3I == 0 ? 0 : 1) << 5 | (this.Param4I == 0 ? 0 : 1) << 6);
    iWriter.Write(num);
    if (((int) num & 2) == 2)
      iWriter.Write(this.TargetHandle);
    if (((int) num & 4) == 4)
      iWriter.Write(this.Param0I);
    if (((int) num & 8) == 8)
      iWriter.Write(this.Param1I);
    if (((int) num & 16 /*0x10*/) == 16 /*0x10*/)
      iWriter.Write(this.Param2I);
    if (((int) num & 32 /*0x20*/) == 32 /*0x20*/)
      iWriter.Write(this.Param3I);
    if (((int) num & 64 /*0x40*/) != 64 /*0x40*/)
      return;
    iWriter.Write(this.Param4I);
  }

  public void Read(BinaryReader iReader)
  {
    this.Action = (ActionType) iReader.ReadByte();
    this.Handle = iReader.ReadUInt16();
    this.TimeStamp = iReader.ReadDouble();
    byte num = iReader.ReadByte();
    if (((int) num & 2) == 2)
      this.TargetHandle = iReader.ReadUInt16();
    if (((int) num & 4) == 4)
      this.Param0I = iReader.ReadInt32();
    if (((int) num & 8) == 8)
      this.Param1I = iReader.ReadInt32();
    if (((int) num & 16 /*0x10*/) == 16 /*0x10*/)
      this.Param2I = iReader.ReadInt32();
    if (((int) num & 32 /*0x20*/) == 32 /*0x20*/)
      this.Param3I = iReader.ReadInt32();
    if (((int) num & 64 /*0x40*/) != 64 /*0x40*/)
      return;
    this.Param4I = iReader.ReadInt32();
  }
}
