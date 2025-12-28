using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000229 RID: 553
	internal struct CharacterDieMessage : ISendable
	{
		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06001142 RID: 4418 RVA: 0x0006B2DB File Offset: 0x000694DB
		public PacketType PacketType
		{
			get
			{
				return PacketType.CharacterDie;
			}
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x0006B2DF File Offset: 0x000694DF
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.Overkill);
			iWriter.Write(this.Drown);
			iWriter.Write(this.KillerHandle);
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x0006B311 File Offset: 0x00069511
		public void Read(BinaryReader iReader)
		{
			this.Handle = iReader.ReadUInt16();
			this.Overkill = iReader.ReadBoolean();
			this.Drown = iReader.ReadBoolean();
			this.KillerHandle = iReader.ReadUInt16();
		}

		// Token: 0x04001028 RID: 4136
		public ushort Handle;

		// Token: 0x04001029 RID: 4137
		public bool Overkill;

		// Token: 0x0400102A RID: 4138
		public bool Drown;

		// Token: 0x0400102B RID: 4139
		public ushort KillerHandle;
	}
}
