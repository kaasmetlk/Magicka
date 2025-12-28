using System;
using System.Xml;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200041A RID: 1050
	public class SetFaction : Action
	{
		// Token: 0x0600207A RID: 8314 RVA: 0x000E6A8F File Offset: 0x000E4C8F
		public SetFaction(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600207B RID: 8315 RVA: 0x000E6A99 File Offset: 0x000E4C99
		public override void Initialize()
		{
			base.Initialize();
		}

		// Token: 0x0600207C RID: 8316 RVA: 0x000E6AA4 File Offset: 0x000E4CA4
		protected override void Execute()
		{
			if (!this.mIsSpecific)
			{
				TriggerArea triggerArea = base.GameScene.GetTriggerArea(this.mAreaHash);
				for (int i = 0; i < triggerArea.PresentCharacters.Count; i++)
				{
					Character character = triggerArea.PresentCharacters[i];
					if (character != null && (this.mTypeHash == SetFaction.ANYID || character.Type == this.mTypeHash))
					{
						switch (this.mAction)
						{
						case OperationAction.Add:
							character.Faction |= this.mFaction;
							break;
						case OperationAction.Remove:
							character.Faction &= ~this.mFaction;
							break;
						case OperationAction.Set:
							character.Faction = this.mFaction;
							break;
						}
					}
				}
				return;
			}
			Character character2 = null;
			int num = -1;
			if (this.mID.Equals(SetFaction.PLAYER1, StringComparison.OrdinalIgnoreCase))
			{
				num = 0;
			}
			else if (this.mID.Equals(SetFaction.PLAYER2, StringComparison.OrdinalIgnoreCase))
			{
				num = 1;
			}
			else if (this.mID.Equals(SetFaction.PLAYER3, StringComparison.OrdinalIgnoreCase))
			{
				num = 2;
			}
			else if (this.mID.Equals(SetFaction.PLAYER4, StringComparison.OrdinalIgnoreCase))
			{
				num = 3;
			}
			if (num != -1)
			{
				if (Game.Instance.Players[num].Playing && Game.Instance.Players[num].Avatar != null)
				{
					character2 = Game.Instance.Players[num].Avatar;
				}
			}
			else
			{
				character2 = (Entity.GetByID(this.mIDHash) as Character);
			}
			if (character2 == null)
			{
				return;
			}
			switch (this.mAction)
			{
			case OperationAction.Add:
				character2.Faction |= this.mFaction;
				return;
			case OperationAction.Remove:
				character2.Faction &= ~this.mFaction;
				return;
			case OperationAction.Set:
				character2.Faction = this.mFaction;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600207D RID: 8317 RVA: 0x000E6C72 File Offset: 0x000E4E72
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x0600207E RID: 8318 RVA: 0x000E6C7A File Offset: 0x000E4E7A
		// (set) Token: 0x0600207F RID: 8319 RVA: 0x000E6C82 File Offset: 0x000E4E82
		public string ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				if (!string.IsNullOrEmpty(this.mID))
				{
					this.mIsSpecific = true;
					this.mIDHash = this.mID.GetHashCodeCustom();
					return;
				}
				this.mIsSpecific = false;
				this.mIDHash = 0;
			}
		}

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x06002080 RID: 8320 RVA: 0x000E6CBF File Offset: 0x000E4EBF
		// (set) Token: 0x06002081 RID: 8321 RVA: 0x000E6CC7 File Offset: 0x000E4EC7
		public Factions Faction
		{
			get
			{
				return this.mFaction;
			}
			set
			{
				this.mFaction = value;
			}
		}

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x06002082 RID: 8322 RVA: 0x000E6CD0 File Offset: 0x000E4ED0
		// (set) Token: 0x06002083 RID: 8323 RVA: 0x000E6CD8 File Offset: 0x000E4ED8
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaHash = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x06002084 RID: 8324 RVA: 0x000E6CF2 File Offset: 0x000E4EF2
		// (set) Token: 0x06002085 RID: 8325 RVA: 0x000E6CFA File Offset: 0x000E4EFA
		public OperationAction Action
		{
			get
			{
				return this.mAction;
			}
			set
			{
				this.mAction = value;
			}
		}

		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x06002086 RID: 8326 RVA: 0x000E6D03 File Offset: 0x000E4F03
		// (set) Token: 0x06002087 RID: 8327 RVA: 0x000E6D0B File Offset: 0x000E4F0B
		public string Type
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
				this.mTypeHash = this.mType.GetHashCodeCustom();
			}
		}

		// Token: 0x040022FA RID: 8954
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x040022FB RID: 8955
		public static readonly string PLAYER1 = "player1";

		// Token: 0x040022FC RID: 8956
		public static readonly string PLAYER2 = "player2";

		// Token: 0x040022FD RID: 8957
		public static readonly string PLAYER3 = "player3";

		// Token: 0x040022FE RID: 8958
		public static readonly string PLAYER4 = "player4";

		// Token: 0x040022FF RID: 8959
		protected string mID;

		// Token: 0x04002300 RID: 8960
		protected int mIDHash;

		// Token: 0x04002301 RID: 8961
		protected OperationAction mAction;

		// Token: 0x04002302 RID: 8962
		protected Factions mFaction;

		// Token: 0x04002303 RID: 8963
		protected string mType;

		// Token: 0x04002304 RID: 8964
		protected int mTypeHash;

		// Token: 0x04002305 RID: 8965
		protected string mArea;

		// Token: 0x04002306 RID: 8966
		protected int mAreaHash;

		// Token: 0x04002307 RID: 8967
		protected bool mIsSpecific;
	}
}
