// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.BusyState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

internal class BusyState : BaseState
{
  private static BusyState sSingelton;
  private static volatile object sSingeltonLock = new object();

  public static BusyState Instance
  {
    get
    {
      if (BusyState.sSingelton == null)
      {
        lock (BusyState.sSingeltonLock)
        {
          if (BusyState.sSingelton == null)
            BusyState.sSingelton = new BusyState();
        }
      }
      return BusyState.sSingelton;
    }
  }

  private BusyState()
  {
  }

  public override void OnEnter(Character iOwner)
  {
    iOwner.CharacterBody.AllowMove = false;
    iOwner.CharacterBody.AllowRotate = false;
    iOwner.GoToAnimation(Animations.emote_confused0, 0.2f);
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    return !iOwner.PlayState.Inventory.Active ? (BaseState) IdleState.Instance : (BaseState) null;
  }

  public override void OnExit(Character iOwner)
  {
    iOwner.PlayState.Inventory.Close(iOwner);
    iOwner.CharacterBody.AllowMove = true;
    iOwner.CharacterBody.AllowRotate = true;
  }
}
