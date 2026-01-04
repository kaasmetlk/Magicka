// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.JSON.JSONDataCollection
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Magicka.WebTools.JSON;

public class JSONDataCollection : JSONValue
{
  private List<JSONDataEntry> values;
  private string timeStamp;

  public JSONDataCollection()
    : this((string) null)
  {
  }

  public JSONDataCollection(string time)
  {
    if (string.IsNullOrEmpty(time))
      this.timeStamp = DateTime.Now.ToUniversalTime().ToString();
    else
      this.timeStamp = time;
  }

  public void Add(JSONDataEntry val) => this.Add(val, true);

  public void Add(JSONDataEntry val, bool useThisTimeStamp)
  {
    if (this.values == null)
      this.values = new List<JSONDataEntry>();
    if (useThisTimeStamp)
      val.SetTimeStamp(this.timeStamp);
    this.values.Add(val);
  }

  public override string JSONLine() => this.OneLine();

  protected override string OneLine()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("\"data\": [\n");
    if (this.values != null && this.values.Count > 0)
    {
      for (int index = 0; index < this.values.Count; ++index)
      {
        if (this.values[index] != null)
        {
          bool flag = index == this.values.Count - 1;
          stringBuilder.Append("\t" + this.values[index].JSONLine(!flag, true));
        }
      }
    }
    stringBuilder.Append("\n]");
    return stringBuilder.ToString().Trim();
  }

  public override string ToString() => this.OneLine();
}
