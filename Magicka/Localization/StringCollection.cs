using System;
using System.Collections.Generic;
using System.Xml;

namespace Magicka.Localization
{
	// Token: 0x0200047D RID: 1149
	public class StringCollection : Dictionary<int, string>
	{
		// Token: 0x060022D3 RID: 8915 RVA: 0x000FB430 File Offset: 0x000F9630
		public void AddFile(string iFileName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(iFileName);
			XmlNode xmlNode = null;
			foreach (object obj in xmlDocument.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				if (xmlNode2.Name.Equals("Workbook", StringComparison.OrdinalIgnoreCase))
				{
					xmlNode = xmlNode2;
					break;
				}
			}
			if (xmlNode == null)
			{
				throw new Exception(string.Format("No Workbook found in file \"{0}\"!", iFileName));
			}
			foreach (object obj2 in xmlNode.ChildNodes)
			{
				XmlNode xmlNode3 = (XmlNode)obj2;
				if (xmlNode3.Name.Equals("Worksheet", StringComparison.OrdinalIgnoreCase))
				{
					foreach (object obj3 in xmlNode3.ChildNodes)
					{
						XmlNode xmlNode4 = (XmlNode)obj3;
						if (xmlNode4.Name.Equals("Table", StringComparison.OrdinalIgnoreCase))
						{
							foreach (object obj4 in xmlNode4.ChildNodes)
							{
								XmlNode xmlNode5 = (XmlNode)obj4;
								if (xmlNode5.Name.Equals("Row", StringComparison.OrdinalIgnoreCase) && xmlNode5.ChildNodes.Count >= 2)
								{
									string innerText = xmlNode5.ChildNodes[0].InnerText;
									if (!string.IsNullOrEmpty(innerText) && innerText[0] == '#')
									{
										int hashCodeCustom = innerText.ToLowerInvariant().GetHashCodeCustom();
										string innerText2 = xmlNode5.ChildNodes[1].InnerText;
										if (!string.IsNullOrEmpty(innerText2))
										{
											base[hashCodeCustom] = innerText2;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
