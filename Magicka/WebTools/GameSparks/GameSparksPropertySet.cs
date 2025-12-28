using System;
using System.Collections.Generic;
using GameSparks.Api.Responses;
using GameSparks.Core;
using Magicka.CoreFramework;
using Magicka.Misc;

namespace Magicka.WebTools.GameSparks
{
	// Token: 0x020000D5 RID: 213
	internal abstract class GameSparksPropertySet
	{
		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600066F RID: 1647
		public abstract string Name { get; }

		// Token: 0x06000670 RID: 1648 RVA: 0x0002645A File Offset: 0x0002465A
		public GameSparksPropertySet()
		{
			this.SetDefaults();
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x00026480 File Offset: 0x00024680
		public T GetProperty<T>(string iKey)
		{
			object propertyRaw = this.GetPropertyRaw(iKey);
			T result = default(T);
			if (propertyRaw is T)
			{
				result = (T)((object)propertyRaw);
			}
			return result;
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x000264B0 File Offset: 0x000246B0
		public void Callback(GetPropertySetResponse iResponse)
		{
			if (iResponse.HasErrors)
			{
				using (IEnumerator<KeyValuePair<string, object>> enumerator = iResponse.Errors.BaseData.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, object> keyValuePair = enumerator.Current;
						Logger.LogError(Logger.Source.GameSparksProperties, string.Format("GameSparks property retrieval failed. ERROR: {0}", keyValuePair.Key));
					}
					return;
				}
			}
			this.Parse(iResponse.PropertySet);
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x00026528 File Offset: 0x00024728
		public virtual void Parse(GSData iRawData)
		{
			this.mProperties.Clear();
			foreach (KeyValuePair<string, object> keyValuePair in iRawData.BaseData)
			{
				foreach (KeyValuePair<string, object> keyValuePair2 in ((GSData)keyValuePair.Value).BaseData)
				{
					if (this.mDefaultProperties.ContainsKey(keyValuePair2.Key))
					{
						if (this.IsTypeCorrect(this.mDefaultProperties[keyValuePair2.Key].mType, keyValuePair2.Value))
						{
							GameSparksPropertySet.Property value = new GameSparksPropertySet.Property(keyValuePair2.Key, this.mDefaultProperties[keyValuePair2.Key].mType, keyValuePair2.Value);
							this.mProperties.Add(keyValuePair2.Key, value);
						}
						else
						{
							GameSparksPropertySet.Property value = new GameSparksPropertySet.Property(keyValuePair2.Key, this.mDefaultProperties[keyValuePair2.Key].mType, this.mDefaultProperties[keyValuePair2.Key].mValue);
							this.mProperties.Add(keyValuePair2.Key, value);
						}
					}
					else
					{
						GameSparksPropertySet.Property value = new GameSparksPropertySet.Property(keyValuePair2.Key, GameSparksPropertySet.PropertyType.COUNT, keyValuePair2.Value);
						this.mProperties.Add(keyValuePair2.Key, value);
					}
				}
			}
			Singleton<GameSparksProperties>.Instance.ConfirmPropertySetReady(this);
		}

		// Token: 0x06000674 RID: 1652
		protected abstract void SetDefaults();

		// Token: 0x06000675 RID: 1653 RVA: 0x000266E8 File Offset: 0x000248E8
		private object GetPropertyRaw(string iKey)
		{
			object result = null;
			if (this.mProperties.ContainsKey(iKey))
			{
				result = this.mProperties[iKey].mValue;
			}
			else if (this.mDefaultProperties.ContainsKey(iKey))
			{
				result = this.mDefaultProperties[iKey].mValue;
			}
			return result;
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x0002673C File Offset: 0x0002493C
		private bool IsTypeCorrect(GameSparksPropertySet.PropertyType iType, object iValue)
		{
			bool result = false;
			switch (iType)
			{
			case GameSparksPropertySet.PropertyType.BOOL:
				result = (iValue is bool);
				break;
			case GameSparksPropertySet.PropertyType.INT:
				result = (iValue is int);
				break;
			case GameSparksPropertySet.PropertyType.FLOAT:
				result = (iValue is float);
				break;
			}
			return result;
		}

		// Token: 0x04000539 RID: 1337
		private const Logger.Source LOGGER_SOURCE = Logger.Source.GameSparksProperties;

		// Token: 0x0400053A RID: 1338
		private const string INVALID_PROPERTY = "{0} is not a valid GameSparks property.";

		// Token: 0x0400053B RID: 1339
		private const string INVALID_TYPE = "GameSparks property '{0}' is not of type '{1}'.";

		// Token: 0x0400053C RID: 1340
		private const string FAILED_GET = "GameSparks property retrieval failed. ERROR: {0}";

		// Token: 0x0400053D RID: 1341
		protected Dictionary<string, GameSparksPropertySet.Property> mProperties = new Dictionary<string, GameSparksPropertySet.Property>();

		// Token: 0x0400053E RID: 1342
		protected Dictionary<string, GameSparksPropertySet.Property> mDefaultProperties = new Dictionary<string, GameSparksPropertySet.Property>();

		// Token: 0x020000D6 RID: 214
		public enum PropertyType
		{
			// Token: 0x04000540 RID: 1344
			BOOL,
			// Token: 0x04000541 RID: 1345
			INT,
			// Token: 0x04000542 RID: 1346
			FLOAT,
			// Token: 0x04000543 RID: 1347
			ARRAY,
			// Token: 0x04000544 RID: 1348
			DICTIONARY,
			// Token: 0x04000545 RID: 1349
			COUNT
		}

		// Token: 0x020000D7 RID: 215
		public struct Property
		{
			// Token: 0x06000677 RID: 1655 RVA: 0x00026784 File Offset: 0x00024984
			public Property(string iName, GameSparksPropertySet.PropertyType iType, object iValue)
			{
				this.mName = iName;
				this.mType = iType;
				this.mValue = iValue;
			}

			// Token: 0x04000546 RID: 1350
			public readonly string mName;

			// Token: 0x04000547 RID: 1351
			public readonly GameSparksPropertySet.PropertyType mType;

			// Token: 0x04000548 RID: 1352
			public readonly object mValue;
		}
	}
}
