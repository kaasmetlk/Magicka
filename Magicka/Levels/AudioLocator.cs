using System;
using Magicka.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.Levels
{
	// Token: 0x02000291 RID: 657
	public struct AudioLocator
	{
		// Token: 0x06001369 RID: 4969 RVA: 0x000780A4 File Offset: 0x000762A4
		public AudioLocator(int iID, Banks iBank, int iCue, float iVolume, int iLocator, float iRadius, bool iApply3D)
		{
			this.mID = iID;
			this.mBank = iBank;
			this.mCueID = iCue;
			this.mCue = AudioManager.Instance.GetCue(iBank, iCue);
			this.mCue.SetVariable(AudioLocator.VOLUME_VAR_NAME, iVolume);
			this.mLocator = iLocator;
			this.mApply3D = iApply3D;
			this.mRadius = iRadius;
			this.mVolume = iVolume;
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x0007810C File Offset: 0x0007630C
		public void Play()
		{
			if (this.mCue.IsPaused)
			{
				this.mCue.Resume();
			}
			if (this.mCue.IsPlaying)
			{
				return;
			}
			if (this.mApply3D)
			{
				this.mCue.Apply3D(AudioManager.Instance.getListener(), AudioLocator.sEmitter);
			}
			if (!this.mCue.IsStopped && !this.mCue.IsStopping)
			{
				this.mCue.Play();
			}
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x00078186 File Offset: 0x00076386
		public void Pause()
		{
			if (this.mCue.IsPlaying)
			{
				this.mCue.Pause();
			}
		}

		// Token: 0x0600136C RID: 4972 RVA: 0x000781A0 File Offset: 0x000763A0
		public void Stop(AudioStopOptions iStopOptions)
		{
			if (!this.mCue.IsPlaying)
			{
				return;
			}
			this.mCue.Stop(iStopOptions);
		}

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x0600136D RID: 4973 RVA: 0x000781BC File Offset: 0x000763BC
		public int Locator
		{
			get
			{
				return this.mLocator;
			}
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x0600136E RID: 4974 RVA: 0x000781C4 File Offset: 0x000763C4
		public bool Apply3D
		{
			get
			{
				return this.mApply3D;
			}
		}

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x0600136F RID: 4975 RVA: 0x000781CC File Offset: 0x000763CC
		public Banks Bank
		{
			get
			{
				return this.mBank;
			}
		}

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06001370 RID: 4976 RVA: 0x000781D4 File Offset: 0x000763D4
		public int CueID
		{
			get
			{
				return this.mCueID;
			}
		}

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06001371 RID: 4977 RVA: 0x000781DC File Offset: 0x000763DC
		public Cue Cue
		{
			get
			{
				return this.mCue;
			}
		}

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06001372 RID: 4978 RVA: 0x000781E4 File Offset: 0x000763E4
		public float Radius
		{
			get
			{
				return this.mRadius;
			}
		}

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06001373 RID: 4979 RVA: 0x000781EC File Offset: 0x000763EC
		public float Volume
		{
			get
			{
				return this.mVolume;
			}
		}

		// Token: 0x06001374 RID: 4980 RVA: 0x000781F4 File Offset: 0x000763F4
		public void Update(GameScene iScene)
		{
			if (!this.mCue.IsPlaying)
			{
				return;
			}
			Matrix matrix;
			iScene.GetLocator(this.mLocator, out matrix);
			Vector3 translation = matrix.Translation;
			Vector3 groundPosition = iScene.PlayState.Camera.GroundPosition;
			float num;
			Vector3.Distance(ref translation, ref groundPosition, out num);
			this.mCue.SetVariable(AudioLocator.DISTANCE_NORMALIZED_VAR_NAME, num / this.mRadius);
			if (this.mApply3D)
			{
				AudioLocator.sEmitter.Position = matrix.Translation;
				AudioLocator.sEmitter.Forward = matrix.Forward;
				AudioLocator.sEmitter.Up = matrix.Up;
				this.mCue.Apply3D(iScene.PlayState.Camera.Listener, AudioLocator.sEmitter);
			}
			this.mCue.SetVariable(AudioLocator.DISTANCE_VAR_NAME, 0f);
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06001375 RID: 4981 RVA: 0x000782C9 File Offset: 0x000764C9
		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		// Token: 0x040014FE RID: 5374
		private static readonly string DISTANCE_NORMALIZED_VAR_NAME = "NormalizedDistance";

		// Token: 0x040014FF RID: 5375
		private static readonly string DISTANCE_VAR_NAME = "Distance";

		// Token: 0x04001500 RID: 5376
		private static readonly string VOLUME_VAR_NAME = "Volume";

		// Token: 0x04001501 RID: 5377
		private static AudioEmitter sEmitter = new AudioEmitter();

		// Token: 0x04001502 RID: 5378
		private int mID;

		// Token: 0x04001503 RID: 5379
		private int mLocator;

		// Token: 0x04001504 RID: 5380
		private bool mApply3D;

		// Token: 0x04001505 RID: 5381
		private Banks mBank;

		// Token: 0x04001506 RID: 5382
		private int mCueID;

		// Token: 0x04001507 RID: 5383
		private Cue mCue;

		// Token: 0x04001508 RID: 5384
		private float mRadius;

		// Token: 0x04001509 RID: 5385
		private float mVolume;
	}
}
