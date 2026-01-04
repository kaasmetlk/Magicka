// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.MoveAbilities.Jump
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.MoveAbilities;

internal class Jump : MoveAbility
{
  private float mMaxRange;
  private float mMinRange;
  private float mAngle;
  private float mForceMultiplier;

  public Jump(ContentReader iInput)
    : base(iInput)
  {
    this.mAngle = 0.0f;
    this.mForceMultiplier = 0.0f;
    this.mMinRange = iInput.ReadSingle();
    this.mMaxRange = iInput.ReadSingle();
  }

  public override void Execute(Agent iAgent)
  {
    iAgent.Owner.NextAttackAnimation = this.mAnimationKeys[MagickaMath.Random.Next(this.mAnimationKeys.Length)];
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override float GetFuzzyWeight(Agent iAgent) => base.GetFuzzyWeight(iAgent);

  public override float GetMaxRange() => this.mMaxRange;

  public override float GetMinRange() => this.mMinRange;

  public override float GetAngle() => this.mAngle;

  public override float GetForceMultiplier() => this.mForceMultiplier;
}
