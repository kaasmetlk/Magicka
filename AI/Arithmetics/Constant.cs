// Decompiled with JetBrains decompiler
// Type: Magicka.AI.Arithmetics.Constant
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.AI.Arithmetics;

internal class Constant : Expression
{
  public float Value;

  public Constant(float iValue) => this.Value = iValue;

  public override float GetValue(ref ExpressionArguments iArgs) => this.Value;
}
