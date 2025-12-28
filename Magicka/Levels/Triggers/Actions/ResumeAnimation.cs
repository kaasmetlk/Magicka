using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020004F8 RID: 1272
	public class ResumeAnimation : Action
	{
		// Token: 0x0600259D RID: 9629 RVA: 0x00111493 File Offset: 0x0010F693
		public ResumeAnimation(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x001114B0 File Offset: 0x0010F6B0
		protected override void Execute()
		{
			AnimatedLevelPart animatedLevelPart = base.GameScene.LevelModel.GetAnimatedLevelPart(this.mAnimationID[0]);
			for (int i = 1; i < this.mAnimationID.Length; i++)
			{
				animatedLevelPart = animatedLevelPart.GetChild(this.mAnimationID[i]);
			}
			animatedLevelPart.Resume(this.mChildren, this.mLength, this.mSpeed, this.mLoop);
		}

		// Token: 0x0600259F RID: 9631 RVA: 0x00111516 File Offset: 0x0010F716
		public override void QuickExecute()
		{
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x060025A0 RID: 9632 RVA: 0x00111518 File Offset: 0x0010F718
		// (set) Token: 0x060025A1 RID: 9633 RVA: 0x00111520 File Offset: 0x0010F720
		public string Name
		{
			get
			{
				return this.mAnimationName;
			}
			set
			{
				this.mAnimationName = value;
				string[] array = this.mAnimationName.Split(new char[]
				{
					'/'
				});
				this.mAnimationID = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.mAnimationID[i] = array[i].GetHashCodeCustom();
				}
			}
		}

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x060025A2 RID: 9634 RVA: 0x00111579 File Offset: 0x0010F779
		// (set) Token: 0x060025A3 RID: 9635 RVA: 0x00111581 File Offset: 0x0010F781
		public float Speed
		{
			get
			{
				return this.mSpeed;
			}
			set
			{
				this.mSpeed = value;
			}
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x060025A4 RID: 9636 RVA: 0x0011158A File Offset: 0x0010F78A
		// (set) Token: 0x060025A5 RID: 9637 RVA: 0x00111592 File Offset: 0x0010F792
		public float Length
		{
			get
			{
				return this.mLength;
			}
			set
			{
				this.mLength = value;
			}
		}

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x060025A6 RID: 9638 RVA: 0x0011159B File Offset: 0x0010F79B
		// (set) Token: 0x060025A7 RID: 9639 RVA: 0x001115A3 File Offset: 0x0010F7A3
		public bool Children
		{
			get
			{
				return this.mChildren;
			}
			set
			{
				this.mChildren = value;
			}
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x060025A8 RID: 9640 RVA: 0x001115AC File Offset: 0x0010F7AC
		// (set) Token: 0x060025A9 RID: 9641 RVA: 0x001115BA File Offset: 0x0010F7BA
		public bool Loop
		{
			get
			{
				return this.mLoop.GetValueOrDefault(false);
			}
			set
			{
				this.mLoop = new bool?(value);
			}
		}

		// Token: 0x04002903 RID: 10499
		private string mAnimationName;

		// Token: 0x04002904 RID: 10500
		private int[] mAnimationID;

		// Token: 0x04002905 RID: 10501
		private float mSpeed = 1f;

		// Token: 0x04002906 RID: 10502
		private float mLength;

		// Token: 0x04002907 RID: 10503
		private bool mChildren = true;

		// Token: 0x04002908 RID: 10504
		private bool? mLoop;
	}
}
