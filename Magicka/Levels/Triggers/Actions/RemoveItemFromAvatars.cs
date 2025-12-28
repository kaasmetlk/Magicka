using System;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020005CE RID: 1486
	internal class RemoveItemFromAvatars : Action
	{
		// Token: 0x06002C6C RID: 11372 RVA: 0x0015DA9C File Offset: 0x0015BC9C
		public RemoveItemFromAvatars(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002C6D RID: 11373 RVA: 0x0015DAA8 File Offset: 0x0015BCA8
		protected override void Execute()
		{
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing)
				{
					Avatar avatar = players[i].Avatar;
					if (avatar != null)
					{
						for (int j = 0; j < avatar.Equipment.Length; j++)
						{
							if (avatar.Equipment[j].Item.Type == this.mItemID)
							{
								avatar.Template.Equipment[j].CopyToInstance(avatar.Equipment[j]);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002C6E RID: 11374 RVA: 0x0015DB2B File Offset: 0x0015BD2B
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000A79 RID: 2681
		// (get) Token: 0x06002C6F RID: 11375 RVA: 0x0015DB33 File Offset: 0x0015BD33
		// (set) Token: 0x06002C70 RID: 11376 RVA: 0x0015DB3B File Offset: 0x0015BD3B
		public string Item
		{
			get
			{
				return this.mItemName;
			}
			set
			{
				this.mItemName = value;
				this.mItemID = this.mItemName.GetHashCodeCustom();
			}
		}

		// Token: 0x04003005 RID: 12293
		private string mItemName;

		// Token: 0x04003006 RID: 12294
		private int mItemID;
	}
}
