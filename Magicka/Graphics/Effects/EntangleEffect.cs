using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000024 RID: 36
	public class EntangleEffect : Effect
	{
		// Token: 0x0600011E RID: 286 RVA: 0x00008010 File Offset: 0x00006210
		public EntangleEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/EntangleEffect"))
		{
			this.mDiffuseColorParameter = base.Parameters["DiffuseColor"];
			this.mEmissiveAmountParameter = base.Parameters["EmissiveAmount"];
			this.mSpecularAmountParameter = base.Parameters["SpecularAmount"];
			this.mSpecularPowerParameter = base.Parameters["SpecularPower"];
			this.mDiffuseMapEnabledParameter = base.Parameters["DiffuseMapEnabled"];
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mViewProjectionParameter = base.Parameters["ViewProjection"];
			this.mMatBonesParameter = base.Parameters["MatBones"];
			this.mAlphaBiasParameter = base.Parameters["AlphaBias"];
			this.mDiffuseMapParameter = base.Parameters["DiffuseMap"];
			this.mColorTintParameter = base.Parameters["ColorTint"];
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00008137 File Offset: 0x00006337
		public void SetTechnique(EntangleEffect.Technique iTechnique)
		{
			base.CurrentTechnique = base.Techniques[(int)iTechnique];
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000120 RID: 288 RVA: 0x0000814B File Offset: 0x0000634B
		// (set) Token: 0x06000121 RID: 289 RVA: 0x00008158 File Offset: 0x00006358
		public Vector3 DiffuseColor
		{
			get
			{
				return this.mDiffuseColorParameter.GetValueVector3();
			}
			set
			{
				this.mDiffuseColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00008166 File Offset: 0x00006366
		// (set) Token: 0x06000123 RID: 291 RVA: 0x00008173 File Offset: 0x00006373
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

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00008181 File Offset: 0x00006381
		// (set) Token: 0x06000125 RID: 293 RVA: 0x0000818E File Offset: 0x0000638E
		public float EmissiveAmount
		{
			get
			{
				return this.mEmissiveAmountParameter.GetValueSingle();
			}
			set
			{
				this.mEmissiveAmountParameter.SetValue(value);
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000126 RID: 294 RVA: 0x0000819C File Offset: 0x0000639C
		// (set) Token: 0x06000127 RID: 295 RVA: 0x000081A9 File Offset: 0x000063A9
		public float SpecularAmount
		{
			get
			{
				return this.mSpecularAmountParameter.GetValueSingle();
			}
			set
			{
				this.mSpecularAmountParameter.SetValue(value);
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000128 RID: 296 RVA: 0x000081B7 File Offset: 0x000063B7
		// (set) Token: 0x06000129 RID: 297 RVA: 0x000081C4 File Offset: 0x000063C4
		public float SpecularPower
		{
			get
			{
				return this.mSpecularPowerParameter.GetValueSingle();
			}
			set
			{
				this.mSpecularPowerParameter.SetValue(value);
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600012A RID: 298 RVA: 0x000081D2 File Offset: 0x000063D2
		// (set) Token: 0x0600012B RID: 299 RVA: 0x000081DF File Offset: 0x000063DF
		public bool DiffuseMapEnabled
		{
			get
			{
				return this.mDiffuseMapEnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mDiffuseMapEnabledParameter.SetValue(value);
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600012C RID: 300 RVA: 0x000081ED File Offset: 0x000063ED
		// (set) Token: 0x0600012D RID: 301 RVA: 0x000081FA File Offset: 0x000063FA
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

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00008208 File Offset: 0x00006408
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00008215 File Offset: 0x00006415
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

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00008223 File Offset: 0x00006423
		// (set) Token: 0x06000131 RID: 305 RVA: 0x00008230 File Offset: 0x00006430
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

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000132 RID: 306 RVA: 0x0000823E File Offset: 0x0000643E
		// (set) Token: 0x06000133 RID: 307 RVA: 0x0000824D File Offset: 0x0000644D
		public Matrix[] MatBones
		{
			get
			{
				return this.mMatBonesParameter.GetValueMatrixArray(10);
			}
			set
			{
				this.mMatBonesParameter.SetValue(value);
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000134 RID: 308 RVA: 0x0000825B File Offset: 0x0000645B
		// (set) Token: 0x06000135 RID: 309 RVA: 0x00008268 File Offset: 0x00006468
		public float AlphaBias
		{
			get
			{
				return this.mAlphaBiasParameter.GetValueSingle();
			}
			set
			{
				this.mAlphaBiasParameter.SetValue(value);
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00008276 File Offset: 0x00006476
		// (set) Token: 0x06000137 RID: 311 RVA: 0x00008283 File Offset: 0x00006483
		public Texture2D DiffuseMap
		{
			get
			{
				return this.mDiffuseMapParameter.GetValueTexture2D();
			}
			set
			{
				this.mDiffuseMapParameter.SetValue(value);
			}
		}

		// Token: 0x040000BD RID: 189
		public const int MAXBONES = 10;

		// Token: 0x040000BE RID: 190
		public static readonly int TYPEHASH = typeof(EntangleEffect).GetHashCode();

		// Token: 0x040000BF RID: 191
		private EffectParameter mDiffuseColorParameter;

		// Token: 0x040000C0 RID: 192
		private EffectParameter mEmissiveAmountParameter;

		// Token: 0x040000C1 RID: 193
		private EffectParameter mSpecularAmountParameter;

		// Token: 0x040000C2 RID: 194
		private EffectParameter mSpecularPowerParameter;

		// Token: 0x040000C3 RID: 195
		private EffectParameter mDiffuseMapEnabledParameter;

		// Token: 0x040000C4 RID: 196
		private EffectParameter mViewParameter;

		// Token: 0x040000C5 RID: 197
		private EffectParameter mProjectionParameter;

		// Token: 0x040000C6 RID: 198
		private EffectParameter mViewProjectionParameter;

		// Token: 0x040000C7 RID: 199
		private EffectParameter mMatBonesParameter;

		// Token: 0x040000C8 RID: 200
		private EffectParameter mAlphaBiasParameter;

		// Token: 0x040000C9 RID: 201
		private EffectParameter mDiffuseMapParameter;

		// Token: 0x040000CA RID: 202
		private EffectParameter mColorTintParameter;

		// Token: 0x02000025 RID: 37
		public enum Technique
		{
			// Token: 0x040000CC RID: 204
			Default,
			// Token: 0x040000CD RID: 205
			Additive,
			// Token: 0x040000CE RID: 206
			Depth,
			// Token: 0x040000CF RID: 207
			Shadow
		}
	}
}
