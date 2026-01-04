// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.LandState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class LandState : BaseState
{
  private static LandState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static LandState Instance
  {
    get
    {
      if (LandState.mSingelton == null)
      {
        lock (LandState.mSingeltonLock)
        {
          if (LandState.mSingelton == null)
            LandState.mSingelton = new LandState();
        }
      }
      return LandState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.CharacterBody.AllowRotate = false;
    iOwner.CharacterBody.AllowMove = false;
    iOwner.GoToAnimation(Animations.move_jump_end, 0.13f);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    if (iOwner.CharacterBody.IsTouchingGround)
    {
      if (iOwner.CurrentAnimation != Animations.move_jump_end && iOwner.CurrentAnimation != Animations.attack_recoil)
        iOwner.GoToAnimation(Animations.move_jump_end, 0.1f);
      else if (iOwner.AnimationController.HasFinished && !iOwner.AnimationController.CrossFadeEnabled)
        return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
    }
    else if (iOwner.IsGripped)
      return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
    return (BaseState) null;
  }
}
