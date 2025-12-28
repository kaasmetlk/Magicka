using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000460 RID: 1120
	public class Charm : SpecialAbility
	{
		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x06002233 RID: 8755 RVA: 0x000F4EC4 File Offset: 0x000F30C4
		public static Charm Instance
		{
			get
			{
				if (Charm.mSingelton == null)
				{
					lock (Charm.mSingeltonLock)
					{
						if (Charm.mSingelton == null)
						{
							Charm.mSingelton = new Charm();
						}
					}
				}
				return Charm.mSingelton;
			}
		}

		// Token: 0x06002234 RID: 8756 RVA: 0x000F4F18 File Offset: 0x000F3118
		public Charm(Animations iAnimation) : base(iAnimation, "#magick_charm".GetHashCodeCustom())
		{
		}

		// Token: 0x06002235 RID: 8757 RVA: 0x000F4F2B File Offset: 0x000F312B
		private Charm() : base(Animations.cast_magick_sweep, "#magick_charm".GetHashCodeCustom())
		{
		}

		// Token: 0x06002236 RID: 8758 RVA: 0x000F4F3F File Offset: 0x000F313F
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Charm have to be called by a character!");
		}

		// Token: 0x06002237 RID: 8759 RVA: 0x000F4F4C File Offset: 0x000F314C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state == NetworkState.Client || (iOwner is Avatar && (iOwner as Avatar).Player.Gamer is NetworkGamer)) && (state != NetworkState.Client || !(iOwner is Avatar) || (iOwner as Avatar).Player.Gamer is NetworkGamer))
			{
				return false;
			}
			base.Execute(iOwner, iPlayState);
			if (!(iOwner is Character))
			{
				return false;
			}
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			List<Entity> entities = iPlayState.EntityManager.GetEntities(position, 14f, true);
			Character character = null;
			float num = float.MaxValue;
			for (int i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Character && entities[i] != iOwner && !(entities[i] as Character).IsCharmed && ((iOwner as Character).Faction & (entities[i] as Character).Faction) == Factions.NONE)
				{
					Vector3 position2 = entities[i].Position;
					Vector3 vector;
					Vector3.Subtract(ref position2, ref position, out vector);
					vector.Y = 0f;
					vector.Normalize();
					float num2 = MagickaMath.Angle(ref direction, ref vector);
					if (num2 < num)
					{
						num = num2;
						character = (entities[i] as Character);
					}
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			if (character == null)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
				return false;
			}
			AudioManager.Instance.PlayCue(Banks.Spells, Charm.SOUND, character.AudioEmitter);
			character.Charm(iOwner as Character, 15f, Charm.CHARM_EFFECT);
			if (state != NetworkState.Offline)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.Handle = character.Handle;
				triggerActionMessage.Id = (int)iOwner.Handle;
				triggerActionMessage.Time = 15f;
				triggerActionMessage.Arg = Charm.CHARM_EFFECT;
				triggerActionMessage.ActionType = TriggerActionType.Charm;
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
			return true;
		}

		// Token: 0x04002549 RID: 9545
		public const float CHARM_TIME = 15f;

		// Token: 0x0400254A RID: 9546
		private const float RADIUS = 14f;

		// Token: 0x0400254B RID: 9547
		private static Charm mSingelton;

		// Token: 0x0400254C RID: 9548
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400254D RID: 9549
		public static readonly int CHARM_EFFECT = "magick_charm".GetHashCodeCustom();

		// Token: 0x0400254E RID: 9550
		public static readonly int SOUND = "magick_charm".GetHashCodeCustom();
	}
}
