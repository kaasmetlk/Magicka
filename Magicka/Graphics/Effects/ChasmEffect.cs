using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020000C3 RID: 195
	public class ChasmEffect : Effect
	{
		// Token: 0x060005BC RID: 1468 RVA: 0x000214AC File Offset: 0x0001F6AC
		public ChasmEffect() : base(Game.Instance.GraphicsDevice, Game.Instance.Content.Load<Effect>("Shaders/ChasmEffect"))
		{
			this.mDiffuseColorParameter = base.Parameters["DiffuseColor"];
			this.mEmissiveAmountParameter = base.Parameters["EmissiveAmount"];
			this.mSpecularAmountParameter = base.Parameters["SpecularAmount"];
			this.mSpecularBiasParameter = base.Parameters["SpecularBias"];
			this.mSpecularPowerParameter = base.Parameters["SpecularPower"];
			this.mAlphaParameter = base.Parameters["Alpha"];
			this.mNormalPower0Parameter = base.Parameters["NormalPower0"];
			this.mReflectivenessParameter = base.Parameters["Reflectiveness"];
			this.mReflectColorParameter = base.Parameters["ReflectColor"];
			this.mIsLavaParameter = base.Parameters["IsLava"];
			this.mPixelSizeParameter = base.Parameters["PixelSize"];
			this.mDiffuseMap0EnabledParameter = base.Parameters["DiffuseMap0Enabled"];
			this.mDiffuseMap1EnabledParameter = base.Parameters["DiffuseMap1Enabled"];
			this.mSpecularMapEnabledParameter = base.Parameters["SpecularMapEnabled"];
			this.mNormalMapEnabledParameter = base.Parameters["NormalMapEnabled"];
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mViewProjectionParameter = base.Parameters["ViewProjection"];
			this.mBonesParameter = base.Parameters["Bones"];
			this.mCameraPositionParameter = base.Parameters["CameraPosition"];
			this.mTimeParameter = base.Parameters["Time"];
			this.mDiffuseMap0Parameter = base.Parameters["DiffuseMap0"];
			this.mDiffuseMap1Parameter = base.Parameters["DiffuseMap1"];
			this.mNormalMap0Parameter = base.Parameters["NormalMap0"];
			this.mSpecularMap0Parameter = base.Parameters["SpecularMap0"];
			this.mDepthMapParameter = base.Parameters["DepthMap"];
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x00021719 File Offset: 0x0001F919
		// (set) Token: 0x060005BE RID: 1470 RVA: 0x00021726 File Offset: 0x0001F926
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

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x00021734 File Offset: 0x0001F934
		// (set) Token: 0x060005C0 RID: 1472 RVA: 0x00021741 File Offset: 0x0001F941
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

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x0002174F File Offset: 0x0001F94F
		// (set) Token: 0x060005C2 RID: 1474 RVA: 0x0002175C File Offset: 0x0001F95C
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

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x0002176A File Offset: 0x0001F96A
		// (set) Token: 0x060005C4 RID: 1476 RVA: 0x00021777 File Offset: 0x0001F977
		public float SpecularBias
		{
			get
			{
				return this.mSpecularBiasParameter.GetValueSingle();
			}
			set
			{
				this.mSpecularBiasParameter.SetValue(value);
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x00021785 File Offset: 0x0001F985
		// (set) Token: 0x060005C6 RID: 1478 RVA: 0x00021792 File Offset: 0x0001F992
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

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x000217A0 File Offset: 0x0001F9A0
		// (set) Token: 0x060005C8 RID: 1480 RVA: 0x000217AD File Offset: 0x0001F9AD
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

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x000217BB File Offset: 0x0001F9BB
		// (set) Token: 0x060005CA RID: 1482 RVA: 0x000217C8 File Offset: 0x0001F9C8
		public float NormalPower0
		{
			get
			{
				return this.mNormalPower0Parameter.GetValueSingle();
			}
			set
			{
				this.mNormalPower0Parameter.SetValue(value);
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x000217D6 File Offset: 0x0001F9D6
		// (set) Token: 0x060005CC RID: 1484 RVA: 0x000217E3 File Offset: 0x0001F9E3
		public float Reflectiveness
		{
			get
			{
				return this.mReflectivenessParameter.GetValueSingle();
			}
			set
			{
				this.mReflectivenessParameter.SetValue(value);
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060005CD RID: 1485 RVA: 0x000217F1 File Offset: 0x0001F9F1
		// (set) Token: 0x060005CE RID: 1486 RVA: 0x000217FE File Offset: 0x0001F9FE
		public Vector3 ReflectColor
		{
			get
			{
				return this.mReflectColorParameter.GetValueVector3();
			}
			set
			{
				this.mReflectColorParameter.SetValue(value);
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060005CF RID: 1487 RVA: 0x0002180C File Offset: 0x0001FA0C
		// (set) Token: 0x060005D0 RID: 1488 RVA: 0x00021819 File Offset: 0x0001FA19
		public bool IsLava
		{
			get
			{
				return this.mIsLavaParameter.GetValueBoolean();
			}
			set
			{
				this.mIsLavaParameter.SetValue(value);
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x00021827 File Offset: 0x0001FA27
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x00021834 File Offset: 0x0001FA34
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

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x00021842 File Offset: 0x0001FA42
		// (set) Token: 0x060005D4 RID: 1492 RVA: 0x0002184F File Offset: 0x0001FA4F
		public bool DiffuseMap0Enabled
		{
			get
			{
				return this.mDiffuseMap0EnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mDiffuseMap0EnabledParameter.SetValue(value);
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060005D5 RID: 1493 RVA: 0x0002185D File Offset: 0x0001FA5D
		// (set) Token: 0x060005D6 RID: 1494 RVA: 0x0002186A File Offset: 0x0001FA6A
		public bool DiffuseMap1Enabled
		{
			get
			{
				return this.mDiffuseMap1EnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mDiffuseMap1EnabledParameter.SetValue(value);
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060005D7 RID: 1495 RVA: 0x00021878 File Offset: 0x0001FA78
		// (set) Token: 0x060005D8 RID: 1496 RVA: 0x00021885 File Offset: 0x0001FA85
		public bool SpecularMapEnabled
		{
			get
			{
				return this.mSpecularMapEnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mSpecularMapEnabledParameter.SetValue(value);
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x00021893 File Offset: 0x0001FA93
		// (set) Token: 0x060005DA RID: 1498 RVA: 0x000218A0 File Offset: 0x0001FAA0
		public bool NormalMapEnabled
		{
			get
			{
				return this.mNormalMapEnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mNormalMapEnabledParameter.SetValue(value);
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x000218AE File Offset: 0x0001FAAE
		// (set) Token: 0x060005DC RID: 1500 RVA: 0x000218BB File Offset: 0x0001FABB
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

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060005DD RID: 1501 RVA: 0x000218C9 File Offset: 0x0001FAC9
		// (set) Token: 0x060005DE RID: 1502 RVA: 0x000218D6 File Offset: 0x0001FAD6
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

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060005DF RID: 1503 RVA: 0x000218E4 File Offset: 0x0001FAE4
		// (set) Token: 0x060005E0 RID: 1504 RVA: 0x000218F1 File Offset: 0x0001FAF1
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

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x000218FF File Offset: 0x0001FAFF
		// (set) Token: 0x060005E2 RID: 1506 RVA: 0x0002190E File Offset: 0x0001FB0E
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

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060005E3 RID: 1507 RVA: 0x0002191C File Offset: 0x0001FB1C
		// (set) Token: 0x060005E4 RID: 1508 RVA: 0x00021929 File Offset: 0x0001FB29
		public Vector3 CameraPosition
		{
			get
			{
				return this.mCameraPositionParameter.GetValueVector3();
			}
			set
			{
				this.mCameraPositionParameter.SetValue(value);
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060005E5 RID: 1509 RVA: 0x00021937 File Offset: 0x0001FB37
		// (set) Token: 0x060005E6 RID: 1510 RVA: 0x00021944 File Offset: 0x0001FB44
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

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x00021952 File Offset: 0x0001FB52
		// (set) Token: 0x060005E8 RID: 1512 RVA: 0x0002195F File Offset: 0x0001FB5F
		public Texture2D DiffuseMap0
		{
			get
			{
				return this.mDiffuseMap0Parameter.GetValueTexture2D();
			}
			set
			{
				this.mDiffuseMap0Parameter.SetValue(value);
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x0002196D File Offset: 0x0001FB6D
		// (set) Token: 0x060005EA RID: 1514 RVA: 0x0002197A File Offset: 0x0001FB7A
		public Texture2D DiffuseMap1
		{
			get
			{
				return this.mDiffuseMap1Parameter.GetValueTexture2D();
			}
			set
			{
				this.mDiffuseMap1Parameter.SetValue(value);
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x00021988 File Offset: 0x0001FB88
		// (set) Token: 0x060005EC RID: 1516 RVA: 0x00021995 File Offset: 0x0001FB95
		public Texture2D NormalMap0
		{
			get
			{
				return this.mNormalMap0Parameter.GetValueTexture2D();
			}
			set
			{
				this.mNormalMap0Parameter.SetValue(value);
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x000219A3 File Offset: 0x0001FBA3
		// (set) Token: 0x060005EE RID: 1518 RVA: 0x000219B0 File Offset: 0x0001FBB0
		public Texture2D SpecularMap0
		{
			get
			{
				return this.mSpecularMap0Parameter.GetValueTexture2D();
			}
			set
			{
				this.mSpecularMap0Parameter.SetValue(value);
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x000219BE File Offset: 0x0001FBBE
		// (set) Token: 0x060005F0 RID: 1520 RVA: 0x000219CB File Offset: 0x0001FBCB
		public Texture2D DepthMap
		{
			get
			{
				return this.mDepthMapParameter.GetValueTexture2D();
			}
			set
			{
				this.mDepthMapParameter.SetValue(value);
			}
		}

		// Token: 0x04000474 RID: 1140
		public static readonly int TYPEHASH = typeof(ChasmEffect).GetHashCode();

		// Token: 0x04000475 RID: 1141
		private EffectParameter mDiffuseColorParameter;

		// Token: 0x04000476 RID: 1142
		private EffectParameter mEmissiveAmountParameter;

		// Token: 0x04000477 RID: 1143
		private EffectParameter mSpecularAmountParameter;

		// Token: 0x04000478 RID: 1144
		private EffectParameter mSpecularBiasParameter;

		// Token: 0x04000479 RID: 1145
		private EffectParameter mSpecularPowerParameter;

		// Token: 0x0400047A RID: 1146
		private EffectParameter mAlphaParameter;

		// Token: 0x0400047B RID: 1147
		private EffectParameter mNormalPower0Parameter;

		// Token: 0x0400047C RID: 1148
		private EffectParameter mReflectivenessParameter;

		// Token: 0x0400047D RID: 1149
		private EffectParameter mReflectColorParameter;

		// Token: 0x0400047E RID: 1150
		private EffectParameter mIsLavaParameter;

		// Token: 0x0400047F RID: 1151
		private EffectParameter mPixelSizeParameter;

		// Token: 0x04000480 RID: 1152
		private EffectParameter mDiffuseMap0EnabledParameter;

		// Token: 0x04000481 RID: 1153
		private EffectParameter mDiffuseMap1EnabledParameter;

		// Token: 0x04000482 RID: 1154
		private EffectParameter mSpecularMapEnabledParameter;

		// Token: 0x04000483 RID: 1155
		private EffectParameter mNormalMapEnabledParameter;

		// Token: 0x04000484 RID: 1156
		private EffectParameter mViewParameter;

		// Token: 0x04000485 RID: 1157
		private EffectParameter mProjectionParameter;

		// Token: 0x04000486 RID: 1158
		private EffectParameter mViewProjectionParameter;

		// Token: 0x04000487 RID: 1159
		private EffectParameter mBonesParameter;

		// Token: 0x04000488 RID: 1160
		private EffectParameter mCameraPositionParameter;

		// Token: 0x04000489 RID: 1161
		private EffectParameter mTimeParameter;

		// Token: 0x0400048A RID: 1162
		private EffectParameter mDiffuseMap0Parameter;

		// Token: 0x0400048B RID: 1163
		private EffectParameter mDiffuseMap1Parameter;

		// Token: 0x0400048C RID: 1164
		private EffectParameter mNormalMap0Parameter;

		// Token: 0x0400048D RID: 1165
		private EffectParameter mSpecularMap0Parameter;

		// Token: 0x0400048E RID: 1166
		private EffectParameter mDepthMapParameter;
	}
}
