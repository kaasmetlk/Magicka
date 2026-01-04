// Decompiled with JetBrains decompiler
// Type: Magicka.Network.AuthenticateRequestMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct AuthenticateRequestMessage : ISendable
{
  public AuthenticationToken Token;
  public byte NrOfPlayers;
  public ulong Version;
  public string Password;

  public PacketType PacketType => PacketType.Authenticate | PacketType.Request;

  public unsafe void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.NrOfPlayers);
    iWriter.Write(this.Token.Length);
    fixed (byte* numPtr = this.Token.Data)
    {
      for (int index = 0; index < this.Token.Length; ++index)
        iWriter.Write(numPtr[index]);
    }
    iWriter.Write(this.Version);
    if (string.IsNullOrEmpty(this.Password))
      iWriter.Write("");
    else
      iWriter.Write(this.Password);
  }

  public unsafe void Read(BinaryReader iReader)
  {
    this.NrOfPlayers = iReader.ReadByte();
    this.Token.Length = iReader.ReadInt32();
    fixed (byte* numPtr = this.Token.Data)
    {
      for (int index = 0; index < this.Token.Length; ++index)
        numPtr[index] = iReader.ReadByte();
    }
    this.Version = iReader.ReadUInt64();
    this.Password = iReader.ReadString();
  }
}
