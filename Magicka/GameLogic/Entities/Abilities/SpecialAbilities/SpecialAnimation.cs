using System;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020001FA RID: 506
	internal class SpecialAnimation : SpecialAbility
	{
		// Token: 0x0600109F RID: 4255 RVA: 0x00068670 File Offset: 0x00066870
		public SpecialAnimation(Animations iAnimation, int iDisplayName) : base(iAnimation, iDisplayName)
		{
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x0006867A File Offset: 0x0006687A
		public SpecialAnimation(Animations iAnimation) : base(iAnimation, "#item_specab_bash".GetHashCodeCustom())
		{
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x0006868D File Offset: 0x0006688D
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("This action must be cast by a character!");
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x00068699 File Offset: 0x00066899
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			return true;
		}

		// Token: 0x04000F37 RID: 3895
		private const float TTL = 4f;
	}
}
