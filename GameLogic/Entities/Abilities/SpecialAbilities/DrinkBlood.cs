// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.DrinkBlood
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class DrinkBlood(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_drain")), IAbilityEffect
{
  private const float RANGE = 8f;
  private const float CHARGE_TIME = 0.78f;
  private const float DRINK_TIME = 0.83f;
  private ISpellCaster mOwner;
  private PlayState mPlayState;
  private float mTTL;
  private float LifeStealAmount;
  private bool mFoundTarget;

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mOwner = iOwner;
    this.mTTL = 0.78f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    this.mPlayState = iPlayState;
    this.mFoundTarget = false;
    Haste instance = Haste.GetInstance();
    instance.CustomTTL = 0.78f;
    return instance.Execute(iOwner, iPlayState, false);
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    throw new NotImplementedException();
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if (this.mFoundTarget)
      return;
    if ((double) this.mOwner.HitPoints <= 0.0)
    {
      this.mTTL = 0.0f;
    }
    else
    {
      List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, 8f, false);
      Entity entity1 = (Entity) null;
      float num1 = float.MaxValue;
      Vector3 position1 = this.mOwner.Position;
      foreach (Entity entity2 in entities)
      {
        if (entity2 is Magicka.GameLogic.Entities.Character && entity2 != this.mOwner)
        {
          Vector3 position2 = entity2.Position;
          float result;
          Vector3.Distance(ref position2, ref position1, out result);
          if ((double) result < (double) num1)
          {
            entity1 = entity2;
            num1 = result;
          }
        }
      }
      if (entity1 is Magicka.GameLogic.Entities.Character character && (double) num1 < (double) this.mOwner.Radius + (double) character.Radius + 1.0)
      {
        Vector3 vector = this.mOwner.Position - character.Position;
        vector.Normalize();
        Vector3 direction = this.mOwner.Direction;
        float num2 = MagickaMath.Angle(ref vector, ref direction);
        if ((double) num2 < 3.1415927410125732 && (double) num2 > 2.3561944961547852)
        {
          this.LifeStealAmount = this.mOwner.MaxHitPoints - this.mOwner.HitPoints;
          if ((double) this.LifeStealAmount > (double) character.HitPoints)
            this.LifeStealAmount = character.HitPoints;
          if ((double) this.LifeStealAmount > 0.0)
          {
            this.mFoundTarget = true;
            (this.mOwner as Magicka.GameLogic.Entities.Character).GoToAnimation(Magicka.Animations.special4, 0.1f);
            character.Damage(this.LifeStealAmount, Elements.Arcane);
            this.mTTL = 0.83f;
            if ((double) this.LifeStealAmount < 0.0)
              this.LifeStealAmount = 0.0f;
            this.mOwner.Damage(-this.LifeStealAmount, Elements.Life);
            this.LifeStealAmount = 0.0f;
          }
          else
          {
            (this.mOwner as Magicka.GameLogic.Entities.Character).GoToAnimation(Magicka.Animations.idle, 0.05f);
            this.mTTL = 0.0f;
          }
          character.Stun(1.5f);
        }
      }
      this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
    }
  }

  public void OnRemove()
  {
  }
}
