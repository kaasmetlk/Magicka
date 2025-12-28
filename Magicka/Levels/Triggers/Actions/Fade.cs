using System;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000391 RID: 913
	public class Fade : Action
	{
		// Token: 0x06001BFB RID: 7163 RVA: 0x000BF7D1 File Offset: 0x000BD9D1
		public Fade(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
			this.mScene = iScene;
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x000BF7E4 File Offset: 0x000BD9E4
		protected override void Execute()
		{
			if (this.mFadeIn)
			{
				RenderManager.Instance.BeginTransition(Transitions.Fade, this.mColor, this.mTime);
			}
			else
			{
				RenderManager.Instance.EndTransition(Transitions.Fade, this.mColor, this.mTime);
			}
			if (this.mOverride)
			{
				base.GameScene.Level.ClearTransition();
			}
		}

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06001BFD RID: 7165 RVA: 0x000BF841 File Offset: 0x000BDA41
		// (set) Token: 0x06001BFE RID: 7166 RVA: 0x000BF849 File Offset: 0x000BDA49
		public Color Color
		{
			get
			{
				return this.mColor;
			}
			set
			{
				this.mColor = value;
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x06001BFF RID: 7167 RVA: 0x000BF852 File Offset: 0x000BDA52
		// (set) Token: 0x06001C00 RID: 7168 RVA: 0x000BF85A File Offset: 0x000BDA5A
		public bool FadeIn
		{
			get
			{
				return this.mFadeIn;
			}
			set
			{
				this.mFadeIn = value;
			}
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06001C01 RID: 7169 RVA: 0x000BF863 File Offset: 0x000BDA63
		// (set) Token: 0x06001C02 RID: 7170 RVA: 0x000BF86B File Offset: 0x000BDA6B
		public bool Override
		{
			get
			{
				return this.mOverride;
			}
			set
			{
				this.mOverride = value;
			}
		}

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06001C03 RID: 7171 RVA: 0x000BF874 File Offset: 0x000BDA74
		// (set) Token: 0x06001C04 RID: 7172 RVA: 0x000BF87C File Offset: 0x000BDA7C
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

		// Token: 0x06001C05 RID: 7173 RVA: 0x000BF885 File Offset: 0x000BDA85
		public override void QuickExecute()
		{
		}

		// Token: 0x04001E42 RID: 7746
		private bool mFadeIn;

		// Token: 0x04001E43 RID: 7747
		private Color mColor;

		// Token: 0x04001E44 RID: 7748
		private float mTime;

		// Token: 0x04001E45 RID: 7749
		private bool mOverride;

		// Token: 0x04001E46 RID: 7750
		private new GameScene mScene;
	}
}
