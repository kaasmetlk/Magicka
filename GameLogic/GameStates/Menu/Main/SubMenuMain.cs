// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuMain
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

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
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuMain : SubMenu
{
  private const float NEW_UV_X = 972f;
  private const float NEW_UV_Y = 974f;
  private const float CAMPAIGN_UV_X = 0.0f;
  private const float CAMPAIGN_UV_Y = 0.0f;
  private const float CAMPAIGN_UV_WIDTH = 388f;
  private const float CAMPAIGN_UV_HEIGHT = 226f;
  private const float MYTHOS_UV_X = 390f;
  private const float MYTHOS_UV_Y = 0.0f;
  private const float MYTHOS_UV_WIDTH = 388f;
  private const float MYTHOS_UV_HEIGHT = 226f;
  private const float ROW_BOTTOM_PADDING = 12f;
  private const float SECOND_ROW_RIGHT_PADDING_AB = 46f;
  private const float SECOND_ROW_RIGHT_PADDING_BC = 27f;
  private const float SECOND_ROW_RIGHT_PADDING_CD = 26f;
  private const float VIETNAM_UV_X = 0.0f;
  private const float VIETNAM_UV_Y = 230f;
  private const float VIETNAM_UV_WIDTH = 165f;
  private const float VIETNAM_UV_HEIGHT = 177f;
  private const float OSOTC_UV_X = 166f;
  private const float OSOTC_UV_Y = 230f;
  private const float OSOTC_UV_WIDTH = 165f;
  private const float OSOTC_UV_HEIGHT = 177f;
  private const float DUNGEONS1_UV_X = 332f;
  private const float DUNGEONS1_UV_Y = 230f;
  private const float DUNGEONS1_UV_WIDTH = 165f;
  private const float DUNGEONS1_UV_HEIGHT = 177f;
  private const float DUNGEONS2_UV_X = 498f;
  private const float DUNGEONS2_UV_Y = 230f;
  private const float DUNGEONS2_UV_WIDTH = 165f;
  private const float DUNGEONS2_UV_HEIGHT = 177f;
  private const float LEADERBOARD_UV_X = 1f;
  private const float LEADERBOARD_UV_Y = 952f;
  private const float LEADERBOARD_UV_WIDTH = 71f;
  private const float LEADERBOARD_UV_HEIGHT = 71f;
  private const float OPTIONS_UV_X = 73f;
  private const float OPTIONS_UV_Y = 952f;
  private const float OPTIONS_UV_WIDTH = 71f;
  private const float OPTIONS_UV_HEIGHT = 71f;
  private const uint APPID_VIETNAM = 42918;
  private const string LEVELNAME_VIETNAM = "#challenge_vietnam";
  private const uint APPID_OSOTC = 73093;
  private const string LEVELNAME_OSOTC = "#challenge_osotc";
  private const uint APPID_DUNG1 = 73115;
  private const string LEVELNAME_DUNGEONS1 = "#challenge_dungeons_chapter1";
  private const uint APPID_DUNG2 = 0;
  private const string LEVELNAME_DUNGEONS2 = "#challenge_dungeons_chapter2";
  private const uint APPID_MYTHOS = 73058;
  private static SubMenuMain mSingelton;
  private static volatile object mSingeltonLock = new object();
  private static readonly BitmapFont mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
  private static readonly int LOC_RU_SURE = "#add_menu_rus_quit".GetHashCodeCustom();
  private static readonly float HALF_LINEHEIGHT = (float) SubMenuMain.mFont.LineHeight * 0.5f;
  private static readonly Vector2 LOCK_SIZE = new Vector2(112f, 160f);
  private static readonly Vector2 NEW_SIZE = new Vector2(52f, 50f);
  private static readonly Vector2 CAMPAIGN_POSITION = new Vector2(166f, 54f);
  private static readonly Vector2 CAMPAIGN_SIZE = new Vector2(388f, 226f);
  private static readonly Vector2 CAMPAIGN_TEXT = new Vector2((float) ((double) SubMenuMain.CAMPAIGN_SIZE.X * 0.5 - 2.0), (float) ((double) SubMenuMain.CAMPAIGN_SIZE.Y - (double) SubMenuMain.HALF_LINEHEIGHT + 5.0));
  private static readonly Vector2 MYTHOS_POSITION = new Vector2((float) ((double) SubMenuMain.CAMPAIGN_POSITION.X + (double) SubMenuMain.CAMPAIGN_SIZE.X + 23.0), SubMenuMain.CAMPAIGN_POSITION.Y);
  private static readonly Vector2 MYTHOS_SIZE = SubMenuMain.CAMPAIGN_SIZE;
  private static readonly Vector2 MYTHOS_TEXT = new Vector2(SubMenuMain.MYTHOS_SIZE.X * 0.8f, (float) ((double) SubMenuMain.MYTHOS_SIZE.Y - (double) SubMenuMain.HALF_LINEHEIGHT + 5.0));
  private static readonly Vector2 MYTHOS_LOCK_POSITION = new Vector2((float) ((double) SubMenuMain.MYTHOS_POSITION.X + (double) SubMenuMain.MYTHOS_SIZE.X * 0.5 - (double) SubMenuMain.LOCK_SIZE.X * 0.5), (float) ((double) SubMenuMain.MYTHOS_POSITION.Y + (double) SubMenuMain.MYTHOS_SIZE.Y * 0.5 - (double) SubMenuMain.LOCK_SIZE.Y * 0.5 - 8.0));
  private static readonly Vector2 MYTHOS_NEW_POSITION = new Vector2((float) ((double) SubMenuMain.MYTHOS_POSITION.X + (double) SubMenuMain.MYTHOS_SIZE.X - ((double) SubMenuMain.NEW_SIZE.X + 34.0)), SubMenuMain.MYTHOS_POSITION.Y + 8f);
  private static readonly float SECOND_ROW_ITEM_HEIGHT = 191f;
  private static readonly float SECOND_ROW_Y = (float) ((double) SubMenuMain.CAMPAIGN_POSITION.Y + (double) SubMenuMain.CAMPAIGN_SIZE.Y + 12.0);
  private static readonly Vector2 VIETNAM_POSITION = new Vector2(SubMenuMain.CAMPAIGN_POSITION.X + 7f, SubMenuMain.SECOND_ROW_Y);
  private static readonly Vector2 VIETNAM_SIZE = new Vector2(165f, 177f);
  private static readonly Vector2 VIETNAM_LOCK_POSITION = new Vector2((float) ((double) SubMenuMain.VIETNAM_POSITION.X + (double) SubMenuMain.VIETNAM_SIZE.X * 0.5 - (double) SubMenuMain.LOCK_SIZE.X * 0.5), (float) ((double) SubMenuMain.VIETNAM_POSITION.Y + (double) SubMenuMain.VIETNAM_SIZE.Y * 0.5 - (double) SubMenuMain.LOCK_SIZE.Y * 0.5));
  private static readonly Vector2 VIETNAM_NEW_POSITION = new Vector2((float) ((double) SubMenuMain.VIETNAM_POSITION.X + (double) SubMenuMain.VIETNAM_SIZE.X - ((double) SubMenuMain.NEW_SIZE.X + 10.0)), SubMenuMain.VIETNAM_POSITION.Y + 8f);
  private static readonly Vector2 OSOTC_POSITION = new Vector2((float) ((double) SubMenuMain.VIETNAM_POSITION.X + (double) SubMenuMain.VIETNAM_SIZE.X + 46.0), SubMenuMain.SECOND_ROW_Y);
  private static readonly Vector2 OSOTC_SIZE = new Vector2(165f, 177f);
  private static readonly Vector2 OSOTC_LOCK_POSITION = new Vector2((float) ((double) SubMenuMain.OSOTC_POSITION.X + (double) SubMenuMain.OSOTC_SIZE.X * 0.5 - (double) SubMenuMain.LOCK_SIZE.X * 0.5), (float) ((double) SubMenuMain.OSOTC_POSITION.Y + (double) SubMenuMain.OSOTC_SIZE.Y * 0.5 - (double) SubMenuMain.LOCK_SIZE.Y * 0.5));
  private static readonly Vector2 OSOTC_NEW_POSITION = new Vector2((float) ((double) SubMenuMain.OSOTC_POSITION.X + (double) SubMenuMain.OSOTC_SIZE.X - ((double) SubMenuMain.NEW_SIZE.X + 10.0)), SubMenuMain.OSOTC_POSITION.Y + 8f);
  private static readonly Vector2 DUNGEONS1_POSITION = new Vector2((float) ((double) SubMenuMain.OSOTC_POSITION.X + (double) SubMenuMain.OSOTC_SIZE.X + 27.0), SubMenuMain.SECOND_ROW_Y);
  private static readonly Vector2 DUNGEONS1_SIZE = new Vector2(165f, 177f);
  private static readonly Vector2 DUNGEONS1_LOCK_POSITION = new Vector2((float) ((double) SubMenuMain.DUNGEONS1_POSITION.X + (double) SubMenuMain.DUNGEONS1_SIZE.X * 0.5 - (double) SubMenuMain.LOCK_SIZE.X * 0.5), (float) ((double) SubMenuMain.DUNGEONS1_POSITION.Y + (double) SubMenuMain.DUNGEONS1_SIZE.Y * 0.5 - (double) SubMenuMain.LOCK_SIZE.Y * 0.5));
  private static readonly Vector2 DUNGEONS1_NEW_POSITION = new Vector2((float) ((double) SubMenuMain.DUNGEONS1_POSITION.X + (double) SubMenuMain.DUNGEONS1_SIZE.X - ((double) SubMenuMain.NEW_SIZE.X + 10.0)), SubMenuMain.DUNGEONS1_POSITION.Y + 8f);
  private static readonly Vector2 DUNGEONS2_POSITION = new Vector2((float) ((double) SubMenuMain.DUNGEONS1_POSITION.X + (double) SubMenuMain.DUNGEONS1_SIZE.X + 26.0), SubMenuMain.SECOND_ROW_Y);
  private static readonly Vector2 DUNGEONS2_SIZE = new Vector2(165f, 177f);
  private static readonly Vector2 DUNGEONS2_LOCK_POSITION = new Vector2((float) ((double) SubMenuMain.DUNGEONS2_POSITION.X + (double) SubMenuMain.DUNGEONS2_SIZE.X * 0.5 - (double) SubMenuMain.LOCK_SIZE.X * 0.5), (float) ((double) SubMenuMain.DUNGEONS2_POSITION.Y + (double) SubMenuMain.DUNGEONS2_SIZE.Y * 0.5 - (double) SubMenuMain.LOCK_SIZE.Y * 0.5));
  private static readonly Vector2 DUNGEONS2_NEW_POSITION = new Vector2((float) ((double) SubMenuMain.DUNGEONS2_POSITION.X + (double) SubMenuMain.DUNGEONS2_SIZE.X - ((double) SubMenuMain.NEW_SIZE.X + 10.0)), SubMenuMain.DUNGEONS2_POSITION.Y + 8f);
  private static readonly float THIRD_ROW_Y = (float) ((double) SubMenuMain.SECOND_ROW_Y + (double) SubMenuMain.SECOND_ROW_ITEM_HEIGHT + 12.0);
  private static readonly Vector2 CHALLENGE_POSITION = new Vector2(SubMenuMain.CAMPAIGN_POSITION.X - 3f, SubMenuMain.THIRD_ROW_Y);
  private static readonly Vector2 CHALLENGE_SIZE = new Vector2(336f, 400f);
  private static readonly Vector2 CHALLENGE_TEXT = new Vector2(SubMenuMain.CHALLENGE_SIZE.X * 0.5f, SubMenuMain.CHALLENGE_SIZE.Y - (float) SubMenuMain.mFont.LineHeight * 0.5f);
  private static readonly Vector2 CHALLANGE_NEW_POSITION = new Vector2((float) ((double) SubMenuMain.CHALLENGE_POSITION.X + (double) SubMenuMain.CHALLENGE_SIZE.X - ((double) SubMenuMain.NEW_SIZE.X + 70.0)), SubMenuMain.CHALLENGE_POSITION.Y + 25f);
  private static readonly Vector2 VERSUS_POSITION = new Vector2((float) ((double) SubMenuMain.CHALLENGE_POSITION.X + (double) SubMenuMain.CHALLENGE_SIZE.X - 112.0), SubMenuMain.THIRD_ROW_Y);
  private static readonly Vector2 VERSUS_SIZE = SubMenuMain.CHALLENGE_SIZE;
  private static readonly Vector2 VERSUS_TEXT = new Vector2(SubMenuMain.VERSUS_SIZE.X * 0.5f, SubMenuMain.VERSUS_SIZE.Y - SubMenuMain.HALF_LINEHEIGHT);
  private static readonly Vector2 VERSUS_NEW_POSITION = new Vector2((float) ((double) SubMenuMain.VERSUS_POSITION.X + (double) SubMenuMain.VERSUS_SIZE.X - ((double) SubMenuMain.NEW_SIZE.X + 35.0)), SubMenuMain.VERSUS_POSITION.Y + 25f);
  private static readonly Vector2 ONLINE_POSITION = new Vector2((float) ((double) SubMenuMain.CHALLENGE_POSITION.X + (double) SubMenuMain.CHALLENGE_SIZE.X + (double) SubMenuMain.VERSUS_SIZE.X - 224.0), SubMenuMain.THIRD_ROW_Y);
  private static readonly Vector2 ONLINE_SIZE = SubMenuMain.CHALLENGE_SIZE;
  private static readonly Vector2 ONLINE_TEXT = new Vector2(SubMenuMain.ONLINE_SIZE.X * 0.5f, SubMenuMain.ONLINE_SIZE.Y - SubMenuMain.HALF_LINEHEIGHT);
  private static readonly Vector2 LEADERBOARD_SIZE = new Vector2(71f, 71f);
  private static readonly Vector2 OPTIONS_SIZE = new Vector2(71f, 71f);
  private static readonly Vector2 LEADERBOARD_POSITION = new Vector2((float) ((double) SubMenuMain.ONLINE_POSITION.X + (double) SubMenuMain.ONLINE_SIZE.X - ((double) SubMenuMain.LEADERBOARD_SIZE.X + 12.0 + (double) SubMenuMain.OPTIONS_SIZE.X) - 25.0), SubMenu.BACK_POSITION.Y);
  private static readonly Vector2 OPTIONS_POSITION = new Vector2((float) ((double) SubMenuMain.LEADERBOARD_POSITION.X + (double) SubMenuMain.LEADERBOARD_SIZE.X + 12.0), SubMenuMain.LEADERBOARD_POSITION.Y);
  private OptionsMessageBox mRUSure;
  private bool mFirstTimeOnEnter = true;
  private bool mHasMythosLicense;
  private MenuImageTextItem mLockItem_Mythos;
  private MenuImageTextItem mNewItem_Mythos;
  private bool mHasVietnamLicense;
  private bool mHasOSOTCLicense;
  private bool mHasDUNG1License;
  private bool mHasDUNG2License;
  private MenuImageTextItem mLockItem_Vietnam;
  private MenuImageTextItem mLockItem_OSOTC;
  private MenuImageTextItem mLockItem_DUNG1;
  private MenuImageTextItem mLockItem_DUNG2;
  private bool mMythosHasNewContent = true;
  private bool mMythosLevelCastle = true;
  private bool mMythosLevelMountain = true;
  private bool mMythosLevelRlyeh = true;
  private bool mVietnamHasNewContent = true;
  private bool mOSOTCHasNewContent = true;
  private bool mDungeons1HasNewContent = true;
  private bool mDungeons2HasNewContent = true;
  private bool mVersusHasNewContent = true;
  private bool mChallangeHasNewContent = true;
  private MenuImageTextItem mNewItem_Vietnam;
  private MenuImageTextItem mNewItem_OSOTC;
  private MenuImageTextItem mNewItem_DUNG1;
  private MenuImageTextItem mNewItem_Challange;
  private MenuImageTextItem mNewItem_Versus;
  private MenuImageTextItem mNewItem_DUNG2;
  protected static Texture2D sNEWPagesTexture;

  public static SubMenuMain Instance
  {
    get
    {
      if (SubMenuMain.mSingelton == null)
      {
        lock (SubMenuMain.mSingeltonLock)
        {
          if (SubMenuMain.mSingelton == null)
            SubMenuMain.mSingelton = new SubMenuMain();
        }
      }
      return SubMenuMain.mSingelton;
    }
  }

  public SubMenuMain()
  {
    if (SubMenuMain.sNEWPagesTexture == null)
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
        SubMenuMain.sNEWPagesTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages_NEW");
    }
    this.mSelectedPosition = 0;
    this.mMenuItems = new List<MenuItem>();
    this.mRUSure = new OptionsMessageBox(SubMenuMain.LOC_RU_SURE, new int[2]
    {
      Defines.LOC_GEN_YES,
      Defines.LOC_GEN_NO
    });
    this.mRUSure.Select += new Action<OptionsMessageBox, int>(this.QuitCallBack);
    this.SetupMenu();
  }

  private void CheckForMythosDLC() => this.mHasMythosLicense = HackHelper.CheckLicenseMythos();

  private void CheckForVietnamDLC() => this.mHasVietnamLicense = HackHelper.CheckLicenseVietnam();

  private void CheckForOSOTCDLC() => this.mHasOSOTCLicense = HackHelper.CheckLicenseOSOTC();

  private void CheckForDungeons1DLC() => this.mHasDUNG1License = HackHelper.CheckLicenseDungeons1();

  private void CheckForDungeons2DLC() => this.mHasDUNG2License = HackHelper.CheckLicenseDungeons2();

  private void DlcInstalled(SteamWrapper.DlcInstalled obj)
  {
    LevelManager.Instance.UpdateMythosLicense();
    this.CheckForMythosDLC();
    this.CheckForVietnamDLC();
    this.CheckForOSOTCDLC();
    this.CheckForDungeons1DLC();
    this.CheckForDungeons2DLC();
  }

  private void QuitCallBack(MessageBox iSender, int iSelection)
  {
    switch (iSelection)
    {
      case 0:
        int num = (int) GC.WaitForFullGCComplete(2000);
        Thread.Sleep(0);
        Magicka.Game.Instance.Exit();
        break;
    }
  }

  public void ShowRUSure() => this.mRUSure.Show();

  public override void ControllerA(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 0:
        SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Campaign;
        Tome.Instance.PushMenu((SubMenu) SubMenuCampaignSelect_SaveSlotSelect.Instance, 1);
        break;
      case 1:
        if (this.mHasMythosLicense)
        {
          DLC_StatusHelper.Instance.Item_TrySetUsed("main_menu_entry", "mythos", true);
          SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Mythos;
          Tome.Instance.PushMenu((SubMenu) SubMenuCampaignSelect_SaveSlotSelect.Instance, 1);
          break;
        }
        SteamUtils.ActivateGameOverlayToStore(73058U, OverlayStoreFlag.None);
        break;
      case 2:
        if (this.mHasVietnamLicense)
        {
          SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.StoryChallange;
          DLC_StatusHelper.Instance.Item_TrySetUsed("level", "#challenge_vietnam", true);
          SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, "ch_vietnam", false);
          SubMenuCharacterSelect.Instance.ValidateLevels();
          SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
          Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
          break;
        }
        SteamUtils.ActivateGameOverlayToStore(42918U, OverlayStoreFlag.None);
        break;
      case 3:
        if (this.mHasOSOTCLicense)
        {
          SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.StoryChallange;
          DLC_StatusHelper.Instance.Item_TrySetUsed("level", "#challenge_osotc", true);
          SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, "ch_osotc", false);
          SubMenuCharacterSelect.Instance.ValidateLevels();
          SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
          Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
          break;
        }
        SteamUtils.ActivateGameOverlayToStore(73093U, OverlayStoreFlag.None);
        break;
      case 4:
        if (this.mHasDUNG1License)
        {
          SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.StoryChallange;
          DLC_StatusHelper.Instance.Item_TrySetUsed("level", "#challenge_dungeons_chapter1", true);
          SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, "ch_dungeons_ch1", false);
          SubMenuCharacterSelect.Instance.ValidateLevels();
          SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
          Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
          break;
        }
        SteamUtils.ActivateGameOverlayToStore(73115U, OverlayStoreFlag.None);
        break;
      case 5:
        if (this.mHasDUNG2License)
        {
          SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.StoryChallange;
          DLC_StatusHelper.Instance.Item_TrySetUsed("level", "#challenge_dungeons_chapter2", true);
          SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, "ch_dungeons_ch2", false);
          SubMenuCharacterSelect.Instance.ValidateLevels();
          SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
          Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
          break;
        }
        SteamUtils.ActivateGameOverlayToStore(0U, OverlayStoreFlag.None);
        break;
      case 6:
        SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Challenge;
        SubMenuCharacterSelect.Instance.SetSettings(GameType.Challenge, -1, false);
        SubMenuCharacterSelect.Instance.ValidateLevels();
        SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
        Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
        break;
      case 7:
        SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Versus;
        SubMenuCharacterSelect.Instance.SetSettings(GameType.Versus, -1, false);
        SubMenuCharacterSelect.Instance.ValidateLevels();
        SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
        Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
        break;
      case 8:
        Tome.Instance.PushMenu((SubMenu) SubMenuOnline.Instance, 1);
        break;
      case 9:
        Tome.Instance.PushMenu((SubMenu) SubMenuLeaderboards.Instance, 1);
        break;
      case 10:
        Tome.Instance.PushMenu((SubMenu) SubMenuOptions.Instance, 1);
        break;
      case 11:
        this.mRUSure.Show();
        break;
    }
  }

  public override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (!Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) || !oRightPageHit)
      return;
    if (this.mMenuItems[this.mMenuItems.Count - 1].Enabled && this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref oHitPosition))
    {
      this.mSelectedPosition = this.mMenuItems.Count - 1;
      this.ControllerMouseClicked(iSender);
    }
    else
    {
      for (int index = 0; index < this.mMenuItems.Count; ++index)
      {
        MenuItem mMenuItem = this.mMenuItems[index];
        if (mMenuItem != null && mMenuItem.Enabled && mMenuItem.InsideBounds(ref oHitPosition))
        {
          this.mSelectedPosition = index;
          if ((iState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed) && (iState.RightButton != ButtonState.Released || iOldState.RightButton != ButtonState.Pressed))
            break;
          this.ControllerMouseClicked(iSender);
          break;
        }
      }
    }
  }

  public override void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit))
    {
      if (!oRightPageHit)
        return;
      bool flag = false;
      if (this.mMenuItems[this.mMenuItems.Count - 1].Enabled && this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref oHitPosition))
      {
        this.UnselectAllMenuItems();
        this.mMenuItems[this.mMenuItems.Count - 1].Selected = true;
        flag = true;
        this.mSelectedPosition = this.mMenuItems.Count - 1;
      }
      else if (this.mMenuItems[11].Enabled && this.mMenuItems[11].InsideBounds(ref oHitPosition))
      {
        this.UnselectAllMenuItems();
        this.mMenuItems[5].Selected = true;
        this.mSelectedPosition = 5;
        flag = true;
      }
      else
      {
        for (int index = 0; index < this.mMenuItems.Count; ++index)
        {
          MenuItem mMenuItem = this.mMenuItems[index];
          if (mMenuItem.Enabled && mMenuItem.InsideBounds(ref oHitPosition))
          {
            if (this.mSelectedPosition != index)
              AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
            this.mKeyboardSelection = false;
            this.mSelectedPosition = index;
            this.UnselectAllMenuItems();
            if (this.mSelectedPosition == 1)
            {
              if (!this.mHasMythosLicense)
                this.mLockItem_Mythos.Selected = true;
              else
                this.mLockItem_Mythos.Selected = false;
              if (this.mMythosHasNewContent)
                this.mNewItem_Mythos.Selected = true;
              ToolTipMan.Instance.KillAll(false);
            }
            else if (this.mSelectedPosition == 2)
            {
              if (!this.mHasVietnamLicense)
                this.mLockItem_Vietnam.Selected = true;
              else
                this.mLockItem_Mythos.Selected = false;
              if (this.mVietnamHasNewContent)
                this.mNewItem_Vietnam.Selected = true;
              ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenu.LOC_TT_VIETNAM), iState);
            }
            else if (this.mSelectedPosition == 3)
            {
              if (!this.mHasOSOTCLicense)
                this.mLockItem_OSOTC.Selected = true;
              else
                this.mLockItem_OSOTC.Selected = false;
              if (this.mOSOTCHasNewContent)
                this.mNewItem_OSOTC.Selected = true;
              ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenu.LOC_TT_OSOTC), iState);
            }
            else if (this.mSelectedPosition == 4)
            {
              if (!this.mHasDUNG1License)
                this.mLockItem_DUNG1.Selected = true;
              else
                this.mLockItem_DUNG1.Selected = false;
              if (this.mDungeons1HasNewContent)
                this.mNewItem_DUNG1.Selected = true;
              ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenu.LOC_TT_DUNG1), iState);
            }
            else if (this.mSelectedPosition == 5)
            {
              if (!this.mHasDUNG2License)
                this.mLockItem_DUNG2.Selected = true;
              else
                this.mLockItem_DUNG2.Selected = false;
              if (this.mDungeons2HasNewContent)
                this.mNewItem_DUNG2.Selected = true;
              ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenu.LOC_TT_DUNG2), iState);
            }
            else if (this.mSelectedPosition == 6)
            {
              if (this.mChallangeHasNewContent)
                this.mNewItem_Challange.Selected = true;
              ToolTipMan.Instance.KillAll(false);
            }
            else if (this.mSelectedPosition == 7)
            {
              if (this.mVersusHasNewContent)
                this.mNewItem_Versus.Selected = true;
              ToolTipMan.Instance.KillAll(false);
            }
            else if (this.mSelectedPosition == 10)
              ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenu.LOC_OPTIONS), iState);
            else if (this.mSelectedPosition == 9)
              ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenu.LOC_LEADERBOARDS), iState);
            else
              ToolTipMan.Instance.KillAll(false);
            mMenuItem.Selected = true;
            flag = true;
            break;
          }
        }
      }
      if (flag)
        return;
      this.UnselectAllMenuItems();
    }
    else
    {
      if (this.mKeyboardSelection)
        return;
      this.UnselectAllMenuItems();
    }
  }

  private void UnselectAllMenuItems()
  {
    lock (this.mMenuItems)
    {
      for (int index = 0; index < this.mMenuItems.Count; ++index)
        this.mMenuItems[index].Selected = false;
    }
    this.mLockItem_Mythos.Selected = this.mLockItem_Vietnam.Selected = this.mLockItem_OSOTC.Selected = this.mLockItem_DUNG1.Selected = this.mNewItem_Mythos.Selected = this.mNewItem_Vietnam.Selected = this.mNewItem_OSOTC.Selected = this.mNewItem_DUNG1.Selected = this.mNewItem_Versus.Selected = this.mNewItem_Challange.Selected = false;
  }

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

  public override void ControllerLeft(Controller iSender) => base.ControllerUp(iSender);

  public override void ControllerRight(Controller iSender) => base.ControllerDown(iSender);

  public override void OnEnter()
  {
    ToolTipMan.Instance.KillAll(true);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, GamePadMenuHelp.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    if (this.mFirstTimeOnEnter)
      this.mFirstTimeOnEnter = false;
    else
      Tome.LoadNewPromotion();
    if (this.mFirstTimeOnEnter)
      this.mMythosHasNewContent = this.mVietnamHasNewContent = this.mOSOTCHasNewContent = this.mDungeons1HasNewContent = this.mDungeons2HasNewContent = this.mVersusHasNewContent = this.mChallangeHasNewContent = false;
    DLC_StatusHelper instance = DLC_StatusHelper.Instance;
    Thread.Sleep(0);
    Magicka.Game.Instance.Form.BeginInvoke((Delegate) (() =>
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
    SteamAPI.GameOverlayActivated -= new Action<GameOverlayActivated>(this.SteamOverlayActivated);
    SteamAPI.GameOverlayActivated += new Action<GameOverlayActivated>(this.SteamOverlayActivated);
    base.OnEnter();
    this.mSelectedPosition = -1;
  }

  public override void OnExit()
  {
    ToolTipMan.Instance.KillAll(true);
    SteamAPI.GameOverlayActivated -= new Action<GameOverlayActivated>(this.SteamOverlayActivated);
    base.OnExit();
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.VertexColorEnabled = false;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    for (int index = 0; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].Draw(this.mEffect);
    if (!this.mHasMythosLicense)
      this.mLockItem_Mythos.Draw(this.mEffect);
    else if (this.mMythosLevelCastle && this.mMythosLevelMountain && this.mMythosLevelRlyeh)
      this.mNewItem_Mythos.Draw(this.mEffect);
    if (!this.mHasVietnamLicense)
      this.mLockItem_Vietnam.Draw(this.mEffect);
    else if (this.mVietnamHasNewContent)
      this.mNewItem_Vietnam.Draw(this.mEffect);
    if (!this.mHasOSOTCLicense)
      this.mLockItem_OSOTC.Draw(this.mEffect);
    else if (this.mOSOTCHasNewContent)
      this.mNewItem_OSOTC.Draw(this.mEffect);
    if (!this.mHasDUNG1License)
      this.mLockItem_DUNG1.Draw(this.mEffect);
    else if (this.mDungeons1HasNewContent)
      this.mNewItem_DUNG1.Draw(this.mEffect);
    if (!this.mHasDUNG2License)
      this.mLockItem_DUNG2.Draw(this.mEffect);
    else if (this.mDungeons2HasNewContent)
      this.mNewItem_DUNG2.Draw(this.mEffect);
    if (this.mChallangeHasNewContent)
      this.mNewItem_Challange.Draw(this.mEffect);
    if (this.mVersusHasNewContent)
      this.mNewItem_Versus.Draw(this.mEffect);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  private void SteamOverlayActivated(GameOverlayActivated gameOverlayActivated)
  {
    if (gameOverlayActivated.mActive != (byte) 0)
      return;
    this.DlcInstalled(new SteamWrapper.DlcInstalled());
  }

  private void SetupMenu()
  {
    Vector2 iTextureOffset1 = new Vector2(944f / (float) SubMenu.sPagesTexture.Width, 816f / (float) SubMenu.sPagesTexture.Height);
    Vector2 iTextureOffset2 = new Vector2(972f / (float) SubMenuMain.sNEWPagesTexture.Width, 974f / (float) SubMenuMain.sNEWPagesTexture.Height);
    Vector2 iTextureOffset3 = new Vector2(0.0f / (float) SubMenuMain.sNEWPagesTexture.Width, 0.0f / (float) SubMenuMain.sNEWPagesTexture.Height);
    Vector2 iTextureScale1 = new Vector2(SubMenuMain.CAMPAIGN_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.CAMPAIGN_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenuMain.CAMPAIGN_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale1, SubMenu.LOC_ADVENTURE, SubMenuMain.CAMPAIGN_TEXT, TextAlign.Center, SubMenuMain.mFont, SubMenuMain.CAMPAIGN_SIZE));
    iTextureOffset3 = new Vector2(390f / (float) SubMenuMain.sNEWPagesTexture.Width, 0.0f / (float) SubMenuMain.sNEWPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.MYTHOS_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.MYTHOS_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenuMain.MYTHOS_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale1, SubMenu.LOC_MYTHOS, SubMenuMain.MYTHOS_TEXT, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.MYTHOS_SIZE));
    iTextureScale1 = new Vector2(SubMenuMain.LOCK_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    this.mLockItem_Mythos = new MenuImageTextItem(SubMenuMain.MYTHOS_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset1, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
    this.mLockItem_Mythos.Selected = false;
    iTextureScale1 = new Vector2(SubMenuMain.NEW_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mNewItem_Mythos = new MenuImageTextItem(SubMenuMain.MYTHOS_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
    this.mNewItem_Mythos.Selected = false;
    iTextureOffset3 = new Vector2(0.0f / (float) SubMenuMain.sNEWPagesTexture.Width, 230f / (float) SubMenuMain.sNEWPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.VIETNAM_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.VIETNAM_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenuMain.VIETNAM_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale1, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.VIETNAM_SIZE));
    iTextureScale1 = new Vector2(SubMenuMain.LOCK_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    this.mLockItem_Vietnam = new MenuImageTextItem(SubMenuMain.VIETNAM_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset1, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
    this.mLockItem_Vietnam.Selected = false;
    iTextureScale1 = new Vector2(SubMenuMain.NEW_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mNewItem_Vietnam = new MenuImageTextItem(SubMenuMain.VIETNAM_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
    this.mNewItem_Vietnam.Selected = false;
    iTextureOffset3 = new Vector2(166f / (float) SubMenuMain.sNEWPagesTexture.Width, 230f / (float) SubMenuMain.sNEWPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.OSOTC_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.OSOTC_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenuMain.OSOTC_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale1, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.OSOTC_SIZE));
    iTextureScale1 = new Vector2(SubMenuMain.LOCK_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    this.mLockItem_OSOTC = new MenuImageTextItem(SubMenuMain.OSOTC_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset1, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
    this.mLockItem_OSOTC.Selected = false;
    iTextureScale1 = new Vector2(SubMenuMain.NEW_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mNewItem_OSOTC = new MenuImageTextItem(SubMenuMain.OSOTC_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
    this.mNewItem_OSOTC.Selected = false;
    iTextureOffset3 = new Vector2(332f / (float) SubMenuMain.sNEWPagesTexture.Width, 230f / (float) SubMenuMain.sNEWPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.DUNGEONS1_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.DUNGEONS1_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenuMain.DUNGEONS1_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale1, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.DUNGEONS1_SIZE));
    iTextureScale1 = new Vector2(SubMenuMain.LOCK_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    this.mLockItem_DUNG1 = new MenuImageTextItem(SubMenuMain.DUNGEONS1_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset1, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
    this.mLockItem_DUNG1.Selected = false;
    iTextureScale1 = new Vector2(SubMenuMain.NEW_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mNewItem_DUNG1 = new MenuImageTextItem(SubMenuMain.DUNGEONS1_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
    this.mNewItem_DUNG1.Selected = false;
    iTextureOffset3 = new Vector2(498f / (float) SubMenuMain.sNEWPagesTexture.Width, 230f / (float) SubMenuMain.sNEWPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.DUNGEONS2_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.DUNGEONS2_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenuMain.DUNGEONS2_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale1, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.DUNGEONS2_SIZE));
    iTextureScale1 = new Vector2(SubMenuMain.LOCK_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenuMain.LOCK_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    this.mLockItem_DUNG2 = new MenuImageTextItem(SubMenuMain.DUNGEONS2_LOCK_POSITION, SubMenu.sPagesTexture, iTextureOffset1, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LOCK_SIZE);
    this.mLockItem_DUNG2.Selected = false;
    iTextureScale1 = new Vector2(SubMenuMain.NEW_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mNewItem_DUNG2 = new MenuImageTextItem(SubMenuMain.DUNGEONS2_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
    this.mNewItem_DUNG2.Selected = false;
    iTextureOffset3 = new Vector2(1040f / (float) SubMenu.sPagesTexture.Width, 416f / (float) SubMenu.sPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.CHALLENGE_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenuMain.CHALLENGE_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    MenuImageTextItem menuImageTextItem1 = new MenuImageTextItem(SubMenuMain.CHALLENGE_POSITION, SubMenu.sPagesTexture, iTextureOffset3, iTextureScale1, SubMenu.LOC_CHALLENGES, SubMenuMain.CHALLENGE_TEXT, TextAlign.Center, SubMenuMain.mFont, SubMenuMain.CHALLENGE_SIZE);
    menuImageTextItem1.SetHitArea(0, 0, (int) SubMenuMain.CHALLENGE_SIZE.X - 56, (int) SubMenuMain.CHALLENGE_SIZE.Y);
    this.mMenuItems.Add((MenuItem) menuImageTextItem1);
    iTextureScale1 = new Vector2(SubMenuMain.NEW_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mNewItem_Challange = new MenuImageTextItem(SubMenuMain.CHALLANGE_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
    this.mNewItem_Challange.Selected = false;
    iTextureOffset3 = new Vector2((1040f + SubMenuMain.CHALLENGE_SIZE.X) / (float) SubMenu.sPagesTexture.Width, 416f / (float) SubMenu.sPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.VERSUS_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenuMain.VERSUS_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    MenuImageTextItem menuImageTextItem2 = new MenuImageTextItem(SubMenuMain.VERSUS_POSITION, SubMenu.sPagesTexture, iTextureOffset3, iTextureScale1, SubMenu.LOC_VERSUS, SubMenuMain.VERSUS_TEXT, TextAlign.Center, SubMenuMain.mFont, SubMenuMain.VERSUS_SIZE);
    menuImageTextItem2.SetHitArea(56, 0, (int) SubMenuMain.VERSUS_SIZE.X - 112 /*0x70*/, (int) SubMenuMain.VERSUS_SIZE.Y);
    this.mMenuItems.Add((MenuItem) menuImageTextItem2);
    iTextureScale1 = new Vector2(SubMenuMain.NEW_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.NEW_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    this.mNewItem_Versus = new MenuImageTextItem(SubMenuMain.VERSUS_NEW_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset2, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.NEW_SIZE);
    this.mNewItem_Versus.Selected = false;
    iTextureOffset3 = new Vector2((1040f + SubMenuMain.CHALLENGE_SIZE.X + SubMenuMain.VERSUS_SIZE.X) / (float) SubMenu.sPagesTexture.Width, 416f / (float) SubMenu.sPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.ONLINE_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenuMain.ONLINE_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    MenuImageTextItem menuImageTextItem3 = new MenuImageTextItem(SubMenuMain.ONLINE_POSITION, SubMenu.sPagesTexture, iTextureOffset3, iTextureScale1, SubMenu.LOC_ONLINE_PLAY, SubMenuMain.ONLINE_TEXT, TextAlign.Center, SubMenuMain.mFont, SubMenuMain.ONLINE_SIZE);
    menuImageTextItem3.SetHitArea(56, 0, (int) SubMenuMain.ONLINE_SIZE.X - 56, (int) SubMenuMain.ONLINE_SIZE.Y);
    this.mMenuItems.Add((MenuItem) menuImageTextItem3);
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      if (this.mMenuItems[index] != null)
      {
        this.mMenuItems[index].Color = new Vector4(1f - Defines.MESSAGEBOX_COLOR_DEFAULT.X, 1f - Defines.MESSAGEBOX_COLOR_DEFAULT.Y, 1f - Defines.MESSAGEBOX_COLOR_DEFAULT.Z, 1f);
        (this.mMenuItems[index] as MenuImageTextItem).Text.DrawShadows = true;
        (this.mMenuItems[index] as MenuImageTextItem).Text.ShadowAlpha = 1f;
        (this.mMenuItems[index] as MenuImageTextItem).Text.ShadowsOffset = new Vector2(2f, 2f);
        this.mMenuItems[index].ColorSelected = new Vector4(1f);
        this.mMenuItems[index].ColorDisabled = this.mMenuItems[index].Color;
      }
    }
    iTextureOffset3 = new Vector2(1f / (float) SubMenuMain.sNEWPagesTexture.Width, 952f / (float) SubMenuMain.sNEWPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.LEADERBOARD_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.LEADERBOARD_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    MenuImageTextItem menuImageTextItem4 = new MenuImageTextItem(SubMenuMain.LEADERBOARD_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale1, 0, Vector2.Zero, TextAlign.Left, SubMenuMain.mFont, SubMenuMain.LEADERBOARD_SIZE);
    menuImageTextItem4.SetHitArea(0, 0, (int) SubMenuMain.LEADERBOARD_SIZE.X, (int) SubMenuMain.LEADERBOARD_SIZE.Y);
    this.mMenuItems.Add((MenuItem) menuImageTextItem4);
    iTextureOffset3 = new Vector2(73f / (float) SubMenuMain.sNEWPagesTexture.Width, 952f / (float) SubMenuMain.sNEWPagesTexture.Height);
    iTextureScale1 = new Vector2(SubMenuMain.OPTIONS_SIZE.X / (float) SubMenuMain.sNEWPagesTexture.Width, SubMenuMain.OPTIONS_SIZE.Y / (float) SubMenuMain.sNEWPagesTexture.Height);
    MenuImageTextItem menuImageTextItem5 = new MenuImageTextItem(SubMenuMain.OPTIONS_POSITION, SubMenuMain.sNEWPagesTexture, iTextureOffset3, iTextureScale1, 0, Vector2.Zero, TextAlign.Right, SubMenuMain.mFont, SubMenuMain.OPTIONS_SIZE);
    menuImageTextItem5.SetHitArea(0, 0, (int) SubMenuMain.OPTIONS_SIZE.X, (int) SubMenuMain.OPTIONS_SIZE.Y);
    this.mMenuItems.Add((MenuItem) menuImageTextItem5);
    Vector2 iTextureScale2 = new Vector2(SubMenu.BACK_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenu.BACK_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, iTextureScale2, 0, new Vector2(), TextAlign.Left, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
    SteamAPI.DlcInstalled += new Action<SteamWrapper.DlcInstalled>(this.DlcInstalled);
    this.CheckForMythosDLC();
    this.CheckForOSOTCDLC();
    this.CheckForVietnamDLC();
    this.CheckForDungeons1DLC();
    this.CheckForDungeons2DLC();
  }

  private enum MenuChoice
  {
    Campaign,
    Stars,
    VIETNAM,
    OSOTC,
    DUNG1,
    DUNG2,
    Challenge,
    Versus,
    Online,
    LeaderBoards,
    Options,
    Exit,
  }
}
