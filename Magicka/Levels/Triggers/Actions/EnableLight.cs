using System;
using PolygonHead.Lights;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200059C RID: 1436
	internal class EnableLight : Action
	{
		// Token: 0x06002AD7 RID: 10967 RVA: 0x0015176E File Offset: 0x0014F96E
		public EnableLight(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002AD8 RID: 10968 RVA: 0x00151778 File Offset: 0x0014F978
		protected override void Execute()
		{
			for (int i = 0; i < this.mLightIDs.Length; i++)
			{
				base.GameScene.LevelModel.Lights[this.mLightIDs[i]].Enable(base.GameScene.Scene, this.mEffect, this.mTime);
			}
		}

		// Token: 0x06002AD9 RID: 10969 RVA: 0x001517D4 File Offset: 0x0014F9D4
		public override void QuickExecute()
		{
			for (int i = 0; i < this.mLightIDs.Length; i++)
			{
				base.GameScene.LevelModel.Lights[this.mLightIDs[i]].Enable(base.GameScene.Scene, LightTransitionType.None, 0f);
			}
		}

		// Token: 0x17000A08 RID: 2568
		// (get) Token: 0x06002ADA RID: 10970 RVA: 0x00151827 File Offset: 0x0014FA27
		// (set) Token: 0x06002ADB RID: 10971 RVA: 0x0015183C File Offset: 0x0014FA3C
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

		// Token: 0x17000A09 RID: 2569
		// (get) Token: 0x06002ADC RID: 10972 RVA: 0x001518A2 File Offset: 0x0014FAA2
		// (set) Token: 0x06002ADD RID: 10973 RVA: 0x001518AA File Offset: 0x0014FAAA
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

		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x06002ADE RID: 10974 RVA: 0x001518B3 File Offset: 0x0014FAB3
		// (set) Token: 0x06002ADF RID: 10975 RVA: 0x001518BB File Offset: 0x0014FABB
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

		// Token: 0x04002E22 RID: 11810
		private string[] mLights;

		// Token: 0x04002E23 RID: 11811
		private int[] mLightIDs;

		// Token: 0x04002E24 RID: 11812
		private LightTransitionType mEffect;

		// Token: 0x04002E25 RID: 11813
		private float mTime;
	}
}
