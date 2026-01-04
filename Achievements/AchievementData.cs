// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.AchievementData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Xml;

#nullable disable
namespace Magicka.Achievements;

public struct AchievementData : IComparable<AchievementData>
{
  public bool Achieved;
  public int Points;
  public string Code;
  public string Name;
  public string Desc;
  public DateTime DateAchieved;

  public static void ParseXml(XmlNode iNode, out AchievementData oData)
  {
    oData = new AchievementData();
    oData.DateAchieved = new DateTime(1970, 1, 1);
    for (int i = 0; i < iNode.Attributes.Count; ++i)
    {
      XmlAttribute attribute = iNode.Attributes[i];
      if (attribute.Name.Equals("code", StringComparison.OrdinalIgnoreCase))
        oData.Code = attribute.Value;
      else if (attribute.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
        oData.Name = attribute.Value;
      else if (attribute.Name.Equals("description", StringComparison.OrdinalIgnoreCase))
        oData.Desc = attribute.Value;
      else if (attribute.Name.Equals("points", StringComparison.OrdinalIgnoreCase))
        oData.Points = int.Parse(attribute.Value);
      else if (attribute.Name.Equals("achieved", StringComparison.OrdinalIgnoreCase))
        oData.Achieved = int.Parse(attribute.Value) != 0;
      else if (attribute.Name.Equals("date", StringComparison.OrdinalIgnoreCase))
      {
        oData.DateAchieved = new DateTime(1970, 1, 1);
        if (!string.IsNullOrEmpty(attribute.Value))
          oData.DateAchieved = oData.DateAchieved.AddSeconds((double) long.Parse(attribute.Value));
      }
    }
  }

  public int CompareTo(AchievementData other) => this.Code.CompareTo(other.Code);
}
