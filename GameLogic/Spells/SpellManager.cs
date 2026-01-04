// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Storage;
using PolygonHead;
using SteamWrapper;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells;

public class SpellManager
{
  internal const ulong VANILLA_CAMPAIGN_MAGICKS = 8384510;
  internal const ulong MYTHOS_CAMPAIGN_MAGICKS = 1040187402 /*0x3E00000A*/;
  private static SpellManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private SpellTree mSpellTree;
  private MagickTree mMagickTree;
  private List<IAbilityEffect> mEffects = new List<IAbilityEffect>(32 /*0x20*/);
  private Elements[][] mCombos;

  public static SpellManager Instance
  {
    get
    {
      if (SpellManager.mSingelton == null)
      {
        lock (SpellManager.mSingeltonLock)
        {
          if (SpellManager.mSingelton == null)
            SpellManager.mSingelton = new SpellManager();
        }
      }
      return SpellManager.mSingelton;
    }
  }

  private SpellManager()
  {
    this.mCombos = new Elements[35][];
    this.mSpellTree = new SpellTree();
    SpellNode spellNode1 = (SpellNode) null;
    Spell oSpell = new Spell();
    this.mSpellTree.GoToRoot();
    spellNode1 = this.mSpellTree.MoveAndAdd(ControllerDirection.Up);
    SpellNode spellNode2 = this.mSpellTree.MoveAndAdd(ControllerDirection.Left);
    Spell.DefaultSpell(Elements.Life, out oSpell);
    spellNode2.Content = oSpell;
    this.mSpellTree.GoToRoot();
    spellNode1 = this.mSpellTree.MoveAndAdd(ControllerDirection.Left);
    SpellNode spellNode3 = this.mSpellTree.MoveAndAdd(ControllerDirection.Up);
    Spell.DefaultSpell(Elements.Water, out oSpell);
    spellNode3.Content = oSpell;
    this.mSpellTree.GoToRoot();
    spellNode1 = this.mSpellTree.MoveAndAdd(ControllerDirection.Right);
    SpellNode spellNode4 = this.mSpellTree.MoveAndAdd(ControllerDirection.Up);
    Spell.DefaultSpell(Elements.Cold, out oSpell);
    spellNode4.Content = oSpell;
    this.mSpellTree.GoToRoot();
    spellNode1 = this.mSpellTree.MoveAndAdd(ControllerDirection.Right);
    SpellNode spellNode5 = this.mSpellTree.MoveAndAdd(ControllerDirection.Down);
    Spell.DefaultSpell(Elements.Fire, out oSpell);
    spellNode5.Content = oSpell;
    this.mSpellTree.GoToRoot();
    spellNode1 = this.mSpellTree.MoveAndAdd(ControllerDirection.Up);
    SpellNode spellNode6 = this.mSpellTree.MoveAndAdd(ControllerDirection.Right);
    Spell.DefaultSpell(Elements.Arcane, out oSpell);
    spellNode6.Content = oSpell;
    this.mSpellTree.GoToRoot();
    spellNode1 = this.mSpellTree.MoveAndAdd(ControllerDirection.Left);
    SpellNode spellNode7 = this.mSpellTree.MoveAndAdd(ControllerDirection.Down);
    Spell.DefaultSpell(Elements.Lightning, out oSpell);
    spellNode7.Content = oSpell;
    this.mSpellTree.GoToRoot();
    spellNode1 = this.mSpellTree.MoveAndAdd(ControllerDirection.Down);
    SpellNode spellNode8 = this.mSpellTree.MoveAndAdd(ControllerDirection.Right);
    Spell.DefaultSpell(Elements.Shield, out oSpell);
    spellNode8.Content = oSpell;
    this.mSpellTree.GoToRoot();
    spellNode1 = this.mSpellTree.MoveAndAdd(ControllerDirection.Down);
    SpellNode spellNode9 = this.mSpellTree.MoveAndAdd(ControllerDirection.Left);
    Spell.DefaultSpell(Elements.Earth, out oSpell);
    spellNode9.Content = oSpell;
    this.mMagickTree = new MagickTree();
    this.AddMagick(MagickType.Revive, new Elements[2]
    {
      Elements.Life,
      Elements.Lightning
    });
    this.AddMagick(MagickType.Haste, new Elements[3]
    {
      Elements.Lightning,
      Elements.Arcane,
      Elements.Fire
    });
    this.AddMagick(MagickType.Rain, new Elements[2]
    {
      Elements.Water,
      Elements.Steam
    });
    this.AddMagick(MagickType.Grease, new Elements[3]
    {
      Elements.Water,
      Elements.Earth,
      Elements.Life
    });
    this.AddMagick(MagickType.Teleport, new Elements[3]
    {
      Elements.Lightning,
      Elements.Arcane,
      Elements.Lightning
    });
    this.AddMagick(MagickType.ThunderB, new Elements[4]
    {
      Elements.Steam,
      Elements.Lightning,
      Elements.Arcane,
      Elements.Lightning
    });
    this.AddMagick(MagickType.Charm, new Elements[3]
    {
      Elements.Life,
      Elements.Shield,
      Elements.Earth
    });
    this.AddMagick(MagickType.MeteorS, new Elements[5]
    {
      Elements.Fire,
      Elements.Earth,
      Elements.Steam,
      Elements.Earth,
      Elements.Fire
    });
    this.AddMagick(MagickType.Fear, new Elements[3]
    {
      Elements.Cold,
      Elements.Arcane,
      Elements.Shield
    });
    this.AddMagick(MagickType.SUndead, new Elements[4]
    {
      Elements.Ice,
      Elements.Earth,
      Elements.Arcane,
      Elements.Cold
    });
    this.AddMagick(MagickType.Conflagration, new Elements[5]
    {
      Elements.Steam,
      Elements.Fire,
      Elements.Steam,
      Elements.Fire,
      Elements.Steam
    });
    this.AddMagick(MagickType.SDeath, new Elements[5]
    {
      Elements.Arcane,
      Elements.Cold,
      Elements.Ice,
      Elements.Cold,
      Elements.Arcane
    });
    this.AddMagick(MagickType.SPhoenix, new Elements[3]
    {
      Elements.Life,
      Elements.Lightning,
      Elements.Fire
    });
    this.AddMagick(MagickType.Invisibility, new Elements[4]
    {
      Elements.Arcane,
      Elements.Shield,
      Elements.Steam,
      Elements.Arcane
    });
    this.AddMagick(MagickType.SElemental, new Elements[5]
    {
      Elements.Arcane,
      Elements.Shield,
      Elements.Earth,
      Elements.Steam,
      Elements.Arcane
    });
    this.AddMagick(MagickType.Blizzard, new Elements[3]
    {
      Elements.Cold,
      Elements.Ice,
      Elements.Cold
    });
    this.AddMagick(MagickType.ThunderS, new Elements[5]
    {
      Elements.Steam,
      Elements.Steam,
      Elements.Lightning,
      Elements.Arcane,
      Elements.Lightning
    });
    this.AddMagick(MagickType.Tornado, new Elements[4]
    {
      Elements.Earth,
      Elements.Steam,
      Elements.Water,
      Elements.Steam
    });
    this.AddMagick(MagickType.Nullify, new Elements[2]
    {
      Elements.Arcane,
      Elements.Shield
    });
    this.AddMagick(MagickType.Vortex, new Elements[5]
    {
      Elements.Ice,
      Elements.Arcane,
      Elements.Ice,
      Elements.Shield,
      Elements.Ice
    });
    this.AddMagick(MagickType.Corporealize, new Elements[5]
    {
      Elements.Arcane,
      Elements.Steam,
      Elements.Lightning,
      Elements.Shield,
      Elements.Arcane
    });
    this.AddMagick(MagickType.TimeWarp, new Elements[2]
    {
      Elements.Cold,
      Elements.Shield
    });
    this.AddMagick(MagickType.CTD, new Elements[4]
    {
      Elements.Lightning,
      Elements.Lightning,
      Elements.Fire,
      Elements.Life
    });
    this.AddMagick(MagickType.Napalm, new Elements[5]
    {
      Elements.Steam,
      Elements.Earth,
      Elements.Life,
      Elements.Fire,
      Elements.Fire
    });
    this.AddMagick(MagickType.Portal, new Elements[3]
    {
      Elements.Steam,
      Elements.Lightning,
      Elements.Shield
    });
    this.AddMagick(MagickType.TractorPull, new Elements[2]
    {
      Elements.Earth,
      Elements.Arcane
    });
    this.AddMagick(MagickType.ProppMagick, new Elements[3]
    {
      Elements.Fire,
      Elements.Steam,
      Elements.Arcane
    });
    this.AddMagick(MagickType.Levitate, new Elements[3]
    {
      Elements.Steam,
      Elements.Arcane,
      Elements.Steam
    });
    this.AddMagick(MagickType.ChainLightning, new Elements[3]
    {
      Elements.Lightning,
      Elements.Lightning,
      Elements.Lightning
    });
    this.AddMagick(MagickType.Confuse, new Elements[3]
    {
      Elements.Arcane,
      Elements.Shield,
      Elements.Lightning
    });
    this.AddMagick(MagickType.Wave, new Elements[5]
    {
      Elements.Earth,
      Elements.Steam,
      Elements.Earth,
      Elements.Steam,
      Elements.Earth
    });
    this.AddMagick(MagickType.PerformanceEnchantment, new Elements[5]
    {
      Elements.Life,
      Elements.Fire,
      Elements.Lightning,
      Elements.Fire,
      Elements.Life
    });
    this.AddMagick(MagickType.JudgementSpray, new Elements[4]
    {
      Elements.Ice,
      Elements.Ice,
      Elements.Arcane,
      Elements.Shield
    });
    this.AddMagick(MagickType.Amalgameddon, new Elements[3]
    {
      Elements.Arcane,
      Elements.Water,
      Elements.Lightning
    });
  }

