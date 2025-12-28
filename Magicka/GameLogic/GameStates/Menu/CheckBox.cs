using System;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x02000513 RID: 1299
	internal class CheckBox : MenuItem
	{
		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06002701 RID: 9985 RVA: 0x0011C8E2 File Offset: 0x0011AAE2
		// (remove) Token: 0x06002702 RID: 9986 RVA: 0x0011C8FB File Offset: 0x0011AAFB
		public event Action<CheckBox> OnCheckedChanged;

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x06002703 RID: 9987 RVA: 0x0011C914 File Offset: 0x0011AB14
		// (set) Token: 0x06002704 RID: 9988 RVA: 0x0011C91C File Offset: 0x0011AB1C
		public int Width
		{
			get
			{
				return this.mWidth;
			}
			set
			{
				this.mWidth = value;
				this.UpdateVertices();
			}
		}

		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06002705 RID: 9989 RVA: 0x0011C92B File Offset: 0x0011AB2B
		// (set) Token: 0x06002706 RID: 9990 RVA: 0x0011C933 File Offset: 0x0011AB33
		public int Height
		{
			get
			{
				return this.mHeight;
			}
			set
			{
				this.mHeight = value;
				this.UpdateVertices();
			}
		}

		// Token: 0x06002707 RID: 9991 RVA: 0x0011C944 File Offset: 0x0011AB44
		public CheckBox(bool iChecked)
		{
			this.mBackColor = Vector4.One;
			this.mBackColorHover = new Vector4(1.5f, 1.5f, 1.5f, 1f);
			this.mTickColor = new Vector4(0.1f, 0.9f, 0.1f, 1f);
			this.mTickColorHover = new Vector4(0.5f, 1.5f, 0.5f, 1f);
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			this.mTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
			this.mVertexBuffer = new VertexBuffer(graphicsDevice, 128, BufferUsage.WriteOnly);
			this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
			});
			this.Width = 32;
			this.Height = 32;
			this.UpdateVertices();
			this.Checked = iChecked;
			if (this.OnCheckedChanged != null)
			{
				this.OnCheckedChanged(this);
			}
		}

		// Token: 0x06002708 RID: 9992 RVA: 0x0011CA70 File Offset: 0x0011AC70
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y * this.mScale;
			this.mBottomRight.X = this.mPosition.X + (float)this.mWidth * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + (float)this.mHeight * this.mScale;
		}

		// Token: 0x06002709 RID: 9993 RVA: 0x0011CB01 File Offset: 0x0011AD01
		public override void LanguageChanged()
		{
		}

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x0600270A RID: 9994 RVA: 0x0011CB03 File Offset: 0x0011AD03
		// (set) Token: 0x0600270B RID: 9995 RVA: 0x0011CB0B File Offset: 0x0011AD0B
		public CheckBox.CheckBoxStyle Style
		{
			get
			{
				return this.mStyle;
			}
			set
			{
				this.mStyle = value;
				this.UpdateVertices();
			}
		}

		// Token: 0x0600270C RID: 9996 RVA: 0x0011CB1C File Offset: 0x0011AD1C
		private void UpdateVertices()
		{
			Vector4[] array = new Vector4[8];
			float value = (float)Math.Min(this.Width, this.Height);
			if (this.mStyle == CheckBox.CheckBoxStyle.RadioButton)
			{
				QuadHelper.CreateQuadFan(array, 0, default(Vector2), new Vector2(value), new Vector2(992f / (float)this.mTexture.Width, 160f / (float)this.mTexture.Height), new Vector2(32f / (float)this.mTexture.Width, 32f / (float)this.mTexture.Height));
				QuadHelper.CreateQuadFan(array, 4, default(Vector2), new Vector2(value), new Vector2(960f / (float)this.mTexture.Width, 160f / (float)this.mTexture.Height), new Vector2(32f / (float)this.mTexture.Width, 32f / (float)this.mTexture.Height));
			}
			else
			{
				QuadHelper.CreateQuadFan(array, 0, default(Vector2), new Vector2(value), new Vector2(992f / (float)this.mTexture.Width, 128f / (float)this.mTexture.Height), new Vector2(32f / (float)this.mTexture.Width, 32f / (float)this.mTexture.Height));
				QuadHelper.CreateQuadFan(array, 4, default(Vector2), new Vector2(value), new Vector2(960f / (float)this.mTexture.Width, 128f / (float)this.mTexture.Height), new Vector2(32f / (float)this.mTexture.Width, 32f / (float)this.mTexture.Height));
			}
			this.mVertexBuffer.SetData<Vector4>(array);
		}

		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x0600270D RID: 9997 RVA: 0x0011CD06 File Offset: 0x0011AF06
		// (set) Token: 0x0600270E RID: 9998 RVA: 0x0011CD0E File Offset: 0x0011AF0E
		public bool Checked
		{
			get
			{
				return this.mChecked;
			}
			set
			{
				this.mChecked = value;
				if (this.OnCheckedChanged != null)
				{
					this.OnCheckedChanged(this);
				}
			}
		}

		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x0600270F RID: 9999 RVA: 0x0011CD2B File Offset: 0x0011AF2B
		// (set) Token: 0x06002710 RID: 10000 RVA: 0x0011CD33 File Offset: 0x0011AF33
		public Vector4 BackgroundColor
		{
			get
			{
				return this.mBackColor;
			}
			set
			{
				this.mBackColor = value;
			}
		}

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x06002711 RID: 10001 RVA: 0x0011CD3C File Offset: 0x0011AF3C
		// (set) Token: 0x06002712 RID: 10002 RVA: 0x0011CD44 File Offset: 0x0011AF44
		public Vector4 BackgroundColorHover
		{
			get
			{
				return this.mBackColorHover;
			}
			set
			{
				this.mBackColorHover = value;
			}
		}

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x06002713 RID: 10003 RVA: 0x0011CD4D File Offset: 0x0011AF4D
		// (set) Token: 0x06002714 RID: 10004 RVA: 0x0011CD55 File Offset: 0x0011AF55
		public Vector4 TickColor
		{
			get
			{
				return this.mBackColor;
			}
			set
			{
				this.mBackColor = value;
			}
		}

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x06002715 RID: 10005 RVA: 0x0011CD5E File Offset: 0x0011AF5E
		// (set) Token: 0x06002716 RID: 10006 RVA: 0x0011CD66 File Offset: 0x0011AF66
		public Vector4 TickColorHover
		{
			get
			{
				return this.mTickColorHover;
			}
			set
			{
				this.mTickColorHover = value;
			}
		}

		// Token: 0x06002717 RID: 10007 RVA: 0x0011CD6F File Offset: 0x0011AF6F
		public void DoMouseClick()
		{
			this.Toggle();
		}

		// Token: 0x06002718 RID: 10008 RVA: 0x0011CD77 File Offset: 0x0011AF77
		public void Toggle()
		{
			this.Checked = !this.Checked;
		}

		// Token: 0x06002719 RID: 10009 RVA: 0x0011CD88 File Offset: 0x0011AF88
		public void SetMouseHover(bool iHover)
		{
			this.mHover = iHover;
		}

		// Token: 0x0600271A RID: 10010 RVA: 0x0011CD94 File Offset: 0x0011AF94
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			iEffect.Texture = this.mTexture;
			iEffect.TextureEnabled = true;
			iEffect.Transform = Matrix.Identity;
			Matrix identity = Matrix.Identity;
			identity.M41 = base.Position.X;
			identity.M42 = base.Position.Y;
			iEffect.Transform = identity;
			iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
			iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			Vector4 color = this.mHover ? this.mBackColorHover : this.mBackColor;
			if (!this.mEnabled)
			{
				color.W *= 0.5f;
			}
			iEffect.Color = color;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			if (this.mChecked)
			{
				if (this.mStyle != CheckBox.CheckBoxStyle.RadioButton)
				{
					color = (this.mHover ? this.mTickColorHover : this.mTickColor);
				}
				if (!this.mEnabled)
				{
					color.W *= 0.5f;
				}
				iEffect.Color = color;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
			}
		}

		// Token: 0x0600271B RID: 10011 RVA: 0x0011CECB File Offset: 0x0011B0CB
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x04002A3F RID: 10815
		private bool mChecked;

		// Token: 0x04002A40 RID: 10816
		private Vector4 mBackColor;

		// Token: 0x04002A41 RID: 10817
		private Vector4 mBackColorHover;

		// Token: 0x04002A42 RID: 10818
		private Vector4 mTickColor;

		// Token: 0x04002A43 RID: 10819
		private Vector4 mTickColorHover;

		// Token: 0x04002A45 RID: 10821
		private CheckBox.CheckBoxStyle mStyle;

		// Token: 0x04002A46 RID: 10822
		private bool mHover;

		// Token: 0x04002A47 RID: 10823
		private Texture2D mTexture;

		// Token: 0x04002A48 RID: 10824
		private VertexBuffer mVertexBuffer;

		// Token: 0x04002A49 RID: 10825
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04002A4A RID: 10826
		private int mWidth;

		// Token: 0x04002A4B RID: 10827
		private int mHeight;

		// Token: 0x02000514 RID: 1300
		public enum CheckBoxStyle
		{
			// Token: 0x04002A4D RID: 10829
			Checkbox,
			// Token: 0x04002A4E RID: 10830
			RadioButton
		}
	}
}
