using System;
using System.Collections.Generic;
using System.Text;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x020002F0 RID: 752
	internal class ContextMenu : MenuItem
	{
		// Token: 0x06001720 RID: 5920 RVA: 0x00094DB0 File Offset: 0x00092FB0
		public ContextMenu(BitmapFont iFont, TextAlign iAlignment, int? iWidth)
		{
			this.mNames = new List<int[]>();
			this.mSelectedIndex = 0;
			this.mAlignment = iAlignment;
			this.mFont = iFont;
			this.mTexts = new List<Text>();
			this.mTextScales = new List<float>();
			this.mAutoSize = (iWidth == null);
			if (!this.mAutoSize)
			{
				this.mWidth = iWidth.Value;
			}
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (ContextMenu.sTexture == null || ContextMenu.sTexture.IsDisposed)
			{
				lock (graphicsDevice)
				{
					if (ContextMenu.sTexture == null || ContextMenu.sTexture.IsDisposed)
					{
						ContextMenu.sTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
					}
				}
			}
			Vector2 vector = default(Vector2);
			vector.X = 1f / (float)ContextMenu.sTexture.Width;
			vector.Y = 1f / (float)ContextMenu.sTexture.Height;
			this.mVertices = new Vector4[16];
			lock (graphicsDevice)
			{
				this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
				this.mVertexBuffer = new VertexBuffer(graphicsDevice, 320, BufferUsage.WriteOnly);
				if (ContextMenu.sIndices == null || ContextMenu.sIndices.IsDisposed)
				{
					ContextMenu.sIndices = new IndexBuffer(graphicsDevice, TextBox.INDICES.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
					ContextMenu.sIndices.SetData<ushort>(TextBox.INDICES);
				}
			}
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x00094F8C File Offset: 0x0009318C
		public int AddOption(int iLocValue)
		{
			lock (this.mNames)
			{
				this.mTextScales.Add(1f);
				this.mTexts.Add(new Text(64, this.mFont, this.mAlignment, false));
				int[] item = new int[]
				{
					iLocValue
				};
				this.mNames.Add(item);
			}
			this.LanguageChanged();
			return this.mTexts.Count - 1;
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x0009501C File Offset: 0x0009321C
		public virtual void RemoveAt(int iIndex)
		{
			this.mNames.RemoveAt(iIndex);
			this.mTexts.RemoveAt(iIndex);
			this.mTextScales.RemoveAt(iIndex);
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x00095042 File Offset: 0x00093242
		public virtual void Clear()
		{
			this.mNames.Clear();
			this.mTexts.Clear();
			this.mTextScales.Clear();
		}

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06001724 RID: 5924 RVA: 0x00095065 File Offset: 0x00093265
		public Vector4 ColorItem
		{
			get
			{
				return this.mColorItem;
			}
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06001725 RID: 5925 RVA: 0x0009506D File Offset: 0x0009326D
		public Vector2 Size
		{
			get
			{
				return this.mSize;
			}
		}

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06001726 RID: 5926 RVA: 0x00095075 File Offset: 0x00093275
		public BitmapFont Font
		{
			get
			{
				return this.mFont;
			}
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06001727 RID: 5927 RVA: 0x0009507D File Offset: 0x0009327D
		public int Count
		{
			get
			{
				return this.mNames.Count;
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06001728 RID: 5928 RVA: 0x0009508A File Offset: 0x0009328A
		// (set) Token: 0x06001729 RID: 5929 RVA: 0x00095092 File Offset: 0x00093292
		public int SelectedIndex
		{
			get
			{
				return this.mSelectedIndex;
			}
			set
			{
				this.mSelectedIndex = value;
			}
		}

		// Token: 0x0600172A RID: 5930 RVA: 0x0009509C File Offset: 0x0009329C
		protected void UpdateVertices()
		{
			Vector2 vector = default(Vector2);
			vector.X = 1f / (float)ContextMenu.sTexture.Width;
			vector.Y = 1f / (float)ContextMenu.sTexture.Height;
			Vector2 vector2 = this.mSize;
			vector2.Y *= (float)this.mNames.Count;
			vector2.Y += 32f;
			Vector2 vector3 = default(Vector2);
			vector3.X = (vector3.Y = 16f);
			Vector2 vector4 = default(Vector2);
			vector4.X = 832f * vector.X;
			vector4.Y = 128f * vector.Y;
			Vector2 vector5 = default(Vector2);
			vector5.X = 128f * vector.X;
			vector5.Y = 128f * vector.Y;
			Vector2 vector6 = default(Vector2);
			vector6.X = 16f * vector.X;
			vector6.Y = 16f * vector.Y;
			SubMenuCharacterSelect.CreateVertices(this.mVertices, 0, ref vector2, ref vector3, ref vector4, ref vector5, ref vector6);
			lock (this.mVertexBuffer.GraphicsDevice)
			{
				this.mVertexBuffer.SetData<Vector4>(this.mVertices);
			}
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x0009521C File Offset: 0x0009341C
		protected override void UpdateBoundingBox()
		{
			if (this.mIsVisible)
			{
				this.mTopLeft.Y = this.mPosition.Y;
				this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * (float)this.mNames.Count * this.mScale;
			}
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x00095280 File Offset: 0x00093480
		internal int GetHitIndex(ref Vector2 iPoint)
		{
			if (!this.mIsVisible)
			{
				return -1;
			}
			float x = this.mPosition.X;
			float num = this.mPosition.Y + 16f * this.mScale;
			float num2 = this.mPosition.X + this.mSize.X * this.mScale;
			float num3 = num + this.mSize.Y * this.mScale;
			if (iPoint.X < x || iPoint.X > num2)
			{
				return -1;
			}
			for (int i = 0; i < this.mNames.Count; i++)
			{
				if (iPoint.Y >= num && iPoint.Y <= num3)
				{
					return i;
				}
				num += this.mSize.Y * this.mScale;
				num3 += this.mSize.Y * this.mScale;
			}
			return -1;
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x00095360 File Offset: 0x00093560
		public void Show(int iX, int iY)
		{
			this.mPosition.X = (float)iX;
			this.mPosition.Y = (float)iY;
			if (this.mNames.Count == 0)
			{
				return;
			}
			if (!this.mIsVisible)
			{
				this.mSelectedIndex = -1;
				this.UpdateBoundingBox();
			}
			this.mIsVisible = true;
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x000953B1 File Offset: 0x000935B1
		public void Hide()
		{
			this.mIsVisible = false;
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x0600172F RID: 5935 RVA: 0x000953BA File Offset: 0x000935BA
		public bool IsVisible
		{
			get
			{
				return this.mIsVisible;
			}
		}

		// Token: 0x06001730 RID: 5936 RVA: 0x000953C4 File Offset: 0x000935C4
		public void Update(float iDeltaTime)
		{
			if (this.mIsVisible && this.mEnabled)
			{
				this.mDownAlpha = Math.Min(this.mDownAlpha + iDeltaTime * 4f, 1f);
				return;
			}
			this.mDownAlpha = Math.Max(this.mDownAlpha - iDeltaTime * 4f, 0f);
		}

		// Token: 0x06001731 RID: 5937 RVA: 0x0009541E File Offset: 0x0009361E
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x00095430 File Offset: 0x00093630
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			if (!this.mIsVisible)
			{
				return;
			}
			float num = this.mDownAlpha;
			if (num > 0f)
			{
				Vector4 color;
				color.X = (color.Y = (color.Z = 1f));
				color.W = num;
				iEffect.Color = color;
				Matrix transform = new Matrix
				{
					M11 = iScale,
					M22 = iScale,
					M41 = this.mPosition.X,
					M42 = this.mPosition.Y,
					M44 = 1f
				};
				iEffect.Transform = transform;
				iEffect.Texture = ContextMenu.sTexture;
				iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
				iEffect.GraphicsDevice.Indices = ContextMenu.sIndices;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 16, 0, 18);
				Vector2 mPosition = this.mPosition;
				mPosition.X += (this.mSize.X - 16f) * iScale;
				mPosition.Y += 16f * iScale;
				lock (this.mNames)
				{
					for (int i = 0; i < this.mNames.Count; i++)
					{
						color = ((i == this.mSelectedIndex) ? this.mColorSelected : this.mColorItem);
						color.W *= num * num;
						iEffect.Color = color;
						transform.M11 = this.mTextScales[i] * iScale;
						transform.M22 = iScale;
						switch (this.mAlignment)
						{
						case TextAlign.Left:
							transform.M41 = mPosition.X - (float)this.mWidth;
							break;
						case TextAlign.Center:
							transform.M41 = mPosition.X - (float)(this.mWidth / 2);
							break;
						case TextAlign.Right:
							transform.M41 = mPosition.X;
							break;
						}
						transform.M42 = mPosition.Y;
						this.mTexts[i].Draw(iEffect, ref transform);
						mPosition.Y += this.mSize.Y * iScale;
					}
				}
			}
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x000956BC File Offset: 0x000938BC
		public override void LanguageChanged()
		{
			LanguageManager instance = LanguageManager.Instance;
			lock (this.mNames)
			{
				for (int i = 0; i < this.mNames.Count; i++)
				{
					StringBuilder stringBuilder = new StringBuilder();
					int[] array = this.mNames[i];
					for (int j = 0; j < array.Length; j++)
					{
						string @string = instance.GetString(array[j]);
						stringBuilder.Append(@string);
						if (j < array.Length - 1)
						{
							stringBuilder.Append(" - ");
						}
					}
					string text = stringBuilder.ToString();
					this.mTexts[i].SetText(text);
					if (this.mAutoSize)
					{
						this.mWidth = (int)Math.Max(this.mFont.MeasureText(text, true).X, (float)this.mWidth);
						this.mTextScales[i] = 1f;
					}
					else
					{
						this.mTextScales[i] = Math.Min(((float)this.mWidth - 32f) / this.mFont.MeasureText(text, true).X, 1f);
					}
				}
			}
			this.mSize = new Vector2
			{
				X = (float)this.mWidth + 32f,
				Y = (float)this.mFont.LineHeight
			};
			this.UpdateVertices();
			this.UpdateBoundingBox();
		}

		// Token: 0x040018A3 RID: 6307
		private const float OFFSETX = 832f;

		// Token: 0x040018A4 RID: 6308
		private const float OFFSETY = 128f;

		// Token: 0x040018A5 RID: 6309
		private const float SIZE = 128f;

		// Token: 0x040018A6 RID: 6310
		protected const float MARGIN = 16f;

		// Token: 0x040018A7 RID: 6311
		protected int mSelectedIndex;

		// Token: 0x040018A8 RID: 6312
		protected List<int[]> mNames;

		// Token: 0x040018A9 RID: 6313
		protected List<Text> mTexts;

		// Token: 0x040018AA RID: 6314
		protected List<float> mTextScales;

		// Token: 0x040018AB RID: 6315
		protected BitmapFont mFont;

		// Token: 0x040018AC RID: 6316
		protected int mWidth;

		// Token: 0x040018AD RID: 6317
		protected Vector2 mSize;

		// Token: 0x040018AE RID: 6318
		private bool mIsVisible;

		// Token: 0x040018AF RID: 6319
		private float mDownAlpha;

		// Token: 0x040018B0 RID: 6320
		private Vector4 mColorItem = Defines.DIALOGUE_COLOR_DEFAULT;

		// Token: 0x040018B1 RID: 6321
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x040018B2 RID: 6322
		private Vector4[] mVertices;

		// Token: 0x040018B3 RID: 6323
		private VertexBuffer mVertexBuffer;

		// Token: 0x040018B4 RID: 6324
		private static IndexBuffer sIndices;

		// Token: 0x040018B5 RID: 6325
		private static Texture2D sTexture;

		// Token: 0x040018B6 RID: 6326
		protected TextAlign mAlignment;

		// Token: 0x040018B7 RID: 6327
		protected bool mAutoSize;
	}
}
