// Decompiled with JetBrains decompiler
// Type: Magicka.AI.Arithmetics.Expression
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Magicka.AI.Arithmetics;

public abstract class Expression
{
  private static Dictionary<char, byte> sPriorities;
  private static string[] sFunctions = Enum.GetNames(typeof (Variable.VariableType));

  static Expression()
  {
    Expression.sPriorities = new Dictionary<char, byte>();
    Expression.sPriorities.Add('^', (byte) 200);
    Expression.sPriorities.Add('!', (byte) 150);
    Expression.sPriorities.Add('*', (byte) 100);
    Expression.sPriorities.Add('/', (byte) 100);
    Expression.sPriorities.Add('+', (byte) 50);
    Expression.sPriorities.Add('-', (byte) 50);
  }

  public abstract float GetValue(ref ExpressionArguments iArgs);

  private static bool IsFunction(string iString, int iExEnd)
  {
    for (int index = 0; index < Expression.sFunctions.Length; ++index)
    {
      if (iExEnd >= Expression.sFunctions[index].Length)
      {
        string str = iString.Substring(iExEnd - Expression.sFunctions[index].Length, Expression.sFunctions[index].Length);
        if (Expression.sFunctions[index].Equals(str, StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
    }
    return false;
  }

  public static Expression Read(string iText)
  {
    List<Expression.Operator> iOperators = new List<Expression.Operator>();
    byte num1 = 0;
    bool flag = false;
    for (int index = 0; index < iText.Length; ++index)
    {
      char key = iText[index];
      switch (key)
      {
        case '(':
          if (flag)
            throw new Exception("Invalid syntax!");
          if (Expression.IsFunction(iText, index))
            flag = true;
          ++num1;
          break;
        case ')':
          flag = false;
          --num1;
          break;
        default:
          byte num2;
          if (!flag && Expression.sPriorities.TryGetValue(key, out num2))
          {
            Expression.Operator @operator;
            @operator.Position = index;
            @operator.Value = key;
            @operator.Priority = (ushort) ((uint) num1 << 8 | (uint) num2);
            iOperators.Add(@operator);
            break;
          }
          break;
      }
    }
    iOperators.Sort();
    return Expression.Read(iText, iOperators, 0, iText.Length);
  }

  private static Expression Read(
    string iText,
    List<Expression.Operator> iOperators,
    int iStart,
    int iCount)
  {
    Expression.Operator @operator = new Expression.Operator();
    int index1 = -1;
    for (int index2 = 0; index2 < iOperators.Count; ++index2)
    {
      Expression.Operator iOperator = iOperators[index2];
      if (iOperator.Position >= iStart & iOperator.Position < iStart + iCount)
      {
        @operator = iOperator;
        index1 = index2;
        break;
      }
    }
    if (@operator.Value == char.MinValue)
    {
      string iText1 = iText.Substring(iStart, iCount);
      float result;
      return float.TryParse(iText1.Replace("(", "").Replace(")", "").Trim().Split(' ')[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out result) ? (Expression) new Constant(result) : (Expression) new Variable(iText1);
    }
    iOperators.RemoveAt(index1);
    switch (@operator.Value)
    {
      case '!':
        return (Expression) new NotExpression(Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
      case '*':
        return (Expression) new MulExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
      case '+':
        return (Expression) new AddExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
      case '-':
        return (Expression) new SubExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
      case '/':
        return (Expression) new DivExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
      case '^':
        return (Expression) new PowExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
      default:
        throw new Exception($"Invalid operator \"{(object) @operator.Value}\"!");
    }
  }

  public static int OperatorCount(string iText, int iStart, int iCount)
  {
    int num = 0;
    for (int index = iStart; index < iCount - iStart; ++index)
    {
      if (Expression.sPriorities.ContainsKey(iText[index]))
        ++num;
    }
    return num;
  }

  private static int OperatorPosition(string iText, int iStart, int iCount)
  {
    byte num1 = byte.MaxValue;
    int num2 = -1;
    for (int index = iStart; index < iCount - iStart; ++index)
    {
      char key = iText[index];
      byte num3;
      if (Expression.sPriorities.TryGetValue(key, out num3) & (int) num3 < (int) num1)
      {
        num1 = num3;
        num2 = index;
      }
    }
    return num2;
  }

  private struct Operator : IComparable<Expression.Operator>
  {
    public char Value;
    public int Position;
    public ushort Priority;

    public int CompareTo(Expression.Operator other) => (int) this.Priority - (int) other.Priority;
  }
}
