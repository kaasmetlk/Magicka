using System;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020002B3 RID: 691
	public class Firebolt : SpecialAbility
	{
		// Token: 0x060014DE RID: 5342 RVA: 0x00082098 File Offset: 0x00080298
		public Firebolt(Animations iAnimation) : base(iAnimation, "#specab_tsal_firebolt".GetHashCodeCustom())
		{
			this.mSpell = default(Spell);
			this.mSpell.Element = (Elements.Earth | Elements.Fire);
			this.mSpell.EarthMagnitude = 2f;
			this.mSpell.FireMagnitude = 5f;
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x000820F0 File Offset: 0x000802F0
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				Vector3 translation = iOwner.CastSource.Translation;
				Vector3 direction = iOwner.Direction;
				if (iOwner is Avatar && (iOwner as Avatar).Template.ID == Firebolt.ROBE_ROGUE)
				{
					translation.Y += 1f;
				}
				Vector3.Multiply(ref direction, 50f, out direction);
				MissileEntity missileInstance = iOwner.GetMissileInstance();
				ProjectileSpell.SpawnMissile(ref missileInstance, iOwner, 0.15f, ref translation, ref direction, ref this.mSpell, 5f, 1);
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
					spawnMissileMessage.Type = SpawnMissileMessage.MissileType.Spell;
					spawnMissileMessage.Handle = missileInstance.Handle;
					spawnMissileMessage.Item = 0;
					spawnMissileMessage.Owner = iOwner.Handle;
					spawnMissileMessage.Position = translation;
					spawnMissileMessage.Velocity = direction;
					spawnMissileMessage.Spell = this.mSpell;
					spawnMissileMessage.Homing = 0.15f;
					spawnMissileMessage.Splash = 5f;
					NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x00082262 File Offset: 0x00080462
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}

		// Token: 0x04001641 RID: 5697
		private const float VELOCITY_MODIFIER = 50f;

		// Token: 0x04001642 RID: 5698
		private const float DAMAGE_RADIUS = 5f;

		// Token: 0x04001643 RID: 5699
		public static readonly int EFFECT_TRAIL_HASH = "firebolt_trail".GetHashCode();

		// Token: 0x04001644 RID: 5700
		public static readonly int EFFECT_HIT_HASH = "firebolt_hit".GetHashCode();

		// Token: 0x04001645 RID: 5701
		public static readonly int SOUND_HASH = "spell_fire_projectile".GetHashCode();

		// Token: 0x04001646 RID: 5702
		public static readonly int SOUND_HIT_HASH = "spell_fire_blast".GetHashCode();

		// Token: 0x04001647 RID: 5703
		public static readonly int SOUND_FORCE_HASH = "spell_earth_force".GetHashCode();

		// Token: 0x04001648 RID: 5704
		private static readonly int ROBE_ROGUE = "wizard_rogue".GetHashCodeCustom();

		// Token: 0x04001649 RID: 5705
		private Spell mSpell;
	}
}
