// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilityAbility
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

internal class SpecialAbilityAbility : Ability
{
  private float mMaxRange;
  private float mMinRange;
  private float mAngle;
  private int mWeapon;

  public SpecialAbilityAbility(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mMaxRange = iInput.ReadSingle();
    this.mMinRange = iInput.ReadSingle();
    this.mAngle = iInput.ReadSingle();
    this.mWeapon = iInput.ReadInt32();
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    return FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    if (this.mAnimationKeys.Length <= 0)
      iAgent.Owner.Equipment[this.mWeapon].Item.ExecuteSpecialAbility();
    else
      iAgent.Owner.Attack(this.mAnimationKeys[MagickaMath.Random.Next(this.mAnimationKeys.Length)], false);
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

  public override bool IsUseful(Agent iAgent)
  {
    return iAgent.Owner.Equipment[this.mWeapon].Item.SpecialAbilityReady;
  }

  public override bool IsUseful(ref ExpressionArguments iArgs)
  {
    return iArgs.AI.Owner.Equipment[this.mWeapon].Item.SpecialAbilityReady;
  }
}
