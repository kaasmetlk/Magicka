using System;
using System.Collections.Generic;
using System.IO;
using Magicka.Achievements;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x02000579 RID: 1401
	internal class InGameMenuTimedObjectiveStatistics : InGameMenu
	{
		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x060029EE RID: 10734 RVA: 0x001491AC File Offset: 0x001473AC
		public static InGameMenuTimedObjectiveStatistics Instance
		{
			get
			{
				if (InGameMenuTimedObjectiveStatistics.sSingelton == null)
				{
					lock (InGameMenuTimedObjectiveStatistics.sSingeltonLock)
					{
						if (InGameMenuTimedObjectiveStatistics.sSingelton == null)
						{
							InGameMenuTimedObjectiveStatistics.sSingelton = new InGameMenuTimedObjectiveStatistics();
						}
					}
				}
				return InGameMenuTimedObjectiveStatistics.sSingelton;
			}
		}

		// Token: 0x060029EF RID: 10735 RVA: 0x00149200 File Offset: 0x00147400
		private InGameMenuTimedObjectiveStatistics()
		{
			this.mBackgroundSize = new Vector2(600f, 600f);
			this.AddMenuTextItem(InGameMenuTimedObjectiveStatistics.LOC_RETRY, this.mItemFont, TextAlign.Center);
			this.AddMenuTextItem(InGameMenuTimedObjectiveStatistics.LOC_DISCONNECT, this.mItemFont, TextAlign.Center);
			this.AddMenuTextItem(InGameMenuTimedObjectiveStatistics.LOC_QUIT, this.mItemFont, TextAlign.Center);
			this.mMissionComplete = new Text(50, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center, false);
			this.mTotalScoreText = new Text(50, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Center, false);
			this.mScoreText = new Text(50, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center, false);
			this.mMissionTimeText = new Text(50, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Center, false);
			this.mTimeText = new Text(50, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center, false);
			this.mMissionTimeBonusText = new Text(50, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Left, false);
			this.mBonusText = new Text(50, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Right, false);
			this.mFontHeight = (float)this.mTimeText.Font.LineHeight;
		}

		// Token: 0x060029F0 RID: 10736 RVA: 0x00149360 File Offset: 0x00147560
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

		// Token: 0x060029F1 RID: 10737 RVA: 0x001493B4 File Offset: 0x001475B4
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			base.IDraw(iDeltaTime, ref iBackgroundSize);
			Vector4 color = InGameMenu.sEffect.Color;
			InGameMenu.sEffect.Color = Vector4.One;
			Vector2 position;
			position.X = InGameMenu.sScreenSize.X * 0.5f;
			position.Y = InGameMenu.sScreenSize.Y * 0.5f - this.mBackgroundSize.Y * 0.5f + (float)this.mMissionComplete.Font.LineHeight * 2f;
			this.mMissionComplete.Draw(InGameMenu.sEffect, position.X, position.Y);
			position.Y += (float)this.mMissionComplete.Font.LineHeight;
			this.mTotalScoreText.Draw(InGameMenu.sEffect, position.X, position.Y);
			position.Y += (float)this.mTotalScoreText.Font.LineHeight;
			this.mScoreText.Draw(InGameMenu.sEffect, position.X, position.Y);
			position.Y += (float)this.mScoreText.Font.LineHeight;
			this.mMissionTimeText.Draw(InGameMenu.sEffect, position.X, position.Y);
			position.Y += (float)this.mMissionTimeText.Font.LineHeight;
			this.mTimeText.Draw(InGameMenu.sEffect, position.X, position.Y);
			position.Y += 1.5f * (float)this.mTimeText.Font.LineHeight;
			for (int i = 0; i < this.mObjectiveNames.Count; i++)
			{
				this.mObjectiveNames[i].Draw(InGameMenu.sEffect, position.X - this.mBackgroundSize.X * 0.5f + 48f, position.Y);
				this.mObjectiveValues[i].Draw(InGameMenu.sEffect, position.X + 128f, position.Y);
				this.mObjectiveScores[i].Draw(InGameMenu.sEffect, position.X + this.mBackgroundSize.X * 0.5f - 48f, position.Y);
				position.Y += (float)this.mObjectiveNames[i].Font.LineHeight;
			}
			position.Y += (float)this.mMissionTimeText.Font.LineHeight;
			this.mMissionTimeBonusText.Draw(InGameMenu.sEffect, position.X - this.mBackgroundSize.X * 0.5f + 48f, position.Y);
			this.mBonusText.Draw(InGameMenu.sEffect, position.X + this.mBackgroundSize.X * 0.5f - 48f, position.Y);
			position.Y += (float)this.mBonusText.Font.LineHeight * 2f;
			InGameMenu.sEffect.Color = color;
			NetworkState state = NetworkManager.Instance.State;
			for (int j = 0; j < this.mMenuItems.Count; j++)
			{
				if ((state != NetworkState.Client || j != 0) && (state != NetworkState.Offline || j != 1))
				{
					MenuItem menuItem = this.mMenuItems[j];
					menuItem.Position = position;
					menuItem.Draw(InGameMenu.sEffect);
					position.Y += menuItem.BottomRight.Y - menuItem.TopLeft.Y;
				}
			}
		}

		// Token: 0x060029F2 RID: 10738 RVA: 0x00149790 File Offset: 0x00147990
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

		// Token: 0x060029F3 RID: 10739 RVA: 0x00149942 File Offset: 0x00147B42
		protected override void IControllerBack(Controller iSender)
		{
		}

		// Token: 0x060029F4 RID: 10740 RVA: 0x00149944 File Offset: 0x00147B44
		protected override string IGetHighlightedButtonName()
		{
			string result = InGameMenuTimedObjectiveStatistics.OPTION_STRINGS[this.mSelectedItem];
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

		// Token: 0x060029F5 RID: 10741 RVA: 0x00149A10 File Offset: 0x00147C10
		protected override void OnEnter()
		{
			TimedObjectiveRuleset timedObjectiveRuleset = InGameMenu.sPlayState.Level.CurrentScene.RuleSet as TimedObjectiveRuleset;
			float timerValue = InGameMenu.sPlayState.Level.GetTimerValue(timedObjectiveRuleset.TimerID);
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)timerValue);
			if (timerValue >= 60f && timerValue < 3600f)
			{
				this.mTimeText.SetText(string.Format("0:{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds));
			}
			else if (timerValue >= 3600f)
			{
				this.mTimeText.SetText(string.Format("{0:0}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
			}
			else
			{
				this.mTimeText.SetText(string.Format("0:00:{0:00}", timeSpan.Seconds));
			}
			if (timedObjectiveRuleset.TimeSuccess)
			{
				this.mMissionComplete.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_MISSION_COMPLETE));
			}
			else
			{
				this.mMissionComplete.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_MISSION_FAILED));
			}
			this.mMissionTimeText.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_MISSION_TIME));
			List<TimedObjectiveRuleset.Objective> objectives = timedObjectiveRuleset.Objectives;
			this.mObjectiveNames.Clear();
			this.mObjectiveScores.Clear();
			this.mObjectiveValues.Clear();
			bool flag = true;
			int num = 0;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			for (int i = 0; i < objectives.Count; i++)
			{
				string @string = LanguageManager.Instance.GetString(objectives[i].NameID);
				int counterValue = InGameMenu.sPlayState.Level.GetCounterValue(objectives[i].CounterID);
				int num2 = counterValue * objectives[i].Score;
				num += num2;
				Text text = new Text(100, font, TextAlign.Left, false);
				text.SetText(@string);
				this.mObjectiveNames.Add(text);
				text = new Text(100, font, TextAlign.Right, false);
				if (objectives[i].MaxValue > 0)
				{
					text.SetText(string.Format("{0}/{1}", counterValue, objectives[i].MaxValue));
				}
				else
				{
					text.SetText("");
				}
				this.mObjectiveValues.Add(text);
				text = new Text(100, font, TextAlign.Right, false);
				text.SetText(num2.ToString());
				this.mObjectiveScores.Add(text);
				if (counterValue < objectives[i].MaxValue)
				{
					flag = false;
				}
			}
			if (flag)
			{
				AchievementsManager.Instance.AwardAchievement(InGameMenu.sPlayState, "goodcompany");
			}
			int num3 = (int)((float)num * timedObjectiveRuleset.GetBonusMultiplier);
			num += num3;
			this.mScoreText.SetText(num.ToString());
			this.mTotalScoreText.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_TSCORE));
			this.mMissionTimeBonusText.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_TIME_BONUS));
			this.mBonusText.SetText(num3.ToString());
			LeaderBoardData data = default(LeaderBoardData);
			data.Score = num;
			FloatIntConverter floatIntConverter = new FloatIntConverter(timerValue);
			data.Data1 = floatIntConverter.Int;
			Player[] players = Game.Instance.Players;
			string text2 = "";
			int j = 0;
			int num4 = 0;
			while (j < players.Length)
			{
				if (players[j].Playing)
				{
					text2 += players[j].GamerTag;
					num4++;
					if (num4 >= Game.Instance.PlayerCount)
					{
						break;
					}
					text2 += ", ";
				}
				j++;
			}
			data.Name = text2;
			string lvl = InGameMenu.sPlayState.Level.Name;
			Game.Instance.AddLoadTask(delegate
			{
				LevelNode[] challenges = LevelManager.Instance.Challenges;
				int k = 0;
				while (k < challenges.Length)
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(challenges[k].FileName);
					if (lvl.Equals(fileNameWithoutExtension, StringComparison.InvariantCultureIgnoreCase))
					{
						StatisticsManager.Instance.AddLocalEntry(k, data);
						if (HackHelper.LicenseStatus == HackHelper.Status.Valid && HackHelper.CheckLicense(challenges[k]) == HackHelper.License.Yes)
						{
							StatisticsManager.Instance.AddOnlineEntry(k, data);
							return;
						}
						break;
					}
					else
					{
						k++;
					}
				}
			});
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				this.mMenuItems[0].Enabled = false;
				this.mMenuItems[1].Enabled = true;
				this.mMenuItems[2].Enabled = true;
				(this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuTimedObjectiveStatistics.LOC_READY);
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mMenuItems[0].Enabled = false;
				this.mMenuItems[1].Enabled = true;
				this.mMenuItems[2].Enabled = false;
				(this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuTimedObjectiveStatistics.LOC_LOBBY);
				return;
			}
			this.mMenuItems[0].Enabled = true;
			this.mMenuItems[1].Enabled = false;
			this.mMenuItems[2].Enabled = true;
			(this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuTimedObjectiveStatistics.LOC_QUIT);
		}

		// Token: 0x060029F6 RID: 10742 RVA: 0x00149F2C File Offset: 0x0014812C
		protected override void OnExit()
		{
		}

		// Token: 0x04002D55 RID: 11605
		private const string OPTION_RETRY = "retry";

		// Token: 0x04002D56 RID: 11606
		private const string OPTION_QUIT = "quit";

		// Token: 0x04002D57 RID: 11607
		private const string OPTION_DISCONNECT = "disconnect";

		// Token: 0x04002D58 RID: 11608
		private const string OPTION_LOBBY = "lobby";

		// Token: 0x04002D59 RID: 11609
		private const string OPTION_READY = "ready";

		// Token: 0x04002D5A RID: 11610
		private static readonly string[] OPTION_STRINGS = new string[]
		{
			"retry",
			"quit",
			"lobby"
		};

		// Token: 0x04002D5B RID: 11611
		private static InGameMenuTimedObjectiveStatistics sSingelton;

		// Token: 0x04002D5C RID: 11612
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002D5D RID: 11613
		private static readonly int LOC_RETRY = "#add_menu_retry".GetHashCodeCustom();

		// Token: 0x04002D5E RID: 11614
		private static readonly int LOC_READY = "#menu_charslct_02".GetHashCodeCustom();

		// Token: 0x04002D5F RID: 11615
		private static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();

		// Token: 0x04002D60 RID: 11616
		private static readonly int LOC_QUIT = "#add_menu_quit".GetHashCodeCustom();

		// Token: 0x04002D61 RID: 11617
		private static readonly int LOC_LOBBY = "#menu_lobby".GetHashCodeCustom();

		// Token: 0x04002D62 RID: 11618
		private static readonly int LOC_TOTAL_SCORE = "#challenge_total_score".GetHashCodeCustom();

		// Token: 0x04002D63 RID: 11619
		private static readonly int LOC_MISSION_COMPLETE = "#menu_mission_complete".GetHashCodeCustom();

		// Token: 0x04002D64 RID: 11620
		private static readonly int LOC_MISSION_FAILED = "#menu_mission_failed".GetHashCodeCustom();

		// Token: 0x04002D65 RID: 11621
		private static readonly int LOC_MISSION_TIME = "#menu_mission_time".GetHashCodeCustom();

		// Token: 0x04002D66 RID: 11622
		private static readonly int LOC_TIME_BONUS = "#menu_mission_time".GetHashCodeCustom();

		// Token: 0x04002D67 RID: 11623
		private static readonly int LOC_TSCORE = "#challenge_total_score".GetHashCodeCustom();

		// Token: 0x04002D68 RID: 11624
		private List<Text> mObjectiveNames = new List<Text>();

		// Token: 0x04002D69 RID: 11625
		private List<Text> mObjectiveValues = new List<Text>();

		// Token: 0x04002D6A RID: 11626
		private List<Text> mObjectiveScores = new List<Text>();

		// Token: 0x04002D6B RID: 11627
		private float mFontHeight;

		// Token: 0x04002D6C RID: 11628
		private Text mMissionComplete;

		// Token: 0x04002D6D RID: 11629
		private Text mMissionTimeText;

		// Token: 0x04002D6E RID: 11630
		private Text mTimeText;

		// Token: 0x04002D6F RID: 11631
		private Text mTotalScoreText;

		// Token: 0x04002D70 RID: 11632
		private Text mScoreText;

		// Token: 0x04002D71 RID: 11633
		private Text mMissionTimeBonusText;

		// Token: 0x04002D72 RID: 11634
		private Text mBonusText;

		// Token: 0x04002D73 RID: 11635
		private BitmapFont mItemFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
	}
}
