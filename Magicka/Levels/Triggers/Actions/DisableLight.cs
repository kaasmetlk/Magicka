using System;
using PolygonHead.Lights;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000242 RID: 578
	internal class DisableLight : Action
	{
		// Token: 0x060011E1 RID: 4577 RVA: 0x0006CBD4 File Offset: 0x0006ADD4
		public DisableLight(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x0006CBE0 File Offset: 0x0006ADE0
		protected override void Execute()
		{
			for (int i = 0; i < this.mLightIDs.Length; i++)
			{
				base.GameScene.LevelModel.Lights[this.mLightIDs[i]].Disable(this.mEffect, this.mTime);
			}
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x0006CC30 File Offset: 0x0006AE30
		public override void QuickExecute()
		{
			for (int i = 0; i < this.mLightIDs.Length; i++)
			{
				base.GameScene.LevelModel.Lights[this.mLightIDs[i]].Disable(LightTransitionType.None, 0f);
			}
		}

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x060011E4 RID: 4580 RVA: 0x0006CC78 File Offset: 0x0006AE78
		// (set) Token: 0x060011E5 RID: 4581 RVA: 0x0006CC8C File Offset: 0x0006AE8C
		public string Id
		{
			get
			{
				return string.Join(",", this.mLights);
			}
			set
			{
				this.mLights = value.ToLowerInvariant().Split(new char[]
				{
					','
				});
				this.mLightIDs = new int[this.mLights.Length];
				for (int i = 0; i < this.mLights.Length; i++)
				{
					this.mLightIDs[i] = this.mLights[i].GetHashCodeCustom();
				}
			}
		}

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x060011E6 RID: 4582 RVA: 0x0006CCF2 File Offset: 0x0006AEF2
		// (set) Token: 0x060011E7 RID: 4583 RVA: 0x0006CCFA File Offset: 0x0006AEFA
		public LightTransitionType Effect
		{
			get
			{
				return this.mEffect;
			}
			set
			{
				this.mEffect = value;
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x060011E8 RID: 4584 RVA: 0x0006CD03 File Offset: 0x0006AF03
		// (set) Token: 0x060011E9 RID: 4585 RVA: 0x0006CD0B File Offset: 0x0006AF0B
		public float Time
		{
			get
			{
				return this.mTime;
			}
			set
			{
				this.mTime = value;
			}
		}

		// Token: 0x040010A5 RID: 4261
		private string[] mLights;

		// Token: 0x040010A6 RID: 4262
		private int[] mLightIDs;

		// Token: 0x040010A7 RID: 4263
		private LightTransitionType mEffect;

		// Token: 0x040010A8 RID: 4264
		private float mTime;
	}
}
