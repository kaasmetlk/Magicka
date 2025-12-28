using System;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;

namespace Magicka.Network
{
	// Token: 0x02000216 RID: 534
	internal struct ChatMessage : ISendable
	{
		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06001110 RID: 4368 RVA: 0x0006A2B2 File Offset: 0x000684B2
		public PacketType PacketType
		{
			get
			{
				return PacketType.ChatMessage;
			}
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x0006A2B6 File Offset: 0x000684B6
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Sender);
			iWriter.Write(this.Message);
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x0006A2D0 File Offset: 0x000684D0
		public void Read(BinaryReader iReader)
		{
			this.Sender = iReader.ReadString();
			this.Message = iReader.ReadString();
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x0006A2EC File Offset: 0x000684EC
		public override string ToString()
		{
			Vector4 dialogue_COLOR_DEFAULT = Defines.DIALOGUE_COLOR_DEFAULT;
			dialogue_COLOR_DEFAULT.X /= 0.7037f;
			dialogue_COLOR_DEFAULT.Y /= 0.7037f;
			dialogue_COLOR_DEFAULT.Z /= 0.7037f;
			IFormatProvider numberFormat = CultureInfo.InvariantCulture.NumberFormat;
			return string.Format("[c={0},{1},{2},{3}]{4}:[/c] {5}", new object[]
			{
				dialogue_COLOR_DEFAULT.X.ToString(numberFormat),
				dialogue_COLOR_DEFAULT.Y.ToString(numberFormat),
				dialogue_COLOR_DEFAULT.Z.ToString(numberFormat),
				dialogue_COLOR_DEFAULT.W.ToString(numberFormat),
				this.Sender,
				this.Message.Replace("[", "[[")
			});
		}

		// Token: 0x04000FD5 RID: 4053
		public string Sender;

		// Token: 0x04000FD6 RID: 4054
		public string Message;
	}
}
