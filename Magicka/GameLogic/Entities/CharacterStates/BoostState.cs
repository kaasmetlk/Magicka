using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020005E9 RID: 1513
	public class BoostState : BaseState
	{
		// Token: 0x17000AAB RID: 2731
		// (get) Token: 0x06002D66 RID: 11622 RVA: 0x0017099C File Offset: 0x0016EB9C
		public static BoostState Instance
		{
			get
			{
				if (BoostState.mSingelton == null)
				{
					lock (BoostState.mSingeltonLock)
					{
						if (BoostState.mSingelton == null)
						{
							BoostState.mSingelton = new BoostState();
						}
					}
				}
				return BoostState.mSingelton;
			}
		}

		// Token: 0x06002D67 RID: 11623 RVA: 0x001709F0 File Offset: 0x0016EBF0
		public override void OnEnter(Character iOwner)
		{
			iOwner.GoToAnimation(Animations.boost, 0.2f);
		}

		// Token: 0x06002D68 RID: 11624 RVA: 0x00170A00 File Offset: 0x0016EC00
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner.IsSelfShielded && !iOwner.IsSolidSelfShielded)
			{
				iOwner.HealSelfShield(-50f * (float)iOwner.Boosts);
				iOwner.Boosts = 0;
			}
			else
			{
				Shield shield = this.ShieldToBoost(iOwner);
				if (shield != null)
				{
					shield.Damage(-100f * (float)iOwner.Boosts);
					iOwner.Boosts = 0;
				}
				if (shield == null)
				{
					if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
					{
						return IdleState.Instance;
					}
					return MoveState.Instance;
				}
			}
			if (iOwner.BoostCooldown > 0f)
			{
				return null;
			}
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x06002D69 RID: 11625 RVA: 0x00170AC9 File Offset: 0x0016ECC9
		public override void OnExit(Character iOwner)
		{
		}

		// Token: 0x06002D6A RID: 11626 RVA: 0x00170ACC File Offset: 0x0016ECCC
		public Shield ShieldToBoost(Character iOwner)
		{
			Shield result = null;
			float num = 400f;
			List<Shield> shields = iOwner.PlayState.EntityManager.Shields;
			Vector3 direction = iOwner.CharacterBody.Direction;
			direction.Y = 0f;
			Vector3 position = iOwner.Position;
			Vector3 vector = position;
			vector.Y = 0f;
			Segment seg = default(Segment);
			seg.Origin = position;
			Vector3.Multiply(ref direction, 20f, out seg.Delta);
			for (int i = 0; i < shields.Count; i++)
			{
				Shield shield = shields[i];
				float num2;
				Vector3 vector2;
				Vector3 vector3;
				if (shield.HitPoints > 0f && shield.Body.CollisionSkin.SegmentIntersect(out num2, out vector2, out vector3, seg))
				{
					vector2.Y = 0f;
					Vector3 vector4;
					Vector3.Subtract(ref vector2, ref vector, out vector4);
					float num3 = vector4.LengthSquared();
					if (num3 < num)
					{
						vector4.Normalize();
						if (num3 <= 1E-45f || MagickaMath.Angle(ref vector4, ref direction) < 0.7853982f)
						{
							num = num3;
							result = shield;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04003173 RID: 12659
		private static BoostState mSingelton;

		// Token: 0x04003174 RID: 12660
		private static volatile object mSingeltonLock = new object();
	}
}
