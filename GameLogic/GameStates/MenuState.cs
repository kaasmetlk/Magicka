// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.MenuState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.GameStates.Menu.Main.Options;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Network;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Threading;

#nullable disable
namespace Magicka.GameLogic.GameStates;

internal class MenuState : GameState
{
  private static readonly int MUSIC_MENU = "music_menu".GetHashCodeCustom();
  private static readonly int MENU_AMBIENCE = "amb_library_menu".GetHashCodeCustom();
  private static MenuState mSingelton;
  private static volatile object mSingeltonLock = new object();
  private static readonly Vector3 CAMERA_STARTPOS = new Vector3(-1f, 0.0f, 0.0f);
  private static float mCameraSpeed;
  private static Vector3 mPosition;
  private bool mStoreShowing;
  private float mStorePosition = 1.25f;
  private float mFindGamepadsTimer = 1f;

  public static MenuState Instance
  {
    get
    {
      if (MenuState.mSingelton == null)
      {
        lock (MenuState.mSingeltonLock)
        {
          if (MenuState.mSingelton == null)
            MenuState.mSingelton = new MenuState();
        }
      }
      return MenuState.mSingelton;
    }
  }

  private MenuState()
    : base(new Camera(MenuState.CAMERA_STARTPOS, Vector3.Right, Vector3.Up, 0.7853982f, 1.77777779f, 1f, 500f))
  {
  }

  public void Initialize()
  {
  }

  public override void OnEnter()
  {
    this.mStoreShowing = false;
    this.mStorePosition = 1.25f;
    AudioManager instance = AudioManager.Instance;
    while ((instance.InitializedBanks & (Banks.Music | Banks.UI)) != (Banks.Music | Banks.UI))
      Thread.Sleep(1);
    AudioManager.Instance.PlayMusic(Banks.Music, MenuState.MUSIC_MENU, new float?());
    Vector4 iBackgroundColor = new Vector4();
    iBackgroundColor.W = 0.75f;
    Vector4 iForegroundColor = new Vector4();
    iForegroundColor.X = iForegroundColor.Y = iForegroundColor.Z = 1f;
    iForegroundColor.W = 1f;
    ToolTipMan.Instance.Initialize(400, ref iBackgroundColor, ref iForegroundColor);
    MenuState.mCameraSpeed = 1f / 1000f;
    if (SaveManager.Instance.AlreadyLoaded)
      SaveManager.Instance.SaveLeaderBoards();
    else
      Magicka.Game.Instance.AddLoadTask(new Action(SaveManager.Instance.LoadLeaderBoards));
    if (GlobalSettings.Instance.StartupLobby == 0UL)
      return;
    Tome.Instance.ControllerA((Controller) null);
  }

  public override void OnExit()
  {
    ToolTipMan.Instance.KillAll(true);
    ControlManager.Instance.ClearControllers();
  }

  public void SetCurrentPosition(Vector3 iNewPosition, float iSpeed)
  {
    MenuState.mCameraSpeed = iSpeed;
    MenuState.mPosition = iNewPosition;
  }

  private bool isApproximately(float a, float b, float precision)
  {
    return (double) Math.Abs(a - b) <= (double) precision;
  }

  private bool isApproximately(Vector3 a, Vector3 b, float precision)
  {
    return this.isApproximately(a.X, b.X, precision) && this.isApproximately(a.Y, b.Y, precision) && this.isApproximately(a.Z, b.Z, precision);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if ((double) this.mFindGamepadsTimer > 0.0)
    {
      this.mFindGamepadsTimer -= iDeltaTime;
      if ((double) this.mFindGamepadsTimer <= 0.0)
        Magicka.Game.Instance.AddLoadTask(new Action(this.FindNewControllers));
    }
    Tome.Instance.Update(iDataChannel, iDeltaTime);
    GamePadMenuHelp.Instance.Update(iDataChannel, iDeltaTime);
    this.mScene.Camera.Update(iDataChannel, iDeltaTime);
    Matrix oMatrix;
    this.mScene.Camera.GetViewProjectionMatrix(iDataChannel, out oMatrix);
    DialogManager.Instance.Update(iDataChannel, iDeltaTime, ref oMatrix);
    ToolTipMan.Instance.Update(this.mScene, iDataChannel, iDeltaTime);
  }

  private void FindNewControllers()
  {
    ControlManager.Instance.FindNewGamePads();
    if (GamePadConfigMessageBox.Instance.Dead)
    {
      for (int index = 0; index < ControlManager.Instance.DInputPads.Count; ++index)
      {
        if (!ControlManager.Instance.DInputPads[index].Configured)
        {
          GamePadConfigMessageBox.Instance.GamePad = ControlManager.Instance.DInputPads[index];
          GamePadConfigMessageBox.Instance.Show();
          break;
        }
      }
    }
    SubMenuOptionsControls.Instance.UpdateControllers();
    this.mFindGamepadsTimer = 5f;
  }

  public bool TomeTakesInput => !this.mStoreShowing && (double) this.mStorePosition >= 1.0;

  public bool StoreTakesInput
  {
    get => this.mStoreShowing && (double) this.mStorePosition <= 9.9999999747524271E-07;
  }

  internal void NetworkInput(ref MenuSelectMessage iMessage)
  {
    switch (iMessage.IntendedMenu)
    {
      case MenuSelectMessage.MenuType.CharacterSelect:
        SubMenuCharacterSelect.Instance.NetworkInput(ref iMessage);
        break;
      case MenuSelectMessage.MenuType.Statistics:
        SubMenuEndGame.Instance.NetworkInput(ref iMessage);
        break;
      default:
        throw new Exception("Invalid target menu!");
    }
  }
}
