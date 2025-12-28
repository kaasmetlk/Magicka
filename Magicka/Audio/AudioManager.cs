using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Magicka.GameLogic;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.Audio
{
	// Token: 0x02000046 RID: 70
	internal class AudioManager
	{
		// Token: 0x06000296 RID: 662 RVA: 0x00010AD0 File Offset: 0x0000ECD0
		public AudioManager()
		{
			this.soundBank = new SoundBank[11];
			this.waveBank = new WaveBank[12];
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00010B38 File Offset: 0x0000ED38
		public void StartInit(List<string> soundfiles)
		{
			this.mDictSound = new Dictionary<int, string>();
			this.mInitializedBanks = (Banks)0;
			for (int i = 0; i < soundfiles.Count; i++)
			{
				this.mDictSound.Add(soundfiles[i].ToLowerInvariant().GetHashCodeCustom(), soundfiles[i]);
			}
			do
			{
				try
				{
					this.engine = new AudioEngine("content\\Audio\\magicka.xgs");
				}
				catch (InvalidOperationException)
				{
					DialogResult dialogResult = MessageBox.Show("Unable to initialize a sound device.", "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Hand);
					if (dialogResult == DialogResult.Ignore)
					{
						return;
					}
					if (dialogResult != DialogResult.Retry)
					{
						Process.GetCurrentProcess().Kill();
						return;
					}
				}
			}
			while (this.engine == null);
			this.engine.GetCategory("Music").SetVolume((float)this.mMusicVolume * 0.1f);
			this.engine.GetCategory("Sfx").SetVolume((float)this.mSoundVolume * 0.1f);
			this.mInitThread = new Thread(new ThreadStart(this.Init));
			this.mInitThread.Name = "Init Audio";
			this.mInitThread.Start();
			LanguageManager.Instance.LanguageChanged += new Action(this.Instance_LanguageChanged);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00010C7C File Offset: 0x0000EE7C
		private void Instance_LanguageChanged()
		{
			if (this.waveBank[this.waveBank.Length - 1] != null)
			{
				this.waveBank[this.waveBank.Length - 1].Dispose();
			}
			string text = string.Format("content/Languages/{0}/loc.xwb", LanguageManager.Instance.CurrentLanguage);
			if (!File.Exists(text))
			{
				text = "content/Languages/eng/loc.xwb";
			}
			this.waveBank[this.waveBank.Length - 1] = new WaveBank(this.engine, text);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00010CF8 File Offset: 0x0000EEF8
		private void Init()
		{
			int num = 0;
			this.soundBank[num] = new SoundBank(this.engine, "content/Audio/Sound Bank.xsb");
			this.waveBank[num] = new WaveBank(this.engine, "content/Audio/Wave Bank.xwb");
			this.mInitializedBanks |= (Banks)(1 << num);
			for (int i = 0; i < 2; i++)
			{
				num++;
				Banks banks = (Banks)(1 << num);
				this.soundBank[num] = new SoundBank(this.engine, string.Format("content/Audio/{0}.xsb", banks));
				this.waveBank[num] = new WaveBank(this.engine, string.Format("content/Audio/{0}.xwb", banks), 0, 64);
				this.mInitializedBanks |= (Banks)(1 << num);
			}
			for (int j = 0; j < 7; j++)
			{
				num++;
				Banks banks2 = (Banks)(1 << num);
				this.soundBank[num] = new SoundBank(this.engine, string.Format("content/Audio/{0}.xsb", banks2));
				this.waveBank[num] = new WaveBank(this.engine, string.Format("content/Audio/{0}.xwb", banks2));
				this.mInitializedBanks |= (Banks)(1 << num);
			}
			for (int k = 0; k < 1; k++)
			{
				num++;
				Banks banks3 = (Banks)(1 << num);
				this.soundBank[num] = new SoundBank(this.engine, string.Format("content/Audio/{0}.xsb", banks3));
				this.waveBank[num] = new WaveBank(this.engine, string.Format("content/Audio/{0}.xwb", banks3), 0, 64);
				this.mInitializedBanks |= (Banks)(1 << num);
			}
			this.Instance_LanguageChanged();
			this.mInitThread = null;
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00010EC2 File Offset: 0x0000F0C2
		public void SetRoomType(RoomType iRoomType)
		{
			this.mCurrentRoomType = iRoomType;
			this.engine.SetGlobalVariable(AudioManager.ROOMTYPE_VAR, (float)this.mCurrentRoomType);
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600029B RID: 667 RVA: 0x00010EE2 File Offset: 0x0000F0E2
		// (set) Token: 0x0600029C RID: 668 RVA: 0x00010EEA File Offset: 0x0000F0EA
		public float TargetReverbMix
		{
			get
			{
				return this.mTargetReverbMix;
			}
			set
			{
				this.mTargetReverbMix = value;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600029D RID: 669 RVA: 0x00010EF3 File Offset: 0x0000F0F3
		public float CurrentReverbMix
		{
			get
			{
				return this.mCurrentReverbMix;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600029E RID: 670 RVA: 0x00010EFB File Offset: 0x0000F0FB
		public static AudioManager Instance
		{
			get
			{
				if (AudioManager.instance == null)
				{
					AudioManager.instance = new AudioManager();
				}
				return AudioManager.instance;
			}
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00010F14 File Offset: 0x0000F114
		public void Dispose()
		{
			for (int i = 0; i < this.mActiveCues.Count; i++)
			{
				if (this.mActiveCues[i].IsPlaying)
				{
					this.mActiveCues[i].Stop(AudioStopOptions.Immediate);
				}
			}
			lock (this.mActiveCues)
			{
				this.mActiveCues.Clear();
			}
			for (int j = 0; j < this.mGroupPause.Count; j++)
			{
				if (this.mGroupPause[j].IsPlaying)
				{
					this.mGroupPause[j].Stop(AudioStopOptions.Immediate);
				}
			}
			this.mGroupPause.Clear();
			if (this.currentMusic != null && this.currentMusic.IsPlaying)
			{
				this.currentMusic.Stop(AudioStopOptions.Immediate);
			}
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00010FF4 File Offset: 0x0000F1F4
		public void Dispose(Banks iBank)
		{
			int num = MagickaMath.CountTrailingZeroBits((uint)iBank);
			if (this.soundBank[num] != null)
			{
				this.soundBank[num].Dispose();
				this.soundBank[num] = null;
			}
			if (this.waveBank[num] != null)
			{
				this.waveBank[num].Dispose();
				this.waveBank[num] = null;
			}
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00011048 File Offset: 0x0000F248
		public Cue PlayCue(Banks iBank, int iSoundIndex)
		{
			Cue cue = null;
			string name;
			if (this.mDictSound.TryGetValue(iSoundIndex, out name))
			{
				int num = MagickaMath.CountTrailingZeroBits((uint)iBank);
				cue = this.soundBank[num].GetCue(name);
				cue.Play();
				lock (this.mActiveCues)
				{
					this.mActiveCues.Add(cue);
				}
			}
			return cue;
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x000110B8 File Offset: 0x0000F2B8
		public Cue PlayCue(Banks iBank, int iSoundIndex, AudioEmitter AE)
		{
			Cue cue = null;
			string name;
			if (this.mDictSound.TryGetValue(iSoundIndex, out name))
			{
				int num = MagickaMath.CountTrailingZeroBits((uint)iBank);
				cue = this.soundBank[num].GetCue(name);
				cue.Apply3D(this.curListener, AE);
				cue.Play();
				lock (this.mActiveCues)
				{
					this.mActiveCues.Add(cue);
				}
			}
			return cue;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00011134 File Offset: 0x0000F334
		public Cue PlayCue<T>(Banks iBank, int iSoundIndex, T iVariables) where T : IAudioVariables
		{
			Cue cue = null;
			string name;
			if (this.mDictSound.TryGetValue(iSoundIndex, out name))
			{
				int num = MagickaMath.CountTrailingZeroBits((uint)iBank);
				cue = this.soundBank[num].GetCue(name);
				iVariables.AssignToCue(cue);
				cue.Play();
				lock (this.mActiveCues)
				{
					this.mActiveCues.Add(cue);
				}
			}
			return cue;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x000111B0 File Offset: 0x0000F3B0
		public Cue PlayCue<T>(Banks iBank, int iSoundIndex, T iVariables, AudioEmitter iEmitter) where T : IAudioVariables
		{
			Cue cue = null;
			string name;
			if (this.mDictSound.TryGetValue(iSoundIndex, out name))
			{
				int num = MagickaMath.CountTrailingZeroBits((uint)iBank);
				cue = this.soundBank[num].GetCue(name);
				iVariables.AssignToCue(cue);
				cue.Apply3D(this.curListener, iEmitter);
				cue.Play();
				lock (this.mActiveCues)
				{
					this.mActiveCues.Add(cue);
				}
			}
			return cue;
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0001123C File Offset: 0x0000F43C
		public Cue GetCue(Banks iBank, int iSoundIndex)
		{
			Cue cue = null;
			string name;
			if (this.mDictSound.TryGetValue(iSoundIndex, out name))
			{
				int num = MagickaMath.CountTrailingZeroBits((uint)iBank);
				cue = this.soundBank[num].GetCue(name);
				lock (this.mActiveCues)
				{
					this.mActiveCues.Add(cue);
				}
			}
			return cue;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x000112A4 File Offset: 0x0000F4A4
		public Cue GetCue<T>(Banks iBank, int iSoundIndex, T iVariable) where T : IAudioVariables
		{
			Cue cue = null;
			string name;
			if (this.mDictSound.TryGetValue(iSoundIndex, out name))
			{
				int num = MagickaMath.CountTrailingZeroBits((uint)iBank);
				cue = this.soundBank[num].GetCue(name);
				iVariable.AssignToCue(cue);
				lock (this.mActiveCues)
				{
					this.mActiveCues.Add(cue);
				}
			}
			return cue;
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0001131C File Offset: 0x0000F51C
		public void Update(float iDeltaTime)
		{
			if (this.engine == null)
			{
				return;
			}
			if (this.currentMusic != null && this.currentMusic.IsStopped)
			{
				this.currentMusic = null;
			}
			lock (this.mActiveCues)
			{
				for (int i = 0; i < this.mActiveCues.Count; i++)
				{
					if (this.mActiveCues[i].IsStopped)
					{
						this.mActiveCues.RemoveAt(i);
						i--;
					}
				}
			}
			if (this.mTargetReverbMix < this.mCurrentReverbMix)
			{
				this.mCurrentReverbMix -= Math.Max(iDeltaTime * 50f, this.mTargetReverbMix - this.mCurrentReverbMix);
			}
			else
			{
				this.mCurrentReverbMix += Math.Min(iDeltaTime * 50f, this.mTargetReverbMix - this.mCurrentReverbMix);
			}
			this.engine.SetGlobalVariable(AudioManager.REVERBMIX_VAR, this.mCurrentReverbMix);
			Vector3 valueOrDefault = this.mMusicFocus.GetValueOrDefault();
			if (this.mMusicFocus != null)
			{
				float num = float.MaxValue;
				Player[] players = Game.Instance.Players;
				for (int j = 0; j < players.Length; j++)
				{
					if (players[j].Playing && players[j].Avatar != null && !players[j].Avatar.Dead)
					{
						Vector3 position = players[j].Avatar.Position;
						float num2;
						Vector3.DistanceSquared(ref valueOrDefault, ref position, out num2);
						if (num2 < num)
						{
							num = num2;
						}
					}
				}
				num = (float)Math.Sqrt((double)num);
				float value = MathHelper.Clamp(num / this.mMusicFocusRadius, 0f, 1f);
				this.engine.SetGlobalVariable(AudioManager.MUSIC_FOCUS_VAR, value);
			}
			else
			{
				this.engine.SetGlobalVariable(AudioManager.MUSIC_FOCUS_VAR, this.mMusicFocusDefaultValue);
			}
			if (this.mThreat)
			{
				this.mThreatLevel = Math.Min(this.mThreatLevel + iDeltaTime * 5f, 10f);
			}
			else
			{
				this.mThreatLevel = Math.Max(this.mThreatLevel - iDeltaTime * 2f, 0f);
			}
			this.engine.SetGlobalVariable("MusicThreat", this.mThreatLevel);
			this.engine.Update();
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x0001155C File Offset: 0x0000F75C
		// (set) Token: 0x060002A9 RID: 681 RVA: 0x00011564 File Offset: 0x0000F764
		public bool Threat
		{
			get
			{
				return this.mThreat;
			}
			set
			{
				if (this.mThreat != value && NetworkManager.Instance.State == NetworkState.Server)
				{
					ThreatMessage threatMessage = default(ThreatMessage);
					threatMessage.Threat = value;
					NetworkManager.Instance.Interface.SendMessage<ThreatMessage>(ref threatMessage);
				}
				this.mThreat = value;
			}
		}

		// Token: 0x060002AA RID: 682 RVA: 0x000115AF File Offset: 0x0000F7AF
		public void SetListener(AudioListener AL)
		{
			this.curListener = AL;
		}

		// Token: 0x060002AB RID: 683 RVA: 0x000115B8 File Offset: 0x0000F7B8
		public AudioListener getListener()
		{
			return this.curListener;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x000115C0 File Offset: 0x0000F7C0
		public int VolumeSound()
		{
			return this.mSoundVolume;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x000115C8 File Offset: 0x0000F7C8
		public void VolumeSound(int value)
		{
			this.mSoundVolume = value;
			if (this.mSoundVolume > 10)
			{
				this.mSoundVolume = 10;
			}
			else if (this.mSoundVolume < 0)
			{
				this.mSoundVolume = 0;
			}
			if (this.engine != null)
			{
				this.engine.GetCategory("Ambient").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("Default").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("Sfx").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("UI").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("VO").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("VOloc").SetVolume((float)this.mSoundVolume * 0.1f);
			}
		}

		// Token: 0x060002AE RID: 686 RVA: 0x000116EC File Offset: 0x0000F8EC
		public void VolumeSoundIncrease()
		{
			if (++this.mSoundVolume > 10)
			{
				this.mSoundVolume = 10;
			}
			if (this.engine != null)
			{
				this.engine.GetCategory("Ambient").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("Default").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("Sfx").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("UI").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("VO").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("VOloc").SetVolume((float)this.mSoundVolume * 0.1f);
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00011804 File Offset: 0x0000FA04
		public void VolumeSoundDecrease()
		{
			if (--this.mSoundVolume < 0)
			{
				this.mSoundVolume = 0;
			}
			if (this.engine != null)
			{
				this.engine.GetCategory("Ambient").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("Default").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("Sfx").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("UI").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("VO").SetVolume((float)this.mSoundVolume * 0.1f);
				this.engine.GetCategory("VOloc").SetVolume((float)this.mSoundVolume * 0.1f);
			}
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00011918 File Offset: 0x0000FB18
		public int VolumeMusic()
		{
			return this.mMusicVolume;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00011920 File Offset: 0x0000FB20
		public void VolumeMusic(int value)
		{
			this.mMusicVolume = value;
			if (this.mMusicVolume > 10)
			{
				this.mMusicVolume = 10;
			}
			else if (this.mMusicVolume < 0)
			{
				this.mMusicVolume = 0;
			}
			if (this.engine != null)
			{
				this.engine.GetCategory("Music").SetVolume((float)this.mMusicVolume * 0.1f);
			}
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00011988 File Offset: 0x0000FB88
		public void VolumeMusicIncrease()
		{
			if (++this.mMusicVolume > 10)
			{
				this.mMusicVolume = 10;
			}
			this.engine.GetCategory("Music").SetVolume((float)this.mMusicVolume * 0.1f);
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x000119D8 File Offset: 0x0000FBD8
		public void VolumeMusicDecrease()
		{
			if (--this.mMusicVolume < 0)
			{
				this.mMusicVolume = 0;
			}
			this.engine.GetCategory("Music").SetVolume((float)this.mMusicVolume * 0.1f);
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00011A28 File Offset: 0x0000FC28
		public void StopAll(AudioStopOptions iStopOptions)
		{
			for (int i = 0; i < this.mActiveCues.Count; i++)
			{
				Cue cue = this.mActiveCues[i];
				if (!cue.IsStopped)
				{
					cue.Stop(iStopOptions);
				}
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00011A68 File Offset: 0x0000FC68
		public void PauseAll()
		{
			this.mGroupPause.Clear();
			for (int i = 0; i < this.mActiveCues.Count; i++)
			{
				Cue cue = this.mActiveCues[i];
				if (cue.IsPlaying)
				{
					cue.Pause();
					this.mGroupPause.Add(cue);
				}
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00011AC0 File Offset: 0x0000FCC0
		public void ResumeAll()
		{
			for (int i = 0; i < this.mGroupPause.Count; i++)
			{
				Cue cue = this.mGroupPause[i];
				if (cue.IsPaused)
				{
					cue.Resume();
				}
			}
			this.mGroupPause.Clear();
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00011B0C File Offset: 0x0000FD0C
		public void PlayMusic(Banks iBank, int musicCueIndex, float? iFocusValue)
		{
			this.mMusicFocus = null;
			this.mMusicFocusDefaultValue = iFocusValue.GetValueOrDefault(this.mMusicFocusDefaultValue);
			string text;
			if (!this.mDictSound.TryGetValue(musicCueIndex, out text))
			{
				text = null;
			}
			if (this.currentMusic != null)
			{
				if (this.currentMusic.Name.Equals(text) && !this.currentMusic.IsStopping && !this.currentMusic.IsStopped)
				{
					return;
				}
				this.currentMusic.Stop(AudioStopOptions.AsAuthored);
				this.oldMusic = this.currentMusic;
			}
			if (!string.IsNullOrEmpty(text))
			{
				int num = MagickaMath.CountTrailingZeroBits((uint)iBank);
				this.currentMusic = this.soundBank[num].GetCue(text);
				this.currentMusic.Play();
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00011BC6 File Offset: 0x0000FDC6
		public void StopMusic()
		{
			if (this.currentMusic != null)
			{
				this.currentMusic.Stop(AudioStopOptions.AsAuthored);
			}
			if (this.oldMusic != null)
			{
				this.oldMusic.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060002B9 RID: 697 RVA: 0x00011BF0 File Offset: 0x0000FDF0
		public bool IsMusicPlaying
		{
			get
			{
				return this.currentMusic != null;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060002BA RID: 698 RVA: 0x00011BFE File Offset: 0x0000FDFE
		public float ThreatLevel
		{
			get
			{
				return this.mThreatLevel;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060002BB RID: 699 RVA: 0x00011C06 File Offset: 0x0000FE06
		public Banks InitializedBanks
		{
			get
			{
				return this.mInitializedBanks;
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x00011C0E File Offset: 0x0000FE0E
		internal void SetMusicFocus(Vector3 iPosition, float iRadius, float iDefaultValue)
		{
			this.mMusicFocus = new Vector3?(iPosition);
			this.mMusicFocusRadius = iRadius;
			this.mMusicFocusDefaultValue = iDefaultValue;
		}

		// Token: 0x060002BD RID: 701 RVA: 0x00011C2A File Offset: 0x0000FE2A
		internal void ClearMusicFocus()
		{
			this.mMusicFocus = null;
		}

		// Token: 0x04000235 RID: 565
		public static readonly string ROOMTYPE_VAR = "RoomType";

		// Token: 0x04000236 RID: 566
		public static readonly string REVERBMIX_VAR = "ReverbMix";

		// Token: 0x04000237 RID: 567
		public static readonly string MUSIC_FOCUS_VAR = "MusicFocus";

		// Token: 0x04000238 RID: 568
		private static AudioManager instance;

		// Token: 0x04000239 RID: 569
		private AudioEngine engine;

		// Token: 0x0400023A RID: 570
		private SoundBank[] soundBank;

		// Token: 0x0400023B RID: 571
		private WaveBank[] waveBank;

		// Token: 0x0400023C RID: 572
		private Dictionary<int, string> mDictSound;

		// Token: 0x0400023D RID: 573
		private int mSoundVolume = 8;

		// Token: 0x0400023E RID: 574
		private int mMusicVolume = 8;

		// Token: 0x0400023F RID: 575
		private float mThreatLevel;

		// Token: 0x04000240 RID: 576
		private bool mThreat;

		// Token: 0x04000241 RID: 577
		private RoomType mCurrentRoomType;

		// Token: 0x04000242 RID: 578
		private float mCurrentReverbMix;

		// Token: 0x04000243 RID: 579
		private float mTargetReverbMix;

		// Token: 0x04000244 RID: 580
		private AudioListener curListener;

		// Token: 0x04000245 RID: 581
		private Cue currentMusic;

		// Token: 0x04000246 RID: 582
		private Cue oldMusic;

		// Token: 0x04000247 RID: 583
		private Thread mInitThread;

		// Token: 0x04000248 RID: 584
		private Banks mInitializedBanks;

		// Token: 0x04000249 RID: 585
		private Vector3? mMusicFocus;

		// Token: 0x0400024A RID: 586
		private float mMusicFocusRadius;

		// Token: 0x0400024B RID: 587
		private float mMusicFocusDefaultValue = 1f;

		// Token: 0x0400024C RID: 588
		private List<Cue> mActiveCues = new List<Cue>(512);

		// Token: 0x0400024D RID: 589
		private List<Cue> mGroupPause = new List<Cue>(512);
	}
}
