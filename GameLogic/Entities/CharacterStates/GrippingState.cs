// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.GrippingState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Abilities;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class GrippingState : BaseState
{
  private static GrippingState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static GrippingState Instance
  {
    get
    {
      if (GrippingState.mSingelton == null)
      {
        lock (GrippingState.mSingeltonLock)
        {
          if (GrippingState.mSingelton == null)
            GrippingState.mSingelton = new GrippingState();
        }
      }
      return GrippingState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    if (iOwner.HasAnimation(Animations.idle_grip))
      iOwner.GoToAnimation(Animations.idle_grip, 0.1f);
    else
      iOwner.GoToAnimation(Animations.idle, 0.1f);
    iOwner.CharacterBody.AllowRotate = false;
    if (!(iOwner is NonPlayerCharacter nonPlayerCharacter))
      return;
    nonPlayerCharacter.AI.BusyAbility = (Ability) null;
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.GripAttacking)
      return (BaseState) GripAttackState.Instance;
    if (iOwner.Attacking)
      return (BaseState) AttackState.Instance;
    if (iOwner.IsGripping && (iOwner.ShouldReleaseGrip || (double) iOwner.GripDamageAccumulation > (double) iOwner.HitTolerance))
    {
      if (iOwner.CharacterBody.IsTouchingGround && iOwner.GripperAttached)
        return (BaseState) DropState.Instance;
      Vector3 iImpulse = new Vector3();
      iOwner.CharacterBody.AddImpulseVelocity(ref iImpulse);
      iOwner.KnockDown();
      return (BaseState) KnockDownState.Instance;
    }
    if (iOwner.IsGripping)
      return (BaseState) null;
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }

  public override void OnExit(Character iOwner)
  {
    base.OnExit(iOwner);
    iOwner.CharacterBody.AllowRotate = true;
  }
}
