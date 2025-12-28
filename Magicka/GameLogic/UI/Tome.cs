using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.GameStates.Menu.Main.Options;
using Magicka.GameLogic.UI.Popup;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.Network;
using Magicka.WebTools;
using Magicka.WebTools.GameSparks;
using Magicka.WebTools.Paradox;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using SteamWrapper;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x020002D5 RID: 725
	internal sealed class Tome
	{
		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06001648 RID: 5704 RVA: 0x0008D760 File Offset: 0x0008B960
		public static Tome Instance
		{
			get
			{
				if (Tome.mSingelton == null)
				{
					lock (Tome.mSingeltonLock)
					{
						if (Tome.mSingelton == null)
						{
							Tome.mSingelton = new Tome();
						}
					}
				}
				return Tome.mSingelton;
			}
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x0008D7B4 File Offset: 0x0008B9B4
		public static bool MousePickTome(Point iScreenSize, int iX, int iY, out Vector2 oHitPosition, out bool oRightPageHit)
		{
			Matrix matrix = Tome.mViewProjection;
			Matrix matrix2;
			Matrix.Invert(ref matrix, out matrix2);
			Vector4 vector = new Vector4(-1f + 2f * (float)iX / (float)iScreenSize.X, 1f - 2f * (float)iY / (float)iScreenSize.Y, 1f, 1f);
			Vector4.Transform(ref vector, ref matrix2, out vector);
			Vector3 value = default(Vector3);
			value.X = vector.X;
			value.Y = vector.Y;
			value.Z = vector.Z;
			Vector3.Divide(ref value, vector.W, out value);
			Vector3 vector2 = Tome.mCameraPosition;
			Vector3 delta = value - vector2;
			Segment segment = new Segment(vector2, delta);
			Triangle[] left_TRIS = Tome.LEFT_TRIS;
			Triangle[] right_TRIS = Tome.RIGHT_TRIS;
			float num;
			float num2;
			float num3;
			if (Intersection.SegmentTriangleIntersection(out num, out num2, out num3, ref segment, ref right_TRIS[0], false))
			{
				oHitPosition = new Vector2((1f - num2 + 0.03f) * 0.97f * (float)Tome.PAGERIGHTSHEET.Width, num3 * (float)Tome.PAGERIGHTSHEET.Height);
				oRightPageHit = true;
				return true;
			}
			if (Intersection.SegmentTriangleIntersection(out num, out num2, out num3, ref segment, ref right_TRIS[1], false))
			{
				oHitPosition = new Vector2((num2 + 0.03f) * 0.97f * (float)Tome.PAGERIGHTSHEET.Width, (1f - num3) * (float)Tome.PAGERIGHTSHEET.Height);
				oRightPageHit = true;
				return true;
			}
			if (Intersection.SegmentTriangleIntersection(out num, out num2, out num3, ref segment, ref left_TRIS[0], false))
			{
				oHitPosition = new Vector2((1f - num2) * 0.97f * (float)Tome.PAGERIGHTSHEET.Width, num3 * (float)Tome.PAGERIGHTSHEET.Height);
				oRightPageHit = false;
				return true;
			}
			if (Intersection.SegmentTriangleIntersection(out num, out num2, out num3, ref segment, ref left_TRIS[1], false))
			{
				oHitPosition = new Vector2(num2 * 0.97f * (float)Tome.PAGERIGHTSHEET.Width, (1f - num3) * (float)Tome.PAGERIGHTSHEET.Height);
				oRightPageHit = false;
				return true;
			}
			oHitPosition = Vector2.Zero;
			oRightPageHit = false;
			return false;
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x0600164A RID: 5706 RVA: 0x0008DA10 File Offset: 0x0008BC10
		// (remove) Token: 0x0600164B RID: 5707 RVA: 0x0008DA29 File Offset: 0x0008BC29
		public event Action OnOpen;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x0600164C RID: 5708 RVA: 0x0008DA42 File Offset: 0x0008BC42
		// (remove) Token: 0x0600164D RID: 5709 RVA: 0x0008DA5B File Offset: 0x0008BC5B
		public event Action OnClose;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x0600164E RID: 5710 RVA: 0x0008DA74 File Offset: 0x0008BC74
		// (remove) Token: 0x0600164F RID: 5711 RVA: 0x0008DA8D File Offset: 0x0008BC8D
		public event Action OnBackClose;

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06001650 RID: 5712 RVA: 0x0008DAA6 File Offset: 0x0008BCA6
		// (set) Token: 0x06001651 RID: 5713 RVA: 0x0008DAAD File Offset: 0x0008BCAD
		public static bool PromotionActive
		{
			get
			{
				return Tome.sPromotionActive;
			}
			set
			{
				Tome.sPromotionActive = value;
			}
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x0008DAB8 File Offset: 0x0008BCB8
		static Tome()
		{
			Tome.TOMELEFTSHEET.X = 0;
			Tome.TOMELEFTSHEET.Width = 1024;
			Tome.TOMELEFTSHEET.Y = 0;
			Tome.TOMELEFTSHEET.Height = 1024;
			Tome.TOMERIGHTSHEET.X = 1024;
			Tome.TOMERIGHTSHEET.Width = 1024;
			Tome.TOMERIGHTSHEET.Y = 0;
			Tome.TOMERIGHTSHEET.Height = 1024;
			Tome.PAGELEFTSHEET.X = 0;
			Tome.PAGELEFTSHEET.Width = 1024;
			Tome.PAGELEFTSHEET.Y = 1024;
			Tome.PAGELEFTSHEET.Height = 1024;
			Tome.PAGERIGHTSHEET.X = 1024;
			Tome.PAGERIGHTSHEET.Width = 1024;
			Tome.PAGERIGHTSHEET.Y = 1024;
			Tome.PAGERIGHTSHEET.Height = 1024;
			Tome.LEFT_TRIS = new Triangle[2];
			Tome.LEFT_TRIS[0].Origin = Tome.PICK_LEFT_AREA_TOPRIGHT - new Vector3(0f, 0.1f, 0f);
			Tome.LEFT_TRIS[0].Edge0 = new Vector3(5.3f, 0.1f, 0f);
			Tome.LEFT_TRIS[0].Edge1 = new Vector3(0f, 0f, -5.5f);
			Tome.LEFT_TRIS[1].Origin = Tome.PICK_LEFT_AREA_BOTTOMLEFT;
			Tome.LEFT_TRIS[1].Edge0 = new Vector3(-5.3f, -0.1f, 0f);
			Tome.LEFT_TRIS[1].Edge1 = new Vector3(0f, 0f, 5.5f);
			Tome.RIGHT_TRIS = new Triangle[2];
			Tome.RIGHT_TRIS[0].Origin = Tome.PICK_RIGHT_AREA_TOPRIGHT;
			Tome.RIGHT_TRIS[0].Edge0 = new Vector3(5.3f, -0.1f, 0f);
			Tome.RIGHT_TRIS[0].Edge1 = new Vector3(0f, 0f, -5.5f);
			Tome.RIGHT_TRIS[1].Origin = Tome.PICK_RIGHT_AREA_BOTTOMLEFT - new Vector3(0f, 0.1f, 0f);
			Tome.RIGHT_TRIS[1].Edge0 = new Vector3(-5.3f, 0.1f, 0f);
			Tome.RIGHT_TRIS[1].Edge1 = new Vector3(0f, 0f, 5.5f);
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x0008DF74 File Offset: 0x0008C174
		private Tome()
		{
			this.currentExtraButtonIndex = -1;
			SkinnedModel skinnedModel;
			SkinnedModel skinnedModel2;
			SkinnedModel skinnedModel3;
			SkinnedModel skinnedModel4;
			SkinnedModel skinnedModel5;
			Texture2D texture2D;
			lock (Game.Instance.GraphicsDevice)
			{
				skinnedModel = Game.Instance.Content.Load<SkinnedModel>("UI/ToM/background_mesh");
				skinnedModel2 = Game.Instance.Content.Load<SkinnedModel>("UI/ToM/background");
				skinnedModel3 = Game.Instance.Content.Load<SkinnedModel>("UI/ToM/tome");
				skinnedModel4 = Game.Instance.Content.Load<SkinnedModel>("UI/ToM/page_flip");
				skinnedModel5 = Game.Instance.Content.Load<SkinnedModel>("UI/ToM/page");
				texture2D = Game.Instance.Content.Load<Texture2D>("UI/ToM/tome_pages_0");
			}
			Texture2D texture2D2;
			Microsoft.Xna.Framework.Rectangle rectangle;
			PdxWidgetWindow.GetTexture(PdxWidgetWindow.Textures.progress_indicator_on, out texture2D2, out rectangle);
			float num = (float)rectangle.Left / (float)texture2D2.Width;
			float num2 = (float)rectangle.Top / (float)texture2D2.Height;
			int num3 = rectangle.Height / 2;
			int num4 = rectangle.Width / 2;
			int num5 = rectangle.Height / 2;
			float num6 = (float)rectangle.Width / (float)texture2D2.Width;
			float num7 = (float)rectangle.Height / (float)texture2D2.Height;
			Texture2D texture2D3 = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
			Tome.sFaceBook = new MenuImageTextItem(default(Vector2), texture2D3, new Vector2(1070f / (float)texture2D3.Width, 830f / (float)texture2D3.Height), new Vector2(104f / (float)texture2D3.Width, 66f / (float)texture2D3.Height), 0, default(Vector2), TextAlign.Center, FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold), Tome.FACEBOOK_SIZE);
			Tome.sTwitter = new MenuImageTextItem(default(Vector2), texture2D3, new Vector2(1174f / (float)texture2D3.Width, 830f / (float)texture2D3.Height), new Vector2(104f / (float)texture2D3.Width, 66f / (float)texture2D3.Height), 0, default(Vector2), TextAlign.Center, FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold), Tome.TWITTER_SIZE);
			if (Tome.sSplashNew == null)
			{
				Tome.sSplashNew = new MenuImageTextItem(Vector2.Zero, texture2D3, new Vector2(1081f / (float)texture2D3.Width, 896f / (float)texture2D3.Height), new Vector2(82f / (float)texture2D3.Width, 38f / (float)texture2D3.Height), 0, Vector2.Zero, TextAlign.Center, FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold), Tome.SPLASH_NEW_SIZE);
			}
			Tome.sFaceBook.Alpha = 0f;
			Tome.sTwitter.Alpha = 0f;
			Tome.sSplashNew.Alpha = 0f;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold);
			Texture2D texture2D4 = Game.Instance.Content.Load<Texture2D>("UI/Menu/tag_spritesheet");
			Tome.sParadoxIcon = new MenuImageTextItem(Vector2.Zero, texture2D4, new Vector2(Tome.PARADOX_LOGO_UV.X / (float)texture2D4.Width, Tome.PARADOX_LOGO_UV.Y / (float)texture2D4.Height), new Vector2(Tome.PARADOX_LOGO_UV_SIZE.X / (float)texture2D4.Width, Tome.PARADOX_LOGO_UV_SIZE.Y / (float)texture2D4.Height), 0, Vector2.Zero, TextAlign.Center, font2, Tome.PARADOX_LOGO_SIZE);
			Tome.sIndicatorBackground = new MenuImageTextItem(Vector2.Zero, texture2D3, new Vector2(Tome.ACCOUNT_BACKGROUND_UV.X / (float)texture2D3.Width, Tome.ACCOUNT_BACKGROUND_UV.Y / (float)texture2D3.Height), new Vector2(Tome.ACCOUNT_BACKGROUND_UV_SIZE.X / (float)texture2D3.Width, Tome.ACCOUNT_BACKGROUND_UV_SIZE.Y / (float)texture2D3.Height), "#acc_loggedout".GetHashCodeCustom(), Tome.ACCOUNT_BACKGROUND_SIZE / 2f, TextAlign.Center, font, Tome.ACCOUNT_BACKGROUND_SIZE);
			Tome.sAccountCreationBtn = new MenuTextButtonItem(Vector2.Zero, texture2D3, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, "#acc_create_acc".GetHashCodeCustom(), font, 230f, 230f, TextAlign.Center);
			Tome.sAccountLoginBtn = new MenuTextButtonItem(Vector2.Zero, texture2D3, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, "#acc_log_in".GetHashCodeCustom(), font, 230f, 230f, TextAlign.Center);
			Tome.sIndicatorBackground.Alpha = 0f;
			Tome.sParadoxIcon.Alpha = 0f;
			Tome.sIndicatorBackground.Alpha = 0f;
			Tome.sAccountLoginBtn.Alpha = 0f;
			Tome.sAccountCreationBtn.Alpha = 0f;
			Tome.sSteamLinkPopup = new MenuMessagePopup();
			Tome.sSteamLinkPopup.Alignment = (PopupAlign.Middle | PopupAlign.Center);
			Tome.sProcessingPopup = new MenuMessagePopup();
			Tome.sProcessingPopup.Alignment = (PopupAlign.Middle | PopupAlign.Center);
			Tome.sProcessingPopup.ClearOnHide = false;
			Tome.sProcessingPopup.CanDismiss = false;
			Tome.sProcessingPopup.SetMessage(Tome.LOC_PROCESSING, Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR);
			Tome.sProcessingPopup.SetButtonType(ButtonConfig.None);
			Tome.sProcessingPopup.EnableLoadingIcon();
			ParadoxAccount.OnBecameIdle = (ParadoxAccount.BecameIdleDelegate)Delegate.Combine(ParadoxAccount.OnBecameIdle, new ParadoxAccount.BecameIdleDelegate(delegate()
			{
				Tome.SetIndicatorState(Singleton<ParadoxAccount>.Instance.IsLoggedFull ? Tome.AccountIndicatorState.LoggedIn : Tome.AccountIndicatorState.LoggedOut);
				Singleton<PopupSystem>.Instance.ForceDismissCurrentPopupIfMatches(Tome.sProcessingPopup);
			}));
			ParadoxAccount.OnBecameBusy = (ParadoxAccount.BecameBusyDelegate)Delegate.Combine(ParadoxAccount.OnBecameBusy, new ParadoxAccount.BecameBusyDelegate(delegate()
			{
				Singleton<PopupSystem>.Instance.ShowPopupImmediately(Tome.sProcessingPopup);
			}));
			this.mNewContentUpdateTimer = 600f;
			Tome.sVersionText = new Text(32, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Left, false);
			Tome.sVersionText.ShadowAlpha = 1f;
			Tome.sVersionText.ShadowsOffset = Vector2.One;
			Tome.sVersionText.DrawShadows = true;
			Tome.sVersionText.SetText(Application.ProductVersion);
			while (HackHelper.LicenseStatus == HackHelper.Status.Pending)
			{
				Thread.Sleep(1);
			}
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				Tome.sVersionText.Append(" (Modified)");
			}
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				OptionsMessageBox mbox = new OptionsMessageBox("#notice_mod".GetHashCode(), new int[]
				{
					"#add_menu_ok".GetHashCode()
				});
				LanguageManager.Instance.LanguageChanged -= new Action(mbox.LanguageChanged);
				this.OnOpen = (Action)Delegate.Combine(this.OnOpen, new Action(delegate()
				{
					mbox.Show();
				}));
			}
			this.OnOpen = (Action)Delegate.Combine(this.OnOpen, new Action(delegate()
			{
				Tome.sFadeInAccountIndicator = true;
			}));
			foreach (SkinnedModelBone skinnedModelBone in skinnedModel2.SkeletonBones)
			{
				if (skinnedModelBone.Name.Equals("light", StringComparison.OrdinalIgnoreCase))
				{
					Matrix matrix;
					Matrix.CreateRotationY(3.1415927f, out matrix);
					Matrix inverseBindPoseTransform = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref inverseBindPoseTransform, ref matrix, out inverseBindPoseTransform);
					Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
					Tome.sLightPosition = inverseBindPoseTransform.Translation;
				}
				else
				{
					if (skinnedModelBone.Name.Equals("camera", StringComparison.OrdinalIgnoreCase))
					{
						this.mCameraBone = (int)skinnedModelBone.Index;
						this.mCameraBindPose = skinnedModelBone.InverseBindPoseTransform;
						Matrix.Invert(ref this.mCameraBindPose, out this.mCameraBindPose);
						Matrix matrix2;
						Matrix.CreateRotationY(3.1415927f, out matrix2);
						Matrix.Multiply(ref this.mCameraBindPose, ref matrix2, out this.mCameraBindPose);
						break;
					}
					if (skinnedModelBone.Name.Equals("camera", StringComparison.OrdinalIgnoreCase))
					{
						this.mTomeBone = (int)skinnedModelBone.Index;
						this.mTomeBindPose = skinnedModelBone.InverseBindPoseTransform;
						Matrix.Invert(ref this.mTomeBindPose, out this.mTomeBindPose);
						Matrix matrix3;
						Matrix.CreateRotationY(3.1415927f, out matrix3);
						Matrix.Multiply(ref this.mTomeBindPose, ref matrix3, out this.mTomeBindPose);
						break;
					}
				}
			}
			this.mCameraAnimation = new AnimationController();
			this.mCameraAnimation.Skeleton = skinnedModel2.SkeletonBones;
			this.mCameraAnimations = new AnimationClip[6];
			foreach (AnimationClip animationClip in skinnedModel2.AnimationClips.Values)
			{
				if (animationClip.Name.Equals("wakeup", StringComparison.OrdinalIgnoreCase))
				{
					this.mCameraAnimations[0] = animationClip;
				}
				else if (animationClip.Name.Equals("idle", StringComparison.OrdinalIgnoreCase))
				{
					this.mCameraAnimations[1] = animationClip;
				}
				else if (animationClip.Name.Equals("lookleft", StringComparison.OrdinalIgnoreCase))
				{
					this.mCameraAnimations[2] = animationClip;
				}
				else if (animationClip.Name.Equals("lookback", StringComparison.OrdinalIgnoreCase))
				{
					this.mCameraAnimations[3] = animationClip;
				}
				else if (animationClip.Name.Equals("dozeoff", StringComparison.OrdinalIgnoreCase))
				{
					this.mCameraAnimations[4] = animationClip;
				}
				else if (animationClip.Name.Equals("exitlevel", StringComparison.OrdinalIgnoreCase))
				{
					this.mCameraAnimations[5] = animationClip;
				}
			}
			for (int i = 0; i < skinnedModel.Model.Meshes.Count; i++)
			{
				ModelMesh modelMesh = skinnedModel.Model.Meshes[i];
				for (int j = 0; j < modelMesh.Effects.Count; j++)
				{
					SkinnedModelBasicEffect skinnedModelBasicEffect = modelMesh.Effects[j] as SkinnedModelBasicEffect;
					skinnedModelBasicEffect.SetTechnique(SkinnedModelBasicEffect.Technique.NonDeffered);
					skinnedModelBasicEffect.LightPosition = new Vector3(5f, 5f, 0f);
					skinnedModelBasicEffect.LightAmbientColor = new Vector3(0.4f);
					skinnedModelBasicEffect.LightDiffuseColor = new Vector3(0.7f);
					skinnedModelBasicEffect.LightSpecularColor = new Vector3(0.4f);
					skinnedModelBasicEffect.NormalPower = 1f;
					skinnedModelBasicEffect.LightPosition = new Vector3(5.25f, 5f, 3.5f);
					skinnedModelBasicEffect.LightSpecularColor = new Vector3(4f);
				}
			}
			this.mTomeClips = new AnimationClip[3];
			this.mTomeClips[0] = skinnedModel3.AnimationClips["open"];
			this.mTomeClips[1] = skinnedModel3.AnimationClips["close"];
			this.mTomeClips[2] = skinnedModel3.AnimationClips["close"];
			for (int k = 0; k < skinnedModel3.Model.Meshes.Count; k++)
			{
				ModelMesh modelMesh2 = skinnedModel3.Model.Meshes[k];
				for (int l = 0; l < modelMesh2.Effects.Count; l++)
				{
					SkinnedModelBasicEffect skinnedModelBasicEffect2 = modelMesh2.Effects[l] as SkinnedModelBasicEffect;
					skinnedModelBasicEffect2.SetTechnique(SkinnedModelBasicEffect.Technique.NonDeffered);
					skinnedModelBasicEffect2.LightPosition = new Vector3(5f, 5f, 0f);
					skinnedModelBasicEffect2.LightAmbientColor = new Vector3(0.4f);
					skinnedModelBasicEffect2.LightDiffuseColor = new Vector3(0.7f);
					skinnedModelBasicEffect2.LightSpecularColor = new Vector3(0.4f);
					skinnedModelBasicEffect2.NormalPower = 1f;
					skinnedModelBasicEffect2.LightPosition = new Vector3(5.25f, 5f, 3.5f);
					skinnedModelBasicEffect2.LightSpecularColor = new Vector3(4f);
				}
			}
			this.mTomeController = new AnimationController();
			this.mTomeController.Skeleton = skinnedModel3.SkeletonBones;
			this.mBackGroundPageControllerLeft = new AnimationController();
			this.mBackGroundPageControllerLeft.Skeleton = skinnedModel5.SkeletonBones;
			this.mBackGroundPageControllerRight = new AnimationController();
			this.mBackGroundPageControllerRight.Skeleton = skinnedModel5.SkeletonBones;
			for (int m = 0; m < skinnedModel5.Model.Meshes.Count; m++)
			{
				ModelMesh modelMesh3 = skinnedModel5.Model.Meshes[m];
				for (int n = 0; n < modelMesh3.Effects.Count; n++)
				{
					SkinnedModelBasicEffect skinnedModelBasicEffect3 = modelMesh3.Effects[n] as SkinnedModelBasicEffect;
					skinnedModelBasicEffect3.SetTechnique(SkinnedModelBasicEffect.Technique.NonDeffered);
					skinnedModelBasicEffect3.LightPosition = new Vector3(5f, 5f, 0f);
					skinnedModelBasicEffect3.LightAmbientColor = new Vector3(0.4f);
					skinnedModelBasicEffect3.LightDiffuseColor = new Vector3(0.7f);
					skinnedModelBasicEffect3.LightSpecularColor = new Vector3(0.4f);
					skinnedModelBasicEffect3.NormalPower = 1f;
					skinnedModelBasicEffect3.LightPosition = new Vector3(5.25f, 5f, 3.5f);
					skinnedModelBasicEffect3.LightSpecularColor = new Vector3(4f);
				}
			}
			for (int num8 = 0; num8 < skinnedModel4.Model.Meshes.Count; num8++)
			{
				ModelMesh modelMesh4 = skinnedModel4.Model.Meshes[num8];
				for (int num9 = 0; num9 < modelMesh4.Effects.Count; num9++)
				{
					SkinnedModelBasicEffect skinnedModelBasicEffect4 = modelMesh4.Effects[num9] as SkinnedModelBasicEffect;
					skinnedModelBasicEffect4.SetTechnique(SkinnedModelBasicEffect.Technique.NonDeffered);
					skinnedModelBasicEffect4.LightPosition = new Vector3(5f, 5f, 0f);
					skinnedModelBasicEffect4.LightAmbientColor = new Vector3(0.4f);
					skinnedModelBasicEffect4.LightDiffuseColor = new Vector3(0.7f);
					skinnedModelBasicEffect4.LightSpecularColor = new Vector3(0.4f);
					skinnedModelBasicEffect4.NormalPower = 1f;
					skinnedModelBasicEffect4.LightPosition = new Vector3(5.25f, 5f, 3.5f);
					skinnedModelBasicEffect4.LightSpecularColor = new Vector3(4f);
				}
			}
			this.mBackGroundClips = new AnimationClip[4];
			this.mBackGroundClips[0] = skinnedModel4.AnimationClips["open_left"];
			this.mBackGroundClips[1] = skinnedModel4.AnimationClips["open_right"];
			this.mBackGroundClips[2] = skinnedModel4.AnimationClips["close_left"];
			this.mBackGroundClips[3] = skinnedModel4.AnimationClips["close_right"];
			this.mRiffleController = new Tome.RiffleController(Tome.MaxNrOfPages, skinnedModel4);
			GraphicsDevice graphicsDevice2 = RenderManager.Instance.GraphicsDevice;
			RenderTarget2D renderTarget2D = new RenderTarget2D(graphicsDevice2, texture2D.Width, texture2D.Height, 3, SurfaceFormat.Color);
			RenderTarget2D iShadowMap = new RenderTarget2D(graphicsDevice2, 1024, 1024, 1, SurfaceFormat.Single);
			DepthStencilBuffer iShadowDepthBuffer = new DepthStencilBuffer(graphicsDevice2, 1024, 1024, DepthFormat.Depth24);
			GUIBasicEffect guibasicEffect = new GUIBasicEffect(graphicsDevice2, null);
			guibasicEffect.SetScreenSize(renderTarget2D.Width, renderTarget2D.Height);
			guibasicEffect.TextureEnabled = true;
			guibasicEffect.ScaleToHDR = false;
			guibasicEffect.Color = new Vector4(1f);
			VertexPositionTexture[] array = new VertexPositionTexture[]
			{
				new VertexPositionTexture(new Vector3(0f, (float)renderTarget2D.Height, 0f), new Vector2(0f, 1f)),
				new VertexPositionTexture(new Vector3(0f), new Vector2(0f)),
				new VertexPositionTexture(new Vector3((float)renderTarget2D.Width, 0f, 0f), new Vector2(1f, 0f)),
				new VertexPositionTexture(new Vector3((float)renderTarget2D.Width, (float)renderTarget2D.Height, 0f), new Vector2(1f, 1f))
			};
			VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice2, VertexPositionTexture.SizeInBytes * array.Length, BufferUsage.None);
			vertexBuffer.SetData<VertexPositionTexture>(array);
			VertexDeclaration iVertexDeclaration = new VertexDeclaration(graphicsDevice2, VertexPositionTexture.VertexElements);
			this.mMenuStack = new SubMenu[32];
			this.mMenuRiffleStack = new Stack<int>(32);
			this.mInputLocked = false;
			this.CloseToMenu = false;
			this.mRenderData = new Tome.RenderData[3];
			for (int num10 = 0; num10 < 3; num10++)
			{
				this.mRenderData[num10] = new Tome.RenderData(renderTarget2D, iShadowMap, iShadowDepthBuffer, skinnedModel3, skinnedModel4, skinnedModel5, skinnedModel, vertexBuffer, texture2D, iVertexDeclaration, guibasicEffect);
			}
			this.TargetLightIntensity = 0f;
			this.mCurrentState = Tome.ClosedState.Instance;
			this.mCurrentState.OnEnter(this);
			SteamAPI.DlcInstalled += this.SteamAPI_DLCInstalled;
			LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
			DLC_StatusHelper.OnDynamicPromotionsLoaded = (Action)Delegate.Remove(DLC_StatusHelper.OnDynamicPromotionsLoaded, new Action(Tome.ReLoadPromotionAfterDynamicLoadingComplete));
			DLC_StatusHelper.OnDynamicPromotionsLoaded = (Action)Delegate.Combine(DLC_StatusHelper.OnDynamicPromotionsLoaded, new Action(Tome.ReLoadPromotionAfterDynamicLoadingComplete));
			Singleton<GameSparksProperties>.Instance.OnPropertiesLoaded -= new Action(Tome.ReLoadPromotionAfterDynamicLoadingComplete);
			Singleton<GameSparksProperties>.Instance.OnPropertiesLoaded += new Action(Tome.ReLoadPromotionAfterDynamicLoadingComplete);
			DLC_StatusHelper instance = DLC_StatusHelper.Instance;
			Thread.Sleep(0);
			Tome.LoadNewPromotion();
			List<PromotionInfo> list = DLC_StatusHelper.Instance.Splash_GetAllOwned();
			List<EventData> list2 = new List<EventData>();
			foreach (PromotionInfo promotionInfo in list)
			{
				list2.Add(new EventData("dlc", new object[]
				{
					Singleton<GameSparksAccount>.Instance.Variant,
					Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
					SteamApps.BIsDlcInstalled(promotionInfo.AppID) ? 1 : 0,
					promotionInfo.Name,
					promotionInfo.AppID
				}));
			}
			Singleton<ParadoxServices>.Instance.TelemetryEvent(list2.ToArray());
			Tome.ChangeResolution(GlobalSettings.Instance.Resolution);
			ResolutionMessageBox.Instance.Complete += Tome.ChangeResolution;
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x0008F1FC File Offset: 0x0008D3FC
		public static void ChangeResolution(ResolutionData iData)
		{
			if (Tome.sParadoxIcon != null)
			{
				Tome.RefreshParadoxAccount(iData);
				Tome.RefreshUpsell(iData);
			}
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x0008F214 File Offset: 0x0008D414
		private static void RefreshParadoxAccount(ResolutionData iData)
		{
			float num = (float)iData.Height / PopupSystem.REFERENCE_SIZE.Y;
			Vector2 zero = Vector2.Zero;
			Tome.sParadoxIcon.Scale = num;
			Tome.sIndicatorBackground.Scale = num;
			Tome.sAccountLoginBtn.Scale = num;
			Tome.sAccountCreationBtn.Scale = num;
			Tome.sIndicatorBackground.TextPosition = Tome.ACCOUNT_BACKGROUND_SIZE * 0.5f * num;
			zero.X = (float)iData.Width - Tome.ACCOUNT_BACKGROUND_SIZE.X * num - 11f;
			zero.Y = 20f;
			Tome.sIndicatorBackground.Position = zero;
			zero.X += (Tome.ACCOUNT_BACKGROUND_SIZE.X - Tome.PARADOX_LOGO_SIZE.X) * 0.5f * num;
			zero.Y += 40f * num;
			Tome.sParadoxIcon.Position = zero;
			zero.X = (float)iData.Width - Tome.ACCOUNT_BACKGROUND_SIZE.X * 0.5f * num - 11f;
			zero.Y = 20f + Tome.ACCOUNT_BACKGROUND_SIZE.Y * 0.5f * num;
			zero.Y += 50f * num;
			Tome.sAccountLoginBtn.Position = zero;
			zero.Y += 50f * num + 25f * num;
			Tome.sAccountCreationBtn.Position = zero;
		}

		// Token: 0x06001656 RID: 5718 RVA: 0x0008F39C File Offset: 0x0008D59C
		private static void RefreshUpsell(ResolutionData iData)
		{
			float num = (float)iData.Height / PopupSystem.REFERENCE_SIZE.Y;
			Vector2 zero = Vector2.Zero;
			zero.X = (float)iData.Width - Tome.TWITTER_SIZE.X * num - 11f;
			zero.Y = (float)iData.Height - Tome.TWITTER_SIZE.Y * num - 11f;
			Tome.sTwitter.Position = zero;
			Tome.sTwitter.Scale = num;
			zero.X -= Tome.FACEBOOK_SIZE.X * num + 5f;
			Tome.sFaceBook.Position = zero;
			Tome.sFaceBook.Scale = num;
			if (Tome.sPromotionActive)
			{
				zero.Y -= Tome.sPromotionButton.Size.Y * num + 11f;
				zero.X = (float)iData.Width - (Tome.sPromotionButton.Size.X * num + 11f);
				Tome.sPromotionButton.Position = zero;
				Tome.sPromotionButton.Scale = num;
				if (Tome.sShownSplashIsNew)
				{
					zero.X += Tome.sPromotionButton.Size.X * num - (Tome.SPLASH_NEW_SIZE.X * num + 3f);
					zero.Y += Tome.sPromotionButton.Size.Y * num - (Tome.SPLASH_NEW_SIZE.Y * num + 3f);
					Tome.sSplashNew.Position = zero;
					Tome.sSplashNew.Scale = num;
				}
			}
		}

		// Token: 0x06001657 RID: 5719 RVA: 0x0008F540 File Offset: 0x0008D740
		public static void ReLoadPromotionAfterDynamicLoadingComplete()
		{
			Tome.PromotionActive = Singleton<GameSparksProperties>.Instance.GetProperty<bool>("ABTestAdLevel", "Ads");
			if (Tome.sPromotionActive)
			{
				Tome.sPromotionButton = DLC_StatusHelper.Instance.Splash_GetMenuItem(Vector2.Zero, Tome.PROMOTION_BUTTON_POSITION, out Tome.sShownSplashIsNew);
			}
			Tome.RefreshUpsell(GlobalSettings.Instance.Resolution);
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x0008F59C File Offset: 0x0008D79C
		public static void LoadNewPromotion()
		{
			if (Tome.sPromotionActive)
			{
				DLC_StatusHelper instance = DLC_StatusHelper.Instance;
				bool flag = false;
				if (Tome.sPromotionButton != null)
				{
					if (!instance.PromotionListIsLocked)
					{
						instance.Splash_TrySelectNextDLC();
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					Tome.sPromotionButton = instance.Splash_GetMenuItem(Vector2.Zero, Tome.PROMOTION_BUTTON_POSITION, out Tome.sShownSplashIsNew);
					Tome.sPromotionButton.Alpha = Tome.sIndicatorBackground.Alpha;
					Tome.RefreshUpsell(GlobalSettings.Instance.Resolution);
				}
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06001659 RID: 5721 RVA: 0x0008F613 File Offset: 0x0008D813
		public Tome.TomeState CurrentState
		{
			get
			{
				return this.mCurrentState;
			}
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x0008F61C File Offset: 0x0008D81C
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			Tome.RenderData renderData = this.mRenderData[(int)iDataChannel];
			Vector3 vector = new Vector3(0.3f, 0.2f, 0.2f);
			Vector3 vector2 = new Vector3(0.7f, 0.75f, 0.75f);
			Vector3 vector3 = new Vector3(1.3f, 1f, 1f);
			this.mIntroTime += iDeltaTime;
			if (this.mHurryAndOpen && this.mCameraAnimation.AnimationClip == this.mCameraAnimations[0] && this.mCameraAnimation.HasFinished)
			{
				this.ChangeState(Tome.OpeningState.Instance);
			}
			if (this.mCameraAnimation.AnimationClip == this.mCameraAnimations[0])
			{
				if (this.mIntroTime > 1.5f || this.mCameraAnimation.ClipSpeed > 1f)
				{
					this.mTargetLightIntensity = 1f;
				}
				else
				{
					if (!this.mCameraAnimation.CrossFadeEnabled && !this.mCameraAnimation.IsPlaying)
					{
						this.PlayCameraAnimation(Tome.CameraAnimation.Wake_Up);
					}
					this.mTargetLightIntensity = 0f;
				}
			}
			this.mLightVariationSpeed += (this.mTargetLightVariationSpeed - this.mLightVariationSpeed) * iDeltaTime * 5f;
			this.mLightVariationAmount += (this.mTargetLightVariationAmount - this.mLightVariationAmount) * iDeltaTime * 5f;
			this.mLightIntensity += (this.mTargetLightIntensity - this.mLightIntensity) * iDeltaTime * this.mLightVariationSpeed;
			this.mLightVariationPosition += ((float)Tome.sRandom.NextDouble() - 0.5f) * 2f * iDeltaTime * this.mLightVariationSpeed;
			this.mLightVariationPosition = MathHelper.Clamp(this.mLightVariationPosition, -1f, 1f);
			float num = this.mLightIntensity + this.mLightVariationPosition * this.mLightVariationAmount;
			num *= 100f;
			Vector3.Multiply(ref vector, num, out renderData.LightAmbient);
			Vector3.Multiply(ref vector2, num, out renderData.LightDiffuse);
			Vector3.Multiply(ref vector3, num, out renderData.LightSpecular);
			if (this.mCurrentState == Tome.ClosedState.Instance && this.currentExtraButtonIndex != -1)
			{
				renderData.LightDiffuse *= 0.5f;
				renderData.LightSpecular *= 0.8f;
			}
			renderData.State = this.mCurrentState;
			Tome.mCurrentTime += iDeltaTime;
			if (Tome.mCurrentTime > 6.2831855f)
			{
				Tome.mCurrentTime = 0f;
			}
			if (this.mMenuStackPosition >= 0)
			{
				SubMenu subMenu = this.mMenuStack[this.mMenuStackPosition];
				renderData.CurrentMenu = subMenu;
				subMenu.Update(iDataChannel, iDeltaTime);
			}
			Matrix matrix;
			Matrix.Multiply(ref this.mCameraBindPose, ref this.mCameraAnimation.SkinnedBoneTransforms[this.mCameraBone], out matrix);
			Tome.mCameraPosition = matrix.Translation;
			Matrix identity = Matrix.Identity;
			this.mCameraAnimation.Update(iDeltaTime, ref identity, true);
			Point screenSize = RenderManager.Instance.ScreenSize;
			Matrix.Invert(ref matrix, out renderData.View);
			Matrix.CreatePerspectiveFieldOfView(0.75f, (float)screenSize.X / (float)screenSize.Y, 2f, 200f, out renderData.Projection);
			Matrix.Multiply(ref renderData.View, ref renderData.Projection, out renderData.ViewProjection);
			Tome.mViewProjection = renderData.ViewProjection;
			this.mRiffleController.ArrayCopyTo(ref renderData.mPageBones);
			this.mCurrentState.Update(iDeltaTime, iDataChannel, this);
			if (this.mNewContentUpdateTimer > 600f)
			{
				this.mNewContentUpdateTimer -= 600f;
				WebParser.CheckSteamDLCs(new Action<bool>(this.NewDLCCheckCallback));
			}
			this.mNewContentUpdateTimer += iDeltaTime;
			renderData.State = this.mCurrentState;
			GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
			if (Tome.sFadeInAccountIndicator && (this.mRiffleController.HasFinished || this.mTomeController.IsPlaying))
			{
				float num2 = Tome.sIndicatorBackground.Alpha;
				num2 += iDeltaTime / 0.25f;
				if (num2 >= 1f)
				{
					Tome.sFadeInAccountIndicator = false;
				}
				Tome.sIndicatorBackground.Alpha = num2;
				Tome.sParadoxIcon.Alpha = num2;
				Tome.sIndicatorBackground.Alpha = num2;
				Tome.sAccountLoginBtn.Alpha = num2;
				Tome.sAccountCreationBtn.Alpha = num2;
				Tome.sFaceBook.Alpha = num2;
				Tome.sTwitter.Alpha = num2;
				if (Tome.sPromotionButton != null)
				{
					Tome.sPromotionButton.Alpha = num2;
				}
				Tome.sSplashNew.Alpha = num2;
			}
			if (!Tome.sFadeInAccountIndicator && (!this.mRiffleController.HasFinished || this.mTomeController.IsPlaying) && Singleton<ParadoxAccount>.Instance.PendingErrorCode != ParadoxAccount.ErrorCode.None)
			{
				ParadoxPopupUtils.ShowErrorPopup(Singleton<ParadoxAccount>.Instance.ConsumePendingErrorCode());
			}
			if (!this.mRiffleController.HasFinished || this.mTomeController.IsPlaying)
			{
				this.mInputLocked = true;
				return;
			}
			this.mInputLocked = false;
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x0008FB18 File Offset: 0x0008DD18
		private void LanguageChanged()
		{
			Tome.sVersionText.MarkAsDirty();
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x0008FB24 File Offset: 0x0008DD24
		internal void NewDLCCheckCallback(bool iNewContent)
		{
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x0008FB26 File Offset: 0x0008DD26
		private void SteamAPI_DLCInstalled(DlcInstalled iDLC)
		{
			this.mNewContentUpdateTimer = 600f;
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x0008FB33 File Offset: 0x0008DD33
		public void ChangeState(Tome.TomeState iState)
		{
			this.mCurrentState.OnExit(this);
			this.mCurrentState = iState;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x0600165F RID: 5727 RVA: 0x0008FB54 File Offset: 0x0008DD54
		public SubMenu CurrentMenu
		{
			get
			{
				if (this.mMenuStackPosition < 0)
				{
					return null;
				}
				return this.mMenuStack[this.mMenuStackPosition];
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x06001660 RID: 5728 RVA: 0x0008FB6E File Offset: 0x0008DD6E
		public SubMenu PreviousMenu
		{
			get
			{
				if (this.mMenuStackPosition <= 0)
				{
					return null;
				}
				return this.mMenuStack[this.mMenuStackPosition - 1];
			}
		}

		// Token: 0x06001661 RID: 5729 RVA: 0x0008FB8C File Offset: 0x0008DD8C
		public static void SetIndicatorState(Tome.AccountIndicatorState iState)
		{
			switch (iState)
			{
			case Tome.AccountIndicatorState.LoggedIn:
				Tome.sAccountCreationBtn.Enabled = false;
				Tome.sAccountLoginBtn.Enabled = false;
				Tome.sIndicatorBackground.SetText(LanguageManager.Instance.GetString(Tome.LOC_LOGGEDIN));
				return;
			case Tome.AccountIndicatorState.LoggedOut:
				Tome.sAccountCreationBtn.Enabled = true;
				Tome.sAccountLoginBtn.Enabled = true;
				Tome.sIndicatorBackground.SetText(LanguageManager.Instance.GetString(Tome.LOC_LOGGEDOUT));
				return;
			default:
				return;
			}
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x0008FC0C File Offset: 0x0008DE0C
		public void PushMenuInstant(SubMenu iMenu, int iRiffles)
		{
			this.mMenuStack[this.mMenuStackPosition].OnExit();
			this.mMenuStack[this.mMenuStackPosition + 1] = iMenu;
			this.mMenuStackPosition++;
			this.mMenuStack[this.mMenuStackPosition].OnEnter();
			this.mMenuRiffleStack.Push(iRiffles);
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x0008FC68 File Offset: 0x0008DE68
		public void PushMenu(SubMenu iMenu, int iRiffles)
		{
			if (this.mCurrentState != Tome.OpenState.Instance)
			{
				return;
			}
			this.mMenuStack[this.mMenuStackPosition].OnExit();
			this.mMenuStack[this.mMenuStackPosition + 1] = iMenu;
			this.mMenuStackPosition++;
			this.mMenuStack[this.mMenuStackPosition].OnEnter();
			this.mMenuRiffleStack.Push(iRiffles);
			if (iRiffles > 0)
			{
				this.ChangeState(Tome.RiffleForwardState.Instance);
			}
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x0008FCE0 File Offset: 0x0008DEE0
		public void PopMenuInstant()
		{
			if (this.mMenuStackPosition > 0)
			{
				this.currentExtraButtonIndex = -1;
				this.UnselectAllExtraButtons();
				this.mMenuStack[this.mMenuStackPosition].OnExit();
				this.mMenuStackPosition--;
				this.mMenuStack[this.mMenuStackPosition].OnEnter();
				this.mMenuRiffleStack.Pop();
			}
		}

		// Token: 0x06001665 RID: 5733 RVA: 0x0008FD44 File Offset: 0x0008DF44
		public void PopPreviousMenu()
		{
			this.currentExtraButtonIndex = -1;
			this.UnselectAllExtraButtons();
			this.mMenuStack[this.mMenuStackPosition - 1] = this.mMenuStack[this.mMenuStackPosition];
			this.mMenuStackPosition--;
			this.mMenuStack[this.mMenuStackPosition + 1] = null;
			this.mMenuRiffleStack.Pop();
		}

		// Token: 0x06001666 RID: 5734 RVA: 0x0008FDA4 File Offset: 0x0008DFA4
		public void PopMenu()
		{
			if (this.mCurrentState != Tome.OpenState.Instance)
			{
				return;
			}
			this.currentExtraButtonIndex = -1;
			this.UnselectAllExtraButtons();
			if (this.mMenuStackPosition > 0 && GameStateManager.Instance.CurrentState is MenuState)
			{
				this.mMenuStack[this.mMenuStackPosition].OnExit();
				this.mMenuStackPosition--;
				this.mMenuStack[this.mMenuStackPosition].OnEnter();
				this.ChangeState(Tome.RiffleBackwardState.Instance);
				this.mMenuRiffleStack.Pop();
				if (!Singleton<ParadoxAccount>.Instance.IsBusy)
				{
					Tome.SetIndicatorState(Singleton<ParadoxAccount>.Instance.IsLoggedFull ? Tome.AccountIndicatorState.LoggedIn : Tome.AccountIndicatorState.LoggedOut);
					return;
				}
			}
			else
			{
				if (GameStateManager.Instance.CurrentState is PlayState)
				{
					this.mMenuStack[this.mMenuStackPosition].OnExit();
					this.mMenuStackPosition--;
					this.mMenuStack[this.mMenuStackPosition].OnEnter();
					this.ChangeState(Tome.RiffleBackwardState.Instance);
					this.mMenuRiffleStack.Pop();
					return;
				}
				this.ChangeState(Tome.ClosingState.Instance);
				this.mMenuStack[this.mMenuStackPosition].OnExit();
			}
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x0008FECC File Offset: 0x0008E0CC
		public void Riffle(PlaybackMode iMode, int iPages)
		{
			this.mMenuRiffleStack.Push(iPages);
			if (iMode == PlaybackMode.Forward)
			{
				this.ChangeState(Tome.RiffleForwardState.Instance);
			}
			else
			{
				this.ChangeState(Tome.RiffleBackwardState.Instance);
			}
			this.mMenuRiffleStack.Pop();
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06001668 RID: 5736 RVA: 0x0008FF01 File Offset: 0x0008E101
		public bool CameraMoving
		{
			get
			{
				return this.mCameraAnimation.IsPlaying;
			}
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x0008FF0E File Offset: 0x0008E10E
		public void SetCameraAnimation(Tome.CameraAnimation iCameraAnimation)
		{
			this.mCameraAnimation.StartClip(this.mCameraAnimations[(int)iCameraAnimation], false);
			this.mCameraAnimation.Stop();
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x0008FF2F File Offset: 0x0008E12F
		public void PlayCameraAnimation(Tome.CameraAnimation iCameraAnimation)
		{
			this.mCameraAnimation.StartClip(this.mCameraAnimations[(int)iCameraAnimation], false);
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x0008FF45 File Offset: 0x0008E145
		public void CrossfadeCameraAnimation(Tome.CameraAnimation iCameraAnimation, float iTime)
		{
			this.mCameraAnimation.CrossFade(this.mCameraAnimations[(int)iCameraAnimation], iTime, false);
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x0008FF5C File Offset: 0x0008E15C
		public void CloseBack()
		{
			this.currentExtraButtonIndex = -1;
			this.ChangeState(Tome.CloseToBack.Instance);
		}

		// Token: 0x0600166D RID: 5741 RVA: 0x0008FF70 File Offset: 0x0008E170
		public void CloseTome()
		{
			this.currentExtraButtonIndex = -1;
			this.ChangeState(Tome.ClosingState.Instance);
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x0008FF84 File Offset: 0x0008E184
		public void CloseTomeInstant()
		{
			if (this.mInputLocked)
			{
				return;
			}
			this.currentExtraButtonIndex = -1;
			this.ChangeState(Tome.ClosedState.Instance);
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x0600166F RID: 5743 RVA: 0x0008FFA1 File Offset: 0x0008E1A1
		public bool MenuOverExtraButtons
		{
			get
			{
				return this.currentExtraButtonIndex != -1;
			}
		}

		// Token: 0x06001670 RID: 5744 RVA: 0x0008FFB0 File Offset: 0x0008E1B0
		public void UnselectAllExtraButtons()
		{
			if (Tome.sPromotionButton != null)
			{
				Tome.sPromotionButton.Selected = false;
			}
			Tome.sFaceBook.Selected = (Tome.sTwitter.Selected = false);
			Tome.sAccountCreationBtn.Selected = (Tome.sAccountLoginBtn.Selected = false);
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x00090000 File Offset: 0x0008E200
		private void NavigateExtraButtons(int parentMenuTopIndex, int parentMenuBottomIndex, int parentMenuLeftIndex, int parentMenuRightIndex, ref int parentMenuCurrIndex, ControllerDirection dir)
		{
			switch (dir)
			{
			case ControllerDirection.Right:
				if (this.currentExtraButtonIndex == -1)
				{
					this.currentExtraButtonIndex = (Singleton<ParadoxAccount>.Instance.IsLoggedFull ? 3 : 0);
				}
				else if (this.currentExtraButtonIndex == 3)
				{
					this.currentExtraButtonIndex = 4;
				}
				else
				{
					this.currentExtraButtonIndex = -1;
					parentMenuCurrIndex = parentMenuLeftIndex;
				}
				break;
			case ControllerDirection.Up:
				if (this.currentExtraButtonIndex == -1)
				{
					this.currentExtraButtonIndex = (Singleton<ParadoxAccount>.Instance.IsLoggedFull ? (Tome.PromotionActive ? 2 : 3) : 0);
				}
				else if (this.currentExtraButtonIndex == 2)
				{
					this.currentExtraButtonIndex = (Singleton<ParadoxAccount>.Instance.IsLoggedFull ? -1 : 1);
				}
				else if (this.currentExtraButtonIndex == 4)
				{
					this.currentExtraButtonIndex = (Tome.PromotionActive ? 2 : (Singleton<ParadoxAccount>.Instance.IsLoggedFull ? -1 : 1));
				}
				else if (this.currentExtraButtonIndex >= 0 && this.currentExtraButtonIndex <= 3)
				{
					this.currentExtraButtonIndex--;
					if (this.currentExtraButtonIndex == 2 && !Tome.PromotionActive)
					{
						this.currentExtraButtonIndex--;
					}
					if (this.currentExtraButtonIndex == 1 && Singleton<ParadoxAccount>.Instance.IsLoggedFull)
					{
						this.currentExtraButtonIndex = -1;
						parentMenuCurrIndex = parentMenuBottomIndex;
					}
				}
				else
				{
					this.currentExtraButtonIndex = -1;
					parentMenuCurrIndex = parentMenuBottomIndex;
				}
				break;
			case ControllerDirection.UpRight:
				break;
			case ControllerDirection.Left:
				if (this.currentExtraButtonIndex == -1)
				{
					this.currentExtraButtonIndex = (Singleton<ParadoxAccount>.Instance.IsLoggedFull ? (Tome.PromotionActive ? 2 : 3) : 0);
				}
				else if (this.currentExtraButtonIndex == 4)
				{
					this.currentExtraButtonIndex = 3;
				}
				else
				{
					this.currentExtraButtonIndex = -1;
					parentMenuCurrIndex = parentMenuRightIndex;
				}
				break;
			default:
				if (dir == ControllerDirection.Down)
				{
					if (this.currentExtraButtonIndex == -1)
					{
						this.currentExtraButtonIndex = (Singleton<ParadoxAccount>.Instance.IsLoggedFull ? (Tome.PromotionActive ? 2 : 3) : 0);
					}
					else if (this.currentExtraButtonIndex >= 0 && this.currentExtraButtonIndex <= 2)
					{
						this.currentExtraButtonIndex++;
						if (this.currentExtraButtonIndex == 2 && !Tome.PromotionActive)
						{
							this.currentExtraButtonIndex++;
						}
					}
					else
					{
						this.currentExtraButtonIndex = -1;
						parentMenuCurrIndex = parentMenuTopIndex;
					}
				}
				break;
			}
			this.UnselectAllExtraButtons();
			if (this.currentExtraButtonIndex == 1)
			{
				Tome.sAccountCreationBtn.Selected = true;
				return;
			}
			if (this.currentExtraButtonIndex == 0)
			{
				Tome.sAccountLoginBtn.Selected = true;
				return;
			}
			if (this.currentExtraButtonIndex == 2)
			{
				if (Tome.sPromotionButton != null)
				{
					Tome.sPromotionButton.Selected = true;
					return;
				}
			}
			else
			{
				if (this.currentExtraButtonIndex == 3)
				{
					Tome.sFaceBook.Selected = true;
					return;
				}
				if (this.currentExtraButtonIndex == 4)
				{
					Tome.sTwitter.Selected = true;
				}
			}
		}

		// Token: 0x06001672 RID: 5746 RVA: 0x000902B4 File Offset: 0x0008E4B4
		private bool ControllerA_OnExtraButton()
		{
			if (this.currentExtraButtonIndex > -1 && this.currentExtraButtonIndex <= 2)
			{
				switch (this.currentExtraButtonIndex)
				{
				case 0:
					this.PushMenu(SubMenuAccountLogin.Instance, 2);
					break;
				case 1:
					this.PushMenu(SubMenuAccountCreate.Instance, 1);
					break;
				case 2:
					if (Tome.PromotionActive)
					{
						this.OpenURL("http://store.steampowered.com/dlc/42910", true);
					}
					break;
				case 3:
					this.OpenURL("http://www.facebook.com/MagickaGame/", true);
					break;
				case 4:
					this.OpenURL("http://www.twitter.com/MagickaGame/", true);
					break;
				}
				if (this.mCurrentState == Tome.ClosedState.Instance)
				{
					this.currentExtraButtonIndex = -1;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001673 RID: 5747 RVA: 0x00090360 File Offset: 0x0008E560
		public void ControllerMovement(Controller iSender, ControllerDirection iDirection)
		{
			if (DialogManager.Instance.MessageBoxActive)
			{
				return;
			}
			if (this.mCurrentState == Tome.ClosedState.Instance)
			{
				int num = -1;
				this.NavigateExtraButtons(-1, -1, -1, -1, ref num, iDirection);
				return;
			}
			int currentlySelectedPosition = this.mMenuStack[this.mMenuStackPosition].CurrentlySelectedPosition;
			int num2 = this.mMenuStack[this.mMenuStackPosition].NumItems - 1;
			int num3 = 0;
			bool flag = false;
			if (!(this.CurrentMenu is SubMenuCutscene) && !(this.CurrentMenu is SubMenuIntro))
			{
				num3 = 0;
				if (this.CurrentMenu is SubMenuCampaignSelect_SaveSlotSelect)
				{
					num3 = 3;
				}
				int num4 = 1;
				bool flag2;
				if (this.CurrentMenu is SubMenuMain)
				{
					flag2 = ((currentlySelectedPosition == num3 || currentlySelectedPosition == num4) && iDirection == ControllerDirection.Up);
				}
				else
				{
					flag2 = (!(this.CurrentMenu is SubMenuOnline) && !(this.CurrentMenu is SubMenuLeaderboards) && !(this.CurrentMenu is SubMenuCharacterSelect) && currentlySelectedPosition == num3 && iDirection == ControllerDirection.Up);
				}
				if (this.CurrentMenu is SubMenuCharacterSelect)
				{
					num3 = 2;
					num2 = 4;
				}
				else if (this.CurrentMenu is SubMenuOnline)
				{
					num2 = 16;
				}
				else if (this.CurrentMenu is SubMenuLeaderboards)
				{
					num2 = 2;
				}
				bool flag3 = currentlySelectedPosition == num2 && (iDirection == ControllerDirection.Down || iDirection == ControllerDirection.Right);
				if (this.CurrentMenu is SubMenuLeaderboards)
				{
					flag3 = (currentlySelectedPosition == num2 && iDirection == ControllerDirection.Down);
				}
				flag = (this.MenuOverExtraButtons || flag3 || flag2);
				if ((this.CurrentMenu is SubMenuOptionsKeyboard || this.CurrentMenu is SubMenuOptionsGamepad) && iDirection == ControllerDirection.Right)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Game.Instance.IsMouseVisible = false;
				AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_scroll".GetHashCodeCustom());
				int num5 = currentlySelectedPosition;
				if (!this.MenuOverExtraButtons)
				{
					this.mMenuStack[this.mMenuStackPosition].UnselectAll();
				}
				if (this.CurrentMenu is SubMenuOptionsKeyboard || this.CurrentMenu is SubMenuOptionsGamepad)
				{
					this.NavigateExtraButtons(currentlySelectedPosition, currentlySelectedPosition, currentlySelectedPosition, currentlySelectedPosition, ref num5, iDirection);
				}
				else if (this.CurrentMenu is SubMenuOnline || this.CurrentMenu is SubMenuOptions || this.CurrentMenu is SubMenuLeaderboards)
				{
					this.NavigateExtraButtons(num2, num2, num2, num2, ref num5, iDirection);
				}
				else
				{
					this.NavigateExtraButtons(num3, num2, num2, num2, ref num5, iDirection);
				}
				if (num5 != currentlySelectedPosition)
				{
					this.mMenuStack[this.mMenuStackPosition].ForceSetAndSelectCurrent(num5);
					return;
				}
			}
			else
			{
				this.UnselectAllExtraButtons();
				switch (iDirection)
				{
				case ControllerDirection.Right:
					Game.Instance.IsMouseVisible = false;
					AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_increase".GetHashCodeCustom());
					this.mMenuStack[this.mMenuStackPosition].ControllerRight(iSender);
					return;
				case ControllerDirection.Up:
					Game.Instance.IsMouseVisible = false;
					AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_scroll".GetHashCodeCustom());
					this.mMenuStack[this.mMenuStackPosition].ControllerUp(iSender);
					return;
				case ControllerDirection.UpRight:
					break;
				case ControllerDirection.Left:
					Game.Instance.IsMouseVisible = false;
					AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_decrease".GetHashCodeCustom());
					this.mMenuStack[this.mMenuStackPosition].ControllerLeft(iSender);
					return;
				default:
					if (iDirection != ControllerDirection.Down)
					{
						return;
					}
					Game.Instance.IsMouseVisible = false;
					AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_scroll".GetHashCodeCustom());
					this.mMenuStack[this.mMenuStackPosition].ControllerDown(iSender);
					break;
				}
			}
		}

		// Token: 0x06001674 RID: 5748 RVA: 0x000906B7 File Offset: 0x0008E8B7
		public void ControllerEvent(Controller iSender, KeyboardState iOldState, KeyboardState iNewState)
		{
			if (this.mMenuStackPosition >= 0 && this.mMenuStackPosition < this.mMenuStack.Length)
			{
				this.mMenuStack[this.mMenuStackPosition].ControllerEvent(iSender, iOldState, iNewState);
			}
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x000906E8 File Offset: 0x0008E8E8
		public void ControllerA(Controller iSender)
		{
			if (this.mInputLocked | DialogManager.Instance.MessageBoxActive)
			{
				return;
			}
			if (this.mCurrentState == Tome.ClosedBack.Instance)
			{
				this.ChangeState(Tome.BackToFront.Instance);
				return;
			}
			if (this.mCurrentState == Tome.ClosedState.Instance)
			{
				if (this.mIntroTime > 1f && !this.ControllerA_OnExtraButton() && !this.mHurryAndOpen)
				{
					this.mHurryAndOpen = true;
					this.mCameraAnimation.ClipSpeed *= 6f;
					return;
				}
			}
			else
			{
				AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_select".GetHashCodeCustom());
				if (!this.ControllerA_OnExtraButton())
				{
					this.mMenuStack[this.mMenuStackPosition].ControllerA(iSender);
				}
			}
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x0009079C File Offset: 0x0008E99C
		public void ControllerB(Controller iSender)
		{
			if (this.mInputLocked | DialogManager.Instance.MessageBoxActive)
			{
				return;
			}
			if (this.mMenuStackPosition > 0)
			{
				this.mMenuStack[this.mMenuStackPosition].ControllerB(iSender);
				return;
			}
			SubMenuMain.Instance.ShowRUSure();
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x000907D9 File Offset: 0x0008E9D9
		public void ControllerX(Controller iSender)
		{
			if (!this.mInputLocked && !DialogManager.Instance.MessageBoxActive && this.mMenuStackPosition > 0)
			{
				this.mMenuStack[this.mMenuStackPosition].ControllerX(iSender);
			}
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x0009080B File Offset: 0x0008EA0B
		public void ControllerY(Controller iSender)
		{
			if (!this.mInputLocked && !DialogManager.Instance.MessageBoxActive && this.mMenuStackPosition > 0)
			{
				this.mMenuStack[this.mMenuStackPosition].ControllerY(iSender);
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x06001679 RID: 5753 RVA: 0x0009083D File Offset: 0x0008EA3D
		// (set) Token: 0x0600167A RID: 5754 RVA: 0x00090845 File Offset: 0x0008EA45
		public float TargetLightVariationAmount
		{
			get
			{
				return this.mTargetLightVariationAmount;
			}
			set
			{
				this.mTargetLightVariationAmount = value;
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x0600167B RID: 5755 RVA: 0x0009084E File Offset: 0x0008EA4E
		// (set) Token: 0x0600167C RID: 5756 RVA: 0x00090856 File Offset: 0x0008EA56
		public float TargetLightVariationSpeed
		{
			get
			{
				return this.mTargetLightVariationSpeed;
			}
			set
			{
				this.mTargetLightVariationSpeed = value;
			}
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x0600167D RID: 5757 RVA: 0x0009085F File Offset: 0x0008EA5F
		// (set) Token: 0x0600167E RID: 5758 RVA: 0x00090867 File Offset: 0x0008EA67
		public float TargetLightIntensity
		{
			get
			{
				return this.mTargetLightIntensity;
			}
			set
			{
				this.mTargetLightIntensity = value;
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x0600167F RID: 5759 RVA: 0x00090870 File Offset: 0x0008EA70
		// (set) Token: 0x06001680 RID: 5760 RVA: 0x00090878 File Offset: 0x0008EA78
		public float LightIntensity
		{
			get
			{
				return this.mLightIntensity;
			}
			set
			{
				this.mLightIntensity = value;
				this.mTargetLightIntensity = value;
			}
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00090898 File Offset: 0x0008EA98
		internal void ControllerMouseMove(KeyboardMouseController keyboardMouseController, Point screenSize, MouseState newMouseState, MouseState mOldMouseState)
		{
			if (Tome.sFaceBook.InsideBounds(newMouseState))
			{
				Tome.sFaceBook.Selected = true;
				Tome.sTwitter.Selected = false;
				if (Tome.sPromotionButton != null)
				{
					Tome.sPromotionButton.Selected = false;
					return;
				}
			}
			else if (Tome.sTwitter.InsideBounds(newMouseState))
			{
				Tome.sFaceBook.Selected = false;
				Tome.sTwitter.Selected = true;
				if (Tome.sPromotionButton != null)
				{
					Tome.sPromotionButton.Selected = false;
					return;
				}
			}
			else if (Tome.sPromotionButton != null && Tome.sPromotionButton.InsideBounds(newMouseState))
			{
				Tome.sFaceBook.Selected = false;
				Tome.sTwitter.Selected = false;
				if (Tome.sPromotionButton != null)
				{
					Tome.sPromotionButton.Selected = true;
					return;
				}
			}
			else
			{
				if (Tome.sAccountCreationBtn.InsideBounds(newMouseState) && Tome.sAccountCreationBtn.Enabled)
				{
					Tome.sAccountCreationBtn.Selected = true;
					Tome.sAccountLoginBtn.Selected = false;
					return;
				}
				if (Tome.sAccountLoginBtn.InsideBounds(newMouseState) && Tome.sAccountLoginBtn.Enabled)
				{
					Tome.sAccountLoginBtn.Selected = true;
					Tome.sAccountCreationBtn.Selected = false;
					return;
				}
				Tome.sFaceBook.Selected = false;
				Tome.sTwitter.Selected = false;
				if (Tome.sPromotionButton != null)
				{
					Tome.sPromotionButton.Selected = false;
				}
				Tome.sAccountCreationBtn.Selected = false;
				Tome.sAccountLoginBtn.Selected = false;
				if (this.CurrentMenu != null && !(this.CurrentState is Tome.ClosedState) && !(this.CurrentState is Tome.ClosedBack) && !(this.CurrentState is Tome.OpeningState))
				{
					this.CurrentMenu.ControllerMouseMove(keyboardMouseController, screenSize, newMouseState, mOldMouseState);
				}
			}
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x00090A34 File Offset: 0x0008EC34
		private void OpenURL(string url)
		{
			this.OpenURL(url, false);
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x00090AC0 File Offset: 0x0008ECC0
		private void OpenURL(string url, bool viaSteam)
		{
			Game.Instance.Form.BeginInvoke(new Action(delegate()
			{
				if (viaSteam)
				{
					SteamUtils.ActivateGameOverlayToWebPage(url);
					return;
				}
				foreach (object obj in Application.OpenForms)
				{
					Form form = (Form)obj;
					form.WindowState = FormWindowState.Minimized;
				}
				Process.Start(url);
			}));
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x00090B00 File Offset: 0x0008ED00
		internal void ControllerMouseAction(KeyboardMouseController keyboardMouseController, Point screenSize, MouseState newMouseState, MouseState mOldMouseState)
		{
			bool flag = newMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && mOldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
			bool flag2 = !(this.CurrentMenu is SubMenuCutscene) && !(this.CurrentMenu is SubMenuIntro);
			if (Tome.sFaceBook.InsideBounds(newMouseState) && flag2)
			{
				this.OpenURL("http://www.facebook.com/MagickaGame/", true);
				return;
			}
			if (Tome.sTwitter.InsideBounds(newMouseState) && flag2)
			{
				this.OpenURL("http://www.twitter.com/MagickaGame/", true);
				return;
			}
			if (Tome.sPromotionButton != null && Tome.sPromotionButton.InsideBounds(newMouseState) && flag2)
			{
				string currentPromotion_NoneStoreURL = DLC_StatusHelper.Instance.CurrentPromotion_NoneStoreURL;
				string str = DLC_StatusHelper.Instance.CurrentPromotion_AppID.ToString();
				TelemetryUtils.SendDLCPromotionClicked();
				Singleton<GameSparksServices>.Instance.LogEvent("AdClicked");
				if (!string.IsNullOrEmpty(currentPromotion_NoneStoreURL))
				{
					this.OpenURL(currentPromotion_NoneStoreURL);
					return;
				}
				string text = "http://store.steampowered.com/";
				if (DLC_StatusHelper.Instance.CurrentPromotion_IsDynamicallyLoaded)
				{
					text = text + "app/" + str;
				}
				else if (DLC_StatusHelper.Instance.HasPromotion)
				{
					text = text + "app/" + str;
				}
				else
				{
					text += "dlc/42910";
				}
				this.OpenURL(text, true);
				return;
			}
			else if (Tome.sAccountCreationBtn.InsideBounds(newMouseState) && Tome.sAccountCreationBtn.Enabled && !(Tome.Instance.CurrentMenu is SubMenuAccountCreate) && flag2)
			{
				if (this.PreviousMenu is SubMenuAccountCreate)
				{
					this.PopMenu();
					return;
				}
				this.PushMenu(SubMenuAccountCreate.Instance, 2);
				return;
			}
			else if (Tome.sAccountLoginBtn.InsideBounds(newMouseState) && Tome.sAccountLoginBtn.Enabled && flag && !(Tome.Instance.CurrentMenu is SubMenuAccountLogin) && flag2)
			{
				if (this.PreviousMenu is SubMenuAccountLogin)
				{
					this.PopMenu();
					return;
				}
				this.PushMenu(SubMenuAccountLogin.Instance, 1);
				return;
			}
			else
			{
				if (this.CurrentMenu == null)
				{
					this.ControllerA(keyboardMouseController);
					return;
				}
				if (!(this.CurrentState is Tome.ClosedState) && !(this.CurrentState is Tome.ClosedBack) && !(this.CurrentState is Tome.OpeningState))
				{
					this.CurrentMenu.ControllerMouseAction(keyboardMouseController, screenSize, newMouseState, mOldMouseState);
				}
				return;
			}
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x00090D24 File Offset: 0x0008EF24
		internal void PageToScreen(bool iRight, ref Vector2 iPagePos, out Vector2 oScreenPos)
		{
			Triangle triangle;
			if (iRight)
			{
				triangle = Tome.RIGHT_TRIS[0];
			}
			else
			{
				triangle = Tome.LEFT_TRIS[0];
			}
			Vector3 vector;
			triangle.GetPoint(1f - iPagePos.X / (float)Tome.PAGERIGHTSHEET.Width, iPagePos.Y / (float)Tome.PAGERIGHTSHEET.Height, out vector);
			Point screenSize = RenderManager.Instance.ScreenSize;
			Vector4 vector2;
			Vector4.Transform(ref vector, ref Tome.mViewProjection, out vector2);
			oScreenPos = default(Vector2);
			oScreenPos.X = vector2.X / vector2.W;
			oScreenPos.Y = vector2.Y / vector2.W;
			oScreenPos.X *= 0.5f;
			oScreenPos.Y *= -0.5f;
			oScreenPos.X += 0.5f;
			oScreenPos.Y += 0.5f;
			oScreenPos.X *= (float)screenSize.X;
			oScreenPos.Y *= (float)screenSize.Y;
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x00090E4F File Offset: 0x0008F04F
		public static void OnLoggedOut()
		{
			Tome.sAccountLoginBtn.Enabled = true;
			Tome.sAccountCreationBtn.Enabled = true;
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x00090E80 File Offset: 0x0008F080
		public static void OnLoggedIn(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode)
		{
			if (iSuccess)
			{
				bool flag = Tome.Instance.CurrentMenu is SubMenuAccountCreate;
				if (!(Tome.Instance.CurrentMenu is SubMenuAccountCreate))
				{
					if (!(Tome.Instance.CurrentMenu is SubMenuAccountLogin))
					{
						goto IL_73;
					}
				}
				while (Tome.Instance.PreviousMenu is SubMenuAccountCreate || Tome.Instance.PreviousMenu is SubMenuAccountLogin)
				{
					Tome.Instance.PopPreviousMenu();
				}
				Tome.Instance.PopMenu();
				IL_73:
				if (flag)
				{
					Tome.Instance.PushMenu(SubMenuOptionsParadoxAccount.Instance, 1);
					Tome.sSteamLinkPopup.SetTitle(Tome.LOC_NOTICE, Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR);
					Tome.sSteamLinkPopup.SetMessage(Tome.LOC_STEAMLINK, Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR);
					Tome.sSteamLinkPopup.SetButtonType(ButtonConfig.OkCancel);
					MenuMessagePopup menuMessagePopup = Tome.sSteamLinkPopup;
					menuMessagePopup.OnPositiveClick = (Action)Delegate.Combine(menuMessagePopup.OnPositiveClick, new Action(delegate()
					{
						Singleton<ParadoxAccount>.Instance.LinkSteam(new ParadoxAccountSequence.ExecutionDoneDelegate(Tome.OnSteamLinkCallback));
					}));
					Singleton<PopupSystem>.Instance.AddPopupToQueue(Tome.sSteamLinkPopup);
					return;
				}
			}
			else
			{
				ParadoxPopupUtils.ShowErrorPopup(iErrorCode);
			}
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x00090F96 File Offset: 0x0008F196
		private static void OnSteamLinkCallback(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode)
		{
			if (!iSuccess)
			{
				ParadoxPopupUtils.ShowErrorPopup(iErrorCode);
				Tome.Instance.PushMenu(SubMenuOptionsParadoxAccount.Instance, 1);
			}
		}

		// Token: 0x04001778 RID: 6008
		private const string AD_CLICKED_EVENT_KEY = "AdClicked";

		// Token: 0x04001779 RID: 6009
		private const string AD_PROPERTY_NAME = "Ads";

		// Token: 0x0400177A RID: 6010
		private const string ADS_PROPERTY_SET_NAME = "ABTestAdLevel";

		// Token: 0x0400177B RID: 6011
		private const float PICK_AREA_HEIGHT = 5.5f;

		// Token: 0x0400177C RID: 6012
		private const float PICK_AREA_WIDTH = 5.3f;

		// Token: 0x0400177D RID: 6013
		private const float ACCOUNT_PADDING = 10f;

		// Token: 0x0400177E RID: 6014
		private const float ACCOUNT_PADDING_INNER = 20f;

		// Token: 0x0400177F RID: 6015
		private const float ACCOUNT_BTN_WIDTH = 230f;

		// Token: 0x04001780 RID: 6016
		private const float ACCOUNT_BTN_HEIGHT = 100f;

		// Token: 0x04001781 RID: 6017
		private const float ACCOUNT_BTN_SPACING = 25f;

		// Token: 0x04001782 RID: 6018
		private const float FADE_IN_TIME = 0.25f;

		// Token: 0x04001783 RID: 6019
		private const float FACEBOOK_UV_X = 1070f;

		// Token: 0x04001784 RID: 6020
		private const float FACEBOOK_UV_Y = 830f;

		// Token: 0x04001785 RID: 6021
		private const float FACEBOOK_UV_WIDTH = 104f;

		// Token: 0x04001786 RID: 6022
		private const float FACEBOOK_UV_HEIGHT = 66f;

		// Token: 0x04001787 RID: 6023
		private const float TWITTER_UV_X = 1174f;

		// Token: 0x04001788 RID: 6024
		private const float TWITTER_UV_Y = 830f;

		// Token: 0x04001789 RID: 6025
		private const float TWITTER_UV_WIDTH = 104f;

		// Token: 0x0400178A RID: 6026
		private const float TWITTER_UV_HEIGHT = 66f;

		// Token: 0x0400178B RID: 6027
		private const float SPLASH_RIGHT_PADDING = 11f;

		// Token: 0x0400178C RID: 6028
		private const float FB_TWITTER_PADDING = 5f;

		// Token: 0x0400178D RID: 6029
		private const float SPLASH_BOTTOM_PADDING = 11f;

		// Token: 0x0400178E RID: 6030
		private const float SPLASH_NEW_PADDING = 3f;

		// Token: 0x0400178F RID: 6031
		private const float SPLASH_NEW_UV_X = 1081f;

		// Token: 0x04001790 RID: 6032
		private const float SPLASH_NEW_UV_Y = 896f;

		// Token: 0x04001791 RID: 6033
		private const float SPLASH_NEW_UV_WIDTH = 82f;

		// Token: 0x04001792 RID: 6034
		private const float SPLASH_NEW_UV_HEIGHT = 38f;

		// Token: 0x04001793 RID: 6035
		private static Vector3 sLightPosition;

		// Token: 0x04001794 RID: 6036
		private static float mCurrentTime;

		// Token: 0x04001795 RID: 6037
		private static readonly Vector2 PROMOTION_BUTTON_POSITION = new Vector2(216f, 316f);

		// Token: 0x04001796 RID: 6038
		private static Tome mSingelton;

		// Token: 0x04001797 RID: 6039
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400179B RID: 6043
		public static readonly Viewport TOMERIGHTSHEET;

		// Token: 0x0400179C RID: 6044
		public static readonly Viewport TOMELEFTSHEET;

		// Token: 0x0400179D RID: 6045
		public static readonly Viewport PAGELEFTSHEET;

		// Token: 0x0400179E RID: 6046
		public static readonly Viewport PAGERIGHTSHEET;

		// Token: 0x0400179F RID: 6047
		public static readonly int MaxNrOfPages = 10;

		// Token: 0x040017A0 RID: 6048
		private AnimationClip[] mTomeClips;

		// Token: 0x040017A1 RID: 6049
		private AnimationClip[] mBackGroundClips;

		// Token: 0x040017A2 RID: 6050
		private AnimationController mTomeController;

		// Token: 0x040017A3 RID: 6051
		private AnimationController mBackGroundPageControllerLeft;

		// Token: 0x040017A4 RID: 6052
		private AnimationController mBackGroundPageControllerRight;

		// Token: 0x040017A5 RID: 6053
		private Tome.RiffleController mRiffleController;

		// Token: 0x040017A6 RID: 6054
		private Tome.TomeState mCurrentState;

		// Token: 0x040017A7 RID: 6055
		private static Vector3 mCameraPosition;

		// Token: 0x040017A8 RID: 6056
		private Matrix mTomeBindPose;

		// Token: 0x040017A9 RID: 6057
		private int mTomeBone;

		// Token: 0x040017AA RID: 6058
		private Matrix mCameraBindPose;

		// Token: 0x040017AB RID: 6059
		private int mCameraBone;

		// Token: 0x040017AC RID: 6060
		private AnimationClip[] mCameraAnimations;

		// Token: 0x040017AD RID: 6061
		private AnimationController mCameraAnimation;

		// Token: 0x040017AE RID: 6062
		private SubMenu[] mMenuStack;

		// Token: 0x040017AF RID: 6063
		private int mMenuStackPosition = -1;

		// Token: 0x040017B0 RID: 6064
		private Stack<int> mMenuRiffleStack;

		// Token: 0x040017B1 RID: 6065
		private bool mInputLocked;

		// Token: 0x040017B2 RID: 6066
		public bool CloseToMenu;

		// Token: 0x040017B3 RID: 6067
		private Tome.RenderData[] mRenderData;

		// Token: 0x040017B4 RID: 6068
		private static readonly Triangle[] LEFT_TRIS;

		// Token: 0x040017B5 RID: 6069
		private static readonly Triangle[] RIGHT_TRIS;

		// Token: 0x040017B6 RID: 6070
		private static readonly Vector3 PICK_LEFT_AREA_TOPRIGHT = new Vector3(0f, 0.5f, 2.75f);

		// Token: 0x040017B7 RID: 6071
		private static readonly Vector3 PICK_LEFT_AREA_BOTTOMLEFT = new Vector3(5.3f, 0.5f, -2.75f);

		// Token: 0x040017B8 RID: 6072
		private static readonly Vector3 PICK_RIGHT_AREA_TOPRIGHT = new Vector3(-5.3f, 0.5f, 2.75f);

		// Token: 0x040017B9 RID: 6073
		private static readonly Vector3 PICK_RIGHT_AREA_BOTTOMLEFT = new Vector3(0f, 0.5f, -2.75f);

		// Token: 0x040017BA RID: 6074
		private static Random sRandom = new Random();

		// Token: 0x040017BB RID: 6075
		private float mLightVariationSpeed = 24f;

		// Token: 0x040017BC RID: 6076
		private float mLightVariationAmount = 1f;

		// Token: 0x040017BD RID: 6077
		private float mTargetLightVariationSpeed = 6f;

		// Token: 0x040017BE RID: 6078
		private float mTargetLightVariationAmount = 0.2f;

		// Token: 0x040017BF RID: 6079
		private float mLightVariationPosition;

		// Token: 0x040017C0 RID: 6080
		private float mLightIntensity;

		// Token: 0x040017C1 RID: 6081
		private float mTargetLightIntensity = 1f;

		// Token: 0x040017C2 RID: 6082
		private float mIntroTime;

		// Token: 0x040017C3 RID: 6083
		private bool mHurryAndOpen;

		// Token: 0x040017C4 RID: 6084
		private static readonly int LOC_LOGGEDIN = "#acc_loggedin".GetHashCodeCustom();

		// Token: 0x040017C5 RID: 6085
		private static readonly int LOC_LOGGEDOUT = "#acc_loggedout".GetHashCodeCustom();

		// Token: 0x040017C6 RID: 6086
		private static readonly int LOC_NOTICE = "#popup_notice".GetHashCodeCustom();

		// Token: 0x040017C7 RID: 6087
		private static readonly int LOC_STEAMLINK = "#steam_link".GetHashCodeCustom();

		// Token: 0x040017C8 RID: 6088
		private static readonly int LOC_PROCESSING = "#menu_main_processing".GetHashCodeCustom();

		// Token: 0x040017C9 RID: 6089
		private static MenuMessagePopup sProcessingPopup = null;

		// Token: 0x040017CA RID: 6090
		private static readonly Vector2 PARADOX_LOGO_UV = new Vector2(18f, 763f);

		// Token: 0x040017CB RID: 6091
		private static readonly Vector2 PARADOX_LOGO_UV_SIZE = new Vector2(717f, 249f);

		// Token: 0x040017CC RID: 6092
		private static readonly Vector2 PARADOX_LOGO_SIZE = new Vector2(150f, 52f) * 1.65f;

		// Token: 0x040017CD RID: 6093
		private static readonly Vector2 ACCOUNT_BACKGROUND_UV = new Vector2(0f, 464f);

		// Token: 0x040017CE RID: 6094
		private static readonly Vector2 ACCOUNT_BACKGROUND_UV_SIZE = new Vector2(448f, 560f);

		// Token: 0x040017CF RID: 6095
		private static readonly Vector2 ACCOUNT_BACKGROUND_SIZE = new Vector2(300f, 400f);

		// Token: 0x040017D0 RID: 6096
		private static MenuImageTextItem sParadoxIcon;

		// Token: 0x040017D1 RID: 6097
		private static MenuImageTextItem sIndicatorBackground;

		// Token: 0x040017D2 RID: 6098
		private static MenuTextButtonItem sAccountLoginBtn;

		// Token: 0x040017D3 RID: 6099
		private static MenuTextButtonItem sAccountCreationBtn;

		// Token: 0x040017D4 RID: 6100
		private static bool sFadeInAccountIndicator = false;

		// Token: 0x040017D5 RID: 6101
		private static MenuMessagePopup sSteamLinkPopup = null;

		// Token: 0x040017D6 RID: 6102
		private static readonly Vector2 FACEBOOK_SIZE = new Vector2(104f, 66f);

		// Token: 0x040017D7 RID: 6103
		private static readonly Vector2 TWITTER_SIZE = new Vector2(104f, 66f);

		// Token: 0x040017D8 RID: 6104
		private static readonly Vector2 SPLASH_NEW_SIZE = new Vector2(82f, 38f);

		// Token: 0x040017D9 RID: 6105
		private static MenuImageTextItem sFaceBook;

		// Token: 0x040017DA RID: 6106
		private static MenuImageTextItem sTwitter;

		// Token: 0x040017DB RID: 6107
		private static MenuImageTextItem sPromotionButton;

		// Token: 0x040017DC RID: 6108
		private static bool sPromotionActive = false;

		// Token: 0x040017DD RID: 6109
		private static MenuImageTextItem sSplashNew;

		// Token: 0x040017DE RID: 6110
		private static bool sShownSplashIsNew = false;

		// Token: 0x040017DF RID: 6111
		private static Text sVersionText;

		// Token: 0x040017E0 RID: 6112
		private int mAchievementsHash = "#achievement_text02".GetHashCodeCustom();

		// Token: 0x040017E1 RID: 6113
		private float mNewContentUpdateTimer;

		// Token: 0x040017E2 RID: 6114
		private static Matrix mViewProjection;

		// Token: 0x040017E3 RID: 6115
		private int currentExtraButtonIndex = -1;

		// Token: 0x020002D6 RID: 726
		public abstract class TomeState
		{
			// Token: 0x0600168D RID: 5773 RVA: 0x00090FB1 File Offset: 0x0008F1B1
			public virtual void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
			{
			}

			// Token: 0x0600168E RID: 5774 RVA: 0x00090FB4 File Offset: 0x0008F1B4
			public virtual void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
			{
				if (iOwner.mMenuStackPosition >= 0)
				{
					iMenu.DrawNewAndOld((iOwner.mMenuStackPosition > 0) ? iOwner.mMenuStack[iOwner.mMenuStackPosition - 1] : null, this.mCurrentLeft, this.mCurrentRight, this.mPreviousLeft, this.mPreviousRight);
				}
			}

			// Token: 0x0600168F RID: 5775 RVA: 0x00091003 File Offset: 0x0008F203
			public virtual void OnEnter(Tome iOwner)
			{
			}

			// Token: 0x06001690 RID: 5776 RVA: 0x00091005 File Offset: 0x0008F205
			public virtual void OnExit(Tome iOwner)
			{
			}

			// Token: 0x040017E8 RID: 6120
			protected Matrix identity = Matrix.Identity;

			// Token: 0x040017E9 RID: 6121
			protected Viewport mPreviousLeft = Tome.PAGELEFTSHEET;

			// Token: 0x040017EA RID: 6122
			protected Viewport mPreviousRight = Tome.PAGERIGHTSHEET;

			// Token: 0x040017EB RID: 6123
			protected Viewport mCurrentLeft = Tome.TOMELEFTSHEET;

			// Token: 0x040017EC RID: 6124
			protected Viewport mCurrentRight = Tome.TOMERIGHTSHEET;
		}

		// Token: 0x020002D7 RID: 727
		public class ClosedState : Tome.TomeState
		{
			// Token: 0x170005BC RID: 1468
			// (get) Token: 0x06001692 RID: 5778 RVA: 0x00091048 File Offset: 0x0008F248
			public static Tome.ClosedState Instance
			{
				get
				{
					if (Tome.ClosedState.mSingelton == null)
					{
						lock (Tome.ClosedState.mSingeltonLock)
						{
							if (Tome.ClosedState.mSingelton == null)
							{
								Tome.ClosedState.mSingelton = new Tome.ClosedState();
							}
						}
					}
					return Tome.ClosedState.mSingelton;
				}
			}

			// Token: 0x06001693 RID: 5779 RVA: 0x0009109C File Offset: 0x0008F29C
			private ClosedState()
			{
			}

			// Token: 0x06001694 RID: 5780 RVA: 0x000910A4 File Offset: 0x0008F2A4
			public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
			{
			}

			// Token: 0x06001695 RID: 5781 RVA: 0x000910A6 File Offset: 0x0008F2A6
			public override void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
			{
			}

			// Token: 0x06001696 RID: 5782 RVA: 0x000910A8 File Offset: 0x0008F2A8
			public override void OnEnter(Tome iOwner)
			{
				iOwner.mTomeController.StartClip(iOwner.mTomeClips[1], false);
				iOwner.mTomeController.Update(iOwner.mTomeClips[1].Duration - 0.001f, ref this.identity, true);
				iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[2], false);
				iOwner.mBackGroundPageControllerLeft.Update(iOwner.mBackGroundClips[2].Duration - 0.001f, ref this.identity, true);
				iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[3], false);
				iOwner.mBackGroundPageControllerRight.Update(iOwner.mBackGroundClips[3].Duration - 0.001f, ref this.identity, true);
				iOwner.mTomeController.Update(0.01f, ref this.identity, true);
				iOwner.mBackGroundPageControllerLeft.Update(0.01f, ref this.identity, true);
				iOwner.mBackGroundPageControllerRight.Update(0.01f, ref this.identity, true);
				for (int i = 0; i < iOwner.mRenderData.Length; i++)
				{
					Array.Copy(iOwner.mTomeController.SkinnedBoneTransforms, iOwner.mRenderData[i].mTomeBones, iOwner.mRenderData[i].mTomeBones.Length);
					Array.Copy(iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, iOwner.mRenderData[i].mBackgroundPageBonesLeft, iOwner.mRenderData[i].mBackgroundPageBonesLeft.Length);
					Array.Copy(iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, iOwner.mRenderData[i].mBackgroundPageBonesRight, iOwner.mRenderData[i].mBackgroundPageBonesRight.Length);
				}
				GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
				GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
				GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
				GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
				if (GameStateManager.Instance.CurrentState is PlayState && Tome.Instance.CloseToMenu)
				{
					GameStateManager.Instance.PopState();
				}
				if (iOwner.OnClose != null)
				{
					iOwner.OnClose.Invoke();
				}
			}

			// Token: 0x06001697 RID: 5783 RVA: 0x000912A8 File Offset: 0x0008F4A8
			public override void OnExit(Tome iOwner)
			{
				GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
				GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
			}

			// Token: 0x040017ED RID: 6125
			private static Tome.ClosedState mSingelton;

			// Token: 0x040017EE RID: 6126
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002D8 RID: 728
		public class OpenState : Tome.TomeState
		{
			// Token: 0x170005BD RID: 1469
			// (get) Token: 0x06001699 RID: 5785 RVA: 0x000912D0 File Offset: 0x0008F4D0
			public static Tome.OpenState Instance
			{
				get
				{
					if (Tome.OpenState.mSingelton == null)
					{
						lock (Tome.OpenState.mSingeltonLock)
						{
							if (Tome.OpenState.mSingelton == null)
							{
								Tome.OpenState.mSingelton = new Tome.OpenState();
							}
						}
					}
					return Tome.OpenState.mSingelton;
				}
			}

			// Token: 0x0600169A RID: 5786 RVA: 0x00091324 File Offset: 0x0008F524
			private OpenState()
			{
			}

			// Token: 0x0600169B RID: 5787 RVA: 0x0009132C File Offset: 0x0008F52C
			public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
			{
				Tome.RenderData renderData = iOwner.mRenderData[(int)iDataChannel];
				Array.Copy(iOwner.mTomeController.SkinnedBoneTransforms, renderData.mTomeBones, renderData.mTomeBones.Length);
				Array.Copy(iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
				Array.Copy(iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
			}

			// Token: 0x0600169C RID: 5788 RVA: 0x0009139C File Offset: 0x0008F59C
			public override void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
			{
				iMenu.Draw(this.mCurrentLeft, this.mCurrentRight);
			}

			// Token: 0x0600169D RID: 5789 RVA: 0x000913B0 File Offset: 0x0008F5B0
			public override void OnEnter(Tome iOwner)
			{
				iOwner.mTomeController.PlaybackMode = PlaybackMode.Forward;
				iOwner.mTomeController.StartClip(iOwner.mTomeClips[0], false);
				iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[0], false);
				iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[1], false);
				iOwner.mTomeController.Update(iOwner.mTomeClips[0].Duration + 1f, ref this.identity, true);
				iOwner.mBackGroundPageControllerLeft.Update(iOwner.mBackGroundClips[0].Duration + 1f, ref this.identity, true);
				iOwner.mBackGroundPageControllerRight.Update(iOwner.mBackGroundClips[1].Duration + 1f, ref this.identity, true);
				ulong startupLobby = GlobalSettings.Instance.StartupLobby;
				if (startupLobby != 0UL)
				{
					NetworkManager.Instance.DirectConnect(startupLobby);
					GlobalSettings.Instance.StartupLobby = 0UL;
				}
			}

			// Token: 0x0600169E RID: 5790 RVA: 0x0009149B File Offset: 0x0008F69B
			public override void OnExit(Tome iOwner)
			{
			}

			// Token: 0x040017EF RID: 6127
			private static Tome.OpenState mSingelton;

			// Token: 0x040017F0 RID: 6128
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002D9 RID: 729
		public class OpeningState : Tome.TomeState
		{
			// Token: 0x170005BE RID: 1470
			// (get) Token: 0x060016A0 RID: 5792 RVA: 0x000914AC File Offset: 0x0008F6AC
			public static Tome.OpeningState Instance
			{
				get
				{
					if (Tome.OpeningState.mSingelton == null)
					{
						lock (Tome.OpeningState.mSingeltonLock)
						{
							if (Tome.OpeningState.mSingelton == null)
							{
								Tome.OpeningState.mSingelton = new Tome.OpeningState();
							}
						}
					}
					return Tome.OpeningState.mSingelton;
				}
			}

			// Token: 0x060016A1 RID: 5793 RVA: 0x00091500 File Offset: 0x0008F700
			private OpeningState()
			{
			}

			// Token: 0x060016A2 RID: 5794 RVA: 0x00091508 File Offset: 0x0008F708
			public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
			{
				if (iOwner.mTomeController.IsPlaying)
				{
					Tome.RenderData renderData = iOwner.mRenderData[(int)iDataChannel];
					iOwner.mTomeController.Update(iDeltaTime, ref this.identity, true);
					iOwner.mBackGroundPageControllerLeft.Update(iDeltaTime, ref this.identity, true);
					iOwner.mBackGroundPageControllerRight.Update(iDeltaTime, ref this.identity, true);
					Array.Copy(iOwner.mTomeController.SkinnedBoneTransforms, renderData.mTomeBones, renderData.mTomeBones.Length);
					Array.Copy(iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
					Array.Copy(iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
					return;
				}
				iOwner.ChangeState(Tome.OpenState.Instance);
			}

			// Token: 0x060016A3 RID: 5795 RVA: 0x000915D0 File Offset: 0x0008F7D0
			public override void OnEnter(Tome iOwner)
			{
				iOwner.mTomeController.PlaybackMode = PlaybackMode.Forward;
				iOwner.mTomeController.StartClip(iOwner.mTomeClips[0], false);
				iOwner.mTomeController.Update(0.001f, ref this.identity, true);
				iOwner.mBackGroundPageControllerLeft.PlaybackMode = PlaybackMode.Forward;
				iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[0], false);
				iOwner.mBackGroundPageControllerLeft.Update(0.001f, ref this.identity, true);
				iOwner.mBackGroundPageControllerRight.PlaybackMode = PlaybackMode.Forward;
				iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[1], false);
				iOwner.mBackGroundPageControllerRight.Update(0.001f, ref this.identity, true);
				AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_bookopen".GetHashCodeCustom());
				if (iOwner.mMenuStackPosition < 0)
				{
					iOwner.mMenuStack[iOwner.mMenuStackPosition + 1] = SubMenuMain.Instance;
					iOwner.mMenuStackPosition++;
					iOwner.mMenuStack[iOwner.mMenuStackPosition].OnEnter();
				}
				else
				{
					iOwner.mMenuStack[iOwner.mMenuStackPosition].OnEnter();
				}
				iOwner.mCameraAnimation.CrossFade(iOwner.mCameraAnimations[1], 0.75f, false);
				if (iOwner.OnOpen != null)
				{
					iOwner.OnOpen.Invoke();
				}
			}

			// Token: 0x060016A4 RID: 5796 RVA: 0x00091715 File Offset: 0x0008F915
			public override void OnExit(Tome iOwner)
			{
			}

			// Token: 0x040017F1 RID: 6129
			private static Tome.OpeningState mSingelton;

			// Token: 0x040017F2 RID: 6130
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002DA RID: 730
		public class ClosingState : Tome.TomeState
		{
			// Token: 0x170005BF RID: 1471
			// (get) Token: 0x060016A6 RID: 5798 RVA: 0x00091728 File Offset: 0x0008F928
			public static Tome.ClosingState Instance
			{
				get
				{
					if (Tome.ClosingState.mSingelton == null)
					{
						lock (Tome.ClosingState.mSingeltonLock)
						{
							if (Tome.ClosingState.mSingelton == null)
							{
								Tome.ClosingState.mSingelton = new Tome.ClosingState();
							}
						}
					}
					return Tome.ClosingState.mSingelton;
				}
			}

			// Token: 0x060016A7 RID: 5799 RVA: 0x0009177C File Offset: 0x0008F97C
			private ClosingState()
			{
			}

			// Token: 0x060016A8 RID: 5800 RVA: 0x00091784 File Offset: 0x0008F984
			public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
			{
				if (iOwner.mTomeController.IsPlaying)
				{
					Tome.RenderData renderData = iOwner.mRenderData[(int)iDataChannel];
					iOwner.mTomeController.Update(iDeltaTime, ref this.identity, true);
					Array.Copy(iOwner.mTomeController.SkinnedBoneTransforms, renderData.mTomeBones, renderData.mTomeBones.Length);
					iOwner.mBackGroundPageControllerLeft.Update(iDeltaTime, ref this.identity, true);
					Array.Copy(iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
					iOwner.mBackGroundPageControllerRight.Update(iDeltaTime, ref this.identity, true);
					Array.Copy(iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
					return;
				}
				iOwner.ChangeState(Tome.ClosedState.Instance);
			}

			// Token: 0x060016A9 RID: 5801 RVA: 0x0009184C File Offset: 0x0008FA4C
			public override void OnEnter(Tome iOwner)
			{
				iOwner.mTomeController.StartClip(iOwner.mTomeClips[1], false);
				iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[2], false);
				iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[3], false);
				AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_bookclose".GetHashCodeCustom());
			}

			// Token: 0x060016AA RID: 5802 RVA: 0x000918AB File Offset: 0x0008FAAB
			public override void OnExit(Tome iOwner)
			{
			}

			// Token: 0x040017F3 RID: 6131
			private static Tome.ClosingState mSingelton;

			// Token: 0x040017F4 RID: 6132
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002DB RID: 731
		public class RiffleForwardState : Tome.TomeState
		{
			// Token: 0x170005C0 RID: 1472
			// (get) Token: 0x060016AC RID: 5804 RVA: 0x000918BC File Offset: 0x0008FABC
			public static Tome.RiffleForwardState Instance
			{
				get
				{
					if (Tome.RiffleForwardState.mSingelton == null)
					{
						lock (Tome.RiffleForwardState.mSingeltonLock)
						{
							if (Tome.RiffleForwardState.mSingelton == null)
							{
								Tome.RiffleForwardState.mSingelton = new Tome.RiffleForwardState();
							}
						}
					}
					return Tome.RiffleForwardState.mSingelton;
				}
			}

			// Token: 0x060016AD RID: 5805 RVA: 0x00091910 File Offset: 0x0008FB10
			private RiffleForwardState()
			{
				this.mPreviousLeft = Tome.TOMELEFTSHEET;
				this.mPreviousRight = Tome.PAGERIGHTSHEET;
				this.mCurrentLeft = Tome.PAGELEFTSHEET;
				this.mCurrentRight = Tome.TOMERIGHTSHEET;
			}

			// Token: 0x060016AE RID: 5806 RVA: 0x00091944 File Offset: 0x0008FB44
			public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
			{
				if (!iOwner.mRiffleController.HasFinished)
				{
					iOwner.mRiffleController.Update(iDeltaTime);
					return;
				}
				iOwner.ChangeState(Tome.OpenState.Instance);
			}

			// Token: 0x060016AF RID: 5807 RVA: 0x0009196C File Offset: 0x0008FB6C
			public override void OnEnter(Tome iOwner)
			{
				iOwner.mRiffleController.StartClip(Tome.FlipPageAnimations.FlipLeft, false, iOwner.mMenuRiffleStack.Peek());
				Tome.PageSoundVariables iVariables = default(Tome.PageSoundVariables);
				iVariables.Pages = (float)iOwner.mMenuRiffleStack.Peek();
				AudioManager.Instance.PlayCue<Tome.PageSoundVariables>(Banks.UI, "ui_menu_tomepage".GetHashCodeCustom(), iVariables);
			}

			// Token: 0x060016B0 RID: 5808 RVA: 0x000919C3 File Offset: 0x0008FBC3
			public override void OnExit(Tome iOwner)
			{
			}

			// Token: 0x040017F5 RID: 6133
			private static Tome.RiffleForwardState mSingelton;

			// Token: 0x040017F6 RID: 6134
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002DC RID: 732
		public class RiffleBackwardState : Tome.TomeState
		{
			// Token: 0x170005C1 RID: 1473
			// (get) Token: 0x060016B2 RID: 5810 RVA: 0x000919D4 File Offset: 0x0008FBD4
			public static Tome.RiffleBackwardState Instance
			{
				get
				{
					if (Tome.RiffleBackwardState.mSingelton == null)
					{
						lock (Tome.RiffleBackwardState.mSingeltonLock)
						{
							if (Tome.RiffleBackwardState.mSingelton == null)
							{
								Tome.RiffleBackwardState.mSingelton = new Tome.RiffleBackwardState();
							}
						}
					}
					return Tome.RiffleBackwardState.mSingelton;
				}
			}

			// Token: 0x060016B3 RID: 5811 RVA: 0x00091A28 File Offset: 0x0008FC28
			private RiffleBackwardState()
			{
				this.mPreviousLeft = Tome.PAGELEFTSHEET;
				this.mPreviousRight = Tome.TOMERIGHTSHEET;
				this.mCurrentLeft = Tome.TOMELEFTSHEET;
				this.mCurrentRight = Tome.PAGERIGHTSHEET;
			}

			// Token: 0x060016B4 RID: 5812 RVA: 0x00091A5C File Offset: 0x0008FC5C
			public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
			{
				if (!iOwner.mRiffleController.HasFinished)
				{
					iOwner.mRiffleController.Update(iDeltaTime);
					return;
				}
				iOwner.ChangeState(Tome.OpenState.Instance);
			}

			// Token: 0x060016B5 RID: 5813 RVA: 0x00091A83 File Offset: 0x0008FC83
			public override void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
			{
				if (iOwner.mMenuStackPosition >= 0)
				{
					iMenu.DrawNewAndOld(iOwner.mMenuStack[iOwner.mMenuStackPosition + 1], this.mCurrentLeft, this.mCurrentRight, this.mPreviousLeft, this.mPreviousRight);
				}
			}

			// Token: 0x060016B6 RID: 5814 RVA: 0x00091ABC File Offset: 0x0008FCBC
			public override void OnEnter(Tome iOwner)
			{
				int num = iOwner.mMenuRiffleStack.Peek();
				iOwner.mRiffleController.StartClip(Tome.FlipPageAnimations.FlipRight, false, num);
				Tome.PageSoundVariables iVariables = default(Tome.PageSoundVariables);
				iVariables.Pages = (float)num;
				AudioManager.Instance.PlayCue<Tome.PageSoundVariables>(Banks.UI, "ui_menu_tomepage".GetHashCodeCustom(), iVariables);
			}

			// Token: 0x060016B7 RID: 5815 RVA: 0x00091B0D File Offset: 0x0008FD0D
			public override void OnExit(Tome iOwner)
			{
			}

			// Token: 0x040017F7 RID: 6135
			private static Tome.RiffleBackwardState mSingelton;

			// Token: 0x040017F8 RID: 6136
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002DD RID: 733
		public class CloseToBack : Tome.TomeState
		{
			// Token: 0x170005C2 RID: 1474
			// (get) Token: 0x060016B9 RID: 5817 RVA: 0x00091B20 File Offset: 0x0008FD20
			public static Tome.CloseToBack Instance
			{
				get
				{
					if (Tome.CloseToBack.mSingelton == null)
					{
						lock (Tome.CloseToBack.mSingeltonLock)
						{
							if (Tome.CloseToBack.mSingelton == null)
							{
								Tome.CloseToBack.mSingelton = new Tome.CloseToBack();
							}
						}
					}
					return Tome.CloseToBack.mSingelton;
				}
			}

			// Token: 0x060016BA RID: 5818 RVA: 0x00091B74 File Offset: 0x0008FD74
			private CloseToBack()
			{
			}

			// Token: 0x060016BB RID: 5819 RVA: 0x00091B7C File Offset: 0x0008FD7C
			public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
			{
				Tome.RenderData renderData = iOwner.mRenderData[(int)iDataChannel];
				iOwner.mTomeController.Update(iDeltaTime, ref this.identity, true);
				Array.Copy(iOwner.mTomeController.SkinnedBoneTransforms, renderData.mTomeBones, renderData.mTomeBones.Length);
				iOwner.mBackGroundPageControllerLeft.Update(iDeltaTime, ref this.identity, true);
				Array.Copy(iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
				iOwner.mBackGroundPageControllerRight.Update(iDeltaTime, ref this.identity, true);
				Array.Copy(iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
				if (iOwner.mTomeController.HasFinished && !iOwner.mTomeController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Tome.ClosedBack.Instance);
				}
			}

			// Token: 0x060016BC RID: 5820 RVA: 0x00091C4C File Offset: 0x0008FE4C
			public override void OnEnter(Tome iOwner)
			{
				iOwner.mTomeController.StartClip(iOwner.mTomeClips[1], false);
				iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[2], false);
				iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[3], false);
				AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_bookclose".GetHashCodeCustom());
			}

			// Token: 0x060016BD RID: 5821 RVA: 0x00091CAB File Offset: 0x0008FEAB
			public override void OnExit(Tome iOwner)
			{
			}

			// Token: 0x040017F9 RID: 6137
			private static Tome.CloseToBack mSingelton;

			// Token: 0x040017FA RID: 6138
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002DE RID: 734
		public class BackToFront : Tome.TomeState
		{
			// Token: 0x170005C3 RID: 1475
			// (get) Token: 0x060016BF RID: 5823 RVA: 0x00091CBC File Offset: 0x0008FEBC
			public static Tome.BackToFront Instance
			{
				get
				{
					if (Tome.BackToFront.mSingelton == null)
					{
						lock (Tome.BackToFront.mSingeltonLock)
						{
							if (Tome.BackToFront.mSingelton == null)
							{
								Tome.BackToFront.mSingelton = new Tome.BackToFront();
							}
						}
					}
					return Tome.BackToFront.mSingelton;
				}
			}

			// Token: 0x060016C0 RID: 5824 RVA: 0x00091D10 File Offset: 0x0008FF10
			private BackToFront()
			{
			}

			// Token: 0x060016C1 RID: 5825 RVA: 0x00091D18 File Offset: 0x0008FF18
			public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
			{
				Tome.RenderData renderData = iOwner.mRenderData[(int)iDataChannel];
				iOwner.mTomeController.Update(iDeltaTime, ref this.identity, true);
				Array.Copy(iOwner.mTomeController.SkinnedBoneTransforms, renderData.mTomeBones, renderData.mTomeBones.Length);
				iOwner.mBackGroundPageControllerLeft.Update(iDeltaTime, ref this.identity, true);
				Array.Copy(iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
				iOwner.mBackGroundPageControllerRight.Update(iDeltaTime, ref this.identity, true);
				Array.Copy(iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
				if (iOwner.mTomeController.HasFinished && !iOwner.mTomeController.CrossFadeEnabled)
				{
					iOwner.ChangeState(Tome.OpenState.Instance);
				}
			}

			// Token: 0x060016C2 RID: 5826 RVA: 0x00091DE8 File Offset: 0x0008FFE8
			public override void OnEnter(Tome iOwner)
			{
				iOwner.mTomeController.StartClip(iOwner.mTomeClips[0], false);
				iOwner.mBackGroundPageControllerLeft.PlaybackMode = PlaybackMode.Forward;
				iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[0], false);
				iOwner.mBackGroundPageControllerLeft.Update(0.001f, ref this.identity, true);
				iOwner.mBackGroundPageControllerRight.PlaybackMode = PlaybackMode.Forward;
				iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[1], false);
				iOwner.mBackGroundPageControllerRight.Update(0.001f, ref this.identity, true);
				AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_bookclose".GetHashCodeCustom());
			}

			// Token: 0x060016C3 RID: 5827 RVA: 0x00091E8D File Offset: 0x0009008D
			public override void OnExit(Tome iOwner)
			{
			}

			// Token: 0x040017FB RID: 6139
			private static Tome.BackToFront mSingelton;

			// Token: 0x040017FC RID: 6140
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002DF RID: 735
		public class ClosedBack : Tome.TomeState
		{
			// Token: 0x170005C4 RID: 1476
			// (get) Token: 0x060016C5 RID: 5829 RVA: 0x00091EA0 File Offset: 0x000900A0
			public static Tome.ClosedBack Instance
			{
				get
				{
					if (Tome.ClosedBack.mSingelton == null)
					{
						lock (Tome.ClosedBack.mSingeltonLock)
						{
							if (Tome.ClosedBack.mSingelton == null)
							{
								Tome.ClosedBack.mSingelton = new Tome.ClosedBack();
							}
						}
					}
					return Tome.ClosedBack.mSingelton;
				}
			}

			// Token: 0x060016C6 RID: 5830 RVA: 0x00091EF4 File Offset: 0x000900F4
			private ClosedBack()
			{
			}

			// Token: 0x060016C7 RID: 5831 RVA: 0x00091EFC File Offset: 0x000900FC
			public override void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
			{
			}

			// Token: 0x060016C8 RID: 5832 RVA: 0x00091EFE File Offset: 0x000900FE
			public override void OnEnter(Tome iOwner)
			{
				base.OnEnter(iOwner);
				if (iOwner.OnBackClose != null)
				{
					iOwner.OnBackClose.Invoke();
				}
			}

			// Token: 0x040017FD RID: 6141
			private static Tome.ClosedBack mSingelton;

			// Token: 0x040017FE RID: 6142
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020002E0 RID: 736
		public enum RightHandItemType
		{
			// Token: 0x04001800 RID: 6144
			None = -1,
			// Token: 0x04001801 RID: 6145
			AccLogIn,
			// Token: 0x04001802 RID: 6146
			AccCreate,
			// Token: 0x04001803 RID: 6147
			Promotion,
			// Token: 0x04001804 RID: 6148
			Facebook,
			// Token: 0x04001805 RID: 6149
			Twitter,
			// Token: 0x04001806 RID: 6150
			Max
		}

		// Token: 0x020002E1 RID: 737
		private struct PageSoundVariables : IAudioVariables
		{
			// Token: 0x060016CA RID: 5834 RVA: 0x00091F28 File Offset: 0x00090128
			public void AssignToCue(Cue iCue)
			{
				iCue.SetVariable(Tome.PageSoundVariables.VARIABLE, this.Pages);
			}

			// Token: 0x04001807 RID: 6151
			public static readonly string VARIABLE = "Pages";

			// Token: 0x04001808 RID: 6152
			public float Pages;
		}

		// Token: 0x020002E2 RID: 738
		private class RenderData : IRenderableGUIObject, IPreRenderRenderer
		{
			// Token: 0x060016CC RID: 5836 RVA: 0x00091F48 File Offset: 0x00090148
			public RenderData(RenderTarget2D iRenderTarget, RenderTarget2D iShadowMap, DepthStencilBuffer iShadowDepthBuffer, SkinnedModel iTomeModel, SkinnedModel iPageModel, SkinnedModel iBackGroundPageModel, SkinnedModel iBackgroundModel, VertexBuffer iVertexBuffer, Texture2D iTexture, VertexDeclaration iVertexDeclaration, GUIBasicEffect iEffect)
			{
				this.mEffect = iEffect;
				this.mTexture = iTexture;
				this.mTomeModel = iTomeModel;
				this.mBackgroundModel = iBackgroundModel;
				this.mRenderTarget = iRenderTarget;
				this.mShadowMap = iShadowMap;
				this.mShadowDepthBuffer = iShadowDepthBuffer;
				this.mVertexBuffer = iVertexBuffer;
				this.mVertexDeclaration = iVertexDeclaration;
				this.mTomeModel = iTomeModel;
				this.mBackGroundPageModel = iBackGroundPageModel;
				this.mPageModel = iPageModel;
				this.mBackgroundBone = new Matrix[1];
				this.mBackgroundBone[0] = Matrix.Identity;
				this.mBackgroundPageBonesLeft = new Matrix[iBackGroundPageModel.SkeletonBones.Count];
				this.mBackgroundPageBonesRight = new Matrix[iBackGroundPageModel.SkeletonBones.Count];
				this.mTomeBones = new Matrix[iTomeModel.SkeletonBones.Count];
				this.mPageBones = new Matrix[Tome.MaxNrOfPages][];
				for (int i = 0; i < Tome.MaxNrOfPages; i++)
				{
					this.mPageBones[i] = new Matrix[iPageModel.SkeletonBones.Count];
				}
			}

			// Token: 0x060016CD RID: 5837 RVA: 0x00092058 File Offset: 0x00090258
			public void Draw(float iDeltaTime)
			{
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				graphicsDevice.RenderState.DepthBufferEnable = true;
				graphicsDevice.RenderState.DepthBufferWriteEnable = true;
				for (int i = 0; i < this.mBackGroundPageModel.Model.Meshes.Count; i++)
				{
					ModelMesh modelMesh = this.mBackGroundPageModel.Model.Meshes[i];
					for (int j = 0; j < modelMesh.Effects.Count; j++)
					{
						(modelMesh.Effects[j] as SkinnedModelBasicEffect).DiffuseIsHDR = true;
						(modelMesh.Effects[j] as SkinnedModelBasicEffect).DiffuseMap0 = this.mRenderTarget.GetTexture();
					}
				}
				for (int k = 0; k < this.mPageModel.Model.Meshes.Count; k++)
				{
					ModelMesh modelMesh2 = this.mPageModel.Model.Meshes[k];
					for (int l = 0; l < modelMesh2.Effects.Count; l++)
					{
						(modelMesh2.Effects[l] as SkinnedModelBasicEffect).DiffuseIsHDR = true;
						(modelMesh2.Effects[l] as SkinnedModelBasicEffect).DiffuseMap0 = this.mRenderTarget.GetTexture();
					}
				}
				if (GameStateManager.Instance.CurrentState is MenuState)
				{
					this.DrawModel(this.mBackgroundModel, this.mBackgroundBone);
				}
				Game.Instance.GraphicsDevice.RenderState.AlphaBlendEnable = false;
				this.DrawModel(this.mTomeModel, this.mTomeBones);
				Game.Instance.GraphicsDevice.RenderState.AlphaBlendEnable = true;
				this.DrawModel(this.mBackGroundPageModel, this.mBackgroundPageBonesRight);
				this.DrawModel(this.mBackGroundPageModel, this.mBackgroundPageBonesLeft);
				if (this.State is Tome.RiffleBackwardState || this.State is Tome.RiffleForwardState)
				{
					for (int m = 0; m < Tome.Instance.mRiffleController.ControllersPlaying; m++)
					{
						this.DrawModel(this.mPageModel, this.mPageBones[m]);
					}
				}
				graphicsDevice.RenderState.DepthBufferEnable = false;
				graphicsDevice.RenderState.DepthBufferWriteEnable = false;
				Vector4 color = default(Vector4);
				color.X = (color.Y = (color.Z = 1f));
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				Point screenSize = RenderManager.Instance.ScreenSize;
				this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				if (!(this.CurrentMenu is SubMenuCutscene) && !(this.CurrentMenu is SubMenuIntro))
				{
					if (Tome.sPromotionButton != null)
					{
						Tome.sPromotionButton.Draw(this.mEffect);
					}
					if (Tome.sShownSplashIsNew)
					{
						Tome.sSplashNew.Draw(this.mEffect);
					}
					Tome.sFaceBook.Draw(this.mEffect);
					Tome.sTwitter.Draw(this.mEffect);
					Tome.sIndicatorBackground.Draw(this.mEffect);
					Tome.sParadoxIcon.Draw(this.mEffect);
					if (!Singleton<ParadoxAccount>.Instance.IsLoggedFull)
					{
						if (Tome.sAccountLoginBtn.Enabled)
						{
							Tome.sAccountLoginBtn.Draw(this.mEffect);
						}
						if (Tome.sAccountCreationBtn.Enabled)
						{
							Tome.sAccountCreationBtn.Draw(this.mEffect);
						}
					}
					this.mEffect.Color = Vector4.One;
					Tome.sVersionText.Draw(this.mEffect, 16f, (float)screenSize.Y - 16f - (float)Tome.sVersionText.Font.LineHeight);
				}
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
				this.mEffect.TextureOffset = default(Vector2);
				this.mEffect.TextureScale = Vector2.One;
				color.W = 1f;
				this.mEffect.Color = color;
			}

			// Token: 0x060016CE RID: 5838 RVA: 0x00092480 File Offset: 0x00090680
			private void DrawModel(SkinnedModel iModel, Matrix[] iBones)
			{
				this.DrawModel(iModel, iBones, SkinnedModelBasicEffect.Technique.NonDeffered, ref this.View, ref this.Projection, ref this.ViewProjection);
			}

			// Token: 0x060016CF RID: 5839 RVA: 0x000924A0 File Offset: 0x000906A0
			private void DrawModel(SkinnedModel iModel, Matrix[] iBones, SkinnedModelBasicEffect.Technique iTechnique, ref Matrix iView, ref Matrix iProjection, ref Matrix iViewProjection)
			{
				foreach (ModelMesh modelMesh in iModel.Model.Meshes)
				{
					foreach (Effect effect in modelMesh.Effects)
					{
						SkinnedModelBasicEffect skinnedModelBasicEffect = (SkinnedModelBasicEffect)effect;
						skinnedModelBasicEffect.SetTechnique(iTechnique);
						skinnedModelBasicEffect.View = iView;
						skinnedModelBasicEffect.Projection = iProjection;
						skinnedModelBasicEffect.ViewProjection = iViewProjection;
						skinnedModelBasicEffect.LightPosition = Tome.sLightPosition;
						skinnedModelBasicEffect.LightAmbientColor = this.LightAmbient;
						skinnedModelBasicEffect.LightDiffuseColor = this.LightDiffuse;
						skinnedModelBasicEffect.LightSpecularColor = this.LightSpecular;
						skinnedModelBasicEffect.NormalPower = 1f;
						if (iTechnique != SkinnedModelBasicEffect.Technique.Shadow)
						{
							skinnedModelBasicEffect.LightViewProjection = this.mLightViewProj;
							skinnedModelBasicEffect.ShadowMapEnabled = true;
							skinnedModelBasicEffect.ShadowMap = this.mShadowMap.GetTexture();
							skinnedModelBasicEffect.ShadowMapScale = 1f / (float)this.mShadowMap.Width;
						}
						skinnedModelBasicEffect.Bones = iBones;
						skinnedModelBasicEffect.CommitChanges();
					}
					modelMesh.Draw();
				}
			}

			// Token: 0x060016D0 RID: 5840 RVA: 0x00092610 File Offset: 0x00090810
			private void DrawShadows()
			{
				this.mShadowMap.GraphicsDevice.SetRenderTarget(0, this.mShadowMap);
				this.mShadowMap.GraphicsDevice.DepthStencilBuffer = this.mShadowDepthBuffer;
				this.mShadowMap.GraphicsDevice.RenderState.DepthBufferEnable = true;
				this.mShadowMap.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
				this.mShadowMap.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
				this.mShadowMap.GraphicsDevice.RenderState.StencilEnable = false;
				this.mShadowMap.GraphicsDevice.RenderState.AlphaBlendEnable = false;
				this.mShadowMap.GraphicsDevice.Clear(Color.White);
				Vector3 vector = new Vector3(-2f, 0f, 2.5f);
				Vector3 vector2 = default(Vector3);
				vector2.Y = 1f;
				Matrix matrix;
				Matrix.CreateLookAt(ref Tome.sLightPosition, ref vector, ref vector2, out matrix);
				Matrix matrix2;
				Matrix.CreatePerspectiveFieldOfView(1.8849558f, 1f, 1f, 15f, out matrix2);
				Matrix.Multiply(ref matrix, ref matrix2, out this.mLightViewProj);
				if (this.State is Tome.RiffleBackwardState | this.State is Tome.RiffleForwardState)
				{
					for (int i = 0; i < Tome.Instance.mRiffleController.ControllersPlaying; i++)
					{
						this.DrawModel(this.mPageModel, this.mPageBones[i], SkinnedModelBasicEffect.Technique.Shadow, ref matrix, ref matrix2, ref this.mLightViewProj);
					}
				}
				this.DrawModel(this.mBackGroundPageModel, this.mBackgroundPageBonesRight, SkinnedModelBasicEffect.Technique.Shadow, ref matrix, ref matrix2, ref this.mLightViewProj);
				this.DrawModel(this.mBackGroundPageModel, this.mBackgroundPageBonesLeft, SkinnedModelBasicEffect.Technique.Shadow, ref matrix, ref matrix2, ref this.mLightViewProj);
				this.DrawModel(this.mTomeModel, this.mTomeBones, SkinnedModelBasicEffect.Technique.Shadow, ref matrix, ref matrix2, ref this.mLightViewProj);
				this.DrawModel(this.mBackgroundModel, this.mBackgroundBone, SkinnedModelBasicEffect.Technique.Shadow, ref matrix, ref matrix2, ref this.mLightViewProj);
			}

			// Token: 0x170005C5 RID: 1477
			// (get) Token: 0x060016D1 RID: 5841 RVA: 0x00092807 File Offset: 0x00090A07
			public int ZIndex
			{
				get
				{
					return 900;
				}
			}

			// Token: 0x060016D2 RID: 5842 RVA: 0x00092810 File Offset: 0x00090A10
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				this.mRenderTarget.GraphicsDevice.SetRenderTarget(0, this.mRenderTarget);
				this.mRenderTarget.GraphicsDevice.DepthStencilBuffer = null;
				this.mRenderTarget.GraphicsDevice.RenderState.DepthBufferEnable = false;
				this.mRenderTarget.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
				this.mRenderTarget.GraphicsDevice.RenderState.AlphaBlendEnable = false;
				this.mRenderTarget.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
				this.mRenderTarget.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				this.mRenderTarget.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
				this.mRenderTarget.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
				this.mRenderTarget.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One;
				Matrix identity = Matrix.Identity;
				this.mEffect.SetScreenSize(this.mRenderTarget.Width, this.mRenderTarget.Height);
				this.mEffect.Transform = identity;
				this.mEffect.Texture = this.mTexture;
				this.mEffect.VertexColorEnabled = false;
				this.mEffect.ScaleToHDR = true;
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
				this.mRenderTarget.GraphicsDevice.RenderState.AlphaBlendEnable = true;
				this.mRenderTarget.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
				this.mRenderTarget.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.One;
				this.mRenderTarget.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.Zero;
				if (this.CurrentMenu != null)
				{
					this.State.Draw(this.mEffect, Tome.Instance, this.CurrentMenu);
				}
				this.DrawShadows();
			}

			// Token: 0x04001809 RID: 6153
			public Matrix View;

			// Token: 0x0400180A RID: 6154
			public Matrix Projection;

			// Token: 0x0400180B RID: 6155
			public Matrix ViewProjection;

			// Token: 0x0400180C RID: 6156
			private Matrix[] mBackgroundBone;

			// Token: 0x0400180D RID: 6157
			public Matrix[] mTomeBones;

			// Token: 0x0400180E RID: 6158
			public Matrix[] mBackgroundPageBonesRight;

			// Token: 0x0400180F RID: 6159
			public Matrix[] mBackgroundPageBonesLeft;

			// Token: 0x04001810 RID: 6160
			public Matrix[][] mPageBones;

			// Token: 0x04001811 RID: 6161
			private SkinnedModel mTomeModel;

			// Token: 0x04001812 RID: 6162
			private SkinnedModel mPageModel;

			// Token: 0x04001813 RID: 6163
			private SkinnedModel mBackGroundPageModel;

			// Token: 0x04001814 RID: 6164
			private SkinnedModel mBackgroundModel;

			// Token: 0x04001815 RID: 6165
			private Matrix mLightViewProj;

			// Token: 0x04001816 RID: 6166
			public Vector3 LightAmbient;

			// Token: 0x04001817 RID: 6167
			public Vector3 LightDiffuse;

			// Token: 0x04001818 RID: 6168
			public Vector3 LightSpecular;

			// Token: 0x04001819 RID: 6169
			private GUIBasicEffect mEffect;

			// Token: 0x0400181A RID: 6170
			private Texture2D mTexture;

			// Token: 0x0400181B RID: 6171
			private VertexBuffer mVertexBuffer;

			// Token: 0x0400181C RID: 6172
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x0400181D RID: 6173
			private RenderTarget2D mRenderTarget;

			// Token: 0x0400181E RID: 6174
			private RenderTarget2D mShadowMap;

			// Token: 0x0400181F RID: 6175
			private DepthStencilBuffer mShadowDepthBuffer;

			// Token: 0x04001820 RID: 6176
			public Tome.TomeState State;

			// Token: 0x04001821 RID: 6177
			public SubMenu CurrentMenu;
		}

		// Token: 0x020002E3 RID: 739
		public class RiffleController
		{
			// Token: 0x060016D3 RID: 5843 RVA: 0x00092A70 File Offset: 0x00090C70
			public RiffleController(int iNrOfControllers, SkinnedModel iModel)
			{
				this.mTimer = 0f;
				this.mSpeed = 2f;
				this.mActiveClipIndex = 0;
				this.mNrOfControllersActive = 0;
				this.mNrOfControllersLeftToActivate = 0;
				this.mNrOfControllers = iNrOfControllers;
				this.mPlaybackMode = PlaybackMode.Forward;
				this.mControllers = new AnimationController[iNrOfControllers];
				for (int i = 0; i < iNrOfControllers; i++)
				{
					this.mControllers[i] = new AnimationController();
					this.mControllers[i].Skeleton = iModel.SkeletonBones;
				}
				this.mClips = new AnimationClip[2];
				this.mClips[0] = iModel.AnimationClips["flip_left"];
				this.mClips[1] = iModel.AnimationClips["flip_right"];
				this.mTimerOffset = this.mClips[0].Duration * 0.625f / (float)this.mNrOfControllers;
			}

			// Token: 0x060016D4 RID: 5844 RVA: 0x00092B5C File Offset: 0x00090D5C
			public void ArrayCopyTo(ref Matrix[][] iTarget)
			{
				for (int i = 0; i < this.mNrOfControllers; i++)
				{
					Array.Copy(this.mControllers[i].SkinnedBoneTransforms, iTarget[i], iTarget[i].Length);
				}
			}

			// Token: 0x170005C6 RID: 1478
			// (get) Token: 0x060016D5 RID: 5845 RVA: 0x00092B96 File Offset: 0x00090D96
			public int ControllersPlaying
			{
				get
				{
					return this.mNrOfControllersActive;
				}
			}

			// Token: 0x060016D6 RID: 5846 RVA: 0x00092BA0 File Offset: 0x00090DA0
			public void StartClip(Tome.FlipPageAnimations iAnimation, bool iLoop, int iNumControllers)
			{
				if (iNumControllers > this.mNrOfControllers)
				{
					iNumControllers = this.mNrOfControllers;
				}
				float elapsedTime = 0f;
				this.mPlaybackMode = PlaybackMode.Forward;
				this.mActiveClipIndex = (int)iAnimation;
				this.mNrOfControllersActive = iNumControllers;
				this.mNrOfControllersLeftToActivate = iNumControllers;
				for (int i = 0; i < iNumControllers; i++)
				{
					this.mControllers[i].Speed = this.mSpeed;
					this.mControllers[i].PlaybackMode = PlaybackMode.Forward;
					this.mControllers[i].StartClip(this.mClips[(int)iAnimation], iLoop);
					this.mControllers[i].Update(elapsedTime, ref this.identity, true);
					this.mControllers[i].PlaybackMode = this.mPlaybackMode;
				}
			}

			// Token: 0x060016D7 RID: 5847 RVA: 0x00092C4C File Offset: 0x00090E4C
			public void Update(float iDeltaTime)
			{
				this.mTimer -= iDeltaTime;
				while (this.mTimer <= 0f && this.mNrOfControllersLeftToActivate > 0)
				{
					this.mNrOfControllersLeftToActivate--;
					int num = this.mNrOfControllersLeftToActivate;
					this.mControllers[num].StartClip(this.mClips[this.mActiveClipIndex], false);
					this.mControllers[num].Update(-this.mTimer + 0.01f, ref this.identity, false);
					this.mTimer += this.mTimerOffset;
				}
				if (this.mNrOfControllersLeftToActivate == 0)
				{
					this.mTimer = 0f;
				}
				for (int i = 0; i < this.mNrOfControllers; i++)
				{
					if (this.mControllers[i].IsPlaying)
					{
						this.mControllers[i].Update(iDeltaTime, ref this.identity, true);
					}
				}
			}

			// Token: 0x170005C7 RID: 1479
			// (get) Token: 0x060016D8 RID: 5848 RVA: 0x00092D2C File Offset: 0x00090F2C
			public bool HasFinished
			{
				get
				{
					for (int i = 0; i < this.mNrOfControllersActive; i++)
					{
						if (!this.mControllers[i].HasFinished)
						{
							return false;
						}
					}
					return true;
				}
			}

			// Token: 0x04001822 RID: 6178
			private PlaybackMode mPlaybackMode;

			// Token: 0x04001823 RID: 6179
			private Matrix identity = Matrix.Identity;

			// Token: 0x04001824 RID: 6180
			private AnimationController[] mControllers;

			// Token: 0x04001825 RID: 6181
			private AnimationClip[] mClips;

			// Token: 0x04001826 RID: 6182
			private int mNrOfControllers;

			// Token: 0x04001827 RID: 6183
			private float mSpeed;

			// Token: 0x04001828 RID: 6184
			private float mTimer;

			// Token: 0x04001829 RID: 6185
			private float mTimerOffset;

			// Token: 0x0400182A RID: 6186
			private int mActiveClipIndex;

			// Token: 0x0400182B RID: 6187
			private int mNrOfControllersActive;

			// Token: 0x0400182C RID: 6188
			private int mNrOfControllersLeftToActivate;
		}

		// Token: 0x020002E4 RID: 740
		public enum CameraAnimation
		{
			// Token: 0x0400182E RID: 6190
			Wake_Up,
			// Token: 0x0400182F RID: 6191
			Idle,
			// Token: 0x04001830 RID: 6192
			Look_Left,
			// Token: 0x04001831 RID: 6193
			Look_Back,
			// Token: 0x04001832 RID: 6194
			Doze_Off,
			// Token: 0x04001833 RID: 6195
			Zoomed_In
		}

		// Token: 0x020002E5 RID: 741
		public enum TomeAnimations
		{
			// Token: 0x04001835 RID: 6197
			Open,
			// Token: 0x04001836 RID: 6198
			Close,
			// Token: 0x04001837 RID: 6199
			Turn,
			// Token: 0x04001838 RID: 6200
			NrOfAnimations
		}

		// Token: 0x020002E6 RID: 742
		public enum BackGroundPageAnimations
		{
			// Token: 0x0400183A RID: 6202
			OpenLeft,
			// Token: 0x0400183B RID: 6203
			OpenRight,
			// Token: 0x0400183C RID: 6204
			CloseLeft,
			// Token: 0x0400183D RID: 6205
			CloseRight,
			// Token: 0x0400183E RID: 6206
			NrOfAnimations
		}

		// Token: 0x020002E7 RID: 743
		public enum FlipPageAnimations
		{
			// Token: 0x04001840 RID: 6208
			FlipLeft,
			// Token: 0x04001841 RID: 6209
			FlipRight,
			// Token: 0x04001842 RID: 6210
			NrOfAnimations
		}

		// Token: 0x020002E8 RID: 744
		internal enum AccountIndicatorState
		{
			// Token: 0x04001844 RID: 6212
			LoggedIn,
			// Token: 0x04001845 RID: 6213
			LoggedOut
		}
	}
}
