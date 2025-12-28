using System;
using JigLibX.Geometry;
using Magicka.Levels.Triggers;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.AI.AgentStates
{
	// Token: 0x02000199 RID: 409
	internal class AIStateWander : IAIState
	{
		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000C33 RID: 3123 RVA: 0x000497E4 File Offset: 0x000479E4
		public static AIStateWander Instance
		{
			get
			{
				if (AIStateWander.mSingelton == null)
				{
					lock (AIStateWander.mSingeltonLock)
					{
						if (AIStateWander.mSingelton == null)
						{
							AIStateWander.mSingelton = new AIStateWander();
						}
					}
				}
				return AIStateWander.mSingelton;
			}
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x00049838 File Offset: 0x00047A38
		public void OnEnter(IAI iOwner)
		{
			if (iOwner is Agent)
			{
				(iOwner as Agent).WanderPause = 10f + (float)this.mRandom.NextDouble() * 10f;
				(iOwner as Agent).WanderTimer = 0f;
				(iOwner as Agent).WanderPaused = false;
			}
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x0004988C File Offset: 0x00047A8C
		public void OnExit(IAI iOwner)
		{
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x0004988E File Offset: 0x00047A8E
		public void IncrementEvent(IAI iOwner)
		{
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x00049890 File Offset: 0x00047A90
		public void OnExecute(IAI iOwner, float iDeltaTime)
		{
			if (!iOwner.Owner.CharacterBody.IsTouchingGround || (iOwner is Agent && (iOwner as Agent).Order != Order.Wander))
			{
				iOwner.PopState();
				return;
			}
			float scaleFactor = 1f;
			TriggerArea triggerArea = null;
			if (iOwner is Agent)
			{
				Agent agent = iOwner as Agent;
				agent.Owner.PlayState.Level.CurrentScene.TryGetTriggerArea(agent.TargetArea, out triggerArea);
				if (agent.WanderPaused)
				{
					agent.WanderTimer -= iDeltaTime;
					scaleFactor = 0f;
					if (agent.WanderTimer <= 0f)
					{
						agent.WanderPaused = false;
					}
				}
				else
				{
					agent.WanderTimer += iDeltaTime;
					scaleFactor = agent.WanderSpeed;
					if (agent.WanderTimer >= agent.WanderPause)
					{
						agent.WanderTimer = 2f + 3f * (float)this.mRandom.NextDouble();
						scaleFactor = 0f;
						agent.WanderPaused = true;
					}
				}
			}
			iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + ((float)this.mRandom.NextDouble() - 0.5f) * 60f * iDeltaTime);
			float radius = iOwner.Owner.Radius;
			Vector3 movement = default(Vector3);
			Vector3 position = iOwner.Owner.Position;
			position.Y = 0f;
			MathApproximation.FastSinCos(iOwner.WanderAngle, out movement.Z, out movement.X);
			Vector3 direction = iOwner.Owner.Direction;
			Vector3.Multiply(ref direction, 4f, out direction);
			Vector3.Add(ref direction, ref movement, out movement);
			Vector3 vector;
			Vector3.Add(ref position, ref movement, out vector);
			Vector3 vector2;
			iOwner.Owner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out vector2, iOwner.MoveAbilities);
			vector2.Y = 0f;
			float num = 0f;
			Vector3.DistanceSquared(ref position, ref vector2, out num);
			if (num > radius * radius)
			{
				Vector3.Subtract(ref vector2, ref position, out movement);
			}
			if (triggerArea != null)
			{
				Box box = triggerArea.CollisionSkin.GetPrimitiveNewWorld(0) as Box;
				float num2;
				float num3;
				box.GetSpan(out num2, out num3, box.Orientation.Right);
				float num4;
				float num5;
				box.GetSpan(out num4, out num5, box.Orientation.Backward);
				Vector3 point;
				Vector3.Add(ref position, ref movement, out point);
				Vector3 vector3;
				box.GetDistanceToPoint(out vector3, point);
				vector3.Y = 0f;
				float num6 = 0f;
				Vector3.DistanceSquared(ref position, ref vector3, out num6);
				if (num6 > radius * radius)
				{
					Vector3.Subtract(ref vector3, ref position, out movement);
				}
				else
				{
					iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + 3.1415927f);
					MathApproximation.FastSinCos(iOwner.WanderAngle, out movement.Z, out movement.X);
					direction = iOwner.Owner.Direction;
					Vector3.Multiply(ref direction, 1.5f, out direction);
					Vector3.Add(ref direction, ref movement, out movement);
				}
			}
			movement.Normalize();
			if (iOwner is Agent)
			{
				Vector3.Multiply(ref movement, scaleFactor, out movement);
			}
			iOwner.Owner.CharacterBody.Movement = movement;
		}

		// Token: 0x04000B60 RID: 2912
		private static AIStateWander mSingelton;

		// Token: 0x04000B61 RID: 2913
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04000B62 RID: 2914
		private Random mRandom = new Random();
	}
}
