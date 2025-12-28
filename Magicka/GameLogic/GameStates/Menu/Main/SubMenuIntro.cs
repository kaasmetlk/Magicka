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
	// Token: 0x020001B8 RID: 440
	internal class SubMenuIntro : SubMenu
	{
		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06000D77 RID: 3447 RVA: 0x0004F3B4 File Offset: 0x0004D5B4
		public static SubMenuIntro Instance
		{
			get
			{
				if (SubMenuIntro.sSingelton == null)
				{
					lock (SubMenuIntro.sSingeltonLock)
					{
						if (SubMenuIntro.sSingelton == null)
						{
							SubMenuIntro.sSingelton = new SubMenuIntro();
						}
					}
				}
				return SubMenuIntro.sSingelton;
			}
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x0004F428 File Offset: 0x0004D628
		private SubMenuIntro()
		{
			lock (Game.Instance.GraphicsDevice)
			{
				this.mTextures = new Texture2D[]
				{
					null,
					Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s1"),
					Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s2"),
					Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s3"),
					Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s4"),
					Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s5"),
					Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s6")
				};
				this.mMaskTexture = Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_mask");
				this.mPageTexture = Game.Instance.Content.Load<Texture2D>("UI/ToM/tome_pages_0");
			}
			Vector4[] array = new Vector4[4 + 4 * this.mTextures.Length];
			int i = 0;
			array[i * 4].X = 0f;
			array[i * 4].Y = 0f;
			array[i * 4].Z = 0.5f;
			array[i * 4].W = 0f;
			array[i * 4 + 1].X = 1024f;
			array[i * 4 + 1].Y = 0f;
			array[i * 4 + 1].Z = 1f;
			array[i * 4 + 1].W = 0f;
			array[i * 4 + 2].X = 1024f;
			array[i * 4 + 2].Y = 1024f;
			array[i * 4 + 2].Z = 1f;
			array[i * 4 + 2].W = 0.5f;
			array[i * 4 + 3].X = 0f;
			array[i * 4 + 3].Y = 1024f;
			array[i * 4 + 3].Z = 0.5f;
			array[i * 4 + 3].W = 0.5f;
			for (i++; i <= this.mTextures.Length; i++)
			{
				if (this.mTextures[i - 1] != null)
				{
					Vector2 vector = default(Vector2);
					vector.X = 1f / (float)this.mTextures[i - 1].Width;
					vector.Y = 1f / (float)this.mTextures[i - 1].Height;
					array[i * 4].X = -426f;
					array[i * 4].Y = -326f;
					array[i * 4].Z = 0f;
					array[i * 4].W = 0f;
					array[i * 4 + 1].X = 426f;
					array[i * 4 + 1].Y = -326f;
					array[i * 4 + 1].Z = 852f * vector.X;
					array[i * 4 + 1].W = 0f;
					array[i * 4 + 2].X = 426f;
					array[i * 4 + 2].Y = 326f;
					array[i * 4 + 2].Z = 852f * vector.X;
					array[i * 4 + 2].W = 652f * vector.Y;
					array[i * 4 + 3].X = -426f;
					array[i * 4 + 3].Y = 326f;
					array[i * 4 + 3].Z = 0f;
					array[i * 4 + 3].W = 652f * vector.Y;
				}
			}
			GraphicsDevice graphicsDevice2 = Game.Instance.GraphicsDevice;
			lock (graphicsDevice2)
			{
				this.mVertices = new VertexBuffer(graphicsDevice2, array.Length * 4 * 4, BufferUsage.WriteOnly);
				this.mVertices.SetData<Vector4>(array);
				this.mVertexDeclaration = new VertexDeclaration(graphicsDevice2, new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
			}
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuTitle);
			this.mTitle = new Text(32, font, TextAlign.Center, false);
			this.mDescription = new Text(128, font, TextAlign.Center, false);
			BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
			this.mText = new Text(512, font2, TextAlign.Center, false);
			this.mOldText = new Text(512, font2, TextAlign.Center, false);
			this.mFinish = false;
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x0004FA8C File Offset: 0x0004DC8C
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (iState.LeftButton == ButtonState.Pressed || !(Tome.Instance.CurrentState is Tome.OpenState))
			{
				return;
			}
			if (iOldState.LeftButton == ButtonState.Pressed)
			{
				this.mFinish = true;
				if (this.mCurrentCue != null)
				{
					this.mCurrentCue.Stop(AudioStopOptions.AsAuthored);
				}
			}
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x0004FADA File Offset: 0x0004DCDA
		public void DrawOld(Viewport iLeftSide, Viewport iRightSide)
		{
			this.Draw(ref iRightSide, 1f, this.mOldText, this.mTextureIndex - 1);
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x0004FAF8 File Offset: 0x0004DCF8
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			if (this.mSwap)
			{
				Helper.Swap<Text>(ref this.mText, ref this.mOldText);
				this.mTextureIndex = this.mIndex;
				this.mSwap = false;
			}
			float num = 1f - this.mHackTimer / this.mTimes[this.mTextureIndex];
			num = num / 0.8f - 0.125f;
			MathHelper.Clamp(num, 0f, 1f);
			num = MathHelper.SmoothStep(0f, 1f, num);
			num = MathHelper.SmoothStep(0f, 1f, num);
			this.Draw(ref iRightSide, num, this.mText, this.mTextureIndex);
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x0004FBA4 File Offset: 0x0004DDA4
		private void Draw(ref Viewport iViewport, float iTime, Text iText, int iTextureIndex)
		{
			this.mEffect.GraphicsDevice.Viewport = iViewport;
			this.mEffect.Color = new Vector4(1f);
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			if ((iTextureIndex >= 0 & iTextureIndex < this.mTextures.Length) && this.mTextures[iTextureIndex] != null)
			{
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				Matrix transform = default(Matrix);
				transform = default(Matrix);
				transform.M11 = (transform.M22 = (transform.M44 = 1f));
				transform.M41 = 512f;
				transform.M42 = 416f;
				this.mEffect.Transform = transform;
				this.mEffect.TextureOffset = default(Vector2);
				this.mEffect.TextureScale = Vector2.One;
				this.mEffect.TextureEnabled = true;
				this.mEffect.Color = Vector4.One;
				this.mEffect.Texture = this.mMaskTexture;
				this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
				this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
				this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
				this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
				this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.Zero;
				this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.SourceAlpha;
				switch (iTextureIndex)
				{
				case 3:
					SubMenuIntro.DrawImagePan1(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
					break;
				case 4:
					SubMenuIntro.DrawImageZoom1(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
					break;
				case 5:
					SubMenuIntro.DrawImageZoom2(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
					break;
				default:
					SubMenuIntro.DrawImageNoAnimation(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
					break;
				}
				this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
				this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
				this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationAlpha;
				this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseDestinationAlpha;
				switch (iTextureIndex)
				{
				case 3:
					SubMenuIntro.DrawImagePan1(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
					break;
				case 4:
					SubMenuIntro.DrawImageZoom1(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
					break;
				case 5:
					SubMenuIntro.DrawImageZoom2(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
					break;
				default:
					SubMenuIntro.DrawImageNoAnimation(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
					break;
				}
				transform.M11 = 1f;
				transform.M22 = 1f;
				transform.M41 = 0f;
				transform.M42 = 0f;
				this.mEffect.Transform = transform;
				this.mEffect.Texture = this.mPageTexture;
				this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
				this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
				this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
				this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
				this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
			}
			else
			{
				this.mEffect.Color = MenuItem.COLOR;
				this.mTitle.Draw(this.mEffect, 512f, 360f);
				this.mDescription.Draw(this.mEffect, 512f, 480f);
			}
			this.mEffect.Color = new Vector4(0.1f, 0.1f, 0.1f, 0.9f);
			this.mEffect.CommitChanges();
			iText.Draw(this.mEffect, 512f, 768f);
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x000500D3 File Offset: 0x0004E2D3
		private static void DrawImageNoAnimation(float iTime, GUIBasicEffect iEffect, Texture2D iTexture, int iStartVertex)
		{
			iEffect.Texture = iTexture;
			iEffect.TextureEnabled = true;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iStartVertex, 2);
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x000500F8 File Offset: 0x0004E2F8
		private static void DrawImagePan1(float iTime, GUIBasicEffect iEffect, Texture2D iTexture, int iStartVertex)
		{
			iEffect.Texture = iTexture;
			iEffect.TextureEnabled = true;
			iEffect.TextureOffset = new Vector2
			{
				X = (100f + iTime * 600f) / (float)iTexture.Width
			};
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iStartVertex, 2);
			iEffect.TextureOffset = default(Vector2);
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x00050164 File Offset: 0x0004E364
		private static void DrawImageZoom1(float iTime, GUIBasicEffect iEffect, Texture2D iTexture, int iStartVertex)
		{
			iEffect.Texture = iTexture;
			iEffect.TextureEnabled = true;
			iTime = 1f - iTime;
			iEffect.TextureOffset = new Vector2
			{
				X = iTime * (float)iTexture.Width * 0.25f / (float)iTexture.Width,
				Y = iTime * 64f / (float)iTexture.Height
			};
			iEffect.TextureScale = new Vector2
			{
				X = 2f - iTime,
				Y = 2f - iTime
			};
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iStartVertex, 2);
			iEffect.TextureOffset = default(Vector2);
			iEffect.TextureScale = new Vector2(1f);
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x0005022C File Offset: 0x0004E42C
		private static void DrawImageZoom2(float iTime, GUIBasicEffect iEffect, Texture2D iTexture, int iStartVertex)
		{
			iEffect.Texture = iTexture;
			iEffect.TextureEnabled = true;
			iEffect.TextureOffset = new Vector2
			{
				X = iTime * 280f / (float)iTexture.Width,
				Y = iTime * 220f / (float)iTexture.Height
			};
			iEffect.TextureScale = new Vector2
			{
				X = 1f + iTime * 0.1f,
				Y = 1f + iTime * 0.1f
			};
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iStartVertex, 2);
			iEffect.TextureOffset = default(Vector2);
			iEffect.TextureScale = new Vector2(1f);
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x000502EC File Offset: 0x0004E4EC
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mHackTimer = Math.Max(this.mHackTimer - iDeltaTime, 0f);
			if (Tome.Instance.CurrentState is Tome.OpenState && (this.mIndex < 0 || this.mCurrentCue == null))
			{
				if (this.mIndex < 0)
				{
					if (this.mHackTimer <= 0f)
					{
						this.mIndex = 0;
						Tome.Instance.TargetLightIntensity = 1f;
						Tome.Instance.TargetLightVariationAmount = 0.2f;
						Tome.Instance.TargetLightVariationSpeed = 0.5f;
						this.mHackTimer = this.mTimes[this.mIndex];
					}
				}
				else
				{
					this.mCurrentCue = AudioManager.Instance.PlayCue(Banks.WaveBank, this.mSounds[this.mIndex]);
				}
			}
			if (this.mFinish)
			{
				if (!this.mFinishing)
				{
					this.mFinishing = true;
					RenderManager.Instance.TransitionEnd += this.OnTransitionEnd;
					RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 2f);
					return;
				}
			}
			else if (this.mIndex < 0)
			{
				if (this.mHackTimer <= 5f && this.mCurrentCue == null)
				{
					this.mCurrentCue = AudioManager.Instance.PlayCue(Banks.WaveBank, this.mSounds[0]);
					string text = LanguageManager.Instance.GetString(this.mTexts[0]);
					text = DialogManager.Instance.SubtitleFont.Wrap(text, RenderManager.Instance.ScreenSize.X * 8 / 10, true);
					DialogManager.Instance.ShowSubtitles(text);
					return;
				}
			}
			else if (this.mHackTimer <= 0f)
			{
				DialogManager.Instance.HideSubtitles();
				lock (this.mTextures)
				{
					if (this.mIndex < this.mTexts.Length - 1)
					{
						int num = this.mIndex + 1;
						if (num == this.mTexts.Length - 1)
						{
							RenderManager.Instance.TransitionEnd += this.OnTransitionEnd;
							RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 2f);
						}
						else
						{
							string text2 = LanguageManager.Instance.GetString(this.mTexts[num]);
							text2 = this.mOldText.Font.Wrap(text2, this.mTextWidth, true);
							this.mOldText.SetText(text2);
							this.mCurrentCue = null;
							this.mHackTimer = this.mTimes[num];
							this.mFinish = false;
							this.mSwap = true;
							Tome.Instance.Riffle(PlaybackMode.Forward, 1);
						}
						this.mIndex = num;
					}
				}
			}
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x00050584 File Offset: 0x0004E784
		private void OnTransitionEnd(TransitionEffect iDeadTransition)
		{
			Tome.Instance.PopMenuInstant();
			if (Tome.Instance.CurrentMenu is SubMenuEndGame)
			{
				Tome.Instance.PopMenuInstant();
			}
			RenderManager.Instance.TransitionEnd -= this.OnTransitionEnd;
			CampaignNode campaignNode = (SubMenuCharacterSelect.Instance.GameType == GameType.Mythos) ? LevelManager.Instance.MythosCampaign[0] : LevelManager.Instance.VanillaCampaign[0];
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
			PlayState playState = new PlayState(iCustom, campaignNode.FullFileName, SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType, campaignNode.SpawnPoint, iSaveSlot, null);
			if (!this.mFinish && this.mIndex != -1)
			{
				this.mCurrentCue = AudioManager.Instance.PlayCue(Banks.WaveBank, this.mSounds[this.mIndex]);
				string @string = LanguageManager.Instance.GetString(this.mTexts[this.mIndex]);
				playState.SetTip(@string, false, this.mCurrentCue);
			}
			GameStateManager.Instance.PushState(playState);
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x000506F6 File Offset: 0x0004E8F6
		public override void DrawNewAndOld(SubMenu iPreviousMenu, Viewport iCurrentLeftSide, Viewport iCurrentRightSide, Viewport iPreviousLeftSide, Viewport iPreviousRightSide)
		{
			if (iPreviousMenu != null & this.mIndex == 0 & !this.mSwap)
			{
				base.DrawNewAndOld(iPreviousMenu, iCurrentLeftSide, iCurrentRightSide, iPreviousLeftSide, iPreviousRightSide);
				return;
			}
			this.Draw(iCurrentLeftSide, iCurrentRightSide);
			this.DrawOld(iPreviousLeftSide, iPreviousRightSide);
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06000D84 RID: 3460 RVA: 0x00050735 File Offset: 0x0004E935
		// (set) Token: 0x06000D85 RID: 3461 RVA: 0x0005073D File Offset: 0x0004E93D
		public bool Play
		{
			get
			{
				return this.mPlay;
			}
			set
			{
				this.mPlay = value;
			}
		}

		// Token: 0x06000D86 RID: 3462 RVA: 0x00050746 File Offset: 0x0004E946
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

		// Token: 0x06000D87 RID: 3463 RVA: 0x00050775 File Offset: 0x0004E975
		public override void ControllerB(Controller iSender)
		{
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x00050778 File Offset: 0x0004E978
		public override void OnEnter()
		{
			base.OnEnter();
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			this.mFinish = false;
			this.mFinishing = false;
			if (this.mPlay)
			{
				CampaignNode campaignNode = (SubMenuCharacterSelect.Instance.GameType == GameType.Mythos) ? LevelManager.Instance.MythosCampaign[0] : LevelManager.Instance.VanillaCampaign[0];
				this.mTitle.SetText(LanguageManager.Instance.GetString(campaignNode.Name.GetHashCodeCustom()));
				this.mDescription.SetText(LanguageManager.Instance.GetString(campaignNode.Description));
				AudioManager.Instance.StopMusic();
				AudioManager.Instance.PlayMusic(Banks.Music, SubMenuIntro.MUSIC, null);
				Tome.Instance.TargetLightIntensity = 0f;
				Tome.Instance.TargetLightVariationAmount = 0f;
				Tome.Instance.TargetLightVariationSpeed = 1.5f;
				this.mIndex = -1;
				this.mTextureIndex = 0;
				this.mText.Clear();
				this.mCurrentCue = null;
				this.mPlay = false;
				this.mHackTimer = 8f;
			}
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x000508B5 File Offset: 0x0004EAB5
		public override void OnExit()
		{
			base.OnExit();
			Tome.Instance.TargetLightIntensity = 1f;
			Tome.Instance.TargetLightVariationAmount = 0.2f;
			Tome.Instance.TargetLightVariationSpeed = 6f;
			DialogManager.Instance.HideSubtitles();
		}

		// Token: 0x04000C37 RID: 3127
		private const float VLAD_DARK_TIME = 5f;

		// Token: 0x04000C38 RID: 3128
		private static SubMenuIntro sSingelton;

		// Token: 0x04000C39 RID: 3129
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04000C3A RID: 3130
		private int mTextWidth = 896;

		// Token: 0x04000C3B RID: 3131
		public static readonly int MUSIC = "music_intro".GetHashCodeCustom();

		// Token: 0x04000C3C RID: 3132
		private int[] mSounds = new int[]
		{
			"cuts_int0".GetHashCodeCustom(),
			"cuts_int1".GetHashCodeCustom(),
			"cuts_int2".GetHashCodeCustom(),
			"cuts_int3".GetHashCodeCustom(),
			"cuts_int4".GetHashCodeCustom(),
			"cuts_int5".GetHashCodeCustom(),
			"cuts_int6".GetHashCodeCustom(),
			"cuts_int7".GetHashCodeCustom()
		};

		// Token: 0x04000C3D RID: 3133
		private int[] mTexts = new int[]
		{
			"#cutscene_intro0".GetHashCodeCustom(),
			"#cutscene_intro1".GetHashCodeCustom(),
			"#cutscene_intro2".GetHashCodeCustom(),
			"#cutscene_intro3".GetHashCodeCustom(),
			"#cutscene_intro4".GetHashCodeCustom(),
			"#cutscene_intro5".GetHashCodeCustom(),
			"#cutscene_intro6".GetHashCodeCustom(),
			"#cutscene_intro7".GetHashCodeCustom()
		};

		// Token: 0x04000C3E RID: 3134
		private float[] mTimes = new float[]
		{
			17f,
			6f,
			20f,
			11f,
			17f,
			13f,
			9f,
			18f
		};

		// Token: 0x04000C3F RID: 3135
		private Text mTitle;

		// Token: 0x04000C40 RID: 3136
		private Text mDescription;

		// Token: 0x04000C41 RID: 3137
		private Texture2D mMaskTexture;

		// Token: 0x04000C42 RID: 3138
		private Texture2D mPageTexture;

		// Token: 0x04000C43 RID: 3139
		private Texture2D[] mTextures;

		// Token: 0x04000C44 RID: 3140
		private VertexBuffer mVertices;

		// Token: 0x04000C45 RID: 3141
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04000C46 RID: 3142
		private int mTextureIndex;

		// Token: 0x04000C47 RID: 3143
		private Text mText;

		// Token: 0x04000C48 RID: 3144
		private Text mOldText;

		// Token: 0x04000C49 RID: 3145
		private bool mFinish;

		// Token: 0x04000C4A RID: 3146
		private bool mFinishing;

		// Token: 0x04000C4B RID: 3147
		private float mHackTimer;

		// Token: 0x04000C4C RID: 3148
		private bool mSwap;

		// Token: 0x04000C4D RID: 3149
		private Cue mCurrentCue;

		// Token: 0x04000C4E RID: 3150
		private int mIndex;

		// Token: 0x04000C4F RID: 3151
		private bool mPlay;
	}
}
