using System;
using System.IO;
using Magicka.GameLogic;

namespace Magicka.Network
{
	// Token: 0x02000219 RID: 537
	internal struct GamerChangedMessage : ISendable
	{
		// Token: 0x0600111A RID: 4378 RVA: 0x0006A4C8 File Offset: 0x000686C8
		public GamerChangedMessage(Player iPlayer)
		{
			this.Id = (byte)iPlayer.ID;
			this.GamerTag = iPlayer.Gamer.GamerTag;
			this.Color = iPlayer.Gamer.Color;
			this.AvatarThumb = iPlayer.Gamer.Avatar.ThumbPath;
			this.AvatarPortrait = iPlayer.Gamer.Avatar.PortraitPath;
			this.AvatarType = iPlayer.Gamer.Avatar.TypeName;
			this.UnlockedMagicks = iPlayer.UnlockedMagicks;
			this.AvatarAllowCampaign = iPlayer.Gamer.Avatar.AllowCampaign;
			this.AvatarAllowChallenge = iPlayer.Gamer.Avatar.AllowChallenge;
			this.AvatarAllowPVP = iPlayer.Gamer.Avatar.AllowPVP;
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x0600111B RID: 4379 RVA: 0x0006A594 File Offset: 0x00068794
		public PacketType PacketType
		{
			get
			{
				return PacketType.GamerChanged;
			}
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x0006A598 File Offset: 0x00068798
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.GamerTag);
			iWriter.Write(this.AvatarThumb);
			iWriter.Write(this.AvatarPortrait);
			iWriter.Write(this.AvatarType);
			iWriter.Write(this.UnlockedMagicks);
			iWriter.Write(this.Color);
			iWriter.Write(this.Id);
			iWriter.Write(this.AvatarAllowCampaign);
			iWriter.Write(this.AvatarAllowChallenge);
			iWriter.Write(this.AvatarAllowPVP);
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x0006A620 File Offset: 0x00068820
		public void Read(BinaryReader iReader)
		{
			this.GamerTag = iReader.ReadString();
			this.AvatarThumb = iReader.ReadString();
			this.AvatarPortrait = iReader.ReadString();
			this.AvatarType = iReader.ReadString();
			this.UnlockedMagicks = iReader.ReadUInt64();
			this.Color = iReader.ReadByte();
			this.Id = iReader.ReadByte();
			this.AvatarAllowCampaign = iReader.ReadBoolean();
			this.AvatarAllowChallenge = iReader.ReadBoolean();
			this.AvatarAllowPVP = iReader.ReadBoolean();
		}

		// Token: 0x04000FE0 RID: 4064
		public string GamerTag;

		// Token: 0x04000FE1 RID: 4065
		public string AvatarThumb;

		// Token: 0x04000FE2 RID: 4066
		public string AvatarPortrait;

		// Token: 0x04000FE3 RID: 4067
		public string AvatarType;

		// Token: 0x04000FE4 RID: 4068
		public ulong UnlockedMagicks;

		// Token: 0x04000FE5 RID: 4069
		public byte Color;

		// Token: 0x04000FE6 RID: 4070
		public byte Id;

		// Token: 0x04000FE7 RID: 4071
		public bool AvatarAllowCampaign;

		// Token: 0x04000FE8 RID: 4072
		public bool AvatarAllowChallenge;

		// Token: 0x04000FE9 RID: 4073
		public bool AvatarAllowPVP;
	}
}
