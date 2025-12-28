using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000561 RID: 1377
	public class SwayEffect : Effect
	{
		// Token: 0x0600291C RID: 10524 RVA: 0x001426E0 File Offset: 0x001408E0
		public SwayEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/SwayEffect"))
		{
			this.mSwayTechnique = base.Techniques["Sway"];
			this.mCharacterOffsetTechnique = base.Techniques["CharacterOffset"];
			this.mWorldParameter = base.Parameters["World"];
			this.mViewProjectionParameter = base.Parameters["ViewProjection"];
			this.mTextureTransformParameter = base.Parameters["TextureTransform"];
			this.mTextureParameter = base.Parameters["Texture"];
		}

		// Token: 0x0600291D RID: 10525 RVA: 0x00142784 File Offset: 0x00140984
		public void SetTechnique(SwayEffect.Technique iTechnique)
		{
			switch (iTechnique)
			{
			case SwayEffect.Technique.Sway:
				base.CurrentTechnique = this.mSwayTechnique;
				return;
			case SwayEffect.Technique.CharacterOffset:
				base.CurrentTechnique = this.mCharacterOffsetTechnique;
				return;
			default:
				return;
			}
		}

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x0600291E RID: 10526 RVA: 0x001427BB File Offset: 0x001409BB
		// (set) Token: 0x0600291F RID: 10527 RVA: 0x001427C3 File Offset: 0x001409C3
		public Matrix World
		{
			get
			{
				return this.mWorld;
			}
			set
			{
				if (this.mWorld != value)
				{
					this.mWorld = value;
					this.mWorldParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x06002920 RID: 10528 RVA: 0x001427E6 File Offset: 0x001409E6
		// (set) Token: 0x06002921 RID: 10529 RVA: 0x001427EE File Offset: 0x001409EE
		public Matrix ViewProjection
		{
			get
			{
				return this.mViewProjection;
			}
			set
			{
				if (this.mViewProjection != value)
				{
					this.mViewProjection = value;
					this.mViewProjectionParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x06002922 RID: 10530 RVA: 0x00142811 File Offset: 0x00140A11
		// (set) Token: 0x06002923 RID: 10531 RVA: 0x00142819 File Offset: 0x00140A19
		public Matrix TextureTransform
		{
			get
			{
				return this.mTextureTransform;
			}
			set
			{
				if (this.mTextureTransform != value)
				{
					this.mTextureTransform = value;
					this.mTextureTransformParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x06002924 RID: 10532 RVA: 0x0014283C File Offset: 0x00140A3C
		// (set) Token: 0x06002925 RID: 10533 RVA: 0x00142844 File Offset: 0x00140A44
		public Texture2D Texture
		{
			get
			{
				return this.mTexture;
			}
			set
			{
				if (this.mTexture != value)
				{
					this.mTexture = value;
					this.mTextureParameter.SetValue(value);
				}
			}
		}

		// Token: 0x04002C6E RID: 11374
		private EffectTechnique mSwayTechnique;

		// Token: 0x04002C6F RID: 11375
		private EffectTechnique mCharacterOffsetTechnique;

		// Token: 0x04002C70 RID: 11376
		private EffectParameter mWorldParameter;

		// Token: 0x04002C71 RID: 11377
		private Matrix mWorld;

		// Token: 0x04002C72 RID: 11378
		private EffectParameter mViewProjectionParameter;

		// Token: 0x04002C73 RID: 11379
		private Matrix mViewProjection;

		// Token: 0x04002C74 RID: 11380
		private EffectParameter mTextureTransformParameter;

		// Token: 0x04002C75 RID: 11381
		private Matrix mTextureTransform;

		// Token: 0x04002C76 RID: 11382
		private EffectParameter mTextureParameter;

		// Token: 0x04002C77 RID: 11383
		private Texture2D mTexture;

		// Token: 0x02000562 RID: 1378
		public enum Technique
		{
			// Token: 0x04002C79 RID: 11385
			Sway,
			// Token: 0x04002C7A RID: 11386
			CharacterOffset
		}
	}
}