  private void AddMagick(MagickType iType, Elements[] iElements)
  {
    this.mMagickTree.GoToRoot();
    MagickNode magickNode = (MagickNode) null;
    for (int index = 0; index < iElements.Length; ++index)
      magickNode = this.mMagickTree.MoveAndAdd(iElements[index]);
    magickNode.Content = iType;
    this.mCombos[(int) magickNode.Content] = iElements;
  }

  public Spell Combine(StaticList<Spell> iSpells)
  {
    if (iSpells.Count == 0)
      return new Spell();
    Spell iSpell = iSpells[0];
    if (iSpell.Element == Elements.All)
      return iSpell;
    for (int iIndex = 1; iIndex < iSpells.Count; ++iIndex)
      iSpell += iSpells[iIndex];
    return iSpell;
  }

  public bool IsMagickAllowed(Player iPlayer, GameType iGameType, MagickType iMagick)
  {
    ulong num = 1UL << (int) (iMagick & (MagickType.Wave | MagickType.PerformanceEnchantment));
    if (iGameType == GameType.Campaign | iGameType == GameType.Mythos)
    {
      Player[] players = Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && ((long) players[index].UnlockedMagicks & (long) num) != 0L)
          return true;
      }
    }
    else if (iGameType == GameType.Challenge || iGameType == GameType.StoryChallange)
    {
      Player[] players = Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && ((long) players[index].UnlockedMagicks & (long) num) != 0L)
          return true;
      }
    }
    else
      return iPlayer == null ? iMagick != MagickType.None : ((long) iPlayer.UnlockedMagicks & 1L << (int) (iMagick & (MagickType.Wave | MagickType.PerformanceEnchantment))) != 0L;
    return false;
  }

  public MagickType GetMagickType(Player iPlayer, PlayState iPlayState, StaticList<Spell> iSpells)
  {
    MagickType oMagick = MagickType.None;
    this.mMagickTree.GoToRoot();
    for (int iIndex = 0; iIndex < iSpells.Count; ++iIndex)
      this.mMagickTree.Move(iSpells[iIndex].Element, out oMagick);
    return oMagick != MagickType.None && (this.IsMagickAllowed(iPlayer, iPlayState.GameType, oMagick) || iPlayer == null) ? oMagick : MagickType.None;
  }

  public bool CombineMagick(Player iPlayer, GameType iGameType, StaticList<Spell> iSpells)
  {
    MagickType oMagick = MagickType.None;
    bool flag = false;
    this.mMagickTree.GoToRoot();
    for (int iIndex = 0; iIndex < iSpells.Count; ++iIndex)
    {
      flag = this.mMagickTree.Move(iSpells[iIndex].Element, out oMagick);
      if (!flag)
        break;
    }
    if (!flag || oMagick == MagickType.None || !this.IsMagickAllowed(iPlayer, iGameType, oMagick) && iPlayer != null)
      return false;
    iSpells.Clear();
    iSpells.Add(new SpellMagickConverter()
    {
      Magick = {
        Element = Elements.All,
        MagickType = oMagick
      }
    }.Spell);
    return true;
  }

  public bool IsMagick(Player iPlayer, GameType iGameType, StaticList<Spell> iSpells)
  {
    MagickType oMagick = MagickType.None;
    bool flag = false;
    this.mMagickTree.GoToRoot();
    for (int iIndex = 0; iIndex < iSpells.Count; ++iIndex)
    {
      flag = this.mMagickTree.Move(iSpells[iIndex].Element, out oMagick);
      if (!flag)
        break;
    }
    return flag && oMagick != MagickType.None && (this.IsMagickAllowed(iPlayer, iGameType, oMagick) || iPlayer == null);
  }

  public Spell HandleCombo(Player iPlayer)
  {
    StaticList<int> inputQueue = iPlayer.InputQueue;
    this.mSpellTree.GoToRoot();
    for (int iIndex = 0; iIndex < inputQueue.Count; ++iIndex)
    {
      Spell oSpell;
      if (!this.mSpellTree.Move((ControllerDirection) inputQueue[iIndex], out oSpell))
      {
        if (inputQueue.Count <= 0)
          return oSpell;
        inputQueue.RemoveAt(0);
      }
      else if (oSpell.Element != Elements.None)
      {
        if (TutorialManager.Instance.IsElementEnabled(oSpell.Element))
          return oSpell;
        iPlayer.InputQueue.Clear();
      }
    }
    return new Spell();
  }

  public static bool InclusiveOpposites(Elements iA, Elements iB)
  {
    bool flag = SpellManager.Opposites(iA, iB);
    if (((iA & Elements.Fire) == Elements.Fire || (iB & Elements.Fire) == Elements.Fire) && ((iA & Elements.Water) == Elements.Water || (iB & Elements.Water) == Elements.Water))
      flag = true;
    return flag;
  }

  public static bool Opposites(Elements iA, Elements iB)
  {
    return ((iA & Elements.Arcane) == Elements.Arcane || (iB & Elements.Arcane) == Elements.Arcane) && ((iA & Elements.Life) == Elements.Life || (iB & Elements.Life) == Elements.Life) || ((iA & Elements.Fire) == Elements.Fire || (iB & Elements.Fire) == Elements.Fire) && ((iA & Elements.Cold) == Elements.Cold || (iB & Elements.Cold) == Elements.Cold) || ((iA & Elements.Earth) == Elements.Earth || (iB & Elements.Earth) == Elements.Earth) && ((iA & Elements.Lightning) == Elements.Lightning || (iB & Elements.Lightning) == Elements.Lightning) || ((iA & Elements.Water) == Elements.Water || (iB & Elements.Water) == Elements.Water) && ((iA & Elements.Lightning) == Elements.Lightning || (iB & Elements.Lightning) == Elements.Lightning) || (iA & Elements.Shield) == Elements.Shield && (iB & Elements.Shield) == Elements.Shield;
  }

  public static bool Revertable(Elements iA, Elements iB)
  {
    return (iA & Elements.Cold) == Elements.Cold && (iB & Elements.Steam) == Elements.Steam || (iB & Elements.Cold) == Elements.Cold && (iA & Elements.Steam) == Elements.Steam || (iA & Elements.Ice) == Elements.Ice && (iB & Elements.Fire) == Elements.Fire || (iB & Elements.Ice) == Elements.Ice && (iA & Elements.Fire) == Elements.Fire;
  }

  public static bool Combinable(Elements iA, Elements iB)
  {
    return (iA & Elements.Water) == Elements.Water && (iB & Elements.Cold) == Elements.Cold || (iB & Elements.Water) == Elements.Water && (iA & Elements.Cold) == Elements.Cold || (iA & Elements.Water) == Elements.Water && (iB & Elements.Fire) == Elements.Fire || (iB & Elements.Water) == Elements.Water && (iA & Elements.Fire) == Elements.Fire;
  }

  public static int FindOpposites(StaticList<Spell> iList, int iLastIndex, Elements iElement)
  {
    for (int iIndex = iLastIndex; iIndex >= 0; --iIndex)
    {
      Spell i = iList[iIndex];
      if (SpellManager.Opposites(iElement, i.Element))
        return iIndex;
    }
    return -1;
  }

  public static int FindRevertables(StaticList<Spell> iList, int iLastIndex, Elements iElement)
  {
    for (int iIndex = iLastIndex; iIndex >= 0; --iIndex)
    {
      Spell i = iList[iIndex];
      if (SpellManager.Revertable(iElement, i.Element))
        return iIndex;
    }
    return -1;
  }

  public static int FindCombines(StaticList<Spell> iList, int iLastIndex, Elements iElement)
  {
    for (int iIndex = iLastIndex; iIndex >= 0; --iIndex)
    {
      Spell i = iList[iIndex];
      if (SpellManager.Combinable(iElement, i.Element))
        return iIndex;
    }
    return -1;
  }

  public static int FindDifferentElement(
    StaticList<Spell> iList,
    int iLastIndex,
    Elements iElement)
  {
    for (int iIndex = iLastIndex; iIndex >= 0; --iIndex)
    {
      Spell i = iList[iIndex];
      if (iElement != i.Element)
        return iIndex;
    }
    return -1;
  }

  public static void FindOppositesAndCombinables(
    Player iPlayer,
    Magicka.GameLogic.Entities.Character iOwner,
    StaticList<Spell> iList)
  {
    Elements iElement1 = Elements.None;
    Elements iElement2 = Elements.None;
    Elements iElementOut = Elements.None;
    for (int index = iList.Count - 1; index >= 0; --index)
    {
      Spell i = iList[index];
      int opposites = SpellManager.FindOpposites(iList, index - 1, i.Element);
      if (opposites >= 0)
      {
        iElement1 = iList[opposites].Element;
        iElement2 = iList[index].Element;
        iPlayer?.IconRenderer.TransformIconForRemoval(index, opposites);
        iList.RemoveAt(opposites);
        --index;
        iList.RemoveAt(index);
      }
    }
    for (int index = iList.Count - 1; index >= 0; --index)
    {
      Spell i = iList[index];
      int revertables = SpellManager.FindRevertables(iList, index - 1, i.Element);
      if (revertables >= 0)
      {
        Spell spell = i + iList[revertables];
        iElement1 = iList[revertables].Element;
        iElement2 = iList[index].Element;
        iElementOut = spell.Element;
        iPlayer?.IconRenderer.TransformIconForCombine(spell.Element, index, revertables);
        iList[revertables] = spell;
        iList.RemoveAt(index);
      }
    }
    for (int index = iList.Count - 1; index >= 0; --index)
    {
      Spell i = iList[index];
      int combines = SpellManager.FindCombines(iList, index - 1, i.Element);
      if (combines >= 0)
      {
        Spell spell = i + iList[combines];
        iElement1 = iList[combines].Element;
        iElement2 = iList[index].Element;
        iElementOut = spell.Element;
        iPlayer?.IconRenderer.TransformIconForCombine(spell.Element, index, combines);
        iList[combines] = spell;
        iList.RemoveAt(index);
      }
    }
    if (iElement1 == Elements.None || iElement2 == Elements.None)
      return;
    ChantSpellManager.Merge(iOwner, iElement1, iElement2, iElementOut);
  }

  public bool TryAddToQueue(
    Player iPlayer,
    Magicka.GameLogic.Entities.Character iOwner,
    StaticList<Spell> iList,
    int iMaxCount,
    ref Spell iSpell)
  {
    iPlayer?.IconRenderer.AddIcon(iSpell.Element, iPlayer.Controller is DirectInputController);
    ChantSpells iChantSpell = new ChantSpells(iSpell.Element, iOwner);
    int num = iList.Count + 1;
    if (iList.Count < iMaxCount)
    {
      ChantSpellManager.Add(ref iChantSpell);
      iList.Add(iSpell);
      SpellManager.FindOppositesAndCombinables(iPlayer, iOwner, iList);
    }
    else
    {
      Elements iElement1 = Elements.None;
      Elements iElement2 = Elements.None;
      Elements iElementOut = Elements.None;
      int opposites = SpellManager.FindOpposites(iList, iMaxCount - 1, iSpell.Element);
      if (opposites >= 0)
      {
        iElement1 = iList[opposites].Element;
        iElement2 = iSpell.Element;
        iPlayer?.IconRenderer.TransformIconForRemoval(iMaxCount, opposites);
        iList.RemoveAt(opposites);
      }
      else
      {
        int revertables = SpellManager.FindRevertables(iList, iMaxCount - 1, iSpell.Element);
        if (revertables >= 0)
        {
          Spell spell = iSpell + iList[revertables];
          iElement1 = iList[revertables].Element;
          iElement2 = iSpell.Element;
          iElementOut = spell.Element;
          iList[revertables] = spell;
          iPlayer?.IconRenderer.TransformIconForCombine(spell.Element, iMaxCount, revertables);
        }
        else
        {
          int combines = SpellManager.FindCombines(iList, iMaxCount - 1, iSpell.Element);
          if (combines >= 0)
          {
            Spell spell = iSpell + iList[combines];
            iElement1 = iList[combines].Element;
            iElement2 = iSpell.Element;
            iElementOut = spell.Element;
            iList[combines] = spell;
            iPlayer?.IconRenderer.TransformIconForCombine(spell.Element, iMaxCount, combines);
          }
        }
      }
      ChantSpellManager.Add(ref iChantSpell);
      if (iElement1 != Elements.None && iElement2 != Elements.None)
      {
        iChantSpell.State = ChantSpellState.Orbiting;
        ChantSpellManager.Merge(iOwner, iElement1, iElement2, iElementOut);
      }
    }
    return num == iList.Count;
  }

  public void UnlockMagick(Player iPlayer, MagickType iMagick)
  {
    iPlayer.UnlockedMagicks |= (ulong) (1L << (int) (iMagick & (MagickType.Wave | MagickType.PerformanceEnchantment)));
  }

  public void UnlockMagick(MagickType iMagick, GameType iGameType)
  {
    ulong num1 = 0;
    Player[] players = Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      players[index].UnlockedMagicks |= (ulong) (1L << (int) (iMagick & (MagickType.Wave | MagickType.PerformanceEnchantment)));
      num1 |= players[index].UnlockedMagicks;
      if (iGameType == GameType.Campaign && ((long) players[index].UnlockedMagicks & 8384510L) == 8384510L && players[index].Avatar != null)
        AchievementsManager.Instance.AwardAchievement(players[index].Avatar.PlayState, "iputonmyrobeandwizar");
    }
    if (iGameType != GameType.Campaign)
      return;
    int nCurProgress = Helper.CountSetBits(num1 & 8384510UL);
    for (int index = 0; index < SaveManager.Instance.SaveSlots.Length; ++index)
    {
      SaveData saveSlot = SaveManager.Instance.SaveSlots[index];
      if (saveSlot != null)
      {
        int num2 = Helper.CountSetBits(saveSlot.UnlockedMagicks & 8384510UL);
        if (num2 > nCurProgress)
          nCurProgress = num2;
      }
    }
    if (iGameType != GameType.Campaign && iGameType != GameType.Mythos || HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    SteamUserStats.IndicateAchievementProgress("iputonmyrobeandwizar", (uint) nCurProgress, (uint) Helper.CountSetBits(8384510UL));
    SteamUserStats.StoreStats();
  }

  public void AddSpellEffect(IAbilityEffect iSpellEffect)
  {
    if (this.mEffects.Contains(iSpellEffect))
      return;
    this.mEffects.Add(iSpellEffect);
  }

  public bool IsEffectActive(Type iType)
  {
    for (int index = 0; index < this.mEffects.Count; ++index)
    {
      if (iType.IsInstanceOfType((object) this.mEffects[index]))
        return true;
    }
    return false;
  }

  public void ClearMagicks()
  {
    for (int index = 0; index < this.mEffects.Count; ++index)
    {
      if (this.mEffects[index] is SpecialAbility)
      {
        this.mEffects[index].OnRemove();
        this.mEffects.RemoveAt(index--);
      }
    }
  }

  public void ClearEffects()
  {
    for (int index = 0; index < this.mEffects.Count; ++index)
      this.mEffects[index].OnRemove();
    this.mEffects.Clear();
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime, PlayState iPlayState)
  {
    ChantSpellManager.Update(iDeltaTime);
    for (int index = 0; index < this.mEffects.Count; ++index)
    {
      IAbilityEffect mEffect = this.mEffects[index];
      mEffect.Update(iDataChannel, iDeltaTime);
      if (mEffect.IsDead)
      {
        mEffect.OnRemove();
        this.mEffects.RemoveAt(index);
        --index;
      }
    }
    Portal.Update(iPlayState);
  }

  public Elements[] GetMagickCombo(MagickType iType) => this.mCombos[(int) iType];

  public static bool Equatable(Elements[] iElements, StaticList<Spell> iSpellQue)
  {
    int num = Math.Min(iElements.Length, iSpellQue.Count);
    for (int iIndex = 0; iIndex < num; ++iIndex)
    {
      if ((iElements[iIndex] & iSpellQue[iIndex].Element) == Elements.None)
        return false;
    }
    return true;
  }
}
