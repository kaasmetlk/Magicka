// Decompiled with JetBrains decompiler
// Type: Magicka.Game
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.Audio;
using Magicka.CoreFramework;
using Magicka.DRM;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.InGameMenus;
using Magicka.GameLogic.GameStates.Persistent;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.Network;
using Magicka.Storage;
using Magicka.WebTools;
using Magicka.WebTools.GameSparks;
using Magicka.WebTools.GameSparks.Platforms;
using Magicka.WebTools.Paradox;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.ParticleEffects;
using SteamWrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using XNAnimation.Effects;

#nullable disable
namespace Magicka;

public sealed class Game : Microsoft.Xna.Framework.Game
{
  private const float TARGET_FPS = 60f;
  private GraphicsDeviceManager mGraphics;
  private bool mChangeResolutionCallback;
  private KeyboardState mKeyboardState;
  private MouseState mMouseState;
  private object mKBStateLock = new object();
  private Form mForm;
  private bool mFocused;
  private Stopwatch mTest;
  private Thread mLogicThread;
  private bool mLogicThreadActive;
  private bool mRunLogic;
  private bool mUpdateSuspended;
  private bool mSuspendUpdate;
  private Thread mLoaderThread;
  private bool mLoaderThreadActive;
  private bool mLoaderThreadBusy;
  private bool mRunLoader;
  private Queue<Action> mLoaderQueue;
  private DataChannel mLogicDataChannel;
  private ulong mVersionID;
  private Player[] mPlayers;
  private static Game mSingelton;
  private static volatile object mSingeltonLock = new object();
  private bool mSuspendRendering;
  private bool mRenderingSuspended;
  private float mTimeSinceLastRender;
  private object mDataChannelLock = new object();
  private DataChannel mUsedByRender = DataChannel.None;
  private DataChannel mNextToRender = DataChannel.None;
  private Cursors mCursor;
  private bool mCursorActive;
  private bool mCursorVisible;
  private Cursor[,] mCursors;
  private Cursor mInvisibleCursor;
  private Action mUpdateCursor;
  private IServiceProvider mServiceProvider;

  public static Game Instance
  {
    get
    {
      if (Game.mSingelton == null)
      {
        lock (Game.mSingeltonLock)
        {
          if (Game.mSingelton == null)
            Game.mSingelton = new Game();
        }
      }
      return Game.mSingelton;
    }
  }

  public GraphicsDeviceManager GraphicsManager => this.mGraphics;

  [DllImport("kernel32")]
  private static extern int GetCurrentThreadId();

  [DllImport("User32.dll")]
  private static extern IntPtr LoadCursorFromFile(string str);

  private static Cursor CreateCursor(string filename)
  {
    IntPtr handle = Game.LoadCursorFromFile(filename);
    return !IntPtr.Zero.Equals((object) handle) ? new Cursor(handle) : throw new ApplicationException("Could not create cursor from file " + filename);
  }

  private Game()
  {
    this.mGraphics = new GraphicsDeviceManager((Microsoft.Xna.Framework.Game) this);
    string[] strArray = Application.ProductVersion.Split('.');
    ushort[] numArray = new ushort[4];
    for (int index = 0; index < strArray.Length; ++index)
      numArray[index] = ushort.Parse(strArray[index]);
    this.mVersionID = (ulong) ((long) numArray[0] << 48 /*0x30*/ | (long) numArray[1] << 32 /*0x20*/ | (long) numArray[2] << 16 /*0x10*/) | (ulong) numArray[3];
    Thread.CurrentThread.Name = "RenderThread";
    this.mLogicThread = new Thread(new ThreadStart(this.ThreadedUpdate));
    this.mLogicThread.Name = "LogicThread";
    this.mLoaderThread = new Thread(new ThreadStart(this.LoaderFunction));
    this.mLoaderThread.Name = "LoaderThread";
    this.mLoaderQueue = new Queue<Action>();
    this.mServiceProvider = this.Content.ServiceProvider;
    this.mTest = new Stopwatch();
    this.mGraphics.IsFullScreen = false;
    this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
    this.mGraphics.MinimumVertexShaderProfile = ShaderProfile.VS_3_0;
    this.mGraphics.MinimumPixelShaderProfile = ShaderProfile.PS_3_0;
    this.mGraphics.SynchronizeWithVerticalRetrace = true;
    this.mGraphics.PreferMultiSampling = false;
    this.mGraphics.PreferredBackBufferWidth = 800;
    this.mGraphics.PreferredBackBufferHeight = 600;
    this.mGraphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
    this.mGraphics.ApplyChanges();
    this.mGraphics.DeviceResetting += new EventHandler(this.mGraphics_DeviceResetting);
    this.mGraphics.DeviceReset += new EventHandler(this.mGraphics_DeviceReset);
    this.mGraphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(this.mGraphics_PreparingDeviceSettings);
    this.mForm = Control.FromHandle(this.Window.Handle) as Form;
    this.mForm.KeyPreview = true;
    this.mForm.MaximizeBox = false;
    try
    {
      int num = (1 << Environment.ProcessorCount) - 1;
      foreach (ProcessThread thread in (ReadOnlyCollectionBase) Process.GetCurrentProcess().Threads)
      {
        if (thread.Id == Game.GetCurrentThreadId())
        {
          thread.ProcessorAffinity = (IntPtr) num;
          thread.IdealProcessor = 0;
        }
      }
    }
    catch (Win32Exception ex)
    {
    }
    catch (PlatformNotSupportedException ex)
    {
    }
    catch (NotSupportedException ex)
    {
    }
    Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
  }

