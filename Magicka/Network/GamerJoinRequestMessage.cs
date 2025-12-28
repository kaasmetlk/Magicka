using System;
using System.IO;
using SteamWrapper;

namespace Magicka.Network
{
	// Token: 0x02000217 RID: 535
	internal struct GamerJoinRequestMessage : ISendable
	{
		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06001114 RID: 4372 RVA: 0x0006A3B6 File Offset: 0x000685B6
		public PacketType PacketType
		{
			get
			{
				return (PacketType)139;
			}
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x0006A3C0 File Offset: 0x000685C0
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Gamer);
			iWriter.Write(this.AvatarThumb);
			iWriter.Write(this.AvatarPortrait);
			iWriter.Write(this.AvatarType);
			iWriter.Write(this.Id);
			iWriter.Write(this.Color);
			iWriter.Write(this.SteamID.AsUInt64);
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x0006A428 File Offset: 0x00068628
		public void Read(BinaryReader iReader)
		{
			this.Gamer = iReader.ReadString();
			this.AvatarThumb = iReader.ReadString();
			this.AvatarPortrait = iReader.ReadString();
			this.AvatarType = iReader.ReadString();
			this.Id = iReader.ReadSByte();
			this.Color = iReader.ReadByte();
			this.SteamID = new SteamID(iReader.ReadUInt64());
		}

		// Token: 0x04000FD7 RID: 4055
		public string Gamer;

		// Token: 0x04000FD8 RID: 4056
		public string AvatarThumb;

		// Token: 0x04000FD9 RID: 4057
		public string AvatarPortrait;

		// Token: 0x04000FDA RID: 4058
		public string AvatarType;

		// Token: 0x04000FDB RID: 4059
		public sbyte Id;

		// Token: 0x04000FDC RID: 4060
		public byte Color;

		// Token: 0x04000FDD RID: 4061
		public SteamID SteamID;
	}
}
