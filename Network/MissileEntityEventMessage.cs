// Decompiled with JetBrains decompiler
// Type: Magicka.Network.MissileEntityEventMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct MissileEntityEventMessage : ISendable
{
  public bool OnCollision;
  public float TimeAlive;
  public float Threshold;
  public float HitPoints;
  public Elements Elements;
  public ushort Handle;
  public ushort TargetHandle;
  public EventConditionType EventConditionType;

  public PacketType PacketType => PacketType.MissileEntity;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.OnCollision);
    iWriter.Write(this.Handle);
    iWriter.Write(this.HitPoints);
    iWriter.Write((ushort) this.Elements);
    iWriter.Write(this.TimeAlive);
    iWriter.Write(this.Threshold);
    iWriter.Write(this.TargetHandle);
    iWriter.Write((byte) this.EventConditionType);
  }

  public void Read(BinaryReader iReader)
  {
    this.OnCollision = iReader.ReadBoolean();
    this.Handle = iReader.ReadUInt16();
    this.HitPoints = iReader.ReadSingle();
    this.Elements = (Elements) iReader.ReadUInt16();
    this.TimeAlive = iReader.ReadSingle();
    this.Threshold = iReader.ReadSingle();
    this.TargetHandle = iReader.ReadUInt16();
    this.EventConditionType = (EventConditionType) iReader.ReadByte();
  }
}
