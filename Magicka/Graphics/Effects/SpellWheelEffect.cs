using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Graphics.Effects
{
	// Token: 0x0200048A RID: 1162
	public class SpellWheelEffect : Effect
	{
		// Token: 0x06002329 RID: 9001 RVA: 0x000FBE2C File Offset: 0x000FA02C
		public SpellWheelEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("shaders/SpellWheel"))
		{
			this.mDirectionRadiusParamter = base.Parameters["DirectionRadius"];
			this.mTargetRadiusParameter = base.Parameters["TargetRadius"];
			this.mTargetPositionParameter = base.Parameters["TargetPosition"];
			this.mTargetDirectionParameter = base.Parameters["TargetDirection"];
			this.mWheelPositionParameter = base.Parameters["WheelPosition"];
			this.mTransformToScreenParameter = base.Parameters["TransformToScreen"];
			this.mScreenRectangleParameter = base.Parameters["ScreenRectangle"];
			this.mAlphaParameter = base.Parameters["Alpha"];
			this.mTextureParameter = base.Parameters["Texture"];
			this.mUseAlphaParameter = base.Parameters["UseAlpha"];
		}

		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x0600232A RID: 9002 RVA: 0x000FBF27 File Offset: 0x000FA127
		// (set) Token: 0x0600232B RID: 9003 RVA: 0x000FBF34 File Offset: 0x000FA134
		public bool UseAlpha
		{
			get
			{
				return this.mUseAlphaParameter.GetValueBoolean();
			}
			set
			{
				this.mUseAlphaParameter.SetValue(value);
			}
		}

		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x0600232C RID: 9004 RVA: 0x000FBF42 File Offset: 0x000FA142
		// (set) Token: 0x0600232D RID: 9005 RVA: 0x000FBF4F File Offset: 0x000FA14F
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

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x0600232E RID: 9006 RVA: 0x000FBF5D File Offset: 0x000FA15D
		// (set) Token: 0x0600232F RID: 9007 RVA: 0x000FBF6A File Offset: 0x000FA16A
		public float Alpha
		{
			get
			{
				return this.mAlphaParameter.GetValueSingle();
			}
			set
			{
				this.mAlphaParameter.SetValue(value);
			}
		}

		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x06002330 RID: 9008 RVA: 0x000FBF78 File Offset: 0x000FA178
		// (set) Token: 0x06002331 RID: 9009 RVA: 0x000FBFD8 File Offset: 0x000FA1D8
		public Rectangle ScreenRectangle
		{
			get
			{
				Rectangle result = default(Rectangle);
				Vector4 valueVector = this.mScreenRectangleParameter.GetValueVector4();
				result.X = (int)valueVector.X;
				result.Y = (int)valueVector.Y;
				result.Width = (int)valueVector.Z;
				result.Height = (int)valueVector.W;
				return result;
			}
			set
			{
				Vector4 value2 = new Vector4((float)value.X, (float)value.Y, (float)value.Width, (float)value.Height);
				this.mScreenRectangleParameter.SetValue(value2);
			}
		}

		// Token: 0x06002332 RID: 9010 RVA: 0x000FC018 File Offset: 0x000FA218
		public void SetScreenSize(int iWidth, int iHeight)
		{
			Matrix value;
			RenderManager.Instance.CreateTransformPixelsToScreen((float)iWidth, (float)iHeight, out value);
			this.mTransformToScreenParameter.SetValue(value);
		}

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x06002333 RID: 9011 RVA: 0x000FC041 File Offset: 0x000FA241
		// (set) Token: 0x06002334 RID: 9012 RVA: 0x000FC04E File Offset: 0x000FA24E
		public float TargetRadius
		{
			get
			{
				return this.mTargetRadiusParameter.GetValueSingle();
			}
			set
			{
				this.mTargetRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x06002335 RID: 9013 RVA: 0x000FC05C File Offset: 0x000FA25C
		// (set) Token: 0x06002336 RID: 9014 RVA: 0x000FC069 File Offset: 0x000FA269
		public float DirectionRadius
		{
			get
			{
				return this.mDirectionRadiusParamter.GetValueSingle();
			}
			set
			{
				this.mDirectionRadiusParamter.SetValue(value);
			}
		}

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x06002337 RID: 9015 RVA: 0x000FC077 File Offset: 0x000FA277
		// (set) Token: 0x06002338 RID: 9016 RVA: 0x000FC084 File Offset: 0x000FA284
		public Vector2 TargetPosition
		{
			get
			{
				return this.mTargetPositionParameter.GetValueVector2();
			}
			set
			{
				this.mTargetPositionParameter.SetValue(value);
			}
		}

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x06002339 RID: 9017 RVA: 0x000FC092 File Offset: 0x000FA292
		// (set) Token: 0x0600233A RID: 9018 RVA: 0x000FC09F File Offset: 0x000FA29F
		public Vector2 TargetDirection
		{
			get
			{
				return this.mTargetDirectionParameter.GetValueVector2();
			}
			set
			{
				this.mTargetDirectionParameter.SetValue(value);
			}
		}

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x0600233B RID: 9019 RVA: 0x000FC0AD File Offset: 0x000FA2AD
		// (set) Token: 0x0600233C RID: 9020 RVA: 0x000FC0BA File Offset: 0x000FA2BA
		public Vector2 WheelPosition
		{
			get
			{
				return this.mWheelPositionParameter.GetValueVector2();
			}
			set
			{
				this.mWheelPositionParameter.SetValue(value);
			}
		}

		// Token: 0x04002639 RID: 9785
		private EffectParameter mDirectionRadiusParamter;

		// Token: 0x0400263A RID: 9786
		private EffectParameter mTargetRadiusParameter;

		// Token: 0x0400263B RID: 9787
		private EffectParameter mTargetDirectionParameter;

		// Token: 0x0400263C RID: 9788
		private EffectParameter mTargetPositionParameter;

		// Token: 0x0400263D RID: 9789
		private EffectParameter mWheelPositionParameter;

		// Token: 0x0400263E RID: 9790
		private EffectParameter mTransformToScreenParameter;

		// Token: 0x0400263F RID: 9791
		private EffectParameter mScreenRectangleParameter;

		// Token: 0x04002640 RID: 9792
		private EffectParameter mAlphaParameter;

		// Token: 0x04002641 RID: 9793
		private EffectParameter mUseAlphaParameter;

		// Token: 0x04002642 RID: 9794
		private EffectParameter mTextureParameter;
	}
}
