using System;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x0200029F RID: 671
	internal class InGameMenuMain : InGameMenu
	{
		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x0600141C RID: 5148 RVA: 0x0007D93C File Offset: 0x0007BB3C
		public static InGameMenuMain Instance
		{
			get
			{
				if (InGameMenuMain.sSingelton == null)
				{
					lock (InGameMenuMain.sSingeltonLock)
					{
						if (InGameMenuMain.sSingelton == null)
						{
							InGameMenuMain.sSingelton = new InGameMenuMain();
						}
					}
				}
				return InGameMenuMain.sSingelton;
			}
		}

		// Token: 0x0600141D RID: 5149 RVA: 0x0007D990 File Offset: 0x0007BB90
		private InGameMenuMain()
		{
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.AddMenuTextItem("#banana".GetHashCodeCustom(), font, TextAlign.Center);
			this.AddMenuTextItem("#menu_resume".GetHashCodeCustom(), font, TextAlign.Center);
			this.AddMenuTextItem(Defines.LOC_GEN_RESTART, font, TextAlign.Center);
			this.AddMenuTextItem("#menu_tome_02".GetHashCodeCustom(), font, TextAlign.Center);
			this.AddMenuTextItem("#menu_main_06".GetHashCodeCustom(), font, TextAlign.Center);
			this.AddMenuTextItem("#menu_main_07".GetHashCodeCustom(), font, TextAlign.Center);
			this.mBackgroundSize = new Vector2(400f, 400f);
			this.mRestartMenu = new InGameMenuAreYouSure(new Action(this.RestartCallback), new int[]
			{
				"#add_menu_rus".GetHashCodeCustom(),
				"#add_menu_rus_quitlev".GetHashCodeCustom()
			});
			this.mQuitMenu = new InGameMenuAreYouSure(new Action(this.QuitCallback), new int[]
			{
				"#add_menu_rus".GetHashCodeCustom(),
				"#add_menu_rus_quitlev".GetHashCodeCustom()
			});
			this.mTutorialSkipMenu = new InGameMenuAreYouSure(new Action(this.TutorialSkipCallback), new int[]
			{
				"#banana_sure".GetHashCodeCustom()
			});
			this.mDisconnectMenu = new InGameMenuAreYouSure(new Action(this.DisconnetCallback), new int[]
			{
				"#add_menu_rus".GetHashCodeCustom()
			});
			this.mSkipCreditsMenu = new InGameMenuAreYouSure(new Action(this.SkipCreditsCallback), new int[]
			{
				"#add_menu_rus".GetHashCodeCustom()
			});
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x0007DB28 File Offset: 0x0007BD28
		public override void UpdatePositions()
		{
			Vector2 position = default(Vector2);
			position.X = InGameMenu.sScreenSize.X * 0.5f;
			position.Y = 290f * InGameMenu.sScale;
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				if (i != 0 || InGameMenu.sPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH)
				{
					MenuItem menuItem = this.mMenuItems[i];
					menuItem.Scale = InGameMenu.sScale;
					menuItem.Position = position;
					position.Y += menuItem.BottomRight.Y - menuItem.TopLeft.Y;
				}
			}
			this.mRestartMenu.UpdatePositions();
			this.mQuitMenu.UpdatePositions();
			this.mTutorialSkipMenu.UpdatePositions();
			this.mDisconnectMenu.UpdatePositions();
			this.mSkipCreditsMenu.UpdatePositions();
		}

		// Token: 0x0600141F RID: 5151 RVA: 0x0007DC18 File Offset: 0x0007BE18
		private void QuitCallback()
		{
			GameEndMessage gameEndMessage;
			gameEndMessage.Condition = EndGameCondition.ChallengeExit;
			gameEndMessage.Argument = 1;
			gameEndMessage.Phony = false;
			gameEndMessage.DelayTime = 0f;
			InGameMenu.sPlayState.Endgame(ref gameEndMessage);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				(NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
				NetworkManager.Instance.Interface.SendMessage<GameEndMessage>(ref gameEndMessage);
			}
			InGameMenu.HideInstant();
			if (TutorialUtils.IsInProgress)
			{
				TutorialUtils.Quit();
			}
		}

		// Token: 0x06001420 RID: 5152 RVA: 0x0007DC98 File Offset: 0x0007BE98
		private void RestartCallback()
		{
			InGameMenu.sPlayState.Restart(this, RestartType.StartOfLevel);
			InGameMenu.HideInstant();
			if (TutorialUtils.IsInProgress)
			{
				TutorialUtils.Restart();
			}
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x0007DCB7 File Offset: 0x0007BEB7
		private void TutorialSkipCallback()
		{
			InGameMenu.Hide();
			InGameMenu.sPlayState.Level.CurrentScene.ExecuteTrigger("skip_tutorial".GetHashCodeCustom(), null, true);
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x0007DCE0 File Offset: 0x0007BEE0
		private void DisconnetCallback()
		{
			NetworkManager.Instance.EndSession();
			while (!(Tome.Instance.CurrentMenu is SubMenuOnline))
			{
				Tome.Instance.PopMenuInstant();
			}
			if (Credits.Instance.IsActive)
			{
				InGameMenu.sPlayState.Endgame(EndGameCondition.LevelComplete, false, false, 0f);
			}
			else
			{
				InGameMenu.sPlayState.Endgame(EndGameCondition.Disconnected, false, false, 0f);
			}
			GameStateManager.Instance.PopState();
			InGameMenu.HideInstant();
		}

		// Token: 0x06001423 RID: 5155 RVA: 0x0007DD58 File Offset: 0x0007BF58
		private void SkipCreditsCallback()
		{
			GameEndMessage gameEndMessage;
			gameEndMessage.Condition = EndGameCondition.ChallengeExit;
			gameEndMessage.Argument = 1;
			gameEndMessage.Phony = false;
			gameEndMessage.DelayTime = 0f;
			InGameMenu.sPlayState.Endgame(ref gameEndMessage);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				(NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
				NetworkManager.Instance.Interface.SendMessage<GameEndMessage>(ref gameEndMessage);
			}
			SubMenuCharacterSelect.Instance.OnEnter();
			InGameMenu.sPlayState.Endgame(EndGameCondition.LevelComplete, false, false, 0f);
			GameStateManager.Instance.PopState();
			InGameMenu.HideInstant();
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x0007DDF2 File Offset: 0x0007BFF2
		protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
		{
			base.IControllerMove(iSender, iDirection);
		}

		// Token: 0x06001425 RID: 5157 RVA: 0x0007DDFC File Offset: 0x0007BFFC
		protected override string IGetHighlightedButtonName()
		{
			return InGameMenuMain.OPTION_STRINGS[this.mSelectedItem];
		}

		// Token: 0x06001426 RID: 5158 RVA: 0x0007DE0C File Offset: 0x0007C00C
		protected override void IControllerSelect(Controller iSender)
		{
			switch (this.mSelectedItem)
			{
			case 0:
				if (InGameMenu.sPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH)
				{
					InGameMenu.PushMenu(this.mTutorialSkipMenu);
					return;
				}
				break;
			case 1:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
				InGameMenu.Hide();
				return;
			case 2:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				InGameMenu.PushMenu(this.mRestartMenu);
				return;
			case 3:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				InGameMenu.PushMenu(InGameMenuMagicks.Instance);
				return;
			case 4:
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				InGameMenu.PushMenu(InGameMenuOptions.Instance);
				return;
			case 5:
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					InGameMenu.PushMenu(this.mDisconnectMenu);
					return;
				}
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				if (Credits.Instance.IsActive)
				{
					InGameMenu.PushMenu(this.mSkipCreditsMenu);
					return;
				}
				InGameMenu.PushMenu(this.mQuitMenu);
				break;
			default:
				return;
			}
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x0007DF22 File Offset: 0x0007C122
		protected override void IControllerBack(Controller iSender)
		{
			InGameMenu.Hide();
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0007DF2C File Offset: 0x0007C12C
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
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				(this.mMenuItems[this.mMenuItems.Count - 1] as MenuTextItem).SetText("#network_10".GetHashCodeCustom());
				return;
			}
			if (Credits.Instance.IsActive)
			{
				(this.mMenuItems[this.mMenuItems.Count - 1] as MenuTextItem).SetText("#credits_skip".GetHashCodeCustom());
				return;
			}
			(this.mMenuItems[this.mMenuItems.Count - 1] as MenuTextItem).SetText("#menu_main_07".GetHashCodeCustom());
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x0007DFF4 File Offset: 0x0007C1F4
		protected override void IUpdate(DataChannel iDataChannel, float iDeltaTime)
		{
			base.IUpdate(iDataChannel, iDeltaTime);
			this.mMenuItems[2].Enabled = (NetworkManager.Instance.State != NetworkState.Client);
			this.mMenuItems[0].Enabled = (InGameMenu.sPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH && NetworkManager.Instance.State != NetworkState.Client);
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x0007E06C File Offset: 0x0007C26C
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = 1f));
			color.W = this.mAlpha;
			Vector4 colorSelected = default(Vector4);
			colorSelected.X = (colorSelected.Y = (colorSelected.Z = 0f));
			colorSelected.W = this.mAlpha;
			Vector4 colorDisabled = default(Vector4);
			colorDisabled.X = (colorDisabled.Y = (colorDisabled.Z = 0.4f));
			colorDisabled.W = this.mAlpha;
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				if (InGameMenu.sPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH || i != 0)
				{
					MenuItem menuItem = this.mMenuItems[i];
					menuItem.Color = color;
					menuItem.ColorSelected = colorSelected;
					menuItem.ColorDisabled = colorDisabled;
					menuItem.Selected = (menuItem.Enabled & this.mSelectedItem == i);
					if (menuItem.Selected)
					{
						Matrix transform = default(Matrix);
						transform.M44 = 1f;
						transform.M11 = iBackgroundSize.X * InGameMenu.sScale;
						transform.M22 = menuItem.BottomRight.Y - menuItem.TopLeft.Y;
						transform.M41 = menuItem.Position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
						transform.M42 = menuItem.TopLeft.Y;
						InGameMenu.sEffect.Transform = transform;
						Vector4 color2 = default(Vector4);
						color2.X = (color2.Y = (color2.Z = 1f));
						color2.W = 0.8f * this.mAlpha;
						InGameMenu.sEffect.Color = color2;
						InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
						InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
						InGameMenu.sEffect.CommitChanges();
						InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
					}
				}
			}
			for (int j = 0; j < this.mMenuItems.Count; j++)
			{
				if (InGameMenu.sPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH || j != 0)
				{
					this.mMenuItems[j].Draw(InGameMenu.sEffect);
				}
			}
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x0007E323 File Offset: 0x0007C523
		protected override void OnExit()
		{
		}

		// Token: 0x04001599 RID: 5529
		private const string OPTION_BANANA = "banana";

		// Token: 0x0400159A RID: 5530
		private const string OPTION_RESUME = "resume";

		// Token: 0x0400159B RID: 5531
		private const string OPTION_RESTART = "restart_level";

		// Token: 0x0400159C RID: 5532
		private const string OPTION_MAGICKS = "magicks";

		// Token: 0x0400159D RID: 5533
		private const string OPTION_OPTIONS = "options";

		// Token: 0x0400159E RID: 5534
		private const string OPTION_CHEATS = "cheats";

		// Token: 0x0400159F RID: 5535
		private const string OPTION_QUIT = "quit";

		// Token: 0x040015A0 RID: 5536
		private static readonly string[] OPTION_STRINGS = new string[]
		{
			"banana",
			"resume",
			"restart_level",
			"magicks",
			"options",
			"quit"
		};

		// Token: 0x040015A1 RID: 5537
		private static InGameMenuMain sSingelton;

		// Token: 0x040015A2 RID: 5538
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040015A3 RID: 5539
		private InGameMenuAreYouSure mSkipCreditsMenu;

		// Token: 0x040015A4 RID: 5540
		private InGameMenuAreYouSure mRestartMenu;

		// Token: 0x040015A5 RID: 5541
		private InGameMenuAreYouSure mQuitMenu;

		// Token: 0x040015A6 RID: 5542
		private InGameMenuAreYouSure mTutorialSkipMenu;

		// Token: 0x040015A7 RID: 5543
		private InGameMenuAreYouSure mDisconnectMenu;
	}
}
