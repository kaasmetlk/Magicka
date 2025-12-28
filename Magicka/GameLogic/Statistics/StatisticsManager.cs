using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Levels.Versus;
using Magicka.Network;
using SteamWrapper;

namespace Magicka.GameLogic.Statistics
{
	// Token: 0x0200033E RID: 830
	public class StatisticsManager
	{
		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x06001947 RID: 6471 RVA: 0x000A9FF0 File Offset: 0x000A81F0
		public static StatisticsManager Instance
		{
			get
			{
				if (StatisticsManager.mSingelton == null)
				{
					lock (StatisticsManager.mSingeltonLock)
					{
						if (StatisticsManager.mSingelton == null)
						{
							StatisticsManager.mSingelton = new StatisticsManager();
						}
					}
				}
				return StatisticsManager.mSingelton;
			}
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x000AA044 File Offset: 0x000A8244
		private StatisticsManager()
		{
			this.mScores = new StatisticsManager.ScoreValues[4];
			this.mPlayerMultiKillCounter = new MultiKillCounter[4];
			for (int i = 0; i < 4; i++)
			{
				this.mPlayerMultiKillCounter[i] = new MultiKillCounter(100);
			}
			int num = LevelManager.Instance.Challenges.Length;
			this.mLeaderboards = new List<List<LeaderBoardData>>(num);
			for (int j = 0; j < num; j++)
			{
				this.mLeaderboards.Add(new List<LeaderBoardData>(8));
			}
			this.mSteamLeaderboards = new List<ulong>(num);
			LevelNode[] challenges = LevelManager.Instance.Challenges;
			for (int k = 0; k < num; k++)
			{
				SteamUserStats.FindLeaderboard(challenges[k].FileName, new Action<LeaderboardFindResult>(this.OnlineFindResult));
			}
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x000AA104 File Offset: 0x000A8304
		public void AddLocalEntry(int iChallengeIndex, LeaderBoardData iData)
		{
			this.mLeaderboards[iChallengeIndex].Add(iData);
			this.mLeaderboards[iChallengeIndex].Sort(LeaderBoardData.ScoreBeforeDataComparer);
			if (this.mLeaderboards[iChallengeIndex].Count > 8)
			{
				this.mLeaderboards[iChallengeIndex].RemoveAt(8);
			}
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x000AA15F File Offset: 0x000A835F
		public List<LeaderBoardData> Leaderboard(int iChallengeIndex)
		{
			return this.mLeaderboards[iChallengeIndex];
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x000AA170 File Offset: 0x000A8370
		public void AddOnlineEntry(int iChallengeIndex, LeaderBoardData iData)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (this.mSteamLeaderboards.Count > iChallengeIndex)
				{
					SteamUserStats.UploadLeaderboardScore(this.mSteamLeaderboards[iChallengeIndex], LeaderboardUploadScoreMethod.KeepBest, iData.Score, new int[]
					{
						iData.Data1
					}, new Action<LeaderboardScoreUploaded>(this.OnlineScoreUploaded));
				}
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					LeaderboardMessage leaderboardMessage = default(LeaderboardMessage);
					leaderboardMessage.SteamLeaderboard = this.mSteamLeaderboards[iChallengeIndex];
					leaderboardMessage.ScoreMethod = LeaderboardUploadScoreMethod.KeepBest;
					leaderboardMessage.Score = iData.Score;
					leaderboardMessage.Data = iData.Data1;
					NetworkManager.Instance.Interface.SendMessage<LeaderboardMessage>(ref leaderboardMessage);
				}
			}
		}

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x0600194C RID: 6476 RVA: 0x000AA22F File Offset: 0x000A842F
		public List<ulong> SteamLeaderboards
		{
			get
			{
				return this.mSteamLeaderboards;
			}
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x000AA237 File Offset: 0x000A8437
		private void OnlineFindResult(LeaderboardFindResult iResult)
		{
			if (iResult.mLeaderboardFound)
			{
				this.mSteamLeaderboards.Add(iResult.mSteamLeaderboard);
				return;
			}
			this.mSteamLeaderboards.Add(0UL);
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x000AA262 File Offset: 0x000A8462
		internal void OnlineScoreUploaded(LeaderboardScoreUploaded iUploaded)
		{
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x000AA264 File Offset: 0x000A8464
		public void SurvivalReset()
		{
			this.mTotalScore = 0;
			this.mTotalPlayerDamage = 0f;
			for (int i = 0; i < this.mScores.Length; i++)
			{
				this.mScores[i] = default(StatisticsManager.ScoreValues);
			}
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x000AA2A8 File Offset: 0x000A84A8
		private void InternalDamageEvent(PlayState iPlayState, IDamageable iAttacker, IDamageable iTarget, double iTimeStamp, Damage iDamage, DamageResult iResult, int iMultiplier)
		{
			if (iResult == DamageResult.None | (iResult & DamageResult.Deflected) == DamageResult.Deflected)
			{
				return;
			}
			if (iAttacker is NonPlayerCharacter)
			{
				NonPlayerCharacter nonPlayerCharacter = iAttacker as NonPlayerCharacter;
				if (nonPlayerCharacter.IsCharmed)
				{
					iAttacker = (nonPlayerCharacter.CharmOwner as IDamageable);
				}
				else if (nonPlayerCharacter.IsSummoned)
				{
					iAttacker = nonPlayerCharacter.Master;
				}
			}
			Avatar avatar = iAttacker as Avatar;
			Avatar avatar2 = iTarget as Avatar;
			IRuleset ruleSet = iPlayState.Level.CurrentScene.RuleSet;
			if (avatar != null)
			{
				Player player = avatar.Player;
				if (player.Gamer == null)
				{
					return;
				}
				if ((iResult & DamageResult.Healed) == DamageResult.Healed)
				{
					StatisticsManager.ScoreValues[] array = this.mScores;
					int id = player.ID;
					array[id].HealingDone = array[id].HealingDone + (ulong)(-(ulong)iDamage.Amount);
					player.Gamer.HealingDone += (ulong)(-(ulong)iDamage.Amount);
				}
				if (iTarget is Character)
				{
					if ((iResult & DamageResult.Damaged) == DamageResult.Damaged)
					{
						this.mTotalPlayerDamage += iDamage.Amount * iDamage.Magnitude;
						StatisticsManager.ScoreValues[] array2 = this.mScores;
						int id2 = player.ID;
						array2[id2].DamageDone = array2[id2].DamageDone + (ulong)(iDamage.Amount * iDamage.Magnitude);
						player.Gamer.DamageDone += (ulong)(iDamage.Amount * iDamage.Magnitude);
					}
					if ((iResult & DamageResult.Killed) != DamageResult.None)
					{
						bool flag = (iResult & DamageResult.OverKilled) == DamageResult.OverKilled;
						NonPlayerCharacter nonPlayerCharacter2 = iTarget as NonPlayerCharacter;
						if (nonPlayerCharacter2 != null)
						{
							if (nonPlayerCharacter2.Undying & !flag & nonPlayerCharacter2.ScoreValue == 0)
							{
								return;
							}
							SurvivalRuleset survivalRuleset = ruleSet as SurvivalRuleset;
							if (survivalRuleset != null)
							{
								int num = nonPlayerCharacter2.ScoreValue;
								if ((avatar.Faction & (iTarget as Character).Faction) != Factions.NONE)
								{
									num = -nonPlayerCharacter2.ScoreValue;
								}
								survivalRuleset.AddScore(nonPlayerCharacter2.DisplayName, num * iMultiplier);
							}
							this.mTotalScore += nonPlayerCharacter2.ScoreValue * iMultiplier;
							this.mTotalPlayerDamage += iDamage.Amount * iDamage.Magnitude;
							this.mPlayerMultiKillCounter[player.ID].Add(iTimeStamp);
						}
					}
				}
			}
			if (avatar2 != null)
			{
				Player player2 = avatar2.Player;
				if (player2 == null || player2.Gamer == null)
				{
					return;
				}
				if (iDamage.Amount * iDamage.Magnitude < 0f)
				{
					StatisticsManager.ScoreValues[] array3 = this.mScores;
					int id3 = player2.ID;
					array3[id3].HealingReceived = array3[id3].HealingReceived + (ulong)(-(ulong)iDamage.Amount);
					player2.Gamer.HealingReceived += (ulong)(-(ulong)iDamage.Amount);
				}
				if ((iResult & DamageResult.Damaged) == DamageResult.Damaged && iDamage.Amount * iDamage.Magnitude > 0f)
				{
					StatisticsManager.ScoreValues[] array4 = this.mScores;
					int id4 = player2.ID;
					array4[id4].DamageReceived = array4[id4].DamageReceived + (ulong)(iDamage.Amount * iDamage.Magnitude);
					player2.Gamer.DamageReceived += (ulong)(iDamage.Amount * iDamage.Magnitude);
				}
			}
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x000AA5C4 File Offset: 0x000A87C4
		public void AddKillEvent(PlayState iPlayState, Entity iTarget, Entity iAttacker)
		{
			VersusRuleset versusRuleset = iPlayState.Level.CurrentScene.RuleSet as VersusRuleset;
			SurvivalRuleset survivalRuleset = iPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset;
			NonPlayerCharacter nonPlayerCharacter = iAttacker as NonPlayerCharacter;
			NonPlayerCharacter nonPlayerCharacter2 = iTarget as NonPlayerCharacter;
			Avatar avatar = iTarget as Avatar;
			Avatar avatar2 = iAttacker as Avatar;
			if (nonPlayerCharacter != null)
			{
				if (nonPlayerCharacter.IsCharmed)
				{
					avatar2 = (nonPlayerCharacter.CharmOwner as Avatar);
				}
				else if (nonPlayerCharacter.IsSummoned)
				{
					avatar2 = (nonPlayerCharacter.Master as Avatar);
				}
			}
			if (avatar != null)
			{
				Player player = avatar.Player;
				StatisticsManager.ScoreValues[] array = this.mScores;
				int id = player.ID;
				array[id].Deaths = array[id].Deaths + 1U;
				if (player.Gamer != null)
				{
					player.Gamer.Deaths += 1U;
				}
			}
			if (versusRuleset != null)
			{
				if (avatar2 != null && avatar != null && (avatar2.Faction & avatar.Faction) == Factions.NONE)
				{
					Player player2 = avatar2.Player;
					StatisticsManager.ScoreValues[] array2 = this.mScores;
					int id2 = player2.ID;
					array2[id2].Kills = array2[id2].Kills + 1U;
					if (player2.Gamer != null)
					{
						player2.Gamer.Kills += 1U;
					}
					versusRuleset.OnPlayerKill(avatar2, avatar);
				}
			}
			else if (survivalRuleset != null && avatar2 != null && nonPlayerCharacter2 != null)
			{
				Player player3 = avatar2.Player;
				StatisticsManager.ScoreValues[] array3 = this.mScores;
				int id3 = player3.ID;
				array3[id3].Kills = array3[id3].Kills + 1U;
				if (player3.Gamer != null)
				{
					player3.Gamer.Kills += 1U;
				}
				int num = nonPlayerCharacter2.ScoreValue;
				int scoreMultiplier = survivalRuleset.ScoreMultiplier;
				if ((avatar2.Faction & nonPlayerCharacter2.Faction) != Factions.NONE)
				{
					num = -nonPlayerCharacter2.ScoreValue;
				}
				survivalRuleset.AddScore(nonPlayerCharacter2.DisplayName, num * scoreMultiplier);
			}
			if (avatar2 != null && nonPlayerCharacter2 != null)
			{
				Profile.Instance.AddMythosKill(iPlayState, nonPlayerCharacter2);
			}
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x000AA7B0 File Offset: 0x000A89B0
		public void AddDamageEvent(PlayState iPlayState, IDamageable iAttacker, IDamageable iTarget, double iTimeStamp, Damage iDamage, DamageResult iResult)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				int num = 1;
				if (iPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
				{
					num = (iPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).ScoreMultiplier;
				}
				this.InternalDamageEvent(iPlayState, iAttacker, iTarget, iTimeStamp, iDamage, iResult, num);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					StatisticsMessage statisticsMessage;
					if (iAttacker == null)
					{
						statisticsMessage.AttackerHandle = ushort.MaxValue;
					}
					else
					{
						statisticsMessage.AttackerHandle = iAttacker.Handle;
					}
					statisticsMessage.TargetHandle = iTarget.Handle;
					statisticsMessage.TimeStamp = iTimeStamp;
					statisticsMessage.Damage = iDamage;
					statisticsMessage.Result = iResult;
					statisticsMessage.Multiplier = (byte)num;
					NetworkManager.Instance.Interface.SendMessage<StatisticsMessage>(ref statisticsMessage);
				}
			}
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x000AA880 File Offset: 0x000A8A80
		internal void NetworkUpdate(ref StatisticsMessage iMsg)
		{
			PlayState playState = null;
			IDamageable iAttacker = null;
			IDamageable iTarget = null;
			if (iMsg.AttackerHandle != 65535)
			{
				Entity fromHandle = Entity.GetFromHandle((int)iMsg.AttackerHandle);
				iAttacker = (fromHandle as IDamageable);
				playState = fromHandle.PlayState;
			}
			if (iMsg.TargetHandle != 65535)
			{
				Entity fromHandle2 = Entity.GetFromHandle((int)iMsg.TargetHandle);
				iTarget = (fromHandle2 as IDamageable);
				playState = fromHandle2.PlayState;
			}
			if (playState == null)
			{
				return;
			}
			this.InternalDamageEvent(playState, iAttacker, iTarget, iMsg.TimeStamp, iMsg.Damage, iMsg.Result, (int)iMsg.Multiplier);
		}

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x06001954 RID: 6484 RVA: 0x000AA909 File Offset: 0x000A8B09
		public int SurvivalTotalScore
		{
			get
			{
				return this.mTotalScore;
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x06001955 RID: 6485 RVA: 0x000AA911 File Offset: 0x000A8B11
		public float SurvivalTotalDamage
		{
			get
			{
				return this.mTotalPlayerDamage;
			}
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x000AA91C File Offset: 0x000A8B1C
		public void UpdatePlayerMultiKillCounter(float iDeltaTime)
		{
			for (int i = 0; i < this.mPlayerMultiKillCounter.Length; i++)
			{
				this.mPlayerMultiKillCounter[i].Update(Game.Instance.Players[i], iDeltaTime);
			}
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x000AA958 File Offset: 0x000A8B58
		public void ClearPlayerMultiKillCounter()
		{
			for (int i = 0; i < this.mPlayerMultiKillCounter.Length; i++)
			{
				this.mPlayerMultiKillCounter[i].Clear();
			}
		}

		// Token: 0x06001958 RID: 6488 RVA: 0x000AA985 File Offset: 0x000A8B85
		public StatisticsManager.ScoreValues GetScore(int iPlayer)
		{
			return this.mScores[iPlayer];
		}

		// Token: 0x04001B77 RID: 7031
		private static StatisticsManager mSingelton;

		// Token: 0x04001B78 RID: 7032
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04001B79 RID: 7033
		private MultiKillCounter[] mPlayerMultiKillCounter;

		// Token: 0x04001B7A RID: 7034
		private List<List<LeaderBoardData>> mLeaderboards;

		// Token: 0x04001B7B RID: 7035
		private List<ulong> mSteamLeaderboards;

		// Token: 0x04001B7C RID: 7036
		private StatisticsManager.ScoreValues[] mScores;

		// Token: 0x04001B7D RID: 7037
		private int mTotalScore;

		// Token: 0x04001B7E RID: 7038
		private float mTotalPlayerDamage;

		// Token: 0x0200033F RID: 831
		public struct ScoreValues
		{
			// Token: 0x04001B7F RID: 7039
			public uint Kills;

			// Token: 0x04001B80 RID: 7040
			public uint Deaths;

			// Token: 0x04001B81 RID: 7041
			public uint OverKilled;

			// Token: 0x04001B82 RID: 7042
			public uint OverKills;

			// Token: 0x04001B83 RID: 7043
			public ulong HealingDone;

			// Token: 0x04001B84 RID: 7044
			public ulong HealingReceived;

			// Token: 0x04001B85 RID: 7045
			public ulong DamageDone;

			// Token: 0x04001B86 RID: 7046
			public ulong DamageReceived;

			// Token: 0x04001B87 RID: 7047
			public uint Suicides;

			// Token: 0x04001B88 RID: 7048
			public uint TeamKills;

			// Token: 0x04001B89 RID: 7049
			public uint TeamKilled;
		}
	}
}
