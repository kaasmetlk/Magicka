// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.IStatusEffected
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;

#nullable disable
namespace Magicka.GameLogic.Entities;

public interface IStatusEffected : IDamageable
{
  bool HasStatus(StatusEffects iStatus);

  StatusEffect[] GetStatusEffects();

  float StatusMagnitude(StatusEffects iStatus);

  void Damage(float iDamage, Elements iElement);

  float Volume { get; }
}
