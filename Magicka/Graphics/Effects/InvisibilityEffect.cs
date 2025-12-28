using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020002D4 RID: 724
	public class InvisibilityEffect : Effect
	{
		// Token: 0x06001635 RID: 5685 RVA: 0x0008D584 File Offset: 0x0008B784
		public InvisibilityEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/InvisibilityEffect"))
		{
			this.mBonesParameter = base.Parameters["Bones"];
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mViewProjectionParameter = base.Parameters["ViewProjection"];
			this.mDistortionParameter = base.Parameters["Distortion"];
			this.mBloatParameter = base.Parameters["Bloat"];
			this.mPixelSizeParameter = base.Parameters["PixelSize"];
			this.mSourceTextureParameter = base.Parameters["SourceMap"];
			this.mDepthTextureParameter = base.Parameters["DepthMap"];
		}

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06001636 RID: 5686 RVA: 0x0008D669 File Offset: 0x0008B869
		// (set) Token: 0x06001637 RID: 5687 RVA: 0x0008D678 File Offset: 0x0008B878
		public Matrix[] Bones
		{
			get
			{
				return this.mBonesParameter.GetValueMatrixArray(80);
			}
			set
			{
				this.mBonesParameter.SetValue(value);
			}
		}

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x06001638 RID: 5688 RVA: 0x0008D686 File Offset: 0x0008B886
		// (set) Token: 0x06001639 RID: 5689 RVA: 0x0008D693 File Offset: 0x0008B893
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

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x0600163A RID: 5690 RVA: 0x0008D6A1 File Offset: 0x0008B8A1
		// (set) Token: 0x0600163B RID: 5691 RVA: 0x0008D6AE File Offset: 0x0008B8AE
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

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x0600163C RID: 5692 RVA: 0x0008D6BC File Offset: 0x0008B8BC
		// (set) Token: 0x0600163D RID: 5693 RVA: 0x0008D6C9 File Offset: 0x0008B8C9
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

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x0600163E RID: 5694 RVA: 0x0008D6D7 File Offset: 0x0008B8D7
		// (set) Token: 0x0600163F RID: 5695 RVA: 0x0008D6E4 File Offset: 0x0008B8E4
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

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x06001640 RID: 5696 RVA: 0x0008D6F2 File Offset: 0x0008B8F2
		// (set) Token: 0x06001641 RID: 5697 RVA: 0x0008D6FF File Offset: 0x0008B8FF
		public float Bloat
		{
			get
			{
				return this.mBloatParameter.GetValueSingle();
			}
			set
			{
				this.mBloatParameter.SetValue(value);
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06001642 RID: 5698 RVA: 0x0008D70D File Offset: 0x0008B90D
		// (set) Token: 0x06001643 RID: 5699 RVA: 0x0008D71A File Offset: 0x0008B91A
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

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06001644 RID: 5700 RVA: 0x0008D728 File Offset: 0x0008B928
		// (set) Token: 0x06001645 RID: 5701 RVA: 0x0008D735 File Offset: 0x0008B935
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

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06001646 RID: 5702 RVA: 0x0008D743 File Offset: 0x0008B943
		// (set) Token: 0x06001647 RID: 5703 RVA: 0x0008D750 File Offset: 0x0008B950
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

		// Token: 0x0400176F RID: 5999
		private EffectParameter mBonesParameter;

		// Token: 0x04001770 RID: 6000
		private EffectParameter mViewParameter;

		// Token: 0x04001771 RID: 6001
		private EffectParameter mProjectionParameter;

		// Token: 0x04001772 RID: 6002
		private EffectParameter mViewProjectionParameter;

		// Token: 0x04001773 RID: 6003
		private EffectParameter mDistortionParameter;

		// Token: 0x04001774 RID: 6004
		private EffectParameter mBloatParameter;

		// Token: 0x04001775 RID: 6005
		private EffectParameter mPixelSizeParameter;

		// Token: 0x04001776 RID: 6006
		private EffectParameter mSourceTextureParameter;

		// Token: 0x04001777 RID: 6007
		private EffectParameter mDepthTextureParameter;
	}
}
