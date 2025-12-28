using System;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x02000033 RID: 51
	internal class InGameMenuOptions : InGameMenu
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001EA RID: 490 RVA: 0x0000DD9C File Offset: 0x0000BF9C
		public static InGameMenuOptions Instance
		{
			get
			{
				if (InGameMenuOptions.sSingelton == null)
				{
					lock (InGameMenuOptions.sSingeltonLock)
					{
						if (InGameMenuOptions.sSingelton == null)
						{
							InGameMenuOptions.sSingelton = new InGameMenuOptions();
						}
					}
				}
				return InGameMenuOptions.sSingelton;
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000DDF0 File Offset: 0x0000BFF0
		private InGameMenuOptions()
		{
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.AddMenuTextItem("#menu_opt_01".GetHashCodeCustom(), font, TextAlign.Center);
			this.AddMenuTextItem("#menu_opt_03".GetHashCodeCustom(), font, TextAlign.Center);
			this.AddMenuTextItem("#menu_opt_04".GetHashCodeCustom(), font, TextAlign.Center);
			this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), font, TextAlign.Center);
			this.mBackgroundSize = new Vector2(400f, 400f);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000DE6C File Offset: 0x0000C06C
		public override void UpdatePositions()
		{
			base.UpdatePositions();
			this.mMenuItems[this.mMenuItems.Count - 1].Position += new Vector2(0f, 10f * InGameMenu.sScale);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000DEBC File Offset: 0x0000C0BC
		protected override void IControllerSelect(Controller iSender)
		{
			switch (this.mSelectedItem)
			{
			case 0:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				InGameMenu.PushMenu(InGameMenuOptionsGame.Instance);
				return;
			case 1:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				InGameMenu.PushMenu(InGameMenuOptionsSound.Instance);
				return;
			case 2:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				InGameMenu.PushMenu(InGameMenuOptionsGraphics.Instance);
				return;
			case 3:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
				InGameMenu.PopMenu();
				return;
			default:
				return;
			}
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000DF51 File Offset: 0x0000C151
		protected override void IControllerBack(Controller iSender)
		{
			InGameMenu.PopMenu();
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000DF58 File Offset: 0x0000C158
		protected override string IGetHighlightedButtonName()
		{
			return InGameMenuOptions.OPTION_STRINGS[this.mSelectedItem];
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000DF66 File Offset: 0x0000C166
		protected override void OnEnter()
		{
			if (InGameMenu.sController is KeyboardMouseController)
			{
				this.mSelectedItem = -1;
				return;
			}
			this.mSelectedItem = 0;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000DF83 File Offset: 0x0000C183
		protected override void OnExit()
		{
		}

		// Token: 0x040001AB RID: 427
		private const string OPTION_GAME = "game";

		// Token: 0x040001AC RID: 428
		private const string OPTION_SOUND = "sound";

		// Token: 0x040001AD RID: 429
		private const string OPTION_GRAPHICS = "graphics";

		// Token: 0x040001AE RID: 430
		private const string OPTION_BACK = "back";

		// Token: 0x040001AF RID: 431
		public const float BIG_SEPARATION = 10f;

		// Token: 0x040001B0 RID: 432
		private static readonly string[] OPTION_STRINGS = new string[]
		{
			"game",
			"sound",
			"graphics",
			"back"
		};

		// Token: 0x040001B1 RID: 433
		private static InGameMenuOptions sSingelton;

		// Token: 0x040001B2 RID: 434
		private static volatile object sSingeltonLock = new object();
	}
}
