using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.ParticleEffects;

namespace Magicka.Graphics
{
	// Token: 0x020003F4 RID: 1012
	public sealed class EffectManager
	{
		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06001EE2 RID: 7906 RVA: 0x000D7FE4 File Offset: 0x000D61E4
		public static EffectManager Instance
		{
			get
			{
				if (EffectManager.mSingelton == null)
				{
					lock (EffectManager.mSingeltonLock)
					{
						if (EffectManager.mSingelton == null)
						{
							EffectManager.mSingelton = new EffectManager();
						}
					}
				}
				return EffectManager.mSingelton;
			}
		}

		// Token: 0x06001EE3 RID: 7907 RVA: 0x000D8038 File Offset: 0x000D6238
		private EffectManager()
		{
			this.mSourceEffects = new Dictionary<int, VisualEffect>();
			this.mEffects = new VisualEffect[256];
			this.mUniqueIDs = new int[256];
			this.mFreeEffects = new IntHeap(256);
			for (int i = 0; i < 256; i++)
			{
				this.mFreeEffects.Push(i);
				this.mUniqueIDs[i] = int.MinValue;
			}
		}

		// Token: 0x06001EE4 RID: 7908 RVA: 0x000D80BC File Offset: 0x000D62BC
		private void ReadDirectory(DirectoryInfo iDir)
		{
			foreach (FileInfo fileInfo in iDir.GetFiles("*.xml"))
			{
				string iString = Path.GetFileNameWithoutExtension(fileInfo.FullName).ToLowerInvariant();
				int hashCodeCustom = iString.GetHashCodeCustom();
				VisualEffect value = VisualEffect.FromFile(fileInfo.FullName);
				this.mSourceEffects.Add(hashCodeCustom, value);
			}
			DirectoryInfo[] directories = iDir.GetDirectories();
			for (int j = 0; j < directories.Length; j++)
			{
				this.ReadDirectory(directories[j]);
			}
		}

		// Token: 0x06001EE5 RID: 7909 RVA: 0x000D8144 File Offset: 0x000D6344
		public void Initialize()
		{
			DirectoryInfo iDir = new DirectoryInfo("content/Effects");
			this.ReadDirectory(iDir);
		}

		// Token: 0x06001EE6 RID: 7910 RVA: 0x000D8164 File Offset: 0x000D6364
		public bool UpdatePositionDirection(ref VisualEffectReference iRef, ref Vector3 iPosition, ref Vector3 iDirection)
		{
			if (iRef.ID < 0 || this.mUniqueIDs[iRef.ID] != iRef.Hash)
			{
				iRef.Hash = -1;
				iRef.ID = -1;
				return false;
			}
			MagickaMath.MakeOrientationMatrix(ref iDirection, out this.mEffects[iRef.ID].Transform);
			this.mEffects[iRef.ID].Transform.Translation = iPosition;
			return true;
		}

		// Token: 0x06001EE7 RID: 7911 RVA: 0x000D81E0 File Offset: 0x000D63E0
		public bool UpdateOrientation(ref VisualEffectReference iRef, ref Matrix iTransform)
		{
			if (iRef.ID < 0 || this.mUniqueIDs[iRef.ID] != iRef.Hash)
			{
				iRef.Hash = 0;
				iRef.ID = -1;
				return false;
			}
			this.mEffects[iRef.ID].Transform = iTransform;
			return true;
		}

		// Token: 0x06001EE8 RID: 7912 RVA: 0x000D8238 File Offset: 0x000D6438
		public bool RestartEffect(ref VisualEffectReference iRef)
		{
			if (iRef.ID < 0 || this.mUniqueIDs[iRef.ID] != iRef.Hash)
			{
				iRef.Hash = 0;
				iRef.ID = -1;
				return false;
			}
			this.mEffects[iRef.ID].Reset();
			return true;
		}

		// Token: 0x06001EE9 RID: 7913 RVA: 0x000D828A File Offset: 0x000D648A
		public bool IsActive(ref VisualEffectReference iReference)
		{
			return iReference.ID >= 0 && this.mUniqueIDs[iReference.ID] == iReference.Hash && this.mEffects[iReference.ID].IsActive;
		}

