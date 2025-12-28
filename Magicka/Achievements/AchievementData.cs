using System;
using System.Xml;

namespace Magicka.Achievements
{
	// Token: 0x0200061A RID: 1562
	public struct AchievementData : IComparable<AchievementData>
	{
		// Token: 0x06002EC4 RID: 11972 RVA: 0x0017B334 File Offset: 0x00179534
		public static void ParseXml(XmlNode iNode, out AchievementData oData)
		{
			oData = default(AchievementData);
			oData.DateAchieved = new DateTime(1970, 1, 1);
			for (int i = 0; i < iNode.Attributes.Count; i++)
			{
				XmlAttribute xmlAttribute = iNode.Attributes[i];
				if (xmlAttribute.Name.Equals("code", StringComparison.OrdinalIgnoreCase))
				{
					oData.Code = xmlAttribute.Value;
				}
				else if (xmlAttribute.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
				{
					oData.Name = xmlAttribute.Value;
				}
				else if (xmlAttribute.Name.Equals("description", StringComparison.OrdinalIgnoreCase))
				{
					oData.Desc = xmlAttribute.Value;
				}
				else if (xmlAttribute.Name.Equals("points", StringComparison.OrdinalIgnoreCase))
				{
					oData.Points = int.Parse(xmlAttribute.Value);
				}
				else if (xmlAttribute.Name.Equals("achieved", StringComparison.OrdinalIgnoreCase))
				{
					oData.Achieved = (int.Parse(xmlAttribute.Value) != 0);
				}
				else if (xmlAttribute.Name.Equals("date", StringComparison.OrdinalIgnoreCase))
				{
					oData.DateAchieved = new DateTime(1970, 1, 1);
					if (!string.IsNullOrEmpty(xmlAttribute.Value))
					{
						oData.DateAchieved = oData.DateAchieved.AddSeconds((double)long.Parse(xmlAttribute.Value));
					}
				}
			}
		}

		// Token: 0x06002EC5 RID: 11973 RVA: 0x0017B490 File Offset: 0x00179690
		public int CompareTo(AchievementData other)
		{
			return this.Code.CompareTo(other.Code);
		}

		// Token: 0x040032DC RID: 13020
		public bool Achieved;

		// Token: 0x040032DD RID: 13021
		public int Points;

		// Token: 0x040032DE RID: 13022
		public string Code;

		// Token: 0x040032DF RID: 13023
		public string Name;

		// Token: 0x040032E0 RID: 13024
		public string Desc;

		// Token: 0x040032E1 RID: 13025
		public DateTime DateAchieved;
	}
}
