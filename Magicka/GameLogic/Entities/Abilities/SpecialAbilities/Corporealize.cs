using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020005B3 RID: 1459
	internal class Corporealize : SpecialAbility
	{
		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x06002BA5 RID: 11173 RVA: 0x00158B8C File Offset: 0x00156D8C
		public static Corporealize Instance
		{
			get
			{
				if (Corporealize.mSingelton == null)
				{
					lock (Corporealize.mSingeltonLock)
					{
						if (Corporealize.mSingelton == null)
						{
							Corporealize.mSingelton = new Corporealize();
						}
					}
				}
				return Corporealize.mSingelton;
			}
		}

		// Token: 0x06002BA6 RID: 11174 RVA: 0x00158BE0 File Offset: 0x00156DE0
		public Corporealize(Animations iAnimation) : base(iAnimation, "#magick_corporealize".GetHashCodeCustom())
		{
		}

		// Token: 0x06002BA7 RID: 11175 RVA: 0x00158BF3 File Offset: 0x00156DF3
		public Corporealize() : base(Animations.cast_magick_self, "#magick_corporealize".GetHashCodeCustom())
		{
		}

		// Token: 0x06002BA8 RID: 11176 RVA: 0x00158C07 File Offset: 0x00156E07
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return this.Execute(null, iPlayState);
		}

		// Token: 0x06002BA9 RID: 11177 RVA: 0x00158C14 File Offset: 0x00156E14
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			AudioManager.Instance.PlayCue(Banks.Spells, Corporealize.SOUND_HASH);
			base.Execute(iOwner, iPlayState);
			Flash.Instance.Execute(iPlayState.Scene, 0.2f);
			List<Entity> entities = iPlayState.EntityManager.GetEntities(iOwner.Position, 30f, false);
			for (int i = 0; i < entities.Count; i++)
			{
				Entity entity = entities[i];
				Character character = entity as Character;
				if (character != null)
				{
					character.IsEthereal = false;
				}
				BossDamageZone bossDamageZone = entity as BossDamageZone;
				if (bossDamageZone != null)
				{
					bossDamageZone.IsEthereal = false;
					if (bossDamageZone.Owner is Grimnir2)
					{
						(bossDamageZone.Owner as Grimnir2).Corporealize();
					}
					if (bossDamageZone.Owner is Grimnir)
					{
						(bossDamageZone.Owner as Grimnir).Corporealize();
					}
					if (bossDamageZone.Owner is Death)
					{
						(bossDamageZone.Owner as Death).Corporealize();
					}
				}
				if (entity is SummonDeath.MagickDeath)
				{
					(entity as SummonDeath.MagickDeath).IsEthereal = false;
				}
			}
			return true;
		}

		// Token: 0x04002F5A RID: 12122
		private static Corporealize mSingelton;

		// Token: 0x04002F5B RID: 12123
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04002F5C RID: 12124
		private static readonly int SOUND_HASH = "magick_corporealize".GetHashCodeCustom();
	}
}
