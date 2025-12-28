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
	// Token: 0x020001FB RID: 507
	internal class Grow : SpecialAbility, IAbilityEffect
	{
		// Token: 0x060010A3 RID: 4259 RVA: 0x0006869C File Offset: 0x0006689C
		public static Grow GetInstance()
		{
			if (Grow.sCache.Count > 0)
			{
				Grow grow = Grow.sCache[Grow.sCache.Count - 1];
				Grow.sCache.RemoveAt(Grow.sCache.Count - 1);
				Grow.sActiveCache.Add(grow);
				return grow;
			}
			Grow grow2 = new Grow();
			Grow.sActiveCache.Add(grow2);
			return grow2;
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x00068704 File Offset: 0x00066904
		public static void InitializeCache(int iNr)
		{
			Grow.sCache = new List<Grow>(iNr);
			Grow.sActiveCache = new List<Grow>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Grow.sCache.Add(new Grow());
			}
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x00068744 File Offset: 0x00066944
		private Grow() : base(Animations.cast_spell0, "#magick_grow".GetHashCodeCustom())
		{
			Damage iDamage = new Damage(AttackProperties.Damage, Elements.Earth, 0f, 3f);
			BuffBoostDamage iBuff = new BuffBoostDamage(iDamage);
			BuffStorage iBuff2 = new BuffStorage(iBuff, VisualCategory.None, default(Vector3));
			AuraBuff iAura = new AuraBuff(iBuff2);
			AuraStorage aura = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, float.MaxValue, 0f, VisualCategory.None, default(Vector3), null, Factions.NONE);
			this.mDamageAura = default(ActiveAura);
			this.mDamageAura.Aura = aura;
			BuffModifyHitPoints iBuff3 = new BuffModifyHitPoints(2f, 0f);
			iBuff2 = new BuffStorage(iBuff3, VisualCategory.None, default(Vector3), 2f, 0f);
			iAura = new AuraBuff(iBuff2);
			aura = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, float.MaxValue, 0f, VisualCategory.None, default(Vector3), null, Factions.NONE);
			this.mHealthAura = default(ActiveAura);
			this.mHealthAura.Aura = aura;
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x00068846 File Offset: 0x00066A46
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Grow have to be cast by a character!");
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x00068854 File Offset: 0x00066A54
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (!(iOwner is Character))
			{
				this.OnRemove();
				return false;
			}
			if (Shrink.KillShrinkOnCharacter(iOwner))
			{
				AudioManager.Instance.PlayCue(Banks.Additional, Grow.SOUND_EFFECT, iOwner.AudioEmitter);
				this.OnRemove();
				return true;
			}
			for (int i = 0; i < Grow.sActiveCache.Count; i++)
			{
				if (Grow.sActiveCache[i].mOwner == iOwner)
				{
					Grow.sActiveCache[i].mTTL = 20f;
					this.OnRemove();
					return true;
				}
			}
			this.mOwner = (iOwner as Character);
			this.mTTL = 20f;
			this.mAnimationTime = 0f;
			if (this.mOwner != null)
			{
				AudioManager.Instance.PlayCue(Banks.Additional, Grow.SOUND_EFFECT, this.mOwner.AudioEmitter);
				SpellManager.Instance.AddSpellEffect(this);
				Vector3 position = this.mOwner.Position;
				Vector3 direction = this.mOwner.Direction;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Grow.MAGICK_EFFECT, ref position, ref direction, out visualEffectReference);
				return true;
			}
			this.OnRemove();
			return false;
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x00068978 File Offset: 0x00066B78
		internal static bool KillGrowOnCharacter(ISpellCaster iOwner)
		{
			for (int i = 0; i < Grow.sActiveCache.Count; i++)
			{
				if (Grow.sActiveCache[i].mOwner == iOwner)
				{
					Grow.sActiveCache[i].mTTL = 0f;
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x060010A9 RID: 4265 RVA: 0x000689C5 File Offset: 0x00066BC5
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f && this.mAnimationTime <= 0f;
			}
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x000689E8 File Offset: 0x00066BE8
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
			float num2 = num + 1f;
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
			this.mOwner.CharacterBody.SpeedMultiplier *= 1f - 0.5f * num;
			this.mOwner.Body.Mass = this.mOwner.Template.Mass + 750f * num;
			this.mDamageAura.Execute(this.mOwner, iDeltaTime);
			this.mHealthAura.Execute(this.mOwner, iDeltaTime);
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x00068B90 File Offset: 0x00066D90
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
			Grow.sActiveCache.Remove(this);
			Grow.sCache.Add(this);
		}

		// Token: 0x04000F38 RID: 3896
		private const float TTL = 20f;

		// Token: 0x04000F39 RID: 3897
		private static List<Grow> sCache;

		// Token: 0x04000F3A RID: 3898
		private static List<Grow> sActiveCache;

		// Token: 0x04000F3B RID: 3899
		private static readonly int MAGICK_EFFECT = "magick_grow".GetHashCodeCustom();

		// Token: 0x04000F3C RID: 3900
		private static readonly int SOUND_EFFECT = "magick_grow".GetHashCodeCustom();

		// Token: 0x04000F3D RID: 3901
		private Character mOwner;

		// Token: 0x04000F3E RID: 3902
		private float mTTL;

		// Token: 0x04000F3F RID: 3903
		private float mAnimationTime;

		// Token: 0x04000F40 RID: 3904
		private ActiveAura mDamageAura;

		// Token: 0x04000F41 RID: 3905
		private ActiveAura mHealthAura;
	}
}
