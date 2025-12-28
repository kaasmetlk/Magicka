using System;
using System.Collections.Generic;
using System.Xml;
using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000238 RID: 568
	public class TriggerCheckpoint : Action
	{
		// Token: 0x06001168 RID: 4456 RVA: 0x0006BA8C File Offset: 0x00069C8C
		public TriggerCheckpoint(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
			this.mSpawnFairy = true;
			for (int i = 0; i < iNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = iNode.ChildNodes[i];
				if (!(xmlNode is XmlComment))
				{
					if (!xmlNode.Name.Equals("ignore", StringComparison.OrdinalIgnoreCase))
					{
						throw new Exception("Invalid node \"" + xmlNode.Name + "\" in \"TriggerCheckpoint\"!");
					}
					for (int j = 0; j < xmlNode.Attributes.Count; j++)
					{
						XmlAttribute xmlAttribute = xmlNode.Attributes[j];
						if (xmlAttribute.Name.Equals("trigger", StringComparison.OrdinalIgnoreCase))
						{
							string iString = xmlAttribute.Value.ToLowerInvariant();
							this.mIgnoreList.Add(iString.GetHashCodeCustom());
						}
					}
				}
			}
			this.mTextBox = new TextBox();
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x0006BBA8 File Offset: 0x00069DA8
		public override void Initialize()
		{
			base.Initialize();
			for (int i = 0; i < 4; i++)
			{
				base.GameScene.GetLocator(this.mSpawnPoints[i], out this.mSpawnPointMatrices[i]);
			}
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x0006BBE8 File Offset: 0x00069DE8
		protected override void Execute()
		{
			this.mTextBox.Initialize(base.GameScene.Scene, MagickaFont.Maiandra18, LanguageManager.Instance.GetString(this.CHECKPOINT), default(Vector2), new Vector2(0f, 1f), true, 0, 2f);
			DialogManager.Instance.AddTextBox(this.mTextBox);
			Player[] players = Game.Instance.Players;
			if (this.SpawnFairy && Game.Instance.PlayerCount == 1 && base.GameScene.PlayState.GameType != GameType.Versus)
			{
				foreach (Player player in players)
				{
					if (player != null && player.Playing)
					{
						player.Avatar.RevivalFairy.Initialize(base.GameScene.PlayState, true);
						break;
					}
				}
			}
			base.GameScene.PlayState.UpdateCheckPoint(this.mSpawnPointMatrices, this.mIgnoreList, this.mSaveToDisk);
			AudioManager.Instance.PlayCue(Banks.UI, "ui_checkpoint01".GetHashCodeCustom());
			for (int j = 0; j < players.Length; j++)
			{
				if (players[j].Playing)
				{
					if (players[j].Avatar != null && players[j].Avatar.Dead)
					{
						if (NetworkManager.Instance.State != NetworkState.Client)
						{
							Revive instance = Revive.GetInstance();
							instance.SetSpecificPlayer(players[j].ID);
							Matrix matrix;
							base.GameScene.GetLocator(this.mSpawnPoints[j], out matrix);
							instance.Execute(matrix.Translation, this.mScene.PlayState);
						}
					}
					else
					{
						players[j].Avatar.HitPoints = players[j].Avatar.MaxHitPoints;
						if (players[j].Avatar.Equipment[1].Item.Type == "staff_of_war".GetHashCodeCustom())
						{
							players[j].Avatar.HitPoints = players[j].Avatar.MaxHitPoints * 2f;
						}
					}
				}
			}
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x0006BDE2 File Offset: 0x00069FE2
		public override void QuickExecute()
		{
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x0600116C RID: 4460 RVA: 0x0006BDE4 File Offset: 0x00069FE4
		// (set) Token: 0x0600116D RID: 4461 RVA: 0x0006BDEC File Offset: 0x00069FEC
		public string SpawnPoint
		{
			get
			{
				return this.mSpawnPoint;
			}
			set
			{
				this.mSpawnPoint = value;
				for (int i = 0; i < 4; i++)
				{
					this.mSpawnPoints[i] = (this.mSpawnPoint + i).GetHashCodeCustom();
				}
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x0600116E RID: 4462 RVA: 0x0006BE2A File Offset: 0x0006A02A
		// (set) Token: 0x0600116F RID: 4463 RVA: 0x0006BE32 File Offset: 0x0006A032
		public bool SpawnFairy
		{
			get
			{
				return this.mSpawnFairy;
			}
			set
			{
				this.mSpawnFairy = value;
			}
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001170 RID: 4464 RVA: 0x0006BE3B File Offset: 0x0006A03B
		// (set) Token: 0x06001171 RID: 4465 RVA: 0x0006BE43 File Offset: 0x0006A043
		public bool SaveToDisk
		{
			get
			{
				return this.mSaveToDisk;
			}
			set
			{
				this.mSaveToDisk = value;
			}
		}

		// Token: 0x0400105A RID: 4186
		public readonly int CHECKPOINT = "#add_checkpoint".GetHashCodeCustom();

		// Token: 0x0400105B RID: 4187
		private bool mSaveToDisk = true;

		// Token: 0x0400105C RID: 4188
		private string mSpawnPoint;

		// Token: 0x0400105D RID: 4189
		private int[] mSpawnPoints = new int[4];

		// Token: 0x0400105E RID: 4190
		private Matrix[] mSpawnPointMatrices = new Matrix[4];

		// Token: 0x0400105F RID: 4191
		private List<int> mIgnoreList = new List<int>();

		// Token: 0x04001060 RID: 4192
		private TextBox mTextBox;

		// Token: 0x04001061 RID: 4193
		private bool mSpawnFairy;
	}
}
