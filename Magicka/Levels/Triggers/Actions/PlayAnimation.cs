using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000598 RID: 1432
	public class PlayAnimation : Action
	{
		// Token: 0x06002AB2 RID: 10930 RVA: 0x001513B9 File Offset: 0x0014F5B9
		public PlayAnimation(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002AB3 RID: 10931 RVA: 0x001513EC File Offset: 0x0014F5EC
		protected override void Execute()
		{
			AnimatedLevelPart animatedLevelPart = base.GameScene.LevelModel.GetAnimatedLevelPart(this.mAnimationID[0]);
			for (int i = 1; i < this.mAnimationID.Length; i++)
			{
				animatedLevelPart = animatedLevelPart.GetChild(this.mAnimationID[i]);
			}
			animatedLevelPart.Play(this.mChildren, this.mStart, this.mEnd, this.mSpeed, this.mLoop, this.mResume);
		}

		// Token: 0x06002AB4 RID: 10932 RVA: 0x0015145E File Offset: 0x0014F65E
		public override void QuickExecute()
		{
		}

		// Token: 0x170009FB RID: 2555
		// (get) Token: 0x06002AB5 RID: 10933 RVA: 0x00151460 File Offset: 0x0014F660
		// (set) Token: 0x06002AB6 RID: 10934 RVA: 0x00151468 File Offset: 0x0014F668
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

		// Token: 0x170009FC RID: 2556
		// (get) Token: 0x06002AB7 RID: 10935 RVA: 0x001514C1 File Offset: 0x0014F6C1
		// (set) Token: 0x06002AB8 RID: 10936 RVA: 0x001514C9 File Offset: 0x0014F6C9
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

		// Token: 0x170009FD RID: 2557
		// (get) Token: 0x06002AB9 RID: 10937 RVA: 0x001514D2 File Offset: 0x0014F6D2
		// (set) Token: 0x06002ABA RID: 10938 RVA: 0x001514DA File Offset: 0x0014F6DA
		public float Start
		{
			get
			{
				return this.mStart;
			}
			set
			{
				this.mStart = value;
			}
		}

		// Token: 0x170009FE RID: 2558
		// (get) Token: 0x06002ABB RID: 10939 RVA: 0x001514E3 File Offset: 0x0014F6E3
		// (set) Token: 0x06002ABC RID: 10940 RVA: 0x001514EB File Offset: 0x0014F6EB
		public float End
		{
			get
			{
				return this.mEnd;
			}
			set
			{
				this.mEnd = value;
			}
		}

		// Token: 0x170009FF RID: 2559
		// (get) Token: 0x06002ABD RID: 10941 RVA: 0x001514F4 File Offset: 0x0014F6F4
		// (set) Token: 0x06002ABE RID: 10942 RVA: 0x001514FC File Offset: 0x0014F6FC
		public bool Loop
		{
			get
			{
				return this.mLoop;
			}
			set
			{
				this.mLoop = value;
			}
		}

		// Token: 0x17000A00 RID: 2560
		// (get) Token: 0x06002ABF RID: 10943 RVA: 0x00151505 File Offset: 0x0014F705
		// (set) Token: 0x06002AC0 RID: 10944 RVA: 0x0015150D File Offset: 0x0014F70D
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

		// Token: 0x17000A01 RID: 2561
		// (get) Token: 0x06002AC1 RID: 10945 RVA: 0x00151516 File Offset: 0x0014F716
		// (set) Token: 0x06002AC2 RID: 10946 RVA: 0x0015151E File Offset: 0x0014F71E
		public bool Resume
		{
			get
			{
				return this.mResume;
			}
			set
			{
				this.mResume = value;
			}
		}

		// Token: 0x04002E03 RID: 11779
		private string mAnimationName;

		// Token: 0x04002E04 RID: 11780
		private int[] mAnimationID;

		// Token: 0x04002E05 RID: 11781
		private bool mResume;

		// Token: 0x04002E06 RID: 11782
		private float mSpeed = 1f;

		// Token: 0x04002E07 RID: 11783
		private float mStart = -1f;

		// Token: 0x04002E08 RID: 11784
		private float mEnd = -1f;

		// Token: 0x04002E09 RID: 11785
		private bool mChildren = true;

		// Token: 0x04002E0A RID: 11786
		private bool mLoop;
	}
}
