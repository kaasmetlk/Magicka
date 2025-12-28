using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.AI.Arithmetics;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.AI.AgentStates
{
	// Token: 0x0200031C RID: 796
	internal class AIStateMove : IAIState
	{
		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06001875 RID: 6261 RVA: 0x000A1B95 File Offset: 0x0009FD95
		public static AIStateMove Instance
		{
			get
			{
				if (AIStateMove.sSingleton == null)
				{
					AIStateMove.sSingleton = new AIStateMove();
				}
				return AIStateMove.sSingleton;
			}
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x000A1BAD File Offset: 0x0009FDAD
		public void IncrementEvent(IAI iOwner)
		{
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x000A1BB0 File Offset: 0x0009FDB0
		public void OnEnter(IAI iOwner)
		{
			bool flag = true;
			iOwner.Path.Clear();
			Vector3 position = iOwner.Owner.Position;
			Vector3 direction = iOwner.Owner.Direction;
			Agent agent = iOwner as Agent;
			if (agent != null)
			{
				Agent leader = null;
				Vector3 wayPoint = agent.WayPoint;
				List<Entity> entities = iOwner.Owner.PlayState.EntityManager.GetEntities(iOwner.Owner.Position, agent.AlertRadius * 0.333f, false);
				for (int i = 0; i < entities.Count; i++)
				{
					NonPlayerCharacter nonPlayerCharacter = entities[i] as NonPlayerCharacter;
					if (nonPlayerCharacter != null && nonPlayerCharacter != agent.Owner && nonPlayerCharacter.Type == agent.Owner.Type && nonPlayerCharacter.AI.CurrentState == AIStateMove.Instance && nonPlayerCharacter.AI.IsLeader && nonPlayerCharacter.AI.CurrentTarget == agent.CurrentTarget)
					{
						Vector3 position2 = nonPlayerCharacter.Position;
						Vector3.Subtract(ref position2, ref position, out position2);
						position2.Normalize();
						float num;
						Vector3.Dot(ref direction, ref position2, out num);
						if (num > 0f)
						{
							if (agent.CurrentTarget != null)
							{
								leader = nonPlayerCharacter.AI;
							}
							else
							{
								Vector3 wayPoint2 = nonPlayerCharacter.AI.WayPoint;
								float num2;
								Vector3.DistanceSquared(ref wayPoint, ref wayPoint2, out num2);
								if (num2 <= 1f)
								{
									leader = nonPlayerCharacter.AI;
								}
							}
						}
					}
				}
				iOwner.Owner.PlayState.EntityManager.ReturnEntityList(entities);
				agent.Leader = leader;
				flag = agent.IsLeader;
			}
			if (flag)
			{
				Vector3 wayPoint3 = iOwner.WayPoint;
				if (agent != null && agent.CurrentTarget != null)
				{
					Vector3 position3 = agent.CurrentTarget.Position;
					Vector3.Add(ref wayPoint3, ref position3, out wayPoint3);
				}
				NavMesh navMesh = iOwner.Owner.PlayState.Level.CurrentScene.NavMesh;
				navMesh.FindShortestPath(ref position, ref wayPoint3, iOwner.Path, iOwner.MoveAbilities);
			}
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x000A1DB0 File Offset: 0x0009FFB0
		public void OnExit(IAI iOwner)
		{
			Agent agent = iOwner as Agent;
			if (agent != null)
			{
				iOwner.Path.Clear();
			}
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x000A1DD4 File Offset: 0x0009FFD4
		public void OnExecute(IAI iOwner, float iDTime)
		{
			if (!iOwner.Owner.CharacterBody.IsTouchingGround)
			{
				iOwner.PopState();
				return;
			}
			Agent agent = iOwner as Agent;
			if (iOwner.CurrentStateAge >= ((iOwner.Path.Count > 0 && (iOwner.Path[0].Properties & MovementProperties.Dynamic) == MovementProperties.Dynamic) ? 0.333f : 1f))
			{
				iOwner.PopState();
				return;
			}
			Vector3 position = iOwner.Owner.Position;
			position.Y = 0f;
			Vector3 vector = iOwner.WayPoint;
			if (agent != null && agent.CurrentTarget != null)
			{
				if (agent.CurrentTarget.Dead || agent.NextAbility == null || !agent.NextAbility.IsUseful(agent))
				{
					agent.PopState();
					return;
				}
				Vector3 position2 = agent.CurrentTarget.Position;
				Vector3.Add(ref vector, ref position2, out vector);
			}
			vector.Y = 0f;
			float num;
			Vector3.DistanceSquared(ref position, ref vector, out num);
			List<PathNode> path = iOwner.Path;
			if (num <= 0.25f)
			{
				iOwner.PopState();
				return;
			}
			if ((path.Count == 0 & num > 9f) && (agent == null || agent.IsLeader))
			{
				iOwner.PopState();
				return;
			}
			Vector3 vector2;
			if (agent != null && !agent.IsLeader)
			{
				if (!(agent.Leader.CurrentState is AIStateMove))
				{
					agent.PopState();
					return;
				}
				List<Entity> entities = iOwner.Owner.PlayState.EntityManager.GetEntities(iOwner.Owner.Position, agent.AlertRadius * 0.333f, false);
				for (int i = 0; i < entities.Count; i++)
				{
					NonPlayerCharacter nonPlayerCharacter = entities[i] as NonPlayerCharacter;
					if ((nonPlayerCharacter == null | nonPlayerCharacter == agent.Owner) || (nonPlayerCharacter.Type != agent.Owner.Type | nonPlayerCharacter.AI.CurrentTarget != agent.CurrentTarget))
					{
						entities.RemoveAt(i--);
					}
					else if (agent.CurrentTarget == null)
					{
						Vector3 wayPoint = nonPlayerCharacter.AI.WayPoint;
						wayPoint.Y = 0f;
						Vector3.DistanceSquared(ref wayPoint, ref vector, out num);
						if (num > 1f)
						{
							entities.RemoveAt(i--);
						}
					}
				}
				if (agent.LeaderAge > 1f)
				{
					iOwner.PopState();
					iOwner.Owner.PlayState.EntityManager.ReturnEntityList(entities);
					return;
				}
				Vector3 position3 = agent.Leader.Owner.Position;
				Vector3 wayPoint2 = agent.Leader.WayPoint;
				if (agent.Leader.CurrentTarget != null)
				{
					Vector3 position4 = agent.Leader.CurrentTarget.Position;
					Vector3.Add(ref wayPoint2, ref position4, out wayPoint2);
				}
				Vector3.DistanceSquared(ref position3, ref wayPoint2, out num);
				if (num <= 0.25f)
				{
					iOwner.PopState();
					return;
				}
				this.FollowGroup(iDTime, agent, entities, out vector2);
				iOwner.Owner.PlayState.EntityManager.ReturnEntityList(entities);
			}
			else
			{
				if (iOwner.Path.Count > 0)
				{
					Vector3 vector3;
					if (iOwner.Path.Count > 1)
					{
						vector3 = path[1].Position;
					}
					else
					{
						vector3 = vector;
					}
					vector = path[0].Position;
					vector3.Y = 0f;
					vector.Y = 0f;
					Vector3.DistanceSquared(ref position, ref vector, out num);
					float num2;
					Vector3.DistanceSquared(ref position, ref vector3, out num2);
					float num3;
					Vector3.DistanceSquared(ref vector, ref vector3, out num3);
					while (path.Count > 0 && (num <= 0.25f || num2 - num3 < 0f))
					{
						path.RemoveAt(0);
						if (path.Count > 1)
						{
							vector = vector3;
							vector3 = iOwner.Path[1].Position;
							vector3.Y = 0f;
						}
						else
						{
							vector3 = iOwner.WayPoint;
							if (agent != null && agent.CurrentTarget != null)
							{
								Vector3 position5 = agent.CurrentTarget.Position;
								Vector3.Add(ref vector3, ref position5, out vector3);
							}
							vector3.Y = 0f;
							vector = vector3;
						}
						Vector3.DistanceSquared(ref position, ref vector, out num);
						Vector3.DistanceSquared(ref position, ref vector3, out num2);
						Vector3.DistanceSquared(ref vector, ref vector3, out num3);
					}
					Vector3.Subtract(ref vector, ref position, out vector2);
					if (path.Count > 0)
					{
						MovementProperties properties = path[0].Properties;
						if (properties == MovementProperties.Jump)
						{
							iOwner.Owner.Jump(vector2, 1.0471976f);
							return;
						}
					}
				}
				else
				{
					Vector3.Subtract(ref vector, ref position, out vector2);
				}
				vector2.Normalize();
				List<Entity> entities2 = iOwner.Owner.PlayState.EntityManager.GetEntities(iOwner.Owner.Position, 4f, false);
				for (int j = 0; j < entities2.Count; j++)
				{
					if (agent != null)
					{
						Barrier barrier = entities2[j] as Barrier;
						Shield shield = entities2[j] as Shield;
						if (barrier != null && barrier.Solid)
						{
							Vector3 position6 = entities2[j].Position;
							Vector3 vector4;
							Vector3.Subtract(ref position6, ref position, out vector4);
							vector4.Y = 0f;
							vector4.Normalize();
							float num4;
							Vector3.Dot(ref vector4, ref vector2, out num4);
							if (num4 > 0.5f)
							{
								if (agent.CurrentTarget is Barrier)
								{
									agent.PopState();
									break;
								}
								ExpressionArguments expressionArguments;
								ExpressionArguments.NewExpressionArguments(agent, barrier, out expressionArguments);
								Ability iAbility;
								agent.ChooseAbility(ref expressionArguments, out iAbility);
								agent.AddTarget(barrier, iAbility);
								agent.PushState(AIStateAttack.Instance);
								break;
							}
						}
						else if (shield != null)
						{
							Segment iSeg;
							iSeg.Origin = position;
							iSeg.Delta = agent.Owner.Direction;
							Vector3.Multiply(ref iSeg.Delta, 4f, out iSeg.Delta);
							Vector3 vector5;
							if ((entities2[j] as Shield).SegmentIntersect(out vector5, iSeg, 0.5f))
							{
								ExpressionArguments expressionArguments2;
								ExpressionArguments.NewExpressionArguments(agent, shield, out expressionArguments2);
								Ability iAbility2;
								agent.ChooseAbility(ref expressionArguments2, out iAbility2);
								agent.AddTarget(shield, iAbility2);
								agent.PushState(AIStateAttack.Instance);
								break;
							}
						}
					}
				}
				iOwner.Owner.PlayState.EntityManager.ReturnEntityList(entities2);
				Vector3.Multiply(ref vector2, 0.9f, out vector2);
			}
			if (iOwner.Events != null && iOwner.CurrentEvent < iOwner.Events.Length && iOwner.Events[iOwner.CurrentEvent].EventType == AIEventType.Move)
			{
				Vector3.Multiply(ref vector2, iOwner.Events[iOwner.CurrentEvent].MoveEvent.Speed, out vector2);
			}
			if (agent != null)
			{
				Vector3 vector6;
				agent.GetAvoidance(out vector6);
				float num5 = vector6.LengthSquared();
				if (num5 > 1E-06f)
				{
					Vector3 vector7;
					Vector3.Divide(ref vector6, (float)Math.Sqrt((double)num5), out vector7);
					float num6;
					Vector3.Dot(ref vector2, ref vector7, out num6);
					num6 = MathHelper.Clamp(num6 + 0.5f, 0f, 1f);
					Vector3.Multiply(ref vector6, num6, out vector6);
					Vector3.Add(ref vector6, ref vector2, out vector2);
					position = agent.Owner.Position;
					Vector3 vector8;
					Vector3.Add(ref position, ref vector2, out vector8);
					Vector3 vector9;
					agent.Owner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector8, out vector9, agent.Owner.MoveAbilities);
					Vector3.Subtract(ref vector9, ref position, out vector2);
					vector2.Y = 0f;
				}
			}
			iOwner.Owner.CharacterBody.Movement = vector2;
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x000A2540 File Offset: 0x000A0740
		private unsafe void FollowGroup(float iDeltaTime, Agent iOwner, List<Entity> iGroup, out Vector3 oDir)
		{
			Vector3 position = iOwner.Owner.Position;
			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);
			Vector3 vector3 = default(Vector3);
			Vector3 vector4 = default(Vector3);
			int num = Math.Min(iGroup.Count, 8);
			vector = iOwner.Leader.Owner.Direction;
			Vector3.Multiply(ref vector, iOwner.GroupAlignment, out vector);
			ulong num2;
			byte* ptr = (byte*)(&num2);
			for (int i = 0; i < num; i++)
			{
				byte b = (byte)this.mRandom.Next(iGroup.Count);
				bool flag = true;
				for (int j = 0; j < i; j++)
				{
					if (ptr[j] == b)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					ptr[i] = b;
				}
				else
				{
					i--;
				}
			}
			for (int k = 0; k < num; k++)
			{
				byte index = ptr[k];
				Vector3 position2 = (iGroup[(int)index] as Magicka.GameLogic.Entities.Character).Position;
				Vector3 vector5;
				Vector3.Subtract(ref position2, ref position, out vector5);
				float num3 = vector5.Length();
				Vector3 vector6;
				Vector3.Multiply(ref vector5, iOwner.GroupSeparation / -num3, out vector6);
				Vector3.Add(ref vector6, ref vector2, out vector2);
				Vector3.Multiply(ref vector5, iOwner.GroupCohesion, out vector6);
				Vector3.Add(ref vector6, ref vector3, out vector3);
			}
			iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + ((float)this.mRandom.NextDouble() - 0.5f) * 30f * iDeltaTime);
			Vector3 vector7 = default(Vector3);
			MathApproximation.FastSinCos(iOwner.WanderAngle, out vector7.Z, out vector7.X);
			Vector3 direction = iOwner.Owner.Direction;
			Vector3.Multiply(ref direction, 1.25f, out direction);
			Vector3.Add(ref direction, ref vector7, out vector7);
			vector7.Normalize();
			Vector3.Multiply(ref vector7, iOwner.GroupWander, out vector4);
			Vector3.Add(ref vector, ref vector2, out oDir);
			Vector3.Add(ref vector3, ref oDir, out oDir);
			Vector3.Add(ref vector4, ref oDir, out oDir);
			float num4 = oDir.LengthSquared();
			if (num4 > 1f)
			{
				Vector3.Divide(ref oDir, (float)Math.Sqrt((double)num4), out oDir);
			}
			Vector3 vector8;
			Vector3.Add(ref position, ref oDir, out vector8);
			Vector3 vector9;
			iOwner.Owner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector8, out vector9, MovementProperties.Default);
			Vector3.Subtract(ref vector9, ref position, out oDir);
		}

		// Token: 0x04001A2A RID: 6698
		private static AIStateMove sSingleton;

		// Token: 0x04001A2B RID: 6699
		private Random mRandom = new Random();
	}
}
