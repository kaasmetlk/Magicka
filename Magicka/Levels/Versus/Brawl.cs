using System;
using System.Xml;
using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Levels.Triggers;
using Magicka.Misc;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Levels.Versus
{
	// Token: 0x020004F1 RID: 1265
	internal class Brawl : VersusRuleset
	{
		// Token: 0x06002560 RID: 9568 RVA: 0x0010FF30 File Offset: 0x0010E130
		public Brawl(GameScene iScene, XmlNode iNode, Brawl.Settings iSettings) : base(iScene, iNode)
		{
			this.mSettings = iSettings;
		}

		// Token: 0x06002561 RID: 9569 RVA: 0x0010FF65 File Offset: 0x0010E165
		public override void OnPlayerKill(Avatar atkAvatar, Avatar tarAvatar)
		{
		}

		// Token: 0x06002562 RID: 9570 RVA: 0x0010FF68 File Offset: 0x0010E168
		public override void OnPlayerDeath(Player iPlayer)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			if (this.mScene.PlayState.IsGameEnded)
			{
				return;
			}
			if ((iPlayer.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
			{
				short[] array = this.mTeamRespawns;
				int num = 0;
				array[num] -= 1;
				short[] array2 = this.mRespawns;
				int id = iPlayer.ID;
				array2[id] -= 1;
				this.mScoreUIs[0].SetScore((int)this.mTeamRespawns[0]);
				this.NetworkScore(iPlayer.ID);
				if (this.mTeamRespawns[0] >= 0)
				{
					this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
					return;
				}
				bool flag = true;
				for (int i = 0; i < this.mPlayers.Length; i++)
				{
					if (this.mPlayers[i].Playing && (this.mPlayers[i].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
					{
						if (this.mPlayers[i].Avatar != null && !this.mPlayers[i].Avatar.Dead)
						{
							flag = false;
						}
						else if (this.mRespawnTimers[this.mPlayers[i].ID] > 0f)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					base.EndGame();
					return;
				}
			}
			else if ((iPlayer.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
			{
				short[] array3 = this.mTeamRespawns;
				int num2 = 1;
				array3[num2] -= 1;
				short[] array4 = this.mRespawns;
				int id2 = iPlayer.ID;
				array4[id2] -= 1;
				this.mScoreUIs[1].SetScore((int)this.mTeamRespawns[1]);
				this.NetworkScore(iPlayer.ID);
				if (this.mTeamRespawns[1] >= 0)
				{
					this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
					return;
				}
				bool flag2 = true;
				for (int j = 0; j < this.mPlayers.Length; j++)
				{
					if (this.mPlayers[j].Playing && (this.mPlayers[j].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
					{
						if (this.mPlayers[j].Avatar != null && !this.mPlayers[j].Avatar.Dead)
						{
							flag2 = false;
						}
						else if (this.mRespawnTimers[this.mPlayers[j].ID] > 0f)
						{
							flag2 = false;
						}
					}
				}
				if (flag2)
				{
					base.EndGame();
					return;
				}
			}
			else
			{
				short[] array5 = this.mRespawns;
				int id3 = iPlayer.ID;
				array5[id3] -= 1;
				int num3 = this.mIDToScoreUILookUp[iPlayer.ID];
				if (num3 != -1)
				{
					this.mScoreUIs[num3].SetScore((int)this.mRespawns[iPlayer.ID]);
				}
				this.NetworkScore(iPlayer.ID);
				if (this.mRespawns[iPlayer.ID] >= 0)
				{
					this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
					return;
				}
				int num4 = 0;
				for (int k = 0; k < this.mPlayers.Length; k++)
				{
					if (this.mPlayers[k].Playing)
					{
						if (this.mPlayers[k].Avatar != null && !this.mPlayers[k].Avatar.Dead)
						{
							num4++;
						}
						else if (this.mRespawnTimers[this.mPlayers[k].ID] > 0f)
						{
							num4++;
						}
					}
				}
				if (num4 <= 1)
				{
					base.EndGame();
				}
			}
		}

		// Token: 0x06002563 RID: 9571 RVA: 0x001102E4 File Offset: 0x0010E4E4
		public override void OnEndGame()
		{
			Player player = ControlManager.Instance.MenuController.Player;
			if ((player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
			{
				if (!this.IsTeamDead(Factions.TEAM_RED))
				{
					Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
					return;
				}
			}
			else if ((player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
			{
				if (!this.IsTeamDead(Factions.TEAM_BLUE))
				{
					Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
					return;
				}
			}
			else if ((player.Avatar != null && !player.Avatar.Dead) || this.mRespawnTimers[player.ID] > 0f)
			{
				Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
			}
		}

		// Token: 0x06002564 RID: 9572 RVA: 0x0011038C File Offset: 0x0010E58C
		private bool IsTeamDead(Factions iFaction)
		{
			bool result = true;
			for (int i = 0; i < this.mPlayers.Length; i++)
			{
				if (this.mPlayers[i].Playing && (this.mPlayers[i].Team & iFaction) == iFaction)
				{
					if (this.mPlayers[i].Avatar != null && !this.mPlayers[i].Avatar.Dead)
					{
						result = false;
					}
					else if (this.mRespawnTimers[this.mPlayers[i].ID] > 0f)
					{
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x06002565 RID: 9573 RVA: 0x00110414 File Offset: 0x0010E614
		public override void Initialize()
		{
			base.Initialize();
			this.mMaxRespawns = this.mSettings.Lives;
			this.mLuggageTimer = (float)this.mSettings.LuggageInterval;
			this.mLuggageTime = this.mLuggageTimer;
			this.mRespawnTime = 5f;
			this.mTeams = this.mSettings.TeamsEnabled;
			this.mTimeLimit = (float)this.mSettings.TimeLimit * 60f;
			this.mTimeLimitTimer = this.mTimeLimit;
			this.mTimeLimitTarget = this.mTimeLimit;
			if (this.mTeams)
			{
				this.mScoreUIs.Add(new VersusRuleset.Score(true));
				this.mScoreUIs.Add(new VersusRuleset.Score(false));
				for (int i = 0; i < this.mPlayers.Length; i++)
				{
					if (this.mPlayers[i].Playing)
					{
						Texture2D portrait = this.mPlayers[i].Gamer.Avatar.Portrait;
						this.mPlayers[i].Avatar.Faction &= ~Factions.FRIENDLY;
						if ((this.mPlayers[i].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
						{
							this.mIDToScoreUILookUp[i] = 0;
							this.mScoreUIs[0].AddPlayer(this.mPlayers[i].GamerTag, this.mPlayers[i].ID, portrait, Defines.PLAYERCOLORS[(int)this.mPlayers[i].Color]);
						}
						else
						{
							this.mIDToScoreUILookUp[i] = 1;
							this.mScoreUIs[1].AddPlayer(this.mPlayers[i].GamerTag, this.mPlayers[i].ID, portrait, Defines.PLAYERCOLORS[(int)this.mPlayers[i].Color]);
						}
					}
				}
			}
			else
			{
				int j = 0;
				int num = 0;
				while (j < this.mPlayers.Length)
				{
					if (this.mPlayers[j].Playing)
					{
						this.mPlayers[j].Avatar.Faction &= ~Factions.FRIENDLY;
						Texture2D portrait2 = this.mPlayers[j].Gamer.Avatar.Portrait;
						VersusRuleset.Score score = new VersusRuleset.Score(num % 2 == 0);
						score.AddPlayer(this.mPlayers[j].GamerTag, this.mPlayers[j].ID, portrait2, Defines.PLAYERCOLORS[(int)this.mPlayers[j].Color]);
						this.mIDToScoreUILookUp[j] = this.mScoreUIs.Count;
						this.mScoreUIs.Add(score);
						num++;
					}
					j++;
				}
			}
			for (int k = 0; k < this.mRespawns.Length; k++)
			{
				this.mRespawns[k] = (short)((byte)this.mMaxRespawns);
			}
			for (int l = 0; l < this.mTeamRespawns.Length; l++)
			{
				this.mTeamRespawns[l] = (short)((byte)this.mMaxRespawns);
			}
			for (int m = 0; m < this.mScoreUIs.Count; m++)
			{
				this.mScoreUIs[m].HideNegativeScore = true;
				this.mScoreUIs[m].SetScore(this.mMaxRespawns);
			}
		}

		// Token: 0x06002566 RID: 9574 RVA: 0x00110760 File Offset: 0x0010E960
		public override void DeInitialize()
		{
			if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
			{
				this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x06002567 RID: 9575 RVA: 0x00110784 File Offset: 0x0010E984
		public override void Update(float iDeltaTime, DataChannel iDataChannel)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			for (int i = 0; i < this.mRespawnTimers.Length; i++)
			{
				float num = Math.Max(this.mRespawnTimers[i] - iDeltaTime, 0f);
				if (this.mRespawnTimers[i] > 0f && num <= 0f && this.mPlayers[i].Playing)
				{
					base.SetupPlayer(i, base.GetTeamArea(this.mPlayers[i].Team));
				}
				this.mRespawnTimers[i] = num;
			}
			for (int j = 0; j < this.mRespawnTimers.Length; j++)
			{
				int num2;
				if (this.mIDToScoreUILookUp.TryGetValue(j, out num2) && num2 != -1)
				{
					this.mScoreUIs[num2].SetTimer(j, (int)this.mRespawnTimers[j]);
				}
			}
			if (this.mLuggageTime > 0f)
			{
				if (this.mLuggageTimer <= 0f)
				{
					TriggerArea triggerArea = this.mScene.PlayState.Level.CurrentScene.GetTriggerArea(TriggerArea.ANYID);
					int num3 = VersusRuleset.RANDOM.Next(0, 3);
					if (triggerArea.GetCount(this.mItemTemplate.ID) == 0 && triggerArea.GetCount(this.mMagickTemplate.ID) == 0)
					{
						base.SpawnLuggage(num3 != 0);
					}
					this.mLuggageTimer = this.mLuggageTime;
				}
				this.mLuggageTimer -= iDeltaTime;
			}
			if (this.mTimeLimit > 0f)
			{
				if (!this.mScene.PlayState.IsGameEnded)
				{
					this.mTimeLimitTimer -= iDeltaTime;
					if (this.mTimeLimitTimer <= 0f)
					{
						base.EndGame();
						this.mTimeLimitTimer = this.mTimeLimit;
					}
				}
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					if (this.mTimeLimitNetworkUpdate > 1f)
					{
						RulesetMessage rulesetMessage = default(RulesetMessage);
						rulesetMessage.Type = this.RulesetType;
						rulesetMessage.Byte01 = 0;
						rulesetMessage.Float01 = this.mTimeLimitTimer;
						NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage);
						this.mTimeLimitNetworkUpdate = 0f;
					}
					this.mTimeLimitNetworkUpdate += iDeltaTime;
				}
			}
			VersusRuleset.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.DrawTime = (this.mTimeLimit > 0f);
			renderData.TimeLimit = this.mTimeLimit;
			renderData.Time = this.mTimeLimitTimer;
			if (this.mTimeLimitTimer <= 10f)
			{
				if (this.mGameClockCue == null && this.mTimeLimitTimer > 0f)
				{
					this.mGameClockCue = AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_CLOCK);
				}
				renderData.SetTimeText((int)this.mTimeLimitTimer);
			}
			else
			{
				renderData.SetTimeText(0);
			}
			base.Update(iDeltaTime, iDataChannel);
		}

		// Token: 0x06002568 RID: 9576 RVA: 0x00110A38 File Offset: 0x0010EC38
		public override void LocalUpdate(float iDeltaTime, DataChannel iDataChannel)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			for (int i = 0; i < this.mRespawnTimers.Length; i++)
			{
				this.mRespawnTimers[i] = Math.Max(this.mRespawnTimers[i] - iDeltaTime, 0f);
				for (int j = 0; j < this.mScoreUIs.Count; j++)
				{
					this.mScoreUIs[j].SetTimer(i, (int)this.mRespawnTimers[i]);
				}
			}
			VersusRuleset.RenderData renderData = this.mRenderData[(int)iDataChannel];
			this.mTimeLimitTimer += (this.mTimeLimitTarget - this.mTimeLimitTimer) * iDeltaTime;
			renderData.DrawTime = (this.mTimeLimit > 0f);
			renderData.TimeLimit = this.mTimeLimit;
			renderData.Time = this.mTimeLimitTimer;
			if (this.mTimeLimitTimer <= 10f)
			{
				if (this.mGameClockCue == null && this.mTimeLimitTimer > 0f)
				{
					this.mGameClockCue = AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_CLOCK);
				}
				renderData.SetTimeText((int)this.mTimeLimitTimer);
			}
			else
			{
				renderData.SetTimeText(0);
			}
			base.LocalUpdate(iDeltaTime, iDataChannel);
		}

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x06002569 RID: 9577 RVA: 0x00110B54 File Offset: 0x0010ED54
		public override Rulesets RulesetType
		{
			get
			{
				return Rulesets.Brawl;
			}
		}

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x0600256A RID: 9578 RVA: 0x00110B57 File Offset: 0x0010ED57
		public override bool DropMagicks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600256B RID: 9579 RVA: 0x00110B5C File Offset: 0x0010ED5C
		public override bool CanRevive(Player iReviver, Player iRevivee)
		{
			if (!this.mTeams)
			{
				return false;
			}
			if ((iReviver.Team & iRevivee.Team) == Factions.NONE)
			{
				return false;
			}
			if ((iReviver.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
			{
				return this.mRespawns[0] > 0;
			}
			return this.mRespawns[1] > 0;
		}

		// Token: 0x0600256C RID: 9580 RVA: 0x00110BB0 File Offset: 0x0010EDB0
		protected unsafe void NetworkScore(int iID)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				RulesetMessage rulesetMessage = default(RulesetMessage);
				rulesetMessage.Type = this.RulesetType;
				rulesetMessage.Byte01 = 4;
				rulesetMessage.Byte02 = (byte)iID;
				*(&rulesetMessage.Scores.FixedElementField) = this.mRespawns[0];
				(&rulesetMessage.Scores.FixedElementField)[1] = this.mRespawns[1];
				(&rulesetMessage.Scores.FixedElementField)[2] = this.mRespawns[2];
				(&rulesetMessage.Scores.FixedElementField)[3] = this.mRespawns[3];
				(&rulesetMessage.Scores.FixedElementField)[4] = this.mTeamRespawns[0];
				(&rulesetMessage.Scores.FixedElementField)[5] = this.mTeamRespawns[1];
				rulesetMessage.NrOfShortItems = 6;
				NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage);
			}
		}

		// Token: 0x0600256D RID: 9581 RVA: 0x00110CA0 File Offset: 0x0010EEA0
		public unsafe override void NetworkUpdate(ref RulesetMessage iMsg)
		{
			if (iMsg.Byte01 == 0)
			{
				float num = NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
				this.mTimeLimitTarget = iMsg.Float01 - num;
				return;
			}
			if (iMsg.Byte01 == 4)
			{
				fixed (short* ptr = &iMsg.Scores.FixedElementField)
				{
					this.mRespawns[0] = *ptr;
					this.mRespawns[1] = ptr[1];
					this.mRespawns[2] = ptr[2];
					this.mRespawns[3] = ptr[3];
					this.mTeamRespawns[0] = ptr[4];
					this.mTeamRespawns[1] = ptr[5];
				}
				if ((this.mPlayers[(int)iMsg.Byte02].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
				{
					if (this.mTeamRespawns[0] > 0)
					{
						this.mRespawnTimers[(int)iMsg.Byte02] = this.mRespawnTime;
						this.mScoreUIs[0].SetScore((int)this.mTeamRespawns[0]);
						return;
					}
				}
				else if ((this.mPlayers[(int)iMsg.Byte02].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
				{
					if (this.mTeamRespawns[1] > 0)
					{
						this.mRespawnTimers[(int)iMsg.Byte02] = this.mRespawnTime;
						this.mScoreUIs[1].SetScore((int)this.mTeamRespawns[1]);
						return;
					}
				}
				else
				{
					if (this.mRespawns[(int)iMsg.Byte02] >= 0)
					{
						this.mRespawnTimers[(int)iMsg.Byte02] = this.mRespawnTime;
					}
					int num2 = this.mIDToScoreUILookUp[(int)iMsg.Byte02];
					if (num2 != -1)
					{
						this.mScoreUIs[num2].SetScore((int)this.mRespawns[(int)iMsg.Byte02]);
						return;
					}
				}
			}
			else
			{
				base.NetworkUpdate(ref iMsg);
			}
		}

		// Token: 0x0600256E RID: 9582 RVA: 0x00110E5B File Offset: 0x0010F05B
		internal override short[] GetScores()
		{
			return this.mRespawns;
		}

		// Token: 0x0600256F RID: 9583 RVA: 0x00110E63 File Offset: 0x0010F063
		internal override short[] GetTeamScores()
		{
			return this.mTeamRespawns;
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x06002570 RID: 9584 RVA: 0x00110E6B File Offset: 0x0010F06B
		internal override bool Teams
		{
			get
			{
				return this.mTeams;
			}
		}

		// Token: 0x040028DD RID: 10461
		private short[] mTeamRespawns = new short[2];

		// Token: 0x040028DE RID: 10462
		private short[] mRespawns = new short[4];

		// Token: 0x040028DF RID: 10463
		private float[] mRespawnTimers = new float[4];

		// Token: 0x040028E0 RID: 10464
		private float mLuggageTimer;

		// Token: 0x040028E1 RID: 10465
		private float mRespawnTime;

		// Token: 0x040028E2 RID: 10466
		private float mLuggageTime;

		// Token: 0x040028E3 RID: 10467
		private bool mTeams;

		// Token: 0x040028E4 RID: 10468
		private float mTimeLimitTimer;

		// Token: 0x040028E5 RID: 10469
		private float mTimeLimitNetworkUpdate;

		// Token: 0x040028E6 RID: 10470
		private float mTimeLimitTarget;

		// Token: 0x040028E7 RID: 10471
		private float mTimeLimit;

		// Token: 0x040028E8 RID: 10472
		private int mMaxRespawns;

		// Token: 0x040028E9 RID: 10473
		private Brawl.Settings mSettings;

		// Token: 0x020004F2 RID: 1266
		internal new class Settings : VersusRuleset.Settings
		{
			// Token: 0x06002571 RID: 9585 RVA: 0x00110EB8 File Offset: 0x0010F0B8
			public Settings()
			{
				this.mTimeLimit = base.AddOption<int>(this.LOC_TIME_LIMIT, this.LOC_TT_TIME, new int[]
				{
					0,
					5,
					10,
					30,
					50
				}, new int?[]
				{
					new int?(this.LOC_UNLIMITED),
					null,
					null,
					null,
					null
				});
				this.mTimeLimit.SelectedIndex = 1;
				this.mLives = base.AddOption<int>(this.LOC_LIVES, this.LOC_TT_LIVES, new int[]
				{
					1,
					5,
					10,
					20,
					50
				}, null);
				this.mLives.SelectedIndex = 1;
				this.mTeams = base.AddOption<bool>(this.LOC_TEAMS, this.LOC_TT_TEAMS, new bool[]
				{
					default(bool),
					true
				}, new int?[]
				{
					new int?(this.LOC_NO),
					new int?(this.LOC_YES)
				});
				this.mTeams.SelectedIndex = 0;
				this.mLuggageInterval = base.AddOption<int>(this.LOC_LUGGAGE_INTERVAL, this.LOC_TT_LUGGAGE, new int[]
				{
					15,
					30,
					90,
					0
				}, new int?[]
				{
					new int?(this.LOC_HIGH),
					new int?(this.LOC_MEDIUM),
					new int?(this.LOC_LOW),
					new int?(this.LOC_OFF)
				});
				this.mLuggageInterval.SelectedIndex = 1;
			}

			// Token: 0x170008AF RID: 2223
			// (get) Token: 0x06002572 RID: 9586 RVA: 0x001110B6 File Offset: 0x0010F2B6
			public int TimeLimit
			{
				get
				{
					return this.mTimeLimit.SelectedValue;
				}
			}

			// Token: 0x170008B0 RID: 2224
			// (get) Token: 0x06002573 RID: 9587 RVA: 0x001110C3 File Offset: 0x0010F2C3
			public int Lives
			{
				get
				{
					return this.mLives.SelectedValue;
				}
			}

			// Token: 0x170008B1 RID: 2225
			// (get) Token: 0x06002574 RID: 9588 RVA: 0x001110D0 File Offset: 0x0010F2D0
			public override bool TeamsEnabled
			{
				get
				{
					return this.mTeams.SelectedValue;
				}
			}

			// Token: 0x170008B2 RID: 2226
			// (get) Token: 0x06002575 RID: 9589 RVA: 0x001110DD File Offset: 0x0010F2DD
			public int LuggageInterval
			{
				get
				{
					return this.mLuggageInterval.SelectedValue;
				}
			}

			// Token: 0x040028EA RID: 10474
			private DropDownBox<int> mTimeLimit;

			// Token: 0x040028EB RID: 10475
			private DropDownBox<int> mLives;

			// Token: 0x040028EC RID: 10476
			private DropDownBox<bool> mTeams;

			// Token: 0x040028ED RID: 10477
			private DropDownBox<int> mLuggageInterval;
		}
	}
}
