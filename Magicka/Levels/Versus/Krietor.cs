using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.Spells;
using Magicka.Misc;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Levels.Versus
{
	// Token: 0x02000625 RID: 1573
	internal class Krietor : VersusRuleset
	{
		// Token: 0x06002F30 RID: 12080 RVA: 0x0017F0D8 File Offset: 0x0017D2D8
		public Krietor(GameScene iScene, XmlNode iNode, Krietor.Settings iSettings) : base(iScene, iNode)
		{
			this.mSettings = iSettings;
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			for (int i = 0; i < iNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = iNode.ChildNodes[i];
				if (xmlNode.Name.Equals("Areas", StringComparison.InvariantCultureIgnoreCase))
				{
					for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
					{
						XmlNode xmlNode2 = xmlNode.ChildNodes[j];
						if (xmlNode2.Name.Equals("area", StringComparison.InvariantCultureIgnoreCase))
						{
							bool flag = false;
							for (int k = 0; k < xmlNode2.Attributes.Count; k++)
							{
								XmlAttribute xmlAttribute = xmlNode2.Attributes[k];
								if (xmlAttribute.Name.Equals("tourney", StringComparison.InvariantCultureIgnoreCase) && xmlAttribute.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								bool flag2 = false;
								for (int l = 0; l < xmlNode2.Attributes.Count; l++)
								{
									XmlAttribute xmlAttribute2 = xmlNode2.Attributes[l];
									if (xmlAttribute2.Name.Equals("team", StringComparison.InvariantCultureIgnoreCase))
									{
										if (xmlAttribute2.Value.Equals("A", StringComparison.InvariantCultureIgnoreCase) || xmlAttribute2.Value.Equals("red", StringComparison.InvariantCultureIgnoreCase))
										{
											flag2 = true;
											list2.Add(xmlNode2.InnerText.ToLowerInvariant().GetHashCodeCustom());
											break;
										}
										if (xmlAttribute2.Value.Equals("B", StringComparison.InvariantCultureIgnoreCase) || xmlAttribute2.Value.Equals("blue", StringComparison.InvariantCultureIgnoreCase))
										{
											flag2 = true;
											list3.Add(xmlNode2.InnerText.ToLowerInvariant().GetHashCodeCustom());
											break;
										}
									}
								}
								if (!flag2)
								{
									list.Add(xmlNode2.InnerText.ToLowerInvariant().GetHashCodeCustom());
								}
							}
						}
					}
				}
				else if (xmlNode.Name.Equals("Tournament", StringComparison.InvariantCultureIgnoreCase))
				{
					for (int m = 0; m < xmlNode.ChildNodes.Count; m++)
					{
						XmlNode xmlNode3 = xmlNode.ChildNodes[m];
						if (xmlNode3.Name.Equals("magick", StringComparison.InvariantCultureIgnoreCase))
						{
							float num = 1f;
							MagickType value = (MagickType)Enum.Parse(typeof(MagickType), xmlNode3.InnerText, true);
							int n = 0;
							while (n < xmlNode3.Attributes.Count)
							{
								XmlAttribute xmlAttribute3 = xmlNode3.Attributes[n];
								if (xmlAttribute3.Name.Equals("unlock", StringComparison.InvariantCultureIgnoreCase))
								{
									if (xmlAttribute3.Value.Equals("start", StringComparison.InvariantCultureIgnoreCase))
									{
										num = 0f;
										break;
									}
									float num2;
									if (float.TryParse(xmlAttribute3.Value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out num2))
									{
										num = num2;
										break;
									}
									break;
								}
								else
								{
									n++;
								}
							}
							if (num < 1f)
							{
								this.mUnlockMagicks.Add(new KeyValuePair<float, MagickType>(num, value));
							}
						}
					}
				}
			}
			this.mUnlockMagicks.Sort(delegate(KeyValuePair<float, MagickType> kvp1, KeyValuePair<float, MagickType> kvp2)
			{
				if (kvp1.Key > kvp2.Key)
				{
					return 1;
				}
				if (kvp1.Key < kvp2.Key)
				{
					return -1;
				}
				return 0;
			});
			this.mAreas_All = list.ToArray();
			this.mAreas_TeamA = list2.ToArray();
			this.mAreas_TeamB = list3.ToArray();
			this.mTemporarySpawns = new List<int>(this.mAreas_All.Length);
		}

		// Token: 0x06002F31 RID: 12081 RVA: 0x0017F488 File Offset: 0x0017D688
		public override void OnPlayerKill(Avatar atkAvatar, Avatar tarAvatar)
		{
		}

		// Token: 0x06002F32 RID: 12082 RVA: 0x0017F48C File Offset: 0x0017D68C
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
			if (this.mResetTimer > 0f)
			{
				return;
			}
			if ((iPlayer.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
			{
				bool flag = true;
				for (int i = 0; i < this.mPlayers.Length; i++)
				{
					if (this.mPlayers[i].Playing && (this.mPlayers[i].Team & Factions.TEAM_RED) == Factions.TEAM_RED && this.mPlayers[i].Avatar != null && !this.mPlayers[i].Avatar.Dead)
					{
						flag = false;
					}
				}
				if (flag)
				{
					short[] array = this.mScores;
					int num = 1;
					array[num] += 1;
					short[] array2 = this.mTeamScores;
					int num2 = 1;
					array2[num2] += 1;
					this.NetworkScore(1);
					this.mScoreUIs[1].SetScore((int)this.mTeamScores[1]);
					if (this.mSuddenDeath)
					{
						base.EndGame();
						return;
					}
					this.mResetTimer = 2f;
					return;
				}
			}
			else if ((iPlayer.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
			{
				bool flag2 = true;
				for (int j = 0; j < this.mPlayers.Length; j++)
				{
					if (this.mPlayers[j].Playing && (this.mPlayers[j].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE && this.mPlayers[j].Avatar != null && !this.mPlayers[j].Avatar.Dead)
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					short[] array3 = this.mScores;
					int num3 = 0;
					array3[num3] += 1;
					short[] array4 = this.mTeamScores;
					int num4 = 0;
					array4[num4] += 1;
					this.NetworkScore(0);
					this.mScoreUIs[0].SetScore((int)this.mTeamScores[0]);
					if (this.mSuddenDeath)
					{
						base.EndGame();
						return;
					}
					this.mResetTimer = 2f;
					return;
				}
			}
			else
			{
				int num5 = 0;
				int num6 = -1;
				for (int k = 0; k < this.mPlayers.Length; k++)
				{
					if (this.mPlayers[k].Playing && k != iPlayer.ID && this.mPlayers[k].Avatar != null && !this.mPlayers[k].Avatar.Dead)
					{
						num5++;
						num6 = k;
					}
				}
				if (num5 <= 1)
				{
					if (this.mSuddenDeath)
					{
						if (num6 > -1)
						{
							short[] array5 = this.mScores;
							int num7 = num6;
							array5[num7] += 1;
							int num8 = this.mIDToScoreUILookUp[num6];
							if (num8 != -1)
							{
								this.mScoreUIs[num8].SetScore((int)this.mScores[num6]);
							}
						}
						base.EndGame();
					}
					else
					{
						if (num6 > -1)
						{
							short[] array6 = this.mScores;
							int num9 = num6;
							array6[num9] += 1;
							int num10 = this.mIDToScoreUILookUp[num6];
							if (num10 != -1)
							{
								this.mScoreUIs[num10].SetScore((int)this.mScores[num6]);
							}
						}
						this.mResetTimer = 2f;
					}
					this.NetworkScore(num6);
				}
			}
		}

		// Token: 0x06002F33 RID: 12083 RVA: 0x0017F7D4 File Offset: 0x0017D9D4
		public override void OnEndGame()
		{
			Player player = ControlManager.Instance.MenuController.Player;
			if ((player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
			{
				if (this.mTeamScores[0] > this.mTeamScores[1])
				{
					Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
					return;
				}
			}
			else if ((player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
			{
				if (this.mTeamScores[1] > this.mTeamScores[0])
				{
					Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
					return;
				}
			}
			else if (this.HaveBestScore(player.ID))
			{
				Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
			}
		}

		// Token: 0x06002F34 RID: 12084 RVA: 0x0017F86C File Offset: 0x0017DA6C
		private bool HaveBestScore(int iPlayerID)
		{
			int num = 0;
			for (int i = 1; i < this.mScores.Length; i++)
			{
				if (this.mScores[i] > this.mScores[num])
				{
					num = i;
				}
			}
			return num == iPlayerID;
		}

		// Token: 0x06002F35 RID: 12085 RVA: 0x0017F8A8 File Offset: 0x0017DAA8
		private void Reset()
		{
			foreach (Liquid liquid in this.mScene.Level.CurrentScene.Liquids)
			{
				if (liquid.AutoFreeze)
				{
					liquid.FreezeAll(2f);
				}
				else
				{
					liquid.FreezeAll(-2f);
				}
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (this.mSuddenDeath && !this.mTeams)
				{
					int num = int.MinValue;
					for (int j = 0; j < this.mScores.Length; j++)
					{
						if ((int)this.mScores[j] > num)
						{
							num = (int)this.mScores[j];
						}
					}
					this.mTemporarySpawns.Clear();
					this.mTemporarySpawns.AddRange(this.mAreas_All);
					int num2 = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
					for (int k = 0; k < this.mScores.Length; k++)
					{
						if (this.mPlayers[k].Playing)
						{
							if ((int)this.mScores[k] == num)
							{
								base.SetupPlayer(k, this.mTemporarySpawns[num2 % this.mTemporarySpawns.Count]);
								this.mTemporarySpawns.RemoveAt(num2 % this.mTemporarySpawns.Count);
							}
							else if (this.mPlayers[k].Avatar != null && !this.mPlayers[k].Avatar.Dead)
							{
								this.mPlayers[k].Avatar.Terminate(true, false, true);
							}
						}
					}
				}
				else if (this.mTeams)
				{
					this.mTemporarySpawns.Clear();
					this.mTemporarySpawns.AddRange(this.mAreas_TeamA);
					int num3 = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
					for (int l = 0; l < this.mPlayers.Length; l++)
					{
						if (this.mPlayers[l].Playing && (this.mPlayers[l].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
						{
							base.SetupPlayer(l, this.mTemporarySpawns[num3 % this.mTemporarySpawns.Count]);
							this.mTemporarySpawns.RemoveAt(num3 % this.mTemporarySpawns.Count);
						}
					}
					this.mTemporarySpawns.Clear();
					this.mTemporarySpawns.AddRange(this.mAreas_TeamB);
					num3 = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
					for (int m = 0; m < this.mPlayers.Length; m++)
					{
						if (this.mPlayers[m].Playing && (this.mPlayers[m].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
						{
							base.SetupPlayer(m, this.mTemporarySpawns[num3 % this.mTemporarySpawns.Count]);
							this.mTemporarySpawns.RemoveAt(num3 % this.mTemporarySpawns.Count);
						}
					}
				}
				else
				{
					this.mTemporarySpawns.Clear();
					this.mTemporarySpawns.AddRange(this.mAreas_All);
					int num4 = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
					for (int n = 0; n < this.mPlayers.Length; n++)
					{
						if (this.mPlayers[n].Playing)
						{
							base.SetupPlayer(n, this.mTemporarySpawns[num4 % this.mTemporarySpawns.Count]);
							this.mTemporarySpawns.RemoveAt(num4 % this.mTemporarySpawns.Count);
						}
					}
				}
			}
			Nullify.Instance.NullifyArea(this.mScene.PlayState, default(Vector3), true);
			base.CountDown(4f);
		}

		// Token: 0x06002F36 RID: 12086 RVA: 0x0017FC70 File Offset: 0x0017DE70
		private void UnlockMagick(MagickType iMagick)
		{
			for (int i = 0; i < this.mPlayers.Length; i++)
			{
				if (this.mPlayers[i].Playing)
				{
					SpellManager.Instance.UnlockMagick(this.mPlayers[i], iMagick);
				}
			}
		}

		// Token: 0x06002F37 RID: 12087 RVA: 0x0017FCB4 File Offset: 0x0017DEB4
		public override void Initialize()
		{
			base.Initialize();
			this.mSuddenDeath = false;
			this.mTempUnlockMagicks = new List<KeyValuePair<float, MagickType>>(this.mUnlockMagicks);
			this.mTimeLimit = (float)this.mSettings.TimeLimit * 60f;
			this.mTimeLimitTimer = this.mTimeLimit;
			this.mTimeLimitTarget = this.mTimeLimit;
			this.mTeams = this.mSettings.TeamsEnabled;
			this.mResetTimer = 0f;
			for (int i = 0; i < this.mScores.Length; i++)
			{
				this.mScores[i] = 0;
			}
			for (int j = 0; j < this.mTeamScores.Length; j++)
			{
				this.mTeamScores[j] = 0;
			}
			if (this.mTeams)
			{
				this.mScoreUIs.Add(new VersusRuleset.Score(true));
				this.mScoreUIs.Add(new VersusRuleset.Score(false));
				for (int k = 0; k < this.mPlayers.Length; k++)
				{
					if (this.mPlayers[k].Playing)
					{
						Texture2D portrait = this.mPlayers[k].Gamer.Avatar.Portrait;
						this.mPlayers[k].Avatar.Faction &= ~Factions.FRIENDLY;
						if ((this.mPlayers[k].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
						{
							this.mIDToScoreUILookUp[k] = 0;
							this.mScoreUIs[0].AddPlayer(this.mPlayers[k].GamerTag, this.mPlayers[k].ID, portrait, Defines.PLAYERCOLORS[(int)this.mPlayers[k].Color]);
						}
						else
						{
							this.mIDToScoreUILookUp[k] = 1;
							this.mScoreUIs[1].AddPlayer(this.mPlayers[k].GamerTag, this.mPlayers[k].ID, portrait, Defines.PLAYERCOLORS[(int)this.mPlayers[k].Color]);
						}
					}
				}
			}
			else
			{
				int l = 0;
				int num = 0;
				while (l < this.mPlayers.Length)
				{
					if (this.mPlayers[l].Playing)
					{
						this.mPlayers[l].Avatar.Faction &= ~Factions.FRIENDLY;
						Texture2D portrait2 = this.mPlayers[l].Gamer.Avatar.Portrait;
						VersusRuleset.Score score = new VersusRuleset.Score(num % 2 == 0);
						score.AddPlayer(this.mPlayers[l].GamerTag, this.mPlayers[l].ID, portrait2, Defines.PLAYERCOLORS[(int)this.mPlayers[l].Color]);
						this.mIDToScoreUILookUp[l] = this.mScoreUIs.Count;
						this.mScoreUIs.Add(score);
						num++;
					}
					l++;
				}
			}
			for (int m = 0; m < this.mTempUnlockMagicks.Count; m++)
			{
				if (this.mTempUnlockMagicks[m].Key <= 0f)
				{
					this.UnlockMagick(this.mTempUnlockMagicks[m].Value);
					this.mTempUnlockMagicks.RemoveAt(m--);
				}
			}
			if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
			{
				this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x06002F38 RID: 12088 RVA: 0x00180023 File Offset: 0x0017E223
		public override void DeInitialize()
		{
			if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
			{
				this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x06002F39 RID: 12089 RVA: 0x00180048 File Offset: 0x0017E248
		public override void Update(float iDeltaTime, DataChannel iDataChannel)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			if (this.mTimeLimit > 0f)
			{
				if (!this.mScene.PlayState.IsGameEnded && this.mResetTimer <= 0f && this.mCountDownTimer <= 0f)
				{
					this.mTimeLimitTimer -= iDeltaTime;
					if (this.mTimeLimitTimer <= 0f)
					{
						int num = 0;
						int num2 = int.MinValue;
						for (int i = 0; i < this.mScores.Length; i++)
						{
							if ((int)this.mScores[i] > num2)
							{
								num2 = (int)this.mScores[i];
								num = i;
							}
						}
						bool flag = false;
						for (int j = 0; j < this.mScores.Length; j++)
						{
							if (j != num && (int)this.mScores[j] == num2)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							base.EndGame();
							this.mTimeLimitTimer = 0f;
						}
						else
						{
							this.mSuddenDeath = true;
							this.mTimeLimit = 0f;
							this.mTimeLimitTimer = this.mTimeLimit;
							if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
							{
								this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
							}
							this.Reset();
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								RulesetMessage rulesetMessage = default(RulesetMessage);
								rulesetMessage.Type = this.RulesetType;
								rulesetMessage.Byte01 = 2;
								rulesetMessage.Float01 = 0f;
								NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage);
							}
						}
					}
				}
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					if (this.mTimeLimitNetworkUpdate > 1f)
					{
						RulesetMessage rulesetMessage2 = default(RulesetMessage);
						rulesetMessage2.Type = this.RulesetType;
						rulesetMessage2.Byte01 = 0;
						rulesetMessage2.Float01 = this.mTimeLimitTimer;
						NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage2);
						this.mTimeLimitNetworkUpdate = 0f;
					}
					this.mTimeLimitNetworkUpdate += iDeltaTime;
				}
			}
			if (this.mResetTimer > 0f)
			{
				this.mResetTimer -= iDeltaTime;
				if (this.mResetTimer <= 0f)
				{
					this.mResetTimer = 0f;
					this.Reset();
				}
			}
			VersusRuleset.RenderData renderData = this.mRenderData[(int)iDataChannel];
			for (int k = 0; k < this.mTempUnlockMagicks.Count; k++)
			{
				if (this.mTimeLimit - this.mTimeLimitTimer > this.mTempUnlockMagicks[k].Key * this.mTimeLimit)
				{
					this.UnlockMagick(this.mTempUnlockMagicks[k].Value);
					this.mTempUnlockMagicks.RemoveAt(k--);
				}
			}
			if (this.mTempUnlockMagicks.Count > 0)
			{
				renderData.SetUnlockMagick(this.mTempUnlockMagicks[0].Value);
				float num3 = this.mTempUnlockMagicks[0].Key * this.mTimeLimit - (this.mTimeLimit - this.mTimeLimitTimer);
				if (num3 < 10f)
				{
					renderData.SetUnlockMagickTime(num3);
				}
				else
				{
					renderData.SetUnlockMagickTime(0f);
				}
			}
			else
			{
				renderData.SetUnlockMagick(MagickType.None);
			}
			renderData.DrawTime = (this.mTimeLimit > 0f);
			renderData.TimeLimit = this.mTimeLimit;
			renderData.Time = this.mTimeLimitTimer;
			renderData.SuddenDeath(this.mSuddenDeath);
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

		// Token: 0x06002F3A RID: 12090 RVA: 0x00180400 File Offset: 0x0017E600
		public override void LocalUpdate(float iDeltaTime, DataChannel iDataChannel)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			VersusRuleset.RenderData renderData = this.mRenderData[(int)iDataChannel];
			if (this.mResetTimer > 0f)
			{
				this.mResetTimer -= iDeltaTime;
				if (this.mResetTimer <= 0f)
				{
					this.mResetTimer = 0f;
					this.Reset();
				}
			}
			this.mTimeLimitTimer += (this.mTimeLimitTarget - this.mTimeLimitTimer) * iDeltaTime;
			for (int i = 0; i < this.mTempUnlockMagicks.Count; i++)
			{
				if (this.mTimeLimit - this.mTimeLimitTimer > this.mTempUnlockMagicks[i].Key * this.mTimeLimit)
				{
					this.UnlockMagick(this.mTempUnlockMagicks[i].Value);
					this.mTempUnlockMagicks.RemoveAt(i--);
				}
			}
			if (this.mTempUnlockMagicks.Count > 0)
			{
				renderData.SetUnlockMagick(this.mTempUnlockMagicks[0].Value);
				float num = this.mTempUnlockMagicks[0].Key * this.mTimeLimit - (this.mTimeLimit - this.mTimeLimitTimer);
				if (num < 10f)
				{
					renderData.SetUnlockMagickTime(num);
				}
			}
			renderData.DrawTime = (this.mTimeLimit > 0f);
			renderData.TimeLimit = this.mTimeLimit;
			renderData.Time = this.mTimeLimitTimer;
			renderData.SuddenDeath(this.mSuddenDeath);
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

		// Token: 0x06002F3B RID: 12091 RVA: 0x001805D0 File Offset: 0x0017E7D0
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

		// Token: 0x06002F3C RID: 12092 RVA: 0x001806C0 File Offset: 0x0017E8C0
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
				if (this.mTeams)
				{
					this.mScoreUIs[0].SetScore((int)this.mTeamScores[0]);
					this.mScoreUIs[1].SetScore((int)this.mTeamScores[1]);
					if (!this.mSuddenDeath)
					{
						float num2 = NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
						this.mResetTimer = 2f - num2;
						return;
					}
				}
				else
				{
					int num3 = this.mIDToScoreUILookUp[(int)iMsg.Byte02];
					if (num3 != -1)
					{
						this.mScoreUIs[num3].SetScore((int)this.mScores[(int)iMsg.Byte02]);
					}
					if (!this.mSuddenDeath)
					{
						float num4 = NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
						this.mResetTimer = 2f - num4;
						return;
					}
				}
			}
			else
			{
				if (iMsg.Byte01 == 2)
				{
					this.mSuddenDeath = true;
					this.mTimeLimit = iMsg.Float01;
					float num5 = NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
					this.mTimeLimitTimer = iMsg.Float01;
					this.mTimeLimitTarget = iMsg.Float01 - num5;
					if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
					{
						this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
					}
					this.Reset();
					return;
				}
				base.NetworkUpdate(ref iMsg);
			}
		}

		// Token: 0x17000B22 RID: 2850
		// (get) Token: 0x06002F3D RID: 12093 RVA: 0x001808B6 File Offset: 0x0017EAB6
		public override bool DropMagicks
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B23 RID: 2851
		// (get) Token: 0x06002F3E RID: 12094 RVA: 0x001808B9 File Offset: 0x0017EAB9
		public override Rulesets RulesetType
		{
			get
			{
				return Rulesets.Kreitor;
			}
		}

		// Token: 0x06002F3F RID: 12095 RVA: 0x001808BC File Offset: 0x0017EABC
		public override bool CanRevive(Player iReviver, Player iRevivee)
		{
			return this.mTeams && (iReviver.Team & iRevivee.Team) != Factions.NONE;
		}

		// Token: 0x06002F40 RID: 12096 RVA: 0x001808DA File Offset: 0x0017EADA
		internal override short[] GetScores()
		{
			return this.mScores;
		}

		// Token: 0x06002F41 RID: 12097 RVA: 0x001808E2 File Offset: 0x0017EAE2
		internal override short[] GetTeamScores()
		{
			return this.mTeamScores;
		}

		// Token: 0x17000B24 RID: 2852
		// (get) Token: 0x06002F42 RID: 12098 RVA: 0x001808EA File Offset: 0x0017EAEA
		internal override bool Teams
		{
			get
			{
				return this.mTeams;
			}
		}

		// Token: 0x0400334F RID: 13135
		private const float RESET_TIME = 2f;

		// Token: 0x04003350 RID: 13136
		internal static readonly int LOC_UNLOCKED = "#magick_unlocked".GetHashCodeCustom();

		// Token: 0x04003351 RID: 13137
		private short[] mTeamScores = new short[2];

		// Token: 0x04003352 RID: 13138
		private short[] mScores = new short[4];

		// Token: 0x04003353 RID: 13139
		private float mTimeLimitNetworkUpdate;

		// Token: 0x04003354 RID: 13140
		private float mTimeLimitTarget;

		// Token: 0x04003355 RID: 13141
		private float mTimeLimitTimer;

		// Token: 0x04003356 RID: 13142
		private float mTimeLimit;

		// Token: 0x04003357 RID: 13143
		private float mResetTimer;

		// Token: 0x04003358 RID: 13144
		private bool mTeams;

		// Token: 0x04003359 RID: 13145
		private bool mSuddenDeath;

		// Token: 0x0400335A RID: 13146
		private List<KeyValuePair<float, MagickType>> mUnlockMagicks = new List<KeyValuePair<float, MagickType>>();

		// Token: 0x0400335B RID: 13147
		private List<KeyValuePair<float, MagickType>> mTempUnlockMagicks;

		// Token: 0x0400335C RID: 13148
		private Krietor.Settings mSettings;

		// Token: 0x02000626 RID: 1574
		internal new class Settings : VersusRuleset.Settings
		{
			// Token: 0x06002F45 RID: 12101 RVA: 0x00180904 File Offset: 0x0017EB04
			public Settings()
			{
				this.mTimeLimit = base.AddOption<int>(this.LOC_TIME_LIMIT, this.LOC_TT_TIME, new int[]
				{
					5,
					10
				}, new int?[]
				{
					null,
					null
				});
				this.mTimeLimit.SelectedIndex = 1;
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
			}

			// Token: 0x17000B25 RID: 2853
			// (get) Token: 0x06002F46 RID: 12102 RVA: 0x001809EB File Offset: 0x0017EBEB
			public int TimeLimit
			{
				get
				{
					return this.mTimeLimit.SelectedValue;
				}
			}

			// Token: 0x17000B26 RID: 2854
			// (get) Token: 0x06002F47 RID: 12103 RVA: 0x001809F8 File Offset: 0x0017EBF8
			public override bool TeamsEnabled
			{
				get
				{
					return this.mTeams.SelectedValue;
				}
			}

			// Token: 0x0400335E RID: 13150
			private DropDownBox<int> mTimeLimit;

			// Token: 0x0400335F RID: 13151
			private DropDownBox<bool> mTeams;
		}
	}
}
