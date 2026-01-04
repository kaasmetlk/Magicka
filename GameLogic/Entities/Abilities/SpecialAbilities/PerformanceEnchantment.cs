// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.PerformanceEnchantment
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class PerformanceEnchantment : SpecialAbility, IAbilityEffect
{
  private const float BOOST_TTL = 6f;
  private const float WITHDRAWAL_TTL = 4f;
  private const float BOOST_RESISTANCE = 0.5f;
  private const float BOOST_DAMAGE = 2f;
  private const float WITHDRAWAL_RESISTANCE = 2f;
  private const float WITHDRAWAL_DAMAGE = 0.25f;
  private static List<PerformanceEnchantment> sCache;
  private static List<PerformanceEnchantment> sActiveEnchantments;
  private static readonly int EFFECT = "special_woot_performance".GetHashCodeCustom();
  private static readonly int DISPLAY_NAME = "#magick_performance".GetHashCodeCustom();
  public static readonly int SOUND_BUFF = "woot_magick_enchantment_buff".GetHashCodeCustom();
  public static readonly int SOUND_NERF = "woot_magick_enchantment_debuff".GetHashCodeCustom();
  private float mTTL;
  private bool mBoosted;
  private Magicka.GameLogic.Entities.Character mOwner;
  private AuraStorage resistanceAura;
  private AuraStorage damageAura;
  private VisualEffectReference mEffect;

  public static PerformanceEnchantment GetInstance()
  {
    if (PerformanceEnchantment.sCache.Count > 0)
    {
      PerformanceEnchantment instance = PerformanceEnchantment.sCache[PerformanceEnchantment.sCache.Count - 1];
      PerformanceEnchantment.sCache.RemoveAt(PerformanceEnchantment.sCache.Count - 1);
      PerformanceEnchantment.sActiveEnchantments.Add(instance);
      return instance;
    }
    PerformanceEnchantment instance1 = new PerformanceEnchantment();
    PerformanceEnchantment.sActiveEnchantments.Add(instance1);
    return instance1;
  }

  public static void InitializeCache(int iNr)
  {
    PerformanceEnchantment.sCache = new List<PerformanceEnchantment>(iNr);
    PerformanceEnchantment.sActiveEnchantments = new List<PerformanceEnchantment>(iNr);
    for (int index = 0; index < iNr; ++index)
      PerformanceEnchantment.sCache.Add(new PerformanceEnchantment());
  }

  public PerformanceEnchantment()
    : base(Magicka.Animations.cast_magick_self, PerformanceEnchantment.DISPLAY_NAME)
  {
    this.mTTL = 10f;
  }

  public PerformanceEnchantment(Magicka.Animations iAnimation)
    : base(iAnimation, PerformanceEnchantment.DISPLAY_NAME)
  {
    this.mTTL = 10f;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("PerformanceEnchantment have to be cast by a character!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    return this.Execute(iOwner, iPlayState, true);
  }

  public bool Execute(ISpellCaster iOwner, PlayState iPlayState, bool iSound)
  {
    if (!(iOwner is Magicka.GameLogic.Entities.Character))
    {
      this.OnRemove();
      return false;
    }
    for (int index = 0; index < PerformanceEnchantment.sActiveEnchantments.Count; ++index)
    {
      if (PerformanceEnchantment.sActiveEnchantments[index].mOwner == iOwner)
      {
        PerformanceEnchantment.sActiveEnchantments[index].mTTL = 10f;
        Vector3 direction = iOwner.Direction;
        Vector3 position = iOwner.Position;
        EffectManager.Instance.Stop(ref PerformanceEnchantment.sActiveEnchantments[index].mEffect);
        EffectManager.Instance.StartEffect(PerformanceEnchantment.EFFECT, ref position, ref direction, out PerformanceEnchantment.sActiveEnchantments[index].mEffect);
        AudioManager.Instance.PlayCue(Banks.Additional, PerformanceEnchantment.SOUND_BUFF, iOwner.AudioEmitter);
        PerformanceEnchantment.sActiveEnchantments[index].mBoosted = true;
        this.OnRemove();
        return true;
      }
    }
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    this.mTTL = 10f;
    if (this.mOwner != null)
    {
      this.Boost();
      SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
      return true;
    }
    this.OnRemove();
    return false;
  }

  public bool IsDead
  {
    get
    {
      if ((double) this.mTTL > 0.0)
        return false;
      this.mOwner.ClearAura();
      return true;
    }
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if (this.mOwner == null || this.mOwner.CharacterBody == null)
      return;
    if ((double) this.mTTL < 4.0)
    {
      this.Withdrawal();
      if (this.mOwner.CharacterBody.IsBodyEnabled)
        this.mOwner.CharacterBody.SpeedMultiplier *= (float) (0.5 + (0.5 - Math.Pow(0.75, (double) this.mTTL)));
    }
    else if (this.mOwner.CharacterBody.IsBodyEnabled)
      this.mOwner.CharacterBody.SpeedMultiplier *= (float) (1.0 + (1.0 - Math.Pow(0.5, (double) this.mTTL - 4.0)));
    Vector3 direction = this.mOwner.Direction;
    Vector3 position = this.mOwner.Position;
    EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref position, ref direction);
  }

  public void Boost()
  {
    this.mBoosted = true;
    this.resistanceAura = new AuraStorage(new AuraBuff(new BuffStorage(new BuffResistance(new Resistance()
    {
      ResistanceAgainst = Elements.All,
      Multiplier = 0.5f
    }), VisualCategory.Defensive, Spell.SHIELDCOLOR)), AuraTarget.Self, AuraType.Buff, 0, this.mTTL, 0.0f, VisualCategory.Special, Spell.SHIELDCOLOR, (int[]) null, Factions.NONE);
    this.mOwner.AddAura(ref this.resistanceAura, true);
    this.damageAura = new AuraStorage(new AuraBuff(new BuffStorage(new BuffBoostDamage(new Damage(AttackProperties.Damage, Elements.All, 2f, 2f)), VisualCategory.Offensive, new Vector3())), AuraTarget.Self, AuraType.Buff, 0, this.mTTL, 0.0f, VisualCategory.None, new Vector3(), (int[]) null, Factions.NONE);
    this.mOwner.AddAura(ref this.damageAura, true);
    Vector3 direction = this.mOwner.Direction;
    Vector3 position = this.mOwner.Position;
    EffectManager.Instance.StartEffect(PerformanceEnchantment.EFFECT, ref position, ref direction, out this.mEffect);
    AudioManager.Instance.PlayCue(Banks.Additional, PerformanceEnchantment.SOUND_BUFF, this.mOwner.AudioEmitter);
  }

  public void Withdrawal()
  {
    if (!this.mBoosted)
      return;
    this.mBoosted = false;
    AuraBuff auraBuff = new AuraBuff(new BuffStorage(new BuffResistance(new Resistance()
    {
      ResistanceAgainst = Elements.All,
      Multiplier = 2f
    }), VisualCategory.Defensive, Spell.SHIELDCOLOR));
    this.damageAura.AuraBuff = new AuraBuff(new BuffStorage(new BuffBoostDamage(new Damage(AttackProperties.Damage, Elements.All, 0.25f, 0.25f)), VisualCategory.Offensive, new Vector3()));
    this.resistanceAura.AuraBuff = auraBuff;
    if (EffectManager.Instance.IsActive(ref this.mEffect))
      EffectManager.Instance.Stop(ref this.mEffect);
    AudioManager.Instance.PlayCue(Banks.Additional, PerformanceEnchantment.SOUND_NERF, this.mOwner.AudioEmitter);
  }

  public void OnRemove()
  {
    this.mOwner = (Magicka.GameLogic.Entities.Character) null;
    PerformanceEnchantment.sActiveEnchantments.Remove(this);
    PerformanceEnchantment.sCache.Add(this);
    if (!EffectManager.Instance.IsActive(ref this.mEffect))
      return;
    EffectManager.Instance.Stop(ref this.mEffect);
  }
}
