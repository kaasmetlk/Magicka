// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Packs.PackMan
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteamWrapper;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Levels.Packs;

internal class PackMan
{
  public const string STORE_URL = "http://store.steampowered.com/app/{0}/";
  private static PackMan sSingelton;
  private static volatile object sSingeltonLock = new object();
  private Texture2D mTexture;
  private ItemPack[] mItemPacks;
  private MagickPack[] mMagickPacks;
  private IPack[] mAllPacks;

  public static PackMan Instance
  {
    get
    {
      if (PackMan.sSingelton == null)
      {
        lock (PackMan.sSingeltonLock)
        {
          if (PackMan.sSingelton == null)
            PackMan.sSingelton = new PackMan();
        }
      }
      return PackMan.sSingelton;
    }
  }

  public event Action<object, bool> PackEnabledChanged;

  private PackMan()
  {
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/PackThumbs");
    List<ItemPack> itemPackList = new List<ItemPack>();
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_vanilla_sticks", "#pack_vanilla_sticksd", this.mTexture, new Point(2, 0), 0U, HashTable.VanillaSticks));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_ward_staffs", "#pack_ward_staffsd", this.mTexture, new Point(7, 0), 0U, HashTable.WardStaffs));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_purple_pack", "#pack_purple_packd", this.mTexture, new Point(8, 0), 0U, HashTable.PurplePack));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_meaky_peaky", "#pack_meaky_peakyd", this.mTexture, new Point(6, 0), 0U, HashTable.MeakyPeaky));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_heavy_hitters", "#pack_heavy_hittersd", this.mTexture, new Point(5, 0), 0U, HashTable.HeavyHitters));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_power_pack", "#pack_power_packd", this.mTexture, new Point(4, 0), 0U, HashTable.PowerPack));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_vietnam_gear", "#pack_vietnam_geard", this.mTexture, new Point(3, 0), 42918U, HashTable.VietnamItems));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_vanilla_weapons", "#pack_vanilla_weaponsd", this.mTexture, new Point(1, 0), 0U, HashTable.VanillaWeapons));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_holiday_spirit", "#pack_holiday_spiritd", this.mTexture, new Point(14, 0), 73091U, HashTable.HolidayItems));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_horror_props", "#pack_horror_propsd", this.mTexture, new Point(15, 0), 73092U, HashTable.HorrorItems));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_gadgets", "#pack_gadgetsd", this.mTexture, new Point(1, 1), 73097U, HashTable.GadgetPack));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#pack_heirlooms", "#pack_heirloomsd", this.mTexture, new Point(2, 1), 73098U, HashTable.HeirloomPack));
    itemPackList.Add(new ItemPack(itemPackList.Count, "#PACK_SPORTING_GOODS", "#PACK_SPORTING_GOODSD", this.mTexture, new Point(3, 1), 42910U, HashTable.SportPack));
    List<MagickPack> magickPackList = new List<MagickPack>();
    magickPackList.Add(new MagickPack(magickPackList.Count, "#mpack_basic", "#mpack_basicd", this.mTexture, new Point(11, 0), 0U, new MagickType[12]
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
    magickPackList.Add(new MagickPack(magickPackList.Count, "#mpack_advanced", "#mpack_advancedd", this.mTexture, new Point(12, 0), 0U, new MagickType[5]
    {
      MagickType.Fear,
      MagickType.ThunderB,
      MagickType.Tornado,
      MagickType.Blizzard,
      MagickType.Conflagration
    }));
    magickPackList.Add(new MagickPack(magickPackList.Count, "#mpack_too_advanced", "#mpack_too_advancedd", this.mTexture, new Point(13, 0), 0U, new MagickType[6]
    {
      MagickType.CTD,
      MagickType.SDeath,
      MagickType.SUndead,
      MagickType.SElemental,
      MagickType.Vortex,
      MagickType.ThunderS
    }));
    magickPackList.Add(new MagickPack(magickPackList.Count, "#mpack_napalm", "#mpack_napalmd", this.mTexture, new Point(9, 0), 42918U, new MagickType[1]
    {
      MagickType.Napalm
    }));
    magickPackList.Add(new MagickPack(magickPackList.Count, "#mpack_meteor_shower", "#mpack_meteor_showerd", this.mTexture, new Point(10, 0), 73030U, new MagickType[1]
    {
      MagickType.MeteorS
    }));
    magickPackList.Add(new MagickPack(magickPackList.Count, "#MPACK_ATHLETIC_TECHNIQUES", "#MPACK_ATHLETIC_TECHNIQUESD", this.mTexture, new Point(4, 1), 0U, new MagickType[3]
    {
      MagickType.JudgementSpray,
      MagickType.PerformanceEnchantment,
      MagickType.Wave
    }));
    this.mItemPacks = itemPackList.ToArray();
    this.mMagickPacks = magickPackList.ToArray();
    this.mAllPacks = new IPack[this.mItemPacks.Length + this.mMagickPacks.Length];
    for (int index = 0; index < this.mItemPacks.Length; ++index)
      this.mAllPacks[index] = (IPack) this.mItemPacks[index];
    for (int index = 0; index < this.mMagickPacks.Length; ++index)
      this.mAllPacks[index + this.mItemPacks.Length] = (IPack) this.mMagickPacks[index];
    SteamAPI.DlcInstalled += new Action<DlcInstalled>(this.SteamAPI_DlcInstalled);
    this.SteamAPI_DlcInstalled(new DlcInstalled());
  }

  private void SteamAPI_DlcInstalled(DlcInstalled obj)
  {
    Magicka.Game.Instance.AddLoadTask(new Action(this.UpdatePackLicense));
  }

  internal void UpdatePackLicense()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    for (int index = 0; index < this.mItemPacks.Length; ++index)
      this.mItemPacks[index].License = HackHelper.CheckLicense(this.mItemPacks[index]);
    for (int index = 0; index < this.mMagickPacks.Length; ++index)
      this.mMagickPacks[index].License = HackHelper.CheckLicense(this.mMagickPacks[index]);
  }

  internal void OnPackEnabledChanged(object iSender, bool iEnabled)
  {
    if (this.PackEnabledChanged == null)
      return;
    this.PackEnabledChanged(iSender, iEnabled);
  }

  public Texture2D Thumbnails => this.mTexture;

  public ItemPack[] ItemPacks => this.mItemPacks;

  public MagickPack[] MagickPacks => this.mMagickPacks;

  public IPack[] AllPacks => this.mAllPacks;

  public ItemPack GetItemPackByHandle(int iID)
  {
    return iID < 0 || iID >= this.mItemPacks.Length ? (ItemPack) null : this.mItemPacks[iID];
  }

  public MagickPack GetMagickPackByHandle(int iID)
  {
    return iID < 0 || iID >= this.mMagickPacks.Length ? (MagickPack) null : this.mMagickPacks[iID];
  }
}
