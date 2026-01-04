// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.ManualConnectMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using SteamWrapper;
using System;
using System.Net;
using System.Text;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class ManualConnectMessageBox : MessageBox
{
  private static ManualConnectMessageBox sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static readonly int LOC_IP = "#network_03".GetHashCodeCustom();
  private static readonly int LOC_PASSWORD = "#settings_p01_password".GetHashCodeCustom();
  private bool mEditAddress;
  private bool mEditPassword;
  private ManualConnectMessageBox.Items mSelectedPosition;
  private MenuTextItem mCancelButton;
  private MenuTextItem mOkButton;
  private PolygonHead.Text mAddressText;
  private StringBuilder mAddress;
  private Rectangle mAddressRect;
  private PolygonHead.Text mPasswordTitle;
  private PolygonHead.Text mPasswordText;
  private StringBuilder mPassword;
  private Rectangle mPasswordRect;
  private Controller mSender;
  private float mTimer;
  private bool mLine;

  public static ManualConnectMessageBox Instance
  {
    get
    {
      if (ManualConnectMessageBox.sSingelton == null)
      {
        lock (ManualConnectMessageBox.sSingeltonLock)
        {
          if (ManualConnectMessageBox.sSingelton == null)
            ManualConnectMessageBox.sSingelton = new ManualConnectMessageBox();
        }
      }
      return ManualConnectMessageBox.sSingelton;
    }
  }

  public event Action<IPAddress, string> Complete;

  private ManualConnectMessageBox()
    : base(LanguageManager.Instance.GetString(SubMenuOnline.LOC_IP))
  {
    this.mAddress = new StringBuilder(32 /*0x20*/, 32 /*0x20*/);
    this.mPassword = new StringBuilder(32 /*0x20*/, 32 /*0x20*/);
    this.mAddressText = new PolygonHead.Text(32 /*0x20*/, this.mFont, TextAlign.Left, true, false);
    this.mAddressText.DefaultColor = MenuItem.COLOR;
    this.mAddressRect.Width = 320;
    this.mAddressRect.Height = this.mFont.LineHeight * 2;
    this.mPasswordText = new PolygonHead.Text(32 /*0x20*/, this.mFont, TextAlign.Left, true, false);
    this.mPasswordText.DefaultColor = MenuItem.COLOR;
    this.mPasswordRect.Width = 320;
    this.mPasswordRect.Height = this.mFont.LineHeight * 2;
    this.mPasswordTitle = new PolygonHead.Text(32 /*0x20*/, this.mFont, TextAlign.Center, false);
    this.mPasswordTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_SETTINGS_PASSWORD));
    this.mPasswordTitle.DefaultColor = MenuItem.COLOR;
    Vector2 iPosition = new Vector2();
    iPosition.X = this.mCenter.X + 16f;
    iPosition.Y = (float) ((double) this.mCenter.Y + (double) this.mSize.Y * 0.5 - 16.0) - (float) this.mFont.LineHeight;
    this.mCancelButton = new MenuTextItem(Defines.LOC_GEN_CANCEL, iPosition, this.mFont, TextAlign.Left);
    this.mCancelButton.ColorDisabled = MenuItem.COLOR_SELECTED * 0.5f;
    this.mCancelButton.Color = MenuItem.COLOR;
    this.mCancelButton.ColorSelected = Vector4.One;
    iPosition.X = this.mCenter.X - 16f;
    this.mOkButton = new MenuTextItem(Defines.LOC_GEN_OK, iPosition, this.mFont, TextAlign.Right);
    this.mOkButton.ColorDisabled = MenuItem.COLOR_SELECTED * 0.5f;
    this.mOkButton.Color = MenuItem.COLOR;
    this.mOkButton.ColorSelected = Vector4.One;
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mCancelButton.LanguageChanged();
    this.mOkButton.LanguageChanged();
    this.mMessage.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_IP));
    this.mPasswordTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_SETTINGS_PASSWORD));
  }

  public override void Show()
  {
    this.mSelectedPosition = ManualConnectMessageBox.Items.IP;
    this.mEditPassword = false;
    base.Show();
  }

  private void DaisyWheelIPInput(string text)
  {
    DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
    this.mAddress = new StringBuilder(text);
    this.mAddressText.SetText(text);
  }

  private void DaisyWheelPasswordInput(string text)
  {
    DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
    this.mPassword = new StringBuilder(text);
    this.mPasswordText.SetText(text);
  }

  public override void OnTextInput(char iChar)
  {
    if (this.mEditPassword)
    {
      if (iChar == '\b')
      {
        if (this.mPassword.Length > 0)
          --this.mPassword.Length;
      }
      else if (this.mPassword.Length < 10)
        this.mPassword.Append(iChar);
      this.mPasswordText.SetText(this.mPassword.ToString());
    }
    if (!this.mEditAddress)
      return;
    if (iChar == '\b')
    {
      if (this.mAddress.Length > 0)
        --this.mAddress.Length;
    }
    else if (this.mAddress.Length < 25 & (char.IsDigit(iChar) | iChar == '.'))
      this.mAddress.Append(iChar);
    this.mAddressText.SetText(this.mAddress.ToString());
  }

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
    switch (iDirection)
    {
      case ControllerDirection.Right:
        ++this.mSelectedPosition;
        if (this.mSelectedPosition >= ManualConnectMessageBox.Items.NrOfItems)
        {
          this.mSelectedPosition = ManualConnectMessageBox.Items.IP;
          break;
        }
        break;
      case ControllerDirection.Up:
        --this.mSelectedPosition;
        if (this.mSelectedPosition < ManualConnectMessageBox.Items.IP)
        {
          this.mSelectedPosition = ManualConnectMessageBox.Items.Cancel;
          break;
        }
        break;
      case ControllerDirection.Left:
        --this.mSelectedPosition;
        if (this.mSelectedPosition < ManualConnectMessageBox.Items.IP)
        {
          this.mSelectedPosition = ManualConnectMessageBox.Items.Cancel;
          break;
        }
        break;
      case ControllerDirection.Down:
        ++this.mSelectedPosition;
        if (this.mSelectedPosition >= ManualConnectMessageBox.Items.NrOfItems)
        {
          this.mSelectedPosition = ManualConnectMessageBox.Items.IP;
          break;
        }
        break;
    }
    Console.WriteLine("Position: " + this.mSelectedPosition.ToString());
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    if (this.mAddressRect.Contains(iNewState.X, iNewState.Y))
      this.mSelectedPosition = ManualConnectMessageBox.Items.IP;
    else if (this.mPasswordRect.Contains(iNewState.X, iNewState.Y))
      this.mSelectedPosition = ManualConnectMessageBox.Items.Password;
    else if (this.mOkButton.InsideBounds((float) iNewState.X, (float) iNewState.Y))
      this.mSelectedPosition = ManualConnectMessageBox.Items.Ok;
    else if (this.mCancelButton.InsideBounds((float) iNewState.X, (float) iNewState.Y))
      this.mSelectedPosition = ManualConnectMessageBox.Items.Cancel;
    else
      this.mSelectedPosition = ManualConnectMessageBox.Items.Invalid;
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (iNewState.LeftButton == ButtonState.Pressed)
      return;
    Vector2 iPoint = new Vector2();
    iPoint.X = (float) iNewState.X;
    iPoint.Y = (float) iNewState.Y;
    int lineHeight = this.mFont.LineHeight;
    this.mEditAddress = false;
    this.mEditPassword = false;
    if (this.mPasswordRect.Contains(iNewState.X, iNewState.Y))
      this.mEditPassword = true;
    if (this.mAddressRect.Contains(iNewState.X, iNewState.Y))
      this.mEditAddress = true;
    if (this.mOkButton.InsideBounds(ref iPoint))
    {
      this.mSelectedPosition = ManualConnectMessageBox.Items.Ok;
      this.OnSelect((Controller) ControlManager.Instance.MenuController);
    }
    if (!this.mCancelButton.InsideBounds(ref iPoint))
      return;
    this.mSelectedPosition = ManualConnectMessageBox.Items.Cancel;
    this.OnSelect((Controller) ControlManager.Instance.MenuController);
  }

  public override void OnSelect(Controller iSender)
  {
    if (this.mEditAddress)
      this.mEditAddress = false;
    if (this.mEditPassword)
      this.mEditPassword = false;
    if (!DaisyWheel.IsDisplaying && iSender is XInputController && (this.mSelectedPosition == ManualConnectMessageBox.Items.IP || this.mSelectedPosition == ManualConnectMessageBox.Items.Password))
    {
      this.DaisyWheelInput(iSender);
    }
    else
    {
      if (DaisyWheel.IsDisplaying)
        return;
      switch (this.mSelectedPosition)
      {
        case ManualConnectMessageBox.Items.IP:
          this.mEditAddress = true;
          break;
        case ManualConnectMessageBox.Items.Password:
          this.mEditPassword = true;
          break;
        case ManualConnectMessageBox.Items.Ok:
          IPAddress address;
          if (IPAddress.TryParse(this.mAddress.ToString(), out address))
          {
            if (this.Complete != null)
            {
              if (string.IsNullOrEmpty(this.mPassword.ToString()))
                this.Complete(address, (string) null);
              else
                this.Complete(address, this.mPassword.ToString());
            }
            this.Kill();
            break;
          }
          ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString("#add_menu_err".GetHashCodeCustom()));
          break;
        case ManualConnectMessageBox.Items.Cancel:
          this.Kill();
          break;
        default:
          this.Kill();
          break;
      }
    }
  }

  private void DaisyWheelInput(Controller iSender)
  {
    switch (this.mSelectedPosition)
    {
      case ManualConnectMessageBox.Items.IP:
        this.mEditAddress = true;
        DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelIPInput));
        if (!DaisyWheel.TryShow(iSender, LanguageManager.Instance.GetString(ManualConnectMessageBox.LOC_IP).ToUpper()))
        {
          DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
          break;
        }
        this.mAddress = new StringBuilder("");
        this.mAddressText.SetText("");
        break;
      case ManualConnectMessageBox.Items.Password:
        this.mEditPassword = true;
        DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelPasswordInput));
        if (!DaisyWheel.TryShow(iSender, LanguageManager.Instance.GetString(ManualConnectMessageBox.LOC_PASSWORD).ToUpper(), false, GamepadTextInputLineMode.GamepadTextInputLineModeSingleLine, 11U))
        {
          DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
          break;
        }
        this.mPassword = new StringBuilder("");
        this.mPasswordText.SetText("");
        break;
    }
  }

  public override void Draw(float iDeltaTime)
  {
    base.Draw(iDeltaTime);
    Vector2 mCenter = this.mCenter;
    this.mTimer -= iDeltaTime;
    while ((double) this.mTimer < 0.0)
    {
      this.mTimer += 0.5f;
      this.mLine = !this.mLine;
      if (this.mEditAddress)
      {
        if (this.mLine)
        {
          this.mAddressText.Characters[this.mAddress.Length] = '_';
          this.mAddressText.Characters[this.mAddress.Length + 1] = char.MinValue;
        }
        else
          this.mAddressText.Characters[this.mAddress.Length] = char.MinValue;
      }
      else
        this.mAddressText.Characters[this.mAddress.Length] = char.MinValue;
      if (this.mEditPassword)
      {
        if (this.mLine)
        {
          this.mPasswordText.Characters[this.mPassword.Length] = '_';
          this.mPasswordText.Characters[this.mPassword.Length + 1] = char.MinValue;
        }
        else
          this.mPasswordText.Characters[this.mPassword.Length] = char.MinValue;
      }
      else
        this.mPasswordText.Characters[this.mPassword.Length] = char.MinValue;
      this.mPasswordText.MarkAsDirty();
      this.mAddressText.MarkAsDirty();
    }
    Vector4 color = MenuItem.COLOR;
    Matrix matrix = new Matrix();
    matrix.M11 = matrix.M22 = matrix.M33 = 1f;
    matrix.M44 = 1f;
    matrix.M41 = mCenter.X;
    matrix.M42 = mCenter.Y;
    MessageBox.sGUIBasicEffect.Transform = matrix;
    MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
    MessageBox.sGUIBasicEffect.TextureEnabled = true;
    float lineHeight = (float) this.mFont.LineHeight;
    mCenter.Y -= lineHeight * 3f;
    Vector4 vector4_1 = this.mSelectedPosition == ManualConnectMessageBox.Items.IP ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    vector4_1.W *= this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4_1;
    this.mMessage.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    this.mAddressRect.X = (int) mCenter.X - 160 /*0xA0*/;
    this.mAddressRect.Y = (int) mCenter.Y;
    mCenter.Y += lineHeight;
    this.mAddressText.Draw(MessageBox.sGUIBasicEffect, mCenter.X - 120f, mCenter.Y);
    mCenter.Y += lineHeight;
    Vector4 vector4_2 = this.mSelectedPosition == ManualConnectMessageBox.Items.Password ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    vector4_2.W *= this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4_2;
    this.mPasswordTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    this.mPasswordRect.X = (int) mCenter.X - 160 /*0xA0*/;
    this.mPasswordRect.Y = (int) mCenter.Y;
    mCenter.Y += lineHeight;
    this.mPasswordText.Draw(MessageBox.sGUIBasicEffect, mCenter.X - 120f, mCenter.Y);
    this.mOkButton.Selected = this.mSelectedPosition == ManualConnectMessageBox.Items.Ok;
    this.mOkButton.Alpha = this.mAlpha;
    this.mOkButton.Draw(MessageBox.sGUIBasicEffect);
    this.mCancelButton.Selected = this.mSelectedPosition == ManualConnectMessageBox.Items.Cancel;
    this.mCancelButton.Alpha = this.mAlpha;
    this.mCancelButton.Draw(MessageBox.sGUIBasicEffect);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }

  private enum Items
  {
    Invalid = -1, // 0xFFFFFFFF
    IP = 0,
    Password = 1,
    Ok = 2,
    Cancel = 3,
    NrOfItems = 4,
  }
}
