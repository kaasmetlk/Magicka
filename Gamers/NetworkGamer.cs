// Decompiled with JetBrains decompiler
// Type: Magicka.Gamers.NetworkGamer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Graphics;
using SteamWrapper;

#nullable disable
namespace Magicka.Gamers;

internal class NetworkGamer : Gamer
{
  private SteamID mClientID;

  public SteamID ClientID
  {
    get => this.mClientID;
    set => this.mClientID = value;
  }

  public NetworkGamer(
    string iGamerTag,
    byte iColor,
    string iAvatarThumb,
    string iAvatarType,
    SteamID iClientID)
    : base(iGamerTag)
  {
    this.mAvatar.ThumbPath = iAvatarThumb;
    this.mAvatar.Thumb = Game.Instance.Content.Load<Texture2D>(this.mAvatar.ThumbPath);
    this.mAvatar.TypeName = iAvatarType;
    this.mAvatar.Type = this.mAvatar.TypeName.GetHashCodeCustom();
    this.mClientID = iClientID;
    this.mColor = iColor;
  }
}
