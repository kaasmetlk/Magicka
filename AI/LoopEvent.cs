// Decompiled with JetBrains decompiler
// Type: Magicka.AI.LoopEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Levels;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

#nullable disable
namespace Magicka.AI;

public struct LoopEvent
{
  public int Trigger;
  public float Delay;
  public LoopType Type;

  public LoopEvent(LevelModel iLevel, XmlNode iNode)
    : this()
  {
    foreach (XmlAttribute attribute in (XmlNamedNodeMap) iNode.Attributes)
    {
      if (attribute.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
        this.Type = (LoopType) Enum.Parse(typeof (LoopType), attribute.Value, true);
      else if (attribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
        this.Delay = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      else
        this.Trigger = attribute.Name.Equals("trigger", StringComparison.OrdinalIgnoreCase) ? attribute.Value.ToLowerInvariant().GetHashCodeCustom() : throw new Exception($"Invalid attribute \"{attribute.Name}\" in \"Event\"!");
    }
  }

  public LoopEvent(BinaryReader iReader)
  {
    this.Trigger = iReader.ReadInt32();
    this.Delay = (float) iReader.ReadInt32();
    this.Type = (LoopType) iReader.ReadByte();
  }

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Trigger);
    iWriter.Write(this.Delay);
    iWriter.Write((byte) this.Type);
  }
}
