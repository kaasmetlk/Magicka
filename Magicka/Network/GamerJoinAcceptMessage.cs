using System;
using System.IO;

namespace Magicka.Network
{
	// Token: 0x02000218 RID: 536
	internal struct GamerJoinAcceptMessage : ISendable
	{
		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06001117 RID: 4375 RVA: 0x0006A48E File Offset: 0x0006868E
		public PacketType PacketType
		{
			get
			{
				return PacketType.GamerJoin;
			}
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x0006A492 File Offset: 0x00068692
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Gamer);
			iWriter.Write(this.Id);
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x0006A4AC File Offset: 0x000686AC
		public void Read(BinaryReader iReader)
		{
			this.Gamer = iReader.ReadString();
			this.Id = iReader.ReadSByte();
		}

		// Token: 0x04000FDE RID: 4062
		public string Gamer;

		// Token: 0x04000FDF RID: 4063
		public sbyte Id;
	}
}
