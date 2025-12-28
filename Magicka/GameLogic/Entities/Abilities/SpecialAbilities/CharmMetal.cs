using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000040 RID: 64
	public class CharmMetal : SpecialAbility
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600028D RID: 653 RVA: 0x00010748 File Offset: 0x0000E948
		public static CharmMetal Instance
		{
			get
			{
				if (CharmMetal.mSingelton == null)
				{
					lock (CharmMetal.mSingeltonLock)
					{
						if (CharmMetal.mSingelton == null)
						{
							CharmMetal.mSingelton = new CharmMetal();
						}
					}
				}
				return CharmMetal.mSingelton;
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0001079C File Offset: 0x0000E99C
		public CharmMetal(Animations iAnimation) : base(iAnimation, "#magick_charm".GetHashCodeCustom())
		{
		}

		// Token: 0x0600028F RID: 655 RVA: 0x000107AF File Offset: 0x0000E9AF
		private CharmMetal() : base(Animations.cast_magick_sweep, "#magick_charm".GetHashCodeCustom())
		{
		}

		// Token: 0x06000290 RID: 656 RVA: 0x000107C3 File Offset: 0x0000E9C3
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Charm have to be called by a character!");
		}

		// Token: 0x06000291 RID: 657 RVA: 0x000107D0 File Offset: 0x0000E9D0
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
				return false;
			}
			character.Charm(iOwner as Character, 15f, CharmMetal.CHARM_EFFECT);
			if (state != NetworkState.Offline)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.Handle = character.Handle;
				triggerActionMessage.Id = (int)iOwner.Handle;
				triggerActionMessage.Time = 15f;
				triggerActionMessage.Arg = CharmMetal.CHARM_EFFECT;
				triggerActionMessage.ActionType = TriggerActionType.Charm;
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
			return true;
		}

		// Token: 0x0400021A RID: 538
		public const float CHARM_TIME = 15f;

		// Token: 0x0400021B RID: 539
		private const float RADIUS = 14f;

		// Token: 0x0400021C RID: 540
		private static CharmMetal mSingelton;

		// Token: 0x0400021D RID: 541
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400021E RID: 542
		public static readonly int CHARM_EFFECT = "magick_charm".GetHashCodeCustom();
	}
}
