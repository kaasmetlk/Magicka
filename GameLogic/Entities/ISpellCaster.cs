// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.ISpellCaster
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.Entities;

public interface ISpellCaster : IStatusEffected, IDamageable
{
  MissileEntity GetMissileInstance();

  AudioEmitter AudioEmitter { get; }

  AnimationController AnimationController { get; }

  void AddSelfShield(Spell iSpell);

  void RemoveSelfShield(Character.SelfShieldType iType);

  CastType CastType { get; }

  Vector3 Direction { get; }

  PlayState PlayState { get; }

  Matrix CastSource { get; }

  Matrix WeaponSource { get; }

  float SpellPower { get; set; }

  SpellEffect CurrentSpell { get; set; }

  bool HasPassiveAbility(Item.PassiveAbilities iAbility);
}
