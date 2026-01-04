// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Tome
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.UI;

internal sealed class Tome
{
  private const string AD_CLICKED_EVENT_KEY = "AdClicked";
  private const string AD_PROPERTY_NAME = "Ads";
  private const string ADS_PROPERTY_SET_NAME = "ABTestAdLevel";
  private const float PICK_AREA_HEIGHT = 5.5f;
  private const float PICK_AREA_WIDTH = 5.3f;
  private const float ACCOUNT_PADDING = 10f;
  private const float ACCOUNT_PADDING_INNER = 20f;
  private const float ACCOUNT_BTN_WIDTH = 230f;
  private const float ACCOUNT_BTN_HEIGHT = 100f;
  private const float ACCOUNT_BTN_SPACING = 25f;
  private const float FADE_IN_TIME = 0.25f;
  private const float FACEBOOK_UV_X = 1070f;
  private const float FACEBOOK_UV_Y = 830f;
  private const float FACEBOOK_UV_WIDTH = 104f;
  private const float FACEBOOK_UV_HEIGHT = 66f;
  private const float TWITTER_UV_X = 1174f;
  private const float TWITTER_UV_Y = 830f;
  private const float TWITTER_UV_WIDTH = 104f;
  private const float TWITTER_UV_HEIGHT = 66f;
  private const float SPLASH_RIGHT_PADDING = 11f;
  private const float FB_TWITTER_PADDING = 5f;
  private const float SPLASH_BOTTOM_PADDING = 11f;
  private const float SPLASH_NEW_PADDING = 3f;
  private const float SPLASH_NEW_UV_X = 1081f;
  private const float SPLASH_NEW_UV_Y = 896f;
  private const float SPLASH_NEW_UV_WIDTH = 82f;
  private const float SPLASH_NEW_UV_HEIGHT = 38f;
  private static Vector3 sLightPosition;
  private static float mCurrentTime;
  private static readonly Vector2 PROMOTION_BUTTON_POSITION = new Vector2(216f, 316f);
  private static Tome mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly Viewport TOMERIGHTSHEET;
  public static readonly Viewport TOMELEFTSHEET;
  public static readonly Viewport PAGELEFTSHEET;
  public static readonly Viewport PAGERIGHTSHEET;
  public static readonly int MaxNrOfPages = 10;
  private AnimationClip[] mTomeClips;
  private AnimationClip[] mBackGroundClips;
  private AnimationController mTomeController;
  private AnimationController mBackGroundPageControllerLeft;
  private AnimationController mBackGroundPageControllerRight;
  private Tome.RiffleController mRiffleController;
  private Tome.TomeState mCurrentState;
  private static Vector3 mCameraPosition;
  private Matrix mTomeBindPose;
  private int mTomeBone;
  private Matrix mCameraBindPose;
  private int mCameraBone;
  private AnimationClip[] mCameraAnimations;
  private AnimationController mCameraAnimation;
  private SubMenu[] mMenuStack;
  private int mMenuStackPosition = -1;
  private Stack<int> mMenuRiffleStack;
  private bool mInputLocked;
  public bool CloseToMenu;
  private Tome.RenderData[] mRenderData;
  private static readonly JigLibX.Geometry.Triangle[] LEFT_TRIS;
  private static readonly JigLibX.Geometry.Triangle[] RIGHT_TRIS;
  private static readonly Vector3 PICK_LEFT_AREA_TOPRIGHT = new Vector3(0.0f, 0.5f, 2.75f);
  private static readonly Vector3 PICK_LEFT_AREA_BOTTOMLEFT = new Vector3(5.3f, 0.5f, -2.75f);
  private static readonly Vector3 PICK_RIGHT_AREA_TOPRIGHT = new Vector3(-5.3f, 0.5f, 2.75f);
  private static readonly Vector3 PICK_RIGHT_AREA_BOTTOMLEFT = new Vector3(0.0f, 0.5f, -2.75f);
  private static Random sRandom = new Random();
  private float mLightVariationSpeed = 24f;
  private float mLightVariationAmount = 1f;
  private float mTargetLightVariationSpeed = 6f;
  private float mTargetLightVariationAmount = 0.2f;
  private float mLightVariationPosition;
  private float mLightIntensity;
  private float mTargetLightIntensity = 1f;
  private float mIntroTime;
  private bool mHurryAndOpen;
  private static readonly int LOC_LOGGEDIN = "#acc_loggedin".GetHashCodeCustom();
  private static readonly int LOC_LOGGEDOUT = "#acc_loggedout".GetHashCodeCustom();
  private static readonly int LOC_NOTICE = "#popup_notice".GetHashCodeCustom();
  private static readonly int LOC_STEAMLINK = "#steam_link".GetHashCodeCustom();
  private static readonly int LOC_PROCESSING = "#menu_main_processing".GetHashCodeCustom();
  private static MenuMessagePopup sProcessingPopup = (MenuMessagePopup) null;
  private static readonly Vector2 PARADOX_LOGO_UV = new Vector2(18f, 763f);
  private static readonly Vector2 PARADOX_LOGO_UV_SIZE = new Vector2(717f, 249f);
  private static readonly Vector2 PARADOX_LOGO_SIZE = new Vector2(150f, 52f) * 1.65f;
  private static readonly Vector2 ACCOUNT_BACKGROUND_UV = new Vector2(0.0f, 464f);
  private static readonly Vector2 ACCOUNT_BACKGROUND_UV_SIZE = new Vector2(448f, 560f);
  private static readonly Vector2 ACCOUNT_BACKGROUND_SIZE = new Vector2(300f, 400f);
  private static MenuImageTextItem sParadoxIcon;
  private static MenuImageTextItem sIndicatorBackground;
  private static MenuTextButtonItem sAccountLoginBtn;
  private static MenuTextButtonItem sAccountCreationBtn;
  private static bool sFadeInAccountIndicator = false;
  private static MenuMessagePopup sSteamLinkPopup = (MenuMessagePopup) null;
  private static readonly Vector2 FACEBOOK_SIZE = new Vector2(104f, 66f);
  private static readonly Vector2 TWITTER_SIZE = new Vector2(104f, 66f);
  private static readonly Vector2 SPLASH_NEW_SIZE = new Vector2(82f, 38f);
  private static MenuImageTextItem sFaceBook;
  private static MenuImageTextItem sTwitter;
  private static MenuImageTextItem sPromotionButton;
  private static bool sPromotionActive = false;
  private static MenuImageTextItem sSplashNew;
  private static bool sShownSplashIsNew = false;
  private static Text sVersionText;
  private int mAchievementsHash = "#achievement_text02".GetHashCodeCustom();
  private float mNewContentUpdateTimer;
  private static Matrix mViewProjection;
  private int currentExtraButtonIndex = -1;

  public static Tome Instance
  {
    get
    {
      if (Tome.mSingelton == null)
      {
        lock (Tome.mSingeltonLock)
        {
          if (Tome.mSingelton == null)
            Tome.mSingelton = new Tome();
        }
      }
      return Tome.mSingelton;
    }
  }

  public static bool MousePickTome(
    Point iScreenSize,
    int iX,
    int iY,
    out Vector2 oHitPosition,
    out bool oRightPageHit)
  {
    Matrix mViewProjection = Tome.mViewProjection;
    Matrix result1;
    Matrix.Invert(ref mViewProjection, out result1);
    Vector4 result2 = new Vector4((float) (2.0 * (double) iX / (double) iScreenSize.X - 1.0), (float) (1.0 - 2.0 * (double) iY / (double) iScreenSize.Y), 1f, 1f);
    Vector4.Transform(ref result2, ref result1, out result2);
    Vector3 result3 = new Vector3();
    result3.X = result2.X;
    result3.Y = result2.Y;
    result3.Z = result2.Z;
    Vector3.Divide(ref result3, result2.W, out result3);
    Vector3 mCameraPosition = Tome.mCameraPosition;
    Vector3 delta = result3 - mCameraPosition;
    Segment seg = new Segment(mCameraPosition, delta);
    JigLibX.Geometry.Triangle[] leftTris = Tome.LEFT_TRIS;
    JigLibX.Geometry.Triangle[] rightTris = Tome.RIGHT_TRIS;
    float tS;
    float tT0;
    float tT1;
    if (Intersection.SegmentTriangleIntersection(out tS, out tT0, out tT1, ref seg, ref rightTris[0], false))
    {
      oHitPosition = new Vector2((float) ((1.0 - (double) tT0 + 0.029999999329447746) * 0.97000002861022949) * (float) Tome.PAGERIGHTSHEET.Width, tT1 * (float) Tome.PAGERIGHTSHEET.Height);
      oRightPageHit = true;
      return true;
    }
    if (Intersection.SegmentTriangleIntersection(out tS, out tT0, out tT1, ref seg, ref rightTris[1], false))
    {
      oHitPosition = new Vector2((float) (((double) tT0 + 0.029999999329447746) * 0.97000002861022949) * (float) Tome.PAGERIGHTSHEET.Width, (1f - tT1) * (float) Tome.PAGERIGHTSHEET.Height);
      oRightPageHit = true;
      return true;
    }
    if (Intersection.SegmentTriangleIntersection(out tS, out tT0, out tT1, ref seg, ref leftTris[0], false))
    {
      oHitPosition = new Vector2((float) ((1.0 - (double) tT0) * 0.97000002861022949) * (float) Tome.PAGERIGHTSHEET.Width, tT1 * (float) Tome.PAGERIGHTSHEET.Height);
      oRightPageHit = false;
      return true;
    }
    if (Intersection.SegmentTriangleIntersection(out tS, out tT0, out tT1, ref seg, ref leftTris[1], false))
    {
      oHitPosition = new Vector2(tT0 * 0.97f * (float) Tome.PAGERIGHTSHEET.Width, (1f - tT1) * (float) Tome.PAGERIGHTSHEET.Height);
      oRightPageHit = false;
      return true;
    }
    oHitPosition = Vector2.Zero;
    oRightPageHit = false;
    return false;
  }

  public event Action OnOpen;

  public event Action OnClose;

  public event Action OnBackClose;

  public static bool PromotionActive
  {
    get => Tome.sPromotionActive;
    set => Tome.sPromotionActive = value;
  }

  static Tome()
  {
    Tome.TOMELEFTSHEET.X = 0;
    Tome.TOMELEFTSHEET.Width = 1024 /*0x0400*/;
    Tome.TOMELEFTSHEET.Y = 0;
    Tome.TOMELEFTSHEET.Height = 1024 /*0x0400*/;
    Tome.TOMERIGHTSHEET.X = 1024 /*0x0400*/;
    Tome.TOMERIGHTSHEET.Width = 1024 /*0x0400*/;
    Tome.TOMERIGHTSHEET.Y = 0;
    Tome.TOMERIGHTSHEET.Height = 1024 /*0x0400*/;
    Tome.PAGELEFTSHEET.X = 0;
    Tome.PAGELEFTSHEET.Width = 1024 /*0x0400*/;
    Tome.PAGELEFTSHEET.Y = 1024 /*0x0400*/;
    Tome.PAGELEFTSHEET.Height = 1024 /*0x0400*/;
    Tome.PAGERIGHTSHEET.X = 1024 /*0x0400*/;
    Tome.PAGERIGHTSHEET.Width = 1024 /*0x0400*/;
    Tome.PAGERIGHTSHEET.Y = 1024 /*0x0400*/;
    Tome.PAGERIGHTSHEET.Height = 1024 /*0x0400*/;
    Tome.LEFT_TRIS = new JigLibX.Geometry.Triangle[2];
    Tome.LEFT_TRIS[0].Origin = Tome.PICK_LEFT_AREA_TOPRIGHT - new Vector3(0.0f, 0.1f, 0.0f);
    Tome.LEFT_TRIS[0].Edge0 = new Vector3(5.3f, 0.1f, 0.0f);
    Tome.LEFT_TRIS[0].Edge1 = new Vector3(0.0f, 0.0f, -5.5f);
    Tome.LEFT_TRIS[1].Origin = Tome.PICK_LEFT_AREA_BOTTOMLEFT;
    Tome.LEFT_TRIS[1].Edge0 = new Vector3(-5.3f, -0.1f, 0.0f);
    Tome.LEFT_TRIS[1].Edge1 = new Vector3(0.0f, 0.0f, 5.5f);
    Tome.RIGHT_TRIS = new JigLibX.Geometry.Triangle[2];
    Tome.RIGHT_TRIS[0].Origin = Tome.PICK_RIGHT_AREA_TOPRIGHT;
    Tome.RIGHT_TRIS[0].Edge0 = new Vector3(5.3f, -0.1f, 0.0f);
    Tome.RIGHT_TRIS[0].Edge1 = new Vector3(0.0f, 0.0f, -5.5f);
    Tome.RIGHT_TRIS[1].Origin = Tome.PICK_RIGHT_AREA_BOTTOMLEFT - new Vector3(0.0f, 0.1f, 0.0f);
    Tome.RIGHT_TRIS[1].Edge0 = new Vector3(-5.3f, 0.1f, 0.0f);
    Tome.RIGHT_TRIS[1].Edge1 = new Vector3(0.0f, 0.0f, 5.5f);
  }

