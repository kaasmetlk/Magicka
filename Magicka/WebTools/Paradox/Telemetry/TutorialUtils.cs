using System;
using Magicka.CoreFramework;
using Magicka.Misc;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x0200054E RID: 1358
	public static class TutorialUtils
	{
		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x06002858 RID: 10328 RVA: 0x0013C448 File Offset: 0x0013A648
		private static int ElapsedTime
		{
			get
			{
				double num = Math.Round((DateTime.Now - TutorialUtils.sStartTime).TotalSeconds);
				if (num > 2147483647.0)
				{
					Logger.LogWarning(Logger.Source.TutorialUtils, "The elapsed time value exceed the int type storage capacity !");
					num = 0.0;
				}
				return (int)num;
			}
		}

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x06002859 RID: 10329 RVA: 0x0013C496 File Offset: 0x0013A696
		public static bool IsInProgress
		{
			get
			{
				return TutorialUtils.sTutorialInProgress;
			}
		}

		// Token: 0x0600285A RID: 10330 RVA: 0x0013C4A0 File Offset: 0x0013A6A0
		public static void Start(string iTutorialName, string iStepName)
		{
			if (!TutorialUtils.sTutorialInProgress)
			{
				Logger.LogDebug(Logger.Source.TutorialUtils, string.Format("Tutorial \"{0}\" {1} at step \"{2}\" ({3}s).", new object[]
				{
					iTutorialName,
					"begin",
					iStepName,
					0
				}));
				TutorialUtils.sStartTime = DateTime.Now;
				TutorialUtils.sTutorialInProgress = true;
				TutorialUtils.sCurrentTutorialName = iTutorialName;
				TutorialUtils.sCurrentStepName = iStepName;
				TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Started, iTutorialName, iStepName, 0);
				return;
			}
			Logger.LogWarning(Logger.Source.TutorialUtils, string.Format("Started new tutorial \"{0}\" during an existing tutorial \"{1}\" !", iTutorialName, TutorialUtils.sCurrentTutorialName));
		}

		// Token: 0x0600285B RID: 10331 RVA: 0x0013C524 File Offset: 0x0013A724
		public static void Step(string iTutorialName, string iStepName)
		{
			if (!TutorialUtils.sTutorialInProgress)
			{
				Logger.LogWarning(Logger.Source.TutorialUtils, string.Format("Entered step \"{0}\" before any tutorial was started !", iStepName));
				TutorialUtils.Start(iTutorialName, iStepName);
				return;
			}
			if (iTutorialName == TutorialUtils.sCurrentTutorialName)
			{
				int elapsedTime = TutorialUtils.ElapsedTime;
				Logger.LogDebug(Logger.Source.TutorialUtils, string.Format("Tutorial \"{0}\" entered new step \"{1}\" ({2}s).", TutorialUtils.sCurrentTutorialName, iStepName, elapsedTime.ToString()));
				EventData[] iEvents = new EventData[]
				{
					TelemetryUtils.GetTutorialActionData(EventEnums.TutorialAction.Completed, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime),
					TelemetryUtils.GetTutorialActionData(EventEnums.TutorialAction.Started, TutorialUtils.sCurrentTutorialName, iStepName, elapsedTime)
				};
				Singleton<ParadoxServices>.Instance.TelemetryEvent(iEvents);
				TutorialUtils.sCurrentStepName = iStepName;
				return;
			}
			throw new Exception(string.Format("Began step {0} in a tutorial ({1}) different from the current one ({2}). !", iStepName, iTutorialName, TutorialUtils.sCurrentTutorialName));
		}

		// Token: 0x0600285C RID: 10332 RVA: 0x0013C5DC File Offset: 0x0013A7DC
		public static void Complete()
		{
			if (TutorialUtils.sTutorialInProgress)
			{
				int elapsedTime = TutorialUtils.ElapsedTime;
				Logger.LogDebug(Logger.Source.TutorialUtils, string.Format("Tutorial \"{0}\" {1} at step \"{2}\" ({3}s).", new object[]
				{
					TutorialUtils.sCurrentTutorialName,
					"completed",
					TutorialUtils.sCurrentStepName,
					elapsedTime
				}));
				Singleton<PlayerSegmentManager>.Instance.NotifyTutorialEnded();
				TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Completed, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
				TutorialUtils.Reset();
				return;
			}
			Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to complete a tutorial when no tutorial is in progress.");
		}

		// Token: 0x0600285D RID: 10333 RVA: 0x0013C660 File Offset: 0x0013A860
		public static void Fail()
		{
			if (TutorialUtils.sTutorialInProgress)
			{
				int elapsedTime = TutorialUtils.ElapsedTime;
				Logger.LogDebug(Logger.Source.TutorialUtils, string.Format("Tutorial \"{0}\" {1} at step \"{2}\" ({3}s).", new object[]
				{
					TutorialUtils.sCurrentTutorialName,
					"failed",
					TutorialUtils.sCurrentStepName,
					elapsedTime
				}));
				TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Failed, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
				TutorialUtils.Reset();
				return;
			}
			Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to fail a tutorial when no tutorial is in progress.");
		}

		// Token: 0x0600285E RID: 10334 RVA: 0x0013C6D8 File Offset: 0x0013A8D8
		public static void Skip()
		{
			if (TutorialUtils.sTutorialInProgress)
			{
				int elapsedTime = TutorialUtils.ElapsedTime;
				Logger.LogDebug(Logger.Source.TutorialUtils, string.Format("Tutorial \"{0}\" {1} at step \"{2}\" ({3}s).", new object[]
				{
					TutorialUtils.sCurrentTutorialName,
					"skipped",
					TutorialUtils.sCurrentStepName,
					elapsedTime
				}));
				TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Skipped, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
				TutorialUtils.Reset();
				return;
			}
			Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to skip a tutorial when no tutorial was in progress.");
		}

		// Token: 0x0600285F RID: 10335 RVA: 0x0013C750 File Offset: 0x0013A950
		public static void Restart()
		{
			if (TutorialUtils.sTutorialInProgress)
			{
				int elapsedTime = TutorialUtils.ElapsedTime;
				Logger.LogDebug(Logger.Source.TutorialUtils, string.Format("Tutorial \"{0}\" {1} at step \"{2}\" ({3}s).", new object[]
				{
					TutorialUtils.sCurrentTutorialName,
					"restarted",
					TutorialUtils.sCurrentStepName,
					elapsedTime
				}));
				TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Restarted, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
				TutorialUtils.Reset();
				return;
			}
			Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to restart a tutorial when no tutorial was in progress.");
		}

		// Token: 0x06002860 RID: 10336 RVA: 0x0013C7C8 File Offset: 0x0013A9C8
		public static void Quit()
		{
			if (TutorialUtils.sTutorialInProgress)
			{
				int elapsedTime = TutorialUtils.ElapsedTime;
				Logger.LogDebug(Logger.Source.TutorialUtils, string.Format("Tutorial \"{0}\" {1} at step \"{2}\" ({3}s).", new object[]
				{
					TutorialUtils.sCurrentTutorialName,
					"quit",
					TutorialUtils.sCurrentStepName,
					elapsedTime
				}));
				TelemetryUtils.SendTutorialAction(EventEnums.TutorialAction.Quit, TutorialUtils.sCurrentTutorialName, TutorialUtils.sCurrentStepName, elapsedTime);
				TutorialUtils.Reset();
				return;
			}
			Logger.LogWarning(Logger.Source.TutorialUtils, "Trying to quit a tutorial when no tutorial was in progress.");
		}

		// Token: 0x06002861 RID: 10337 RVA: 0x0013C83F File Offset: 0x0013AA3F
		private static void Reset()
		{
			TutorialUtils.sCurrentTutorialName = string.Empty;
			TutorialUtils.sCurrentStepName = string.Empty;
			TutorialUtils.sStartTime = DateTime.MinValue;
			TutorialUtils.sTutorialInProgress = false;
		}

		// Token: 0x04002BD8 RID: 11224
		private const Logger.Source LOGGER_SOURCE = Logger.Source.TutorialUtils;

		// Token: 0x04002BD9 RID: 11225
		private const string EXCEPTION_TUTORIAL_MISMATCH = "Began step {0} in a tutorial ({1}) different from the current one ({2}). !";

		// Token: 0x04002BDA RID: 11226
		private const string TUTORIAL_LOG_MESSAGE = "Tutorial \"{0}\" {1} at step \"{2}\" ({3}s).";

		// Token: 0x04002BDB RID: 11227
		private static bool sTutorialInProgress = false;

		// Token: 0x04002BDC RID: 11228
		private static DateTime sStartTime = DateTime.MinValue;

		// Token: 0x04002BDD RID: 11229
		private static string sCurrentTutorialName = string.Empty;

		// Token: 0x04002BDE RID: 11230
		private static string sCurrentStepName = string.Empty;
	}
}
