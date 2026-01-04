// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.KnockDownState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class KnockDownState : BaseState
{
  private static KnockDownState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static KnockDownState Instance
  {
    get
    {
      if (KnockDownState.mSingelton == null)
      {
        lock (KnockDownState.mSingeltonLock)
        {
          if (KnockDownState.mSingelton == null)
            KnockDownState.mSingelton = new KnockDownState();
        }
      }
      return KnockDownState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.CharacterBody.AllowRotate = false;
    iOwner.GoToAnimation(Animations.hit_stun_begin, 0.05f);
    iOwner.ReleaseAttachedCharacter();
    if (!(iOwner.PreviousState is ChargeState))
      return;
    iOwner.CastSpell(true, "");
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    iOwner.IsHit = false;
    iOwner.IsKnockedDown = false;
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.AnimationController.HasFinished && !iOwner.AnimationController.CrossFadeEnabled || iOwner.HasStatus(StatusEffects.Frozen))
    {
      if (iOwner.CurrentAnimation == Animations.hit_stun_begin)
        iOwner.GoToAnimation(Animations.hit_stun_end, 0.05f);
      else
        return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
    }
    return (BaseState) null;
  }

  public override void OnExit(Character iOwner)
  {
    iOwner.IsHit = false;
    iOwner.IsKnockedDown = false;
  }
}
