// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.DrainLife
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

public class DrainLife(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_drain")), IAbilityEffect
{
  private const float TTL = 1f;
  private const float RANGE = 8f;
  private const float HIT_TIME = 0.25f;
  public static readonly int EFFECT = "drainlife".GetHashCodeCustom();
  private ISpellCaster mOwner;
  private PlayState mPlayState;
  private VisualEffectReference mEffect;
  private float mHitTimer;
  private float mTTL;
  private float LifeStealAmount;
  private Vector3 effectPos;
  private Vector3 targetInitialPos;

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mOwner = iOwner;
    List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, 8f, false, true);
    Entity entity1 = (Entity) null;
    float num1 = float.MaxValue;
    Vector3 position1 = this.mOwner.Position;
    foreach (Entity entity2 in entities)
    {
      if (entity2 is Magicka.GameLogic.Entities.Character && entity2.Position != this.mOwner.Position)
      {
        Vector3 position2 = entity2.Position;
        float result;
        Vector3.DistanceSquared(ref position2, ref position1, out result);
        float num2 = 8f + entity2.Radius;
        float num3 = num2 * num2;
        if ((double) result <= (double) num3 && (double) result < (double) num1)
        {
          entity1 = entity2;
          num1 = result;
        }
      }
    }
    if (!(entity1 is Magicka.GameLogic.Entities.Character character))
      return false;
    this.LifeStealAmount = this.mOwner.MaxHitPoints - this.mOwner.HitPoints;
    if ((double) this.LifeStealAmount > (double) character.HitPoints)
      this.LifeStealAmount = character.HitPoints;
    character.Damage(this.LifeStealAmount, Elements.Arcane);
    this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
    this.mPlayState = iPlayState;
    if (this.IsDead)
    {
      this.targetInitialPos = character.Position;
      Vector3 iDirection = this.mOwner.Position - character.Position;
      iDirection.Normalize();
      EffectManager.Instance.StartEffect(DrainLife.EFFECT, ref this.targetInitialPos, ref iDirection, out this.mEffect);
      SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    }
    this.mTTL = 1f;
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new NotImplementedException();
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    this.mHitTimer -= iDeltaTime;
    Vector3 iDirection = this.mOwner.Position - this.targetInitialPos;
    iDirection.Normalize();
    this.effectPos = Vector3.Lerp(this.targetInitialPos, this.mOwner.Position, 1f - this.mTTL);
    EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref this.effectPos, ref iDirection);
    if ((double) this.mHitTimer > 0.0)
      return;
    this.mHitTimer = 0.25f;
  }

  public void OnRemove()
  {
    this.mOwner.Damage(-this.LifeStealAmount, Elements.Life);
    EffectManager.Instance.Stop(ref this.mEffect);
  }
}
