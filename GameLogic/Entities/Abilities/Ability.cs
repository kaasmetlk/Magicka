// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.Ability
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public abstract class Ability
{
  protected static Random sRandom = new Random();
  private int mIndex;
  protected float mCooldown;
  protected Target mTarget;
  protected Expression mFuzzyExpression;
  protected Animations[] mAnimationKeys;

  protected Ability(
    float iCooldown,
    Target iTarget,
    Expression iExpression,
    Animations[] iAnimationKeys)
  {
    this.mCooldown = iCooldown;
    this.mTarget = iTarget;
    this.mFuzzyExpression = iExpression;
    this.mAnimationKeys = iAnimationKeys;
  }

  protected Ability(ContentReader iInput, AnimationClipAction[][] iAnimations)
  {
    this.mCooldown = iInput.ReadSingle();
    this.mTarget = (Target) iInput.ReadByte();
    if (iInput.ReadBoolean())
      this.mFuzzyExpression = Expression.Read(iInput.ReadString());
    this.mAnimationKeys = new Animations[iInput.ReadInt32()];
    for (int index = 0; index < this.mAnimationKeys.Length; ++index)
      this.mAnimationKeys[index] = (Animations) Enum.Parse(typeof (Animations), iInput.ReadString(), true);
  }

  protected Ability(Ability iCloneSource)
  {
    this.mIndex = iCloneSource.mIndex;
    this.mTarget = iCloneSource.mTarget;
    this.mFuzzyExpression = iCloneSource.mFuzzyExpression;
    this.mAnimationKeys = new Animations[iCloneSource.mAnimationKeys.Length];
    iCloneSource.mAnimationKeys.CopyTo((Array) this.mAnimationKeys, 0);
  }

  public virtual float GetDesirability(ref ExpressionArguments iArgs)
  {
    if (!this.IsUseful(ref iArgs) || (double) iArgs.AI.NPC.AbilityCooldown[this.mIndex] > 0.0)
      return float.MinValue;
    return this.mFuzzyExpression != null ? this.mFuzzyExpression.GetValue(ref iArgs) : this.Desirability(ref iArgs);
  }

  protected abstract float Desirability(ref ExpressionArguments iArgs);

  protected static Type GetType(string name)
  {
    Type[] types = Assembly.GetExecutingAssembly().GetTypes();
    for (int index = 0; index < types.Length; ++index)
    {
      if (types[index].IsSubclassOf(typeof (Ability)) && types[index].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        return types[index];
    }
    return (Type) null;
  }

  public abstract void Update(Agent iAgent, float iDeltaTime);

  public bool Execute(Agent iAgent)
  {
    bool flag = this.InternalExecute(iAgent);
    if (flag)
      iAgent.BusyAbility = this;
    return flag;
  }

  public abstract bool InternalExecute(Agent iAgent);

  public virtual bool ForceExecute(Agent iAgent) => this.InternalExecute(iAgent);

  public abstract float GetMaxRange(Agent iAgent);

  public abstract float GetMinRange(Agent iAgent);

  public abstract float GetArc(Agent iAgent);

  public abstract int[] GetWeapons();

  public virtual float Cooldown => this.mCooldown;

  public int Index => this.mIndex;

  public Target Target => this.mTarget;

  public static Ability Read(int iIndex, ContentReader iInput, AnimationClipAction[][] iAnimations)
  {
    Ability ability = (Ability) Ability.GetType(iInput.ReadString()).GetConstructor(new Type[2]
    {
      typeof (ContentReader),
      typeof (AnimationClipAction[][])
    }).Invoke(new object[2]
    {
      (object) iInput,
      (object) iAnimations
    });
    ability.mIndex = iIndex;
    return ability;
  }

  public virtual bool ChooseAttackAngle(Agent iAgent, out Vector3 oDirection)
  {
    Vector3 position1 = iAgent.Owner.Position;
    Vector3 position2 = iAgent.CurrentTarget.Position;
    float num = (float) (((double) this.GetMinRange(iAgent) + (double) this.GetMaxRange(iAgent)) * 0.5) + iAgent.CurrentTarget.Radius;
    Vector3 result1;
    Vector3.Subtract(ref position1, ref position2, out result1);
    result1.Y = 0.0f;
    if ((double) result1.LengthSquared() <= 9.9999999747524271E-07)
    {
      oDirection = new Vector3();
      return false;
    }
    result1.Normalize();
    Vector3.Multiply(ref result1, num, out result1);
    Segment iSeg;
    iSeg.Origin = position2;
    GameScene currentScene = iAgent.Owner.PlayState.Level.CurrentScene;
    List<Entity> entities = iAgent.Owner.PlayState.EntityManager.GetEntities(position2, num, true);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (!(entities[index] is Character character) || (character.Faction & iAgent.Owner.Faction) != Factions.NONE && !iAgent.AttackedBy.ContainsKey(character.Handle))
      {
        entities.RemoveAt(index);
        --index;
      }
    }
    for (int index1 = 0; index1 < 3; ++index1)
    {
      Quaternion result2;
      Quaternion.CreateFromYawPitchRoll((float) (Ability.sRandom.NextDouble() * 3.1415927410125732 - 1.5707963705062866), 0.0f, 0.0f, out result2);
      Vector3.Transform(ref result1, ref result2, out iSeg.Delta);
      Vector3 vector3;
      if (!currentScene.SegmentIntersect(out float _, out vector3, out Vector3 _, iSeg))
      {
        int index2 = 0;
        while (index2 < entities.Count && !(entities[index2] as Character).SegmentIntersect(out vector3, iSeg, 0.5f))
          ++index2;
        oDirection = iSeg.Delta;
        iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
        return true;
      }
    }
    oDirection = new Vector3();
    iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
    return false;
  }

  public abstract Vector3 GetDesiredDirection(Agent iAgent);

  public virtual bool IsUseful(ref ExpressionArguments iArgs) => this.IsUseful(iArgs.AI);

  public virtual bool IsUseful(Agent iAgent) => true;

  public virtual bool InRange(Agent iAgent)
  {
    IDamageable currentTarget = iAgent.CurrentTarget;
    if (currentTarget == null)
      return false;
    float num1 = this.GetMaxRange(iAgent) + iAgent.Owner.Radius + iAgent.CurrentTarget.Radius;
    float num2 = Math.Max(this.GetMinRange(iAgent) - iAgent.CurrentTarget.Radius, 0.0f);
    float num3 = num1 * num1;
    float num4 = num2 * num2;
    Vector3 position1 = iAgent.Owner.Position with
    {
      Y = 0.0f
    };
    Vector3 position2 = currentTarget.Position with
    {
      Y = 0.0f
    };
    float result;
    Vector3.DistanceSquared(ref position1, ref position2, out result);
    return (double) result <= (double) num3 & (double) result >= (double) num4;
  }

  public virtual bool FacingTarget(Agent iAgent)
  {
    IDamageable currentTarget = iAgent.CurrentTarget;
    float arc = this.GetArc(iAgent);
    Vector3 direction = iAgent.Owner.CharacterBody.Direction;
    Vector3 position1 = iAgent.Owner.Position;
    Vector3 position2 = currentTarget.Position;
    Vector3 result;
    Vector3.Subtract(ref position2, ref position1, out result);
    result.Y = 0.0f;
    float num = result.Length();
    if ((double) num < 9.9999999747524271E-07)
      return true;
    Vector3.Divide(ref result, num, out result);
    if ((double) MagickaMath.Angle(ref direction, ref result) >= (double) arc)
      return false;
    Vector3.Subtract(ref position2, ref position1, out new Segment()
    {
      Origin = position1
    }.Delta);
    return true;
  }
}
