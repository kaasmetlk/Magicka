using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020000C4 RID: 196
	public class ArcaneEffect : Effect
	{
		// Token: 0x060005F2 RID: 1522 RVA: 0x000219F0 File Offset: 0x0001FBF0
		public ArcaneEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/ArcaneEffect"))
		{
			this.mViewProjectionParameter = base.Parameters["ViewProjection"];
			this.mWorldParameter = base.Parameters["World"];
			this.mOriginParameter = base.Parameters["Origin"];
			this.mDirectionParameter = base.Parameters["Direction"];
			this.mColorCenterParameter = base.Parameters["ColorCenter"];
			this.mColorEdgeParameter = base.Parameters["ColorEdge"];
			this.mAlphaParameter = base.Parameters["Alpha"];
			this.mCutParameter = base.Parameters["Cut"];
			this.mEyePosParameter = base.Parameters["EyePos"];
			this.mMinRadiusParameter = base.Parameters["MinRadius"];
			this.mMaxRadiusParameter = base.Parameters["MaxRadius"];
			this.mStartLengthParameter = base.Parameters["StartLength"];
			this.mDropoffParameter = base.Parameters["Dropoff"];
			this.mRayRadiusParameter = base.Parameters["RayRadius"];
			this.mTimeParameter = base.Parameters["Time"];
			this.mTextureScaleParameter = base.Parameters["TextureScale"];
			this.mWaveScaleParameter = base.Parameters["WaveScale"];
			this.mClockwiceParameter = base.Parameters["Clockwice"];
			this.mLengthParameter = base.Parameters["Length"];
			this.mTextureParameter = base.Parameters["Texture"];
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x00021BC7 File Offset: 0x0001FDC7
		public void SetTechnique(ArcaneEffect.Technique iTechnique)
		{
			base.CurrentTechnique = base.Techniques[(int)iTechnique];
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060005F4 RID: 1524 RVA: 0x00021BDB File Offset: 0x0001FDDB
		// (set) Token: 0x060005F5 RID: 1525 RVA: 0x00021BE8 File Offset: 0x0001FDE8
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

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060005F6 RID: 1526 RVA: 0x00021BF6 File Offset: 0x0001FDF6
		// (set) Token: 0x060005F7 RID: 1527 RVA: 0x00021C03 File Offset: 0x0001FE03
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

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060005F8 RID: 1528 RVA: 0x00021C11 File Offset: 0x0001FE11
		// (set) Token: 0x060005F9 RID: 1529 RVA: 0x00021C1E File Offset: 0x0001FE1E
		public Vector3 Origin
		{
			get
			{
				return this.mOriginParameter.GetValueVector3();
			}
			set
			{
				this.mOriginParameter.SetValue(value);
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060005FA RID: 1530 RVA: 0x00021C2C File Offset: 0x0001FE2C
		// (set) Token: 0x060005FB RID: 1531 RVA: 0x00021C39 File Offset: 0x0001FE39
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

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x00021C47 File Offset: 0x0001FE47
		// (set) Token: 0x060005FD RID: 1533 RVA: 0x00021C54 File Offset: 0x0001FE54
		public Vector3 ColorCenter
		{
			get
			{
				return this.mColorCenterParameter.GetValueVector3();
			}
			set
			{
				this.mColorCenterParameter.SetValue(value);
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060005FE RID: 1534 RVA: 0x00021C62 File Offset: 0x0001FE62
		// (set) Token: 0x060005FF RID: 1535 RVA: 0x00021C6F File Offset: 0x0001FE6F
		public Vector3 ColorEdge
		{
			get
			{
				return this.mColorEdgeParameter.GetValueVector3();
			}
			set
			{
				this.mColorEdgeParameter.SetValue(value);
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000600 RID: 1536 RVA: 0x00021C7D File Offset: 0x0001FE7D
		// (set) Token: 0x06000601 RID: 1537 RVA: 0x00021C8A File Offset: 0x0001FE8A
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

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000602 RID: 1538 RVA: 0x00021C98 File Offset: 0x0001FE98
		// (set) Token: 0x06000603 RID: 1539 RVA: 0x00021CA5 File Offset: 0x0001FEA5
		public float Cut
		{
			get
			{
				return this.mCutParameter.GetValueSingle();
			}
			set
			{
				this.mCutParameter.SetValue(value);
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000604 RID: 1540 RVA: 0x00021CB3 File Offset: 0x0001FEB3
		// (set) Token: 0x06000605 RID: 1541 RVA: 0x00021CC0 File Offset: 0x0001FEC0
		public Vector3 EyePos
		{
			get
			{
				return this.mEyePosParameter.GetValueVector3();
			}
			set
			{
				this.mEyePosParameter.SetValue(value);
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x00021CCE File Offset: 0x0001FECE
		// (set) Token: 0x06000607 RID: 1543 RVA: 0x00021CDB File Offset: 0x0001FEDB
		public float MinRadius
		{
			get
			{
				return this.mMinRadiusParameter.GetValueSingle();
			}
			set
			{
				this.mMinRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x00021CE9 File Offset: 0x0001FEE9
		// (set) Token: 0x06000609 RID: 1545 RVA: 0x00021CF6 File Offset: 0x0001FEF6
		public float MaxRadius
		{
			get
			{
				return this.mMaxRadiusParameter.GetValueSingle();
			}
			set
			{
				this.mMaxRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x00021D04 File Offset: 0x0001FF04
		// (set) Token: 0x0600060B RID: 1547 RVA: 0x00021D11 File Offset: 0x0001FF11
		public float StartLength
		{
			get
			{
				return this.mStartLengthParameter.GetValueSingle();
			}
			set
			{
				this.mStartLengthParameter.SetValue(value);
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x00021D1F File Offset: 0x0001FF1F
		// (set) Token: 0x0600060D RID: 1549 RVA: 0x00021D2C File Offset: 0x0001FF2C
		public float Dropoff
		{
			get
			{
				return this.mDropoffParameter.GetValueSingle();
			}
			set
			{
				this.mDropoffParameter.SetValue(value);
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x00021D3A File Offset: 0x0001FF3A
		// (set) Token: 0x0600060F RID: 1551 RVA: 0x00021D47 File Offset: 0x0001FF47
		public float RayRadius
		{
			get
			{
				return this.mRayRadiusParameter.GetValueSingle();
			}
			set
			{
				this.mRayRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x00021D55 File Offset: 0x0001FF55
		// (set) Token: 0x06000611 RID: 1553 RVA: 0x00021D62 File Offset: 0x0001FF62
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

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x00021D70 File Offset: 0x0001FF70
		// (set) Token: 0x06000613 RID: 1555 RVA: 0x00021D7D File Offset: 0x0001FF7D
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

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x00021D8B File Offset: 0x0001FF8B
		// (set) Token: 0x06000615 RID: 1557 RVA: 0x00021D98 File Offset: 0x0001FF98
		public float WaveScale
		{
			get
			{
				return this.mWaveScaleParameter.GetValueSingle();
			}
			set
			{
				this.mWaveScaleParameter.SetValue(value);
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x00021DA6 File Offset: 0x0001FFA6
		// (set) Token: 0x06000617 RID: 1559 RVA: 0x00021DB3 File Offset: 0x0001FFB3
		public bool Clockwice
		{
			get
			{
				return this.mClockwiceParameter.GetValueBoolean();
			}
			set
			{
				this.mClockwiceParameter.SetValue(value);
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x00021DC1 File Offset: 0x0001FFC1
		// (set) Token: 0x06000619 RID: 1561 RVA: 0x00021DCE File Offset: 0x0001FFCE
		public float Length
		{
			get
			{
				return this.mLengthParameter.GetValueSingle();
			}
			set
			{
				this.mLengthParameter.SetValue(value);
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x00021DDC File Offset: 0x0001FFDC
		// (set) Token: 0x0600061B RID: 1563 RVA: 0x00021DE9 File Offset: 0x0001FFE9
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

		// Token: 0x0400048F RID: 1167
		public static readonly int TYPEHASH = typeof(ArcaneEffect).GetHashCode();

		// Token: 0x04000490 RID: 1168
		private EffectParameter mViewProjectionParameter;

		// Token: 0x04000491 RID: 1169
		private EffectParameter mWorldParameter;

		// Token: 0x04000492 RID: 1170
		private EffectParameter mOriginParameter;

		// Token: 0x04000493 RID: 1171
		private EffectParameter mDirectionParameter;

		// Token: 0x04000494 RID: 1172
		private EffectParameter mColorCenterParameter;

		// Token: 0x04000495 RID: 1173
		private EffectParameter mColorEdgeParameter;

		// Token: 0x04000496 RID: 1174
		private EffectParameter mAlphaParameter;

		// Token: 0x04000497 RID: 1175
		private EffectParameter mEyePosParameter;

		// Token: 0x04000498 RID: 1176
		private EffectParameter mMinRadiusParameter;

		// Token: 0x04000499 RID: 1177
		private EffectParameter mMaxRadiusParameter;

		// Token: 0x0400049A RID: 1178
		private EffectParameter mStartLengthParameter;

		// Token: 0x0400049B RID: 1179
		private EffectParameter mDropoffParameter;

		// Token: 0x0400049C RID: 1180
		private EffectParameter mRayRadiusParameter;

		// Token: 0x0400049D RID: 1181
		private EffectParameter mTimeParameter;

		// Token: 0x0400049E RID: 1182
		private EffectParameter mTextureScaleParameter;

		// Token: 0x0400049F RID: 1183
		private EffectParameter mWaveScaleParameter;

		// Token: 0x040004A0 RID: 1184
		private EffectParameter mClockwiceParameter;

		// Token: 0x040004A1 RID: 1185
		private EffectParameter mLengthParameter;

		// Token: 0x040004A2 RID: 1186
		private EffectParameter mTextureParameter;

		// Token: 0x040004A3 RID: 1187
		private EffectParameter mCutParameter;

		// Token: 0x020000C5 RID: 197
		public enum Technique
		{
			// Token: 0x040004A5 RID: 1189
			Generic,
			// Token: 0x040004A6 RID: 1190
			Beam
		}
	}
}
