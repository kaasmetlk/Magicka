// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Popup.MenuBasePopup
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.UI.Popup;

public abstract class MenuBasePopup
{
  protected Vector2 mPosition = Vector2.Zero;
  protected Vector2 mRelativePosition = new Vector2(0.5f, 0.5f);
  protected float mScale = 1f;
  protected Matrix mTransform = Matrix.Identity;
  protected bool mEnabled = true;
  protected bool mCanDismiss = true;
  protected Vector4 mColour = Vector4.One;
  protected Vector2 mSize = Vector2.Zero;
  protected Vector2 mTopLeft = Vector2.Zero;
  protected Vector2 mBottomRight = Vector2.Zero;
  protected Rectangle mHitBox;
  protected PopupAlign mAlignment = PopupAlign.Top | PopupAlign.Left;
  protected int mSelectedItem = -1;
  protected Action mOnPositiveClickDelegate;
  protected Action mOnNegativeClickDelegate;

  public bool CanDismiss
  {
    get => this.mCanDismiss;
    set => this.mCanDismiss = value;
  }

  public Vector2 Size
  {
    get => this.mSize;
    set => this.mSize = value;
  }

  public bool Enabled
  {
    get => this.mEnabled;
    set => this.mEnabled = value;
  }

  public float Alpha
  {
    get => this.mColour.W;
    set => this.mColour.W = value;
  }

  public PopupAlign Alignment
  {
    get => this.mAlignment;
    set => this.SetAlignment(value);
  }

  public Action OnPositiveClick
  {
    get => this.mOnPositiveClickDelegate;
    set => this.mOnPositiveClickDelegate = value;
  }

  public Action OnNegativeClick
  {
    get => this.mOnNegativeClickDelegate;
    set => this.mOnNegativeClickDelegate = value;
  }

  protected void SetAlignment(PopupAlign iAlignment)
  {
    this.mAlignment = iAlignment;
    Vector2 positionFromAlignment = this.GetPositionFromAlignment();
    this.mTransform.M41 = positionFromAlignment.X;
    this.mTransform.M42 = positionFromAlignment.Y;
  }

  public void SetPosition(Vector2 iPosition)
  {
    this.mRelativePosition = iPosition / PopupSystem.REFERENCE_SIZE;
  }

  public void SetRelativePosition(Vector2 iPosition) => this.mRelativePosition = iPosition;

  protected Vector2 GetPositionFromAlignment()
  {
    Vector2 mPosition = this.mPosition;
    Vector2 vector2 = this.mSize * this.mScale;
    if ((this.mAlignment & PopupAlign.Center) != PopupAlign.None)
      mPosition.Y -= vector2.Y * 0.5f;
    else if ((this.mAlignment & PopupAlign.Bottom) != PopupAlign.None)
      mPosition.Y -= vector2.Y;
    if ((this.mAlignment & PopupAlign.Center) != PopupAlign.None)
      mPosition.X -= vector2.X * 0.5f;
    else if ((this.mAlignment & PopupAlign.Right) != PopupAlign.None)
      mPosition.X -= vector2.X;
    return mPosition;
  }

  public bool InsideBounds(float iX, float iY)
  {
    return (double) iX >= (double) this.mTopLeft.X && (double) iY >= (double) this.mTopLeft.Y && (double) iX <= (double) this.mBottomRight.X && (double) iY <= (double) this.mBottomRight.Y;
  }

  public bool InsideBounds(MouseState iState)
  {
    return this.InsideBounds((float) iState.X, (float) iState.Y);
  }

  public bool InsideBounds(ref Vector2 iPoint) => this.InsideBounds(iPoint.X, iPoint.Y);

  public virtual void LanguageChanged()
  {
  }

  public virtual void Dismiss() => Singleton<PopupSystem>.Instance.DismissCurrentPopup();

  public virtual void OnShow()
  {
    this.mPosition = this.mRelativePosition * PopupSystem.Resolution;
    this.mScale = PopupSystem.Resolution.Y / PopupSystem.REFERENCE_SIZE.Y;
    this.UpdateBoundingBox();
    this.mSelectedItem = -1;
  }

  public virtual void OnHide()
  {
  }

  protected virtual void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X + (float) this.mHitBox.X * this.mScale;
    this.mTopLeft.Y = this.mPosition.Y + (float) this.mHitBox.Y * this.mScale;
    this.mBottomRight.X = this.mPosition.X + (float) (this.mHitBox.X + this.mHitBox.Width) * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + (float) (this.mHitBox.Y + this.mHitBox.Height) * this.mScale;
  }

  public virtual void ResetHitArea()
  {
    this.mHitBox.X = 0;
    this.mHitBox.Y = 0;
    this.mHitBox.Width = (int) this.mSize.X;
    this.mHitBox.Height = (int) this.mSize.Y;
    this.UpdateBoundingBox();
  }

  public virtual void SetHitArea(int iX, int iY, int iWidth, int iHeight)
  {
    this.mHitBox.X = iX;
    this.mHitBox.Y = iY;
    this.mHitBox.Width = iWidth;
    this.mHitBox.Height = iHeight;
    this.UpdateBoundingBox();
  }

  public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
  {
  }

  public virtual void Draw(GUIBasicEffect iEffect)
  {
  }

  internal virtual void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
  }

  internal virtual void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
  }

  internal virtual void ControllerMovement(Controller iSender, ControllerDirection iDirection)
  {
  }

  internal virtual void ControllerUp(Controller iSender)
  {
  }

  internal virtual void ControllerDown(Controller iSender)
  {
  }

  internal virtual void ControllerLeft(Controller iSender)
  {
  }

  internal virtual void ControllerRight(Controller iSender)
  {
  }

  internal virtual void ControllerA(Controller iSender)
  {
  }

  internal virtual void ControllerB(Controller iSender)
  {
  }

  internal virtual void ControllerX(Controller iSender)
  {
  }

  internal virtual void ControllerY(Controller iSender)
  {
  }
}
