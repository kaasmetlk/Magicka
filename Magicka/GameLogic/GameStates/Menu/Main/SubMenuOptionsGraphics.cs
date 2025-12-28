using System;
using System.Collections.Generic;
using System.Windows.Forms;
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
	// Token: 0x02000340 RID: 832
	internal class SubMenuOptionsGraphics : SubMenu
	{
		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x0600195A RID: 6490 RVA: 0x000AA9A8 File Offset: 0x000A8BA8
		public static SubMenuOptionsGraphics Instance
		{
			get
			{
				if (SubMenuOptionsGraphics.mSingelton == null)
				{
					lock (SubMenuOptionsGraphics.mSingeltonLock)
					{
						if (SubMenuOptionsGraphics.mSingelton == null)
						{
							SubMenuOptionsGraphics.mSingelton = new SubMenuOptionsGraphics();
						}
					}
				}
				return SubMenuOptionsGraphics.mSingelton;
			}
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x000AA9FC File Offset: 0x000A8BFC
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			for (int i = 0; i < this.mMenuOptions.Count; i++)
			{
				this.mMenuOptions[i].LanguageChanged();
			}
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGraphics.LOC_GFX_OPTS));
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x000AAA50 File Offset: 0x000A8C50
		public SubMenuOptionsGraphics()
		{
			this.mSelectedPosition = 0;
			this.mMenuItems = new List<MenuItem>();
			this.mMenuTitle = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGraphics.LOC_GFX_OPTS));
			this.mMenuOptions = new List<MenuTextItem>();
			this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_WINDOWED);
			this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_RESOLUTION);
			this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_SHADOWS);
			this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_DECAL_LIMIT);
			this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_PARTICLES);
			this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_PARTICLELIGHTS);
			this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_VSYNC);
			if (GlobalSettings.Instance.Fullscreen)
			{
				this.AddMenuOptions(SubMenu.LOC_OFF);
			}
			else
			{
				this.AddMenuOptions(SubMenu.LOC_ON);
			}
			string iText = string.Format("{0} x {1}", GlobalSettings.Instance.Resolution.Width, GlobalSettings.Instance.Resolution.Height);
			this.AddMenuOptions(iText);
			this.AddMenuOptions(SubMenu.GetSettingLoc(GlobalSettings.Instance.ShadowQuality));
			this.AddMenuOptions(SubMenu.GetSettingLoc(GlobalSettings.Instance.DecalLimit));
			this.AddMenuOptions(SubMenu.GetSettingLoc(GlobalSettings.Instance.Particles));
			this.AddMenuOptions(SubMenu.GetSettingLoc(GlobalSettings.Instance.ParticleLights));
			this.AddMenuOptions(GlobalSettings.Instance.VSync ? SubMenu.LOC_ON : SubMenu.LOC_OFF);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
			ResolutionMessageBox.Instance.Complete += this.SetResolution;
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x000AAC38 File Offset: 0x000A8E38
		private void SetResolution(ResolutionData iData)
		{
			string text = string.Format("{0} x {1}", iData.Width, iData.Height);
			this.mMenuOptions[1].SetText(text);
			GlobalSettings.Instance.Resolution = iData;
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x000AAC88 File Offset: 0x000A8E88
		public override MenuTextItem AddMenuTextItem(int iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuItems.Count;
			MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, font, TextAlign.Right);
			this.mMenuItems.Add(menuTextItem);
			return menuTextItem;
		}

		// Token: 0x0600195F RID: 6495 RVA: 0x000AACE8 File Offset: 0x000A8EE8
		public void AddMenuOptions(string iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.X += 40f;
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuOptions.Count;
			this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left));
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x000AAD58 File Offset: 0x000A8F58
		public void AddMenuOptions(int iText)
		{
			Vector2 mPosition = this.mPosition;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			mPosition.X += 40f;
			mPosition.Y += ((float)font.LineHeight + 10f) * (float)this.mMenuOptions.Count;
			this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left));
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x000AADC8 File Offset: 0x000A8FC8
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

		// Token: 0x06001962 RID: 6498 RVA: 0x000AAE38 File Offset: 0x000A9038
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			base.Draw(iLeftSide, iRightSide);
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			base.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48), new Rectangle(208, 220, 608, 48));
			base.DrawGraphics(SubMenu.sStainsTexture, new Rectangle(0, 0, 256, 256), new Rectangle(670, 650, 256, 256));
			base.DrawGraphics(SubMenu.sStainsTexture, new Rectangle(0, 256, 128, 128), new Rectangle(120, 200, 128, 128));
			foreach (MenuTextItem menuTextItem in this.mMenuOptions)
			{
				menuTextItem.Draw(this.mEffect);
			}
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06001963 RID: 6499 RVA: 0x000AAF88 File Offset: 0x000A9188
		public override void ControllerA(Controller iSender)
		{
			int mSelectedPosition = this.mSelectedPosition;
			switch (mSelectedPosition)
			{
			case 0:
				break;
			case 1:
				ResolutionMessageBox.Instance.Show();
				return;
			default:
				if (mSelectedPosition == 7)
				{
					Tome.Instance.PopMenu();
					return;
				}
				break;
			}
			this.ControllerRight(iSender);
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x000AAFCC File Offset: 0x000A91CC
		public override void ControllerRight(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 0:
			{
				GlobalSettings.Instance.Fullscreen = !GlobalSettings.Instance.Fullscreen;
				if (GlobalSettings.Instance.Fullscreen)
				{
					this.mMenuOptions[0].SetText(LanguageManager.Instance.GetString(SubMenu.LOC_OFF));
					ResolutionData resolution = GlobalSettings.Instance.Resolution;
					resolution.Width = Screen.PrimaryScreen.Bounds.Width;
					resolution.Height = Screen.PrimaryScreen.Bounds.Height;
					Tome.ChangeResolution(resolution);
					return;
				}
				this.mMenuOptions[0].SetText(LanguageManager.Instance.GetString(SubMenu.LOC_ON));
				Tome.ChangeResolution(GlobalSettings.Instance.Resolution);
				int height = RenderManager.Instance.GraphicsDevice.DisplayMode.Height;
				return;
			}
			case 1:
				break;
			case 2:
				switch (GlobalSettings.Instance.ShadowQuality)
				{
				default:
					GlobalSettings.Instance.ShadowQuality = SettingOptions.Medium;
					break;
				case SettingOptions.Medium:
					GlobalSettings.Instance.ShadowQuality = SettingOptions.High;
					break;
				case SettingOptions.High:
					GlobalSettings.Instance.ShadowQuality = SettingOptions.Low;
					break;
				}
				this.mMenuOptions[2].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.ShadowQuality));
				return;
			case 3:
				switch (GlobalSettings.Instance.DecalLimit)
				{
				default:
					GlobalSettings.Instance.DecalLimit = SettingOptions.Medium;
					break;
				case SettingOptions.Medium:
					GlobalSettings.Instance.DecalLimit = SettingOptions.High;
					break;
				case SettingOptions.High:
					GlobalSettings.Instance.DecalLimit = SettingOptions.Low;
					break;
				}
				this.mMenuOptions[3].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.DecalLimit));
				return;
			case 4:
				switch (GlobalSettings.Instance.Particles)
				{
				default:
					GlobalSettings.Instance.Particles = SettingOptions.Medium;
					break;
				case SettingOptions.Medium:
					GlobalSettings.Instance.Particles = SettingOptions.High;
					break;
				case SettingOptions.High:
					GlobalSettings.Instance.Particles = SettingOptions.Low;
					break;
				}
				this.mMenuOptions[4].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.Particles));
				return;
			case 5:
				GlobalSettings.Instance.ParticleLights = !GlobalSettings.Instance.ParticleLights;
				this.mMenuOptions[5].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.ParticleLights));
				return;
			case 6:
				GlobalSettings.Instance.VSync = !GlobalSettings.Instance.VSync;
				SaveManager.Instance.SaveSettings();
				this.mMenuOptions[6].SetText(GlobalSettings.Instance.VSync ? SubMenu.LOC_ON : SubMenu.LOC_OFF);
				break;
			default:
				return;
			}
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x000AB28C File Offset: 0x000A948C
		public override void ControllerLeft(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 0:
				GlobalSettings.Instance.Fullscreen = !GlobalSettings.Instance.Fullscreen;
				if (GlobalSettings.Instance.Fullscreen)
				{
					this.mMenuOptions[0].SetText(LanguageManager.Instance.GetString(SubMenu.LOC_OFF));
					return;
				}
				this.mMenuOptions[0].SetText(LanguageManager.Instance.GetString(SubMenu.LOC_ON));
				return;
			case 1:
				break;
			case 2:
				switch (GlobalSettings.Instance.ShadowQuality)
				{
				case SettingOptions.Low:
					GlobalSettings.Instance.ShadowQuality = SettingOptions.High;
					break;
				case SettingOptions.Medium:
					GlobalSettings.Instance.ShadowQuality = SettingOptions.Low;
					break;
				default:
					GlobalSettings.Instance.ShadowQuality = SettingOptions.Medium;
					break;
				}
				this.mMenuOptions[2].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.ShadowQuality));
				return;
			case 3:
				switch (GlobalSettings.Instance.DecalLimit)
				{
				default:
					GlobalSettings.Instance.DecalLimit = SettingOptions.High;
					break;
				case SettingOptions.Low:
					GlobalSettings.Instance.DecalLimit = SettingOptions.Off;
					break;
				case SettingOptions.Medium:
					GlobalSettings.Instance.DecalLimit = SettingOptions.Low;
					break;
				case SettingOptions.High:
					GlobalSettings.Instance.DecalLimit = SettingOptions.Medium;
					break;
				}
				this.mMenuOptions[3].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.DecalLimit));
				return;
			case 4:
				switch (GlobalSettings.Instance.Particles)
				{
				default:
					GlobalSettings.Instance.Particles = SettingOptions.High;
					break;
				case SettingOptions.Medium:
					GlobalSettings.Instance.Particles = SettingOptions.Low;
					break;
				case SettingOptions.High:
					GlobalSettings.Instance.Particles = SettingOptions.Medium;
					break;
				}
				this.mMenuOptions[4].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.Particles));
				return;
			case 5:
				GlobalSettings.Instance.ParticleLights = !GlobalSettings.Instance.ParticleLights;
				this.mMenuOptions[5].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.ParticleLights));
				return;
			case 6:
				Game.Instance.GraphicsManager.SynchronizeWithVerticalRetrace = !Game.Instance.GraphicsManager.SynchronizeWithVerticalRetrace;
				this.mMenuOptions[6].SetText(Game.Instance.GraphicsManager.SynchronizeWithVerticalRetrace ? SubMenu.LOC_OFF : SubMenu.LOC_ON);
				break;
			default:
				return;
			}
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x000AB4F8 File Offset: 0x000A96F8
		public override void OnEnter()
		{
			string text = string.Format("{0} x {1}", RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
			this.mMenuOptions[1].SetText(text);
			this.mShadowQuality = GlobalSettings.Instance.ShadowQuality;
			this.mDecalLimit = GlobalSettings.Instance.DecalLimit;
			this.mWindowed = GlobalSettings.Instance.Fullscreen;
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x000AB5B0 File Offset: 0x000A97B0
		public override void OnExit()
		{
			base.OnExit();
			if (this.mWindowed != GlobalSettings.Instance.Fullscreen | this.mDecalLimit != GlobalSettings.Instance.DecalLimit | this.mShadowQuality != GlobalSettings.Instance.ShadowQuality)
			{
				SaveManager.Instance.SaveSettings();
			}
		}

		// Token: 0x06001968 RID: 6504 RVA: 0x000AB610 File Offset: 0x000A9810
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
						if (iState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iOldState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
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

		// Token: 0x06001969 RID: 6505 RVA: 0x000AB6E0 File Offset: 0x000A98E0
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

		// Token: 0x04001B8A RID: 7050
		private static readonly int LOC_WINDOWED = "#menu_opt_gfx_01".GetHashCodeCustom();

		// Token: 0x04001B8B RID: 7051
		private static readonly int LOC_RESOLUTION = "#menu_opt_gfx_02".GetHashCodeCustom();

		// Token: 0x04001B8C RID: 7052
		private static readonly int LOC_REFRESH_RATE = "#menu_opt_gfx_03".GetHashCodeCustom();

		// Token: 0x04001B8D RID: 7053
		private static readonly int LOC_SHADOWS = "#menu_opt_gfx_04".GetHashCodeCustom();

		// Token: 0x04001B8E RID: 7054
		private static readonly int LOC_BLOOM = "#menu_opt_gfx_05".GetHashCodeCustom();

		// Token: 0x04001B8F RID: 7055
		private static readonly int LOC_AMBIENT_OCCLUSION = "#menu_opt_gfx_06".GetHashCodeCustom();

		// Token: 0x04001B90 RID: 7056
		private static readonly int LOC_OVERSCAN_COMPENSATION = "#menu_opt_gfx_07".GetHashCodeCustom();

		// Token: 0x04001B91 RID: 7057
		private static readonly int LOC_BRIGHTNESS = "#menu_opt_gfx_08".GetHashCodeCustom();

		// Token: 0x04001B92 RID: 7058
		private static readonly int LOC_ENVIRONMENT_DYNAMICS = "#menu_opt_gfx_09".GetHashCodeCustom();

		// Token: 0x04001B93 RID: 7059
		private static readonly int LOC_RESOLUTIONS = "#menu_opt_gfx_10".GetHashCodeCustom();

		// Token: 0x04001B94 RID: 7060
		private static readonly int LOC_WIDTH = "#menu_opt_gfx_11".GetHashCodeCustom();

		// Token: 0x04001B95 RID: 7061
		private static readonly int LOC_HEIGHT = "#menu_opt_gfx_12".GetHashCodeCustom();

		// Token: 0x04001B96 RID: 7062
		private static readonly int LOC_DECAL_LIMIT = "#menu_opt_gfx_13".GetHashCodeCustom();

		// Token: 0x04001B97 RID: 7063
		private static readonly int LOC_PARTICLES = "#menu_opt_gfx_particles".GetHashCodeCustom();

		// Token: 0x04001B98 RID: 7064
		private static readonly int LOC_PARTICLELIGHTS = "#menu_opt_gfx_particlelights".GetHashCodeCustom();

		// Token: 0x04001B99 RID: 7065
		private static readonly int LOC_VSYNC = "#menu_vsync".GetHashCodeCustom();

		// Token: 0x04001B9A RID: 7066
		private static readonly int LOC_GFX_OPTS = "#menu_opt_10".GetHashCodeCustom();

		// Token: 0x04001B9B RID: 7067
		private static SubMenuOptionsGraphics mSingelton;

		// Token: 0x04001B9C RID: 7068
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04001B9D RID: 7069
		private List<MenuTextItem> mMenuOptions;

		// Token: 0x04001B9E RID: 7070
		private bool mWindowed;

		// Token: 0x04001B9F RID: 7071
		private SettingOptions mShadowQuality;

		// Token: 0x04001BA0 RID: 7072
		private SettingOptions mDecalLimit;
	}
}
