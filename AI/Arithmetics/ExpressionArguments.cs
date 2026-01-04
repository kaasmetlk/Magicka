// Decompiled with JetBrains decompiler
// Type: Magicka.AI.Arithmetics.ExpressionArguments
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.AI.Arithmetics;

public struct ExpressionArguments
{
  public Agent AI;
  public IDamageable Target;
  public Vector3 Delta;
  public Vector3 DeltaNormalized;
  public float Distance;
  public Vector3 AIPos;
  public Vector3 TargetPos;
  public Vector3 AIDir;
  public Vector3 TargetDir;

  public static void NewExpressionArguments(
    Agent iAI,
    IDamageable iTarget,
    out ExpressionArguments oExpressionArguments)
  {
    oExpressionArguments.AI = iAI;
    oExpressionArguments.AIPos = iAI.Owner.Position;
    oExpressionArguments.AIDir = iAI.Owner.Direction;
    oExpressionArguments.Target = iTarget;
    oExpressionArguments.TargetPos = iTarget.Position;
    oExpressionArguments.TargetDir = iTarget.Body.Orientation.Forward;
    Vector3.Subtract(ref oExpressionArguments.TargetPos, ref oExpressionArguments.AIPos, out oExpressionArguments.Delta);
    oExpressionArguments.Distance = oExpressionArguments.Delta.Length();
    Vector3.Divide(ref oExpressionArguments.Delta, oExpressionArguments.Distance, out oExpressionArguments.DeltaNormalized);
  }
}
