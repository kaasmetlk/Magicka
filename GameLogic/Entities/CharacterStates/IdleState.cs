// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.IdleState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Spells;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public sealed class IdleState : BaseState
{
  private static IdleState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static IdleState Instance
  {
    get
    {
      if (IdleState.mSingelton == null)
      {
        lock (IdleState.mSingeltonLock)
        {
          if (IdleState.mSingelton == null)
            IdleState.mSingelton = new IdleState();
        }
      }
      return IdleState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    if (iOwner is NonPlayerCharacter nonPlayerCharacter)
      nonPlayerCharacter.AI.BusyAbility = (Ability) null;
    if (!iOwner.IsEntangled)
    {
      iOwner.CharacterBody.AllowMove = true;
      iOwner.CharacterBody.AllowRotate = true;
    }
    if (iOwner.NetworkAnimation != Animations.None)
    {
      iOwner.GoToAnimation(iOwner.NetworkAnimation, iOwner.NetworkAnimationBlend);
      iOwner.NetworkAnimation = Animations.None;
    }
    else if (iOwner.SpecialIdleAnimation != Animations.None)
      iOwner.GoToAnimation(iOwner.SpecialIdleAnimation, 0.2f);
    else if (iOwner.GrippedCharacter != null && !iOwner.AttachedToGripped && iOwner.HasAnimation(Animations.special1))
      iOwner.GoToAnimation(Animations.special1, 0.2f);
    else if (iOwner.HasStatus(StatusEffects.Poisoned))
      iOwner.GoToAnimation(Animations.idle_wnd, 0.2f);
    else if (iOwner.IsAggressive)
      iOwner.GoToAnimation(Animations.idle_agg, 0.2f);
    else if (iOwner.CurrentAnimation == Animations.idle_agg | iOwner.CurrentAnimation == Animations.idle_wnd)
      iOwner.GoToAnimation(Animations.idle, 0.3f);
    else
      iOwner.GoToAnimation(Animations.idle, 0.2f);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = (this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime)) ?? this.UpdateActions(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.IsAlerted)
      return (BaseState) AlarmState.Instance;
    if (iOwner.CharacterBody.Moving)
      return (BaseState) MoveState.Instance;
    if (iOwner is Avatar && iOwner.CastType != CastType.None && !iOwner.PlayState.IsInCutscene)
    {
      Avatar avatar = iOwner as Avatar;
      SpellType spellType = iOwner.Spell.GetSpellType();
      return avatar.ChargeUnlocked && spellType == SpellType.Projectile && iOwner.CastType == CastType.Force || avatar.ChargeUnlocked && spellType == SpellType.Push && iOwner.CastType == CastType.Force | iOwner.CastType == CastType.Area ? (BaseState) ChargeState.Instance : (BaseState) CastState.Instance;
    }
    if (iOwner.Dashing)
      return (BaseState) DashState.Instance;
    if (iOwner.Attacking)
      return (BaseState) AttackState.Instance;
    if (iOwner.GripAttacking)
      return (BaseState) GripAttackState.Instance;
    if (iOwner.IsBlocking)
      return (BaseState) BlockState.Instance;
    if (iOwner.IsGripped && iOwner.Gripper.GripperAttached)
      return (BaseState) EntangledState.Instance;
    if (iOwner.AnimationController.HasFinished && !iOwner.AnimationController.CrossFadeEnabled)
    {
      if (iOwner.GrippedCharacter != null && !iOwner.AttachedToGripped)
        iOwner.GoToAnimation(Animations.special1, 0.2f);
      if (iOwner.HasStatus(StatusEffects.Poisoned))
        iOwner.GoToAnimation(Animations.idle_wnd, 0.3f);
      else if (iOwner.IsAggressive)
        iOwner.GoToAnimation(Animations.idle_agg, 0.2f);
      else if (iOwner.SpecialIdleAnimation != Animations.None)
        iOwner.GoToAnimation(iOwner.SpecialIdleAnimation, 0.1f);
      else
        iOwner.GoToAnimation(Animations.idle, 0.5f);
    }
    return (BaseState) null;
  }

  public override void OnExit(Character iOwner)
  {
    iOwner.CharacterBody.AllowMove = false;
    iOwner.SpecialIdleAnimation = Animations.None;
  }
}
