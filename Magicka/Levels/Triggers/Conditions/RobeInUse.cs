using System;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020000B6 RID: 182
	internal class RobeInUse : Condition
	{
		// Token: 0x06000553 RID: 1363 RVA: 0x0001FFCC File Offset: 0x0001E1CC
		public RobeInUse(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x0001FFD8 File Offset: 0x0001E1D8
		protected override bool InternalMet(Character iSender)
		{
			if (string.IsNullOrEmpty(this.mName))
			{
				return false;
			}
			Player[] players = Game.Instance.Players;
			if (this.mPlayer == 0)
			{
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing && players[i].Avatar != null && players[i].Avatar.Template.ID == this.mNameID)
					{
						return true;
					}
				}
			}
			else if (players[this.mPlayer - 1].Playing && players[this.mPlayer - 1].Avatar != null && players[this.mPlayer - 1].Avatar.Template.ID == this.mNameID)
			{
				return true;
			}
			return false;
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x0002008D File Offset: 0x0001E28D
		// (set) Token: 0x06000556 RID: 1366 RVA: 0x00020095 File Offset: 0x0001E295
		public string Name
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
				this.mNameID = this.mName.ToLowerInvariant().GetHashCodeCustom();
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x000200B4 File Offset: 0x0001E2B4
		// (set) Token: 0x06000558 RID: 1368 RVA: 0x000200BC File Offset: 0x0001E2BC
		public int Player
		{
			get
			{
				return this.mPlayer;
			}
			set
			{
				this.mPlayer = value;
			}
		}

		// Token: 0x04000427 RID: 1063
		private string mName;

		// Token: 0x04000428 RID: 1064
		private int mNameID;

		// Token: 0x04000429 RID: 1065
		private int mPlayer;
	}
}
