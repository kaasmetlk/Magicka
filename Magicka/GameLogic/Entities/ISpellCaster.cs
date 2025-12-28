using System;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020001BA RID: 442
	public interface ISpellCaster : IStatusEffected, IDamageable
	{
		// Token: 0x06000D9C RID: 3484
		MissileEntity GetMissileInstance();

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06000D9D RID: 3485
		AudioEmitter AudioEmitter { get; }

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000D9E RID: 3486
		AnimationController AnimationController { get; }

		// Token: 0x06000D9F RID: 3487
		void AddSelfShield(Spell iSpell);

		// Token: 0x06000DA0 RID: 3488
		void RemoveSelfShield(Character.SelfShieldType iType);

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06000DA1 RID: 3489
		CastType CastType { get; }

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06000DA2 RID: 3490
		Vector3 Direction { get; }

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06000DA3 RID: 3491
		PlayState PlayState { get; }

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06000DA4 RID: 3492
		Matrix CastSource { get; }

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06000DA5 RID: 3493
		Matrix WeaponSource { get; }

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06000DA6 RID: 3494
		// (set) Token: 0x06000DA7 RID: 3495
		float SpellPower { get; set; }

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06000DA8 RID: 3496
		// (set) Token: 0x06000DA9 RID: 3497
		SpellEffect CurrentSpell { get; set; }

		// Token: 0x06000DAA RID: 3498
		bool HasPassiveAbility(Item.PassiveAbilities iAbility);
	}
}
