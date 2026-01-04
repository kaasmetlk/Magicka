// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.CastSpell
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.Arithmetics;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public class CastSpell : Ability
{
  private CastType mCastType;
  private Magicka.Elements[] mElements;
  private Magicka.Elements mCombinedElements;
  private float mMinRange;
  private float mMaxRange;
  private float mArc;
  private float mChantSpeed;
  private float mPower;

  public CastSpell(
    float iCooldown,
    Target iTarget,
    Expression iExpression,
    Magicka.Animations[] iAnimationKeys,
    float iMinRange,
    float iMaxRange,
    float iAngle,
    float iChantSpeed,
    float iPower)
    : base(iCooldown, iTarget, iExpression, iAnimationKeys)
  {
    this.mMinRange = iMinRange;
    this.mMaxRange = iMaxRange;
    this.mArc = iAngle;
    this.mChantSpeed = iChantSpeed;
    this.mPower = iPower;
  }

  public CastSpell(CastSpell iCloneSource)
    : base((Ability) iCloneSource)
  {
    this.mMinRange = iCloneSource.mMinRange;
    this.mMaxRange = iCloneSource.mMaxRange;
    this.mArc = iCloneSource.mArc;
    this.mChantSpeed = iCloneSource.mChantSpeed;
    this.mPower = iCloneSource.mPower;
    this.mCastType = iCloneSource.mCastType;
    this.mElements = new Magicka.Elements[iCloneSource.mElements.Length];
    iCloneSource.mElements.CopyTo((Array) this.mElements, 0);
    for (int index = 0; index < this.mElements.Length; ++index)
      this.mCombinedElements |= this.mElements[index];
  }

  public Expression FuzzyExpression
  {
    get => this.mFuzzyExpression;
    set => this.mFuzzyExpression = value;
  }

  public Magicka.Elements[] Elements
  {
    get => this.mElements;
    set => this.mElements = value;
  }

  public CastType CastType
  {
    get => this.mCastType;
    set => this.mCastType = value;
  }

  public Magicka.Animations[] Animations
  {
    get => this.mAnimationKeys;
    set => this.mAnimationKeys = value;
  }

  public new Target Target
  {
    get => this.mTarget;
    set => this.mTarget = value;
  }

  public float MinRange
  {
    get => this.mMinRange;
    set => this.mMinRange = value;
  }

  public float MaxRange
  {
    get => this.mMaxRange;
    set => this.mMaxRange = value;
  }

  public new float Cooldown
  {
    get => this.mCooldown;
    set => this.mCooldown = value;
  }

  public CastSpell(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mMinRange = iInput.ReadSingle();
    this.mMaxRange = iInput.ReadSingle();
    this.mArc = iInput.ReadSingle();
    this.mChantSpeed = iInput.ReadSingle();
    this.mPower = iInput.ReadSingle();
    this.mCastType = (CastType) iInput.ReadInt32();
    this.mElements = new Magicka.Elements[iInput.ReadInt32()];
    for (int index = 0; index < this.mElements.Length; ++index)
    {
      this.mElements[index] = (Magicka.Elements) iInput.ReadInt32();
      this.mCombinedElements |= this.mElements[index];
    }
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    throw new NotImplementedException("Cast spell must define a desirability expression!");
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
    if (iAgent.BusyAbility == iAgent.NextAbility || iAgent.NPC.CurrentSpell != null || !((double) iAgent.NPC.ChantCooldown < 0.0 & iAgent.Owner.SpellQueue.Count < this.mElements.Length) || iAgent.NPC.Attacking || (double) Vector3.DistanceSquared(iAgent.Owner.Position, iAgent.CurrentTarget.Position) > (double) this.mMaxRange * (double) this.mMaxRange * 1.25 * 1.25)
      return;
    if (this.mCastType == CastType.Weapon)
    {
      for (int index = 0; index < this.mElements.Length; ++index)
        iAgent.NPC.ConjureSpell(this.mElements[index]);
      iAgent.NPC.ChantCooldown = this.mChantSpeed;
    }
    else
    {
      iAgent.NPC.ChantCooldown = this.mChantSpeed;
      iAgent.NPC.ConjureSpell(this.mElements[iAgent.NPC.SpellQueue.Count]);
    }
  }

  public override bool InternalExecute(Agent iAgent)
  {
    if (iAgent.BusyAbility == iAgent.NextAbility || iAgent.Owner.SpellQueue.Count < this.mElements.Length || iAgent.NPC.Attacking)
      return false;
    iAgent.Owner.SpellPower = this.mPower;
    iAgent.Owner.CastType = this.mCastType;
    iAgent.NPC.ChantCooldown = this.mChantSpeed;
    iAgent.NPC.Attack(this.mAnimationKeys[Ability.sRandom.Next(this.mAnimationKeys.Length)], true);
    return true;
  }

  public override float GetMinRange(Agent iAgent) => this.mMinRange;

  public override float GetMaxRange(Agent iAgent) => this.mMaxRange;

  public override float GetArc(Agent iAgent) => this.mArc;

  public override int[] GetWeapons() => (int[]) null;

  public override Vector3 GetDesiredDirection(Agent iAgent)
  {
    if (iAgent.Owner == iAgent.CurrentTarget)
      return iAgent.Owner.Body.Orientation.Forward;
    Vector3 position1 = iAgent.Owner.Position;
    Vector3 position2 = iAgent.CurrentTarget.Position;
    Vector3 result;
    Vector3.Subtract(ref position2, ref position1, out result);
    float num = result.Length();
    if ((double) num > 9.9999999747524271E-07)
      Vector3.Divide(ref result, num, out result);
    else
      result.Z = 1f;
    return result;
  }
}
