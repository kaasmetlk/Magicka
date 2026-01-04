// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.HitState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class HitState : BaseState
{
  private static HitState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static HitState Instance
  {
    get
    {
      if (HitState.mSingelton == null)
      {
        lock (HitState.mSingeltonLock)
        {
          if (HitState.mSingelton == null)
            HitState.mSingelton = new HitState();
        }
      }
      return HitState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.CharacterBody.AllowRotate = false;
    iOwner.GoToAnimation(Animations.hit, 0.1f);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (!(iOwner.AnimationController.HasFinished | iOwner.AnimationController.IsLooping | iOwner.HasStatus(StatusEffects.Frozen)))
      return (BaseState) null;
    if (iOwner.PreviousState is ChargeState)
      iOwner.CastSpell(true, "");
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }

  public override void OnExit(Character iOwner) => iOwner.IsHit = false;
}
