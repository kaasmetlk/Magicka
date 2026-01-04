// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AgentStates.AIStateMove
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI.Arithmetics;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.AI.AgentStates;

internal class AIStateMove : IAIState
{
  private static AIStateMove sSingleton;
  private Random mRandom = new Random();

  public static AIStateMove Instance
  {
    get
    {
      if (AIStateMove.sSingleton == null)
        AIStateMove.sSingleton = new AIStateMove();
      return AIStateMove.sSingleton;
    }
  }

  public void IncrementEvent(IAI iOwner)
  {
  }

  public void OnEnter(IAI iOwner)
  {
    bool flag = true;
    iOwner.Path.Clear();
    Vector3 position1 = iOwner.Owner.Position;
    Vector3 direction = iOwner.Owner.Direction;
    if (iOwner is Agent agent1)
    {
      Agent agent = (Agent) null;
      Vector3 wayPoint1 = agent1.WayPoint;
      List<Entity> entities = iOwner.Owner.PlayState.EntityManager.GetEntities(iOwner.Owner.Position, agent1.AlertRadius * 0.333f, false);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is NonPlayerCharacter nonPlayerCharacter && nonPlayerCharacter != agent1.Owner && nonPlayerCharacter.Type == agent1.Owner.Type && nonPlayerCharacter.AI.CurrentState == AIStateMove.Instance && nonPlayerCharacter.AI.IsLeader && nonPlayerCharacter.AI.CurrentTarget == agent1.CurrentTarget)
        {
          Vector3 result1 = nonPlayerCharacter.Position;
          Vector3.Subtract(ref result1, ref position1, out result1);
          result1.Normalize();
          float result2;
          Vector3.Dot(ref direction, ref result1, out result2);
          if ((double) result2 > 0.0)
          {
            if (agent1.CurrentTarget != null)
            {
              agent = nonPlayerCharacter.AI;
            }
            else
            {
              Vector3 wayPoint2 = nonPlayerCharacter.AI.WayPoint;
              float result3;
              Vector3.DistanceSquared(ref wayPoint1, ref wayPoint2, out result3);
              if ((double) result3 <= 1.0)
                agent = nonPlayerCharacter.AI;
            }
          }
        }
      }
      iOwner.Owner.PlayState.EntityManager.ReturnEntityList(entities);
      agent1.Leader = agent;
      flag = agent1.IsLeader;
    }
    if (!flag)
      return;
    Vector3 result = iOwner.WayPoint;
    if (agent1 != null && agent1.CurrentTarget != null)
    {
      Vector3 position2 = agent1.CurrentTarget.Position;
      Vector3.Add(ref result, ref position2, out result);
    }
    iOwner.Owner.PlayState.Level.CurrentScene.NavMesh.FindShortestPath(ref position1, ref result, iOwner.Path, iOwner.MoveAbilities);
  }

  public void OnExit(IAI iOwner)
  {
    if (!(iOwner is Agent))
      return;
    iOwner.Path.Clear();
  }

  public void OnExecute(IAI iOwner, float iDTime)
  {
    if (!iOwner.Owner.CharacterBody.IsTouchingGround)
    {
      iOwner.PopState();
    }
    else
    {
      Agent agent = iOwner as Agent;
      if ((double) iOwner.CurrentStateAge >= (iOwner.Path.Count <= 0 || (iOwner.Path[0].Properties & MovementProperties.Dynamic) != MovementProperties.Dynamic ? 1.0 : 0.33300000429153442))
      {
        iOwner.PopState();
      }
      else
      {
        Vector3 position1 = iOwner.Owner.Position with
        {
          Y = 0.0f
        };
        Vector3 result1 = iOwner.WayPoint;
        if (agent != null && agent.CurrentTarget != null)
        {
          if (agent.CurrentTarget.Dead || agent.NextAbility == null || !agent.NextAbility.IsUseful(agent))
          {
            agent.PopState();
            return;
          }
          Vector3 position2 = agent.CurrentTarget.Position;
          Vector3.Add(ref result1, ref position2, out result1);
        }
        result1.Y = 0.0f;
        float result2;
        Vector3.DistanceSquared(ref position1, ref result1, out result2);
        List<PathNode> path = iOwner.Path;
        if ((double) result2 <= 0.25)
          iOwner.PopState();
        else if (path.Count == 0 & (double) result2 > 9.0 && (agent == null || agent.IsLeader))
        {
          iOwner.PopState();
        }
        else
        {
          Vector3 iDelta;
          if (agent != null && !agent.IsLeader)
          {
            if (!(agent.Leader.CurrentState is AIStateMove))
            {
              agent.PopState();
              return;
            }
            List<Entity> entities = iOwner.Owner.PlayState.EntityManager.GetEntities(iOwner.Owner.Position, agent.AlertRadius * 0.333f, false);
            for (int index = 0; index < entities.Count; ++index)
            {
              NonPlayerCharacter nonPlayerCharacter = entities[index] as NonPlayerCharacter;
              if (nonPlayerCharacter == null | nonPlayerCharacter == agent.Owner || nonPlayerCharacter.Type != agent.Owner.Type | nonPlayerCharacter.AI.CurrentTarget != agent.CurrentTarget)
                entities.RemoveAt(index--);
              else if (agent.CurrentTarget == null)
              {
                Vector3 wayPoint = nonPlayerCharacter.AI.WayPoint with
                {
                  Y = 0.0f
                };
                Vector3.DistanceSquared(ref wayPoint, ref result1, out result2);
                if ((double) result2 > 1.0)
                  entities.RemoveAt(index--);
              }
            }
            if ((double) agent.LeaderAge > 1.0)
            {
              iOwner.PopState();
              iOwner.Owner.PlayState.EntityManager.ReturnEntityList(entities);
              return;
            }
            Vector3 position3 = agent.Leader.Owner.Position;
            Vector3 result3 = agent.Leader.WayPoint;
            if (agent.Leader.CurrentTarget != null)
            {
              Vector3 position4 = agent.Leader.CurrentTarget.Position;
              Vector3.Add(ref result3, ref position4, out result3);
            }
            Vector3.DistanceSquared(ref position3, ref result3, out result2);
            if ((double) result2 <= 0.25)
            {
              iOwner.PopState();
              return;
            }
            this.FollowGroup(iDTime, agent, entities, out iDelta);
            iOwner.Owner.PlayState.EntityManager.ReturnEntityList(entities);
          }
          else
          {
            if (iOwner.Path.Count > 0)
            {
              Vector3 result4 = iOwner.Path.Count <= 1 ? result1 : path[1].Position;
              result1 = path[0].Position;
              result4.Y = 0.0f;
              result1.Y = 0.0f;
              Vector3.DistanceSquared(ref position1, ref result1, out result2);
              float result5;
              Vector3.DistanceSquared(ref position1, ref result4, out result5);
              float result6;
              for (Vector3.DistanceSquared(ref result1, ref result4, out result6); path.Count > 0 && ((double) result2 <= 0.25 || (double) result5 - (double) result6 < 0.0); Vector3.DistanceSquared(ref result1, ref result4, out result6))
              {
                path.RemoveAt(0);
                if (path.Count > 1)
                {
                  result1 = result4;
                  result4 = iOwner.Path[1].Position with
                  {
                    Y = 0.0f
                  };
                }
                else
                {
                  result4 = iOwner.WayPoint;
                  if (agent != null && agent.CurrentTarget != null)
                  {
                    Vector3 position5 = agent.CurrentTarget.Position;
                    Vector3.Add(ref result4, ref position5, out result4);
                  }
                  result4.Y = 0.0f;
                  result1 = result4;
                }
                Vector3.DistanceSquared(ref position1, ref result1, out result2);
                Vector3.DistanceSquared(ref position1, ref result4, out result5);
              }
              Vector3.Subtract(ref result1, ref position1, out iDelta);
              if (path.Count > 0 && path[0].Properties == MovementProperties.Jump)
              {
                iOwner.Owner.Jump(iDelta, 1.04719758f);
                return;
              }
            }
            else
              Vector3.Subtract(ref result1, ref position1, out iDelta);
            iDelta.Normalize();
            List<Entity> entities = iOwner.Owner.PlayState.EntityManager.GetEntities(iOwner.Owner.Position, 4f, false);
            for (int index = 0; index < entities.Count; ++index)
            {
              if (agent != null)
              {
                Barrier iTarget1 = entities[index] as Barrier;
                Shield iTarget2 = entities[index] as Shield;
                if (iTarget1 != null && iTarget1.Solid)
                {
                  Vector3 position6 = entities[index].Position;
                  Vector3 result7;
                  Vector3.Subtract(ref position6, ref position1, out result7);
                  result7.Y = 0.0f;
                  result7.Normalize();
                  float result8;
                  Vector3.Dot(ref result7, ref iDelta, out result8);
                  if ((double) result8 > 0.5)
                  {
                    if (agent.CurrentTarget is Barrier)
                    {
                      agent.PopState();
                      break;
                    }
                    ExpressionArguments oExpressionArguments;
                    ExpressionArguments.NewExpressionArguments(agent, (IDamageable) iTarget1, out oExpressionArguments);
                    Ability oAbility;
                    double num = (double) agent.ChooseAbility(ref oExpressionArguments, out oAbility);
                    agent.AddTarget((IDamageable) iTarget1, oAbility);
                    agent.PushState((IAIState) AIStateAttack.Instance);
                    break;
                  }
                }
                else if (iTarget2 != null)
                {
                  Segment iSeg;
                  iSeg.Origin = position1;
                  iSeg.Delta = agent.Owner.Direction;
                  Vector3.Multiply(ref iSeg.Delta, 4f, out iSeg.Delta);
                  if ((entities[index] as Shield).SegmentIntersect(out Vector3 _, iSeg, 0.5f))
                  {
                    ExpressionArguments oExpressionArguments;
                    ExpressionArguments.NewExpressionArguments(agent, (IDamageable) iTarget2, out oExpressionArguments);
                    Ability oAbility;
                    double num = (double) agent.ChooseAbility(ref oExpressionArguments, out oAbility);
                    agent.AddTarget((IDamageable) iTarget2, oAbility);
                    agent.PushState((IAIState) AIStateAttack.Instance);
                    break;
                  }
                }
              }
            }
            iOwner.Owner.PlayState.EntityManager.ReturnEntityList(entities);
            Vector3.Multiply(ref iDelta, 0.9f, out iDelta);
          }
          if (iOwner.Events != null && iOwner.CurrentEvent < iOwner.Events.Length && iOwner.Events[iOwner.CurrentEvent].EventType == AIEventType.Move)
            Vector3.Multiply(ref iDelta, iOwner.Events[iOwner.CurrentEvent].MoveEvent.Speed, out iDelta);
          if (agent != null)
          {
            Vector3 vector3;
            agent.GetAvoidance(out vector3);
            float d = vector3.LengthSquared();
            if ((double) d > 9.9999999747524271E-07)
            {
              Vector3 result9;
              Vector3.Divide(ref vector3, (float) Math.Sqrt((double) d), out result9);
              float result10;
              Vector3.Dot(ref iDelta, ref result9, out result10);
              float scaleFactor = MathHelper.Clamp(result10 + 0.5f, 0.0f, 1f);
              Vector3.Multiply(ref vector3, scaleFactor, out vector3);
              Vector3.Add(ref vector3, ref iDelta, out iDelta);
              position1 = agent.Owner.Position;
              Vector3 result11;
              Vector3.Add(ref position1, ref iDelta, out result11);
              Vector3 oPoint;
              double nearestPosition = (double) agent.Owner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result11, out oPoint, agent.Owner.MoveAbilities);
              Vector3.Subtract(ref oPoint, ref position1, out iDelta);
              iDelta.Y = 0.0f;
            }
          }
          iOwner.Owner.CharacterBody.Movement = iDelta;
        }
      }
    }
  }

  private unsafe void FollowGroup(
    float iDeltaTime,
    Agent iOwner,
    List<Entity> iGroup,
    out Vector3 oDir)
  {
    Vector3 position1 = iOwner.Owner.Position;
    Vector3 vector3 = new Vector3();
    Vector3 result1 = new Vector3();
    Vector3 result2 = new Vector3();
    Vector3 result3 = new Vector3();
    int num1 = Math.Min(iGroup.Count, 8);
    Vector3 result4 = iOwner.Leader.Owner.Direction;
    Vector3.Multiply(ref result4, iOwner.GroupAlignment, out result4);
    ulong num2;
    byte* numPtr = (byte*) &num2;
    for (int index1 = 0; index1 < num1; ++index1)
    {
      byte num3 = (byte) this.mRandom.Next(iGroup.Count);
      bool flag = true;
      for (int index2 = 0; index2 < index1; ++index2)
      {
        if ((int) numPtr[index2] == (int) num3)
        {
          flag = false;
          break;
        }
      }
      if (flag)
        numPtr[index1] = num3;
      else
        --index1;
    }
    for (int index3 = 0; index3 < num1; ++index3)
    {
      byte index4 = numPtr[index3];
      Vector3 position2 = (iGroup[(int) index4] as Magicka.GameLogic.Entities.Character).Position;
      Vector3 result5;
      Vector3.Subtract(ref position2, ref position1, out result5);
      float num4 = result5.Length();
      Vector3 result6;
      Vector3.Multiply(ref result5, iOwner.GroupSeparation / -num4, out result6);
      Vector3.Add(ref result6, ref result1, out result1);
      Vector3.Multiply(ref result5, iOwner.GroupCohesion, out result6);
      Vector3.Add(ref result6, ref result2, out result2);
    }
    iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + (float) ((this.mRandom.NextDouble() - 0.5) * 30.0) * iDeltaTime);
    Vector3 result7 = new Vector3();
    MathApproximation.FastSinCos(iOwner.WanderAngle, out result7.Z, out result7.X);
    Vector3 result8 = iOwner.Owner.Direction;
    Vector3.Multiply(ref result8, 1.25f, out result8);
    Vector3.Add(ref result8, ref result7, out result7);
    result7.Normalize();
    Vector3.Multiply(ref result7, iOwner.GroupWander, out result3);
    Vector3.Add(ref result4, ref result1, out oDir);
    Vector3.Add(ref result2, ref oDir, out oDir);
    Vector3.Add(ref result3, ref oDir, out oDir);
    float d = oDir.LengthSquared();
    if ((double) d > 1.0)
      Vector3.Divide(ref oDir, (float) Math.Sqrt((double) d), out oDir);
    Vector3 result9;
    Vector3.Add(ref position1, ref oDir, out result9);
    Vector3 oPoint;
    double nearestPosition = (double) iOwner.Owner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result9, out oPoint, MovementProperties.Default);
    Vector3.Subtract(ref oPoint, ref position1, out oDir);
  }
}
