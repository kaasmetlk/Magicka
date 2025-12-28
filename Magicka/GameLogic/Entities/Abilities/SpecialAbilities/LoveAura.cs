using System;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000318 RID: 792
	public class LoveAura : SpecialAbility
	{
		// Token: 0x06001855 RID: 6229 RVA: 0x000A166B File Offset: 0x0009F86B
		public LoveAura(Animations iAnimation) : base(iAnimation, "#specab_tsal_charm".GetHashCodeCustom())
		{
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x000A1680 File Offset: 0x0009F880
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			Character character = iOwner as Character;
			if (character == null)
			{
				return false;
			}
			AuraStorage auraStorage = new AuraStorage(LoveAura.AURA, AuraTarget.Enemy, AuraType.Love, 0, 5f, 3f, VisualCategory.Special, Spell.FIRECOLOR, null, Factions.NONE);
			character.AddAura(ref auraStorage, false);
			return base.Execute(iOwner, iPlayState);
		}

		// Token: 0x04001A1D RID: 6685
		private static AuraLove AURA = new AuraLove(9f, 3f);
	}
}
