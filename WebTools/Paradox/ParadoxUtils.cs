// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.ParadoxUtils
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Misc;
using System;
using System.Globalization;
using System.IO;
using System.Net.Mail;

#nullable disable
namespace Magicka.WebTools.Paradox;

public static class ParadoxUtils
{
  public const int EMAIL_MAX_LEN = 128 /*0x80*/;
  private const int PASSWORD_MIN_LEN = 5;
  public const int PASSWORD_MAX_LEN = 128 /*0x80*/;
  public const int DATEOFBIRTH_MAX_LEN = 10;
  private const int MINIMUM_AGE = 5;
  private const int MIN_YEAR = 1900;

  public static void EnsureParadoxFolder()
  {
    if (Directory.Exists(ParadoxSettings.PARADOX_CACHE_PATH))
      return;
    Directory.CreateDirectory(ParadoxSettings.PARADOX_CACHE_PATH);
  }

  public static bool IsValidEmail(string iEmail)
  {
    try
    {
      return new MailAddress(iEmail).Address == iEmail;
    }
    catch
    {
      return false;
    }
  }

  public static bool IsValidPassword(string iPassword)
  {
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = iPassword.Length >= 5 && iPassword.Length <= 128 /*0x80*/;
    for (int index = 0; index < iPassword.Length; ++index)
    {
      int num = (int) iPassword[index];
      if (char.IsUpper(iPassword, index))
        flag1 = true;
      if (char.IsNumber(iPassword, index))
        flag2 = true;
    }
    return flag1 && flag2 && flag3;
  }

  public static bool IsValidDoB(string iDateOfBirth)
  {
    DateTime result;
    return DateTime.TryParse(iDateOfBirth, (IFormatProvider) new CultureInfo("en-US"), DateTimeStyles.AdjustToUniversal, out result) && result.Year >= 1900 && result.Year <= DateTime.Now.Year - 5;
  }

  public static void AuthenticateWithGameSparks(
    GameSparksAccount.OperationCompleteDelegate iCallback)
  {
    Singleton<GameSparksAccount>.Instance.Authenticate(Singleton<ParadoxServices>.Instance.RetrieveAccountGuid(), Singleton<ParadoxServices>.Instance.RetrieveAuthToken(), iCallback);
  }

  public static void RegisterWithGameSparks(
    GameSparksAccount.OperationCompleteDelegate iCallback)
  {
    Singleton<GameSparksAccount>.Instance.Register(Singleton<ParadoxServices>.Instance.RetrieveAccountGuid(), Singleton<ParadoxServices>.Instance.RetrieveAuthToken(), iCallback);
  }
}
