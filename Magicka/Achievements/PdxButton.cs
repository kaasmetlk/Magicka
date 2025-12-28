using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Achievements
{
	// Token: 0x02000136 RID: 310
	internal class PdxButton : PdxUIElement
	{
		// Token: 0x060008C6 RID: 2246 RVA: 0x000380C0 File Offset: 0x000362C0
		public PdxButton(Texture2D iTexture, Rectangle iOffRectangle, Rectangle iOnRectangle, BitmapFont iFont, int iText)
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			this.mTexture = iTexture;
			this.mTextId = iText;
			this.mFont = iFont;
			if (iFont != null)
			{
				this.mText = new Text(32, this.mFont, TextAlign.Center, false);
			}
			Vector4[] array = new Vector4[8];
			int num = 0;
			PdxWidgetWindow.CreateVertices(array, ref num, ref iOffRectangle);
			PdxWidgetWindow.CreateVertices(array, ref num, ref iOnRectangle);
			lock (graphicsDevice)
			{
				this.mVertices = new VertexBuffer(graphicsDevice, 16 * array.Length, BufferUsage.WriteOnly);
				this.mVertices.SetData<Vector4>(array);
			}
			this.mSize.X = (float)iOffRectangle.Width;
			this.mSize.Y = (float)iOffRectangle.Height;
			this.OnLanguageChanged();
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x060008C7 RID: 2247 RVA: 0x00038198 File Offset: 0x00036398
		// (set) Token: 0x060008C8 RID: 2248 RVA: 0x000381A0 File Offset: 0x000363A0
		public PdxButton.ButtonState State
		{
			get
			{
				return this.mState;
			}
			set
			{
				this.mState = value;
			}
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x000381AC File Offset: 0x000363AC
		public override void Draw(GUIBasicEffect iEffect, float iAlpha)
		{
			iEffect.Transform = new Matrix
			{
				M11 = 1f,
				M22 = 1f,
				M41 = this.mPosition.X,
				M42 = this.mPosition.Y,
				M44 = 1f
			};
			iEffect.Texture = this.mTexture;
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = 1f));
			color.W = iAlpha;
			iEffect.Color = color;
			iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16);
			iEffect.GraphicsDevice.VertexDeclaration = PdxUIElement.sVertexDeclaration;
			iEffect.CommitChanges();
			Vector2 vector = default(Vector2);
			vector.X = this.mPosition.X + (float)Math.Floor((double)(this.mSize.X * 0.5f)) - 1f;
			vector.Y = this.mPosition.Y + (float)Math.Floor((double)((this.mSize.Y - this.mTextSize.Y) * 0.5f));
			if ((this.mState & (PdxButton.ButtonState)3) == (PdxButton.ButtonState)3)
			{
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
				vector.X += 1f;
				vector.Y += 1f;
			}
			else
			{
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			}
			if (this.mText != null)
			{
				if ((this.mState & PdxButton.ButtonState.Over) == PdxButton.ButtonState.Over)
				{
					color.X = (color.Y = (color.Z = 0.627451f));
				}
				else
				{
					color.X = (color.Y = (color.Z = 0.5019608f));
				}
				iEffect.Color = color;
				this.mText.Draw(iEffect, vector.X, vector.Y);
			}
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x000383C8 File Offset: 0x000365C8
		public override void OnLanguageChanged()
		{
			base.OnLanguageChanged();
			if (this.mFont != null)
			{
				this.mText.SetText(AchievementsManager.Instance.GetTranslation(this.mTextId));
				this.mTextSize = this.mFont.MeasureText(this.mText.Characters, true);
			}
		}

		// Token: 0x04000822 RID: 2082
		private const float sUpColor = 0.5019608f;

		// Token: 0x04000823 RID: 2083
		private const float sOverColor = 0.627451f;

		// Token: 0x04000824 RID: 2084
		private Texture2D mTexture;

		// Token: 0x04000825 RID: 2085
		private int mTextId;

		// Token: 0x04000826 RID: 2086
		private Text mText;

		// Token: 0x04000827 RID: 2087
		private BitmapFont mFont;

		// Token: 0x04000828 RID: 2088
		private Vector2 mTextSize;

		// Token: 0x04000829 RID: 2089
		private VertexBuffer mVertices;

		// Token: 0x0400082A RID: 2090
		private PdxButton.ButtonState mState;

		// Token: 0x02000137 RID: 311
		public enum ButtonState
		{
			// Token: 0x0400082C RID: 2092
			Up,
			// Token: 0x0400082D RID: 2093
			Down,
			// Token: 0x0400082E RID: 2094
			Over
		}
	}
}
