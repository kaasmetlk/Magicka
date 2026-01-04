// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.Options.SubMenuOptionsKeyboard
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
namespace Magicka.GameLogic.GameStates.Menu.Main.Options;

internal class SubMenuOptionsKeyboard : SubMenu
{
  private const int VISIBLE_ITEMS = 12;
  private static readonly int LOC_KEYB_OPTS = "#menu_opt_11".GetHashCodeCustom();
  private static readonly int LOC_RECONFIG = "#menu_reconfig".GetHashCodeCustom();
  private static readonly int LOC_DEFAULTS = "#menu_restore_d".GetHashCodeCustom();
  private static readonly int LOC_KB_INVENTORY = "#ctrl_kb_inventory".GetHashCodeCustom();
  private static readonly int LOC_KB_BOOST = "#ctrl_kb_boost".GetHashCodeCustom();
  private static readonly int LOC_KB_CASTSELF = "#ctrl_cast_self".GetHashCodeCustom();
  private static readonly int LOC_KB_BLOCK = "#ctrl_kb_block".GetHashCodeCustom();
  private static readonly int LOC_KB_SHIFT = "#ctrl_kb_shift".GetHashCodeCustom();
  private static readonly int LOC_KB_CAST = "#ctrl_kb_cast".GetHashCodeCustom();
  private static readonly int LOC_KB_CAST_MAGICK = "#ctrl_kb_cast_magick".GetHashCodeCustom();
  private static readonly int LOC_KB_CAST_SELF = "#ctrl_kb_cast_self".GetHashCodeCustom();
  private static readonly int LOC_FIRE = "#element_fire".GetHashCodeCustom();
  private static readonly int LOC_COLD = "#element_cold".GetHashCodeCustom();
  private static readonly int LOC_WATER = "#element_water".GetHashCodeCustom();
  private static readonly int LOC_EARTH = "#element_earth".GetHashCodeCustom();
  private static readonly int LOC_ARCANE = "#element_arcane".GetHashCodeCustom();
  private static readonly int LOC_LIFE = "#element_life".GetHashCodeCustom();
  private static readonly int LOC_LIGHTNING = "#element_lightning".GetHashCodeCustom();
  private static readonly int LOC_SHIELD = "#element_shield".GetHashCodeCustom();
  private static readonly int LOC_ICE = "#element_ice".GetHashCodeCustom();
  private static readonly int LOC_STEAM = "#element_steam".GetHashCodeCustom();
  private static readonly int LOC_POISON = "#element_poison".GetHashCodeCustom();
  private static readonly int LOC_CAST_FORCE = "#menu_ctrl_01".GetHashCodeCustom();
  private static readonly int LOC_CAST_AREA = "#menu_ctrl_02".GetHashCodeCustom();
  private static readonly int LOC_CAST_SELF = "#menu_ctrl_03".GetHashCodeCustom();
  private static readonly int LOC_CAST_WEAPON = "#menu_ctrl_04".GetHashCodeCustom();
  private static readonly int LOC_CAST_MAGICK = "#menu_ctrl_05".GetHashCodeCustom();
  private static readonly int LOC_BOOST = "#menu_ctrl_06".GetHashCodeCustom();
  private static readonly int LOC_ITEM_ABILITY = "#menu_ctrl_07".GetHashCodeCustom();
  private static readonly int LOC_INTERACT = "#menu_ctrl_08".GetHashCodeCustom();
  private static readonly int LOC_ATTACK = "#menu_ctrl_09".GetHashCodeCustom();
  private static readonly int LOC_WALK = "#menu_ctrl_10".GetHashCodeCustom();
  private static readonly int LOC_CONJURE_SPELL = "#menu_ctrl_11".GetHashCodeCustom();
  private static readonly int LOC_TOME = "#menu_ctrl_12".GetHashCodeCustom();
  private static readonly int LOC_LEFT = "#menu_ctrl_13".GetHashCodeCustom();
  private static readonly int LOC_RIGHT = "#menu_ctrl_14".GetHashCodeCustom();
  private static readonly int LOC_SHOW = "#menu_ctrl_15".GetHashCodeCustom();
  private static readonly int LOC_HIDE = "#menu_ctrl_16".GetHashCodeCustom();
  private static readonly int LOC_BLOCK = "#menu_ctrl_17".GetHashCodeCustom();
  private static readonly int LOC_MOVE = "#ctrl_move".GetHashCodeCustom();
  private static readonly int LOC_CHANGEBINDING = "#opt_changebinding".GetHashCodeCustom();
  private static readonly int LOC_MAGICK_PREV = "#ctrl_magick_prev".GetHashCodeCustom();
  private static readonly int LOC_MAGICK_NEXT = "#ctrl_magick_next".GetHashCodeCustom();
  private static SubMenuOptionsKeyboard mSingelton;
  private static volatile object mSingeltonLock = new object();
  private bool mIsKeyRead;
  private bool mKeysHasChanged;
  private readonly int mSampleHash = "misc_gib".GetHashCodeCustom();
  private BitmapFont mFont;
  private List<MenuTextItem> mMenuOptions;
  private MenuScrollBar mScrollBar;
  private MenuImageTextItem mBackItem;

