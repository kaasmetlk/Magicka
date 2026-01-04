// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.RessurectionState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class RessurectionState : BaseState
{
  private static RessurectionState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static RessurectionState Instance
  {
    get
    {
      if (RessurectionState.mSingelton == null)
      {
        lock (RessurectionState.mSingeltonLock)
        {
          if (RessurectionState.mSingelton == null)
            RessurectionState.mSingelton = new RessurectionState();
        }
      }
      return RessurectionState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.CharacterBody.AllowMove = false;
    iOwner.CharacterBody.AllowRotate = false;
    iOwner.ForceAnimation(iOwner.SpawnAnimation);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    if (iOwner.Bloating)
      return (BaseState) BloatState.Instance;
    if (iOwner.Overkilled && !iOwner.CannotDieWithoutExplicitKill)
      return (BaseState) DeadState.Instance;
    if ((!iOwner.AnimationController.HasFinished || iOwner.AnimationController.CrossFadeEnabled) && !iOwner.AnimationController.IsLooping)
      return (BaseState) null;
    if ((double) iOwner.HitPoints <= 0.0)
      return (BaseState) DeadState.Instance;
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }

  public override void OnExit(Character iOwner) => iOwner.CharacterBody.AllowRotate = true;
}
