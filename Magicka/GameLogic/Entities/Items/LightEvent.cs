using System;
using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using PolygonHead.Lights;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A9 RID: 1193
	public struct LightEvent
	{
		// Token: 0x0600240E RID: 9230 RVA: 0x00102E5C File Offset: 0x0010105C
		public LightEvent(ContentReader iInput)
		{
			this.Radius = iInput.ReadSingle();
			this.DiffuseColor = iInput.ReadVector3();
			this.AmbientColor = iInput.ReadVector3();
			this.SpecularAmount = iInput.ReadSingle();
			this.VariationType = (LightVariationType)iInput.ReadByte();
			this.VariationAmount = iInput.ReadSingle();
			this.VariationSpeed = iInput.ReadSingle();
		}

		// Token: 0x0600240F RID: 9231 RVA: 0x00102EBD File Offset: 0x001010BD
		public LightEvent(float iRadius, Vector3 iDiffuseColor, Vector3 iAmbientColor, float iSpecularAmount)
		{
			this.Radius = iRadius;
			this.DiffuseColor = iDiffuseColor;
			this.AmbientColor = iAmbientColor;
			this.SpecularAmount = iSpecularAmount;
			this.VariationType = LightVariationType.None;
			this.VariationAmount = 0f;
			this.VariationSpeed = 0f;
		}

		// Token: 0x06002410 RID: 9232 RVA: 0x00102EF9 File Offset: 0x001010F9
		public LightEvent(float iRadius, Vector3 iDiffuseColor, Vector3 iAmbientColor, float iSpecularAmount, LightVariationType iVariationType, float iVariationAmount, float iVariationSpeed)
		{
			this.Radius = iRadius;
			this.DiffuseColor = iDiffuseColor;
			this.AmbientColor = iAmbientColor;
			this.SpecularAmount = iSpecularAmount;
			this.VariationType = iVariationType;
			this.VariationAmount = iVariationAmount;
			this.VariationSpeed = iVariationSpeed;
		}

		// Token: 0x06002411 RID: 9233 RVA: 0x00102F30 File Offset: 0x00101130
		public void Execute(Entity iItem, Entity iTarget)
		{
			if (iItem is MissileEntity)
			{
				DynamicLight cachedLight = DynamicLight.GetCachedLight();
				cachedLight.Intensity = 1f;
				cachedLight.Position = iItem.Position;
				cachedLight.Radius = this.Radius;
				cachedLight.DiffuseColor = this.DiffuseColor;
				cachedLight.AmbientColor = this.AmbientColor;
				cachedLight.SpecularAmount = this.SpecularAmount;
				cachedLight.VariationType = this.VariationType;
				cachedLight.VariationAmount = this.VariationAmount;
				cachedLight.VariationSpeed = this.VariationSpeed;
				(iItem as MissileEntity).AddLight(cachedLight);
				cachedLight.Enable(iItem.PlayState.Scene);
			}
		}

		// Token: 0x04002712 RID: 10002
		public float Radius;

		// Token: 0x04002713 RID: 10003
		public Vector3 DiffuseColor;

		// Token: 0x04002714 RID: 10004
		public Vector3 AmbientColor;

		// Token: 0x04002715 RID: 10005
		public float SpecularAmount;

		// Token: 0x04002716 RID: 10006
		public LightVariationType VariationType;

		// Token: 0x04002717 RID: 10007
		public float VariationAmount;

		// Token: 0x04002718 RID: 10008
		public float VariationSpeed;
	}
}
