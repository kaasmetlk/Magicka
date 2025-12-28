using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x02000030 RID: 48
	internal class SubMenuOptionsGame : SubMenu
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000BCC8 File Offset: 0x00009EC8
		public static SubMenuOptionsGame Instance
		{
			get
			{
				if (SubMenuOptionsGame.mSingelton == null)
				{
					lock (SubMenuOptionsGame.mSingeltonLock)
					{
						if (SubMenuOptionsGame.mSingelton == null)
						{
							SubMenuOptionsGame.mSingelton = new SubMenuOptionsGame();
						}
					}
				}
				return SubMenuOptionsGame.mSingelton;
			}
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000BD1C File Offset: 0x00009F1C
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			for (int i = 0; i < this.mMenuOptions.Count; i++)
			{
				this.mMenuOptions[i].LanguageChanged();
			}
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGame.LOC_GAME_OPTS));
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000BDA8 File Offset: 0x00009FA8
		private SubMenuOptionsGame()
		{
			this.mSelectedPosition = 0;
			this.mMenuItems = new List<MenuItem>();
			this.mMenuOptions = new List<MenuTextItem>();
			this.mMenuTitle = new Text(30, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGame.LOC_GAME_OPTS));
			this.AddMenuOptions(SubMenu.GetSettingLoc(this.mGlobalSettings.BloodAndGore));
			this.AddMenuOptions(SubMenu.GetSettingLoc(this.mGlobalSettings.DamageNumbers));
			this.AddMenuOptions(SubMenu.GetSettingLoc(this.mGlobalSettings.HealthBars));
			this.AddMenuOptions(SubMenu.GetSettingLoc(this.mGlobalSettings.SpellWheel));
			this.AddMenuOptions(LanguageManager.Instance.GetNativeName(LanguageManager.Instance.CurrentLanguage));
			this.AddMenuTextItem(SubMenuOptionsGame.LOC_GORE);
			this.AddMenuTextItem(SubMenuOptionsGame.LOC_DAMNUMBERS);
			this.AddMenuTextItem(SubMenuOptionsGame.LOC_HBARS);
			this.AddMenuTextItem(SubMenuOptionsGame.LOC_SPELLWHEEL);
			this.AddMenuTextItem(SubMenuOptionsGame.LOC_LANGUAGE);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
			LanguageMessageBox.Instance.Complete += this.OnSelect;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000BF34 File Offset: 0x0000A134
		public override MenuTextItem AddMenuTextItem(int iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count;
			MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, font, TextAlign.Right);
			this.mMenuItems.Add(menuTextItem);
			return menuTextItem;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000BF94 File Offset: 0x0000A194
		public void AddMenuOptions(int iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.X += 40f;
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuOptions.Count;
			this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left));
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000C004 File Offset: 0x0000A204
		public void AddMenuOptions(string iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.X += 40f;
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuOptions.Count;
			this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left));
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000C073 File Offset: 0x0000A273
		private void OnSelect(Language iLanguage)
		{
			LanguageManager.Instance.SetLanguage(iLanguage);
			this.mMenuOptions[4].SetText(LanguageManager.Instance.GetNativeName(LanguageManager.Instance.CurrentLanguage));
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000C0A8 File Offset: 0x0000A2A8
		public override void OnEnter()
		{
			base.OnEnter();
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			this.mBloodAndGore = this.mGlobalSettings.BloodAndGore;
			this.mDamageNumbers = this.mGlobalSettings.DamageNumbers;
			this.mHealthBars = this.mGlobalSettings.HealthBars;
			this.mSpellWheel = this.mGlobalSettings.SpellWheel;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000C138 File Offset: 0x0000A338
		public override void OnExit()
		{
			base.OnExit();
			if (this.mBloodAndGore != this.mGlobalSettings.BloodAndGore | this.mDamageNumbers != this.mGlobalSettings.DamageNumbers | this.mHealthBars != this.mGlobalSettings.HealthBars | this.mSpellWheel != this.mGlobalSettings.SpellWheel)
			{
				SaveManager.Instance.SaveSettings();
			}
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000C1B4 File Offset: 0x0000A3B4
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			base.Draw(iLeftSide, iRightSide);
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			base.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48), new Rectangle(208, 220, 608, 48));
			foreach (MenuItem menuItem in this.mMenuOptions)
			{
				menuItem.Draw(this.mEffect);
			}
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000C298 File Offset: 0x0000A498
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mKeyboardSelection)
			{
				for (int i = 0; i < this.mMenuItems.Count; i++)
				{
					this.mMenuItems[i].Selected = (i == this.mSelectedPosition);
					if (i < this.mMenuOptions.Count)
					{
						this.mMenuOptions[i].Selected = (i == this.mSelectedPosition);
					}
				}
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000C308 File Offset: 0x0000A508
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mMenuItems == null || this.mMenuItems.Count == 0)
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
					MenuItem menuItem2 = null;
					if (i < this.mMenuOptions.Count)
					{
						menuItem2 = this.mMenuOptions[i];
					}
					if (menuItem.Enabled && (menuItem.InsideBounds(ref vector) || (menuItem2 != null && menuItem2.InsideBounds(ref vector))))
					{
						this.mSelectedPosition = i;
						if (iState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
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

		// Token: 0x060001B3 RID: 435 RVA: 0x0000C3D6 File Offset: 0x0000A5D6
		protected override void ControllerMouseClicked(Controller iSender)
		{
			this.mKeyboardSelection = true;
			if (this.mSelectedPosition == 5)
			{
				Tome.Instance.PopMenu();
			}
			if (this.mSelectedPosition == 4)
			{
				LanguageMessageBox.Instance.Show();
			}
			else
			{
				this.ControllerRight(iSender);
			}
			this.mKeyboardSelection = false;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000C418 File Offset: 0x0000A618
		public override void ControllerA(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 4:
				LanguageMessageBox.Instance.Show();
				return;
			case 5:
				Tome.Instance.PopMenu();
				return;
			default:
				this.ControllerRight(iSender);
				return;
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000C45C File Offset: 0x0000A65C
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag) && flag)
			{
				bool flag2 = false;
				for (int i = 0; i < this.mMenuItems.Count; i++)
				{
					MenuItem menuItem = this.mMenuItems[i];
					MenuItem menuItem2 = null;
					if (i < this.mMenuOptions.Count)
					{
						menuItem2 = this.mMenuOptions[i];
					}
					if (menuItem.Enabled && (menuItem.InsideBounds(ref vector) || (menuItem2 != null && menuItem2.InsideBounds(ref vector))))
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
							if (j < this.mMenuOptions.Count)
							{
								this.mMenuOptions[j].Selected = (j == i);
							}
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
						if (k < this.mMenuOptions.Count)
						{
							this.mMenuOptions[k].Selected = false;
						}
					}
				}
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000C5E0 File Offset: 0x0000A7E0
		public override void ControllerRight(Controller iSender)
		{
			if (!this.mKeyboardSelection)
			{
				return;
			}
			GlobalSettings instance = GlobalSettings.Instance;
			switch (this.mSelectedPosition)
			{
			case 0:
				switch (instance.BloodAndGore)
				{
				default:
					instance.BloodAndGore = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.BloodAndGore = SettingOptions.Off;
					break;
				}
				this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.BloodAndGore));
				return;
			case 1:
				switch (instance.DamageNumbers)
				{
				default:
					instance.DamageNumbers = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.DamageNumbers = SettingOptions.Off;
					break;
				}
				this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.DamageNumbers));
				return;
			case 2:
				switch (instance.HealthBars)
				{
				default:
					instance.HealthBars = SettingOptions.Players_Only;
					break;
				case SettingOptions.On:
					instance.HealthBars = SettingOptions.Off;
					break;
				case SettingOptions.Players_Only:
					instance.HealthBars = SettingOptions.On;
					break;
				}
				this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.HealthBars));
				return;
			case 3:
				switch (instance.SpellWheel)
				{
				default:
					instance.SpellWheel = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.SpellWheel = SettingOptions.Off;
					break;
				}
				this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.SpellWheel));
				return;
			default:
				return;
			}
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000C748 File Offset: 0x0000A948
		public override void ControllerLeft(Controller iSender)
		{
			if (!this.mKeyboardSelection)
			{
				return;
			}
			GlobalSettings instance = GlobalSettings.Instance;
			switch (this.mSelectedPosition)
			{
			case 0:
				switch (instance.BloodAndGore)
				{
				default:
					instance.BloodAndGore = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.BloodAndGore = SettingOptions.Off;
					break;
				}
				this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.BloodAndGore));
				return;
			case 1:
				switch (instance.DamageNumbers)
				{
				default:
					instance.DamageNumbers = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.DamageNumbers = SettingOptions.Off;
					break;
				}
				this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.DamageNumbers));
				return;
			case 2:
				switch (instance.HealthBars)
				{
				default:
					instance.HealthBars = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.HealthBars = SettingOptions.Players_Only;
					break;
				case SettingOptions.Players_Only:
					instance.HealthBars = SettingOptions.Off;
					break;
				}
				this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.HealthBars));
				return;
			case 3:
				switch (instance.SpellWheel)
				{
				default:
					instance.SpellWheel = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.SpellWheel = SettingOptions.Off;
					break;
				}
				this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.SpellWheel));
				return;
			default:
				return;
			}
		}

		// Token: 0x0400017B RID: 379
		private static readonly int LOC_GAME_OPTS = "#menu_opt_12".GetHashCodeCustom();

		// Token: 0x0400017C RID: 380
		private static readonly int LOC_GORE = "#menu_opt_game_01".GetHashCodeCustom();

		// Token: 0x0400017D RID: 381
		private static readonly int LOC_DAMNUMBERS = "#menu_opt_game_02".GetHashCodeCustom();

		// Token: 0x0400017E RID: 382
		private static readonly int LOC_HBARS = "#menu_opt_game_03".GetHashCodeCustom();

		// Token: 0x0400017F RID: 383
		private static readonly int LOC_SPELLWHEEL = "#menu_opt_game_04".GetHashCodeCustom();

		// Token: 0x04000180 RID: 384
		private static readonly int LOC_LANGUAGE = "#menu_opt_game_05".GetHashCodeCustom();

		// Token: 0x04000181 RID: 385
		private static SubMenuOptionsGame mSingelton;

		// Token: 0x04000182 RID: 386
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04000183 RID: 387
		private readonly int mSampleHash = "misc_gib".GetHashCodeCustom();

		// Token: 0x04000184 RID: 388
		private List<MenuTextItem> mMenuOptions;

		// Token: 0x04000185 RID: 389
		private SettingOptions mBloodAndGore;

		// Token: 0x04000186 RID: 390
		private SettingOptions mDamageNumbers;

		// Token: 0x04000187 RID: 391
		private SettingOptions mHealthBars;

		// Token: 0x04000188 RID: 392
		private SettingOptions mSpellWheel;

		// Token: 0x04000189 RID: 393
		private GlobalSettings mGlobalSettings = GlobalSettings.Instance;
	}
}
