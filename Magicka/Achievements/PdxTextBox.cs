using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Achievements
{
	// Token: 0x02000138 RID: 312
	internal class PdxTextBox : PdxUIElement
	{
		// Token: 0x060008CB RID: 2251 RVA: 0x0003841C File Offset: 0x0003661C
		public PdxTextBox(Texture2D iTexture, Rectangle iOffRectangle, Rectangle iOnRectangle, Rectangle iCursorRectangle, BitmapFont iFont, int iMaxLength)
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			this.mString = new StringBuilder(iMaxLength, iMaxLength);
			this.mTexture = iTexture;
			this.mFont = iFont;
			this.mText = new Text(iMaxLength, this.mFont, TextAlign.Left, false, false);
			Vector4[] array = new Vector4[12];
			int num = 0;
			PdxWidgetWindow.CreateVertices(array, ref num, ref iOffRectangle);
			PdxWidgetWindow.CreateVertices(array, ref num, ref iOnRectangle);
			PdxWidgetWindow.CreateVertices(array, ref num, ref iCursorRectangle);
			lock (graphicsDevice)
			{
				this.mVertices = new VertexBuffer(graphicsDevice, 16 * array.Length, BufferUsage.WriteOnly);
				this.mVertices.SetData<Vector4>(array);
			}
			this.mSize.X = (float)iOffRectangle.Width;
			this.mSize.Y = (float)iOffRectangle.Height;
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x000384FC File Offset: 0x000366FC
		public override void Draw(GUIBasicEffect iEffect, float iAlpha)
		{
			iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16);
			iEffect.GraphicsDevice.VertexDeclaration = PdxUIElement.sVertexDeclaration;
			Matrix transform = new Matrix
			{
				M11 = 1f,
				M22 = 1f,
				M41 = this.mPosition.X,
				M42 = this.mPosition.Y,
				M44 = 1f
			};
			iEffect.Transform = transform;
			iEffect.Texture = this.mTexture;
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = 1f));
			color.W = iAlpha;
			iEffect.Color = color;
			iEffect.CommitChanges();
			if (this.mActive)
			{
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
			}
			else
			{
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			}
			transform.M41 += 5f;
			transform.M42 += 7f;
			for (int i = 0; i < this.mCursorPosition; i++)
			{
				Glyph glyph;
				this.mFont.GetGlyph(this.mText.Characters[i], out glyph);
				transform.M41 += (float)glyph.AdvanceWidth;
				if (i > 0)
				{
					transform.M41 += (float)this.mFont.CalcKern(this.mText.Characters[i - 1], this.mText.Characters[i]);
				}
			}
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			if (this.mActive & this.mCursorTimer < 0.5f)
			{
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
			}
			iEffect.Color = color;
			this.mText.Draw(iEffect, this.mPosition.X + 5f, this.mPosition.Y + 4f);
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x0003870A File Offset: 0x0003690A
		public void Update(float iDeltaTime)
		{
			this.mCursorTimer = (this.mCursorTimer + iDeltaTime) % 1f;
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x060008CE RID: 2254 RVA: 0x00038720 File Offset: 0x00036920
		// (set) Token: 0x060008CF RID: 2255 RVA: 0x00038728 File Offset: 0x00036928
		public bool Mask
		{
			get
			{
				return this.mMask;
			}
			set
			{
				this.mMask = value;
			}
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x060008D0 RID: 2256 RVA: 0x00038731 File Offset: 0x00036931
		// (set) Token: 0x060008D1 RID: 2257 RVA: 0x00038739 File Offset: 0x00036939
		public bool Active
		{
			get
			{
				return this.mActive;
			}
			set
			{
				if (value & !this.mActive)
				{
					this.mCursorTimer = 0f;
				}
				this.mActive = value;
			}
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x0003875C File Offset: 0x0003695C
		public void Delete()
		{
			if (this.mCursorPosition < this.mString.Length)
			{
				this.mString.Remove(this.mCursorPosition, 1);
			}
			if (this.mMask)
			{
				for (int i = 0; i < this.mString.Length; i++)
				{
					this.mText.Characters[i] = '*';
				}
				if (this.mString.Length < this.mString.MaxCapacity)
				{
					this.mText.Characters[this.mString.Length] = '\0';
				}
				this.mText.MarkAsDirty();
				return;
			}
			this.mText.SetText(this.mString.ToString());
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x00038810 File Offset: 0x00036A10
		public void AppendChar(char iChar)
		{
			if (iChar == '\t' | iChar == '\n' | iChar == '\r')
			{
				return;
			}
			if (iChar == '\b')
			{
				if (this.mCursorPosition <= 0)
				{
					return;
				}
				this.mString.Remove(this.mCursorPosition - 1, 1);
				this.mCursorPosition--;
			}
			else
			{
				if (this.mString.Length >= this.mString.MaxCapacity)
				{
					return;
				}
				this.mString.Insert(this.mCursorPosition, iChar);
				this.mCursorPosition++;
			}
			if (this.mMask)
			{
				for (int i = 0; i < this.mString.Length; i++)
				{
					this.mText.Characters[i] = '*';
				}
				if (this.mString.Length < this.mString.MaxCapacity)
				{
					this.mText.Characters[this.mString.Length] = '\0';
				}
				this.mText.MarkAsDirty();
				return;
			}
			this.mText.SetText(this.mString.ToString());
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x060008D4 RID: 2260 RVA: 0x0003891D File Offset: 0x00036B1D
		// (set) Token: 0x060008D5 RID: 2261 RVA: 0x00038925 File Offset: 0x00036B25
		public int Cursor
		{
			get
			{
				return this.mCursorPosition;
			}
			set
			{
				if (value < 0)
				{
					this.mCursorPosition = 0;
					return;
				}
				if (value > this.mString.Length)
				{
					this.mCursorPosition = this.mString.Length;
					return;
				}
				this.mCursorPosition = value;
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x060008D6 RID: 2262 RVA: 0x0003895A File Offset: 0x00036B5A
		public string String
		{
			get
			{
				return this.mString.ToString();
			}
		}

		// Token: 0x0400082F RID: 2095
		private bool mMask;

		// Token: 0x04000830 RID: 2096
		private bool mActive;

		// Token: 0x04000831 RID: 2097
		private Texture2D mTexture;

		// Token: 0x04000832 RID: 2098
		private StringBuilder mString;

		// Token: 0x04000833 RID: 2099
		private Text mText;

		// Token: 0x04000834 RID: 2100
		private BitmapFont mFont;

		// Token: 0x04000835 RID: 2101
		private VertexBuffer mVertices;

		// Token: 0x04000836 RID: 2102
		private float mCursorTimer;

		// Token: 0x04000837 RID: 2103
		private int mCursorPosition;
	}
}
