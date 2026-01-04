// Decompiled with JetBrains decompiler
// Type: Magicka.AI.FaceEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Levels;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

#nullable disable
namespace Magicka.AI;

public struct FaceEvent
{
  public int Trigger;
  public float Delay;
  public float Speed;
  public int TargetID;
  public Vector3 Direction;

  public FaceEvent(LevelModel iLevel, XmlNode iNode)
    : this()
  {
    this.Speed = 1f;
    foreach (XmlAttribute attribute in (XmlNamedNodeMap) iNode.Attributes)
    {
      if (attribute.Name.Equals("facingDirection", StringComparison.OrdinalIgnoreCase) || attribute.Name.Equals("target", StringComparison.OrdinalIgnoreCase))
      {
        string[] strArray = attribute.Value.Split(',');
        if (strArray.Length == 3)
        {
          this.Direction.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          this.Direction.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          this.Direction.Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          this.Direction.Normalize();
        }
        else
          this.TargetID = attribute.Value.ToLowerInvariant().GetHashCodeCustom();
      }
      else if (attribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
        this.Delay = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      else if (attribute.Name.Equals("speed", StringComparison.OrdinalIgnoreCase))
        this.Speed = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      else
        this.Trigger = attribute.Name.Equals("trigger", StringComparison.OrdinalIgnoreCase) ? attribute.Value.ToLowerInvariant().GetHashCodeCustom() : throw new Exception($"Invalid attribute \"{attribute.Name}\" in \"Event\"!");
    }
  }

  public FaceEvent(BinaryReader iReader)
  {
    this.Trigger = iReader.ReadInt32();
    this.Delay = (float) iReader.ReadInt32();
    this.Speed = iReader.ReadSingle();
    this.TargetID = iReader.ReadInt32();
    this.Direction.X = iReader.ReadSingle();
    this.Direction.Y = iReader.ReadSingle();
    this.Direction.Z = iReader.ReadSingle();
  }

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Trigger);
    iWriter.Write(this.Delay);
    iWriter.Write(this.Speed);
    iWriter.Write(this.TargetID);
    iWriter.Write(this.Direction.X);
    iWriter.Write(this.Direction.Y);
    iWriter.Write(this.Direction.Z);
  }
}
