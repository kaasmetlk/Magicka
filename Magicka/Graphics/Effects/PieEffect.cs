using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Graphics.Effects
{
	// Token: 0x0200048D RID: 1165
	public class PieEffect : Effect
	{
		// Token: 0x06002354 RID: 9044 RVA: 0x000FD010 File Offset: 0x000FB210
		public PieEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/PieEffect"))
		{
			this.mTechnique1 = base.Techniques["Technique1"];
			this.mTransformParameter = base.Parameters["Transform"];
			this.mTextureParameter = base.Parameters["Texture"];
			this.mTextureOffsetParameter = base.Parameters["TextureOffset"];
			this.mTransformToScreenParameter = base.Parameters["TransformToScreen"];
			this.mMaxAngleParameter = base.Parameters["MaxAngle"];
			this.mRadiusParameter = base.Parameters["Radius"];
		}

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x06002355 RID: 9045 RVA: 0x000FD0C9 File Offset: 0x000FB2C9
		// (set) Token: 0x06002356 RID: 9046 RVA: 0x000FD0D6 File Offset: 0x000FB2D6
		public float Radius
		{
			get
			{
				return this.mRadiusParameter.GetValueSingle();
			}
			set
			{
				this.mRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x06002357 RID: 9047 RVA: 0x000FD0E4 File Offset: 0x000FB2E4
		// (set) Token: 0x06002358 RID: 9048 RVA: 0x000FD0F1 File Offset: 0x000FB2F1
		public float MaxAngle
		{
			get
			{
				return this.mMaxAngleParameter.GetValueSingle();
			}
			set
			{
				this.mMaxAngleParameter.SetValue(value);
			}
		}

		// Token: 0x06002359 RID: 9049 RVA: 0x000FD100 File Offset: 0x000FB300
		public void SetScreenSize(int iWidth, int iHeight)
		{
			Matrix value;
			RenderManager.Instance.CreateTransformPixelsToScreen((float)iWidth, (float)iHeight, out value);
			this.mTransformToScreenParameter.SetValue(value);
		}

		// Token: 0x0600235A RID: 9050 RVA: 0x000FD12C File Offset: 0x000FB32C
		public void SetTechnique(PieEffect.Technique iTechnique)
		{
			if (iTechnique != PieEffect.Technique.Technique1)
			{
				return;
			}
			base.CurrentTechnique = this.mTechnique1;
		}

		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x0600235B RID: 9051 RVA: 0x000FD14C File Offset: 0x000FB34C
		// (set) Token: 0x0600235C RID: 9052 RVA: 0x000FD159 File Offset: 0x000FB359
		public Vector2 TextureOffset
		{
			get
			{
				return this.mTextureOffsetParameter.GetValueVector2();
			}
			set
			{
				this.mTextureOffsetParameter.SetValue(value);
			}
		}

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x0600235D RID: 9053 RVA: 0x000FD167 File Offset: 0x000FB367
		// (set) Token: 0x0600235E RID: 9054 RVA: 0x000FD174 File Offset: 0x000FB374
		public Matrix Transform
		{
			get
			{
				return this.mTransformParameter.GetValueMatrix();
			}
			set
			{
				this.mTransformParameter.SetValue(value);
			}
		}

		// Token: 0x1700086E RID: 2158
		// (set) Token: 0x0600235F RID: 9055 RVA: 0x000FD182 File Offset: 0x000FB382
		public Texture Texture
		{
			set
			{
				this.mTextureParameter.SetValue(value);
			}
		}

		// Token: 0x0400265D RID: 9821
		private EffectTechnique mTechnique1;

		// Token: 0x0400265E RID: 9822
		private EffectParameter mTransformParameter;

		// Token: 0x0400265F RID: 9823
		private EffectParameter mTextureParameter;

		// Token: 0x04002660 RID: 9824
		private EffectParameter mTextureOffsetParameter;

		// Token: 0x04002661 RID: 9825
		private EffectParameter mTransformToScreenParameter;

		// Token: 0x04002662 RID: 9826
		private EffectParameter mMaxAngleParameter;

		// Token: 0x04002663 RID: 9827
		private EffectParameter mRadiusParameter;

		// Token: 0x0200048E RID: 1166
		public enum Technique
		{
			// Token: 0x04002665 RID: 9829
			Technique1
		}
	}
}
