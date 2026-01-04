// Decompiled with JetBrains decompiler
// Type: Magicka.AI.KillEvent
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

public struct KillEvent
{
  public int Trigger;
  public float Delay;
  public bool Remove;

  public KillEvent(LevelModel iLevel, XmlNode iNode)
    : this()
  {
    this.Remove = true;
    foreach (XmlAttribute attribute in (XmlNamedNodeMap) iNode.Attributes)
    {
      if (attribute.Name.Equals("remove", StringComparison.OrdinalIgnoreCase))
        this.Remove = bool.Parse(attribute.Value);
      else if (attribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
        this.Delay = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      else
        this.Trigger = attribute.Name.Equals("trigger", StringComparison.OrdinalIgnoreCase) ? attribute.Value.ToLowerInvariant().GetHashCodeCustom() : throw new Exception($"Invalid attribute \"{attribute.Name}\" in \"Event\"!");
    }
  }

  public KillEvent(BinaryReader iReader)
  {
    this.Trigger = iReader.ReadInt32();
    this.Delay = (float) iReader.ReadInt32();
    this.Remove = iReader.ReadBoolean();
  }

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Trigger);
    iWriter.Write(this.Delay);
    iWriter.Write(this.Remove);
  }
}
