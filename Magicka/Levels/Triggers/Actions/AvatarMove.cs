using System;
using System.Collections.Generic;
using System.Xml;
using Magicka.AI;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000285 RID: 645
	public class AvatarMove : Action
	{
		// Token: 0x06001314 RID: 4884 RVA: 0x00075C6B File Offset: 0x00073E6B
		public AvatarMove(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
			this.mNode = iNode;
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x00075C88 File Offset: 0x00073E88
		public override void Initialize()
		{
			base.Initialize();
			for (int i = 0; i < 4; i++)
			{
				this.mEvents[i] = null;
			}
			LevelModel levelModel = base.GameScene.LevelModel;
			foreach (object obj in this.mNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlComment) && xmlNode.Name.StartsWith("playerid", StringComparison.OrdinalIgnoreCase))
				{
					string iString = "";
					foreach (object obj2 in xmlNode.Attributes)
					{
						XmlAttribute xmlAttribute = (XmlAttribute)obj2;
						if (xmlAttribute.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
						{
							iString = xmlAttribute.Value.ToLowerInvariant();
						}
					}
					List<AIEvent> list = new List<AIEvent>();
					foreach (object obj3 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj3;
						if (!(xmlNode2 is XmlComment))
						{
							list.Add(new AIEvent(levelModel, xmlNode2));
						}
					}
					Entity byID = Entity.GetByID(iString.GetHashCodeCustom());
					if (byID != null)
					{
						for (int j = 0; j < Game.Instance.Players.Length; j++)
						{
							if (Game.Instance.Players[j].Avatar == byID)
							{
								this.mEvents[j] = list.ToArray();
								break;
							}
						}
					}
				}
			}
			foreach (object obj4 in this.mNode.ChildNodes)
			{
				XmlNode xmlNode3 = (XmlNode)obj4;
				if (!(xmlNode3 is XmlComment))
				{
					if (xmlNode3.Name.StartsWith("player", StringComparison.OrdinalIgnoreCase) && char.IsDigit(xmlNode3.Name[xmlNode3.Name.Length - 1]))
					{
						List<AIEvent> list2 = new List<AIEvent>();
						foreach (object obj5 in xmlNode3.ChildNodes)
						{
							XmlNode xmlNode4 = (XmlNode)obj5;
							if (!(xmlNode4 is XmlComment))
							{
								list2.Add(new AIEvent(levelModel, xmlNode4));
							}
						}
						int num = (int)(xmlNode3.Name[xmlNode3.Name.Length - 1] - '1');
						int num2 = 0;
						for (;;)
						{
							if (num > 3)
							{
								num = 0;
								num2++;
								if (num2 > 10)
								{
									break;
								}
							}
							if (this.mEvents[num] == null)
							{
								goto Block_31;
							}
							num++;
						}
						throw new Exception("Unable to map AvatarMove event " + list2.ToArray() + "!");
						Block_31:
						this.mEvents[num] = list2.ToArray();
					}
					else if (!xmlNode3.Name.StartsWith("playerid", StringComparison.OrdinalIgnoreCase))
					{
						throw new Exception("Invalid node \"" + xmlNode3.Name + "\" in AvatarMove! Expected \"Player1\"-\"Player4\".");
					}
				}
			}
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x00076048 File Offset: 0x00074248
		protected override void Execute()
		{
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing && players[i].Avatar != null && this.mEvents[i] != null)
				{
					players[i].Avatar.Events = this.mEvents[i];
				}
			}
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x000760A1 File Offset: 0x000742A1
		public override void QuickExecute()
		{
		}

		// Token: 0x040014D3 RID: 5331
		private XmlNode mNode;

		// Token: 0x040014D4 RID: 5332
		private AIEvent[][] mEvents = new AIEvent[4][];
	}
}
