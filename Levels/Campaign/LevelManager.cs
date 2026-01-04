// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Campaign.LevelManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Magicka.GameLogic.GameStates;
using Magicka.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Campaign;

internal sealed class LevelManager
{
  private static LevelManager sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static Dictionary<string, int> LOC_LOOKUP;
  private HackHelper.Status mCampaignIsHacked;
  private HackHelper.License mMythosCampaignLicense;
  private CampaignNode[] mMythosCampaign;
  private CampaignNode[] mDungeonsCampaign;
  private CampaignNode[] mCampaign;
  private LevelNode[] mChallenges;
  private LevelNode[] mVersus;
  private LevelNode[] mStoryChallenges;
  private static readonly string[] hcStoryChallanges = new string[4]
  {
    "ch_vietnam",
    "ch_osotc",
    "ch_dungeons_ch1",
    "ch_dungeons_ch2"
  };

  public static LevelManager Instance
  {
    get
    {
      if (LevelManager.sSingelton == null)
      {
        lock (LevelManager.sSingeltonLock)
        {
          if (LevelManager.sSingelton == null)
            LevelManager.sSingelton = new LevelManager();
        }
      }
      return LevelManager.sSingelton;
    }
  }

  static LevelManager()
  {
    LevelManager.LOC_LOOKUP = new Dictionary<string, int>();
    LevelManager.LOC_LOOKUP.Add("wizard_castle", "#chapter_01".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("mountaindale", "#chapter_02".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("highlands", "#chapter_03".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("havindr", "#chapter_04".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("battlefield", "#chapter_05".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("endofworld", "#chapter_06".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("wizard_castle2", "#chapter_07".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("mines", "#chapter_08".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("swamp", "#chapter_09".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("niflheim", "#chapter_10".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ruins", "#chapter_11".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("endofworld2", "#chapter_12".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_arena", "#challenge_arena".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_glade", "#challenge_glade".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_cavern", "#challenge_cavern".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_swamp", "#challenge_swamp".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_vietnam", "#challenge_vietnam".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_fields", "#challenge_vietnams".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_end_of_world", "#challenge_end_of_world_name".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_frosty_holiday", "#challenge_xmas2012".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_mirror_crystal_cavern", "#challenge_mirror_crystal_cavern_hideout_name".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_necro", "#challenge_necro".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_outsmouth", "#challenge_outsmouth".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_shrine", "#challenge_shrine".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_volcano_hideout", "#challenge_volcano_hideout_name".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_woot_bout_of_madness", "#challenge_bout_of_madness".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_woot_elemental_roulette", "#challenge_elemental_roulette".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_woot_eye_sockey_rink", "#challenge_eye_sockey_rink".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_dungeons_ch1", "#challenge_dungeons_chapter1".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_dungeons_ch2", "#challenge_dungeons_chapter2".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_osotc", "#challenge_osotc".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_havindrarena", "#versus_arena".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_frozen_lake", "#versus_lake".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_traininggrounds", "#versus_training".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_watchtower", "#versus_top".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_vulcanus_arena", "#versus_star_trek".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_boat", "#versus_lonely_island".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_woot_diamond_ring", "#versus_diamond_ring".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_woot_elemental_roulette", "#versus_elemental_roulette".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_woot_halls_of_moisture", "#versus_halls_of_moisture".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_woot_prize_podium", "#versus_prize_podium".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("ch_vulcanus", "#versus_star_trek".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("vs_monster_belly", "#challenge_winterpvp2012stomach".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("tsar_wizardcastle", "#tsar_wizardcastled".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("tsar_mountaindale", "#tsar_mountaindaled".GetHashCodeCustom());
    LevelManager.LOC_LOOKUP.Add("tsar_rlyeh", "#tsar_rlyehd".GetHashCodeCustom());
  }

  public bool IsStoryChallange(string lvlFileName)
  {
    if (string.IsNullOrEmpty(lvlFileName))
      return false;
    lvlFileName = lvlFileName.Replace(".lvl", "").Trim();
    if (string.IsNullOrEmpty(lvlFileName))
      return false;
    for (int index = 0; index < LevelManager.hcStoryChallanges.Length; ++index)
    {
      if (string.Compare(lvlFileName, LevelManager.hcStoryChallanges[index]) == 0)
        return true;
    }
    return false;
  }

  private LevelManager()
  {
    this.mCampaignIsHacked = HackHelper.Status.Pending;
    this.mMythosCampaignLicense = HackHelper.License.Pending;
    FileInfo[] files1 = new DirectoryInfo("content/Levels/Challenges/").GetFiles("*.lvl");
    List<LevelNode> levelNodeList1 = new List<LevelNode>();
    List<LevelNode> levelNodeList2 = new List<LevelNode>();
    for (int index = 0; index < files1.Length; ++index)
    {
      string name = files1[index].Name;
      if (name.Equals("ch_vietnam.lvl", StringComparison.InvariantCultureIgnoreCase))
      {
        levelNodeList2.Add(new LevelNode("Challenges/", name));
        levelNodeList1.Add(new LevelNode("Challenges/", name));
      }
      else if (this.IsStoryChallange(name))
        levelNodeList2.Add(new LevelNode("Challenges/", name));
      else
        levelNodeList1.Add(new LevelNode("Challenges/", name));
    }
    this.mChallenges = levelNodeList1.ToArray();
    levelNodeList1.Clear();
    FileInfo[] files2 = new DirectoryInfo("content/Levels/Versus/").GetFiles("*.lvl");
    this.mVersus = new LevelNode[files2.Length];
    for (int index = 0; index < files2.Length; ++index)
      this.mVersus[index] = new LevelNode("Versus/", files2[index].Name);
    this.mCampaign = new CampaignNode[12];
    this.mCampaign[0] = new CampaignNode("Wizard_Castle.lvl", new SpawnPoint?());
    this.mCampaign[1] = new CampaignNode("Mountaindale.lvl", new SpawnPoint?());
    this.mCampaign[2] = new CampaignNode("Highlands.lvl", new SpawnPoint?());
    this.mCampaign[3] = new CampaignNode("Havindr.lvl", new SpawnPoint?());
    this.mCampaign[4] = new CampaignNode("Battlefield.lvl", new SpawnPoint?());
    this.mCampaign[5] = new CampaignNode("EndOfWorld.lvl", new SpawnPoint?());
    this.mCampaign[6] = new CampaignNode("Wizard_Castle2.lvl", new SpawnPoint?());
    this.mCampaign[7] = new CampaignNode("Mines.lvl", new SpawnPoint?());
    this.mCampaign[8] = new CampaignNode("Swamp.lvl", new SpawnPoint?());
    this.mCampaign[9] = new CampaignNode("Niflheim.lvl", new SpawnPoint?());
    this.mCampaign[10] = new CampaignNode("Ruins.lvl", new SpawnPoint?());
    this.mCampaign[11] = new CampaignNode("EndOfWorld2.lvl", new SpawnPoint?());
    XmlDocument xmlDocument1 = new XmlDocument();
    xmlDocument1.Load("content/Levels/Cutscenes.xml");
    XmlNode xmlNode1 = (XmlNode) null;
    for (int i = 0; i < xmlDocument1.ChildNodes.Count; ++i)
    {
      XmlNode childNode = xmlDocument1.ChildNodes[i];
      if (childNode.Name.Equals("cutscenes", StringComparison.OrdinalIgnoreCase))
        xmlNode1 = childNode;
    }
    for (int i1 = 0; i1 < xmlNode1.ChildNodes.Count; ++i1)
    {
      XmlNode childNode = xmlNode1.ChildNodes[i1];
      if (!(childNode is XmlComment) && childNode.Name.Equals("cutscene", StringComparison.OrdinalIgnoreCase))
      {
        Cutscene cutscene = new Cutscene(childNode);
        int[] numArray = (int[]) null;
        for (int i2 = 0; i2 < childNode.Attributes.Count; ++i2)
        {
          XmlAttribute attribute = childNode.Attributes[i2];
          if (attribute.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
          {
            string[] strArray = attribute.Value.Split(',');
            numArray = new int[strArray.Length];
            for (int index = 0; index < strArray.Length; ++index)
              numArray[index] = int.Parse(strArray[index]);
          }
        }
        for (int index = 0; index < numArray.Length; ++index)
          this.mCampaign[numArray[index]].Cutscene = cutscene;
      }
    }
    this.mMythosCampaign = new CampaignNode[3];
    this.mMythosCampaign[0] = new CampaignNode("Tsar/Tsar_WizardCastle.lvl", new SpawnPoint?());
    this.mMythosCampaign[1] = new CampaignNode("Tsar/Tsar_Mountaindale.lvl", new SpawnPoint?());
    this.mMythosCampaign[2] = new CampaignNode("Tsar/Tsar_Rlyeh.lvl", new SpawnPoint?());
    XmlDocument xmlDocument2 = new XmlDocument();
    xmlDocument2.Load("content/Levels/Tsar/Cutscenes.xml");
    XmlNode xmlNode2 = (XmlNode) null;
    for (int i = 0; i < xmlDocument2.ChildNodes.Count; ++i)
    {
      XmlNode childNode = xmlDocument2.ChildNodes[i];
      if (childNode.Name.Equals("cutscenes", StringComparison.OrdinalIgnoreCase))
        xmlNode2 = childNode;
    }
    for (int i3 = 0; i3 < xmlNode2.ChildNodes.Count; ++i3)
    {
      XmlNode childNode = xmlNode2.ChildNodes[i3];
      if (!(childNode is XmlComment) && childNode.Name.Equals("cutscene", StringComparison.OrdinalIgnoreCase))
      {
        Cutscene cutscene = new Cutscene(childNode);
        int[] numArray = (int[]) null;
        for (int i4 = 0; i4 < childNode.Attributes.Count; ++i4)
        {
          XmlAttribute attribute = childNode.Attributes[i4];
          if (attribute.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
          {
            string[] strArray = attribute.Value.Split(',');
            numArray = new int[strArray.Length];
            for (int index = 0; index < strArray.Length; ++index)
              numArray[index] = int.Parse(strArray[index]);
          }
        }
        for (int index = 0; index < numArray.Length; ++index)
          this.mMythosCampaign[numArray[index]].Cutscene = cutscene;
      }
    }
    this.mDungeonsCampaign = new CampaignNode[2];
    bool flag1 = true;
    try
    {
      this.mDungeonsCampaign[0] = new CampaignNode("Challenges/ch_dungeons_ch1.lvl", new SpawnPoint?());
      this.mDungeonsCampaign[1] = (CampaignNode) null;
    }
    catch (Exception ex)
    {
      flag1 = false;
      this.mDungeonsCampaign[0] = (CampaignNode) null;
    }
    if (flag1)
    {
      XmlDocument xmlDocument3 = new XmlDocument();
      bool flag2 = true;
      try
      {
        xmlDocument3.Load("content/Levels/Challenges/Dungeons_Cutscenes.xml");
      }
      catch (Exception ex)
      {
        flag2 = false;
        xmlDocument3 = (XmlDocument) null;
      }
      if (flag2)
      {
        XmlNode xmlNode3 = (XmlNode) null;
        for (int i = 0; i < xmlDocument3.ChildNodes.Count; ++i)
        {
          XmlNode childNode = xmlDocument3.ChildNodes[i];
          if (childNode.Name.Equals("cutscenes", StringComparison.OrdinalIgnoreCase))
            xmlNode3 = childNode;
        }
        for (int i = 0; i < xmlNode3.ChildNodes.Count; ++i)
        {
          XmlNode childNode = xmlNode3.ChildNodes[i];
          if (!(childNode is XmlComment) && childNode.Name.Equals("cutscene", StringComparison.OrdinalIgnoreCase))
            this.mDungeonsCampaign[0].Cutscene = new Cutscene(childNode);
        }
      }
    }
    bool flag3 = true;
    try
    {
      this.mDungeonsCampaign[1] = new CampaignNode("Challenges/ch_dungeons_ch2.lvl", new SpawnPoint?());
    }
    catch (Exception ex)
    {
      flag3 = false;
      this.mDungeonsCampaign[1] = (CampaignNode) null;
    }
    if (flag3)
    {
      XmlDocument xmlDocument4 = new XmlDocument();
      bool flag4 = true;
      try
      {
        xmlDocument4.Load("content/Levels/Challenges/Dungeons2_Cutscenes.xml");
      }
      catch (Exception ex)
      {
        flag4 = false;
        xmlDocument4 = (XmlDocument) null;
      }
      if (flag4)
      {
        XmlNode xmlNode4 = (XmlNode) null;
        for (int i = 0; i < xmlDocument4.ChildNodes.Count; ++i)
        {
          XmlNode childNode = xmlDocument4.ChildNodes[i];
          if (childNode.Name.Equals("cutscenes", StringComparison.OrdinalIgnoreCase))
            xmlNode4 = childNode;
        }
        for (int i = 0; i < xmlNode4.ChildNodes.Count; ++i)
        {
          XmlNode childNode = xmlNode4.ChildNodes[i];
          if (!(childNode is XmlComment) && childNode.Name.Equals("cutscene", StringComparison.OrdinalIgnoreCase))
            this.mDungeonsCampaign[1].Cutscene = new Cutscene(childNode);
        }
      }
    }
    this.mStoryChallenges = levelNodeList2.ToArray();
    levelNodeList2.Clear();
  }

  private void ChallengeSort()
  {
    int length = this.mChallenges.Length;
    LevelNode[] levelNodeArray = new LevelNode[length];
    int index1 = 0;
    for (int index2 = 0; index2 < length; ++index2)
    {
      if (this.mChallenges[index2].Name.Equals("#challenge_osotc"))
      {
        levelNodeArray[index1] = this.mChallenges[index2];
        ++index1;
      }
      else
      {
        int num = 1 - index1;
        levelNodeArray[index2 + num] = this.mChallenges[index2];
      }
    }
    this.mChallenges = levelNodeArray;
  }

  internal static string GetLocalizedName(string iLevelFileName)
  {
    int iID;
    return LevelManager.LOC_LOOKUP.TryGetValue(iLevelFileName.ToLowerInvariant(), out iID) ? LanguageManager.Instance.GetString(iID) : (string) null;
  }

  public void BeginComputeHashes()
  {
    this.mCampaignIsHacked = HackHelper.Status.Pending;
    this.mMythosCampaignLicense = HackHelper.License.Pending;
    new Thread(new ThreadStart(this.ComputeHashes))
    {
      Name = "Level Hasher"
    }.Start();
  }

  private void ComputeHashes()
  {
    SHA256 iSHA = SHA256.Create();
    for (int index = 0; index < this.mCampaign.Length; ++index)
    {
      this.mCampaign[index].ComputeHashSums(iSHA);
      switch (HackHelper.CheckLicense((LevelNode) this.mCampaign[index]))
      {
        case HackHelper.License.No:
          throw new Exception("User does not have a license for the campaign!");
        case HackHelper.License.Custom:
          this.mCampaignIsHacked = HackHelper.Status.Hacked;
          break;
      }
    }
    if (this.mCampaignIsHacked == HackHelper.Status.Pending)
      this.mCampaignIsHacked = HackHelper.Status.Valid;
    for (int index = 0; index < this.mMythosCampaign.Length; ++index)
      this.mMythosCampaign[index].ComputeHashSums(iSHA);
    this.UpdateMythosLicense();
    if (this.mDungeonsCampaign != null)
    {
      for (int index = 0; index < this.mDungeonsCampaign.Length; ++index)
        this.mDungeonsCampaign[index].ComputeHashSums(iSHA);
    }
    for (int index = 0; index < this.mChallenges.Length; ++index)
      this.mChallenges[index].ComputeHashSums(iSHA);
    for (int index = 0; index < this.mStoryChallenges.Length; ++index)
      this.mStoryChallenges[index].ComputeHashSums(iSHA);
    for (int index = 0; index < this.mVersus.Length; ++index)
      this.mVersus[index].ComputeHashSums(iSHA);
  }

  internal void UpdateMythosLicense()
  {
    HackHelper.License license1 = HackHelper.License.Pending;
    for (int index = 0; index < this.mMythosCampaign.Length; ++index)
    {
      HackHelper.License license2 = HackHelper.CheckLicense((LevelNode) this.mMythosCampaign[index]);
      if (license1 != HackHelper.License.No && license2 == HackHelper.License.Custom)
        license1 = HackHelper.License.Custom;
      else if (license2 == HackHelper.License.No)
        license1 = HackHelper.License.No;
    }
    if (!HackHelper.CheckLicenseMythos())
      license1 = HackHelper.License.No;
    else if (license1 == HackHelper.License.Pending)
      license1 = HackHelper.License.Yes;
    this.mMythosCampaignLicense = license1;
  }

  public LevelNode GetLevel(GameType iGameType, int iLevel)
  {
    if (iLevel < 0)
      return (LevelNode) null;
    switch (iGameType)
    {
      case GameType.Campaign:
        return (LevelNode) this.mCampaign[iLevel];
      case GameType.Challenge:
        return this.mChallenges[iLevel];
      case GameType.Versus:
        return this.mVersus[iLevel];
      case GameType.Mythos:
        return (LevelNode) this.mMythosCampaign[iLevel];
      case GameType.StoryChallange:
        return this.mStoryChallenges[iLevel < 0 ? 0 : (iLevel > this.mStoryChallenges.Length - 1 ? this.mStoryChallenges.Length - 1 : iLevel)];
      default:
        throw new Exception("Invalid GameType!");
    }
  }

  public LevelNode GetLevel(GameType iGameType, byte[] iLevelHashSum)
  {
    return this.GetLevel(iGameType, iLevelHashSum, out int _);
  }

  public LevelNode GetLevel(GameType iGameType, byte[] iLevelHashSum, out int oIndex)
  {
    List<LevelNode> levelNodeList = new List<LevelNode>();
    switch (iGameType)
    {
      case GameType.Campaign:
        lock (this.mCampaign)
          levelNodeList.AddRange((IEnumerable<LevelNode>) this.mCampaign);
        for (int index = 0; index < levelNodeList.Count; ++index)
        {
          if (Helper.ArrayEquals(iLevelHashSum, levelNodeList[index].GetCombinedHash()))
          {
            oIndex = index;
            return levelNodeList[index];
          }
        }
        break;
      case GameType.Challenge:
        lock (this.mChallenges)
          levelNodeList.AddRange((IEnumerable<LevelNode>) this.mChallenges);
        for (int index = 0; index < levelNodeList.Count; ++index)
        {
          if (Helper.ArrayEquals(iLevelHashSum, levelNodeList[index].GetCombinedHash()))
          {
            oIndex = index;
            return levelNodeList[index];
          }
        }
        break;
      case GameType.Versus:
        lock (this.mVersus)
          levelNodeList.AddRange((IEnumerable<LevelNode>) this.mVersus);
        for (int index = 0; index < levelNodeList.Count; ++index)
        {
          if (Helper.ArrayEquals(iLevelHashSum, levelNodeList[index].GetCombinedHash()))
          {
            oIndex = index;
            return levelNodeList[index];
          }
        }
        break;
      case GameType.Mythos:
        lock (this.mMythosCampaign)
          levelNodeList.AddRange((IEnumerable<LevelNode>) this.mMythosCampaign);
        for (int index = 0; index < levelNodeList.Count; ++index)
        {
          if (Helper.ArrayEquals(iLevelHashSum, levelNodeList[index].GetCombinedHash()))
          {
            oIndex = index;
            return levelNodeList[index];
          }
        }
        break;
      case GameType.StoryChallange:
        lock (this.mStoryChallenges)
          levelNodeList.AddRange((IEnumerable<LevelNode>) this.mStoryChallenges);
        for (int index = 0; index < levelNodeList.Count; ++index)
        {
          if (Helper.ArrayEquals(iLevelHashSum, levelNodeList[index].GetCombinedHash()))
          {
            oIndex = index;
            return levelNodeList[index];
          }
        }
        break;
      default:
        throw new Exception("Invalid GameType!");
    }
    oIndex = -1;
    return (LevelNode) null;
  }

  public HackHelper.Status CampaignIsHacked => this.mCampaignIsHacked;

  public HackHelper.License MythosCampaignLicense => this.mMythosCampaignLicense;

  public CampaignNode[] MythosCampaign => this.mMythosCampaign;

  public CampaignNode[] DungeonsCampaign => this.mDungeonsCampaign;

  public CampaignNode[] VanillaCampaign => this.mCampaign;

  public LevelNode[] Challenges => this.mChallenges;

  public LevelNode[] StoryChallanges => this.mStoryChallenges;

  public LevelNode[] Versus => this.mVersus;
}
