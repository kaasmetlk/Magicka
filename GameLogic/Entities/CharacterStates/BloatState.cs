// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.CharacterStates.BloatState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.CharacterStates;

public class BloatState : BaseState
{
  private static BloatState mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static BloatState Instance
  {
    get
    {
      if (BloatState.mSingelton == null)
      {
        lock (BloatState.mSingeltonLock)
        {
          if (BloatState.mSingelton == null)
            BloatState.mSingelton = new BloatState();
        }
      }
      return BloatState.mSingelton;
    }
  }

  public override BaseState Update(Character iOwner, float iDeltaTime)
  {
    return iOwner.BloatKilled ? (BaseState) DeadState.Instance : (BaseState) null;
  }
}
