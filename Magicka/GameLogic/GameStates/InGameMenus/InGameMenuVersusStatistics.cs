using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x02000648 RID: 1608
	internal class InGameMenuVersusStatistics : InGameMenu
	{
		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x060030EB RID: 12523 RVA: 0x0019192C File Offset: 0x0018FB2C
		public static InGameMenuVersusStatistics Instance
		{
			get
			{
				if (InGameMenuVersusStatistics.sSingelton == null)
				{
					lock (InGameMenuVersusStatistics.sSingeltonLock)
					{
						if (InGameMenuVersusStatistics.sSingelton == null)
						{
							InGameMenuVersusStatistics.sSingelton = new InGameMenuVersusStatistics();
						}
					}
				}
				return InGameMenuVersusStatistics.sSingelton;
			}
		}

		// Token: 0x060030EC RID: 12524 RVA: 0x00191980 File Offset: 0x0018FB80
		private InGameMenuVersusStatistics()
		{
			this.mBackgroundSize = new Vector2(600f, 600f);
			this.mRedWin = false;
			this.mBlueWin = false;
			this.mSortingOrder = new List<KeyValuePair<int, int>>(4);
			this.mLoserTeamOrder = new List<KeyValuePair<int, int>>(4);
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			this.mModeText = new Text(200, font, TextAlign.Center, false);
			this.mModeText.SetText("");
			this.mVictorText = new Text(200, font, TextAlign.Center, false);
			this.mVictorText.SetText("");
			this.mLoserTeamText = new Text(200, font, TextAlign.Center, false);
			this.mLoserTeamText.SetText("");
			lock (Game.Instance.GraphicsDevice)
			{
				this.mPagesTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
			}
			font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.mKillsTexts = new Text[4];
			this.mDeathsTexts = new Text[4];
			this.mPlayerTexts = new Text[4];
			for (int i = 0; i < 4; i++)
			{
				this.mKillsTexts[i] = new Text(200, font, TextAlign.Left, false);
				this.mKillsTexts[i].SetText("");
				this.mDeathsTexts[i] = new Text(200, font, TextAlign.Left, false);
				this.mDeathsTexts[i].SetText("");
				this.mPlayerTexts[i] = new Text(200, font, TextAlign.Left, false);
				this.mPlayerTexts[i].SetText("");
			}
			this.AddMenuTextItem(InGameMenuVersusStatistics.LOC_RETRY, this.mItemFont, TextAlign.Center);
			this.AddMenuTextItem(InGameMenuVersusStatistics.LOC_DISCONNECT, this.mItemFont, TextAlign.Center);
			this.AddMenuTextItem(InGameMenuVersusStatistics.LOC_QUIT, this.mItemFont, TextAlign.Center);
		}

		// Token: 0x060030ED RID: 12525 RVA: 0x00191B80 File Offset: 0x0018FD80
		protected override void IUpdate(DataChannel iDataChannel, float iDeltaTime)
		{
			base.IUpdate(iDataChannel, iDeltaTime);
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer != null && networkServer.AllClientsReady)
			{
				this.mMenuItems[0].Enabled = true;
				this.mMenuItems[2].Enabled = true;
			}
		}

		// Token: 0x060030EE RID: 12526 RVA: 0x00191BD4 File Offset: 0x0018FDD4
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			base.IDraw(iDeltaTime, ref iBackgroundSize);
			Vector2 position = default(Vector2);
			position.X = InGameMenu.sScreenSize.X * 0.5f;
			position.Y = InGameMenu.sScreenSize.Y * 0.5f - 200f;
			InGameMenu.sEffect.Color = new Vector4(1f);
			this.mModeText.Draw(InGameMenu.sEffect, position.X, position.Y);
			position.Y += (float)this.mModeText.Font.LineHeight * 2f;
			if (this.mRuleset.Teams)
			{
				if (this.mRedWin)
				{
					Matrix matrix = default(Matrix);
					matrix.M44 = 1f;
					matrix.M11 = iBackgroundSize.X * InGameMenu.sScale;
					matrix.M22 = (float)this.mVictorText.Font.LineHeight;
					matrix.M41 = position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
					matrix.M42 = position.Y;
					InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[0], 0.8f);
					this.DrawStrip(ref matrix);
					InGameMenu.sEffect.Color = new Vector4(1f);
					this.mVictorText.Draw(InGameMenu.sEffect, position.X, position.Y);
					position.Y += (float)this.mVictorText.Font.LineHeight;
					this.DrawSortedPlayerScores(ref position, this.mSortingOrder);
					position.Y += (float)this.mVictorText.Font.LineHeight;
					matrix.M42 = position.Y;
					InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[3], 0.8f);
					this.DrawStrip(ref matrix);
					InGameMenu.sEffect.Color = new Vector4(1f);
					this.mLoserTeamText.Draw(InGameMenu.sEffect, position.X, position.Y);
					position.Y += (float)this.mLoserTeamText.Font.LineHeight;
					this.DrawSortedPlayerScores(ref position, this.mLoserTeamOrder);
				}
				else if (this.mBlueWin)
				{
					Matrix matrix2 = default(Matrix);
					matrix2.M44 = 1f;
					matrix2.M11 = iBackgroundSize.X * InGameMenu.sScale;
					matrix2.M22 = (float)this.mVictorText.Font.LineHeight;
					matrix2.M41 = position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
					matrix2.M42 = position.Y;
					InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[3], 0.8f);
					this.DrawStrip(ref matrix2);
					InGameMenu.sEffect.Color = new Vector4(1f);
					this.mVictorText.Draw(InGameMenu.sEffect, position.X, position.Y);
					position.Y += (float)this.mVictorText.Font.LineHeight;
					this.DrawSortedPlayerScores(ref position, this.mSortingOrder);
					position.Y += (float)this.mVictorText.Font.LineHeight;
					matrix2.M42 = position.Y;
					InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[0], 0.8f);
					this.DrawStrip(ref matrix2);
					InGameMenu.sEffect.Color = new Vector4(1f);
					this.mLoserTeamText.Draw(InGameMenu.sEffect, position.X, position.Y);
					position.Y += (float)this.mLoserTeamText.Font.LineHeight;
					this.DrawSortedPlayerScores(ref position, this.mLoserTeamOrder);
				}
				else
				{
					Matrix matrix3 = default(Matrix);
					matrix3.M44 = 1f;
					matrix3.M11 = iBackgroundSize.X * InGameMenu.sScale;
					matrix3.M22 = (float)this.mVictorText.Font.LineHeight;
					matrix3.M41 = position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
					matrix3.M42 = position.Y;
					if (this.mVictorPlayerIndex != -1)
					{
						InGameMenu.sEffect.Color = new Vector4(1f, 1f, 1f, 0.8f);
						this.DrawStrip(ref matrix3);
						InGameMenu.sEffect.Color = new Vector4(1f);
					}
					this.mVictorText.Draw(InGameMenu.sEffect, position.X, position.Y);
					position.Y += (float)this.mVictorText.Font.LineHeight;
					this.DrawSortedPlayerScores(ref position, this.mSortingOrder);
				}
			}
			else
			{
				Matrix matrix4 = default(Matrix);
				matrix4.M44 = 1f;
				matrix4.M11 = iBackgroundSize.X * InGameMenu.sScale;
				matrix4.M22 = (float)this.mVictorText.Font.LineHeight;
				matrix4.M41 = position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
				matrix4.M42 = position.Y;
				if (this.mVictorPlayerIndex != -1)
				{
					InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[this.mVictorPlayerColor], 0.8f);
					this.DrawStrip(ref matrix4);
					if (this.mVictorPlayerColor == 10)
					{
						InGameMenu.sEffect.Color = new Vector4(0f, 0f, 0f, 1f);
					}
					else
					{
						InGameMenu.sEffect.Color = new Vector4(1f);
					}
				}
				this.mVictorText.Draw(InGameMenu.sEffect, position.X, position.Y);
				InGameMenu.sEffect.Color = new Vector4(1f);
				position.Y += (float)this.mVictorText.Font.LineHeight;
				this.DrawSortedPlayerScores(ref position, this.mSortingOrder);
			}
			position.Y = InGameMenu.sScreenSize.Y * 0.5f + 300f - (float)this.mMenuItems.Count * 32f;
			NetworkState state = NetworkManager.Instance.State;
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				if ((state != NetworkState.Client || i != 0) && (state != NetworkState.Offline || i != 1))
				{
					MenuItem menuItem = this.mMenuItems[i];
					menuItem.Position = position;
					menuItem.Draw(InGameMenu.sEffect);
					position.Y += menuItem.BottomRight.Y - menuItem.TopLeft.Y;
				}
			}
		}

		// Token: 0x060030EF RID: 12527 RVA: 0x00192310 File Offset: 0x00190510
		private void DrawStrip(ref Matrix iTransform)
		{
			InGameMenu.sEffect.TextureOffset = new Vector2(0f, 0f);
			InGameMenu.sEffect.TextureScale = new Vector2(1f, 1f);
			InGameMenu.sEffect.Texture = InGameMenu.sBackgroundTexture;
			InGameMenu.sEffect.TextureEnabled = true;
			InGameMenu.sEffect.Transform = iTransform;
			InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
			InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
			InGameMenu.sEffect.CommitChanges();
			InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
		}

		// Token: 0x060030F0 RID: 12528 RVA: 0x001923CC File Offset: 0x001905CC
		private void DrawSortedPlayerScores(ref Vector2 iPosition, List<KeyValuePair<int, int>> iSortedList)
		{
			for (int i = 0; i < iSortedList.Count; i++)
			{
				int value = iSortedList[i].Value;
				this.mPlayerTexts[value].Draw(InGameMenu.sEffect, iPosition.X - 180f, iPosition.Y);
				this.mKillsTexts[value].Draw(InGameMenu.sEffect, iPosition.X + 10f, iPosition.Y);
				this.mDeathsTexts[value].Draw(InGameMenu.sEffect, iPosition.X + 110f, iPosition.Y);
				base.DrawGraphics(this.mPagesTexture, new Rectangle(1024, 64, 64, 64), new Vector4(iPosition.X + 40f, iPosition.Y, 32f * InGameMenu.sScale, 32f * InGameMenu.sScale));
				base.DrawGraphics(this.mPagesTexture, new Rectangle(1024, 0, 64, 64), new Vector4(iPosition.X + 140f, iPosition.Y, 32f * InGameMenu.sScale, 32f * InGameMenu.sScale));
				iPosition.Y += (float)this.mPlayerTexts[value].Font.LineHeight;
			}
		}

		// Token: 0x060030F1 RID: 12529 RVA: 0x00192520 File Offset: 0x00190720
		protected override void IControllerSelect(Controller iSender)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
				if (this.mSelectedItem == 0 && (networkServer == null || networkServer.AllClientsReady))
				{
					InGameMenu.sPlayState.Restart(this, RestartType.StartOfLevel);
					if (networkServer != null)
					{
						networkServer.SetAllClientsBusy();
						return;
					}
				}
				else
				{
					if (this.mSelectedItem == 1)
					{
						GameEndMessage gameEndMessage;
						gameEndMessage.Condition = EndGameCondition.ChallengeExit;
						gameEndMessage.Argument = 1;
						gameEndMessage.DelayTime = 0f;
						gameEndMessage.Phony = false;
						if (networkServer != null)
						{
							NetworkManager.Instance.EndSession();
							while (!(Tome.Instance.CurrentMenu is SubMenuMain))
							{
								Tome.Instance.PopMenuInstant();
							}
						}
						InGameMenu.sPlayState.Endgame(ref gameEndMessage);
						return;
					}
					if (this.mSelectedItem == 2 && (networkServer == null || networkServer.AllClientsReady))
					{
						GameEndMessage gameEndMessage2;
						gameEndMessage2.Condition = EndGameCondition.ChallengeExit;
						gameEndMessage2.Argument = 1;
						gameEndMessage2.DelayTime = 0f;
						gameEndMessage2.Phony = false;
						InGameMenu.sPlayState.Endgame(ref gameEndMessage2);
						if (networkServer != null)
						{
							networkServer.SetAllClientsBusy();
							networkServer.SendMessage<GameEndMessage>(ref gameEndMessage2);
							return;
						}
					}
				}
			}
			else
			{
				if (this.mSelectedItem == 1)
				{
					GameEndMessage gameEndMessage3;
					gameEndMessage3.Condition = EndGameCondition.ChallengeExit;
					gameEndMessage3.Argument = 1;
					gameEndMessage3.DelayTime = 0f;
					gameEndMessage3.Phony = false;
					NetworkManager.Instance.EndSession();
					while (!(Tome.Instance.CurrentMenu is SubMenuMain))
					{
						Tome.Instance.PopMenuInstant();
					}
					InGameMenu.sPlayState.Endgame(ref gameEndMessage3);
					return;
				}
				if (this.mSelectedItem == 2)
				{
					GameEndLoadMessage gameEndLoadMessage = default(GameEndLoadMessage);
					NetworkManager.Instance.Interface.SendMessage<GameEndLoadMessage>(ref gameEndLoadMessage, 0);
					this.mMenuItems[1].Enabled = false;
				}
			}
		}

		// Token: 0x060030F2 RID: 12530 RVA: 0x001926D2 File Offset: 0x001908D2
		protected override void IControllerBack(Controller iSender)
		{
		}

		// Token: 0x060030F3 RID: 12531 RVA: 0x001926D4 File Offset: 0x001908D4
		protected override string IGetHighlightedButtonName()
		{
			string result = InGameMenuVersusStatistics.OPTION_STRINGS[this.mSelectedItem];
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (this.mSelectedItem == 1)
				{
					result = "disconnect";
				}
				else if (this.mSelectedItem == 2)
				{
					result = "ready";
				}
			}
			return result;
		}

		// Token: 0x060030F4 RID: 12532 RVA: 0x00192808 File Offset: 0x00190A08
		protected override void OnEnter()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				this.mMenuItems[0].Enabled = false;
				this.mMenuItems[1].Enabled = true;
				this.mMenuItems[2].Enabled = true;
				(this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuVersusStatistics.LOC_READY);
			}
			else if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mMenuItems[0].Enabled = false;
				this.mMenuItems[1].Enabled = true;
				this.mMenuItems[2].Enabled = false;
				(this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuVersusStatistics.LOC_LOBBY);
			}
			else
			{
				this.mMenuItems[0].Enabled = true;
				this.mMenuItems[1].Enabled = false;
				this.mMenuItems[2].Enabled = true;
				(this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuVersusStatistics.LOC_QUIT);
			}
			this.mRedWin = false;
			this.mBlueWin = false;
			this.mPlayers = Game.Instance.Players;
			this.mRuleset = (InGameMenu.sPlayState.Level.CurrentScene.RuleSet as VersusRuleset);
			short[] scores = this.mRuleset.GetScores();
			short[] teamScores = this.mRuleset.GetTeamScores();
			this.mSortingOrder.Clear();
			this.mLoserTeamOrder.Clear();
			this.mVictorPlayerColor = 10;
			if (this.mRuleset.Teams)
			{
				for (int i = 0; i < this.mPlayers.Length; i++)
				{
					if (this.mPlayers[i].Playing)
					{
						this.mPlayerTexts[i].SetText(this.mPlayers[i].GamerTag);
					}
				}
				int num = 0;
				while (num < scores.Length && num < this.mKillsTexts.Length)
				{
					if (this.mPlayers[num].Playing)
					{
						this.mPlayerTexts[num].SetText(this.mPlayers[num].GamerTag);
						StatisticsManager.ScoreValues score = StatisticsManager.Instance.GetScore(num);
						this.mKillsTexts[num].SetText(score.Kills.ToString());
						this.mDeathsTexts[num].SetText(score.Deaths.ToString());
					}
					num++;
				}
				if (teamScores[0] > teamScores[1])
				{
					this.mRedWin = true;
					string text = LanguageManager.Instance.GetString(Defines.LOC_GAME_VS_WINNER);
					text = text.Replace("#player;", LanguageManager.Instance.GetString(VersusRuleset.LOC_TEAMRED));
					this.mVictorText.SetText(text);
					this.mLoserTeamText.SetText(LanguageManager.Instance.GetString(VersusRuleset.LOC_TEAMBLUE));
					for (int j = 0; j < this.mPlayers.Length; j++)
					{
						if (this.mPlayers[j].Playing)
						{
							if ((this.mPlayers[j].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
							{
								StatisticsManager.ScoreValues score2 = StatisticsManager.Instance.GetScore(j);
								this.mSortingOrder.Add(new KeyValuePair<int, int>((int)score2.Kills, j));
								if (this.mPlayers[j].Gamer != null && !(this.mPlayers[j].Gamer is NetworkGamer))
								{
									this.mPlayers[j].Gamer.VersusWins += 1U;
									this.mPlayers[j].Gamer.IncrementVersusWinStreak(InGameMenu.sPlayState);
								}
							}
							else if ((this.mPlayers[j].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
							{
								StatisticsManager.ScoreValues score3 = StatisticsManager.Instance.GetScore(j);
								this.mLoserTeamOrder.Add(new KeyValuePair<int, int>((int)score3.Kills, j));
								if (this.mPlayers[j].Gamer != null && !(this.mPlayers[j].Gamer is NetworkGamer))
								{
									this.mPlayers[j].Gamer.VersusDefeats += 1U;
									this.mPlayers[j].Gamer.ResetVersusWinStreak();
								}
							}
						}
					}
					this.mSortingOrder.Sort(delegate(KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2)
					{
						if (kvp1.Key > kvp2.Key)
						{
							return -1;
						}
						if (kvp1.Key < kvp2.Key)
						{
							return 1;
						}
						return 0;
					});
					this.mLoserTeamOrder.Sort(delegate(KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2)
					{
						if (kvp1.Key > kvp2.Key)
						{
							return -1;
						}
						if (kvp1.Key < kvp2.Key)
						{
							return 1;
						}
						return 0;
					});
				}
				else if (teamScores[0] < teamScores[1])
				{
					this.mBlueWin = true;
					string text2 = LanguageManager.Instance.GetString(Defines.LOC_GAME_VS_WINNER);
					text2 = text2.Replace("#player;", LanguageManager.Instance.GetString(VersusRuleset.LOC_TEAMBLUE));
					this.mVictorText.SetText(text2);
					this.mLoserTeamText.SetText(LanguageManager.Instance.GetString(VersusRuleset.LOC_TEAMRED));
					for (int k = 0; k < this.mPlayers.Length; k++)
					{
						if (this.mPlayers[k].Playing)
						{
							if ((this.mPlayers[k].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
							{
								StatisticsManager.ScoreValues score4 = StatisticsManager.Instance.GetScore(k);
								this.mSortingOrder.Add(new KeyValuePair<int, int>((int)score4.Kills, k));
								if (this.mPlayers[k].Gamer != null && !(this.mPlayers[k].Gamer is NetworkGamer))
								{
									this.mPlayers[k].Gamer.VersusWins += 1U;
									this.mPlayers[k].Gamer.IncrementVersusWinStreak(InGameMenu.sPlayState);
								}
							}
							else if ((this.mPlayers[k].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
							{
								StatisticsManager.ScoreValues score5 = StatisticsManager.Instance.GetScore(k);
								this.mLoserTeamOrder.Add(new KeyValuePair<int, int>((int)score5.Kills, k));
								if (this.mPlayers[k].Gamer != null && !(this.mPlayers[k].Gamer is NetworkGamer))
								{
									this.mPlayers[k].Gamer.VersusDefeats += 1U;
									this.mPlayers[k].Gamer.ResetVersusWinStreak();
								}
							}
						}
					}
					this.mSortingOrder.Sort(delegate(KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2)
					{
						if (kvp1.Key > kvp2.Key)
						{
							return -1;
						}
						if (kvp1.Key < kvp2.Key)
						{
							return 1;
						}
						return 0;
					});
					this.mLoserTeamOrder.Sort(delegate(KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2)
					{
						if (kvp1.Key > kvp2.Key)
						{
							return -1;
						}
						if (kvp1.Key < kvp2.Key)
						{
							return 1;
						}
						return 0;
					});
				}
				else
				{
					this.mVictorText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_DRAW));
					for (int l = 0; l < this.mPlayers.Length; l++)
					{
						if (this.mPlayers[l].Playing)
						{
							StatisticsManager.ScoreValues score6 = StatisticsManager.Instance.GetScore(l);
							this.mSortingOrder.Add(new KeyValuePair<int, int>((int)score6.Kills, l));
						}
					}
					this.mSortingOrder.Sort(delegate(KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2)
					{
						if (kvp1.Key > kvp2.Key)
						{
							return -1;
						}
						if (kvp1.Key < kvp2.Key)
						{
							return 1;
						}
						return 0;
					});
				}
			}
			else
			{
				int num2 = 0;
				while (num2 < scores.Length && num2 < this.mKillsTexts.Length)
				{
					if (this.mPlayers[num2].Playing)
					{
						this.mPlayerTexts[num2].SetText(this.mPlayers[num2].GamerTag);
						StatisticsManager.ScoreValues score7 = StatisticsManager.Instance.GetScore(num2);
						this.mKillsTexts[num2].SetText(score7.Kills.ToString());
						this.mDeathsTexts[num2].SetText(score7.Deaths.ToString());
						this.mSortingOrder.Add(new KeyValuePair<int, int>((int)score7.Kills, num2));
					}
					num2++;
				}
				this.mSortingOrder.Sort(delegate(KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2)
				{
					if (kvp1.Key > kvp2.Key)
					{
						return -1;
					}
					if (kvp1.Key < kvp2.Key)
					{
						return 1;
					}
					return 0;
				});
				int num3 = int.MinValue;
				this.mVictorPlayerIndex = -1;
				bool flag = false;
				for (int m = 0; m < scores.Length; m++)
				{
					if (this.mPlayers[m].Playing)
					{
						if ((int)scores[m] > num3)
						{
							num3 = (int)scores[m];
							this.mVictorPlayerIndex = m;
							flag = false;
						}
						else if (this.mVictorPlayerIndex != -1 && (int)scores[m] == num3)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					this.mVictorText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_DRAW));
					for (int n = 0; n < scores.Length; n++)
					{
						if (this.mPlayers[n].Playing && this.mPlayers[n].Gamer != null && !(this.mPlayers[n].Gamer is NetworkGamer))
						{
							this.mPlayers[n].Gamer.ResetVersusWinStreak();
							this.mPlayers[n].Gamer.VersusDefeats += 1U;
						}
					}
				}
				else
				{
					string text3 = LanguageManager.Instance.GetString(Defines.LOC_GAME_VS_WINNER);
					text3 = text3.Replace("#player;", this.mPlayers[this.mVictorPlayerIndex].GamerTag);
					this.mVictorText.SetText(text3);
					this.mVictorPlayerColor = (int)this.mPlayers[this.mVictorPlayerIndex].Color;
					for (int num4 = 0; num4 < scores.Length; num4++)
					{
						if (this.mPlayers[num4].Playing && this.mPlayers[num4].Gamer != null && !(this.mPlayers[num4].Gamer is NetworkGamer))
						{
							if (num4 == this.mVictorPlayerIndex)
							{
								this.mPlayers[num4].Gamer.VersusWins += 1U;
								this.mPlayers[num4].Gamer.IncrementVersusWinStreak(InGameMenu.sPlayState);
							}
							else
							{
								this.mPlayers[num4].Gamer.ResetVersusWinStreak();
								this.mPlayers[num4].Gamer.VersusDefeats += 1U;
							}
						}
					}
				}
			}
			if (this.mRuleset is DeathMatch)
			{
				this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_DM));
				return;
			}
			if (this.mRuleset is Brawl)
			{
				this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_BRAWL));
				return;
			}
			if (this.mRuleset is Krietor)
			{
				this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_TOURNEY));
				return;
			}
			if (this.mRuleset is King)
			{
				this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_KING));
				return;
			}
			if (this.mRuleset is Pyrite)
			{
				this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_PYRITE));
			}
		}

		// Token: 0x060030F5 RID: 12533 RVA: 0x001932FD File Offset: 0x001914FD
		protected override void OnExit()
		{
		}

		// Token: 0x04003501 RID: 13569
		private const string OPTION_RETRY = "retry";

		// Token: 0x04003502 RID: 13570
		private const string OPTION_QUIT = "quit";

		// Token: 0x04003503 RID: 13571
		private const string OPTION_DISCONNECT = "disconnect";

		// Token: 0x04003504 RID: 13572
		private const string OPTION_LOBBY = "lobby";

		// Token: 0x04003505 RID: 13573
		private const string OPTION_READY = "ready";

		// Token: 0x04003506 RID: 13574
		private static readonly string[] OPTION_STRINGS = new string[]
		{
			"retry",
			"quit",
			"lobby"
		};

		// Token: 0x04003507 RID: 13575
		private static InGameMenuVersusStatistics sSingelton;

		// Token: 0x04003508 RID: 13576
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04003509 RID: 13577
		private static readonly int LOC_VS_KING = "#versus_king".GetHashCodeCustom();

		// Token: 0x0400350A RID: 13578
		private static readonly int LOC_VS_DM = "#versus_dm".GetHashCodeCustom();

		// Token: 0x0400350B RID: 13579
		private static readonly int LOC_VS_BRAWL = "#versus_brawl".GetHashCodeCustom();

		// Token: 0x0400350C RID: 13580
		private static readonly int LOC_VS_PYRITE = "#versus_pyrite".GetHashCodeCustom();

		// Token: 0x0400350D RID: 13581
		private static readonly int LOC_VS_TOURNEY = "#versus_tourney".GetHashCodeCustom();

		// Token: 0x0400350E RID: 13582
		private static readonly int LOC_RETRY = "#add_menu_retry".GetHashCodeCustom();

		// Token: 0x0400350F RID: 13583
		private static readonly int LOC_READY = "#menu_charslct_02".GetHashCodeCustom();

		// Token: 0x04003510 RID: 13584
		private static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();

		// Token: 0x04003511 RID: 13585
		private static readonly int LOC_QUIT = "#add_menu_quit".GetHashCodeCustom();

		// Token: 0x04003512 RID: 13586
		private static readonly int LOC_LOBBY = "#menu_lobby".GetHashCodeCustom();

		// Token: 0x04003513 RID: 13587
		private static readonly int LOC_DRAW = "#menu_stat_draw".GetHashCodeCustom();

		// Token: 0x04003514 RID: 13588
		private BitmapFont mItemFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);

		// Token: 0x04003515 RID: 13589
		private Texture2D mPagesTexture;

		// Token: 0x04003516 RID: 13590
		private Text mModeText;

		// Token: 0x04003517 RID: 13591
		private Text mVictorText;

		// Token: 0x04003518 RID: 13592
		private Text mLoserTeamText;

		// Token: 0x04003519 RID: 13593
		private int mVictorPlayerIndex;

		// Token: 0x0400351A RID: 13594
		private int mVictorPlayerColor;

		// Token: 0x0400351B RID: 13595
		private List<KeyValuePair<int, int>> mSortingOrder;

		// Token: 0x0400351C RID: 13596
		private List<KeyValuePair<int, int>> mLoserTeamOrder;

		// Token: 0x0400351D RID: 13597
		private Text[] mPlayerTexts;

		// Token: 0x0400351E RID: 13598
		private Text[] mKillsTexts;

		// Token: 0x0400351F RID: 13599
		private Text[] mDeathsTexts;

		// Token: 0x04003520 RID: 13600
		private Player[] mPlayers;

		// Token: 0x04003521 RID: 13601
		private VersusRuleset mRuleset;

		// Token: 0x04003522 RID: 13602
		private bool mRedWin;

		// Token: 0x04003523 RID: 13603
		private bool mBlueWin;
	}
}
