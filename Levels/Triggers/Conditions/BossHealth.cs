// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.BossHealth
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public class BossHealth(GameScene iScene) : Condition(iScene)
{
  private CompareMethod mCompareMethod;
  private float mValue;

  public override bool IsMet(Character iSender)
  {
    return this.Scene.PlayState.BossFight != null && base.IsMet(iSender);
  }

  protected override bool InternalMet(Character iSender)
  {
    float normalizedHealth = this.Scene.PlayState.BossFight.NormalizedHealth;
    switch (this.mCompareMethod)
    {
      case CompareMethod.LESS:
        return (double) normalizedHealth < (double) this.mValue;
      case CompareMethod.EQUAL:
        return (double) Math.Abs(normalizedHealth - this.mValue) <= 9.9999999747524271E-07;
      case CompareMethod.GREATER:
        return (double) normalizedHealth > (double) this.mValue;
      default:
        throw new Exception("Invalid compare method!");
    }
  }

  public CompareMethod CompareMethod
  {
    get => this.mCompareMethod;
    set => this.mCompareMethod = value;
  }

  public float Value
  {
    get => this.mValue;
    set => this.mValue = value;
  }
}
