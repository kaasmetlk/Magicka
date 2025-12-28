using System;
using System.Collections.Generic;
using Magicka.DRM;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteamWrapper;

namespace Magicka.Levels.Packs
{
	// Token: 0x020004FF RID: 1279
	internal class PackMan
	{
		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x060025DA RID: 9690 RVA: 0x00111B88 File Offset: 0x0010FD88
		public static PackMan Instance
		{
			get
			{
				if (PackMan.sSingelton == null)
				{
					lock (PackMan.sSingeltonLock)
					{
						if (PackMan.sSingelton == null)
						{
							PackMan.sSingelton = new PackMan();
						}
					}
				}
				return PackMan.sSingelton;
			}
		}

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060025DB RID: 9691 RVA: 0x00111BDC File Offset: 0x0010FDDC
		// (remove) Token: 0x060025DC RID: 9692 RVA: 0x00111BF5 File Offset: 0x0010FDF5
		public event Action<object, bool> PackEnabledChanged;

		// Token: 0x060025DD RID: 9693 RVA: 0x00111C10 File Offset: 0x0010FE10
		private PackMan()
		{
			lock (Game.Instance.GraphicsDevice)
			{
				this.mTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/PackThumbs");
			}
			List<ItemPack> list = new List<ItemPack>();
			list.Add(new ItemPack(list.Count, "#pack_vanilla_sticks", "#pack_vanilla_sticksd", this.mTexture, new Point(2, 0), 0U, HashTable.VanillaSticks));
			list.Add(new ItemPack(list.Count, "#pack_ward_staffs", "#pack_ward_staffsd", this.mTexture, new Point(7, 0), 0U, HashTable.WardStaffs));
			list.Add(new ItemPack(list.Count, "#pack_purple_pack", "#pack_purple_packd", this.mTexture, new Point(8, 0), 0U, HashTable.PurplePack));
			list.Add(new ItemPack(list.Count, "#pack_meaky_peaky", "#pack_meaky_peakyd", this.mTexture, new Point(6, 0), 0U, HashTable.MeakyPeaky));
			list.Add(new ItemPack(list.Count, "#pack_heavy_hitters", "#pack_heavy_hittersd", this.mTexture, new Point(5, 0), 0U, HashTable.HeavyHitters));
			list.Add(new ItemPack(list.Count, "#pack_power_pack", "#pack_power_packd", this.mTexture, new Point(4, 0), 0U, HashTable.PowerPack));
			list.Add(new ItemPack(list.Count, "#pack_vietnam_gear", "#pack_vietnam_geard", this.mTexture, new Point(3, 0), 42918U, HashTable.VietnamItems));
			list.Add(new ItemPack(list.Count, "#pack_vanilla_weapons", "#pack_vanilla_weaponsd", this.mTexture, new Point(1, 0), 0U, HashTable.VanillaWeapons));
			list.Add(new ItemPack(list.Count, "#pack_holiday_spirit", "#pack_holiday_spiritd", this.mTexture, new Point(14, 0), 73091U, HashTable.HolidayItems));
			list.Add(new ItemPack(list.Count, "#pack_horror_props", "#pack_horror_propsd", this.mTexture, new Point(15, 0), 73092U, HashTable.HorrorItems));
			list.Add(new ItemPack(list.Count, "#pack_gadgets", "#pack_gadgetsd", this.mTexture, new Point(1, 1), 73097U, HashTable.GadgetPack));
			list.Add(new ItemPack(list.Count, "#pack_heirlooms", "#pack_heirloomsd", this.mTexture, new Point(2, 1), 73098U, HashTable.HeirloomPack));
			list.Add(new ItemPack(list.Count, "#PACK_SPORTING_GOODS", "#PACK_SPORTING_GOODSD", this.mTexture, new Point(3, 1), 42910U, HashTable.SportPack));
			List<MagickPack> list2 = new List<MagickPack>();
			list2.Add(new MagickPack(list2.Count, "#mpack_basic", "#mpack_basicd", this.mTexture, new Point(11, 0), 0U, new MagickType[]
			{
				MagickType.Revive,
				MagickType.Grease,
				MagickType.Haste,
				MagickType.Invisibility,
				MagickType.Teleport,
				MagickType.Rain,
				MagickType.Charm,
				MagickType.TimeWarp,
				MagickType.SPhoenix,
				MagickType.Nullify,
				MagickType.Corporealize,
				MagickType.Confuse
			}));
			list2.Add(new MagickPack(list2.Count, "#mpack_advanced", "#mpack_advancedd", this.mTexture, new Point(12, 0), 0U, new MagickType[]
			{
				MagickType.Fear,
				MagickType.ThunderB,
				MagickType.Tornado,
				MagickType.Blizzard,
				MagickType.Conflagration
			}));
			list2.Add(new MagickPack(list2.Count, "#mpack_too_advanced", "#mpack_too_advancedd", this.mTexture, new Point(13, 0), 0U, new MagickType[]
			{
				MagickType.CTD,
				MagickType.SDeath,
				MagickType.SUndead,
				MagickType.SElemental,
				MagickType.Vortex,
				MagickType.ThunderS
			}));
			list2.Add(new MagickPack(list2.Count, "#mpack_napalm", "#mpack_napalmd", this.mTexture, new Point(9, 0), 42918U, new MagickType[]
			{
				MagickType.Napalm
			}));
			list2.Add(new MagickPack(list2.Count, "#mpack_meteor_shower", "#mpack_meteor_showerd", this.mTexture, new Point(10, 0), 73030U, new MagickType[]
			{
				MagickType.MeteorS
			}));
			list2.Add(new MagickPack(list2.Count, "#MPACK_ATHLETIC_TECHNIQUES", "#MPACK_ATHLETIC_TECHNIQUESD", this.mTexture, new Point(4, 1), 0U, new MagickType[]
			{
				MagickType.JudgementSpray,
				MagickType.PerformanceEnchantment,
				MagickType.Wave
			}));
			this.mItemPacks = list.ToArray();
			this.mMagickPacks = list2.ToArray();
			this.mAllPacks = new IPack[this.mItemPacks.Length + this.mMagickPacks.Length];
			for (int i = 0; i < this.mItemPacks.Length; i++)
			{
				this.mAllPacks[i] = this.mItemPacks[i];
			}
			for (int j = 0; j < this.mMagickPacks.Length; j++)
			{
				this.mAllPacks[j + this.mItemPacks.Length] = this.mMagickPacks[j];
			}
			SteamAPI.DlcInstalled += this.SteamAPI_DlcInstalled;
			this.SteamAPI_DlcInstalled(default(DlcInstalled));
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x0011216C File Offset: 0x0011036C
		private void SteamAPI_DlcInstalled(DlcInstalled obj)
		{
			Game.Instance.AddLoadTask(new Action(this.UpdatePackLicense));
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x00112184 File Offset: 0x00110384
		internal void UpdatePackLicense()
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				for (int i = 0; i < this.mItemPacks.Length; i++)
				{
					this.mItemPacks[i].License = HackHelper.CheckLicense(this.mItemPacks[i]);
				}
				for (int j = 0; j < this.mMagickPacks.Length; j++)
				{
					this.mMagickPacks[j].License = HackHelper.CheckLicense(this.mMagickPacks[j]);
				}
			}
		}

