using System;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000614 RID: 1556
	public class EpicBuff : SpecialAbility
	{
		// Token: 0x06002EA3 RID: 11939 RVA: 0x0017AA18 File Offset: 0x00178C18
		public EpicBuff(Animations iAnimation) : base(iAnimation, "#item_steam_ability02".GetHashCodeCustom())
		{
			BuffBoostDamage iBuff = new BuffBoostDamage(new Damage(AttackProperties.Damage, Elements.All, 1f, 1.1f));
			BuffStorage iBuff2 = new BuffStorage(iBuff, VisualCategory.Offensive, default(Vector3));
			AuraBuff iAura = new AuraBuff(iBuff2);
			this.mDamageAura = new AuraStorage(iAura, AuraTarget.Self, AuraType.Buff, 0, 8f, 0f, VisualCategory.None, default(Vector3), null, Factions.NONE);
		}

		// Token: 0x06002EA4 RID: 11940 RVA: 0x0017AAA0 File Offset: 0x00178CA0
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (!(iOwner is Character))
			{
				return false;
			}
			Vector3 right = Vector3.Right;
			Vector3 translation = iOwner.CastSource.Translation;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(EpicBuff.EFFECT, ref translation, ref right, out visualEffectReference);
			(iOwner as Character).AddAura(ref this.mDamageAura, true);
			AudioManager.Instance.PlayCue(Banks.Spells, EpicBuff.SOUND, iOwner.AudioEmitter);
			return true;
		}

		// Token: 0x040032BE RID: 12990
		private const float RANGE = 10f;

		// Token: 0x040032BF RID: 12991
		public const float FEAR_TIME = 6f;

		// Token: 0x040032C0 RID: 12992
		public static readonly int SOUND = "magick_revive_cast".GetHashCodeCustom();

		// Token: 0x040032C1 RID: 12993
		public static readonly int EFFECT = "epic_buff".GetHashCodeCustom();

		// Token: 0x040032C2 RID: 12994
		public static readonly int FEARED_EFFECT = "magick_feared".GetHashCodeCustom();

		// Token: 0x040032C3 RID: 12995
		private AudioEmitter mAudioEmitter = new AudioEmitter();

		// Token: 0x040032C4 RID: 12996
		private AuraStorage mDamageAura;
	}
}
