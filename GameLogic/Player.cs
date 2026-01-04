// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Player
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using PolygonHead;
using SteamWrapper;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic;

public class Player
{
  private static List<Player.QueuedGamer> sWaitingGamers = new List<Player.QueuedGamer>(4);
  public static readonly int[] UNIQUE_ID = new int[4]
  {
    "player1".GetHashCodeCustom(),
    "player2".GetHashCodeCustom(),
    "player3".GetHashCodeCustom(),
    "player4".GetHashCodeCustom()
  };
  public static readonly int ALUCART_UNIQUE_ID = "wizard_alucart".GetHashCodeCustom();
  private Gamer mGamer;
  private int mID;
  private WeakReference mAvatar;
  private StaticList<int> mInputQueue;
  private SpellWheel mSpellWheel;
  private IconRenderer mIconRenderer;
  private TextBox mObtainedTextBox;
  private float mDeadTimer;
  protected ulong mUnlockedMagicks;
  protected NotifierButton mNotifierButton;
  private bool mRessing;

  public string GamerTag => this.mGamer.GamerTag;

  public string Weapon { get; set; }

  public string Staff { get; set; }

  public bool Ressing
  {
    get => this.mRessing;
    set => this.mRessing = value;
  }

  public Player(int iID)
  {
    this.mID = iID;
    this.mInputQueue = (StaticList<int>) new StaticEquatableList<int>(5);
    this.mAvatar = new WeakReference((object) null);
  }

  public void InitializeGame(PlayState iPlayState)
  {
    this.mDeadTimer = 0.0f;
    if (this.mSpellWheel == null)
      this.mSpellWheel = new SpellWheel(this, iPlayState);
    else
      this.mSpellWheel.Initialize(iPlayState);
    if (this.mNotifierButton == null)
      this.mNotifierButton = new NotifierButton();
    if (this.mIconRenderer == null)
      this.mIconRenderer = new IconRenderer(this, iPlayState);
    else
      this.mIconRenderer.Initialize(iPlayState);
    if (this.mObtainedTextBox != null)
      return;
    this.mObtainedTextBox = new TextBox();
  }

  public void DeinitializeGame()
  {
  }

  public bool Playing { get; set; }

  public byte Color
  {
    get
    {
      if (this.Team == Factions.TEAM_RED)
        return 0;
      if (this.Team == Factions.TEAM_BLUE)
        return 3;
      return this.mGamer == null ? (byte) 0 : this.mGamer.Color;
    }
    set
    {
      this.mGamer.Color = value;
      if (this.mGamer is NetworkGamer || NetworkManager.Instance.State == NetworkState.Offline)
        return;
      GamerChangedMessage iMessage = new GamerChangedMessage(this);
      NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
    }
  }

  public NotifierButton NotifierButton => this.mNotifierButton;

  public TextBox ObtainedTextBox => this.mObtainedTextBox;

  public Gamer Gamer
  {
    get => this.mGamer;
    set => this.mGamer = value;
  }

  public Avatar Avatar
  {
    get => this.mAvatar.Target as Avatar;
    set => this.mAvatar.Target = (object) value;
  }

  public SpellWheel SpellWheel => this.mSpellWheel;

  public IconRenderer IconRenderer => this.mIconRenderer;

  public Factions Team { get; set; }

  public int ID
  {
    get => this.mID;
    set => this.mID = value;
  }

  public float DeadAge => this.mDeadTimer;

  internal Controller Controller { get; set; }

  public StaticList<int> InputQueue => this.mInputQueue;

  public StaticList<Spell> SpellQueue
  {
    get => this.Avatar == null ? (StaticList<Spell>) null : this.Avatar.SpellQueue;
  }

