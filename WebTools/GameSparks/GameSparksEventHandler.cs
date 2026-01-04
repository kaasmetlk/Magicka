// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.GameSparks.GameSparksEventHandler
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using GameSparks.Core;
using Magicka.CoreFramework;
using Magicka.Misc;

#nullable disable
namespace Magicka.WebTools.GameSparks;

public class GameSparksEventHandler : Singleton<GameSparksEventHandler>
{
  private const string EVENT_TYPE_KEY = "EventType";
  private const string VARIANT_KEY = "Variant";
  private const string AD_CLICKED_TYPE = "AdClicked";
  private const string VARIANT_REQUEST_TYPE = "VariantRequest";

  public void HandleResponse(GSData iScriptData)
  {
    if (!iScriptData.ContainsKey("EventType"))
    {
      Logger.LogWarning($"GameSparksEventHandler was sent a response with no EventType Key, Script Data JSON follows: /n {iScriptData.JSON} /n");
    }
    else
    {
      string str = iScriptData.GetString("EventType");
      switch (str)
      {
        case "AdClicked":
          break;
        case "VariantRequest":
          Singleton<GameSparksAccount>.Instance.Variant = iScriptData.GetString("Variant");
          break;
        default:
          Logger.LogWarning($"GameSparksEventHandler was sent a response with an unknown EventType '{str}', Script Data JSON follows: /n {iScriptData.JSON} /n");
          break;
      }
    }
  }
}
