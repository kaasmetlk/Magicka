// Decompiled with JetBrains decompiler
// Type: Magicka.Audio.AudioManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace Magicka.Audio;

internal class AudioManager
{
  public static readonly string ROOMTYPE_VAR = "RoomType";
  public static readonly string REVERBMIX_VAR = "ReverbMix";
  public static readonly string MUSIC_FOCUS_VAR = "MusicFocus";
  private static AudioManager instance;
  private AudioEngine engine;
  private SoundBank[] soundBank;
  private WaveBank[] waveBank;
  private Dictionary<int, string> mDictSound;
  private int mSoundVolume = 8;
  private int mMusicVolume = 8;
  private float mThreatLevel;
  private bool mThreat;
  private RoomType mCurrentRoomType;
  private float mCurrentReverbMix;
  private float mTargetReverbMix;
  private AudioListener curListener;
  private Cue currentMusic;
  private Cue oldMusic;
  private Thread mInitThread;
  private Banks mInitializedBanks;
  private Vector3? mMusicFocus;
  private float mMusicFocusRadius;
  private float mMusicFocusDefaultValue = 1f;
  private List<Cue> mActiveCues = new List<Cue>(512 /*0x0200*/);
  private List<Cue> mGroupPause = new List<Cue>(512 /*0x0200*/);

  public AudioManager()
  {
    this.soundBank = new SoundBank[11];
    this.waveBank = new WaveBank[12];
  }

