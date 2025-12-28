using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000406 RID: 1030
	internal class PerformanceEnchantment : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06001FB4 RID: 8116 RVA: 0x000DE810 File Offset: 0x000DCA10
		public static PerformanceEnchantment GetInstance()
		{
			if (PerformanceEnchantment.sCache.Count > 0)
			{
				PerformanceEnchantment performanceEnchantment = PerformanceEnchantment.sCache[PerformanceEnchantment.sCache.Count - 1];
				PerformanceEnchantment.sCache.RemoveAt(PerformanceEnchantment.sCache.Count - 1);
				PerformanceEnchantment.sActiveEnchantments.Add(performanceEnchantment);
				return performanceEnchantment;
			}
			PerformanceEnchantment performanceEnchantment2 = new PerformanceEnchantment();
			PerformanceEnchantment.sActiveEnchantments.Add(performanceEnchantment2);
			return performanceEnchantment2;
		}

		// Token: 0x06001FB5 RID: 8117 RVA: 0x000DE878 File Offset: 0x000DCA78
		public static void InitializeCache(int iNr)
		{
			PerformanceEnchantment.sCache = new List<PerformanceEnchantment>(iNr);
			PerformanceEnchantment.sActiveEnchantments = new List<PerformanceEnchantment>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				PerformanceEnchantment.sCache.Add(new PerformanceEnchantment());
			}
		}

		// Token: 0x06001FB6 RID: 8118 RVA: 0x000DE8B6 File Offset: 0x000DCAB6
		public PerformanceEnchantment() : base(Animations.cast_magick_self, PerformanceEnchantment.DISPLAY_NAME)
		{
			this.mTTL = 10f;
		}

		// Token: 0x06001FB7 RID: 8119 RVA: 0x000DE8D0 File Offset: 0x000DCAD0
		public PerformanceEnchantment(Animations iAnimation) : base(iAnimation, PerformanceEnchantment.DISPLAY_NAME)
		{
			this.mTTL = 10f;
		}

		// Token: 0x06001FB8 RID: 8120 RVA: 0x000DE8E9 File Offset: 0x000DCAE9
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("PerformanceEnchantment have to be cast by a character!");
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x000DE8F5 File Offset: 0x000DCAF5
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			return this.Execute(iOwner, iPlayState, true);
		}

		// Token: 0x06001FBA RID: 8122 RVA: 0x000DE900 File Offset: 0x000DCB00
		public bool Execute(ISpellCaster iOwner, PlayState iPlayState, bool iSound)
		{
			if (!(iOwner is Character))
			{
				this.OnRemove();
				return false;
			}
			for (int i = 0; i < PerformanceEnchantment.sActiveEnchantments.Count; i++)
			{
				if (PerformanceEnchantment.sActiveEnchantments[i].mOwner == iOwner)
				{
					PerformanceEnchantment.sActiveEnchantments[i].mTTL = 10f;
					Vector3 direction = iOwner.Direction;
					Vector3 position = iOwner.Position;
					EffectManager.Instance.Stop(ref PerformanceEnchantment.sActiveEnchantments[i].mEffect);
					EffectManager.Instance.StartEffect(PerformanceEnchantment.EFFECT, ref position, ref direction, out PerformanceEnchantment.sActiveEnchantments[i].mEffect);
					AudioManager.Instance.PlayCue(Banks.Additional, PerformanceEnchantment.SOUND_BUFF, iOwner.AudioEmitter);
					PerformanceEnchantment.sActiveEnchantments[i].mBoosted = true;
					this.OnRemove();
					return true;
				}
			}
			this.mOwner = (iOwner as Character);
			this.mTTL = 10f;
			if (this.mOwner != null)
			{
				this.Boost();
				SpellManager.Instance.AddSpellEffect(this);
				return true;
			}
			this.OnRemove();
			return false;
		}

		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x06001FBB RID: 8123 RVA: 0x000DEA1C File Offset: 0x000DCC1C
		public bool IsDead
		{
			get
			{
				if (this.mTTL <= 0f)
				{
					this.mOwner.ClearAura();
					return true;
				}
				return false;
			}
		}

		// Token: 0x06001FBC RID: 8124 RVA: 0x000DEA3C File Offset: 0x000DCC3C
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (this.mOwner == null)
			{
				return;
			}
			if (this.mOwner.CharacterBody == null)
			{
				return;
			}
			if (this.mTTL < 4f)
			{
				this.Withdrawal();
				if (this.mOwner.CharacterBody.IsBodyEnabled)
				{
					float num = (float)Math.Pow(0.75, (double)this.mTTL);
					this.mOwner.CharacterBody.SpeedMultiplier *= 0.5f + (0.5f - num);
				}
			}
			else if (this.mOwner.CharacterBody.IsBodyEnabled)
			{
				float num2 = (float)Math.Pow(0.5, (double)(this.mTTL - 4f));
				this.mOwner.CharacterBody.SpeedMultiplier *= 1f + (1f - num2);
			}
			Vector3 direction = this.mOwner.Direction;
			Vector3 position = this.mOwner.Position;
			EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref position, ref direction);
		}

		// Token: 0x06001FBD RID: 8125 RVA: 0x000DEB54 File Offset: 0x000DCD54
		public void Boost()
		{
			this.mBoosted = true;
			AuraBuff iAura = new AuraBuff(new BuffStorage(new BuffResistance(new Resistance
			{
				ResistanceAgainst = Elements.All,
				Multiplier = 0.5f
			}), VisualCategory.Defensive, Spell.SHIELDCOLOR));
			this.resistanceAura = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, this.mTTL, 0f, VisualCategory.Special, Spell.SHIELDCOLOR, null, Factions.NONE);
			this.mOwner.AddAura(ref this.resistanceAura, true);
			BuffBoostDamage iBuff = new BuffBoostDamage(new Damage(AttackProperties.Damage, Elements.All, 2f, 2f));
			BuffStorage iBuff2 = new BuffStorage(iBuff, VisualCategory.Offensive, default(Vector3));
			AuraBuff iAura2 = new AuraBuff(iBuff2);
			this.damageAura = new AuraStorage(iAura2, AuraTarget.Self, AuraType.Buff, 0, this.mTTL, 0f, VisualCategory.None, default(Vector3), null, Factions.NONE);
			this.mOwner.AddAura(ref this.damageAura, true);
			Vector3 direction = this.mOwner.Direction;
			Vector3 position = this.mOwner.Position;
			EffectManager.Instance.StartEffect(PerformanceEnchantment.EFFECT, ref position, ref direction, out this.mEffect);
			AudioManager.Instance.PlayCue(Banks.Additional, PerformanceEnchantment.SOUND_BUFF, this.mOwner.AudioEmitter);
		}

		// Token: 0x06001FBE RID: 8126 RVA: 0x000DEC9C File Offset: 0x000DCE9C
		public void Withdrawal()
		{
			if (!this.mBoosted)
			{
				return;
			}
			this.mBoosted = false;
			AuraBuff auraBuff = new AuraBuff(new BuffStorage(new BuffResistance(new Resistance
			{
				ResistanceAgainst = Elements.All,
				Multiplier = 2f
			}), VisualCategory.Defensive, Spell.SHIELDCOLOR));
			BuffBoostDamage iBuff = new BuffBoostDamage(new Damage(AttackProperties.Damage, Elements.All, 0.25f, 0.25f));
			AuraBuff auraBuff2 = new AuraBuff(new BuffStorage(iBuff, VisualCategory.Offensive, default(Vector3)));
			this.damageAura.AuraBuff = auraBuff2;
			this.resistanceAura.AuraBuff = auraBuff;
			if (EffectManager.Instance.IsActive(ref this.mEffect))
			{
				EffectManager.Instance.Stop(ref this.mEffect);
			}
			AudioManager.Instance.PlayCue(Banks.Additional, PerformanceEnchantment.SOUND_NERF, this.mOwner.AudioEmitter);
		}

		// Token: 0x06001FBF RID: 8127 RVA: 0x000DED80 File Offset: 0x000DCF80
		public void OnRemove()
		{
			this.mOwner = null;
			PerformanceEnchantment.sActiveEnchantments.Remove(this);
			PerformanceEnchantment.sCache.Add(this);
			if (EffectManager.Instance.IsActive(ref this.mEffect))
			{
				EffectManager.Instance.Stop(ref this.mEffect);
			}
		}

		// Token: 0x040021FF RID: 8703
		private const float BOOST_TTL = 6f;

		// Token: 0x04002200 RID: 8704
		private const float WITHDRAWAL_TTL = 4f;

		// Token: 0x04002201 RID: 8705
		private const float BOOST_RESISTANCE = 0.5f;

		// Token: 0x04002202 RID: 8706
		private const float BOOST_DAMAGE = 2f;

		// Token: 0x04002203 RID: 8707
		private const float WITHDRAWAL_RESISTANCE = 2f;

		// Token: 0x04002204 RID: 8708
		private const float WITHDRAWAL_DAMAGE = 0.25f;

		// Token: 0x04002205 RID: 8709
		private static List<PerformanceEnchantment> sCache;

		// Token: 0x04002206 RID: 8710
		private static List<PerformanceEnchantment> sActiveEnchantments;

		// Token: 0x04002207 RID: 8711
		private static readonly int EFFECT = "special_woot_performance".GetHashCodeCustom();

		// Token: 0x04002208 RID: 8712
		private static readonly int DISPLAY_NAME = "#magick_performance".GetHashCodeCustom();

		// Token: 0x04002209 RID: 8713
		public static readonly int SOUND_BUFF = "woot_magick_enchantment_buff".GetHashCodeCustom();

		// Token: 0x0400220A RID: 8714
		public static readonly int SOUND_NERF = "woot_magick_enchantment_debuff".GetHashCodeCustom();

		// Token: 0x0400220B RID: 8715
		private float mTTL;

		// Token: 0x0400220C RID: 8716
		private bool mBoosted;

		// Token: 0x0400220D RID: 8717
		private Character mOwner;

		// Token: 0x0400220E RID: 8718
		private AuraStorage resistanceAura;

		// Token: 0x0400220F RID: 8719
		private AuraStorage damageAura;

		// Token: 0x04002210 RID: 8720
		private VisualEffectReference mEffect;
	}
}
