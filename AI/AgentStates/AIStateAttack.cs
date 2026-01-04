// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AgentStates.AIStateAttack
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI.Arithmetics;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.AI.AgentStates;

internal class AIStateAttack : IAIState
{
  private static AIStateAttack sSingleton;
  private Random mRandom = new Random();

  public static AIStateAttack Instance
  {
    get
    {
      if (AIStateAttack.sSingleton == null)
        AIStateAttack.sSingleton = new AIStateAttack();
      return AIStateAttack.sSingleton;
    }
  }

  public void OnEnter(IAI iOwner)
  {
  }

  public void OnExit(IAI iOwner)
  {
    if (!(iOwner is Agent))
      return;
    (iOwner as Agent).NPC.StopAttacking();
  }

  public void IncrementEvent(IAI iOwner)
  {
  }

  public void OnExecute(IAI iOwner, float dTime)
  {
    if (!(iOwner is Agent agent))
      throw new NotImplementedException();
    if (iOwner.Owner.IsGripped | iOwner.Owner.IsEntangled && (double) iOwner.Owner.BreakFreeStrength > 1.4012984643248171E-45)
      agent.PushState((IAIState) AIStateBreakFree.Instance);
    else if (agent.CurrentTarget == null)
    {
      agent.PopState();
      agent.ReleaseTarget();
    }
    else if (agent.BusyAbility != null)
    {
      agent.Owner.CharacterBody.Movement = new Vector3();
      agent.Owner.CharacterBody.DesiredDirection = agent.BusyAbility.GetDesiredDirection(agent);
    }
    else
    {
      Vector3 oAvoidance;
      agent.GetAvoidance(out oAvoidance);
      if ((double) oAvoidance.Length() > 20.0)
      {
        agent.PopState();
        agent.ReleaseTarget();
      }
      else
      {
        Ability oAbility = agent.NextAbility;
        if (oAbility == null || !oAbility.IsUseful(agent) || (double) agent.CurrentTargetAge > 1.0)
        {
          ExpressionArguments oExpressionArguments = new ExpressionArguments();
          ExpressionArguments.NewExpressionArguments(agent, agent.CurrentTarget, out oExpressionArguments);
          double num = (double) agent.ChooseAbility(ref oExpressionArguments, out oAbility);
          agent.NextAbility = oAbility;
        }
        if (oAbility == null || agent.CurrentTarget == null || (double) agent.CurrentTarget.HitPoints <= 0.0 || agent.CurrentTarget.Dead || agent.CurrentTarget is Character && (agent.CurrentTarget as Character).IsInvisibile & !iOwner.Owner.CanSeeInvisible)
        {
          iOwner.Owner.CharacterBody.Movement = new Vector3();
          iOwner.PopState();
          agent.ReleaseTarget();
        }
        else
        {
          bool flag = true;
          if (iOwner.Owner.PlayState.Level.CurrentScene.ForceNavMesh || iOwner.Owner.ForceNavMesh)
          {
            NavMesh navMesh = iOwner.Owner.PlayState.Level.CurrentScene.NavMesh;
            Vector3 position = iOwner.Owner.Position;
            flag = (double) navMesh.GetNearestPosition(ref position, out Vector3 _, iOwner.MoveAbilities) < 3.0;
          }
          if ((iOwner.Owner.PlayState.Level.CurrentScene.ForceCamera || iOwner.Owner.ForceCamera) && iOwner.Owner.CollidedWithCamera)
          {
            agent.WayPoint = new Vector3();
            agent.PushState((IAIState) AIStateMove.Instance);
          }
          else if (flag && oAbility.InRange(agent))
          {
            agent.Owner.CharacterBody.Movement = new Vector3();
            agent.Owner.CharacterBody.DesiredDirection = oAbility.GetDesiredDirection(agent);
            if (!oAbility.FacingTarget(agent))
              return;
            if (agent.NextAbility.Execute(agent))
            {
              agent.NPC.AbilityCooldown[agent.NextAbility.Index] = agent.NextAbility.Cooldown;
              IDamageable oTarget;
              double num = (double) agent.ChooseTarget(out oTarget, out oAbility);
              if (oTarget == null)
                return;
              agent.ChangeTarget(oTarget, oAbility);
            }
            else
            {
              if (agent.Owner.Attacking)
                return;
              Vector3 oDirection;
              oAbility.ChooseAttackAngle(agent, out oDirection);
              agent.WayPoint = oDirection;
              agent.PushState((IAIState) AIStateMove.Instance);
            }
          }
          else if (agent.Order != Order.Defend)
          {
            if (agent.Owner.Attacking | agent.Owner.Dashing)
            {
              agent.Owner.CharacterBody.Movement = new Vector3();
              agent.Owner.CharacterBody.DesiredDirection = oAbility.GetDesiredDirection(agent);
            }
            else
            {
              Vector3 oDirection;
              if (oAbility.ChooseAttackAngle(agent, out oDirection))
              {
                agent.WayPoint = oDirection;
                agent.PushState((IAIState) AIStateMove.Instance);
              }
              else
              {
                agent.Owner.CharacterBody.Movement = new Vector3();
                agent.PopState();
                agent.ReleaseTarget();
              }
            }
          }
          else
          {
            iOwner.Owner.CharacterBody.Movement = new Vector3();
            iOwner.PopState();
            agent.ReleaseTarget();
          }
        }
      }
    }
  }
}
