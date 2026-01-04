// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.EventCollection
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public class EventCollection : List<EventStorage>
{
  public EventCondition Condition;
  public static bool Repeat = true;

  public EventCollection(int iCapacity)
    : base(iCapacity)
  {
    this.Condition.EventConditionType = EventConditionType.Default;
    EventCollection.Repeat = false;
  }

  public static EventCollection Read(ContentReader iInput)
  {
    EventCondition eventCondition = new EventCondition(iInput);
    EventCollection.Repeat = iInput.ReadBoolean();
    eventCondition.Repeat = EventCollection.Repeat;
    int iCapacity = iInput.ReadInt32();
    EventCollection eventCollection = new EventCollection(iCapacity);
    eventCollection.Condition = eventCondition;
    for (int index = 0; index < iCapacity; ++index)
    {
      EventStorage eventStorage = new EventStorage(iInput);
      eventCollection.Add(eventStorage);
    }
    return eventCollection;
  }

  public void ExecuteAll(Entity iOwner, Entity iTarget, Vector3? iPosition)
  {
    for (int index = 0; index < this.Count; ++index)
    {
      int num = (int) this[index].Execute(iOwner, iTarget, ref iPosition);
    }
  }

  public bool ExecuteAll(
    Entity iOwner,
    Entity iTarget,
    ref EventCondition iArgs,
    out DamageResult oDamageResult)
  {
    oDamageResult = DamageResult.None;
    if (this.Condition.IsMet(ref iArgs))
    {
      for (int index = 0; index < this.Count; ++index)
      {
        EventStorage eventStorage = this[index];
        oDamageResult |= eventStorage.Execute(iOwner, iTarget, ref iArgs.Position);
        this[index] = eventStorage;
      }
      if (this.Count > 0)
        return true;
    }
    return false;
  }

  public virtual void CopyTo(EventCollection iCollection)
  {
    iCollection.Clear();
    iCollection.AddRange((IEnumerable<EventStorage>) this);
    iCollection.Condition = this.Condition;
  }
}
