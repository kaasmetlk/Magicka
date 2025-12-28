using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000231 RID: 561
	internal struct ThreatMessage : ISendable
	{
		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001154 RID: 4436 RVA: 0x0006B911 File Offset: 0x00069B11
		public PacketType PacketType
		{
			get
			{
				return PacketType.Threat;
			}
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x0006B915 File Offset: 0x00069B15
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Threat);
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x0006B923 File Offset: 0x00069B23
		public void Read(BinaryReader iReader)
		{
			this.Threat = iReader.ReadBoolean();
		}

		// Token: 0x04001050 RID: 4176
		public bool Threat;
	}
}
