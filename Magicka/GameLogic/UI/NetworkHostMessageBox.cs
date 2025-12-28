using System;
using System.Text;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.InGameMenus;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using SteamWrapper;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000028 RID: 40
	internal class NetworkHostMessageBox : MessageBox
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600014C RID: 332 RVA: 0x00008A50 File Offset: 0x00006C50
		public static NetworkHostMessageBox Instance
		{
			get
			{
				if (NetworkHostMessageBox.sSingelton == null)
				{
					lock (NetworkHostMessageBox.sSingeltonLock)
					{
						if (NetworkHostMessageBox.sSingelton == null)
						{
							NetworkHostMessageBox.sSingelton = new NetworkHostMessageBox();
						}
					}
				}
				return NetworkHostMessageBox.sSingelton;
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00008AA4 File Offset: 0x00006CA4
		private NetworkHostMessageBox() : base("#menu_opt_online_01".GetHashCodeCustom())
		{
			this.mNameTitle = new Text(32, this.mFont, TextAlign.Center, false);
			this.mNameTitle.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.NAME_HASH));
			this.mName = new StringBuilder(15, 15);
			this.mNameText = new Text(this.mName.Capacity + 2, this.mFont, TextAlign.Left, true, false);
			this.mNameRect.Width = 320;
			this.mNameRect.Height = this.mFont.LineHeight * 2;
			this.mPasswordTitle = new Text(32, this.mFont, TextAlign.Center, false);
			this.mPasswordTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_SETTINGS_PASSWORD));
			this.mPassword = new StringBuilder(10, 10);
			this.mPasswordText = new Text(this.mPassword.Capacity + 2, this.mFont, TextAlign.Left, true, false);
			this.mPasswordRect.Width = 320;
			this.mPasswordRect.Height = this.mFont.LineHeight * 2;
			this.mVACTitle = new Text(32, this.mFont, TextAlign.Center, false);
			this.mVACTitle.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.VAC_HASH));
			this.mVACText = new Text(32, this.mFont, TextAlign.Center, false);
			this.mVACText.SetText(LanguageManager.Instance.GetString(InGameMenu.LOC_ON));
			this.mVACRect.Width = (int)Math.Max(this.mFont.MeasureText(this.mVACTitle.Characters, true).X, this.mFont.MeasureText(this.mVACText.Characters, true).X);
			this.mVACRect.Height = 2 * this.mFont.LineHeight;
			this.mName.Append(GlobalSettings.Instance.GameName);
			this.mNameText.SetText(this.mName.ToString());
			Vector2 iPosition = default(Vector2);
			this.mCancelButton = new MenuTextItem(Defines.LOC_GEN_CANCEL, iPosition, this.mFont, TextAlign.Left);
			this.mModeTitle = new Text(32, this.mFont, TextAlign.Center, false);
			this.mModeTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_MODE));
			this.mGameType = GameType.Challenge;
			this.mModeText = new Text(35, this.mFont, TextAlign.Center, false);
			this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
			this.mModeRect.Width = 320;
			this.mModeRect.Height = this.mFont.LineHeight * 2;
			this.mOkButton = new MenuTextItem(Defines.LOC_GEN_OK, iPosition, this.mFont, TextAlign.Right);
			this.mNameError = new OptionsMessageBox(LanguageManager.Instance.GetString("#add_menu_err".GetHashCodeCustom()) + LanguageManager.Instance.GetString("#add_menu_err_gamename".GetHashCodeCustom()), new string[]
			{
				LanguageManager.Instance.GetString(Defines.LOC_GEN_OK)
			});
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00008DE0 File Offset: 0x00006FE0
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mMessage.SetText(LanguageManager.Instance.GetString("#menu_opt_online_01".GetHashCodeCustom()));
			this.mNameTitle.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.NAME_HASH));
			this.mPasswordTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_SETTINGS_PASSWORD));
			this.mVACTitle.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.VAC_HASH));
			this.mVACText.SetText(LanguageManager.Instance.GetString(this.mVAC ? InGameMenu.LOC_ON : InGameMenu.LOC_OFF));
			this.mVACRect.Width = (int)Math.Max(this.mFont.MeasureText(this.mVACTitle.Characters, true).X, this.mFont.MeasureText(this.mVACText.Characters, true).X);
			GameType gameType = this.mGameType;
			switch (gameType)
			{
			case GameType.Campaign:
				this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
				break;
			case GameType.Challenge:
				this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
				break;
			case (GameType)3:
				break;
			case GameType.Versus:
				this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
				break;
			default:
				if (gameType != GameType.Mythos)
				{
					if (gameType == GameType.StoryChallange)
					{
						switch (this.storyChallengeType)
						{
						case NetworkHostMessageBox.StoryChallengeType.Vietnam:
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
							break;
						case NetworkHostMessageBox.StoryChallengeType.OSOTC:
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
							break;
						case NetworkHostMessageBox.StoryChallengeType.DUNG1:
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
							break;
						case NetworkHostMessageBox.StoryChallengeType.DUNG2:
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
							break;
						}
					}
				}
				else
				{
					this.mModeText.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_MYTHOS));
				}
				break;
			}
			this.mModeTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_MODE));
			this.mNameError = new OptionsMessageBox(LanguageManager.Instance.GetString("#add_menu_err".GetHashCodeCustom()) + LanguageManager.Instance.GetString("#add_menu_err_gamename".GetHashCodeCustom()), new string[]
			{
				LanguageManager.Instance.GetString(Defines.LOC_GEN_OK)
			});
			this.mCancelButton.LanguageChanged();
			this.mOkButton.LanguageChanged();
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00009075 File Offset: 0x00007275
		public override void Show()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000907C File Offset: 0x0000727C
		public void Show(NetworkHostMessageBox.Complete iOnComplete)
		{
			NetworkHostMessageBox.mHasVietnamLicense = HackHelper.CheckLicenseVietnam();
			NetworkHostMessageBox.mHasOSOTCLicense = HackHelper.CheckLicenseOSOTC();
			NetworkHostMessageBox.mHasDUNG1License = HackHelper.CheckLicenseDungeons1();
			NetworkHostMessageBox.mHasDUNG2License = HackHelper.CheckLicenseDungeons2();
			NetworkHostMessageBox.mHasMythosLicense = HackHelper.CheckLicenseMythos();
			if (HackHelper.LicenseStatus != HackHelper.Status.Valid)
			{
				this.mVAC = false;
				this.mVACText.SetText(LanguageManager.Instance.GetString(InGameMenu.LOC_OFF));
			}
			this.mComplete = (NetworkHostMessageBox.Complete)Delegate.Combine(this.mComplete, iOnComplete);
			this.mSelectedPosition = -1;
			this.mEditPassword = false;
			this.mPassword.Length = 0;
			this.mPasswordText.SetText("");
			base.Show();
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000912B File Offset: 0x0000732B
		public override void Kill()
		{
			this.mComplete = null;
			base.Kill();
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000913C File Offset: 0x0000733C
		public override void OnTextInput(char iChar)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			if (this.mEditName)
			{
				if (iChar == '\b')
				{
					if (this.mName.Length > 0)
					{
						this.mName.Length--;
					}
				}
				else if (this.mName.Length < this.mName.Capacity & !(this.mName.Length == 0 & iChar == ' '))
				{
					this.mName.Append(iChar);
				}
				this.mNameText.SetText(this.mName.ToString());
				return;
			}
			if (this.mEditPassword)
			{
				if (iChar == '\b')
				{
					if (this.mPassword.Length > 0)
					{
						this.mPassword.Length--;
					}
				}
				else if (this.mPassword.Length < this.mPassword.Capacity & !(this.mPassword.Length == 0 & iChar == ' '))
				{
					this.mPassword.Append(iChar);
				}
				this.mPasswordText.SetText(this.mPassword.ToString());
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000925C File Offset: 0x0000745C
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			switch (iDirection)
			{
			case ControllerDirection.Right:
				if (this.mSelectedPosition == 1)
				{
					this.ScrollGameMode_Right();
					return;
				}
				if (this.mSelectedPosition == 4)
				{
					this.mSelectedPosition = 5;
					return;
				}
				if (this.mSelectedPosition == 5)
				{
					this.mSelectedPosition = 4;
				}
				break;
			case ControllerDirection.Up:
			{
				int num = this.mSelectedPosition - 1;
				if (num < 0)
				{
					num += 6;
				}
				this.mSelectedPosition = num;
				return;
			}
			case ControllerDirection.UpRight:
				break;
			case ControllerDirection.Left:
				if (this.mSelectedPosition == 1)
				{
					this.ScrollGameMode_Left();
					return;
				}
				if (this.mSelectedPosition == 4)
				{
					this.mSelectedPosition = 5;
					return;
				}
				if (this.mSelectedPosition == 5)
				{
					this.mSelectedPosition = 4;
					return;
				}
				break;
			default:
			{
				if (iDirection != ControllerDirection.Down)
				{
					return;
				}
				int num2 = this.mSelectedPosition + 1;
				if (num2 >= 6)
				{
					num2 -= 6;
				}
				this.mSelectedPosition = num2;
				return;
			}
			}
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00009328 File Offset: 0x00007528
		private void ScrollGameMode_Left()
		{
			GameType gameType = this.mGameType;
			switch (gameType)
			{
			case GameType.Campaign:
				if (NetworkHostMessageBox.mHasDUNG1License)
				{
					this.mGameType = GameType.StoryChallange;
					this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG1;
					this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
					return;
				}
				if (NetworkHostMessageBox.mHasDUNG2License)
				{
					this.mGameType = GameType.StoryChallange;
					this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
					this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
					return;
				}
				if (NetworkHostMessageBox.mHasOSOTCLicense)
				{
					this.mGameType = GameType.StoryChallange;
					this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
					this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
					return;
				}
				if (NetworkHostMessageBox.mHasVietnamLicense)
				{
					this.mGameType = GameType.StoryChallange;
					this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
					this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
					return;
				}
				this.mGameType = GameType.Versus;
				this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
				this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
				return;
			case GameType.Challenge:
				if (NetworkHostMessageBox.mHasMythosLicense)
				{
					this.mGameType = GameType.Mythos;
					this.mModeText.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_MYTHOS));
					return;
				}
				this.mGameType = GameType.Campaign;
				this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
				return;
			case (GameType)3:
				break;
			case GameType.Versus:
				this.mGameType = GameType.Challenge;
				this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
				return;
			default:
				if (gameType != GameType.Mythos)
				{
					if (gameType != GameType.StoryChallange)
					{
						return;
					}
					switch (this.storyChallengeType)
					{
					case NetworkHostMessageBox.StoryChallengeType.Vietnam:
						this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
						this.mGameType = GameType.Versus;
						this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
						return;
					case NetworkHostMessageBox.StoryChallengeType.OSOTC:
						if (NetworkHostMessageBox.mHasVietnamLicense)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
							return;
						}
						this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
						this.mGameType = GameType.Versus;
						this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
						return;
					case NetworkHostMessageBox.StoryChallengeType.DUNG1:
						if (NetworkHostMessageBox.mHasDUNG2License)
						{
							this.mGameType = GameType.StoryChallange;
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
							return;
						}
						if (NetworkHostMessageBox.mHasOSOTCLicense)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
							return;
						}
						if (NetworkHostMessageBox.mHasVietnamLicense)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
							return;
						}
						this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
						this.mGameType = GameType.Versus;
						this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
						return;
					case NetworkHostMessageBox.StoryChallengeType.DUNG2:
						if (NetworkHostMessageBox.mHasOSOTCLicense)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
							return;
						}
						if (NetworkHostMessageBox.mHasVietnamLicense)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
							return;
						}
						this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
						this.mGameType = GameType.Versus;
						this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
						return;
					default:
						return;
					}
				}
				else
				{
					this.mGameType = GameType.Campaign;
					this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
				}
				break;
			}
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00009670 File Offset: 0x00007870
		private void ScrollGameMode_Right()
		{
			GameType gameType = this.mGameType;
			switch (gameType)
			{
			case GameType.Campaign:
				if (NetworkHostMessageBox.mHasMythosLicense)
				{
					this.mGameType = GameType.Mythos;
					this.mModeText.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_MYTHOS));
					return;
				}
				this.mGameType = GameType.Challenge;
				this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
				return;
			case GameType.Challenge:
				this.mGameType = GameType.Versus;
				this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
				return;
			case (GameType)3:
				break;
			case GameType.Versus:
				if (NetworkHostMessageBox.mHasVietnamLicense)
				{
					this.mGameType = GameType.StoryChallange;
					this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
					this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
					return;
				}
				if (NetworkHostMessageBox.mHasOSOTCLicense)
				{
					this.mGameType = GameType.StoryChallange;
					this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
					this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
					return;
				}
				if (NetworkHostMessageBox.mHasDUNG1License)
				{
					this.mGameType = GameType.StoryChallange;
					this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG1;
					this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
					return;
				}
				if (NetworkHostMessageBox.mHasDUNG2License)
				{
					this.mGameType = GameType.StoryChallange;
					this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
					this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
					return;
				}
				this.mGameType = GameType.Campaign;
				this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
				this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
				return;
			default:
				if (gameType != GameType.Mythos)
				{
					if (gameType != GameType.StoryChallange)
					{
						return;
					}
					switch (this.storyChallengeType)
					{
					case NetworkHostMessageBox.StoryChallengeType.Vietnam:
						if (NetworkHostMessageBox.mHasOSOTCLicense)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
							return;
						}
						if (NetworkHostMessageBox.mHasDUNG1License)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG1;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
							return;
						}
						if (NetworkHostMessageBox.mHasDUNG2License)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
							return;
						}
						this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
						this.mGameType = GameType.Campaign;
						this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
						return;
					case NetworkHostMessageBox.StoryChallengeType.OSOTC:
						if (NetworkHostMessageBox.mHasDUNG1License)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG1;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
							return;
						}
						if (NetworkHostMessageBox.mHasDUNG2License)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
							return;
						}
						this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
						this.mGameType = GameType.Campaign;
						this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
						return;
					case NetworkHostMessageBox.StoryChallengeType.DUNG1:
						if (NetworkHostMessageBox.mHasDUNG2License)
						{
							this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
							this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
							return;
						}
						this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
						this.mGameType = GameType.Campaign;
						this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
						return;
					case NetworkHostMessageBox.StoryChallengeType.DUNG2:
						this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
						this.mGameType = GameType.Campaign;
						this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
						return;
					default:
						return;
					}
				}
				else
				{
					this.mGameType = GameType.Challenge;
					this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
				}
				break;
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x000099B0 File Offset: 0x00007BB0
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			if (this.mNameRect.Contains(iNewState.X, iNewState.Y))
			{
				this.mSelectedPosition = 0;
				return;
			}
			if (this.mPasswordRect.Contains(iNewState.X, iNewState.Y))
			{
				this.mSelectedPosition = 3;
				return;
			}
			if (this.mVACRect.Contains(iNewState.X, iNewState.Y))
			{
				this.mSelectedPosition = 2;
				return;
			}
			if (this.mModeRect.Contains(iNewState.X, iNewState.Y))
			{
				this.mSelectedPosition = 1;
				return;
			}
			if (this.mOkButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mSelectedPosition = 4;
				return;
			}
			if (this.mCancelButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mSelectedPosition = 5;
				return;
			}
			this.mSelectedPosition = -1;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00009A9C File Offset: 0x00007C9C
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
			{
				if (this.mNameRect.Contains(iNewState.X, iNewState.Y))
				{
					this.mSelectedPosition = 0;
				}
				else if (this.mModeRect.Contains(iNewState.X, iNewState.Y))
				{
					this.mSelectedPosition = 1;
				}
				else if (this.mPasswordRect.Contains(iNewState.X, iNewState.Y))
				{
					this.mSelectedPosition = 3;
				}
				else if (this.mVACRect.Contains(iNewState.X, iNewState.Y))
				{
					this.mSelectedPosition = 2;
				}
				else if (this.mOkButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
				{
					this.mSelectedPosition = 4;
				}
				else if (this.mCancelButton.InsideBounds((float)iNewState.X, (float)iNewState.Y))
				{
					this.mSelectedPosition = 5;
				}
				else
				{
					this.mSelectedPosition = -1;
				}
				this.OnSelect(ControlManager.Instance.MenuController);
			}
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00009BBC File Offset: 0x00007DBC
		private void DaisyWheelInput(Controller iSender)
		{
			NetworkHostMessageBox.Items items = (NetworkHostMessageBox.Items)this.mSelectedPosition;
			if (items != NetworkHostMessageBox.Items.Name)
			{
				if (items != NetworkHostMessageBox.Items.Password)
				{
					return;
				}
				this.mEditPassword = true;
				string iDescr = LanguageManager.Instance.GetString(NetworkHostMessageBox.LOC_PASSWORD).ToUpper();
				DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelInputRecived_Psw));
				if (!DaisyWheel.TryShow(iSender, iDescr, false, GamepadTextInputLineMode.GamepadTextInputLineModeSingleLine, 11U))
				{
					DaisyWheel.SetActionToCallWhenComplete(null);
					return;
				}
				this.mPassword = new StringBuilder("");
				this.mPasswordText.SetText("");
				return;
			}
			else
			{
				this.mEditName = true;
				string iDescr2 = LanguageManager.Instance.GetString(NetworkHostMessageBox.LOC_NAME).ToUpper();
				DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelInputRecived_Name));
				if (!DaisyWheel.TryShow(iSender, iDescr2, false))
				{
					DaisyWheel.SetActionToCallWhenComplete(null);
					return;
				}
				this.mName = new StringBuilder("");
				this.mNameText.SetText("");
				return;
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00009C99 File Offset: 0x00007E99
		private void DaisyWheelInputRecived_Name(string name)
		{
			DaisyWheel.SetActionToCallWhenComplete(null);
			this.mName = new StringBuilder(name);
			this.mNameText.SetText(name);
			this.mEditName = false;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00009CC0 File Offset: 0x00007EC0
		private void DaisyWheelInputRecived_Psw(string psw)
		{
			DaisyWheel.SetActionToCallWhenComplete(null);
			this.mPassword = new StringBuilder(psw);
			this.mPasswordText.SetText(psw);
			this.mEditPassword = false;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00009CE8 File Offset: 0x00007EE8
		public override void OnSelect(Controller iSender)
		{
			if (this.mEditName)
			{
				this.mEditName = false;
				return;
			}
			if (this.mEditPassword)
			{
				this.mEditPassword = false;
				return;
			}
			if (!DaisyWheel.IsDisplaying && iSender is XInputController && (this.mSelectedPosition == 0 || this.mSelectedPosition == 3))
			{
				this.DaisyWheelInput(iSender);
				return;
			}
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			switch (this.mSelectedPosition)
			{
			case 0:
				this.mEditName = true;
				return;
			case 1:
				this.ScrollGameMode_Right();
				return;
			case 2:
				if (HackHelper.LicenseStatus == HackHelper.Status.Valid)
				{
					this.mVAC = !this.mVAC;
					this.mVACText.SetText(LanguageManager.Instance.GetString(this.mVAC ? InGameMenu.LOC_ON : InGameMenu.LOC_OFF));
					return;
				}
				break;
			case 3:
				this.mEditPassword = true;
				return;
			case 4:
				if ((this.mGameType == GameType.Campaign | this.mGameType == GameType.Mythos) && LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Pending)
				{
					return;
				}
				if (this.mName.Length <= 0)
				{
					this.mNameError.Show();
					return;
				}
				if (this.mVAC && (this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos) && LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Hacked)
				{
					OptionsMessageBox optionsMessageBox = new OptionsMessageBox("#notice_mod_campaign_vac".GetHashCode(), new int[]
					{
						"#add_menu_ok".GetHashCode()
					});
					LanguageManager.Instance.LanguageChanged -= new Action(optionsMessageBox.LanguageChanged);
					optionsMessageBox.Show();
					return;
				}
				if (this.mComplete != null)
				{
					if (string.IsNullOrEmpty(this.mPassword.ToString()))
					{
						this.mComplete(this.mGameType, this.storyChallengeType, this.mName.ToString(), this.mVAC, null);
					}
					else
					{
						this.mComplete(this.mGameType, this.storyChallengeType, this.mName.ToString(), this.mVAC, this.mPassword.ToString());
					}
				}
				SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
				GlobalSettings.Instance.GameName = this.mName.ToString();
				SaveManager.Instance.SaveSettings();
				this.Kill();
				return;
			case 5:
				this.Kill();
				break;
			default:
				return;
			}
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00009F20 File Offset: 0x00008120
		public override void Draw(float iDeltaTime)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			base.Draw(iDeltaTime);
			this.mTimer -= iDeltaTime;
			while (this.mTimer < 0f)
			{
				this.mTimer += 0.5f;
				this.mLine = !this.mLine;
				if (this.mEditName)
				{
					if (this.mLine)
					{
						this.mNameText.Characters[this.mName.Length] = '_';
						this.mNameText.Characters[this.mName.Length + 1] = '\0';
					}
					else
					{
						this.mNameText.Characters[this.mName.Length] = '\0';
					}
				}
				else
				{
					this.mNameText.Characters[this.mName.Length] = '\0';
				}
				if (this.mEditPassword)
				{
					if (this.mLine)
					{
						this.mPasswordText.Characters[this.mPassword.Length] = '_';
						this.mPasswordText.Characters[this.mPassword.Length + 1] = '\0';
					}
					else
					{
						this.mPasswordText.Characters[this.mPassword.Length] = '\0';
					}
				}
				else
				{
					this.mPasswordText.Characters[this.mPassword.Length] = '\0';
				}
				this.mPasswordText.MarkAsDirty();
				this.mNameText.MarkAsDirty();
			}
			float num = (float)this.mFont.LineHeight;
			float num2 = (float)Math.Floor((double)(num * 1.5f));
			Vector4 color = MenuItem.COLOR;
			color.W *= this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			Vector2 mCenter = this.mCenter;
			mCenter.Y -= this.mSize.Y * 0.5f - 48f;
			this.mMessage.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			mCenter.Y += num2;
			this.mNameRect.X = (int)mCenter.X - 160;
			this.mNameRect.Y = (int)mCenter.Y;
			color = ((this.mSelectedPosition == 0) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			color.W *= this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			this.mNameTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			mCenter.Y += num;
			float x = this.mNameText.Font.MeasureText(this.mNameText.Characters, true, this.mName.Length).X;
			Matrix matrix = default(Matrix);
			float num3 = Math.Min(320f / x, 1f);
			matrix.M11 = num3;
			matrix.M22 = 1f;
			matrix.M44 = 1f;
			matrix.M41 = mCenter.X - x * num3 * 0.5f + 0.5f;
			matrix.M42 = mCenter.Y;
			color = ((this.mSelectedPosition == 0 | this.mEditName) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			color.W *= this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			this.mNameText.Draw(MessageBox.sGUIBasicEffect, ref matrix);
			mCenter.Y += num2;
			color = ((this.mSelectedPosition == 1) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			color.W *= this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			this.mModeRect.X = (int)mCenter.X - 160;
			this.mModeRect.Y = (int)mCenter.Y;
			this.mModeTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			mCenter.Y += num;
			this.mModeText.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			mCenter.Y += num2;
			color = ((HackHelper.LicenseStatus == HackHelper.Status.Valid && this.mSelectedPosition == 2) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			color.W *= this.mAlpha;
			if (HackHelper.LicenseStatus != HackHelper.Status.Valid)
			{
				color.W *= 0.5f;
			}
			MessageBox.sGUIBasicEffect.Color = color;
			this.mVACRect.X = (int)mCenter.X - 160;
			this.mVACRect.Y = (int)mCenter.Y;
			this.mVACRect.Width = 320;
			this.mVACTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			mCenter.Y += num;
			this.mVACText.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			mCenter.Y += num2;
			this.mPasswordRect.X = (int)mCenter.X - 160;
			this.mPasswordRect.Y = (int)mCenter.Y;
			this.mPasswordRect.Width = 320;
			color = ((this.mSelectedPosition == 3) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			color.W *= this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			this.mPasswordTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
			mCenter.Y += num;
			x = this.mPasswordText.Font.MeasureText(this.mPasswordText.Characters, true, this.mPassword.Length).X;
			color = ((this.mSelectedPosition == 3 | this.mEditPassword) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			color.W *= this.mAlpha * this.mAlpha;
			MessageBox.sGUIBasicEffect.Color = color;
			this.mPasswordText.Draw(MessageBox.sGUIBasicEffect, mCenter.X - (float)Math.Floor((double)(x * 0.5f)), mCenter.Y);
			this.mOkButton.Selected = (this.mSelectedPosition == 4);
			mCenter.X = this.mCenter.X - 16f;
			mCenter.Y = this.mCenter.Y + this.mSize.Y * 0.5f - 64f;
			this.mOkButton.Position = mCenter;
			this.mOkButton.Alpha = this.mAlpha;
			this.mOkButton.Draw(MessageBox.sGUIBasicEffect);
			this.mCancelButton.Selected = (this.mSelectedPosition == 5);
			mCenter.X = this.mCenter.X + 16f;
			this.mCancelButton.Position = mCenter;
			this.mCancelButton.Alpha = this.mAlpha;
			this.mCancelButton.Draw(MessageBox.sGUIBasicEffect);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x040000E7 RID: 231
		public const float MAX_WIDTH = 320f;

		// Token: 0x040000E8 RID: 232
		private static NetworkHostMessageBox sSingelton;

		// Token: 0x040000E9 RID: 233
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040000EA RID: 234
		public static readonly int GAMETYPE_ADVENTURE_HASH = SubMenu.LOC_ADVENTURE;

		// Token: 0x040000EB RID: 235
		public static readonly int GAMETYPE_CHALLENGE_HASH = SubMenuOnline.LOC_CHALLENGE;

		// Token: 0x040000EC RID: 236
		public static readonly int GAMETYPE_STORYCHALLANGE_HASH = SubMenuOnline.LOC_STORYCHALLANGE;

		// Token: 0x040000ED RID: 237
		public static readonly int GAMETYPE_VERSUS_HASH = "#menu_main_03".GetHashCodeCustom();

		// Token: 0x040000EE RID: 238
		public static readonly int VAC_HASH = "#network_27".GetHashCodeCustom();

		// Token: 0x040000EF RID: 239
		private static readonly int NAME_HASH = "#add_menu_gamename".GetHashCodeCustom();

		// Token: 0x040000F0 RID: 240
		private static readonly int PORT_HASH = "#add_menu_port".GetHashCodeCustom();

		// Token: 0x040000F1 RID: 241
		private static bool mHasOSOTCLicense = false;

		// Token: 0x040000F2 RID: 242
		private static bool mHasDUNG1License = false;

		// Token: 0x040000F3 RID: 243
		private static bool mHasDUNG2License = false;

		// Token: 0x040000F4 RID: 244
		private static bool mHasVietnamLicense = false;

		// Token: 0x040000F5 RID: 245
		private static bool mHasMythosLicense = false;

		// Token: 0x040000F6 RID: 246
		private static readonly int LOC_NAME = "#add_menu_prof_name".GetHashCodeCustom();

		// Token: 0x040000F7 RID: 247
		private static readonly int LOC_PASSWORD = "#settings_p01_password".GetHashCodeCustom();

		// Token: 0x040000F8 RID: 248
		private NetworkHostMessageBox.StoryChallengeType storyChallengeType;

		// Token: 0x040000F9 RID: 249
		private bool mEditName;

		// Token: 0x040000FA RID: 250
		private int mSelectedPosition;

		// Token: 0x040000FB RID: 251
		private Text mNameTitle;

		// Token: 0x040000FC RID: 252
		private Text mNameText;

		// Token: 0x040000FD RID: 253
		private StringBuilder mName;

		// Token: 0x040000FE RID: 254
		private Rectangle mNameRect;

		// Token: 0x040000FF RID: 255
		private bool mEditPassword;

		// Token: 0x04000100 RID: 256
		private Text mPasswordTitle;

		// Token: 0x04000101 RID: 257
		private Text mPasswordText;

		// Token: 0x04000102 RID: 258
		private StringBuilder mPassword;

		// Token: 0x04000103 RID: 259
		private Rectangle mPasswordRect;

		// Token: 0x04000104 RID: 260
		private bool mVAC = true;

		// Token: 0x04000105 RID: 261
		private Text mVACText;

		// Token: 0x04000106 RID: 262
		private Text mVACTitle;

		// Token: 0x04000107 RID: 263
		private Rectangle mVACRect;

		// Token: 0x04000108 RID: 264
		private Text mModeText;

		// Token: 0x04000109 RID: 265
		private Text mModeTitle;

		// Token: 0x0400010A RID: 266
		private Rectangle mModeRect;

		// Token: 0x0400010B RID: 267
		private GameType mGameType = GameType.Challenge;

		// Token: 0x0400010C RID: 268
		private MenuTextItem mCancelButton;

		// Token: 0x0400010D RID: 269
		private MenuTextItem mOkButton;

		// Token: 0x0400010E RID: 270
		private float mTimer;

		// Token: 0x0400010F RID: 271
		private bool mLine;

		// Token: 0x04000110 RID: 272
		private NetworkHostMessageBox.Complete mComplete;

		// Token: 0x04000111 RID: 273
		private OptionsMessageBox mNameError;

		// Token: 0x02000029 RID: 41
		public enum StoryChallengeType
		{
			// Token: 0x04000113 RID: 275
			None,
			// Token: 0x04000114 RID: 276
			Vietnam,
			// Token: 0x04000115 RID: 277
			OSOTC,
			// Token: 0x04000116 RID: 278
			DUNG1,
			// Token: 0x04000117 RID: 279
			DUNG2
		}

		// Token: 0x0200002A RID: 42
		private enum Items
		{
			// Token: 0x04000119 RID: 281
			Invalid = -1,
			// Token: 0x0400011A RID: 282
			Name,
			// Token: 0x0400011B RID: 283
			Mode,
			// Token: 0x0400011C RID: 284
			VAC,
			// Token: 0x0400011D RID: 285
			Password,
			// Token: 0x0400011E RID: 286
			Ok,
			// Token: 0x0400011F RID: 287
			Cancel,
			// Token: 0x04000120 RID: 288
			NrOfItems
		}

		// Token: 0x0200002B RID: 43
		// (Invoke) Token: 0x0600015F RID: 351
		public delegate void Complete(GameType iType, NetworkHostMessageBox.StoryChallengeType iStoryChallangeType, string iName, bool iVAC, string iPassword);
	}
}
