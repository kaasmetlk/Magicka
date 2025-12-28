using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020003CD RID: 973
	public class Zap : SpecialAbility
	{
		// Token: 0x06001DCB RID: 7627 RVA: 0x000D1F60 File Offset: 0x000D0160
		public static Zap GetInstance()
		{
			if (Zap.sCache.Count > 0)
			{
				Zap result = Zap.sCache[Zap.sCache.Count - 1];
				Zap.sCache.RemoveAt(Zap.sCache.Count - 1);
				return result;
			}
			return new Zap();
		}

		// Token: 0x06001DCC RID: 7628 RVA: 0x000D1FB0 File Offset: 0x000D01B0
		public static void InitializeCache(int iNr)
		{
			Zap.sCache = new List<Zap>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Zap.sCache.Add(new Zap());
			}
		}

		// Token: 0x06001DCD RID: 7629 RVA: 0x000D1FE3 File Offset: 0x000D01E3
		public Zap(Animations iAnimation) : base(iAnimation, "#specab_lightbolt".GetHashCodeCustom())
		{
		}

		// Token: 0x06001DCE RID: 7630 RVA: 0x000D2003 File Offset: 0x000D0203
		private Zap() : base(Animations.cast_magick_direct, "#magick_chainlightning".GetHashCodeCustom())
		{
		}

		// Token: 0x06001DCF RID: 7631 RVA: 0x000D2024 File Offset: 0x000D0224
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			Vector3 direction = iOwner.Direction;
			LightningBolt lightning = LightningBolt.GetLightning();
			this.mHitList.Clear();
			if (!iOwner.HasStatus(StatusEffects.Wet))
			{
				this.mHitList.Add(iOwner);
			}
			DamageCollection5 damageCollection = default(DamageCollection5);
			damageCollection.AddDamage(new Damage
			{
				Amount = 850f,
				AttackProperty = AttackProperties.Damage,
				Element = Elements.Lightning,
				Magnitude = 1f
			});
			lightning.Cast(iOwner, iOwner.CastSource.Translation, direction, this.mHitList, Spell.LIGHTNINGCOLOR * 2f, 2f, 15f, ref damageCollection, null, iPlayState);
			AudioManager.Instance.PlayCue(Banks.Spells, Zap.SOUND, iOwner.AudioEmitter);
			iPlayState.Camera.CameraShake(iOwner.Position, 1.5f, 0.333f);
			return true;
		}

		// Token: 0x06001DD0 RID: 7632 RVA: 0x000D211F File Offset: 0x000D031F
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}

		// Token: 0x04002051 RID: 8273
		private const float RANGE = 15f;

		// Token: 0x04002052 RID: 8274
		private static List<Zap> sCache;

		// Token: 0x04002053 RID: 8275
		private HitList mHitList = new HitList(32);

		// Token: 0x04002054 RID: 8276
		public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();
	}
}
