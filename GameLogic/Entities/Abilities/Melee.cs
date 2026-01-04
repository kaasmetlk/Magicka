// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.Melee
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

public class Melee : Ability
{
  private int[] mWeapons;
  private float mMaxRange;
  private float mMinRange;
  private float mArc;
  private bool mRotate;

  public Melee(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mMinRange = iInput.ReadSingle();
    this.mMaxRange = iInput.ReadSingle();
    this.mArc = iInput.ReadSingle();
    this.mWeapons = new int[iInput.ReadInt32()];
    for (int index = 0; index < this.mWeapons.Length; ++index)
      this.mWeapons[index] = iInput.ReadInt32();
    this.mRotate = iInput.ReadBoolean();
  }

  public Melee(Melee iCloneSource)
    : base((Ability) iCloneSource)
  {
    this.mWeapons = new int[iCloneSource.mWeapons.Length];
    iCloneSource.mWeapons.CopyTo((Array) this.mWeapons, 0);
    this.mMinRange = iCloneSource.mMinRange;
    this.mMaxRange = iCloneSource.mMaxRange;
    this.mArc = iCloneSource.mArc;
    this.mRotate = iCloneSource.mRotate;
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    return iArgs.AI.Owner.IsGripping ? float.MinValue : FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    iAgent.Owner.Attack(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], this.mRotate);
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => this.mMaxRange;

  public override float GetMinRange(Agent iAgent) => this.mMinRange;

  public override float GetArc(Agent iAgent) => this.mArc;

  public override int[] GetWeapons() => this.mWeapons;

  public bool AllowRotation() => this.mRotate;

  public override bool IsUseful(Agent iAgent) => !iAgent.Owner.IsGripping;

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
}
