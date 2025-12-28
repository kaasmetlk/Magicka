using System;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.UI;
using Magicka.Graphics.Effects;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Graphics
{
	// Token: 0x02000245 RID: 581
	public class TextBox
	{
		// Token: 0x06001200 RID: 4608 RVA: 0x0006CE24 File Offset: 0x0006B024
		public TextBox()
		{
			if (TextBox.sVertexBuffer == null)
			{
				TextBox.sIndexBuffer = new IndexBuffer(Game.Instance.GraphicsDevice, 108, BufferUsage.None, IndexElementSize.SixteenBits);
				TextBox.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, 16 * VertexPositionNormalTexture.SizeInBytes, BufferUsage.None);
				TextBox.sVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
				lock (Game.Instance.GraphicsDevice)
				{
					TextBox.sIndexBuffer.SetData<ushort>(TextBox.INDICES);
					TextBox.sVertexBuffer.SetData<VertexPositionNormalTexture>(TextBox.VERTICES);
				}
			}
			this.mText = new TypingText(1024, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Left, false, 40f);
			this.mText.DefaultColor = Defines.DIALOGUE_COLOR_DEFAULT;
			this.mOwnerName = new Text(64, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, false);
			this.mOwnerName.DefaultColor = Defines.DIALOGUE_COLOR_DEFAULT;
			this.mRenderData = new TextBox.RenderData[3];
			Point screenSize = RenderManager.Instance.ScreenSize;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mTextBoxEffect = new TextBoxEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
			}
			this.mTextBoxEffect.BorderSize = 32f;
			this.mTextBoxEffect.ScreenSize = new Vector2((float)screenSize.X, (float)screenSize.Y);
			this.mGUIBasicEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mGUIBasicEffect.VertexColorEnabled = true;
			this.mGUIBasicEffect.TextureEnabled = true;
			this.mGUIBasicEffect.Color = Vector4.One;
			for (int i = 0; i < 3; i++)
			{
				TextBox.RenderData renderData = new TextBox.RenderData();
				renderData.mTextBoxEffect = this.mTextBoxEffect;
				renderData.mGUIBasicEffect = this.mGUIBasicEffect;
				renderData.VertexBuffer = TextBox.sVertexBuffer;
				renderData.VertexDeclaration = TextBox.sVertexDeclaration;
				renderData.IndexBuffer = TextBox.sIndexBuffer;
				this.mRenderData[i] = renderData;
			}
			this.mBox = default(Rectangle);
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x0006D078 File Offset: 0x0006B278
		public static void GetRenderBuffers(out VertexBuffer oVertexBuffer, out VertexDeclaration oVertexDeclaration, out IndexBuffer oIndexBuffer, out int oVertexStride)
		{
			oVertexBuffer = TextBox.sVertexBuffer;
			oVertexDeclaration = TextBox.sVertexDeclaration;
			oIndexBuffer = TextBox.sIndexBuffer;
			oVertexStride = VertexPositionNormalTexture.SizeInBytes;
		}

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06001202 RID: 4610 RVA: 0x0006D096 File Offset: 0x0006B296
		public Entity Owner
		{
			get
			{
				return this.mOwner;
			}
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0006D0A0 File Offset: 0x0006B2A0
		public virtual void Initialize(Scene iScene, MagickaFont iFont, string iText, Vector2 iMinSize, bool iForceOnScreen, Entity iOwner, float iTTL)
		{
			if (iScene == null)
			{
				throw new ArgumentNullException("iScene");
			}
			this.mOwner = iOwner;
			this.mIsScreenPos = false;
			if (iOwner != null)
			{
				this.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, iOwner.UniqueID, iTTL);
				return;
			}
			this.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, 0, iTTL);
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x0006D0F5 File Offset: 0x0006B2F5
		public virtual void Initialize(Scene iScene, MagickaFont iFont, string iText, Vector2 iMinSize, Vector3 iWorldPosition, bool iForceOnScreen, int iName, float iTTL)
		{
			if (iScene == null)
			{
				throw new ArgumentNullException("iScene");
			}
			this.mOwner = null;
			this.mWorldPos = iWorldPosition;
			this.mIsScreenPos = false;
			this.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, iName, iTTL);
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x0006D12C File Offset: 0x0006B32C
		public virtual void Initialize(Scene iScene, MagickaFont iFont, string iText, Vector2 iMinSize, Vector2 iScreenPosition, bool iForceOnScreen, int iName, float iTTL)
		{
			if (iScene == null)
			{
				throw new ArgumentNullException("iScene");
			}
			this.mOwner = null;
			this.mWorldPos.X = iScreenPosition.X;
			this.mWorldPos.Y = iScreenPosition.Y;
			this.mWorldPos.Z = 0f;
			this.mIsScreenPos = true;
			this.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, iName, iTTL);
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x0006D19C File Offset: 0x0006B39C
		protected virtual void Initialize(Scene iScene, MagickaFont iFont, string iText, Vector2 iMinSize, bool iForceOnScreen, int iName, float iTTL)
		{
			if (!this.mGrow)
			{
				this.mScale = 0f;
			}
			this.mGrow = true;
			this.mForceScreen = iForceOnScreen;
			this.mTTL = iTTL;
			this.mAutomaticAdvance = (iTTL > 0f);
			this.mString = iText;
			if (iText != null)
			{
				this.mString = LanguageManager.Instance.ParseReferences(this.mString);
				this.mText.SetText(this.mString);
			}
			this.mScene = iScene;
			this.mTextBoxEffect.Texture = Game.Instance.Content.Load<Texture2D>("UI/HUD/Dialog_Say");
			string text;
			this.mShowName = LanguageManager.Instance.TryGetString(iName, out text);
			if (this.mShowName)
			{
				this.mOwnerName.SetText(text);
				Vector2 vector = this.mOwnerName.Font.MeasureText(this.mOwnerName.Characters, true);
				iMinSize.X = Math.Max(iMinSize.X + 20f, vector.X);
				iMinSize.Y += vector.Y;
			}
			if (string.IsNullOrEmpty(this.mString))
			{
				return;
			}
			Vector2 vector2 = Vector2.Max(this.mText.Font.MeasureText(this.mText.Characters, true), iMinSize);
			this.mBox.Width = (int)vector2.X;
			this.mBox.Height = (int)vector2.Y;
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x0006D30B File Offset: 0x0006B50B
		public virtual void Hide()
		{
			this.mGrow = false;
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x0006D314 File Offset: 0x0006B514
		public virtual void Update(float iDeltaTime, DataChannel iDataChannel, ref Matrix iViewProjection)
		{
			if (this.mScene == null || (this.mScale <= 1E-45f && !this.mGrow))
			{
				return;
			}
			TextBox.RenderData renderData = this.mRenderData[(int)iDataChannel];
			if (this.mGrow)
			{
				this.mScale = Math.Min(this.mScale + iDeltaTime * 4f, 1f);
			}
			else
			{
				this.mScale = Math.Max(this.mScale - iDeltaTime * 8f, 0f);
			}
			if (this.mOwner != null)
			{
				renderData.mWorldPosition = this.mOwner.Position;
				if (this.mOwner is Magicka.GameLogic.Entities.Character)
				{
					Magicka.GameLogic.Entities.Character character = this.mOwner as Magicka.GameLogic.Entities.Character;
					TextBox.RenderData renderData2 = renderData;
					renderData2.mWorldPosition.Y = renderData2.mWorldPosition.Y + (character.Capsule.Length * 0.5f + character.Capsule.Radius);
				}
			}
			else
			{
				renderData.mWorldPosition = this.mWorldPos;
			}
			if (this.mText.IsFinished)
			{
				this.mTTL -= iDeltaTime;
			}
			if (this.mAutomaticAdvance && this.mTTL < 0f)
			{
				DialogManager.Instance.Advance(this);
			}
			renderData.mIsScreenPos = this.mIsScreenPos;
			renderData.mForceOnScreen = this.mForceScreen;
			renderData.mScale = this.mScale;
			renderData.mText = this.mText;
			renderData.mOwnerName = this.mOwnerName;
			renderData.mShowName = this.mShowName;
			renderData.mSize.X = (float)this.mBox.Width;
			renderData.mSize.Y = (float)this.mBox.Height;
			this.mScene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x0006D4BC File Offset: 0x0006B6BC
		public void FinishAnimation(bool iRemoveTTLAdvance)
		{
			if (iRemoveTTLAdvance)
			{
				this.mAutomaticAdvance = false;
			}
			this.mScale = (this.mGrow ? 1f : 0f);
			lock (Game.Instance.GraphicsDevice)
			{
				this.mText.Finish();
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x0600120A RID: 4618 RVA: 0x0006D524 File Offset: 0x0006B724
		public string Text
		{
			get
			{
				return this.mString;
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x0600120B RID: 4619 RVA: 0x0006D52C File Offset: 0x0006B72C
		public Point Position
		{
			get
			{
				return this.mBox.Location;
			}
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x0600120C RID: 4620 RVA: 0x0006D539 File Offset: 0x0006B739
		public bool Animating
		{
			get
			{
				return (this.mGrow && this.mScale < 0.99f) || (!this.mGrow && this.mScale > float.Epsilon) || !this.mText.IsFinished;
			}
		}

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x0600120D RID: 4621 RVA: 0x0006D575 File Offset: 0x0006B775
		public bool Visible
		{
			get
			{
				return this.mGrow || this.mScale > float.Epsilon;
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x0600120E RID: 4622 RVA: 0x0006D58E File Offset: 0x0006B78E
		public int Width
		{
			get
			{
				return this.mBox.Width;
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x0600120F RID: 4623 RVA: 0x0006D59B File Offset: 0x0006B79B
		public int Height
		{
			get
			{
				return this.mBox.Height;
			}
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06001210 RID: 4624 RVA: 0x0006D5A8 File Offset: 0x0006B7A8
		public bool AutomaticAdvance
		{
			get
			{
				return this.mAutomaticAdvance;
			}
		}

		// Token: 0x040010B6 RID: 4278
		public const float BORDERSIZE = 32f;

		// Token: 0x040010B7 RID: 4279
		public const int INDENTATION = 20;

		// Token: 0x040010B8 RID: 4280
		public static readonly ushort[] INDICES = new ushort[]
		{
			0,
			1,
			4,
			1,
			5,
			4,
			1,
			2,
			5,
			2,
			6,
			5,
			2,
			3,
			6,
			3,
			7,
			6,
			4,
			5,
			8,
			5,
			9,
			8,
			5,
			6,
			9,
			6,
			10,
			9,
			6,
			7,
			10,
			7,
			11,
			10,
			8,
			9,
			12,
			9,
			13,
			12,
			9,
			10,
			13,
			10,
			14,
			13,
			10,
			11,
			14,
			11,
			15,
			14
		};

		// Token: 0x040010B9 RID: 4281
		public static readonly VertexPositionNormalTexture[] VERTICES = new VertexPositionNormalTexture[]
		{
			new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0f), new Vector3(-1f, -1f, 0f), new Vector2(0f, 0f)),
			new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0f), new Vector3(0f, -1f, 0f), new Vector2(0.25f, 0f)),
			new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0f), new Vector3(0f, -1f, 0f), new Vector2(0.75f, 0f)),
			new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0f), new Vector3(1f, -1f, 0f), new Vector2(1f, 0f)),
			new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0f), new Vector3(-1f, 0f, 0f), new Vector2(0f, 0.25f)),
			new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0f), new Vector3(0f, 0f, 0f), new Vector2(0.25f, 0.25f)),
			new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0f), new Vector3(0f, 0f, 0f), new Vector2(0.75f, 0.25f)),
			new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0f), new Vector3(1f, 0f, 0f), new Vector2(1f, 0.25f)),
			new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0f), new Vector3(-1f, 0f, 0f), new Vector2(0f, 0.75f)),
			new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0f), new Vector3(0f, 0f, 0f), new Vector2(0.25f, 0.75f)),
			new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0f), new Vector3(0f, 0f, 0f), new Vector2(0.75f, 0.75f)),
			new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 0f, 0f), new Vector2(1f, 0.75f)),
			new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0f), new Vector3(-1f, 1f, 0f), new Vector2(0f, 1f)),
			new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0f), new Vector3(0f, 1f, 0f), new Vector2(0.25f, 1f)),
			new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0f), new Vector3(0f, 1f, 0f), new Vector2(0.75f, 1f)),
			new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 0f), new Vector2(1f, 1f))
		};

		// Token: 0x040010BA RID: 4282
		protected string mString;

		// Token: 0x040010BB RID: 4283
		protected Rectangle mBox;

		// Token: 0x040010BC RID: 4284
		protected Text mOwnerName;

		// Token: 0x040010BD RID: 4285
		protected bool mShowName;

		// Token: 0x040010BE RID: 4286
		protected TypingText mText;

		// Token: 0x040010BF RID: 4287
		protected TextBoxEffect mTextBoxEffect;

		// Token: 0x040010C0 RID: 4288
		protected GUIBasicEffect mGUIBasicEffect;

		// Token: 0x040010C1 RID: 4289
		protected static IndexBuffer sIndexBuffer;

		// Token: 0x040010C2 RID: 4290
		protected static VertexBuffer sVertexBuffer;

		// Token: 0x040010C3 RID: 4291
		protected static VertexDeclaration sVertexDeclaration;

		// Token: 0x040010C4 RID: 4292
		protected TextBox.RenderData[] mRenderData;

		// Token: 0x040010C5 RID: 4293
		protected Scene mScene;

		// Token: 0x040010C6 RID: 4294
		protected Entity mOwner;

		// Token: 0x040010C7 RID: 4295
		protected Vector3 mWorldPos;

		// Token: 0x040010C8 RID: 4296
		protected bool mIsScreenPos;

		// Token: 0x040010C9 RID: 4297
		protected bool mForceScreen;

		// Token: 0x040010CA RID: 4298
		private float mScale;

		// Token: 0x040010CB RID: 4299
		private bool mGrow;

		// Token: 0x040010CC RID: 4300
		private float mTTL;

		// Token: 0x040010CD RID: 4301
		private bool mAutomaticAdvance;

		// Token: 0x02000246 RID: 582
		protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
		{
			// Token: 0x170004AE RID: 1198
			// (get) Token: 0x06001212 RID: 4626 RVA: 0x0006DAD5 File Offset: 0x0006BCD5
			// (set) Token: 0x06001213 RID: 4627 RVA: 0x0006DADD File Offset: 0x0006BCDD
			public IndexBuffer IndexBuffer
			{
				get
				{
					return this.mIndexBuffer;
				}
				set
				{
					this.mIndexBuffer = value;
				}
			}

			// Token: 0x170004AF RID: 1199
			// (get) Token: 0x06001214 RID: 4628 RVA: 0x0006DAE6 File Offset: 0x0006BCE6
			// (set) Token: 0x06001215 RID: 4629 RVA: 0x0006DAEE File Offset: 0x0006BCEE
			public VertexBuffer VertexBuffer
			{
				get
				{
					return this.mVertexBuffer;
				}
				set
				{
					this.mVertexBuffer = value;
				}
			}

			// Token: 0x170004B0 RID: 1200
			// (get) Token: 0x06001216 RID: 4630 RVA: 0x0006DAF7 File Offset: 0x0006BCF7
			// (set) Token: 0x06001217 RID: 4631 RVA: 0x0006DAFF File Offset: 0x0006BCFF
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
				set
				{
					this.mVertexDeclaration = value;
				}
			}

			// Token: 0x06001218 RID: 4632 RVA: 0x0006DB08 File Offset: 0x0006BD08
			public virtual void Draw(float iDeltaTime)
			{
				this.mTextBoxEffect.Color = Vector4.One;
				this.mTextBoxEffect.Position = this.mPosition;
				this.mTextBoxEffect.Size = this.mSize;
				this.mTextBoxEffect.Scale = this.mScale;
				this.mTextBoxEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
				this.mTextBoxEffect.GraphicsDevice.Indices = this.mIndexBuffer;
				this.mTextBoxEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mTextBoxEffect.Begin();
				this.mTextBoxEffect.CurrentTechnique.Passes[0].Begin();
				this.mTextBoxEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 18);
				this.mTextBoxEffect.CurrentTechnique.Passes[0].End();
				this.mTextBoxEffect.End();
				this.mGUIBasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
				this.mGUIBasicEffect.Begin();
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
				if (this.mShowName)
				{
					this.mOwnerName.Draw(this.mGUIBasicEffect, this.mPosition.X - this.mSize.X * 0.5f * this.mScale, this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale, this.mScale);
					this.mText.Draw(this.mGUIBasicEffect, this.mPosition.X - (this.mSize.X * 0.5f - 20f) * this.mScale, this.mPosition.Y - (this.mSize.Y * 0.5f - (float)this.mOwnerName.Font.LineHeight) * this.mScale, this.mScale);
				}
				else
				{
					this.mText.Draw(this.mGUIBasicEffect, this.mPosition.X - this.mSize.X * 0.5f * this.mScale, this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale, this.mScale);
				}
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
				this.mGUIBasicEffect.End();
			}

			// Token: 0x170004B1 RID: 1201
			// (get) Token: 0x06001219 RID: 4633 RVA: 0x0006DDC6 File Offset: 0x0006BFC6
			public int ZIndex
			{
				get
				{
					return 201;
				}
			}

			// Token: 0x0600121A RID: 4634 RVA: 0x0006DDD0 File Offset: 0x0006BFD0
			public virtual void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				if (this.mScale > 0.99f)
				{
					this.mText.Update(iDeltaTime);
				}
				Point screenSize = RenderManager.Instance.ScreenSize;
				Vector2 screenSize2 = default(Vector2);
				screenSize2.X = (float)screenSize.X;
				screenSize2.Y = (float)screenSize.Y;
				this.mTextBoxEffect.ScreenSize = screenSize2;
				if (this.mIsScreenPos)
				{
					this.mPosition.X = (float)Math.Floor((double)(screenSize2.X * 0.5f * (1f + this.mWorldPosition.X)));
					this.mPosition.Y = (float)Math.Floor((double)(screenSize2.Y * 0.5f * (1f + this.mWorldPosition.Y)));
				}
				else
				{
					Vector4 vector;
					Vector4.Transform(ref this.mWorldPosition, ref iViewProjectionMatrix, out vector);
					this.mPosition.X = (vector.X / vector.W * 0.5f + 0.5f) * screenSize2.X;
					this.mPosition.X = this.mPosition.X - (this.mSize.X * 0.5f + 64f) / 3f * this.mScale;
					this.mPosition.Y = (vector.Y / vector.W * -0.5f + 0.5f) * screenSize2.Y;
					this.mPosition.Y = this.mPosition.Y - (this.mSize.Y * 0.5f + 64f) * this.mScale;
				}
				if (this.mForceOnScreen)
				{
					Vector2 vector2 = default(Vector2);
					vector2.X = (float)Math.Floor((double)(screenSize2.X * 0.05f));
					vector2.Y = (float)Math.Floor((double)(screenSize2.Y * 0.05f));
					this.mPosition.X = Math.Max(this.mPosition.X, (this.mSize.X + 32f) * 0.5f + vector2.X);
					this.mPosition.Y = Math.Max(this.mPosition.Y, (this.mSize.Y + 32f) * 0.5f + vector2.Y);
					this.mPosition.X = Math.Min(this.mPosition.X, screenSize2.X - (this.mSize.X + 32f) * 0.5f - vector2.X);
					this.mPosition.Y = Math.Min(this.mPosition.Y, screenSize2.Y - (this.mSize.Y + 32f) * 0.5f - vector2.Y);
				}
			}

			// Token: 0x040010CE RID: 4302
			public TextBoxEffect mTextBoxEffect;

			// Token: 0x040010CF RID: 4303
			public GUIBasicEffect mGUIBasicEffect;

			// Token: 0x040010D0 RID: 4304
			protected float mAlpha;

			// Token: 0x040010D1 RID: 4305
			protected VertexBuffer mVertexBuffer;

			// Token: 0x040010D2 RID: 4306
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x040010D3 RID: 4307
			protected IndexBuffer mIndexBuffer;

			// Token: 0x040010D4 RID: 4308
			protected int mZIndex;

			// Token: 0x040010D5 RID: 4309
			public Vector3 mWorldPosition;

			// Token: 0x040010D6 RID: 4310
			public bool mIsScreenPos;

			// Token: 0x040010D7 RID: 4311
			public bool mForceOnScreen;

			// Token: 0x040010D8 RID: 4312
			protected Vector2 mPosition;

			// Token: 0x040010D9 RID: 4313
			public TypingText mText;

			// Token: 0x040010DA RID: 4314
			public bool mShowName;

			// Token: 0x040010DB RID: 4315
			public Text mOwnerName;

			// Token: 0x040010DC RID: 4316
			public Vector2 mSize;

			// Token: 0x040010DD RID: 4317
			public float mScale;
		}
	}
}
