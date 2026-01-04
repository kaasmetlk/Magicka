// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.AlarmState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class AlarmState : BaseState
{
  private static AlarmState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static AlarmState Instance
  {
    get
    {
      if (AlarmState.mSingelton == null)
      {
        lock (AlarmState.mSingeltonLock)
        {
          if (AlarmState.mSingelton == null)
            AlarmState.mSingelton = new AlarmState();
        }
      }
      return AlarmState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    if ((iOwner as NonPlayerCharacter).AI.AlertMode == AlertMode.Discover)
      iOwner.GoToAnimation(Animations.spec_alert0, 0.2f);
    else
      iOwner.GoToAnimation(Animations.spec_alert1, 0.2f);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    AnimationController animationController = iOwner.AnimationController;
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.NextAttackAnimation != Animations.None || !animationController.HasFinished && (!animationController.IsLooping || animationController.CrossFadeEnabled))
      return (BaseState) null;
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }

  public override void OnExit(Character iOwner) => iOwner.IsAlerted = false;
}
