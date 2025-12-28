using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x0200002F RID: 47
	internal abstract class SubMenu
	{
		// Token: 0x06000182 RID: 386 RVA: 0x0000AD25 File Offset: 0x00008F25
		protected static int GetSettingLoc(bool iSetting)
		{
			return SubMenu.GetSettingLoc(iSetting ? SettingOptions.On : SettingOptions.Off);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000AD34 File Offset: 0x00008F34
		protected static int GetSettingLoc(SettingOptions iSetting)
		{
			switch (iSetting)
			{
			case SettingOptions.Off:
				return SubMenu.LOC_OFF;
			case SettingOptions.On:
				return SubMenu.LOC_ON;
			case SettingOptions.Players_Only:
				return SubMenu.LOC_PLAYERS_ONLY;
			case SettingOptions.Low:
				return SubMenu.LOC_LOW;
			case SettingOptions.Medium:
				return SubMenu.LOC_MEDIUM;
			case SettingOptions.High:
				return SubMenu.LOC_HIGH;
			default:
				return 0;
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000AD88 File Offset: 0x00008F88
		protected static string GetSettingString(bool iSetting)
		{
			return SubMenu.GetSettingString(iSetting ? SettingOptions.On : SettingOptions.Off);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000AD96 File Offset: 0x00008F96
		protected static string GetSettingString(SettingOptions iSetting)
		{
			return LanguageManager.Instance.GetString(SubMenu.GetSettingLoc(iSetting));
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000ADA8 File Offset: 0x00008FA8
		public SubMenu()
		{
			this.mEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
			this.mEffect.TextureEnabled = true;
			this.mEffect.ScaleToHDR = true;
			Viewport pagerightsheet = Tome.PAGERIGHTSHEET;
			this.mEffect.SetScreenSize(pagerightsheet.Width, pagerightsheet.Height);
			this.mPosition = new Vector2((float)pagerightsheet.Width * 0.5f, (float)pagerightsheet.Height * 0.333f);
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (SubMenu.sPagesTexture == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					SubMenu.sPagesTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
					SubMenu.sStainsTexture = Game.Instance.Content.Load<Texture2D>("UI/ToM/Stains");
					SubMenu.sTagTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/tag_spritesheet");
				}
			}
			if (SubMenu.sGenericVertexBuffer == null)
			{
				Vector4[] data = new Vector4[]
				{
					new Vector4(0f, 0f, 0f, 0f),
					new Vector4(1f, 0f, 1f, 0f),
					new Vector4(1f, 1f, 1f, 1f),
					new Vector4(0f, 1f, 0f, 1f)
				};
				SubMenu.sGenericVertexBuffer = new VertexBuffer(graphicsDevice, 4 * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				SubMenu.sGenericVertexBuffer.SetData<Vector4>(data);
				SubMenu.sGenericVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
			}
			this.mKeyboardSelection = true;
			LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000AFF0 File Offset: 0x000091F0
		public virtual MenuTextItem AddMenuTextItem(int iText)
		{
			string @string = LanguageManager.Instance.GetString(iText);
			return this.AddMenuTextItem(@string);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000B010 File Offset: 0x00009210
		public virtual MenuTextItem AddMenuTextItem(string iText)
		{
			Vector2 iPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			iPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count;
			MenuTextItem menuTextItem = new MenuTextItem(iText, iPosition, font, TextAlign.Center);
			this.mMenuItems.Add(menuTextItem);
			return menuTextItem;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000B070 File Offset: 0x00009270
		public virtual MenuTextItem AddMenuTextItemBelowPrevious(int iText, float iExtraSpacing)
		{
			string @string = LanguageManager.Instance.GetString(iText);
			return this.AddMenuTextItemBelowPrevious(@string, iExtraSpacing);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000B094 File Offset: 0x00009294
		public virtual MenuTextItem AddMenuTextItemBelowPrevious(string iText, float iExtraSpacing)
		{
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			Vector2 position = this.mPosition;
			if (this.mMenuItems.Count > 0)
			{
				position = this.mMenuItems[this.mMenuItems.Count - 1].Position;
			}
			position.Y += (float)font.LineHeight + 10f + iExtraSpacing;
			MenuTextItem menuTextItem = new MenuTextItem(iText, position, font, TextAlign.Center);
			this.mMenuItems.Add(menuTextItem);
			return menuTextItem;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000B114 File Offset: 0x00009314
		public virtual MenuTextButtonItem CreateMenuTextButton(int iLoc, float minWidth)
		{
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			return new MenuTextButtonItem(Vector2.Zero, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, iLoc, font, minWidth, TextAlign.Center);
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000B14C File Offset: 0x0000934C
		public virtual void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.mEffect.VertexColorEnabled = false;
			this.mEffect.Color = MenuItem.COLOR;
			if (this.mMenuTitle != null)
			{
				this.mMenuTitle.Draw(this.mEffect, 512f, 96f);
			}
			foreach (MenuItem menuItem in this.mMenuItems)
			{
				menuItem.Draw(this.mEffect);
			}
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000B240 File Offset: 0x00009440
		public virtual void DrawNewAndOld(SubMenu iPreviousMenu, Viewport iCurrentLeftSide, Viewport iCurrentRightSide, Viewport iPreviousLeftSide, Viewport iPreviousRightSide)
		{
			this.Draw(iCurrentLeftSide, iCurrentRightSide);
			if (iPreviousMenu != null)
			{
				iPreviousMenu.Draw(iPreviousLeftSide, iPreviousRightSide);
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000B258 File Offset: 0x00009458
		public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mKeyboardSelection)
			{
				for (int i = 0; i < this.mMenuItems.Count; i++)
				{
					this.mMenuItems[i].Selected = (i == this.mSelectedPosition);
				}
			}
			if (this.mSelectedPosition >= 0 && this.mSelectedPosition < this.mMenuItems.Count)
			{
				MenuItem menuItem = this.mMenuItems[this.mSelectedPosition];
				this.mSelectedPositionF = MathHelper.Lerp(menuItem.Position.Y, this.mSelectedPositionF, (float)Math.Pow(0.00025, (double)iDeltaTime));
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000B2F8 File Offset: 0x000094F8
		public virtual void ControllerUp(Controller iSender)
		{
			if (this.mMenuItems == null)
			{
				return;
			}
			this.mKeyboardSelection = true;
			do
			{
				this.mSelectedPosition--;
				if (this.mSelectedPosition < 0)
				{
					this.mSelectedPosition = this.mMenuItems.Count - 1;
				}
			}
			while (!this.mMenuItems[this.mSelectedPosition].Enabled);
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000190 RID: 400 RVA: 0x0000B357 File Offset: 0x00009557
		public int NumItems
		{
			get
			{
				if (this.mMenuItems != null)
				{
					return this.mMenuItems.Count;
				}
				return 0;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000191 RID: 401 RVA: 0x0000B36E File Offset: 0x0000956E
		public int CurrentlySelectedPosition
		{
			get
			{
				return this.mSelectedPosition;
			}
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000B378 File Offset: 0x00009578
		public void ForceSetCurrentSelected(bool truefalse)
		{
			if (this.mMenuItems == null || this.mMenuItems.Count == 0 || this.mSelectedPosition < 0 || this.mSelectedPosition > this.mMenuItems.Count - 1)
			{
				return;
			}
			do
			{
				this.mMenuItems[this.mSelectedPosition].Selected = truefalse;
			}
			while (!this.mMenuItems[this.mSelectedPosition].Enabled);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000B3E8 File Offset: 0x000095E8
		public void UnselectAll()
		{
			this.ForceSetCurrentSelected(false);
			this.mSelectedPosition = -1;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000B3F8 File Offset: 0x000095F8
		public void ForceSetAndSelectCurrent(int index)
		{
			if (this.mMenuItems == null)
			{
				return;
			}
			if (index < 0)
			{
				index = 0;
			}
			else if (index > this.mMenuItems.Count - 1)
			{
				index = this.mMenuItems.Count - 1;
			}
			this.mSelectedPosition = index;
			this.ForceSetCurrentSelected(true);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000B444 File Offset: 0x00009644
		public virtual void ControllerDown(Controller iSender)
		{
			if (this.mMenuItems == null)
			{
				return;
			}
			this.mKeyboardSelection = true;
			do
			{
				this.mSelectedPosition++;
				if (this.mSelectedPosition >= this.mMenuItems.Count)
				{
					this.mSelectedPosition = 0;
				}
			}
			while (!this.mMenuItems[this.mSelectedPosition].Enabled);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000B4A4 File Offset: 0x000096A4
		public virtual void LanguageChanged()
		{
			if (this.mMenuItems == null)
			{
				return;
			}
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].LanguageChanged();
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000B4E1 File Offset: 0x000096E1
		protected virtual void ControllerMouseClicked(Controller iSender)
		{
			AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_select".GetHashCodeCustom());
			this.ControllerA(iSender);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000B500 File Offset: 0x00009700
		public virtual void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mMenuItems == null || this.mMenuItems.Count == 0 || iState.LeftButton == ButtonState.Pressed)
			{
				return;
			}
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag) && flag)
			{
				int i = 0;
				while (i < this.mMenuItems.Count)
				{
					MenuItem menuItem = this.mMenuItems[i];
					if (menuItem.Enabled && menuItem.InsideBounds(ref vector))
					{
						this.mSelectedPosition = i;
						if ((iState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed) || (iState.RightButton == ButtonState.Released && iOldState.RightButton == ButtonState.Pressed))
						{
							this.ControllerMouseClicked(iSender);
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000B5B4 File Offset: 0x000097B4
		public virtual void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mMenuItems == null || this.mMenuItems.Count == 0)
			{
				return;
			}
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag))
			{
				if (flag)
				{
					bool flag2 = false;
					for (int i = 0; i < this.mMenuItems.Count; i++)
					{
						MenuItem menuItem = this.mMenuItems[i];
						if (menuItem.Enabled && menuItem.InsideBounds(ref vector))
						{
							if (this.mSelectedPosition != i)
							{
								AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
							}
							this.mKeyboardSelection = false;
							this.mSelectedPosition = i;
							for (int j = 0; j < this.mMenuItems.Count; j++)
							{
								this.mMenuItems[j].Selected = (j == i);
							}
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						for (int k = 0; k < this.mMenuItems.Count; k++)
						{
							this.mMenuItems[k].Selected = false;
						}
						this.mSelectedPosition = -1;
						return;
					}
				}
			}
			else if (!this.mKeyboardSelection & this.mMenuItems != null)
			{
				for (int l = 0; l < this.mMenuItems.Count; l++)
				{
					this.mMenuItems[l].Selected = false;
				}
			}
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000B719 File Offset: 0x00009919
		protected void DrawGraphics(Texture2D iTexture, Rectangle iScrRect, Rectangle iDestRect)
		{
			this.DrawGraphics(iTexture, iScrRect, iDestRect, Vector4.One);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000B72C File Offset: 0x0000992C
		protected void DrawGraphics(Texture2D iTexture, Rectangle iScrRect, Rectangle iDestRect, Vector4 iColor)
		{
			Vector2 textureOffset = default(Vector2);
			Vector2 textureScale = default(Vector2);
			textureOffset.X = (float)iScrRect.X / (float)iTexture.Width;
			textureOffset.Y = (float)iScrRect.Y / (float)iTexture.Height;
			textureScale.X = (float)iScrRect.Width / (float)iTexture.Width;
			textureScale.Y = (float)iScrRect.Height / (float)iTexture.Height;
			this.mEffect.TextureOffset = textureOffset;
			this.mEffect.TextureScale = textureScale;
			Matrix transform = default(Matrix);
			transform.M11 = (float)iDestRect.Width;
			transform.M22 = (float)iDestRect.Height;
			transform.M41 = (float)iDestRect.X;
			transform.M42 = (float)iDestRect.Y;
			transform.M44 = 1f;
			this.mEffect.Transform = transform;
			this.mEffect.Texture = iTexture;
			this.mEffect.Color = iColor;
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenu.sGenericVertexBuffer, 0, 16);
			this.mEffect.GraphicsDevice.VertexDeclaration = SubMenu.sGenericVertexDeclaration;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000B888 File Offset: 0x00009A88
		public virtual void ControllerEvent(Controller iSender, KeyboardState iOldState, KeyboardState iNewState)
		{
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000B88A File Offset: 0x00009A8A
		public virtual void ControllerRight(Controller iSender)
		{
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000B88C File Offset: 0x00009A8C
		public virtual void ControllerLeft(Controller iSender)
		{
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000B88E File Offset: 0x00009A8E
		public virtual void ControllerA(Controller iSender)
		{
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000B890 File Offset: 0x00009A90
		public virtual void ControllerB(Controller iSender)
		{
			Tome.Instance.PopMenu();
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000B89C File Offset: 0x00009A9C
		public virtual void ControllerX(Controller iSender)
		{
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000B89E File Offset: 0x00009A9E
		public virtual void ControllerY(Controller iSender)
		{
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000B8A0 File Offset: 0x00009AA0
		public virtual void OnEnter()
		{
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000B8A2 File Offset: 0x00009AA2
		public virtual void OnExit()
		{
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000B8A4 File Offset: 0x00009AA4
		internal virtual void NetworkInput(ref MenuSelectMessage iMessage)
		{
		}

		// Token: 0x0400012C RID: 300
		public const float LINE_SEPARATION = 10f;

		// Token: 0x0400012D RID: 301
		protected const float BACK_PADDING = 8f;

		// Token: 0x0400012E RID: 302
		protected const int MAX_TITLE_LEN = 32;

		// Token: 0x0400012F RID: 303
		private const string TAG_SPRITESHEET_NAME = "UI/Menu/tag_spritesheet";

		// Token: 0x04000130 RID: 304
		public static readonly int LOC_MYTHOS = "#tsar_menu_mythos".GetHashCodeCustom();

		// Token: 0x04000131 RID: 305
		public static readonly int LOC_ADVENTURE = "#menu_main_01".GetHashCodeCustom();

		// Token: 0x04000132 RID: 306
		public static readonly int LOC_TSAR = "#menu_main_tsar".GetHashCodeCustom();

		// Token: 0x04000133 RID: 307
		public static readonly int LOC_CHALLENGES = "#menu_main_02".GetHashCodeCustom();

		// Token: 0x04000134 RID: 308
		public static readonly int LOC_VERSUS = "#menu_main_03".GetHashCodeCustom();

		// Token: 0x04000135 RID: 309
		public static readonly int LOC_LEADERBOARDS = "#menu_main_04".GetHashCodeCustom();

		// Token: 0x04000136 RID: 310
		public static readonly int LOC_ONLINE_PLAY = "#menu_main_05".GetHashCodeCustom();

		// Token: 0x04000137 RID: 311
		public static readonly int LOC_OPTIONS = "#menu_main_06".GetHashCodeCustom();

		// Token: 0x04000138 RID: 312
		public static readonly int LOC_QUIT = "#menu_main_07".GetHashCodeCustom();

		// Token: 0x04000139 RID: 313
		public static readonly int LOC_DOWNLOADABLE_CONTENT = "#menu_main_08".GetHashCodeCustom();

		// Token: 0x0400013A RID: 314
		public static readonly int LOC_HOW_TO_PLAY = "#menu_main_09".GetHashCodeCustom();

		// Token: 0x0400013B RID: 315
		public static readonly int LOC_YES = "#add_menu_yes".GetHashCodeCustom();

		// Token: 0x0400013C RID: 316
		public static readonly int LOC_NO = "#add_menu_no".GetHashCodeCustom();

		// Token: 0x0400013D RID: 317
		public static readonly int LOC_OK = "#add_menu_ok".GetHashCodeCustom();

		// Token: 0x0400013E RID: 318
		public static readonly int LOC_CANCEL = "#add_menu_cancel".GetHashCodeCustom();

		// Token: 0x0400013F RID: 319
		public static readonly int LOC_DELETE = "#add_menu_delete".GetHashCodeCustom();

		// Token: 0x04000140 RID: 320
		public static readonly int LOC_ON = "#menu_opt_alt_01".GetHashCodeCustom();

		// Token: 0x04000141 RID: 321
		public static readonly int LOC_OFF = "#menu_opt_alt_02".GetHashCodeCustom();

		// Token: 0x04000142 RID: 322
		public static readonly int LOC_PLAYERS_ONLY = "#menu_opt_alt_03".GetHashCodeCustom();

		// Token: 0x04000143 RID: 323
		public static readonly int LOC_LOW = "#menu_opt_alt_04".GetHashCodeCustom();

		// Token: 0x04000144 RID: 324
		public static readonly int LOC_MEDIUM = "#menu_opt_alt_05".GetHashCodeCustom();

		// Token: 0x04000145 RID: 325
		public static readonly int LOC_HIGH = "#menu_opt_alt_06".GetHashCodeCustom();

		// Token: 0x04000146 RID: 326
		public static readonly int LOC_NONE = "#menu_opt_alt_09".GetHashCodeCustom();

		// Token: 0x04000147 RID: 327
		public static readonly int LOC_ENABLED = "#menu_opt_alt_10".GetHashCodeCustom();

		// Token: 0x04000148 RID: 328
		public static readonly int LOC_DISABLED = "#menu_opt_alt_11".GetHashCodeCustom();

		// Token: 0x04000149 RID: 329
		public static readonly int LOC_ANY = "#network_32".GetHashCodeCustom();

		// Token: 0x0400014A RID: 330
		public static readonly int LOC_PREVIOUS_PAGE = "#menu_page_p".GetHashCodeCustom();

		// Token: 0x0400014B RID: 331
		public static readonly int LOC_NEXT_PAGE = "#menu_page_n".GetHashCodeCustom();

		// Token: 0x0400014C RID: 332
		public static readonly int LOC_SELECT = "#menu_select".GetHashCodeCustom();

		// Token: 0x0400014D RID: 333
		public static readonly int LOC_BACK = "#menu_back".GetHashCodeCustom();

		// Token: 0x0400014E RID: 334
		public static readonly int LOC_CLOSE = "#menu_close".GetHashCodeCustom();

		// Token: 0x0400014F RID: 335
		public static readonly int LOC_OPEN = "#menu_open".GetHashCodeCustom();

		// Token: 0x04000150 RID: 336
		public static readonly int LOC_PAUSED = "#menu_paused".GetHashCodeCustom();

		// Token: 0x04000151 RID: 337
		public static readonly int LOC_RESUME = "#menu_resume".GetHashCodeCustom();

		// Token: 0x04000152 RID: 338
		public static readonly int LOC_SETTINGS = "#add_menu_settings".GetHashCodeCustom();

		// Token: 0x04000153 RID: 339
		public static readonly int LOC_START = "#add_menu_start".GetHashCodeCustom();

		// Token: 0x04000154 RID: 340
		public static readonly int LOC_LOGIN = "".GetHashCodeCustom();

		// Token: 0x04000155 RID: 341
		public static readonly int LOC_REMEMBER_ME = "".GetHashCodeCustom();

		// Token: 0x04000156 RID: 342
		public static readonly int LOC_CHANGE_CHAPTER = "#change_chapter".GetHashCodeCustom();

		// Token: 0x04000157 RID: 343
		public static readonly int LOC_ENTER_NAME = "#ADD_MENU_PROF_NAME".GetHashCodeCustom();

		// Token: 0x04000158 RID: 344
		public static readonly int LOC_TT_DUNG1 = "#challenge_dungeons_chapter1".GetHashCodeCustom();

		// Token: 0x04000159 RID: 345
		public static readonly int LOC_TT_DUNG2 = "#challenge_dungeons_chapter2".GetHashCodeCustom();

		// Token: 0x0400015A RID: 346
		public static readonly int LOC_TT_OSOTC = "#challenge_osotc".GetHashCodeCustom();

		// Token: 0x0400015B RID: 347
		public static readonly int LOC_TT_VIETNAM = "#challenge_vietnam".GetHashCodeCustom();

		// Token: 0x0400015C RID: 348
		public static readonly int LOC_EMAIL = "#acc_login_email".GetHashCodeCustom();

		// Token: 0x0400015D RID: 349
		public static readonly int LOC_PASSWORD = "#acc_login_password".GetHashCodeCustom();

		// Token: 0x0400015E RID: 350
		public static readonly int LOC_DATEOFBIRTH = "#acc_dob".GetHashCodeCustom();

		// Token: 0x0400015F RID: 351
		public static readonly int LOC_POPUP_WARNING = "#popup_warning".GetHashCodeCustom();

		// Token: 0x04000160 RID: 352
		public static readonly int LOC_POPUP_INVALID_EMAIL = "#popup_invalid_email".GetHashCodeCustom();

		// Token: 0x04000161 RID: 353
		public static readonly int LOC_POPUP_INVALID_PASSWORD = "#popup_invalid_password".GetHashCodeCustom();

		// Token: 0x04000162 RID: 354
		public static readonly int LOC_POPUP_INVALID_DATEOFBIRTH = "#popup_invalid_dateofbirth".GetHashCodeCustom();

		// Token: 0x04000163 RID: 355
		public static readonly int LOC_POPUP_ACCOUNT_EXISTS = "#popup_acc_exists".GetHashCodeCustom();

		// Token: 0x04000164 RID: 356
		public static readonly int LOC_POPUP_ACCOUNT_REGISTERED = "#popup_acc_registered".GetHashCodeCustom();

		// Token: 0x04000165 RID: 357
		public static readonly int LOC_POPUP_MISSING_DETAILS = "#popup_missing_details".GetHashCodeCustom();

		// Token: 0x04000166 RID: 358
		public static readonly int LOC_POPUP_LOGIN_SUCCESS = "#popup_login_success".GetHashCodeCustom();

		// Token: 0x04000167 RID: 359
		public static readonly int LOC_POPUP_TERMSANDCONDITIONS = "#popup_termsandconditions".GetHashCodeCustom();

		// Token: 0x04000168 RID: 360
		protected static Texture2D sPagesTexture;

		// Token: 0x04000169 RID: 361
		protected static Texture2D sStainsTexture;

		// Token: 0x0400016A RID: 362
		protected static Texture2D sTagTexture;

		// Token: 0x0400016B RID: 363
		protected static readonly Vector2 BACK_SIZE = new Vector2(320f, 96f);

		// Token: 0x0400016C RID: 364
		protected static readonly Vector2 BACK_UVOFFSET = new Vector2((2048f - SubMenu.BACK_SIZE.X - 32f) / 2048f, 0.046875f);

		// Token: 0x0400016D RID: 365
		protected static readonly Vector2 BACK_UVSCALE = new Vector2(SubMenu.BACK_SIZE.X / 2048f, SubMenu.BACK_SIZE.Y / 1024f);

		// Token: 0x0400016E RID: 366
		protected static readonly Vector2 BACK_TEXTPOS = new Vector2(216f, SubMenu.BACK_SIZE.Y * 0.5f + 14f);

		// Token: 0x0400016F RID: 367
		protected static readonly MagickaFont BACK_FONT = MagickaFont.MenuOption;

		// Token: 0x04000170 RID: 368
		protected static readonly TextAlign BACK_TEXT_ALIGN = TextAlign.Center;

		// Token: 0x04000171 RID: 369
		protected static readonly Vector2 BACK_POSITION = new Vector2(96f, (float)Tome.PAGERIGHTSHEET.Y - SubMenu.BACK_SIZE.Y - 16f);

		// Token: 0x04000172 RID: 370
		protected int mSelectedPosition;

		// Token: 0x04000173 RID: 371
		protected bool mKeyboardSelection;

		// Token: 0x04000174 RID: 372
		protected Vector2 mPosition;

		// Token: 0x04000175 RID: 373
		protected GUIBasicEffect mEffect;

		// Token: 0x04000176 RID: 374
		protected List<MenuItem> mMenuItems;

		// Token: 0x04000177 RID: 375
		protected Text mMenuTitle;

		// Token: 0x04000178 RID: 376
		protected float mSelectedPositionF;

		// Token: 0x04000179 RID: 377
		private static VertexBuffer sGenericVertexBuffer;

		// Token: 0x0400017A RID: 378
		private static VertexDeclaration sGenericVertexDeclaration;
	}
}
