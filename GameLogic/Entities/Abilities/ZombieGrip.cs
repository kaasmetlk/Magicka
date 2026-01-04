// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.ZombieGrip
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

public class ZombieGrip : Ability
{
  private float mMinRange;
  private float mMaxRange;
  private float mAngle;
  private float mMaxWeight;
  private Animations mDropAnimation;

  public ZombieGrip(ContentReader iInput, AnimationClipAction[][] iAnimations)
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
    return target == null | target.IsGripped | (double) iArgs.Target.Body.Mass > (double) this.mMaxWeight | iArgs.AI.Owner.IsGripping ? float.MinValue : float.MaxValue;
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    if (iAgent.Owner.CurrentAnimation == this.mAnimationKeys[0])
    {
      if (iAgent.Owner.IsGripping)
        iAgent.Owner.Attack(this.mAnimationKeys[1], false);
    }
    else
    {
      iAgent.Owner.DropAnimation = this.mDropAnimation;
      iAgent.Owner.Attack(this.mAnimationKeys[1], false);
    }
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => this.mMaxRange;

  public override float GetMinRange(Agent iAgent) => this.mMinRange;

  public override float GetArc(Agent iAgent) => this.mAngle;

  public override int[] GetWeapons() => (int[]) null;

  public override bool IsUseful(ref ExpressionArguments iArgs)
  {
    return (!iArgs.AI.Owner.IsGripping || iArgs.AI.Owner.CurrentAnimation == this.mAnimationKeys[0]) && (double) iArgs.Target.Body.Mass <= (double) this.mMaxWeight;
  }

  public override bool IsUseful(Agent iAgent)
  {
    return (!iAgent.Owner.IsGripping || iAgent.Owner.CurrentAnimation == this.mAnimationKeys[0]) && (double) iAgent.CurrentTarget.Body.Mass <= (double) this.mMaxWeight;
  }

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
}
