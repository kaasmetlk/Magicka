// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.IBoss
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public interface IBoss
{
  BossEnum GetBossType();

  void Initialize(ref Matrix iOrientation);

  void Initialize(ref Matrix iOrientation, int iUniqueID);

  void DeInitialize();

  bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance);

  bool Dead { get; }

  float MaxHitPoints { get; }

  float HitPoints { get; }

  void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted);

  DamageResult Damage(
    int iPartIndex,
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    ref Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures);

  void Damage(int iPartIndex, float iDamage, Elements iElement);

  void SetSlow(int iIndex);

  bool HasStatus(int iIndex, StatusEffects iStatus);

  float StatusMagnitude(int iIndex, StatusEffects iStatus);

  StatusEffect[] GetStatusEffects();

  float ResistanceAgainst(Elements iElement);

  void ScriptMessage(BossMessages iMessage);

  void NetworkUpdate(ref BossUpdateMessage iMsg);

  void NetworkInitialize(ref BossInitializeMessage iMsg);

  bool NetworkInitialized { get; }
}
