// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.DirectInput.DirectInputDPad
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Controls.DirectInput;

internal struct DirectInputDPad
{
  public ControllerDirection Direction;

  public DirectInputDPad(int direction)
  {
    this.Direction = ControllerDirection.Center;
    if (direction == -1)
      return;
    if (0 < direction & direction < 18000)
      this.Direction |= ControllerDirection.Right;
    if (direction > 27000 | direction < 9000)
      this.Direction |= ControllerDirection.Up;
    if (18000 < direction)
      this.Direction |= ControllerDirection.Left;
    if (!(9000 < direction & direction < 27000))
      return;
    this.Direction |= ControllerDirection.Down;
  }

  public bool this[int iIndex]
  {
    get => (this.Direction & (ControllerDirection) (1 << iIndex)) != ControllerDirection.Center;
  }
}
