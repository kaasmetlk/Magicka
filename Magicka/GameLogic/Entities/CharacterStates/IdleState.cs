using System;
using Magicka.GameLogic.Spells;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020002A0 RID: 672
	public sealed class IdleState : BaseState
	{
		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x0600142D RID: 5165 RVA: 0x0007E380 File Offset: 0x0007C580
		public static IdleState Instance
		{
			get
			{
				if (IdleState.mSingelton == null)
				{
					lock (IdleState.mSingeltonLock)
					{
						if (IdleState.mSingelton == null)
						{
							IdleState.mSingelton = new IdleState();
						}
					}
				}
				return IdleState.mSingelton;
			}
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x0007E3D4 File Offset: 0x0007C5D4
		public override void OnEnter(Character iOwner)
		{
			NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
			if (nonPlayerCharacter != null)
			{
				nonPlayerCharacter.AI.BusyAbility = null;
			}
			if (!iOwner.IsEntangled)
			{
				iOwner.CharacterBody.AllowMove = true;
				iOwner.CharacterBody.AllowRotate = true;
			}
			if (iOwner.NetworkAnimation != Animations.None)
			{
				iOwner.GoToAnimation(iOwner.NetworkAnimation, iOwner.NetworkAnimationBlend);
				iOwner.NetworkAnimation = Animations.None;
				return;
			}
			if (iOwner.SpecialIdleAnimation != Animations.None)
			{
				iOwner.GoToAnimation(iOwner.SpecialIdleAnimation, 0.2f);
				return;
			}
			if (iOwner.GrippedCharacter != null && !iOwner.AttachedToGripped && iOwner.HasAnimation(Animations.special1))
			{
				iOwner.GoToAnimation(Animations.special1, 0.2f);
				return;
			}
			if (iOwner.HasStatus(StatusEffects.Poisoned))
			{
				iOwner.GoToAnimation(Animations.idle_wnd, 0.2f);
				return;
			}
			if (iOwner.IsAggressive)
			{
				iOwner.GoToAnimation(Animations.idle_agg, 0.2f);
				return;
			}
			if (iOwner.CurrentAnimation == Animations.idle_agg | iOwner.CurrentAnimation == Animations.idle_wnd)
			{
				iOwner.GoToAnimation(Animations.idle, 0.3f);
				return;
			}
			iOwner.GoToAnimation(Animations.idle, 0.2f);
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x0007E4E0 File Offset: 0x0007C6E0
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
			if (iOwner.IsAlerted)
			{
				return AlarmState.Instance;
			}
			if (iOwner.CharacterBody.Moving)
			{
				return MoveState.Instance;
			}
			if (iOwner is Avatar && iOwner.CastType != CastType.None && !iOwner.PlayState.IsInCutscene)
			{
				Avatar avatar = iOwner as Avatar;
				SpellType spellType = iOwner.Spell.GetSpellType();
				if ((avatar.ChargeUnlocked && spellType == SpellType.Projectile && iOwner.CastType == CastType.Force) || (avatar.ChargeUnlocked && spellType == SpellType.Push && (iOwner.CastType == CastType.Force | iOwner.CastType == CastType.Area)))
				{
					return ChargeState.Instance;
				}
				return CastState.Instance;
			}
			else
			{
				if (iOwner.Dashing)
				{
					return DashState.Instance;
				}
				if (iOwner.Attacking)
				{
					return AttackState.Instance;
				}
				if (iOwner.GripAttacking)
				{
					return GripAttackState.Instance;
				}
				if (iOwner.IsBlocking)
				{
					return BlockState.Instance;
				}
				if (iOwner.IsGripped && iOwner.Gripper.GripperAttached)
				{
					return EntangledState.Instance;
				}
				if (iOwner.AnimationController.HasFinished && !iOwner.AnimationController.CrossFadeEnabled)
				{
					if (iOwner.GrippedCharacter != null && !iOwner.AttachedToGripped)
					{
						iOwner.GoToAnimation(Animations.special1, 0.2f);
					}
					if (iOwner.HasStatus(StatusEffects.Poisoned))
					{
						iOwner.GoToAnimation(Animations.idle_wnd, 0.3f);
					}
					else if (iOwner.IsAggressive)
					{
						iOwner.GoToAnimation(Animations.idle_agg, 0.2f);
					}
					else if (iOwner.SpecialIdleAnimation != Animations.None)
					{
						iOwner.GoToAnimation(iOwner.SpecialIdleAnimation, 0.1f);
					}
					else
					{
						iOwner.GoToAnimation(Animations.idle, 0.5f);
					}
				}
				return null;
			}
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x0007E68F File Offset: 0x0007C88F
		public override void OnExit(Character iOwner)
		{
			iOwner.CharacterBody.AllowMove = false;
			iOwner.SpecialIdleAnimation = Animations.None;
		}

		// Token: 0x040015A8 RID: 5544
		private static IdleState mSingelton;

		// Token: 0x040015A9 RID: 5545
		private static volatile object mSingeltonLock = new object();
	}
}
