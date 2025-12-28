using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
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
using XNAnimation.Effects;

namespace Magicka
{
	// Token: 0x02000094 RID: 148
	public sealed class Game : Game
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000443 RID: 1091 RVA: 0x00014CE4 File Offset: 0x00012EE4
		public static Game Instance
		{
			get
			{
				if (Game.mSingelton == null)
				{
					lock (Game.mSingeltonLock)
					{
						if (Game.mSingelton == null)
						{
							Game.mSingelton = new Game();
						}
					}
				}
				return Game.mSingelton;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x00014D38 File Offset: 0x00012F38
		public GraphicsDeviceManager GraphicsManager
		{
			get
			{
				return this.mGraphics;
			}
		}

		// Token: 0x06000445 RID: 1093
		[DllImport("kernel32")]
		private static extern int GetCurrentThreadId();

		// Token: 0x06000446 RID: 1094
		[DllImport("User32.dll")]
		private static extern IntPtr LoadCursorFromFile(string str);

		// Token: 0x06000447 RID: 1095 RVA: 0x00014D40 File Offset: 0x00012F40
		private static Cursor CreateCursor(string filename)
		{
			IntPtr intPtr = Game.LoadCursorFromFile(filename);
			if (!IntPtr.Zero.Equals(intPtr))
			{
				return new Cursor(intPtr);
			}
			throw new ApplicationException("Could not create cursor from file " + filename);
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00014D88 File Offset: 0x00012F88
		private Game()
		{
			this.mGraphics = new GraphicsDeviceManager(this);
			string productVersion = Application.ProductVersion;
			string[] array = productVersion.Split(new char[]
			{
				'.'
			});
			ushort[] array2 = new ushort[4];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = ushort.Parse(array[i]);
			}
			this.mVersionID = ((ulong)array2[0] << 48 | (ulong)array2[1] << 32 | (ulong)array2[2] << 16 | (ulong)array2[3]);
			Thread.CurrentThread.Name = "RenderThread";
			this.mLogicThread = new Thread(new ThreadStart(this.ThreadedUpdate));
			this.mLogicThread.Name = "LogicThread";
			this.mLoaderThread = new Thread(new ThreadStart(this.LoaderFunction));
			this.mLoaderThread.Name = "LoaderThread";
			this.mLoaderQueue = new Queue<Action>();
			this.mServiceProvider = base.Content.ServiceProvider;
			this.mTest = new Stopwatch();
			this.mGraphics.IsFullScreen = false;
			base.TargetElapsedTime = TimeSpan.FromSeconds(0.016666666666666666);
			this.mGraphics.MinimumVertexShaderProfile = ShaderProfile.VS_3_0;
			this.mGraphics.MinimumPixelShaderProfile = ShaderProfile.PS_3_0;
			this.mGraphics.SynchronizeWithVerticalRetrace = true;
			this.mGraphics.PreferMultiSampling = false;
			this.mGraphics.PreferredBackBufferWidth = 800;
			this.mGraphics.PreferredBackBufferHeight = 600;
			this.mGraphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
			this.mGraphics.ApplyChanges();
			this.mGraphics.DeviceResetting += this.mGraphics_DeviceResetting;
			this.mGraphics.DeviceReset += this.mGraphics_DeviceReset;
			this.mGraphics.PreparingDeviceSettings += this.mGraphics_PreparingDeviceSettings;
			this.mForm = (Control.FromHandle(base.Window.Handle) as Form);
			this.mForm.KeyPreview = true;
			this.mForm.MaximizeBox = false;
			try
			{
				int value = (1 << Environment.ProcessorCount) - 1;
				foreach (object obj in Process.GetCurrentProcess().Threads)
				{
					ProcessThread processThread = (ProcessThread)obj;
					if (processThread.Id == Game.GetCurrentThreadId())
					{
						processThread.ProcessorAffinity = (IntPtr)value;
						processThread.IdealProcessor = 0;
					}
				}
			}
			catch (Win32Exception)
			{
			}
			catch (PlatformNotSupportedException)
			{
			}
			catch (NotSupportedException)
			{
			}
			Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x0001506C File Offset: 0x0001326C
		private void mGraphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.BackBufferCount = 1;
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x0001507F File Offset: 0x0001327F
		private void mGraphics_DeviceResetting(object sender, EventArgs e)
		{
			if (this.mLogicThreadActive)
			{
				this.mSuspendUpdate = true;
				while (!this.mUpdateSuspended)
				{
					Thread.Sleep(1);
				}
			}
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x000150A0 File Offset: 0x000132A0
		private void mGraphics_DeviceReset(object sender, EventArgs e)
		{
			this.mSuspendUpdate = false;
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600044C RID: 1100 RVA: 0x000150A9 File Offset: 0x000132A9
		internal ulong Version
		{
			get
			{
				return this.mVersionID;
			}
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x000150B4 File Offset: 0x000132B4
		private void UpdateCursor()
		{
			if (this.mCursorVisible)
			{
				this.mForm.Cursor = this.mCursors[this.mCursorActive ? 0 : 1, (int)this.mCursor];
				return;
			}
			this.mForm.Cursor = this.mInvisibleCursor;
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x0001510C File Offset: 0x0001330C
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
				base.GraphicsDevice.PresentationParameters.FullScreenRefreshRateInHz = resolution.RefreshRate;
				this.mGraphics.PreferredBackBufferWidth = resolution.Width;
				this.mGraphics.PreferredBackBufferHeight = resolution.Height;
				this.mGraphics.IsFullScreen = GlobalSettings.Instance.Fullscreen;
				lock (base.GraphicsDevice)
				{
					this.mGraphics.ApplyChanges();
				}
			}
			this.AddLoadTask(new Action(Profile.Instance.Read));
			this.mPlayers = new Player[4];
			for (int i = 0; i < 4; i++)
			{
				this.mPlayers[i] = new Player(i);
			}
			Singleton<ParadoxServices>.Instance.Initialize();
			Singleton<GameSparksServices>.Instance.Initialize<GSWindowsPlatform>();
			Singleton<ParadoxAccount>.Instance.GameStartup(delegate(bool iSuccess, ParadoxAccount.ErrorCode iErrorCode)
			{
				TelemetryUtils.SendHardwareReport();
			});
			base.Initialize();
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x000152B4 File Offset: 0x000134B4
		protected override void LoadContent()
		{
			base.Content.Dispose();
			base.Content = new ContentManager(this.mServiceProvider, "content");
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
			AudioManager.Instance.StartInit(base.Content.Load<List<string>>("Audio/SoundList"));
			AudioManager.Instance.VolumeMusic(GlobalSettings.Instance.VolumeMusic);
			AudioManager.Instance.VolumeSound(GlobalSettings.Instance.VolumeSound);
			this.AddLoadTask(new Action(EffectManager.Instance.Initialize));
			RenderManager.Instance.Initialize(base.GraphicsDevice);
			this.mRunLoader = true;
			this.mLoaderThread.Start();
			DummyEffect dummyEffect = new DummyEffect(Game.Instance.GraphicsDevice, base.Content.Load<Effect>("Shaders/DummyEffect"));
			SkinnedModelBasicEffect.DefaultEffectPool = dummyEffect.EffectPool;
			RenderManager.Instance.LocalDummyEffect = dummyEffect;
			RenderManager.Instance.RegisterEffect(new RenderDeferredEffect(base.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
			RenderManager.Instance.RegisterEffect(new AdditiveEffect(base.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
			RenderManager.Instance.RegisterEffect(new SkinnedModelBasicEffect(base.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
			RenderManager.Instance.RegisterEffect(new SkinnedModelSkeletonEffect(base.GraphicsDevice, base.Content));
			RenderManager.Instance.RegisterEffect(new EntangleEffect(base.GraphicsDevice, base.Content));
			RenderManager.Instance.RegisterEffect(new SkinnedShieldEffect(base.GraphicsDevice, base.Content));
			Texture3D iParticleSheetA = base.Content.Load<Texture3D>("EffectTextures/ParticlesA");
			Texture3D iParticleSheetB = base.Content.Load<Texture3D>("EffectTextures/ParticlesB");
			Texture3D iParticleSheetC = base.Content.Load<Texture3D>("EffectTextures/ParticlesC");
			Texture3D iParticleSheetD = base.Content.Load<Texture3D>("EffectTextures/ParticlesD");
			ParticleSystem.Instance.Initialize(base.GraphicsDevice, iParticleSheetA, iParticleSheetB, iParticleSheetC, iParticleSheetD);
			CompanyState companyState = new CompanyState();
			companyState.Initialize();
			GameStateManager.Instance.PushState(companyState);
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x00015628 File Offset: 0x00013828
		protected override void BeginRun()
		{
			this.mRunLogic = true;
			this.mLogicThread.Start();
			base.BeginRun();
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00015644 File Offset: 0x00013844
		private DataChannel GetNextChannelToRender()
		{
			DataChannel result;
			lock (this.mDataChannelLock)
			{
				this.mUsedByRender = this.mNextToRender;
				this.mNextToRender = DataChannel.None;
				result = this.mUsedByRender;
			}
			return result;
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00015694 File Offset: 0x00013894
		private DataChannel GetNextChannelToUpdate()
		{
			DataChannel result;
			lock (this.mDataChannelLock)
			{
				switch (this.mNextToRender)
				{
				default:
					if (this.mUsedByRender == DataChannel.A)
					{
						result = DataChannel.B;
					}
					else if (this.mUsedByRender == DataChannel.B)
					{
						result = DataChannel.C;
					}
					else
					{
						result = DataChannel.A;
					}
					break;
				case DataChannel.A:
					if (this.mUsedByRender == DataChannel.B)
					{
						result = DataChannel.C;
					}
					else
					{
						result = DataChannel.B;
					}
					break;
				case DataChannel.B:
					if (this.mUsedByRender == DataChannel.A)
					{
						result = DataChannel.C;
					}
					else
					{
						result = DataChannel.A;
					}
					break;
				case DataChannel.C:
					if (this.mUsedByRender == DataChannel.A)
					{
						result = DataChannel.B;
					}
					else
					{
						result = DataChannel.A;
					}
					break;
				}
			}
			return result;
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00015734 File Offset: 0x00013934
		private void UpdateDataChannelFinished(DataChannel iOldChannel)
		{
			lock (this.mDataChannelLock)
			{
				this.mNextToRender = iOldChannel;
			}
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00015770 File Offset: 0x00013970
		protected override void UnloadContent()
		{
			AudioManager.Instance.Dispose();
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x0001577C File Offset: 0x0001397C
		private void Update(float iDeltaTime)
		{
			SteamAPI.RunCallbacks();
			this.mTimeSinceLastRender += iDeltaTime;
			this.mLogicDataChannel = this.GetNextChannelToUpdate();
			GameStateManager.Instance.Update(this.mLogicDataChannel, iDeltaTime);
			NetworkManager.Instance.Update();
			if (NetworkManager.Instance.State != NetworkState.Offline)
			{
				NetworkManager.Instance.Interface.FlushMessageBuffers();
			}
			AchievementsManager.Instance.Update(this.mLogicDataChannel, iDeltaTime);
			Logger.LogDebug(Logger.Source.Threads, string.Format("Update on Thread {0} \n", Thread.CurrentThread.Name));
			this.UpdateDataChannelFinished(this.mLogicDataChannel);
			this.mLogicDataChannel = DataChannel.None;
			Singleton<ParadoxServices>.Instance.Update();
			Singleton<GameSparksServices>.Instance.Update();
			Singleton<ParadoxAccount>.Instance.Update();
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0001583C File Offset: 0x00013A3C
		private void LoaderFunction()
		{
			while (this.mGraphics.GraphicsDevice == null)
			{
				Thread.Sleep(1);
			}
			this.mLoaderThreadActive = true;
			while (this.mRunLoader)
			{
				if (this.mLoaderQueue.Count > 0)
				{
					base.IsFixedTimeStep = true;
					this.mLoaderThreadBusy = true;
					this.mLoaderQueue.Dequeue().Invoke();
				}
				else
				{
					base.IsFixedTimeStep = false;
					this.mLoaderThreadBusy = false;
				}
				Thread.Sleep(1);
			}
			this.mLoaderThreadActive = false;
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x000158B8 File Offset: 0x00013AB8
		public void AddLoadTask(Action iTask)
		{
			this.mLoaderQueue.Enqueue(iTask);
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x000158C8 File Offset: 0x00013AC8
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
				float iDeltaTime = (float)elapsed.TotalSeconds;
				if (elapsed.TotalSeconds > 0.05)
				{
					iDeltaTime = 0.05f;
				}
				AudioManager.Instance.Update(iDeltaTime);
				this.Update(iDeltaTime);
				Thread.Sleep(1);
				while (!this.mRenderingSuspended & this.mNextToRender != DataChannel.None & (this.mTimeSinceLastRender > 0.2f && this.mRunLogic))
				{
					this.mUpdateSuspended = true;
					Thread.Sleep(1);
				}
				this.mUpdateSuspended = false;
			}
			this.mLogicThreadActive = false;
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x000159B4 File Offset: 0x00013BB4
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
			{
				Thread.Sleep(1);
			}
			this.mRunLoader = false;
			while (this.mLoaderThreadActive)
			{
				Thread.Sleep(1);
			}
			NetworkManager.Instance.Dispose();
			Singleton<ParadoxServices>.Instance.Dispose();
			Singleton<GameSparksServices>.Instance.Dispose();
			base.EndRun();
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x00015A38 File Offset: 0x00013C38
		public int PlayerCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < 4; i++)
				{
					if (this.mPlayers[i].Playing)
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x0600045B RID: 1115 RVA: 0x00015A67 File Offset: 0x00013C67
		public int LoadTaskCount
		{
			get
			{
				return this.mLoaderQueue.Count + (this.mLoaderThreadBusy ? 1 : 0);
			}
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00015A84 File Offset: 0x00013C84
		protected override void Update(GameTime gameTime)
		{
			ResolutionData resolution = GlobalSettings.Instance.Resolution;
			if (resolution.Width != this.mGraphics.PreferredBackBufferWidth || resolution.Height != this.mGraphics.PreferredBackBufferHeight)
			{
				base.GraphicsDevice.PresentationParameters.FullScreenRefreshRateInHz = resolution.RefreshRate;
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
			if (this.mFocused && GameStateManager.Instance.CurrentState is PlayState && ControlManager.Instance.MenuController.Player != null && ControlManager.Instance.MenuController.Player.Playing && !InGameMenu.Visible)
			{
				Cursor.Clip = this.mForm.RectangleToScreen(this.mForm.ClientRectangle);
			}
			else
			{
				Cursor.Clip = default(System.Drawing.Rectangle);
			}
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

		// Token: 0x0600045D RID: 1117 RVA: 0x00015C2E File Offset: 0x00013E2E
		protected override bool BeginDraw()
		{
			return true;
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x00015C31 File Offset: 0x00013E31
		protected override void EndDraw()
		{
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00015C34 File Offset: 0x00013E34
		protected override void Draw(GameTime gameTime)
		{
			if (this.mSuspendRendering)
			{
				this.mRenderingSuspended = true;
				return;
			}
			if (!base.BeginDraw())
			{
				return;
			}
			this.mRenderingSuspended = false;
			GameState currentState = GameStateManager.Instance.CurrentState;
			PersistentGameState persistentState = GameStateManager.Instance.PersistentState;
			if (currentState == null)
			{
				return;
			}
			lock (this.mKBStateLock)
			{
				this.mMouseState = Mouse.GetState();
				this.mKeyboardState = Keyboard.GetState();
			}
			DataChannel nextChannelToRender = this.GetNextChannelToRender();
			Logger.LogDebug(Logger.Source.Threads, string.Format("Draw on Thread {0} \n", Thread.CurrentThread.Name));
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
			lock (base.GraphicsDevice)
			{
				RenderManager.Instance.RenderScene(currentState.Scene, nextChannelToRender, ref gameTime, persistentState.Scene);
				base.Draw(gameTime);
				this.mUsedByRender = DataChannel.None;
			}
			base.EndDraw();
			this.mTimeSinceLastRender = 0f;
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x00015D84 File Offset: 0x00013F84
		public Player[] Players
		{
			get
			{
				return this.mPlayers;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x00015D8C File Offset: 0x00013F8C
		public DataChannel UpdatingDataChannel
		{
			get
			{
				return this.mLogicDataChannel;
			}
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00015D94 File Offset: 0x00013F94
		public void DisableRendering()
		{
			this.mSuspendRendering = true;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x00015D9D File Offset: 0x00013F9D
		public void EnableRendering()
		{
			this.mSuspendRendering = false;
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x00015DA6 File Offset: 0x00013FA6
		public bool RenderingEnabled
		{
			get
			{
				return !(this.mSuspendRendering & this.mRenderingSuspended);
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000465 RID: 1125 RVA: 0x00015DB8 File Offset: 0x00013FB8
		public Form Form
		{
			get
			{
				return this.mForm;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x00015DC0 File Offset: 0x00013FC0
		public KeyboardState KeyboardState
		{
			get
			{
				KeyboardState result;
				lock (this.mKBStateLock)
				{
					result = this.mKeyboardState;
				}
				return result;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x00015DFC File Offset: 0x00013FFC
		public MouseState MouseState
		{
			get
			{
				MouseState result;
				lock (this.mKBStateLock)
				{
					result = this.mMouseState;
				}
				return result;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x00015E38 File Offset: 0x00014038
		public bool Focused
		{
			get
			{
				return this.mFocused;
			}
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x00015E40 File Offset: 0x00014040
		public void SetCursor(bool iActive, Cursors iCursor)
		{
			if (this.mCursorActive != iActive | this.mCursor != iCursor)
			{
				this.mCursorActive = iActive;
				this.mCursor = iCursor;
				if (!this.mForm.IsDisposed)
				{
					this.mForm.BeginInvoke(this.mUpdateCursor);
				}
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x00015E95 File Offset: 0x00014095
		// (set) Token: 0x0600046B RID: 1131 RVA: 0x00015E9D File Offset: 0x0001409D
		public new bool IsMouseVisible
		{
			get
			{
				return this.mCursorVisible;
			}
			set
			{
				if (value != this.mCursorVisible)
				{
					this.mCursorVisible = value;
					this.mForm.BeginInvoke(this.mUpdateCursor);
				}
			}
		}

		// Token: 0x040002C9 RID: 713
		private const float TARGET_FPS = 60f;

		// Token: 0x040002CA RID: 714
		private GraphicsDeviceManager mGraphics;

		// Token: 0x040002CB RID: 715
		private bool mChangeResolutionCallback;

		// Token: 0x040002CC RID: 716
		private KeyboardState mKeyboardState;

		// Token: 0x040002CD RID: 717
		private MouseState mMouseState;

		// Token: 0x040002CE RID: 718
		private object mKBStateLock = new object();

		// Token: 0x040002CF RID: 719
		private Form mForm;

		// Token: 0x040002D0 RID: 720
		private bool mFocused;

		// Token: 0x040002D1 RID: 721
		private Stopwatch mTest;

		// Token: 0x040002D2 RID: 722
		private Thread mLogicThread;

		// Token: 0x040002D3 RID: 723
		private bool mLogicThreadActive;

		// Token: 0x040002D4 RID: 724
		private bool mRunLogic;

		// Token: 0x040002D5 RID: 725
		private bool mUpdateSuspended;

		// Token: 0x040002D6 RID: 726
		private bool mSuspendUpdate;

		// Token: 0x040002D7 RID: 727
		private Thread mLoaderThread;

		// Token: 0x040002D8 RID: 728
		private bool mLoaderThreadActive;

		// Token: 0x040002D9 RID: 729
		private bool mLoaderThreadBusy;

		// Token: 0x040002DA RID: 730
		private bool mRunLoader;

		// Token: 0x040002DB RID: 731
		private Queue<Action> mLoaderQueue;

		// Token: 0x040002DC RID: 732
		private DataChannel mLogicDataChannel;

		// Token: 0x040002DD RID: 733
		private ulong mVersionID;

		// Token: 0x040002DE RID: 734
		private Player[] mPlayers;

		// Token: 0x040002DF RID: 735
		private static Game mSingelton;

		// Token: 0x040002E0 RID: 736
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040002E1 RID: 737
		private bool mSuspendRendering;

		// Token: 0x040002E2 RID: 738
		private bool mRenderingSuspended;

		// Token: 0x040002E3 RID: 739
		private float mTimeSinceLastRender;

		// Token: 0x040002E4 RID: 740
		private object mDataChannelLock = new object();

		// Token: 0x040002E5 RID: 741
		private DataChannel mUsedByRender = DataChannel.None;

		// Token: 0x040002E6 RID: 742
		private DataChannel mNextToRender = DataChannel.None;

		// Token: 0x040002E7 RID: 743
		private Cursors mCursor;

		// Token: 0x040002E8 RID: 744
		private bool mCursorActive;

		// Token: 0x040002E9 RID: 745
		private bool mCursorVisible;

		// Token: 0x040002EA RID: 746
		private Cursor[,] mCursors;

		// Token: 0x040002EB RID: 747
		private Cursor mInvisibleCursor;

		// Token: 0x040002EC RID: 748
		private Action mUpdateCursor;

		// Token: 0x040002ED RID: 749
		private IServiceProvider mServiceProvider;
	}
}
