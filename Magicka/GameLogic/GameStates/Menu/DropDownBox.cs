using System;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x02000518 RID: 1304
	internal abstract class DropDownBox : MenuItem
	{
		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06002738 RID: 10040 RVA: 0x0011DBE9 File Offset: 0x0011BDE9
		// (remove) Token: 0x06002739 RID: 10041 RVA: 0x0011DC02 File Offset: 0x0011BE02
		public event Action<DropDownBox, int> SelectedIndexChanged;

		// Token: 0x0600273A RID: 10042 RVA: 0x0011DC1C File Offset: 0x0011BE1C
		public DropDownBox(BitmapFont iFont, string[] iNames, int iWidth)
		{
			this.mWidth = iWidth;
			this.mFont = iFont;
			this.SetNames(iNames);
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (DropDownBox.sTexture == null || DropDownBox.sTexture.IsDisposed)
			{
				lock (graphicsDevice)
				{
					if (DropDownBox.sTexture == null || DropDownBox.sTexture.IsDisposed)
					{
						DropDownBox.sTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
					}
				}
			}
			Vector2 vector = default(Vector2);
			vector.X = 1f / (float)DropDownBox.sTexture.Width;
			vector.Y = 1f / (float)DropDownBox.sTexture.Height;
			this.mVertices = new Vector4[20];
			this.mVertices[0].X = 0f;
			this.mVertices[0].Y = 0f;
			this.mVertices[0].Z = 802f * vector.X;
			this.mVertices[0].W = 130f * vector.Y;
			this.mVertices[1].X = 28f;
			this.mVertices[1].Y = 0f;
			this.mVertices[1].Z = 830f * vector.X;
			this.mVertices[1].W = 130f * vector.Y;
			this.mVertices[2].X = 28f;
			this.mVertices[2].Y = 38f;
			this.mVertices[2].Z = 830f * vector.X;
			this.mVertices[2].W = 168f * vector.Y;
			this.mVertices[3].X = 0f;
			this.mVertices[3].Y = 38f;
			this.mVertices[3].Z = 802f * vector.X;
			this.mVertices[3].W = 168f * vector.Y;
			ushort[] array = new ushort[TextBox.INDICES.Length];
			TextBox.INDICES.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				ushort[] array2 = array;
				int num = i;
				array2[num] += 4;
			}
			lock (graphicsDevice)
			{
				this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
				this.mVertexBuffer = new VertexBuffer(graphicsDevice, 320, BufferUsage.WriteOnly);
				if (DropDownBox.sIndices == null || DropDownBox.sIndices.IsDisposed)
				{
					DropDownBox.sIndices = new IndexBuffer(graphicsDevice, array.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
					DropDownBox.sIndices.SetData<ushort>(array);
				}
			}
		}

		// Token: 0x0600273B RID: 10043 RVA: 0x0011DF88 File Offset: 0x0011C188
		protected void SetNames(string[] iNames)
		{
			this.mNames = iNames;
			this.mSelectedIndex = 0;
			this.mTexts = new Text[this.mNames.Length];
			this.mTextScales = new float[this.mNames.Length];
			for (int i = 0; i < this.mTexts.Length; i++)
			{
				this.mTexts[i] = new Text(64, this.mFont, TextAlign.Right, false);
				this.mTextScales[i] = 1f;
			}
		}

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x0600273C RID: 10044 RVA: 0x0011E000 File Offset: 0x0011C200
		public Vector4 ColorItem
		{
			get
			{
				return this.mColorItem;
			}
		}

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x0600273D RID: 10045 RVA: 0x0011E008 File Offset: 0x0011C208
		public Vector2 Size
		{
			get
			{
				return this.mSize;
			}
		}

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x0600273E RID: 10046 RVA: 0x0011E010 File Offset: 0x0011C210
		public BitmapFont Font
		{
			get
			{
				return this.mFont;
			}
		}

		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x0600273F RID: 10047 RVA: 0x0011E018 File Offset: 0x0011C218
		public int Count
		{
			get
			{
				return this.mNames.Length;
			}
		}

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x06002740 RID: 10048 RVA: 0x0011E022 File Offset: 0x0011C222
		// (set) Token: 0x06002741 RID: 10049 RVA: 0x0011E02A File Offset: 0x0011C22A
		public int SelectedIndex
		{
			get
			{
				return this.mSelectedIndex;
			}
			set
			{
				if (value >= 0 & value < this.mNames.Length & value != this.mSelectedIndex)
				{
					this.mSelectedIndex = value;
					this.OnSelectedIndexChanged();
				}
			}
		}

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x06002742 RID: 10050 RVA: 0x0011E05B File Offset: 0x0011C25B
		// (set) Token: 0x06002743 RID: 10051 RVA: 0x0011E063 File Offset: 0x0011C263
		public int NewSelection
		{
			get
			{
				return this.mNewSelection;
			}
			set
			{
				this.mNewSelection = value;
			}
		}

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x06002744 RID: 10052 RVA: 0x0011E06C File Offset: 0x0011C26C
		public string SelectedName
		{
			get
			{
				return this.mNames[this.mSelectedIndex];
			}
		}

		// Token: 0x06002745 RID: 10053 RVA: 0x0011E07B File Offset: 0x0011C27B
		protected virtual void OnSelectedIndexChanged()
		{
			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged.Invoke(this, this.mSelectedIndex);
			}
		}

		// Token: 0x06002746 RID: 10054 RVA: 0x0011E098 File Offset: 0x0011C298
		protected void UpdateVertices()
		{
			Vector2 vector = default(Vector2);
			vector.X = 1f / (float)DropDownBox.sTexture.Width;
			vector.Y = 1f / (float)DropDownBox.sTexture.Height;
			Vector2 vector2 = this.mSize;
			vector2.Y *= (float)this.mNames.Length;
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
			SubMenuCharacterSelect.CreateVertices(this.mVertices, 4, ref vector2, ref vector3, ref vector4, ref vector5, ref vector6);
			lock (this.mVertexBuffer.GraphicsDevice)
			{
				this.mVertexBuffer.SetData<Vector4>(this.mVertices);
			}
		}

		// Token: 0x06002747 RID: 10055 RVA: 0x0011E214 File Offset: 0x0011C414
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X;
			this.mBottomRight.X = this.mPosition.X + this.mSize.X * this.mScale;
			if (this.mIsDown)
			{
				this.mTopLeft.Y = this.mPosition.Y + this.mSize.Y * this.mScale;
				this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * ((float)this.mNames.Length + 1f) * this.mScale;
				return;
			}
			this.mTopLeft.Y = this.mPosition.Y;
			this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * this.mScale;
		}

		// Token: 0x06002748 RID: 10056 RVA: 0x0011E30C File Offset: 0x0011C50C
		internal int GetHitIndex(ref Vector2 iPoint)
		{
			if (!this.mIsDown)
			{
				return -1;
			}
			float x = this.mPosition.X;
			float num = this.mPosition.Y + (this.mSize.Y + 16f) * this.mScale;
			float num2 = this.mPosition.X + this.mSize.X * this.mScale;
			float num3 = num + this.mSize.Y * this.mScale;
			if (iPoint.X < x || iPoint.X > num2)
			{
				return -1;
			}
			for (int i = 0; i < this.mNames.Length; i++)
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

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x06002749 RID: 10057 RVA: 0x0011E3F3 File Offset: 0x0011C5F3
		// (set) Token: 0x0600274A RID: 10058 RVA: 0x0011E3FB File Offset: 0x0011C5FB
		public bool IsDown
		{
			get
			{
				return this.mIsDown;
			}
			set
			{
				this.mNewSelection = -1;
				this.mIsDown = value;
				this.mSelected = false;
				this.UpdateBoundingBox();
			}
		}

		// Token: 0x0600274B RID: 10059 RVA: 0x0011E418 File Offset: 0x0011C618
		public void Update(float iDeltaTime)
		{
			if (this.mIsDown && this.mEnabled)
			{
				this.mDownAlpha = Math.Min(this.mDownAlpha + iDeltaTime * 4f, 1f);
				return;
			}
			this.mDownAlpha = Math.Max(this.mDownAlpha - iDeltaTime * 4f, 0f);
		}

		// Token: 0x0600274C RID: 10060 RVA: 0x0011E472 File Offset: 0x0011C672
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x0600274D RID: 10061 RVA: 0x0011E481 File Offset: 0x0011C681
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			this.Draw(iEffect, iScale, 1f);
		}

		// Token: 0x0600274E RID: 10062 RVA: 0x0011E490 File Offset: 0x0011C690
		public void Draw(GUIBasicEffect iEffect, float iScale, float iAlpha)
		{
			if (this.mTexts.Length == 0)
			{
				return;
			}
			Vector4 color = default(Vector4);
			if (!this.mEnabled)
			{
				iEffect.Saturation = 0f;
			}
			else if (this.mSelected & !this.mIsDown)
			{
				iEffect.Saturation = 1.5f;
			}
			else
			{
				iEffect.Saturation = 1f;
			}
			Matrix transform = new Matrix
			{
				M22 = iScale,
				M11 = iScale,
				M44 = 1f,
				M41 = this.mPosition.X + (this.mSize.X - 28f) * iScale,
				M42 = this.mPosition.Y
			};
			iEffect.Transform = transform;
			iEffect.Texture = DropDownBox.sTexture;
			color.X = (color.Y = (color.Z = 1f));
			color.W = iAlpha;
			iEffect.Color = color;
			iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			iEffect.Saturation = 1f;
			color = (this.mEnabled ? ((this.mSelected & !this.mIsDown) ? this.mColorSelected : this.mColor) : this.mColorDisabled);
			color.W *= iAlpha;
			iEffect.Color = color;
			transform.M11 = this.mTextScales[this.mSelectedIndex] * iScale;
			transform.M22 = iScale;
			transform.M41 = this.mPosition.X + (this.mSize.X - 32f) * iScale;
			transform.M42 = this.mPosition.Y;
			this.mTexts[this.mSelectedIndex].Draw(iEffect, ref transform);
			float num = this.mDownAlpha * iAlpha;
			if (num > 0f)
			{
				color.X = (color.Y = (color.Z = 1f));
				color.W = num;
				iEffect.Color = color;
				Matrix transform2 = new Matrix
				{
					M11 = iScale,
					M22 = iScale,
					M41 = this.mPosition.X,
					M42 = this.mPosition.Y + this.mSize.Y * iScale,
					M44 = 1f
				};
				iEffect.Transform = transform2;
				iEffect.Texture = DropDownBox.sTexture;
				iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
				iEffect.GraphicsDevice.Indices = DropDownBox.sIndices;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 20, 0, 18);
				Vector2 mPosition = this.mPosition;
				mPosition.X += (this.mSize.X - 16f) * iScale;
				mPosition.Y += (this.mSize.Y + 16f) * iScale;
				for (int i = 0; i < this.mTexts.Length; i++)
				{
					color = ((i == this.mNewSelection) ? this.mColorSelected : this.mColorItem);
					color.W *= num * num;
					iEffect.Color = color;
					transform2.M11 = this.mTextScales[i] * iScale;
					transform2.M22 = iScale;
					transform2.M41 = mPosition.X;
					transform2.M42 = mPosition.Y;
					this.mTexts[i].Draw(iEffect, ref transform2);
					mPosition.Y += this.mSize.Y * iScale;
				}
			}
		}

		// Token: 0x0600274F RID: 10063 RVA: 0x0011E898 File Offset: 0x0011CA98
		public override void LanguageChanged()
		{
			Vector2 vector = default(Vector2);
			vector.X = (float)this.mWidth;
			vector.Y = (float)this.mFont.LineHeight;
			for (int i = 0; i < this.mNames.Length; i++)
			{
				this.mTexts[i].SetText(this.mNames[i]);
				this.mTextScales[i] = Math.Min(((float)this.mWidth - 32f) / this.mFont.MeasureText(this.mNames[i], true).X, 1f);
			}
			this.mSize = vector;
			this.UpdateVertices();
			this.UpdateBoundingBox();
		}

		// Token: 0x04002A74 RID: 10868
		private const float OFFSETX = 832f;

		// Token: 0x04002A75 RID: 10869
		private const float OFFSETY = 128f;

		// Token: 0x04002A76 RID: 10870
		private const float SIZE = 128f;

		// Token: 0x04002A77 RID: 10871
		private const float MARGIN = 16f;

		// Token: 0x04002A79 RID: 10873
		protected int mSelectedIndex;

		// Token: 0x04002A7A RID: 10874
		private int mNewSelection;

		// Token: 0x04002A7B RID: 10875
		protected string[] mNames;

		// Token: 0x04002A7C RID: 10876
		private Text[] mTexts;

		// Token: 0x04002A7D RID: 10877
		private float[] mTextScales;

		// Token: 0x04002A7E RID: 10878
		private BitmapFont mFont;

		// Token: 0x04002A7F RID: 10879
		private int mWidth;

		// Token: 0x04002A80 RID: 10880
		private Vector2 mSize;

		// Token: 0x04002A81 RID: 10881
		private bool mIsDown;

		// Token: 0x04002A82 RID: 10882
		private float mDownAlpha;

		// Token: 0x04002A83 RID: 10883
		private Vector4 mColorItem = Defines.DIALOGUE_COLOR_DEFAULT;

		// Token: 0x04002A84 RID: 10884
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04002A85 RID: 10885
		private Vector4[] mVertices;

		// Token: 0x04002A86 RID: 10886
		private VertexBuffer mVertexBuffer;

		// Token: 0x04002A87 RID: 10887
		private static IndexBuffer sIndices;

		// Token: 0x04002A88 RID: 10888
		private static Texture2D sTexture;
	}
}
