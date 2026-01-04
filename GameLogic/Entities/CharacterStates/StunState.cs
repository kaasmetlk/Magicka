// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.StunState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class StunState : BaseState
{
  private static StunState sSingelton;
  private static volatile object sSingeltonLock = new object();

  public static StunState Instance
  {
    get
    {
      if (StunState.sSingelton == null)
      {
        lock (StunState.sSingeltonLock)
        {
          if (StunState.sSingelton == null)
            StunState.sSingelton = new StunState();
        }
      }
      return StunState.sSingelton;
    }
  }

  public override void OnEnter(Character iOwner)
  {
    if (iOwner.HasAnimation(Animations.stunned))
      iOwner.GoToAnimation(Animations.stunned, 0.35f);
    else
      iOwner.GoToAnimation(Animations.idle_wnd, 0.35f);
    iOwner.CharacterBody.AllowMove = false;
    iOwner.CharacterBody.AllowRotate = false;
    base.OnEnter(iOwner);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    return !iOwner.IsStunned ? (BaseState) IdleState.Instance : (BaseState) null;
  }

  public override void OnExit(Character iOwner)
  {
    iOwner.CharacterBody.AllowMove = true;
    iOwner.CharacterBody.AllowRotate = true;
    base.OnExit(iOwner);
  }
}
