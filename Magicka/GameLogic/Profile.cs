using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;
using Magicka.Achievements;
using Magicka.DRM;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Gamers;
using Magicka.Storage;
using Microsoft.Xna.Framework.Graphics;
using SteamWrapper;

namespace Magicka.GameLogic
{
	// Token: 0x02000645 RID: 1605
	public class Profile
	{
		// Token: 0x17000B76 RID: 2934
		// (get) Token: 0x060030AF RID: 12463 RVA: 0x0018EDD8 File Offset: 0x0018CFD8
		public static Profile Instance
		{
			get
			{
				if (Profile.sSingelton == null)
				{
					lock (Profile.sSingeltonLock)
					{
						if (Profile.sSingelton == null)
						{
							Profile.sSingelton = new Profile();
						}
					}
				}
				return Profile.sSingelton;
			}
		}

		// Token: 0x060030B0 RID: 12464 RVA: 0x0018EE2C File Offset: 0x0018D02C
		static Profile()
		{
			Profile.mMythosCreatures.Add("deep_one".GetHashCodeCustom());
			Profile.mMythosCreatures.Add("elder_thing".GetHashCodeCustom());
			Profile.mMythosCreatures.Add("enderman".GetHashCodeCustom());
			Profile.mMythosCreatures.Add("shoggoth".GetHashCodeCustom());
			Profile.mMythosCreatures.Add("starspawn".GetHashCodeCustom());
		}

		// Token: 0x060030B1 RID: 12465 RVA: 0x0018EEB4 File Offset: 0x0018D0B4
		private Profile()
		{
			this.mGamers = new SortedList<string, Gamer>();
			this.mFoundMoose = new Dictionary<string, bool>();
			this.mFoundSecretAreas = new Dictionary<string, bool>();
			this.mKilledPony = new Dictionary<string, bool>();
			this.mPlayerUsedElements = new Dictionary<string, Elements>();
		}

		// Token: 0x17000B77 RID: 2935
		// (get) Token: 0x060030B2 RID: 12466 RVA: 0x0018EF14 File Offset: 0x0018D114
		public Profile.PlayableAvatar DefaultAvatar
		{
			get
			{
				Profile.PlayableAvatar result;
				lock (this.mPlayables)
				{
					Profile.PlayableAvatar playableAvatar;
					if (!this.mPlayables.TryGetValue("wizard", out playableAvatar))
					{
						playableAvatar = Profile.PlayableAvatar.CreateDefault();
						this.mPlayables.Add(playableAvatar.Name, playableAvatar);
					}
					result = playableAvatar;
				}
				return result;
			}
		}

		// Token: 0x060030B3 RID: 12467 RVA: 0x0018EF78 File Offset: 0x0018D178
		public void Read()
		{
			lock (this.mPlayables)
			{
				FileInfo[] files = new DirectoryInfo("content/Data/Characters/Playable").GetFiles("*.xml");
				for (int i = 0; i < files.Length; i++)
				{
					Profile.PlayableAvatar value = new Profile.PlayableAvatar(files[i].FullName);
					if (!this.mPlayables.ContainsKey(value.Name))
					{
						this.mPlayables.Add(value.Name, value);
					}
				}
			}
			BinaryReader binaryReader = null;
			try
			{
				binaryReader = new BinaryReader(File.OpenRead("SaveData/Profile.sav"));
				string text = binaryReader.ReadString();
				string[] array = text.Split(new char[]
				{
					'.'
				});
				ushort[] array2 = new ushort[4];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = ushort.Parse(array[j]);
				}
				ulong num = (ulong)array2[0] << 48 | (ulong)array2[1] << 32 | (ulong)array2[2] << 16 | (ulong)array2[3];
				if (num >= 281492157038593UL)
				{
					this.Read1471(binaryReader);
				}
				else if (num >= 281492156973056UL)
				{
					this.Read1460(binaryReader);
				}
				else if (num >= 281492156710912UL)
				{
					this.Read1420(binaryReader);
				}
				else if (num >= 281492156579843UL)
				{
					this.Read1403(binaryReader);
				}
				else if (num >= 281487861940229UL)
				{
					this.Read1355(binaryReader);
				}
				else if (num >= 281483567235072UL)
				{
					this.Read1290(binaryReader);
				}
				binaryReader.Close();
			}
			catch
			{
				if (binaryReader != null)
				{
					binaryReader.Close();
				}
			}
			if (this.mLastGamer == null)
			{
				string personaName = SteamFriends.GetPersonaName();
				if (!this.mGamers.TryGetValue(personaName, out this.mLastGamer))
				{
					this.mLastGamer = new Gamer(personaName);
					this.Add(this.mLastGamer);
				}
			}
		}

