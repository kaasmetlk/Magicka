using System;
using System.Collections.Generic;
using GameSparks.Api.Responses;
using Magicka.CoreFramework;
using Magicka.Misc;
using Magicka.WebTools.GameSparks.PropertySets;

namespace Magicka.WebTools.GameSparks
{
	// Token: 0x02000031 RID: 49
	internal class GameSparksProperties : Singleton<GameSparksProperties>
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060001B9 RID: 441 RVA: 0x0000C923 File Offset: 0x0000AB23
		// (remove) Token: 0x060001BA RID: 442 RVA: 0x0000C93C File Offset: 0x0000AB3C
		public event Action OnPropertiesLoaded;

		// Token: 0x060001BB RID: 443 RVA: 0x0000C955 File Offset: 0x0000AB55
		public void ConfirmPropertySetReady(GameSparksPropertySet iSet)
		{
			this.mSetCount++;
			if (this.mSetCount == this.mPropertySets.Count && this.OnPropertiesLoaded != null)
			{
				this.OnPropertiesLoaded.Invoke();
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000C98C File Offset: 0x0000AB8C
		public void RetrievePropertySetsFromGameSparks()
		{
			foreach (GameSparksPropertySet gameSparksPropertySet in this.mPropertySets)
			{
				Singleton<GameSparksServices>.Instance.RequestPropertySet(gameSparksPropertySet.Name, new Action<GetPropertySetResponse>(gameSparksPropertySet.Callback));
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000CA10 File Offset: 0x0000AC10
		public T GetProperty<T>(string iSetKey, string iPropertyKey)
		{
			GameSparksPropertySet gameSparksPropertySet = this.mPropertySets.Find((GameSparksPropertySet x) => x.Name == iSetKey);
			return gameSparksPropertySet.GetProperty<T>(iPropertyKey);
		}

		// Token: 0x0400018A RID: 394
		private const Logger.Source LOGGER_SOURCE = Logger.Source.GameSparksProperties;

		// Token: 0x0400018B RID: 395
		private const string INVALID_SET_NAME = "{0} is not the name of any known Property Set";

		// Token: 0x0400018C RID: 396
		private List<GameSparksPropertySet> mPropertySets = new List<GameSparksPropertySet>
		{
			new AdsABTestPropertySet()
		};

		// Token: 0x0400018D RID: 397
		private int mSetCount;
	}
}
