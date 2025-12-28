using System;
using System.Collections.Generic;
using System.IO;
using Magicka.Graphics;

namespace Magicka.Storage
{
	// Token: 0x020003DD RID: 989
	public class SaveData
	{
		// Token: 0x06001E42 RID: 7746 RVA: 0x000D4818 File Offset: 0x000D2A18
		public SaveData()
		{
			this.mLevel = 0;
			this.mMaxAllowedLevel = 0;
			this.mPlayers = new Dictionary<string, PlayerSaveData>();
			for (int i = 0; i < this.mShownTips.Length; i++)
			{
				this.mShownTips[i].tipName = TutorialManager.TipsNames[i];
			}
			this.mUnlockedMagicks = 0UL;
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06001E43 RID: 7747 RVA: 0x000D4884 File Offset: 0x000D2A84
		// (set) Token: 0x06001E44 RID: 7748 RVA: 0x000D488C File Offset: 0x000D2A8C
		public MemoryStream Checkpoint
		{
			get
			{
				return this.mCheckPoint;
			}
			set
			{
				this.mCheckPoint = value;
			}
		}

		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x06001E45 RID: 7749 RVA: 0x000D4895 File Offset: 0x000D2A95
		// (set) Token: 0x06001E46 RID: 7750 RVA: 0x000D489D File Offset: 0x000D2A9D
		public byte Level
		{
			get
			{
				return this.mLevel;
			}
			set
			{
				this.mLevel = value;
				this.mMaxAllowedLevel = Math.Max(this.mMaxAllowedLevel, this.mLevel);
			}
		}

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x06001E47 RID: 7751 RVA: 0x000D48BD File Offset: 0x000D2ABD
		public byte MaxAllowedLevel
		{
			get
			{
				return this.mMaxAllowedLevel;
			}
		}

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x06001E48 RID: 7752 RVA: 0x000D48C5 File Offset: 0x000D2AC5
		// (set) Token: 0x06001E49 RID: 7753 RVA: 0x000D48CD File Offset: 0x000D2ACD
		public bool Looped
		{
			get
			{
				return this.mLooped;
			}
			set
			{
				this.mLooped = value;
			}
		}

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06001E4A RID: 7754 RVA: 0x000D48D6 File Offset: 0x000D2AD6
		// (set) Token: 0x06001E4B RID: 7755 RVA: 0x000D48DE File Offset: 0x000D2ADE
		public int TotalPlayTime
		{
			get
			{
				return this.mTotalPlayTime;
			}
			set
			{
				this.mTotalPlayTime = value;
			}
		}

		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x06001E4C RID: 7756 RVA: 0x000D48E7 File Offset: 0x000D2AE7
		// (set) Token: 0x06001E4D RID: 7757 RVA: 0x000D48EF File Offset: 0x000D2AEF
		public int CurrentPlayTime
		{
			get
			{
				return this.mCurrentPlayTime;
			}
			set
			{
				this.mCurrentPlayTime = value;
			}
		}

		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x06001E4E RID: 7758 RVA: 0x000D48F8 File Offset: 0x000D2AF8
		public Dictionary<string, PlayerSaveData> Players
		{
			get
			{
				return this.mPlayers;
			}
		}

		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x06001E4F RID: 7759 RVA: 0x000D4900 File Offset: 0x000D2B00
		public SaveData.tip[] ShownTips
		{
			get
			{
				return this.mShownTips;
			}
		}

		// Token: 0x06001E50 RID: 7760 RVA: 0x000D4908 File Offset: 0x000D2B08
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.mLevel);
			iWriter.Write(this.mMaxAllowedLevel);
			iWriter.Write(this.mLooped);
			iWriter.Write(this.mTotalPlayTime);
			iWriter.Write(this.mCurrentPlayTime);
			iWriter.Write(this.mPlayers.Count);
			foreach (KeyValuePair<string, PlayerSaveData> keyValuePair in this.mPlayers)
			{
				iWriter.Write(keyValuePair.Key);
				keyValuePair.Value.Write(iWriter);
			}
			iWriter.Write(this.mUnlockedMagicks);
			iWriter.Write(this.mShownTips.Length);
			for (int i = 0; i < this.mShownTips.Length; i++)
			{
				iWriter.Write(this.mShownTips[i].tipName);
				iWriter.Write(this.mShownTips[i].count);
			}
			if (this.mCheckPoint != null && this.mCheckPoint.Length > 0L)
			{
				iWriter.Write((int)this.mCheckPoint.Length);
				this.mCheckPoint.WriteTo(iWriter.BaseStream);
				return;
			}
			iWriter.Write(0);
		}

