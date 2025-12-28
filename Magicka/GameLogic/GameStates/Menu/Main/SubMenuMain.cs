using System;
using System.Collections.Generic;
using System.Threading;
using Magicka.Audio;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using SteamWrapper;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x020005A5 RID: 1445
	internal class SubMenuMain : SubMenu
	{
		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06002B32 RID: 11058 RVA: 0x00153800 File Offset: 0x00151A00
		public static SubMenuMain Instance
		{
			get
			{
				if (SubMenuMain.mSingelton == null)
				{
					lock (SubMenuMain.mSingeltonLock)
					{
						if (SubMenuMain.mSingelton == null)
						{
							SubMenuMain.mSingelton = new SubMenuMain();
						}
					}
				}
				return SubMenuMain.mSingelton;
			}
		}

		// Token: 0x06002B33 RID: 11059 RVA: 0x00153854 File Offset: 0x00151A54
		public SubMenuMain()
		{
			if (SubMenuMain.sNEWPagesTexture == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					SubMenuMain.sNEWPagesTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages_NEW");
				}
			}
			this.mSelectedPosition = 0;
			this.mMenuItems = new List<MenuItem>();
			this.mRUSure = new OptionsMessageBox(SubMenuMain.LOC_RU_SURE, new int[]
			{
				Defines.LOC_GEN_YES,
				Defines.LOC_GEN_NO
			});
			this.mRUSure.Select += new Action<OptionsMessageBox, int>(this.QuitCallBack);
			this.SetupMenu();
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x00153958 File Offset: 0x00151B58
		private void CheckForMythosDLC()
		{
			this.mHasMythosLicense = HackHelper.CheckLicenseMythos();
		}

		// Token: 0x06002B35 RID: 11061 RVA: 0x00153965 File Offset: 0x00151B65
		private void CheckForVietnamDLC()
		{
			this.mHasVietnamLicense = HackHelper.CheckLicenseVietnam();
		}

		// Token: 0x06002B36 RID: 11062 RVA: 0x00153972 File Offset: 0x00151B72
		private void CheckForOSOTCDLC()
		{
			this.mHasOSOTCLicense = HackHelper.CheckLicenseOSOTC();
		}

		// Token: 0x06002B37 RID: 11063 RVA: 0x0015397F File Offset: 0x00151B7F
		private void CheckForDungeons1DLC()
		{
			this.mHasDUNG1License = HackHelper.CheckLicenseDungeons1();
		}

		// Token: 0x06002B38 RID: 11064 RVA: 0x0015398C File Offset: 0x00151B8C
		private void CheckForDungeons2DLC()
		{
			this.mHasDUNG2License = HackHelper.CheckLicenseDungeons2();
		}

		// Token: 0x06002B39 RID: 11065 RVA: 0x00153999 File Offset: 0x00151B99
		private void DlcInstalled(DlcInstalled obj)
		{
			LevelManager.Instance.UpdateMythosLicense();
			this.CheckForMythosDLC();
			this.CheckForVietnamDLC();
			this.CheckForOSOTCDLC();
			this.CheckForDungeons1DLC();
			this.CheckForDungeons2DLC();
		}

		// Token: 0x06002B3A RID: 11066 RVA: 0x001539C4 File Offset: 0x00151BC4
		private void QuitCallBack(MessageBox iSender, int iSelection)
		{
			switch (iSelection)
			{
			case 0:
				GC.WaitForFullGCComplete(2000);
				Thread.Sleep(0);
				Game.Instance.Exit();
				break;
			case 1:
				break;
			default:
				return;
			}
		}

		// Token: 0x06002B3B RID: 11067 RVA: 0x001539FD File Offset: 0x00151BFD
		public void ShowRUSure()
		{
			this.mRUSure.Show();
		}

		// Token: 0x06002B3C RID: 11068 RVA: 0x00153A0C File Offset: 0x00151C0C
		public override void ControllerA(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 0:
				SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Campaign;
				Tome.Instance.PushMenu(SubMenuCampaignSelect_SaveSlotSelect.Instance, 1);
				return;
			case 1:
				if (this.mHasMythosLicense)
				{
					DLC_StatusHelper.Instance.Item_TrySetUsed("main_menu_entry", "mythos", true);
					SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Mythos;
					Tome.Instance.PushMenu(SubMenuCampaignSelect_SaveSlotSelect.Instance, 1);
					return;
				}
				SteamUtils.ActivateGameOverlayToStore(73058U, OverlayStoreFlag.None);
				return;
			case 2:
				if (this.mHasVietnamLicense)
				{
					SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.StoryChallange;
					DLC_StatusHelper.Instance.Item_TrySetUsed("level", "#challenge_vietnam", true);
					SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, "ch_vietnam", false);
					SubMenuCharacterSelect.Instance.ValidateLevels();
					SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
					Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
					return;
				}
				SteamUtils.ActivateGameOverlayToStore(42918U, OverlayStoreFlag.None);
				return;
			case 3:
				if (this.mHasOSOTCLicense)
				{
					SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.StoryChallange;
					DLC_StatusHelper.Instance.Item_TrySetUsed("level", "#challenge_osotc", true);
					SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, "ch_osotc", false);
					SubMenuCharacterSelect.Instance.ValidateLevels();
					SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
					Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
					return;
				}
				SteamUtils.ActivateGameOverlayToStore(73093U, OverlayStoreFlag.None);
				return;
			case 4:
				if (this.mHasDUNG1License)
				{
					SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.StoryChallange;
					DLC_StatusHelper.Instance.Item_TrySetUsed("level", "#challenge_dungeons_chapter1", true);
					SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, "ch_dungeons_ch1", false);
					SubMenuCharacterSelect.Instance.ValidateLevels();
					SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
					Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
					return;
				}
				SteamUtils.ActivateGameOverlayToStore(73115U, OverlayStoreFlag.None);
				return;
			case 5:
				if (this.mHasDUNG2License)
				{
					SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.StoryChallange;
					DLC_StatusHelper.Instance.Item_TrySetUsed("level", "#challenge_dungeons_chapter2", true);
					SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, "ch_dungeons_ch2", false);
					SubMenuCharacterSelect.Instance.ValidateLevels();
					SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
					Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
					return;
				}
				SteamUtils.ActivateGameOverlayToStore(0U, OverlayStoreFlag.None);
				return;
			case 6:
				SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Challenge;
				SubMenuCharacterSelect.Instance.SetSettings(GameType.Challenge, -1, false);
				SubMenuCharacterSelect.Instance.ValidateLevels();
				SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
				Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
				return;
			case 7:
				SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Versus;
				SubMenuCharacterSelect.Instance.SetSettings(GameType.Versus, -1, false);
				SubMenuCharacterSelect.Instance.ValidateLevels();
				SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
				Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
				return;
			case 8:
				Tome.Instance.PushMenu(SubMenuOnline.Instance, 1);
				return;
			case 9:
				Tome.Instance.PushMenu(SubMenuLeaderboards.Instance, 1);
				return;
			case 10:
				Tome.Instance.PushMenu(SubMenuOptions.Instance, 1);
				return;
			case 11:
				this.mRUSure.Show();
				return;
			default:
				return;
			}
		}

		// Token: 0x06002B3D RID: 11069 RVA: 0x00153D24 File Offset: 0x00151F24
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag) && flag)
			{
				if (this.mMenuItems[this.mMenuItems.Count - 1].Enabled && this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref vector))
				{
					this.mSelectedPosition = this.mMenuItems.Count - 1;
					this.ControllerMouseClicked(iSender);
					return;
				}
				int i = 0;
				while (i < this.mMenuItems.Count)
				{
					MenuItem menuItem = this.mMenuItems[i];
					if (menuItem != null && menuItem.Enabled && menuItem.InsideBounds(ref vector))
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

		// Token: 0x06002B3E RID: 11070 RVA: 0x00153E1C File Offset: 0x0015201C
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag))
			{
				if (flag)
				{
					bool flag2 = false;
					if (this.mMenuItems[this.mMenuItems.Count - 1].Enabled && this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref vector))
					{
						this.UnselectAllMenuItems();
						this.mMenuItems[this.mMenuItems.Count - 1].Selected = true;
						flag2 = true;
						this.mSelectedPosition = this.mMenuItems.Count - 1;
					}
					else if (this.mMenuItems[11].Enabled && this.mMenuItems[11].InsideBounds(ref vector))
					{
						this.UnselectAllMenuItems();
						this.mMenuItems[5].Selected = true;
						this.mSelectedPosition = 5;
						flag2 = true;
					}
					else
					{
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
								this.UnselectAllMenuItems();
								if (this.mSelectedPosition == 1)
								{
									if (!this.mHasMythosLicense)
									{
										this.mLockItem_Mythos.Selected = true;
									}
									else
									{
										this.mLockItem_Mythos.Selected = false;
									}
									if (this.mMythosHasNewContent)
									{
										this.mNewItem_Mythos.Selected = true;
									}
									ToolTipMan.Instance.KillAll(false);
								}
								else if (this.mSelectedPosition == 2)
								{
									if (!this.mHasVietnamLicense)
									{
										this.mLockItem_Vietnam.Selected = true;
									}
									else
									{
										this.mLockItem_Mythos.Selected = false;
									}
									if (this.mVietnamHasNewContent)
									{
										this.mNewItem_Vietnam.Selected = true;
									}
									ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenu.LOC_TT_VIETNAM), iState);
								}
								else if (this.mSelectedPosition == 3)
								{
									if (!this.mHasOSOTCLicense)
									{
										this.mLockItem_OSOTC.Selected = true;
									}
									else
									{
										this.mLockItem_OSOTC.Selected = false;
									}
									if (this.mOSOTCHasNewContent)
									{
										this.mNewItem_OSOTC.Selected = true;
									}
									ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenu.LOC_TT_OSOTC), iState);
								}
								else if (this.mSelectedPosition == 4)
								{
									if (!this.mHasDUNG1License)
									{
										this.mLockItem_DUNG1.Selected = true;
									}
									else
									{
										this.mLockItem_DUNG1.Selected = false;
									}
									if (this.mDungeons1HasNewContent)
									{
										this.mNewItem_DUNG1.Selected = true;
									}
									ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenu.LOC_TT_DUNG1), iState);
								}
								else if (this.mSelectedPosition == 5)
								{
									if (!this.mHasDUNG2License)
									{
										this.mLockItem_DUNG2.Selected = true;
									}
									else
									{
										this.mLockItem_DUNG2.Selected = false;
									}
									if (this.mDungeons2HasNewContent)
									{
										this.mNewItem_DUNG2.Selected = true;
									}
									ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenu.LOC_TT_DUNG2), iState);
								}
								else if (this.mSelectedPosition == 6)
								{
									if (this.mChallangeHasNewContent)
									{
										this.mNewItem_Challange.Selected = true;
									}
									ToolTipMan.Instance.KillAll(false);
								}
								else if (this.mSelectedPosition == 7)
								{
									if (this.mVersusHasNewContent)
									{
										this.mNewItem_Versus.Selected = true;
									}
									ToolTipMan.Instance.KillAll(false);
								}
								else if (this.mSelectedPosition == 10)
								{
									ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenu.LOC_OPTIONS), iState);
								}
								else if (this.mSelectedPosition == 9)
								{
									ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenu.LOC_LEADERBOARDS), iState);
								}
								else
								{
									ToolTipMan.Instance.KillAll(false);
								}
								menuItem.Selected = true;
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						this.UnselectAllMenuItems();
						return;
					}
				}
			}
			else if (!this.mKeyboardSelection)
			{
				this.UnselectAllMenuItems();
			}
		}

		// Token: 0x06002B3F RID: 11071 RVA: 0x00154234 File Offset: 0x00152434
		private void UnselectAllMenuItems()
		{
			lock (this.mMenuItems)
			{
				for (int i = 0; i < this.mMenuItems.Count; i++)
				{
					this.mMenuItems[i].Selected = false;
				}
			}
			this.mLockItem_Mythos.Selected = (this.mLockItem_Vietnam.Selected = (this.mLockItem_OSOTC.Selected = (this.mLockItem_DUNG1.Selected = (this.mNewItem_Mythos.Selected = (this.mNewItem_Vietnam.Selected = (this.mNewItem_OSOTC.Selected = (this.mNewItem_DUNG1.Selected = (this.mNewItem_Versus.Selected = (this.mNewItem_Challange.Selected = false)))))))));
		}

		// Token: 0x06002B40 RID: 11072 RVA: 0x00154328 File Offset: 0x00152528
		public override void ControllerUp(Controller iSender)
		{
			this.mKeyboardSelection = true;
			try
			{
				do
				{
					switch (this.mSelectedPosition)
					{
					case 0:
					case 1:
						this.mSelectedPosition = this.mMenuItems.Count - 1;
						break;
					case 2:
					case 3:
						this.mSelectedPosition = 0;
						break;
					case 4:
					case 5:
						this.mSelectedPosition = 1;
						break;
					case 6:
						this.mSelectedPosition = 2;
						break;
					case 7:
						this.mSelectedPosition = 3;
						break;
					case 8:
						this.mSelectedPosition = 4;
						break;
					case 9:
						this.mSelectedPosition = 6;
						break;
					case 10:
						this.mSelectedPosition = 8;
						break;
					case 11:
						this.mSelectedPosition = 9;
						break;
					}
				}
				while (!this.mMenuItems[this.mSelectedPosition].Enabled);
			}
			catch
			{
				this.mSelectedPosition = 0;
			}
		}

		// Token: 0x06002B41 RID: 11073 RVA: 0x00154410 File Offset: 0x00152610
		public override void ControllerDown(Controller iSender)
		{
			this.mKeyboardSelection = true;
			try
			{
				do
				{
					switch (this.mSelectedPosition)
					{
					case 0:
						this.mSelectedPosition = 2;
						break;
					case 1:
						this.mSelectedPosition = 4;
						break;
					case 2:
						this.mSelectedPosition = 6;
						break;
					case 3:
						this.mSelectedPosition = 7;
						break;
					case 4:
					case 5:
						this.mSelectedPosition = 8;
						break;
					case 6:
						this.mSelectedPosition = 9;
						break;
					case 7:
					case 8:
						this.mSelectedPosition = 10;
						break;
					case 9:
					case 10:
						this.mSelectedPosition = 11;
						break;
					case 11:
						this.mSelectedPosition = 0;
						break;
					}
				}
				while (!this.mMenuItems[this.mSelectedPosition].Enabled);
			}
			catch
			{
				this.mSelectedPosition = 0;
			}
		}

		// Token: 0x06002B42 RID: 11074 RVA: 0x001544EC File Offset: 0x001526EC
		public override void ControllerLeft(Controller iSender)
		{
			base.ControllerUp(iSender);
		}

		// Token: 0x06002B43 RID: 11075 RVA: 0x001544F5 File Offset: 0x001526F5
		public override void ControllerRight(Controller iSender)
		{
			base.ControllerDown(iSender);
		}

		// Token: 0x06002B44 RID: 11076 RVA: 0x00154624 File Offset: 0x00152824
		public override void OnEnter()
		{
			ToolTipMan.Instance.KillAll(true);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, GamePadMenuHelp.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			if (this.mFirstTimeOnEnter)
			{
				this.mFirstTimeOnEnter = false;
			}
			else
			{
				Tome.LoadNewPromotion();
			}
			if (this.mFirstTimeOnEnter)
			{
				this.mMythosHasNewContent = (this.mVietnamHasNewContent = (this.mOSOTCHasNewContent = (this.mDungeons1HasNewContent = (this.mDungeons2HasNewContent = (this.mVersusHasNewContent = (this.mChallangeHasNewContent = false))))));
			}
			DLC_StatusHelper instance = DLC_StatusHelper.Instance;
			Thread.Sleep(0);
			Game.Instance.Form.BeginInvoke(new Action(delegate()
			{
				this.mMythosHasNewContent = DLC_StatusHelper.Instance.Item_IsUnused("main_menu_entry", "mythos", 73058U, false);
				this.mMythosLevelCastle = DLC_StatusHelper.Instance.Item_IsUnused("level", "#tsar_wizardcastle", 73058U, false);
				this.mMythosLevelMountain = DLC_StatusHelper.Instance.Item_IsUnused("level", "#tsar_mountaindale", 73058U, false);
				this.mMythosLevelRlyeh = DLC_StatusHelper.Instance.Item_IsUnused("level", "#tsar_rlyeh", 73058U, false);
				this.mVietnamHasNewContent = DLC_StatusHelper.Instance.Item_IsUnused("level", "#challenge_vietnam", 42918U, false);
				this.mOSOTCHasNewContent = DLC_StatusHelper.Instance.Item_IsUnused("level", "#challenge_osotc", 73093U, false);
				this.mDungeons1HasNewContent = DLC_StatusHelper.Instance.Item_IsUnused("level", "#challenge_dungeons_chapter1", 73115U, false);
				this.mDungeons2HasNewContent = DLC_StatusHelper.Instance.Item_IsUnused("level", "#challenge_dungeons_chapter2", 0U, false);
				this.mChallangeHasNewContent = DLC_StatusHelper.HasAnyUnusedLevels(GameType.Challenge);
				this.mVersusHasNewContent = DLC_StatusHelper.HasAnyUnusedLevels(GameType.Versus);
			}));
			SteamAPI.GameOverlayActivated -= this.SteamOverlayActivated;
			SteamAPI.GameOverlayActivated += this.SteamOverlayActivated;
			base.OnEnter();
			this.mSelectedPosition = -1;
		}

		// Token: 0x06002B45 RID: 11077 RVA: 0x00154728 File Offset: 0x00152928
		public override void OnExit()
		{
			ToolTipMan.Instance.KillAll(true);
			SteamAPI.GameOverlayActivated -= this.SteamOverlayActivated;
			base.OnExit();
		}

		// Token: 0x06002B46 RID: 11078 RVA: 0x0015474C File Offset: 0x0015294C
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.VertexColorEnabled = false;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].Draw(this.mEffect);
			}
			if (!this.mHasMythosLicense)
			{
				this.mLockItem_Mythos.Draw(this.mEffect);
			}
			else if (this.mMythosLevelCastle && this.mMythosLevelMountain && this.mMythosLevelRlyeh)
			{
				this.mNewItem_Mythos.Draw(this.mEffect);
			}
			if (!this.mHasVietnamLicense)
			{
				this.mLockItem_Vietnam.Draw(this.mEffect);
			}
			else if (this.mVietnamHasNewContent)
			{
				this.mNewItem_Vietnam.Draw(this.mEffect);
			}
			if (!this.mHasOSOTCLicense)
			{
				this.mLockItem_OSOTC.Draw(this.mEffect);
			}
			else if (this.mOSOTCHasNewContent)
			{
				this.mNewItem_OSOTC.Draw(this.mEffect);
			}
			if (!this.mHasDUNG1License)
			{
				this.mLockItem_DUNG1.Draw(this.mEffect);
			}
			else if (this.mDungeons1HasNewContent)
			{
				this.mNewItem_DUNG1.Draw(this.mEffect);
			}
			if (!this.mHasDUNG2License)
			{
				this.mLockItem_DUNG2.Draw(this.mEffect);
			}
			else if (this.mDungeons2HasNewContent)
			{
				this.mNewItem_DUNG2.Draw(this.mEffect);
			}
			if (this.mChallangeHasNewContent)
			{
				this.mNewItem_Challange.Draw(this.mEffect);
			}
			if (this.mVersusHasNewContent)
			{
				this.mNewItem_Versus.Draw(this.mEffect);
			}
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06002B47 RID: 11079 RVA: 0x00154938 File Offset: 0x00152B38
		private void SteamOverlayActivated(GameOverlayActivated gameOverlayActivated)
		{
			if (gameOverlayActivated.mActive == 0)
			{
				this.DlcInstalled(default(DlcInstalled));
			}
		}

		// Token: 0x06002B48 RID: 11080 RVA: 0x00154968 File Offset: 0x00152B68
		private void SetupMenu()
		{
			Vector2 iTextureOffset = new Vector2(944f / (float)SubMenu.sPagesTexture.Width, 816f / (float)SubMenu.sPagesTexture.Height);
			Vector2 iTextureOffset2 = new Vector2(972f / (float)SubMenuMain.sNEWPagesTexture.Width, 974f / (float)SubMenuMain.sNEWPagesTexture.Height);
			Vector2 iTextureOffset3 = new Vector2(0f / (float)SubMenuMain.sNEWPagesTexture.Width, 0f / (float)SubMenuMain.sNEWPagesTexture.Height);
			Vector2 iTextureScale = new Vector2(SubMenuMain.CAMPAIGN_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.CAMPAIGN_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenuMain.CAMPAIGN_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale, SubMenu.LOC_ADVENTURE, SubMenuMain.CAMPAIGN_TEXT, TextAlign.Center, SubMenuMain.mFont, SubMenuMain.CAMPAIGN_SIZE));
			iTextureOffset3 = new Vector2(390f / (float)SubMenuMain.sNEWPagesTexture.Width, 0f / (float)SubMenuMain.sNEWPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.MYTHOS_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.MYTHOS_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenuMain.MYTHOS_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale, SubMenu.LOC_MYTHOS, SubMenuMain.MYTHOS_TEXT, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.MYTHOS_SIZE));
			iTextureScale = new Vector2(SubMenuMain.LOCK_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			this.mLockItem_Mythos = new MenuImageTextItem(SubMenuMain.MYTHOS_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
			this.mLockItem_Mythos.Selected = false;
			iTextureScale = new Vector2(SubMenuMain.NEW_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mNewItem_Mythos = new MenuImageTextItem(SubMenuMain.MYTHOS_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
			this.mNewItem_Mythos.Selected = false;
			iTextureOffset3 = new Vector2(0f / (float)SubMenuMain.sNEWPagesTexture.Width, 230f / (float)SubMenuMain.sNEWPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.VIETNAM_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.VIETNAM_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenuMain.VIETNAM_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.VIETNAM_SIZE));
			iTextureScale = new Vector2(SubMenuMain.LOCK_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			this.mLockItem_Vietnam = new MenuImageTextItem(SubMenuMain.VIETNAM_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
			this.mLockItem_Vietnam.Selected = false;
			iTextureScale = new Vector2(SubMenuMain.NEW_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mNewItem_Vietnam = new MenuImageTextItem(SubMenuMain.VIETNAM_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
			this.mNewItem_Vietnam.Selected = false;
			iTextureOffset3 = new Vector2(166f / (float)SubMenuMain.sNEWPagesTexture.Width, 230f / (float)SubMenuMain.sNEWPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.OSOTC_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.OSOTC_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenuMain.OSOTC_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.OSOTC_SIZE));
			iTextureScale = new Vector2(SubMenuMain.LOCK_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			this.mLockItem_OSOTC = new MenuImageTextItem(SubMenuMain.OSOTC_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
			this.mLockItem_OSOTC.Selected = false;
			iTextureScale = new Vector2(SubMenuMain.NEW_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mNewItem_OSOTC = new MenuImageTextItem(SubMenuMain.OSOTC_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
			this.mNewItem_OSOTC.Selected = false;
			iTextureOffset3 = new Vector2(332f / (float)SubMenuMain.sNEWPagesTexture.Width, 230f / (float)SubMenuMain.sNEWPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.DUNGEONS1_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.DUNGEONS1_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenuMain.DUNGEONS1_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.DUNGEONS1_SIZE));
			iTextureScale = new Vector2(SubMenuMain.LOCK_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			this.mLockItem_DUNG1 = new MenuImageTextItem(SubMenuMain.DUNGEONS1_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
			this.mLockItem_DUNG1.Selected = false;
			iTextureScale = new Vector2(SubMenuMain.NEW_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mNewItem_DUNG1 = new MenuImageTextItem(SubMenuMain.DUNGEONS1_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
			this.mNewItem_DUNG1.Selected = false;
			iTextureOffset3 = new Vector2(498f / (float)SubMenuMain.sNEWPagesTexture.Width, 230f / (float)SubMenuMain.sNEWPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.DUNGEONS2_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.DUNGEONS2_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenuMain.DUNGEONS2_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.DUNGEONS2_SIZE));
			iTextureScale = new Vector2(SubMenuMain.LOCK_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			this.mLockItem_DUNG2 = new MenuImageTextItem(SubMenuMain.DUNGEONS2_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
			this.mLockItem_DUNG2.Selected = false;
			iTextureScale = new Vector2(SubMenuMain.NEW_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mNewItem_DUNG2 = new MenuImageTextItem(SubMenuMain.DUNGEONS2_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
			this.mNewItem_DUNG2.Selected = false;
			iTextureOffset3 = new Vector2(1040f / (float)SubMenu.sPagesTexture.Width, 416f / (float)SubMenu.sPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.CHALLENGE_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenuMain.CHALLENGE_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			MenuImageTextItem menuImageTextItem = new MenuImageTextItem(SubMenuMain.CHALLENGE_POSITION, SubMenu.sPagesTexture, iTextureOffset3, iTextureScale, SubMenu.LOC_CHALLENGES, SubMenuMain.CHALLENGE_TEXT, TextAlign.Center, SubMenuMain.mFont, SubMenuMain.CHALLENGE_SIZE);
			menuImageTextItem.SetHitArea(0, 0, (int)SubMenuMain.CHALLENGE_SIZE.X - 56, (int)SubMenuMain.CHALLENGE_SIZE.Y);
			this.mMenuItems.Add(menuImageTextItem);
			iTextureScale = new Vector2(SubMenuMain.NEW_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mNewItem_Challange = new MenuImageTextItem(SubMenuMain.CHALLANGE_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
			this.mNewItem_Challange.Selected = false;
			iTextureOffset3 = new Vector2((1040f + SubMenuMain.CHALLENGE_SIZE.X) / (float)SubMenu.sPagesTexture.Width, 416f / (float)SubMenu.sPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.VERSUS_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenuMain.VERSUS_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			MenuImageTextItem menuImageTextItem2 = new MenuImageTextItem(SubMenuMain.VERSUS_POSITION, SubMenu.sPagesTexture, iTextureOffset3, iTextureScale, SubMenu.LOC_VERSUS, SubMenuMain.VERSUS_TEXT, TextAlign.Center, SubMenuMain.mFont, SubMenuMain.VERSUS_SIZE);
			menuImageTextItem2.SetHitArea(56, 0, (int)SubMenuMain.VERSUS_SIZE.X - 112, (int)SubMenuMain.VERSUS_SIZE.Y);
			this.mMenuItems.Add(menuImageTextItem2);
			iTextureScale = new Vector2(SubMenuMain.NEW_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			this.mNewItem_Versus = new MenuImageTextItem(SubMenuMain.VERSUS_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
			this.mNewItem_Versus.Selected = false;
			iTextureOffset3 = new Vector2((1040f + SubMenuMain.CHALLENGE_SIZE.X + SubMenuMain.VERSUS_SIZE.X) / (float)SubMenu.sPagesTexture.Width, 416f / (float)SubMenu.sPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.ONLINE_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenuMain.ONLINE_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			MenuImageTextItem menuImageTextItem3 = new MenuImageTextItem(SubMenuMain.ONLINE_POSITION, SubMenu.sPagesTexture, iTextureOffset3, iTextureScale, SubMenu.LOC_ONLINE_PLAY, SubMenuMain.ONLINE_TEXT, TextAlign.Center, SubMenuMain.mFont, SubMenuMain.ONLINE_SIZE);
			menuImageTextItem3.SetHitArea(56, 0, (int)SubMenuMain.ONLINE_SIZE.X - 56, (int)SubMenuMain.ONLINE_SIZE.Y);
			this.mMenuItems.Add(menuImageTextItem3);
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				if (this.mMenuItems[i] != null)
				{
					this.mMenuItems[i].Color = new Vector4(1f - Defines.MESSAGEBOX_COLOR_DEFAULT.X, 1f - Defines.MESSAGEBOX_COLOR_DEFAULT.Y, 1f - Defines.MESSAGEBOX_COLOR_DEFAULT.Z, 1f);
					(this.mMenuItems[i] as MenuImageTextItem).Text.DrawShadows = true;
					(this.mMenuItems[i] as MenuImageTextItem).Text.ShadowAlpha = 1f;
					(this.mMenuItems[i] as MenuImageTextItem).Text.ShadowsOffset = new Vector2(2f, 2f);
					this.mMenuItems[i].ColorSelected = new Vector4(1f);
					this.mMenuItems[i].ColorDisabled = this.mMenuItems[i].Color;
				}
			}
			iTextureOffset3 = new Vector2(1f / (float)SubMenuMain.sNEWPagesTexture.Width, 952f / (float)SubMenuMain.sNEWPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.LEADERBOARD_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.LEADERBOARD_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			MenuImageTextItem menuImageTextItem4 = new MenuImageTextItem(SubMenuMain.LEADERBOARD_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LEADERBOARD_SIZE);
			menuImageTextItem4.SetHitArea(0, 0, (int)SubMenuMain.LEADERBOARD_SIZE.X, (int)SubMenuMain.LEADERBOARD_SIZE.Y);
			this.mMenuItems.Add(menuImageTextItem4);
			iTextureOffset3 = new Vector2(73f / (float)SubMenuMain.sNEWPagesTexture.Width, 952f / (float)SubMenuMain.sNEWPagesTexture.Height);
			iTextureScale = new Vector2(SubMenuMain.OPTIONS_SIZE.X / (float)SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.OPTIONS_SIZE.Y / (float)SubMenuMain.sNEWPagesTexture.Height);
			MenuImageTextItem menuImageTextItem5 = new MenuImageTextItem(SubMenuMain.OPTIONS_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.OPTIONS_SIZE);
			menuImageTextItem5.SetHitArea(0, 0, (int)SubMenuMain.OPTIONS_SIZE.X, (int)SubMenuMain.OPTIONS_SIZE.Y);
			this.mMenuItems.Add(menuImageTextItem5);
			Vector2 iTextureScale2 = new Vector2(SubMenu.BACK_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenu.BACK_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, iTextureScale2, 0, default(Vector2), TextAlign.Left, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
			SteamAPI.DlcInstalled += this.DlcInstalled;
			this.CheckForMythosDLC();
			this.CheckForOSOTCDLC();
			this.CheckForVietnamDLC();
			this.CheckForDungeons1DLC();
			this.CheckForDungeons2DLC();
		}

		// Token: 0x04002E6A RID: 11882
		private const float NEW_UV_X = 972f;

		// Token: 0x04002E6B RID: 11883
		private const float NEW_UV_Y = 974f;

		// Token: 0x04002E6C RID: 11884
		private const float CAMPAIGN_UV_X = 0f;

		// Token: 0x04002E6D RID: 11885
		private const float CAMPAIGN_UV_Y = 0f;

		// Token: 0x04002E6E RID: 11886
		private const float CAMPAIGN_UV_WIDTH = 388f;

		// Token: 0x04002E6F RID: 11887
		private const float CAMPAIGN_UV_HEIGHT = 226f;

		// Token: 0x04002E70 RID: 11888
		private const float MYTHOS_UV_X = 390f;

		// Token: 0x04002E71 RID: 11889
		private const float MYTHOS_UV_Y = 0f;

		// Token: 0x04002E72 RID: 11890
		private const float MYTHOS_UV_WIDTH = 388f;

		// Token: 0x04002E73 RID: 11891
		private const float MYTHOS_UV_HEIGHT = 226f;

		// Token: 0x04002E74 RID: 11892
		private const float ROW_BOTTOM_PADDING = 12f;

		// Token: 0x04002E75 RID: 11893
		private const float SECOND_ROW_RIGHT_PADDING_AB = 46f;

		// Token: 0x04002E76 RID: 11894
		private const float SECOND_ROW_RIGHT_PADDING_BC = 27f;

		// Token: 0x04002E77 RID: 11895
		private const float SECOND_ROW_RIGHT_PADDING_CD = 26f;

		// Token: 0x04002E78 RID: 11896
		private const float VIETNAM_UV_X = 0f;

		// Token: 0x04002E79 RID: 11897
		private const float VIETNAM_UV_Y = 230f;

		// Token: 0x04002E7A RID: 11898
		private const float VIETNAM_UV_WIDTH = 165f;

		// Token: 0x04002E7B RID: 11899
		private const float VIETNAM_UV_HEIGHT = 177f;

		// Token: 0x04002E7C RID: 11900
		private const float OSOTC_UV_X = 166f;

		// Token: 0x04002E7D RID: 11901
		private const float OSOTC_UV_Y = 230f;

		// Token: 0x04002E7E RID: 11902
		private const float OSOTC_UV_WIDTH = 165f;

		// Token: 0x04002E7F RID: 11903
		private const float OSOTC_UV_HEIGHT = 177f;

		// Token: 0x04002E80 RID: 11904
		private const float DUNGEONS1_UV_X = 332f;

		// Token: 0x04002E81 RID: 11905
		private const float DUNGEONS1_UV_Y = 230f;

		// Token: 0x04002E82 RID: 11906
		private const float DUNGEONS1_UV_WIDTH = 165f;

		// Token: 0x04002E83 RID: 11907
		private const float DUNGEONS1_UV_HEIGHT = 177f;

		// Token: 0x04002E84 RID: 11908
		private const float DUNGEONS2_UV_X = 498f;

		// Token: 0x04002E85 RID: 11909
		private const float DUNGEONS2_UV_Y = 230f;

		// Token: 0x04002E86 RID: 11910
		private const float DUNGEONS2_UV_WIDTH = 165f;

		// Token: 0x04002E87 RID: 11911
		private const float DUNGEONS2_UV_HEIGHT = 177f;

		// Token: 0x04002E88 RID: 11912
		private const float LEADERBOARD_UV_X = 1f;

		// Token: 0x04002E89 RID: 11913
		private const float LEADERBOARD_UV_Y = 952f;

		// Token: 0x04002E8A RID: 11914
		private const float LEADERBOARD_UV_WIDTH = 71f;

		// Token: 0x04002E8B RID: 11915
		private const float LEADERBOARD_UV_HEIGHT = 71f;

		// Token: 0x04002E8C RID: 11916
		private const float OPTIONS_UV_X = 73f;

		// Token: 0x04002E8D RID: 11917
		private const float OPTIONS_UV_Y = 952f;

		// Token: 0x04002E8E RID: 11918
		private const float OPTIONS_UV_WIDTH = 71f;

		// Token: 0x04002E8F RID: 11919
		private const float OPTIONS_UV_HEIGHT = 71f;

		// Token: 0x04002E90 RID: 11920
		private const uint APPID_VIETNAM = 42918U;

		// Token: 0x04002E91 RID: 11921
		private const string LEVELNAME_VIETNAM = "#challenge_vietnam";

		// Token: 0x04002E92 RID: 11922
		private const uint APPID_OSOTC = 73093U;

		// Token: 0x04002E93 RID: 11923
		private const string LEVELNAME_OSOTC = "#challenge_osotc";

		// Token: 0x04002E94 RID: 11924
		private const uint APPID_DUNG1 = 73115U;

		// Token: 0x04002E95 RID: 11925
		private const string LEVELNAME_DUNGEONS1 = "#challenge_dungeons_chapter1";

		// Token: 0x04002E96 RID: 11926
		private const uint APPID_DUNG2 = 0U;

		// Token: 0x04002E97 RID: 11927
		private const string LEVELNAME_DUNGEONS2 = "#challenge_dungeons_chapter2";

		// Token: 0x04002E98 RID: 11928
		private const uint APPID_MYTHOS = 73058U;

		// Token: 0x04002E99 RID: 11929
		private static SubMenuMain mSingelton;

		// Token: 0x04002E9A RID: 11930
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04002E9B RID: 11931
		private static readonly BitmapFont mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);

		// Token: 0x04002E9C RID: 11932
		private static readonly int LOC_RU_SURE = "#add_menu_rus_quit".GetHashCodeCustom();

		// Token: 0x04002E9D RID: 11933
		private static readonly float HALF_LINEHEIGHT = (float)SubMenuMain.mFont.LineHeight * 0.5f;

		// Token: 0x04002E9E RID: 11934
		private static readonly Vector2 LOCK_SIZE = new Vector2(112f, 160f);

		// Token: 0x04002E9F RID: 11935
		private static readonly Vector2 NEW_SIZE = new Vector2(52f, 50f);

		// Token: 0x04002EA0 RID: 11936
		private static readonly Vector2 CAMPAIGN_POSITION = new Vector2(166f, 54f);

		// Token: 0x04002EA1 RID: 11937
		private static readonly Vector2 CAMPAIGN_SIZE = new Vector2(388f, 226f);

		// Token: 0x04002EA2 RID: 11938
		private static readonly Vector2 CAMPAIGN_TEXT = new Vector2(SubMenuMain.CAMPAIGN_SIZE.X * 0.5f - 2f, SubMenuMain.CAMPAIGN_SIZE.Y - SubMenuMain.HALF_LINEHEIGHT + 5f);

		// Token: 0x04002EA3 RID: 11939
		private static readonly Vector2 MYTHOS_POSITION = new Vector2(SubMenuMain.CAMPAIGN_POSITION.X + SubMenuMain.CAMPAIGN_SIZE.X + 23f, SubMenuMain.CAMPAIGN_POSITION.Y);

		// Token: 0x04002EA4 RID: 11940
		private static readonly Vector2 MYTHOS_SIZE = SubMenuMain.CAMPAIGN_SIZE;

		// Token: 0x04002EA5 RID: 11941
		private static readonly Vector2 MYTHOS_TEXT = new Vector2(SubMenuMain.MYTHOS_SIZE.X * 0.8f, SubMenuMain.MYTHOS_SIZE.Y - SubMenuMain.HALF_LINEHEIGHT + 5f);

		// Token: 0x04002EA6 RID: 11942
		private static readonly Vector2 MYTHOS_LOCK_POSITION = new Vector2(SubMenuMain.MYTHOS_POSITION.X + SubMenuMain.MYTHOS_SIZE.X * 0.5f - SubMenuMain.LOCK_SIZE.X * 0.5f, SubMenuMain.MYTHOS_POSITION.Y + SubMenuMain.MYTHOS_SIZE.Y * 0.5f - SubMenuMain.LOCK_SIZE.Y * 0.5f - 8f);

		// Token: 0x04002EA7 RID: 11943
		private static readonly Vector2 MYTHOS_NEW_POSITION = new Vector2(SubMenuMain.MYTHOS_POSITION.X + SubMenuMain.MYTHOS_SIZE.X - (SubMenuMain.NEW_SIZE.X + 34f), SubMenuMain.MYTHOS_POSITION.Y + 8f);

		// Token: 0x04002EA8 RID: 11944
		private static readonly float SECOND_ROW_ITEM_HEIGHT = 191f;

		// Token: 0x04002EA9 RID: 11945
		private static readonly float SECOND_ROW_Y = SubMenuMain.CAMPAIGN_POSITION.Y + SubMenuMain.CAMPAIGN_SIZE.Y + 12f;

		// Token: 0x04002EAA RID: 11946
		private static readonly Vector2 VIETNAM_POSITION = new Vector2(SubMenuMain.CAMPAIGN_POSITION.X + 7f, SubMenuMain.SECOND_ROW_Y);

		// Token: 0x04002EAB RID: 11947
		private static readonly Vector2 VIETNAM_SIZE = new Vector2(165f, 177f);

		// Token: 0x04002EAC RID: 11948
		private static readonly Vector2 VIETNAM_LOCK_POSITION = new Vector2(SubMenuMain.VIETNAM_POSITION.X + SubMenuMain.VIETNAM_SIZE.X * 0.5f - SubMenuMain.LOCK_SIZE.X * 0.5f, SubMenuMain.VIETNAM_POSITION.Y + SubMenuMain.VIETNAM_SIZE.Y * 0.5f - SubMenuMain.LOCK_SIZE.Y * 0.5f);

		// Token: 0x04002EAD RID: 11949
		private static readonly Vector2 VIETNAM_NEW_POSITION = new Vector2(SubMenuMain.VIETNAM_POSITION.X + SubMenuMain.VIETNAM_SIZE.X - (SubMenuMain.NEW_SIZE.X + 10f), SubMenuMain.VIETNAM_POSITION.Y + 8f);

		// Token: 0x04002EAE RID: 11950
		private static readonly Vector2 OSOTC_POSITION = new Vector2(SubMenuMain.VIETNAM_POSITION.X + SubMenuMain.VIETNAM_SIZE.X + 46f, SubMenuMain.SECOND_ROW_Y);

		// Token: 0x04002EAF RID: 11951
		private static readonly Vector2 OSOTC_SIZE = new Vector2(165f, 177f);

		// Token: 0x04002EB0 RID: 11952
		private static readonly Vector2 OSOTC_LOCK_POSITION = new Vector2(SubMenuMain.OSOTC_POSITION.X + SubMenuMain.OSOTC_SIZE.X * 0.5f - SubMenuMain.LOCK_SIZE.X * 0.5f, SubMenuMain.OSOTC_POSITION.Y + SubMenuMain.OSOTC_SIZE.Y * 0.5f - SubMenuMain.LOCK_SIZE.Y * 0.5f);

		// Token: 0x04002EB1 RID: 11953
		private static readonly Vector2 OSOTC_NEW_POSITION = new Vector2(SubMenuMain.OSOTC_POSITION.X + SubMenuMain.OSOTC_SIZE.X - (SubMenuMain.NEW_SIZE.X + 10f), SubMenuMain.OSOTC_POSITION.Y + 8f);

		// Token: 0x04002EB2 RID: 11954
		private static readonly Vector2 DUNGEONS1_POSITION = new Vector2(SubMenuMain.OSOTC_POSITION.X + SubMenuMain.OSOTC_SIZE.X + 27f, SubMenuMain.SECOND_ROW_Y);

		// Token: 0x04002EB3 RID: 11955
		private static readonly Vector2 DUNGEONS1_SIZE = new Vector2(165f, 177f);

		// Token: 0x04002EB4 RID: 11956
		private static readonly Vector2 DUNGEONS1_LOCK_POSITION = new Vector2(SubMenuMain.DUNGEONS1_POSITION.X + SubMenuMain.DUNGEONS1_SIZE.X * 0.5f - SubMenuMain.LOCK_SIZE.X * 0.5f, SubMenuMain.DUNGEONS1_POSITION.Y + SubMenuMain.DUNGEONS1_SIZE.Y * 0.5f - SubMenuMain.LOCK_SIZE.Y * 0.5f);

		// Token: 0x04002EB5 RID: 11957
		private static readonly Vector2 DUNGEONS1_NEW_POSITION = new Vector2(SubMenuMain.DUNGEONS1_POSITION.X + SubMenuMain.DUNGEONS1_SIZE.X - (SubMenuMain.NEW_SIZE.X + 10f), SubMenuMain.DUNGEONS1_POSITION.Y + 8f);

		// Token: 0x04002EB6 RID: 11958
		private static readonly Vector2 DUNGEONS2_POSITION = new Vector2(SubMenuMain.DUNGEONS1_POSITION.X + SubMenuMain.DUNGEONS1_SIZE.X + 26f, SubMenuMain.SECOND_ROW_Y);

		// Token: 0x04002EB7 RID: 11959
		private static readonly Vector2 DUNGEONS2_SIZE = new Vector2(165f, 177f);

		// Token: 0x04002EB8 RID: 11960
		private static readonly Vector2 DUNGEONS2_LOCK_POSITION = new Vector2(SubMenuMain.DUNGEONS2_POSITION.X + SubMenuMain.DUNGEONS2_SIZE.X * 0.5f - SubMenuMain.LOCK_SIZE.X * 0.5f, SubMenuMain.DUNGEONS2_POSITION.Y + SubMenuMain.DUNGEONS2_SIZE.Y * 0.5f - SubMenuMain.LOCK_SIZE.Y * 0.5f);

		// Token: 0x04002EB9 RID: 11961
		private static readonly Vector2 DUNGEONS2_NEW_POSITION = new Vector2(SubMenuMain.DUNGEONS2_POSITION.X + SubMenuMain.DUNGEONS2_SIZE.X - (SubMenuMain.NEW_SIZE.X + 10f), SubMenuMain.DUNGEONS2_POSITION.Y + 8f);

		// Token: 0x04002EBA RID: 11962
		private static readonly float THIRD_ROW_Y = SubMenuMain.SECOND_ROW_Y + SubMenuMain.SECOND_ROW_ITEM_HEIGHT + 12f;

		// Token: 0x04002EBB RID: 11963
		private static readonly Vector2 CHALLENGE_POSITION = new Vector2(SubMenuMain.CAMPAIGN_POSITION.X - 3f, SubMenuMain.THIRD_ROW_Y);

		// Token: 0x04002EBC RID: 11964
		private static readonly Vector2 CHALLENGE_SIZE = new Vector2(336f, 400f);

		// Token: 0x04002EBD RID: 11965
		private static readonly Vector2 CHALLENGE_TEXT = new Vector2(SubMenuMain.CHALLENGE_SIZE.X * 0.5f, SubMenuMain.CHALLENGE_SIZE.Y - (float)SubMenuMain.mFont.LineHeight * 0.5f);

		// Token: 0x04002EBE RID: 11966
		private static readonly Vector2 CHALLANGE_NEW_POSITION = new Vector2(SubMenuMain.CHALLENGE_POSITION.X + SubMenuMain.CHALLENGE_SIZE.X - (SubMenuMain.NEW_SIZE.X + 70f), SubMenuMain.CHALLENGE_POSITION.Y + 25f);

		// Token: 0x04002EBF RID: 11967
		private static readonly Vector2 VERSUS_POSITION = new Vector2(SubMenuMain.CHALLENGE_POSITION.X + SubMenuMain.CHALLENGE_SIZE.X - 112f, SubMenuMain.THIRD_ROW_Y);

		// Token: 0x04002EC0 RID: 11968
		private static readonly Vector2 VERSUS_SIZE = SubMenuMain.CHALLENGE_SIZE;

		// Token: 0x04002EC1 RID: 11969
		private static readonly Vector2 VERSUS_TEXT = new Vector2(SubMenuMain.VERSUS_SIZE.X * 0.5f, SubMenuMain.VERSUS_SIZE.Y - SubMenuMain.HALF_LINEHEIGHT);

		// Token: 0x04002EC2 RID: 11970
		private static readonly Vector2 VERSUS_NEW_POSITION = new Vector2(SubMenuMain.VERSUS_POSITION.X + SubMenuMain.VERSUS_SIZE.X - (SubMenuMain.NEW_SIZE.X + 35f), SubMenuMain.VERSUS_POSITION.Y + 25f);

		// Token: 0x04002EC3 RID: 11971
		private static readonly Vector2 ONLINE_POSITION = new Vector2(SubMenuMain.CHALLENGE_POSITION.X + SubMenuMain.CHALLENGE_SIZE.X + SubMenuMain.VERSUS_SIZE.X - 224f, SubMenuMain.THIRD_ROW_Y);

		// Token: 0x04002EC4 RID: 11972
		private static readonly Vector2 ONLINE_SIZE = SubMenuMain.CHALLENGE_SIZE;

		// Token: 0x04002EC5 RID: 11973
		private static readonly Vector2 ONLINE_TEXT = new Vector2(SubMenuMain.ONLINE_SIZE.X * 0.5f, SubMenuMain.ONLINE_SIZE.Y - SubMenuMain.HALF_LINEHEIGHT);

		// Token: 0x04002EC6 RID: 11974
		private static readonly Vector2 LEADERBOARD_SIZE = new Vector2(71f, 71f);

		// Token: 0x04002EC7 RID: 11975
		private static readonly Vector2 OPTIONS_SIZE = new Vector2(71f, 71f);

		// Token: 0x04002EC8 RID: 11976
		private static readonly Vector2 LEADERBOARD_POSITION = new Vector2(SubMenuMain.ONLINE_POSITION.X + SubMenuMain.ONLINE_SIZE.X - (SubMenuMain.LEADERBOARD_SIZE.X + 12f + SubMenuMain.OPTIONS_SIZE.X) - 25f, SubMenu.BACK_POSITION.Y);

		// Token: 0x04002EC9 RID: 11977
		private static readonly Vector2 OPTIONS_POSITION = new Vector2(SubMenuMain.LEADERBOARD_POSITION.X + SubMenuMain.LEADERBOARD_SIZE.X + 12f, SubMenuMain.LEADERBOARD_POSITION.Y);

		// Token: 0x04002ECA RID: 11978
		private OptionsMessageBox mRUSure;

		// Token: 0x04002ECB RID: 11979
		private bool mFirstTimeOnEnter = true;

		// Token: 0x04002ECC RID: 11980
		private bool mHasMythosLicense;

		// Token: 0x04002ECD RID: 11981
		private MenuImageTextItem mLockItem_Mythos;

		// Token: 0x04002ECE RID: 11982
		private MenuImageTextItem mNewItem_Mythos;

		// Token: 0x04002ECF RID: 11983
		private bool mHasVietnamLicense;

		// Token: 0x04002ED0 RID: 11984
		private bool mHasOSOTCLicense;

		// Token: 0x04002ED1 RID: 11985
		private bool mHasDUNG1License;

		// Token: 0x04002ED2 RID: 11986
		private bool mHasDUNG2License;

		// Token: 0x04002ED3 RID: 11987
		private MenuImageTextItem mLockItem_Vietnam;

		// Token: 0x04002ED4 RID: 11988
		private MenuImageTextItem mLockItem_OSOTC;

		// Token: 0x04002ED5 RID: 11989
		private MenuImageTextItem mLockItem_DUNG1;

		// Token: 0x04002ED6 RID: 11990
		private MenuImageTextItem mLockItem_DUNG2;

		// Token: 0x04002ED7 RID: 11991
		private bool mMythosHasNewContent = true;

		// Token: 0x04002ED8 RID: 11992
		private bool mMythosLevelCastle = true;

		// Token: 0x04002ED9 RID: 11993
		private bool mMythosLevelMountain = true;

		// Token: 0x04002EDA RID: 11994
		private bool mMythosLevelRlyeh = true;

		// Token: 0x04002EDB RID: 11995
		private bool mVietnamHasNewContent = true;

		// Token: 0x04002EDC RID: 11996
		private bool mOSOTCHasNewContent = true;

		// Token: 0x04002EDD RID: 11997
		private bool mDungeons1HasNewContent = true;

		// Token: 0x04002EDE RID: 11998
		private bool mDungeons2HasNewContent = true;

		// Token: 0x04002EDF RID: 11999
		private bool mVersusHasNewContent = true;

		// Token: 0x04002EE0 RID: 12000
		private bool mChallangeHasNewContent = true;

		// Token: 0x04002EE1 RID: 12001
		private MenuImageTextItem mNewItem_Vietnam;

		// Token: 0x04002EE2 RID: 12002
		private MenuImageTextItem mNewItem_OSOTC;

		// Token: 0x04002EE3 RID: 12003
		private MenuImageTextItem mNewItem_DUNG1;

		// Token: 0x04002EE4 RID: 12004
		private MenuImageTextItem mNewItem_Challange;

		// Token: 0x04002EE5 RID: 12005
		private MenuImageTextItem mNewItem_Versus;

		// Token: 0x04002EE6 RID: 12006
		private MenuImageTextItem mNewItem_DUNG2;

		// Token: 0x04002EE7 RID: 12007
		protected static Texture2D sNEWPagesTexture;

		// Token: 0x020005A6 RID: 1446
		private enum MenuChoice
		{
			// Token: 0x04002EE9 RID: 12009
			Campaign,
			// Token: 0x04002EEA RID: 12010
			Stars,
			// Token: 0x04002EEB RID: 12011
			VIETNAM,
			// Token: 0x04002EEC RID: 12012
			OSOTC,
			// Token: 0x04002EED RID: 12013
			DUNG1,
			// Token: 0x04002EEE RID: 12014
			DUNG2,
			// Token: 0x04002EEF RID: 12015
			Challenge,
			// Token: 0x04002EF0 RID: 12016
			Versus,
			// Token: 0x04002EF1 RID: 12017
			Online,
			// Token: 0x04002EF2 RID: 12018
			LeaderBoards,
			// Token: 0x04002EF3 RID: 12019
			Options,
			// Token: 0x04002EF4 RID: 12020
			Exit
		}
	}
}
