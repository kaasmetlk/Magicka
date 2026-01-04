// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuOptionsGraphics
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
using System.Windows.Forms;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuOptionsGraphics : SubMenu
{
  private static readonly int LOC_WINDOWED = "#menu_opt_gfx_01".GetHashCodeCustom();
  private static readonly int LOC_RESOLUTION = "#menu_opt_gfx_02".GetHashCodeCustom();
  private static readonly int LOC_REFRESH_RATE = "#menu_opt_gfx_03".GetHashCodeCustom();
  private static readonly int LOC_SHADOWS = "#menu_opt_gfx_04".GetHashCodeCustom();
  private static readonly int LOC_BLOOM = "#menu_opt_gfx_05".GetHashCodeCustom();
  private static readonly int LOC_AMBIENT_OCCLUSION = "#menu_opt_gfx_06".GetHashCodeCustom();
  private static readonly int LOC_OVERSCAN_COMPENSATION = "#menu_opt_gfx_07".GetHashCodeCustom();
  private static readonly int LOC_BRIGHTNESS = "#menu_opt_gfx_08".GetHashCodeCustom();
  private static readonly int LOC_ENVIRONMENT_DYNAMICS = "#menu_opt_gfx_09".GetHashCodeCustom();
  private static readonly int LOC_RESOLUTIONS = "#menu_opt_gfx_10".GetHashCodeCustom();
  private static readonly int LOC_WIDTH = "#menu_opt_gfx_11".GetHashCodeCustom();
  private static readonly int LOC_HEIGHT = "#menu_opt_gfx_12".GetHashCodeCustom();
  private static readonly int LOC_DECAL_LIMIT = "#menu_opt_gfx_13".GetHashCodeCustom();
  private static readonly int LOC_PARTICLES = "#menu_opt_gfx_particles".GetHashCodeCustom();
  private static readonly int LOC_PARTICLELIGHTS = "#menu_opt_gfx_particlelights".GetHashCodeCustom();
  private static readonly int LOC_VSYNC = "#menu_vsync".GetHashCodeCustom();
  private static readonly int LOC_GFX_OPTS = "#menu_opt_10".GetHashCodeCustom();
  private static SubMenuOptionsGraphics mSingelton;
  private static volatile object mSingeltonLock = new object();
  private List<MenuTextItem> mMenuOptions;
  private bool mWindowed;
  private SettingOptions mShadowQuality;
  private SettingOptions mDecalLimit;

  public static SubMenuOptionsGraphics Instance
  {
    get
    {
      if (SubMenuOptionsGraphics.mSingelton == null)
      {
        lock (SubMenuOptionsGraphics.mSingeltonLock)
        {
          if (SubMenuOptionsGraphics.mSingelton == null)
            SubMenuOptionsGraphics.mSingelton = new SubMenuOptionsGraphics();
        }
      }
      return SubMenuOptionsGraphics.mSingelton;
    }
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    for (int index = 0; index < this.mMenuOptions.Count; ++index)
      this.mMenuOptions[index].LanguageChanged();
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGraphics.LOC_GFX_OPTS));
  }

  public SubMenuOptionsGraphics()
  {
    this.mSelectedPosition = 0;
    this.mMenuItems = new List<Magicka.GameLogic.GameStates.Menu.MenuItem>();
    this.mMenuTitle = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGraphics.LOC_GFX_OPTS));
    this.mMenuOptions = new List<MenuTextItem>();
    this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_WINDOWED);
    this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_RESOLUTION);
    this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_SHADOWS);
    this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_DECAL_LIMIT);
    this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_PARTICLES);
    this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_PARTICLELIGHTS);
    this.AddMenuTextItem(SubMenuOptionsGraphics.LOC_VSYNC);
    if (GlobalSettings.Instance.Fullscreen)
      this.AddMenuOptions(SubMenu.LOC_OFF);
    else
      this.AddMenuOptions(SubMenu.LOC_ON);
    this.AddMenuOptions($"{GlobalSettings.Instance.Resolution.Width} x {GlobalSettings.Instance.Resolution.Height}");
    this.AddMenuOptions(SubMenu.GetSettingLoc(GlobalSettings.Instance.ShadowQuality));
    this.AddMenuOptions(SubMenu.GetSettingLoc(GlobalSettings.Instance.DecalLimit));
    this.AddMenuOptions(SubMenu.GetSettingLoc(GlobalSettings.Instance.Particles));
    this.AddMenuOptions(SubMenu.GetSettingLoc(GlobalSettings.Instance.ParticleLights));
    this.AddMenuOptions(GlobalSettings.Instance.VSync ? SubMenu.LOC_ON : SubMenu.LOC_OFF);
    this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
    ResolutionMessageBox.Instance.Complete += new Action<ResolutionData>(this.SetResolution);
  }

  private void SetResolution(ResolutionData iData)
  {
    this.mMenuOptions[1].SetText($"{iData.Width} x {iData.Height}");
    GlobalSettings.Instance.Resolution = iData;
  }

  public override MenuTextItem AddMenuTextItem(int iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuItems.Count;
    MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, font, TextAlign.Right);
    this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) menuTextItem);
    return menuTextItem;
  }

  public void AddMenuOptions(string iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.X += 40f;
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuOptions.Count;
    this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left));
  }

  public void AddMenuOptions(int iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.X += 40f;
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuOptions.Count;
    this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left));
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

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    base.Draw(iLeftSide, iRightSide);
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48 /*0x30*/), new Rectangle(208 /*0xD0*/, 220, 608, 48 /*0x30*/));
    this.DrawGraphics(SubMenu.sStainsTexture, new Rectangle(0, 0, 256 /*0x0100*/, 256 /*0x0100*/), new Rectangle(670, 650, 256 /*0x0100*/, 256 /*0x0100*/));
    this.DrawGraphics(SubMenu.sStainsTexture, new Rectangle(0, 256 /*0x0100*/, 128 /*0x80*/, 128 /*0x80*/), new Rectangle(120, 200, 128 /*0x80*/, 128 /*0x80*/));
    foreach (Magicka.GameLogic.GameStates.Menu.MenuItem mMenuOption in this.mMenuOptions)
      mMenuOption.Draw(this.mEffect);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public override void ControllerA(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 1:
        ResolutionMessageBox.Instance.Show();
        break;
      case 7:
        Tome.Instance.PopMenu();
        break;
      default:
        this.ControllerRight(iSender);
        break;
    }
  }

  public override void ControllerRight(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 0:
        GlobalSettings.Instance.Fullscreen = !GlobalSettings.Instance.Fullscreen;
        if (GlobalSettings.Instance.Fullscreen)
        {
          this.mMenuOptions[0].SetText(LanguageManager.Instance.GetString(SubMenu.LOC_OFF));
          Tome.ChangeResolution(GlobalSettings.Instance.Resolution with
          {
            Width = Screen.PrimaryScreen.Bounds.Width,
            Height = Screen.PrimaryScreen.Bounds.Height
          });
          break;
        }
        this.mMenuOptions[0].SetText(LanguageManager.Instance.GetString(SubMenu.LOC_ON));
        Tome.ChangeResolution(GlobalSettings.Instance.Resolution);
        int height = RenderManager.Instance.GraphicsDevice.DisplayMode.Height;
        break;
      case 2:
        switch (GlobalSettings.Instance.ShadowQuality)
        {
          case SettingOptions.Medium:
            GlobalSettings.Instance.ShadowQuality = SettingOptions.High;
            break;
          case SettingOptions.High:
            GlobalSettings.Instance.ShadowQuality = SettingOptions.Low;
            break;
          default:
            GlobalSettings.Instance.ShadowQuality = SettingOptions.Medium;
            break;
        }
        this.mMenuOptions[2].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.ShadowQuality));
        break;
      case 3:
        switch (GlobalSettings.Instance.DecalLimit)
        {
          case SettingOptions.Medium:
            GlobalSettings.Instance.DecalLimit = SettingOptions.High;
            break;
          case SettingOptions.High:
            GlobalSettings.Instance.DecalLimit = SettingOptions.Low;
            break;
          default:
            GlobalSettings.Instance.DecalLimit = SettingOptions.Medium;
            break;
        }
        this.mMenuOptions[3].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.DecalLimit));
        break;
      case 4:
        switch (GlobalSettings.Instance.Particles)
        {
          case SettingOptions.Medium:
            GlobalSettings.Instance.Particles = SettingOptions.High;
            break;
          case SettingOptions.High:
            GlobalSettings.Instance.Particles = SettingOptions.Low;
            break;
          default:
            GlobalSettings.Instance.Particles = SettingOptions.Medium;
            break;
        }
        this.mMenuOptions[4].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.Particles));
        break;
      case 5:
        GlobalSettings.Instance.ParticleLights = !GlobalSettings.Instance.ParticleLights;
        this.mMenuOptions[5].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.ParticleLights));
        break;
      case 6:
        GlobalSettings.Instance.VSync = !GlobalSettings.Instance.VSync;
        SaveManager.Instance.SaveSettings();
        this.mMenuOptions[6].SetText(GlobalSettings.Instance.VSync ? SubMenu.LOC_ON : SubMenu.LOC_OFF);
        break;
    }
  }

  public override void ControllerLeft(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 0:
        GlobalSettings.Instance.Fullscreen = !GlobalSettings.Instance.Fullscreen;
        if (GlobalSettings.Instance.Fullscreen)
        {
          this.mMenuOptions[0].SetText(LanguageManager.Instance.GetString(SubMenu.LOC_OFF));
          break;
        }
        this.mMenuOptions[0].SetText(LanguageManager.Instance.GetString(SubMenu.LOC_ON));
        break;
      case 2:
        switch (GlobalSettings.Instance.ShadowQuality)
        {
          case SettingOptions.Low:
            GlobalSettings.Instance.ShadowQuality = SettingOptions.High;
            break;
          case SettingOptions.Medium:
            GlobalSettings.Instance.ShadowQuality = SettingOptions.Low;
            break;
          default:
            GlobalSettings.Instance.ShadowQuality = SettingOptions.Medium;
            break;
        }
        this.mMenuOptions[2].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.ShadowQuality));
        break;
      case 3:
        switch (GlobalSettings.Instance.DecalLimit)
        {
          case SettingOptions.Low:
            GlobalSettings.Instance.DecalLimit = SettingOptions.Off;
            break;
          case SettingOptions.Medium:
            GlobalSettings.Instance.DecalLimit = SettingOptions.Low;
            break;
          case SettingOptions.High:
            GlobalSettings.Instance.DecalLimit = SettingOptions.Medium;
            break;
          default:
            GlobalSettings.Instance.DecalLimit = SettingOptions.High;
            break;
        }
        this.mMenuOptions[3].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.DecalLimit));
        break;
      case 4:
        switch (GlobalSettings.Instance.Particles)
        {
          case SettingOptions.Medium:
            GlobalSettings.Instance.Particles = SettingOptions.Low;
            break;
          case SettingOptions.High:
            GlobalSettings.Instance.Particles = SettingOptions.Medium;
            break;
          default:
            GlobalSettings.Instance.Particles = SettingOptions.High;
            break;
        }
        this.mMenuOptions[4].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.Particles));
        break;
      case 5:
        GlobalSettings.Instance.ParticleLights = !GlobalSettings.Instance.ParticleLights;
        this.mMenuOptions[5].SetText(SubMenu.GetSettingString(GlobalSettings.Instance.ParticleLights));
        break;
      case 6:
        Magicka.Game.Instance.GraphicsManager.SynchronizeWithVerticalRetrace = !Magicka.Game.Instance.GraphicsManager.SynchronizeWithVerticalRetrace;
        this.mMenuOptions[6].SetText(Magicka.Game.Instance.GraphicsManager.SynchronizeWithVerticalRetrace ? SubMenu.LOC_OFF : SubMenu.LOC_ON);
        break;
    }
  }

  public override void OnEnter()
  {
    this.mMenuOptions[1].SetText($"{RenderManager.Instance.ScreenSize.X} x {RenderManager.Instance.ScreenSize.Y}");
    this.mShadowQuality = GlobalSettings.Instance.ShadowQuality;
    this.mDecalLimit = GlobalSettings.Instance.DecalLimit;
    this.mWindowed = GlobalSettings.Instance.Fullscreen;
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
  }

  public override void OnExit()
  {
    base.OnExit();
    if (!(this.mWindowed != GlobalSettings.Instance.Fullscreen | this.mDecalLimit != GlobalSettings.Instance.DecalLimit | this.mShadowQuality != GlobalSettings.Instance.ShadowQuality))
      return;
    SaveManager.Instance.SaveSettings();
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
      Magicka.GameLogic.GameStates.Menu.MenuItem mMenuItem = this.mMenuItems[index];
      Magicka.GameLogic.GameStates.Menu.MenuItem menuItem = (Magicka.GameLogic.GameStates.Menu.MenuItem) null;
      if (index < this.mMenuOptions.Count)
        menuItem = (Magicka.GameLogic.GameStates.Menu.MenuItem) this.mMenuOptions[index];
      if (mMenuItem.Enabled && (mMenuItem.InsideBounds(ref oHitPosition) || menuItem != null && menuItem.InsideBounds(ref oHitPosition)))
      {
        this.mSelectedPosition = index;
        if (iState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Released || iOldState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)
          break;
        this.ControllerMouseClicked(iSender);
        break;
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
    if (!Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) || !oRightPageHit)
      return;
    bool flag = false;
    for (int index1 = 0; index1 < this.mMenuItems.Count; ++index1)
    {
      Magicka.GameLogic.GameStates.Menu.MenuItem mMenuItem = this.mMenuItems[index1];
      Magicka.GameLogic.GameStates.Menu.MenuItem menuItem = (Magicka.GameLogic.GameStates.Menu.MenuItem) null;
      if (index1 < this.mMenuOptions.Count)
        menuItem = (Magicka.GameLogic.GameStates.Menu.MenuItem) this.mMenuOptions[index1];
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
}
