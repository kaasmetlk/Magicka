using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;

namespace Magicka.Graphics.Effects
{
	// Token: 0x0200039B RID: 923
	public class MistEffect : PostProcessingEffect
	{
		// Token: 0x06001C43 RID: 7235 RVA: 0x000C0A3B File Offset: 0x000BEC3B
		public MistEffect(GraphicsDevice iGraphicsDevice, ContentManager iContent) : base(iGraphicsDevice, iContent.Load<Effect>("Shaders/Mist"))
		{
		}
	}
}
