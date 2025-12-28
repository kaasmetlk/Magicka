using System;
using System.Collections.Generic;
using System.IO;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Levels;
using Magicka.Levels.Versus;
using Magicka.Localization;

namespace Magicka
{
	// Token: 0x0200014B RID: 331
	internal class GlobalSettings
	{
		// Token: 0x170001EB RID: 491
		// (get) Token: 0x0600096C RID: 2412 RVA: 0x0003B060 File Offset: 0x00039260
		public static GlobalSettings Instance
		{
			get
			{
				if (GlobalSettings.mSingelton == null)
				{
					lock (GlobalSettings.mSingeltonLock)
					{
						if (GlobalSettings.mSingelton == null)
						{
							GlobalSettings.mSingelton = new GlobalSettings();
						}
					}
				}
				return GlobalSettings.mSingelton;
			}
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0003B0B4 File Offset: 0x000392B4
		private GlobalSettings()
		{
			this.Filter = new FilterData
			{
				GameType = GameType.Campaign,
				Scope = Scope.WAN,
				FreeSlots = 1,
				MaxLatency = 0,
				VACOnly = true,
				FilterPassword = false
			};
			this.VSSettings = new GlobalSettings.VersusSettings();
			this.BloodAndGore = SettingOptions.On;
			this.DamageNumbers = SettingOptions.On;
			this.HealthBars = SettingOptions.On;
			this.SpellWheel = SettingOptions.On;
			this.GameName = "Magicka Game";
			this.mXInputBindings = new XInputController.Binding[4][];
			this.mDInputBindings = new Dictionary<Guid, DirectInputController.Binding[]>();
			this.Resolution = new ResolutionData(Game.Instance.GraphicsDevice.DisplayMode);
			this.Fullscreen = false;
			this.BloomQuality = SettingOptions.Medium;
			this.ShadowQuality = SettingOptions.Medium;
			this.AmbientOcclusionQuality = SettingOptions.Medium;
			this.DecalLimit = SettingOptions.Medium;
			this.VolumeMusic = 7;
			this.VolumeSound = 7;
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x0600096E RID: 2414 RVA: 0x0003B19A File Offset: 0x0003939A
		// (set) Token: 0x0600096F RID: 2415 RVA: 0x0003B1A2 File Offset: 0x000393A2
		public ulong StartupLobby { get; set; }

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000970 RID: 2416 RVA: 0x0003B1AB File Offset: 0x000393AB
		// (set) Token: 0x06000971 RID: 2417 RVA: 0x0003B1B3 File Offset: 0x000393B3
		public string StartupPassword { get; set; }

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000972 RID: 2418 RVA: 0x0003B1BC File Offset: 0x000393BC
		// (set) Token: 0x06000973 RID: 2419 RVA: 0x0003B1C4 File Offset: 0x000393C4
		public ResolutionData Resolution
		{
			get
			{
				return this.mResolution;
			}
			set
			{
				this.mResolution = value;
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000974 RID: 2420 RVA: 0x0003B1CD File Offset: 0x000393CD
		// (set) Token: 0x06000975 RID: 2421 RVA: 0x0003B1D5 File Offset: 0x000393D5
		public bool VSync
		{
			get
			{
				return this.mVsync;
			}
			set
			{
				this.mVsync = value;
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000976 RID: 2422 RVA: 0x0003B1DE File Offset: 0x000393DE
		// (set) Token: 0x06000977 RID: 2423 RVA: 0x0003B1E6 File Offset: 0x000393E6
		public SettingOptions ShadowQuality { get; set; }

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000978 RID: 2424 RVA: 0x0003B1EF File Offset: 0x000393EF
		// (set) Token: 0x06000979 RID: 2425 RVA: 0x0003B1F7 File Offset: 0x000393F7
		public SettingOptions DecalLimit { get; set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x0600097A RID: 2426 RVA: 0x0003B200 File Offset: 0x00039400
		// (set) Token: 0x0600097B RID: 2427 RVA: 0x0003B208 File Offset: 0x00039408
		public SettingOptions Particles { get; set; }

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x0600097C RID: 2428 RVA: 0x0003B211 File Offset: 0x00039411
		// (set) Token: 0x0600097D RID: 2429 RVA: 0x0003B219 File Offset: 0x00039419
		public bool ParticleLights { get; set; }

		// Token: 0x0600097E RID: 2430 RVA: 0x0003B224 File Offset: 0x00039424
		public int ModShadowResolution(int iBaseResolution)
		{
			switch (this.ShadowQuality)
			{
			case SettingOptions.Low:
				return iBaseResolution / 2;
			case SettingOptions.High:
				return iBaseResolution * 2;
			}
			return iBaseResolution;
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x0600097F RID: 2431 RVA: 0x0003B257 File Offset: 0x00039457
		// (set) Token: 0x06000980 RID: 2432 RVA: 0x0003B25F File Offset: 0x0003945F
		public bool Fullscreen { get; set; }

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000981 RID: 2433 RVA: 0x0003B268 File Offset: 0x00039468
		// (set) Token: 0x06000982 RID: 2434 RVA: 0x0003B270 File Offset: 0x00039470
		public SettingOptions BloomQuality { get; set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000983 RID: 2435 RVA: 0x0003B279 File Offset: 0x00039479
		// (set) Token: 0x06000984 RID: 2436 RVA: 0x0003B281 File Offset: 0x00039481
		public SettingOptions AmbientOcclusionQuality { get; set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000985 RID: 2437 RVA: 0x0003B28A File Offset: 0x0003948A
		public XInputController.Binding[][] XInputBindings
		{
			get
			{
				return this.mXInputBindings;
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000986 RID: 2438 RVA: 0x0003B292 File Offset: 0x00039492
		public Dictionary<Guid, DirectInputController.Binding[]> DInputBindings
		{
			get
			{
				return this.mDInputBindings;
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000987 RID: 2439 RVA: 0x0003B29A File Offset: 0x0003949A
		// (set) Token: 0x06000988 RID: 2440 RVA: 0x0003B2A4 File Offset: 0x000394A4
		public int VolumeMusic
		{
			get
			{
				return this.mVolumeMusic;
			}
			set
			{
				int num = value;
				if (num > 10)
				{
					num = 10;
				}
				else if (num < 0)
				{
					num = 0;
				}
				this.mVolumeMusic = num;
				AudioManager.Instance.VolumeMusic(num);
			}
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000989 RID: 2441 RVA: 0x0003B2D5 File Offset: 0x000394D5
		// (set) Token: 0x0600098A RID: 2442 RVA: 0x0003B2E0 File Offset: 0x000394E0
		public int VolumeSound
		{
			get
			{
				return this.mVolumeSound;
			}
			set
			{
				int num = value;
				if (num > 10)
				{
					num = 10;
				}
				else if (num < 0)
				{
					num = 0;
				}
				this.mVolumeSound = num;
				AudioManager.Instance.VolumeSound(num);
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x0600098B RID: 2443 RVA: 0x0003B311 File Offset: 0x00039511
		// (set) Token: 0x0600098C RID: 2444 RVA: 0x0003B319 File Offset: 0x00039519
		public SettingOptions BloodAndGore { get; set; }

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x0600098D RID: 2445 RVA: 0x0003B322 File Offset: 0x00039522
		// (set) Token: 0x0600098E RID: 2446 RVA: 0x0003B32A File Offset: 0x0003952A
		public SettingOptions DamageNumbers { get; set; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x0600098F RID: 2447 RVA: 0x0003B333 File Offset: 0x00039533
		// (set) Token: 0x06000990 RID: 2448 RVA: 0x0003B33B File Offset: 0x0003953B
		public SettingOptions HealthBars { get; set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000991 RID: 2449 RVA: 0x0003B344 File Offset: 0x00039544
		// (set) Token: 0x06000992 RID: 2450 RVA: 0x0003B34C File Offset: 0x0003954C
		public SettingOptions SpellWheel { get; set; }

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x0003B355 File Offset: 0x00039555
		// (set) Token: 0x06000994 RID: 2452 RVA: 0x0003B35D File Offset: 0x0003955D
		public string GameName { get; set; }

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000995 RID: 2453 RVA: 0x0003B366 File Offset: 0x00039566
		// (set) Token: 0x06000996 RID: 2454 RVA: 0x0003B36E File Offset: 0x0003956E
		public FilterData Filter { get; set; }

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000997 RID: 2455 RVA: 0x0003B377 File Offset: 0x00039577
		// (set) Token: 0x06000998 RID: 2456 RVA: 0x0003B37F File Offset: 0x0003957F
		public string SteamGameLanguage { get; set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000999 RID: 2457 RVA: 0x0003B388 File Offset: 0x00039588
		// (set) Token: 0x0600099A RID: 2458 RVA: 0x0003B390 File Offset: 0x00039590
		public Language Language { get; set; }

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x0600099B RID: 2459 RVA: 0x0003B399 File Offset: 0x00039599
		// (set) Token: 0x0600099C RID: 2460 RVA: 0x0003B3A1 File Offset: 0x000395A1
		public GlobalSettings.VersusSettings VSSettings { get; private set; }

		// Token: 0x040008BF RID: 2239
		private static GlobalSettings mSingelton;

		// Token: 0x040008C0 RID: 2240
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040008C1 RID: 2241
		private ResolutionData mResolution;

		// Token: 0x040008C2 RID: 2242
		private int mVolumeMusic;

		// Token: 0x040008C3 RID: 2243
		private int mVolumeSound;

		// Token: 0x040008C4 RID: 2244
		private XInputController.Binding[][] mXInputBindings;

		// Token: 0x040008C5 RID: 2245
		private Dictionary<Guid, DirectInputController.Binding[]> mDInputBindings;

		// Token: 0x040008C6 RID: 2246
		private bool mVsync;

		// Token: 0x0200014C RID: 332
		internal class VersusSettings
		{
			// Token: 0x0600099E RID: 2462 RVA: 0x0003B3B8 File Offset: 0x000395B8
			public VersusSettings()
			{
				this.Mode = Rulesets.DeathMatch;
				this.DeathMatch.Ruleset = Rulesets.DeathMatch;
				this.Brawl.Ruleset = Rulesets.Brawl;
				this.Kreitor.Ruleset = Rulesets.Kreitor;
				this.KingOfTheHill.Ruleset = Rulesets.King;
				this.PyriteSnitch.Ruleset = Rulesets.Pyrite;
			}

			// Token: 0x0600099F RID: 2463 RVA: 0x0003B410 File Offset: 0x00039610
			public void Write(BinaryWriter iWriter)
			{
				iWriter.Write((byte)this.Mode);
				this.DeathMatch.Write(iWriter);
				this.Brawl.Write(iWriter);
				this.Kreitor.Write(iWriter);
				this.KingOfTheHill.Write(iWriter);
				this.PyriteSnitch.Write(iWriter);
			}

			// Token: 0x17000204 RID: 516
			// (get) Token: 0x060009A0 RID: 2464 RVA: 0x0003B465 File Offset: 0x00039665
			// (set) Token: 0x060009A1 RID: 2465 RVA: 0x0003B46D File Offset: 0x0003966D
			public Rulesets Mode { get; set; }

			// Token: 0x060009A2 RID: 2466 RVA: 0x0003B478 File Offset: 0x00039678
			internal void Read1370(BinaryReader iReader)
			{
				this.Mode = (Rulesets)iReader.ReadByte();
				this.DeathMatch.Read(iReader);
				this.Brawl.Read(iReader);
				this.Kreitor.Read(iReader);
				this.KingOfTheHill.Read(iReader);
				this.PyriteSnitch.Read(iReader);
				this.DeathMatch.Ruleset = Rulesets.DeathMatch;
				this.Brawl.Ruleset = Rulesets.Brawl;
				this.Kreitor.Ruleset = Rulesets.Kreitor;
				this.KingOfTheHill.Ruleset = Rulesets.King;
				this.PyriteSnitch.Ruleset = Rulesets.Pyrite;
			}

			// Token: 0x040008D9 RID: 2265
			public VersusRuleset.Settings.OptionsMessage DeathMatch;

			// Token: 0x040008DA RID: 2266
			public VersusRuleset.Settings.OptionsMessage Brawl;

			// Token: 0x040008DB RID: 2267
			public VersusRuleset.Settings.OptionsMessage Kreitor;

			// Token: 0x040008DC RID: 2268
			public VersusRuleset.Settings.OptionsMessage KingOfTheHill;

			// Token: 0x040008DD RID: 2269
			public VersusRuleset.Settings.OptionsMessage PyriteSnitch;
		}
	}
}
