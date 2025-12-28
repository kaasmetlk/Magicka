using System;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000037 RID: 55
	public interface IBossSpellCaster : IBoss
	{
		// Token: 0x0600023B RID: 571
		void AddSelfShield(int iIndex, Spell iSpell);

		// Token: 0x0600023C RID: 572
		void RemoveSelfShield(int iIndex, Character.SelfShieldType iType);

		// Token: 0x0600023D RID: 573
		CastType CastType(int iIndex);

		// Token: 0x0600023E RID: 574
		float SpellPower(int iIndex);

		// Token: 0x0600023F RID: 575
		void SpellPower(int iIndex, float iSpellPower);

		// Token: 0x06000240 RID: 576
		SpellEffect CurrentSpell(int iIndex);

		// Token: 0x06000241 RID: 577
		void CurrentSpell(int iIndex, SpellEffect iEffect);
	}
}
