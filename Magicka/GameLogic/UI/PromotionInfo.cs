using System;
using System.Xml;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000640 RID: 1600
	public sealed class PromotionInfo
	{
		// Token: 0x17000B6F RID: 2927
		// (get) Token: 0x06003084 RID: 12420 RVA: 0x0018E0E7 File Offset: 0x0018C2E7
		public bool NewButOwnd
		{
			get
			{
				return this.IsOwndByPlayer && this.IsNewToPlayer;
			}
		}

		// Token: 0x17000B70 RID: 2928
		// (get) Token: 0x06003085 RID: 12421 RVA: 0x0018E0F9 File Offset: 0x0018C2F9
		// (set) Token: 0x06003086 RID: 12422 RVA: 0x0018E101 File Offset: 0x0018C301
		public string NoneStoreURL
		{
			get
			{
				return this.nonStoreURL;
			}
			set
			{
				this.nonStoreURL = value;
			}
		}

		// Token: 0x17000B71 RID: 2929
		// (get) Token: 0x06003087 RID: 12423 RVA: 0x0018E10A File Offset: 0x0018C30A
		// (set) Token: 0x06003088 RID: 12424 RVA: 0x0018E112 File Offset: 0x0018C312
		public bool IsDynamicallyLoaded
		{
			get
			{
				return this.isDynamicallyLoaded;
			}
			set
			{
				this.isDynamicallyLoaded = value;
			}
		}

		// Token: 0x17000B72 RID: 2930
		// (get) Token: 0x06003089 RID: 12425 RVA: 0x0018E11B File Offset: 0x0018C31B
		// (set) Token: 0x0600308A RID: 12426 RVA: 0x0018E123 File Offset: 0x0018C323
		public int DynamicTextureID
		{
			get
			{
				return this.dynamicTextureID;
			}
			set
			{
				this.dynamicTextureID = value;
			}
		}

		// Token: 0x0600308B RID: 12427 RVA: 0x0018E12C File Offset: 0x0018C32C
		public static int CompareByDate(PromotionInfo x, PromotionInfo y)
		{
			if (x == null)
			{
				if (y == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (y == null)
				{
					return 1;
				}
				return y.ReleasedDate.CompareTo(x.ReleasedDate);
			}
		}

		// Token: 0x0600308C RID: 12428 RVA: 0x0018E150 File Offset: 0x0018C350
		public string ToXML()
		{
			if (this.isDynamicallyLoaded)
			{
				return "";
			}
			return string.Format("<DLC_SplashInfo AppID=\"{0}\" ReleaseDate=\"{1}\" Name=\"{2}\" IsNew=\"{3}\" IsOwnd=\"{4}\" />\n", new object[]
			{
				this.AppID,
				this.ReleasedDate.ToShortDateString(),
				this.Name,
				this.IsNewToPlayer ? "true" : "false",
				this.IsOwndByPlayer ? "true" : "false"
			});
		}

		// Token: 0x0600308D RID: 12429 RVA: 0x0018E1D0 File Offset: 0x0018C3D0
		public static PromotionInfo FromXml(XmlNode n)
		{
			XmlAttribute xmlAttribute = n.Attributes["AppID"];
			if (xmlAttribute == null)
			{
				return null;
			}
			PromotionInfo promotionInfo = new PromotionInfo();
			promotionInfo.AppID = uint.Parse(xmlAttribute.Value);
			xmlAttribute = n.Attributes["ReleaseDate"];
			if (xmlAttribute == null)
			{
				return null;
			}
			promotionInfo.ReleasedDate = DateTime.Parse(xmlAttribute.Value);
			xmlAttribute = n.Attributes["Name"];
			if (xmlAttribute == null)
			{
				return null;
			}
			promotionInfo.Name = xmlAttribute.Value;
			xmlAttribute = n.Attributes["IsNew"];
			if (xmlAttribute == null)
			{
				return null;
			}
			promotionInfo.IsNewToPlayer = (string.Compare(xmlAttribute.Value, "true") == 0);
			xmlAttribute = n.Attributes["IsOwnd"];
			if (xmlAttribute == null)
			{
				return null;
			}
			promotionInfo.IsOwndByPlayer = (string.Compare(xmlAttribute.Value, "true") == 0);
			return promotionInfo;
		}

		// Token: 0x0400348E RID: 13454
		public uint AppID;

		// Token: 0x0400348F RID: 13455
		public DateTime ReleasedDate;

		// Token: 0x04003490 RID: 13456
		public string Name;

		// Token: 0x04003491 RID: 13457
		public bool IsOwndByPlayer;

		// Token: 0x04003492 RID: 13458
		public bool IsNewToPlayer = true;

		// Token: 0x04003493 RID: 13459
		private bool isDynamicallyLoaded;

		// Token: 0x04003494 RID: 13460
		private string nonStoreURL = "";

		// Token: 0x04003495 RID: 13461
		private int dynamicTextureID;
	}
}
