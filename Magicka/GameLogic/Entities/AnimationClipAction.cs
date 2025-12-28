using System;
using Magicka.GameLogic.Entities.AnimationActions;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000164 RID: 356
	public class AnimationClipAction
	{
		// Token: 0x06000AA3 RID: 2723 RVA: 0x00040B0C File Offset: 0x0003ED0C
		public AnimationClipAction(Animations iAnimation, ContentReader iInput, AnimationClipDictionary iAnimations, SkinnedModelBoneCollection iSkeleton)
		{
			string key = iInput.ReadString();
			float num = iInput.ReadSingle();
			this.mBlendTime = iInput.ReadSingle();
			bool flag = iInput.ReadBoolean();
			this.mClip = iAnimations[key];
			this.mLoopAnimation = flag;
			this.mAnimationSpeed = num;
			int num2 = iInput.ReadInt32();
			this.mActions = new AnimationAction[num2];
			for (int i = 0; i < num2; i++)
			{
				this.mActions[i] = AnimationAction.Read(iAnimation, iInput, iSkeleton);
			}
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000AA4 RID: 2724 RVA: 0x00040B90 File Offset: 0x0003ED90
		public AnimationAction[] Actions
		{
			get
			{
				return this.mActions;
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000AA5 RID: 2725 RVA: 0x00040B98 File Offset: 0x0003ED98
		public AnimationClip Clip
		{
			get
			{
				return this.mClip;
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000AA6 RID: 2726 RVA: 0x00040BA0 File Offset: 0x0003EDA0
		public float AnimationSpeed
		{
			get
			{
				return this.mAnimationSpeed;
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000AA7 RID: 2727 RVA: 0x00040BA8 File Offset: 0x0003EDA8
		public float BlendTime
		{
			get
			{
				return this.mBlendTime;
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000AA8 RID: 2728 RVA: 0x00040BB0 File Offset: 0x0003EDB0
		public bool LoopAnimation
		{
			get
			{
				return this.mLoopAnimation;
			}
		}

		// Token: 0x040009A6 RID: 2470
		private AnimationClip mClip;

		// Token: 0x040009A7 RID: 2471
		private AnimationAction[] mActions;

		// Token: 0x040009A8 RID: 2472
		private float mAnimationSpeed;

		// Token: 0x040009A9 RID: 2473
		private float mBlendTime;

		// Token: 0x040009AA RID: 2474
		private bool mLoopAnimation;
	}
}
