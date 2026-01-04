// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.PlayState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.Achievements;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates.InGameMenus;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Levels.Triggers;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.Network;
using Magicka.Physics;
using Magicka.Storage;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using PolygonHead.ParticleEffects;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;

#nullable disable
namespace Magicka.GameLogic.GameStates;

public class PlayState : GameState, IDisposable
{
  public const float NETWORK_UPDATE_PERIOD = 0.0333333351f;
  public static readonly Point CHAT_SIZE = new Point(600, 200);
  public static readonly float CUTSCENE_BLACKBAR_SIZE = 0.1f;
  public static readonly int STARTHASHCODE = "start".GetHashCodeCustom();
  public static readonly int MUSIC_DEFEAT = "music_defeat".GetHashCodeCustom();
  public static readonly int MUSIC_VICTORY = "music_fanfare".GetHashCodeCustom();
  private static PlayState sRecentPlayState;
  private PlayState.State mLevelStartState;
  private PlayState.State mCheckpointState;
  private GameType mGameType;
  private double mPlayTime;
  private float mTimeModifier = 1f;
  private float mTimeMultiplier = 1f;
  private EntityManager mEntityManager;
  private string mLevelFileName;
  private Level mLevel;
  private ContentManager mContent;
  private static bool sWaitingForPlayers = true;
  private List<SpellEffect> mSpellEffects;
  private Text mEndGameText;
  private float mEndGameTextAlpha;
  private bool mInitialized;
  private bool mHasEntered;
  private bool mIsPaused;
  private bool mOverlayIsPaused;
  private bool mUIEnabled = true;
  private MagickCamera mCamera;
  private BossFight mBossFight;
  private GenericHealthBar mGenericHealtBar = new GenericHealthBar((Scene) null);
  private bool mPlayerStaffLight;
  private bool mEndGameActive;
  private bool mEndGameMusicActive;
  private bool mEndGamePhony;
  private float mEndGameTimer;
  private EndGameCondition mEndGameCondition;
  private bool mBusy = true;
  private float mNetworkUpdateTimer = 0.0333333351f;
  private PlayState.RenderData[] mRenderData;
  private bool mCutscene;
  private bool mCutsceneSkipped;
  private int mCutsceneSkipTrigger;
  private bool mShowCutsceneSkipTip;
  private float mCutsceneSkipTipAlpha;
  private bool mCutsceneSkipRemoveBars;
  private float mCutsceneTimer;
  private PlayState.CutsceneRenderData[] mCutsceneRenderData;
  private bool[] mTooFancyForFireballs;
  private bool[] mHasUsedMagick;
  private HitList[] mItsRainingBeastMen;
  private uint mBlizzardRainCount;
  private bool mDiedInLevel;
  private InventoryBox mInventoryBox;
  private string mLoadingTip;
  private bool mShowProgress = true;
  private Cue mCueToFinish;
  private bool[] mSpawnFairies = new bool[4];
  private Banks mMusicBank;
  private int mMusicCue;
  private float? mMusicFocusValue;
  private SpawnPoint? mSpawnPoint;
  private SaveData mSaveSlot;
  private SaveSlotInfo mInfo;
  private MemoryStream mCheckpointStream;
  private VersusRuleset.Settings mSettings;
  private byte[] mLevelHash;
  private byte[] mDialogHash;
  private LoadingScreen loadingScreen;
  private float StartupActionsPercentageAtStart;
  private float StartupActionsPercentageTarget;

  public static PlayState RecentPlayState => PlayState.sRecentPlayState;

  public bool HasEntered => this.mHasEntered;