		// Token: 0x060025E0 RID: 9696 RVA: 0x001121F8 File Offset: 0x001103F8
		internal void OnPackEnabledChanged(object iSender, bool iEnabled)
		{
			if (this.PackEnabledChanged != null)
			{
				this.PackEnabledChanged.Invoke(iSender, iEnabled);
			}
		}

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x060025E1 RID: 9697 RVA: 0x0011220F File Offset: 0x0011040F
		public Texture2D Thumbnails
		{
			get
			{
				return this.mTexture;
			}
		}

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x060025E2 RID: 9698 RVA: 0x00112217 File Offset: 0x00110417
		public ItemPack[] ItemPacks
		{
			get
			{
				return this.mItemPacks;
			}
		}

		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x060025E3 RID: 9699 RVA: 0x0011221F File Offset: 0x0011041F
		public MagickPack[] MagickPacks
		{
			get
			{
				return this.mMagickPacks;
			}
		}

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x060025E4 RID: 9700 RVA: 0x00112227 File Offset: 0x00110427
		public IPack[] AllPacks
		{
			get
			{
				return this.mAllPacks;
			}
		}

		// Token: 0x060025E5 RID: 9701 RVA: 0x0011222F File Offset: 0x0011042F
		public ItemPack GetItemPackByHandle(int iID)
		{
			if (iID < 0 || iID >= this.mItemPacks.Length)
			{
				return null;
			}
			return this.mItemPacks[iID];
		}

		// Token: 0x060025E6 RID: 9702 RVA: 0x0011224A File Offset: 0x0011044A
		public MagickPack GetMagickPackByHandle(int iID)
		{
			if (iID < 0 || iID >= this.mMagickPacks.Length)
			{
				return null;
			}
			return this.mMagickPacks[iID];
		}

		// Token: 0x04002924 RID: 10532
		public const string STORE_URL = "http://store.steampowered.com/app/{0}/";

		// Token: 0x04002925 RID: 10533
		private static PackMan sSingelton;

		// Token: 0x04002926 RID: 10534
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002928 RID: 10536
		private Texture2D mTexture;

		// Token: 0x04002929 RID: 10537
		private ItemPack[] mItemPacks;

		// Token: 0x0400292A RID: 10538
		private MagickPack[] mMagickPacks;

		// Token: 0x0400292B RID: 10539
		private IPack[] mAllPacks;
	}
}