  private void mGraphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
  {
    e.GraphicsDeviceInformation.PresentationParameters.BackBufferCount = 1;
  }

  private void mGraphics_DeviceResetting(object sender, EventArgs e)
  {
    if (!this.mLogicThreadActive)
      return;
    this.mSuspendUpdate = true;
    while (!this.mUpdateSuspended)
      Thread.Sleep(1);
  }

  private void mGraphics_DeviceReset(object sender, EventArgs e) => this.mSuspendUpdate = false;

  internal ulong Version => this.mVersionID;

  private void UpdateCursor()
  {
    if (this.mCursorVisible)
      this.mForm.Cursor = this.mCursors[this.mCursorActive ? 0 : 1, (int) this.mCursor];
    else
      this.mForm.Cursor = this.mInvisibleCursor;
  }

  protected override void Initialize()
  {
    HackHelper.BeginCoreCheck();
    LevelManager.Instance.BeginComputeHashes();
    SaveManager.Instance.LoadSettings();
    string steamGameLanguage = GlobalSettings.Instance.SteamGameLanguage;
    string currentGameLanguage = SteamApps.GetCurrentGameLanguage();
    if (!currentGameLanguage.Equals(steamGameLanguage, StringComparison.OrdinalIgnoreCase))
    {
      GlobalSettings.Instance.SteamGameLanguage = currentGameLanguage;
      GlobalSettings.Instance.Language = LanguageManager.Instance.GetLanguage(currentGameLanguage);
    }
    LanguageManager.Instance.SetLanguage(GlobalSettings.Instance.Language);
    ResolutionData resolution = GlobalSettings.Instance.Resolution;
    if (resolution.Width != this.mGraphics.PreferredBackBufferWidth || resolution.Height != this.mGraphics.PreferredBackBufferHeight)
    {
      this.GraphicsDevice.PresentationParameters.FullScreenRefreshRateInHz = resolution.RefreshRate;
      this.mGraphics.PreferredBackBufferWidth = resolution.Width;
      this.mGraphics.PreferredBackBufferHeight = resolution.Height;
      this.mGraphics.IsFullScreen = GlobalSettings.Instance.Fullscreen;
      lock (this.GraphicsDevice)
        this.mGraphics.ApplyChanges();
    }
    this.AddLoadTask(new Action(Profile.Instance.Read));
    this.mPlayers = new Player[4];
    for (int iID = 0; iID < 4; ++iID)
      this.mPlayers[iID] = new Player(iID);
    Singleton<ParadoxServices>.Instance.Initialize();
    Singleton<GameSparksServices>.Instance.Initialize<GSWindowsPlatform>();
    Singleton<ParadoxAccount>.Instance.GameStartup((ParadoxAccountSequence.ExecutionDoneDelegate) ((iSuccess, iErrorCode) => TelemetryUtils.SendHardwareReport()));
    base.Initialize();
  }

