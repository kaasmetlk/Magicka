// Decompiled with JetBrains decompiler
// Type: Magicka.Network.ChatMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct ChatMessage : ISendable
{
  public string Sender;
  public string Message;

  public PacketType PacketType => PacketType.ChatMessage;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Sender);
    iWriter.Write(this.Message);
  }

  public void Read(BinaryReader iReader)
  {
    this.Sender = iReader.ReadString();
    this.Message = iReader.ReadString();
  }

  public override string ToString()
  {
    Vector4 dialogueColorDefault = Defines.DIALOGUE_COLOR_DEFAULT;
    dialogueColorDefault.X /= 0.7037f;
    dialogueColorDefault.Y /= 0.7037f;
    dialogueColorDefault.Z /= 0.7037f;
    IFormatProvider numberFormat = (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat;
    return $"[c={dialogueColorDefault.X.ToString(numberFormat)},{dialogueColorDefault.Y.ToString(numberFormat)},{dialogueColorDefault.Z.ToString(numberFormat)},{dialogueColorDefault.W.ToString(numberFormat)}]{this.Sender}:[/c] {this.Message.Replace("[", "[[")}";
  }
}
