using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000026 RID: 38
	internal struct ForceFieldMaterial : IMaterial<ForceFieldEffect>
	{
		// Token: 0x06000139 RID: 313 RVA: 0x000082A8 File Offset: 0x000064A8
		public void FetchFromEffect(ForceFieldEffect iEffect)
		{
			this.VertexColorEnabled = iEffect.VertexColorEnabled;
			this.Color = iEffect.Color;
			this.Width = iEffect.Width;
			this.AlphaPower = iEffect.AlphaPower;
			this.AlphaFalloffPower = iEffect.AlphaFalloffPower;
			this.MaxRadius = iEffect.MaxRadius;
			this.RippleDistortion = iEffect.RippleDistortion;
			this.MapDistortion = iEffect.MapDistortion;
			this.DisplacementMap = iEffect.DisplacementMap;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008321 File Offset: 0x00006521
		public void AssignOpacityToEffect(ForceFieldEffect iEffect)
		{
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00008324 File Offset: 0x00006524
		public void AssignToEffect(ForceFieldEffect iEffect)
		{
			iEffect.VertexColorEnabled = this.VertexColorEnabled;
			iEffect.Color = this.Color;
			iEffect.Width = this.Width;
			iEffect.AlphaPower = this.AlphaPower;
			iEffect.AlphaFalloffPower = this.AlphaFalloffPower;
			iEffect.MaxRadius = this.MaxRadius;
			iEffect.RippleDistortion = this.RippleDistortion;
			iEffect.MapDistortion = this.MapDistortion;
			iEffect.DisplacementMap = this.DisplacementMap;
		}

		// Token: 0x040000D0 RID: 208
		public bool VertexColorEnabled;

		// Token: 0x040000D1 RID: 209
		public Vector3 Color;

		// Token: 0x040000D2 RID: 210
		public float Width;

		// Token: 0x040000D3 RID: 211
		public float AlphaPower;

		// Token: 0x040000D4 RID: 212
		public float AlphaFalloffPower;

		// Token: 0x040000D5 RID: 213
		public float MaxRadius;

		// Token: 0x040000D6 RID: 214
		public float RippleDistortion;

		// Token: 0x040000D7 RID: 215
		public float MapDistortion;

		// Token: 0x040000D8 RID: 216
		public Texture2D DisplacementMap;
	}
}
