using System;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020005B0 RID: 1456
	public class Arcanebolt : SpecialAbility
	{
		// Token: 0x06002B90 RID: 11152 RVA: 0x00158424 File Offset: 0x00156624
		public Arcanebolt(Animations iAnimation) : base(iAnimation, "#specab_arcane".GetHashCodeCustom())
		{
			this.mSpell = default(Spell);
			this.mSpell.Element = (Elements.Earth | Elements.Arcane);
			this.mSpell.EarthMagnitude = 2f;
			this.mSpell.ArcaneMagnitude = 5f;
		}

		// Token: 0x06002B91 RID: 11153 RVA: 0x0015847C File Offset: 0x0015667C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				Vector3 translation = iOwner.CastSource.Translation;
				Vector3 direction = iOwner.Direction;
				if (iOwner is Avatar && (iOwner as Avatar).Template.ID == Arcanebolt.ROBE_ROGUE)
				{
					translation.Y += 1f;
				}
				Vector3.Multiply(ref direction, 50f, out direction);
				MissileEntity missileInstance = iOwner.GetMissileInstance();
				ProjectileSpell.SpawnMissile(ref missileInstance, iOwner, 0.15f, ref translation, ref direction, ref this.mSpell, 4f, 1);
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
					spawnMissileMessage.Splash = 4f;
					NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06002B92 RID: 11154 RVA: 0x001585EE File Offset: 0x001567EE
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}

		// Token: 0x04002F47 RID: 12103
		private const float VELOCITY_MODIFIER = 50f;

		// Token: 0x04002F48 RID: 12104
		private const float DAMAGE_RADIUS = 4f;

		// Token: 0x04002F49 RID: 12105
		public static readonly int EFFECT_TRAIL_HASH = "arcanebolt_trail".GetHashCodeCustom();

		// Token: 0x04002F4A RID: 12106
		public static readonly int EFFECT_HIT_HASH = "arcanebolt_hit".GetHashCodeCustom();

		// Token: 0x04002F4B RID: 12107
		public static readonly int SOUND_HASH = "spell_arcane_projectile".GetHashCodeCustom();

		// Token: 0x04002F4C RID: 12108
		public static readonly int SOUND_HIT_HASH = "spell_arcane_blast".GetHashCodeCustom();

		// Token: 0x04002F4D RID: 12109
		private static readonly int ROBE_ROGUE = "wizard_rogue".GetHashCodeCustom();

		// Token: 0x04002F4E RID: 12110
		private Spell mSpell;
	}
}
