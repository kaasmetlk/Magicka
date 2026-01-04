// Decompiled with JetBrains decompiler
// Type: Magicka.DRM.HackHelper
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Magicka.Levels.Campaign;
using Magicka.Levels.Packs;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

#nullable disable
namespace Magicka.DRM;

internal static class HackHelper
{
  private static Dictionary<int, string[]> sRobeAchievementDict;
  private static uint[] sMagickAppIDs;
  private static SHA256 sHasher = SHA256.Create();
  private static HackHelper.Status sStatus = HackHelper.Status.Pending;

  static HackHelper()
  {
    HackHelper.sRobeAchievementDict = new Dictionary<int, string[]>(1);
    HackHelper.sRobeAchievementDict.Add("wizard_cultist".GetHashCodeCustom(), new string[1]
    {
      "fhtagnoncemore"
    });
    HackHelper.sMagickAppIDs = new uint[35];
    for (int index = 0; index < HackHelper.sMagickAppIDs.Length; ++index)
      HackHelper.sMagickAppIDs[index] = SteamUtils.GetAppID();
    HackHelper.sMagickAppIDs[12] = 73030U;
    HackHelper.sMagickAppIDs[24] = 42918U;
  }

  public static uint GetAppIDForMagick(MagickType iMagick)
  {
    return iMagick < MagickType.None || iMagick >= (MagickType) HackHelper.sMagickAppIDs.Length ? SteamUtils.GetAppID() : HackHelper.sMagickAppIDs[(int) iMagick];
  }

  public static HackHelper.Status LicenseStatus => HackHelper.sStatus;

  public static HackHelper.License CheckLicense(string iFilename)
  {
    Stream inputStream = (Stream) null;
    try
    {
      inputStream = (Stream) File.OpenRead(iFilename);
      byte[] hash = HackHelper.sHasher.ComputeHash(inputStream);
      inputStream.Close();
      uint oAppID;
      if (!HashTable.GetAppID(hash, out oAppID))
        return HackHelper.License.Custom;
      return !SteamApps.BIsSubscribedApp(oAppID) ? HackHelper.License.No : HackHelper.License.Yes;
    }
    catch
    {
      inputStream?.Close();
    }
    return HackHelper.License.No;
  }

  public static HackHelper.License CheckLicense(LevelNode iLevel)
  {
    if (iLevel.HashSum == null)
      return HackHelper.License.Pending;
    HackHelper.License license = HackHelper.License.Yes;
    uint oAppID;
    if (HashTable.GetAppID(iLevel.HashSum, out oAppID))
    {
      if (!SteamApps.BIsSubscribedApp(oAppID))
        return HackHelper.License.No;
    }
    else
      license = HackHelper.License.Custom;
    for (int index = 0; index < iLevel.Scenes.Length; ++index)
    {
      if (HashTable.GetAppID(iLevel.Scenes[index].ScriptHashSum, out oAppID))
      {
        if (!SteamApps.BIsSubscribedApp(oAppID))
          return HackHelper.License.No;
      }
      else
        license = HackHelper.License.Custom;
      if (HashTable.GetAppID(iLevel.Scenes[index].ModelHashSum, out oAppID))
      {
        if (!SteamApps.BIsSubscribedApp(oAppID))
          return HackHelper.License.No;
      }
      else
        license = HackHelper.License.Custom;
    }
    return license;
  }

