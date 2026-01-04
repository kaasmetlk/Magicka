// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.TypeValidators.BaseTypeValidator`1
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Text;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry.TypeValidators;

public class BaseTypeValidator<T> : ITypeValidator
{
  public bool MatchType(object iObject)
  {
    bool flag = false;
    if (iObject.GetType() == typeof (T))
      flag = this.OnMatchType((T) iObject);
    return flag;
  }

  public string GetFormattedString(object iValue) => this.ToString((T) iValue);

  public System.Type GetSystemType() => typeof (T);

  protected virtual string ToString(T iValue)
  {
    string lowerUnderscore = iValue.ToString();
    if (typeof (T).IsEnum)
      lowerUnderscore = BaseTypeValidator<T>.ToLowerUnderscore(lowerUnderscore);
    return lowerUnderscore;
  }

  protected virtual bool OnMatchType(T iValue) => true;

  protected static string ToLowerUnderscore(string iInputString)
  {
    string lowerUnderscore = string.Empty;
    if (!string.IsNullOrEmpty(iInputString))
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(iInputString[0]);
      for (int index = 1; index < iInputString.Length; ++index)
      {
        char c = iInputString[index];
        if (char.IsUpper(c) || char.IsNumber(c))
          stringBuilder.Append('_');
        stringBuilder.Append(c);
      }
      lowerUnderscore = stringBuilder.ToString().ToLower();
    }
    return lowerUnderscore;
  }
}
