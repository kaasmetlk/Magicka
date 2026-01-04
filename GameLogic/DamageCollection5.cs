// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.DamageCollection5
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.GameLogic;

public struct DamageCollection5
{
  public const int LENGTH = 5;
  public Damage A;
  public Damage B;
  public Damage C;
  public Damage D;
  public Damage E;
  private int count;

  public void MultiplyMagnitude(float iMultiplier)
  {
    this.A.Magnitude *= iMultiplier;
    this.B.Magnitude *= iMultiplier;
    this.C.Magnitude *= iMultiplier;
    this.D.Magnitude *= iMultiplier;
    this.E.Magnitude *= iMultiplier;
  }

  public void AddDamage(Damage iDamage)
  {
    switch (this.count)
    {
      case 0:
        this.A = iDamage;
        break;
      case 1:
        this.B = iDamage;
        break;
      case 2:
        this.C = iDamage;
        break;
      case 3:
        this.D = iDamage;
        break;
      case 4:
        this.E = iDamage;
        break;
      default:
        throw new Exception("DamageCollection struct is full, pull up!");
    }
    ++this.count;
  }

  public Elements GetAllElements()
  {
    return this.A.Element | this.B.Element | this.C.Element | this.D.Element | this.E.Element;
  }

  public float GetTotalMagnitude()
  {
    return this.A.Magnitude + this.B.Magnitude + this.C.Magnitude + this.D.Magnitude + this.E.Magnitude;
  }

  public static unsafe DamageCollection5 Combine(DamageCollection5 iA, DamageCollection5 iB)
  {
    DamageCollection5 damageCollection5 = new DamageCollection5();
    Damage* damagePtr1 = &iA.A;
    Damage* damagePtr2 = &iB.A;
    Damage* damagePtr3 = &damageCollection5.A;
    int index1 = 0;
    for (int index2 = 0; index2 < 5; ++index2)
    {
      Damage damage1 = damagePtr1[index2];
      if (damage1.AttackProperty != (AttackProperties) 0)
      {
        bool flag = false;
        for (int index3 = 0; index3 < 5; ++index3)
        {
          Damage damage2 = damagePtr2[index3];
          if (damage1.AttackProperty == damage2.AttackProperty & damage1.Element == damage2.Element)
          {
            flag = true;
            damagePtr3[index1].AttackProperty = damage1.AttackProperty;
            damagePtr3[index1].Element = damage1.Element;
            damagePtr3[index1].Amount = (float) (((double) damage1.Amount * (double) damage1.Magnitude + (double) damage2.Amount * (double) damage2.Magnitude) / ((double) damage1.Magnitude + (double) damage2.Magnitude));
            damagePtr3[index1].Magnitude = damage1.Magnitude + damage2.Magnitude;
            ++index1;
            damagePtr2[index3].AttackProperty = (AttackProperties) 0;
            break;
          }
        }
        if (!flag)
          damagePtr3[index1++] = damage1;
      }
    }
    for (int index4 = 0; index4 < 5 & index1 < 5; ++index4)
    {
      if (damagePtr2[index4].AttackProperty != (AttackProperties) 0)
        damagePtr3[index1++] = damagePtr2[index4];
    }
    return damageCollection5;
  }
}
