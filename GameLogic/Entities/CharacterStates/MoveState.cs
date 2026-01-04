// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.MoveState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public sealed class MoveState : BaseState
{
  private static MoveState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static MoveState Instance
  {
    get
    {
      if (MoveState.mSingelton == null)
      {
        lock (MoveState.mSingeltonLock)
        {
          if (MoveState.mSingelton == null)
            MoveState.mSingelton = new MoveState();
        }
      }
      return MoveState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.CharacterBody.AllowMove = true;
    iOwner.CharacterBody.AllowRotate = true;
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = (this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime)) ?? this.UpdateActions(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.Dashing)
      return (BaseState) DashState.Instance;
    if (iOwner.Attacking)
      return (BaseState) AttackState.Instance;
    if (iOwner.IsBlocking)
      return (BaseState) BlockState.Instance;
    if (iOwner is Avatar && iOwner.CastType != CastType.None)
    {
      Avatar avatar = iOwner as Avatar;
      if ((iOwner.CastType == CastType.Weapon || !avatar.ChargeUnlocked || iOwner.Spell.GetSpellType() != SpellType.Projectile || iOwner.CastType != CastType.Force) && (!avatar.ChargeUnlocked || iOwner.Spell.GetSpellType() != SpellType.Push || iOwner.CastType != CastType.Force && iOwner.CastType != CastType.Area))
        return (BaseState) CastState.Instance;
      if (!(iOwner.CurrentState is AttackState))
        return (BaseState) ChargeState.Instance;
    }
    else
    {
      if ((double) iOwner.ZapTimer >= 1.4012984643248171E-45)
        return (BaseState) null;
      if ((double) iOwner.CharacterBody.Movement.LengthSquared() <= 0.0099999997764825821)
        return (BaseState) IdleState.Instance;
      if (iOwner.HasStatus(StatusEffects.Poisoned))
        iOwner.GoToAnimation(Animations.move_wnd, 0.4f);
      else if ((double) iOwner.CharacterBody.NormalizedVelocity >= 0.60000002384185791)
        iOwner.GoToAnimation(Animations.move_run, 0.25f);
      else if ((double) iOwner.CharacterBody.NormalizedVelocity < 0.40000000596046448)
        iOwner.GoToAnimation(Animations.move_walk, 0.25f);
      else if (iOwner.CurrentAnimation != Animations.move_run & iOwner.CurrentAnimation != Animations.move_walk)
        iOwner.GoToAnimation(Animations.move_walk, 0.25f);
    }
    return (BaseState) null;
  }

  public override void OnExit(Character iOwner) => iOwner.CharacterBody.AllowMove = false;
}
