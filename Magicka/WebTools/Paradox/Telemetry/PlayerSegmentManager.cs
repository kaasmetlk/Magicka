using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Magicka.CoreFramework;
using Magicka.Misc;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x0200031E RID: 798
	public class PlayerSegmentManager : Singleton<PlayerSegmentManager>
	{
		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x0600187E RID: 6270 RVA: 0x000A27E8 File Offset: 0x000A09E8
		public PlayerSegment CurrentSegment
		{
			get
			{
				if (this.mData == null)
				{
					throw new Exception("Player segment data have not been initialized !");
				}
				return new PlayerSegment(new PlayerSegment.Section[]
				{
					this.mData.mHasPassedTutorial,
					this.mData.mHasWonAgainstAI,
					this.mData.mHasWonAgainstHuman,
					this.mData.mHasSpentRealMoney,
					this.mData.mHasSpentHardCurrency
				});
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x0600187F RID: 6271 RVA: 0x000A285D File Offset: 0x000A0A5D
		private string DataPath
		{
			get
			{
				return Path.Combine(ParadoxSettings.PARADOX_CACHE_PATH, "segment.dat");
			}
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x000A286E File Offset: 0x000A0A6E
		public PlayerSegmentManager()
		{
			this.Load();
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x000A287C File Offset: 0x000A0A7C
		public void NotifyTutorialEnded()
		{
			this.mData.mHasPassedTutorial = PlayerSegment.Section.True;
			this.Save();
		}

		// Token: 0x06001882 RID: 6274 RVA: 0x000A2890 File Offset: 0x000A0A90
		public void NotifyWonAgainstAI()
		{
			this.mData.mHasWonAgainstAI = PlayerSegment.Section.True;
			this.Save();
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x000A28A4 File Offset: 0x000A0AA4
		public void NotifyWonAgainstHuman()
		{
			this.mData.mHasWonAgainstHuman = PlayerSegment.Section.True;
			this.Save();
		}

		// Token: 0x06001884 RID: 6276 RVA: 0x000A28B8 File Offset: 0x000A0AB8
		public void NotifySpentRealMoney()
		{
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x000A28BA File Offset: 0x000A0ABA
		public void NotifySpentHardCurrency()
		{
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x000A28BC File Offset: 0x000A0ABC
		private void Load()
		{
			if (File.Exists(this.DataPath))
			{
				FileStream fileStream = File.Open(this.DataPath, FileMode.Open);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				this.mData = (PlayerSegmentManager.SegmentData)binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
				return;
			}
			this.mData = new PlayerSegmentManager.SegmentData();
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x000A2910 File Offset: 0x000A0B10
		private void Save()
		{
			if (this.mData != null)
			{
				ParadoxUtils.EnsureParadoxFolder();
				FileStream fileStream = File.Open(this.DataPath, FileMode.Create);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(fileStream, this.mData);
				fileStream.Close();
				return;
			}
			Logger.LogWarning(Logger.Source.PlayerSegmentManager, "There is no data to save.");
		}

		// Token: 0x04001A30 RID: 6704
		private const Logger.Source LOGGER_SOURCE = Logger.Source.PlayerSegmentManager;

		// Token: 0x04001A31 RID: 6705
		private const string EXCEPTION_DATA_NOT_INITIALIZED = "Player segment data have not been initialized !";

		// Token: 0x04001A32 RID: 6706
		private const string SAVE_FILENAME = "segment.dat";

		// Token: 0x04001A33 RID: 6707
		private PlayerSegmentManager.SegmentData mData;

		// Token: 0x0200031F RID: 799
		[Serializable]
		private class SegmentData
		{
			// Token: 0x04001A34 RID: 6708
			public PlayerSegment.Section mHasPassedTutorial;

			// Token: 0x04001A35 RID: 6709
			public PlayerSegment.Section mHasWonAgainstAI;

			// Token: 0x04001A36 RID: 6710
			public PlayerSegment.Section mHasWonAgainstHuman;

			// Token: 0x04001A37 RID: 6711
			public PlayerSegment.Section mHasSpentRealMoney = PlayerSegment.Section.NotApplicable;

			// Token: 0x04001A38 RID: 6712
			public PlayerSegment.Section mHasSpentHardCurrency = PlayerSegment.Section.NotApplicable;
		}
	}
}
