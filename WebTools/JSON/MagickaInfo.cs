// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.JSON.MagickaInfo
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using SteamWrapper;
using System;
using System.Text;

#nullable disable
namespace Magicka.WebTools.JSON;

public class MagickaInfo : JSONObject
{
  private readonly string GAME = "game";
  private readonly string GAMENAME = "magicka";
  private readonly string UNIVERSE = "universe";
  private readonly string PLATFORM = "steam";
  private readonly string USER_ID = "userid";
  private SteamID mUserID;
  private string timeStampStr;

  public string TimeStampString => this.timeStampStr;

  public MagickaInfo()
  {
    this.mUserID = SteamUser.GetSteamID();
    this.timeStampStr = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff");
    this.collection.Add((JSONValue) new JSONString(this.GAME, this.GAMENAME));
    this.collection.Add((JSONValue) new JSONString(this.UNIVERSE, this.PLATFORM));
    this.collection.Add((JSONValue) new JSONIntegral(this.USER_ID, this.mUserID.AsUInt64));
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("{\n");
    foreach (JSONValue jsonValue in this.collection)
      stringBuilder.AppendLine("\t" + jsonValue.JSONLine());
    stringBuilder.Append("\n}");
    return stringBuilder.ToString().Trim();
  }
}
