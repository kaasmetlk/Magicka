using System;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020004F6 RID: 1270
	internal class PlayersHaveStatusEffect : Condition
	{
		// Token: 0x06002592 RID: 9618 RVA: 0x001113A5 File Offset: 0x0010F5A5
		public PlayersHaveStatusEffect(GameScene iScene) : base(iScene)
		{
			this.mPlayers = Game.Instance.Players;
		}

		// Token: 0x06002593 RID: 9619 RVA: 0x001113C0 File Offset: 0x0010F5C0
		protected override bool InternalMet(Character iSender)
		{
			for (int i = 0; i < this.mPlayers.Length; i++)
			{
				if (this.mPlayers[i].Playing && this.mPlayers[i].Avatar != null)
				{
					for (int j = 0; j < 9; j++)
					{
						StatusEffects iStatus = Magicka.GameLogic.Spells.StatusEffect.StatusFromIndex(j);
						if (this.mPlayers[i].Avatar.HasStatus(iStatus))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x06002594 RID: 9620 RVA: 0x0011142A File Offset: 0x0010F62A
		// (set) Token: 0x06002595 RID: 9621 RVA: 0x00111432 File Offset: 0x0010F632
		public StatusEffects StatusEffect
		{
			get
			{
				return this.mStatusEffects;
			}
			set
			{
				this.mStatusEffects = value;
			}
		}

		// Token: 0x040028FE RID: 10494
		private StatusEffects mStatusEffects;

		// Token: 0x040028FF RID: 10495
		private Player[] mPlayers;
	}
}
