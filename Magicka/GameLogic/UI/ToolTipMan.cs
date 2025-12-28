using System;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000563 RID: 1379
	internal class ToolTipMan
	{
		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x06002926 RID: 10534 RVA: 0x00142864 File Offset: 0x00140A64
		public static ToolTipMan Instance
		{
			get
			{
				if (ToolTipMan.sSingelton == null)
				{
					lock (ToolTipMan.sSingeltonLock)
					{
						if (ToolTipMan.sSingelton == null)
						{
							ToolTipMan.sSingelton = new ToolTipMan();
						}
					}
				}
				return ToolTipMan.sSingelton;
			}
		}

		// Token: 0x06002927 RID: 10535 RVA: 0x001428B8 File Offset: 0x00140AB8
		private ToolTipMan()
		{
			this.mRenderData = new ToolTipMan.RenderData[3];
			Vector2[] array = new Vector2[5];
			array[0].X = 0f;
			array[0].Y = 0f;
			array[1].X = 1f;
			array[1].Y = 0f;
			array[2].X = 1f;
			array[2].Y = 1f;
			array[3].X = 0f;
			array[3].Y = 1f;
			array[4] = array[0];
			this.mTips = new ToolTipMan.Tip[5];
			for (int i = 0; i < this.mTips.Length; i++)
			{
				this.mTips[i].ShowDelay = 0.25f;
				this.mTips[i].String = "";
				this.mTips[i].OldString = "";
			}
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
				this.mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0)
				});
				this.mVertices = new VertexBuffer(graphicsDevice, 8 * array.Length, BufferUsage.WriteOnly);
				this.mVertices.SetData<Vector2>(array);
				this.mEffect = new GUIBasicEffect(graphicsDevice, null);
				for (int j = 0; j < 3; j++)
				{
					this.mRenderData[j] = new ToolTipMan.RenderData(this.mTips.Length, this.mFont, this.mEffect, this.mVertices, this.mVertexDeclaration);
				}
			}
		}

		// Token: 0x06002928 RID: 10536 RVA: 0x00142AB0 File Offset: 0x00140CB0
		public void Initialize(int iMaxWidth, ref Vector4 iBackgroundColor, ref Vector4 iForegroundColor)
		{
			this.mMaxWidth = (int)Math.Ceiling((double)((float)iMaxWidth / 0.8f));
			this.KillAll(true);
			for (int i = 0; i < this.mRenderData.Length; i++)
			{
				this.mRenderData[i].Init(iBackgroundColor, iForegroundColor);
			}
		}

		// Token: 0x06002929 RID: 10537 RVA: 0x00142B08 File Offset: 0x00140D08
		public void Kill(Player iPlayer, bool iInstant)
		{
			int num = this.mTips.Length - 1;
			if (iPlayer != null)
			{
				num = iPlayer.ID;
			}
			this.mTips[num].TTL = 0f;
			if (iInstant)
			{
				this.mTips[num].ShowDelay = 0.25f;
				this.mTips[num].TTL = (this.mTips[num].Alpha = 0f);
			}
		}

		// Token: 0x0600292A RID: 10538 RVA: 0x00142B84 File Offset: 0x00140D84
		public void KillAll(bool iInstant)
		{
			for (int i = 0; i < this.mTips.Length; i++)
			{
				this.mTips[i].TTL = 0f;
				if (iInstant)
				{
					this.mTips[i].ShowDelay = 0.25f;
					this.mTips[i].TTL = (this.mTips[i].Alpha = 0f);
				}
			}
		}

		// Token: 0x0600292B RID: 10539 RVA: 0x00142C00 File Offset: 0x00140E00
		public void Set(Player iPlayer, string iString, MouseState iMouseState)
		{
			Vector2 vector = default(Vector2);
			vector.X = (float)iMouseState.X;
			vector.Y = (float)iMouseState.Y + 32f;
			this.Set(iPlayer, iString, ref vector, float.PositiveInfinity, false);
		}

		// Token: 0x0600292C RID: 10540 RVA: 0x00142C4C File Offset: 0x00140E4C
		public void Set(Player iPlayer, string iString, MouseState iMouseState, float iTTL)
		{
			Vector2 vector = default(Vector2);
			vector.X = (float)iMouseState.X;
			vector.Y = (float)iMouseState.Y + 32f;
			this.Set(iPlayer, iString, ref vector, iTTL, false);
		}

		// Token: 0x0600292D RID: 10541 RVA: 0x00142C94 File Offset: 0x00140E94
		public void Set(Player iPlayer, string iString, MouseState iMouseState, float iTTL, bool iSkipWait)
		{
			Vector2 vector = default(Vector2);
			vector.X = (float)iMouseState.X;
			vector.Y = (float)iMouseState.Y + 32f;
			this.Set(iPlayer, iString, ref vector, iTTL, iSkipWait);
		}

		// Token: 0x0600292E RID: 10542 RVA: 0x00142CDB File Offset: 0x00140EDB
		public void Set(Player iPlayer, string iString, ref Vector2 iPosition)
		{
			this.Set(iPlayer, iString, ref iPosition, float.PositiveInfinity, false);
		}

		// Token: 0x0600292F RID: 10543 RVA: 0x00142CEC File Offset: 0x00140EEC
		public void Set(Player iPlayer, string iString, ref Vector2 iPosition, float iTTL)
		{
			this.Set(iPlayer, iString, ref iPosition, iTTL, false);
		}

		// Token: 0x06002930 RID: 10544 RVA: 0x00142CFC File Offset: 0x00140EFC
		public void Set(Player iPlayer, string iString, ref Vector2 iPosition, float iTTL, bool iSkipWait)
		{
			if (iString == null)
			{
				throw new ArgumentNullException("iString");
			}
			int num = this.mTips.Length - 1;
			if (iPlayer != null)
			{
				num = iPlayer.ID;
			}
			iString = this.mFont.Wrap(iString, this.mMaxWidth, true);
			if (iSkipWait)
			{
				this.mTips[num].ShowDelay = 0f;
			}
			else if (!iString.Equals(this.mTips[num].String) || (this.mTips[num].TTL <= 0f && this.mTips[num].Alpha <= 0f))
			{
				this.mTips[num].ShowDelay = 0.25f;
			}
			this.mTips[num].String = iString;
			this.mTips[num].Position = iPosition;
			this.mTips[num].TTL = iTTL;
		}

		// Token: 0x06002931 RID: 10545 RVA: 0x00142DF8 File Offset: 0x00140FF8
		public void Update(Scene iScene, DataChannel iDataChannel, float iDeltaTime)
		{
			bool flag = false;
			for (int i = 0; i < this.mTips.Length; i++)
			{
				ToolTipMan.Tip[] array = this.mTips;
				int num = i;
				array[num].ShowDelay = array[num].ShowDelay - iDeltaTime;
				if (this.mTips[i].ShowDelay <= 0f)
				{
					this.mTips[i].OldString = this.mTips[i].String;
					this.mTips[i].OldPosition = this.mTips[i].Position;
					if (this.mTips[i].TTL > 0f)
					{
						this.mTips[i].Alpha = Math.Min(this.mTips[i].Alpha + iDeltaTime * 4f, 1f);
					}
					else
					{
						this.mTips[i].Alpha = Math.Max(this.mTips[i].Alpha - iDeltaTime * 4f, 0f);
					}
					ToolTipMan.Tip[] array2 = this.mTips;
					int num2 = i;
					array2[num2].TTL = array2[num2].TTL - iDeltaTime;
				}
				else
				{
					this.mTips[i].Alpha = Math.Max(this.mTips[i].Alpha - iDeltaTime * 4f, 0f);
				}
				flag |= (this.mTips[i].Alpha > 0f);
			}
			if (flag)
			{
				ToolTipMan.RenderData renderData = this.mRenderData[(int)iDataChannel];
				renderData.Update(this.mTips);
				iScene.AddRenderableGUIObject(iDataChannel, renderData);
			}
		}

		// Token: 0x04002C7B RID: 11387
		private const float SCALE = 0.8f;

		// Token: 0x04002C7C RID: 11388
		private const float MARGIN = 2f;

		// Token: 0x04002C7D RID: 11389
		public const float FADE_IN_SPEED = 4f;

		// Token: 0x04002C7E RID: 11390
		public const float FADE_OUT_SPEED = 4f;

		// Token: 0x04002C7F RID: 11391
		public const float SHOW_DELAY = 0.25f;

		// Token: 0x04002C80 RID: 11392
		private static ToolTipMan sSingelton;

		// Token: 0x04002C81 RID: 11393
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002C82 RID: 11394
		private int mMaxWidth;

		// Token: 0x04002C83 RID: 11395
		private ToolTipMan.Tip[] mTips;

		// Token: 0x04002C84 RID: 11396
		private ToolTipMan.RenderData[] mRenderData;

		// Token: 0x04002C85 RID: 11397
		private BitmapFont mFont;

		// Token: 0x04002C86 RID: 11398
		private GUIBasicEffect mEffect;

		// Token: 0x04002C87 RID: 11399
		private VertexBuffer mVertices;

		// Token: 0x04002C88 RID: 11400
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x02000564 RID: 1380
		private class RenderData : IRenderableGUIObject
		{
			// Token: 0x06002933 RID: 10547 RVA: 0x00142FB4 File Offset: 0x001411B4
			public RenderData(int iCount, BitmapFont iFont, GUIBasicEffect iEffect, VertexBuffer iBackgroundVertices, VertexDeclaration iDeclaration)
			{
				this.mEffect = iEffect;
				this.mVertices = iBackgroundVertices;
				this.mVertexDeclaration = iDeclaration;
				this.mData = new ToolTipMan.RenderData.Data[iCount];
				for (int i = 0; i < this.mData.Length; i++)
				{
					this.mData[i].Text = new Text(512, iFont, TextAlign.Left, false);
				}
			}

			// Token: 0x06002934 RID: 10548 RVA: 0x0014301C File Offset: 0x0014121C
			public void Init(Vector4 iBackgroundColor, Vector4 iForegroundColor)
			{
				this.mBackgroundColor = iBackgroundColor;
				this.mForegroundColor = iForegroundColor;
				for (int i = 0; i < this.mData.Length; i++)
				{
					this.mData[i].Text.DefaultColor = iForegroundColor;
				}
			}

			// Token: 0x06002935 RID: 10549 RVA: 0x00143064 File Offset: 0x00141264
			public void Update(ToolTipMan.Tip[] iTips)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				Vector2 vector = default(Vector2);
				vector.X = (float)screenSize.X;
				vector.Y = (float)screenSize.Y;
				for (int i = 0; i < this.mData.Length; i++)
				{
					Vector2 position;
					string text;
					if (iTips[i].ShowDelay <= 0f)
					{
						position = iTips[i].Position;
						text = iTips[i].String;
					}
					else
					{
						position = iTips[i].OldPosition;
						text = iTips[i].OldString;
					}
					Vector2 textSize;
					if (!text.Equals(this.mData[i].String))
					{
						this.mData[i].String = text;
						this.mData[i].Text.SetText(text);
						textSize = this.mData[i].Text.Font.MeasureText(text, true);
						textSize.X = (float)Math.Ceiling((double)(textSize.X * 0.8f));
						textSize.Y = (float)Math.Ceiling((double)(textSize.Y * 0.8f));
						this.mData[i].TextSize = textSize;
					}
					else
					{
						textSize = this.mData[i].TextSize;
					}
					position.X = Math.Max(Math.Min(position.X, vector.X - textSize.X - 2f), 3f);
					position.Y = Math.Max(Math.Min(position.Y, vector.Y - textSize.Y - 2f), 3f);
					this.mData[i].Position = position;
					this.mData[i].Alpha = iTips[i].Alpha;
				}
			}

			// Token: 0x06002936 RID: 10550 RVA: 0x0014326C File Offset: 0x0014146C
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				Vector2 vector = default(Vector2);
				vector.X = (float)screenSize.X;
				vector.Y = (float)screenSize.Y;
				this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				Matrix transform = default(Matrix);
				transform.M44 = 1f;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				for (int i = 0; i < this.mData.Length; i++)
				{
					float alpha = this.mData[i].Alpha;
					if (alpha > 0f)
					{
						this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 8);
						this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
						this.mEffect.VertexColorEnabled = false;
						this.mEffect.TextureEnabled = false;
						Vector2 position = this.mData[i].Position;
						transform.M41 = position.X - 2f - 0.5f;
						transform.M42 = position.Y - 2f - 0.5f;
						transform.M11 = this.mData[i].TextSize.X + 4f;
						transform.M22 = this.mData[i].TextSize.Y + 4f;
						this.mEffect.Transform = transform;
						Vector4 color = this.mBackgroundColor;
						color.W *= alpha;
						this.mEffect.Color = color;
						this.mEffect.CommitChanges();
						this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
						color = this.mForegroundColor;
						color.W *= alpha;
						this.mEffect.Color = color;
						this.mEffect.CommitChanges();
						this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);
						color.X = (color.Y = (color.Z = 1f));
						color.W = alpha * alpha;
						this.mEffect.Color = color;
						this.mEffect.VertexColorEnabled = true;
						this.mData[i].Text.Draw(this.mEffect, position.X, position.Y, 0.8f);
					}
				}
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x170009B1 RID: 2481
			// (get) Token: 0x06002937 RID: 10551 RVA: 0x0014354B File Offset: 0x0014174B
			public int ZIndex
			{
				get
				{
					return int.MaxValue;
				}
			}

			// Token: 0x04002C89 RID: 11401
			private ToolTipMan.RenderData.Data[] mData;

			// Token: 0x04002C8A RID: 11402
			private Vector4 mBackgroundColor;

			// Token: 0x04002C8B RID: 11403
			private Vector4 mForegroundColor;

			// Token: 0x04002C8C RID: 11404
			private GUIBasicEffect mEffect;

			// Token: 0x04002C8D RID: 11405
			private VertexBuffer mVertices;

			// Token: 0x04002C8E RID: 11406
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x02000565 RID: 1381
			private struct Data
			{
				// Token: 0x04002C8F RID: 11407
				public string String;

				// Token: 0x04002C90 RID: 11408
				public Text Text;

				// Token: 0x04002C91 RID: 11409
				public Vector2 TextSize;

				// Token: 0x04002C92 RID: 11410
				public float Alpha;

				// Token: 0x04002C93 RID: 11411
				public Vector2 Position;
			}
		}

		// Token: 0x02000566 RID: 1382
		private struct Tip
		{
			// Token: 0x04002C94 RID: 11412
			public float ShowDelay;

			// Token: 0x04002C95 RID: 11413
			public float TTL;

			// Token: 0x04002C96 RID: 11414
			public float Alpha;

			// Token: 0x04002C97 RID: 11415
			public Vector2 Position;

			// Token: 0x04002C98 RID: 11416
			public string String;

			// Token: 0x04002C99 RID: 11417
			public Vector2 OldPosition;

			// Token: 0x04002C9A RID: 11418
			public string OldString;
		}
	}
}
