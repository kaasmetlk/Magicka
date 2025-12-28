using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI.Popup
{
	// Token: 0x02000428 RID: 1064
	public class MenuMessagePopup : MenuImagePopup
	{
		// Token: 0x17000811 RID: 2065
		// (set) Token: 0x060020F0 RID: 8432 RVA: 0x000EA6F3 File Offset: 0x000E88F3
		public bool ClearOnHide
		{
			set
			{
				this.mClearOnHide = value;
			}
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x000EA6FC File Offset: 0x000E88FC
		static MenuMessagePopup()
		{
			MenuMessagePopup.sDefaultTextFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			MenuMessagePopup.sDefaultButtonFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			MenuMessagePopup.sTagTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/tag_spritesheet");
		}

		// Token: 0x060020F2 RID: 8434 RVA: 0x000EA824 File Offset: 0x000E8A24
		public MenuMessagePopup() : this(MenuMessagePopup.sTagTexture, MenuMessagePopup.DEFAULT_UV, MenuMessagePopup.DEFAULT_SCALE, MenuMessagePopup.DEFAULT_MARGINS, 200f)
		{
			this.mSize = MenuMessagePopup.DEFAULT_SIZE;
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x000EA850 File Offset: 0x000E8A50
		public MenuMessagePopup(Texture2D iTexture, Vector2 iTextureUV, Vector2 iTextureSize, Vector4 iMargins, float iButtonMinWidth) : base(iTexture, iTextureUV, iTextureSize)
		{
			this.mButtons = new List<MenuTextButtonItem>();
			this.mLineHeight = MenuMessagePopup.sDefaultTextFont.LineHeight;
			this.mMargins = iMargins;
			this.mMinButtonWidth = iButtonMinWidth;
			this.mTitleText = new Text(256, MenuMessagePopup.sDefaultTextFont, TextAlign.Center, true);
			this.mTitleText.SetText(string.Empty);
			this.mMessageText = new Text(256, MenuMessagePopup.sDefaultTextFont, TextAlign.Center, true);
			this.mMessageText.SetText(string.Empty);
			this.mExtraText = new Text(256, MenuMessagePopup.sDefaultTextFont, TextAlign.Right, true);
			this.mExtraText.SetText(string.Empty);
			this.SetButtonType(ButtonConfig.Ok);
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x000EA92C File Offset: 0x000E8B2C
		public override void LanguageChanged()
		{
			this.SetTitle(this.mTitleLocId, new Color(this.mTitleText.DefaultColor));
			this.SetMessage(this.mMessageLocId, new Color(this.mMessageText.DefaultColor));
			this.SetExtra(this.mExtraLocId, new Color(this.mExtraText.DefaultColor));
			foreach (MenuTextButtonItem menuTextButtonItem in this.mButtons)
			{
				menuTextButtonItem.LanguageChanged();
			}
			this.UpdateBoundingBox();
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x000EA9D8 File Offset: 0x000E8BD8
		public void SetTitle(int iText, Vector4 iColour)
		{
			this.mTitleLocId = iText;
			this.mTitleText.DefaultColor = iColour;
			if (this.mTitleLocId == 0)
			{
				this.mTitleLineCount = 1;
				this.mTitleText.SetText(string.Empty);
				return;
			}
			string @string = LanguageManager.Instance.GetString(iText);
			this.mTitleLineCount = this.WrapText(ref @string, MenuMessagePopup.sDefaultTitleFont);
			this.mTitleText.SetText(@string);
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x000EAA43 File Offset: 0x000E8C43
		public void SetTitle(int iText, Color iColour)
		{
			this.SetTitle(iText, iColour.ToVector4());
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x000EAA54 File Offset: 0x000E8C54
		public void SetTitle(string iText, Vector4 iColour)
		{
			this.mTitleLocId = 0;
			this.mTitleText.DefaultColor = iColour;
			string text = iText;
			this.mTitleLineCount = this.WrapText(ref text, MenuMessagePopup.sDefaultTitleFont);
			this.mTitleText.SetText(text);
		}

		// Token: 0x060020F8 RID: 8440 RVA: 0x000EAA98 File Offset: 0x000E8C98
		public void SetMessage(int iText, Vector4 iColour)
		{
			this.mMessageLocId = iText;
			this.mMessageText.DefaultColor = iColour;
			if (this.mMessageLocId == 0)
			{
				this.mMessageLineCount = 1;
				this.mMessageText.SetText(string.Empty);
				return;
			}
			string @string = LanguageManager.Instance.GetString(iText);
			this.mMessageLineCount = this.WrapText(ref @string, MenuMessagePopup.sDefaultTextFont);
			this.mMessageText.SetText(@string);
		}

		// Token: 0x060020F9 RID: 8441 RVA: 0x000EAB04 File Offset: 0x000E8D04
		public void SetMessage(string iText, Vector4 iColour)
		{
			this.mMessageLocId = 0;
			this.mMessageText.DefaultColor = iColour;
			string text = iText;
			this.mMessageLineCount = this.WrapText(ref text, MenuMessagePopup.sDefaultTextFont);
			this.mMessageText.SetText(text);
		}

		// Token: 0x060020FA RID: 8442 RVA: 0x000EAB45 File Offset: 0x000E8D45
		public void SetMessage(int iText, Color iColour)
		{
			this.SetMessage(iText, iColour.ToVector4());
		}

		// Token: 0x060020FB RID: 8443 RVA: 0x000EAB58 File Offset: 0x000E8D58
		public void SetExtra(int iText, Vector4 iColour)
		{
			this.mExtraLocId = iText;
			this.mExtraText.DefaultColor = iColour;
			if (this.mExtraLocId == 0)
			{
				this.mExtraLineCount = 1;
				this.mExtraText.SetText(string.Empty);
				return;
			}
			string @string = LanguageManager.Instance.GetString(iText);
			this.mExtraLineCount = this.WrapText(ref @string, MenuMessagePopup.sDefaultTextFont);
			this.mExtraText.SetText(@string);
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x000EABC4 File Offset: 0x000E8DC4
		public void SetExtra(string iText, Vector4 iColour)
		{
			this.mExtraLocId = 0;
			this.mExtraText.DefaultColor = iColour;
			string text = iText;
			this.mExtraLineCount = this.WrapText(ref text, MenuMessagePopup.sDefaultTextFont);
			this.mExtraText.SetText(text);
		}

		// Token: 0x060020FD RID: 8445 RVA: 0x000EAC05 File Offset: 0x000E8E05
		public void SetExtra(int iText, Color iColour)
		{
			this.SetExtra(iText, iColour.ToVector4());
		}

		// Token: 0x060020FE RID: 8446 RVA: 0x000EAC15 File Offset: 0x000E8E15
		public void Clear()
		{
			this.SetTitle(0, Color.White);
			this.SetMessage(0, Color.White);
		}

		// Token: 0x060020FF RID: 8447 RVA: 0x000EAC30 File Offset: 0x000E8E30
		private int WrapText(ref string oText, BitmapFont iFont)
		{
			int iTargetLineWidth = (int)(this.mSize.X - this.mMargins.X - this.mMargins.Y);
			oText = iFont.Wrap(oText, iTargetLineWidth, true);
			oText = oText.Replace("\\n", "\n");
			return oText.Split(new char[]
			{
				'\n'
			}).Length;
		}

		// Token: 0x06002100 RID: 8448 RVA: 0x000EAC98 File Offset: 0x000E8E98
		private MenuTextButtonItem CreateButton(int iLoc)
		{
			return new MenuTextButtonItem(Vector2.Zero, MenuImagePopup.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, iLoc, MenuMessagePopup.sDefaultButtonFont, this.mMinButtonWidth, TextAlign.Center);
		}

		// Token: 0x06002101 RID: 8449 RVA: 0x000EACCC File Offset: 0x000E8ECC
		public void SetButtonType(ButtonConfig iType)
		{
			this.mButtons.Clear();
			this.mButtonConfig = iType;
			switch (this.mButtonConfig)
			{
			case ButtonConfig.Ok:
				this.mButtons.Add(this.CreateButton(MenuMessagePopup.LOC_BTN_OK));
				break;
			case ButtonConfig.Back:
				this.mButtons.Add(this.CreateButton(MenuMessagePopup.LOC_BTN_BACK));
				break;
			case ButtonConfig.OkCancel:
				this.mButtons.Add(this.CreateButton(MenuMessagePopup.LOC_BTN_YES));
				this.mButtons.Add(this.CreateButton(MenuMessagePopup.LOC_BTN_NO));
				break;
			}
			this.AlignButtons();
		}

		// Token: 0x06002102 RID: 8450 RVA: 0x000EAD6C File Offset: 0x000E8F6C
		private void AlignButtons()
		{
			Vector2 vector = new Vector2(this.mPosition.X, this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale - this.mMargins.W * this.mScale);
			if (this.mButtons.Count == 1)
			{
				this.mButtons[0].Position = vector;
				return;
			}
			if (this.mButtons.Count == 2)
			{
				this.mButtons[0].Position = vector - new Vector2(0.5f * (this.mButtons[0].RealWidth + 20f), 0f);
				this.mButtons[1].Position = vector + new Vector2(0.5f * (this.mButtons[1].RealWidth + 20f), 0f);
			}
		}

		// Token: 0x06002103 RID: 8451 RVA: 0x000EAE6D File Offset: 0x000E906D
		public void EnableLoadingIcon()
		{
			this.mLoadingImageState = new MenuMessagePopup.LoadingImageState(MenuMessagePopup.sTagTexture, MenuMessagePopup.LOADING_IMAGE_UV, MenuMessagePopup.LOADING_IMAGE_SCALE, 1.5f);
		}

		// Token: 0x06002104 RID: 8452 RVA: 0x000EAE90 File Offset: 0x000E9090
		public override void OnShow()
		{
			base.OnShow();
			this.mTitleText.MarkAsDirty();
			this.mTitleText.Position = new Vector2(this.mPosition.X, this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale + this.mMargins.Z * this.mScale);
			this.mMessageText.MarkAsDirty();
			this.mMessageText.Position = new Vector2(this.mPosition.X, this.mPosition.Y - (float)(this.mLineHeight * this.mMessageLineCount) * 0.5f * this.mScale);
			this.mExtraText.MarkAsDirty();
			this.mExtraText.Position = new Vector2(this.mPosition.X + this.mSize.X * 0.5f * this.mScale - this.mMargins.Z * this.mScale, this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale - this.mMargins.W * this.mScale - (float)(this.mLineHeight * this.mExtraLineCount) * 0.5f * this.mScale);
			this.AlignButtons();
			foreach (MenuTextButtonItem menuTextButtonItem in this.mButtons)
			{
				menuTextButtonItem.Selected = false;
			}
			if (this.mLoadingImageState != null)
			{
				Vector2 mPosition = this.mPosition;
				mPosition.X -= MenuMessagePopup.LOADING_IMAGE_SIZE.X * 0.5f * this.mScale;
				mPosition.Y += this.mSize.Y * 0.5f * this.mScale - (100f + MenuMessagePopup.LOADING_IMAGE_SIZE.Y) * this.mScale;
				this.mLoadingImageState.SetTransform(mPosition, MenuMessagePopup.LOADING_IMAGE_SIZE * this.mScale);
				this.mLoadingImageState.Reset();
			}
		}

		// Token: 0x06002105 RID: 8453 RVA: 0x000EB0DC File Offset: 0x000E92DC
		public override void OnHide()
		{
			if (this.mClearOnHide)
			{
				this.SetTitle(0, Color.White);
				this.SetMessage(0, Color.White);
				this.SetExtra(0, Color.White);
			}
		}

		// Token: 0x06002106 RID: 8454 RVA: 0x000EB10A File Offset: 0x000E930A
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (this.mLoadingImageState != null)
			{
				this.mLoadingImageState.Update(iDeltaTime);
			}
		}

		// Token: 0x06002107 RID: 8455 RVA: 0x000EB128 File Offset: 0x000E9328
		public override void Draw(GUIBasicEffect iEffect)
		{
			base.Draw(iEffect);
			this.mTitleText.Draw(iEffect, this.mScale);
			this.mMessageText.Draw(iEffect, this.mScale);
			this.mExtraText.Draw(iEffect, this.mScale);
			foreach (MenuTextButtonItem menuTextButtonItem in this.mButtons)
			{
				menuTextButtonItem.Draw(iEffect, this.mScale);
			}
			if (this.mLoadingImageState != null)
			{
				iEffect.GraphicsDevice.Vertices[0].SetSource(MenuImagePopup.sVertices, 0, VertexPositionTexture.SizeInBytes);
				iEffect.GraphicsDevice.VertexDeclaration = MenuImagePopup.sDeclaration;
				iEffect.VertexColorEnabled = false;
				Vector4 color = new Vector4(Vector3.One, this.mLoadingImageState.Alpha);
				iEffect.Color = color;
				iEffect.Saturation = 1f;
				iEffect.Texture = this.mLoadingImageState.Img;
				iEffect.TextureOffset = this.mLoadingImageState.TextureCoord;
				iEffect.TextureScale = this.mLoadingImageState.TextureScale;
				iEffect.TextureEnabled = true;
				iEffect.Transform = this.mLoadingImageState.Transform;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				iEffect.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
				iEffect.Color = this.mColour;
			}
		}

		// Token: 0x06002108 RID: 8456 RVA: 0x000EB2B0 File Offset: 0x000E94B0
		internal override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			this.mSelectedItem = -1;
			for (int i = 0; i < this.mButtons.Count; i++)
			{
				if (this.mButtons[i].InsideBounds(iState))
				{
					this.mButtons[i].Selected = true;
					this.mSelectedItem = i;
				}
				else
				{
					this.mButtons[i].Selected = false;
				}
			}
		}

		// Token: 0x06002109 RID: 8457 RVA: 0x000EB31C File Offset: 0x000E951C
		internal override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mSelectedItem == 0)
			{
				this.Dismiss();
				if (this.mOnPositiveClickDelegate != null)
				{
					this.mOnPositiveClickDelegate.Invoke();
					this.mOnPositiveClickDelegate = null;
					return;
				}
			}
			else if (this.mSelectedItem == 1)
			{
				this.Dismiss();
				if (this.mOnNegativeClickDelegate != null)
				{
					this.mOnNegativeClickDelegate.Invoke();
					this.mOnNegativeClickDelegate = null;
				}
			}
		}

		// Token: 0x0600210A RID: 8458 RVA: 0x000EB37C File Offset: 0x000E957C
		internal override void ControllerA(Controller iSender)
		{
			if (this.mSelectedItem == 0)
			{
				this.Dismiss();
				if (this.mOnPositiveClickDelegate != null)
				{
					this.mOnPositiveClickDelegate.Invoke();
					this.mOnPositiveClickDelegate = null;
					return;
				}
			}
			else if (this.mSelectedItem == 1)
			{
				this.Dismiss();
				if (this.mOnNegativeClickDelegate != null)
				{
					this.mOnNegativeClickDelegate.Invoke();
					this.mOnNegativeClickDelegate = null;
				}
			}
		}

		// Token: 0x04002379 RID: 9081
		public const float DEFAULT_BTN_SIZE = 200f;

		// Token: 0x0400237A RID: 9082
		private const float BUTTON_PADDING = 20f;

		// Token: 0x0400237B RID: 9083
		private const float LOADING_IMAGE_PADDING = 100f;

		// Token: 0x0400237C RID: 9084
		private const float LOADING_IMAGE_FADE_TIME = 1.5f;

		// Token: 0x0400237D RID: 9085
		private const int MAX_TEXT_LENGTH = 256;

		// Token: 0x0400237E RID: 9086
		private const string TAG_SPRITESHEET_NAME = "UI/Menu/tag_spritesheet";

		// Token: 0x0400237F RID: 9087
		public static readonly Vector2 DEFAULT_UV = new Vector2(517f, 4f);

		// Token: 0x04002380 RID: 9088
		public static readonly Vector2 DEFAULT_SCALE = new Vector2(499f, 357f);

		// Token: 0x04002381 RID: 9089
		public static readonly Vector4 DEFAULT_MARGINS = new Vector4(25f, 25f, 80f, 100f);

		// Token: 0x04002382 RID: 9090
		public static readonly Vector2 DEFAULT_SIZE = MenuMessagePopup.DEFAULT_SCALE * 1.6f;

		// Token: 0x04002383 RID: 9091
		private static readonly Vector2 LOADING_IMAGE_UV = new Vector2(889f, 406f);

		// Token: 0x04002384 RID: 9092
		private static readonly Vector2 LOADING_IMAGE_SCALE = new Vector2(120f, 94f);

		// Token: 0x04002385 RID: 9093
		private static readonly Vector2 LOADING_IMAGE_SIZE = new Vector2(120f, 94f);

		// Token: 0x04002386 RID: 9094
		protected static readonly int LOC_BTN_OK = "#add_menu_ok".GetHashCodeCustom();

		// Token: 0x04002387 RID: 9095
		protected static readonly int LOC_BTN_BACK = "#menu_back".GetHashCodeCustom();

		// Token: 0x04002388 RID: 9096
		protected static readonly int LOC_BTN_YES = "#add_menu_yes".GetHashCodeCustom();

		// Token: 0x04002389 RID: 9097
		protected static readonly int LOC_BTN_NO = "#add_menu_no".GetHashCodeCustom();

		// Token: 0x0400238A RID: 9098
		protected static BitmapFont sDefaultTextFont;

		// Token: 0x0400238B RID: 9099
		protected static BitmapFont sDefaultTitleFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);

		// Token: 0x0400238C RID: 9100
		protected static BitmapFont sDefaultButtonFont;

		// Token: 0x0400238D RID: 9101
		protected static Texture2D sTagTexture;

		// Token: 0x0400238E RID: 9102
		private int mTitleLocId;

		// Token: 0x0400238F RID: 9103
		private Text mTitleText;

		// Token: 0x04002390 RID: 9104
		private int mTitleLineCount = 1;

		// Token: 0x04002391 RID: 9105
		private int mMessageLocId;

		// Token: 0x04002392 RID: 9106
		private Text mMessageText;

		// Token: 0x04002393 RID: 9107
		private int mMessageLineCount = 1;

		// Token: 0x04002394 RID: 9108
		private int mExtraLocId;

		// Token: 0x04002395 RID: 9109
		private Text mExtraText;

		// Token: 0x04002396 RID: 9110
		private int mExtraLineCount = 1;

		// Token: 0x04002397 RID: 9111
		private Vector4 mMargins;

		// Token: 0x04002398 RID: 9112
		private int mLineHeight;

		// Token: 0x04002399 RID: 9113
		private float mMinButtonWidth;

		// Token: 0x0400239A RID: 9114
		private ButtonConfig mButtonConfig;

		// Token: 0x0400239B RID: 9115
		private List<MenuTextButtonItem> mButtons;

		// Token: 0x0400239C RID: 9116
		private MenuMessagePopup.LoadingImageState mLoadingImageState;

		// Token: 0x0400239D RID: 9117
		private bool mClearOnHide = true;

		// Token: 0x02000429 RID: 1065
		private enum FadeState
		{
			// Token: 0x0400239F RID: 9119
			FadeIn,
			// Token: 0x040023A0 RID: 9120
			FadeOut
		}

		// Token: 0x0200042A RID: 1066
		protected class LoadingImageState
		{
			// Token: 0x17000812 RID: 2066
			// (get) Token: 0x0600210B RID: 8459 RVA: 0x000EB3DB File Offset: 0x000E95DB
			// (set) Token: 0x0600210C RID: 8460 RVA: 0x000EB3E3 File Offset: 0x000E95E3
			public float Alpha { get; private set; }

			// Token: 0x17000813 RID: 2067
			// (get) Token: 0x0600210D RID: 8461 RVA: 0x000EB3EC File Offset: 0x000E95EC
			// (set) Token: 0x0600210E RID: 8462 RVA: 0x000EB3F4 File Offset: 0x000E95F4
			public Matrix Transform { get; private set; }

			// Token: 0x0600210F RID: 8463 RVA: 0x000EB400 File Offset: 0x000E9600
			public LoadingImageState(Texture2D iTexture, Vector2 iTextureCoord, Vector2 iTextureScale, float iFadeTime)
			{
				this.Alpha = 1f;
				this.mFadeState = MenuMessagePopup.FadeState.FadeOut;
				this.Img = iTexture;
				this.TextureCoord = iTextureCoord / new Vector2((float)iTexture.Width, (float)iTexture.Height);
				this.TextureScale = iTextureScale / new Vector2((float)iTexture.Width, (float)iTexture.Height);
				this.mFadeTime = iFadeTime;
			}

			// Token: 0x06002110 RID: 8464 RVA: 0x000EB474 File Offset: 0x000E9674
			public void SetTransform(Vector2 iPosition, Vector2 iSize)
			{
				Matrix identity = Matrix.Identity;
				identity.M11 = iSize.X;
				identity.M22 = iSize.Y;
				identity.M41 = iPosition.X;
				identity.M42 = iPosition.Y;
				this.Transform = identity;
			}

			// Token: 0x06002111 RID: 8465 RVA: 0x000EB4C6 File Offset: 0x000E96C6
			public void Reset()
			{
				this.Alpha = 1f;
				this.mFadeState = MenuMessagePopup.FadeState.FadeOut;
			}

			// Token: 0x06002112 RID: 8466 RVA: 0x000EB4DC File Offset: 0x000E96DC
			public void Update(float iDeltaTime)
			{
				this.Alpha += ((this.mFadeState == MenuMessagePopup.FadeState.FadeIn) ? iDeltaTime : (-iDeltaTime)) / this.mFadeTime;
				if (this.Alpha < 0f || this.Alpha > 1f)
				{
					this.mFadeState = ((this.mFadeState == MenuMessagePopup.FadeState.FadeIn) ? MenuMessagePopup.FadeState.FadeOut : MenuMessagePopup.FadeState.FadeIn);
					this.Alpha = ((this.Alpha < 0f) ? 0f : ((this.Alpha > 1f) ? 1f : this.Alpha));
				}
			}

			// Token: 0x040023A1 RID: 9121
			public readonly Texture2D Img;

			// Token: 0x040023A2 RID: 9122
			public readonly Vector2 TextureCoord;

			// Token: 0x040023A3 RID: 9123
			public readonly Vector2 TextureScale;

			// Token: 0x040023A4 RID: 9124
			private MenuMessagePopup.FadeState mFadeState;

			// Token: 0x040023A5 RID: 9125
			private float mFadeTime;
		}
	}
}
