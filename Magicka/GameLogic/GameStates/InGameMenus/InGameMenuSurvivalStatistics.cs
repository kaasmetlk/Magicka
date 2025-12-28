using System;
using System.IO;
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
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x0200029E RID: 670
	internal class InGameMenuSurvivalStatistics : InGameMenu
	{
		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06001410 RID: 5136 RVA: 0x0007CAC0 File Offset: 0x0007ACC0
		public static InGameMenuSurvivalStatistics Instance
		{
			get
			{
				if (InGameMenuSurvivalStatistics.sSingelton == null)
				{
					lock (InGameMenuSurvivalStatistics.sSingeltonLock)
					{
						if (InGameMenuSurvivalStatistics.sSingelton == null)
						{
							InGameMenuSurvivalStatistics.sSingelton = new InGameMenuSurvivalStatistics();
						}
					}
				}
				return InGameMenuSurvivalStatistics.sSingelton;
			}
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x0007CB14 File Offset: 0x0007AD14
		private InGameMenuSurvivalStatistics()
		{
			this.mBackgroundSize = new Vector2(600f, 600f);
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.MenuTitle);
			this.AddMenuTextItem(InGameMenuSurvivalStatistics.LOC_RETRY, font, TextAlign.Center);
			this.AddMenuTextItem(InGameMenuSurvivalStatistics.LOC_DISCONNECT, font, TextAlign.Center);
			this.AddMenuTextItem(InGameMenuSurvivalStatistics.LOC_QUIT, font, TextAlign.Center);
			this.mScoreTag = new Text(64, font, TextAlign.Center, false);
			this.mScore = new Text(64, font2, TextAlign.Center, false);
			this.mPlayers = new Text[4];
			this.mKills = new Text[4];
			this.mDeaths = new Text[4];
			for (int i = 0; i < 4; i++)
			{
				this.mPlayers[i] = new Text(64, font, TextAlign.Left, false);
				this.mKills[i] = new Text(64, font, TextAlign.Center, false);
				this.mDeaths[i] = new Text(64, font, TextAlign.Center, false);
			}
			lock (Game.Instance.GraphicsDevice)
			{
				this.mPagesTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
			}
			this.LanguageChanged();
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x0007CC4C File Offset: 0x0007AE4C
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mScoreTag.SetText(LanguageManager.Instance.GetString(InGameMenu.sPlayState.Level.DisplayName));
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x0007CC78 File Offset: 0x0007AE78
		protected override string IGetHighlightedButtonName()
		{
			string result = InGameMenuSurvivalStatistics.OPTION_STRINGS[this.mSelectedItem];
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

		// Token: 0x06001414 RID: 5140 RVA: 0x0007CCC0 File Offset: 0x0007AEC0
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

		// Token: 0x06001415 RID: 5141 RVA: 0x0007CE72 File Offset: 0x0007B072
		protected override void IControllerBack(Controller iSender)
		{
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x0007CE74 File Offset: 0x0007B074
		protected override void OnEnter()
		{
			SurvivalRuleset survivalRuleset = InGameMenu.sPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset;
			if (survivalRuleset != null)
			{
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < 4; i++)
				{
					if (players[i].Playing)
					{
						StatisticsManager.ScoreValues score = StatisticsManager.Instance.GetScore(i);
						this.mPlayers[i].SetText(players[i].GamerTag);
						this.mKills[i].SetText(score.Kills.ToString());
						this.mDeaths[i].SetText(score.Deaths.ToString());
					}
					else
					{
						this.mPlayers[i].SetText("");
						this.mKills[i].SetText("");
						this.mDeaths[i].SetText("");
					}
				}
				this.LanguageChanged();
				this.mTargetScore = StatisticsManager.Instance.SurvivalTotalScore;
				this.mScoreCount = 0;
				this.mScore.SetText(this.mScoreCount.ToString());
				this.mDelay = 0f;
				this.AddHighScore();
			}
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				this.mMenuItems[0].Enabled = false;
				this.mMenuItems[1].Enabled = true;
				this.mMenuItems[2].Enabled = true;
				(this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuSurvivalStatistics.LOC_READY);
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mMenuItems[0].Enabled = false;
				this.mMenuItems[1].Enabled = true;
				this.mMenuItems[2].Enabled = false;
				(this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuSurvivalStatistics.LOC_LOBBY);
				return;
			}
			this.mMenuItems[0].Enabled = true;
			this.mMenuItems[1].Enabled = false;
			this.mMenuItems[2].Enabled = true;
			(this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuSurvivalStatistics.LOC_QUIT);
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x0007D12C File Offset: 0x0007B32C
		private void AddHighScore()
		{
			SurvivalRuleset survivalRuleset = InGameMenu.sPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset;
			if (survivalRuleset != null)
			{
				LeaderBoardData data = default(LeaderBoardData);
				data.Score = StatisticsManager.Instance.SurvivalTotalScore;
				data.Data1 = (int)((byte)survivalRuleset.WaveIndex);
				if (data.Score == 0)
				{
					return;
				}
				Player[] players = Game.Instance.Players;
				string text = "";
				int i = 0;
				int num = 0;
				while (i < players.Length)
				{
					if (players[i].Playing)
					{
						text += players[i].GamerTag;
						num++;
						if (num >= Game.Instance.PlayerCount)
						{
							break;
						}
						text += ", ";
					}
					i++;
				}
				data.Name = text;
				string lvl = InGameMenu.sPlayState.Level.Name;
				Game.Instance.AddLoadTask(delegate
				{
					LevelNode[] challenges = LevelManager.Instance.Challenges;
					int j = 0;
					while (j < challenges.Length)
					{
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(challenges[j].FileName);
						if (lvl.Equals(fileNameWithoutExtension, StringComparison.InvariantCultureIgnoreCase))
						{
							StatisticsManager.Instance.AddLocalEntry(j, data);
							if (HackHelper.LicenseStatus == HackHelper.Status.Valid && HackHelper.CheckLicense(challenges[j]) == HackHelper.License.Yes)
							{
								StatisticsManager.Instance.AddOnlineEntry(j, data);
								return;
							}
							break;
						}
						else
						{
							j++;
						}
					}
				});
			}
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x0007D23C File Offset: 0x0007B43C
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

		// Token: 0x06001419 RID: 5145 RVA: 0x0007D290 File Offset: 0x0007B490
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			this.mDelay += iDeltaTime;
			if (this.mScoreCount < this.mTargetScore)
			{
				this.mScoreCount = (int)((float)this.mTargetScore * (this.mDelay / 1f));
				if (this.mScoreCount > this.mTargetScore)
				{
					this.mScoreCount = this.mTargetScore;
				}
				this.mScore.SetText(this.mScoreCount.ToString());
			}
			else if (this.mScoreCount > this.mTargetScore)
			{
				this.mScoreCount = this.mTargetScore;
				this.mScore.SetText(this.mScoreCount.ToString());
			}
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = 1f));
			color.W = this.mAlpha;
			Vector4 colorSelected = default(Vector4);
			colorSelected.X = (colorSelected.Y = (colorSelected.Z = 0f));
			colorSelected.W = this.mAlpha;
			Vector4 colorDisabled = default(Vector4);
			colorDisabled.X = (colorDisabled.Y = (colorDisabled.Z = 0.4f));
			colorDisabled.W = this.mAlpha;
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				MenuItem menuItem = this.mMenuItems[i];
				menuItem.Color = color;
				menuItem.ColorSelected = colorSelected;
				menuItem.ColorDisabled = colorDisabled;
				menuItem.Selected = (menuItem.Enabled & this.mSelectedItem == i);
				if (menuItem.Selected)
				{
					Matrix transform = default(Matrix);
					transform.M44 = 1f;
					transform.M11 = 200f * InGameMenu.sScale;
					transform.M22 = menuItem.BottomRight.Y - menuItem.TopLeft.Y;
					transform.M41 = menuItem.Position.X - 100f * InGameMenu.sScale;
					transform.M42 = menuItem.TopLeft.Y;
					InGameMenu.sEffect.Transform = transform;
					Vector4 color2 = default(Vector4);
					color2.X = (color2.Y = (color2.Z = 1f));
					color2.W = 0.8f * this.mAlpha;
					InGameMenu.sEffect.Color = color2;
					InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
					InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
					InGameMenu.sEffect.CommitChanges();
					InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				}
			}
			Vector2 position = default(Vector2);
			position.Y = InGameMenu.sScreenSize.Y * 0.5f - 220f * InGameMenu.sScale;
			position.X = InGameMenu.sScreenSize.X * 0.5f;
			InGameMenu.sEffect.Color = Vector4.One;
			this.mScoreTag.Draw(InGameMenu.sEffect, position.X, position.Y, InGameMenu.sScale);
			position.Y += 20f * InGameMenu.sScale;
			this.mScore.Draw(InGameMenu.sEffect, position.X, position.Y, InGameMenu.sScale);
			position.X = InGameMenu.sScreenSize.X * 0.5f + 74f * InGameMenu.sScale;
			position.Y += 80f * InGameMenu.sScale;
			base.DrawGraphics(this.mPagesTexture, new Rectangle(1024, 64, 64, 64), new Vector4(position.X, position.Y, 32f * InGameMenu.sScale, 32f * InGameMenu.sScale));
			position.X = InGameMenu.sScreenSize.X * 0.5f + 174f * InGameMenu.sScale;
			base.DrawGraphics(this.mPagesTexture, new Rectangle(1024, 0, 64, 64), new Vector4(position.X, position.Y, 32f * InGameMenu.sScale, 32f * InGameMenu.sScale));
			position.X = InGameMenu.sScreenSize.X * 0.5f - 210f * InGameMenu.sScale;
			position.Y += 48f * InGameMenu.sScale;
			for (int j = 0; j < 4; j++)
			{
				this.mPlayers[j].Draw(InGameMenu.sEffect, position.X, position.Y);
				this.mKills[j].Draw(InGameMenu.sEffect, position.X + 300f * InGameMenu.sScale, position.Y);
				this.mDeaths[j].Draw(InGameMenu.sEffect, position.X + 400f * InGameMenu.sScale, position.Y);
				position.Y += 32f * InGameMenu.sScale;
			}
			position.X = InGameMenu.sScreenSize.X * 0.5f;
			position.Y = InGameMenu.sScreenSize.Y * 0.5f + 210f - 64f;
			NetworkState state = NetworkManager.Instance.State;
			for (int k = 0; k < this.mMenuItems.Count; k++)
			{
				if ((state != NetworkState.Client || k != 0) && (state != NetworkState.Offline || k != 1))
				{
					MenuItem menuItem2 = this.mMenuItems[k];
					menuItem2.Position = position;
					menuItem2.Draw(InGameMenu.sEffect);
					position.Y += menuItem2.BottomRight.Y - menuItem2.TopLeft.Y;
				}
			}
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x0007D8A2 File Offset: 0x0007BAA2
		protected override void OnExit()
		{
		}

		// Token: 0x04001582 RID: 5506
		private const string OPTION_RETRY = "retry";

		// Token: 0x04001583 RID: 5507
		private const string OPTION_QUIT = "quit";

		// Token: 0x04001584 RID: 5508
		private const string OPTION_DISCONNECT = "disconnect";

		// Token: 0x04001585 RID: 5509
		private const string OPTION_LOBBY = "lobby";

		// Token: 0x04001586 RID: 5510
		private const string OPTION_READY = "ready";

		// Token: 0x04001587 RID: 5511
		private static readonly string[] OPTION_STRINGS = new string[]
		{
			"retry",
			"quit",
			"lobby"
		};

		// Token: 0x04001588 RID: 5512
		private static InGameMenuSurvivalStatistics sSingelton;

		// Token: 0x04001589 RID: 5513
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400158A RID: 5514
		private static readonly int LOC_RETRY = "#add_menu_retry".GetHashCodeCustom();

		// Token: 0x0400158B RID: 5515
		private static readonly int LOC_READY = "#menu_charslct_02".GetHashCodeCustom();

		// Token: 0x0400158C RID: 5516
		private static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();

		// Token: 0x0400158D RID: 5517
		private static readonly int LOC_QUIT = "#add_menu_quit".GetHashCodeCustom();

		// Token: 0x0400158E RID: 5518
		private static readonly int LOC_LOBBY = "#menu_lobby".GetHashCodeCustom();

		// Token: 0x0400158F RID: 5519
		private static readonly int LOC_SCORE = "#lb_score".GetHashCodeCustom();

		// Token: 0x04001590 RID: 5520
		private Texture2D mPagesTexture;

		// Token: 0x04001591 RID: 5521
		private Text mScore;

		// Token: 0x04001592 RID: 5522
		private Text[] mPlayers;

		// Token: 0x04001593 RID: 5523
		private Text[] mKills;

		// Token: 0x04001594 RID: 5524
		private Text[] mDeaths;

		// Token: 0x04001595 RID: 5525
		private Text mScoreTag;

		// Token: 0x04001596 RID: 5526
		private int mScoreCount;

		// Token: 0x04001597 RID: 5527
		private int mTargetScore;

		// Token: 0x04001598 RID: 5528
		private float mDelay;
	}
}
