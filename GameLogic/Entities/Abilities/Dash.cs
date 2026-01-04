// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.Dash
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public class Dash : Ability
{
  private float mMaxRange;
  private float mMinRange;
  private float mArc;
  private Vector3 mVector;

  public Dash(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mMinRange = iInput.ReadSingle();
    this.mMaxRange = iInput.ReadSingle();
    this.mArc = iInput.ReadSingle();
    this.mVector = iInput.ReadVector3();
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    if (iAgent.Owner.IsGripping)
      return false;
    iAgent.Owner.Dash(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], false);
    iAgent.Owner.NextDashAnimation = this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)];
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => this.mMaxRange;

  public override float GetMinRange(Agent iAgent) => this.mMinRange;

  public override float GetArc(Agent iAgent) => this.mArc;

  public override int[] GetWeapons() => throw new NotImplementedException();

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    return iArgs.AI.Owner.IsGripping ? float.MinValue : FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
  }

  public override Vector3 GetDesiredDirection(Agent iAgent)
  {
    Vector3 position1 = iAgent.Owner.Position;
    Vector3 position2 = iAgent.CurrentTarget.Position;
    Vector3 result;
    Vector3.Subtract(ref position2, ref position1, out result);
    float num = result.Length();
    if ((double) num <= 1.4012984643248171E-45)
      return Vector3.Forward;
    Vector3.Divide(ref result, num, out result);
    return result;
  }

  public override bool IsUseful(Agent iAgent) => !iAgent.Owner.IsGripping && base.IsUseful(iAgent);
}
