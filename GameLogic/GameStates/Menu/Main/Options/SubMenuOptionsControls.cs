// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.Options.SubMenuOptionsControls
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

internal class SubMenuOptionsControls : SubMenu
{
  private const int VISIBLE_ITEMS = 10;
  private static readonly int LOC_CTRL_OPTS = "#menu_opt_07".GetHashCodeCustom();
  private static readonly int LOC_KEYBOARD_AND_MOUSE = "#menu_opt_alt_07".GetHashCodeCustom();
  private static readonly int LOC_XINPUT = "#menu_xinput_gamep".GetHashCodeCustom();
  private static SubMenuOptionsControls mSingelton;
  private static volatile object mSingeltonLock = new object();
  private List<Guid> mControllers = new List<Guid>();
  private MenuScrollBar mScrollBar;

  public static SubMenuOptionsControls Instance
  {
    get
    {
      if (SubMenuOptionsControls.mSingelton == null)
      {
        lock (SubMenuOptionsControls.mSingeltonLock)
        {
          if (SubMenuOptionsControls.mSingelton == null)
            SubMenuOptionsControls.mSingelton = new SubMenuOptionsControls();
        }
      }
      return SubMenuOptionsControls.mSingelton;
    }
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsControls.LOC_CTRL_OPTS));
    string str = LanguageManager.Instance.GetString(SubMenuOptionsControls.LOC_XINPUT);
    (this.mMenuItems[2] as MenuTextItem).SetText(str.Replace("#1;", "1"));
    (this.mMenuItems[3] as MenuTextItem).SetText(str.Replace("#1;", "2"));
    (this.mMenuItems[4] as MenuTextItem).SetText(str.Replace("#1;", "3"));
    (this.mMenuItems[5] as MenuTextItem).SetText(str.Replace("#1;", "4"));
  }

  private SubMenuOptionsControls()
  {
    this.mSelectedPosition = 0;
    this.mMenuItems = new List<MenuItem>();
    this.mMenuTitle = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsControls.LOC_CTRL_OPTS));
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    this.AddMenuTextItem(SubMenuOptionsControls.LOC_KEYBOARD_AND_MOUSE);
    this.mMenuItems.Add((MenuItem) new MenuItemSeparator(new Vector2(this.mPosition.X, this.mPosition.Y + ((float) font.LineHeight + 10f) * (float) this.mMenuItems.Count)));
    string str = LanguageManager.Instance.GetString(SubMenuOptionsControls.LOC_XINPUT);
    this.AddMenuTextItem(str.Replace("#1;", "1"));
    this.AddMenuTextItem(str.Replace("#1;", "2"));
    this.AddMenuTextItem(str.Replace("#1;", "3"));
    this.AddMenuTextItem(str.Replace("#1;", "4"));
    this.mMenuItems.Add((MenuItem) new MenuItemSeparator(new Vector2(this.mPosition.X, this.mPosition.Y + ((float) font.LineHeight + 10f) * (float) this.mMenuItems.Count)));
    this.mScrollBar = new MenuScrollBar(new Vector2(), (float) (font.LineHeight * 10), 0);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
    this.UpdateControllers();
  }

  public void AddMenuTextItem(string iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuItems.Count;
    this.mMenuItems.Add((MenuItem) new MenuTextItem(iText, mPosition, font, TextAlign.Center));
  }

  public void UpdateControllers()
  {
    List<DirectInputController> dinputPads = ControlManager.Instance.DInputPads;
    int index1;
    for (index1 = 0; index1 < dinputPads.Count; ++index1)
    {
      if (index1 >= this.mControllers.Count)
      {
        string iTitle = dinputPads[index1].Device.DeviceInformation.InstanceName;
        if (iTitle.Length > 30)
          iTitle = iTitle.Substring(0, 27) + "...";
        this.mMenuItems.Insert(this.mMenuItems.Count - 1, (MenuItem) new MenuTextItem(iTitle, new Vector2(), FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center));
        this.mControllers.Add(dinputPads[index1].Device.DeviceInformation.InstanceGuid);
      }
      else if (this.mControllers[index1] != dinputPads[index1].Device.DeviceInformation.InstanceGuid)
      {
        string iText = dinputPads[index1].Device.DeviceInformation.InstanceName;
        if (iText.Length > 30)
          iText = iText.Substring(0, 27) + "...";
        (this.mMenuItems[index1 + 7] as MenuTextItem).SetText(iText);
        this.mControllers[index1] = dinputPads[index1].Device.DeviceInformation.InstanceGuid;
      }
    }
    while (index1 < this.mControllers.Count)
    {
      this.mMenuItems.RemoveAt(this.mMenuItems.Count - 2);
      this.mControllers.RemoveAt(this.mControllers.Count - 1);
    }
    int num = 0;
    for (int index2 = 0; index2 < ControlManager.Instance.XInputPads.Count; ++index2)
    {
      if (ControlManager.Instance.XInputPads[index2].IsConnected)
        ++num;
    }
    this.mScrollBar.SetMaxValue(this.mMenuItems.Count - (4 - num) - 10);
  }

  public override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
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
      if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit))
      {
        if (iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released)
        {
          if (oRightPageHit)
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
              base.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
          }
          else
            base.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
        }
        else
          base.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
      }
    }
    if (iState.LeftButton != ButtonState.Released)
      return;
    this.mScrollBar.Grabbed = false;
  }

  public override void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (!Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit))
      return;
    if (this.mScrollBar.Grabbed)
    {
      this.mScrollBar.ScrollTo(oHitPosition.Y);
    }
    else
    {
      if (!oRightPageHit)
        return;
      bool flag = false;
      for (int index1 = this.mScrollBar.Value; index1 < Math.Min(this.mScrollBar.Value + 10, this.mMenuItems.Count); ++index1)
      {
        MenuItem mMenuItem = this.mMenuItems[index1];
        if (mMenuItem.Enabled && mMenuItem.InsideBounds(ref oHitPosition))
        {
          if (this.mSelectedPosition != index1)
            AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
          this.mKeyboardSelection = false;
          this.mSelectedPosition = index1;
          for (int index2 = 0; index2 < this.mMenuItems.Count; ++index2)
            this.mMenuItems[index2].Selected = index2 == index1;
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      if (this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref oHitPosition))
      {
        if (this.mSelectedPosition != this.mMenuItems.Count - 1)
          AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
        this.mKeyboardSelection = false;
        this.mSelectedPosition = this.mMenuItems.Count - 1;
        for (int index = 0; index < this.mMenuItems.Count; ++index)
          this.mMenuItems[index].Selected = index == this.mMenuItems.Count - 1;
      }
      else
      {
        for (int index = 0; index < this.mMenuItems.Count; ++index)
          this.mMenuItems[index].Selected = false;
        this.mSelectedPosition = -1;
      }
    }
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.mEffect.Color = new Vector4(0.0f, 0.0f, 0.0f, 1f);
    int num1 = 0;
    for (int index = 0; index < ControlManager.Instance.XInputPads.Count; ++index)
    {
      if (ControlManager.Instance.XInputPads[index].IsConnected)
        ++num1;
    }
    while (this.mSelectedPosition < this.mMenuItems.Count - 1 && this.mSelectedPosition - (4 - num1) >= this.mScrollBar.Value + 10)
      ++this.mScrollBar.Value;
    while (this.mSelectedPosition >= 0 && this.mSelectedPosition < this.mScrollBar.Value)
      --this.mScrollBar.Value;
    if (this.mMenuTitle != null)
      this.mMenuTitle.Draw(this.mEffect, this.mPosition.X, 96f);
    this.mScrollBar.Height = 512f;
    this.mScrollBar.Position = new Vector2(860f, 558f);
    if (this.mScrollBar.MaxValue > 0)
      this.mScrollBar.Draw(this.mEffect);
    Vector2 mPosition = this.mPosition;
    float num2 = 48f;
    for (int index = this.mScrollBar.Value; index < Math.Min(this.mScrollBar.Value + 10, this.mMenuItems.Count - 1); ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      if (index == this.mMenuItems.Count - 1)
        mMenuItem.Draw(this.mEffect);
      else if (!(index == 1 & num1 == 0 | index == 6 & ControlManager.Instance.DInputPads.Count == 0))
      {
        if (index >= 2 & index < 6 && !ControlManager.Instance.XInputPads[index - 2].IsConnected)
        {
          mMenuItem.Enabled = false;
        }
        else
        {
          mMenuItem.Enabled = true;
          mMenuItem.Position = mPosition;
          mMenuItem.Draw(this.mEffect);
          mPosition.Y += num2;
        }
      }
    }
    this.mMenuItems[this.mMenuItems.Count - 1].Draw(this.mEffect);
    this.mEffect.GraphicsDevice.Viewport = iLeftSide;
    this.mEffect.Color = new Vector4(2f, 2f, 2f, 1f);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  protected override void ControllerMouseClicked(Controller iSender)
  {
    if (this.mSelectedPosition == this.mMenuItems.Count - 1)
      Tome.Instance.PopMenu();
    else
      base.ControllerMouseClicked(iSender);
  }

  public override void ControllerA(Controller iSender)
  {
    if (this.mSelectedPosition == 1 || this.mSelectedPosition == 6)
      return;
    if (this.mSelectedPosition == 0)
      Tome.Instance.PushMenu((SubMenu) SubMenuOptionsKeyboard.Instance, 1);
    else if (this.mSelectedPosition == this.mMenuItems.Count - 1)
      Tome.Instance.PopMenu();
    else if (this.mSelectedPosition > 1 && this.mSelectedPosition < 6)
    {
      SubMenuOptionsGamepad.Instance.Controller = (Controller) ControlManager.Instance.XInputPads[this.mSelectedPosition - 2];
      Tome.Instance.PushMenu((SubMenu) SubMenuOptionsGamepad.Instance, 1);
    }
    else
    {
      SubMenuOptionsGamepad.Instance.Controller = (Controller) ControlManager.Instance.DInputPads[this.mSelectedPosition - 7];
      Tome.Instance.PushMenu((SubMenu) SubMenuOptionsGamepad.Instance, 1);
    }
  }

  public override void ControllerB(Controller iSender) => base.ControllerB(iSender);

  public override void ControllerUp(Controller iSender) => base.ControllerUp(iSender);

  public override void ControllerDown(Controller iSender) => base.ControllerDown(iSender);

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
  }

  public override void OnEnter()
  {
    this.UpdateControllers();
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
  }

  public override void OnExit() => SaveManager.Instance.SaveSettings();
}
