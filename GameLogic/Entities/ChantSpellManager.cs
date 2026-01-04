// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.ChantSpellManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities;

public static class ChantSpellManager
{
  private static readonly int MAXELEMENTS = 128 /*0x80*/;
  private static IntHeap sChantSpellHeap;
  private static ChantSpells[] sChantSpells;
  private static int sLastActiveChantSpell = -1;

  static ChantSpellManager()
  {
    ChantSpellManager.sChantSpellHeap = new IntHeap(ChantSpellManager.MAXELEMENTS);
    for (int iValue = 1; iValue < ChantSpellManager.MAXELEMENTS; ++iValue)
      ChantSpellManager.sChantSpellHeap.Push(iValue);
    ChantSpellManager.sChantSpells = new ChantSpells[ChantSpellManager.MAXELEMENTS];
  }

  public static void Add(ref ChantSpells iChantSpell)
  {
    if (ChantSpellManager.sChantSpellHeap.IsEmpty)
      return;
    iChantSpell.Active = true;
    iChantSpell.Index = ChantSpellManager.sChantSpellHeap.Pop();
    ChantSpellManager.sChantSpells[iChantSpell.Index] = iChantSpell;
    ChantSpellManager.sLastActiveChantSpell = Math.Max(ChantSpellManager.sLastActiveChantSpell, iChantSpell.Index);
  }

  public static void Remove(ref ChantSpells iChantSpell)
  {
    iChantSpell.Active = false;
    ChantSpellManager.sChantSpellHeap.Push(iChantSpell.Index);
    ChantSpellManager.sChantSpells[iChantSpell.Index] = new ChantSpells();
  }

  public static void Set(ChantSpells iChantSpell)
  {
    ChantSpellManager.sChantSpells[iChantSpell.Index] = iChantSpell;
  }

  public static ChantSpells GetChantSpell(int iIndex) => ChantSpellManager.sChantSpells[iIndex];

  internal static void Update(float iDeltaTime)
  {
    for (int index = 1; index <= ChantSpellManager.sLastActiveChantSpell; ++index)
    {
      if (ChantSpellManager.sChantSpells[index].Active)
        ChantSpellManager.sChantSpells[index].Update(iDeltaTime);
    }
  }

  internal static void Merge(
    Character iOwner,
    Elements iElement1,
    Elements iElement2,
    Elements iElementOut)
  {
    int index = -1;
    int iIndex = -1;
    for (int iValue = 1; iValue < ChantSpellManager.sChantSpells.Length; ++iValue)
    {
      if (!ChantSpellManager.sChantSpellHeap.Contains(iValue) && ChantSpellManager.sChantSpells[iValue].Owner == iOwner && ChantSpellManager.sChantSpells[iValue].State != ChantSpellState.Merging)
      {
        if (ChantSpellManager.sChantSpells[iValue].Element == iElement1 && index == -1)
          index = iValue;
        else if (ChantSpellManager.sChantSpells[iValue].Element == iElement2 && iIndex == -1)
          iIndex = iValue;
      }
    }
    if (index < 1 || iIndex < 1)
      return;
    ChantSpellManager.sChantSpells[index].MergeWith(ChantSpellManager.GetChantSpell(iIndex), iElementOut);
  }
}
