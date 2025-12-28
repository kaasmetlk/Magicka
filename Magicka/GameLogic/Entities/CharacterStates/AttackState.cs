using System;
using Magicka.GameLogic.Spells;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020001C9 RID: 457
	internal class AttackState : BaseState
	{
		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06000F6E RID: 3950 RVA: 0x000605D4 File Offset: 0x0005E7D4
		public static AttackState Instance
		{
			get
			{
				if (AttackState.mSingelton == null)
				{
					lock (AttackState.mSingeltonLock)
					{
						if (AttackState.mSingelton == null)
						{
							AttackState.mSingelton = new AttackState();
						}
					}
				}
				return AttackState.mSingelton;
			}
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x00060628 File Offset: 0x0005E828
		public override void OnEnter(Character iOwner)
		{
			iOwner.Ethereal(false, 1f, 1f);
			if (!iOwner.JustCastInvisible)
			{
				iOwner.SetInvisible(0f);
			}
			else
			{
				iOwner.JustCastInvisible = false;
			}
			if (iOwner.NextAttackAnimation == Animations.None)
			{
				iOwner.GoToAnimation(Animations.idle, 0.075f);
			}
			else
			{
				iOwner.GoToAnimation(iOwner.NextAttackAnimation, 0.075f);
			}
			iOwner.NextAttackAnimation = Animations.None;
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x00060690 File Offset: 0x0005E890
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
			iOwner.CharacterBody.AllowRotate = (!iOwner.IsEntangled & (animationController.CrossFadeEnabled | iOwner.AllowAttackRotate | animationController.Time < 0.1f));
			if (iOwner.NextAttackAnimation != Animations.None && iOwner.Attacking && !animationController.CrossFadeEnabled && animationController.HasFinished)
			{
				if (iOwner.CurrentSpell != null)
				{
					iOwner.CurrentSpell.Stop(iOwner);
					iOwner.CurrentSpell = null;
				}
				if (iOwner.CurrentAnimation == Animations.attack_recoil)
				{
					iOwner.NextAttackAnimation = Animations.None;
				}
				else
				{
					iOwner.GoToAnimation(iOwner.NextAttackAnimation, 0.2f);
					iOwner.NextAttackAnimation = Animations.None;
				}
			}
			if (iOwner.NextAttackAnimation != Animations.None || (!animationController.HasFinished && (!animationController.IsLooping || animationController.CrossFadeEnabled)))
			{
				return null;
			}
			iOwner.Attacking = false;
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f || iOwner.IsEntangled)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x000607AC File Offset: 0x0005E9AC
		public override void OnExit(Character iOwner)
		{
			NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
			if (nonPlayerCharacter != null)
			{
				nonPlayerCharacter.AI.BusyAbility = null;
			}
			iOwner.Attacking = false;
			if (iOwner.CurrentSpell != null)
			{
				iOwner.CurrentSpell.Stop(iOwner);
				iOwner.CurrentSpell = null;
			}
			iOwner.CastType = CastType.None;
			iOwner.Equipment[0].Item.StopGunfire();
			iOwner.Equipment[1].Item.StopGunfire();
		}

		// Token: 0x04000E11 RID: 3601
		private static AttackState mSingelton;

		// Token: 0x04000E12 RID: 3602
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04000E13 RID: 3603
		public static readonly int SOUND_STATEATTACK = "wizard_attack".GetHashCodeCustom();
	}
}
