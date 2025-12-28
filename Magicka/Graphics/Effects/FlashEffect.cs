using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020000C2 RID: 194
	public class FlashEffect : Effect
	{
		// Token: 0x060005B9 RID: 1465 RVA: 0x00021455 File Offset: 0x0001F655
		public FlashEffect() : base(Game.Instance.GraphicsDevice, Game.Instance.Content.Load<Effect>("Shaders/FlashEffect"))
		{
			this.mColorParam = base.Parameters["Color"];
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x00021491 File Offset: 0x0001F691
		// (set) Token: 0x060005BB RID: 1467 RVA: 0x0002149E File Offset: 0x0001F69E
		public Vector4 Color
		{
			get
			{
				return this.mColorParam.GetValueVector4();
			}
			set
			{
				this.mColorParam.SetValue(value);
			}
		}

		// Token: 0x04000473 RID: 1139
		private EffectParameter mColorParam;
	}
}
