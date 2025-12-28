using System;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200017E RID: 382
	public class StaffShield : SpecialAbility
	{
		// Token: 0x06000BAB RID: 2987 RVA: 0x00046013 File Offset: 0x00044213
		public StaffShield(Animations iAnimation) : base(iAnimation, "#specab_ss".GetHashCodeCustom())
		{
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x00046028 File Offset: 0x00044228
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			Spell iSpell = default(Spell);
			iSpell.ShieldMagnitude = 1f;
			iSpell.Element = Elements.Shield;
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Action = ActionType.SelfShield;
					characterActionMessage.Handle = iOwner.Handle;
					characterActionMessage.Param0I = (16777216 | (int)((ushort)iSpell.Element));
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
				iOwner.AddSelfShield(iSpell);
			}
			return true;
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x000460C4 File Offset: 0x000442C4
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}
	}
}
