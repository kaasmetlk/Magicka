using System;
using Magicka.Graphics;
using PolygonHead.ParticleEffects;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200038E RID: 910
	public class SpawnEffect : Action
	{
		// Token: 0x06001BD7 RID: 7127 RVA: 0x000BF247 File Offset: 0x000BD447
		public SpawnEffect(Trigger iTrigger, GameScene iGameScene) : base(iTrigger, iGameScene)
		{
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x000BF251 File Offset: 0x000BD451
		protected override void Execute()
		{
			this.Spawn();
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x000BF25C File Offset: 0x000BD45C
		public override void QuickExecute()
		{
			if (EffectManager.Instance.GetEffect(this.mEffectID).Type != VisualEffectType.Single)
			{
				this.Spawn();
			}
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x000BF28C File Offset: 0x000BD48C
		protected void Spawn()
		{
			LevelModel.VisualEffectStorage visualEffectStorage;
			visualEffectStorage.ID = this.mNameID;
			visualEffectStorage.Effect = this.mEffectID;
			base.GameScene.GetLocator(this.mAreaID, out visualEffectStorage.Transform);
			visualEffectStorage.Range = this.mRange;
			base.GameScene.AddEffect(this.mNameID, ref visualEffectStorage);
		}

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x06001BDB RID: 7131 RVA: 0x000BF2EB File Offset: 0x000BD4EB
		// (set) Token: 0x06001BDC RID: 7132 RVA: 0x000BF2F3 File Offset: 0x000BD4F3
		public string Effect
		{
			get
			{
				return this.mEffect;
			}
			set
			{
				this.mEffect = value;
				this.mEffectID = this.mEffect.GetHashCodeCustom();
			}
		}

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x06001BDD RID: 7133 RVA: 0x000BF30D File Offset: 0x000BD50D
		// (set) Token: 0x06001BDE RID: 7134 RVA: 0x000BF315 File Offset: 0x000BD515
		public string Name
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
				this.mNameID = this.mName.GetHashCodeCustom();
			}
		}

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x06001BDF RID: 7135 RVA: 0x000BF32F File Offset: 0x000BD52F
		// (set) Token: 0x06001BE0 RID: 7136 RVA: 0x000BF337 File Offset: 0x000BD537
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaID = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x06001BE1 RID: 7137 RVA: 0x000BF351 File Offset: 0x000BD551
		// (set) Token: 0x06001BE2 RID: 7138 RVA: 0x000BF359 File Offset: 0x000BD559
		public float Range
		{
			get
			{
				return this.mRange;
			}
			set
			{
				this.mRange = value;
			}
		}

		// Token: 0x04001E29 RID: 7721
		private string mEffect;

		// Token: 0x04001E2A RID: 7722
		private int mEffectID;

		// Token: 0x04001E2B RID: 7723
		private string mName;

		// Token: 0x04001E2C RID: 7724
		private int mNameID;

		// Token: 0x04001E2D RID: 7725
		private string mArea;

		// Token: 0x04001E2E RID: 7726
		private int mAreaID;

		// Token: 0x04001E2F RID: 7727
		private float mRange;
	}
}
