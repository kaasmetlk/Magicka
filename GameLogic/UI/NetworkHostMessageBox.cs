// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.NetworkHostMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.InGameMenus;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using SteamWrapper;
using System;
using System.Text;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class NetworkHostMessageBox : MessageBox
{
  public const float MAX_WIDTH = 320f;
  private static NetworkHostMessageBox sSingelton;
  private static volatile object sSingeltonLock = new object();
  public static readonly int GAMETYPE_ADVENTURE_HASH = SubMenu.LOC_ADVENTURE;
  public static readonly int GAMETYPE_CHALLENGE_HASH = SubMenuOnline.LOC_CHALLENGE;
  public static readonly int GAMETYPE_STORYCHALLANGE_HASH = SubMenuOnline.LOC_STORYCHALLANGE;
  public static readonly int GAMETYPE_VERSUS_HASH = "#menu_main_03".GetHashCodeCustom();
  public static readonly int VAC_HASH = "#network_27".GetHashCodeCustom();
  private static readonly int NAME_HASH = "#add_menu_gamename".GetHashCodeCustom();
  private static readonly int PORT_HASH = "#add_menu_port".GetHashCodeCustom();
  private static bool mHasOSOTCLicense = false;
  private static bool mHasDUNG1License = false;
  private static bool mHasDUNG2License = false;
  private static bool mHasVietnamLicense = false;
  private static bool mHasMythosLicense = false;
  private static readonly int LOC_NAME = "#add_menu_prof_name".GetHashCodeCustom();
  private static readonly int LOC_PASSWORD = "#settings_p01_password".GetHashCodeCustom();
  private NetworkHostMessageBox.StoryChallengeType storyChallengeType;
  private bool mEditName;
  private int mSelectedPosition;
  private PolygonHead.Text mNameTitle;
  private PolygonHead.Text mNameText;
  private StringBuilder mName;
  private Rectangle mNameRect;
  private bool mEditPassword;
  private PolygonHead.Text mPasswordTitle;
  private PolygonHead.Text mPasswordText;
  private StringBuilder mPassword;
  private Rectangle mPasswordRect;
  private bool mVAC = true;
  private PolygonHead.Text mVACText;
  private PolygonHead.Text mVACTitle;
  private Rectangle mVACRect;
  private PolygonHead.Text mModeText;
  private PolygonHead.Text mModeTitle;
  private Rectangle mModeRect;
  private GameType mGameType = GameType.Challenge;
  private MenuTextItem mCancelButton;
  private MenuTextItem mOkButton;
  private float mTimer;
  private bool mLine;
  private NetworkHostMessageBox.Complete mComplete;
  private OptionsMessageBox mNameError;

  public static NetworkHostMessageBox Instance
  {
    get
    {
      if (NetworkHostMessageBox.sSingelton == null)
      {
        lock (NetworkHostMessageBox.sSingeltonLock)
        {
          if (NetworkHostMessageBox.sSingelton == null)
            NetworkHostMessageBox.sSingelton = new NetworkHostMessageBox();
        }
      }
      return NetworkHostMessageBox.sSingelton;
    }
  }

  private NetworkHostMessageBox()
    : base("#menu_opt_online_01".GetHashCodeCustom())
  {
    this.mNameTitle = new PolygonHead.Text(32 /*0x20*/, this.mFont, TextAlign.Center, false);
    this.mNameTitle.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.NAME_HASH));
    this.mName = new StringBuilder(15, 15);
    this.mNameText = new PolygonHead.Text(this.mName.Capacity + 2, this.mFont, TextAlign.Left, true, false);
    this.mNameRect.Width = 320;
    this.mNameRect.Height = this.mFont.LineHeight * 2;
    this.mPasswordTitle = new PolygonHead.Text(32 /*0x20*/, this.mFont, TextAlign.Center, false);
    this.mPasswordTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_SETTINGS_PASSWORD));
    this.mPassword = new StringBuilder(10, 10);
    this.mPasswordText = new PolygonHead.Text(this.mPassword.Capacity + 2, this.mFont, TextAlign.Left, true, false);
    this.mPasswordRect.Width = 320;
    this.mPasswordRect.Height = this.mFont.LineHeight * 2;
    this.mVACTitle = new PolygonHead.Text(32 /*0x20*/, this.mFont, TextAlign.Center, false);
    this.mVACTitle.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.VAC_HASH));
    this.mVACText = new PolygonHead.Text(32 /*0x20*/, this.mFont, TextAlign.Center, false);
    this.mVACText.SetText(LanguageManager.Instance.GetString(InGameMenu.LOC_ON));
    this.mVACRect.Width = (int) Math.Max(this.mFont.MeasureText(this.mVACTitle.Characters, true).X, this.mFont.MeasureText(this.mVACText.Characters, true).X);
    this.mVACRect.Height = 2 * this.mFont.LineHeight;
    this.mName.Append(GlobalSettings.Instance.GameName);
    this.mNameText.SetText(this.mName.ToString());
    Vector2 iPosition = new Vector2();
    this.mCancelButton = new MenuTextItem(Defines.LOC_GEN_CANCEL, iPosition, this.mFont, TextAlign.Left);
    this.mModeTitle = new PolygonHead.Text(32 /*0x20*/, this.mFont, TextAlign.Center, false);
    this.mModeTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_MODE));
    this.mGameType = GameType.Challenge;
    this.mModeText = new PolygonHead.Text(35, this.mFont, TextAlign.Center, false);
    this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
    this.mModeRect.Width = 320;
    this.mModeRect.Height = this.mFont.LineHeight * 2;
    this.mOkButton = new MenuTextItem(Defines.LOC_GEN_OK, iPosition, this.mFont, TextAlign.Right);
    this.mNameError = new OptionsMessageBox(LanguageManager.Instance.GetString("#add_menu_err".GetHashCodeCustom()) + LanguageManager.Instance.GetString("#add_menu_err_gamename".GetHashCodeCustom()), new string[1]
    {
      LanguageManager.Instance.GetString(Defines.LOC_GEN_OK)
    });
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mMessage.SetText(LanguageManager.Instance.GetString("#menu_opt_online_01".GetHashCodeCustom()));
    this.mNameTitle.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.NAME_HASH));
    this.mPasswordTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_SETTINGS_PASSWORD));
    this.mVACTitle.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.VAC_HASH));
    this.mVACText.SetText(LanguageManager.Instance.GetString(this.mVAC ? InGameMenu.LOC_ON : InGameMenu.LOC_OFF));
    this.mVACRect.Width = (int) Math.Max(this.mFont.MeasureText(this.mVACTitle.Characters, true).X, this.mFont.MeasureText(this.mVACText.Characters, true).X);
    switch (this.mGameType)
    {
      case GameType.Campaign:
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
        break;
      case GameType.Challenge:
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
        break;
      case GameType.Versus:
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
        break;
      case GameType.Mythos:
        this.mModeText.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_MYTHOS));
        break;
      case GameType.StoryChallange:
        switch (this.storyChallengeType)
        {
          case NetworkHostMessageBox.StoryChallengeType.Vietnam:
            this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
            break;
          case NetworkHostMessageBox.StoryChallengeType.OSOTC:
            this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
            break;
          case NetworkHostMessageBox.StoryChallengeType.DUNG1:
            this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
            break;
          case NetworkHostMessageBox.StoryChallengeType.DUNG2:
            this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
            break;
        }
        break;
    }
    this.mModeTitle.SetText(LanguageManager.Instance.GetString(SubMenuOnline.LOC_MODE));
    this.mNameError = new OptionsMessageBox(LanguageManager.Instance.GetString("#add_menu_err".GetHashCodeCustom()) + LanguageManager.Instance.GetString("#add_menu_err_gamename".GetHashCodeCustom()), new string[1]
    {
      LanguageManager.Instance.GetString(Defines.LOC_GEN_OK)
    });
    this.mCancelButton.LanguageChanged();
    this.mOkButton.LanguageChanged();
  }

  public override void Show() => throw new InvalidOperationException();

  public void Show(NetworkHostMessageBox.Complete iOnComplete)
  {
    NetworkHostMessageBox.mHasVietnamLicense = HackHelper.CheckLicenseVietnam();
    NetworkHostMessageBox.mHasOSOTCLicense = HackHelper.CheckLicenseOSOTC();
    NetworkHostMessageBox.mHasDUNG1License = HackHelper.CheckLicenseDungeons1();
    NetworkHostMessageBox.mHasDUNG2License = HackHelper.CheckLicenseDungeons2();
    NetworkHostMessageBox.mHasMythosLicense = HackHelper.CheckLicenseMythos();
    if (HackHelper.LicenseStatus != HackHelper.Status.Valid)
    {
      this.mVAC = false;
      this.mVACText.SetText(LanguageManager.Instance.GetString(InGameMenu.LOC_OFF));
    }
    this.mComplete += iOnComplete;
    this.mSelectedPosition = -1;
    this.mEditPassword = false;
    this.mPassword.Length = 0;
    this.mPasswordText.SetText("");
    base.Show();
  }

  public override void Kill()
  {
    this.mComplete = (NetworkHostMessageBox.Complete) null;
    base.Kill();
  }

  public override void OnTextInput(char iChar)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    if (this.mEditName)
    {
      if (iChar == '\b')
      {
        if (this.mName.Length > 0)
          --this.mName.Length;
      }
      else if (this.mName.Length < this.mName.Capacity & !(this.mName.Length == 0 & iChar == ' '))
        this.mName.Append(iChar);
      this.mNameText.SetText(this.mName.ToString());
    }
    else
    {
      if (!this.mEditPassword)
        return;
      if (iChar == '\b')
      {
        if (this.mPassword.Length > 0)
          --this.mPassword.Length;
      }
      else if (this.mPassword.Length < this.mPassword.Capacity & !(this.mPassword.Length == 0 & iChar == ' '))
        this.mPassword.Append(iChar);
      this.mPasswordText.SetText(this.mPassword.ToString());
    }
  }

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    switch (iDirection)
    {
      case ControllerDirection.Right:
        if (this.mSelectedPosition == 1)
        {
          this.ScrollGameMode_Right();
          break;
        }
        if (this.mSelectedPosition == 4)
        {
          this.mSelectedPosition = 5;
          break;
        }
        if (this.mSelectedPosition != 5)
          break;
        this.mSelectedPosition = 4;
        break;
      case ControllerDirection.Up:
        int num1 = this.mSelectedPosition - 1;
        if (num1 < 0)
          num1 += 6;
        this.mSelectedPosition = num1;
        break;
      case ControllerDirection.Left:
        if (this.mSelectedPosition == 1)
        {
          this.ScrollGameMode_Left();
          break;
        }
        if (this.mSelectedPosition == 4)
        {
          this.mSelectedPosition = 5;
          break;
        }
        if (this.mSelectedPosition != 5)
          break;
        this.mSelectedPosition = 4;
        break;
      case ControllerDirection.Down:
        int num2 = this.mSelectedPosition + 1;
        if (num2 >= 6)
          num2 -= 6;
        this.mSelectedPosition = num2;
        break;
    }
  }

  private void ScrollGameMode_Left()
  {
    switch (this.mGameType)
    {
      case GameType.Campaign:
        if (NetworkHostMessageBox.mHasDUNG1License)
        {
          this.mGameType = GameType.StoryChallange;
          this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG1;
          this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
          break;
        }
        if (NetworkHostMessageBox.mHasDUNG2License)
        {
          this.mGameType = GameType.StoryChallange;
          this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
          this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
          break;
        }
        if (NetworkHostMessageBox.mHasOSOTCLicense)
        {
          this.mGameType = GameType.StoryChallange;
          this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
          this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
          break;
        }
        if (NetworkHostMessageBox.mHasVietnamLicense)
        {
          this.mGameType = GameType.StoryChallange;
          this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
          this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
          break;
        }
        this.mGameType = GameType.Versus;
        this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
        break;
      case GameType.Challenge:
        if (NetworkHostMessageBox.mHasMythosLicense)
        {
          this.mGameType = GameType.Mythos;
          this.mModeText.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_MYTHOS));
          break;
        }
        this.mGameType = GameType.Campaign;
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
        break;
      case GameType.Versus:
        this.mGameType = GameType.Challenge;
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
        break;
      case GameType.Mythos:
        this.mGameType = GameType.Campaign;
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
        break;
      case GameType.StoryChallange:
        switch (this.storyChallengeType)
        {
          case NetworkHostMessageBox.StoryChallengeType.Vietnam:
            this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
            this.mGameType = GameType.Versus;
            this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
            return;
          case NetworkHostMessageBox.StoryChallengeType.OSOTC:
            if (NetworkHostMessageBox.mHasVietnamLicense)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
              return;
            }
            this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
            this.mGameType = GameType.Versus;
            this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
            return;
          case NetworkHostMessageBox.StoryChallengeType.DUNG1:
            if (NetworkHostMessageBox.mHasDUNG2License)
            {
              this.mGameType = GameType.StoryChallange;
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
              return;
            }
            if (NetworkHostMessageBox.mHasOSOTCLicense)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
              return;
            }
            if (NetworkHostMessageBox.mHasVietnamLicense)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
              return;
            }
            this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
            this.mGameType = GameType.Versus;
            this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
            return;
          case NetworkHostMessageBox.StoryChallengeType.DUNG2:
            if (NetworkHostMessageBox.mHasOSOTCLicense)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
              return;
            }
            if (NetworkHostMessageBox.mHasVietnamLicense)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
              return;
            }
            this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
            this.mGameType = GameType.Versus;
            this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
            return;
          default:
            return;
        }
    }
  }

  private void ScrollGameMode_Right()
  {
    switch (this.mGameType)
    {
      case GameType.Campaign:
        if (NetworkHostMessageBox.mHasMythosLicense)
        {
          this.mGameType = GameType.Mythos;
          this.mModeText.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_MYTHOS));
          break;
        }
        this.mGameType = GameType.Challenge;
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
        break;
      case GameType.Challenge:
        this.mGameType = GameType.Versus;
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_VERSUS_HASH));
        break;
      case GameType.Versus:
        if (NetworkHostMessageBox.mHasVietnamLicense)
        {
          this.mGameType = GameType.StoryChallange;
          this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.Vietnam;
          this.mModeText.SetText(LevelManager.GetLocalizedName("ch_vietnam"));
          break;
        }
        if (NetworkHostMessageBox.mHasOSOTCLicense)
        {
          this.mGameType = GameType.StoryChallange;
          this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
          this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
          break;
        }
        if (NetworkHostMessageBox.mHasDUNG1License)
        {
          this.mGameType = GameType.StoryChallange;
          this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG1;
          this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
          break;
        }
        if (NetworkHostMessageBox.mHasDUNG2License)
        {
          this.mGameType = GameType.StoryChallange;
          this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
          this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
          break;
        }
        this.mGameType = GameType.Campaign;
        this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
        break;
      case GameType.Mythos:
        this.mGameType = GameType.Challenge;
        this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_CHALLENGE_HASH));
        break;
      case GameType.StoryChallange:
        switch (this.storyChallengeType)
        {
          case NetworkHostMessageBox.StoryChallengeType.Vietnam:
            if (NetworkHostMessageBox.mHasOSOTCLicense)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.OSOTC;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_osotc"));
              return;
            }
            if (NetworkHostMessageBox.mHasDUNG1License)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG1;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
              return;
            }
            if (NetworkHostMessageBox.mHasDUNG2License)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
              return;
            }
            this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
            this.mGameType = GameType.Campaign;
            this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
            return;
          case NetworkHostMessageBox.StoryChallengeType.OSOTC:
            if (NetworkHostMessageBox.mHasDUNG1License)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG1;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch1"));
              return;
            }
            if (NetworkHostMessageBox.mHasDUNG2License)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
              return;
            }
            this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
            this.mGameType = GameType.Campaign;
            this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
            return;
          case NetworkHostMessageBox.StoryChallengeType.DUNG1:
            if (NetworkHostMessageBox.mHasDUNG2License)
            {
              this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.DUNG2;
              this.mModeText.SetText(LevelManager.GetLocalizedName("ch_dungeons_ch2"));
              return;
            }
            this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
            this.mGameType = GameType.Campaign;
            this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
            return;
          case NetworkHostMessageBox.StoryChallengeType.DUNG2:
            this.storyChallengeType = NetworkHostMessageBox.StoryChallengeType.None;
            this.mGameType = GameType.Campaign;
            this.mModeText.SetText(LanguageManager.Instance.GetString(NetworkHostMessageBox.GAMETYPE_ADVENTURE_HASH));
            return;
          default:
            return;
        }
    }
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    if (this.mNameRect.Contains(iNewState.X, iNewState.Y))
      this.mSelectedPosition = 0;
    else if (this.mPasswordRect.Contains(iNewState.X, iNewState.Y))
      this.mSelectedPosition = 3;
    else if (this.mVACRect.Contains(iNewState.X, iNewState.Y))
      this.mSelectedPosition = 2;
    else if (this.mModeRect.Contains(iNewState.X, iNewState.Y))
      this.mSelectedPosition = 1;
    else if (this.mOkButton.InsideBounds((float) iNewState.X, (float) iNewState.Y))
      this.mSelectedPosition = 4;
    else if (this.mCancelButton.InsideBounds((float) iNewState.X, (float) iNewState.Y))
      this.mSelectedPosition = 5;
    else
      this.mSelectedPosition = -1;
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (iNewState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
      return;
    this.mSelectedPosition = !this.mNameRect.Contains(iNewState.X, iNewState.Y) ? (!this.mModeRect.Contains(iNewState.X, iNewState.Y) ? (!this.mPasswordRect.Contains(iNewState.X, iNewState.Y) ? (!this.mVACRect.Contains(iNewState.X, iNewState.Y) ? (!this.mOkButton.InsideBounds((float) iNewState.X, (float) iNewState.Y) ? (!this.mCancelButton.InsideBounds((float) iNewState.X, (float) iNewState.Y) ? -1 : 5) : 4) : 2) : 3) : 1) : 0;
    this.OnSelect((Controller) ControlManager.Instance.MenuController);
  }

  private void DaisyWheelInput(Controller iSender)
  {
    switch ((NetworkHostMessageBox.Items) this.mSelectedPosition)
    {
      case NetworkHostMessageBox.Items.Name:
        this.mEditName = true;
        string upper1 = LanguageManager.Instance.GetString(NetworkHostMessageBox.LOC_NAME).ToUpper();
        DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelInputRecived_Name));
        if (!DaisyWheel.TryShow(iSender, upper1, false))
        {
          DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
          break;
        }
        this.mName = new StringBuilder("");
        this.mNameText.SetText("");
        break;
      case NetworkHostMessageBox.Items.Password:
        this.mEditPassword = true;
        string upper2 = LanguageManager.Instance.GetString(NetworkHostMessageBox.LOC_PASSWORD).ToUpper();
        DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelInputRecived_Psw));
        if (!DaisyWheel.TryShow(iSender, upper2, false, GamepadTextInputLineMode.GamepadTextInputLineModeSingleLine, 11U))
        {
          DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
          break;
        }
        this.mPassword = new StringBuilder("");
        this.mPasswordText.SetText("");
        break;
    }
  }

  private void DaisyWheelInputRecived_Name(string name)
  {
    DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
    this.mName = new StringBuilder(name);
    this.mNameText.SetText(name);
    this.mEditName = false;
  }

  private void DaisyWheelInputRecived_Psw(string psw)
  {
    DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
    this.mPassword = new StringBuilder(psw);
    this.mPasswordText.SetText(psw);
    this.mEditPassword = false;
  }

  public override void OnSelect(Controller iSender)
  {
    if (this.mEditName)
      this.mEditName = false;
    else if (this.mEditPassword)
      this.mEditPassword = false;
    else if (!DaisyWheel.IsDisplaying && iSender is XInputController && (this.mSelectedPosition == 0 || this.mSelectedPosition == 3))
    {
      this.DaisyWheelInput(iSender);
    }
    else
    {
      if (DaisyWheel.IsDisplaying)
        return;
      switch (this.mSelectedPosition)
      {
        case 0:
          this.mEditName = true;
          break;
        case 1:
          this.ScrollGameMode_Right();
          break;
        case 2:
          if (HackHelper.LicenseStatus != HackHelper.Status.Valid)
            break;
          this.mVAC = !this.mVAC;
          this.mVACText.SetText(LanguageManager.Instance.GetString(this.mVAC ? InGameMenu.LOC_ON : InGameMenu.LOC_OFF));
          break;
        case 3:
          this.mEditPassword = true;
          break;
        case 4:
          if (this.mGameType == GameType.Campaign | this.mGameType == GameType.Mythos && LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Pending)
            break;
          if (this.mName.Length > 0)
          {
            if (this.mVAC && (this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos) && LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Hacked)
            {
              OptionsMessageBox optionsMessageBox = new OptionsMessageBox("#notice_mod_campaign_vac".GetHashCode(), new int[1]
              {
                "#add_menu_ok".GetHashCode()
              });
              LanguageManager.Instance.LanguageChanged -= new Action(((MessageBox) optionsMessageBox).LanguageChanged);
              optionsMessageBox.Show();
              break;
            }
            if (this.mComplete != null)
            {
              if (string.IsNullOrEmpty(this.mPassword.ToString()))
                this.mComplete(this.mGameType, this.storyChallengeType, this.mName.ToString(), this.mVAC, (string) null);
              else
                this.mComplete(this.mGameType, this.storyChallengeType, this.mName.ToString(), this.mVAC, this.mPassword.ToString());
            }
            SubMenuCharacterSelect.Instance.SetPlayerActive(iSender);
            GlobalSettings.Instance.GameName = this.mName.ToString();
            SaveManager.Instance.SaveSettings();
            this.Kill();
            break;
          }
          this.mNameError.Show();
          break;
        case 5:
          this.Kill();
          break;
      }
    }
  }

  public override void Draw(float iDeltaTime)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    base.Draw(iDeltaTime);
    this.mTimer -= iDeltaTime;
    while ((double) this.mTimer < 0.0)
    {
      this.mTimer += 0.5f;
      this.mLine = !this.mLine;
      if (this.mEditName)
      {
        if (this.mLine)
        {
          this.mNameText.Characters[this.mName.Length] = '_';
          this.mNameText.Characters[this.mName.Length + 1] = char.MinValue;
        }
        else
          this.mNameText.Characters[this.mName.Length] = char.MinValue;
      }
      else
        this.mNameText.Characters[this.mName.Length] = char.MinValue;
      if (this.mEditPassword)
      {
        if (this.mLine)
        {
          this.mPasswordText.Characters[this.mPassword.Length] = '_';
          this.mPasswordText.Characters[this.mPassword.Length + 1] = char.MinValue;
        }
        else
          this.mPasswordText.Characters[this.mPassword.Length] = char.MinValue;
      }
      else
        this.mPasswordText.Characters[this.mPassword.Length] = char.MinValue;
      this.mPasswordText.MarkAsDirty();
      this.mNameText.MarkAsDirty();
    }
    float lineHeight = (float) this.mFont.LineHeight;
    float num1 = (float) Math.Floor((double) lineHeight * 1.5);
    Vector4 color = MenuItem.COLOR;
    color.W *= this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = color;
    Vector2 mCenter = this.mCenter;
    mCenter.Y -= (float) ((double) this.mSize.Y * 0.5 - 48.0);
    this.mMessage.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    mCenter.Y += num1;
    this.mNameRect.X = (int) mCenter.X - 160 /*0xA0*/;
    this.mNameRect.Y = (int) mCenter.Y;
    Vector4 vector4 = this.mSelectedPosition == 0 ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    vector4.W *= this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4;
    this.mNameTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    mCenter.Y += lineHeight;
    float x1 = this.mNameText.Font.MeasureText(this.mNameText.Characters, true, this.mName.Length).X;
    Matrix iTransform = new Matrix();
    float num2 = Math.Min(320f / x1, 1f);
    iTransform.M11 = num2;
    iTransform.M22 = 1f;
    iTransform.M44 = 1f;
    iTransform.M41 = (float) ((double) mCenter.X - (double) x1 * (double) num2 * 0.5 + 0.5);
    iTransform.M42 = mCenter.Y;
    vector4 = this.mSelectedPosition == 0 | this.mEditName ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    vector4.W *= this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4;
    this.mNameText.Draw(MessageBox.sGUIBasicEffect, ref iTransform);
    mCenter.Y += num1;
    vector4 = this.mSelectedPosition == 1 ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    vector4.W *= this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4;
    this.mModeRect.X = (int) mCenter.X - 160 /*0xA0*/;
    this.mModeRect.Y = (int) mCenter.Y;
    this.mModeTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    mCenter.Y += lineHeight;
    this.mModeText.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    mCenter.Y += num1;
    vector4 = HackHelper.LicenseStatus != HackHelper.Status.Valid || this.mSelectedPosition != 2 ? MenuItem.COLOR : MenuItem.COLOR_SELECTED;
    vector4.W *= this.mAlpha;
    if (HackHelper.LicenseStatus != HackHelper.Status.Valid)
      vector4.W *= 0.5f;
    MessageBox.sGUIBasicEffect.Color = vector4;
    this.mVACRect.X = (int) mCenter.X - 160 /*0xA0*/;
    this.mVACRect.Y = (int) mCenter.Y;
    this.mVACRect.Width = 320;
    this.mVACTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    mCenter.Y += lineHeight;
    this.mVACText.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    mCenter.Y += num1;
    this.mPasswordRect.X = (int) mCenter.X - 160 /*0xA0*/;
    this.mPasswordRect.Y = (int) mCenter.Y;
    this.mPasswordRect.Width = 320;
    vector4 = this.mSelectedPosition == 3 ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    vector4.W *= this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4;
    this.mPasswordTitle.Draw(MessageBox.sGUIBasicEffect, mCenter.X, mCenter.Y);
    mCenter.Y += lineHeight;
    float x2 = this.mPasswordText.Font.MeasureText(this.mPasswordText.Characters, true, this.mPassword.Length).X;
    vector4 = this.mSelectedPosition == 3 | this.mEditPassword ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    vector4.W *= this.mAlpha * this.mAlpha;
    MessageBox.sGUIBasicEffect.Color = vector4;
    this.mPasswordText.Draw(MessageBox.sGUIBasicEffect, mCenter.X - (float) Math.Floor((double) x2 * 0.5), mCenter.Y);
    this.mOkButton.Selected = this.mSelectedPosition == 4;
    mCenter.X = this.mCenter.X - 16f;
    mCenter.Y = (float) ((double) this.mCenter.Y + (double) this.mSize.Y * 0.5 - 64.0);
    this.mOkButton.Position = mCenter;
    this.mOkButton.Alpha = this.mAlpha;
    this.mOkButton.Draw(MessageBox.sGUIBasicEffect);
    this.mCancelButton.Selected = this.mSelectedPosition == 5;
    mCenter.X = this.mCenter.X + 16f;
    this.mCancelButton.Position = mCenter;
    this.mCancelButton.Alpha = this.mAlpha;
    this.mCancelButton.Draw(MessageBox.sGUIBasicEffect);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }

  public enum StoryChallengeType
  {
    None,
    Vietnam,
    OSOTC,
    DUNG1,
    DUNG2,
  }

  private enum Items
  {
    Invalid = -1, // 0xFFFFFFFF
    Name = 0,
    Mode = 1,
    VAC = 2,
    Password = 3,
    Ok = 4,
    Cancel = 5,
    NrOfItems = 6,
  }

  public delegate void Complete(
    GameType iType,
    NetworkHostMessageBox.StoryChallengeType iStoryChallangeType,
    string iName,
    bool iVAC,
    string iPassword);
}
