// Decompiled with JetBrains decompiler
// Type: Magicka.AI.Messaging.Message
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.AI.Messaging;

public struct Message(
  Agent iSender,
  Agent iReciever,
  MessageType iMessage,
  float iDispatchTime,
  float iValue,
  object iTag)
{
  public Agent Sender = iSender;
  public Agent Reciever = iReciever;
  public MessageType MessageType = iMessage;
  public float DispatchTime = iDispatchTime;
  public float Value = iValue;
  public object Tag = iTag;
}
