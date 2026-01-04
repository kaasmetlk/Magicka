// Decompiled with JetBrains decompiler
// Type: Magicka.Network.CharacterDieMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct CharacterDieMessage : ISendable
{
  public ushort Handle;
  public bool Overkill;
  public bool Drown;
  public ushort KillerHandle;

  public PacketType PacketType => PacketType.CharacterDie;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Handle);
    iWriter.Write(this.Overkill);
    iWriter.Write(this.Drown);
    iWriter.Write(this.KillerHandle);
  }

  public void Read(BinaryReader iReader)
  {
    this.Handle = iReader.ReadUInt16();
    this.Overkill = iReader.ReadBoolean();
    this.Drown = iReader.ReadBoolean();
    this.KillerHandle = iReader.ReadUInt16();
  }
}
