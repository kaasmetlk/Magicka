using System;
using System.Xml;

namespace Magicka.Achievements
{
	// Token: 0x0200061B RID: 1563
	public struct GameData
	{
		// Token: 0x06002EC6 RID: 11974 RVA: 0x0017B4A4 File Offset: 0x001796A4
		public static void ParseXml(XmlNode iNode, out GameData oData)
		{
			oData = default(GameData);
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
			}
		}

		// Token: 0x040032E2 RID: 13026
		public string Code;

		// Token: 0x040032E3 RID: 13027
		public string Name;

		// Token: 0x040032E4 RID: 13028
		public string Desc;
	}
}
