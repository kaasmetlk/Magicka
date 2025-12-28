using System;
using System.Collections.Generic;
using System.IO;
using Magicka.Network;
using Magicka.Storage;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x0200042E RID: 1070
	internal struct SaveSlotInfo : ISendable
	{
		// Token: 0x0600212B RID: 8491 RVA: 0x000EBF14 File Offset: 0x000EA114
		public SaveSlotInfo(SaveData iSaveData)
		{
			this.mValid = true;
			this.mLooped = iSaveData.Looped;
			this.mUnlockedMagicks = iSaveData.UnlockedMagicks;
			this.mPlayers = iSaveData.Players;
			this.mShownTips = iSaveData.ShownTips;
		}

		// Token: 0x0600212C RID: 8492 RVA: 0x000EBF4D File Offset: 0x000EA14D
		public SaveSlotInfo(bool iLooped, ulong iUnlockedMagicks, Dictionary<string, PlayerSaveData> iPlayers, SaveData.tip[] iShownTips)
		{
			this.mValid = true;
			this.mLooped = iLooped;
			this.mUnlockedMagicks = iUnlockedMagicks;
			this.mPlayers = iPlayers;
			this.mShownTips = iShownTips;
		}

		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x0600212D RID: 8493 RVA: 0x000EBF73 File Offset: 0x000EA173
		public bool IsValid
		{
			get
			{
				return this.mValid;
			}
		}

		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x0600212E RID: 8494 RVA: 0x000EBF7B File Offset: 0x000EA17B
		public bool Looped
		{
			get
			{
				return this.mLooped;
			}
		}

		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x0600212F RID: 8495 RVA: 0x000EBF83 File Offset: 0x000EA183
		public Dictionary<string, PlayerSaveData> Players
		{
			get
			{
				return this.mPlayers;
			}
		}

		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x06002130 RID: 8496 RVA: 0x000EBF8B File Offset: 0x000EA18B
		public ulong UnlockedMagicks
		{
			get
			{
				return this.mUnlockedMagicks;
			}
		}

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x06002131 RID: 8497 RVA: 0x000EBF93 File Offset: 0x000EA193
		public SaveData.tip[] ShownTips
		{
			get
			{
				return this.mShownTips;
			}
		}

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x06002132 RID: 8498 RVA: 0x000EBF9B File Offset: 0x000EA19B
		public PacketType PacketType
		{
			get
			{
				return PacketType.SaveData;
			}
		}

		// Token: 0x06002133 RID: 8499 RVA: 0x000EBFA0 File Offset: 0x000EA1A0
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Looped);
			iWriter.Write(this.UnlockedMagicks);
			iWriter.Write(this.Players.Count);
			foreach (KeyValuePair<string, PlayerSaveData> keyValuePair in this.mPlayers)
			{
				iWriter.Write(keyValuePair.Key);
				keyValuePair.Value.Write(iWriter);
			}
			iWriter.Write(this.ShownTips.Length);
			for (int i = 0; i < this.ShownTips.Length; i++)
			{
				iWriter.Write(this.ShownTips[i].tipName);
				iWriter.Write(this.ShownTips[i].count);
			}
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x000EC080 File Offset: 0x000EA280
		public void Read(BinaryReader iReader)
		{
			this.mValid = true;
			this.mLooped = iReader.ReadBoolean();
			this.mUnlockedMagicks = iReader.ReadUInt64();
			int num = iReader.ReadInt32();
			this.mPlayers = new Dictionary<string, PlayerSaveData>(num);
			for (int i = 0; i < num; i++)
			{
				string key = iReader.ReadString();
				PlayerSaveData value = PlayerSaveData.Read(iReader);
				this.mPlayers.Add(key, value);
			}
			num = iReader.ReadInt32();
			this.mShownTips = new SaveData.tip[11];
			for (int j = 0; j < num; j++)
			{
				SaveData.tip tip;
				tip.tipName = iReader.ReadString();
				tip.timeStamp = double.MinValue;
				tip.count = iReader.ReadInt32();
				this.mShownTips[j] = tip;
			}
		}

		// Token: 0x040023C1 RID: 9153
		private bool mValid;

		// Token: 0x040023C2 RID: 9154
		private bool mLooped;

		// Token: 0x040023C3 RID: 9155
		private ulong mUnlockedMagicks;

		// Token: 0x040023C4 RID: 9156
		private Dictionary<string, PlayerSaveData> mPlayers;

		// Token: 0x040023C5 RID: 9157
		private SaveData.tip[] mShownTips;
	}
}
