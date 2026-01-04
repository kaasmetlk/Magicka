// Decompiled with JetBrains decompiler
// Type: Magicka.Network.AnimatedLevelPartUpdateMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct AnimatedLevelPartUpdateMessage : ISendable
{
  public ushort Handle;
  public bool Playing;
  public float AnimationTime;

  public PacketType PacketType => PacketType.AnimatedLevelPartUpdate;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Handle);
    iWriter.Write(this.Playing);
    iWriter.Write(new HalfSingle(this.AnimationTime).PackedValue);
  }

  public void Read(BinaryReader iReader)
  {
    this.Handle = iReader.ReadUInt16();
    this.Playing = iReader.ReadBoolean();
    this.AnimationTime = new HalfSingle()
    {
      PackedValue = iReader.ReadUInt16()
    }.ToSingle();
  }
}
