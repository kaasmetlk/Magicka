using System;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020000E6 RID: 230
	internal class GripAttackState : BaseState
	{
		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000718 RID: 1816 RVA: 0x00029838 File Offset: 0x00027A38
		public static GripAttackState Instance
		{
			get
			{
				if (GripAttackState.sSingelton == null)
				{
					lock (GripAttackState.sSingeltonLock)
					{
						if (GripAttackState.sSingelton == null)
						{
							GripAttackState.sSingelton = new GripAttackState();
						}
					}
				}
				return GripAttackState.sSingelton;
			}
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x0002988C File Offset: 0x00027A8C
		public override void OnEnter(Character iOwner)
		{
			iOwner.SetInvisible(0f);
			iOwner.Ethereal(false, 1f, 1f);
			iOwner.GoToAnimation(iOwner.NextGripAttackAnimation, 0.075f);
			iOwner.NextGripAttackAnimation = Animations.None;
			iOwner.CharacterBody.AllowRotate = false;
			iOwner.CharacterBody.AllowMove = false;
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x000298E8 File Offset: 0x00027AE8
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState == null)
			{
				baseState = this.UpdateHit(iOwner, iDeltaTime);
			}
			if (baseState != null)
			{
				return baseState;
			}
			AnimationController animationController = iOwner.AnimationController;
			if (!animationController.CrossFadeEnabled && animationController.HasFinished && iOwner.GripAttacking && iOwner.NextGripAttackAnimation != Animations.None)
			{
				if (iOwner.CurrentAnimation == Animations.attack_recoil)
				{
					iOwner.NextGripAttackAnimation = Animations.None;
				}
				else
				{
					iOwner.GoToAnimation(iOwner.NextGripAttackAnimation, 0.2f);
					iOwner.NextGripAttackAnimation = Animations.None;
				}
			}
			else if (iOwner.ShouldReleaseGrip)
			{
				iOwner.GripAttacking = false;
				if (iOwner.CharacterBody.Movement.Length() < 1E-45f || iOwner.IsEntangled)
				{
					return IdleState.Instance;
				}
				return MoveState.Instance;
			}
			else if (animationController.HasFinished)
			{
				NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
				if (nonPlayerCharacter != null)
				{
					nonPlayerCharacter.AI.BusyAbility = null;
				}
			}
			return null;
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x000299C0 File Offset: 0x00027BC0
		public override void OnExit(Character iOwner)
		{
			NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
			if (nonPlayerCharacter != null)
			{
				nonPlayerCharacter.AI.BusyAbility = null;
			}
			iOwner.GripAttacking = false;
			iOwner.CharacterBody.AllowMove = true;
			iOwner.CharacterBody.AllowRotate = true;
		}

		// Token: 0x040005BA RID: 1466
		private static GripAttackState sSingelton;

		// Token: 0x040005BB RID: 1467
		private static volatile object sSingeltonLock = new object();
	}
}
