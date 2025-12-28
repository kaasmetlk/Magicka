using System;

namespace Magicka.AI.Messaging
{
	// Token: 0x020005BC RID: 1468
	public struct Message
	{
		// Token: 0x06002BD7 RID: 11223 RVA: 0x0015A70F File Offset: 0x0015890F
		public Message(Agent iSender, Agent iReciever, MessageType iMessage, float iDispatchTime, float iValue, object iTag)
		{
			this.Sender = iSender;
			this.Reciever = iReciever;
			this.MessageType = iMessage;
			this.DispatchTime = iDispatchTime;
			this.Value = iValue;
			this.Tag = iTag;
		}

		// Token: 0x04002F92 RID: 12178
		public Agent Sender;

		// Token: 0x04002F93 RID: 12179
		public Agent Reciever;

		// Token: 0x04002F94 RID: 12180
		public MessageType MessageType;

		// Token: 0x04002F95 RID: 12181
		public float DispatchTime;

		// Token: 0x04002F96 RID: 12182
		public float Value;

		// Token: 0x04002F97 RID: 12183
		public object Tag;
	}
}
