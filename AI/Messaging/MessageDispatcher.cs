// Decompiled with JetBrains decompiler
// Type: Magicka.AI.Messaging.MessageDispatcher
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Collections.Generic;

#nullable disable
namespace Magicka.AI.Messaging;

internal class MessageDispatcher
{
  private static MessageDispatcher sSingleton;
  private float mCurrentTime;
  private SortedList<float, Message> mPrioQueue = new SortedList<float, Message>(1024 /*0x0400*/);

  public static MessageDispatcher Instance
  {
    get
    {
      if (MessageDispatcher.sSingleton == null)
        MessageDispatcher.sSingleton = new MessageDispatcher();
      return MessageDispatcher.sSingleton;
    }
  }

  private void MessageDischarge(Agent iReciever, ref Message iMessage)
  {
    iReciever.HandleMessage(iMessage);
  }

  public void DischargeDelayedMessages(float iDeltaTime)
  {
    this.mCurrentTime += iDeltaTime;
    if (this.mPrioQueue.Count > 0)
    {
      Message iMessage;
      while (this.mPrioQueue.Count > 0 && (double) (iMessage = this.mPrioQueue.Values[0]).DispatchTime < (double) this.mCurrentTime && (double) iMessage.DispatchTime > 0.0)
      {
        this.MessageDischarge(iMessage.Reciever, ref iMessage);
        this.mPrioQueue.RemoveAt(0);
      }
    }
    if (this.mPrioQueue.Count != 0)
      return;
    this.mCurrentTime = 0.0f;
  }

  public void Flush()
  {
    while (this.mPrioQueue.Count > 0)
    {
      Message iMessage = this.mPrioQueue.Values[0];
      this.MessageDischarge(iMessage.Reciever, ref iMessage);
      this.mPrioQueue.RemoveAt(0);
    }
    this.mCurrentTime = 0.0f;
  }

  public void DispatchMessage(
    float iDelay,
    Agent iSender,
    Agent iReciever,
    MessageType iMessage,
    float iValue,
    object iTag)
  {
    Message iMessage1 = new Message(iSender, iReciever, iMessage, iDelay, iValue, iTag);
    if ((double) iDelay <= 1.4012984643248171E-45)
    {
      this.MessageDischarge(iReciever, ref iMessage1);
    }
    else
    {
      iMessage1.DispatchTime = this.mCurrentTime + iDelay;
      this.AddToQueue(ref iMessage1);
    }
  }

  private void AddToQueue(ref Message iMessage)
  {
    if (this.mPrioQueue.ContainsValue(iMessage) || this.mPrioQueue.ContainsKey(iMessage.DispatchTime))
      return;
    this.mPrioQueue.Add(iMessage.DispatchTime, iMessage);
  }
}
