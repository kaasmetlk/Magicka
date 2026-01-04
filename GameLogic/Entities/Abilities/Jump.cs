// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.Jump
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public class Jump : Ability
{
  private float mMaxRange;
  private float mMinRange;
  private float mAngle;
  private float mElevation;

  public Jump(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mMaxRange = iInput.ReadSingle();
    this.mMinRange = iInput.ReadSingle();
    this.mAngle = iInput.ReadSingle();
    this.mElevation = iInput.ReadSingle();
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    return !(iArgs.Target as Character is Avatar) ? float.MinValue : float.MaxValue;
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    Vector3 position = iAgent.Owner.Position;
    Vector3 result1 = iAgent.CurrentTarget.Position;
    Vector3 result2 = iAgent.CurrentTarget.Body.Velocity;
    if ((double) result2.LengthSquared() > 1.4012984643248171E-45)
    {
      Vector3.Multiply(ref result2, 0.5f, out result2);
      Vector3.Add(ref result1, ref result2, out result1);
    }
    Vector3 result3;
    Vector3.Subtract(ref result1, ref position, out result3);
    float num = result3.Length();
    if ((double) num < 1.4012984643248171E-45)
      return false;
    Vector3.Divide(ref result3, num, out result3);
    float scaleFactor = MathHelper.Clamp(num * 1.1f, this.mMinRange, this.mMaxRange);
    Vector3.Multiply(ref result3, scaleFactor, out result3);
    iAgent.Owner.Jump(result3, this.mElevation);
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => this.mMaxRange;

  public override float GetMinRange(Agent iAgent) => this.mMinRange;

  public override float GetArc(Agent iAgent) => this.mAngle;

  public override int[] GetWeapons() => (int[]) null;

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

  public override bool InRange(Agent iAgent)
  {
    if (iAgent.CurrentTarget == null)
      return false;
    Vector3 position1 = iAgent.Owner.Position;
    Vector3 position2 = iAgent.CurrentTarget.Position;
    float result;
    Vector3.DistanceSquared(ref position1, ref position2, out result);
    return (double) result >= (double) this.mMinRange * (double) this.mMinRange & (double) result <= (double) this.mMaxRange * (double) this.mMaxRange;
  }
}
