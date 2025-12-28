using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020005E8 RID: 1512
	internal class LandState : BaseState
	{
		// Token: 0x17000AAA RID: 2730
		// (get) Token: 0x06002D61 RID: 11617 RVA: 0x00170854 File Offset: 0x0016EA54
		public static LandState Instance
		{
			get
			{
				if (LandState.mSingelton == null)
				{
					lock (LandState.mSingeltonLock)
					{
						if (LandState.mSingelton == null)
						{
							LandState.mSingelton = new LandState();
						}
					}
				}
				return LandState.mSingelton;
			}
		}

		// Token: 0x06002D62 RID: 11618 RVA: 0x001708A8 File Offset: 0x0016EAA8
		public override void OnEnter(Character iOwner)
		{
			iOwner.CharacterBody.AllowRotate = false;
			iOwner.CharacterBody.AllowMove = false;
			iOwner.GoToAnimation(Animations.move_jump_end, 0.13f);
		}

		// Token: 0x06002D63 RID: 11619 RVA: 0x001708D4 File Offset: 0x0016EAD4
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			if (iOwner.CharacterBody.IsTouchingGround)
			{
				if (iOwner.CurrentAnimation != Animations.move_jump_end && iOwner.CurrentAnimation != Animations.attack_recoil)
				{
					iOwner.GoToAnimation(Animations.move_jump_end, 0.1f);
				}
				else if (iOwner.AnimationController.HasFinished && !iOwner.AnimationController.CrossFadeEnabled)
				{
					if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
					{
						return IdleState.Instance;
					}
					return MoveState.Instance;
				}
			}
			else if (iOwner.IsGripped)
			{
				if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
				{
					return IdleState.Instance;
				}
				return MoveState.Instance;
			}
			return null;
		}

		// Token: 0x04003171 RID: 12657
		private static LandState mSingelton;

		// Token: 0x04003172 RID: 12658
		private static volatile object mSingeltonLock = new object();
	}
}
