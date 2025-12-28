using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x020003FB RID: 1019
	internal class InGameMenuOptionsGraphics : InGameMenu
	{
		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x06001F62 RID: 8034 RVA: 0x000DCF64 File Offset: 0x000DB164
		public static InGameMenuOptionsGraphics Instance
		{
			get
			{
				if (InGameMenuOptionsGraphics.sSingelton == null)
				{
					lock (InGameMenuOptionsGraphics.sSingeltonLock)
					{
						if (InGameMenuOptionsGraphics.sSingelton == null)
						{
							InGameMenuOptionsGraphics.sSingelton = new InGameMenuOptionsGraphics();
						}
					}
				}
				return InGameMenuOptionsGraphics.sSingelton;
			}
		}

		// Token: 0x06001F63 RID: 8035 RVA: 0x000DCFB8 File Offset: 0x000DB1B8
		private InGameMenuOptionsGraphics()
		{
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.mOptions = new List<MenuTextItem>();
			this.mOptions.Add(new MenuTextItem(InGameMenu.GetOnOffLoc(!this.mGlobalSettings.Fullscreen), default(Vector2), font, TextAlign.Left));
			this.AddMenuTextItem("#menu_opt_gfx_01".GetHashCodeCustom(), font, TextAlign.Right);
			this.mOptions.Add(new MenuTextItem("1280 x 720 - 60Hz", default(Vector2), font, TextAlign.Left));
			this.AddMenuTextItem("#menu_opt_gfx_02".GetHashCodeCustom(), font, TextAlign.Right);
			this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), font, TextAlign.Center);
			this.mBackgroundSize = new Vector2(500f, 150f);
		}

		// Token: 0x06001F64 RID: 8036 RVA: 0x000DD088 File Offset: 0x000DB288
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			for (int i = 0; i < this.mOptions.Count; i++)
			{
				this.mOptions[i].LanguageChanged();
			}
		}

		// Token: 0x06001F65 RID: 8037 RVA: 0x000DD0C4 File Offset: 0x000DB2C4
		public override void UpdatePositions()
		{
			Vector2 position = default(Vector2);
			position.X = InGameMenu.sScreenSize.X * 0.5f;
			position.Y = InGameMenu.sScreenSize.Y * 0.5f - 25f * InGameMenu.sScale;
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
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
			this.mBackgroundSize = new Vector2(500f, 150f);
		}

		// Token: 0x06001F66 RID: 8038 RVA: 0x000DD25C File Offset: 0x000DB45C
		protected override void IControllerSelect(Controller iSender)
		{
			switch (this.mSelectedItem)
			{
			case 0:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				this.mGlobalSettings.Fullscreen = !this.mGlobalSettings.Fullscreen;
				if (this.mGlobalSettings.Fullscreen)
				{
					this.mOptions[0].SetText("#menu_opt_alt_02".GetHashCodeCustom());
					return;
				}
				this.mOptions[0].SetText("#menu_opt_alt_01".GetHashCodeCustom());
				return;
			case 1:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				InGameMenu.PushMenu(InGameMenuOptionsResolution.Instance);
				return;
			case 2:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
				InGameMenu.PopMenu();
				return;
			default:
				return;
			}
		}

		// Token: 0x06001F67 RID: 8039 RVA: 0x000DD324 File Offset: 0x000DB524
		protected override string IGetHighlightedButtonName()
		{
			return InGameMenuOptionsGraphics.OPTION_STRINGS[this.mSelectedItem];
		}

		// Token: 0x06001F68 RID: 8040 RVA: 0x000DD332 File Offset: 0x000DB532
		protected override void IControllerBack(Controller iSender)
		{
			AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
			InGameMenu.PopMenu();
		}

		// Token: 0x06001F69 RID: 8041 RVA: 0x000DD34C File Offset: 0x000DB54C
		protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
		{
			base.IControllerMove(iSender, iDirection);
			if ((byte)(iDirection & (ControllerDirection.Right | ControllerDirection.Left)) != 0 & this.mSelectedItem == 0)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				this.mGlobalSettings.Fullscreen = !this.mGlobalSettings.Fullscreen;
				if (this.mGlobalSettings.Fullscreen)
				{
					this.mOptions[0].SetText(LanguageManager.Instance.GetString("#menu_opt_alt_02".GetHashCodeCustom()));
					return;
				}
				this.mOptions[0].SetText(LanguageManager.Instance.GetString("#menu_opt_alt_01".GetHashCodeCustom()));
			}
		}

		// Token: 0x06001F6A RID: 8042 RVA: 0x000DD3FC File Offset: 0x000DB5FC
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
			ResolutionData resolution = GlobalSettings.Instance.Resolution;
			string text = string.Format("{0} x {1}", resolution.Width, resolution.Height);
			this.mOptions[1].SetText(text);
			this.mWindowed = this.mGlobalSettings.Fullscreen;
		}

		// Token: 0x06001F6B RID: 8043 RVA: 0x000DD478 File Offset: 0x000DB678
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			base.IDraw(iDeltaTime, ref iBackgroundSize);
			this.UpdatePositions();
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

		// Token: 0x06001F6C RID: 8044 RVA: 0x000DD541 File Offset: 0x000DB741
		protected override void OnExit()
		{
			if (this.mWindowed != this.mGlobalSettings.Fullscreen)
			{
				SaveManager.Instance.SaveSettings();
			}
		}

		// Token: 0x040021D9 RID: 8665
		private const string OPTION_WINDOWED = "windowed";

		// Token: 0x040021DA RID: 8666
		private const string OPTION_RESOLUTION = "resolution";

		// Token: 0x040021DB RID: 8667
		private const string OPTION_BACK = "back";

		// Token: 0x040021DC RID: 8668
		private static readonly string[] OPTION_STRINGS = new string[]
		{
			"windowed",
			"resolution",
			"back"
		};

		// Token: 0x040021DD RID: 8669
		private static InGameMenuOptionsGraphics sSingelton;

		// Token: 0x040021DE RID: 8670
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040021DF RID: 8671
		private List<MenuTextItem> mOptions;

		// Token: 0x040021E0 RID: 8672
		private GlobalSettings mGlobalSettings = GlobalSettings.Instance;

		// Token: 0x040021E1 RID: 8673
		private bool mWindowed;
	}
}
