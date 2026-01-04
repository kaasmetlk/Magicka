// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.IAbilityEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using PolygonHead;

#nullable disable
namespace Magicka.GameLogic;

public interface IAbilityEffect
{
  bool IsDead { get; }

  void Update(DataChannel iDataChannel, float iDeltaTime);

  void OnRemove();
}
