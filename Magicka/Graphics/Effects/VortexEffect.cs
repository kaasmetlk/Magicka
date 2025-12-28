using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020001AF RID: 431
	public class VortexEffect : Effect
	{
		// Token: 0x06000CF1 RID: 3313 RVA: 0x0004BD58 File Offset: 0x00049F58
		public VortexEffect() : base(Game.Instance.GraphicsDevice, Game.Instance.Content.Load<Effect>("Shaders/VortexEffect"))
		{
			this.mWorldParameter = base.Parameters["World"];
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mDistortionParameter = base.Parameters["Distortion"];
			this.mDistortionPowerParameter = base.Parameters["DistortionPower"];
			this.mPixelSizeParameter = base.Parameters["PixelSize"];
			this.mSourceTextureParameter = base.Parameters["SourceTexture"];
			this.mDepthTextureParameter = base.Parameters["DepthTexture"];
			this.mTextureParameter = base.Parameters["Texture"];
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06000CF2 RID: 3314 RVA: 0x0004BE4F File Offset: 0x0004A04F
		// (set) Token: 0x06000CF3 RID: 3315 RVA: 0x0004BE5C File Offset: 0x0004A05C
		public Matrix World
		{
			get
			{
				return this.mWorldParameter.GetValueMatrix();
			}
			set
			{
				this.mWorldParameter.SetValue(value);
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06000CF4 RID: 3316 RVA: 0x0004BE6A File Offset: 0x0004A06A
		// (set) Token: 0x06000CF5 RID: 3317 RVA: 0x0004BE77 File Offset: 0x0004A077
		public Matrix View
		{
			get
			{
				return this.mViewParameter.GetValueMatrix();
			}
			set
			{
				this.mViewParameter.SetValue(value);
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06000CF6 RID: 3318 RVA: 0x0004BE85 File Offset: 0x0004A085
		// (set) Token: 0x06000CF7 RID: 3319 RVA: 0x0004BE92 File Offset: 0x0004A092
		public Matrix Projection
		{
			get
			{
				return this.mProjectionParameter.GetValueMatrix();
			}
			set
			{
				this.mProjectionParameter.SetValue(value);
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06000CF8 RID: 3320 RVA: 0x0004BEA0 File Offset: 0x0004A0A0
		// (set) Token: 0x06000CF9 RID: 3321 RVA: 0x0004BEAD File Offset: 0x0004A0AD
		public float Distortion
		{
			get
			{
				return this.mDistortionParameter.GetValueSingle();
			}
			set
			{
				this.mDistortionParameter.SetValue(value);
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06000CFA RID: 3322 RVA: 0x0004BEBB File Offset: 0x0004A0BB
		// (set) Token: 0x06000CFB RID: 3323 RVA: 0x0004BEC8 File Offset: 0x0004A0C8
		public float DistortionPower
		{
			get
			{
				return this.mDistortionPowerParameter.GetValueSingle();
			}
			set
			{
				this.mDistortionPowerParameter.SetValue(value);
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06000CFC RID: 3324 RVA: 0x0004BED6 File Offset: 0x0004A0D6
		// (set) Token: 0x06000CFD RID: 3325 RVA: 0x0004BEE3 File Offset: 0x0004A0E3
		public Vector2 PixelSize
		{
			get
			{
				return this.mPixelSizeParameter.GetValueVector2();
			}
			set
			{
				this.mPixelSizeParameter.SetValue(value);
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06000CFE RID: 3326 RVA: 0x0004BEF1 File Offset: 0x0004A0F1
		// (set) Token: 0x06000CFF RID: 3327 RVA: 0x0004BEFE File Offset: 0x0004A0FE
		public Texture2D SourceTexture
		{
			get
			{
				return this.mSourceTextureParameter.GetValueTexture2D();
			}
			set
			{
				this.mSourceTextureParameter.SetValue(value);
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06000D00 RID: 3328 RVA: 0x0004BF0C File Offset: 0x0004A10C
		// (set) Token: 0x06000D01 RID: 3329 RVA: 0x0004BF19 File Offset: 0x0004A119
		public Texture2D DepthTexture
		{
			get
			{
				return this.mDepthTextureParameter.GetValueTexture2D();
			}
			set
			{
				this.mDepthTextureParameter.SetValue(value);
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06000D02 RID: 3330 RVA: 0x0004BF27 File Offset: 0x0004A127
		// (set) Token: 0x06000D03 RID: 3331 RVA: 0x0004BF34 File Offset: 0x0004A134
		public Texture2D Texture
		{
			get
			{
				return this.mTextureParameter.GetValueTexture2D();
			}
			set
			{
				this.mTextureParameter.SetValue(value);
			}
		}

		// Token: 0x04000BBF RID: 3007
		public static readonly int TYPEHASH = typeof(VortexEffect).GetHashCode();

		// Token: 0x04000BC0 RID: 3008
		private EffectParameter mWorldParameter;

		// Token: 0x04000BC1 RID: 3009
		private EffectParameter mViewParameter;

		// Token: 0x04000BC2 RID: 3010
		private EffectParameter mProjectionParameter;

		// Token: 0x04000BC3 RID: 3011
		private EffectParameter mDistortionParameter;

		// Token: 0x04000BC4 RID: 3012
		private EffectParameter mDistortionPowerParameter;

		// Token: 0x04000BC5 RID: 3013
		private EffectParameter mPixelSizeParameter;

		// Token: 0x04000BC6 RID: 3014
		private EffectParameter mSourceTextureParameter;

		// Token: 0x04000BC7 RID: 3015
		private EffectParameter mDepthTextureParameter;

		// Token: 0x04000BC8 RID: 3016
		private EffectParameter mTextureParameter;
	}
}
