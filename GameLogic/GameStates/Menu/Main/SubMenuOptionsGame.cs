// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuOptionsGame
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuOptionsGame : SubMenu
{
  private static readonly int LOC_GAME_OPTS = "#menu_opt_12".GetHashCodeCustom();
  private static readonly int LOC_GORE = "#menu_opt_game_01".GetHashCodeCustom();
  private static readonly int LOC_DAMNUMBERS = "#menu_opt_game_02".GetHashCodeCustom();
  private static readonly int LOC_HBARS = "#menu_opt_game_03".GetHashCodeCustom();
  private static readonly int LOC_SPELLWHEEL = "#menu_opt_game_04".GetHashCodeCustom();
  private static readonly int LOC_LANGUAGE = "#menu_opt_game_05".GetHashCodeCustom();
  private static SubMenuOptionsGame mSingelton;
  private static volatile object mSingeltonLock = new object();
  private readonly int mSampleHash = "misc_gib".GetHashCodeCustom();
  private List<MenuTextItem> mMenuOptions;
  private SettingOptions mBloodAndGore;
  private SettingOptions mDamageNumbers;
  private SettingOptions mHealthBars;
  private SettingOptions mSpellWheel;
  private GlobalSettings mGlobalSettings = GlobalSettings.Instance;

  public static SubMenuOptionsGame Instance
  {
    get
    {
      if (SubMenuOptionsGame.mSingelton == null)
      {
        lock (SubMenuOptionsGame.mSingeltonLock)
        {
          if (SubMenuOptionsGame.mSingelton == null)
            SubMenuOptionsGame.mSingelton = new SubMenuOptionsGame();
        }
      }
      return SubMenuOptionsGame.mSingelton;
    }
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    for (int index = 0; index < this.mMenuOptions.Count; ++index)
      this.mMenuOptions[index].LanguageChanged();
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGame.LOC_GAME_OPTS));
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
  }

  private SubMenuOptionsGame()
  {
    this.mSelectedPosition = 0;
    this.mMenuItems = new List<MenuItem>();
    this.mMenuOptions = new List<MenuTextItem>();
    this.mMenuTitle = new Text(30, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGame.LOC_GAME_OPTS));
    this.AddMenuOptions(SubMenu.GetSettingLoc(this.mGlobalSettings.BloodAndGore));
    this.AddMenuOptions(SubMenu.GetSettingLoc(this.mGlobalSettings.DamageNumbers));
    this.AddMenuOptions(SubMenu.GetSettingLoc(this.mGlobalSettings.HealthBars));
    this.AddMenuOptions(SubMenu.GetSettingLoc(this.mGlobalSettings.SpellWheel));
    this.AddMenuOptions(LanguageManager.Instance.GetNativeName(LanguageManager.Instance.CurrentLanguage));
    this.AddMenuTextItem(SubMenuOptionsGame.LOC_GORE);
    this.AddMenuTextItem(SubMenuOptionsGame.LOC_DAMNUMBERS);
    this.AddMenuTextItem(SubMenuOptionsGame.LOC_HBARS);
    this.AddMenuTextItem(SubMenuOptionsGame.LOC_SPELLWHEEL);
    this.AddMenuTextItem(SubMenuOptionsGame.LOC_LANGUAGE);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
    LanguageMessageBox.Instance.Complete += new Action<Language>(this.OnSelect);
  }

  public override MenuTextItem AddMenuTextItem(int iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuItems.Count;
    MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, font, TextAlign.Right);
    this.mMenuItems.Add((MenuItem) menuTextItem);
    return menuTextItem;
  }

  public void AddMenuOptions(int iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.X += 40f;
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuOptions.Count;
    this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left));
  }

  public void AddMenuOptions(string iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.X += 40f;
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuOptions.Count;
    this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left));
  }

  private void OnSelect(Language iLanguage)
  {
    LanguageManager.Instance.SetLanguage(iLanguage);
    this.mMenuOptions[4].SetText(LanguageManager.Instance.GetNativeName(LanguageManager.Instance.CurrentLanguage));
  }

  public override void OnEnter()
  {
    base.OnEnter();
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    this.mBloodAndGore = this.mGlobalSettings.BloodAndGore;
    this.mDamageNumbers = this.mGlobalSettings.DamageNumbers;
    this.mHealthBars = this.mGlobalSettings.HealthBars;
    this.mSpellWheel = this.mGlobalSettings.SpellWheel;
  }

  public override void OnExit()
  {
    base.OnExit();
    if (!(this.mBloodAndGore != this.mGlobalSettings.BloodAndGore | this.mDamageNumbers != this.mGlobalSettings.DamageNumbers | this.mHealthBars != this.mGlobalSettings.HealthBars | this.mSpellWheel != this.mGlobalSettings.SpellWheel))
      return;
    SaveManager.Instance.SaveSettings();
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    base.Draw(iLeftSide, iRightSide);
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48 /*0x30*/), new Rectangle(208 /*0xD0*/, 220, 608, 48 /*0x30*/));
    foreach (MenuItem mMenuOption in this.mMenuOptions)
      mMenuOption.Draw(this.mEffect);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (!this.mKeyboardSelection)
      return;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      this.mMenuItems[index].Selected = index == this.mSelectedPosition;
      if (index < this.mMenuOptions.Count)
        this.mMenuOptions[index].Selected = index == this.mSelectedPosition;
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
    if (this.mMenuItems == null || this.mMenuItems.Count == 0 || !Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) || !oRightPageHit)
      return;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      MenuItem menuItem = (MenuItem) null;
      if (index < this.mMenuOptions.Count)
        menuItem = (MenuItem) this.mMenuOptions[index];
      if (mMenuItem.Enabled && (mMenuItem.InsideBounds(ref oHitPosition) || menuItem != null && menuItem.InsideBounds(ref oHitPosition)))
      {
        this.mSelectedPosition = index;
        if (iState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
          break;
        this.ControllerMouseClicked(iSender);
        break;
      }
    }
  }

  protected override void ControllerMouseClicked(Controller iSender)
  {
    this.mKeyboardSelection = true;
    if (this.mSelectedPosition == 5)
      Tome.Instance.PopMenu();
    if (this.mSelectedPosition == 4)
      LanguageMessageBox.Instance.Show();
    else
      this.ControllerRight(iSender);
    this.mKeyboardSelection = false;
  }

  public override void ControllerA(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 4:
        LanguageMessageBox.Instance.Show();
        break;
      case 5:
        Tome.Instance.PopMenu();
        break;
      default:
        this.ControllerRight(iSender);
        break;
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
    if (!Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) || !oRightPageHit)
      return;
    bool flag = false;
    for (int index1 = 0; index1 < this.mMenuItems.Count; ++index1)
    {
      MenuItem mMenuItem = this.mMenuItems[index1];
      MenuItem menuItem = (MenuItem) null;
      if (index1 < this.mMenuOptions.Count)
        menuItem = (MenuItem) this.mMenuOptions[index1];
      if (mMenuItem.Enabled && (mMenuItem.InsideBounds(ref oHitPosition) || menuItem != null && menuItem.InsideBounds(ref oHitPosition)))
      {
        if (this.mSelectedPosition != index1)
          AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
        this.mKeyboardSelection = false;
        this.mSelectedPosition = index1;
        for (int index2 = 0; index2 < this.mMenuItems.Count; ++index2)
        {
          this.mMenuItems[index2].Selected = index2 == index1;
          if (index2 < this.mMenuOptions.Count)
            this.mMenuOptions[index2].Selected = index2 == index1;
        }
        flag = true;
        break;
      }
    }
    if (flag)
      return;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      this.mMenuItems[index].Selected = false;
      if (index < this.mMenuOptions.Count)
        this.mMenuOptions[index].Selected = false;
    }
  }

  public override void ControllerRight(Controller iSender)
  {
    if (!this.mKeyboardSelection)
      return;
    GlobalSettings instance = GlobalSettings.Instance;
    switch (this.mSelectedPosition)
    {
      case 0:
        switch (instance.BloodAndGore)
        {
          case SettingOptions.On:
            instance.BloodAndGore = SettingOptions.Off;
            break;
          default:
            instance.BloodAndGore = SettingOptions.On;
            break;
        }
        this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.BloodAndGore));
        break;
      case 1:
        switch (instance.DamageNumbers)
        {
          case SettingOptions.On:
            instance.DamageNumbers = SettingOptions.Off;
            break;
          default:
            instance.DamageNumbers = SettingOptions.On;
            break;
        }
        this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.DamageNumbers));
        break;
      case 2:
        switch (instance.HealthBars)
        {
          case SettingOptions.On:
            instance.HealthBars = SettingOptions.Off;
            break;
          case SettingOptions.Players_Only:
            instance.HealthBars = SettingOptions.On;
            break;
          default:
            instance.HealthBars = SettingOptions.Players_Only;
            break;
        }
        this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.HealthBars));
        break;
      case 3:
        switch (instance.SpellWheel)
        {
          case SettingOptions.On:
            instance.SpellWheel = SettingOptions.Off;
            break;
          default:
            instance.SpellWheel = SettingOptions.On;
            break;
        }
        this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.SpellWheel));
        break;
    }
  }

  public override void ControllerLeft(Controller iSender)
  {
    if (!this.mKeyboardSelection)
      return;
    GlobalSettings instance = GlobalSettings.Instance;
    switch (this.mSelectedPosition)
    {
      case 0:
        switch (instance.BloodAndGore)
        {
          case SettingOptions.On:
            instance.BloodAndGore = SettingOptions.Off;
            break;
          default:
            instance.BloodAndGore = SettingOptions.On;
            break;
        }
        this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.BloodAndGore));
        break;
      case 1:
        switch (instance.DamageNumbers)
        {
          case SettingOptions.On:
            instance.DamageNumbers = SettingOptions.Off;
            break;
          default:
            instance.DamageNumbers = SettingOptions.On;
            break;
        }
        this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.DamageNumbers));
        break;
      case 2:
        switch (instance.HealthBars)
        {
          case SettingOptions.On:
            instance.HealthBars = SettingOptions.Players_Only;
            break;
          case SettingOptions.Players_Only:
            instance.HealthBars = SettingOptions.Off;
            break;
          default:
            instance.HealthBars = SettingOptions.On;
            break;
        }
        this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.HealthBars));
        break;
      case 3:
        switch (instance.SpellWheel)
        {
          case SettingOptions.On:
            instance.SpellWheel = SettingOptions.Off;
            break;
          default:
            instance.SpellWheel = SettingOptions.On;
            break;
        }
        this.mMenuOptions[this.mSelectedPosition].SetText(SubMenu.GetSettingLoc(instance.SpellWheel));
        break;
    }
  }
}
