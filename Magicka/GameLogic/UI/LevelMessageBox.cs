using System;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x020001B5 RID: 437
	internal class LevelMessageBox : MessageBox
	{
		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06000D56 RID: 3414 RVA: 0x0004D130 File Offset: 0x0004B330
		public static LevelMessageBox Instance
		{
			get
			{
				if (LevelMessageBox.sSingelton == null)
				{
					lock (LevelMessageBox.sSingeltonLock)
					{
						if (LevelMessageBox.sSingelton == null)
						{
							LevelMessageBox.sSingelton = new LevelMessageBox();
						}
					}
				}
				return LevelMessageBox.sSingelton;
			}
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x0004D184 File Offset: 0x0004B384
		static LevelMessageBox()
		{
			lock (Game.Instance.GraphicsDevice)
			{
				LevelMessageBox.sLevelVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_C.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				LevelMessageBox.sLevelVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_C);
				LevelMessageBox.sLevelVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			}
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x0004D26C File Offset: 0x0004B46C
		private LevelMessageBox() : base(" ")
		{
			Texture2D texture2D = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
			VertexPositionTexture[] array = new VertexPositionTexture[Defines.QUAD_TEX_VERTS_C.Length];
			Defines.QUAD_TEX_VERTS_C.CopyTo(array, 0);
			array[0].TextureCoordinate.X = 1344f / (float)texture2D.Width;
			array[0].TextureCoordinate.Y = 160f / (float)texture2D.Height;
			array[1].TextureCoordinate.X = 1280f / (float)texture2D.Width;
			array[1].TextureCoordinate.Y = 160f / (float)texture2D.Height;
			array[2].TextureCoordinate.X = 1280f / (float)texture2D.Width;
			array[2].TextureCoordinate.Y = 96f / (float)texture2D.Height;
			array[3].TextureCoordinate.X = 1344f / (float)texture2D.Width;
			array[3].TextureCoordinate.Y = 96f / (float)texture2D.Height;
			VertexBuffer vertexBuffer;
			VertexDeclaration iDeclaration;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				vertexBuffer.SetData<VertexPositionTexture>(array);
				iDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			}
			this.mTextureAlpha = 0f;
			this.mLeftArrow = new MenuImageItem(new Vector2(this.mCenter.X - this.mSize.X * 0.5f + LevelMessageBox.ARROW_PADDING, this.mCenter.Y + this.mSize.Y * 0.25f), MessageBox.sTexture, vertexBuffer, iDeclaration, 1.5707964f, 0, VertexPositionTexture.SizeInBytes, 32f, 32f);
			this.mRightArrow = new MenuImageItem(new Vector2(this.mCenter.X + this.mSize.X * 0.5f - LevelMessageBox.ARROW_PADDING, this.mCenter.Y + this.mSize.Y * 0.25f), MessageBox.sTexture, vertexBuffer, iDeclaration, -1.5707964f, 0, VertexPositionTexture.SizeInBytes, 32f, 32f);
			this.mLevelTitleSpacing = (this.mTitleFontHeight = (float)this.mLevelTitle.Font.LineHeight);
			lock (Game.Instance.GraphicsDevice)
			{
				this.mCustomLevelTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/CustomLevel");
			}
			this.mCustomLevelText = new Text(256, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, false);
			string text = LanguageManager.Instance.GetString(LevelMessageBox.LOC_CUSTOM);
			text = this.mCustomLevelText.Font.Wrap(text, 170, true);
			this.mCustomLevelText.SetText(text);
			text = LanguageManager.Instance.GetString(LevelMessageBox.LOC_LOADING);
			this.mLoadingTextLength = text.Length;
			this.mLoadingText.SetText(text);
			Vector2 iPosition = new Vector2(this.mCenter.X - 16f, this.mCenter.Y + this.mSize.Y * 0.5f - this.mTitleFontHeight - LevelMessageBox.OK_PADDING);
			this.mOkItem = new MenuTextItem(SubMenu.LOC_OK, iPosition, this.mFont, TextAlign.Right);
			this.mOkItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
			this.mOkItem.ColorSelected = Vector4.One;
			iPosition = new Vector2(this.mCenter.X + 16f, this.mCenter.Y + this.mSize.Y * 0.5f - this.mTitleFontHeight - LevelMessageBox.OK_PADDING);
			this.mCancelItem = new MenuTextItem(SubMenu.LOC_CANCEL, iPosition, this.mFont, TextAlign.Left);
			this.mCancelItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
			this.mCancelItem.ColorSelected = Vector4.One;
			this.mLevelTitle.DefaultColor = Defines.MESSAGEBOX_COLOR_DEFAULT;
			this.mLevelDescription.DefaultColor = Defines.MESSAGEBOX_COLOR_DEFAULT;
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x0004D74C File Offset: 0x0004B94C
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			LevelNode[] array;
			if (this.mLevelType == GameType.Challenge)
			{
				array = LevelManager.Instance.Challenges;
			}
			else
			{
				array = LevelManager.Instance.Versus;
			}
			if (this.mLevelSelection >= 0 && this.mLevelSelection < array.Length)
			{
				LevelNode levelNode = array[this.mLevelSelection];
				string text = LanguageManager.Instance.GetString(levelNode.Name.GetHashCodeCustom());
				text = this.mLevelTitle.Font.Wrap(text, LevelMessageBox.TITLE_WRAPPING, true);
				this.mLevelTitle.SetText(text);
				this.mLevelTitleSpacing = this.mLevelTitle.Font.MeasureText(text, true).Y;
				this.mLevelDescription.SetText(this.mLevelDescription.Font.Wrap(LanguageManager.Instance.GetString(levelNode.Description), LevelMessageBox.DESCR_WRAPPING, true));
			}
			string text2 = LanguageManager.Instance.GetString(LevelMessageBox.LOC_CUSTOM);
			text2 = this.mCustomLevelText.Font.Wrap(text2, 170, true);
			this.mCustomLevelText.SetText(text2);
			text2 = LanguageManager.Instance.GetString(LevelMessageBox.LOC_LOADING);
			this.mLoadingTextLength = text2.Length;
			this.mLoadingText.SetText(text2);
			this.mOkItem.LanguageChanged();
			this.mCancelItem.LanguageChanged();
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x0004D89D File Offset: 0x0004BA9D
		public override void Show()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x0004D8A4 File Offset: 0x0004BAA4
		public void Show(GameType iType, bool iAllowCustom, Action<Controller, GameType, int, bool> iOnComplete)
		{
			this.mTextureAlpha = 0f;
			this.mComplete = (Action<Controller, GameType, int, bool>)Delegate.Combine(this.mComplete, iOnComplete);
			this.mAllowCustom = iAllowCustom;
			if (iType != this.mLevelType || (!iAllowCustom && this.mCustomLevel != HackHelper.License.Yes))
			{
				this.mLevelSelection = 0;
				this.mLastStep = 1;
				LevelNode[] array;
				if (iType == GameType.Challenge)
				{
					array = LevelManager.Instance.Challenges;
				}
				else if (iType == GameType.StoryChallange)
				{
					array = LevelManager.Instance.StoryChallanges;
				}
				else
				{
					array = LevelManager.Instance.Versus;
				}
				HackHelper.License license = HackHelper.License.Pending;
				while (this.mLevelSelection < array.Length)
				{
					license = HackHelper.CheckLicense(array[this.mLevelSelection]);
					if (license == HackHelper.License.Pending || license == HackHelper.License.Yes || (license == HackHelper.License.Custom && iAllowCustom))
					{
						break;
					}
					this.mLevelSelection++;
				}
				this.mCustomLevel = license;
				LevelNode levelNode = array[this.mLevelSelection];
				string text = LanguageManager.Instance.GetString(levelNode.Name.GetHashCodeCustom());
				text = this.mLevelTitle.Font.Wrap(text, LevelMessageBox.TITLE_WRAPPING, true);
				this.mLevelTitle.SetText(text);
				this.mLevelTitleSpacing = this.mLevelTitle.Font.MeasureText(text, true).Y;
				if (levelNode.Name.Equals("#Location_debug", StringComparison.InvariantCultureIgnoreCase))
				{
					this.mLevelDescription.SetText(levelNode.FileName);
				}
				else
				{
					this.mLevelDescription.SetText(this.mLevelDescription.Font.Wrap(LanguageManager.Instance.GetString(levelNode.Description), LevelMessageBox.DESCR_WRAPPING, true));
				}
				this.mLevelType = iType;
				this.SetPreview(levelNode.LoadingImage);
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			this.mCenter.X = (float)screenSize.X * 0.5f;
			this.mCenter.Y = (float)screenSize.Y * 0.5f;
			this.mLeftArrow.Position = new Vector2(this.mCenter.X - this.mSize.X * 0.5f + LevelMessageBox.ARROW_PADDING, this.mCenter.Y + this.mSize.Y * 0.25f);
			this.mRightArrow.Position = new Vector2(this.mCenter.X + this.mSize.X * 0.5f - LevelMessageBox.ARROW_PADDING, this.mCenter.Y + this.mSize.Y * 0.25f);
			this.mOkItem.Position = new Vector2(this.mCenter.X - 16f, this.mCenter.Y + this.mSize.Y * 0.5f - this.mTitleFontHeight - LevelMessageBox.OK_PADDING);
			this.mCancelItem.Position = new Vector2(this.mCenter.X + 16f, this.mCenter.Y + this.mSize.Y * 0.5f - this.mTitleFontHeight - LevelMessageBox.OK_PADDING);
			base.Show();
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x0004DBB6 File Offset: 0x0004BDB6
		public override void Kill()
		{
			this.mComplete = null;
			base.Kill();
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x0004DBC5 File Offset: 0x0004BDC5
		public override void OnTextInput(char iChar)
		{
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x0004DBC8 File Offset: 0x0004BDC8
		private void SetPreview(string iPreviewFileName)
		{
			if (this.mLevelType == GameType.Versus)
			{
				this.mCurrentTexturePath = "Levels/Versus/" + iPreviewFileName;
			}
			else
			{
				this.mCurrentTexturePath = "Levels/Challenges/" + iPreviewFileName;
			}
			this.mTextureAlpha = 0f;
			Game.Instance.AddLoadTask(new Action(this.LoadTexture));
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x0004DC24 File Offset: 0x0004BE24
		private void LoadTexture()
		{
			lock (Game.Instance.GraphicsDevice)
			{
				this.mCurrentTexture = Game.Instance.Content.Load<Texture2D>(this.mCurrentTexturePath);
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06000D60 RID: 3424 RVA: 0x0004DC78 File Offset: 0x0004BE78
		public Texture2D SelectedTexture
		{
			get
			{
				return this.mSelectedTexture;
			}
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x0004DC80 File Offset: 0x0004BE80
		private void ChangeLevelPreview(int iNewLevel)
		{
			LevelNode[] array;
			if (this.mLevelType == GameType.Challenge)
			{
				array = LevelManager.Instance.Challenges;
			}
			else
			{
				array = LevelManager.Instance.Versus;
			}
			if (iNewLevel >= array.Length)
			{
				this.mLevelSelection = 0;
			}
			else if (iNewLevel < 0)
			{
				this.mLevelSelection = array.Length - 1;
			}
			else
			{
				this.mLevelSelection = iNewLevel;
			}
			LevelNode levelNode = array[this.mLevelSelection];
			string text = LanguageManager.Instance.GetString(levelNode.Name.GetHashCodeCustom());
			text = this.mLevelTitle.Font.Wrap(text, LevelMessageBox.TITLE_WRAPPING, true);
			this.mLevelTitle.SetText(text);
			this.mLevelTitleSpacing = this.mLevelTitle.Font.MeasureText(text, true).Y;
			if (levelNode.Name.Equals("#Location_debug", StringComparison.InvariantCultureIgnoreCase))
			{
				this.mLevelDescription.SetText(levelNode.FileName);
			}
			else
			{
				this.mLevelDescription.SetText(this.mLevelDescription.Font.Wrap(LanguageManager.Instance.GetString(levelNode.Description), LevelMessageBox.DESCR_WRAPPING, true));
			}
			this.SetPreview(levelNode.LoadingImage);
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x0004DD98 File Offset: 0x0004BF98
		private void ChangeSelection(int iStep)
		{
			this.mLastStep = iStep;
			int num = this.mLevelSelection + iStep;
			LevelNode[] array;
			if (this.mLevelType == GameType.Versus)
			{
				array = LevelManager.Instance.Versus;
			}
			else
			{
				array = LevelManager.Instance.Challenges;
			}
			HackHelper.License license = HackHelper.License.Pending;
			while (this.mLevelSelection != num)
			{
				if (num >= array.Length)
				{
					num -= array.Length;
				}
				if (num < 0)
				{
					num += array.Length;
				}
				license = HackHelper.CheckLicense(array[num]);
				if (license == HackHelper.License.Pending || license == HackHelper.License.Yes || (license == HackHelper.License.Custom && this.mAllowCustom))
				{
					break;
				}
				num += iStep;
			}
			this.mCustomLevel = license;
			this.mLevelSelection = num;
			this.ChangeLevelPreview(this.mLevelSelection);
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x0004DE38 File Offset: 0x0004C038
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
			switch (this.mSelection)
			{
			case LevelMessageBox.Selections.Level:
				if (iDirection == ControllerDirection.Left)
				{
					this.ChangeSelection(-1);
					return;
				}
				if (iDirection == ControllerDirection.Right)
				{
					this.ChangeSelection(1);
					return;
				}
				if (iDirection == ControllerDirection.Down | iDirection == ControllerDirection.Up)
				{
					this.mSelection = LevelMessageBox.Selections.OkButton;
					this.mOkItem.Selected = true;
					this.mCancelItem.Selected = false;
					return;
				}
				break;
			case LevelMessageBox.Selections.OkButton:
				if (iDirection == ControllerDirection.Up | iDirection == ControllerDirection.Down | iDirection == ControllerDirection.Left)
				{
					this.mSelection = LevelMessageBox.Selections.Level;
					this.mOkItem.Selected = false;
					this.mCancelItem.Selected = false;
					return;
				}
				if (iDirection == ControllerDirection.Right)
				{
					this.mSelection = LevelMessageBox.Selections.CancelButton;
					this.mOkItem.Selected = false;
					this.mCancelItem.Selected = true;
					return;
				}
				break;
			case LevelMessageBox.Selections.CancelButton:
				if (iDirection == ControllerDirection.Up)
				{
					this.mSelection = LevelMessageBox.Selections.Level;
					this.mOkItem.Selected = false;
					this.mCancelItem.Selected = false;
					return;
				}
				if (iDirection == ControllerDirection.Right | iDirection == ControllerDirection.Left)
				{
					this.mSelection = LevelMessageBox.Selections.OkButton;
					this.mOkItem.Selected = true;
					this.mCancelItem.Selected = false;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x0004DF48 File Offset: 0x0004C148
		public override void OnSelect(Controller iSender)
		{
			switch (this.mSelection)
			{
			case LevelMessageBox.Selections.Level:
			case LevelMessageBox.Selections.OkButton:
				if (this.mCustomLevel == HackHelper.License.Yes || this.mCustomLevel == HackHelper.License.Custom)
				{
					this.mSelectedTexture = this.mCurrentTexture;
					if (this.mComplete != null)
					{
						this.mComplete.Invoke(iSender, this.mLevelType, this.mLevelSelection, this.mCustomLevel != HackHelper.License.Yes);
					}
					this.Kill();
					return;
				}
				break;
			case LevelMessageBox.Selections.CancelButton:
				this.Kill();
				break;
			default:
				return;
			}
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x0004DFC8 File Offset: 0x0004C1C8
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			if ((float)iNewState.X > this.mCenter.X - this.mSize.X * 0.5f && (float)iNewState.X < this.mCenter.X + this.mSize.X * 0.5f && (float)iNewState.Y > this.mCenter.Y - this.mSize.Y * 0.5f && (float)iNewState.Y < this.mCenter.Y + this.mSize.Y * 0.5f)
			{
				if (this.mLeftArrow.InsideBounds(iNewState))
				{
					this.mOkItem.Selected = false;
					this.mCancelItem.Selected = false;
					this.mSelection = LevelMessageBox.Selections.Level;
					this.mLeftArrow.Selected = true;
					this.mRightArrow.Selected = false;
					return;
				}
				if (this.mRightArrow.InsideBounds(iNewState))
				{
					this.mOkItem.Selected = false;
					this.mCancelItem.Selected = false;
					this.mSelection = LevelMessageBox.Selections.Level;
					this.mLeftArrow.Selected = false;
					this.mRightArrow.Selected = true;
					return;
				}
				if (this.mOkItem.Enabled && this.mOkItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
				{
					this.mOkItem.Selected = true;
					this.mCancelItem.Selected = false;
					this.mSelection = LevelMessageBox.Selections.OkButton;
					this.mLeftArrow.Selected = false;
					this.mRightArrow.Selected = false;
					return;
				}
				if (this.mCancelItem.Enabled && this.mCancelItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
				{
					this.mOkItem.Selected = false;
					this.mCancelItem.Selected = true;
					this.mSelection = LevelMessageBox.Selections.CancelButton;
					this.mLeftArrow.Selected = false;
					this.mRightArrow.Selected = false;
					return;
				}
				this.mOkItem.Selected = false;
				this.mCancelItem.Selected = false;
				this.mSelection = LevelMessageBox.Selections.Level;
				this.mLeftArrow.Selected = false;
				this.mRightArrow.Selected = false;
			}
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x0004E204 File Offset: 0x0004C404
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (iNewState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
			{
				return;
			}
			if (this.mOkItem.Enabled && this.mOkItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mSelection = LevelMessageBox.Selections.OkButton;
				this.OnSelect(ControlManager.Instance.MenuController);
				return;
			}
			if (this.mCancelItem.Enabled && this.mCancelItem.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mSelection = LevelMessageBox.Selections.CancelButton;
				this.OnSelect(ControlManager.Instance.MenuController);
				return;
			}
			if (this.mLeftArrow.Enabled && this.mLeftArrow.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mSelection = LevelMessageBox.Selections.Level;
				this.ChangeSelection(-1);
				return;
			}
			if (this.mRightArrow.Enabled && this.mRightArrow.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mSelection = LevelMessageBox.Selections.Level;
				this.ChangeSelection(1);
			}
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x0004E31C File Offset: 0x0004C51C
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (this.mCustomLevel == HackHelper.License.Pending)
			{
				LevelNode iLevel;
				if (this.mLevelType == GameType.Challenge)
				{
					iLevel = LevelManager.Instance.Challenges[this.mLevelSelection];
				}
				else
				{
					iLevel = LevelManager.Instance.Versus[this.mLevelSelection];
				}
				HackHelper.License license = HackHelper.CheckLicense(iLevel);
				if (license == HackHelper.License.No || (license == HackHelper.License.Custom && !this.mAllowCustom))
				{
					this.ChangeSelection(this.mLastStep);
					return;
				}
				this.mCustomLevel = license;
			}
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x0004E394 File Offset: 0x0004C594
		public override void Draw(float iDeltaTime)
		{
			Matrix identity = Matrix.Identity;
			if (this.mCustomLevel == HackHelper.License.Custom)
			{
				identity.M11 = (identity.M22 = (identity.M33 = 1f));
				identity.M44 = 1f;
				identity.M41 = 0f;
				identity.M42 = this.mCenter.Y;
				MessageBox.sGUIBasicEffect.Transform = identity;
				Vector4 color = default(Vector4);
				color.X = (color.Y = (color.Z = 1f));
				color.W = this.mAlpha * this.mTextureAlpha;
				MessageBox.sGUIBasicEffect.Color = color;
				MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
				MessageBox.sGUIBasicEffect.Texture = MessageBox.sTexture;
				MessageBox.sGUIBasicEffect.TextureEnabled = true;
				MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(MessageBox.sVertexBuffer, 0, 16);
				MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = MessageBox.sVertexDeclaration;
				MessageBox.sGUIBasicEffect.Begin();
				MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				color = MenuItem.COLOR;
				color.W *= this.mTextureAlpha * this.mTextureAlpha;
				MessageBox.sGUIBasicEffect.Color = color;
				this.mCustomLevelText.Draw(MessageBox.sGUIBasicEffect, 20f, this.mCenter.Y - 200f);
				MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
				MessageBox.sGUIBasicEffect.End();
			}
			base.Draw(iDeltaTime);
			float num = this.mCenter.Y - this.mSize.Y * 0.5f + this.mTitleFontHeight + LevelMessageBox.TITLE_PADDING;
			Vector2 position = default(Vector2);
			position.Y = num + this.mTitleFontHeight * 0.5f;
			position.X = this.mCenter.X - this.mSize.X * 0.5f + LevelMessageBox.ARROW_PADDING;
			this.mLeftArrow.Position = position;
			position.X = this.mCenter.X + this.mSize.X * 0.5f - LevelMessageBox.ARROW_PADDING;
			this.mRightArrow.Position = position;
			if (this.mCustomLevel == HackHelper.License.Pending || this.mCustomLevel == HackHelper.License.No)
			{
				this.mLoadingDotTimer -= iDeltaTime;
				while (this.mLoadingDotTimer < 0f)
				{
					this.mLoadingDotTimer += 0.5f;
					int num2;
					if (this.mLoadingText.Characters[this.mLoadingTextLength] == '\0')
					{
						num2 = this.mLoadingTextLength;
					}
					else if (this.mLoadingText.Characters[this.mLoadingTextLength + 1] == '\0')
					{
						num2 = this.mLoadingTextLength + 1;
					}
					else if (this.mLoadingText.Characters[this.mLoadingTextLength + 2] == '\0')
					{
						num2 = this.mLoadingTextLength + 2;
					}
					else
					{
						num2 = -1;
						this.mLoadingText.Characters[this.mLoadingTextLength] = '\0';
					}
					if (num2 > 0)
					{
						this.mLoadingText.Characters[num2] = '.';
						this.mLoadingText.Characters[num2 + 1] = '\0';
					}
					this.mLoadingText.MarkAsDirty();
				}
				Vector2 vector = this.mLoadingText.Font.MeasureText(this.mLoadingText.Characters, true, this.mLoadingTextLength);
				this.mLoadingText.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X - vector.X * 0.5f, this.mCenter.Y - 50f);
			}
			else
			{
				MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
				MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
				this.mLevelTitle.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X, num);
				num += this.mLevelTitleSpacing;
				if (this.mCurrentTexture != null && !this.mCurrentTexture.IsDisposed)
				{
					this.mTextureAlpha = Math.Min(this.mTextureAlpha + iDeltaTime * 4f, 1f);
					lock (this.mCurrentTexture)
					{
						num += (float)this.mCurrentTexture.Height * 0.25f + LevelMessageBox.IMAGE_PADDING;
						identity.M42 = num;
						identity.M41 = this.mCenter.X;
						identity.M11 = (float)this.mCurrentTexture.Width * 0.5f;
						identity.M22 = (float)this.mCurrentTexture.Height * 0.5f;
						MessageBox.sGUIBasicEffect.Transform = identity;
						MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(LevelMessageBox.sLevelVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
						MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = LevelMessageBox.sLevelVertexDeclaration;
						MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
						MessageBox.sGUIBasicEffect.Texture = this.mCurrentTexture;
						MessageBox.sGUIBasicEffect.TextureEnabled = true;
						MessageBox.sGUIBasicEffect.TextureScale = Vector2.One;
						MessageBox.sGUIBasicEffect.TextureOffset = Vector2.Zero;
						MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mTextureAlpha * this.mAlpha);
						MessageBox.sGUIBasicEffect.Saturation = 1f;
						MessageBox.sGUIBasicEffect.CommitChanges();
						MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
						if (this.mCustomLevel == HackHelper.License.Custom)
						{
							MessageBox.sGUIBasicEffect.Texture = this.mCustomLevelTexture;
							MessageBox.sGUIBasicEffect.CommitChanges();
							MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
						}
						num += (float)this.mCurrentTexture.Height * 0.25f;
						goto IL_609;
					}
				}
				num += 256f;
				IL_609:
				MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
				MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
				this.mLevelDescription.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X, num);
			}
			this.mOkItem.Alpha = this.mAlpha;
			this.mOkItem.Draw(MessageBox.sGUIBasicEffect);
			this.mCancelItem.Alpha = this.mAlpha;
			this.mCancelItem.Draw(MessageBox.sGUIBasicEffect);
			this.mLeftArrow.Draw(MessageBox.sGUIBasicEffect, 48f);
			this.mRightArrow.Draw(MessageBox.sGUIBasicEffect, 48f);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x04000C04 RID: 3076
		private static LevelMessageBox sSingelton;

		// Token: 0x04000C05 RID: 3077
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04000C06 RID: 3078
		public static readonly int LOC_CUSTOM = "#notice_mod_content".GetHashCodeCustom();

		// Token: 0x04000C07 RID: 3079
		public static readonly int LOC_LOADING = "#network_23".GetHashCodeCustom();

		// Token: 0x04000C08 RID: 3080
		private static VertexBuffer sLevelVertexBuffer;

		// Token: 0x04000C09 RID: 3081
		private static VertexDeclaration sLevelVertexDeclaration;

		// Token: 0x04000C0A RID: 3082
		private static readonly int TITLE_WRAPPING = 336;

		// Token: 0x04000C0B RID: 3083
		private static readonly int DESCR_WRAPPING = 360;

		// Token: 0x04000C0C RID: 3084
		private static readonly float TITLE_PADDING = 8f;

		// Token: 0x04000C0D RID: 3085
		private static readonly float IMAGE_PADDING = 8f;

		// Token: 0x04000C0E RID: 3086
		private static readonly float ARROW_PADDING = 48f;

		// Token: 0x04000C0F RID: 3087
		private static readonly float OK_PADDING = 16f;

		// Token: 0x04000C10 RID: 3088
		private float mTitleFontHeight;

		// Token: 0x04000C11 RID: 3089
		private string mCurrentTexturePath;

		// Token: 0x04000C12 RID: 3090
		private Texture2D mCurrentTexture;

		// Token: 0x04000C13 RID: 3091
		private Texture2D mSelectedTexture;

		// Token: 0x04000C14 RID: 3092
		private float mTextureAlpha;

		// Token: 0x04000C15 RID: 3093
		private float mLevelTitleSpacing;

		// Token: 0x04000C16 RID: 3094
		private int mLevelSelection;

		// Token: 0x04000C17 RID: 3095
		private bool mAllowCustom;

		// Token: 0x04000C18 RID: 3096
		private HackHelper.License mCustomLevel = HackHelper.License.Pending;

		// Token: 0x04000C19 RID: 3097
		private Action<Controller, GameType, int, bool> mComplete;

		// Token: 0x04000C1A RID: 3098
		private GameType mLevelType = GameType.Campaign;

		// Token: 0x04000C1B RID: 3099
		private LevelMessageBox.Selections mSelection;

		// Token: 0x04000C1C RID: 3100
		private int mLoadingTextLength;

		// Token: 0x04000C1D RID: 3101
		private Text mLoadingText = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuDefault), TextAlign.Left, true);

		// Token: 0x04000C1E RID: 3102
		private float mLoadingDotTimer = 0.5f;

		// Token: 0x04000C1F RID: 3103
		private Text mLevelTitle = new Text(64, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center, false);

		// Token: 0x04000C20 RID: 3104
		private Text mLevelDescription = new Text(1024, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Center, false);

		// Token: 0x04000C21 RID: 3105
		private MenuTextItem mOkItem;

		// Token: 0x04000C22 RID: 3106
		private MenuTextItem mCancelItem;

		// Token: 0x04000C23 RID: 3107
		private MenuImageItem mLeftArrow;

		// Token: 0x04000C24 RID: 3108
		private MenuImageItem mRightArrow;

		// Token: 0x04000C25 RID: 3109
		private Texture2D mCustomLevelTexture;

		// Token: 0x04000C26 RID: 3110
		private Text mCustomLevelText;

		// Token: 0x04000C27 RID: 3111
		private int mLastStep;

		// Token: 0x020001B6 RID: 438
		private enum Selections
		{
			// Token: 0x04000C29 RID: 3113
			Level,
			// Token: 0x04000C2A RID: 3114
			OkButton,
			// Token: 0x04000C2B RID: 3115
			CancelButton
		}
	}
}
