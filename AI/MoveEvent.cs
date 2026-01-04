// Decompiled with JetBrains decompiler
// Type: Magicka.AI.MoveEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Levels;
using Magicka.Levels.Triggers;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

#nullable disable
namespace Magicka.AI;

public struct MoveEvent
{
  public int Trigger;
  public float Delay;
  public float Speed;
  public Vector3 Waypoint;
  public readonly int WaypointID;
  public bool FixedDirection;
  public Vector3 Direction;

  public MoveEvent(LevelModel iLevel, XmlNode iNode)
    : this()
  {
    this.Speed = 1f;
    foreach (XmlAttribute attribute in (XmlNamedNodeMap) iNode.Attributes)
    {
      if (attribute.Name.Equals("position", StringComparison.OrdinalIgnoreCase))
      {
        string[] strArray = attribute.Value.Split(',');
        if (strArray.Length == 3)
        {
          this.Waypoint.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          this.Waypoint.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          this.Waypoint.Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        }
        else
        {
          this.WaypointID = attribute.Value.ToLowerInvariant().GetHashCodeCustom();
          Locator locator;
          if (iLevel.Locators.TryGetValue(this.WaypointID, out locator))
          {
            this.Waypoint = locator.Transform.Translation;
            this.Direction = locator.Transform.Forward;
          }
          else
          {
            TriggerArea triggerArea;
            if (iLevel.TriggerAreas.TryGetValue(this.WaypointID, out triggerArea))
              this.Waypoint = triggerArea.GetRandomLocation();
          }
        }
      }
      else if (attribute.Name.Equals("facingDirection", StringComparison.OrdinalIgnoreCase))
      {
        string[] strArray = attribute.Value.Split(',');
        if (strArray.Length == 3)
        {
          this.FixedDirection = true;
          this.Direction.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          this.Direction.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          this.Direction.Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          this.Direction.Normalize();
        }
        else
          this.FixedDirection = bool.Parse(attribute.Value);
      }
      else if (attribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
        this.Delay = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      else if (attribute.Name.Equals("speed", StringComparison.OrdinalIgnoreCase))
        this.Speed = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      else
        this.Trigger = attribute.Name.Equals("trigger", StringComparison.OrdinalIgnoreCase) ? attribute.Value.ToLowerInvariant().GetHashCodeCustom() : throw new Exception($"Invalid attribute \"{attribute.Name}\" in \"Event\"!");
    }
  }

  public MoveEvent(BinaryReader iReader)
  {
    this.Trigger = iReader.ReadInt32();
    this.Delay = (float) iReader.ReadInt32();
    this.Direction.X = iReader.ReadSingle();
    this.Direction.Y = iReader.ReadSingle();
    this.Direction.Z = iReader.ReadSingle();
    this.FixedDirection = iReader.ReadBoolean();
    this.Speed = iReader.ReadSingle();
    this.Waypoint.X = iReader.ReadSingle();
    this.Waypoint.Y = iReader.ReadSingle();
    this.Waypoint.Z = iReader.ReadSingle();
    this.WaypointID = iReader.ReadInt32();
  }

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Trigger);
    iWriter.Write(this.Delay);
    iWriter.Write(this.Direction.X);
    iWriter.Write(this.Direction.Y);
    iWriter.Write(this.Direction.Z);
    iWriter.Write(this.FixedDirection);
    iWriter.Write(this.Speed);
    iWriter.Write(this.Waypoint.X);
    iWriter.Write(this.Waypoint.Y);
    iWriter.Write(this.Waypoint.Z);
    iWriter.Write(this.WaypointID);
  }
}