  private Tome()
  {
    this.currentExtraButtonIndex = -1;
    SkinnedModel iBackgroundModel;
    SkinnedModel skinnedModel1;
    SkinnedModel iTomeModel;
    SkinnedModel skinnedModel2;
    SkinnedModel iBackGroundPageModel;
    Texture2D iTexture1;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iBackgroundModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("UI/ToM/background_mesh");
      skinnedModel1 = Magicka.Game.Instance.Content.Load<SkinnedModel>("UI/ToM/background");
      iTomeModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("UI/ToM/tome");
      skinnedModel2 = Magicka.Game.Instance.Content.Load<SkinnedModel>("UI/ToM/page_flip");
      iBackGroundPageModel = Magicka.Game.Instance.Content.Load<SkinnedModel>("UI/ToM/page");
      iTexture1 = Magicka.Game.Instance.Content.Load<Texture2D>("UI/ToM/tome_pages_0");
    }
    Texture2D oTexture;
    Microsoft.Xna.Framework.Rectangle oRect;
    PdxWidgetWindow.GetTexture(PdxWidgetWindow.Textures.progress_indicator_on, out oTexture, out oRect);
    double num1 = (double) oRect.Left / (double) oTexture.Width;
    double num2 = (double) oRect.Top / (double) oTexture.Height;
    int num3 = oRect.Height / 2;
    int num4 = oRect.Width / 2;
    int num5 = oRect.Height / 2;
    double num6 = (double) oRect.Width / (double) oTexture.Width;
    double num7 = (double) oRect.Height / (double) oTexture.Height;
    Texture2D iTexture2 = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    Tome.sFaceBook = new MenuImageTextItem(new Vector2(), iTexture2, new Vector2(1070f / (float) iTexture2.Width, 830f / (float) iTexture2.Height), new Vector2(104f / (float) iTexture2.Width, 66f / (float) iTexture2.Height), 0, new Vector2(), TextAlign.Center, FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold), Tome.FACEBOOK_SIZE);
    Tome.sTwitter = new MenuImageTextItem(new Vector2(), iTexture2, new Vector2(1174f / (float) iTexture2.Width, 830f / (float) iTexture2.Height), new Vector2(104f / (float) iTexture2.Width, 66f / (float) iTexture2.Height), 0, new Vector2(), TextAlign.Center, FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold), Tome.TWITTER_SIZE);
    if (Tome.sSplashNew == null)
      Tome.sSplashNew = new MenuImageTextItem(Vector2.Zero, iTexture2, new Vector2(1081f / (float) iTexture2.Width, 896f / (float) iTexture2.Height), new Vector2(82f / (float) iTexture2.Width, 38f / (float) iTexture2.Height), 0, Vector2.Zero, TextAlign.Center, FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold), Tome.SPLASH_NEW_SIZE);
    Tome.sFaceBook.Alpha = 0.0f;
    Tome.sTwitter.Alpha = 0.0f;
    Tome.sSplashNew.Alpha = 0.0f;
    BitmapFont font1 = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold);
    Texture2D iTexture3 = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/tag_spritesheet");
    Tome.sParadoxIcon = new MenuImageTextItem(Vector2.Zero, iTexture3, new Vector2(Tome.PARADOX_LOGO_UV.X / (float) iTexture3.Width, Tome.PARADOX_LOGO_UV.Y / (float) iTexture3.Height), new Vector2(Tome.PARADOX_LOGO_UV_SIZE.X / (float) iTexture3.Width, Tome.PARADOX_LOGO_UV_SIZE.Y / (float) iTexture3.Height), 0, Vector2.Zero, TextAlign.Center, font2, Tome.PARADOX_LOGO_SIZE);
    Tome.sIndicatorBackground = new MenuImageTextItem(Vector2.Zero, iTexture2, new Vector2(Tome.ACCOUNT_BACKGROUND_UV.X / (float) iTexture2.Width, Tome.ACCOUNT_BACKGROUND_UV.Y / (float) iTexture2.Height), new Vector2(Tome.ACCOUNT_BACKGROUND_UV_SIZE.X / (float) iTexture2.Width, Tome.ACCOUNT_BACKGROUND_UV_SIZE.Y / (float) iTexture2.Height), "#acc_loggedout".GetHashCodeCustom(), Tome.ACCOUNT_BACKGROUND_SIZE / 2f, TextAlign.Center, font1, Tome.ACCOUNT_BACKGROUND_SIZE);
    Tome.sAccountCreationBtn = new MenuTextButtonItem(Vector2.Zero, iTexture2, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, "#acc_create_acc".GetHashCodeCustom(), font1, 230f, 230f, TextAlign.Center);
    Tome.sAccountLoginBtn = new MenuTextButtonItem(Vector2.Zero, iTexture2, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, "#acc_log_in".GetHashCodeCustom(), font1, 230f, 230f, TextAlign.Center);
    Tome.sIndicatorBackground.Alpha = 0.0f;
    Tome.sParadoxIcon.Alpha = 0.0f;
    Tome.sIndicatorBackground.Alpha = 0.0f;
    Tome.sAccountLoginBtn.Alpha = 0.0f;
    Tome.sAccountCreationBtn.Alpha = 0.0f;
    Tome.sSteamLinkPopup = new MenuMessagePopup();
    Tome.sSteamLinkPopup.Alignment = PopupAlign.Middle | PopupAlign.Center;
    Tome.sProcessingPopup = new MenuMessagePopup();
    Tome.sProcessingPopup.Alignment = PopupAlign.Middle | PopupAlign.Center;
    Tome.sProcessingPopup.ClearOnHide = false;
    Tome.sProcessingPopup.CanDismiss = false;
    Tome.sProcessingPopup.SetMessage(Tome.LOC_PROCESSING, Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR);
    Tome.sProcessingPopup.SetButtonType(ButtonConfig.None);
    Tome.sProcessingPopup.EnableLoadingIcon();
    ParadoxAccount.OnBecameIdle += (ParadoxAccount.BecameIdleDelegate) (() =>
    {
      Tome.SetIndicatorState(Singleton<ParadoxAccount>.Instance.IsLoggedFull ? Tome.AccountIndicatorState.LoggedIn : Tome.AccountIndicatorState.LoggedOut);
      Singleton<PopupSystem>.Instance.ForceDismissCurrentPopupIfMatches((MenuBasePopup) Tome.sProcessingPopup);
    });
    ParadoxAccount.OnBecameBusy += (ParadoxAccount.BecameBusyDelegate) (() => Singleton<PopupSystem>.Instance.ShowPopupImmediately((MenuBasePopup) Tome.sProcessingPopup));
    this.mNewContentUpdateTimer = 600f;
    Tome.sVersionText = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Left, false);
    Tome.sVersionText.ShadowAlpha = 1f;
    Tome.sVersionText.ShadowsOffset = Vector2.One;
    Tome.sVersionText.DrawShadows = true;
    Tome.sVersionText.SetText(Application.ProductVersion);
    while (HackHelper.LicenseStatus == HackHelper.Status.Pending)
      Thread.Sleep(1);
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      Tome.sVersionText.Append(" (Modified)");
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
    {
      OptionsMessageBox mbox = new OptionsMessageBox("#notice_mod".GetHashCode(), new int[1]
      {
        "#add_menu_ok".GetHashCode()
      });
      LanguageManager.Instance.LanguageChanged -= new Action(((MessageBox) mbox).LanguageChanged);
      this.OnOpen += (Action) (() => mbox.Show());
    }
    this.OnOpen += (Action) (() => Tome.sFadeInAccountIndicator = true);
    foreach (SkinnedModelBone skeletonBone in (ReadOnlyCollection<SkinnedModelBone>) skinnedModel1.SkeletonBones)
    {
      if (skeletonBone.Name.Equals("light", StringComparison.OrdinalIgnoreCase))
      {
        Matrix result1;
        Matrix.CreateRotationY(3.14159274f, out result1);
        Matrix result2 = skeletonBone.InverseBindPoseTransform;
        Matrix.Multiply(ref result2, ref result1, out result2);
        Matrix.Invert(ref result2, out result2);
        Tome.sLightPosition = result2.Translation;
      }
      else
      {
        if (skeletonBone.Name.Equals("camera", StringComparison.OrdinalIgnoreCase))
        {
          this.mCameraBone = (int) skeletonBone.Index;
          this.mCameraBindPose = skeletonBone.InverseBindPoseTransform;
          Matrix.Invert(ref this.mCameraBindPose, out this.mCameraBindPose);
          Matrix result;
          Matrix.CreateRotationY(3.14159274f, out result);
          Matrix.Multiply(ref this.mCameraBindPose, ref result, out this.mCameraBindPose);
          break;
        }
        if (skeletonBone.Name.Equals("camera", StringComparison.OrdinalIgnoreCase))
        {
          this.mTomeBone = (int) skeletonBone.Index;
          this.mTomeBindPose = skeletonBone.InverseBindPoseTransform;
          Matrix.Invert(ref this.mTomeBindPose, out this.mTomeBindPose);
          Matrix result;
          Matrix.CreateRotationY(3.14159274f, out result);
          Matrix.Multiply(ref this.mTomeBindPose, ref result, out this.mTomeBindPose);
          break;
        }
      }
    }
    this.mCameraAnimation = new AnimationController();
    this.mCameraAnimation.Skeleton = skinnedModel1.SkeletonBones;
    this.mCameraAnimations = new AnimationClip[6];
    foreach (AnimationClip animationClip in skinnedModel1.AnimationClips.Values)
    {
      if (animationClip.Name.Equals("wakeup", StringComparison.OrdinalIgnoreCase))
        this.mCameraAnimations[0] = animationClip;
      else if (animationClip.Name.Equals("idle", StringComparison.OrdinalIgnoreCase))
        this.mCameraAnimations[1] = animationClip;
      else if (animationClip.Name.Equals("lookleft", StringComparison.OrdinalIgnoreCase))
        this.mCameraAnimations[2] = animationClip;
      else if (animationClip.Name.Equals("lookback", StringComparison.OrdinalIgnoreCase))
        this.mCameraAnimations[3] = animationClip;
      else if (animationClip.Name.Equals("dozeoff", StringComparison.OrdinalIgnoreCase))
        this.mCameraAnimations[4] = animationClip;
      else if (animationClip.Name.Equals("exitlevel", StringComparison.OrdinalIgnoreCase))
        this.mCameraAnimations[5] = animationClip;
    }
    for (int index1 = 0; index1 < iBackgroundModel.Model.Meshes.Count; ++index1)
    {
      ModelMesh mesh = iBackgroundModel.Model.Meshes[index1];
      for (int index2 = 0; index2 < mesh.Effects.Count; ++index2)
      {
        SkinnedModelBasicEffect effect = mesh.Effects[index2] as SkinnedModelBasicEffect;
        effect.SetTechnique(SkinnedModelBasicEffect.Technique.NonDeffered);
        effect.LightPosition = new Vector3(5f, 5f, 0.0f);
        effect.LightAmbientColor = new Vector3(0.4f);
        effect.LightDiffuseColor = new Vector3(0.7f);
        effect.LightSpecularColor = new Vector3(0.4f);
        effect.NormalPower = 1f;
        effect.LightPosition = new Vector3(5.25f, 5f, 3.5f);
        effect.LightSpecularColor = new Vector3(4f);
      }
    }
    this.mTomeClips = new AnimationClip[3];
    this.mTomeClips[0] = iTomeModel.AnimationClips["open"];
    this.mTomeClips[1] = iTomeModel.AnimationClips["close"];
    this.mTomeClips[2] = iTomeModel.AnimationClips["close"];
    for (int index3 = 0; index3 < iTomeModel.Model.Meshes.Count; ++index3)
    {
      ModelMesh mesh = iTomeModel.Model.Meshes[index3];
      for (int index4 = 0; index4 < mesh.Effects.Count; ++index4)
      {
        SkinnedModelBasicEffect effect = mesh.Effects[index4] as SkinnedModelBasicEffect;
        effect.SetTechnique(SkinnedModelBasicEffect.Technique.NonDeffered);
        effect.LightPosition = new Vector3(5f, 5f, 0.0f);
        effect.LightAmbientColor = new Vector3(0.4f);
        effect.LightDiffuseColor = new Vector3(0.7f);
        effect.LightSpecularColor = new Vector3(0.4f);
        effect.NormalPower = 1f;
        effect.LightPosition = new Vector3(5.25f, 5f, 3.5f);
        effect.LightSpecularColor = new Vector3(4f);
      }
    }
    this.mTomeController = new AnimationController();
    this.mTomeController.Skeleton = iTomeModel.SkeletonBones;
    this.mBackGroundPageControllerLeft = new AnimationController();
    this.mBackGroundPageControllerLeft.Skeleton = iBackGroundPageModel.SkeletonBones;
    this.mBackGroundPageControllerRight = new AnimationController();
    this.mBackGroundPageControllerRight.Skeleton = iBackGroundPageModel.SkeletonBones;
    for (int index5 = 0; index5 < iBackGroundPageModel.Model.Meshes.Count; ++index5)
    {
      ModelMesh mesh = iBackGroundPageModel.Model.Meshes[index5];
      for (int index6 = 0; index6 < mesh.Effects.Count; ++index6)
      {
        SkinnedModelBasicEffect effect = mesh.Effects[index6] as SkinnedModelBasicEffect;
        effect.SetTechnique(SkinnedModelBasicEffect.Technique.NonDeffered);
        effect.LightPosition = new Vector3(5f, 5f, 0.0f);
        effect.LightAmbientColor = new Vector3(0.4f);
        effect.LightDiffuseColor = new Vector3(0.7f);
        effect.LightSpecularColor = new Vector3(0.4f);
        effect.NormalPower = 1f;
        effect.LightPosition = new Vector3(5.25f, 5f, 3.5f);
        effect.LightSpecularColor = new Vector3(4f);
      }
    }
    for (int index7 = 0; index7 < skinnedModel2.Model.Meshes.Count; ++index7)
    {
      ModelMesh mesh = skinnedModel2.Model.Meshes[index7];
      for (int index8 = 0; index8 < mesh.Effects.Count; ++index8)
      {
        SkinnedModelBasicEffect effect = mesh.Effects[index8] as SkinnedModelBasicEffect;
        effect.SetTechnique(SkinnedModelBasicEffect.Technique.NonDeffered);
        effect.LightPosition = new Vector3(5f, 5f, 0.0f);
        effect.LightAmbientColor = new Vector3(0.4f);
        effect.LightDiffuseColor = new Vector3(0.7f);
        effect.LightSpecularColor = new Vector3(0.4f);
        effect.NormalPower = 1f;
        effect.LightPosition = new Vector3(5.25f, 5f, 3.5f);
        effect.LightSpecularColor = new Vector3(4f);
      }
    }
    this.mBackGroundClips = new AnimationClip[4];
    this.mBackGroundClips[0] = skinnedModel2.AnimationClips["open_left"];
    this.mBackGroundClips[1] = skinnedModel2.AnimationClips["open_right"];
    this.mBackGroundClips[2] = skinnedModel2.AnimationClips["close_left"];
    this.mBackGroundClips[3] = skinnedModel2.AnimationClips["close_right"];
    this.mRiffleController = new Tome.RiffleController(Tome.MaxNrOfPages, skinnedModel2);
    GraphicsDevice graphicsDevice = RenderManager.Instance.GraphicsDevice;
    RenderTarget2D iRenderTarget = new RenderTarget2D(graphicsDevice, iTexture1.Width, iTexture1.Height, 3, SurfaceFormat.Color);
    RenderTarget2D iShadowMap = new RenderTarget2D(graphicsDevice, 1024 /*0x0400*/, 1024 /*0x0400*/, 1, SurfaceFormat.Single);
    DepthStencilBuffer iShadowDepthBuffer = new DepthStencilBuffer(graphicsDevice, 1024 /*0x0400*/, 1024 /*0x0400*/, DepthFormat.Depth24);
    GUIBasicEffect iEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
    iEffect.SetScreenSize(iRenderTarget.Width, iRenderTarget.Height);
    iEffect.TextureEnabled = true;
    iEffect.ScaleToHDR = false;
    iEffect.Color = new Vector4(1f);
    VertexPositionTexture[] data = new VertexPositionTexture[4]
    {
      new VertexPositionTexture(new Vector3(0.0f, (float) iRenderTarget.Height, 0.0f), new Vector2(0.0f, 1f)),
      new VertexPositionTexture(new Vector3(0.0f), new Vector2(0.0f)),
      new VertexPositionTexture(new Vector3((float) iRenderTarget.Width, 0.0f, 0.0f), new Vector2(1f, 0.0f)),
      new VertexPositionTexture(new Vector3((float) iRenderTarget.Width, (float) iRenderTarget.Height, 0.0f), new Vector2(1f, 1f))
    };
    VertexBuffer iVertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.SizeInBytes * data.Length, BufferUsage.None);
    iVertexBuffer.SetData<VertexPositionTexture>(data);
    VertexDeclaration iVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
    this.mMenuStack = new SubMenu[32 /*0x20*/];
    this.mMenuRiffleStack = new Stack<int>(32 /*0x20*/);
    this.mInputLocked = false;
    this.CloseToMenu = false;
    this.mRenderData = new Tome.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new Tome.RenderData(iRenderTarget, iShadowMap, iShadowDepthBuffer, iTomeModel, skinnedModel2, iBackGroundPageModel, iBackgroundModel, iVertexBuffer, iTexture1, iVertexDeclaration, iEffect);
    this.TargetLightIntensity = 0.0f;
    this.mCurrentState = (Tome.TomeState) Tome.ClosedState.Instance;
    this.mCurrentState.OnEnter(this);
    SteamAPI.DlcInstalled += new Action<DlcInstalled>(this.SteamAPI_DLCInstalled);
    LanguageManager.Instance.LanguageChanged += new Action(this.LanguageChanged);
    DLC_StatusHelper.OnDynamicPromotionsLoaded -= new Action(Tome.ReLoadPromotionAfterDynamicLoadingComplete);
    DLC_StatusHelper.OnDynamicPromotionsLoaded += new Action(Tome.ReLoadPromotionAfterDynamicLoadingComplete);
    Singleton<GameSparksProperties>.Instance.OnPropertiesLoaded -= new Action(Tome.ReLoadPromotionAfterDynamicLoadingComplete);
    Singleton<GameSparksProperties>.Instance.OnPropertiesLoaded += new Action(Tome.ReLoadPromotionAfterDynamicLoadingComplete);
    DLC_StatusHelper instance = DLC_StatusHelper.Instance;
    Thread.Sleep(0);
    Tome.LoadNewPromotion();
    List<PromotionInfo> allOwned = DLC_StatusHelper.Instance.Splash_GetAllOwned();
    List<EventData> eventDataList = new List<EventData>();
    foreach (PromotionInfo promotionInfo in allOwned)
      eventDataList.Add(new EventData("dlc", new object[5]
      {
        (object) Singleton<GameSparksAccount>.Instance.Variant,
        (object) Singleton<PlayerSegmentManager>.Instance.CurrentSegment.ToSegmentString(),
        (object) (SteamApps.BIsDlcInstalled(promotionInfo.AppID) ? 1 : 0),
        (object) promotionInfo.Name,
        (object) promotionInfo.AppID
      }));
    Singleton<ParadoxServices>.Instance.TelemetryEvent(eventDataList.ToArray());
    Tome.ChangeResolution(GlobalSettings.Instance.Resolution);
    ResolutionMessageBox.Instance.Complete += new Action<ResolutionData>(Tome.ChangeResolution);
  }

  public static void ChangeResolution(ResolutionData iData)
  {
    if (Tome.sParadoxIcon == null)
      return;
    Tome.RefreshParadoxAccount(iData);
    Tome.RefreshUpsell(iData);
  }

  private static void RefreshParadoxAccount(ResolutionData iData)
  {
    float num = (float) iData.Height / PopupSystem.REFERENCE_SIZE.Y;
    Vector2 zero = Vector2.Zero;
    Tome.sParadoxIcon.Scale = num;
    Tome.sIndicatorBackground.Scale = num;
    Tome.sAccountLoginBtn.Scale = num;
    Tome.sAccountCreationBtn.Scale = num;
    Tome.sIndicatorBackground.TextPosition = Tome.ACCOUNT_BACKGROUND_SIZE * 0.5f * num;
    zero.X = (float) ((double) iData.Width - (double) Tome.ACCOUNT_BACKGROUND_SIZE.X * (double) num - 11.0);
    zero.Y = 20f;
    Tome.sIndicatorBackground.Position = zero;
    zero.X += (float) (((double) Tome.ACCOUNT_BACKGROUND_SIZE.X - (double) Tome.PARADOX_LOGO_SIZE.X) * 0.5) * num;
    zero.Y += 40f * num;
    Tome.sParadoxIcon.Position = zero;
    zero.X = (float) ((double) iData.Width - (double) Tome.ACCOUNT_BACKGROUND_SIZE.X * 0.5 * (double) num - 11.0);
    zero.Y = (float) (20.0 + (double) Tome.ACCOUNT_BACKGROUND_SIZE.Y * 0.5 * (double) num);
    zero.Y += 50f * num;
    Tome.sAccountLoginBtn.Position = zero;
    zero.Y += (float) (50.0 * (double) num + 25.0 * (double) num);
    Tome.sAccountCreationBtn.Position = zero;
  }

  private static void RefreshUpsell(ResolutionData iData)
  {
    float num = (float) iData.Height / PopupSystem.REFERENCE_SIZE.Y;
    Vector2 zero = Vector2.Zero with
    {
      X = (float) ((double) iData.Width - (double) Tome.TWITTER_SIZE.X * (double) num - 11.0),
      Y = (float) ((double) iData.Height - (double) Tome.TWITTER_SIZE.Y * (double) num - 11.0)
    };
    Tome.sTwitter.Position = zero;
    Tome.sTwitter.Scale = num;
    zero.X -= (float) ((double) Tome.FACEBOOK_SIZE.X * (double) num + 5.0);
    Tome.sFaceBook.Position = zero;
    Tome.sFaceBook.Scale = num;
    if (!Tome.sPromotionActive)
      return;
    zero.Y -= (float) ((double) Tome.sPromotionButton.Size.Y * (double) num + 11.0);
    zero.X = (float) iData.Width - (float) ((double) Tome.sPromotionButton.Size.X * (double) num + 11.0);
    Tome.sPromotionButton.Position = zero;
    Tome.sPromotionButton.Scale = num;
    if (!Tome.sShownSplashIsNew)
      return;
    zero.X += (float) ((double) Tome.sPromotionButton.Size.X * (double) num - ((double) Tome.SPLASH_NEW_SIZE.X * (double) num + 3.0));
    zero.Y += (float) ((double) Tome.sPromotionButton.Size.Y * (double) num - ((double) Tome.SPLASH_NEW_SIZE.Y * (double) num + 3.0));
    Tome.sSplashNew.Position = zero;
    Tome.sSplashNew.Scale = num;
  }

  public static void ReLoadPromotionAfterDynamicLoadingComplete()
  {
    Tome.PromotionActive = Singleton<GameSparksProperties>.Instance.GetProperty<bool>("ABTestAdLevel", "Ads");
    if (Tome.sPromotionActive)
      Tome.sPromotionButton = DLC_StatusHelper.Instance.Splash_GetMenuItem(Vector2.Zero, Tome.PROMOTION_BUTTON_POSITION, out Tome.sShownSplashIsNew);
    Tome.RefreshUpsell(GlobalSettings.Instance.Resolution);
  }

  public static void LoadNewPromotion()
  {
    if (!Tome.sPromotionActive)
      return;
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
      flag = true;
    if (!flag)
      return;
    Tome.sPromotionButton = instance.Splash_GetMenuItem(Vector2.Zero, Tome.PROMOTION_BUTTON_POSITION, out Tome.sShownSplashIsNew);
    Tome.sPromotionButton.Alpha = Tome.sIndicatorBackground.Alpha;
    Tome.RefreshUpsell(GlobalSettings.Instance.Resolution);
  }

  public Tome.TomeState CurrentState => this.mCurrentState;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    Tome.RenderData iObject = this.mRenderData[(int) iDataChannel];
    Vector3 vector3_1 = new Vector3(0.3f, 0.2f, 0.2f);
    Vector3 vector3_2 = new Vector3(0.7f, 0.75f, 0.75f);
    Vector3 vector3_3 = new Vector3(1.3f, 1f, 1f);
    this.mIntroTime += iDeltaTime;
    if (this.mHurryAndOpen && this.mCameraAnimation.AnimationClip == this.mCameraAnimations[0] && this.mCameraAnimation.HasFinished)
      this.ChangeState((Tome.TomeState) Tome.OpeningState.Instance);
    if (this.mCameraAnimation.AnimationClip == this.mCameraAnimations[0])
    {
      if ((double) this.mIntroTime > 1.5 || (double) this.mCameraAnimation.ClipSpeed > 1.0)
      {
        this.mTargetLightIntensity = 1f;
      }
      else
      {
        if (!this.mCameraAnimation.CrossFadeEnabled && !this.mCameraAnimation.IsPlaying)
          this.PlayCameraAnimation(Tome.CameraAnimation.Wake_Up);
        this.mTargetLightIntensity = 0.0f;
      }
    }
    this.mLightVariationSpeed += (float) (((double) this.mTargetLightVariationSpeed - (double) this.mLightVariationSpeed) * (double) iDeltaTime * 5.0);
    this.mLightVariationAmount += (float) (((double) this.mTargetLightVariationAmount - (double) this.mLightVariationAmount) * (double) iDeltaTime * 5.0);
    this.mLightIntensity += (this.mTargetLightIntensity - this.mLightIntensity) * iDeltaTime * this.mLightVariationSpeed;
    this.mLightVariationPosition += (float) ((Tome.sRandom.NextDouble() - 0.5) * 2.0) * iDeltaTime * this.mLightVariationSpeed;
    this.mLightVariationPosition = MathHelper.Clamp(this.mLightVariationPosition, -1f, 1f);
    float scaleFactor = (this.mLightIntensity + this.mLightVariationPosition * this.mLightVariationAmount) * 100f;
    Vector3.Multiply(ref vector3_1, scaleFactor, out iObject.LightAmbient);
    Vector3.Multiply(ref vector3_2, scaleFactor, out iObject.LightDiffuse);
    Vector3.Multiply(ref vector3_3, scaleFactor, out iObject.LightSpecular);
    if (this.mCurrentState == Tome.ClosedState.Instance && this.currentExtraButtonIndex != -1)
    {
      iObject.LightDiffuse *= 0.5f;
      iObject.LightSpecular *= 0.8f;
    }
    iObject.State = this.mCurrentState;
    Tome.mCurrentTime += iDeltaTime;
    if ((double) Tome.mCurrentTime > 6.2831854820251465)
      Tome.mCurrentTime = 0.0f;
    if (this.mMenuStackPosition >= 0)
    {
      SubMenu mMenu = this.mMenuStack[this.mMenuStackPosition];
      iObject.CurrentMenu = mMenu;
      mMenu.Update(iDataChannel, iDeltaTime);
    }
    Matrix result;
    Matrix.Multiply(ref this.mCameraBindPose, ref this.mCameraAnimation.SkinnedBoneTransforms[this.mCameraBone], out result);
    Tome.mCameraPosition = result.Translation;
    Matrix identity = Matrix.Identity;
    this.mCameraAnimation.Update(iDeltaTime, ref identity, true);
    Point screenSize = RenderManager.Instance.ScreenSize;
    Matrix.Invert(ref result, out iObject.View);
    Matrix.CreatePerspectiveFieldOfView(0.75f, (float) screenSize.X / (float) screenSize.Y, 2f, 200f, out iObject.Projection);
    Matrix.Multiply(ref iObject.View, ref iObject.Projection, out iObject.ViewProjection);
    Tome.mViewProjection = iObject.ViewProjection;
    this.mRiffleController.ArrayCopyTo(ref iObject.mPageBones);
    this.mCurrentState.Update(iDeltaTime, iDataChannel, this);
    if ((double) this.mNewContentUpdateTimer > 600.0)
    {
      this.mNewContentUpdateTimer -= 600f;
      WebParser.CheckSteamDLCs(new Action<bool>(this.NewDLCCheckCallback));
    }
    this.mNewContentUpdateTimer += iDeltaTime;
    iObject.State = this.mCurrentState;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
    if (Tome.sFadeInAccountIndicator && (this.mRiffleController.HasFinished || this.mTomeController.IsPlaying))
    {
      float num = Tome.sIndicatorBackground.Alpha + iDeltaTime / 0.25f;
      if ((double) num >= 1.0)
        Tome.sFadeInAccountIndicator = false;
      Tome.sIndicatorBackground.Alpha = num;
      Tome.sParadoxIcon.Alpha = num;
      Tome.sIndicatorBackground.Alpha = num;
      Tome.sAccountLoginBtn.Alpha = num;
      Tome.sAccountCreationBtn.Alpha = num;
      Tome.sFaceBook.Alpha = num;
      Tome.sTwitter.Alpha = num;
      if (Tome.sPromotionButton != null)
        Tome.sPromotionButton.Alpha = num;
      Tome.sSplashNew.Alpha = num;
    }
    if (!Tome.sFadeInAccountIndicator && (!this.mRiffleController.HasFinished || this.mTomeController.IsPlaying) && Singleton<ParadoxAccount>.Instance.PendingErrorCode != ParadoxAccount.ErrorCode.None)
      ParadoxPopupUtils.ShowErrorPopup(Singleton<ParadoxAccount>.Instance.ConsumePendingErrorCode());
    if (!this.mRiffleController.HasFinished || this.mTomeController.IsPlaying)
      this.mInputLocked = true;
    else
      this.mInputLocked = false;
  }

  private void LanguageChanged() => Tome.sVersionText.MarkAsDirty();

  internal void NewDLCCheckCallback(bool iNewContent)
  {
  }

  private void SteamAPI_DLCInstalled(DlcInstalled iDLC) => this.mNewContentUpdateTimer = 600f;

  public void ChangeState(Tome.TomeState iState)
  {
    this.mCurrentState.OnExit(this);
    this.mCurrentState = iState;
    this.mCurrentState.OnEnter(this);
  }

  public SubMenu CurrentMenu
  {
    get => this.mMenuStackPosition < 0 ? (SubMenu) null : this.mMenuStack[this.mMenuStackPosition];
  }

  public SubMenu PreviousMenu
  {
    get
    {
      return this.mMenuStackPosition <= 0 ? (SubMenu) null : this.mMenuStack[this.mMenuStackPosition - 1];
    }
  }

  public static void SetIndicatorState(Tome.AccountIndicatorState iState)
  {
    switch (iState)
    {
      case Tome.AccountIndicatorState.LoggedIn:
        Tome.sAccountCreationBtn.Enabled = false;
        Tome.sAccountLoginBtn.Enabled = false;
        Tome.sIndicatorBackground.SetText(LanguageManager.Instance.GetString(Tome.LOC_LOGGEDIN));
        break;
      case Tome.AccountIndicatorState.LoggedOut:
        Tome.sAccountCreationBtn.Enabled = true;
        Tome.sAccountLoginBtn.Enabled = true;
        Tome.sIndicatorBackground.SetText(LanguageManager.Instance.GetString(Tome.LOC_LOGGEDOUT));
        break;
    }
  }

  public void PushMenuInstant(SubMenu iMenu, int iRiffles)
  {
    this.mMenuStack[this.mMenuStackPosition].OnExit();
    this.mMenuStack[this.mMenuStackPosition + 1] = iMenu;
    ++this.mMenuStackPosition;
    this.mMenuStack[this.mMenuStackPosition].OnEnter();
    this.mMenuRiffleStack.Push(iRiffles);
  }

  public void PushMenu(SubMenu iMenu, int iRiffles)
  {
    if (this.mCurrentState != Tome.OpenState.Instance)
      return;
    this.mMenuStack[this.mMenuStackPosition].OnExit();
    this.mMenuStack[this.mMenuStackPosition + 1] = iMenu;
    ++this.mMenuStackPosition;
    this.mMenuStack[this.mMenuStackPosition].OnEnter();
    this.mMenuRiffleStack.Push(iRiffles);
    if (iRiffles <= 0)
      return;
    this.ChangeState((Tome.TomeState) Tome.RiffleForwardState.Instance);
  }

  public void PopMenuInstant()
  {
    if (this.mMenuStackPosition <= 0)
      return;
    this.currentExtraButtonIndex = -1;
    this.UnselectAllExtraButtons();
    this.mMenuStack[this.mMenuStackPosition].OnExit();
    --this.mMenuStackPosition;
    this.mMenuStack[this.mMenuStackPosition].OnEnter();
    this.mMenuRiffleStack.Pop();
  }

  public void PopPreviousMenu()
  {
    this.currentExtraButtonIndex = -1;
    this.UnselectAllExtraButtons();
    this.mMenuStack[this.mMenuStackPosition - 1] = this.mMenuStack[this.mMenuStackPosition];
    --this.mMenuStackPosition;
    this.mMenuStack[this.mMenuStackPosition + 1] = (SubMenu) null;
    this.mMenuRiffleStack.Pop();
  }

  public void PopMenu()
  {
    if (this.mCurrentState != Tome.OpenState.Instance)
      return;
    this.currentExtraButtonIndex = -1;
    this.UnselectAllExtraButtons();
    if (this.mMenuStackPosition > 0 && GameStateManager.Instance.CurrentState is MenuState)
    {
      this.mMenuStack[this.mMenuStackPosition].OnExit();
      --this.mMenuStackPosition;
      this.mMenuStack[this.mMenuStackPosition].OnEnter();
      this.ChangeState((Tome.TomeState) Tome.RiffleBackwardState.Instance);
      this.mMenuRiffleStack.Pop();
      if (Singleton<ParadoxAccount>.Instance.IsBusy)
        return;
      Tome.SetIndicatorState(Singleton<ParadoxAccount>.Instance.IsLoggedFull ? Tome.AccountIndicatorState.LoggedIn : Tome.AccountIndicatorState.LoggedOut);
    }
    else if (GameStateManager.Instance.CurrentState is PlayState)
    {
      this.mMenuStack[this.mMenuStackPosition].OnExit();
      --this.mMenuStackPosition;
      this.mMenuStack[this.mMenuStackPosition].OnEnter();
      this.ChangeState((Tome.TomeState) Tome.RiffleBackwardState.Instance);
      this.mMenuRiffleStack.Pop();
    }
    else
    {
      this.ChangeState((Tome.TomeState) Tome.ClosingState.Instance);
      this.mMenuStack[this.mMenuStackPosition].OnExit();
    }
  }

  public void Riffle(PlaybackMode iMode, int iPages)
  {
    this.mMenuRiffleStack.Push(iPages);
    if (iMode == PlaybackMode.Forward)
      this.ChangeState((Tome.TomeState) Tome.RiffleForwardState.Instance);
    else
      this.ChangeState((Tome.TomeState) Tome.RiffleBackwardState.Instance);
    this.mMenuRiffleStack.Pop();
  }

  public bool CameraMoving => this.mCameraAnimation.IsPlaying;

  public void SetCameraAnimation(Tome.CameraAnimation iCameraAnimation)
  {
    this.mCameraAnimation.StartClip(this.mCameraAnimations[(int) iCameraAnimation], false);
    this.mCameraAnimation.Stop();
  }

  public void PlayCameraAnimation(Tome.CameraAnimation iCameraAnimation)
  {
    this.mCameraAnimation.StartClip(this.mCameraAnimations[(int) iCameraAnimation], false);
  }

  public void CrossfadeCameraAnimation(Tome.CameraAnimation iCameraAnimation, float iTime)
  {
    this.mCameraAnimation.CrossFade(this.mCameraAnimations[(int) iCameraAnimation], iTime, false);
  }

  public void CloseBack()
  {
    this.currentExtraButtonIndex = -1;
    this.ChangeState((Tome.TomeState) Tome.CloseToBack.Instance);
  }

  public void CloseTome()
  {
    this.currentExtraButtonIndex = -1;
    this.ChangeState((Tome.TomeState) Tome.ClosingState.Instance);
  }

  public void CloseTomeInstant()
  {
    if (this.mInputLocked)
      return;
    this.currentExtraButtonIndex = -1;
    this.ChangeState((Tome.TomeState) Tome.ClosedState.Instance);
  }

  public bool MenuOverExtraButtons => this.currentExtraButtonIndex != -1;

  public void UnselectAllExtraButtons()
  {
    if (Tome.sPromotionButton != null)
      Tome.sPromotionButton.Selected = false;
    Tome.sFaceBook.Selected = Tome.sTwitter.Selected = false;
    Tome.sAccountCreationBtn.Selected = Tome.sAccountLoginBtn.Selected = false;
  }

  private void NavigateExtraButtons(
    int parentMenuTopIndex,
    int parentMenuBottomIndex,
    int parentMenuLeftIndex,
    int parentMenuRightIndex,
    ref int parentMenuCurrIndex,
    ControllerDirection dir)
  {
    switch (dir)
    {
      case ControllerDirection.Right:
        if (this.currentExtraButtonIndex == -1)
        {
          this.currentExtraButtonIndex = Singleton<ParadoxAccount>.Instance.IsLoggedFull ? 3 : 0;
          break;
        }
        if (this.currentExtraButtonIndex == 3)
        {
          this.currentExtraButtonIndex = 4;
          break;
        }
        this.currentExtraButtonIndex = -1;
        parentMenuCurrIndex = parentMenuLeftIndex;
        break;
      case ControllerDirection.Up:
        if (this.currentExtraButtonIndex == -1)
        {
          this.currentExtraButtonIndex = Singleton<ParadoxAccount>.Instance.IsLoggedFull ? (Tome.PromotionActive ? 2 : 3) : 0;
          break;
        }
        if (this.currentExtraButtonIndex == 2)
        {
          this.currentExtraButtonIndex = Singleton<ParadoxAccount>.Instance.IsLoggedFull ? -1 : 1;
          break;
        }
        if (this.currentExtraButtonIndex == 4)
        {
          this.currentExtraButtonIndex = Tome.PromotionActive ? 2 : (Singleton<ParadoxAccount>.Instance.IsLoggedFull ? -1 : 1);
          break;
        }
        if (this.currentExtraButtonIndex >= 0 && this.currentExtraButtonIndex <= 3)
        {
          --this.currentExtraButtonIndex;
          if (this.currentExtraButtonIndex == 2 && !Tome.PromotionActive)
            --this.currentExtraButtonIndex;
          if (this.currentExtraButtonIndex == 1 && Singleton<ParadoxAccount>.Instance.IsLoggedFull)
          {
            this.currentExtraButtonIndex = -1;
            parentMenuCurrIndex = parentMenuBottomIndex;
            break;
          }
          break;
        }
        this.currentExtraButtonIndex = -1;
        parentMenuCurrIndex = parentMenuBottomIndex;
        break;
      case ControllerDirection.Left:
        if (this.currentExtraButtonIndex == -1)
        {
          this.currentExtraButtonIndex = Singleton<ParadoxAccount>.Instance.IsLoggedFull ? (Tome.PromotionActive ? 2 : 3) : 0;
          break;
        }
        if (this.currentExtraButtonIndex == 4)
        {
          this.currentExtraButtonIndex = 3;
          break;
        }
        this.currentExtraButtonIndex = -1;
        parentMenuCurrIndex = parentMenuRightIndex;
        break;
      case ControllerDirection.Down:
        if (this.currentExtraButtonIndex == -1)
        {
          this.currentExtraButtonIndex = Singleton<ParadoxAccount>.Instance.IsLoggedFull ? (Tome.PromotionActive ? 2 : 3) : 0;
          break;
        }
        if (this.currentExtraButtonIndex >= 0 && this.currentExtraButtonIndex <= 2)
        {
          ++this.currentExtraButtonIndex;
          if (this.currentExtraButtonIndex == 2 && !Tome.PromotionActive)
          {
            ++this.currentExtraButtonIndex;
            break;
          }
          break;
        }
        this.currentExtraButtonIndex = -1;
        parentMenuCurrIndex = parentMenuTopIndex;
        break;
    }
    this.UnselectAllExtraButtons();
    if (this.currentExtraButtonIndex == 1)
      Tome.sAccountCreationBtn.Selected = true;
    else if (this.currentExtraButtonIndex == 0)
      Tome.sAccountLoginBtn.Selected = true;
    else if (this.currentExtraButtonIndex == 2)
    {
      if (Tome.sPromotionButton == null)
        return;
      Tome.sPromotionButton.Selected = true;
    }
    else if (this.currentExtraButtonIndex == 3)
    {
      Tome.sFaceBook.Selected = true;
    }
    else
    {
      if (this.currentExtraButtonIndex != 4)
        return;
      Tome.sTwitter.Selected = true;
    }
  }

  private bool ControllerA_OnExtraButton()
  {
    if (this.currentExtraButtonIndex <= -1 || this.currentExtraButtonIndex > 2)
      return false;
    switch (this.currentExtraButtonIndex)
    {
      case 0:
        this.PushMenu((SubMenu) SubMenuAccountLogin.Instance, 2);
        break;
      case 1:
        this.PushMenu((SubMenu) SubMenuAccountCreate.Instance, 1);
        break;
      case 2:
        if (Tome.PromotionActive)
        {
          this.OpenURL("http://store.steampowered.com/dlc/42910", true);
          break;
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
      this.currentExtraButtonIndex = -1;
    return true;
  }

  public void ControllerMovement(Controller iSender, ControllerDirection iDirection)
  {
    if (DialogManager.Instance.MessageBoxActive)
      return;
    if (this.mCurrentState == Tome.ClosedState.Instance)
    {
      int parentMenuCurrIndex = -1;
      this.NavigateExtraButtons(-1, -1, -1, -1, ref parentMenuCurrIndex, iDirection);
    }
    else
    {
      int selectedPosition = this.mMenuStack[this.mMenuStackPosition].CurrentlySelectedPosition;
      int num1 = this.mMenuStack[this.mMenuStackPosition].NumItems - 1;
      int parentMenuTopIndex = 0;
      bool flag1 = false;
      if (!(this.CurrentMenu is SubMenuCutscene) && !(this.CurrentMenu is SubMenuIntro))
      {
        parentMenuTopIndex = 0;
        if (this.CurrentMenu is SubMenuCampaignSelect_SaveSlotSelect)
          parentMenuTopIndex = 3;
        int num2 = 1;
        bool flag2 = !(this.CurrentMenu is SubMenuMain) ? !(this.CurrentMenu is SubMenuOnline) && !(this.CurrentMenu is SubMenuLeaderboards) && !(this.CurrentMenu is SubMenuCharacterSelect) && selectedPosition == parentMenuTopIndex && iDirection == ControllerDirection.Up : (selectedPosition == parentMenuTopIndex || selectedPosition == num2) && iDirection == ControllerDirection.Up;
        if (this.CurrentMenu is SubMenuCharacterSelect)
        {
          parentMenuTopIndex = 2;
          num1 = 4;
        }
        else if (this.CurrentMenu is SubMenuOnline)
          num1 = 16 /*0x10*/;
        else if (this.CurrentMenu is SubMenuLeaderboards)
          num1 = 2;
        bool flag3 = selectedPosition == num1 && (iDirection == ControllerDirection.Down || iDirection == ControllerDirection.Right);
        if (this.CurrentMenu is SubMenuLeaderboards)
          flag3 = selectedPosition == num1 && iDirection == ControllerDirection.Down;
        flag1 = this.MenuOverExtraButtons || flag3 || flag2;
        if ((this.CurrentMenu is SubMenuOptionsKeyboard || this.CurrentMenu is SubMenuOptionsGamepad) && iDirection == ControllerDirection.Right)
          flag1 = true;
      }
      if (flag1)
      {
        Magicka.Game.Instance.IsMouseVisible = false;
        AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_scroll".GetHashCodeCustom());
        int parentMenuCurrIndex = selectedPosition;
        if (!this.MenuOverExtraButtons)
          this.mMenuStack[this.mMenuStackPosition].UnselectAll();
        if (this.CurrentMenu is SubMenuOptionsKeyboard || this.CurrentMenu is SubMenuOptionsGamepad)
          this.NavigateExtraButtons(selectedPosition, selectedPosition, selectedPosition, selectedPosition, ref parentMenuCurrIndex, iDirection);
        else if (this.CurrentMenu is SubMenuOnline || this.CurrentMenu is SubMenuOptions || this.CurrentMenu is SubMenuLeaderboards)
          this.NavigateExtraButtons(num1, num1, num1, num1, ref parentMenuCurrIndex, iDirection);
        else
          this.NavigateExtraButtons(parentMenuTopIndex, num1, num1, num1, ref parentMenuCurrIndex, iDirection);
        if (parentMenuCurrIndex == selectedPosition)
          return;
        this.mMenuStack[this.mMenuStackPosition].ForceSetAndSelectCurrent(parentMenuCurrIndex);
      }
      else
      {
        this.UnselectAllExtraButtons();
        switch (iDirection)
        {
          case ControllerDirection.Right:
            Magicka.Game.Instance.IsMouseVisible = false;
            AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_increase".GetHashCodeCustom());
            this.mMenuStack[this.mMenuStackPosition].ControllerRight(iSender);
            break;
          case ControllerDirection.Up:
            Magicka.Game.Instance.IsMouseVisible = false;
            AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_scroll".GetHashCodeCustom());
            this.mMenuStack[this.mMenuStackPosition].ControllerUp(iSender);
            break;
          case ControllerDirection.Left:
            Magicka.Game.Instance.IsMouseVisible = false;
            AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_decrease".GetHashCodeCustom());
            this.mMenuStack[this.mMenuStackPosition].ControllerLeft(iSender);
            break;
          case ControllerDirection.Down:
            Magicka.Game.Instance.IsMouseVisible = false;
            AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_scroll".GetHashCodeCustom());
            this.mMenuStack[this.mMenuStackPosition].ControllerDown(iSender);
            break;
        }
      }
    }
  }

  public void ControllerEvent(Controller iSender, KeyboardState iOldState, KeyboardState iNewState)
  {
    if (this.mMenuStackPosition < 0 || this.mMenuStackPosition >= this.mMenuStack.Length)
      return;
    this.mMenuStack[this.mMenuStackPosition].ControllerEvent(iSender, iOldState, iNewState);
  }

  public void ControllerA(Controller iSender)
  {
    if (this.mInputLocked | DialogManager.Instance.MessageBoxActive)
      return;
    if (this.mCurrentState == Tome.ClosedBack.Instance)
      this.ChangeState((Tome.TomeState) Tome.BackToFront.Instance);
    else if (this.mCurrentState == Tome.ClosedState.Instance)
    {
      if ((double) this.mIntroTime <= 1.0 || this.ControllerA_OnExtraButton() || this.mHurryAndOpen)
        return;
      this.mHurryAndOpen = true;
      this.mCameraAnimation.ClipSpeed *= 6f;
    }
    else
    {
      AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_select".GetHashCodeCustom());
      if (this.ControllerA_OnExtraButton())
        return;
      this.mMenuStack[this.mMenuStackPosition].ControllerA(iSender);
    }
  }

  public void ControllerB(Controller iSender)
  {
    if (this.mInputLocked | DialogManager.Instance.MessageBoxActive)
      return;
    if (this.mMenuStackPosition > 0)
      this.mMenuStack[this.mMenuStackPosition].ControllerB(iSender);
    else
      SubMenuMain.Instance.ShowRUSure();
  }

  public void ControllerX(Controller iSender)
  {
    if (this.mInputLocked || DialogManager.Instance.MessageBoxActive || this.mMenuStackPosition <= 0)
      return;
    this.mMenuStack[this.mMenuStackPosition].ControllerX(iSender);
  }

  public void ControllerY(Controller iSender)
  {
    if (this.mInputLocked || DialogManager.Instance.MessageBoxActive || this.mMenuStackPosition <= 0)
      return;
    this.mMenuStack[this.mMenuStackPosition].ControllerY(iSender);
  }

  public float TargetLightVariationAmount
  {
    get => this.mTargetLightVariationAmount;
    set => this.mTargetLightVariationAmount = value;
  }

  public float TargetLightVariationSpeed
  {
    get => this.mTargetLightVariationSpeed;
    set => this.mTargetLightVariationSpeed = value;
  }

  public float TargetLightIntensity
  {
    get => this.mTargetLightIntensity;
    set => this.mTargetLightIntensity = value;
  }

  public float LightIntensity
  {
    get => this.mLightIntensity;
    set => this.mTargetLightIntensity = this.mLightIntensity = value;
  }

  internal void ControllerMouseMove(
    KeyboardMouseController keyboardMouseController,
    Point screenSize,
    MouseState newMouseState,
    MouseState mOldMouseState)
  {
    if (Tome.sFaceBook.InsideBounds(newMouseState))
    {
      Tome.sFaceBook.Selected = true;
      Tome.sTwitter.Selected = false;
      if (Tome.sPromotionButton == null)
        return;
      Tome.sPromotionButton.Selected = false;
    }
    else if (Tome.sTwitter.InsideBounds(newMouseState))
    {
      Tome.sFaceBook.Selected = false;
      Tome.sTwitter.Selected = true;
      if (Tome.sPromotionButton == null)
        return;
      Tome.sPromotionButton.Selected = false;
    }
    else if (Tome.sPromotionButton != null && Tome.sPromotionButton.InsideBounds(newMouseState))
    {
      Tome.sFaceBook.Selected = false;
      Tome.sTwitter.Selected = false;
      if (Tome.sPromotionButton == null)
        return;
      Tome.sPromotionButton.Selected = true;
    }
    else if (Tome.sAccountCreationBtn.InsideBounds(newMouseState) && Tome.sAccountCreationBtn.Enabled)
    {
      Tome.sAccountCreationBtn.Selected = true;
      Tome.sAccountLoginBtn.Selected = false;
    }
    else if (Tome.sAccountLoginBtn.InsideBounds(newMouseState) && Tome.sAccountLoginBtn.Enabled)
    {
      Tome.sAccountLoginBtn.Selected = true;
      Tome.sAccountCreationBtn.Selected = false;
    }
    else
    {
      Tome.sFaceBook.Selected = false;
      Tome.sTwitter.Selected = false;
      if (Tome.sPromotionButton != null)
        Tome.sPromotionButton.Selected = false;
      Tome.sAccountCreationBtn.Selected = false;
      Tome.sAccountLoginBtn.Selected = false;
      if (this.CurrentMenu == null || this.CurrentState is Tome.ClosedState || this.CurrentState is Tome.ClosedBack || this.CurrentState is Tome.OpeningState)
        return;
      this.CurrentMenu.ControllerMouseMove((Controller) keyboardMouseController, screenSize, newMouseState, mOldMouseState);
    }
  }

  private void OpenURL(string url) => this.OpenURL(url, false);

  private void OpenURL(string url, bool viaSteam)
  {
    Magicka.Game.Instance.Form.BeginInvoke((Delegate) (() =>
    {
      if (viaSteam)
      {
        SteamUtils.ActivateGameOverlayToWebPage(url);
      }
      else
      {
        foreach (Form openForm in (ReadOnlyCollectionBase) Application.OpenForms)
          openForm.WindowState = FormWindowState.Minimized;
        Process.Start(url);
      }
    }));
  }

  internal void ControllerMouseAction(
    KeyboardMouseController keyboardMouseController,
    Point screenSize,
    MouseState newMouseState,
    MouseState mOldMouseState)
  {
    bool flag1 = newMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && mOldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
    bool flag2 = !(this.CurrentMenu is SubMenuCutscene) && !(this.CurrentMenu is SubMenuIntro);
    if (Tome.sFaceBook.InsideBounds(newMouseState) && flag2)
      this.OpenURL("http://www.facebook.com/MagickaGame/", true);
    else if (Tome.sTwitter.InsideBounds(newMouseState) && flag2)
      this.OpenURL("http://www.twitter.com/MagickaGame/", true);
    else if (Tome.sPromotionButton != null && Tome.sPromotionButton.InsideBounds(newMouseState) && flag2)
    {
      string promotionNoneStoreUrl = DLC_StatusHelper.Instance.CurrentPromotion_NoneStoreURL;
      string str1 = DLC_StatusHelper.Instance.CurrentPromotion_AppID.ToString();
      TelemetryUtils.SendDLCPromotionClicked();
      Singleton<GameSparksServices>.Instance.LogEvent("AdClicked");
      if (!string.IsNullOrEmpty(promotionNoneStoreUrl))
      {
        this.OpenURL(promotionNoneStoreUrl);
      }
      else
      {
        string str2 = "http://store.steampowered.com/";
        this.OpenURL(!DLC_StatusHelper.Instance.CurrentPromotion_IsDynamicallyLoaded ? (!DLC_StatusHelper.Instance.HasPromotion ? str2 + "dlc/42910" : $"{str2}app/{str1}") : $"{str2}app/{str1}", true);
      }
    }
    else if (Tome.sAccountCreationBtn.InsideBounds(newMouseState) && Tome.sAccountCreationBtn.Enabled && !(Tome.Instance.CurrentMenu is SubMenuAccountCreate) && flag2)
    {
      if (this.PreviousMenu is SubMenuAccountCreate)
        this.PopMenu();
      else
        this.PushMenu((SubMenu) SubMenuAccountCreate.Instance, 2);
    }
    else if (Tome.sAccountLoginBtn.InsideBounds(newMouseState) && Tome.sAccountLoginBtn.Enabled && flag1 && !(Tome.Instance.CurrentMenu is SubMenuAccountLogin) && flag2)
    {
      if (this.PreviousMenu is SubMenuAccountLogin)
        this.PopMenu();
      else
        this.PushMenu((SubMenu) SubMenuAccountLogin.Instance, 1);
    }
    else if (this.CurrentMenu == null)
    {
      this.ControllerA((Controller) keyboardMouseController);
    }
    else
    {
      if (this.CurrentState is Tome.ClosedState || this.CurrentState is Tome.ClosedBack || this.CurrentState is Tome.OpeningState)
        return;
      this.CurrentMenu.ControllerMouseAction((Controller) keyboardMouseController, screenSize, newMouseState, mOldMouseState);
    }
  }

  internal void PageToScreen(bool iRight, ref Vector2 iPagePos, out Vector2 oScreenPos)
  {
    Vector3 point;
    (!iRight ? Tome.LEFT_TRIS[0] : Tome.RIGHT_TRIS[0]).GetPoint((float) (1.0 - (double) iPagePos.X / (double) Tome.PAGERIGHTSHEET.Width), iPagePos.Y / (float) Tome.PAGERIGHTSHEET.Height, out point);
    Point screenSize = RenderManager.Instance.ScreenSize;
    Vector4 result;
    Vector4.Transform(ref point, ref Tome.mViewProjection, out result);
    oScreenPos = new Vector2();
    oScreenPos.X = result.X / result.W;
    oScreenPos.Y = result.Y / result.W;
    oScreenPos.X *= 0.5f;
    oScreenPos.Y *= -0.5f;
    oScreenPos.X += 0.5f;
    oScreenPos.Y += 0.5f;
    oScreenPos.X *= (float) screenSize.X;
    oScreenPos.Y *= (float) screenSize.Y;
  }

  public static void OnLoggedOut()
  {
    Tome.sAccountLoginBtn.Enabled = true;
    Tome.sAccountCreationBtn.Enabled = true;
  }

  public static void OnLoggedIn(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode)
  {
    if (iSuccess)
    {
      bool flag = Tome.Instance.CurrentMenu is SubMenuAccountCreate;
      if (Tome.Instance.CurrentMenu is SubMenuAccountCreate || Tome.Instance.CurrentMenu is SubMenuAccountLogin)
      {
        while (Tome.Instance.PreviousMenu is SubMenuAccountCreate || Tome.Instance.PreviousMenu is SubMenuAccountLogin)
          Tome.Instance.PopPreviousMenu();
        Tome.Instance.PopMenu();
      }
      if (!flag)
        return;
      Tome.Instance.PushMenu((SubMenu) SubMenuOptionsParadoxAccount.Instance, 1);
      Tome.sSteamLinkPopup.SetTitle(Tome.LOC_NOTICE, Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR);
      Tome.sSteamLinkPopup.SetMessage(Tome.LOC_STEAMLINK, Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR);
      Tome.sSteamLinkPopup.SetButtonType(ButtonConfig.OkCancel);
      MenuMessagePopup sSteamLinkPopup = Tome.sSteamLinkPopup;
      sSteamLinkPopup.OnPositiveClick = sSteamLinkPopup.OnPositiveClick + (Action) (() => Singleton<ParadoxAccount>.Instance.LinkSteam(new ParadoxAccountSequence.ExecutionDoneDelegate(Tome.OnSteamLinkCallback)));
      Singleton<PopupSystem>.Instance.AddPopupToQueue((MenuBasePopup) Tome.sSteamLinkPopup);
    }
    else
      ParadoxPopupUtils.ShowErrorPopup(iErrorCode);
  }

  private static void OnSteamLinkCallback(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode)
  {
    if (iSuccess)
      return;
    ParadoxPopupUtils.ShowErrorPopup(iErrorCode);
    Tome.Instance.PushMenu((SubMenu) SubMenuOptionsParadoxAccount.Instance, 1);
  }

  public abstract class TomeState
  {
    protected Matrix identity = Matrix.Identity;
    protected Viewport mPreviousLeft = Tome.PAGELEFTSHEET;
    protected Viewport mPreviousRight = Tome.PAGERIGHTSHEET;
    protected Viewport mCurrentLeft = Tome.TOMELEFTSHEET;
    protected Viewport mCurrentRight = Tome.TOMERIGHTSHEET;

    public virtual void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
    {
    }

    public virtual void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
    {
      if (iOwner.mMenuStackPosition < 0)
        return;
      iMenu.DrawNewAndOld(iOwner.mMenuStackPosition > 0 ? iOwner.mMenuStack[iOwner.mMenuStackPosition - 1] : (SubMenu) null, this.mCurrentLeft, this.mCurrentRight, this.mPreviousLeft, this.mPreviousRight);
    }

    public virtual void OnEnter(Tome iOwner)
    {
    }

    public virtual void OnExit(Tome iOwner)
    {
    }
  }

  public class ClosedState : Tome.TomeState
  {
    private static Tome.ClosedState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Tome.ClosedState Instance
    {
      get
      {
        if (Tome.ClosedState.mSingelton == null)
        {
          lock (Tome.ClosedState.mSingeltonLock)
          {
            if (Tome.ClosedState.mSingelton == null)
              Tome.ClosedState.mSingelton = new Tome.ClosedState();
          }
        }
        return Tome.ClosedState.mSingelton;
      }
    }

    private ClosedState()
    {
    }

    public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
    {
    }

    public override void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
    {
    }

    public override void OnEnter(Tome iOwner)
    {
      iOwner.mTomeController.StartClip(iOwner.mTomeClips[1], false);
      iOwner.mTomeController.Update(iOwner.mTomeClips[1].Duration - 1f / 1000f, ref this.identity, true);
      iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[2], false);
      iOwner.mBackGroundPageControllerLeft.Update(iOwner.mBackGroundClips[2].Duration - 1f / 1000f, ref this.identity, true);
      iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[3], false);
      iOwner.mBackGroundPageControllerRight.Update(iOwner.mBackGroundClips[3].Duration - 1f / 1000f, ref this.identity, true);
      iOwner.mTomeController.Update(0.01f, ref this.identity, true);
      iOwner.mBackGroundPageControllerLeft.Update(0.01f, ref this.identity, true);
      iOwner.mBackGroundPageControllerRight.Update(0.01f, ref this.identity, true);
      for (int index = 0; index < iOwner.mRenderData.Length; ++index)
      {
        Array.Copy((Array) iOwner.mTomeController.SkinnedBoneTransforms, (Array) iOwner.mRenderData[index].mTomeBones, iOwner.mRenderData[index].mTomeBones.Length);
        Array.Copy((Array) iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, (Array) iOwner.mRenderData[index].mBackgroundPageBonesLeft, iOwner.mRenderData[index].mBackgroundPageBonesLeft.Length);
        Array.Copy((Array) iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, (Array) iOwner.mRenderData[index].mBackgroundPageBonesRight, iOwner.mRenderData[index].mBackgroundPageBonesRight.Length);
      }
      GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
      GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
      GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
      GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
      if (GameStateManager.Instance.CurrentState is PlayState && Tome.Instance.CloseToMenu)
        GameStateManager.Instance.PopState();
      if (iOwner.OnClose == null)
        return;
      iOwner.OnClose();
    }

    public override void OnExit(Tome iOwner)
    {
      GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
      GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
    }
  }

  public class OpenState : Tome.TomeState
  {
    private static Tome.OpenState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Tome.OpenState Instance
    {
      get
      {
        if (Tome.OpenState.mSingelton == null)
        {
          lock (Tome.OpenState.mSingeltonLock)
          {
            if (Tome.OpenState.mSingelton == null)
              Tome.OpenState.mSingelton = new Tome.OpenState();
          }
        }
        return Tome.OpenState.mSingelton;
      }
    }

    private OpenState()
    {
    }

    public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
    {
      Tome.RenderData renderData = iOwner.mRenderData[(int) iDataChannel];
      Array.Copy((Array) iOwner.mTomeController.SkinnedBoneTransforms, (Array) renderData.mTomeBones, renderData.mTomeBones.Length);
      Array.Copy((Array) iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
      Array.Copy((Array) iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
    }

    public override void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
    {
      iMenu.Draw(this.mCurrentLeft, this.mCurrentRight);
    }

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
      if (startupLobby == 0UL)
        return;
      NetworkManager.Instance.DirectConnect(startupLobby);
      GlobalSettings.Instance.StartupLobby = 0UL;
    }

    public override void OnExit(Tome iOwner)
    {
    }
  }

  public class OpeningState : Tome.TomeState
  {
    private static Tome.OpeningState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Tome.OpeningState Instance
    {
      get
      {
        if (Tome.OpeningState.mSingelton == null)
        {
          lock (Tome.OpeningState.mSingeltonLock)
          {
            if (Tome.OpeningState.mSingelton == null)
              Tome.OpeningState.mSingelton = new Tome.OpeningState();
          }
        }
        return Tome.OpeningState.mSingelton;
      }
    }

    private OpeningState()
    {
    }

    public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
    {
      if (iOwner.mTomeController.IsPlaying)
      {
        Tome.RenderData renderData = iOwner.mRenderData[(int) iDataChannel];
        iOwner.mTomeController.Update(iDeltaTime, ref this.identity, true);
        iOwner.mBackGroundPageControllerLeft.Update(iDeltaTime, ref this.identity, true);
        iOwner.mBackGroundPageControllerRight.Update(iDeltaTime, ref this.identity, true);
        Array.Copy((Array) iOwner.mTomeController.SkinnedBoneTransforms, (Array) renderData.mTomeBones, renderData.mTomeBones.Length);
        Array.Copy((Array) iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
        Array.Copy((Array) iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
      }
      else
        iOwner.ChangeState((Tome.TomeState) Tome.OpenState.Instance);
    }

    public override void OnEnter(Tome iOwner)
    {
      iOwner.mTomeController.PlaybackMode = PlaybackMode.Forward;
      iOwner.mTomeController.StartClip(iOwner.mTomeClips[0], false);
      iOwner.mTomeController.Update(1f / 1000f, ref this.identity, true);
      iOwner.mBackGroundPageControllerLeft.PlaybackMode = PlaybackMode.Forward;
      iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[0], false);
      iOwner.mBackGroundPageControllerLeft.Update(1f / 1000f, ref this.identity, true);
      iOwner.mBackGroundPageControllerRight.PlaybackMode = PlaybackMode.Forward;
      iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[1], false);
      iOwner.mBackGroundPageControllerRight.Update(1f / 1000f, ref this.identity, true);
      AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_bookopen".GetHashCodeCustom());
      if (iOwner.mMenuStackPosition < 0)
      {
        iOwner.mMenuStack[iOwner.mMenuStackPosition + 1] = (SubMenu) SubMenuMain.Instance;
        ++iOwner.mMenuStackPosition;
        iOwner.mMenuStack[iOwner.mMenuStackPosition].OnEnter();
      }
      else
        iOwner.mMenuStack[iOwner.mMenuStackPosition].OnEnter();
      iOwner.mCameraAnimation.CrossFade(iOwner.mCameraAnimations[1], 0.75f, false);
      if (iOwner.OnOpen == null)
        return;
      iOwner.OnOpen();
    }

    public override void OnExit(Tome iOwner)
    {
    }
  }

  public class ClosingState : Tome.TomeState
  {
    private static Tome.ClosingState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Tome.ClosingState Instance
    {
      get
      {
        if (Tome.ClosingState.mSingelton == null)
        {
          lock (Tome.ClosingState.mSingeltonLock)
          {
            if (Tome.ClosingState.mSingelton == null)
              Tome.ClosingState.mSingelton = new Tome.ClosingState();
          }
        }
        return Tome.ClosingState.mSingelton;
      }
    }

    private ClosingState()
    {
    }

    public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
    {
      if (iOwner.mTomeController.IsPlaying)
      {
        Tome.RenderData renderData = iOwner.mRenderData[(int) iDataChannel];
        iOwner.mTomeController.Update(iDeltaTime, ref this.identity, true);
        Array.Copy((Array) iOwner.mTomeController.SkinnedBoneTransforms, (Array) renderData.mTomeBones, renderData.mTomeBones.Length);
        iOwner.mBackGroundPageControllerLeft.Update(iDeltaTime, ref this.identity, true);
        Array.Copy((Array) iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
        iOwner.mBackGroundPageControllerRight.Update(iDeltaTime, ref this.identity, true);
        Array.Copy((Array) iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
      }
      else
        iOwner.ChangeState((Tome.TomeState) Tome.ClosedState.Instance);
    }

    public override void OnEnter(Tome iOwner)
    {
      iOwner.mTomeController.StartClip(iOwner.mTomeClips[1], false);
      iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[2], false);
      iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[3], false);
      AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_bookclose".GetHashCodeCustom());
    }

    public override void OnExit(Tome iOwner)
    {
    }
  }

  public class RiffleForwardState : Tome.TomeState
  {
    private static Tome.RiffleForwardState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Tome.RiffleForwardState Instance
    {
      get
      {
        if (Tome.RiffleForwardState.mSingelton == null)
        {
          lock (Tome.RiffleForwardState.mSingeltonLock)
          {
            if (Tome.RiffleForwardState.mSingelton == null)
              Tome.RiffleForwardState.mSingelton = new Tome.RiffleForwardState();
          }
        }
        return Tome.RiffleForwardState.mSingelton;
      }
    }

    private RiffleForwardState()
    {
      this.mPreviousLeft = Tome.TOMELEFTSHEET;
      this.mPreviousRight = Tome.PAGERIGHTSHEET;
      this.mCurrentLeft = Tome.PAGELEFTSHEET;
      this.mCurrentRight = Tome.TOMERIGHTSHEET;
    }

    public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
    {
      if (!iOwner.mRiffleController.HasFinished)
        iOwner.mRiffleController.Update(iDeltaTime);
      else
        iOwner.ChangeState((Tome.TomeState) Tome.OpenState.Instance);
    }

    public override void OnEnter(Tome iOwner)
    {
      iOwner.mRiffleController.StartClip(Tome.FlipPageAnimations.FlipLeft, false, iOwner.mMenuRiffleStack.Peek());
      AudioManager.Instance.PlayCue<Tome.PageSoundVariables>(Banks.UI, "ui_menu_tomepage".GetHashCodeCustom(), new Tome.PageSoundVariables()
      {
        Pages = (float) iOwner.mMenuRiffleStack.Peek()
      });
    }

    public override void OnExit(Tome iOwner)
    {
    }
  }

  public class RiffleBackwardState : Tome.TomeState
  {
    private static Tome.RiffleBackwardState mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Tome.RiffleBackwardState Instance
    {
      get
      {
        if (Tome.RiffleBackwardState.mSingelton == null)
        {
          lock (Tome.RiffleBackwardState.mSingeltonLock)
          {
            if (Tome.RiffleBackwardState.mSingelton == null)
              Tome.RiffleBackwardState.mSingelton = new Tome.RiffleBackwardState();
          }
        }
        return Tome.RiffleBackwardState.mSingelton;
      }
    }

    private RiffleBackwardState()
    {
      this.mPreviousLeft = Tome.PAGELEFTSHEET;
      this.mPreviousRight = Tome.TOMERIGHTSHEET;
      this.mCurrentLeft = Tome.TOMELEFTSHEET;
      this.mCurrentRight = Tome.PAGERIGHTSHEET;
    }

    public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
    {
      if (!iOwner.mRiffleController.HasFinished)
        iOwner.mRiffleController.Update(iDeltaTime);
      else
        iOwner.ChangeState((Tome.TomeState) Tome.OpenState.Instance);
    }

    public override void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
    {
      if (iOwner.mMenuStackPosition < 0)
        return;
      iMenu.DrawNewAndOld(iOwner.mMenuStack[iOwner.mMenuStackPosition + 1], this.mCurrentLeft, this.mCurrentRight, this.mPreviousLeft, this.mPreviousRight);
    }

    public override void OnEnter(Tome iOwner)
    {
      int iNumControllers = iOwner.mMenuRiffleStack.Peek();
      iOwner.mRiffleController.StartClip(Tome.FlipPageAnimations.FlipRight, false, iNumControllers);
      AudioManager.Instance.PlayCue<Tome.PageSoundVariables>(Banks.UI, "ui_menu_tomepage".GetHashCodeCustom(), new Tome.PageSoundVariables()
      {
        Pages = (float) iNumControllers
      });
    }

    public override void OnExit(Tome iOwner)
    {
    }
  }

  public class CloseToBack : Tome.TomeState
  {
    private static Tome.CloseToBack mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Tome.CloseToBack Instance
    {
      get
      {
        if (Tome.CloseToBack.mSingelton == null)
        {
          lock (Tome.CloseToBack.mSingeltonLock)
          {
            if (Tome.CloseToBack.mSingelton == null)
              Tome.CloseToBack.mSingelton = new Tome.CloseToBack();
          }
        }
        return Tome.CloseToBack.mSingelton;
      }
    }

    private CloseToBack()
    {
    }

    public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
    {
      Tome.RenderData renderData = iOwner.mRenderData[(int) iDataChannel];
      iOwner.mTomeController.Update(iDeltaTime, ref this.identity, true);
      Array.Copy((Array) iOwner.mTomeController.SkinnedBoneTransforms, (Array) renderData.mTomeBones, renderData.mTomeBones.Length);
      iOwner.mBackGroundPageControllerLeft.Update(iDeltaTime, ref this.identity, true);
      Array.Copy((Array) iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
      iOwner.mBackGroundPageControllerRight.Update(iDeltaTime, ref this.identity, true);
      Array.Copy((Array) iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
      if (!iOwner.mTomeController.HasFinished || iOwner.mTomeController.CrossFadeEnabled)
        return;
      iOwner.ChangeState((Tome.TomeState) Tome.ClosedBack.Instance);
    }

    public override void OnEnter(Tome iOwner)
    {
      iOwner.mTomeController.StartClip(iOwner.mTomeClips[1], false);
      iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[2], false);
      iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[3], false);
      AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_bookclose".GetHashCodeCustom());
    }

    public override void OnExit(Tome iOwner)
    {
    }
  }

  public class BackToFront : Tome.TomeState
  {
    private static Tome.BackToFront mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Tome.BackToFront Instance
    {
      get
      {
        if (Tome.BackToFront.mSingelton == null)
        {
          lock (Tome.BackToFront.mSingeltonLock)
          {
            if (Tome.BackToFront.mSingelton == null)
              Tome.BackToFront.mSingelton = new Tome.BackToFront();
          }
        }
        return Tome.BackToFront.mSingelton;
      }
    }

    private BackToFront()
    {
    }

    public override void Update(float iDeltaTime, DataChannel iDataChannel, Tome iOwner)
    {
      Tome.RenderData renderData = iOwner.mRenderData[(int) iDataChannel];
      iOwner.mTomeController.Update(iDeltaTime, ref this.identity, true);
      Array.Copy((Array) iOwner.mTomeController.SkinnedBoneTransforms, (Array) renderData.mTomeBones, renderData.mTomeBones.Length);
      iOwner.mBackGroundPageControllerLeft.Update(iDeltaTime, ref this.identity, true);
      Array.Copy((Array) iOwner.mBackGroundPageControllerLeft.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesLeft, renderData.mBackgroundPageBonesLeft.Length);
      iOwner.mBackGroundPageControllerRight.Update(iDeltaTime, ref this.identity, true);
      Array.Copy((Array) iOwner.mBackGroundPageControllerRight.SkinnedBoneTransforms, (Array) renderData.mBackgroundPageBonesRight, renderData.mBackgroundPageBonesRight.Length);
      if (!iOwner.mTomeController.HasFinished || iOwner.mTomeController.CrossFadeEnabled)
        return;
      iOwner.ChangeState((Tome.TomeState) Tome.OpenState.Instance);
    }

    public override void OnEnter(Tome iOwner)
    {
      iOwner.mTomeController.StartClip(iOwner.mTomeClips[0], false);
      iOwner.mBackGroundPageControllerLeft.PlaybackMode = PlaybackMode.Forward;
      iOwner.mBackGroundPageControllerLeft.StartClip(iOwner.mBackGroundClips[0], false);
      iOwner.mBackGroundPageControllerLeft.Update(1f / 1000f, ref this.identity, true);
      iOwner.mBackGroundPageControllerRight.PlaybackMode = PlaybackMode.Forward;
      iOwner.mBackGroundPageControllerRight.StartClip(iOwner.mBackGroundClips[1], false);
      iOwner.mBackGroundPageControllerRight.Update(1f / 1000f, ref this.identity, true);
      AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_bookclose".GetHashCodeCustom());
    }

    public override void OnExit(Tome iOwner)
    {
    }
  }

  public class ClosedBack : Tome.TomeState
  {
    private static Tome.ClosedBack mSingelton;
    private static volatile object mSingeltonLock = new object();

    public static Tome.ClosedBack Instance
    {
      get
      {
        if (Tome.ClosedBack.mSingelton == null)
        {
          lock (Tome.ClosedBack.mSingeltonLock)
          {
            if (Tome.ClosedBack.mSingelton == null)
              Tome.ClosedBack.mSingelton = new Tome.ClosedBack();
          }
        }
        return Tome.ClosedBack.mSingelton;
      }
    }

    private ClosedBack()
    {
    }

    public override void Draw(GUIBasicEffect iEffect, Tome iOwner, SubMenu iMenu)
    {
    }

    public override void OnEnter(Tome iOwner)
    {
      base.OnEnter(iOwner);
      if (iOwner.OnBackClose == null)
        return;
      iOwner.OnBackClose();
    }
  }

  public enum RightHandItemType
  {
    None = -1, // 0xFFFFFFFF
    AccLogIn = 0,
    AccCreate = 1,
    Promotion = 2,
    Facebook = 3,
    Twitter = 4,
    Max = 5,
  }

  private struct PageSoundVariables : IAudioVariables
  {
    public static readonly string VARIABLE = nameof (Pages);
    public float Pages;

    public void AssignToCue(Cue iCue)
    {
      iCue.SetVariable(Tome.PageSoundVariables.VARIABLE, this.Pages);
    }
  }

  private class RenderData : IRenderableGUIObject, IPreRenderRenderer
  {
    public Matrix View;
    public Matrix Projection;
    public Matrix ViewProjection;
    private Matrix[] mBackgroundBone;
    public Matrix[] mTomeBones;
    public Matrix[] mBackgroundPageBonesRight;
    public Matrix[] mBackgroundPageBonesLeft;
    public Matrix[][] mPageBones;
    private SkinnedModel mTomeModel;
    private SkinnedModel mPageModel;
    private SkinnedModel mBackGroundPageModel;
    private SkinnedModel mBackgroundModel;
    private Matrix mLightViewProj;
    public Vector3 LightAmbient;
    public Vector3 LightDiffuse;
    public Vector3 LightSpecular;
    private GUIBasicEffect mEffect;
    private Texture2D mTexture;
    private VertexBuffer mVertexBuffer;
    private VertexDeclaration mVertexDeclaration;
    private RenderTarget2D mRenderTarget;
    private RenderTarget2D mShadowMap;
    private DepthStencilBuffer mShadowDepthBuffer;
    public Tome.TomeState State;
    public SubMenu CurrentMenu;

    public RenderData(
      RenderTarget2D iRenderTarget,
      RenderTarget2D iShadowMap,
      DepthStencilBuffer iShadowDepthBuffer,
      SkinnedModel iTomeModel,
      SkinnedModel iPageModel,
      SkinnedModel iBackGroundPageModel,
      SkinnedModel iBackgroundModel,
      VertexBuffer iVertexBuffer,
      Texture2D iTexture,
      VertexDeclaration iVertexDeclaration,
      GUIBasicEffect iEffect)
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
      for (int index = 0; index < Tome.MaxNrOfPages; ++index)
        this.mPageBones[index] = new Matrix[iPageModel.SkeletonBones.Count];
    }

    public void Draw(float iDeltaTime)
    {
      GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
      graphicsDevice.RenderState.DepthBufferEnable = true;
      graphicsDevice.RenderState.DepthBufferWriteEnable = true;
      for (int index1 = 0; index1 < this.mBackGroundPageModel.Model.Meshes.Count; ++index1)
      {
        ModelMesh mesh = this.mBackGroundPageModel.Model.Meshes[index1];
        for (int index2 = 0; index2 < mesh.Effects.Count; ++index2)
        {
          (mesh.Effects[index2] as SkinnedModelBasicEffect).DiffuseIsHDR = true;
          (mesh.Effects[index2] as SkinnedModelBasicEffect).DiffuseMap0 = this.mRenderTarget.GetTexture();
        }
      }
      for (int index3 = 0; index3 < this.mPageModel.Model.Meshes.Count; ++index3)
      {
        ModelMesh mesh = this.mPageModel.Model.Meshes[index3];
        for (int index4 = 0; index4 < mesh.Effects.Count; ++index4)
        {
          (mesh.Effects[index4] as SkinnedModelBasicEffect).DiffuseIsHDR = true;
          (mesh.Effects[index4] as SkinnedModelBasicEffect).DiffuseMap0 = this.mRenderTarget.GetTexture();
        }
      }
      if (GameStateManager.Instance.CurrentState is MenuState)
        this.DrawModel(this.mBackgroundModel, this.mBackgroundBone);
      Magicka.Game.Instance.GraphicsDevice.RenderState.AlphaBlendEnable = false;
      this.DrawModel(this.mTomeModel, this.mTomeBones);
      Magicka.Game.Instance.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      this.DrawModel(this.mBackGroundPageModel, this.mBackgroundPageBonesRight);
      this.DrawModel(this.mBackGroundPageModel, this.mBackgroundPageBonesLeft);
      if (this.State is Tome.RiffleBackwardState || this.State is Tome.RiffleForwardState)
      {
        for (int index = 0; index < Tome.Instance.mRiffleController.ControllersPlaying; ++index)
          this.DrawModel(this.mPageModel, this.mPageBones[index]);
      }
      graphicsDevice.RenderState.DepthBufferEnable = false;
      graphicsDevice.RenderState.DepthBufferWriteEnable = false;
      Vector4 vector4 = new Vector4();
      vector4.X = vector4.Y = vector4.Z = 1f;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      Point screenSize = RenderManager.Instance.ScreenSize;
      this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
      if (!(this.CurrentMenu is SubMenuCutscene) && !(this.CurrentMenu is SubMenuIntro))
      {
        if (Tome.sPromotionButton != null)
          Tome.sPromotionButton.Draw(this.mEffect);
        if (Tome.sShownSplashIsNew)
          Tome.sSplashNew.Draw(this.mEffect);
        Tome.sFaceBook.Draw(this.mEffect);
        Tome.sTwitter.Draw(this.mEffect);
        Tome.sIndicatorBackground.Draw(this.mEffect);
        Tome.sParadoxIcon.Draw(this.mEffect);
        if (!Singleton<ParadoxAccount>.Instance.IsLoggedFull)
        {
          if (Tome.sAccountLoginBtn.Enabled)
            Tome.sAccountLoginBtn.Draw(this.mEffect);
          if (Tome.sAccountCreationBtn.Enabled)
            Tome.sAccountCreationBtn.Draw(this.mEffect);
        }
        this.mEffect.Color = Vector4.One;
        Tome.sVersionText.Draw(this.mEffect, 16f, (float) screenSize.Y - 16f - (float) Tome.sVersionText.Font.LineHeight);
      }
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
      this.mEffect.TextureOffset = new Vector2();
      this.mEffect.TextureScale = Vector2.One;
      vector4.W = 1f;
      this.mEffect.Color = vector4;
    }

    private void DrawModel(SkinnedModel iModel, Matrix[] iBones)
    {
      this.DrawModel(iModel, iBones, SkinnedModelBasicEffect.Technique.NonDeffered, ref this.View, ref this.Projection, ref this.ViewProjection);
    }

    private void DrawModel(
      SkinnedModel iModel,
      Matrix[] iBones,
      SkinnedModelBasicEffect.Technique iTechnique,
      ref Matrix iView,
      ref Matrix iProjection,
      ref Matrix iViewProjection)
    {
      foreach (ModelMesh mesh in iModel.Model.Meshes)
      {
        foreach (SkinnedModelBasicEffect effect in mesh.Effects)
        {
          effect.SetTechnique(iTechnique);
          effect.View = iView;
          effect.Projection = iProjection;
          effect.ViewProjection = iViewProjection;
          effect.LightPosition = Tome.sLightPosition;
          effect.LightAmbientColor = this.LightAmbient;
          effect.LightDiffuseColor = this.LightDiffuse;
          effect.LightSpecularColor = this.LightSpecular;
          effect.NormalPower = 1f;
          if (iTechnique != SkinnedModelBasicEffect.Technique.Shadow)
          {
            effect.LightViewProjection = this.mLightViewProj;
            effect.ShadowMapEnabled = true;
            effect.ShadowMap = this.mShadowMap.GetTexture();
            effect.ShadowMapScale = 1f / (float) this.mShadowMap.Width;
          }
          effect.Bones = iBones;
          effect.CommitChanges();
        }
        mesh.Draw();
      }
    }

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
      Vector3 cameraTarget = new Vector3(-2f, 0.0f, 2.5f);
      Matrix result1;
      Matrix.CreateLookAt(ref Tome.sLightPosition, ref cameraTarget, ref new Vector3()
      {
        Y = 1f
      }, out result1);
      Matrix result2;
      Matrix.CreatePerspectiveFieldOfView(1.88495576f, 1f, 1f, 15f, out result2);
      Matrix.Multiply(ref result1, ref result2, out this.mLightViewProj);
      if (this.State is Tome.RiffleBackwardState | this.State is Tome.RiffleForwardState)
      {
        for (int index = 0; index < Tome.Instance.mRiffleController.ControllersPlaying; ++index)
          this.DrawModel(this.mPageModel, this.mPageBones[index], SkinnedModelBasicEffect.Technique.Shadow, ref result1, ref result2, ref this.mLightViewProj);
      }
      this.DrawModel(this.mBackGroundPageModel, this.mBackgroundPageBonesRight, SkinnedModelBasicEffect.Technique.Shadow, ref result1, ref result2, ref this.mLightViewProj);
      this.DrawModel(this.mBackGroundPageModel, this.mBackgroundPageBonesLeft, SkinnedModelBasicEffect.Technique.Shadow, ref result1, ref result2, ref this.mLightViewProj);
      this.DrawModel(this.mTomeModel, this.mTomeBones, SkinnedModelBasicEffect.Technique.Shadow, ref result1, ref result2, ref this.mLightViewProj);
      this.DrawModel(this.mBackgroundModel, this.mBackgroundBone, SkinnedModelBasicEffect.Technique.Shadow, ref result1, ref result2, ref this.mLightViewProj);
    }

    public int ZIndex => 900;

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      this.mRenderTarget.GraphicsDevice.SetRenderTarget(0, this.mRenderTarget);
      this.mRenderTarget.GraphicsDevice.DepthStencilBuffer = (DepthStencilBuffer) null;
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
      this.mEffect.Texture = (Texture) this.mTexture;
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
        this.State.Draw(this.mEffect, Tome.Instance, this.CurrentMenu);
      this.DrawShadows();
    }
  }

  public class RiffleController
  {
    private PlaybackMode mPlaybackMode;
    private Matrix identity = Matrix.Identity;
    private AnimationController[] mControllers;
    private AnimationClip[] mClips;
    private int mNrOfControllers;
    private float mSpeed;
    private float mTimer;
    private float mTimerOffset;
    private int mActiveClipIndex;
    private int mNrOfControllersActive;
    private int mNrOfControllersLeftToActivate;

    public RiffleController(int iNrOfControllers, SkinnedModel iModel)
    {
      this.mTimer = 0.0f;
      this.mSpeed = 2f;
      this.mActiveClipIndex = 0;
      this.mNrOfControllersActive = 0;
      this.mNrOfControllersLeftToActivate = 0;
      this.mNrOfControllers = iNrOfControllers;
      this.mPlaybackMode = PlaybackMode.Forward;
      this.mControllers = new AnimationController[iNrOfControllers];
      for (int index = 0; index < iNrOfControllers; ++index)
      {
        this.mControllers[index] = new AnimationController();
        this.mControllers[index].Skeleton = iModel.SkeletonBones;
      }
      this.mClips = new AnimationClip[2];
      this.mClips[0] = iModel.AnimationClips["flip_left"];
      this.mClips[1] = iModel.AnimationClips["flip_right"];
      this.mTimerOffset = this.mClips[0].Duration * 0.625f / (float) this.mNrOfControllers;
    }

    public void ArrayCopyTo(ref Matrix[][] iTarget)
    {
      for (int index = 0; index < this.mNrOfControllers; ++index)
        Array.Copy((Array) this.mControllers[index].SkinnedBoneTransforms, (Array) iTarget[index], iTarget[index].Length);
    }

    public int ControllersPlaying => this.mNrOfControllersActive;

    public void StartClip(Tome.FlipPageAnimations iAnimation, bool iLoop, int iNumControllers)
    {
      if (iNumControllers > this.mNrOfControllers)
        iNumControllers = this.mNrOfControllers;
      float elapsedTime = 0.0f;
      this.mPlaybackMode = PlaybackMode.Forward;
      this.mActiveClipIndex = (int) iAnimation;
      this.mNrOfControllersActive = iNumControllers;
      this.mNrOfControllersLeftToActivate = iNumControllers;
      for (int index = 0; index < iNumControllers; ++index)
      {
        this.mControllers[index].Speed = this.mSpeed;
        this.mControllers[index].PlaybackMode = PlaybackMode.Forward;
        this.mControllers[index].StartClip(this.mClips[(int) iAnimation], iLoop);
        this.mControllers[index].Update(elapsedTime, ref this.identity, true);
        this.mControllers[index].PlaybackMode = this.mPlaybackMode;
      }
    }

    public void Update(float iDeltaTime)
    {
      for (this.mTimer -= iDeltaTime; (double) this.mTimer <= 0.0 && this.mNrOfControllersLeftToActivate > 0; this.mTimer += this.mTimerOffset)
      {
        --this.mNrOfControllersLeftToActivate;
        int controllersLeftToActivate = this.mNrOfControllersLeftToActivate;
        this.mControllers[controllersLeftToActivate].StartClip(this.mClips[this.mActiveClipIndex], false);
        this.mControllers[controllersLeftToActivate].Update((float) (-(double) this.mTimer + 0.0099999997764825821), ref this.identity, false);
      }
      if (this.mNrOfControllersLeftToActivate == 0)
        this.mTimer = 0.0f;
      for (int index = 0; index < this.mNrOfControllers; ++index)
      {
        if (this.mControllers[index].IsPlaying)
          this.mControllers[index].Update(iDeltaTime, ref this.identity, true);
      }
    }

    public bool HasFinished
    {
      get
      {
        for (int index = 0; index < this.mNrOfControllersActive; ++index)
        {
          if (!this.mControllers[index].HasFinished)
            return false;
        }
        return true;
      }
    }
  }

  public enum CameraAnimation
  {
    Wake_Up,
    Idle,
    Look_Left,
    Look_Back,
    Doze_Off,
    Zoomed_In,
  }

  public enum TomeAnimations
  {
    Open,
    Close,
    Turn,
    NrOfAnimations,
  }

  public enum BackGroundPageAnimations
  {
    OpenLeft,
    OpenRight,
    CloseLeft,
    CloseRight,
    NrOfAnimations,
  }

  public enum FlipPageAnimations
  {
    FlipLeft,
    FlipRight,
    NrOfAnimations,
  }

  internal enum AccountIndicatorState
  {
    LoggedIn,
    LoggedOut,
  }
}
