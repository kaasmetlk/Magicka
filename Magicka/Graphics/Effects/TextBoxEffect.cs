using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000488 RID: 1160
	public class TextBoxEffect : Effect
	{
		// Token: 0x06002319 RID: 8985 RVA: 0x000FBC70 File Offset: 0x000F9E70
		public TextBoxEffect(GraphicsDevice iDevice, ContentManager iContent) : base(iDevice, iContent.Load<Effect>("shaders/textboxeffect"))
		{
			this.mTechnique1Technique = base.Techniques["Technique1"];
			this.mScreenSizeParameter = base.Parameters["ScreenSize"];
			this.mPositionParameter = base.Parameters["Position"];
			this.mSizeParameter = base.Parameters["Size"];
			this.mScaleParameter = base.Parameters["Scale"];
			this.mBorderSizeParameter = base.Parameters["BorderSize"];
			this.mColorParameter = base.Parameters["Color"];
			this.mTextureParameter = base.Parameters["Texture"];
		}

		// Token: 0x0600231A RID: 8986 RVA: 0x000FBD40 File Offset: 0x000F9F40
		public void SetTechnique(TextBoxEffect.Technique iTechnique)
		{
			if (iTechnique != TextBoxEffect.Technique.Technique1)
			{
				return;
			}
			base.CurrentTechnique = this.mTechnique1Technique;
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x0600231B RID: 8987 RVA: 0x000FBD60 File Offset: 0x000F9F60
		// (set) Token: 0x0600231C RID: 8988 RVA: 0x000FBD68 File Offset: 0x000F9F68
		public Vector2 ScreenSize
		{
			get
			{
				return this.mScreenSize;
			}
			set
			{
				this.mScreenSize = value;
				this.mScreenSizeParameter.SetValue(value);
			}
		}

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x0600231D RID: 8989 RVA: 0x000FBD7D File Offset: 0x000F9F7D
		// (set) Token: 0x0600231E RID: 8990 RVA: 0x000FBD85 File Offset: 0x000F9F85
		public Vector2 Position
		{
			get
			{
				return this.mPosition;
			}
			set
			{
				this.mPosition = value;
				this.mPositionParameter.SetValue(value);
			}
		}

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x0600231F RID: 8991 RVA: 0x000FBD9A File Offset: 0x000F9F9A
		// (set) Token: 0x06002320 RID: 8992 RVA: 0x000FBDA2 File Offset: 0x000F9FA2
		public Vector2 Size
		{
			get
			{
				return this.mSize;
			}
			set
			{
				this.mSize = value;
				this.mSizeParameter.SetValue(value);
			}
		}

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x06002321 RID: 8993 RVA: 0x000FBDB7 File Offset: 0x000F9FB7
		// (set) Token: 0x06002322 RID: 8994 RVA: 0x000FBDBF File Offset: 0x000F9FBF
		public float BorderSize
		{
			get
			{
				return this.mBorderSize;
			}
			set
			{
				this.mBorderSize = value;
				this.mBorderSizeParameter.SetValue(value);
			}
		}

		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x06002323 RID: 8995 RVA: 0x000FBDD4 File Offset: 0x000F9FD4
		// (set) Token: 0x06002324 RID: 8996 RVA: 0x000FBDDC File Offset: 0x000F9FDC
		public float Scale
		{
			get
			{
				return this.mScale;
			}
			set
			{
				this.mScale = value;
				this.mScaleParameter.SetValue(value);
			}
		}

		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x06002325 RID: 8997 RVA: 0x000FBDF1 File Offset: 0x000F9FF1
		// (set) Token: 0x06002326 RID: 8998 RVA: 0x000FBDF9 File Offset: 0x000F9FF9
		public Vector4 Color
		{
			get
			{
				return this.mColor;
			}
			set
			{
				this.mColor = value;
				this.mColorParameter.SetValue(value);
			}
		}

		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x06002327 RID: 8999 RVA: 0x000FBE0E File Offset: 0x000FA00E
		// (set) Token: 0x06002328 RID: 9000 RVA: 0x000FBE16 File Offset: 0x000FA016
		public Texture Texture
		{
			get
			{
				return this.mTexture;
			}
			set
			{
				this.mTexture = value;
				this.mTextureParameter.SetValue(value);
			}
		}

		// Token: 0x04002628 RID: 9768
		private EffectTechnique mTechnique1Technique;

		// Token: 0x04002629 RID: 9769
		private EffectParameter mScreenSizeParameter;

		// Token: 0x0400262A RID: 9770
		private Vector2 mScreenSize;

		// Token: 0x0400262B RID: 9771
		private EffectParameter mPositionParameter;

		// Token: 0x0400262C RID: 9772
		private Vector2 mPosition;

		// Token: 0x0400262D RID: 9773
		private EffectParameter mSizeParameter;

		// Token: 0x0400262E RID: 9774
		private Vector2 mSize;

		// Token: 0x0400262F RID: 9775
		private EffectParameter mBorderSizeParameter;

		// Token: 0x04002630 RID: 9776
		private float mBorderSize;

		// Token: 0x04002631 RID: 9777
		private EffectParameter mScaleParameter;

		// Token: 0x04002632 RID: 9778
		private float mScale;

		// Token: 0x04002633 RID: 9779
		private EffectParameter mColorParameter;

		// Token: 0x04002634 RID: 9780
		private Vector4 mColor;

		// Token: 0x04002635 RID: 9781
		private EffectParameter mTextureParameter;

		// Token: 0x04002636 RID: 9782
		private Texture mTexture;

		// Token: 0x02000489 RID: 1161
		public enum Technique
		{
			// Token: 0x04002638 RID: 9784
			Technique1
		}
	}
}
