// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.GripCharacterFromBehind
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public class GripCharacterFromBehind : Ability
{
  private float mMaxRange;
  private float mMinRange;
  private float mAngle;
  private float mMaxWeight;

  public GripCharacterFromBehind(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mMaxRange = iInput.ReadSingle();
    this.mMinRange = iInput.ReadSingle();
    this.mAngle = iInput.ReadSingle();
    this.mMaxWeight = iInput.ReadSingle();
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    Character target = iArgs.Target as Character;
    if (iArgs.AI.Owner.IsGripping | !(target is Avatar) || target.IsGripped)
      return float.MinValue;
    float num = FuzzyMath.FuzzyAngle(ref iArgs.DeltaNormalized, ref iArgs.TargetDir);
    return (num * num * 2f + FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange)) * 0.333333343f;
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    Vector3 position1 = iAgent.Owner.Position;
    Vector3 position2 = iAgent.CurrentTarget.Position;
    Vector3 result;
    Vector3.Subtract(ref position2, ref position1, out result);
    float num = result.Length();
    Vector3.Divide(ref result, num, out result);
    float scaleFactor = MathHelper.Clamp(num * 1.2f, this.mMinRange, this.mMaxRange);
    Vector3.Multiply(ref result, scaleFactor, out result);
    iAgent.Owner.Jump(result, 0.7853982f);
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => this.mMaxRange;

  public override float GetMinRange(Agent iAgent) => this.mMinRange;

  public override float GetArc(Agent iAgent) => this.mAngle;

  public override int[] GetWeapons() => (int[]) null;

  public override bool InRange(Agent iAgent)
  {
    Vector3 result1 = iAgent.Owner.Position;
    Vector3 position = iAgent.CurrentTarget.Position;
    Vector3.Subtract(ref position, ref result1, out result1);
    Vector3 direction = (iAgent.CurrentTarget as Character).Direction;
    result1.Normalize();
    float result2;
    Vector3.Dot(ref direction, ref result1, out result2);
    return (double) result2 > 0.5 && base.InRange(iAgent);
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

  public override bool IsUseful(Agent iAgent)
  {
    return iAgent.CurrentTarget is Character && !iAgent.Owner.IsGripping;
  }

  public override bool ChooseAttackAngle(Agent iAgent, out Vector3 oDirection)
  {
    float scaleFactor = (float) (((double) this.mMaxRange + (double) this.mMinRange) * -0.5);
    Vector3 result = (iAgent.CurrentTarget as Character).Direction;
    Vector3.Multiply(ref result, scaleFactor, out result);
    oDirection = result;
    return true;
  }
}
