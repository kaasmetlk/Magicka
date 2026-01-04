// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.WarlordCharacter
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI.Arithmetics;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

internal class WarlordCharacter : NonPlayerCharacter
{
  public DamageCollection5 mAbsorbedDamage;

  public WarlordCharacter(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mAbsorbedDamage = new DamageCollection5();
    this.mEquipment[0].Item = (Item) new WarlordCharacter.AbsorbShield(iPlayState, (Character) this);
  }

  protected override void ApplyTemplate(CharacterTemplate iTemplate, ref int iModel)
  {
    base.ApplyTemplate(iTemplate, ref iModel);
    this.Abilities[0] = (Ability) new WarlordCharacter.Bash(this.Abilities[0] as Melee);
  }

  public override void OverKill()
  {
    this.mDead = false;
    this.mOverkilled = true;
    this.mCannotDieWithoutExplicitKill = true;
    this.mAnimationController.Stop();
    this.mHitPoints = -1f;
  }

  public override DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    if (this.BlockItem < 0 || iDamage.Element == Elements.None)
      return base.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    if ((iDamage.Element & Elements.PhysicalElements) == Elements.None)
    {
      Spell iSpell = new Spell();
      for (int index = 0; index < 11; ++index)
      {
        Elements iElement = (Elements) (1 << index);
        if ((iElement & iDamage.Element) != Elements.None)
        {
          iSpell[iElement] += iDamage.Magnitude;
          iSpell.Element |= iElement;
        }
      }
      this.Equipment[0].Item.TryAddToQueue(ref iSpell, false);
      return DamageResult.Deflected;
    }
    float oBlocked;
    this.BlockDamage(iDamage, iAttackPosition, out oBlocked);
    iDamage.Amount -= oBlocked;
    return DamageResult.Deflected;
  }

  private class Bash(Melee iCloneSource) : Melee(iCloneSource)
  {
    public override float GetDesirability(ref ExpressionArguments iArgs)
    {
      return base.GetDesirability(ref iArgs) + (float) iArgs.AI.Owner.Equipment[0].Item.SpellList.Count * 0.2f;
    }
  }

  private class AbsorbShield(PlayState iPlayState, Character iOwner) : Item(iPlayState, iOwner)
  {
    public override void Execute(DealDamage.Targets iTargets)
    {
      if (this.mHitlist.Count == 0)
        this.mContinueHitting = true;
      if (!this.mMeleeMultiHit && !this.mContinueHitting)
        return;
      WarlordCharacter owner = this.Owner as WarlordCharacter;
      Segment iSeg1 = new Segment();
      iSeg1.Origin = this.mAttach0AbsoluteTransform.Translation;
      Vector3.Subtract(ref this.mLastAttachAbsolutePosition, ref iSeg1.Origin, out iSeg1.Delta);
      List<Shield> shields = owner.mPlayState.EntityManager.Shields;
      Segment iSeg2 = new Segment();
      iSeg2.Origin = owner.Position;
      Vector3.Subtract(ref iSeg2.Origin, ref iSeg1.Origin, out iSeg2.Delta);
      for (int index = 0; index < shields.Count; ++index)
      {
        Shield iTarget = shields[index];
        Vector3 oPosition;
        if (iTarget != null && (iTarget.SegmentIntersect(out oPosition, iSeg1, this.mMeleeRange) || iTarget.SegmentIntersect(out oPosition, iSeg2, this.mMeleeRange)))
        {
          this.mMeleeConditions.ExecuteAll((Entity) this.Owner, (Entity) iTarget, ref new EventCondition()
          {
            EventConditionType = EventConditionType.Hit
          });
          owner.GoToAnimation(Magicka.Animations.attack_recoil, 0.03f);
          return;
        }
      }
      List<Entity> entities = owner.mPlayState.EntityManager.GetEntities(this.mAttach0AbsoluteTransform.Translation, this.mMeleeRange, true);
      for (int index = 0; index < entities.Count; ++index)
      {
        IDamageable damageable = entities[index] as IDamageable;
        if (damageable != owner && damageable != null && !this.mHitlist.Contains(damageable) && damageable.SegmentIntersect(out Vector3 _, iSeg1, this.mMeleeRange))
        {
          this.mHitlist.Add(damageable);
          if (!this.mMeleeMultiHit)
            break;
        }
      }
      owner.mPlayState.EntityManager.ReturnEntityList(entities);
      DamageResult oDamageResult = DamageResult.None;
      for (int mNextToDamage = this.mNextToDamage; mNextToDamage < this.mHitlist.Count; ++mNextToDamage)
      {
        this.mMeleeConditions.ExecuteAll((Entity) this, (Entity) this.mHitlist[mNextToDamage], ref new EventCondition()
        {
          EventConditionType = EventConditionType.Hit
        }, out oDamageResult);
        this.mContinueHitting = this.mContinueHitting && (oDamageResult & (DamageResult.Knockeddown | DamageResult.Knockedback | DamageResult.Pushed | DamageResult.Killed)) != DamageResult.None;
        if (!this.mContinueHitting && (oDamageResult & DamageResult.Deflected) == DamageResult.Deflected)
          owner.GoToAnimation(Magicka.Animations.attack_recoil, 0.03f);
        this.mNextToDamage = mNextToDamage + 1;
        owner.mAbsorbedDamage = new DamageCollection5();
        if (!this.mMeleeMultiHit)
          break;
      }
      if (this.mHitlist.Count <= 0)
        return;
      Spell spell = owner.Equipment[0].Item.PeekSpell();
      if (spell.Element == Elements.None)
        return;
      owner.Equipment[0].Item.SpellList.Clear();
      spell.Cast(false, (ISpellCaster) owner, CastType.Area);
    }
  }
}
