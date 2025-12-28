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
	// Token: 0x020004DB RID: 1243
	internal class Shrink : SpecialAbility, IAbilityEffect
	{
		// Token: 0x060024F3 RID: 9459 RVA: 0x0010AC78 File Offset: 0x00108E78
		public static Shrink GetInstance()
		{
			if (Shrink.sCache.Count > 0)
			{
				Shrink shrink = Shrink.sCache[Shrink.sCache.Count - 1];
				Shrink.sCache.RemoveAt(Shrink.sCache.Count - 1);
				Shrink.sActiveCache.Add(shrink);
				return shrink;
			}
			Shrink shrink2 = new Shrink();
			Shrink.sActiveCache.Add(shrink2);
			return shrink2;
		}

		// Token: 0x060024F4 RID: 9460 RVA: 0x0010ACE0 File Offset: 0x00108EE0
		public static void InitializeCache(int iNr)
		{
			Shrink.sCache = new List<Shrink>(iNr);
			Shrink.sActiveCache = new List<Shrink>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Shrink.sCache.Add(new Shrink());
			}
		}

		// Token: 0x060024F5 RID: 9461 RVA: 0x0010AD20 File Offset: 0x00108F20
		private Shrink() : base(Animations.cast_spell0, "#magick_shrink".GetHashCodeCustom())
		{
			Damage iDamage = new Damage(AttackProperties.Damage, Elements.Earth, 0f, 0.5f);
			BuffBoostDamage iBuff = new BuffBoostDamage(iDamage);
			BuffStorage iBuff2 = new BuffStorage(iBuff, VisualCategory.None, default(Vector3));
			AuraBuff iAura = new AuraBuff(iBuff2);
			AuraStorage aura = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, float.MaxValue, 0f, VisualCategory.None, default(Vector3), null, Factions.NONE);
			this.mDamageAura = default(ActiveAura);
			this.mDamageAura.Aura = aura;
			BuffModifyHitPoints iBuff3 = new BuffModifyHitPoints(0.5f, 0f);
			iBuff2 = new BuffStorage(iBuff3, VisualCategory.None, default(Vector3), 0.5f, 0f);
			iAura = new AuraBuff(iBuff2);
			aura = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, float.MaxValue, 0f, VisualCategory.None, default(Vector3), null, Factions.NONE);
			this.mHealthAura = default(ActiveAura);
			this.mHealthAura.Aura = aura;
		}

		// Token: 0x060024F6 RID: 9462 RVA: 0x0010AE22 File Offset: 0x00109022
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Shrink have to be cast by a character!");
		}

		// Token: 0x060024F7 RID: 9463 RVA: 0x0010AE30 File Offset: 0x00109030
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (!(iOwner is Character))
			{
				this.OnRemove();
				return false;
			}
			if (Grow.KillGrowOnCharacter(iOwner))
			{
				AudioManager.Instance.PlayCue(Banks.Additional, Shrink.SOUND_EFFECT, iOwner.AudioEmitter);
				this.OnRemove();
				return true;
			}
			for (int i = 0; i < Shrink.sActiveCache.Count; i++)
			{
				if (Shrink.sActiveCache[i].mOwner == iOwner)
				{
					Shrink.sActiveCache[i].mTTL = 10f;
					this.OnRemove();
					return true;
				}
			}
			this.mOwner = (iOwner as Character);
			this.mTTL = 10f;
			this.mAnimationTime = 0f;
			if (this.mOwner != null)
			{
				AudioManager.Instance.PlayCue(Banks.Additional, Shrink.SOUND_EFFECT, this.mOwner.AudioEmitter);
				SpellManager.Instance.AddSpellEffect(this);
				Vector3 position = this.mOwner.Position;
				Vector3 direction = this.mOwner.Direction;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Shrink.MAGICK_EFFECT, ref position, ref direction, out visualEffectReference);
				return true;
			}
			this.OnRemove();
			return false;
		}

		// Token: 0x060024F8 RID: 9464 RVA: 0x0010AF54 File Offset: 0x00109154
		internal static bool KillShrinkOnCharacter(ISpellCaster iOwner)
		{
			for (int i = 0; i < Shrink.sActiveCache.Count; i++)
			{
				if (Shrink.sActiveCache[i].mOwner == iOwner)
				{
					Shrink.sActiveCache[i].mTTL = 0f;
					return true;
				}
			}
			return false;
		}

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x060024F9 RID: 9465 RVA: 0x0010AFA1 File Offset: 0x001091A1
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f && this.mAnimationTime <= 0f;
			}
		}

		// Token: 0x060024FA RID: 9466 RVA: 0x0010AFC4 File Offset: 0x001091C4
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (this.mTTL <= 0f || this.mOwner.HitPoints <= 0f)
			{
				this.mAnimationTime = Math.Max(this.mAnimationTime - iDeltaTime, 0f);
			}
			else if (this.mAnimationTime < 1f)
			{
				this.mAnimationTime = Math.Min(this.mAnimationTime + iDeltaTime, 1f);
			}
			float num = this.mAnimationTime + (float)Math.Sin((double)(this.mAnimationTime * 6.2831855f * 4f)) * 0.125f;
			num = Math.Max(Math.Min(num, 1f), 0f);
			float num2 = 1f - 0.5f * num;
			float length = this.mOwner.Template.Length;
			float radius = this.mOwner.Template.Radius;
			float length2 = this.mOwner.Capsule.Length;
			float radius2 = this.mOwner.Capsule.Radius;
			float num3 = length * num2;
			float num4 = radius * num2;
			if (num3 != length2 || num4 != radius2)
			{
				this.mOwner.SetCapsuleForm(num3, num4);
				this.mOwner.SetStaticTransform(num2, num2, num2);
			}
			this.mOwner.CharacterBody.SpeedMultiplier *= 1f + num * 0.5f;
			this.mOwner.Body.Mass = this.mOwner.Template.Mass * (0.5f + (1f - num) * 0.5f);
			this.mDamageAura.Execute(this.mOwner, iDeltaTime);
			this.mHealthAura.Execute(this.mOwner, iDeltaTime);
		}

		// Token: 0x060024FB RID: 9467 RVA: 0x0010B180 File Offset: 0x00109380
		public void OnRemove()
		{
			if (this.mOwner != null)
			{
				this.mOwner.ResetCapsuleForm();
				this.mOwner.ResetStaticTransform();
				this.mOwner.Body.Mass = this.mOwner.Template.Mass;
			}
			this.mAnimationTime = 0f;
			this.mOwner = null;
			Shrink.sActiveCache.Remove(this);
			Shrink.sCache.Add(this);
		}

		// Token: 0x0400284D RID: 10317
		private const float TTL = 10f;

		// Token: 0x0400284E RID: 10318
		private static List<Shrink> sCache;

		// Token: 0x0400284F RID: 10319
		private static List<Shrink> sActiveCache;

		// Token: 0x04002850 RID: 10320
		private static readonly int SOUND_EFFECT = "magick_shrink".GetHashCodeCustom();

		// Token: 0x04002851 RID: 10321
		private static readonly int MAGICK_EFFECT = "magick_shrink".GetHashCodeCustom();

		// Token: 0x04002852 RID: 10322
		private Character mOwner;

		// Token: 0x04002853 RID: 10323
		private float mTTL;

		// Token: 0x04002854 RID: 10324
		private float mAnimationTime;

		// Token: 0x04002855 RID: 10325
		private ActiveAura mDamageAura;

		// Token: 0x04002856 RID: 10326
		private ActiveAura mHealthAura;
	}
}
