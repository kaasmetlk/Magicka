using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020003F2 RID: 1010
	public class SaturationEffect : Effect
	{
		// Token: 0x06001ED2 RID: 7890 RVA: 0x000D7E18 File Offset: 0x000D6018
		public SaturationEffect() : base(Game.Instance.GraphicsDevice, Game.Instance.Content.Load<Effect>("shaders/SaturationEffect"))
		{
			this.mTransformParameter = base.Parameters["Transform"];
			this.mColorParameter = base.Parameters["Color"];
			this.mTextureEnabledParameter = base.Parameters["TextureEnabled"];
			this.mVertexColorEnabledParameter = base.Parameters["VertexColorEnabled"];
			this.mTextureParameter = base.Parameters["Texture"];
			this.mTransformToScreenParameter = base.Parameters["TransformToScreen"];
			this.mTextureOffsetParameter = base.Parameters["TextureOffset"];
			this.mSaturationParameter = base.Parameters["Saturation"];
		}

		// Token: 0x06001ED3 RID: 7891 RVA: 0x000D7EFC File Offset: 0x000D60FC
		public void SetScreenSize(int iWidth, int iHeight)
		{
			Matrix value;
			RenderManager.Instance.CreateTransformPixelsToScreen((float)iWidth, (float)iHeight, out value);
			this.mTransformToScreenParameter.SetValue(value);
		}

		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x06001ED4 RID: 7892 RVA: 0x000D7F25 File Offset: 0x000D6125
		// (set) Token: 0x06001ED5 RID: 7893 RVA: 0x000D7F32 File Offset: 0x000D6132
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

		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x06001ED6 RID: 7894 RVA: 0x000D7F40 File Offset: 0x000D6140
		// (set) Token: 0x06001ED7 RID: 7895 RVA: 0x000D7F4D File Offset: 0x000D614D
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

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06001ED8 RID: 7896 RVA: 0x000D7F5B File Offset: 0x000D615B
		// (set) Token: 0x06001ED9 RID: 7897 RVA: 0x000D7F68 File Offset: 0x000D6168
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

		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06001EDA RID: 7898 RVA: 0x000D7F76 File Offset: 0x000D6176
		// (set) Token: 0x06001EDB RID: 7899 RVA: 0x000D7F83 File Offset: 0x000D6183
		public bool TextureEnabled
		{
			get
			{
				return this.mTextureEnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mTextureEnabledParameter.SetValue(value);
			}
		}

		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06001EDC RID: 7900 RVA: 0x000D7F91 File Offset: 0x000D6191
		// (set) Token: 0x06001EDD RID: 7901 RVA: 0x000D7F9E File Offset: 0x000D619E
		public bool VertexColorEnabled
		{
			get
			{
				return this.mVertexColorEnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mVertexColorEnabledParameter.SetValue(value);
			}
		}

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06001EDE RID: 7902 RVA: 0x000D7FAC File Offset: 0x000D61AC
		// (set) Token: 0x06001EDF RID: 7903 RVA: 0x000D7FB9 File Offset: 0x000D61B9
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

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x06001EE0 RID: 7904 RVA: 0x000D7FC7 File Offset: 0x000D61C7
		// (set) Token: 0x06001EE1 RID: 7905 RVA: 0x000D7FD4 File Offset: 0x000D61D4
		public float Saturation
		{
			get
			{
				return this.mSaturationParameter.GetValueSingle();
			}
			set
			{
				this.mSaturationParameter.SetValue(value);
			}
		}

		// Token: 0x04002148 RID: 8520
		private EffectParameter mTransformParameter;

		// Token: 0x04002149 RID: 8521
		private EffectParameter mColorParameter;

		// Token: 0x0400214A RID: 8522
		private EffectParameter mTextureEnabledParameter;

		// Token: 0x0400214B RID: 8523
		private EffectParameter mVertexColorEnabledParameter;

		// Token: 0x0400214C RID: 8524
		private EffectParameter mTextureParameter;

		// Token: 0x0400214D RID: 8525
		private EffectParameter mTextureOffsetParameter;

		// Token: 0x0400214E RID: 8526
		private EffectParameter mTransformToScreenParameter;

		// Token: 0x0400214F RID: 8527
		private EffectParameter mSaturationParameter;
	}
}
