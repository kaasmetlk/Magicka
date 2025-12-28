using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x0200062C RID: 1580
	public class NormalDistortionEffect : Effect
	{
		// Token: 0x06002F9D RID: 12189 RVA: 0x001815DC File Offset: 0x0017F7DC
		public NormalDistortionEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/NormalDistortionEffect"))
		{
			this.mWorldParameter = base.Parameters["World"];
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mTextureScaleParameter = base.Parameters["TextureScale"];
			this.mTimeParameter = base.Parameters["Time"];
			this.mDistortionParameter = base.Parameters["Distortion"];
			this.mPixelSizeParameter = base.Parameters["PixelSize"];
			this.mSourceTextureParameter = base.Parameters["SourceTexture"];
			this.mDepthTextureParameter = base.Parameters["DepthTexture"];
			this.mNormalTextureParameter = base.Parameters["NormalTexture"];
		}

		// Token: 0x17000B4A RID: 2890
		// (get) Token: 0x06002F9E RID: 12190 RVA: 0x001816D7 File Offset: 0x0017F8D7
		// (set) Token: 0x06002F9F RID: 12191 RVA: 0x001816E4 File Offset: 0x0017F8E4
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

		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x06002FA0 RID: 12192 RVA: 0x001816F2 File Offset: 0x0017F8F2
		// (set) Token: 0x06002FA1 RID: 12193 RVA: 0x001816FF File Offset: 0x0017F8FF
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

		// Token: 0x17000B4C RID: 2892
		// (get) Token: 0x06002FA2 RID: 12194 RVA: 0x0018170D File Offset: 0x0017F90D
		// (set) Token: 0x06002FA3 RID: 12195 RVA: 0x0018171A File Offset: 0x0017F91A
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

		// Token: 0x17000B4D RID: 2893
		// (get) Token: 0x06002FA4 RID: 12196 RVA: 0x00181728 File Offset: 0x0017F928
		// (set) Token: 0x06002FA5 RID: 12197 RVA: 0x00181735 File Offset: 0x0017F935
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

		// Token: 0x17000B4E RID: 2894
		// (get) Token: 0x06002FA6 RID: 12198 RVA: 0x00181743 File Offset: 0x0017F943
		// (set) Token: 0x06002FA7 RID: 12199 RVA: 0x00181750 File Offset: 0x0017F950
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

		// Token: 0x17000B4F RID: 2895
		// (get) Token: 0x06002FA8 RID: 12200 RVA: 0x0018175E File Offset: 0x0017F95E
		// (set) Token: 0x06002FA9 RID: 12201 RVA: 0x0018176B File Offset: 0x0017F96B
		public float TextureScale
		{
			get
			{
				return this.mTextureScaleParameter.GetValueSingle();
			}
			set
			{
				this.mTextureScaleParameter.SetValue(value);
			}
		}

		// Token: 0x17000B50 RID: 2896
		// (get) Token: 0x06002FAA RID: 12202 RVA: 0x00181779 File Offset: 0x0017F979
		// (set) Token: 0x06002FAB RID: 12203 RVA: 0x00181786 File Offset: 0x0017F986
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

		// Token: 0x17000B51 RID: 2897
		// (get) Token: 0x06002FAC RID: 12204 RVA: 0x00181794 File Offset: 0x0017F994
		// (set) Token: 0x06002FAD RID: 12205 RVA: 0x001817A1 File Offset: 0x0017F9A1
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

		// Token: 0x17000B52 RID: 2898
		// (get) Token: 0x06002FAE RID: 12206 RVA: 0x001817AF File Offset: 0x0017F9AF
		// (set) Token: 0x06002FAF RID: 12207 RVA: 0x001817BC File Offset: 0x0017F9BC
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

		// Token: 0x17000B53 RID: 2899
		// (get) Token: 0x06002FB0 RID: 12208 RVA: 0x001817CA File Offset: 0x0017F9CA
		// (set) Token: 0x06002FB1 RID: 12209 RVA: 0x001817D7 File Offset: 0x0017F9D7
		public Texture2D NormalTexture
		{
			get
			{
				return this.mNormalTextureParameter.GetValueTexture2D();
			}
			set
			{
				this.mNormalTextureParameter.SetValue(value);
			}
		}

		// Token: 0x04003394 RID: 13204
		private EffectParameter mWorldParameter;

		// Token: 0x04003395 RID: 13205
		private EffectParameter mViewParameter;

		// Token: 0x04003396 RID: 13206
		private EffectParameter mProjectionParameter;

		// Token: 0x04003397 RID: 13207
		private EffectParameter mDistortionParameter;

		// Token: 0x04003398 RID: 13208
		private EffectParameter mTimeParameter;

		// Token: 0x04003399 RID: 13209
		private EffectParameter mTextureScaleParameter;

		// Token: 0x0400339A RID: 13210
		private EffectParameter mPixelSizeParameter;

		// Token: 0x0400339B RID: 13211
		private EffectParameter mSourceTextureParameter;

		// Token: 0x0400339C RID: 13212
		private EffectParameter mDepthTextureParameter;

		// Token: 0x0400339D RID: 13213
		private EffectParameter mNormalTextureParameter;
	}
}
