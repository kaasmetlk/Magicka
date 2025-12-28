using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000295 RID: 661
	public class DecalHardwareInstancingEffect : Effect
	{
		// Token: 0x06001397 RID: 5015 RVA: 0x0007862C File Offset: 0x0007682C
		public DecalHardwareInstancingEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/DecalHardwareInstancingEffect"))
		{
			this.mDefaultTechnique = base.Techniques["Default"];
			this.mNormalMappedTechnique = base.Techniques["NormalMapped"];
			this.mTransformsParameter = base.Parameters["Transforms"];
			this.mTextureOffsetsParameter = base.Parameters["TextureOffsets"];
			this.mTTLsParameter = base.Parameters["TTLs"];
			this.mViewProjectionParameter = base.Parameters["ViewProjection"];
			this.mDiffuseMapParameter = base.Parameters["DiffuseMap"];
			this.mNormalMapParameter = base.Parameters["NormalMap"];
			this.mTextureScaleParameter = base.Parameters["TextureScale"];
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x00078714 File Offset: 0x00076914
		public void SetTechnique(DecalHardwareInstancingEffect.Technique iTechnique)
		{
			switch (iTechnique)
			{
			case DecalHardwareInstancingEffect.Technique.Default:
				base.CurrentTechnique = this.mDefaultTechnique;
				return;
			case DecalHardwareInstancingEffect.Technique.NormalMapped:
				base.CurrentTechnique = this.mNormalMappedTechnique;
				return;
			default:
				return;
			}
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06001399 RID: 5017 RVA: 0x0007874B File Offset: 0x0007694B
		// (set) Token: 0x0600139A RID: 5018 RVA: 0x0007875A File Offset: 0x0007695A
		public Matrix[] Transforms
		{
			get
			{
				return this.mTransformsParameter.GetValueMatrixArray(32);
			}
			set
			{
				this.mTransformsParameter.SetValue(value);
			}
		}

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x0600139B RID: 5019 RVA: 0x00078768 File Offset: 0x00076968
		// (set) Token: 0x0600139C RID: 5020 RVA: 0x00078777 File Offset: 0x00076977
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

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x0600139D RID: 5021 RVA: 0x00078785 File Offset: 0x00076985
		// (set) Token: 0x0600139E RID: 5022 RVA: 0x00078794 File Offset: 0x00076994
		public Vector3[] TTLs
		{
			get
			{
				return this.mTTLsParameter.GetValueVector3Array(32);
			}
			set
			{
				this.mTTLsParameter.SetValue(value);
			}
		}

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x0600139F RID: 5023 RVA: 0x000787A2 File Offset: 0x000769A2
		// (set) Token: 0x060013A0 RID: 5024 RVA: 0x000787AF File Offset: 0x000769AF
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

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x060013A1 RID: 5025 RVA: 0x000787BD File Offset: 0x000769BD
		// (set) Token: 0x060013A2 RID: 5026 RVA: 0x000787CA File Offset: 0x000769CA
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

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x060013A3 RID: 5027 RVA: 0x000787D8 File Offset: 0x000769D8
		// (set) Token: 0x060013A4 RID: 5028 RVA: 0x000787E5 File Offset: 0x000769E5
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

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x060013A5 RID: 5029 RVA: 0x000787F3 File Offset: 0x000769F3
		// (set) Token: 0x060013A6 RID: 5030 RVA: 0x00078800 File Offset: 0x00076A00
		public Texture2D NormalMap
		{
			get
			{
				return this.mNormalMapParameter.GetValueTexture2D();
			}
			set
			{
				this.mNormalMapParameter.SetValue(value);
			}
		}

		// Token: 0x0400151D RID: 5405
		public const int MAXINSTANCES = 32;

		// Token: 0x0400151E RID: 5406
		public static readonly int TYPEHASH = typeof(DecalHardwareInstancingEffect).GetHashCode();

		// Token: 0x0400151F RID: 5407
		private EffectTechnique mDefaultTechnique;

		// Token: 0x04001520 RID: 5408
		private EffectTechnique mNormalMappedTechnique;

		// Token: 0x04001521 RID: 5409
		private EffectParameter mTransformsParameter;

		// Token: 0x04001522 RID: 5410
		private EffectParameter mTextureOffsetsParameter;

		// Token: 0x04001523 RID: 5411
		private EffectParameter mTTLsParameter;

		// Token: 0x04001524 RID: 5412
		private EffectParameter mViewProjectionParameter;

		// Token: 0x04001525 RID: 5413
		private EffectParameter mDiffuseMapParameter;

		// Token: 0x04001526 RID: 5414
		private EffectParameter mNormalMapParameter;

		// Token: 0x04001527 RID: 5415
		private EffectParameter mTextureScaleParameter;

		// Token: 0x02000296 RID: 662
		public enum Technique
		{
			// Token: 0x04001529 RID: 5417
			Default,
			// Token: 0x0400152A RID: 5418
			NormalMapped
		}
	}
}
