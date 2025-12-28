using System;
using Magicka.GameLogic.GameStates;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000132 RID: 306
	internal interface ITargetAbility
	{
		// Token: 0x0600088B RID: 2187
		bool Execute(ISpellCaster iOwner, Entity iTarget, PlayState iPlayState);
	}
}
