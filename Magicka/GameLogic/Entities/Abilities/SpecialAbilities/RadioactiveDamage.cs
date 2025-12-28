using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020004DD RID: 1245
	public class RadioactiveDamage : SpecialAbility
	{
		// Token: 0x06002501 RID: 9473 RVA: 0x0010B2F8 File Offset: 0x001094F8
		public RadioactiveDamage(Animations iAnimation) : base(iAnimation, "#item_specab_radioactive".GetHashCodeCustom())
		{
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x0010B30C File Offset: 0x0010950C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(RadioactiveDamage.EFFECT, ref position, ref direction, out visualEffectReference);
			Damage damage = default(Damage);
			damage.Amount = 20f;
			damage.Magnitude = 1f;
			damage.Element = Elements.Poison;
			damage.AttackProperty = AttackProperties.Status;
			Helper.CircleDamage(iPlayState, iOwner as Entity, iPlayState.PlayTime, iOwner as Entity, ref position, 7.5f, ref damage);
			AudioManager.Instance.PlayCue(Banks.Spells, RadioactiveDamage.SOUND, iOwner.AudioEmitter);
			return true;
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x0010B3B6 File Offset: 0x001095B6
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}

		// Token: 0x0400285B RID: 10331
		private static readonly int EFFECT = "radioactive_damage".GetHashCodeCustom();

		// Token: 0x0400285C RID: 10332
		public static readonly int SOUND = "magick_raise_dead".GetHashCodeCustom();
	}
}
