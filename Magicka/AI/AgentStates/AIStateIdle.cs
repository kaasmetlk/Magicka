using System;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.Gamers;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.AI.AgentStates
{
	// Token: 0x020002B6 RID: 694
	internal class AIStateIdle : IAIState
	{
		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x060014EE RID: 5358 RVA: 0x00082948 File Offset: 0x00080B48
		public static AIStateIdle Instance
		{
			get
			{
				if (AIStateIdle.sSingleton == null)
				{
					AIStateIdle.sSingleton = new AIStateIdle();
				}
				return AIStateIdle.sSingleton;
			}
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x00082960 File Offset: 0x00080B60
		public void OnEnter(IAI iOwner)
		{
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x00082962 File Offset: 0x00080B62
		public void OnExit(IAI iOwner)
		{
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x00082964 File Offset: 0x00080B64
		public void IncrementEvent(IAI iOwner)
		{
			Avatar avatar = iOwner as Avatar;
			if (NetworkManager.Instance.State != NetworkState.Offline && avatar != null && !(avatar.Player.Gamer is NetworkGamer))
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Handle = avatar.Handle;
				characterActionMessage.Action = ActionType.EventComplete;
				AIEventType eventType = iOwner.Events[iOwner.CurrentEvent].EventType;
				int currentEvent = iOwner.CurrentEvent;
				characterActionMessage.Param0I = (int)((currentEvent << 16) + eventType);
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
			iOwner.CurrentEvent++;
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x00082A04 File Offset: 0x00080C04
		public void OnExecute(IAI iOwner, float iDeltaTime)
		{
			Agent agent = iOwner as Agent;
			if (agent != null)
			{
				if ((iOwner.Owner.IsGripped | iOwner.Owner.IsEntangled) && iOwner.Owner.BreakFreeStrength > 1E-45f)
				{
					agent.PushState(AIStateBreakFree.Instance);
					return;
				}
				if (agent.Order != Order.Defend)
				{
					Vector3 vector;
					agent.GetAvoidance(out vector);
					float num = vector.Length();
					if (num > 20f)
					{
						Vector3 position = iOwner.Owner.Position;
						Vector3.Add(ref vector, ref position, out position);
						agent.WayPoint = position;
						agent.PushState(AIStateMove.Instance);
						return;
					}
				}
				switch (agent.Order)
				{
				case Order.Attack:
				{
					IDamageable damageable;
					Ability iAbility;
					agent.ChooseTarget(out damageable, out iAbility);
					if (damageable != null)
					{
						AIStateIdle.mIdleFreezePreventionCounter = 0f;
						agent.AddTarget(damageable, iAbility);
						agent.PushState(AIStateAttack.Instance);
						return;
					}
					if (agent.Owner.SpellQueue.Count > 0)
					{
						AIStateIdle.mIdleFreezePreventionCounter += iDeltaTime;
						if (AIStateIdle.mIdleFreezePreventionCounter > 2f)
						{
							AIStateIdle.mIdleFreezePreventionCounter = 0f;
							agent.Owner.SpellQueue.Clear();
						}
					}
					else
					{
						AIStateIdle.mIdleFreezePreventionCounter = 0f;
					}
					break;
				}
				case Order.Defend:
				{
					TriggerArea triggerArea;
					if (agent.Owner.PlayState.Level.CurrentScene.TryGetTriggerArea(agent.TargetArea, out triggerArea))
					{
						Vector3 position2 = agent.Owner.Position;
						Box box = triggerArea.CollisionSkin.GetPrimitiveNewWorld(0) as Box;
						Vector3 vector2;
						box.GetDistanceToPoint(out vector2, position2);
						Vector3 vector3 = vector2;
						vector3.Y = 0f;
						position2.Y = 0f;
						float num2;
						Vector3.DistanceSquared(ref position2, ref vector3, out num2);
						if (num2 >= 1f)
						{
							agent.WayPoint = vector2;
							agent.PushState(AIStateMove.Instance);
							return;
						}
					}
					Matrix matrix;
					if (agent.Owner.PlayState.Level.CurrentScene.TryGetLocator(agent.TargetArea, out matrix))
					{
						Vector3 position3 = agent.Owner.Position;
						Vector3 translation = matrix.Translation;
						Vector3 vector4 = translation;
						vector4.Y = 0f;
						position3.Y = 0f;
						float num3;
						Vector3.DistanceSquared(ref position3, ref vector4, out num3);
						if (num3 >= 1f)
						{
							agent.WayPoint = translation;
							agent.PushState(AIStateMove.Instance);
							return;
						}
					}
					IDamageable damageable2;
					Ability iAbility2;
					agent.ChooseTarget(out damageable2, out iAbility2);
					if (damageable2 != null)
					{
						agent.AddTarget(damageable2, iAbility2);
						agent.PushState(AIStateAttack.Instance);
						return;
					}
					break;
				}
				case Order.Wander:
					agent.PushState(AIStateWander.Instance);
					break;
				case Order.Panic:
					agent.PushState(AIStatePanic.Instance);
					break;
				}
			}
			if (iOwner.Events != null && iOwner.Events.Length > 0 && (iOwner.CurrentEvent < iOwner.Events.Length | iOwner.CurrentEventDelay >= 0f | float.IsNaN(iOwner.CurrentEventDelay)))
			{
				if (iOwner.CurrentEventDelay < 0f)
				{
					switch (iOwner.Events[iOwner.CurrentEvent].EventType)
					{
					case AIEventType.Move:
					{
						MoveEvent moveEvent = iOwner.Events[iOwner.CurrentEvent].MoveEvent;
						Vector3 waypoint = moveEvent.Waypoint;
						Vector3 vector5 = (iOwner is Agent && (iOwner as Agent).Leader != null) ? (iOwner as Agent).Leader.Owner.Position : iOwner.Owner.Position;
						Vector3 vector6;
						Vector3.Subtract(ref waypoint, ref vector5, out vector6);
						vector6.Y = 0f;
						float num4 = vector6.LengthSquared();
						if (num4 >= 0.25f)
						{
							iOwner.WayPoint = waypoint;
							iOwner.PushState(AIStateMove.Instance);
							return;
						}
						if (moveEvent.FixedDirection)
						{
							Vector3 direction = iOwner.Owner.Direction;
							float num5;
							Vector3.Dot(ref direction, ref moveEvent.Direction, out num5);
							if (num5 < 0.99f)
							{
								iOwner.Owner.CharacterBody.DesiredDirection = moveEvent.Direction;
								return;
							}
						}
						iOwner.CurrentEventDelay = moveEvent.Delay;
						if (moveEvent.Delay < 1E-45f && moveEvent.Trigger != 0)
						{
							iOwner.Owner.PlayState.Level.CurrentScene.ExecuteTrigger(moveEvent.Trigger, iOwner.Owner, false);
						}
						this.IncrementEvent(iOwner);
						if (iOwner.CurrentEvent < iOwner.Events.Length && iOwner.Events[iOwner.CurrentEvent].EventType == AIEventType.Move && iOwner.CurrentEventDelay <= 0f)
						{
							return;
						}
						break;
					}
					case AIEventType.Animation:
					{
						AnimationEvent animationEvent = iOwner.Events[iOwner.CurrentEvent].AnimationEvent;
						if (animationEvent.IdleAnimation != Animations.None)
						{
							iOwner.Owner.SpecialIdleAnimation = animationEvent.IdleAnimation;
						}
						iOwner.Owner.GoToAnimation(animationEvent.Animation, animationEvent.BlendTime);
						if (iOwner.Owner is NonPlayerCharacter && NetworkManager.Instance.State != NetworkState.Offline)
						{
							CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
							characterActionMessage.Handle = iOwner.Owner.Handle;
							characterActionMessage.Action = ActionType.EventAnimation;
							characterActionMessage.Param0I = (int)animationEvent.Animation;
							characterActionMessage.Param1F = animationEvent.BlendTime;
							characterActionMessage.Param2I = (int)animationEvent.IdleAnimation;
							NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
						}
						float num6 = animationEvent.Delay;
						if (num6 < 1E-45f)
						{
							num6 = float.NaN;
						}
						iOwner.CurrentEventDelay = num6;
						this.IncrementEvent(iOwner);
						break;
					}
					case AIEventType.Face:
					{
						FaceEvent faceEvent = iOwner.Events[iOwner.CurrentEvent].FaceEvent;
						Vector3 direction2 = faceEvent.Direction;
						if (faceEvent.TargetID != 0)
						{
							Entity byID = Entity.GetByID(faceEvent.TargetID);
							if (byID != null)
							{
								Vector3 position4 = iOwner.Owner.Position;
								Vector3 position5 = byID.Position;
								Vector3.Subtract(ref position5, ref position4, out direction2);
								direction2.Y = 0f;
								direction2.Normalize();
							}
							else
							{
								Matrix matrix2;
								if (!iOwner.Owner.PlayState.Level.CurrentScene.TryGetLocator(faceEvent.TargetID, out matrix2))
								{
									iOwner.CurrentEventDelay = faceEvent.Delay;
									this.IncrementEvent(iOwner);
									break;
								}
								Vector3 position6 = iOwner.Owner.Position;
								Vector3 translation2 = matrix2.Translation;
								Vector3.Subtract(ref translation2, ref position6, out direction2);
								direction2.Y = 0f;
								direction2.Normalize();
							}
						}
						iOwner.Owner.CharacterBody.DesiredDirection = direction2;
						Vector3 direction3 = iOwner.Owner.Direction;
						float num7;
						Vector3.Dot(ref direction2, ref direction3, out num7);
						if (num7 < 0.99f)
						{
							return;
						}
						iOwner.CurrentEventDelay = faceEvent.Delay;
						this.IncrementEvent(iOwner);
						break;
					}
					case AIEventType.Kill:
					{
						KillEvent killEvent = iOwner.Events[iOwner.CurrentEvent].KillEvent;
						if (killEvent.Remove)
						{
							iOwner.Owner.Terminate(true, true);
						}
						else
						{
							iOwner.Owner.Kill();
						}
						break;
					}
					case AIEventType.Loop:
					{
						LoopEvent loopEvent = iOwner.Events[iOwner.CurrentEvent].LoopEvent;
						switch (loopEvent.Type)
						{
						case LoopType.Loop:
							iOwner.CurrentEvent = 0;
							break;
						case LoopType.Reverse:
						{
							AIEvent[] events = iOwner.Events;
							if (events.Length > 2)
							{
								for (int i = 0; i < (events.Length - 2) / 2; i++)
								{
									Helper.Swap<AIEvent>(ref events[i], ref events[events.Length - 2 - i]);
								}
							}
							iOwner.CurrentEvent = 0;
							break;
						}
						}
						break;
					}
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
						{
							iOwner.Owner.PlayState.Level.CurrentScene.ExecuteTrigger(trigger, iOwner.Owner, false);
						}
					}
				}
				else
				{
					iOwner.CurrentEventDelay -= iDeltaTime;
					if (iOwner.CurrentEventDelay < 0f & iOwner.CurrentEvent > 0)
					{
						int trigger2 = iOwner.Events[iOwner.CurrentEvent - 1].MoveEvent.Trigger;
						if (trigger2 != 0)
						{
							iOwner.Owner.PlayState.Level.CurrentScene.ExecuteTrigger(trigger2, iOwner.Owner, false);
						}
					}
					if (iOwner.CurrentEvent < iOwner.Events.Length && iOwner.Events[iOwner.CurrentEvent].EventType == AIEventType.Move && iOwner.CurrentEventDelay <= 0f)
					{
						return;
					}
				}
			}
			iOwner.Owner.CharacterBody.Movement = default(Vector3);
		}

		// Token: 0x04001655 RID: 5717
		private const float mIdleFreezePreventionMaxSeconds = 2f;

		// Token: 0x04001656 RID: 5718
		private static AIStateIdle sSingleton;

		// Token: 0x04001657 RID: 5719
		private static float mIdleFreezePreventionCounter = 0f;
	}
}
