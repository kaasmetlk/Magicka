using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020003A3 RID: 931
	public class Ensnare : SpecialAbility
	{
		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x06001C8C RID: 7308 RVA: 0x000C63A0 File Offset: 0x000C45A0
		public static Ensnare Instance
		{
			get
			{
				if (Ensnare.mSingelton == null)
				{
					lock (Ensnare.mSingeltonLock)
					{
						if (Ensnare.mSingelton == null)
						{
							Ensnare.mSingelton = new Ensnare();
						}
					}
				}
				return Ensnare.mSingelton;
			}
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x000C63F4 File Offset: 0x000C45F4
		public Ensnare(Animations iAnimation) : base(iAnimation, "#specab_root".GetHashCodeCustom())
		{
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x000C6407 File Offset: 0x000C4607
		private Ensnare() : base(Animations.cast_magick_direct, "#specab_root".GetHashCodeCustom())
		{
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x000C641B File Offset: 0x000C461B
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return this.Execute(iPosition, null, iPlayState);
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x000C6426 File Offset: 0x000C4626
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			return this.Execute(iOwner.Position, iOwner, iPlayState);
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x000C6438 File Offset: 0x000C4638
		private bool Execute(Vector3 iPosition, ISpellCaster iOwner, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return false;
			}
			base.Execute(iOwner, iPlayState);
			Character character = null;
			float num = float.MaxValue;
			List<Entity> entities = iPlayState.EntityManager.GetEntities(iOwner.Position, 10f, true);
			Vector3 vector = iPosition;
			Vector3 vector2 = default(Vector3);
			if (iOwner != null)
			{
				vector2 = iOwner.Direction;
			}
			else
			{
				vector2 = new Vector3((float)SpecialAbility.RANDOM.NextDouble(), 0f, (float)SpecialAbility.RANDOM.NextDouble());
				vector2.Normalize();
			}
			vector.Y = 0f;
			for (int i = 0; i < entities.Count; i++)
			{
				Character character2 = entities[i] as Character;
				if (character2 != null && (iOwner == null || character2 != iOwner))
				{
					Vector3 position = character2.Position;
					position.Y = 0f;
					float num2;
					Vector3.DistanceSquared(ref vector, ref position, out num2);
					if (num2 < num)
					{
						Vector3 vector3;
						Vector3.Subtract(ref position, ref vector, out vector3);
						vector3.Normalize();
						float num3;
						Vector3.Dot(ref vector2, ref vector3, out num3);
						if (num3 > 0.7f)
						{
							character = character2;
							num = num2;
						}
					}
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			if (character != null)
			{
				character.Entangle(4f);
				return true;
			}
			AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
			return false;
		}

		// Token: 0x04001EDD RID: 7901
		private static Ensnare mSingelton;

		// Token: 0x04001EDE RID: 7902
		private static volatile object mSingeltonLock = new object();
	}
}
