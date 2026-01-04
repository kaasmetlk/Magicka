// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Packs.ItemPack
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

#nullable disable
namespace Magicka.Levels.Packs;

internal class ItemPack : IPack
{
  private HackHelper.License mLicense = HackHelper.License.Yes;
  private bool mEnabled = true;
  private bool mIsUsed;
  private int mID;
  private string mName;
  private string mDescription;
  private Vector2 mThumbOffset;
  private uint mAppID;
  private int mNameID;
  private int mDescID;
  private string[] mItems;
  private int[] mItemIDs;

  public ItemPack(
    int iID,
    string iName,
    string iDesc,
    Texture2D iThumbTexture,
    Point iThumb,
    uint iAppID,
    string[] iItems)
  {
    this.mID = iID;
    this.mAppID = iAppID;
    this.mName = iName;
    this.mDescription = iDesc;
    this.mNameID = iName.ToLowerInvariant().GetHashCodeCustom();
    this.mDescID = iDesc.ToLowerInvariant().GetHashCodeCustom();
    this.mThumbOffset.X = 64f * (float) iThumb.X / (float) iThumbTexture.Width;
    this.mThumbOffset.Y = 64f * (float) iThumb.Y / (float) iThumbTexture.Height;
    this.mItems = new string[iItems.Length];
    this.mItemIDs = new int[iItems.Length];
    for (int index = 0; index < this.mItems.Length; ++index)
    {
      string lowerInvariant = iItems[index].ToLowerInvariant();
      this.mItems[index] = lowerInvariant.Substring("content".Length + 1, lowerInvariant.Length - 4 - ("content".Length + 1));
      string withoutExtension = Path.GetFileNameWithoutExtension(lowerInvariant);
      this.mItemIDs[index] = withoutExtension.GetHashCodeCustom();
    }
    this.mIsUsed = DLC_StatusHelper.Instance.Item_IsUnused(nameof (ItemPack), this.mName, this.mAppID, false);
  }

  public HackHelper.License License
  {
    get => this.mLicense;
    set
    {
      this.mLicense = value;
      if (value == HackHelper.License.Yes)
        return;
      this.mEnabled = false;
    }
  }

  public bool Enabled
  {
    get => this.mEnabled;
    set
    {
      bool mEnabled = this.mEnabled;
      this.mEnabled = value & this.mLicense == HackHelper.License.Yes;
      if (mEnabled == this.mEnabled)
        return;
      PackMan.Instance.OnPackEnabledChanged((object) this, this.mEnabled);
    }
  }

  public bool IsUsed => this.mIsUsed;

  public void SetUsed(bool forceSave)
  {
    if (this.mLicense != HackHelper.License.Yes)
      return;
    DLC_StatusHelper.Instance.Item_TrySetUsed(nameof (ItemPack), this.mName, forceSave);
    this.mIsUsed = true;
  }

  public uint StoreURL => this.mAppID;

  public int ID => this.mID;

  public int Name => this.mNameID;

  public int Descritpion => this.mDescID;

  public string[] Items => this.mItems;

  public int[] ItemIDs => this.mItemIDs;

  public Vector2 ThumbOffset => this.mThumbOffset;
}
