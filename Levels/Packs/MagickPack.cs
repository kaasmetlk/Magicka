// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Packs.MagickPack
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Levels.Packs;

internal class MagickPack : IPack
{
  private HackHelper.License mLicense = HackHelper.License.Yes;
  private bool mEnabled = true;
  private bool mIsUsed;
  private int mID;
  private string mName;
  private string mDescription;
  private int mNameID;
  private int mDescID;
  private Vector2 mThumbOffset;
  private uint mAppID;
  private MagickType[] mMagicks;

  public MagickPack(
    int iID,
    string iName,
    string iDesc,
    Texture2D iThumbTexture,
    Point iThumb,
    uint iAppID,
    MagickType[] iMagicks)
  {
    this.mID = iID;
    this.mAppID = iAppID;
    this.mNameID = iName.ToLowerInvariant().GetHashCodeCustom();
    this.mDescID = iDesc.ToLowerInvariant().GetHashCodeCustom();
    this.mThumbOffset.X = 64f * (float) iThumb.X / (float) iThumbTexture.Width;
    this.mThumbOffset.Y = 64f * (float) iThumb.Y / (float) iThumbTexture.Height;
    this.mMagicks = iMagicks;
    this.mIsUsed = DLC_StatusHelper.Instance.Item_IsUnused(nameof (MagickPack), this.mName, this.mAppID, false);
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
    DLC_StatusHelper.Instance.Item_TrySetUsed(nameof (MagickPack), this.mName, forceSave);
    this.mIsUsed = true;
  }

  public int ID => this.mID;

  public int Name => this.mNameID;

  public int Descritpion => this.mDescID;

  public uint StoreURL => this.mAppID;

  public MagickType[] Magicks => this.mMagicks;

  public Vector2 ThumbOffset => this.mThumbOffset;
}
