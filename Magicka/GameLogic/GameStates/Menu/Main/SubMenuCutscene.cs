using System;
using Magicka.Audio;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Magicka.Network;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x02000154 RID: 340
	internal class SubMenuCutscene : SubMenu
	{
		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06000A1B RID: 2587 RVA: 0x0003CF2C File Offset: 0x0003B12C
		public static SubMenuCutscene Instance
		{
			get
			{
				if (SubMenuCutscene.sSingelton == null)
				{
					lock (SubMenuCutscene.sSingeltonLock)
					{
						if (SubMenuCutscene.sSingelton == null)
						{
							SubMenuCutscene.sSingelton = new SubMenuCutscene();
						}
					}
				}
				return SubMenuCutscene.sSingelton;
			}
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x0003CF80 File Offset: 0x0003B180
		private SubMenuCutscene()
		{
			this.mTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/CampaignMap");
			this.mMaskTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/MapMask");
			this.mPageTexture = Game.Instance.Content.Load<Texture2D>("UI/ToM/tome_pages_0");
			this.mTexture_Dungeons = Game.Instance.Content.Load<Texture2D>("UI/Menu/CampaignMap_Dungeons");
			this.mMaskTexture_Dungeons = Game.Instance.Content.Load<Texture2D>("UI/Menu/MapMask_Dungeons");
			if (this.mIsDungeons)
			{
				this.mapImagePolygonWidth = (float)this.mTexture_Dungeons.Width;
				this.mapImagePolygonHeight = (float)this.mTexture_Dungeons.Height;
			}
			this.mTexture_Dungeons2 = Game.Instance.Content.Load<Texture2D>("UI/Menu/CampaignMap_Dungeons2");
			this.mMaskTexture_Dungeons2 = Game.Instance.Content.Load<Texture2D>("UI/Menu/MapMask_Dungeons2");
			if (this.mIsDungeons2)
			{
				this.mapImagePolygonWidth = (float)this.mTexture_Dungeons2.Width;
				this.mapImagePolygonHeight = (float)this.mTexture_Dungeons2.Height;
			}
			this.mVertices_Pages = new VertexBuffer(Game.Instance.GraphicsDevice, 64, BufferUsage.WriteOnly);
			Vector4[] array = new Vector4[4];
			array[0].X = 0f;
			array[0].Y = 0f;
			array[0].Z = 0f;
			array[0].W = 0f;
			array[1].X = 2048f;
			array[1].Y = 0f;
			array[1].Z = 1f;
			array[1].W = 0f;
			array[2].X = 2048f;
			array[2].Y = 2048f;
			array[2].Z = 1f;
			array[2].W = 1f;
			array[3].X = 0f;
			array[3].Y = 2048f;
			array[3].Z = 0f;
			array[3].W = 1f;
			this.mVertices_Pages.SetData<Vector4>(array);
			if (this.mapImagePolygonWidth != 2048f || this.mapImagePolygonHeight != 2048f)
			{
				this.RebuildVertexes();
			}
			this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
			});
			this.mChapterNumber = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mChapterTitle = new Text(64, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mFinish = false;
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x0003D298 File Offset: 0x0003B498
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (iState.LeftButton == ButtonState.Pressed || !(Tome.Instance.CurrentState is Tome.OpenState))
			{
				return;
			}
			this.mFinish = true;
			if (this.mCurrentCue != null && this.mCurrentCue.IsPlaying)
			{
				this.mCurrentCue.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x0003D2E9 File Offset: 0x0003B4E9
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x0003D2EB File Offset: 0x0003B4EB
		public override void DrawNewAndOld(SubMenu iPreviousMenu, Viewport iCurrentLeftSide, Viewport iCurrentRightSide, Viewport iPreviousLeftSide, Viewport iPreviousRightSide)
		{
			if (iPreviousMenu != null & !this.mMap)
			{
				base.DrawNewAndOld(iPreviousMenu, iCurrentLeftSide, iCurrentRightSide, iPreviousLeftSide, iPreviousRightSide);
				return;
			}
			this.Draw(iCurrentLeftSide, iCurrentRightSide);
			this.DrawOld(iPreviousLeftSide, iPreviousRightSide);
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x0003D320 File Offset: 0x0003B520
		public void DrawOld(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.DrawChapter();
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x0003D390 File Offset: 0x0003B590
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			if (this.mCutscene == null)
			{
				return;
			}
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			if (this.mMap)
			{
				this.DrawMap();
			}
			else
			{
				this.DrawChapter();
			}
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x0003D41C File Offset: 0x0003B61C
		private void DrawChapter()
		{
			this.mEffect.Color = MenuItem.COLOR;
			this.mChapterNumber.Draw(this.mEffect, 512f, 360f);
			this.mChapterTitle.Draw(this.mEffect, 512f, 480f);
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x0003D470 File Offset: 0x0003B670
		public void SetImageDimensions(float width, float height)
		{
			float num = this.mapImagePolygonWidth;
			float num2 = this.mapImagePolygonHeight;
			this.mapImagePolygonWidth = width;
			this.mapImagePolygonHeight = height;
			if (this.mVertices_Image == null || (this.mVertices_Image != null && (width != num || height != num2)))
			{
				this.RebuildVertexes();
			}
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x0003D4B8 File Offset: 0x0003B6B8
		private void DrawMap()
		{
			bool flag = this.mapImagePolygonWidth != 2048f || this.mapImagePolygonHeight != 2048f;
			this.mEffect.Color = Vector4.One;
			Matrix transform = default(Matrix);
			transform.M11 = 0.5f;
			transform.M22 = 0.5f;
			transform.M44 = 1f;
			this.mEffect.Transform = transform;
			this.mEffect.Texture = this.mMaskTexture;
			this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
			this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices_Pages, 0, 16);
			this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			Matrix transform2 = default(Matrix);
			Vector2 vector;
			float num;
			this.mCutscene.GetCamera(this.mTime, out vector, out num);
			transform2.M11 = (transform2.M22 = num);
			transform2.M44 = 1f;
			transform2.M41 = this.mapImagePolygonWidth / 4f - vector.X * num;
			transform2.M42 = this.mapImagePolygonHeight / 4f - vector.Y * num;
			if (flag)
			{
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices_Image, 0, 16);
			}
			if (this.mIsDungeons || this.mIsDungeons2)
			{
				this.mEffect.Color = new Vector4(1f);
				this.mEffect.TextureScale = Vector2.One;
				this.mEffect.TextureEnabled = true;
			}
			this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
			this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
			this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationAlpha;
			this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseDestinationAlpha;
			if (this.mIsDungeons)
			{
				this.mEffect.Texture = this.mTexture_Dungeons;
			}
			else if (this.mIsDungeons2)
			{
				this.mEffect.Texture = this.mTexture_Dungeons2;
			}
			else
			{
				this.mEffect.Texture = this.mTexture;
			}
			this.mEffect.Transform = transform2;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			transform.M11 = 1f;
			transform.M22 = 1f;
			transform.M41 = -1024f;
			this.mEffect.Transform = transform;
			this.mEffect.Texture = this.mPageTexture;
			if (flag)
			{
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices_Pages, 0, 16);
			}
			this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
			this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
			this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
			this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
			this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x0003D890 File Offset: 0x0003BA90
		public override void ControllerUp(Controller iSender)
		{
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x0003D892 File Offset: 0x0003BA92
		public override void ControllerDown(Controller iSender)
		{
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x0003D894 File Offset: 0x0003BA94
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mFinish)
			{
				if (!this.mFinishing)
				{
					this.mFinishing = true;
					if (this.mCurrentCue != null && this.mCurrentCue.IsPlaying)
					{
						this.mCurrentCue.Stop(AudioStopOptions.AsAuthored);
					}
					DialogManager.Instance.HideSubtitles();
					RenderManager.Instance.TransitionEnd += this.OnTransitionEnd;
					RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 0.5f);
					return;
				}
			}
			else if (!this.mMap)
			{
				this.mMapTimer -= iDeltaTime;
				if (this.mMapTimer <= 0f)
				{
					this.mMap = true;
					Tome.Instance.Riffle(PlaybackMode.Forward, 1);
					if (this.mCurrentCue != null && !this.mCurrentCue.IsPlaying)
					{
						this.mCurrentCue.Play();
					}
					if (!string.IsNullOrEmpty(this.mSubtitle))
					{
						DialogManager.Instance.ShowSubtitles(this.mSubtitle);
						return;
					}
				}
			}
			else
			{
				this.mTime += iDeltaTime;
				if (this.mTime > this.mCutscene.Duration & (this.mCurrentCue == null || (this.mCurrentCue.IsStopping | this.mCurrentCue.IsStopped)))
				{
					this.mFinish = true;
				}
			}
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x0003D9D8 File Offset: 0x0003BBD8
		private void OnTransitionEnd(TransitionEffect iDeadTransition)
		{
			Tome.Instance.PopMenuInstant();
			if (Tome.Instance.CurrentMenu is SubMenuEndGame)
			{
				Tome.Instance.PopMenuInstant();
			}
			RenderManager.Instance.TransitionEnd -= this.OnTransitionEnd;
			CampaignNode campaignNode;
			if (this.mLevel < 0)
			{
				campaignNode = LevelManager.Instance.DungeonsCampaign[this.mLevel * -1 - 1];
				this.mIsDungeons = false;
				this.mIsDungeons2 = false;
			}
			else
			{
				campaignNode = ((SubMenuCharacterSelect.Instance.GameType == GameType.Mythos) ? LevelManager.Instance.MythosCampaign[this.mLevel] : LevelManager.Instance.VanillaCampaign[this.mLevel]);
			}
			SaveData iSaveSlot = null;
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				iSaveSlot = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
			}
			bool iCustom = HackHelper.LicenseStatus == HackHelper.Status.Hacked || HackHelper.CheckLicense(campaignNode) != HackHelper.License.Yes || LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Hacked;
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing && HackHelper.CheckLicense(players[i].Gamer.Avatar) != HackHelper.License.Yes)
				{
					iCustom = true;
					break;
				}
			}
			bool flag = true;
			if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).HasEntered)
			{
				flag = false;
			}
			if (flag)
			{
				PlayState iGameState = new PlayState(iCustom, campaignNode.FullFileName, SubMenuCharacterSelect.Instance.GameType, campaignNode.SpawnPoint, iSaveSlot, null);
				GameStateManager.Instance.PushState(iGameState);
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000A29 RID: 2601 RVA: 0x0003DB5C File Offset: 0x0003BD5C
		// (set) Token: 0x06000A2A RID: 2602 RVA: 0x0003DB64 File Offset: 0x0003BD64
		public bool Play
		{
			get
			{
				return this.mPlay;
			}
			set
			{
				this.mPlay = value;
				if (this.mPlay)
				{
					this.mMap = false;
				}
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000A2B RID: 2603 RVA: 0x0003DB7C File Offset: 0x0003BD7C
		// (set) Token: 0x06000A2C RID: 2604 RVA: 0x0003DB84 File Offset: 0x0003BD84
		public int Level
		{
			get
			{
				return this.mLevel;
			}
			set
			{
				this.mLevel = value;
				CampaignNode campaignNode;
				if (value < 0 && value >= -2)
				{
					this.mIsDungeons = (value == -1);
					this.mIsDungeons2 = (value == -2);
					int num = value * -1 - 1;
					campaignNode = LevelManager.Instance.DungeonsCampaign[num];
				}
				else
				{
					campaignNode = ((SubMenuCharacterSelect.Instance.GameType == GameType.Mythos) ? LevelManager.Instance.MythosCampaign[value] : LevelManager.Instance.VanillaCampaign[value]);
				}
				if (this.mIsDungeons)
				{
					if (this.mTexture_Dungeons == null)
					{
						this.SetImageDimensions(1704f, 652f);
					}
					else
					{
						this.SetImageDimensions((float)this.mTexture_Dungeons.Width, (float)this.mTexture_Dungeons.Height);
					}
				}
				else if (this.mIsDungeons2)
				{
					if (this.mTexture_Dungeons2 == null)
					{
						this.SetImageDimensions(1704f, 652f);
					}
					else
					{
						this.SetImageDimensions((float)this.mTexture_Dungeons2.Width, (float)this.mTexture_Dungeons2.Height);
					}
				}
				else
				{
					this.SetImageDimensions(2048f, 2048f);
				}
				this.mCutscene = campaignNode.Cutscene;
				this.mChapterNumber.SetText(LanguageManager.Instance.GetString(campaignNode.Name.GetHashCodeCustom()));
				string text = LanguageManager.Instance.GetString(campaignNode.ShortDescription);
				if (string.IsNullOrEmpty(text) || text.Length > 64)
				{
					text = "";
				}
				text = this.mChapterTitle.Font.Wrap(text, 800, true);
				this.mChapterTitle.SetText(text);
			}
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x0003DD00 File Offset: 0x0003BF00
		private void RebuildVertexes()
		{
			if (this.mapImagePolygonWidth == 2048f && this.mapImagePolygonHeight == 2048f)
			{
				this.mVertices_Image = null;
				return;
			}
			this.mVertices_Image = new VertexBuffer(Game.Instance.GraphicsDevice, 64, BufferUsage.WriteOnly);
			Vector4[] array = new Vector4[4];
			array[0].X = 0f;
			array[0].Y = 0f;
			array[0].Z = 0f;
			array[0].W = 0f;
			array[1].X = this.mapImagePolygonWidth;
			array[1].Y = 0f;
			array[1].Z = 1f;
			array[1].W = 0f;
			array[2].X = this.mapImagePolygonWidth;
			array[2].Y = this.mapImagePolygonHeight;
			array[2].Z = 1f;
			array[2].W = 1f;
			array[3].X = 0f;
			array[3].Y = this.mapImagePolygonHeight;
			array[3].Z = 0f;
			array[3].W = 1f;
			this.mVertices_Image.SetData<Vector4>(array);
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x0003DE6E File Offset: 0x0003C06E
		public override void ControllerA(Controller iSender)
		{
			if (!(Tome.Instance.CurrentState is Tome.OpenState))
			{
				return;
			}
			this.mFinish = true;
			if (this.mCurrentCue != null)
			{
				this.mCurrentCue.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x0003DE9D File Offset: 0x0003C09D
		public override void ControllerB(Controller iSender)
		{
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x0003DEA0 File Offset: 0x0003C0A0
		public override void OnEnter()
		{
			base.OnEnter();
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			this.mFinishing = false;
			if (this.mPlay)
			{
				this.mMapTimer = 3f;
				if (this.mCutscene != null)
				{
					Point screenSize = RenderManager.Instance.ScreenSize;
					string iText;
					if (LanguageManager.Instance.TryGetString(this.mCutscene.SubTitles, out iText))
					{
						this.mSubtitle = DialogManager.Instance.SubtitleFont.Wrap(iText, screenSize.X - 128, true);
					}
					this.mCurrentCue = AudioManager.Instance.GetCue(this.mCutscene.DialogBank, this.mCutscene.Dialog);
					this.mFinish = false;
				}
				else
				{
					this.mFinish = true;
				}
				this.mPlay = false;
				this.mTime = 0f;
				return;
			}
			this.mFinish = true;
		}

		// Token: 0x04000926 RID: 2342
		private const float pagesAndMaskPolygonWidth = 2048f;

		// Token: 0x04000927 RID: 2343
		private const float pagesAndMaskPolygonHeight = 2048f;

		// Token: 0x04000928 RID: 2344
		private static SubMenuCutscene sSingelton;

		// Token: 0x04000929 RID: 2345
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400092A RID: 2346
		private float mapImagePolygonWidth = 2048f;

		// Token: 0x0400092B RID: 2347
		private float mapImagePolygonHeight = 2048f;

		// Token: 0x0400092C RID: 2348
		protected int mLevel;

		// Token: 0x0400092D RID: 2349
		private VertexBuffer mVertices_Image;

		// Token: 0x0400092E RID: 2350
		private VertexBuffer mVertices_Pages;

		// Token: 0x0400092F RID: 2351
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04000930 RID: 2352
		private Texture2D mPageTexture;

		// Token: 0x04000931 RID: 2353
		private Texture2D mTexture;

		// Token: 0x04000932 RID: 2354
		private Texture2D mMaskTexture;

		// Token: 0x04000933 RID: 2355
		private Texture2D mTexture_Dungeons;

		// Token: 0x04000934 RID: 2356
		private Texture2D mMaskTexture_Dungeons;

		// Token: 0x04000935 RID: 2357
		private Texture2D mPageTexture_Dungeons;

		// Token: 0x04000936 RID: 2358
		private bool mIsDungeons;

		// Token: 0x04000937 RID: 2359
		private Texture2D mTexture_Dungeons2;

		// Token: 0x04000938 RID: 2360
		private Texture2D mMaskTexture_Dungeons2;

		// Token: 0x04000939 RID: 2361
		private Texture2D mPageTexture_Dungeons2;

		// Token: 0x0400093A RID: 2362
		private bool mIsDungeons2;

		// Token: 0x0400093B RID: 2363
		private bool mFinish;

		// Token: 0x0400093C RID: 2364
		private bool mFinishing;

		// Token: 0x0400093D RID: 2365
		private float mTime;

		// Token: 0x0400093E RID: 2366
		private string mSubtitle;

		// Token: 0x0400093F RID: 2367
		private Cue mCurrentCue;

		// Token: 0x04000940 RID: 2368
		private bool mPlay;

		// Token: 0x04000941 RID: 2369
		private bool mMap;

		// Token: 0x04000942 RID: 2370
		private float mMapTimer;

		// Token: 0x04000943 RID: 2371
		private Cutscene mCutscene;

		// Token: 0x04000944 RID: 2372
		private Text mChapterNumber;

		// Token: 0x04000945 RID: 2373
		private Text mChapterTitle;
	}
}
