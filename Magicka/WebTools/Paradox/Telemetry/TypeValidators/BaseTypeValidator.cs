using System;
using System.Text;

namespace Magicka.WebTools.Paradox.Telemetry.TypeValidators
{
	// Token: 0x0200019C RID: 412
	public class BaseTypeValidator<T> : ITypeValidator
	{
		// Token: 0x06000C43 RID: 3139 RVA: 0x00049FF8 File Offset: 0x000481F8
		public bool MatchType(object iObject)
		{
			bool result = false;
			Type type = iObject.GetType();
			if (type == typeof(T))
			{
				result = this.OnMatchType((T)((object)iObject));
			}
			return result;
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x0004A029 File Offset: 0x00048229
		public string GetFormattedString(object iValue)
		{
			return this.ToString((T)((object)iValue));
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x0004A037 File Offset: 0x00048237
		public Type GetSystemType()
		{
			return typeof(T);
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x0004A044 File Offset: 0x00048244
		protected virtual string ToString(T iValue)
		{
			string text = iValue.ToString();
			if (typeof(T).IsEnum)
			{
				text = BaseTypeValidator<T>.ToLowerUnderscore(text);
			}
			return text;
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x0004A078 File Offset: 0x00048278
		protected virtual bool OnMatchType(T iValue)
		{
			return true;
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x0004A07C File Offset: 0x0004827C
		protected static string ToLowerUnderscore(string iInputString)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(iInputString))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(iInputString[0]);
				for (int i = 1; i < iInputString.Length; i++)
				{
					char c = iInputString[i];
					if (char.IsUpper(c) || char.IsNumber(c))
					{
						stringBuilder.Append('_');
					}
					stringBuilder.Append(c);
				}
				result = stringBuilder.ToString().ToLower();
			}
			return result;
		}
	}
}
