// Decompiled with JetBrains decompiler
// Type: Magicka.Network.MenuSelectMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.Network;

[StructLayout(LayoutKind.Explicit)]
internal struct MenuSelectMessage : ISendable
{
  [FieldOffset(0)]
  public int Option;
  [FieldOffset(4)]
  public int Param0I;
  [FieldOffset(4)]
  public float Param0F;
  [FieldOffset(8)]
  public int Param1I;
  [FieldOffset(8)]
  public float Param1F;
  [FieldOffset(12)]
  public int Param2I;
  [FieldOffset(12)]
  public float Param2F;
  [FieldOffset(16 /*0x10*/)]
  public int Param3I;
  [FieldOffset(16 /*0x10*/)]
  public float Param3F;
  [FieldOffset(20)]
  public int Param4I;
  [FieldOffset(20)]
  public float Param4F;
  [FieldOffset(24)]
  public int Param5I;
  [FieldOffset(24)]
  public float Param5F;
  [FieldOffset(28)]
  public int Param6I;
  [FieldOffset(28)]
  public float Param6F;
  [FieldOffset(32 /*0x20*/)]
  public int Param7I;
  [FieldOffset(32 /*0x20*/)]
  public float Param7F;
  [FieldOffset(36)]
  public MenuSelectMessage.MenuType IntendedMenu;

  public PacketType PacketType => PacketType.MenuSelection;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Option);
    iWriter.Write((byte) this.IntendedMenu);
    iWriter.Write(this.Param0I);
    iWriter.Write(this.Param1I);
    iWriter.Write(this.Param2I);
    iWriter.Write(this.Param3I);
    iWriter.Write(this.Param4I);
    iWriter.Write(this.Param5I);
    iWriter.Write(this.Param6I);
    iWriter.Write(this.Param7I);
  }

  public void Read(BinaryReader iReader)
  {
    this.Option = iReader.ReadInt32();
    this.IntendedMenu = (MenuSelectMessage.MenuType) iReader.ReadByte();
    this.Param0I = iReader.ReadInt32();
    this.Param1I = iReader.ReadInt32();
    this.Param2I = iReader.ReadInt32();
    this.Param3I = iReader.ReadInt32();
    this.Param4I = iReader.ReadInt32();
    this.Param5I = iReader.ReadInt32();
    this.Param6I = iReader.ReadInt32();
    this.Param7I = iReader.ReadInt32();
  }

  public enum MenuType : byte
  {
    CharacterSelect,
    Statistics,
  }
}