		// Token: 0x06001EEA RID: 7914 RVA: 0x000D82C4 File Offset: 0x000D64C4
		public void Clear()
		{
			this.mFreeEffects.Clear();
			for (int i = 0; i < 256; i++)
			{
				this.mEffects[i] = default(VisualEffect);
				this.mUniqueIDs[i] = int.MinValue;
				this.mFreeEffects.Push(i);
			}
		}

		// Token: 0x06001EEB RID: 7915 RVA: 0x000D8317 File Offset: 0x000D6517
		public VisualEffect GetEffect(int iHash)
		{
			return this.mSourceEffects[iHash];
		}

		// Token: 0x06001EEC RID: 7916 RVA: 0x000D8325 File Offset: 0x000D6525
		public bool TryGetEffect(int iHash, out VisualEffect oEffect)
		{
			return this.mSourceEffects.TryGetValue(iHash, out oEffect);
		}

		// Token: 0x06001EED RID: 7917 RVA: 0x000D8334 File Offset: 0x000D6534
		public bool StartEffect(int iHash, ref Vector3 iPosition, ref Vector3 iDirection, out VisualEffectReference oRef)
		{
			Vector3 vector = default(Vector3);
			if (Math.Abs(iDirection.X) < 1E-45f && Math.Abs(iDirection.Z) < 1E-45f)
			{
				vector = Vector3.Forward;
			}
			else
			{
				vector = Vector3.Up;
			}
			Matrix matrix;
			Matrix.CreateWorld(ref iPosition, ref iDirection, ref vector, out matrix);
			return this.StartEffect(iHash, ref matrix, out oRef);
		}

		// Token: 0x06001EEE RID: 7918 RVA: 0x000D8394 File Offset: 0x000D6594
		public bool StartEffect(int iHash, ref Matrix iTransform, out VisualEffectReference oRef)
		{
			oRef.Hash = 0;
			oRef.ID = -1;
			if (this.mFreeEffects.IsEmpty)
			{
				return false;
			}
			VisualEffect visualEffect;
			if (this.mSourceEffects.TryGetValue(iHash, out visualEffect))
			{
				int num = this.mFreeEffects.Pop();
				visualEffect.Start(ref iTransform);
				this.mEffects[num] = visualEffect;
				this.mLastActive = Math.Max(num, this.mLastActive);
				oRef.ID = num;
				oRef.Hash = this.mRandomizer.Next(2147483646) + 1;
				this.mUniqueIDs[num] = oRef.Hash;
				return true;
			}
			return false;
		}

		// Token: 0x06001EEF RID: 7919 RVA: 0x000D8438 File Offset: 0x000D6638
		public void Stop(ref VisualEffectReference iRef)
		{
			lock (this)
			{
				if (iRef.ID >= 0 && this.mUniqueIDs[iRef.ID] == iRef.Hash)
				{
					this.mUniqueIDs[iRef.ID] = int.MinValue;
					this.mEffects[iRef.ID].Stop();
					this.mFreeEffects.Push(iRef.ID);
					iRef.Hash = 0;
					iRef.ID = -1;
				}
			}
		}

		// Token: 0x06001EF0 RID: 7920 RVA: 0x000D84D0 File Offset: 0x000D66D0
		public void Update(float iDeltaTime)
		{
			for (int i = this.mLastActive; i >= 0; i--)
			{
				if (this.mEffects[i].IsActive)
				{
					this.mEffects[i].Update(iDeltaTime);
					if (!this.mEffects[i].IsActive)
					{
						this.mUniqueIDs[i] = int.MinValue;
						this.mFreeEffects.Push(i);
					}
				}
			}
			while (this.mLastActive >= 0 && !this.mEffects[this.mLastActive].IsActive)
			{
				this.mLastActive--;
			}
		}

		// Token: 0x04002152 RID: 8530
		public const int MAXEFFECTS = 256;

		// Token: 0x04002153 RID: 8531
		private static EffectManager mSingelton;

		// Token: 0x04002154 RID: 8532
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04002155 RID: 8533
		private Random mRandomizer = new Random();

		// Token: 0x04002156 RID: 8534
		private Dictionary<int, VisualEffect> mSourceEffects;

		// Token: 0x04002157 RID: 8535
		private VisualEffect[] mEffects;

		// Token: 0x04002158 RID: 8536
		private int[] mUniqueIDs;

		// Token: 0x04002159 RID: 8537
		private IntHeap mFreeEffects;

		// Token: 0x0400215A RID: 8538
		private int mLastActive;
	}
}
