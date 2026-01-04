// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Bosses.BossDamageZone
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Bosses;

public class BossDamageZone : BossCollisionZone, IStatusEffected, IDamageable
{
  protected int mIndex;
  private bool mIsEthereal;

  public bool IsEthereal
  {
    get => this.mIsEthereal;
    set => this.mIsEthereal = value;
  }

  public BossDamageZone(
    PlayState iPlayState,
    IBoss iParent,
    int iIndex,
    float iRadius,
    params Primitive[] iPrimitives)
    : base(iPlayState, iParent, iPrimitives)
  {
    this.mIsEthereal = false;
    this.mIndex = iIndex;
    this.mRadius = iRadius;
  }

  public BossDamageZone(
    PlayState iPlayState,
    IBoss iParent,
    int iIndex,
    float iRadius,
    Primitive iPrimitives)
    : base(iPlayState, iParent, iPrimitives)
  {
    this.mIsEthereal = false;
    this.mIndex = iIndex;
    this.mRadius = iRadius;
  }

  public BossDamageZone(PlayState iPlayState, IBoss iParent, int iIndex, float iRadius)
    : base(iPlayState, iParent, (Primitive) new Sphere(new Vector3(), iRadius))
  {
    this.mIsEthereal = false;
    this.mIndex = iIndex;
    this.mRadius = iRadius;
  }

  public float ResistanceAgainst(Elements iElement) => this.mParent.ResistanceAgainst(iElement);

  public override Vector3 CalcImpulseVelocity(
    Vector3 iDirection,
    float iElevation,
    float iMassPower,
    float iDistance)
  {
    return new Vector3();
  }

  public void SetPosition(ref Vector3 iPosition) => this.mBody.MoveTo(iPosition, Matrix.Identity);

  public int ZoneIndex => this.mIndex;

  public DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return this.mParent.Damage(this.mIndex, iDamage, iAttacker, ref iAttackPosition, iFeatures);
  }

  public override bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
  {
    if (!this.mIsEthereal)
      return this.mCollision.SegmentIntersect(out float _, out oPosition, out Vector3 _, iSeg);
    oPosition = new Vector3();
    return false;
  }

  public override bool ArcIntersect(
    out Vector3 oPosition,
    Vector3 iOrigin,
    Vector3 iDirection,
    float iRange,
    float iAngle,
    float iHeightDifference)
  {
    if (this.mIsEthereal)
    {
      oPosition = new Vector3();
      return false;
    }
    if ((double) Math.Abs(this.Position.Y - iOrigin.Y) > (double) iHeightDifference)
    {
      oPosition = new Vector3();
      return false;
    }
    iOrigin.Y = 0.0f;
    iDirection.Y = 0.0f;
    Vector3 position = this.Position with { Y = 0.0f };
    Vector3 result1;
    Vector3.Subtract(ref iOrigin, ref position, out result1);
    if ((double) result1.LengthSquared() <= 9.9999999747524271E-07)
    {
      oPosition = new Vector3();
      return false;
    }
    float num1 = result1.Length();
    if ((double) num1 < (double) this.Radius)
    {
      oPosition = iOrigin;
      return true;
    }
    float mRadius = this.mRadius;
    if ((double) num1 - (double) mRadius > (double) iRange)
    {
      oPosition = new Vector3();
      return false;
    }
    Vector3.Divide(ref result1, num1, out result1);
    float result2;
    Vector3.Dot(ref result1, ref iDirection, out result2);
    result2 = -result2;
    float num2 = (float) Math.Acos((double) result2);
    float num3 = -2f * num1 * num1;
    float num4 = (float) Math.Acos((double) ((mRadius * mRadius + num3) / num3));
    if ((double) num2 - (double) num4 < (double) iAngle)
    {
      Vector3.Multiply(ref result1, mRadius, out result1);
      position = this.Position;
      Vector3.Add(ref position, ref result1, out oPosition);
      return true;
    }
    oPosition = new Vector3();
    return false;
  }

  public bool HasStatus(StatusEffects iStatus) => this.mParent.HasStatus(this.mIndex, iStatus);

  public StatusEffect[] GetStatusEffects() => this.mParent.GetStatusEffects();

  public float StatusMagnitude(StatusEffects iStatus)
  {
    return this.mParent.StatusMagnitude(this.mIndex, iStatus);
  }

  public void SetSlow()
  {
  }

  public void Damage(float iDamage, Elements iElement)
  {
    this.mParent.Damage(this.mIndex, iDamage, iElement);
  }

  public float Volume => 4.18879032f * this.mRadius * this.mRadius * this.mRadius;

  public DamageResult InternalDamage(
    DamageCollection5 iDamages,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    return DamageResult.None | this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures) | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public void OverKill()
  {
    Vector3 position = this.Position;
    int num1 = (int) this.mParent.Damage(0, new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth | Elements.Arcane, 9000000f, 1f), (Entity) null, ref position, Defines.DamageFeatures.Damage);
    int num2 = (int) this.mParent.Damage(1, new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth | Elements.Arcane, 9000000f, 1f), (Entity) null, ref position, Defines.DamageFeatures.Damage);
    int num3 = (int) this.mParent.Damage(2, new Magicka.GameLogic.Damage(AttackProperties.Damage, Elements.Earth | Elements.Arcane, 9000000f, 1f), (Entity) null, ref position, Defines.DamageFeatures.Damage);
  }

  public void Electrocute(IDamageable iTarget, float iMultiplyer)
  {
  }
}
