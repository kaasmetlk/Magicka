// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.IBossState`1
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public interface IBossState<T> where T : IBoss
{
  void OnEnter(T iOwner);

  void OnUpdate(float iDeltaTime, T iOwner);

  void OnExit(T iOwner);
}
