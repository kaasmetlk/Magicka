// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.PushedState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class PushedState : BaseState
{
  private static PushedState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static PushedState Instance
  {
    get
    {
      if (PushedState.mSingelton == null)
      {
        lock (PushedState.mSingeltonLock)
        {
          if (PushedState.mSingelton == null)
            PushedState.mSingelton = new PushedState();
        }
      }
      return PushedState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.GoToAnimation(Animations.hit_slide, 0.05f);
    iOwner.ReleaseAttachedCharacter();
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null && !(baseState is FlyingState))
      return baseState;
    if (iOwner.CharacterBody.IsPushed)
      return (BaseState) null;
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }
}
