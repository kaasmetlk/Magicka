// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.PickUpCharacter
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

public class PickUpCharacter : Ability
{
  private float mMaxRange;
  private float mMinRange;
  private float mAngle;
  private float mMaxWeight;
  private Animations mDropAnimation;

  public PickUpCharacter(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mMaxRange = iInput.ReadSingle();
    this.mMinRange = iInput.ReadSingle();
    this.mAngle = iInput.ReadSingle();
    this.mMaxWeight = iInput.ReadSingle();
    this.mDropAnimation = (Animations) Enum.Parse(typeof (Animations), iInput.ReadString(), true);
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    Character target = iArgs.Target as Character;
    return target == null | (double) iArgs.Target.Body.Mass > (double) this.mMaxWeight | iArgs.AI.Owner.IsGripping ? float.MinValue : FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange) + (1f - target.CharacterBody.Movement.Length());
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    iAgent.Owner.DropAnimation = this.mDropAnimation;
    iAgent.Owner.Attack(this.mAnimationKeys[0], false);
    if (this.mAnimationKeys.Length > 1)
      iAgent.Owner.DamageGripped(this.mAnimationKeys[1]);
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => this.mMaxRange;

  public override float GetMinRange(Agent iAgent) => this.mMinRange;

  public override float GetArc(Agent iAgent) => this.mAngle;

  public override int[] GetWeapons() => (int[]) null;

  public override Vector3 GetDesiredDirection(Agent iAgent)
  {
    Vector3 position1 = iAgent.CurrentTarget.Position;
    Vector3 position2 = iAgent.Owner.Position;
    Vector3 result;
    Vector3.Subtract(ref position1, ref position2, out result);
    result.Y = 0.0f;
    float num = result.Length();
    if ((double) num < 9.9999999747524271E-07)
      return Vector3.Forward;
    Vector3.Divide(ref result, num, out result);
    return result;
  }

  public override bool IsUseful(ref ExpressionArguments iArgs)
  {
    bool flag = true;
    if (iArgs.AI.Owner.IsGripping)
      flag = false;
    else if (!(iArgs.Target is Character))
      flag = false;
    else if ((double) iArgs.Target.Body.Mass > (double) this.mMaxWeight)
      flag = false;
    else if ((iArgs.Target as Character).IsSolidSelfShielded)
      flag = false;
    return flag;
  }

  public override bool IsUseful(Agent iAgent)
  {
    bool flag = true;
    if (iAgent.Owner.IsGripping)
      flag = false;
    else if (!(iAgent.CurrentTarget is Character))
      flag = false;
    else if ((double) iAgent.CurrentTarget.Body.Mass > (double) this.mMaxWeight)
      flag = false;
    else if ((iAgent.CurrentTarget as Character).IsSolidSelfShielded)
      flag = false;
    return flag;
  }

  public override bool InRange(Agent iAgent) => iAgent.Owner.IsGripping || base.InRange(iAgent);

  public override bool FacingTarget(Agent iAgent)
  {
    return iAgent.Owner.IsGripping || base.FacingTarget(iAgent);
  }
}
