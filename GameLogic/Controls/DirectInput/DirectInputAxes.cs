// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.DirectInput.DirectInputAxes
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.DirectX.DirectInput;
using System;

#nullable disable
namespace Magicka.GameLogic.Controls.DirectInput;

internal struct DirectInputAxes
{
  public const int NR_OF_AXES = 6;
  private const float CENTER = 32767.5f;
  private const float INVCENTER = 3.05180438E-05f;
  public static float DeadZone = 0.1f;
  private unsafe fixed float mAxes[6];

  public unsafe DirectInputAxes(Device iDevice, JoystickState iState)
  {
    fixed (float* numPtr = this.mAxes)
    {
      numPtr[0] = (float) (((double) iState.X - 32767.5) * 3.0518043786287308E-05);
      if ((double) Math.Abs(numPtr[0]) < (double) DirectInputAxes.DeadZone)
        numPtr[0] = 0.0f;
      numPtr[1] = (float) (((double) iState.Y - 32767.5) * 3.0518043786287308E-05);
      if ((double) Math.Abs(numPtr[1]) < (double) DirectInputAxes.DeadZone)
        numPtr[1] = 0.0f;
      numPtr[2] = (float) (((double) iState.Z - 32767.5) * 3.0518043786287308E-05);
      if ((double) Math.Abs(numPtr[2]) < (double) DirectInputAxes.DeadZone)
        numPtr[2] = 0.0f;
      numPtr[3] = (float) (((double) iState.Rx - 32767.5) * 3.0518043786287308E-05);
      if ((double) Math.Abs(numPtr[3]) < (double) DirectInputAxes.DeadZone)
        numPtr[3] = 0.0f;
      numPtr[4] = (float) (((double) iState.Ry - 32767.5) * 3.0518043786287308E-05);
      if ((double) Math.Abs(numPtr[4]) < (double) DirectInputAxes.DeadZone)
        numPtr[4] = 0.0f;
      numPtr[5] = (float) (((double) iState.Rz - 32767.5) * 3.0518043786287308E-05);
      if ((double) Math.Abs(numPtr[5]) < (double) DirectInputAxes.DeadZone)
        numPtr[5] = 0.0f;
    }
  }

  public unsafe float this[int index]
  {
    get
    {
      if (index < 0 | index >= 6)
        return 0.0f;
      fixed (float* numPtr = this.mAxes)
        return numPtr[index];
    }
  }
}
