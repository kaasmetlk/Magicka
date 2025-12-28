using System;
using Magicka.AI.Arithmetics;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;

namespace Magicka.AI.AgentStates
{
	// Token: 0x0200019A RID: 410
	internal class AIStateAttack : IAIState
	{
		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000C3A RID: 3130 RVA: 0x00049BC0 File Offset: 0x00047DC0
		public static AIStateAttack Instance
		{
			get
			{
				if (AIStateAttack.sSingleton == null)
				{
					AIStateAttack.sSingleton = new AIStateAttack();
				}
				return AIStateAttack.sSingleton;
			}
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x00049BD8 File Offset: 0x00047DD8
		public void OnEnter(IAI iOwner)
		{
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x00049BDA File Offset: 0x00047DDA
		public void OnExit(IAI iOwner)
		{
			if (iOwner is Agent)
			{
				(iOwner as Agent).NPC.StopAttacking();
			}
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x00049BF4 File Offset: 0x00047DF4
		public void IncrementEvent(IAI iOwner)
		{
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x00049BF8 File Offset: 0x00047DF8
		public void OnExecute(IAI iOwner, float dTime)
		{
			Agent agent = iOwner as Agent;
			if (agent == null)
			{
				throw new NotImplementedException();
			}
			if ((iOwner.Owner.IsGripped | iOwner.Owner.IsEntangled) && iOwner.Owner.BreakFreeStrength > 1E-45f)
			{
				agent.PushState(AIStateBreakFree.Instance);
				return;
			}
			if (agent.CurrentTarget == null)
			{
				agent.PopState();
				agent.ReleaseTarget();
				return;
			}
			if (agent.BusyAbility != null)
			{
				agent.Owner.CharacterBody.Movement = default(Vector3);
				agent.Owner.CharacterBody.DesiredDirection = agent.BusyAbility.GetDesiredDirection(agent);
				return;
			}
			Vector3 vector;
			agent.GetAvoidance(out vector);
			float num = vector.Length();
			if (num > 20f)
			{
				agent.PopState();
				agent.ReleaseTarget();
				return;
			}
			Ability nextAbility = agent.NextAbility;
			if (nextAbility == null || !nextAbility.IsUseful(agent) || agent.CurrentTargetAge > 1f)
			{
				ExpressionArguments expressionArguments = default(ExpressionArguments);
				ExpressionArguments.NewExpressionArguments(agent, agent.CurrentTarget, out expressionArguments);
				agent.ChooseAbility(ref expressionArguments, out nextAbility);
				agent.NextAbility = nextAbility;
			}
			if (nextAbility == null || agent.CurrentTarget == null || agent.CurrentTarget.HitPoints <= 0f || agent.CurrentTarget.Dead || (agent.CurrentTarget is Character && ((agent.CurrentTarget as Character).IsInvisibile & !iOwner.Owner.CanSeeInvisible)))
			{
				iOwner.Owner.CharacterBody.Movement = default(Vector3);
				iOwner.PopState();
				agent.ReleaseTarget();
				return;
			}
			bool flag = true;
			if (iOwner.Owner.PlayState.Level.CurrentScene.ForceNavMesh || iOwner.Owner.ForceNavMesh)
			{
				NavMesh navMesh = iOwner.Owner.PlayState.Level.CurrentScene.NavMesh;
				Vector3 position = iOwner.Owner.Position;
				Vector3 vector2;
				num = navMesh.GetNearestPosition(ref position, out vector2, iOwner.MoveAbilities);
				flag = (num < 3f);
			}
			bool flag2 = (iOwner.Owner.PlayState.Level.CurrentScene.ForceCamera || iOwner.Owner.ForceCamera) && iOwner.Owner.CollidedWithCamera;
			if (flag2)
			{
				agent.WayPoint = default(Vector3);
				agent.PushState(AIStateMove.Instance);
				return;
			}
			if (flag && nextAbility.InRange(agent))
			{
				agent.Owner.CharacterBody.Movement = default(Vector3);
				agent.Owner.CharacterBody.DesiredDirection = nextAbility.GetDesiredDirection(agent);
				if (nextAbility.FacingTarget(agent))
				{
					if (agent.NextAbility.Execute(agent))
					{
						agent.NPC.AbilityCooldown[agent.NextAbility.Index] = agent.NextAbility.Cooldown;
						IDamageable damageable;
						agent.ChooseTarget(out damageable, out nextAbility);
						if (damageable != null)
						{
							agent.ChangeTarget(damageable, nextAbility);
							return;
						}
					}
					else if (!agent.Owner.Attacking)
					{
						Vector3 wayPoint;
						nextAbility.ChooseAttackAngle(agent, out wayPoint);
						agent.WayPoint = wayPoint;
						agent.PushState(AIStateMove.Instance);
						return;
					}
				}
			}
			else if (agent.Order != Order.Defend)
			{
				if (agent.Owner.Attacking | agent.Owner.Dashing)
				{
					agent.Owner.CharacterBody.Movement = default(Vector3);
					agent.Owner.CharacterBody.DesiredDirection = nextAbility.GetDesiredDirection(agent);
					return;
				}
				Vector3 wayPoint2;
				if (nextAbility.ChooseAttackAngle(agent, out wayPoint2))
				{
					agent.WayPoint = wayPoint2;
					agent.PushState(AIStateMove.Instance);
					return;
				}
				agent.Owner.CharacterBody.Movement = default(Vector3);
				agent.PopState();
				agent.ReleaseTarget();
				return;
			}
			else
			{
				iOwner.Owner.CharacterBody.Movement = default(Vector3);
				iOwner.PopState();
				agent.ReleaseTarget();
			}
		}

		// Token: 0x04000B63 RID: 2915
		private static AIStateAttack sSingleton;

		// Token: 0x04000B64 RID: 2916
		private Random mRandom = new Random();
	}
}
