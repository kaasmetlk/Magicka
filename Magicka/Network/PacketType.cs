using System;

namespace Magicka.Network
{
	// Token: 0x02000206 RID: 518
	public enum PacketType : byte
	{
		// Token: 0x04000F76 RID: 3958
		Ping,
		// Token: 0x04000F77 RID: 3959
		Connect,
		// Token: 0x04000F78 RID: 3960
		Authenticate,
		// Token: 0x04000F79 RID: 3961
		LobbyInfo,
		// Token: 0x04000F7A RID: 3962
		ClientConnected,
		// Token: 0x04000F7B RID: 3963
		ConnectionClosed,
		// Token: 0x04000F7C RID: 3964
		GameFull,
		// Token: 0x04000F7D RID: 3965
		GameInfo,
		// Token: 0x04000F7E RID: 3966
		VersusOptions,
		// Token: 0x04000F7F RID: 3967
		PackOptions,
		// Token: 0x04000F80 RID: 3968
		ChatMessage,
		// Token: 0x04000F81 RID: 3969
		GamerJoin,
		// Token: 0x04000F82 RID: 3970
		GamerChanged,
		// Token: 0x04000F83 RID: 3971
		GamerLeave,
		// Token: 0x04000F84 RID: 3972
		GamerReady,
		// Token: 0x04000F85 RID: 3973
		LevelList,
		// Token: 0x04000F86 RID: 3974
		SaveData,
		// Token: 0x04000F87 RID: 3975
		MenuSelection,
		// Token: 0x04000F88 RID: 3976
		GameChanged,
		// Token: 0x04000F89 RID: 3977
		GameBeginSceneChange,
		// Token: 0x04000F8A RID: 3978
		GameEndLoad,
		// Token: 0x04000F8B RID: 3979
		GameEnd,
		// Token: 0x04000F8C RID: 3980
		GameRestart,
		// Token: 0x04000F8D RID: 3981
		TriggerAction,
		// Token: 0x04000F8E RID: 3982
		ScoreAdded,
		// Token: 0x04000F8F RID: 3983
		StatisticsUpdate,
		// Token: 0x04000F90 RID: 3984
		LeaderboardEntry,
		// Token: 0x04000F91 RID: 3985
		RulesetUpdate,
		// Token: 0x04000F92 RID: 3986
		DialogAdvance,
		// Token: 0x04000F93 RID: 3987
		GameUpdate,
		// Token: 0x04000F94 RID: 3988
		EntityUpdate,
		// Token: 0x04000F95 RID: 3989
		PlayerUpdate,
		// Token: 0x04000F96 RID: 3990
		CharacterAction,
		// Token: 0x04000F97 RID: 3991
		AnimatedLevelPartUpdate,
		// Token: 0x04000F98 RID: 3992
		BossUpdate,
		// Token: 0x04000F99 RID: 3993
		BossInitialize,
		// Token: 0x04000F9A RID: 3994
		SpawnPlayer,
		// Token: 0x04000F9B RID: 3995
		SpawnMissile,
		// Token: 0x04000F9C RID: 3996
		SpawnShield,
		// Token: 0x04000F9D RID: 3997
		SpawnBarrier,
		// Token: 0x04000F9E RID: 3998
		SpawnWave,
		// Token: 0x04000F9F RID: 3999
		SpawnMine,
		// Token: 0x04000FA0 RID: 4000
		SpawnVortex,
		// Token: 0x04000FA1 RID: 4001
		SpawnPortal,
		// Token: 0x04000FA2 RID: 4002
		Damage,
		// Token: 0x04000FA3 RID: 4003
		EntityRemove,
		// Token: 0x04000FA4 RID: 4004
		CharacterDie,
		// Token: 0x04000FA5 RID: 4005
		MissileEntity,
		// Token: 0x04000FA6 RID: 4006
		Threat,
		// Token: 0x04000FA7 RID: 4007
		EnterSync,
		// Token: 0x04000FA8 RID: 4008
		LeaveSync,
		// Token: 0x04000FA9 RID: 4009
		Checkpoint,
		// Token: 0x04000FAA RID: 4010
		ForceSyncPlayersMessage,
		// Token: 0x04000FAB RID: 4011
		RequestForcedPlayerStatusSync,
		// Token: 0x04000FAC RID: 4012
		Request = 128
	}
}
