using System;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020003B1 RID: 945
	internal class PanicState : BaseState
	{
		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x06001D14 RID: 7444 RVA: 0x000CE8F8 File Offset: 0x000CCAF8
		public static PanicState Instance
		{
			get
			{
				if (PanicState.mSingelton == null)
				{
					lock (PanicState.mSingeltonLock)
					{
						if (PanicState.mSingelton == null)
						{
							PanicState.mSingelton = new PanicState();
						}
					}
				}
				return PanicState.mSingelton;
			}
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x000CE94C File Offset: 0x000CCB4C
		public override void OnEnter(Character iOwner)
		{
			iOwner.SetInvisible(0f);
			iOwner.CharacterBody.AllowMove = true;
			iOwner.CharacterBody.AllowRotate = true;
			iOwner.ReleaseAttachedCharacter();
			if (iOwner.IsStumbling)
			{
				iOwner.CharacterBody.RunBackward = true;
				iOwner.GoToAnimation(Animations.move_stumble, 0.2f);
				return;
			}
			iOwner.GoToAnimation(Animations.move_panic, 0.2f);
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x000CE9B8 File Offset: 0x000CCBB8
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner is Avatar && iOwner.CastType != CastType.None)
			{
				return PanicCastState.Instance;
			}
			if (iOwner.IsEntangled)
			{
				return EntangledState.Instance;
			}
			if (NetworkManager.Instance.State != NetworkState.Client | (iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + ((float)MagickaMath.Random.NextDouble() - 0.5f) * 60f * iDeltaTime);
			}
			Vector3 movement = default(Vector3);
			MathApproximation.FastSinCos(iOwner.WanderAngle, out movement.Z, out movement.X);
			Vector3 direction = iOwner.Direction;
			Vector3.Multiply(ref direction, 4f, out direction);
			Vector3.Add(ref direction, ref movement, out movement);
			movement.Normalize();
			Vector3 movement2;
			if (iOwner is Avatar)
			{
				movement2 = (iOwner as Avatar).DesiredInputDirection;
			}
			else
			{
				movement2 = iOwner.CharacterBody.Movement;
			}
			if (iOwner.IsPanicing)
			{
				iOwner.CharacterBody.SpeedMultiplier *= 0.8f;
				if (movement2.LengthSquared() > 1E-06f)
				{
					float panic = iOwner.Panic;
					Vector3.Multiply(ref movement, panic, out movement);
					Vector3.Multiply(ref movement2, 1f - panic, out movement2);
					Vector3.Add(ref movement2, ref movement, out movement2);
					movement2.Normalize();
					iOwner.CharacterBody.Movement = movement2;
				}
				else
				{
					iOwner.CharacterBody.Movement = movement;
				}
			}
			else if (iOwner.IsFeared)
			{
				iOwner.CharacterBody.SpeedMultiplier *= 0.8f;
				Vector3 fearPosition = iOwner.FearPosition;
				Vector3 position = iOwner.Position;
				Vector3.Subtract(ref position, ref fearPosition, out fearPosition);
				fearPosition.Y = 0f;
				fearPosition.X += 0.0001f;
				fearPosition.Normalize();
				if (movement2.LengthSquared() > 1E-06f)
				{
					Vector3.Multiply(ref fearPosition, 0.5f, out fearPosition);
					Vector3.Multiply(ref movement2, 0.5f, out movement2);
					Vector3.Add(ref movement2, ref fearPosition, out fearPosition);
					fearPosition.Normalize();
					iOwner.CharacterBody.Movement = fearPosition;
				}
				else
				{
					iOwner.CharacterBody.Movement = fearPosition;
				}
			}
			else
			{
				if (!iOwner.IsStumbling)
				{
					return IdleState.Instance;
				}
				iOwner.CharacterBody.SpeedMultiplier *= 0.6f;
				Vector3.Multiply(ref direction, -1f, out direction);
				Vector3.Add(ref direction, ref movement, out movement);
				movement.Normalize();
				iOwner.CharacterBody.Movement = movement;
			}
			return null;
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x000CEC5B File Offset: 0x000CCE5B
		public override void OnExit(Character iOwner)
		{
			iOwner.CharacterBody.RunBackward = false;
		}

		// Token: 0x04001FB7 RID: 8119
		private static PanicState mSingelton;

		// Token: 0x04001FB8 RID: 8120
		private static volatile object mSingeltonLock = new object();
	}
}
