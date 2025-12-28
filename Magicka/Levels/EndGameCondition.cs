using System;

namespace Magicka.Levels
{
	// Token: 0x02000008 RID: 8
	public enum EndGameCondition : byte
	{
		// Token: 0x04000021 RID: 33
		None,
		// Token: 0x04000022 RID: 34
		Victory,
		// Token: 0x04000023 RID: 35
		LevelComplete,
		// Token: 0x04000024 RID: 36
		Defeat,
		// Token: 0x04000025 RID: 37
		VersusPlayer,
		// Token: 0x04000026 RID: 38
		VersusTeam,
		// Token: 0x04000027 RID: 39
		EightyeightMilesPerHour,
		// Token: 0x04000028 RID: 40
		ChallengeExit,
		// Token: 0x04000029 RID: 41
		EndOffGame,
		// Token: 0x0400002A RID: 42
		Disconnected,
		// Token: 0x0400002B RID: 43
		ToBeContinued
	}
}
