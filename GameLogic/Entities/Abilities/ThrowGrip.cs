// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.ThrowGrip
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.Arithmetics;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public class ThrowGrip : Ability
{
  private float mMaxRange;
  private float mMinRange;
  private float mElevation;
  private DamageCollection5 mDamages;

  public unsafe ThrowGrip(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mMaxRange = iInput.ReadSingle();
    this.mMinRange = iInput.ReadSingle();
    this.mElevation = iInput.ReadSingle();
    int num = Math.Min(iInput.ReadInt32(), 4);
    DamageCollection5 damageCollection5 = new DamageCollection5();
    Damage* damagePtr = &damageCollection5.A;
    for (int index = 0; index < num; ++index)
      damagePtr[index] = new Damage((AttackProperties) iInput.ReadInt32(), (Elements) iInput.ReadInt32(), (float) iInput.ReadInt32(), iInput.ReadSingle());
    this.mDamages = damageCollection5;
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    return !iArgs.AI.Owner.IsGripping ? float.MinValue : iArgs.AI.Owner.GripDamageAccumulation / iArgs.AI.Owner.HitTolerance;
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    iAgent.Owner.Attack(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], false);
    iAgent.Owner.GrippedCharacter.SetCollisionDamage(ref this.mDamages);
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => 0.0f;

  public override float GetMinRange(Agent iAgent) => 0.0f;

  public override float GetArc(Agent iAgent) => 3.14159274f;

  public override int[] GetWeapons() => (int[]) null;

  public override bool IsUseful(Agent iAgent) => iAgent.Owner.IsGripping;

  public override Vector3 GetDesiredDirection(Agent iAgent)
  {
    Vector3 result1 = Vector3.Forward;
    Quaternion result2;
    Quaternion.CreateFromYawPitchRoll((float) Ability.sRandom.NextDouble() * 6.28318548f, 0.0f, 0.0f, out result2);
    Vector3.Transform(ref result1, ref result2, out result1);
    return result1;
  }

  public void CalcThrow(Agent iAgent, out Vector3 oVelocity)
  {
    Vector3 position1 = iAgent.Owner.Position;
    oVelocity = iAgent.Owner.Direction;
    Vector3 result1;
    Vector3.Multiply(ref oVelocity, (float) (((double) this.mMinRange + (double) this.mMaxRange) * 0.5), out result1);
    Vector3.Add(ref result1, ref position1, out result1);
    Vector3 position2 = iAgent.Owner.GrippedCharacter.Position;
    float num1 = result1.Y - position2.Y;
    result1.Y = position2.Y = 0.0f;
    float result2;
    Vector3.Distance(ref result1, ref position2, out result2);
    float num2 = oVelocity.Y = (float) Math.Sin((double) this.mElevation);
    float num3 = (float) Math.Cos((double) this.mElevation);
    oVelocity.X *= num3;
    oVelocity.Z *= num3;
    float num4 = (float) Math.Sqrt((double) PhysicsManager.Instance.Simulator.Gravity.Y * -1.0 * (double) result2 * (double) result2 / (2.0 * ((double) result2 * (double) num2 / (double) num3 - (double) num1) * (double) num3 * (double) num3));
    if (float.IsNaN(num4) || float.IsInfinity(num4))
      return;
    Vector3.Multiply(ref oVelocity, num4, out oVelocity);
  }

  public override bool FacingTarget(Agent iAgent) => true;

  public override bool InRange(Agent iAgent) => true;
}
