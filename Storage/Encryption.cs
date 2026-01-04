// Decompiled with JetBrains decompiler
// Type: Magicka.Storage.Encryption
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Storage;

public static class Encryption
{
  public static string Vigenere(string iInput, string iKeyword, bool iDecode)
  {
    int index1 = 0;
    string str = string.Empty;
    for (int index2 = 0; index2 < iInput.Length; ++index2)
    {
      str = !iDecode ? str + (object) (char) ((uint) iInput[index2] + (uint) iKeyword[index1]) : str + (object) (char) ((uint) iInput[index2] - (uint) iKeyword[index1]);
      ++index1;
      if (index1 >= iKeyword.Length)
        index1 = 0;
    }
    return str;
  }
}
