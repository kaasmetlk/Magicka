using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x02000436 RID: 1078
	internal class InGameMenuOptionsGame : InGameMenu
	{
		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x06002164 RID: 8548 RVA: 0x000EDF54 File Offset: 0x000EC154
		public static InGameMenuOptionsGame Instance
		{
			get
			{
				if (InGameMenuOptionsGame.sSingelton == null)
				{
					lock (InGameMenuOptionsGame.sSingeltonLock)
					{
						if (InGameMenuOptionsGame.sSingelton == null)
						{
							InGameMenuOptionsGame.sSingelton = new InGameMenuOptionsGame();
						}
					}
				}
				return InGameMenuOptionsGame.sSingelton;
			}
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x000EDFA8 File Offset: 0x000EC1A8
		private InGameMenuOptionsGame()
		{
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.mOptions = new List<MenuTextItem>();
			this.mOptions.Add(new MenuTextItem(InGameMenu.GetSettingLoc(this.mGlobalSettings.BloodAndGore), default(Vector2), font, TextAlign.Left));
			this.AddMenuTextItem("#menu_opt_game_01".GetHashCodeCustom(), font, TextAlign.Right);
			this.mOptions.Add(new MenuTextItem(InGameMenu.GetSettingLoc(this.mGlobalSettings.DamageNumbers), default(Vector2), font, TextAlign.Left));
			this.AddMenuTextItem("#menu_opt_game_02".GetHashCodeCustom(), font, TextAlign.Right);
			this.mOptions.Add(new MenuTextItem(InGameMenu.GetSettingLoc(this.mGlobalSettings.HealthBars), default(Vector2), font, TextAlign.Left));
			this.AddMenuTextItem("#menu_opt_game_03".GetHashCodeCustom(), font, TextAlign.Right);
			this.mOptions.Add(new MenuTextItem(InGameMenu.GetSettingLoc(this.mGlobalSettings.SpellWheel), default(Vector2), font, TextAlign.Left));
			this.AddMenuTextItem("#menu_opt_game_04".GetHashCodeCustom(), font, TextAlign.Right);
			this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), font, TextAlign.Center);
			this.mBackgroundSize = new Vector2(600f, 400f);
		}

		// Token: 0x06002166 RID: 8550 RVA: 0x000EE0FC File Offset: 0x000EC2FC
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			for (int i = 0; i < this.mOptions.Count; i++)
			{
				this.mOptions[i].LanguageChanged();
			}
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x000EE138 File Offset: 0x000EC338
		public override void UpdatePositions()
		{
			Vector2 position = default(Vector2);
			position.X = InGameMenu.sScreenSize.X * 0.5f + 80f;
			position.Y = 290f * InGameMenu.sScale;
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				if (i == this.mMenuItems.Count - 1)
				{
					position.X -= 80f;
				}
				MenuItem menuItem = this.mMenuItems[i];
				menuItem.Scale = InGameMenu.sScale;
				menuItem.Position = position;
				position.Y += menuItem.BottomRight.Y - menuItem.TopLeft.Y;
			}
			for (int j = 0; j < this.mOptions.Count; j++)
			{
				position = this.mMenuItems[j].Position;
				position.X -= 15f * InGameMenu.sScale;
				this.mMenuItems[j].Position = position;
				position.X += 30f * InGameMenu.sScale;
				this.mOptions[j].Position = position;
				this.mOptions[j].Scale = InGameMenu.sScale;
			}
			this.mMenuItems[this.mMenuItems.Count - 1].Position += new Vector2(0f, 10f * InGameMenu.sScale);
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x000EE2D0 File Offset: 0x000EC4D0
		protected override void IControllerSelect(Controller iSender)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			switch (this.mSelectedItem)
			{
			case 0:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				switch (instance.BloodAndGore)
				{
				default:
					instance.BloodAndGore = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.BloodAndGore = SettingOptions.Off;
					break;
				}
				this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.BloodAndGore));
				return;
			case 1:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				switch (instance.DamageNumbers)
				{
				default:
					instance.DamageNumbers = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.DamageNumbers = SettingOptions.Off;
					break;
				}
				this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.DamageNumbers));
				return;
			case 2:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
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
				this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.HealthBars));
				return;
			case 3:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				switch (instance.SpellWheel)
				{
				default:
					instance.SpellWheel = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.SpellWheel = SettingOptions.Off;
					break;
				}
				this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.SpellWheel));
				return;
			case 4:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
				InGameMenu.PopMenu();
				return;
			default:
				return;
			}
		}

		// Token: 0x06002169 RID: 8553 RVA: 0x000EE48C File Offset: 0x000EC68C
		protected override void IControllerBack(Controller iSender)
		{
			AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
			InGameMenu.PopMenu();
		}

		// Token: 0x0600216A RID: 8554 RVA: 0x000EE4A4 File Offset: 0x000EC6A4
		protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
		{
			base.IControllerMove(iSender, iDirection);
			GlobalSettings instance = GlobalSettings.Instance;
			if (iDirection != ControllerDirection.Right)
			{
				if (iDirection == ControllerDirection.Left)
				{
					switch (this.mSelectedItem)
					{
					case 0:
						AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
						switch (instance.BloodAndGore)
						{
						default:
							instance.BloodAndGore = SettingOptions.On;
							break;
						case SettingOptions.On:
							instance.BloodAndGore = SettingOptions.Off;
							break;
						}
						this.mOptions[this.mSelectedItem].SetText(instance.BloodAndGore.ToString());
						return;
					case 1:
						AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
						switch (instance.DamageNumbers)
						{
						default:
							instance.DamageNumbers = SettingOptions.On;
							break;
						case SettingOptions.On:
							instance.DamageNumbers = SettingOptions.Off;
							break;
						}
						this.mOptions[this.mSelectedItem].SetText(instance.DamageNumbers.ToString());
						return;
					case 2:
						AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
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
						this.mOptions[this.mSelectedItem].SetText(instance.HealthBars.ToString());
						return;
					case 3:
						AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
						switch (instance.SpellWheel)
						{
						default:
							instance.SpellWheel = SettingOptions.On;
							break;
						case SettingOptions.On:
							instance.SpellWheel = SettingOptions.Off;
							break;
						}
						this.mOptions[this.mSelectedItem].SetText(instance.SpellWheel.ToString());
						break;
					default:
						return;
					}
				}
				return;
			}
			switch (this.mSelectedItem)
			{
			case 0:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				switch (instance.BloodAndGore)
				{
				default:
					instance.BloodAndGore = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.BloodAndGore = SettingOptions.Off;
					break;
				}
				this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.BloodAndGore));
				return;
			case 1:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				switch (instance.DamageNumbers)
				{
				default:
					instance.DamageNumbers = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.DamageNumbers = SettingOptions.Off;
					break;
				}
				this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.DamageNumbers));
				return;
			case 2:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
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
				this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.HealthBars));
				return;
			case 3:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				switch (instance.SpellWheel)
				{
				default:
					instance.SpellWheel = SettingOptions.On;
					break;
				case SettingOptions.On:
					instance.SpellWheel = SettingOptions.Off;
					break;
				}
				this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.SpellWheel));
				return;
			default:
				return;
			}
		}

		// Token: 0x0600216B RID: 8555 RVA: 0x000EE804 File Offset: 0x000ECA04
		protected override string IGetHighlightedButtonName()
		{
			return InGameMenuOptionsGame.OPTION_STRINGS[this.mSelectedItem];
		}

		// Token: 0x0600216C RID: 8556 RVA: 0x000EE814 File Offset: 0x000ECA14
		protected override void OnEnter()
		{
			if (InGameMenu.sController is KeyboardMouseController)
			{
				this.mSelectedItem = -1;
			}
			else
			{
				this.mSelectedItem = 0;
			}
			this.mBloodAndGore = this.mGlobalSettings.BloodAndGore;
			this.mDamageNumbers = this.mGlobalSettings.DamageNumbers;
			this.mHealthBars = this.mGlobalSettings.HealthBars;
			this.mSpellWheel = this.mGlobalSettings.SpellWheel;
		}

		// Token: 0x0600216D RID: 8557 RVA: 0x000EE884 File Offset: 0x000ECA84
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			base.IDraw(iDeltaTime, ref iBackgroundSize);
			Vector4 color = this.mMenuItems[0].Color;
			Vector4 colorSelected = this.mMenuItems[0].ColorSelected;
			Vector4 colorDisabled = this.mMenuItems[0].ColorDisabled;
			for (int i = 0; i < this.mOptions.Count; i++)
			{
				MenuItem menuItem = this.mOptions[i];
				menuItem.Color = color;
				menuItem.ColorSelected = colorSelected;
				menuItem.ColorDisabled = colorDisabled;
				menuItem.Selected = this.mMenuItems[i].Selected;
				menuItem.Enabled = this.mMenuItems[i].Enabled;
				menuItem.Draw(InGameMenu.sEffect);
			}
		}

		// Token: 0x0600216E RID: 8558 RVA: 0x000EE948 File Offset: 0x000ECB48
		protected override void OnExit()
		{
			if (this.mBloodAndGore != this.mGlobalSettings.BloodAndGore | this.mDamageNumbers != this.mGlobalSettings.DamageNumbers | this.mHealthBars != this.mGlobalSettings.HealthBars | this.mSpellWheel != this.mGlobalSettings.SpellWheel)
			{
				SaveManager.Instance.SaveSettings();
			}
		}

		// Token: 0x04002430 RID: 9264
		private const string OPTION_BLOOD_GORE = "blood_and_gore";

		// Token: 0x04002431 RID: 9265
		private const string OPTION_DAMAGE_NUMBERS = "damage_numbers";

		// Token: 0x04002432 RID: 9266
		private const string OPTION_HEALTH_BARS = "health_bars";

		// Token: 0x04002433 RID: 9267
		private const string OPTION_SPELL_WHEELS = "spell_wheels";

		// Token: 0x04002434 RID: 9268
		private const string OPTION_BACK = "back";

		// Token: 0x04002435 RID: 9269
		private static readonly string[] OPTION_STRINGS = new string[]
		{
			"blood_and_gore",
			"damage_numbers",
			"health_bars",
			"spell_wheels",
			"back"
		};

		// Token: 0x04002436 RID: 9270
		private static InGameMenuOptionsGame sSingelton;

		// Token: 0x04002437 RID: 9271
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002438 RID: 9272
		private List<MenuTextItem> mOptions;

		// Token: 0x04002439 RID: 9273
		private SettingOptions mBloodAndGore;

		// Token: 0x0400243A RID: 9274
		private SettingOptions mDamageNumbers;

		// Token: 0x0400243B RID: 9275
		private SettingOptions mHealthBars;

		// Token: 0x0400243C RID: 9276
		private SettingOptions mSpellWheel;

		// Token: 0x0400243D RID: 9277
		private GlobalSettings mGlobalSettings = GlobalSettings.Instance;
	}
}
