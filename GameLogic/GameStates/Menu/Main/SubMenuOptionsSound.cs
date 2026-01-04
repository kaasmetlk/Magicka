// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuOptionsSound
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

internal class SubMenuOptionsSound : SubMenu
{
  private static readonly int LOC_MUSIC = "#menu_opt_sfx_01".GetHashCodeCustom();
  private static readonly int LOC_SFX = "#menu_opt_sfx_02".GetHashCodeCustom();
  private static readonly int LOC_UI = "#menu_opt_sfx_03".GetHashCodeCustom();
  private static readonly int LOC_DIALOGUES = "#menu_opt_sfx_04".GetHashCodeCustom();
  private static readonly int LOC_SOUND_OPTS = "#menu_opt_08".GetHashCodeCustom();
  private static SubMenuOptionsSound mSingelton;
  private static volatile object mSingeltonLock = new object();
  private int mMusicVolume = AudioManager.Instance.VolumeMusic();
  private int mSFXVolume = AudioManager.Instance.VolumeSound();
  private readonly int mSampleHash = "misc_thud01".GetHashCodeCustom();
  private MenuScrollSlider mMusicScrollSlider;
  private MenuScrollSlider mSFXScrollSlider;
  private int mMusicLevel;
  private int mSfxLevel;

  public static SubMenuOptionsSound Instance
  {
    get
    {
      if (SubMenuOptionsSound.mSingelton == null)
      {
        lock (SubMenuOptionsSound.mSingeltonLock)
        {
          if (SubMenuOptionsSound.mSingelton == null)
            SubMenuOptionsSound.mSingelton = new SubMenuOptionsSound();
        }
      }
      return SubMenuOptionsSound.mSingelton;
    }
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsSound.LOC_SOUND_OPTS));
  }

  private SubMenuOptionsSound()
  {
    this.mSelectedPosition = 0;
    this.mMenuItems = new List<MenuItem>();
    this.mMenuTitle = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsSound.LOC_SOUND_OPTS));
    this.mPosition.X -= 40f;
    float num = Math.Min(1f, (float) (((double) FontManager.Instance.GetFont(MagickaFont.MenuOption).LineHeight + 10.0) / 64.0));
    this.AddMenuTextItem(SubMenuOptionsSound.LOC_MUSIC);
    Vector2 position1 = this.mMenuItems[0].Position;
    position1.X += 200f;
    this.mMusicScrollSlider = new MenuScrollSlider(position1, 320f, 10);
    this.mMusicScrollSlider.Scale = num;
    this.mMusicScrollSlider.Value = this.mMusicVolume;
    this.AddMenuTextItem(SubMenuOptionsSound.LOC_SFX);
    Vector2 position2 = this.mMenuItems[1].Position;
    position2.X += 200f;
    this.mSFXScrollSlider = new MenuScrollSlider(position2, 320f, 10);
    this.mSFXScrollSlider.Scale = num;
    this.mSFXScrollSlider.Value = this.mSFXVolume;
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
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

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    base.Draw(iLeftSide, iRightSide);
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48 /*0x30*/), new Rectangle(208 /*0xD0*/, 220, 608, 48 /*0x30*/));
    this.mMusicScrollSlider.Draw(this.mEffect);
    this.mSFXScrollSlider.Draw(this.mEffect);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public override void ControllerA(Controller iSender)
  {
    if (this.mSelectedPosition == 2)
      Tome.Instance.PopMenu();
    else
      this.ControllerRight(iSender);
  }

  public override void ControllerRight(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 0:
        ++this.mMusicVolume;
        if (this.mMusicVolume > 10)
          this.mMusicVolume = 10;
        this.mMusicScrollSlider.Value = this.mMusicVolume;
        AudioManager.Instance.VolumeMusic(this.mMusicVolume);
        break;
      case 1:
        ++this.mSFXVolume;
        if (this.mSFXVolume > 10)
          this.mSFXVolume = 10;
        this.mSFXScrollSlider.Value = this.mSFXVolume;
        AudioManager.Instance.VolumeSound(this.mSFXVolume);
        AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
        break;
    }
  }

  public override void ControllerLeft(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case 0:
        --this.mMusicVolume;
        if (this.mMusicVolume < 0)
          this.mMusicVolume = 0;
        this.mMusicScrollSlider.Value = this.mMusicVolume;
        AudioManager.Instance.VolumeMusic(this.mMusicVolume);
        break;
      case 1:
        --this.mSFXVolume;
        if (this.mSFXVolume < 0)
          this.mSFXVolume = 0;
        this.mSFXScrollSlider.Value = this.mSFXVolume;
        AudioManager.Instance.VolumeSound(this.mSFXVolume);
        AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
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
    if (this.mMenuItems == null || this.mMenuItems.Count == 0 || !Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) || !oRightPageHit)
      return;
    if (this.mMusicScrollSlider.InsideBounds(ref oHitPosition))
    {
      if (iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released)
      {
        if (this.mMusicScrollSlider.InsideDragBounds(oHitPosition))
          this.mMusicScrollSlider.Grabbed = true;
        else if (this.mMusicScrollSlider.InsideLeftBounds(oHitPosition))
        {
          --this.mMusicScrollSlider.Value;
          this.mMusicVolume = this.mMusicScrollSlider.Value;
          AudioManager.Instance.VolumeMusic(this.mMusicVolume);
        }
        else if (this.mMusicScrollSlider.InsideRightBounds(oHitPosition))
        {
          ++this.mMusicScrollSlider.Value;
          this.mMusicVolume = this.mMusicScrollSlider.Value;
          AudioManager.Instance.VolumeMusic(this.mMusicVolume);
        }
        else
        {
          this.mMusicScrollSlider.ScrollTo(oHitPosition.X);
          this.mMusicVolume = this.mMusicScrollSlider.Value;
          AudioManager.Instance.VolumeMusic(this.mMusicVolume);
        }
      }
      else if (this.mMusicScrollSlider.Value > 0 && iState.ScrollWheelValue > iOldState.ScrollWheelValue)
      {
        --this.mMusicScrollSlider.Value;
        this.mMusicVolume = this.mMusicScrollSlider.Value;
        AudioManager.Instance.VolumeMusic(this.mMusicVolume);
      }
      else
      {
        if (this.mMusicScrollSlider.Value >= 10 || iState.ScrollWheelValue >= iOldState.ScrollWheelValue)
          return;
        ++this.mMusicScrollSlider.Value;
        this.mMusicVolume = this.mMusicScrollSlider.Value;
        AudioManager.Instance.VolumeMusic(this.mMusicVolume);
      }
    }
    else if (this.mSFXScrollSlider.InsideBounds(ref oHitPosition))
    {
      if (iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released)
      {
        if (this.mSFXScrollSlider.InsideDragBounds(oHitPosition))
          this.mSFXScrollSlider.Grabbed = true;
        else if (this.mSFXScrollSlider.InsideLeftBounds(oHitPosition))
        {
          --this.mSFXScrollSlider.Value;
          this.mSFXVolume = this.mSFXScrollSlider.Value;
          AudioManager.Instance.VolumeSound(this.mSFXVolume);
          AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
        }
        else if (this.mSFXScrollSlider.InsideRightBounds(oHitPosition))
        {
          ++this.mSFXScrollSlider.Value;
          this.mSFXVolume = this.mSFXScrollSlider.Value;
          AudioManager.Instance.VolumeSound(this.mSFXVolume);
          AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
        }
        else
        {
          this.mSFXScrollSlider.ScrollTo(oHitPosition.X);
          this.mSFXVolume = this.mSFXScrollSlider.Value;
          AudioManager.Instance.VolumeSound(this.mSFXVolume);
          AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
        }
      }
      else if (this.mSFXScrollSlider.Value > 0 && iState.ScrollWheelValue > iOldState.ScrollWheelValue)
      {
        --this.mSFXScrollSlider.Value;
        this.mSFXVolume = this.mSFXScrollSlider.Value;
        AudioManager.Instance.VolumeSound(this.mSFXVolume);
        AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
      }
      else
      {
        if (this.mSFXScrollSlider.Value >= 10 || iState.ScrollWheelValue >= iOldState.ScrollWheelValue)
          return;
        ++this.mSFXScrollSlider.Value;
        this.mSFXVolume = this.mSFXScrollSlider.Value;
        AudioManager.Instance.VolumeSound(this.mSFXVolume);
        AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
      }
    }
    else
    {
      if (!this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref oHitPosition))
        return;
      Tome.Instance.PopMenu();
    }
  }

  public override void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (this.mMenuItems == null || this.mMenuItems.Count == 0)
      return;
    if (iState.LeftButton == ButtonState.Released)
    {
      this.mSFXScrollSlider.Grabbed = false;
      this.mMusicScrollSlider.Grabbed = false;
    }
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit))
    {
      if (!oRightPageHit)
        return;
      if (this.mMusicScrollSlider.Grabbed)
      {
        this.mMusicScrollSlider.ScrollTo(oHitPosition.X);
        this.mMusicVolume = this.mMusicScrollSlider.Value;
        AudioManager.Instance.VolumeMusic(this.mMusicVolume);
      }
      else if (this.mSFXScrollSlider.Grabbed)
      {
        int num = this.mSFXScrollSlider.Value;
        this.mSFXScrollSlider.ScrollTo(oHitPosition.X);
        if (num == this.mSFXScrollSlider.Value)
          return;
        this.mSFXVolume = this.mSFXScrollSlider.Value;
        AudioManager.Instance.VolumeSound(this.mSFXVolume);
        AudioManager.Instance.PlayCue(Banks.Misc, this.mSampleHash);
      }
      else
      {
        bool flag = false;
        for (int index1 = 0; index1 < this.mMenuItems.Count; ++index1)
        {
          MenuItem mMenuItem = this.mMenuItems[index1];
          if (mMenuItem.Enabled && mMenuItem.InsideBounds(ref oHitPosition))
          {
            if (this.mSelectedPosition != index1)
              AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
            this.mKeyboardSelection = false;
            this.mSelectedPosition = index1;
            for (int index2 = 0; index2 < this.mMenuItems.Count; ++index2)
              this.mMenuItems[index2].Selected = false;
            mMenuItem.Selected = true;
            flag = true;
            break;
          }
        }
        if (flag)
          return;
        for (int index = 0; index < this.mMenuItems.Count; ++index)
          this.mMenuItems[index].Selected = false;
      }
    }
    else
    {
      if (this.mKeyboardSelection)
        return;
      for (int index = 0; index < this.mMenuItems.Count; ++index)
        this.mMenuItems[index].Selected = false;
    }
  }

  public override void OnEnter()
  {
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    this.mMusicLevel = AudioManager.Instance.VolumeMusic();
    this.mSfxLevel = AudioManager.Instance.VolumeSound();
  }

  public override void OnExit()
  {
    GlobalSettings.Instance.VolumeMusic = this.mMusicVolume;
    GlobalSettings.Instance.VolumeSound = this.mSFXVolume;
    if (!(this.mMusicLevel != this.mMusicVolume | this.mSfxLevel != this.mSFXVolume))
      return;
    SaveManager.Instance.SaveSettings();
  }
}
