using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000672 RID: 1650
	public class Doom_Zap : SpecialAbility
	{
		// Token: 0x060031CD RID: 12749 RVA: 0x0019981C File Offset: 0x00197A1C
		public static Doom_Zap GetInstance()
		{
			if (Doom_Zap.sCache.Count > 0)
			{
				Doom_Zap result = Doom_Zap.sCache[Doom_Zap.sCache.Count - 1];
				Doom_Zap.sCache.RemoveAt(Doom_Zap.sCache.Count - 1);
				return result;
			}
			return new Doom_Zap();
		}

		// Token: 0x060031CE RID: 12750 RVA: 0x0019986C File Offset: 0x00197A6C
		public static void InitializeCache(int iNr)
		{
			Doom_Zap.sCache = new List<Doom_Zap>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Doom_Zap.sCache.Add(new Doom_Zap());
			}
		}

		// Token: 0x060031CF RID: 12751 RVA: 0x0019989F File Offset: 0x00197A9F
		public Doom_Zap(Animations iAnimation) : base(iAnimation, "#specab_doomzap".GetHashCodeCustom())
		{
		}

		// Token: 0x060031D0 RID: 12752 RVA: 0x001998BF File Offset: 0x00197ABF
		private Doom_Zap() : base(Animations.cast_magick_direct, "#specab_doomzap".GetHashCodeCustom())
		{
		}

		// Token: 0x060031D1 RID: 12753 RVA: 0x001998E0 File Offset: 0x00197AE0
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			Vector3 direction = iOwner.Direction;
			LightningBolt lightning = LightningBolt.GetLightning();
			LightningBolt lightning2 = LightningBolt.GetLightning();
			this.mHitList.Clear();
			this.mHitList.Add(iOwner.Handle, 1f);
			DamageCollection5 damageCollection = default(DamageCollection5);
			damageCollection.AddDamage(new Damage
			{
				Amount = 300f,
				AttackProperty = AttackProperties.Damage,
				Element = Elements.Lightning,
				Magnitude = 1f
			});
			Quaternion quaternion = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(30f));
			Vector3 iDirection;
			Vector3.Transform(ref direction, ref quaternion, out iDirection);
			lightning.Cast(iOwner, iOwner.CastSource.Translation, iDirection, this.mHitList, Spell.LIGHTNINGCOLOR * 2f, 2f, 10f, ref damageCollection, null, iPlayState);
			AudioManager.Instance.PlayCue(Banks.Spells, Doom_Zap.SOUND, iOwner.AudioEmitter);
			iPlayState.Camera.CameraShake(iOwner.Position, 1.5f, 0.333f);
			quaternion = Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.ToRadians(30f));
			Vector3.Transform(ref direction, ref quaternion, out iDirection);
			this.mHitList.Clear();
			this.mHitList.Add(iOwner.Handle, 1f);
			if (iOwner is Character && ((Character)iOwner).Equipment[0].Item.Type == Doom_Zap.OTHER_GLOVE_NAME)
			{
				iPlayState.Camera.CameraShake(iOwner.Position, 1.5f, 0.333f);
				AudioManager.Instance.PlayCue(Banks.Spells, Doom_Zap.SOUND, iOwner.AudioEmitter);
				lightning2.Cast(iOwner, ((Character)iOwner).Equipment[0].Item.Position, iDirection, this.mHitList, Spell.LIGHTNINGCOLOR * 2f, 2f, 10f, ref damageCollection, null, iPlayState);
			}
			return true;
		}

		// Token: 0x060031D2 RID: 12754 RVA: 0x00199AEF File Offset: 0x00197CEF
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}

		// Token: 0x04003638 RID: 13880
		private const float RANGE = 10f;

		// Token: 0x04003639 RID: 13881
		private const float DAMAGE = 300f;

		// Token: 0x0400363A RID: 13882
		private static List<Doom_Zap> sCache = null;

		// Token: 0x0400363B RID: 13883
		private static readonly int OTHER_GLOVE_NAME = "weapon_doomgauntlet".GetHashCodeCustom();

		// Token: 0x0400363C RID: 13884
		private HitList mHitList = new HitList(32);

		// Token: 0x0400363D RID: 13885
		public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();
	}
}