  public static HackHelper.License CheckLicense(Profile.PlayableAvatar iAvatar)
  {
    if (iAvatar.HashSum == null)
      return HackHelper.License.Yes;
    HackHelper.License license = HackHelper.License.Yes;
    uint oAppID;
    if (HashTable.GetAppID(iAvatar.HashSum, out oAppID))
    {
      if (!SteamApps.BIsSubscribedApp(oAppID))
        return HackHelper.License.No;
    }
    else
      license = HackHelper.License.Custom;
    switch (HackHelper.CheckLicense($"content/data/characters/{iAvatar.TypeName}.xnb"))
    {
      case HackHelper.License.No:
        return HackHelper.License.No;
      case HackHelper.License.Custom:
        license = HackHelper.License.Custom;
        break;
    }
    if (!HackHelper.sRobeAchievementDict.ContainsKey(iAvatar.Type))
      return license;
    foreach (string iAchievement in HackHelper.sRobeAchievementDict[iAvatar.Type])
    {
      if (!AchievementsManager.Instance.HasAchievement(iAchievement))
        return HackHelper.License.No;
    }
    return HackHelper.License.Yes;
  }

  internal static HackHelper.License CheckLicense(ItemPack iPack)
  {
    SHA256 shA256 = SHA256.Create();
    HackHelper.License license = HackHelper.License.Yes;
    for (int index = 0; index < iPack.Items.Length; ++index)
    {
      FileStream inputStream = (FileStream) null;
      try
      {
        inputStream = File.OpenRead(Path.Combine("content", iPack.Items[index] + ".xnb"));
        byte[] hash = shA256.ComputeHash((Stream) inputStream);
        inputStream.Close();
        uint oAppID;
        if (!HashTable.GetAppID(hash, out oAppID))
          license = HackHelper.License.Custom;
        else if (!SteamApps.BIsSubscribedApp(oAppID))
          return HackHelper.License.No;
      }
      catch
      {
        inputStream?.Close();
        return HackHelper.License.No;
      }
    }
    return license;
  }

  internal static HackHelper.License CheckLicense(MagickPack iPack)
  {
    for (int index = 0; index < iPack.Magicks.Length; ++index)
    {
      if (!SteamApps.BIsSubscribedApp(HackHelper.GetAppIDForMagick(iPack.Magicks[index])))
        return HackHelper.License.No;
    }
    return HackHelper.License.Yes;
  }

  public static void BeginCoreCheck()
  {
    new Thread(new ThreadStart(HackHelper.CoreCheck))
    {
      Name = "Hash Checker"
    }.Start();
  }

  private static void CoreCheck()
  {
    HackHelper.sStatus = HackHelper.Status.Pending;
    for (int index = 0; index < HashTable.CoreFiles.Length; ++index)
    {
      Stream inputStream = (Stream) null;
      try
      {
        inputStream = (Stream) File.OpenRead(HashTable.CoreFiles[index]);
        byte[] hash = HackHelper.sHasher.ComputeHash(inputStream);
        inputStream.Close();
        string oName;
        if (HashTable.GetName(hash, out oName))
        {
          if (Path.GetFileName(HashTable.CoreFiles[index]).Equals(oName, StringComparison.OrdinalIgnoreCase))
            continue;
        }
        HackHelper.sStatus = HackHelper.Status.Hacked;
        return;
      }
      catch (Exception ex)
      {
        inputStream?.Close();
        HackHelper.sStatus = HackHelper.Status.Hacked;
        return;
      }
    }
    HackHelper.sStatus = HackHelper.Status.Valid;
  }

  internal static bool CheckLicenseMythos() => SteamApps.BIsSubscribedApp(73058U);

  internal static bool CheckLicenseVietnam() => SteamApps.BIsSubscribedApp(42918U);

  internal static bool CheckLicenseOSOTC() => SteamApps.BIsSubscribedApp(73093U);

  internal static bool CheckLicenseDungeons1() => SteamApps.BIsSubscribedApp(73115U);

  internal static bool CheckLicenseDungeons2() => SteamApps.BIsSubscribedApp(255980U);

  internal static bool CheckLicenseDungeons3() => false;

  public enum Status
  {
    Pending,
    Valid,
    Hacked,
  }

  public enum License
  {
    Pending = -1, // 0xFFFFFFFF
    No = 0,
    Yes = 1,
    Custom = 2,
  }
}
