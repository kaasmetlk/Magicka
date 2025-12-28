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
	// Token: 0x02000551 RID: 1361
	internal class DeathMatch : VersusRuleset
	{
		// Token: 0x0600286E RID: 10350 RVA: 0x0013C969 File Offset: 0x0013AB69
		public DeathMatch(GameScene iScene, XmlNode iNode, DeathMatch.Settings iSettings) : base(iScene, iNode)
		{
			this.mSettings = iSettings;
		}

		// Token: 0x0600286F RID: 10351 RVA: 0x0013C9A0 File Offset: 0x0013ABA0
		public override void OnPlayerKill(Avatar iAttacker, Avatar iTarget)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			if (this.mScene.PlayState.IsGameEnded)
			{
				return;
			}
			if (iAttacker == iTarget)
			{
				short[] array = this.mScores;
				int id = iAttacker.Player.ID;
				array[id] -= 1;
			}
			else
			{
				short[] array2 = this.mScores;
				int id2 = iAttacker.Player.ID;
				array2[id2] += 1;
			}
			if ((iAttacker.Player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
			{
				if (iTarget != null && (iTarget.Player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
				{
					short[] array3 = this.mTeamScores;
					int num = 0;
					array3[num] -= 1;
				}
				else
				{
					short[] array4 = this.mTeamScores;
					int num2 = 0;
					array4[num2] += 1;
				}
				this.mScoreUIs[0].SetScore((int)this.mTeamScores[0]);
				if ((int)this.mTeamScores[0] >= this.mScoreLimit)
				{
					base.EndGame();
				}
			}
			else if ((iAttacker.Player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
			{
				if (iTarget != null && (iTarget.Player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
				{
					short[] array5 = this.mTeamScores;
					int num3 = 1;
					array5[num3] -= 1;
				}
				else
				{
					short[] array6 = this.mTeamScores;
					int num4 = 1;
					array6[num4] += 1;
				}
				this.mScoreUIs[1].SetScore((int)this.mTeamScores[1]);
				if ((int)this.mTeamScores[1] >= this.mScoreLimit)
				{
					base.EndGame();
				}
			}
			else
			{
				int num5 = this.mIDToScoreUILookUp[iAttacker.Player.ID];
				if (num5 != -1)
				{
					this.mScoreUIs[num5].SetScore((int)this.mScores[iAttacker.Player.ID]);
				}
				if ((int)this.mScores[iAttacker.Player.ID] >= this.mScoreLimit)
				{
					base.EndGame();
				}
			}
			this.NetworkScore(iAttacker.Player.ID);
		}

		// Token: 0x06002870 RID: 10352 RVA: 0x0013CBCC File Offset: 0x0013ADCC
		public override void OnEndGame()
		{
			Player player = ControlManager.Instance.MenuController.Player;
			if ((player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
			{
				if ((int)this.mTeamScores[0] >= this.mScoreLimit)
				{
					Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
					return;
				}
			}
			else if ((player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
			{
				if ((int)this.mTeamScores[1] >= this.mScoreLimit)
				{
					Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
					return;
				}
			}
			else if ((int)this.mScores[player.ID] >= this.mScoreLimit)
			{
				Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
			}
		}

		// Token: 0x06002871 RID: 10353 RVA: 0x0013CC64 File Offset: 0x0013AE64
		public override void OnPlayerDeath(Player iPlayer)
		{
			if ((this.mScene.PlayState.IsGameEnded && this.mRespawnTimers[iPlayer.ID] <= 0f) || this.mRespawnTimers[iPlayer.ID] > 0f)
			{
				return;
			}
			this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
		}

		// Token: 0x06002872 RID: 10354 RVA: 0x0013CCC0 File Offset: 0x0013AEC0
		public override void Initialize()
		{
			base.Initialize();
			this.mTimeLimit = (float)this.mSettings.TimeLimit * 60f;
			this.mTimeLimitTimer = this.mTimeLimit;
			this.mTimeLimitTarget = this.mTimeLimit;
			this.mScoreLimit = this.mSettings.ScoreLimit;
			this.mRespawnTime = 5f;
			this.mLuggageTime = (float)this.mSettings.LuggageInterval;
			this.mLuggageTimer = this.mLuggageTime;
			this.mTeams = this.mSettings.TeamsEnabled;
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
			for (int k = 0; k < this.mScores.Length; k++)
			{
				this.mScores[k] = 0;
			}
			for (int l = 0; l < this.mTeamScores.Length; l++)
			{
				this.mTeamScores[l] = 0;
			}
			for (int m = 0; m < this.mRespawnTimers.Length; m++)
			{
				this.mRespawnTimers[m] = 0f;
			}
			this.mNetworkInitialized = false;
		}

		// Token: 0x06002873 RID: 10355 RVA: 0x0013CFE7 File Offset: 0x0013B1E7
		public override void DeInitialize()
		{
			if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
			{
				this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x06002874 RID: 10356 RVA: 0x0013D00C File Offset: 0x0013B20C
		public override void Update(float iDeltaTime, DataChannel iDataChannel)
		{
			if (!this.mNetworkInitialized)
			{
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					this.mTemporarySpawns.Clear();
					this.mTemporarySpawns.AddRange(this.mAreas_All);
					int num = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
					for (int i = 0; i < this.mScores.Length; i++)
					{
						if (this.mPlayers[i].Playing)
						{
							int index = num % this.mTemporarySpawns.Count;
							base.SetupPlayer(i, this.mTemporarySpawns[index]);
							this.mTemporarySpawns.RemoveAt(index);
						}
					}
				}
				this.mNetworkInitialized = true;
			}
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			for (int j = 0; j < this.mRespawnTimers.Length; j++)
			{
				float num2 = Math.Max(this.mRespawnTimers[j] - iDeltaTime, 0f);
				if (this.mRespawnTimers[j] > 0f && num2 <= 0f && this.mPlayers[j].Playing)
				{
					base.SetupPlayer(j, base.GetTeamArea(this.mPlayers[j].Team));
				}
				this.mRespawnTimers[j] = num2;
			}
			for (int k = 0; k < this.mRespawnTimers.Length; k++)
			{
				int num3;
				if (this.mIDToScoreUILookUp.TryGetValue(k, out num3) && num3 != -1)
				{
					this.mScoreUIs[num3].SetTimer(k, (int)this.mRespawnTimers[k]);
				}
			}
			if (this.mLuggageTime > 0f)
			{
				if (this.mLuggageTimer <= 0f)
				{
					TriggerArea triggerArea = this.mScene.PlayState.Level.CurrentScene.GetTriggerArea(TriggerArea.ANYID);
					int num4 = VersusRuleset.RANDOM.Next(0, 3);
					if (triggerArea.GetCount(this.mItemTemplate.ID) == 0 && triggerArea.GetCount(this.mMagickTemplate.ID) == 0)
					{
						base.SpawnLuggage(num4 != 0);
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
						this.mTimeLimitTimer = 0f;
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

		// Token: 0x06002875 RID: 10357 RVA: 0x0013D36C File Offset: 0x0013B56C
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

		// Token: 0x06002876 RID: 10358 RVA: 0x0013D488 File Offset: 0x0013B688
		protected unsafe void NetworkScore(int iID)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				RulesetMessage rulesetMessage = default(RulesetMessage);
				rulesetMessage.Type = this.RulesetType;
				rulesetMessage.Byte01 = 4;
				rulesetMessage.Byte02 = (byte)iID;
				*(&rulesetMessage.Scores.FixedElementField) = this.mScores[0];
				(&rulesetMessage.Scores.FixedElementField)[1] = this.mScores[1];
				(&rulesetMessage.Scores.FixedElementField)[2] = this.mScores[2];
				(&rulesetMessage.Scores.FixedElementField)[3] = this.mScores[3];
				(&rulesetMessage.Scores.FixedElementField)[4] = this.mTeamScores[0];
				(&rulesetMessage.Scores.FixedElementField)[5] = this.mTeamScores[1];
				rulesetMessage.NrOfShortItems = 6;
				NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage);
			}
		}

		// Token: 0x06002877 RID: 10359 RVA: 0x0013D578 File Offset: 0x0013B778
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
					this.mScores[0] = *ptr;
					this.mScores[1] = ptr[1];
					this.mScores[2] = ptr[2];
					this.mScores[3] = ptr[3];
					this.mTeamScores[0] = ptr[4];
					this.mTeamScores[1] = ptr[5];
				}
				if ((this.mPlayers[(int)iMsg.Byte02].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
				{
					this.mScoreUIs[0].SetScore((int)this.mTeamScores[0]);
					if ((int)this.mTeamScores[0] >= this.mScoreLimit)
					{
						base.EndGame();
						return;
					}
				}
				else if ((this.mPlayers[(int)iMsg.Byte02].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
				{
					this.mScoreUIs[1].SetScore((int)this.mScores[1]);
					if ((int)this.mTeamScores[1] >= this.mScoreLimit)
					{
						base.EndGame();
						return;
					}
				}
				else
				{
					int num2 = this.mIDToScoreUILookUp[(int)iMsg.Byte02];
					if (num2 != -1)
					{
						this.mScoreUIs[num2].SetScore((int)this.mScores[(int)iMsg.Byte02]);
					}
					if ((int)this.mScores[(int)iMsg.Byte02] >= this.mScoreLimit)
					{
						base.EndGame();
						return;
					}
				}
			}
			else
			{
				base.NetworkUpdate(ref iMsg);
			}
		}

		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x06002878 RID: 10360 RVA: 0x0013D718 File Offset: 0x0013B918
		public override Rulesets RulesetType
		{
			get
			{
				return Rulesets.DeathMatch;
			}
		}

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x06002879 RID: 10361 RVA: 0x0013D71B File Offset: 0x0013B91B
		public override bool DropMagicks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600287A RID: 10362 RVA: 0x0013D71E File Offset: 0x0013B91E
		public override bool CanRevive(Player iReviver, Player iRevivee)
		{
			return this.mTeams && (iReviver.Team & iRevivee.Team) != Factions.NONE;
		}

		// Token: 0x0600287B RID: 10363 RVA: 0x0013D73C File Offset: 0x0013B93C
		internal override short[] GetScores()
		{
			return this.mScores;
		}

		// Token: 0x0600287C RID: 10364 RVA: 0x0013D744 File Offset: 0x0013B944
		internal override short[] GetTeamScores()
		{
			return this.mTeamScores;
		}

		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x0600287D RID: 10365 RVA: 0x0013D74C File Offset: 0x0013B94C
		internal override bool Teams
		{
			get
			{
				return this.mTeams;
			}
		}

		// Token: 0x04002BE6 RID: 11238
		private short[] mTeamScores = new short[2];

		// Token: 0x04002BE7 RID: 11239
		private short[] mScores = new short[4];

		// Token: 0x04002BE8 RID: 11240
		private float[] mRespawnTimers = new float[4];

		// Token: 0x04002BE9 RID: 11241
		private float mTimeLimitTimer;

		// Token: 0x04002BEA RID: 11242
		private float mLuggageTimer;

		// Token: 0x04002BEB RID: 11243
		private float mTimeLimitNetworkUpdate;

		// Token: 0x04002BEC RID: 11244
		private float mTimeLimitTarget;

		// Token: 0x04002BED RID: 11245
		private float mTimeLimit;

		// Token: 0x04002BEE RID: 11246
		private int mScoreLimit;

		// Token: 0x04002BEF RID: 11247
		private float mRespawnTime;

		// Token: 0x04002BF0 RID: 11248
		private float mLuggageTime;

		// Token: 0x04002BF1 RID: 11249
		private bool mTeams;

		// Token: 0x04002BF2 RID: 11250
		private bool mNetworkInitialized;

		// Token: 0x04002BF3 RID: 11251
		private DeathMatch.Settings mSettings;

		// Token: 0x02000552 RID: 1362
		internal new class Settings : VersusRuleset.Settings
		{
			// Token: 0x0600287E RID: 10366 RVA: 0x0013D798 File Offset: 0x0013B998
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
				this.mScoreLimit = base.AddOption<int>(this.LOC_SCORE_LIMIT, this.LOC_TT_SCORE, new int[]
				{
					int.MaxValue,
					5,
					10,
					20,
					50
				}, new int?[]
				{
					new int?(this.LOC_UNLIMITED),
					null,
					null,
					null,
					null
				});
				this.mScoreLimit.SelectedIndex = 2;
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

			// Token: 0x1700097B RID: 2427
			// (get) Token: 0x0600287F RID: 10367 RVA: 0x0013DA13 File Offset: 0x0013BC13
			public int TimeLimit
			{
				get
				{
					return this.mTimeLimit.SelectedValue;
				}
			}

			// Token: 0x1700097C RID: 2428
			// (get) Token: 0x06002880 RID: 10368 RVA: 0x0013DA20 File Offset: 0x0013BC20
			public int ScoreLimit
			{
				get
				{
					return this.mScoreLimit.SelectedValue;
				}
			}

			// Token: 0x1700097D RID: 2429
			// (get) Token: 0x06002881 RID: 10369 RVA: 0x0013DA2D File Offset: 0x0013BC2D
			public override bool TeamsEnabled
			{
				get
				{
					return this.mTeams.SelectedValue;
				}
			}

			// Token: 0x1700097E RID: 2430
			// (get) Token: 0x06002882 RID: 10370 RVA: 0x0013DA3A File Offset: 0x0013BC3A
			public int LuggageInterval
			{
				get
				{
					return this.mLuggageInterval.SelectedValue;
				}
			}

			// Token: 0x04002BF4 RID: 11252
			private DropDownBox<int> mTimeLimit;

			// Token: 0x04002BF5 RID: 11253
			private DropDownBox<int> mScoreLimit;

			// Token: 0x04002BF6 RID: 11254
			private DropDownBox<bool> mTeams;

			// Token: 0x04002BF7 RID: 11255
			private DropDownBox<int> mLuggageInterval;
		}
	}
}
