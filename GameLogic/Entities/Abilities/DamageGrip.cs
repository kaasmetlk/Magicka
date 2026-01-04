// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.DamageGrip
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public class DamageGrip(ContentReader iInput, AnimationClipAction[][] iAnimations) : Ability(iInput, iAnimations)
{
  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    return !iArgs.AI.Owner.IsGripping ? float.MinValue : (float) ((-2.0 * (double) iArgs.AI.Owner.GripDamageAccumulation / (double) iArgs.AI.Owner.HitTolerance + 1.0) * 2.0);
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    iAgent.Owner.DamageGripped(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)]);
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => 0.0f;

  public override float GetMinRange(Agent iAgent) => 0.0f;

  public override float GetArc(Agent iAgent) => 3.14159274f;

  public override int[] GetWeapons() => (int[]) null;

  public override Vector3 GetDesiredDirection(Agent iAgent) => Vector3.Backward;

  public override bool InRange(Agent iAgent) => true;

  public override bool FacingTarget(Agent iAgent) => true;

  public override bool IsUseful(Agent iAgent)
  {
    return iAgent.Owner.IsGripping && (double) iAgent.Owner.GripDamageAccumulation < (double) iAgent.Owner.HitTolerance / 2.0;
  }
}
