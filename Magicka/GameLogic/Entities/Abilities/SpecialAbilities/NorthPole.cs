using System;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020001FD RID: 509
	public class NorthPole : SpecialAbility
	{
		// Token: 0x060010B2 RID: 4274 RVA: 0x00068FE4 File Offset: 0x000671E4
		public NorthPole(Animations iAnimation) : base(iAnimation, "#specab_tsal_firebolt".GetHashCodeCustom())
		{
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x00068FF8 File Offset: 0x000671F8
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				Avatar avatar = iOwner as Avatar;
				Vector3 direction = iOwner.Direction;
				Vector3 position = iOwner.Position;
				Vector3.Multiply(ref direction, 10f, out direction);
				Vector3.Add(ref direction, ref position, out position);
				Teleport.Instance.DoTeleport(iOwner, position, direction, Teleport.TeleportType.Regular);
				avatar.ConjureCold();
				avatar.ConjureCold();
				avatar.ConjureCold();
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = avatar.Handle;
					characterActionMessage.Action = ActionType.CastSpell;
					characterActionMessage.Param0I = 2;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
				avatar.CastType = CastType.Area;
				avatar.CastSpell(true, null);
				return true;
			}
			return this.Execute(iOwner.Position, iPlayState);
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x00069117 File Offset: 0x00067317
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}
	}
}