		// Token: 0x060030B4 RID: 12468 RVA: 0x0018F17C File Offset: 0x0018D37C
		public void Read1290(BinaryReader iReader)
		{
			int num = iReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Gamer gamer = Gamer.Read(iReader);
				this.mGamers[gamer.GamerTag] = gamer;
			}
			num = iReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				string key = iReader.ReadString();
				bool value = iReader.ReadBoolean();
				this.mFoundMoose.Add(key, value);
			}
			num = iReader.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				string key2 = iReader.ReadString();
				Elements value2 = (Elements)iReader.ReadInt32();
				this.mPlayerUsedElements.Add(key2, value2);
			}
			num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				string key3 = iReader.ReadString();
				bool value3 = iReader.ReadBoolean();
				this.mFoundSecretAreas.Add(key3, value3);
			}
			this.mTotalAmountHealed = iReader.ReadSingle();
			this.mNumberOfOverKills = iReader.ReadInt32();
			string key4 = iReader.ReadString();
			this.mGamers.TryGetValue(key4, out this.mLastGamer);
		}

		// Token: 0x060030B5 RID: 12469 RVA: 0x0018F288 File Offset: 0x0018D488
		public void Read1355(BinaryReader iReader)
		{
			int num = iReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Gamer gamer = Gamer.Read(iReader);
				this.mGamers[gamer.GamerTag] = gamer;
			}
			num = iReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				string key = iReader.ReadString();
				bool value = iReader.ReadBoolean();
				this.mFoundMoose.Add(key, value);
			}
			num = iReader.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				string key2 = iReader.ReadString();
				Elements value2 = (Elements)iReader.ReadInt32();
				this.mPlayerUsedElements.Add(key2, value2);
			}
			num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				string key3 = iReader.ReadString();
				bool value3 = iReader.ReadBoolean();
				this.mFoundSecretAreas.Add(key3, value3);
			}
			this.mTotalAmountHealed = iReader.ReadSingle();
			this.mNumberOfOverKills = iReader.ReadInt32();
			this.mLedFarmerKills = iReader.ReadInt32();
			string key4 = iReader.ReadString();
			this.mGamers.TryGetValue(key4, out this.mLastGamer);
		}

		// Token: 0x060030B6 RID: 12470 RVA: 0x0018F3A0 File Offset: 0x0018D5A0
		public void Read1403(BinaryReader iReader)
		{
			int num = iReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Gamer gamer = Gamer.Read(iReader);
				this.mGamers[gamer.GamerTag] = gamer;
			}
			num = iReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				string key = iReader.ReadString();
				bool value = iReader.ReadBoolean();
				this.mFoundMoose.Add(key, value);
			}
			num = iReader.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				string key2 = iReader.ReadString();
				Elements value2 = (Elements)iReader.ReadInt32();
				this.mPlayerUsedElements.Add(key2, value2);
			}
			num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				string key3 = iReader.ReadString();
				bool value3 = iReader.ReadBoolean();
				this.mFoundSecretAreas.Add(key3, value3);
			}
			this.mTotalAmountHealed = iReader.ReadSingle();
			this.mNumberOfOverKills = iReader.ReadInt32();
			this.mLedFarmerKills = iReader.ReadInt32();
			string key4 = iReader.ReadString();
			this.mGamers.TryGetValue(key4, out this.mLastGamer);
			this.mConsectuiveCruiseCount = iReader.ReadInt32();
			this.mLastCruiseDate = DateTime.FromBinary(iReader.ReadInt64());
		}

		// Token: 0x060030B7 RID: 12471 RVA: 0x0018F4D4 File Offset: 0x0018D6D4
		public void Read1420(BinaryReader iReader)
		{
			int num = iReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Gamer gamer = Gamer.Read(iReader);
				this.mGamers[gamer.GamerTag] = gamer;
			}
			num = iReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				string key = iReader.ReadString();
				bool value = iReader.ReadBoolean();
				this.mFoundMoose.Add(key, value);
			}
			num = iReader.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				string key2 = iReader.ReadString();
				Elements value2 = (Elements)iReader.ReadInt32();
				this.mPlayerUsedElements.Add(key2, value2);
			}
			num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				string key3 = iReader.ReadString();
				bool value3 = iReader.ReadBoolean();
				this.mFoundSecretAreas.Add(key3, value3);
			}
			this.mTotalAmountHealed = iReader.ReadSingle();
			this.mNumberOfOverKills = iReader.ReadInt32();
			this.mLedFarmerKills = iReader.ReadInt32();
			string key4 = iReader.ReadString();
			this.mGamers.TryGetValue(key4, out this.mLastGamer);
			this.mConsectuiveCruiseCount = iReader.ReadInt32();
			this.mLastCruiseDate = DateTime.FromBinary(iReader.ReadInt64());
			this.mBanisherOfHorrors = iReader.ReadInt32();
		}

		// Token: 0x060030B8 RID: 12472 RVA: 0x0018F614 File Offset: 0x0018D814
		public void Read1460(BinaryReader iReader)
		{
			int num = iReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Gamer gamer = Gamer.Read(iReader);
				this.mGamers[gamer.GamerTag] = gamer;
			}
			num = iReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				string key = iReader.ReadString();
				bool value = iReader.ReadBoolean();
				this.mFoundMoose.Add(key, value);
			}
			num = iReader.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				string key2 = iReader.ReadString();
				Elements value2 = (Elements)iReader.ReadInt32();
				this.mPlayerUsedElements.Add(key2, value2);
			}
			num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				string key3 = iReader.ReadString();
				bool value3 = iReader.ReadBoolean();
				this.mFoundSecretAreas.Add(key3, value3);
			}
			this.mTotalAmountHealed = iReader.ReadSingle();
			this.mNumberOfOverKills = iReader.ReadInt32();
			this.mNumberOfFrozenOverKills = iReader.ReadInt32();
			this.mLedFarmerKills = iReader.ReadInt32();
			string key4 = iReader.ReadString();
			this.mGamers.TryGetValue(key4, out this.mLastGamer);
			this.mConsectuiveCruiseCount = iReader.ReadInt32();
			this.mLastCruiseDate = DateTime.FromBinary(iReader.ReadInt64());
			this.mBanisherOfHorrors = iReader.ReadInt32();
		}

		// Token: 0x060030B9 RID: 12473 RVA: 0x0018F760 File Offset: 0x0018D960
		public void Read1471(BinaryReader iReader)
		{
			int num = iReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Gamer gamer = Gamer.Read(iReader);
				this.mGamers[gamer.GamerTag] = gamer;
			}
			num = iReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				string key = iReader.ReadString();
				bool value = iReader.ReadBoolean();
				this.mFoundMoose.Add(key, value);
			}
			num = iReader.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				string key2 = iReader.ReadString();
				Elements value2 = (Elements)iReader.ReadInt32();
				this.mPlayerUsedElements.Add(key2, value2);
			}
			num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				string key3 = iReader.ReadString();
				bool value3 = iReader.ReadBoolean();
				this.mFoundSecretAreas.Add(key3, value3);
			}
			this.mTotalAmountHealed = iReader.ReadSingle();
			this.mNumberOfOverKills = iReader.ReadInt32();
			this.mNumberOfFrozenOverKills = iReader.ReadInt32();
			this.mLedFarmerKills = iReader.ReadInt32();
			string key4 = iReader.ReadString();
			this.mGamers.TryGetValue(key4, out this.mLastGamer);
			this.mConsectuiveCruiseCount = iReader.ReadInt32();
			this.mLastCruiseDate = DateTime.FromBinary(iReader.ReadInt64());
			this.mBanisherOfHorrors = iReader.ReadInt32();
			num = iReader.ReadInt32();
			for (int m = 0; m < num; m++)
			{
				string key5 = iReader.ReadString();
				bool value4 = iReader.ReadBoolean();
				this.mKilledPony.Add(key5, value4);
			}
		}

		// Token: 0x060030BA RID: 12474 RVA: 0x0018F8E4 File Offset: 0x0018DAE4
		public void Write()
		{
			BinaryWriter binaryWriter = null;
			try
			{
				binaryWriter = new BinaryWriter(File.Create("SaveData/Profile.sav"));
				binaryWriter.Write(Application.ProductVersion);
				binaryWriter.Write(this.mGamers.Count);
				foreach (Gamer gamer in this.mGamers.Values)
				{
					gamer.Write(binaryWriter);
				}
				binaryWriter.Write(this.mFoundMoose.Count);
				for (int i = 0; i < this.mFoundMoose.Count; i++)
				{
					binaryWriter.Write(this.mFoundMoose.ElementAt(i).Key);
					binaryWriter.Write(this.mFoundMoose.ElementAt(i).Value);
				}
				binaryWriter.Write(this.mPlayerUsedElements.Count);
				for (int j = 0; j < this.mPlayerUsedElements.Count; j++)
				{
					binaryWriter.Write(this.mPlayerUsedElements.ElementAt(j).Key);
					binaryWriter.Write((int)this.mPlayerUsedElements.ElementAt(j).Value);
				}
				binaryWriter.Write(this.mFoundSecretAreas.Count);
				for (int k = 0; k < this.mFoundSecretAreas.Count; k++)
				{
					binaryWriter.Write(this.mFoundSecretAreas.ElementAt(k).Key);
					binaryWriter.Write(this.mFoundSecretAreas.ElementAt(k).Value);
				}
				binaryWriter.Write(this.mTotalAmountHealed);
				binaryWriter.Write(this.mNumberOfOverKills);
				binaryWriter.Write(this.mNumberOfFrozenOverKills);
				binaryWriter.Write(this.mLedFarmerKills);
				binaryWriter.Write(this.mLastGamer.GamerTag);
				binaryWriter.Write(this.mConsectuiveCruiseCount);
				binaryWriter.Write(this.mLastCruiseDate.ToBinary());
				binaryWriter.Write(this.mBanisherOfHorrors);
				binaryWriter.Write(this.mKilledPony.Count);
				for (int l = 0; l < this.mKilledPony.Count; l++)
				{
					binaryWriter.Write(this.mKilledPony.ElementAt(l).Key);
					binaryWriter.Write(this.mKilledPony.ElementAt(l).Value);
				}
				binaryWriter.Close();
			}
			catch
			{
				if (binaryWriter != null)
				{
					binaryWriter.Close();
				}
			}
		}

		// Token: 0x17000B78 RID: 2936
		// (get) Token: 0x060030BB RID: 12475 RVA: 0x0018FB8C File Offset: 0x0018DD8C
		public SortedList<string, Profile.PlayableAvatar> Avatars
		{
			get
			{
				return this.mPlayables;
			}
		}

		// Token: 0x17000B79 RID: 2937
		// (get) Token: 0x060030BC RID: 12476 RVA: 0x0018FB94 File Offset: 0x0018DD94
		public SortedList<string, Gamer> Gamers
		{
			get
			{
				return this.mGamers;
			}
		}

		// Token: 0x060030BD RID: 12477 RVA: 0x0018FB9C File Offset: 0x0018DD9C
		internal void ChangeName(Gamer iGamer, string iName)
		{
			for (int i = 0; i < SaveManager.Instance.SaveSlots.Length; i++)
			{
				PlayerSaveData value;
				if (SaveManager.Instance.SaveSlots[i] != null && SaveManager.Instance.SaveSlots[i].Players.TryGetValue(iGamer.GamerTag, out value))
				{
					SaveManager.Instance.SaveSlots[i].Players.Remove(iGamer.GamerTag);
					SaveManager.Instance.SaveSlots[i].Players.Add(iName, value);
				}
			}
			this.mGamers.Remove(iGamer.GamerTag);
			iGamer.GamerTag = iName;
			this.Add(iGamer);
		}

		// Token: 0x060030BE RID: 12478 RVA: 0x0018FC43 File Offset: 0x0018DE43
		public void Add(Gamer iGamer)
		{
			this.mGamers[iGamer.GamerTag] = iGamer;
			this.Write();
		}

		// Token: 0x060030BF RID: 12479 RVA: 0x0018FC60 File Offset: 0x0018DE60
		public bool Remove(string iGamerTag)
		{
			for (int i = 0; i < SaveManager.Instance.SaveSlots.Length; i++)
			{
				PlayerSaveData playerSaveData;
				if (SaveManager.Instance.SaveSlots[i] != null && SaveManager.Instance.SaveSlots[i].Players.TryGetValue(iGamerTag, out playerSaveData))
				{
					SaveManager.Instance.SaveSlots[i].Players.Remove(iGamerTag);
				}
			}
			bool flag = this.mGamers.Remove(iGamerTag);
			if (flag)
			{
				this.Write();
			}
			return flag;
		}

		// Token: 0x17000B7A RID: 2938
		// (get) Token: 0x060030C0 RID: 12480 RVA: 0x0018FCDC File Offset: 0x0018DEDC
		// (set) Token: 0x060030C1 RID: 12481 RVA: 0x0018FCE4 File Offset: 0x0018DEE4
		public Gamer LastGamer
		{
			get
			{
				return this.mLastGamer;
			}
			set
			{
				this.mLastGamer = value;
			}
		}

		// Token: 0x060030C2 RID: 12482 RVA: 0x0018FCF0 File Offset: 0x0018DEF0
		public void FoundAMoose(PlayState iPlayState, string iID)
		{
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			this.mFoundMoose[iID] = true;
			int num = 0;
			foreach (bool flag in this.mFoundMoose.Values)
			{
				if (flag)
				{
					num++;
				}
			}
			SteamUserStats.IndicateAchievementProgress("kingsquest", (uint)num, 12U);
			SteamUserStats.StoreStats();
			if (num == 12)
			{
				AchievementsManager.Instance.AwardAchievement(iPlayState, "kingsquest");
			}
		}

		// Token: 0x060030C3 RID: 12483 RVA: 0x0018FD8C File Offset: 0x0018DF8C
		public void FoundSecretArea(PlayState iPlayState, string iID)
		{
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			this.mFoundSecretAreas[iID] = true;
			int num = 0;
			foreach (bool flag in this.mFoundSecretAreas.Values)
			{
				if (flag)
				{
					num++;
				}
			}
			SteamUserStats.IndicateAchievementProgress("sherlockholmes", (uint)num, 14U);
			SteamUserStats.StoreStats();
			if (num == 14)
			{
				AchievementsManager.Instance.AwardAchievement(iPlayState, "sherlockholmes");
			}
		}

		// Token: 0x060030C4 RID: 12484 RVA: 0x0018FE28 File Offset: 0x0018E028
		public void KilledAPony(PlayState iPlayState, string iID)
		{
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			this.mKilledPony[iID] = true;
			int num = 0;
			foreach (bool flag in this.mKilledPony.Values)
			{
				if (flag)
				{
					num++;
				}
			}
			SteamUserStats.IndicateAchievementProgress("friendship", (uint)num, 11U);
			SteamUserStats.StoreStats();
			if (num == 11)
			{
				AchievementsManager.Instance.AwardAchievement(iPlayState, "friendship");
			}
		}

		// Token: 0x060030C5 RID: 12485 RVA: 0x0018FEC4 File Offset: 0x0018E0C4
		public void UsedElements(PlayState iPlayState, string iTag, Elements iElements)
		{
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			Elements elements;
			if (!this.mPlayerUsedElements.TryGetValue(iTag, out elements))
			{
				this.mPlayerUsedElements.Add(iTag, iElements);
			}
			else
			{
				Dictionary<string, Elements> dictionary;
				(dictionary = this.mPlayerUsedElements)[iTag] = (dictionary[iTag] | iElements);
			}
			if ((this.mPlayerUsedElements[iTag] & Elements.Basic) == Elements.Basic)
			{
				AchievementsManager.Instance.AwardAchievement(iPlayState, "basicelement");
			}
		}

		// Token: 0x060030C6 RID: 12486 RVA: 0x0018FF3B File Offset: 0x0018E13B
		public void AddHealedAmount(PlayState iPlayState, float iAmount)
		{
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			this.mTotalAmountHealed += -iAmount;
			if (this.mTotalAmountHealed >= 100000f)
			{
				AchievementsManager.Instance.AwardAchievement(iPlayState, "killingyourfriendsyo");
			}
		}

		// Token: 0x060030C7 RID: 12487 RVA: 0x0018FF74 File Offset: 0x0018E174
		public void AddOverKills(Character iVictim)
		{
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			if (iVictim.HasStatus(StatusEffects.Frozen))
			{
				this.AddFrozenOverKill(iVictim.PlayState);
			}
			this.mNumberOfOverKills++;
			if (this.mNumberOfOverKills < 1000 && this.mNumberOfOverKills % 25 == 0)
			{
				SteamUserStats.IndicateAchievementProgress("badtaste", (uint)Math.Min(this.mNumberOfOverKills, 1000), 1000U);
				SteamUserStats.StoreStats();
			}
			if (this.mNumberOfOverKills >= 1000)
			{
				AchievementsManager.Instance.AwardAchievement(iVictim.PlayState, "badtaste");
			}
		}

		// Token: 0x060030C8 RID: 12488 RVA: 0x00190010 File Offset: 0x0018E210
		private void AddFrozenOverKill(PlayState iPlayState)
		{
			this.mNumberOfFrozenOverKills++;
			if (this.mNumberOfFrozenOverKills == 1 || (this.mNumberOfFrozenOverKills < 100 && this.mNumberOfFrozenOverKills % 10 == 0))
			{
				SteamUserStats.IndicateAchievementProgress("iceage", (uint)Math.Min(this.mNumberOfFrozenOverKills, 100), 100U);
				SteamUserStats.StoreStats();
			}
			if (this.mNumberOfFrozenOverKills >= 100)
			{
				AchievementsManager.Instance.AwardAchievement(iPlayState, "iceage");
			}
		}

		// Token: 0x060030C9 RID: 12489 RVA: 0x00190084 File Offset: 0x0018E284
		public void AddMythosKill(PlayState iPlayState, Character iTarget)
		{
			if (iTarget == null)
			{
				return;
			}
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			if (SubMenuCharacterSelect.Instance.GameType == GameType.Mythos && Profile.mMythosCreatures.Contains(iTarget.Template.ID))
			{
				this.mBanisherOfHorrors++;
				Console.WriteLine(this.mBanisherOfHorrors.ToString());
				if (this.mBanisherOfHorrors >= 1000)
				{
					AchievementsManager.Instance.AwardAchievement(iPlayState, "banisherofhorrors");
					return;
				}
				if (this.mBanisherOfHorrors == 1 || this.mBanisherOfHorrors % 25 == 0)
				{
					SteamUserStats.IndicateAchievementProgress("banisherofhorrors", (uint)Math.Min(this.mBanisherOfHorrors, 1000), 1000U);
					SteamUserStats.StoreStats();
				}
			}
		}

		// Token: 0x060030CA RID: 12490 RVA: 0x0019013C File Offset: 0x0018E33C
		public void AddLedKill(PlayState iPlayState)
		{
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			this.mLedFarmerKills++;
			if (this.mLedFarmerKills >= 1000)
			{
				AchievementsManager.Instance.AwardAchievement(iPlayState, "ledfarmer");
				return;
			}
			if (this.mLedFarmerKills % 25 == 0)
			{
				SteamUserStats.IndicateAchievementProgress("ledfarmer", (uint)Math.Min(this.mLedFarmerKills, 1000), 1000U);
				SteamUserStats.StoreStats();
			}
		}

		// Token: 0x060030CB RID: 12491 RVA: 0x001901B0 File Offset: 0x0018E3B0
		public void PlayingIslandCruise(PlayState iPlayState)
		{
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			DateTime now = DateTime.Now;
			int num = now.Day - this.mLastCruiseDate.Day;
			if (num == 1)
			{
				this.mConsectuiveCruiseCount++;
				if (this.mConsectuiveCruiseCount >= 7)
				{
					AchievementsManager.Instance.AwardAchievement(iPlayState, "daycruise");
				}
				else
				{
					SteamUserStats.IndicateAchievementProgress("daycruise", (uint)this.mConsectuiveCruiseCount, 7U);
					SteamUserStats.StoreStats();
				}
			}
			else if (num >= 2)
			{
				this.mConsectuiveCruiseCount = 0;
			}
			this.mLastCruiseDate = now;
		}

		// Token: 0x060030CC RID: 12492 RVA: 0x0019023C File Offset: 0x0018E43C
		internal Profile.PlayableAvatar GetAvatar(string iName)
		{
			Profile.PlayableAvatar result;
			if (this.mPlayables.TryGetValue(iName, out result))
			{
				return result;
			}
			return this.DefaultAvatar;
		}

		// Token: 0x040034AB RID: 13483
		public const string SAVE_PATH = "SaveData/Profile.sav";

		// Token: 0x040034AC RID: 13484
		private static Profile sSingelton;

		// Token: 0x040034AD RID: 13485
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040034AE RID: 13486
		private SortedList<string, Gamer> mGamers = new SortedList<string, Gamer>();

		// Token: 0x040034AF RID: 13487
		private Dictionary<string, bool> mFoundMoose;

		// Token: 0x040034B0 RID: 13488
		private Dictionary<string, bool> mFoundSecretAreas;

		// Token: 0x040034B1 RID: 13489
		private Dictionary<string, bool> mKilledPony;

		// Token: 0x040034B2 RID: 13490
		private Dictionary<string, Elements> mPlayerUsedElements;

		// Token: 0x040034B3 RID: 13491
		private float mTotalAmountHealed;

		// Token: 0x040034B4 RID: 13492
		private int mNumberOfOverKills;

		// Token: 0x040034B5 RID: 13493
		private int mNumberOfFrozenOverKills;

		// Token: 0x040034B6 RID: 13494
		private int mLedFarmerKills;

		// Token: 0x040034B7 RID: 13495
		private int mBanisherOfHorrors;

		// Token: 0x040034B8 RID: 13496
		private Gamer mLastGamer;

		// Token: 0x040034B9 RID: 13497
		private static readonly List<int> mMythosCreatures = new List<int>();

		// Token: 0x040034BA RID: 13498
		private int mConsectuiveCruiseCount;

		// Token: 0x040034BB RID: 13499
		private DateTime mLastCruiseDate;

		// Token: 0x040034BC RID: 13500
		private SortedList<string, Profile.PlayableAvatar> mPlayables = new SortedList<string, Profile.PlayableAvatar>();

		// Token: 0x02000646 RID: 1606
		public struct PlayableAvatar
		{
			// Token: 0x060030CD RID: 12493 RVA: 0x00190264 File Offset: 0x0018E464
			internal PlayableAvatar(string iFileName)
			{
				this = default(Profile.PlayableAvatar);
				this.Name = Path.GetFileNameWithoutExtension(iFileName).ToLowerInvariant();
				this.AppID = 42910U;
				FileStream fileStream = File.OpenRead(iFileName);
				this.mHashSum = Profile.PlayableAvatar.sHasher.ComputeHash(fileStream);
				fileStream.Position = 0L;
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(fileStream);
				fileStream.Close();
				XmlNode xmlNode = null;
				for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
				{
					XmlNode xmlNode2 = xmlDocument.ChildNodes[i];
					if (!(xmlNode2 is XmlComment) && xmlNode2.Name.Equals("PlayableCharacter", StringComparison.OrdinalIgnoreCase))
					{
						xmlNode = xmlNode2;
					}
				}
				for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
				{
					XmlNode xmlNode3 = xmlNode.ChildNodes[j];
					if (!(xmlNode3 is XmlComment))
					{
						if (xmlNode3.Name.Equals("DisplayName", StringComparison.OrdinalIgnoreCase))
						{
							string iString = xmlNode3.InnerText.ToLowerInvariant();
							this.mDisplayName = iString.GetHashCodeCustom();
						}
						else if (xmlNode3.Name.Equals("Description", StringComparison.OrdinalIgnoreCase))
						{
							string iString2 = xmlNode3.InnerText.ToLowerInvariant();
							this.mDescription = iString2.GetHashCodeCustom();
						}
						else
						{
							if (xmlNode3.Name.Equals("thumb", StringComparison.OrdinalIgnoreCase))
							{
								this.ThumbPath = xmlNode3.InnerText;
								lock (Game.Instance.GraphicsDevice)
								{
									this.Thumb = Game.Instance.Content.Load<Texture2D>(this.ThumbPath);
									goto IL_2B7;
								}
							}
							if (xmlNode3.Name.Equals("portrait", StringComparison.OrdinalIgnoreCase))
							{
								this.PortraitPath = xmlNode3.InnerText;
								lock (Game.Instance.GraphicsDevice)
								{
									this.Portrait = Game.Instance.Content.Load<Texture2D>(this.PortraitPath);
									goto IL_2B7;
								}
							}
							if (xmlNode3.Name.Equals("character", StringComparison.OrdinalIgnoreCase))
							{
								this.TypeName = xmlNode3.InnerText.ToLowerInvariant();
								this.Type = this.TypeName.GetHashCodeCustom();
							}
							else if (xmlNode3.Name.Equals("allowCampaign", StringComparison.OrdinalIgnoreCase))
							{
								this.AllowCampaign = bool.Parse(xmlNode3.InnerText);
							}
							else if (xmlNode3.Name.Equals("allowChallenge", StringComparison.OrdinalIgnoreCase))
							{
								this.AllowChallenge = bool.Parse(xmlNode3.InnerText);
							}
							else if (xmlNode3.Name.Equals("allowPVP", StringComparison.OrdinalIgnoreCase))
							{
								this.AllowPVP = bool.Parse(xmlNode3.InnerText);
							}
							else if (xmlNode3.Name.Equals("appid", StringComparison.OrdinalIgnoreCase))
							{
								this.AppID = uint.Parse(xmlNode3.InnerText);
							}
						}
					}
					IL_2B7:;
				}
			}

			// Token: 0x060030CE RID: 12494 RVA: 0x0019055C File Offset: 0x0018E75C
			internal static Profile.PlayableAvatar CreateDefault()
			{
				Profile.PlayableAvatar result;
				result.Name = "wizard";
				result.ThumbPath = "Models/Characters/Wizard/Thumb";
				lock (Game.Instance.GraphicsDevice)
				{
					result.Thumb = Game.Instance.Content.Load<Texture2D>(result.ThumbPath);
				}
				result.PortraitPath = "Models/Characters/Wizard/Thumb_Portrait";
				lock (Game.Instance.GraphicsDevice)
				{
					result.Portrait = Game.Instance.Content.Load<Texture2D>(result.PortraitPath);
				}
				result.AllowCampaign = true;
				result.AllowChallenge = true;
				result.AllowPVP = true;
				result.AppID = 42910U;
				result.mHashSum = null;
				result.TypeName = "wizard";
				result.Type = result.TypeName.GetHashCodeCustom();
				result.mDisplayName = "#tooltip_avatar_vanilla".GetHashCodeCustom();
				result.mDescription = "#tooltip_avatar_vanillad".GetHashCodeCustom();
				return result;
			}

			// Token: 0x17000B7B RID: 2939
			// (get) Token: 0x060030CF RID: 12495 RVA: 0x00190688 File Offset: 0x0018E888
			internal byte[] HashSum
			{
				get
				{
					return this.mHashSum;
				}
			}

			// Token: 0x17000B7C RID: 2940
			// (get) Token: 0x060030D0 RID: 12496 RVA: 0x00190690 File Offset: 0x0018E890
			public int DisplayName
			{
				get
				{
					return this.mDisplayName;
				}
			}

			// Token: 0x17000B7D RID: 2941
			// (get) Token: 0x060030D1 RID: 12497 RVA: 0x00190698 File Offset: 0x0018E898
			public int Description
			{
				get
				{
					return this.mDescription;
				}
			}

			// Token: 0x060030D2 RID: 12498 RVA: 0x001906A0 File Offset: 0x0018E8A0
			public bool Equals(Profile.PlayableAvatar rhs)
			{
				return this == rhs;
			}

			// Token: 0x060030D3 RID: 12499 RVA: 0x001906AE File Offset: 0x0018E8AE
			public static bool operator ==(Profile.PlayableAvatar a, Profile.PlayableAvatar b)
			{
				return object.Equals(a, b) || a.GetHashCode() == b.GetHashCode();
			}

			// Token: 0x060030D4 RID: 12500 RVA: 0x001906E1 File Offset: 0x0018E8E1
			public static bool operator !=(Profile.PlayableAvatar a, Profile.PlayableAvatar b)
			{
				return !(a == b);
			}

			// Token: 0x040034BD RID: 13501
			private static SHA256 sHasher = SHA256.Create();

			// Token: 0x040034BE RID: 13502
			internal string Name;

			// Token: 0x040034BF RID: 13503
			internal string ThumbPath;

			// Token: 0x040034C0 RID: 13504
			internal Texture2D Thumb;

			// Token: 0x040034C1 RID: 13505
			internal string PortraitPath;

			// Token: 0x040034C2 RID: 13506
			internal Texture2D Portrait;

			// Token: 0x040034C3 RID: 13507
			internal string TypeName;

			// Token: 0x040034C4 RID: 13508
			internal int Type;

			// Token: 0x040034C5 RID: 13509
			internal bool AllowChallenge;

			// Token: 0x040034C6 RID: 13510
			internal bool AllowPVP;

			// Token: 0x040034C7 RID: 13511
			internal bool AllowCampaign;

			// Token: 0x040034C8 RID: 13512
			internal uint AppID;

			// Token: 0x040034C9 RID: 13513
			private byte[] mHashSum;

			// Token: 0x040034CA RID: 13514
			private int mDisplayName;

			// Token: 0x040034CB RID: 13515
			private int mDescription;
		}
	}
}
