using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu.Main.Options;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.WebTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x020003A4 RID: 932
	internal class SubMenuOptions : SubMenu
	{
		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x06001C93 RID: 7315 RVA: 0x000C6594 File Offset: 0x000C4794
		public static SubMenuOptions Instance
		{
			get
			{
				if (SubMenuOptions.mSingelton == null)
				{
					lock (SubMenuOptions.mSingeltonLock)
					{
						if (SubMenuOptions.mSingelton == null)
						{
							SubMenuOptions.mSingelton = new SubMenuOptions();
						}
					}
				}
				return SubMenuOptions.mSingelton;
			}
		}

		// Token: 0x06001C94 RID: 7316 RVA: 0x000C65E8 File Offset: 0x000C47E8
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptions.LOC_MENU_MAIN_06));
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x000C660C File Offset: 0x000C480C
		private SubMenuOptions()
		{
			this.mSelectedPosition = 0;
			this.mMenuItems = new List<MenuItem>();
			this.mMenuTitle = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptions.LOC_MENU_MAIN_06));
			this.AddMenuTextItem(SubMenuOptions.LOC_GAME);
			this.AddMenuTextItem(SubMenuOptions.LOC_SOUND);
			this.AddMenuTextItem(SubMenuOptions.LOC_GRAPHICS);
			this.AddMenuTextItem(SubMenuOptions.LOC_CONTROLS);
			this.AddMenuTextItem(SubMenuOptions.LOC_PARADOX_ACCOUNT);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x000C66E8 File Offset: 0x000C48E8
		public override void ControllerUp(Controller iSender)
		{
			if (!this.mKeyboardSelection)
			{
				this.mSelectedPosition = 0;
				this.mKeyboardSelection = true;
				return;
			}
			this.mSelectedPosition--;
			if (this.mSelectedPosition < 0)
			{
				this.mSelectedPosition = this.mMenuItems.Count - 1;
			}
		}

		// Token: 0x06001C97 RID: 7319 RVA: 0x000C6738 File Offset: 0x000C4938
		public override void ControllerDown(Controller iSender)
		{
			if (!this.mKeyboardSelection)
			{
				this.mSelectedPosition = 0;
				this.mKeyboardSelection = true;
				return;
			}
			this.mSelectedPosition++;
			if (this.mSelectedPosition >= this.mMenuItems.Count)
			{
				this.mSelectedPosition = 0;
			}
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x000C6784 File Offset: 0x000C4984
		public override void ControllerA(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 0:
				Tome.Instance.PushMenu(SubMenuOptionsGame.Instance, 1);
				return;
			case 1:
				Tome.Instance.PushMenu(SubMenuOptionsSound.Instance, 3);
				return;
			case 2:
				Tome.Instance.PushMenu(SubMenuOptionsGraphics.Instance, 4);
				return;
			case 3:
				Tome.Instance.PushMenu(SubMenuOptionsControls.Instance, 1);
				return;
			case 4:
				Tome.Instance.PushMenu(SubMenuOptionsParadoxAccount.Instance, 1);
				return;
			case 5:
				Tome.Instance.PopMenu();
				return;
			default:
				return;
			}
		}

		// Token: 0x06001C99 RID: 7321 RVA: 0x000C6818 File Offset: 0x000C4A18
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			base.Draw(iLeftSide, iRightSide);
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			base.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48), new Rectangle(208, 220, 608, 48));
			base.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 768, 496, 208), new Rectangle(264, 620, 496, 208));
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x000C68F0 File Offset: 0x000C4AF0
		public override void OnEnter()
		{
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, GamePadMenuHelp.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			this.mMenuItems[4].Enabled = Singleton<ParadoxAccount>.Instance.IsLoggedFull;
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x000C694E File Offset: 0x000C4B4E
		public override void OnExit()
		{
		}

		// Token: 0x04001EDF RID: 7903
		private static readonly int LOC_GAME = "#menu_opt_01".GetHashCodeCustom();

		// Token: 0x04001EE0 RID: 7904
		private static readonly int LOC_CONTROLS = "#menu_opt_02".GetHashCodeCustom();

		// Token: 0x04001EE1 RID: 7905
		private static readonly int LOC_SOUND = "#menu_opt_03".GetHashCodeCustom();

		// Token: 0x04001EE2 RID: 7906
		private static readonly int LOC_GRAPHICS = "#menu_opt_04".GetHashCodeCustom();

		// Token: 0x04001EE3 RID: 7907
		private static readonly int LOC_MENU_MAIN_06 = "#menu_main_06".GetHashCodeCustom();

		// Token: 0x04001EE4 RID: 7908
		private static readonly int LOC_PARADOX_ACCOUNT = "#paradox_account".GetHashCodeCustom();

		// Token: 0x04001EE5 RID: 7909
		private static SubMenuOptions mSingelton;

		// Token: 0x04001EE6 RID: 7910
		private static volatile object mSingeltonLock = new object();

		// Token: 0x020003A5 RID: 933
		internal enum MenuItemId
		{
			// Token: 0x04001EE8 RID: 7912
			Game,
			// Token: 0x04001EE9 RID: 7913
			Sound,
			// Token: 0x04001EEA RID: 7914
			Graphics,
			// Token: 0x04001EEB RID: 7915
			Controls,
			// Token: 0x04001EEC RID: 7916
			Paradox,
			// Token: 0x04001EED RID: 7917
			Back
		}
	}
}
