using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000302 RID: 770
	public struct AuraLove
	{
		// Token: 0x060017A9 RID: 6057 RVA: 0x0009BE02 File Offset: 0x0009A002
		public AuraLove(float iTTL, float iRadius)
		{
			this.Radius = iRadius;
			this.TTL = iTTL;
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x0009BE12 File Offset: 0x0009A012
		public AuraLove(ContentReader iInput)
		{
			this.Radius = iInput.ReadSingle();
			this.TTL = iInput.ReadSingle();
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x0009BE2C File Offset: 0x0009A02C
		public float Execute(Character iOwner, AuraTarget iAuraTarget, int iEffect, float iRadius)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(iOwner.Position, this.Radius, true);
				entities.Remove(iOwner);
				foreach (Entity entity in entities)
				{
					Character character = entity as Character;
					if (character != null && !character.IsCharmed && (character.Faction & iOwner.Faction) == Factions.NONE)
					{
						character.Charm(iOwner, this.TTL, Charm.CHARM_EFFECT);
						if (state != NetworkState.Offline)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.Handle = character.Handle;
							triggerActionMessage.Id = (int)iOwner.Handle;
							triggerActionMessage.Time = this.TTL;
							triggerActionMessage.Arg = Charm.CHARM_EFFECT;
							triggerActionMessage.ActionType = TriggerActionType.Charm;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
					}
				}
			}
			return 1f;
		}

		// Token: 0x04001967 RID: 6503
		public float Radius;

		// Token: 0x04001968 RID: 6504
		public float TTL;
	}
}
