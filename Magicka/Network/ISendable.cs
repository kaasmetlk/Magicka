using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x0200009A RID: 154
	internal interface ISendable
	{
		// Token: 0x170000AA RID: 170
		// (get) Token: 0x0600048E RID: 1166
		PacketType PacketType { get; }

		// Token: 0x0600048F RID: 1167
		void Write(BinaryWriter iWriter);

		// Token: 0x06000490 RID: 1168
		void Read(BinaryReader iReader);
	}
}
