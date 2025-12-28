using System;
using System.Xml;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000641 RID: 1601
	public class DLC_ContentUsedStatus
	{
		// Token: 0x17000B73 RID: 2931
		// (get) Token: 0x0600308F RID: 12431 RVA: 0x0018E2CC File Offset: 0x0018C4CC
		// (set) Token: 0x06003090 RID: 12432 RVA: 0x0018E2D4 File Offset: 0x0018C4D4
		public uint AppID
		{
			get
			{
				return this.mAppID;
			}
			set
			{
				this.mAppID = value;
				if (this.StoreAppID == 0U)
				{
					this.StoreAppID = value;
				}
			}
		}

		// Token: 0x06003091 RID: 12433 RVA: 0x0018E2EC File Offset: 0x0018C4EC
		public string ToXML()
		{
			string text = (this.StoreAppID != this.AppID && this.StoreAppID != 0U) ? string.Format(" StoreAppID=\"{0}\"", this.StoreAppID) : "";
			return string.Format("<DLC_ContentNewStatus Type=\"{0}\" Name=\"{1}\" IsUsed=\"{2}\" AppID=\"{3}\"{4}/>\n", new object[]
			{
				this.ItemType,
				this.Name,
				this.IsUsed ? "true" : "false",
				this.AppID,
				text
			});
		}

		// Token: 0x06003092 RID: 12434 RVA: 0x0018E37C File Offset: 0x0018C57C
		public static DLC_ContentUsedStatus FromXml(XmlNode n)
		{
			DLC_ContentUsedStatus dlc_ContentUsedStatus = new DLC_ContentUsedStatus();
			dlc_ContentUsedStatus.StoreAppID = 0U;
			XmlAttribute xmlAttribute = n.Attributes["AppID"];
			if (xmlAttribute != null)
			{
				dlc_ContentUsedStatus.AppID = uint.Parse(xmlAttribute.Value);
			}
			xmlAttribute = n.Attributes["Type"];
			if (xmlAttribute == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(xmlAttribute.Value))
			{
				return null;
			}
			dlc_ContentUsedStatus.ItemType = xmlAttribute.Value;
			xmlAttribute = n.Attributes["Name"];
			if (xmlAttribute == null)
			{
				return null;
			}
			dlc_ContentUsedStatus.Name = xmlAttribute.Value;
			xmlAttribute = n.Attributes["IsUsed"];
			if (xmlAttribute == null)
			{
				return null;
			}
			dlc_ContentUsedStatus.IsUsed = (string.Compare(xmlAttribute.Value, "true") == 0);
			xmlAttribute = n.Attributes["StoreAppID"];
			if (xmlAttribute != null)
			{
				dlc_ContentUsedStatus.StoreAppID = uint.Parse(xmlAttribute.Value);
			}
			if (dlc_ContentUsedStatus.StoreAppID == 0U)
			{
				dlc_ContentUsedStatus.StoreAppID = dlc_ContentUsedStatus.AppID;
			}
			return dlc_ContentUsedStatus;
		}

		// Token: 0x06003093 RID: 12435 RVA: 0x0018E477 File Offset: 0x0018C677
		public static int CompareByType(DLC_ContentUsedStatus x, DLC_ContentUsedStatus y)
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
				return x.ItemType.CompareTo(y.ItemType);
			}
		}

		// Token: 0x04003496 RID: 13462
		public string Name = "";

		// Token: 0x04003497 RID: 13463
		public string ItemType = "UNKNOWN";

		// Token: 0x04003498 RID: 13464
		public uint StoreAppID;

		// Token: 0x04003499 RID: 13465
		public bool IsUsed;

		// Token: 0x0400349A RID: 13466
		private uint mAppID;
	}
}
