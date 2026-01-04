// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.BreakBarriers
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class BreakBarriers : SpecialAbility, IAbilityEffect
{
  private static List<BreakBarriers> sCache;
  private Magicka.GameLogic.Entities.Character mOwner;
  public PlayState mPlayState;
  private VisualEffectReference mEffect;
  private float TIME = 5f;
  private float mTTL;

  public static BreakBarriers GetInstance()
  {
    if (BreakBarriers.sCache.Count <= 0)
      return new BreakBarriers();
    BreakBarriers instance = BreakBarriers.sCache[BreakBarriers.sCache.Count - 1];
    BreakBarriers.sCache.RemoveAt(BreakBarriers.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    BreakBarriers.sCache = new List<BreakBarriers>(iNr);
    for (int index = 0; index < iNr; ++index)
      BreakBarriers.sCache.Add(new BreakBarriers());
  }

  public BreakBarriers(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_grease".GetHashCodeCustom())
  {
  }

  private BreakBarriers()
    : base(Magicka.Animations.None, "#magick_grease".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new Exception("BarrierBreak needs an owner!");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mTTL = this.TIME;
    this.mPlayState = iPlayState;
    this.mOwner = iOwner as Magicka.GameLogic.Entities.Character;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if (this.mOwner == null)
      return;
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, 3.5f * this.mOwner.Radius, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Barrier || entities[index] is Shield)
        entities[index].Kill();
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
  }

  public void OnRemove()
  {
    this.mTTL = 0.0f;
    EffectManager.Instance.Stop(ref this.mEffect);
    BreakBarriers.sCache.Add(this);
  }
}
