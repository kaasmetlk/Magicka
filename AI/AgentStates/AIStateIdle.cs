// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AgentStates.AIStateIdle
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.Gamers;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.AI.AgentStates;

internal class AIStateIdle : IAIState
{
  private const float mIdleFreezePreventionMaxSeconds = 2f;
  private static AIStateIdle sSingleton;
  private static float mIdleFreezePreventionCounter = 0.0f;

  public static AIStateIdle Instance
  {
    get
    {
      if (AIStateIdle.sSingleton == null)
        AIStateIdle.sSingleton = new AIStateIdle();
      return AIStateIdle.sSingleton;
    }
  }

  public void OnEnter(IAI iOwner)
  {
  }

  public void OnExit(IAI iOwner)
  {
  }

  public void IncrementEvent(IAI iOwner)
  {
    Avatar avatar = iOwner as Avatar;
    if (NetworkManager.Instance.State != NetworkState.Offline && avatar != null && !(avatar.Player.Gamer is NetworkGamer))
    {
      CharacterActionMessage iMessage = new CharacterActionMessage();
      iMessage.Handle = avatar.Handle;
      iMessage.Action = ActionType.EventComplete;
      AIEventType eventType = iOwner.Events[iOwner.CurrentEvent].EventType;
      int currentEvent = iOwner.CurrentEvent;
      iMessage.Param0I = (int) ((currentEvent << 16 /*0x10*/) + eventType);
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref iMessage);
    }
    ++iOwner.CurrentEvent;
  }

  public void OnExecute(IAI iOwner, float iDeltaTime)
  {
    if (iOwner is Agent agent)
    {
      if (iOwner.Owner.IsGripped | iOwner.Owner.IsEntangled && (double) iOwner.Owner.BreakFreeStrength > 1.4012984643248171E-45)
      {
        agent.PushState((IAIState) AIStateBreakFree.Instance);
        return;
      }
      if (agent.Order != Order.Defend)
      {
        Vector3 oAvoidance;
        agent.GetAvoidance(out oAvoidance);
        if ((double) oAvoidance.Length() > 20.0)
        {
          Vector3 result = iOwner.Owner.Position;
          Vector3.Add(ref oAvoidance, ref result, out result);
          agent.WayPoint = result;
          agent.PushState((IAIState) AIStateMove.Instance);
          return;
        }
      }
      switch (agent.Order)
      {
        case Order.Attack:
          IDamageable oTarget1;
          Ability oAbility1;
          double num1 = (double) agent.ChooseTarget(out oTarget1, out oAbility1);
          if (oTarget1 != null)
          {
            AIStateIdle.mIdleFreezePreventionCounter = 0.0f;
            agent.AddTarget(oTarget1, oAbility1);
            agent.PushState((IAIState) AIStateAttack.Instance);
            return;
          }
          if (agent.Owner.SpellQueue.Count > 0)
          {
            AIStateIdle.mIdleFreezePreventionCounter += iDeltaTime;
            if ((double) AIStateIdle.mIdleFreezePreventionCounter > 2.0)
            {
              AIStateIdle.mIdleFreezePreventionCounter = 0.0f;
              agent.Owner.SpellQueue.Clear();
              break;
            }
            break;
          }
          AIStateIdle.mIdleFreezePreventionCounter = 0.0f;
          break;
        case Order.Defend:
          TriggerArea oArea;
          if (agent.Owner.PlayState.Level.CurrentScene.TryGetTriggerArea(agent.TargetArea, out oArea))
          {
            Vector3 position = agent.Owner.Position;
            Vector3 closestBoxPoint;
            double distanceToPoint = (double) (oArea.CollisionSkin.GetPrimitiveNewWorld(0) as Box).GetDistanceToPoint(out closestBoxPoint, position);
            Vector3 vector3 = closestBoxPoint with
            {
              Y = 0.0f
            };
            position.Y = 0.0f;
            float result;
            Vector3.DistanceSquared(ref position, ref vector3, out result);
            if ((double) result >= 1.0)
            {
              agent.WayPoint = closestBoxPoint;
              agent.PushState((IAIState) AIStateMove.Instance);
              return;
            }
          }
          Matrix oLocator;
          if (agent.Owner.PlayState.Level.CurrentScene.TryGetLocator(agent.TargetArea, out oLocator))
          {
            Vector3 position = agent.Owner.Position;
            Vector3 translation = oLocator.Translation;
            Vector3 vector3 = translation with { Y = 0.0f };
            position.Y = 0.0f;
            float result;
            Vector3.DistanceSquared(ref position, ref vector3, out result);
            if ((double) result >= 1.0)
            {
              agent.WayPoint = translation;
              agent.PushState((IAIState) AIStateMove.Instance);
              return;
            }
          }
          IDamageable oTarget2;
          Ability oAbility2;
          double num2 = (double) agent.ChooseTarget(out oTarget2, out oAbility2);
          if (oTarget2 != null)
          {
            agent.AddTarget(oTarget2, oAbility2);
            agent.PushState((IAIState) AIStateAttack.Instance);
            return;
          }
          break;
        case Order.Wander:
          agent.PushState((IAIState) AIStateWander.Instance);
          break;
        case Order.Panic:
          agent.PushState((IAIState) AIStatePanic.Instance);
          break;
      }
    }
    if (iOwner.Events != null && iOwner.Events.Length > 0 && iOwner.CurrentEvent < iOwner.Events.Length | (double) iOwner.CurrentEventDelay >= 0.0 | float.IsNaN(iOwner.CurrentEventDelay))
    {
      if ((double) iOwner.CurrentEventDelay < 0.0)
      {
        switch (iOwner.Events[iOwner.CurrentEvent].EventType)
        {
          case AIEventType.Move:
            MoveEvent moveEvent = iOwner.Events[iOwner.CurrentEvent].MoveEvent;
            Vector3 waypoint = moveEvent.Waypoint;
            Vector3 vector3_1 = !(iOwner is Agent) || (iOwner as Agent).Leader == null ? iOwner.Owner.Position : (iOwner as Agent).Leader.Owner.Position;
            Vector3 result1;
            Vector3.Subtract(ref waypoint, ref vector3_1, out result1);
            result1.Y = 0.0f;
            if ((double) result1.LengthSquared() < 0.25)
            {
              if (moveEvent.FixedDirection)
              {
                Vector3 direction = iOwner.Owner.Direction;
                float result2;
                Vector3.Dot(ref direction, ref moveEvent.Direction, out result2);
                if ((double) result2 < 0.99000000953674316)
                {
                  iOwner.Owner.CharacterBody.DesiredDirection = moveEvent.Direction;
                  return;
                }
              }
              iOwner.CurrentEventDelay = moveEvent.Delay;
              if ((double) moveEvent.Delay < 1.4012984643248171E-45 && moveEvent.Trigger != 0)
                iOwner.Owner.PlayState.Level.CurrentScene.ExecuteTrigger(moveEvent.Trigger, iOwner.Owner, false);
              this.IncrementEvent(iOwner);
              if (iOwner.CurrentEvent < iOwner.Events.Length && iOwner.Events[iOwner.CurrentEvent].EventType == AIEventType.Move && (double) iOwner.CurrentEventDelay <= 0.0)
                return;
              break;
            }
            iOwner.WayPoint = waypoint;
            iOwner.PushState((IAIState) AIStateMove.Instance);
            return;
          case AIEventType.Animation:
            AnimationEvent animationEvent = iOwner.Events[iOwner.CurrentEvent].AnimationEvent;
            if (animationEvent.IdleAnimation != Animations.None)
              iOwner.Owner.SpecialIdleAnimation = animationEvent.IdleAnimation;
            iOwner.Owner.GoToAnimation(animationEvent.Animation, animationEvent.BlendTime);
            if (iOwner.Owner is NonPlayerCharacter && NetworkManager.Instance.State != NetworkState.Offline)
              NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
              {
                Handle = iOwner.Owner.Handle,
                Action = ActionType.EventAnimation,
                Param0I = (int) animationEvent.Animation,
                Param1F = animationEvent.BlendTime,
                Param2I = (int) animationEvent.IdleAnimation
              });
            float num = animationEvent.Delay;
            if ((double) num < 1.4012984643248171E-45)
              num = float.NaN;
            iOwner.CurrentEventDelay = num;
            this.IncrementEvent(iOwner);
            break;
          case AIEventType.Face:
            FaceEvent faceEvent = iOwner.Events[iOwner.CurrentEvent].FaceEvent;
            Vector3 result3 = faceEvent.Direction;
            if (faceEvent.TargetID != 0)
            {
              Entity byId = Entity.GetByID(faceEvent.TargetID);
              if (byId != null)
              {
                Vector3 position1 = iOwner.Owner.Position;
                Vector3 position2 = byId.Position;
                Vector3.Subtract(ref position2, ref position1, out result3);
                result3.Y = 0.0f;
                result3.Normalize();
              }
              else
              {
                Matrix oLocator;
                if (iOwner.Owner.PlayState.Level.CurrentScene.TryGetLocator(faceEvent.TargetID, out oLocator))
                {
                  Vector3 position = iOwner.Owner.Position;
                  Vector3 translation = oLocator.Translation;
                  Vector3.Subtract(ref translation, ref position, out result3);
                  result3.Y = 0.0f;
                  result3.Normalize();
                }
                else
                {
                  iOwner.CurrentEventDelay = faceEvent.Delay;
                  this.IncrementEvent(iOwner);
                  break;
                }
              }
            }
            iOwner.Owner.CharacterBody.DesiredDirection = result3;
            Vector3 direction1 = iOwner.Owner.Direction;
            float result4;
            Vector3.Dot(ref result3, ref direction1, out result4);
            if ((double) result4 < 0.99000000953674316)
              return;
            iOwner.CurrentEventDelay = faceEvent.Delay;
            this.IncrementEvent(iOwner);
            break;
          case AIEventType.Kill:
            if (iOwner.Events[iOwner.CurrentEvent].KillEvent.Remove)
            {
              iOwner.Owner.Terminate(true, true);
              break;
            }
            iOwner.Owner.Kill();
            break;
          case AIEventType.Loop:
            switch (iOwner.Events[iOwner.CurrentEvent].LoopEvent.Type)
            {
              case LoopType.Loop:
                iOwner.CurrentEvent = 0;
                break;
              case LoopType.Reverse:
                AIEvent[] events = iOwner.Events;
                if (events.Length > 2)
                {
                  for (int index = 0; index < (events.Length - 2) / 2; ++index)
                    Helper.Swap<AIEvent>(ref events[index], ref events[events.Length - 2 - index]);
                }
                iOwner.CurrentEvent = 0;
                break;
            }
            break;
          default:
            throw new Exception("Invalid event type!");
        }
      }
      else if (float.IsNaN(iOwner.CurrentEventDelay) && (iOwner.Owner.AnimationController.HasFinished || iOwner.Owner.CurrentAnimation != iOwner.Events[iOwner.CurrentEvent - 1].AnimationEvent.Animation))
      {
        iOwner.CurrentEventDelay = -1f;
        if (iOwner.CurrentEvent > 0)
        {
          int trigger = iOwner.Events[iOwner.CurrentEvent - 1].MoveEvent.Trigger;
          if (trigger != 0)
            iOwner.Owner.PlayState.Level.CurrentScene.ExecuteTrigger(trigger, iOwner.Owner, false);
        }
      }
      else
      {
        iOwner.CurrentEventDelay -= iDeltaTime;
        if ((double) iOwner.CurrentEventDelay < 0.0 & iOwner.CurrentEvent > 0)
        {
          int trigger = iOwner.Events[iOwner.CurrentEvent - 1].MoveEvent.Trigger;
          if (trigger != 0)
            iOwner.Owner.PlayState.Level.CurrentScene.ExecuteTrigger(trigger, iOwner.Owner, false);
        }
        if (iOwner.CurrentEvent < iOwner.Events.Length && iOwner.Events[iOwner.CurrentEvent].EventType == AIEventType.Move && (double) iOwner.CurrentEventDelay <= 0.0)
          return;
      }
    }
    iOwner.Owner.CharacterBody.Movement = new Vector3();
  }
}
