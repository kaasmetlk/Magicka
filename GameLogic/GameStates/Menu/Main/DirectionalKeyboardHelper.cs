// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.DirectionalKeyboardHelper
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class DirectionalKeyboardHelper
{
  private Stopwatch mWatch;
  internal Action<Controller> UpPressed;
  internal Action<Controller> DownPressed;
  internal Action<Controller> RightPressed;
  internal Action<Controller> LeftPressed;
  private bool mFirstPress;

  internal DirectionalKeyboardHelper() => this.mWatch = new Stopwatch();

  internal void Update(Controller iSender, KeyboardState iOldState, KeyboardState iNewState)
  {
    if (iOldState == iNewState)
    {
      if (this.mWatch.ElapsedMilliseconds <= (this.mFirstPress ? 500L : 100L))
        return;
      this.mWatch.Reset();
      this.mWatch.Start();
      this.mFirstPress = false;
      this.Press(iSender, iNewState);
    }
    else
    {
      if (this.CheckNewPresses(iSender, iOldState, iNewState))
        return;
      this.CheckReleases(iOldState, iNewState);
    }
  }

  private bool CheckNewPresses(
    Controller iSender,
    KeyboardState iOldState,
    KeyboardState iNewState)
  {
    bool flag = iOldState.IsKeyUp(Keys.Down) && iNewState.IsKeyDown(Keys.Down) || iOldState.IsKeyUp(Keys.Up) && iNewState.IsKeyDown(Keys.Up) || iOldState.IsKeyUp(Keys.Right) && iNewState.IsKeyDown(Keys.Right) || iOldState.IsKeyUp(Keys.Left) && iNewState.IsKeyDown(Keys.Left);
    if (flag)
    {
      this.mFirstPress = true;
      this.mWatch.Reset();
      this.mWatch.Start();
    }
    return flag;
  }

  private void CheckReleases(KeyboardState iOldState, KeyboardState iNewState)
  {
    if ((!iOldState.IsKeyDown(Keys.Down) || !iNewState.IsKeyUp(Keys.Down)) && (!iOldState.IsKeyDown(Keys.Up) || !iNewState.IsKeyUp(Keys.Up)) && (!iOldState.IsKeyDown(Keys.Right) || !iNewState.IsKeyUp(Keys.Right)) && (!iOldState.IsKeyDown(Keys.Left) || !iNewState.IsKeyUp(Keys.Left)))
      return;
    this.mWatch.Stop();
    this.mWatch.Reset();
  }

  private void Press(Controller iSender, KeyboardState iState)
  {
    if (iState.IsKeyDown(Keys.Down))
    {
      if (this.DownPressed == null)
        return;
      this.DownPressed(iSender);
    }
    else if (iState.IsKeyDown(Keys.Up))
    {
      if (this.UpPressed == null)
        return;
      this.UpPressed(iSender);
    }
    else if (iState.IsKeyDown(Keys.Right))
    {
      if (this.RightPressed == null)
        return;
      this.RightPressed(iSender);
    }
    else
    {
      if (!iState.IsKeyDown(Keys.Left) || this.LeftPressed == null)
        return;
      this.LeftPressed(iSender);
    }
  }
}
