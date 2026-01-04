// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.DirectInput.DirectInputButtons
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.DirectX.DirectInput;
using System;

#nullable disable
namespace Magicka.GameLogic.Controls.DirectInput;

internal struct DirectInputButtons
{
  private unsafe fixed uint mButtons[8];
  public int NrOfButtons;

  public unsafe DirectInputButtons(Device iDevice, JoystickState iState)
    : this()
  {
    byte[] buttons = iState.GetButtons();
    fixed (uint* numPtr = this.mButtons)
    {
      this.NrOfButtons = iDevice.Caps.NumberButtons;
      int num1 = Math.Max(this.NrOfButtons / 32 /*0x20*/, 1);
      for (int index1 = 0; index1 < num1; ++index1)
      {
        uint num2 = 0;
        int num3 = Math.Min(this.NrOfButtons - 32 /*0x20*/ * index1, 32 /*0x20*/);
        for (int index2 = 0; index2 < num3; ++index2)
          num2 |= (uint) ((buttons[index1 * 32 /*0x20*/ + index2] == (byte) 0 ? 0 : 1) << index2);
        numPtr[index1] = num2;
      }
    }
  }

  public unsafe bool this[int index]
  {
    get
    {
      if (index < 0 | index >= this.NrOfButtons)
        return false;
      fixed (uint* numPtr = this.mButtons)
        return (1 & (int) (numPtr[index / 32 /*0x20*/] >> index % 32 /*0x20*/)) != 0;
    }
  }
}
