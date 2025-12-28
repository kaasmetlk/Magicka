using System;
using System.Collections.Generic;
using System.Globalization;

namespace Magicka.AI.Arithmetics
{
	// Token: 0x02000465 RID: 1125
	public abstract class Expression
	{
		// Token: 0x06002248 RID: 8776 RVA: 0x000F54EC File Offset: 0x000F36EC
		static Expression()
		{
			Expression.sPriorities = new Dictionary<char, byte>();
			Expression.sPriorities.Add('^', 200);
			Expression.sPriorities.Add('!', 150);
			Expression.sPriorities.Add('*', 100);
			Expression.sPriorities.Add('/', 100);
			Expression.sPriorities.Add('+', 50);
			Expression.sPriorities.Add('-', 50);
		}

		// Token: 0x06002249 RID: 8777
		public abstract float GetValue(ref ExpressionArguments iArgs);

		// Token: 0x0600224A RID: 8778 RVA: 0x000F5574 File Offset: 0x000F3774
		private static bool IsFunction(string iString, int iExEnd)
		{
			for (int i = 0; i < Expression.sFunctions.Length; i++)
			{
				if (iExEnd >= Expression.sFunctions[i].Length)
				{
					string value = iString.Substring(iExEnd - Expression.sFunctions[i].Length, Expression.sFunctions[i].Length);
					if (Expression.sFunctions[i].Equals(value, StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600224B RID: 8779 RVA: 0x000F55D8 File Offset: 0x000F37D8
		public static Expression Read(string iText)
		{
			List<Expression.Operator> list = new List<Expression.Operator>();
			byte b = 0;
			bool flag = false;
			for (int i = 0; i < iText.Length; i++)
			{
				char c = iText[i];
				byte b2;
				if (c == '(')
				{
					if (flag)
					{
						throw new Exception("Invalid syntax!");
					}
					if (Expression.IsFunction(iText, i))
					{
						flag = true;
					}
					b += 1;
				}
				else if (c == ')')
				{
					flag = false;
					b -= 1;
				}
				else if (!flag && Expression.sPriorities.TryGetValue(c, out b2))
				{
					Expression.Operator item;
					item.Position = i;
					item.Value = c;
					item.Priority = (ushort)((int)b << 8 | (int)b2);
					list.Add(item);
				}
			}
			list.Sort();
			return Expression.Read(iText, list, 0, iText.Length);
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x000F569C File Offset: 0x000F389C
		private static Expression Read(string iText, List<Expression.Operator> iOperators, int iStart, int iCount)
		{
			Expression.Operator @operator = default(Expression.Operator);
			int index = -1;
			for (int i = 0; i < iOperators.Count; i++)
			{
				Expression.Operator operator2 = iOperators[i];
				if (operator2.Position >= iStart & operator2.Position < iStart + iCount)
				{
					@operator = operator2;
					index = i;
					break;
				}
			}
			if (@operator.Value == '\0')
			{
				string text = iText.Substring(iStart, iCount);
				string[] array = text.Replace("(", "").Replace(")", "").Trim().Split(new char[]
				{
					' '
				});
				float iValue;
				if (float.TryParse(array[0], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out iValue))
				{
					return new Constant(iValue);
				}
				return new Variable(text);
			}
			else
			{
				iOperators.RemoveAt(index);
				char value = @operator.Value;
				if (value != '!')
				{
					switch (value)
					{
					case '*':
						return new MulExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
					case '+':
						return new AddExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
					case ',':
					case '.':
						break;
					case '-':
						return new SubExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
					case '/':
						return new DivExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
					default:
						if (value == '^')
						{
							return new PowExpression(Expression.Read(iText, iOperators, iStart, @operator.Position - iStart), Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
						}
						break;
					}
					throw new Exception("Invalid operator \"" + @operator.Value + "\"!");
				}
				return new NotExpression(Expression.Read(iText, iOperators, @operator.Position + 1, iCount - (@operator.Position + 1 - iStart)));
			}
		}

		// Token: 0x0600224D RID: 8781 RVA: 0x000F58F4 File Offset: 0x000F3AF4
		public static int OperatorCount(string iText, int iStart, int iCount)
		{
			int num = 0;
			for (int i = iStart; i < iCount - iStart; i++)
			{
				if (Expression.sPriorities.ContainsKey(iText[i]))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600224E RID: 8782 RVA: 0x000F592C File Offset: 0x000F3B2C
		private static int OperatorPosition(string iText, int iStart, int iCount)
		{
			byte b = byte.MaxValue;
			int result = -1;
			for (int i = iStart; i < iCount - iStart; i++)
			{
				char key = iText[i];
				byte b2;
				if (Expression.sPriorities.TryGetValue(key, out b2) & b2 < b)
				{
					b = b2;
					result = i;
				}
			}
			return result;
		}

		// Token: 0x0400255E RID: 9566
		private static Dictionary<char, byte> sPriorities;

		// Token: 0x0400255F RID: 9567
		private static string[] sFunctions = Enum.GetNames(typeof(Variable.VariableType));

		// Token: 0x02000466 RID: 1126
		private struct Operator : IComparable<Expression.Operator>
		{
			// Token: 0x06002250 RID: 8784 RVA: 0x000F597E File Offset: 0x000F3B7E
			public int CompareTo(Expression.Operator other)
			{
				return (int)(this.Priority - other.Priority);
			}

			// Token: 0x04002560 RID: 9568
			public char Value;

			// Token: 0x04002561 RID: 9569
			public int Position;

			// Token: 0x04002562 RID: 9570
			public ushort Priority;
		}
	}
}
