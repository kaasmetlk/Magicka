using System;
using System.Collections.Generic;
using System.IO;
using Magicka.AI;
using Magicka.AI.AgentStates;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020001C7 RID: 455
	public class NonPlayerCharacter : Character
	{
		// Token: 0x06000F34 RID: 3892 RVA: 0x0005F44C File Offset: 0x0005D64C
		public static NonPlayerCharacter GetInstance(PlayState iPlayState)
		{
			NonPlayerCharacter result;
			lock (NonPlayerCharacter.sCache)
			{
				if (NonPlayerCharacter.sCache.Count > 0)
				{
					NonPlayerCharacter nonPlayerCharacter = NonPlayerCharacter.sCache[0];
					NonPlayerCharacter.sCache.RemoveAt(0);
					result = nonPlayerCharacter;
				}
				else
				{
					result = new NonPlayerCharacter(iPlayState);
				}
			}
			return result;
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0005F4B0 File Offset: 0x0005D6B0
		public static NonPlayerCharacter GetSpecificInstance(ushort iHandle)
		{
			NonPlayerCharacter nonPlayerCharacter;
			lock (NonPlayerCharacter.sCache)
			{
				nonPlayerCharacter = (Entity.GetFromHandle((int)iHandle) as NonPlayerCharacter);
				NonPlayerCharacter.sCache.Remove(nonPlayerCharacter);
			}
			return nonPlayerCharacter;
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0005F4FC File Offset: 0x0005D6FC
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			NonPlayerCharacter.sCache = new List<NonPlayerCharacter>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				NonPlayerCharacter.sCache.Add(new NonPlayerCharacter(iPlayState));
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06000F37 RID: 3895 RVA: 0x0005F530 File Offset: 0x0005D730
		// (set) Token: 0x06000F38 RID: 3896 RVA: 0x0005F538 File Offset: 0x0005D738
		internal Fairy RevivalFairy
		{
			get
			{
				return this.mFairy;
			}
			set
			{
				this.mFairy = value;
			}
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x0005F544 File Offset: 0x0005D744
		protected NonPlayerCharacter(PlayState iPlayState) : base(iPlayState)
		{
			this.mAI = new Agent(this);
			this.mFairy = Fairy.MakeFairy(iPlayState, this);
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x0005F594 File Offset: 0x0005D794
		protected override void ApplyTemplate(CharacterTemplate iTemplate, ref int iModel)
		{
			base.ApplyTemplate(iTemplate, ref iModel);
			this.mAbilities = iTemplate.Abilities;
			for (int i = 0; i < this.mAbilityCooldown.Length; i++)
			{
				this.mAbilityCooldown[i] = 0f;
			}
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x0005F5D8 File Offset: 0x0005D7D8
		public override void Initialize(CharacterTemplate iTemplate, int iRandomOverride, Vector3 iPosition, int iUniqueID)
		{
			base.Initialize(iTemplate, iRandomOverride, iPosition, iUniqueID);
			this.mSpellToCast = default(Spell);
			this.mSpellAreaGrowthRate = 0f;
			this.mSpellPushGrowthRate = 0f;
			this.mSummoned = false;
			this.mUndeadSummon = false;
			this.mBlockTimer = 0f;
			base.SpellQueue.Clear();
			this.mAI.Reset();
			this.mAI.Initialize(this, iTemplate);
			this.mAI.Enable();
			if (base.HasFairy)
			{
				this.RevivalFairy.Initialize(this.mPlayState, false);
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06000F3C RID: 3900 RVA: 0x0005F673 File Offset: 0x0005D873
		// (set) Token: 0x06000F3D RID: 3901 RVA: 0x0005F67B File Offset: 0x0005D87B
		public float SpellPushGrowthRate
		{
			get
			{
				return this.mSpellPushGrowthRate;
			}
			set
			{
				this.mSpellPushGrowthRate = value;
			}
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06000F3E RID: 3902 RVA: 0x0005F684 File Offset: 0x0005D884
		// (set) Token: 0x06000F3F RID: 3903 RVA: 0x0005F68C File Offset: 0x0005D88C
		public float SpellAreaGrowthRate
		{
			get
			{
				return this.mSpellAreaGrowthRate;
			}
			set
			{
				this.mSpellAreaGrowthRate = value;
			}
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x0005F695 File Offset: 0x0005D895
		public void SetSpellToCast(ref Spell iSpell)
		{
			this.mSpellToCast = iSpell;
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06000F41 RID: 3905 RVA: 0x0005F6A3 File Offset: 0x0005D8A3
		// (set) Token: 0x06000F42 RID: 3906 RVA: 0x0005F6AB File Offset: 0x0005D8AB
		public float ChantCooldown
		{
			get
			{
				return this.mChantCooldown;
			}
			set
			{
				this.mChantCooldown = value;
			}
		}

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06000F43 RID: 3907 RVA: 0x0005F6B4 File Offset: 0x0005D8B4
		public virtual Ability[] Abilities
		{
			get
			{
				return this.mAbilities;
			}
		}

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06000F44 RID: 3908 RVA: 0x0005F6BC File Offset: 0x0005D8BC
		public float[] AbilityCooldown
		{
			get
			{
				return this.mAbilityCooldown;
			}
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x0005F6C4 File Offset: 0x0005D8C4
		public override void Charm(Entity iCassanova, float iTTL, int iEffect)
		{
			base.Charm(iCassanova, iTTL, iEffect);
			while (this.AI.CurrentState != AIStateIdle.Instance)
			{
				if (this.AI.CurrentState is AIStateAttack)
				{
					this.AI.ReleaseTarget();
				}
				this.AI.PopState();
			}
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x0005F718 File Offset: 0x0005D918
		public override void Hypnotize(ref Vector3 iDirection, int iEffect)
		{
			base.Hypnotize(ref iDirection, iEffect);
			while (this.AI.CurrentState != AIStateIdle.Instance)
			{
				if (this.AI.CurrentState is AIStateAttack)
				{
					this.AI.ReleaseTarget();
				}
				this.AI.PopState();
			}
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x0005F76C File Offset: 0x0005D96C
		public override void StopHypnotize()
		{
			base.StopHypnotize();
			if (this.mHypnotized)
			{
				while (this.AI.CurrentState != AIStateIdle.Instance)
				{
					if (this.AI.CurrentState is AIStateAttack)
					{
						this.AI.ReleaseTarget();
					}
					this.AI.PopState();
				}
			}
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x0005F7C4 File Offset: 0x0005D9C4
		internal void Confuse(Factions iNewFaction)
		{
			this.mFaction = iNewFaction;
			while (this.AI.CurrentState != AIStateIdle.Instance)
			{
				if (this.AI.CurrentState is AIStateAttack)
				{
					this.AI.ReleaseTarget();
				}
				this.AI.PopState();
			}
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x0005F814 File Offset: 0x0005DA14
		public override void EndCharm()
		{
			base.EndCharm();
			while (this.AI.CurrentState != AIStateIdle.Instance)
			{
				if (this.AI.CurrentState is AIStateAttack)
				{
					this.AI.ReleaseTarget();
				}
				this.AI.PopState();
			}
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06000F4A RID: 3914 RVA: 0x0005F863 File Offset: 0x0005DA63
		public Agent AI
		{
			get
			{
				return this.mAI;
			}
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x0005F86C File Offset: 0x0005DA6C
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mBlockTimer -= iDeltaTime;
			if (this.mBlockTimer <= 0f & this.IsBlocking)
			{
				this.IsBlocking = false;
			}
			if (GlobalSettings.Instance.HealthBars == SettingOptions.On && base.HitPoints > 0f)
			{
				float num = Math.Min(this.mTimeSinceLastDamage, this.mTimeSinceLastStatusDamage);
				Vector4 value = new Vector4(1f, 0f, 0f, 1f);
				if (this.HasStatus(StatusEffects.Poisoned))
				{
					value.X = 0f;
					value.Y = 1f;
				}
				for (int i = 0; i < this.mNumberOfHealtBars; i++)
				{
					Vector2 value2 = new Vector2(0f, (float)i * 6f);
					if (i == 0)
					{
						value = new Vector4(1f, 0f, 0f, 1f);
						if (this.HasStatus(StatusEffects.Poisoned) && this.mNumberOfHealtBars == 1)
						{
							value = new Vector4(0f, 1f, 0f, 1f);
						}
					}
					else if (i == this.mNumberOfHealtBars - 1)
					{
						value = new Vector4(2f, 1.5f, 1f, 1f);
						if (this.HasStatus(StatusEffects.Poisoned))
						{
							value = new Vector4(0f, 1f, 0f, 1f);
						}
					}
					else
					{
						value = new Vector4(0.6f, 0.6f, 0.6f, 1f);
					}
					Vector3 position = this.Position;
					position.Y -= base.Capsule.Length * 0.5f + base.Capsule.Radius;
					if (i == this.mNumberOfHealtBars - 1)
					{
						Healthbars.Instance.AddHealthBar(position, base.NormalizedHitPoints, this.mRadius, 1f, num, true, new Vector4?(value), new Vector2?(value2));
					}
					else
					{
						Healthbars.Instance.AddHealthBar(position, 1f, this.mRadius, 1f, Math.Max(num, 0.3f), true, new Vector4?(value), new Vector2?(value2));
					}
				}
			}
			this.mBreakFreeTimer -= iDeltaTime;
			Vector3 position2 = this.Position;
			Vector3 direction = this.Direction;
			if (this.mSummoned)
			{
				EffectManager.Instance.UpdatePositionDirection(ref this.mSummonedEffect, ref position2, ref direction);
			}
			for (int j = 0; j < 16; j++)
			{
				this.mAbilityCooldown[j] -= iDeltaTime;
			}
			this.mChantCooldown -= iDeltaTime;
			this.mSpellCooldown -= iDeltaTime;
			base.Update(iDataChannel, iDeltaTime);
			this.mStaffOrb = this.GetLeftAttachOrientation();
			this.mWeaponTransform = this.GetRightAttachOrientation();
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x0005FB40 File Offset: 0x0005DD40
		public override DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (iAttacker is Character && this.AI != null)
			{
				float iAmount = iDamage.Amount * iDamage.Magnitude;
				this.AI.AddAttackedBy((Character)iAttacker, iAmount);
			}
			return base.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x0005FB8C File Offset: 0x0005DD8C
		public override void Terminate(bool iKillItems, bool iIsKillPlane, bool iNetwork)
		{
			base.Terminate(iKillItems, iIsKillPlane, iNetwork);
			this.Die();
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x0005FB9D File Offset: 0x0005DD9D
		public override void Drown()
		{
			base.Drown();
			this.Die();
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x0005FBAB File Offset: 0x0005DDAB
		public override void OverKill()
		{
			base.OverKill();
			this.Die();
		}

		// Token: 0x06000F50 RID: 3920 RVA: 0x0005FBBC File Offset: 0x0005DDBC
		public override void Die()
		{
			base.Die();
			if (this.mAI != null & !this.mUndying)
			{
				this.mAI.Disable();
			}
			EffectManager.Instance.Stop(ref this.mSummonedEffect);
			lock (NonPlayerCharacter.sCache)
			{
				if (!NonPlayerCharacter.sCache.Contains(this))
				{
					NonPlayerCharacter.sCache.Add(this);
				}
			}
			if (this.Master != null)
			{
				this.Master.RemoveBubbleShield();
			}
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x0005FC54 File Offset: 0x0005DE54
		public void Summoned(Character iMaster)
		{
			this.Summoned(iMaster, false, false);
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x0005FC5F File Offset: 0x0005DE5F
		public void Summoned(Character iMaster, bool undead)
		{
			this.Summoned(iMaster, undead, false);
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x0005FC6C File Offset: 0x0005DE6C
		public void Summoned(Character iMaster, bool undead, bool flamer)
		{
			this.mSummoned = true;
			this.mFlamerSummon = flamer;
			this.mUndeadSummon = undead;
			if (this.mUndeadSummon)
			{
				this.mFlamerSummon = false;
			}
			this.mSummonMaster = iMaster;
			this.mFaction = iMaster.Faction;
			this.mSummonMaster.OnSpawnedSummon(this);
		}

		// Token: 0x06000F54 RID: 3924 RVA: 0x0005FCBC File Offset: 0x0005DEBC
		public void Block()
		{
			this.mBlockTimer = 1f;
			this.IsBlocking = true;
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06000F55 RID: 3925 RVA: 0x0005FCD0 File Offset: 0x0005DED0
		// (set) Token: 0x06000F56 RID: 3926 RVA: 0x0005FCD8 File Offset: 0x0005DED8
		public bool IsSummoned
		{
			get
			{
				return this.mSummoned;
			}
			set
			{
				this.mSummoned = value;
			}
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06000F57 RID: 3927 RVA: 0x0005FCE1 File Offset: 0x0005DEE1
		public bool IsUndeadSummon
		{
			get
			{
				return this.mUndeadSummon;
			}
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06000F58 RID: 3928 RVA: 0x0005FCE9 File Offset: 0x0005DEE9
		// (set) Token: 0x06000F59 RID: 3929 RVA: 0x0005FCF1 File Offset: 0x0005DEF1
		public bool IsFlamerSummon
		{
			get
			{
				return this.mFlamerSummon;
			}
			set
			{
				if (this.mUndeadSummon && value)
				{
					this.mFlamerSummon = false;
					return;
				}
				this.mFlamerSummon = value;
			}
		}

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06000F5A RID: 3930 RVA: 0x0005FD0D File Offset: 0x0005DF0D
		public Character Master
		{
			get
			{
				return this.mSummonMaster;
			}
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06000F5B RID: 3931 RVA: 0x0005FD15 File Offset: 0x0005DF15
		public override Factions Faction
		{
			get
			{
				if (base.IsCharmed)
				{
					return Factions.FRIENDLY;
				}
				return base.Faction;
			}
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0005FD27 File Offset: 0x0005DF27
		public override void BreakFree()
		{
			if (this.mBreakFreeTimer <= 0f)
			{
				base.BreakFree();
				this.mBreakFreeTimer = 0.5f;
			}
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x0005FD47 File Offset: 0x0005DF47
		public void StopAttacking()
		{
			this.mAttacking = false;
			this.mGripAttack = false;
			this.mDashing = false;
			this.mNextAttackAnimation = Animations.None;
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x0005FD68 File Offset: 0x0005DF68
		public override void Deinitialize()
		{
			base.Deinitialize();
			EffectManager.Instance.Stop(ref this.mSummonedEffect);
			for (int i = 0; i < base.Equipment.Length; i++)
			{
				base.Equipment[i].Item.Deinitialize();
			}
			this.mAI.Disable();
			lock (NonPlayerCharacter.sCache)
			{
				if (!NonPlayerCharacter.sCache.Contains(this))
				{
					NonPlayerCharacter.sCache.Add(this);
				}
			}
			if (this.mSummonMaster != null)
			{
				this.mSummonMaster.OnDespawnedSummon(this);
			}
			this.mSummonMaster = null;
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06000F5F RID: 3935 RVA: 0x0005FE14 File Offset: 0x0005E014
		// (set) Token: 0x06000F60 RID: 3936 RVA: 0x0005FE17 File Offset: 0x0005E017
		public override int Boosts
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06000F61 RID: 3937 RVA: 0x0005FE19 File Offset: 0x0005E019
		public bool CanCastSpellNow
		{
			get
			{
				return this.mSpellCooldown < 0f;
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06000F62 RID: 3938 RVA: 0x0005FE28 File Offset: 0x0005E028
		public override bool IsInAEvent
		{
			get
			{
				return this.mAI.Events != null && this.mAI.CurrentEvent < this.mAI.Events.Length;
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06000F63 RID: 3939 RVA: 0x0005FE54 File Offset: 0x0005E054
		public override float BoostCooldown
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06000F64 RID: 3940 RVA: 0x0005FE5B File Offset: 0x0005E05B
		public override bool IsAggressive
		{
			get
			{
				return this.mAI != null && this.mAI.CurrentTarget != null;
			}
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x0005FE78 File Offset: 0x0005E078
		public override void CastSpell(bool iFromStaff, string iJoint)
		{
			if (this.CastType == CastType.Weapon)
			{
				this.mSpell = SpellManager.Instance.Combine(this.mSpellQueue);
				base.SpellQueue.Clear();
				this.mSpell.Cast(iFromStaff, this, this.CastType);
				this.mSpell = default(Spell);
				return;
			}
			base.CastSpell(iFromStaff, iJoint);
		}

		// Token: 0x17000400 RID: 1024
		// (set) Token: 0x06000F66 RID: 3942 RVA: 0x0005FED8 File Offset: 0x0005E0D8
		public override CastType CastType
		{
			set
			{
				if (value != CastType.Weapon & value != CastType.None & value != this.mCastType)
				{
					if (value == CastType.Magick)
					{
						SpellManager.Instance.CombineMagick(null, this.mPlayState.GameType, this.mSpellQueue);
					}
					this.CombineSpell();
				}
				this.mCastType = value;
			}
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x0005FF34 File Offset: 0x0005E134
		internal void ConjureSpell(Elements iElements)
		{
			Spell spell;
			Spell.DefaultSpell(iElements, out spell);
			if (this.mSummonElementCue != 0)
			{
				AudioManager.Instance.PlayCue(this.mSummonElementBank, this.mSummonElementCue, base.AudioEmitter);
			}
			if (SpellManager.Instance.TryAddToQueue(null, this, this.mSpellQueue, this.mSpellQueue.Capacity, ref spell) && NetworkManager.Instance.State == NetworkState.Server)
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Handle = base.Handle;
				characterActionMessage.Action = ActionType.ConjureElement;
				characterActionMessage.Param0I = (int)iElements;
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x0005FFD4 File Offset: 0x0005E1D4
		internal void CopyPolymorphValuesFrom(NonPlayerCharacter iSource)
		{
			this.mType = iSource.Type;
			this.mFaction = iSource.mFaction;
			this.mHitPoints = iSource.mHitPoints / iSource.mMaxHitPoints * this.mMaxHitPoints;
			this.mTimeSinceLastDamage = iSource.mTimeSinceLastDamage;
			this.mTimeSinceLastStatusDamage = iSource.mTimeSinceLastStatusDamage;
			base.CharacterBody.DesiredDirection = iSource.CharacterBody.DesiredDirection;
			if (iSource.IsEthereal)
			{
				base.Ethereal(true, 1f, 1f);
			}
			if (iSource.IsFeared)
			{
				if (iSource.mFearedBy != null)
				{
					base.Fear(iSource.mFearedBy);
				}
				else
				{
					base.Fear(iSource.mFearPosition);
				}
				this.mFearTimer = iSource.mFearTimer;
			}
			StatusEffect[] statusEffects = iSource.GetStatusEffects();
			for (int i = 0; i < statusEffects.Length; i++)
			{
				base.AddStatusEffect(statusEffects[i]);
			}
			if (iSource.IsSummoned)
			{
				this.mSummoned = true;
				this.mSummonMaster = iSource.mSummonMaster;
				this.mSummonMaster.OnDespawnedSummon(iSource);
				this.mSummonMaster.OnSpawnedSummon(this);
			}
			if (iSource.IsCharmed)
			{
				this.Charm(iSource.mCharmOwner, iSource.mCharmTimer, iSource.mCharmEffectID);
				this.mCharmTimer = iSource.mCharmTimer;
			}
			if (iSource.IsHypnotized)
			{
				this.Hypnotize(ref iSource.mHypnotizeDirection, iSource.mHypnotizeEffectID);
			}
		}

		// Token: 0x04000DEC RID: 3564
		public const int MAX_ABILITY_COUNT = 16;

		// Token: 0x04000DED RID: 3565
		private static List<NonPlayerCharacter> sCache;

		// Token: 0x04000DEE RID: 3566
		public static Factions CharmFaction = Factions.FRIENDLY;

		// Token: 0x04000DEF RID: 3567
		protected Agent mAI;

		// Token: 0x04000DF0 RID: 3568
		private Spell mSpellToCast;

		// Token: 0x04000DF1 RID: 3569
		protected Ability[] mAbilities;

		// Token: 0x04000DF2 RID: 3570
		protected float[] mAbilityCooldown = new float[16];

		// Token: 0x04000DF3 RID: 3571
		private bool mSummoned;

		// Token: 0x04000DF4 RID: 3572
		private bool mUndeadSummon;

		// Token: 0x04000DF5 RID: 3573
		private bool mFlamerSummon;

		// Token: 0x04000DF6 RID: 3574
		private Character mSummonMaster;

		// Token: 0x04000DF7 RID: 3575
		private VisualEffectReference mSummonedEffect;

		// Token: 0x04000DF8 RID: 3576
		private float mSpellCooldown = 4f;

		// Token: 0x04000DF9 RID: 3577
		private float mChantCooldown = 1f;

		// Token: 0x04000DFA RID: 3578
		private float mSpellPushGrowthRate;

		// Token: 0x04000DFB RID: 3579
		private float mSpellAreaGrowthRate;

		// Token: 0x04000DFC RID: 3580
		private float mBlockTimer;

		// Token: 0x04000DFD RID: 3581
		private float mBreakFreeTimer;

		// Token: 0x04000DFE RID: 3582
		private Fairy mFairy;

		// Token: 0x020001C8 RID: 456
		public struct State
		{
			// Token: 0x06000F6A RID: 3946 RVA: 0x0006013C File Offset: 0x0005E33C
			public State(BinaryReader iReader)
			{
				this.Name = iReader.ReadString();
				this.Model = iReader.ReadInt32();
				this.UniqueID = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				this.HitPoints = iReader.ReadSingle();
				this.IsEthereal = iReader.ReadBoolean();
				this.Dialog = iReader.ReadInt32();
				this.OnDamageTrigger = iReader.ReadInt32();
				this.OnDeathTrigger = iReader.ReadInt32();
				this.Order = (Order)iReader.ReadByte();
				this.ReactTo = (ReactTo)iReader.ReadByte();
				this.Reaction = (Order)iReader.ReadByte();
				this.PriorityTarget = iReader.ReadInt32();
				this.PriorityAbility = iReader.ReadInt32();
				this.ReactionTrigger = iReader.ReadInt32();
				this.Events = null;
				int num = iReader.ReadInt32();
				if (num > 0)
				{
					this.Events = new AIEvent[num];
					for (int i = 0; i < num; i++)
					{
						this.Events[i] = new AIEvent(iReader);
					}
				}
				this.CurrentEvent = iReader.ReadInt32();
				this.CurrentEventDelay = iReader.ReadSingle();
			}

			// Token: 0x06000F6B RID: 3947 RVA: 0x00060280 File Offset: 0x0005E480
			public State(NonPlayerCharacter iNPC)
			{
				this.Name = iNPC.Name;
				this.Model = iNPC.ModelIndex;
				this.UniqueID = iNPC.UniqueID;
				this.Position = iNPC.Position;
				this.HitPoints = iNPC.HitPoints;
				this.IsEthereal = iNPC.IsEthereal;
				this.Dialog = iNPC.Dialog;
				this.OnDamageTrigger = iNPC.OnDamageTrigger;
				this.OnDeathTrigger = iNPC.OnDeathTrigger;
				this.Order = iNPC.AI.Order;
				this.ReactTo = iNPC.AI.ReactsTo;
				this.Reaction = iNPC.AI.ReactionOrder;
				this.PriorityTarget = iNPC.AI.PriorityTargetID;
				this.PriorityAbility = iNPC.AI.PriorityAbility;
				this.ReactionTrigger = iNPC.AI.ReactionTrigger;
				if (iNPC.AI.Events == null)
				{
					this.Events = null;
				}
				else
				{
					this.Events = (iNPC.AI.Events.Clone() as AIEvent[]);
				}
				this.CurrentEvent = iNPC.AI.CurrentEvent;
				this.CurrentEventDelay = iNPC.AI.CurrentEventDelay;
			}

			// Token: 0x06000F6C RID: 3948 RVA: 0x000603B4 File Offset: 0x0005E5B4
			public void ApplyTo(NonPlayerCharacter iNPC)
			{
				CharacterTemplate iTemplate = iNPC.PlayState.Content.Load<CharacterTemplate>("data/characters/" + this.Name);
				iNPC.Initialize(iTemplate, this.Model, this.Position, this.UniqueID);
				iNPC.HitPoints = this.HitPoints;
				iNPC.IsEthereal = this.IsEthereal;
				iNPC.Dialog = this.Dialog;
				iNPC.OnDamageTrigger = this.OnDamageTrigger;
				iNPC.OnDeathTrigger = this.OnDeathTrigger;
				iNPC.AI.SetOrder(this.Order, this.ReactTo, this.Reaction, this.PriorityTarget, this.PriorityAbility, this.ReactionTrigger, this.Events);
				iNPC.AI.CurrentEvent = this.CurrentEvent;
				iNPC.AI.CurrentEventDelay = this.CurrentEventDelay;
			}

			// Token: 0x06000F6D RID: 3949 RVA: 0x00060490 File Offset: 0x0005E690
			public void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.Name);
				iWriter.Write(this.Model);
				iWriter.Write(this.UniqueID);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(this.HitPoints);
				iWriter.Write(this.IsEthereal);
				iWriter.Write(this.Dialog);
				iWriter.Write(this.OnDamageTrigger);
				iWriter.Write(this.OnDeathTrigger);
				iWriter.Write((byte)this.Order);
				iWriter.Write((byte)this.ReactTo);
				iWriter.Write((byte)this.Reaction);
				iWriter.Write(this.PriorityTarget);
				iWriter.Write(this.PriorityAbility);
				iWriter.Write(this.ReactionTrigger);
				if (this.Events == null)
				{
					iWriter.Write(0);
				}
				else
				{
					iWriter.Write(this.Events.Length);
					for (int i = 0; i < this.Events.Length; i++)
					{
						this.Events[i].Write(iWriter);
					}
				}
				iWriter.Write(this.CurrentEvent);
				iWriter.Write(this.CurrentEventDelay);
			}

			// Token: 0x04000DFF RID: 3583
			public string Name;

			// Token: 0x04000E00 RID: 3584
			public int Model;

			// Token: 0x04000E01 RID: 3585
			public int UniqueID;

			// Token: 0x04000E02 RID: 3586
			public Vector3 Position;

			// Token: 0x04000E03 RID: 3587
			public float HitPoints;

			// Token: 0x04000E04 RID: 3588
			public bool IsEthereal;

			// Token: 0x04000E05 RID: 3589
			public int Dialog;

			// Token: 0x04000E06 RID: 3590
			public int OnDamageTrigger;

			// Token: 0x04000E07 RID: 3591
			public int OnDeathTrigger;

			// Token: 0x04000E08 RID: 3592
			public Order Order;

			// Token: 0x04000E09 RID: 3593
			public ReactTo ReactTo;

			// Token: 0x04000E0A RID: 3594
			public Order Reaction;

			// Token: 0x04000E0B RID: 3595
			public int PriorityTarget;

			// Token: 0x04000E0C RID: 3596
			public int PriorityAbility;

			// Token: 0x04000E0D RID: 3597
			public int ReactionTrigger;

			// Token: 0x04000E0E RID: 3598
			public AIEvent[] Events;

			// Token: 0x04000E0F RID: 3599
			public int CurrentEvent;

			// Token: 0x04000E10 RID: 3600
			public float CurrentEventDelay;
		}
	}
}
