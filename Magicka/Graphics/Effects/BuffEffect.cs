using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000297 RID: 663
	public class BuffEffect : Effect
	{
		// Token: 0x060013A8 RID: 5032 RVA: 0x00078824 File Offset: 0x00076A24
		public BuffEffect(GraphicsDevice iDevice, ContentManager iContent) : base(iDevice, iContent.Load<Effect>("Shaders/BuffEffect"))
		{
			this.mPositionsParameter = base.Parameters["Positions"];
			this.mColorsParameter = base.Parameters["Colors"];
			this.mTextureOffsetsParameter = base.Parameters["TextureOffsets"];
			this.mTimeParameter = base.Parameters["Time"];
			this.mViewProjectionParameter = base.Parameters["ViewProjection"];
			this.mTextureParameter = base.Parameters["Texture"];
		}

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x060013A9 RID: 5033 RVA: 0x000788C7 File Offset: 0x00076AC7
		// (set) Token: 0x060013AA RID: 5034 RVA: 0x000788D6 File Offset: 0x00076AD6
		public Vector4[] Positions
		{
			get
			{
				return this.mPositionsParameter.GetValueVector4Array(32);
			}
			set
			{
				this.mPositionsParameter.SetValue(value);
			}
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x060013AB RID: 5035 RVA: 0x000788E4 File Offset: 0x00076AE4
		// (set) Token: 0x060013AC RID: 5036 RVA: 0x000788F3 File Offset: 0x00076AF3
		public Vector4[] Colors
		{
			get
			{
				return this.mColorsParameter.GetValueVector4Array(32);
			}
			set
			{
				this.mColorsParameter.SetValue(value);
			}
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x060013AD RID: 5037 RVA: 0x00078901 File Offset: 0x00076B01
		// (set) Token: 0x060013AE RID: 5038 RVA: 0x00078910 File Offset: 0x00076B10
		public Vector2[] TextureOffsets
		{
			get
			{
				return this.mTextureOffsetsParameter.GetValueVector2Array(32);
			}
			set
			{
				this.mTextureOffsetsParameter.SetValue(value);
			}
		}

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x060013AF RID: 5039 RVA: 0x0007891E File Offset: 0x00076B1E
		// (set) Token: 0x060013B0 RID: 5040 RVA: 0x0007892B File Offset: 0x00076B2B
		public float Time
		{
			get
			{
				return this.mTimeParameter.GetValueSingle();
			}
			set
			{
				this.mTimeParameter.SetValue(value);
			}
		}

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x060013B1 RID: 5041 RVA: 0x00078939 File Offset: 0x00076B39
		// (set) Token: 0x060013B2 RID: 5042 RVA: 0x00078946 File Offset: 0x00076B46
		public Matrix ViewProjection
		{
			get
			{
				return this.mViewProjectionParameter.GetValueMatrix();
			}
			set
			{
				this.mViewProjectionParameter.SetValue(value);
			}
		}

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x060013B3 RID: 5043 RVA: 0x00078954 File Offset: 0x00076B54
		// (set) Token: 0x060013B4 RID: 5044 RVA: 0x00078961 File Offset: 0x00076B61
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

		// Token: 0x0400152B RID: 5419
		public const int MAX_INSTANCES = 32;

		// Token: 0x0400152C RID: 5420
		public static readonly int TYPEHASH = typeof(BuffEffect).GetHashCode();

		// Token: 0x0400152D RID: 5421
		private EffectParameter mPositionsParameter;

		// Token: 0x0400152E RID: 5422
		private EffectParameter mColorsParameter;

		// Token: 0x0400152F RID: 5423
		private EffectParameter mTextureOffsetsParameter;

		// Token: 0x04001530 RID: 5424
		private EffectParameter mTimeParameter;

		// Token: 0x04001531 RID: 5425
		private EffectParameter mViewProjectionParameter;

		// Token: 0x04001532 RID: 5426
		private EffectParameter mTextureParameter;
	}
}
