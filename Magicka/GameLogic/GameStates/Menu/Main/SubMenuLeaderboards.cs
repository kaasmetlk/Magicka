using System;
using System.Collections.Generic;
using System.Text;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using SteamWrapper;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x02000507 RID: 1287
	internal class SubMenuLeaderboards : SubMenu
	{
		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x06002665 RID: 9829 RVA: 0x00115BD0 File Offset: 0x00113DD0
		public static SubMenuLeaderboards Instance
		{
			get
			{
				if (SubMenuLeaderboards.mSingelton == null)
				{
					lock (SubMenuLeaderboards.mSingeltonLock)
					{
						if (SubMenuLeaderboards.mSingelton == null)
						{
							SubMenuLeaderboards.mSingelton = new SubMenuLeaderboards();
						}
					}
				}
				return SubMenuLeaderboards.mSingelton;
			}
		}

		// Token: 0x06002666 RID: 9830 RVA: 0x00115C24 File Offset: 0x00113E24
		public SubMenuLeaderboards()
		{
			this.mSteamLeaderboards = StatisticsManager.Instance.SteamLeaderboards;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_C.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				this.mVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_C);
				this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
			}
			this.mRankStringBuilder = new StringBuilder(512);
			this.mNameStringBuilder = new StringBuilder(512);
			this.mScoreStringBuilder = new StringBuilder(512);
			this.mDataStringBuilder = new StringBuilder(512);
			this.mTextureAlpha = 0f;
			this.mCurrentTopRank = 0;
			this.mSelectedPosition = 0;
			this.mShowLocalBoards = true;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			this.mHeadLineHeight = (float)font.LineHeight;
			this.mRanksHeader = new Text(64, font, TextAlign.Left, false);
			this.mNamesHeader = new Text(64, font, TextAlign.Left, false);
			this.mScoresHeader = new Text(64, font, TextAlign.Right, false);
			this.mWavesHeader = new Text(64, font, TextAlign.Left, false);
			this.mRanksHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_RANK));
			this.mNamesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_NAME));
			this.mWavesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_WAVES));
			this.mScoresHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_SCORE));
			font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			this.mLineHeight = (float)font.LineHeight;
			this.mRankColumn = new Text(1024, font, TextAlign.Left, false);
			this.mNameColumn = new Text(1024, font, TextAlign.Left, false);
			this.mScoreColumn = new Text(1024, font, TextAlign.Right, false);
			this.mWavesColumn = new Text(1024, font, TextAlign.Left, false);
			this.mMenuItems = new List<MenuItem>();
			this.mMenuTitle = new Text(30, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_LEADERBOARDS));
			VertexPositionTexture[] array = new VertexPositionTexture[Defines.QUAD_TEX_VERTS_C.Length];
			Defines.QUAD_TEX_VERTS_C.CopyTo(array, 0);
			array[0].TextureCoordinate.X = 1344f / (float)SubMenu.sPagesTexture.Width;
			array[0].TextureCoordinate.Y = 160f / (float)SubMenu.sPagesTexture.Height;
			array[1].TextureCoordinate.X = 1280f / (float)SubMenu.sPagesTexture.Width;
			array[1].TextureCoordinate.Y = 160f / (float)SubMenu.sPagesTexture.Height;
			array[2].TextureCoordinate.X = 1280f / (float)SubMenu.sPagesTexture.Width;
			array[2].TextureCoordinate.Y = 96f / (float)SubMenu.sPagesTexture.Height;
			array[3].TextureCoordinate.X = 1344f / (float)SubMenu.sPagesTexture.Width;
			array[3].TextureCoordinate.Y = 96f / (float)SubMenu.sPagesTexture.Height;
			VertexBuffer vertexBuffer;
			VertexDeclaration iDeclaration;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				vertexBuffer.SetData<VertexPositionTexture>(array);
				iDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			}
			this.mMenuItems.Add(new MenuTextItem(SubMenuLeaderboards.LOC_DEBUG, SubMenuLeaderboards.TITLE_POSITION, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center));
			this.mMenuItems.Add(new MenuTextItem(SubMenuLeaderboards.LOC_LOCAL, SubMenuLeaderboards.ONLINE_POSITION, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center));
			this.mListRightArrow = new MenuImageItem(new Vector2(this.mPosition.X + 128f, 896f), SubMenu.sPagesTexture, vertexBuffer, iDeclaration, -1.5707964f, 0, VertexPositionTexture.SizeInBytes, 64f, 64f);
			this.mListLeftArrow = new MenuImageItem(new Vector2(this.mPosition.X - 128f, 896f), SubMenu.sPagesTexture, vertexBuffer, iDeclaration, 1.5707964f, 0, VertexPositionTexture.SizeInBytes, 64f, 64f);
			this.mListTop = new Text(10, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Left, false);
			this.mListTop.SetText("1");
			this.mListTop.DefaultColor = MenuItem.COLOR;
			this.mListBottom = new Text(10, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Right, false);
			this.mListBottom.SetText(8.ToString());
			this.mListBottom.DefaultColor = MenuItem.COLOR;
			Vector2 iPosition = SubMenuLeaderboards.CHALLENGE_POSITION + new Vector2(SubMenuLeaderboards.CHALLENGE_SIZE.X + 16f, SubMenuLeaderboards.CHALLENGE_SIZE.Y * 0.5f);
			this.mLevelRightArrow = new MenuImageItem(iPosition, SubMenu.sPagesTexture, vertexBuffer, iDeclaration, -1.5707964f, 0, VertexPositionTexture.SizeInBytes, 64f, 64f);
			iPosition = SubMenuLeaderboards.CHALLENGE_POSITION + new Vector2(-16f, SubMenuLeaderboards.CHALLENGE_SIZE.Y * 0.5f);
			this.mLevelLeftArrow = new MenuImageItem(iPosition, SubMenu.sPagesTexture, vertexBuffer, iDeclaration, 1.5707964f, 0, VertexPositionTexture.SizeInBytes, 64f, 64f);
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
		}

		// Token: 0x06002667 RID: 9831 RVA: 0x00116254 File Offset: 0x00114454
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mRanksHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_RANK));
			this.mNamesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_NAME));
			if (LevelManager.Instance.Challenges[this.mChallenge].FileName.Equals("ch_vietnam", StringComparison.OrdinalIgnoreCase))
			{
				this.mWavesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_TITLE_TIME));
			}
			else
			{
				this.mWavesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_WAVES));
			}
			this.mScoresHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_SCORE));
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_LEADERBOARDS));
			this.UpdateScores();
			this.ChangeLeaderboard();
		}

		// Token: 0x06002668 RID: 9832 RVA: 0x00116334 File Offset: 0x00114534
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.mEffect.VertexColorEnabled = false;
			this.mEffect.Color = ((this.mSelection == SubMenuLeaderboards.Selections.List) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			this.mRanksHeader.Draw(this.mEffect, SubMenuLeaderboards.RANKS_POSITION.X, SubMenuLeaderboards.RANKS_POSITION.Y);
			this.mNamesHeader.Draw(this.mEffect, SubMenuLeaderboards.NAMES_POSITION.X, SubMenuLeaderboards.NAMES_POSITION.Y);
			this.mWavesHeader.Draw(this.mEffect, SubMenuLeaderboards.WAVES_POSITION.X, SubMenuLeaderboards.WAVES_POSITION.Y);
			this.mScoresHeader.Draw(this.mEffect, SubMenuLeaderboards.SCORES_POSITION.X, SubMenuLeaderboards.SCORES_POSITION.Y);
			this.mRankColumn.Draw(this.mEffect, SubMenuLeaderboards.RANKS_POSITION.X + 8f, SubMenuLeaderboards.RANKS_POSITION.Y + this.mHeadLineHeight);
			this.mNameColumn.Draw(this.mEffect, SubMenuLeaderboards.NAMES_POSITION.X, SubMenuLeaderboards.NAMES_POSITION.Y + this.mHeadLineHeight);
			this.mWavesColumn.Draw(this.mEffect, SubMenuLeaderboards.WAVES_POSITION.X, SubMenuLeaderboards.WAVES_POSITION.Y + this.mHeadLineHeight);
			this.mScoreColumn.Draw(this.mEffect, SubMenuLeaderboards.SCORES_POSITION.X, SubMenuLeaderboards.SCORES_POSITION.Y + this.mHeadLineHeight);
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
			this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			this.mEffect.VertexColorEnabled = true;
			this.mEffect.TextureEnabled = false;
			this.mEffect.Color = Vector4.One;
			if (!this.mShowLocalBoards)
			{
				this.mListRightArrow.Draw(this.mEffect, 64f);
				this.mListLeftArrow.Draw(this.mEffect, 64f);
				this.mEffect.VertexColorEnabled = false;
				this.mEffect.Color = ((this.mSelection == SubMenuLeaderboards.Selections.List) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
				Vector2 position = this.mListRightArrow.Position;
				position.Y -= (float)this.mListBottom.Font.LineHeight * 0.5f;
				position.X -= 32f;
				this.mListBottom.Draw(this.mEffect, position.X, position.Y);
				position = this.mListLeftArrow.Position;
				position.Y -= (float)this.mListBottom.Font.LineHeight * 0.5f;
				position.X += 32f;
				this.mListTop.Draw(this.mEffect, position.X, position.Y);
				this.mEffect.Color = Vector4.One;
			}
			this.mLevelRightArrow.Draw(this.mEffect, 64f);
			this.mLevelLeftArrow.Draw(this.mEffect, 64f);
			this.mEffect.VertexColorEnabled = false;
			this.mEffect.Color = new Vector4(0f, 0f, 0f, 0.8f);
			this.mEffect.CommitChanges();
			this.mMenuTitle.Draw(this.mEffect, this.mPosition.X, 96f);
			foreach (MenuItem menuItem in this.mMenuItems)
			{
				menuItem.Draw(this.mEffect);
			}
			if (this.mCurrentTexture != null && !this.mCurrentTexture.IsDisposed)
			{
				lock (this.mCurrentTexture)
				{
					this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
					this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
					Matrix identity = Matrix.Identity;
					identity.M11 = SubMenuLeaderboards.CHALLENGE_SIZE.X;
					identity.M22 = SubMenuLeaderboards.CHALLENGE_SIZE.Y;
					identity.M41 = SubMenuLeaderboards.CHALLENGE_POSITION.X + SubMenuLeaderboards.CHALLENGE_SIZE.X * 0.5f;
					identity.M42 = SubMenuLeaderboards.CHALLENGE_POSITION.Y + SubMenuLeaderboards.CHALLENGE_SIZE.Y * 0.5f;
					this.mEffect.Color = new Vector4(1f, 1f, 1f, this.mTextureAlpha);
					this.mEffect.Transform = identity;
					this.mEffect.VertexColorEnabled = false;
					this.mEffect.Texture = this.mCurrentTexture;
					this.mEffect.TextureEnabled = true;
					this.mEffect.TextureOffset = Vector2.Zero;
					this.mEffect.TextureScale = Vector2.One;
					this.mEffect.Saturation = 1f;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				}
			}
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06002669 RID: 9833 RVA: 0x0011694C File Offset: 0x00114B4C
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mCurrentTexture != null && !this.mCurrentTexture.IsDisposed)
			{
				this.mTextureAlpha = Math.Min(this.mTextureAlpha + iDeltaTime * 4f, 1f);
			}
		}

		// Token: 0x0600266A RID: 9834 RVA: 0x00116984 File Offset: 0x00114B84
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (iState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
			{
				return;
			}
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag) && flag)
			{
				this.mSelectedPosition = -1;
				this.mMenuItems[0].Selected = false;
				this.mMenuItems[1].Selected = false;
				this.mMenuItems[2].Selected = false;
				if (!this.mShowLocalBoards && this.mListRightArrow.InsideBounds(ref vector))
				{
					int num = Math.Max(0, this.mCurrentTopRank + 8);
					if (this.mShowLocalBoards)
					{
						if (StatisticsManager.Instance.Leaderboard(this.mChallenge).Count > num)
						{
							this.mCurrentTopRank = num;
							this.UpdateScores();
						}
					}
					else if (this.mSteamLeaderboards.Count > this.mChallenge && SteamUserStats.GetLeaderboardEntryCount(this.mSteamLeaderboards[this.mChallenge]) > num)
					{
						this.mCurrentTopRank = num;
						this.UpdateScores();
					}
				}
				else if (!this.mShowLocalBoards && this.mListLeftArrow.InsideBounds(ref vector) && this.mCurrentTopRank > 0)
				{
					int num2 = Math.Max(0, this.mCurrentTopRank - 8);
					if (this.mShowLocalBoards)
					{
						if (StatisticsManager.Instance.Leaderboard(this.mChallenge).Count > num2)
						{
							this.mCurrentTopRank = num2;
							this.UpdateScores();
						}
					}
					else if (this.mSteamLeaderboards.Count > this.mChallenge && SteamUserStats.GetLeaderboardEntryCount(this.mSteamLeaderboards[this.mChallenge]) > num2)
					{
						this.mCurrentTopRank = num2;
						this.UpdateScores();
					}
				}
				else if (this.mLevelRightArrow.InsideBounds(ref vector))
				{
					this.mSelectedPosition = 0;
					this.mCurrentTopRank = 0;
					int num3 = this.mChallenge - 1;
					if (num3 < 0)
					{
						num3 = LevelManager.Instance.Challenges.Length - 1;
					}
					this.mChallenge = num3;
					this.ChangeLeaderboard();
					this.mMenuItems[0].Selected = false;
				}
				else if (this.mLevelLeftArrow.InsideBounds(ref vector))
				{
					this.mSelectedPosition = 0;
					this.mCurrentTopRank = 0;
					int num4 = this.mChallenge + 1;
					if (num4 >= LevelManager.Instance.Challenges.Length)
					{
						num4 = 0;
					}
					this.mChallenge = num4;
					this.ChangeLeaderboard();
					this.mMenuItems[0].Selected = false;
				}
				else if (this.mMenuItems[this.mMenuItems.Count - 2].InsideBounds(ref vector))
				{
					this.mSelectedPosition = 1;
					this.mShowLocalBoards = !this.mShowLocalBoards;
					if (this.mShowLocalBoards)
					{
						(this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_LOCAL));
					}
					else
					{
						(this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_ONLINE));
					}
					this.mCurrentTopRank = 0;
					this.ChangeLeaderboard();
					this.mMenuItems[1].Selected = true;
				}
				else if (this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref vector))
				{
					Tome.Instance.PopMenu();
				}
			}
			this.mSelection = SubMenuLeaderboards.Selections.None;
		}

		// Token: 0x0600266B RID: 9835 RVA: 0x00116CFC File Offset: 0x00114EFC
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mMenuItems == null || this.mMenuItems.Count == 0)
			{
				return;
			}
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag))
			{
				if (flag)
				{
					bool flag2 = false;
					for (int i = 0; i < this.mMenuItems.Count; i++)
					{
						MenuItem menuItem = this.mMenuItems[i];
						if (menuItem.Enabled && menuItem.InsideBounds(ref vector))
						{
							if (this.mSelectedPosition != i)
							{
								AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
							}
							this.mKeyboardSelection = false;
							this.mSelectedPosition = i;
							for (int j = 0; j < this.mMenuItems.Count; j++)
							{
								this.mMenuItems[j].Selected = (j == i);
							}
							flag2 = (i != 0);
							break;
						}
					}
					if (!flag2)
					{
						for (int k = 0; k < this.mMenuItems.Count; k++)
						{
							this.mMenuItems[k].Selected = false;
						}
						this.mSelectedPosition = -1;
						return;
					}
				}
			}
			else if (!this.mKeyboardSelection & this.mMenuItems != null)
			{
				for (int l = 0; l < this.mMenuItems.Count; l++)
				{
					this.mMenuItems[l].Selected = false;
				}
			}
		}

		// Token: 0x0600266C RID: 9836 RVA: 0x00116E68 File Offset: 0x00115068
		public override void ControllerLeft(Controller iSender)
		{
			this.mMenuItems[0].Selected = false;
			this.mMenuItems[1].Selected = false;
			this.mMenuItems[2].Selected = false;
			switch (this.mSelection)
			{
			default:
				this.mMenuItems[2].Selected = true;
				return;
			case SubMenuLeaderboards.Selections.LocalOnline:
				this.mShowLocalBoards = !this.mShowLocalBoards;
				if (this.mShowLocalBoards)
				{
					(this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_LOCAL));
				}
				else
				{
					(this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_ONLINE));
				}
				this.mMenuItems[1].Selected = true;
				this.mCurrentTopRank = 0;
				this.ChangeLeaderboard();
				return;
			case SubMenuLeaderboards.Selections.List:
			{
				int num = Math.Max(0, this.mCurrentTopRank - 8);
				if (this.mShowLocalBoards)
				{
					if (StatisticsManager.Instance.Leaderboard(this.mChallenge).Count > num)
					{
						this.mCurrentTopRank = num;
						this.UpdateScores();
						return;
					}
				}
				else if (this.mSteamLeaderboards.Count > this.mChallenge && SteamUserStats.GetLeaderboardEntryCount(this.mSteamLeaderboards[this.mChallenge]) > num)
				{
					this.mCurrentTopRank = num;
					this.UpdateScores();
					return;
				}
				break;
			}
			case SubMenuLeaderboards.Selections.Level:
			{
				this.mCurrentTopRank = 0;
				int num2 = this.mChallenge + 1;
				if (num2 >= LevelManager.Instance.Challenges.Length)
				{
					num2 = 0;
				}
				this.mChallenge = num2;
				this.ChangeLeaderboard();
				this.mMenuItems[0].Selected = !(iSender is KeyboardMouseController);
				break;
			}
			}
		}

		// Token: 0x0600266D RID: 9837 RVA: 0x00117028 File Offset: 0x00115228
		public override void ControllerRight(Controller iSender)
		{
			this.mMenuItems[0].Selected = false;
			this.mMenuItems[1].Selected = false;
			this.mMenuItems[2].Selected = false;
			switch (this.mSelection)
			{
			default:
				this.mMenuItems[2].Selected = true;
				return;
			case SubMenuLeaderboards.Selections.LocalOnline:
				this.mShowLocalBoards = !this.mShowLocalBoards;
				if (this.mShowLocalBoards)
				{
					(this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_LOCAL));
				}
				else
				{
					(this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_ONLINE));
				}
				this.mCurrentTopRank = 0;
				this.ChangeLeaderboard();
				this.mMenuItems[1].Selected = true;
				return;
			case SubMenuLeaderboards.Selections.List:
			{
				int num = Math.Max(0, this.mCurrentTopRank + 8);
				if (this.mShowLocalBoards)
				{
					if (StatisticsManager.Instance.Leaderboard(this.mChallenge).Count > num)
					{
						this.mCurrentTopRank = num;
						this.UpdateScores();
						return;
					}
				}
				else if (this.mSteamLeaderboards.Count > this.mChallenge && SteamUserStats.GetLeaderboardEntryCount(this.mSteamLeaderboards[this.mChallenge]) > num)
				{
					this.mCurrentTopRank = num;
					this.UpdateScores();
					return;
				}
				break;
			}
			case SubMenuLeaderboards.Selections.Level:
			{
				this.mCurrentTopRank = 0;
				int num2 = this.mChallenge - 1;
				if (num2 < 0)
				{
					num2 = LevelManager.Instance.Challenges.Length - 1;
				}
				this.mChallenge = num2;
				this.ChangeLeaderboard();
				this.mMenuItems[0].Selected = !(iSender is KeyboardMouseController);
				break;
			}
			}
		}

		// Token: 0x0600266E RID: 9838 RVA: 0x001171E8 File Offset: 0x001153E8
		public override void ControllerUp(Controller iSender)
		{
			this.mMenuItems[0].Selected = false;
			this.mMenuItems[1].Selected = false;
			this.mMenuItems[2].Selected = false;
			switch (this.mSelection)
			{
			default:
				this.mSelection = SubMenuLeaderboards.Selections.LocalOnline;
				this.mMenuItems[1].Selected = true;
				return;
			case SubMenuLeaderboards.Selections.LocalOnline:
				if (!this.mShowLocalBoards)
				{
					this.mSelection = SubMenuLeaderboards.Selections.List;
					return;
				}
				this.mSelection = SubMenuLeaderboards.Selections.Level;
				this.mMenuItems[0].Selected = true;
				return;
			case SubMenuLeaderboards.Selections.List:
				this.mSelection = SubMenuLeaderboards.Selections.Level;
				this.mMenuItems[0].Selected = true;
				return;
			case SubMenuLeaderboards.Selections.Level:
				this.mSelection = SubMenuLeaderboards.Selections.Back;
				this.mMenuItems[2].Selected = true;
				return;
			}
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x001172C4 File Offset: 0x001154C4
		public override void ControllerDown(Controller iSender)
		{
			this.mMenuItems[0].Selected = false;
			this.mMenuItems[1].Selected = false;
			this.mMenuItems[2].Selected = false;
			switch (this.mSelection)
			{
			default:
				this.mSelection = SubMenuLeaderboards.Selections.Level;
				this.mMenuItems[0].Selected = true;
				return;
			case SubMenuLeaderboards.Selections.LocalOnline:
				this.mSelection = SubMenuLeaderboards.Selections.Back;
				this.mMenuItems[2].Selected = true;
				return;
			case SubMenuLeaderboards.Selections.List:
				this.mSelection = SubMenuLeaderboards.Selections.LocalOnline;
				this.mMenuItems[1].Selected = true;
				return;
			case SubMenuLeaderboards.Selections.Level:
				if (!this.mShowLocalBoards)
				{
					this.mSelection = SubMenuLeaderboards.Selections.List;
					return;
				}
				this.mSelection = SubMenuLeaderboards.Selections.LocalOnline;
				this.mMenuItems[1].Selected = true;
				return;
			}
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x001173A0 File Offset: 0x001155A0
		public override void ControllerA(Controller iSender)
		{
			switch (this.mSelection)
			{
			case SubMenuLeaderboards.Selections.Back:
				Tome.Instance.PopMenu();
				return;
			}
			this.ControllerRight(iSender);
		}

		// Token: 0x06002671 RID: 9841 RVA: 0x001173E0 File Offset: 0x001155E0
		private void LoadTexture()
		{
			if (this.mCurrentTexture != null)
			{
				lock (this.mCurrentTexture)
				{
					if (this.mCurrentContent == null)
					{
						this.mCurrentContent = new ContentManager(Game.Instance.Content.ServiceProvider, Game.Instance.Content.RootDirectory);
					}
					else
					{
						this.mCurrentContent.Unload();
					}
					this.mCurrentTexture = this.mCurrentContent.Load<Texture2D>(this.mCurrentTexturePath);
					return;
				}
			}
			if (this.mCurrentContent == null)
			{
				this.mCurrentContent = new ContentManager(Game.Instance.Content.ServiceProvider, Game.Instance.Content.RootDirectory);
			}
			else
			{
				this.mCurrentContent.Unload();
			}
			this.mCurrentTexture = this.mCurrentContent.Load<Texture2D>(this.mCurrentTexturePath);
		}

		// Token: 0x06002672 RID: 9842 RVA: 0x001174C8 File Offset: 0x001156C8
		private void ChangeLeaderboard()
		{
			string text = this.mCurrentTexturePath;
			LevelNode levelNode = LevelManager.Instance.Challenges[this.mChallenge];
			(this.mMenuItems[0] as MenuTextItem).SetText(LanguageManager.Instance.GetString(levelNode.Name.GetHashCodeCustom()));
			this.mCurrentTexturePath = this.mTexturePath.Replace("#1;", levelNode.LoadingImage);
			if (LevelManager.Instance.Challenges[this.mChallenge].FileName.Equals("ch_vietnam", StringComparison.OrdinalIgnoreCase))
			{
				string @string = LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_TITLE_TIME);
				this.mWavesHeader.SetText(@string);
			}
			else
			{
				this.mWavesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_WAVES));
			}
			if (text == null || !text.Equals(this.mCurrentTexturePath, StringComparison.InvariantCultureIgnoreCase))
			{
				this.mTextureAlpha = 0f;
				Game.Instance.AddLoadTask(new Action(this.LoadTexture));
			}
			this.mVisibleRanks = 0;
			this.mRankColumn.Clear();
			this.mNameColumn.Clear();
			this.mWavesColumn.Clear();
			this.mScoreColumn.Clear();
			this.UpdateScores();
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x001175FC File Offset: 0x001157FC
		private void OnlineFindResult(LeaderboardFindResult iResult)
		{
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x00117600 File Offset: 0x00115800
		private void OnlineScoresDownloaded(LeaderboardScoresDownloaded iDownloaded)
		{
			bool flag = false;
			if (LevelManager.Instance.Challenges[this.mChallenge].RulesetType == Rulesets.TimedObjective)
			{
				flag = true;
			}
			this.mRankStringBuilder.Remove(0, this.mRankStringBuilder.Length);
			this.mNameStringBuilder.Remove(0, this.mNameStringBuilder.Length);
			this.mScoreStringBuilder.Remove(0, this.mScoreStringBuilder.Length);
			this.mDataStringBuilder.Remove(0, this.mDataStringBuilder.Length);
			this.mVisibleRanks = iDownloaded.mEntryCount;
			for (int i = 0; i < iDownloaded.mEntryCount; i++)
			{
				int[] array = new int[1];
				LeaderboardEntry leaderboardEntry;
				if (SteamUserStats.GetDownloadedLeaderboardEntry(iDownloaded.mSteamLeaderboardEntries, i, out leaderboardEntry, array))
				{
					this.mRankStringBuilder.Append(leaderboardEntry.GlobalRank);
					float num = SubMenuLeaderboards.WAVES_POSITION.X - SubMenuLeaderboards.NAMES_POSITION.X + 12f;
					bool flag2 = false;
					string text = SteamFriends.GetFriendPersonaName(leaderboardEntry.SteamIDUser);
					while (this.mNameColumn.Font.MeasureText(text, true).X > num)
					{
						flag2 = true;
						text = text.Remove(text.Length - 1, 1);
					}
					if (flag2)
					{
						text = text.Remove(text.Length - 1, 1);
						this.mNameStringBuilder.Append(text);
						this.mNameStringBuilder.Append("...");
					}
					else
					{
						this.mNameStringBuilder.Append(text);
					}
					this.mScoreStringBuilder.Append(leaderboardEntry.Score);
					if (flag)
					{
						FloatIntConverter floatIntConverter = new FloatIntConverter(array[leaderboardEntry.Details - 1]);
						TimeSpan timeSpan = TimeSpan.FromSeconds((double)floatIntConverter.Float);
						if (floatIntConverter.Float >= 60f && floatIntConverter.Float < 3600f)
						{
							this.mDataStringBuilder.Append(string.Format("0:{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds));
						}
						else if (floatIntConverter.Float >= 3600f)
						{
							this.mDataStringBuilder.Append(string.Format("{0:0}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
						}
						else
						{
							this.mDataStringBuilder.Append(string.Format("0:00:{0:00}", timeSpan.Seconds));
						}
					}
					else
					{
						this.mDataStringBuilder.Append(array[leaderboardEntry.Details - 1]);
					}
					this.mRankStringBuilder.Append("\n");
					this.mNameStringBuilder.Append("\n");
					this.mScoreStringBuilder.Append("\n");
					this.mDataStringBuilder.Append("\n");
				}
			}
			this.mRankColumn.SetText(this.mRankStringBuilder.ToString());
			this.mNameColumn.SetText(this.mNameStringBuilder.ToString());
			this.mScoreColumn.SetText(this.mScoreStringBuilder.ToString());
			this.mWavesColumn.SetText(this.mDataStringBuilder.ToString());
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x00117930 File Offset: 0x00115B30
		private void UpdateScores()
		{
			if (!this.mShowLocalBoards)
			{
				if (this.mSteamLeaderboards.Count > this.mChallenge)
				{
					this.mRankColumn.SetText("");
					this.mNameColumn.SetText("");
					this.mWavesColumn.SetText("");
					this.mScoreColumn.SetText("");
					SteamUserStats.DownloadLeaderboardEntries(this.mSteamLeaderboards[this.mChallenge], LeaderboardDataRequest.Global, this.mCurrentTopRank + 1, this.mCurrentTopRank + 8, new Action<LeaderboardScoresDownloaded>(this.OnlineScoresDownloaded));
					this.mListBottom.Clear();
					this.mListBottom.Append(this.mCurrentTopRank + 8);
					this.mListTop.Clear();
					this.mListTop.Append(this.mCurrentTopRank + 1);
					return;
				}
			}
			else
			{
				List<LeaderBoardData> list = StatisticsManager.Instance.Leaderboard(this.mChallenge);
				if (this.mCurrentTopRank >= list.Count && list.Count > 0)
				{
					this.mCurrentTopRank -= 8;
					return;
				}
				bool flag = false;
				if (LevelManager.Instance.Challenges[this.mChallenge].RulesetType == Rulesets.TimedObjective)
				{
					flag = true;
				}
				int num = Math.Min(this.mCurrentTopRank, list.Count);
				int num2 = Math.Min(8, list.Count - num);
				this.mRankStringBuilder.Remove(0, this.mRankStringBuilder.Length);
				this.mNameStringBuilder.Remove(0, this.mNameStringBuilder.Length);
				this.mScoreStringBuilder.Remove(0, this.mScoreStringBuilder.Length);
				this.mDataStringBuilder.Remove(0, this.mDataStringBuilder.Length);
				float num3 = SubMenuLeaderboards.WAVES_POSITION.X - SubMenuLeaderboards.NAMES_POSITION.X;
				this.mVisibleRanks = Math.Min(this.mCurrentTopRank + num2, list.Count);
				int num4 = 0;
				int i = num;
				while (i < this.mVisibleRanks)
				{
					this.mRankStringBuilder.Append(num4 + 1);
					bool flag2 = false;
					string text = list[i].Name;
					while (this.mNameColumn.Font.MeasureText(text, true).X > num3)
					{
						flag2 = true;
						text = text.Remove(text.Length - 1, 1);
					}
					if (flag2)
					{
						text = text.Remove(text.Length - 1, 1);
						this.mNameStringBuilder.Append(text);
						this.mNameStringBuilder.Append("...");
					}
					else
					{
						this.mNameStringBuilder.Append(text);
					}
					this.mScoreStringBuilder.Append(list[i].Score);
					if (flag)
					{
						FloatIntConverter floatIntConverter = new FloatIntConverter(list[i].Data1);
						TimeSpan timeSpan = TimeSpan.FromSeconds((double)floatIntConverter.Float);
						if (floatIntConverter.Float >= 60f && floatIntConverter.Float < 3600f)
						{
							this.mDataStringBuilder.Append(string.Format("0:{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds));
						}
						else if (floatIntConverter.Float >= 3600f)
						{
							this.mDataStringBuilder.Append(string.Format("{0:0}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
						}
						else
						{
							this.mDataStringBuilder.Append(string.Format("0:00:{0:00}", timeSpan.Seconds));
						}
					}
					else
					{
						this.mDataStringBuilder.Append(list[i].Data1);
					}
					this.mRankStringBuilder.Append("\n");
					this.mNameStringBuilder.Append("\n");
					this.mScoreStringBuilder.Append("\n");
					this.mDataStringBuilder.Append("\n");
					i++;
					num4++;
				}
				this.mRankColumn.SetText(this.mRankStringBuilder.ToString());
				this.mNameColumn.SetText(this.mNameStringBuilder.ToString());
				this.mWavesColumn.SetText(this.mDataStringBuilder.ToString());
				this.mScoreColumn.SetText(this.mScoreStringBuilder.ToString());
			}
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x00117D88 File Offset: 0x00115F88
		public override void OnEnter()
		{
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, GamePadMenuHelp.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			base.OnEnter();
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].Selected = false;
			}
			this.mSelection = SubMenuLeaderboards.Selections.Level;
			this.mChallenge = 0;
			this.ChangeLeaderboard();
		}

		// Token: 0x04002999 RID: 10649
		private const int VISIBLE_RANKS = 8;

		// Token: 0x0400299A RID: 10650
		private static SubMenuLeaderboards mSingelton;

		// Token: 0x0400299B RID: 10651
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400299C RID: 10652
		private VertexBuffer mVertexBuffer;

		// Token: 0x0400299D RID: 10653
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x0400299E RID: 10654
		private static readonly int LOC_LEADERBOARD = "#menu_lb_01".GetHashCodeCustom();

		// Token: 0x0400299F RID: 10655
		private static readonly int LOC_ONLINE = "#network_02".GetHashCodeCustom();

		// Token: 0x040029A0 RID: 10656
		private static readonly int LOC_LOCAL = "#network_31".GetHashCodeCustom();

		// Token: 0x040029A1 RID: 10657
		public static readonly int LOC_DEBUG = "#location_debug".GetHashCodeCustom();

		// Token: 0x040029A2 RID: 10658
		public static readonly int LOC_SWAMP = "#challenge_swamp".GetHashCodeCustom();

		// Token: 0x040029A3 RID: 10659
		public static readonly int LOC_GLADE = "#challenge_glade".GetHashCodeCustom();

		// Token: 0x040029A4 RID: 10660
		public static readonly int LOC_CAVERN = "#challenge_cavern".GetHashCodeCustom();

		// Token: 0x040029A5 RID: 10661
		public static readonly int LOC_ARENA = "#challenge_arena".GetHashCodeCustom();

		// Token: 0x040029A6 RID: 10662
		public static readonly int LOC_TITLE_TIME = "#lb_time".GetHashCodeCustom();

		// Token: 0x040029A7 RID: 10663
		private static readonly int LOC_RANK = "#lb_rank".GetHashCodeCustom();

		// Token: 0x040029A8 RID: 10664
		private static readonly int LOC_SCORE = "#lb_score".GetHashCodeCustom();

		// Token: 0x040029A9 RID: 10665
		private static readonly int LOC_NAME = "#lb_name".GetHashCodeCustom();

		// Token: 0x040029AA RID: 10666
		private static readonly int LOC_TIME = "#lb_time".GetHashCodeCustom();

		// Token: 0x040029AB RID: 10667
		private static readonly int LOC_WAVES = "#lb_wave".GetHashCodeCustom();

		// Token: 0x040029AC RID: 10668
		private SubMenuLeaderboards.Selections mSelection = SubMenuLeaderboards.Selections.Level;

		// Token: 0x040029AD RID: 10669
		private int mChallenge;

		// Token: 0x040029AE RID: 10670
		private bool mShowLocalBoards;

		// Token: 0x040029AF RID: 10671
		private List<ulong> mSteamLeaderboards;

		// Token: 0x040029B0 RID: 10672
		private static readonly Vector2 TITLE_POSITION = new Vector2(512f, 192f);

		// Token: 0x040029B1 RID: 10673
		private static readonly Vector2 CHALLENGE_POSITION = new Vector2(144f, SubMenuLeaderboards.TITLE_POSITION.Y + 48f);

		// Token: 0x040029B2 RID: 10674
		private static readonly Vector2 CHALLENGE_SIZE = new Vector2(768f, 256f);

		// Token: 0x040029B3 RID: 10675
		private static readonly Vector2 RANKS_POSITION = new Vector2(SubMenuLeaderboards.CHALLENGE_POSITION.X, SubMenuLeaderboards.CHALLENGE_POSITION.Y + SubMenuLeaderboards.CHALLENGE_SIZE.Y);

		// Token: 0x040029B4 RID: 10676
		private static readonly Vector2 NAMES_POSITION = new Vector2(SubMenuLeaderboards.RANKS_POSITION.X + 128f, SubMenuLeaderboards.RANKS_POSITION.Y);

		// Token: 0x040029B5 RID: 10677
		private static readonly Vector2 WAVES_POSITION = new Vector2(SubMenuLeaderboards.RANKS_POSITION.X + 128f + 256f + 32f + 32f, SubMenuLeaderboards.RANKS_POSITION.Y);

		// Token: 0x040029B6 RID: 10678
		private static readonly Vector2 SCORES_POSITION = new Vector2(896f, SubMenuLeaderboards.RANKS_POSITION.Y);

		// Token: 0x040029B7 RID: 10679
		private static readonly Vector2 ONLINE_POSITION = new Vector2(848f, 960f);

		// Token: 0x040029B8 RID: 10680
		private StringBuilder mRankStringBuilder;

		// Token: 0x040029B9 RID: 10681
		private StringBuilder mNameStringBuilder;

		// Token: 0x040029BA RID: 10682
		private StringBuilder mScoreStringBuilder;

		// Token: 0x040029BB RID: 10683
		private StringBuilder mDataStringBuilder;

		// Token: 0x040029BC RID: 10684
		private Text mRanksHeader;

		// Token: 0x040029BD RID: 10685
		private Text mNamesHeader;

		// Token: 0x040029BE RID: 10686
		private Text mScoresHeader;

		// Token: 0x040029BF RID: 10687
		private Text mWavesHeader;

		// Token: 0x040029C0 RID: 10688
		private Text mRankColumn;

		// Token: 0x040029C1 RID: 10689
		private Text mNameColumn;

		// Token: 0x040029C2 RID: 10690
		private Text mScoreColumn;

		// Token: 0x040029C3 RID: 10691
		private Text mWavesColumn;

		// Token: 0x040029C4 RID: 10692
		private float mHeadLineHeight;

		// Token: 0x040029C5 RID: 10693
		private float mLineHeight;

		// Token: 0x040029C6 RID: 10694
		private int mCurrentTopRank;

		// Token: 0x040029C7 RID: 10695
		private int mVisibleRanks;

		// Token: 0x040029C8 RID: 10696
		private MenuImageItem mListRightArrow;

		// Token: 0x040029C9 RID: 10697
		private MenuImageItem mListLeftArrow;

		// Token: 0x040029CA RID: 10698
		private Text mListTop;

		// Token: 0x040029CB RID: 10699
		private Text mListBottom;

		// Token: 0x040029CC RID: 10700
		private MenuImageItem mLevelRightArrow;

		// Token: 0x040029CD RID: 10701
		private MenuImageItem mLevelLeftArrow;

		// Token: 0x040029CE RID: 10702
		private ContentManager mCurrentContent;

		// Token: 0x040029CF RID: 10703
		private string mCurrentTexturePath;

		// Token: 0x040029D0 RID: 10704
		private string mTexturePath = "Levels/Challenges/#1;";

		// Token: 0x040029D1 RID: 10705
		private Texture2D mCurrentTexture;

		// Token: 0x040029D2 RID: 10706
		private float mTextureAlpha;

		// Token: 0x02000508 RID: 1288
		private enum Selections
		{
			// Token: 0x040029D4 RID: 10708
			None,
			// Token: 0x040029D5 RID: 10709
			Back,
			// Token: 0x040029D6 RID: 10710
			LocalOnline,
			// Token: 0x040029D7 RID: 10711
			List,
			// Token: 0x040029D8 RID: 10712
			Level
		}
	}
}
