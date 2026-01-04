// Decompiled with JetBrains decompiler
// Type: Magicka.AI.Arithmetics.NotExpression
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.AI.Arithmetics;

internal class NotExpression : Expression
{
  public Expression mChild;

  public NotExpression(Expression iChild) => this.mChild = iChild;

  public override float GetValue(ref ExpressionArguments iArgs)
  {
    return 1f - this.mChild.GetValue(ref iArgs);
  }
}
