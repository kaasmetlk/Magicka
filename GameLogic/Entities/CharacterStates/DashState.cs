// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.DashState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class DashState : BaseState
{
  private static DashState sSingelton;
  private static volatile object sSingeltonLock = new object();

  public static DashState Instance
  {
    get
    {
      if (DashState.sSingelton == null)
      {
        lock (DashState.sSingeltonLock)
        {
          if (DashState.sSingelton == null)
            DashState.sSingelton = new DashState();
        }
      }
      return DashState.sSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.GoToAnimation(iOwner.NextDashAnimation, 0.2f);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.IsGripping)
      return (BaseState) GrippingState.Instance;
    AnimationController animationController = iOwner.AnimationController;
    if (animationController.CrossFadeEnabled || !animationController.HasFinished)
      return (BaseState) null;
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }

  public override void OnExit(Character iOwner) => iOwner.Dashing = false;
}
