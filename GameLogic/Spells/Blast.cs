// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.Blast
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Spells;

internal static class Blast
{
  private const int SEGMENTS = 8;
  private const float SEGMENTS_DIVISOR = 0.125f;
  private static readonly Dictionary<float, int> SEGMENT_LOOKUP = new Dictionary<float, int>(8);
  private static readonly int[] BLAST_EFFECT_LOOKUP;
  private static readonly int[] GROUND_EFFECT_LOOKUP;

  static Blast()
  {
    Blast.SEGMENT_LOOKUP.Add(0.0f, 0);
    Blast.SEGMENT_LOOKUP.Add(0.7853982f, 1);
    Blast.SEGMENT_LOOKUP.Add(1.57079637f, 2);
    Blast.SEGMENT_LOOKUP.Add(2.3561945f, 3);
    Blast.SEGMENT_LOOKUP.Add(3.14159274f, 4);
    Blast.SEGMENT_LOOKUP.Add(3.926991f, 5);
    Blast.SEGMENT_LOOKUP.Add(4.712389f, 6);
    Blast.SEGMENT_LOOKUP.Add(5.49778748f, 7);
    Blast.BLAST_EFFECT_LOOKUP = new int[11];
    Blast.GROUND_EFFECT_LOOKUP = new int[11];
    Blast.GROUND_EFFECT_LOOKUP[0] = "area_ground_earth".GetHashCodeCustom();
    for (int iIndex = 1; iIndex < Blast.GROUND_EFFECT_LOOKUP.Length; ++iIndex)
    {
      Elements elements = Spell.ElementFromIndex(iIndex);
      Blast.GROUND_EFFECT_LOOKUP[iIndex] = ("area_ground_" + elements.ToString().ToLowerInvariant()).GetHashCodeCustom();
    }
    Blast.BLAST_EFFECT_LOOKUP[0] = "area_ground_earth".GetHashCodeCustom();
    for (int iIndex = 1; iIndex < Blast.BLAST_EFFECT_LOOKUP.Length; ++iIndex)
    {
      Elements elements = Spell.ElementFromIndex(iIndex);
      Blast.BLAST_EFFECT_LOOKUP[iIndex] = $"area_{elements.ToString().ToLowerInvariant()}1_b".GetHashCodeCustom();
    }
  }

