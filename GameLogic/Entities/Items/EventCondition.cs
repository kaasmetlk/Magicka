// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.EventCondition
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct EventCondition
{
  public EventConditionType EventConditionType;
  public float Hitpoints;
  public Elements Elements;
  public float Time;
  public float Threshold;
  public int Count;
  public bool Activated;
  public bool Repeat;
  public Vector3? Position;

  public EventCondition(ContentReader iInput)
  {
    this.EventConditionType = (EventConditionType) iInput.ReadByte();
    this.Hitpoints = (float) iInput.ReadInt32();
    this.Elements = (Elements) iInput.ReadInt32();
    this.Threshold = iInput.ReadSingle();
    this.Time = iInput.ReadSingle();
    this.Activated = false;
    this.Repeat = true;
    this.Count = 0;
    this.Position = new Vector3?();
  }

  public bool IsMet(ref EventCondition iArgs)
  {
    EventConditionType eventConditionType1 = iArgs.EventConditionType & this.EventConditionType;
    if (eventConditionType1 == (EventConditionType) 0 || !(!this.Activated | this.Repeat))
      return false;
    for (int index = 0; index < 7; ++index)
    {
      EventConditionType eventConditionType2 = (EventConditionType) (1 << index) & eventConditionType1;
      if ((uint) eventConditionType2 <= 8U)
      {
        switch (eventConditionType2 - (byte) 1)
        {
          case (EventConditionType) 0:
          case EventConditionType.Default:
            break;
          case EventConditionType.Hit:
            continue;
          case EventConditionType.Default | EventConditionType.Hit:
            if ((double) iArgs.Threshold >= (double) this.Threshold)
            {
              this.Activated = true;
              return true;
            }
            continue;
          default:
            if (eventConditionType2 == EventConditionType.Damaged && (double) iArgs.Hitpoints >= (double) this.Hitpoints && (iArgs.Elements & this.Elements) != Elements.None)
            {
              this.Activated = true;
              return true;
            }
            continue;
        }
      }
      else if (eventConditionType2 != EventConditionType.Timer)
      {
        if (eventConditionType2 != EventConditionType.Death && eventConditionType2 != EventConditionType.OverKill)
          continue;
      }
      else
      {
        if ((double) iArgs.Time >= (double) this.Time * (double) (this.Count + 1))
        {
          ++this.Count;
          this.Activated = true;
          return true;
        }
        continue;
      }
      this.Activated = true;
      return true;
    }
    return false;
  }
}
