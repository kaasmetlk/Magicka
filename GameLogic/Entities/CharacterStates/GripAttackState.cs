// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.GripAttackState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Abilities;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class GripAttackState : BaseState
{
  private static GripAttackState sSingelton;
  private static volatile object sSingeltonLock = new object();

  public static GripAttackState Instance
  {
    get
    {
      if (GripAttackState.sSingelton == null)
      {
        lock (GripAttackState.sSingeltonLock)
        {
          if (GripAttackState.sSingelton == null)
            GripAttackState.sSingelton = new GripAttackState();
        }
      }
      return GripAttackState.sSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.SetInvisible(0.0f);
    iOwner.Ethereal(false, 1f, 1f);
    iOwner.GoToAnimation(iOwner.NextGripAttackAnimation, 0.075f);
    iOwner.NextGripAttackAnimation = Animations.None;
    iOwner.CharacterBody.AllowRotate = false;
    iOwner.CharacterBody.AllowMove = false;
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    AnimationController animationController = iOwner.AnimationController;
    if (!animationController.CrossFadeEnabled && animationController.HasFinished && iOwner.GripAttacking && iOwner.NextGripAttackAnimation != Animations.None)
    {
      if (iOwner.CurrentAnimation == Animations.attack_recoil)
      {
        iOwner.NextGripAttackAnimation = Animations.None;
      }
      else
      {
        iOwner.GoToAnimation(iOwner.NextGripAttackAnimation, 0.2f);
        iOwner.NextGripAttackAnimation = Animations.None;
      }
    }
    else
    {
      if (iOwner.ShouldReleaseGrip)
      {
        iOwner.GripAttacking = false;
        return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 || iOwner.IsEntangled ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
      }
      if (animationController.HasFinished && iOwner is NonPlayerCharacter nonPlayerCharacter)
        nonPlayerCharacter.AI.BusyAbility = (Ability) null;
    }
    return (BaseState) null;
  }

  public override void OnExit(Character iOwner)
  {
    if (iOwner is NonPlayerCharacter nonPlayerCharacter)
      nonPlayerCharacter.AI.BusyAbility = (Ability) null;
    iOwner.GripAttacking = false;
    iOwner.CharacterBody.AllowMove = true;
    iOwner.CharacterBody.AllowRotate = true;
  }
}
