using System;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000613 RID: 1555
	public class LightningDamageAura : SpecialAbility
	{
		// Token: 0x06002E9F RID: 11935 RVA: 0x0017A95A File Offset: 0x00178B5A
		public LightningDamageAura(Animations iAnimation) : base(iAnimation, "#specab_lig_au".GetHashCodeCustom())
		{
		}

		// Token: 0x06002EA0 RID: 11936 RVA: 0x0017A970 File Offset: 0x00178B70
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			AuraBuff iAura = new AuraBuff(new BuffStorage(new BuffDealDamage(new Damage
			{
				Amount = 100f,
				Magnitude = 1f,
				Element = Elements.Lightning,
				AttackProperty = AttackProperties.Damage
			}), VisualCategory.None, Spell.LIGHTNINGCOLOR));
			AuraStorage auraStorage = new AuraStorage(iAura, AuraTarget.AllButSelf, AuraType.Buff, LightningDamageAura.LDA_EFFECT, 5f, 10f, VisualCategory.Offensive, Spell.LIGHTNINGCOLOR, null, Factions.NONE);
			(iOwner as Character).AddAura(ref auraStorage, false);
			return true;
		}

		// Token: 0x06002EA1 RID: 11937 RVA: 0x0017AA04 File Offset: 0x00178C04
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}

		// Token: 0x040032BD RID: 12989
		private static readonly int LDA_EFFECT = "lightning_damage_aura".GetHashCodeCustom();
	}
}
