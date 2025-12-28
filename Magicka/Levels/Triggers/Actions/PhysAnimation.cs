using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000629 RID: 1577
	public class PhysAnimation : Action
	{
		// Token: 0x06002F84 RID: 12164 RVA: 0x00181082 File Offset: 0x0017F282
		public PhysAnimation(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002F85 RID: 12165 RVA: 0x001810A0 File Offset: 0x0017F2A0
		protected override void Execute()
		{
			DamageablePhysicsEntity damageablePhysicsEntity = Entity.GetByID(this.mPhysNameID) as DamageablePhysicsEntity;
			AnimatedLevelPart animatedLevelPart = base.GameScene.LevelModel.GetAnimatedLevelPart(this.mAnimationID[0]);
			for (int i = 1; i < this.mAnimationID.Length; i++)
			{
				animatedLevelPart = animatedLevelPart.GetChild(this.mAnimationID[i]);
			}
			PhysAnimationControl physAnimCont;
			physAnimCont.AnimatedLevelPart = animatedLevelPart;
			physAnimCont.End = this.mEnd;
			physAnimCont.Start = this.mStart;
			physAnimCont.Speed = this.mSpeed;
			physAnimCont.Children = this.mChildren;
			damageablePhysicsEntity.AddAnimation(physAnimCont);
		}

		// Token: 0x06002F86 RID: 12166 RVA: 0x0018113E File Offset: 0x0017F33E
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000B43 RID: 2883
		// (get) Token: 0x06002F87 RID: 12167 RVA: 0x00181146 File Offset: 0x0017F346
		// (set) Token: 0x06002F88 RID: 12168 RVA: 0x0018114E File Offset: 0x0017F34E
		public string Id
		{
			get
			{
				return this.mPhysName;
			}
			set
			{
				this.mPhysName = value;
				this.mPhysNameID = this.mPhysName.GetHashCodeCustom();
			}
		}

		// Token: 0x17000B44 RID: 2884
		// (get) Token: 0x06002F89 RID: 12169 RVA: 0x00181168 File Offset: 0x0017F368
		// (set) Token: 0x06002F8A RID: 12170 RVA: 0x00181170 File Offset: 0x0017F370
		public string Name
		{
			get
			{
				return this.mAnimationNames;
			}
			set
			{
				this.mAnimationNames = value;
				string[] array = this.mAnimationNames.Split(new char[]
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

		// Token: 0x17000B45 RID: 2885
		// (get) Token: 0x06002F8B RID: 12171 RVA: 0x001811C9 File Offset: 0x0017F3C9
		// (set) Token: 0x06002F8C RID: 12172 RVA: 0x001811D1 File Offset: 0x0017F3D1
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

		// Token: 0x17000B46 RID: 2886
		// (get) Token: 0x06002F8D RID: 12173 RVA: 0x001811DA File Offset: 0x0017F3DA
		// (set) Token: 0x06002F8E RID: 12174 RVA: 0x001811E2 File Offset: 0x0017F3E2
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

		// Token: 0x17000B47 RID: 2887
		// (get) Token: 0x06002F8F RID: 12175 RVA: 0x001811EB File Offset: 0x0017F3EB
		// (set) Token: 0x06002F90 RID: 12176 RVA: 0x001811F3 File Offset: 0x0017F3F3
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

		// Token: 0x17000B48 RID: 2888
		// (get) Token: 0x06002F91 RID: 12177 RVA: 0x001811FC File Offset: 0x0017F3FC
		// (set) Token: 0x06002F92 RID: 12178 RVA: 0x00181204 File Offset: 0x0017F404
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

		// Token: 0x04003388 RID: 13192
		private string mPhysName;

		// Token: 0x04003389 RID: 13193
		private int mPhysNameID;

		// Token: 0x0400338A RID: 13194
		private string mAnimationNames;

		// Token: 0x0400338B RID: 13195
		private int[] mAnimationID;

		// Token: 0x0400338C RID: 13196
		private float mSpeed = 1f;

		// Token: 0x0400338D RID: 13197
		private float mStart;

		// Token: 0x0400338E RID: 13198
		private float mEnd;

		// Token: 0x0400338F RID: 13199
		private bool mChildren = true;
	}
}
