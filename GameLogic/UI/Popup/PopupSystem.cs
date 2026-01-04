// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Popup.PopupSystem
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.UI.Popup;

internal sealed class PopupSystem : Singleton<PopupSystem>, IRenderableGUIObject
{
  public static readonly Vector2 REFERENCE_SIZE = new Vector2(1920f, 1080f);
  public static Vector2 Resolution = new Vector2((float) RenderManager.Instance.ScreenSize.X, (float) RenderManager.Instance.ScreenSize.Y);
  private Queue<MenuBasePopup> mPopupQueue;
  private MenuBasePopup mCurrentPopup;
  private volatile object mLock = new object();
  private bool mDismissPopup;
  private GUIBasicEffect mEffect;

  public bool IsDisplaying => this.mCurrentPopup != null;

  public MenuBasePopup CurrentPopup => this.mCurrentPopup;

  public int ZIndex => 300;

  public PopupSystem()
  {
    this.mPopupQueue = new Queue<MenuBasePopup>();
    this.mEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    this.mEffect.TextureEnabled = true;
    this.mEffect.Color = Vector4.One;
    ResolutionMessageBox.Instance.Complete += new Action<ResolutionData>(this.ChangeResolution);
    Magicka.Game.Instance.Form.SizeChanged += (EventHandler) ((iObj, iArgs) =>
    {
      if (this.mCurrentPopup == null)
        return;
      this.mCurrentPopup.OnShow();
    });
  }

  private void ChangeResolution(ResolutionData iData)
  {
    PopupSystem.Resolution = new Vector2((float) iData.Width, (float) iData.Height);
    if (this.mCurrentPopup == null)
      return;
    this.mCurrentPopup.OnShow();
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    lock (this.mLock)
    {
      if (this.mCurrentPopup == null)
      {
        if (this.mPopupQueue.Count <= 0)
          return;
        this.mCurrentPopup = this.mPopupQueue.Dequeue();
        this.mCurrentPopup.OnShow();
      }
      else if (this.mDismissPopup)
      {
        this.mCurrentPopup.OnHide();
        this.mCurrentPopup = (MenuBasePopup) null;
        this.mDismissPopup = false;
      }
      else
        this.mCurrentPopup.Update(iDataChannel, iDeltaTime);
    }
  }

  public void Draw(float iDeltaTime)
  {
    lock (this.mLock)
    {
      if (this.mCurrentPopup == null)
        return;
      this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
      this.mEffect.Color = Vector4.One;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      this.mCurrentPopup.Draw(this.mEffect);
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }
  }

  public void AddPopupToQueue(MenuBasePopup iPopup)
  {
    if (iPopup == null)
      return;
    this.mPopupQueue.Enqueue(iPopup);
  }

  public void ShowPopupImmediately(MenuBasePopup iPopup)
  {
    if (iPopup == null)
      return;
    if (this.mCurrentPopup != null)
      this.mCurrentPopup.OnHide();
    this.mCurrentPopup = iPopup;
    this.mCurrentPopup.OnShow();
  }

  public void DismissCurrentPopup() => this.mDismissPopup = true;

  public void ForceDismissCurrentPopupIfMatches(MenuBasePopup iPopup)
  {
    if (this.mCurrentPopup != iPopup || this.mCurrentPopup.CanDismiss)
      return;
    this.DismissCurrentPopup();
  }

  public void ControllerEvent(Controller iSender, KeyboardState iState, KeyboardState iOldState)
  {
  }

  public void ControllerMovement(Controller iSender, ControllerDirection iDirection)
  {
    if (this.mCurrentPopup == null || !this.mCurrentPopup.Enabled)
      return;
    switch (iDirection)
    {
      case ControllerDirection.Right:
        this.mCurrentPopup.ControllerRight(iSender);
        break;
      case ControllerDirection.Up:
        this.mCurrentPopup.ControllerUp(iSender);
        break;
      case ControllerDirection.Left:
        this.mCurrentPopup.ControllerLeft(iSender);
        break;
      case ControllerDirection.Down:
        this.mCurrentPopup.ControllerDown(iSender);
        break;
    }
  }

  public void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (this.mCurrentPopup == null || !this.mCurrentPopup.Enabled || iState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
      return;
    this.mCurrentPopup.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
  }

  public void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (this.mCurrentPopup == null || !this.mCurrentPopup.Enabled)
      return;
    this.mCurrentPopup.ControllerMouseMove(iSender, iScreenSize, iState, iOldState);
  }

  public void ControllerA(Controller iSender)
  {
    if (this.mCurrentPopup == null || !this.mCurrentPopup.Enabled)
      return;
    this.mCurrentPopup.ControllerA(iSender);
  }

  public void ControllerB(Controller iSender)
  {
    if (this.mCurrentPopup == null || !this.mCurrentPopup.Enabled || !this.mCurrentPopup.CanDismiss)
      return;
    this.DismissCurrentPopup();
  }

  public void ControllerX(Controller iSender)
  {
    if (this.mCurrentPopup == null || !this.mCurrentPopup.Enabled)
      return;
    this.mCurrentPopup.ControllerX(iSender);
  }

  public void ControllerY(Controller iSender)
  {
    if (this.mCurrentPopup == null || !this.mCurrentPopup.Enabled)
      return;
    this.mCurrentPopup.ControllerY(iSender);
  }
}
