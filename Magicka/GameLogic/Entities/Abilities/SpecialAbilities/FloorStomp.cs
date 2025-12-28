using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200025B RID: 603
	public class FloorStomp : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06001294 RID: 4756 RVA: 0x00072D9C File Offset: 0x00070F9C
		public static FloorStomp GetInstance()
		{
			if (FloorStomp.sCache.Count > 0)
			{
				FloorStomp result = FloorStomp.sCache[FloorStomp.sCache.Count - 1];
				FloorStomp.sCache.RemoveAt(FloorStomp.sCache.Count - 1);
				return result;
			}
			return new FloorStomp();
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x00072DEC File Offset: 0x00070FEC
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			FloorStomp.sCache = new List<FloorStomp>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				FloorStomp.sCache.Add(new FloorStomp());
			}
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x00072E1F File Offset: 0x0007101F
		public FloorStomp(Animations iAnimation) : base(iAnimation, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x00072E32 File Offset: 0x00071032
		private FloorStomp() : base(Animations.cast_magick_sweep, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x00072E46 File Offset: 0x00071046
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("FloorStomp can not be cast without an owner!");
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x00072E54 File Offset: 0x00071054
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mTTL = 8f;
			this.FIRST_STOMP = 1.15f;
			this.SECOND_STOMP = 2.1f;
			this.THIRD_STOMP = 3.05f;
			this.mOwner = (iOwner as Character);
			this.mPlayState = iPlayState;
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x0600129A RID: 4762 RVA: 0x00072EB5 File Offset: 0x000710B5
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x00072EC8 File Offset: 0x000710C8
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			StatusEffect[] statusEffects = this.mOwner.GetStatusEffects();
			if (this.mOwner.HasStatus(StatusEffects.Cold))
			{
				iDeltaTime *= statusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown();
			}
			if (this.mOwner.ZapTimer > 0f)
			{
				iDeltaTime *= this.mOwner.ZapModifier;
			}
			this.mTTL -= iDeltaTime;
			this.FIRST_STOMP -= iDeltaTime;
			this.SECOND_STOMP -= iDeltaTime;
			this.THIRD_STOMP -= iDeltaTime;
			if (this.FIRST_STOMP < 0f)
			{
				Spell iSpell = default(Spell);
				iSpell.Element = (Elements.Earth | Elements.Fire);
				iSpell.EarthMagnitude = 3f;
				iSpell.FireMagnitude = 2f;
				SpellEffect fromCache = ProjectileSpell.GetFromCache();
				this.mOwner.CurrentSpell = fromCache;
				this.mOwner.CurrentSpell.CastArea(iSpell, this.mOwner, false);
				List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, iSpell.BlastSize() * 10f, false);
				for (int i = 0; i < entities.Count; i++)
				{
					if (entities[i] is Barrier || entities[i] is Shield)
					{
						entities[i].Kill();
					}
				}
				this.mPlayState.EntityManager.ReturnEntityList(entities);
				this.FIRST_STOMP = 1000000f;
			}
			if (this.SECOND_STOMP < 0f)
			{
				Spell iSpell2 = default(Spell);
				iSpell2.Element = (Elements.Earth | Elements.Fire);
				iSpell2.EarthMagnitude = 5f;
				iSpell2.FireMagnitude = 2f;
				SpellEffect fromCache2 = ProjectileSpell.GetFromCache();
				this.mOwner.CurrentSpell = fromCache2;
				this.mOwner.CurrentSpell.CastArea(iSpell2, this.mOwner, false);
				List<Entity> entities2 = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, iSpell2.BlastSize() * 10f, false);
				for (int j = 0; j < entities2.Count; j++)
				{
					if (entities2[j] is Barrier || entities2[j] is Shield)
					{
						entities2[j].Kill();
					}
				}
				this.mPlayState.EntityManager.ReturnEntityList(entities2);
				this.SECOND_STOMP = 1000000f;
			}
			if (this.THIRD_STOMP < 0f)
			{
				Spell iSpell3 = default(Spell);
				iSpell3.Element = (Elements.Earth | Elements.Fire);
				iSpell3.EarthMagnitude = 8f;
				iSpell3.FireMagnitude = 2f;
				SpellEffect fromCache3 = ProjectileSpell.GetFromCache();
				this.mOwner.CurrentSpell = fromCache3;
				this.mOwner.CurrentSpell.CastArea(iSpell3, this.mOwner, false);
				List<Entity> entities3 = this.mPlayState.EntityManager.GetEntities(this.mOwner.Position, iSpell3.BlastSize() * 10f, false);
				for (int k = 0; k < entities3.Count; k++)
				{
					if (entities3[k] is Barrier || entities3[k] is Shield)
					{
						entities3[k].Kill();
					}
				}
				this.mPlayState.EntityManager.ReturnEntityList(entities3);
				this.THIRD_STOMP = 1000000f;
			}
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x0007322A File Offset: 0x0007142A
		public void OnRemove()
		{
			this.mTTL = 0f;
			EffectManager.Instance.Stop(ref this.mEffect);
			FloorStomp.sCache.Add(this);
		}

		// Token: 0x04001153 RID: 4435
		private static List<FloorStomp> sCache;

		// Token: 0x04001154 RID: 4436
		private Character mOwner;

		// Token: 0x04001155 RID: 4437
		public PlayState mPlayState;

		// Token: 0x04001156 RID: 4438
		private VisualEffectReference mEffect;

		// Token: 0x04001157 RID: 4439
		private float mTTL;

		// Token: 0x04001158 RID: 4440
		private float FIRST_STOMP;

		// Token: 0x04001159 RID: 4441
		private float SECOND_STOMP;

		// Token: 0x0400115A RID: 4442
		private float THIRD_STOMP;
	}
}
