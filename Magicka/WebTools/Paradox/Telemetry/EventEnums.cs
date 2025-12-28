using System;

namespace Magicka.WebTools.Paradox.Telemetry
{
	// Token: 0x02000004 RID: 4
	public static class EventEnums
	{
		// Token: 0x02000005 RID: 5
		public enum TutorialAction
		{
			// Token: 0x0400000F RID: 15
			Started,
			// Token: 0x04000010 RID: 16
			Completed,
			// Token: 0x04000011 RID: 17
			Failed,
			// Token: 0x04000012 RID: 18
			Skipped,
			// Token: 0x04000013 RID: 19
			Restarted,
			// Token: 0x04000014 RID: 20
			Quit
		}

		// Token: 0x02000006 RID: 6
		public enum ControllerType
		{
			// Token: 0x04000016 RID: 22
			Keyboard,
			// Token: 0x04000017 RID: 23
			Gamepad,
			// Token: 0x04000018 RID: 24
			NotApplicable
		}

		// Token: 0x02000007 RID: 7
		public enum DeathCategory
		{
			// Token: 0x0400001A RID: 26
			Unknown,
			// Token: 0x0400001B RID: 27
			Suicide,
			// Token: 0x0400001C RID: 28
			PlayerVsPlayer,
			// Token: 0x0400001D RID: 29
			FriendlyFire,
			// Token: 0x0400001E RID: 30
			NPC,
			// Token: 0x0400001F RID: 31
			Environment
		}
	}
}
