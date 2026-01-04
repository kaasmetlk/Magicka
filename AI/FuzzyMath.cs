// Decompiled with JetBrains decompiler
// Type: Magicka.AI.FuzzyMath
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.AI;

public static class FuzzyMath
{
  public static float FuzzyDistanceExponential(float iDistance, float iMaxDistance)
  {
    return FuzzyMath.FuzzyDistanceExponential(iDistance, -iMaxDistance, iMaxDistance);
  }

  public static float FuzzyDistanceExponential(
    float iDistance,
    float iMinDistance,
    float iMaxDistance)
  {
    double num = ((double) iMinDistance + (double) iMaxDistance) * 0.5;
    return (float) Math.Pow(Math.Pow(Math.E, -2.3025850929940459 / (num - (double) iMaxDistance)), -Math.Abs((double) iDistance - num));
  }

  public static float FuzzyDistanceLinear(float iDistance, float iMaxDistance)
  {
    return (float) (1.0 - (double) iDistance / (double) iMaxDistance);
  }

  public static float FuzzyDistanceLinear(float iDistance, float iMinDistance, float iMaxDistance)
  {
    float num1 = (float) (((double) iMaxDistance + (double) iMinDistance) * 0.5);
    float num2 = (float) (2.0 / ((double) iMaxDistance - (double) iMinDistance));
    return (double) iDistance >= (double) num1 ? (iMaxDistance - iDistance) * num2 : (iDistance - iMinDistance) * num2;
  }

  public static float FuzzyAnger(Agent iOwner, Entity iTarget)
  {
    float y;
    iOwner.AttackedBy.TryGetValue(iTarget.Handle, out y);
    return 1f - (float) Math.Pow(0.977237221, (double) y);
  }

  public static float FuzzyAngle(ref Vector3 iA, ref Vector3 iB)
  {
    float result;
    Vector3.Dot(ref iA, ref iB, out result);
    return (float) (((double) result + 1.0) * 0.5);
  }

  public static float FuzzyEnemyDensity(Entity iEntity, Factions iFactions, float iRange)
  {
    if (iEntity.PlayState.EntityManager == null)
      return 1f;
    List<Entity> entities = iEntity.PlayState.EntityManager.GetEntities(iEntity.Position, iRange, false);
    double y = 0.0;
    for (int index = 0; index < entities.Count; ++index)
    {
      Character character = entities[index] as Character;
      if (character != null & character != iEntity && (character.Faction & iFactions) == Factions.NONE)
        ++y;
    }
    iEntity.PlayState.EntityManager.ReturnEntityList(entities);
    return 1f - (float) Math.Pow(0.5, y);
  }

  public static float FuzzyFriendlyDensity(Entity iEntity, Factions iFactions, float iRange)
  {
    if (iEntity.PlayState.EntityManager == null)
      return 1f;
    List<Entity> entities = iEntity.PlayState.EntityManager.GetEntities(iEntity.Position, iRange, false);
    double y = 0.0;
    for (int index = 0; index < entities.Count; ++index)
    {
      Character character = entities[index] as Character;
      if (character != null & character != iEntity && (character.Faction & iFactions) != Factions.NONE)
        ++y;
    }
    iEntity.PlayState.EntityManager.ReturnEntityList(entities);
    return 1f - (float) Math.Pow(0.5, y);
  }

  public static float FuzzyThreat(NonPlayerCharacter iOwner, Character iTarget, Elements iElements)
  {
    Vector3 direction = iTarget.Direction;
    Vector3 position = iOwner.Position;
    Vector3 result = iTarget.Position;
    Vector3.Subtract(ref position, ref result, out result);
    float iDistance = result.Length();
    if ((double) iDistance <= 9.9999999747524271E-07)
      return 1f;
    Vector3.Divide(ref result, iDistance, out result);
    float num1;
    if (iTarget.CurrentState is AttackState)
    {
      num1 = FuzzyMath.FuzzyAngle(ref result, ref direction) * FuzzyMath.FuzzyDistanceExponential(iDistance, 20f);
    }
    else
    {
      float num2 = Math.Max(FuzzyMath.FuzzyAngle(ref result, ref direction), FuzzyMath.FuzzyDistanceExponential(iDistance, 20f));
      Spell spell = SpellManager.Instance.Combine(iTarget.SpellQueue);
      float num3 = 0.0f;
      for (int index = 0; index < 11; ++index)
      {
        Elements iElement = (Elements) (1 << index);
        if ((iElement & iElements) == iElement)
          num3 += spell[iElement];
      }
      float x = num3 * 0.2f;
      if (iTarget.CurrentState is ChargeState || iTarget.CurrentState is CastState)
        x = (float) Math.Pow((double) x, 0.25);
      num1 = num2 * x;
    }
    return num1;
  }

  public static float FuzzyDanger(NonPlayerCharacter iCharacter, Elements iElements)
  {
    if (iCharacter.PlayState.EntityManager == null)
      return 1f;
    List<Entity> entities = iCharacter.PlayState.EntityManager.GetEntities(iCharacter.Position, iCharacter.AI.AlertRadius, true);
    float num = 0.0f;
    for (int index = 0; index < entities.Count; ++index)
    {
      Character iTarget = entities[index] as Character;
      if (iTarget != null & iTarget != iCharacter && (iTarget.Faction & iCharacter.Faction) == Factions.NONE)
        num += FuzzyMath.FuzzyThreat(iCharacter, iTarget, iElements);
    }
    float y = num + iCharacter.GetDanger();
    iCharacter.PlayState.EntityManager.ReturnEntityList(entities);
    if ((double) iCharacter.ZapTimer > 0.0 & (iElements & Elements.Lightning) != Elements.None)
      ++y;
    return 1f - (float) Math.Pow(0.25, (double) y);
  }
}
