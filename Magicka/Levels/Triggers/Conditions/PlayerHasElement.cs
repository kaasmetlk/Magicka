using System;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x02000286 RID: 646
	public class PlayerHasElement : Condition
	{
		// Token: 0x06001318 RID: 4888 RVA: 0x000760A3 File Offset: 0x000742A3
		public PlayerHasElement(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x000760AC File Offset: 0x000742AC
		protected override bool InternalMet(Character iSender)
		{
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				Avatar avatar = players[i].Avatar;
				if (players[i].Playing && avatar != null)
				{
					for (int j = 0; j < avatar.SpellQueue.Count; j++)
					{
						if ((avatar.SpellQueue[j].Element & this.mElement) != Elements.None)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x0600131A RID: 4890 RVA: 0x0007611B File Offset: 0x0007431B
		// (set) Token: 0x0600131B RID: 4891 RVA: 0x00076123 File Offset: 0x00074323
		public Elements Element
		{
			get
			{
				return this.mElement;
			}
			set
			{
				this.mElement = value;
			}
		}

		// Token: 0x040014D5 RID: 5333
		private Elements mElement;
	}
}
