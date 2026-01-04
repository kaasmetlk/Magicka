// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.ParadoxSettings
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.WebTools.Paradox;

public static class ParadoxSettings
{
  public const string ROOT_PATH = ".";
  public const string PARADOX_CACHE_FOLDER_NAME = "cache";
  public const string SHADOW_ACCOUNT_ID_TYPE = "generated";
  public const string MAGICKA_NEWSLETTER = "red_wizard";
  public const int MERGE_SHADOW_WAIT_DELAY = 2000;
  public static readonly string PARADOX_CACHE_PATH = Path.Combine(".", "cache");
}
