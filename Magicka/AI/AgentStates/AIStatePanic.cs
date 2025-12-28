using System;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.AI.AgentStates
{
	// Token: 0x0200027B RID: 635
	internal class AIStatePanic : IAIState
	{
		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x060012C9 RID: 4809 RVA: 0x00074B67 File Offset: 0x00072D67
		public static AIStatePanic Instance
		{
			get
			{
				if (AIStatePanic.sSingleton == null)
				{
					AIStatePanic.sSingleton = new AIStatePanic();
				}
				return AIStatePanic.sSingleton;
			}
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x00074B7F File Offset: 0x00072D7F
		public void OnEnter(IAI iOwner)
		{
			iOwner.WanderAngle = ((float)this.mRandom.NextDouble() - 0.5f) * 6.2831855f;
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x00074BA0 File Offset: 0x00072DA0
		public void OnExit(IAI iOwner)
		{
			iOwner.Owner.CharacterBody.Movement = default(Vector3);
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x00074BC6 File Offset: 0x00072DC6
		public void IncrementEvent(IAI iOwner)
		{
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x00074BC8 File Offset: 0x00072DC8
		public void OnExecute(IAI iOwner, float iDeltaTime)
		{
			if (!iOwner.Owner.IsPanicing && !iOwner.Owner.IsStumbling)
			{
				iOwner.PopState();
				return;
			}
			iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + ((float)this.mRandom.NextDouble() - 0.5f) * 60f * iDeltaTime);
			Vector3 movement = default(Vector3);
			MathApproximation.FastSinCos(iOwner.WanderAngle, out movement.Z, out movement.X);
			Vector3 direction = iOwner.Owner.Direction;
			if (iOwner.Owner.CharacterBody.RunBackward)
			{
				Vector3.Multiply(ref direction, -1.25f, out direction);
			}
			else
			{
				Vector3.Multiply(ref direction, 1.25f, out direction);
			}
			Vector3.Add(ref direction, ref movement, out movement);
			movement.Normalize();
			iOwner.Owner.CharacterBody.Movement = movement;
		}

		// Token: 0x040014A9 RID: 5289
		private static AIStatePanic sSingleton;

		// Token: 0x040014AA RID: 5290
		private Random mRandom = new Random();
	}
}
