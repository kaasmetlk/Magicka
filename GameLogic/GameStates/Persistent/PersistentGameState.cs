// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Persistent.PersistentGameState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI.Popup;
using Magicka.Graphics;
using Magicka.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.GameStates.Persistent;

public class PersistentGameState : GameState
{
  private const float NEAR_PLANE = 1f;
  private const float FAR_PLANE = 500f;
  private PopupSystem mPopupSystem;

  public PersistentGameState()
    : base(new Camera(Vector3.Zero, Vector3.Forward, Vector3.Up, MagickCamera.DEFAULTFOV, MagickCamera.RATIO_16_9, 1f, 500f))
  {
    this.mPopupSystem = Singleton<PopupSystem>.Instance;
    for (DataChannel iDataChannel = DataChannel.A; iDataChannel < DataChannel.Count; ++iDataChannel)
      this.mScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) this.mPopupSystem);
  }

  public override void OnEnter()
  {
  }

  public override void OnExit()
  {
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mPopupSystem.Update(iDataChannel, iDeltaTime);
  }

  internal void ControllerEvent(Controller iSender, KeyboardState iState, KeyboardState iOldState)
  {
    if (!this.mPopupSystem.IsDisplaying)
      return;
    this.mPopupSystem.ControllerEvent(iSender, iState, iOldState);
  }

  internal void ControllerMovement(Controller iSender, ControllerDirection iDirection)
  {
    if (!this.mPopupSystem.IsDisplaying)
      return;
    Magicka.Game.Instance.IsMouseVisible = false;
    this.mPopupSystem.ControllerMovement(iSender, iDirection);
  }

  internal void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (!this.mPopupSystem.IsDisplaying)
      return;
    this.mPopupSystem.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
  }

  internal void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (!this.mPopupSystem.IsDisplaying)
      return;
    this.mPopupSystem.ControllerMouseMove(iSender, iScreenSize, iState, iOldState);
  }

  internal void ControllerA(Controller iSender)
  {
    if (!this.mPopupSystem.IsDisplaying)
      return;
    this.mPopupSystem.ControllerA(iSender);
  }

  internal void ControllerB(Controller iSender)
  {
    if (!this.mPopupSystem.IsDisplaying)
      return;
    this.mPopupSystem.ControllerB(iSender);
  }

  internal void ControllerX(Controller iSender)
  {
    if (!this.mPopupSystem.IsDisplaying)
      return;
    this.mPopupSystem.ControllerX(iSender);
  }

  internal void ControllerY(Controller iSender)
  {
    if (!this.mPopupSystem.IsDisplaying)
      return;
    this.mPopupSystem.ControllerY(iSender);
  }

  public bool IsolateControls()
  {
    bool flag = false;
    if (this.mPopupSystem.IsDisplaying)
      flag = true;
    return flag;
  }
}
