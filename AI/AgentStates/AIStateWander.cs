// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AgentStates.AIStateWander
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Levels.Triggers;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.AI.AgentStates;

internal class AIStateWander : IAIState
{
  private static AIStateWander mSingelton;
  private static volatile object mSingeltonLock = new object();
  private Random mRandom = new Random();

  public static AIStateWander Instance
  {
    get
    {
      if (AIStateWander.mSingelton == null)
      {
        lock (AIStateWander.mSingeltonLock)
        {
          if (AIStateWander.mSingelton == null)
            AIStateWander.mSingelton = new AIStateWander();
        }
      }
      return AIStateWander.mSingelton;
    }
  }

  public void OnEnter(IAI iOwner)
  {
    if (!(iOwner is Agent))
      return;
    (iOwner as Agent).WanderPause = (float) (10.0 + this.mRandom.NextDouble() * 10.0);
    (iOwner as Agent).WanderTimer = 0.0f;
    (iOwner as Agent).WanderPaused = false;
  }

  public void OnExit(IAI iOwner)
  {
  }

  public void IncrementEvent(IAI iOwner)
  {
  }

  public void OnExecute(IAI iOwner, float iDeltaTime)
  {
    if (!iOwner.Owner.CharacterBody.IsTouchingGround || iOwner is Agent && (iOwner as Agent).Order != Order.Wander)
    {
      iOwner.PopState();
    }
    else
    {
      float scaleFactor = 1f;
      TriggerArea oArea = (TriggerArea) null;
      if (iOwner is Agent)
      {
        Agent agent = iOwner as Agent;
        agent.Owner.PlayState.Level.CurrentScene.TryGetTriggerArea(agent.TargetArea, out oArea);
        if (agent.WanderPaused)
        {
          agent.WanderTimer -= iDeltaTime;
          scaleFactor = 0.0f;
          if ((double) agent.WanderTimer <= 0.0)
            agent.WanderPaused = false;
        }
        else
        {
          agent.WanderTimer += iDeltaTime;
          scaleFactor = agent.WanderSpeed;
          if ((double) agent.WanderTimer >= (double) agent.WanderPause)
          {
            agent.WanderTimer = (float) (2.0 + 3.0 * this.mRandom.NextDouble());
            scaleFactor = 0.0f;
            agent.WanderPaused = true;
          }
        }
      }
      iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + (float) ((this.mRandom.NextDouble() - 0.5) * 60.0) * iDeltaTime);
      float radius = iOwner.Owner.Radius;
      Vector3 result1 = new Vector3();
      Vector3 position = iOwner.Owner.Position with
      {
        Y = 0.0f
      };
      MathApproximation.FastSinCos(iOwner.WanderAngle, out result1.Z, out result1.X);
      Vector3 result2 = iOwner.Owner.Direction;
      Vector3.Multiply(ref result2, 4f, out result2);
      Vector3.Add(ref result2, ref result1, out result1);
      Vector3 result3;
      Vector3.Add(ref position, ref result1, out result3);
      Vector3 oPoint;
      double nearestPosition = (double) iOwner.Owner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result3, out oPoint, iOwner.MoveAbilities);
      oPoint.Y = 0.0f;
      float result4 = 0.0f;
      Vector3.DistanceSquared(ref position, ref oPoint, out result4);
      if ((double) result4 > (double) radius * (double) radius)
        Vector3.Subtract(ref oPoint, ref position, out result1);
      if (oArea != null)
      {
        Box primitiveNewWorld = oArea.CollisionSkin.GetPrimitiveNewWorld(0) as Box;
        primitiveNewWorld.GetSpan(out float _, out float _, primitiveNewWorld.Orientation.Right);
        primitiveNewWorld.GetSpan(out float _, out float _, primitiveNewWorld.Orientation.Backward);
        Vector3 result5;
        Vector3.Add(ref position, ref result1, out result5);
        Vector3 closestBoxPoint;
        double distanceToPoint = (double) primitiveNewWorld.GetDistanceToPoint(out closestBoxPoint, result5);
        closestBoxPoint.Y = 0.0f;
        float result6 = 0.0f;
        Vector3.DistanceSquared(ref position, ref closestBoxPoint, out result6);
        if ((double) result6 > (double) radius * (double) radius)
        {
          Vector3.Subtract(ref closestBoxPoint, ref position, out result1);
        }
        else
        {
          iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + 3.14159274f);
          MathApproximation.FastSinCos(iOwner.WanderAngle, out result1.Z, out result1.X);
          result2 = iOwner.Owner.Direction;
          Vector3.Multiply(ref result2, 1.5f, out result2);
          Vector3.Add(ref result2, ref result1, out result1);
        }
      }
      result1.Normalize();
      if (iOwner is Agent)
        Vector3.Multiply(ref result1, scaleFactor, out result1);
      iOwner.Owner.CharacterBody.Movement = result1;
    }
  }
}
