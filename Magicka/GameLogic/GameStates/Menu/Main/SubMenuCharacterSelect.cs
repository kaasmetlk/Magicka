using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Magicka.Achievements;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Levels.Packs;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.Network;
using Magicka.Storage;
using Magicka.WebTools.Paradox;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using SteamWrapper;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x020005E0 RID: 1504
	internal class SubMenuCharacterSelect : SubMenu
	{
		// Token: 0x17000AA3 RID: 2723
		// (get) Token: 0x06002CED RID: 11501 RVA: 0x001611F8 File Offset: 0x0015F3F8
		public static SubMenuCharacterSelect Instance
		{
			get
			{
				if (SubMenuCharacterSelect.sSingelton == null)
				{
					lock (SubMenuCharacterSelect.sSingeltonLock)
					{
						if (SubMenuCharacterSelect.sSingelton == null)
						{
							SubMenuCharacterSelect.sSingelton = new SubMenuCharacterSelect();
						}
					}
				}
				return SubMenuCharacterSelect.sSingelton;
			}
		}

		// Token: 0x06002CEE RID: 11502 RVA: 0x0016124C File Offset: 0x0015F44C
		public SubMenuCharacterSelect()
		{
			LanguageManager instance = LanguageManager.Instance;
			this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			this.mPlayerSlots = new SubMenuCharacterSelect.PlayerState[4];
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mCustomTexture = Game.Instance.Content.Load<Texture2D>("UI/menu/customAvatar");
				this.mMagicksTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks");
				this.mMapTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/CampaignMap");
				this.mMapMaskTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/MapMask");
				SubMenuCharacterSelect.mControllerTextures = new Texture2D[5];
				SubMenuCharacterSelect.mControllerTextures[0] = Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/keyboard");
				SubMenuCharacterSelect.mControllerTextures[1] = Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/gp1");
				SubMenuCharacterSelect.mControllerTextures[2] = Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/gp2");
				SubMenuCharacterSelect.mControllerTextures[3] = Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/gp3");
				SubMenuCharacterSelect.mControllerTextures[4] = Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/gp4");
			}
			this.mMapRect = default(Rectangle);
			this.mMapRect.Width = (this.mMapRect.Height = 448);
			this.mMapRect.X = 64;
			this.mMapRect.Y = 224;
			this.mValidatingLevels = false;
			Vector4[] array = new Vector4[64];
			Vector2 vector = default(Vector2);
			vector.X = (vector.Y = 16f);
			Vector2 vector2 = default(Vector2);
			vector2.X = 448f;
			vector2.Y = 112f;
			Vector2 vector3 = default(Vector2);
			vector3.X = (vector3.Y = 96f);
			Vector2 vector4 = default(Vector2);
			vector4.X = 832f / (float)SubMenu.sPagesTexture.Width;
			vector4.Y = 128f / (float)SubMenu.sPagesTexture.Height;
			Vector2 vector5 = default(Vector2);
			vector5.X = 128f / (float)SubMenu.sPagesTexture.Width;
			vector5.Y = 128f / (float)SubMenu.sPagesTexture.Height;
			Vector2 vector6 = default(Vector2);
			vector6.X = 16f / (float)SubMenu.sPagesTexture.Width;
			vector6.Y = 16f / (float)SubMenu.sPagesTexture.Height;
			int num = SubMenuCharacterSelect.CreateVertices(array, 0, ref vector2, ref vector, ref vector4, ref vector5, ref vector6);
			num = SubMenuCharacterSelect.CreateVertices(array, num, ref vector3, ref vector, ref vector4, ref vector5, ref vector6);
			vector3.X = 320f;
			vector3.Y = 144f;
			num = SubMenuCharacterSelect.CreateVertices(array, num, ref vector3, ref vector, ref vector4, ref vector5, ref vector6);
			Vector2 vector7 = new Vector2(64f / (float)SubMenu.sPagesTexture.Width, 64f / (float)SubMenu.sPagesTexture.Height);
			Vector2 vector8 = new Vector2(1408f / (float)SubMenu.sPagesTexture.Width, 32f / (float)SubMenu.sPagesTexture.Height);
			array[num].X = -0.5f;
			array[num].Y = -0.5f;
			array[num].Z = vector8.X;
			array[num].W = vector8.Y;
			num++;
			array[num].X = 0.5f;
			array[num].Y = -0.5f;
			array[num].Z = vector8.X + vector7.X;
			array[num].W = vector8.Y;
			num++;
			array[num].X = 0.5f;
			array[num].Y = 0.5f;
			array[num].Z = vector8.X + vector7.X;
			array[num].W = vector8.Y + vector7.Y;
			num++;
			array[num].X = -0.5f;
			array[num].Y = 0.5f;
			array[num].Z = vector8.X;
			array[num].W = vector8.Y + vector7.Y;
			num++;
			vector8 = new Vector2(896f / (float)SubMenu.sPagesTexture.Width, 320f / (float)SubMenu.sPagesTexture.Height);
			array[num].X = -0.5f;
			array[num].Y = -0.5f;
			array[num].Z = vector8.X;
			array[num].W = vector8.Y;
			num++;
			array[num].X = 0.5f;
			array[num].Y = -0.5f;
			array[num].Z = vector8.X + vector7.X;
			array[num].W = vector8.Y;
			num++;
			array[num].X = 0.5f;
			array[num].Y = 0.5f;
			array[num].Z = vector8.X + vector7.X;
			array[num].W = vector8.Y + vector7.Y;
			num++;
			array[num].X = -0.5f;
			array[num].Y = 0.5f;
			array[num].Z = vector8.X;
			array[num].W = vector8.Y + vector7.Y;
			num++;
			array[num].X = -0.5f;
			array[num].Y = -0.5f;
			array[num].Z = 0f;
			array[num].W = 0f;
			num++;
			array[num].X = 0.5f;
			array[num].Y = -0.5f;
			array[num].Z = 1f;
			array[num].W = 0f;
			num++;
			array[num].X = 0.5f;
			array[num].Y = 0.5f;
			array[num].Z = 1f;
			array[num].W = 1f;
			num++;
			array[num].X = -0.5f;
			array[num].Y = 0.5f;
			array[num].Z = 0f;
			array[num].W = 1f;
			num++;
			lock (graphicsDevice)
			{
				this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
				this.mVertexBuffer = new VertexBuffer(graphicsDevice, array.Length * 4 * 4, BufferUsage.WriteOnly);
				this.mVertexBuffer.SetData<Vector4>(array);
				this.mIndexBuffer = new IndexBuffer(graphicsDevice, TextBox.INDICES.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
				this.mIndexBuffer.SetData<ushort>(TextBox.INDICES);
			}
			this.mOpenText = new Text(48, this.mFont, TextAlign.Left, false);
			this.mOpenText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_JOIN));
			this.mClosedText = new Text(48, this.mFont, TextAlign.Left, false);
			this.mClosedText.SetText(instance.GetString(0));
			this.ResetLevelTexts();
			this.mLoadingText = new Text(64, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mLoadingText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LOADING));
			this.mGamerDropDownMenu = new ContextMenu(this.mFont, TextAlign.Right, new int?(200));
			this.mAdminDropDownMenu = new ContextMenu(this.mFont, TextAlign.Right, new int?(200));
			this.mAdminDropDownMenu.AddOption(SubMenuCharacterSelect.LOC_KICK);
			this.mGameModeBox = new DropDownBox<Rulesets>(this.mFont, new Rulesets[]
			{
				Rulesets.DeathMatch,
				Rulesets.Brawl,
				Rulesets.Kreitor
			}, new int?[]
			{
				new int?("#versus_dm".GetHashCodeCustom()),
				new int?("#versus_brawl".GetHashCodeCustom()),
				new int?("#versus_tourney".GetHashCodeCustom())
			}, 250);
			this.mGameModeBox.Position = new Vector2(512f - this.mGameModeBox.Size.X, 89f - (float)this.mFont.LineHeight);
			this.mGameModeBox.ValueChanged += new Action<DropDownBox, Rulesets>(this.mGameModeBox_ValueChanged);
			this.mGameModeTitle = new Text(48, this.mFont, TextAlign.Left, false);
			this.mGameModeTitle.SetText(instance.GetString(SubMenuCharacterSelect.LOC_GAMEMODE));
			this.mSettingsScrollbar = new MenuScrollBar(default(Vector2), 170f, 0);
			this.mGameModeBox_ValueChanged(this.mGameModeBox, this.mGameModeBox.SelectedValue);
			this.mPacksText = new Text(64, this.mFont, TextAlign.Left, false);
			this.mItemsText = new Text(64, this.mFont, TextAlign.Left, false);
			this.mMagicksText = new Text(64, this.mFont, TextAlign.Left, false);
			this.mPacksText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_PACKS));
			this.mItemsText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_ITEMPACKS));
			this.mMagicksText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_MAGICKPACKS));
			this.mPosition.X = 128f;
			this.mPosition.Y = (float)Tome.PAGERIGHTSHEET.Height - 128f;
			this.mBackButton = new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE);
			this.mPosition.X = (float)Tome.PAGERIGHTSHEET.Width - 128f;
			this.mPosition.Y = (float)Tome.PAGERIGHTSHEET.Height - 128f;
			this.mStartButton = new MenuTextButtonItem(this.mPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenu.LOC_START, this.mFont, 200f, TextAlign.Right);
			this.mMenuTitle = new Text(48, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mGamerItems = new List<Text>();
			this.mGamerFont = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
			this.mGamerScrollBars = new MenuScrollBar[6];
			Vector2 iPosition = default(Vector2);
			iPosition.X = 948f;
			iPosition.Y = 145f;
			Vector2 textureOffset = new Vector2(-384f, 224f);
			Vector4 dialogue_COLOR_DEFAULT = Defines.DIALOGUE_COLOR_DEFAULT;
			dialogue_COLOR_DEFAULT.X *= 1.333f;
			dialogue_COLOR_DEFAULT.Y *= 1.333f;
			dialogue_COLOR_DEFAULT.Z *= 1.333f;
			int iMaxValue = Math.Max(0, this.mGamerItems.Count - 4);
			for (int i = 0; i < this.mGamerScrollBars.Length; i++)
			{
				this.mGamerScrollBars[i] = new MenuScrollBar(iPosition, 190f, iMaxValue);
				this.mGamerScrollBars[i].TextureOffset = textureOffset;
				this.mGamerScrollBars[i].Scale = 0.75f;
				this.mGamerScrollBars[i].Color = dialogue_COLOR_DEFAULT;
				iPosition.Y += 144f;
			}
			this.UpdateGamers();
			for (int j = 0; j < this.mPlayerSlots.Length; j++)
			{
				this.mPlayerSlots[j].State = SubMenuCharacterSelect.GamerState.Open;
				this.mPlayerSlots[j].Team = 0;
				this.mPlayerSlots[j].Name = new Text(32, this.mFont, TextAlign.Left, false);
				this.mPlayerSlots[j].LatencyText = new Text(32, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, true);
				this.mPlayerSlots[j].ControllerType = -1;
			}
			lock (Game.Instance.GraphicsDevice)
			{
				this.mCustomLevelOverlay = Game.Instance.Content.Load<Texture2D>("UI/Menu/CustomLevel");
				this.mLevelLockedOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_locked");
				this.mLevelUnusedOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_unused");
				this.mLevelFreeOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_free");
				this.mLevelFreeAndLockedOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_locked_free");
				this.mLevelFreeAndUnusedOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_unused_free");
				this.mLevelNewOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_new");
				this.mRobeLockedOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_locked");
				this.mRobeUnusedOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_unused");
				this.mRobeFreeOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_free");
				this.mRobeFreeAndLockedOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_locked_free");
				this.mRobeFreeAndUnusedOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_unused_free");
				this.mRobeNewOverlay = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_new");
				if (SubMenuCharacterSelect.sSelectLevelButtonFrame == null)
				{
					SubMenuCharacterSelect.sSelectLevelButtonFrame = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/select_level_frame");
				}
				if (SubMenuCharacterSelect.sStarTexture == null)
				{
					SubMenuCharacterSelect.sStarTexture = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/unused_generic");
				}
			}
			this.mSelectLevelButton = new MenuTextButtonItem(SubMenuCharacterSelect.SELECT_LEVEL_BUTTON_POS, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuCharacterSelect.LOC_TT_CHANGE_LEVEL, this.mFont, 320f, TextAlign.Left);
			this.mGenericStar = new MenuImageTextItem(Vector2.Zero, SubMenuCharacterSelect.sStarTexture, Vector2.Zero, Vector2.One, 0, Vector2.Zero, TextAlign.Center, this.mFont, new Vector2((float)SubMenuCharacterSelect.sStarTexture.Width, (float)SubMenuCharacterSelect.sStarTexture.Height));
			this.mPackCloseButton = new MenuTextButtonItem(default(Vector2), SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenu.LOC_OK, this.mFont, 200f, TextAlign.Right);
			this.mSpecialScrollBar = new MenuScrollBar(default(Vector2), 100f, 0);
			SubMenuCharacterSelect.VertexPositionTextureTexture[] array2 = new SubMenuCharacterSelect.VertexPositionTextureTexture[20];
			array2[0].Position.X = -0.5f;
			array2[0].Position.Y = -0.5f;
			array2[0].TexCoord0.X = 0f;
			array2[0].TexCoord0.Y = 0f;
			array2[0].TexCoord1.X = 0f;
			array2[0].TexCoord1.Y = 0.5f;
			array2[1].Position.X = 0.5f;
			array2[1].Position.Y = -0.5f;
			array2[1].TexCoord0.X = 1f;
			array2[1].TexCoord0.Y = 0f;
			array2[1].TexCoord1.X = 1f;
			array2[1].TexCoord1.Y = 0.5f;
			array2[2].Position.X = 0.5f;
			array2[2].Position.Y = 0.5f;
			array2[2].TexCoord0.X = 1f;
			array2[2].TexCoord0.Y = 0.5f;
			array2[2].TexCoord1.X = 1f;
			array2[2].TexCoord1.Y = 1f;
			array2[3].Position.X = -0.5f;
			array2[3].Position.Y = 0.5f;
			array2[3].TexCoord0.X = 0f;
			array2[3].TexCoord0.Y = 0.5f;
			array2[3].TexCoord1.X = 0f;
			array2[3].TexCoord1.Y = 1f;
			Vector2 vector9 = default(Vector2);
			vector9.X = 912f / (float)SubMenu.sPagesTexture.Width;
			vector9.Y = 512f / (float)SubMenu.sPagesTexture.Height;
			Vector2 vector10 = default(Vector2);
			vector10.X = 912f / (float)SubMenu.sPagesTexture.Width;
			vector10.Y = 640f / (float)SubMenu.sPagesTexture.Height;
			SubMenuCharacterSelect.CreateVertices(array2, 4, ref vector2, ref vector, ref vector9, ref vector10, ref vector5, ref vector6);
			lock (Game.Instance.GraphicsDevice)
			{
				this.mAvatarVertices = new VertexBuffer(Game.Instance.GraphicsDevice, array2.Length * 24, BufferUsage.WriteOnly);
				this.mAvatarVertices.SetData<SubMenuCharacterSelect.VertexPositionTextureTexture>(array2);
			}
			this.mAvatarVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(SubMenuCharacterSelect.VertexPositionTextureTexture.VertexElements);
			this.mNameInputBox = new TextInputMessageBox(SubMenuCharacterSelect.LOC_ENTER_NAME, 15);
			this.mLevelContent = new ContentManager(Game.Instance.Content.ServiceProvider, Game.Instance.Content.RootDirectory);
			SteamAPI.DlcInstalled += this.UpdateAvailableLevels;
			SteamAPI.DlcInstalled += this.UpdateAvailableAvatars;
			SteamAPI.DlcInstalled += this.UpdateAvailableDefaultAvatars;
			PackMan.Instance.PackEnabledChanged += new Action<object, bool>(this.Instance_PackEnabledChanged);
			this.mCheckPointRUSure = new OptionsMessageBox("#chapter_notification".GetHashCodeCustom(), new int[]
			{
				Defines.LOC_GEN_YES,
				Defines.LOC_GEN_NO
			});
			this.mCheckPointRUSure.Select += new Action<OptionsMessageBox, int>(this.DeleteCheckpointCallback);
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x001626B4 File Offset: 0x001608B4
		private void ResetLevelTexts()
		{
			if (this.mChapterName == null)
			{
				this.mChapterName = new Text(64, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			}
			int iLength = 150;
			TextAlign iAlign = TextAlign.Center;
			if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
			{
				iLength = 250;
				iAlign = TextAlign.Left;
			}
			this.mChapterDescription = new Text(iLength, FontManager.Instance.GetFont(MagickaFont.MenuTitle), iAlign, false);
		}

		// Token: 0x06002CF0 RID: 11504 RVA: 0x00162728 File Offset: 0x00160928
		private static bool AvatarAllowedInGameMode(GameType gameType, Profile.PlayableAvatar target)
		{
			switch (gameType)
			{
			case GameType.Campaign:
				goto IL_33;
			case GameType.Challenge:
				break;
			case (GameType)3:
				return false;
			case GameType.Versus:
				return target.AllowPVP;
			default:
				if (gameType == GameType.Mythos)
				{
					goto IL_33;
				}
				if (gameType != GameType.StoryChallange)
				{
					return false;
				}
				break;
			}
			return target.AllowChallenge;
			IL_33:
			return target.AllowCampaign;
		}

		// Token: 0x06002CF1 RID: 11505 RVA: 0x00162774 File Offset: 0x00160974
		public void UpdateAvailableAvatars(DlcInstalled dlcInstalled)
		{
			LevelNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
			if (level != null && level.AllowedAvatars != null && level.AllowedAvatars.Count > 0)
			{
				this.UpdateAvailableDefaultAvatars(dlcInstalled);
				return;
			}
			this.mDefaultAvatars = false;
			if (SubMenuCharacterSelect.mRobeRepresentations == null)
			{
				SubMenuCharacterSelect.mRobeRepresentations = new List<SubMenuCharacterSelect.RobeRep>();
			}
			else
			{
				SubMenuCharacterSelect.mRobeRepresentations.Clear();
			}
			lock (SubMenuCharacterSelect.mRobeRepresentations)
			{
				IList<Profile.PlayableAvatar> values = Profile.Instance.Avatars.Values;
				for (int i = 0; i < values.Count; i++)
				{
					Profile.PlayableAvatar playableAvatar = values[i];
					string name = playableAvatar.Name;
					if (string.Compare(name, "wizardcul") != 0 || AchievementsManager.Instance.HasAchievement("fhtagnoncemore"))
					{
						HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
						SubMenuCharacterSelect.RobeRep robeRep = new SubMenuCharacterSelect.RobeRep();
						robeRep.Name = name;
						robeRep.OriginalIndex = i;
						robeRep.IsCustom = (license == HackHelper.License.Custom);
						bool flag = SubMenuCharacterSelect.AvatarAllowedInGameMode(this.mGameSettings.GameType, playableAvatar);
						if (flag && (!robeRep.IsCustom || this.AllowCustom))
						{
							if (license == HackHelper.License.No)
							{
								robeRep.IsLocked = true;
							}
							uint num = 0U;
							robeRep.DisplayName = playableAvatar.DisplayName;
							robeRep.Description = playableAvatar.Description;
							robeRep.HashSum = playableAvatar.HashSum;
							if (string.Compare(name, "wizard") == 0)
							{
								robeRep.IsLocked = false;
								robeRep.IsUsed = true;
								robeRep.IsFree = false;
								robeRep.IsNew = false;
							}
							else if (flag && license != HackHelper.License.Custom)
							{
								if (license != HackHelper.License.Custom)
								{
									if (DLC_StatusHelper.ValidateRobeLocked(license, playableAvatar, out num))
									{
										robeRep.IsLocked = true;
										robeRep.IsUsed = true;
									}
									else
									{
										robeRep.IsLocked = false;
										robeRep.IsUsed = !DLC_StatusHelper.Instance.Item_IsUnused("robe", name, num, false);
									}
									robeRep.BelongsToAppID = num;
								}
								robeRep.IsNew = DLC_StatusHelper.Instance.AppID_IsNew(num, name);
								robeRep.IsFree = DLC_StatusHelper.Instance.IsFreeDLC(num, name);
								if (robeRep.IsLocked && string.Compare(name, "wizardbat") == 0)
								{
									goto IL_223;
								}
							}
							SubMenuCharacterSelect.mRobeRepresentations.Add(robeRep);
						}
					}
					IL_223:;
				}
			}
			DLC_StatusHelper.Instance.SaveLocalData();
			for (int j = 0; j < Game.Instance.Players.Length; j++)
			{
				Player player = Game.Instance.Players[j];
				if (player.Gamer != null)
				{
					this.VerifyAvatar(ref player);
				}
			}
			this.SortRobeRepList();
		}

		// Token: 0x06002CF2 RID: 11506 RVA: 0x00162A2C File Offset: 0x00160C2C
		private void SteamOverlayActivated(GameOverlayActivated gameOverlayActivated)
		{
			if (gameOverlayActivated.mActive == 0)
			{
				this.UpdateAvailableLevels(default(DlcInstalled));
				this.UpdateAvailableAvatars(default(DlcInstalled));
			}
		}

		// Token: 0x06002CF3 RID: 11507 RVA: 0x00162A68 File Offset: 0x00160C68
		private void UpdateAvailableDefaultAvatars(DlcInstalled dlcInstalled)
		{
			LevelNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
			this.mDefaultAvatars = true;
			if (SubMenuCharacterSelect.mRobeRepresentations == null)
			{
				SubMenuCharacterSelect.mRobeRepresentations = new List<SubMenuCharacterSelect.RobeRep>();
			}
			else
			{
				SubMenuCharacterSelect.mRobeRepresentations.Clear();
			}
			lock (SubMenuCharacterSelect.mRobeRepresentations)
			{
				IList<Profile.PlayableAvatar> values = Profile.Instance.Avatars.Values;
				foreach (string a in level.AllowedAvatars)
				{
					for (int i = 0; i < values.Count; i++)
					{
						Profile.PlayableAvatar playableAvatar = values[i];
						if (a == playableAvatar.Name)
						{
							SubMenuCharacterSelect.RobeRep robeRep = new SubMenuCharacterSelect.RobeRep();
							HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
							if (license == HackHelper.License.Yes || (license == HackHelper.License.Custom && this.AllowCustom))
							{
								robeRep.OriginalIndex = i;
								robeRep.IsCustom = (license != HackHelper.License.Yes);
							}
							else
							{
								if (license != HackHelper.License.No)
								{
									goto IL_1D3;
								}
								robeRep.IsLocked = true;
							}
							uint num = 0U;
							string text = robeRep.Name = playableAvatar.Name;
							robeRep.DisplayName = playableAvatar.DisplayName;
							robeRep.Description = playableAvatar.Description;
							robeRep.HashSum = playableAvatar.HashSum;
							if (string.Compare(text, "wizard") == 0)
							{
								robeRep.IsLocked = false;
								robeRep.IsUsed = true;
								robeRep.IsFree = false;
								robeRep.IsNew = false;
							}
							else
							{
								if (license != HackHelper.License.Custom)
								{
									if (DLC_StatusHelper.ValidateRobeLocked(license, playableAvatar, out num))
									{
										robeRep.IsLocked = true;
										robeRep.IsUsed = true;
									}
									else
									{
										robeRep.IsLocked = false;
										robeRep.IsUsed = !DLC_StatusHelper.Instance.Item_IsUnused("robe", text, num, false);
									}
								}
								robeRep.BelongsToAppID = num;
								robeRep.IsNew = DLC_StatusHelper.Instance.AppID_IsNew(num, text);
								robeRep.IsFree = DLC_StatusHelper.Instance.IsFreeDLC(num, text);
							}
							SubMenuCharacterSelect.mRobeRepresentations.Add(robeRep);
						}
						IL_1D3:;
					}
				}
			}
			DLC_StatusHelper.Instance.SaveLocalData();
			this.DefaultAvatars();
		}

		// Token: 0x06002CF4 RID: 11508 RVA: 0x00162CC8 File Offset: 0x00160EC8
		public bool NeedToUpdateDefaultAvatarsUponClientLeaving()
		{
			if (this.mGameSettings.GameType != GameType.StoryChallange)
			{
				return false;
			}
			if (!this.HasSelectedLevel)
			{
				return false;
			}
			LevelNode level = LevelManager.Instance.GetLevel(GameType.StoryChallange, this.mGameSettings.Level);
			return level.FileName.Equals("ch_osotc");
		}

		// Token: 0x06002CF5 RID: 11509 RVA: 0x00162D20 File Offset: 0x00160F20
		public void DefaultAvatars()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			Player[] players = Game.Instance.Players;
			bool[] array = new bool[4];
			array[0] = (array[1] = (array[2] = (array[3] = false)));
			for (int i = 0; i < SubMenuCharacterSelect.mRobeRepresentations.Count; i++)
			{
				Profile.PlayableAvatar playableAvatar = Profile.Instance.Avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[i].OriginalIndex];
				bool flag = false;
				for (int j = 0; j < players.Length; j++)
				{
					Player player = players[j];
					if (player.Playing && !array[j] && player.Gamer.Avatar == playableAvatar)
					{
						flag = true;
						array[j] = true;
						break;
					}
				}
				if (!flag)
				{
					for (int k = 0; k < players.Length; k++)
					{
						Player player2 = players[k];
						if (player2.Playing && !array[k])
						{
							HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
							if (license == HackHelper.License.Yes || (license == HackHelper.License.Custom && (NetworkManager.Instance.State == NetworkState.Offline || !NetworkManager.Instance.Interface.IsVACSecure)))
							{
								if (!player2.Gamer.Avatar.Name.Equals(playableAvatar.Name))
								{
									this.mPlayerSlots[k].ConsecutiveColorChanges = -1;
								}
								array[k] = true;
								player2.Gamer.Avatar = playableAvatar;
								this.mPlayerSlots[k].Custom = SubMenuCharacterSelect.mRobeRepresentations[k].IsCustom;
								if (NetworkManager.Instance.State == NetworkState.Server)
								{
									GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(player2);
									NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
								}
								ToolTipMan.Instance.Kill(player2, false);
								this.mPlayerSlots[k].SelectedItem = -1;
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06002CF6 RID: 11510 RVA: 0x00162F18 File Offset: 0x00161118
		private void DefaultAvatar(Controller iSender)
		{
			int num = this.NumActivePlayer();
			num--;
			bool flag = false;
			Profile.PlayableAvatar playableAvatar = Profile.Instance.DefaultAvatar;
			if (num >= SubMenuCharacterSelect.mRobeRepresentations.Count)
			{
				LevelNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
				if (NetworkManager.Instance.State == NetworkState.Client && level.AllowedAvatars != null && level.AllowedAvatars.Count > 0)
				{
					string key = level.AllowedAvatars[num];
					playableAvatar = Profile.Instance.Avatars[key];
					flag = true;
				}
			}
			else
			{
				playableAvatar = Profile.Instance.Avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[num].OriginalIndex];
			}
			HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
			if (flag || license == HackHelper.License.Yes || (license == HackHelper.License.Custom && (NetworkManager.Instance.State == NetworkState.Offline || !NetworkManager.Instance.Interface.IsVACSecure)))
			{
				if (!iSender.Player.Gamer.Avatar.Name.Equals(playableAvatar.Name))
				{
					this.mPlayerSlots[num].ConsecutiveColorChanges = -1;
				}
				iSender.Player.Gamer.Avatar = playableAvatar;
				if (flag)
				{
					this.mPlayerSlots[num].Custom = false;
				}
				else
				{
					this.mPlayerSlots[num].Custom = SubMenuCharacterSelect.mRobeRepresentations[num].IsCustom;
				}
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(iSender.Player);
					NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
				}
				ToolTipMan.Instance.Kill(iSender.Player, false);
				this.mPlayerSlots[num].SelectedItem = -1;
			}
		}

		// Token: 0x06002CF7 RID: 11511 RVA: 0x001630E0 File Offset: 0x001612E0
		private int NumActivePlayer()
		{
			int num = 0;
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing && this.mPlayerSlots[i].AvatarSelected)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06002CF8 RID: 11512 RVA: 0x0016312C File Offset: 0x0016132C
		private void UpdateAvailableLevels(DlcInstalled obj)
		{
			GameType gameType = this.mGameSettings.GameType;
			LevelNode[] array;
			switch (gameType)
			{
			case GameType.Campaign:
				array = LevelManager.Instance.VanillaCampaign;
				break;
			case GameType.Challenge:
				array = LevelManager.Instance.Challenges;
				break;
			case (GameType)3:
				return;
			case GameType.Versus:
				array = LevelManager.Instance.Versus;
				break;
			default:
				if (gameType != GameType.Mythos)
				{
					if (gameType != GameType.StoryChallange)
					{
						return;
					}
					array = LevelManager.Instance.StoryChallanges;
				}
				else
				{
					array = LevelManager.Instance.MythosCampaign;
				}
				break;
			}
			bool flag = NetworkManager.Instance.State != NetworkState.Client;
			if ((this.mGameSettings.GameType == GameType.Mythos || this.mGameSettings.GameType == GameType.Campaign) && flag)
			{
				SubMenuCharacterSelect.mLevelRepresentations.Clear();
				SaveData currentSaveData = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
				if (currentSaveData != null)
				{
					int val = (int)(currentSaveData.MaxAllowedLevel + 1);
					for (int i = 0; i < Math.Min(array.Length, val); i++)
					{
						SubMenuCharacterSelect.LevelRep levelRep = new SubMenuCharacterSelect.LevelRep();
						levelRep.OriginalIndex = i;
						levelRep.IsCustom = false;
						levelRep.PreviewImage = null;
						uint belongsToAppID = 0U;
						levelRep.Name = array[i].Name;
						levelRep.FileName = array[i].FileName;
						levelRep.IsLocked = false;
						levelRep.BelongsToAppID = belongsToAppID;
						levelRep.IsNew = false;
						levelRep.IsUsed = false;
						levelRep.IsFree = false;
						levelRep.HashSum = array[i].HashSum;
						SubMenuCharacterSelect.mLevelRepresentations.Add(levelRep);
					}
				}
			}
			else
			{
				uint num = 0U;
				SubMenuCharacterSelect.LevelRep levelRep2 = null;
				for (int j = 0; j < array.Length; j++)
				{
					bool flag2 = false;
					foreach (SubMenuCharacterSelect.LevelRep levelRep3 in SubMenuCharacterSelect.mLevelRepresentations)
					{
						if (levelRep3.OriginalIndex == j)
						{
							levelRep2 = levelRep3;
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						HackHelper.License license = HackHelper.CheckLicense(array[j]);
						levelRep2.IsCustom = (license == HackHelper.License.Custom);
						num = 0U;
						string name = array[j].Name;
						levelRep2.IsLocked = DLC_StatusHelper.ValidateLevelLocked(license, array[j], out num);
						levelRep2.BelongsToAppID = num;
						levelRep2.IsNew = DLC_StatusHelper.Instance.AppID_IsNew(num, array[j].FileName);
						levelRep2.IsUsed = !DLC_StatusHelper.Instance.Item_IsUnused("level", name, num, false);
						levelRep2.IsFree = DLC_StatusHelper.Instance.IsFreeDLC(num, array[j].FileName);
					}
				}
			}
			DLC_StatusHelper.Instance.SaveLocalData();
			this.UpdateLevelDescriptions();
			if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus)
			{
				this.SortLevelRepList();
			}
			this.mSpecialScrollBar.Value = 0;
		}

		// Token: 0x06002CF9 RID: 11513 RVA: 0x00163414 File Offset: 0x00161614
		private void SortLevelRepList()
		{
			if (SubMenuCharacterSelect.mLevelRepresentations == null || SubMenuCharacterSelect.mLevelRepresentations.Count == 0)
			{
				return;
			}
			lock (SubMenuCharacterSelect.mLevelRepresentations)
			{
				SubMenuCharacterSelect.mLevelRepresentations.Sort(new Comparison<SubMenuCharacterSelect.LevelRep>(SubMenuCharacterSelect.SelectableObjectRep.CompareRep));
			}
		}

		// Token: 0x06002CFA RID: 11514 RVA: 0x00163470 File Offset: 0x00161670
		private void SortRobeRepList()
		{
			if (SubMenuCharacterSelect.mRobeRepresentations == null || SubMenuCharacterSelect.mRobeRepresentations.Count == 0)
			{
				return;
			}
			lock (SubMenuCharacterSelect.mRobeRepresentations)
			{
				SubMenuCharacterSelect.mRobeRepresentations.Sort(new Comparison<SubMenuCharacterSelect.RobeRep>(SubMenuCharacterSelect.SelectableObjectRep.CompareRep));
			}
		}

		// Token: 0x06002CFB RID: 11515 RVA: 0x001634CC File Offset: 0x001616CC
		private int GetLevelOriginalIndex(int sortedIndex)
		{
			return SubMenuCharacterSelect.mLevelRepresentations[sortedIndex].OriginalIndex;
		}

		// Token: 0x06002CFC RID: 11516 RVA: 0x001634E0 File Offset: 0x001616E0
		private int GetLevelSortedIndex(int originalIndex)
		{
			if (SubMenuCharacterSelect.mLevelRepresentations == null)
			{
				return -1;
			}
			for (int i = 0; i < SubMenuCharacterSelect.mLevelRepresentations.Count; i++)
			{
				if (SubMenuCharacterSelect.mLevelRepresentations[i].OriginalIndex == originalIndex)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06002CFD RID: 11517 RVA: 0x00163521 File Offset: 0x00161721
		private int GetRobeOriginalIndex(int sortedIndex)
		{
			return SubMenuCharacterSelect.mRobeRepresentations[sortedIndex].OriginalIndex;
		}

		// Token: 0x06002CFE RID: 11518 RVA: 0x00163534 File Offset: 0x00161734
		private int GetRobeSortedIndex(int originalIndex)
		{
			for (int i = 0; i < SubMenuCharacterSelect.mRobeRepresentations.Count; i++)
			{
				if (SubMenuCharacterSelect.mRobeRepresentations[i].OriginalIndex == originalIndex)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06002CFF RID: 11519 RVA: 0x0016356C File Offset: 0x0016176C
		private void UpdateLevelDescriptions()
		{
			GameType gameType = this.mGameSettings.GameType;
			LevelNode[] array;
			switch (gameType)
			{
			case GameType.Campaign:
				array = LevelManager.Instance.VanillaCampaign;
				break;
			case GameType.Challenge:
				array = LevelManager.Instance.Challenges;
				break;
			case (GameType)3:
				return;
			case GameType.Versus:
				array = LevelManager.Instance.Versus;
				break;
			default:
				if (gameType != GameType.Mythos)
				{
					if (gameType != GameType.StoryChallange)
					{
						return;
					}
					array = LevelManager.Instance.StoryChallanges;
				}
				else
				{
					array = LevelManager.Instance.MythosCampaign;
				}
				break;
			}
			LanguageManager instance = LanguageManager.Instance;
			if (SubMenuCharacterSelect.mLevelRepresentations != null)
			{
				for (int i = 0; i < SubMenuCharacterSelect.mLevelRepresentations.Count; i++)
				{
					if (SubMenuCharacterSelect.mLevelRepresentations[i].Title != null)
					{
						SubMenuCharacterSelect.mLevelRepresentations[i].Title.Dispose();
					}
					if (SubMenuCharacterSelect.mLevelRepresentations[i].Descr != null)
					{
						SubMenuCharacterSelect.mLevelRepresentations[i].Descr.Dispose();
					}
					SubMenuCharacterSelect.mLevelRepresentations[i].Title = new Text(64, this.mFont, TextAlign.Center, false);
					SubMenuCharacterSelect.mLevelRepresentations[i].Title.SetText(instance.GetString(SubMenuCharacterSelect.mLevelRepresentations[i].Name.GetHashCodeCustom()));
				}
			}
			if (this.mGameSettings.GameType == GameType.Mythos || this.mGameSettings.GameType == GameType.Campaign)
			{
				BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuTitle);
				if (SubMenuCharacterSelect.mLevelRepresentations != null)
				{
					for (int j = 0; j < SubMenuCharacterSelect.mLevelRepresentations.Count; j++)
					{
						SubMenuCharacterSelect.mLevelRepresentations[j].Descr = new Text(64, this.mFont, TextAlign.Center, false);
						string text = instance.GetString(array[SubMenuCharacterSelect.mLevelRepresentations[j].OriginalIndex].Description);
						text = font.Wrap(text, 539, true);
						SubMenuCharacterSelect.mLevelRepresentations[j].Descr.SetText(text);
					}
				}
			}
		}

		// Token: 0x06002D00 RID: 11520 RVA: 0x0016376C File Offset: 0x0016196C
		private void Instance_PackEnabledChanged(object arg1, bool arg2)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				PackOptionsMessage packOptionsMessage = default(PackOptionsMessage);
				NetworkManager.Instance.Interface.SendMessage<PackOptionsMessage>(ref packOptionsMessage);
			}
		}

		// Token: 0x06002D01 RID: 11521 RVA: 0x001637A0 File Offset: 0x001619A0
		private void LoadLevelPreviews()
		{
			lock (this.mLevelContent)
			{
				this.mLevelContent.Unload();
			}
			LevelNode[] array = null;
			string text = null;
			bool flag = false;
			GameType gameType = this.mGameSettings.GameType;
			switch (gameType)
			{
			case GameType.Campaign:
				break;
			case GameType.Challenge:
				array = LevelManager.Instance.Challenges;
				text = "Levels/Challenges/";
				goto IL_99;
			case (GameType)3:
				return;
			case GameType.Versus:
				array = LevelManager.Instance.Versus;
				text = "Levels/Versus/";
				goto IL_99;
			default:
				if (gameType != GameType.Mythos)
				{
					if (gameType != GameType.StoryChallange)
					{
						return;
					}
					array = LevelManager.Instance.StoryChallanges;
					text = "Levels/Challenges/";
					goto IL_99;
				}
				break;
			}
			flag = true;
			IL_99:
			SubMenuCharacterSelect.mLevelRepresentations = new List<SubMenuCharacterSelect.LevelRep>();
			if (!string.IsNullOrEmpty(text))
			{
				for (int i = 0; i < array.Length; i++)
				{
					HackHelper.License license = HackHelper.CheckLicense(array[i]);
					if (license != HackHelper.License.Custom || this.AllowCustom)
					{
						string name = array[i].Name;
						if (!string.IsNullOrEmpty(name) && (!name.Equals("#challenge_vietnam", StringComparison.InvariantCultureIgnoreCase) || this.mGameSettings.GameType != GameType.Challenge))
						{
							SubMenuCharacterSelect.LevelRep levelRep = new SubMenuCharacterSelect.LevelRep();
							levelRep.OriginalIndex = i;
							levelRep.Name = name;
							levelRep.FileName = array[i].FileName;
							levelRep.HashSum = array[i].HashSum;
							if (!flag)
							{
								try
								{
									levelRep.PreviewImage = this.mLevelContent.Load<Texture2D>(text + array[i].LoadingImage);
									goto IL_176;
								}
								catch (ContentLoadException)
								{
									levelRep.PreviewImage = null;
									goto IL_176;
								}
								goto IL_16F;
							}
							goto IL_16F;
							IL_176:
							SubMenuCharacterSelect.mLevelRepresentations.Add(levelRep);
							goto IL_181;
							IL_16F:
							levelRep.PreviewImage = null;
							goto IL_176;
						}
					}
					IL_181:;
				}
			}
			this.UpdateAvailableLevels(default(DlcInstalled));
			this.mLoadingLevels = false;
			if (array != null)
			{
				if (NetworkManager.Instance.State == NetworkState.Client && this.HasSelectedLevel && this.mGameSettings.Level > -1)
				{
					this.SortLevelRepList();
					this.mGameSettings.Level = this.GetLevelSortedIndex(this.mGameSettings.Level);
					this.HasSelectedLevel = (this.mGameSettings.Level > -1);
					return;
				}
				if (!string.IsNullOrEmpty(this.mLevelNameToFocusWhenLevelComplete))
				{
					bool flag2 = false;
					int num = 0;
					foreach (LevelNode levelNode in array)
					{
						if (levelNode.FileName.Equals(this.mLevelNameToFocusWhenLevelComplete, StringComparison.InvariantCultureIgnoreCase))
						{
							flag2 = true;
							this.mSelectedPack = (this.mGameSettings.Level = num);
							this.HasSelectedLevel = true;
							break;
						}
						num++;
					}
					if (!flag2)
					{
						num = -1;
						this.HasSelectedLevel = false;
					}
					this.mGameSettings.Level = num;
					this.OnLevelChange(null, this.mGameSettings.GameType, num);
					this.ChangeState(null, SubMenuCharacterSelect.State.Normal);
					this.mLevelNameToFocusWhenLevelComplete = null;
				}
			}
		}

		// Token: 0x06002D02 RID: 11522 RVA: 0x00163A84 File Offset: 0x00161C84
		private void mGameModeBox_ValueChanged(DropDownBox iSender, Rulesets iValue)
		{
			if (this.mVersusSettings != null)
			{
				this.mVersusSettings.Changed -= this.mVersusSettings_Changed;
			}
			if (this.mVersusSettings != null)
			{
				VersusRuleset.Settings.OptionsMessage optionsMessage;
				this.mVersusSettings.GetMessage(out optionsMessage);
				switch (optionsMessage.Ruleset)
				{
				case Rulesets.DeathMatch:
					GlobalSettings.Instance.VSSettings.DeathMatch = optionsMessage;
					break;
				case Rulesets.Brawl:
					GlobalSettings.Instance.VSSettings.Brawl = optionsMessage;
					break;
				case Rulesets.Pyrite:
					GlobalSettings.Instance.VSSettings.PyriteSnitch = optionsMessage;
					break;
				case Rulesets.Kreitor:
					GlobalSettings.Instance.VSSettings.Kreitor = optionsMessage;
					break;
				case Rulesets.King:
					GlobalSettings.Instance.VSSettings.KingOfTheHill = optionsMessage;
					break;
				}
			}
			switch (iValue)
			{
			case Rulesets.DeathMatch:
				VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.DeathMatch);
				break;
			case Rulesets.Brawl:
				VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.Brawl);
				break;
			case Rulesets.Pyrite:
				VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.PyriteSnitch);
				break;
			case Rulesets.Kreitor:
				VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.Kreitor);
				break;
			case Rulesets.King:
				VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.KingOfTheHill);
				break;
			default:
				VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.DeathMatch);
				break;
			}
			this.mSettingsScrollbar.SetMaxValue(this.mVersusSettings.MenuItems.Count - 5);
			LanguageManager instance = LanguageManager.Instance;
			this.mVersusSettingsTitles = new Text[this.mVersusSettings.MenuTitles.Count];
			for (int i = 0; i < this.mVersusSettings.MenuTitles.Count; i++)
			{
				this.mVersusSettingsTitles[i] = new Text(48, this.mVersusSettings.MenuItems[i].Font, TextAlign.Left, false);
				this.mVersusSettingsTitles[i].SetText(instance.GetString(this.mVersusSettings.MenuTitles[i]));
			}
			this.mVersusSettings.Changed += this.mVersusSettings_Changed;
			NetworkState state = NetworkManager.Instance.State;
			if (state == NetworkState.Server)
			{
				VersusRuleset.Settings.OptionsMessage optionsMessage2;
				this.mVersusSettings.GetMessage(out optionsMessage2);
				NetworkManager.Instance.Interface.SendMessage<VersusRuleset.Settings.OptionsMessage>(ref optionsMessage2);
			}
			if (state != NetworkState.Client)
			{
				bool flag = this.mVersusSettings.TeamsEnabled && this.mGameSettings.GameType == GameType.Versus;
				int num = 0;
				int num2 = 0;
				Player[] players = Game.Instance.Players;
				for (int j = 0; j < players.Length; j++)
				{
					if (players[j].Playing)
					{
						if (flag)
						{
							if (players[j].Team == Factions.NONE)
							{
								if (num <= num2)
								{
									players[j].Team = Factions.TEAM_RED;
									num++;
								}
								else
								{
									players[j].Team = Factions.TEAM_BLUE;
									num2++;
								}
								if (state == NetworkState.Server)
								{
									MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
									menuSelectMessage.Option = 2;
									menuSelectMessage.Param0I = j;
									menuSelectMessage.Param1I = (int)players[j].Team;
									NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
								}
							}
							else if ((players[j].Team & Factions.TEAM_RED) != Factions.NONE)
							{
								num++;
							}
							else
							{
								num2++;
							}
						}
						else
						{
							players[j].Team = Factions.NONE;
							if (state == NetworkState.Server)
							{
								MenuSelectMessage menuSelectMessage2 = default(MenuSelectMessage);
								menuSelectMessage2.Option = 2;
								menuSelectMessage2.Param0I = j;
								menuSelectMessage2.Param1I = (int)players[j].Team;
								NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage2);
							}
						}
					}
				}
			}
			this.UpdateGamerDropDownMenu();
		}

		// Token: 0x06002D03 RID: 11523 RVA: 0x00163E5C File Offset: 0x0016205C
		private void mVersusSettings_Changed(int iOption, int iNewSelection)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
				menuSelectMessage.IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect;
				menuSelectMessage.Option = 0;
				menuSelectMessage.Param0I = iOption;
				menuSelectMessage.Param1I = iNewSelection;
				NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
			}
			this.UpdateGamerDropDownMenu();
			Player[] players = Game.Instance.Players;
			if (NetworkManager.Instance.State != NetworkState.Client && this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled)
			{
				int i = 0;
				int num = 3;
				while (i <= num)
				{
					while (i < num && !players[i].Playing)
					{
						i++;
					}
					if (players[i].Playing && players[i].Team == Factions.NONE)
					{
						players[i].Team = Factions.TEAM_RED;
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							MenuSelectMessage menuSelectMessage2 = default(MenuSelectMessage);
							menuSelectMessage2.IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect;
							menuSelectMessage2.Option = 2;
							menuSelectMessage2.Param0I = i;
							menuSelectMessage2.Param1I = 4096;
							NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage2);
						}
					}
					while (i < num && !players[num].Playing)
					{
						num--;
					}
					if (players[num].Playing && players[num].Team == Factions.NONE)
					{
						players[num].Team = Factions.TEAM_BLUE;
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							MenuSelectMessage menuSelectMessage3 = default(MenuSelectMessage);
							menuSelectMessage3.IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect;
							menuSelectMessage3.Option = 2;
							menuSelectMessage3.Param0I = num;
							menuSelectMessage3.Param1I = 8192;
							NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage3);
						}
					}
					i++;
					num--;
				}
				return;
			}
			if (!this.mVersusSettings.TeamsEnabled || this.mGameSettings.GameType != GameType.Versus)
			{
				for (int j = 0; j < players.Length; j++)
				{
					players[j].Team = Factions.NONE;
				}
			}
		}

		// Token: 0x06002D04 RID: 11524 RVA: 0x00164040 File Offset: 0x00162240
		private void UpdateGamerDropDownMenu()
		{
			if (this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled)
			{
				if (this.mGamerDropDownMenu.Count <= 0)
				{
					this.mGamerDropDownMenu.AddOption("#menu_change_team".GetHashCodeCustom());
					return;
				}
			}
			else if (this.mGamerDropDownMenu.Count > 0)
			{
				this.mGamerDropDownMenu.RemoveAt(0);
			}
		}

		// Token: 0x06002D05 RID: 11525 RVA: 0x001640A8 File Offset: 0x001622A8
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.UpdateLevelDescriptions();
			LanguageManager instance = LanguageManager.Instance;
			this.mPacksText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_PACKS));
			this.mItemsText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_ITEMPACKS));
			this.mMagicksText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_MAGICKPACKS));
			this.mOpenText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_JOIN));
			this.mSelectLevelButton.SetText(instance.GetString(SubMenuCharacterSelect.LOC_TT_CHANGE_LEVEL));
			this.mBackButton.LanguageChanged();
			this.mStartButton.LanguageChanged();
			this.mPackCloseButton.LanguageChanged();
			this.mLoadingText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LOADING));
			this.mGameModeBox.LanguageChanged();
			this.mGameModeTitle.SetText(instance.GetString(SubMenuCharacterSelect.LOC_GAMEMODE));
			for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
			{
				this.mVersusSettings.MenuItems[i].LanguageChanged();
				this.mVersusSettingsTitles[i].SetText(instance.GetString(this.mVersusSettings.MenuTitles[i]));
			}
		}

		// Token: 0x06002D06 RID: 11526 RVA: 0x001641E6 File Offset: 0x001623E6
		public void GameChanged()
		{
			this.SetReady(false);
		}

		// Token: 0x06002D07 RID: 11527 RVA: 0x001641F0 File Offset: 0x001623F0
		private void UpdateGamers()
		{
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
			lock (Game.Instance.GraphicsDevice)
			{
				this.mGamerItems.Clear();
				Text text;
				for (int i = 0; i < Profile.Instance.Gamers.Count; i++)
				{
					text = new Text(32, font, TextAlign.Center, false);
					text.SetText(Profile.Instance.Gamers.Keys[i]);
					this.mGamerItems.Add(text);
				}
				text = new Text(32, font, TextAlign.Center, false);
				text.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_NEW));
				this.mGamerItems.Add(text);
				for (int j = 0; j < this.mGamerScrollBars.Length; j++)
				{
					this.mGamerScrollBars[j].SetMaxValue(this.mGamerItems.Count - 4);
				}
			}
		}

		// Token: 0x06002D08 RID: 11528 RVA: 0x001642E8 File Offset: 0x001624E8
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			NetworkState state = NetworkManager.Instance.State;
			if (this.mCurrentState == SubMenuCharacterSelect.State.Normal || this.mCurrentState == SubMenuCharacterSelect.State.CountDown)
			{
				this.mOptionsAlpha = Math.Min(this.mOptionsAlpha + iDeltaTime * 4f, 1f);
				this.mLevelSelectAlpha = Math.Max(this.mLevelSelectAlpha - iDeltaTime * 4f, 0f);
				this.mPackSelectAlpha = Math.Max(this.mPackSelectAlpha - iDeltaTime * 4f, 0f);
			}
			else
			{
				this.mOptionsAlpha = Math.Max(this.mOptionsAlpha - iDeltaTime * 4f, 0f);
				if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
				{
					this.mLevelSelectAlpha = Math.Min(this.mLevelSelectAlpha + iDeltaTime * 4f, 1f);
					this.mPackSelectAlpha = Math.Max(this.mPackSelectAlpha - iDeltaTime * 4f, 0f);
				}
				else
				{
					this.mLevelSelectAlpha = Math.Max(this.mLevelSelectAlpha - iDeltaTime * 4f, 0f);
					this.mPackSelectAlpha = Math.Min(this.mPackSelectAlpha + iDeltaTime * 4f, 1f);
				}
			}
			if (state == NetworkState.Server)
			{
				bool isTransitionActive = RenderManager.Instance.IsTransitionActive;
				(NetworkManager.Instance.Interface as NetworkServer).Playing = (this.mCountDown > 0f || isTransitionActive);
			}
			if (this.mLastCountDownNr >= 0)
			{
				this.mCountDown -= iDeltaTime;
				int num = (int)Math.Ceiling((double)this.mCountDown);
				if (num != this.mLastCountDownNr)
				{
					string iMessage = LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_COUNTDOWN).Replace("#1;", num.ToString());
					NetworkChat.Instance.AddMessage(iMessage);
					if (num == 0 && NetworkManager.Instance.State != NetworkState.Offline)
					{
						this.StartLevel();
					}
					this.mLastCountDownNr = num;
				}
			}
			bool enabled = this.mCurrentState != SubMenuCharacterSelect.State.CountDown && this.mGameSettings.GameType == GameType.Versus && NetworkManager.Instance.State != NetworkState.Client;
			this.mGameModeBox.Enabled = enabled;
			for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
			{
				this.mVersusSettings.MenuItems[i].Enabled = enabled;
			}
			this.mGamerDropDownMenu.Update(iDeltaTime);
			this.mAdminDropDownMenu.Update(iDeltaTime);
			this.mGameModeBox.Update(iDeltaTime);
			for (int j = 0; j < this.mVersusSettings.MenuItems.Count; j++)
			{
				this.mVersusSettings.MenuItems[j].Update(iDeltaTime);
			}
			Player[] players = Game.Instance.Players;
			this.mStartButton.Enabled = (this.HasSelectedLevel && !this.mValidatingLevels && !this.mLoadingLevels && state != NetworkState.Client);
			if (state != NetworkState.Offline)
			{
				for (int k = 0; k < players.Length; k++)
				{
					NetworkGamer networkGamer = players[k].Gamer as NetworkGamer;
					if (players[k].Playing && networkGamer != null)
					{
						this.mPlayerSlots[k].SetLatency(NetworkManager.Instance.Interface.GetLatencyMS(networkGamer.ClientID));
					}
					else
					{
						this.mPlayerSlots[k].SetLatency(0);
					}
				}
				NetworkChat.Instance.Update(iDeltaTime);
			}
		}

		// Token: 0x06002D09 RID: 11529 RVA: 0x00164640 File Offset: 0x00162840
		private bool HitPackList(ref Vector2 iMousePos, out int oIndex)
		{
			oIndex = -1;
			if (iMousePos.X < 128f || iMousePos.X > 448f)
			{
				return false;
			}
			int value = this.mSpecialScrollBar.Value;
			ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
			MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
			int num = Math.Max(0, 1 - value);
			float num2 = this.mSpecialScrollBar.Position.Y - 224f;
			if (value == 0)
			{
				num2 += 64f;
			}
			int num3 = 5 * Math.Max(value - 1, 0);
			while (num < 7 && num3 < itemPacks.Length)
			{
				float num4 = 128f;
				int num5 = 0;
				while (num3 < itemPacks.Length && num5 < 5)
				{
					if (iMousePos.X >= num4 && iMousePos.X <= num4 + 64f && iMousePos.Y >= num2 && iMousePos.Y <= num2 + 64f)
					{
						oIndex = num3;
						return true;
					}
					num4 += 64f;
					num3++;
					num5++;
				}
				num2 += 64f;
				num++;
			}
			int num6 = itemPacks.Length / 5;
			if (itemPacks.Length % 5 != 0)
			{
				num6++;
			}
			if (num < 7 && num6 + 1 - value >= 0)
			{
				num2 += 64f;
				num++;
			}
			int num7 = Math.Max(0, value - 2 - num6);
			int num8 = num7 * 5;
			while (num < 7 && num8 < magickPacks.Length)
			{
				float num9 = 128f;
				int num10 = 0;
				while (num8 < magickPacks.Length && num10 < 5)
				{
					if (iMousePos.X >= num9 && iMousePos.X <= num9 + 64f && iMousePos.Y >= num2 && iMousePos.Y <= num2 + 64f)
					{
						oIndex = num8 + itemPacks.Length;
						return true;
					}
					num9 += 64f;
					num8++;
					num10++;
				}
				num2 += 64f;
				num++;
			}
			return false;
		}

		// Token: 0x06002D0A RID: 11530 RVA: 0x00164820 File Offset: 0x00162A20
		private bool HitPackOverview(ref Vector2 iMousePos, out int oIndex, out ControllerDirection oScrollDirection)
		{
			oIndex = -1;
			oScrollDirection = ControllerDirection.Center;
			float num = 408f;
			if (iMousePos.Y >= num && iMousePos.Y <= num + 64f)
			{
				float num2 = 288f;
				IPack[] allPacks = PackMan.Instance.AllPacks;
				int num3 = 0;
				for (int i = 0; i < allPacks.Length; i++)
				{
					if (allPacks[i].Enabled)
					{
						num3++;
					}
				}
				int num4 = this.mPackScrollValue;
				float num5;
				if (num3 > 5)
				{
					num5 = num2 - 224f;
					if (iMousePos.X >= num5 && iMousePos.X <= num5 + 64f)
					{
						oScrollDirection = ControllerDirection.Left;
						return true;
					}
					num5 = num2 + 160f;
					if (iMousePos.X >= num5 && iMousePos.X <= num5 + 64f)
					{
						oScrollDirection = ControllerDirection.Right;
						return true;
					}
				}
				else
				{
					num4 = 0;
				}
				int num6 = num4;
				num5 = num2 - 160f;
				for (int j = 0; j < 5; j++)
				{
					while (!allPacks[num6].Enabled)
					{
						num6 = (num6 + 1) % allPacks.Length;
						if (num6 == num4)
						{
							return false;
						}
					}
					if (iMousePos.X >= num5 && iMousePos.X <= num5 + 64f)
					{
						oIndex = num6;
						return true;
					}
					num6 = (num6 + 1) % allPacks.Length;
					num5 += 64f;
				}
			}
			else
			{
				Vector2 vector = this.mPacksText.Font.MeasureText(this.mPacksText.Characters, true);
				if (iMousePos.X >= 128f && iMousePos.X <= 128f + vector.X && iMousePos.Y >= 368f && iMousePos.Y <= 408f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002D0B RID: 11531 RVA: 0x001649B0 File Offset: 0x00162BB0
		private bool HitGamer(ref Vector2 iMousePos, out int oIndex)
		{
			if (iMousePos.X >= 528f && iMousePos.X <= 1008f)
			{
				float num = 89f;
				float num2 = 201f;
				for (int i = 0; i < 4; i++)
				{
					if (iMousePos.Y >= num && iMousePos.Y <= num2)
					{
						oIndex = i;
						return true;
					}
					num += 137f;
					num2 += 137f;
				}
			}
			oIndex = -1;
			return false;
		}

		// Token: 0x06002D0C RID: 11532 RVA: 0x00164A1C File Offset: 0x00162C1C
		private bool HitAvatar(int iID, ref Vector2 iMousePos, out int oIndex)
		{
			int count = SubMenuCharacterSelect.mRobeRepresentations.Count;
			float num = 76.5f + 137f * (float)iID;
			float num2 = num + 112f + 25f;
			if (iMousePos.Y >= num && iMousePos.Y <= num2)
			{
				bool flag = count > 4;
				float num3 = 576f;
				float num4 = num3 + 96f;
				for (int i = 0; i < Math.Min(count, 4); i++)
				{
					if (iMousePos.X >= num3 && iMousePos.X <= num4)
					{
						int num5 = 0;
						if (flag)
						{
							num5 = this.mPlayerSlots[iID].ScrollValue;
						}
						oIndex = (i + num5) % count;
						return true;
					}
					num3 += 96f;
					num4 += 96f;
				}
			}
			oIndex = -1;
			return false;
		}

		// Token: 0x06002D0D RID: 11533 RVA: 0x00164AE4 File Offset: 0x00162CE4
		private bool HitColor(int iID, ref Vector2 iMousePos, out int oIndex)
		{
			Vector2 vector = default(Vector2);
			vector.X = 936f - (float)(Defines.PLAYERCOLORS.Length / 2) * 34f;
			vector.Y = 89f + (float)iID * 137f + 56f - 34f;
			for (int i = 0; i < Defines.PLAYERCOLORS.Length; i++)
			{
				if (iMousePos.X >= vector.X && iMousePos.X <= vector.X + 34f && iMousePos.Y >= vector.Y && iMousePos.Y <= vector.Y + 34f)
				{
					oIndex = i;
					return true;
				}
				if (i == Defines.PLAYERCOLORS.Length / 2)
				{
					vector.X -= 34f * (float)(Defines.PLAYERCOLORS.Length / 2);
					vector.Y += 34f;
				}
				else
				{
					vector.X += 34f;
				}
			}
			oIndex = -1;
			return false;
		}

		// Token: 0x06002D0E RID: 11534 RVA: 0x00164BF4 File Offset: 0x00162DF4
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			float num = this.mOptionsAlpha;
			Vector2 vector = default(Vector2);
			vector.X = 544f;
			vector.Y = 710f;
			NetworkChat.Instance.Draw(ref vector);
			Matrix transform = default(Matrix);
			transform.M11 = (transform.M22 = (transform.M44 = 1f));
			Vector4 color = Vector4.One;
			bool flag = NetworkManager.Instance.State == NetworkState.Server || NetworkManager.Instance.State == NetworkState.Offline;
			Point point = new Point(64, 480);
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			if (this.mLoadingLevels || SubMenuCharacterSelect.mLevelRepresentations == null || SubMenuCharacterSelect.mLevelRepresentations.Count == 0)
			{
				this.mSelectLevelButton.Enabled = (this.mSelectLevelButton.Selected = false);
				this.mSelectLevelButton.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LOADING));
				this.DrawLevel(-1, false, false, ref point, 448, 150, 1f);
			}
			else
			{
				this.mSelectLevelButton.Enabled = true;
				this.mSelectLevelButton.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_TT_CHANGE_LEVEL));
				float num2 = this.mLevelSelectAlpha;
				if (num2 > 0f)
				{
					Point point2 = default(Point);
					point2.X = 64;
					point2.Y = 64;
					this.mSpecialScrollBar.Position = new Vector2((float)point2.X + 416f, (float)point2.Y + 420f);
					color.W *= num2;
					this.mSpecialScrollBar.Color = color;
					this.mSpecialScrollBar.Draw(this.mEffect);
					int value = this.mSpecialScrollBar.Value;
					for (int i = value; i < Math.Min(value + 6, SubMenuCharacterSelect.mLevelRepresentations.Count); i++)
					{
						point2.Y += 38;
						bool flag2 = i == this.mSelectedPack;
						if (this.GameType != GameType.Mythos && this.GameType != GameType.Campaign)
						{
							this.DrawLevel(i, flag2, SubMenuCharacterSelect.mLevelRepresentations[i].IsCustom, ref point2, 384, 100, num2);
						}
						color = (flag2 ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
						color.W *= num2;
						this.mEffect.Color = color;
						if (SubMenuCharacterSelect.mLevelRepresentations[i].Title != null)
						{
							SubMenuCharacterSelect.mLevelRepresentations[i].Title.Draw(this.mEffect, (float)point2.X + 192f, (float)point2.Y - 32f, 0.8f);
						}
						if (this.GameType == GameType.Mythos || this.GameType == GameType.Campaign)
						{
							SubMenuCharacterSelect.mLevelRepresentations[i].Descr.Draw(this.mEffect, (float)point2.X + 192f, (float)point2.Y, 0.7f);
							this.mEffect.Saturation = (flag2 ? 1.3f : 0.7f);
							Vector4 iColor = Vector4.One * (flag2 ? 1.5f : 1f);
							iColor.W *= num2;
							this.mEffect.CommitChanges();
							base.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 286, 48), new Rectangle(point2.X, point2.Y - 36, 116, 32), iColor);
							base.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(770, 976, 286, 48), new Rectangle(point2.X + 268, point2.Y - 36, 116, 32), iColor);
							this.mEffect.Saturation = 1f;
						}
						point2.Y += 102;
					}
				}
			}
			float num3 = this.mPackSelectAlpha;
			if (num3 > 0f)
			{
				this.mSpecialScrollBar.Position = new Vector2(480f, 96f + this.mSpecialScrollBar.Height * 0.5f);
				color.W *= num3;
				this.mSpecialScrollBar.Color = color;
				this.mSpecialScrollBar.Draw(this.mEffect);
				this.DrawPacksList(num3);
				this.mPackCloseButton.Selected = (this.mSelectedPosition == 5 && this.mSelectedPack < 0);
				this.mPackCloseButton.Position = new Vector2(512f, this.mSpecialScrollBar.Position.Y + this.mSpecialScrollBar.Height * 0.5f + 32f);
				this.mPackCloseButton.Draw(this.mEffect, 1f, num3);
			}
			if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
			{
				if (!this.HasSelectedLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingPacks)
				{
					if (flag)
					{
						this.DrawLevel(-1, true, false, ref point, 448, 150, 1f);
					}
					else
					{
						Vector4 color2 = this.mEffect.Color;
						this.mEffect.Color = SubMenuCharacterSelect.LEVEL_DESCR_COLOR_NONCAMP;
						vector.X = (float)point.X + 224f;
						vector.Y = (float)point.Y + 35f;
						this.mChapterName.SetText(SubMenuCharacterSelect.sServerChangingLevelText);
						this.mChapterName.Draw(this.mEffect, vector.X, vector.Y, 0.8f);
						this.mEffect.Color = color2;
					}
				}
				else if (this.HasSelectedLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingPacks)
				{
					int levelSortedIndex = this.GetLevelSortedIndex(this.mGameSettings.Level);
					if (levelSortedIndex == -1)
					{
						this.HasSelectedLevel = false;
					}
					else
					{
						this.DrawLevel(levelSortedIndex, this.mSelectedPosition == 6, this.mCustomLevel, ref point, 448, 150, num);
					}
				}
				if (this.HasSelectedLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingLevel)
				{
					Vector4 color3 = this.mEffect.Color;
					this.mEffect.Color = SubMenuCharacterSelect.LEVEL_DESCR_COLOR_NONCAMP;
					Vector2 vector2 = this.mChapterName.Font.MeasureText(this.mChapterName.Characters, true);
					vector.X = (float)point.X + 224f;
					if ((int)vector2.X >= 545)
					{
						vector.X += 10f;
					}
					vector.Y = (float)point.Y + 155f;
					this.mChapterName.Draw(this.mEffect, vector.X, vector.Y, 0.8f);
					vector.X = (float)point.X + 30f;
					vector.Y += vector2.Y - 7f;
					this.mChapterDescription.Draw(this.mEffect, vector.X, vector.Y, 0.45f);
					this.mEffect.Color = color3;
				}
			}
			else
			{
				bool flag3 = this.mSelectedPosition == 9;
				if (this.mCurrentState != SubMenuCharacterSelect.State.ChangingLevel)
				{
					Vector4 color4 = flag3 ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
					color4.W = (1f - this.mLevelSelectAlpha) * color4.W;
					this.mEffect.Color = color4;
					vector.X = 288f;
					vector.Y = 96f;
					this.mEffect.Saturation = (flag3 ? 1.3f : 1f);
					this.mChapterName.Draw(this.mEffect, vector.X, vector.Y);
					vector.Y += (float)this.mChapterName.Font.LineHeight;
					this.mChapterDescription.Draw(this.mEffect, vector.X, vector.Y, 0.8f);
					this.mEffect.Saturation = 1f;
				}
				this.mEffect.Color = MenuItem.COLOR;
				if (this.HasSelectedLevel && this.mGameSettings.Level > -1)
				{
					CampaignNode campaignNode = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level) as CampaignNode;
					if (campaignNode.Cutscene != null)
					{
						Vector2 vector3;
						float num4;
						campaignNode.Cutscene.GetCamera(float.MaxValue, out vector3, out num4);
						Rectangle iScrRect = default(Rectangle);
						iScrRect.Width = (iScrRect.Height = (int)(1024f / num4));
						iScrRect.X = (int)(vector3.X - 512f / num4);
						iScrRect.Y = (int)(vector3.Y - 512f / num4);
						float num5 = 1.3f;
						flag3 = (this.mSelectedPosition == 9);
						Vector4 iColor2;
						if (flag3)
						{
							iColor2 = new Vector4(num5, num5, num5, Math.Min(num5, 1f - this.mLevelSelectAlpha));
						}
						else
						{
							iColor2 = new Vector4(1f, 1f, 1f, 1f - this.mLevelSelectAlpha);
						}
						this.mEffect.Saturation = (flag3 ? 1.3f : 1f);
						this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
						this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
						this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
						base.DrawGraphics(this.mMapMaskTexture, new Rectangle(0, 0, this.mMapMaskTexture.Width, this.mMapMaskTexture.Height), this.mMapRect, iColor2);
						this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
						this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
						this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseDestinationAlpha;
						this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationAlpha;
						this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
						this.mEffect.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.Zero;
						this.mEffect.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One;
						if (this.mLevelSelectAlpha == 0f)
						{
							base.DrawGraphics(this.mMapTexture, iScrRect, this.mMapRect, iColor2);
						}
						else
						{
							base.DrawGraphics(this.mMapTexture, iScrRect, this.mMapRect);
						}
						this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
						this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
						this.mEffect.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.One;
						this.mEffect.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.Zero;
						this.mEffect.Color = Vector4.One;
						this.mEffect.Saturation = 1f;
					}
				}
			}
			int mSelectedPosition = this.mSelectedPosition;
			this.mEffect.TextureOffset = Vector2.Zero;
			this.mEffect.TextureScale = Vector2.One;
			float x = 544f;
			float y = 89f;
			vector.X = x;
			vector.Y = y;
			color = Vector4.One;
			Player[] players = Game.Instance.Players;
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mAvatarVertices, 0, 24);
			this.mEffect.GraphicsDevice.Indices = this.mIndexBuffer;
			this.mEffect.GraphicsDevice.VertexDeclaration = this.mAvatarVertexDeclaration;
			for (int j = 0; j < 4; j++)
			{
				this.DrawSlotBackground(j, ref vector, this.mPlayerSlots[j].State == SubMenuCharacterSelect.GamerState.Locked, players[j].Playing, j == this.mSelectedPosition);
				vector.Y += 137f;
			}
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
			this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			vector.Y = 89f;
			for (int k = 0; k < 4; k++)
			{
				Gamer gamer = players[k].Gamer;
				bool playing = players[k].Playing;
				if (playing && gamer != null)
				{
					if (gamer == Gamer.INVALID_GAMER)
					{
						this.mEffect.TextureOffset = Vector2.Zero;
						this.mEffect.TextureScale = Vector2.One;
						this.mEffect.Saturation = 1f;
						this.mEffect.Color = Vector4.One;
						transform.M11 = 1f;
						transform.M22 = 1f;
						transform.M41 = vector.X + 112f;
						transform.M42 = vector.Y - 16f;
						this.mEffect.Transform = transform;
						this.mEffect.CommitChanges();
						this.mEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 32, 0, 16, 0, 18);
					}
					else if ((NetworkManager.Instance.State == NetworkState.Client || gamer is NetworkGamer) && (this.mPlayerSlots[k].State == SubMenuCharacterSelect.GamerState.Open || this.mPlayerSlots[k].State == SubMenuCharacterSelect.GamerState.Ready))
					{
						color.X = (color.Y = (color.Z = 1f));
						color.W = 1f;
						float num6;
						if (this.mPlayerSlots[k].State == SubMenuCharacterSelect.GamerState.Ready || (gamer is NetworkGamer && NetworkManager.Instance.State == NetworkState.Client && (gamer as NetworkGamer).ClientID == NetworkManager.Instance.Interface.ServerID))
						{
							num6 = 1f;
						}
						else
						{
							num6 = 0f;
						}
						if (this.mPlayerSlots[k].SelectedItem == 0)
						{
							num6 += 0.5f;
							color.X += 0.5f;
							color.Y += 0.5f;
							color.Z += 0.5f;
						}
						this.mEffect.Saturation = num6;
						this.mEffect.Color = color;
						this.mEffect.TextureOffset = new Vector2(912f / (float)SubMenu.sPagesTexture.Width, 416f / (float)SubMenu.sPagesTexture.Height);
						this.mEffect.TextureScale = new Vector2(128f / (float)SubMenu.sPagesTexture.Width, 80f / (float)SubMenu.sPagesTexture.Height);
						transform.M11 = 128f;
						transform.M22 = 80f;
						transform.M41 = vector.X + 448f - 64f + 48f;
						transform.M42 = vector.Y + 112f - 40f;
						this.mEffect.Transform = transform;
						this.mEffect.CommitChanges();
						this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
					}
				}
				vector.Y += 137f;
			}
			this.mEffect.TextureOffset = Vector2.Zero;
			this.mEffect.TextureScale = Vector2.One;
			vector.Y = 593f;
			vector.Y = 764f;
			for (int l = 0; l < 2; l++)
			{
				vector.Y += 112f;
			}
			this.mEffect.Saturation = 1f;
			vector.X = 544f;
			vector.Y = y;
			for (int m = 0; m < 4; m++)
			{
				Gamer gamer2 = players[m].Gamer;
				bool playing2 = players[m].Playing;
				if (playing2 && gamer2 != null)
				{
					if (gamer2 == Gamer.INVALID_GAMER)
					{
						MenuScrollBar menuScrollBar = this.mGamerScrollBars[m];
						Vector2 vector4 = default(Vector2);
						menuScrollBar.Draw(this.mEffect);
						vector4.X = vector.X + 256f;
						vector4.Y = vector.Y + 56f - 62f;
						int value2 = menuScrollBar.Value;
						for (int n = value2; n < Math.Min(value2 + 4, this.mGamerItems.Count); n++)
						{
							if (n < Profile.Instance.Gamers.Count && Profile.Instance.Gamers.Values[n].InUse)
							{
								this.mEffect.Color = MenuItem.COLOR_DISABLED;
							}
							else if (n == this.mPlayerSlots[m].SelectedItem)
							{
								this.mEffect.Color = MenuItem.COLOR_SELECTED;
							}
							else
							{
								color = Defines.DIALOGUE_COLOR_DEFAULT;
								color.X *= 1.333f;
								color.Y *= 1.333f;
								color.Z *= 1.333f;
								this.mEffect.Color = color;
							}
							float x2 = this.mGamerItems[n].Font.MeasureText(this.mGamerItems[n].Characters, true).X;
							transform.M11 = Math.Min(0.9f, 260f / x2);
							transform.M22 = 0.9f;
							transform.M41 = vector4.X;
							transform.M42 = vector4.Y;
							transform.M44 = 1f;
							this.mGamerItems[n].Draw(this.mEffect, ref transform);
							vector4.Y += 31f;
						}
					}
					else
					{
						if (gamer2 != null && !(gamer2 is NetworkGamer))
						{
							int controllerType = this.mPlayerSlots[m].ControllerType;
							if (controllerType >= 0)
							{
								Rectangle iScrRect2 = new Rectangle(0, 0, 128, 64);
								Rectangle iDestRect = new Rectangle(iDestRect.X = (int)(vector.X + 448f + -108.8f), iDestRect.Y = (int)(vector.Y + 56f - 32f + -54.4f), iScrRect2.Width, iScrRect2.Height);
								base.DrawGraphics(SubMenuCharacterSelect.mControllerTextures[controllerType], iScrRect2, iDestRect, new Vector4(1f, 1f, 1f, 1f));
							}
						}
						if (this.mPlayerSlots[m].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
						{
							this.DrawAvatars(m, ref vector);
							this.mEffect.OverlayTextureEnabled = false;
						}
						else if (this.mPlayerSlots[m].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
						{
							Vector2 vector5 = default(Vector2);
							vector5.X = vector.X + 224f - 144f;
							vector5.Y = vector.Y + 56f;
							int num7 = this.mPlayerSlots[m].SelectedItem;
							if (num7 < 0 || num7 >= Defines.PLAYERCOLORS.Length)
							{
								num7 = (int)players[m].Color;
							}
							this.DrawAvatar(gamer2.Avatar.Thumb, this.mPlayerSlots[m].Custom, Defines.PLAYERCOLORS[num7], ref vector5, 0.5f);
							this.mEffect.OverlayTextureEnabled = false;
							Vector2 textureOffset = default(Vector2);
							textureOffset.X = -64f / (float)SubMenu.sPagesTexture.Width;
							transform.M41 = vector.X + 448f - 56f - ((float)(Defines.PLAYERCOLORS.Length / 2) - 0.5f) * 34f;
							transform.M42 = vector.Y + 56f - 17f;
							for (int num8 = 0; num8 < Defines.PLAYERCOLORS.Length; num8++)
							{
								if (num8 == num7)
								{
									transform.M11 = (transform.M22 = 50f);
								}
								else
								{
									transform.M11 = (transform.M22 = 38f);
								}
								this.mEffect.Texture = SubMenu.sPagesTexture;
								this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
								this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
								color.X = (color.Y = (color.Z = ((num8 == num7) ? 1.5f : 1f)));
								color.W = 1f;
								this.mEffect.Color = color;
								this.mEffect.Transform = transform;
								this.mEffect.TextureOffset = default(Vector2);
								this.mEffect.CommitChanges();
								this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 48, 2);
								this.mEffect.Color = new Vector4(Defines.PLAYERCOLORS[num8], 1f);
								this.mEffect.TextureOffset = textureOffset;
								this.mEffect.CommitChanges();
								this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 48, 2);
								if (num8 == Defines.PLAYERCOLORS.Length / 2)
								{
									transform.M41 -= 34f * (float)num8;
									transform.M42 += 34f;
								}
								else
								{
									transform.M41 += 34f;
								}
							}
							this.mEffect.TextureOffset = default(Vector2);
						}
						else
						{
							Vector2 vector6 = default(Vector2);
							vector6.X = vector.X + 224f - 144f;
							vector6.Y = vector.Y + 56f;
							transform.M11 = Math.Min(1f, 288f / this.mPlayerSlots[m].Name.Font.MeasureText(this.mPlayerSlots[m].Name.Characters, true).X);
							transform.M22 = 1f;
							transform.M41 = vector.X + 144f;
							transform.M42 = vector.Y + 16f;
							transform.M44 = 1f;
							this.mEffect.Color = Defines.DIALOGUE_COLOR_DEFAULT * 1.2f;
							this.mPlayerSlots[m].Name.Draw(this.mEffect, ref transform);
							if (players[m].Gamer is NetworkGamer)
							{
								this.mEffect.Color = Vector4.One;
								this.mEffect.VertexColorEnabled = true;
								this.mPlayerSlots[m].LatencyText.Draw(this.mEffect, vector.X + 144f, vector.Y + 112f - 16f - (float)this.mPlayerSlots[m].LatencyText.Font.LineHeight);
								this.mEffect.VertexColorEnabled = false;
							}
							this.DrawAvatar(gamer2.Avatar.Thumb, this.mPlayerSlots[m].Custom, Defines.PLAYERCOLORS[(int)players[m].Color], ref vector6, 0.5f);
							this.mEffect.OverlayTextureEnabled = false;
						}
					}
				}
				else if (this.mPlayerSlots[m].State != SubMenuCharacterSelect.GamerState.Locked)
				{
					this.mEffect.Color = ((this.mSelectedPosition == m) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
					this.mOpenText.Draw(this.mEffect, vector.X + 112f, vector.Y + 16f);
				}
				else
				{
					this.mEffect.Color = MenuItem.COLOR_DISABLED;
					this.mClosedText.Draw(this.mEffect, vector.X + 112f, vector.Y + 16f);
				}
				vector.Y += 137f;
			}
			if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.StoryChallange && this.mGameSettings.GameType != GameType.Mythos && !(this.mVersusSettings is Krietor.Settings))
			{
				this.DrawPacksOverview(num);
			}
			this.mStartButton.Position = new Vector2(992f, SubMenu.BACK_POSITION.Y + 40f);
			if (!this.mStartButton.Enabled)
			{
				this.mStartButton.Selected = false;
				Vector4 color5 = this.mEffect.Color;
				this.mEffect.Color = new Vector4(0.75f, 0.75f, 0.75f, 1f);
				this.mStartButton.Draw(this.mEffect);
				this.mEffect.Color = color5;
			}
			else
			{
				this.mStartButton.Selected = (this.mSelectedPosition == 4);
				this.mStartButton.Draw(this.mEffect);
			}
			this.mBackButton.Draw(this.mEffect);
			if (this.mGameSettings.GameType == GameType.Versus)
			{
				this.DrawVSSettings(num);
			}
			else if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.StoryChallange)
			{
				this.mEffect.Saturation = 1f;
				Rectangle iScrRect3 = default(Rectangle);
				iScrRect3.X = 800;
				iScrRect3.Y = 1250;
				iScrRect3.Width = 400;
				iScrRect3.Height = 250;
				Rectangle iDestRect2 = default(Rectangle);
				iDestRect2.Width = iScrRect3.Width;
				iDestRect2.Height = iScrRect3.Height;
				iDestRect2.X = 64 + (448 - iDestRect2.Width) / 2;
				iDestRect2.Y = 96;
				base.DrawGraphics(this.mMagicksTexture, iScrRect3, iDestRect2, new Vector4(1f, 1f, 1f, num));
			}
			this.mGamerDropDownMenu.Draw(this.mEffect);
			this.mAdminDropDownMenu.Draw(this.mEffect);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06002D0F RID: 11535 RVA: 0x00166790 File Offset: 0x00164990
		private void DrawLevel(int idx, bool iSelected, bool iCustom, ref Point iPos, int iMaxWidth, int iMaxHeight, float iAlpha)
		{
			if (this.mValidatingLevels)
			{
				Vector4 color = MenuItem.COLOR;
				color.W *= iAlpha;
				this.mEffect.Color = color;
				this.mLoadingText.Draw(this.mEffect, (float)iPos.X + (float)iMaxWidth * 0.5f, (float)iPos.Y + (float)(iMaxHeight - this.mLoadingText.Font.LineHeight) * 0.5f);
				return;
			}
			if (idx == -1)
			{
				this.mSelectLevelButton.Draw(this.mEffect, this.mSelectLevelButton.Scale, 1f);
				return;
			}
			Texture2D texture2D = null;
			if (SubMenuCharacterSelect.mLevelRepresentations != null)
			{
				texture2D = SubMenuCharacterSelect.mLevelRepresentations[idx].PreviewImage;
			}
			if (texture2D == null || texture2D.IsDisposed)
			{
				Vector4 color2 = MenuItem.COLOR;
				color2.W *= iAlpha;
				this.mEffect.Color = color2;
				this.mLoadingText.Draw(this.mEffect, (float)iPos.X + (float)iMaxWidth * 0.5f, (float)iPos.Y + (float)(iMaxHeight - this.mLoadingText.Font.LineHeight) * 0.5f);
				return;
			}
			Vector4 vector = default(Vector4);
			vector.W = iAlpha;
			Rectangle iScrRect = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			float saturation;
			if (iSelected)
			{
				saturation = 1.3f;
				vector.X = (vector.Y = (vector.Z = 1.5f));
			}
			else
			{
				saturation = 1f;
				vector.X = (vector.Y = (vector.Z = 1f));
			}
			float num = Math.Min((float)iMaxWidth / (float)iScrRect.Width, (float)iMaxHeight / (float)iScrRect.Height);
			Rectangle iDestRect = new Rectangle(iPos.X, iPos.Y, (int)((float)iScrRect.Width * num), (int)((float)iScrRect.Height * num));
			iDestRect.X += (iMaxWidth - iDestRect.Width) / 2;
			this.mEffect.Saturation = saturation;
			bool flag = this.Level_CheckIfLocked(idx);
			bool flag2 = !flag && !this.Level_CheckIfUsed(idx) && !iCustom;
			bool flag3 = this.Level_CheckIfFree(idx);
			bool flag4 = this.Level_CheckIfNew(idx);
			if (texture2D != null && !texture2D.IsDisposed)
			{
				base.DrawGraphics(texture2D, iScrRect, iDestRect, flag ? new Vector4(0.75f, 0.75f, 0.75f, 1f) : vector);
				if (iCustom && !flag)
				{
					iScrRect.Width = this.mCustomLevelOverlay.Width;
					iScrRect.Height = this.mCustomLevelOverlay.Height;
					num = Math.Min((float)iMaxWidth / (float)iScrRect.Width, (float)iMaxHeight / (float)iScrRect.Height);
					base.DrawGraphics(this.mCustomLevelOverlay, iScrRect, iDestRect, vector);
				}
				if (flag3)
				{
					if (flag)
					{
						base.DrawGraphics(this.mLevelFreeAndLockedOverlay, iScrRect, iDestRect, vector);
					}
					else if (flag2)
					{
						base.DrawGraphics(this.mLevelFreeAndUnusedOverlay, iScrRect, iDestRect, vector);
					}
					else
					{
						base.DrawGraphics(this.mLevelFreeOverlay, iScrRect, iDestRect, vector);
					}
				}
				else if (flag)
				{
					base.DrawGraphics(this.mLevelLockedOverlay, iScrRect, iDestRect, vector);
				}
				else if (flag2)
				{
					base.DrawGraphics(this.mLevelUnusedOverlay, iScrRect, iDestRect, vector);
				}
				if (flag4)
				{
					base.DrawGraphics(this.mLevelNewOverlay, iScrRect, iDestRect, vector);
				}
				this.mEffect.Saturation = 1f;
				return;
			}
			if (this.mLoadingLevels)
			{
				Thread.Sleep(0);
				return;
			}
			this.mLoadingLevels = true;
			Game.Instance.AddLoadTask(new Action(this.LoadLevelPreviews));
		}

		// Token: 0x06002D10 RID: 11536 RVA: 0x00166B3C File Offset: 0x00164D3C
		private void DrawVSSettings(float iAlpha)
		{
			Vector4 color = default(Vector4);
			this.mGameModeBox.Selected = (this.mSelectedPosition == 7);
			Vector2 position = this.mGameModeBox.Position;
			position.Y += 16f;
			position.X += this.mGameModeBox.Size.X - 16f;
			position.Y += this.mGameModeBox.Size.Y + 85f;
			color.X = (color.Y = (color.Z = 1f));
			color.W = iAlpha;
			this.mSettingsScrollbar.Color = color;
			this.mSettingsScrollbar.Scale = 0.75f;
			this.mSettingsScrollbar.Height = 244f;
			this.mSettingsScrollbar.Position = position;
			if (this.mSettingsScrollbar.MaxValue > 0)
			{
				this.mSettingsScrollbar.Draw(this.mEffect);
			}
			if (!this.mGameModeBox.Enabled)
			{
				color = this.mGameModeBox.ColorDisabled;
			}
			else if (this.mGameModeBox.Selected && !this.mGameModeBox.IsDown)
			{
				color = this.mGameModeBox.ColorSelected;
			}
			else
			{
				color = this.mGameModeBox.Color;
			}
			color.W *= iAlpha;
			this.mEffect.Color = color;
			this.mGameModeTitle.Draw(this.mEffect, 64f, this.mGameModeBox.Position.Y);
			int num = this.mSelectedPosition - 8;
			if (num >= 0 && num < this.mVersusSettings.MenuItems.Count)
			{
				while (this.mSettingsScrollbar.Value > num)
				{
					this.mSettingsScrollbar.Value--;
				}
				while (this.mSettingsScrollbar.Value < this.mSettingsScrollbar.MaxValue && this.mSettingsScrollbar.Value + 5 - 1 < num)
				{
					this.mSettingsScrollbar.Value++;
				}
			}
			position.X = this.mGameModeBox.Position.X + this.mGameModeBox.Size.X - 180f - 40f;
			position.Y = this.mGameModeBox.Position.Y + this.mGameModeBox.Size.Y + 16f + (float)(Math.Min(5, this.mVersusSettings.MenuItems.Count) - 1) * 34f;
			int value = this.mSettingsScrollbar.Value;
			for (int i = Math.Min(value + 5, this.mVersusSettings.MenuItems.Count) - 1; i >= value; i--)
			{
				DropDownBox dropDownBox = this.mVersusSettings.MenuItems[i];
				dropDownBox.Selected = (this.mSelectedPosition == 8 + i);
				if (!dropDownBox.Enabled)
				{
					color = dropDownBox.ColorDisabled;
				}
				else if (dropDownBox.Selected & !dropDownBox.IsDown)
				{
					color = dropDownBox.ColorSelected;
				}
				else
				{
					color = dropDownBox.Color;
				}
				color.W *= iAlpha;
				this.mEffect.Color = color;
				this.mVersusSettingsTitles[i].Draw(this.mEffect, 96f, position.Y, 0.9f);
				dropDownBox.Scale = 0.9f;
				dropDownBox.Position = position;
				dropDownBox.Draw(this.mEffect, dropDownBox.Scale, iAlpha);
				position.Y -= 34f;
			}
			this.mGameModeBox.Draw(this.mEffect, this.mGameModeBox.Scale, iAlpha);
		}

		// Token: 0x06002D11 RID: 11537 RVA: 0x00166F18 File Offset: 0x00165118
		private void DrawPacksList(float iAlpha)
		{
			ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
			MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
			Texture2D thumbnails = PackMan.Instance.Thumbnails;
			Vector2 textureScale = default(Vector2);
			Matrix transform = default(Matrix);
			transform.M44 = 1f;
			transform.M11 = 64f;
			transform.M22 = 64f;
			transform.M42 = this.mSpecialScrollBar.Position.Y - 192f;
			Vector2 textureOffset = new Vector2(((float)thumbnails.Width - 64f) / (float)thumbnails.Width, ((float)thumbnails.Height - 64f) / (float)thumbnails.Height);
			int num = 0;
			int value = this.mSpecialScrollBar.Value;
			bool flag = this.mSelectedPosition == 5;
			Vector4 color;
			if (value == 0)
			{
				color = MenuItem.COLOR;
				color.W *= iAlpha;
				this.mEffect.Color = color;
				textureScale.X = (textureScale.Y = 1f);
				this.mEffect.TextureScale = textureScale;
				this.mEffect.TextureOffset = default(Vector2);
				this.mItemsText.Draw(this.mEffect, 96f, transform.M42 - 14f);
				transform.M42 += 64f;
				num++;
			}
			color.W = iAlpha;
			this.mEffect.Texture = thumbnails;
			this.mEffect.TextureEnabled = true;
			this.mEffect.VertexColorEnabled = false;
			textureScale.X = 64f / (float)thumbnails.Width;
			textureScale.Y = 64f / (float)thumbnails.Height;
			this.mEffect.TextureScale = textureScale;
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
			this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			int num2 = 5 * Math.Max(value - 1, 0);
			while (num < 7 && num2 < itemPacks.Length)
			{
				transform.M41 = 160f;
				int num3 = 0;
				while (num2 < itemPacks.Length && num3 < 5)
				{
					ItemPack itemPack = itemPacks[num2];
					if (itemPack.License == HackHelper.License.Yes)
					{
						this.mEffect.TextureOffset = itemPack.ThumbOffset;
					}
					else
					{
						this.mEffect.TextureOffset = default(Vector2);
					}
					if (itemPack.Enabled)
					{
						this.mEffect.Saturation = 1f;
					}
					else
					{
						this.mEffect.Saturation = 0f;
					}
					if (flag && this.mSelectedPack == num2)
					{
						color.X = (color.Y = (color.Z = 1.5f));
					}
					else
					{
						color.X = (color.Y = (color.Z = 1f));
					}
					this.mEffect.Color = color;
					this.mEffect.Transform = transform;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
					if (!itemPack.IsUsed && itemPack.License == HackHelper.License.Yes && itemPack.Enabled)
					{
						this.mEffect.TextureOffset = textureOffset;
						this.mEffect.CommitChanges();
						this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
					}
					transform.M41 += 64f;
					num2++;
					num3++;
				}
				transform.M42 += 64f;
				num++;
			}
			int num4 = itemPacks.Length / 5;
			if (itemPacks.Length % 5 != 0)
			{
				num4++;
			}
			if (num < 7 && num4 + 1 - value >= 0)
			{
				color = MenuItem.COLOR;
				color.W *= iAlpha;
				this.mEffect.Color = color;
				textureScale.X = (textureScale.Y = 1f);
				this.mEffect.TextureScale = textureScale;
				this.mEffect.TextureOffset = default(Vector2);
				this.mMagicksText.Draw(this.mEffect, 96f, transform.M42 - 14f);
				transform.M42 += 64f;
				num++;
			}
			color.W = iAlpha;
			this.mEffect.Texture = thumbnails;
			this.mEffect.TextureEnabled = true;
			this.mEffect.VertexColorEnabled = false;
			textureScale.X = 64f / (float)thumbnails.Width;
			textureScale.Y = 64f / (float)thumbnails.Height;
			this.mEffect.TextureScale = textureScale;
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
			this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			int num5 = 5 * Math.Max(value - 2 - num4, 0);
			while (num < 7 && num5 < magickPacks.Length)
			{
				transform.M41 = 160f;
				int num6 = 0;
				while (num5 < magickPacks.Length && num6 < 5)
				{
					MagickPack magickPack = magickPacks[num5];
					if (magickPack.License == HackHelper.License.Yes)
					{
						this.mEffect.TextureOffset = magickPack.ThumbOffset;
					}
					else
					{
						this.mEffect.TextureOffset = default(Vector2);
					}
					if (magickPack.Enabled)
					{
						this.mEffect.Saturation = 1f;
					}
					else
					{
						this.mEffect.Saturation = 0f;
					}
					if (flag && this.mSelectedPack == num5 + itemPacks.Length)
					{
						color.X = (color.Y = (color.Z = 1.5f));
					}
					else
					{
						color.X = (color.Y = (color.Z = 1f));
					}
					this.mEffect.Color = color;
					this.mEffect.Transform = transform;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
					if (!magickPack.IsUsed && magickPack.License == HackHelper.License.Yes && magickPack.Enabled)
					{
						this.mEffect.TextureOffset = textureOffset;
						this.mEffect.CommitChanges();
						this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
					}
					transform.M41 += 64f;
					num5++;
					num6++;
				}
				transform.M42 += 64f;
				num++;
			}
			this.mEffect.TextureScale = Vector2.One;
			this.mEffect.TextureOffset = Vector2.Zero;
		}

		// Token: 0x06002D12 RID: 11538 RVA: 0x00167614 File Offset: 0x00165814
		private void DrawPacksOverview(float iAlpha)
		{
			bool flag = this.mSelectedPosition == 5;
			bool flag2 = DLC_StatusHelper.HasAnyUnusedItemPacks() || DLC_StatusHelper.HasAnyUnusedMagicPacks();
			Vector4 color = flag ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
			color.W *= iAlpha;
			this.mEffect.Color = color;
			this.mPacksText.Draw(this.mEffect, 128f, 368f);
			if (flag2 && this.mCurrentState == SubMenuCharacterSelect.State.Normal && !this.mValidatingLevels)
			{
				float num = this.mFont.MeasureText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_PACKS), false).X + 15f;
				this.mGenericStar.Position = new Vector2(128f + num, 368f);
				this.mGenericStar.Draw(this.mEffect);
			}
			Texture2D thumbnails = PackMan.Instance.Thumbnails;
			Vector2 textureScale = default(Vector2);
			Matrix transform = default(Matrix);
			transform.M44 = 1f;
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
			this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			float num2 = 288f;
			IPack[] allPacks = PackMan.Instance.AllPacks;
			int num3 = 0;
			for (int i = 0; i < allPacks.Length; i++)
			{
				if (allPacks[i].Enabled)
				{
					num3++;
				}
			}
			bool flag3 = num3 > 5;
			color.W = iAlpha;
			this.mEffect.Texture = SubMenu.sPagesTexture;
			textureScale.X = 64f / (float)SubMenu.sPagesTexture.Width;
			textureScale.Y = 64f / (float)SubMenu.sPagesTexture.Height;
			this.mEffect.TextureScale = textureScale;
			Vector2 textureOffset = default(Vector2);
			textureOffset.X = 1280f / (float)SubMenu.sPagesTexture.Width;
			textureOffset.Y = 96f / (float)SubMenu.sPagesTexture.Height;
			this.mEffect.TextureOffset = textureOffset;
			if (flag3)
			{
				transform.M42 = 440f;
				transform.M11 = 0f;
				transform.M22 = 0f;
				transform.M41 = num2 - 192f;
				transform.M12 = -64f;
				transform.M21 = 64f;
				this.mEffect.Transform = transform;
				if (flag && this.mSelectedPackScroll == ControllerDirection.Left)
				{
					color.X = (color.Y = (color.Z = 1.5f));
					this.mEffect.Saturation = 1.5f;
				}
				else
				{
					color.X = (color.Y = (color.Z = 1f));
					this.mEffect.Saturation = 1f;
				}
				this.mEffect.Color = color;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
				transform.M41 = num2 + 192f;
				transform.M12 = 64f;
				transform.M21 = -64f;
				this.mEffect.Transform = transform;
				if (flag && this.mSelectedPackScroll == ControllerDirection.Right)
				{
					color.X = (color.Y = (color.Z = 1.5f));
					this.mEffect.Saturation = 1.5f;
				}
				else
				{
					color.X = (color.Y = (color.Z = 1f));
					this.mEffect.Saturation = 1f;
				}
				this.mEffect.Color = color;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
			}
			transform.M41 = num2 - 128f;
			transform.M42 = 440f;
			transform.M11 = 64f;
			transform.M12 = 0f;
			transform.M22 = 64f;
			transform.M21 = 0f;
			this.mEffect.Texture = thumbnails;
			textureScale.X = 64f / (float)thumbnails.Width;
			textureScale.Y = 64f / (float)thumbnails.Height;
			this.mEffect.TextureScale = textureScale;
			this.mEffect.Saturation = 1f;
			if (!flag3)
			{
				this.mPackScrollValue = 0;
			}
			int num4 = this.mPackScrollValue;
			int num5 = num4;
			int num6 = Math.Min(num3, 5);
			for (int j = 0; j < num6; j++)
			{
				while (!allPacks[num5].Enabled)
				{
					num5 = (num5 + 1) % allPacks.Length;
					if (num5 == num4)
					{
						goto IL_5A1;
					}
				}
				color.W = 1f * iAlpha;
				if (flag && this.mSelectedPack == num5)
				{
					color.X = (color.Y = (color.Z = 1.5f));
				}
				else
				{
					color.X = (color.Y = (color.Z = 1f));
				}
				this.mEffect.Color = color;
				this.mEffect.TextureOffset = allPacks[num5].ThumbOffset;
				this.mEffect.Transform = transform;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
				transform.M41 += 64f;
				num5 = (num5 + 1) % allPacks.Length;
			}
			IL_5A1:
			this.mEffect.TextureScale = Vector2.One;
			this.mEffect.TextureOffset = Vector2.Zero;
		}

		// Token: 0x06002D13 RID: 11539 RVA: 0x00167BE4 File Offset: 0x00165DE4
		private void DrawSlotBackground(int iIndex, ref Vector2 iPos, bool iLocked, bool iInUse, bool iSelected)
		{
			this.mEffect.Texture = (this.mEffect.OverlayTexture = SubMenu.sPagesTexture);
			this.mEffect.TextureEnabled = (this.mEffect.OverlayTextureEnabled = true);
			Matrix transform = default(Matrix);
			transform.M11 = (transform.M22 = (transform.M44 = 1f));
			transform.M41 = iPos.X;
			transform.M42 = iPos.Y;
			this.mEffect.Transform = transform;
			this.mEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
			if (iLocked)
			{
				this.mEffect.Saturation = 0f;
			}
			else if (iSelected)
			{
				this.mEffect.Saturation = 1.5f;
			}
			else
			{
				this.mEffect.Saturation = 1f;
			}
			Vector4 vector = default(Vector4);
			vector.X = (vector.Y = (vector.Z = 1f));
			vector.W = 1f;
			if (!iInUse)
			{
				vector.W = 0.66f;
			}
			vector.X = (vector.Y = (vector.Z = (iSelected ? 1.5f : 1f)));
			this.mEffect.Color = vector;
			if (this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled)
			{
				Player player = Game.Instance.Players[iIndex];
				if (player.Playing && (player.Team & Factions.TEAM_RED) != Factions.NONE)
				{
					vector.X = Defines.PLAYERCOLORS[0].X;
					vector.Y = Defines.PLAYERCOLORS[0].Y;
					vector.Z = Defines.PLAYERCOLORS[0].Z;
				}
				else if (player.Playing && (player.Team & Factions.TEAM_BLUE) != Factions.NONE)
				{
					vector.X = Defines.PLAYERCOLORS[3].X;
					vector.Y = Defines.PLAYERCOLORS[3].Y;
					vector.Z = Defines.PLAYERCOLORS[3].Z;
				}
				else
				{
					vector.X = 0.75f;
					vector.Y = 0.75f;
					vector.Z = 0.75f;
				}
			}
			else
			{
				vector.X = 0.75f;
				vector.Y = 0.75f;
				vector.Z = 0.75f;
			}
			vector.W = 1f;
			this.mEffect.OverlayTint = vector;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 4, 0, 16, 0, 18);
			this.mEffect.OverlayTextureEnabled = false;
		}

		// Token: 0x06002D14 RID: 11540 RVA: 0x00167ED0 File Offset: 0x001660D0
		private void DrawAvatars(int iID, ref Vector2 iPos)
		{
			Player player = Game.Instance.Players[iID];
			Gamer gamer = player.Gamer;
			SortedList<string, Profile.PlayableAvatar> avatars = Profile.Instance.Avatars;
			Vector2 vector = default(Vector2);
			vector.X = iPos.X + 224f - 144f;
			vector.Y = iPos.Y + 56f;
			if (this.mValidatingLevels)
			{
				Vector4 color = MenuItem.COLOR;
				color.W *= 1f;
				this.mEffect.Color = color;
				this.mLoadingText.Draw(this.mEffect, iPos.X - 72f, iPos.Y + (float)this.mLoadingText.Font.LineHeight * 0.5f);
			}
			else
			{
				bool flag = SubMenuCharacterSelect.mRobeRepresentations.Count > 4;
				if (flag)
				{
					this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
					this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
					this.mEffect.Texture = SubMenu.sPagesTexture;
					this.mEffect.Color = new Vector4(1.25f, 1.25f, 1.25f, 1f);
					Matrix transform = default(Matrix);
					transform.M12 = -64f;
					transform.M21 = 64f;
					transform.M41 = vector.X - 72f;
					transform.M42 = vector.Y;
					transform.M44 = 1f;
					this.mEffect.Transform = transform;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 52, 2);
					transform.M12 = 64f;
					transform.M21 = -64f;
					transform.M41 += 432f;
					this.mEffect.Transform = transform;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 52, 2);
				}
				int num = 0;
				if (flag)
				{
					if (this.mPlayerSlots[iID].ScrollValue >= SubMenuCharacterSelect.mRobeRepresentations.Count)
					{
						this.mPlayerSlots[iID].ScrollValue = SubMenuCharacterSelect.mRobeRepresentations.Count - 1;
					}
					num = this.mPlayerSlots[iID].ScrollValue;
				}
				for (int i = 0; i < Math.Min(SubMenuCharacterSelect.mRobeRepresentations.Count, 4); i++)
				{
					bool flag2 = this.Robe_CheckIfLocked(num);
					bool flag3 = !flag2 && !this.Robe_CheckIfUsed(num);
					bool flag4 = this.Robe_CheckIfFree(num);
					bool flag5 = this.Robe_CheckIfNew(num);
					float iScale;
					if (this.mPlayerSlots[iID].SelectedItem == num || (this.mPlayerSlots[iID].SelectedItem < 0 && gamer != null && avatars.IndexOfKey(gamer.Avatar.Name) == SubMenuCharacterSelect.mRobeRepresentations[num].OriginalIndex))
					{
						iScale = 0.5f;
						this.mEffect.Saturation = (flag2 ? 0.25f : 1f);
					}
					else
					{
						iScale = 0.4f;
						this.mEffect.Saturation = 0.25f;
					}
					Vector3 pColor = Defines.PLAYERCOLORS[(int)player.Color] * (flag2 ? 0.75f : 1f);
					Texture2D thumb = avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[num].OriginalIndex].Thumb;
					this.DrawAvatar(thumb, SubMenuCharacterSelect.mRobeRepresentations[num].IsCustom, pColor, ref vector, iScale);
					Vector2 vector2 = vector;
					vector2.X += 3f;
					vector2.Y += 33f;
					this.mEffect.Saturation = 1f;
					if (flag4)
					{
						if (flag2)
						{
							this.DrawAvatar(this.mRobeFreeAndLockedOverlay, false, Vector3.One, ref vector2, iScale);
						}
						else if (flag3)
						{
							this.DrawAvatar(this.mRobeFreeAndUnusedOverlay, false, Vector3.One, ref vector2, iScale);
						}
						else
						{
							this.DrawAvatar(this.mRobeFreeOverlay, false, Vector3.One, ref vector2, iScale);
						}
					}
					else if (flag2)
					{
						this.DrawAvatar(this.mRobeLockedOverlay, false, Vector3.One, ref vector2, iScale);
					}
					else if (flag3)
					{
						this.DrawAvatar(this.mRobeUnusedOverlay, false, Vector3.One, ref vector2, iScale);
					}
					if (flag5)
					{
						vector2 = vector;
						vector2.X += (float)(thumb.Width - this.mRobeNewOverlay.Width) - 6f;
						vector2.Y -= 26f;
						this.DrawAvatar(this.mRobeNewOverlay, false, Vector3.One, ref vector2, iScale);
					}
					vector.X += 96f;
					num = (num + 1) % SubMenuCharacterSelect.mRobeRepresentations.Count;
				}
			}
			this.mEffect.Saturation = 1f;
		}

		// Token: 0x06002D15 RID: 11541 RVA: 0x001683E8 File Offset: 0x001665E8
		private void DrawAvatar(Texture2D iTexture, bool iCustom, Vector3 pColor, ref Vector2 iPos, float iScale)
		{
			this.mEffect.Color = Vector4.One;
			this.mEffect.OverlayTint = new Vector4(pColor.X, pColor.Y, pColor.Z, 1f);
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mAvatarVertices, 0, 24);
			this.mEffect.GraphicsDevice.VertexDeclaration = this.mAvatarVertexDeclaration;
			GUIBasicEffect mEffect = this.mEffect;
			this.mEffect.OverlayTexture = iTexture;
			mEffect.Texture = iTexture;
			this.mEffect.TextureEnabled = true;
			this.mEffect.OverlayTextureEnabled = true;
			Matrix transform = default(Matrix);
			transform.M41 = iPos.X;
			transform.M42 = iPos.Y;
			transform.M11 = (float)iTexture.Width * iScale;
			transform.M22 = (float)iTexture.Height * 0.5f * iScale;
			transform.M44 = 1f;
			this.mEffect.Transform = transform;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			if (iCustom)
			{
				this.mEffect.TextureOffset = Vector2.Zero;
				this.mEffect.TextureScale = Vector2.One;
				this.mEffect.Texture = this.mCustomTexture;
				this.mEffect.OverlayTextureEnabled = false;
				this.mEffect.Color = Vector4.One;
				transform.M42 += (0.25f * (float)iTexture.Height - (float)this.mCustomTexture.Height * 0.5f) * iScale;
				transform.M11 = (float)this.mCustomTexture.Width * iScale;
				transform.M22 = (float)this.mCustomTexture.Height * iScale;
				this.mEffect.Transform = transform;
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mEffect.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Clamp;
				this.mEffect.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Clamp;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
			}
		}

		// Token: 0x06002D16 RID: 11542 RVA: 0x0016866C File Offset: 0x0016686C
		private void NewGamerCreated(string iName)
		{
			if (string.IsNullOrEmpty(iName))
			{
				return;
			}
			string text = null;
			if (this.mGameSettings.Level != -1)
			{
				GameType gameType = this.mGameSettings.GameType;
				switch (gameType)
				{
				case GameType.Campaign:
					text = LevelManager.Instance.VanillaCampaign[this.mGameSettings.Level].PreferredAvatar;
					break;
				case GameType.Challenge:
					text = LevelManager.Instance.Challenges[this.mGameSettings.Level].PreferredAvatar;
					break;
				case (GameType)3:
					break;
				case GameType.Versus:
					text = LevelManager.Instance.Versus[this.mGameSettings.Level].PreferredAvatar;
					break;
				default:
					if (gameType != GameType.Mythos)
					{
						if (gameType == GameType.StoryChallange)
						{
							text = LevelManager.Instance.StoryChallanges[this.mGameSettings.Level].PreferredAvatar;
						}
					}
					else
					{
						text = LevelManager.Instance.MythosCampaign[this.mGameSettings.Level].PreferredAvatar;
					}
					break;
				}
			}
			Profile.PlayableAvatar avatar = Profile.Instance.DefaultAvatar;
			Profile.PlayableAvatar playableAvatar;
			if (!string.IsNullOrEmpty(text) && Profile.Instance.Avatars.TryGetValue(text, out playableAvatar))
			{
				HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
				if (license == HackHelper.License.Yes || (license == HackHelper.License.Custom && this.AllowCustom))
				{
					avatar = playableAvatar;
				}
			}
			Gamer gamer = new Gamer(iName);
			gamer.Avatar = avatar;
			gamer.Color = (byte)(Profile.Instance.Gamers.Count % Defines.PLAYERCOLORS.Length);
			Profile.Instance.Add(gamer);
			this.GamerSelected(this.mNameInputController, gamer);
			this.mNameInputController = null;
		}

		// Token: 0x06002D17 RID: 11543 RVA: 0x001687F4 File Offset: 0x001669F4
		private void UpdateControllerIcon(Controller controller, int slotIndex)
		{
			int controllerType;
			if (controller is KeyboardMouseController)
			{
				controllerType = 0;
			}
			else if (controller is XInputController)
			{
				XInputController xinputController = controller as XInputController;
				controllerType = (int)(xinputController.PlayerIndex + 1);
			}
			else
			{
				controllerType = -1;
			}
			this.mPlayerSlots[slotIndex].ControllerType = controllerType;
		}

		// Token: 0x06002D18 RID: 11544 RVA: 0x0016883B File Offset: 0x00166A3B
		private void DaisyWheelChatInput(string text)
		{
			DaisyWheel.SetActionToCallWhenComplete(null);
			if (text != null)
			{
				NetworkChat.Instance.AddMessage(text);
			}
		}

		// Token: 0x06002D19 RID: 11545 RVA: 0x00168854 File Offset: 0x00166A54
		public override void ControllerA(Controller iSender)
		{
			if ((this.mOptionsAlpha > 0f && this.mOptionsAlpha < 1f) || RenderManager.Instance.IsTransitionActive)
			{
				return;
			}
			if (iSender.Player == null || !iSender.Player.Playing)
			{
				this.JoinPlayer(iSender, -1, null);
				return;
			}
			int id = iSender.Player.ID;
			if (iSender.Player.Gamer != Gamer.INVALID_GAMER && iSender is XInputController && this.mSelectedPosition == -1 && this.mCurrentState == SubMenuCharacterSelect.State.Normal && this.mPlayerSlots[id].State != SubMenuCharacterSelect.GamerState.CustomizingAvatar && this.mPlayerSlots[id].State != SubMenuCharacterSelect.GamerState.CustomizingColor)
			{
				if (!DaisyWheel.IsDisplaying)
				{
					DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelChatInput));
					DaisyWheel.TryShow(iSender, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_CHATMESSAGE), false, GamepadTextInputLineMode.GamepadTextInputLineModeMultipleLines, 255U);
				}
				return;
			}
			this.UpdateControllerIcon(iSender, iSender.Player.ID);
			if (iSender.Player.Gamer == Gamer.INVALID_GAMER)
			{
				int selectedItem = this.mPlayerSlots[id].SelectedItem;
				if (selectedItem >= Profile.Instance.Gamers.Count)
				{
					this.mNameInputController = iSender;
					string iDescr = LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_ENTER_NAME).ToUpper();
					this.mNameInputBox.Show(new Action<string>(this.NewGamerCreated), iSender, iDescr);
					return;
				}
				if (selectedItem >= 0 && !Profile.Instance.Gamers.Values[selectedItem].InUse)
				{
					this.GamerSelected(iSender, Profile.Instance.Gamers.Values[selectedItem]);
					Player player = iSender.Player;
					if (player.Gamer != null)
					{
						this.VerifyAvatar(ref player, false);
						return;
					}
				}
			}
			else
			{
				if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.Open && NetworkManager.Instance.State == NetworkState.Client)
				{
					this.SetReady(true, (byte)id);
					return;
				}
				if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
				{
					int selectedItem2 = this.mPlayerSlots[id].SelectedItem;
					if (selectedItem2 >= 0 && selectedItem2 < SubMenuCharacterSelect.mRobeRepresentations.Count)
					{
						int originalIndex = SubMenuCharacterSelect.mRobeRepresentations[selectedItem2].OriginalIndex;
						Profile.PlayableAvatar playableAvatar = Profile.Instance.Avatars.Values[originalIndex];
						uint num = 0U;
						uint num2 = 0U;
						if (DLC_StatusHelper.ValidateRobeLocked(playableAvatar, out num, out num2))
						{
							SteamUtils.ActivateGameOverlayToWebPage("http://store.steampowered.com/app/" + num2 + "/");
							return;
						}
						HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
						if (license == HackHelper.License.Yes || (license == HackHelper.License.Custom && (NetworkManager.Instance.State == NetworkState.Offline || !NetworkManager.Instance.Interface.IsVACSecure)))
						{
							if (!iSender.Player.Gamer.Avatar.Name.Equals(playableAvatar.Name))
							{
								this.mPlayerSlots[id].ConsecutiveColorChanges = -1;
							}
							iSender.Player.Gamer.Avatar = playableAvatar;
							this.mPlayerSlots[id].Custom = SubMenuCharacterSelect.mRobeRepresentations[selectedItem2].IsCustom;
							if (NetworkManager.Instance.State != NetworkState.Offline)
							{
								GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(iSender.Player);
								NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
							}
							ToolTipMan.Instance.Kill(iSender.Player, false);
							this.SetRobeUsed(playableAvatar.Name);
							this.mPlayerSlots[id].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
							this.mPlayerSlots[id].SelectedItem = (int)iSender.Player.Color;
							return;
						}
					}
				}
				else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
				{
					if (this.mPlayerSlots[id].SelectedItem >= 0 && this.mPlayerSlots[id].SelectedItem < Defines.PLAYERCOLORS.Length)
					{
						if (this.mPlayerSlots[id].SelectedItem != (int)iSender.Player.Color)
						{
							SubMenuCharacterSelect.PlayerState[] array = this.mPlayerSlots;
							int num3 = id;
							array[num3].ConsecutiveColorChanges = array[num3].ConsecutiveColorChanges + 1;
							if (this.mPlayerSlots[id].ConsecutiveColorChanges == 2)
							{
								AchievementsManager.Instance.AwardAchievement(null, "bluenoyelloooow");
							}
						}
						this.mPlayerSlots[id].State = SubMenuCharacterSelect.GamerState.Open;
						iSender.Player.Color = (byte)this.mPlayerSlots[id].SelectedItem;
						return;
					}
				}
				else if (NetworkManager.Instance.State != NetworkState.Client)
				{
					if (this.mGameModeBox.IsDown)
					{
						this.mGameModeBox.SelectedIndex = this.mGameModeBox.NewSelection;
						this.mGameModeBox.IsDown = false;
						return;
					}
					for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
					{
						DropDownBox dropDownBox = this.mVersusSettings.MenuItems[i];
						if (dropDownBox.IsDown)
						{
							dropDownBox.SelectedIndex = dropDownBox.NewSelection;
							dropDownBox.IsDown = false;
							return;
						}
					}
					if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
					{
						if (this.mSelectedPack >= 0 && this.mSelectedPack < SubMenuCharacterSelect.mLevelRepresentations.Count)
						{
							int levelOriginalIndex = this.GetLevelOriginalIndex(this.mSelectedPack);
							this.OnLevelChange(iSender, this.mGameSettings.GameType, levelOriginalIndex);
							ToolTipMan.Instance.Kill(null, false);
							this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
						}
						return;
					}
					if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
					{
						if (this.mSelectedPack >= 0 && this.mSelectedPack < PackMan.Instance.AllPacks.Length)
						{
							IPack pack = PackMan.Instance.AllPacks[this.mSelectedPack];
							pack.Enabled = !pack.Enabled;
							return;
						}
						this.mSelectedPosition = 5;
						this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
						return;
					}
					else
					{
						if (this.mCurrentState == SubMenuCharacterSelect.State.Normal && this.mSelectedPosition == 9 && this.mGameSettings.GameType != GameType.Versus)
						{
							this.mSpecialScrollBar.Height = 840f;
							this.mSpecialScrollBar.SetMaxValue(SubMenuCharacterSelect.mLevelRepresentations.Count - 6);
							ToolTipMan.Instance.Kill(null, false);
							if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus)
							{
								this.SortLevelRepList();
							}
							this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingLevel);
							return;
						}
						if (this.mSelectedPosition == 6 && this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos && NetworkManager.Instance.State != NetworkState.Client)
						{
							if (!this.HasSelectedLevel && (SubMenuCharacterSelect.mLevelRepresentations == null || SubMenuCharacterSelect.mLevelRepresentations.Count == 0 || !this.mSelectLevelButton.Enabled))
							{
								return;
							}
							this.mSpecialScrollBar.Height = (this.mSpecialScrollBar.Height = 840f);
							this.mSpecialScrollBar.SetMaxValue(SubMenuCharacterSelect.mLevelRepresentations.Count - 6);
							ToolTipMan.Instance.Kill(null, false);
							if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus)
							{
								this.SortLevelRepList();
							}
							this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingLevel);
							this.mCurrentController = iSender;
							return;
						}
						else
						{
							if (this.mSelectedPosition == 5 && this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos && NetworkManager.Instance.State != NetworkState.Client)
							{
								if (NetworkManager.Instance.State != NetworkState.Client)
								{
									int num4 = PackMan.Instance.ItemPacks.Length;
									int num5 = PackMan.Instance.MagickPacks.Length;
									int num6 = 2 + num4 / 5 + num5 / 5;
									if (num4 % 5 != 0)
									{
										num6++;
									}
									if (num5 % 5 != 0)
									{
										num6++;
									}
									this.mSpecialScrollBar.Height = 448f;
									this.mSpecialScrollBar.SetMaxValue(num6 - 7);
									ToolTipMan.Instance.Kill(null, false);
									this.mSelectedPack = 0;
									this.mSpecialScrollBar.Value = 0;
									this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingPacks);
								}
								return;
							}
							if (this.mSelectedPosition == 4 && this.mStartButton.Enabled && this.CanStart())
							{
								NetworkState state = NetworkManager.Instance.State;
								if (state == NetworkState.Offline)
								{
									this.StartLevel();
									return;
								}
								if (state == NetworkState.Server)
								{
									this.Start();
								}
								return;
							}
							else
							{
								if (this.mSelectedPosition == 7 && this.mGameSettings.GameType == GameType.Versus && this.mGameModeBox.Enabled)
								{
									this.mGameModeBox.IsDown = true;
									return;
								}
								if (this.mSelectedPosition >= 8 && this.mGameSettings.GameType == GameType.Versus)
								{
									DropDownBox dropDownBox2 = this.mVersusSettings.MenuItems[this.mSelectedPosition - 8];
									if (dropDownBox2.Enabled)
									{
										dropDownBox2.IsDown = true;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002D1A RID: 11546 RVA: 0x001690FC File Offset: 0x001672FC
		private void ChangeState(Controller iSender, SubMenuCharacterSelect.State iNewState)
		{
			this.mCurrentState = iNewState;
			this.mCurrentController = iSender;
			if (iNewState == SubMenuCharacterSelect.State.Normal)
			{
				this.mCurrentController = null;
			}
			if (!this.HasSelectedLevel)
			{
				this.mChapterName.SetText("");
				this.mChapterDescription.SetText("");
				this.mCurrentController = null;
				this.mStartButton.Enabled = false;
				this.mSelectLevelButton.Enabled = false;
			}
		}

		// Token: 0x17000AA4 RID: 2724
		// (get) Token: 0x06002D1B RID: 11547 RVA: 0x00169168 File Offset: 0x00167368
		public bool AllowCustom
		{
			get
			{
				return NetworkManager.Instance.State == NetworkState.Offline || !NetworkManager.Instance.Interface.IsVACSecure;
			}
		}

		// Token: 0x06002D1C RID: 11548 RVA: 0x0016918C File Offset: 0x0016738C
		public override void ControllerB(Controller iSender)
		{
			if (this.mGameModeBox.IsDown)
			{
				this.mGameModeBox.IsDown = false;
				return;
			}
			for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
			{
				if (this.mVersusSettings.MenuItems[i].IsDown)
				{
					this.mVersusSettings.MenuItems[i].IsDown = false;
					return;
				}
			}
			bool flag = this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController == iSender;
			if (flag && this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
			{
				this.mSelectedPosition = 5;
				this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
				return;
			}
			if (this.mCurrentController == iSender && this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
			{
				if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
				{
					this.HasSelectedLevel = false;
					this.SetNoLevelSelected();
					this.mSelectLevelButton.Enabled = (this.mSelectLevelButton.Selected = true);
					this.mSelectedPosition = 6;
				}
				this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
				return;
			}
			if (flag && this.mCurrentState != SubMenuCharacterSelect.State.Normal)
			{
				this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
				return;
			}
			if (iSender.Player == null)
			{
				bool flag2 = false;
				for (int j = 0; j < this.mPlayerSlots.Length; j++)
				{
					if (this.mPlayerSlots[j].ControllerType != -1)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					if (NetworkManager.Instance.HasHostSettings && (NetworkManager.Instance.GameType == GameType.Campaign || NetworkManager.Instance.GameType == GameType.Mythos))
					{
						Tome.Instance.PopPreviousMenu();
					}
					NetworkManager.Instance.EndSession();
					Tome.Instance.PopMenu();
					return;
				}
			}
			else if (iSender.Player.Playing)
			{
				byte b = (byte)iSender.Player.ID;
				if (this.mPlayerSlots[(int)b].State == SubMenuCharacterSelect.GamerState.Open)
				{
					this.GamerSelected(iSender, null);
					this.mStartButton.Enabled = this.CanStart();
					return;
				}
				if (this.mPlayerSlots[(int)b].State == SubMenuCharacterSelect.GamerState.Ready)
				{
					this.SetReady(false, b);
					return;
				}
				if (this.mPlayerSlots[(int)b].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
				{
					ToolTipMan.Instance.Kill(iSender.Player, false);
					this.mPlayerSlots[(int)b].State = SubMenuCharacterSelect.GamerState.Open;
					this.mStartButton.Enabled = this.CanStart();
					return;
				}
				if (this.mPlayerSlots[(int)b].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
				{
					int num = Profile.Instance.Avatars.IndexOfKey(iSender.Player.Gamer.Avatar.Name);
					int scrollValue = 0;
					for (int k = 0; k < SubMenuCharacterSelect.mRobeRepresentations.Count; k++)
					{
						if (SubMenuCharacterSelect.mRobeRepresentations[k].OriginalIndex == num)
						{
							scrollValue = k;
							break;
						}
					}
					this.mPlayerSlots[(int)b].SelectedItem = (this.mPlayerSlots[(int)b].ScrollValue = scrollValue);
					if (this.mDefaultAvatars)
					{
						this.mPlayerSlots[(int)b].State = SubMenuCharacterSelect.GamerState.Open;
						return;
					}
					this.mPlayerSlots[(int)b].State = SubMenuCharacterSelect.GamerState.CustomizingAvatar;
					return;
				}
			}
			else
			{
				if (NetworkManager.Instance.HasHostSettings && (NetworkManager.Instance.GameType == GameType.Campaign || NetworkManager.Instance.GameType == GameType.Mythos))
				{
					Tome.Instance.PopPreviousMenu();
				}
				Tome.Instance.PopMenu();
				NetworkManager.Instance.EndSession();
			}
		}

		// Token: 0x06002D1D RID: 11549 RVA: 0x001694F4 File Offset: 0x001676F4
		public override void ControllerX(Controller iSender)
		{
			base.ControllerX(iSender);
			if (this.mVersusSettings.TeamsEnabled && this.mGameSettings.GameType == GameType.Versus)
			{
				if (iSender.Player != null && iSender.Player.Playing)
				{
					if ((iSender.Player.Team & Factions.TEAM_RED) != Factions.NONE)
					{
						iSender.Player.Team = Factions.TEAM_BLUE;
					}
					else
					{
						iSender.Player.Team = Factions.TEAM_RED;
					}
					if (NetworkManager.Instance.State != NetworkState.Offline)
					{
						MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
						menuSelectMessage.IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect;
						menuSelectMessage.Option = 2;
						menuSelectMessage.Param0I = iSender.Player.ID;
						menuSelectMessage.Param1I = (int)iSender.Player.Team;
						NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
						return;
					}
				}
			}
			else if (iSender.Player != null && iSender.Player.Playing)
			{
				iSender.Player.Team = Factions.NONE;
			}
		}

		// Token: 0x06002D1E RID: 11550 RVA: 0x001695F4 File Offset: 0x001677F4
		public override void ControllerY(Controller iSender)
		{
			base.ControllerY(iSender);
			if (iSender.Player != null && iSender.Player.Playing)
			{
				int id = iSender.Player.ID;
				if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.Open)
				{
					int num = Profile.Instance.Avatars.IndexOfKey(iSender.Player.Gamer.Avatar.Name);
					for (int i = 0; i < SubMenuCharacterSelect.mRobeRepresentations.Count; i++)
					{
						if (num == SubMenuCharacterSelect.mRobeRepresentations[i].OriginalIndex)
						{
							num = i;
							break;
						}
					}
					this.mPlayerSlots[id].ConsecutiveColorChanges = Math.Max(0, this.mPlayerSlots[id].ConsecutiveColorChanges);
					this.mPlayerSlots[id].SelectedItem = (this.mPlayerSlots[id].ScrollValue = num);
					if (this.mDefaultAvatars)
					{
						this.mPlayerSlots[id].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
						return;
					}
					this.mPlayerSlots[id].State = SubMenuCharacterSelect.GamerState.CustomizingAvatar;
				}
			}
		}

		// Token: 0x06002D1F RID: 11551 RVA: 0x00169714 File Offset: 0x00167914
		public override void ControllerDown(Controller iSender)
		{
			if (iSender.Player != null && iSender.Player.Playing)
			{
				int id = iSender.Player.ID;
				if (iSender.Player.Gamer == Gamer.INVALID_GAMER)
				{
					int num = this.mPlayerSlots[id].SelectedItem;
					do
					{
						num++;
						if (num >= this.mGamerItems.Count)
						{
							num -= this.mGamerItems.Count;
						}
						if (num >= this.mGamerItems.Count - 1)
						{
							break;
						}
					}
					while (Profile.Instance.Gamers.Values[num].InUse);
					IL_B1:
					while (this.mGamerScrollBars[id].Value < this.mGamerScrollBars[id].MaxValue)
					{
						if (this.mGamerScrollBars[id].Value >= num - 2)
						{
							break;
						}
						this.mGamerScrollBars[id].Value++;
					}
					while (this.mGamerScrollBars[id].Value > 0 && this.mGamerScrollBars[id].Value >= num)
					{
						this.mGamerScrollBars[id].Value--;
					}
					this.mPlayerSlots[id].SelectedItem = num;
					return;
					goto IL_B1;
				}
				if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
				{
					return;
				}
				if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
				{
					int num2 = this.mPlayerSlots[id].SelectedItem + Defines.PLAYERCOLORS.Length / 2 + Defines.PLAYERCOLORS.Length % 2;
					if (num2 >= Defines.PLAYERCOLORS.Length)
					{
						num2 -= Defines.PLAYERCOLORS.Length;
					}
					this.mPlayerSlots[id].SelectedItem = num2;
					return;
				}
				if (this.mGameModeBox.IsDown)
				{
					int num3 = this.mGameModeBox.NewSelection + 1;
					if (num3 >= this.mGameModeBox.Count)
					{
						num3 -= this.mGameModeBox.Count;
					}
					this.mGameModeBox.NewSelection = num3;
					return;
				}
				for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
				{
					DropDownBox dropDownBox = this.mVersusSettings.MenuItems[i];
					if (dropDownBox.IsDown)
					{
						int num4 = dropDownBox.NewSelection + 1;
						if (num4 >= dropDownBox.Count)
						{
							num4 -= dropDownBox.Count;
						}
						dropDownBox.NewSelection = num4;
						return;
					}
				}
				if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
				{
					return;
				}
				if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
				{
					int num5 = this.mSelectedPack + 1;
					if (num5 > SubMenuCharacterSelect.mLevelRepresentations.Count - 1)
					{
						num5 = 0;
					}
					if (this.mSpecialScrollBar.Value > num5)
					{
						this.mSpecialScrollBar.Value = num5;
					}
					if (this.mSpecialScrollBar.Value + 6 <= num5)
					{
						this.mSpecialScrollBar.Value = num5 - 6 + 1;
					}
					this.mSelectedPack = num5;
					return;
				}
				if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
				{
					if (this.mSelectedPack < 0)
					{
						this.mSelectedPosition = 5;
						this.mSelectedPack = 0;
						this.mSpecialScrollBar.Value = 0;
						this.ShowPackToolTip(this.mSelectedPack);
						return;
					}
					this.mSelectedPosition = 5;
					int num6 = PackMan.Instance.ItemPacks.Length;
					int num7 = PackMan.Instance.MagickPacks.Length;
					int num8 = this.mSelectedPack + 5;
					if (num8 >= num6 + num7)
					{
						if ((num8 - num6) / 5 * 5 < num7)
						{
							this.mSpecialScrollBar.Value = this.mSpecialScrollBar.MaxValue;
							num8 = num6 + num7 - 1;
						}
						else
						{
							num8 = -1;
						}
					}
					else
					{
						if (this.mSelectedPack < num6 && num8 >= num6)
						{
							if (num8 / 5 * 5 < num6)
							{
								num8 = num6 - 1;
							}
							else
							{
								num8 = num6 + num8 % 5;
							}
						}
						if (num8 < num6)
						{
							while (this.mSpecialScrollBar.Value < this.mSpecialScrollBar.MaxValue)
							{
								if (this.mSpecialScrollBar.Value + 7 > num8 / 5 + 1)
								{
									break;
								}
								this.mSpecialScrollBar.Value++;
							}
						}
						else
						{
							int num9 = num6 / 5;
							if (num6 % 5 != 0)
							{
								num9++;
							}
							while (this.mSpecialScrollBar.Value < this.mSpecialScrollBar.MaxValue && this.mSpecialScrollBar.Value + 7 <= (num8 - num6) / 5 + 2 + num9)
							{
								this.mSpecialScrollBar.Value++;
							}
						}
					}
					this.mSelectedPack = num8;
					if (num8 >= 0)
					{
						this.ShowPackToolTip(num8);
						return;
					}
					ToolTipMan.Instance.Kill(null, false);
					return;
				}
				else
				{
					if (this.mSelectedPosition == 7 + this.mVersusSettings.MenuItems.Count && this.mGameSettings.GameType == GameType.Versus)
					{
						this.mSelectedPosition = 5;
					}
					else
					{
						if (iSender.Player.Gamer != Gamer.INVALID_GAMER && iSender is XInputController)
						{
							if (this.mSelectedPosition == -1)
							{
								this.mSelectedPosition = 4;
								return;
							}
							if (this.mSelectedPosition == 3)
							{
								this.mSelectedPosition = -1;
								return;
							}
						}
						this.mSelectLevelButton.Selected = false;
						int mSelectedPosition = this.mSelectedPosition;
						if (mSelectedPosition != -1)
						{
							switch (mSelectedPosition)
							{
							case 4:
								this.mSelectedPosition = 0;
								goto IL_5AF;
							case 5:
								break;
							case 6:
								if (this.mGameSettings.GameType == GameType.Versus)
								{
									this.mSelectedPosition = 7;
									goto IL_5AF;
								}
								if (this.mGameSettings.GameType == GameType.Challenge)
								{
									this.mSelectedPosition = 5;
									goto IL_5AF;
								}
								goto IL_5AF;
							default:
								if (mSelectedPosition == 9)
								{
									if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
									{
										this.mSelectedPosition++;
										goto IL_5AF;
									}
									goto IL_5AF;
								}
								break;
							}
							this.mSelectedPosition++;
							if (this.mSelectedPosition == 6 && !this.HasSelectedLevel)
							{
								this.mSelectLevelButton.Selected = true;
							}
						}
						else
						{
							this.mSelectedPosition = 0;
						}
					}
					IL_5AF:
					if (this.mSelectedPosition == 5)
					{
						this.mSelectedPack = this.mPackScrollValue;
					}
				}
			}
		}

		// Token: 0x06002D20 RID: 11552 RVA: 0x00169CE8 File Offset: 0x00167EE8
		public override void ControllerUp(Controller iSender)
		{
			if (iSender.Player != null && iSender.Player.Playing)
			{
				int id = iSender.Player.ID;
				if (iSender.Player.Gamer == Gamer.INVALID_GAMER)
				{
					int num = this.mPlayerSlots[id].SelectedItem;
					do
					{
						num--;
						if (num < 0)
						{
							num += this.mGamerItems.Count;
						}
						if (num >= this.mGamerItems.Count - 1)
						{
							break;
						}
					}
					while (Profile.Instance.Gamers.Values[num].InUse);
					IL_A7:
					while (this.mGamerScrollBars[id].Value < this.mGamerScrollBars[id].MaxValue)
					{
						if (this.mGamerScrollBars[id].Value >= num - 2)
						{
							break;
						}
						this.mGamerScrollBars[id].Value++;
					}
					while (this.mGamerScrollBars[id].Value > 0 && this.mGamerScrollBars[id].Value >= num)
					{
						this.mGamerScrollBars[id].Value--;
					}
					this.mPlayerSlots[id].SelectedItem = num;
					return;
					goto IL_A7;
				}
				if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
				{
					return;
				}
				if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
				{
					int num2 = this.mPlayerSlots[id].SelectedItem - Defines.PLAYERCOLORS.Length / 2 - Defines.PLAYERCOLORS.Length % 2;
					if (num2 < 0)
					{
						num2 += Defines.PLAYERCOLORS.Length;
					}
					this.mPlayerSlots[id].SelectedItem = num2;
					return;
				}
				if (this.mGameModeBox.IsDown)
				{
					int num3 = this.mGameModeBox.NewSelection - 1;
					if (num3 < 0)
					{
						num3 += this.mGameModeBox.Count;
					}
					this.mGameModeBox.NewSelection = num3;
					return;
				}
				for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
				{
					DropDownBox dropDownBox = this.mVersusSettings.MenuItems[i];
					if (dropDownBox.IsDown)
					{
						int num4 = dropDownBox.NewSelection - 1;
						if (num4 < 0)
						{
							num4 += dropDownBox.Count;
						}
						dropDownBox.NewSelection = num4;
						return;
					}
				}
				if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
				{
					return;
				}
				if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
				{
					int num5;
					if (this.mSelectedPack == 0)
					{
						num5 = SubMenuCharacterSelect.mLevelRepresentations.Count - 1;
					}
					else
					{
						num5 = this.mSelectedPack - 1;
					}
					if (num5 < 0)
					{
						num5 += SubMenuCharacterSelect.mLevelRepresentations.Count - 1;
					}
					if (this.mSpecialScrollBar.Value > num5)
					{
						this.mSpecialScrollBar.Value = num5;
					}
					if (this.mSpecialScrollBar.Value + 6 <= num5)
					{
						this.mSpecialScrollBar.Value = num5 - 6 + 1;
					}
					this.mSelectedPack = num5;
					return;
				}
				if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
				{
					if (this.mSelectedPack < 0)
					{
						this.mSelectedPosition = 5;
						int num6 = PackMan.Instance.ItemPacks.Length;
						int num7 = PackMan.Instance.MagickPacks.Length;
						int num8 = num7 / 5;
						if (num7 % 5 == 0)
						{
							num8--;
						}
						this.mSelectedPack = num6 + num8 * 5;
						this.mSpecialScrollBar.Value = this.mSpecialScrollBar.MaxValue;
						this.ShowPackToolTip(this.mSelectedPack);
						return;
					}
					this.mSelectedPosition = 5;
					int num9 = PackMan.Instance.ItemPacks.Length;
					int num10 = this.mSelectedPack - 5;
					if (num10 < 0)
					{
						num10 = -1;
					}
					else
					{
						if (this.mSelectedPack >= num9 && num10 < num9)
						{
							int num11 = num9 / 5;
							if (num9 % 5 == 0)
							{
								num11--;
							}
							num10 = num11 * 5 + (this.mSelectedPack - num9) % 5;
							if (num10 >= num9)
							{
								num10 = num9 - 1;
							}
						}
						if (num10 < num9)
						{
							while (this.mSpecialScrollBar.Value > 0)
							{
								if (this.mSpecialScrollBar.Value < num10 / 5 + 2)
								{
									break;
								}
								this.mSpecialScrollBar.Value--;
							}
						}
						else
						{
							int num12 = num9 / 5;
							if (num9 % 5 != 0)
							{
								num12++;
							}
							while (this.mSpecialScrollBar.Value > 0 && this.mSpecialScrollBar.Value >= (num10 - num9) / 5 + 3 + num12)
							{
								this.mSpecialScrollBar.Value--;
							}
						}
					}
					this.mSelectedPack = num10;
					if (num10 >= 0)
					{
						this.ShowPackToolTip(num10);
						return;
					}
					ToolTipMan.Instance.Kill(null, false);
					return;
				}
				else
				{
					if (iSender.Player.Gamer != Gamer.INVALID_GAMER && iSender is XInputController)
					{
						if (this.mSelectedPosition == -1)
						{
							this.mSelectedPosition = 3;
							return;
						}
						if (this.mSelectedPosition == 4)
						{
							this.mSelectedPosition = -1;
							return;
						}
					}
					this.mSelectLevelButton.Selected = false;
					int mSelectedPosition = this.mSelectedPosition;
					if (mSelectedPosition != 0)
					{
						switch (mSelectedPosition)
						{
						case 4:
							this.mSelectedPosition = 0;
							goto IL_594;
						case 5:
							if (this.mGameSettings.GameType == GameType.Versus)
							{
								this.mSelectedPosition = 7 + this.mVersusSettings.MenuItems.Count;
								goto IL_594;
							}
							if (this.mGameSettings.GameType == GameType.Challenge)
							{
								this.mSelectedPosition = 6;
								goto IL_594;
							}
							goto IL_594;
						case 7:
							this.mSelectedPosition = 6;
							goto IL_594;
						case 9:
							if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
							{
								this.mSelectedPosition--;
								goto IL_594;
							}
							goto IL_594;
						}
						this.mSelectedPosition--;
						if (this.mSelectedPosition == 6 && !this.HasSelectedLevel)
						{
							this.mSelectLevelButton.Selected = true;
						}
					}
					else
					{
						this.mSelectedPosition = 4;
					}
					IL_594:
					if (this.mSelectedPosition == 5)
					{
						this.mSelectedPack = this.mPackScrollValue;
					}
				}
			}
		}

		// Token: 0x06002D21 RID: 11553 RVA: 0x0016A2A0 File Offset: 0x001684A0
		public override void ControllerRight(Controller iSender)
		{
			if (iSender.Player != null && iSender.Player.Playing)
			{
				if (iSender.Player.Gamer != Gamer.INVALID_GAMER)
				{
					int id = iSender.Player.ID;
					if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
					{
						int count = SubMenuCharacterSelect.mRobeRepresentations.Count;
						int num = this.mPlayerSlots[id].SelectedItem + 1;
						if (num >= count)
						{
							num -= count;
						}
						if ((this.mPlayerSlots[id].ScrollValue + 3) % count == this.mPlayerSlots[id].SelectedItem)
						{
							this.mPlayerSlots[id].ScrollValue = (num - 3 + count) % count;
						}
						this.mPlayerSlots[id].SelectedItem = num;
						this.ShowAvatarToolTip(iSender, num, null);
						return;
					}
					if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
					{
						int num2 = this.mPlayerSlots[id].SelectedItem + 1;
						if (num2 >= Defines.PLAYERCOLORS.Length)
						{
							num2 -= Defines.PLAYERCOLORS.Length;
						}
						this.mPlayerSlots[id].SelectedItem = num2;
						return;
					}
				}
				if (this.mGameModeBox.IsDown)
				{
					return;
				}
				for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
				{
					if (this.mVersusSettings.MenuItems[i].IsDown)
					{
						return;
					}
				}
				if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
				{
					return;
				}
				if (this.mCurrentState == SubMenuCharacterSelect.State.Normal && this.mSelectedPosition == 9)
				{
					this.mSelectedPosition = 0;
					return;
				}
				if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
				{
					if (this.mSelectedPack >= 0 && this.mSelectedPack < PackMan.Instance.AllPacks.Length)
					{
						this.mSelectedPosition = 5;
						int num3 = PackMan.Instance.ItemPacks.Length;
						int num4;
						if (this.mSelectedPack < num3)
						{
							num4 = this.mSelectedPack / 5 * 5 + (this.mSelectedPack + 1) % 5;
							if (num4 >= num3)
							{
								num4 = this.mSelectedPack / 5 * 5;
							}
						}
						else
						{
							int num5 = PackMan.Instance.MagickPacks.Length;
							num4 = num3 + (this.mSelectedPack - num3) / 5 * 5 + (this.mSelectedPack - num3 + 1) % 5;
							if (num4 >= num3 + num5)
							{
								num4 = num3 + num5 / 5 * 5;
							}
						}
						this.mSelectedPack = num4;
						this.ShowPackToolTip(num4);
					}
					return;
				}
				if (this.mSelectedPosition == 5)
				{
					this.ScrollPack(1);
					return;
				}
				if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
				{
					switch (this.mSelectedPosition)
					{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
						if (this.mGameSettings.GameType == GameType.Versus)
						{
							this.mSelectedPosition = 7;
							return;
						}
						if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
						{
							this.mSelectedPosition = 5;
							return;
						}
						break;
					default:
						this.mSelectedPosition = 0;
						break;
					}
				}
			}
		}

		// Token: 0x06002D22 RID: 11554 RVA: 0x0016A5B8 File Offset: 0x001687B8
		private void ShowPackToolTip(int iIndex)
		{
			IPack pack = PackMan.Instance.AllPacks[iIndex];
			int num = PackMan.Instance.ItemPacks.Length;
			int num2;
			int num3;
			if (iIndex < num)
			{
				num2 = 1 + iIndex / 5;
				num3 = iIndex % 5;
			}
			else
			{
				num2 = 2 + num / 5 + (iIndex - num) / 5;
				if (num % 5 != 0)
				{
					num2++;
				}
				num3 = (iIndex - num) % 5;
			}
			Vector2 vector = default(Vector2);
			vector.X = 160f + 64f * (float)num3;
			vector.Y = 64f + this.mSpecialScrollBar.Position.Y - this.mSpecialScrollBar.Height * 0.5f + (float)Math.Max(num2 - this.mSpecialScrollBar.Value, 0) * 64f;
			Vector2 vector2;
			Tome.Instance.PageToScreen(true, ref vector, out vector2);
			ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetStringWithReferencs(pack.Name) + "\n" + LanguageManager.Instance.GetStringWithReferencs(pack.Descritpion), ref vector2);
		}

		// Token: 0x06002D23 RID: 11555 RVA: 0x0016A6B8 File Offset: 0x001688B8
		public override void ControllerLeft(Controller iSender)
		{
			if (iSender.Player != null && iSender.Player.Playing)
			{
				if (iSender.Player.Gamer != Gamer.INVALID_GAMER)
				{
					int id = iSender.Player.ID;
					if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
					{
						int num = this.mPlayerSlots[id].SelectedItem - 1;
						if (num < 0)
						{
							num += SubMenuCharacterSelect.mRobeRepresentations.Count;
						}
						if (this.mPlayerSlots[id].ScrollValue == this.mPlayerSlots[id].SelectedItem)
						{
							this.mPlayerSlots[id].ScrollValue = num;
						}
						this.mPlayerSlots[id].SelectedItem = num;
						this.ShowAvatarToolTip(iSender, num, null);
						return;
					}
					if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
					{
						int num2 = this.mPlayerSlots[id].SelectedItem - 1;
						if (num2 < 0)
						{
							num2 += Defines.PLAYERCOLORS.Length;
						}
						this.mPlayerSlots[id].SelectedItem = num2;
						return;
					}
				}
				if (this.mGameModeBox.IsDown)
				{
					return;
				}
				for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
				{
					if (this.mVersusSettings.MenuItems[i].IsDown)
					{
						return;
					}
				}
				if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
				{
					return;
				}
				if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
				{
					if (this.mSelectedPack >= 0 && this.mSelectedPack < PackMan.Instance.AllPacks.Length)
					{
						this.mSelectedPosition = 5;
						int num3 = PackMan.Instance.ItemPacks.Length;
						int num4;
						if (this.mSelectedPack < num3)
						{
							num4 = this.mSelectedPack / 5 * 5 + (this.mSelectedPack - 1 + 5) % 5;
							if (num4 >= num3)
							{
								num4 = num3 - 1;
							}
						}
						else
						{
							int num5 = PackMan.Instance.MagickPacks.Length;
							num4 = num3 + (this.mSelectedPack - num3) / 5 * 5 + (this.mSelectedPack - num3 - 1 + 5) % 5;
							if (num4 >= num3 + num5)
							{
								num4 = num3 + num5 - 1;
							}
						}
						this.mSelectedPack = num4;
						this.ShowPackToolTip(num4);
					}
					return;
				}
				if (this.mSelectedPosition == 5)
				{
					this.ScrollPack(-1);
					return;
				}
				switch (this.mSelectedPosition)
				{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
					if (this.mGameSettings.GameType == GameType.Versus)
					{
						this.mSelectedPosition = 7;
						return;
					}
					if (this.mGameSettings.GameType == GameType.Campaign || this.mGameSettings.GameType == GameType.Mythos)
					{
						this.mSelectedPosition = 9;
						return;
					}
					this.mSelectedPosition = 5;
					return;
				default:
					if (this.mGameSettings.GameType == GameType.Campaign || this.mGameSettings.GameType == GameType.Mythos)
					{
						this.mSelectedPosition = 9;
					}
					break;
				}
			}
		}

		// Token: 0x06002D24 RID: 11556 RVA: 0x0016A9A4 File Offset: 0x00168BA4
		private void ScrollPack(int iDir)
		{
			int num = this.mPackScrollValue;
			IPack[] allPacks = PackMan.Instance.AllPacks;
			do
			{
				num += iDir;
				if (num < 0)
				{
					num += allPacks.Length;
				}
				if (num >= allPacks.Length)
				{
					num -= allPacks.Length;
				}
			}
			while (!allPacks[num].Enabled && num != this.mPackScrollValue);
			this.mPackScrollValue = num;
		}

		// Token: 0x06002D25 RID: 11557 RVA: 0x0016A9F8 File Offset: 0x00168BF8
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if ((this.mOptionsAlpha > 0f && this.mOptionsAlpha < 1f) || RenderManager.Instance.IsTransitionActive)
			{
				return;
			}
			Vector2 iPoint;
			bool flag;
			if (iState.ScrollWheelValue > iOldState.ScrollWheelValue)
			{
				if (iSender.Player != null && iSender.Player.Playing && iSender.Player.Gamer == Gamer.INVALID_GAMER)
				{
					this.mGamerScrollBars[iSender.Player.ID].Value--;
					this.ControllerMouseMove(iSender, iScreenSize, iState, iOldState);
					return;
				}
				if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel || this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
				{
					this.mSpecialScrollBar.Value--;
					return;
				}
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					NetworkChat.Instance.ScrollBar.Value--;
					return;
				}
			}
			else if (iState.ScrollWheelValue < iOldState.ScrollWheelValue)
			{
				if (iSender.Player != null && iSender.Player.Playing && iSender.Player.Gamer == Gamer.INVALID_GAMER)
				{
					this.mGamerScrollBars[iSender.Player.ID].Value++;
					this.ControllerMouseMove(iSender, iScreenSize, iState, iOldState);
					return;
				}
				if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel || this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
				{
					this.mSpecialScrollBar.Value++;
					return;
				}
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					NetworkChat.Instance.ScrollBar.Value++;
					return;
				}
			}
			else if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out iPoint, out flag))
			{
				if (!flag)
				{
					this.mGamerDropDownMenu.Hide();
					this.mGameModeBox.IsDown = false;
					for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
					{
						this.mVersusSettings.MenuItems[i].IsDown = false;
					}
					return;
				}
				if (iState.RightButton == ButtonState.Released && iOldState.RightButton == ButtonState.Pressed)
				{
					int num;
					this.HitGamer(ref iPoint, out num);
					if (iSender.Player != null && iSender.Player.Playing && iSender.Player.ID == num)
					{
						this.UpdateGamerDropDownMenu();
						this.mGamerDropDownMenu.Show(Math.Min((int)iPoint.X, 1024 - (int)this.mGamerDropDownMenu.Size.X - 16), Math.Min((int)iPoint.Y, 1024 - (int)this.mGamerDropDownMenu.Size.Y * this.mGamerDropDownMenu.Count - 16));
						return;
					}
					this.mGamerDropDownMenu.Hide();
					Player[] players = Game.Instance.Players;
					if (NetworkManager.Instance.State == NetworkState.Server && num >= 0 && players[num].Playing && players[num].Gamer is NetworkGamer)
					{
						this.mAdminDropDownMenu.Tag = num;
						this.mAdminDropDownMenu.Show(Math.Min((int)iPoint.X, 1024 - (int)this.mAdminDropDownMenu.Size.X - 16), Math.Min((int)iPoint.Y, 1024 - (int)this.mAdminDropDownMenu.Size.Y * this.mAdminDropDownMenu.Count - 16));
						return;
					}
				}
				else if (iState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
				{
					if (this.mSpecialScrollBar.Grabbed)
					{
						this.mSpecialScrollBar.Grabbed = false;
						return;
					}
					if (NetworkManager.Instance.State != NetworkState.Offline && NetworkChat.Instance.ScrollBar.Grabbed)
					{
						NetworkChat.Instance.ScrollBar.Grabbed = false;
						return;
					}
					if (this.mSettingsScrollbar.Grabbed)
					{
						this.mSettingsScrollbar.Grabbed = false;
						return;
					}
					if (this.mGamerDropDownMenu.IsVisible)
					{
						if (this.mGamerDropDownMenu.GetHitIndex(ref iPoint) == 0 && this.mVersusSettings.TeamsEnabled && iSender.Player != null && iSender.Player.Playing)
						{
							if ((iSender.Player.Team & Factions.TEAM_RED) != Factions.NONE)
							{
								iSender.Player.Team = Factions.TEAM_BLUE;
							}
							else
							{
								iSender.Player.Team = Factions.TEAM_RED;
							}
							if (NetworkManager.Instance.State != NetworkState.Offline)
							{
								MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
								menuSelectMessage.Option = 2;
								menuSelectMessage.Param0I = iSender.Player.ID;
								menuSelectMessage.Param1I = (int)iSender.Player.Team;
								NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
							}
						}
						this.mGamerDropDownMenu.Hide();
						return;
					}
					if (this.mAdminDropDownMenu.IsVisible)
					{
						if (this.mAdminDropDownMenu.GetHitIndex(ref iPoint) == 0)
						{
							NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
							NetworkGamer networkGamer = Game.Instance.Players[(int)this.mAdminDropDownMenu.Tag].Gamer as NetworkGamer;
							if (networkServer != null)
							{
								networkServer.CloseConnection(networkGamer.ClientID, ConnectionClosedMessage.CReason.Kicked);
							}
						}
						this.mAdminDropDownMenu.Hide();
						return;
					}
					if (this.mGameModeBox.IsDown)
					{
						this.mGameModeBox.SelectedIndex = this.mGameModeBox.GetHitIndex(ref iPoint);
						this.mGameModeBox.IsDown = false;
						return;
					}
					for (int j = 0; j < this.mVersusSettings.MenuItems.Count; j++)
					{
						if (this.mVersusSettings.MenuItems[j].IsDown)
						{
							this.mVersusSettings.MenuItems[j].SelectedIndex = this.mVersusSettings.MenuItems[j].GetHitIndex(ref iPoint);
							this.mVersusSettings.MenuItems[j].IsDown = false;
							return;
						}
					}
					float num2 = (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel) ? 904f : 629.3333f;
					bool flag2 = !this.HasSelectedLevel && !this.mSelectLevelButton.InsideBounds(iState) && NetworkManager.Instance.State != NetworkState.Client;
					bool flag3 = this.mCurrentState != SubMenuCharacterSelect.State.Normal && (SubMenuCharacterSelect.mLevelRepresentations == null || iPoint.X <= 64f || iPoint.X >= 512f || iPoint.Y <= 64f || iPoint.Y >= num2 || NetworkManager.Instance.State == NetworkState.Client);
					if (flag2 || flag3)
					{
						if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel && this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
						{
							this.SetNoLevelSelected();
						}
						this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
					}
					if (this.mCurrentState == SubMenuCharacterSelect.State.Normal)
					{
						if (this.mGameSettings.GameType == GameType.Versus)
						{
							if (this.mGameModeBox.InsideBounds(ref iPoint))
							{
								this.mGameModeBox.IsDown = true;
								return;
							}
							for (int k = this.mSettingsScrollbar.Value; k < Math.Min(this.mSettingsScrollbar.Value + 5, this.mVersusSettings.MenuItems.Count); k++)
							{
								if (this.mVersusSettings.MenuItems[k].InsideBounds(ref iPoint))
								{
									this.mVersusSettings.MenuItems[k].IsDown = true;
									return;
								}
							}
						}
						bool flag4 = this.mSelectLevelButton.InsideBounds(iState) || (iPoint.X >= 64f && iPoint.X <= 512f && iPoint.Y >= this.mSelectLevelButton.Position.Y - 64f && iPoint.Y <= this.mSelectLevelButton.Position.Y + 64f);
						if (this.mGameSettings.GameType == GameType.Mythos || this.mGameSettings.GameType == GameType.Campaign)
						{
							if (SubMenuCharacterSelect.mLevelRepresentations != null && flag4 && (NetworkManager.Instance.State == NetworkState.Server || NetworkManager.Instance.State == NetworkState.Offline))
							{
								this.mSpecialScrollBar.Height = 840f;
								this.mSpecialScrollBar.SetMaxValue(SubMenuCharacterSelect.mLevelRepresentations.Count - 6);
								this.mSpecialScrollBar.Value = 0;
								ToolTipMan.Instance.Kill(null, false);
								this.HasSelectedLevel = true;
								this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingLevel);
							}
						}
						else
						{
							if (SubMenuCharacterSelect.mLevelRepresentations != null && flag4 && (NetworkManager.Instance.State == NetworkState.Server || NetworkManager.Instance.State == NetworkState.Offline))
							{
								this.mSpecialScrollBar.Height = 840f;
								this.mSpecialScrollBar.SetMaxValue(SubMenuCharacterSelect.mLevelRepresentations.Count - 6);
								this.mSpecialScrollBar.Value = 0;
								ToolTipMan.Instance.Kill(null, false);
								this.HasSelectedLevel = true;
								this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingLevel);
							}
							int num3;
							ControllerDirection controllerDirection;
							if ((this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus) && this.HitPackOverview(ref iPoint, out num3, out controllerDirection) && !(this.mVersusSettings is Krietor.Settings))
							{
								if (controllerDirection == ControllerDirection.Left)
								{
									this.ScrollPack(-1);
								}
								else if (controllerDirection == ControllerDirection.Right)
								{
									this.ScrollPack(1);
								}
								else if (NetworkManager.Instance.State != NetworkState.Client)
								{
									int num4 = PackMan.Instance.ItemPacks.Length;
									int num5 = PackMan.Instance.MagickPacks.Length;
									int num6 = 2 + num4 / 5 + num5 / 5;
									if (num4 % 5 != 0)
									{
										num6++;
									}
									if (num5 % 5 != 0)
									{
										num6++;
									}
									this.mSpecialScrollBar.Height = 448f;
									this.mSpecialScrollBar.SetMaxValue(num6 - 7);
									ToolTipMan.Instance.Kill(null, false);
									this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingPacks);
								}
							}
						}
					}
					else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
					{
						if (iPoint.X >= 64f && iPoint.X <= 448f)
						{
							float num7 = 64f;
							float num8 = num7 + 140f;
							int num9 = Math.Min(this.mSpecialScrollBar.Value + 5, SubMenuCharacterSelect.mLevelRepresentations.Count - 1);
							for (int l = this.mSpecialScrollBar.Value; l <= num9; l++)
							{
								if (iPoint.Y >= num7 && iPoint.Y <= num8)
								{
									int levelOriginalIndex = this.GetLevelOriginalIndex(l);
									ToolTipMan.Instance.Kill(null, false);
									this.OnLevelChange(iSender, this.mGameSettings.GameType, levelOriginalIndex);
									this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
									break;
								}
								num7 += 140f;
								num8 += 140f;
							}
						}
					}
					else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
					{
						if (this.mPackCloseButton.InsideBounds(ref iPoint))
						{
							DLC_StatusHelper.TrySetAllItemsAndMagicsUsed();
							this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
						}
						else if (this.HitPackList(ref iPoint, out this.mSelectedPack))
						{
							if (PackMan.Instance.AllPacks[this.mSelectedPack].License == HackHelper.License.No && PackMan.Instance.AllPacks[this.mSelectedPack].StoreURL != 0U)
							{
								SteamUtils.ActivateGameOverlayToStore(PackMan.Instance.AllPacks[this.mSelectedPack].StoreURL, OverlayStoreFlag.None);
							}
							else
							{
								IPack pack = PackMan.Instance.AllPacks[this.mSelectedPack];
								pack.Enabled = !pack.Enabled;
							}
						}
					}
					int num10;
					this.HitGamer(ref iPoint, out num10);
					if (iSender.Player == null || !iSender.Player.Playing)
					{
						if (num10 >= 0 && !Game.Instance.Players[num10].Playing)
						{
							this.mSelectedPosition = -1;
							this.JoinPlayer(iSender, num10, null);
							this.UpdateControllerIcon(iSender, num10);
						}
					}
					else if (iSender.Player.Gamer == Gamer.INVALID_GAMER)
					{
						MenuScrollBar menuScrollBar = this.mGamerScrollBars[iSender.Player.ID];
						if (menuScrollBar.Grabbed)
						{
							menuScrollBar.Grabbed = false;
						}
						else if (iPoint.X >= 672f && iPoint.X <= 960f)
						{
							float num11 = menuScrollBar.Position.Y - 62f;
							float num12 = menuScrollBar.Position.Y - 31f;
							int m = menuScrollBar.Value;
							while (m < Math.Min(menuScrollBar.Value + 4, this.mGamerItems.Count))
							{
								if (iPoint.Y >= num11 && iPoint.Y <= num12)
								{
									if (m >= Profile.Instance.Gamers.Count)
									{
										this.mNameInputController = iSender;
										string iDescr = LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_ENTER_NAME).ToUpper();
										this.mNameInputBox.Show(new Action<string>(this.NewGamerCreated), iSender, iDescr);
										break;
									}
									if (Profile.Instance.Gamers.Values[m].InUse)
									{
										break;
									}
									this.GamerSelected(iSender, Profile.Instance.Gamers.Values[m]);
									Player player = iSender.Player;
									if (player.Gamer != null)
									{
										this.VerifyAvatar(ref player, false);
										break;
									}
									break;
								}
								else
								{
									num11 += 31f;
									num12 += 31f;
									m++;
								}
							}
						}
					}
					else
					{
						byte b = (byte)iSender.Player.ID;
						if (num10 == (int)b)
						{
							int num19;
							if (NetworkManager.Instance.State == NetworkState.Client && (this.mPlayerSlots[num10].State == SubMenuCharacterSelect.GamerState.Open || this.mPlayerSlots[num10].State == SubMenuCharacterSelect.GamerState.Ready) && iPoint.X >= 912f && iPoint.X <= 1040f && iPoint.Y >= 89f + (float)b * 137f + 112f - 80f && iPoint.Y <= 89f + (float)b * 137f + 112f)
							{
								this.SetReady(this.mPlayerSlots[(int)b].State == SubMenuCharacterSelect.GamerState.Open, b);
							}
							else if (this.mPlayerSlots[num10].State == SubMenuCharacterSelect.GamerState.Open)
							{
								this.mPlayerSlots[(int)b].ConsecutiveColorChanges = Math.Max(0, this.mPlayerSlots[(int)b].ConsecutiveColorChanges);
								this.mPlayerSlots[(int)b].SelectedItem = -1;
								if (this.mDefaultAvatars)
								{
									this.mPlayerSlots[(int)b].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
								}
								else
								{
									this.mPlayerSlots[(int)b].State = SubMenuCharacterSelect.GamerState.CustomizingAvatar;
								}
							}
							else if (this.mPlayerSlots[(int)b].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
							{
								int index;
								if (this.HitAvatar((int)b, ref iPoint, out index))
								{
									Profile.PlayableAvatar playableAvatar = Profile.Instance.Avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[index].OriginalIndex];
									uint num13 = 0U;
									uint num14 = 0U;
									if (DLC_StatusHelper.ValidateRobeLocked(playableAvatar, out num13, out num14))
									{
										SteamUtils.ActivateGameOverlayToWebPage("http://store.steampowered.com/app/" + num14 + "/");
										return;
									}
									this.SetRobeUsed(playableAvatar.Name);
									this.SortRobeRepList();
									HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
									if (license == HackHelper.License.Yes || (license == HackHelper.License.Custom && (NetworkManager.Instance.State == NetworkState.Offline || !NetworkManager.Instance.Interface.IsVACSecure)))
									{
										if (!iSender.Player.Gamer.Avatar.Name.Equals(playableAvatar.Name))
										{
											this.mPlayerSlots[(int)b].ConsecutiveColorChanges = -1;
										}
										iSender.Player.Gamer.Avatar = playableAvatar;
										this.mPlayerSlots[(int)b].Custom = SubMenuCharacterSelect.mRobeRepresentations[index].IsCustom;
										if (NetworkManager.Instance.State != NetworkState.Offline)
										{
											GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(iSender.Player);
											NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
										}
										ToolTipMan.Instance.Kill(iSender.Player, false);
										this.mPlayerSlots[(int)b].SelectedItem = -1;
										this.mPlayerSlots[(int)b].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
									}
								}
								else if (iPoint.Y >= 89f + 137f * (float)b && iPoint.Y <= 89f + 137f * ((float)b + 1f))
								{
									float num15 = 528f;
									float num16 = num15 + 48f;
									if (iPoint.X >= num15 && iPoint.X <= num16)
									{
										int num17 = this.mPlayerSlots[(int)b].ScrollValue - 1;
										if (num17 < 0)
										{
											num17 += SubMenuCharacterSelect.mRobeRepresentations.Count;
										}
										this.mPlayerSlots[(int)b].ScrollValue = num17;
									}
									else
									{
										num15 += 432f;
										num16 += 432f;
										if (iPoint.X >= num15 && iPoint.X <= num16)
										{
											int num18 = this.mPlayerSlots[(int)b].ScrollValue + 1;
											if (num18 >= SubMenuCharacterSelect.mRobeRepresentations.Count)
											{
												num18 -= SubMenuCharacterSelect.mRobeRepresentations.Count;
											}
											this.mPlayerSlots[(int)b].ScrollValue = num18;
										}
									}
								}
							}
							else if (this.mPlayerSlots[(int)b].State == SubMenuCharacterSelect.GamerState.CustomizingColor && this.HitColor((int)b, ref iPoint, out num19))
							{
								if ((int)iSender.Player.Color != num19)
								{
									SubMenuCharacterSelect.PlayerState[] array = this.mPlayerSlots;
									byte b2 = b;
									array[(int)b2].ConsecutiveColorChanges = array[(int)b2].ConsecutiveColorChanges + 1;
									if (this.mPlayerSlots[(int)b].ConsecutiveColorChanges == 2)
									{
										AchievementsManager.Instance.AwardAchievement(null, "bluenoyelloooow");
									}
								}
								iSender.Player.Color = (byte)num19;
								this.mPlayerSlots[(int)b].SelectedItem = -1;
								this.mPlayerSlots[(int)b].State = SubMenuCharacterSelect.GamerState.Open;
							}
						}
					}
					if (this.mBackButton.InsideBounds(ref iPoint))
					{
						this.ControllerB(iSender);
						return;
					}
					if (this.mStartButton.InsideBounds(ref iPoint) && this.mStartButton.Enabled && this.CanStart())
					{
						NetworkState state = NetworkManager.Instance.State;
						if (state == NetworkState.Offline)
						{
							this.StartLevel();
							return;
						}
						if (state == NetworkState.Server)
						{
							this.Start();
							return;
						}
					}
				}
				else if (iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released)
				{
					MenuScrollBar scrollBar = NetworkChat.Instance.ScrollBar;
					if ((this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel || this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks) && this.mSpecialScrollBar.InsideBounds(ref iPoint))
					{
						if (this.mSpecialScrollBar.InsideDragBounds(iPoint))
						{
							this.mSpecialScrollBar.Grabbed = true;
							return;
						}
						if (this.mSpecialScrollBar.InsideDragDownBounds(iPoint))
						{
							this.mSpecialScrollBar.Value++;
							return;
						}
						if (this.mSpecialScrollBar.InsideUpBounds(iPoint))
						{
							this.mSpecialScrollBar.Value--;
							return;
						}
					}
					else if (NetworkManager.Instance.State != NetworkState.Offline && scrollBar.InsideBounds(ref iPoint))
					{
						if (scrollBar.InsideDragBounds(iPoint))
						{
							scrollBar.Grabbed = true;
							return;
						}
						if (scrollBar.InsideDownBounds(iPoint))
						{
							scrollBar.Value++;
							return;
						}
						if (scrollBar.InsideUpBounds(iPoint))
						{
							scrollBar.Value--;
							return;
						}
					}
					else if (this.mSettingsScrollbar.InsideBounds(ref iPoint))
					{
						if (this.mSettingsScrollbar.InsideDragBounds(iPoint))
						{
							this.mSettingsScrollbar.Grabbed = true;
							return;
						}
						if (this.mSettingsScrollbar.InsideDragDownBounds(iPoint))
						{
							this.mSettingsScrollbar.Value++;
							return;
						}
						if (this.mSettingsScrollbar.InsideDragUpBounds(iPoint))
						{
							this.mSettingsScrollbar.Value--;
							return;
						}
					}
					else if (iSender.Player != null && iSender.Player.Playing && iSender.Player.Gamer == Gamer.INVALID_GAMER)
					{
						MenuScrollBar menuScrollBar2 = this.mGamerScrollBars[iSender.Player.ID];
						if (menuScrollBar2.InsideDragBounds(iPoint))
						{
							menuScrollBar2.Grabbed = true;
							return;
						}
						if (menuScrollBar2.InsideDownBounds(iPoint))
						{
							menuScrollBar2.Value++;
							return;
						}
						if (menuScrollBar2.InsideUpBounds(iPoint))
						{
							menuScrollBar2.Value--;
							return;
						}
					}
				}
			}
			else
			{
				this.mGamerDropDownMenu.Hide();
				this.mGameModeBox.IsDown = false;
				for (int n = 0; n < this.mVersusSettings.MenuItems.Count; n++)
				{
					this.mVersusSettings.MenuItems[n].IsDown = false;
				}
			}
		}

		// Token: 0x06002D26 RID: 11558 RVA: 0x0016BEFC File Offset: 0x0016A0FC
		private bool CanStart()
		{
			if (this.mValidatingLevels)
			{
				return false;
			}
			if (!this.HasSelectedLevel)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < this.mPlayerSlots.Length; i++)
			{
				if (this.mPlayerSlots[i].AvatarSelected)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x06002D27 RID: 11559 RVA: 0x0016BF50 File Offset: 0x0016A150
		private unsafe void StartLevel()
		{
			if (RenderManager.Instance.IsTransitionActive)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing)
				{
					num++;
					if (players[i].Team == Factions.TEAM_RED)
					{
						num2++;
					}
					if (players[i].Team == Factions.TEAM_BLUE)
					{
						num3++;
					}
				}
			}
			if (this.mGameSettings.GameType == GameType.Versus)
			{
				if (num < 2)
				{
					return;
				}
				if (this.mVersusSettings.TeamsEnabled && (num2 == 0 || num3 == 0))
				{
					return;
				}
			}
			else if (num < 1)
			{
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
				for (int j = 0; j < networkServer.Connections; j++)
				{
					bool flag = true;
					SteamID steamID = networkServer.GetSteamID(j);
					for (int k = 0; k < players.Length; k++)
					{
						NetworkGamer networkGamer = players[k].Gamer as NetworkGamer;
						if (networkGamer != null && networkGamer.ClientID == steamID)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						ConnectionClosedMessage connectionClosedMessage = default(ConnectionClosedMessage);
						connectionClosedMessage.Reason = ConnectionClosedMessage.CReason.Kicked;
						networkServer.SendMessage<ConnectionClosedMessage>(ref connectionClosedMessage, j);
						Thread.Sleep(100);
						networkServer.CloseConnection(j, ConnectionClosedMessage.CReason.Kicked);
					}
				}
				MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
				menuSelectMessage.Option = 1;
				menuSelectMessage.IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect;
				byte* ptr = (byte*)(&menuSelectMessage.Param0I);
				byte[] combinedHash = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level).GetCombinedHash();
				for (int l = 0; l < combinedHash.Length; l++)
				{
					ptr[l] = combinedHash[l];
				}
				NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
			}
			if (this.mGameSettings.GameType == GameType.Campaign | this.mGameSettings.GameType == GameType.Mythos)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					SaveSlotInfo saveSlotInfo = new SaveSlotInfo(SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData);
					NetworkManager.Instance.Interface.SendMessage<SaveSlotInfo>(ref saveSlotInfo);
				}
				if (this.mGameSettings.Level == 0 && this.mGameSettings.GameType == GameType.Campaign)
				{
					SubMenuIntro.Instance.Play = true;
					Tome.Instance.PushMenu(SubMenuIntro.Instance, 10);
				}
				else
				{
					SubMenuCutscene.Instance.Play = true;
					SubMenuCutscene.Instance.Level = this.mGameSettings.Level;
					Tome.Instance.PushMenu(SubMenuCutscene.Instance, 10);
				}
			}
			else
			{
				int level = 0;
				bool flag2 = false;
				int level2 = this.mGameSettings.Level;
				if (this.mGameSettings.GameType == GameType.StoryChallange)
				{
					flag2 = true;
					if (level2 < 0 || level2 > LevelManager.Instance.DungeonsCampaign.Length - 1)
					{
						flag2 = false;
					}
					if (flag2)
					{
						LevelNode levelNode = LevelManager.Instance.DungeonsCampaign[level2];
						if (levelNode == null)
						{
							flag2 = false;
						}
						else if (string.IsNullOrEmpty(levelNode.FileName))
						{
							flag2 = false;
						}
						else
						{
							flag2 = false;
							for (int m = 0; m < SubMenuCharacterSelect.dungeonLevelFileNames.Length; m++)
							{
								flag2 |= (string.Compare(levelNode.FileName, SubMenuCharacterSelect.dungeonLevelFileNames[m]) == 0);
								level = (m + 1) * -1;
								if (flag2)
								{
									break;
								}
							}
						}
					}
				}
				if (flag2)
				{
					SubMenuCutscene.Instance.Play = true;
					SubMenuCutscene.Instance.Level = level;
					Tome.Instance.PushMenu(SubMenuCutscene.Instance, 20);
				}
				else
				{
					RenderManager.Instance.TransitionEnd += this.OnTransitionEnd;
					RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 0.5f);
				}
			}
			LevelNode levelNode2 = (this.mGameSettings.Level < 0) ? null : LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
			EventData[] iEvents = new EventData[]
			{
				TelemetryUtils.GetGameplayStartedData(this.mGameSettings.GameType.ToString(), levelNode2.Name.Substring(levelNode2.Name.IndexOf('#') + 1), Game.Instance.PlayerCount),
				TelemetryUtils.GetControllerChangedData(players)
			};
			Singleton<ParadoxServices>.Instance.TelemetryEvent(iEvents);
		}

		// Token: 0x06002D28 RID: 11560 RVA: 0x0016C384 File Offset: 0x0016A584
		private void OnLevelChange(Controller iSender, GameType iGameType, int iLevel)
		{
			if (iLevel == -1)
			{
				this.SetNoLevelSelected();
				return;
			}
			LevelNode level = LevelManager.Instance.GetLevel(iGameType, iLevel);
			if (level == null)
			{
				return;
			}
			bool flag = true;
			if (iGameType == GameType.Campaign || iGameType == GameType.Mythos)
			{
				SaveData currentSaveData = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
				if ((int)currentSaveData.Level == iLevel)
				{
					return;
				}
				flag = this.ChooseLevel(iLevel);
			}
			else
			{
				this.SetLevelUsed(level.Name);
				uint num = 0U;
				if (DLC_StatusHelper.ValidateLevelLocked(level, out num))
				{
					flag = false;
					SteamUtils.ActivateGameOverlayToWebPage("http://store.steampowered.com/app/" + num + "/");
					this.HasSelectedLevel = false;
					this.mStartButton.Enabled = false;
				}
				if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus)
				{
					this.SortLevelRepList();
				}
			}
			if (flag)
			{
				this.SetChangedLevel(iGameType, iLevel);
				this.mGameSettings.Level = iLevel;
				this.UpdateAvailableAvatars(default(DlcInstalled));
				this.UpdateChapterText(level);
			}
			if (level != null && level.AllowedAvatars != null && level.AllowedAvatars.Count > 0)
			{
				for (int i = 0; i < this.mPlayerSlots.Length; i++)
				{
					if (this.mPlayerSlots[i].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
					{
						this.mPlayerSlots[i].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
					}
				}
			}
		}

		// Token: 0x06002D29 RID: 11561 RVA: 0x0016C4C8 File Offset: 0x0016A6C8
		private void UpdateChapterText(LevelNode iLevel)
		{
			int iTargetLineWidth = 560;
			string text = LanguageManager.Instance.GetString(iLevel.Name.GetHashCodeCustom());
			string text2 = this.mChapterName.Font.Wrap(text, iTargetLineWidth, true);
			this.mChapterName.SetText(text2);
			if (this.mGameSettings.GameType == GameType.Campaign || this.mGameSettings.GameType == GameType.Mythos)
			{
				iTargetLineWidth = 560;
			}
			else
			{
				iTargetLineWidth = 940;
			}
			text = LanguageManager.Instance.GetString(iLevel.Description);
			text2 = this.mChapterDescription.Font.Wrap(text, iTargetLineWidth, true);
			int num = this.mChapterDescription.Font.LineHeight * 4;
			int num2 = text.Length;
			Vector2 vector = this.mChapterDescription.Font.MeasureText(text2.ToCharArray(), true);
			if (vector.Y > (float)num)
			{
				while (vector.Y > (float)num)
				{
					num2 = text.LastIndexOf(" ");
					if (num2 == -1)
					{
						num2 = text.Length;
						if (num2 > 3)
						{
							num2 -= 3;
						}
					}
					text = text.Substring(0, num2);
					text = text.Trim();
					text += "...";
					text2 = this.mChapterDescription.Font.Wrap(text, iTargetLineWidth, true);
					vector = this.mChapterDescription.Font.MeasureText(text2.ToCharArray(), true);
				}
			}
			this.mChapterDescription.SetText(text2);
		}

		// Token: 0x06002D2A RID: 11562 RVA: 0x0016C628 File Offset: 0x0016A828
		private void SetNoLevelSelected()
		{
			this.HasSelectedLevel = false;
			this.mGameSettings.Level = -1;
			this.mGameSettings.GameName = NetworkManager.Instance.GameName;
			this.mCustomLevel = false;
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				string iLevelName = SubMenuOnline.LOC_NO_LEVEL_SELECTED.ToString();
				NetworkManager.Instance.SetGame(this.mGameSettings.GameType, iLevelName);
				NetworkManager.Instance.Interface.SendMessage<GameInfoMessage>(ref this.mGameSettings);
			}
		}

		// Token: 0x06002D2B RID: 11563 RVA: 0x0016C6AC File Offset: 0x0016A8AC
		private void SetChangedLevel(GameType iGameType, int iLevel)
		{
			LevelNode level = LevelManager.Instance.GetLevel(iGameType, iLevel);
			HackHelper.License license = HackHelper.CheckLicense(level);
			if (license == HackHelper.License.No)
			{
				return;
			}
			this.mGameSettings.GameType = iGameType;
			this.mGameSettings.Level = iLevel;
			this.mGameSettings.GameName = NetworkManager.Instance.GameName;
			this.mCustomLevel = (license == HackHelper.License.Custom);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				string fileName = level.FileName;
				NetworkManager.Instance.SetGame(iGameType, fileName);
				NetworkManager.Instance.Interface.SendMessage<GameInfoMessage>(ref this.mGameSettings);
				this.HasSelectedLevel = true;
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				this.HasSelectedLevel = (iLevel >= 0);
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Offline)
			{
				this.HasSelectedLevel = true;
			}
		}

		// Token: 0x06002D2C RID: 11564 RVA: 0x0016C774 File Offset: 0x0016A974
		private bool ChooseLevel(int iLevel)
		{
			SaveData currentSaveData = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
			MemoryStream checkpoint = currentSaveData.Checkpoint;
			bool flag = checkpoint != null && checkpoint.Length > 0L;
			if (flag)
			{
				this.mLevelToSet = iLevel;
				this.mCheckPointRUSure.SelectedIndex = 1;
				this.mCheckPointRUSure.Show();
			}
			else
			{
				currentSaveData.Level = (byte)iLevel;
				SaveManager.Instance.SaveCampaign();
			}
			return !flag;
		}

		// Token: 0x06002D2D RID: 11565 RVA: 0x0016C7E0 File Offset: 0x0016A9E0
		private void DeleteCheckpointCallback(MessageBox iSender, int iSelection)
		{
			if (iSelection == 0)
			{
				SaveData currentSaveData = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
				currentSaveData.Level = (byte)this.mLevelToSet;
				currentSaveData.Checkpoint = null;
				SaveManager.Instance.SaveCampaign();
				LevelNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mLevelToSet);
				this.UpdateChapterText(level);
				this.SetChangedLevel(this.mGameSettings.GameType, this.mLevelToSet);
			}
		}

		// Token: 0x06002D2E RID: 11566 RVA: 0x0016C854 File Offset: 0x0016AA54
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			int num = -1;
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag))
			{
				if (flag)
				{
					if (this.mGamerDropDownMenu.IsVisible)
					{
						this.mGamerDropDownMenu.SelectedIndex = this.mGamerDropDownMenu.GetHitIndex(ref vector);
					}
					else if (this.mAdminDropDownMenu.IsVisible)
					{
						this.mAdminDropDownMenu.SelectedIndex = this.mAdminDropDownMenu.GetHitIndex(ref vector);
					}
					else if (this.mGameModeBox.IsDown)
					{
						this.mGameModeBox.NewSelection = this.mGameModeBox.GetHitIndex(ref vector);
					}
					else
					{
						for (int i = 0; i < this.mVersusSettings.MenuItems.Count; i++)
						{
							if (this.mVersusSettings.MenuItems[i].IsDown)
							{
								this.mVersusSettings.MenuItems[i].NewSelection = this.mVersusSettings.MenuItems[i].GetHitIndex(ref vector);
								goto IL_9C1;
							}
						}
						if (this.mCurrentState == SubMenuCharacterSelect.State.Normal)
						{
							if (this.mGameSettings.GameType == GameType.Mythos || this.mGameSettings.GameType == GameType.Campaign)
							{
								if (SubMenuCharacterSelect.mLevelRepresentations != null && vector.X >= 64f && vector.X <= 512f && vector.Y >= 64f && vector.Y <= 629.3333f && NetworkManager.Instance.State != NetworkState.Client)
								{
									num = 9;
								}
							}
							else
							{
								if (this.mGameSettings.GameType == GameType.Versus)
								{
									if (this.mGameModeBox.InsideBounds(ref vector))
									{
										num = 7;
									}
									for (int j = this.mSettingsScrollbar.Value; j < Math.Min(this.mSettingsScrollbar.Value + 5, this.mVersusSettings.MenuItems.Count); j++)
									{
										if (this.mVersusSettings.MenuItems[j].InsideBounds(ref vector))
										{
											num = j + 8;
										}
									}
								}
								if (this.HasSelectedLevel)
								{
									if (vector.X >= 64f && vector.X <= 512f && vector.Y >= 480f && vector.Y <= 629.3333f)
									{
										num = 6;
									}
								}
								else if (this.mSelectLevelButton.InsideBounds(iState))
								{
									num = 6;
								}
								if ((this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus) && this.HitPackOverview(ref vector, out this.mSelectedPack, out this.mSelectedPackScroll) && !(this.mVersusSettings is Krietor.Settings))
								{
									num = 5;
								}
							}
						}
						if (this.mSettingsScrollbar.Grabbed)
						{
							this.mSettingsScrollbar.ScrollTo(vector.Y);
						}
						if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
						{
							if (this.mSpecialScrollBar.Grabbed)
							{
								this.mSpecialScrollBar.ScrollTo(vector.Y);
							}
							int num2 = -1;
							if (vector.X >= 64f && vector.X <= 448f)
							{
								float num3 = 64f;
								float num4 = num3 + 140f;
								int num5 = Math.Min(this.mSpecialScrollBar.Value + 5, SubMenuCharacterSelect.mLevelRepresentations.Count);
								for (int k = this.mSpecialScrollBar.Value; k <= num5; k++)
								{
									if (vector.Y >= num3 && vector.Y <= num4)
									{
										num2 = k;
										break;
									}
									num3 += 140f;
									num4 += 140f;
								}
							}
							this.mSelectedPack = num2;
						}
						else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
						{
							if (this.mSpecialScrollBar.Grabbed)
							{
								this.mSpecialScrollBar.ScrollTo(vector.Y);
								this.mSelectedPack = -1;
							}
							else if (this.mPackCloseButton.InsideBounds(ref vector))
							{
								num = 5;
								this.mSelectedPack = -1;
							}
							else if ((this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus) && this.HitPackList(ref vector, out this.mSelectedPack))
							{
								num = 5;
							}
						}
						if (NetworkManager.Instance.State != NetworkState.Offline && NetworkChat.Instance.ScrollBar.Grabbed)
						{
							NetworkChat.Instance.ScrollBar.ScrollTo(vector.Y);
						}
						this.mBackButton.Selected = this.mBackButton.InsideBounds(ref vector);
						if (this.mStartButton.InsideBounds(ref vector))
						{
							num = 4;
						}
						if (iSender.Player == null || !iSender.Player.Playing)
						{
							int num6 = -1;
							if (vector.X >= 544f && vector.X <= 992f)
							{
								Player[] players = Game.Instance.Players;
								float num7 = 89f;
								float num8 = 201f;
								for (int l = 0; l < 4; l++)
								{
									if (this.mPlayerSlots[l].State != SubMenuCharacterSelect.GamerState.Locked && !players[l].Playing && vector.Y >= num7 && vector.Y <= num8)
									{
										num6 = l;
										break;
									}
									num7 += 137f;
									num8 += 137f;
								}
							}
							if (num6 >= 0)
							{
								num = num6;
							}
						}
						else
						{
							int id = iSender.Player.ID;
							if (iSender.Player.Gamer != Gamer.INVALID_GAMER)
							{
								if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.Open)
								{
									float num9 = 89f + (float)iSender.Player.ID * 144f;
									float num10 = num9 + 112f;
									if (NetworkManager.Instance.State != NetworkState.Offline && vector.X >= 912f && vector.X <= 1040f && vector.Y >= num10 - 80f && vector.Y <= num10)
									{
										this.mPlayerSlots[iSender.Player.ID].SelectedItem = 0;
										num = iSender.Player.ID;
									}
									else if (vector.X >= 544f && vector.X <= 992f && vector.Y >= num9 && vector.Y <= num10)
									{
										num = iSender.Player.ID;
										this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
									}
									else
									{
										this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
									}
								}
								else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.Ready)
								{
									float num11 = 89f + (float)iSender.Player.ID * 144f;
									float num12 = num11 + 112f;
									if (NetworkManager.Instance.State != NetworkState.Offline && vector.X >= 912f && vector.X <= 1040f && vector.Y >= num12 - 80f && vector.Y <= num12)
									{
										this.mPlayerSlots[iSender.Player.ID].SelectedItem = 0;
										num = iSender.Player.ID;
									}
									else
									{
										this.mPlayerSlots[id].SelectedItem = -1;
									}
								}
								else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
								{
									int selectedItem;
									if (this.HitAvatar(id, ref vector, out selectedItem))
									{
										this.mPlayerSlots[id].SelectedItem = selectedItem;
									}
									else
									{
										this.mPlayerSlots[id].SelectedItem = -1;
									}
								}
								else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
								{
									int selectedItem2;
									if (this.HitColor(id, ref vector, out selectedItem2))
									{
										this.mPlayerSlots[id].SelectedItem = selectedItem2;
									}
									else
									{
										this.mPlayerSlots[id].SelectedItem = -1;
									}
								}
								else
								{
									this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
								}
							}
							else
							{
								this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
							}
						}
						if (iSender.Player != null && iSender.Player.Playing && iSender.Player.Gamer == Gamer.INVALID_GAMER)
						{
							this.mSelectedPosition = -1;
							MenuScrollBar menuScrollBar = this.mGamerScrollBars[iSender.Player.ID];
							if (menuScrollBar.Grabbed)
							{
								menuScrollBar.ScrollTo(vector.Y);
							}
							else if (vector.X >= 672f && vector.X <= 960f)
							{
								float num13 = menuScrollBar.Position.Y - 62f;
								float num14 = menuScrollBar.Position.Y - 31f;
								int selectedItem3 = -1;
								for (int m = menuScrollBar.Value; m < Math.Min(menuScrollBar.Value + 4, this.mGamerItems.Count); m++)
								{
									if (vector.Y >= num13 && vector.Y <= num14)
									{
										selectedItem3 = m;
										break;
									}
									num13 += 31f;
									num14 += 31f;
								}
								this.mPlayerSlots[iSender.Player.ID].SelectedItem = selectedItem3;
							}
						}
					}
				}
				else
				{
					this.mSelectedPack = -1;
					if (iSender.Player != null && iSender.Player.Playing)
					{
						this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
					}
				}
			}
			else
			{
				this.mSelectedPack = -1;
				if (iSender.Player != null && iSender.Player.Playing)
				{
					this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
				}
			}
			IL_9C1:
			if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
			{
				return;
			}
			bool flag2 = false;
			if (iSender.Player != null && iSender.Player.Playing)
			{
				int id2 = iSender.Player.ID;
				if (this.mPlayerSlots[id2].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
				{
					flag2 = true;
					this.ShowAvatarToolTip(iSender, this.mPlayerSlots[id2].SelectedItem, new MouseState?(iState));
				}
				else if (this.mPlayerSlots[id2].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
				{
					flag2 = true;
					ToolTipMan.Instance.Kill(iSender.Player, false);
				}
			}
			if (!flag2 && this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel && this.mSelectedPack >= 0 && this.mSelectedPack < SubMenuCharacterSelect.mLevelRepresentations.Count)
			{
				LevelNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, SubMenuCharacterSelect.mLevelRepresentations[this.mSelectedPack].HashSum);
				if (level != null)
				{
					flag2 = true;
					ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(level.Description), iState);
				}
			}
			if (!flag2 && this.mGameSettings.GameType == GameType.Versus)
			{
				int num15 = num - 8;
				if (num15 >= 0 && num15 < this.mVersusSettings.ToolTips.Count)
				{
					flag2 = true;
					ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(this.mVersusSettings.ToolTips[num15]), iState);
				}
			}
			if (!flag2)
			{
				switch (num)
				{
				case 0:
				case 1:
				case 2:
				case 3:
					if (iSender.Player == null || !iSender.Player.Playing)
					{
						if (!Game.Instance.Players[num].Playing)
						{
							ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_JOIN), iState);
							goto IL_DE4;
						}
						goto IL_DE4;
					}
					else
					{
						if (num == iSender.Player.ID)
						{
							ToolTipMan.Instance.Set(iSender.Player, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_TT_CUSTOMIZE), iState);
							goto IL_DE4;
						}
						goto IL_DE4;
					}
					break;
				case 5:
				{
					IPack[] allPacks = PackMan.Instance.AllPacks;
					if (this.mSelectedPack >= 0 && this.mSelectedPack < allPacks.Length)
					{
						ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(allPacks[this.mSelectedPack].Name) + "\n" + LanguageManager.Instance.GetStringWithReferencs(allPacks[this.mSelectedPack].Descritpion), iState);
						goto IL_DE4;
					}
					ToolTipMan.Instance.Kill(null, false);
					goto IL_DE4;
				}
				case 6:
				{
					if (NetworkManager.Instance.State != NetworkState.Client || !this.HasSelectedLevel)
					{
						ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_TT_CHANGE_LEVEL), iState);
						goto IL_DE4;
					}
					LevelNode levelNode = null;
					GameType gameType = this.mGameSettings.GameType;
					switch (gameType)
					{
					case GameType.Campaign:
						levelNode = LevelManager.Instance.VanillaCampaign[this.mGameSettings.Level];
						break;
					case GameType.Challenge:
						levelNode = LevelManager.Instance.Challenges[this.mGameSettings.Level];
						break;
					case (GameType)3:
						break;
					case GameType.Versus:
						levelNode = LevelManager.Instance.Versus[this.mGameSettings.Level];
						break;
					default:
						if (gameType != GameType.Mythos)
						{
							if (gameType == GameType.StoryChallange)
							{
								levelNode = LevelManager.Instance.StoryChallanges[this.mGameSettings.Level];
							}
						}
						else
						{
							levelNode = LevelManager.Instance.MythosCampaign[this.mGameSettings.Level];
						}
						break;
					}
					if (levelNode != null)
					{
						ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(levelNode.Name.GetHashCodeCustom()), iState);
						goto IL_DE4;
					}
					goto IL_DE4;
				}
				case 7:
					ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_TT_GAME_MODE), iState);
					goto IL_DE4;
				case 9:
					ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenu.LOC_CHANGE_CHAPTER), iState);
					goto IL_DE4;
				}
				ToolTipMan.Instance.Kill(null, false);
				ToolTipMan.Instance.Kill(iSender.Player, false);
			}
			IL_DE4:
			this.mSelectedPosition = num;
		}

		// Token: 0x06002D2F RID: 11567 RVA: 0x0016D64C File Offset: 0x0016B84C
		private void ShowAvatarToolTip(Controller iSender, int iIndex, MouseState? iMouseState)
		{
			IList<Profile.PlayableAvatar> values = Profile.Instance.Avatars.Values;
			if (iIndex < 0 || iIndex >= SubMenuCharacterSelect.mRobeRepresentations.Count)
			{
				ToolTipMan.Instance.Kill(iSender.Player, false);
				return;
			}
			string iString = LanguageManager.Instance.GetStringWithReferencs(SubMenuCharacterSelect.mRobeRepresentations[iIndex].DisplayName) + "\n" + LanguageManager.Instance.GetStringWithReferencs(SubMenuCharacterSelect.mRobeRepresentations[iIndex].Description);
			if (iMouseState != null)
			{
				ToolTipMan.Instance.Set(iSender.Player, iString, iMouseState.Value);
				return;
			}
			Vector2 vector = default(Vector2);
			vector.X = 544f;
			vector.Y = 89f + (float)iSender.Player.ID * 137f;
			vector.X = vector.X + 224f - 144f;
			vector.Y += 56f;
			int num = 0;
			if (SubMenuCharacterSelect.mRobeRepresentations.Count > 4)
			{
				num = this.mPlayerSlots[iSender.Player.ID].ScrollValue;
			}
			vector.X += 96f * (float)((iIndex - num + values.Count) % values.Count);
			vector.Y += 56f;
			Vector2 vector2;
			Tome.Instance.PageToScreen(true, ref vector, out vector2);
			ToolTipMan.Instance.Set(iSender.Player, iString, ref vector2);
		}

		// Token: 0x06002D30 RID: 11568 RVA: 0x0016D7E0 File Offset: 0x0016B9E0
		private void GamerSelected(Controller iController, Gamer iGamer)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			this.UpdateGamers();
			if (iGamer == null)
			{
				this.mPlayerSlots[iController.Player.ID].AvatarSelected = false;
				this.mPlayerSlots[iController.Player.ID].ControllerType = -1;
				ToolTipMan.Instance.Kill(iController.Player, false);
				iController.Player.Leave();
				this.UpdateAvailableAvatars(default(DlcInstalled));
			}
			else
			{
				HackHelper.License license = HackHelper.CheckLicense(iGamer.Avatar);
				if (license != HackHelper.License.Yes && (license != HackHelper.License.Custom || !this.AllowCustom))
				{
					iGamer.Avatar = Profile.Instance.DefaultAvatar;
				}
				this.mPlayerSlots[iController.Player.ID].Custom = (!(iGamer is NetworkGamer) && HackHelper.CheckLicense(iGamer.Avatar) != HackHelper.License.Yes);
				this.mPlayerSlots[iController.Player.ID].Name.SetText(iGamer.GamerTag);
				iController.Player.Gamer = iGamer;
				this.mPlayerSlots[iController.Player.ID].AvatarSelected = true;
				if (!(iGamer is NetworkGamer))
				{
					if (!Profile.Instance.LastGamer.InUse)
					{
						Profile.Instance.LastGamer = iController.Player.Gamer;
						Profile.Instance.Write();
					}
					if (this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled && iController.Player.Team == Factions.NONE)
					{
						int num = 0;
						int num2 = 0;
						Player[] players = Game.Instance.Players;
						for (int i = 0; i < players.Length; i++)
						{
							if (players[i].Playing)
							{
								if ((players[i].Team & Factions.TEAM_RED) != Factions.NONE)
								{
									num++;
								}
								if ((players[i].Team & Factions.TEAM_BLUE) != Factions.NONE)
								{
									num2++;
								}
							}
						}
						if (num <= num2)
						{
							iController.Player.Team = Factions.TEAM_RED;
						}
						else
						{
							iController.Player.Team = Factions.TEAM_BLUE;
						}
						if (NetworkManager.Instance.State != NetworkState.Offline)
						{
							MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
							menuSelectMessage.Option = 2;
							menuSelectMessage.Param0I = iController.Player.ID;
							menuSelectMessage.Param1I = (int)iController.Player.Team;
							NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
						}
					}
				}
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(iController.Player);
					gamerChangedMessage.UnlockedMagicks = iController.Player.UnlockedMagicks;
					NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
				}
				LevelNode levelNode = null;
				if (this.mGameSettings.Level > -1)
				{
					levelNode = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
				}
				if (levelNode != null && levelNode.AllowedAvatars.Count > 0)
				{
					this.DefaultAvatar(iController);
				}
				else
				{
					Player player = iController.Player;
					this.VerifyAvatar(ref player);
				}
			}
			if (!(iGamer is NetworkGamer))
			{
				this.SetReady(false);
			}
		}

		// Token: 0x06002D31 RID: 11569 RVA: 0x0016DB04 File Offset: 0x0016BD04
		public void SetVsSettings(ref VersusRuleset.Settings.OptionsMessage iVersusSettings)
		{
			VersusRuleset.Settings settings = this.mVersusSettings;
			VersusRuleset.Settings.ApplyMessage(ref settings, ref iVersusSettings);
			if (settings != this.mVersusSettings)
			{
				this.mVersusSettings.Changed -= this.mVersusSettings_Changed;
				this.mVersusSettings = settings;
				this.mVersusSettings.Changed += this.mVersusSettings_Changed;
				LanguageManager instance = LanguageManager.Instance;
				this.mVersusSettingsTitles = new Text[this.mVersusSettings.MenuTitles.Count];
				int i;
				for (i = 0; i < this.mVersusSettings.MenuTitles.Count; i++)
				{
					this.mVersusSettingsTitles[i] = new Text(48, this.mVersusSettings.MenuItems[i].Font, TextAlign.Left, false);
					this.mVersusSettingsTitles[i].SetText(instance.GetString(this.mVersusSettings.MenuTitles[i]));
				}
				this.mGameModeBox.ValueChanged -= new Action<DropDownBox, Rulesets>(this.mGameModeBox_ValueChanged);
				for (i = 0; i < this.mGameModeBox.Values.Length; i++)
				{
					if (this.mGameModeBox.Values[i] == iVersusSettings.Ruleset)
					{
						this.mGameModeBox.SelectedIndex = i;
						break;
					}
				}
				if (i == this.mGameModeBox.Values.Length)
				{
					Console.WriteLine("Invalid settings!");
					this.mGameModeBox.SelectedIndex = 0;
				}
				this.mGameModeBox.ValueChanged += new Action<DropDownBox, Rulesets>(this.mGameModeBox_ValueChanged);
			}
		}

		// Token: 0x06002D32 RID: 11570 RVA: 0x0016DC77 File Offset: 0x0016BE77
		public void SetSettings(GameType iGameType, string tryLevelName, bool iCustomLevel)
		{
			this.mLevelNameToFocusWhenLevelComplete = tryLevelName;
			this.SetSettings(iGameType, -1, iCustomLevel);
		}

		// Token: 0x06002D33 RID: 11571 RVA: 0x0016DC8C File Offset: 0x0016BE8C
		public void SetSettings(GameType iGameType, int iLevel, bool iCustomLevel)
		{
			GameType gameType = this.mGameSettings.GameType;
			int num = this.mGameSettings.Level;
			if (gameType != iGameType)
			{
				num = 0;
			}
			if (iLevel == -1)
			{
				this.HasSelectedLevel = false;
				this.mStartButton.Enabled = false;
				this.mGameSettings.Level = num;
				iCustomLevel = false;
			}
			else
			{
				this.HasSelectedLevel = true;
				this.mGameSettings.Level = iLevel;
			}
			if (gameType != iGameType || SubMenuCharacterSelect.mLevelRepresentations == null || (gameType == iGameType && !string.IsNullOrEmpty(this.mLevelNameToFocusWhenLevelComplete)))
			{
				if (SubMenuCharacterSelect.mLevelRepresentations != null)
				{
					SubMenuCharacterSelect.mLevelRepresentations.Clear();
				}
				this.mSelectLevelButton.Enabled = (this.mSelectLevelButton.Selected = false);
				this.mSelectLevelButton.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LOADING));
				this.HasSelectedLevel = true;
				this.mGameSettings.Level = iLevel;
				this.mLoadingLevels = true;
				Game.Instance.AddLoadTask(new Action(this.LoadLevelPreviews));
				Thread.Sleep(0);
			}
			this.mGameSettings.GameType = iGameType;
			this.mCustomLevel = iCustomLevel;
			this.UpdateAvailableAvatars(default(DlcInstalled));
			this.mGameSettings.Level = iLevel;
			LevelNode levelNode = (iLevel < 0) ? null : LevelManager.Instance.GetLevel(iGameType, iLevel);
			if (levelNode != null)
			{
				this.UpdateChapterText(levelNode);
			}
			else
			{
				this.mChapterName.SetText(null);
				this.mChapterDescription.SetText(null);
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				GameInfoMessage gameInfoMessage = default(GameInfoMessage);
				gameInfoMessage.NrOfPlayers = (byte)Game.Instance.PlayerCount;
				gameInfoMessage.GameName = NetworkManager.Instance.GameName;
				gameInfoMessage.GameType = iGameType;
				gameInfoMessage.Level = iLevel;
				NetworkManager.Instance.Interface.SendMessage<GameInfoMessage>(ref gameInfoMessage);
			}
			else if (NetworkManager.Instance.State == NetworkState.Client)
			{
				if (num != iLevel && levelNode != null && levelNode.AllowedAvatars != null && levelNode.AllowedAvatars.Count > 0)
				{
					for (int i = 0; i < this.mPlayerSlots.Length; i++)
					{
						if (this.mPlayerSlots[i].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
						{
							this.mPlayerSlots[i].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
						}
					}
				}
				GamerReadyMessage gamerReadyMessage;
				gamerReadyMessage.Id = 0;
				gamerReadyMessage.Ready = false;
				NetworkManager.Instance.Interface.SendMessage<GamerReadyMessage>(ref gamerReadyMessage);
			}
			this.SetReady(false);
			if (iLevel < -1)
			{
				ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_MISMATCH));
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					NetworkManager.Instance.EndSession();
				}
				if (Tome.Instance.CurrentMenu is SubMenuCharacterSelect)
				{
					Tome.Instance.PopMenu();
				}
				return;
			}
			this.ResetLevelTexts();
		}

		// Token: 0x06002D34 RID: 11572 RVA: 0x0016DF39 File Offset: 0x0016C139
		public void GetSettings(out GameType oGameType, out int oLevel, out VersusRuleset.Settings.OptionsMessage oVersusSettings)
		{
			this.mVersusSettings.GetMessage(out oVersusSettings);
			oGameType = this.mGameSettings.GameType;
			oLevel = this.mGameSettings.Level;
		}

		// Token: 0x17000AA5 RID: 2725
		// (get) Token: 0x06002D35 RID: 11573 RVA: 0x0016DF61 File Offset: 0x0016C161
		public GameType GameType
		{
			get
			{
				return this.mGameSettings.GameType;
			}
		}

		// Token: 0x06002D36 RID: 11574 RVA: 0x0016DF70 File Offset: 0x0016C170
		public void JoinPlayer(Controller iSender, int iIndex, Gamer iGamer)
		{
			if (iGamer == null)
			{
				iGamer = Gamer.INVALID_GAMER;
			}
			Player player = Player.Join(iSender, iIndex, iGamer);
			if (player != null)
			{
				this.mPlayerSlots[player.ID].State = SubMenuCharacterSelect.GamerState.Open;
				this.mPlayerSlots[player.ID].Name.SetText(player.GamerTag);
			}
		}

		// Token: 0x06002D37 RID: 11575 RVA: 0x0016DFCC File Offset: 0x0016C1CC
		public void SetPlayerActive(Controller iSender)
		{
			if (iSender.Player != null && iSender.Player.Gamer != null)
			{
				Player.Join(iSender, iSender.Player.ID, iSender.Player.Gamer);
				Player player = iSender.Player;
				this.VerifyAvatar(ref player);
				return;
			}
			if (!Profile.Instance.LastGamer.InUse)
			{
				Player.Join(iSender, -1, Profile.Instance.LastGamer);
				if (iSender.Player != null && iSender.Player.Gamer != null)
				{
					this.UpdateGamer(iSender.Player, iSender.Player.Gamer);
				}
				Player player2 = iSender.Player;
				this.VerifyAvatar(ref player2);
				return;
			}
			Player.Join(iSender, -1, Gamer.INVALID_GAMER);
		}

		// Token: 0x06002D38 RID: 11576 RVA: 0x0016E089 File Offset: 0x0016C289
		private bool VerifyAvatar(ref Player p)
		{
			return this.VerifyAvatar(ref p, true);
		}

		// Token: 0x06002D39 RID: 11577 RVA: 0x0016E094 File Offset: 0x0016C294
		private bool VerifyAvatar(ref Player p, bool ignoreIfClient)
		{
			if (ignoreIfClient && NetworkManager.Instance.State == NetworkState.Client)
			{
				return true;
			}
			int num = 0;
			try
			{
				num = ((SubMenuCharacterSelect.mRobeRepresentations == null) ? 0 : SubMenuCharacterSelect.mRobeRepresentations[0].OriginalIndex);
			}
			catch
			{
				num = 0;
			}
			if (num == -1)
			{
				num = 0;
			}
			GameType gameType = this.mGameSettings.GameType;
			switch (gameType)
			{
			case GameType.Campaign:
				goto IL_1BA;
			case GameType.Challenge:
				break;
			case (GameType)3:
				goto IL_1F0;
			case GameType.Versus:
				if (p.Gamer.Avatar.AllowPVP)
				{
					return true;
				}
				p.Gamer.Avatar = Profile.Instance.Avatars.Values[num];
				goto IL_1F0;
			default:
				if (gameType == GameType.Mythos)
				{
					goto IL_1BA;
				}
				if (gameType != GameType.StoryChallange)
				{
					goto IL_1F0;
				}
				break;
			}
			if (p.Gamer.Avatar.AllowChallenge)
			{
				return true;
			}
			p.Gamer.Avatar = Profile.Instance.Avatars.Values[num];
			if (string.Compare(p.Gamer.Avatar.Name, "wizardalu") == 0)
			{
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i] != null && players[i].Gamer != null)
					{
						Profile.PlayableAvatar avatar = players[i].Gamer.Avatar;
						if (players[i] != p && (string.Compare(players[i].Gamer.Avatar.Name, "wizardalu") == 0 || string.Compare(players[i].Gamer.Avatar.TypeName, "wizard_alucart") == 0))
						{
							p.Gamer.Avatar = Profile.Instance.Avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[1].OriginalIndex];
							break;
						}
					}
				}
				goto IL_1F0;
			}
			goto IL_1F0;
			IL_1BA:
			if (p.Gamer.Avatar.AllowCampaign)
			{
				return true;
			}
			p.Gamer.Avatar = Profile.Instance.Avatars.Values[num];
			IL_1F0:
			if (NetworkManager.Instance.State != NetworkState.Offline)
			{
				GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(p);
				NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
			}
			return false;
		}

		// Token: 0x06002D3A RID: 11578 RVA: 0x0016E2D0 File Offset: 0x0016C4D0
		private void Start()
		{
			if (this.mValidatingLevels)
			{
				return;
			}
			int num = 0;
			bool flag = true;
			int num2 = 0;
			int num3 = 0;
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing)
				{
					num++;
					if (players[i].Gamer is NetworkGamer && this.mPlayerSlots[i].State != SubMenuCharacterSelect.GamerState.Ready)
					{
						flag = false;
					}
					if (players[i].Team == Factions.TEAM_RED)
					{
						num2++;
					}
					if (players[i].Team == Factions.TEAM_BLUE)
					{
						num3++;
					}
				}
			}
			if (this.mGameSettings.GameType == GameType.Versus)
			{
				if (num < 2)
				{
					return;
				}
				if (this.mVersusSettings.TeamsEnabled && (num2 == 0 || num3 == 0))
				{
					return;
				}
			}
			else if (num < 1)
			{
				return;
			}
			this.Start(flag ? 3 : 10);
		}

		// Token: 0x06002D3B RID: 11579 RVA: 0x0016E3AC File Offset: 0x0016C5AC
		private void Start(int iCountDown)
		{
			if (this.mValidatingLevels)
			{
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
				menuSelectMessage.IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect;
				menuSelectMessage.Option = 4;
				menuSelectMessage.Param0I = iCountDown;
				NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref menuSelectMessage);
			}
			string iMessage = LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_COUNTDOWN).Replace("#1;", iCountDown.ToString());
			NetworkChat.Instance.AddMessage(iMessage);
			this.mCountDown = (float)iCountDown;
			this.mLastCountDownNr = iCountDown;
		}

		// Token: 0x06002D3C RID: 11580 RVA: 0x0016E43C File Offset: 0x0016C63C
		internal void SetReady(bool iReady)
		{
			Player[] players = Game.Instance.Players;
			byte b = 0;
			while ((int)b < players.Length)
			{
				if (!(players[(int)b].Gamer is NetworkGamer))
				{
					this.SetReady(iReady, b);
				}
				b += 1;
			}
		}

		// Token: 0x06002D3D RID: 11581 RVA: 0x0016E47C File Offset: 0x0016C67C
		internal void SetReady(bool iReady, byte iID)
		{
			if (NetworkManager.Instance.State == NetworkState.Offline)
			{
				if (this.mPlayerSlots[(int)iID].State == SubMenuCharacterSelect.GamerState.Ready)
				{
					this.mPlayerSlots[(int)iID].State = SubMenuCharacterSelect.GamerState.Open;
				}
				return;
			}
			if (iReady)
			{
				this.mPlayerSlots[(int)iID].State = SubMenuCharacterSelect.GamerState.Ready;
			}
			else
			{
				if (this.mLastCountDownNr >= 0)
				{
					NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_COUNTDOWN_ABORTED));
					this.mLastCountDownNr = -1;
					this.mCountDown = 0f;
				}
				if (this.mPlayerSlots[(int)iID].State == SubMenuCharacterSelect.GamerState.Ready)
				{
					this.mPlayerSlots[(int)iID].State = SubMenuCharacterSelect.GamerState.Open;
				}
			}
			if (NetworkManager.Instance.State != NetworkState.Offline && Game.Instance.Players[(int)iID].Playing && !(Game.Instance.Players[(int)iID].Gamer is NetworkGamer))
			{
				GamerReadyMessage gamerReadyMessage = default(GamerReadyMessage);
				gamerReadyMessage.Id = iID;
				gamerReadyMessage.Ready = iReady;
				NetworkManager.Instance.Interface.SendMessage<GamerReadyMessage>(ref gamerReadyMessage);
			}
		}

		// Token: 0x06002D3E RID: 11582 RVA: 0x0016E58D File Offset: 0x0016C78D
		internal bool GetReady(int iID)
		{
			return this.mPlayerSlots[iID].State == SubMenuCharacterSelect.GamerState.Ready;
		}

		// Token: 0x06002D3F RID: 11583 RVA: 0x0016E5A4 File Offset: 0x0016C7A4
		internal override void NetworkInput(ref MenuSelectMessage iMessage)
		{
			if (iMessage.Option == 0)
			{
				this.mVersusSettings.MenuItems[iMessage.Param0I].SelectedIndex = iMessage.Param1I;
				return;
			}
			if (iMessage.Option == 1)
			{
				this.StartLevel();
				return;
			}
			if (iMessage.Option == 2)
			{
				Game.Instance.Players[iMessage.Param0I].Team = (Factions)iMessage.Param1I;
				return;
			}
			if (iMessage.Option == 4)
			{
				this.Start(iMessage.Param0I);
				return;
			}
			throw new InvalidOperationException();
		}

		// Token: 0x06002D40 RID: 11584 RVA: 0x0016E638 File Offset: 0x0016C838
		private void OnTransitionEnd(TransitionEffect iDeadTransition)
		{
			RenderManager.Instance.TransitionEnd -= this.OnTransitionEnd;
			SpawnPoint? iSpawnPoint = null;
			GameType gameType = this.mGameSettings.GameType;
			LevelNode levelNode;
			switch (gameType)
			{
			case GameType.Campaign:
				levelNode = LevelManager.Instance.VanillaCampaign[this.mGameSettings.Level];
				iSpawnPoint = (levelNode as CampaignNode).SpawnPoint;
				goto IL_F7;
			case GameType.Challenge:
				levelNode = LevelManager.Instance.Challenges[this.mGameSettings.Level];
				goto IL_F7;
			case (GameType)3:
				break;
			case GameType.Versus:
				levelNode = LevelManager.Instance.Versus[this.mGameSettings.Level];
				goto IL_F7;
			default:
				if (gameType == GameType.Mythos)
				{
					levelNode = LevelManager.Instance.MythosCampaign[this.mGameSettings.Level];
					iSpawnPoint = (levelNode as CampaignNode).SpawnPoint;
					goto IL_F7;
				}
				if (gameType == GameType.StoryChallange)
				{
					levelNode = LevelManager.Instance.StoryChallanges[this.mGameSettings.Level];
					goto IL_F7;
				}
				break;
			}
			throw new Exception("Invalid Game Type");
			IL_F7:
			NetworkState state = NetworkManager.Instance.State;
			SaveData iSaveSlot = null;
			if (NetworkManager.Instance.State != NetworkState.Client && (this.mGameSettings.GameType == GameType.Campaign | this.mGameSettings.GameType == GameType.Mythos))
			{
				iSaveSlot = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
			}
			bool iCustom = HackHelper.LicenseStatus == HackHelper.Status.Hacked || HackHelper.CheckLicense(levelNode) != HackHelper.License.Yes || (this.mGameSettings.GameType == GameType.Campaign && LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Hacked);
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing && HackHelper.CheckLicense(players[i].Gamer.Avatar) != HackHelper.License.Yes)
				{
					iCustom = true;
					break;
				}
			}
			Game.Instance.AddLoadTask(delegate
			{
				SaveManager.Instance.SaveSettings();
			});
			PlayState iGameState = new PlayState(iCustom, levelNode.FullFileName, this.mGameSettings.GameType, iSpawnPoint, iSaveSlot, this.mVersusSettings);
			GameStateManager.Instance.PushState(iGameState);
		}

		// Token: 0x06002D41 RID: 11585 RVA: 0x0016E84F File Offset: 0x0016CA4F
		internal void ValidateLevels()
		{
			this.mValidatingLevels = true;
			Game.Instance.AddLoadTask(new Action(this.LevelValidation));
		}

		// Token: 0x06002D42 RID: 11586 RVA: 0x0016E870 File Offset: 0x0016CA70
		private void LevelValidation()
		{
			GameType gameType = this.mGameSettings.GameType;
			LevelNode[] array;
			switch (gameType)
			{
			case GameType.Challenge:
				array = LevelManager.Instance.Challenges;
				goto IL_65;
			case (GameType)3:
				break;
			case GameType.Versus:
				array = LevelManager.Instance.Versus;
				goto IL_65;
			default:
				if (gameType == GameType.StoryChallange)
				{
					array = LevelManager.Instance.StoryChallanges;
					goto IL_65;
				}
				break;
			}
			this.mValidatingLevels = false;
			this.mStartButton.Enabled = true;
			return;
			IL_65:
			int num = (this.mGameSettings.Level == -1) ? 0 : this.mGameSettings.Level;
			int num2 = num;
			int i = 0;
			while (i < array.Length)
			{
				HackHelper.License license;
				do
				{
					license = HackHelper.CheckLicense(array[num2]);
					Thread.Sleep(1);
				}
				while (license == HackHelper.License.Pending);
				if (license == HackHelper.License.Yes || (license == HackHelper.License.Custom && this.AllowCustom))
				{
					this.mGameSettings.Level = num2;
					break;
				}
				i++;
				num2 = (num2 + 1) % array.Length;
			}
			this.mValidatingLevels = false;
		}

		// Token: 0x06002D43 RID: 11587 RVA: 0x0016E954 File Offset: 0x0016CB54
		public override void OnEnter()
		{
			this.mLastCountDownNr = -1;
			this.ChangeState(null, SubMenuCharacterSelect.State.Normal);
			ToolTipMan.Instance.KillAll(false);
			this.mOptionsAlpha = 1f;
			this.mLevelSelectAlpha = 0f;
			this.mPackSelectAlpha = 0f;
			Player[] players = Game.Instance.Players;
			NetworkState state = NetworkManager.Instance.State;
			int num = 0;
			int num2 = 0;
			SteamAPI.GameOverlayActivated -= this.SteamOverlayActivated;
			SteamAPI.GameOverlayActivated += this.SteamOverlayActivated;
			for (int i = 0; i < players.Length; i++)
			{
				this.mPlayerSlots[i].ConsecutiveColorChanges = 0;
				if (players[i].Playing && players[i].Gamer != null)
				{
					if (state != NetworkState.Client)
					{
						this.mPlayerSlots[i].AvatarSelected = true;
						this.UpdateControllerIcon(players[i].Controller, i);
						if (this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled)
						{
							if (players[i].Team == Factions.NONE)
							{
								if (num <= num2)
								{
									players[i].Team = Factions.TEAM_RED;
									num++;
								}
								else
								{
									players[i].Team = Factions.TEAM_BLUE;
									num2++;
								}
							}
							else if ((players[i].Team & Factions.TEAM_RED) != Factions.NONE)
							{
								num++;
							}
							else
							{
								num2++;
							}
						}
						else
						{
							players[i].Team = Factions.NONE;
						}
					}
					if (!(players[i].Gamer is NetworkGamer))
					{
						HackHelper.License license = HackHelper.CheckLicense(players[i].Gamer.Avatar);
						if (license != HackHelper.License.Yes && (license != HackHelper.License.Custom || !this.AllowCustom))
						{
							players[i].Gamer.Avatar = Profile.Instance.DefaultAvatar;
							if (state != NetworkState.Offline)
							{
								GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(players[i]);
								NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
							}
						}
					}
				}
				else
				{
					players[i].Team = Factions.NONE;
				}
			}
			NetworkManager.Instance.AbortQuery();
			LanguageManager instance = LanguageManager.Instance;
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				this.mStartButton.Enabled = false;
				GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, instance.GetString(SubMenuCharacterSelect.LOC_JOIN) + "/" + instance.GetString(SubMenuCharacterSelect.LOC_READY));
				GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
				GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Left, "#menu_change_team".GetHashCodeCustom());
				GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Top, SubMenuCharacterSelect.LOC_TT_CUSTOMIZE);
			}
			else
			{
				this.mStartButton.Enabled = true;
				GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, instance.GetString(SubMenuCharacterSelect.LOC_JOIN) + "/" + instance.GetString(SubMenu.LOC_SELECT));
				GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
				GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Left, "#menu_change_team".GetHashCodeCustom());
				GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Top, SubMenuCharacterSelect.LOC_TT_CUSTOMIZE);
			}
			for (int j = 0; j < this.mPlayerSlots.Length; j++)
			{
				this.mPlayerSlots[j].SelectedItem = -1;
				if (this.mPlayerSlots[j].State != SubMenuCharacterSelect.GamerState.Locked)
				{
					this.mPlayerSlots[j].State = SubMenuCharacterSelect.GamerState.Open;
				}
				this.mPlayerSlots[j].Custom = (players[j].Playing && HackHelper.CheckLicense(players[j].Gamer.Avatar) != HackHelper.License.Yes);
			}
			NetworkChat.Instance.Set(448, 185, SubMenu.sPagesTexture, default(Rectangle), FontManager.Instance.GetFont(MagickaFont.Maiandra18), false, 8, true, float.MaxValue);
			NetworkChat.Instance.Active = true;
			this.UpdateGamerDropDownMenu();
			this.HasSelectedLevel = (this.mGameSettings.Level > -1);
			if (this.HasSelectedLevel)
			{
				this.UpdateChapterText(LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level));
			}
			base.OnEnter();
		}

		// Token: 0x06002D44 RID: 11588 RVA: 0x0016ED68 File Offset: 0x0016CF68
		public override void OnExit()
		{
			ToolTipMan.Instance.KillAll(false);
			NetworkChat.Instance.Active = false;
			SteamAPI.GameOverlayActivated -= this.SteamOverlayActivated;
			Game.Instance.AddLoadTask(delegate
			{
				SaveManager.Instance.SaveSettings();
			});
			base.OnExit();
		}

		// Token: 0x06002D45 RID: 11589 RVA: 0x0016EDCC File Offset: 0x0016CFCC
		internal void UpdateGamer(Player iPlayer, Gamer iGamer)
		{
			if (iGamer == Gamer.INVALID_GAMER)
			{
				if (!(iPlayer.Controller is KeyboardMouseController))
				{
					int num = 0;
					while (num < Profile.Instance.Gamers.Count && Profile.Instance.Gamers.Values[num].InUse)
					{
						num++;
					}
					this.mPlayerSlots[iPlayer.ID].SelectedItem = num;
				}
			}
			else if (iGamer != null)
			{
				this.mPlayerSlots[iPlayer.ID].Name.SetText(iGamer.GamerTag);
				this.mPlayerSlots[iPlayer.ID].AvatarSelected = true;
			}
			else
			{
				this.mPlayerSlots[iPlayer.ID].AvatarSelected = false;
			}
			if (!(iGamer is NetworkGamer))
			{
				this.SetReady(false);
			}
		}

		// Token: 0x17000AA6 RID: 2726
		// (get) Token: 0x06002D46 RID: 11590 RVA: 0x0016EEA4 File Offset: 0x0016D0A4
		public bool[] Ready
		{
			get
			{
				bool[] array = new bool[4];
				for (int i = 0; i < 4; i++)
				{
					array[i] = (this.mPlayerSlots[i].State == SubMenuCharacterSelect.GamerState.Ready);
				}
				return array;
			}
		}

		// Token: 0x17000AA7 RID: 2727
		// (get) Token: 0x06002D47 RID: 11591 RVA: 0x0016EEDC File Offset: 0x0016D0DC
		public bool IsTeamsEnabled
		{
			get
			{
				return this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled;
			}
		}

		// Token: 0x06002D48 RID: 11592 RVA: 0x0016EEFC File Offset: 0x0016D0FC
		internal static int CreateVertices(Vector4[] iVertices, int iStartIndex, ref Vector2 iSize, ref Vector2 iMargin, ref Vector2 iUVOffset, ref Vector2 iUVSize, ref Vector2 iUVMargin)
		{
			iVertices[iStartIndex].X = 0f;
			iVertices[iStartIndex].Y = 0f;
			iVertices[iStartIndex].Z = iUVOffset.X;
			iVertices[iStartIndex].W = iUVOffset.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iMargin.X;
			iVertices[iStartIndex].Y = 0f;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVMargin.X;
			iVertices[iStartIndex].W = iUVOffset.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iSize.X - iMargin.X;
			iVertices[iStartIndex].Y = 0f;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].W = iUVOffset.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iSize.X;
			iVertices[iStartIndex].Y = 0f;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X;
			iVertices[iStartIndex].W = iUVOffset.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = 0f;
			iVertices[iStartIndex].Y = iMargin.Y;
			iVertices[iStartIndex].Z = iUVOffset.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iMargin.X;
			iVertices[iStartIndex].Y = iMargin.Y;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVMargin.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iSize.X - iMargin.X;
			iVertices[iStartIndex].Y = iMargin.Y;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iSize.X;
			iVertices[iStartIndex].Y = iMargin.Y;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = 0f;
			iVertices[iStartIndex].Y = iSize.Y - iMargin.Y;
			iVertices[iStartIndex].Z = iUVOffset.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y - iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iMargin.X;
			iVertices[iStartIndex].Y = iSize.Y - iMargin.Y;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVMargin.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y - iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iSize.X - iMargin.X;
			iVertices[iStartIndex].Y = iSize.Y - iMargin.Y;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y - iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iSize.X;
			iVertices[iStartIndex].Y = iSize.Y - iMargin.Y;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y - iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = 0f;
			iVertices[iStartIndex].Y = iSize.Y;
			iVertices[iStartIndex].Z = iUVOffset.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iMargin.X;
			iVertices[iStartIndex].Y = iSize.Y;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVMargin.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iSize.X - iMargin.X;
			iVertices[iStartIndex].Y = iSize.Y;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y;
			iStartIndex++;
			iVertices[iStartIndex].X = iSize.X;
			iVertices[iStartIndex].Y = iSize.Y;
			iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X;
			iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y;
			iStartIndex++;
			return iStartIndex;
		}

		// Token: 0x06002D49 RID: 11593 RVA: 0x0016F52C File Offset: 0x0016D72C
		internal static int CreateVertices(SubMenuCharacterSelect.VertexPositionTextureTexture[] iVertices, int iStartIndex, ref Vector2 iSize, ref Vector2 iMargin, ref Vector2 iUVOffset0, ref Vector2 iUVOffset1, ref Vector2 iUVSize, ref Vector2 iUVMargin)
		{
			iVertices[iStartIndex].Position.X = 0f;
			iVertices[iStartIndex].Position.Y = 0f;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iMargin.X;
			iVertices[iStartIndex].Position.Y = 0f;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVMargin.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVMargin.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iSize.X - iMargin.X;
			iVertices[iStartIndex].Position.Y = 0f;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iSize.X;
			iVertices[iStartIndex].Position.Y = 0f;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = 0f;
			iVertices[iStartIndex].Position.Y = iMargin.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVMargin.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iMargin.X;
			iVertices[iStartIndex].Position.Y = iMargin.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVMargin.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVMargin.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVMargin.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iSize.X - iMargin.X;
			iVertices[iStartIndex].Position.Y = iMargin.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVMargin.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iSize.X;
			iVertices[iStartIndex].Position.Y = iMargin.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVMargin.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = 0f;
			iVertices[iStartIndex].Position.Y = iSize.Y - iMargin.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y - iUVMargin.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y - iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iMargin.X;
			iVertices[iStartIndex].Position.Y = iSize.Y - iMargin.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVMargin.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y - iUVMargin.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVMargin.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y - iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iSize.X - iMargin.X;
			iVertices[iStartIndex].Position.Y = iSize.Y - iMargin.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y - iUVMargin.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y - iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iSize.X;
			iVertices[iStartIndex].Position.Y = iSize.Y - iMargin.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y - iUVMargin.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y - iUVMargin.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = 0f;
			iVertices[iStartIndex].Position.Y = iSize.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iMargin.X;
			iVertices[iStartIndex].Position.Y = iSize.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVMargin.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVMargin.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iSize.X - iMargin.X;
			iVertices[iStartIndex].Position.Y = iSize.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X - iUVMargin.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y;
			iStartIndex++;
			iVertices[iStartIndex].Position.X = iSize.X;
			iVertices[iStartIndex].Position.Y = iSize.Y;
			iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X;
			iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y;
			iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X;
			iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y;
			iStartIndex++;
			return iStartIndex;
		}

		// Token: 0x06002D4A RID: 11594 RVA: 0x0017009A File Offset: 0x0016E29A
		private bool Level_CheckIfLocked(int idx)
		{
			return SubMenuCharacterSelect.mLevelRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mLevelRepresentations.Count - 1 && SubMenuCharacterSelect.mLevelRepresentations[idx].IsLocked;
		}

		// Token: 0x06002D4B RID: 11595 RVA: 0x001700C8 File Offset: 0x0016E2C8
		private bool Level_CheckIfUsed(int idx)
		{
			return SubMenuCharacterSelect.mLevelRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mLevelRepresentations.Count - 1 && SubMenuCharacterSelect.mLevelRepresentations[idx].IsUsed;
		}

		// Token: 0x06002D4C RID: 11596 RVA: 0x001700F6 File Offset: 0x0016E2F6
		private bool Level_CheckIfNew(int idx)
		{
			return SubMenuCharacterSelect.mLevelRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mLevelRepresentations.Count - 1 && SubMenuCharacterSelect.mLevelRepresentations[idx].IsNew;
		}

		// Token: 0x06002D4D RID: 11597 RVA: 0x00170124 File Offset: 0x0016E324
		private bool Level_CheckIfFree(int idx)
		{
			return SubMenuCharacterSelect.mLevelRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mLevelRepresentations.Count - 1 && SubMenuCharacterSelect.mLevelRepresentations[idx].IsFree;
		}

		// Token: 0x06002D4E RID: 11598 RVA: 0x00170154 File Offset: 0x0016E354
		public void SetLevelUsed(string lvlName)
		{
			if (SubMenuCharacterSelect.mLevelRepresentations == null || SubMenuCharacterSelect.mLevelRepresentations.Count == 0 || string.IsNullOrEmpty(lvlName))
			{
				return;
			}
			int num = 0;
			bool flag = false;
			foreach (SubMenuCharacterSelect.LevelRep levelRep in SubMenuCharacterSelect.mLevelRepresentations)
			{
				if (string.Compare(levelRep.Name, lvlName) == 0)
				{
					DLC_StatusHelper.Instance.Item_TrySetUsed("level", levelRep.Name, true);
					flag = true;
					break;
				}
				num++;
			}
			if (flag)
			{
				SubMenuCharacterSelect.mLevelRepresentations[num].IsUsed = true;
			}
		}

		// Token: 0x06002D4F RID: 11599 RVA: 0x00170200 File Offset: 0x0016E400
		private bool Robe_CheckIfLocked(int idx)
		{
			return SubMenuCharacterSelect.mRobeRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mRobeRepresentations.Count - 1 && SubMenuCharacterSelect.mRobeRepresentations[idx].IsLocked;
		}

		// Token: 0x06002D50 RID: 11600 RVA: 0x0017022E File Offset: 0x0016E42E
		private bool Robe_CheckIfUsed(int idx)
		{
			return SubMenuCharacterSelect.mRobeRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mRobeRepresentations.Count - 1 && SubMenuCharacterSelect.mRobeRepresentations[idx].IsUsed;
		}

		// Token: 0x06002D51 RID: 11601 RVA: 0x0017025C File Offset: 0x0016E45C
		private bool Robe_CheckIfFree(int idx)
		{
			return SubMenuCharacterSelect.mRobeRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mRobeRepresentations.Count - 1 && SubMenuCharacterSelect.mRobeRepresentations[idx].IsFree;
		}

		// Token: 0x06002D52 RID: 11602 RVA: 0x0017028A File Offset: 0x0016E48A
		private bool Robe_CheckIfNew(int idx)
		{
			return SubMenuCharacterSelect.mRobeRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mRobeRepresentations.Count - 1 && SubMenuCharacterSelect.mRobeRepresentations[idx].IsNew;
		}

		// Token: 0x06002D53 RID: 11603 RVA: 0x001702B8 File Offset: 0x0016E4B8
		private void SetRobeUsed(string robeName)
		{
			if (SubMenuCharacterSelect.mRobeRepresentations == null || SubMenuCharacterSelect.mRobeRepresentations.Count == 0 || string.IsNullOrEmpty(robeName))
			{
				return;
			}
			if (string.Compare(robeName, "wizard") == 0)
			{
				return;
			}
			int num = 0;
			bool flag = false;
			foreach (SubMenuCharacterSelect.RobeRep robeRep in SubMenuCharacterSelect.mRobeRepresentations)
			{
				if (string.Compare(robeRep.Name, robeName) == 0)
				{
					DLC_StatusHelper.Instance.Item_TrySetUsed("robe", robeRep.Name, true);
					flag = true;
					break;
				}
				num++;
			}
			if (flag)
			{
				SubMenuCharacterSelect.mRobeRepresentations[num].IsUsed = true;
			}
		}

		// Token: 0x040030A8 RID: 12456
		private const float OFFSETX = 1008f;

		// Token: 0x040030A9 RID: 12457
		private const float OFFSETY = 224f;

		// Token: 0x040030AA RID: 12458
		private const float SIZE = 128f;

		// Token: 0x040030AB RID: 12459
		private const float MARGIN = 16f;

		// Token: 0x040030AC RID: 12460
		private const float MAX_GAMERTAG_WIDTH = 190f;

		// Token: 0x040030AD RID: 12461
		private const float LOCKED_ITEM_SATURATION = 0.75f;

		// Token: 0x040030AE RID: 12462
		private const int MAXVISIBLEGAMERS = 4;

		// Token: 0x040030AF RID: 12463
		private const int SELECTION_PLAYER0 = 0;

		// Token: 0x040030B0 RID: 12464
		private const int SELECTION_PLAYER1 = 1;

		// Token: 0x040030B1 RID: 12465
		private const int SELECTION_PLAYER2 = 2;

		// Token: 0x040030B2 RID: 12466
		private const int SELECTION_PLAYER3 = 3;

		// Token: 0x040030B3 RID: 12467
		private const int SELECTION_START = 4;

		// Token: 0x040030B4 RID: 12468
		private const int SELECTION_PACKS = 5;

		// Token: 0x040030B5 RID: 12469
		private const int SELECTION_LEVEL = 6;

		// Token: 0x040030B6 RID: 12470
		private const int SELECTION_CHAT = -1;

		// Token: 0x040030B7 RID: 12471
		private const int SELECTION_GAME_MODE = 7;

		// Token: 0x040030B8 RID: 12472
		private const int SELECTION_VS_SETTINGS = 8;

		// Token: 0x040030B9 RID: 12473
		private const int SELECTION_CHAPTER = 9;

		// Token: 0x040030BA RID: 12474
		private const int MAX_PACK_ROWS = 7;

		// Token: 0x040030BB RID: 12475
		private const float PACK_THUMB_SPACING = 64f;

		// Token: 0x040030BC RID: 12476
		private const int MAX_VISIBLE_PACKS = 5;

		// Token: 0x040030BD RID: 12477
		private const float PACK_POS_X = 64f;

		// Token: 0x040030BE RID: 12478
		private const float PACK_POS_Y = 408f;

		// Token: 0x040030BF RID: 12479
		private const int MAX_VISIBLE_LEVELS = 6;

		// Token: 0x040030C0 RID: 12480
		private const float LEVEL_SPACING = 140f;

		// Token: 0x040030C1 RID: 12481
		private const int LEVEL_POS_Y = 64;

		// Token: 0x040030C2 RID: 12482
		private const int LEVEL_POS_X = 64;

		// Token: 0x040030C3 RID: 12483
		private const float LEVEL_LOWEST_POSY_Y = 904f;

		// Token: 0x040030C4 RID: 12484
		private const float AVATAR_WIDTH = 96f;

		// Token: 0x040030C5 RID: 12485
		private const float COLOR_SPACING = 34f;

		// Token: 0x040030C6 RID: 12486
		private const int COLUMN_WIDTH = 448;

		// Token: 0x040030C7 RID: 12487
		private const int LEVEL_NAME_TARGET_LINE_WIDTH = 560;

		// Token: 0x040030C8 RID: 12488
		private const int LEVEL_DESC_TARGET_LINE_WIDTH = 940;

		// Token: 0x040030C9 RID: 12489
		private const int LEVEL_DESC_TARGET_LINE_WIDTH_CHAPTER = 560;

		// Token: 0x040030CA RID: 12490
		private const float SLOT_HEIGHT = 112f;

		// Token: 0x040030CB RID: 12491
		private const float SLOT_SPACING = 25f;

		// Token: 0x040030CC RID: 12492
		private const float THUMB_SLOT_SIZE = 96f;

		// Token: 0x040030CD RID: 12493
		private const float SLOT_POS_X = 544f;

		// Token: 0x040030CE RID: 12494
		private const float PLAYER_SLOT_POS_Y = 89f;

		// Token: 0x040030CF RID: 12495
		private const float SPECTATOR_SLOT_POS_Y = 593f;

		// Token: 0x040030D0 RID: 12496
		private const float GAMER_SPACING = 31f;

		// Token: 0x040030D1 RID: 12497
		private const float CHAT_POS_X = 544f;

		// Token: 0x040030D2 RID: 12498
		private const float CHAT_POS_Y = 710f;

		// Token: 0x040030D3 RID: 12499
		private const int CHAT_SIZE_X = 448;

		// Token: 0x040030D4 RID: 12500
		private const int CHAT_SIZE_Y = 185;

		// Token: 0x040030D5 RID: 12501
		private const float READY_OFFSET_X = 48f;

		// Token: 0x040030D6 RID: 12502
		private const float READY_WIDTH = 128f;

		// Token: 0x040030D7 RID: 12503
		private const float READY_HEIGHT = 80f;

		// Token: 0x040030D8 RID: 12504
		private const float CONTROLLER_ICON_WIDTH = 128f;

		// Token: 0x040030D9 RID: 12505
		private const float CONTROLLER_ICON_HEIGHT = 64f;

		// Token: 0x040030DA RID: 12506
		private const float CONTROLLER_ICON_OFFSET_X = -108.8f;

		// Token: 0x040030DB RID: 12507
		private const float CONTROLLER_ICON_OFFSET_Y = -54.4f;

		// Token: 0x040030DC RID: 12508
		private const int MAX_VISIBLE_SETTINGS = 5;

		// Token: 0x040030DD RID: 12509
		protected static readonly string[] dungeonLevelFileNames = new string[]
		{
			"ch_dungeons_ch1",
			"ch_dungeons_ch2"
		};

		// Token: 0x040030DE RID: 12510
		protected static readonly int LOC_LOADING = "#network_23".GetHashCodeCustom();

		// Token: 0x040030DF RID: 12511
		protected static readonly int LOC_SELECT_CHARACTER = "#menu_charslct_01".GetHashCodeCustom();

		// Token: 0x040030E0 RID: 12512
		protected static readonly int LOC_READY = "#menu_charslct_02".GetHashCodeCustom();

		// Token: 0x040030E1 RID: 12513
		protected static readonly int LOC_CHANGE_COLOR = "#menu_charslct_03".GetHashCodeCustom();

		// Token: 0x040030E2 RID: 12514
		protected static readonly int LOC_JOIN = "#add_menu_join".GetHashCodeCustom();

		// Token: 0x040030E3 RID: 12515
		protected static readonly int LOC_NEW = "#add_menu_prof_new".GetHashCodeCustom();

		// Token: 0x040030E4 RID: 12516
		protected static readonly int LOC_LEVEL = "#menu_vs_13".GetHashCodeCustom();

		// Token: 0x040030E5 RID: 12517
		protected static readonly int LOC_GAMEMODE = "#opt_vs_mode".GetHashCodeCustom();

		// Token: 0x040030E6 RID: 12518
		protected static readonly int LOC_PACKS = "#menu_enabledpacks".GetHashCodeCustom();

		// Token: 0x040030E7 RID: 12519
		protected static readonly int LOC_MAGICKPACKS = "#menu_tome_02".GetHashCodeCustom();

		// Token: 0x040030E8 RID: 12520
		protected static readonly int LOC_ITEMPACKS = "#menu_tome_03".GetHashCodeCustom();

		// Token: 0x040030E9 RID: 12521
		protected static readonly int LOC_LATENCY = "#menu_latency".GetHashCodeCustom();

		// Token: 0x040030EA RID: 12522
		protected new static readonly int LOC_ENTER_NAME = "#add_menu_prof_name".GetHashCodeCustom();

		// Token: 0x040030EB RID: 12523
		protected static readonly int LOC_COUNTDOWN = "#menu_countdown".GetHashCodeCustom();

		// Token: 0x040030EC RID: 12524
		protected static readonly int LOC_COUNTDOWN_ABORTED = "#menu_countdown_abort".GetHashCodeCustom();

		// Token: 0x040030ED RID: 12525
		protected static readonly int LOC_TT_CHANGE_LEVEL = "#tooltip_lobby_level".GetHashCodeCustom();

		// Token: 0x040030EE RID: 12526
		protected static readonly int LOC_TT_CUSTOMIZE = "#tooltip_lobby_customize".GetHashCodeCustom();

		// Token: 0x040030EF RID: 12527
		protected static readonly int LOC_TT_GAME_MODE = "#tooltip_vs_mode".GetHashCodeCustom();

		// Token: 0x040030F0 RID: 12528
		protected static readonly int LOC_KICK = "#network_06".GetHashCodeCustom();

		// Token: 0x040030F1 RID: 12529
		protected static readonly int LOC_KICKED = "#add_menu_not_kicked".GetHashCodeCustom();

		// Token: 0x040030F2 RID: 12530
		protected static readonly int LOC_CHATMESSAGE = "#menu_chat_message".GetHashCodeCustom();

		// Token: 0x040030F3 RID: 12531
		protected static readonly string sServerChangingLevelText = LanguageManager.Instance.GetString(SubMenuOnline.LOC_NO_LEVEL_SELECTED);

		// Token: 0x040030F4 RID: 12532
		private static SubMenuCharacterSelect sSingelton;

		// Token: 0x040030F5 RID: 12533
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040030F6 RID: 12534
		private static Vector4 MENU_COLOR_LIGHT_GRAY = new Vector4(1f, 1f, 1f, 1f);

		// Token: 0x040030F7 RID: 12535
		private SubMenuCharacterSelect.State mCurrentState;

		// Token: 0x040030F8 RID: 12536
		private float mOptionsAlpha = 1f;

		// Token: 0x040030F9 RID: 12537
		private float mLevelSelectAlpha;

		// Token: 0x040030FA RID: 12538
		private float mPackSelectAlpha;

		// Token: 0x040030FB RID: 12539
		private bool HasSelectedLevel = true;

		// Token: 0x040030FC RID: 12540
		private GameInfoMessage mGameSettings;

		// Token: 0x040030FD RID: 12541
		private bool mCustomLevel;

		// Token: 0x040030FE RID: 12542
		private Texture2D mCustomLevelOverlay;

		// Token: 0x040030FF RID: 12543
		private static readonly Vector4 LEVEL_DESCR_COLOR_NONCAMP = new Vector4(0f, 0f, 0f, 1f);

		// Token: 0x04003100 RID: 12544
		private Texture2D mLevelLockedOverlay;

		// Token: 0x04003101 RID: 12545
		private Texture2D mLevelUnusedOverlay;

		// Token: 0x04003102 RID: 12546
		private Texture2D mRobeLockedOverlay;

		// Token: 0x04003103 RID: 12547
		private Texture2D mRobeUnusedOverlay;

		// Token: 0x04003104 RID: 12548
		private Texture2D mRobeFreeOverlay;

		// Token: 0x04003105 RID: 12549
		private Texture2D mRobeFreeAndLockedOverlay;

		// Token: 0x04003106 RID: 12550
		private Texture2D mRobeFreeAndUnusedOverlay;

		// Token: 0x04003107 RID: 12551
		private Texture2D mRobeNewOverlay;

		// Token: 0x04003108 RID: 12552
		private Texture2D mLevelFreeOverlay;

		// Token: 0x04003109 RID: 12553
		private Texture2D mLevelFreeAndLockedOverlay;

		// Token: 0x0400310A RID: 12554
		private Texture2D mLevelFreeAndUnusedOverlay;

		// Token: 0x0400310B RID: 12555
		private Texture2D mLevelNewOverlay;

		// Token: 0x0400310C RID: 12556
		private List<Text> mGamerItems;

		// Token: 0x0400310D RID: 12557
		private MenuImageTextItem mBackButton;

		// Token: 0x0400310E RID: 12558
		private MenuTextButtonItem mStartButton;

		// Token: 0x0400310F RID: 12559
		private MenuImageTextItem mGenericStar;

		// Token: 0x04003110 RID: 12560
		protected static Texture2D sSelectLevelButtonFrame;

		// Token: 0x04003111 RID: 12561
		protected static Texture2D sStarTexture;

		// Token: 0x04003112 RID: 12562
		private MenuTextButtonItem mSelectLevelButton;

		// Token: 0x04003113 RID: 12563
		private BitmapFont mFont;

		// Token: 0x04003114 RID: 12564
		private BitmapFont mGamerFont;

		// Token: 0x04003115 RID: 12565
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04003116 RID: 12566
		private VertexBuffer mVertexBuffer;

		// Token: 0x04003117 RID: 12567
		private IndexBuffer mIndexBuffer;

		// Token: 0x04003118 RID: 12568
		private ContextMenu mGamerDropDownMenu;

		// Token: 0x04003119 RID: 12569
		private ContextMenu mAdminDropDownMenu;

		// Token: 0x0400311A RID: 12570
		private MenuScrollBar mSpecialScrollBar;

		// Token: 0x0400311B RID: 12571
		private Controller mCurrentController;

		// Token: 0x0400311C RID: 12572
		private Text mChapterName;

		// Token: 0x0400311D RID: 12573
		private Text mChapterDescription;

		// Token: 0x0400311E RID: 12574
		private Texture2D mMapTexture;

		// Token: 0x0400311F RID: 12575
		private Texture2D mMapMaskTexture;

		// Token: 0x04003120 RID: 12576
		private Rectangle mMapRect;

		// Token: 0x04003121 RID: 12577
		private Text mItemsText;

		// Token: 0x04003122 RID: 12578
		private Text mMagicksText;

		// Token: 0x04003123 RID: 12579
		private Text mPacksText;

		// Token: 0x04003124 RID: 12580
		private int mSelectedPack = -1;

		// Token: 0x04003125 RID: 12581
		private ControllerDirection mSelectedPackScroll;

		// Token: 0x04003126 RID: 12582
		private int mPackScrollValue;

		// Token: 0x04003127 RID: 12583
		private Texture2D mMagicksTexture;

		// Token: 0x04003128 RID: 12584
		private static Texture2D[] mControllerTextures;

		// Token: 0x04003129 RID: 12585
		public static readonly Vector2 SELECT_LEVEL_BUTTON_POS = new Vector2(129f, 560f);

		// Token: 0x0400312A RID: 12586
		private ContentManager mLevelContent;

		// Token: 0x0400312B RID: 12587
		private Text mLoadingText;

		// Token: 0x0400312C RID: 12588
		private bool mValidatingLevels;

		// Token: 0x0400312D RID: 12589
		private bool mLoadingLevels;

		// Token: 0x0400312E RID: 12590
		private Controller mNameInputController;

		// Token: 0x0400312F RID: 12591
		private TextInputMessageBox mNameInputBox;

		// Token: 0x04003130 RID: 12592
		private VertexBuffer mAvatarVertices;

		// Token: 0x04003131 RID: 12593
		private VertexDeclaration mAvatarVertexDeclaration;

		// Token: 0x04003132 RID: 12594
		private Texture2D mCustomTexture;

		// Token: 0x04003133 RID: 12595
		private bool mDefaultAvatars;

		// Token: 0x04003134 RID: 12596
		private MenuScrollBar[] mGamerScrollBars;

		// Token: 0x04003135 RID: 12597
		private Text mOpenText;

		// Token: 0x04003136 RID: 12598
		private Text mClosedText;

		// Token: 0x04003137 RID: 12599
		private SubMenuCharacterSelect.PlayerState[] mPlayerSlots;

		// Token: 0x04003138 RID: 12600
		private MenuTextButtonItem mPackCloseButton;

		// Token: 0x04003139 RID: 12601
		private DropDownBox<Rulesets> mGameModeBox;

		// Token: 0x0400313A RID: 12602
		private Text mGameModeTitle;

		// Token: 0x0400313B RID: 12603
		private VersusRuleset.Settings mVersusSettings;

		// Token: 0x0400313C RID: 12604
		private Text[] mVersusSettingsTitles;

		// Token: 0x0400313D RID: 12605
		private MenuScrollBar mSettingsScrollbar;

		// Token: 0x0400313E RID: 12606
		private OptionsMessageBox mCheckPointRUSure;

		// Token: 0x0400313F RID: 12607
		private int mLastCountDownNr = -1;

		// Token: 0x04003140 RID: 12608
		private float mCountDown;

		// Token: 0x04003141 RID: 12609
		private int mLevelToSet;

		// Token: 0x04003142 RID: 12610
		private string mLevelNameToFocusWhenLevelComplete = "";

		// Token: 0x04003143 RID: 12611
		private static List<SubMenuCharacterSelect.LevelRep> mLevelRepresentations = null;

		// Token: 0x04003144 RID: 12612
		private static List<SubMenuCharacterSelect.RobeRep> mRobeRepresentations = null;

		// Token: 0x020005E1 RID: 1505
		internal struct VertexPositionTextureTexture
		{
			// Token: 0x04003147 RID: 12615
			public const int SIZE = 24;

			// Token: 0x04003148 RID: 12616
			public Vector2 Position;

			// Token: 0x04003149 RID: 12617
			public Vector2 TexCoord0;

			// Token: 0x0400314A RID: 12618
			public Vector2 TexCoord1;

			// Token: 0x0400314B RID: 12619
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(0, 16, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1)
			};
		}

		// Token: 0x020005E2 RID: 1506
		private enum GamerState
		{
			// Token: 0x0400314D RID: 12621
			Locked = -1,
			// Token: 0x0400314E RID: 12622
			Open,
			// Token: 0x0400314F RID: 12623
			CustomizingAvatar,
			// Token: 0x04003150 RID: 12624
			CustomizingColor,
			// Token: 0x04003151 RID: 12625
			Ready
		}

		// Token: 0x020005E3 RID: 1507
		private struct PlayerState
		{
			// Token: 0x06002D58 RID: 11608 RVA: 0x001705B8 File Offset: 0x0016E7B8
			public void SetLatency(int iLatencyMS)
			{
				this.Latency = iLatencyMS;
				string text = LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LATENCY);
				text = text.Replace("#1;", iLatencyMS.ToString());
				if (iLatencyMS < 100)
				{
					this.LatencyText.DefaultColor = new Vector4(0f, 1f, 0f, 1f);
				}
				else if (iLatencyMS < 200)
				{
					this.LatencyText.DefaultColor = new Vector4(1f, 1f, 0f, 1f);
				}
				else
				{
					this.LatencyText.DefaultColor = new Vector4(1f, 0f, 0f, 1f);
				}
				this.LatencyText.SetText(text);
			}

			// Token: 0x04003152 RID: 12626
			public SubMenuCharacterSelect.GamerState State;

			// Token: 0x04003153 RID: 12627
			public Text Name;

			// Token: 0x04003154 RID: 12628
			public int Latency;

			// Token: 0x04003155 RID: 12629
			public Text LatencyText;

			// Token: 0x04003156 RID: 12630
			public int Team;

			// Token: 0x04003157 RID: 12631
			public int SelectedItem;

			// Token: 0x04003158 RID: 12632
			public int ScrollValue;

			// Token: 0x04003159 RID: 12633
			public bool Custom;

			// Token: 0x0400315A RID: 12634
			public int ConsecutiveColorChanges;

			// Token: 0x0400315B RID: 12635
			public bool AvatarSelected;

			// Token: 0x0400315C RID: 12636
			public int ControllerType;
		}

		// Token: 0x020005E4 RID: 1508
		private enum State
		{
			// Token: 0x0400315E RID: 12638
			Normal,
			// Token: 0x0400315F RID: 12639
			ChangingLevel,
			// Token: 0x04003160 RID: 12640
			ChangingPacks,
			// Token: 0x04003161 RID: 12641
			CountDown
		}

		// Token: 0x020005E5 RID: 1509
		private class SelectableObjectRep
		{
			// Token: 0x17000AA8 RID: 2728
			// (get) Token: 0x06002D59 RID: 11609 RVA: 0x00170678 File Offset: 0x0016E878
			// (set) Token: 0x06002D5A RID: 11610 RVA: 0x00170680 File Offset: 0x0016E880
			public int OriginalIndex
			{
				get
				{
					return this.originalIndex;
				}
				set
				{
					this.originalIndex = value;
				}
			}

			// Token: 0x17000AA9 RID: 2729
			// (get) Token: 0x06002D5B RID: 11611 RVA: 0x0017068C File Offset: 0x0016E88C
			public int SortVal
			{
				get
				{
					if (this.IsCustom)
					{
						return -5;
					}
					if (this.IsNew && this.IsLocked)
					{
						if (this.IsFree)
						{
							return -3;
						}
						return -4;
					}
					else
					{
						if (this.IsFree && !this.IsUsed)
						{
							return -2;
						}
						if (!this.IsLocked && !this.IsUsed)
						{
							return -2;
						}
						if (this.IsFree)
						{
							return -1;
						}
						if (this.IsUsed && !this.IsLocked && !this.IsFree)
						{
							if (this.IsNew)
							{
								return -1;
							}
							return 0;
						}
						else
						{
							if (this.IsLocked && this.IsFree)
							{
								return 1;
							}
							if (this.IsLocked)
							{
								return 2;
							}
							return 3;
						}
					}
				}
			}

			// Token: 0x06002D5C RID: 11612 RVA: 0x00170734 File Offset: 0x0016E934
			public static int CompareRep(SubMenuCharacterSelect.SelectableObjectRep A, SubMenuCharacterSelect.SelectableObjectRep B)
			{
				if (A == null)
				{
					if (B == null)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (B == null)
					{
						return 1;
					}
					int sortVal = A.SortVal;
					int sortVal2 = B.SortVal;
					if (sortVal < sortVal2)
					{
						return -1;
					}
					if (sortVal != sortVal2)
					{
						return 1;
					}
					return 0;
				}
			}

			// Token: 0x06002D5D RID: 11613 RVA: 0x0017076C File Offset: 0x0016E96C
			public override string ToString()
			{
				return string.Format("Index = {0}, Name = \"{1}\", BelongsToAppID = {2}, {3}, {4}, {5}, {6}", new object[]
				{
					this.OriginalIndex,
					this.Name,
					this.BelongsToAppID,
					this.IsFree ? "FREE" : "NOT FREE",
					this.IsLocked ? "LOCKED" : "UNLOCKED",
					this.IsUsed ? "USED" : "UNUSED",
					this.IsCustom ? "CUSTOM" : "NOT CUSTOM",
					this.IsNew ? "NEW" : "NOT NEW"
				});
			}

			// Token: 0x04003162 RID: 12642
			public string Name = "UNDEFINED";

			// Token: 0x04003163 RID: 12643
			public bool IsLocked = true;

			// Token: 0x04003164 RID: 12644
			public bool IsUsed;

			// Token: 0x04003165 RID: 12645
			public bool IsCustom;

			// Token: 0x04003166 RID: 12646
			public bool IsFree;

			// Token: 0x04003167 RID: 12647
			public bool IsNew;

			// Token: 0x04003168 RID: 12648
			public uint BelongsToAppID;

			// Token: 0x04003169 RID: 12649
			public byte[] HashSum;

			// Token: 0x0400316A RID: 12650
			protected int originalIndex = -1;
		}

		// Token: 0x020005E6 RID: 1510
		private sealed class LevelRep : SubMenuCharacterSelect.SelectableObjectRep
		{
			// Token: 0x0400316B RID: 12651
			public Text Title;

			// Token: 0x0400316C RID: 12652
			public Text Descr;

			// Token: 0x0400316D RID: 12653
			public Texture2D PreviewImage;

			// Token: 0x0400316E RID: 12654
			public string FileName;
		}

		// Token: 0x020005E7 RID: 1511
		private sealed class RobeRep : SubMenuCharacterSelect.SelectableObjectRep
		{
			// Token: 0x0400316F RID: 12655
			public int DisplayName;

			// Token: 0x04003170 RID: 12656
			public int Description;
		}
	}
}