  public void StartInit(List<string> soundfiles)
  {
    this.mDictSound = new Dictionary<int, string>();
    this.mInitializedBanks = (Banks) 0;
    for (int index = 0; index < soundfiles.Count; ++index)
      this.mDictSound.Add(soundfiles[index].ToLowerInvariant().GetHashCodeCustom(), soundfiles[index]);
    do
    {
      try
      {
        this.engine = new AudioEngine("content\\Audio\\magicka.xgs");
      }
      catch (InvalidOperationException ex)
      {
        switch (MessageBox.Show("Unable to initialize a sound device.", "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Hand))
        {
          case DialogResult.Retry:
            break;
          case DialogResult.Ignore:
            return;
          default:
            Process.GetCurrentProcess().Kill();
            return;
        }
      }
    }
    while (this.engine == null);
    this.engine.GetCategory("Music").SetVolume((float) this.mMusicVolume * 0.1f);
    this.engine.GetCategory("Sfx").SetVolume((float) this.mSoundVolume * 0.1f);
    this.mInitThread = new Thread(new ThreadStart(this.Init));
    this.mInitThread.Name = "Init Audio";
    this.mInitThread.Start();
    LanguageManager.Instance.LanguageChanged += new Action(this.Instance_LanguageChanged);
  }

  private void Instance_LanguageChanged()
  {
    if (this.waveBank[this.waveBank.Length - 1] != null)
      this.waveBank[this.waveBank.Length - 1].Dispose();
    string str = $"content/Languages/{LanguageManager.Instance.CurrentLanguage}/loc.xwb";
    if (!File.Exists(str))
      str = "content/Languages/eng/loc.xwb";
    this.waveBank[this.waveBank.Length - 1] = new WaveBank(this.engine, str);
  }

  private void Init()
  {
    int index1 = 0;
    this.soundBank[index1] = new SoundBank(this.engine, "content/Audio/Sound Bank.xsb");
    this.waveBank[index1] = new WaveBank(this.engine, "content/Audio/Wave Bank.xwb");
    this.mInitializedBanks |= (Banks) (1 << index1);
    for (int index2 = 0; index2 < 2; ++index2)
    {
      ++index1;
      Banks banks = (Banks) (1 << index1);
      this.soundBank[index1] = new SoundBank(this.engine, $"content/Audio/{banks}.xsb");
      this.waveBank[index1] = new WaveBank(this.engine, $"content/Audio/{banks}.xwb", 0, (short) 64 /*0x40*/);
      this.mInitializedBanks |= (Banks) (1 << index1);
    }
    for (int index3 = 0; index3 < 7; ++index3)
    {
      ++index1;
      Banks banks = (Banks) (1 << index1);
      this.soundBank[index1] = new SoundBank(this.engine, $"content/Audio/{banks}.xsb");
      this.waveBank[index1] = new WaveBank(this.engine, $"content/Audio/{banks}.xwb");
      this.mInitializedBanks |= (Banks) (1 << index1);
    }
    for (int index4 = 0; index4 < 1; ++index4)
    {
      ++index1;
      Banks banks = (Banks) (1 << index1);
      this.soundBank[index1] = new SoundBank(this.engine, $"content/Audio/{banks}.xsb");
      this.waveBank[index1] = new WaveBank(this.engine, $"content/Audio/{banks}.xwb", 0, (short) 64 /*0x40*/);
      this.mInitializedBanks |= (Banks) (1 << index1);
    }
    this.Instance_LanguageChanged();
    this.mInitThread = (Thread) null;
  }

  public void SetRoomType(RoomType iRoomType)
  {
    this.mCurrentRoomType = iRoomType;
    this.engine.SetGlobalVariable(AudioManager.ROOMTYPE_VAR, (float) this.mCurrentRoomType);
  }

  public float TargetReverbMix
  {
    get => this.mTargetReverbMix;
    set => this.mTargetReverbMix = value;
  }

  public float CurrentReverbMix => this.mCurrentReverbMix;

  public static AudioManager Instance
  {
    get
    {
      if (AudioManager.instance == null)
        AudioManager.instance = new AudioManager();
      return AudioManager.instance;
    }
  }

  public void Dispose()
  {
    for (int index = 0; index < this.mActiveCues.Count; ++index)
    {
      if (this.mActiveCues[index].IsPlaying)
        this.mActiveCues[index].Stop(AudioStopOptions.Immediate);
    }
    lock (this.mActiveCues)
      this.mActiveCues.Clear();
    for (int index = 0; index < this.mGroupPause.Count; ++index)
    {
      if (this.mGroupPause[index].IsPlaying)
        this.mGroupPause[index].Stop(AudioStopOptions.Immediate);
    }
    this.mGroupPause.Clear();
    if (this.currentMusic == null || !this.currentMusic.IsPlaying)
      return;
    this.currentMusic.Stop(AudioStopOptions.Immediate);
  }

  public void Dispose(Banks iBank)
  {
    int index = MagickaMath.CountTrailingZeroBits((uint) iBank);
    if (this.soundBank[index] != null)
    {
      this.soundBank[index].Dispose();
      this.soundBank[index] = (SoundBank) null;
    }
    if (this.waveBank[index] == null)
      return;
    this.waveBank[index].Dispose();
    this.waveBank[index] = (WaveBank) null;
  }

  public Cue PlayCue(Banks iBank, int iSoundIndex)
  {
    Cue cue = (Cue) null;
    string name;
    if (this.mDictSound.TryGetValue(iSoundIndex, out name))
    {
      cue = this.soundBank[MagickaMath.CountTrailingZeroBits((uint) iBank)].GetCue(name);
      cue.Play();
      lock (this.mActiveCues)
        this.mActiveCues.Add(cue);
    }
    return cue;
  }

  public Cue PlayCue(Banks iBank, int iSoundIndex, AudioEmitter AE)
  {
    Cue cue = (Cue) null;
    string name;
    if (this.mDictSound.TryGetValue(iSoundIndex, out name))
    {
      cue = this.soundBank[MagickaMath.CountTrailingZeroBits((uint) iBank)].GetCue(name);
      cue.Apply3D(this.curListener, AE);
      cue.Play();
      lock (this.mActiveCues)
        this.mActiveCues.Add(cue);
    }
    return cue;
  }

  public Cue PlayCue<T>(Banks iBank, int iSoundIndex, T iVariables) where T : IAudioVariables
  {
    Cue iCue = (Cue) null;
    string name;
    if (this.mDictSound.TryGetValue(iSoundIndex, out name))
    {
      iCue = this.soundBank[MagickaMath.CountTrailingZeroBits((uint) iBank)].GetCue(name);
      iVariables.AssignToCue(iCue);
      iCue.Play();
      lock (this.mActiveCues)
        this.mActiveCues.Add(iCue);
    }
    return iCue;
  }

  public Cue PlayCue<T>(Banks iBank, int iSoundIndex, T iVariables, AudioEmitter iEmitter) where T : IAudioVariables
  {
    Cue iCue = (Cue) null;
    string name;
    if (this.mDictSound.TryGetValue(iSoundIndex, out name))
    {
      iCue = this.soundBank[MagickaMath.CountTrailingZeroBits((uint) iBank)].GetCue(name);
      iVariables.AssignToCue(iCue);
      iCue.Apply3D(this.curListener, iEmitter);
      iCue.Play();
      lock (this.mActiveCues)
        this.mActiveCues.Add(iCue);
    }
    return iCue;
  }

  public Cue GetCue(Banks iBank, int iSoundIndex)
  {
    Cue cue = (Cue) null;
    string name;
    if (this.mDictSound.TryGetValue(iSoundIndex, out name))
    {
      cue = this.soundBank[MagickaMath.CountTrailingZeroBits((uint) iBank)].GetCue(name);
      lock (this.mActiveCues)
        this.mActiveCues.Add(cue);
    }
    return cue;
  }

  public Cue GetCue<T>(Banks iBank, int iSoundIndex, T iVariable) where T : IAudioVariables
  {
    Cue iCue = (Cue) null;
    string name;
    if (this.mDictSound.TryGetValue(iSoundIndex, out name))
    {
      iCue = this.soundBank[MagickaMath.CountTrailingZeroBits((uint) iBank)].GetCue(name);
      iVariable.AssignToCue(iCue);
      lock (this.mActiveCues)
        this.mActiveCues.Add(iCue);
    }
    return iCue;
  }

  public void Update(float iDeltaTime)
  {
    if (this.engine == null)
      return;
    if (this.currentMusic != null && this.currentMusic.IsStopped)
      this.currentMusic = (Cue) null;
    lock (this.mActiveCues)
    {
      for (int index = 0; index < this.mActiveCues.Count; ++index)
      {
        if (this.mActiveCues[index].IsStopped)
        {
          this.mActiveCues.RemoveAt(index);
          --index;
        }
      }
    }
    if ((double) this.mTargetReverbMix < (double) this.mCurrentReverbMix)
      this.mCurrentReverbMix -= Math.Max(iDeltaTime * 50f, this.mTargetReverbMix - this.mCurrentReverbMix);
    else
      this.mCurrentReverbMix += Math.Min(iDeltaTime * 50f, this.mTargetReverbMix - this.mCurrentReverbMix);
    this.engine.SetGlobalVariable(AudioManager.REVERBMIX_VAR, this.mCurrentReverbMix);
    Vector3 valueOrDefault = this.mMusicFocus.GetValueOrDefault();
    if (this.mMusicFocus.HasValue)
    {
      float d = float.MaxValue;
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && players[index].Avatar != null && !players[index].Avatar.Dead)
        {
          Vector3 position = players[index].Avatar.Position;
          float result;
          Vector3.DistanceSquared(ref valueOrDefault, ref position, out result);
          if ((double) result < (double) d)
            d = result;
        }
      }
      float num = MathHelper.Clamp((float) Math.Sqrt((double) d) / this.mMusicFocusRadius, 0.0f, 1f);
      this.engine.SetGlobalVariable(AudioManager.MUSIC_FOCUS_VAR, num);
    }
    else
      this.engine.SetGlobalVariable(AudioManager.MUSIC_FOCUS_VAR, this.mMusicFocusDefaultValue);
    this.mThreatLevel = !this.mThreat ? Math.Max(this.mThreatLevel - iDeltaTime * 2f, 0.0f) : Math.Min(this.mThreatLevel + iDeltaTime * 5f, 10f);
    this.engine.SetGlobalVariable("MusicThreat", this.mThreatLevel);
    this.engine.Update();
  }

