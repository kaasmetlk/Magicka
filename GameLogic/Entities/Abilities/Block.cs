// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.Block
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public class Block : Ability
{
  private float mArc;
  private int mShield;

  public Block(ContentReader iInput, AnimationClipAction[][] iAnimations)
    : base(iInput, iAnimations)
  {
    this.mArc = iInput.ReadSingle();
    this.mShield = iInput.ReadInt32();
  }

  public Block(Block iCloneSource)
    : base((Ability) iCloneSource)
  {
    this.mArc = iCloneSource.mArc;
    this.mShield = iCloneSource.mShield;
  }

  protected override float Desirability(ref ExpressionArguments iArgs)
  {
    throw new NotImplementedException("Block must define a desirability expression!");
  }

  public override void Update(Agent iAgent, float iDeltaTime)
  {
  }

  public override bool InternalExecute(Agent iAgent)
  {
    iAgent.NPC.Block();
    return true;
  }

  public override float GetMaxRange(Agent iAgent) => float.MaxValue;

  public override float GetMinRange(Agent iAgent) => 0.0f;

  public override float GetArc(Agent iAgent) => this.mArc;

  public override int[] GetWeapons() => throw new NotImplementedException();

  public override Vector3 GetDesiredDirection(Agent iAgent)
  {
    Vector3 position1 = iAgent.Owner.Position;
    Vector3 position2 = iAgent.CurrentTarget.Position;
    Vector3 result;
    Vector3.Subtract(ref position2, ref position1, out result);
    float num = result.Length();
    if ((double) num > 9.9999999747524271E-07)
      Vector3.Divide(ref result, num, out result);
    else
      result.Z = 1f;
    return result;
  }
}
