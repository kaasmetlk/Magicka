// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.BaseState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public abstract class BaseState
{
  public virtual void OnEnter(Character iOwner)
  {
  }

  public virtual BaseState UpdateBloatDeath(Character iOwner, float iDeltaTime)
  {
    if (iOwner.Bloating)
      return (BaseState) BloatState.Instance;
    return (iOwner.Overkilled || (double) iOwner.HitPoints <= 0.0) && !iOwner.CannotDieWithoutExplicitKill ? (BaseState) DeadState.Instance : (BaseState) null;
  }

  public virtual BaseState UpdateHit(Character iOwner, float iDeltaTime)
  {
    if (!iOwner.CharacterBody.IsJumping && !iOwner.IsGripped && iOwner.CharacterBody.IsPushed && !iOwner.CharacterBody.IsTouchingGround)
      return (BaseState) FlyingState.Instance;
    if ((iOwner.IsEntangled || iOwner.Gripper != null && !iOwner.GripperAttached && !iOwner.AttachedToGripped) && !iOwner.Attacking)
      return (BaseState) EntangledState.Instance;
    if (iOwner.CharacterBody.IsPushed)
      return (BaseState) PushedState.Instance;
    if (iOwner.IsKnockedDown)
      return (BaseState) KnockDownState.Instance;
    if (iOwner.IsGripping && (iOwner.ShouldReleaseGrip || (double) iOwner.GripDamageAccumulation > (double) iOwner.HitTolerance))
    {
      if (iOwner.CharacterBody.IsTouchingGround && iOwner.GripperAttached)
        return (BaseState) DropState.Instance;
      Vector3 iImpulse = new Vector3();
      iOwner.CharacterBody.AddImpulseVelocity(ref iImpulse);
      iOwner.KnockDown();
    }
    if (iOwner.IsHit)
      return (BaseState) HitState.Instance;
    return iOwner.IsStunned && !iOwner.IsHit && !iOwner.IsKnockedDown && !iOwner.CharacterBody.IsPushed ? (BaseState) StunState.Instance : (BaseState) null;
  }

  public virtual BaseState UpdateActions(Character iOwner, float iDeltaTime)
  {
    if (iOwner.CharacterBody.IsJumping && !iOwner.IsGripping)
      return (BaseState) JumpState.Instance;
    if (iOwner.IsPanicing | iOwner.IsStumbling | iOwner.IsFeared && !iOwner.IsEntangled)
      return (BaseState) PanicState.Instance;
    if (iOwner.Boosts > 0)
    {
      if (BoostState.Instance.ShieldToBoost(iOwner) != null || iOwner.IsSelfShielded)
        return (BaseState) BoostState.Instance;
      iOwner.Boosts = 0;
    }
    return iOwner.IsGripping && !iOwner.AttachedToGripped ? (BaseState) GrippingState.Instance : (BaseState) null;
  }

  public virtual BaseState Update(Character iOwner, float iDeltaTime) => (BaseState) null;

  public virtual void OnExit(Character iOwner)
  {
  }
}
