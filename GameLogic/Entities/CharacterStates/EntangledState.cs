// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.EntangledState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class EntangledState : BaseState
{
  private static EntangledState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static EntangledState Instance
  {
    get
    {
      if (EntangledState.mSingelton == null)
      {
        lock (EntangledState.mSingeltonLock)
        {
          if (EntangledState.mSingelton == null)
            EntangledState.mSingelton = new EntangledState();
        }
      }
      return EntangledState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.CharacterBody.AllowMove = false;
    iOwner.CharacterBody.AllowRotate = false;
    iOwner.GoToAnimation(Animations.spec_entangled, 0.2f);
    iOwner.SpecialIdleAnimation = Animations.None;
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner is Avatar && iOwner.CastType != CastType.None)
      return (BaseState) CastState.Instance;
    if (iOwner.Attacking)
      return (BaseState) AttackState.Instance;
    if (!iOwner.IsEntangled && iOwner.Gripper == null)
      return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
    if (iOwner.CurrentAnimation != Animations.spec_entangled & iOwner.AnimationController.HasFinished)
      iOwner.GoToAnimation(Animations.spec_entangled, 0.1f);
    return (BaseState) null;
  }

  public override void OnExit(Character iOwner)
  {
  }
}
