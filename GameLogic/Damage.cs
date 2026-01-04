// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Damage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Buffs;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic;

public struct Damage(
  AttackProperties iAttackProperty,
  Elements iElement,
  float iAmount,
  float iMagnitude)
{
  public AttackProperties AttackProperty = iAttackProperty;
  public Elements Element = iElement;
  public float Amount = iAmount;
  public float Magnitude = iMagnitude;

  public static void Add(ref Damage iA, ref Damage iB, out Damage oResult)
  {
    oResult.AttackProperty = iA.AttackProperty | iB.AttackProperty;
    oResult.Element = iA.Element | iB.Element;
    oResult.Amount = (float) (((double) iA.Amount * (double) iA.Magnitude + (double) iB.Amount * (double) iB.Magnitude) / ((double) iA.Magnitude + (double) iB.Magnitude));
    oResult.Magnitude = iA.Magnitude + iB.Magnitude;
  }

  public void ApplyResistances(
    Resistance[] iResistances,
    Resistance[] iAdditionalResistances,
    IList<BuffStorage> iBuffs,
    StatusEffects iStatusEffects)
  {
    bool flag1 = (this.AttackProperty & AttackProperties.ArmourPiercing) != (AttackProperties) 0;
    bool flag2 = false;
    bool flag3 = false;
    float num = 1f;
    if (iResistances != null)
    {
      for (int index = 0; index < iResistances.Length; ++index)
      {
        Elements elements = (Elements) (1 << index);
        if ((elements & this.Element) != Elements.None)
        {
          float multiplier1 = iResistances[index].Multiplier;
          if (!flag1 || (double) multiplier1 > 1.0)
          {
            bool flag4 = (double) multiplier1 < 0.0;
            flag2 = ((flag2 ? 1 : 0) | (!flag4 ? 0 : (elements == Elements.Life ? 1 : 0))) != 0;
            flag3 |= flag4;
            num *= Math.Abs(multiplier1);
          }
          if (!flag1 || (double) iResistances[index].Modifier > 0.0)
            this.Amount = (double) this.Amount < 0.0 ? Math.Min(0.0f, this.Amount + iResistances[index].Modifier) : Math.Max(0.0f, this.Amount + iResistances[index].Modifier);
          if (iAdditionalResistances != null)
          {
            float multiplier2 = iAdditionalResistances[index].Multiplier;
            if (!flag1 || (double) multiplier2 > 1.0)
            {
              bool flag5 = (double) multiplier2 < 0.0;
              flag2 = ((flag2 ? 1 : 0) | (!flag5 ? 0 : (elements == Elements.Life ? 1 : 0))) != 0;
              flag3 |= flag5;
              num *= Math.Abs(multiplier2);
            }
            if (!flag1 || (double) iAdditionalResistances[index].Modifier > 0.0)
              this.Amount = (double) this.Amount < 0.0 ? Math.Min(0.0f, this.Amount + iAdditionalResistances[index].Modifier) : Math.Max(0.0f, this.Amount + iAdditionalResistances[index].Modifier);
          }
        }
      }
    }
    if (iBuffs != null)
    {
      for (int index = 0; index < iBuffs.Count; ++index)
      {
        if (iBuffs[index].BuffType == BuffType.Resistance && (iBuffs[index].BuffResistance.Resistance.ResistanceAgainst & this.Element) != Elements.None)
        {
          Resistance resistance = iBuffs[index].BuffResistance.Resistance;
          if (!flag1 || (double) resistance.Multiplier > 1.0)
          {
            bool flag6 = (double) resistance.Multiplier < 0.0;
            flag2 = ((flag2 ? 1 : 0) | (!flag6 ? 0 : ((resistance.ResistanceAgainst & Elements.Life) != Elements.None ? 1 : 0))) != 0;
            flag3 |= flag6;
            num *= resistance.Multiplier;
          }
          if (!flag1 || (double) resistance.Modifier > 0.0)
            this.Amount = (double) this.Amount < 0.0 ? Math.Min(0.0f, this.Amount + resistance.Modifier) : Math.Max(0.0f, this.Amount + resistance.Modifier);
        }
      }
    }
    if (flag2)
      this.Amount = Math.Abs(this.Amount);
    else if (flag3)
      this.Amount = -this.Amount;
    this.Magnitude *= num;
  }

  public void ApplyResistancesInclusive(
    Resistance[] iResistances,
    Resistance[] iAdditionalResistances,
    IList<BuffStorage> iBuffs,
    StatusEffects iStatusEffects)
  {
    float val1 = float.NegativeInfinity;
    for (int index = 0; index < iResistances.Length; ++index)
    {
      if (((Elements) (1 << index) & this.Element) != Elements.None)
      {
        double multiplier1 = (double) iResistances[index].Multiplier;
        val1 = Math.Max(val1, iResistances[index].Multiplier);
        this.Amount = (double) this.Amount < 0.0 ? Math.Min(0.0f, this.Amount + iResistances[index].Modifier) : Math.Max(0.0f, this.Amount + iResistances[index].Modifier);
        if (iAdditionalResistances != null)
        {
          double multiplier2 = (double) iAdditionalResistances[index].Multiplier;
          val1 = Math.Max(val1, iAdditionalResistances[index].Multiplier);
          this.Amount = (double) this.Amount < 0.0 ? Math.Min(0.0f, this.Amount + iAdditionalResistances[index].Modifier) : Math.Max(0.0f, this.Amount + iAdditionalResistances[index].Modifier);
        }
      }
    }
    if (iBuffs != null)
    {
      for (int index = 0; index < iBuffs.Count; ++index)
      {
        if (iBuffs[index].BuffType == BuffType.Resistance && (iBuffs[index].BuffResistance.Resistance.ResistanceAgainst & this.Element) != Elements.None)
        {
          Resistance resistance = iBuffs[index].BuffResistance.Resistance;
          val1 = Math.Max(val1, resistance.Multiplier);
        }
      }
    }
    this.Magnitude *= val1;
  }
}
