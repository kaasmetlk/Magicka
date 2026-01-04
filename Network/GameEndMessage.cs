// Decompiled with JetBrains decompiler
// Type: Magicka.Network.GameEndMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Levels;
using System.IO;

#nullable disable
namespace Magicka.Network;

public struct GameEndMessage : ISendable
{
  public EndGameCondition Condition;
  public int Argument;
  public float DelayTime;
  public bool Phony;

  public PacketType PacketType => PacketType.GameEnd;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write((byte) this.Condition);
    iWriter.Write(this.Argument);
    iWriter.Write(this.DelayTime);
    iWriter.Write(this.Phony);
  }

  public void Read(BinaryReader iReader)
  {
    this.Condition = (EndGameCondition) iReader.ReadByte();
    this.Argument = iReader.ReadInt32();
    this.DelayTime = iReader.ReadSingle();
    this.Phony = iReader.ReadBoolean();
  }
}
