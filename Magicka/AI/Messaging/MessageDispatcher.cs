using System;
using System.Collections.Generic;

namespace Magicka.AI.Messaging
{
	// Token: 0x0200040A RID: 1034
	internal class MessageDispatcher
	{
		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x06001FE6 RID: 8166 RVA: 0x000DFCC4 File Offset: 0x000DDEC4
		public static MessageDispatcher Instance
		{
			get
			{
				if (MessageDispatcher.sSingleton == null)
				{
					MessageDispatcher.sSingleton = new MessageDispatcher();
				}
				return MessageDispatcher.sSingleton;
			}
		}

		// Token: 0x06001FE7 RID: 8167 RVA: 0x000DFCDC File Offset: 0x000DDEDC
		private void MessageDischarge(Agent iReciever, ref Message iMessage)
		{
			iReciever.HandleMessage(iMessage);
		}

		// Token: 0x06001FE8 RID: 8168 RVA: 0x000DFCEC File Offset: 0x000DDEEC
		public void DischargeDelayedMessages(float iDeltaTime)
		{
			this.mCurrentTime += iDeltaTime;
			if (this.mPrioQueue.Count > 0)
			{
				Message message;
				while (this.mPrioQueue.Count > 0 && (message = this.mPrioQueue.Values[0]).DispatchTime < this.mCurrentTime && message.DispatchTime > 0f)
				{
					this.MessageDischarge(message.Reciever, ref message);
					this.mPrioQueue.RemoveAt(0);
				}
			}
			if (this.mPrioQueue.Count == 0)
			{
				this.mCurrentTime = 0f;
			}
		}

		// Token: 0x06001FE9 RID: 8169 RVA: 0x000DFD88 File Offset: 0x000DDF88
		public void Flush()
		{
			while (this.mPrioQueue.Count > 0)
			{
				Message message = this.mPrioQueue.Values[0];
				this.MessageDischarge(message.Reciever, ref message);
				this.mPrioQueue.RemoveAt(0);
			}
			this.mCurrentTime = 0f;
		}

		// Token: 0x06001FEA RID: 8170 RVA: 0x000DFDE0 File Offset: 0x000DDFE0
		public void DispatchMessage(float iDelay, Agent iSender, Agent iReciever, MessageType iMessage, float iValue, object iTag)
		{
			Message message = new Message(iSender, iReciever, iMessage, iDelay, iValue, iTag);
			if (iDelay <= 1E-45f)
			{
				this.MessageDischarge(iReciever, ref message);
				return;
			}
			message.DispatchTime = this.mCurrentTime + iDelay;
			this.AddToQueue(ref message);
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x000DFE2B File Offset: 0x000DE02B
		private void AddToQueue(ref Message iMessage)
		{
			if (!this.mPrioQueue.ContainsValue(iMessage) && !this.mPrioQueue.ContainsKey(iMessage.DispatchTime))
			{
				this.mPrioQueue.Add(iMessage.DispatchTime, iMessage);
			}
		}

		// Token: 0x0400222C RID: 8748
		private static MessageDispatcher sSingleton;

		// Token: 0x0400222D RID: 8749
		private float mCurrentTime;

		// Token: 0x0400222E RID: 8750
		private SortedList<float, Message> mPrioQueue = new SortedList<float, Message>(1024);
	}
}
