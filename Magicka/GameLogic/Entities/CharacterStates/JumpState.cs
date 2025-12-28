using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x02000168 RID: 360
	internal class JumpState : BaseState
	{
		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06000AFB RID: 2811 RVA: 0x00042204 File Offset: 0x00040404
		public static JumpState Instance
		{
			get
			{
				if (JumpState.mSingelton == null)
				{
					lock (JumpState.mSingeltonLock)
					{
						if (JumpState.mSingelton == null)
						{
							JumpState.mSingelton = new JumpState();
						}
					}
				}
				return JumpState.mSingelton;
			}
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x00042258 File Offset: 0x00040458
		public override void OnEnter(Character iOwner)
		{
			iOwner.CharacterBody.AllowRotate = false;
			iOwner.CharacterBody.AllowMove = false;
			if (iOwner.HasAnimation(Animations.move_jump_up))
			{
				iOwner.GoToAnimation(Animations.move_jump_up, 0.2f);
				return;
			}
			iOwner.GoToAnimation(Animations.move_jump_mid, 0.2f);
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x000422AC File Offset: 0x000404AC
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if (!iOwner.CharacterBody.IsJumping)
			{
				return LandState.Instance;
			}
			if (iOwner.IsGripping)
			{
				return GrippingState.Instance;
			}
			if (iOwner.Body.Velocity.Y < 0f && iOwner.CurrentAnimation == Animations.move_jump_up && iOwner.HasAnimation(Animations.move_jump_down) && !iOwner.AnimationController.CrossFadeEnabled)
			{
				iOwner.GoToAnimation(Animations.move_jump_down, 0.3f);
			}
			return null;
		}

		// Token: 0x04000A01 RID: 2561
		private static JumpState mSingelton;

		// Token: 0x04000A02 RID: 2562
		private static volatile object mSingeltonLock = new object();
	}
}
