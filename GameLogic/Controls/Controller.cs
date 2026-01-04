// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.Controller
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Controls;

internal abstract class Controller
{
  public const float MOVETIME = 0.2f;
  public const float FADETIME = 0.2f;
  protected Player mPlayer;
  protected Avatar mAvatar;
  protected bool mInverted;

  public Player Player
  {
    get => this.mPlayer;
    set => this.mPlayer = value;
  }

  protected ControllerDirection GetDirection(Vector2 iValue)
  {
    float divider = iValue.Length();
    if ((double) divider < 0.89999997615814209 || (double) divider <= 0.949999988079071)
      return ControllerDirection.Center;
    Vector2.Divide(ref iValue, divider, out iValue);
    float num1 = (float) Math.Acos((double) Vector2.Dot(iValue, Vector2.UnitX));
    if ((double) iValue.Y < 0.0)
      num1 *= -1f;
    float num2 = Math.Abs(num1);
    if ((double) num2 <= 0.62831854820251465)
      return ControllerDirection.Right;
    if ((double) num2 >= 2.5132741928100586)
      return ControllerDirection.Left;
    if ((double) num1 >= 0.942477822303772 && (double) num1 <= 2.1991147994995117)
      return ControllerDirection.Up;
    return (double) num1 <= -0.942477822303772 && (double) num1 >= -2.1991147994995117 ? ControllerDirection.Down : ControllerDirection.Center;
  }

  public abstract void Update(DataChannel iDataChannel, float iDeltaTime);

  public abstract void Rumble(float iLeft, float iRight);

  public abstract float LeftRumble();

  public abstract float RightRumble();

  public abstract void Clear();

  public abstract void Invert(bool iInvert);

  public bool Inverted => this.mInverted;
}
