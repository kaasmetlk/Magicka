// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.FloorStomp
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class FloorStomp : SpecialAbility, IAbilityEffect
{
  private static List<FloorStomp> sCache;
  private Magicka.GameLogic.Entities.Character mOwner;
  public PlayState mPlayState;
  private VisualEffectReference mEffect;
  private float mTTL;
  private float FIRST_STOMP;
  private float SECOND_STOMP;
  private float THIRD_STOMP;

  public static FloorStomp GetInstance()
  {
    if (FloorStomp.sCache.Count <= 0)
      return new FloorStomp();
    FloorStomp instance = FloorStomp.sCache[FloorStomp.sCache.Count - 1];
    FloorStomp.sCache.RemoveAt(FloorStomp.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    FloorStomp.sCache = new List<FloorStomp>(iNr);
    for (int index = 0; index < iNr; ++index)
      FloorStomp.sCache.Add(new FloorStomp());
  }

  public FloorStomp(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_grease".GetHashCodeCustom())
  {
  }

  private FloorStomp()
    : base(Magicka.Animations.cast_magick_sweep, "#magick_grease".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("FloorStomp can not be cast without an owner!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mTTL = 8f;
    this.FIRST_STOMP = 1.15f;
    this.SECOND_STOMP = 2.1f;
    this.THIRD_STOMP = 3.05f;
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    this.mPlayState = iPlayState;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    StatusEffect[] statusEffects = this.mOwner.GetStatusEffects();
    if (this.mOwner.HasStatus(StatusEffects.Cold))
      iDeltaTime *= statusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown();
    if ((double) this.mOwner.ZapTimer > 0.0)
      iDeltaTime *= this.mOwner.ZapModifier;
    this.mTTL -= iDeltaTime;
    this.FIRST_STOMP -= iDeltaTime;
    this.SECOND_STOMP -= iDeltaTime;
    this.THIRD_STOMP -= iDeltaTime;
    if ((double) this.FIRST_STOMP < 0.0)
    {
      Spell iSpell = new Spell();
      iSpell.Element = Elements.Earth | Elements.Fire;
      iSpell.EarthMagnitude = 3f;
      iSpell.FireMagnitude = 2f;
      this.mOwner.CurrentSpell = ProjectileSpell.GetFromCache();
      this.mOwner.CurrentSpell.CastArea(iSpell, (ISpellCaster) this.mOwner, false);
      List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, iSpell.BlastSize() * 10f, false);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Barrier || entities[index] is Shield)
          entities[index].Kill();
      }
      this.mPlayState.EntityManager.ReturnEntityList(entities);
      this.FIRST_STOMP = 1000000f;
    }
    if ((double) this.SECOND_STOMP < 0.0)
    {
      Spell iSpell = new Spell();
      iSpell.Element = Elements.Earth | Elements.Fire;
      iSpell.EarthMagnitude = 5f;
      iSpell.FireMagnitude = 2f;
      this.mOwner.CurrentSpell = ProjectileSpell.GetFromCache();
      this.mOwner.CurrentSpell.CastArea(iSpell, (ISpellCaster) this.mOwner, false);
      List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, iSpell.BlastSize() * 10f, false);
      for (int index = 0; index < entities.Count; ++index)
      {
        if (entities[index] is Barrier || entities[index] is Shield)
          entities[index].Kill();
      }
      this.mPlayState.EntityManager.ReturnEntityList(entities);
      this.SECOND_STOMP = 1000000f;
    }
    if ((double) this.THIRD_STOMP >= 0.0)
      return;
    Spell iSpell1 = new Spell();
    iSpell1.Element = Elements.Earth | Elements.Fire;
    iSpell1.EarthMagnitude = 8f;
    iSpell1.FireMagnitude = 2f;
    this.mOwner.CurrentSpell = ProjectileSpell.GetFromCache();
    this.mOwner.CurrentSpell.CastArea(iSpell1, (ISpellCaster) this.mOwner, false);
    List<Entity> entities1 = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, iSpell1.BlastSize() * 10f, false);
    for (int index = 0; index < entities1.Count; ++index)
    {
      if (entities1[index] is Barrier || entities1[index] is Shield)
        entities1[index].Kill();
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities1);
    this.THIRD_STOMP = 1000000f;
  }

  public void OnRemove()
  {
    this.mTTL = 0.0f;
    EffectManager.Instance.Stop(ref this.mEffect);
    FloorStomp.sCache.Add(this);
  }
}
