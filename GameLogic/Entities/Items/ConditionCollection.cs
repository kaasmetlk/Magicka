// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.ConditionCollection
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public class ConditionCollection
{
  private EventCollection[] mEventCollection = new EventCollection[5];

  public ConditionCollection()
  {
    for (int index = 0; index < this.mEventCollection.Length; ++index)
      this.mEventCollection[index] = new EventCollection(8);
  }

  public ConditionCollection(ContentReader iInput)
  {
    int num = iInput.ReadInt32();
    for (int index = 0; index < num; ++index)
      this.mEventCollection[index] = EventCollection.Read(iInput);
  }

  public EventCollection this[int iIndex] => this.mEventCollection[iIndex];

  public int Count => this.mEventCollection.Length;

  public void CopyTo(ConditionCollection iCollection)
  {
    for (int iIndex = 0; iIndex < this.mEventCollection.Length; ++iIndex)
    {
      if (this.mEventCollection[iIndex] != null)
        this.mEventCollection[iIndex].CopyTo(iCollection[iIndex]);
    }
  }

  public void Clear()
  {
    for (int index = 0; index < this.mEventCollection.Length; ++index)
    {
      this.mEventCollection[index].Condition = new EventCondition();
      this.mEventCollection[index].Condition.Repeat = true;
      this.mEventCollection[index].Clear();
    }
  }

  public bool ExecuteAll(Entity iOwner, Entity iTarget, ref EventCondition iArgs)
  {
    bool flag = false;
    for (int index = 0; index < this.mEventCollection.Length; ++index)
    {
      if (this.mEventCollection[index] != null)
        flag |= this.mEventCollection[index].ExecuteAll(iOwner, iTarget, ref iArgs, out DamageResult _);
    }
    return flag;
  }

  public bool ExecuteAll(
    Entity iOwner,
    Entity iTarget,
    ref EventCondition iArgs,
    out DamageResult oDamageResult)
  {
    bool flag = false;
    oDamageResult = DamageResult.None;
    for (int index = 0; index < this.mEventCollection.Length; ++index)
    {
      if (this.mEventCollection[index] != null)
      {
        DamageResult oDamageResult1;
        flag |= this.mEventCollection[index].ExecuteAll(iOwner, iTarget, ref iArgs, out oDamageResult1);
        oDamageResult |= oDamageResult1;
      }
    }
    return flag;
  }
}
