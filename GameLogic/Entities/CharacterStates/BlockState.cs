// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.BlockState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class BlockState : BaseState
{
  private static BlockState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static BlockState Instance
  {
    get
    {
      if (BlockState.mSingelton == null)
      {
        lock (BlockState.mSingeltonLock)
        {
          if (BlockState.mSingelton == null)
            BlockState.mSingelton = new BlockState();
        }
      }
      return BlockState.mSingelton;
    }
  }

  public override void OnEnter(Character iOwner) => iOwner.GoToAnimation(Animations.block, 0.1f);

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime) ?? this.UpdateHit(iOwner, iDeltaTime);
    if (baseState != null)
      return baseState;
    if (iOwner.Attacking)
      return (BaseState) AttackState.Instance;
    if (iOwner.IsBlocking)
      return (BaseState) null;
    return (double) iOwner.CharacterBody.Movement.Length() < 1.4012984643248171E-45 ? (BaseState) IdleState.Instance : (BaseState) MoveState.Instance;
  }
}
