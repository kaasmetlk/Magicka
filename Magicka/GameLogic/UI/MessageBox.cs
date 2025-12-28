using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000027 RID: 39
	internal abstract class MessageBox : IRenderableGUIObject, IDisposable
	{
		// Token: 0x0600013C RID: 316 RVA: 0x000083A0 File Offset: 0x000065A0
		public MessageBox(string iMessage)
		{
			this.mDead = true;
			Point screenSize = RenderManager.Instance.ScreenSize;
			this.mCenter.X = (float)screenSize.X * 0.5f;
			this.mCenter.Y = (float)screenSize.Y * 0.5f;
			if (MessageBox.sVertexBuffer == null)
			{
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				lock (graphicsDevice)
				{
					MessageBox.sTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
				}
				Vector2 vector = default(Vector2);
				vector.X = 1f / (float)MessageBox.sTexture.Width;
				vector.Y = 1f / (float)MessageBox.sTexture.Height;
				Vector4[] array = new Vector4[4];
				array[0].X = -224f;
				array[0].Y = -280f;
				array[0].Z = 0f * vector.X;
				array[0].W = 464f * vector.Y;
				array[1].X = 224f;
				array[1].Y = -280f;
				array[1].Z = 448f * vector.X;
				array[1].W = 464f * vector.Y;
				array[2].X = 224f;
				array[2].Y = 280f;
				array[2].Z = 448f * vector.X;
				array[2].W = 1024f * vector.Y;
				array[3].X = -224f;
				array[3].Y = 280f;
				array[3].Z = 0f * vector.X;
				array[3].W = 1024f * vector.Y;
				lock (graphicsDevice)
				{
					MessageBox.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * 4 * 4, BufferUsage.None);
					MessageBox.sVertexBuffer.SetData<Vector4>(array);
					MessageBox.sVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, new VertexElement[]
					{
						new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
						new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
					});
				}
				MessageBox.sGUIBasicEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
				MessageBox.sGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
				MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
				MessageBox.sGUIBasicEffect.TextureEnabled = true;
				MessageBox.sGUIBasicEffect.Color = Vector4.One;
			}
			this.mSize = new Vector2(448f, 560f);
			this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
			string text = this.mFont.Wrap(iMessage, (int)(this.mSize.X * 0.9f), true);
			this.mMessage = new Text(512, this.mFont, TextAlign.Center, false);
			this.mMessage.SetText(text);
			this.mMessage.DefaultColor = MenuItem.COLOR;
			this.mMessageHeight = this.mFont.MeasureText(text, true).Y;
			LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000876C File Offset: 0x0000696C
		public MessageBox(int iMessage) : this(LanguageManager.Instance.GetString(iMessage))
		{
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000877F File Offset: 0x0000697F
		public virtual void LanguageChanged()
		{
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00008784 File Offset: 0x00006984
		public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			this.mCenter.X = (float)screenSize.X * 0.5f;
			this.mCenter.Y = (float)screenSize.Y * 0.5f;
			GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, this);
		}

		// Token: 0x06000140 RID: 320
		public abstract void OnTextInput(char iChar);

		// Token: 0x06000141 RID: 321
		public abstract void OnMove(Controller iSender, ControllerDirection iDirection);

		// Token: 0x06000142 RID: 322
		public abstract void OnMouseMove(MouseState iNewState, MouseState iOldState);

		// Token: 0x06000143 RID: 323
		public abstract void OnMouseClick(MouseState iNewState, MouseState iOldState);

		// Token: 0x06000144 RID: 324
		public abstract void OnSelect(Controller iSender);

		// Token: 0x06000145 RID: 325 RVA: 0x000087EC File Offset: 0x000069EC
		public virtual void ControllerEsc(Controller iSender)
		{
			this.Kill();
		}

		// Token: 0x06000146 RID: 326 RVA: 0x000087F4 File Offset: 0x000069F4
		public virtual void Show()
		{
			this.mAlpha = 0f;
			this.mDead = false;
			DialogManager.Instance.AddMessageBox(this);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00008813 File Offset: 0x00006A13
		public virtual void Kill()
		{
			this.mDead = true;
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000148 RID: 328 RVA: 0x0000881C File Offset: 0x00006A1C
		public bool Dead
		{
			get
			{
				return this.mDead & this.mAlpha < float.Epsilon;
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00008834 File Offset: 0x00006A34
		public virtual void Draw(float iDeltaTime)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			this.mCenter.X = (float)screenSize.X * 0.5f;
			this.mCenter.Y = (float)screenSize.Y * 0.5f;
			MessageBox.sGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
			if (this.mDead)
			{
				this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 4f, 0f);
			}
			else
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 4f, 1f);
			}
			Matrix transform = default(Matrix);
			transform.M11 = (transform.M22 = (transform.M33 = 1f));
			transform.M44 = 1f;
			transform.M41 = this.mCenter.X;
			transform.M42 = this.mCenter.Y;
			MessageBox.sGUIBasicEffect.Transform = transform;
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = 1f));
			color.W = this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
			MessageBox.sGUIBasicEffect.Texture = MessageBox.sTexture;
			MessageBox.sGUIBasicEffect.TextureEnabled = true;
			MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(MessageBox.sVertexBuffer, 0, 16);
			MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = MessageBox.sVertexDeclaration;
			MessageBox.sGUIBasicEffect.Begin();
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
			MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600014A RID: 330 RVA: 0x00008A1E File Offset: 0x00006C1E
		public virtual int ZIndex
		{
			get
			{
				return 2000;
			}
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00008A25 File Offset: 0x00006C25
		public virtual void Dispose()
		{
			DaisyWheel.SetActionToCallWhenComplete(null);
			LanguageManager.Instance.LanguageChanged -= new Action(this.LanguageChanged);
			this.mMessage.Dispose();
		}

		// Token: 0x040000D9 RID: 217
		protected const float BORDERSIZE = 32f;

		// Token: 0x040000DA RID: 218
		protected const float INDENTATION = 20f;

		// Token: 0x040000DB RID: 219
		protected const float VERTICAL_PADDING = 16f;

		// Token: 0x040000DC RID: 220
		protected static VertexBuffer sVertexBuffer;

		// Token: 0x040000DD RID: 221
		protected static VertexDeclaration sVertexDeclaration;

		// Token: 0x040000DE RID: 222
		protected static Texture2D sTexture;

		// Token: 0x040000DF RID: 223
		protected static GUIBasicEffect sGUIBasicEffect;

		// Token: 0x040000E0 RID: 224
		protected Text mMessage;

		// Token: 0x040000E1 RID: 225
		protected Vector2 mCenter;

		// Token: 0x040000E2 RID: 226
		protected BitmapFont mFont;

		// Token: 0x040000E3 RID: 227
		protected float mMessageHeight;

		// Token: 0x040000E4 RID: 228
		protected bool mDead;

		// Token: 0x040000E5 RID: 229
		protected float mAlpha;

		// Token: 0x040000E6 RID: 230
		protected Vector2 mSize;
	}
}
