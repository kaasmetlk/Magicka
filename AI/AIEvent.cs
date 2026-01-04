// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AIEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Levels;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

#nullable disable
namespace Magicka.AI;

[StructLayout(LayoutKind.Explicit)]
public struct AIEvent
{
  [FieldOffset(0)]
  public AIEventType EventType;
  [FieldOffset(4)]
  public MoveEvent MoveEvent;
  [FieldOffset(4)]
  public AnimationEvent AnimationEvent;
  [FieldOffset(4)]
  public FaceEvent FaceEvent;
  [FieldOffset(4)]
  public KillEvent KillEvent;
  [FieldOffset(4)]
  public LoopEvent LoopEvent;

  public AIEvent(LevelModel iLevel, XmlNode iNode)
    : this()
  {
    this.EventType = (AIEventType) Enum.Parse(typeof (AIEventType), iNode.Name, true);
    switch (this.EventType)
    {
      case AIEventType.Move:
        this.MoveEvent = new MoveEvent(iLevel, iNode);
        break;
      case AIEventType.Animation:
        this.AnimationEvent = new AnimationEvent(iLevel, iNode);
        break;
      case AIEventType.Face:
        this.FaceEvent = new FaceEvent(iLevel, iNode);
        break;
      case AIEventType.Kill:
        this.KillEvent = new KillEvent(iLevel, iNode);
        break;
      case AIEventType.Loop:
        this.LoopEvent = new LoopEvent(iLevel, iNode);
        break;
      default:
        throw new Exception("Invalid EventType!");
    }
  }

  public AIEvent(BinaryReader iReader)
    : this()
  {
    this.EventType = (AIEventType) iReader.ReadByte();
    switch (this.EventType)
    {
      case AIEventType.Move:
        this.MoveEvent = new MoveEvent(iReader);
        break;
      case AIEventType.Animation:
        this.AnimationEvent = new AnimationEvent(iReader);
        break;
      case AIEventType.Face:
        this.FaceEvent = new FaceEvent(iReader);
        break;
      case AIEventType.Kill:
        this.KillEvent = new KillEvent(iReader);
        break;
      case AIEventType.Loop:
        this.LoopEvent = new LoopEvent(iReader);
        break;
    }
  }

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write((byte) this.EventType);
    switch (this.EventType)
    {
      case AIEventType.Move:
        this.MoveEvent.Write(iWriter);
        break;
      case AIEventType.Animation:
        this.AnimationEvent.Write(iWriter);
        break;
      case AIEventType.Face:
        this.FaceEvent.Write(iWriter);
        break;
      case AIEventType.Kill:
        this.KillEvent.Write(iWriter);
        break;
      case AIEventType.Loop:
        this.LoopEvent.Write(iWriter);
        break;
    }
  }
}
