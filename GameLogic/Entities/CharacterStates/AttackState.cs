// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.AttackState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class AttackState : BaseState
{
  private static AttackState mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly int SOUND_STATEATTACK = "wizard_attack".GetHashCodeCustom();

  public static AttackState Instance
  {
    get
    {
      if (AttackState.mSingelton == null)
      {
        lock (AttackState.mSingeltonLock)
        {
          if (AttackState.mSingelton == null)
            AttackState.mSingelton = new AttackState();
        }
      }
      return AttackState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.Ethereal(false, 1f, 1f);
    if (!iOwner.JustCastInvisible)
      iOwner.SetInvisible(0.0f);
    else
      iOwner.JustCastInvisible = false;
    if (iOwner.NextAttackAnimation == Animations.None)
      iOwner.GoToAnimation(Animations.idle, 0.075f);
    else
      iOwner.GoToAnimation(iOwner.NextAttackAnimation, 0.075f);
    iOwner.NextAttackAnimation = Animations.None;
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    AnimationController animationController = iOwner.AnimationController;
    iOwner.CharacterBody.AllowRotate = !iOwner.IsEntangled & (animationController.CrossFadeEnabled | iOwner.AllowAttackRotate | (double) animationController.Time < 0.10000000149011612);
    if (iOwner.NextAttackAnimation != Animations.None && iOwner.Attacking && !animationController.CrossFadeEnabled && animationController.HasFinished)
    {
      if (iOwner.CurrentSpell != null)
      {
        iOwner.CurrentSpell.Stop((ISpellCaster) iOwner);
        iOwner.CurrentSpell = (SpellEffect) null;
      }
      if (iOwner.CurrentAnimation == Animations.attack_recoil)
      {
        iOwner.NextAttackAnimation = Animations.None;
      }
      else
      {
        iOwner.GoToAnimation(iOwner.NextAttackAnimation, 0.2f);
        iOwner.NextAttackAnimation = Animations.None;
      }
    }
    if (iOwner.NextAttackAnimation != Animations.None || !animationController.HasFinished && (!animationController.IsLooping || animationController.CrossFadeEnabled))
      return (BaseState) null;
    iOwner.Attacking = false;
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 || iOwner.IsEntangled ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }

  public override void OnExit(Character iOwner)
  {
    if (iOwner is NonPlayerCharacter nonPlayerCharacter)
      nonPlayerCharacter.AI.BusyAbility = (Ability) null;
    iOwner.Attacking = false;
    if (iOwner.CurrentSpell != null)
    {
      iOwner.CurrentSpell.Stop((ISpellCaster) iOwner);
      iOwner.CurrentSpell = (SpellEffect) null;
    }
    iOwner.CastType = CastType.None;
    iOwner.Equipment[0].Item.StopGunfire();
    iOwner.Equipment[1].Item.StopGunfire();
  }
}
