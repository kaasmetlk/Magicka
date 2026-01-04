// Decompiled with JetBrains decompiler
// Type: Magicka.CoreFramework.Logger
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Collections.Generic;

#nullable disable
namespace Magicka.CoreFramework;

public static class Logger
{
  private const string VERBOSE_PREFIXED_FORMAT = "[{0}] {1}";
  private const string DEBUG_FORMAT = "DEBUG : {0}";
  private const string DEBUG_PREFIXED_FORMAT = "[{0}] DEBUG : {1}";
  private const string WARNING_FORMAT = "WARNING : {0}";
  private const string WARNING_PREFIXED_FORMAT = "[{0}] WARNING : {1}";
  private const string ERROR_FORMAT = "ERROR : {0}";
  private const string ERROR_PREFIXED_FORMAT = "[{0}] ERROR : {1}";
  private const string CRITICAL_FORMAT = "CRITICAL : {0}";
  private const string CRITICAL_PREFIXED_FORMAT = "[{0}] CRITICAL : {1}";
  private const string FILE_AND_LINE_FORMAT = "In File {0} ({1})";
  private static readonly List<Logger.Source> sWhiteList = new List<Logger.Source>()
  {
    Logger.Source.Global
  };

  public static void LogVerbose(string iMessage)
  {
    Logger.LogVerbose(Logger.Source.Global, iMessage);
  }

  public static void LogVerbose(string iMessage, bool iFileAndLine)
  {
    Logger.LogVerbose(Logger.Source.Global, iMessage, iFileAndLine);
  }

  public static void LogVerbose(Logger.Source iSource, string iMessage)
  {
    Logger.sWhiteList.Contains(iSource);
  }

  public static void LogVerbose(Logger.Source iSource, string iMessage, bool iFileAndLine)
  {
    if (!Logger.sWhiteList.Contains(iSource))
      return;
    string iPrefix = iSource.ToString();
    if (!iFileAndLine)
      return;
    Logger.PrintFileAndLine(iPrefix);
  }

  public static void LogDebug(string iMessage)
  {
  }

  public static void LogDebug(string iMessage, bool iFileAndLine)
  {
  }

  public static void LogDebug(Logger.Source iSource, string iMessage)
  {
  }

  public static void LogDebug(Logger.Source iSource, string iMessage, bool iFileAndLine)
  {
  }

  public static void LogWarning(string iMessage)
  {
  }

  public static void LogWarning(string iMessage, bool iFileAndLine)
  {
  }

  public static void LogWarning(Logger.Source iSource, string iMessage)
  {
  }

  public static void LogWarning(Logger.Source iSource, string iMessage, bool iFileAndLine)
  {
  }

  public static void LogError(string iMessage)
  {
  }

  public static void LogError(string iMessage, bool iFileAndLine)
  {
  }

  public static void LogError(Logger.Source iSource, string iMessage)
  {
  }

  public static void LogError(Logger.Source iSource, string iMessage, bool iFileAndLine)
  {
  }

  public static void LogCritical(string iMessage)
  {
  }

  public static void LogCritical(string iMessage, bool iFileAndLine)
  {
  }

  public static void LogCritical(Logger.Source iSource, string iMessage)
  {
  }

  public static void LogCritical(Logger.Source iSource, string iMessage, bool iFileAndLine)
  {
  }

  private static void PrintFileAndLine(string iPrefix)
  {
  }

  public enum Source
  {
    Global,
    EventParameter,
    GameSparks,
    GameSparksAccount,
    GameSparksServices,
    GameSparksProperties,
    HardwareInfoManager,
    ParadoxAccount,
    ParadoxAccountGameStartup,
    ParadoxAccountPlayerCreate,
    ParadoxAccountPlayerLogin,
    ParadoxAccountPlayerLogout,
    ParadoxAccountSteamLink,
    ParadoxAccountSteamUnlink,
    ParadoxAccountGameSparksAvailable,
    ParadoxPopupUtils,
    ParadoxAccountSaveData,
    ParadoxAccountSequence,
    ParadoxOPS,
    ParadoxServices,
    PlayerSegmentManager,
    TutorialUtils,
    Threads,
  }
}
