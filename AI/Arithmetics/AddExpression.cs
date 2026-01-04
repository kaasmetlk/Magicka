// Decompiled with JetBrains decompiler
// Type: Magicka.AI.Arithmetics.AddExpression
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.AI.Arithmetics;

internal class AddExpression : Expression
{
  public Expression mChildA;
  public Expression mChildB;

  public AddExpression(Expression iChildA, Expression iChildB)
  {
    this.mChildA = iChildA;
    this.mChildB = iChildB;
  }

  public override float GetValue(ref ExpressionArguments iArgs)
  {
    return this.mChildA.GetValue(ref iArgs) + this.mChildB.GetValue(ref iArgs);
  }
}
