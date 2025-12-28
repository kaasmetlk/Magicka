using System;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020004D9 RID: 1241
	public class DamageImmunity : SpecialAbility
	{
		// Token: 0x060024EC RID: 9452 RVA: 0x0010A915 File Offset: 0x00108B15
		public DamageImmunity(Animations iAnimation) : base(iAnimation, "#specab_immunity".GetHashCodeCustom())
		{
		}

		// Token: 0x060024ED RID: 9453 RVA: 0x0010A928 File Offset: 0x00108B28
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			return true;
		}

		// Token: 0x060024EE RID: 9454 RVA: 0x0010A934 File Offset: 0x00108B34
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}
	}
}
