// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.TutorialUtils
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using System;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry;

public static class TutorialUtils
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.TutorialUtils;
  private const string EXCEPTION_TUTORIAL_MISMATCH = "Began step {0} in a tutorial ({1}) different from the current one ({2}). !";
  private const string TUTORIAL_LOG_MESSAGE = "Tutorial \"{0}\" {1} at step \"{2}\" ({3}s).";
  private static bool sTutorialInProgress = false;
  private static DateTime sStartTime = DateTime.MinValue;
  private static string sCurrentTutorialName = string.Empty;
  private static string sCurrentStepName = string.Empty;

  private static int ElapsedTime
  {
    get
    {
      double elapsedTime = Math.Round((DateTime.Now - TutorialUtils.sStartTime).TotalSeconds);
      if (elapsedTime > (double) int.MaxValue)
      {
        Logger.LogWarning(Logger.Source.TutorialUtils, "The elapsed time value exceed the int type storage capacity !");
        elapsedTime = 0.0;
      }
      return (int) elapsedTime;
    }
  }

  public static bool IsInProgress => TutorialUtils.sTutorialInProgress;

  public static void Start(string iTutorialName, string iStepName)
  {
    if (!TutorialUtils.sTutorialInProgress)
    {
      Logger.LogDebug(Logger.Source.TutorialUtils, $"Tutorial \"{iTutorialName}\" {"begin"} at step \"{iStepName}\" ({0}s).");
      TutorialUtils.sStartTime = DateTime.Now;
      TutorialUtils.sTutorialInProgress = true;
      TutorialUtils.sCurrentTutorialName = iTutorialName;
      TutorialUtils.sCurrentStepName = iStepName;
      TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Started, iTutorialName, iStepName, 0);
    }
    else
      Logger.LogWarning(Logger.Source.TutorialUtils, $"Started new tutorial \"{iTutorialName}\" during an existing tutorial \"{TutorialUtils.sCurrentTutorialName}\" !");
  }

  public static void Step(string iTutorialName, string iStepName)
  {
    if (TutorialUtils.sTutorialInProgress)
    {
      if (!(iTutorialName == TutorialUtils.sCurrentTutorialName))
        throw new Exception($"Began step {iStepName} in a tutorial ({iTutorialName}) different from the current one ({TutorialUtils.sCurrentTutorialName}). !");
      int elapsedTime = TutorialUtils.ElapsedTime;
      Logger.LogDebug(Logger.Source.TutorialUtils, $"Tutorial \"{TutorialUtils.sCurrentTutorialName}\" entered new step \"{iStepName}\" ({elapsedTime.ToString()}s).");
      Singleton<ParadoxServices>.Instance.TelemetryEvent(new EventData[2]
      {
        TelemetryUtils.GetTutorialActionData(EventEnums.TutorialAction.Completed, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime),
        TelemetryUtils.GetTutorialActionData(EventEnums.TutorialAction.Started, TutorialUtils.sCurrentTutorialName, iStepName, elapsedTime)
      });
      TutorialUtils.sCurrentStepName = iStepName;
    }
    else
    {
      Logger.LogWarning(Logger.Source.TutorialUtils, $"Entered step \"{iStepName}\" before any tutorial was started !");
      TutorialUtils.Start(iTutorialName, iStepName);
    }
  }

  public static void Complete()
  {
    if (TutorialUtils.sTutorialInProgress)
    {
      int elapsedTime = TutorialUtils.ElapsedTime;
      Logger.LogDebug(Logger.Source.TutorialUtils, $"Tutorial \"{TutorialUtils.sCurrentTutorialName}\" {"completed"} at step \"{TutorialUtils.sCurrentStepName}\" ({elapsedTime}s).");
      Singleton<PlayerSegmentManager>.Instance.NotifyTutorialEnded();
      TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Completed, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
      TutorialUtils.Reset();
    }
    else
      Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to complete a tutorial when no tutorial is in progress.");
  }

  public static void Fail()
  {
    if (TutorialUtils.sTutorialInProgress)
    {
      int elapsedTime = TutorialUtils.ElapsedTime;
      Logger.LogDebug(Logger.Source.TutorialUtils, $"Tutorial \"{TutorialUtils.sCurrentTutorialName}\" {"failed"} at step \"{TutorialUtils.sCurrentStepName}\" ({elapsedTime}s).");
      TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Failed, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
      TutorialUtils.Reset();
    }
    else
      Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to fail a tutorial when no tutorial is in progress.");
  }

  public static void Skip()
  {
    if (TutorialUtils.sTutorialInProgress)
    {
      int elapsedTime = TutorialUtils.ElapsedTime;
      Logger.LogDebug(Logger.Source.TutorialUtils, $"Tutorial \"{TutorialUtils.sCurrentTutorialName}\" {"skipped"} at step \"{TutorialUtils.sCurrentStepName}\" ({elapsedTime}s).");
      TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Skipped, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
      TutorialUtils.Reset();
    }
    else
      Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to skip a tutorial when no tutorial was in progress.");
  }

  public static void Restart()
  {
    if (TutorialUtils.sTutorialInProgress)
    {
      int elapsedTime = TutorialUtils.ElapsedTime;
      Logger.LogDebug(Logger.Source.TutorialUtils, $"Tutorial \"{TutorialUtils.sCurrentTutorialName}\" {"restarted"} at step \"{TutorialUtils.sCurrentStepName}\" ({elapsedTime}s).");
      TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Restarted, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
      TutorialUtils.Reset();
    }
    else
      Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to restart a tutorial when no tutorial was in progress.");
  }

  public static void Quit()
  {
    if (TutorialUtils.sTutorialInProgress)
    {
      int elapsedTime = TutorialUtils.ElapsedTime;
      Logger.LogDebug(Logger.Source.TutorialUtils, $"Tutorial \"{TutorialUtils.sCurrentTutorialName}\" {"quit"} at step \"{TutorialUtils.sCurrentStepName}\" ({elapsedTime}s).");
      TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Quit, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
      TutorialUtils.Reset();
    }
    else
      Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to quit a tutorial when no tutorial was in progress.");
  }

  private static void Reset()
  {
    TutorialUtils.sCurrentTutorialName = string.Empty;
    TutorialUtils.sCurrentStepName = string.Empty;
    TutorialUtils.sStartTime = DateTime.MinValue;
    TutorialUtils.sTutorialInProgress = false;
  }
}
