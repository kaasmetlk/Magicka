using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000022 RID: 34
	public class ShieldEffect : Effect
	{
		// Token: 0x060000FF RID: 255 RVA: 0x00007D14 File Offset: 0x00005F14
		public ShieldEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/ShieldEffect"))
		{
			this.mThicknessParameter = base.Parameters["Thickness"];
			this.mWorldParameter = base.Parameters["World"];
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mNoise0OffsetParameter = base.Parameters["Noise0Offset"];
			this.mNoise1OffsetParameter = base.Parameters["Noise1Offset"];
			this.mNoise2OffsetParameter = base.Parameters["Noise2Offset"];
			this.mMinDotProductParameter = base.Parameters["MinDotProduct"];
			this.mDirectionParameter = base.Parameters["Direction"];
			this.mTextureScaleParameter = base.Parameters["TextureScale"];
			this.mColorTintParameter = base.Parameters["ColorTint"];
			this.mTextureParameter = base.Parameters["NormalMap"];
			this.mDepthMapParameter = base.Parameters["DepthMap"];
			this.mDamagePointsParameter = base.Parameters["DamagePoints"];
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00007E67 File Offset: 0x00006067
		public void SetTechnique(ShieldEffect.Technique iTechnique)
		{
			base.CurrentTechnique = base.Techniques[(int)iTechnique];
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00007E7B File Offset: 0x0000607B
		// (set) Token: 0x06000102 RID: 258 RVA: 0x00007E88 File Offset: 0x00006088
		public float Thickness
		{
			get
			{
				return this.mThicknessParameter.GetValueSingle();
			}
			set
			{
				this.mThicknessParameter.SetValue(value);
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00007E96 File Offset: 0x00006096
		// (set) Token: 0x06000104 RID: 260 RVA: 0x00007EA3 File Offset: 0x000060A3
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

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00007EB1 File Offset: 0x000060B1
		// (set) Token: 0x06000106 RID: 262 RVA: 0x00007EC0 File Offset: 0x000060C0
		public Vector4[] DamagePoints
		{
			get
			{
				return this.mDamagePointsParameter.GetValueVector4Array(16);
			}
			set
			{
				this.mDamagePointsParameter.SetValue(value);
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00007ECE File Offset: 0x000060CE
		// (set) Token: 0x06000108 RID: 264 RVA: 0x00007EDB File Offset: 0x000060DB
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000109 RID: 265 RVA: 0x00007EE9 File Offset: 0x000060E9
		// (set) Token: 0x0600010A RID: 266 RVA: 0x00007EF6 File Offset: 0x000060F6
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600010B RID: 267 RVA: 0x00007F04 File Offset: 0x00006104
		// (set) Token: 0x0600010C RID: 268 RVA: 0x00007F11 File Offset: 0x00006111
		public Vector2 Noise0Offset
		{
			get
			{
				return this.mNoise0OffsetParameter.GetValueVector2();
			}
			set
			{
				this.mNoise0OffsetParameter.SetValue(value);
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00007F1F File Offset: 0x0000611F
		// (set) Token: 0x0600010E RID: 270 RVA: 0x00007F2C File Offset: 0x0000612C
		public Vector2 Noise1Offset
		{
			get
			{
				return this.mNoise1OffsetParameter.GetValueVector2();
			}
			set
			{
				this.mNoise1OffsetParameter.SetValue(value);
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00007F3A File Offset: 0x0000613A
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00007F47 File Offset: 0x00006147
		public Vector2 Noise2Offset
		{
			get
			{
				return this.mNoise2OffsetParameter.GetValueVector2();
			}
			set
			{
				this.mNoise2OffsetParameter.SetValue(value);
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000111 RID: 273 RVA: 0x00007F55 File Offset: 0x00006155
		// (set) Token: 0x06000112 RID: 274 RVA: 0x00007F62 File Offset: 0x00006162
		public float MinDotProduct
		{
			get
			{
				return this.mMinDotProductParameter.GetValueSingle();
			}
			set
			{
				this.mMinDotProductParameter.SetValue(value);
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00007F70 File Offset: 0x00006170
		// (set) Token: 0x06000114 RID: 276 RVA: 0x00007F7D File Offset: 0x0000617D
		public Vector3 Direction
		{
			get
			{
				return this.mDirectionParameter.GetValueVector3();
			}
			set
			{
				this.mDirectionParameter.SetValue(value);
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00007F8B File Offset: 0x0000618B
		// (set) Token: 0x06000116 RID: 278 RVA: 0x00007F98 File Offset: 0x00006198
		public Vector2 TextureScale
		{
			get
			{
				return this.mTextureScaleParameter.GetValueVector2();
			}
			set
			{
				this.mTextureScaleParameter.SetValue(value);
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00007FA6 File Offset: 0x000061A6
		// (set) Token: 0x06000118 RID: 280 RVA: 0x00007FB3 File Offset: 0x000061B3
		public Vector4 ColorTint
		{
			get
			{
				return this.mColorTintParameter.GetValueVector4();
			}
			set
			{
				this.mColorTintParameter.SetValue(value);
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00007FC1 File Offset: 0x000061C1
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00007FCE File Offset: 0x000061CE
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00007FDC File Offset: 0x000061DC
		// (set) Token: 0x0600011C RID: 284 RVA: 0x00007FE9 File Offset: 0x000061E9
		public Texture2D DepthMap
		{
			get
			{
				return this.mDepthMapParameter.GetValueTexture2D();
			}
			set
			{
				this.mDepthMapParameter.SetValue(value);
			}
		}

		// Token: 0x040000AA RID: 170
		public const int MAXDAMAGEPOINTS = 16;

		// Token: 0x040000AB RID: 171
		public static readonly int TYPEHASH = typeof(ShieldEffect).GetHashCode();

		// Token: 0x040000AC RID: 172
		private EffectParameter mWorldParameter;

		// Token: 0x040000AD RID: 173
		private EffectParameter mViewParameter;

		// Token: 0x040000AE RID: 174
		private EffectParameter mProjectionParameter;

		// Token: 0x040000AF RID: 175
		private EffectParameter mNoise0OffsetParameter;

		// Token: 0x040000B0 RID: 176
		private EffectParameter mNoise1OffsetParameter;

		// Token: 0x040000B1 RID: 177
		private EffectParameter mNoise2OffsetParameter;

		// Token: 0x040000B2 RID: 178
		private EffectParameter mTextureScaleParameter;

		// Token: 0x040000B3 RID: 179
		private EffectParameter mColorTintParameter;

		// Token: 0x040000B4 RID: 180
		private EffectParameter mMinDotProductParameter;

		// Token: 0x040000B5 RID: 181
		private EffectParameter mDirectionParameter;

		// Token: 0x040000B6 RID: 182
		private EffectParameter mThicknessParameter;

		// Token: 0x040000B7 RID: 183
		private EffectParameter mTextureParameter;

		// Token: 0x040000B8 RID: 184
		private EffectParameter mDepthMapParameter;

		// Token: 0x040000B9 RID: 185
		private EffectParameter mDamagePointsParameter;

		// Token: 0x02000023 RID: 35
		public enum Technique
		{
			// Token: 0x040000BB RID: 187
			Sphere,
			// Token: 0x040000BC RID: 188
			Wall
		}
	}
}
