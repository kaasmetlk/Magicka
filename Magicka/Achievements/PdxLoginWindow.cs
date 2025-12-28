using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Achievements
{
	// Token: 0x020002BC RID: 700
	internal class PdxLoginWindow : PdxWidgetWindow
	{
		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06001518 RID: 5400 RVA: 0x000854E0 File Offset: 0x000836E0
		public static PdxLoginWindow Instance
		{
			get
			{
				if (PdxLoginWindow.sSingelton == null)
				{
					lock (PdxLoginWindow.sSingeltonLock)
					{
						if (PdxLoginWindow.sSingelton == null)
						{
							PdxLoginWindow.sSingelton = new PdxLoginWindow();
						}
					}
				}
				return PdxLoginWindow.sSingelton;
			}
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x00085534 File Offset: 0x00083734
		private PdxLoginWindow()
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mEffect = new GUIBasicEffect(graphicsDevice, null);
			}
			Vector4[] array = new Vector4[16];
			int i = 0;
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.frame_profilebox_middle);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.frame_profilebox_top);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.frame_profilebox_bottom);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.popup_input_error);
			lock (graphicsDevice)
			{
				this.mVertices = new VertexBuffer(graphicsDevice, array.Length * 4 * 4, BufferUsage.WriteOnly);
				this.mVertices.SetData<Vector4>(array);
				this.mDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
			}
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.PDX_UI);
			BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.PDX_Edit);
			this.mErrorText = new Text(512, font, TextAlign.Left, false);
			this.mErrorText.SetText(AchievementsManager.Instance.GetTranslation(AchievementsManager.ERROR_SERVICE_UNAVAILABLE));
			this.mCloseButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[40], PdxWidgetWindow.sRectangles[41], font, AchievementsManager.BTN_CLOSE);
			this.mEnterButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[18], PdxWidgetWindow.sRectangles[19], font2, AchievementsManager.BTN_ENTER);
			this.mLoginHeaderText = new Text(64, font, TextAlign.Center, false);
			this.mUsernameText = new Text(32, font, TextAlign.Left, false);
			this.mPasswordText = new Text(32, font, TextAlign.Left, false);
			this.mInputErrorText = new Text(64, font, TextAlign.Left, false);
			this.mUsernameTextBox = new PdxTextBox(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[13], PdxWidgetWindow.sRectangles[12], PdxWidgetWindow.sRectangles[42], font2, 15);
			this.mUsernameTextBox.Active = true;
			this.mPasswordTextBox = new PdxTextBox(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[13], PdxWidgetWindow.sRectangles[12], PdxWidgetWindow.sRectangles[42], font2, 50);
			this.mPasswordTextBox.Mask = true;
			this.mRenderData = new PdxLoginWindow.RenderData[3];
			for (i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new PdxLoginWindow.RenderData();
			}
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x000857FC File Offset: 0x000839FC
		public override void Show()
		{
			if (AchievementsManager.Instance.LoggedIn)
			{
				return;
			}
			if (!this.mVisible)
			{
				AchievementsManager.Instance.SetLanguage(LanguageManager.Instance.CurrentLanguage);
			}
			base.Show();
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x00085830 File Offset: 0x00083A30
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (!AchievementsManager.Instance.Busy & AchievementsManager.Instance.LoggedIn)
			{
				this.Hide();
			}
			this.mProgress = (this.mProgress + iDeltaTime) % 2f;
			this.mUsernameTextBox.Update(iDeltaTime);
			this.mPasswordTextBox.Update(iDeltaTime);
			string logInError = AchievementsManager.Instance.LogInError;
			if (string.IsNullOrEmpty(logInError))
			{
				if (this.mInputError != 0)
				{
					this.mInputError = 0;
					this.mInputErrorText.Clear();
				}
			}
			else
			{
				int hashCodeCustom = logInError.GetHashCodeCustom();
				if (hashCodeCustom != this.mInputError)
				{
					this.mInputError = hashCodeCustom;
					string text = AchievementsManager.Instance.GetTranslation(this.mInputError);
					text = this.mInputErrorText.Font.Wrap(text, PdxWidgetWindow.sRectangles[6].Width - 45, true);
					this.mInputErrorText.SetText(text);
				}
			}
			PdxLoginWindow.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.Alpha = this.mAlpha;
			bool busy = AchievementsManager.Instance.Busy;
			renderData.DoProgress = (busy | !this.mVisible);
			renderData.DoServiceUnavailable = (AchievementsManager.Instance.Status != ServerRequestResult.SUCCESS);
			renderData.Progress = this.mProgress;
			GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x00085988 File Offset: 0x00083B88
		public override void OnLanguageChanged()
		{
			base.OnLanguageChanged();
			AchievementsManager instance = AchievementsManager.Instance;
			this.mErrorText.SetText(instance.GetTranslation(AchievementsManager.ERROR_SERVICE_UNAVAILABLE));
			this.mLoginHeaderText.SetText(instance.GetTranslation(AchievementsManager.LOGIN_HEADER));
			this.mUsernameText.SetText(instance.GetTranslation(AchievementsManager.USERNAME));
			this.mPasswordText.SetText(instance.GetTranslation(AchievementsManager.PASSWORD));
			this.mCloseButton.OnLanguageChanged();
			this.mEnterButton.OnLanguageChanged();
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x00085A10 File Offset: 0x00083C10
		public override void OnMouseDown(ref Vector2 iMousePos)
		{
			if (this.mCloseButton.InsideBounds(ref iMousePos))
			{
				this.mCloseButton.State |= PdxButton.ButtonState.Down;
			}
			if (this.mEnterButton.InsideBounds(ref iMousePos))
			{
				this.mEnterButton.State |= PdxButton.ButtonState.Down;
			}
			if (this.mUsernameTextBox.InsideBounds(ref iMousePos))
			{
				this.mPasswordTextBox.Active = false;
				this.mUsernameTextBox.Active = true;
				return;
			}
			if (this.mPasswordTextBox.InsideBounds(ref iMousePos))
			{
				this.mUsernameTextBox.Active = false;
				this.mPasswordTextBox.Active = true;
			}
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x00085AAC File Offset: 0x00083CAC
		public override void OnMouseUp(ref Vector2 iMousePos)
		{
			if (this.mCloseButton.InsideBounds(ref iMousePos) & (this.mCloseButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
			{
				this.Hide();
			}
			this.mCloseButton.State &= (PdxButton.ButtonState)(-2);
			if (this.mEnterButton.InsideBounds(ref iMousePos) & (this.mEnterButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
			{
				AchievementsManager.Instance.LogIn(this.mUsernameTextBox.String, this.mPasswordTextBox.String);
			}
			this.mEnterButton.State &= (PdxButton.ButtonState)(-2);
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x00085B4C File Offset: 0x00083D4C
		public override void OnMouseMove(ref Vector2 iMousePos)
		{
			if (this.mCloseButton.InsideBounds(ref iMousePos))
			{
				this.mCloseButton.State |= PdxButton.ButtonState.Over;
			}
			else
			{
				this.mCloseButton.State &= (PdxButton.ButtonState)(-3);
			}
			if (this.mEnterButton.InsideBounds(ref iMousePos))
			{
				this.mEnterButton.State |= PdxButton.ButtonState.Over;
				return;
			}
			this.mEnterButton.State &= (PdxButton.ButtonState)(-3);
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x00085BC8 File Offset: 0x00083DC8
		public override void OnKeyDown(KeyData iData)
		{
			Keys key = iData.Key;
			if (key <= Keys.Enter)
			{
				if (key != Keys.Tab)
				{
					if (key != Keys.Enter)
					{
						return;
					}
					AchievementsManager.Instance.LogIn(this.mUsernameTextBox.String, this.mPasswordTextBox.String);
				}
				else
				{
					if (this.mUsernameTextBox.Active)
					{
						this.mUsernameTextBox.Active = false;
						this.mPasswordTextBox.Active = true;
						return;
					}
					this.mPasswordTextBox.Active = false;
					this.mUsernameTextBox.Active = true;
					return;
				}
			}
			else
			{
				switch (key)
				{
				case Keys.Left:
					if (this.mUsernameTextBox.Active)
					{
						this.mUsernameTextBox.Cursor--;
						return;
					}
					if (this.mPasswordTextBox.Active)
					{
						this.mPasswordTextBox.Cursor--;
						return;
					}
					break;
				case Keys.Up:
					break;
				case Keys.Right:
					if (this.mUsernameTextBox.Active)
					{
						this.mUsernameTextBox.Cursor++;
						return;
					}
					if (this.mPasswordTextBox.Active)
					{
						this.mPasswordTextBox.Cursor++;
						return;
					}
					break;
				default:
					if (key != Keys.Delete)
					{
						return;
					}
					if (this.mUsernameTextBox.Active)
					{
						this.mUsernameTextBox.Delete();
						return;
					}
					if (this.mPasswordTextBox.Active)
					{
						this.mPasswordTextBox.Delete();
						return;
					}
					break;
				}
			}
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x00085D29 File Offset: 0x00083F29
		public override void OnKeyPress(char iChar)
		{
			if (this.mUsernameTextBox.Active)
			{
				this.mUsernameTextBox.AppendChar(iChar);
				return;
			}
			if (this.mPasswordTextBox.Active)
			{
				this.mPasswordTextBox.AppendChar(iChar);
			}
		}

		// Token: 0x040016B6 RID: 5814
		private static PdxLoginWindow sSingelton;

		// Token: 0x040016B7 RID: 5815
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040016B8 RID: 5816
		private GUIBasicEffect mEffect;

		// Token: 0x040016B9 RID: 5817
		private PdxLoginWindow.RenderData[] mRenderData;

		// Token: 0x040016BA RID: 5818
		private VertexBuffer mVertices;

		// Token: 0x040016BB RID: 5819
		private VertexDeclaration mDeclaration;

		// Token: 0x040016BC RID: 5820
		private float mProgress;

		// Token: 0x040016BD RID: 5821
		private Text mErrorText;

		// Token: 0x040016BE RID: 5822
		private PdxButton mCloseButton;

		// Token: 0x040016BF RID: 5823
		private PdxButton mEnterButton;

		// Token: 0x040016C0 RID: 5824
		private Text mLoginHeaderText;

		// Token: 0x040016C1 RID: 5825
		private Text mUsernameText;

		// Token: 0x040016C2 RID: 5826
		private Text mPasswordText;

		// Token: 0x040016C3 RID: 5827
		private int mInputError;

		// Token: 0x040016C4 RID: 5828
		private Text mInputErrorText;

		// Token: 0x040016C5 RID: 5829
		private PdxTextBox mUsernameTextBox;

		// Token: 0x040016C6 RID: 5830
		private PdxTextBox mPasswordTextBox;

		// Token: 0x020002BD RID: 701
		private class RenderData : IRenderableGUIObject
		{
			// Token: 0x06001523 RID: 5411 RVA: 0x00085D6C File Offset: 0x00083F6C
			public RenderData()
			{
				if (PdxLoginWindow.RenderData.mTmpVertices == null)
				{
					PdxLoginWindow.RenderData.mTmpVertices = new Vector4[4];
				}
			}

			// Token: 0x06001524 RID: 5412 RVA: 0x00085D94 File Offset: 0x00083F94
			public void Draw(float iDeltaTime)
			{
				PdxLoginWindow instance = PdxLoginWindow.Instance;
				GUIBasicEffect mEffect = instance.mEffect;
				Point screenSize = RenderManager.Instance.ScreenSize;
				mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				Vector4 color = default(Vector4);
				color.X = (color.Y = (color.Z = 1f));
				color.W = this.Alpha;
				mEffect.Color = color;
				mEffect.Texture = PdxWidgetWindow.sTexture;
				mEffect.TextureEnabled = true;
				mEffect.GraphicsDevice.Vertices[0].SetSource(instance.mVertices, 0, 16);
				mEffect.GraphicsDevice.VertexDeclaration = instance.mDeclaration;
				Vector2 vector = default(Vector2);
				vector.X = (float)((screenSize.X - 800) / 2);
				vector.Y = (float)((screenSize.Y - 600) / 2);
				mEffect.Begin();
				mEffect.CurrentTechnique.Passes[0].Begin();
				this.DrawQuad(mEffect, vector.X + 13f, vector.Y + 118f, 0);
				this.DrawQuad(mEffect, vector.X + 0f, vector.Y + 0f, 1);
				this.DrawQuad(mEffect, vector.X + 0f, vector.Y + 446f, 2);
				Vector2 position = default(Vector2);
				if (this.DoProgress)
				{
					Rectangle rectangle = PdxWidgetWindow.sRectangles[5];
					Rectangle rectangle2 = PdxWidgetWindow.sRectangles[4];
					int num = (int)(this.Progress * (float)rectangle.Width);
					if (num <= rectangle.Width)
					{
						rectangle.Width = num;
						rectangle2.X += num;
						rectangle2.Width -= num;
						this.DrawQuad(mEffect, vector.X + 330f, vector.Y + 210f, ref rectangle);
						this.DrawQuad(mEffect, vector.X + 330f + (float)num, vector.Y + 210f, ref rectangle2);
					}
					else
					{
						num -= rectangle.Width;
						rectangle2.Width = num;
						rectangle.X += num;
						rectangle.Width -= num;
						this.DrawQuad(mEffect, vector.X + 330f, vector.Y + 210f, ref rectangle2);
						this.DrawQuad(mEffect, vector.X + 330f + (float)num, vector.Y + 210f, ref rectangle);
					}
				}
				else if (this.DoServiceUnavailable)
				{
					instance.mErrorText.Draw(mEffect, vector.X + 200f, vector.Y + 200f);
				}
				else
				{
					if (instance.mInputErrorText.Characters[0] != '\0')
					{
						color.W = this.Alpha * this.Alpha;
						mEffect.Color = color;
						this.DrawQuad(mEffect, vector.X + 530f, vector.Y + 195f, 3);
						color.X = (color.Y = (color.Z = 0f));
						mEffect.Color = color;
						instance.mInputErrorText.Draw(mEffect, vector.X + 560f, vector.Y + 205f);
					}
					color.X = (color.Y = (color.Z = 1f));
					position.X = vector.X + 400f;
					position.Y = vector.Y + 170f;
					color.W = this.Alpha * this.Alpha * 0.8784314f;
					mEffect.Color = color;
					instance.mLoginHeaderText.Draw(mEffect, position.X, position.Y);
					position.X = vector.X + 300f;
					position.Y = vector.Y + 210f;
					color.W = this.Alpha * this.Alpha * 0.5647059f;
					mEffect.Color = color;
					instance.mUsernameText.Draw(mEffect, position.X, position.Y);
					position.Y = vector.Y + 285f;
					instance.mPasswordText.Draw(mEffect, position.X, position.Y);
					position.X = vector.X + 295f;
					position.Y = vector.Y + 233f;
					instance.mUsernameTextBox.Position = position;
					instance.mUsernameTextBox.Draw(mEffect, this.Alpha * this.Alpha);
					position.X = vector.X + 295f;
					position.Y = vector.Y + 308f;
					instance.mPasswordTextBox.Position = position;
					instance.mPasswordTextBox.Draw(mEffect, this.Alpha * this.Alpha);
					position.X = vector.X + 330f;
					position.Y = vector.Y + 367f;
					instance.mEnterButton.Position = position;
					instance.mEnterButton.Draw(mEffect, this.Alpha * this.Alpha);
				}
				position.X = vector.X + 580f;
				position.Y = vector.Y + 85f;
				instance.mCloseButton.Position = position;
				instance.mCloseButton.Draw(mEffect, this.Alpha * this.Alpha);
				mEffect.CurrentTechnique.Passes[0].End();
				mEffect.End();
			}

			// Token: 0x17000562 RID: 1378
			// (get) Token: 0x06001525 RID: 5413 RVA: 0x00086396 File Offset: 0x00084596
			public int ZIndex
			{
				get
				{
					return 2147483645;
				}
			}

			// Token: 0x06001526 RID: 5414 RVA: 0x0008639D File Offset: 0x0008459D
			private void DrawQuad(GUIBasicEffect iEffect, float iX, float iY, int iId)
			{
				this.mTransform.M41 = iX;
				this.mTransform.M42 = iY;
				iEffect.Transform = this.mTransform;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iId * 4, 2);
			}

			// Token: 0x06001527 RID: 5415 RVA: 0x000863DC File Offset: 0x000845DC
			private void DrawQuad(GUIBasicEffect iEffect, float iX, float iY, ref Rectangle iSourceRect)
			{
				this.mTransform.M41 = iX;
				this.mTransform.M42 = iY;
				iEffect.Transform = this.mTransform;
				iEffect.CommitChanges();
				PdxLoginWindow.RenderData.mTmpVertices[0].X = 0f;
				PdxLoginWindow.RenderData.mTmpVertices[0].Y = 0f;
				PdxLoginWindow.RenderData.mTmpVertices[0].Z = (float)iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
				PdxLoginWindow.RenderData.mTmpVertices[0].W = (float)iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
				PdxLoginWindow.RenderData.mTmpVertices[1].X = (float)iSourceRect.Width;
				PdxLoginWindow.RenderData.mTmpVertices[1].Y = 0f;
				PdxLoginWindow.RenderData.mTmpVertices[1].Z = (float)(iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
				PdxLoginWindow.RenderData.mTmpVertices[1].W = (float)iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
				PdxLoginWindow.RenderData.mTmpVertices[2].X = (float)iSourceRect.Width;
				PdxLoginWindow.RenderData.mTmpVertices[2].Y = (float)iSourceRect.Height;
				PdxLoginWindow.RenderData.mTmpVertices[2].Z = (float)(iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
				PdxLoginWindow.RenderData.mTmpVertices[2].W = (float)(iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
				PdxLoginWindow.RenderData.mTmpVertices[3].X = 0f;
				PdxLoginWindow.RenderData.mTmpVertices[3].Y = (float)iSourceRect.Height;
				PdxLoginWindow.RenderData.mTmpVertices[3].Z = (float)iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
				PdxLoginWindow.RenderData.mTmpVertices[3].W = (float)(iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
				iEffect.GraphicsDevice.DrawUserPrimitives<Vector4>(PrimitiveType.TriangleFan, PdxLoginWindow.RenderData.mTmpVertices, 0, 2);
			}

			// Token: 0x040016C7 RID: 5831
			public float Alpha;

			// Token: 0x040016C8 RID: 5832
			public bool DoProgress;

			// Token: 0x040016C9 RID: 5833
			public bool DoServiceUnavailable;

			// Token: 0x040016CA RID: 5834
			private Matrix mTransform = Matrix.Identity;

			// Token: 0x040016CB RID: 5835
			private static Vector4[] mTmpVertices;

			// Token: 0x040016CC RID: 5836
			public float Progress;
		}
	}
}
