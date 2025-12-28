using System;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x02000581 RID: 1409
	internal class PanicCastState : BaseState
	{
		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x06002A20 RID: 10784 RVA: 0x0014B658 File Offset: 0x00149858
		public static PanicCastState Instance
		{
			get
			{
				if (PanicCastState.mSingelton == null)
				{
					lock (PanicCastState.mSingeltonLock)
					{
						if (PanicCastState.mSingelton == null)
						{
							PanicCastState.mSingelton = new PanicCastState();
						}
					}
				}
				return PanicCastState.mSingelton;
			}
		}

		// Token: 0x06002A21 RID: 10785 RVA: 0x0014B6AC File Offset: 0x001498AC
		private PanicCastState()
		{
		}

		// Token: 0x06002A22 RID: 10786 RVA: 0x0014B6B4 File Offset: 0x001498B4
		public override void OnEnter(Character iOwner)
		{
			iOwner.SetInvisible(0f);
			iOwner.CharacterBody.AllowMove = true;
			iOwner.CharacterBody.AllowRotate = true;
			if (iOwner.IsStumbling)
			{
				iOwner.CharacterBody.RunBackward = true;
			}
			iOwner.CastSpell(true, "");
		}

		// Token: 0x06002A23 RID: 10787 RVA: 0x0014B704 File Offset: 0x00149904
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			if ((iOwner.CurrentSpell != null && !iOwner.CurrentSpell.Active) | iOwner.CastType == CastType.Weapon)
			{
				return PanicState.Instance;
			}
			if (NetworkManager.Instance.State != NetworkState.Client | (iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + ((float)MagickaMath.Random.NextDouble() - 0.5f) * 60f * iDeltaTime);
			}
			Vector3 movement = default(Vector3);
			MathApproximation.FastSinCos(iOwner.WanderAngle, out movement.Z, out movement.X);
			Vector3 direction = iOwner.Direction;
			Vector3.Multiply(ref direction, 4f, out direction);
			if (iOwner.IsPanicing)
			{
				Vector3.Add(ref direction, ref movement, out movement);
				movement.Normalize();
				Vector3 movement2 = iOwner.CharacterBody.Movement;
				if (movement2.LengthSquared() > 1E-06f)
				{
					float panic = iOwner.Panic;
					Vector3.Multiply(ref movement, panic, out movement);
					Vector3.Multiply(ref movement2, 1f - panic, out movement2);
					Vector3.Add(ref movement2, ref movement, out movement2);
				}
				else
				{
					Vector3.Multiply(ref movement, 0.75f, out movement2);
				}
				iOwner.CharacterBody.Movement = movement2;
			}
			else if (iOwner.IsFeared)
			{
				Vector3.Add(ref direction, ref movement, out movement);
				movement.Normalize();
				Vector3 fearPosition = iOwner.FearPosition;
				Vector3 position = iOwner.Position;
				Vector3.Subtract(ref position, ref fearPosition, out fearPosition);
				fearPosition.Y = 0f;
				fearPosition.Normalize();
				Vector3.Add(ref fearPosition, ref movement, out movement);
				movement.Normalize();
				iOwner.CharacterBody.Movement = movement;
			}
			else if (iOwner.IsStumbling)
			{
				iOwner.CharacterBody.SpeedMultiplier *= 0.666f;
				Vector3.Multiply(ref direction, -1f, out direction);
				Vector3.Add(ref direction, ref movement, out movement);
				movement.Normalize();
				iOwner.CharacterBody.Movement = movement;
			}
			else
			{
				if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
				{
					return IdleState.Instance;
				}
				return MoveState.Instance;
			}
			return null;
		}

		// Token: 0x06002A24 RID: 10788 RVA: 0x0014B934 File Offset: 0x00149B34
		public override void OnExit(Character iOwner)
		{
			if (iOwner.CurrentSpell != null)
			{
				iOwner.CurrentSpell.Stop(iOwner);
				iOwner.CurrentSpell = null;
			}
			iOwner.CastType = CastType.None;
			iOwner.CharacterBody.RunBackward = false;
		}

		// Token: 0x04002D91 RID: 11665
		private static PanicCastState mSingelton;

		// Token: 0x04002D92 RID: 11666
		private static volatile object mSingeltonLock = new object();
	}
}
