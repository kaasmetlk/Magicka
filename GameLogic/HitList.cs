// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.HitList
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic;

public class HitList(int iCapacity) : SortedList<ushort, float>(iCapacity)
{
  public void Add(Entity iEntity) => this[iEntity.Handle] = 0.25f;

  public void Add(IDamageable iDamageable) => this[iDamageable.Handle] = 0.25f;

  public void Add(ISpellCaster iCaster) => this[iCaster.Handle] = 0.25f;

  public void Add(ushort iHandle) => this[iHandle] = 0.25f;

  public bool Contains(IDamageable iDamageable) => this.ContainsKey(iDamageable.Handle);

  public void Update(float iDeltaTime)
  {
    for (int index = 0; index < this.Count; ++index)
    {
      float num = this.Values[index] - iDeltaTime;
      if ((double) num <= 0.0)
        this.RemoveAt(index--);
      else
        this[this.Keys[index]] = num;
    }
  }
}
