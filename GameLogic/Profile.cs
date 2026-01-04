// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Profile
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.DRM;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Gamers;
using Magicka.Storage;
using Microsoft.Xna.Framework.Graphics;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace Magicka.GameLogic;

public class Profile
{
  public const string SAVE_PATH = "SaveData/Profile.sav";
  private static Profile sSingelton;
  private static volatile object sSingeltonLock = new object();
  private SortedList<string, Gamer> mGamers = new SortedList<string, Gamer>();
  private Dictionary<string, bool> mFoundMoose;
  private Dictionary<string, bool> mFoundSecretAreas;
  private Dictionary<string, bool> mKilledPony;
  private Dictionary<string, Elements> mPlayerUsedElements;
  private float mTotalAmountHealed;
  private int mNumberOfOverKills;
  private int mNumberOfFrozenOverKills;
  private int mLedFarmerKills;
  private int mBanisherOfHorrors;
  private Gamer mLastGamer;
  private static readonly List<int> mMythosCreatures = new List<int>();
  private int mConsectuiveCruiseCount;
  private DateTime mLastCruiseDate;
  private SortedList<string, Profile.PlayableAvatar> mPlayables = new SortedList<string, Profile.PlayableAvatar>();

  public static Profile Instance
  {
    get
    {
      if (Profile.sSingelton == null)
      {
        lock (Profile.sSingeltonLock)
        {
          if (Profile.sSingelton == null)
            Profile.sSingelton = new Profile();
        }
      }
      return Profile.sSingelton;
    }
  }

  static Profile()
  {
    Profile.mMythosCreatures.Add("deep_one".GetHashCodeCustom());
    Profile.mMythosCreatures.Add("elder_thing".GetHashCodeCustom());
    Profile.mMythosCreatures.Add("enderman".GetHashCodeCustom());
    Profile.mMythosCreatures.Add("shoggoth".GetHashCodeCustom());
    Profile.mMythosCreatures.Add("starspawn".GetHashCodeCustom());
  }

  private Profile()
  {
    this.mGamers = new SortedList<string, Gamer>();
    this.mFoundMoose = new Dictionary<string, bool>();
    this.mFoundSecretAreas = new Dictionary<string, bool>();
    this.mKilledPony = new Dictionary<string, bool>();
    this.mPlayerUsedElements = new Dictionary<string, Elements>();
  }

  public Profile.PlayableAvatar DefaultAvatar
  {
    get
    {
      lock (this.mPlayables)
      {
        Profile.PlayableAvatar defaultAvatar;
        if (!this.mPlayables.TryGetValue("wizard", out defaultAvatar))
        {
          defaultAvatar = Profile.PlayableAvatar.CreateDefault();
          this.mPlayables.Add(defaultAvatar.Name, defaultAvatar);
        }
        return defaultAvatar;
      }
    }
  }

  public void Read()
  {
    lock (this.mPlayables)
    {
      foreach (FileSystemInfo file in new DirectoryInfo("content/Data/Characters/Playable").GetFiles("*.xml"))
      {
        Profile.PlayableAvatar playableAvatar = new Profile.PlayableAvatar(file.FullName);
        if (!this.mPlayables.ContainsKey(playableAvatar.Name))
          this.mPlayables.Add(playableAvatar.Name, playableAvatar);
      }
    }
    BinaryReader iReader = (BinaryReader) null;
    try
    {
      iReader = new BinaryReader((Stream) File.OpenRead("SaveData/Profile.sav"));
      string[] strArray = iReader.ReadString().Split('.');
      ushort[] numArray = new ushort[4];
      for (int index = 0; index < strArray.Length; ++index)
        numArray[index] = ushort.Parse(strArray[index]);
      ulong num = (ulong) ((long) numArray[0] << 48 /*0x30*/ | (long) numArray[1] << 32 /*0x20*/ | (long) numArray[2] << 16 /*0x10*/) | (ulong) numArray[3];
      if (num >= 281492157038593UL /*0x01000400070001*/)
        this.Read1471(iReader);
      else if (num >= 281492156973056UL /*0x01000400060000*/)
        this.Read1460(iReader);
      else if (num >= 281492156710912UL /*0x01000400020000*/)
        this.Read1420(iReader);
      else if (num >= 281492156579843UL /*0x01000400000003*/)
        this.Read1403(iReader);
      else if (num >= 281487861940229UL /*0x01000300050005*/)
        this.Read1355(iReader);
      else if (num >= 281483567235072UL /*0x01000200090000*/)
        this.Read1290(iReader);
      iReader.Close();
    }
    catch
    {
      iReader?.Close();
    }
    if (this.mLastGamer != null)
      return;
    string personaName = SteamFriends.GetPersonaName();
    if (this.mGamers.TryGetValue(personaName, out this.mLastGamer))
      return;
    this.mLastGamer = new Gamer(personaName);
    this.Add(this.mLastGamer);
  }

