// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.ControlManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.DirectX.DirectInput;
using Microsoft.Xna.Framework;
using PolygonHead;
using System.Collections.Generic;
using XInput;

#nullable disable
namespace Magicka.GameLogic.Controls;

internal class ControlManager
{
  private static ControlManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private List<object> mLimitInput = new List<object>();
  private List<XInputController> mXInputPads = new List<XInputController>();
  private List<DirectInputController> mDInputPads = new List<DirectInputController>();
  private float mLimitInputCooldown;
  private InputMessageFilter mInputMessageFilter;
  private bool[] mPlayerInputLocked = new bool[4];
  private KeyboardMouseController mMenuController;

  public static ControlManager Instance
  {
    get
    {
      if (ControlManager.mSingelton == null)
      {
        lock (ControlManager.mSingeltonLock)
        {
          if (ControlManager.mSingelton == null)
            ControlManager.mSingelton = new ControlManager();
        }
      }
      return ControlManager.mSingelton;
    }
  }

  public KeyboardMouseController MenuController => this.mMenuController;

  public List<DirectInputController> DInputPads => this.mDInputPads;

  public List<XInputController> XInputPads => this.mXInputPads;

  private ControlManager()
  {
    this.mInputMessageFilter = new InputMessageFilter();
    this.mInputMessageFilter.TranslateMessage = true;
    this.mMenuController = new KeyboardMouseController(this.mInputMessageFilter);
    this.mXInputPads.Add(new XInputController(PlayerIndex.One));
    this.mXInputPads.Add(new XInputController(PlayerIndex.Two));
    this.mXInputPads.Add(new XInputController(PlayerIndex.Three));
    this.mXInputPads.Add(new XInputController(PlayerIndex.Four));
  }

  public unsafe void FindNewGamePads()
  {
    foreach (DeviceInstance device in Manager.GetDevices(DeviceType.Gamepad, EnumDevicesFlags.AttachedOnly))
    {
      if (!XInputHelper.IsXInputDevice((GUID*) &device.ProductGuid))
      {
        bool flag = false;
        for (int index = 0; index < this.mDInputPads.Count; ++index)
        {
          if (this.mDInputPads[index].Device.DeviceInformation.InstanceGuid == device.InstanceGuid)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          this.mDInputPads.Add(new DirectInputController(device.InstanceGuid));
      }
    }
    foreach (DeviceInstance device in Manager.GetDevices(DeviceType.Joystick, EnumDevicesFlags.AttachedOnly))
    {
      if (!XInputHelper.IsXInputDevice((GUID*) &device.ProductGuid))
      {
        bool flag = false;
        for (int index = 0; index < this.mDInputPads.Count; ++index)
        {
          if (this.mDInputPads[index].Device.DeviceInformation.InstanceGuid == device.InstanceGuid)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          this.mDInputPads.Add(new DirectInputController(device.InstanceGuid));
      }
    }
  }

  public void ClearControllers()
  {
    for (int index = 0; index < this.mXInputPads.Count; ++index)
      this.mXInputPads[index].Clear();
    for (int index = 0; index < this.mDInputPads.Count; ++index)
      this.mDInputPads[index].Clear();
    this.mMenuController.Clear();
    this.mLimitInput.Clear();
  }

  public void HandleInput(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mLimitInputCooldown -= iDeltaTime;
    for (int index = 0; index < this.mXInputPads.Count; ++index)
      this.mXInputPads[index].Update(iDataChannel, iDeltaTime);
    for (int index = 0; index < this.mDInputPads.Count; ++index)
      this.mDInputPads[index].Update(iDataChannel, iDeltaTime);
    this.mMenuController.Update(iDataChannel, iDeltaTime);
  }

  public void LimitInput(object iLocker)
  {
    if (!this.mLimitInput.Contains(iLocker))
      this.mLimitInput.Add(iLocker);
    this.mLimitInputCooldown = 0.1f;
  }

  public void UnlimitInput(object iLocker)
  {
    if (!this.mLimitInput.Remove(iLocker))
      return;
    this.mLimitInputCooldown = 0.1f;
  }

  public bool IsInputLimited
  {
    get => this.mLimitInput.Count > 0 || (double) this.mLimitInputCooldown > 0.0;
  }

  public void UnlimitInput()
  {
    this.mLimitInput.Clear();
    this.mLimitInputCooldown = 0.1f;
  }

  public void LockPlayerInput(int iPlayerIndex) => this.mPlayerInputLocked[iPlayerIndex] = true;

  public bool IsPlayerInputLocked(int iPlayerIndex) => this.mPlayerInputLocked[iPlayerIndex];

  public void UnlockPlayerInput(int iPlayerIndex) => this.mPlayerInputLocked[iPlayerIndex] = false;

  public void LockPlayerInput(Controller iSender)
  {
    this.mPlayerInputLocked[iSender.Player.ID] = true;
  }

  public bool IsPlayerInputLocked(Controller iSender) => this.mPlayerInputLocked[iSender.Player.ID];

  public void UnlockPlayerInput(Controller iSender)
  {
    this.mPlayerInputLocked[iSender.Player.ID] = false;
  }

  public int GamePadCount
  {
    get
    {
      int gamePadCount = 0;
      for (int index = 0; index < this.mDInputPads.Count; ++index)
      {
        if (this.mDInputPads[index].IsConnected && this.mDInputPads[index].Configured)
          ++gamePadCount;
      }
      for (int index = 0; index < this.mXInputPads.Count; ++index)
      {
        if (this.mXInputPads[index].IsConnected)
          ++gamePadCount;
      }
      return gamePadCount;
    }
  }

  public void UnlockPlayerInput()
  {
    for (int index = 0; index < 4; ++index)
      this.mPlayerInputLocked[index] = false;
  }
}
