using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using Magicka.DRM;
using Magicka.GameLogic.GameStates;
using Magicka.Localization;

namespace Magicka.Levels.Campaign
{
	// Token: 0x02000290 RID: 656
	internal sealed class LevelManager
	{
		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06001355 RID: 4949 RVA: 0x00076D4C File Offset: 0x00074F4C
		public static LevelManager Instance
		{
			get
			{
				if (LevelManager.sSingelton == null)
				{
					lock (LevelManager.sSingeltonLock)
					{
						if (LevelManager.sSingelton == null)
						{
							LevelManager.sSingelton = new LevelManager();
						}
					}
				}
				return LevelManager.sSingelton;
			}
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x00076DA0 File Offset: 0x00074FA0
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

		// Token: 0x06001357 RID: 4951 RVA: 0x00077270 File Offset: 0x00075470
		public bool IsStoryChallange(string lvlFileName)
		{
			if (string.IsNullOrEmpty(lvlFileName))
			{
				return false;
			}
			lvlFileName = lvlFileName.Replace(".lvl", "").Trim();
			if (string.IsNullOrEmpty(lvlFileName))
			{
				return false;
			}
			for (int i = 0; i < LevelManager.hcStoryChallanges.Length; i++)
			{
				if (string.Compare(lvlFileName, LevelManager.hcStoryChallanges[i]) == 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001358 RID: 4952 RVA: 0x000772CC File Offset: 0x000754CC
		private LevelManager()
		{
			this.mCampaignIsHacked = HackHelper.Status.Pending;
			this.mMythosCampaignLicense = HackHelper.License.Pending;
			FileInfo[] files = new DirectoryInfo("content/Levels/Challenges/").GetFiles("*.lvl");
			List<LevelNode> list = new List<LevelNode>();
			List<LevelNode> list2 = new List<LevelNode>();
			for (int i = 0; i < files.Length; i++)
			{
				string name = files[i].Name;
				if (name.Equals("ch_vietnam.lvl", StringComparison.InvariantCultureIgnoreCase))
				{
					list2.Add(new LevelNode("Challenges/", name));
					list.Add(new LevelNode("Challenges/", name));
				}
				else if (this.IsStoryChallange(name))
				{
					list2.Add(new LevelNode("Challenges/", name));
				}
				else
				{
					list.Add(new LevelNode("Challenges/", name));
				}
			}
			this.mChallenges = list.ToArray();
			list.Clear();
			files = new DirectoryInfo("content/Levels/Versus/").GetFiles("*.lvl");
			this.mVersus = new LevelNode[files.Length];
			for (int j = 0; j < files.Length; j++)
			{
				this.mVersus[j] = new LevelNode("Versus/", files[j].Name);
			}
			this.mCampaign = new CampaignNode[12];
			this.mCampaign[0] = new CampaignNode("Wizard_Castle.lvl", null);
			this.mCampaign[1] = new CampaignNode("Mountaindale.lvl", null);
			this.mCampaign[2] = new CampaignNode("Highlands.lvl", null);
			this.mCampaign[3] = new CampaignNode("Havindr.lvl", null);
			this.mCampaign[4] = new CampaignNode("Battlefield.lvl", null);
			this.mCampaign[5] = new CampaignNode("EndOfWorld.lvl", null);
			this.mCampaign[6] = new CampaignNode("Wizard_Castle2.lvl", null);
			this.mCampaign[7] = new CampaignNode("Mines.lvl", null);
			this.mCampaign[8] = new CampaignNode("Swamp.lvl", null);
			this.mCampaign[9] = new CampaignNode("Niflheim.lvl", null);
			this.mCampaign[10] = new CampaignNode("Ruins.lvl", null);
			this.mCampaign[11] = new CampaignNode("EndOfWorld2.lvl", null);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load("content/Levels/Cutscenes.xml");
			XmlNode xmlNode = null;
			for (int k = 0; k < xmlDocument.ChildNodes.Count; k++)
			{
				XmlNode xmlNode2 = xmlDocument.ChildNodes[k];
				if (xmlNode2.Name.Equals("cutscenes", StringComparison.OrdinalIgnoreCase))
				{
					xmlNode = xmlNode2;
				}
			}
			for (int l = 0; l < xmlNode.ChildNodes.Count; l++)
			{
				XmlNode xmlNode3 = xmlNode.ChildNodes[l];
				if (!(xmlNode3 is XmlComment) && xmlNode3.Name.Equals("cutscene", StringComparison.OrdinalIgnoreCase))
				{
					Cutscene cutscene = new Cutscene(xmlNode3);
					int[] array = null;
					for (int m = 0; m < xmlNode3.Attributes.Count; m++)
					{
						XmlAttribute xmlAttribute = xmlNode3.Attributes[m];
						if (xmlAttribute.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
						{
							string[] array2 = xmlAttribute.Value.Split(new char[]
							{
								','
							});
							array = new int[array2.Length];
							for (int n = 0; n < array2.Length; n++)
							{
								array[n] = int.Parse(array2[n]);
							}
						}
					}
					for (int num = 0; num < array.Length; num++)
					{
						this.mCampaign[array[num]].Cutscene = cutscene;
					}
				}
			}
			this.mMythosCampaign = new CampaignNode[3];
			this.mMythosCampaign[0] = new CampaignNode("Tsar/Tsar_WizardCastle.lvl", null);
			this.mMythosCampaign[1] = new CampaignNode("Tsar/Tsar_Mountaindale.lvl", null);
			this.mMythosCampaign[2] = new CampaignNode("Tsar/Tsar_Rlyeh.lvl", null);
			xmlDocument = new XmlDocument();
			xmlDocument.Load("content/Levels/Tsar/Cutscenes.xml");
			xmlNode = null;
			for (int num2 = 0; num2 < xmlDocument.ChildNodes.Count; num2++)
			{
				XmlNode xmlNode4 = xmlDocument.ChildNodes[num2];
				if (xmlNode4.Name.Equals("cutscenes", StringComparison.OrdinalIgnoreCase))
				{
					xmlNode = xmlNode4;
				}
			}
			for (int num3 = 0; num3 < xmlNode.ChildNodes.Count; num3++)
			{
				XmlNode xmlNode5 = xmlNode.ChildNodes[num3];
				if (!(xmlNode5 is XmlComment) && xmlNode5.Name.Equals("cutscene", StringComparison.OrdinalIgnoreCase))
				{
					Cutscene cutscene2 = new Cutscene(xmlNode5);
					int[] array3 = null;
					for (int num4 = 0; num4 < xmlNode5.Attributes.Count; num4++)
					{
						XmlAttribute xmlAttribute2 = xmlNode5.Attributes[num4];
						if (xmlAttribute2.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
						{
							string[] array4 = xmlAttribute2.Value.Split(new char[]
							{
								','
							});
							array3 = new int[array4.Length];
							for (int num5 = 0; num5 < array4.Length; num5++)
							{
								array3[num5] = int.Parse(array4[num5]);
							}
						}
					}
					for (int num6 = 0; num6 < array3.Length; num6++)
					{
						this.mMythosCampaign[array3[num6]].Cutscene = cutscene2;
					}
				}
			}
			this.mDungeonsCampaign = new CampaignNode[2];
			bool flag = true;
			try
			{
				this.mDungeonsCampaign[0] = new CampaignNode("Challenges/ch_dungeons_ch1.lvl", null);
				this.mDungeonsCampaign[1] = null;
			}
			catch (Exception)
			{
				flag = false;
				this.mDungeonsCampaign[0] = null;
			}
			if (flag)
			{
				xmlDocument = new XmlDocument();
				bool flag2 = true;
				try
				{
					xmlDocument.Load("content/Levels/Challenges/Dungeons_Cutscenes.xml");
				}
				catch (Exception)
				{
					flag2 = false;
					xmlDocument = null;
				}
				if (flag2)
				{
					xmlNode = null;
					for (int num7 = 0; num7 < xmlDocument.ChildNodes.Count; num7++)
					{
						XmlNode xmlNode6 = xmlDocument.ChildNodes[num7];
						if (xmlNode6.Name.Equals("cutscenes", StringComparison.OrdinalIgnoreCase))
						{
							xmlNode = xmlNode6;
						}
					}
					for (int num8 = 0; num8 < xmlNode.ChildNodes.Count; num8++)
					{
						XmlNode xmlNode7 = xmlNode.ChildNodes[num8];
						if (!(xmlNode7 is XmlComment) && xmlNode7.Name.Equals("cutscene", StringComparison.OrdinalIgnoreCase))
						{
							Cutscene cutscene3 = new Cutscene(xmlNode7);
							this.mDungeonsCampaign[0].Cutscene = cutscene3;
						}
					}
				}
			}
			bool flag3 = true;
			try
			{
				this.mDungeonsCampaign[1] = new CampaignNode("Challenges/ch_dungeons_ch2.lvl", null);
			}
			catch (Exception)
			{
				flag3 = false;
				this.mDungeonsCampaign[1] = null;
			}
			if (flag3)
			{
				xmlDocument = new XmlDocument();
				bool flag4 = true;
				try
				{
					xmlDocument.Load("content/Levels/Challenges/Dungeons2_Cutscenes.xml");
				}
				catch (Exception)
				{
					flag4 = false;
					xmlDocument = null;
				}
				if (flag4)
				{
					xmlNode = null;
					for (int num9 = 0; num9 < xmlDocument.ChildNodes.Count; num9++)
					{
						XmlNode xmlNode8 = xmlDocument.ChildNodes[num9];
						if (xmlNode8.Name.Equals("cutscenes", StringComparison.OrdinalIgnoreCase))
						{
							xmlNode = xmlNode8;
						}
					}
					for (int num10 = 0; num10 < xmlNode.ChildNodes.Count; num10++)
					{
						XmlNode xmlNode9 = xmlNode.ChildNodes[num10];
						if (!(xmlNode9 is XmlComment) && xmlNode9.Name.Equals("cutscene", StringComparison.OrdinalIgnoreCase))
						{
							Cutscene cutscene4 = new Cutscene(xmlNode9);
							this.mDungeonsCampaign[1].Cutscene = cutscene4;
						}
					}
				}
			}
			this.mStoryChallenges = list2.ToArray();
			list2.Clear();
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x00077AF4 File Offset: 0x00075CF4
		private void ChallengeSort()
		{
			int num = this.mChallenges.Length;
			LevelNode[] array = new LevelNode[num];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (this.mChallenges[i].Name.Equals("#challenge_osotc"))
				{
					array[num2] = this.mChallenges[i];
					num2++;
				}
				else
				{
					int num3 = 1 - num2;
					array[i + num3] = this.mChallenges[i];
				}
			}
			this.mChallenges = array;
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x00077B64 File Offset: 0x00075D64
		internal static string GetLocalizedName(string iLevelFileName)
		{
			int iID;
			if (LevelManager.LOC_LOOKUP.TryGetValue(iLevelFileName.ToLowerInvariant(), out iID))
			{
				return LanguageManager.Instance.GetString(iID);
			}
			return null;
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x00077B94 File Offset: 0x00075D94
		public void BeginComputeHashes()
		{
			this.mCampaignIsHacked = HackHelper.Status.Pending;
			this.mMythosCampaignLicense = HackHelper.License.Pending;
			new Thread(new ThreadStart(this.ComputeHashes))
			{
				Name = "Level Hasher"
			}.Start();
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x00077BD4 File Offset: 0x00075DD4
		private void ComputeHashes()
		{
			SHA256 iSHA = SHA256.Create();
			for (int i = 0; i < this.mCampaign.Length; i++)
			{
				this.mCampaign[i].ComputeHashSums(iSHA);
				HackHelper.License license = HackHelper.CheckLicense(this.mCampaign[i]);
				if (license == HackHelper.License.Custom)
				{
					this.mCampaignIsHacked = HackHelper.Status.Hacked;
				}
				else if (license == HackHelper.License.No)
				{
					throw new Exception("User does not have a license for the campaign!");
				}
			}
			if (this.mCampaignIsHacked == HackHelper.Status.Pending)
			{
				this.mCampaignIsHacked = HackHelper.Status.Valid;
			}
			for (int j = 0; j < this.mMythosCampaign.Length; j++)
			{
				this.mMythosCampaign[j].ComputeHashSums(iSHA);
			}
			this.UpdateMythosLicense();
			if (this.mDungeonsCampaign != null)
			{
				for (int k = 0; k < this.mDungeonsCampaign.Length; k++)
				{
					this.mDungeonsCampaign[k].ComputeHashSums(iSHA);
				}
			}
			for (int l = 0; l < this.mChallenges.Length; l++)
			{
				this.mChallenges[l].ComputeHashSums(iSHA);
			}
			for (int m = 0; m < this.mStoryChallenges.Length; m++)
			{
				this.mStoryChallenges[m].ComputeHashSums(iSHA);
			}
			for (int n = 0; n < this.mVersus.Length; n++)
			{
				this.mVersus[n].ComputeHashSums(iSHA);
			}
		}

		// Token: 0x0600135D RID: 4957 RVA: 0x00077D08 File Offset: 0x00075F08
		internal void UpdateMythosLicense()
		{
			HackHelper.License license = HackHelper.License.Pending;
			for (int i = 0; i < this.mMythosCampaign.Length; i++)
			{
				HackHelper.License license2 = HackHelper.CheckLicense(this.mMythosCampaign[i]);
				if (license != HackHelper.License.No && license2 == HackHelper.License.Custom)
				{
					license = HackHelper.License.Custom;
				}
				else if (license2 == HackHelper.License.No)
				{
					license = HackHelper.License.No;
				}
			}
			if (!HackHelper.CheckLicenseMythos())
			{
				license = HackHelper.License.No;
			}
			else if (license == HackHelper.License.Pending)
			{
				license = HackHelper.License.Yes;
			}
			this.mMythosCampaignLicense = license;
		}

		// Token: 0x0600135E RID: 4958 RVA: 0x00077D60 File Offset: 0x00075F60
		public LevelNode GetLevel(GameType iGameType, int iLevel)
		{
			if (iLevel < 0)
			{
				return null;
			}
			switch (iGameType)
			{
			case GameType.Campaign:
				return this.mCampaign[iLevel];
			case GameType.Challenge:
				return this.mChallenges[iLevel];
			case (GameType)3:
				break;
			case GameType.Versus:
				return this.mVersus[iLevel];
			default:
				if (iGameType == GameType.Mythos)
				{
					return this.mMythosCampaign[iLevel];
				}
				if (iGameType == GameType.StoryChallange)
				{
					int num = (iLevel < 0) ? 0 : ((iLevel > this.mStoryChallenges.Length - 1) ? (this.mStoryChallenges.Length - 1) : iLevel);
					return this.mStoryChallenges[num];
				}
				break;
			}
			throw new Exception("Invalid GameType!");
		}

		// Token: 0x0600135F RID: 4959 RVA: 0x00077DF4 File Offset: 0x00075FF4
		public LevelNode GetLevel(GameType iGameType, byte[] iLevelHashSum)
		{
			int num;
			return this.GetLevel(iGameType, iLevelHashSum, out num);
		}

		// Token: 0x06001360 RID: 4960 RVA: 0x00077E0C File Offset: 0x0007600C
		public LevelNode GetLevel(GameType iGameType, byte[] iLevelHashSum, out int oIndex)
		{
			List<LevelNode> list = new List<LevelNode>();
			switch (iGameType)
			{
			case GameType.Campaign:
				lock (this.mCampaign)
				{
					list.AddRange(this.mCampaign);
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (Helper.ArrayEquals(iLevelHashSum, list[i].GetCombinedHash()))
					{
						oIndex = i;
						return list[i];
					}
				}
				goto IL_205;
			case GameType.Challenge:
				lock (this.mChallenges)
				{
					list.AddRange(this.mChallenges);
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (Helper.ArrayEquals(iLevelHashSum, list[j].GetCombinedHash()))
					{
						oIndex = j;
						return list[j];
					}
				}
				goto IL_205;
			case (GameType)3:
				break;
			case GameType.Versus:
				lock (this.mVersus)
				{
					list.AddRange(this.mVersus);
				}
				for (int k = 0; k < list.Count; k++)
				{
					if (Helper.ArrayEquals(iLevelHashSum, list[k].GetCombinedHash()))
					{
						oIndex = k;
						return list[k];
					}
				}
				goto IL_205;
			default:
				if (iGameType == GameType.Mythos)
				{
					lock (this.mMythosCampaign)
					{
						list.AddRange(this.mMythosCampaign);
					}
					for (int l = 0; l < list.Count; l++)
					{
						if (Helper.ArrayEquals(iLevelHashSum, list[l].GetCombinedHash()))
						{
							oIndex = l;
							return list[l];
						}
					}
					goto IL_205;
				}
				if (iGameType == GameType.StoryChallange)
				{
					lock (this.mStoryChallenges)
					{
						list.AddRange(this.mStoryChallenges);
					}
					for (int m = 0; m < list.Count; m++)
					{
						if (Helper.ArrayEquals(iLevelHashSum, list[m].GetCombinedHash()))
						{
							oIndex = m;
							return list[m];
						}
					}
					goto IL_205;
				}
				break;
			}
			throw new Exception("Invalid GameType!");
			IL_205:
			oIndex = -1;
			return null;
		}

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06001361 RID: 4961 RVA: 0x00078064 File Offset: 0x00076264
		public HackHelper.Status CampaignIsHacked
		{
			get
			{
				return this.mCampaignIsHacked;
			}
		}

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06001362 RID: 4962 RVA: 0x0007806C File Offset: 0x0007626C
		public HackHelper.License MythosCampaignLicense
		{
			get
			{
				return this.mMythosCampaignLicense;
			}
		}

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06001363 RID: 4963 RVA: 0x00078074 File Offset: 0x00076274
		public CampaignNode[] MythosCampaign
		{
			get
			{
				return this.mMythosCampaign;
			}
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06001364 RID: 4964 RVA: 0x0007807C File Offset: 0x0007627C
		public CampaignNode[] DungeonsCampaign
		{
			get
			{
				return this.mDungeonsCampaign;
			}
		}

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06001365 RID: 4965 RVA: 0x00078084 File Offset: 0x00076284
		public CampaignNode[] VanillaCampaign
		{
			get
			{
				return this.mCampaign;
			}
		}

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06001366 RID: 4966 RVA: 0x0007808C File Offset: 0x0007628C
		public LevelNode[] Challenges
		{
			get
			{
				return this.mChallenges;
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06001367 RID: 4967 RVA: 0x00078094 File Offset: 0x00076294
		public LevelNode[] StoryChallanges
		{
			get
			{
				return this.mStoryChallenges;
			}
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06001368 RID: 4968 RVA: 0x0007809C File Offset: 0x0007629C
		public LevelNode[] Versus
		{
			get
			{
				return this.mVersus;
			}
		}

		// Token: 0x040014F2 RID: 5362
		private static LevelManager sSingelton;

		// Token: 0x040014F3 RID: 5363
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040014F4 RID: 5364
		private static Dictionary<string, int> LOC_LOOKUP;

		// Token: 0x040014F5 RID: 5365
		private HackHelper.Status mCampaignIsHacked;

		// Token: 0x040014F6 RID: 5366
		private HackHelper.License mMythosCampaignLicense;

		// Token: 0x040014F7 RID: 5367
		private CampaignNode[] mMythosCampaign;

		// Token: 0x040014F8 RID: 5368
		private CampaignNode[] mDungeonsCampaign;

		// Token: 0x040014F9 RID: 5369
		private CampaignNode[] mCampaign;

		// Token: 0x040014FA RID: 5370
		private LevelNode[] mChallenges;

		// Token: 0x040014FB RID: 5371
		private LevelNode[] mVersus;

		// Token: 0x040014FC RID: 5372
		private LevelNode[] mStoryChallenges;

		// Token: 0x040014FD RID: 5373
		private static readonly string[] hcStoryChallanges = new string[]
		{
			"ch_vietnam",
			"ch_osotc",
			"ch_dungeons_ch1",
			"ch_dungeons_ch2"
		};
	}
}