  public bool Threat
  {
    get => this.mThreat;
    set
    {
      if (this.mThreat != value && NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<ThreatMessage>(ref new ThreatMessage()
        {
          Threat = value
        });
      this.mThreat = value;
    }
  }

  public void SetListener(AudioListener AL) => this.curListener = AL;

  public AudioListener getListener() => this.curListener;

  public int VolumeSound() => this.mSoundVolume;

  public void VolumeSound(int value)
  {
    this.mSoundVolume = value;
    if (this.mSoundVolume > 10)
      this.mSoundVolume = 10;
    else if (this.mSoundVolume < 0)
      this.mSoundVolume = 0;
    if (this.engine == null)
      return;
    this.engine.GetCategory("Ambient").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("Default").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("Sfx").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("UI").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("VO").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("VOloc").SetVolume((float) this.mSoundVolume * 0.1f);
  }

  public void VolumeSoundIncrease()
  {
    if (++this.mSoundVolume > 10)
      this.mSoundVolume = 10;
    if (this.engine == null)
      return;
    this.engine.GetCategory("Ambient").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("Default").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("Sfx").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("UI").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("VO").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("VOloc").SetVolume((float) this.mSoundVolume * 0.1f);
  }

  public void VolumeSoundDecrease()
  {
    if (--this.mSoundVolume < 0)
      this.mSoundVolume = 0;
    if (this.engine == null)
      return;
    this.engine.GetCategory("Ambient").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("Default").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("Sfx").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("UI").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("VO").SetVolume((float) this.mSoundVolume * 0.1f);
    this.engine.GetCategory("VOloc").SetVolume((float) this.mSoundVolume * 0.1f);
  }

  public int VolumeMusic() => this.mMusicVolume;

  public void VolumeMusic(int value)
  {
    this.mMusicVolume = value;
    if (this.mMusicVolume > 10)
      this.mMusicVolume = 10;
    else if (this.mMusicVolume < 0)
      this.mMusicVolume = 0;
    if (this.engine == null)
      return;
    this.engine.GetCategory("Music").SetVolume((float) this.mMusicVolume * 0.1f);
  }

  public void VolumeMusicIncrease()
  {
    if (++this.mMusicVolume > 10)
      this.mMusicVolume = 10;
    this.engine.GetCategory("Music").SetVolume((float) this.mMusicVolume * 0.1f);
  }

  public void VolumeMusicDecrease()
  {
    if (--this.mMusicVolume < 0)
      this.mMusicVolume = 0;
    this.engine.GetCategory("Music").SetVolume((float) this.mMusicVolume * 0.1f);
  }

  public void StopAll(AudioStopOptions iStopOptions)
  {
    for (int index = 0; index < this.mActiveCues.Count; ++index)
    {
      Cue mActiveCue = this.mActiveCues[index];
      if (!mActiveCue.IsStopped)
        mActiveCue.Stop(iStopOptions);
    }
  }

  public void PauseAll()
  {
    this.mGroupPause.Clear();
    for (int index = 0; index < this.mActiveCues.Count; ++index)
    {
      Cue mActiveCue = this.mActiveCues[index];
      if (mActiveCue.IsPlaying)
      {
        mActiveCue.Pause();
        this.mGroupPause.Add(mActiveCue);
      }
    }
  }

  public void ResumeAll()
  {
    for (int index = 0; index < this.mGroupPause.Count; ++index)
    {
      Cue cue = this.mGroupPause[index];
      if (cue.IsPaused)
        cue.Resume();
    }
    this.mGroupPause.Clear();
  }

  public void PlayMusic(Banks iBank, int musicCueIndex, float? iFocusValue)
  {
    this.mMusicFocus = new Vector3?();
    this.mMusicFocusDefaultValue = iFocusValue ?? this.mMusicFocusDefaultValue;
    string name;
    if (!this.mDictSound.TryGetValue(musicCueIndex, out name))
      name = (string) null;
    if (this.currentMusic != null)
    {
      if (this.currentMusic.Name.Equals(name) && !this.currentMusic.IsStopping && !this.currentMusic.IsStopped)
        return;
      this.currentMusic.Stop(AudioStopOptions.AsAuthored);
      this.oldMusic = this.currentMusic;
    }
    if (string.IsNullOrEmpty(name))
      return;
    this.currentMusic = this.soundBank[MagickaMath.CountTrailingZeroBits((uint) iBank)].GetCue(name);
    this.currentMusic.Play();
  }

  public void StopMusic()
  {
    if (this.currentMusic != null)
      this.currentMusic.Stop(AudioStopOptions.AsAuthored);
    if (this.oldMusic == null)
      return;
    this.oldMusic.Stop(AudioStopOptions.AsAuthored);
  }

  public bool IsMusicPlaying => this.currentMusic != null;

  public float ThreatLevel => this.mThreatLevel;

  public Banks InitializedBanks => this.mInitializedBanks;

  internal void SetMusicFocus(Vector3 iPosition, float iRadius, float iDefaultValue)
  {
    this.mMusicFocus = new Vector3?(iPosition);
    this.mMusicFocusRadius = iRadius;
    this.mMusicFocusDefaultValue = iDefaultValue;
  }

  internal void ClearMusicFocus() => this.mMusicFocus = new Vector3?();
}
