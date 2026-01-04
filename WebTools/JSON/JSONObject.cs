// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.JSON.JSONObject
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Magicka.WebTools.JSON;

public class JSONObject
{
  protected readonly string TIMESTAMP = "timestamp";
  protected List<JSONValue> collection;

  public JSONObject() => this.collection = new List<JSONValue>();

  public void Add(JSONValue js) => this.collection.Add(js);

  public new virtual string ToString()
  {
    string str = "{\n";
    foreach (JSONValue jsonValue in this.collection)
    {
      if (jsonValue == this.collection.Last<JSONValue>())
      {
        str = $"{str}\t{jsonValue.JSONLine()}";
        int startIndex = str.LastIndexOf(",");
        str = str.Remove(startIndex, 1);
      }
      else
        str = $"{str}\t{jsonValue.JSONLine()}";
    }
    return str + "},";
  }
}
