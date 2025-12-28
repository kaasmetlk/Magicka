using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.AI.Arithmetics;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000356 RID: 854
	internal class WarlordCharacter : NonPlayerCharacter
	{
		// Token: 0x060019EF RID: 6639 RVA: 0x000AD552 File Offset: 0x000AB752
		public WarlordCharacter(PlayState iPlayState) : base(iPlayState)
		{
			this.mAbsorbedDamage = default(DamageCollection5);
			this.mEquipment[0].Item = new WarlordCharacter.AbsorbShield(iPlayState, this);
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x000AD57B File Offset: 0x000AB77B
		protected override void ApplyTemplate(CharacterTemplate iTemplate, ref int iModel)
		{
			base.ApplyTemplate(iTemplate, ref iModel);
			this.Abilities[0] = new WarlordCharacter.Bash(this.Abilities[0] as Melee);
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x000AD59F File Offset: 0x000AB79F
		public override void OverKill()
		{
			this.mDead = false;
			this.mOverkilled = true;
			this.mCannotDieWithoutExplicitKill = true;
			this.mAnimationController.Stop();
			this.mHitPoints = -1f;
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x000AD5CC File Offset: 0x000AB7CC
		public override DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (base.BlockItem < 0 || iDamage.Element == Elements.None)
			{
				return base.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			}
			if ((iDamage.Element & Elements.PhysicalElements) == Elements.None)
			{
				Spell spell = default(Spell);
				for (int i = 0; i < 11; i++)
				{
					Elements elements = (Elements)(1 << i);
					if ((elements & iDamage.Element) != Elements.None)
					{
						ref Spell ptr = ref spell;
						Elements iElement;
						spell[iElement = elements] = ptr[iElement] + iDamage.Magnitude;
						spell.Element |= elements;
					}
				}
				base.Equipment[0].Item.TryAddToQueue(ref spell, false);
				return DamageResult.Deflected;
			}
			float num;
			base.BlockDamage(iDamage, iAttackPosition, out num);
			iDamage.Amount -= num;
			return DamageResult.Deflected;
		}

		// Token: 0x04001C27 RID: 7207
		public DamageCollection5 mAbsorbedDamage;

		// Token: 0x02000358 RID: 856
		private class Bash : Melee
		{
			// Token: 0x060019FF RID: 6655 RVA: 0x000AD87D File Offset: 0x000ABA7D
			public Bash(Melee iCloneSource) : base(iCloneSource)
			{
			}

			// Token: 0x06001A00 RID: 6656 RVA: 0x000AD886 File Offset: 0x000ABA86
			public override float GetDesirability(ref ExpressionArguments iArgs)
			{
				return base.GetDesirability(ref iArgs) + (float)iArgs.AI.Owner.Equipment[0].Item.SpellList.Count * 0.2f;
			}
		}

		// Token: 0x02000360 RID: 864
		private class AbsorbShield : Item
		{
			// Token: 0x06001A5F RID: 6751 RVA: 0x000B35B7 File Offset: 0x000B17B7
			public AbsorbShield(PlayState iPlayState, Character iOwner) : base(iPlayState, iOwner)
			{
			}

			// Token: 0x06001A60 RID: 6752 RVA: 0x000B35C4 File Offset: 0x000B17C4
			public override void Execute(DealDamage.Targets iTargets)
			{
				if (this.mHitlist.Count == 0)
				{
					this.mContinueHitting = true;
				}
				if (!this.mMeleeMultiHit && !this.mContinueHitting)
				{
					return;
				}
				WarlordCharacter warlordCharacter = base.Owner as WarlordCharacter;
				Segment iSeg = default(Segment);
				iSeg.Origin = this.mAttach0AbsoluteTransform.Translation;
				Vector3.Subtract(ref this.mLastAttachAbsolutePosition, ref iSeg.Origin, out iSeg.Delta);
				List<Shield> shields = warlordCharacter.mPlayState.EntityManager.Shields;
				Segment iSeg2 = default(Segment);
				iSeg2.Origin = warlordCharacter.Position;
				Vector3.Subtract(ref iSeg2.Origin, ref iSeg.Origin, out iSeg2.Delta);
				for (int i = 0; i < shields.Count; i++)
				{
					Shield shield = shields[i];
					Vector3 vector;
					if (shield != null && (shield.SegmentIntersect(out vector, iSeg, this.mMeleeRange) || shield.SegmentIntersect(out vector, iSeg2, this.mMeleeRange)))
					{
						EventCondition eventCondition = default(EventCondition);
						eventCondition.EventConditionType = EventConditionType.Hit;
						this.mMeleeConditions.ExecuteAll(base.Owner, shield, ref eventCondition);
						warlordCharacter.GoToAnimation(Animations.attack_recoil, 0.03f);
						return;
					}
				}
				List<Entity> entities = warlordCharacter.mPlayState.EntityManager.GetEntities(this.mAttach0AbsoluteTransform.Translation, this.mMeleeRange, true);
				for (int j = 0; j < entities.Count; j++)
				{
					IDamageable damageable = entities[j] as IDamageable;
					Vector3 vector2;
					if (damageable != warlordCharacter && damageable != null && !this.mHitlist.Contains(damageable) && damageable.SegmentIntersect(out vector2, iSeg, this.mMeleeRange))
					{
						this.mHitlist.Add(damageable);
						if (!this.mMeleeMultiHit)
						{
							break;
						}
					}
				}
				warlordCharacter.mPlayState.EntityManager.ReturnEntityList(entities);
				DamageResult damageResult = DamageResult.None;
				for (int k = this.mNextToDamage; k < this.mHitlist.Count; k++)
				{
					IDamageable damageable2 = this.mHitlist[k];
					EventCondition eventCondition2 = default(EventCondition);
					eventCondition2.EventConditionType = EventConditionType.Hit;
					this.mMeleeConditions.ExecuteAll(this, (Entity)damageable2, ref eventCondition2, out damageResult);
					this.mContinueHitting = (this.mContinueHitting && (damageResult & (DamageResult.Knockeddown | DamageResult.Knockedback | DamageResult.Pushed | DamageResult.Killed)) != DamageResult.None);
					if (!this.mContinueHitting && (damageResult & DamageResult.Deflected) == DamageResult.Deflected)
					{
						warlordCharacter.GoToAnimation(Animations.attack_recoil, 0.03f);
					}
					this.mNextToDamage = k + 1;
					warlordCharacter.mAbsorbedDamage = default(DamageCollection5);
					if (!this.mMeleeMultiHit)
					{
						break;
					}
				}
				if (this.mHitlist.Count > 0)
				{
					Spell spell = warlordCharacter.Equipment[0].Item.PeekSpell();
					if (spell.Element != Elements.None)
					{
						warlordCharacter.Equipment[0].Item.SpellList.Clear();
						spell.Cast(false, warlordCharacter, CastType.Area);
					}
				}
			}
		}
	}
}
