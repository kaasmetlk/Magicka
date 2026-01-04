// Decompiled with JetBrains decompiler
// Type: Magicka.Network.GamerChangedMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct GamerChangedMessage : ISendable
{
  public string GamerTag;
  public string AvatarThumb;
  public string AvatarPortrait;
  public string AvatarType;
  public ulong UnlockedMagicks;
  public byte Color;
  public byte Id;
  public bool AvatarAllowCampaign;
  public bool AvatarAllowChallenge;
  public bool AvatarAllowPVP;

  public GamerChangedMessage(Player iPlayer)
  {
    this.Id = (byte) iPlayer.ID;
    this.GamerTag = iPlayer.Gamer.GamerTag;
    this.Color = iPlayer.Gamer.Color;
    this.AvatarThumb = iPlayer.Gamer.Avatar.ThumbPath;
    this.AvatarPortrait = iPlayer.Gamer.Avatar.PortraitPath;
    this.AvatarType = iPlayer.Gamer.Avatar.TypeName;
    this.UnlockedMagicks = iPlayer.UnlockedMagicks;
    this.AvatarAllowCampaign = iPlayer.Gamer.Avatar.AllowCampaign;
    this.AvatarAllowChallenge = iPlayer.Gamer.Avatar.AllowChallenge;
    this.AvatarAllowPVP = iPlayer.Gamer.Avatar.AllowPVP;
  }

  public PacketType PacketType => PacketType.GamerChanged;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.GamerTag);
    iWriter.Write(this.AvatarThumb);
    iWriter.Write(this.AvatarPortrait);
    iWriter.Write(this.AvatarType);
    iWriter.Write(this.UnlockedMagicks);
    iWriter.Write(this.Color);
    iWriter.Write(this.Id);
    iWriter.Write(this.AvatarAllowCampaign);
    iWriter.Write(this.AvatarAllowChallenge);
    iWriter.Write(this.AvatarAllowPVP);
  }

  public void Read(BinaryReader iReader)
  {
    this.GamerTag = iReader.ReadString();
    this.AvatarThumb = iReader.ReadString();
    this.AvatarPortrait = iReader.ReadString();
    this.AvatarType = iReader.ReadString();
    this.UnlockedMagicks = iReader.ReadUInt64();
    this.Color = iReader.ReadByte();
    this.Id = iReader.ReadByte();
    this.AvatarAllowCampaign = iReader.ReadBoolean();
    this.AvatarAllowChallenge = iReader.ReadBoolean();
    this.AvatarAllowPVP = iReader.ReadBoolean();
  }
}
