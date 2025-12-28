using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Achievements
{
	// Token: 0x020002BA RID: 698
	internal class PdxWidget : PdxWidgetWindow
	{
		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06001505 RID: 5381 RVA: 0x00083754 File Offset: 0x00081954
		public static PdxWidget Instance
		{
			get
			{
				if (PdxWidget.sSingelton == null)
				{
					lock (PdxWidget.sSingeltonLock)
					{
						if (PdxWidget.sSingelton == null)
						{
							PdxWidget.sSingelton = new PdxWidget();
						}
					}
				}
				return PdxWidget.sSingelton;
			}
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x000837A8 File Offset: 0x000819A8
		private PdxWidget()
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mEffect = new GUIBasicEffect(graphicsDevice, null);
			}
			Vector4[] array = new Vector4[84];
			int i = 0;
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_0);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_1);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_2);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_3);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_4);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_5);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_6);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_7);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_8);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.points_number_9);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.frame_profilebox_middle);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.frame_profilebox_top);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.frame_profilebox_bottom);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.frame_points);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.menu_highlight_left);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.menu_highlight_right);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.badge_achievement_off);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.badge_achievement_on);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.badge_game);
			PdxWidgetWindow.CreateVertices(array, ref i, PdxWidgetWindow.Textures.popup_info);
			array[i].X = 0f;
			array[i].Y = 0f;
			array[i].Z = 0f;
			array[i].W = 0f;
			i++;
			array[i].X = 1f;
			array[i].Y = 0f;
			array[i].Z = 1f;
			array[i].W = 0f;
			i++;
			array[i].X = 1f;
			array[i].Y = 1f;
			array[i].Z = 1f;
			array[i].W = 1f;
			i++;
			array[i].X = 0f;
			array[i].Y = 1f;
			array[i].Z = 0f;
			array[i].W = 1f;
			i++;
			lock (graphicsDevice)
			{
				this.mVertices = new VertexBuffer(graphicsDevice, array.Length * 4 * 4, BufferUsage.WriteOnly);
				this.mVertices.SetData<Vector4>(array);
				this.mDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
			}
			this.mAchievementsRect = PdxWidgetWindow.sRectangles[16];
			this.mGamesRect = PdxWidgetWindow.sRectangles[14];
			this.mGamePopupRect = PdxWidgetWindow.sRectangles[8];
			this.mAchievementPopupRect.Width = 75;
			this.mAchievementPopupRect.Height = 75;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Small);
			BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Small_Bold);
			BitmapFont font3 = FontManager.Instance.GetFont(MagickaFont.PDX_Points);
			this.mLogoutButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[40], PdxWidgetWindow.sRectangles[41], font, AchievementsManager.BTN_LOGOUT);
			this.mCloseButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[40], PdxWidgetWindow.sRectangles[41], font, AchievementsManager.BTN_CLOSE);
			this.mLeftButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[22], PdxWidgetWindow.sRectangles[23], null, 0);
			this.mLeftButton.State |= PdxButton.ButtonState.Over;
			this.mRightButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[24], PdxWidgetWindow.sRectangles[25], null, 0);
			this.mRightButton.State |= PdxButton.ButtonState.Over;
			this.mRankText = new Text(32, font, TextAlign.Left, false);
			this.mRankNumText = new Text(32, font, TextAlign.Left, false);
			this.mAchievementsText = new Text(32, font, TextAlign.Center, false);
			this.mGamesText = new Text(32, font, TextAlign.Center, false);
			this.mPopupHeaderText = new Text(64, font2, TextAlign.Left, false);
			this.mPopupBodyText = new Text(256, font, TextAlign.Left, false);
			this.mPopupDateText = new Text(64, font, TextAlign.Right, false);
			this.mPopupPointsText = new Text(32, font3, TextAlign.Left, false);
			this.mRenderData = new PdxWidget.RenderData[3];
			for (i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new PdxWidget.RenderData();
			}
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x00083CA8 File Offset: 0x00081EA8
		public override void Show()
		{
			if (!AchievementsManager.Instance.LoggedIn)
			{
				return;
			}
			this.mAchievements = true;
			this.mScroll = 0;
			this.mDoScroll = (AchievementsManager.Instance.Achievements.Count > 10);
			base.Show();
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x00083CE4 File Offset: 0x00081EE4
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			this.mProgress = (this.mProgress + iDeltaTime) % 2f;
			Point screenSize = RenderManager.Instance.ScreenSize;
			this.mAchievementsRect.X = (screenSize.X - 800) / 2 + 180;
			this.mAchievementsRect.Y = (screenSize.Y - 600) / 2 + 119;
			this.mGamesRect.X = (screenSize.X - 800) / 2 + 397;
			this.mGamesRect.Y = (screenSize.Y - 600) / 2 + 119;
			PdxWidget.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.Alpha = this.mAlpha;
			renderData.Progress = this.mProgress;
			renderData.PopupAchievementEarned = this.mPopupAchievementEarned;
			renderData.DoProgress = AchievementsManager.Instance.Busy;
			renderData.Points = AchievementsManager.Instance.TotalPoints;
			renderData.DoScroll = this.mDoScroll;
			renderData.Scroll = this.mScroll;
			renderData.DoPopup = (this.mPopup >= 0);
			renderData.MousePos = this.mPopupPos;
			renderData.Achievements = this.mAchievements;
			GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x06001509 RID: 5385 RVA: 0x00083E37 File Offset: 0x00082037
		public override void OnLanguageChanged()
		{
			base.OnLanguageChanged();
			this.mLogoutButton.OnLanguageChanged();
			this.mCloseButton.OnLanguageChanged();
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x00083E58 File Offset: 0x00082058
		public override void OnMouseDown(ref Vector2 iMousePos)
		{
			if (this.mCloseButton.InsideBounds(ref iMousePos))
			{
				this.mCloseButton.State |= PdxButton.ButtonState.Down;
			}
			if (this.mLogoutButton.InsideBounds(ref iMousePos))
			{
				this.mLogoutButton.State |= PdxButton.ButtonState.Down;
			}
			if (this.mDoScroll)
			{
				if (this.mLeftButton.InsideBounds(ref iMousePos))
				{
					this.mLeftButton.State |= PdxButton.ButtonState.Down;
				}
				if (this.mRightButton.InsideBounds(ref iMousePos))
				{
					this.mRightButton.State |= PdxButton.ButtonState.Down;
				}
			}
			if (this.mAchievementsRect.Contains((int)iMousePos.X, (int)iMousePos.Y))
			{
				this.mAchievements = true;
				this.mScroll = 0;
				this.mDoScroll = (AchievementsManager.Instance.Achievements.Count > 10);
				return;
			}
			if (this.mGamesRect.Contains((int)iMousePos.X, (int)iMousePos.Y))
			{
				this.mAchievements = false;
				this.mScroll = 0;
				this.mDoScroll = (AchievementsManager.Instance.Games.Count > 10);
			}
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x00083F78 File Offset: 0x00082178
		public override void OnMouseUp(ref Vector2 iMousePos)
		{
			if (this.mCloseButton.InsideBounds(ref iMousePos) & (this.mCloseButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
			{
				this.Hide();
			}
			this.mCloseButton.State &= (PdxButton.ButtonState)(-2);
			if (this.mLogoutButton.InsideBounds(ref iMousePos) & (this.mLogoutButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
			{
				AchievementsManager.Instance.LogOut();
				this.Hide();
			}
			this.mLogoutButton.State &= (PdxButton.ButtonState)(-2);
			if (this.mDoScroll)
			{
				if (this.mLeftButton.InsideBounds(ref iMousePos) & (this.mLeftButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
				{
					int num = this.mScroll - 10;
					if (num >= 0)
					{
						this.mScroll = num;
					}
					else
					{
						this.mScroll = 0;
					}
				}
				if (this.mRightButton.InsideBounds(ref iMousePos) & (this.mRightButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
				{
					int num2 = this.mScroll + 10;
					int num3 = this.mAchievements ? AchievementsManager.Instance.Achievements.Count : AchievementsManager.Instance.Games.Count;
					if (num2 < num3)
					{
						this.mScroll = num2;
					}
				}
			}
			this.mLeftButton.State &= (PdxButton.ButtonState)(-2);
			this.mRightButton.State &= (PdxButton.ButtonState)(-2);
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x000840DC File Offset: 0x000822DC
		public override void OnMouseMove(ref Vector2 iMousePos)
		{
			if (this.mCloseButton.InsideBounds(ref iMousePos))
			{
				this.mCloseButton.State |= PdxButton.ButtonState.Over;
			}
			else
			{
				this.mCloseButton.State &= (PdxButton.ButtonState)(-3);
			}
			if (this.mLogoutButton.InsideBounds(ref iMousePos))
			{
				this.mLogoutButton.State |= PdxButton.ButtonState.Over;
			}
			else
			{
				this.mLogoutButton.State &= (PdxButton.ButtonState)(-3);
			}
			AchievementsManager instance = AchievementsManager.Instance;
			Point screenSize = RenderManager.Instance.ScreenSize;
			screenSize.X = (screenSize.X - 800) / 2;
			screenSize.Y = (screenSize.Y - 600) / 2;
			int num = -1;
			Rectangle rectangle = this.mAchievements ? this.mAchievementPopupRect : this.mGamePopupRect;
			int num2 = this.mAchievements ? instance.Achievements.Count : instance.Games.Count;
			rectangle.Y = screenSize.Y + (this.mAchievements ? 204 : 170);
			for (int i = 0; i < 2; i++)
			{
				rectangle.X = screenSize.X + (this.mAchievements ? 78 : 55);
				for (int j = 0; j < 5; j++)
				{
					int num3 = i * 5 + j + this.mScroll;
					if ((num3 >= 0 & num3 < num2) && rectangle.Contains((int)iMousePos.X, (int)iMousePos.Y))
					{
						num = num3;
						break;
					}
					rectangle.X += 137;
				}
				rectangle.Y += 137;
			}
			if (this.mPopup != num)
			{
				this.mPopup = num;
				if (num >= 0)
				{
					if (this.mAchievements)
					{
						AchievementData achievementData = instance.Achievements[this.mPopup];
						this.mPopupHeaderText.SetText(achievementData.Name);
						string text = this.mPopupBodyText.Font.Wrap(achievementData.Desc, 238, true);
						this.mPopupBodyText.SetText(text);
						if (achievementData.DateAchieved.Year == 1970)
						{
							text = AchievementsManager.Instance.GetTranslation(AchievementsManager.EARNED_THIS_SESSION);
						}
						else
						{
							text = AchievementsManager.Instance.GetTranslation(AchievementsManager.ACHIEVEMENT_EARNED);
							text = text.Replace("%s", achievementData.DateAchieved.ToString());
						}
						this.mPopupDateText.SetText(text);
						text = AchievementsManager.Instance.GetTranslation(AchievementsManager.NUM_PP);
						text = text.Replace("%d", achievementData.Points.ToString());
						this.mPopupPointsText.SetText(text);
						this.mPopupAchievementEarned = achievementData.Achieved;
					}
					else
					{
						GameData gameData = instance.Games[this.mPopup];
						this.mPopupHeaderText.SetText(gameData.Name);
						string text2 = this.mPopupBodyText.Font.Wrap(gameData.Desc, 238, true);
						this.mPopupBodyText.SetText(text2);
					}
				}
			}
			this.mPopupPos = iMousePos;
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x00084416 File Offset: 0x00082616
		public override void OnKeyDown(KeyData iData)
		{
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00084418 File Offset: 0x00082618
		public override void OnKeyPress(char iChar)
		{
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x0008441C File Offset: 0x0008261C
		internal void OnProfileUpdate()
		{
			string text = AchievementsManager.Instance.GetTranslation(AchievementsManager.YOUR_RANK);
			this.mRankText.SetText(text);
			this.mRankNumText.SetText(" " + AchievementsManager.Instance.Rank.ToString());
			this.mRankWidth = this.mRankText.Font.MeasureText(this.mRankText.Characters, true).X;
			this.mRankNumWidth = this.mRankNumText.Font.MeasureText(this.mRankNumText.Characters, true).X;
			text = AchievementsManager.Instance.GetTranslation(AchievementsManager.MENU_ACHIEVEMENTS);
			int num = text.IndexOf("%d");
			text = text.Substring(0, num) + AchievementsManager.Instance.AwardedAchievements.ToString() + text.Substring(num + 2);
			num = text.IndexOf("%d");
			text = text.Substring(0, num) + AchievementsManager.Instance.Achievements.Count.ToString() + text.Substring(num + 2);
			this.mAchievementsText.SetText(text);
			text = AchievementsManager.Instance.GetTranslation(AchievementsManager.MENU_GAMES);
			text = text.Replace("%d", AchievementsManager.Instance.Games.Count.ToString());
			this.mGamesText.SetText(text);
		}

		// Token: 0x0400168B RID: 5771
		private static PdxWidget sSingelton;

		// Token: 0x0400168C RID: 5772
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400168D RID: 5773
		private GUIBasicEffect mEffect;

		// Token: 0x0400168E RID: 5774
		private PdxWidget.RenderData[] mRenderData;

		// Token: 0x0400168F RID: 5775
		private float mProgress;

		// Token: 0x04001690 RID: 5776
		private VertexBuffer mVertices;

		// Token: 0x04001691 RID: 5777
		private VertexDeclaration mDeclaration;

		// Token: 0x04001692 RID: 5778
		private PdxButton mLogoutButton;

		// Token: 0x04001693 RID: 5779
		private PdxButton mCloseButton;

		// Token: 0x04001694 RID: 5780
		private PdxButton mLeftButton;

		// Token: 0x04001695 RID: 5781
		private PdxButton mRightButton;

		// Token: 0x04001696 RID: 5782
		private bool mDoScroll;

		// Token: 0x04001697 RID: 5783
		private int mScroll;

		// Token: 0x04001698 RID: 5784
		private bool mAchievements = true;

		// Token: 0x04001699 RID: 5785
		private Rectangle mAchievementsRect;

		// Token: 0x0400169A RID: 5786
		private Rectangle mGamesRect;

		// Token: 0x0400169B RID: 5787
		private Text mRankText;

		// Token: 0x0400169C RID: 5788
		private float mRankWidth;

		// Token: 0x0400169D RID: 5789
		private Text mRankNumText;

		// Token: 0x0400169E RID: 5790
		private float mRankNumWidth;

		// Token: 0x0400169F RID: 5791
		private Text mAchievementsText;

		// Token: 0x040016A0 RID: 5792
		private Text mGamesText;

		// Token: 0x040016A1 RID: 5793
		private int mPopup = -1;

		// Token: 0x040016A2 RID: 5794
		private Vector2 mPopupPos;

		// Token: 0x040016A3 RID: 5795
		private Rectangle mAchievementPopupRect;

		// Token: 0x040016A4 RID: 5796
		private Rectangle mGamePopupRect;

		// Token: 0x040016A5 RID: 5797
		private bool mPopupAchievementEarned;

		// Token: 0x040016A6 RID: 5798
		private Text mPopupHeaderText;

		// Token: 0x040016A7 RID: 5799
		private Text mPopupBodyText;

		// Token: 0x040016A8 RID: 5800
		private Text mPopupDateText;

		// Token: 0x040016A9 RID: 5801
		private Text mPopupPointsText;

		// Token: 0x020002BB RID: 699
		private class RenderData : IRenderableGUIObject
		{
			// Token: 0x06001511 RID: 5393 RVA: 0x00084594 File Offset: 0x00082794
			public void Draw(float iDeltaTime)
			{
				PdxWidget instance = PdxWidget.Instance;
				GUIBasicEffect mEffect = instance.mEffect;
				Point screenSize = RenderManager.Instance.ScreenSize;
				mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				Vector4 color = default(Vector4);
				color.X = (color.Y = (color.Z = 1f));
				color.W = this.Alpha;
				mEffect.Color = color;
				mEffect.Texture = PdxWidgetWindow.sTexture;
				mEffect.TextureEnabled = true;
				mEffect.GraphicsDevice.Vertices[0].SetSource(instance.mVertices, 0, 16);
				mEffect.GraphicsDevice.VertexDeclaration = instance.mDeclaration;
				Vector2 vector = default(Vector2);
				vector.X = (float)((screenSize.X - 800) / 2);
				vector.Y = (float)((screenSize.Y - 600) / 2);
				mEffect.Begin();
				mEffect.CurrentTechnique.Passes[0].Begin();
				this.DrawQuad(mEffect, vector.X + 13f, vector.Y + 118f, 10);
				this.DrawQuad(mEffect, vector.X + 0f, vector.Y + 0f, 11);
				this.DrawQuad(mEffect, vector.X + 0f, vector.Y + 446f, 12);
				Vector2 position = default(Vector2);
				if (this.DoProgress)
				{
					Rectangle rectangle = PdxWidgetWindow.sRectangles[5];
					Rectangle rectangle2 = PdxWidgetWindow.sRectangles[4];
					int num = (int)(this.Progress * (float)rectangle.Width);
					if (num <= rectangle.Width)
					{
						rectangle.Width = num;
						rectangle2.X += num;
						rectangle2.Width -= num;
						this.DrawQuad(mEffect, vector.X + 330f, vector.Y + 210f, ref rectangle);
						this.DrawQuad(mEffect, vector.X + 330f + (float)num, vector.Y + 210f, ref rectangle2);
					}
					else
					{
						num -= rectangle.Width;
						rectangle2.Width = num;
						rectangle.X += num;
						rectangle.Width -= num;
						this.DrawQuad(mEffect, vector.X + 330f, vector.Y + 210f, ref rectangle2);
						this.DrawQuad(mEffect, vector.X + 330f + (float)num, vector.Y + 210f, ref rectangle);
					}
				}
				else
				{
					color.W = this.Alpha * this.Alpha;
					mEffect.Color = color;
					this.DrawQuad(mEffect, vector.X + 278f, vector.Y + 42f, 13);
					if (this.Achievements)
					{
						this.DrawQuad(mEffect, vector.X + 180f, vector.Y + 119f, 14);
					}
					else
					{
						this.DrawQuad(mEffect, vector.X + 397f, vector.Y + 119f, 15);
					}
					color.W = this.Alpha * this.Alpha * this.Alpha;
					mEffect.Color = color;
					position.X = vector.X + 467f;
					position.Y = vector.Y + 45f;
					int num2 = this.Points;
					for (int i = 0; i < 6; i++)
					{
						this.DrawQuad(mEffect, position.X, position.Y, num2 % 10);
						num2 /= 10;
						position.X -= 37f;
					}
					color.W = 0.627451f;
					mEffect.Color = color;
					position.X = vector.X + 391f - (float)Math.Floor((double)((instance.mRankWidth + instance.mRankNumWidth) * 0.5f));
					position.Y = vector.Y + 16f;
					instance.mRankText.Draw(mEffect, position.X, position.Y);
					color.W = 0.8784314f;
					mEffect.Color = color;
					position.X += instance.mRankWidth;
					instance.mRankNumText.Draw(mEffect, position.X, position.Y);
					color.W = (this.Achievements ? 1f : 0.5f);
					mEffect.Color = color;
					instance.mAchievementsText.Draw(mEffect, vector.X + 295f, vector.Y + 128f);
					color.W = (this.Achievements ? 0.5f : 1f);
					mEffect.Color = color;
					instance.mGamesText.Draw(mEffect, vector.X + 484f, vector.Y + 128f);
					if (this.DoScroll)
					{
						position.X = vector.X + 10f;
						position.Y = vector.Y + 240f;
						instance.mLeftButton.Position = position;
						instance.mLeftButton.Draw(mEffect, this.Alpha * this.Alpha);
						position.X = vector.X + 751f;
						position.Y = vector.Y + 240f;
						instance.mRightButton.Position = position;
						instance.mRightButton.Draw(mEffect, this.Alpha * this.Alpha);
					}
					mEffect.GraphicsDevice.Vertices[0].SetSource(instance.mVertices, 0, 16);
					mEffect.GraphicsDevice.VertexDeclaration = instance.mDeclaration;
					mEffect.Texture = PdxWidgetWindow.sTexture;
					color.W = this.Alpha * this.Alpha;
					mEffect.Color = color;
					if (this.Achievements)
					{
						this.DrawAchievements(mEffect, ref vector, instance);
					}
					else
					{
						this.DrawGames(mEffect, ref vector, instance);
					}
				}
				position.X = vector.X + 140f;
				position.Y = vector.Y + 85f;
				instance.mLogoutButton.Position = position;
				instance.mLogoutButton.Draw(mEffect, this.Alpha * this.Alpha);
				position.X = vector.X + 580f;
				position.Y = vector.Y + 85f;
				instance.mCloseButton.Position = position;
				instance.mCloseButton.Draw(mEffect, this.Alpha * this.Alpha);
				mEffect.CurrentTechnique.Passes[0].End();
				mEffect.End();
			}

			// Token: 0x17000560 RID: 1376
			// (get) Token: 0x06001512 RID: 5394 RVA: 0x00084C92 File Offset: 0x00082E92
			public int ZIndex
			{
				get
				{
					return 2147483644;
				}
			}

			// Token: 0x06001513 RID: 5395 RVA: 0x00084C9C File Offset: 0x00082E9C
			private void DrawQuad(GUIBasicEffect iEffect, float iX, float iY, int iId)
			{
				this.mTransform.M11 = 1f;
				this.mTransform.M22 = 1f;
				this.mTransform.M41 = iX;
				this.mTransform.M42 = iY;
				iEffect.Transform = this.mTransform;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iId * 4, 2);
			}

			// Token: 0x06001514 RID: 5396 RVA: 0x00084D04 File Offset: 0x00082F04
			private void DrawQuad(GUIBasicEffect iEffect, float iX, float iY, ref Rectangle iSourceRect)
			{
				this.mTransform.M11 = 1f;
				this.mTransform.M22 = 1f;
				this.mTransform.M41 = iX;
				this.mTransform.M42 = iY;
				iEffect.Transform = this.mTransform;
				iEffect.CommitChanges();
				this.mTmpVertices[0].X = 0f;
				this.mTmpVertices[0].Y = 0f;
				this.mTmpVertices[0].Z = (float)iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
				this.mTmpVertices[0].W = (float)iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
				this.mTmpVertices[1].X = (float)iSourceRect.Width;
				this.mTmpVertices[1].Y = 0f;
				this.mTmpVertices[1].Z = (float)(iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
				this.mTmpVertices[1].W = (float)iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
				this.mTmpVertices[2].X = (float)iSourceRect.Width;
				this.mTmpVertices[2].Y = (float)iSourceRect.Height;
				this.mTmpVertices[2].Z = (float)(iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
				this.mTmpVertices[2].W = (float)(iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
				this.mTmpVertices[3].X = 0f;
				this.mTmpVertices[3].Y = (float)iSourceRect.Height;
				this.mTmpVertices[3].Z = (float)iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
				this.mTmpVertices[3].W = (float)(iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
				iEffect.GraphicsDevice.DrawUserPrimitives<Vector4>(PrimitiveType.TriangleFan, this.mTmpVertices, 0, 2);
			}

			// Token: 0x06001515 RID: 5397 RVA: 0x00084F6C File Offset: 0x0008316C
			private void DrawAchievements(GUIBasicEffect iEffect, ref Vector2 iTopLeft, PdxWidget iOwner)
			{
				AchievementsManager instance = AchievementsManager.Instance;
				List<AchievementData> achievements = instance.Achievements;
				Vector2 vector = default(Vector2);
				vector.Y = 180f + iTopLeft.Y;
				for (int i = 0; i < 2; i++)
				{
					vector.X = 55f + iTopLeft.X;
					for (int j = 0; j < 5; j++)
					{
						int num = i * 5 + j + this.Scroll;
						if (num >= 0 & num < achievements.Count)
						{
							AchievementData achievementData = achievements[num];
							if (achievementData.Achieved)
							{
								this.DrawQuad(iEffect, vector.X, vector.Y, 17);
							}
							else
							{
								this.DrawQuad(iEffect, vector.X, vector.Y, 16);
							}
							Texture2D achievementImage = instance.GetAchievementImage(achievementData.Code);
							if (achievementImage != null)
							{
								iEffect.Texture = achievementImage;
								this.mTransform.M11 = (float)achievementImage.Width;
								this.mTransform.M22 = (float)achievementImage.Height;
								this.mTransform.M41 = vector.X + (float)Math.Floor((double)((124f - this.mTransform.M11) * 0.5f));
								this.mTransform.M42 = vector.Y + (float)Math.Floor((double)((124f - this.mTransform.M22) * 0.5f));
								iEffect.Transform = this.mTransform;
								iEffect.CommitChanges();
								iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 80, 2);
								iEffect.Texture = PdxWidgetWindow.sTexture;
							}
						}
						vector.X += 137f;
					}
					vector.Y += 137f;
				}
				if (this.DoPopup)
				{
					this.DrawQuad(iEffect, this.MousePos.X, this.MousePos.Y - 25f, 19);
					float num2 = 0f;
					if (this.PopupAchievementEarned)
					{
						iEffect.Color = new Vector4(0.45882353f, 0.8980392f, 0.23137255f, this.Alpha * this.Alpha);
						iOwner.mPopupDateText.Draw(iEffect, this.MousePos.X + 251f, this.MousePos.Y - 15f);
						num2 += 25f;
						iEffect.Color = new Vector4(this.Alpha * this.Alpha);
					}
					iOwner.mPopupHeaderText.Draw(iEffect, this.MousePos.X + 29f, num2 + this.MousePos.Y - 16f);
					iOwner.mPopupBodyText.Draw(iEffect, this.MousePos.X + 29f, num2 + this.MousePos.Y - 16f + (float)iOwner.mPopupHeaderText.Font.LineHeight + 2f);
					iOwner.mPopupPointsText.Draw(iEffect, this.MousePos.X + 29f, this.MousePos.Y + 85f);
				}
			}

			// Token: 0x06001516 RID: 5398 RVA: 0x0008529C File Offset: 0x0008349C
			private void DrawGames(GUIBasicEffect iEffect, ref Vector2 iTopLeft, PdxWidget iOwner)
			{
				AchievementsManager instance = AchievementsManager.Instance;
				List<GameData> games = instance.Games;
				Vector2 vector = default(Vector2);
				vector.Y = 170f + iTopLeft.Y;
				for (int i = 0; i < 2; i++)
				{
					vector.X = 55f + iTopLeft.X;
					for (int j = 0; j < 5; j++)
					{
						int num = i * 5 + j + this.Scroll;
						if (num >= 0 & num < games.Count)
						{
							GameData gameData = games[num];
							this.DrawQuad(iEffect, vector.X, vector.Y, 18);
							Texture2D gameImage = instance.GetGameImage(gameData.Code);
							if (gameImage != null)
							{
								iEffect.Texture = gameImage;
								this.mTransform.M11 = (float)gameImage.Width;
								this.mTransform.M22 = (float)gameImage.Height;
								this.mTransform.M41 = vector.X + 17f;
								this.mTransform.M42 = vector.Y + 15f;
								iEffect.Transform = this.mTransform;
								iEffect.CommitChanges();
								iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 80, 2);
								iEffect.Texture = PdxWidgetWindow.sTexture;
							}
						}
						vector.X += 137f;
					}
					vector.Y += 137f;
				}
				if (this.DoPopup)
				{
					this.DrawQuad(iEffect, this.MousePos.X, this.MousePos.Y - 25f, 19);
					iOwner.mPopupHeaderText.Draw(iEffect, this.MousePos.X + 29f, this.MousePos.Y - 16f);
					iOwner.mPopupBodyText.Draw(iEffect, this.MousePos.X + 29f, this.MousePos.Y - 16f + (float)iOwner.mPopupHeaderText.Font.LineHeight + 2f);
				}
			}

			// Token: 0x040016AA RID: 5802
			public float Alpha;

			// Token: 0x040016AB RID: 5803
			public bool DoProgress;

			// Token: 0x040016AC RID: 5804
			public float Progress;

			// Token: 0x040016AD RID: 5805
			public Vector2 MousePos;

			// Token: 0x040016AE RID: 5806
			public bool PopupAchievementEarned;

			// Token: 0x040016AF RID: 5807
			public bool DoPopup;

			// Token: 0x040016B0 RID: 5808
			public bool Achievements;

			// Token: 0x040016B1 RID: 5809
			public bool DoScroll;

			// Token: 0x040016B2 RID: 5810
			public int Scroll;

			// Token: 0x040016B3 RID: 5811
			public int Points;

			// Token: 0x040016B4 RID: 5812
			private Matrix mTransform = Matrix.Identity;

			// Token: 0x040016B5 RID: 5813
			private Vector4[] mTmpVertices = new Vector4[4];
		}
	}
}
