// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.JSON.JSONDataEntry
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Magicka.WebTools.JSON;

public class JSONDataEntry : JSONValue
{
  private List<JSONValue> values;
  private JSONValue timeStamp;
  private readonly string timeStampKey = "timestamp";

  public void SetTimeStamp(string time)
  {
    this.timeStamp = (JSONValue) new JSONString(this.timeStampKey, time);
  }

  public void Add(JSONValue val)
  {
    if (this.values == null)
      this.values = new List<JSONValue>();
    this.values.Add(val);
  }

  protected override string OneLine()
  {
    if (this.timeStamp == null)
      this.SetTimeStamp(DateTime.Now.ToUniversalTime().ToString());
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("{");
    stringBuilder.Append(this.timeStamp.JSONLine(true, false));
    if (this.values != null && this.values.Count > 0)
    {
      for (int index = 0; index < this.values.Count; ++index)
      {
        bool addComma = index < this.values.Count - 1;
        stringBuilder.Append(this.values[index].JSONLine(addComma, false));
      }
    }
    stringBuilder.Append("}");
    return stringBuilder.ToString().Trim();
  }
}
