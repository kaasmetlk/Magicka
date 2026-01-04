// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.MoveAbilities.Run
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.MoveAbilities;

internal class Run : MoveAbility
{
  private float mMaxRange;
  private float mMinRange;
  private float mArc;

  public Run(ContentReader iInput)
    : base(iInput)
  {
    this.mMinRange = iInput.ReadSingle();
    this.mMaxRange = iInput.ReadSingle();
    this.mArc = iInput.ReadSingle();
  }

  public override void Execute(Agent iAgent)
  {
    iAgent.Owner.Attack(this.mAnimationKeys[MagickaMath.Random.Next(this.mAnimationKeys.Length)], false);
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override float GetFuzzyWeight(Agent iAgent) => base.GetFuzzyWeight(iAgent);

  public override float GetForceMultiplier() => throw new NotImplementedException();

  public override float GetAngle() => throw new NotImplementedException();

  public override float GetMaxRange() => this.mMaxRange;

  public override float GetMinRange() => this.mMinRange;
}
