using System;
using Magicka.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A2 RID: 1186
	public struct PlaySoundEvent
	{
		// Token: 0x060023E9 RID: 9193 RVA: 0x00101FFD File Offset: 0x001001FD
		public PlaySoundEvent(Banks iBank, int iHash, bool iStopOnRemove, float iMagnitude)
		{
			this.mSoundBank = iBank;
			this.mSoundHash = iHash;
			this.mStopOnRemove = iStopOnRemove;
			this.mMagnitude = iMagnitude;
		}

		// Token: 0x060023EA RID: 9194 RVA: 0x0010201C File Offset: 0x0010021C
		public PlaySoundEvent(Banks iBank, int iHash, bool iStopOnRemove)
		{
			this.mSoundBank = iBank;
			this.mSoundHash = iHash;
			this.mStopOnRemove = iStopOnRemove;
			this.mMagnitude = 1f;
		}

		// Token: 0x060023EB RID: 9195 RVA: 0x0010203E File Offset: 0x0010023E
		public PlaySoundEvent(Banks iBank, int iHash)
		{
			this.mSoundBank = iBank;
			this.mSoundHash = iHash;
			this.mStopOnRemove = false;
			this.mMagnitude = 1f;
		}

		// Token: 0x060023EC RID: 9196 RVA: 0x00102060 File Offset: 0x00100260
		public PlaySoundEvent(Banks iBank, string iCue, bool iStopOnRemove, float iMagnitude)
		{
			this.mSoundBank = iBank;
			this.mSoundHash = iCue.GetHashCodeCustom();
			this.mStopOnRemove = iStopOnRemove;
			this.mMagnitude = iMagnitude;
		}

		// Token: 0x060023ED RID: 9197 RVA: 0x00102084 File Offset: 0x00100284
		public PlaySoundEvent(Banks iBank, string iCue, bool iStopOnRemove)
		{
			this.mSoundBank = iBank;
			this.mSoundHash = iCue.GetHashCodeCustom();
			this.mStopOnRemove = iStopOnRemove;
			this.mMagnitude = 1f;
		}

		// Token: 0x060023EE RID: 9198 RVA: 0x001020AB File Offset: 0x001002AB
		public PlaySoundEvent(Banks iBank, int iHash, float iMagnitude)
		{
			this.mSoundBank = iBank;
			this.mSoundHash = iHash;
			this.mStopOnRemove = false;
			this.mMagnitude = iMagnitude;
		}

		// Token: 0x060023EF RID: 9199 RVA: 0x001020C9 File Offset: 0x001002C9
		public PlaySoundEvent(Banks iBank, string iCue)
		{
			this.mSoundBank = iBank;
			this.mSoundHash = iCue.GetHashCodeCustom();
			this.mStopOnRemove = false;
			this.mMagnitude = 1f;
		}

		// Token: 0x060023F0 RID: 9200 RVA: 0x001020F0 File Offset: 0x001002F0
		public PlaySoundEvent(ContentReader iInput)
		{
			this.mSoundBank = (Banks)iInput.ReadInt32();
			string iString = iInput.ReadString();
			this.mSoundHash = iString.GetHashCodeCustom();
			this.mMagnitude = iInput.ReadSingle();
			this.mStopOnRemove = iInput.ReadBoolean();
		}

		// Token: 0x060023F1 RID: 9201 RVA: 0x00102138 File Offset: 0x00100338
		public void Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
		{
			AudioEmitter audioEmitter = iItem.AudioEmitter;
			if (iPosition != null)
			{
				audioEmitter = PlaySoundEvent.sEmitter;
				audioEmitter.Position = iPosition.Value;
			}
			Cue cue = AudioManager.Instance.PlayCue(this.mSoundBank, this.mSoundHash, audioEmitter);
			if (cue != null)
			{
				cue.SetVariable(PlaySoundEvent.VAR_NAME, this.mMagnitude);
				if (iItem is MissileEntity && this.mStopOnRemove)
				{
					(iItem as MissileEntity).AddCue(cue);
				}
			}
		}

		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x060023F2 RID: 9202 RVA: 0x001021AE File Offset: 0x001003AE
		public Banks SoundBank
		{
			get
			{
				return this.mSoundBank;
			}
		}

		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x060023F3 RID: 9203 RVA: 0x001021B6 File Offset: 0x001003B6
		public int SoundHash
		{
			get
			{
				return this.mSoundHash;
			}
		}

		// Token: 0x040026F7 RID: 9975
		private static AudioEmitter sEmitter = new AudioEmitter();

		// Token: 0x040026F8 RID: 9976
		private static readonly string VAR_NAME = "Magnitude";

		// Token: 0x040026F9 RID: 9977
		private Banks mSoundBank;

		// Token: 0x040026FA RID: 9978
		private int mSoundHash;

		// Token: 0x040026FB RID: 9979
		private float mMagnitude;

		// Token: 0x040026FC RID: 9980
		private bool mStopOnRemove;
	}
}
