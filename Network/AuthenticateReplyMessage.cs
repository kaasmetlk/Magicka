// Decompiled with JetBrains decompiler
// Type: Magicka.Network.AuthenticateReplyMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Levels.Packs;
using Magicka.Levels.Versus;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct AuthenticateReplyMessage : ISendable
{
  public AuthenticateReplyMessage.Reply Response;
  public GameInfoMessage GameInfo;
  public VersusRuleset.Settings.OptionsMessage VersusSettings;
  public PackOptionsMessage PackOptions;

  public PacketType PacketType => PacketType.Authenticate;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write((int) this.Response);
    if (this.GameInfo.GameName != null)
    {
      iWriter.Write(true);
      this.GameInfo.Write(iWriter);
      if (this.GameInfo.GameType == GameType.Versus)
      {
        this.VersusSettings.Write(iWriter);
        this.PackOptions.Write(iWriter);
      }
      else
      {
        if (this.GameInfo.GameType != GameType.Challenge)
          return;
        this.PackOptions.Write(iWriter);
      }
    }
    else
      iWriter.Write(false);
  }

  public void Read(BinaryReader iReader)
  {
    this.Response = (AuthenticateReplyMessage.Reply) iReader.ReadInt32();
    if (!iReader.ReadBoolean())
      return;
    this.GameInfo.Read(iReader);
    if (this.GameInfo.GameType == GameType.Versus)
    {
      this.VersusSettings.Read(iReader);
      this.PackOptions.Read(iReader);
    }
    else
    {
      if (this.GameInfo.GameType != GameType.Challenge)
        return;
      this.PackOptions.Read(iReader);
    }
  }

  public enum Reply
  {
    Invalid,
    Ok,
    Error_ServerFull,
    Error_AuthFailed,
    Error_GamePlaying,
    Error_Version,
    Error_Password,
  }
}
