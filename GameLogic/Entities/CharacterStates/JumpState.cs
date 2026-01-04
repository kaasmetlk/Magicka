// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.JumpState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class JumpState : BaseState
{
  private static JumpState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static JumpState Instance
  {
    get
    {
      if (JumpState.mSingelton == null)
      {
        lock (JumpState.mSingeltonLock)
        {
          if (JumpState.mSingelton == null)
            JumpState.mSingelton = new JumpState();
        }
      }
      return JumpState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.CharacterBody.AllowRotate = false;
    iOwner.CharacterBody.AllowMove = false;
    if (iOwner.HasAnimation(Animations.move_jump_up))
      iOwner.GoToAnimation(Animations.move_jump_up, 0.2f);
    else
      iOwner.GoToAnimation(Animations.move_jump_mid, 0.2f);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (!iOwner.CharacterBody.IsJumping)
      return (BaseState) LandState.Instance;
    if (iOwner.IsGripping)
      return (BaseState) GrippingState.Instance;
    if ((double) iOwner.Body.Velocity.Y < 0.0 && iOwner.CurrentAnimation == Animations.move_jump_up && iOwner.HasAnimation(Animations.move_jump_down) && !iOwner.AnimationController.CrossFadeEnabled)
      iOwner.GoToAnimation(Animations.move_jump_down, 0.3f);
    return (BaseState) null;
  }
}
