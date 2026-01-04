// Decompiled with JetBrains decompiler
// Type: Magicka.AI.AgentStates.AIStatePanic
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.AI.AgentStates;

internal class AIStatePanic : IAIState
{
  private static AIStatePanic sSingleton;
  private Random mRandom = new Random();

  public static AIStatePanic Instance
  {
    get
    {
      if (AIStatePanic.sSingleton == null)
        AIStatePanic.sSingleton = new AIStatePanic();
      return AIStatePanic.sSingleton;
    }
  }

  public void OnEnter(IAI iOwner)
  {
    iOwner.WanderAngle = (float) ((this.mRandom.NextDouble() - 0.5) * 6.2831854820251465);
  }

  public void OnExit(IAI iOwner) => iOwner.Owner.CharacterBody.Movement = new Vector3();

  public void IncrementEvent(IAI iOwner)
  {
  }

  public void OnExecute(IAI iOwner, float iDeltaTime)
  {
    if (!iOwner.Owner.IsPanicing && !iOwner.Owner.IsStumbling)
    {
      iOwner.PopState();
    }
    else
    {
      iOwner.WanderAngle = MathHelper.WrapAngle(iOwner.WanderAngle + (float) ((this.mRandom.NextDouble() - 0.5) * 60.0) * iDeltaTime);
      Vector3 result1 = new Vector3();
      MathApproximation.FastSinCos(iOwner.WanderAngle, out result1.Z, out result1.X);
      Vector3 result2 = iOwner.Owner.Direction;
      if (iOwner.Owner.CharacterBody.RunBackward)
        Vector3.Multiply(ref result2, -1.25f, out result2);
      else
        Vector3.Multiply(ref result2, 1.25f, out result2);
      Vector3.Add(ref result2, ref result1, out result1);
      result1.Normalize();
      iOwner.Owner.CharacterBody.Movement = result1;
    }
  }
}