  public void Read1290(BinaryReader iReader)
  {
    int num1 = iReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      Gamer gamer = Gamer.Read(iReader);
      this.mGamers[gamer.GamerTag] = gamer;
    }
    int num2 = iReader.ReadInt32();
    for (int index = 0; index < num2; ++index)
      this.mFoundMoose.Add(iReader.ReadString(), iReader.ReadBoolean());
    int num3 = iReader.ReadInt32();
    for (int index = 0; index < num3; ++index)
      this.mPlayerUsedElements.Add(iReader.ReadString(), (Elements) iReader.ReadInt32());
    int num4 = iReader.ReadInt32();
    for (int index = 0; index < num4; ++index)
      this.mFoundSecretAreas.Add(iReader.ReadString(), iReader.ReadBoolean());
    this.mTotalAmountHealed = iReader.ReadSingle();
    this.mNumberOfOverKills = iReader.ReadInt32();
    this.mGamers.TryGetValue(iReader.ReadString(), out this.mLastGamer);
  }

  public void Read1355(BinaryReader iReader)
  {
    int num1 = iReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      Gamer gamer = Gamer.Read(iReader);
      this.mGamers[gamer.GamerTag] = gamer;
    }
    int num2 = iReader.ReadInt32();
    for (int index = 0; index < num2; ++index)
      this.mFoundMoose.Add(iReader.ReadString(), iReader.ReadBoolean());
    int num3 = iReader.ReadInt32();
    for (int index = 0; index < num3; ++index)
      this.mPlayerUsedElements.Add(iReader.ReadString(), (Elements) iReader.ReadInt32());
    int num4 = iReader.ReadInt32();
    for (int index = 0; index < num4; ++index)
      this.mFoundSecretAreas.Add(iReader.ReadString(), iReader.ReadBoolean());
    this.mTotalAmountHealed = iReader.ReadSingle();
    this.mNumberOfOverKills = iReader.ReadInt32();
    this.mLedFarmerKills = iReader.ReadInt32();
    this.mGamers.TryGetValue(iReader.ReadString(), out this.mLastGamer);
  }

  public void Read1403(BinaryReader iReader)
  {
    int num1 = iReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      Gamer gamer = Gamer.Read(iReader);
      this.mGamers[gamer.GamerTag] = gamer;
    }
    int num2 = iReader.ReadInt32();
    for (int index = 0; index < num2; ++index)
      this.mFoundMoose.Add(iReader.ReadString(), iReader.ReadBoolean());
    int num3 = iReader.ReadInt32();
    for (int index = 0; index < num3; ++index)
      this.mPlayerUsedElements.Add(iReader.ReadString(), (Elements) iReader.ReadInt32());
    int num4 = iReader.ReadInt32();
    for (int index = 0; index < num4; ++index)
      this.mFoundSecretAreas.Add(iReader.ReadString(), iReader.ReadBoolean());
    this.mTotalAmountHealed = iReader.ReadSingle();
    this.mNumberOfOverKills = iReader.ReadInt32();
    this.mLedFarmerKills = iReader.ReadInt32();
    this.mGamers.TryGetValue(iReader.ReadString(), out this.mLastGamer);
    this.mConsectuiveCruiseCount = iReader.ReadInt32();
    this.mLastCruiseDate = DateTime.FromBinary(iReader.ReadInt64());
  }

  public void Read1420(BinaryReader iReader)
  {
    int num1 = iReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      Gamer gamer = Gamer.Read(iReader);
      this.mGamers[gamer.GamerTag] = gamer;
    }
    int num2 = iReader.ReadInt32();
    for (int index = 0; index < num2; ++index)
      this.mFoundMoose.Add(iReader.ReadString(), iReader.ReadBoolean());
    int num3 = iReader.ReadInt32();
    for (int index = 0; index < num3; ++index)
      this.mPlayerUsedElements.Add(iReader.ReadString(), (Elements) iReader.ReadInt32());
    int num4 = iReader.ReadInt32();
    for (int index = 0; index < num4; ++index)
      this.mFoundSecretAreas.Add(iReader.ReadString(), iReader.ReadBoolean());
    this.mTotalAmountHealed = iReader.ReadSingle();
    this.mNumberOfOverKills = iReader.ReadInt32();
    this.mLedFarmerKills = iReader.ReadInt32();
    this.mGamers.TryGetValue(iReader.ReadString(), out this.mLastGamer);
    this.mConsectuiveCruiseCount = iReader.ReadInt32();
    this.mLastCruiseDate = DateTime.FromBinary(iReader.ReadInt64());
    this.mBanisherOfHorrors = iReader.ReadInt32();
  }

  public void Read1460(BinaryReader iReader)
  {
    int num1 = iReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      Gamer gamer = Gamer.Read(iReader);
      this.mGamers[gamer.GamerTag] = gamer;
    }
    int num2 = iReader.ReadInt32();
    for (int index = 0; index < num2; ++index)
      this.mFoundMoose.Add(iReader.ReadString(), iReader.ReadBoolean());
    int num3 = iReader.ReadInt32();
    for (int index = 0; index < num3; ++index)
      this.mPlayerUsedElements.Add(iReader.ReadString(), (Elements) iReader.ReadInt32());
    int num4 = iReader.ReadInt32();
    for (int index = 0; index < num4; ++index)
      this.mFoundSecretAreas.Add(iReader.ReadString(), iReader.ReadBoolean());
    this.mTotalAmountHealed = iReader.ReadSingle();
    this.mNumberOfOverKills = iReader.ReadInt32();
    this.mNumberOfFrozenOverKills = iReader.ReadInt32();
    this.mLedFarmerKills = iReader.ReadInt32();
    this.mGamers.TryGetValue(iReader.ReadString(), out this.mLastGamer);
    this.mConsectuiveCruiseCount = iReader.ReadInt32();
    this.mLastCruiseDate = DateTime.FromBinary(iReader.ReadInt64());
    this.mBanisherOfHorrors = iReader.ReadInt32();
  }

  public void Read1471(BinaryReader iReader)
  {
    int num1 = iReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      Gamer gamer = Gamer.Read(iReader);
      this.mGamers[gamer.GamerTag] = gamer;
    }
    int num2 = iReader.ReadInt32();
    for (int index = 0; index < num2; ++index)
      this.mFoundMoose.Add(iReader.ReadString(), iReader.ReadBoolean());
    int num3 = iReader.ReadInt32();
    for (int index = 0; index < num3; ++index)
      this.mPlayerUsedElements.Add(iReader.ReadString(), (Elements) iReader.ReadInt32());
    int num4 = iReader.ReadInt32();
    for (int index = 0; index < num4; ++index)
      this.mFoundSecretAreas.Add(iReader.ReadString(), iReader.ReadBoolean());
    this.mTotalAmountHealed = iReader.ReadSingle();
    this.mNumberOfOverKills = iReader.ReadInt32();
    this.mNumberOfFrozenOverKills = iReader.ReadInt32();
    this.mLedFarmerKills = iReader.ReadInt32();
    this.mGamers.TryGetValue(iReader.ReadString(), out this.mLastGamer);
    this.mConsectuiveCruiseCount = iReader.ReadInt32();
    this.mLastCruiseDate = DateTime.FromBinary(iReader.ReadInt64());
    this.mBanisherOfHorrors = iReader.ReadInt32();
    int num5 = iReader.ReadInt32();
    for (int index = 0; index < num5; ++index)
      this.mKilledPony.Add(iReader.ReadString(), iReader.ReadBoolean());
  }

  public void Write()
  {
    BinaryWriter iWriter = (BinaryWriter) null;
    try
    {
      iWriter = new BinaryWriter((Stream) File.Create("SaveData/Profile.sav"));
      iWriter.Write(Application.ProductVersion);
      iWriter.Write(this.mGamers.Count);
      foreach (Gamer gamer in (IEnumerable<Gamer>) this.mGamers.Values)
        gamer.Write(iWriter);
      iWriter.Write(this.mFoundMoose.Count);
      for (int index = 0; index < this.mFoundMoose.Count; ++index)
      {
        iWriter.Write(this.mFoundMoose.ElementAt<KeyValuePair<string, bool>>(index).Key);
        iWriter.Write(this.mFoundMoose.ElementAt<KeyValuePair<string, bool>>(index).Value);
      }
      iWriter.Write(this.mPlayerUsedElements.Count);
      for (int index = 0; index < this.mPlayerUsedElements.Count; ++index)
      {
        iWriter.Write(this.mPlayerUsedElements.ElementAt<KeyValuePair<string, Elements>>(index).Key);
        iWriter.Write((int) this.mPlayerUsedElements.ElementAt<KeyValuePair<string, Elements>>(index).Value);
      }
      iWriter.Write(this.mFoundSecretAreas.Count);
      for (int index = 0; index < this.mFoundSecretAreas.Count; ++index)
      {
        iWriter.Write(this.mFoundSecretAreas.ElementAt<KeyValuePair<string, bool>>(index).Key);
        iWriter.Write(this.mFoundSecretAreas.ElementAt<KeyValuePair<string, bool>>(index).Value);
      }
      iWriter.Write(this.mTotalAmountHealed);
      iWriter.Write(this.mNumberOfOverKills);
      iWriter.Write(this.mNumberOfFrozenOverKills);
      iWriter.Write(this.mLedFarmerKills);
      iWriter.Write(this.mLastGamer.GamerTag);
      iWriter.Write(this.mConsectuiveCruiseCount);
      iWriter.Write(this.mLastCruiseDate.ToBinary());
      iWriter.Write(this.mBanisherOfHorrors);
      iWriter.Write(this.mKilledPony.Count);
      for (int index = 0; index < this.mKilledPony.Count; ++index)
      {
        iWriter.Write(this.mKilledPony.ElementAt<KeyValuePair<string, bool>>(index).Key);
        iWriter.Write(this.mKilledPony.ElementAt<KeyValuePair<string, bool>>(index).Value);
      }
      iWriter.Close();
    }
    catch
    {
      iWriter?.Close();
    }
  }

  public SortedList<string, Profile.PlayableAvatar> Avatars => this.mPlayables;

  public SortedList<string, Gamer> Gamers => this.mGamers;

  internal void ChangeName(Gamer iGamer, string iName)
  {
    for (int index = 0; index < SaveManager.Instance.SaveSlots.Length; ++index)
    {
      PlayerSaveData playerSaveData;
      if (SaveManager.Instance.SaveSlots[index] != null && SaveManager.Instance.SaveSlots[index].Players.TryGetValue(iGamer.GamerTag, out playerSaveData))
      {
        SaveManager.Instance.SaveSlots[index].Players.Remove(iGamer.GamerTag);
        SaveManager.Instance.SaveSlots[index].Players.Add(iName, playerSaveData);
      }
    }
    this.mGamers.Remove(iGamer.GamerTag);
    iGamer.GamerTag = iName;
    this.Add(iGamer);
  }

  public void Add(Gamer iGamer)
  {
    this.mGamers[iGamer.GamerTag] = iGamer;
    this.Write();
  }

  public bool Remove(string iGamerTag)
  {
    for (int index = 0; index < SaveManager.Instance.SaveSlots.Length; ++index)
    {
      if (SaveManager.Instance.SaveSlots[index] != null && SaveManager.Instance.SaveSlots[index].Players.TryGetValue(iGamerTag, out PlayerSaveData _))
        SaveManager.Instance.SaveSlots[index].Players.Remove(iGamerTag);
    }
    bool flag = this.mGamers.Remove(iGamerTag);
    if (flag)
      this.Write();
    return flag;
  }

  public Gamer LastGamer
  {
    get => this.mLastGamer;
    set => this.mLastGamer = value;
  }

  public void FoundAMoose(PlayState iPlayState, string iID)
  {
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    this.mFoundMoose[iID] = true;
    int nCurProgress = 0;
    foreach (bool flag in this.mFoundMoose.Values)
    {
      if (flag)
        ++nCurProgress;
    }
    SteamUserStats.IndicateAchievementProgress("kingsquest", (uint) nCurProgress, 12U);
    SteamUserStats.StoreStats();
    if (nCurProgress != 12)
      return;
    AchievementsManager.Instance.AwardAchievement(iPlayState, "kingsquest");
  }

  public void FoundSecretArea(PlayState iPlayState, string iID)
  {
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    this.mFoundSecretAreas[iID] = true;
    int nCurProgress = 0;
    foreach (bool flag in this.mFoundSecretAreas.Values)
    {
      if (flag)
        ++nCurProgress;
    }
    SteamUserStats.IndicateAchievementProgress("sherlockholmes", (uint) nCurProgress, 14U);
    SteamUserStats.StoreStats();
    if (nCurProgress != 14)
      return;
    AchievementsManager.Instance.AwardAchievement(iPlayState, "sherlockholmes");
  }

  public void KilledAPony(PlayState iPlayState, string iID)
  {
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    this.mKilledPony[iID] = true;
    int nCurProgress = 0;
    foreach (bool flag in this.mKilledPony.Values)
    {
      if (flag)
        ++nCurProgress;
    }
    SteamUserStats.IndicateAchievementProgress("friendship", (uint) nCurProgress, 11U);
    SteamUserStats.StoreStats();
    if (nCurProgress != 11)
      return;
    AchievementsManager.Instance.AwardAchievement(iPlayState, "friendship");
  }

  public void UsedElements(PlayState iPlayState, string iTag, Elements iElements)
  {
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    if (!this.mPlayerUsedElements.TryGetValue(iTag, out Elements _))
    {
      this.mPlayerUsedElements.Add(iTag, iElements);
    }
    else
    {
      Dictionary<string, Elements> playerUsedElements;
      string key;
      (playerUsedElements = this.mPlayerUsedElements)[key = iTag] = playerUsedElements[key] | iElements;
    }
    if ((this.mPlayerUsedElements[iTag] & Elements.Basic) != Elements.Basic)
      return;
    AchievementsManager.Instance.AwardAchievement(iPlayState, "basicelement");
  }

  public void AddHealedAmount(PlayState iPlayState, float iAmount)
  {
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    this.mTotalAmountHealed += -iAmount;
    if ((double) this.mTotalAmountHealed < 100000.0)
      return;
    AchievementsManager.Instance.AwardAchievement(iPlayState, "killingyourfriendsyo");
  }

  public void AddOverKills(Character iVictim)
  {
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    if (iVictim.HasStatus(StatusEffects.Frozen))
      this.AddFrozenOverKill(iVictim.PlayState);
    ++this.mNumberOfOverKills;
    if (this.mNumberOfOverKills < 1000 && this.mNumberOfOverKills % 25 == 0)
    {
      SteamUserStats.IndicateAchievementProgress("badtaste", (uint) Math.Min(this.mNumberOfOverKills, 1000), 1000U);
      SteamUserStats.StoreStats();
    }
    if (this.mNumberOfOverKills < 1000)
      return;
    AchievementsManager.Instance.AwardAchievement(iVictim.PlayState, "badtaste");
  }

  private void AddFrozenOverKill(PlayState iPlayState)
  {
    ++this.mNumberOfFrozenOverKills;
    if (this.mNumberOfFrozenOverKills == 1 || this.mNumberOfFrozenOverKills < 100 && this.mNumberOfFrozenOverKills % 10 == 0)
    {
      SteamUserStats.IndicateAchievementProgress("iceage", (uint) Math.Min(this.mNumberOfFrozenOverKills, 100), 100U);
      SteamUserStats.StoreStats();
    }
    if (this.mNumberOfFrozenOverKills < 100)
      return;
    AchievementsManager.Instance.AwardAchievement(iPlayState, "iceage");
  }

  public void AddMythosKill(PlayState iPlayState, Character iTarget)
  {
    if (iTarget == null || HackHelper.LicenseStatus == HackHelper.Status.Hacked || SubMenuCharacterSelect.Instance.GameType != GameType.Mythos || !Profile.mMythosCreatures.Contains(iTarget.Template.ID))
      return;
    ++this.mBanisherOfHorrors;
    Console.WriteLine(this.mBanisherOfHorrors.ToString());
    if (this.mBanisherOfHorrors >= 1000)
    {
      AchievementsManager.Instance.AwardAchievement(iPlayState, "banisherofhorrors");
    }
    else
    {
      if (this.mBanisherOfHorrors != 1 && this.mBanisherOfHorrors % 25 != 0)
        return;
      SteamUserStats.IndicateAchievementProgress("banisherofhorrors", (uint) Math.Min(this.mBanisherOfHorrors, 1000), 1000U);
      SteamUserStats.StoreStats();
    }
  }

  public void AddLedKill(PlayState iPlayState)
  {
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    ++this.mLedFarmerKills;
    if (this.mLedFarmerKills >= 1000)
    {
      AchievementsManager.Instance.AwardAchievement(iPlayState, "ledfarmer");
    }
    else
    {
      if (this.mLedFarmerKills % 25 != 0)
        return;
      SteamUserStats.IndicateAchievementProgress("ledfarmer", (uint) Math.Min(this.mLedFarmerKills, 1000), 1000U);
      SteamUserStats.StoreStats();
    }
  }

  public void PlayingIslandCruise(PlayState iPlayState)
  {
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    DateTime now = DateTime.Now;
    int num = now.Day - this.mLastCruiseDate.Day;
    if (num == 1)
    {
      ++this.mConsectuiveCruiseCount;
      if (this.mConsectuiveCruiseCount >= 7)
      {
        AchievementsManager.Instance.AwardAchievement(iPlayState, "daycruise");
      }
      else
      {
        SteamUserStats.IndicateAchievementProgress("daycruise", (uint) this.mConsectuiveCruiseCount, 7U);
        SteamUserStats.StoreStats();
      }
    }
    else if (num >= 2)
      this.mConsectuiveCruiseCount = 0;
    this.mLastCruiseDate = now;
  }

  internal Profile.PlayableAvatar GetAvatar(string iName)
  {
    Profile.PlayableAvatar playableAvatar;
    return this.mPlayables.TryGetValue(iName, out playableAvatar) ? playableAvatar : this.DefaultAvatar;
  }

  public struct PlayableAvatar
  {
    private static SHA256 sHasher = SHA256.Create();
    internal string Name;
    internal string ThumbPath;
    internal Texture2D Thumb;
    internal string PortraitPath;
    internal Texture2D Portrait;
    internal string TypeName;
    internal int Type;
    internal bool AllowChallenge;
    internal bool AllowPVP;
    internal bool AllowCampaign;
    internal uint AppID;
    private byte[] mHashSum;
    private int mDisplayName;
    private int mDescription;

    internal PlayableAvatar(string iFileName)
      : this()
    {
      this.Name = Path.GetFileNameWithoutExtension(iFileName).ToLowerInvariant();
      this.AppID = 42910U;
      FileStream fileStream = File.OpenRead(iFileName);
      this.mHashSum = Profile.PlayableAvatar.sHasher.ComputeHash((Stream) fileStream);
      fileStream.Position = 0L;
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load((Stream) fileStream);
      fileStream.Close();
      XmlNode xmlNode = (XmlNode) null;
      for (int i = 0; i < xmlDocument.ChildNodes.Count; ++i)
      {
        XmlNode childNode = xmlDocument.ChildNodes[i];
        if (!(childNode is XmlComment) && childNode.Name.Equals("PlayableCharacter", StringComparison.OrdinalIgnoreCase))
          xmlNode = childNode;
      }
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlNode childNode = xmlNode.ChildNodes[i];
        if (!(childNode is XmlComment))
        {
          if (childNode.Name.Equals(nameof (DisplayName), StringComparison.OrdinalIgnoreCase))
            this.mDisplayName = childNode.InnerText.ToLowerInvariant().GetHashCodeCustom();
          else if (childNode.Name.Equals(nameof (Description), StringComparison.OrdinalIgnoreCase))
            this.mDescription = childNode.InnerText.ToLowerInvariant().GetHashCodeCustom();
          else if (childNode.Name.Equals("thumb", StringComparison.OrdinalIgnoreCase))
          {
            this.ThumbPath = childNode.InnerText;
            lock (Game.Instance.GraphicsDevice)
              this.Thumb = Game.Instance.Content.Load<Texture2D>(this.ThumbPath);
          }
          else if (childNode.Name.Equals("portrait", StringComparison.OrdinalIgnoreCase))
          {
            this.PortraitPath = childNode.InnerText;
            lock (Game.Instance.GraphicsDevice)
              this.Portrait = Game.Instance.Content.Load<Texture2D>(this.PortraitPath);
          }
          else if (childNode.Name.Equals("character", StringComparison.OrdinalIgnoreCase))
          {
            this.TypeName = childNode.InnerText.ToLowerInvariant();
            this.Type = this.TypeName.GetHashCodeCustom();
          }
          else if (childNode.Name.Equals("allowCampaign", StringComparison.OrdinalIgnoreCase))
            this.AllowCampaign = bool.Parse(childNode.InnerText);
          else if (childNode.Name.Equals("allowChallenge", StringComparison.OrdinalIgnoreCase))
            this.AllowChallenge = bool.Parse(childNode.InnerText);
          else if (childNode.Name.Equals("allowPVP", StringComparison.OrdinalIgnoreCase))
            this.AllowPVP = bool.Parse(childNode.InnerText);
          else if (childNode.Name.Equals("appid", StringComparison.OrdinalIgnoreCase))
            this.AppID = uint.Parse(childNode.InnerText);
        }
      }
    }

    internal static Profile.PlayableAvatar CreateDefault()
    {
      Profile.PlayableAvatar playableAvatar;
      playableAvatar.Name = "wizard";
      playableAvatar.ThumbPath = "Models/Characters/Wizard/Thumb";
      lock (Game.Instance.GraphicsDevice)
        playableAvatar.Thumb = Game.Instance.Content.Load<Texture2D>(playableAvatar.ThumbPath);
      playableAvatar.PortraitPath = "Models/Characters/Wizard/Thumb_Portrait";
      lock (Game.Instance.GraphicsDevice)
        playableAvatar.Portrait = Game.Instance.Content.Load<Texture2D>(playableAvatar.PortraitPath);
      playableAvatar.AllowCampaign = true;
      playableAvatar.AllowChallenge = true;
      playableAvatar.AllowPVP = true;
      playableAvatar.AppID = 42910U;
      playableAvatar.mHashSum = (byte[]) null;
      playableAvatar.TypeName = "wizard";
      playableAvatar.Type = playableAvatar.TypeName.GetHashCodeCustom();
      playableAvatar.mDisplayName = "#tooltip_avatar_vanilla".GetHashCodeCustom();
      playableAvatar.mDescription = "#tooltip_avatar_vanillad".GetHashCodeCustom();
      return playableAvatar;
    }

    internal byte[] HashSum => this.mHashSum;

    public int DisplayName => this.mDisplayName;

    public int Description => this.mDescription;

    public bool Equals(Profile.PlayableAvatar rhs) => this == rhs;

    public static bool operator ==(Profile.PlayableAvatar a, Profile.PlayableAvatar b)
    {
      return object.Equals((object) a, (object) b) || a.GetHashCode() == b.GetHashCode();
    }

    public static bool operator !=(Profile.PlayableAvatar a, Profile.PlayableAvatar b) => !(a == b);
  }
}
