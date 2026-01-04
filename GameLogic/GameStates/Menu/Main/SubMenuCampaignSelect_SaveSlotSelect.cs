// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuCampaignSelect_SaveSlotSelect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

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
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal sealed class SubMenuCampaignSelect_SaveSlotSelect : SubMenu
{
  private static SubMenuCampaignSelect_SaveSlotSelect sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static readonly Vector2 DELETE_UVOFFSET = new Vector2(0.75f, 3f / 32f);
  private static readonly Vector2 DELETE_SIZE = new Vector2(64f, 64f);
  private static readonly Vector2 DELETE_UVSIZE = new Vector2(SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE.X / 2048f, SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE.Y / 1024f);
  private int mLoadingTextLength;
  private Text mLoadingText = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Left, true);
  private float mLoadingDotTimer = 0.5f;
  private bool mHasNotifiedHax;
  public Action mComplete;
  private GameType mGameType = GameType.Campaign;
  private SaveData mCurrentSaveData;
  private SaveData[] mSaveData;
  private CampaignNode[] mLevelNodes;
  private OptionsMessageBox mRUSure;
  public static readonly int NEWCAMP = "#add_menu_newcamp".GetHashCodeCustom();

  public static SubMenuCampaignSelect_SaveSlotSelect Instance
  {
    get
    {
      if (SubMenuCampaignSelect_SaveSlotSelect.sSingelton == null)
      {
        lock (SubMenuCampaignSelect_SaveSlotSelect.sSingeltonLock)
        {
          if (SubMenuCampaignSelect_SaveSlotSelect.sSingelton == null)
            SubMenuCampaignSelect_SaveSlotSelect.sSingelton = new SubMenuCampaignSelect_SaveSlotSelect();
        }
      }
      return SubMenuCampaignSelect_SaveSlotSelect.sSingelton;
    }
  }

  private SubMenuCampaignSelect_SaveSlotSelect()
  {
    this.mMenuItems = new List<MenuItem>();
    this.mPosition.Y = 192f;
    Vector2 mPosition1 = this.mPosition;
    mPosition1.X += 384f;
    mPosition1.Y -= 24f;
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(mPosition1, SubMenu.sPagesTexture, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVOFFSET, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVSIZE, 0, new Vector2(), TextAlign.Center, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE));
    mPosition1.Y += 256f;
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(mPosition1, SubMenu.sPagesTexture, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVOFFSET, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVSIZE, 0, new Vector2(), TextAlign.Center, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE));
    mPosition1.Y += 256f;
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(mPosition1, SubMenu.sPagesTexture, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVOFFSET, SubMenuCampaignSelect_SaveSlotSelect.DELETE_UVSIZE, 0, new Vector2(), TextAlign.Center, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenuCampaignSelect_SaveSlotSelect.DELETE_SIZE));
    Vector2 mPosition2 = this.mPosition;
    for (int index = 0; index < 3; ++index)
    {
      this.mMenuItems.Add((MenuItem) new SaveSlot(mPosition2, SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP, 0, 0, false, 0UL, false));
      mPosition2.Y += 256f;
    }
    this.mRUSure = new OptionsMessageBox("#add_menu_rus_del".GetHashCodeCustom(), new int[2]
    {
      Defines.LOC_GEN_YES,
      Defines.LOC_GEN_NO
    });
    this.mRUSure.Select += new Action<OptionsMessageBox, int>(this.DeleteCallback);
    string iText = LanguageManager.Instance.GetString("#network_23".GetHashCodeCustom());
    this.mLoadingTextLength = iText.Length;
    this.mLoadingText.SetText(iText);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    string iText = LanguageManager.Instance.GetString(LevelMessageBox.LOC_LOADING);
    this.mLoadingTextLength = iText.Length;
    this.mLoadingText.SetText(iText);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if (this.mGameType == GameType.Mythos && LevelManager.Instance.MythosCampaignLicense == HackHelper.License.No)
      Tome.Instance.PopMenu();
    this.mLoadingDotTimer -= iDeltaTime;
    if (!this.mHasNotifiedHax && LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Hacked)
    {
      this.mHasNotifiedHax = true;
      OptionsMessageBox optionsMessageBox = new OptionsMessageBox("#notice_mod_campaign".GetHashCode(), new int[1]
      {
        "#add_menu_ok".GetHashCode()
      });
      LanguageManager.Instance.LanguageChanged -= new Action(((MessageBox) optionsMessageBox).LanguageChanged);
      optionsMessageBox.Show();
    }
    Vector2 mPosition = this.mPosition;
    mPosition.X += 340f;
    mPosition.Y += 44f;
    for (int index = 0; index < 3; ++index)
    {
      this.mMenuItems[index].Position = mPosition;
      this.mMenuItems[index].Enabled = !(this.mMenuItems[index + 3] as SaveSlot).EmptySlot;
      mPosition.Y += 256f;
    }
  }

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
      while ((double) this.mLoadingDotTimer < 0.0)
      {
        this.mLoadingDotTimer += 0.5f;
        int index;
        if (this.mLoadingText.Characters[this.mLoadingTextLength] == char.MinValue)
          index = this.mLoadingTextLength;
        else if (this.mLoadingText.Characters[this.mLoadingTextLength + 1] == char.MinValue)
          index = this.mLoadingTextLength + 1;
        else if (this.mLoadingText.Characters[this.mLoadingTextLength + 2] == char.MinValue)
        {
          index = this.mLoadingTextLength + 2;
        }
        else
        {
          index = -1;
          this.mLoadingText.Characters[this.mLoadingTextLength] = char.MinValue;
        }
        if (index > 0)
        {
          this.mLoadingText.Characters[index] = '.';
          this.mLoadingText.Characters[index + 1] = char.MinValue;
        }
        this.mLoadingText.MarkAsDirty();
      }
      this.mLoadingText.Draw(this.mEffect, (float) (512.0 - (double) this.mLoadingText.Font.MeasureText(this.mLoadingText.Characters, true, this.mLoadingTextLength).X * 0.5), 850f);
    }
    for (int index = 3; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].Draw(this.mEffect);
    for (int index = 0; index < 3; ++index)
    {
      if (this.mMenuItems[index].Enabled)
        this.mMenuItems[index].Draw(this.mEffect);
    }
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public void Delete(Controller iSender)
  {
    if (NetworkManager.Instance.State == NetworkState.Client | this.mSelectedPosition < 3 || this.mSaveData[this.mSelectedPosition - 3] == null)
      return;
    this.mRUSure.SelectedIndex = 1;
    this.mRUSure.Show();
  }

  private void DeleteCallback(MessageBox iSender, int iSelection)
  {
    if (iSelection != 0)
      return;
    if (this.mSelectedPosition < 3)
    {
      this.mSaveData[this.mSelectedPosition] = (SaveData) null;
      (this.mMenuItems[this.mSelectedPosition + 3] as SaveSlot).Set(SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP, 0, 0, false, 0UL, false);
    }
    else
    {
      this.mSaveData[this.mSelectedPosition - 3] = (SaveData) null;
      (this.mMenuItems[this.mSelectedPosition] as SaveSlot).Set(SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP, 0, 0, false, 0UL, false);
    }
    SaveManager.Instance.SaveCampaign();
  }

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

  public override void ControllerA(Controller iSender)
  {
    if (NetworkManager.Instance.State == NetworkState.Client || this.mGameType == GameType.Mythos && LevelManager.Instance.MythosCampaignLicense == HackHelper.License.No)
      return;
    if (this.mSelectedPosition == this.mMenuItems.Count - 1)
    {
      Tome.Instance.PopMenu();
      this.mCurrentSaveData = (SaveData) null;
    }
    else if (this.mSelectedPosition < 3)
    {
      if (this.mSaveData[this.mSelectedPosition] == null)
        return;
      this.mRUSure.SelectedIndex = 1;
      this.mRUSure.Show();
    }
    else
    {
      if ((this.mGameType != GameType.Campaign || LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Pending) && (this.mGameType != GameType.Mythos || LevelManager.Instance.MythosCampaignLicense == HackHelper.License.Pending))
        return;
      if (this.mSaveData[this.mSelectedPosition - 3] == null)
      {
        SaveData saveData = new SaveData();
        this.mSaveData[this.mSelectedPosition - 3] = saveData;
        SaveManager.Instance.SaveCampaign();
        int hashCodeCustom = this.mLevelNodes[(int) saveData.Level].Name.GetHashCodeCustom();
        int description = this.mLevelNodes[(int) saveData.Level].Description;
        bool looped = saveData.Looped;
        int currentPlayTime = saveData.CurrentPlayTime;
        ulong unlockedMagicks = saveData.UnlockedMagicks;
        (this.mMenuItems[this.mSelectedPosition] as SaveSlot).Set(hashCodeCustom, description, currentPlayTime, looped, unlockedMagicks, this.mGameType == GameType.Mythos);
        this.mCurrentSaveData = this.mSaveData[this.mSelectedPosition - 3];
        SubMenuCharacterSelect.Instance.SetSettings(this.mGameType, (int) this.mCurrentSaveData.Level, false);
        SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
        if (this.mComplete != null)
          this.mComplete();
        Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
      }
      else
      {
        this.mCurrentSaveData = this.mSaveData[this.mSelectedPosition - 3];
        SubMenuCharacterSelect.Instance.SetSettings(this.mGameType, (int) this.mCurrentSaveData.Level, false);
        SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
        SubMenuCharacterSelect.Instance.SetLevelUsed(LevelManager.Instance.GetLevel(this.mGameType, (int) this.mCurrentSaveData.Level).Name);
        if (this.mComplete != null)
          this.mComplete();
        Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
      }
    }
  }

  public override void ControllerB(Controller iSender)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.mCurrentSaveData = (SaveData) null;
    for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
    {
      Player player = Magicka.Game.Instance.Players[index];
      player.Weapon = "";
      player.Staff = "";
    }
    base.ControllerB(iSender);
  }

  internal GameType GameType
  {
    get => this.mGameType;
    set => this.mGameType = value;
  }

  internal CampaignNode[] Campaign => this.mLevelNodes;

  public SaveData CurrentSaveData => this.mCurrentSaveData;

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

  public override void OnExit() => this.mComplete = (Action) null;

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
    for (int index = 0; index < 3; ++index)
    {
      int iName = SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP;
      int iDesc = 0;
      int iTime = 0;
      ulong iMagicks = 0;
      bool iLooped = false;
      if (this.mSaveData[index] != null)
      {
        SaveData saveData = this.mSaveData[index];
        iName = this.mLevelNodes[(int) saveData.Level].Name.GetHashCodeCustom();
        iDesc = this.mLevelNodes[(int) saveData.Level].Description;
        iTime = saveData.CurrentPlayTime;
        iLooped = saveData.Looped;
        iMagicks = saveData.UnlockedMagicks;
      }
      (this.mMenuItems[index + 3] as SaveSlot).Set(iName, iDesc, iTime, iLooped, iMagicks, this.mGameType == GameType.Mythos);
    }
  }
}
