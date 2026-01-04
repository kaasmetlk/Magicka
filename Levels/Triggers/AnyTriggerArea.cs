// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.AnyTriggerArea
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers;

public class AnyTriggerArea : TriggerArea
{
  public AnyTriggerArea()
    : base((Box) null)
  {
  }

  public void Count(EntityManager iEntityManager)
  {
    StaticList<Entity> entities = iEntityManager.Entities;
    for (int iIndex = 0; iIndex < entities.Count; ++iIndex)
    {
      if (entities[iIndex] is Avatar)
      {
        if ((entities[iIndex] as Avatar).IgnoreTriggers)
          continue;
      }
      try
      {
        this.mPresentEntities.Add(entities[iIndex]);
      }
      catch (StaticWeakListNeedsToExpandException ex)
      {
        this.mPresentEntities.Expand();
        this.mPresentEntities.Add(entities[iIndex]);
      }
      ++this.mTotalEntities;
      if (entities[iIndex] is Character iItem && (!iItem.Dead || iItem.Template.Undying))
      {
        this.mPresentCharacters.Add(iItem);
        int num;
        if (!this.mTypeCount.TryGetValue(iItem.Type, out num))
          num = 0;
        this.mTypeCount[iItem.Type] = num + 1;
        for (int index = 0; index < 8; ++index)
        {
          Factions key = (Factions) (1 << index);
          if ((key & iItem.GetOriginalFaction) != Factions.NONE)
          {
            if (!this.mFactionCount.TryGetValue((int) key, out num))
              num = 0;
            this.mFactionCount[(int) key] = num + 1;
          }
        }
        ++this.mTotalCharacters;
      }
    }
  }

  public override void Register()
  {
  }

  internal override void UpdatePresent(EntityManager iManager)
  {
    this.Reset();
    this.Count(iManager);
  }
}