  internal PlayState(
    bool iCustom,
    string iLevelFileName,
    GameType iGameType,
    SpawnPoint? iSpawnPoint,
    SaveData iSaveSlot,
    VersusRuleset.Settings iSettings)
    : base((PolygonHead.Camera) new MagickCamera(MagickCamera.CAMERAOFFSET, Vector3.Forward, Vector3.Up, MagickCamera.DEFAULTFOV, 1.77777779f, MagickCamera.NEARCLIP, MagickCamera.FARCLIP))
  {
    this.mGameType = iGameType;
    this.mSpawnPoint = iSpawnPoint;
    this.mSettings = iSettings;
    this.mSaveSlot = iSaveSlot;
    SubMenuEndGame.Instance.Set(this.mGameType == GameType.Mythos | this.mGameType == GameType.Campaign, iSaveSlot);
    this.mCamera = this.mScene.Camera as MagickCamera;
    this.mCamera.SetPlayState(this);
    this.mLevelFileName = iLevelFileName;
    this.mEndGameText = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.Stonecross50), TextAlign.Center, false);
    GUIBasicEffect iEffect = (GUIBasicEffect) null;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
      iEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
    this.mRenderData = new PlayState.RenderData[3];
    this.mCutsceneRenderData = new PlayState.CutsceneRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new PlayState.RenderData(iEffect, this.mEndGameText);
      this.mCutsceneRenderData[index] = new PlayState.CutsceneRenderData(iEffect);
    }
    TutorialManager instance = TutorialManager.Instance;
    int num = MagickaMath.Random.Next(22) + 1;
    bool flag1 = false;
    if (num == 6)
      flag1 = true;
    int iID = num >= 10 ? ("#tip" + (object) num).GetHashCodeCustom() : ("#tip0" + (object) num).GetHashCodeCustom();
    int hashCodeCustom = "#tip".GetHashCodeCustom();
    string str = LanguageManager.Instance.GetString(iID);
    if (flag1)
    {
      bool flag2 = false;
      Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && !(players[index].Gamer is NetworkGamer))
        {
          flag2 = !(players[index].Controller is KeyboardMouseController);
          break;
        }
      }
      str = !flag2 ? str.Replace("#KEY_BOOST;", '̪'.ToString()) : str.Replace("#KEY_BOOST;", '̥'.ToString());
    }
    this.mLoadingTip = $"{LanguageManager.Instance.GetString(hashCodeCustom)}\n{str}";
  }

  public bool DiedInLevel => this.mDiedInLevel;

  public void SetDiedInLevel() => this.mDiedInLevel = true;

  public InventoryBox Inventory => this.mInventoryBox;

  public void SetTip(string iTip, bool iShowProgress, Cue iCueToFinish)
  {
    this.mLoadingTip = iTip;
    this.mShowProgress = iShowProgress;
    this.mCueToFinish = iCueToFinish;
  }

  public GameType GameType => this.mGameType;

  public SaveData SaveSlot => this.mSaveSlot;

  public MemoryStream CheckpointStream
  {
    get => this.mCheckpointStream;
    set => this.mCheckpointStream = value;
  }

  public bool Busy => this.mBusy;

  public void UpdateCheckPoint(Matrix[] iSpawnPoints, List<int> iIgnoredTriggers, bool iSaveToDisk)
  {
    lock (this.mCheckpointState)
    {
      this.mCheckpointState.UpdateState(iSpawnPoints, iIgnoredTriggers);
      Profile.Instance.Write();
      if (!iSaveToDisk || this.mGameType == GameType.Versus || NetworkManager.Instance.State == NetworkState.Client)
        return;
      MemoryStream output = new MemoryStream();
      BinaryWriter iWriter = new BinaryWriter((Stream) output);
      iWriter.Write(this.mLevelHash, 0, 32 /*0x20*/);
      iWriter.Write(this.mDialogHash, 0, 32 /*0x20*/);
      this.mCheckpointState.Write(iWriter);
      if (this.mSaveSlot == null)
        this.mSaveSlot = new SaveData();
      this.mSaveSlot.Checkpoint = output;
      if (this.mGameType != GameType.Campaign && this.mGameType != GameType.Mythos)
        return;
      SaveManager.Instance.SaveCampaign();
    }
  }

  public void Restart(object iSender, RestartType iRestartType)
  {
    if (!(iSender is NetworkInterface) && NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (iRestartType == RestartType.StartOfLevel)
    {
      RenderManager.Instance.TransitionEnd += new TransitionEnd(this.RestartLevel);
    }
    else
    {
      if (iRestartType != RestartType.Checkpoint)
        throw new ArgumentException("Invalid RestartType!", nameof (iRestartType));
      RenderManager.Instance.TransitionEnd += new TransitionEnd(this.RestartAtCheckPoint);
    }
    InGameMenu.Hide();
    this.mBlizzardRainCount = 0U;
    this.mDiedInLevel = iRestartType != RestartType.StartOfLevel;
    RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 0.5f);
    RenderManager.Instance.SkyMapColor = Vector3.One;
  }

  private void RestartAtCheckPoint(TransitionEffect iDeadTransition)
  {
    Magicka.Game.Instance.DisableRendering();
    RenderManager.Instance.TransitionEnd -= new TransitionEnd(this.RestartAtCheckPoint);
    AudioManager.Instance.StopAll(AudioStopOptions.Immediate);
    this.EndCutscene(true);
    SpellManager.Instance.ClearMagicks();
    Magicka.Game.Instance.AddLoadTask((System.Action) (() =>
    {
      while (!this.mBusy)
        Thread.Sleep(1);
      PlayState.sWaitingForPlayers = true;
      if (NetworkManager.Instance.Interface is NetworkServer)
      {
        (NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
        GameRestartMessage iMessage;
        iMessage.Type = RestartType.Checkpoint;
        NetworkManager.Instance.Interface.SendMessage<GameRestartMessage>(ref iMessage);
      }
      this.ApplyState(this.mCheckpointState);
      RenderManager.Instance.EndTransition(Transitions.Fade, Color.Black, 0.5f);
      RenderManager.Instance.SkyMapColor = Vector3.One;
      Magicka.Game.Instance.EnableRendering();
    }));
    this.mDiedInLevel = true;
    for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
    {
      if (Magicka.Game.Instance.Players[index].Playing && Magicka.Game.Instance.Players[index].IconRenderer != null)
        Magicka.Game.Instance.Players[index].IconRenderer.TomeMagick = MagickType.None;
    }
    SubMenuCharacterSelect.Instance.UpdateAvailableAvatars(new DlcInstalled());
  }

  private void RestartLevel(TransitionEffect iDeadTransition)
  {
    Magicka.Game.Instance.DisableRendering();
    RenderManager.Instance.TransitionEnd -= new TransitionEnd(this.RestartLevel);
    AudioManager.Instance.StopAll(AudioStopOptions.Immediate);
    SpellManager.Instance.ClearMagicks();
    this.mLevel.ClearDisplayTitles();
    if (this.mLevel.CurrentScene.RuleSet is VersusRuleset)
    {
      for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
        Magicka.Game.Instance.Players[index].UnlockedMagicks = 0UL;
    }
    Magicka.Game.Instance.AddLoadTask((System.Action) (() =>
    {
      while (!this.mBusy)
        Thread.Sleep(1);
      PlayState.sWaitingForPlayers = true;
      if (NetworkManager.Instance.Interface is NetworkServer)
      {
        (NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
        GameRestartMessage iMessage;
        iMessage.Type = RestartType.StartOfLevel;
        NetworkManager.Instance.Interface.SendMessage<GameRestartMessage>(ref iMessage);
      }
      this.ApplyState(this.mLevelStartState);
      RenderManager.Instance.EndTransition(Transitions.Fade, Color.Black, 0.5f);
      RenderManager.Instance.SkyMapColor = Vector3.One;
      Magicka.Game.Instance.EnableRendering();
    }));
    this.mDiedInLevel = false;
    for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
    {
      this.mHasUsedMagick[index] = false;
      this.mTooFancyForFireballs[index] = false;
      if (Magicka.Game.Instance.Players[index].Playing)
      {
        this.mTooFancyForFireballs[index] = true;
        this.mItsRainingBeastMen[index].Clear();
        this.mItsRainingBeastMen[index] = new HitList(10);
        if (Magicka.Game.Instance.Players[index].IconRenderer != null)
          Magicka.Game.Instance.Players[index].IconRenderer.TomeMagick = MagickType.None;
      }
    }
  }

  private void ApplyState(PlayState.State iState) => this.ApplyState(iState, (Action<float>) null);

  private void ApplyState(PlayState.State iState, Action<float> reportProgressBackAction)
  {
    this.EndCutscene(true);
    if (Credits.Instance.IsActive)
      Credits.Instance.Kill();
    this.mCutsceneSkipTipAlpha = 0.0f;
    this.mShowCutsceneSkipTip = false;
    this.mEndGameActive = false;
    this.mEndGameMusicActive = false;
    DialogManager.Instance.EndAll();
    this.mCamera.Release(0.0f);
    this.mTimeModifier = 1f;
    this.mTimeMultiplier = 1f;
    TutorialManager.Instance.Reset();
    if (this.mGenericHealtBar != null)
      this.mGenericHealtBar.Reset();
    this.mCamera.RemoveEffects();
    iState.ApplyState(reportProgressBackAction);
  }

  private unsafe void Initialize()
  {
    TutorialManager.Instance.Initialize(this);
    TutorialManager.Instance.Reset();
    Magicka.Levels.Triggers.Actions.Action.ClearInstances();
    this.mInitialized = false;
    this.mBusy = true;
    this.mEndGamePhony = false;
    Magicka.Game.Instance.DisableRendering();
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.loadingScreen = new LoadingScreen(this.mShowProgress, this.mLoadingTip);
    while (Magicka.Game.Instance.RenderingEnabled)
      Thread.Sleep(1);
    this.mDiedInLevel = false;
    this.loadingScreen.Progress = 0.0f;
    this.loadingScreen.FadeIn(0.5f);
    this.mCutsceneSkipped = false;
    this.mCutscene = false;
    if (PlayState.sRecentPlayState != null)
      PlayState.sRecentPlayState.Dispose();
    PlayState.sRecentPlayState = this;
    this.loadingScreen.Progress = 0.01f;
    this.loadingScreen.Draw();
    this.mTimeModifier = 1f;
    this.mTimeMultiplier = 1f;
    this.mContent = (ContentManager) new SharedContentManager(Magicka.Game.Instance.Content.ServiceProvider);
    this.mInventoryBox = new InventoryBox();
    LanguageManager.Instance.SetPlayerStrings();
    this.mLevelFileName = Path.GetFullPath("content/Levels/" + this.mLevelFileName);
    this.mScene.Camera.Direction = new Vector3(0.0f, (float) System.Math.Sin(-2.0 * System.Math.PI / 9.0), -(float) System.Math.Cos(-2.0 * System.Math.PI / 9.0));
    XmlDocument iInput = new XmlDocument();
    iInput.Load(this.mLevelFileName);
    this.mLevel = new Level(this.mLevelFileName, iInput, this, this.mSpawnPoint, this.mSettings);
    this.mLevelHash = this.mLevel.ShaHash;
    this.mDialogHash = this.mLevel.DialogHash;
    foreach (GameScene scene in this.mLevel.Scenes)
    {
      byte[] shaHash = scene.ShaHash;
      for (int index = 0; index < 32 /*0x20*/; ++index)
        this.mLevelHash[index] ^= shaHash[index];
    }
    if (this.mLevel.Name.Equals("vs_boat", StringComparison.InvariantCultureIgnoreCase))
      Profile.Instance.PlayingIslandCruise(this);
    this.loadingScreen.Progress = 0.02f;
    this.loadingScreen.Draw();
    InGameMenu.Initialize(this);
    this.mLevel.Initialize();
    this.mPlayerStaffLight = false;
    this.loadingScreen.Progress = 0.03f;
    this.loadingScreen.Draw();
    this.mGenericHealtBar = new GenericHealthBar(this.mScene);
    this.mEntityManager = new EntityManager(this);
    this.loadingScreen.Progress = 0.04f;
    this.loadingScreen.Draw();
    this.mSpellEffects = new List<SpellEffect>(16 /*0x10*/);
    SpellEffect.IntializeCaches(this, this.mContent);
    this.loadingScreen.Progress = 0.05f;
    this.loadingScreen.Draw();
    Magicka.GameLogic.Spells.Magick.InitializeMagicks(this);
    this.loadingScreen.Progress = 0.06f;
    this.loadingScreen.Draw();
    LightningBolt.InitializeCache(Magicka.Game.Instance.Content, 128 /*0x80*/);
    this.loadingScreen.Progress = 0.07f;
    this.loadingScreen.Draw();
    Railgun.InitializeCache(64 /*0x40*/);
    this.loadingScreen.Progress = 0.08f;
    this.loadingScreen.Draw();
    ArcaneBlast.InitializeCache(64 /*0x40*/);
    this.loadingScreen.Progress = 0.09f;
    this.loadingScreen.Draw();
    ArcaneBlade.InitializeCache(64 /*0x40*/);
    this.loadingScreen.Progress = 0.1f;
    this.loadingScreen.Draw();
    IceBlade.InitializeCache(64 /*0x40*/);
    this.loadingScreen.Progress = 0.11f;
    this.loadingScreen.Draw();
    UnderGroundAttack.InitializeCache(32 /*0x20*/, this);
    this.loadingScreen.Progress = 0.14f;
    this.loadingScreen.Draw();
    SpellMine.InitializeCache(64 /*0x40*/, this);
    this.loadingScreen.Progress = 0.17f;
    this.loadingScreen.Draw();
    IceSpikes.InitializeCache(32 /*0x20*/);
    this.loadingScreen.Progress = 0.2f;
    this.loadingScreen.Draw();
    TeslaField.InitializeCache(32 /*0x20*/, this);
    this.loadingScreen.Progress = 0.22f;
    this.loadingScreen.Draw();
    Shield.InitializeCache(64 /*0x40*/, this);
    this.loadingScreen.Progress = 0.25f;
    this.loadingScreen.Draw();
    Barrier.InitializeCache(128 /*0x80*/, this);
    WaveEntity.InitializeCache(16 /*0x10*/, this);
    this.loadingScreen.Progress = 0.28f;
    this.loadingScreen.Draw();
    Barrier.HitListWithBarriers.InitializeCache(32 /*0x20*/);
    this.loadingScreen.Progress = 0.3f;
    this.loadingScreen.Draw();
    SprayEntity.InitializeCache(this, 48 /*0x30*/);
    this.loadingScreen.Progress = 0.32f;
    this.loadingScreen.Draw();
    RadialBlur.InitializeCache(this.mContent, 16 /*0x10*/);
    this.loadingScreen.Progress = 0.35f;
    this.loadingScreen.Draw();
    Dispenser.InitializeCache(32 /*0x20*/, this);
    this.loadingScreen.Progress = 0.39f;
    this.loadingScreen.Draw();
    Gib.InitializeCache(256 /*0x0100*/, this);
    this.loadingScreen.Progress = 0.41f;
    this.loadingScreen.Draw();
    DamageNotifyer.Instance.Scene = this.mScene;
    this.loadingScreen.Progress = 0.45f;
    this.loadingScreen.Draw();
    DynamicLight.Initialize(this.mScene);
    this.loadingScreen.Progress = 0.48f;
    this.loadingScreen.Draw();
    Avatar.InitializeCache(this);
    this.loadingScreen.Progress = 0.5f;
    this.loadingScreen.Draw();
    BookOfMagick.InitializeCache(64 /*0x40*/, this);
    this.loadingScreen.Progress = 0.52f;
    this.loadingScreen.Draw();
    this.mItsRainingBeastMen = new HitList[4];
    this.mTooFancyForFireballs = new bool[4];
    this.mHasUsedMagick = new bool[4];
    int length = Magicka.Game.Instance.Players.Length;
    this.mInfo = new SaveSlotInfo();
    if (this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos)
    {
      if (NetworkManager.Instance.State != NetworkState.Client)
      {
        this.mInfo = new SaveSlotInfo(this.mSaveSlot);
      }
      else
      {
        this.mInfo = (NetworkManager.Instance.Interface as NetworkClient).SaveSlot;
        while (!this.mInfo.IsValid)
        {
          this.mInfo = (NetworkManager.Instance.Interface as NetworkClient).SaveSlot;
          Thread.Sleep(100);
        }
        (NetworkManager.Instance.Interface as NetworkClient).ClearSaveSlot();
      }
    }
    this.loadingScreen.Progress = 0.55f;
    this.loadingScreen.Draw();
    for (int index = 0; index < length; ++index)
    {
      Magicka.GameLogic.Player player = Magicka.Game.Instance.Players[index];
      if (player.Playing && player.IconRenderer != null)
        player.IconRenderer.TomeMagick = MagickType.None;
      if (player.Playing)
      {
        this.mItsRainingBeastMen[index] = new HitList(10);
        this.mTooFancyForFireballs[index] = true;
      }
      if (this.mGameType == GameType.Campaign | this.mGameType == GameType.Mythos)
      {
        player.UnlockedMagicks = this.mInfo.UnlockedMagicks;
        if (player.Playing)
        {
          PlayerSaveData playerSaveData;
          if (!this.mInfo.Players.TryGetValue(Magicka.Game.Instance.Players[index].GamerTag, out playerSaveData))
          {
            playerSaveData = new PlayerSaveData();
            this.mInfo.Players.Add(Magicka.Game.Instance.Players[index].GamerTag, playerSaveData);
          }
          player.Staff = playerSaveData.Staff;
          player.Weapon = playerSaveData.Weapon;
        }
      }
      else if (this.mGameType == GameType.Challenge || this.mGameType == GameType.StoryChallange)
      {
        player.UnlockedMagicks = 2UL;
        player.Staff = (string) null;
        player.Weapon = (string) null;
      }
      else
      {
        player.UnlockedMagicks = 0UL;
        player.Staff = (string) null;
        player.Weapon = (string) null;
      }
      player.InitializeGame(this);
      if (player.Playing)
      {
        if (!string.IsNullOrEmpty(player.Weapon))
        {
          try
          {
            this.mContent.Load<Item>("data/items/wizard/" + player.Weapon);
          }
          catch
          {
            this.mContent.Load<Item>("data/items/npc/" + player.Weapon);
          }
        }
        if (!string.IsNullOrEmpty(player.Staff))
        {
          try
          {
            this.mContent.Load<Item>("data/items/wizard/" + player.Staff);
          }
          catch
          {
            this.mContent.Load<Item>("data/items/npc/" + player.Staff);
          }
        }
        player.Avatar = Avatar.GetFromCache(player);
        CharacterTemplate iTemplate = this.mContent.Load<CharacterTemplate>("Data/Characters/" + player.Gamer.Avatar.TypeName);
        player.Avatar.Initialize(iTemplate, new Vector3(), Magicka.GameLogic.Player.UNIQUE_ID[index]);
        if (this.mLevel.SpawnFairy && Magicka.Game.Instance.PlayerCount == 1 && this.GameType == GameType.Campaign | this.GameType == GameType.Mythos)
          this.SetSpawnFairies();
      }
    }
    Item.InitializePickableCache(16 /*0x10*/, this);
    AIManager.Instance.Clear();
    this.loadingScreen.Progress = 0.6f;
    this.loadingScreen.Draw();
    ParticleLightBatcher.Instance.Initialize(Magicka.Game.Instance.GraphicsDevice);
    if (GlobalSettings.Instance.ParticleLights)
      ParticleLightBatcher.Instance.Enable(this.mScene);
    PointLightBatcher.Instance.Initialize(Magicka.Game.Instance.GraphicsDevice);
    PointLightBatcher.Instance.Enable(this.mScene);
    DecalManager.Instance.Initialize(this.mScene, Defines.DecalLimit());
    ParticleSystem.SetSpawnModifier(Defines.ParticleMultiplyer());
    this.mEndGameCondition = EndGameCondition.None;
    this.loadingScreen.Progress = 0.65f;
    this.loadingScreen.Draw();
    if (this.mSpawnPoint.HasValue && this.mSpawnPoint.HasValue)
    {
      this.StartupActionsPercentageAtStart = this.loadingScreen.Progress;
      this.StartupActionsPercentageTarget = 0.75f - this.StartupActionsPercentageAtStart;
      this.mLevel.GoToScene(this.mSpawnPoint.Value, Transitions.None, 0.0f, false, (System.Action) null, new Action<float>(this.StartupActionsReportHandle));
    }
    else
    {
      this.StartupActionsPercentageAtStart = this.loadingScreen.Progress;
      this.StartupActionsPercentageTarget = 0.75f - this.StartupActionsPercentageAtStart;
      this.mLevel.GoToScene(this.mLevel.SpawnPoint, Transitions.None, 0.0f, false, (System.Action) null, new Action<float>(this.StartupActionsReportHandle));
    }
    this.loadingScreen.Progress = 0.8f;
    this.loadingScreen.Draw();
    this.mLevelStartState = new PlayState.State(this);
    this.mCheckpointState = new PlayState.State(this);
    bool flag = false;
    NetworkState state = NetworkManager.Instance.State;
    if ((this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos) && (state != NetworkState.Client && this.mSaveSlot.Checkpoint != null || state == NetworkState.Client))
    {
      if (state == NetworkState.Client)
      {
        while (this.mCheckpointStream == null)
          Thread.Sleep(100);
      }
      else
        this.mCheckpointStream = this.mSaveSlot.Checkpoint;
      this.mCheckpointStream.Position = 0L;
      BinaryReader iReader = new BinaryReader((Stream) this.mCheckpointStream);
      byte[] iB1 = iReader.ReadBytes(32 /*0x20*/);
      byte[] iB2 = iReader.ReadBytes(32 /*0x20*/);
      if (Helper.ArrayEquals(this.mLevelHash, iB1))
      {
        if (Helper.ArrayEquals(this.mDialogHash, iB2))
        {
          try
          {
            this.mCheckpointState.Read(iReader);
            flag = true;
          }
          catch
          {
            flag = false;
            this.mCheckpointState = new PlayState.State(this);
          }
        }
      }
    }
    if (state == NetworkState.Server)
    {
      if (flag)
      {
        fixed (byte* iPtr = this.mCheckpointStream.GetBuffer())
          NetworkManager.Instance.Interface.SendRaw(PacketType.Checkpoint, (void*) iPtr, (int) this.mCheckpointStream.Length);
      }
      else
        NetworkManager.Instance.Interface.SendRaw(PacketType.Checkpoint, (void*) null, 0);
    }
    while (this.mCueToFinish != null)
    {
      if (this.mCueToFinish.IsStopped)
        this.mCueToFinish = (Cue) null;
      else
        Thread.Sleep(100);
    }
    this.loadingScreen.Progress += 0.01f;
    this.loadingScreen.Draw();
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.Sync();
    AudioManager.Instance.StopMusic();
    if (flag)
    {
      this.StartupActionsPercentageAtStart = this.loadingScreen.Progress;
      this.StartupActionsPercentageTarget = 1f - this.StartupActionsPercentageAtStart;
      this.ApplyState(this.mCheckpointState, new Action<float>(this.StartupActionsReportHandle));
    }
    PhysicsManager.Instance.Update(0.0f);
    this.mLevel.CurrentScene.Update(DataChannel.None, 0.0f);
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.Sync();
    this.loadingScreen.Progress = 1f;
    this.loadingScreen.Draw();
    this.loadingScreen.FadeOut(0.5f);
    GC.Collect();
    GC.WaitForPendingFinalizers();
    if (!RenderManager.Instance.IsTransitionActive && !this.IgnoreInitFade)
      RenderManager.Instance.EndTransition(Transitions.Fade, Color.Black, 0.5f);
    Magicka.Game.Instance.EnableRendering();
    this.mInitialized = true;
  }

  private void StartupActionsReportHandle(float f)
  {
    if (this.loadingScreen == null)
      return;
    this.loadingScreen.Progress = System.Math.Min(this.StartupActionsPercentageAtStart + this.StartupActionsPercentageTarget * f, 1f);
    this.loadingScreen.Draw();
  }

  public void HookupLoadingScreen(
    out Action<float> loadingCallBack,
    out System.Action whenDoneCallBack,
    float startPercentage,
    float targetPercentage)
  {
    loadingCallBack = (Action<float>) null;
    whenDoneCallBack = (System.Action) null;
    if (this.loadingScreen == null)
      this.loadingScreen = new LoadingScreen(true, this.mLoadingTip, true);
    else
      this.loadingScreen.Initialize(true, this.mLoadingTip, true);
    this.StartupActionsPercentageAtStart = startPercentage;
    this.StartupActionsPercentageTarget = targetPercentage;
    loadingCallBack = new Action<float>(this.StartupActionsReportHandle);
    whenDoneCallBack = new System.Action(this.TryDestroyLoadingScreen);
    Magicka.Game.Instance.DisableRendering();
    this.loadingScreen.Progress = 0.0f;
    this.loadingScreen.FadeIn(0.5f);
  }

  private void TryDestroyLoadingScreen()
  {
    if (this.loadingScreen != null)
    {
      this.loadingScreen.FadeOut(0.5f);
      this.loadingScreen.EndDraw();
    }
    Magicka.Game.Instance.EnableRendering();
  }

  public bool IgnoreInitFade { get; set; }

  public bool Initialized => this.mInitialized;

  public bool IsGameEnded => this.mEndGameCondition != EndGameCondition.None;

  public bool IsNotClientAndHasCampaignCheckpoint()
  {
    return (this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos) && NetworkManager.Instance.State != NetworkState.Client && this.mSaveSlot != null && this.mSaveSlot.Checkpoint != null;
  }

  public bool IsNotClientAndHasChallengeCheckpoint()
  {
    return this.mGameType == GameType.Challenge && NetworkManager.Instance.State != NetworkState.Client && this.mSaveSlot != null && this.mSaveSlot.Checkpoint != null;
  }

  public void ClearCurrentCampaignCheckPoint()
  {
    if (!this.IsNotClientAndHasCampaignCheckpoint())
      return;
    this.mSaveSlot.Checkpoint = (MemoryStream) null;
  }

  public void ReloadFromCheckpoint()
  {
    if (this.IsNotClientAndHasCampaignCheckpoint() || this.IsNotClientAndHasChallengeCheckpoint())
    {
      this.mSaveSlot.Checkpoint.Position = 0L;
      BinaryReader reader = new BinaryReader((Stream) this.mSaveSlot.Checkpoint);
      byte[] iB1 = reader.ReadBytes(32 /*0x20*/);
      byte[] iB2 = reader.ReadBytes(32 /*0x20*/);
      if (Helper.ArrayEquals(this.mLevelHash, iB1) && Helper.ArrayEquals(this.mDialogHash, iB2))
      {
        bool done = false;
        Magicka.Game.Instance.AddLoadTask((System.Action) (() =>
        {
          this.mCheckpointState.Read(reader);
          done = true;
        }));
        while (!done)
          Thread.Sleep(1);
        this.Restart((object) null, RestartType.Checkpoint);
      }
      else
        this.Restart((object) this, RestartType.StartOfLevel);
    }
    else
      this.Restart((object) this, RestartType.StartOfLevel);
  }

  public void Endgame(EndGameCondition iType, bool iFreezeGame, bool iPhony, float iTime)
  {
    this.mEndGamePhony = iPhony;
    this.mEndGameCondition = iType;
    this.mEndGameTimer = System.Math.Min(iTime, this.mEndGameTimer);
    switch (iType)
    {
      case EndGameCondition.Victory:
        if (this.Level.CurrentScene.RuleSet is SurvivalRuleset)
          AchievementsManager.Instance.AwardAchievement(this, "wearethechampions");
        this.mEndGameText.SetText(LanguageManager.Instance.GetString(Defines.LOC_GAME_VICTORY));
        break;
      case EndGameCondition.LevelComplete:
        if (this.mEndGamePhony)
          RenderManager.Instance.TimeModifier = 0.0f;
        this.mEndGameText.SetText(LanguageManager.Instance.GetString(Defines.LOC_GAME_LEVCOMP));
        Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstAI();
        break;
      case EndGameCondition.Defeat:
        this.mEndGameText.SetText(LanguageManager.Instance.GetString(Defines.LOC_GAME_DEFEATED));
        if (TutorialUtils.IsInProgress)
        {
          TutorialUtils.Fail();
          break;
        }
        break;
      case EndGameCondition.VersusPlayer:
      case EndGameCondition.VersusTeam:
        this.mEndGameText.SetText((string) null);
        break;
      case EndGameCondition.Disconnected:
        this.mEndGameText.SetText((string) null);
        break;
      case EndGameCondition.ToBeContinued:
        this.mEndGameText.Clear();
        RenderManager.Instance.TransitionEnd += new TransitionEnd(this.OnTransitionEnd);
        RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 8f);
        break;
      default:
        this.mEndGameText.SetText((string) null);
        break;
    }
    StatisticsManager.Instance.ClearPlayerMultiKillCounter();
    if (!iFreezeGame)
      return;
    ControlManager.Instance.LimitInput((object) this);
  }

  public void OnTransitionEnd(TransitionEffect iDeadTransition)
  {
    RenderManager.Instance.TransitionEnd -= new TransitionEnd(this.OnTransitionEnd);
    ControlManager.Instance.UnlimitInput();
    InGameMenu.HideInstant();
    RenderManager.Instance.EndTransition(Transitions.CrossFade, Color.White, 1f);
    Tome.Instance.PlayCameraAnimation(Tome.CameraAnimation.Zoomed_In);
    Tome.Instance.PushMenuInstant((SubMenu) SubMenuEndGame.Instance, 1);
    Tome.Instance.ChangeState((Tome.TomeState) Tome.ClosedState.Instance);
    GameStateManager.Instance.PopState();
  }

  public void ChangingScene() => this.mCutsceneSkipped = false;

  public void BeginCutscene(int iOnSkipID, bool iSkipBarMove, bool iKillDialogs)
  {
    this.UIEnabled = false;
    this.mCutsceneSkipped = false;
    this.mCutscene = true;
    this.mCutsceneTimer = iSkipBarMove ? 1f : 0.0f;
    this.mCutsceneSkipTrigger = iOnSkipID;
    if (iKillDialogs)
      DialogManager.Instance.EndAll();
    for (int index = 0; index < this.mCutsceneRenderData.Length; ++index)
      this.mCutsceneRenderData[index].UpdateText();
    this.mInventoryBox.Close((Magicka.GameLogic.Entities.Character) null);
    Vector3 result1 = MagickCamera.CAMERAOFFSET;
    Vector3.Negate(ref result1, out result1);
    Vector3 result2 = this.mCamera.Position;
    Vector3.Add(ref result2, ref result1, out result2);
    SpellManager.Instance.ClearMagicks();
    List<Entity> entities = this.EntityManager.GetEntities(result2, 40f, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      IDamageable damageable = entities[index] as IDamageable;
      if (entities[index] is TornadoEntity | entities[index] is Barrier | entities[index] is SpellMine | entities[index] is Shield | entities[index] is Grease.GreaseField | entities[index] is SummonDeath.MagickDeath)
        entities[index].Kill();
      else if (damageable == null)
        continue;
      if (entities[index] is Magicka.GameLogic.Entities.Character character)
      {
        character.StopAllActions();
        character.StopStatusEffects(StatusEffects.Frozen);
        if (character.IsEntangled)
          character.ReleaseEntanglement();
        if (character.IsFeared)
          character.RemoveFear();
        if (character is NonPlayerCharacter nonPlayerCharacter)
        {
          if (nonPlayerCharacter.IsCharmed)
            nonPlayerCharacter.EndCharm();
          if (nonPlayerCharacter.IsSummoned)
            nonPlayerCharacter.Kill();
        }
      }
    }
    this.EntityManager.ReturnEntityList(entities);
  }

  public void EndCutscene(bool iSkipBarMove)
  {
    this.UIEnabled = true;
    this.mCutscene = false;
    this.mCutsceneSkipped = false;
    this.mCutsceneSkipRemoveBars = iSkipBarMove;
    DialogManager.Instance.EndAll();
  }

  public void SkipCutscene()
  {
    if (this.mCutsceneSkipped || NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (!this.mShowCutsceneSkipTip)
    {
      this.mShowCutsceneSkipTip = true;
    }
    else
    {
      this.mCutsceneSkipped = true;
      if (this.mCutsceneSkipTrigger != 0)
        this.Level.CurrentScene.ExecuteTrigger(this.mCutsceneSkipTrigger, (Magicka.GameLogic.Entities.Character) null, false);
      this.mCutsceneSkipTrigger = 0;
    }
  }

  public bool IsInCutscene => this.mCutscene | (double) this.mCutsceneTimer >= 1.0;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mScene.ClearObjects(iDataChannel);
    if (!this.mInitialized || this.mLevel.Busy || !Magicka.Game.Instance.RenderingEnabled)
    {
      this.mBusy = true;
    }
    else
    {
      this.mBusy = false;
      if (this.mOverlayIsPaused)
      {
        if (NetworkManager.Instance.State == NetworkState.Offline)
          iDeltaTime = 0.0f;
      }
      else if (InGameMenu.Visible)
      {
        InGameMenu.Update(iDataChannel, iDeltaTime);
        if (NetworkManager.Instance.State == NetworkState.Offline)
          iDeltaTime = 0.0f;
      }
      else if (this.mIsPaused)
      {
        AudioManager.Instance.ResumeAll();
        ControlManager.Instance.UnlimitInput((object) this);
        this.mIsPaused = false;
      }
      StatisticsManager.Instance.UpdatePlayerMultiKillCounter(iDeltaTime);
      this.mPlayTime += (double) iDeltaTime;
      iDeltaTime *= this.mTimeModifier * this.mTimeMultiplier;
      this.UpdatePhysics(iDataChannel, iDeltaTime);
      this.UpdateAI(iDataChannel, iDeltaTime);
      this.mEntityManager.UpdateQuadGrid();
      float iDeltaTime1 = iDeltaTime;
      if (this.mEndGamePhony && this.mEndGameCondition == EndGameCondition.LevelComplete)
        iDeltaTime1 = 0.0f;
      this.UpdateAnimatedLevelParts(iDataChannel, iDeltaTime1);
      this.UpdateEntitiesA(iDataChannel, iDeltaTime1);
      if (this.mBossFight != null)
        this.mBossFight.Update(iDataChannel, iDeltaTime1);
      if (this.mGenericHealtBar != null)
        this.mGenericHealtBar.Update(iDataChannel, iDeltaTime);
      this.mEntityManager.RemoveDeadEntities();
      this.UpdateMiscA(iDataChannel, iDeltaTime);
      this.UpdateMiscB(iDataChannel, iDeltaTime);
      for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
      {
        if (Magicka.Game.Instance.Players[index].Playing)
        {
          this.mItsRainingBeastMen[index].Update(iDeltaTime);
          if (this.mItsRainingBeastMen[index].Count >= 5)
            AchievementsManager.Instance.AwardAchievement(this, "itsrainingbeastmen");
        }
      }
      NetworkInterface networkInterface = NetworkManager.Instance.Interface;
      if (networkInterface != null)
      {
        this.mNetworkUpdateTimer -= iDeltaTime;
        if ((double) this.mNetworkUpdateTimer <= 0.0)
        {
          this.mNetworkUpdateTimer = 0.0333333351f;
          for (int index = 0; index < networkInterface.Connections; ++index)
          {
            float iPrediction = networkInterface.GetLatency(index) * 0.5f;
            this.mEntityManager.UpdateNetwork(index, iPrediction);
          }
        }
      }
      ControlManager.Instance.HandleInput(iDataChannel, iDeltaTime);
    }
  }

  private void UpdateAI(DataChannel iDataChannel, float iDeltaTime)
  {
    AIManager.Instance.Update(iDeltaTime);
  }

  public double PlayTime => this.mPlayTime;

  public float TimeModifier
  {
    get => this.mTimeModifier;
    set => this.mTimeModifier = value;
  }

  public float TimeMultiplier
  {
    get => this.mTimeMultiplier;
    set => this.mTimeMultiplier = value;
  }

  private void UpdatePhysics(DataChannel iDataChannel, float iDeltaTime)
  {
    PhysicsManager.Instance.Update(iDeltaTime);
  }

  private void UpdateEntitiesA(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mEntityManager.Update(iDataChannel, iDeltaTime);
  }

  private void UpdateMiscA(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mEndGameCondition == EndGameCondition.Disconnected)
      this.mEndGameText.SetText("NDFJ");
    float iDeltaTime1 = iDeltaTime;
    if (this.mEndGamePhony && this.mEndGameCondition == EndGameCondition.LevelComplete)
      iDeltaTime1 = 0.0f;
    DecalManager.Instance.Update(iDataChannel, iDeltaTime1);
    ShadowBlobs.Instance.Update(iDataChannel, iDeltaTime1);
    Credits.Instance.Update(iDataChannel, iDeltaTime1, this);
    TutorialManager.Instance.Update(iDataChannel, iDeltaTime1);
    this.mInventoryBox.Update(iDeltaTime1, iDataChannel);
    if ((double) this.mEndGameTimer > 0.0)
    {
      bool flag = true;
      Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        players[index]?.Update(iDataChannel, iDeltaTime1);
        if (players[index].Playing && players[index].Avatar != null && (!players[index].Avatar.Dead || Magicka.Game.Instance.PlayerCount == 1 && players[index].Avatar.RevivalFairy.Active))
          flag = false;
      }
      if (Credits.Instance.IsActive && (this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos))
        flag = false;
      if (flag & this.mGameType != GameType.Versus)
      {
        if (this.mEndGameCondition != EndGameCondition.Defeat)
          this.Endgame(EndGameCondition.Defeat, false, false, 3f);
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          GameEndMessage iMessage;
          iMessage.Condition = this.mEndGameCondition;
          iMessage.DelayTime = 3f;
          iMessage.Argument = 0;
          iMessage.Phony = false;
          NetworkManager.Instance.Interface.SendMessage<GameEndMessage>(ref iMessage);
        }
      }
      else if (this.mEndGameCondition == EndGameCondition.Defeat)
        this.mEndGameCondition = EndGameCondition.None;
    }
    if (this.mEndGameCondition != EndGameCondition.None)
    {
      if ((double) this.mEndGameTimer <= -2.0 && (!this.mEndGameMusicActive || !AudioManager.Instance.IsMusicPlaying))
        this.mEndGameTextAlpha = System.Math.Max(this.mEndGameTextAlpha - iDeltaTime, 0.0f);
      else if ((double) this.mEndGameTimer <= 0.0)
        this.mEndGameTextAlpha = System.Math.Min(this.mEndGameTextAlpha + iDeltaTime, 1f);
      if (!this.mEndGameActive)
      {
        if ((double) this.mEndGameTimer <= 0.0 && !this.mEndGameMusicActive)
        {
          switch (this.mEndGameCondition)
          {
            case EndGameCondition.Defeat:
              AudioManager.Instance.PlayMusic(Banks.Music, PlayState.MUSIC_DEFEAT, new float?());
              this.mEndGameMusicActive = true;
              break;
            case EndGameCondition.EightyeightMilesPerHour:
            case EndGameCondition.ChallengeExit:
            case EndGameCondition.EndOffGame:
              break;
            default:
              AudioManager.Instance.PlayMusic(Banks.Music, PlayState.MUSIC_VICTORY, new float?());
              this.mEndGameMusicActive = true;
              break;
          }
        }
        if (this.mEndGameCondition == EndGameCondition.ChallengeExit)
        {
          this.mEndGameActive = true;
          ControlManager.Instance.UnlimitInput((object) this);
          Texture2D screenShot = RenderManager.Instance.GetScreenShot(new System.Action(this.ScreenCaptureCallback));
          SubMenuEndGame.Instance.ScreenShot = screenShot;
          RenderManager.Instance.GetTransitionEffect(Transitions.CrossFade).SourceTexture1 = screenShot;
        }
        else if ((double) this.mEndGameTimer < -3.0 && (double) this.mEndGameTextAlpha <= 0.0)
        {
          if (this.mEndGamePhony)
          {
            this.mEndGameCondition = EndGameCondition.None;
            this.mEndGamePhony = false;
            ControlManager.Instance.UnlimitInput((object) this);
            RenderManager.Instance.TimeModifier = 1f;
          }
          else
          {
            this.mEndGameActive = true;
            if (this.mGameType != GameType.Campaign && this.mGameType != GameType.Mythos)
            {
              if (this.mLevel.CurrentScene.RuleSet != null)
              {
                InGameMenu.Show((Controller) null);
                if (this.mLevel.CurrentScene.RuleSet is SurvivalRuleset)
                  InGameMenu.PushMenu((InGameMenu) InGameMenuSurvivalStatistics.Instance);
                else if (this.mLevel.CurrentScene.RuleSet is TimedObjectiveRuleset)
                  InGameMenu.PushMenu((InGameMenu) InGameMenuTimedObjectiveStatistics.Instance);
                else if (this.mLevel.CurrentScene.RuleSet is VersusRuleset)
                  InGameMenu.PushMenu((InGameMenu) InGameMenuVersusStatistics.Instance);
                this.mIsPaused = true;
              }
              else
                this.Restart((object) this, RestartType.Checkpoint);
            }
            else if (this.mEndGameCondition == EndGameCondition.LevelComplete)
            {
              ControlManager.Instance.UnlimitInput((object) this);
              Texture2D screenShot = RenderManager.Instance.GetScreenShot(new System.Action(this.ScreenCaptureCallback));
              SubMenuEndGame.Instance.ScreenShot = screenShot;
              RenderManager.Instance.GetTransitionEffect(Transitions.CrossFade).SourceTexture1 = screenShot;
              for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
              {
                if (Magicka.Game.Instance.Players[index].Playing && !(Magicka.Game.Instance.Players[index].Gamer is NetworkGamer) && this.mTooFancyForFireballs[index] && this.mHasUsedMagick[index])
                  AchievementsManager.Instance.AwardAchievement(this, "toofancyforfireballs");
              }
            }
            else
              this.Restart((object) this, RestartType.Checkpoint);
            SubMenuEndGame.Instance.Set(this.mGameType == GameType.Mythos | this.mGameType == GameType.Campaign, this.mSaveSlot);
          }
        }
      }
      this.mEndGameTimer -= iDeltaTime;
    }
    else
    {
      this.mEndGameTextAlpha = 0.0f;
      this.mEndGameTimer = 3f;
    }
    if (this.mCutscene)
    {
      this.mCutsceneSkipTipAlpha = this.mCutsceneSkipTrigger == 0 || !this.mShowCutsceneSkipTip ? System.Math.Max(this.mCutsceneSkipTipAlpha - iDeltaTime * 2f, 0.0f) : System.Math.Min(this.mCutsceneSkipTipAlpha + iDeltaTime * 2f, 1f);
      this.mCutsceneTimer = System.Math.Min(this.mCutsceneTimer + iDeltaTime, 1f);
      PlayState.CutsceneRenderData iObject = this.mCutsceneRenderData[(int) iDataChannel];
      iObject.Time = this.mCutsceneTimer;
      iObject.mSkipAlpha = this.mCutsceneSkipTipAlpha;
      this.mScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
    }
    else if ((double) this.mCutsceneTimer > 0.0)
    {
      this.mCutsceneTimer = System.Math.Max(this.mCutsceneTimer - iDeltaTime, 0.0f);
      if (this.mShowCutsceneSkipTip)
        this.mCutsceneSkipTipAlpha = System.Math.Max(this.mCutsceneSkipTipAlpha - iDeltaTime, 0.0f);
      PlayState.CutsceneRenderData iObject = this.mCutsceneRenderData[(int) iDataChannel];
      iObject.Time = this.mCutsceneTimer;
      iObject.mSkipAlpha = this.mCutsceneSkipTipAlpha;
      this.mScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
    }
    else
    {
      this.mCutsceneSkipTipAlpha = 0.0f;
      this.mShowCutsceneSkipTip = false;
    }
    EffectManager.Instance.Update(iDeltaTime1);
    SpellManager.Instance.Update(iDataChannel, iDeltaTime1, this);
    Healthbars.Instance.Update(iDataChannel, iDeltaTime1);
    this.mScene.Camera.Update(iDataChannel, iDeltaTime1);
    Matrix oMatrix;
    this.mScene.Camera.GetViewProjectionMatrix(iDataChannel, out oMatrix);
    DialogManager.Instance.Update(iDataChannel, iDeltaTime1, ref oMatrix);
    NetworkChat.Instance.Update(iDeltaTime);
    PlayState.RenderData iObject1 = this.mRenderData[(int) iDataChannel];
    iObject1.PostTextAlpha = this.mEndGameTextAlpha;
    this.mScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject1);
  }

  private void ScreenCaptureCallback()
  {
    InGameMenu.HideInstant();
    RenderManager.Instance.EndTransition(Transitions.CrossFade, Color.White, 1f);
    Tome.Instance.PlayCameraAnimation(Tome.CameraAnimation.Zoomed_In);
    Tome.Instance.PushMenuInstant((SubMenu) SubMenuEndGame.Instance, 1);
    Tome.Instance.ChangeState((Tome.TomeState) Tome.OpenState.Instance);
    GameStateManager.Instance.PopState();
  }

  private void UpdateMiscB(DataChannel iDataChannel, float iDeltaTime)
  {
    float iDeltaTime1 = iDeltaTime;
    if (this.mEndGamePhony && this.mEndGameCondition == EndGameCondition.LevelComplete)
      iDeltaTime1 = 0.0f;
    TracerMan.Instance.Update(iDeltaTime1);
    DamageNotifyer.Instance.Update(iDataChannel, iDeltaTime1);
    this.mLevel.Update(iDataChannel, iDeltaTime1);
    ParticleSystem.Instance.UpdateParticles(iDataChannel, iDeltaTime1);
    ParticleLightBatcher.Instance.Update(iDataChannel, iDeltaTime1);
    PointLightBatcher.Instance.Update(iDataChannel, iDeltaTime1);
  }

  private void UpdateAnimatedLevelParts(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mLevel.UpdateAnimatedLevelParts(iDataChannel, iDeltaTime);
  }

  public void Dispose()
  {
    if (!this.mInitialized)
      return;
    this.mHasEntered = false;
    DialogManager.Instance.SetDialogs((DialogCollection) null);
    Item.ClearCache();
    Avatar.ClearCache();
    CharacterTemplate.ClearCache();
    BossFight.Instance.Clear();
    RadialBlur.DisposeCache();
    this.mContent.Unload();
    this.mContent.Dispose();
    this.mContent = (ContentManager) null;
    Entanglement.DisposeModels();
    SpellManager.Instance.ClearEffects();
    this.mEntityManager.Clear();
    this.mEntityManager = (EntityManager) null;
    Entity.ClearHandles();
    this.mLevel.Dispose();
    this.mLevel = (Level) null;
    DecalManager.Instance.Initialize((Scene) null, 0);
    DialogManager.Instance.EndAll();
    AIManager.Instance.Clear();
    PhysicsManager.Instance.Clear();
    this.mScene = (Scene) null;
    GC.Collect();
    GC.WaitForPendingFinalizers();
  }

  public override void OnEnter()
  {
    this.mHasEntered = true;
    if (!this.mInitialized)
    {
      Magicka.Game.Instance.AddLoadTask(new System.Action(this.Initialize));
      NetworkChat.Instance.Set(PlayState.CHAT_SIZE.X, PlayState.CHAT_SIZE.Y, (Texture2D) null, new Microsoft.Xna.Framework.Rectangle(), FontManager.Instance.GetFont(MagickaFont.Maiandra14), true, 10, false, 10f);
      NetworkChat.Instance.Active = false;
    }
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle();
    Point screenSize = RenderManager.Instance.ScreenSize;
    rectangle.Width = screenSize.X;
    rectangle.Height = screenSize.Y;
    AudioManager.Instance.SetListener(this.mCamera.Listener);
    this.mCamera.Init();
    this.mCamera.AspectRation = (float) rectangle.Width / (float) rectangle.Height;
  }

  public override void OnExit()
  {
    if (!this.mInitialized)
      return;
    this.EndCutscene(true);
    Tome.Instance.ChangeState((Tome.TomeState) Tome.OpenState.Instance);
    if (Credits.Instance.IsActive)
      Credits.Instance.Kill();
    ControlManager.Instance.ClearControllers();
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mScene.ClearObjects(DataChannel.A);
      this.mScene.ClearObjects(DataChannel.B);
      this.mScene.ClearObjects(DataChannel.C);
      PhysicsManager.Instance.Clear();
    }
    RenderManager.Instance.Fog = new Fog();
    RenderManager.Instance.Brightness = 1f;
    RenderManager.Instance.Contrast = 1f;
    RenderManager.Instance.Saturation = 1f;
    RenderManager.Instance.SkyMap = (Texture2D) null;
    RenderManager.Instance.BloomThreshold = 0.8f;
    RenderManager.Instance.BloomMultiplier = 1f;
    RenderManager.Instance.BlurSigma = 2.5f;
    AudioManager.Instance.StopAll(AudioStopOptions.AsAuthored);
    EffectManager.Instance.Clear();
    NetworkState state = NetworkManager.Instance.State;
    if (this.mGameType == GameType.Campaign | this.mGameType == GameType.Mythos)
    {
      if (state != NetworkState.Client && this.mEndGameCondition != EndGameCondition.Disconnected && this.mSaveSlot != null)
      {
        this.mSaveSlot.TotalPlayTime += (int) this.mPlayTime;
        this.mSaveSlot.CurrentPlayTime += (int) this.mPlayTime;
        SubMenuCampaignSelect_SaveSlotSelect.Instance.UpdateSlots();
      }
      this.mPlayTime = 0.0;
      if (this.mEndGameCondition == EndGameCondition.LevelComplete | this.mEndGameCondition == EndGameCondition.Victory | this.mEndGameCondition == EndGameCondition.EndOffGame | this.mEndGameCondition == EndGameCondition.ToBeContinued && state != NetworkState.Client && this.mSaveSlot != null)
      {
        int iLevel = (int) this.mSaveSlot.Level + 1;
        CampaignNode[] campaignNodeArray = SubMenuCharacterSelect.Instance.GameType == GameType.Mythos ? LevelManager.Instance.MythosCampaign : LevelManager.Instance.VanillaCampaign;
        if (iLevel >= campaignNodeArray.Length)
        {
          iLevel = 0;
          this.mSaveSlot.Looped = true;
        }
        this.mSaveSlot.Level = (byte) iLevel;
        SubMenuCharacterSelect.Instance.SetSettings(this.mGameType, iLevel, false);
        for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
        {
          if (Magicka.Game.Instance.Players[index].Playing)
          {
            this.mSaveSlot.UnlockedMagicks |= Magicka.Game.Instance.Players[index].UnlockedMagicks;
            PlayerSaveData playerSaveData = (PlayerSaveData) null;
            if (!this.mSaveSlot.Players.TryGetValue(Magicka.Game.Instance.Players[index].GamerTag, out playerSaveData))
            {
              playerSaveData = new PlayerSaveData();
              this.mSaveSlot.Players.Add(Magicka.Game.Instance.Players[index].GamerTag, playerSaveData);
            }
            playerSaveData.Staff = Magicka.Game.Instance.Players[index].Staff;
            playerSaveData.Weapon = Magicka.Game.Instance.Players[index].Weapon;
            this.mSaveSlot.Players[Magicka.Game.Instance.Players[index].GamerTag] = playerSaveData;
          }
        }
      }
      this.mHasEntered = false;
      if (state != NetworkState.Client && this.mSaveSlot != null)
        SaveManager.Instance.SaveCampaign();
      if (this.mEndGameCondition == EndGameCondition.ChallengeExit)
        Tome.Instance.PopMenu();
    }
    int length = Magicka.Game.Instance.Players.Length;
    for (int index = 0; index < length; ++index)
      Magicka.Game.Instance.Players[index].DeinitializeGame();
    SaveManager.Instance.SaveLeaderBoards();
    Profile.Instance.Write();
    BossFight.Instance.Clear();
  }

  public Level Level => this.mLevel;

  public bool StaffLight
  {
    get => this.mPlayerStaffLight;
    set => this.mPlayerStaffLight = value;
  }

  public MagickCamera Camera => this.mScene.Camera as MagickCamera;

  public EntityManager EntityManager => this.mEntityManager;

  public ContentManager Content => this.mContent;

  public List<SpellEffect> SpellEffects => this.mSpellEffects;

  public BossFight BossFight
  {
    get => this.mBossFight;
    set => this.mBossFight = value;
  }

  public GenericHealthBar GenericHealthBar
  {
    get => this.mGenericHealtBar;
    set => this.mGenericHealtBar = value;
  }

  public void OverlayPause(bool iPause)
  {
    if (iPause)
    {
      this.mOverlayIsPaused = true;
      AudioManager.Instance.PauseAll();
    }
    else
    {
      this.mOverlayIsPaused = false;
      AudioManager.Instance.PauseAll();
    }
  }

  internal void TogglePause(Controller iController)
  {
    if (InGameMenu.Visible && (InGameMenu.CurrentMenu is InGameMenuSurvivalStatistics || InGameMenu.CurrentMenu is InGameMenuTimedObjectiveStatistics))
      return;
    if (!this.mIsPaused)
    {
      InGameMenu.Show(iController);
      this.mIsPaused = true;
      AudioManager.Instance.PauseAll();
      ControlManager.Instance.LimitInput((object) this);
    }
    else
      InGameMenu.ControllerBack(iController);
  }

  public bool IsPaused => this.mIsPaused;

  public bool UIEnabled
  {
    get => this.mUIEnabled;
    set
    {
      this.mUIEnabled = value;
      KeyboardHUD.Instance.UIEnabled = this.mUIEnabled;
      Healthbars.Instance.UIEnabled = this.mUIEnabled;
    }
  }

  public static bool WaitingForPlayers
  {
    get => PlayState.sWaitingForPlayers;
    set => PlayState.sWaitingForPlayers = value;
  }

  public void IncrementBlizzardRainCount()
  {
    if (!(this.mLevel.CurrentScene.RuleSet is VersusRuleset))
      return;
    ++this.mBlizzardRainCount;
    if (this.mBlizzardRainCount < 20U)
      return;
    AchievementsManager.Instance.AwardAchievement(this, "swedishsummer");
  }

  public HitList[] ItsRainingBeastMen => this.mItsRainingBeastMen;

  public bool[] TooFancyForFireballs => this.mTooFancyForFireballs;

  public bool[] HasUsedMagick => this.mHasUsedMagick;

  internal void Endgame(ref GameEndMessage iMsg)
  {
    this.mEndGameActive = false;
    this.Endgame(iMsg.Condition, iMsg.Argument != 0, iMsg.Phony, iMsg.DelayTime);
  }

  internal SaveSlotInfo Info => this.mInfo;

  internal void ToggleChat()
  {
    if (NetworkManager.Instance.State != NetworkState.Offline)
    {
      if (NetworkChat.Instance.Active)
        NetworkChat.Instance.SendMessage();
      NetworkChat.Instance.Active = !NetworkChat.Instance.Active;
    }
    else
      NetworkChat.Instance.Active = false;
  }

  internal void PlayMusic(Banks iBank, int iCueID, float? iFocusValue)
  {
    this.mMusicBank = iBank;
    this.mMusicCue = iCueID;
    this.mMusicFocusValue = iFocusValue;
    AudioManager.Instance.PlayMusic(iBank, iCueID, iFocusValue);
  }

  private void SetSpawnFairies()
  {
    if (Magicka.Game.Instance.PlayerCount != 1 && (this.GameType == GameType.Mythos || this.GameType == GameType.Campaign))
      return;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
      this.mSpawnFairies[index] = players[index] != null && players[index].Avatar != null;
  }

  internal void SpawnFairies()
  {
    if (Magicka.Game.Instance.PlayerCount != 1 && (this.GameType == GameType.Mythos || this.GameType == GameType.Campaign))
      return;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index] != null && players[index].Avatar != null && this.mSpawnFairies[index])
        players[index].Avatar.RevivalFairy.Initialize(this, false);
    }
  }

  internal void RemoveFairyFrom(Avatar mOwner)
  {
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index] == mOwner.Player)
        this.mSpawnFairies[index] = false;
    }
  }

  protected class RenderData : IRenderableGUIObject
  {
    private Text mText;
    public float PostTextAlpha;
    private GUIBasicEffect mEffect;

    public RenderData(GUIBasicEffect iEffect, Text iText)
    {
      this.mEffect = iEffect;
      this.mText = iText;
    }

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      if ((double) this.PostTextAlpha > 1.4012984643248171E-45)
      {
        this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
        Vector4 vector4 = new Vector4();
        vector4.X = vector4.Y = vector4.Z = 1f;
        vector4.W = this.PostTextAlpha;
        this.mEffect.Color = vector4;
        this.mEffect.Begin();
        this.mEffect.CurrentTechnique.Passes[0].Begin();
        this.mText.Draw(this.mEffect, (float) screenSize.X * 0.5f, (float) (screenSize.Y - this.mText.Font.LineHeight) * 0.5f);
        this.mEffect.CurrentTechnique.Passes[0].End();
        this.mEffect.End();
      }
      if (NetworkManager.Instance.State == NetworkState.Offline)
        return;
      NetworkChat.Instance.Draw(ref new Vector2()
      {
        Y = (float) (screenSize.Y - NetworkChat.Instance.Size.Y) - 200f
      });
    }

    public int ZIndex => 999;
  }

  protected class CutsceneRenderData : IRenderableGUIObject
  {
    private Matrix mTransform;
    public float Time;
    private GUIBasicEffect mEffect;
    private Text mText;
    private string mSkipText;
    public float mSkipAlpha;
    private static VertexBuffer sVertexBuffer;
    private static VertexDeclaration sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionColor.VertexElements);
    private static int sVertexStride = VertexPositionColor.SizeInBytes;
    private static readonly int CUTSCENE_SKIP_TIP = "#cut_press_skip".GetHashCodeCustom();

    static CutsceneRenderData()
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        PlayState.CutsceneRenderData.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Defines.QUAD_COL_VERTS_TL.Length * PlayState.CutsceneRenderData.sVertexStride, BufferUsage.None);
        PlayState.CutsceneRenderData.sVertexBuffer.SetData<VertexPositionColor>(Defines.QUAD_COL_VERTS_TL);
      }
    }

    public CutsceneRenderData(GUIBasicEffect iEffect)
    {
      this.mEffect = iEffect;
      this.mTransform = Matrix.Identity;
      this.mText = new Text(100, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Right, false);
      this.mSkipText = (string) null;
    }

    public void UpdateText()
    {
      if (this.mSkipText != null)
        return;
      if (this.mSkipText == null)
      {
        this.mSkipText = LanguageManager.Instance.GetString(PlayState.CutsceneRenderData.CUTSCENE_SKIP_TIP);
        bool flag = false;
        Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
        for (int index = 0; index < players.Length; ++index)
        {
          if (players[index].Playing && !(players[index].Gamer is NetworkGamer))
          {
            flag = !(players[index].Controller is KeyboardMouseController);
            break;
          }
        }
        this.mSkipText = this.mSkipText.Replace("#1;", (!flag ? '̪' : '̤').ToString());
      }
      this.mText.SetText(this.mSkipText);
    }

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.mTransform.M11 = (float) screenSize.X;
      this.mTransform.M22 = (float) screenSize.Y * PlayState.CUTSCENE_BLACKBAR_SIZE;
      this.mTransform.M42 = (float) (-(double) this.mTransform.M22 + (double) this.mTransform.M22 * (double) this.Time);
      this.mEffect.TextureEnabled = false;
      this.mEffect.VertexColorEnabled = true;
      this.mEffect.Color = new Vector4(1f);
      this.mEffect.Transform = this.mTransform;
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(PlayState.CutsceneRenderData.sVertexBuffer, 0, PlayState.CutsceneRenderData.sVertexStride);
      this.mEffect.GraphicsDevice.VertexDeclaration = PlayState.CutsceneRenderData.sVertexDeclaration;
      this.mEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
      this.mEffect.Begin();
      this.mEffect.CurrentTechnique.Passes[0].Begin();
      this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      this.mTransform.M42 = (float) screenSize.Y - this.mTransform.M22 * this.Time;
      this.mEffect.Transform = this.mTransform;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
      Vector4 vector4 = new Vector4(1f, 1f, 1f, this.mSkipAlpha);
      this.mEffect.Color = vector4;
      this.mText.DefaultColor = vector4;
      this.mText.Draw(this.mEffect, (float) screenSize.X - 20f, (float) ((double) screenSize.Y - (double) screenSize.Y * (double) PlayState.CUTSCENE_BLACKBAR_SIZE * 0.5 - (double) this.mText.Font.LineHeight * 0.5));
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
      this.mEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    }

    public int ZIndex => 990;
  }

  private class State
  {
    private PlayState mPlayState;
    private MagickCamera.State mCamera;
    private bool[] mSpawnFairies = new bool[4];
    private Matrix[] mSpawnPoints;
    private DialogCollection.State mDialogState;
    private Level.State mLevel;
    private Banks mMusicBank;
    private int mMusicCue;
    private float? mMusicFocusValue;
    private List<int> mIgnoredTriggers = new List<int>(10);
    public string[] mWeapons = new string[4];
    public string[] mStaves = new string[4];
    public ulong mUnlockedMagicks;
    public bool[] mTooFancyForFireballs = new bool[4];
    public bool[] mHasUsedMagick = new bool[4];
    private bool mFirstStart;

    public State(PlayState iPlayState)
    {
      this.mPlayState = iPlayState;
      this.mCamera = new MagickCamera.State(this.mPlayState.mCamera);
      this.mLevel = new Level.State(this.mPlayState.mLevel);
      if (DialogManager.Instance.Dialogs != null)
        this.mDialogState = new DialogCollection.State(DialogManager.Instance.Dialogs);
      this.UpdateState((Matrix[]) null, (List<int>) null);
      this.mFirstStart = true;
    }

    public void UpdateState(Matrix[] iSpawnPoints, List<int> iIgnoredTriggers)
    {
      this.mFirstStart = false;
      this.mIgnoredTriggers.Clear();
      if (iIgnoredTriggers != null)
        this.mIgnoredTriggers.AddRange((IEnumerable<int>) iIgnoredTriggers);
      this.mSpawnPoints = iSpawnPoints;
      this.mCamera.UpdateState();
      this.mLevel.UpdateState();
      if (this.mDialogState != null)
        this.mDialogState.UpdateState();
      this.mMusicBank = this.mPlayState.mMusicBank;
      this.mMusicCue = this.mPlayState.mMusicCue;
      this.mMusicFocusValue = this.mPlayState.mMusicFocusValue;
      this.mUnlockedMagicks = 0UL;
      Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing)
        {
          this.mUnlockedMagicks |= players[index].UnlockedMagicks;
          if (this.mPlayState.GameType == GameType.Campaign | this.mPlayState.GameType == GameType.Mythos)
          {
            this.mWeapons[index] = players[index].Weapon;
            this.mStaves[index] = players[index].Staff;
            this.mHasUsedMagick[index] = this.mPlayState.HasUsedMagick[index];
            this.mTooFancyForFireballs[index] = this.mPlayState.TooFancyForFireballs[index];
          }
          this.mSpawnFairies[index] = players[index].Avatar != null && players[index].Avatar.RevivalFairy != null && players[index].Avatar.RevivalFairy.Active;
        }
        else
          this.mSpawnFairies[index] = false;
      }
      Array.Copy((Array) this.mSpawnFairies, (Array) this.mPlayState.mSpawnFairies, this.mPlayState.mSpawnFairies.Length);
    }

    public void ApplyState() => this.ApplyState((Action<float>) null);

    public unsafe void ApplyState(Action<float> reportProgressBackAction)
    {
      this.mPlayState.Inventory.Close((Magicka.GameLogic.Entities.Character) null);
      EffectManager.Instance.Clear();
      ControlManager.Instance.UnlimitInput();
      DamageNotifyer.Instance.Clear();
      BossFight.Instance.Reset();
      this.mPlayState.mEntityManager.Clear();
      TutorialManager.Instance.Reset();
      DecalManager.Instance.Clear();
      this.mPlayState.mEndGameCondition = EndGameCondition.None;
      this.mPlayState.mMusicBank = this.mMusicBank;
      this.mPlayState.mMusicCue = this.mMusicCue;
      this.mPlayState.mMusicFocusValue = this.mMusicFocusValue;
      AudioManager.Instance.StopMusic();
      AudioManager.Instance.PlayMusic(this.mMusicBank, this.mMusicCue, this.mMusicFocusValue);
      DialogManager.Instance.Reset();
      if (this.mDialogState != null)
        this.mDialogState.ApplyState();
      this.mLevel.ApplyState(this.mIgnoredTriggers, reportProgressBackAction);
      this.mPlayState.Camera.Release(1f / 1000f);
      this.mCamera.ApplyState();
      this.mPlayState.Camera.ClearPlayers();
      Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
      for (int index1 = 0; index1 < players.Length; ++index1)
      {
        Magicka.GameLogic.Player iPlayer = players[index1];
        if (iPlayer.Playing)
        {
          if ("weapon_unarmed".Equals(this.mWeapons[index1], StringComparison.OrdinalIgnoreCase) && (TutorialManager.Instance.EnabledElements & Elements.Basic) != Elements.None)
          {
            PlayerSaveData playerSaveData;
            this.mWeapons[index1] = !this.mPlayState.mInfo.Players.TryGetValue(iPlayer.GamerTag, out playerSaveData) ? "" : playerSaveData.Weapon;
          }
          if ("weapon_unarmed".Equals(this.mStaves[index1], StringComparison.OrdinalIgnoreCase) && (TutorialManager.Instance.EnabledElements & Elements.Basic) != Elements.None)
          {
            PlayerSaveData playerSaveData;
            this.mStaves[index1] = !this.mPlayState.mInfo.Players.TryGetValue(iPlayer.GamerTag, out playerSaveData) ? "" : playerSaveData.Weapon;
          }
          iPlayer.Weapon = this.mWeapons[index1];
          iPlayer.Staff = this.mStaves[index1];
          iPlayer.UnlockedMagicks = this.mUnlockedMagicks;
          this.mPlayState.HasUsedMagick[index1] = this.mHasUsedMagick[index1];
          this.mPlayState.TooFancyForFireballs[index1] = this.mTooFancyForFireballs[index1];
          if (iPlayer.Avatar != null)
            iPlayer.Avatar.Terminate(true, true);
          iPlayer.Avatar = Avatar.GetFromCache(iPlayer);
          iPlayer.Avatar.Initialize(CharacterTemplate.GetCachedTemplate(iPlayer.Gamer.Avatar.Type), new Vector3(), Magicka.GameLogic.Player.UNIQUE_ID[iPlayer.ID]);
          Array.Copy((Array) this.mSpawnFairies, (Array) this.mPlayState.mSpawnFairies, this.mPlayState.mSpawnFairies.Length);
          if (Magicka.Game.Instance.PlayerCount == 1 && this.mSpawnFairies[index1])
            iPlayer.Avatar.RevivalFairy.Initialize(this.mPlayState, false);
          else
            iPlayer.Avatar.RevivalFairy.Kill();
          if (iPlayer.Controller != null)
            iPlayer.Controller.Invert(false);
          if (!this.mPlayState.mLevel.SpawnPoint.SpawnPlayers)
          {
            iPlayer.Avatar.Body.DisableBody();
          }
          else
          {
            Matrix oLocator = new Matrix();
            if (this.mSpawnPoints != null)
              oLocator = this.mSpawnPoints[index1];
            else if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
            {
              int teamArea = (this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).GetTeamArea(iPlayer.Team);
              TriggerArea oArea;
              if (!this.mPlayState.mLevel.CurrentScene.TryGetLocator(teamArea, out oLocator) && this.mPlayState.mLevel.CurrentScene.TryGetTriggerArea(teamArea, out oArea))
                oLocator.Translation = oArea.GetRandomLocation();
            }
            else
            {
              SpawnPoint spawnPoint = this.mPlayState.mLevel.SpawnPoint;
              int index2;
              for (index2 = index1; index2 >= 0; --index2)
              {
                Locator locator;
                if (this.mPlayState.mLevel.CurrentScene.LevelModel.Locators.TryGetValue(spawnPoint.Locations[index2], out locator))
                {
                  oLocator = locator.Transform;
                  break;
                }
              }
              oLocator.M41 += 2f * (float) (index1 - index2);
              ++oLocator.M42;
            }
            Segment seg = new Segment();
            Transform transform = new Transform()
            {
              Orientation = oLocator
            };
            transform.Orientation.Translation = new Vector3();
            seg.Delta.Y = -5f;
            seg.Origin = oLocator.Translation;
            float num;
            Vector3 vector3_1;
            Vector3 vector3_2;
            if (!this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector3_1, out vector3_2, seg))
            {
              int index3 = 0;
              while (index3 < this.mPlayState.Level.CurrentScene.Level.CurrentScene.Liquids.Length && !this.mPlayState.Level.CurrentScene.Liquids[index3].SegmentIntersect(out num, out vector3_1, out vector3_2, ref seg, true, true, false))
                ++index3;
            }
            transform.Position = vector3_1;
            transform.Position.Y += iPlayer.Avatar.Radius + iPlayer.Avatar.Capsule.Length * 0.5f;
            iPlayer.Avatar.CharacterBody.MoveTo(transform.Position, transform.Orientation);
            this.mPlayState.mEntityManager.AddEntity((Entity) iPlayer.Avatar);
          }
        }
      }
      if (this.mFirstStart && this.mPlayState.mLevel.NoItems)
      {
        Item obj = this.mPlayState.Content.Load<Item>("Data/Items/Wizard/weapon_unarmed");
        for (int index = 0; index < players.Length; ++index)
        {
          if (players[index].Playing && players[index].Avatar != null)
          {
            Avatar avatar = players[index].Avatar;
            obj.Copy(avatar.Equipment[1].Item);
            obj.Copy(avatar.Equipment[0].Item);
          }
        }
      }
      if (NetworkManager.Instance.State == NetworkState.Offline)
        return;
      NetworkManager.Instance.Interface.Sync();
    }

    public void Read(BinaryReader iReader)
    {
      this.mFirstStart = false;
      this.mIgnoredTriggers.Clear();
      int num1 = iReader.ReadInt32();
      for (int index = 0; index < num1; ++index)
        this.mIgnoredTriggers.Add(iReader.ReadInt32());
      if (iReader.ReadBoolean())
      {
        this.mSpawnPoints = new Matrix[4];
        Vector3 up = Vector3.Up;
        for (int index = 0; index < 4; ++index)
          Matrix.CreateWorld(ref new Vector3()
          {
            X = iReader.ReadSingle(),
            Y = iReader.ReadSingle(),
            Z = iReader.ReadSingle()
          }, ref new Vector3()
          {
            X = iReader.ReadSingle(),
            Y = iReader.ReadSingle(),
            Z = iReader.ReadSingle()
          }, ref up, out this.mSpawnPoints[index]);
      }
      else
        this.mSpawnPoints = (Matrix[]) null;
      this.mCamera.Read(iReader);
      this.mLevel.Read(iReader);
      this.mDialogState.Read(iReader);
      this.mMusicBank = (Banks) iReader.ReadUInt16();
      this.mMusicCue = iReader.ReadInt32();
      this.mMusicFocusValue = !iReader.ReadBoolean() ? new float?() : new float?(iReader.ReadSingle());
      Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        this.mHasUsedMagick[index] = false;
        this.mTooFancyForFireballs[index] = false;
      }
      this.mUnlockedMagicks = iReader.ReadUInt64();
      int num2 = iReader.ReadInt32();
      for (int index1 = 0; index1 < num2; ++index1)
      {
        string str1 = iReader.ReadString();
        string str2 = iReader.ReadString();
        string str3 = iReader.ReadString();
        bool flag1 = iReader.ReadBoolean();
        bool flag2 = iReader.ReadBoolean();
        for (int index2 = 0; index2 < players.Length; ++index2)
        {
          if (players[index2].Playing && players[index2].GamerTag.Equals(str1))
          {
            this.mWeapons[index1] = str2;
            this.mStaves[index1] = str3;
            if (!string.IsNullOrEmpty(str2))
            {
              try
              {
                this.mPlayState.mContent.Load<Item>("data/items/wizard/" + str2);
              }
              catch
              {
                this.mPlayState.mContent.Load<Item>("data/items/npc/" + str2);
              }
            }
            if (!string.IsNullOrEmpty(str3))
            {
              try
              {
                this.mPlayState.mContent.Load<Item>("data/items/wizard/" + str3);
              }
              catch
              {
                this.mPlayState.mContent.Load<Item>("data/items/npc/" + str3);
              }
            }
            this.mHasUsedMagick[index1] = flag1;
            this.mTooFancyForFireballs[index1] = flag2;
          }
        }
      }
      bool flag = iReader.ReadBoolean();
      for (int index = 0; index < this.mSpawnFairies.Length; ++index)
        this.mSpawnFairies[index] = flag;
    }

    public void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.mIgnoredTriggers.Count);
      for (int index = 0; index < this.mIgnoredTriggers.Count; ++index)
        iWriter.Write(this.mIgnoredTriggers[index]);
      iWriter.Write(this.mSpawnPoints != null);
      if (this.mSpawnPoints != null)
      {
        for (int index = 0; index < 4; ++index)
        {
          Vector3 translation = this.mSpawnPoints[index].Translation;
          iWriter.Write(translation.X);
          iWriter.Write(translation.Y);
          iWriter.Write(translation.Z);
          Vector3 forward = this.mSpawnPoints[index].Forward;
          iWriter.Write(forward.X);
          iWriter.Write(forward.Y);
          iWriter.Write(forward.Z);
        }
      }
      this.mCamera.Write(iWriter);
      this.mLevel.Write(iWriter);
      this.mDialogState.Write(iWriter);
      iWriter.Write((ushort) this.mMusicBank);
      iWriter.Write(this.mMusicCue);
      iWriter.Write(this.mMusicFocusValue.HasValue);
      if (this.mMusicFocusValue.HasValue)
        iWriter.Write(this.mMusicFocusValue.Value);
      iWriter.Write(this.mUnlockedMagicks);
      iWriter.Write(Magicka.Game.Instance.PlayerCount);
      Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing)
        {
          iWriter.Write(players[index].GamerTag);
          iWriter.Write(players[index].Weapon != null ? players[index].Weapon : "");
          iWriter.Write(players[index].Staff != null ? players[index].Staff : "");
          iWriter.Write(this.mHasUsedMagick[index]);
          iWriter.Write(this.mTooFancyForFireballs[index]);
        }
      }
      bool flag = false;
      for (int index = 0; index < this.mSpawnFairies.Length; ++index)
        flag |= this.mSpawnFairies[index];
      iWriter.Write(flag);
    }
  }
}
