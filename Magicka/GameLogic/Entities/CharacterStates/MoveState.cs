using System;
using Magicka.GameLogic.Spells;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x0200057B RID: 1403
	public sealed class MoveState : BaseState
	{
		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x06002A07 RID: 10759 RVA: 0x0014AEE8 File Offset: 0x001490E8
		public static MoveState Instance
		{
			get
			{
				if (MoveState.mSingelton == null)
				{
					lock (MoveState.mSingeltonLock)
					{
						if (MoveState.mSingelton == null)
						{
							MoveState.mSingelton = new MoveState();
						}
					}
				}
				return MoveState.mSingelton;
			}
		}

		// Token: 0x06002A08 RID: 10760 RVA: 0x0014AF3C File Offset: 0x0014913C
		public override void OnEnter(Character iOwner)
		{
			iOwner.CharacterBody.AllowMove = true;
			iOwner.CharacterBody.AllowRotate = true;
		}

		// Token: 0x06002A09 RID: 10761 RVA: 0x0014AF58 File Offset: 0x00149158
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState == null)
			{
				baseState = this.UpdateHit(iOwner, iDeltaTime);
			}
			if (baseState == null)
			{
				baseState = this.UpdateActions(iOwner, iDeltaTime);
			}
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner.Dashing)
			{
				return DashState.Instance;
			}
			if (iOwner.Attacking)
			{
				return AttackState.Instance;
			}
			if (iOwner.IsBlocking)
			{
				return BlockState.Instance;
			}
			if (iOwner is Avatar && iOwner.CastType != CastType.None)
			{
				Avatar avatar = iOwner as Avatar;
				if ((iOwner.CastType == CastType.Weapon || !avatar.ChargeUnlocked || iOwner.Spell.GetSpellType() != SpellType.Projectile || iOwner.CastType != CastType.Force) && (!avatar.ChargeUnlocked || iOwner.Spell.GetSpellType() != SpellType.Push || (iOwner.CastType != CastType.Force && iOwner.CastType != CastType.Area)))
				{
					return CastState.Instance;
				}
				if (!(iOwner.CurrentState is AttackState))
				{
					return ChargeState.Instance;
				}
			}
			else
			{
				if (iOwner.ZapTimer >= 1E-45f)
				{
					return null;
				}
				if (iOwner.CharacterBody.Movement.LengthSquared() <= 0.01f)
				{
					return IdleState.Instance;
				}
				if (iOwner.HasStatus(StatusEffects.Poisoned))
				{
					iOwner.GoToAnimation(Animations.move_wnd, 0.4f);
				}
				else if (iOwner.CharacterBody.NormalizedVelocity >= 0.6f)
				{
					iOwner.GoToAnimation(Animations.move_run, 0.25f);
				}
				else if (iOwner.CharacterBody.NormalizedVelocity < 0.4f)
				{
					iOwner.GoToAnimation(Animations.move_walk, 0.25f);
				}
				else if (iOwner.CurrentAnimation != Animations.move_run & iOwner.CurrentAnimation != Animations.move_walk)
				{
					iOwner.GoToAnimation(Animations.move_walk, 0.25f);
				}
			}
			return null;
		}

		// Token: 0x06002A0A RID: 10762 RVA: 0x0014B108 File Offset: 0x00149308
		public override void OnExit(Character iOwner)
		{
			iOwner.CharacterBody.AllowMove = false;
		}

		// Token: 0x04002D7D RID: 11645
		private static MoveState mSingelton;

		// Token: 0x04002D7E RID: 11646
		private static volatile object mSingeltonLock = new object();
	}
}
