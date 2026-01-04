// Decompiled with JetBrains decompiler
// Type: Magicka.AI.Arithmetics.Variable
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Magicka.AI.Arithmetics;

internal class Variable : Expression
{
  public Variable.VariableScope Scope;
  public Variable.VariableType Type;
  public float ArgumentSingle1;
  public float ArgumentSingle2;
  public float ArgumentSingle3;
  public float ArgumentSingle4;
  public StatusEffects ArgumentStatus;
  public Elements ArgumentElement;

  public Variable(string iText)
  {
    string[] strArray1 = iText.Replace(" ", "").Split(new char[4]
    {
      ',',
      '/',
      '(',
      ')'
    }, StringSplitOptions.RemoveEmptyEntries);
    int index1 = 0;
    try
    {
      string[] strArray2 = strArray1[index1].Split('.');
      this.Scope = (Variable.VariableScope) Enum.Parse(typeof (Variable.VariableScope), strArray2[0], true);
      string[] strArray3 = new string[strArray1.Length + 1];
      strArray3[0] = strArray2[0];
      strArray3[1] = strArray2[1];
      for (int index2 = 1; index2 < strArray1.Length; ++index2)
        strArray3[index2 + 1] = strArray1[index2];
      strArray1 = strArray3;
      ++index1;
    }
    catch
    {
      this.Scope = Variable.VariableScope.Global;
    }
    System.Type enumType1 = typeof (Variable.VariableType);
    string[] strArray4 = strArray1;
    int index3 = index1;
    int index4 = index3 + 1;
    string str1 = strArray4[index3];
    this.Type = (Variable.VariableType) Enum.Parse(enumType1, str1, true);
    int num1;
    switch (this.Type)
    {
      case Variable.VariableType.DistanceLinear:
      case Variable.VariableType.DistanceExponential:
        string[] strArray5 = strArray1;
        int index5 = index4;
        int num2 = index5 + 1;
        this.ArgumentSingle2 = float.Parse(strArray5[index5], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        if (num2 < strArray1.Length)
        {
          this.ArgumentSingle1 = this.ArgumentSingle2;
          string[] strArray6 = strArray1;
          int index6 = num2;
          num1 = index6 + 1;
          this.ArgumentSingle2 = float.Parse(strArray6[index6], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          break;
        }
        this.ArgumentSingle1 = -this.ArgumentSingle2;
        break;
      case Variable.VariableType.Danger:
      case Variable.VariableType.Threat:
        strArray1[index4] = strArray1[index4].Replace('|', ',');
        System.Type enumType2 = typeof (Elements);
        string[] strArray7 = strArray1;
        int index7 = index4;
        num1 = index7 + 1;
        string str2 = strArray7[index7];
        this.ArgumentElement = (Elements) Enum.Parse(enumType2, str2, true);
        break;
      case Variable.VariableType.HasStatus:
        System.Type enumType3 = typeof (StatusEffects);
        string[] strArray8 = strArray1;
        int index8 = index4;
        num1 = index8 + 1;
        string str3 = strArray8[index8];
        this.ArgumentStatus = (StatusEffects) Enum.Parse(enumType3, str3, true);
        break;
      case Variable.VariableType.FriendlyDensity:
      case Variable.VariableType.EnemyDensity:
        if (index4 < strArray1.Length)
        {
          string[] strArray9 = strArray1;
          int index9 = index4;
          num1 = index9 + 1;
          this.ArgumentSingle1 = float.Parse(strArray9[index9], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          break;
        }
        this.ArgumentSingle1 = 6f;
        break;
      case Variable.VariableType.LOSC:
        this.ArgumentSingle1 = 0.0f;
        this.ArgumentSingle2 = 0.0f;
        this.ArgumentSingle3 = 0.0f;
        this.ArgumentSingle4 = -1f;
        if (strArray1.Length <= 1)
          break;
        string[] strArray10 = strArray1;
        int index10 = index4;
        int num3 = index10 + 1;
        this.ArgumentSingle1 = float.Parse(strArray10[index10], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        if (strArray1.Length <= 2)
          break;
        string[] strArray11 = strArray1;
        int index11 = num3;
        int num4 = index11 + 1;
        this.ArgumentSingle2 = float.Parse(strArray11[index11], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        if (strArray1.Length <= 3)
          break;
        string[] strArray12 = strArray1;
        int index12 = num4;
        int num5 = index12 + 1;
        this.ArgumentSingle3 = float.Parse(strArray12[index12], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        if (strArray1.Length <= 4)
          break;
        string[] strArray13 = strArray1;
        int index13 = num5;
        num1 = index13 + 1;
        this.ArgumentSingle4 = float.Parse(strArray13[index13], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        break;
    }
  }

  public override float GetValue(ref ExpressionArguments iArgs)
  {
    bool flag1 = false;
    switch (this.Type)
    {
      case Variable.VariableType.DistanceLinear:
        return FuzzyMath.FuzzyDistanceLinear(iArgs.Distance, this.ArgumentSingle1, this.ArgumentSingle2);
      case Variable.VariableType.DistanceExponential:
        return FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.ArgumentSingle1, this.ArgumentSingle2);
      case Variable.VariableType.Behind:
        return FuzzyMath.FuzzyAngle(ref iArgs.DeltaNormalized, ref iArgs.TargetDir);
      case Variable.VariableType.Danger:
        return FuzzyMath.FuzzyDanger(iArgs.AI.NPC, this.ArgumentElement);
      case Variable.VariableType.HasStatus:
        if (this.Scope == Variable.VariableScope.Self)
        {
          flag1 = iArgs.AI.Owner.HasStatus(this.ArgumentStatus);
          break;
        }
        if (this.Scope != Variable.VariableScope.Target)
          throw new Exception();
        flag1 = iArgs.Target is IStatusEffected target1 && target1.HasStatus(this.ArgumentStatus);
        break;
      case Variable.VariableType.Gripped:
        if (this.Scope == Variable.VariableScope.Self)
        {
          flag1 = iArgs.AI.Owner.IsGripped;
          break;
        }
        if (this.Scope != Variable.VariableScope.Target)
          throw new Exception();
        flag1 = iArgs.Target is Character target2 && target2.IsGripped;
        break;
      case Variable.VariableType.Gripping:
        if (this.Scope == Variable.VariableScope.Self)
        {
          flag1 = iArgs.AI.Owner.IsGripping;
          break;
        }
        if (this.Scope != Variable.VariableScope.Target)
          throw new Exception();
        flag1 = iArgs.Target is Character target3 && target3.IsGripping;
        break;
      case Variable.VariableType.Entangled:
        if (this.Scope == Variable.VariableScope.Self)
        {
          flag1 = iArgs.AI.Owner.IsEntangled;
          break;
        }
        if (this.Scope != Variable.VariableScope.Target)
          throw new Exception();
        flag1 = iArgs.Target is Character target4 && target4.IsEntangled;
        break;
      case Variable.VariableType.Speed:
        if (this.Scope == Variable.VariableScope.Self)
          return iArgs.AI.Owner.CharacterBody.Movement.Length();
        if (this.Scope != Variable.VariableScope.Target)
          throw new Exception();
        return !(iArgs.Target is Character target5) ? 0.0f : target5.CharacterBody.Movement.Length();
      case Variable.VariableType.Health:
        return this.Scope == Variable.VariableScope.Self ? iArgs.AI.Owner.HitPoints / iArgs.AI.Owner.MaxHitPoints : iArgs.Target.HitPoints / iArgs.Target.MaxHitPoints;
      case Variable.VariableType.FriendlyDensity:
        return FuzzyMath.FuzzyFriendlyDensity(this.Scope == Variable.VariableScope.Self ? (Entity) iArgs.AI.Owner : iArgs.Target as Entity, iArgs.AI.Owner.Faction, this.ArgumentSingle1);
      case Variable.VariableType.EnemyDensity:
        return FuzzyMath.FuzzyEnemyDensity(this.Scope == Variable.VariableScope.Self ? (Entity) iArgs.AI.Owner : iArgs.Target as Entity, iArgs.AI.Owner.Faction, this.ArgumentSingle1);
      case Variable.VariableType.Threat:
        if (this.Scope != Variable.VariableScope.Target)
          throw new Exception("Invalid scope!");
        return iArgs.Target is Character target6 ? FuzzyMath.FuzzyThreat(iArgs.AI.NPC, target6, this.ArgumentElement) : 0.0f;
      case Variable.VariableType.LOS:
        Segment segment = new Segment();
        segment.Origin = iArgs.AIPos;
        segment.Delta = iArgs.Delta;
        Vector3 point;
        segment.GetPoint(0.5f, out point);
        List<Entity> entities = iArgs.AI.Owner.PlayState.EntityManager.GetEntities(point, iArgs.Distance * 0.5f, false);
        bool flag2 = false;
        for (int index = 0; index < entities.Count && !flag2; ++index)
        {
          Entity entity = entities[index];
          if (entity != iArgs.AI.Owner && entity != iArgs.Target)
          {
            Vector3 vector3;
            flag2 = !(entity is IDamageable) ? entity.Body.CollisionSkin.SegmentIntersect(out float _, out vector3, out Vector3 _, segment) : (entity as IDamageable).SegmentIntersect(out vector3, segment, iArgs.AI.Owner.Radius);
          }
        }
        iArgs.AI.Owner.PlayState.EntityManager.ReturnEntityList(entities);
        return !flag2 ? 1f : 0.0f;
      case Variable.VariableType.IsCharacter:
        return this.Scope == Variable.VariableScope.Target ? (!(iArgs.Target is Character) ? 0.0f : 1f) : (this.Scope == Variable.VariableScope.Self ? 1f : 0.0f);
      case Variable.VariableType.IsEthereal:
        return this.Scope == Variable.VariableScope.Target ? (!(iArgs.Target is Character) || !(iArgs.Target as Character).IsEthereal ? 0.0f : 1f) : (iArgs.AI.Owner == null || !iArgs.AI.Owner.IsEthereal ? 0.0f : 1f);
      case Variable.VariableType.Resistance:
        if (this.Scope == Variable.VariableScope.Target)
        {
          float num = 0.0f;
          if (iArgs.Target != null)
            num = 1f - iArgs.Target.ResistanceAgainst(this.ArgumentElement);
          return num;
        }
        float num1 = 0.0f;
        if (iArgs.AI.Owner != null)
          num1 = 1f - iArgs.AI.Owner.ResistanceAgainst(this.ArgumentElement);
        return num1;
      case Variable.VariableType.Shielded:
        if (this.Scope == Variable.VariableScope.Target)
          return !(iArgs.Target is Character) || !(iArgs.Target as Character).IsSelfShielded ? 0.0f : 1f;
        if (this.Scope == Variable.VariableScope.Self)
          return iArgs.AI.Owner == null || !iArgs.AI.Owner.IsSelfShielded ? 0.0f : 1f;
        if (this.Scope != Variable.VariableScope.Global)
          break;
        break;
      case Variable.VariableType.LOSC:
        Segment iSeg = new Segment();
        iSeg.Origin = iArgs.AIPos;
        iSeg.Origin.X += iArgs.AIDir.X * this.ArgumentSingle1;
        iSeg.Origin.Y += this.ArgumentSingle2;
        iSeg.Origin.Z += iArgs.AIDir.Z * this.ArgumentSingle3;
        if ((double) this.ArgumentSingle4 <= 0.0)
        {
          iSeg.Delta = iArgs.Delta;
        }
        else
        {
          float num2 = (double) this.ArgumentSingle4 > (double) iArgs.Distance ? iArgs.Distance : this.ArgumentSingle4;
          iSeg.Delta.X = iArgs.DeltaNormalized.X * num2;
          iSeg.Delta.Y = iArgs.DeltaNormalized.Y * num2;
          iSeg.Delta.Z = iArgs.DeltaNormalized.Z * num2;
        }
        return iArgs.AI.Owner.PlayState.Level.CurrentScene.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, iSeg) ? 0.0f : 1f;
      case Variable.VariableType.IsShield:
        return this.Scope == Variable.VariableScope.Target && iArgs.Target is Shield ? 1f : 0.0f;
      case Variable.VariableType.IsBarrier:
        return this.Scope == Variable.VariableScope.Target && iArgs.Target is Barrier ? 1f : 0.0f;
      case Variable.VariableType.GripDamageAccumulation:
        return iArgs.AI.Owner.IsGripping ? iArgs.AI.Owner.GripDamageAccumulation : 0.0f;
      case Variable.VariableType.GripDamageAccumulationNormalized:
        return iArgs.AI.Owner.IsGripping ? iArgs.AI.Owner.GripDamageAccumulation / iArgs.AI.Owner.HitTolerance : 0.0f;
      case Variable.VariableType.IsOnGround:
        Character character = (Character) null;
        if (this.Scope == Variable.VariableScope.Target)
          character = iArgs.Target as Character;
        else if (this.Scope == Variable.VariableScope.Self)
          character = iArgs.AI.Owner;
        return character != null && character.CharacterBody.IsTouchingSolidGround ? 1f : 0.0f;
      case Variable.VariableType.Angle:
        Vector3 delta = iArgs.Delta;
        Vector3 aiDir = iArgs.AIDir;
        delta.Y = 0.0f;
        aiDir.Y = 0.0f;
        float result = 0.0f;
        float num3 = 1E-06f;
        if ((double) delta.LengthSquared() > (double) num3 && (double) aiDir.LengthSquared() > (double) num3)
        {
          delta.Normalize();
          aiDir.Normalize();
          Vector3.Dot(ref delta, ref aiDir, out result);
        }
        return result;
      default:
        return 0.0f;
    }
    return !flag1 ? 0.0f : 1f;
  }

  public enum VariableType
  {
    DistLin = 0,
    DistanceLinear = 0,
    DistExp = 1,
    DistanceExponential = 1,
    Behind = 2,
    Danger = 3,
    HasStatus = 4,
    Status = 4,
    Gripped = 5,
    Gripping = 6,
    Entangled = 7,
    Speed = 8,
    Health = 9,
    FDensity = 10, // 0x0000000A
    FriendlyDensity = 10, // 0x0000000A
    EDensity = 11, // 0x0000000B
    EnemyDensity = 11, // 0x0000000B
    Threat = 12, // 0x0000000C
    LOS = 13, // 0x0000000D
    LineOfSight = 13, // 0x0000000D
    Character = 14, // 0x0000000E
    IsC = 14, // 0x0000000E
    IsCharacter = 14, // 0x0000000E
    Ethereal = 15, // 0x0000000F
    IsEthereal = 15, // 0x0000000F
    Res = 16, // 0x00000010
    Resistance = 16, // 0x00000010
    Shielded = 17, // 0x00000011
    LOSC = 18, // 0x00000012
    LineOfSightCollision = 18, // 0x00000012
    IsS = 19, // 0x00000013
    IsShield = 19, // 0x00000013
    Shield = 19, // 0x00000013
    Barrier = 20, // 0x00000014
    IsB = 20, // 0x00000014
    IsBarrier = 20, // 0x00000014
    GripDamageAccumulation = 21, // 0x00000015
    GripDmgAcc = 21, // 0x00000015
    GripDamageAccumulationNormalized = 22, // 0x00000016
    GripDmgAccNrm = 22, // 0x00000016
    IsOnGround = 23, // 0x00000017
    Angle = 24, // 0x00000018
  }

  public enum VariableScope
  {
    G = 0,
    Global = 0,
    S = 1,
    Self = 1,
    T = 2,
    Target = 2,
  }
}
