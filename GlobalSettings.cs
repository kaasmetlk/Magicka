// Decompiled with JetBrains decompiler
// Type: Magicka.GlobalSettings
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Levels;
using Magicka.Levels.Versus;
using Magicka.Localization;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka;

internal class GlobalSettings
{
  private static GlobalSettings mSingelton;
  private static volatile object mSingeltonLock = new object();
  private ResolutionData mResolution;
  private int mVolumeMusic;
  private int mVolumeSound;
  private XInputController.Binding[][] mXInputBindings;
  private Dictionary<Guid, DirectInputController.Binding[]> mDInputBindings;
  private bool mVsync;

  public static GlobalSettings Instance
  {
    get
    {
      if (GlobalSettings.mSingelton == null)
      {
        lock (GlobalSettings.mSingeltonLock)
        {
          if (GlobalSettings.mSingelton == null)
            GlobalSettings.mSingelton = new GlobalSettings();
        }
      }
      return GlobalSettings.mSingelton;
    }
  }

  private GlobalSettings()
  {
    this.Filter = new FilterData()
    {
      GameType = GameType.Campaign,
      Scope = Scope.WAN,
      FreeSlots = (byte) 1,
      MaxLatency = (short) 0,
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

  public ulong StartupLobby { get; set; }

  public string StartupPassword { get; set; }

  public ResolutionData Resolution
  {
    get => this.mResolution;
    set => this.mResolution = value;
  }

  public bool VSync
  {
    get => this.mVsync;
    set => this.mVsync = value;
  }

  public SettingOptions ShadowQuality { get; set; }

  public SettingOptions DecalLimit { get; set; }

  public SettingOptions Particles { get; set; }

  public bool ParticleLights { get; set; }

  public int ModShadowResolution(int iBaseResolution)
  {
    switch (this.ShadowQuality)
    {
      case SettingOptions.Low:
        return iBaseResolution / 2;
      case SettingOptions.High:
        return iBaseResolution * 2;
      default:
        return iBaseResolution;
    }
  }

  public bool Fullscreen { get; set; }

  public SettingOptions BloomQuality { get; set; }

  public SettingOptions AmbientOcclusionQuality { get; set; }

  public XInputController.Binding[][] XInputBindings => this.mXInputBindings;

  public Dictionary<Guid, DirectInputController.Binding[]> DInputBindings => this.mDInputBindings;

  public int VolumeMusic
  {
    get => this.mVolumeMusic;
    set
    {
      int num = value;
      if (num > 10)
        num = 10;
      else if (num < 0)
        num = 0;
      this.mVolumeMusic = num;
      AudioManager.Instance.VolumeMusic(num);
    }
  }

  public int VolumeSound
  {
    get => this.mVolumeSound;
    set
    {
      int num = value;
      if (num > 10)
        num = 10;
      else if (num < 0)
        num = 0;
      this.mVolumeSound = num;
      AudioManager.Instance.VolumeSound(num);
    }
  }

  public SettingOptions BloodAndGore { get; set; }

  public SettingOptions DamageNumbers { get; set; }

  public SettingOptions HealthBars { get; set; }

  public SettingOptions SpellWheel { get; set; }

  public string GameName { get; set; }

  public FilterData Filter { get; set; }

  public string SteamGameLanguage { get; set; }

  public Language Language { get; set; }

  public GlobalSettings.VersusSettings VSSettings { get; private set; }

  internal class VersusSettings
  {
    public VersusRuleset.Settings.OptionsMessage DeathMatch;
    public VersusRuleset.Settings.OptionsMessage Brawl;
    public VersusRuleset.Settings.OptionsMessage Kreitor;
    public VersusRuleset.Settings.OptionsMessage KingOfTheHill;
    public VersusRuleset.Settings.OptionsMessage PyriteSnitch;

    public VersusSettings()
    {
      this.Mode = Rulesets.DeathMatch;
      this.DeathMatch.Ruleset = Rulesets.DeathMatch;
      this.Brawl.Ruleset = Rulesets.Brawl;
      this.Kreitor.Ruleset = Rulesets.Kreitor;
      this.KingOfTheHill.Ruleset = Rulesets.King;
      this.PyriteSnitch.Ruleset = Rulesets.Pyrite;
    }

    public void Write(BinaryWriter iWriter)
    {
      iWriter.Write((byte) this.Mode);
      this.DeathMatch.Write(iWriter);
      this.Brawl.Write(iWriter);
      this.Kreitor.Write(iWriter);
      this.KingOfTheHill.Write(iWriter);
      this.PyriteSnitch.Write(iWriter);
    }

    public Rulesets Mode { get; set; }

    internal void Read1370(BinaryReader iReader)
    {
      this.Mode = (Rulesets) iReader.ReadByte();
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
  }
}