  public void ForceSetNotDead() => this.Avatar.ForceSetNotDead();

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mAvatar.Target == null)
      return;
    if (this.Avatar.Dead)
    {
      this.mDeadTimer += iDeltaTime;
    }
    else
    {
      this.mDeadTimer = 0.0f;
      if (GlobalSettings.Instance.SpellWheel != SettingOptions.On || this.Avatar.PlayState == null || this.Avatar.PlayState.IsInCutscene)
        return;
      if (this.Controller is DirectInputController | this.Controller is XInputController)
        this.mSpellWheel.Update(iDataChannel, iDeltaTime);
      this.mIconRenderer.Update(iDataChannel, iDeltaTime);
    }
  }

  public void Leave()
  {
    if (!this.Playing)
      return;
    this.Playing = false;
    if (this.Avatar != null && !this.Avatar.Dead)
      this.Avatar.Kill();
    this.Avatar = (Avatar) null;
    if (this.Controller != null)
      this.Controller.Player = (Player) null;
    this.Controller = (Controller) null;
    this.Gamer = (Gamer) null;
    this.Team = Factions.NONE;
    SubMenuCharacterSelect.Instance.UpdateGamer(this, (Gamer) null);
    if (NetworkManager.Instance.State == NetworkState.Offline)
      return;
    GamerLeaveMessage iMessage;
    iMessage.Id = (byte) this.mID;
    NetworkManager.Instance.Interface.SendMessage<GamerLeaveMessage>(ref iMessage);
  }

  internal static Player Join(Controller iController, int iIndex, Gamer iGamer)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
    {
      GamerJoinRequestMessage iMessage;
      iMessage.Gamer = iGamer.GamerTag;
      iMessage.AvatarThumb = iGamer.Avatar.ThumbPath;
      iMessage.AvatarPortrait = iGamer.Avatar.PortraitPath;
      iMessage.AvatarType = iGamer.Avatar.TypeName;
      iMessage.Color = iGamer.Color;
      iMessage.Id = (sbyte) iIndex;
      iMessage.SteamID = SteamUser.GetSteamID();
      NetworkManager.Instance.Interface.SendMessage<GamerJoinRequestMessage>(ref iMessage, 0);
      Player.sWaitingGamers.Add(new Player.QueuedGamer(iController, iGamer));
    }
    else
    {
      Player[] players = Game.Instance.Players;
      if (iIndex < 0)
      {
        if (iGamer != Gamer.INVALID_GAMER)
        {
          for (int index = 0; index < players.Length; ++index)
          {
            if (players[index].Gamer == iGamer)
            {
              Player iPlayer = players[index];
              Controller controller = iPlayer.Controller;
              if (controller != null)
              {
                controller.Player.Controller = (Controller) null;
                controller.Player = (Player) null;
              }
              if (!(iGamer is NetworkGamer) && NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(iPlayer.Gamer.Avatar) != HackHelper.License.Yes)
              {
                iPlayer.Gamer.Avatar = Profile.Instance.DefaultAvatar;
                if (NetworkManager.Instance.State != NetworkState.Offline)
                {
                  GamerChangedMessage iMessage = new GamerChangedMessage(iPlayer);
                  NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
                }
              }
              iPlayer.Controller = iController;
              iController.Player = iPlayer;
              iPlayer.Playing = true;
              Player.ServerPlayerJoin(iPlayer);
              return iPlayer;
            }
          }
        }
        for (int index = 0; index < players.Length; ++index)
        {
          if (!players[index].Playing)
          {
            Player iPlayer = players[index];
            Controller controller = iPlayer.Controller;
            if (controller != null)
            {
              controller.Player.Controller = (Controller) null;
              controller.Player = (Player) null;
            }
            iPlayer.Gamer = iGamer;
            if (!(iGamer is NetworkGamer) && NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(iPlayer.Gamer.Avatar) != HackHelper.License.Yes)
            {
              iPlayer.Gamer.Avatar = Profile.Instance.DefaultAvatar;
              if (NetworkManager.Instance.State != NetworkState.Offline)
              {
                GamerChangedMessage iMessage = new GamerChangedMessage(iPlayer);
                NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
              }
            }
            iPlayer.Controller = iController;
            if (iController != null)
              iController.Player = iPlayer;
            iPlayer.Playing = true;
            Player.ServerPlayerJoin(iPlayer);
            return iPlayer;
          }
        }
      }
      else
      {
        if (iGamer != Gamer.INVALID_GAMER && players[iIndex].Gamer == iGamer)
        {
          Player iPlayer = players[iIndex];
          Controller controller = iPlayer.Controller;
          if (controller != null)
          {
            controller.Player.Controller = (Controller) null;
            controller.Player = (Player) null;
          }
          iPlayer.Controller = iController;
          iController.Player = iPlayer;
          if (!(iGamer is NetworkGamer) && NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(iPlayer.Gamer.Avatar) != HackHelper.License.Yes)
          {
            iPlayer.Gamer.Avatar = Profile.Instance.DefaultAvatar;
            if (NetworkManager.Instance.State != NetworkState.Offline)
            {
              GamerChangedMessage iMessage = new GamerChangedMessage(iPlayer);
              NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
            }
          }
          iPlayer.Playing = true;
          Player.ServerPlayerJoin(iPlayer);
          return iPlayer;
        }
        if (!players[iIndex].Playing)
        {
          Player iPlayer = players[iIndex];
          Controller controller = iPlayer.Controller;
          if (controller != null)
          {
            controller.Player.Controller = (Controller) null;
            controller.Player = (Player) null;
          }
          iPlayer.Gamer = iGamer;
          if (!(iGamer is NetworkGamer) && NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(iPlayer.Gamer.Avatar) != HackHelper.License.Yes)
          {
            iPlayer.Gamer.Avatar = Profile.Instance.DefaultAvatar;
            if (NetworkManager.Instance.State != NetworkState.Offline)
            {
              GamerChangedMessage iMessage = new GamerChangedMessage(iPlayer);
              NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
            }
          }
          iPlayer.Controller = iController;
          if (iController != null)
            iController.Player = iPlayer;
          iPlayer.Playing = true;
          Player.ServerPlayerJoin(iPlayer);
          return iPlayer;
        }
      }
    }
    return (Player) null;
  }

  internal static void JoinServerGranted(ref GamerJoinAcceptMessage iMessage)
  {
    Player player = Game.Instance.Players[(int) iMessage.Id];
    long ticks = DateTime.Now.Ticks;
    bool flag = false;
    for (int index = 0; index < Player.sWaitingGamers.Count; ++index)
    {
      if (Player.sWaitingGamers[index].Gamer.GamerTag.Equals(iMessage.Gamer))
      {
        player.Gamer = Player.sWaitingGamers[index].Gamer;
        if (player.Controller != null)
          player.Controller.Player = (Player) null;
        player.Controller = Player.sWaitingGamers[index].Controller;
        player.Controller.Player = player;
        player.Playing = true;
        if (NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(player.Gamer.Avatar) != HackHelper.License.Yes)
        {
          player.Gamer.Avatar = Profile.Instance.DefaultAvatar;
          if (NetworkManager.Instance.State != NetworkState.Offline)
          {
            GamerChangedMessage iMessage1 = new GamerChangedMessage(player);
            NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage1);
          }
        }
        SubMenuCharacterSelect.Instance.UpdateGamer(player, player.Gamer);
        Player.sWaitingGamers.RemoveAt(index);
        flag = true;
        Player.sWaitingGamers.Clear();
        break;
      }
      if (ticks - Player.sWaitingGamers[index].RequestTime > 10000000L)
      {
        Player.sWaitingGamers.RemoveAt(index);
        --index;
      }
    }
    if (flag)
      return;
    GamerLeaveMessage iMessage2;
    iMessage2.Id = (byte) iMessage.Id;
    NetworkManager.Instance.Interface.SendMessage<GamerLeaveMessage>(ref iMessage2);
  }

  private static void ServerPlayerJoin(Player iPlayer)
  {
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    GamerJoinRequestMessage iMessage;
    iMessage.Id = (sbyte) iPlayer.ID;
    iMessage.Gamer = iPlayer.mGamer.GamerTag;
    iMessage.AvatarThumb = iPlayer.mGamer.Avatar.ThumbPath;
    iMessage.AvatarPortrait = iPlayer.mGamer.Avatar.PortraitPath;
    iMessage.AvatarType = iPlayer.mGamer.Avatar.TypeName;
    iMessage.Color = iPlayer.mGamer.Color;
    iMessage.SteamID = !(iPlayer.mGamer is NetworkGamer) ? SteamGameServer.GetSteamID() : (iPlayer.mGamer as NetworkGamer).ClientID;
    NetworkManager.Instance.Interface.SendMessage<GamerJoinRequestMessage>(ref iMessage);
  }

  public ulong UnlockedMagicks
  {
    get => this.mUnlockedMagicks;
    set
    {
      this.mUnlockedMagicks = value;
      if (this.mGamer == null || this.mGamer is NetworkGamer || NetworkManager.Instance.State != NetworkState.Server)
        return;
      GamerChangedMessage iMessage = new GamerChangedMessage(this);
      NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
    }
  }

  private struct QueuedGamer
  {
    public Controller Controller;
    public Gamer Gamer;
    public long RequestTime;

    public QueuedGamer(Controller iController, Gamer iGamer)
    {
      this.Controller = iController;
      this.Gamer = iGamer;
      this.RequestTime = DateTime.Now.Ticks;
    }
  }
}
