using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Graphics
{
	// Token: 0x020005D3 RID: 1491
	internal sealed class GamePadMenuHelp
	{
		// Token: 0x17000A95 RID: 2709
		// (get) Token: 0x06002CB0 RID: 11440 RVA: 0x0015E290 File Offset: 0x0015C490
		public static GamePadMenuHelp Instance
		{
			get
			{
				if (GamePadMenuHelp.mSingelton == null)
				{
					lock (GamePadMenuHelp.mSingeltonLock)
					{
						if (GamePadMenuHelp.mSingelton == null)
						{
							GamePadMenuHelp.mSingelton = new GamePadMenuHelp();
						}
					}
				}
				return GamePadMenuHelp.mSingelton;
			}
		}

		// Token: 0x06002CB1 RID: 11441 RVA: 0x0015E2E4 File Offset: 0x0015C4E4
		private GamePadMenuHelp()
		{
			for (int i = 0; i < this.mActionButtons.Length; i++)
			{
				this.mActionButtons[i].String = " - ";
				this.mActionButtons[i].OldString = " - ";
			}
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			GUIBasicEffect iEffect;
			Texture2D texture2D;
			lock (graphicsDevice)
			{
				iEffect = new GUIBasicEffect(graphicsDevice, null);
				texture2D = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
			}
			Vector4[] array = new Vector4[4];
			Vector2 vector = default(Vector2);
			vector.X = 1536f / (float)texture2D.Width;
			vector.Y = 0f / (float)texture2D.Height;
			Vector2 vector2 = default(Vector2);
			vector2.X = 80f / (float)texture2D.Width;
			vector2.Y = 96f / (float)texture2D.Height;
			array[0].X = -80f;
			array[0].Y = 0f;
			array[0].Z = vector.X;
			array[0].W = vector.Y;
			array[1].X = 0f;
			array[1].Y = 0f;
			array[1].Z = vector.X + vector2.X;
			array[1].W = vector.Y;
			array[2].X = 0f;
			array[2].Y = 96f;
			array[2].Z = vector.X + vector2.X;
			array[2].W = vector.Y + vector2.Y;
			array[3].X = -80f;
			array[3].Y = 96f;
			array[3].Z = vector.X;
			array[3].W = vector.Y + vector2.Y;
			VertexBuffer vertexBuffer;
			VertexDeclaration iVertexDeclaration;
			lock (graphicsDevice)
			{
				vertexBuffer = new VertexBuffer(graphicsDevice, 16 * array.Length, BufferUsage.WriteOnly);
				vertexBuffer.SetData<Vector4>(array);
				iVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
			}
			for (int j = 0; j < 3; j++)
			{
				this.mRenderData[j] = new GamePadMenuHelp.RenderData(texture2D, iEffect, vertexBuffer, iVertexDeclaration);
			}
		}

		// Token: 0x06002CB2 RID: 11442 RVA: 0x0015E5F4 File Offset: 0x0015C7F4
		public void ActivateButton(GamePadMenuHelp.Button iButton, int iText)
		{
			string @string = LanguageManager.Instance.GetString(iText);
			this.ActivateButton(iButton, @string);
		}

		// Token: 0x06002CB3 RID: 11443 RVA: 0x0015E615 File Offset: 0x0015C815
		public void ActivateButton(GamePadMenuHelp.Button iButton, string iText)
		{
			this.mActionButtons[(int)iButton].SetText(iText);
		}

		// Token: 0x06002CB4 RID: 11444 RVA: 0x0015E629 File Offset: 0x0015C829
		public void DeactivateButton(GamePadMenuHelp.Button iButton)
		{
			if (this.mActionButtons[(int)iButton].Active)
			{
				this.mActionButtons[(int)iButton].SetText(" - ");
			}
			this.mActionButtons[(int)iButton].Active = false;
		}

		// Token: 0x06002CB5 RID: 11445 RVA: 0x0015E668 File Offset: 0x0015C868
		public void DeactivateAll()
		{
			for (int i = 0; i < this.mActionButtons.Length; i++)
			{
				if (this.mActionButtons[i].Active)
				{
					this.mActionButtons[i].SetText(" - ");
				}
				this.mActionButtons[i].Active = false;
			}
		}

		// Token: 0x06002CB6 RID: 11446 RVA: 0x0015E6C4 File Offset: 0x0015C8C4
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			bool flag = false;
			for (int i = 0; i < this.mActionButtons.Length; i++)
			{
				flag |= this.mActionButtons[i].Active;
			}
			flag &= (ControlManager.Instance.GamePadCount > 0);
			for (int j = 0; j < this.mActionButtons.Length; j++)
			{
				if (flag)
				{
					this.mActionButtons[j].Alpha = Math.Min(this.mActionButtons[j].Alpha + iDeltaTime * GamePadMenuHelp.FADE_SPEED, 1f);
				}
				else
				{
					this.mActionButtons[j].Alpha = Math.Max(this.mActionButtons[j].Alpha - iDeltaTime * GamePadMenuHelp.FADE_SPEED, 0f);
				}
				this.mActionButtons[j].OldAlpha = Math.Max(this.mActionButtons[j].OldAlpha - iDeltaTime * GamePadMenuHelp.FADE_SPEED, 0f);
			}
			GamePadMenuHelp.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.Update(this.mActionButtons);
			GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x0400303B RID: 12347
		private static GamePadMenuHelp mSingelton;

		// Token: 0x0400303C RID: 12348
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400303D RID: 12349
		public static readonly int LOC_BACK = "#menu_back".GetHashCodeCustom();

		// Token: 0x0400303E RID: 12350
		public static readonly int LOC_SELECT = "#menu_select".GetHashCodeCustom();

		// Token: 0x0400303F RID: 12351
		public static readonly int HOSTLOCHASH = "#menu_opt_online_01".GetHashCodeCustom();

		// Token: 0x04003040 RID: 12352
		public static readonly int LOC_JOIN = "#menu_opt_online_03".GetHashCodeCustom();

		// Token: 0x04003041 RID: 12353
		public static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();

		// Token: 0x04003042 RID: 12354
		public static readonly int LOC_KICK = "#network_06".GetHashCodeCustom();

		// Token: 0x04003043 RID: 12355
		public static readonly int LOC_QUIT = "#menu_main_07".GetHashCodeCustom();

		// Token: 0x04003044 RID: 12356
		public static readonly int LOC_OPEN = "#menu_open".GetHashCodeCustom();

		// Token: 0x04003045 RID: 12357
		public static readonly int LOC_CLOSE = "#menu_close".GetHashCodeCustom();

		// Token: 0x04003046 RID: 12358
		private GamePadMenuHelp.ButtonData[] mActionButtons = new GamePadMenuHelp.ButtonData[4];

		// Token: 0x04003047 RID: 12359
		private GamePadMenuHelp.RenderData[] mRenderData = new GamePadMenuHelp.RenderData[3];

		// Token: 0x04003048 RID: 12360
		private static readonly float FADE_SPEED = 3f;

		// Token: 0x020005D4 RID: 1492
		private class RenderData : IRenderableGUIObject
		{
			// Token: 0x06002CB8 RID: 11448 RVA: 0x0015E89C File Offset: 0x0015CA9C
			public RenderData(Texture2D iTexture, GUIBasicEffect iEffect, VertexBuffer iVertexBuffer, VertexDeclaration iVertexDeclaration)
			{
				this.mEffect = iEffect;
				this.mTexture = iTexture;
				this.mVertexBuffer = iVertexBuffer;
				this.mVertexDeclaration = iVertexDeclaration;
				this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
				this.mData = new GamePadMenuHelp.RenderData.Data[4];
				for (int i = 0; i < this.mData.Length; i++)
				{
					this.mData[i].Text = new Text(64, this.mFont, TextAlign.Right, false);
					this.mData[i].Text.DrawShadows = true;
					this.mData[i].Text.ShadowsOffset = Vector2.One;
					this.mData[i].OldText = new Text(64, this.mFont, TextAlign.Right, false);
					this.mData[i].OldText.DrawShadows = true;
					this.mData[i].OldText.ShadowsOffset = Vector2.One;
				}
			}

			// Token: 0x06002CB9 RID: 11449 RVA: 0x0015E9A8 File Offset: 0x0015CBA8
			internal void Update(GamePadMenuHelp.ButtonData[] iButtons)
			{
				for (int i = 0; i < iButtons.Length; i++)
				{
					if (!iButtons[i].String.Equals(this.mData[i].String) && iButtons[i].Alpha > 0f)
					{
						this.mData[i].Text.SetText(iButtons[i].String);
						this.mData[i].String = iButtons[i].String;
					}
					this.mData[i].Alpha = iButtons[i].Alpha;
					if (!iButtons[i].OldString.Equals(this.mData[i].OldString) && iButtons[i].OldAlpha > 0f)
					{
						this.mData[i].OldText.SetText(iButtons[i].OldString);
						this.mData[i].OldString = iButtons[i].OldString;
					}
					this.mData[i].OldAlpha = iButtons[i].OldAlpha;
				}
			}

			// Token: 0x06002CBA RID: 11450 RVA: 0x0015EAF0 File Offset: 0x0015CCF0
			public void Draw(float iDeltaTime)
			{
				Vector4 color = default(Vector4);
				color.X = (color.Y = (color.Z = 1f));
				Point screenSize = RenderManager.Instance.ScreenSize;
				this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				Vector2 vector = default(Vector2);
				vector.X = (float)(screenSize.X - 8);
				vector.Y = 8f;
				float num = 0f;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				float num2 = 0f;
				for (int i = 0; i < this.mData.Length; i++)
				{
					color.W = this.mData[i].Alpha;
					if (color.W > 0f)
					{
						num = Math.Max(num, color.W);
						this.mEffect.Color = color;
						this.mData[i].Text.ShadowAlpha = color.W;
						this.mData[i].Text.Draw(this.mEffect, vector.X - 84f, vector.Y + num2);
					}
					color.W = this.mData[i].OldAlpha;
					if (color.W > 0f)
					{
						num = Math.Max(num, color.W);
						this.mEffect.Color = color;
						this.mData[i].OldText.ShadowAlpha = color.W;
						this.mData[i].OldText.Draw(this.mEffect, vector.X - 84f, vector.Y + num2);
					}
					num2 += 24f;
				}
				this.mEffect.Texture = this.mTexture;
				this.mEffect.TextureEnabled = true;
				color.W = num;
				this.mEffect.Color = color;
				Matrix transform = default(Matrix);
				transform.M11 = 1f;
				transform.M22 = 1f;
				transform.M41 = vector.X;
				transform.M42 = vector.Y;
				transform.M44 = 1f;
				this.mEffect.Transform = transform;
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x17000A96 RID: 2710
			// (get) Token: 0x06002CBB RID: 11451 RVA: 0x0015EDF6 File Offset: 0x0015CFF6
			public int ZIndex
			{
				get
				{
					return 1001;
				}
			}

			// Token: 0x04003049 RID: 12361
			private GUIBasicEffect mEffect;

			// Token: 0x0400304A RID: 12362
			private BitmapFont mFont;

			// Token: 0x0400304B RID: 12363
			private GamePadMenuHelp.RenderData.Data[] mData;

			// Token: 0x0400304C RID: 12364
			private Texture2D mTexture;

			// Token: 0x0400304D RID: 12365
			private VertexBuffer mVertexBuffer;

			// Token: 0x0400304E RID: 12366
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x020005D5 RID: 1493
			private struct Data
			{
				// Token: 0x0400304F RID: 12367
				public float Alpha;

				// Token: 0x04003050 RID: 12368
				public string String;

				// Token: 0x04003051 RID: 12369
				public Text Text;

				// Token: 0x04003052 RID: 12370
				public float OldAlpha;

				// Token: 0x04003053 RID: 12371
				public string OldString;

				// Token: 0x04003054 RID: 12372
				public Text OldText;
			}
		}

		// Token: 0x020005D6 RID: 1494
		private struct ButtonData
		{
			// Token: 0x06002CBC RID: 11452 RVA: 0x0015EE00 File Offset: 0x0015D000
			public void SetText(string iText)
			{
				if (!iText.Equals(this.String))
				{
					this.OldAlpha = this.Alpha;
					this.Alpha = 0f;
					this.OldString = this.String;
					this.String = iText;
				}
				this.Active = true;
			}

			// Token: 0x04003055 RID: 12373
			public string String;

			// Token: 0x04003056 RID: 12374
			public string OldString;

			// Token: 0x04003057 RID: 12375
			public float Alpha;

			// Token: 0x04003058 RID: 12376
			public float OldAlpha;

			// Token: 0x04003059 RID: 12377
			public bool Active;
		}

		// Token: 0x020005D7 RID: 1495
		public enum Button
		{
			// Token: 0x0400305B RID: 12379
			Top,
			// Token: 0x0400305C RID: 12380
			Y = 0,
			// Token: 0x0400305D RID: 12381
			Left,
			// Token: 0x0400305E RID: 12382
			X = 1,
			// Token: 0x0400305F RID: 12383
			Bottom,
			// Token: 0x04003060 RID: 12384
			A = 2,
			// Token: 0x04003061 RID: 12385
			Right,
			// Token: 0x04003062 RID: 12386
			B = 3
		}
	}
}
