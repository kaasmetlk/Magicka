// Decompiled with JetBrains decompiler
// Type: Magicka.Network.TriggerActionType
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Network;

internal enum TriggerActionType : byte
{
  TriggerExecute,
  SpawnNPC,
  SpawnElemental,
  SpawnLuggage,
  SpawnFood,
  SpawnItem,
  SpawnMagick,
  ChangeScene,
  EnterScene,
  ThunderBolt,
  LightningBolt,
  SpawnGrease,
  Nullify,
  SpawnTornado,
  NapalmStrike,
  EarthQuake,
  Charm,
  Starfall,
  OtherworldlyDischarge,
  OtherworldlyBoltDestroyed,
  Confuse,
  StarGaze,
}
