// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.Options.SubMenuOptionsGamepad
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

internal class SubMenuOptionsGamepad : SubMenu
{
  private const int VISIBLE_ITEMS = 12;
  private static readonly int LOC_GAMEPAD = "#menu_opt_alt_08".GetHashCodeCustom();
  private static readonly int LOC_RECONFIG = "#menu_reconfig".GetHashCodeCustom();
  private static readonly int LOC_DEFAULTS = "#menu_restore_d".GetHashCodeCustom();
  private static readonly int LOC_XINPUT = "#menu_xinput_gamep".GetHashCodeCustom();
  private static readonly int LOC_CHANGEBINDING = "#opt_changebinding".GetHashCodeCustom();
  private static SubMenuOptionsGamepad sSingelton;
  private static volatile object sSingeltonLock = new object();
  private List<MenuTextItem> mMenuOptions;
  private Controller mController;
  private ControllerFunction mFunction;
  private MenuScrollBar mScrollBar;
  private MenuImageTextItem mBackItem;
  private SubMenuOptionsGamepad.Selection mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
  private BitmapFont mFont;

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.Controller = this.mController;
    this.mBackItem.LanguageChanged();
  }

  public static SubMenuOptionsGamepad Instance
  {
    get
    {
      if (SubMenuOptionsGamepad.sSingelton == null)
      {
        lock (SubMenuOptionsGamepad.sSingeltonLock)
        {
          if (SubMenuOptionsGamepad.sSingelton == null)
            SubMenuOptionsGamepad.sSingelton = new SubMenuOptionsGamepad();
        }
      }
      return SubMenuOptionsGamepad.sSingelton;
    }
  }

  private SubMenuOptionsGamepad()
  {
    this.mMenuTitle = new Text(128 /*0x80*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_GAMEPAD));
    this.mMenuItems = new List<MenuItem>();
    this.mMenuOptions = new List<MenuTextItem>();
    this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    this.mMenuItems.Add((MenuItem) new MenuTextItem(LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_DEFAULTS), new Vector2(this.mPosition.X, 832f), this.mFont, TextAlign.Center));
    for (int index = 0; index < 24; ++index)
    {
      this.AddMenuTextItem(("#ctrl_" + ((ControllerFunction) index).ToString()).ToLowerInvariant().GetHashCodeCustom());
      this.AddMenuOptions("-");
    }
    this.mBackItem = new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE);
    this.mScrollBar = new MenuScrollBar(new Vector2(928f, 512f), (float) (this.mFont.LineHeight * 13), 12);
  }

  public void AddMenuTextItem(string iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.X -= 30f;
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuItems.Count;
    this.mMenuItems.Add((MenuItem) new MenuTextItem(iText, mPosition, font, TextAlign.Right));
  }

  public override MenuTextItem AddMenuTextItem(int iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.X -= 30f;
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuItems.Count;
    MenuTextItem menuTextItem = new MenuTextItem(iText, mPosition, font, TextAlign.Right);
    this.mMenuItems.Add((MenuItem) menuTextItem);
    return menuTextItem;
  }

  public void AddMenuOptions(string iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    mPosition.X += 30f;
    mPosition.Y += ((float) font.LineHeight + 10f) * (float) this.mMenuItems.Count;
    this.mMenuOptions.Add(new MenuTextItem(iText, mPosition, font, TextAlign.Left, false));
  }

  public Controller Controller
  {
    get => this.mController;
    set
    {
      this.mController = value;
      DirectInputController mController1 = this.mController as DirectInputController;
      XInputController mController2 = this.mController as XInputController;
      if (mController1 != null)
      {
        (this.mMenuItems[0] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_RECONFIG));
        this.mMenuTitle.SetText(mController1.Device.DeviceInformation.InstanceName);
        for (int index = 0; index < 24; ++index)
        {
          DirectInputController.Binding binding = mController1.Bindings[index];
          if (binding.Type == DirectInputController.Binding.BindingType.Button)
            this.mMenuOptions[index].SetText("B" + (object) (binding.BindingIndex + 1));
          else if (binding.Type == DirectInputController.Binding.BindingType.POV)
            this.mMenuOptions[index].SetText(((ControllerDirection) (1 << binding.BindingIndex)).ToString());
          else if (binding.Type == DirectInputController.Binding.BindingType.PositiveAxis)
          {
            string str = ((char) (88 + binding.BindingIndex % 3)).ToString();
            if (binding.BindingIndex >= 3)
              str = 'R'.ToString() + str;
            string iText = '+'.ToString() + str;
            this.mMenuOptions[index].SetText(iText);
          }
          else if (binding.Type == DirectInputController.Binding.BindingType.NegativeAxis)
          {
            string str = ((char) (88 + binding.BindingIndex % 3)).ToString();
            if (binding.BindingIndex >= 3)
              str = 'R'.ToString() + str;
            string iText = '-'.ToString() + str;
            this.mMenuOptions[index].SetText(iText);
          }
          else
            this.mMenuOptions[index].SetText("-");
        }
      }
      else
      {
        if (mController2 == null)
          return;
        (this.mMenuItems[0] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_DEFAULTS));
        this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenuOptionsGamepad.LOC_XINPUT).Replace("#1;", ((int) (mController2.PlayerIndex + 1)).ToString()));
        for (int index = 0; index < 24; ++index)
        {
          XInputController.Binding binding = mController2.Bindings[index];
          if (binding.Type == XInputController.Binding.BindingType.Button)
            this.mMenuOptions[index].SetText(XInputController.GetButtonName((Buttons) binding.BindingIndex));
          else if (binding.Type == XInputController.Binding.BindingType.Trigger)
          {
            if (binding.BindingIndex == 0)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.LeftTrigger));
            else if (binding.BindingIndex == 1)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.RightTrigger));
            else
              this.mMenuOptions[index].SetText("-");
          }
          else if (binding.Type == XInputController.Binding.BindingType.PositiveStick)
          {
            if (binding.BindingIndex == 0)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.LeftThumbstickRight));
            else if (binding.BindingIndex == 1)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.LeftThumbstickUp));
            else if (binding.BindingIndex == 2)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.RightThumbstickRight));
            else if (binding.BindingIndex == 3)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.RightThumbstickUp));
            else
              this.mMenuOptions[index].SetText("-");
          }
          else if (binding.Type == XInputController.Binding.BindingType.NegativeStick)
          {
            if (binding.BindingIndex == 0)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.LeftThumbstickLeft));
            else if (binding.BindingIndex == 1)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.LeftThumbstickDown));
            else if (binding.BindingIndex == 2)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.RightThumbstickLeft));
            else if (binding.BindingIndex == 3)
              this.mMenuOptions[index].SetText(XInputController.GetButtonName(Buttons.RightThumbstickDown));
            else
              this.mMenuOptions[index].SetText("-");
          }
          else
            this.mMenuOptions[index].SetText("-");
        }
      }
    }
  }

  public override void ControllerUp(Controller iSender)
  {
    if (this.mCurrentSelection == SubMenuOptionsGamepad.Selection.Back)
    {
      this.mSelectedPosition = 0;
      this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
      this.mBackItem.Selected = false;
      this.mMenuItems[0].Selected = true;
    }
    else if (this.mSelectedPosition == 1)
    {
      this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Back;
      this.mBackItem.Selected = true;
      this.mMenuItems[1].Selected = false;
      this.mMenuOptions[0].Selected = false;
    }
    else
    {
      this.mBackItem.Selected = false;
      --this.mSelectedPosition;
      if (this.mSelectedPosition < 0)
        this.mSelectedPosition = this.mMenuItems.Count - 1;
      for (int index = 0; index < this.mMenuItems.Count; ++index)
      {
        this.mMenuItems[index].Selected = index == this.mSelectedPosition;
        if (index > 0)
          this.mMenuOptions[index - 1].Selected = index == this.mSelectedPosition;
      }
    }
  }

  public override void ControllerDown(Controller iSender)
  {
    if (this.mCurrentSelection == SubMenuOptionsGamepad.Selection.Back)
    {
      this.mSelectedPosition = 1;
      this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
      this.mBackItem.Selected = false;
      this.mMenuItems[this.mSelectedPosition].Selected = true;
      this.mMenuOptions[this.mSelectedPosition - 1].Selected = true;
    }
    else if (this.mSelectedPosition == 0)
    {
      this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Back;
      this.mBackItem.Selected = true;
      this.mMenuItems[0].Selected = false;
    }
    else
    {
      this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
      this.mBackItem.Selected = false;
      ++this.mSelectedPosition;
      if (this.mSelectedPosition >= this.mMenuItems.Count)
        this.mSelectedPosition = 0;
      for (int index = 0; index < this.mMenuItems.Count; ++index)
      {
        this.mMenuItems[index].Selected = index == this.mSelectedPosition;
        if (index > 0)
          this.mMenuOptions[index - 1].Selected = index == this.mSelectedPosition;
      }
    }
  }

  public override void ControllerA(Controller iSender)
  {
    if (this.mCurrentSelection == SubMenuOptionsGamepad.Selection.Back)
      Tome.Instance.PopMenu();
    else if (this.mSelectedPosition == 0)
    {
      if (this.mController is XInputController)
      {
        (this.mController as XInputController).LoadDefaults();
        this.Controller = this.mController;
      }
      else
      {
        if (!(this.mController is DirectInputController))
          return;
        GamePadConfigMessageBox.Instance.GamePad = this.mController as DirectInputController;
        GamePadConfigMessageBox.Instance.Show();
      }
    }
    else
    {
      if (this.mSelectedPosition <= 0)
        return;
      this.mMenuOptions[this.mSelectedPosition - 1].SetText("???");
      this.mFunction = (ControllerFunction) (this.mSelectedPosition - 1);
      if (this.mController is DirectInputController)
      {
        (this.mController as DirectInputController).OnChange += new Action<DirectInputController.Binding>(this.OnBindingChanged);
      }
      else
      {
        if (!(this.mController is XInputController))
          return;
        (this.mController as XInputController).OnChange += new Action<XInputController.Binding>(this.OnBindingChanged);
      }
    }
  }

  public override void ControllerB(Controller iSender)
  {
    if (this.mFunction >= ControllerFunction.Move_Right)
      this.mFunction = ~ControllerFunction.Move_Right;
    else
      Tome.Instance.PopMenu();
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
      if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) && iState.LeftButton == ButtonState.Pressed && iOldState.LeftButton == ButtonState.Released && oRightPageHit)
      {
        if (this.mScrollBar.InsideBounds(ref oHitPosition))
        {
          if (this.mScrollBar.InsideDragBounds(oHitPosition))
          {
            this.mScrollBar.Grabbed = true;
            return;
          }
          if (this.mScrollBar.InsideUpBounds(oHitPosition))
          {
            --this.mScrollBar.Value;
            return;
          }
          if (this.mScrollBar.InsideDownBounds(oHitPosition))
          {
            ++this.mScrollBar.Value;
            return;
          }
          this.mScrollBar.ScrollTo(oHitPosition.Y);
          return;
        }
        if (this.mBackItem.Enabled && this.mBackItem.InsideBounds(ref oHitPosition))
        {
          Tome.Instance.PopMenu();
          return;
        }
        if (this.mFunction < ControllerFunction.Move_Right)
        {
          bool flag = false;
          MenuItem mMenuItem1 = this.mMenuItems[0];
          if (mMenuItem1.Enabled & mMenuItem1.InsideBounds(ref oHitPosition))
          {
            if (this.mSelectedPosition != 0)
              AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
            this.mKeyboardSelection = false;
            this.mSelectedPosition = 0;
            mMenuItem1.Selected = true;
            for (int index = 1; index < this.mMenuItems.Count; ++index)
            {
              this.mMenuItems[index].Selected = false;
              this.mMenuOptions[index - 1].Selected = false;
            }
            flag = true;
            if (this.mController is XInputController)
            {
              (this.mController as XInputController).LoadDefaults();
              this.Controller = this.mController;
            }
            else if (this.mController is DirectInputController)
            {
              GamePadConfigMessageBox.Instance.GamePad = this.mController as DirectInputController;
              GamePadConfigMessageBox.Instance.Show();
            }
          }
          if (!flag)
          {
            for (int index1 = this.mScrollBar.Value + 1; index1 < this.mScrollBar.Value + 12 + 11; ++index1)
            {
              MenuItem mMenuItem2 = this.mMenuItems[index1];
              MenuItem mMenuOption = (MenuItem) this.mMenuOptions[index1 - 1];
              if (mMenuItem2.Enabled && (mMenuItem2.InsideBounds(ref oHitPosition) || mMenuOption.InsideBounds(ref oHitPosition)))
              {
                if (this.mSelectedPosition != index1)
                  AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
                this.mKeyboardSelection = false;
                this.mSelectedPosition = index1;
                for (int index2 = 0; index2 < this.mMenuItems.Count; ++index2)
                {
                  this.mMenuItems[index2].Selected = index2 == index1;
                  if (index2 > 0)
                    this.mMenuOptions[index2 - 1].Selected = index2 == index1;
                }
                flag = true;
                if (index1 > 0)
                {
                  (mMenuOption as MenuTextItem).SetText("???");
                  this.mFunction = (ControllerFunction) (index1 - 1);
                  if (this.mController is DirectInputController)
                  {
                    (this.mController as DirectInputController).OnChange += new Action<DirectInputController.Binding>(this.OnBindingChanged);
                    break;
                  }
                  if (this.mController is XInputController)
                  {
                    (this.mController as XInputController).OnChange += new Action<XInputController.Binding>(this.OnBindingChanged);
                    break;
                  }
                  break;
                }
                break;
              }
            }
          }
          if (!flag)
          {
            for (int index = 0; index < this.mMenuItems.Count; ++index)
              this.mMenuItems[index].Selected = false;
          }
        }
      }
    }
    if (iState.LeftButton != ButtonState.Released)
      return;
    this.mScrollBar.Grabbed = false;
  }

  private void OnBindingChanged(DirectInputController.Binding iBinding)
  {
    if (this.mFunction < ControllerFunction.Move_Right)
      return;
    (this.mController as DirectInputController).Bindings[(int) this.mFunction] = iBinding;
    this.mFunction = ~ControllerFunction.Move_Right;
    this.Controller = this.mController;
  }

  private void OnBindingChanged(XInputController.Binding iBinding)
  {
    if (this.mFunction < ControllerFunction.Move_Right)
      return;
    (this.mController as XInputController).Bindings[(int) this.mFunction] = iBinding;
    this.mFunction = ~ControllerFunction.Move_Right;
    this.Controller = this.mController;
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
    this.mBackItem.Selected = false;
    if (this.mScrollBar.InsideBounds(ref oHitPosition))
    {
      if (!this.mScrollBar.Grabbed)
        return;
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
    else if (this.mBackItem.Enabled && this.mBackItem.InsideBounds(ref oHitPosition))
    {
      this.mBackItem.Selected = true;
      this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Back;
    }
    else
    {
      bool flag = false;
      MenuItem mMenuItem1 = this.mMenuItems[0];
      if (mMenuItem1.Enabled & mMenuItem1.InsideBounds(ref oHitPosition))
      {
        if (this.mSelectedPosition != 0)
          AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
        this.mKeyboardSelection = false;
        this.mSelectedPosition = 0;
        mMenuItem1.Selected = true;
        for (int index = 1; index < this.mMenuItems.Count; ++index)
        {
          this.mMenuItems[index].Selected = false;
          this.mMenuOptions[index - 1].Selected = false;
        }
        flag = true;
        this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
      }
      if (!flag)
      {
        for (int index1 = this.mScrollBar.Value + 1; index1 < this.mScrollBar.Value + 12 + 1; ++index1)
        {
          MenuItem mMenuItem2 = this.mMenuItems[index1];
          MenuItem mMenuOption = (MenuItem) this.mMenuOptions[index1 - 1];
          if (mMenuItem2.Enabled && (mMenuItem2.InsideBounds(ref oHitPosition) || mMenuOption.InsideBounds(ref oHitPosition)))
          {
            if (this.mSelectedPosition != index1)
              AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
            this.mKeyboardSelection = false;
            this.mSelectedPosition = index1;
            for (int index2 = 0; index2 < this.mMenuItems.Count; ++index2)
            {
              this.mMenuItems[index2].Selected = index2 == index1;
              if (index2 > 0)
                this.mMenuOptions[index2 - 1].Selected = index2 == index1;
            }
            flag = true;
            this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
            break;
          }
        }
      }
      if (flag)
        return;
      this.mSelectedPosition = -1;
      for (int index = 0; index < this.mMenuItems.Count; ++index)
      {
        this.mMenuItems[index].Selected = false;
        if (index > 0)
          this.mMenuOptions[index - 1].Selected = false;
      }
    }
  }

  protected override void ControllerMouseClicked(Controller iSender)
  {
    base.ControllerMouseClicked(iSender);
  }

  public override void OnEnter()
  {
    base.OnEnter();
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenuOptionsGamepad.LOC_CHANGEBINDING);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    this.mFunction = ~ControllerFunction.Move_Right;
    this.mCurrentSelection = SubMenuOptionsGamepad.Selection.Bindings;
    this.mSelectedPosition = 0;
    this.mBackItem.Selected = false;
  }

  public override void OnExit()
  {
    base.OnExit();
    SaveManager.Instance.SaveSettings();
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (!this.mKeyboardSelection || this.mCurrentSelection != SubMenuOptionsGamepad.Selection.Bindings)
      return;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].Selected = index == this.mSelectedPosition;
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.mEffect.VertexColorEnabled = false;
    this.mEffect.Color = new Vector4(0.0f, 0.0f, 0.0f, 0.8f);
    this.mMenuTitle.Draw(this.mEffect, this.mPosition.X, 96f);
    this.mScrollBar.Draw(this.mEffect);
    this.mBackItem.Draw(this.mEffect);
    if (this.mSelectedPosition > 0)
    {
      while (this.mSelectedPosition <= this.mScrollBar.Value)
        --this.mScrollBar.Value;
      while (this.mSelectedPosition > this.mScrollBar.Value + 12)
        ++this.mScrollBar.Value;
    }
    float lineHeight = (float) this.mFont.LineHeight;
    float num = this.mPosition.Y - lineHeight * 0.5f;
    for (int index = this.mScrollBar.Value + 1; index < this.mScrollBar.Value + 12 + 1; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      if (this.mFunction >= ControllerFunction.Move_Right)
        mMenuItem.Selected = (ControllerFunction) index == this.mFunction + 1;
      Vector2 position1 = mMenuItem.Position with
      {
        Y = num
      };
      mMenuItem.Position = position1;
      mMenuItem.Draw(this.mEffect);
      MenuItem mMenuOption = (MenuItem) this.mMenuOptions[index - 1];
      if (this.mFunction >= ControllerFunction.Move_Right)
        mMenuOption.Selected = (ControllerFunction) index == this.mFunction + 1;
      Vector2 position2 = mMenuOption.Position with
      {
        Y = num
      };
      mMenuOption.Position = position2;
      mMenuOption.Draw(this.mEffect);
      num += lineHeight;
    }
    MenuItem mMenuItem1 = this.mMenuItems[0];
    Vector2 position = mMenuItem1.Position with
    {
      Y = num + 20f
    };
    mMenuItem1.Position = position;
    mMenuItem1.Draw(this.mEffect);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  private enum Selection
  {
    Back,
    Bindings,
  }
}
