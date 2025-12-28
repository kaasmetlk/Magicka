using System;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020004DC RID: 1244
	internal class CoffeeBoost : SpecialAbility
	{
		// Token: 0x060024FD RID: 9469 RVA: 0x0010B214 File Offset: 0x00109414
		public CoffeeBoost(Animations iAnimation) : base(Animations.special0, CoffeeBoost.DISPLAY_NAME)
		{
			this.mHealingA = new Damage(AttackProperties.Damage, Elements.Life, -100f, 1f);
			this.mHealingB = new Damage(AttackProperties.Status, Elements.Life, -100f, 2f);
		}

		// Token: 0x060024FE RID: 9470 RVA: 0x0010B262 File Offset: 0x00109462
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("CoffeeBoost have to be cast by a character!");
		}

		// Token: 0x060024FF RID: 9471 RVA: 0x0010B270 File Offset: 0x00109470
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			DamageResult damageResult = iOwner.Damage(this.mHealingA, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
			damageResult |= iOwner.Damage(this.mHealingB, iOwner as Entity, this.mTimeStamp, iOwner.CastSource.Translation);
			Haste instance = Haste.GetInstance();
			instance.CustomTTL = 4f;
			return instance.Execute(iOwner, iPlayState, false);
		}

		// Token: 0x04002857 RID: 10327
		private const float TTL = 4f;

		// Token: 0x04002858 RID: 10328
		private static readonly int DISPLAY_NAME = "#specab_drink".GetHashCodeCustom();

		// Token: 0x04002859 RID: 10329
		private Damage mHealingA;

		// Token: 0x0400285A RID: 10330
		private Damage mHealingB;
	}
}
