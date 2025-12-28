using System;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200058A RID: 1418
	public class FireImmunityAura : SpecialAbility
	{
		// Token: 0x06002A5D RID: 10845 RVA: 0x0014D00A File Offset: 0x0014B20A
		public FireImmunityAura(Animations iAnimation) : base(iAnimation, "#specab_fire_im".GetHashCodeCustom())
		{
		}

		// Token: 0x06002A5E RID: 10846 RVA: 0x0014D020 File Offset: 0x0014B220
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			AuraBuff iAura = new AuraBuff(new BuffStorage(new BuffResistance(new Resistance
			{
				ResistanceAgainst = Elements.Fire,
				Multiplier = 0f
			}), VisualCategory.Defensive, Spell.FIRECOLOR));
			AuraStorage auraStorage = new AuraStorage(iAura, AuraTarget.Friendly, AuraType.Buff, 0, 10f, 10f, VisualCategory.Defensive, Spell.FIRECOLOR, null, Factions.NONE);
			(iOwner as Character).AddAura(ref auraStorage, false);
			return true;
		}

		// Token: 0x06002A5F RID: 10847 RVA: 0x0014D09B File Offset: 0x0014B29B
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}
	}
}
