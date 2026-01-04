// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.IDamageable
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities;

public interface IDamageable
{
  ushort Handle { get; }

  bool Dead { get; }

  Vector3 Position { get; }

  float HitPoints { get; }

  float MaxHitPoints { get; }

  float Radius { get; }

  Body Body { get; }

  bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius);

  DamageResult InternalDamage(
    DamageCollection5 iDamages,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures);

  DamageResult InternalDamage(
    Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures);

  bool ArcIntersect(
    out Vector3 oPosition,
    Vector3 iOrigin,
    Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference);

  void Kill();

  void OverKill();

  void Electrocute(IDamageable iTarget, float iMultiplyer);

  float ResistanceAgainst(Elements iElement);
}