  public static SubMenuOptionsKeyboard Instance
  {
    get
    {
      if (SubMenuOptionsKeyboard.mSingelton == null)
      {
        lock (SubMenuOptionsKeyboard.mSingeltonLock)
        {
          if (SubMenuOptionsKeyboard.mSingelton == null)
            SubMenuOptionsKeyboard.mSingelton = new SubMenuOptionsKeyboard();
        }
      }
      return SubMenuOptionsKeyboard.mSingelton;
    }
  }

  private SubMenuOptionsKeyboard()
  {
    LanguageManager instance = LanguageManager.Instance;
    this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    this.mSelectedPosition = 0;
    this.mMenuItems = new List<MenuItem>();
    this.mMenuOptions = new List<MenuTextItem>();
    this.mMenuTitle = new Text(48 /*0x30*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(instance.GetString(SubMenuOptionsKeyboard.LOC_KEYB_OPTS));
    base.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_DEFAULTS);
    this.mMenuItems[this.mMenuItems.Count - 1].Position = new Vector2(512f, (float) ((double) this.mPosition.Y + (double) ((this.mMenuItems.Count - 1) * this.mFont.LineHeight) + 20.0));
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Water));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_WATER);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Life));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_LIFE);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Shield));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_SHIELD);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Cold));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_COLD);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Lightning));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_LIGHTNING);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Arcane));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_ARCANE);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Earth));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_EARTH);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Fire));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_FIRE);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Cast));
    this.AddMenuTextItem(instance.GetString(SubMenuOptionsKeyboard.LOC_CAST_FORCE) + (object) '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_CAST_AREA));
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.CastSelf));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_CAST_SELF);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Boost));
    this.AddMenuTextItem(instance.GetString(SubMenuOptionsKeyboard.LOC_KB_BOOST) + (object) '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_KB_CAST_MAGICK));
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Move));
    this.AddMenuTextItem(instance.GetString(SubMenuOptionsKeyboard.LOC_MOVE) + (object) '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_ATTACK));
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Block));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_KB_BLOCK);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Shift));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_KB_SHIFT);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.Inventory));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_KB_INVENTORY);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.PrevMagick));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_MAGICK_PREV);
    this.AddMenuOptions(KeyboardMouseController.KeyToString(KeyboardBindings.NextMagick));
    this.AddMenuTextItem(SubMenuOptionsKeyboard.LOC_MAGICK_NEXT);
    this.mBackItem = new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE);
    this.mScrollBar = new MenuScrollBar(new Vector2(928f, 512f), (float) (this.mFont.LineHeight * 13), 5);
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsKeyboard.LOC_KEYB_OPTS));
    LanguageManager instance = LanguageManager.Instance;
    (this.mMenuItems[9] as MenuTextItem).SetText(instance.GetString(SubMenuOptionsKeyboard.LOC_CAST_FORCE) + (object) '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_CAST_AREA));
    (this.mMenuItems[11] as MenuTextItem).SetText(instance.GetString(SubMenuOptionsKeyboard.LOC_KB_BOOST) + (object) '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_KB_CAST_MAGICK));
    (this.mMenuItems[12] as MenuTextItem).SetText(instance.GetString(SubMenuOptionsKeyboard.LOC_MOVE) + (object) '/' + instance.GetString(SubMenuOptionsKeyboard.LOC_ATTACK));
    this.UpdateMenuOptions();
  }

  public override MenuTextItem AddMenuTextItem(int iText)
  {
    Vector2 mPosition = this.mPosition;
    mPosition.X -= 20f;
    mPosition.Y += (float) (this.mFont.LineHeight * this.mMenuItems.Count);
    MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, this.mFont, TextAlign.Right);
    this.mMenuItems.Add((MenuItem) menuTextItem);
    return menuTextItem;
  }

  public void AddMenuTextItem(string iText)
  {
    Vector2 mPosition = this.mPosition;
    mPosition.X -= 20f;
    mPosition.Y += (float) (this.mFont.LineHeight * this.mMenuItems.Count);
    this.mMenuItems.Add((MenuItem) new MenuTextItem(iText, mPosition, this.mFont, TextAlign.Right));
  }

  public void AddMenuOptions(string iText)
  {
    Vector2 mPosition = this.mPosition;
    mPosition.X += 20f;
    mPosition.Y += (float) (this.mFont.LineHeight * this.mMenuItems.Count);
    this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, this.mFont, TextAlign.Left, false));
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.mEffect.VertexColorEnabled = false;
    this.mEffect.Color = MenuItem.COLOR;
    this.mMenuTitle.Draw(this.mEffect, 512f, 96f);
    this.mScrollBar.Draw(this.mEffect);
    this.mBackItem.Selected = this.mSelectedPosition == this.mMenuItems.Count;
    this.mBackItem.Draw(this.mEffect);
    this.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 608, 48 /*0x30*/), new Rectangle(208 /*0xD0*/, 220, 608, 48 /*0x30*/));
    float lineHeight = (float) this.mFont.LineHeight;
    float num = this.mPosition.Y - lineHeight * 0.5f;
    Vector2 position;
    for (int index = this.mScrollBar.Value + 1; index < this.mScrollBar.Value + 12 + 1; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Selected = index == this.mSelectedPosition;
      position = mMenuItem.Position with { Y = num };
      mMenuItem.Position = position;
      mMenuItem.Draw(this.mEffect);
      MenuItem mMenuOption = (MenuItem) this.mMenuOptions[index - 1];
      mMenuOption.Selected = index == this.mSelectedPosition;
      position = mMenuOption.Position with { Y = num };
      mMenuOption.Position = position;
      mMenuOption.Draw(this.mEffect);
      num += lineHeight;
    }
    MenuItem mMenuItem1 = this.mMenuItems[0];
    mMenuItem1.Selected = this.mSelectedPosition == 0;
    position = mMenuItem1.Position with { Y = num + 20f };
    mMenuItem1.Position = position;
    mMenuItem1.Draw(this.mEffect);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    for (int index = 0; index < this.mMenuItems.Count; ++index)
      (this.mMenuItems[index] as MenuTextItem).MaxWidth = 400f;
    if (this.mKeyboardSelection)
    {
      for (int index = 0; index < this.mMenuItems.Count; ++index)
      {
        this.mMenuItems[index].Selected = index == this.mSelectedPosition;
        if (index < this.mMenuOptions.Count)
          this.mMenuOptions[index].Selected = index == this.mSelectedPosition;
      }
    }
    if (!(this.mIsKeyRead & !KeyboardMouseController.mCatchKeyActive))
      return;
    this.UpdateMenuOptions();
    this.mIsKeyRead = false;
    this.mKeysHasChanged = true;
  }

  protected override void ControllerMouseClicked(Controller iSender)
  {
    if (this.mSelectedPosition == this.mMenuItems.Count)
    {
      Tome.Instance.PopMenu();
    }
    else
    {
      this.mKeyboardSelection = true;
      this.ControllerRight(iSender);
      this.mKeyboardSelection = false;
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
    if (this.mIsKeyRead || !Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) || !oRightPageHit)
      return;
    if (this.mScrollBar.Grabbed)
    {
      if (this.mScrollBar.InsideDragUpBounds(oHitPosition))
      {
        --this.mScrollBar.Value;
      }
      else
      {
        if (!this.mScrollBar.InsideDragDownBounds(oHitPosition))
          return;
        ++this.mScrollBar.Value;
      }
    }
    else
    {
      int num = -1;
      if (this.mMenuItems[0].InsideBounds(ref oHitPosition))
        num = 0;
      if (this.mBackItem.InsideBounds(ref oHitPosition))
        this.mSelectedPosition = this.mMenuItems.Count;
      for (int index = this.mScrollBar.Value + 1; index < this.mScrollBar.Value + 1 + 12; ++index)
      {
        MenuItem mMenuItem = this.mMenuItems[index];
        MenuItem menuItem = (MenuItem) null;
        if (index > 0 && index <= this.mMenuOptions.Count)
          menuItem = (MenuItem) this.mMenuOptions[index - 1];
        if (mMenuItem.Enabled && (mMenuItem.InsideBounds(ref oHitPosition) || menuItem != null && menuItem.InsideBounds(ref oHitPosition)))
        {
          num = index;
          break;
        }
      }
      if (num >= 0)
      {
        if (this.mSelectedPosition != num)
          AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
        this.mKeyboardSelection = false;
      }
      this.mSelectedPosition = num;
    }
  }

  public override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (this.mIsKeyRead)
      return;
    if (iState.LeftButton == ButtonState.Released)
      this.mScrollBar.Grabbed = false;
    if (iState.ScrollWheelValue > iOldState.ScrollWheelValue)
      --this.mScrollBar.Value;
    else if (iState.ScrollWheelValue < iOldState.ScrollWheelValue)
    {
      ++this.mScrollBar.Value;
    }
    else
    {
      Vector2 oHitPosition;
      bool oRightPageHit;
      if (!Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) || !oRightPageHit)
        return;
      if (iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released)
      {
        if (this.mScrollBar.InsideBounds(ref oHitPosition))
        {
          if (this.mScrollBar.InsideDragBounds(oHitPosition))
            this.mScrollBar.Grabbed = true;
          else if (this.mScrollBar.InsideUpBounds(oHitPosition))
            --this.mScrollBar.Value;
          else if (this.mScrollBar.InsideDownBounds(oHitPosition))
            ++this.mScrollBar.Value;
          else
            this.mScrollBar.ScrollTo(oHitPosition.Y);
        }
        else
        {
          if (!this.mBackItem.Enabled || !this.mBackItem.InsideBounds(ref oHitPosition))
            return;
          Tome.Instance.PopMenu();
        }
      }
      else
      {
        if (iState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed || this.mSelectedPosition < 0)
          return;
        MenuItem mMenuItem = this.mMenuItems[this.mSelectedPosition];
        MenuItem menuItem = (MenuItem) null;
        if (this.mSelectedPosition > 0 && this.mSelectedPosition <= this.mMenuOptions.Count)
          menuItem = (MenuItem) this.mMenuOptions[this.mSelectedPosition - 1];
        if (!mMenuItem.Enabled || !mMenuItem.InsideBounds(ref oHitPosition) && (menuItem == null || !menuItem.InsideBounds(ref oHitPosition)))
          return;
        this.ControllerMouseClicked(iSender);
      }
    }
  }

  public override void ControllerA(Controller iSender)
  {
    if (this.mSelectedPosition == this.mMenuItems.Count)
    {
      Tome.Instance.PopMenu();
    }
    else
    {
      base.ControllerA(iSender);
      this.ControllerRight(iSender);
    }
  }

  public override void ControllerB(Controller iSender)
  {
    if (this.mIsKeyRead)
      return;
    base.ControllerB(iSender);
  }

  public override void ControllerUp(Controller iSender)
  {
    if (this.mSelectedPosition == 1)
      this.mSelectedPosition = this.mMenuItems.Count;
    else if (this.mSelectedPosition == this.mMenuItems.Count)
      this.mSelectedPosition = 0;
    else if (this.mSelectedPosition == 0)
    {
      this.mSelectedPosition = this.mMenuItems.Count - 1;
      this.mScrollBar.Value = this.mScrollBar.MaxValue;
    }
    else
    {
      base.ControllerUp(iSender);
      if (this.mSelectedPosition <= 0 || this.mSelectedPosition > this.mMenuOptions.Count)
        return;
      while (this.mScrollBar.Value + 1 > this.mSelectedPosition)
        --this.mScrollBar.Value;
      while (this.mScrollBar.Value + 12 < this.mSelectedPosition)
        ++this.mScrollBar.Value;
    }
  }

  public override void ControllerDown(Controller iSender)
  {
    if (this.mSelectedPosition == 0)
      this.mSelectedPosition = this.mMenuItems.Count;
    else if (this.mSelectedPosition == this.mMenuItems.Count)
    {
      this.mSelectedPosition = 1;
      this.mScrollBar.Value = 0;
    }
    else
    {
      base.ControllerDown(iSender);
      if (this.mSelectedPosition <= 0 || this.mSelectedPosition > this.mMenuOptions.Count)
        return;
      while (this.mScrollBar.Value + 1 > this.mSelectedPosition)
        --this.mScrollBar.Value;
      while (this.mScrollBar.Value + 12 < this.mSelectedPosition)
        ++this.mScrollBar.Value;
    }
  }

  public override void ControllerRight(Controller iSender)
  {
    if (!this.mKeyboardSelection)
      return;
    if (this.mSelectedPosition == 0)
    {
      KeyboardMouseController.LoadDefaults();
      this.UpdateMenuOptions();
      this.mKeysHasChanged = true;
    }
    else
    {
      if (this.mSelectedPosition <= 0 || this.mSelectedPosition > 17)
        return;
      this.ActivateCatchKeyIndicator((KeyboardBindings) (this.mSelectedPosition - 1));
    }
  }

  public override void OnEnter()
  {
    base.OnEnter();
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenuOptionsKeyboard.LOC_CHANGEBINDING);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
  }

  public override void OnExit()
  {
    this.UpdateMenuOptions();
    if (this.mKeysHasChanged)
    {
      KeyboardMouseController.mKeyboardBindings.CopyTo((Array) SaveManager.Instance.KeyBindings, 0);
      SaveManager.Instance.SaveSettings();
      KeyboardHUD.Instance.UpdateControls();
    }
    this.mKeysHasChanged = false;
    this.mIsKeyRead = false;
  }

  public void UpdateMenuOptions()
  {
    this.mSelectedPosition = -1;
    for (int index = 0; index < 17; ++index)
      this.mMenuOptions[index].SetText(KeyboardMouseController.KeyToString((KeyboardBindings) index));
  }

  public void ActivateCatchKeyIndicator(KeyboardBindings iKeyBinding)
  {
    this.mIsKeyRead = true;
    KeyboardMouseController.mCatchKeyActive = true;
    KeyboardMouseController.mCatchKeyIndex = iKeyBinding;
    this.mMenuOptions[(int) iKeyBinding].SetText(KeyboardMouseController.KeyToString(iKeyBinding) + " ???");
  }
}