  protected override void LoadContent()
  {
    this.Content.Dispose();
    this.Content = new ContentManager(this.mServiceProvider, "content");
    this.mCursors = new Cursor[2, 6];
    this.mCursors[0, 0] = Game.CreateCursor("./Cursors/Default.ani");
    this.mCursors[1, 0] = this.mCursors[0, 0];
    this.mCursors[0, 1] = Game.CreateCursor("./Cursors/Talk.ani");
    this.mCursors[1, 1] = Game.CreateCursor("./Cursors/TalkGray.ani");
    this.mCursors[0, 2] = Game.CreateCursor("./Cursors/Examine.ani");
    this.mCursors[1, 2] = Game.CreateCursor("./Cursors/ExamineGray.ani");
    this.mCursors[0, 3] = Game.CreateCursor("./Cursors/PickUp.ani");
    this.mCursors[1, 3] = Game.CreateCursor("./Cursors/PickUpGray.ani");
    this.mCursors[0, 4] = Game.CreateCursor("./Cursors/Attack.ani");
    this.mCursors[1, 4] = this.mCursors[0, 4];
    this.mCursors[0, 5] = Game.CreateCursor("./Cursors/Interact.ani");
    this.mCursors[1, 5] = Game.CreateCursor("./Cursors/InteractGray.ani");
    this.mInvisibleCursor = Game.CreateCursor("./Cursors/Invisible.ani");
    this.mUpdateCursor = new Action(this.UpdateCursor);
    this.IsMouseVisible = true;
    this.SetCursor(true, Cursors.Default);
    base.IsMouseVisible = true;
    AudioManager.Instance.StartInit(this.Content.Load<List<string>>("Audio/SoundList"));
    AudioManager.Instance.VolumeMusic(GlobalSettings.Instance.VolumeMusic);
    AudioManager.Instance.VolumeSound(GlobalSettings.Instance.VolumeSound);
    this.AddLoadTask(new Action(EffectManager.Instance.Initialize));
    RenderManager.Instance.Initialize(this.GraphicsDevice);
    this.mRunLoader = true;
    this.mLoaderThread.Start();
    DummyEffect dummyEffect = new DummyEffect(Game.Instance.GraphicsDevice, this.Content.Load<Effect>("Shaders/DummyEffect"));
    SkinnedModelBasicEffect.DefaultEffectPool = dummyEffect.EffectPool;
    RenderManager.Instance.LocalDummyEffect = dummyEffect;
    RenderManager.Instance.RegisterEffect((Effect) new RenderDeferredEffect(this.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
    RenderManager.Instance.RegisterEffect((Effect) new AdditiveEffect(this.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
    RenderManager.Instance.RegisterEffect((Effect) new SkinnedModelBasicEffect(this.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
    RenderManager.Instance.RegisterEffect((Effect) new SkinnedModelSkeletonEffect(this.GraphicsDevice, this.Content));
    RenderManager.Instance.RegisterEffect((Effect) new EntangleEffect(this.GraphicsDevice, this.Content));
    RenderManager.Instance.RegisterEffect((Effect) new SkinnedShieldEffect(this.GraphicsDevice, this.Content));
    ParticleSystem.Instance.Initialize(this.GraphicsDevice, this.Content.Load<Texture3D>("EffectTextures/ParticlesA"), this.Content.Load<Texture3D>("EffectTextures/ParticlesB"), this.Content.Load<Texture3D>("EffectTextures/ParticlesC"), this.Content.Load<Texture3D>("EffectTextures/ParticlesD"));
    CompanyState iGameState = new CompanyState();
    iGameState.Initialize();
    GameStateManager.Instance.PushState((GameState) iGameState);
  }

  protected override void BeginRun()
  {
    this.mRunLogic = true;
    this.mLogicThread.Start();
    base.BeginRun();
  }

  private DataChannel GetNextChannelToRender()
  {
    lock (this.mDataChannelLock)
    {
      this.mUsedByRender = this.mNextToRender;
      this.mNextToRender = DataChannel.None;
      return this.mUsedByRender;
    }
  }

  private DataChannel GetNextChannelToUpdate()
  {
    lock (this.mDataChannelLock)
    {
      switch (this.mNextToRender)
      {
        case DataChannel.A:
          return this.mUsedByRender == DataChannel.B ? DataChannel.C : DataChannel.B;
        case DataChannel.B:
          return this.mUsedByRender == DataChannel.A ? DataChannel.C : DataChannel.A;
        case DataChannel.C:
          return this.mUsedByRender == DataChannel.A ? DataChannel.B : DataChannel.A;
        default:
          if (this.mUsedByRender == DataChannel.A)
            return DataChannel.B;
          return this.mUsedByRender == DataChannel.B ? DataChannel.C : DataChannel.A;
      }
    }
  }

  private void UpdateDataChannelFinished(DataChannel iOldChannel)
  {
    lock (this.mDataChannelLock)
      this.mNextToRender = iOldChannel;
  }

  protected override void UnloadContent() => AudioManager.Instance.Dispose();

  private void Update(float iDeltaTime)
  {
    SteamAPI.RunCallbacks();
    this.mTimeSinceLastRender += iDeltaTime;
    this.mLogicDataChannel = this.GetNextChannelToUpdate();
    GameStateManager.Instance.Update(this.mLogicDataChannel, iDeltaTime);
    NetworkManager.Instance.Update();
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.FlushMessageBuffers();
    AchievementsManager.Instance.Update(this.mLogicDataChannel, iDeltaTime);
    Logger.LogDebug(Logger.Source.Threads, $"Update on Thread {Thread.CurrentThread.Name} \n");
    this.UpdateDataChannelFinished(this.mLogicDataChannel);
    this.mLogicDataChannel = DataChannel.None;
    Singleton<ParadoxServices>.Instance.Update();
    Singleton<GameSparksServices>.Instance.Update();
    Singleton<ParadoxAccount>.Instance.Update();
  }

  private void LoaderFunction()
  {
    while (this.mGraphics.GraphicsDevice == null)
      Thread.Sleep(1);
    this.mLoaderThreadActive = true;
    while (this.mRunLoader)
    {
      if (this.mLoaderQueue.Count > 0)
      {
        this.IsFixedTimeStep = true;
        this.mLoaderThreadBusy = true;
        this.mLoaderQueue.Dequeue()();
      }
      else
      {
        this.IsFixedTimeStep = false;
        this.mLoaderThreadBusy = false;
      }
      Thread.Sleep(1);
    }
    this.mLoaderThreadActive = false;
  }

  public void AddLoadTask(Action iTask) => this.mLoaderQueue.Enqueue(iTask);

  private void ThreadedUpdate()
  {
    this.mLogicThreadActive = true;
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    while (this.mRunLogic)
    {
      while (this.mSuspendUpdate)
      {
        this.mUpdateSuspended = true;
        Thread.Sleep(1);
      }
      this.mUpdateSuspended = false;
      TimeSpan elapsed = stopwatch.Elapsed;
      stopwatch.Stop();
      stopwatch.Reset();
      stopwatch.Start();
      float iDeltaTime = (float) elapsed.TotalSeconds;
      if (elapsed.TotalSeconds > 0.05)
        iDeltaTime = 0.05f;
      AudioManager.Instance.Update(iDeltaTime);
      this.Update(iDeltaTime);
      Thread.Sleep(1);
      while (((!this.mRenderingSuspended & this.mNextToRender != DataChannel.None ? 1 : 0) & ((double) this.mTimeSinceLastRender <= 0.20000000298023224 ? 0 : (this.mRunLogic ? 1 : 0))) != 0)
      {
        this.mUpdateSuspended = true;
        Thread.Sleep(1);
      }
      this.mUpdateSuspended = false;
    }
    this.mLogicThreadActive = false;
  }

  protected override void EndRun()
  {
    try
    {
      GameStateManager.Instance.CurrentState.OnEnter();
    }
    catch
    {
    }
    this.mRunLogic = false;
    while (this.mLogicThreadActive)
      Thread.Sleep(1);
    this.mRunLoader = false;
    while (this.mLoaderThreadActive)
      Thread.Sleep(1);
    NetworkManager.Instance.Dispose();
    Singleton<ParadoxServices>.Instance.Dispose();
    Singleton<GameSparksServices>.Instance.Dispose();
    base.EndRun();
  }

  public int PlayerCount
  {
    get
    {
      int playerCount = 0;
      for (int index = 0; index < 4; ++index)
      {
        if (this.mPlayers[index].Playing)
          ++playerCount;
      }
      return playerCount;
    }
  }

  public int LoadTaskCount => this.mLoaderQueue.Count + (this.mLoaderThreadBusy ? 1 : 0);

  protected override void Update(GameTime gameTime)
  {
    ResolutionData resolution = GlobalSettings.Instance.Resolution;
    if (resolution.Width != this.mGraphics.PreferredBackBufferWidth || resolution.Height != this.mGraphics.PreferredBackBufferHeight)
    {
      this.GraphicsDevice.PresentationParameters.FullScreenRefreshRateInHz = resolution.RefreshRate;
      this.mGraphics.PreferredBackBufferWidth = resolution.Width;
      this.mGraphics.PreferredBackBufferHeight = resolution.Height;
      this.mGraphics.ApplyChanges();
    }
    else if (GlobalSettings.Instance.VSync != this.mGraphics.SynchronizeWithVerticalRetrace)
    {
      this.mGraphics.SynchronizeWithVerticalRetrace = GlobalSettings.Instance.VSync;
      this.mGraphics.ApplyChanges();
    }
    this.mFocused = this.mForm.Focused;
    Cursor.Clip = !this.mFocused || !(GameStateManager.Instance.CurrentState is PlayState) || ControlManager.Instance.MenuController.Player == null || !ControlManager.Instance.MenuController.Player.Playing || InGameMenu.Visible ? new System.Drawing.Rectangle() : this.mForm.RectangleToScreen(this.mForm.ClientRectangle);
    if (GlobalSettings.Instance.Fullscreen != this.mGraphics.IsFullScreen)
    {
      this.mForm.MaximizeBox = true;
      this.mGraphics.ToggleFullScreen();
      this.mChangeResolutionCallback = true;
    }
    else if (this.mChangeResolutionCallback)
    {
      Tome.ChangeResolution(GlobalSettings.Instance.Resolution);
      this.mChangeResolutionCallback = false;
      this.mForm.MaximizeBox = false;
    }
    base.Update(gameTime);
  }

  protected override bool BeginDraw() => true;

  protected override void EndDraw()
  {
  }

  protected override void Draw(GameTime gameTime)
  {
    if (this.mSuspendRendering)
    {
      this.mRenderingSuspended = true;
    }
    else
    {
      if (!base.BeginDraw())
        return;
      this.mRenderingSuspended = false;
      GameState currentState = GameStateManager.Instance.CurrentState;
      PersistentGameState persistentState = GameStateManager.Instance.PersistentState;
      if (currentState == null)
        return;
      lock (this.mKBStateLock)
      {
        this.mMouseState = Mouse.GetState();
        this.mKeyboardState = Keyboard.GetState();
      }
      DataChannel nextChannelToRender = this.GetNextChannelToRender();
      Logger.LogDebug(Logger.Source.Threads, $"Draw on Thread {Thread.CurrentThread.Name} \n");
      while (nextChannelToRender == DataChannel.None)
      {
        nextChannelToRender = this.GetNextChannelToRender();
        Thread.Sleep(1);
        lock (this.mKBStateLock)
        {
          this.mKeyboardState = Keyboard.GetState();
          this.mMouseState = Mouse.GetState();
        }
      }
      lock (this.GraphicsDevice)
      {
        RenderManager.Instance.RenderScene(currentState.Scene, nextChannelToRender, ref gameTime, persistentState.Scene);
        base.Draw(gameTime);
        this.mUsedByRender = DataChannel.None;
      }
      base.EndDraw();
      this.mTimeSinceLastRender = 0.0f;
    }
  }

  public Player[] Players => this.mPlayers;

  public DataChannel UpdatingDataChannel => this.mLogicDataChannel;

  public void DisableRendering() => this.mSuspendRendering = true;

  public void EnableRendering() => this.mSuspendRendering = false;

  public bool RenderingEnabled => !(this.mSuspendRendering & this.mRenderingSuspended);

  public Form Form => this.mForm;

  public KeyboardState KeyboardState
  {
    get
    {
      lock (this.mKBStateLock)
        return this.mKeyboardState;
    }
  }

  public MouseState MouseState
  {
    get
    {
      lock (this.mKBStateLock)
        return this.mMouseState;
    }
  }

  public bool Focused => this.mFocused;

  public void SetCursor(bool iActive, Cursors iCursor)
  {
    if (!(this.mCursorActive != iActive | this.mCursor != iCursor))
      return;
    this.mCursorActive = iActive;
    this.mCursor = iCursor;
    if (this.mForm.IsDisposed)
      return;
    this.mForm.BeginInvoke((Delegate) this.mUpdateCursor);
  }

  public new bool IsMouseVisible
  {
    get => this.mCursorVisible;
    set
    {
      if (value == this.mCursorVisible)
        return;
      this.mCursorVisible = value;
      this.mForm.BeginInvoke((Delegate) this.mUpdateCursor);
    }
  }
}
