using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation.Effects;

namespace Magicka.Graphics
{
	// Token: 0x020000BF RID: 191
	public struct MagickaSkinnedModelMaterial : IMaterial<SkinnedModelBasicEffect>
	{
		// Token: 0x06000590 RID: 1424 RVA: 0x000209E8 File Offset: 0x0001EBE8
		public void AssignOpacityToEffect(SkinnedModelBasicEffect iEffect)
		{
			iEffect.DiffuseMap0 = this.DiffuseMap0;
			iEffect.DiffuseMap0Enabled = this.DiffuseMap0Enabled;
			iEffect.DiffuseMap1 = this.DiffuseMap1;
			iEffect.DiffuseMap1Enabled = this.DiffuseMap1Enabled;
			iEffect.Diffuse1Alpha = this.OverlayAlpha;
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x00020A28 File Offset: 0x0001EC28
		public void AssignToEffect(SkinnedModelBasicEffect iEffect)
		{
			iEffect.DamageMap0 = this.DamageMap0;
			iEffect.DamageMap0Enabled = this.DamageMap0Enabled;
			iEffect.DamageMap1 = this.DamageMap1;
			iEffect.DamageMap1Enabled = this.DamageMap1Enabled;
			iEffect.DiffuseMap0 = this.DiffuseMap0;
			iEffect.DiffuseMap0Enabled = this.DiffuseMap0Enabled;
			iEffect.DiffuseMap1 = this.DiffuseMap1;
			iEffect.DiffuseMap1Enabled = this.DiffuseMap1Enabled;
			iEffect.DiffuseColor = this.DiffuseColor;
			iEffect.EmissiveAmount = this.EmissiveAmount;
			iEffect.SpecularAmount = this.SpecularAmount;
			iEffect.SpecularPower = this.SpecularPower;
			iEffect.TintColor = this.TintColor;
			iEffect.SpecularMap = this.SpecularMap;
			iEffect.SpecularMapEnabled = this.SpecularMapEnabled;
			iEffect.Diffuse1Alpha = this.OverlayAlpha;
			iEffect.UseSoftLightBlend = this.UseSoftLightBlend;
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x00020B04 File Offset: 0x0001ED04
		public void FetchFromEffect(SkinnedModelBasicEffect iEffect)
		{
			this.Technique = iEffect.ActiveTechnique;
			this.DamageMap0 = iEffect.DamageMap0;
			this.DamageMap0Enabled = iEffect.DamageMap0Enabled;
			this.DamageMap1 = iEffect.DamageMap1;
			this.DamageMap1Enabled = iEffect.DamageMap1Enabled;
			this.DiffuseMap0 = iEffect.DiffuseMap0;
			this.DiffuseMap0Enabled = iEffect.DiffuseMap0Enabled;
			this.DiffuseMap1 = iEffect.DiffuseMap1;
			this.DiffuseMap1Enabled = iEffect.DiffuseMap1Enabled;
			this.DiffuseColor = iEffect.DiffuseColor;
			this.EmissiveAmount = iEffect.EmissiveAmount;
			this.SpecularAmount = iEffect.SpecularAmount;
			this.SpecularPower = iEffect.SpecularPower;
			this.TintColor = iEffect.TintColor;
			this.SpecularMap = iEffect.SpecularMap;
			this.SpecularMapEnabled = iEffect.SpecularMapEnabled;
			this.OverlayAlpha = iEffect.Diffuse1Alpha;
			this.UseSoftLightBlend = iEffect.UseSoftLightBlend;
		}

		// Token: 0x04000443 RID: 1091
		public SkinnedModelBasicEffect.Technique Technique;

		// Token: 0x04000444 RID: 1092
		public Texture2D DamageMap0;

		// Token: 0x04000445 RID: 1093
		public bool DamageMap0Enabled;

		// Token: 0x04000446 RID: 1094
		public Texture2D DamageMap1;

		// Token: 0x04000447 RID: 1095
		public bool DamageMap1Enabled;

		// Token: 0x04000448 RID: 1096
		public Texture2D DiffuseMap0;

		// Token: 0x04000449 RID: 1097
		public bool DiffuseMap0Enabled;

		// Token: 0x0400044A RID: 1098
		public Texture2D DiffuseMap1;

		// Token: 0x0400044B RID: 1099
		public bool DiffuseMap1Enabled;

		// Token: 0x0400044C RID: 1100
		public Vector3 DiffuseColor;

		// Token: 0x0400044D RID: 1101
		public float EmissiveAmount;

		// Token: 0x0400044E RID: 1102
		public float SpecularAmount;

		// Token: 0x0400044F RID: 1103
		public float SpecularPower;

		// Token: 0x04000450 RID: 1104
		public float OverlayAlpha;

		// Token: 0x04000451 RID: 1105
		public Vector3 TintColor;

		// Token: 0x04000452 RID: 1106
		public Texture2D SpecularMap;

		// Token: 0x04000453 RID: 1107
		public bool SpecularMapEnabled;

		// Token: 0x04000454 RID: 1108
		public bool UseSoftLightBlend;
	}
}
