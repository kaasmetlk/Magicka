// Decompiled with JetBrains decompiler
// Type: Magicka.Network.PacketType
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Network;

public enum PacketType : byte
{
  Ping = 0,
  Connect = 1,
  Authenticate = 2,
  LobbyInfo = 3,
  ClientConnected = 4,
  ConnectionClosed = 5,
  GameFull = 6,
  GameInfo = 7,
  VersusOptions = 8,
  PackOptions = 9,
  ChatMessage = 10, // 0x0A
  GamerJoin = 11, // 0x0B
  GamerChanged = 12, // 0x0C
  GamerLeave = 13, // 0x0D
  GamerReady = 14, // 0x0E
  LevelList = 15, // 0x0F
  SaveData = 16, // 0x10
  MenuSelection = 17, // 0x11
  GameChanged = 18, // 0x12
  GameBeginSceneChange = 19, // 0x13
  GameEndLoad = 20, // 0x14
  GameEnd = 21, // 0x15
  GameRestart = 22, // 0x16
  TriggerAction = 23, // 0x17
  ScoreAdded = 24, // 0x18
  StatisticsUpdate = 25, // 0x19
  LeaderboardEntry = 26, // 0x1A
  RulesetUpdate = 27, // 0x1B
  DialogAdvance = 28, // 0x1C
  GameUpdate = 29, // 0x1D
  EntityUpdate = 30, // 0x1E
  PlayerUpdate = 31, // 0x1F
  CharacterAction = 32, // 0x20
  AnimatedLevelPartUpdate = 33, // 0x21
  BossUpdate = 34, // 0x22
  BossInitialize = 35, // 0x23
  SpawnPlayer = 36, // 0x24
  SpawnMissile = 37, // 0x25
  SpawnShield = 38, // 0x26
  SpawnBarrier = 39, // 0x27
  SpawnWave = 40, // 0x28
  SpawnMine = 41, // 0x29
  SpawnVortex = 42, // 0x2A
  SpawnPortal = 43, // 0x2B
  Damage = 44, // 0x2C
  EntityRemove = 45, // 0x2D
  CharacterDie = 46, // 0x2E
  MissileEntity = 47, // 0x2F
  Threat = 48, // 0x30
  EnterSync = 49, // 0x31
  LeaveSync = 50, // 0x32
  Checkpoint = 51, // 0x33
  ForceSyncPlayersMessage = 52, // 0x34
  RequestForcedPlayerStatusSync = 53, // 0x35
  Request = 128, // 0x80
}
