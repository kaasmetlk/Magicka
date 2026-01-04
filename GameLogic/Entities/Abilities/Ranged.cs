// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.Ranged
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public class Ranged : Ability
{
  private int[] mWeapons;
  private float mMaxRange;
  private float mMinRange;
  private float mElevation;
  private float mArc;
  private float mAccuracy;

  public Ranged(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mMinRange = iInput.ReadSingle();
    this.mMaxRange = iInput.ReadSingle();
    this.mElevation = iInput.ReadSingle();
    this.mArc = iInput.ReadSingle();
    this.mArc = 3.14159274f;
    this.mAccuracy = iInput.ReadSingle();
    this.mWeapons = new int[iInput.ReadInt32()];
    for (int index = 0; index < this.mWeapons.Length; ++index)
      this.mWeapons[index] = iInput.ReadInt32();
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    return FuzzyMath.FuzzyDistanceExponential(iArgs.Distance, this.mMinRange, this.mMaxRange);
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    Segment iSeg;
    iSeg.Origin = iAgent.Owner.Position;
    Vector3 position = iAgent.CurrentTarget.Position;
    Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
    iAgent.Owner.CharacterBody.DesiredDirection = iSeg.Delta;
    Vector3.Multiply(ref iSeg.Delta, (float) Math.Cos((double) this.mElevation), out iSeg.Delta);
    Vector3 point;
    iSeg.GetPoint(0.5f, out point);
    List<Entity> entities = iAgent.Owner.PlayState.EntityManager.GetEntities(point, this.mMaxRange, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Character character && character != iAgent.Owner && (character.Faction & iAgent.Owner.Faction) != Factions.NONE && character.SegmentIntersect(out Vector3 _, iSeg, 0.75f))
      {
        iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
        return false;
      }
    }
    iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
    for (int index = 0; index < this.mWeapons.Length; ++index)
      iAgent.Owner.Equipment[this.mWeapons[index]].Item.PrepareToExecute((Ability) this);
    iAgent.Owner.Attack(this.mAnimationKeys[MagickaMath.Random.Next(this.mAnimationKeys.Length)], true);
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => this.mMaxRange;

  public override float GetMinRange(Agent iAgent) => this.mMinRange;

  public float GetElevation() => this.mElevation;

  public float GetAccuracy() => this.mAccuracy;

  public override float GetArc(Agent iAgent) => this.mArc;

  public override int[] GetWeapons() => this.mWeapons;

  public override Vector3 GetDesiredDirection(Agent iAgent)
  {
    Vector3 position1 = iAgent.Owner.Position;
    Vector3 position2 = iAgent.CurrentTarget.Position;
    Vector3 result;
    Vector3.Subtract(ref position2, ref position1, out result);
    float num = result.Length();
    if ((double) num <= 1.4012984643248171E-45)
      return Vector3.Forward;
    Vector3.Divide(ref result, num, out result);
    return result;
  }

  public override bool ChooseAttackAngle(Agent iAgent, out Vector3 oDirection)
  {
    Vector3 position1 = iAgent.Owner.Position;
    Vector3 position2 = iAgent.CurrentTarget.Position;
    float num = (float) (((double) this.GetMinRange(iAgent) + (double) this.GetMaxRange(iAgent)) * 0.5);
    Vector3 result1;
    Vector3.Subtract(ref position1, ref position2, out result1);
    result1.Y = 0.0f;
    if ((double) result1.LengthSquared() <= 9.9999999747524271E-07)
    {
      oDirection = new Vector3();
      return false;
    }
    result1.Normalize();
    Vector3.Multiply(ref result1, num, out result1);
    Segment iSeg1;
    iSeg1.Origin = position2;
    GameScene currentScene = iAgent.Owner.PlayState.Level.CurrentScene;
    List<Entity> entities = iAgent.Owner.PlayState.EntityManager.GetEntities(position2, num, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (!(entities[index] is Character character) || (character.Faction & iAgent.Owner.Faction) == Factions.NONE || !iAgent.AttackedBy.ContainsKey(character.Handle))
      {
        entities.RemoveAt(index);
        --index;
      }
    }
    for (int index1 = 0; index1 < 3; ++index1)
    {
      Quaternion result2;
      Quaternion.CreateFromYawPitchRoll((float) (Ability.sRandom.NextDouble() * 3.1415927410125732 - 1.5707963705062866), 0.0f, 0.0f, out result2);
      Vector3.Transform(ref result1, ref result2, out oDirection);
      Vector3.Multiply(ref oDirection, (float) ((1.0 - (double) this.mElevation / 1.5707963705062866) * 0.5), out iSeg1.Delta);
      Segment iSeg2;
      Vector3.Add(ref iSeg1.Origin, ref oDirection, out iSeg2.Origin);
      Vector3.Multiply(ref oDirection, (float) ((1.0 - (double) this.mElevation / 1.5707963705062866) * -0.5), out iSeg2.Delta);
      float oFrac;
      Vector3 vector3;
      Vector3 oNrm;
      if (!currentScene.SegmentIntersect(out oFrac, out vector3, out oNrm, iSeg1) && !currentScene.SegmentIntersect(out oFrac, out vector3, out oNrm, iSeg2))
      {
        int index2 = 0;
        while (index2 < entities.Count && !(entities[index2] as Character).SegmentIntersect(out vector3, iSeg1, 0.5f) && !(entities[index2] as Character).SegmentIntersect(out vector3, iSeg2, 0.5f))
          ++index2;
        iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
        return true;
      }
    }
    oDirection = new Vector3();
    iAgent.Owner.PlayState.EntityManager.ReturnEntityList(entities);
    return false;
  }
}
