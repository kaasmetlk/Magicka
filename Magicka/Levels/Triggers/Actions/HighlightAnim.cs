using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000484 RID: 1156
	public class HighlightAnim : Action
	{
		// Token: 0x060022FE RID: 8958 RVA: 0x000FB9D1 File Offset: 0x000F9BD1
		public HighlightAnim(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x000FB9E8 File Offset: 0x000F9BE8
		protected override void Execute()
		{
			AnimatedLevelPart animatedLevelPart = base.GameScene.LevelModel.GetAnimatedLevelPart(this.mIDHash[0]);
			for (int i = 1; i < this.mIDHash.Length; i++)
			{
				animatedLevelPart = animatedLevelPart.GetChild(this.mIDHash[i]);
			}
			animatedLevelPart.Highlight(this.mTime);
		}

		// Token: 0x06002300 RID: 8960 RVA: 0x000FBA3C File Offset: 0x000F9C3C
		public override void QuickExecute()
		{
		}

		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x06002301 RID: 8961 RVA: 0x000FBA3E File Offset: 0x000F9C3E
		// (set) Token: 0x06002302 RID: 8962 RVA: 0x000FBA48 File Offset: 0x000F9C48
		public string Name
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				string[] array = this.mID.Split(new char[]
				{
					'/'
				});
				this.mIDHash = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.mIDHash[i] = array[i].GetHashCodeCustom();
				}
			}
		}

		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x06002303 RID: 8963 RVA: 0x000FBAA1 File Offset: 0x000F9CA1
		// (set) Token: 0x06002304 RID: 8964 RVA: 0x000FBAA9 File Offset: 0x000F9CA9
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

		// Token: 0x0400261E RID: 9758
		private string mID;

		// Token: 0x0400261F RID: 9759
		private int[] mIDHash;

		// Token: 0x04002620 RID: 9760
		private float mTime = 1f;
	}
}
