using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020005D0 RID: 1488
	public class GUIHardwareInstancingEffect : Effect
	{
		// Token: 0x06002C7B RID: 11387 RVA: 0x0015DC48 File Offset: 0x0015BE48
		public GUIHardwareInstancingEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("shaders/GUIHardwareInstancing"))
		{
			this.mSpritesTechnique = base.Techniques["Sprites"];
			this.mNumbersTechnique = base.Techniques["Numbers"];
			this.mHealthbarsTechnique = base.Techniques["Healthbars"];
			this.mTextureOffsetsParameter = base.Parameters["TextureOffsets"];
			this.mPositionsParameter = base.Parameters["Positions"];
			this.mScalesParameter = base.Parameters["Scales"];
			this.mValuesParameter = base.Parameters["Values"];
			this.mColorsParameter = base.Parameters["Colors"];
			this.mSaturationParameter = base.Parameters["Saturations"];
			this.mDigitWidthParameter = base.Parameters["DigitWidth"];
			this.mTransformToScreenParameter = base.Parameters["TransformToScreen"];
			this.mTextureParameter = base.Parameters["Texture"];
			this.mWorldPositionsParameter = base.Parameters["WorldPositions"];
			this.mWorldToScreenParameter = base.Parameters["WorldToScreen"];
			this.mScreenRectangleParameter = base.Parameters["ScreenSize"];
		}

		// Token: 0x06002C7C RID: 11388 RVA: 0x0015DDB4 File Offset: 0x0015BFB4
		public void SetTechnique(GUIHardwareInstancingEffect.Technique iTechnique)
		{
			switch (iTechnique)
			{
			case GUIHardwareInstancingEffect.Technique.Sprites:
				base.CurrentTechnique = this.mSpritesTechnique;
				return;
			case GUIHardwareInstancingEffect.Technique.Numbers:
				base.CurrentTechnique = this.mNumbersTechnique;
				return;
			case GUIHardwareInstancingEffect.Technique.Healthbars:
				base.CurrentTechnique = this.mHealthbarsTechnique;
				return;
			default:
				return;
			}
		}

		// Token: 0x06002C7D RID: 11389 RVA: 0x0015DDFC File Offset: 0x0015BFFC
		public void SetScreenSize(int iWidth, int iHeight)
		{
			Matrix value;
			RenderManager.Instance.CreateTransformPixelsToScreen((float)iWidth, (float)iHeight, out value);
			this.mTransformToScreenParameter.SetValue(value);
		}

		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x06002C7E RID: 11390 RVA: 0x0015DE25 File Offset: 0x0015C025
		// (set) Token: 0x06002C7F RID: 11391 RVA: 0x0015DE32 File Offset: 0x0015C032
		public Texture Texture
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

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x06002C80 RID: 11392 RVA: 0x0015DE40 File Offset: 0x0015C040
		// (set) Token: 0x06002C81 RID: 11393 RVA: 0x0015DE4F File Offset: 0x0015C04F
		public Vector3[] WorldPositions
		{
			get
			{
				return this.mWorldPositionsParameter.GetValueVector3Array(40);
			}
			set
			{
				this.mWorldPositionsParameter.SetValue(value);
			}
		}

		// Token: 0x17000A7F RID: 2687
		// (get) Token: 0x06002C82 RID: 11394 RVA: 0x0015DE60 File Offset: 0x0015C060
		// (set) Token: 0x06002C83 RID: 11395 RVA: 0x0015DEA0 File Offset: 0x0015C0A0
		public Point ScreenSize
		{
			get
			{
				Point result = default(Point);
				Vector2 valueVector = this.mScreenRectangleParameter.GetValueVector2();
				result.X = (int)valueVector.X;
				result.Y = (int)valueVector.Y;
				return result;
			}
			set
			{
				Vector2 value2 = default(Vector2);
				value2.X = (float)value.X;
				value2.Y = (float)value.Y;
				this.mScreenRectangleParameter.SetValue(value2);
			}
		}

		// Token: 0x17000A80 RID: 2688
		// (get) Token: 0x06002C84 RID: 11396 RVA: 0x0015DEDF File Offset: 0x0015C0DF
		// (set) Token: 0x06002C85 RID: 11397 RVA: 0x0015DEEC File Offset: 0x0015C0EC
		public Matrix WorldToScreen
		{
			get
			{
				return this.mWorldToScreenParameter.GetValueMatrix();
			}
			set
			{
				this.mWorldToScreenParameter.SetValue(value);
			}
		}

		// Token: 0x17000A81 RID: 2689
		// (get) Token: 0x06002C86 RID: 11398 RVA: 0x0015DEFA File Offset: 0x0015C0FA
		// (set) Token: 0x06002C87 RID: 11399 RVA: 0x0015DF09 File Offset: 0x0015C109
		public float[] Saturations
		{
			get
			{
				return this.mSaturationParameter.GetValueSingleArray(40);
			}
			set
			{
				this.mSaturationParameter.SetValue(value);
			}
		}

		// Token: 0x17000A82 RID: 2690
		// (get) Token: 0x06002C88 RID: 11400 RVA: 0x0015DF17 File Offset: 0x0015C117
		// (set) Token: 0x06002C89 RID: 11401 RVA: 0x0015DF26 File Offset: 0x0015C126
		public Vector2[] TextureOffsets
		{
			get
			{
				return this.mTextureOffsetsParameter.GetValueVector2Array(40);
			}
			set
			{
				this.mTextureOffsetsParameter.SetValue(value);
			}
		}

		// Token: 0x17000A83 RID: 2691
		// (get) Token: 0x06002C8A RID: 11402 RVA: 0x0015DF34 File Offset: 0x0015C134
		// (set) Token: 0x06002C8B RID: 11403 RVA: 0x0015DF43 File Offset: 0x0015C143
		public Vector2[] Positions
		{
			get
			{
				return this.mPositionsParameter.GetValueVector2Array(40);
			}
			set
			{
				this.mPositionsParameter.SetValue(value);
			}
		}

		// Token: 0x17000A84 RID: 2692
		// (get) Token: 0x06002C8C RID: 11404 RVA: 0x0015DF51 File Offset: 0x0015C151
		// (set) Token: 0x06002C8D RID: 11405 RVA: 0x0015DF60 File Offset: 0x0015C160
		public Vector3[] Scales
		{
			get
			{
				return this.mScalesParameter.GetValueVector3Array(40);
			}
			set
			{
				this.mScalesParameter.SetValue(value);
			}
		}

		// Token: 0x17000A85 RID: 2693
		// (get) Token: 0x06002C8E RID: 11406 RVA: 0x0015DF6E File Offset: 0x0015C16E
		// (set) Token: 0x06002C8F RID: 11407 RVA: 0x0015DF7D File Offset: 0x0015C17D
		public float[] Values
		{
			get
			{
				return this.mValuesParameter.GetValueSingleArray(40);
			}
			set
			{
				this.mValuesParameter.SetValue(value);
			}
		}

		// Token: 0x17000A86 RID: 2694
		// (get) Token: 0x06002C90 RID: 11408 RVA: 0x0015DF8B File Offset: 0x0015C18B
		// (set) Token: 0x06002C91 RID: 11409 RVA: 0x0015DF9A File Offset: 0x0015C19A
		public Vector4[] Colors
		{
			get
			{
				return this.mColorsParameter.GetValueVector4Array(40);
			}
			set
			{
				this.mColorsParameter.SetValue(value);
			}
		}

		// Token: 0x17000A87 RID: 2695
		// (get) Token: 0x06002C92 RID: 11410 RVA: 0x0015DFA8 File Offset: 0x0015C1A8
		// (set) Token: 0x06002C93 RID: 11411 RVA: 0x0015DFB5 File Offset: 0x0015C1B5
		public float DigitWidth
		{
			get
			{
				return this.mDigitWidthParameter.GetValueSingle();
			}
			set
			{
				this.mDigitWidthParameter.SetValue(value);
			}
		}

		// Token: 0x0400300C RID: 12300
		public const int MAXINSTANCES = 40;

		// Token: 0x0400300D RID: 12301
		private EffectTechnique mSpritesTechnique;

		// Token: 0x0400300E RID: 12302
		private EffectTechnique mNumbersTechnique;

		// Token: 0x0400300F RID: 12303
		private EffectTechnique mHealthbarsTechnique;

		// Token: 0x04003010 RID: 12304
		private EffectParameter mTextureOffsetsParameter;

		// Token: 0x04003011 RID: 12305
		private EffectParameter mPositionsParameter;

		// Token: 0x04003012 RID: 12306
		private EffectParameter mScalesParameter;

		// Token: 0x04003013 RID: 12307
		private EffectParameter mValuesParameter;

		// Token: 0x04003014 RID: 12308
		private EffectParameter mColorsParameter;

		// Token: 0x04003015 RID: 12309
		private EffectParameter mSaturationParameter;

		// Token: 0x04003016 RID: 12310
		private EffectParameter mDigitWidthParameter;

		// Token: 0x04003017 RID: 12311
		private EffectParameter mTransformToScreenParameter;

		// Token: 0x04003018 RID: 12312
		private EffectParameter mTextureParameter;

		// Token: 0x04003019 RID: 12313
		private EffectParameter mWorldPositionsParameter;

		// Token: 0x0400301A RID: 12314
		private EffectParameter mWorldToScreenParameter;

		// Token: 0x0400301B RID: 12315
		private EffectParameter mScreenRectangleParameter;

		// Token: 0x020005D1 RID: 1489
		public enum Technique
		{
			// Token: 0x0400301D RID: 12317
			Sprites,
			// Token: 0x0400301E RID: 12318
			Numbers,
			// Token: 0x0400301F RID: 12319
			Healthbars
		}
	}
}
