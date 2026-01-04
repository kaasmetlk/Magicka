// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AnimationEvent
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

public struct AnimationEvent
{
  public int Trigger;
  public float Delay;
  public Animations Animation;
  public float BlendTime;
  public Animations IdleAnimation;

  public AnimationEvent(LevelModel iLevel, XmlNode iNode)
    : this()
  {
    foreach (XmlAttribute attribute in (XmlNamedNodeMap) iNode.Attributes)
    {
      if (attribute.Name.Equals("animation", StringComparison.OrdinalIgnoreCase))
        this.Animation = (Animations) Enum.Parse(typeof (Animations), attribute.Value, true);
      else if (attribute.Name.Equals("idleanimation", StringComparison.OrdinalIgnoreCase))
        this.IdleAnimation = (Animations) Enum.Parse(typeof (Animations), attribute.Value, true);
      else if (attribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
        this.Delay = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      else if (attribute.Name.Equals("blendTime", StringComparison.OrdinalIgnoreCase))
        this.BlendTime = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      else
        this.Trigger = attribute.Name.Equals("trigger", StringComparison.OrdinalIgnoreCase) ? attribute.Value.ToLowerInvariant().GetHashCodeCustom() : throw new Exception($"Invalid attribute \"{attribute.Name}\" in \"Event\"!");
    }
  }

  public AnimationEvent(BinaryReader iReader)
  {
    this.Trigger = iReader.ReadInt32();
    this.Delay = (float) iReader.ReadInt32();
    this.Animation = (Animations) iReader.ReadInt32();
    this.BlendTime = iReader.ReadSingle();
    this.IdleAnimation = (Animations) iReader.ReadInt32();
  }

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Trigger);
    iWriter.Write(this.Delay);
    iWriter.Write((int) this.Animation);
    iWriter.Write(this.BlendTime);
    iWriter.Write((int) this.IdleAnimation);
  }
}
