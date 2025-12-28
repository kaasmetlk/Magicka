using System;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Graphics
{
	// Token: 0x020003EE RID: 1006
	public class NotifierButton
	{
		// Token: 0x06001EC0 RID: 7872 RVA: 0x000D6B7C File Offset: 0x000D4D7C
		public NotifierButton()
		{
			VertexPositionTexture[] array = new VertexPositionTexture[8];
			VertexPositionTexture vertexPositionTexture = default(VertexPositionTexture);
			vertexPositionTexture.Position.Z = 0f;
			vertexPositionTexture.Position.X = 64f;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = 1f;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[0] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 64f;
			vertexPositionTexture.Position.Y = 32f;
			vertexPositionTexture.TextureCoordinate.X = 1f;
			vertexPositionTexture.TextureCoordinate.Y = 1f;
			array[1] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 48f;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = 0.75f;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[2] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 48f;
			vertexPositionTexture.Position.Y = 32f;
			vertexPositionTexture.TextureCoordinate.X = 0.75f;
			vertexPositionTexture.TextureCoordinate.Y = 1f;
			array[3] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 32f;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = 0.5f;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[4] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 32f;
			vertexPositionTexture.Position.Y = 32f;
			vertexPositionTexture.TextureCoordinate.X = 0.5f;
			vertexPositionTexture.TextureCoordinate.Y = 1f;
			array[5] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = 0f;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[6] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = 32f;
			vertexPositionTexture.TextureCoordinate.X = 0f;
			vertexPositionTexture.TextureCoordinate.Y = 1f;
			array[7] = vertexPositionTexture;
			this.mVertices = new VertexBuffer(Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * 8, BufferUsage.WriteOnly);
			this.mVertices.SetData<VertexPositionTexture>(array);
			this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			this.mEffect = new NotifierButtonEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
			Game.Instance.AddLoadTask(delegate
			{
				this.mEffect.Texture = Game.Instance.Content.Load<Texture2D>("UI/HUD/NotifierButton");
			});
			this.mGUIEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			this.mGUIEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
			this.mGUIEffect.TextureEnabled = true;
			this.mGUIEffect.VertexColorEnabled = false;
			this.mRenderData = new NotifierButton.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				NotifierButton.RenderData renderData = new NotifierButton.RenderData(this.mEffect, this.mGUIEffect, this.mVertices, this.mVertexDeclaration);
				this.mRenderData[i] = renderData;
			}
			this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
			this.mKeyFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
			this.mText = new Text(32, this.mFont, TextAlign.Left, true);
			this.mKey = new Text(1, this.mKeyFont, TextAlign.Center, true);
		}

		// Token: 0x06001EC1 RID: 7873 RVA: 0x000D6FB4 File Offset: 0x000D51B4
		public void Show(string iText, ButtonChar iKey, Entity iOwner)
		{
			this.mText.SetText(iText);
			this.mKey.Characters[0] = (char)iKey;
			this.mKey.MarkAsDirty();
			this.mWidth = this.mFont.MeasureText(this.mText.Characters, true).X;
			this.mOwner = iOwner;
			this.mDialogAttach = null;
			this.mTargetAlpha = 1f;
		}

		// Token: 0x06001EC2 RID: 7874 RVA: 0x000D7024 File Offset: 0x000D5224
		public void Show(string iText, ButtonChar iKey, Vector2 iPosition)
		{
			this.mText.SetText(iText);
			this.mKey.Characters[0] = (char)iKey;
			this.mKey.MarkAsDirty();
			this.mWidth = this.mFont.MeasureText(this.mText.Characters, true).X;
			this.mOwner = null;
			this.mDialogAttach = null;
			this.mScreenPosition = iPosition;
			this.mTargetAlpha = 1f;
		}

		// Token: 0x06001EC3 RID: 7875 RVA: 0x000D7098 File Offset: 0x000D5298
		public void SetButtonIntensity(float iIntensity)
		{
			this.mIntensity = iIntensity;
		}

		// Token: 0x06001EC4 RID: 7876 RVA: 0x000D70A1 File Offset: 0x000D52A1
		public void SetButtonTargetIntensity(float iIntensity)
		{
			this.mTargetIntensity = iIntensity;
		}

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x06001EC5 RID: 7877 RVA: 0x000D70AA File Offset: 0x000D52AA
		public float ButtonIntensity
		{
			get
			{
				return this.mIntensity;
			}
		}

		// Token: 0x06001EC6 RID: 7878 RVA: 0x000D70B2 File Offset: 0x000D52B2
		public void Hide()
		{
			this.mTargetAlpha = 0f;
		}

		// Token: 0x06001EC7 RID: 7879 RVA: 0x000D70C0 File Offset: 0x000D52C0
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mTargetAlpha > 1E-45f)
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 3f, 1f);
			}
			else
			{
				this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 3f, 0f);
			}
			if (this.mAlpha <= 1E-45f)
			{
				return;
			}
			float amount = (float)Math.Pow(0.001, (double)iDeltaTime);
			this.mIntensity = MathHelper.Lerp(this.mTargetIntensity, this.mIntensity, amount);
			NotifierButton.RenderData renderData = this.mRenderData[(int)iDataChannel];
			if (this.mDialogAttach != null)
			{
				Entity owner = this.mDialogAttach.Owner;
				if (owner != null)
				{
					renderData.m3DPosition = true;
					renderData.mWorldPosition = owner.Position;
					if (owner is Magicka.GameLogic.Entities.Character)
					{
						Magicka.GameLogic.Entities.Character character = owner as Magicka.GameLogic.Entities.Character;
						NotifierButton.RenderData renderData2 = renderData;
						renderData2.mWorldPosition.Y = renderData2.mWorldPosition.Y + (character.Capsule.Length * 0.5f + character.Capsule.Radius);
					}
					renderData.mOffset.X = ((float)this.mDialogAttach.Width + 64f) / 3f - 90f;
					renderData.mOffset.Y = -58f;
				}
				else
				{
					renderData.m3DPosition = false;
					renderData.mWorldPosition.X = (float)this.mDialogAttach.Position.X;
					renderData.mWorldPosition.Y = (float)this.mDialogAttach.Position.Y;
				}
			}
			else
			{
				if (this.mOwner != null)
				{
					renderData.mWorldPosition = this.mOwner.Position;
					renderData.m3DPosition = true;
				}
				else
				{
					renderData.m3DPosition = false;
					renderData.mWorldPosition.X = this.mScreenPosition.X;
					renderData.mWorldPosition.Y = this.mScreenPosition.Y;
				}
				renderData.mOffset.X = -42f;
				renderData.mOffset.Y = 20f;
			}
			renderData.mButtonIntensity = this.mIntensity;
			renderData.mText = this.mText;
			renderData.mAlpha = this.mAlpha;
			renderData.mWidth = this.mWidth;
			renderData.mKey = this.mKey;
			if (KeyboardHUD.Instance.UIEnabled)
			{
				GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
			}
		}

		// Token: 0x04002114 RID: 8468
		private VertexBuffer mVertices;

		// Token: 0x04002115 RID: 8469
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04002116 RID: 8470
		private NotifierButtonEffect mEffect;

		// Token: 0x04002117 RID: 8471
		private GUIBasicEffect mGUIEffect;

		// Token: 0x04002118 RID: 8472
		private Entity mOwner;

		// Token: 0x04002119 RID: 8473
		private TextBox mDialogAttach;

		// Token: 0x0400211A RID: 8474
		private float mAlpha;

		// Token: 0x0400211B RID: 8475
		private float mTargetAlpha;

		// Token: 0x0400211C RID: 8476
		private Text mText;

		// Token: 0x0400211D RID: 8477
		private Text mKey;

		// Token: 0x0400211E RID: 8478
		private float mWidth;

		// Token: 0x0400211F RID: 8479
		private BitmapFont mFont;

		// Token: 0x04002120 RID: 8480
		private BitmapFont mKeyFont;

		// Token: 0x04002121 RID: 8481
		private NotifierButton.RenderData[] mRenderData;

		// Token: 0x04002122 RID: 8482
		private float mIntensity = 1f;

		// Token: 0x04002123 RID: 8483
		private float mTargetIntensity = 1f;

		// Token: 0x04002124 RID: 8484
		private Vector2 mScreenPosition;

		// Token: 0x020003EF RID: 1007
		protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
		{
			// Token: 0x06001EC9 RID: 7881 RVA: 0x000D7319 File Offset: 0x000D5519
			public RenderData(NotifierButtonEffect iEffect, GUIBasicEffect iGUIEffect, VertexBuffer iVertexBuffer, VertexDeclaration iVertexDeclaration)
			{
				this.mEffect = iEffect;
				this.mGUIEffect = iGUIEffect;
				this.mVertexBuffer = iVertexBuffer;
				this.mVertexDeclaration = iVertexDeclaration;
			}

			// Token: 0x06001ECA RID: 7882 RVA: 0x000D7340 File Offset: 0x000D5540
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				Vector4 color = default(Vector4);
				color.X = (color.Y = (color.Z = 1f));
				color.W = this.mAlpha;
				Vector2 screenSize2 = default(Vector2);
				screenSize2.X = (float)screenSize.X;
				screenSize2.Y = (float)screenSize.Y;
				this.mEffect.ScreenSize = screenSize2;
				this.mGUIEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.mEffect.Color = color;
				this.mEffect.Position = this.mPosition;
				this.mEffect.Width = this.mWidth;
				this.mEffect.Scale = Vector2.One;
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				if (this.mKey.Characters[0] == '̰')
				{
					this.mEffect.Width = this.mWidth - 34f;
					this.mEffect.Texture = Game.Instance.Content.Load<Texture2D>("UI/HUD/NotifierButton_Keyboard");
				}
				else
				{
					this.mEffect.Texture = Game.Instance.Content.Load<Texture2D>("UI/HUD/NotifierButton");
				}
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 6);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
				color.X = (color.Y = (color.Z = this.mButtonIntensity));
				this.mGUIEffect.Color = color;
				this.mGUIEffect.Begin();
				this.mGUIEffect.CurrentTechnique.Passes[0].Begin();
				if (this.mKey.Characters[0] == '̰')
				{
					color.X = (color.Y = (color.Z = 1f));
					this.mGUIEffect.Color = color;
					this.mText.Draw(this.mGUIEffect, this.mPosition.X + 8f, this.mPosition.Y + 8f);
				}
				else
				{
					this.mKey.Draw(this.mGUIEffect, this.mPosition.X + 16f, this.mPosition.Y + 3f);
					color.X = (color.Y = (color.Z = 1f));
					this.mGUIEffect.Color = color;
					this.mText.Draw(this.mGUIEffect, this.mPosition.X + 34f, this.mPosition.Y + 8f);
				}
				this.mGUIEffect.CurrentTechnique.Passes[0].End();
				this.mGUIEffect.End();
			}

			// Token: 0x17000787 RID: 1927
			// (get) Token: 0x06001ECB RID: 7883 RVA: 0x000D76AE File Offset: 0x000D58AE
			public int ZIndex
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06001ECC RID: 7884 RVA: 0x000D76B4 File Offset: 0x000D58B4
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				if (this.m3DPosition)
				{
					this.mPosition = MagickaMath.WorldToScreenPosition(ref this.mWorldPosition, ref iViewProjectionMatrix);
				}
				else
				{
					this.mPosition.X = this.mWorldPosition.X;
					this.mPosition.Y = this.mWorldPosition.Y;
				}
				Vector2.Add(ref this.mPosition, ref this.mOffset, out this.mPosition);
			}

			// Token: 0x04002125 RID: 8485
			protected NotifierButtonEffect mEffect;

			// Token: 0x04002126 RID: 8486
			protected GUIBasicEffect mGUIEffect;

			// Token: 0x04002127 RID: 8487
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04002128 RID: 8488
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04002129 RID: 8489
			public float mAlpha;

			// Token: 0x0400212A RID: 8490
			public bool m3DPosition;

			// Token: 0x0400212B RID: 8491
			public Vector3 mWorldPosition;

			// Token: 0x0400212C RID: 8492
			public Vector2 mOffset;

			// Token: 0x0400212D RID: 8493
			private Vector2 mPosition;

			// Token: 0x0400212E RID: 8494
			public float mWidth;

			// Token: 0x0400212F RID: 8495
			public Text mText;

			// Token: 0x04002130 RID: 8496
			public Text mKey;

			// Token: 0x04002131 RID: 8497
			public float mButtonIntensity;
		}
	}
}
