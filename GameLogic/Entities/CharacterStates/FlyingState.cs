// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.FlyingState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class FlyingState : BaseState
{
  private static FlyingState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static FlyingState Instance
  {
    get
    {
      if (FlyingState.mSingelton == null)
      {
        lock (FlyingState.mSingeltonLock)
        {
          if (FlyingState.mSingelton == null)
            FlyingState.mSingelton = new FlyingState();
        }
      }
      return FlyingState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.CharacterBody.AllowRotate = false;
    iOwner.GoToAnimation(Animations.hit_fly, 0.03f);
    iOwner.ReleaseAttachedCharacter();
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    iOwner.IsHit = false;
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.IsGripping | iOwner.IsGripped)
      return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
    if (!iOwner.CharacterBody.IsTouchingGround)
      return (BaseState) null;
    if (iOwner.IsKnockedDown)
      return (BaseState) KnockDownState.Instance;
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }

  public override void OnExit(Character iOwner) => iOwner.IsHit = false;
}
