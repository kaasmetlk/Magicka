using System;
using System.Collections.Generic;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Magicka.Network;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x02000494 RID: 1172
	internal sealed class SubMenuCampaignSelect_SaveSlotSelect : SubMenu
	{
		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x06002380 RID: 9088 RVA: 0x000FF364 File Offset: 0x000FD564
		public static SubMenuCampaignSelect_SaveSlotSelect Instance
		{
			get
			{
				if (SubMenuCampaignSelect_SaveSlotSelect.sSingelton == null)
				{
					lock (SubMenuCampaignSelect_SaveSlotSelect.sSingeltonLock)
					{
						if (SubMenuCampaignSelect_SaveSlotSelect.sSingelton == null)
						{
							SubMenuCampaignSelect_SaveSlotSelect.sSingelton = new SubMenuCampaignSelect_SaveSlotSelect();
						}
					}
				}
				return SubMenuCampaignSelect_SaveSlotSelect.sSingelton;
			}
		}

		// Token: 0x06002381 RID: 9089 RVA: 0x000FF3B8 File Offset: 0x000FD5B8
		private SubMenuCampaignSelect_SaveSlotSelect()
		{
			this.mMenuItems = new List<MenuItem>();
			this.mPosition.Y = 192f;
			Vector2 mPosition = this.mPosition;
			mPosition.X += 384f;
			mPosition.Y -= 24f;
			this.mMenuItems.Add(new MenuImageTextItem(mPosition, SubMenu.sPagesTexture, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVOFFSET, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVSIZE, 0, default(Vector2), TextAlign.Center, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE));
			mPosition.Y += 256f;
			this.mMenuItems.Add(new MenuImageTextItem(mPosition, SubMenu.sPagesTexture, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVOFFSET, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVSIZE, 0, default(Vector2), TextAlign.Center, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE));
			mPosition.Y += 256f;
			this.mMenuItems.Add(new MenuImageTextItem(mPosition, SubMenu.sPagesTexture, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVOFFSET, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVSIZE, 0, default(Vector2), TextAlign.Center, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE));
			mPosition = this.mPosition;
			for (int i = 0; i < 3; i++)
			{
				this.mMenuItems.Add(new SaveSlot(mPosition, SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP, 0, 0, false, 0UL, false));
				mPosition.Y += 256f;
			}
			this.mRUSure = new OptionsMessageBox("#add_menu_rus_del".GetHashCodeCustom(), new int[]
			{
				Defines.LOC_GEN_YES,
				Defines.LOC_GEN_NO
			});
			this.mRUSure.Select += new Action<OptionsMessageBox, int>(this.DeleteCallback);
			string @string = LanguageManager.Instance.GetString("#network_23".GetHashCodeCustom());
			this.mLoadingTextLength = @string.Length;
			this.mLoadingText.SetText(@string);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
		}

		// Token: 0x06002382 RID: 9090 RVA: 0x000FF624 File Offset: 0x000FD824
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			string @string = LanguageManager.Instance.GetString(LevelMessageBox.LOC_LOADING);
			this.mLoadingTextLength = @string.Length;
			this.mLoadingText.SetText(@string);
		}

		// Token: 0x06002383 RID: 9091 RVA: 0x000FF660 File Offset: 0x000FD860
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (this.mGameType == GameType.Mythos && LevelManager.Instance.MythosCampaignLicense == HackHelper.License.No)
			{
				Tome.Instance.PopMenu();
			}
			this.mLoadingDotTimer -= iDeltaTime;
			if (!this.mHasNotifiedHax && LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Hacked)
			{
				this.mHasNotifiedHax = true;
				OptionsMessageBox optionsMessageBox = new OptionsMessageBox("#notice_mod_campaign".GetHashCode(), new int[]
				{
					"#add_menu_ok".GetHashCode()
				});
				LanguageManager.Instance.LanguageChanged -= new Action(optionsMessageBox.LanguageChanged);
				optionsMessageBox.Show();
			}
			Vector2 mPosition = this.mPosition;
			mPosition.X += 340f;
			mPosition.Y += 44f;
			for (int i = 0; i < 3; i++)
			{
				this.mMenuItems[i].Position = mPosition;
				this.mMenuItems[i].Enabled = !(this.mMenuItems[i + 3] as SaveSlot).EmptySlot;
				mPosition.Y += 256f;
			}
		}

		// Token: 0x06002384 RID: 9092 RVA: 0x000FF78C File Offset: 0x000FD98C
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
			this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
			this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			if (LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Pending)
			{
				while (this.mLoadingDotTimer < 0f)
				{
					this.mLoadingDotTimer += 0.5f;
					int num;
					if (this.mLoadingText.Characters[this.mLoadingTextLength] == '\0')
					{
						num = this.mLoadingTextLength;
					}
					else if (this.mLoadingText.Characters[this.mLoadingTextLength + 1] == '\0')
					{
						num = this.mLoadingTextLength + 1;
					}
					else if (this.mLoadingText.Characters[this.mLoadingTextLength + 2] == '\0')
					{
						num = this.mLoadingTextLength + 2;
					}
					else
					{
						num = -1;
						this.mLoadingText.Characters[this.mLoadingTextLength] = '\0';
					}
					if (num > 0)
					{
						this.mLoadingText.Characters[num] = '.';
						this.mLoadingText.Characters[num + 1] = '\0';
					}
					this.mLoadingText.MarkAsDirty();
				}
				Vector2 vector = this.mLoadingText.Font.MeasureText(this.mLoadingText.Characters, true, this.mLoadingTextLength);
				this.mLoadingText.Draw(this.mEffect, 512f - vector.X * 0.5f, 850f);
			}
			for (int i = 3; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].Draw(this.mEffect);
			}
			for (int j = 0; j < 3; j++)
			{
				if (this.mMenuItems[j].Enabled)
				{
					this.mMenuItems[j].Draw(this.mEffect);
				}
			}
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06002385 RID: 9093 RVA: 0x000FF9C0 File Offset: 0x000FDBC0
		public void Delete(Controller iSender)
		{
			if (NetworkManager.Instance.State == NetworkState.Client | this.mSelectedPosition < 3)
			{
				return;
			}
			if (this.mSaveData[this.mSelectedPosition - 3] != null)
			{
				this.mRUSure.SelectedIndex = 1;
				this.mRUSure.Show();
			}
		}

		// Token: 0x06002386 RID: 9094 RVA: 0x000FFA10 File Offset: 0x000FDC10
		private void DeleteCallback(MessageBox iSender, int iSelection)
		{
			if (iSelection == 0)
			{
				if (this.mSelectedPosition < 3)
				{
					this.mSaveData[this.mSelectedPosition] = null;
					(this.mMenuItems[this.mSelectedPosition + 3] as SaveSlot).Set(SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP, 0, 0, false, 0UL, false);
				}
				else
				{
					this.mSaveData[this.mSelectedPosition - 3] = null;
					(this.mMenuItems[this.mSelectedPosition] as SaveSlot).Set(SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP, 0, 0, false, 0UL, false);
				}
				SaveManager.Instance.SaveCampaign();
			}
		}

		// Token: 0x06002387 RID: 9095 RVA: 0x000FFAA4 File Offset: 0x000FDCA4
		public override void ControllerRight(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 0:
			case 1:
			case 2:
				this.mSelectedPosition += 3;
				break;
			case 3:
			case 4:
			case 5:
				this.mSelectedPosition -= 3;
				break;
			default:
				this.mSelectedPosition = this.mMenuItems.Count - 1;
				break;
			}
			base.ControllerRight(iSender);
		}

		// Token: 0x06002388 RID: 9096 RVA: 0x000FFB14 File Offset: 0x000FDD14
		public override void ControllerLeft(Controller iSender)
		{
			switch (this.mSelectedPosition)
			{
			case 0:
			case 1:
			case 2:
				this.mSelectedPosition += 3;
				break;
			case 3:
			case 4:
			case 5:
				this.mSelectedPosition -= 3;
				break;
			default:
				this.mSelectedPosition = this.mMenuItems.Count - 1;
				break;
			}
			base.ControllerLeft(iSender);
		}

		// Token: 0x06002389 RID: 9097 RVA: 0x000FFB84 File Offset: 0x000FDD84
		public override void ControllerA(Controller iSender)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			if (this.mGameType == GameType.Mythos && LevelManager.Instance.MythosCampaignLicense == HackHelper.License.No)
			{
				return;
			}
			if (this.mSelectedPosition == this.mMenuItems.Count - 1)
			{
				Tome.Instance.PopMenu();
				this.mCurrentSaveData = null;
				return;
			}
			if (this.mSelectedPosition < 3)
			{
				if (this.mSaveData[this.mSelectedPosition] != null)
				{
					this.mRUSure.SelectedIndex = 1;
					this.mRUSure.Show();
					return;
				}
			}
			else if ((this.mGameType == GameType.Campaign && LevelManager.Instance.CampaignIsHacked != HackHelper.Status.Pending) || (this.mGameType == GameType.Mythos && LevelManager.Instance.MythosCampaignLicense != HackHelper.License.Pending))
			{
				if (this.mSaveData[this.mSelectedPosition - 3] == null)
				{
					SaveData saveData = new SaveData();
					this.mSaveData[this.mSelectedPosition - 3] = saveData;
					SaveManager.Instance.SaveCampaign();
					int hashCodeCustom = this.mLevelNodes[(int)saveData.Level].Name.GetHashCodeCustom();
					int description = this.mLevelNodes[(int)saveData.Level].Description;
					bool looped = saveData.Looped;
					int currentPlayTime = saveData.CurrentPlayTime;
					ulong unlockedMagicks = saveData.UnlockedMagicks;
					(this.mMenuItems[this.mSelectedPosition] as SaveSlot).Set(hashCodeCustom, description, currentPlayTime, looped, unlockedMagicks, this.mGameType == GameType.Mythos);
					this.mCurrentSaveData = this.mSaveData[this.mSelectedPosition - 3];
					SubMenuCharacterSelect.Instance.SetSettings(this.mGameType, (int)this.mCurrentSaveData.Level, false);
					SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
					if (this.mComplete != null)
					{
						this.mComplete.Invoke();
					}
					Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
					return;
				}
				this.mCurrentSaveData = this.mSaveData[this.mSelectedPosition - 3];
				SubMenuCharacterSelect.Instance.SetSettings(this.mGameType, (int)this.mCurrentSaveData.Level, false);
				SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
				LevelNode level = LevelManager.Instance.GetLevel(this.mGameType, (int)this.mCurrentSaveData.Level);
				SubMenuCharacterSelect.Instance.SetLevelUsed(level.Name);
				if (this.mComplete != null)
				{
					this.mComplete.Invoke();
				}
				Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
			}
		}

		// Token: 0x0600238A RID: 9098 RVA: 0x000FFDD0 File Offset: 0x000FDFD0
		public override void ControllerB(Controller iSender)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			this.mCurrentSaveData = null;
			for (int i = 0; i < Game.Instance.Players.Length; i++)
			{
				Player player = Game.Instance.Players[i];
				player.Weapon = "";
				player.Staff = "";
			}
			base.ControllerB(iSender);
		}

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x0600238B RID: 9099 RVA: 0x000FFE33 File Offset: 0x000FE033
		// (set) Token: 0x0600238C RID: 9100 RVA: 0x000FFE3B File Offset: 0x000FE03B
		internal GameType GameType
		{
			get
			{
				return this.mGameType;
			}
			set
			{
				this.mGameType = value;
			}
		}

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x0600238D RID: 9101 RVA: 0x000FFE44 File Offset: 0x000FE044
		internal CampaignNode[] Campaign
		{
			get
			{
				return this.mLevelNodes;
			}
		}

		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x0600238E RID: 9102 RVA: 0x000FFE4C File Offset: 0x000FE04C
		public SaveData CurrentSaveData
		{
			get
			{
				return this.mCurrentSaveData;
			}
		}

		// Token: 0x0600238F RID: 9103 RVA: 0x000FFE54 File Offset: 0x000FE054
		public override void OnEnter()
		{
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			this.mSelectedPosition = 3;
			this.UpdateSlots();
			base.OnEnter();
		}

		// Token: 0x06002390 RID: 9104 RVA: 0x000FFEAA File Offset: 0x000FE0AA
		public override void OnExit()
		{
			this.mComplete = null;
		}

		// Token: 0x06002391 RID: 9105 RVA: 0x000FFEB4 File Offset: 0x000FE0B4
		internal void UpdateSlots()
		{
			if (this.mGameType == GameType.Mythos)
			{
				this.mSaveData = SaveManager.Instance.MythosSaveSlots;
				this.mLevelNodes = LevelManager.Instance.MythosCampaign;
			}
			else
			{
				this.mSaveData = SaveManager.Instance.SaveSlots;
				this.mLevelNodes = LevelManager.Instance.VanillaCampaign;
			}
			for (int i = 0; i < 3; i++)
			{
				int iName = SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP;
				int iDesc = 0;
				int iTime = 0;
				ulong iMagicks = 0UL;
				bool iLooped = false;
				if (this.mSaveData[i] != null)
				{
					SaveData saveData = this.mSaveData[i];
					iName = this.mLevelNodes[(int)saveData.Level].Name.GetHashCodeCustom();
					iDesc = this.mLevelNodes[(int)saveData.Level].Description;
					iTime = saveData.CurrentPlayTime;
					iLooped = saveData.Looped;
					iMagicks = saveData.UnlockedMagicks;
				}
				(this.mMenuItems[i + 3] as SaveSlot).Set(iName, iDesc, iTime, iLooped, iMagicks, this.mGameType == GameType.Mythos);
			}
		}

		// Token: 0x04002693 RID: 9875
		private static SubMenuCampaignSelect_SaveSlotSelect sSingelton;

		// Token: 0x04002694 RID: 9876
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002695 RID: 9877
		private static readonly Vector2 DELETE_UVOFFSET = new Vector2(0.75f, 0.09375f);

		// Token: 0x04002696 RID: 9878
		private static readonly Vector2 DELETE_SIZE = new Vector2(64f, 64f);

		// Token: 0x04002697 RID: 9879
		private static readonly Vector2 DELETE_UVSIZE = new Vector2(SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE.X / 2048f, SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE.Y / 1024f);

		// Token: 0x04002698 RID: 9880
		private int mLoadingTextLength;

		// Token: 0x04002699 RID: 9881
		private Text mLoadingText = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Left, true);

		// Token: 0x0400269A RID: 9882
		private float mLoadingDotTimer = 0.5f;

		// Token: 0x0400269B RID: 9883
		private bool mHasNotifiedHax;

		// Token: 0x0400269C RID: 9884
		public Action mComplete;

		// Token: 0x0400269D RID: 9885
		private GameType mGameType = GameType.Campaign;

		// Token: 0x0400269E RID: 9886
		private SaveData mCurrentSaveData;

		// Token: 0x0400269F RID: 9887
		private SaveData[] mSaveData;

		// Token: 0x040026A0 RID: 9888
		private CampaignNode[] mLevelNodes;

		// Token: 0x040026A1 RID: 9889
		private OptionsMessageBox mRUSure;

		// Token: 0x040026A2 RID: 9890
		public static readonly int NEWCAMP = "#add_menu_newcamp".GetHashCodeCustom();
	}
}
