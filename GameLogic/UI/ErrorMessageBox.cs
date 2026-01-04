// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.ErrorMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class ErrorMessageBox : MessageBox
{
  private static ErrorMessageBox sSingelton;
  private static volatile object sSingeltonLock = new object();
  private float mTimer;
  private MenuTextItem mOkbutton;

  public static ErrorMessageBox Instance
  {
    get
    {
      if (ErrorMessageBox.sSingelton == null)
      {
        lock (ErrorMessageBox.sSingeltonLock)
        {
          if (ErrorMessageBox.sSingelton == null)
            ErrorMessageBox.sSingelton = new ErrorMessageBox();
        }
      }
      return ErrorMessageBox.sSingelton;
    }
  }

  private ErrorMessageBox()
    : base(SubMenu.LOC_ANY)
  {
    this.mMessage.DefaultColor = MenuItem.COLOR;
    this.mOkbutton = new MenuTextItem(SubMenu.LOC_OK, new Vector2(this.mCenter.X, this.mCenter.Y + 64f), this.mMessage.Font, TextAlign.Center);
  }

  public override void Show() => throw new NotSupportedException();

  public void Show(string iMessage)
  {
    this.mAlpha = 0.0f;
    this.mDead = false;
    DialogManager.Instance.AddMessageBox((MessageBox) this);
    this.mMessage.SetText(iMessage);
    this.mTimer = 5f;
  }

  public override void Draw(float iDeltaTime)
  {
    base.Draw(iDeltaTime);
    if ((double) this.mTimer <= 0.0)
      this.mDead = true;
    this.mTimer -= iDeltaTime;
    MessageBox.sGUIBasicEffect.Color = MenuItem.COLOR;
    this.mMessage.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X, this.mCenter.Y);
    this.mOkbutton.Draw(MessageBox.sGUIBasicEffect);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }

  public override int ZIndex => 2002;

  public override void OnTextInput(char iChar)
  {
  }

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    this.mOkbutton.Selected = this.mOkbutton.InsideBounds(iNewState);
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (iNewState.LeftButton != ButtonState.Released || !this.mOkbutton.InsideBounds(iNewState))
      return;
    this.OnSelect((Controller) null);
  }

  public override void OnSelect(Controller iSender) => this.mTimer = 0.0f;
}