  public static DamageResult FullBlast(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    float iRadius,
    Vector3 iPosition,
    Damage iDamage)
  {
    float[] blastScalars = Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition);
    if ((iDamage.Element & Elements.Beams) != Elements.None)
    {
      ArcaneBlast instance = ArcaneBlast.GetInstance();
      Spell oSpell;
      Spell.DefaultSpell(iDamage.Element, out oSpell);
      instance.Initialize(iOwner, iPosition, oSpell.GetColor(), iRadius, iDamage.Element);
    }
    else
    {
      for (int iIndex = 0; iIndex < 11; ++iIndex)
      {
        Elements elements = Spell.ElementFromIndex(iIndex);
        if ((elements & Elements.Earth) != Elements.Earth & (iDamage.Element & elements) == elements)
          Blast.BlastVisual(iPlayState, iOwner, iRadius, iPosition, iDamage.Magnitude, blastScalars, new int?(Blast.BLAST_EFFECT_LOOKUP[iIndex]), new int?(), new Decal?(), new float?());
      }
    }
    iPlayState.Camera.CameraShake(iPosition, iRadius * 0.333f, 0.4f);
    return Blast.BlastDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, iPosition, iRadius, iDamage, true);
  }

  public static unsafe DamageResult FullBlast(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    float iRadius,
    Vector3 iPosition,
    DamageCollection5 iDamage)
  {
    float[] blastScalars = Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition);
    Damage* damagePtr = &iDamage.A;
    if ((iDamage.GetAllElements() & Elements.Beams) != Elements.None)
    {
      ArcaneBlast instance = ArcaneBlast.GetInstance();
      Spell oSpell;
      Spell.DefaultSpell(iDamage.GetAllElements(), out oSpell);
      instance.Initialize(iOwner, iPosition, oSpell.GetColor(), iRadius, iDamage.GetAllElements());
    }
    else
    {
      for (int index1 = 0; index1 < 5; ++index1)
      {
        Decal? iDecal = new Decal?();
        float? iDecalAlpha = new float?();
        if (damagePtr[index1].Element != Elements.None)
        {
          int index2 = MagickaMath.CountTrailingZeroBits((uint) damagePtr[index1].Element);
          if (damagePtr[index1].Element == Elements.Fire)
            iDecal = new Decal?(Decal.Scorched);
          else if (damagePtr[index1].Element == Elements.Cold)
            iDecal = new Decal?(Decal.Cold);
          else if (damagePtr[index1].Element == Elements.Water)
            iDecal = new Decal?(Decal.Water);
          Blast.BlastVisual(iPlayState, iOwner, iRadius, iPosition, damagePtr[index1].Magnitude, blastScalars, new int?(Blast.BLAST_EFFECT_LOOKUP[index2]), new int?(), iDecal, iDecalAlpha);
        }
      }
    }
    iPlayState.Camera.CameraShake(iPosition, iRadius * 0.333f, 0.3f);
    return Blast.BlastDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, iPosition, iRadius, iDamage, true, false);
  }

  public static DamageResult BlastDamage(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    Vector3 iPosition,
    float iRadius,
    DamageCollection5 iDamage,
    bool iFalloff,
    bool iGroundBlast)
  {
    DamageResult damageResult = DamageResult.None;
    List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, iRadius, true);
    entities.Remove(iIgnoreEntity);
    Segment segment;
    segment.Origin = iPosition;
    bool flag = (iDamage.GetAllElements() & (Elements.Water | Elements.Steam)) == Elements.Water & iIgnoreEntity == iOwner;
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is IDamageable t && (!(t is Character) || !(t as Character).IsEthereal && (!iGroundBlast || !(t as Character).IsLevitating) && !(t as Character).IsEthereal))
      {
        segment.Delta = entities[index].Position;
        Vector3.Subtract(ref segment.Delta, ref segment.Origin, out segment.Delta);
        float scaleFactor;
        Vector3 vector3_1;
        Vector3 vector3_2;
        if (t.Body.CollisionSkin.SegmentIntersect(out scaleFactor, out vector3_1, out vector3_2, segment))
          Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
        if (!iPlayState.Level.CurrentScene.SegmentIntersect(out scaleFactor, out vector3_1, out vector3_2, segment))
        {
          float iDistance = segment.Delta.Length() - t.Radius;
          if ((double) iDistance <= (double) iRadius)
          {
            if (flag)
              t.Electrocute(iOwner as IDamageable, 1f);
            DamageCollection5 iDamage1 = iDamage;
            if (iFalloff && (iDamage1.GetAllElements() & Elements.Beams) == Elements.None)
            {
              float iMultiplier = Blast.BlastDamageScale(iDistance, iRadius);
              iDamage1.MultiplyMagnitude(iMultiplier);
            }
            if (iOwner != null)
            {
              int num1 = (int) t.Damage(iDamage1, iOwner, iTimeStamp, iPosition);
            }
            else
            {
              int num2 = (int) t.Damage(iDamage1, (Entity) null, iTimeStamp, iPosition);
            }
          }
        }
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    Vector3 result = Vector3.Right;
    Vector3.Multiply(ref result, iRadius, out result);
    Liquid.Freeze(iPlayState.Level.CurrentScene, ref iPosition, ref result, 6.28318548f, 1f, ref iDamage);
    return damageResult;
  }

  public static DamageResult BlastDamage(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    Vector3 iPosition,
    float iRadius,
    Damage iDamage,
    bool iFalloff)
  {
    DamageResult damageResult = DamageResult.None;
    List<Entity> entities = iPlayState.EntityManager.GetEntities(iPosition, iRadius, true);
    entities.Remove(iIgnoreEntity);
    Segment iSeg;
    iSeg.Origin = iPosition;
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is IDamageable t)
      {
        iSeg.Delta = entities[index].Position;
        Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
        if (!iPlayState.Level.CurrentScene.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, iSeg))
        {
          float iDistance = iSeg.Delta.Length() - t.Radius;
          if ((double) iDistance <= (double) iRadius)
          {
            Damage iDamage1 = iDamage;
            if (iFalloff && (iDamage1.Element & Elements.Beams) == Elements.None)
              iDamage1.Magnitude *= Blast.BlastDamageScale(iDistance, iRadius);
            damageResult |= t.Damage(iDamage1, iOwner, iTimeStamp, iPosition);
          }
        }
      }
    }
    iPlayState.EntityManager.ReturnEntityList(entities);
    Vector3 result = Vector3.Right;
    Vector3.Multiply(ref result, iRadius, out result);
    Liquid.Freeze(iPlayState.Level.CurrentScene, ref iPosition, ref result, 6.28318548f, 1f, ref iDamage);
    return damageResult;
  }

  private static float BlastDamageScale(float iDistance, float iRange)
  {
    float num = Math.Max(iDistance / iRange, 0.0f);
    return (float) (1.0 - (double) num * (double) num * (double) num);
  }

  public static void BlastVisual(
    PlayState iPlayState,
    Entity iOwner,
    float iRadius,
    Vector3 iPosition,
    float iMagnitude,
    float[] iFracs,
    int? iBlastEffect,
    int? iAfterEffect,
    Decal? iDecal,
    float? iDecalAlpha)
  {
    float[] numArray = new float[8];
    if (iFracs != null)
      iFracs.CopyTo((Array) numArray, 0);
    else
      numArray = Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition);
    Vector3 up = Vector3.Up;
    for (int index = 0; index < 8; ++index)
    {
      Matrix result1;
      Matrix.CreateRotationY((float) ((double) index * 0.125 * 6.2831854820251465), out result1);
      Vector3 result2 = result1.Right;
      numArray[index] *= iRadius * 0.1f;
      result1.Translation = iPosition;
      result1.M11 *= numArray[index];
      result1.M12 *= numArray[index];
      result1.M13 *= numArray[index];
      result1.M31 *= numArray[index];
      result1.M32 *= numArray[index];
      result1.M33 *= numArray[index];
      VisualEffectReference oRef;
      if (iBlastEffect.HasValue)
        EffectManager.Instance.StartEffect(iBlastEffect.Value, ref result1, out oRef);
      Vector2 iScale = new Vector2(iRadius, iRadius);
      Vector3 iPosition1 = iPosition;
      Vector3.Multiply(ref result2, iRadius * numArray[index], out result2);
      Vector3.Add(ref iPosition1, ref result2, out iPosition1);
      AnimatedLevelPart oAnimatedLevelPart;
      if (iPlayState.Level.CurrentScene.SegmentIntersect(out float _, out iPosition1, out Vector3 _, out oAnimatedLevelPart, new Segment()
      {
        Origin = iPosition1,
        Delta = {
          Y = -2f
        }
      }))
      {
        iPosition1.Y += 0.1f;
        if (iDecal.HasValue)
        {
          float iAlpha = iDecalAlpha.HasValue ? iDecalAlpha.Value : MathHelper.Clamp(iMagnitude / 5f, 0.25f, 0.75f);
          DecalManager.Instance.AddAlphaBlendedDecal(iDecal.Value, oAnimatedLevelPart, ref iScale, ref iPosition1, new Vector3?(result2), ref up, 60f, iAlpha);
        }
        if (iAfterEffect.HasValue)
        {
          result1.Translation = iPosition1;
          result1.M11 *= iScale.X * numArray[index];
          result1.M12 *= iScale.X * numArray[index];
          result1.M13 *= iScale.X * numArray[index];
          result1.M31 *= iScale.Y * numArray[index];
          result1.M32 *= iScale.Y * numArray[index];
          result1.M33 *= iScale.Y * numArray[index];
          EffectManager.Instance.StartEffect(iAfterEffect.Value, ref result1, out oRef);
        }
      }
    }
  }

  public static float[] GetBlastScalars(
    PlayState iPlayState,
    Entity iOwner,
    float iRadius,
    Vector3 iPosition)
  {
    return Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition, false, false);
  }

  public static float[] GetBlastScalars(
    PlayState iPlayState,
    Entity iOwner,
    float iRadius,
    Vector3 iPosition,
    bool iIgnoreWorld,
    bool iIgnoreShields)
  {
    float[] blastScalars = new float[8];
    List<Shield> shields = iPlayState.EntityManager.Shields;
    Segment segment = new Segment(iPosition, Vector3.Zero);
    for (int index1 = 0; index1 < 8; ++index1)
    {
      Matrix result;
      Matrix.CreateRotationY((float) ((double) index1 * 0.125 * 6.2831854820251465), out result);
      Vector3 right = result.Right;
      Vector3.Multiply(ref right, iRadius, out segment.Delta);
      if (iIgnoreWorld || !iPlayState.Level.CurrentScene.SegmentIntersect(out blastScalars[index1], out right, out right, segment))
        blastScalars[index1] = 1f;
      float frac = blastScalars[index1];
      if (!iIgnoreShields)
      {
        for (int index2 = 0; index2 < shields.Count; ++index2)
        {
          if (shields[index2].Body.CollisionSkin.SegmentIntersect(out frac, out right, out right, segment))
            blastScalars[index1] = (double) blastScalars[index1] > (double) frac ? frac : blastScalars[index1];
        }
      }
    }
    return blastScalars;
  }

  private static int GetSegmentIndex(float iAngle)
  {
    iAngle *= 1.27323949f;
    iAngle = (float) Math.Floor((double) iAngle);
    iAngle *= 0.7853982f;
    return Blast.SEGMENT_LOOKUP[iAngle];
  }

  public static unsafe DamageResult GroundBlast(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    float iRadius,
    Vector3 iPosition,
    DamageCollection5 iDamage)
  {
    float[] blastScalars = Blast.GetBlastScalars(iPlayState, iOwner, iRadius, iPosition, true, true);
    Damage* damagePtr = &iDamage.A;
    Spell oSpell;
    Spell.DefaultSpell(iDamage.GetAllElements() & ~Elements.Earth, out oSpell);
    if ((double) oSpell.TotalMagnitude() > 0.0)
    {
      DynamicLight cachedLight = DynamicLight.GetCachedLight();
      cachedLight.Initialize(iPosition, oSpell.GetColor(), 6f, iRadius * 2f, 4f, 1f, 1f);
      cachedLight.Enable();
    }
    for (int index1 = 0; index1 < 5; ++index1)
    {
      Decal? iDecal = new Decal?();
      float? iDecalAlpha = new float?();
      if (damagePtr[index1].Element != Elements.None)
      {
        int index2 = MagickaMath.CountTrailingZeroBits((uint) damagePtr[index1].Element);
        if ((damagePtr[index1].Element & Elements.PhysicalElements) != Elements.None)
        {
          iDecal = new Decal?(Decal.Cracks);
          iDecalAlpha = new float?(1f);
        }
        if (damagePtr[index1].Element == Elements.Fire)
          iDecal = new Decal?(Decal.Scorched);
        else if (damagePtr[index1].Element == Elements.Cold)
          iDecal = new Decal?(Decal.Cold);
        else if (damagePtr[index1].Element == Elements.Water)
          iDecal = new Decal?(Decal.Water);
        if ((damagePtr[index1].Element & Elements.Ice) != Elements.None)
          IceSpikes.GetInstance().Initialize(iOwner.PlayState, ref iPosition, iRadius);
        Blast.BlastVisual(iPlayState, iOwner, iRadius, iPosition, damagePtr[index1].Magnitude, blastScalars, new int?(Blast.GROUND_EFFECT_LOOKUP[index2]), new int?(), iDecal, iDecalAlpha);
      }
    }
    iPlayState.Camera.CameraShake(iPosition, 2f, 0.9f);
    return Blast.BlastDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, iPosition, iRadius, iDamage, false, true);
  }
}
