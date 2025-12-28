using System;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200058E RID: 1422
	public class AlucartDeathExplosion : SpecialAbility
	{
		// Token: 0x06002A74 RID: 10868 RVA: 0x0014D7D4 File Offset: 0x0014B9D4
		public AlucartDeathExplosion(Animations iAnimation) : base(iAnimation, "#specab_tsal_firebolt".GetHashCodeCustom())
		{
			this.mSpell = default(Spell);
			this.mSpell.Element = (Elements.Earth | Elements.Fire);
			this.mSpell.EarthMagnitude = 3f;
			this.mSpell.FireMagnitude = 100f;
		}

		// Token: 0x06002A75 RID: 10869 RVA: 0x0014D82C File Offset: 0x0014BA2C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				Vector3 translation = iOwner.CastSource.Translation;
				Vector3 direction = iOwner.Direction;
				if (iOwner is Avatar && (iOwner as Avatar).Template.ID == AlucartDeathExplosion.ROBE_ROGUE)
				{
					translation.Y += 1f;
				}
				Vector3.Multiply(ref direction, 0f, out direction);
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

		// Token: 0x06002A76 RID: 10870 RVA: 0x0014D99E File Offset: 0x0014BB9E
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}

		// Token: 0x04002DD2 RID: 11730
		private const float VELOCITY_MODIFIER = 0f;

		// Token: 0x04002DD3 RID: 11731
		private const float DAMAGE_RADIUS = 5f;

		// Token: 0x04002DD4 RID: 11732
		public static readonly int EFFECT_TRAIL_HASH = "firebolt_trail".GetHashCode();

		// Token: 0x04002DD5 RID: 11733
		public static readonly int EFFECT_HIT_HASH = "firebolt_hit".GetHashCode();

		// Token: 0x04002DD6 RID: 11734
		public static readonly int SOUND_HASH = "spell_fire_projectile".GetHashCode();

		// Token: 0x04002DD7 RID: 11735
		public static readonly int SOUND_HIT_HASH = "spell_fire_blast".GetHashCode();

		// Token: 0x04002DD8 RID: 11736
		public static readonly int SOUND_FORCE_HASH = "spell_earth_force".GetHashCode();

		// Token: 0x04002DD9 RID: 11737
		private static readonly int ROBE_ROGUE = "wizard_rogue".GetHashCodeCustom();

		// Token: 0x04002DDA RID: 11738
		private Spell mSpell;
	}
}
