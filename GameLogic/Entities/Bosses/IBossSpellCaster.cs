// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.IBossSpellCaster
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public interface IBossSpellCaster : IBoss
{
  void AddSelfShield(int iIndex, Spell iSpell);

  void RemoveSelfShield(int iIndex, Character.SelfShieldType iType);

  Magicka.GameLogic.Spells.CastType CastType(int iIndex);

  float SpellPower(int iIndex);

  void SpellPower(int iIndex, float iSpellPower);

  SpellEffect CurrentSpell(int iIndex);

  void CurrentSpell(int iIndex, SpellEffect iEffect);
}
