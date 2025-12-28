using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000294 RID: 660
	public class NotifierButtonEffect : Effect
	{
		// Token: 0x0600138A RID: 5002 RVA: 0x000784D0 File Offset: 0x000766D0
		public NotifierButtonEffect(GraphicsDevice iDevice, ContentManager iContent) : base(iDevice, iContent.Load<Effect>("shaders/NotifierButtonEffect"))
		{
			this.mScreenSizeParameter = base.Parameters["ScreenSize"];
			this.mPositionParameter = base.Parameters["Position"];
			this.mWidthParameter = base.Parameters["Width"];
			this.mColorParameter = base.Parameters["Color"];
			this.mAlphaParameter = base.Parameters["Alpha"];
			this.mTextureParameter = base.Parameters["Texture"];
			this.mScaleParameter = base.Parameters["Scale"];
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x0600138B RID: 5003 RVA: 0x00078589 File Offset: 0x00076789
		// (set) Token: 0x0600138C RID: 5004 RVA: 0x00078596 File Offset: 0x00076796
		public Vector2 ScreenSize
		{
			get
			{
				return this.mScreenSizeParameter.GetValueVector2();
			}
			set
			{
				this.mScreenSizeParameter.SetValue(value);
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x0600138D RID: 5005 RVA: 0x000785A4 File Offset: 0x000767A4
		// (set) Token: 0x0600138E RID: 5006 RVA: 0x000785B1 File Offset: 0x000767B1
		public Vector4 Color
		{
			get
			{
				return this.mColorParameter.GetValueVector4();
			}
			set
			{
				this.mColorParameter.SetValue(value);
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x0600138F RID: 5007 RVA: 0x000785BF File Offset: 0x000767BF
		// (set) Token: 0x06001390 RID: 5008 RVA: 0x000785CC File Offset: 0x000767CC
		public Vector2 Position
		{
			get
			{
				return this.mPositionParameter.GetValueVector2();
			}
			set
			{
				this.mPositionParameter.SetValue(value);
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06001391 RID: 5009 RVA: 0x000785DA File Offset: 0x000767DA
		// (set) Token: 0x06001392 RID: 5010 RVA: 0x000785E7 File Offset: 0x000767E7
		public float Width
		{
			get
			{
				return this.mWidthParameter.GetValueSingle();
			}
			set
			{
				this.mWidthParameter.SetValue(value);
			}
		}

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06001393 RID: 5011 RVA: 0x000785F5 File Offset: 0x000767F5
		// (set) Token: 0x06001394 RID: 5012 RVA: 0x00078602 File Offset: 0x00076802
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

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06001395 RID: 5013 RVA: 0x00078610 File Offset: 0x00076810
		// (set) Token: 0x06001396 RID: 5014 RVA: 0x0007861D File Offset: 0x0007681D
		public Vector2 Scale
		{
			get
			{
				return this.mScaleParameter.GetValueVector2();
			}
			set
			{
				this.mScaleParameter.SetValue(value);
			}
		}

		// Token: 0x04001516 RID: 5398
		private EffectParameter mScreenSizeParameter;

		// Token: 0x04001517 RID: 5399
		private EffectParameter mPositionParameter;

		// Token: 0x04001518 RID: 5400
		private EffectParameter mWidthParameter;

		// Token: 0x04001519 RID: 5401
		private EffectParameter mColorParameter;

		// Token: 0x0400151A RID: 5402
		private EffectParameter mAlphaParameter;

		// Token: 0x0400151B RID: 5403
		private EffectParameter mTextureParameter;

		// Token: 0x0400151C RID: 5404
		private EffectParameter mScaleParameter;
	}
}
