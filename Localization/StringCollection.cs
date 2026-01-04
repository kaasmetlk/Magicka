// Decompiled with JetBrains decompiler
// Type: Magicka.Localization.StringCollection
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Localization;

public class StringCollection : Dictionary<int, string>
{
  public void AddFile(string iFileName)
  {
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.Load(iFileName);
    XmlNode xmlNode = (XmlNode) null;
    foreach (XmlNode childNode in xmlDocument.ChildNodes)
    {
      if (childNode.Name.Equals("Workbook", StringComparison.OrdinalIgnoreCase))
      {
        xmlNode = childNode;
        break;
      }
    }
    if (xmlNode == null)
      throw new Exception($"No Workbook found in file \"{iFileName}\"!");
    foreach (XmlNode childNode1 in xmlNode.ChildNodes)
    {
      if (childNode1.Name.Equals("Worksheet", StringComparison.OrdinalIgnoreCase))
      {
        foreach (XmlNode childNode2 in childNode1.ChildNodes)
        {
          if (childNode2.Name.Equals("Table", StringComparison.OrdinalIgnoreCase))
          {
            foreach (XmlNode childNode3 in childNode2.ChildNodes)
            {
              if (childNode3.Name.Equals("Row", StringComparison.OrdinalIgnoreCase) && childNode3.ChildNodes.Count >= 2)
              {
                string innerText1 = childNode3.ChildNodes[0].InnerText;
                if (!string.IsNullOrEmpty(innerText1) && innerText1[0] == '#')
                {
                  int hashCodeCustom = innerText1.ToLowerInvariant().GetHashCodeCustom();
                  string innerText2 = childNode3.ChildNodes[1].InnerText;
                  if (!string.IsNullOrEmpty(innerText2))
                    this[hashCodeCustom] = innerText2;
                }
              }
            }
          }
        }
      }
    }
  }
}
