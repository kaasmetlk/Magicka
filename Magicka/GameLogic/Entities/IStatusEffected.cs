using System;
using Magicka.GameLogic.Spells;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000159 RID: 345
	public interface IStatusEffected : IDamageable
	{
		// Token: 0x06000A49 RID: 2633
		bool HasStatus(StatusEffects iStatus);

		// Token: 0x06000A4A RID: 2634
		StatusEffect[] GetStatusEffects();

		// Token: 0x06000A4B RID: 2635
		float StatusMagnitude(StatusEffects iStatus);

		// Token: 0x06000A4C RID: 2636
		void Damage(float iDamage, Elements iElement);

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000A4D RID: 2637
		float Volume { get; }
	}
}
