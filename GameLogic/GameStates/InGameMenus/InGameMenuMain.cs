// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuMain
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuMain : InGameMenu
{
  private const string OPTION_BANANA = "banana";
  private const string OPTION_RESUME = "resume";
  private const string OPTION_RESTART = "restart_level";
  private const string OPTION_MAGICKS = "magicks";
  private const string OPTION_OPTIONS = "options";
  private const string OPTION_CHEATS = "cheats";
  private const string OPTION_QUIT = "quit";
  private static readonly string[] OPTION_STRINGS = new string[6]
  {
    "banana",
    "resume",
    "restart_level",
    "magicks",
    "options",
    "quit"
  };
  private static InGameMenuMain sSingelton;
  private static volatile object sSingeltonLock = new object();
  private InGameMenuAreYouSure mSkipCreditsMenu;
  private InGameMenuAreYouSure mRestartMenu;
  private InGameMenuAreYouSure mQuitMenu;
  private InGameMenuAreYouSure mTutorialSkipMenu;
  private InGameMenuAreYouSure mDisconnectMenu;

  public static InGameMenuMain Instance
  {
    get
    {
      if (InGameMenuMain.sSingelton == null)
      {
        lock (InGameMenuMain.sSingeltonLock)
        {
          if (InGameMenuMain.sSingelton == null)
            InGameMenuMain.sSingelton = new InGameMenuMain();
        }
      }
      return InGameMenuMain.sSingelton;
    }
  }

  private InGameMenuMain()
  {
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.AddMenuTextItem("#banana".GetHashCodeCustom(), font, TextAlign.Center);
    this.AddMenuTextItem("#menu_resume".GetHashCodeCustom(), font, TextAlign.Center);
    this.AddMenuTextItem(Defines.LOC_GEN_RESTART, font, TextAlign.Center);
    this.AddMenuTextItem("#menu_tome_02".GetHashCodeCustom(), font, TextAlign.Center);
    this.AddMenuTextItem("#menu_main_06".GetHashCodeCustom(), font, TextAlign.Center);
    this.AddMenuTextItem("#menu_main_07".GetHashCodeCustom(), font, TextAlign.Center);
    this.mBackgroundSize = new Vector2(400f, 400f);
    this.mRestartMenu = new InGameMenuAreYouSure(new Action(this.RestartCallback), new int[2]
    {
      "#add_menu_rus".GetHashCodeCustom(),
      "#add_menu_rus_quitlev".GetHashCodeCustom()
    });
    this.mQuitMenu = new InGameMenuAreYouSure(new Action(this.QuitCallback), new int[2]
    {
      "#add_menu_rus".GetHashCodeCustom(),
      "#add_menu_rus_quitlev".GetHashCodeCustom()
    });
    this.mTutorialSkipMenu = new InGameMenuAreYouSure(new Action(this.TutorialSkipCallback), new int[1]
    {
      "#banana_sure".GetHashCodeCustom()
    });
    this.mDisconnectMenu = new InGameMenuAreYouSure(new Action(this.DisconnetCallback), new int[1]
    {
      "#add_menu_rus".GetHashCodeCustom()
    });
    this.mSkipCreditsMenu = new InGameMenuAreYouSure(new Action(this.SkipCreditsCallback), new int[1]
    {
      "#add_menu_rus".GetHashCodeCustom()
    });
  }

  public override void UpdatePositions()
  {
    Vector2 vector2 = new Vector2();
    vector2.X = InGameMenu.sScreenSize.X * 0.5f;
    vector2.Y = 290f * InGameMenu.sScale;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      if (index != 0 || InGameMenu.sPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH)
      {
        MenuItem mMenuItem = this.mMenuItems[index];
        mMenuItem.Scale = InGameMenu.sScale;
        mMenuItem.Position = vector2;
        vector2.Y += mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y;
      }
    }
    this.mRestartMenu.UpdatePositions();
    this.mQuitMenu.UpdatePositions();
    this.mTutorialSkipMenu.UpdatePositions();
    this.mDisconnectMenu.UpdatePositions();
    this.mSkipCreditsMenu.UpdatePositions();
  }

  private void QuitCallback()
  {
    GameEndMessage gameEndMessage;
    gameEndMessage.Condition = EndGameCondition.ChallengeExit;
    gameEndMessage.Argument = 1;
    gameEndMessage.Phony = false;
    gameEndMessage.DelayTime = 0.0f;
    InGameMenu.sPlayState.Endgame(ref gameEndMessage);
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      (NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
      NetworkManager.Instance.Interface.SendMessage<GameEndMessage>(ref gameEndMessage);
    }
    InGameMenu.HideInstant();
    if (!TutorialUtils.IsInProgress)
      return;
    TutorialUtils.Quit();
  }

  private void RestartCallback()
  {
    InGameMenu.sPlayState.Restart((object) this, RestartType.StartOfLevel);
    InGameMenu.HideInstant();
    if (!TutorialUtils.IsInProgress)
      return;
    TutorialUtils.Restart();
  }

  private void TutorialSkipCallback()
  {
    InGameMenu.Hide();
    InGameMenu.sPlayState.Level.CurrentScene.ExecuteTrigger("skip_tutorial".GetHashCodeCustom(), (Magicka.GameLogic.Entities.Character) null, true);
  }

  private void DisconnetCallback()
  {
    NetworkManager.Instance.EndSession();
    while (!(Tome.Instance.CurrentMenu is SubMenuOnline))
      Tome.Instance.PopMenuInstant();
    if (Credits.Instance.IsActive)
      InGameMenu.sPlayState.Endgame(EndGameCondition.LevelComplete, false, false, 0.0f);
    else
      InGameMenu.sPlayState.Endgame(EndGameCondition.Disconnected, false, false, 0.0f);
    GameStateManager.Instance.PopState();
    InGameMenu.HideInstant();
  }

  private void SkipCreditsCallback()
  {
    GameEndMessage gameEndMessage;
    gameEndMessage.Condition = EndGameCondition.ChallengeExit;
    gameEndMessage.Argument = 1;
    gameEndMessage.Phony = false;
    gameEndMessage.DelayTime = 0.0f;
    InGameMenu.sPlayState.Endgame(ref gameEndMessage);
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      (NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
      NetworkManager.Instance.Interface.SendMessage<GameEndMessage>(ref gameEndMessage);
    }
    SubMenuCharacterSelect.Instance.OnEnter();
    InGameMenu.sPlayState.Endgame(EndGameCondition.LevelComplete, false, false, 0.0f);
    GameStateManager.Instance.PopState();
    InGameMenu.HideInstant();
  }

  protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
  {
    base.IControllerMove(iSender, iDirection);
  }

  protected override string IGetHighlightedButtonName()
  {
    return InGameMenuMain.OPTION_STRINGS[this.mSelectedItem];
  }

  protected override void IControllerSelect(Controller iSender)
  {
    switch (this.mSelectedItem)
    {
      case 0:
        if (InGameMenu.sPlayState.Level.CurrentScene.ID != Defines.WC_3_HASH)
          break;
        InGameMenu.PushMenu((InGameMenu) this.mTutorialSkipMenu);
        break;
      case 1:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
        InGameMenu.Hide();
        break;
      case 2:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        InGameMenu.PushMenu((InGameMenu) this.mRestartMenu);
        break;
      case 3:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        InGameMenu.PushMenu((InGameMenu) InGameMenuMagicks.Instance);
        break;
      case 4:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        InGameMenu.PushMenu((InGameMenu) InGameMenuOptions.Instance);
        break;
      case 5:
        if (NetworkManager.Instance.State == NetworkState.Client)
        {
          InGameMenu.PushMenu((InGameMenu) this.mDisconnectMenu);
          break;
        }
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        if (Credits.Instance.IsActive)
        {
          InGameMenu.PushMenu((InGameMenu) this.mSkipCreditsMenu);
          break;
        }
        InGameMenu.PushMenu((InGameMenu) this.mQuitMenu);
        break;
    }
  }

  protected override void IControllerBack(Controller iSender) => InGameMenu.Hide();

  protected override void OnEnter()
  {
    if (InGameMenu.sController is KeyboardMouseController)
      this.mSelectedItem = -1;
    else
      this.mSelectedItem = 0;
    if (NetworkManager.Instance.State == NetworkState.Client)
      (this.mMenuItems[this.mMenuItems.Count - 1] as MenuTextItem).SetText("#network_10".GetHashCodeCustom());
    else if (Credits.Instance.IsActive)
      (this.mMenuItems[this.mMenuItems.Count - 1] as MenuTextItem).SetText("#credits_skip".GetHashCodeCustom());
    else
      (this.mMenuItems[this.mMenuItems.Count - 1] as MenuTextItem).SetText("#menu_main_07".GetHashCodeCustom());
  }

  protected override void IUpdate(DataChannel iDataChannel, float iDeltaTime)
  {
    base.IUpdate(iDataChannel, iDeltaTime);
    this.mMenuItems[2].Enabled = NetworkManager.Instance.State != NetworkState.Client;
    this.mMenuItems[0].Enabled = InGameMenu.sPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH && NetworkManager.Instance.State != NetworkState.Client;
  }

  protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
  {
    Vector4 vector4_1 = new Vector4();
    vector4_1.X = vector4_1.Y = vector4_1.Z = 1f;
    vector4_1.W = this.mAlpha;
    Vector4 vector4_2 = new Vector4();
    vector4_2.X = vector4_2.Y = vector4_2.Z = 0.0f;
    vector4_2.W = this.mAlpha;
    Vector4 vector4_3 = new Vector4();
    vector4_3.X = vector4_3.Y = vector4_3.Z = 0.4f;
    vector4_3.W = this.mAlpha;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      if (InGameMenu.sPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH || index != 0)
      {
        MenuItem mMenuItem = this.mMenuItems[index];
        mMenuItem.Color = vector4_1;
        mMenuItem.ColorSelected = vector4_2;
        mMenuItem.ColorDisabled = vector4_3;
        mMenuItem.Selected = mMenuItem.Enabled & this.mSelectedItem == index;
        if (mMenuItem.Selected)
        {
          InGameMenu.sEffect.Transform = new Matrix()
          {
            M44 = 1f,
            M11 = iBackgroundSize.X * InGameMenu.sScale,
            M22 = mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y,
            M41 = mMenuItem.Position.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale,
            M42 = mMenuItem.TopLeft.Y
          };
          Vector4 vector4_4 = new Vector4();
          vector4_4.X = vector4_4.Y = vector4_4.Z = 1f;
          vector4_4.W = 0.8f * this.mAlpha;
          InGameMenu.sEffect.Color = vector4_4;
          InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
          InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
          InGameMenu.sEffect.CommitChanges();
          InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
        }
      }
    }
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      if (InGameMenu.sPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH || index != 0)
        this.mMenuItems[index].Draw(InGameMenu.sEffect);
    }
  }

  protected override void OnExit()
  {
  }
}
