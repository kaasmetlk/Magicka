using System;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Graphics
{
	// Token: 0x02000247 RID: 583
	public class CutsceneText : TextBox
	{
		// Token: 0x0600121C RID: 4636 RVA: 0x0006E0BC File Offset: 0x0006C2BC
		public CutsceneText()
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
			this.mText = new TypingText(1024, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, false, 40f);
			this.mText.DefaultColor = Defines.DIALOGUE_COLOR_DEFAULT;
			this.mOwnerName = new Text(32, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, false);
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
			Texture2D iTexture;
			lock (Game.Instance.GraphicsDevice)
			{
				iTexture = Game.Instance.Content.Load<Texture2D>("UI/HUD/Dialog_say");
			}
			this.mGUIBasicEffect.Color = Vector4.One;
			for (int i = 0; i < 3; i++)
			{
				TextBox.RenderData renderData = new CutsceneText.CutSceneRenderData(iTexture);
				renderData.mTextBoxEffect = this.mTextBoxEffect;
				renderData.mGUIBasicEffect = this.mGUIBasicEffect;
				renderData.VertexBuffer = TextBox.sVertexBuffer;
				renderData.VertexDeclaration = TextBox.sVertexDeclaration;
				renderData.IndexBuffer = TextBox.sIndexBuffer;
				this.mRenderData[i] = renderData;
			}
			this.mBox = default(Rectangle);
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x0006E350 File Offset: 0x0006C550
		protected override void Initialize(Scene iScene, MagickaFont iFont, string iText, Vector2 iMinSize, bool iForceOnScreen, int iName, float iTTL)
		{
			base.Initialize(iScene, iFont, iText, iMinSize, iForceOnScreen, iName, iTTL);
			this.mBox.Width = 640;
		}

		// Token: 0x02000248 RID: 584
		protected class CutSceneRenderData : TextBox.RenderData
		{
			// Token: 0x0600121E RID: 4638 RVA: 0x0006E373 File Offset: 0x0006C573
			public CutSceneRenderData(Texture2D iTexture)
			{
				this.mTexture = iTexture;
			}

			// Token: 0x0600121F RID: 4639 RVA: 0x0006E384 File Offset: 0x0006C584
			public override void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				this.mTextBoxEffect.Color = new Vector4(1f, 1f, 1f, 1f);
				this.mTextBoxEffect.Position = this.mPosition;
				this.mTextBoxEffect.Size = this.mSize;
				this.mTextBoxEffect.Scale = this.mScale;
				this.mTextBoxEffect.Texture = this.mTexture;
				this.mTextBoxEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
				this.mTextBoxEffect.GraphicsDevice.Indices = this.mIndexBuffer;
				this.mTextBoxEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mTextBoxEffect.Begin();
				this.mTextBoxEffect.CurrentTechnique.Passes[0].Begin();
				this.mTextBoxEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 18);
				this.mTextBoxEffect.CurrentTechnique.Passes[0].End();
				this.mTextBoxEffect.End();
				this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
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

			// Token: 0x06001220 RID: 4640 RVA: 0x0006E664 File Offset: 0x0006C864
			public override void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
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
				Vector2 vector = default(Vector2);
				vector.Y = (float)Math.Floor((double)(screenSize2.Y * PlayState.CUTSCENE_BLACKBAR_SIZE));
				this.mPosition = new Vector2((float)screenSize.X * 0.5f, (float)screenSize.Y * 0.8f);
				this.mPosition.X = Math.Max(this.mPosition.X, (this.mSize.X + 32f) * 0.5f + vector.X);
				this.mPosition.Y = Math.Max(this.mPosition.Y, (this.mSize.Y + 32f) * 0.5f + vector.Y);
				this.mPosition.X = Math.Min(this.mPosition.X, screenSize2.X - (this.mSize.X + 32f) * 0.5f - vector.X);
				this.mPosition.Y = Math.Min(this.mPosition.Y, screenSize2.Y - (this.mSize.Y + 32f) * 0.5f - vector.Y);
			}

			// Token: 0x040010DE RID: 4318
			private Texture2D mTexture;
		}
	}
}