		// Token: 0x06001E51 RID: 7761 RVA: 0x000D4A58 File Offset: 0x000D2C58
		public static SaveData Read(ulong iVersion, BinaryReader iReader, SaveData iExistingInstance)
		{
			SaveData saveData = iExistingInstance;
			if (saveData == null)
			{
				saveData = new SaveData();
			}
			else
			{
				saveData.mPlayers.Clear();
			}
			if (iVersion >= 281492156645376UL)
			{
				SaveData.Read1410(iReader, saveData);
			}
			else
			{
				SaveData.Read1000(iReader, saveData);
			}
			return saveData;
		}

		// Token: 0x06001E52 RID: 7762 RVA: 0x000D4A9C File Offset: 0x000D2C9C
		private static void Read1410(BinaryReader iReader, SaveData iTarget)
		{
			iTarget.mLevel = iReader.ReadByte();
			iTarget.mMaxAllowedLevel = iReader.ReadByte();
			iTarget.mLooped = iReader.ReadBoolean();
			iTarget.mTotalPlayTime = iReader.ReadInt32();
			iTarget.mCurrentPlayTime = iReader.ReadInt32();
			int num = iReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string key = iReader.ReadString();
				PlayerSaveData value = PlayerSaveData.Read(iReader);
				iTarget.mPlayers.Add(key, value);
			}
			iTarget.mUnlockedMagicks = iReader.ReadUInt64();
			num = iReader.ReadInt32();
			iTarget.mShownTips = new SaveData.tip[11];
			for (int j = 0; j < num; j++)
			{
				SaveData.tip tip = default(SaveData.tip);
				tip.tipName = iReader.ReadString();
				tip.timeStamp = double.NegativeInfinity;
				tip.count = iReader.ReadInt32();
				iTarget.mShownTips[j] = tip;
			}
			int k = iReader.ReadInt32();
			if (k > 0)
			{
				iTarget.mCheckPoint = new MemoryStream(k);
				while (k > 0)
				{
					int num2 = iReader.Read(SaveData.sBuffer, 0, Math.Min(k, SaveData.sBuffer.Length));
					iTarget.mCheckPoint.Write(SaveData.sBuffer, 0, num2);
					k -= num2;
				}
				iTarget.mCheckPoint.Position = 0L;
				return;
			}
			iTarget.mCheckPoint = null;
		}

		// Token: 0x06001E53 RID: 7763 RVA: 0x000D4BF8 File Offset: 0x000D2DF8
		private static void Read1000(BinaryReader iReader, SaveData iTarget)
		{
			iTarget.mLevel = iReader.ReadByte();
			iTarget.mMaxAllowedLevel = iTarget.mLevel;
			iTarget.mLooped = iReader.ReadBoolean();
			iTarget.mTotalPlayTime = iReader.ReadInt32();
			iTarget.mCurrentPlayTime = iReader.ReadInt32();
			int num = iReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string key = iReader.ReadString();
				PlayerSaveData value = PlayerSaveData.Read(iReader);
				iTarget.mPlayers.Add(key, value);
			}
			iTarget.mUnlockedMagicks = iReader.ReadUInt64();
			num = iReader.ReadInt32();
			iTarget.mShownTips = new SaveData.tip[11];
			for (int j = 0; j < num; j++)
			{
				SaveData.tip tip = default(SaveData.tip);
				tip.tipName = iReader.ReadString();
				tip.timeStamp = double.NegativeInfinity;
				tip.count = iReader.ReadInt32();
				iTarget.mShownTips[j] = tip;
			}
		}

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x06001E54 RID: 7764 RVA: 0x000D4CE5 File Offset: 0x000D2EE5
		// (set) Token: 0x06001E55 RID: 7765 RVA: 0x000D4CED File Offset: 0x000D2EED
		public ulong UnlockedMagicks
		{
			get
			{
				return this.mUnlockedMagicks;
			}
			set
			{
				this.mUnlockedMagicks = value;
			}
		}

		// Token: 0x040020BC RID: 8380
		private static byte[] sBuffer = new byte[1024];

		// Token: 0x040020BD RID: 8381
		private byte mLevel;

		// Token: 0x040020BE RID: 8382
		private byte mMaxAllowedLevel;

		// Token: 0x040020BF RID: 8383
		private bool mLooped;

		// Token: 0x040020C0 RID: 8384
		private int mTotalPlayTime;

		// Token: 0x040020C1 RID: 8385
		private int mCurrentPlayTime;

		// Token: 0x040020C2 RID: 8386
		private Dictionary<string, PlayerSaveData> mPlayers;

		// Token: 0x040020C3 RID: 8387
		private SaveData.tip[] mShownTips = new SaveData.tip[11];

		// Token: 0x040020C4 RID: 8388
		private ulong mUnlockedMagicks;

		// Token: 0x040020C5 RID: 8389
		private MemoryStream mCheckPoint;

		// Token: 0x020003DE RID: 990
		public struct tip
		{
			// Token: 0x040020C6 RID: 8390
			public string tipName;

			// Token: 0x040020C7 RID: 8391
			public double timeStamp;

			// Token: 0x040020C8 RID: 8392
			public int count;
		}
	}
}
